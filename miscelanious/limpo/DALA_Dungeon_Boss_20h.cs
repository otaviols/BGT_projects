using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_20h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_BossClearOverload_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_BossClearOverload_01.prefab:3870ef7ed8015664b9221cc70a5aaf45");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_BossClearOverload_03 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_BossClearOverload_03.prefab:6707d7fba33bf6f409f500aab44ed52c");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_BossLavaBurst_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_BossLavaBurst_01.prefab:0be790314f82fb1479612f57da169bd2");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_BossLightningSpell_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_BossLightningSpell_01.prefab:ef2ff2d39dcdaae41b8d29d5f5e0dc12");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_Death_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_Death_01.prefab:424af123c04fbaf46b1091ddd942c1c8");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_DefeatPlayer_01.prefab:c40f584115e7c79448c12e02f6034338");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_EmoteResponse_01.prefab:ab116ca948492c64097a6e485fbc4d4d");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_01.prefab:10e14eec3cbd6ee43b1c5fd7af9772d3");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_02 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_02.prefab:a4abb8934bf564145ac03e6c8715921d");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_03 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_03.prefab:6c4c62f57b38b84409743bf02d3607cf");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_Idle_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_Idle_01.prefab:46a57a908826f11458c04e1006b8e87b");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_Idle_02 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_Idle_02.prefab:144dbf8c61c7e4840b036835f94283ab");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_Idle_03 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_Idle_03.prefab:c4aa34caca890e34c84592b18e262e66");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_Intro_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_Intro_01.prefab:ed29208739cf12c459e1b60bda01b0ab");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_OverloadPass_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_OverloadPass_01.prefab:9d5ab9e0eb86ad6458d6b1fdd721e9fc");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_OverloadPass_02 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_OverloadPass_02.prefab:c1ffe8083fd219a44a4cfb51aa80dfd1");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_OverloadPass_03 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_OverloadPass_03.prefab:bf68200fba0024a408642e1dc4f5b10a");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_PlayerRainOfToads_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_PlayerRainOfToads_01.prefab:c53f52cb79e63a14eb869bde05cdb48d");
  private static readonly AssetReference VO_DALA_BOSS_20h_Male_Troll_PlayerShamanSpell_01 = new AssetReference("VO_DALA_BOSS_20h_Male_Troll_PlayerShamanSpell_01.prefab:af4bd121c9205ee4eb6ec09f913dba2d");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Idle_01,
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Idle_02,
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Idle_03
  };
  private static List<string> m_OverloadPass = new List<string>()
  {
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_OverloadPass_01,
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_OverloadPass_02,
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_OverloadPass_03
  };
  private static List<string> m_BossClearOverload = new List<string>()
  {
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_BossClearOverload_01,
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_BossClearOverload_03
  };
  private static List<string> m_HeropowerTrigger = new List<string>()
  {
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_01,
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_02,
    (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_BossClearOverload_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_BossClearOverload_03,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_BossLavaBurst_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_BossLightningSpell_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Death_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_02,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_HeroPowerTrigger_03,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Idle_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Idle_02,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Idle_03,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Intro_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_OverloadPass_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_OverloadPass_02,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_OverloadPass_03,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_PlayerRainOfToads_01,
      (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_PlayerShamanSpell_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_20h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Vessina"))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_20h dalaDungeonBoss20h = this;
    while (dalaDungeonBoss20h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss20h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_20h.m_HeropowerTrigger);
        break;
      case 102:
        yield return (object) dalaDungeonBoss20h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_20h.m_BossClearOverload);
        break;
      case 103:
        yield return (object) dalaDungeonBoss20h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_20h.m_OverloadPass);
        break;
      case 104:
        yield return (object) dalaDungeonBoss20h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_PlayerShamanSpell_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss20h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_20h dalaDungeonBoss20h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss20h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss20h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss20h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss20h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss20h.m_playedLines.Add(cardId);
      if (cardId == "TRL_351")
        yield return (object) dalaDungeonBoss20h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_PlayerRainOfToads_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_20h dalaDungeonBoss20h = this;
    while (dalaDungeonBoss20h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss20h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss20h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss20h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_241"))
      {
        if (cardId == "BOT_246" || cardId == "EX1_238" || cardId == "EX1_251" || cardId == "EX1_259" || cardId == "GIL_600")
          yield return (object) dalaDungeonBoss20h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_BossLightningSpell_01);
      }
      else
        yield return (object) dalaDungeonBoss20h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_20h.VO_DALA_BOSS_20h_Male_Troll_BossLavaBurst_01);
    }
  }
}
