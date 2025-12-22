using Blizzard.T5.Jobs;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardPopups : IDisposable
{
  private List<Reward> m_rewards = new List<Reward>();
  private List<Reward> m_purchasedCardRewards = new List<Reward>();
  private List<Reward> m_genericRewards = new List<Reward>();
  private readonly Queue<NetCache.ProfileNotice> m_cardReplacementNotices = new Queue<NetCache.ProfileNotice>();
  private readonly HashSet<long> m_genericRewardChestNoticeIdsReady = new HashSet<long>();
  private readonly HashSet<long> m_deckRewardIds = new HashSet<long>();
  private int m_numRewardsToLoad;
  private readonly Dictionary<long, HashSet<int>> m_seenNotices = new Dictionary<long, HashSet<int>>();
  private readonly Queue<NetCache.ProfileNotice> m_dustRewardNotices = new Queue<NetCache.ProfileNotice>();
  private Action OnPopupShown;
  private Action OnPopupClosed;
  private Action<bool> SetIsShowing;
  private static HashSet<Assets.Achieve.RewardTiming> s_timingOutAndImmediate = new HashSet<Assets.Achieve.RewardTiming>()
  {
    Assets.Achieve.RewardTiming.OUT_OF_BAND,
    Assets.Achieve.RewardTiming.IMMEDIATE
  };
  private static HashSet<Assets.Achieve.RewardTiming> s_timingImmediate = new HashSet<Assets.Achieve.RewardTiming>()
  {
    Assets.Achieve.RewardTiming.IMMEDIATE
  };
  private PopupDisplayManager m_popupDisplayManager;

  public bool IsLoadingRewards => this.m_numRewardsToLoad > 0;

  private event Action<long> OnGenericRewardShown = originData => { };

  private static HashSet<Assets.Achieve.RewardTiming> CurrentRewardTimings => SceneMgr.Get().GetMode() != SceneMgr.Mode.LOGIN ? RewardPopups.s_timingImmediate : RewardPopups.s_timingOutAndImmediate;

  private bool IsShowingPromptInStore => UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || StoreManager.Get().IsPromptShowing;

  private PopupDisplayManagerBones ChestBones { get; set; }

  private PopupDisplayManagerBones QuestChestBones { get; set; }

  public RewardPopups(
    PopupDisplayManager popupDisplayManager,
    Action<bool> setIsShowing,
    Action onPopupShown,
    Action onPopupClosed)
  {
    this.m_popupDisplayManager = popupDisplayManager;
    this.SetIsShowing = setIsShowing;
    this.OnPopupShown = onPopupShown;
    this.OnPopupClosed = onPopupClosed;
    Processor.QueueJob("Load_Popup_Bones", this.LoadBones());
    AchievementManager.Get().GetRewardPresenter().OnRewardItemQueued += new Action<int>(this.m_popupDisplayManager.OnRewardPresenterScrollQueued);
    QuestManager.Get().GetRewardPresenter().OnRewardItemQueued += new Action<int>(this.m_popupDisplayManager.OnRewardPresenterScrollQueued);
    RewardTrackManager.Get().GetRewardPresenter().OnRewardItemQueued += new Action<int>(this.m_popupDisplayManager.OnRewardPresenterScrollQueued);
    this.StartupRegistration();
  }

  private void StartupRegistration()
  {
    HearthstoneApplication.Get().WillReset += new Action(this.WillReset);
    GenericRewardChestNoticeManager.Get().RegisterRewardsUpdatedListener(new GenericRewardChestNoticeManager.GenericRewardUpdatedCallback(this.OnGenericRewardUpdated));
    NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
    Network.Get().RegisterNetHandler((object) GetDeckContentsResponse.PacketID.ID, new Network.NetHandler(this.OnGetDeckContentsResponse));
  }

  public void Dispose()
  {
    HearthstoneApplication.Get().WillReset -= new Action(this.WillReset);
    GenericRewardChestNoticeManager.Get().RemoveRewardsUpdatedListener(new GenericRewardChestNoticeManager.GenericRewardUpdatedCallback(this.OnGenericRewardUpdated), (object) null);
    NetCache.Get().RemoveNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
    Network.Get().RemoveNetHandler((object) GetDeckContentsResponse.PacketID.ID, new Network.NetHandler(this.OnGetDeckContentsResponse));
    AchievementManager.Get().GetRewardPresenter().OnRewardItemQueued -= new Action<int>(this.m_popupDisplayManager.OnRewardPresenterScrollQueued);
    QuestManager.Get().GetRewardPresenter().OnRewardItemQueued -= new Action<int>(this.m_popupDisplayManager.OnRewardPresenterScrollQueued);
    RewardTrackManager.Get().GetRewardPresenter().OnRewardItemQueued -= new Action<int>(this.m_popupDisplayManager.OnRewardPresenterScrollQueued);
  }

  private void WillReset()
  {
    this.m_cardReplacementNotices.Clear();
    this.m_dustRewardNotices.Clear();
    this.ClearSeenNotices();
  }

  public IEnumerator<IAsyncJobResult> LoadBones()
  {
    LoadComponentFromResource<PopupDisplayManagerBones> loadBones = new LoadComponentFromResource<PopupDisplayManagerBones>("ServiceData/PopupDisplayManagerBones", LoadResourceFlags.AutoInstantiateOnLoad | LoadResourceFlags.FailOnError);
    yield return (IAsyncJobResult) loadBones;
    this.ChestBones = loadBones.LoadedComponent;
    loadBones = new LoadComponentFromResource<PopupDisplayManagerBones>("ServiceData/PopupDisplayManagerBonesForQuestChests", LoadResourceFlags.AutoInstantiateOnLoad | LoadResourceFlags.FailOnError);
    yield return (IAsyncJobResult) loadBones;
    this.QuestChestBones = loadBones.LoadedComponent;
  }

  public void RegisterGenericRewardShownListener(Action<long> callback)
  {
    if (callback == null)
      return;
    this.OnGenericRewardShown -= callback;
    this.OnGenericRewardShown += callback;
  }

  private void OnRewardObjectLoaded(Reward reward, object callbackData) => this.LoadReward(reward, ref this.m_rewards);

  private void OnPurchasedCardRewardObjectLoaded(Reward reward, object callbackData) => this.LoadReward(reward, ref this.m_purchasedCardRewards);

  private void OnGenericRewardObjectLoaded(Reward reward, object callbackData) => this.LoadReward(reward, ref this.m_genericRewards);

  private void OnRewardShown(object callbackData)
  {
    Reward reward = callbackData as Reward;
    if ((UnityEngine.Object) reward == (UnityEngine.Object) null)
      return;
    reward.RegisterClickListener(new Reward.OnClickedCallback(this.OnRewardClicked));
    reward.EnableClickCatcher(true);
  }

  private void ShowChestRewardsWhenReady_DoneCallback()
  {
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing == null)
      return;
    setIsShowing(false);
  }

  private void ShowNextLeaguePromotionReward_DoneCallback()
  {
    this.m_popupDisplayManager.ShowRankedIntro();
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing == null)
      return;
    setIsShowing(false);
  }

  private void OnRewardClicked(Reward reward, object userData)
  {
    reward.RemoveClickListener(new Reward.OnClickedCallback(this.OnRewardClicked));
    reward.Hide(true);
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing == null)
      return;
    setIsShowing(false);
  }

  private void OnGenericRewardUpdated(long rewardNoticeId, object userData)
  {
    this.m_genericRewardChestNoticeIdsReady.Add(rewardNoticeId);
    if (!this.m_popupDisplayManager.CanShowPopups())
      return;
    this.UpdateRewards(this.m_popupDisplayManager.AchievementPopups.CompletedAchieves);
  }

  private void OnCollectionManagerUpdatedNetCacheDecks()
  {
    foreach (long deckRewardId in this.m_deckRewardIds)
      Network.Get().RequestDeckContents(deckRewardId);
    CollectionManager.Get().RemoveOnNetCacheDecksProcessedListener(new Action(this.OnCollectionManagerUpdatedNetCacheDecks));
  }

  private void OnGetDeckContentsResponse()
  {
    List<PegasusUtil.DeckContents> deckContentsList = new List<PegasusUtil.DeckContents>();
    foreach (PegasusUtil.DeckContents deck in Network.Get().GetDeckContentsResponse().Decks)
    {
      if (this.m_deckRewardIds.Contains(deck.DeckId))
        deckContentsList.Add(deck);
    }
    if (deckContentsList.Count <= 0)
      return;
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    List<DeckInfo> listFromNetCache = NetCache.Get().GetDeckListFromNetCache();
    OfflineDataCache.CacheLocalAndOriginalDeckList(ref data, listFromNetCache, listFromNetCache);
    foreach (PegasusUtil.DeckContents deckContents in deckContentsList)
    {
      OfflineDataCache.CacheLocalAndOriginalDeckContents(ref data, deckContents, deckContents);
      this.m_deckRewardIds.Remove(deckContents.DeckId);
    }
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    if (!this.m_popupDisplayManager.CanShowPopups() || newNotices.Count <= 0)
      return;
    this.UpdateRewards(this.m_popupDisplayManager.AchievementPopups.CompletedAchieves);
    newNotices.ForEach((Action<NetCache.ProfileNotice>) (notice =>
    {
      if (notice.Origin == NetCache.ProfileNotice.NoticeOrigin.CARD_REPLACEMENT)
      {
        this.m_cardReplacementNotices.Enqueue(notice);
      }
      else
      {
        if (notice.Origin != NetCache.ProfileNotice.NoticeOrigin.HOF_COMPENSATION || notice.Type != NetCache.ProfileNotice.NoticeType.REWARD_DUST)
          return;
        this.m_dustRewardNotices.Enqueue(notice);
      }
    }));
  }

  public bool UpdateNoticesSeen(RewardData rewardData)
  {
    if (!rewardData.HasNotices())
      return true;
    bool flag = false;
    foreach (long noticeId in rewardData.GetNoticeIDs())
    {
      if (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.GENERIC_REWARD_CHEST_ACHIEVE && rewardData.RewardChestBagNum.HasValue)
      {
        if (!this.m_seenNotices.ContainsKey(noticeId))
          this.m_seenNotices.Add(noticeId, new HashSet<int>());
        if (this.m_seenNotices[noticeId].Add(rewardData.RewardChestBagNum.Value))
          flag = true;
      }
      else if (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_LUCKY_DRAW)
      {
        if (!this.m_seenNotices.ContainsKey(noticeId))
          this.m_seenNotices.Add(noticeId, new HashSet<int>());
        if (this.m_seenNotices[noticeId].Add((int) rewardData.OriginData))
          flag = true;
      }
      else if (!this.m_seenNotices.ContainsKey(noticeId))
      {
        this.m_seenNotices.Add(noticeId, new HashSet<int>());
        flag = true;
      }
    }
    return flag;
  }

  public void ClearSeenNotices() => this.m_seenNotices.Clear();

  private void LoadReward(Reward reward, ref List<Reward> allRewards)
  {
    reward.Hide();
    this.PositionReward(reward);
    allRewards.Add(reward);
    if (Reward.Type.CARD == reward.RewardType && reward is CardReward)
      (reward as CardReward).MakeActorsUnlit();
    LayerUtils.SetLayer(reward.gameObject, GameLayer.Default);
    --this.m_numRewardsToLoad;
    if (this.m_numRewardsToLoad > 0)
      return;
    RewardUtils.SortRewards(ref allRewards);
  }

  private void LoadRewards(List<RewardData> rewardsToLoad, Reward.DelOnRewardLoaded callback)
  {
    foreach (RewardData rewardData in rewardsToLoad)
    {
      if (this.UpdateNoticesSeen(rewardData))
      {
        if (ReturningPlayerMgr.Get().SuppressOldPopups && (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.TOURNEY || rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.TAVERN_BRAWL_REWARD || rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.LEAGUE_PROMOTION))
        {
          Log.ReturningPlayer.Print("Suppressing popup for Reward {0} due to being a Returning Player!", (object) rewardData);
          rewardData.AcknowledgeNotices();
        }
        else
        {
          ++this.m_numRewardsToLoad;
          rewardData.LoadRewardObject(callback);
        }
      }
    }
  }

  internal bool ShowRewardPopups(
    List<Achievement> completedAchieves,
    bool suppressRewardPopups,
    Func<bool> ShowNextRankedIntro,
    Func<bool> ShowNextCompletedQuest)
  {
    return this.ShowNextTavernBrawlReward() || this.ShowNextLeaguePromotionReward() || ShowNextRankedIntro() || this.ShowNextFreeDeckReward() || this.ShowNextSellableDeckReward() || this.ShowNextQuestChestReward() || this.ShowNextDuelsReward() || this.ShowNextProgressionAchievementReward() || this.ShowNextProgressionQuestReward() || this.ShowNextProgressionTrackReward() || this.ShowRewardTrackXpGains() || this.ShowEventEndedPopup() || this.ShowRewardTrackSeasonRoll() || (completedAchieves.Count > 0 || this.m_rewards.Count > 0 || this.m_purchasedCardRewards.Count > 0 || this.m_genericRewards.Count > 0) && (ShowNextCompletedQuest() || !suppressRewardPopups && this.ShowNextUnAckedReward() || this.ShowNextUnAckedGenericReward() || !suppressRewardPopups && this.ShowNextUnAckedPurchasedCardReward()) || !suppressRewardPopups && this.ShowFixedRewards(RewardPopups.CurrentRewardTimings) || this.IsLoadingRewards;
  }

  private bool ShowNextTavernBrawlReward()
  {
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber("PopupDisplayManager.UpdateTavernBrawlRewards") || SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB)
      return false;
    NetCache.NetCacheProfileNotices cacheProfileNotices = NetCache.Get() != null ? NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() : (NetCache.NetCacheProfileNotices) null;
    if (cacheProfileNotices == null || cacheProfileNotices.Notices == null)
      return false;
    NetCache.ProfileNoticeTavernBrawlRewards notice = (NetCache.ProfileNoticeTavernBrawlRewards) cacheProfileNotices.Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.TAVERN_BRAWL_REWARDS));
    if (notice == null)
      return false;
    Network network = Network.Get();
    if (PopupDisplayManager.ShouldDisableNotificationOnLogin())
      network?.AckNotice(notice.NoticeID);
    else if (ReturningPlayerMgr.Get() != null && ReturningPlayerMgr.Get().SuppressOldPopups)
    {
      if (network != null)
      {
        network.AckNotice(notice.NoticeID);
        Log.ReturningPlayer.Print("Suppressing popup for TavernBrawlRewardRewards due to being a Returning Player!");
      }
    }
    else
    {
      Action<bool> setIsShowing = this.SetIsShowing;
      if (setIsShowing != null)
        setIsShowing(true);
      Action onPopupShown = this.OnPopupShown;
      if (onPopupShown != null)
        onPopupShown();
      Transform rewardBoneForScene = this.GetChestRewardBoneForScene();
      if ((UnityEngine.Object) rewardBoneForScene == (UnityEngine.Object) null)
      {
        Log.All.PrintWarning("No bone set for reward chest in scene={0}!", (object) SceneMgr.Get().GetMode());
        return false;
      }
      List<RewardData> rewards = Network.ConvertRewardChest(notice.Chest).Rewards;
      RewardUtils.ShowTavernBrawlRewards(notice.Wins, rewards, rewardBoneForScene, new Action(this.ShowChestRewardsWhenReady_DoneCallback), true, notice);
    }
    return true;
  }

  private bool ShowNextLeaguePromotionReward()
  {
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber("PopupDisplayManager.ShowNextLeaguePromotionReward") || (UnityEngine.Object) LoadingScreen.Get() != (UnityEngine.Object) null && LoadingScreen.Get().IsTransitioning())
      return false;
    NetCache.NetCacheProfileNotices cacheProfileNotices = NetCache.Get() != null ? NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() : (NetCache.NetCacheProfileNotices) null;
    if (cacheProfileNotices == null || cacheProfileNotices.Notices == null)
      return false;
    NetCache.ProfileNoticeLeaguePromotionRewards promotionRewards = (NetCache.ProfileNoticeLeaguePromotionRewards) cacheProfileNotices.Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.LEAGUE_PROMOTION_REWARDS));
    if (promotionRewards == null)
      return false;
    Network network = Network.Get();
    if (PopupDisplayManager.ShouldDisableNotificationOnLogin())
      network?.AckNotice(promotionRewards.NoticeID);
    else if (ReturningPlayerMgr.Get() != null && ReturningPlayerMgr.Get().SuppressOldPopups)
    {
      if (network != null)
      {
        network.AckNotice(promotionRewards.NoticeID);
        Log.ReturningPlayer.Print("Suppressing popup for ProfileNoticeLeaguePromotionRewards due to being a Returning Player!");
      }
    }
    else
    {
      Action<bool> setIsShowing = this.SetIsShowing;
      if (setIsShowing != null)
        setIsShowing(true);
      Action onPopupShown = this.OnPopupShown;
      if (onPopupShown != null)
        onPopupShown();
      Transform rewardBoneForScene = this.GetChestRewardBoneForScene();
      if ((UnityEngine.Object) rewardBoneForScene == (UnityEngine.Object) null)
      {
        Log.All.PrintWarning("No bone set for reward chest in scene={0}!", (object) SceneMgr.Get().GetMode());
        return false;
      }
      List<RewardData> rewards = Network.ConvertRewardChest(promotionRewards.Chest).Rewards;
      RewardUtils.ShowLeaguePromotionRewards(promotionRewards.LeagueId, rewards, rewardBoneForScene, new Action(this.ShowNextLeaguePromotionReward_DoneCallback), true, promotionRewards.NoticeID);
    }
    return true;
  }

  private bool ShowNextFreeDeckReward()
  {
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber("PopupDisplayManager.ShowNextFreeDeckReward"))
      return false;
    NetCache.NetCacheProfileNotices cacheProfileNotices = NetCache.Get() != null ? NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() : (NetCache.NetCacheProfileNotices) null;
    if (cacheProfileNotices == null || cacheProfileNotices.Notices == null)
      return false;
    int index = 0;
    for (int count = cacheProfileNotices.Notices.Count; index < count; ++index)
    {
      NetCache.ProfileNotice notice = cacheProfileNotices.Notices[index];
      if (notice.Type == NetCache.ProfileNotice.NoticeType.DECK_GRANTED)
      {
        NetCache.ProfileNoticeDeckGranted deckRewardNotice = (NetCache.ProfileNoticeDeckGranted) notice;
        Action<bool> setIsShowing = this.SetIsShowing;
        if (setIsShowing != null)
          setIsShowing(true);
        this.UpdateOfflineDeckCache();
        DeckRewardData deckRewardData = RewardUtils.CreateDeckRewardData(deckRewardNotice.DeckDbiID, deckRewardNotice.ClassId, (string) null);
        DbfLocValue name = GameDbf.Deck.GetRecord((Predicate<DeckDbfRecord>) (deckRecord => deckRecord.ID == deckRewardNotice.DeckDbiID)).Name;
        this.ShowDeckRewardToast((NetCache.ProfileNotice) deckRewardNotice, deckRewardData, name, GameStrings.Get("GLUE_FREE_DECK_TITLE"), GameStrings.Get("GLUE_FREE_DECK_DESC"));
        Options.Get().SetLong(Option.LAST_CUSTOM_DECK_CHOSEN, deckRewardNotice.PlayerDeckID);
        return true;
      }
    }
    return false;
  }

  private bool ShowNextSellableDeckReward()
  {
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber("PopupDisplayManager.ShowNextSellableDeckReward") || StoreManager.Get().IsPromptShowing)
      return false;
    NetCache.NetCacheProfileNotices cacheProfileNotices = NetCache.Get() != null ? NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() : (NetCache.NetCacheProfileNotices) null;
    if (cacheProfileNotices == null || cacheProfileNotices.Notices == null)
      return false;
    int index = 0;
    for (int count = cacheProfileNotices.Notices.Count; index < count; ++index)
    {
      NetCache.ProfileNotice notice = cacheProfileNotices.Notices[index];
      if (notice.Type == NetCache.ProfileNotice.NoticeType.SELLABLE_DECK_GRANTED)
        return this.ShowSellablePopup((NetCache.ProfileNoticeSellableDeckGranted) notice);
    }
    return false;
  }

  private bool ShowSellablePopup(
    NetCache.ProfileNoticeSellableDeckGranted deckRewardNotice)
  {
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    this.UpdateOfflineDeckCache();
    SellableDeckDbfRecord sellableDeckDbfRecord;
    if (!RewardUtils.TryGetSellableDeck(deckRewardNotice.SellableDeckID, out sellableDeckDbfRecord))
      return false;
    DeckTemplateDbfRecord deckTemplateRecord = sellableDeckDbfRecord.DeckTemplateRecord;
    int num = 1;
    int count = deckTemplateRecord.DeckRecord.Cards.Count;
    string displayTitle = GameStrings.Get("GLUE_SELLABLE_DECK_TITLE");
    string displayDescription = GameStrings.Format("GLUE_SELLABLE_DECK_DESC", (object) count, (object) num);
    DbfLocValue deckName = deckRewardNotice.Premium == TAG_PREMIUM.GOLDEN ? sellableDeckDbfRecord.GoldenName : deckTemplateRecord.DeckRecord.Name;
    DeckRewardData deckRewardData = RewardUtils.CreateDeckRewardData(deckTemplateRecord.DeckId, deckTemplateRecord.ClassId, deckName?.GetString() ?? string.Empty);
    this.ShowDeckRewardToast((NetCache.ProfileNotice) deckRewardNotice, deckRewardData, deckName, displayTitle, displayDescription);
    RewardUtils.SetNewRewardedDeck(deckRewardNotice.PlayerDeckID);
    return true;
  }

  private bool ShowNextQuestChestReward()
  {
    NetCache.NetCacheProfileNotices notices = NetCache.Get() != null ? NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() : (NetCache.NetCacheProfileNotices) null;
    if (notices == null || notices.Notices == null)
      return false;
    int index = 0;
    for (int count = notices.Notices.Count; index < count; ++index)
    {
      NetCache.ProfileNotice notice = notices.Notices[index];
      if (notice.Type == NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST && notice.Origin == NetCache.ProfileNotice.NoticeOrigin.GENERIC_REWARD_CHEST_ACHIEVE)
        return this.ShowRewardChest(notices, (NetCache.ProfileNoticeGenericRewardChest) notice);
    }
    return false;
  }

  private bool ShowRewardChest(
    NetCache.NetCacheProfileNotices notices,
    NetCache.ProfileNoticeGenericRewardChest rewardChestNotice)
  {
    Network net = Network.Get();
    if (PopupDisplayManager.ShouldDisableNotificationOnLogin() && net != null)
    {
      net.AckNotice(rewardChestNotice.NoticeID);
      notices.Notices.Remove((NetCache.ProfileNotice) rewardChestNotice);
      return false;
    }
    if (AchieveManager.Get() == null)
      return false;
    Achievement achievement = AchieveManager.Get().GetAchievement((int) rewardChestNotice.OriginData);
    if (!achievement.HasRewardChestVisuals)
    {
      Log.Achievements.PrintError("Achieve id = {0} not properly set up for chest visuals", (object) (int) rewardChestNotice.OriginData);
      return false;
    }
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    List<RewardData> rewards = Network.ConvertRewardChest(rewardChestNotice.RewardChest).Rewards;
    RewardUtils.ShowQuestChestReward(achievement.Name, achievement.Description, rewards, this.GetChestRewardBoneForScene(this.QuestChestBones), (Action) (() =>
    {
      if (net == null)
        return;
      net.AckNotice(rewardChestNotice.NoticeID);
      Action onPopupClosed = this.OnPopupClosed;
      if (onPopupClosed == null)
        return;
      onPopupClosed();
    }), true, achievement.ID, achievement.ChestVisualPrefabPath);
    return true;
  }

  private bool ShowNextDuelsReward()
  {
    if (DuelsConfig.Get().IsReadyToShowRewards())
    {
      NetCache.ProfileNoticeGenericRewardChest rewardNoticeToShow = DuelsConfig.Get().GetRewardNoticeToShow();
      if (rewardNoticeToShow != null)
      {
        Action<bool> setIsShowing = this.SetIsShowing;
        if (setIsShowing != null)
          setIsShowing(true);
        DuelsConfig.Get().ShowRewardsForNotice(rewardNoticeToShow, this.OnPopupClosed, this.GetChestRewardBoneForScene());
        return true;
      }
    }
    return false;
  }

  private bool ShowNextProgressionAchievementReward()
  {
    AchievementManager achievementManager = AchievementManager.Get();
    if (achievementManager == null || !achievementManager.HasReward() || !achievementManager.ShowNextReward(this.OnPopupClosed))
      return false;
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    return true;
  }

  private bool ShowNextProgressionQuestReward()
  {
    QuestManager questManager = QuestManager.Get();
    if (questManager == null || !questManager.HasReward() || !questManager.ShowNextReward(this.OnPopupClosed))
      return false;
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    return true;
  }

  private bool ShowNextProgressionTrackReward()
  {
    RewardTrackManager rewardTrackManager = RewardTrackManager.Get();
    if (rewardTrackManager == null || !rewardTrackManager.HasReward() || !rewardTrackManager.ShowNextReward(this.OnPopupClosed))
      return false;
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    return true;
  }

  private bool ShowRewardTrackXpGains()
  {
    RewardXpNotificationManager notificationManager = RewardXpNotificationManager.Get();
    if (notificationManager == null || !notificationManager.HasXpGainsToShow)
      return false;
    notificationManager.ShowXpNotificationsImmediate((Action) (() =>
    {
      Action<bool> setIsShowing = this.SetIsShowing;
      if (setIsShowing == null)
        return;
      setIsShowing(false);
    }));
    Action<bool> setIsShowing1 = this.SetIsShowing;
    if (setIsShowing1 != null)
      setIsShowing1(true);
    return true;
  }

  private bool ShowEventEndedPopup()
  {
    RewardTrackManager rewardTrackManager = RewardTrackManager.Get();
    if (rewardTrackManager == null || !rewardTrackManager.ShowEventEndedPopup(this.OnPopupClosed))
      return false;
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    return true;
  }

  private bool ShowRewardTrackSeasonRoll()
  {
    RewardTrackManager rewardTrackManager = RewardTrackManager.Get();
    if (rewardTrackManager == null || !rewardTrackManager.ShowUnclaimedTrackRewardsPopup(this.OnPopupClosed))
      return false;
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    return true;
  }

  public bool HasUnAckedRewards() => this.m_rewards.FindAll((Predicate<Reward>) (reward => RewardUtils.IsRequiredDataLoadedToShowReward(reward) && RewardUtils.IsRequiredContextForReward(reward))).Count != 0;

  private bool ShowNextUnAckedReward()
  {
    if (this.IsShowingPromptInStore)
      return false;
    Reward reward1 = (Reward) null;
    int index = 0;
    for (int count = this.m_rewards.Count; index < count; ++index)
    {
      Reward reward2 = this.m_rewards[index];
      if (RewardUtils.IsRequiredDataLoadedToShowReward(reward2) && RewardUtils.IsRequiredContextForReward(reward2))
      {
        reward1 = reward2;
        this.m_rewards.RemoveAt(index);
        break;
      }
    }
    if ((UnityEngine.Object) reward1 == (UnityEngine.Object) null)
      return false;
    RewardData data = reward1.Data;
    UserAttentionBlocker blockerForReward = RewardUtils.GetUserAttentionBlockerForReward(data.Origin, data.OriginData);
    if (data.ShowQuestToast)
    {
      Action<bool> setIsShowing1 = this.SetIsShowing;
      if (setIsShowing1 != null)
        setIsShowing1(true);
      this.OnPopupShown();
      string title;
      string description;
      RewardUtils.GetTitleAndDescriptionFromReward(reward1, out title, out description);
      QuestToast.ShowGenericRewardQuestToast(blockerForReward, (QuestToast.DelOnCloseQuestToast) (userData =>
      {
        Action<bool> setIsShowing2 = this.SetIsShowing;
        if (setIsShowing2 == null)
          return;
        setIsShowing2(false);
      }), data, title, description);
    }
    else if (RewardUtils.ShowReward(blockerForReward, reward1, false, this.GetRewardPunchScale(), this.GetRewardScale(), new AnimationUtil.DelOnShownWithPunch(this.OnRewardShown), (object) reward1))
    {
      Action<bool> setIsShowing = this.SetIsShowing;
      if (setIsShowing != null)
        setIsShowing(true);
      Action onPopupShown = this.OnPopupShown;
      if (onPopupShown != null)
        onPopupShown();
    }
    return true;
  }

  private bool ShowNextUnAckedGenericReward()
  {
    if (this.m_genericRewards.Count == 0 || this.IsShowingPromptInStore)
      return false;
    Action<bool> setIsShowing1 = this.SetIsShowing;
    if (setIsShowing1 != null)
      setIsShowing1(true);
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    Reward genericReward = this.m_genericRewards[0];
    this.m_genericRewards.RemoveAt(0);
    int blockerForReward = (int) RewardUtils.GetUserAttentionBlockerForReward(genericReward.Data.Origin, genericReward.Data.OriginData);
    string title;
    string description1;
    RewardUtils.GetTitleAndDescriptionFromReward(genericReward, out title, out description1);
    QuestToast.DelOnCloseQuestToast onClosedCallback = (QuestToast.DelOnCloseQuestToast) (userData =>
    {
      Action<bool> setIsShowing2 = this.SetIsShowing;
      if (setIsShowing2 == null)
        return;
      setIsShowing2(false);
    });
    RewardData data = genericReward.Data;
    string name = title;
    string description2 = description1;
    QuestToast.ShowGenericRewardQuestToast((UserAttentionBlocker) blockerForReward, onClosedCallback, data, name, description2);
    this.OnGenericRewardShown(genericReward.Data.OriginData);
    return true;
  }

  private bool ShowNextUnAckedPurchasedCardReward()
  {
    if (this.m_purchasedCardRewards.Count == 0 || this.IsShowingPromptInStore)
      return false;
    if (QuestToast.IsQuestActive())
      QuestToast.GetCurrentToast().CloseQuestToast();
    Reward purchasedCardReward = this.m_purchasedCardRewards[0];
    UserAttentionBlocker blockerForReward = RewardUtils.GetUserAttentionBlockerForReward(purchasedCardReward.Data.Origin, purchasedCardReward.Data.OriginData);
    if (!UserAttentionManager.CanShowAttentionGrabber(blockerForReward, nameof (ShowNextUnAckedPurchasedCardReward)))
      return false;
    this.m_purchasedCardRewards.RemoveAt(0);
    Action<bool> setIsShowing1 = this.SetIsShowing;
    if (setIsShowing1 != null)
      setIsShowing1(true);
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    string title;
    string description;
    RewardUtils.GetTitleAndDescriptionFromReward(purchasedCardReward, out title, out description);
    QuestToast.ShowQuestToastPopup(blockerForReward, (QuestToast.DelOnCloseQuestToast) (userData =>
    {
      Action<bool> setIsShowing2 = this.SetIsShowing;
      if (setIsShowing2 == null)
        return;
      setIsShowing2(false);
    }), (object) null, purchasedCardReward.Data, title, description, false, false, (Achievement) null);
    return true;
  }

  private bool ShowFixedRewards(HashSet<Assets.Achieve.RewardTiming> rewardTimings)
  {
    FixedRewardsMgr fixedRewardsMgr = FixedRewardsMgr.Get();
    if (fixedRewardsMgr == null || !fixedRewardsMgr.HasRewardsToShow((IEnumerable<Assets.Achieve.RewardTiming>) rewardTimings))
      return false;
    Log.Achievements.Print("PopupDisplayManager: Showing Fixed Rewards");
    if (!fixedRewardsMgr.ShowFixedRewards(UserAttentionBlocker.NONE, rewardTimings, (FixedRewardsMgr.DelOnAllFixedRewardsShown) (() =>
    {
      Action<bool> setIsShowing = this.SetIsShowing;
      if (setIsShowing == null)
        return;
      setIsShowing(false);
    }), (FixedRewardsMgr.DelPositionNonToastReward) null))
    {
      Action<bool> setIsShowing = this.SetIsShowing;
      if (setIsShowing != null)
        setIsShowing(false);
      return false;
    }
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    Action<bool> setIsShowing1 = this.SetIsShowing;
    if (setIsShowing1 != null)
      setIsShowing1(true);
    return true;
  }

  private void ShowDeckRewardToast(
    NetCache.ProfileNotice profileNotice,
    DeckRewardData rewardData,
    DbfLocValue deckName,
    string displayTitle,
    string displayDescription)
  {
    QuestToast.ShowFixedRewardQuestToast(UserAttentionBlocker.NONE, (QuestToast.DelOnCloseQuestToast) (userData =>
    {
      if (Network.Get() == null)
        return;
      Action<bool> setIsShowing = this.SetIsShowing;
      if (setIsShowing != null)
        setIsShowing(false);
      Network.Get().AckNotice(profileNotice.NoticeID);
      Network.Get().RenameDeck(profileNotice.OriginData, (string) deckName);
    }), (RewardData) rewardData, displayTitle, displayDescription);
    this.m_deckRewardIds.Add(profileNotice.OriginData);
  }

  private void UpdateOfflineDeckCache()
  {
    if (NetCache.Get() == null)
      return;
    CollectionManager.Get().AddOnNetCacheDecksProcessedListener(new Action(this.OnCollectionManagerUpdatedNetCacheDecks));
    NetCache.Get().RefreshNetObject<NetCache.NetCacheDecks>();
    NetCache.Get().RefreshNetObject<NetCache.NetCacheHeroLevels>();
  }

  private Transform GetChestRewardBoneForScene(PopupDisplayManagerBones boneSet = null)
  {
    PopupDisplayManagerBones displayManagerBones = (UnityEngine.Object) boneSet != (UnityEngine.Object) null ? boneSet : this.ChestBones;
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.LOGIN:
      case SceneMgr.Mode.HUB:
        return displayManagerBones.m_rewardChestBone_Box;
      case SceneMgr.Mode.PACKOPENING:
      case SceneMgr.Mode.LETTUCE_VILLAGE:
      case SceneMgr.Mode.LETTUCE_MAP:
      case SceneMgr.Mode.LETTUCE_PLAY:
        return displayManagerBones.m_rewardChestBone_PackOpening;
      case SceneMgr.Mode.TOURNAMENT:
        return displayManagerBones.m_rewardChestBone_PlayMode;
      case SceneMgr.Mode.PVP_DUNGEON_RUN:
        return displayManagerBones.m_rewardChestBone_DungeonCrawl;
      default:
        return (Transform) null;
    }
  }

  public void ShowRewardsForAdventureUnlocks(
    List<AdventureHeroPowerDbfRecord> unlockedHeroPowers,
    List<AdventureDeckDbfRecord> unlockedDecks,
    List<AdventureLoadoutTreasuresDbfRecord> unlockedLoadoutTreasures,
    List<AdventureLoadoutTreasuresDbfRecord> upgradedLoadoutTreasures,
    Action callback)
  {
    List<RewardData> rewardsToLoad = new List<RewardData>();
    if (unlockedHeroPowers != null)
    {
      foreach (AdventureHeroPowerDbfRecord unlockedHeroPower in unlockedHeroPowers)
        rewardsToLoad.Add((RewardData) new AdventureHeroPowerRewardData(unlockedHeroPower));
    }
    if (unlockedDecks != null)
    {
      foreach (AdventureDeckDbfRecord unlockedDeck in unlockedDecks)
        rewardsToLoad.Add((RewardData) new AdventureDeckRewardData(unlockedDeck));
    }
    if (unlockedLoadoutTreasures != null)
    {
      foreach (AdventureLoadoutTreasuresDbfRecord unlockedLoadoutTreasure in unlockedLoadoutTreasures)
        rewardsToLoad.Add((RewardData) new AdventureLoadoutTreasureRewardData(unlockedLoadoutTreasure, false));
    }
    if (upgradedLoadoutTreasures != null)
    {
      foreach (AdventureLoadoutTreasuresDbfRecord upgradedLoadoutTreasure in upgradedLoadoutTreasures)
        rewardsToLoad.Add((RewardData) new AdventureLoadoutTreasureRewardData(upgradedLoadoutTreasure, true));
    }
    this.LoadRewards(rewardsToLoad, new Reward.DelOnRewardLoaded(this.OnRewardObjectLoaded));
    if (callback != null)
      this.m_popupDisplayManager.RegisterAllPopupsShownListener(callback);
    this.m_popupDisplayManager.ReadyToShowPopups();
  }

  public bool DisplayRewardObject(Reward reward, AnimationUtil.DelOnShownWithPunch onShowCallback) => this.DisplayRewardObject(reward, onShowCallback, (object) reward);

  public bool DisplayRewardObject(
    Reward reward,
    AnimationUtil.DelOnShownWithPunch onShowCallback,
    object callbackData)
  {
    reward.Hide();
    this.PositionReward(reward);
    LayerUtils.SetLayer(reward.gameObject, GameLayer.IgnoreFullScreenEffects);
    return RewardUtils.ShowReward(UserAttentionBlocker.NONE, reward, false, this.GetRewardPunchScale(), this.GetRewardScale(), onShowCallback, callbackData);
  }

  public void DisplayLoadedRewardObject(Reward reward, object callbackData)
  {
    if (!this.DisplayRewardObject(reward, new AnimationUtil.DelOnShownWithPunch(this.OnRewardShown)))
      return;
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing == null)
      return;
    setIsShowing(true);
  }

  public NetCache.ProfileNoticeMercenariesRewards GetNextNonAutoRetireRewardMercenariesRewardToShow() => this.GetNextMercenariesRewardToShow((Predicate<NetCache.ProfileNoticeMercenariesRewards>) (notice => notice.RewardType != PegasusShared.ProfileNoticeMercenariesRewards.RewardType.REWARD_TYPE_PVE_AUTO_RETIRE));

  public NetCache.ProfileNoticeMercenariesRewards GetNextBonusMercenariesRewardToShow() => this.GetNextMercenariesRewardToShow((Predicate<NetCache.ProfileNoticeMercenariesRewards>) (notice => notice.RewardType == PegasusShared.ProfileNoticeMercenariesRewards.RewardType.REWARD_TYPE_PVE_BONUS_CHEST));

  public NetCache.ProfileNoticeMercenariesRewards GetNextMercenariesRewardToShow(
    Predicate<NetCache.ProfileNoticeMercenariesRewards> filter = null)
  {
    return (NetCache.ProfileNoticeMercenariesRewards) NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>().Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.MERCENARIES_REWARDS && obj.Origin == NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_MERCENARIES && (filter == null || obj is NetCache.ProfileNoticeMercenariesRewards mercenariesRewards && filter(mercenariesRewards))));
  }

  public bool HasNonAutoRetireMercenariesRewardsToShow() => this.HasMercenariesRewardsToShow((Predicate<NetCache.ProfileNoticeMercenariesRewards>) (notice => notice.RewardType != PegasusShared.ProfileNoticeMercenariesRewards.RewardType.REWARD_TYPE_PVE_AUTO_RETIRE));

  public bool HasMercenariesRewardsToShow(
    Predicate<NetCache.ProfileNoticeMercenariesRewards> filter = null)
  {
    return this.GetNextMercenariesRewardToShow(filter) != null;
  }

  public bool ShowMercenariesRewards(
    bool autoOpenChest,
    NetCache.ProfileNoticeMercenariesRewards rewardNotice,
    NetCache.ProfileNoticeMercenariesRewards bonusRewardNotice = null,
    Action doneCallback = null)
  {
    if (rewardNotice == null)
      return false;
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    switch (rewardNotice.RewardType)
    {
      case PegasusShared.ProfileNoticeMercenariesRewards.RewardType.REWARD_TYPE_PVE_CONSOLATION:
        RewardListDataModel rewardItemDataModel1 = RewardFactory.CreateRewardItemDataModel(rewardNotice.Chest);
        foreach (RewardItemDataModel rewardItemDataModel2 in rewardItemDataModel1.Items)
        {
          if (rewardItemDataModel2.ItemType == RewardItemType.MERCENARY_COIN)
            rewardItemDataModel2.MercenaryCoin.NameActive = true;
        }
        RewardUtils.ShowConsolationMercenariesReward(rewardNotice.RewardType, rewardItemDataModel1, this.GetChestRewardBoneForScene(this.ChestBones), (Action) (() =>
        {
          Network.Get().AckNotice(rewardNotice.NoticeID);
          Action action = doneCallback;
          if (action != null)
            action();
          this.OnPopupClosed();
        }));
        break;
      case PegasusShared.ProfileNoticeMercenariesRewards.RewardType.REWARD_TYPE_PVE_AUTO_RETIRE:
        RewardListDataModel rewardItemDataModel3 = RewardFactory.CreateRewardItemDataModel(rewardNotice.Chest);
        foreach (RewardItemDataModel rewardItemDataModel4 in rewardItemDataModel3.Items)
        {
          if (rewardItemDataModel4.ItemType == RewardItemType.MERCENARY_COIN)
            rewardItemDataModel4.MercenaryCoin.NameActive = true;
        }
        RewardUtils.ShowAutoRetireMercenariesReward(rewardNotice.RewardType, rewardItemDataModel3, this.GetChestRewardBoneForScene(this.ChestBones), (Action) (() =>
        {
          Network.Get().AckNotice(rewardNotice.NoticeID);
          Action action = doneCallback;
          if (action != null)
            action();
          this.OnPopupClosed();
        }));
        break;
      default:
        List<RewardData> rewards = Network.ConvertRewardChest(rewardNotice.Chest).Rewards;
        List<RewardData> rewardDataList = (List<RewardData>) null;
        if (bonusRewardNotice != null)
          rewardDataList = Network.ConvertRewardChest(bonusRewardNotice.Chest).Rewards;
        List<RewardData> bonusRewards = rewardDataList;
        Transform rewardBoneForScene = this.GetChestRewardBoneForScene(this.ChestBones);
        Action doneCallback1 = (Action) (() =>
        {
          Network.Get().AckNotice(rewardNotice.NoticeID);
          if (bonusRewardNotice != null)
            Network.Get().AckNotice(bonusRewardNotice.NoticeID);
          Action action = doneCallback;
          if (action != null)
            action();
          this.OnPopupClosed();
        });
        int num = autoOpenChest ? 1 : 0;
        int noticeId = (int) rewardNotice.NoticeID;
        RewardUtils.ShowMercenariesChestReward(rewards, bonusRewards, rewardBoneForScene, doneCallback1, num != 0, true, noticeId);
        break;
    }
    return true;
  }

  private NetCache.ProfileNoticeMercenariesMercenaryFullyUpgraded GetNextMercenaryFullUpgradedToShow() => (NetCache.ProfileNoticeMercenariesMercenaryFullyUpgraded) NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>().Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.MERCENARIES_MERC_FULL_UPGRADE));

  public bool ShowMercenariesFullyUpgraded(Action doneCallback = null)
  {
    NetCache.ProfileNoticeMercenariesMercenaryFullyUpgraded upgradeNotice = this.GetNextMercenaryFullUpgradedToShow();
    if (upgradeNotice == null)
      return false;
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    RewardUtils.ShowMercenaryFullyUpgraded(RewardFactory.CreateFullyUpgradedMercenaryDataModel(upgradeNotice.MercenaryId), this.GetChestRewardBoneForScene(this.ChestBones), (Action) (() =>
    {
      Network.Get().AckNotice(upgradeNotice.NoticeID);
      CollectionManager.Get().GetMercenary((long) upgradeNotice.MercenaryId).m_isFullyUpgraded = true;
      Action action = doneCallback;
      if (action != null)
        action();
      this.OnPopupClosed();
    }));
    return true;
  }

  public NetCache.ProfileNoticeMercenariesSeasonRewards GetNextMercenariesSeasonRewardsNotice()
  {
    foreach (NetCache.ProfileNotice notice in NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>().Notices)
    {
      if (notice.Type == NetCache.ProfileNotice.NoticeType.MERCENARIES_SEASON_REWARDS)
        return notice as NetCache.ProfileNoticeMercenariesSeasonRewards;
    }
    return (NetCache.ProfileNoticeMercenariesSeasonRewards) null;
  }

  public bool ShowNextMercenariesSeasonRewards(Action doneCallback = null)
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.LOGIN)
      return false;
    NetCache.ProfileNoticeMercenariesSeasonRewards seasonRewardsNotice = this.GetNextMercenariesSeasonRewardsNotice();
    if (seasonRewardsNotice == null)
      return false;
    DialogManager.Get().ShowMercenariesSeasonRewardsDialog(seasonRewardsNotice, doneCallback);
    return true;
  }

  private NetCache.ProfileNoticeMercenariesZoneUnlock GetNextMercenariesZoneUnlockToShow() => (NetCache.ProfileNoticeMercenariesZoneUnlock) NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>().Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.MERCENARIES_ZONE_UNLOCK));

  public bool ShowMercenariesZoneUnlockPopup(Action onPopupCompleteCallback = null)
  {
    NetCache.ProfileNoticeMercenariesZoneUnlock zoneNotice = this.GetNextMercenariesZoneUnlockToShow();
    if (zoneNotice != null)
    {
      if (GameDbf.LettuceBountySet.GetRecord(zoneNotice.ZoneId) != null)
      {
        Action<bool> setIsShowing1 = this.SetIsShowing;
        if (setIsShowing1 != null)
          setIsShowing1(true);
        Action onPopupShown = this.OnPopupShown;
        if (onPopupShown != null)
          onPopupShown();
        DialogManager.Get().ShowMercenariesZoneUnlockDialog(zoneNotice.ZoneId, (Action) (() =>
        {
          Network.Get().AckNotice(zoneNotice.NoticeID);
          Action<bool> setIsShowing2 = this.SetIsShowing;
          if (setIsShowing2 != null)
            setIsShowing2(false);
          Action action = onPopupCompleteCallback;
          if (action != null)
            action();
          Action onPopupClosed = this.OnPopupClosed;
          if (onPopupClosed == null)
            return;
          onPopupClosed();
        }));
        return true;
      }
      Debug.LogError((object) ("ShowMercenariesZoneUnlockPopup attempted to show invalid zone unlock with id: " + (object) zoneNotice.ZoneId));
      Network.Get().AckNotice(zoneNotice.NoticeID);
    }
    return false;
  }

  public NetCache.ProfileNoticeMercenariesAbilityUnlock GetNextMercenariesAbilityUnlockReward() => (NetCache.ProfileNoticeMercenariesAbilityUnlock) NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>().Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.MERCENARIES_ABILITY_UNLOCK));

  public bool ShowNextMercenariesAbilityUnlockReward(
    NetCache.ProfileNoticeMercenariesAbilityUnlock rewardNotice = null,
    Action doneCallback = null)
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.LETTUCE_VILLAGE)
    {
      Action action = doneCallback;
      if (action != null)
        action();
      return false;
    }
    if (rewardNotice == null)
      rewardNotice = this.GetNextMercenariesAbilityUnlockReward();
    if (rewardNotice == null)
    {
      Action action = doneCallback;
      if (action != null)
        action();
      return false;
    }
    Action<bool> setIsShowing1 = this.SetIsShowing;
    if (setIsShowing1 != null)
      setIsShowing1(true);
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    RewardUtils.LoadAndDisplayRewards(RewardUtils.GetRewards(new List<NetCache.ProfileNotice>()
    {
      (NetCache.ProfileNotice) rewardNotice
    }), (Action) (() =>
    {
      Network.Get().AckNotice(rewardNotice.NoticeID);
      Action<bool> setIsShowing2 = this.SetIsShowing;
      if (setIsShowing2 != null)
        setIsShowing2(false);
      Action action = doneCallback;
      if (action != null)
        action();
      Action onPopupClosed = this.OnPopupClosed;
      if (onPopupClosed != null)
        onPopupClosed();
      foreach (Reward reward in this.m_rewards)
      {
        long num = 0;
        List<long> noticeIds = reward.Data.GetNoticeIDs();
        if (noticeIds != null && noticeIds.Count > 0)
          num = noticeIds[0];
        if (num == rewardNotice.NoticeID)
        {
          this.m_rewards.Remove(reward);
          break;
        }
      }
    }));
    return true;
  }

  public void UpdateRewards(List<Achievement> completedAchieves)
  {
    NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
    List<RewardData> rewardsToShow = new List<RewardData>();
    List<RewardData> genericRewardChestsToShow = new List<RewardData>();
    List<RewardData> purchasedCardRewardsToShow = new List<RewardData>();
    if (netObject != null)
    {
      AchieveManager.Get();
      List<RewardData> rewards = RewardUtils.GetRewards(netObject.Notices.Where<NetCache.ProfileNotice>((Func<NetCache.ProfileNotice, bool>) (n =>
      {
        if (n.Type == NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST && n.Origin == NetCache.ProfileNotice.NoticeOrigin.GENERIC_REWARD_CHEST_ACHIEVE)
        {
          Achievement achievement = AchieveManager.Get().GetAchievement((int) n.OriginData);
          if (achievement != null && achievement.HasRewardChestVisuals)
            return false;
        }
        if (n.Type == NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST && n.Origin == NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_DUELS)
          return false;
        return n.Type != NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST || this.m_genericRewardChestNoticeIdsReady.Any<long>((Func<long, bool>) (r => n.NoticeID == r));
      })).ToList<NetCache.ProfileNotice>());
      HashSet<Assets.Achieve.RewardTiming> rewardTimings = new HashSet<Assets.Achieve.RewardTiming>();
      foreach (Assets.Achieve.RewardTiming rewardTiming in Enum.GetValues(typeof (Assets.Achieve.RewardTiming)))
        rewardTimings.Add(rewardTiming);
      RewardUtils.GetViewableRewards(rewards, rewardTimings, out rewardsToShow, out genericRewardChestsToShow, ref purchasedCardRewardsToShow, ref completedAchieves);
    }
    if (ReturningPlayerMgr.Get().SuppressOldPopups)
    {
      List<Achievement> achievementList = new List<Achievement>();
      foreach (Achievement completedAchieve in completedAchieves)
      {
        if (completedAchieve.ShowToReturningPlayer == Assets.Achieve.ShowToReturningPlayer.SUPPRESSED)
        {
          Log.ReturningPlayer.Print("Suppressing popup for Achievement {0} due to being a Returning Player!", (object) completedAchieve);
          completedAchieve.AckCurrentProgressAndRewardNotices();
        }
        else
          achievementList.Add(completedAchieve);
      }
      completedAchieves = achievementList;
      genericRewardChestsToShow.RemoveAll((Predicate<RewardData>) (rewardData =>
      {
        if (!rewardData.RewardChestAssetId.HasValue)
        {
          AckNotices(rewardData);
          return true;
        }
        RewardChestDbfRecord record = GameDbf.RewardChest.GetRecord(rewardData.RewardChestAssetId.Value);
        if (record != null && record.ShowToReturningPlayer)
          return false;
        AckNotices(rewardData);
        return true;
      }));
    }
    if (!PopupDisplayManager.ShouldDisableNotificationOnLogin())
    {
      this.LoadRewards(rewardsToShow, new Reward.DelOnRewardLoaded(this.OnRewardObjectLoaded));
      this.LoadRewards(purchasedCardRewardsToShow, new Reward.DelOnRewardLoaded(this.OnPurchasedCardRewardObjectLoaded));
      this.LoadRewards(genericRewardChestsToShow, new Reward.DelOnRewardLoaded(this.OnGenericRewardObjectLoaded));
    }
    Log.Achievements.Print("PopupDisplayManager: adding {0} rewards to load total={1}", (object) rewardsToShow.Count, (object) this.m_numRewardsToLoad);

    static void AckNotices(RewardData rewardData)
    {
      foreach (long noticeId in rewardData.GetNoticeIDs())
        Network.Get().AckNotice(noticeId);
    }
  }

  private void PositionReward(Reward reward)
  {
    Transform transform = reward.transform;
    transform.parent = this.ChestBones.transform;
    transform.localRotation = Quaternion.identity;
    transform.localPosition = this.GetRewardLocalPos(reward);
  }

  public Vector3 GetRewardLocalPos(Reward reward = null)
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.GAMEPLAY:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(-7.72f, 8.371922f, -3.883112f),
          Phone = new Vector3(-7.72f, 7.3f, -3.94f)
        };
      case SceneMgr.Mode.LETTUCE_VILLAGE:
        if ((UnityEngine.Object) reward != (UnityEngine.Object) null && reward.RewardType == Reward.Type.MERCENARY_EQUIPMENT)
          return this.ChestBones.m_rewardChestBone_EquipOpening.localPosition;
        break;
      case SceneMgr.Mode.LETTUCE_MAP:
      case SceneMgr.Mode.LETTUCE_PLAY:
        return new Vector3(0.1438589f, -7f, 10f);
    }
    return new Vector3(0.1438589f, 31.27692f, 12.97332f);
  }

  public Vector3 GetRewardScale()
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.STARTUP:
      case SceneMgr.Mode.LOGIN:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(15f, 15f, 15f),
          Phone = new Vector3(14f, 14f, 14f)
        };
      case SceneMgr.Mode.GAMEPLAY:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = Vector3.one,
          Phone = new Vector3(0.8f, 0.8f, 0.8f)
        };
      case SceneMgr.Mode.PACKOPENING:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(8f, 8f, 8f),
          Phone = new Vector3(7.5f, 7.5f, 7.5f)
        };
      case SceneMgr.Mode.ADVENTURE:
      case SceneMgr.Mode.LETTUCE_MAP:
      case SceneMgr.Mode.LETTUCE_PLAY:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(10f, 10f, 10f),
          Phone = new Vector3(7f, 7f, 7f)
        };
      case SceneMgr.Mode.LETTUCE_VILLAGE:
      case SceneMgr.Mode.LETTUCE_PACK_OPENING:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(7f, 7f, 7f),
          Phone = new Vector3(5f, 5f, 5f)
        };
      default:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(15f, 15f, 15f),
          Phone = new Vector3(8f, 8f, 8f)
        };
    }
  }

  public Vector3 GetRewardPunchScale()
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.STARTUP:
      case SceneMgr.Mode.LOGIN:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(15.1f, 15.1f, 15.1f),
          Phone = new Vector3(14.1f, 14.1f, 14.1f)
        };
      case SceneMgr.Mode.GAMEPLAY:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(1.2f, 1.2f, 1.2f),
          Phone = new Vector3(1.25f, 1.25f, 1.25f)
        };
      case SceneMgr.Mode.PACKOPENING:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(8f, 8f, 8f),
          Phone = new Vector3(7.5f, 7.5f, 7.5f)
        };
      case SceneMgr.Mode.ADVENTURE:
      case SceneMgr.Mode.LETTUCE_MAP:
      case SceneMgr.Mode.LETTUCE_PLAY:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(10.2f, 10.2f, 10.2f),
          Phone = new Vector3(7.1f, 7.1f, 7.1f)
        };
      case SceneMgr.Mode.LETTUCE_VILLAGE:
      case SceneMgr.Mode.LETTUCE_PACK_OPENING:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(7.2f, 7.2f, 7.2f),
          Phone = new Vector3(5.1f, 5.1f, 5.1f)
        };
      default:
        return (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
        {
          PC = new Vector3(15.1f, 15.1f, 15.1f),
          Phone = new Vector3(8.1f, 8.1f, 8.1f)
        };
    }
  }
}
