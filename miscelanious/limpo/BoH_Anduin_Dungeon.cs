using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Anduin_Dungeon : BoH_Andiun_MissionEntity
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
    BoH_Anduin_Dungeon boHAnduinDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (boHAnduinDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) boHAnduinDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Anduin_Dungeon boHAnduinDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      boHAnduinDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    boHAnduinDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) boHAnduinDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Anduin_Dungeon boHAnduinDungeon = this;
    while (boHAnduinDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHAnduinDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num = (double) Random.Range(0.0f, 1f);
      boHAnduinDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 600:
          boHAnduinDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          boHAnduinDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          boHAnduinDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          boHAnduinDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          boHAnduinDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          boHAnduinDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          boHAnduinDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (boHAnduinDungeon.m_PlayPlayerVOLineIndex + 1 >= boHAnduinDungeon.m_PlayerVOLines.Count)
            boHAnduinDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++boHAnduinDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(boHAnduinDungeon.m_PlayerVOLines[boHAnduinDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) boHAnduinDungeon.PlayBossLine(actor, boHAnduinDungeon.m_PlayerVOLines[boHAnduinDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(boHAnduinDungeon.m_PlayerVOLines[boHAnduinDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) boHAnduinDungeon.PlayBossLine(actor, boHAnduinDungeon.m_PlayerVOLines[boHAnduinDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (boHAnduinDungeon.m_PlayBossVOLineIndex + 1 >= boHAnduinDungeon.m_BossVOLines.Count)
            boHAnduinDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++boHAnduinDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(boHAnduinDungeon.m_BossVOLines[boHAnduinDungeon.m_PlayBossVOLineIndex]);
          yield return (object) boHAnduinDungeon.PlayBossLine(enemyActor, boHAnduinDungeon.m_BossVOLines[boHAnduinDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(boHAnduinDungeon.m_BossVOLines[boHAnduinDungeon.m_PlayBossVOLineIndex]);
          yield return (object) boHAnduinDungeon.PlayBossLine(enemyActor, boHAnduinDungeon.m_BossVOLines[boHAnduinDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (boHAnduinDungeon.m_forceAlwaysPlayLine)
          {
            boHAnduinDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          boHAnduinDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in boHAnduinDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) boHAnduinDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in boHAnduinDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) boHAnduinDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in boHAnduinDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) boHAnduinDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in boHAnduinDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) boHAnduinDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHAnduinDungeon.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }
}
