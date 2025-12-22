using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOM_05_Tamsin_Dungeon : BOM_05_Tamsin_MissionEntity
{
  public readonly AssetReference Brukan_BrassRing = new AssetReference("Brukan_BrassRing_Quote.prefab:16aa2801dfe06db489bd2259944af32b");
  public readonly AssetReference Tamsin_BrassRing = new AssetReference("Tamsin_BrassRing_Quote.prefab:62964357f9958d64f9346685fc1f87f5");
  public readonly AssetReference Dawngrasp_BrassRing = new AssetReference("Dawngrasp_BrassRing_Quote.prefab:45d9ad7c018bcf7429f8ff3d10e2aaf0");
  public readonly AssetReference Cariel_BrassRing_Quote = new AssetReference("Cariel_BrassRing_Quote.prefab:0a68b69767569144c8001265992df14f");
  public readonly AssetReference Xyrella2_BrassRing_Quote = new AssetReference("Xyrella2_BrassRing_Quote.prefab:d239b219d1d4962448ce25db0c6d4d28");
  public readonly AssetReference Hamuul_20_4_BrassRing_Quote = new AssetReference("Hamuul_20_4_BrassRing_Quote.prefab:54c037c90dc48994b8db6374e72f32ab");
  public readonly AssetReference Naralex_BrassRing = new AssetReference("Tavish_BrassRing_Quote.prefab:ad6adae48f4bfba4da53b7138111c1e3");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_01.prefab:53f58e50aac9ccc41a764aff34c50340");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_02 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_02.prefab:e5614706798e2cf43ac1fca0e2581af8");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_03 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_03.prefab:e1577bcbb62807e45aa6c808714db2e7");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_04 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_04.prefab:97404acfc3770224fb1d352abadce4fa");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_05 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_05.prefab:ba5c601009dc72f4da982fa94abd7e7c");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_06 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_06.prefab:6246f9c7f3340b14f89b396dc3cc05fe");
  private static readonly AssetReference VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_01 = new AssetReference("VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_01.prefab:ba9ea9cc6632a5e44883797e594f9b66");
  private static readonly AssetReference VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_02 = new AssetReference("VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_02.prefab:42ba2603fb3d75b499fdcab3041574d8");
  private static readonly AssetReference VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_04 = new AssetReference("VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_04.prefab:63a368c752c74d343ad61f4ec3b38642");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_02 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_02.prefab:60004d0fb7385e547a8224910590ae8e");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_03 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_03.prefab:5c82e945fe93bc241bd3e0e8e7a24dea");
  private static readonly AssetReference VO_Story_Minion_Cariel_Female_Human_EndDormant_03 = new AssetReference("VO_Story_Minion_Cariel_Female_Human_EndDormant_03.prefab:fc225626c1d1ae14a8b509bc46ada31c");
  private static readonly AssetReference VO_PVPDR_Hero_Cariel_Female_Human_Greetings_01 = new AssetReference("VO_PVPDR_Hero_Cariel_Female_Human_Greetings_01.prefab:d9f8ecfd0c9012f439f6eba42606e077");
  private static readonly AssetReference VO_PVPDR_Hero_Cariel_Female_Human_Start_01 = new AssetReference("VO_PVPDR_Hero_Cariel_Female_Human_Start_01.prefab:81050842099d21642acc88629c917953");
  private List<string> m_Xyrella_HeroPowerLines = new List<string>()
  {
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_01,
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_02,
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_03,
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_04,
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_05,
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_06
  };
  private List<string> m_Scabbs_HeroPowerLines = new List<string>()
  {
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_01,
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_02,
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_04
  };
  private List<string> m_Tavish_HeroPowerLines = new List<string>()
  {
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_02,
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_03
  };
  private List<string> m_Cariel_HeroPowerLines = new List<string>()
  {
    (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Cariel_Female_Human_EndDormant_03,
    (string) BOM_05_Tamsin_Dungeon.VO_PVPDR_Hero_Cariel_Female_Human_Greetings_01,
    (string) BOM_05_Tamsin_Dungeon.VO_PVPDR_Hero_Cariel_Female_Human_Start_01
  };
  public bool CarielIsHeroPower;
  public bool ScabbsIsHeroPower;
  public bool TavishIsHeroPower;
  public bool XyrellaIsHeroPower;

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
    List<string> VOLines = new List<string>()
    {
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_01,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_02,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_03,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_04,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_05,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_06,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_02,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_03,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_01,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_02,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_04,
      (string) BOM_05_Tamsin_Dungeon.VO_Story_Minion_Cariel_Female_Human_EndDormant_03,
      (string) BOM_05_Tamsin_Dungeon.VO_PVPDR_Hero_Cariel_Female_Human_Greetings_01,
      (string) BOM_05_Tamsin_Dungeon.VO_PVPDR_Hero_Cariel_Female_Human_Start_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.BOM;

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_05_Tamsin_Dungeon bom05TamsinDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom05TamsinDungeon.\u003C\u003En__0(entity);
    yield return (object) bom05TamsinDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_05_Tamsin_Dungeon bom05TamsinDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      bom05TamsinDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    bom05TamsinDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) bom05TamsinDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_05_Tamsin_Dungeon bom05TamsinDungeon = this;
    while (bom05TamsinDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (bom05TamsinDungeon.m_enemySpeaking)
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
      bom05TamsinDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      double num2 = (double) Random.Range(0.0f, 1f);
      switch (missionEvent)
      {
        case 508:
          if (bom05TamsinDungeon.CarielIsHeroPower)
            yield return (object) bom05TamsinDungeon.MissionPlaySound(friendlyHeroPowerActor, bom05TamsinDungeon.m_Cariel_HeroPowerLines);
          if (bom05TamsinDungeon.XyrellaIsHeroPower)
            yield return (object) bom05TamsinDungeon.MissionPlaySound(friendlyHeroPowerActor, bom05TamsinDungeon.m_Xyrella_HeroPowerLines);
          if (bom05TamsinDungeon.ScabbsIsHeroPower)
            yield return (object) bom05TamsinDungeon.MissionPlaySound(friendlyHeroPowerActor, bom05TamsinDungeon.m_Scabbs_HeroPowerLines);
          if (!bom05TamsinDungeon.TavishIsHeroPower)
            break;
          yield return (object) bom05TamsinDungeon.MissionPlaySound(friendlyHeroPowerActor, bom05TamsinDungeon.m_Tavish_HeroPowerLines);
          break;
        case 516:
          if (bom05TamsinDungeon.m_SupressEnemyDeathTextBubble)
          {
            yield return (object) bom05TamsinDungeon.MissionPlaySound(enemyActor, bom05TamsinDungeon.m_deathLine);
            break;
          }
          yield return (object) bom05TamsinDungeon.MissionPlayVO(enemyActor, bom05TamsinDungeon.m_deathLine);
          break;
        case 600:
          bom05TamsinDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          bom05TamsinDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          bom05TamsinDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          bom05TamsinDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          bom05TamsinDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          bom05TamsinDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          bom05TamsinDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom05TamsinDungeon.m_PlayPlayerVOLineIndex + 1 >= bom05TamsinDungeon.m_PlayerVOLines.Count)
            bom05TamsinDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++bom05TamsinDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(bom05TamsinDungeon.m_PlayerVOLines[bom05TamsinDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom05TamsinDungeon.PlayBossLine(actor, bom05TamsinDungeon.m_PlayerVOLines[bom05TamsinDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom05TamsinDungeon.m_PlayerVOLines[bom05TamsinDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom05TamsinDungeon.PlayBossLine(actor, bom05TamsinDungeon.m_PlayerVOLines[bom05TamsinDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom05TamsinDungeon.m_PlayBossVOLineIndex + 1 >= bom05TamsinDungeon.m_BossVOLines.Count)
            bom05TamsinDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++bom05TamsinDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(bom05TamsinDungeon.m_BossVOLines[bom05TamsinDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom05TamsinDungeon.PlayBossLine(enemyActor, bom05TamsinDungeon.m_BossVOLines[bom05TamsinDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom05TamsinDungeon.m_BossVOLines[bom05TamsinDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom05TamsinDungeon.PlayBossLine(enemyActor, bom05TamsinDungeon.m_BossVOLines[bom05TamsinDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (bom05TamsinDungeon.m_forceAlwaysPlayLine)
          {
            bom05TamsinDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          bom05TamsinDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom05TamsinDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom05TamsinDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in bom05TamsinDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom05TamsinDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom05TamsinDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom05TamsinDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in bom05TamsinDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom05TamsinDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        case 58024:
          bom05TamsinDungeon.CarielIsHeroPower = false;
          bom05TamsinDungeon.XyrellaIsHeroPower = true;
          bom05TamsinDungeon.ScabbsIsHeroPower = false;
          bom05TamsinDungeon.TavishIsHeroPower = false;
          break;
        case 58025:
          bom05TamsinDungeon.CarielIsHeroPower = false;
          bom05TamsinDungeon.XyrellaIsHeroPower = false;
          bom05TamsinDungeon.ScabbsIsHeroPower = true;
          bom05TamsinDungeon.TavishIsHeroPower = false;
          break;
        case 58026:
          bom05TamsinDungeon.CarielIsHeroPower = false;
          bom05TamsinDungeon.XyrellaIsHeroPower = false;
          bom05TamsinDungeon.ScabbsIsHeroPower = false;
          bom05TamsinDungeon.TavishIsHeroPower = true;
          break;
        case 58027:
          bom05TamsinDungeon.CarielIsHeroPower = true;
          bom05TamsinDungeon.XyrellaIsHeroPower = false;
          bom05TamsinDungeon.ScabbsIsHeroPower = false;
          bom05TamsinDungeon.TavishIsHeroPower = false;
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) bom05TamsinDungeon.\u003C\u003En__1(missionEvent);
          break;
      }
    }
  }
}
