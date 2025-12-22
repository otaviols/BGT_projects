using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOM_04_Kurtrus_Dungeon : BOM_04_Kurtrus_MissionEntity
{
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
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_01,
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_02,
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_03,
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_04,
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_05,
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_06
  };
  private List<string> m_Scabbs_HeroPowerLines = new List<string>()
  {
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_01,
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_02,
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_04
  };
  private List<string> m_Tavish_HeroPowerLines = new List<string>()
  {
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_02,
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_03
  };
  private List<string> m_Cariel_HeroPowerLines = new List<string>()
  {
    (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Cariel_Female_Human_EndDormant_03,
    (string) BOM_04_Kurtrus_Dungeon.VO_PVPDR_Hero_Cariel_Female_Human_Greetings_01,
    (string) BOM_04_Kurtrus_Dungeon.VO_PVPDR_Hero_Cariel_Female_Human_Start_01
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
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_01,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_02,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_03,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_04,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_05,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_HPHeal_06,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_02,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Trigger_03,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_01,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_02,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Scabbs_Male_Gnome_Story_Xyrella_Trigger_04,
      (string) BOM_04_Kurtrus_Dungeon.VO_Story_Minion_Cariel_Female_Human_EndDormant_03,
      (string) BOM_04_Kurtrus_Dungeon.VO_PVPDR_Hero_Cariel_Female_Human_Greetings_01,
      (string) BOM_04_Kurtrus_Dungeon.VO_PVPDR_Hero_Cariel_Female_Human_Start_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.BOM;

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_04_Kurtrus_Dungeon obj = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) obj.\u003C\u003En__0(entity);
    yield return (object) obj.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_04_Kurtrus_Dungeon obj = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      obj.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    obj.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) obj.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_04_Kurtrus_Dungeon obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (obj.m_enemySpeaking)
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
      obj.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      double num2 = (double) Random.Range(0.0f, 1f);
      switch (missionEvent)
      {
        case 508:
          if (obj.CarielIsHeroPower)
            yield return (object) obj.MissionPlaySound(friendlyHeroPowerActor, obj.m_Cariel_HeroPowerLines);
          if (obj.XyrellaIsHeroPower)
            yield return (object) obj.MissionPlaySound(friendlyHeroPowerActor, obj.m_Xyrella_HeroPowerLines);
          if (obj.ScabbsIsHeroPower)
            yield return (object) obj.MissionPlaySound(friendlyHeroPowerActor, obj.m_Scabbs_HeroPowerLines);
          if (!obj.TavishIsHeroPower)
            break;
          yield return (object) obj.MissionPlaySound(friendlyHeroPowerActor, obj.m_Tavish_HeroPowerLines);
          break;
        case 516:
          if (obj.m_SupressEnemyDeathTextBubble)
          {
            yield return (object) obj.MissionPlaySound(enemyActor, obj.m_deathLine);
            break;
          }
          yield return (object) obj.MissionPlayVO(enemyActor, obj.m_deathLine);
          break;
        case 600:
          obj.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          obj.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          obj.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          obj.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          obj.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          obj.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          obj.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (obj.m_PlayPlayerVOLineIndex + 1 >= obj.m_PlayerVOLines.Count)
            obj.m_PlayPlayerVOLineIndex = 0;
          else
            ++obj.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(obj.m_PlayerVOLines[obj.m_PlayPlayerVOLineIndex]);
          yield return (object) obj.PlayBossLine(actor, obj.m_PlayerVOLines[obj.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(obj.m_PlayerVOLines[obj.m_PlayPlayerVOLineIndex]);
          yield return (object) obj.PlayBossLine(actor, obj.m_PlayerVOLines[obj.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (obj.m_PlayBossVOLineIndex + 1 >= obj.m_BossVOLines.Count)
            obj.m_PlayBossVOLineIndex = 0;
          else
            ++obj.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(obj.m_BossVOLines[obj.m_PlayBossVOLineIndex]);
          yield return (object) obj.PlayBossLine(enemyActor, obj.m_BossVOLines[obj.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(obj.m_BossVOLines[obj.m_PlayBossVOLineIndex]);
          yield return (object) obj.PlayBossLine(enemyActor, obj.m_BossVOLines[obj.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (obj.m_forceAlwaysPlayLine)
          {
            obj.m_forceAlwaysPlayLine = false;
            break;
          }
          obj.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in obj.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) obj.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in obj.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) obj.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in obj.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) obj.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in obj.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) obj.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        case 58024:
          obj.CarielIsHeroPower = false;
          obj.XyrellaIsHeroPower = true;
          obj.ScabbsIsHeroPower = false;
          obj.TavishIsHeroPower = false;
          break;
        case 58025:
          obj.CarielIsHeroPower = false;
          obj.XyrellaIsHeroPower = false;
          obj.ScabbsIsHeroPower = true;
          obj.TavishIsHeroPower = false;
          break;
        case 58026:
          obj.CarielIsHeroPower = false;
          obj.XyrellaIsHeroPower = false;
          obj.ScabbsIsHeroPower = false;
          obj.TavishIsHeroPower = true;
          break;
        case 58027:
          obj.CarielIsHeroPower = true;
          obj.XyrellaIsHeroPower = false;
          obj.ScabbsIsHeroPower = false;
          obj.TavishIsHeroPower = false;
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) obj.\u003C\u003En__1(missionEvent);
          break;
      }
    }
  }
}
