using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOM_09_Brukan_Dungeon : BOM_09_Brukan_MissionEntity
{
  public static AssetReference SWDawngraspMinion_BrassRing_Quote = new AssetReference("SWDawngraspMinion_BrassRing_Quote.prefab:dfa0a79775ba5c34ea888cd56c91f517");
  public static AssetReference Brukan_20_4_BrassRing_Quote = new AssetReference("Brukan_20_4_BrassRing_Quote.prefab:8bece690907cc3b4897efce42d839510");
  public static AssetReference Guff_BrassRing_Quote = new AssetReference("GruffTier5_BrassRing_Quote.prefab:199725ce3c5e52043915e19a0a880c71");
  public static AssetReference Rokara_B_BrassRing_Quote = new AssetReference("Rokara_B_BrassRing_Quote.prefab:301c3d7a32636944884d6fa120099950");
  public static AssetReference Cariel_BrassRing_Quote = new AssetReference("Cariel_BrassRing_Quote.prefab:f92b72ab12fd34a4db73d365311ceb20");
  public static AssetReference Kurtrus_Stormwind_BrassRing_Quote = new AssetReference("Kurtrus_Stormwind_BrassRing_Quote.prefab:76cde32559de9c643af479d3f38970a8");
  public static AssetReference Tavish4_BrassRing_Quote = new AssetReference("Tavish4_BrassRing_Quote.prefab:28458b58b7d010d42b0bda2ff89683e9");
  public static AssetReference Cariel5_BrassRing_Quote = new AssetReference("Cariel5_BrassRing_Quote.prefab:7accbc43ce82f9c40adcdfd07b81bde6");
  public static AssetReference Scabbs5_BrassRing_Quote = new AssetReference("Scabbs5_BrassRing_Quote.prefab:7a1b7f2bbe41dd0409b4571f5c37452b");
  public static AssetReference Tamsin5_BrassRing_Quote = new AssetReference("Tamsin5_BrassRing_Quote.prefab:24d7633f9befde240b99a096f129dbd0");
  public static AssetReference Xyrella3_BrassRing_Quote = new AssetReference("Xyrella3_BrassRing_Quote.prefab:5ac0e3f7c01211944b826ee68336ae51");
  public static AssetReference KurtrusTier5_BrassRing_Quote = new AssetReference("KurtrusTier5_BrassRing_Quote.prefab:4fd3809b8824f6e40bc72288fddb573a");
  public static AssetReference Alterac_XyrellaArt_BrassRing_Quote = new AssetReference("Alterac_XyrellaArt_BrassRing_Quote.prefab:ace91814810897646a63ca8c9ada2fdd");
  public static AssetReference Alterac_CarielArt_BrassRing_Quote = new AssetReference("Alterac_CarielArt_BrassRing_Quote.prefab:8ff3e82fb40459349b450ddaa59e3ecd");
  public static AssetReference Alterac_GuffHero_BrassRing_Quote = new AssetReference("Alterac_GuffHero_BrassRing_Quote.prefab:22fb1479b121d3e4488052fb7d9d674c");
  public static AssetReference Alterac_TavishArt_BrassRing_Quote = new AssetReference("Alterac_TavishArt_BrassRing_Quote.prefab:2b66fcb0841100942b78a0a057e8b705");
  public static AssetReference RokaraTier5_BrassRing_Quote = new AssetReference("RokaraTier5_BrassRing_Quote.prefab:e508949e4c942624798c72b30774529e");
  public static AssetReference DawngraspTier5_BrassRing_Quote = new AssetReference("DawngraspTier5_BrassRing_Quote.prefab:f33138a364e51624092b1307fdec7f70");
  public static AssetReference MinerTogwaggle_BrassRing_Quote = new AssetReference("MinerTogwaggle_BrassRing_Quote.prefab:97cedc05a1267d142a72df6dcd8ca8bc");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_01.prefab:c40bb67529e2f75488a687b29e366dce");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_02 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_02.prefab:1bd2a973a10a50f449fa853434c5c19c");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_03 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_03.prefab:de12dc2c97279184aae3cc11e1756c0c");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_01 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_01.prefab:782455e872e6bc147938f97842d9817e");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_02 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_02.prefab:6f7943e790fab104d86b4d19c6ce975b");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_03 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_03.prefab:ad35989c054538b4da47fd868b74a657");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_01 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_01.prefab:41c1f8fa116752f46af51caeb6caeed9");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_02 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_02.prefab:c5d62462456c86d4991db76dc7a2bde1");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_03 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_03.prefab:2b8ab2e036415d14e934f46f7f1c2d6d");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_01.prefab:6cc5a32c122f74f47afad43a2625ccab");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_02 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_02.prefab:56dc1b62b35fd6141bdaefa9e544381f");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_03 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_03.prefab:1bf8a68524d3dad41bc7755a7987d208");
  private List<string> m_Cariel_HeroPowerLines = new List<string>()
  {
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_01,
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_02,
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_03
  };
  private List<string> m_Kurtrus_HeroPowerLines = new List<string>()
  {
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_01,
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_02,
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_03
  };
  private List<string> m_Tavish_HeroPowerLines = new List<string>()
  {
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_01,
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_02,
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_03
  };
  private List<string> m_Xyrella_HeroPowerLines = new List<string>()
  {
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_01,
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_02,
    (string) BOM_09_Brukan_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_03
  };
  public bool HeroPowerIsCariel;
  public bool HeroPowerIsKurtrus;
  public bool HeroPowerIsTavish;
  public bool HeroPowerIsXyrella;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) null;
    this.m_deathLine = (string) null;
    this.m_standardEmoteResponseLine = (string) null;
    this.m_BossIdleLines = new List<string>((IEnumerable<string>) this.GetBossIdleLines());
    this.m_BossIdleLinesCopy = new List<string>((IEnumerable<string>) this.GetBossIdleLines());
    this.m_OverrideMusicTrack = MusicPlaylistType.Invalid;
    this.m_OverrideMulliganMusicTrack = MusicPlaylistType.Invalid;
    this.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
    this.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
    this.m_OverrideBossSubtext = (string) null;
    this.m_OverridePlayerSubtext = (string) null;
    this.m_SupressEnemyDeathTextBubble = false;
  }

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>();
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.BOM;

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_09_Brukan_Dungeon bom09BrukanDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom09BrukanDungeon.\u003C\u003En__0(entity);
    yield return (object) bom09BrukanDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_09_Brukan_Dungeon bom09BrukanDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      bom09BrukanDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    bom09BrukanDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) bom09BrukanDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_09_Brukan_Dungeon bom09BrukanDungeon = this;
    while (bom09BrukanDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (bom09BrukanDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num1 = (double) Random.Range(0.0f, 1f);
      bom09BrukanDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      double num2 = (double) Random.Range(0.0f, 1f);
      switch (missionEvent)
      {
        case 508:
          if (bom09BrukanDungeon.HeroPowerIsCariel)
            yield return (object) bom09BrukanDungeon.MissionPlaySound(friendlyHeroPowerActor, bom09BrukanDungeon.m_Cariel_HeroPowerLines);
          if (bom09BrukanDungeon.HeroPowerIsKurtrus)
            yield return (object) bom09BrukanDungeon.MissionPlaySound(friendlyHeroPowerActor, bom09BrukanDungeon.m_Kurtrus_HeroPowerLines);
          if (bom09BrukanDungeon.HeroPowerIsTavish)
            yield return (object) bom09BrukanDungeon.MissionPlaySound(friendlyHeroPowerActor, bom09BrukanDungeon.m_Tavish_HeroPowerLines);
          if (!bom09BrukanDungeon.HeroPowerIsXyrella)
            break;
          yield return (object) bom09BrukanDungeon.MissionPlaySound(friendlyHeroPowerActor, bom09BrukanDungeon.m_Xyrella_HeroPowerLines);
          break;
        case 600:
          bom09BrukanDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          bom09BrukanDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          bom09BrukanDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          bom09BrukanDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          bom09BrukanDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          bom09BrukanDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          bom09BrukanDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom09BrukanDungeon.m_PlayPlayerVOLineIndex + 1 >= bom09BrukanDungeon.m_PlayerVOLines.Count)
            bom09BrukanDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++bom09BrukanDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(bom09BrukanDungeon.m_PlayerVOLines[bom09BrukanDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom09BrukanDungeon.PlayBossLine(actor, bom09BrukanDungeon.m_PlayerVOLines[bom09BrukanDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom09BrukanDungeon.m_PlayerVOLines[bom09BrukanDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom09BrukanDungeon.PlayBossLine(actor, bom09BrukanDungeon.m_PlayerVOLines[bom09BrukanDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom09BrukanDungeon.m_PlayBossVOLineIndex + 1 >= bom09BrukanDungeon.m_BossVOLines.Count)
            bom09BrukanDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++bom09BrukanDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(bom09BrukanDungeon.m_BossVOLines[bom09BrukanDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom09BrukanDungeon.PlayBossLine(enemyActor, bom09BrukanDungeon.m_BossVOLines[bom09BrukanDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom09BrukanDungeon.m_BossVOLines[bom09BrukanDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom09BrukanDungeon.PlayBossLine(enemyActor, bom09BrukanDungeon.m_BossVOLines[bom09BrukanDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (bom09BrukanDungeon.m_forceAlwaysPlayLine)
          {
            bom09BrukanDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          bom09BrukanDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom09BrukanDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom09BrukanDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in bom09BrukanDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom09BrukanDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom09BrukanDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom09BrukanDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in bom09BrukanDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom09BrukanDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        case 58024:
          bom09BrukanDungeon.HeroPowerIsCariel = true;
          bom09BrukanDungeon.HeroPowerIsKurtrus = false;
          bom09BrukanDungeon.HeroPowerIsTavish = false;
          bom09BrukanDungeon.HeroPowerIsXyrella = false;
          break;
        case 58025:
          bom09BrukanDungeon.HeroPowerIsCariel = false;
          bom09BrukanDungeon.HeroPowerIsKurtrus = true;
          bom09BrukanDungeon.HeroPowerIsTavish = false;
          bom09BrukanDungeon.HeroPowerIsXyrella = false;
          break;
        case 58026:
          bom09BrukanDungeon.HeroPowerIsCariel = false;
          bom09BrukanDungeon.HeroPowerIsKurtrus = false;
          bom09BrukanDungeon.HeroPowerIsTavish = true;
          bom09BrukanDungeon.HeroPowerIsXyrella = false;
          break;
        case 58027:
          bom09BrukanDungeon.HeroPowerIsCariel = false;
          bom09BrukanDungeon.HeroPowerIsKurtrus = false;
          bom09BrukanDungeon.HeroPowerIsTavish = false;
          bom09BrukanDungeon.HeroPowerIsXyrella = true;
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) bom09BrukanDungeon.\u003C\u003En__1(missionEvent);
          break;
      }
    }
  }
}
