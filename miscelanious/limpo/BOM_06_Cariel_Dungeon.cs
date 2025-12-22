using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOM_06_Cariel_Dungeon : BOM_06_Cariel_MissionEntity
{
  public readonly AssetReference Cariel_BrassRing_Quote = new AssetReference("Cariel_BrassRing_Quote.prefab:0a68b69767569144c8001265992df14f");
  public readonly AssetReference Cornelius_BrassRing_Quote = new AssetReference("Cornelius_BrassRing_Quote.prefab:c9573291191b0484e88657d665d844a8");
  public readonly AssetReference YoungCornelius_BrassRing_Quote = new AssetReference("YoungCornelius_BrassRing_Quote.prefab:99b9f6f88001de544a711cd375d85ea7");
  public readonly AssetReference Tamsin_BrassRing = new AssetReference("Tamsin4_BrassRing_Quote.prefab:8cb215bc36bd2854fa000e5f6453a338");
  public readonly AssetReference Tavish4_BrassRing_Quote = new AssetReference("Tavish4_BrassRing_Quote.prefab:28458b58b7d010d42b0bda2ff89683e9");
  public readonly AssetReference Kurtrus_Stormwind_BrassRing_Quote = new AssetReference("Kurtrus_Stormwind_BrassRing_Quote.prefab:76cde32559de9c643af479d3f38970a8");

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
    BOM_06_Cariel_Dungeon bom06CarielDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom06CarielDungeon.\u003C\u003En__0(entity);
    yield return (object) bom06CarielDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_Dungeon bom06CarielDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      bom06CarielDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    bom06CarielDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) bom06CarielDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_06_Cariel_Dungeon bom06CarielDungeon = this;
    while (bom06CarielDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (bom06CarielDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num1 = (double) Random.Range(0.0f, 1f);
      bom06CarielDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      double num2 = (double) Random.Range(0.0f, 1f);
      switch (missionEvent)
      {
        case 516:
          if (bom06CarielDungeon.m_SupressEnemyDeathTextBubble)
          {
            yield return (object) bom06CarielDungeon.MissionPlaySound(enemyActor, bom06CarielDungeon.m_deathLine);
            break;
          }
          yield return (object) bom06CarielDungeon.MissionPlayVO(enemyActor, bom06CarielDungeon.m_deathLine);
          break;
        case 600:
          bom06CarielDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          bom06CarielDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          bom06CarielDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          bom06CarielDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          bom06CarielDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          bom06CarielDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          bom06CarielDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom06CarielDungeon.m_PlayPlayerVOLineIndex + 1 >= bom06CarielDungeon.m_PlayerVOLines.Count)
            bom06CarielDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++bom06CarielDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(bom06CarielDungeon.m_PlayerVOLines[bom06CarielDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom06CarielDungeon.PlayBossLine(actor, bom06CarielDungeon.m_PlayerVOLines[bom06CarielDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom06CarielDungeon.m_PlayerVOLines[bom06CarielDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom06CarielDungeon.PlayBossLine(actor, bom06CarielDungeon.m_PlayerVOLines[bom06CarielDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom06CarielDungeon.m_PlayBossVOLineIndex + 1 >= bom06CarielDungeon.m_BossVOLines.Count)
            bom06CarielDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++bom06CarielDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(bom06CarielDungeon.m_BossVOLines[bom06CarielDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom06CarielDungeon.PlayBossLine(enemyActor, bom06CarielDungeon.m_BossVOLines[bom06CarielDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom06CarielDungeon.m_BossVOLines[bom06CarielDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom06CarielDungeon.PlayBossLine(enemyActor, bom06CarielDungeon.m_BossVOLines[bom06CarielDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (bom06CarielDungeon.m_forceAlwaysPlayLine)
          {
            bom06CarielDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          bom06CarielDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom06CarielDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom06CarielDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in bom06CarielDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom06CarielDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom06CarielDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom06CarielDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in bom06CarielDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom06CarielDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) bom06CarielDungeon.\u003C\u003En__1(missionEvent);
          break;
      }
    }
  }
}
