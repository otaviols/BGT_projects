using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Faelin_Dungeon : BoH_Faelin_MissionEntity
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

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Faelin_Dungeon boHFaelinDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (boHFaelinDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) boHFaelinDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Faelin_Dungeon boHFaelinDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      boHFaelinDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    boHFaelinDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) boHFaelinDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Faelin_Dungeon boHFaelinDungeon = this;
    while (boHFaelinDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHFaelinDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num = (double) Random.Range(0.0f, 1f);
      boHFaelinDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 600:
          boHFaelinDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          boHFaelinDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          boHFaelinDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          boHFaelinDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          boHFaelinDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          boHFaelinDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          boHFaelinDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (boHFaelinDungeon.m_PlayPlayerVOLineIndex + 1 >= boHFaelinDungeon.m_PlayerVOLines.Count)
            boHFaelinDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++boHFaelinDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(boHFaelinDungeon.m_PlayerVOLines[boHFaelinDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) boHFaelinDungeon.PlayBossLine(actor, boHFaelinDungeon.m_PlayerVOLines[boHFaelinDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(boHFaelinDungeon.m_PlayerVOLines[boHFaelinDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) boHFaelinDungeon.PlayBossLine(actor, boHFaelinDungeon.m_PlayerVOLines[boHFaelinDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (boHFaelinDungeon.m_PlayBossVOLineIndex + 1 >= boHFaelinDungeon.m_BossVOLines.Count)
            boHFaelinDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++boHFaelinDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(boHFaelinDungeon.m_BossVOLines[boHFaelinDungeon.m_PlayBossVOLineIndex]);
          yield return (object) boHFaelinDungeon.PlayBossLine(enemyActor, boHFaelinDungeon.m_BossVOLines[boHFaelinDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(boHFaelinDungeon.m_BossVOLines[boHFaelinDungeon.m_PlayBossVOLineIndex]);
          yield return (object) boHFaelinDungeon.PlayBossLine(enemyActor, boHFaelinDungeon.m_BossVOLines[boHFaelinDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (boHFaelinDungeon.m_forceAlwaysPlayLine)
          {
            boHFaelinDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          boHFaelinDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in boHFaelinDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) boHFaelinDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in boHFaelinDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) boHFaelinDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in boHFaelinDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) boHFaelinDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in boHFaelinDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) boHFaelinDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHFaelinDungeon.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }
}
