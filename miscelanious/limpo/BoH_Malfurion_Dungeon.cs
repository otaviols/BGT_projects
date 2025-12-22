using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Malfurion_Dungeon : BoH_Malfurion_MissionEntity
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
    BoH_Malfurion_Dungeon malfurionDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (malfurionDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) malfurionDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Malfurion_Dungeon malfurionDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      malfurionDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    malfurionDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) malfurionDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Malfurion_Dungeon malfurionDungeon = this;
    while (malfurionDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (malfurionDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num = (double) Random.Range(0.0f, 1f);
      malfurionDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 600:
          malfurionDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          malfurionDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          malfurionDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          malfurionDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          malfurionDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          malfurionDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          malfurionDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (malfurionDungeon.m_PlayPlayerVOLineIndex + 1 >= malfurionDungeon.m_PlayerVOLines.Count)
            malfurionDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++malfurionDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(malfurionDungeon.m_PlayerVOLines[malfurionDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) malfurionDungeon.PlayBossLine(actor, malfurionDungeon.m_PlayerVOLines[malfurionDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(malfurionDungeon.m_PlayerVOLines[malfurionDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) malfurionDungeon.PlayBossLine(actor, malfurionDungeon.m_PlayerVOLines[malfurionDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (malfurionDungeon.m_PlayBossVOLineIndex + 1 >= malfurionDungeon.m_BossVOLines.Count)
            malfurionDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++malfurionDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(malfurionDungeon.m_BossVOLines[malfurionDungeon.m_PlayBossVOLineIndex]);
          yield return (object) malfurionDungeon.PlayBossLine(enemyActor, malfurionDungeon.m_BossVOLines[malfurionDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(malfurionDungeon.m_BossVOLines[malfurionDungeon.m_PlayBossVOLineIndex]);
          yield return (object) malfurionDungeon.PlayBossLine(enemyActor, malfurionDungeon.m_BossVOLines[malfurionDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (malfurionDungeon.m_forceAlwaysPlayLine)
          {
            malfurionDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          malfurionDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in malfurionDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) malfurionDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in malfurionDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) malfurionDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in malfurionDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) malfurionDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in malfurionDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) malfurionDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) malfurionDungeon.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }
}
