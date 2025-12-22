using System.Collections;
using UnityEngine;

public class TB11_CoOpv3 : MissionEntity
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
    this.PreloadSound("VO_BRMA13_1_RESPONSE_05.prefab:ec4f58f21067dde49b2ee26538259c89");
    this.PreloadSound("VO_NEFARIAN_NEF2_65.prefab:cad99daf56acb69428af9299fe9fb04b");
    this.PreloadSound("VO_BRMA17_1_RESPONSE_85.prefab:c7bbc928438b13241bde42c6578ad5c8");
    this.PreloadSound("VO_BRMA17_1_TRANSFORM1_80.prefab:82475f6129d5587448c3aa398a77c580");
    this.PreloadSound("VO_BRMA17_1_TRANSFORM2_81.prefab:d064be3da78c0f5449db24a40f9a609b");
    this.PreloadSound("VO_BRMA13_1_TURN1_PT1_02.prefab:ac211cc8ab665034da99720e38b6b994");
    this.PreloadSound("VO_BRMA17_1_START_78.prefab:76391ad5bad9fcb4382a2bc98d2765d7");
    this.PreloadSound("VO_BRMA13_1_HP_PRIEST_08.prefab:75d6f8035f037dd43af7c058f318c2fb");
    this.PreloadSound("VO_BRMA13_1_HP_SHAMAN_13.prefab:e248e28c2032e5c4c84490af8596f093");
    this.PreloadSound("VO_Innkeeper_Male_Dwarf_Brawl_01.prefab:283019fef346e8f4688167eb0c3bfb3c");
    this.PreloadSound("VO_Innkeeper_Male_Dwarf_Brawl_02.prefab:a43ebf2271976b447a26d614b80948f0");
    this.PreloadSound("VO_Innkeeper_Male_Dwarf_NEFARIAN_Tavern_Brawl.prefab:5dfeed5d6b1827848999565cb1ef42fa");
  }

  public override AudioSource GetAnnouncerLine(
    Card heroCard,
    Card.AnnouncerLineType type)
  {
    if (heroCard.GetEntity().IsControlledByFriendlySidePlayer())
    {
      switch (Random.Range(0, 2))
      {
        case 0:
          return this.GetPreloadedSound("VO_Innkeeper_Male_Dwarf_Brawl_01.prefab:283019fef346e8f4688167eb0c3bfb3c");
        case 1:
          return this.GetPreloadedSound("VO_Innkeeper_Male_Dwarf_Brawl_02.prefab:a43ebf2271976b447a26d614b80948f0");
      }
    }
    return heroCard.GetEntity().IsControlledByOpposingSidePlayer() ? this.GetPreloadedSound("VO_Innkeeper_Male_Dwarf_NEFARIAN_Tavern_Brawl.prefab:5dfeed5d6b1827848999565cb1ef42fa") : base.GetAnnouncerLine(heroCard, type);
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB11_CoOpv3 tb11CoOpv3 = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    tb11CoOpv3.SetUpBossCard();
    if ((Object) tb11CoOpv3.m_bossCard == (Object) null || turn != 1)
      return false;
    Gameplay.Get().StartCoroutine(tb11CoOpv3.PlaySoundAndBlockSpeechWithCustomGameString("VO_BRMA13_1_RESPONSE_05.prefab:ec4f58f21067dde49b2ee26538259c89", "VO_COOP03_01", Notification.SpeechBubbleDirection.TopRight, tb11CoOpv3.m_bossCard.GetActor()));
    return false;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB11_CoOpv3 tb11CoOpv3 = this;
    while (tb11CoOpv3.m_enemySpeaking)
      yield return (object) null;
    tb11CoOpv3.SetUpBossCard();
    if (!((Object) tb11CoOpv3.m_bossCard == (Object) null))
    {
      Actor actor = tb11CoOpv3.m_bossCard.GetActor();
      switch (missionEvent)
      {
        case 2:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tb11CoOpv3.PlaySoundAndBlockSpeechWithCustomGameString("VO_BRMA17_1_RESPONSE_85.prefab:c7bbc928438b13241bde42c6578ad5c8", "VO_COOP03_03", Notification.SpeechBubbleDirection.TopRight, actor));
          GameState.Get().SetBusy(false);
          break;
        case 6:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tb11CoOpv3.PlaySoundAndBlockSpeechWithCustomGameString("VO_BRMA13_1_HP_PRIEST_08.prefab:75d6f8035f037dd43af7c058f318c2fb", "VO_COOP03_06", Notification.SpeechBubbleDirection.TopRight, actor));
          GameState.Get().SetBusy(false);
          break;
        case 7:
          GameState.Get().SetBusy(true);
          Gameplay.Get().StartCoroutine(tb11CoOpv3.PlaySoundAndBlockSpeechWithCustomGameString("VO_BRMA13_1_HP_SHAMAN_13.prefab:e248e28c2032e5c4c84490af8596f093", "VO_COOP03_07", Notification.SpeechBubbleDirection.TopRight, actor));
          GameState.Get().SetBusy(false);
          break;
        case 97:
          GameState.Get().SetBusy(true);
          yield return (object) Gameplay.Get().StartCoroutine(tb11CoOpv3.PlaySoundAndBlockSpeechWithCustomGameString("VO_NEFARIAN_NEF2_65.prefab:cad99daf56acb69428af9299fe9fb04b", "VO_COOP03_02", Notification.SpeechBubbleDirection.TopRight, actor));
          GameState.Get().SetBusy(false);
          break;
        case 98:
          GameState.Get().SetBusy(true);
          yield return (object) Gameplay.Get().StartCoroutine(tb11CoOpv3.PlaySoundAndBlockSpeechWithCustomGameString("VO_BRMA13_1_TURN1_PT1_02.prefab:ac211cc8ab665034da99720e38b6b994", "VO_COOP03_08", Notification.SpeechBubbleDirection.TopRight, actor));
          GameState.Get().SetBusy(false);
          break;
        case 99:
          GameState.Get().SetBusy(true);
          yield return (object) Gameplay.Get().StartCoroutine(tb11CoOpv3.PlaySoundAndBlockSpeechWithCustomGameString("VO_BRMA17_1_START_78.prefab:76391ad5bad9fcb4382a2bc98d2765d7", "VO_COOP03_09", Notification.SpeechBubbleDirection.TopRight, actor));
          GameState.Get().SetBusy(false);
          break;
      }
    }
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public TB11_CoOpv3()
    : base()
  {
  }
}
