using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoM_01_Rokara_Dungeon : BoM_01_Rokara_MissionEntity
{
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_01.prefab:11a175183db47a2479e85d896681ace2");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_02.prefab:0f5afd55abea8414ab0666ad2d554856");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_03.prefab:76aeab237ec8c5249afe67a0818dfd27");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_04 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_04.prefab:4d14e36f878dc0b4d942948de1889d6e");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_05 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_05.prefab:2e39f62db6c882e4f995500a61a10d05");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_06 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_06.prefab:380b013abcc400940967646bf74d1b8c");
  private List<string> m_Brukan_HeroPowerLines = new List<string>()
  {
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_01,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_02,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_03,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_05,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_06
  };
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_01.prefab:76d1d6750f4df5f42b9be52198b91d26");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_02 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_02.prefab:7a343d0c99bbcba40a35d7da20c4e63a");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_03 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_03.prefab:f3fc710cf6c0f77459668006ee89ff24");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_04 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_04.prefab:43e03a31a84886344bbddf022687ca4b");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_06 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_06.prefab:a318145d17c8edf4f8558d02cdac8bfd");
  private List<string> m_Guff_HeroPowerLines = new List<string>()
  {
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_01,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_03,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_04,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_06
  };
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_01.prefab:a087f78e1d15dd14f8a9ea9161837d12");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_02.prefab:a4b7a5b42073a5345ae27e15ec092f67");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_03 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_03.prefab:f72faaa261994144c9e392a8b623b21c");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_04 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_04.prefab:ac3868d46ef9a834c8230c4b9fa26e8c");
  private List<string> m_Tamsin_HeroPowerLines = new List<string>()
  {
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_01,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_02,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_04
  };
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_01 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_01.prefab:18a6357110fe5f143ace127d06ea34ba");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_02 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_02.prefab:0c2f296e8512e684680879cdac1cc46b");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_03 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_03.prefab:c72eda15e76cfc44fae7e09af9993f42");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_04 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_04.prefab:17f4aa15f1baa8f42b4ebef24fb8c799");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_05 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_05.prefab:13846171e1eb4824288706cb62c3a910");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_06 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_06.prefab:959a3b20fe5da1f4c8675fbd0d8a6dca");
  private List<string> m_Dawngrasp_HeroPowerLines = new List<string>()
  {
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_01,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_02,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_03,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_04,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_05,
    (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_06
  };
  public bool HeroPowerIsBrukan;
  public bool HeroPowerIsGuff;
  public bool HeroPowerIsTamsin;
  public bool HeroPowerIsDawngrasp;
  public readonly AssetReference Garrosh_BrassRing = new AssetReference("Garrosh_BrassRing_Quote.prefab:9c911310fb2bf7246ae78ef14a1b4dc5");
  public readonly AssetReference Brukan_BrassRing = new AssetReference("Brukan_BrassRing_Quote.prefab:16aa2801dfe06db489bd2259944af32b");
  public readonly AssetReference Guff_BrassRing = new AssetReference("Guff_BrassRing_Quote.prefab:2b02f1e9a212d7e41ace41f997923b8a");
  public readonly AssetReference Rokara_B_BrassRing = new AssetReference("Rokara_B_BrassRing_Quote.prefab:301c3d7a32636944884d6fa120099950");
  public readonly AssetReference Tamsin_BrassRing = new AssetReference("Tamsin_BrassRing_Quote.prefab:62964357f9958d64f9346685fc1f87f5");
  public readonly AssetReference Dawngrasp_BrassRing = new AssetReference("Dawngrasp_BrassRing_Quote.prefab:45d9ad7c018bcf7429f8ff3d10e2aaf0");
  public readonly AssetReference Kazakus_BrassRing = new AssetReference("Kazakus_BrassRing_Quote.prefab:74f40b18119e73f4fb7b8bc9c3f9b70f");

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
    this.m_SupressEnemyDeathTextBubble = true;
  }

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_01,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_02,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_03,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_04,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_05,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission5HeroPower_06,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_01,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_02,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_03,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4HeroPower_04,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_01,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_02,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_03,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_04,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_05,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission2HeroPower_06,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_01,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_02,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_03,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_04,
      (string) BoM_01_Rokara_Dungeon.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission3HeroPower_06
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.BOM;

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoM_01_Rokara_Dungeon m01RokaraDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) m01RokaraDungeon.\u003C\u003En__0(entity);
    yield return (object) m01RokaraDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoM_01_Rokara_Dungeon m01RokaraDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      m01RokaraDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    m01RokaraDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) m01RokaraDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoM_01_Rokara_Dungeon m01RokaraDungeon = this;
    while (m01RokaraDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (m01RokaraDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num = (double) Random.Range(0.0f, 1f);
      m01RokaraDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 508:
          if (m01RokaraDungeon.HeroPowerIsBrukan)
            yield return (object) m01RokaraDungeon.MissionPlaySound(friendlyHeroPowerActor, m01RokaraDungeon.m_Brukan_HeroPowerLines);
          if (m01RokaraDungeon.HeroPowerIsGuff)
            yield return (object) m01RokaraDungeon.MissionPlaySound(friendlyHeroPowerActor, m01RokaraDungeon.m_Guff_HeroPowerLines);
          if (m01RokaraDungeon.HeroPowerIsTamsin)
            yield return (object) m01RokaraDungeon.MissionPlaySound(friendlyHeroPowerActor, m01RokaraDungeon.m_Tamsin_HeroPowerLines);
          if (!m01RokaraDungeon.HeroPowerIsDawngrasp)
            break;
          yield return (object) m01RokaraDungeon.MissionPlaySound(friendlyHeroPowerActor, m01RokaraDungeon.m_Dawngrasp_HeroPowerLines);
          break;
        case 516:
          if (m01RokaraDungeon.m_SupressEnemyDeathTextBubble)
          {
            yield return (object) m01RokaraDungeon.MissionPlaySound(enemyActor, m01RokaraDungeon.m_deathLine);
            break;
          }
          yield return (object) m01RokaraDungeon.MissionPlayVO(enemyActor, m01RokaraDungeon.m_deathLine);
          break;
        case 600:
          m01RokaraDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          m01RokaraDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          m01RokaraDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          m01RokaraDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          m01RokaraDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          m01RokaraDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          m01RokaraDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (m01RokaraDungeon.m_PlayPlayerVOLineIndex + 1 >= m01RokaraDungeon.m_PlayerVOLines.Count)
            m01RokaraDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++m01RokaraDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(m01RokaraDungeon.m_PlayerVOLines[m01RokaraDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) m01RokaraDungeon.PlayBossLine(actor, m01RokaraDungeon.m_PlayerVOLines[m01RokaraDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(m01RokaraDungeon.m_PlayerVOLines[m01RokaraDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) m01RokaraDungeon.PlayBossLine(actor, m01RokaraDungeon.m_PlayerVOLines[m01RokaraDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (m01RokaraDungeon.m_PlayBossVOLineIndex + 1 >= m01RokaraDungeon.m_BossVOLines.Count)
            m01RokaraDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++m01RokaraDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(m01RokaraDungeon.m_BossVOLines[m01RokaraDungeon.m_PlayBossVOLineIndex]);
          yield return (object) m01RokaraDungeon.PlayBossLine(enemyActor, m01RokaraDungeon.m_BossVOLines[m01RokaraDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(m01RokaraDungeon.m_BossVOLines[m01RokaraDungeon.m_PlayBossVOLineIndex]);
          yield return (object) m01RokaraDungeon.PlayBossLine(enemyActor, m01RokaraDungeon.m_BossVOLines[m01RokaraDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (m01RokaraDungeon.m_forceAlwaysPlayLine)
          {
            m01RokaraDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          m01RokaraDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in m01RokaraDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) m01RokaraDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in m01RokaraDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) m01RokaraDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in m01RokaraDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) m01RokaraDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in m01RokaraDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) m01RokaraDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        case 58024:
          m01RokaraDungeon.HeroPowerIsBrukan = true;
          m01RokaraDungeon.HeroPowerIsGuff = false;
          m01RokaraDungeon.HeroPowerIsTamsin = false;
          m01RokaraDungeon.HeroPowerIsDawngrasp = false;
          break;
        case 58025:
          m01RokaraDungeon.HeroPowerIsBrukan = false;
          m01RokaraDungeon.HeroPowerIsGuff = true;
          m01RokaraDungeon.HeroPowerIsTamsin = false;
          m01RokaraDungeon.HeroPowerIsDawngrasp = false;
          break;
        case 58026:
          m01RokaraDungeon.HeroPowerIsBrukan = false;
          m01RokaraDungeon.HeroPowerIsGuff = false;
          m01RokaraDungeon.HeroPowerIsTamsin = true;
          m01RokaraDungeon.HeroPowerIsDawngrasp = false;
          break;
        case 58027:
          m01RokaraDungeon.HeroPowerIsBrukan = false;
          m01RokaraDungeon.HeroPowerIsGuff = false;
          m01RokaraDungeon.HeroPowerIsTamsin = false;
          m01RokaraDungeon.HeroPowerIsDawngrasp = true;
          break;
        case 58028:
          if (m01RokaraDungeon.m_MissionDisableAutomaticVO)
            break;
          GameState.Get().SetBusy(true);
          yield return (object) m01RokaraDungeon.MissionPlayVO("BOM_01_Brukan_08t", m01RokaraDungeon.m_Brukan_HeroPowerLines);
          GameState.Get().SetBusy(false);
          break;
        case 58029:
          if (m01RokaraDungeon.m_MissionDisableAutomaticVO)
            break;
          GameState.Get().SetBusy(true);
          yield return (object) m01RokaraDungeon.MissionPlayVOOnce("BOM_01_Guff_02t", m01RokaraDungeon.m_Guff_HeroPowerLines);
          GameState.Get().SetBusy(false);
          break;
        case 58030:
          if (m01RokaraDungeon.m_MissionDisableAutomaticVO)
            break;
          GameState.Get().SetBusy(true);
          yield return (object) m01RokaraDungeon.MissionPlayVOOnce("BOM_01_Tamsin_03t", m01RokaraDungeon.m_Tamsin_HeroPowerLines);
          GameState.Get().SetBusy(false);
          break;
        case 58031:
          if (m01RokaraDungeon.m_MissionDisableAutomaticVO)
            break;
          GameState.Get().SetBusy(true);
          yield return (object) m01RokaraDungeon.MissionPlayVOOnce("BOM_01_Dawngrasp_04t", m01RokaraDungeon.m_Dawngrasp_HeroPowerLines);
          GameState.Get().SetBusy(false);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) m01RokaraDungeon.\u003C\u003En__1(missionEvent);
          break;
      }
    }
  }
}
