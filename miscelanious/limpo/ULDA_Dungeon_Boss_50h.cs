using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_50h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_BossHyenaAlpha_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_BossHyenaAlpha_01.prefab:cf30112d4814caf43a0b382397f28fd7");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_BossKoboldSandtrooper_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_BossKoboldSandtrooper_01.prefab:709a5199fa8579a4c95acfc00b4a4f38");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_BossMarkedShot_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_BossMarkedShot_01.prefab:b3a000ae6f5784a4ca32df9b966aff01");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_DeathALT_01.prefab:8d07bcbe136dc76468943479e8c9cc00");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_DefeatPlayer_01.prefab:47ee05a5ccb26cb44985a261a88e34f6");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_EmoteResponse_01.prefab:1b453b9dc0b9088469267002499654a8");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_01.prefab:0179726d05f3fdf43a0ac711d17eb756");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_02.prefab:d1e0cffd867979e41af4962c08bd8d61");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_03.prefab:6641bf43c63d3414cbc04751abcbbf13");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_04.prefab:6b0869d7cd1846f4cbb875f2e0422553");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_05.prefab:20197153fb4e7b34bb54f8186cedbed9");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_Idle_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_Idle_01.prefab:cceebdf2900f871498e93a209485865b");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_Idle_02 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_Idle_02.prefab:e212f90192f2eb349b2da0d0c0202888");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_Idle_03 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_Idle_03.prefab:46c802af5c80a8d4e85b0c5c8adcec31");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_Intro_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_Intro_01.prefab:f2574e701b4bf29428035e8200398500");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_IntroBrann_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_IntroBrann_01.prefab:47009177b2f223943a96e9ece415a063");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_IntroFinley_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_IntroFinley_01.prefab:7a69e245b94c09542b44d9d1aea884e3");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_PlayerBlunderbussTreasure_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_PlayerBlunderbussTreasure_01.prefab:70670ae1a682d5d448b93e26974153a5");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_PlayerSwarmofLocusts_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_PlayerSwarmofLocusts_01.prefab:bb405a98bf5a4834fb17c51385d15a85");
  private static readonly AssetReference VO_ULDA_BOSS_50h_Male_Gnoll_PlayerUntamedBeastmaster_01 = new AssetReference("VO_ULDA_BOSS_50h_Male_Gnoll_PlayerUntamedBeastmaster_01.prefab:80c19b52f5284854190add70b9ba2f85");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_01,
    (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_02,
    (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_03,
    (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_04,
    (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_Idle_01,
    (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_Idle_02,
    (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_BossHyenaAlpha_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_BossKoboldSandtrooper_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_BossMarkedShot_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_DeathALT_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_02,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_03,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_04,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_HeroPower_05,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_Idle_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_Idle_02,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_Idle_03,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_Intro_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_IntroBrann_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_IntroFinley_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_PlayerBlunderbussTreasure_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_PlayerSwarmofLocusts_01,
      (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_PlayerUntamedBeastmaster_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Brann")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_IntroBrann_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "ULDA_Finley")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_IntroFinley_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
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
    ULDA_Dungeon_Boss_50h uldaDungeonBoss50h = this;
    while (uldaDungeonBoss50h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss50h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss50h.m_HeroPowerLines);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss50h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_50h uldaDungeonBoss50h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss50h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss50h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss50h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss50h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss50h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULDA_401"))
      {
        if (!(cardId == "ULD_713"))
        {
          if (cardId == "TRL_405")
            yield return (object) uldaDungeonBoss50h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_PlayerUntamedBeastmaster_01);
        }
        else
          yield return (object) uldaDungeonBoss50h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_PlayerSwarmofLocusts_01);
      }
      else
        yield return (object) uldaDungeonBoss50h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_PlayerBlunderbussTreasure_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_50h uldaDungeonBoss50h = this;
    while (uldaDungeonBoss50h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss50h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss50h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss50h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss50h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_154"))
      {
        if (!(cardId == "ULD_184"))
        {
          if (cardId == "DAL_371")
            yield return (object) uldaDungeonBoss50h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_BossMarkedShot_01);
        }
        else
          yield return (object) uldaDungeonBoss50h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_BossKoboldSandtrooper_01);
      }
      else
        yield return (object) uldaDungeonBoss50h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_50h.VO_ULDA_BOSS_50h_Male_Gnoll_BossHyenaAlpha_01);
    }
  }
}
