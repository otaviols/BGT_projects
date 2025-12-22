using System.Collections;
using UnityEngine;

public class TB02_CoOp : MissionEntity
{
  private Card m_bossCard;

  private void SetUpBossCard()
  {
    if (!((Object) this.m_bossCard == (Object) null))
      return;
    int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_1);
    Entity entity = GameState.Get().GetEntity(tag);
    if (entity == null)
      return;
    this.m_bossCard = entity.GetCard();
  }

  public override void PreloadAssets()
  {
    this.PreloadSound("FX_MinionSummon_Cast.prefab:d0a0997a72042914f8779e138bb2755e");
    this.PreloadSound("CleanMechSmall_Trigger_Underlay.prefab:c943e7c65e3196d48a630fb118a2458b");
    this.PreloadSound("CleanMechLarge_Play_Underlay.prefab:ba8c1d07706f9284b8013f05e8d1f664");
    this.PreloadSound("CleanMechLarge_Death_Underlay.prefab:a7cad6027a0d13444a1ad0c49e6f7f23");
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB02_CoOp tb02CoOp = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    tb02CoOp.SetUpBossCard();
    if ((Object) tb02CoOp.m_bossCard == (Object) null || turn != 1)
      return false;
    Gameplay.Get().StartCoroutine(tb02CoOp.PlaySoundAndBlockSpeechWithCustomGameString("CleanMechSmall_Trigger_Underlay.prefab:c943e7c65e3196d48a630fb118a2458b", "VO_COOP02_00", Notification.SpeechBubbleDirection.TopRight, tb02CoOp.m_bossCard.GetActor()));
    return false;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB02_CoOp tb02CoOp = this;
    while (tb02CoOp.m_enemySpeaking)
      yield return (object) null;
    tb02CoOp.SetUpBossCard();
    if (!((Object) tb02CoOp.m_bossCard == (Object) null))
    {
      switch (missionEvent)
      {
        case 5:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tb02CoOp.PlaySoundAndBlockSpeechWithCustomGameString("CleanMechLarge_Play_Underlay.prefab:ba8c1d07706f9284b8013f05e8d1f664", "VO_COOP02_ABILITY_05", Notification.SpeechBubbleDirection.TopRight, tb02CoOp.m_bossCard.GetActor()));
          GameState.Get().SetBusy(false);
          break;
        case 6:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tb02CoOp.PlaySoundAndBlockSpeechWithCustomGameString("CleanMechLarge_Death_Underlay.prefab:a7cad6027a0d13444a1ad0c49e6f7f23", "VO_COOP02_ABILITY_06", Notification.SpeechBubbleDirection.TopRight, tb02CoOp.m_bossCard.GetActor()));
          GameState.Get().SetBusy(false);
          break;
      }
    }
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public TB02_CoOp()
    : base()
  {
  }
}
