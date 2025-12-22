using System.Collections;
using System.Collections.Generic;

public class BOM_01_Rokara_08 : BoM_01_Rokara_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Death_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Death_01.prefab:d57f499dd401e4f4c839e841707c9605");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8EmoteResponse_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8EmoteResponse_01.prefab:6d311296a889b904c9a29cb79dc089c2");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeA_01.prefab:18e711394b1278546a1c7ced7f50dd09");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_01.prefab:b130b40a08b2b1944b2b64fb1db64a7c");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_03.prefab:40c9e53c2f1e50e4ebc51cd4375eebe2");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeC_01.prefab:49b39039325ad3c4887d55ebb094baff");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeD_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeD_01.prefab:57bfb32acf9ca8648a4cb862f01d19c7");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeE_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeE_02.prefab:eb076d1bfa8e79b4f96b5dd8e910d986");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_01.prefab:cbef19348ba154f4bbf2bba2704688a2");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_02.prefab:2b284754b8d9e864a80ccc2229cfe166");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_01.prefab:cb5ff3787df26514d80e731b04e6f16d");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_02.prefab:61d04c10c9329a64f8a694ebae888320");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_03.prefab:18360fe4696cd7b448cc10d2624c5556");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_01.prefab:1a8a670ce5f8bf3428a8f3e41fd44217");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_02.prefab:39e9056e2c47fc74e9de0afa5ca9481d");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_03.prefab:3833873a266f11b4c9389705a24beed1");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Intro_01.prefab:a144fec3f78382f44a3d06c2b69c9ecb");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Loss_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Loss_01.prefab:c42ccb28859f6a24195679f062a5906f");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeA_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeA_02.prefab:a0d3e50809404814da5738a80f8c0e8a");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeB_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeB_02.prefab:4f4e12ba3fd558043b71cf14347ed9f8");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeC_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeC_02.prefab:6e8322ae52f3fe547ab4742e307c2468");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeD_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeD_02.prefab:93efbe7b4663c8a4b98db5de18bcb93b");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeE_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeE_01.prefab:31e86b085b455794a81a4827d9c48dbe");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8Intro_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8Intro_02.prefab:f55245828b083094da18790b620bab15");
  private List<string> m_VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeBLines = new List<string>()
  {
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_01,
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_03
  };
  private List<string> m_VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeFLines = new List<string>()
  {
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_01,
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_02
  };
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_01,
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_02,
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_03
  };
  private List<string> m_BossIdleLines2 = new List<string>()
  {
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_01,
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_02,
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_03
  };
  private List<string> m_IntroductionLines = new List<string>()
  {
    (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Intro_01,
    (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8Intro_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Death_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8EmoteResponse_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeA_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_03,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeC_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeD_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeE_02,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_02,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_02,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8HeroPower_03,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_02,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Idle_03,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Intro_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Loss_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeA_02,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeB_02,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeC_02,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeD_02,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeE_01,
      (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8Intro_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines2;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_TRLFinalBoss;
    this.m_deathLine = (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Death_01;
    this.m_standardEmoteResponseLine = (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_01_Rokara_08 bom01Rokara08 = this;
    while (bom01Rokara08.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 500:
        yield return (object) bom01Rokara08.MissionPlayVOOnce(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeC_01);
        yield return (object) bom01Rokara08.MissionPlayVOOnce(friendlyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeC_02);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_01);
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeF_02);
        GameState.Get().SetBusy(false);
        break;
      case 506:
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Loss_01);
        break;
      case 514:
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8Intro_01);
        yield return (object) bom01Rokara08.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8Intro_02);
        break;
      case 515:
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, bom01Rokara08.m_standardEmoteResponseLine);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) bom01Rokara08.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_01_Rokara_08 bom01Rokara08 = this;
    while (bom01Rokara08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom01Rokara08.\u003C\u003En__1(entity);
    if (!bom01Rokara08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) bom01Rokara08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom01Rokara08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_01_Rokara_08 bom01Rokara08 = this;
    while (bom01Rokara08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!bom01Rokara08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) bom01Rokara08.\u003C\u003En__2(entity);
      yield return (object) bom01Rokara08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom01Rokara08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BOM_01_Rokara_08 bom01Rokara08 = this;
    while (bom01Rokara08.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeA_01);
        yield return (object) bom01Rokara08.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeA_02);
        break;
      case 7:
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_01);
        yield return (object) bom01Rokara08.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeB_02);
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeB_03);
        break;
      case 15:
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeD_01);
        yield return (object) bom01Rokara08.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeD_02);
        break;
      case 19:
        yield return (object) bom01Rokara08.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission8ExchangeE_01);
        yield return (object) bom01Rokara08.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Rokara_Mission8ExchangeE_02);
        break;
    }
  }
}
