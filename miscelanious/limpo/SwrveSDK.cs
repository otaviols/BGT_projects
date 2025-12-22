using SwrveUnity;
using SwrveUnity.Device;
using SwrveUnity.Helpers;
using SwrveUnity.Input;
using SwrveUnity.Messaging;
using SwrveUnity.ResourceManager;
using SwrveUnity.REST;
using SwrveUnity.Storage;
using SwrveUnity.SwrveUsers;
using SwrveUnityMiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SwrveSDK
{
  public const string SdkVersion = "7.3.2";
  protected int appId;
  protected string apiKey;
  internal SwrveConfig config;
  public string Language;
  public SwrveResourceManager ResourceManager;
  protected ISwrveAssetsManager SwrveAssetsManager;
  public MonoBehaviour Container;
  public bool Initialised;
  public bool Destroyed;
  private const string Platform = "Unity ";
  private const float DefaultDPI = 160f;
  protected const string EventsSave = "Swrve_Events";
  protected const string SwrveTrackingState = "SwrveSdkState";
  protected const string AppInstallTimeSecondsSave = "Swrve_JoinedDate";
  protected const string UserJoinedTimeSecondsSave = "Swrve_InitTimeDate";
  protected const string iOSdeviceTokenSave = "Swrve_iOSDeviceToken";
  protected const string FirebaseDeviceTokenSave = "Swrve_gcmDeviceToken";
  protected const string AdmDeviceTokenSave = "Swrve_admDeviceToken";
  protected const string WindowsDeviceTokenSave = "Swrve_windowsDeviceToken";
  protected const string GoogleAdvertisingIdSave = "Swrve_googleAdvertisingId";
  protected const string AbTestUserResourcesSave = "srcngt2";
  protected const string AbTestUserResourcesDiffSave = "rsdfngt2";
  protected const string RealtimeUserPropertiesSave = "rupp2";
  protected const string DeviceUUID = "Swrve_Device_UUID";
  protected const string SeqNumSave = "Swrve_SeqNum";
  protected const string ResourcesCampaignTagSave = "cmpg_etag";
  protected const string ResourcesCampaignFlushFrequencySave = "swrve_cr_flush_frequency";
  protected const string ResourcesCampaignFlushDelaySave = "swrve_cr_flush_delay";
  private const string EmptyJSONObject = "{}";
  private const float DefaultCampaignResourcesFlushFrenquency = 60f;
  private const float DefaultCampaignResourcesFlushRefreshDelay = 5f;
  public const string DefaultAutoShowMessagesTrigger = "Swrve.Messages.showAtSessionStart";
  private const string PushTrackingKey = "_p";
  private const string SilentPushTrackingKey = "_sp";
  private const string PushDeeplinkKey = "_sd";
  private const string PushContentKey = "_sw";
  private const int PushContentVersion = 1;
  private const string PushNestedJsonKey = "_s.JsonPayload";
  private const string PushButtonToCampaignIdKey = "PUSH_BUTTON_TO_CAMPAIGN_ID";
  private const string PushUnityDoNotProcessKey = "SWRVE_UNITY_DO_NOT_PROCESS";
  private SwrveSDK.SwrveSdkState trackingState;
  private long installTimeSeconds;
  private string installTimeSecondsFormatted;
  private long userInitTimeSeconds;
  private string lastPushEngagedId;
  private int deviceWidth;
  private int deviceHeight;
  private long lastSessionTick;
  private ICarrierInfo deviceCarrierInfo;
  protected StringBuilder eventBufferStringBuilder;
  protected string eventsPostString;
  protected string swrvePath;
  protected ISwrveStorage storage;
  protected SwrveProfileManager profileManager;
  internal IRESTClient restClient;
  private string eventsUrl;
  private string identifyUrl;
  private string abTestResourcesDiffUrl;
  protected bool eventsConnecting;
  protected bool abTestUserResourcesDiffConnecting;
  protected string userResourcesRaw;
  protected Dictionary<string, Dictionary<string, string>> userResources;
  protected string realtimeUserPropertiesRaw;
  protected Dictionary<string, string> realtimeUserProperties;
  protected float campaignsAndResourcesFlushFrequency;
  protected float campaignsAndResourcesFlushRefreshDelay;
  protected string lastETag;
  protected long campaignsAndResourcesLastRefreshed;
  protected bool campaignsAndResourcesInitialized;
  protected static readonly int CampaignEndpointVersion = 8;
  protected static readonly int EmbeddedCampaignVersion = 1;
  private static readonly int CampaignResponseVersion = 2;
  protected static readonly string CampaignsSave = "cmcc2";
  protected static readonly string LastExternalCampaignSave = "cmcc3";
  protected static readonly string CampaignsSettingsSave = "Swrve_CampaignsData";
  private static readonly string WaitTimeFormat = "HH\\:mm\\:ss zzz";
  protected static readonly string InstallTimeFormat = "yyyyMMdd";
  private string resourcesAndCampaignsUrl;
  protected string swrveTemporaryPath;
  protected bool campaignsConnecting;
  protected bool autoShowMessagesEnabled;
  protected Dictionary<int, SwrveCampaignState> campaignsState = new Dictionary<int, SwrveCampaignState>();
  protected List<SwrveBaseCampaign> campaigns = new List<SwrveBaseCampaign>();
  protected Dictionary<string, object> campaignSettings = new Dictionary<string, object>();
  protected Dictionary<string, string> appStoreLinks = new Dictionary<string, string>();
  protected SwrveMessageFormat currentMessage;
  protected SwrveMessageFormat currentDisplayingMessage;
  protected SwrveMessageRenderer messageRenderer;
  protected SwrveOrientation currentOrientation;
  protected IInputManager inputManager = (IInputManager) NativeInputManager.Instance;
  protected string prefabName;
  protected bool sdkStarted;
  private bool applicationPaused;
  private const int DefaultDelayFirstMessage = 150;
  private const long DefaultMaxShows = 99999;
  private const int DefaultMinDelay = 55;
  private DateTime initialisedTime;
  private DateTime showMessagesAfterLaunch;
  private DateTime showMessagesAfterDelay;
  private long messagesLeftToShow;
  private int minDelayBetweenMessage;
  internal List<SwrveBaseCampaign> campaignDisplayQueue = new List<SwrveBaseCampaign>();
  protected SwrveDeeplinkManager deeplinkManager;
  private bool campaignAndResourcesCoroutineEnabled = true;
  private IEnumerator campaignAndResourcesCoroutineInstance;
  private int conversationVersion;

  private void setNativeInfo(Dictionary<string, string> deviceInfo)
  {
  }

  private string getNativeLanguage() => (string) null;

  private void setNativeAppVersion()
  {
  }

  private void showNativeConversation(string conversation)
  {
  }

  private void setNativeConversationVersion()
  {
  }

  private bool NativeIsBackPressed() => false;

  private bool IsConversationDisplaying() => false;

  private void initNative()
  {
  }

  public string ApiKey => this.apiKey;

  public string UserId => this.profileManager.userId;

  public virtual void Init(MonoBehaviour container, int appId, string apiKey, SwrveConfig config = null)
  {
    if (config == null)
      config = new SwrveConfig();
    this.Container = container;
    this.ResourceManager = new SwrveResourceManager();
    this.config = config;
    this.prefabName = container.name;
    this.appId = appId;
    this.apiKey = apiKey;
    this.Language = config.Language;
    this.profileManager = new SwrveProfileManager(config.InitMode);
    this.swrvePath = SwrveSDK.GetSwrvePath();
    this.storage = this.CreateStorage();
    this.storage.SetSecureFailedListener((Action) (() => this.NamedEventInternal("Swrve.signature_invalid", allowShowMessage: false)));
    this.swrveTemporaryPath = SwrveSDK.GetSwrveTemporaryCachePath();
    this.InitAssetsManager(container, this.swrveTemporaryPath);
    new SwrveMigrationsManager(this.storage, this.profileManager).CheckMigrations();
    string s = this.storage.Load("Swrve_JoinedDate");
    if (string.IsNullOrEmpty(s))
    {
      this.installTimeSeconds = this.GetSessionTime();
      this.storage.Save("Swrve_JoinedDate", this.userInitTimeSeconds.ToString());
    }
    else
      long.TryParse(s, out this.installTimeSeconds);
    this.installTimeSecondsFormatted = SwrveHelper.EpochToFormat(this.installTimeSeconds, SwrveSDK.InstallTimeFormat);
    if (string.IsNullOrEmpty(apiKey))
      throw new Exception("The api key has not been specified.");
    if (string.IsNullOrEmpty(this.Language))
    {
      this.Language = this.GetDeviceLanguage();
      if (string.IsNullOrEmpty(this.Language))
        this.Language = config.DefaultLanguage;
    }
    config.CalculateEndpoints(appId);
    string contentServer = config.ContentServer;
    this.eventsUrl = config.EventsServer + "/1/batch";
    this.identifyUrl = config.IdentityServer + "/identify";
    this.abTestResourcesDiffUrl = contentServer + "/api/1/user_resources_diff";
    this.resourcesAndCampaignsUrl = contentServer + "/api/1/user_content";
    this.eventBufferStringBuilder = new StringBuilder(config.MaxBufferChars);
    this.restClient = this.CreateRestClient();
    if (config.InitMode == SwrveInitMode.AUTO)
      SwrveQaUser.Init(this.Container, config.EventsServer, apiKey, appId, this.UserId, this.GetAppVersion(), this.GetDeviceUUID(), this.storage);
    if (SwrveHelper.IsOnDevice())
      this.InitNative();
    this.sdkStarted = this.ShouldAutoStart();
    if (!this.sdkStarted)
      return;
    this.InitUser();
    this.ProcessInfluenceData();
    if (!config.MessagingEnabled)
      return;
    if (string.IsNullOrEmpty(this.Language))
      throw new Exception("Language needed to use messaging");
    if (string.IsNullOrEmpty(config.AppStore))
      throw new Exception("App store must be apple, google, amazon or a custom app store");
  }

  protected virtual void InitAssetsManager(MonoBehaviour container, string swrveTemporaryPath) => this.SwrveAssetsManager = (ISwrveAssetsManager) new SwrveUnity.SwrveAssetsManager(container, swrveTemporaryPath);

  public virtual void SessionStart()
  {
    this.QueueSessionStart();
    this.SendQueuedEvents();
  }

  public virtual void UserUpdate(Dictionary<string, string> attributes)
  {
    if (!this.IsSDKReady())
      return;
    if (attributes != null && attributes.Count > 0)
      this.AppendEventToBuffer("user", new Dictionary<string, object>()
      {
        {
          nameof (attributes),
          (object) attributes
        }
      });
    else
      SwrveLog.LogError((object) "Invoked user update with no update attributes");
  }

  public virtual bool SendQueuedEvents()
  {
    if (this.trackingState == SwrveSDK.SwrveSdkState.EVENT_SENDING_PAUSED)
    {
      this.LogTrackingState();
      return false;
    }
    bool flag = false;
    if (!this.eventsConnecting)
    {
      byte[] eventsPostEncodedData = (byte[]) null;
      if (this.eventsPostString == null || this.eventsPostString.Length == 0)
      {
        this.eventsPostString = this.eventBufferStringBuilder.ToString();
        this.eventBufferStringBuilder.Length = 0;
      }
      if (this.eventsPostString.Length > 0)
      {
        long sessionTime = this.GetSessionTime();
        eventsPostEncodedData = PostBodyBuilder.BuildEvent(this.apiKey, this.appId, this.UserId, this.GetDeviceUUID(), this.GetAppVersion(), sessionTime, this.eventsPostString);
      }
      if (eventsPostEncodedData != null)
      {
        this.eventsConnecting = true;
        SwrveLog.Log((object) "Sending events to Swrve");
        Dictionary<string, string> requestHeaders = new Dictionary<string, string>()
        {
          {
            "Content-Type",
            "application/json; charset=utf-8"
          }
        };
        flag = true;
        this.StartTask("PostEvents_Coroutine", this.PostEvents_Coroutine(requestHeaders, eventsPostEncodedData));
      }
      else
        this.eventsPostString = (string) null;
    }
    else
      SwrveLog.LogWarning((object) "Sending events already in progress");
    return flag;
  }

  public virtual void LoadFromDisk() => this.LoadEventsFromDisk();

  public virtual void FlushToDisk(bool saveEventsBeingSent = false)
  {
    if (this.trackingState == SwrveSDK.SwrveSdkState.EVENT_SENDING_PAUSED)
    {
      this.LogTrackingState();
    }
    else
    {
      if (!this.Initialised || this.eventBufferStringBuilder == null)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = this.eventBufferStringBuilder.ToString();
      this.eventBufferStringBuilder.Length = 0;
      if (saveEventsBeingSent)
      {
        stringBuilder.Append(this.eventsPostString);
        this.eventsPostString = (string) null;
        if (str1.Length > 0)
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append(",");
          stringBuilder.Append(str1);
        }
      }
      else
        stringBuilder.Append(str1);
      try
      {
        string str2 = this.storage.Load("Swrve_Events", this.UserId);
        if (!string.IsNullOrEmpty(str2))
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append(",");
          stringBuilder.Append(str2);
        }
      }
      catch (Exception ex)
      {
        SwrveLog.LogWarning((object) ("Could not read events from cache (" + ex.ToString() + ")"));
      }
      this.storage.Save("Swrve_Events", stringBuilder.ToString(), this.UserId);
    }
  }

  public virtual Dictionary<string, string> GetDeviceInfo()
  {
    string deviceModel = this.GetDeviceModel();
    string operatingSystem = SystemInfo.operatingSystem;
    string platformOs = this.GetPlatformOS();
    float num = (double) Screen.dpi == 0.0 ? 160f : Screen.dpi;
    Dictionary<string, string> deviceInfo = new Dictionary<string, string>()
    {
      {
        "swrve.device_name",
        deviceModel
      },
      {
        "swrve.os",
        platformOs
      },
      {
        "swrve.device_width",
        this.deviceWidth.ToString()
      },
      {
        "swrve.device_height",
        this.deviceHeight.ToString()
      },
      {
        "swrve.device_dpi",
        num.ToString()
      },
      {
        "swrve.language",
        this.Language
      },
      {
        "swrve.os_version",
        operatingSystem
      },
      {
        "swrve.app_store",
        this.config.AppStore
      },
      {
        "swrve.sdk_version",
        "Unity 7.3.2"
      },
      {
        "swrve.unity_version",
        Application.unityVersion
      },
      {
        "swrve.install_date",
        this.installTimeSecondsFormatted
      },
      {
        "swrve.device_type",
        this.GetDeviceType()
      }
    };
    string str = DateTimeOffset.Now.Offset.TotalSeconds.ToString();
    deviceInfo["swrve.utc_offset_seconds"] = str;
    this.setNativeInfo(deviceInfo);
    ICarrierInfo carrierInfoProvider = this.GetCarrierInfoProvider();
    if (carrierInfoProvider != null)
    {
      string name = carrierInfoProvider.GetName();
      if (!string.IsNullOrEmpty(name))
        deviceInfo["swrve.sim_operator.name"] = name;
      string isoCountryCode = carrierInfoProvider.GetIsoCountryCode();
      if (!string.IsNullOrEmpty(isoCountryCode))
        deviceInfo["swrve.sim_operator.iso_country_code"] = isoCountryCode;
      string carrierCode = carrierInfoProvider.GetCarrierCode();
      if (!string.IsNullOrEmpty(carrierCode))
        deviceInfo["swrve.sim_operator.code"] = carrierCode;
    }
    return deviceInfo;
  }

  public virtual void OnSwrvePause()
  {
    if (!this.IsSDKReady())
      return;
    this.applicationPaused = true;
    if (!this.Initialised)
      return;
    this.FlushToDisk();
    this.GenerateNewSessionInterval();
    if (this.config == null || !this.config.AutoDownloadCampaignsAndResources)
      return;
    this.StopCheckForCampaignAndResources();
  }

  public virtual void OnSwrveResume()
  {
    this.applicationPaused = false;
    if (!this.Initialised || !this.IsSDKReady())
      return;
    this.LoadFromDisk();
    this.QueueDeviceInfo();
    if (this.GetSessionTime() >= this.lastSessionTick)
      this.SessionStart();
    else
      this.SendQueuedEvents();
    this.GenerateNewSessionInterval();
    this.StartCampaignsAndResourcesTimer();
    this.DisableAutoShowAfterDelay();
    this.ProcessInfluenceData();
    if (!this.IsMessageDisplaying())
      this.ConversationClosed();
    this.DownloadAnyMissingAssets();
  }

  public virtual void OnSwrveDestroy()
  {
    if (this.Destroyed)
      return;
    this.Destroyed = true;
    if (this.Initialised)
      this.FlushToDisk(true);
    if (this.config == null || !this.config.AutoDownloadCampaignsAndResources)
      return;
    this.StopCheckForCampaignAndResources();
  }

  public virtual void ButtonWasPressedByUser(SwrveButton button)
  {
    if (button == null)
      return;
    try
    {
      SwrveLog.Log((object) ("Button " + (object) button.ActionType + ": " + button.Action + " app id: " + (object) button.AppId));
      if (button.ActionType == SwrveActionType.Dismiss)
        return;
      string name = "Swrve.Messages.Message-" + (object) button.Message.Id + ".click";
      SwrveLog.Log((object) ("Sending click event: " + name));
      this.NamedEventInternal(name, new Dictionary<string, string>()
      {
        {
          "name",
          button.Name
        },
        {
          "embedded",
          "false"
        }
      }, false);
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ("Error while processing button click " + (object) ex));
    }
  }

  public virtual void MessageWasShownToUser(SwrveMessageFormat messageFormat)
  {
    try
    {
      this.SetMessageMinDelayThrottle();
      --this.messagesLeftToShow;
      SwrveInAppCampaign campaign = (SwrveInAppCampaign) messageFormat.Message.Campaign;
      if (campaign != null)
      {
        campaign.MessageWasShownToUser(messageFormat);
        this.SaveCampaignData((SwrveBaseCampaign) campaign);
      }
      Dictionary<string, string> payload = new Dictionary<string, string>();
      payload.Add("embedded", "false");
      string name = "Swrve.Messages.Message-" + (object) messageFormat.Message.Id + ".impression";
      SwrveLog.Log((object) ("Sending view event: " + name));
      this.NamedEventInternal(name, payload, false);
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ("Error while processing message impression " + (object) ex));
    }
  }

  public virtual bool IsMessageDisplaying() => this.currentMessage != null;

  public virtual SwrveBaseMessage GetBaseMessageForEvent(
    string eventName,
    IDictionary<string, string> eventPayload = null)
  {
    if (!this.IsSDKReady())
      return (SwrveBaseMessage) null;
    if (!this.checkGlobalRules(eventName, eventPayload, SwrveHelper.GetNow()))
      return (SwrveBaseMessage) null;
    try
    {
      return this._getBaseMessageForEvent(eventName, eventPayload);
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ex.ToString(), "message");
    }
    return (SwrveBaseMessage) null;
  }

  private SwrveBaseMessage _getBaseMessageForEvent(
    string eventName,
    IDictionary<string, string> eventPayload)
  {
    SwrveBaseMessage baseMessageForEvent = (SwrveBaseMessage) null;
    SwrveBaseCampaign swrveBaseCampaign = (SwrveBaseCampaign) null;
    SwrveLog.Log((object) ("Trying to get message for: " + eventName));
    IEnumerator<SwrveBaseCampaign> enumerator1 = (IEnumerator<SwrveBaseCampaign>) this.campaigns.GetEnumerator();
    List<SwrveBaseMessage> swrveBaseMessageList = new List<SwrveBaseMessage>();
    int num = int.MaxValue;
    List<SwrveBaseMessage> list = new List<SwrveBaseMessage>();
    SwrveOrientation deviceOrientation = this.GetDeviceOrientation();
    List<SwrveQaUserCampaignInfo> userCampaignInfoList = new List<SwrveQaUserCampaignInfo>();
    while (enumerator1.MoveNext() && baseMessageForEvent == null)
    {
      if (enumerator1.Current is SwrveInAppCampaign || enumerator1.Current is SwrveEmbeddedCampaign)
      {
        SwrveBaseCampaign current = enumerator1.Current;
        SwrveBaseMessage swrveBaseMessage = (SwrveBaseMessage) null;
        if (current is SwrveEmbeddedCampaign)
          swrveBaseMessage = (SwrveBaseMessage) ((SwrveEmbeddedCampaign) current).GetMessageForEvent(eventName, eventPayload, userCampaignInfoList);
        else if (current is SwrveInAppCampaign)
          swrveBaseMessage = (SwrveBaseMessage) ((SwrveInAppCampaign) current).GetMessageForEvent(eventName, eventPayload, userCampaignInfoList);
        if (swrveBaseMessage != null)
        {
          if (swrveBaseMessage.SupportsOrientation(deviceOrientation))
          {
            swrveBaseMessageList.Add(swrveBaseMessage);
            if (swrveBaseMessage.Priority <= num)
            {
              if (swrveBaseMessage.Priority < num)
                list.Clear();
              num = swrveBaseMessage.Priority;
              list.Add(swrveBaseMessage);
            }
          }
          else
          {
            string reason = "Message didn't support the current device orientation: " + (object) deviceOrientation;
            SwrveQaUserCampaignInfo userCampaignInfo = new SwrveQaUserCampaignInfo((long) current.Id, (long) swrveBaseMessage.Id, current.GetCampaignType(), false, reason);
            userCampaignInfoList.Add(userCampaignInfo);
          }
        }
      }
    }
    if (list.Count > 0)
    {
      list.Shuffle<SwrveBaseMessage>();
      baseMessageForEvent = list[0];
      swrveBaseCampaign = baseMessageForEvent.Campaign;
    }
    if (SwrveQaUser.Instance.loggingEnabled && swrveBaseCampaign != null && baseMessageForEvent != null)
    {
      IEnumerator<SwrveBaseMessage> enumerator2 = (IEnumerator<SwrveBaseMessage>) swrveBaseMessageList.GetEnumerator();
      while (enumerator2.MoveNext())
      {
        SwrveBaseCampaign campaign = enumerator2.Current.Campaign;
        if (campaign != baseMessageForEvent.Campaign)
        {
          int id1 = campaign.Id;
          int id2 = enumerator2.Current.Id;
          string reason = "Campaign " + (object) swrveBaseCampaign.Id + " was selected for display ahead of this campaign";
          SwrveQaUserCampaignInfo userCampaignInfo = new SwrveQaUserCampaignInfo((long) id1, (long) id2, campaign.GetCampaignType(), false, reason);
          userCampaignInfoList.Add(userCampaignInfo);
        }
        SwrveQaUserCampaignInfo userCampaignInfo1 = new SwrveQaUserCampaignInfo((long) swrveBaseCampaign.Id, (long) baseMessageForEvent.Id, swrveBaseCampaign.GetCampaignType(), true);
        userCampaignInfoList.Add(userCampaignInfo1);
      }
    }
    SwrveQaUser.CampaignTriggeredMessage(eventName, eventPayload, baseMessageForEvent != null, userCampaignInfoList);
    return baseMessageForEvent;
  }

  public virtual SwrveConversation GetConversationForEvent(
    string eventName,
    IDictionary<string, string> eventPayload = null)
  {
    if (!this.IsSDKReady())
      return (SwrveConversation) null;
    if (!this.checkGlobalRules(eventName, eventPayload, SwrveHelper.GetNow()))
      return (SwrveConversation) null;
    try
    {
      return this._getConversationForEvent(eventName, eventPayload);
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ex.ToString(), SwrveQaUserCampaignInfo.SwrveCampaignType.Conversation.Value);
    }
    return (SwrveConversation) null;
  }

  private SwrveConversation _getConversationForEvent(
    string eventName,
    IDictionary<string, string> eventPayload = null)
  {
    SwrveConversation conversationForEvent1 = (SwrveConversation) null;
    SwrveBaseCampaign swrveBaseCampaign = (SwrveBaseCampaign) null;
    SwrveLog.Log((object) ("Trying to get conversation for: " + eventName));
    IEnumerator<SwrveBaseCampaign> enumerator1 = (IEnumerator<SwrveBaseCampaign>) this.campaigns.GetEnumerator();
    List<SwrveConversation> swrveConversationList = new List<SwrveConversation>();
    int num = int.MaxValue;
    List<SwrveConversation> list = new List<SwrveConversation>();
    List<SwrveQaUserCampaignInfo> userCampaignInfoList = new List<SwrveQaUserCampaignInfo>();
    while (enumerator1.MoveNext() && conversationForEvent1 == null)
    {
      if (enumerator1.Current is SwrveConversationCampaign)
      {
        SwrveConversation conversationForEvent2 = ((SwrveConversationCampaign) enumerator1.Current).GetConversationForEvent(eventName, eventPayload, userCampaignInfoList);
        if (conversationForEvent2 != null)
        {
          swrveConversationList.Add(conversationForEvent2);
          if (conversationForEvent2.Priority <= num)
          {
            if (conversationForEvent2.Priority < num)
              list.Clear();
            num = conversationForEvent2.Priority;
            list.Add(conversationForEvent2);
          }
        }
      }
    }
    if (list.Count > 0)
    {
      list.Shuffle<SwrveConversation>();
      conversationForEvent1 = list[0];
      swrveBaseCampaign = conversationForEvent1.Campaign;
    }
    if (SwrveQaUser.Instance.loggingEnabled && swrveBaseCampaign != null && conversationForEvent1 != null)
    {
      IEnumerator<SwrveConversation> enumerator2 = (IEnumerator<SwrveConversation>) swrveConversationList.GetEnumerator();
      while (enumerator2.MoveNext())
      {
        SwrveBaseCampaign campaign = enumerator2.Current.Campaign;
        if (campaign != conversationForEvent1.Campaign)
        {
          int id1 = campaign.Id;
          int id2 = enumerator2.Current.Id;
          string reason = "Campaign " + (object) swrveBaseCampaign.Id + " was selected for display ahead of this campaign";
          SwrveQaUserCampaignInfo userCampaignInfo = new SwrveQaUserCampaignInfo((long) id1, (long) id2, campaign.GetCampaignType(), false, reason);
          userCampaignInfoList.Add(userCampaignInfo);
        }
        SwrveQaUserCampaignInfo userCampaignInfo1 = new SwrveQaUserCampaignInfo((long) swrveBaseCampaign.Id, (long) conversationForEvent1.Id, conversationForEvent1.Campaign.GetCampaignType(), true);
        userCampaignInfoList.Add(userCampaignInfo1);
      }
    }
    SwrveQaUser.CampaignTriggeredConversation(eventName, eventPayload, conversationForEvent1 != null, userCampaignInfoList);
    return conversationForEvent1;
  }

  private bool checkGlobalRules(
    string eventName,
    IDictionary<string, string> eventPayload,
    DateTime now)
  {
    if (this.campaigns == null || this.campaigns.Count == 0)
    {
      this.NoMessagesWereShown(eventName, eventPayload, "No campaigns available");
      return false;
    }
    if (!string.Equals(eventName, "Swrve.Messages.showAtSessionStart", StringComparison.OrdinalIgnoreCase) && this.IsTooSoonToShowMessageAfterLaunch(now))
    {
      this.NoMessagesWereShown(eventName, eventPayload, "{App throttle limit} Too soon after launch. Wait until " + this.showMessagesAfterLaunch.ToString(SwrveSDK.WaitTimeFormat));
      return false;
    }
    if (this.IsTooSoonToShowMessageAfterDelay(now))
    {
      this.NoMessagesWereShown(eventName, eventPayload, "{App throttle limit} Too soon after last base message. Wait until " + this.showMessagesAfterDelay.ToString(SwrveSDK.WaitTimeFormat));
      return false;
    }
    if (!this.HasShowTooManyMessagesAlready())
      return true;
    this.NoMessagesWereShown(eventName, eventPayload, "{App throttle limit} Too many base messages shown");
    return false;
  }

  public virtual IEnumerator ShowMessageForEvent(
    string eventName,
    IDictionary<string, string> payload,
    SwrveBaseMessage message,
    ISwrveInstallButtonListener installButtonListener = null,
    ISwrveCustomButtonListener customButtonListener = null,
    ISwrveMessageListener messageListener = null,
    ISwrveClipboardButtonListener clipboardButtonListener = null,
    ISwrveEmbeddedMessageListener embeddedMessageListener = null)
  {
    if (!this.IsSDKReady())
      yield return (object) null;
    switch (message)
    {
      case SwrveMessage _:
        if (this.config.TriggeredMessageListener != null)
        {
          if (message != null)
          {
            this.config.TriggeredMessageListener.OnMessageTriggered((SwrveMessage) message);
            break;
          }
          break;
        }
        if (this.currentMessage == null)
        {
          Dictionary<string, string> properties = (Dictionary<string, string>) null;
          if (this.config.InAppMessageConfig != null && this.config.InAppMessageConfig.PersonalizationProvider != null)
            properties = this.config.InAppMessageConfig.PersonalizationProvider.Personalize(payload);
          yield return (object) this.Container.StartCoroutine(this.LaunchMessage(message, installButtonListener, customButtonListener, clipboardButtonListener, messageListener, properties));
          break;
        }
        break;
      case SwrveEmbeddedMessage _ when this.config.EmbeddedMessageConfig.EmbeddedMessageListener != null && message != null:
        this.config.EmbeddedMessageConfig.EmbeddedMessageListener.OnMessage((SwrveEmbeddedMessage) message);
        break;
    }
    this.TaskFinished(nameof (ShowMessageForEvent));
  }

  public virtual IEnumerator ShowConversationForEvent(
    string eventName,
    SwrveConversation conversation)
  {
    if (!this.IsSDKReady())
      yield return (object) null;
    yield return (object) this.Container.StartCoroutine(this.LaunchConversation(conversation));
    this.TaskFinished(nameof (ShowConversationForEvent));
  }

  public virtual void DismissMessage()
  {
    if (!this.IsSDKReady())
      return;
    if (this.config.TriggeredMessageListener != null)
    {
      this.config.TriggeredMessageListener.DismissCurrentMessage();
    }
    else
    {
      try
      {
        if (this.currentMessage == null)
          return;
        this.SetMessageMinDelayThrottle();
        this.currentMessage.Dismiss();
      }
      catch (Exception ex)
      {
        SwrveLog.LogError((object) ("Error while dismissing a message " + (object) ex));
      }
    }
  }

  public virtual void RefreshUserResourcesAndCampaigns()
  {
    if (!this.IsSDKReady())
      return;
    this.LoadResourcesAndCampaigns();
  }

  internal DateTime GetInitialisedTime() => this.initialisedTime;

  internal bool IsSDKReady()
  {
    if (this.config.InitMode != SwrveInitMode.MANAGED || this.sdkStarted)
      return true;
    SwrveLog.LogWarning((object) "Warning: SwrveSDK needs to be started in MANAGED mode before calling this api.");
    return false;
  }

  private void EnableEventSending()
  {
    this.trackingState = SwrveSDK.SwrveSdkState.ON;
    this.StartCampaignsAndResourcesTimer();
  }

  private void LogTrackingState()
  {
    switch (this.trackingState)
    {
      case SwrveSDK.SwrveSdkState.ON:
        SwrveLog.LogInfo((object) "SDK tracking state is ON");
        break;
      case SwrveSDK.SwrveSdkState.EVENT_SENDING_PAUSED:
        SwrveLog.LogInfo((object) "SDK tracking state is EVENT_SENDING_PAUSED");
        break;
    }
  }

  private void QueueSessionStart() => this.AppendEventToBuffer("session_start", new Dictionary<string, object>());

  protected void NamedEventInternal(
    string name,
    Dictionary<string, string> payload = null,
    bool allowShowMessage = true)
  {
    if (payload == null)
      payload = new Dictionary<string, string>();
    this.AppendEventToBuffer("event", new Dictionary<string, object>()
    {
      {
        nameof (name),
        (object) name
      },
      {
        nameof (payload),
        (object) payload
      }
    }, allowShowMessage);
  }

  protected static string GetSwrvePath()
  {
    string swrvePath = Application.persistentDataPath;
    if (string.IsNullOrEmpty(swrvePath))
    {
      swrvePath = Application.temporaryCachePath;
      SwrveLog.Log((object) ("Swrve path (tried again): " + swrvePath));
    }
    return swrvePath;
  }

  protected static string GetSwrveTemporaryCachePath()
  {
    string path = Application.temporaryCachePath;
    if (path == null || path.Length == 0)
      path = Application.persistentDataPath;
    if (!File.Exists(path))
      Directory.CreateDirectory(path);
    return path;
  }

  internal virtual SwrveOrientation GetDeviceOrientation()
  {
    switch (Screen.orientation)
    {
      case ScreenOrientation.Portrait:
      case ScreenOrientation.PortraitUpsideDown:
        return SwrveOrientation.Portrait;
      case ScreenOrientation.LandscapeLeft:
      case ScreenOrientation.LandscapeRight:
        return SwrveOrientation.Landscape;
      default:
        return Screen.height >= Screen.width ? SwrveOrientation.Portrait : SwrveOrientation.Landscape;
    }
  }

  private Dictionary<string, Dictionary<string, string>> ProcessUserResources(
    IList<object> userResources)
  {
    Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
    if (userResources != null)
    {
      IEnumerator<object> enumerator = userResources.GetEnumerator();
      while (enumerator.MoveNext())
      {
        Dictionary<string, object> current = (Dictionary<string, object>) enumerator.Current;
        string key = (string) current["uid"];
        dictionary.Add(key, this.NormalizeJson(current));
      }
    }
    return dictionary;
  }

  private Dictionary<string, string> NormalizeJson(Dictionary<string, object> json)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    Dictionary<string, object>.Enumerator enumerator = json.GetEnumerator();
    while (enumerator.MoveNext())
    {
      KeyValuePair<string, object> current = enumerator.Current;
      if (current.Value != null)
        dictionary.Add(current.Key, current.Value.ToString());
    }
    return dictionary;
  }

  private void ProcessUserResourcesDiff(
    string abTestJson,
    Dictionary<string, Dictionary<string, string>> newResources,
    Dictionary<string, Dictionary<string, string>> oldResources)
  {
    IList<object> objectList = (IList<object>) Json.Deserialize(abTestJson);
    if (objectList == null)
      return;
    IEnumerator<object> enumerator1 = objectList.GetEnumerator();
    while (enumerator1.MoveNext())
    {
      Dictionary<string, object> current = (Dictionary<string, object>) enumerator1.Current;
      string key = (string) current["uid"];
      Dictionary<string, object> dictionary1 = (Dictionary<string, object>) current["diff"];
      IEnumerator<string> enumerator2 = (IEnumerator<string>) dictionary1.Keys.GetEnumerator();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
      while (enumerator2.MoveNext())
      {
        Dictionary<string, string> dictionary4 = this.NormalizeJson((Dictionary<string, object>) dictionary1[enumerator2.Current]);
        dictionary2.Add(enumerator2.Current, dictionary4["new"]);
        dictionary3.Add(enumerator2.Current, dictionary4["old"]);
      }
      newResources.Add(key, dictionary2);
      oldResources.Add(key, dictionary3);
    }
  }

  private string GetDeviceUUID()
  {
    string data = this.storage.Load("Swrve_Device_UUID");
    if (string.IsNullOrEmpty(data))
    {
      data = SwrveHelper.GetRandomUUID();
      this.storage.Save("Swrve_Device_UUID", data);
    }
    return data;
  }

  private void BeginSession()
  {
    this.EnableEventSending();
    this.DisableAutoShowAfterDelay();
    if (this.config.AutomaticSessionManagement)
    {
      this.QueueSessionStart();
      this.GenerateNewSessionInterval();
    }
    if (this.profileManager.isNewUser)
      this.NamedEventInternal("Swrve.first_session", allowShowMessage: false);
    this.QueueDeviceInfo();
    this.StartCampaignsAndResourcesTimer();
    this.SendQueuedEvents();
  }

  private bool ShouldAutoStart() => this.config.InitMode == SwrveInitMode.AUTO || this.config.InitMode == SwrveInitMode.MANAGED && this.config.ManagedModeAutoStartLastUser && !string.IsNullOrEmpty(this.profileManager.userId);

  private void InitUser()
  {
    this.lastSessionTick = SwrveHelper.GetMilliseconds();
    this.initialisedTime = SwrveHelper.GetNow();
    this.showMessagesAfterDelay = this.initialisedTime;
    this.autoShowMessagesEnabled = true;
    this.trackingState = SwrveSDK.SwrveSdkState.ON;
    this.CheckUserTimes();
    this.LoadData();
    if (this.config.ABTestDetailsEnabled)
    {
      try
      {
        this.LoadABTestDetails();
      }
      catch (Exception ex)
      {
        SwrveLog.LogError((object) ("Error while initializing " + (object) ex));
      }
    }
    this.InitUserResources();
    this.InitRealtimeUserProperties();
    this.deviceCarrierInfo = (ICarrierInfo) new DeviceCarrierInfo();
    this.GetDeviceScreenInfo();
    this.Initialised = true;
    if (this.config.MessagingEnabled)
      this.LoadTalkData();
    this.BeginSession();
  }

  private void CheckUserTimes()
  {
    string s = this.storage.Load("Swrve_InitTimeDate", this.UserId);
    if (string.IsNullOrEmpty(s))
    {
      this.profileManager.isNewUser = true;
      this.userInitTimeSeconds = this.GetSessionTime();
      this.storage.Save("Swrve_InitTimeDate", this.userInitTimeSeconds.ToString(), this.UserId);
    }
    else
    {
      this.profileManager.isNewUser = false;
      long.TryParse(s, out this.userInitTimeSeconds);
    }
  }

  private string GetNextSeqNum()
  {
    int result;
    int num;
    string data = int.TryParse(this.storage.Load("Swrve_SeqNum", this.UserId), out result) ? (num = result + 1).ToString() : "1";
    this.storage.Save("Swrve_SeqNum", data, this.UserId);
    return data;
  }

  protected string GetDeviceLanguage()
  {
    string deviceLanguage = this.getNativeLanguage();
    if (string.IsNullOrEmpty(deviceLanguage))
    {
      string lower = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
      if (lower != "iv")
        deviceLanguage = lower;
    }
    return deviceLanguage;
  }

  protected void InvalidateETag()
  {
    this.lastETag = string.Empty;
    this.storage.Remove("cmpg_etag", this.UserId);
  }

  private void InitUserResources()
  {
    this.userResourcesRaw = this.storage.LoadSecure("srcngt2", this.UserId);
    if (!string.IsNullOrEmpty(this.userResourcesRaw))
    {
      this.userResources = this.ProcessUserResources((IList<object>) Json.Deserialize(this.userResourcesRaw));
      this.NotifyUpdateUserResources();
    }
    else
      this.InvalidateETag();
  }

  private void InitRealtimeUserProperties()
  {
    this.realtimeUserPropertiesRaw = this.storage.LoadSecure("rupp2", this.UserId);
    if (!string.IsNullOrEmpty(this.realtimeUserPropertiesRaw))
      this.realtimeUserProperties = this.NormalizeJson((Dictionary<string, object>) Json.Deserialize(this.realtimeUserPropertiesRaw));
    else
      this.InvalidateETag();
  }

  private void NotifyUpdateUserResources()
  {
    if (this.userResources == null)
      return;
    this.ResourceManager.SetResourcesFromJSON(this.userResources);
    if (this.config.ResourcesUpdatedCallback == null)
      return;
    this.config.ResourcesUpdatedCallback();
  }

  private void LoadEventsFromDisk()
  {
    try
    {
      string str = this.storage.Load("Swrve_Events", this.UserId);
      this.storage.Remove("Swrve_Events", this.UserId);
      if (string.IsNullOrEmpty(str))
        return;
      if (this.eventBufferStringBuilder.Length != 0)
        this.eventBufferStringBuilder.Insert(0, ",");
      this.eventBufferStringBuilder.Insert(0, str);
    }
    catch (Exception ex)
    {
      SwrveLog.LogWarning((object) ("Could not read events from cache (" + ex.ToString() + ")"));
    }
  }

  private void LoadData()
  {
    this.LoadEventsFromDisk();
    this.lastETag = this.storage.Load("cmpg_etag", this.UserId);
    string s1 = this.storage.Load("swrve_cr_flush_frequency", this.UserId);
    if (!string.IsNullOrEmpty(s1) && float.TryParse(s1, out this.campaignsAndResourcesFlushFrequency))
      this.campaignsAndResourcesFlushFrequency /= 1000f;
    if ((double) this.campaignsAndResourcesFlushFrequency == 0.0)
      this.campaignsAndResourcesFlushFrequency = 60f;
    string s2 = this.storage.Load("swrve_cr_flush_delay", this.UserId);
    if (!string.IsNullOrEmpty(s2) && float.TryParse(s2, out this.campaignsAndResourcesFlushRefreshDelay))
      this.campaignsAndResourcesFlushRefreshDelay /= 1000f;
    if ((double) this.campaignsAndResourcesFlushRefreshDelay != 0.0)
      return;
    this.campaignsAndResourcesFlushRefreshDelay = 5f;
  }

  protected string GetUniqueKey() => this.apiKey + this.UserId;

  protected virtual IRESTClient CreateRestClient() => (IRESTClient) new RESTClient();

  protected virtual ISwrveStorage CreateStorage() => this.config.StoreDataInPlayerPrefs ? (ISwrveStorage) new SwrvePlayerPrefsStorage() : (ISwrveStorage) new SwrveFileStorage(this.swrvePath, this.GetUniqueKey());

  private IEnumerator PostEvents_Coroutine(
    Dictionary<string, string> requestHeaders,
    byte[] eventsPostEncodedData)
  {
    yield return (object) this.Container.StartCoroutine(this.restClient.Post(this.eventsUrl, eventsPostEncodedData, requestHeaders, (Action<RESTResponse>) (response =>
    {
      if (response.Error != WwwDeducedError.NetworkError)
      {
        this.ClearEventBuffer();
        eventsPostEncodedData = (byte[]) null;
      }
      this.eventsConnecting = false;
      this.TaskFinished(nameof (PostEvents_Coroutine));
    })));
  }

  protected virtual void ClearEventBuffer() => this.eventsPostString = (string) null;

  private void AppendEventToBuffer(
    string eventType,
    Dictionary<string, object> eventParameters,
    bool allowShowMessage = true)
  {
    eventParameters.Add("type", (object) eventType);
    eventParameters.Add("seqnum", (object) this.GetNextSeqNum());
    eventParameters.Add("time", (object) this.GetSessionTime());
    string eventJson = Json.Serialize((object) eventParameters);
    string eventName = SwrveHelper.GetEventName(eventParameters);
    bool flag = this.eventBufferStringBuilder.Length + eventJson.Length <= this.config.MaxBufferChars;
    if (flag || this.config.SendEventsIfBufferTooLarge)
    {
      if (!flag && this.config.SendEventsIfBufferTooLarge)
        this.SendQueuedEvents();
      if (this.eventBufferStringBuilder.Length > 0)
        this.eventBufferStringBuilder.Append(',');
      this.AppendEventToBuffer(eventJson);
      SwrveQaUser.WrappedEvent(eventParameters);
    }
    else
      SwrveLog.LogError((object) "Could not append the event to the buffer. Please consider enabling SendEventsIfBufferTooLarge");
    if (!allowShowMessage)
      return;
    object payload;
    eventParameters.TryGetValue("payload", out payload);
    this.ShowBaseMessage(eventName, (IDictionary<string, string>) payload);
  }

  protected virtual void AppendEventToBuffer(string eventJson) => this.eventBufferStringBuilder.Append(eventJson);

  protected virtual Coroutine StartTask(string tag, IEnumerator task) => this.Container.StartCoroutine(task);

  protected virtual void TaskFinished(string tag)
  {
  }

  protected void ShowBaseMessage(string eventName, IDictionary<string, string> payload)
  {
    SwrveBaseMessage baseMessage = this.GetBaseMessage(eventName, payload);
    if (baseMessage == null)
      return;
    if (baseMessage is SwrveConversation)
      this.StartTask("ShowConversationForEvent", this.ShowConversationForEvent(eventName, (SwrveConversation) baseMessage));
    else
      this.StartTask("ShowMessageForEvent", this.ShowMessageForEvent(eventName, payload, baseMessage, this.config.InAppMessageInstallButtonListener, this.config.InAppMessageCustomButtonListener, this.config.InAppMessageListener, this.config.InAppMessageClipboardButtonListener, this.config.EmbeddedMessageConfig.EmbeddedMessageListener));
  }

  public SwrveBaseMessage GetBaseMessage(
    string eventName,
    IDictionary<string, string> eventPayload = null)
  {
    if (!this.checkGlobalRules(eventName, eventPayload, SwrveHelper.GetNow()))
      return (SwrveBaseMessage) null;
    SwrveBaseMessage baseMessage = (SwrveBaseMessage) null;
    if (this.config.MessagingEnabled)
      baseMessage = this.GetBaseMessageForEvent(eventName, eventPayload);
    if (baseMessage == null && this.config.ConversationsEnabled)
      baseMessage = (SwrveBaseMessage) this.GetConversationForEvent(eventName, eventPayload);
    else if (baseMessage != null && this.config.ConversationsEnabled)
      SwrveQaUser.CampaignTriggeredConversationNoDisplay(eventName, eventPayload);
    if (baseMessage == null)
      SwrveLog.Log((object) ("Not showing message: no candidate for " + eventName));
    else
      SwrveLog.Log((object) string.Format("[{0}] {1} has been chosen for {2}\nstate: {3}", (object) baseMessage, (object) baseMessage.Campaign.Id, (object) eventName, (object) baseMessage.Campaign.State));
    return baseMessage;
  }

  private bool IsAlive() => (UnityEngine.Object) this.Container != (UnityEngine.Object) null && !this.Destroyed;

  protected virtual void GetDeviceScreenInfo()
  {
    this.deviceWidth = Screen.width;
    this.deviceHeight = Screen.height;
    if (this.deviceWidth <= this.deviceHeight)
      return;
    int deviceWidth = this.deviceWidth;
    this.deviceWidth = this.deviceHeight;
    this.deviceHeight = deviceWidth;
  }

  private void QueueDeviceInfo()
  {
    Dictionary<string, string> deviceInfo = this.GetDeviceInfo();
    if (deviceInfo != null && deviceInfo.Count > 0)
      this.AppendEventToBuffer("device_update", new Dictionary<string, object>()
      {
        {
          "attributes",
          (object) deviceInfo
        }
      }, false);
    else
      SwrveLog.LogError((object) "Invoked user update with no update attributes");
  }

  private IEnumerator WaitAndRefreshResourcesAndCampaigns_Coroutine(float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    this.RefreshUserResourcesAndCampaigns();
  }

  private void CheckForCampaignsAndResourcesUpdates(bool invokedByTimer)
  {
    if (!this.IsAlive())
      return;
    if (this.SendQueuedEvents())
      this.Container.StartCoroutine(this.WaitAndRefreshResourcesAndCampaigns_Coroutine(this.campaignsAndResourcesFlushRefreshDelay));
    if (invokedByTimer)
      return;
    this.StopCheckForCampaignAndResources();
    this.StartCheckForCampaignsAndResources();
  }

  private void StartCheckForCampaignsAndResources()
  {
    if (this.campaignAndResourcesCoroutineInstance == null)
    {
      this.campaignAndResourcesCoroutineInstance = this.CheckForCampaignsAndResourcesUpdates_Coroutine();
      this.Container.StartCoroutine(this.campaignAndResourcesCoroutineInstance);
    }
    this.campaignAndResourcesCoroutineEnabled = true;
  }

  private void StopCheckForCampaignAndResources()
  {
    if (this.campaignAndResourcesCoroutineInstance != null)
    {
      this.Container.StopCoroutine("campaignAndResourcesCoroutineInstance");
      this.campaignAndResourcesCoroutineInstance = (IEnumerator) null;
    }
    this.campaignAndResourcesCoroutineEnabled = false;
  }

  private IEnumerator CheckForCampaignsAndResourcesUpdates_Coroutine()
  {
    yield return (object) new WaitForSeconds(this.campaignsAndResourcesFlushFrequency);
    this.CheckForCampaignsAndResourcesUpdates(true);
    if (this.campaignAndResourcesCoroutineEnabled)
    {
      this.campaignAndResourcesCoroutineInstance = (IEnumerator) null;
      this.StartCheckForCampaignsAndResources();
    }
  }

  protected virtual long GetSessionTime() => SwrveHelper.GetMilliseconds();

  private void GenerateNewSessionInterval() => this.lastSessionTick = this.GetSessionTime() + (long) (this.config.NewSessionInterval * 1000);

  public void Update()
  {
    if (this.currentDisplayingMessage == null)
      return;
    if (!this.currentMessage.Closing)
    {
      if (this.inputManager.GetMouseButtonDown(0))
        this.messageRenderer.ProcessButtonDown(this.inputManager);
      else if (this.inputManager.GetMouseButtonUp(0))
        this.ProcessButtonUp();
    }
    if (this.currentMessage.Closing || !this.NativeIsBackPressed())
      return;
    this.currentMessage.Dismiss();
  }

  public void OnGUI()
  {
    if (this.currentDisplayingMessage == null)
      return;
    SwrveOrientation deviceOrientation = this.GetDeviceOrientation();
    if (deviceOrientation != this.currentOrientation)
    {
      if (this.currentDisplayingMessage.Orientation != deviceOrientation)
      {
        if (this.currentDisplayingMessage.Message.SupportsOrientation(deviceOrientation))
          this.StartTask("SwitchMessageOrienation", this.SwitchMessageOrienation(deviceOrientation));
        else
          this.currentDisplayingMessage.Rotate = true;
      }
      else
        this.currentDisplayingMessage.Rotate = false;
    }
    int depth = GUI.depth;
    Matrix4x4 matrix = GUI.matrix;
    GUI.depth = 0;
    this.messageRenderer.DrawMessage(Screen.width, Screen.height);
    GUI.matrix = matrix;
    GUI.depth = depth;
    if (this.currentDisplayingMessage.MessageListener != null)
      this.currentDisplayingMessage.MessageListener.OnShowing(this.currentDisplayingMessage);
    if (this.currentMessage.Dismissed)
    {
      this.currentMessage = (SwrveMessageFormat) null;
      this.currentDisplayingMessage = (SwrveMessageFormat) null;
      this.messageRenderer = (SwrveMessageRenderer) null;
      this.HandleNextCampaign();
    }
    this.currentOrientation = deviceOrientation;
  }

  private IEnumerator SwitchMessageOrienation(SwrveOrientation newOrientation)
  {
    SwrveMessageFormat newFormat = this.currentMessage.Message.GetFormat(newOrientation);
    if (newFormat != null && newFormat != this.currentMessage)
    {
      SwrveMessageFormat oldFormat = this.currentMessage;
      CoroutineReference<bool> wereAllLoaded = new CoroutineReference<bool>(false);
      yield return (object) this.StartTask("PreloadFormatAssets", this.PreloadFormatAssets(newFormat, wereAllLoaded));
      if (wereAllLoaded.Value())
      {
        this.currentOrientation = this.GetDeviceOrientation();
        newFormat.MessageListener = oldFormat.MessageListener;
        newFormat.CustomButtonListener = oldFormat.CustomButtonListener;
        newFormat.InstallButtonListener = oldFormat.InstallButtonListener;
        newFormat.ClipboardButtonListener = oldFormat.ClipboardButtonListener;
        this.currentMessage = this.currentDisplayingMessage = newFormat;
        this.messageRenderer.InitMessage(newFormat, this.config.InAppMessageConfig, this.currentOrientation, true);
        oldFormat.UnloadAssets();
      }
      else
        SwrveLog.LogError((object) "Could not switch orientation. Not all assets could be preloaded");
      this.TaskFinished(nameof (SwitchMessageOrienation));
      oldFormat = (SwrveMessageFormat) null;
      wereAllLoaded = (CoroutineReference<bool>) null;
    }
  }

  private void ProcessButtonUp()
  {
    SwrveButtonClickResult buttonClickResult = this.messageRenderer.ProcessButtonUp(this.inputManager);
    if (buttonClickResult == null)
      return;
    SwrveButton button = buttonClickResult.Button;
    SwrveLog.Log((object) ("Clicked button " + (object) button.ActionType));
    this.ButtonWasPressedByUser(button);
    string actionType = this.QaActionType(button);
    SwrveQaUser.CampaignButtonClicked(button.Message.Campaign.Id, button.Message.Id, button.Name, actionType, button.Action);
    try
    {
      if (button.ActionType == SwrveActionType.Install)
      {
        string key = button.AppId.ToString();
        if (this.appStoreLinks.ContainsKey(key))
        {
          string appStoreLink = this.appStoreLinks[key];
          if (!string.IsNullOrEmpty(appStoreLink))
          {
            bool flag = true;
            if (this.currentMessage.InstallButtonListener != null)
              flag = this.currentMessage.InstallButtonListener.OnAction(appStoreLink);
            if (flag)
              this.OpenURL(appStoreLink);
          }
          else
            SwrveLog.LogError((object) ("No app store url for app " + key));
        }
        else
          SwrveLog.LogError((object) "Install button app store url empty!");
      }
      else if (button.ActionType == SwrveActionType.Custom)
      {
        string resolvedAction = buttonClickResult.ResolvedAction;
        if (this.currentMessage.CustomButtonListener != null)
        {
          this.currentMessage.CustomButtonListener.OnAction(resolvedAction);
        }
        else
        {
          SwrveLog.Log((object) "No custom button listener, treating action as URL");
          if (!string.IsNullOrEmpty(resolvedAction))
            this.OpenURL(resolvedAction);
        }
      }
      else if (button.ActionType == SwrveActionType.CopyToClipboard)
      {
        string resolvedAction = buttonClickResult.ResolvedAction;
        SwrveLog.Log((object) "Copying text to clipboard");
        if (!string.IsNullOrEmpty(resolvedAction))
          SwrveLog.Log((object) "Copy to clipboard is only implemented for Android and iOS");
        if (this.currentMessage.ClipboardButtonListener != null)
          this.currentMessage.ClipboardButtonListener.OnAction(resolvedAction);
      }
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ("Error processing the clicked button: " + ex.Message));
    }
    button.Pressed = false;
    this.DismissMessage();
  }

  protected virtual void OpenURL(string url) => Application.OpenURL(url);

  protected void SetMessageMinDelayThrottle() => this.showMessagesAfterDelay = SwrveHelper.GetNow() + TimeSpan.FromSeconds((double) this.minDelayBetweenMessage);

  public void ConversationClosed()
  {
    if (this.currentMessage != null)
      return;
    this.HandleNextCampaign();
  }

  internal void ShowCampaign(SwrveBaseCampaign campaign, bool isQueued) => this.ShowCampaign(campaign, isQueued, this.GetDeviceOrientation(), (Dictionary<string, string>) null);

  private void ShowCampaign(
    SwrveBaseCampaign campaign,
    bool isQueued,
    SwrveOrientation orientation,
    Dictionary<string, string> properties)
  {
    if (!this.IsMessageDisplaying() && !this.IsConversationDisplaying() && !this.applicationPaused)
    {
      switch (campaign)
      {
        case SwrveInAppCampaign _:
          this.Container.StartCoroutine(this.LaunchMessage((SwrveBaseMessage) ((SwrveInAppCampaign) campaign).Messages.Where<SwrveMessage>((Func<SwrveMessage, bool>) (a => a.SupportsOrientation(orientation))).First<SwrveMessage>(), this.config.InAppMessageInstallButtonListener, this.config.InAppMessageCustomButtonListener, this.config.InAppMessageClipboardButtonListener, this.config.InAppMessageListener, properties));
          break;
        case SwrveConversationCampaign _:
          this.Container.StartCoroutine(this.LaunchConversation(((SwrveConversationCampaign) campaign).Conversation));
          break;
        case SwrveEmbeddedCampaign _:
          SwrveEmbeddedMessage message = ((SwrveEmbeddedCampaign) campaign).Message;
          if (this.config.EmbeddedMessageConfig.EmbeddedMessageListener != null)
          {
            this.config.EmbeddedMessageConfig.EmbeddedMessageListener.OnMessage(message);
            break;
          }
          SwrveLog.LogError((object) "Could not find a valid EmbeddedMessageListener defined as part of the EmbeddedMessageConfig, be sure that you did set it as parf of the SDK initialisation");
          break;
      }
    }
    else
    {
      if (!isQueued)
        return;
      this.campaignDisplayQueue.Add(campaign);
    }
  }

  private void HandleNextCampaign()
  {
    if (this.campaignDisplayQueue.Count <= 0)
      return;
    SwrveBaseCampaign campaignDisplay = this.campaignDisplayQueue[0];
    this.campaignDisplayQueue.RemoveAt(0);
    this.ShowCampaign(campaignDisplay, false);
  }

  private void AutoShowMessages()
  {
    if (!this.autoShowMessagesEnabled || !this.campaignsAndResourcesInitialized || this.campaigns == null || this.campaigns.Count == 0)
      return;
    SwrveBaseMessage swrveBaseMessage1 = (SwrveBaseMessage) null;
    for (int index = 0; index < this.campaigns.Count; ++index)
    {
      if (this.campaigns[index] is SwrveConversationCampaign)
      {
        SwrveConversationCampaign campaign = (SwrveConversationCampaign) this.campaigns[index];
        if (campaign.CanTrigger("Swrve.Messages.showAtSessionStart") && campaign.CheckImpressions())
        {
          SwrveConversation conversationForEvent = this.GetConversationForEvent("Swrve.Messages.showAtSessionStart");
          if (campaign.AreAssetsReady())
          {
            this.autoShowMessagesEnabled = false;
            this.Container.StartCoroutine(this.LaunchConversation(conversationForEvent));
            swrveBaseMessage1 = (SwrveBaseMessage) conversationForEvent;
            break;
          }
        }
      }
    }
    if (swrveBaseMessage1 != null)
      return;
    for (int index = 0; index < this.campaigns.Count; ++index)
    {
      if (this.campaigns[index] is SwrveInAppCampaign || this.campaigns[index] is SwrveEmbeddedCampaign)
      {
        SwrveBaseCampaign campaign = this.campaigns[index];
        if (campaign.CanTrigger("Swrve.Messages.showAtSessionStart") && campaign.CheckImpressions())
        {
          SwrveBaseMessage baseMessageForEvent = this.GetBaseMessageForEvent("Swrve.Messages.showAtSessionStart");
          SwrveBaseMessage swrveBaseMessage2;
          switch (baseMessageForEvent)
          {
            case SwrveMessage _:
              if (this.config.TriggeredMessageListener != null)
              {
                if (baseMessageForEvent == null || !(baseMessageForEvent is SwrveMessage))
                  return;
                this.autoShowMessagesEnabled = false;
                this.config.TriggeredMessageListener.OnMessageTriggered((SwrveMessage) baseMessageForEvent);
                swrveBaseMessage2 = baseMessageForEvent;
                return;
              }
              if (this.currentMessage != null)
                return;
              this.autoShowMessagesEnabled = false;
              Dictionary<string, string> properties = (Dictionary<string, string>) null;
              if (this.config.InAppMessageConfig != null && this.config.InAppMessageConfig.PersonalizationProvider != null)
                properties = this.config.InAppMessageConfig.PersonalizationProvider.Personalize((IDictionary<string, string>) null);
              this.Container.StartCoroutine(this.LaunchMessage(baseMessageForEvent, this.config.InAppMessageInstallButtonListener, this.config.InAppMessageCustomButtonListener, this.config.InAppMessageClipboardButtonListener, this.config.InAppMessageListener, properties));
              swrveBaseMessage2 = baseMessageForEvent;
              return;
            case SwrveEmbeddedMessage _:
              if (this.currentMessage != null || this.config.EmbeddedMessageConfig.EmbeddedMessageListener == null)
                return;
              this.autoShowMessagesEnabled = false;
              this.config.EmbeddedMessageConfig.EmbeddedMessageListener.OnMessage((SwrveEmbeddedMessage) baseMessageForEvent);
              swrveBaseMessage2 = baseMessageForEvent;
              return;
            default:
              continue;
          }
        }
      }
    }
  }

  private IEnumerator LaunchMessage(
    SwrveBaseMessage message,
    ISwrveInstallButtonListener installButtonListener,
    ISwrveCustomButtonListener customButtonListener,
    ISwrveClipboardButtonListener clipboardButtonListener,
    ISwrveMessageListener messageListener,
    Dictionary<string, string> properties)
  {
    if (message != null && message is SwrveMessage)
    {
      SwrveOrientation deviceOrientation = this.GetDeviceOrientation();
      SwrveMessageFormat selectedFormat = ((SwrveMessage) message).GetFormat(deviceOrientation);
      if (selectedFormat != null)
      {
        SwrveMessageTextTemplatingResolver resolver = new SwrveMessageTextTemplatingResolver();
        if (resolver.ResolveTemplating((SwrveMessage) message, properties))
        {
          this.currentMessage = selectedFormat;
          CoroutineReference<bool> wereAllLoaded = new CoroutineReference<bool>(false);
          yield return (object) this.StartTask("PreloadFormatAssets", this.PreloadFormatAssets(selectedFormat, wereAllLoaded));
          if (wereAllLoaded.Value())
          {
            this.ShowMessageFormat(selectedFormat, installButtonListener, customButtonListener, clipboardButtonListener, messageListener, resolver);
          }
          else
          {
            SwrveLog.LogError((object) ("Could not preload all the assets for message " + (object) message.Id));
            this.currentMessage = (SwrveMessageFormat) null;
          }
          wereAllLoaded = (CoroutineReference<bool>) null;
        }
        resolver = (SwrveMessageTextTemplatingResolver) null;
      }
      else
        SwrveLog.LogError((object) ("Could not get a format for the current orientation: " + deviceOrientation.ToString()));
      selectedFormat = (SwrveMessageFormat) null;
    }
  }

  private IEnumerator LaunchConversation(SwrveConversation conversation)
  {
    if (conversation != null)
    {
      yield return (object) null;
      this.ShowConversation(conversation.Conversation);
      this.ConversationWasShownToUser(conversation);
    }
  }

  public void ConversationWasShownToUser(SwrveConversation conversation)
  {
    this.SetMessageMinDelayThrottle();
    if (conversation.Campaign == null)
      return;
    conversation.Campaign.WasShownToUser();
    this.SaveCampaignData(conversation.Campaign);
  }

  private void NoMessagesWereShown(
    string eventName,
    IDictionary<string, string> eventPayload,
    string reason)
  {
    SwrveLog.Log((object) ("Not showing message for " + eventName + ": " + reason));
  }

  private IEnumerator PreloadFormatAssets(
    SwrveMessageFormat format,
    CoroutineReference<bool> wereAllLoaded)
  {
    SwrveLog.Log((object) "Preloading format");
    bool allLoaded = true;
    int ii;
    CoroutineReference<Texture2D> result;
    for (ii = 0; ii < format.Images.Count; ++ii)
    {
      SwrveImage image = format.Images[ii];
      if ((UnityEngine.Object) image.Texture == (UnityEngine.Object) null && !string.IsNullOrEmpty(image.File))
      {
        SwrveLog.Log((object) ("Preloading image file " + image.File));
        result = new CoroutineReference<Texture2D>();
        yield return (object) this.StartTask("LoadAsset", this.LoadAsset(image.File, result));
        if ((UnityEngine.Object) result.Value() != (UnityEngine.Object) null)
          image.Texture = result.Value();
        else
          allLoaded = false;
        result = (CoroutineReference<Texture2D>) null;
      }
      image = (SwrveImage) null;
    }
    for (ii = 0; ii < format.Buttons.Count; ++ii)
    {
      SwrveButton button = format.Buttons[ii];
      if ((UnityEngine.Object) button.Texture == (UnityEngine.Object) null && !string.IsNullOrEmpty(button.Image))
      {
        SwrveLog.Log((object) ("Preloading button image " + button.Image));
        result = new CoroutineReference<Texture2D>();
        yield return (object) this.StartTask("LoadAsset", this.LoadAsset(button.Image, result));
        if ((UnityEngine.Object) result.Value() != (UnityEngine.Object) null)
          button.Texture = result.Value();
        else
          allLoaded = false;
        result = (CoroutineReference<Texture2D>) null;
      }
      button = (SwrveButton) null;
    }
    wereAllLoaded.Value(allLoaded);
    this.TaskFinished(nameof (PreloadFormatAssets));
  }

  private bool HasShowTooManyMessagesAlready() => this.messagesLeftToShow <= 0L;

  private bool IsTooSoonToShowMessageAfterLaunch(DateTime now) => now < this.showMessagesAfterLaunch;

  private bool IsTooSoonToShowMessageAfterDelay(DateTime now) => now < this.showMessagesAfterDelay;

  private SwrveMessageFormat ShowMessageFormat(
    SwrveMessageFormat format,
    ISwrveInstallButtonListener installButtonListener,
    ISwrveCustomButtonListener customButtonListener,
    ISwrveClipboardButtonListener clipboardButtonListener,
    ISwrveMessageListener messageListener,
    SwrveMessageTextTemplatingResolver templatingResolver)
  {
    this.currentMessage = this.currentDisplayingMessage = format;
    format.MessageListener = messageListener;
    format.CustomButtonListener = customButtonListener;
    format.InstallButtonListener = installButtonListener;
    format.ClipboardButtonListener = clipboardButtonListener;
    this.currentOrientation = this.GetDeviceOrientation();
    this.messageRenderer = new SwrveMessageRenderer(this.config.InAppMessageConfig.Animator, templatingResolver);
    this.messageRenderer.InitMessage(format, this.config.InAppMessageConfig, this.currentOrientation);
    messageListener?.OnShow(format);
    this.MessageWasShownToUser(this.currentDisplayingMessage);
    return format;
  }

  private string GetTemporaryPathFileName(string fileName) => Path.Combine(this.swrveTemporaryPath, fileName);

  private IEnumerator LoadAsset(string fileName, CoroutineReference<Texture2D> texture)
  {
    string filePath = this.GetTemporaryPathFileName(fileName);
    UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + filePath);
    yield return (object) www.SendWebRequest();
    if (!www.isNetworkError && !www.isHttpError)
    {
      texture.Value(((DownloadHandlerTexture) www.downloadHandler).texture);
    }
    else
    {
      SwrveLog.LogError((object) ("Could not load asset with WWW " + filePath + ": " + www.error));
      if (CrossPlatformFile.Exists(filePath))
      {
        byte[] data = CrossPlatformFile.ReadAllBytes(filePath);
        Texture2D texture2D = new Texture2D(4, 4);
        if (texture2D.LoadImage(data))
          texture.Value(texture2D);
        else
          SwrveLog.LogWarning((object) ("Could not load asset from I/O" + filePath));
      }
      else
        SwrveLog.LogError((object) ("The file " + filePath + " does not exist."));
    }
    this.TaskFinished(nameof (LoadAsset));
  }

  protected virtual void ProcessCampaigns(
    Dictionary<string, object> root,
    bool loadingPreviousCampaignState)
  {
    List<SwrveBaseCampaign> collection = new List<SwrveBaseCampaign>();
    HashSet<SwrveAssetsQueueItem> assetQueue = new HashSet<SwrveAssetsQueueItem>();
    HashSet<SwrveAssetsQueueItem> autoShowQueue = new HashSet<SwrveAssetsQueueItem>();
    try
    {
      if (root != null)
      {
        if (root.ContainsKey("version"))
        {
          if (MiniJsonHelper.GetInt(root, "version") == SwrveSDK.CampaignResponseVersion)
          {
            this.UpdateCdnPaths(root);
            Dictionary<string, object> dictionary1 = (Dictionary<string, object>) root["game_data"];
            Dictionary<string, object>.Enumerator enumerator = dictionary1.GetEnumerator();
            while (enumerator.MoveNext())
            {
              string key = enumerator.Current.Key;
              if (this.appStoreLinks.ContainsKey(key))
                this.appStoreLinks.Remove(key);
              Dictionary<string, object> dictionary2 = (Dictionary<string, object>) dictionary1[key];
              if (dictionary2 != null && dictionary2.ContainsKey("app_store_url"))
              {
                object obj = dictionary2["app_store_url"];
                if (obj != null && obj is string)
                  this.appStoreLinks.Add(key, (string) obj);
              }
            }
            Dictionary<string, object> json = (Dictionary<string, object>) root["rules"];
            int num1 = json.ContainsKey("delay_first_message") ? MiniJsonHelper.GetInt(json, "delay_first_message") : 150;
            long num2 = json.ContainsKey("max_messages_per_session") ? MiniJsonHelper.GetLong(json, "max_messages_per_session") : 99999L;
            int num3 = json.ContainsKey("min_delay_between_messages") ? MiniJsonHelper.GetInt(json, "min_delay_between_messages") : 55;
            DateTime now = SwrveHelper.GetNow();
            this.minDelayBetweenMessage = num3;
            this.messagesLeftToShow = num2;
            this.showMessagesAfterLaunch = this.initialisedTime + TimeSpan.FromSeconds((double) num1);
            SwrveLog.Log((object) ("App rules OK: Delay Seconds: " + (object) num1 + " Max shows: " + (object) num2));
            SwrveLog.Log((object) ("Time is " + now.ToString() + " show messages after " + this.showMessagesAfterLaunch.ToString()));
            IList<object> objectList = (IList<object>) root["campaigns"];
            List<SwrveQaUserCampaignInfo> userCampaignInfoList = new List<SwrveQaUserCampaignInfo>();
            int index1 = 0;
            for (int count = objectList.Count; index1 < count; ++index1)
            {
              SwrveBaseCampaign swrveBaseCampaign = SwrveBaseCampaign.LoadFromJSON(this.SwrveAssetsManager, (Dictionary<string, object>) objectList[index1], this.initialisedTime, this.config.DefaultBackgroundColor, userCampaignInfoList);
              if (swrveBaseCampaign != null)
              {
                bool flag = false;
                List<SwrveTrigger> triggers = swrveBaseCampaign.GetTriggers();
                for (int index2 = 0; index2 < triggers.Count; ++index2)
                {
                  if (string.Equals(triggers[index2].GetEventName(), "Swrve.Messages.showAtSessionStart"))
                  {
                    flag = true;
                    break;
                  }
                }
                switch (swrveBaseCampaign)
                {
                  case SwrveConversationCampaign _:
                    SwrveConversationCampaign conversationCampaign = (SwrveConversationCampaign) swrveBaseCampaign;
                    if (flag)
                      autoShowQueue.UnionWith((IEnumerable<SwrveAssetsQueueItem>) conversationCampaign.Conversation.ConversationAssets);
                    else
                      assetQueue.UnionWith((IEnumerable<SwrveAssetsQueueItem>) conversationCampaign.Conversation.ConversationAssets);
                    userCampaignInfoList.Add(new SwrveQaUserCampaignInfo((long) swrveBaseCampaign.Id, (long) conversationCampaign.Conversation.Id, conversationCampaign.GetCampaignType(), false));
                    break;
                  case SwrveInAppCampaign _:
                    SwrveInAppCampaign swrveInAppCampaign = (SwrveInAppCampaign) swrveBaseCampaign;
                    if (flag)
                      autoShowQueue.UnionWith((IEnumerable<SwrveAssetsQueueItem>) swrveInAppCampaign.GetImageAssets());
                    else
                      assetQueue.UnionWith((IEnumerable<SwrveAssetsQueueItem>) swrveInAppCampaign.GetImageAssets());
                    userCampaignInfoList.Add(new SwrveQaUserCampaignInfo((long) swrveBaseCampaign.Id, (long) swrveInAppCampaign.Messages[0].Id, swrveInAppCampaign.GetCampaignType(), false));
                    break;
                  case SwrveEmbeddedCampaign _:
                    SwrveEmbeddedCampaign embeddedCampaign = (SwrveEmbeddedCampaign) swrveBaseCampaign;
                    userCampaignInfoList.Add(new SwrveQaUserCampaignInfo((long) swrveBaseCampaign.Id, (long) embeddedCampaign.Message.Id, embeddedCampaign.GetCampaignType(), false));
                    break;
                }
                if (loadingPreviousCampaignState)
                {
                  SwrveCampaignState swrveCampaignState = (SwrveCampaignState) null;
                  this.campaignsState.TryGetValue(swrveBaseCampaign.Id, out swrveCampaignState);
                  if (swrveCampaignState != null)
                    swrveBaseCampaign.State = swrveCampaignState;
                  else if (this.campaignSettings != null)
                    swrveBaseCampaign.State = new SwrveCampaignState(swrveBaseCampaign.Id, this.campaignSettings);
                }
                this.campaignsState[swrveBaseCampaign.Id] = swrveBaseCampaign.State;
                collection.Add(swrveBaseCampaign);
              }
            }
            SwrveQaUser.CampaignsDownloaded(userCampaignInfoList);
          }
        }
      }
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ("Could not process campaigns: " + ex.ToString()));
    }
    this.StartTask("SwrveAssetsManager.DownloadAssets", this.SwrveAssetsManager.DownloadAssets(autoShowQueue, assetQueue, new Action(this.AutoShowMessages)));
    this.campaigns = new List<SwrveBaseCampaign>((IEnumerable<SwrveBaseCampaign>) collection);
  }

  internal void UpdateCdnPaths(Dictionary<string, object> root)
  {
    if (root.ContainsKey("cdn_root"))
    {
      string str = (string) root["cdn_root"];
      this.SwrveAssetsManager.CdnImages = str;
      SwrveLog.Log((object) ("CDN URL " + str));
    }
    else
    {
      if (!root.ContainsKey("cdn_paths"))
        return;
      Dictionary<string, object> dictionary = (Dictionary<string, object>) root["cdn_paths"];
      string str1 = (string) dictionary["message_images"];
      string str2 = (string) dictionary["message_fonts"];
      this.SwrveAssetsManager.CdnImages = str1;
      this.SwrveAssetsManager.CdnFonts = str2;
      SwrveLog.Log((object) ("CDN URL images:" + str1 + " fonts:" + str2));
    }
  }

  internal ISwrveAssetsManager GetSwrveAssetsManager() => this.SwrveAssetsManager;

  internal void DownloadAnyMissingAssets() => this.StartTask("SwrveAssetsManager.DownloadMissingAssets", this.SwrveAssetsManager.DownloadAnyMissingAssets(new Action(this.AutoShowMessages)));

  private void LoadResourcesAndCampaigns()
  {
    if (!this.IsAlive())
      return;
    try
    {
      if (this.campaignsConnecting)
        return;
      if (!this.config.AutoDownloadCampaignsAndResources)
      {
        if (this.campaignsAndResourcesLastRefreshed != 0L && this.GetSessionTime() < this.campaignsAndResourcesLastRefreshed)
        {
          SwrveLog.Log((object) "Request to retrieve campaign and user resource data was rate-limited.");
          return;
        }
        this.campaignsAndResourcesLastRefreshed = this.GetSessionTime() + (long) ((double) this.campaignsAndResourcesFlushFrequency * 1000.0);
      }
      this.campaignsConnecting = true;
      this.StartTask("GetCampaignsAndResources_Coroutine", this.GetCampaignsAndResources_Coroutine(this.GetCampaignsAndResourcesUrl(this.resourcesAndCampaignsUrl).ToString()));
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ("Error while trying to get user resources and campaign data: " + (object) ex));
    }
  }

  internal string GetCampaignsAndResourcesUrl(string endPoint)
  {
    float num = (double) Screen.dpi == 0.0 ? 160f : Screen.dpi;
    string deviceModel = this.GetDeviceModel();
    string platformOs = this.GetPlatformOS();
    string deviceType = this.GetDeviceType();
    string operatingSystem = SystemInfo.operatingSystem;
    StringBuilder stringBuilder = new StringBuilder(endPoint).AppendFormat("?user={0}&api_key={1}&app_version={2}&joined={3}", (object) SwrveHelper.EscapeURL(this.UserId), (object) this.ApiKey, (object) SwrveHelper.EscapeURL(this.GetAppVersion()), (object) this.userInitTimeSeconds);
    if (this.config.MessagingEnabled)
      stringBuilder.AppendFormat("&version={0}&orientation={1}&language={2}&app_store={3}&embedded_campaign_version={4}&device_width={5}&device_height={6}&device_dpi={7}&os_version={8}&device_name={9}&os={10}&device_type={11}", (object) SwrveSDK.CampaignEndpointVersion, (object) this.config.Orientation.ToString().ToLower(), (object) this.Language, (object) this.config.AppStore, (object) SwrveSDK.EmbeddedCampaignVersion, (object) this.deviceWidth, (object) this.deviceHeight, (object) num, (object) SwrveHelper.EscapeURL(operatingSystem), (object) SwrveHelper.EscapeURL(deviceModel), (object) platformOs, (object) deviceType);
    if (this.config.ConversationsEnabled)
      stringBuilder.AppendFormat("&conversation_version={0}", (object) this.conversationVersion);
    if (this.config.ABTestDetailsEnabled)
      stringBuilder.AppendFormat("&ab_test_details=1");
    if (!string.IsNullOrEmpty(this.lastETag))
      stringBuilder.AppendFormat("&etag={0}", (object) this.lastETag);
    return stringBuilder.ToString();
  }

  private string GetDeviceModel()
  {
    string deviceModel = SystemInfo.deviceModel;
    if (string.IsNullOrEmpty(deviceModel))
      deviceModel = "ModelUnknown";
    return deviceModel;
  }

  private string GetPlatformOS() => "pc";

  private string GetDeviceType() => "desktop";

  protected virtual IEnumerator GetCampaignsAndResources_Coroutine(string getRequest)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    SwrveSDK swrveSdk = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    SwrveLog.Log((object) ("Campaigns and resources request: " + getRequest));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u003E2__current = (object) swrveSdk.Container.StartCoroutine(swrveSdk.restClient.Get(getRequest, new Action<RESTResponse>(swrveSdk.\u003CGetCampaignsAndResources_Coroutine\u003Eb__268_0)));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void SaveCampaignsCache(string cacheContent)
  {
    try
    {
      if (cacheContent == null)
        cacheContent = string.Empty;
      this.storage.SaveSecure(SwrveSDK.CampaignsSave, cacheContent, this.UserId);
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ("Error while saving campaigns to the cache " + (object) ex));
    }
  }

  internal void SaveExternalCampaignCache(string cacheContent)
  {
    try
    {
      if (cacheContent == null)
        cacheContent = string.Empty;
      this.storage.SaveSecure(SwrveSDK.LastExternalCampaignSave, cacheContent, this.UserId);
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ("Error while saving last external campaign to the cache " + (object) ex));
    }
  }

  private void SaveCampaignData(SwrveBaseCampaign campaign)
  {
    try
    {
      this.campaignSettings["Next" + (object) campaign.Id] = (object) campaign.Next;
      this.campaignSettings["Impressions" + (object) campaign.Id] = (object) campaign.Impressions;
      this.campaignSettings["Status" + (object) campaign.Id] = (object) campaign.Status.ToString();
      string data = Json.Serialize((object) this.campaignSettings);
      this.storage.Save(SwrveSDK.CampaignsSettingsSave, data, this.UserId);
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ("Error while trying to save campaign settings " + (object) ex));
    }
  }

  private void LoadTalkData()
  {
    try
    {
      string data = this.storage.Load(SwrveSDK.CampaignsSettingsSave, this.UserId);
      if (data != null)
      {
        if (data.Length != 0)
        {
          string decodedString;
          if (ResponseBodyTester.TestUTF8(data, out decodedString))
            this.campaignSettings = (Dictionary<string, object>) Json.Deserialize(decodedString);
        }
      }
    }
    catch (Exception ex)
    {
      SwrveLog.LogWarning((object) ("Could not read default campaign settings." + ex.ToString()));
    }
    try
    {
      string data = this.storage.LoadSecure(SwrveSDK.CampaignsSave, this.UserId);
      if (!string.IsNullOrEmpty(data))
      {
        string decodedString = (string) null;
        if (ResponseBodyTester.TestUTF8(data, out decodedString))
        {
          this.ProcessCampaigns((Dictionary<string, object>) Json.Deserialize(decodedString), !SwrveQaUser.Instance.resetDevice);
        }
        else
        {
          SwrveLog.Log((object) "Failed to parse campaigns cache");
          this.InvalidateETag();
        }
      }
      else
        this.InvalidateETag();
    }
    catch (Exception ex)
    {
      SwrveLog.LogWarning((object) ("Could not read campaigns from cache, using default (" + ex.ToString() + ")"));
      this.InvalidateETag();
    }
  }

  private void LoadABTestDetails()
  {
    try
    {
      string data = this.storage.LoadSecure(SwrveSDK.CampaignsSave, this.UserId);
      if (string.IsNullOrEmpty(data))
        return;
      string decodedString = (string) null;
      if (ResponseBodyTester.TestUTF8(data, out decodedString))
      {
        Dictionary<string, object> dictionary = (Dictionary<string, object>) Json.Deserialize(decodedString);
        if (!dictionary.ContainsKey("ab_test_details"))
          return;
        this.ResourceManager.SetABTestDetailsFromJSON((Dictionary<string, object>) dictionary["ab_test_details"]);
      }
      else
        SwrveLog.Log((object) "Failed to parse AB test details cache");
    }
    catch (Exception ex)
    {
      SwrveLog.LogWarning((object) ("Could not read ABTest details from cache, using default (" + ex.ToString() + ")"));
    }
  }

  private IEnumerator WaitASecondAndSendEvents_Coroutine()
  {
    yield return (object) new WaitForSeconds(1f);
    this.SendQueuedEvents();
  }

  protected virtual ICarrierInfo GetCarrierInfoProvider() => this.deviceCarrierInfo;

  public string GetAppVersion()
  {
    if (string.IsNullOrEmpty(this.config.AppVersion))
      this.setNativeAppVersion();
    return this.config.AppVersion;
  }

  private void ShowConversation(string conversation) => this.showNativeConversation(conversation);

  protected void StartCampaignsAndResourcesTimer()
  {
    if (!this.config.AutoDownloadCampaignsAndResources)
      return;
    this.RefreshUserResourcesAndCampaigns();
    this.StartCheckForCampaignsAndResources();
    this.Container.StartCoroutine(this.WaitAndRefreshResourcesAndCampaigns_Coroutine(this.campaignsAndResourcesFlushRefreshDelay));
  }

  protected void DisableAutoShowAfterDelay() => this.Container.StartCoroutine(this.DisableAutoShowAfterDelay_Coroutine());

  private IEnumerator DisableAutoShowAfterDelay_Coroutine()
  {
    yield return (object) new WaitForSeconds(this.config.AutoShowMessagesMaxDelay);
    this.autoShowMessagesEnabled = false;
  }

  private void InitNative()
  {
    this.initNative();
    this.setNativeConversationVersion();
  }

  private void ProcessInfluenceData()
  {
    string dataJsonPerPlatform = this.GetInfluencedDataJsonPerPlatform();
    if (dataJsonPerPlatform == null)
      return;
    List<object> objectList = (List<object>) Json.Deserialize(dataJsonPerPlatform);
    if (objectList != null)
    {
      for (int index = 0; index < objectList.Count; ++index)
        this.CheckInfluenceData((Dictionary<string, object>) objectList[index]);
    }
    else
      SwrveLog.LogError((object) "Could not parse influence data");
  }

  protected virtual string GetInfluencedDataJsonPerPlatform() => (string) null;

  public void CheckInfluenceData(Dictionary<string, object> influenceData)
  {
    if (influenceData == null)
      return;
    object obj1 = influenceData["trackingId"];
    object obj2 = influenceData["maxInfluencedMillis"];
    object obj3 = influenceData["silent"];
    long num = 0;
    switch (obj2)
    {
      case long _:
      case int _:
      case long _:
        num = (long) obj2;
        break;
    }
    if (obj1 == null || !(obj1 is string) || num <= 0L)
      return;
    string str = (string) obj1;
    long milliseconds = SwrveHelper.GetMilliseconds();
    if (milliseconds > num)
      return;
    this.AppendEventToBuffer("generic_campaign_event", new Dictionary<string, object>()
    {
      {
        "id",
        (object) str
      },
      {
        "campaignType",
        (object) "push"
      },
      {
        "actionType",
        (object) "influenced"
      },
      {
        "payload",
        (object) new Dictionary<string, string>()
        {
          {
            "delta",
            ((num - milliseconds) / 6000L).ToString()
          },
          {
            "silent",
            obj3.ToString().ToLower()
          }
        }
      }
    }, false);
    SwrveLog.Log((object) ("User was influenced by push " + str));
    this.Container.StartCoroutine(this.WaitASecondAndSendEvents_Coroutine());
  }

  private void UpdateQaUser(Dictionary<string, object> qaUserDictionary)
  {
    if (qaUserDictionary == null)
    {
      qaUserDictionary = new Dictionary<string, object>();
      qaUserDictionary.Add("reset_device_state", (object) false);
      qaUserDictionary.Add("logging", (object) false);
    }
    SwrveQaUser.SaveQaUser(qaUserDictionary);
    SwrveQaUser.Update(qaUserDictionary);
  }

  protected string QaActionType(SwrveButton button)
  {
    string str = "";
    switch (button.ActionType)
    {
      case SwrveActionType.Install:
        str = "install";
        break;
      case SwrveActionType.Dismiss:
        str = "dismiss";
        break;
      case SwrveActionType.Custom:
        str = "deeplink";
        break;
      case SwrveActionType.CopyToClipboard:
        str = "clipboard";
        break;
    }
    return str;
  }

  private enum SwrveSdkState
  {
    ON,
    EVENT_SENDING_PAUSED,
  }
}
