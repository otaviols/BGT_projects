using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdventureProgressMgr : IService
{
  private Map<int, AdventureMission.WingProgress> m_wingProgress = new Map<int, AdventureMission.WingProgress>();
  private Map<int, int> m_wingAckState = new Map<int, int>();
  private Map<int, AdventureMission> m_missions = new Map<int, AdventureMission>();
  private List<AdventureProgressMgr.AdventureProgressUpdatedListener> m_progressUpdatedListeners = new List<AdventureProgressMgr.AdventureProgressUpdatedListener>();

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AdventureProgressMgr adventureProgressMgr = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new Action(adventureProgressMgr.WillReset);
    adventureProgressMgr.LoadAdventureMissionsFromDBF();
    serviceLocator.Get<Network>().RegisterNetHandler((object) AdventureProgressResponse.PacketID.ID, new Network.NetHandler(adventureProgressMgr.OnAdventureProgress));
    serviceLocator.Get<NetCache>().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(adventureProgressMgr.OnNewNotices));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[3]
  {
    typeof (Network),
    typeof (NetCache),
    typeof (GameDbf)
  };

  public void Shutdown()
  {
  }

  private void WillReset()
  {
    this.m_wingProgress.Clear();
    this.m_wingAckState.Clear();
    this.m_progressUpdatedListeners.Clear();
    this.IsReady = false;
  }

  public static AdventureProgressMgr Get() => ServiceManager.Get<AdventureProgressMgr>();

  public static void InitRequests() => Network.Get().RequestAdventureProgress();

  public bool IsReady { get; private set; }

  public bool RegisterProgressUpdatedListener(
    AdventureProgressMgr.AdventureProgressUpdatedCallback callback)
  {
    return this.RegisterProgressUpdatedListener(callback, (object) null);
  }

  public bool RegisterProgressUpdatedListener(
    AdventureProgressMgr.AdventureProgressUpdatedCallback callback,
    object userData)
  {
    if (callback == null)
      return false;
    AdventureProgressMgr.AdventureProgressUpdatedListener progressUpdatedListener = new AdventureProgressMgr.AdventureProgressUpdatedListener();
    progressUpdatedListener.SetCallback(callback);
    progressUpdatedListener.SetUserData(userData);
    if (this.m_progressUpdatedListeners.Contains(progressUpdatedListener))
      return false;
    this.m_progressUpdatedListeners.Add(progressUpdatedListener);
    return true;
  }

  public bool RemoveProgressUpdatedListener(
    AdventureProgressMgr.AdventureProgressUpdatedCallback callback)
  {
    return this.RemoveProgressUpdatedListener(callback, (object) null);
  }

  public bool RemoveProgressUpdatedListener(
    AdventureProgressMgr.AdventureProgressUpdatedCallback callback,
    object userData)
  {
    if (callback == null)
      return false;
    AdventureProgressMgr.AdventureProgressUpdatedListener progressUpdatedListener = new AdventureProgressMgr.AdventureProgressUpdatedListener();
    progressUpdatedListener.SetCallback(callback);
    progressUpdatedListener.SetUserData(userData);
    if (!this.m_progressUpdatedListeners.Contains(progressUpdatedListener))
      return false;
    this.m_progressUpdatedListeners.Remove(progressUpdatedListener);
    return true;
  }

  public List<AdventureMission.WingProgress> GetAllProgress() => new List<AdventureMission.WingProgress>((IEnumerable<AdventureMission.WingProgress>) this.m_wingProgress.Values);

  public AdventureMission.WingProgress GetProgress(int wing)
  {
    AdventureMission.WingProgress wingProgress;
    return !this.m_wingProgress.TryGetValue(wing, out wingProgress) ? (AdventureMission.WingProgress) null : wingProgress;
  }

  public int GetProgressValueForWing(int wing)
  {
    AdventureMission.WingProgress progress = this.GetProgress(wing);
    return progress == null ? 0 : progress.Progress;
  }

  public bool OwnsOneOrMoreAdventureWings(AdventureDbId adventureID)
  {
    foreach (WingDbfRecord record in GameDbf.Wing.GetRecords())
    {
      if ((AdventureDbId) record.AdventureId == adventureID && this.OwnsWing(record.ID))
        return true;
    }
    return false;
  }

  public bool OwnsAllAdventureWings(AdventureDbId adventureID)
  {
    foreach (WingDbfRecord record in GameDbf.Wing.GetRecords())
    {
      if ((AdventureDbId) record.AdventureId == adventureID && !this.OwnsWing(record.ID))
        return false;
    }
    return true;
  }

  public bool OwnsWing(int wing) => this.m_wingProgress.ContainsKey(wing) && this.m_wingProgress[wing].IsOwned();

  public WingDbfRecord GetFirstUnownedAdventureWing(AdventureDbId adventureID)
  {
    WingDbfRecord unownedAdventureWing = (WingDbfRecord) null;
    foreach (WingDbfRecord record in GameDbf.Wing.GetRecords((Predicate<WingDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureID)))
    {
      if (!this.OwnsWing(record.ID) && (unownedAdventureWing == null || record.UnlockOrder < unownedAdventureWing.UnlockOrder))
        unownedAdventureWing = record;
    }
    return unownedAdventureWing;
  }

  public bool IsWingComplete(AdventureDbId adventureID, AdventureModeDbId modeID, WingDbId wingId) => this.IsWingComplete(adventureID, modeID, wingId, out bool _);

  public bool IsWingComplete(
    AdventureDbId adventureID,
    AdventureModeDbId modeID,
    WingDbId wingId,
    out bool wingHasUnackedProgress)
  {
    List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords();
    wingHasUnackedProgress = false;
    foreach (ScenarioDbfRecord scenarioDbfRecord in records)
    {
      if ((AdventureDbId) scenarioDbfRecord.AdventureId == adventureID && (AdventureModeDbId) scenarioDbfRecord.ModeId == modeID && (WingDbId) scenarioDbfRecord.WingId == wingId)
      {
        bool hasUnackedProgress = false;
        if (!this.HasDefeatedScenario(scenarioDbfRecord.ID, out hasUnackedProgress))
          return false;
        if (hasUnackedProgress)
          wingHasUnackedProgress = true;
      }
    }
    return true;
  }

  public bool IsAdventureModeAndSectionComplete(
    AdventureDbId adventureID,
    AdventureModeDbId modeID,
    int bookSection = 0)
  {
    foreach (ScenarioDbfRecord record1 in GameDbf.Scenario.GetRecords((Predicate<ScenarioDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureID && (AdventureModeDbId) r.ModeId == modeID)))
    {
      int wingId = record1.WingId;
      if (wingId > 0)
      {
        WingDbfRecord record2 = GameDbf.Wing.GetRecord(wingId);
        if (record2 != null && bookSection == record2.BookSection && !this.HasDefeatedScenario(record1.ID))
          return false;
      }
    }
    return true;
  }

  public bool IsAdventureComplete(AdventureDbId adventureID)
  {
    List<AdventureDataDbfRecord> records = GameDbf.AdventureData.GetRecords((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureID));
    if (records.Count == 0)
    {
      Debug.LogWarningFormat("No Adventure mode records found for AdventureDbId {0}! Returning True for IsAdventureComplete()", (object) adventureID);
      return true;
    }
    foreach (AdventureDataDbfRecord adventureDataDbfRecord in records)
    {
      if (!this.IsAdventureModeAndSectionComplete(adventureID, (AdventureModeDbId) adventureDataDbfRecord.ModeId))
        return false;
    }
    return true;
  }

  public bool IsWingLocked(AdventureWingDef wingDef)
  {
    if (wingDef.GetWingId() == WingDbId.LOE_HALL_OF_EXPLORERS)
    {
      int num1 = this.IsWingComplete(AdventureDbId.LOE, AdventureModeDbId.LINEAR, WingDbId.LOE_TEMPLE_OF_ORSIS) ? 1 : 0;
      bool flag1 = this.IsWingComplete(AdventureDbId.LOE, AdventureModeDbId.LINEAR, WingDbId.LOE_ULDAMAN);
      bool flag2 = this.IsWingComplete(AdventureDbId.LOE, AdventureModeDbId.LINEAR, WingDbId.LOE_RUINED_CITY);
      int num2 = flag1 ? 1 : 0;
      return (num1 & num2 & (flag2 ? 1 : 0)) == 0;
    }
    if (wingDef.GetOpenPrereqId() != WingDbId.INVALID)
    {
      int ack;
      this.GetWingAck((int) wingDef.GetOpenPrereqId(), out ack);
      if (ack < 1 || wingDef.GetMustCompleteOpenPrereq() && !this.IsWingComplete(wingDef.GetAdventureId(), AdventureConfig.Get().GetSelectedMode(), wingDef.GetOpenPrereqId()))
        return true;
    }
    return false;
  }

  public int GetNumPlayableAdventureScenarios(AdventureDbId adventureID, AdventureModeDbId modeID)
  {
    List<WingDbfRecord> records = GameDbf.Wing.GetRecords((Predicate<WingDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureID));
    int adventureScenarios = 0;
    foreach (WingDbfRecord wing in records)
      adventureScenarios += this.GetNumPlayableScenariosForWing(wing, modeID);
    return adventureScenarios;
  }

  private int GetNumPlayableScenariosForWing(WingDbfRecord wing, AdventureModeDbId modeID)
  {
    int scenariosForWing = 0;
    if (!this.OwnsWing(wing.ID) || !AdventureProgressMgr.IsWingEventActive(wing.ID))
      return 0;
    foreach (ScenarioDbfRecord record in GameDbf.Scenario.GetRecords((Predicate<ScenarioDbfRecord>) (r => r.WingId == wing.ID && (AdventureModeDbId) r.ModeId == modeID)))
    {
      if (!this.HasDefeatedScenario(record.ID) && this.CanPlayScenario(record.ID))
        ++scenariosForWing;
    }
    return scenariosForWing;
  }

  public int GetPlayableClassChallenges(AdventureDbId adventureID, AdventureModeDbId modeID)
  {
    int playableClassChallenges = 0;
    foreach (ScenarioDbfRecord record in GameDbf.Scenario.GetRecords())
    {
      if ((AdventureDbId) record.AdventureId == adventureID && (AdventureModeDbId) record.ModeId == modeID && this.CanPlayScenario(record.ID) && !this.HasDefeatedScenario(record.ID))
        ++playableClassChallenges;
    }
    return playableClassChallenges;
  }

  public static List<RewardData> GetRewardsForWing(
    int wing,
    HashSet<Assets.Achieve.RewardTiming> rewardTimings)
  {
    List<RewardData> forAdventureWing = AchieveManager.Get().GetRewardsForAdventureWing(wing, rewardTimings);
    List<RewardData> rewardsForWing = new List<RewardData>();
    foreach (RewardData rewardData in forAdventureWing)
    {
      if (Reward.Type.CARD == rewardData.RewardType)
        rewardsForWing.Add((RewardData) (rewardData as CardRewardData));
      if (Reward.Type.CARD_BACK == rewardData.RewardType)
        rewardsForWing.Add((RewardData) (rewardData as CardBackRewardData));
      if (Reward.Type.BOOSTER_PACK == rewardData.RewardType)
        rewardsForWing.Add((RewardData) (rewardData as BoosterPackRewardData));
      if (Reward.Type.RANDOM_CARD == rewardData.RewardType)
        rewardsForWing.Add((RewardData) (rewardData as RandomCardRewardData));
    }
    return rewardsForWing;
  }

  public static List<RewardData> GetRewardsForAdventureByMode(
    int adventureId,
    int adventureModeId,
    HashSet<Assets.Achieve.RewardTiming> rewardTimings)
  {
    List<RewardData> adventureAndMode = AchieveManager.Get().GetRewardsForAdventureAndMode(adventureId, adventureModeId, rewardTimings);
    List<RewardData> forAdventureByMode = new List<RewardData>();
    foreach (RewardData rewardData in adventureAndMode)
    {
      if (Reward.Type.CARD == rewardData.RewardType)
        forAdventureByMode.Add((RewardData) (rewardData as CardRewardData));
      if (Reward.Type.CARD_BACK == rewardData.RewardType)
        forAdventureByMode.Add((RewardData) (rewardData as CardBackRewardData));
      if (Reward.Type.BOOSTER_PACK == rewardData.RewardType)
        forAdventureByMode.Add((RewardData) (rewardData as BoosterPackRewardData));
      if (Reward.Type.RANDOM_CARD == rewardData.RewardType)
        forAdventureByMode.Add((RewardData) (rewardData as RandomCardRewardData));
    }
    return forAdventureByMode;
  }

  public static SpecialEventType GetWingEventTiming(int wing)
  {
    WingDbfRecord record = GameDbf.Wing.GetRecord(wing);
    if (record == null)
    {
      Debug.LogWarning((object) string.Format("AdventureProgressMgr.GetWingEventTiming could not find DBF record for wing {0}, assuming it is has no open event", (object) wing));
      return SpecialEventType.IGNORE;
    }
    SpecialEventType requiredEvent = record.RequiredEvent;
    if (requiredEvent != SpecialEventType.UNKNOWN)
      return requiredEvent;
    Debug.LogWarning((object) string.Format("AdventureProgressMgr.GetWing wing={0} could not find SpecialEventType record for event", (object) wing));
    return SpecialEventType.IGNORE;
  }

  public static string GetWingName(int wing)
  {
    WingDbfRecord record = GameDbf.Wing.GetRecord(wing);
    if (record != null)
      return (string) record.Name;
    Debug.LogWarning((object) string.Format("AdventureProgressMgr.GetWingName could not find DBF record for wing {0}", (object) wing));
    return string.Empty;
  }

  public static bool IsWingEventActive(int wing)
  {
    SpecialEventType wingEventTiming = AdventureProgressMgr.GetWingEventTiming(wing);
    return SpecialEventManager.Get().IsEventActive(wingEventTiming, false);
  }

  public bool CanPlayScenario(int scenarioID, bool checkEventTiming = true)
  {
    if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2015 && 1061 != scenarioID)
      return false;
    if (!this.m_missions.ContainsKey(scenarioID))
      return true;
    AdventureMission mission = this.m_missions[scenarioID];
    if (!mission.HasRequiredProgress())
      return true;
    AdventureMission.WingProgress progress = this.GetProgress(mission.RequiredProgress.Wing);
    return progress != null && progress.MeetsProgressAndFlagsRequirements(mission.RequiredProgress) && (!checkEventTiming || AdventureProgressMgr.IsWingEventActive(mission.RequiredProgress.Wing));
  }

  public bool HasDefeatedScenario(int scenarioID) => this.HasDefeatedScenario(scenarioID, out bool _);

  public bool HasDefeatedScenario(int scenarioID, out bool hasUnackedProgress)
  {
    hasUnackedProgress = false;
    AdventureMission adventureMission;
    if (!this.m_missions.TryGetValue(scenarioID, out adventureMission) || adventureMission.RequiredProgress == null || adventureMission.GrantedProgress == null)
      return false;
    AdventureMission.WingProgress progress = this.GetProgress(adventureMission.GrantedProgress.Wing);
    if (progress == null)
      return false;
    int ack;
    this.GetWingAck(adventureMission.GrantedProgress.Wing, out ack);
    hasUnackedProgress = ack < adventureMission.GrantedProgress.Progress;
    return progress.MeetsProgressAndFlagsRequirements(adventureMission.GrantedProgress);
  }

  public static bool GetGameSaveDataProgressForScenario(
    int scenarioId,
    out int progress,
    out int maxProgress)
  {
    if (!AdventureProgressMgr.ScenarioUsesGameSaveDataProgress(scenarioId))
    {
      progress = 0;
      maxProgress = 0;
      Debug.LogError((object) string.Format("Attempting to get Game Save Data progress for Scenario={0}, which does not have any Game Save Data. Add a GSD Subkey to that scenario's dbi.", (object) scenarioId));
      return false;
    }
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(scenarioId);
    GameSaveKeyId saveDataServerKey = (GameSaveKeyId) AdventureConfig.Get().GetSelectedAdventureDataRecord().GameSaveDataServerKey;
    GameSaveKeySubkeyId dataProgressSubkey = (GameSaveKeySubkeyId) record.GameSaveDataProgressSubkey;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, dataProgressSubkey, out num);
    progress = (int) num;
    maxProgress = record.GameSaveDataProgressMax;
    return true;
  }

  public static bool ScenarioUsesGameSaveDataProgress(int scenarioId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(scenarioId);
    return record.GameSaveDataProgressSubkey != 0 && Enum.IsDefined(typeof (GameSaveKeySubkeyId), (object) record.GameSaveDataProgressSubkey);
  }

  public bool ScenarioHasRewardData(int scenarioId)
  {
    List<RewardData> defeatingScenario = this.GetImmediateRewardsForDefeatingScenario(scenarioId);
    return defeatingScenario != null && defeatingScenario.Count > 0;
  }

  public List<RewardData> GetImmediateRewardsForDefeatingScenario(int scenarioID)
  {
    HashSet<Assets.Achieve.RewardTiming> rewardTimings = new HashSet<Assets.Achieve.RewardTiming>()
    {
      Assets.Achieve.RewardTiming.IMMEDIATE
    };
    return this.GetRewardsForDefeatingScenario(scenarioID, rewardTimings);
  }

  public List<RewardData> GetRewardsForDefeatingScenario(
    int scenarioID,
    HashSet<Assets.Achieve.RewardTiming> rewardTimings)
  {
    AdventureMission adventureMission;
    if (!this.m_missions.TryGetValue(scenarioID, out adventureMission))
      return new List<RewardData>();
    List<RewardData> defeatingScenario = (List<RewardData>) null;
    if (GameUtils.IsHeroicAdventureMission(scenarioID) || GameUtils.IsClassChallengeMission(scenarioID) || adventureMission.GrantedProgress != null)
      defeatingScenario = AchieveManager.Get().GetRewardsForAdventureScenario(adventureMission.GrantedProgress.Wing, scenarioID, rewardTimings);
    return defeatingScenario;
  }

  public bool SetWingAck(int wing, int ackId)
  {
    Log.Adventures.Print("SetWingAck for wing {0}", (object) wing);
    int num;
    if (this.m_wingAckState.TryGetValue(wing, out num))
    {
      if (ackId < num)
        return false;
      if (ackId == num)
        return true;
    }
    this.m_wingAckState[wing] = ackId;
    Network.Get().AckWingProgress(wing, ackId);
    return true;
  }

  public bool GetWingAck(int wing, out int ack) => this.m_wingAckState.TryGetValue(wing, out ack);

  public AdventureMissionState AdventureMissionStateForScenario(int scenarioID)
  {
    if (this.HasDefeatedScenario(scenarioID))
      return AdventureMissionState.COMPLETED;
    return this.CanPlayScenario(scenarioID) ? AdventureMissionState.UNLOCKED : AdventureMissionState.LOCKED;
  }

  public AdventureChapterState AdventureBookChapterStateForWing(
    WingDbfRecord wingRecord,
    AdventureModeDbId adventureMode)
  {
    if (this.IsWingComplete((AdventureDbId) wingRecord.AdventureId, adventureMode, (WingDbId) wingRecord.ID))
      return AdventureChapterState.COMPLETED;
    return this.GetNumPlayableScenariosForWing(wingRecord, adventureMode) > 0 ? AdventureChapterState.UNLOCKED : AdventureChapterState.LOCKED;
  }

  public bool OwnershipPrereqWingIsOwned(AdventureWingDef wingDef) => wingDef.GetOwnershipPrereqId() == WingDbId.INVALID || this.OwnsWing((int) wingDef.GetOwnershipPrereqId());

  public bool OwnershipPrereqWingIsOwned(WingDbfRecord wingRecord) => wingRecord.OwnershipPrereqWingId == 0 || this.OwnsWing(wingRecord.OwnershipPrereqWingId);

  private void LoadAdventureMissionsFromDBF()
  {
    foreach (AdventureMissionDbfRecord record in GameDbf.AdventureMission.GetRecords())
    {
      int scenarioId = record.ScenarioId;
      if (this.m_missions.ContainsKey(scenarioId))
      {
        Debug.LogWarning((object) string.Format("AdventureProgressMgr.LoadAdventureMissionsFromDBF(): duplicate entry found for scenario ID {0}", (object) scenarioId));
      }
      else
      {
        string noteDesc = record.NoteDesc;
        AdventureMission.WingProgress requiredProgress = new AdventureMission.WingProgress(record.ReqWingId, record.ReqProgress, record.ReqFlags);
        AdventureMission.WingProgress grantedProgress = new AdventureMission.WingProgress(record.GrantsWingId, record.GrantsProgress, record.GrantsFlags);
        this.m_missions[scenarioId] = new AdventureMission(scenarioId, noteDesc, requiredProgress, grantedProgress);
      }
    }
  }

  private void OnAdventureProgress()
  {
    foreach (Network.AdventureProgress adventureProgress in Network.Get().GetAdventureProgressResponse())
    {
      this.CreateOrUpdateProgress(true, adventureProgress.Wing, adventureProgress.Progress);
      this.CreateOrUpdateWingFlags(true, adventureProgress.Wing, adventureProgress.Flags);
      this.CreateOrUpdateWingAck(adventureProgress.Wing, adventureProgress.Ack);
    }
    this.IsReady = true;
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    List<long> longList = new List<long>();
    foreach (NetCache.ProfileNotice newNotice in newNotices)
    {
      if (newNotice.Type == NetCache.ProfileNotice.NoticeType.ADVENTURE_PROGRESS)
      {
        NetCache.ProfileNoticeAdventureProgress adventureProgress = newNotice as NetCache.ProfileNoticeAdventureProgress;
        if (adventureProgress.Progress.HasValue)
          this.CreateOrUpdateProgress(false, adventureProgress.Wing, adventureProgress.Progress.Value);
        if (adventureProgress.Flags.HasValue)
          this.CreateOrUpdateWingFlags(false, adventureProgress.Wing, adventureProgress.Flags.Value);
        longList.Add(newNotice.NoticeID);
      }
    }
    foreach (long id in longList)
      Network.Get().AckNotice(id);
  }

  private void FireProgressUpdate(
    bool isStartupAction,
    AdventureMission.WingProgress oldProgress,
    AdventureMission.WingProgress newProgress)
  {
    foreach (AdventureProgressMgr.AdventureProgressUpdatedListener progressUpdatedListener in this.m_progressUpdatedListeners.ToArray())
      progressUpdatedListener.Fire(isStartupAction, oldProgress, newProgress);
  }

  private void CreateOrUpdateProgress(bool isStartupAction, int wing, int progress)
  {
    if (!this.m_wingProgress.ContainsKey(wing))
    {
      this.m_wingProgress[wing] = new AdventureMission.WingProgress(wing, progress, 0UL);
      this.FireProgressUpdate(isStartupAction, (AdventureMission.WingProgress) null, this.m_wingProgress[wing]);
    }
    else
    {
      AdventureMission.WingProgress oldProgress = this.m_wingProgress[wing].Clone();
      this.m_wingProgress[wing].SetProgress(progress);
      Log.Adventures.Print("AdventureProgressMgr.CreateOrUpdateProgress: updating wing {0} : PROGRESS {1} (former progress {2})", (object) wing, (object) this.m_wingProgress[wing], (object) oldProgress);
      this.FireProgressUpdate(isStartupAction, oldProgress, this.m_wingProgress[wing]);
    }
  }

  private void CreateOrUpdateWingFlags(bool isStartupAction, int wing, ulong flags)
  {
    if (!this.m_wingProgress.ContainsKey(wing))
    {
      this.m_wingProgress[wing] = new AdventureMission.WingProgress(wing, 0, flags);
      Log.Adventures.Print("AdventureProgressMgr.CreateOrUpdateWingFlags: creating wing {0} : PROGRESS {1}", (object) wing, (object) this.m_wingProgress[wing]);
      this.FireProgressUpdate(isStartupAction, (AdventureMission.WingProgress) null, this.m_wingProgress[wing]);
    }
    else
    {
      AdventureMission.WingProgress oldProgress = this.m_wingProgress[wing].Clone();
      this.m_wingProgress[wing].SetFlags(flags);
      this.FireProgressUpdate(isStartupAction, oldProgress, this.m_wingProgress[wing]);
    }
  }

  private void CreateOrUpdateWingAck(int wing, int ack) => this.m_wingAckState[wing] = ack;

  public delegate void AdventureProgressUpdatedCallback(
    bool isStartupAction,
    AdventureMission.WingProgress oldProgress,
    AdventureMission.WingProgress newProgress,
    object userData);

  private class AdventureProgressUpdatedListener : 
    EventListener<AdventureProgressMgr.AdventureProgressUpdatedCallback>
  {
    public void Fire(
      bool isStartupAction,
      AdventureMission.WingProgress oldProgress,
      AdventureMission.WingProgress newProgress)
    {
      this.m_callback(isStartupAction, oldProgress, newProgress, this.m_userData);
    }
  }
}
