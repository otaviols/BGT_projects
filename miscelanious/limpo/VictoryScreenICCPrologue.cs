using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class VictoryScreenICCPrologue : VictoryScreen
{
  public Animation m_BurnAwayAnimation;
  public AudioSource m_BurnAwayAudio;
  public Renderer m_LichPortraitRenderer;
  public string m_PortraitTextureName;
  private static readonly float TIRION_LINE_DELAY_SEC = 4.5f;
  private static readonly float LICH_BURN_ANIM_SPEED = 0.25f;

  protected override void Awake()
  {
    base.Awake();
    Card heroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    this.m_LichPortraitRenderer.GetMaterial().SetTexture(this.m_PortraitTextureName, heroCard.GetPortraitTexture());
    VictoryTwoScoop twoScoop = this.m_twoScoop as VictoryTwoScoop;
    if ((Object) twoScoop != (Object) null)
      twoScoop.SetOverrideHero(GameState.Get().GetFriendlySidePlayer().GetStartingHero().GetEntityDef());
    else
      Log.Gameplay.PrintError("VictoryScreenICCPrologue.Awake() - m_twoScoop is not an instance of VictoryTwoScoop!");
  }

  protected override void ShowStandardFlow() => this.ShowTwoScoop();

  protected override void OnTwoScoopShown()
  {
    base.OnTwoScoopShown();
    this.StartCoroutine(this.PlayAnim());
  }

  private IEnumerator PlayAnim()
  {
    VictoryScreenICCPrologue screenIccPrologue = this;
    if (GameState.Get().GetGameEntity() is ICC_01_LICHKING missionEntity)
    {
      yield return (object) new WaitForSeconds(VictoryScreenICCPrologue.TIRION_LINE_DELAY_SEC);
      while (NotificationManager.Get().IsQuotePlaying)
        yield return (object) null;
      yield return (object) screenIccPrologue.StartCoroutine(missionEntity.PlayTirionVictoryScreenLine());
      if ((Object) screenIccPrologue.m_BurnAwayAudio != (Object) null)
        SoundManager.Get().Play(screenIccPrologue.m_BurnAwayAudio);
      screenIccPrologue.m_BurnAwayAnimation["LichHeroBurnAway"].speed = VictoryScreenICCPrologue.LICH_BURN_ANIM_SPEED;
      screenIccPrologue.m_BurnAwayAnimation.Play("LichHeroBurnAway");
      yield return (object) screenIccPrologue.StartCoroutine(missionEntity.PlayJainaVictoryScreenLine(screenIccPrologue.m_twoScoop.m_heroActor));
    }
    else
      Log.Gameplay.PrintError("VictoryScreenICCPrologue.PlayAnim(): GameEntity is not an instance of ICC_01_LICHKING!.");
    screenIccPrologue.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(((EndGameScreen) screenIccPrologue).ContinueButtonPress_PrevMode));
    if (!(bool) UniversalInputManager.UsePhoneUI)
      screenIccPrologue.m_continueText.gameObject.SetActive(true);
  }
}
