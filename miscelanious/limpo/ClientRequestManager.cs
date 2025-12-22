using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Services;
using Networking;
using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientRequestManager : IClientRequestManager
{
  private static Map<int, string> s_typeToStringMap = new Map<int, string>();
  private readonly ClientRequestManager.ClientRequestConfig m_defaultConfig = new ClientRequestManager.ClientRequestConfig()
  {
    ShouldRetryOnError = true,
    ShouldRetryOnUnhandled = true,
    RequestedSystem = UtilSystemId.CLIENT
  };
  public uint m_nextContexId;
  public uint m_nextRequestId;
  private ClientRequestManager.InternalState m_state = new ClientRequestManager.InternalState();
  private Subscribe m_subscribePacket = new Subscribe();
  private bool m_hasSubscribedToUtilClient;

  public bool SendClientRequest(
    int type,
    IProtoBuf body,
    ClientRequestManager.ClientRequestConfig clientRequestConfig,
    RequestPhase requestPhase = RequestPhase.RUNNING)
  {
    return this.SendClientRequestImpl(type, body, clientRequestConfig, requestPhase);
  }

  public void EnsureSubscribedTo(UtilSystemId system) => this.EnsureSubscribedToImpl(system);

  public void NotifyResponseReceived(PegasusPacket packet) => this.NotifyResponseReceivedImpl(packet);

  public void NotifyStartupSequenceComplete() => this.NotifyStartupSequenceCompleteImpl();

  public bool HasPendingDeliveryPackets() => this.HasPendingDeliveryPacketsImpl();

  public int PeekNetClientRequestType() => this.PeekNetClientRequestTypeImpl();

  public ResponseWithRequest GetNextClientRequest() => this.GetNextClientRequestImpl();

  public void DropNextClientRequest() => this.DropNextClientRequestImpl();

  public void NotifyLoginSequenceCompleted() => this.NotifyLoginSequenceCompletedImpl();

  public bool ShouldIgnoreError(BnetErrorInfo errorInfo) => this.ShouldIgnoreErrorImpl(errorInfo);

  public void Terminate() => this.TerminateImpl();

  public void SetDisconnectedFromBattleNet() => this.m_state = new ClientRequestManager.InternalState();

  public void Update() => this.UpdateImpl();

  public bool HasErrors() => this.HasErrorsImpl();

  private bool ShouldIgnoreErrorImpl(BnetErrorInfo errorInfo)
  {
    uint context = (uint) errorInfo.GetContext();
    if (context == 0U)
      return false;
    ClientRequestManager.ClientRequestType clientRequest = this.GetClientRequest(context, "should_ignore_error", true);
    if (clientRequest == null)
      return this.GetDroppedRequest(context, "should_ignore") || this.GetPendingSendRequest(context, "should_ignore") != null;
    BattleNetErrors error = errorInfo.GetError();
    if (clientRequest.IsSubscribeRequest)
      return (ulong) clientRequest.System.SubscribeAttempt < clientRequest.System.MaxResubscribeAttempts || !clientRequest.ShouldRetryOnError;
    switch (error)
    {
      case BattleNetErrors.ERROR_INTERNAL:
      case BattleNetErrors.ERROR_RPC_REQUEST_TIMED_OUT:
        if (!clientRequest.ShouldRetryOnError)
          return true;
        return clientRequest.System.PendingResponseTimeout != 0UL && this.RescheduleSubscriptionAndRetryRequest(clientRequest, "received_error_util_lost");
      case BattleNetErrors.ERROR_GAME_UTILITY_SERVER_NO_SERVER:
        ++clientRequest.RequestNotHandledCount;
        return !clientRequest.ShouldRetryOnUnhandled || this.RescheduleSubscriptionAndRetryRequest(clientRequest, "received_error_util_server_no_server");
      default:
        return false;
    }
  }

  private bool RescheduleSubscriptionAndRetryRequest(
    ClientRequestManager.ClientRequestType clientRequest,
    string errorReason)
  {
    if ((long) clientRequest.RouteDispatchedTo == (long) clientRequest.System.Route)
      this.ScheduleResubscribeWithNewRoute(clientRequest.System);
    this.AddRequestToPendingSendQueue(clientRequest, "resubscribe_and_retry_request");
    return true;
  }

  private void ProcessServiceUnavailable(
    ClientRequestResponse response,
    ClientRequestManager.ClientRequestType clientRequest)
  {
    ++clientRequest.RequestNotHandledCount;
    this.RescheduleSubscriptionAndRetryRequest(clientRequest, "received_CRRF_SERVICE_UNAVAILABLE");
  }

  private void ProcessClientRequestResponse(
    PegasusPacket packet,
    ClientRequestManager.ClientRequestType clientRequest)
  {
    if (!(packet.Body is ClientRequestResponse))
      return;
    ClientRequestResponse body = (ClientRequestResponse) packet.Body;
    ClientRequestResponse.ClientRequestResponseFlags requestResponseFlags1 = ClientRequestResponse.ClientRequestResponseFlags.CRRF_SERVICE_UNAVAILABLE;
    if ((body.ResponseFlags & requestResponseFlags1) != ClientRequestResponse.ClientRequestResponseFlags.CRRF_SERVICE_NONE)
      this.ProcessServiceUnavailable(body, clientRequest);
    ClientRequestResponse.ClientRequestResponseFlags requestResponseFlags2 = ClientRequestResponse.ClientRequestResponseFlags.CRRF_SERVICE_UNKNOWN_ERROR;
    if ((body.ResponseFlags & requestResponseFlags2) == ClientRequestResponse.ClientRequestResponseFlags.CRRF_SERVICE_NONE)
      return;
    this.m_state.m_receivedErrorSignal = true;
  }

  private bool HasPendingDeliveryPacketsImpl() => this.m_state.m_responsesPendingDelivery.Count > 0;

  private int PeekNetClientRequestTypeImpl() => this.m_state.m_responsesPendingDelivery.Count == 0 ? 0 : this.m_state.m_responsesPendingDelivery.Peek().Response.Type;

  private ResponseWithRequest GetNextClientRequestImpl() => this.m_state.m_responsesPendingDelivery.Count == 0 ? (ResponseWithRequest) null : this.m_state.m_responsesPendingDelivery.Peek();

  private void DropNextClientRequestImpl()
  {
    if (this.m_state.m_responsesPendingDelivery.Count == 0)
      return;
    this.m_state.m_responsesPendingDelivery.Dequeue();
  }

  private bool HasErrorsImpl() => this.m_state.m_receivedErrorSignal;

  private void UpdateImpl()
  {
    if (!this.m_state.m_loginCompleteNotificationReceived || !this.m_hasSubscribedToUtilClient)
      return;
    ClientRequestManager.SystemChannel system1 = this.m_state.m_systems.Systems[UtilSystemId.CLIENT];
    if (!this.UpdateStateSubscribeImpl(system1))
      return;
    this.ProcessClientRequests(system1);
    foreach (KeyValuePair<UtilSystemId, ClientRequestManager.SystemChannel> system2 in this.m_state.m_systems.Systems)
    {
      if (system2.Key != UtilSystemId.CLIENT && this.UpdateStateSubscribeImpl(system2.Value))
        this.ProcessClientRequests(system2.Value);
    }
  }

  private bool SendClientRequestImpl(
    int type,
    IProtoBuf body,
    ClientRequestManager.ClientRequestConfig clientRequestConfig,
    RequestPhase requestPhase)
  {
    if (type == 0 || requestPhase < RequestPhase.STARTUP || requestPhase > RequestPhase.RUNNING)
      return false;
    ClientRequestManager.ClientRequestConfig clientRequestConfig1 = clientRequestConfig == null ? this.m_defaultConfig : clientRequestConfig;
    ClientRequestManager.SystemChannel system = this.GetOrCreateSystem(clientRequestConfig1.RequestedSystem);
    if (requestPhase < system.CurrentPhase || system.WasEverInRunningPhase && requestPhase < RequestPhase.RUNNING || body == null)
      return false;
    ClientRequestManager.ClientRequestType clientRequestType = new ClientRequestManager.ClientRequestType(system);
    clientRequestType.Type = type;
    clientRequestType.ShouldRetryOnError = clientRequestConfig1.ShouldRetryOnError;
    clientRequestType.ShouldRetryOnUnhandled = clientRequestConfig1.ShouldRetryOnUnhandled;
    clientRequestType.Body = ProtobufUtil.ToByteArray(body);
    clientRequestType.Phase = requestPhase;
    clientRequestType.SendCount = 0U;
    clientRequestType.RequestNotHandledCount = 0U;
    clientRequestType.RequestId = this.GetNextRequestId();
    if (clientRequestType.Phase == RequestPhase.STARTUP)
      system.Phases.StartUp.PendingSend.Enqueue(clientRequestType);
    else
      system.Phases.Running.PendingSend.Enqueue(clientRequestType);
    return true;
  }

  private ClientRequestManager.SystemChannel GetOrCreateSystem(
    UtilSystemId systemId)
  {
    ClientRequestManager.SystemChannel system1 = (ClientRequestManager.SystemChannel) null;
    if (this.m_state.m_systems.Systems.TryGetValue(systemId, out system1))
      return system1;
    ClientRequestManager.SystemChannel system2 = new ClientRequestManager.SystemChannel();
    system2.SystemId = systemId;
    this.m_state.m_systems.Systems[systemId] = system2;
    if (systemId == UtilSystemId.CLIENT)
      this.m_hasSubscribedToUtilClient = true;
    return system2;
  }

  public void EnsureSubscribedToImpl(UtilSystemId systemId) => this.GetOrCreateSystem(systemId);

  private uint GenerateContextId() => ++this.m_nextContexId;

  private void NotifyResponseReceivedImpl(PegasusPacket packet)
  {
    uint context = (uint) packet.Context;
    ClientRequestManager.ClientRequestType clientRequest = this.GetClientRequest(context, "received_response", true);
    if (clientRequest == null)
    {
      if (packet.Context != 0 && this.GetDroppedRequest(context, "received_response"))
        return;
      this.m_state.m_responsesPendingDelivery.Enqueue(new ResponseWithRequest(packet));
    }
    else
    {
      switch (packet.Type)
      {
        case 315:
          this.ProcessSubscribeResponse(packet, clientRequest);
          break;
        case 328:
          this.ProcessClientRequestResponse(packet, clientRequest);
          break;
        default:
          this.ProcessResponse(packet, clientRequest);
          break;
      }
    }
  }

  private void NotifyStartupSequenceCompleteImpl() => this.m_state.m_runningPhaseEnabled = true;

  private void NotifyLoginSequenceCompletedImpl() => this.m_state.m_loginCompleteNotificationReceived = true;

  private uint SendToUtil(ClientRequestManager.ClientRequestType request)
  {
    uint contextId = this.GenerateContextId();
    ulong route = request.System.Route;
    byte[] utilPacketBytes = request.GetUtilPacketBytes();
    BattleNet.SendUtilPacket(request.System.SystemId, utilPacketBytes, (int) contextId, route);
    request.Context = contextId;
    request.SendTime = Time.realtimeSinceStartup;
    ++request.SendCount;
    request.RouteDispatchedTo = route;
    this.AddRequestToPendingResponse(request, "send_to_util");
    if (!request.IsSubscribeRequest)
      request.Phase.ToString();
    return contextId;
  }

  private uint GetNextRequestId() => ++this.m_nextRequestId;

  private void SendSubscriptionRequest(ClientRequestManager.SystemChannel system)
  {
    UtilSystemId systemId = system.SystemId;
    this.m_subscribePacket.FirstSubscribeForRoute = system.Route == 0UL;
    this.m_subscribePacket.FirstSubscribe = system.SubscriptionStatus.LastSend == DateTime.MinValue;
    this.m_subscribePacket.UtilSystemId = (int) systemId;
    ClientRequestManager.ClientRequestType request = new ClientRequestManager.ClientRequestType(system);
    request.Type = 314;
    request.Body = ProtobufUtil.ToByteArray((IProtoBuf) this.m_subscribePacket);
    request.RequestId = this.GetNextRequestId();
    request.IsSubscribeRequest = true;
    system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.PENDING_RESPONSE;
    system.SubscriptionStatus.LastSend = DateTime.Now;
    system.SubscriptionStatus.ContexId = this.SendToUtil(request);
    ++system.SubscribeAttempt;
    ++this.m_state.m_subscribePacketsSent;
  }

  private void ScheduleResubscribeWithNewRoute(ClientRequestManager.SystemChannel system)
  {
    system.Route = 0UL;
    system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.PENDING_SEND;
  }

  private void TerminateImpl()
  {
    Unsubscribe packet = new Unsubscribe();
    foreach (KeyValuePair<UtilSystemId, ClientRequestManager.SystemChannel> system in this.m_state.m_systems.Systems)
    {
      ClientRequestManager.SystemChannel systemChannel = system.Value;
      Network service;
      if (systemChannel.SubscriptionStatus.CurrentState == ClientRequestManager.SubscriptionStatusType.State.SUBSCRIBED && systemChannel.Route != 0UL && ServiceManager.TryGet<Network>(out service))
        service.SendUnsubcribeRequest(packet, systemChannel.SystemId);
    }
  }

  private bool UpdateStateSubscribeImpl(ClientRequestManager.SystemChannel system)
  {
    switch (system.SubscriptionStatus.CurrentState)
    {
      case ClientRequestManager.SubscriptionStatusType.State.PENDING_SEND:
        return this.ProcessSubscribeStatePendingSend(system);
      case ClientRequestManager.SubscriptionStatusType.State.PENDING_RESPONSE:
        return this.ProcessSubscribeStatePendingResponse(system);
      case ClientRequestManager.SubscriptionStatusType.State.SUBSCRIBED:
        return this.ProcessSubscribeStateSubscribed(system);
      default:
        return system.SubscriptionStatus.CurrentState == ClientRequestManager.SubscriptionStatusType.State.SUBSCRIBED;
    }
  }

  private bool ProcessSubscribeStatePendingSend(ClientRequestManager.SystemChannel system)
  {
    if ((DateTime.Now - system.SubscriptionStatus.LastSend).TotalSeconds > (double) system.PendingSubscribeTimeout)
      this.SendSubscriptionRequest(system);
    return system.Route > 0UL;
  }

  private bool ProcessSubscribeStatePendingResponse(ClientRequestManager.SystemChannel system)
  {
    if ((DateTime.Now - system.SubscriptionStatus.LastSend).TotalSeconds > (double) system.PendingSubscribeTimeout)
      this.ScheduleResubscribeWithNewRoute(system);
    return system.Route > 0UL;
  }

  private int CountPendingResponsesForSystemId(ClientRequestManager.SystemChannel system)
  {
    int num = 0;
    foreach (KeyValuePair<uint, ClientRequestManager.ClientRequestType> activePendingResponse in this.m_state.m_activePendingResponseMap)
    {
      if (activePendingResponse.Value.System.SystemId == system.SystemId)
        ++num;
    }
    return num;
  }

  private bool ProcessSubscribeStateSubscribed(ClientRequestManager.SystemChannel system)
  {
    if ((ulong) ((double) Time.realtimeSinceStartup - (double) system.SubscriptionStatus.SubscribedTime) < system.KeepAliveSecs || this.CountPendingResponsesForSystemId(system) > 0 || system.KeepAliveSecs <= 0UL)
      return true;
    system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.PENDING_SEND;
    return true;
  }

  private void ProcessSubscribeResponse(
    PegasusPacket packet,
    ClientRequestManager.ClientRequestType request)
  {
    if (!(packet.Body is SubscribeResponse))
      return;
    ClientRequestManager.SystemChannel system = request.System;
    int systemId = (int) system.SystemId;
    SubscribeResponse body = (SubscribeResponse) packet.Body;
    if (body.Result == SubscribeResponse.ResponseResult.FAILED_UNAVAILABLE)
    {
      this.ScheduleResubscribeWithNewRoute(system);
    }
    else
    {
      system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.SUBSCRIBED;
      system.SubscriptionStatus.SubscribedTime = Time.realtimeSinceStartup;
      system.Route = body.Route;
      system.CurrentPhase = RequestPhase.STARTUP;
      system.SubscribeAttempt = 0U;
      system.KeepAliveSecs = body.KeepAliveSecs;
      system.MaxResubscribeAttempts = body.MaxResubscribeAttempts;
      system.PendingResponseTimeout = body.PendingResponseTimeout;
      system.PendingSubscribeTimeout = body.PendingSubscribeTimeout;
      PegasusPacket request1 = new PegasusPacket(request.Type, packet.Context, (object) request.Body);
      this.m_state.m_responsesPendingDelivery.Enqueue(new ResponseWithRequest(packet, request1));
      ++system.m_subscribePacketsReceived;
    }
  }

  private void ProcessClientRequests(ClientRequestManager.SystemChannel system)
  {
    ClientRequestManager.PendingMapType pendingMapType = system.CurrentPhase == RequestPhase.STARTUP ? system.Phases.StartUp : system.Phases.Running;
    foreach (KeyValuePair<uint, ClientRequestManager.ClientRequestType> activePendingResponse in this.m_state.m_activePendingResponseMap)
    {
      ClientRequestManager.ClientRequestType clientRequestType = activePendingResponse.Value;
      if (!clientRequestType.IsSubscribeRequest && clientRequestType.System != null && clientRequestType.System.SystemId == system.SystemId && system.PendingResponseTimeout != 0UL && (double) Time.realtimeSinceStartup - (double) clientRequestType.SendTime >= (double) system.PendingResponseTimeout)
      {
        this.m_state.m_activePendingResponseMap.Remove(activePendingResponse.Key);
        this.ScheduleResubscribeWithNewRoute(system);
        return;
      }
    }
    if (system.Route == 0UL)
      return;
    bool flag = pendingMapType.PendingSend.Count > 0;
    while (pendingMapType.PendingSend.Count > 0)
    {
      int util = (int) this.SendToUtil(pendingMapType.PendingSend.Dequeue());
    }
    if (flag || system.CurrentPhase != RequestPhase.STARTUP || !this.m_state.m_runningPhaseEnabled)
      return;
    system.CurrentPhase = RequestPhase.RUNNING;
  }

  private void ProcessResponse(
    PegasusPacket packet,
    ClientRequestManager.ClientRequestType clientRequest)
  {
    if (packet.Type == 254)
      return;
    PegasusPacket request = new PegasusPacket(clientRequest.Type, packet.Context, (object) clientRequest.Body);
    this.m_state.m_responsesPendingDelivery.Enqueue(new ResponseWithRequest(packet, request));
  }

  private ClientRequestManager.ClientRequestType GetClientRequest(
    uint contextId,
    string reason,
    bool removeFromPendingResponse)
  {
    if (contextId == 0U)
      return (ClientRequestManager.ClientRequestType) null;
    ClientRequestManager.ClientRequestType clientRequest;
    if (!this.m_state.m_activePendingResponseMap.TryGetValue(contextId, out clientRequest))
    {
      if (this.GetDroppedRequest(contextId, "get_client_request", false))
        this.GetPendingSendRequest(contextId, "get_client_request", false);
      return (ClientRequestManager.ClientRequestType) null;
    }
    if (removeFromPendingResponse)
      this.m_state.m_activePendingResponseMap.Remove(contextId);
    return clientRequest;
  }

  private void AddRequestToPendingSendQueue(
    ClientRequestManager.ClientRequestType clientRequest,
    string reason)
  {
    if (clientRequest.Phase == RequestPhase.STARTUP)
    {
      clientRequest.System.Phases.StartUp.PendingSend.Enqueue(clientRequest);
      int count = clientRequest.System.Phases.StartUp.PendingSend.Count;
    }
    else
    {
      clientRequest.System.Phases.Running.PendingSend.Enqueue(clientRequest);
      int count = clientRequest.System.Phases.Running.PendingSend.Count;
    }
  }

  private void AddRequestToPendingResponse(
    ClientRequestManager.ClientRequestType clientRequest,
    string reason)
  {
    if (this.m_state.m_activePendingResponseMap.ContainsKey(clientRequest.Context))
      return;
    this.m_state.m_activePendingResponseMap.Add(clientRequest.Context, clientRequest);
  }

  private bool GetDroppedRequest(uint contextId, string reason, bool removeIfFound = true)
  {
    if (!this.m_state.m_ignorePendingResponseMap.Contains(contextId) || !removeIfFound)
      return false;
    this.m_state.m_ignorePendingResponseMap.Remove(contextId);
    return true;
  }

  private ClientRequestManager.ClientRequestType GetPendingSendRequestForPhase(
    uint contextId,
    bool removeIfFound,
    ClientRequestManager.PendingMapType pendingMap)
  {
    ClientRequestManager.ClientRequestType sendRequestForPhase = (ClientRequestManager.ClientRequestType) null;
    Queue<ClientRequestManager.ClientRequestType> clientRequestTypeQueue = new Queue<ClientRequestManager.ClientRequestType>();
    foreach (ClientRequestManager.ClientRequestType clientRequestType in pendingMap.PendingSend)
    {
      if (sendRequestForPhase == null && (int) clientRequestType.Context == (int) contextId)
      {
        sendRequestForPhase = clientRequestType;
        if (!removeIfFound)
          clientRequestTypeQueue.Enqueue(clientRequestType);
      }
      else
        clientRequestTypeQueue.Enqueue(clientRequestType);
    }
    pendingMap.PendingSend = clientRequestTypeQueue;
    return sendRequestForPhase;
  }

  private ClientRequestManager.ClientRequestType GetPendingSendRequest(
    uint contextId,
    string reason,
    bool removeIfFound = true)
  {
    ClientRequestManager.ClientRequestType pendingSendRequest = (ClientRequestManager.ClientRequestType) null;
    foreach (KeyValuePair<UtilSystemId, ClientRequestManager.SystemChannel> system in this.m_state.m_systems.Systems)
    {
      ClientRequestManager.SystemChannel systemChannel = system.Value;
      pendingSendRequest = this.GetPendingSendRequestForPhase(contextId, removeIfFound, systemChannel.Phases.Running);
      if (pendingSendRequest == null)
        pendingSendRequest = this.GetPendingSendRequestForPhase(contextId, removeIfFound, systemChannel.Phases.StartUp);
      else
        break;
    }
    return pendingSendRequest;
  }

  public class ClientRequestConfig
  {
    public bool ShouldRetryOnError { get; set; }

    public bool ShouldRetryOnUnhandled { get; set; }

    public UtilSystemId RequestedSystem { get; set; }
  }

  private class ClientRequestType
  {
    public int Type;
    public byte[] Body;
    public uint Context;
    public RequestPhase Phase;
    public uint SendCount;
    public uint RequestNotHandledCount;
    public float SendTime;
    public uint RequestId;
    public bool IsSubscribeRequest;
    public ClientRequestManager.SystemChannel System;
    public bool ShouldRetryOnError;
    public bool ShouldRetryOnUnhandled;
    public ulong RouteDispatchedTo;

    public ClientRequestType(ClientRequestManager.SystemChannel system) => this.System = system;

    public byte[] GetUtilPacketBytes()
    {
      RpcHeader rpcHeader = new RpcHeader();
      rpcHeader.Type = (ulong) this.Type;
      if (this.SendCount > 0U)
        rpcHeader.RetryCount = (ulong) this.SendCount;
      if (this.RequestNotHandledCount > 0U)
        rpcHeader.RequestNotHandledCount = (ulong) this.RequestNotHandledCount;
      RpcMessage protobuf = new RpcMessage();
      protobuf.RpcHeader = rpcHeader;
      if (this.Body != null && this.Body.Length != 0)
        protobuf.MessageBody = this.Body;
      return ProtobufUtil.ToByteArray((IProtoBuf) protobuf);
    }
  }

  private class SubscriptionStatusType
  {
    public ClientRequestManager.SubscriptionStatusType.State CurrentState;
    public DateTime LastSend = DateTime.MinValue;
    public float SubscribedTime;
    public uint ContexId;

    public enum State
    {
      PENDING_SEND,
      PENDING_RESPONSE,
      SUBSCRIBED,
    }
  }

  private class PendingMapType
  {
    public Queue<ClientRequestManager.ClientRequestType> PendingSend = new Queue<ClientRequestManager.ClientRequestType>();
  }

  private class PhaseMapType
  {
    public ClientRequestManager.PendingMapType StartUp = new ClientRequestManager.PendingMapType();
    public ClientRequestManager.PendingMapType Running = new ClientRequestManager.PendingMapType();
  }

  private class SystemChannel
  {
    public ClientRequestManager.PhaseMapType Phases = new ClientRequestManager.PhaseMapType();
    public ClientRequestManager.SubscriptionStatusType SubscriptionStatus = new ClientRequestManager.SubscriptionStatusType();
    public ulong Route;
    public RequestPhase CurrentPhase;
    public ulong KeepAliveSecs;
    public ulong MaxResubscribeAttempts;
    public ulong PendingResponseTimeout;
    public ulong PendingSubscribeTimeout = 15;
    public uint SubscribeAttempt;
    public bool WasEverInRunningPhase;
    public UtilSystemId SystemId;
    public uint m_subscribePacketsReceived;
  }

  private class SystemMap
  {
    public Map<UtilSystemId, ClientRequestManager.SystemChannel> Systems = new Map<UtilSystemId, ClientRequestManager.SystemChannel>();
  }

  private class InternalState
  {
    public Queue<ResponseWithRequest> m_responsesPendingDelivery = new Queue<ResponseWithRequest>();
    public ClientRequestManager.SystemMap m_systems = new ClientRequestManager.SystemMap();
    public uint m_subscribePacketsSent;
    public bool m_loginCompleteNotificationReceived;
    public Map<uint, ClientRequestManager.ClientRequestType> m_activePendingResponseMap = new Map<uint, ClientRequestManager.ClientRequestType>();
    public HashSet<uint> m_ignorePendingResponseMap = new HashSet<uint>();
    public bool m_runningPhaseEnabled;
    public bool m_receivedErrorSignal;
  }
}
