using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.APIGateway;
using Hearthstone.Core;
using Hearthstone.InGameMessage;
using Hearthstone.Login;
using Hearthstone.Streaming;
using PegasusFSG;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LoginManager : IService
{
  private WaitForCallback UpdateLoginCompleteDependency;
  private WaitForCallback SetProgressDependency;
  private static SortedList<StartupSceneSource, DetermineStartupSceneCallback> s_determinePostLoginCallbacks = new SortedList<StartupSceneSource, DetermineStartupSceneCallback>();
  private JobDefinition WaitForLogin;
  private Hearthstone.BreakingNews.BreakingNews m_breakingNews;
  public WaitForCallback LoggedInDependency;
  public WaitForCallback ReadyToGoToNextModeDependency;
  public WaitForCallback ReadyToReconnectOrChangeModeDependency;
  public WaitForCallback InitialClientStateReceivedDependency;
  public WaitForCallback LoginScreenNetCacheReceivedDependency;
  public WaitForCallback OptInsReceivedDependency;

  public OptInApi OptInApi { get; private set; }

  public event System.Action OnLoginCompleted;

  public event System.Action OnAchievesLoaded;

  public event System.Action OnInitialClientStateReceived;

  public event System.Action OnFullLoginFlowComplete;

  public event System.Action<Network.QueueInfo> OnQueueModifiedEvent;

  public Network.QueueInfo CurrentQueueInfo { get; private set; }

  public bool IsFullLoginFlowComplete { get; private set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LoginManager loginManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    loginManager.LoggedInDependency = new WaitForCallback();
    loginManager.ReadyToGoToNextModeDependency = new WaitForCallback();
    loginManager.ReadyToReconnectOrChangeModeDependency = new WaitForCallback();
    loginManager.InitialClientStateReceivedDependency = new WaitForCallback();
    loginManager.LoginScreenNetCacheReceivedDependency = new WaitForCallback();
    loginManager.OptInsReceivedDependency = new WaitForCallback();
    loginManager.UpdateLoginCompleteDependency = new WaitForCallback();
    loginManager.SetProgressDependency = new WaitForCallback();
    loginManager.m_breakingNews = serviceLocator.Get<Hearthstone.BreakingNews.BreakingNews>();
    Network.Get().RegisterNetHandler((object) AssetsVersionResponse.PacketID.ID, new Network.NetHandler(loginManager.OnAssetsVersionResponse));
    Network.Get().RegisterNetHandler((object) InitialClientState.PacketID.ID, new Network.NetHandler(loginManager.OnInitialClientStateResponse));
    Network.Get().RegisterNetHandler((object) SetProgressResponse.PacketID.ID, new Network.NetHandler(loginManager.OnSetProgressResponse));
    Network.Get().RegisterNetHandler((object) UpdateLoginComplete.PacketID.ID, new Network.NetHandler(loginManager.UpdateLoginCompleteDependency.Callback.Invoke));
    loginManager.OnInitialClientStateReceived += new System.Action(loginManager.InitializeManagers);
    loginManager.OnAchievesLoaded += new System.Action(loginManager.UpdateTutorialPresence);
    HearthstoneApplication.Get().Resetting += new System.Action(loginManager.OnReset);
    loginManager.CurrentQueueInfo = (Network.QueueInfo) null;
    if (!Vars.Key("Aurora.ClientCheck").GetBool(true) || !BattleNetClient.needsToRun)
      Network.Get().RegisterQueueInfoHandler(new Network.QueueInfoHandler(loginManager.QueueInfoHandler));
    loginManager.BeginLoginProcess();
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[7]
  {
    typeof (Network),
    typeof (GameDownloadManager),
    typeof (NetCache),
    typeof (ILoginService),
    typeof (SceneMgr),
    typeof (AchieveManager),
    typeof (Hearthstone.BreakingNews.BreakingNews)
  };

  public void Shutdown()
  {
    NetCache service1;
    if (ServiceManager.TryGet<NetCache>(out service1))
      service1.UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    Network service2;
    if (!ServiceManager.TryGet<Network>(out service2))
      return;
    service2.RemoveNetHandler((object) AssetsVersionResponse.PacketID.ID, new Network.NetHandler(this.OnAssetsVersionResponse));
    service2.RemoveNetHandler((object) InitialClientState.PacketID.ID, new Network.NetHandler(this.OnInitialClientStateResponse));
    service2.RemoveNetHandler((object) SetProgressResponse.PacketID.ID, new Network.NetHandler(this.OnSetProgressResponse));
    service2.RemoveNetHandler((object) UpdateLoginComplete.PacketID.ID, new Network.NetHandler(this.UpdateLoginCompleteDependency.Callback.Invoke));
  }

  private void OnReset()
  {
    this.WaitForLogin = (JobDefinition) null;
    this.BeginLoginProcess();
  }

  public static LoginManager Get() => ServiceManager.Get<LoginManager>();

  public static void RegisterDeterminePostLoginSceneCallback(
    StartupSceneSource source,
    DetermineStartupSceneCallback cb)
  {
    DetermineStartupSceneCallback startupSceneCallback;
    if (LoginManager.s_determinePostLoginCallbacks.TryGetValue(source, out startupSceneCallback))
      Log.All.PrintError("RegisterDetermineStartupSceneCallback error: source={0} already registered to {1} - will overwrite with {2}.", (object) source, (object) startupSceneCallback, (object) cb);
    LoginManager.s_determinePostLoginCallbacks[source] = cb;
  }

  public static SortedList<StartupSceneSource, DetermineStartupSceneCallback> GetPostLoginCallbacks() => LoginManager.s_determinePostLoginCallbacks;

  public void BeginLoginProcess()
  {
    this.InitializeForNewLogin();
    if (!Network.ShouldBeConnectedToAurora())
    {
      Log.Login.Print("Entering No Account flow.");
      DefLoader.Get().Initialize();
      this.ReadyToReconnectOrChangeModeDependency.Callback();
    }
    else
    {
      if (this.WaitForLogin != null)
        return;
      Log.Login.Print("Entering Login flow.");
      ServiceManager.Get<ILoginService>().StartLogin();
      Network.Get().OnLoginStarted();
      this.WaitForLogin = new JobDefinition("LoginManager.WaitForLogin", this.Job_WaitForLogin(), new IJobDependency[1]
      {
        (IJobDependency) new WaitForGameDownloadManagerState()
      });
      Processor.QueueJob(this.WaitForLogin);
      HearthstoneApplication.SendStartupTimeTelemetry("LoginManager.BeginLoginProcess");
    }
  }

  private void InitializeForNewLogin()
  {
    this.UpdateLoginCompleteDependency.Reset();
    this.SetProgressDependency.Reset();
    this.ReadyToGoToNextModeDependency.Reset();
    this.ReadyToReconnectOrChangeModeDependency.Reset();
    this.LoggedInDependency.Reset();
    this.InitialClientStateReceivedDependency.Reset();
    this.LoginScreenNetCacheReceivedDependency.Reset();
    this.OptInsReceivedDependency.Reset();
    this.IsFullLoginFlowComplete = false;
  }

  private IEnumerator<IAsyncJobResult> Job_WaitForLogin()
  {
    while (true)
    {
      ConnectionState connectionState = Network.BattleNetStatus();
      if (connectionState != ConnectionState.Ready || BattleNet.GetAccountCountry() == null || BattleNet.GetAccountRegion() == BnetRegion.REGION_UNINITIALIZED)
      {
        if (connectionState != ConnectionState.Error && connectionState != ConnectionState.Disconnected)
          yield return (IAsyncJobResult) null;
        else
          goto label_4;
      }
      else
        break;
    }
    this.WaitForLogin = (JobDefinition) null;
    Log.TemporaryAccount.Print("Is Temporary Account: " + (BattleNet.IsHeadlessAccount() ? "Yes" : "No"));
    this.OnLoginComplete();
    yield break;
label_4:
    this.WaitForLogin = (JobDefinition) null;
    Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_LOGIN_FAILURE");
  }

  private IEnumerator<IAsyncJobResult> Job_WaitForStartupPacketSequenceComplete()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LoginManager loginManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.SendStartupTimeTelemetry("LoginManager.WaitForStartupPacketSequenceComplete");
    Network.Get().OnStartupPacketSequenceComplete();
    NetCache.Get().RegisterScreenLogin(new NetCache.NetCacheCallback(loginManager.OnNetCacheReady));
    return false;
  }

  private void InitializeManagers()
  {
    if (Network.IsLoggedIn())
    {
      TelemetryManager.RebuildContext();
      BnetPresenceMgr.Get().Initialize();
      BnetFriendMgr.Get().Initialize();
      BnetWhisperMgr.Get().Initialize();
      BnetRecentPlayerMgr.Get().Initialize();
      BnetNearbyPlayerMgr.Get().Initialize();
      FriendChallengeMgr.Get().OnLoggedIn();
      SpectatorManager.Get().InitializeConnectedToBnet();
      NarrativeManager.Get().Initialize();
      if (!Options.Get().GetBool(Option.CONNECT_TO_AURORA))
        Options.Get().SetBool(Option.CONNECT_TO_AURORA, true);
      if (PlatformSettings.IsMobile() && (BnetRegion) Options.Get().GetInt(Option.PREFERRED_REGION) != MobileDeviceLocale.GetCurrentRegionId())
        Options.Get().SetInt(Option.PREFERRED_REGION, (int) MobileDeviceLocale.GetCurrentRegionId());
      if (Options.Get().GetBool(Option.CREATED_ACCOUNT))
      {
        AdTrackingManager.Get().TrackAccountCreated();
        if (PlatformSettings.IsMobile())
          AchieveManager.Get().NotifyOfAccountCreation();
        Options.Get().DeleteOption(Option.CREATED_ACCOUNT);
      }
    }
    RAFManager.Get().InitializeRequests();
    Tournament.Init();
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.LOGIN);
    TemporaryAccountManager.Get().Initialize();
    TemporaryAccountManager.Get().UpdateTemporaryAccountData();
    if (PlatformSettings.IsMobile())
    {
      AdTrackingManager.Get().TrackLogin();
      if (TemporaryAccountManager.IsTemporaryAccount())
        CloudStorageManager.Get().StartInitialize(new CloudStorageManager.OnInitializedFinished(this.OnCloudStorageInitialized), GameStrings.Get("GLUE_CLOUD_STORAGE_CONTEXT_BODY_01"));
      else if (PushNotificationManager.Get().CanRegisterPushAtLogin())
        PushNotificationManager.Get().RegisterPushNotifications();
      else
        PushNotificationManager.Get().UnregisterPushNotifications();
    }
    SceneMgr.Get().LoadShaderPreCompiler();
  }

  private void OnProfileProgressResponse()
  {
    HearthstoneApplication.SendStartupTimeTelemetry("LoginManager.OnProfileProgressResponse");
    if (!Options.Get().GetBool(Option.HAS_SEEN_NEW_CINEMATIC, false) && PlatformSettings.OS == OSCategory.PC)
      ServiceManager.Get<Cinematic>().Play((System.Action) (() => this.ReadyToReconnectOrChangeModeDependency.Callback()));
    else
      this.ReadyToReconnectOrChangeModeDependency.Callback();
  }

  private void UpdateTutorialPresence()
  {
    BnetPresenceMgr.Get().SetGameField(15U, GameUtils.IsTraditionalTutorialComplete() ? 1 : 0);
    BnetPresenceMgr.Get().SetGameField(28U, GameUtils.IsBattleGroundsTutorialComplete() ? 1 : 0);
    BnetPresenceMgr.Get().SetGameField(29U, GameUtils.IsMercenariesVillageTutorialComplete() ? 1 : 0);
  }

  private void OnNetCacheReady()
  {
    Log.Login.Print("LoginManager: Net Cache Ready");
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    this.LoginScreenNetCacheReceivedDependency.Callback();
    HearthstoneApplication.SendStartupTimeTelemetry("LoginManager.OnNetCacheReady");
    Processor.QueueJob("LoginManager.WaitForAchievesThenInit", this.Job_WaitForAchievesThenInit(), ServiceManager.CreateServiceDependency(typeof (SceneMgr)), (IJobDependency) new WaitForBox());
  }

  private void OnAssetsVersionResponse()
  {
    HearthstoneApplication.SendStartupTimeTelemetry("LoginManager.OnAssetsVersionResponse");
    AssetsVersionResponse assetsVersion = Network.Get().GetAssetsVersion();
    if (assetsVersion == null || !assetsVersion.HasReturningPlayerInfo)
      return;
    ReturningPlayerMgr.Get().SetReturningPlayerInfo(assetsVersion.ReturningPlayerInfo);
  }

  private void OnInitialClientStateResponse()
  {
    Log.Login.Print("LoginManager: Assets Version Check Completed");
    this.InitialClientStateReceivedDependency.Callback();
    HearthstoneApplication.SendStartupTimeTelemetry("LoginManager.OnInitialClientStateResponse");
    if (this.OnInitialClientStateReceived != null)
      this.OnInitialClientStateReceived();
    if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null)
      Box.Get().OnLoggedIn();
    BaseUI.Get().OnLoggedIn();
    InactivePlayerKicker.Get().OnLoggedIn();
    HealthyGamingMgr.Get().OnLoggedIn();
    GameMgr.Get().OnLoggedIn();
    DraftManager.Get().OnLoggedIn();
    AccountLicenseMgr.Get().InitRequests();
    AdventureProgressMgr.InitRequests();
    Network network = Network.Get();
    if (Network.IsLoggedIn())
    {
      TutorialProgress tutorialProgress = Options.Get().GetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS);
      if (tutorialProgress > TutorialProgress.NOTHING_COMPLETE)
        network.SetProgress((long) tutorialProgress);
      else
        this.SetProgressDependency.Callback();
    }
    network.ResetConnectionFailureCount();
    network.DoLoginUpdate();
    Processor.QueueJob("LoginManager.WaitForStartupPacketSequenceComplete", this.Job_WaitForStartupPacketSequenceComplete(), (IJobDependency) this.SetProgressDependency, (IJobDependency) this.UpdateLoginCompleteDependency);
  }

  private void OnSetProgressResponse()
  {
    HearthstoneApplication.SendStartupTimeTelemetry("LoginManager.OnSetProgressResponse");
    SetProgressResponse progressResponse = Network.Get().GetSetProgressResponse();
    switch (progressResponse.Result_)
    {
      case SetProgressResponse.Result.SUCCESS:
      case SetProgressResponse.Result.ALREADY_DONE:
        Options.Get().DeleteOption(Option.LOCAL_TUTORIAL_PROGRESS);
        break;
      default:
        Debug.LogWarning((object) string.Format("LoginManager.OnSetProgressResponse(): received unexpected result {0}", (object) progressResponse.Result_));
        break;
    }
    this.SetProgressDependency.Callback();
  }

  private void OnCloudStorageInitialized()
  {
    if (!PushNotificationManager.Get().CanRegisterPushAtLogin())
      return;
    if (TemporaryAccountManager.Get().IsSelectedTemporaryAccountMinor())
      PushNotificationManager.Get().UnregisterPushNotifications();
    else
      PushNotificationManager.Get().RegisterPushNotifications();
  }

  private void OnLoginComplete()
  {
    this.LoggedInDependency.Callback();
    Log.Login.Print("LoginManager: OnLoginComplete");
    HearthstoneApplication.SendStartupTimeTelemetry("LoginManager.OnLoginComplete");
    if (PlatformSettings.IsMobile())
      Processor.QueueJob("SetupMobilePushRegistration", this.Job_SetupMobilePushRegistration());
    HsAppsFlyer.SetCustomerUserId(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:D}", (object) BattleNet.GetMyAccoundId().Low));
    DefLoader.Get().Initialize();
    CollectionManager.Init();
    Log.Login.Print("LoginManager: CollectionManager is initialized.");
    InnKeepersSpecial.Get().InitializeURLAndUpdate();
    InGameMessageScheduler.Get()?.OnLoginCompleted();
    ExceptionReporterControl.Get().OnLoginCompleted();
    Processor.QueueJob("LoginManager.SetupOptIns", this.Job_SetupOptIns(), ServiceManager.CreateServiceDependency(typeof (APIGatewayService)));
    StoreManager.Get().Init();
    Network network = Network.Get();
    network.LoginOk();
    network.MercenariesPlayerInfoRequest();
    network.UpdateCachedBnetValues();
    Log.Login.Print("LoginManager: Requesting assets version and initial client state.");
    network.RequestAssetsVersion();
    NetCache.Get().RegisterScreenStartup(new NetCache.NetCacheCallback(this.OnProfileProgressResponse));
    System.Action onLoginCompleted = this.OnLoginCompleted;
    if (onLoginCompleted == null)
      return;
    onLoginCompleted();
  }

  private IEnumerator<IAsyncJobResult> Job_SetupMobilePushRegistration()
  {
    GenerateSSOToken tassadarAuthenticationToken = new GenerateSSOToken();
    yield return (IAsyncJobResult) tassadarAuthenticationToken;
    while (!tassadarAuthenticationToken.HasToken)
      yield return (IAsyncJobResult) null;
    Log.Login.Print("LoginManager: Setting up mobile push registration...");
    PushNotificationManager.Get().SetPushRegistrationInfo(tassadarAuthenticationToken.Token, BattleNet.GetMyAccoundId().Low, ExternalUrlService.GetRegionString(), Localization.GetLocaleName());
  }

  private IEnumerator<IAsyncJobResult> Job_SetupOptIns()
  {
    APIGatewayService apiGatewayService = ServiceManager.Get<APIGatewayService>();
    apiGatewayService.OnLoginComplete();
    this.OptInApi = new OptInApi(apiGatewayService, (Blizzard.T5.Core.ILogger) Log.BattleNet);
    this.OptInApi.Init(this.OptInsReceivedDependency.Callback);
    yield return (IAsyncJobResult) null;
  }

  private IEnumerator<IAsyncJobResult> Job_WaitForAchievesThenInit()
  {
    while (DownloadableDbfCache.Get().IsRequiredClientStaticAssetsStillPending)
      yield return (IAsyncJobResult) null;
    while (!AdventureProgressMgr.Get().IsReady)
      yield return (IAsyncJobResult) null;
    FixedRewardsMgr.Get().InitStartupFixedRewards();
    bool flag = false;
    float startTime = Time.realtimeSinceStartup;
    FSGFeatureConfig netObject1;
    int num;
    for (; !flag; flag = num == 0 || netObject1 != null)
    {
      yield return (IAsyncJobResult) null;
      if ((double) Time.realtimeSinceStartup - (double) startTime <= 10.0)
      {
        NetCache.NetCacheFeatures netObject2 = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
        num = netObject2 == null ? 0 : (netObject2.FSGEnabled ? 1 : 0);
        netObject1 = NetCache.Get().GetNetObject<FSGFeatureConfig>();
      }
      else
        break;
    }
    if (this.OnAchievesLoaded != null)
      this.OnAchievesLoaded();
    Log.Login.Print("LoginManager: Achieves Loaded");
    Log.Downloader.Print("LOADING PROCESS COMPLETE at " + (object) Time.realtimeSinceStartup);
  }

  public IEnumerator<IAsyncJobResult> ShowIntroPopups()
  {
    Log.Login.Print("LoginManager: Showing Intro Popups");
    yield return (IAsyncJobResult) new JobDefinition("DialogManager.WaitForSeasonEndPopup", DialogManager.Get().Job_WaitForSeasonEndPopup(), Array.Empty<IJobDependency>());
    yield return (IAsyncJobResult) new JobDefinition("PopupDisplayManager.WaitForAllPopups", PopupDisplayManager.Get().Job_WaitForAllPopups(), Array.Empty<IJobDependency>());
    yield return (IAsyncJobResult) new JobDefinition("NarrativeManager.WaitForOutstandingCharacterDialog", NarrativeManager.Get().Job_WaitForOutstandingCharacterDialog(), Array.Empty<IJobDependency>());
    yield return (IAsyncJobResult) new JobDefinition("LoginManager.ShowBreakingNews", this.Job_ShowBreakingNews(), Array.Empty<IJobDependency>());
  }

  public bool AttemptToReconnectToGame(ReconnectMgr.TimeoutCallback timeoutCallback)
  {
    if (GameMgr.Get().ConnectToGameIfHaveDeferredConnectionPacket())
      return true;
    if (!ReconnectMgr.Get().ReconnectToGameFromLogin())
      return false;
    ReconnectMgr.Get().AddTimeoutListener(timeoutCallback);
    return true;
  }

  private IEnumerator<IAsyncJobResult> Job_ShowBreakingNews()
  {
    if (this.m_breakingNews.ShouldShowForCurrentPlatform || Cheats.ShowFakeBreakingNews)
    {
      WaitForCallback waitForCallback = new WaitForCallback();
      this.ShowBreakingNews(waitForCallback.Callback);
      yield return (IAsyncJobResult) waitForCallback;
    }
  }

  public IEnumerator<IAsyncJobResult> CompleteLoginFlow()
  {
    this.IsFullLoginFlowComplete = true;
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.LOGIN))
    {
      if (this.OnFullLoginFlowComplete != null)
        this.OnFullLoginFlowComplete();
      Log.Login.Print("LoginManager: Complete Login Flow");
      this.ReadyToGoToNextModeDependency.Callback();
      yield break;
    }
  }

  private void ShowBreakingNews(System.Action callback)
  {
    if (this.m_breakingNews.GetStatus() == Hearthstone.BreakingNews.BreakingNews.Status.Available || Cheats.ShowFakeBreakingNews)
    {
      string str = this.m_breakingNews.GetText();
      if (string.IsNullOrEmpty(str) && Cheats.ShowFakeBreakingNews)
      {
        str = "FAKE BREAKING NEWS ARE BREAKING NOW";
        UIStatus.Get().AddInfo("SHOWING FAKE BREAKING NEWS!\nTo disable this, remove ShowFakeBreakingNews from client.config", 5f);
      }
      if (!string.IsNullOrEmpty(str))
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_MOBILE_SPLASH_SCREEN_BREAKING_NEWS"),
          m_text = str,
          m_showAlertIcon = true,
          m_richTextEnabled = false,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK,
          m_responseCallback = (AlertPopup.ResponseCallback) ((_param1, _param2) => callback())
        });
      else
        callback();
    }
    else
    {
      if (!Application.isEditor)
        Debug.LogWarning((object) "Breaking News response is taking too long!");
      callback();
    }
  }

  private void QueueInfoHandler(Network.QueueInfo queueInfo)
  {
    this.CurrentQueueInfo = queueInfo;
    if (this.OnQueueModifiedEvent == null)
      return;
    this.OnQueueModifiedEvent(queueInfo);
  }

  public void RegisterQueueModifiedListener(System.Action<Network.QueueInfo> listener)
  {
    this.OnQueueModifiedEvent -= listener;
    this.OnQueueModifiedEvent += listener;
  }

  public void RemoveQueueModifiedListener(System.Action<Network.QueueInfo> listener) => this.OnQueueModifiedEvent -= listener;
}
