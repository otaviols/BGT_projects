using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DALA_Dungeon_Boss_06h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_Attack_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_Attack_01.prefab:15dd88b078005e54397f95c95af81ff2");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_BossWarriorSpell_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_BossWarriorSpell_01.prefab:7df6ea496dcc4ba4d8c1e23092de4ed4");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_Death_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_Death_01.prefab:7f93d455de1f91c4682c69f5d06baa1c");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_DefeatPlayer_01.prefab:d6a0cdbb8f4d3834281fc2ead4c4f57f");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_EmoteResponse_01.prefab:9b9908062f8314b488e5fd02ae670bbf");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseGreetings_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseGreetings_01.prefab:41c01f6cc71c1264586f6759706dd570");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseOops_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseOops_01.prefab:8c76cee51431f63429a8b76cbd8d5f22");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThanks_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThanks_01.prefab:f8ed42fac5f4ce846819e6d426fd7f1a");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThreaten_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThreaten_01.prefab:b5565c9a7f3be1a4ea0a394f84eb5b02");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThreaten_02 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThreaten_02.prefab:ec94bb20b735a6f46b5ce7fda59183f8");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseWellPlayed_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseWellPlayed_01.prefab:9c8f2cb7e807ad94487803044118c4af");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseWow_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseWow_01.prefab:e326467f25f85b5429f2ae90ec58dd76");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_HeroPower_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_HeroPower_01.prefab:8acd66684adcd034782705d51f2c3c4d");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_HeroPower_02 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_HeroPower_02.prefab:ce05eddba59e90243a39fee727a34f19");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_HeroPower_03 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_HeroPower_03.prefab:6bd3befa9b1da504bbe60aac688d31df");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_Intro_03 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_Intro_03.prefab:ad88627146b8feb43b5b56b2a464971e");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_PlayerArmor_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_PlayerArmor_01.prefab:4698fd92375f0d34c901d1c6a21635d6");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_PlayerDKCardHeroic_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_PlayerDKCardHeroic_01.prefab:6d3ae02e263997347b5cac50397c435e");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_PlayerGorehowl_01 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_PlayerGorehowl_01.prefab:40a4a78c4165e8d47945ae7eebf3bfb0");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_Attack_03 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_Attack_03.prefab:665540cff61744b4098f744b5bc4ba51");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_BossCharge_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_BossCharge_01.prefab:81ac521d762da4b458a615e86a17f553");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_BossGorehowl_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_BossGorehowl_01.prefab:8a121f8af0bdba3428842f0eccc174c0");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseGreetings_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseGreetings_01.prefab:68b4c1b71f8931c49ae9addd1481a851");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseOops_02 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseOops_02.prefab:ca3fee93b5f9e9a4c86f8a474acaf109");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseThanks_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseThanks_01.prefab:d0468e6da7afc114cb20ccf3a8cbd137");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseThreaten_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseThreaten_01.prefab:23948cc4b84fd4f4cae96938fc821cdc");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseWellPlayed_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseWellPlayed_01.prefab:5ffe0d163d1bd2a4d96be52faacf53d7");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseWow_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseWow_01.prefab:a8f2ca41f54b2fe48be84789555bdb43");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_01.prefab:a55235537a698a04ab1a69ba7ff3f16e");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_02 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_02.prefab:8a96a7cf141bc524c914ed457c6bfe58");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_03 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_03.prefab:aa9cb380c83542542869235c6e7973b6");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_PlayerScourgelord_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_PlayerScourgelord_01.prefab:df08d25d7cb0e604795cf8e89ac58d45");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_PlayerWhirlwind_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_PlayerWhirlwind_01.prefab:976141c1bb0621a48813994be3d3e11f");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_Idle_01 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_Idle_01.prefab:b70421d5abdf9b840aed30d2245115a4");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_Idle_02 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_Idle_02.prefab:57f5de88521bbb342aae4e3f922f9a77");
  private static readonly AssetReference VO_DALA_BOSS_06dk_Male_Goblin_Idle_03 = new AssetReference("VO_DALA_BOSS_06dk_Male_Goblin_Idle_03.prefab:53eed559da61c304b98e666d9fe03378");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_Idle_02 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_Idle_02.prefab:4b985e77744ef30429120e25c285bf28");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_Idle_03 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_Idle_03.prefab:4ca2a710058db864db917dd0bce87b5a");
  private static readonly AssetReference VO_DALA_BOSS_06h_Male_Goblin_Idle_05 = new AssetReference("VO_DALA_BOSS_06h_Male_Goblin_Idle_05.prefab:cdfd7e9dc8a0dac4c85ac6af2aa68477");
  private static readonly string DAZZIK_CARDID = "DALA_BOSS_06h";
  private static readonly string DAZZIK_DK_CARDID = "DALA_BOSS_06dk";
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Idle_02,
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Idle_03,
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Idle_05
  };
  private static List<string> m_DKIdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_Idle_01,
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_Idle_02,
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_Idle_03
  };
  private List<string> m_IdleLinesCopy = new List<string>((IEnumerable<string>) DALA_Dungeon_Boss_06h.m_IdleLines);
  private List<string> m_DKIdleLinesCopy = new List<string>((IEnumerable<string>) DALA_Dungeon_Boss_06h.m_DKIdleLines);
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_HeroPower_01,
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_HeroPower_02,
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_HeroPower_03
  };
  private List<string> m_DKHeroPowerLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_01,
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_02,
    (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Attack_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_BossWarriorSpell_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Death_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseGreetings_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseOops_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThanks_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThreaten_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThreaten_02,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseWellPlayed_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseWow_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_HeroPower_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_HeroPower_02,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_HeroPower_03,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Intro_03,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_PlayerArmor_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_PlayerDKCardHeroic_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_PlayerGorehowl_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_Attack_03,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_BossCharge_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_BossGorehowl_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseGreetings_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseOops_02,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseThanks_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseThreaten_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseWellPlayed_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseWow_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_02,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_HeroPower_03,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_PlayerScourgelord_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_PlayerWhirlwind_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_Idle_01,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_Idle_02,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_Idle_03,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Idle_02,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Idle_03,
      (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Idle_05
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Intro_03;
    this.m_deathLine = (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Death_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    string cardId2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (cardId1 == DALA_Dungeon_Boss_06h.DAZZIK_CARDID && emoteType == EmoteType.START && cardId2 != "DALA_Chu" && cardId2 != "DALA_George")
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    if (cardId1 == DALA_Dungeon_Boss_06h.DAZZIK_DK_CARDID)
    {
      switch (emoteType)
      {
        case EmoteType.GREETINGS:
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseGreetings_01, Notification.SpeechBubbleDirection.TopRight, actor));
          break;
        case EmoteType.WELL_PLAYED:
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseWellPlayed_01, Notification.SpeechBubbleDirection.TopRight, actor));
          break;
        case EmoteType.OOPS:
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseOops_02, Notification.SpeechBubbleDirection.TopRight, actor));
          break;
        case EmoteType.THREATEN:
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseThreaten_01, Notification.SpeechBubbleDirection.TopRight, actor));
          break;
        case EmoteType.THANKS:
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseThanks_01, Notification.SpeechBubbleDirection.TopRight, actor));
          break;
        case EmoteType.WOW:
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseWow_01, Notification.SpeechBubbleDirection.TopRight, actor));
          break;
        default:
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_EmoteResponseGreetings_01, Notification.SpeechBubbleDirection.TopRight, actor));
          break;
      }
    }
    if (!(cardId1 == DALA_Dungeon_Boss_06h.DAZZIK_CARDID))
      return;
    switch (emoteType)
    {
      case EmoteType.GREETINGS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseGreetings_01, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.WELL_PLAYED:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseWellPlayed_01, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.OOPS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseOops_01, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.THREATEN:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThreaten_01, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.THANKS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseThanks_01, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.WOW:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponseWow_01, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      default:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_EmoteResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
    }
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_06h dalaDungeonBoss06h = this;
    while (dalaDungeonBoss06h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_BossWarriorSpell_01);
        break;
      case 102:
        yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_PlayerArmor_01);
        break;
      case 103:
        yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_Attack_03);
        break;
      case 104:
        yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_Attack_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss06h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_06h dalaDungeonBoss06h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss06h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss06h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss06h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss06h.WaitForEntitySoundToFinish(entity);
      string cardID = entity.GetCardId();
      dalaDungeonBoss06h.m_playedLines.Add(cardID);
      string enemyHeroCardID = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      if (enemyHeroCardID == DALA_Dungeon_Boss_06h.DAZZIK_CARDID)
      {
        switch (cardID)
        {
          case "EX1_411":
            yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_PlayerGorehowl_01);
            break;
          case "ICC_481":
          case "ICC_827":
          case "ICC_828":
          case "ICC_829":
          case "ICC_830":
          case "ICC_831":
          case "ICC_832":
          case "ICC_833":
          case "ICC_834":
            yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06h_Male_Goblin_PlayerDKCardHeroic_01);
            break;
        }
      }
      if (enemyHeroCardID == DALA_Dungeon_Boss_06h.DAZZIK_DK_CARDID)
      {
        string str = cardID;
        if (!(str == "ICC_834"))
        {
          if (str == "EX1_400")
            yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_PlayerWhirlwind_01);
        }
        else
          yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_PlayerScourgelord_01);
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_06h dalaDungeonBoss06h = this;
    while (dalaDungeonBoss06h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss06h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      if (entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
        dalaDungeonBoss06h.OnBossHeroPowerPlayed(entity);
      yield return (object) dalaDungeonBoss06h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId() == DALA_Dungeon_Boss_06h.DAZZIK_DK_CARDID)
      {
        if (!(cardId == "CS2_103"))
        {
          if (cardId == "EX1_411")
            yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_BossGorehowl_01);
        }
        else
          yield return (object) dalaDungeonBoss06h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_06h.VO_DALA_BOSS_06dk_Male_Goblin_BossCharge_01);
      }
    }
  }

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    if ((double) this.GetThinkEmoteBossThinkChancePercentage() > (double) Random.Range(1f, 101f))
    {
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      if (cardId == DALA_Dungeon_Boss_06h.DAZZIK_CARDID)
      {
        string line = this.PopRandomLine(this.m_IdleLinesCopy);
        if (this.m_IdleLinesCopy.Count == 0)
          this.m_IdleLinesCopy = new List<string>((IEnumerable<string>) DALA_Dungeon_Boss_06h.m_IdleLines);
        Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, line));
      }
      else
      {
        if (!(cardId == DALA_Dungeon_Boss_06h.DAZZIK_DK_CARDID))
          return;
        string line = this.PopRandomLine(this.m_DKIdleLinesCopy);
        if (this.m_DKIdleLinesCopy.Count == 0)
          this.m_DKIdleLinesCopy = new List<string>((IEnumerable<string>) DALA_Dungeon_Boss_06h.m_DKIdleLines);
        Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, line));
      }
    }
    else
    {
      EmoteType emoteType = EmoteType.THINK1;
      switch (Random.Range(1, 4))
      {
        case 1:
          emoteType = EmoteType.THINK1;
          break;
        case 2:
          emoteType = EmoteType.THINK2;
          break;
        case 3:
          emoteType = EmoteType.THINK3;
          break;
      }
      GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType);
    }
  }

  protected override void OnBossHeroPowerPlayed(Entity entity)
  {
    float bossHeroPowerVoLine = this.ChanceToPlayBossHeroPowerVOLine();
    float num = Random.Range(0.0f, 1f);
    if (this.m_enemySpeaking || (double) bossHeroPowerVoLine < (double) num)
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if ((Object) actor == (Object) null)
      return;
    List<string> stringList = (List<string>) null;
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    if (cardId == DALA_Dungeon_Boss_06h.DAZZIK_CARDID)
      stringList = this.m_HeroPowerLines;
    if (cardId == DALA_Dungeon_Boss_06h.DAZZIK_DK_CARDID)
      stringList = this.m_DKHeroPowerLines;
    string soundPath = "";
    while (stringList.Count > 0)
    {
      int index = Random.Range(0, stringList.Count);
      soundPath = stringList[index];
      stringList.RemoveAt(index);
      if (!NotificationManager.Get().HasSoundPlayedThisSession(soundPath))
        break;
    }
    if (soundPath == "")
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeechOnce(soundPath, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId() != DALA_Dungeon_Boss_06h.DAZZIK_CARDID || this.m_enemySpeaking || string.IsNullOrEmpty(this.m_deathLine) || gameResult != TAG_PLAYSTATE.WON)
      return;
    if (this.GetShouldSuppressDeathTextBubble())
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_deathLine, Notification.SpeechBubbleDirection.None, actor));
    else
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_deathLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }
}
