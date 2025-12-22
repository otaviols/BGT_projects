using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_MagicalGuardians_Dungeon : TB_MagicalGuardians_MissionEntity
{
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

  public static TB_MagicalGuardians_Dungeon InstantiateTemplate_SoloDungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    Log.All.PrintError("TB_MagicalGuardians_Dungeon.InstantiateTemplate_SoloDungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame));
    return new TB_MagicalGuardians_Dungeon();
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    TB_MagicalGuardians_Dungeon guardiansDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) guardiansDungeon.\u003C\u003En__0(entity);
    yield return (object) guardiansDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_Dungeon guardiansDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      guardiansDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    guardiansDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) guardiansDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_MagicalGuardians_Dungeon guardiansDungeon = this;
    while (guardiansDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (guardiansDungeon.m_enemySpeaking)
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
      guardiansDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      double num2 = (double) Random.Range(0.0f, 1f);
      switch (missionEvent)
      {
        case 600:
          guardiansDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          guardiansDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          guardiansDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          guardiansDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          guardiansDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          guardiansDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          guardiansDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (guardiansDungeon.m_PlayPlayerVOLineIndex + 1 >= guardiansDungeon.m_PlayerVOLines.Count)
            guardiansDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++guardiansDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(guardiansDungeon.m_PlayerVOLines[guardiansDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) guardiansDungeon.PlayBossLine(actor, guardiansDungeon.m_PlayerVOLines[guardiansDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(guardiansDungeon.m_PlayerVOLines[guardiansDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) guardiansDungeon.PlayBossLine(actor, guardiansDungeon.m_PlayerVOLines[guardiansDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (guardiansDungeon.m_PlayBossVOLineIndex + 1 >= guardiansDungeon.m_BossVOLines.Count)
            guardiansDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++guardiansDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(guardiansDungeon.m_BossVOLines[guardiansDungeon.m_PlayBossVOLineIndex]);
          yield return (object) guardiansDungeon.PlayBossLine(enemyActor, guardiansDungeon.m_BossVOLines[guardiansDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(guardiansDungeon.m_BossVOLines[guardiansDungeon.m_PlayBossVOLineIndex]);
          yield return (object) guardiansDungeon.PlayBossLine(enemyActor, guardiansDungeon.m_BossVOLines[guardiansDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (guardiansDungeon.m_forceAlwaysPlayLine)
          {
            guardiansDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          guardiansDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in guardiansDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) guardiansDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in guardiansDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) guardiansDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in guardiansDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) guardiansDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in guardiansDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) guardiansDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) guardiansDungeon.\u003C\u003En__1(missionEvent);
          break;
      }
    }
  }
}
