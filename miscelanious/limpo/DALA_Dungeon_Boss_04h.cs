using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_04h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_Death_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_Death_01.prefab:46a0e0c00caa6e44888c2eace85bc99a");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_DefeatPlayer_01.prefab:76126d54a811dc04e93293e90792abe7");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_EmoteResponse_01.prefab:3aac8184734d5414dbf1e02434130751");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPower_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPower_01.prefab:8bb49a9fb35a2a34aaf7606d76db6f0c");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPower_02 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPower_02.prefab:d527a85b6751d584d99723594f0533ef");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPower_03 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPower_03.prefab:dcba7364df536c44aa8261c004aa672b");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPower_04 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPower_04.prefab:5eb3c02bcff8d164a804d2e12e58189d");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPowerBossOnly_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPowerBossOnly_01.prefab:02fbc3172008a6b4ba7afdfba782fee4");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPowerBoth_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPowerBoth_01.prefab:5bf1dd5e4ae72d6438d80263333205c6");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPowerBoth_02 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPowerBoth_02.prefab:0b9ddce9a968bb84f87c14196ccd8727");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPowerLose_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPowerLose_01.prefab:8b615dd50781cb5419ce7d0ceee9fb2a");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPowerLose_02 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPowerLose_02.prefab:a803cdde33698af44bfd6c0b878f5094");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPowerPlayerOnly_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPowerPlayerOnly_01.prefab:fe29a261abc694e4ebc6bddfb8ed5737");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPowerWin_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPowerWin_01.prefab:e803d23a1629058428b21c7acbc73dbf");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_HeroPowerWin_02 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_HeroPowerWin_02.prefab:bc306118e17e4e349b4d55bc6e1eeb74");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_Idle_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_Idle_01.prefab:77528513aa5105c44b8b1c88d4109cea");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_Idle_02 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_Idle_02.prefab:f08533aa89553c84bb8a7cdc75194783");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_Idle_03 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_Idle_03.prefab:28536f8e8afc7ec42bd6deb2fed2e854");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_Intro_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_Intro_01.prefab:c716f8fea2de9d14f81db9d6ad33b0b5");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_IntroSqueamlish_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_IntroSqueamlish_01.prefab:454f78f9863711b4dbe731012b075d0d");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_PlayerPortal_01 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_PlayerPortal_01.prefab:6fbd49194cbc21845ab17bc9da6b620f");
  private static readonly AssetReference VO_DALA_BOSS_04h_Female_Undead_PlayerPortal_02 = new AssetReference("VO_DALA_BOSS_04h_Female_Undead_PlayerPortal_02.prefab:b578b27f60a2c944199c30eb2fd1c38d");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPower_01,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPower_02,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPower_03,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPower_04
  };
  private List<string> m_HeroPowerWinLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerWin_01,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerWin_02
  };
  private List<string> m_HeroPowerBothLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerBoth_01,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerBoth_02
  };
  private List<string> m_HeroPowerLoseLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerLose_01,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerLose_02
  };
  private List<string> m_PlayerPortalLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_PlayerPortal_01,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_PlayerPortal_02
  };
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Idle_01,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Idle_02,
    (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Death_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPower_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPower_02,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPower_03,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPower_04,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerBossOnly_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerBoth_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerBoth_02,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerLose_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerLose_02,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerPlayerOnly_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerWin_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerWin_02,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Idle_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Idle_02,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Idle_03,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Intro_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_IntroSqueamlish_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_PlayerPortal_01,
      (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_PlayerPortal_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_04h.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Squeamlish"))
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

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_04h dalaDungeonBoss04h = this;
    while (dalaDungeonBoss04h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss04h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss04h.m_HeroPowerLines);
        break;
      case 102:
        yield return (object) dalaDungeonBoss04h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss04h.m_HeroPowerWinLines);
        break;
      case 103:
        yield return (object) dalaDungeonBoss04h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss04h.m_HeroPowerBothLines);
        break;
      case 104:
        yield return (object) dalaDungeonBoss04h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss04h.m_HeroPowerLoseLines);
        break;
      case 105:
        yield return (object) dalaDungeonBoss04h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerPlayerOnly_01);
        break;
      case 106:
        yield return (object) dalaDungeonBoss04h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_04h.VO_DALA_BOSS_04h_Female_Undead_HeroPowerBossOnly_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss04h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_04h dalaDungeonBoss04h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss04h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss04h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss04h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss04h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss04h.m_playedLines.Add(cardId);
      if (cardId == "GVG_003" || cardId == "KAR_073" || cardId == "KAR_075" || cardId == "KAR_076" || cardId == "KAR_077" || cardId == "KAR_091")
        yield return (object) dalaDungeonBoss04h.PlayAndRemoveRandomLineOnlyOnce(enemyActor, dalaDungeonBoss04h.m_PlayerPortalLines);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_04h dalaDungeonBoss04h = this;
    while (dalaDungeonBoss04h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss04h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss04h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss04h.m_playedLines.Add(cardId);
    }
  }
}
