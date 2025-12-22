using Assets;
using Blizzard.T5.Core;
using Hearthstone.DataModels;
using Hearthstone.DungeonCrawl;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureConfig : MonoBehaviour
{
  public const string DEFAULT_SET_UP_STATE = "SetUpState";
  public const string PLAY_BUTTON_ANOMALY_ACTIVE_STATE = "PURPLE_SWIRL";
  public const string PLAY_BUTTON_ANOMALY_INACTIVE_STATE = "BLUE_SWIRL";
  private static AdventureConfig s_instance;
  private AdventureDbId m_SelectedAdventure = AdventureDbId.PRACTICE;
  private AdventureModeDbId m_SelectedMode = AdventureModeDbId.LINEAR;
  private Stack<AdventureData.Adventuresubscene> m_SubSceneBackStack = new Stack<AdventureData.Adventuresubscene>();
  private AdventureData.Adventuresubscene m_CurrentSubScene;
  private AdventureData.Adventuresubscene m_PreviousSubScene = AdventureData.Adventuresubscene.INVALID;
  private ScenarioDbId m_SelectedMission;
  private ScenarioDbId m_MissionOverride;
  private bool m_anomalyModeActivated;
  private List<long> NeedsChapterNewlyUnlockedHighlight = new List<long>();
  private bool m_allChaptersOwned;
  private Reward.Type m_completionRewardType;
  private int m_completionRewardId;
  private List<AdventureConfig.AdventureModeChange> m_AdventureModeChangeEventList = new List<AdventureConfig.AdventureModeChange>();
  private List<AdventureConfig.SubSceneChange> m_SubSceneChangeEventList = new List<AdventureConfig.SubSceneChange>();
  private List<AdventureConfig.SelectedModeChange> m_SelectedModeChangeEventList = new List<AdventureConfig.SelectedModeChange>();
  private List<AdventureConfig.AdventureMissionSet> m_AdventureMissionSetEventList = new List<AdventureConfig.AdventureMissionSet>();
  private Map<string, int> m_WingBossesDefeatedCache = new Map<string, int>();
  private Map<string, ScenarioDbId> m_LastSelectedMissions = new Map<string, ScenarioDbId>();
  private Map<ScenarioDbId, bool> m_CachedDefeatedScenario = new Map<ScenarioDbId, bool>();
  private Map<ScenarioDbId, AdventureBossDef> m_CachedBossDef = new Map<ScenarioDbId, AdventureBossDef>();
  private Map<AdventureDbId, AdventureModeDbId> m_ClientChooserAdventureModes = new Map<AdventureDbId, AdventureModeDbId>();

  public static AdventureConfig Get() => AdventureConfig.s_instance;

  private AdventureDbId SelectedAdventure
  {
    get => this.m_SelectedAdventure;
    set
    {
      if (value != this.m_SelectedAdventure)
        this.ResetLoadout();
      this.m_SelectedAdventure = value;
      AdventureDataModel adventureDataModel = this.GetAdventureDataModel();
      if (adventureDataModel == null)
        return;
      adventureDataModel.SelectedAdventure = value;
      adventureDataModel.IsDuelsAdventure = AdventureUtils.IsDuelsAdventure(value);
      AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) value);
      adventureDataModel.StoreDescriptionTextTimelockedTrue = record != null ? (string) record.StoreBuyRemainingWingsDescTimelockedTrue : string.Empty;
      adventureDataModel.StoreDescriptionTextTimelockedFalse = record != null ? (string) record.StoreBuyRemainingWingsDescTimelockedFalse : string.Empty;
    }
  }

  private AdventureModeDbId SelectedMode
  {
    get => this.m_SelectedMode;
    set
    {
      if (value != this.m_SelectedMode)
        this.ResetLoadout();
      this.m_SelectedMode = value;
      AdventureDataModel adventureDataModel = this.GetAdventureDataModel();
      if (adventureDataModel == null)
        return;
      adventureDataModel.SelectedAdventureMode = value;
      adventureDataModel.IsSelectedModeHeroic = GameUtils.IsModeHeroic(value);
    }
  }

  public AdventureData.Adventuresubscene CurrentSubScene => this.m_CurrentSubScene;

  public AdventureData.Adventuresubscene PreviousSubScene => this.m_PreviousSubScene;

  public event AdventureConfig.AnomalyModeChangedHandler OnAnomalyModeChanged;

  public bool AnomalyModeActivated
  {
    get => this.m_anomalyModeActivated;
    set
    {
      if (value == this.m_anomalyModeActivated)
        return;
      this.m_anomalyModeActivated = value;
      AdventureDataModel adventureDataModel = this.GetAdventureDataModel();
      if (adventureDataModel != null)
        adventureDataModel.AnomalyActivated = this.m_anomalyModeActivated;
      if (this.OnAnomalyModeChanged == null)
        return;
      this.OnAnomalyModeChanged(value);
    }
  }

  public long SelectedHeroCardDbId { get; set; }

  public long SelectedLoadoutTreasureDbId { get; set; }

  public long SelectedDeckId { get; set; }

  public long SelectedHeroPowerDbId { get; set; }

  public bool ShouldSeeFirstTimeFlow
  {
    get => this.GetAdventureDataModel().ShouldSeeFirstTimeFlow;
    set => this.GetAdventureDataModel().ShouldSeeFirstTimeFlow = value;
  }

  public bool AllChaptersOwned
  {
    get => this.m_allChaptersOwned;
    set
    {
      this.m_allChaptersOwned = value;
      AdventureDataModel adventureDataModel = this.GetAdventureDataModel();
      if (adventureDataModel == null)
        return;
      adventureDataModel.AllChaptersOwned = this.m_allChaptersOwned;
    }
  }

  public RewardListDataModel CompletionRewards
  {
    get
    {
      AdventureDataModel adventureDataModel = this.GetAdventureDataModel();
      if (adventureDataModel == null)
        return (RewardListDataModel) null;
      if (adventureDataModel.CompletionRewards == null)
        adventureDataModel.CompletionRewards = new RewardListDataModel();
      return adventureDataModel.CompletionRewards;
    }
    set
    {
      AdventureDataModel adventureDataModel = this.GetAdventureDataModel();
      if (adventureDataModel == null)
        return;
      adventureDataModel.CompletionRewards = value;
    }
  }

  public Reward.Type CompletionRewardType
  {
    get => this.m_completionRewardType;
    set
    {
      this.m_completionRewardType = value;
      AdventureDataModel adventureDataModel = this.GetAdventureDataModel();
      if (adventureDataModel == null)
        return;
      adventureDataModel.CompletionRewardType = this.m_completionRewardType;
    }
  }

  public int CompletionRewardId
  {
    get => this.m_completionRewardId;
    set
    {
      this.m_completionRewardId = value;
      AdventureDataModel adventureDataModel = this.GetAdventureDataModel();
      if (adventureDataModel == null)
        return;
      adventureDataModel.CompletionRewardId = this.m_completionRewardId;
    }
  }

  public event Action OnAdventureSceneUnloadEvent;

  public static AdventureData.Adventuresubscene GetSubSceneFromMode(
    AdventureDbId adventureId,
    AdventureModeDbId modeId)
  {
    int adventureId1 = (int) adventureId;
    int modeId1 = (int) modeId;
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord(adventureId1, modeId1);
    if (adventureDataRecord == null)
    {
      Debug.LogErrorFormat("AdventureConfig.GetSubSceneFromMode() - No Adventure Data record found for Adventure {0} and Mode {1}", (object) adventureId1, (object) modeId1);
      return AdventureData.Adventuresubscene.CHOOSER;
    }
    return adventureDataRecord.StartingSubscene == AdventureData.Adventuresubscene.DUNGEON_CRAWL ? AdventureConfig.Get().GetCorrectSubSceneWhenLoadingDungeonCrawlMode() : adventureDataRecord.StartingSubscene;
  }

  public AdventureDbId GetSelectedAdventure() => this.SelectedAdventure;

  public AdventureModeDbId GetSelectedMode() => this.SelectedMode;

  public AdventureDataModel GetAdventureDataModel()
  {
    IDataModel model;
    if (!GlobalDataContext.Get().GetDataModel(7, out model))
    {
      model = (IDataModel) new AdventureDataModel();
      GlobalDataContext.Get().BindDataModel(model);
    }
    if (model is AdventureDataModel adventureDataModel)
      return adventureDataModel;
    Log.Adventures.PrintWarning("AdventureDataModel is null!");
    return adventureDataModel;
  }

  public AdventureDataDbfRecord GetSelectedAdventureDataRecord() => AdventureConfig.GetAdventureDataRecord(this.GetSelectedAdventure(), this.GetSelectedMode());

  public AdventureModeDbId GetClientChooserAdventureMode(
    AdventureDbId adventureDbId)
  {
    AdventureModeDbId chooserAdventureMode;
    if (this.m_ClientChooserAdventureModes.TryGetValue(adventureDbId, out chooserAdventureMode))
      return chooserAdventureMode;
    return this.SelectedAdventure != adventureDbId ? AdventureModeDbId.LINEAR : this.SelectedMode;
  }

  public static AdventureDataDbfRecord GetAdventureDataRecord(
    AdventureDbId adventureId,
    AdventureModeDbId modeId)
  {
    return GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureId && (AdventureModeDbId) r.ModeId == modeId));
  }

  public static bool CanPlayMode(
    AdventureDbId adventureId,
    AdventureModeDbId modeId,
    bool checkEventTimings = true)
  {
    bool flag = AchieveManager.Get().HasUnlockedFeature(Achieve.Unlocks.VANILLA_HEROES);
    if (adventureId == AdventureDbId.PRACTICE)
      return modeId != AdventureModeDbId.EXPERT || flag;
    if (!flag && AdventureUtils.DoesAdventureRequireAllHeroesUnlocked(adventureId, modeId))
      return false;
    return modeId == AdventureModeDbId.LINEAR || modeId == AdventureModeDbId.DUNGEON_CRAWL || GameDbf.Scenario.GetRecord((Predicate<ScenarioDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureId && (AdventureModeDbId) r.ModeId == modeId && r.WingId > 0 && AdventureProgressMgr.Get().CanPlayScenario(r.ID, checkEventTimings))) != null;
  }

  public static bool IsFeaturedMode(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    if (!AdventureConfig.CanPlayMode(adventureId, modeId))
      return false;
    Option fromAdventureData = AdventureConfig.GetHasSeenFeaturedModeOptionFromAdventureData(adventureId, modeId);
    return fromAdventureData != Option.INVALID && !Options.Get().GetBool(fromAdventureData, false);
  }

  public static bool MarkFeaturedMode(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    if (!AdventureConfig.CanPlayMode(adventureId, modeId))
      return false;
    Option fromAdventureData = AdventureConfig.GetHasSeenFeaturedModeOptionFromAdventureData(adventureId, modeId);
    if (fromAdventureData == Option.INVALID)
      return false;
    Options.Get().SetBool(fromAdventureData, true);
    return true;
  }

  public static bool ShouldShowNewModePopup(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    if (!AdventureConfig.CanPlayMode(adventureId, modeId))
      return false;
    Option fromAdventureData = AdventureConfig.GetHasSeenNewModePopupOptionFromAdventureData(adventureId, modeId);
    return fromAdventureData != Option.INVALID && !Options.Get().GetBool(fromAdventureData, false);
  }

  public static bool MarkHasSeenNewModePopup(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    if (!AdventureConfig.CanPlayMode(adventureId, modeId))
      return false;
    Option fromAdventureData = AdventureConfig.GetHasSeenNewModePopupOptionFromAdventureData(adventureId, modeId);
    if (fromAdventureData == Option.INVALID)
      return false;
    Options.Get().SetBool(fromAdventureData, true);
    return true;
  }

  private static Option GetHasSeenFeaturedModeOptionFromAdventureData(
    AdventureDbId adventureId,
    AdventureModeDbId modeId)
  {
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventureId, (int) modeId);
    return adventureDataRecord == null ? Option.INVALID : OptionUtils.GetOptionFromString(adventureDataRecord.HasSeenFeaturedModeOption);
  }

  private static Option GetHasSeenNewModePopupOptionFromAdventureData(
    AdventureDbId adventureId,
    AdventureModeDbId modeId)
  {
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventureId, (int) modeId);
    return adventureDataRecord == null ? Option.INVALID : OptionUtils.GetOptionFromString(adventureDataRecord.HasSeenNewModePopupOption);
  }

  public string GetSelectedAdventureAndModeString() => string.Format("{0}_{1}", (object) this.SelectedAdventure, (object) this.SelectedMode);

  public void SetSelectedAdventureMode(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    this.SelectedAdventure = adventureId;
    this.SelectedMode = modeId;
    this.m_ClientChooserAdventureModes[adventureId] = modeId;
    Options.Get().SetInt(Option.SELECTED_ADVENTURE, (int) this.SelectedAdventure);
    Options.Get().SetInt(Option.SELECTED_ADVENTURE_MODE, (int) this.SelectedMode);
    this.SetPropertiesForAdventureAndMode();
    this.FireSelectedModeChangeEvent();
  }

  public void MarkHasSeenFirstTimeFlowComplete()
  {
    if (GameUtils.IsModeHeroic(this.SelectedMode))
      return;
    AdventureDataDbfRecord record = GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == this.SelectedAdventure && (AdventureModeDbId) r.ModeId == this.SelectedMode));
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest((GameSaveKeyId) record.GameSaveDataClientKey, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_FIRST_TIME_FLOW, new long[1]
    {
      1L
    }));
    this.ShouldSeeFirstTimeFlow = false;
  }

  public void UpdateShouldSeeFirstTimeFlowForSelectedMode()
  {
    if (GameUtils.IsModeHeroic(this.SelectedMode))
    {
      this.ShouldSeeFirstTimeFlow = false;
    }
    else
    {
      long num = 0;
      AdventureDataDbfRecord record = GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == this.SelectedAdventure && (AdventureModeDbId) r.ModeId == this.SelectedMode));
      if (record == null)
        this.ShouldSeeFirstTimeFlow = true;
      else if (record.GameSaveDataClientKey <= 0)
      {
        this.ShouldSeeFirstTimeFlow = false;
      }
      else
      {
        GameSaveDataManager.Get().GetSubkeyValue((GameSaveKeyId) record.GameSaveDataClientKey, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_FIRST_TIME_FLOW, out num);
        this.ShouldSeeFirstTimeFlow = num <= 0L;
      }
    }
  }

  public static AdventureModeDbId GetDefaultModeDbIdForAdventure(
    AdventureDbId adventureId)
  {
    if (adventureId == AdventureDbId.INVALID)
      return AdventureModeDbId.INVALID;
    AdventureDataDbfRecord record = GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureId));
    return record == null ? AdventureModeDbId.INVALID : (AdventureModeDbId) record.ModeId;
  }

  public ScenarioDbId GetMission() => this.m_SelectedMission;

  public ScenarioDbId GetMissionToPlay() => this.m_MissionOverride == ScenarioDbId.INVALID ? this.GetMission() : this.m_MissionOverride;

  public ScenarioDbId GetLastSelectedMission()
  {
    string adventureAndModeString = this.GetSelectedAdventureAndModeString();
    ScenarioDbId lastSelectedMission = ScenarioDbId.INVALID;
    this.m_LastSelectedMissions.TryGetValue(adventureAndModeString, out lastSelectedMission);
    return lastSelectedMission;
  }

  public bool IsScenarioDefeatedAndInitCache(ScenarioDbId mission)
  {
    bool flag = AdventureProgressMgr.Get().HasDefeatedScenario((int) mission);
    if (!this.m_CachedDefeatedScenario.ContainsKey(mission))
      this.m_CachedDefeatedScenario[mission] = flag;
    return flag;
  }

  public bool IsScenarioJustDefeated(ScenarioDbId mission)
  {
    bool flag1 = AdventureProgressMgr.Get().HasDefeatedScenario((int) mission);
    bool flag2 = false;
    this.m_CachedDefeatedScenario.TryGetValue(mission, out flag2);
    this.m_CachedDefeatedScenario[mission] = flag1;
    return flag1 != flag2;
  }

  public AdventureBossDef GetBossDef(ScenarioDbId mission)
  {
    AdventureBossDef bossDef = (AdventureBossDef) null;
    if (!this.m_CachedBossDef.TryGetValue(mission, out bossDef) && !string.IsNullOrEmpty(AdventureConfig.GetBossDefAssetPath(mission)))
      Debug.LogErrorFormat("Boss def for mission not loaded: {0}\nCall LoadBossDef first.", (object) mission);
    return bossDef;
  }

  public void LoadBossDef(ScenarioDbId mission, AdventureConfig.DelBossDefLoaded callback)
  {
    AdventureBossDef bossDef = (AdventureBossDef) null;
    if (this.m_CachedBossDef.TryGetValue(mission, out bossDef))
    {
      callback(bossDef, true);
    }
    else
    {
      string bossDefAssetPath = AdventureConfig.GetBossDefAssetPath(mission);
      if (string.IsNullOrEmpty(bossDefAssetPath))
      {
        if (callback == null)
          return;
        callback((AdventureBossDef) null, false);
      }
      else
        AssetLoader.Get().InstantiatePrefab((AssetReference) bossDefAssetPath, (PrefabCallback<GameObject>) ((path, go, data) =>
        {
          if ((UnityEngine.Object) go == (UnityEngine.Object) null)
          {
            Debug.LogError((object) string.Format("Unable to instantiate boss def: {0}", (object) path));
            AdventureConfig.DelBossDefLoaded delBossDefLoaded = callback;
            if (delBossDefLoaded == null)
              return;
            delBossDefLoaded((AdventureBossDef) null, false);
          }
          else
          {
            AdventureBossDef component = go.GetComponent<AdventureBossDef>();
            if ((UnityEngine.Object) component == (UnityEngine.Object) null)
              Debug.LogError((object) string.Format("Object does not contain AdventureBossDef component: {0}", (object) path));
            else
              this.m_CachedBossDef[mission] = component;
            AdventureConfig.DelBossDefLoaded delBossDefLoaded = callback;
            if (delBossDefLoaded == null)
              return;
            delBossDefLoaded(component, (UnityEngine.Object) component != (UnityEngine.Object) null);
          }
        }));
    }
  }

  public static string GetBossDefAssetPath(ScenarioDbId mission) => GameDbf.AdventureMission.GetRecord((Predicate<AdventureMissionDbfRecord>) (r => (ScenarioDbId) r.ScenarioId == mission))?.BossDefAssetPath;

  public void ClearBossDefs()
  {
    foreach (KeyValuePair<ScenarioDbId, AdventureBossDef> keyValuePair in this.m_CachedBossDef)
      UnityEngine.Object.Destroy((UnityEngine.Object) keyValuePair.Value);
    this.m_CachedBossDef.Clear();
  }

  public void SetMission(ScenarioDbId mission, bool showDetails = true)
  {
    this.m_SelectedMission = mission;
    Log.Adventures.Print("Selected Mission set to {0}", (object) mission);
    this.m_LastSelectedMissions[this.GetSelectedAdventureAndModeString()] = mission;
    foreach (AdventureConfig.AdventureMissionSet adventureMissionSet in this.m_AdventureMissionSetEventList.ToArray())
      adventureMissionSet(mission, showDetails);
  }

  public void SetMissionOverride(ScenarioDbId missionOverride) => this.m_MissionOverride = missionOverride;

  public ScenarioDbId GetMissionOverride() => this.m_MissionOverride;

  public bool DoesSelectedMissionRequireDeck() => AdventureConfig.DoesMissionRequireDeck(this.m_SelectedMission);

  public static bool DoesMissionRequireDeck(ScenarioDbId scenario)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) scenario);
    return record == null || record.Player1DeckId == 0;
  }

  public void AddAdventureMissionSetListener(AdventureConfig.AdventureMissionSet dlg) => this.m_AdventureMissionSetEventList.Add(dlg);

  public void RemoveAdventureMissionSetListener(AdventureConfig.AdventureMissionSet dlg) => this.m_AdventureMissionSetEventList.Remove(dlg);

  public void AddAdventureModeChangeListener(AdventureConfig.AdventureModeChange dlg) => this.m_AdventureModeChangeEventList.Add(dlg);

  public void RemoveAdventureModeChangeListener(AdventureConfig.AdventureModeChange dlg) => this.m_AdventureModeChangeEventList.Remove(dlg);

  public void AddSubSceneChangeListener(AdventureConfig.SubSceneChange dlg) => this.m_SubSceneChangeEventList.Add(dlg);

  public void RemoveSubSceneChangeListener(AdventureConfig.SubSceneChange dlg) => this.m_SubSceneChangeEventList.Remove(dlg);

  public void AddSelectedModeChangeListener(AdventureConfig.SelectedModeChange dlg) => this.m_SelectedModeChangeEventList.Add(dlg);

  public void RemoveSelectedModeChangeListener(AdventureConfig.SelectedModeChange dlg) => this.m_SelectedModeChangeEventList.Remove(dlg);

  public void ResetSubScene(AdventureData.Adventuresubscene subscene)
  {
    this.m_CurrentSubScene = subscene;
    this.m_PreviousSubScene = AdventureData.Adventuresubscene.INVALID;
    this.m_SubSceneBackStack.Clear();
  }

  public void ChangeSubScene(AdventureData.Adventuresubscene subscene, bool pushToBackStack = true)
  {
    if (subscene == this.m_CurrentSubScene)
    {
      Debug.Log((object) string.Format("Sub scene {0} is already set.", (object) subscene));
    }
    else
    {
      if (pushToBackStack)
        this.m_SubSceneBackStack.Push(this.m_CurrentSubScene);
      this.m_PreviousSubScene = this.m_CurrentSubScene;
      this.m_CurrentSubScene = subscene;
      this.FireSubSceneChangeEvent(true);
      this.FireAdventureModeChangeEvent();
    }
  }

  public void SubSceneGoBack(bool fireevent = true)
  {
    if (this.m_SubSceneBackStack.Count == 0)
    {
      Debug.Log((object) "No sub scenes exist in the back stack.");
    }
    else
    {
      this.m_PreviousSubScene = this.m_CurrentSubScene;
      this.m_CurrentSubScene = this.m_SubSceneBackStack.Pop();
      if (fireevent)
        this.FireSubSceneChangeEvent(false);
      this.FireAdventureModeChangeEvent();
    }
  }

  public void RemoveSubScenesFromStackUntilTargetReached(
    AdventureData.Adventuresubscene targetSubscene)
  {
    while (this.m_SubSceneBackStack.Count > 0 && this.m_SubSceneBackStack.Peek() != targetSubscene)
    {
      int num = (int) this.m_SubSceneBackStack.Pop();
    }
  }

  public void RemoveSubSceneIfOnTopOfStack(AdventureData.Adventuresubscene subscene)
  {
    if (this.m_SubSceneBackStack.Peek() != subscene)
      return;
    int num = (int) this.m_SubSceneBackStack.Pop();
  }

  public void ChangeSubSceneToSelectedAdventure() => this.RequestGameSaveDataKeysForSelectedAdventure((GameSaveDataManager.OnRequestDataResponseDelegate) (success =>
  {
    if (success)
    {
      if (GameUtils.DoesAdventureModeUseDungeonCrawlFormat(this.GetSelectedMode()))
      {
        AdventureDataDbfRecord adventureDataRecord = this.GetSelectedAdventureDataRecord();
        if (adventureDataRecord != null)
          DungeonCrawlUtil.MigrateDungeonCrawlSubkeys((GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey, (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey);
      }
    }
    else
      Debug.LogError((object) "ChangeSubSceneToSelectedAdventure - Request for Adventure Game Save Keys failed.");
    AdventureData.Adventuresubscene subSceneFromMode = AdventureConfig.GetSubSceneFromMode(this.SelectedAdventure, this.SelectedMode);
    this.UpdateShouldSeeFirstTimeFlowForSelectedMode();
    if (this.ShouldSeeFirstTimeFlow && this.AllChaptersOwned && !AdventureUtils.IsEntireAdventureFree(this.SelectedAdventure))
      this.MarkHasSeenFirstTimeFlowComplete();
    this.ChangeSubScene(subSceneFromMode);
  }));

  public void RequestGameSaveDataKeysForSelectedAdventure(
    GameSaveDataManager.OnRequestDataResponseDelegate onCompleteCallback)
  {
    AdventureDataDbfRecord adventureDataRecord1 = this.GetSelectedAdventureDataRecord();
    List<GameSaveKeyId> keys = new List<GameSaveKeyId>();
    if (adventureDataRecord1 != null && adventureDataRecord1.GameSaveDataClientKey != 0)
      keys.Add((GameSaveKeyId) adventureDataRecord1.GameSaveDataClientKey);
    if (adventureDataRecord1 != null && adventureDataRecord1.GameSaveDataServerKey != 0)
      keys.Add((GameSaveKeyId) adventureDataRecord1.GameSaveDataServerKey);
    if (GameUtils.IsModeHeroic(this.GetSelectedMode()))
    {
      AdventureDataDbfRecord adventureDataRecord2 = AdventureConfig.GetAdventureDataRecord(this.GetSelectedAdventure(), GameUtils.GetNormalModeFromHeroicMode(this.GetSelectedMode()));
      if (adventureDataRecord2 != null && adventureDataRecord2.GameSaveDataClientKey != 0)
        keys.Add((GameSaveKeyId) adventureDataRecord2.GameSaveDataClientKey);
    }
    if (keys.Count > 0)
      GameSaveDataManager.Get().Request(keys, onCompleteCallback);
    else
      onCompleteCallback(true);
  }

  public static bool IsMissionAvailable(int missionId)
  {
    bool flag = AdventureProgressMgr.Get().CanPlayScenario(missionId);
    if (!flag)
      return false;
    int missionReqProgress = 0;
    int wingId = 0;
    if (!AdventureConfig.GetMissionPlayableParameters(missionId, ref wingId, ref missionReqProgress))
      return false;
    int ack = 0;
    AdventureProgressMgr.Get().GetWingAck(wingId, out ack);
    return flag && missionReqProgress <= ack;
  }

  public static bool IsMissionNewlyAvailableAndGetReqs(
    int missionId,
    ref int wingId,
    ref int missionReqProgress)
  {
    if (!AdventureConfig.GetMissionPlayableParameters(missionId, ref wingId, ref missionReqProgress))
      return false;
    bool flag = AdventureProgressMgr.Get().CanPlayScenario(missionId);
    int ack = 0;
    AdventureProgressMgr.Get().GetWingAck(wingId, out ack);
    return ack < missionReqProgress & flag;
  }

  public static bool AckCurrentWingProgress(int wingId) => AdventureConfig.SetWingAckIfGreater(wingId, AdventureProgressMgr.Get().GetProgressValueForWing(wingId));

  public static bool SetWingAckIfGreater(int wingId, int ackProgress)
  {
    int ack = 0;
    AdventureProgressMgr.Get().GetWingAck(wingId, out ack);
    if (ackProgress <= ack)
      return false;
    AdventureProgressMgr.Get().SetWingAck(wingId, ackProgress);
    return true;
  }

  public static bool ShouldDisplayAdventure(AdventureDbId adventureId) => (!GameUtils.IsAdventureRotated(adventureId) || AdventureProgressMgr.Get().OwnsOneOrMoreAdventureWings(adventureId)) && (adventureId == AdventureDbId.PRACTICE || AchieveManager.Get().HasUnlockedFeature(Achieve.Unlocks.VANILLA_HEROES) || AdventureProgressMgr.Get().OwnsOneOrMoreAdventureWings(adventureId) || !AdventureUtils.DoesAdventureRequireAllHeroesUnlocked(adventureId)) && (AdventureConfig.IsAdventureComingSoon(adventureId) || AdventureConfig.IsAdventureEventActive(adventureId));

  public static bool IsAdventureEventActive(AdventureDbId advId)
  {
    bool flag = true;
    foreach (WingDbfRecord record in GameDbf.Wing.GetRecords())
    {
      if ((AdventureDbId) record.AdventureId == advId)
      {
        if (AdventureProgressMgr.IsWingEventActive(record.ID))
          return true;
        flag = false;
      }
    }
    return flag;
  }

  public static SpecialEventType GetEarliestWingEventTiming(AdventureDbId advId)
  {
    SpecialEventType eventType = SpecialEventType.SPECIAL_EVENT_NEVER;
    foreach (WingDbfRecord record in GameDbf.Wing.GetRecords())
    {
      if ((AdventureDbId) record.AdventureId == advId)
      {
        SpecialEventType wingEventTiming = AdventureProgressMgr.GetWingEventTiming(record.ID);
        if (eventType != SpecialEventType.SPECIAL_EVENT_NEVER)
        {
          DateTime? eventStartTimeUtc1 = SpecialEventManager.Get().GetEventStartTimeUtc(wingEventTiming);
          DateTime? eventStartTimeUtc2 = SpecialEventManager.Get().GetEventStartTimeUtc(eventType);
          if ((eventStartTimeUtc1.HasValue & eventStartTimeUtc2.HasValue ? (eventStartTimeUtc1.GetValueOrDefault() < eventStartTimeUtc2.GetValueOrDefault() ? 1 : 0) : 0) == 0)
            continue;
        }
        eventType = wingEventTiming;
      }
    }
    return eventType;
  }

  public static bool IsAdventureComingSoon(AdventureDbId advId)
  {
    AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) advId);
    if (record != null)
      return SpecialEventManager.Get().IsEventActive(record.ComingSoonEvent, false);
    Debug.LogErrorFormat("IsAdventureComingSoon - Adventure Id is invalid: {0}", (object) (int) advId);
    return false;
  }

  public static AdventureDbId GetAdventurePlayerShouldSee(
    out int latestActiveAdventureWing)
  {
    latestActiveAdventureWing = 0;
    if (!Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false))
      return AdventureDbId.INVALID;
    AdventureDbfRecord highestSortOrder = AdventureConfig.GetActiveExpansionAdventureWithHighestSortOrder();
    if (highestSortOrder == null)
      return AdventureDbId.INVALID;
    long finalAdventureWing = (long) AdventureUtils.GetFinalAdventureWing(highestSortOrder.ID, false, true);
    latestActiveAdventureWing = (int) finalAdventureWing;
    long num = 0;
    if (!GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LATEST_ADVENTURE_WING_SEEN, out num))
      num = 2522L;
    return finalAdventureWing != num ? (AdventureDbId) highestSortOrder.ID : AdventureDbId.INVALID;
  }

  public static AdventureDbId GetAdventurePlayerShouldSee()
  {
    int latestActiveAdventureWing = 0;
    return AdventureConfig.GetAdventurePlayerShouldSee(out latestActiveAdventureWing);
  }

  public static AdventureDbfRecord GetActiveExpansionAdventureWithHighestSortOrder()
  {
    List<AdventureDbfRecord> recordsWithDefPrefab = GameUtils.GetAdventureRecordsWithDefPrefab();
    AdventureDbfRecord highestSortOrder = (AdventureDbfRecord) null;
    foreach (AdventureDbfRecord adventureDbfRecord in recordsWithDefPrefab)
    {
      if (GameUtils.IsExpansionAdventure((AdventureDbId) adventureDbfRecord.ID) && AdventureConfig.ShouldDisplayAdventure((AdventureDbId) adventureDbfRecord.ID) && !AdventureConfig.IsAdventureComingSoon((AdventureDbId) adventureDbfRecord.ID) && (highestSortOrder == null || adventureDbfRecord.SortOrder > highestSortOrder.SortOrder))
        highestSortOrder = adventureDbfRecord;
    }
    return highestSortOrder;
  }

  public static bool GetMissionPlayableParameters(
    int missionId,
    ref int wingId,
    ref int missionReqProgress)
  {
    ScenarioDbfRecord scenarioRecord = GameDbf.Scenario.GetRecord(missionId);
    if (scenarioRecord == null)
      return false;
    AdventureMissionDbfRecord record1 = GameDbf.AdventureMission.GetRecord((Predicate<AdventureMissionDbfRecord>) (r => r.ScenarioId == scenarioRecord.ID));
    if (record1 == null)
      return false;
    WingDbfRecord record2 = GameDbf.Wing.GetRecord(record1.ReqWingId);
    if (record2 == null)
      return false;
    missionReqProgress = record1.ReqProgress;
    wingId = record2.ID;
    return true;
  }

  public int GetWingBossesDefeated(
    AdventureDbId advId,
    AdventureModeDbId mode,
    WingDbId wing,
    int defaultvalue = 0)
  {
    int num = 0;
    return this.m_WingBossesDefeatedCache.TryGetValue(this.GetWingUniqueId(advId, mode, wing), out num) ? num : defaultvalue;
  }

  public void UpdateWingBossesDefeated(
    AdventureDbId advId,
    AdventureModeDbId mode,
    WingDbId wing,
    int bossesDefeated)
  {
    this.m_WingBossesDefeatedCache[this.GetWingUniqueId(advId, mode, wing)] = bossesDefeated;
  }

  private string GetWingUniqueId(AdventureDbId advId, AdventureModeDbId modeId, WingDbId wing) => string.Format("{0}_{1}_{2}", (object) advId, (object) modeId, (object) wing);

  private void Awake()
  {
    AdventureConfig.s_instance = this;
    this.gameObject.AddComponent<HSDontDestroyOnLoad>();
  }

  private void Start()
  {
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    this.AddSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubSceneChange));
  }

  private void OnDestroy()
  {
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    AdventureConfig.s_instance = (AdventureConfig) null;
  }

  public void OnAdventureSceneAwake()
  {
    this.SelectedAdventure = Options.Get().GetEnum<AdventureDbId>(Option.SELECTED_ADVENTURE, AdventureDbId.PRACTICE);
    this.SelectedMode = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.LINEAR);
    if (!AdventureConfig.ShouldDisplayAdventure(this.SelectedAdventure))
    {
      this.SelectedAdventure = AdventureDbId.PRACTICE;
      this.SelectedMode = AdventureModeDbId.LINEAR;
    }
    this.SetPropertiesForAdventureAndMode();
  }

  public void OnAdventureSceneUnload()
  {
    if (this.OnAdventureSceneUnloadEvent != null)
      this.OnAdventureSceneUnloadEvent();
    this.SelectedAdventure = AdventureDbId.INVALID;
    this.SelectedMode = AdventureModeDbId.INVALID;
  }

  public void ResetSubScene() => this.ResetSubScene(AdventureData.Adventuresubscene.CHOOSER);

  private void FireAdventureModeChangeEvent()
  {
    foreach (AdventureConfig.AdventureModeChange adventureModeChange in this.m_AdventureModeChangeEventList.ToArray())
      adventureModeChange(this.SelectedAdventure, this.SelectedMode);
  }

  private void FireSubSceneChangeEvent(bool forward)
  {
    this.UpdatePresence();
    foreach (AdventureConfig.SubSceneChange subSceneChange in this.m_SubSceneChangeEventList.ToArray())
      subSceneChange(this.m_CurrentSubScene, forward);
  }

  private void FireSelectedModeChangeEvent()
  {
    foreach (AdventureConfig.SelectedModeChange selectedModeChange in this.m_SelectedModeChangeEventList.ToArray())
      selectedModeChange(this.SelectedAdventure, this.SelectedMode);
  }

  public void UpdatePresence()
  {
    switch (this.m_CurrentSubScene)
    {
      case AdventureData.Adventuresubscene.MISSION_DECK_PICKER:
      case AdventureData.Adventuresubscene.MISSION_DISPLAY:
      case AdventureData.Adventuresubscene.CLASS_CHALLENGE:
      case AdventureData.Adventuresubscene.DUNGEON_CRAWL:
      case AdventureData.Adventuresubscene.ADVENTURER_PICKER:
      case AdventureData.Adventuresubscene.LOCATION_SELECT:
        PresenceMgr.Get().SetStatus_EnteringAdventure(this.SelectedAdventure, this.SelectedMode);
        break;
      default:
        if (!((UnityEngine.Object) AdventureScene.Get() != (UnityEngine.Object) null) || AdventureScene.Get().IsUnloading())
          break;
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.ADVENTURE_CHOOSING_MODE);
        break;
    }
  }

  public bool IsHeroSelectedBeforeDungeonCrawlScreenForSelectedAdventure()
  {
    AdventureDataDbfRecord adventureDataRecord = this.GetSelectedAdventureDataRecord();
    return adventureDataRecord != null && adventureDataRecord.DungeonCrawlPickHeroFirst;
  }

  public bool IsChapterSelectedBeforeDungeonCrawlScreenForSelectedAdventure()
  {
    AdventureDataDbfRecord adventureDataRecord = this.GetSelectedAdventureDataRecord();
    return adventureDataRecord != null && adventureDataRecord.DungeonCrawlSelectChapter;
  }

  private bool ValidLoadoutIsLockedInForSelectedAdventure()
  {
    AdventureDataDbfRecord adventureDataRecord = this.GetSelectedAdventureDataRecord();
    GameSaveKeyId saveDataServerKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey;
    if (!GameSaveDataManager.Get().ValidateIfKeyCanBeAccessed(saveDataServerKey, (string) adventureDataRecord.Name))
      return false;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_SCENARIO_ID, out num);
    long heroPowerDbId;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_POWER, out heroPowerDbId);
    long deckDbId;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_DECK, out deckDbId);
    long treasureDbId;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_LOADOUT_TREASURE_ID, out treasureDbId);
    if (adventureDataRecord.DungeonCrawlSaveHeroUsingHeroDbId)
    {
      long heroCardDbId;
      GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CARD_DB_ID, out heroCardDbId);
      return AdventureUtils.IsValidLoadoutForSelectedAdventureAndHero(this.SelectedAdventure, this.SelectedMode, (ScenarioDbId) num, (int) heroCardDbId, (int) heroPowerDbId, (int) treasureDbId);
    }
    long heroClass;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CLASS, out heroClass);
    return AdventureUtils.IsValidLoadoutForSelectedAdventureAndClass(this.SelectedAdventure, this.SelectedMode, (ScenarioDbId) num, (TAG_CLASS) heroClass, (int) heroPowerDbId, (int) deckDbId, (int) treasureDbId);
  }

  public bool GuestHeroesExistForCurrentAdventure() => GameDbf.AdventureGuestHeroes.HasRecord((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == this.GetSelectedAdventure()));

  public List<GuestHero> GetGuestHeroesForCurrentAdventure() => AdventureUtils.GetGuestHeroesForAdventure(this.GetSelectedAdventure());

  public static List<int> GetGuestHeroesForWing(int wingId)
  {
    List<AdventureGuestHeroesDbfRecord> records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => r.WingId == wingId));
    records.Sort((Comparison<AdventureGuestHeroesDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    List<int> guestHeroesForWing = new List<int>();
    foreach (AdventureGuestHeroesDbfRecord guestHeroesDbfRecord in records)
      guestHeroesForWing.Add(GameUtils.GetCardIdFromGuestHeroDbId(guestHeroesDbfRecord.GuestHeroId));
    return guestHeroesForWing;
  }

  public static int GetAdventureBossesInRun(WingDbfRecord wingRecord)
  {
    if (wingRecord != null)
      return wingRecord.DungeonCrawlBosses;
    Debug.LogError((object) "GetAdventureBossesInRun - no WingDbfRecord passed in!");
    return 0;
  }

  public AdventureData.Adventuresubscene SubSceneForPickingHeroForCurrentAdventure() => !this.GuestHeroesExistForCurrentAdventure() ? AdventureData.Adventuresubscene.MISSION_DECK_PICKER : AdventureData.Adventuresubscene.ADVENTURER_PICKER;

  public AdventureData.Adventuresubscene GetCorrectSubSceneWhenLoadingDungeonCrawlMode()
  {
    bool flag = DungeonCrawlUtil.IsDungeonRunInProgress(this.SelectedAdventure, this.SelectedMode) || this.ValidLoadoutIsLockedInForSelectedAdventure();
    if (!flag && this.IsChapterSelectedBeforeDungeonCrawlScreenForSelectedAdventure())
      return AdventureData.Adventuresubscene.LOCATION_SELECT;
    return !flag && this.IsHeroSelectedBeforeDungeonCrawlScreenForSelectedAdventure() ? this.SubSceneForPickingHeroForCurrentAdventure() : AdventureData.Adventuresubscene.DUNGEON_CRAWL;
  }

  private void OnSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod purchaseMethod) => this.EvaluateIfAllWingsOwnedForSelectedAdventure();

  private void OnSubSceneChange(AdventureData.Adventuresubscene subScene, bool forward)
  {
    if (((GameUtils.DoesAdventureModeUseDungeonCrawlFormat(this.GetSelectedMode()) ? 1 : 0) & (subScene == AdventureData.Adventuresubscene.MISSION_DECK_PICKER ? (true ? 1 : 0) : (subScene == AdventureData.Adventuresubscene.ADVENTURER_PICKER ? 1 : 0))) == 0)
      return;
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(this.GetMission());
    DungeonCrawlSubDef_VOLines.PlayVOLine(this.GetSelectedAdventure(), wingIdFromMissionId, 0, DungeonCrawlSubDef_VOLines.VOEventType.CHARACTER_SELECT);
  }

  private void SetPropertiesForAdventureAndMode()
  {
    this.EvaluateIfAllWingsOwnedForSelectedAdventure();
    this.UpdateCompletionRewards();
  }

  private void EvaluateIfAllWingsOwnedForSelectedAdventure()
  {
    if (this.SelectedAdventure == AdventureDbId.INVALID || this.SelectedMode == AdventureModeDbId.INVALID)
      return;
    this.AllChaptersOwned = AdventureProgressMgr.Get().OwnsAllAdventureWings(this.SelectedAdventure);
  }

  private void UpdateCompletionRewards()
  {
    List<RewardData> forAdventureByMode = AdventureProgressMgr.GetRewardsForAdventureByMode((int) this.SelectedAdventure, (int) this.SelectedMode, new HashSet<Achieve.RewardTiming>()
    {
      Achieve.RewardTiming.ADVENTURE_CHEST
    });
    this.Legacy_UpdateCompletionRewardData(forAdventureByMode);
    this.CompletionRewards.Items.Clear();
    foreach (RewardData rewardData in forAdventureByMode)
    {
      RewardItemDataModel rewardItemDataModel = RewardUtils.RewardDataToRewardItemDataModel(rewardData);
      if (rewardItemDataModel != null)
        this.CompletionRewards.Items.Add(rewardItemDataModel);
    }
  }

  private void Legacy_UpdateCompletionRewardData(List<RewardData> adventureCompletionRewards)
  {
    bool flag = false;
    foreach (RewardData completionReward in adventureCompletionRewards)
    {
      if (completionReward is CardBackRewardData)
      {
        flag = true;
        CardBackRewardData cardBackRewardData = completionReward as CardBackRewardData;
        this.CompletionRewardType = Reward.Type.CARD_BACK;
        this.CompletionRewardId = cardBackRewardData.CardBackID;
      }
    }
    if (adventureCompletionRewards.Count >= 1 && flag)
      return;
    this.CompletionRewardType = Reward.Type.NONE;
    this.CompletionRewardId = 0;
  }

  public void ResetLoadout()
  {
    this.AnomalyModeActivated = false;
    this.SelectedHeroCardDbId = 0L;
    this.SelectedLoadoutTreasureDbId = 0L;
    this.SelectedHeroPowerDbId = 0L;
    this.SelectedDeckId = 0L;
    this.SetMissionOverride(ScenarioDbId.INVALID);
  }

  public void SetHasSeenUnlockedChapterPage(WingDbId wingId, bool hasSeen)
  {
    if (hasSeen)
    {
      this.NeedsChapterNewlyUnlockedHighlight.Remove((long) wingId);
    }
    else
    {
      if (!this.GetHasSeenUnlockedChapterPage(wingId))
        return;
      this.NeedsChapterNewlyUnlockedHighlight.Add((long) wingId);
    }
  }

  public bool GetHasSeenUnlockedChapterPage(WingDbId wingId) => !this.NeedsChapterNewlyUnlockedHighlight.Contains((long) wingId);

  public bool HasUnacknowledgedChapterUnlocks()
  {
    foreach (WingDbfRecord record in GameDbf.Wing.GetRecords((Predicate<WingDbfRecord>) (r => (AdventureDbId) r.AdventureId == this.SelectedAdventure)))
    {
      int num = (int) AdventureProgressMgr.Get().AdventureBookChapterStateForWing(record, this.SelectedMode);
      int ack;
      AdventureProgressMgr.Get().GetWingAck(record.ID, out ack);
      if (num == 1 && ack == 0)
        return true;
    }
    return false;
  }

  public bool HasValidLoadoutForSelectedAdventure() => this.GetSelectedAdventureDataRecord().DungeonCrawlSaveHeroUsingHeroDbId ? AdventureUtils.IsValidLoadoutForSelectedAdventureAndHero(this.SelectedAdventure, this.SelectedMode, this.m_SelectedMission, (int) this.SelectedHeroCardDbId, (int) this.SelectedHeroPowerDbId, (int) this.SelectedLoadoutTreasureDbId) : AdventureUtils.IsValidLoadoutForSelectedAdventureAndClass(this.SelectedAdventure, this.SelectedMode, this.m_SelectedMission, AdventureUtils.GetHeroClassFromHeroId((int) this.SelectedHeroCardDbId), (int) this.SelectedHeroPowerDbId, (int) this.SelectedDeckId, (int) this.SelectedLoadoutTreasureDbId);

  public delegate void DelBossDefLoaded(AdventureBossDef bossDef, bool success);

  public delegate void AdventureModeChange(AdventureDbId adventureId, AdventureModeDbId modeId);

  public delegate void AdventureMissionSet(ScenarioDbId mission, bool showDetails);

  public delegate void SubSceneChange(AdventureData.Adventuresubscene newscene, bool forward);

  public delegate void SelectedModeChange(AdventureDbId adventureId, AdventureModeDbId modeId);

  public delegate void AnomalyModeChangedHandler(bool anomalyModeActived);
}
