using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.Telemetry;
using Hearthstone.Core;
using Hearthstone.Telemetry;
using HearthstoneTelemetry;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelemetryManager
{
  private readonly ITelemetryClient m_telemetryClient = (ITelemetryClient) new TelemetryClient();
  private static readonly TelemetryManager s_instance = new TelemetryManager();
  private Service m_telemetryService;
  private bool m_auroraConnected;
  private readonly List<System.Action> m_shutdownListeners = new List<System.Action>();
  private readonly object m_listenerLock = new object();
  private BaseContextData m_contextData;
  private static List<ITelemetryManagerComponent> s_components;
  private readonly Dictionary<long, List<System.Action<long>>> m_messagesWaitingForCallback = new Dictionary<long, List<System.Action<long>>>();

  public static TelemetryManagerComponentNetwork NetworkComponent { get; private set; }

  public static string ProgramId => TelemetryManager.s_instance.m_telemetryService != null && TelemetryManager.s_instance.m_telemetryService.Context != null && TelemetryManager.s_instance.m_telemetryService.Context.Program != null && TelemetryManager.s_instance.m_telemetryService.Context.Program.Id != null ? TelemetryManager.s_instance.m_telemetryService.Context.Program.Id : string.Empty;

  public static string ProgramName => TelemetryManager.s_instance.m_telemetryService != null && TelemetryManager.s_instance.m_telemetryService.Context != null && TelemetryManager.s_instance.m_telemetryService.Context.Program != null && TelemetryManager.s_instance.m_telemetryService.Context.Program.Name != null ? TelemetryManager.s_instance.m_telemetryService.Context.Program.Name : string.Empty;

  public static string ProgramVersion => TelemetryManager.s_instance.m_telemetryService != null && TelemetryManager.s_instance.m_telemetryService.Context != null && TelemetryManager.s_instance.m_telemetryService.Context.Program != null && TelemetryManager.s_instance.m_telemetryService.Context.Program.Version != null ? TelemetryManager.s_instance.m_telemetryService.Context.Program.Version : string.Empty;

  public static string SessionId => TelemetryManager.s_instance.m_telemetryService != null && TelemetryManager.s_instance.m_telemetryService.Context != null && TelemetryManager.s_instance.m_telemetryService.Context.SessionId != null ? TelemetryManager.s_instance.m_telemetryService.Context.SessionId : string.Empty;

  static TelemetryManager()
  {
    TelemetryManager.NetworkComponent = new TelemetryManagerComponentNetwork();
    TelemetryManager.s_components = new List<ITelemetryManagerComponent>()
    {
      (ITelemetryManagerComponent) new TelemetryManagerComponentAudio(TelemetryManager.s_instance.m_telemetryClient),
      (ITelemetryManagerComponent) TelemetryManager.NetworkComponent
    };
  }

  private TelemetryManager()
  {
  }

  public static ITelemetryClient Client() => TelemetryManager.s_instance.m_telemetryClient;

  public static void RegisterMessageSentCallback(long messageId, System.Action<long> sentMessageCallback)
  {
    if (TelemetryManager.s_instance.m_messagesWaitingForCallback.ContainsKey(messageId))
    {
      if (TelemetryManager.s_instance.m_messagesWaitingForCallback[messageId] == null)
        TelemetryManager.s_instance.m_messagesWaitingForCallback[messageId] = new List<System.Action<long>>();
      TelemetryManager.s_instance.m_messagesWaitingForCallback[messageId].Add(sentMessageCallback);
    }
    else
      TelemetryManager.s_instance.m_messagesWaitingForCallback.Add(messageId, new List<System.Action<long>>()
      {
        sentMessageCallback
      });
  }

  public static void RegisterShutdownListener(System.Action handler)
  {
    lock (TelemetryManager.s_instance.m_listenerLock)
    {
      if (TelemetryManager.s_instance.m_shutdownListeners.Contains(handler))
        return;
      TelemetryManager.s_instance.m_shutdownListeners.Add(handler);
    }
  }

  public static void UnregisterShutdownListener(System.Action handler)
  {
    lock (TelemetryManager.s_instance.m_listenerLock)
      TelemetryManager.s_instance.m_shutdownListeners.Remove(handler);
  }

  public static string GetApplicationId() => TelemetryManager.s_instance.m_contextData != null ? TelemetryManager.s_instance.m_contextData.ApplicationID : string.Empty;

  public static void RebuildContext() => TelemetryManager.s_instance.SetServiceContext();

  public static void Flush() => TelemetryManager.s_instance.m_telemetryService.Flush();

  public static void FlushSync()
  {
    if (!TelemetryManager.s_instance.m_telemetryClient.IsInitialized())
      return;
    TelemetryManager.s_instance.m_telemetryService.Stop(new TimeSpan(0, 0, 0, 0, 3000));
    Processor.RunCoroutine(TelemetryManager.s_instance.m_telemetryService.Run());
  }

  public static void Reset()
  {
    if (!TelemetryManager.s_instance.m_telemetryClient.IsInitialized())
      return;
    TelemetryManager.s_instance.m_auroraConnected = false;
    PresenceMgr.Get().ResetTelemetry();
    TelemetryManager.s_instance.m_telemetryService.Stop(new TimeSpan(0, 0, 0, 0, 3000));
    TelemetryManager.s_instance.SetTelemetryServiceData();
    Processor.RunCoroutine(TelemetryManager.s_instance.m_telemetryService.Run());
    Processor.RunCoroutine(TelemetryManager.s_instance.SetPushSdkTelemetryInfo());
  }

  private IEnumerator SetPushSdkTelemetryInfo()
  {
    yield return (object) new WaitUntil((Func<bool>) (() => TelemetryManager.s_instance.m_telemetryService.Running));
    PushNotificationManager.Get().SetTelemetryInfo(TelemetryManager.ProgramId, TelemetryManager.ProgramName, TelemetryManager.ProgramVersion, TelemetryManager.SessionId);
  }

  public static void OnBattleNetConnect(string host, int port, BattleNetErrors error)
  {
    if (error != BattleNetErrors.ERROR_OK)
    {
      TelemetryManager.s_instance.m_telemetryClient.SendConnectFail("AURORA", error.ToString(), host, new uint?((uint) port));
    }
    else
    {
      TelemetryManager.s_instance.m_auroraConnected = true;
      TelemetryManager.s_instance.m_telemetryClient.SendConnectSuccess("AURORA", host, new uint?((uint) port));
      TelemetryManager.RegisterShutdownListener((System.Action) (() => TelemetryManager.OnBattleNetDisconnect(host, port, BattleNetErrors.ERROR_OK)));
    }
  }

  public static void OnBattleNetDisconnect(string host, int port, BattleNetErrors error)
  {
    if (!TelemetryManager.s_instance.m_auroraConnected)
      return;
    TelemetryManager.s_instance.m_auroraConnected = false;
    TelemetryManager.s_instance.m_telemetryClient.SendDisconnect("AURORA", TelemetryUtil.GetReasonFromBnetError(error), error == BattleNetErrors.ERROR_OK ? (string) null : error.ToString());
  }

  public static void Shutdown()
  {
    if (!TelemetryManager.s_instance.m_telemetryClient.IsInitialized())
      return;
    Log.Telemetry.Print("Shutting down telemetry");
    foreach (ITelemetryManagerComponent component in TelemetryManager.s_components)
      component.Shutdown();
    Processor.UnregisterUpdateDelegate(new System.Action(TelemetryManager.s_instance.OnUpdate));
    TelemetryManager.ProcessShutdownListeners();
    TelemetryManager.s_instance.m_telemetryService.Stop(new TimeSpan(0, 0, 0, 0, 1000));
    TelemetryManager.s_instance.m_telemetryClient.Shutdown();
  }

  public static void Initialize()
  {
    if (!Vars.Key("Telemetry.Enabled").GetBool(true))
      return;
    TelemetryManager.s_instance.SetTelemetryServiceData();
    Log.Telemetry.Print("Sending telemetry messages to TDK instance: {0}, SSL={1} IngestPort={2}", (object) TelemetryManager.s_instance.m_contextData.IngestUri.AbsoluteUri, (object) (TelemetryManager.s_instance.m_contextData.IngestUri.Scheme == "https"), (object) TelemetryManager.s_instance.m_contextData.IngestUri.Port);
    Processor.RunCoroutine(TelemetryManager.s_instance.m_telemetryService.Run());
    Processor.RunCoroutine(TelemetryManager.s_instance.SetPushSdkTelemetryInfo());
    foreach (ITelemetryManagerComponent component in TelemetryManager.s_components)
      component.Initialize();
    Processor.RegisterUpdateDelegate(new System.Action(TelemetryManager.s_instance.OnUpdate));
  }

  public static void SetTelemetryFeatureStatus(bool isEnabled)
  {
    if (isEnabled)
      TelemetryManager.s_instance.m_telemetryClient.Enable();
    else
      TelemetryManager.s_instance.m_telemetryClient.Disable();
  }

  private void SetTelemetryServiceData()
  {
    TelemetryManager.s_instance.m_contextData = (BaseContextData) new StandaloneContext();
    ServiceOptions options = new ServiceOptions(this.m_contextData.ProgramId);
    options.IngestBaseUrl = this.m_contextData.IngestUri.AbsoluteUri;
    options.SendStartAndFinishMessages = true;
    options.MaxQueueSize = 1500;
    options.MaxBatchSize = 10;
    options.OnMessageSent = new MessageCallback(this.TelemetryMessageSent);
    if (Vars.Key("Telemetry.LogEnabled").GetBool(false))
    {
      Blizzard.Telemetry.Log.Logger = (Blizzard.Telemetry.ILogger) new TelemetryLogWrapper();
      options.LogEnqueue = true;
      options.LogMessageRequest = true;
    }
    this.m_telemetryService = new Service(options);
    this.SetServiceContext();
    this.m_telemetryClient.Initialize(this.m_telemetryService);
  }

  private void SetServiceContext()
  {
    List<string> stringList = new List<string>();
    if (this.m_contextData.ConnectionType == TelemetryConnectionType.Internal)
    {
      stringList.Add("dev");
      Log.Telemetry.Print("Sending telemetry to production endpoint. Messages will be tagged as dev");
    }
    else
    {
      stringList.Add("prod");
      Log.Telemetry.Print("Sending telemetry to production endpoint. Messages will be tagged as prod");
    }
    Service telemetryService = this.m_telemetryService;
    Blizzard.Telemetry.Context context = new Blizzard.Telemetry.Context();
    context.Account = BnetUtils.TryGetGameAccountId();
    context.BnetId = BnetUtils.TryGetBnetAccountId();
    Blizzard.Telemetry.Context.LocationInfo locationInfo = new Blizzard.Telemetry.Context.LocationInfo();
    BnetRegion? gameRegion = BnetUtils.TryGetGameRegion();
    locationInfo.BnetRegion = gameRegion.HasValue ? new int?((int) gameRegion.GetValueOrDefault()) : new int?();
    context.GameLocation = locationInfo;
    context.Program = new Blizzard.Telemetry.Context.ProgramInfo()
    {
      Id = TelemetryManager.s_instance.m_contextData.ProgramId,
      Name = TelemetryManager.s_instance.m_contextData.ProgramName,
      Version = TelemetryManager.s_instance.m_contextData.ProgramVersion
    };
    context.PlayerLocation = new Blizzard.Telemetry.Context.LocationInfo()
    {
      BnetRegion = TelemetryManager.s_instance.m_contextData.BattleNetRegion
    };
    context.Host = new Blizzard.Telemetry.Context.HostInfo()
    {
      Tag = stringList,
      Arch = SystemInfo.deviceModel
    };
    telemetryService.UserContext = context;
  }

  private void OnUpdate()
  {
    if (!this.m_telemetryClient.IsInitialized())
      return;
    foreach (ITelemetryManagerComponent component in TelemetryManager.s_components)
      component.Update();
    this.m_telemetryClient.OnUpdate();
  }

  private void TelemetryMessageSent(long messageId)
  {
    if (!this.m_messagesWaitingForCallback.ContainsKey(messageId))
      return;
    foreach (System.Action<long> action in this.m_messagesWaitingForCallback[messageId])
    {
      if (action != null)
        action(messageId);
    }
    this.m_messagesWaitingForCallback.Remove(messageId);
  }

  private static void ProcessShutdownListeners()
  {
    if (TelemetryManager.s_instance.m_shutdownListeners.Count == 0)
      return;
    System.Action[] array;
    lock (TelemetryManager.s_instance.m_listenerLock)
    {
      array = TelemetryManager.s_instance.m_shutdownListeners.ToArray();
      TelemetryManager.s_instance.m_shutdownListeners.Clear();
    }
    Log.Telemetry.Print("Processing {0} shutdown listeners", (object) array.Length);
    foreach (System.Action action in array)
      action();
  }
}
