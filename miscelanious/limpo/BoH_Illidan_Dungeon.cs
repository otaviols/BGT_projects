using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Illidan_Dungeon : BoH_Illidan_MissionEntity
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
    BoH_Illidan_Dungeon boHIllidanDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (boHIllidanDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) boHIllidanDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Illidan_Dungeon boHIllidanDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      boHIllidanDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    boHIllidanDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) boHIllidanDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Illidan_Dungeon boHIllidanDungeon = this;
    while (boHIllidanDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHIllidanDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num = (double) Random.Range(0.0f, 1f);
      boHIllidanDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 600:
          boHIllidanDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          boHIllidanDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          boHIllidanDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          boHIllidanDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          boHIllidanDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          boHIllidanDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          boHIllidanDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (boHIllidanDungeon.m_PlayPlayerVOLineIndex + 1 >= boHIllidanDungeon.m_PlayerVOLines.Count)
            boHIllidanDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++boHIllidanDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(boHIllidanDungeon.m_PlayerVOLines[boHIllidanDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) boHIllidanDungeon.PlayBossLine(actor, boHIllidanDungeon.m_PlayerVOLines[boHIllidanDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(boHIllidanDungeon.m_PlayerVOLines[boHIllidanDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) boHIllidanDungeon.PlayBossLine(actor, boHIllidanDungeon.m_PlayerVOLines[boHIllidanDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (boHIllidanDungeon.m_PlayBossVOLineIndex + 1 >= boHIllidanDungeon.m_BossVOLines.Count)
            boHIllidanDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++boHIllidanDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(boHIllidanDungeon.m_BossVOLines[boHIllidanDungeon.m_PlayBossVOLineIndex]);
          yield return (object) boHIllidanDungeon.PlayBossLine(enemyActor, boHIllidanDungeon.m_BossVOLines[boHIllidanDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(boHIllidanDungeon.m_BossVOLines[boHIllidanDungeon.m_PlayBossVOLineIndex]);
          yield return (object) boHIllidanDungeon.PlayBossLine(enemyActor, boHIllidanDungeon.m_BossVOLines[boHIllidanDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (boHIllidanDungeon.m_forceAlwaysPlayLine)
          {
            boHIllidanDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          boHIllidanDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in boHIllidanDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) boHIllidanDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in boHIllidanDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) boHIllidanDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in boHIllidanDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) boHIllidanDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in boHIllidanDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) boHIllidanDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHIllidanDungeon.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }
}
