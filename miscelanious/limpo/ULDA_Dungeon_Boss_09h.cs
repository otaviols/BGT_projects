using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_09h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_BossDemolisher_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_BossDemolisher_01.prefab:6a769776cb71790418056207c6b1f1c9");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_BossHeavyMetal_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_BossHeavyMetal_01.prefab:bd5485fe8a87db143b786b0a2c1c032f");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_BossOmegaAssembly_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_BossOmegaAssembly_01.prefab:75e9ae1206632af47836d7831d0e9f9e");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_DeathALT_01.prefab:f54f591ff1a95fc46a8d329026d77fe7");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_DefeatPlayer_01.prefab:d4fc2b8a8441a1648a7cf6005fc17536");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_EmoteResponse_01.prefab:50eaa1ca98962a545ae303c92d3ed40f");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_HeroPower_01.prefab:4c8ae017981bac447a59b2029471eda8");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_HeroPower_02.prefab:459259d4540cd934491731abbdfd7e06");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_HeroPowerLowArmor_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_HeroPowerLowArmor_01.prefab:23c385895cb8d1642a1b518bc9ea1f19");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_Idle_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_Idle_01.prefab:fdbb1c54c2da7df42a307d47b24c828a");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_Idle_02 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_Idle_02.prefab:bc31cefc4fef68f4e844d5a1419ecbae");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_Idle_03 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_Idle_03.prefab:694ecc4f130d2fc49a25a6bea4e81fdd");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_Intro_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_Intro_01.prefab:ef27d916fda53be49aa4e791a2dbfd24");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_PlayerDemolisher_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_PlayerDemolisher_01.prefab:2093352b5ed095f49b4b444373a15390");
  private static readonly AssetReference VO_ULDA_BOSS_09h_Male_Mech_PlayerMechathun_01 = new AssetReference("VO_ULDA_BOSS_09h_Male_Mech_PlayerMechathun_01.prefab:73dd4a80d04e6d944b5400de5cb91797");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_HeroPower_01,
    (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_HeroPower_02
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_Idle_01,
    (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_Idle_02,
    (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_BossDemolisher_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_BossHeavyMetal_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_BossOmegaAssembly_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_DeathALT_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_HeroPower_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_HeroPower_02,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_HeroPowerLowArmor_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_Idle_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_Idle_02,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_Idle_03,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_Intro_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_PlayerDemolisher_01,
      (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_PlayerMechathun_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
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
    ULDA_Dungeon_Boss_09h uldaDungeonBoss09h = this;
    while (uldaDungeonBoss09h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss09h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_HeroPowerLowArmor_01);
        break;
      case 102:
        yield return (object) uldaDungeonBoss09h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss09h.m_HeroPowerLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss09h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_09h uldaDungeonBoss09h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss09h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss09h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss09h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss09h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss09h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_102"))
      {
        if (cardId == "BOT_424")
          yield return (object) uldaDungeonBoss09h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_PlayerMechathun_01);
      }
      else
        yield return (object) uldaDungeonBoss09h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_PlayerDemolisher_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_09h uldaDungeonBoss09h = this;
    while (uldaDungeonBoss09h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss09h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss09h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss09h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss09h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BOT_424"))
      {
        if (!(cardId == "TRL_324"))
        {
          if (cardId == "BOT_299")
            yield return (object) uldaDungeonBoss09h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_BossOmegaAssembly_01);
        }
        else
          yield return (object) uldaDungeonBoss09h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_BossHeavyMetal_01);
      }
      else
        yield return (object) uldaDungeonBoss09h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_09h.VO_ULDA_BOSS_09h_Male_Mech_BossDemolisher_01);
    }
  }
}
