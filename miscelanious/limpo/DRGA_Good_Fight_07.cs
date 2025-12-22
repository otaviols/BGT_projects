using System.Collections;
using System.Collections.Generic;

public class DRGA_Good_Fight_07 : DRGA_Dungeon
{
  private static readonly AssetReference VO_DRGA_BOSS_02h_Female_NightElf_Good_Fight_07_Backstory_01a_01 = new AssetReference("VO_DRGA_BOSS_02h_Female_NightElf_Good_Fight_07_Backstory_01a_01.prefab:7033ca23bb5b3384d8f3fdca08b55488");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Backstory_01b_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Backstory_01b_01.prefab:280a2851162529f40aedfbebdaf24145");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_Dragonbreath_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_Dragonbreath_01.prefab:f51c135e7a81a904dacfd433f35b1274");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_Dragonbreath_02 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_Dragonbreath_02.prefab:240c24a6aa1d0df47aee6af9d9ea7b2f");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Misc_01_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Misc_01_01.prefab:776a9f12df22e7441904a3285c0c403f");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_PlayerStart_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_PlayerStart_01.prefab:6618c7c7da1bb354388dbdfa7d68f6a8");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_RotwingCaptured_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_RotwingCaptured_01.prefab:ff7d3fbc2412adb4cb702195549219ed");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_TurnOne_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_TurnOne_01.prefab:f3406880fcab36c42abca7246646fa14");
  private static readonly AssetReference VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Victory_01 = new AssetReference("VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Victory_01.prefab:b2e35fdf67c238c409ae31f8ce3df139");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_Death_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_Death_01.prefab:4d00dd974d1496d4a9eaaa95c4cc99aa");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_01_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_01_01.prefab:d4991e5db14d6464db6032a4fc30a95f");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_02_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_02_01.prefab:7eb0b4f79c563514c89fb2a7607814d5");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_03_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_03_01.prefab:5eef3d52f04244544aaa4911b2c9a35c");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BossAttack_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BossAttack_01.prefab:326aada7a3277244e946724ed8f8d25b");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BossStart_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BossStart_01.prefab:eaf1cd45f0b9d2b4bbebf455b402283d");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BrannIdle_01_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BrannIdle_01_01.prefab:04b1e548994fd2d47bc351f5f308dd69");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_EmoteResponse_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_EmoteResponse_01.prefab:dc524f3a9ce93e941bc12c0cf1c227e6");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_01_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_01_01.prefab:ecef7a29e9f13a948a33bfbb88f34985");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_02_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_02_01.prefab:14dc271f0e6ea08498f0fb965618c782");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_03_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_03_01.prefab:6ed8c497fc1538648ac05162c5f5c589");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_02_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_02_01.prefab:dd6457b68fdf7ed468a1c92c2e264429");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_03_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_03_01.prefab:de1341dcf88c1e7469a6e742cdcd3a9d");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_04_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_04_01.prefab:a9f14b3f910dcc2489af329df073d599");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_05_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_05_01.prefab:900435e8f499da04391a2b07373de8b2");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_06_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_06_01.prefab:d617ccd45c4ffb447a97f1d259550b40");
  private static readonly AssetReference VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Player_Rotwing_01 = new AssetReference("VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Player_Rotwing_01.prefab:3b670cff8181a5e4f9ba9302e49b7a9c");
  private List<string> m_missionEventTrigger106Lines = new List<string>()
  {
    (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_01_01,
    (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_02_01,
    (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_03_01
  };
  private List<string> m_VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_DragonbreathLines = new List<string>()
  {
    (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_Dragonbreath_01,
    (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_Dragonbreath_02
  };
  private List<string> m_VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_IdleLines = new List<string>()
  {
    (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_01_01,
    (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_02_01,
    (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_03_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_02h_Female_NightElf_Good_Fight_07_Backstory_01a_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Backstory_01b_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_Dragonbreath_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_Dragonbreath_02,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Misc_01_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_PlayerStart_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_RotwingCaptured_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_TurnOne_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Victory_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_Death_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_01_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_02_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_HeroPowerTrigger_03_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BossAttack_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BossStart_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BrannIdle_01_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_EmoteResponse_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_01_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_02_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Idle_03_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_02_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_03_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_04_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_05_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_06_01,
      (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Player_Rotwing_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Boss_Death_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BossStart_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_EmoteResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DRGA_Good_Fight_07 drgaGoodFight07 = this;
    while (drgaGoodFight07.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 100:
        if (drgaGoodFight07.m_Heroic)
          break;
        yield return (object) drgaGoodFight07.PlayLineAlways((string) DRGA_Dungeon.EliseBrassRing, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_02h_Female_NightElf_Good_Fight_07_Backstory_01a_01);
        yield return (object) drgaGoodFight07.PlayLineAlways(friendlyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Backstory_01b_01);
        break;
      case 102:
        if (drgaGoodFight07.m_Heroic)
          break;
        yield return (object) drgaGoodFight07.PlayLineAlways(friendlyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Misc_01_01);
        break;
      case 103:
        if (drgaGoodFight07.m_Heroic)
          break;
        yield return (object) drgaGoodFight07.PlayLineAlways(friendlyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_RotwingCaptured_01);
        break;
      case 104:
        if (drgaGoodFight07.m_Heroic)
          break;
        yield return (object) drgaGoodFight07.PlayLineAlways(friendlyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_TurnOne_01);
        break;
      case 105:
        if (drgaGoodFight07.m_Heroic)
          break;
        GameState.Get().SetBusy(true);
        yield return (object) drgaGoodFight07.PlayLineAlways(friendlyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Victory_01);
        yield return (object) drgaGoodFight07.PlayLineAlways(enemyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_06_01);
        GameState.Get().SetBusy(false);
        break;
      case 106:
        yield return (object) drgaGoodFight07.PlayAndRemoveRandomLineOnlyOnce(enemyActor, drgaGoodFight07.m_missionEventTrigger106Lines);
        break;
      case 107:
        yield return (object) drgaGoodFight07.PlayLineOnlyOnce(enemyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_BossAttack_01);
        break;
      case 108:
        yield return (object) drgaGoodFight07.PlayLineAlways(enemyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_02_01);
        break;
      case 109:
        yield return (object) drgaGoodFight07.PlayLineAlways(enemyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_03_01);
        break;
      case 110:
        yield return (object) drgaGoodFight07.PlayLineAlways(enemyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_04_01);
        break;
      case 111:
        yield return (object) drgaGoodFight07.PlayLineAlways(enemyActor, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Misc_05_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) drgaGoodFight07.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DRGA_Good_Fight_07 drgaGoodFight07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) drgaGoodFight07.\u003C\u003En__1(entity);
    while (drgaGoodFight07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!drgaGoodFight07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) drgaGoodFight07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      drgaGoodFight07.m_playedLines.Add(cardId);
      Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "DRGA_BOSS_04t"))
      {
        if (cardId == "YOD_036")
          yield return (object) drgaGoodFight07.PlayLineOnlyOnce(actor1, (string) DRGA_Good_Fight_07.VO_DRGA_BOSS_19h_Male_Dragon_Good_Fight_07_Player_Rotwing_01);
      }
      else
        yield return (object) drgaGoodFight07.PlayAndRemoveRandomLineOnlyOnce(actor2, drgaGoodFight07.m_VO_DRGA_BOSS_04h_Male_Dwarf_Good_Fight_07_Brann_DragonbreathLines);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DRGA_Good_Fight_07 drgaGoodFight07 = this;
    while (drgaGoodFight07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!drgaGoodFight07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) drgaGoodFight07.\u003C\u003En__2(entity);
      yield return (object) drgaGoodFight07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      drgaGoodFight07.m_playedLines.Add(cardId);
    }
  }
}
