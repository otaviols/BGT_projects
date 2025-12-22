using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class VictoryScreenFinleyBubbleHearth : VictoryScreen
{
  public Animation m_BurnAwayAnimation;
  public AudioSource m_BurnAwayAudio;
  public Renderer m_LichPortraitRenderer;
  public string m_PortraitTextureName;
  private static readonly float FINLEY_LINE_DELAY_SEC = 4.5f;
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
    VictoryScreenFinleyBubbleHearth finleyBubbleHearth = this;
    if (GameState.Get().GetGameEntity() is ICC_01_LICHKING missionEntity)
    {
      yield return (object) new WaitForSeconds(VictoryScreenFinleyBubbleHearth.FINLEY_LINE_DELAY_SEC);
      while (NotificationManager.Get().IsQuotePlaying)
        yield return (object) null;
      yield return (object) finleyBubbleHearth.StartCoroutine(missionEntity.PlayTirionVictoryScreenLine());
      if ((Object) finleyBubbleHearth.m_BurnAwayAudio != (Object) null)
        SoundManager.Get().Play(finleyBubbleHearth.m_BurnAwayAudio);
      finleyBubbleHearth.m_BurnAwayAnimation["LichHeroBurnAway"].speed = VictoryScreenFinleyBubbleHearth.LICH_BURN_ANIM_SPEED;
      finleyBubbleHearth.m_BurnAwayAnimation.Play("LichHeroBurnAway");
      yield return (object) finleyBubbleHearth.StartCoroutine(missionEntity.PlayJainaVictoryScreenLine(finleyBubbleHearth.m_twoScoop.m_heroActor));
    }
    else
      Log.Gameplay.PrintError("VictoryScreenICCPrologue.PlayAnim(): GameEntity is not an instance of ICC_01_LICHKING!.");
    finleyBubbleHearth.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(((EndGameScreen) finleyBubbleHearth).ContinueButtonPress_PrevMode));
    if (!(bool) UniversalInputManager.UsePhoneUI)
      finleyBubbleHearth.m_continueText.gameObject.SetActive(true);
  }
}
