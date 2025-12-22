using Blizzard.GameService.SDK.Client.Integration;
using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Achievement
{
  public static readonly int NEW_ACHIEVE_ACK_PROGRESS = -1;
  private int m_id;
  private bool m_enabled;
  private string m_name = "";
  private string m_description = "";
  private Assets.Achieve.AltTextPredicate m_altTextPredicate;
  private string m_altName;
  private string m_altDescription;
  private Assets.Achieve.Type m_type;
  private int m_maxProgress;
  private TAG_RACE? m_raceReq;
  private TAG_CLASS? m_classReward;
  private TAG_CARD_SET? m_cardSetReq;
  private TAG_CLASS? m_myHeroClassReq;
  private Achievement.ClickTriggerType? m_clickType;
  private SpecialEventType m_eventTrigger;
  private int m_linkToId;
  private Assets.Achieve.Trigger m_trigger = Assets.Achieve.Trigger.NONE;
  private Assets.Achieve.GameMode m_gameMode;
  private Assets.Achieve.Unlocks m_unlockedFeature = Assets.Achieve.Unlocks.NONE;
  private List<RewardData> m_rewards = new List<RewardData>();
  private List<int> m_scenarios = new List<int>();
  private int m_wingID;
  private int m_adventureID;
  private int m_adventureModeID;
  private Assets.Achieve.RewardTiming m_rewardTiming;
  private int m_boosterReq;
  private Assets.Achieve.ClientFlags m_clientFlags;
  private bool m_useGenericRewardVisual;
  private Assets.Achieve.ShowToReturningPlayer m_showToReturningPlayer;
  private int m_questDialogId;
  private bool m_autoDestroy;
  private string m_questTilePrefabName;
  private int m_onCompleteQuestDialogBannerId;
  private CharacterDialogSequence m_onReceivedDialogSequence;
  private CharacterDialogSequence m_onCompleteDialogSequence;
  private CharacterDialogSequence m_onProgress1DialogSequence;
  private CharacterDialogSequence m_onProgress2DialogSequence;
  private CharacterDialogSequence m_onDismissDialogSequence;
  private bool m_isGenericRewardChest;
  private string m_chestVisualPrefabPath = "";
  private string m_customVisualWidget = "";
  private int m_enemyHeroClassId;
  private int m_progress;
  private int m_ackProgress;
  private int m_completionCount;
  private bool m_active;
  private long m_dateGiven;
  private long m_dateCompleted;
  private bool m_canAck;
  private int m_intervalRewardCount;
  private long m_intervalRewardStartDate;
  private List<long> m_rewardNoticeIDs = new List<long>();

  public int ID => this.m_id;

  public bool Enabled => this.DbfRecord.Enabled && this.DbfRecord.EnabledWithProgression;

  public Assets.Achieve.Type AchieveType => this.m_type;

  public int MaxProgress => this.m_maxProgress;

  public TAG_RACE? RaceRequirement => this.m_raceReq;

  public TAG_CLASS? ClassReward => this.m_classReward;

  public TAG_CARD_SET? CardSetRequirement => this.m_cardSetReq;

  public TAG_CLASS? MyHeroClassRequirement => this.m_myHeroClassReq;

  public Achievement.ClickTriggerType? ClickType => this.m_clickType;

  public SpecialEventType EventTrigger => this.m_eventTrigger;

  public int LinkToId => this.m_linkToId;

  public Assets.Achieve.Trigger AchieveTrigger => this.m_trigger;

  public Assets.Achieve.GameMode Mode => this.m_gameMode;

  public Assets.Achieve.Unlocks UnlockedFeature => this.m_unlockedFeature;

  public List<RewardData> Rewards => this.m_rewards;

  public List<int> Scenarios => this.m_scenarios;

  public int WingID => this.m_wingID;

  public int AdventureID => this.m_adventureID;

  public int AdventureModeID => this.m_adventureModeID;

  public Assets.Achieve.RewardTiming RewardTiming => this.m_rewardTiming;

  public int BoosterRequirement => this.m_boosterReq;

  public bool UseGenericRewardVisual => this.m_useGenericRewardVisual;

  public Assets.Achieve.ShowToReturningPlayer ShowToReturningPlayer => this.m_showToReturningPlayer;

  public int QuestDialogId => this.m_questDialogId;

  public bool AutoDestroy => this.m_autoDestroy;

  public string QuestTilePrefabName => this.m_questTilePrefabName;

  public CharacterDialogSequence OnReceivedDialogSequence => this.m_onReceivedDialogSequence;

  public CharacterDialogSequence OnCompleteDialogSequence => this.m_onCompleteDialogSequence;

  public CharacterDialogSequence OnProgress1DialogSequence => this.m_onProgress1DialogSequence;

  public CharacterDialogSequence OnProgress2DialogSequence => this.m_onProgress2DialogSequence;

  public CharacterDialogSequence OnDismissDialogSequence => this.m_onDismissDialogSequence;

  public AchieveDbfRecord DbfRecord { get; private set; }

  public int Progress => this.m_progress;

  public int AcknowledgedProgress => this.m_ackProgress;

  public bool CanBeAcknowledged => this.m_canAck;

  public int CompletionCount => this.m_completionCount;

  public bool Active => this.m_active;

  public long DateGiven => this.m_dateGiven;

  public long DateCompleted => this.m_dateCompleted;

  public int IntervalRewardCount => this.m_intervalRewardCount;

  public long IntervalRewardStartDate => this.m_intervalRewardStartDate;

  public bool IsGenericRewardChest => this.m_isGenericRewardChest;

  public string ChestVisualPrefabPath => this.m_chestVisualPrefabPath;

  public string CustomVisualWidget => this.m_customVisualWidget;

  public bool IsLegendary => (this.m_clientFlags & Assets.Achieve.ClientFlags.IS_LEGENDARY) != 0;

  public bool IsAffectedByDoubleGold => (this.m_clientFlags & Assets.Achieve.ClientFlags.IS_AFFECTED_BY_DOUBLE_GOLD) != 0;

  public bool HasRewardChestVisuals => !string.IsNullOrEmpty(this.m_chestVisualPrefabPath);

  public bool CanShowInQuestLog
  {
    get
    {
      if ((this.m_clientFlags & Assets.Achieve.ClientFlags.SHOW_IN_QUEST_LOG) != Assets.Achieve.ClientFlags.NONE)
        return true;
      switch (this.AchieveType)
      {
        case Assets.Achieve.Type.STARTER:
        case Assets.Achieve.Type.DAILY:
        case Assets.Achieve.Type.NORMAL_QUEST:
          return true;
        case Assets.Achieve.Type.HERO:
        case Assets.Achieve.Type.GOLDHERO:
        case Assets.Achieve.Type.DAILY_REPEATABLE:
        case Assets.Achieve.Type.HIDDEN:
        case Assets.Achieve.Type.INTERNAL_ACTIVE:
        case Assets.Achieve.Type.INTERNAL_INACTIVE:
          return false;
        default:
          return false;
      }
    }
  }

  public bool IsAffectedByFriendWeek => (this.m_clientFlags & Assets.Achieve.ClientFlags.IS_AFFECTED_BY_FRIEND_WEEK) != Assets.Achieve.ClientFlags.NONE;

  public bool IsFriendlyChallengeQuest => this.m_gameMode == Assets.Achieve.GameMode.FRIENDLY;

  public bool GameModeRequiresNonFriendlyChallenge
  {
    get
    {
      switch (this.m_gameMode)
      {
        case Assets.Achieve.GameMode.ANY:
        case Assets.Achieve.GameMode.FRIENDLY:
          return false;
        default:
          return true;
      }
    }
  }

  public bool CanBeCancelled => this.IsLegendary || this.AchieveType == Assets.Achieve.Type.DAILY;

  public PegasusShared.PlayerType PlayerType => (PegasusShared.PlayerType) this.DbfRecord.PlayerType;

  public Achievement()
  {
  }

  public Achievement(
    AchieveDbfRecord dbfRecord,
    int id,
    Assets.Achieve.Type achieveType,
    int maxProgress,
    int linkToId,
    Assets.Achieve.Trigger trigger,
    Assets.Achieve.GameMode gameMode,
    TAG_RACE? raceReq,
    TAG_CLASS? classReward,
    TAG_CARD_SET? cardSetReq,
    TAG_CLASS? myHeroClassReq,
    Achievement.ClickTriggerType? clickType,
    Assets.Achieve.Unlocks unlockedFeature,
    List<RewardData> rewards,
    List<int> scenarios,
    int wingID,
    int adventureID,
    int adventureModeID,
    Assets.Achieve.RewardTiming rewardTiming,
    int boosterReq,
    bool useGenericRewardVisual,
    Assets.Achieve.ShowToReturningPlayer showToReturningPlayer,
    int questDialogId,
    bool autoDestroy,
    string questTilePrefabName,
    int onCompleteQuestDialogBannerId,
    CharacterDialogSequence onReceivedDialogSequence,
    CharacterDialogSequence onCompleteDialogSequence,
    CharacterDialogSequence onProgress1DialogSequence,
    CharacterDialogSequence onProgress2DialogSequence,
    CharacterDialogSequence onDismissDialogSequence,
    bool isGenericRewardChest,
    string chestVisualPrefabPath,
    string customVisualWidget,
    int enemyHeroClassId)
  {
    this.DbfRecord = dbfRecord == null ? new AchieveDbfRecord() : dbfRecord;
    this.m_id = id;
    this.m_type = achieveType;
    this.m_maxProgress = maxProgress;
    this.m_linkToId = linkToId;
    this.m_trigger = trigger;
    this.m_gameMode = gameMode;
    this.m_raceReq = raceReq;
    this.m_classReward = classReward;
    this.m_cardSetReq = cardSetReq;
    this.m_myHeroClassReq = myHeroClassReq;
    this.m_clickType = clickType;
    this.SetRewards(rewards);
    this.m_unlockedFeature = unlockedFeature;
    this.m_scenarios = scenarios;
    this.m_wingID = wingID;
    this.m_adventureID = adventureID;
    this.m_adventureModeID = adventureModeID;
    this.m_rewardTiming = rewardTiming;
    this.m_boosterReq = boosterReq;
    this.m_useGenericRewardVisual = useGenericRewardVisual;
    this.m_showToReturningPlayer = showToReturningPlayer;
    this.m_questDialogId = questDialogId;
    this.m_autoDestroy = autoDestroy;
    this.m_questTilePrefabName = questTilePrefabName;
    this.m_onCompleteQuestDialogBannerId = onCompleteQuestDialogBannerId;
    this.m_onReceivedDialogSequence = onReceivedDialogSequence;
    this.m_onCompleteDialogSequence = onCompleteDialogSequence;
    this.m_onProgress1DialogSequence = onProgress1DialogSequence;
    this.m_onProgress2DialogSequence = onProgress2DialogSequence;
    this.m_onDismissDialogSequence = onDismissDialogSequence;
    this.m_isGenericRewardChest = isGenericRewardChest;
    this.m_chestVisualPrefabPath = chestVisualPrefabPath;
    this.m_customVisualWidget = customVisualWidget;
    this.m_enemyHeroClassId = enemyHeroClassId;
    this.m_progress = 0;
    this.m_ackProgress = Achievement.NEW_ACHIEVE_ACK_PROGRESS;
    this.m_completionCount = 0;
    this.m_active = false;
    this.m_dateGiven = 0L;
    this.m_dateCompleted = 0L;
  }

  public string Name => !string.IsNullOrEmpty(this.m_altName) && AchieveManager.IsPredicateTrue(this.m_altTextPredicate) ? this.m_altName : this.m_name;

  public string Description => !string.IsNullOrEmpty(this.m_altDescription) && AchieveManager.IsPredicateTrue(this.m_altTextPredicate) ? this.m_altDescription : this.m_description;

  public void SetClientFlags(Assets.Achieve.ClientFlags clientFlags) => this.m_clientFlags = clientFlags;

  public void SetAltTextPredicate(Assets.Achieve.AltTextPredicate altTextPredicate) => this.m_altTextPredicate = altTextPredicate;

  public void SetName(string name, string altName)
  {
    this.m_name = name;
    this.m_altName = altName;
  }

  public void SetDescription(string description, string altDescription)
  {
    this.m_description = description;
    this.m_altDescription = altDescription;
  }

  public void SetEventTrigger(SpecialEventType eventType) => this.m_eventTrigger = eventType;

  public void OnAchieveData(PegasusUtil.Achieve achieveData)
  {
    this.SetProgress(achieveData.Progress);
    this.SetAcknowledgedProgress(achieveData.AckProgress);
    this.m_completionCount = achieveData.HasCompletionCount ? achieveData.CompletionCount : 0;
    this.m_active = achieveData.HasActive && achieveData.Active;
    this.m_dateGiven = achieveData.HasDateGiven ? TimeUtils.PegDateToFileTimeUtc(achieveData.DateGiven) : 0L;
    this.m_dateCompleted = achieveData.HasDateCompleted ? TimeUtils.PegDateToFileTimeUtc(achieveData.DateCompleted) : 0L;
    this.m_canAck = !achieveData.HasDoNotAck || !achieveData.DoNotAck;
    if (achieveData.HasIntervalRewardCount)
      this.m_intervalRewardCount = achieveData.IntervalRewardCount;
    this.m_intervalRewardStartDate = achieveData.HasIntervalRewardStart ? TimeUtils.PegDateToFileTimeUtc(achieveData.IntervalRewardStart) : 0L;
    this.AutoAckIfNeeded();
  }

  public void OnAchieveNotification(AchievementNotification notification)
  {
    PegasusUtil.Achieve achieveData = new PegasusUtil.Achieve();
    achieveData.Id = (int) notification.AchievementId;
    achieveData.CompletionCount = this.CompletionCount;
    achieveData.Progress = this.Progress;
    achieveData.Active = this.Active;
    achieveData.DoNotAck = !this.CanBeAcknowledged;
    achieveData.DateCompleted = TimeUtils.FileTimeUtcToPegDate(this.DateCompleted);
    achieveData.DateGiven = TimeUtils.FileTimeUtcToPegDate(this.DateGiven);
    achieveData.AckProgress = this.AcknowledgedProgress;
    Log.Achievements.Print("OnAchieveNotification PlayerID={0} ID={1} Complete={2} New={3} Remove={4} Amount={5}", (object) notification.PlayerId, (object) notification.AchievementId, (object) notification.Complete, (object) notification.NewAchievement, (object) notification.RemoveAchievement, (object) notification.Amount);
    if (notification.NewAchievement)
    {
      achieveData.DateGiven = TimeUtils.FileTimeUtcToPegDate(DateTime.UtcNow.ToFileTimeUtc());
      achieveData.Active = true;
      achieveData.AckProgress = Achievement.NEW_ACHIEVE_ACK_PROGRESS;
      achieveData.Progress = 0;
    }
    achieveData.Progress += notification.Amount;
    if (notification.Complete)
    {
      achieveData.Progress = this.MaxProgress;
      ++achieveData.CompletionCount;
      achieveData.DateCompleted = TimeUtils.FileTimeUtcToPegDate(DateTime.UtcNow.ToFileTimeUtc());
      achieveData.Active = false;
      achieveData.DoNotAck = false;
    }
    if (notification.RemoveAchievement)
      achieveData.Active = false;
    if (!achieveData.Active)
      this.OnAchieveData(achieveData);
    else
      this.UpdateActiveAchieve(achieveData);
  }

  public void UpdateActiveAchieve(PegasusUtil.Achieve achieveData)
  {
    this.SetProgress(achieveData.Progress);
    this.SetAcknowledgedProgress(achieveData.AckProgress);
    this.m_active = true;
    this.m_dateGiven = achieveData.HasDateGiven ? TimeUtils.PegDateToFileTimeUtc(achieveData.DateGiven) : 0L;
    if (achieveData.HasIntervalRewardCount)
      this.m_intervalRewardCount = achieveData.IntervalRewardCount;
    if (achieveData.HasIntervalRewardStart)
      this.m_intervalRewardStartDate = TimeUtils.PegDateToFileTimeUtc(achieveData.IntervalRewardStart);
    this.AutoAckIfNeeded();
  }

  public void AddRewardNoticeID(long noticeID)
  {
    if (this.m_rewardNoticeIDs.Contains(noticeID))
      return;
    if (this.IsCompleted() && !this.NeedToAcknowledgeProgress(false))
      Network.Get().AckNotice(noticeID);
    this.m_rewardNoticeIDs.Add(noticeID);
  }

  public void OnCancelSuccess() => this.m_active = false;

  public bool IsInternal() => Assets.Achieve.Type.INTERNAL_ACTIVE == this.AchieveType || Assets.Achieve.Type.INTERNAL_INACTIVE == this.AchieveType;

  public bool IsNewlyActive() => this.m_ackProgress == Achievement.NEW_ACHIEVE_ACK_PROGRESS;

  public bool IsCompleted() => this.Progress >= this.MaxProgress;

  public bool IsNewlyCompleted() => this.IsCompleted() && this.AcknowledgedProgress < this.MaxProgress;

  public bool IsActiveLicenseAddedAchieve() => Assets.Achieve.Trigger.LICENSEADDED == this.AchieveTrigger && this.Active;

  public void AckCurrentProgressAndRewardNotices() => this.AckCurrentProgressAndRewardNotices(false);

  public void AckCurrentProgressAndRewardNotices(bool ackIntermediateProgress)
  {
    long[] array = this.m_rewardNoticeIDs.ToArray();
    this.m_rewardNoticeIDs.Clear();
    Network network = Network.Get();
    foreach (long id in array)
      network.AckNotice(id);
    if (!this.NeedToAcknowledgeProgress(ackIntermediateProgress))
      return;
    this.m_ackProgress = this.Progress;
    if (!this.m_canAck)
      return;
    network.AckAchieveProgress(this.ID, this.AcknowledgedProgress);
  }

  public void IncrementIntervalRewardCount()
  {
    if (this.m_intervalRewardCount < 0)
      this.m_intervalRewardCount = 0;
    ++this.m_intervalRewardCount;
    if (this.m_intervalRewardStartDate != 0L)
      return;
    this.m_intervalRewardStartDate = DateTime.UtcNow.ToFileTimeUtc();
  }

  public bool IsValidFriendlyPlayerChallengeType(PegasusShared.PlayerType playerType) => this.PlayerType == PegasusShared.PlayerType.PT_ANY || playerType == this.PlayerType;

  public override string ToString() => string.Format("[Achievement: ID={0} Type={1} Name='{2}' MaxProgress={3} Progress={4} AckProgress={5} IsActive={6} DateGiven={7} DateCompleted={8} Description='{9}' Trigger={10} CanAck={11}]", (object) this.ID, (object) this.AchieveType, (object) this.m_name, (object) this.MaxProgress, (object) this.Progress, (object) this.AcknowledgedProgress, (object) this.Active, (object) this.DateGiven, (object) this.DateCompleted, (object) this.m_description, (object) this.AchieveTrigger, (object) this.m_canAck);

  public UserAttentionBlocker GetUserAttentionBlocker() => (UserAttentionBlocker) this.DbfRecord.AttentionBlocker;

  public AchieveRegionDataDbfRecord GetCurrentRegionData()
  {
    BnetRegion currentRegion = BattleNet.GetCurrentRegion();
    return GameDbf.AchieveRegionData.GetRecord((Predicate<AchieveRegionDataDbfRecord>) (dbf => dbf.AchieveId == this.ID && (BnetRegion) dbf.Region == currentRegion)) ?? GameDbf.AchieveRegionData.GetRecord((Predicate<AchieveRegionDataDbfRecord>) (dbf => dbf.AchieveId == this.ID && dbf.Region == 0));
  }

  private bool NeedToAcknowledgeProgress(bool ackIntermediateProgress) => this.AcknowledgedProgress < this.MaxProgress && this.AcknowledgedProgress != this.Progress && (ackIntermediateProgress || this.Progress <= 0 || this.Progress >= this.MaxProgress);

  private void SetProgress(int progress) => this.m_progress = progress;

  private void SetAcknowledgedProgress(int acknowledgedProgress) => this.m_ackProgress = Mathf.Clamp(acknowledgedProgress, Achievement.NEW_ACHIEVE_ACK_PROGRESS, this.Progress);

  private void AutoAckIfNeeded()
  {
    if ((this.IsInternal() ? 1 : (Assets.Achieve.Type.DAILY_REPEATABLE == this.AchieveType ? 1 : 0)) == 0)
      return;
    this.AckCurrentProgressAndRewardNotices();
  }

  private void SetRewards(List<RewardData> rewardDataList)
  {
    this.m_rewards = new List<RewardData>((IEnumerable<RewardData>) rewardDataList);
    this.FixUpRewardOrigins(this.m_rewards);
  }

  private void FixUpRewardOrigins(List<RewardData> rewardDataList)
  {
    foreach (RewardData rewardData in rewardDataList)
      rewardData.SetOrigin(NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT, (long) this.ID);
  }

  public enum ClickTriggerType
  {
    BUTTON_PLAY = 1,
    BUTTON_ARENA = 2,
    BUTTON_ADVENTURE = 3,
  }
}
