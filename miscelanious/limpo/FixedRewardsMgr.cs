using Assets;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using FixedReward;
using Hearthstone;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FixedRewardsMgr : IService
{
  private readonly HashSet<NetCache.CardDefinition> m_craftableCardRewards = new HashSet<NetCache.CardDefinition>();
  private readonly Map<int, MetaAction> m_earnedMetaActionRewards = new Map<int, MetaAction>();
  private readonly RewardQueue m_rewardQueue = new RewardQueue();
  private readonly HashSet<int> m_rewardMapIDsAwarded = new HashSet<int>();
  private bool m_isStartupFinished;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    FixedRewardsMgr fixedRewardsMgr = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new Action(fixedRewardsMgr.WillReset);
    HearthstoneApplication.Get().Resetting += new Action(fixedRewardsMgr.OnReset);
    serviceLocator.Get<AdventureProgressMgr>().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(fixedRewardsMgr.OnAdventureProgressUpdate));
    serviceLocator.Get<NetCache>().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(fixedRewardsMgr.OnNewNotices));
    serviceLocator.Get<AchieveManager>().RegisterAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(fixedRewardsMgr.OnAchievesUpdated));
    serviceLocator.Get<AccountLicenseMgr>().RegisterAccountLicensesChangedListener(new AccountLicenseMgr.AccountLicensesChangedCallback(fixedRewardsMgr.OnAccountLicensesUpdate));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[6]
  {
    typeof (AdventureProgressMgr),
    typeof (NetCache),
    typeof (GameDbf),
    typeof (AchieveManager),
    typeof (AccountLicenseMgr),
    typeof (CardBackManager)
  };

  public void Shutdown()
  {
  }

  private void WillReset()
  {
    this.m_craftableCardRewards.Clear();
    this.m_earnedMetaActionRewards.Clear();
    this.m_rewardQueue.Clear();
    this.m_rewardMapIDsAwarded.Clear();
    this.m_isStartupFinished = false;
  }

  private void OnReset()
  {
    ServiceManager.Get<AdventureProgressMgr>().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdate));
    ServiceManager.Get<AchieveManager>().RegisterAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated));
  }

  public static FixedRewardsMgr Get() => ServiceManager.Get<FixedRewardsMgr>();

  public void InitStartupFixedRewards()
  {
    this.m_rewardMapIDsAwarded.Clear();
    List<CardRewardData> cardRewards = new List<CardRewardData>();
    foreach (AdventureMission.WingProgress wingProgress in AdventureProgressMgr.Get().GetAllProgress())
    {
      if (wingProgress.MeetsFlagsRequirement(1UL))
      {
        this.TriggerWingProgressAction(FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, wingProgress.Wing, wingProgress.Progress, cardRewards);
        this.TriggerWingFlagsAction(FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, wingProgress.Wing, wingProgress.Flags, cardRewards);
      }
    }
    this.GrantAchieveRewards(cardRewards);
    foreach (AccountLicenseInfo accountLicenseInfo in AccountLicenseMgr.Get().GetAllOwnedAccountLicenseInfo())
      this.TriggerAccountLicenseFlagsAction(FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, accountLicenseInfo.License, accountLicenseInfo.Flags_, cardRewards);
    this.m_isStartupFinished = true;
  }

  public bool IsStartupFinished() => this.m_isStartupFinished;

  public bool HasRewardsToShow(IEnumerable<Achieve.RewardTiming> rewardTimings) => this.m_rewardQueue.HasRewardsToShow(rewardTimings);

  public bool ShowFixedRewards(
    UserAttentionBlocker blocker,
    HashSet<Achieve.RewardTiming> rewardVisualTimings,
    FixedRewardsMgr.DelOnAllFixedRewardsShown allRewardsShownCallback,
    FixedRewardsMgr.DelPositionNonToastReward positionNonToastRewardCallback)
  {
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(blocker, string.Format("FixedRewardsMgr.ShowFixedRewards:{0}", (object) blocker)) || StoreManager.Get().IsPromptShowing)
      return false;
    FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo callbackInfo = new FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo()
    {
      rewardMapIDsToShow = new List<RewardMapIDToShow>(),
      onAllRewardsShownCallback = allRewardsShownCallback,
      positionNonToastRewardCallback = positionNonToastRewardCallback,
      showingCheatRewards = false
    };
    foreach (Achieve.RewardTiming rewardVisualTiming in rewardVisualTimings)
    {
      HashSet<RewardMapIDToShow> rewards;
      if (this.m_rewardQueue.TryGetRewards(rewardVisualTiming, out rewards))
      {
        if (PopupDisplayManager.SuppressPopupsForNewPlayer)
        {
          foreach (RewardMapIDToShow rewardMapIdToShow in rewards)
          {
            FixedRewardMapDbfRecord record = GameDbf.FixedRewardMap.GetRecord(rewardMapIdToShow.rewardMapID);
            if (record != null && record.ActionRecord.Type == FixedRewardAction.Type.HERO_LEVEL)
              callbackInfo.rewardMapIDsToShow.Add(rewardMapIdToShow);
          }
          foreach (RewardMapIDToShow rewardMapIdToShow in callbackInfo.rewardMapIDsToShow)
            rewards.Remove(rewardMapIdToShow);
        }
        else
        {
          callbackInfo.rewardMapIDsToShow.AddRange((IEnumerable<RewardMapIDToShow>) rewards);
          this.m_rewardQueue.Clear(rewardVisualTiming);
        }
      }
    }
    if (callbackInfo.rewardMapIDsToShow.Count == 0)
      return false;
    if (PopupDisplayManager.ShouldDisableNotificationOnLogin())
    {
      RewardMapIDToShow rewardMapIdToShow = callbackInfo.rewardMapIDsToShow[0];
      callbackInfo.rewardMapIDsToShow.RemoveAt(0);
      if (rewardMapIdToShow.achieveID != RewardMapIDToShow.NoAchieveID)
        AchieveManager.Get().GetAchievement(rewardMapIdToShow.achieveID)?.AckCurrentProgressAndRewardNotices();
      if (callbackInfo.onAllRewardsShownCallback != null)
        callbackInfo.onAllRewardsShownCallback();
      return false;
    }
    callbackInfo.rewardMapIDsToShow.Sort((Comparison<RewardMapIDToShow>) ((a, b) => a.sortOrder - b.sortOrder));
    this.ShowFixedRewards_Internal(blocker, callbackInfo);
    return true;
  }

  public bool Cheat_ShowFixedReward(
    int fixedRewardMapID,
    FixedRewardsMgr.DelPositionNonToastReward positionNonToastRewardCallback)
  {
    if (!HearthstoneApplication.IsInternal())
      return false;
    FixedRewardMapDbfRecord record = GameDbf.FixedRewardMap.GetRecord(fixedRewardMapID);
    int sortOrder = record != null ? record.SortOrder : 0;
    this.ShowFixedRewards_Internal(UserAttentionBlocker.NONE, new FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo()
    {
      rewardMapIDsToShow = new List<RewardMapIDToShow>()
      {
        new RewardMapIDToShow(fixedRewardMapID, RewardMapIDToShow.NoAchieveID, sortOrder)
      },
      onAllRewardsShownCallback = (FixedRewardsMgr.DelOnAllFixedRewardsShown) null,
      positionNonToastRewardCallback = positionNonToastRewardCallback,
      showingCheatRewards = true
    });
    return true;
  }

  public bool CanCraftCard(string cardID, TAG_PREMIUM premium)
  {
    if (GameUtils.GetFixedRewardForCard(cardID, premium) == null)
      return true;
    return this.m_craftableCardRewards.Contains(new NetCache.CardDefinition()
    {
      Name = cardID,
      Premium = premium
    }) || ((!GameUtils.IsCardCraftableWhenWild(cardID) ? 0 : (GameUtils.IsWildCard(cardID) ? 1 : 0)) | (!GameUtils.IsClassicCard(cardID) ? (false ? 1 : 0) : (this.CanCraftCard(GameUtils.TranslateDbIdToCardId(GameUtils.GetCardTagValue(cardID, GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID)), premium) ? 1 : 0))) != 0;
  }

  private void OnAdventureProgressUpdate(
    bool isStartupAction,
    AdventureMission.WingProgress oldProgress,
    AdventureMission.WingProgress newProgress,
    object userData)
  {
    List<CardRewardData> cardRewards = new List<CardRewardData>();
    if (isStartupAction || newProgress == null || !newProgress.IsOwned())
      return;
    if (oldProgress == null)
    {
      this.TriggerWingProgressAction(FixedRewardsMgr.ShowVisualOption.SHOW, newProgress.Wing, newProgress.Progress, cardRewards);
      this.TriggerWingFlagsAction(FixedRewardsMgr.ShowVisualOption.SHOW, newProgress.Wing, newProgress.Flags, cardRewards);
    }
    else
    {
      bool flag = !oldProgress.IsOwned() && newProgress.IsOwned();
      if (flag || oldProgress.Progress != newProgress.Progress)
        this.TriggerWingProgressAction(flag ? FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW : FixedRewardsMgr.ShowVisualOption.SHOW, newProgress.Wing, newProgress.Progress, cardRewards);
      if ((long) oldProgress.Flags != (long) newProgress.Flags)
        this.TriggerWingFlagsAction(FixedRewardsMgr.ShowVisualOption.SHOW, newProgress.Wing, newProgress.Flags, cardRewards);
    }
    CollectionManager.Get().AddCardRewards(cardRewards, false);
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    bool flag = false;
    foreach (NetCache.ProfileNotice newNotice in newNotices)
    {
      if (NetCache.ProfileNotice.NoticeType.HERO_LEVEL_UP == newNotice.Type)
      {
        NetCache.ProfileNoticeLevelUp profileNoticeLevelUp = newNotice as NetCache.ProfileNoticeLevelUp;
        FixedRewardsMgr.ShowVisualOption showRewardVisual = newNotice.Origin == NetCache.ProfileNotice.NoticeOrigin.LEVEL_UP ? FixedRewardsMgr.ShowVisualOption.SHOW : FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW;
        this.TriggerHeroLevelAction(showRewardVisual, profileNoticeLevelUp.HeroClass, profileNoticeLevelUp.NewLevel);
        this.TriggerTotalHeroLevelAction(showRewardVisual, profileNoticeLevelUp.TotalLevel);
        Network.Get().AckNotice(newNotice.NoticeID);
      }
      else if (NetCache.ProfileNotice.NoticeType.DECK_GRANTED == newNotice.Type)
        flag = true;
    }
    if (CollectionManager.Get() == null || !flag || SceneMgr.Get().GetMode() != SceneMgr.Mode.COLLECTIONMANAGER)
      return;
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
  }

  private void OnAchievesUpdated(
    List<Achievement> updatedAchieves,
    List<Achievement> achieves,
    object userData)
  {
    List<CardRewardData> cardRewards = new List<CardRewardData>();
    foreach (Achievement achieve in achieves)
      this.TriggerAchieveAction(FixedRewardsMgr.ShowVisualOption.SHOW, achieve.ID, cardRewards);
    if (CollectionManager.Get() == null)
      return;
    CollectionManager.Get().AddCardRewards(cardRewards, false);
  }

  private void OnAccountLicensesUpdate(
    List<AccountLicenseInfo> changedAccountLicenses,
    object userData)
  {
    List<CardRewardData> cardRewards = new List<CardRewardData>();
    foreach (AccountLicenseInfo changedAccountLicense in changedAccountLicenses)
    {
      if (AccountLicenseMgr.Get().OwnsAccountLicense(changedAccountLicense))
        this.TriggerAccountLicenseFlagsAction(FixedRewardsMgr.ShowVisualOption.FORCE_SHOW, changedAccountLicense.License, changedAccountLicense.Flags_, cardRewards);
    }
    CollectionManager.Get().AddCardRewards(cardRewards, false);
  }

  private MetaAction GetEarnedMetaActionReward(int metaActionID)
  {
    if (!this.m_earnedMetaActionRewards.ContainsKey(metaActionID))
      this.m_earnedMetaActionRewards[metaActionID] = new MetaAction(metaActionID);
    return this.m_earnedMetaActionRewards[metaActionID];
  }

  private void UpdateEarnedMetaActionFlags(int metaActionID, ulong addFlags, ulong removeFlags) => this.GetEarnedMetaActionReward(metaActionID).UpdateFlags(addFlags, removeFlags);

  private bool QueueRewardVisual(FixedRewardMapDbfRecord record, int achieveID)
  {
    Achieve.RewardTiming rewardTiming = record.GetRewardTiming();
    Log.Achievements.Print(string.Format("QueueRewardVisual achieveID={0} fixedRewardMapId={1} {2} {3}", (object) achieveID, (object) record.ID, (object) record.NoteDesc, (object) rewardTiming));
    FixedReward.Reward fixedReward = record.GetFixedReward();
    if (FixedRewardUtils.ShouldSkipRewardVisual(rewardTiming, fixedReward))
      return false;
    this.m_rewardQueue.Add(rewardTiming, new RewardMapIDToShow(record.ID, achieveID, record.SortOrder));
    return true;
  }

  private void TriggerRewardsForAction(
    int actionID,
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    List<CardRewardData> cardRewards)
  {
    this.TriggerRewardsForAction(actionID, showRewardVisual, cardRewards, RewardMapIDToShow.NoAchieveID);
  }

  private void TriggerRewardsForAction(
    int actionID,
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    List<CardRewardData> cardRewards,
    int achieveID)
  {
    foreach (FixedRewardMapDbfRecord record in GameUtils.GetFixedRewardMapRecordsForAction(actionID))
    {
      FixedReward.Reward fixedReward = record.GetFixedReward();
      int id = record.ID;
      if (this.m_rewardMapIDsAwarded.Contains(id))
      {
        if (showRewardVisual != FixedRewardsMgr.ShowVisualOption.FORCE_SHOW)
          continue;
      }
      else
        this.m_rewardMapIDsAwarded.Add(id);
      if (record.RewardCount > 0)
      {
        bool flag = showRewardVisual != 0;
        if (fixedReward.FixedCardRewardData != null && (!flag || !this.QueueRewardVisual(record, achieveID)))
          cardRewards.Add(fixedReward.FixedCardRewardData);
        if (fixedReward.FixedCardBackRewardData != null && (!flag || !this.QueueRewardVisual(record, achieveID)))
          CardBackManager.Get().AddNewCardBack(fixedReward.FixedCardBackRewardData.CardBackID);
        if (fixedReward.FixedCraftableCardRewardData != null)
          this.m_craftableCardRewards.Add(fixedReward.FixedCraftableCardRewardData);
        if (fixedReward.MetaActionData != null)
        {
          this.UpdateEarnedMetaActionFlags(fixedReward.MetaActionData.MetaActionID, fixedReward.MetaActionData.MetaActionFlags, 0UL);
          this.TriggerMetaActionFlagsAction(showRewardVisual, fixedReward.MetaActionData.MetaActionID, cardRewards);
        }
      }
    }
  }

  private void TriggerWingProgressAction(
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    int wingID,
    int progress,
    List<CardRewardData> cardRewards)
  {
    foreach (FixedRewardActionDbfRecord fixedActionRecord in GameUtils.GetFixedActionRecords(FixedRewardAction.Type.WING_PROGRESS))
    {
      if (fixedActionRecord.WingId == wingID && fixedActionRecord.WingProgress <= progress && SpecialEventManager.Get().IsEventActive(fixedActionRecord.ActiveEvent, false))
        this.TriggerRewardsForAction(fixedActionRecord.ID, showRewardVisual, cardRewards);
    }
  }

  private void TriggerWingFlagsAction(
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    int wingID,
    ulong flags,
    List<CardRewardData> cardRewards)
  {
    foreach (FixedRewardActionDbfRecord fixedActionRecord in GameUtils.GetFixedActionRecords(FixedRewardAction.Type.WING_FLAGS))
    {
      if (fixedActionRecord.WingId == wingID)
      {
        ulong wingFlags = fixedActionRecord.WingFlags;
        if (((long) wingFlags & (long) flags) == (long) wingFlags && SpecialEventManager.Get().IsEventActive(fixedActionRecord.ActiveEvent, false))
          this.TriggerRewardsForAction(fixedActionRecord.ID, showRewardVisual, cardRewards);
      }
    }
  }

  private void TriggerAchieveAction(
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    int achieveId,
    List<CardRewardData> cardRewards)
  {
    foreach (FixedRewardActionDbfRecord fixedActionRecord in GameUtils.GetFixedActionRecords(FixedRewardAction.Type.ACHIEVE))
    {
      if (fixedActionRecord.AchieveId == achieveId && SpecialEventManager.Get().IsEventActive(fixedActionRecord.ActiveEvent, false))
        this.TriggerRewardsForAction(fixedActionRecord.ID, showRewardVisual, cardRewards, achieveId);
    }
  }

  private void TriggerTotalHeroLevelAction(
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    int totalHeroLevel)
  {
    List<CardRewardData> cardRewards = new List<CardRewardData>();
    foreach (FixedRewardActionDbfRecord fixedActionRecord in GameUtils.GetFixedActionRecords(FixedRewardAction.Type.TOTAL_HERO_LEVEL))
    {
      if (fixedActionRecord.TotalHeroLevel == totalHeroLevel && SpecialEventManager.Get().IsEventActive(fixedActionRecord.ActiveEvent, false))
        this.TriggerRewardsForAction(fixedActionRecord.ID, showRewardVisual, cardRewards);
    }
  }

  private void TriggerHeroLevelAction(
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    int classID,
    int heroLevel)
  {
    List<CardRewardData> cardRewards = new List<CardRewardData>();
    foreach (FixedRewardActionDbfRecord fixedActionRecord in GameUtils.GetFixedActionRecords(FixedRewardAction.Type.HERO_LEVEL))
    {
      if (fixedActionRecord.ClassId == classID && fixedActionRecord.HeroLevel == heroLevel && SpecialEventManager.Get().IsEventActive(fixedActionRecord.ActiveEvent, false))
        this.TriggerRewardsForAction(fixedActionRecord.ID, showRewardVisual, cardRewards);
    }
  }

  private void TriggerAccountLicenseFlagsAction(
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    long license,
    ulong flags,
    List<CardRewardData> cardRewards)
  {
    foreach (FixedRewardActionDbfRecord fixedActionRecord in GameUtils.GetFixedActionRecords(FixedRewardAction.Type.ACCOUNT_LICENSE_FLAGS))
    {
      if (fixedActionRecord.AccountLicenseId == license)
      {
        ulong accountLicenseFlags = fixedActionRecord.AccountLicenseFlags;
        if (((long) accountLicenseFlags & (long) flags) == (long) accountLicenseFlags && SpecialEventManager.Get().IsEventActive(fixedActionRecord.ActiveEvent, false))
          this.TriggerRewardsForAction(fixedActionRecord.ID, showRewardVisual, cardRewards);
      }
    }
  }

  private void TriggerMetaActionFlagsAction(
    FixedRewardsMgr.ShowVisualOption showRewardVisual,
    int metaActionID,
    List<CardRewardData> cardRewards)
  {
    FixedRewardActionDbfRecord record = GameDbf.FixedRewardAction.GetRecord(metaActionID);
    if (record == null)
      return;
    ulong metaActionFlags = record.MetaActionFlags;
    if (!this.GetEarnedMetaActionReward(metaActionID).HasAllRequiredFlags(metaActionFlags) || !SpecialEventManager.Get().IsEventActive(record.ActiveEvent, false))
      return;
    this.TriggerRewardsForAction(metaActionID, showRewardVisual, cardRewards);
  }

  private void ShowFixedRewards_Internal(
    UserAttentionBlocker blocker,
    FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo callbackInfo)
  {
    if (callbackInfo.rewardMapIDsToShow.Count == 0)
    {
      if (callbackInfo.onAllRewardsShownCallback == null)
        return;
      callbackInfo.onAllRewardsShownCallback();
    }
    else
    {
      RewardMapIDToShow rewardMapIdToShow = callbackInfo.rewardMapIDsToShow[0];
      callbackInfo.rewardMapIDsToShow.RemoveAt(0);
      FixedRewardMapDbfRecord record = GameDbf.FixedRewardMap.GetRecord(rewardMapIdToShow.rewardMapID);
      FixedReward.Reward fixedReward = record.GetFixedReward();
      RewardData rewardData = (RewardData) null;
      if (fixedReward.FixedCardRewardData != null)
        rewardData = (RewardData) fixedReward.FixedCardRewardData;
      else if (fixedReward.FixedCardBackRewardData != null)
        rewardData = (RewardData) fixedReward.FixedCardBackRewardData;
      else if (fixedReward.FixedRewardData != null)
        rewardData = (RewardData) fixedReward.FixedRewardData;
      Log.Achievements.Print("Showing Fixed Reward: " + (object) rewardMapIdToShow.achieveID);
      if (rewardData == null)
      {
        this.ShowFixedRewards_Internal(blocker, callbackInfo);
      }
      else
      {
        if (callbackInfo.showingCheatRewards)
          rewardData.MarkAsDummyReward();
        if (rewardMapIdToShow.achieveID != RewardMapIDToShow.NoAchieveID)
          AchieveManager.Get().GetAchievement(rewardMapIdToShow.achieveID)?.AckCurrentProgressAndRewardNotices();
        if (record.UseQuestToast)
        {
          string toastName = (string) record.ToastName;
          string toastDescription = (string) record.ToastDescription;
          QuestToast.ShowFixedRewardQuestToast(blocker, (QuestToast.DelOnCloseQuestToast) (userData => this.ShowFixedRewards_Internal(blocker, callbackInfo)), rewardData, toastName, toastDescription);
        }
        else
          rewardData.LoadRewardObject((Reward.DelOnRewardLoaded) ((reward, callbackData) =>
          {
            reward.transform.localPosition = (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
            {
              PC = new Vector3(0.0f, 0.0f, 43f),
              Phone = new Vector3(0.0f, 0.0f, 35f)
            };
            PlatformDependentValue<Vector3> rewardPunchScale = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
            {
              PC = new Vector3(27.6f, 27.6f, 27.6f),
              Phone = new Vector3(26.4f, 26.4f, 26.4f)
            };
            PlatformDependentValue<Vector3> rewardScale = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
            {
              PC = new Vector3(23f, 23f, 23f),
              Phone = new Vector3(22f, 22f, 22f)
            };
            OverlayUI.Get().AddGameObject(reward.gameObject);
            LayerUtils.SetLayer(reward.gameObject, GameLayer.UI);
            if (callbackInfo.positionNonToastRewardCallback != null)
              callbackInfo.positionNonToastRewardCallback(reward);
            bool updateCacheValues = true;
            RewardUtils.ShowReward(blocker, reward, updateCacheValues, (Vector3) rewardPunchScale, (Vector3) rewardScale, (AnimationUtil.DelOnShownWithPunch) (showRewardUserData =>
            {
              reward.RegisterClickListener(new Reward.OnClickedCallback(this.OnNonToastRewardClicked), (object) callbackInfo);
              reward.EnableClickCatcher(true);
            }), (object) null);
          }));
      }
    }
  }

  private void OnNonToastRewardClicked(Reward reward, object userData)
  {
    FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo shownCallbackInfo = userData as FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo;
    reward.RemoveClickListener(new Reward.OnClickedCallback(this.OnNonToastRewardClicked), (object) shownCallbackInfo);
    reward.Hide(true);
    this.ShowFixedRewards_Internal(UserAttentionBlocker.NONE, shownCallbackInfo);
  }

  private void GrantAchieveRewards(List<CardRewardData> cardRewards)
  {
    AchieveManager achieveManager = AchieveManager.Get();
    if (achieveManager == null)
    {
      Debug.LogWarning((object) "FixedRewardsMgr.GrantAchieveRewards(): null == AchieveManager.Get()");
    }
    else
    {
      foreach (Achievement completedAchieve in achieveManager.GetCompletedAchieves())
        this.TriggerAchieveAction(completedAchieve.IsNewlyCompleted() ? FixedRewardsMgr.ShowVisualOption.SHOW : FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, completedAchieve.ID, cardRewards);
      if (CollectionManager.Get() == null)
        return;
      CollectionManager.Get().AddCardRewards(cardRewards, false);
    }
  }

  public RewardData GetNextHeroLevelReward(
    TAG_CLASS classID,
    int currentHeroLevel,
    out int nextRewardLevel)
  {
    List<RewardData> rewardDataList = new List<RewardData>();
    List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedRewardAction.Type.HERO_LEVEL);
    FixedRewardActionDbfRecord rewardActionDbfRecord1 = (FixedRewardActionDbfRecord) null;
    nextRewardLevel = 0;
    int num = int.MaxValue;
    foreach (FixedRewardActionDbfRecord rewardActionDbfRecord2 in fixedActionRecords)
    {
      if ((TAG_CLASS) rewardActionDbfRecord2.ClassId == classID && SpecialEventManager.Get().IsEventActive(rewardActionDbfRecord2.ActiveEvent, false) && rewardActionDbfRecord2.HeroLevel > currentHeroLevel && rewardActionDbfRecord2.HeroLevel - currentHeroLevel < num)
      {
        num = rewardActionDbfRecord2.HeroLevel - currentHeroLevel;
        rewardActionDbfRecord1 = rewardActionDbfRecord2;
      }
    }
    if (rewardActionDbfRecord1 == null)
      return (RewardData) null;
    nextRewardLevel = rewardActionDbfRecord1.HeroLevel;
    foreach (FixedRewardMapDbfRecord record in GameUtils.GetFixedRewardMapRecordsForAction(rewardActionDbfRecord1.ID))
    {
      if (record.RewardRecord != null)
      {
        FixedReward.Reward fixedReward = record.GetFixedReward();
        if (fixedReward.FixedCardRewardData != null)
          rewardDataList.Add((RewardData) fixedReward.FixedCardRewardData);
      }
    }
    if (rewardDataList.Count == 0)
    {
      Debug.LogFormat("No subsequent reward found for Hero Class: {0} after Level: {1}. Check FIXED REWARD MAPS if you think there should be one", (object) classID.ToString(), (object) currentHeroLevel);
      return (RewardData) null;
    }
    if (rewardDataList.Count > 1)
      Debug.LogWarningFormat("More than one reward listed for the subsequent reward for Hero Class: {0} after Level: {1}. Check FIXED REWARD ACTIONS and FIXED REWARD MAPS to ensure there is only one reward per level", (object) classID.ToString(), (object) currentHeroLevel);
    return rewardDataList[0];
  }

  public RewardData GetNextTotalLevelReward(
    int currentTotalLevel,
    out int nextRewardLevel)
  {
    List<RewardData> rewardDataList = new List<RewardData>();
    List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedRewardAction.Type.TOTAL_HERO_LEVEL);
    FixedRewardActionDbfRecord rewardActionDbfRecord1 = (FixedRewardActionDbfRecord) null;
    nextRewardLevel = 0;
    int num = int.MaxValue;
    foreach (FixedRewardActionDbfRecord rewardActionDbfRecord2 in fixedActionRecords)
    {
      if (SpecialEventManager.Get().IsEventActive(rewardActionDbfRecord2.ActiveEvent, false) && rewardActionDbfRecord2.TotalHeroLevel > currentTotalLevel && rewardActionDbfRecord2.TotalHeroLevel - currentTotalLevel < num)
      {
        num = rewardActionDbfRecord2.TotalHeroLevel - currentTotalLevel;
        rewardActionDbfRecord1 = rewardActionDbfRecord2;
      }
    }
    if (rewardActionDbfRecord1 == null)
      return (RewardData) null;
    nextRewardLevel = rewardActionDbfRecord1.TotalHeroLevel;
    foreach (FixedRewardMapDbfRecord record in GameUtils.GetFixedRewardMapRecordsForAction(rewardActionDbfRecord1.ID))
    {
      if (record.RewardRecord != null)
      {
        FixedReward.Reward fixedReward = record.GetFixedReward();
        if (fixedReward.FixedCardRewardData != null)
          rewardDataList.Add((RewardData) fixedReward.FixedCardRewardData);
      }
    }
    if (rewardDataList.Count == 0)
    {
      Debug.LogFormat("No subsequent reward found for after Total Level: {0}. Check FIXED REWARD MAPS if you think there should be one", (object) currentTotalLevel);
      return (RewardData) null;
    }
    if (rewardDataList.Count > 1)
      Debug.LogErrorFormat("More than one reward listed for the subsequent reward after Total Level: {0}. Check FIXED REWARD ACTIONS and FIXED REWARD MAPS to ensure there is only one reward per level", (object) currentTotalLevel);
    return rewardDataList[0];
  }

  public delegate void DelOnAllFixedRewardsShown();

  public delegate void DelPositionNonToastReward(Reward reward);

  private class OnAllFixedRewardsShownCallbackInfo
  {
    public List<RewardMapIDToShow> rewardMapIDsToShow;
    public FixedRewardsMgr.DelOnAllFixedRewardsShown onAllRewardsShownCallback;
    public FixedRewardsMgr.DelPositionNonToastReward positionNonToastRewardCallback;
    public bool showingCheatRewards;
  }

  private enum ShowVisualOption
  {
    DO_NOT_SHOW,
    SHOW,
    FORCE_SHOW,
  }
}
