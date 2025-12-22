using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_55h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_BossIronforgePortal_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_BossIronforgePortal_01.prefab:a71e219608fcdd04aa4a7d4776c1a558");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_BossWeaponEquip_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_BossWeaponEquip_01.prefab:55d140ff585b4b64397788bd07c54a76");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_Death_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_Death_01.prefab:7fa220001eeb478418dd7b7309d03c10");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_DefeatPlayer_01.prefab:a912782e2b587744793d2d18b41b02e6");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_EmoteResponse_01.prefab:53656d2bc6cffb64b99c6bb72b9fd792");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_Exposition_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_Exposition_01.prefab:7b0f11701b6c2b04ea634a560ae1acbf");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_Exposition_02 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_Exposition_02.prefab:4e0c4d020ba2a8943aadcd87980c01bb");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_HeroPower_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_HeroPower_01.prefab:c638bc2846bb6c141b58fd66b74c1fc4");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_HeroPower_02 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_HeroPower_02.prefab:464d1914a1522984c8849d9205ffc1a0");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_HeroPower_04 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_HeroPower_04.prefab:f75f61a614eea2c48ac339eecba56bc9");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_HeroPower_05 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_HeroPower_05.prefab:f1c267858447f2f4ea8dba80caeffb39");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_HeroPowerFull_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_HeroPowerFull_01.prefab:2cd82130b7e62d04db7495d009f7d51f");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_01.prefab:d3dea3a9cf41a764f97a3ee848cf5c5f");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_02 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_02.prefab:06adc3a5006373144860e6cce662733d");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_03 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_03.prefab:a3e3f29b886948240831a44c2c9207fc");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_Idle_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_Idle_01.prefab:0b0b7eacdfb36454cb6f324dba55090d");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_Idle_02 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_Idle_02.prefab:24ae67e54b0a0fd459d810f2f5b02d5a");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_Idle_03 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_Idle_03.prefab:99dd91211c559cf4083ae5ac757c44f4");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_Intro_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_Intro_01.prefab:4c7229cf7ae7880489ae19653691c1db");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_IntroGeorge_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_IntroGeorge_01.prefab:8ae3e8bbd355a584c9af8ecbba373abc");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_PlayerBigBoomba_02 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_PlayerBigBoomba_02.prefab:a3f6f447aac8f9b4ba621c8308ea0b10");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_PlayerHyperblaster_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_PlayerHyperblaster_01.prefab:e594eecd9eb04bc479ef16d5989a9b48");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_PlayerLackey_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_PlayerLackey_01.prefab:73a116e98338275469094d59195d1013");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_PlayerVillainLegendary_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_PlayerVillainLegendary_01.prefab:d1a78bd5a712a8b4cb8600d82d5b0c96");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_PlayerVioletWarden_01 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_PlayerVioletWarden_01.prefab:f47bdd589e1ee614c852c3eb092df7a9");
  private static readonly AssetReference VO_DALA_BOSS_55h_Male_Human_TurnOne_02 = new AssetReference("VO_DALA_BOSS_55h_Male_Human_TurnOne_02.prefab:41bb39d61593b7c42a519d772521aeac");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Idle_01,
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Idle_02,
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Idle_03
  };
  private static List<string> m_BossHeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPower_01,
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPower_02,
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPower_04,
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPower_05
  };
  private static List<string> m_HeroPowerMany = new List<string>()
  {
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_01,
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_02,
    (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_BossIronforgePortal_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_BossWeaponEquip_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Death_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Exposition_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Exposition_02,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPower_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPower_02,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPower_04,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPower_05,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPowerFull_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_02,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPowerMany_03,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Idle_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Idle_02,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Idle_03,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Intro_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_IntroGeorge_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerBigBoomba_02,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerHyperblaster_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerLackey_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerVillainLegendary_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerVioletWarden_01,
      (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_TurnOne_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_55h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_George")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_IntroGeorge_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "DALA_Eudora"))
          return;
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
      }
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
    DALA_Dungeon_Boss_55h dalaDungeonBoss55h = this;
    while (dalaDungeonBoss55h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_TurnOne_02);
        break;
      case 102:
        yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Exposition_02);
        break;
      case 103:
        yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_Exposition_01);
        break;
      case 104:
        yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_BossWeaponEquip_01);
        break;
      case 105:
        yield return (object) dalaDungeonBoss55h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_55h.m_BossHeroPower);
        break;
      case 106:
        yield return (object) dalaDungeonBoss55h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_55h.m_HeroPowerMany);
        break;
      case 107:
        yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_HeroPowerFull_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss55h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_55h dalaDungeonBoss55h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss55h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss55h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss55h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss55h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss55h.m_playedLines.Add(cardId);
      switch (cardId)
      {
        case "DALA_723":
          yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerHyperblaster_01);
          break;
        case "DALA_724":
          yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerBigBoomba_02);
          break;
        case "DAL_096":
          yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerVioletWarden_01);
          break;
        case "DAL_417":
        case "DAL_422":
        case "DAL_431":
        case "LOE_092":
          yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerVillainLegendary_01);
          break;
        case "DAL_613":
        case "DAL_614":
        case "DAL_615":
        case "DAL_739":
        case "DAL_741":
          yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_PlayerLackey_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_55h dalaDungeonBoss55h = this;
    while (dalaDungeonBoss55h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss55h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss55h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss55h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "KAR_091")
        yield return (object) dalaDungeonBoss55h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_55h.VO_DALA_BOSS_55h_Male_Human_BossIronforgePortal_01);
    }
  }
}
