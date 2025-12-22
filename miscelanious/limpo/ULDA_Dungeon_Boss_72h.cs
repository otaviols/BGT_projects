using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_72h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerDeathwing_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerDeathwing_01.prefab:19b429d812aebc4488976a81b00a4112");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerPlagueofDeath_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerPlagueofDeath_01.prefab:c02cbe6c127b41441bbfc2322d0bd31b");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerShadowWordDeath_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerShadowWordDeath_01.prefab:312198ed68318ef43b9a1369a6637188");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_DeathALT_01.prefab:54587749ea1c49a4dab5703e6fdba274");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_DefeatPlayer_01.prefab:9a7cafac41cffc64e9fcdda124b3a9f5");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_EmoteResponse_01.prefab:6ec271111d0c9f440a810b18bd5b26bf");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_01.prefab:c07c5ffe5174d6d478c8b6b314efe161");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_02.prefab:bb5cb786f4c27f743ad58e247aa74785");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_03.prefab:0cc21ed1d7310544f8ba7311fb786cc9");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_04.prefab:507eb783c1a47734fb0c77054d793829");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_05.prefab:dfc9338a32aead8458423eb1af00473c");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_Idle_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_Idle_01.prefab:f92b3584a49666046914da842c1b68ca");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_Idle_02 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_Idle_02.prefab:e7fa0943f034d6d478f6c734b13ddb09");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_Intro_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_Intro_01.prefab:e692437eb5f7f834881f0b1f803f802f");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_IntroSpecialFinley_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_IntroSpecialFinley_01.prefab:16d3fa49cf161b548be82b30a6fd6768");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Bone_Wraith_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Bone_Wraith_01.prefab:1f46442aab4a5b147854e2c2b6e3cc0f");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Murmy_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Murmy_01.prefab:94b834339fcc71348b39968efae27cd5");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Octosari_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Octosari_01.prefab:80bf4fa2f77eae043be25f22c9758f4d");
  private static readonly AssetReference VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTriggerDOOM_01 = new AssetReference("VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTriggerDOOM_01.prefab:79f741af94b1e6949bfb887e91854bf0");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_01,
    (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_02,
    (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_03,
    (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_04,
    (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_Idle_01,
    (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_Idle_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerDeathwing_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerPlagueofDeath_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerShadowWordDeath_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_DeathALT_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_02,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_03,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_04,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_HeroPower_05,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_Idle_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_Idle_02,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_Intro_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_IntroSpecialFinley_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Bone_Wraith_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Murmy_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Octosari_01,
      (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTriggerDOOM_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Finley")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_IntroSpecialFinley_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_72h uldaDungeonBoss72h = this;
    while (uldaDungeonBoss72h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss72h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_72h uldaDungeonBoss72h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss72h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss72h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss72h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss72h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss72h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_723"))
      {
        if (!(cardId == "ULD_177"))
        {
          if (cardId == "ULD_275")
            yield return (object) uldaDungeonBoss72h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Bone_Wraith_01);
        }
        else
          yield return (object) uldaDungeonBoss72h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Octosari_01);
      }
      else
        yield return (object) uldaDungeonBoss72h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTrigger_Murmy_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_72h uldaDungeonBoss72h = this;
    while (uldaDungeonBoss72h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss72h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss72h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss72h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss72h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "NEW1_030") && !(cardId == "OG_317"))
      {
        if (!(cardId == "ULD_718"))
        {
          if (!(cardId == "EX1_622"))
          {
            if (cardId == "OG_239")
              yield return (object) uldaDungeonBoss72h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_PlayerTriggerDOOM_01);
          }
          else
            yield return (object) uldaDungeonBoss72h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerShadowWordDeath_01);
        }
        else
          yield return (object) uldaDungeonBoss72h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerPlagueofDeath_01);
      }
      else
        yield return (object) uldaDungeonBoss72h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_72h.VO_ULDA_BOSS_72h_Male_TitanConstruct_BossTriggerDeathwing_01);
    }
  }
}
