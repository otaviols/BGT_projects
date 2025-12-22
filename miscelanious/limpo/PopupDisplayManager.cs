using Assets;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.InGameMessage.UI;
using Hearthstone.Progression;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDisplayManager : IHasUpdate, IService
{
  private static AchievementPopups s_achievementPopups;
  private static QuestPopups s_questPopups;
  private RewardPopups m_rewardPopups;
  private CardPopups m_cardPopups;
  private LoginPopups m_loginPopups;
  private HealUpPopup m_healUpPopup;
  private RedundantNDERerollPopups m_redundantNDERerollPopups;
  private static bool s_isShowing;
  private static bool s_shouldShowRankedIntro;
  private static bool s_hasPlayerReachedHub;
  private bool m_readyToShowPopups;
  private bool m_hasShownMetaShakeupEventPopups;
  private bool m_hasCheckedNewPlayerSetRotationPopup;
  private bool m_showMessagesForVillageMailbox;
  private static float m_timePlayerInHubAfterLogin;
  private FiresideGatheringManager.OnCloseSign m_onCloseSign = new FiresideGatheringManager.OnCloseSign(PopupDisplayManager.OnPopupClosed);
  private BannerManager.DelOnCloseBanner m_delOnCloseBanner = new BannerManager.DelOnCloseBanner(PopupDisplayManager.OnPopupClosed);
  private ReturningPlayerMgr.WelcomeBannerCloseCallback m_welcomeBannerClose = new ReturningPlayerMgr.WelcomeBannerCloseCallback(PopupDisplayManager.OnPopupClosed);
  private Action m_popClosedCallback = new Action(PopupDisplayManager.OnPopupClosed);
  private Func<bool> m_nextCompletedQuestFunc = new Func<bool>(PopupDisplayManager.ShowNextCompletedQuest);
  private Func<bool> m_nextRankedIntroFunc = new Func<bool>(PopupDisplayManager.ShowNextRankedIntro);
  private bool m_receivedPlayerData;
  public static bool SuppressPopupsTemporarily;

  public RewardPopups RewardPopups => this.m_rewardPopups;

  public AchievementPopups AchievementPopups => PopupDisplayManager.s_achievementPopups;

  public QuestPopups QuestPopups => PopupDisplayManager.s_questPopups;

  public CardPopups CardPopups => this.m_cardPopups;

  public LoginPopups LoginPopups => this.m_loginPopups;

  public HealUpPopup HealUpPopup => this.m_healUpPopup;

  public RedundantNDERerollPopups RedundantNDERerollPopups => this.m_redundantNDERerollPopups;

  private event Action OnAllPopupsShown = () => { };

  private event Action OnPopupShown = () => { };

  public static bool SuppressPopupsForNewPlayer { get; private set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    PopupDisplayManager popupDisplayManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    popupDisplayManager.m_rewardPopups = new RewardPopups(popupDisplayManager, new Action<bool>(popupDisplayManager.SetPopupShowingFlag), popupDisplayManager.OnPopupShown, new Action(PopupDisplayManager.OnPopupClosed));
    PopupDisplayManager.s_achievementPopups = new AchievementPopups(popupDisplayManager, new Action<List<Achievement>>(popupDisplayManager.m_rewardPopups.UpdateRewards));
    PopupDisplayManager.s_questPopups = new QuestPopups(new Action<bool>(popupDisplayManager.SetPopupShowingFlag), popupDisplayManager.OnPopupShown, new Action(PopupDisplayManager.OnPopupClosed), new Reward.DelOnRewardLoaded(popupDisplayManager.m_rewardPopups.DisplayLoadedRewardObject));
    popupDisplayManager.m_cardPopups = new CardPopups();
    popupDisplayManager.m_loginPopups = new LoginPopups();
    popupDisplayManager.m_healUpPopup = new HealUpPopup();
    popupDisplayManager.m_redundantNDERerollPopups = new RedundantNDERerollPopups(new Action<bool>(popupDisplayManager.SetPopupShowingFlag), popupDisplayManager.OnPopupShown, new Action(PopupDisplayManager.OnPopupClosed));
    HearthstoneApplication.Get().WillReset += new Action(popupDisplayManager.WillReset);
    LoginManager.Get().OnFullLoginFlowComplete += new Action(popupDisplayManager.InitializePlayerTimeInHubAfterLogin);
    PopupDisplayManager.m_timePlayerInHubAfterLogin = 0.0f;
    PopupDisplayManager.s_hasPlayerReachedHub = false;
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[5]
  {
    typeof (Network),
    typeof (NetCache),
    typeof (AchieveManager),
    typeof (ReturningPlayerMgr),
    typeof (LoginManager)
  };

  public void Shutdown()
  {
    LoginManager service;
    if (ServiceManager.TryGet<LoginManager>(out service))
      service.OnFullLoginFlowComplete -= new Action(this.InitializePlayerTimeInHubAfterLogin);
    HearthstoneApplication.Get().WillReset -= new Action(this.WillReset);
    this.m_rewardPopups.Dispose();
    PopupDisplayManager.s_achievementPopups.Dispose();
    PopupDisplayManager.s_questPopups.Dispose();
    this.m_loginPopups.Dispose();
    this.m_healUpPopup.Dispose();
    this.m_onCloseSign = (FiresideGatheringManager.OnCloseSign) null;
    this.m_delOnCloseBanner = (BannerManager.DelOnCloseBanner) null;
    this.m_welcomeBannerClose = (ReturningPlayerMgr.WelcomeBannerCloseCallback) null;
    this.m_popClosedCallback = (Action) null;
    this.m_nextCompletedQuestFunc = (Func<bool>) null;
    this.m_nextRankedIntroFunc = (Func<bool>) null;
  }

  public void RegisterAllPopupsShownListener(Action callback)
  {
    if (callback == null)
      return;
    this.OnAllPopupsShown -= callback;
    this.OnAllPopupsShown += callback;
  }

  public void AddPopupShownListener(Action callback)
  {
    if (callback == null)
      return;
    this.OnPopupShown -= callback;
    this.OnPopupShown += callback;
  }

  public void RemovePopupShownListener(Action callback)
  {
    if (callback == null)
      return;
    this.OnPopupShown -= callback;
  }

  public void Update()
  {
    if (!this.m_receivedPlayerData && NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>() != null)
    {
      this.m_receivedPlayerData = true;
      PopupDisplayManager.SuppressPopupsForNewPlayer = !GameUtils.IsAnyTutorialComplete();
    }
    if (!this.m_readyToShowPopups || SceneMgr.Get() == null)
      return;
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.STARTUP:
        break;
      case SceneMgr.Mode.GAMEPLAY:
        break;
      case SceneMgr.Mode.FATAL_ERROR:
        break;
      default:
        if ((UnityEngine.Object) SceneMgr.Get().GetScene() != (UnityEngine.Object) null && SceneMgr.Get().GetScene().IsBlockingPopupDisplayManager())
          break;
        PopupDisplayManager.s_questPopups.ShowQuestProgressToasts(PopupDisplayManager.s_achievementPopups.ProgressedAchieves);
        if (GameUtils.IsAnyTransitionActive() || this.IsShowing)
          break;
        if (!this.m_hasCheckedNewPlayerSetRotationPopup)
        {
          this.m_hasCheckedNewPlayerSetRotationPopup = true;
          if (SetRotationManager.Get().ShowNewPlayerSetRotationPopupIfNeeded())
            break;
        }
        if (ReturningPlayerMgr.Get() != null && ReturningPlayerMgr.Get().ShowReturningPlayerWelcomeBannerIfNeeded(this.m_welcomeBannerClose))
        {
          this.OnPopupShown();
          PopupDisplayManager.s_isShowing = true;
          break;
        }
        if (FiresideGatheringManager.Get() != null && FiresideGatheringManager.Get().ShowSignIfNeeded(this.m_onCloseSign))
        {
          this.OnPopupShown();
          PopupDisplayManager.s_isShowing = true;
          break;
        }
        if (PopupDisplayManager.ShouldDisableNotificationOnLogin())
          BannerManager.Get().AutoAcknowledgeOutstandingBanner();
        else if (BannerManager.Get().ShowOutstandingBannerEvent(this.m_delOnCloseBanner))
        {
          this.OnPopupShown();
          PopupDisplayManager.s_isShowing = true;
          break;
        }
        if (DraftManager.Get() != null && DraftManager.Get().ShowNextArenaPopup(this.m_popClosedCallback))
        {
          this.OnPopupShown();
          PopupDisplayManager.s_isShowing = true;
          break;
        }
        if (this.m_cardPopups.ShowChangedCards(ignoredAttentionBlockers: UserAttentionBlocker.SET_ROTATION_INTRO))
          break;
        if (!this.m_hasShownMetaShakeupEventPopups && this.m_loginPopups.ShowLoginPopupSequence(PopupDisplayManager.SuppressPopupsForNewPlayer, PopupDisplayManager.ShouldDisableNotificationOnLogin(), this.m_cardPopups))
        {
          this.m_hasShownMetaShakeupEventPopups = true;
          break;
        }
        if (this.ShowRewardAndOtherPopups() || !PopupDisplayManager.ShouldSuppressPopups() && PopupDisplayManager.s_questPopups.ShowNextQuestNotification() || (!PopupDisplayManager.ShouldSuppressPopups() || this.m_showMessagesForVillageMailbox) && !JournalPopup.s_isShowing && this.ShowInGameMessagePopups() || this.ShowCreateSkipPopup())
          break;
        NarrativeManager.Get().OnAllPopupsShown();
        if (this.IsShowing)
          break;
        this.OnAllPopupsShown();
        this.ClearAllPopupsShownListeners();
        break;
    }
  }

  public static PopupDisplayManager Get() => ServiceManager.Get<PopupDisplayManager>();

  public void SetVillageMailboxMessageShouldShow() => this.m_showMessagesForVillageMailbox = true;

  private void WillReset()
  {
    this.m_readyToShowPopups = false;
    PopupDisplayManager.s_isShowing = false;
    this.m_receivedPlayerData = false;
    this.m_hasShownMetaShakeupEventPopups = false;
    this.m_hasCheckedNewPlayerSetRotationPopup = false;
    PopupDisplayManager.SuppressPopupsForNewPlayer = false;
    PopupDisplayManager.SuppressPopupsTemporarily = false;
    this.m_showMessagesForVillageMailbox = false;
    LoginManager service1;
    if (ServiceManager.TryGet<LoginManager>(out service1))
      service1.OnFullLoginFlowComplete -= new Action(this.InitializePlayerTimeInHubAfterLogin);
    UniversalInputManager service2;
    if (ServiceManager.TryGet<UniversalInputManager>(out service2))
      service2.SetGameDialogActive(false);
    DialogManager dialogManager = DialogManager.Get();
    if ((UnityEngine.Object) dialogManager != (UnityEngine.Object) null)
    {
      dialogManager.ReadyForSeasonEndPopup(false);
      dialogManager.ClearHandledMedalNotices();
    }
    this.ClearAllPopupsShownListeners();
  }

  public void ReadyToShowPopups()
  {
    if (!Network.IsLoggedIn())
      return;
    this.m_readyToShowPopups = true;
    this.Update();
  }

  public bool IsShowing => PopupDisplayManager.s_isShowing || (UnityEngine.Object) DialogManager.Get() != (UnityEngine.Object) null && DialogManager.Get().ShowingDialog() || (UnityEngine.Object) WelcomeQuests.Get() != (UnityEngine.Object) null || (UnityEngine.Object) NarrativeManager.Get() != (UnityEngine.Object) null && NarrativeManager.Get().IsShowingBlockingDialog() || BannerManager.Get().IsShowing || RewardXpNotificationManager.Get().IsShowingXpGains;

  public IEnumerator WaitForAllPopups()
  {
    bool allPopupsShown = false;
    this.RegisterAllPopupsShownListener((Action) (() => allPopupsShown = true));
    while (!allPopupsShown)
      yield return (object) null;
  }

  public IEnumerator<IAsyncJobResult> Job_WaitForAllPopups()
  {
    this.ReadyToShowPopups();
    bool allPopupsShown = false;
    this.RegisterAllPopupsShownListener((Action) (() => allPopupsShown = true));
    while (!allPopupsShown)
      yield return (IAsyncJobResult) null;
  }

  private void SetPopupShowingFlag(bool isShowing) => PopupDisplayManager.s_isShowing = isShowing;

  private static void OnPopupClosed() => PopupDisplayManager.s_isShowing = false;

  public void OnRewardPresenterScrollQueued(int rewardItemId) => this.m_redundantNDERerollPopups.OnRewardPresenterScrollQueued(rewardItemId);

  public bool CanShowPopups() => SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY || !((UnityEngine.Object) EndGameScreen.Get() == (UnityEngine.Object) null) && EndGameScreen.Get().IsDoneDisplayingRewards();

  public static bool ShouldSuppressPopups() => PopupDisplayManager.SuppressPopupsForNewPlayer || PopupDisplayManager.SuppressPopupsTemporarily;

  public void ShowAnyOutstandingPopups() => this.ShowAnyOutstandingPopups((Action) null);

  public void ShowAnyOutstandingPopups(Action callback) => this.ShowAnyOutstandingPopups(new HashSet<Achieve.RewardTiming>()
  {
    Achieve.RewardTiming.IMMEDIATE,
    Achieve.RewardTiming.OUT_OF_BAND,
    Achieve.RewardTiming.ADVENTURE_CHEST
  }, callback);

  private void ShowAnyOutstandingPopups(
    HashSet<Achieve.RewardTiming> rewardTimings,
    Action callback)
  {
    PopupDisplayManager.s_achievementPopups.PrepareNewlyCompletedAchievesToBeShown(rewardTimings);
    if (callback != null)
      this.RegisterAllPopupsShownListener(callback);
    this.ReadyToShowPopups();
  }

  private void ClearAllPopupsShownListeners() => this.OnAllPopupsShown = (Action) (() => { });

  public void ShowRankedIntro() => PopupDisplayManager.s_shouldShowRankedIntro = true;

  private static bool ShowNextCompletedQuest() => PopupDisplayManager.s_questPopups.ShowNextCompletedQuest(PopupDisplayManager.s_achievementPopups.CompletedAchieves, PopupDisplayManager.ShouldSuppressPopups());

  private static bool ShowNextRankedIntro()
  {
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber("PopupDisplayManager.ShowNextRankedIntro") || !PopupDisplayManager.s_shouldShowRankedIntro)
      return false;
    PopupDisplayManager.s_isShowing = true;
    PopupDisplayManager.s_shouldShowRankedIntro = false;
    DialogManager.Get().ShowRankedIntroPopUp((Action) null);
    MedalInfoTranslator localPlayerMedalInfo = RankMgr.Get().GetLocalPlayerMedalInfo();
    DialogManager.Get().ShowBonusStarsPopup(localPlayerMedalInfo.CreateDataModel(PegasusShared.FormatType.FT_STANDARD, RankedMedal.DisplayMode.Default), new Action(PopupDisplayManager.OnPopupClosed));
    return true;
  }

  private bool ShowRewardAndOtherPopups() => this.m_rewardPopups.ShowRewardPopups(PopupDisplayManager.s_achievementPopups.CompletedAchieves, PopupDisplayManager.ShouldSuppressPopups(), this.m_nextRankedIntroFunc, this.m_nextCompletedQuestFunc) || this.m_redundantNDERerollPopups.ShowRerollPopup();

  public static bool ShouldDisableNotificationOnLogin()
  {
    if (HearthstoneApplication.IsPublic() || StoreManager.Get().IsShown() || !Options.Get().GetBool(Option.DISABLE_LOGIN_POPUPS))
      return false;
    return !PopupDisplayManager.s_hasPlayerReachedHub || (double) Time.realtimeSinceStartup - (double) PopupDisplayManager.m_timePlayerInHubAfterLogin < 20.0;
  }

  private void InitializePlayerTimeInHubAfterLogin()
  {
    PopupDisplayManager.s_hasPlayerReachedHub = true;
    PopupDisplayManager.m_timePlayerInHubAfterLogin = Time.realtimeSinceStartup;
  }

  private bool ShowInGameMessagePopups()
  {
    MessagePopupDisplay service;
    if (ServiceManager.TryGet<MessagePopupDisplay>(out service))
    {
      if (service.IsDisplayingMessage)
        return true;
      if (service.HasMessageToDisplay)
      {
        PopupDisplayManager.s_isShowing = true;
        this.OnPopupShown();
        service.DisplayIGMMessage((Action) (() => PopupDisplayManager.s_isShowing = false));
        return true;
      }
    }
    return false;
  }

  private bool ShowCreateSkipPopup()
  {
    if (!this.m_healUpPopup.IsHealUpPopupQueuedForDisplay)
      return false;
    PopupDisplayManager.s_isShowing = true;
    this.OnPopupShown();
    if (!this.m_healUpPopup.ShowQueuedPopupAfterTutorial((Action) (() => PopupDisplayManager.s_isShowing = false)))
      PopupDisplayManager.s_isShowing = false;
    return true;
  }
}
