using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOM_07_Scabbs_Dungeon : BOM_07_Scabbs_MissionEntity
{
  public static AssetReference SWDawngraspMinion_BrassRing_Quote = new AssetReference("SWDawngraspMinion_BrassRing_Quote.prefab:dfa0a79775ba5c34ea888cd56c91f517");
  public static AssetReference Brukan_20_4_BrassRing_Quote = new AssetReference("Brukan_20_4_BrassRing_Quote.prefab:8bece690907cc3b4897efce42d839510");
  public static AssetReference Guff_BrassRing_Quote = new AssetReference("Guff_BrassRing_Quote.prefab:2b02f1e9a212d7e41ace41f997923b8a");
  public static AssetReference Rokara_B_BrassRing_Quote = new AssetReference("Rokara_B_BrassRing_Quote.prefab:301c3d7a32636944884d6fa120099950");
  public static AssetReference Xyrella2_BrassRing_Quote = new AssetReference("Xyrella2_BrassRing_Quote.prefab:d239b219d1d4962448ce25db0c6d4d28");
  public static AssetReference Cariel_BrassRing_Quote = new AssetReference("Cariel_BrassRing_Quote.prefab:f92b72ab12fd34a4db73d365311ceb20");
  public static AssetReference Kurtrus_Stormwind_BrassRing_Quote = new AssetReference("Kurtrus_Stormwind_BrassRing_Quote.prefab:76cde32559de9c643af479d3f38970a8");
  public static AssetReference Tavish4_BrassRing_Quote = new AssetReference("Tavish4_BrassRing_Quote.prefab:28458b58b7d010d42b0bda2ff89683e9");
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
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_01,
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_02,
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Cariel_Female_Human_BOM_Scabbs_HeroPower_03
  };
  private List<string> m_Kurtrus_HeroPowerLines = new List<string>()
  {
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_01,
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_02,
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Kurtrus_Male_NightElf_BOM_Scabbs_HeroPower_03
  };
  private List<string> m_Tavish_HeroPowerLines = new List<string>()
  {
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_01,
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_02,
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Scabbs_HeroPower_03
  };
  private List<string> m_Xyrella_HeroPowerLines = new List<string>()
  {
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_01,
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_02,
    (string) BOM_07_Scabbs_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_BOM_Scabbs_HeroPower_03
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
    BOM_07_Scabbs_Dungeon bom07ScabbsDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom07ScabbsDungeon.\u003C\u003En__0(entity);
    yield return (object) bom07ScabbsDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_07_Scabbs_Dungeon bom07ScabbsDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      bom07ScabbsDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    bom07ScabbsDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) bom07ScabbsDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_07_Scabbs_Dungeon bom07ScabbsDungeon = this;
    while (bom07ScabbsDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (bom07ScabbsDungeon.m_enemySpeaking)
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
      bom07ScabbsDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      double num2 = (double) Random.Range(0.0f, 1f);
      switch (missionEvent)
      {
        case 508:
          if (bom07ScabbsDungeon.HeroPowerIsCariel)
            yield return (object) bom07ScabbsDungeon.MissionPlaySound(friendlyHeroPowerActor, bom07ScabbsDungeon.m_Cariel_HeroPowerLines);
          if (bom07ScabbsDungeon.HeroPowerIsKurtrus)
            yield return (object) bom07ScabbsDungeon.MissionPlaySound(friendlyHeroPowerActor, bom07ScabbsDungeon.m_Kurtrus_HeroPowerLines);
          if (bom07ScabbsDungeon.HeroPowerIsTavish)
            yield return (object) bom07ScabbsDungeon.MissionPlaySound(friendlyHeroPowerActor, bom07ScabbsDungeon.m_Tavish_HeroPowerLines);
          if (!bom07ScabbsDungeon.HeroPowerIsXyrella)
            break;
          yield return (object) bom07ScabbsDungeon.MissionPlaySound(friendlyHeroPowerActor, bom07ScabbsDungeon.m_Xyrella_HeroPowerLines);
          break;
        case 600:
          bom07ScabbsDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          bom07ScabbsDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          bom07ScabbsDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          bom07ScabbsDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          bom07ScabbsDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          bom07ScabbsDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          bom07ScabbsDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom07ScabbsDungeon.m_PlayPlayerVOLineIndex + 1 >= bom07ScabbsDungeon.m_PlayerVOLines.Count)
            bom07ScabbsDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++bom07ScabbsDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(bom07ScabbsDungeon.m_PlayerVOLines[bom07ScabbsDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom07ScabbsDungeon.PlayBossLine(actor, bom07ScabbsDungeon.m_PlayerVOLines[bom07ScabbsDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom07ScabbsDungeon.m_PlayerVOLines[bom07ScabbsDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom07ScabbsDungeon.PlayBossLine(actor, bom07ScabbsDungeon.m_PlayerVOLines[bom07ScabbsDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom07ScabbsDungeon.m_PlayBossVOLineIndex + 1 >= bom07ScabbsDungeon.m_BossVOLines.Count)
            bom07ScabbsDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++bom07ScabbsDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(bom07ScabbsDungeon.m_BossVOLines[bom07ScabbsDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom07ScabbsDungeon.PlayBossLine(enemyActor, bom07ScabbsDungeon.m_BossVOLines[bom07ScabbsDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom07ScabbsDungeon.m_BossVOLines[bom07ScabbsDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom07ScabbsDungeon.PlayBossLine(enemyActor, bom07ScabbsDungeon.m_BossVOLines[bom07ScabbsDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (bom07ScabbsDungeon.m_forceAlwaysPlayLine)
          {
            bom07ScabbsDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          bom07ScabbsDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom07ScabbsDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom07ScabbsDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in bom07ScabbsDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom07ScabbsDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom07ScabbsDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom07ScabbsDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in bom07ScabbsDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom07ScabbsDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        case 58024:
          bom07ScabbsDungeon.HeroPowerIsCariel = true;
          bom07ScabbsDungeon.HeroPowerIsKurtrus = false;
          bom07ScabbsDungeon.HeroPowerIsTavish = false;
          bom07ScabbsDungeon.HeroPowerIsXyrella = false;
          break;
        case 58025:
          bom07ScabbsDungeon.HeroPowerIsCariel = false;
          bom07ScabbsDungeon.HeroPowerIsKurtrus = true;
          bom07ScabbsDungeon.HeroPowerIsTavish = false;
          bom07ScabbsDungeon.HeroPowerIsXyrella = false;
          break;
        case 58026:
          bom07ScabbsDungeon.HeroPowerIsCariel = false;
          bom07ScabbsDungeon.HeroPowerIsKurtrus = false;
          bom07ScabbsDungeon.HeroPowerIsTavish = true;
          bom07ScabbsDungeon.HeroPowerIsXyrella = false;
          break;
        case 58027:
          bom07ScabbsDungeon.HeroPowerIsCariel = false;
          bom07ScabbsDungeon.HeroPowerIsKurtrus = false;
          bom07ScabbsDungeon.HeroPowerIsTavish = false;
          bom07ScabbsDungeon.HeroPowerIsXyrella = true;
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) bom07ScabbsDungeon.\u003C\u003En__1(missionEvent);
          break;
      }
    }
  }
}
