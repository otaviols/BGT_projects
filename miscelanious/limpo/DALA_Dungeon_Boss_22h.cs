using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_22h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_BossDalaranLibrarian_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_BossDalaranLibrarian_02.prefab:4e21f6e0fcb282545beb8896bd3428f0");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_BossSpell_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_BossSpell_01.prefab:3bade612bce93ff46806244b194fef80");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_BossSpell_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_BossSpell_02.prefab:e59f7c0e26b793e4c95c80a3904109e4");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Death_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Death_02.prefab:3d660e550aed7494c83c88b1201fa942");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_DefeatPlayer_01.prefab:32cbf6e0b63e4b4469756b1d1f647055");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_EmoteResponse_01.prefab:ceb932d66ad5d7243b9f8e984a1d9d99");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_01.prefab:079b6a94bbd59074798c8ffe5a0e79c9");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_02.prefab:428b17ba535d00241b124855acb17ced");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_03 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_03.prefab:b3f2e317a37bed746ba463948a2facdc");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_04 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_04.prefab:1028805b1d59c134782dd98186af0bba");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Idle_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Idle_01.prefab:558feecaaee57f642974936442a345e8");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Idle_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Idle_02.prefab:ffa907bf45be4f143af37e649776fcb4");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Idle_03 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Idle_03.prefab:3d8cf1fddd6e5674d86dc14e552bd95e");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_Intro_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_Intro_01.prefab:590f9e4748fd3164dbec90ca7b95701e");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_IntroChu_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_IntroChu_02.prefab:8c8247d303b4e13489f739a098cb43a8");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_IntroGeorge_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_IntroGeorge_01.prefab:b8a522ccaf6fa54459a3635790c5d028");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_IntroOlBarkeye_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_IntroOlBarkeye_01.prefab:05b6c32beb5249a4cac586832fea9118");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_IntroRakanishu_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_IntroRakanishu_01.prefab:d223e3790be9da847a3f3bc41d85716f");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_IntroTekahn_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_IntroTekahn_01.prefab:d54ca44fa2b84e84c8c87e0b5dba9c4f");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerBabblingBook_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerBabblingBook_01.prefab:e4fdcc886031cbe4c8b1c99eba888423");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerBattlecry_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerBattlecry_01.prefab:f9ae1c2749c5e4645a1c8a0f8ffd73c3");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerBattlecry_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerBattlecry_02.prefab:fbbeace78e271a04ab6fb411b1efdb67");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerDalaranLibrarian_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerDalaranLibrarian_01.prefab:3ba64882148a65d41a8b71b95e6d2c77");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerLegendary_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerLegendary_01.prefab:1a09311bfabae134c99a279f2aa472ec");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerLegendary_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerLegendary_02.prefab:4f0eec7a7bdcfa44c9c4ab09181c4a58");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_01 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_01.prefab:8d287d8dc77a45c429b8f992c7a3100b");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_02 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_02.prefab:4a7af8985582e504f92664f963698a41");
  private static readonly AssetReference VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_03 = new AssetReference("VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_03.prefab:b9c7385116fc65d4088ec5335d048801");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Idle_01,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Idle_02,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Idle_03
  };
  private static List<string> m_PlayerSilence = new List<string>()
  {
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_01,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_02,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_03
  };
  private static List<string> m_HeroPowerTrigger = new List<string>()
  {
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_01,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_02,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_03,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_04
  };
  private static List<string> m_PlayerBattleCry = new List<string>()
  {
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerBattlecry_01,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerBattlecry_02
  };
  private static List<string> m_PlayerLegendary = new List<string>()
  {
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerLegendary_01,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerLegendary_02
  };
  private static List<string> m_BossSpell = new List<string>()
  {
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_BossSpell_01,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_BossSpell_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_BossDalaranLibrarian_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_BossSpell_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_BossSpell_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Death_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_03,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_04,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Idle_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Idle_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Idle_03,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Intro_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_IntroChu_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_IntroGeorge_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_IntroOlBarkeye_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_IntroRakanishu_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_IntroTekahn_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerBabblingBook_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerBattlecry_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerBattlecry_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerDalaranLibrarian_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerLegendary_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerLegendary_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_01,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_02,
      (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerSilence_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_Death_02;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_22h.m_IdleLines;

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_01,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_02,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_03,
    (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_HeroPower_04
  };

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Tekahn") || !(cardId != "DALA_Chu") || !(cardId != "DALA_Rakanishu") || !(cardId != "DALA_George") || !(cardId != "DALA_Barkeye"))
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
    DALA_Dungeon_Boss_22h dalaDungeonBoss22h = this;
    while (dalaDungeonBoss22h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss22h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_22h.m_BossSpell);
        break;
      case 102:
        yield return (object) dalaDungeonBoss22h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_22h.m_PlayerBattleCry);
        break;
      case 103:
        yield return (object) dalaDungeonBoss22h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_22h.m_PlayerLegendary);
        break;
      case 104:
        yield return (object) dalaDungeonBoss22h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_22h.m_PlayerSilence);
        break;
      case 105:
        yield return (object) dalaDungeonBoss22h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_22h.m_HeroPowerTrigger);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss22h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_22h dalaDungeonBoss22h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss22h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss22h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss22h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss22h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss22h.m_playedLines.Add(cardId);
      if (!(cardId == "KAR_009"))
      {
        if (cardId == "DAL_735")
          yield return (object) dalaDungeonBoss22h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerDalaranLibrarian_01);
      }
      else
        yield return (object) dalaDungeonBoss22h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_PlayerBabblingBook_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_22h dalaDungeonBoss22h = this;
    while (dalaDungeonBoss22h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss22h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss22h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss22h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss22h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "DAL_735")
        yield return (object) dalaDungeonBoss22h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_22h.VO_DALA_BOSS_22h_Female_Pandaren_BossDalaranLibrarian_02);
    }
  }
}
