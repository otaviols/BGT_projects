using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_21h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_01.prefab:99d0ed6d1c54ec4448ae857bbae61e9b");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_03 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_03.prefab:e12f7cb8444f0ae4388785973cf2fee7");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_04 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_04.prefab:733a8602cad771b4aa3ae34cb4e069dd");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_Death_02 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_Death_02.prefab:62d36b042e742b8409bde9ea0c929ae6");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_DefeatPlayer_01.prefab:a88c48cc7be215e4c9f3a236675aae40");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_EmoteResponse_02 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_EmoteResponse_02.prefab:a6d855255b1e95f42a96c365119d0a74");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_HeroPower_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_HeroPower_01.prefab:65d44008864b3024cb00581af8874746");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_HeroPower_03 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_HeroPower_03.prefab:bf8ae6909691ebf4b81ee01459780941");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_HeroPower_04 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_HeroPower_04.prefab:3b3b4498ea12c9a4f877fb83225c3cea");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_Idle_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_Idle_01.prefab:3689bdca2b6bf2c458a399dfa4a06f68");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_Idle_02 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_Idle_02.prefab:0357bdf1f24ae164f94a614df5a49dda");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_Idle_03 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_Idle_03.prefab:721ba6a06db3fa34ab5a7896efc8550e");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_Intro_02 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_Intro_02.prefab:4ba21e06012280847850f37f2d6f11d3");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_PlayerCairne_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_PlayerCairne_01.prefab:f5eed11a03020c144bc145ec42acb202");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_PlayerETC_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_PlayerETC_01.prefab:14361c1273dc26e48837e9da265a5c51");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_PlayerSecret_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_PlayerSecret_01.prefab:3b17e725dd858c844a7e007f70b459fd");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_PlayerSecret_02 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_PlayerSecret_02.prefab:ee73d80fec75e4144b9d05a87d9157bf");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_01.prefab:c1980252cb29bfc42ba413a8ee6c1fe9");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_02 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_02.prefab:8aec5aa686a78504da924873dda3fe3f");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_03 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_03.prefab:e71188bfb1ce70e4f9ea2b01c5421b5c");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_WrongPrediction_01 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_WrongPrediction_01.prefab:721173acc263c044d814dcaccb5c129d");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_WrongPrediction_03 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_WrongPrediction_03.prefab:d0bd76ef2efa59548815cd669a930511");
  private static readonly AssetReference VO_DALA_BOSS_21h_Male_Human_WrongPrediction_04 = new AssetReference("VO_DALA_BOSS_21h_Male_Human_WrongPrediction_04.prefab:d896b394c7b38ef4999c2dfa760bd528");
  private HashSet<string> m_playedLines = new HashSet<string>();
  private static List<string> m_BossHeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_HeroPower_01,
    (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_HeroPower_03,
    (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_HeroPower_04
  };
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Idle_01,
    (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Idle_02,
    (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Idle_03
  };
  private List<string> m_PlayerSecret = new List<string>()
  {
    (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_PlayerSecret_01,
    (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_PlayerSecret_02
  };
  private bool m_PredictionSpoken;

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_03,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_04,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Death_02,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_EmoteResponse_02,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_HeroPower_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_HeroPower_03,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_HeroPower_04,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Idle_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Idle_02,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Idle_03,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Intro_02,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_PlayerCairne_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_PlayerETC_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_PlayerSecret_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_PlayerSecret_02,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_02,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_03,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_WrongPrediction_01,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_WrongPrediction_03,
      (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_WrongPrediction_04
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Intro_02;
    this.m_deathLine = (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_Death_02;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_EmoteResponse_02;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_21h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => false;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_George") || !(cardId != "DALA_Eudora"))
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
    DALA_Dungeon_Boss_21h dalaDungeonBoss21h = this;
    if (dalaDungeonBoss21h.m_enemySpeaking)
    {
      switch (missionEvent)
      {
        case 101:
          dalaDungeonBoss21h.m_PredictionSpoken = false;
          yield break;
        case 102:
          dalaDungeonBoss21h.m_PredictionSpoken = false;
          yield break;
        case 103:
          dalaDungeonBoss21h.m_PredictionSpoken = false;
          yield break;
      }
    }
    while (dalaDungeonBoss21h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        dalaDungeonBoss21h.m_PredictionSpoken = true;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_01);
        break;
      case 102:
        dalaDungeonBoss21h.m_PredictionSpoken = true;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_02);
        break;
      case 103:
        dalaDungeonBoss21h.m_PredictionSpoken = true;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_StartOfTurnPrediction_03);
        break;
      case 104:
        yield return (object) dalaDungeonBoss21h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss21h.m_PlayerSecret);
        break;
      case 105:
        yield return (object) dalaDungeonBoss21h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_21h.m_BossHeroPower);
        break;
      case 111:
        if (!dalaDungeonBoss21h.m_PredictionSpoken)
          break;
        dalaDungeonBoss21h.m_PredictionSpoken = false;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_WrongPrediction_01);
        break;
      case 112:
        if (!dalaDungeonBoss21h.m_PredictionSpoken)
          break;
        dalaDungeonBoss21h.m_PredictionSpoken = false;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_WrongPrediction_03);
        break;
      case 113:
        if (!dalaDungeonBoss21h.m_PredictionSpoken)
          break;
        dalaDungeonBoss21h.m_PredictionSpoken = false;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_WrongPrediction_04);
        break;
      case 131:
        if (!dalaDungeonBoss21h.m_PredictionSpoken)
          break;
        dalaDungeonBoss21h.m_PredictionSpoken = false;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_01);
        break;
      case 132:
        if (!dalaDungeonBoss21h.m_PredictionSpoken)
          break;
        dalaDungeonBoss21h.m_PredictionSpoken = false;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_03);
        break;
      case 133:
        if (!dalaDungeonBoss21h.m_PredictionSpoken)
          break;
        dalaDungeonBoss21h.m_PredictionSpoken = false;
        yield return (object) dalaDungeonBoss21h.PlayBossLine(actor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_CorrectPrediction_04);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss21h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_21h dalaDungeonBoss21h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss21h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss21h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss21h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss21h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss21h.m_playedLines.Add(cardId);
      if (!(cardId == "PRO_001"))
      {
        if (cardId == "EX1_110")
          yield return (object) dalaDungeonBoss21h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_PlayerCairne_01);
      }
      else
        yield return (object) dalaDungeonBoss21h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_21h.VO_DALA_BOSS_21h_Male_Human_PlayerETC_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_21h dalaDungeonBoss21h = this;
    while (dalaDungeonBoss21h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss21h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss21h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss21h.m_playedLines.Add(cardId);
    }
  }
}
