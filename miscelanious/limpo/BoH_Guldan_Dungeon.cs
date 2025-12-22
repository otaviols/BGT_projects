using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Guldan_Dungeon : BoH_Guldan_MissionEntity
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
    this.m_Mission_FriendlyPlayIdleLines = false;
  }

  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.BOH;

  public static BoH_Guldan_Dungeon InstantiateTemplate_SoloDungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    Log.All.PrintError("BoH_Guldan_Dungeon.InstantiateTemplate_SoloDungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame));
    return new BoH_Guldan_Dungeon();
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_Dungeon boHGuldanDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (boHGuldanDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) boHGuldanDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_Dungeon boHGuldanDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      boHGuldanDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    boHGuldanDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) boHGuldanDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Guldan_Dungeon boHGuldanDungeon = this;
    while (boHGuldanDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHGuldanDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num = (double) Random.Range(0.0f, 1f);
      boHGuldanDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 600:
          boHGuldanDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          boHGuldanDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          boHGuldanDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          boHGuldanDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          boHGuldanDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          boHGuldanDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          boHGuldanDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (boHGuldanDungeon.m_PlayPlayerVOLineIndex + 1 >= boHGuldanDungeon.m_PlayerVOLines.Count)
            boHGuldanDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++boHGuldanDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(boHGuldanDungeon.m_PlayerVOLines[boHGuldanDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) boHGuldanDungeon.PlayBossLine(actor, boHGuldanDungeon.m_PlayerVOLines[boHGuldanDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(boHGuldanDungeon.m_PlayerVOLines[boHGuldanDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) boHGuldanDungeon.PlayBossLine(actor, boHGuldanDungeon.m_PlayerVOLines[boHGuldanDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (boHGuldanDungeon.m_PlayBossVOLineIndex + 1 >= boHGuldanDungeon.m_BossVOLines.Count)
            boHGuldanDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++boHGuldanDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(boHGuldanDungeon.m_BossVOLines[boHGuldanDungeon.m_PlayBossVOLineIndex]);
          yield return (object) boHGuldanDungeon.PlayBossLine(enemyActor, boHGuldanDungeon.m_BossVOLines[boHGuldanDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(boHGuldanDungeon.m_BossVOLines[boHGuldanDungeon.m_PlayBossVOLineIndex]);
          yield return (object) boHGuldanDungeon.PlayBossLine(enemyActor, boHGuldanDungeon.m_BossVOLines[boHGuldanDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (boHGuldanDungeon.m_forceAlwaysPlayLine)
          {
            boHGuldanDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          boHGuldanDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in boHGuldanDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) boHGuldanDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in boHGuldanDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) boHGuldanDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in boHGuldanDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) boHGuldanDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in boHGuldanDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) boHGuldanDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHGuldanDungeon.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }
}
