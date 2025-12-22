using Assets;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.InGameMessage.UI;
using Hearthstone.UI;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub : PegasusScene
{
  private static readonly WaitForSeconds HumanInteractionPollSpan = new WaitForSeconds(0.1f);
  private Notification m_modesButtonNotification;
  private bool m_hasCheckedForNewPlayer;
  private bool m_isTutorialPreviewOpen;
  private const float PracticeModePopupOffsetX = 33.62785f;
  private const float PracticeModePopupOffsetXPhoneUI = 30.46f;
  private Notification m_innkeeperPopup;

  private void Start()
  {
    IJobDependency[] jobDependencyArray = HearthstoneJobs.BuildDependencies((object) typeof (SceneMgr), (object) typeof (IAssetLoader), (object) typeof (NetCache), (object) typeof (SpecialEventManager), (object) typeof (DemoMgr), (object) typeof (AchieveManager), (object) typeof (HealthyGamingMgr), (object) typeof (FiresideGatheringManager), (object) typeof (TavernBrawlManager), (object) typeof (GameMgr), (object) typeof (ShownUIMgr), (object) typeof (MusicManager), (object) typeof (SoundManager), (object) typeof (SetRotationManager), (object) typeof (PopupDisplayManager));
    Processor.QueueJob("Hub.Initialize", this.Job_Initialize(), jobDependencyArray);
  }

  private IEnumerator<IAsyncJobResult> Job_Initialize()
  {
    this.VerifyPrequisitesInitialized();
    if (Network.ShouldBeConnectedToAurora())
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.HUB);
    else
      Error.AddDevWarning("Alert", "There is no connection to Battle.net, please restart Hearthstone to log in.");
    this.RegisterEventListeners();
    SceneMgr.Get().NotifySceneLoaded();
    if (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LOGIN)
      Box.Get().PlayBoxMusic();
    this.ShowHubStartNotifications();
    if (!Network.ShouldBeConnectedToAurora())
    {
      Box.Get().DisableAllButtons();
      yield break;
    }
  }

  private void OnTutorialPreviewOpened()
  {
    this.m_isTutorialPreviewOpen = true;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_innkeeperPopup);
  }

  private void OnTutorialPreviewClosed() => this.m_isTutorialPreviewOpen = false;

  private void OnMessageModalShopButtonPressed() => this.HideTooltipNotification(false);

  private void OnDestroy() => this.UnregisterEventListeners();

  private void VerifyPrequisitesInitialized()
  {
    if (CollectionManager.Get() == null)
      Debug.LogError((object) "Hub.Start Error - CollectionManager is null");
    if (PresenceMgr.Get() == null)
      Debug.LogError((object) "Hub.Start Error - PresenceMgr is null");
    if ((UnityEngine.Object) Box.Get() == (UnityEngine.Object) null)
      Debug.LogError((object) "Hub.Start Error - Box is null");
    if (Options.Get() == null)
      Debug.LogError((object) "Hub.Start Error - Options is null");
    if ((UnityEngine.Object) NotificationManager.Get() == (UnityEngine.Object) null)
      Debug.LogError((object) "Hub.Start Error - NotificationManager is null");
    if (StoreManager.Get() != null)
      return;
    Debug.LogError((object) "Hub.Start Error - StoreManager is null");
  }

  private void RegisterEventListeners()
  {
    Box box = Box.Get();
    if ((UnityEngine.Object) box != (UnityEngine.Object) null)
    {
      box.AddButtonPressListener(new Box.ButtonPressCallback(this.OnBoxButtonPressed));
      if ((UnityEngine.Object) box.m_QuestLogButton == (UnityEngine.Object) null)
        Debug.LogError((object) "Hub.Start Error - QuestLogButton is null");
      else
        box.m_QuestLogButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
      if ((UnityEngine.Object) box.m_journalButtonWidget == (UnityEngine.Object) null)
        Debug.LogError((object) "Hub.Start Error - JournalButton is null");
      else
        box.m_journalButtonWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnButtonClickedHandleJournalWidgetTooltipNotification));
      if ((UnityEngine.Object) box.m_StoreButton == (UnityEngine.Object) null)
        Debug.LogError((object) "Hub.Start Error - StoreButton is null");
      else
        box.m_StoreButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        if ((UnityEngine.Object) box.m_ribbonButtons == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "Hub.Start Error - RibbonButtons is null");
        }
        else
        {
          if ((UnityEngine.Object) box.m_ribbonButtons.m_questLogRibbon == (UnityEngine.Object) null)
            Debug.LogError((object) "Hub.Start Error - QuestLogRibbon is null");
          else
            box.m_ribbonButtons.m_questLogRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
          if ((UnityEngine.Object) box.m_ribbonButtons.m_storeRibbon == (UnityEngine.Object) null)
            Debug.LogError((object) "Hub.Start Error - StoreRibbon is null");
          else
            box.m_ribbonButtons.m_storeRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
          if ((UnityEngine.Object) box.m_ribbonButtons.m_journalButtonWidget == (UnityEngine.Object) null)
            Debug.LogError((object) "Hub.Start Error - JournalRibbon is null");
          else
            box.m_ribbonButtons.m_journalButtonWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnButtonClickedHandleJournalWidgetTooltipNotification));
        }
      }
    }
    else
      Debug.LogError((object) "Hub.Start Error - box is null");
    if (StoreManager.IsInitialized())
      StoreManager.Get().RegisterSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnAdventureBundlePurchase));
    else
      Debug.LogError((object) "Hub.Start Error - RegisterSuccessfulPurchaseListener not assigned");
    SpecialEventType eventType = SpecialEventType.IGNORE;
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    if (specialEventManager != null)
      eventType = specialEventManager.GetActiveEventType();
    else
      Debug.LogError((object) "Hub.Start Error - SpecialEventManager was null and eventType was not received");
    AchieveManager achieveManager = AchieveManager.Get();
    if (eventType != SpecialEventType.IGNORE && achieveManager != null && achieveManager.HasUnlockedArena())
    {
      specialEventManager?.Visuals.LoadEvent(eventType);
      if (SceneMgr.IsInitialized())
        SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
      else
        Debug.LogError((object) "Hub.Start Error - SceneMgr did not register scene unload event");
    }
    TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
    if (tavernBrawlManager != null)
      tavernBrawlManager.OnSessionLimitRaised += new TavernBrawlManager.TavernBrawlSessionLimitRaisedCallback(this.MaybeDoTavernBrawlLimitRaisedAlert);
    else
      Debug.LogError((object) "Hub.Start Error - TavernBrawlManager did not register certain events");
    TutorialPreviewController.PreviewOpened += new Action(this.OnTutorialPreviewOpened);
    TutorialPreviewController.PreviewClosed += new Action(this.OnTutorialPreviewClosed);
    MessageModal.ShopButtonPressed += new Action(this.OnMessageModalShopButtonPressed);
  }

  private void UnregisterEventListeners()
  {
    TavernBrawlManager service;
    if (ServiceManager.TryGet<TavernBrawlManager>(out service))
      service.OnSessionLimitRaised -= new TavernBrawlManager.TavernBrawlSessionLimitRaisedCallback(this.MaybeDoTavernBrawlLimitRaisedAlert);
    TutorialPreviewController.PreviewOpened -= new Action(this.OnTutorialPreviewOpened);
    TutorialPreviewController.PreviewClosed -= new Action(this.OnTutorialPreviewClosed);
    MessageModal.ShopButtonPressed -= new Action(this.OnMessageModalShopButtonPressed);
    Box box = Box.Get();
    if (!((UnityEngine.Object) box != (UnityEngine.Object) null))
      return;
    box.RemoveButtonPressListener(new Box.ButtonPressCallback(this.OnBoxButtonPressed));
    if ((UnityEngine.Object) box.m_QuestLogButton != (UnityEngine.Object) null)
      box.m_QuestLogButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
    if ((UnityEngine.Object) box.m_journalButtonWidget != (UnityEngine.Object) null)
      box.m_journalButtonWidget.RemoveEventListener(new Widget.EventListenerDelegate(this.OnButtonClickedHandleJournalWidgetTooltipNotification));
    if ((UnityEngine.Object) box.m_StoreButton != (UnityEngine.Object) null)
      box.m_StoreButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
    if (!(bool) UniversalInputManager.UsePhoneUI || !((UnityEngine.Object) box.m_ribbonButtons != (UnityEngine.Object) null))
      return;
    if ((UnityEngine.Object) box.m_ribbonButtons.m_questLogRibbon != (UnityEngine.Object) null)
      box.m_ribbonButtons.m_questLogRibbon.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
    if ((UnityEngine.Object) box.m_ribbonButtons.m_storeRibbon != (UnityEngine.Object) null)
      box.m_ribbonButtons.m_storeRibbon.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
    if (!((UnityEngine.Object) box.m_ribbonButtons.m_journalButtonWidget != (UnityEngine.Object) null))
      return;
    box.m_ribbonButtons.m_journalButtonWidget.RemoveEventListener(new Widget.EventListenerDelegate(this.OnButtonClickedHandleJournalWidgetTooltipNotification));
  }

  private void OnButtonClickedHandleJournalWidgetTooltipNotification(string eventName)
  {
    if (!(eventName == "JOURNAL_OPENED"))
      return;
    this.HideTooltipNotification(false);
  }

  private void ShowModesButtonNotification(string message)
  {
    this.HideTooltipNotification(false);
    Vector3 modesButtonPosition = Box.Get().GetModesButtonPosition();
    NotificationManager notificationManager = NotificationManager.Get();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      modesButtonPosition.x -= 30.46f;
      this.m_modesButtonNotification = notificationManager.CreatePopupText(UserAttentionBlocker.NONE, modesButtonPosition, 25f * Vector3.one, GameStrings.Get(message));
    }
    else
    {
      modesButtonPosition.x -= 33.62785f;
      this.m_modesButtonNotification = notificationManager.CreatePopupText(UserAttentionBlocker.NONE, modesButtonPosition, 15f * Vector3.one, GameStrings.Get(message));
    }
    this.m_modesButtonNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
  }

  private void ShowHubStartNotifications()
  {
    PopupDisplayManager.Get().ReadyToShowPopups();
    if (!Network.ShouldBeConnectedToAurora())
      return;
    if (!Options.Get().GetBool(Option.HAS_SEEN_HUB, false) && UserAttentionManager.CanShowAttentionGrabber("Hub.Start:" + (object) Option.HAS_SEEN_HUB))
      this.StartCoroutine(this.DoFirstTimeHubWelcome());
    else if (!Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false) && UserAttentionManager.CanShowAttentionGrabber("Hub.Start:" + (object) Option.HAS_SEEN_PRACTICE_MODE))
      this.ShowModesButtonNotification("GLUE_PRACTICE_HINT");
    else if (GameModeUtils.ShouldSeeSoloAdventuresMovedPopup() && GameModeUtils.CanAccessGameModes() && UserAttentionManager.CanShowAttentionGrabber("Hub.Start:" + (object) GameSaveKeySubkeyId.FTUE_SHOULD_SEE_SOLO_ADVENTURES_MOVED_POPUP))
      this.ShowModesButtonNotification("GLUE_SOLO_ADVENTURES_MOVED_HINT");
    else if (!Options.Get().GetBool(Option.HAS_SEEN_100g_REMINDER, false))
    {
      NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
      if (netObject == null)
        Debug.LogError((object) "Hub.Start Error - NetCache.NetCacheGoldBalance is null");
      if (netObject.GetTotal() >= 100L && UserAttentionManager.CanShowAttentionGrabber("Hub.Start:" + (object) Option.HAS_SEEN_100g_REMINDER))
      {
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FIRST_100_GOLD"), "VO_INNKEEPER_FIRST_100_GOLD.prefab:c6a50337099a454488acd96d2f37320f");
        Options.Get().SetBool(Option.HAS_SEEN_100g_REMINDER, true);
      }
    }
    if (GameModeUtils.HasSeenMercenariesButtonActivation())
      return;
    Box.Get().PlayMercenariesButtonActivation(true);
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.FTUE, GameSaveKeySubkeyId.FTUE_HAS_SEEN_MERCENARIES_BUTTON_ACTIVATION, new long[1]
    {
      1L
    }));
  }

  private void MaybeDoTavernBrawlLimitRaisedAlert(int lastSeenLimit, int newLimit)
  {
    int availableForPurchase = TavernBrawlManager.Get().NumSessionsAvailableForPurchase;
    if (availableForPurchase != newLimit - lastSeenLimit)
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_headerText = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_LIMIT_ALERT_TITLE"),
      m_text = GameStrings.Format("GLUE_HEROIC_BRAWL_SESSION_LIMIT_ALERT_LIMIT_RAISED", (object) availableForPurchase)
    });
  }

  private void Update() => Network.Get().ProcessNetwork();

  public override void Unload()
  {
    StoreManager.Get().RemoveSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnAdventureBundlePurchase));
    this.HideTooltipNotification(true);
    Box box = Box.Get();
    if (!((UnityEngine.Object) box != (UnityEngine.Object) null))
      return;
    box.m_QuestLogButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
    box.m_StoreButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleasedHideTooltipNotification));
    box.RemoveButtonPressListener(new Box.ButtonPressCallback(this.OnBoxButtonPressed));
    box.Unload();
  }

  private void OnSceneUnloaded(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    SpecialEventType activeEventType = SpecialEventManager.Get().GetActiveEventType();
    if (activeEventType != SpecialEventType.IGNORE)
      SpecialEventManager.Get().Visuals.UnloadEvent(activeEventType);
    SceneMgr.Get().UnregisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
  }

  private void OnBoxButtonPressed(
    Box.ButtonType buttonType,
    bool isShowingTutorialVideo,
    object userData)
  {
    switch (buttonType)
    {
      case Box.ButtonType.TRADITIONAL:
        if (isShowingTutorialVideo)
        {
          this.HideTooltipNotification(true);
          break;
        }
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
        Tournament.Get().NotifyOfBoxTransitionStart();
        break;
      case Box.ButtonType.OPEN_PACKS:
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.PACKOPENING);
        break;
      case Box.ButtonType.COLLECTION:
        CollectionManager.Get().NotifyOfBoxTransitionStart();
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.COLLECTIONMANAGER);
        break;
      case Box.ButtonType.SET_ROTATION:
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
        Tournament.Get().NotifyOfBoxTransitionStart();
        break;
      case Box.ButtonType.GAME_MODES:
        this.HandleGameModesButtonPressed();
        break;
      case Box.ButtonType.BACON:
        if (isShowingTutorialVideo)
        {
          this.HideTooltipNotification(true);
          break;
        }
        this.HandleBaconButtonPressed();
        break;
      case Box.ButtonType.MERCENARIES:
        if (!isShowingTutorialVideo)
          break;
        this.HideTooltipNotification(true);
        break;
    }
  }

  private void HandleGameModesButtonPressed() => SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAME_MODE);

  private void HandleBaconButtonPressed() => SceneMgr.Get().SetNextMode(SceneMgr.Mode.BACON);

  private IEnumerator DoFirstTimeHubWelcome()
  {
    for (Box box = Box.Get(); (UnityEngine.Object) box == (UnityEngine.Object) null || box.IsBusy() || (UnityEngine.Object) box.m_BattleGroundsButton == (UnityEngine.Object) null || !box.m_BattleGroundsButton.IsEnabled() || box.GetState() != Box.State.HUB_WITH_DRAWER || (UnityEngine.Object) box.GetBoxCamera() == (UnityEngine.Object) null || box.GetBoxCamera().GetState() != BoxCamera.State.CLOSED_WITH_DRAWER; box = Box.Get())
      yield return (object) Hub.HumanInteractionPollSpan;
    while (!GameUtils.IsTraditionalTutorialComplete())
      yield return (object) Hub.HumanInteractionPollSpan;
    StoreManager storeManager = StoreManager.Get();
    for (QuestLog questLog = QuestLog.Get(); storeManager != null && storeManager.IsShown() || (UnityEngine.Object) questLog != (UnityEngine.Object) null && questLog.IsShown(); questLog = QuestLog.Get())
    {
      yield return (object) Hub.HumanInteractionPollSpan;
      storeManager = StoreManager.Get();
    }
    for (AchieveManager achieveManager = AchieveManager.Get(); achieveManager != null && achieveManager.HasQuestsToShow(true) || (UnityEngine.Object) WelcomeQuests.Get() != (UnityEngine.Object) null; achieveManager = AchieveManager.Get())
      yield return (object) Hub.HumanInteractionPollSpan;
    for (PopupDisplayManager popupDisplayManager = PopupDisplayManager.Get(); popupDisplayManager != null && popupDisplayManager.IsShowing; popupDisplayManager = PopupDisplayManager.Get())
      yield return (object) Hub.HumanInteractionPollSpan;
    NotificationManager notificationManager = NotificationManager.Get();
    if ((UnityEngine.Object) notificationManager != (UnityEngine.Object) null && !this.m_isTutorialPreviewOpen)
    {
      this.m_innkeeperPopup = notificationManager.CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_1ST_HUB_06"), "VO_INNKEEPER_1ST_HUB_06.prefab:9774392944a21424788286f80d401d8c", 3f);
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        while ((UnityEngine.Object) this.m_innkeeperPopup != (UnityEngine.Object) null)
          yield return (object) Hub.HumanInteractionPollSpan;
      }
    }
    if (!this.m_isTutorialPreviewOpen)
      this.ShowModesButtonNotification("GLUE_PRACTICE_HINT");
    Options.Get().SetBool(Option.HAS_SEEN_HUB, true);
    AdTrackingManager adTrackingManager = AdTrackingManager.Get();
    if (adTrackingManager != null)
      adTrackingManager.TrackFirstLogin();
    else
      Debug.LogWarning((object) "AdTrackingManager was not initialized during Hub.DoFirstTimeHubWelcome()");
  }

  private void OnAdventureBundlePurchase(Network.Bundle bundle, PaymentMethod purchaseMethod)
  {
    if ((Record) bundle == (Record) null || bundle.Items == null)
      return;
    foreach (Network.BundleItem bundleItem in bundle.Items)
    {
      if (bundleItem.ItemType == ProductType.PRODUCT_TYPE_NAXX)
      {
        AdventureConfig.Get().SetSelectedAdventureMode(AdventureDbId.NAXXRAMAS, AdventureModeDbId.LINEAR);
        break;
      }
    }
  }

  private void OnButtonReleasedHideTooltipNotification(UIEvent e) => this.HideTooltipNotification(true);

  private void HideTooltipNotification(bool animate)
  {
    if ((UnityEngine.Object) this.m_modesButtonNotification == (UnityEngine.Object) null)
      return;
    if (animate)
      NotificationManager.Get().DestroyNotification(this.m_modesButtonNotification, 0.0f);
    else
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_modesButtonNotification);
  }
}
