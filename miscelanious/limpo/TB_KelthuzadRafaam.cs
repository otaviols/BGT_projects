using System.Collections;
using UnityEngine;

public class TB_KelthuzadRafaam : MissionEntity
{
  private Actor m_kelthuzadActor;
  private bool once = true;

  public override void PreloadAssets() => this.PreloadSound("KT_Minions_Servants.prefab:128dc3329f23b2b439500351e9a1ec72");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_KelthuzadRafaam tbKelthuzadRafaam = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    foreach (Entity entity in GameState.Get().GetPlayerMap().Values)
    {
      Entity hero = entity.GetHero();
      Card card = hero.GetCard();
      if (hero.GetCardId() == "TB_KTRAF_H_1")
        tbKelthuzadRafaam.m_kelthuzadActor = card.GetActor();
    }
    if (missionEvent == 3)
    {
      Debug.Log((object) "mission event 3");
      if (tbKelthuzadRafaam.once)
      {
        tbKelthuzadRafaam.once = false;
        GameState.Get().SetBusy(true);
        Notification.SpeechBubbleDirection direction = tbKelthuzadRafaam.m_kelthuzadActor.GetEntity().IsControlledByFriendlySidePlayer() ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopRight;
        Gameplay.Get().StartCoroutine(tbKelthuzadRafaam.PlaySoundAndBlockSpeechWithCustomGameString("KT_Minions_Servants.prefab:128dc3329f23b2b439500351e9a1ec72", "VO_EMOTE_HERO_TBKTRAF_KT_MINIONS_SERVANTS", direction, tbKelthuzadRafaam.m_kelthuzadActor));
        GameState.Get().SetBusy(false);
      }
    }
    return false;
  }

  public TB_KelthuzadRafaam()
    : base()
  {
  }
}
