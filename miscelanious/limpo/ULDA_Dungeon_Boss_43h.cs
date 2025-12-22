using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_43h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_BossGnomeferatu_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_BossGnomeferatu_01.prefab:1f50840d32b924346a1b2d7618bd6c1a");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_BossSiphonSoul_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_BossSiphonSoul_01.prefab:236fe058a193aa04ea2ebc32529f2e08");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_BossSoulInfusion_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_BossSoulInfusion_01.prefab:62771f4e154f7ce43b0d437ec5a09b63");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_DeathALT_01.prefab:fd1b466e8a781df4d90ccb953afe6f64");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_DefeatPlayer_01.prefab:438ff5a226b67b544889dd690db95ee8");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_EmoteResponse_01.prefab:ad8f8039cd80e6f4ebdaa473e44c155e");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_HeroPower_01.prefab:1c3729a4f9b71ff42bcec0d0bc6b50aa");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_HeroPower_02.prefab:bd1f00ab53b348b438fe3acfb68b2c81");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_HeroPower_03.prefab:15625ab2f64ad2d4887720071fd0b197");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_HeroPower_04.prefab:f5c56522df6dbf04496484ee0b7a607b");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_HeroPower_05.prefab:941daf7a6b214794b89fc110b8bbef24");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_Idle1_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_Idle1_01.prefab:31150cedd029ba54989300d32c484d1c");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_Idle2_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_Idle2_01.prefab:d6c17c65068a0504ab426314cb49f99e");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_Idle3_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_Idle3_01.prefab:1ab4758de3265f44ab6d04ac0b3521d1");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_Intro_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_Intro_01.prefab:fe99cd47663465d42a597a8eb48be752");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_IntroElise_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_IntroElise_01.prefab:fbe3c5d185103724abd1eeef16b85e81");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_PlayerBookofSpecters_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_PlayerBookofSpecters_01.prefab:ffaa292aa121ef24cb388e5547146c6f");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_PlayerBookoftheDeadTreasure_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_PlayerBookoftheDeadTreasure_01.prefab:57f3417a58cebf84d8c1a9d195c0840f");
  private static readonly AssetReference VO_ULDA_BOSS_43h_Female_Human_PlayerPlague_01 = new AssetReference("VO_ULDA_BOSS_43h_Female_Human_PlayerPlague_01.prefab:d3a6bc6ac0f714a4c969c017fedc34af");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_01,
    (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_02,
    (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_03,
    (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_04,
    (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_Idle1_01,
    (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_Idle2_01,
    (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_Idle3_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_BossGnomeferatu_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_BossSiphonSoul_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_BossSoulInfusion_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_DeathALT_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_02,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_03,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_04,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_HeroPower_05,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_Idle1_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_Idle2_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_Idle3_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_Intro_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_IntroElise_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_PlayerBookofSpecters_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_PlayerBookoftheDeadTreasure_01,
      (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_PlayerPlague_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_EmoteResponse_01;
  }

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (this.m_IdleLines.Count == 0)
      return;
    string idleLine = this.m_IdleLines[0];
    this.m_IdleLines.RemoveAt(0);
    Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, idleLine));
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Elise")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_IntroElise_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_43h uldaDungeonBoss43h = this;
    while (uldaDungeonBoss43h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss43h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_43h uldaDungeonBoss43h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss43h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss43h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss43h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss43h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss43h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "GIL_548":
          yield return (object) uldaDungeonBoss43h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_PlayerBookofSpecters_01);
          break;
        case "ULDA_006":
          yield return (object) uldaDungeonBoss43h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_PlayerBookoftheDeadTreasure_01);
          break;
        case "ULD_172":
        case "ULD_707":
        case "ULD_715":
        case "ULD_717":
        case "ULD_718":
          yield return (object) uldaDungeonBoss43h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_PlayerPlague_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_43h uldaDungeonBoss43h = this;
    while (uldaDungeonBoss43h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss43h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss43h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss43h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss43h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ICC_407"))
      {
        if (!(cardId == "EX1_309"))
        {
          if (cardId == "BOT_263")
            yield return (object) uldaDungeonBoss43h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_BossSoulInfusion_01);
        }
        else
          yield return (object) uldaDungeonBoss43h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_BossSiphonSoul_01);
      }
      else
        yield return (object) uldaDungeonBoss43h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_43h.VO_ULDA_BOSS_43h_Female_Human_BossGnomeferatu_01);
    }
  }
}
