using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Blizzard.Telemetry.WTCG.Client;
using BobNetProto;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Streaming;
using PegasusFSG;
using PegasusLettuce;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NetCache : IService, IHasUpdate
{
  private static readonly Map<System.Type, GetAccountInfo.Request> m_getAccountInfoTypeMap = new Map<System.Type, GetAccountInfo.Request>()
  {
    {
      typeof (NetCache.NetCacheDecks),
      GetAccountInfo.Request.DECK_LIST
    },
    {
      typeof (NetCache.NetCacheMedalInfo),
      GetAccountInfo.Request.MEDAL_INFO
    },
    {
      typeof (NetCache.NetCacheCardBacks),
      GetAccountInfo.Request.CARD_BACKS
    },
    {
      typeof (NetCache.NetCachePlayerRecords),
      GetAccountInfo.Request.PLAYER_RECORD
    },
    {
      typeof (NetCache.NetCacheGamesPlayed),
      GetAccountInfo.Request.GAMES_PLAYED
    },
    {
      typeof (NetCache.NetCacheProfileProgress),
      GetAccountInfo.Request.CAMPAIGN_INFO
    },
    {
      typeof (NetCache.NetCacheCardValues),
      GetAccountInfo.Request.CARD_VALUES
    },
    {
      typeof (NetCache.NetCacheFeatures),
      GetAccountInfo.Request.FEATURES
    },
    {
      typeof (NetCache.NetCacheRewardProgress),
      GetAccountInfo.Request.REWARD_PROGRESS
    },
    {
      typeof (NetCache.NetCacheHeroLevels),
      GetAccountInfo.Request.HERO_XP
    },
    {
      typeof (NetCache.NetCacheFavoriteHeroes),
      GetAccountInfo.Request.FAVORITE_HEROES
    },
    {
      typeof (NetCache.NetCacheAccountLicenses),
      GetAccountInfo.Request.ACCOUNT_LICENSES
    },
    {
      typeof (NetCache.NetCacheCoins),
      GetAccountInfo.Request.COINS
    },
    {
      typeof (NetCache.NetCacheBattlegroundsHeroSkins),
      GetAccountInfo.Request.BATTLEGROUNDS_SKINS
    },
    {
      typeof (NetCache.NetCacheBattlegroundsGuideSkins),
      GetAccountInfo.Request.BATTLEGROUNDS_GUIDE_SKINS
    },
    {
      typeof (NetCache.NetCacheBattlegroundsBoardSkins),
      GetAccountInfo.Request.BATTLEGROUNDS_BOARD_SKINS
    },
    {
      typeof (NetCache.NetCacheBattlegroundsFinishers),
      GetAccountInfo.Request.BATTLEGROUNDS_FINISHERS
    },
    {
      typeof (NetCache.NetCacheBattlegroundsEmotes),
      GetAccountInfo.Request.BATTLEGROUNDS_EMOTES
    }
  };
  private static readonly Map<System.Type, int> m_genericRequestTypeMap = new Map<System.Type, int>()
  {
    {
      typeof (ClientStaticAssetsResponse),
      340
    }
  };
  private static readonly List<System.Type> m_ServerInitiatedAccountInfoTypes = new List<System.Type>()
  {
    typeof (NetCache.NetCacheCollection),
    typeof (NetCache.NetCacheClientOptions),
    typeof (NetCache.NetCacheArcaneDustBalance),
    typeof (NetCache.NetCacheGoldBalance),
    typeof (NetCache.NetCacheProfileNotices),
    typeof (NetCache.NetCacheBoosters),
    typeof (NetCache.NetCacheDecks),
    typeof (NetCache.NetCacheRenownBalance)
  };
  private static readonly Map<GetAccountInfo.Request, System.Type> m_requestTypeMap = NetCache.GetInvertTypeMap();
  private Map<System.Type, object> m_netCache = new Map<System.Type, object>();
  private NetCache.NetCacheHeroLevels m_prevHeroLevels;
  private NetCache.NetCacheMedalInfo m_previousMedalInfo;
  private List<NetCache.DelNewNoticesListener> m_newNoticesListeners = new List<NetCache.DelNewNoticesListener>();
  private List<NetCache.DelGoldBalanceListener> m_goldBalanceListeners = new List<NetCache.DelGoldBalanceListener>();
  private Map<System.Type, HashSet<System.Action>> m_updatedListeners = new Map<System.Type, HashSet<System.Action>>();
  private Map<System.Type, int> m_changeRequests = new Map<System.Type, int>();
  private bool m_receivedInitialClientState;
  private HashSet<long> m_ackedNotices = new HashSet<long>();
  private List<NetCache.ProfileNotice> m_queuedProfileNotices = new List<NetCache.ProfileNotice>();
  private bool m_receivedInitialProfileNotices;
  private long m_currencyVersion;
  private long m_initialCollectionVersion;
  private HashSet<long> m_expectedCardModifications = new HashSet<long>();
  private HashSet<long> m_handledCardModifications = new HashSet<long>();
  private long m_lastForceCheckedSeason;
  private List<NetCache.NetCacheBatchRequest> m_cacheRequests = new List<NetCache.NetCacheBatchRequest>();
  private List<NetCache.NetCacheBatchRequest> m_cacheRequestScratchList = new List<NetCache.NetCacheBatchRequest>();
  private List<System.Type> m_inTransitRequests = new List<System.Type>();
  private static bool m_fatalErrorCodeSet = false;

  private static Map<GetAccountInfo.Request, System.Type> GetInvertTypeMap()
  {
    Map<GetAccountInfo.Request, System.Type> invertTypeMap = new Map<GetAccountInfo.Request, System.Type>();
    foreach (KeyValuePair<System.Type, GetAccountInfo.Request> getAccountInfoType in NetCache.m_getAccountInfoTypeMap)
      invertTypeMap[getAccountInfoType.Value] = getAccountInfoType.Key;
    return invertTypeMap;
  }

  public event NetCache.DelFavoriteCardBackChangedListener FavoriteCardBackChanged;

  public event NetCache.DelFavoriteBattlegroundsHeroSkinChangedListener FavoriteBattlegroundsHeroSkinChanged;

  public event NetCache.DelFavoriteBattlegroundsGuideSkinChangedListener FavoriteBattlegroundsGuideSkinChanged;

  public event NetCache.DelFavoriteBattlegroundsBoardSkinChangedListener FavoriteBattlegroundsBoardSkinChanged;

  public event NetCache.DelFavoriteBattlegroundsFinisherChangedListener FavoriteBattlegroundsFinisherChanged;

  public event NetCache.DelFavoriteCoinChangedListener FavoriteCoinChanged;

  public event NetCache.DelOwnedBattlegroundsSkinsChanged OwnedBattlegroundsSkinsChanged;

  public bool HasReceivedInitialClientState => this.m_receivedInitialClientState;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    NetCache netCache = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    serviceLocator.Get<Network>().RegisterThrottledPacketListener(new Network.ThrottledPacketListener(netCache.OnPacketThrottled));
    netCache.RegisterNetCacheHandlers();
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (Network)
  };

  public void Shutdown()
  {
  }

  public static NetCache Get() => ServiceManager.Get<NetCache>();

  public T GetNetObject<T>()
  {
    object testData = this.GetTestData(typeof (T));
    if (testData != null)
      return (T) testData;
    return this.m_netCache.TryGetValue(typeof (T), out testData) && testData is T obj ? obj : default (T);
  }

  public bool IsNetObjectAvailable<T>() => (object) this.GetNetObject<T>() != null;

  private object GetTestData(System.Type type)
  {
    if (!(type == typeof (NetCache.NetCacheBoosters)) || !GameUtils.IsFakePackOpeningEnabled())
      return (object) null;
    NetCache.NetCacheBoosters testData = new NetCache.NetCacheBoosters();
    int fakePackCount = GameUtils.GetFakePackCount();
    testData.BoosterStacks.Add(new NetCache.BoosterStack()
    {
      Id = 1,
      Count = fakePackCount
    });
    return (object) testData;
  }

  public void UnloadNetObject<T>() => this.m_netCache[typeof (T)] = (object) null;

  public void ReloadNetObject<T>() => this.NetCacheReload_Internal((NetCache.NetCacheBatchRequest) null, typeof (T));

  public void RefreshNetObject<T>() => this.RequestNetCacheObject(typeof (T));

  public long GetArcaneDustBalance()
  {
    NetCache.NetCacheArcaneDustBalance netObject = this.GetNetObject<NetCache.NetCacheArcaneDustBalance>();
    if (netObject == null)
      return 0;
    return CraftingManager.IsInitialized ? netObject.Balance + CraftingManager.Get().GetUnCommitedArcaneDustChanges() : netObject.Balance;
  }

  public long GetGoldBalance()
  {
    NetCache.NetCacheGoldBalance netObject = this.GetNetObject<NetCache.NetCacheGoldBalance>();
    return netObject == null ? 0L : netObject.GetTotal();
  }

  public long GetRenownBalance()
  {
    NetCache.NetCacheRenownBalance netObject = this.GetNetObject<NetCache.NetCacheRenownBalance>();
    return netObject == null ? 0L : netObject.Balance;
  }

  public int GetArenaTicketBalance()
  {
    NetCache.NetPlayerArenaTickets netObject = this.GetNetObject<NetCache.NetPlayerArenaTickets>();
    return netObject == null ? 0 : netObject.Balance;
  }

  private bool GetOption<T>(ServerOption type, out T ret) where T : NetCache.ClientOptionBase
  {
    ret = default (T);
    NetCache.NetCacheClientOptions netObject = NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>();
    if (!this.ClientOptionExists(type) || !(netObject.ClientState[type] is T obj))
      return false;
    ret = obj;
    return true;
  }

  public int GetIntOption(ServerOption type)
  {
    NetCache.ClientOptionInt ret = (NetCache.ClientOptionInt) null;
    return !this.GetOption<NetCache.ClientOptionInt>(type, out ret) ? 0 : ret.OptionValue;
  }

  public bool GetIntOption(ServerOption type, out int ret)
  {
    ret = 0;
    NetCache.ClientOptionInt ret1 = (NetCache.ClientOptionInt) null;
    if (!this.GetOption<NetCache.ClientOptionInt>(type, out ret1))
      return false;
    ret = ret1.OptionValue;
    return true;
  }

  public long GetLongOption(ServerOption type)
  {
    NetCache.ClientOptionLong ret = (NetCache.ClientOptionLong) null;
    return !this.GetOption<NetCache.ClientOptionLong>(type, out ret) ? 0L : ret.OptionValue;
  }

  public bool GetLongOption(ServerOption type, out long ret)
  {
    ret = 0L;
    NetCache.ClientOptionLong ret1 = (NetCache.ClientOptionLong) null;
    if (!this.GetOption<NetCache.ClientOptionLong>(type, out ret1))
      return false;
    ret = ret1.OptionValue;
    return true;
  }

  public float GetFloatOption(ServerOption type)
  {
    NetCache.ClientOptionFloat ret = (NetCache.ClientOptionFloat) null;
    return !this.GetOption<NetCache.ClientOptionFloat>(type, out ret) ? 0.0f : ret.OptionValue;
  }

  public bool GetFloatOption(ServerOption type, out float ret)
  {
    ret = 0.0f;
    NetCache.ClientOptionFloat ret1 = (NetCache.ClientOptionFloat) null;
    if (!this.GetOption<NetCache.ClientOptionFloat>(type, out ret1))
      return false;
    ret = ret1.OptionValue;
    return true;
  }

  public ulong GetULongOption(ServerOption type)
  {
    NetCache.ClientOptionULong ret = (NetCache.ClientOptionULong) null;
    return !this.GetOption<NetCache.ClientOptionULong>(type, out ret) ? 0UL : ret.OptionValue;
  }

  public bool GetULongOption(ServerOption type, out ulong ret)
  {
    ret = 0UL;
    NetCache.ClientOptionULong ret1 = (NetCache.ClientOptionULong) null;
    if (!this.GetOption<NetCache.ClientOptionULong>(type, out ret1))
      return false;
    ret = ret1.OptionValue;
    return true;
  }

  public void RegisterUpdatedListener(System.Type type, System.Action listener)
  {
    if (listener == null)
      return;
    HashSet<System.Action> actionSet;
    if (!this.m_updatedListeners.TryGetValue(type, out actionSet))
    {
      actionSet = new HashSet<System.Action>();
      this.m_updatedListeners[type] = actionSet;
    }
    this.m_updatedListeners[type].Add(listener);
  }

  public void RemoveUpdatedListener(System.Type type, System.Action listener)
  {
    HashSet<System.Action> actionSet;
    if (listener == null || !this.m_updatedListeners.TryGetValue(type, out actionSet))
      return;
    actionSet.Remove(listener);
  }

  public void RegisterNewNoticesListener(NetCache.DelNewNoticesListener listener)
  {
    if (this.m_newNoticesListeners.Contains(listener))
      return;
    this.m_newNoticesListeners.Add(listener);
  }

  public void RemoveNewNoticesListener(NetCache.DelNewNoticesListener listener) => this.m_newNoticesListeners.Remove(listener);

  public bool RemoveNotice(long ID)
  {
    if (!(this.m_netCache[typeof (NetCache.NetCacheProfileNotices)] is NetCache.NetCacheProfileNotices cacheProfileNotices))
    {
      Debug.LogWarning((object) string.Format("NetCache.RemoveNotice({0}) - profileNotices is null", (object) ID));
      return false;
    }
    if (cacheProfileNotices.Notices == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.RemoveNotice({0}) - profileNotices.Notices is null", (object) ID));
      return false;
    }
    NetCache.ProfileNotice profileNotice = cacheProfileNotices.Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.NoticeID == ID));
    if (profileNotice == null)
      return false;
    cacheProfileNotices.Notices.Remove(profileNotice);
    this.m_ackedNotices.Add(profileNotice.NoticeID);
    return true;
  }

  public void NetCacheChanged<T>()
  {
    System.Type key = typeof (T);
    int num1 = 0;
    this.m_changeRequests.TryGetValue(key, out num1);
    int num2 = num1 + 1;
    this.m_changeRequests[key] = num2;
    if (num2 > 1)
      return;
    while (this.m_changeRequests[key] > 0)
    {
      this.NetCacheChangedImpl<T>();
      --this.m_changeRequests[key];
    }
  }

  private void NetCacheChangedImpl<T>()
  {
    foreach (NetCache.NetCacheBatchRequest request1 in this.m_cacheRequests.ToArray())
    {
      foreach (KeyValuePair<System.Type, NetCache.Request> request2 in request1.m_requests)
      {
        if (!(request2.Key != typeof (T)))
        {
          this.NetCacheCheckRequest(request1);
          break;
        }
      }
    }
  }

  public void CheckSeasonForRoll()
  {
    if (this.GetNetObject<NetCache.NetCacheProfileNotices>() == null)
      return;
    NetCache.NetCacheRewardProgress netObject = this.GetNetObject<NetCache.NetCacheRewardProgress>();
    if (netObject == null)
      return;
    DateTime utcNow = DateTime.UtcNow;
    DateTime dateTime = DateTime.FromFileTimeUtc(netObject.SeasonEndDate);
    if (dateTime >= utcNow || this.m_lastForceCheckedSeason == (long) netObject.Season)
      return;
    this.m_lastForceCheckedSeason = (long) netObject.Season;
    Log.Net.Print("NetCache.CheckSeasonForRoll oldSeason = {0} season end = {1} utc now = {2}", (object) this.m_lastForceCheckedSeason, (object) dateTime, (object) utcNow);
  }

  public void RegisterGoldBalanceListener(NetCache.DelGoldBalanceListener listener)
  {
    if (this.m_goldBalanceListeners.Contains(listener))
      return;
    this.m_goldBalanceListeners.Add(listener);
  }

  public void RemoveGoldBalanceListener(NetCache.DelGoldBalanceListener listener) => this.m_goldBalanceListeners.Remove(listener);

  public static void DefaultErrorHandler(NetCache.ErrorInfo info)
  {
    if (info.Error == NetCache.ErrorCode.TIMEOUT)
    {
      Hearthstone.BreakingNews.BreakingNews breakingNews = ServiceManager.Get<Hearthstone.BreakingNews.BreakingNews>();
      if (breakingNews != null && breakingNews.ShouldShowForCurrentPlatform)
      {
        string error = "GLOBAL_ERROR_NETWORK_UTIL_TIMEOUT";
        Network.Get().ShowBreakingNewsOrError(error);
      }
      else
        NetCache.ShowError(info, "GLOBAL_ERROR_NETWORK_UTIL_TIMEOUT");
    }
    else
      NetCache.ShowError(info, "GLOBAL_ERROR_NETWORK_GENERIC");
  }

  public static void ShowError(
    NetCache.ErrorInfo info,
    string localizationKey,
    params object[] localizationArgs)
  {
    Error.AddFatal(FatalErrorReason.NET_CACHE, localizationKey, localizationArgs);
    Debug.LogError((object) NetCache.GetInternalErrorMessage(info));
  }

  public static string GetInternalErrorMessage(NetCache.ErrorInfo info, bool includeStackTrace = true)
  {
    Map<System.Type, object> netCache = NetCache.Get().m_netCache;
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat("NetCache Error: {0}", (object) info.Error);
    stringBuilder.AppendFormat("\nFrom: {0}", (object) info.RequestingFunction.Method.Name);
    stringBuilder.AppendFormat("\nRequested Data ({0}):", (object) info.RequestedTypes.Count);
    foreach (KeyValuePair<System.Type, NetCache.Request> requestedType in info.RequestedTypes)
    {
      object obj = (object) null;
      netCache.TryGetValue(requestedType.Key, out obj);
      if (obj == null)
        stringBuilder.AppendFormat("\n[{0}] MISSING", (object) requestedType.Key);
      else
        stringBuilder.AppendFormat("\n[{0}]", (object) requestedType.Key);
    }
    if (includeStackTrace)
      stringBuilder.AppendFormat("\nStack Trace:\n{0}", (object) info.RequestStackTrace);
    return stringBuilder.ToString();
  }

  private void NetCacheMakeBatchRequest(NetCache.NetCacheBatchRequest batchRequest)
  {
    List<GetAccountInfo.Request> requestList = new List<GetAccountInfo.Request>();
    List<GenericRequest> genericRequests = (List<GenericRequest>) null;
    foreach (KeyValuePair<System.Type, NetCache.Request> request1 in batchRequest.m_requests)
    {
      NetCache.Request request2 = request1.Value;
      if (request2 == null)
        Debug.LogError((object) string.Format("NetUseBatchRequest Null request for {0}...SKIP", (object) request2.m_type.Name));
      else if (NetCache.m_ServerInitiatedAccountInfoTypes.Contains(request2.m_type))
      {
        if (request2.m_reload)
          Log.All.PrintWarning("Attempting to reload server-initiated NetCache request {0}. This is not valid - the server sends this data when it changes!", (object) request2.m_type.FullName);
      }
      else
      {
        if (request2.m_reload)
          this.m_netCache[request2.m_type] = (object) null;
        if ((!this.m_netCache.ContainsKey(request2.m_type) || this.m_netCache[request2.m_type] == null) && !this.m_inTransitRequests.Contains(request2.m_type))
        {
          request2.m_result = NetCache.RequestResult.PENDING;
          this.m_inTransitRequests.Add(request2.m_type);
          GetAccountInfo.Request request3;
          if (NetCache.m_getAccountInfoTypeMap.TryGetValue(request2.m_type, out request3))
          {
            requestList.Add(request3);
          }
          else
          {
            int num;
            if (NetCache.m_genericRequestTypeMap.TryGetValue(request2.m_type, out num))
            {
              if (genericRequests == null)
                genericRequests = new List<GenericRequest>();
              genericRequests.Add(new GenericRequest()
              {
                RequestId = num
              });
            }
            else
              Log.Net.Print("NetCache: Unable to make request for type={0}", (object) request2.m_type.FullName);
          }
        }
      }
    }
    if (requestList.Count > 0 || genericRequests != null)
      Network.Get().RequestNetCacheObjectList(requestList, genericRequests);
    if (this.m_cacheRequests.FindIndex((Predicate<NetCache.NetCacheBatchRequest>) (o => o.m_callback != null && o.m_callback == batchRequest.m_callback)) >= 0)
      Log.Net.PrintError("NetCache: detected multiple registrations for same callback! {0}.{1}", (object) batchRequest.m_callback.Target.GetType().Name, (object) batchRequest.m_callback.Method.Name);
    this.m_cacheRequests.Add(batchRequest);
    this.NetCacheCheckRequest(batchRequest);
  }

  private void NetCacheUse_Internal(NetCache.NetCacheBatchRequest request, System.Type type)
  {
    if (request != null && request.m_requests.ContainsKey(type))
      Log.Net.Print(string.Format("NetCache ...SKIP {0}", (object) type.Name));
    else if (this.m_netCache.ContainsKey(type) && this.m_netCache[type] != null)
    {
      Log.Net.Print(string.Format("NetCache ...USE {0}", (object) type.Name));
    }
    else
    {
      Log.Net.Print(string.Format("NetCache <<<GET {0}", (object) type.Name));
      this.RequestNetCacheObject(type);
    }
  }

  private void RequestNetCacheObject(System.Type type)
  {
    if (this.m_inTransitRequests.Contains(type))
      return;
    this.m_inTransitRequests.Add(type);
    Network.Get().RequestNetCacheObject(NetCache.m_getAccountInfoTypeMap[type]);
  }

  private void NetCacheReload_Internal(NetCache.NetCacheBatchRequest request, System.Type type)
  {
    this.m_netCache[type] = (object) null;
    if (type == typeof (NetCache.NetCacheProfileNotices))
      Debug.LogError((object) "NetCacheReload_Internal - tried to issue request with type NetCacheProfileNotices - this is no longer allowed!");
    else
      this.NetCacheUse_Internal(request, type);
  }

  private void NetCacheCheckRequest(NetCache.NetCacheBatchRequest request)
  {
    foreach (KeyValuePair<System.Type, NetCache.Request> request1 in request.m_requests)
    {
      if (!this.m_netCache.ContainsKey(request1.Key) || this.m_netCache[request1.Key] == null)
        return;
    }
    request.m_canTimeout = false;
    if (request.m_callback == null)
      return;
    request.m_callback();
  }

  private void UpdateRequestNeedState(System.Type type, NetCache.RequestResult result)
  {
    foreach (NetCache.NetCacheBatchRequest cacheRequest in this.m_cacheRequests)
    {
      if (cacheRequest.m_requests.ContainsKey(type))
        cacheRequest.m_requests[type].m_result = result;
    }
  }

  private void OnNetCacheObjReceived<T>(T netCacheObject)
  {
    System.Type type = typeof (T);
    Log.Net.Print(string.Format("OnNetCacheObjReceived SAVE --> {0}", (object) type.Name));
    this.UpdateRequestNeedState(type, NetCache.RequestResult.DATA_COMPLETE);
    this.m_netCache[type] = (object) netCacheObject;
    this.m_inTransitRequests.Remove(type);
    this.NetCacheChanged<T>();
    HashSet<System.Action> source;
    if (!this.m_updatedListeners.TryGetValue(type, out source))
      return;
    foreach (System.Action action in source.ToArray<System.Action>())
      action();
  }

  public void Clear()
  {
    Log.Net.PrintDebug("Clearing NetCache");
    this.m_netCache.Clear();
    this.m_prevHeroLevels = (NetCache.NetCacheHeroLevels) null;
    this.m_previousMedalInfo = (NetCache.NetCacheMedalInfo) null;
    this.m_changeRequests.Clear();
    this.m_cacheRequests.Clear();
    this.m_inTransitRequests.Clear();
    this.m_receivedInitialClientState = false;
    this.m_ackedNotices.Clear();
    this.m_queuedProfileNotices.Clear();
    this.m_receivedInitialProfileNotices = false;
    this.m_currencyVersion = 0L;
    this.m_initialCollectionVersion = 0L;
    this.m_expectedCardModifications.Clear();
    this.m_handledCardModifications.Clear();
    SceneDebugger service;
    if (!HearthstoneApplication.IsInternal() || !ServiceManager.TryGet<SceneDebugger>(out service))
      return;
    service.SetPlayerId(new long?());
  }

  public void ClearForNewAuroraConnection()
  {
    this.m_cacheRequests.Clear();
    this.m_inTransitRequests.Clear();
    this.m_receivedInitialClientState = false;
  }

  public void UnregisterNetCacheHandler(NetCache.NetCacheCallback handler) => this.m_cacheRequests.RemoveAll((Predicate<NetCache.NetCacheBatchRequest>) (o => o.m_callback == handler));

  public void Update()
  {
    if (!Network.IsRunning())
      return;
    this.m_cacheRequestScratchList.Clear();
    this.m_cacheRequestScratchList.AddRange((IEnumerable<NetCache.NetCacheBatchRequest>) this.m_cacheRequests);
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    foreach (NetCache.NetCacheBatchRequest cacheRequestScratch in this.m_cacheRequestScratchList)
    {
      if (cacheRequestScratch.m_canTimeout && (double) realtimeSinceStartup - (double) cacheRequestScratch.m_timeAdded >= (double) Network.GetMaxDeferredWait() && !Network.Get().HaveUnhandledPackets())
      {
        cacheRequestScratch.m_canTimeout = false;
        if (!NetCache.m_fatalErrorCodeSet)
        {
          NetCache.ErrorInfo info = new NetCache.ErrorInfo();
          info.Error = NetCache.ErrorCode.TIMEOUT;
          info.RequestingFunction = cacheRequestScratch.m_requestFunc;
          info.RequestedTypes = new Map<System.Type, NetCache.Request>((IEnumerable<KeyValuePair<System.Type, NetCache.Request>>) cacheRequestScratch.m_requests);
          info.RequestStackTrace = cacheRequestScratch.m_requestStackTrace;
          string errorSubset1 = "CT";
          int num = 0;
          foreach (KeyValuePair<System.Type, NetCache.Request> request in cacheRequestScratch.m_requests)
          {
            switch (request.Value.m_result)
            {
              case NetCache.RequestResult.GENERIC_COMPLETE:
              case NetCache.RequestResult.DATA_COMPLETE:
                if (num < 3)
                  continue;
                goto label_13;
              default:
                string[] strArray = request.Value.m_type.ToString().Split('+');
                if (strArray.GetLength(0) != 0)
                {
                  string str = strArray[strArray.GetLength(0) - 1];
                  errorSubset1 = errorSubset1 + ";" + str + "=" + (object) (int) request.Value.m_result;
                  ++num;
                  goto case NetCache.RequestResult.GENERIC_COMPLETE;
                }
                else
                  goto case NetCache.RequestResult.GENERIC_COMPLETE;
            }
          }
label_13:
          FatalErrorMgr.Get().SetErrorCode("HS", errorSubset1);
          NetCache.m_fatalErrorCodeSet = true;
          cacheRequestScratch.m_errorCallback(info);
        }
      }
    }
    this.CheckSeasonForRoll();
  }

  private void OnGenericResponse()
  {
    Network.GenericResponse genericResponse = Network.Get().GetGenericResponse();
    if (genericResponse == null)
    {
      Debug.LogError((object) string.Format("NetCache - GenericResponse parse error"));
    }
    else
    {
      if (genericResponse.RequestId != 201)
        return;
      System.Type key;
      if (!NetCache.m_requestTypeMap.TryGetValue((GetAccountInfo.Request) genericResponse.RequestSubId, out key))
      {
        Debug.LogError((object) string.Format("NetCache - Ignoring unexpected requestId={0}:{1}", (object) genericResponse.RequestId, (object) genericResponse.RequestSubId));
      }
      else
      {
        foreach (NetCache.NetCacheBatchRequest cacheBatchRequest in this.m_cacheRequests.ToArray())
        {
          if (cacheBatchRequest.m_requests.ContainsKey(key))
          {
            switch (genericResponse.ResultCode)
            {
              case Network.GenericResponse.Result.RESULT_REQUEST_IN_PROCESS:
                if (NetCache.RequestResult.PENDING == cacheBatchRequest.m_requests[key].m_result)
                {
                  cacheBatchRequest.m_requests[key].m_result = NetCache.RequestResult.IN_PROCESS;
                  continue;
                }
                continue;
              case Network.GenericResponse.Result.RESULT_REQUEST_COMPLETE:
                cacheBatchRequest.m_requests[key].m_result = NetCache.RequestResult.GENERIC_COMPLETE;
                Debug.LogWarning((object) string.Format("GenericResponse Success for requestId={0}:{1}", (object) genericResponse.RequestId, (object) genericResponse.RequestSubId));
                continue;
              case Network.GenericResponse.Result.RESULT_DATA_MIGRATION_REQUIRED:
                cacheBatchRequest.m_requests[key].m_result = NetCache.RequestResult.MIGRATION_REQUIRED;
                Debug.LogWarning((object) string.Format("GenericResponse player migration required code={0} {1} for requestId={2}:{3}", (object) (int) genericResponse.ResultCode, (object) genericResponse.ResultCode.ToString(), (object) genericResponse.RequestId, (object) genericResponse.RequestSubId));
                continue;
              default:
                Debug.LogError((object) string.Format("Unhandled failure code={0} {1} for requestId={2}:{3}", (object) (int) genericResponse.ResultCode, (object) genericResponse.ResultCode.ToString(), (object) genericResponse.RequestId, (object) genericResponse.RequestSubId));
                cacheBatchRequest.m_requests[key].m_result = NetCache.RequestResult.ERROR;
                NetCache.ErrorInfo info = new NetCache.ErrorInfo();
                info.Error = NetCache.ErrorCode.SERVER;
                info.ServerError = (uint) genericResponse.ResultCode;
                info.RequestingFunction = cacheBatchRequest.m_requestFunc;
                info.RequestedTypes = new Map<System.Type, NetCache.Request>((IEnumerable<KeyValuePair<System.Type, NetCache.Request>>) cacheBatchRequest.m_requests);
                info.RequestStackTrace = cacheBatchRequest.m_requestStackTrace;
                FatalErrorMgr.Get().SetErrorCode("HS", "CG" + genericResponse.ResultCode.ToString(), genericResponse.RequestId.ToString(), genericResponse.RequestSubId.ToString());
                cacheBatchRequest.m_errorCallback(info);
                continue;
            }
          }
        }
      }
    }
  }

  private void OnDBAction()
  {
    Network.DBAction dbAction = Network.Get().GetDbAction();
    if (Network.DBAction.ResultType.SUCCESS == dbAction.Result)
      return;
    Debug.LogError((object) string.Format("Unhandled dbAction {0} with error {1}", (object) dbAction.Action, (object) dbAction.Result));
  }

  private void OnInitialClientState()
  {
    InitialClientState initialClientState = Network.Get().GetInitialClientState();
    if (initialClientState == null)
      return;
    this.m_receivedInitialClientState = true;
    if (initialClientState.HasGuardianVars)
      this.OnGuardianVars(initialClientState.GuardianVars);
    if (initialClientState.HasPlayerProfileProgress)
      this.OnNetCacheObjReceived<NetCache.NetCacheProfileProgress>(new NetCache.NetCacheProfileProgress()
      {
        CampaignProgress = (TutorialProgress) initialClientState.PlayerProfileProgress.Progress,
        BestForgeWins = initialClientState.PlayerProfileProgress.BestForge,
        LastForgeDate = initialClientState.PlayerProfileProgress.HasLastForge ? TimeUtils.PegDateToFileTimeUtc(initialClientState.PlayerProfileProgress.LastForge) : 0L
      });
    if (initialClientState.GameSaveData != null)
      GameSaveDataManager.Get().ApplyGameSaveDataFromInitialClientState();
    if (initialClientState.SpecialEventTiming.Count > 0)
    {
      long devTimeOffsetSeconds = initialClientState.HasDevTimeOffsetSeconds ? initialClientState.DevTimeOffsetSeconds : 0L;
      SpecialEventManager.Get().InitEventTimingsFromServer(devTimeOffsetSeconds, (IList<SpecialEventTiming>) initialClientState.SpecialEventTiming);
    }
    if (initialClientState.HasClientOptions)
      this.OnClientOptions(initialClientState.ClientOptions);
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    if (initialClientState.HasCollection)
      this.OnCollection(ref data, initialClientState.Collection);
    else
      this.OnCollection(ref data, data.Collection);
    if (initialClientState.HasAchievements)
      AchieveManager.Get().OnInitialAchievements(initialClientState.Achievements);
    if (initialClientState.HasNotices)
      this.OnInitialClientState_ProfileNotices(initialClientState.Notices);
    if (initialClientState.HasGameCurrencyStates)
      this.OnCurrencyState(initialClientState.GameCurrencyStates);
    if (initialClientState.HasBoosters)
      this.OnBoosters(initialClientState.Boosters);
    if (initialClientState.HasPlayerDraftTickets)
      this.OnPlayerDraftTickets(initialClientState.PlayerDraftTickets);
    foreach (object tavernBrawls in initialClientState.TavernBrawlsList)
    {
      PegasusPacket packet = new PegasusPacket(316, 0, tavernBrawls);
      Network.Get().SimulateReceivedPacketFromServer(packet);
    }
    if (initialClientState.HasDisconnectedGame)
      this.OnDisconnectedGame(initialClientState.DisconnectedGame);
    if (initialClientState.HasArenaSession)
    {
      PegasusPacket packet = new PegasusPacket(351, 0, (object) initialClientState.ArenaSession);
      Network.Get().SimulateReceivedPacketFromServer(packet);
    }
    if (initialClientState.HasDisplayBanner)
      this.OnDisplayBanner(initialClientState.DisplayBanner);
    if (initialClientState.Decks != null)
      this.OnReceivedDeckHeaders_InitialClientState(ref data, initialClientState.Decks, initialClientState.DeckContents, initialClientState.ValidCachedDeckIds);
    OfflineDataCache.WriteOfflineDataToFile(data);
    if (initialClientState.MedalInfo != null)
      this.OnMedalInfo(initialClientState.MedalInfo);
    if (HearthstoneApplication.IsInternal() && initialClientState.HasPlayerId)
    {
      SceneDebugger service;
      if (!ServiceManager.TryGet<SceneDebugger>(out service))
        return;
      service.SetPlayerId(new long?(initialClientState.PlayerId));
    }
    if (Network.Get() == null)
      return;
    Network.Get().OnInitialClientStateProcessed();
  }

  public void OnCollection(ref OfflineDataCache.OfflineData data, Collection collection)
  {
    this.m_initialCollectionVersion = collection.CollectionVersion;
    if (CollectionManager.Get() != null)
      this.OnNetCacheObjReceived<NetCache.NetCacheCollection>(CollectionManager.Get().OnInitialCollectionReceived(collection));
    OfflineDataCache.CacheCollection(ref data, collection);
  }

  private void OnBoosters(Boosters boosters)
  {
    NetCache.NetCacheBoosters netCacheObject = new NetCache.NetCacheBoosters();
    for (int index = 0; index < boosters.List.Count; ++index)
    {
      BoosterInfo boosterInfo = boosters.List[index];
      NetCache.BoosterStack boosterStack = new NetCache.BoosterStack()
      {
        Id = boosterInfo.Type,
        Count = boosterInfo.Count,
        EverGrantedCount = boosterInfo.EverGrantedCount
      };
      netCacheObject.BoosterStacks.Add(boosterStack);
    }
    this.OnNetCacheObjReceived<NetCache.NetCacheBoosters>(netCacheObject);
  }

  public void OnPlayerDraftTickets(PlayerDraftTickets playerDraftTickets) => this.OnNetCacheObjReceived<NetCache.NetPlayerArenaTickets>(new NetCache.NetPlayerArenaTickets()
  {
    Balance = playerDraftTickets.UnusedTicketBalance
  });

  private void OnDisconnectedGame(GameConnectionInfo packet)
  {
    if (!packet.HasAddress)
      return;
    NetCache.NetCacheDisconnectedGame netCacheObject = new NetCache.NetCacheDisconnectedGame()
    {
      ServerInfo = new GameServerInfo()
    };
    netCacheObject.ServerInfo.Address = packet.Address;
    netCacheObject.ServerInfo.GameHandle = (uint) packet.GameHandle;
    netCacheObject.ServerInfo.ClientHandle = packet.ClientHandle;
    netCacheObject.ServerInfo.Port = (uint) packet.Port;
    netCacheObject.ServerInfo.AuroraPassword = packet.AuroraPassword;
    netCacheObject.ServerInfo.Mission = packet.Scenario;
    netCacheObject.ServerInfo.BrawlLibraryItemId = packet.BrawlLibraryItemId;
    netCacheObject.ServerInfo.Version = BattleNet.GetVersion();
    netCacheObject.ServerInfo.Resumable = true;
    netCacheObject.GameType = packet.GameType;
    netCacheObject.FormatType = packet.FormatType;
    netCacheObject.LoadGameState = packet.HasLoadGameState && packet.LoadGameState;
    this.OnNetCacheObjReceived<NetCache.NetCacheDisconnectedGame>(netCacheObject);
  }

  private void OnDisplayBanner(int displayBanner) => this.OnNetCacheObjReceived<NetCache.NetCacheDisplayBanner>(new NetCache.NetCacheDisplayBanner()
  {
    Id = displayBanner
  });

  private void OnReceivedDeckHeaders() => this.OnNetCacheObjReceived<NetCache.NetCacheDecks>(Network.Get().GetDeckHeaders());

  private void OnReceivedDeckHeaders_InitialClientState(
    ref OfflineDataCache.OfflineData data,
    List<DeckInfo> deckHeaders,
    List<PegasusUtil.DeckContents> deckContents,
    List<long> validCachedDeckIds)
  {
    foreach (DeckInfo fakeDeckInfo in OfflineDataCache.GetFakeDeckInfos(data))
      deckHeaders.Add(fakeDeckInfo);
    NetCache.NetCacheDecks deckHeaders1 = Network.GetDeckHeaders(deckHeaders);
    this.OnNetCacheObjReceived<NetCache.NetCacheDecks>(deckHeaders1);
    Network.Get().ReconcileDeckContentsForChangedOfflineDecks(ref data, deckHeaders, deckContents, validCachedDeckIds);
    CollectionManager.Get().OnInitialClientStateDeckContents(deckHeaders1, data.LocalDeckContents);
  }

  public List<DeckInfo> GetDeckListFromNetCache()
  {
    List<DeckInfo> listFromNetCache = new List<DeckInfo>();
    foreach (NetCache.DeckHeader deck in this.GetNetObject<NetCache.NetCacheDecks>().Decks)
      listFromNetCache.Add(Network.GetDeckInfoFromDeckHeader(deck));
    return listFromNetCache;
  }

  private void OnCardValues()
  {
    NetCache.NetCacheCardValues netCacheObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardValues>();
    CardValues cardValues = Network.Get().GetCardValues();
    if (cardValues != null)
    {
      if (netCacheObject == null)
        netCacheObject = new NetCache.NetCacheCardValues(cardValues.Cards.Count);
      SpecialEventManager specialEventManager = SpecialEventManager.Get();
      foreach (PegasusUtil.CardValue card in cardValues.Cards)
      {
        string cardId = GameUtils.TranslateDbIdToCardId(card.Card.Asset);
        if (cardId == null)
          Log.All.PrintError("NetCache.OnCardValues(): Cannot find card '{0}' in card manifest.  Confirm your card manifest matches your game server's database.", (object) card.Card.Asset);
        else
          netCacheObject.Values.Add(new NetCache.CardDefinition()
          {
            Name = cardId,
            Premium = (TAG_PREMIUM) card.Card.Premium
          }, new NetCache.CardValue()
          {
            BaseBuyValue = card.Buy,
            BaseSellValue = card.Sell,
            BaseUpgradeValue = card.Upgrade,
            BuyValueOverride = card.HasBuyValueOverride ? card.BuyValueOverride : 0,
            SellValueOverride = card.HasSellValueOverride ? card.SellValueOverride : 0,
            OverrideEvent = card.HasOverrideEventName ? specialEventManager.GetEventType(card.OverrideEventName) : SpecialEventType.SPECIAL_EVENT_NEVER
          });
      }
    }
    else if (netCacheObject == null)
      netCacheObject = new NetCache.NetCacheCardValues();
    this.OnNetCacheObjReceived<NetCache.NetCacheCardValues>(netCacheObject);
  }

  private void OnMedalInfo()
  {
    NetCache.NetCacheMedalInfo medalInfo = Network.Get().GetMedalInfo();
    if (this.m_previousMedalInfo != null)
      medalInfo.PreviousMedalInfo = this.m_previousMedalInfo.Clone();
    this.m_previousMedalInfo = medalInfo;
    this.OnNetCacheObjReceived<NetCache.NetCacheMedalInfo>(medalInfo);
  }

  private void OnMedalInfo(MedalInfo packet)
  {
    NetCache.NetCacheMedalInfo netCacheObject = new NetCache.NetCacheMedalInfo(packet);
    if (this.m_previousMedalInfo != null)
      netCacheObject.PreviousMedalInfo = this.m_previousMedalInfo.Clone();
    this.m_previousMedalInfo = netCacheObject;
    this.OnNetCacheObjReceived<NetCache.NetCacheMedalInfo>(netCacheObject);
  }

  private void OnBaconRatingInfo() => this.OnNetCacheObjReceived<NetCache.NetCacheBaconRatingInfo>(Network.Get().GetBaconRatingInfo());

  public long GetDuelsEarlyAccessLicenseId()
  {
    NetCache.NetCacheFeatures netObject = this.GetNetObject<NetCache.NetCacheFeatures>();
    return netObject != null ? (long) netObject.DuelsEarlyAccessLicense : 77345L;
  }

  private void OnPVPDRStatsInfo() => this.OnNetCacheObjReceived<NetCache.NetCachePVPDRStatsInfo>(Network.Get().GetPVPDRStatsInfo());

  private void OnLettuceMapResponse()
  {
    LettuceMapResponse lettuceMapResponse = Network.Get().GetLettuceMapResponse();
    this.OnNetCacheObjReceived<NetCache.NetCacheLettuceMap>(new NetCache.NetCacheLettuceMap()
    {
      Map = lettuceMapResponse.Map
    });
  }

  private Dictionary<MercenaryBuilding.Mercenarybuildingtype, bool> MakeBuildingEnabledMap(
    MercenariesOperabilityData opData)
  {
    if (opData == null)
      return new Dictionary<MercenaryBuilding.Mercenarybuildingtype, bool>()
      {
        {
          MercenaryBuilding.Mercenarybuildingtype.BUILDINGMANAGER,
          false
        },
        {
          MercenaryBuilding.Mercenarybuildingtype.COLLECTION,
          false
        },
        {
          MercenaryBuilding.Mercenarybuildingtype.MAILBOX,
          false
        },
        {
          MercenaryBuilding.Mercenarybuildingtype.PVEZONES,
          false
        },
        {
          MercenaryBuilding.Mercenarybuildingtype.PVP,
          false
        },
        {
          MercenaryBuilding.Mercenarybuildingtype.SHOP,
          false
        },
        {
          MercenaryBuilding.Mercenarybuildingtype.TASKBOARD,
          false
        },
        {
          MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL,
          false
        }
      };
    return new Dictionary<MercenaryBuilding.Mercenarybuildingtype, bool>()
    {
      {
        MercenaryBuilding.Mercenarybuildingtype.BUILDINGMANAGER,
        !opData.HasBuildingManagementEnabled || opData.BuildingManagementEnabled
      },
      {
        MercenaryBuilding.Mercenarybuildingtype.COLLECTION,
        !opData.HasCollectionPortalEnabled || opData.CollectionPortalEnabled
      },
      {
        MercenaryBuilding.Mercenarybuildingtype.MAILBOX,
        !opData.HasInGameMessagingEnabled || opData.InGameMessagingEnabled
      },
      {
        MercenaryBuilding.Mercenarybuildingtype.PVEZONES,
        !opData.HasPvePortalEnabled || opData.PvePortalEnabled
      },
      {
        MercenaryBuilding.Mercenarybuildingtype.PVP,
        !opData.HasPvpPortalEnabled || opData.PvpPortalEnabled
      },
      {
        MercenaryBuilding.Mercenarybuildingtype.SHOP,
        !opData.HasShopPortalEnabled || opData.ShopPortalEnabled
      },
      {
        MercenaryBuilding.Mercenarybuildingtype.TASKBOARD,
        !opData.HasTasksEnabled || opData.TasksEnabled
      },
      {
        MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL,
        !opData.HasTrainingHallEnabled || opData.TrainingHallEnabled
      }
    };
  }

  private void OnMercenariesPlayerInfoResponse()
  {
    MercenariesPlayerInfoResponse playerInfoResponse = Network.Get().MercenariesPlayerInfoResponse();
    if (playerInfoResponse == null)
      Log.CollectionManager.PrintError("OnMercenariesPlayerInfoResponse(): No response received.");
    else if (!playerInfoResponse.HasPvpRewardChestWinsProgress)
      Log.CollectionManager.PrintError("OnMercenariesPlayerInfoResponse(): No pvp reward chest wins progress received.");
    else if (!playerInfoResponse.HasPvpRewardChestWinsRequired)
    {
      Log.CollectionManager.PrintError("OnMercenariesPlayerInfoResponse(): No pvp reward chest wins required received.");
    }
    else
    {
      Dictionary<int, NetCache.NetCacheMercenariesPlayerInfo.BountyInfo> dictionary = new Dictionary<int, NetCache.NetCacheMercenariesPlayerInfo.BountyInfo>();
      foreach (MercenariesPlayerBountyInfo playerBountyInfo in playerInfoResponse.BountyInfoList.BountyInfo)
      {
        NetCache.NetCacheMercenariesPlayerInfo.BountyInfo bountyInfo = new NetCache.NetCacheMercenariesPlayerInfo.BountyInfo()
        {
          FewestTurns = (int) playerBountyInfo.FewestTurns,
          Completions = (int) playerBountyInfo.Completions,
          IsComplete = playerBountyInfo.IsComplete,
          IsAcknowledged = playerBountyInfo.Acknowledged
        };
        dictionary.Add((int) playerBountyInfo.BountyId, bountyInfo);
      }
      MercenariesOperabilityData opData = playerInfoResponse.HasOperabilityData ? playerInfoResponse.OperabilityData : (MercenariesOperabilityData) null;
      this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesPlayerInfo>(new NetCache.NetCacheMercenariesPlayerInfo()
      {
        PvpRating = playerInfoResponse.PvpRating,
        PvpRewardChestWinsProgress = playerInfoResponse.PvpRewardChestWinsProgress,
        PvpRewardChestWinsRequired = playerInfoResponse.PvpRewardChestWinsRequired,
        BountyInfoMap = dictionary,
        BuildingEnabledMap = this.MakeBuildingEnabledMap(opData),
        DisabledMercenaryList = opData?.DisabledMercenaryId ?? new List<int>(),
        DisabledVisitorList = new HashSet<int>((IEnumerable<int>) (opData?.DisabledVisitorId ?? new List<int>())),
        PvpSeasonHighestRating = playerInfoResponse.PvpSeasonHighestRating,
        PvpSeasonId = playerInfoResponse.PvpSeasonId
      });
    }
  }

  public void UpdateNetCachePlayerInfoAcknowledgedBounties(List<int> bountiesToAcknowledge)
  {
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    foreach (int key in bountiesToAcknowledge)
    {
      if (netObject.BountyInfoMap.ContainsKey(key))
        netObject.BountyInfoMap[key].IsAcknowledged = true;
      else
        netObject.BountyInfoMap[key] = new NetCache.NetCacheMercenariesPlayerInfo.BountyInfo()
        {
          IsAcknowledged = true,
          IsComplete = false,
          Completions = 0,
          FewestTurns = 0
        };
    }
    this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesPlayerInfo>(netObject);
  }

  private void OnMercenariesPvPWinUpdate()
  {
    MercenariesPvPWinUpdate mercenariesPvPwinUpdate = Network.Get().MercenariesPvPWinUpdate();
    if (mercenariesPvPwinUpdate == null)
      Log.CollectionManager.PrintError("OnMercenariesPvPWinUpdate(): No response received.");
    else if (!mercenariesPvPwinUpdate.HasPvpRewardChestWinsProgress)
      Log.CollectionManager.PrintError("OnMercenariesPvPWinUpdate(): No pvp reward chest wins progress received.");
    else if (!mercenariesPvPwinUpdate.HasPvpRewardChestWinsRequired)
    {
      Log.CollectionManager.PrintError("OnMercenariesPvPWinUpdate(): No pvp reward chest wins required received.");
    }
    else
    {
      NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
      if (netObject == null)
        return;
      netObject.PvpRewardChestWinsProgress = mercenariesPvPwinUpdate.PvpRewardChestWinsProgress;
      netObject.PvpRewardChestWinsRequired = mercenariesPvPwinUpdate.PvpRewardChestWinsRequired;
      this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesPlayerInfo>(netObject);
    }
  }

  private void OnMercenariesPlayerBountyInfoUpdate()
  {
    MercenariesPlayerBountyInfoUpdate bountyInfoUpdate = Network.Get().MercenariesPlayerBountyInfoUpdate();
    if (bountyInfoUpdate == null)
    {
      Log.CollectionManager.PrintError("OnMercenariesPlayerBountyInfoUpdate(): No response received.");
    }
    else
    {
      NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
      if (netObject == null)
      {
        Log.CollectionManager.PrintError("OnMercenariesPlayerBountyInfoUpdate(): No player info.");
      }
      else
      {
        if (netObject.BountyInfoMap == null)
          netObject.BountyInfoMap = new Dictionary<int, NetCache.NetCacheMercenariesPlayerInfo.BountyInfo>();
        NetCache.NetCacheMercenariesPlayerInfo.BountyInfo bountyInfo;
        netObject.BountyInfoMap.TryGetValue((int) bountyInfoUpdate.BountyId, out bountyInfo);
        if (bountyInfo != null)
        {
          if (bountyInfo.FewestTurns == 0)
            bountyInfo.FewestTurns = (int) bountyInfoUpdate.FewestTurns;
          else if (bountyInfoUpdate.FewestTurns != 0U)
            bountyInfo.FewestTurns = Math.Min(bountyInfo.FewestTurns, (int) bountyInfoUpdate.FewestTurns);
          bountyInfo.Completions = Math.Max(bountyInfo.Completions, (int) bountyInfoUpdate.Completions);
          bountyInfo.IsComplete = bountyInfo.IsComplete || bountyInfoUpdate.IsComplete;
          bountyInfo.IsAcknowledged = bountyInfo.IsAcknowledged || bountyInfoUpdate.IsAcknowledged;
        }
        else
          netObject.BountyInfoMap[(int) bountyInfoUpdate.BountyId] = new NetCache.NetCacheMercenariesPlayerInfo.BountyInfo()
          {
            FewestTurns = (int) bountyInfoUpdate.FewestTurns,
            Completions = (int) bountyInfoUpdate.Completions,
            IsComplete = bountyInfoUpdate.IsComplete,
            IsAcknowledged = bountyInfoUpdate.IsAcknowledged
          };
        this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesPlayerInfo>(netObject);
      }
    }
  }

  private static int CompareVisitorStates(MercenariesVisitorState x, MercenariesVisitorState y)
  {
    MercenaryVisitorDbfRecord visitorRecordById1 = LettuceVillageDataUtil.GetVisitorRecordByID(x.VisitorId);
    MercenaryVisitorDbfRecord visitorRecordById2 = LettuceVillageDataUtil.GetVisitorRecordByID(y.VisitorId);
    if (visitorRecordById1 == null || visitorRecordById2 == null)
      return 0;
    if (visitorRecordById1.VisitorType > visitorRecordById2.VisitorType)
      return -1;
    if (visitorRecordById2.VisitorType > visitorRecordById1.VisitorType)
      return 1;
    long fileTimeUtc = TimeUtils.PegDateToFileTimeUtc(x.LastArrivalDate);
    return TimeUtils.PegDateToFileTimeUtc(y.LastArrivalDate).CompareTo(fileTimeUtc);
  }

  private void OnMercenariesBountyAcknowledgeResponse() => Network.Get().AcknowledgeBountiesResponse();

  private void OnVillageDataResponse()
  {
    MercenariesGetVillageResponse getVillageResponse = Network.Get().MercenariesVillageStatusResponse();
    NetCache.NetCacheMercenariesVillageInfo netCacheObject = new NetCache.NetCacheMercenariesVillageInfo();
    NetCache.NetCacheMercenariesVillageVisitorInfo villageVisitorInfo = new NetCache.NetCacheMercenariesVillageVisitorInfo();
    netCacheObject.Initialized = true;
    if (getVillageResponse == null)
    {
      Log.CollectionManager.PrintError("OnVillageDataResponse(): No response received.");
      this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesVillageInfo>(netCacheObject);
      this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesVillageVisitorInfo>(villageVisitorInfo);
    }
    else
    {
      if (!getVillageResponse.Success)
        Debug.LogError((object) "Failed to load village data");
      villageVisitorInfo.VisitorStates = getVillageResponse.Visitor ?? new List<MercenariesVisitorState>();
      villageVisitorInfo.VisitorStates.Sort(new Comparison<MercenariesVisitorState>(NetCache.CompareVisitorStates));
      villageVisitorInfo.CompletedTasks = new List<MercenariesTaskState>();
      villageVisitorInfo.CompletedVisitorStates = getVillageResponse.CompletedVisitor ?? new List<MercenariesCompletedVisitorState>();
      villageVisitorInfo.ActiveRenownOffers = getVillageResponse.RenownOffer ?? new List<MercenariesRenownOfferData>();
      this.CollectVisitingMercenariesFromVisitorStates(villageVisitorInfo);
      netCacheObject.BuildingStates = new List<MercenariesBuildingState>();
      foreach (MercenariesBuildingState bldgState in getVillageResponse.Building)
      {
        if (this.IsBuildingStateValid(bldgState))
        {
          netCacheObject.TrySetDifficultyUnlock(bldgState);
          netCacheObject.BuildingStates.Add(bldgState);
        }
      }
      netCacheObject.CacheTierTree();
      netCacheObject.CacheRenownConversionRates(getVillageResponse.RenownConversionRate);
      this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesVillageInfo>(netCacheObject);
      this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesVillageVisitorInfo>(villageVisitorInfo);
      NarrativeManager.Get().PreloadDialogForActiveVillageBuildings();
      GameSaveDataManager.Get().ApplyGameSaveDataUpdate(getVillageResponse.GameSaveData);
    }
  }

  private void OnVillageVisitorStateUpdated()
  {
    MercenariesVisitorStateUpdate visitorStateUpdate = Network.Get().MercenariesVisitorStateUpdate();
    if (visitorStateUpdate == null)
    {
      Log.CollectionManager.PrintError("OnVillageVisitorStateUpdated(): No response received.");
    }
    else
    {
      NetCache.NetCacheMercenariesVillageVisitorInfo villageVisitorInfo = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
      if (villageVisitorInfo == null)
        villageVisitorInfo = new NetCache.NetCacheMercenariesVillageVisitorInfo()
        {
          VisitorStates = new List<MercenariesVisitorState>()
        };
      if (visitorStateUpdate.Visitor != null)
      {
        foreach (MercenariesVisitorState mercenariesVisitorState in visitorStateUpdate.Visitor)
        {
          MercenariesVisitorState stateUpdate = mercenariesVisitorState;
          if (!villageVisitorInfo.VisitorStates.Exists((Predicate<MercenariesVisitorState>) (state => state.VisitorId == stateUpdate.VisitorId)))
          {
            villageVisitorInfo.VisitorStates.Add(stateUpdate);
          }
          else
          {
            for (int index = villageVisitorInfo.VisitorStates.Count - 1; index >= 0; --index)
            {
              MercenariesVisitorState visitorState = villageVisitorInfo.VisitorStates[index];
              if (visitorState.VisitorId == stateUpdate.VisitorId)
              {
                if (stateUpdate.ActiveTaskState == null || stateUpdate.ActiveTaskState.TaskId == 0)
                {
                  if (visitorState.ActiveTaskState != null)
                  {
                    VisitorTaskChainDbfRecord chainByVisitorState = LettuceVillageDataUtil.GetCurrentTaskChainByVisitorState(visitorState);
                    if (chainByVisitorState != null && stateUpdate.TaskChainProgress >= chainByVisitorState.TaskList.Count)
                      villageVisitorInfo.CompletedVisitorStates.Add(new MercenariesCompletedVisitorState()
                      {
                        VisitorId = stateUpdate.VisitorId,
                        CompletedTaskChainId = chainByVisitorState.ID
                      });
                  }
                  villageVisitorInfo.VisitorStates.RemoveAt(index);
                }
                else
                  villageVisitorInfo.VisitorStates[index] = stateUpdate;
              }
            }
          }
          if (stateUpdate.HasActiveTaskState && stateUpdate.ActiveTaskState.Status_ == MercenariesTaskState.Status.COMPLETE)
          {
            if (villageVisitorInfo.CompletedTasks == null)
              villageVisitorInfo.CompletedTasks = new List<MercenariesTaskState>();
            villageVisitorInfo.CompletedTasks.Add(stateUpdate.ActiveTaskState);
          }
        }
        villageVisitorInfo.VisitorStates.Sort(new Comparison<MercenariesVisitorState>(NetCache.CompareVisitorStates));
      }
      if (visitorStateUpdate.UpdatedRenownOffer != null && visitorStateUpdate.UpdatedRenownOffer.Count > 0)
      {
        foreach (MercenariesRenownOfferData mercenariesRenownOfferData in visitorStateUpdate.UpdatedRenownOffer)
        {
          bool flag = false;
          for (int index = villageVisitorInfo.ActiveRenownOffers.Count - 1; index >= 0; --index)
          {
            if (villageVisitorInfo.ActiveRenownOffers[index].RenownOfferId == mercenariesRenownOfferData.RenownOfferId)
            {
              villageVisitorInfo.ActiveRenownOffers[index] = mercenariesRenownOfferData;
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            if (villageVisitorInfo.ActiveRenownOffers == null)
              villageVisitorInfo.ActiveRenownOffers = new List<MercenariesRenownOfferData>();
            villageVisitorInfo.ActiveRenownOffers.Add(mercenariesRenownOfferData);
          }
        }
      }
      if (visitorStateUpdate.RemovedRenownOfferId != null && visitorStateUpdate.RemovedRenownOfferId.Count > 0 && villageVisitorInfo.ActiveRenownOffers != null)
      {
        for (int index = villageVisitorInfo.ActiveRenownOffers.Count - 1; index >= 0; --index)
        {
          MercenariesRenownOfferData activeRenownOffer = villageVisitorInfo.ActiveRenownOffers[index];
          if (visitorStateUpdate.RemovedRenownOfferId.Contains(activeRenownOffer.RenownOfferId))
            villageVisitorInfo.ActiveRenownOffers.RemoveAt(index);
        }
      }
      GameSaveDataManager.Get().ApplyGameSaveDataUpdate(visitorStateUpdate.GameSaveData);
      this.CollectVisitingMercenariesFromVisitorStates(villageVisitorInfo);
      this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesVillageVisitorInfo>(villageVisitorInfo);
    }
  }

  private void CollectVisitingMercenariesFromVisitorStates(
    NetCache.NetCacheMercenariesVillageVisitorInfo visitorInfo)
  {
    HashSet<int> source = new HashSet<int>();
    HashSet<int> disabledVisitorList = this.GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>().DisabledVisitorList;
    foreach (MercenariesVisitorState visitorState in visitorInfo.VisitorStates)
    {
      if (!disabledVisitorList.Contains(visitorState.VisitorId) && visitorState.ActiveTaskState != null)
      {
        MercenaryVisitorDbfRecord visitorRecordById = LettuceVillageDataUtil.GetVisitorRecordByID(visitorState.VisitorId);
        if (visitorRecordById.VisitorType != MercenaryVisitor.VillageVisitorType.STANDARD)
        {
          VisitorTaskDbfRecord taskRecordById = LettuceVillageDataUtil.GetTaskRecordByID(visitorState.ActiveTaskState.TaskId);
          if (visitorRecordById != null && taskRecordById != null)
          {
            if (visitorRecordById.VisitorType == MercenaryVisitor.VillageVisitorType.PROCEDURAL)
              source.Add(visitorState.ProceduralMercenaryId);
            else
              source.Add(LettuceVillageDataUtil.GetMercenaryIdForVisitor(visitorRecordById, taskRecordById));
          }
        }
      }
    }
    if (visitorInfo.ActiveRenownOffers != null)
    {
      foreach (MercenariesRenownOfferData activeRenownOffer in visitorInfo.ActiveRenownOffers)
      {
        if (activeRenownOffer.MercenaryId != 0)
          source.Add(activeRenownOffer.MercenaryId);
      }
    }
    visitorInfo.VisitingMercenaries = source.ToArray<int>();
  }

  private void OnRefreshVisitorDataResponse()
  {
    MercenariesRefreshVisitorsResponse visitorsResponse = Network.Get().MercenariesVisitorRefreshResponse();
    if (visitorsResponse != null && visitorsResponse.Success)
      return;
    Debug.LogError((object) string.Format("Failed to refresh visitor data"));
  }

  private bool IsBuildingStateValid(MercenariesBuildingState bldgState)
  {
    if (bldgState == null)
      return false;
    MercenaryBuildingDbfRecord bldgRecord = GameDbf.MercenaryBuilding.GetRecord((Predicate<MercenaryBuildingDbfRecord>) (r => r.ID == bldgState.BuildingId));
    return bldgRecord != null && GameDbf.BuildingTier.GetRecords((Predicate<BuildingTierDbfRecord>) (r => r.MercenaryBuildingId == bldgRecord.ID && r.ID == bldgState.CurrentTierId)) != null;
  }

  private void OnVillageBuildingStateUpdated()
  {
    MercenariesBuildingStateUpdate buildingStateUpdate = Network.Get().MercenariesBuildingStateUpdate();
    NetCache.NetCacheMercenariesVillageInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>();
    foreach (MercenariesBuildingState bldgState in buildingStateUpdate.Building)
    {
      if (this.IsBuildingStateValid(bldgState))
      {
        bool flag = false;
        for (int index = 0; index < netObject.BuildingStates.Count; ++index)
        {
          if (netObject.BuildingStates[index].BuildingId == bldgState.BuildingId)
          {
            netObject.BuildingStates[index] = bldgState;
            flag = true;
            break;
          }
        }
        if (!flag)
          netObject.BuildingStates.Add(bldgState);
        netObject.TrySetDifficultyUnlock(bldgState);
      }
    }
    GameSaveDataManager.Get().ApplyGameSaveDataUpdate(buildingStateUpdate.GameSaveData);
    netObject.LastBuildingUpdate = buildingStateUpdate.Building;
    this.OnNetCacheObjReceived<NetCache.NetCacheMercenariesVillageInfo>(netObject);
  }

  private void OnGuardianVars()
  {
    GuardianVars guardianVars = Network.Get().GetGuardianVars();
    if (guardianVars == null)
      return;
    this.OnGuardianVars(guardianVars);
  }

  private void OnGuardianVars(GuardianVars packet)
  {
    NetCache.NetCacheFeatures netCacheObject = new NetCache.NetCacheFeatures();
    netCacheObject.Games.Tournament = !packet.HasTourney || packet.Tourney;
    netCacheObject.Games.Practice = !packet.HasPractice || packet.Practice;
    netCacheObject.Games.Casual = !packet.HasCasual || packet.Casual;
    netCacheObject.Games.Forge = !packet.HasForge || packet.Forge;
    netCacheObject.Games.Friendly = !packet.HasFriendly || packet.Friendly;
    netCacheObject.Games.TavernBrawl = !packet.HasTavernBrawl || packet.TavernBrawl;
    netCacheObject.Games.Battlegrounds = !packet.HasBattlegrounds || packet.Battlegrounds;
    netCacheObject.Games.BattlegroundsFriendlyChallenge = !packet.HasBattlegroundsFriendlyChallenge || packet.BattlegroundsFriendlyChallenge;
    netCacheObject.Games.BattlegroundsTutorial = !packet.HasBattlegroundsTutorial || packet.BattlegroundsTutorial;
    netCacheObject.Games.ShowUserUI = packet.HasShowUserUI ? packet.ShowUserUI : 0;
    netCacheObject.Games.Duels = !packet.HasDuels || packet.Duels;
    netCacheObject.Games.PaidDuels = !packet.HasPaidDuels || packet.PaidDuels;
    netCacheObject.Games.Mercenaries = !packet.HasMercenaries || packet.Mercenaries;
    netCacheObject.Games.MercenariesAI = !packet.HasMercenariesAi || packet.MercenariesAi;
    netCacheObject.Games.MercenariesCoOp = !packet.HasMercenariesCoop || packet.MercenariesCoop;
    netCacheObject.Games.MercenariesFriendly = !packet.HasMercenariesFriendlyChallenge || packet.MercenariesFriendlyChallenge;
    netCacheObject.Collection.Manager = !packet.HasManager || packet.Manager;
    netCacheObject.Collection.Crafting = !packet.HasCrafting || packet.Crafting;
    netCacheObject.Collection.DeckReordering = !packet.HasDeckReordering || packet.DeckReordering;
    netCacheObject.Collection.MultipleFavoriteCardBacks = !packet.HasMultipleFavoriteCardBacks || packet.MultipleFavoriteCardBacks;
    netCacheObject.Store.Store = !packet.HasStore || packet.Store;
    netCacheObject.Store.BattlePay = !packet.HasBattlePay || packet.BattlePay;
    netCacheObject.Store.BuyWithGold = !packet.HasBuyWithGold || packet.BuyWithGold;
    netCacheObject.Store.SimpleCheckout = !packet.HasSimpleCheckout || packet.SimpleCheckout;
    netCacheObject.Store.SoftAccountPurchasing = !packet.HasSoftAccountPurchasing || packet.SoftAccountPurchasing;
    netCacheObject.Store.VirtualCurrencyEnabled = packet.HasVirtualCurrencyEnabled && packet.VirtualCurrencyEnabled;
    netCacheObject.Store.NumClassicPacksUntilDeprioritize = packet.HasNumClassicPacksUntilDeprioritize ? packet.NumClassicPacksUntilDeprioritize : -1;
    netCacheObject.Store.SimpleCheckoutIOS = !packet.HasSimpleCheckoutIos || packet.SimpleCheckoutIos;
    netCacheObject.Store.SimpleCheckoutAndroidAmazon = !packet.HasSimpleCheckoutAndroidAmazon || packet.SimpleCheckoutAndroidAmazon;
    netCacheObject.Store.SimpleCheckoutAndroidGoogle = !packet.HasSimpleCheckoutAndroidGoogle || packet.SimpleCheckoutAndroidGoogle;
    netCacheObject.Store.SimpleCheckoutAndroidGlobal = !packet.HasSimpleCheckoutAndroidGlobal || packet.SimpleCheckoutAndroidGlobal;
    netCacheObject.Store.SimpleCheckoutWin = !packet.HasSimpleCheckoutWin || packet.SimpleCheckoutWin;
    netCacheObject.Store.SimpleCheckoutMac = !packet.HasSimpleCheckoutMac || packet.SimpleCheckoutMac;
    netCacheObject.Store.BoosterRotatingSoonWarnDaysWithoutSale = packet.HasBoosterRotatingSoonWarnDaysWithoutSale ? packet.BoosterRotatingSoonWarnDaysWithoutSale : 0;
    netCacheObject.Store.BoosterRotatingSoonWarnDaysWithSale = packet.HasBoosterRotatingSoonWarnDaysWithSale ? packet.BoosterRotatingSoonWarnDaysWithSale : 0;
    netCacheObject.Store.VintageStore = !packet.HasVintageStoreEnabled || packet.VintageStoreEnabled;
    netCacheObject.Store.BuyCardBacksFromCollectionManager = !packet.HasBuyCardBacksFromCollectionManagerEnabled || packet.BuyCardBacksFromCollectionManagerEnabled;
    netCacheObject.Store.BuyHeroSkinsFromCollectionManager = !packet.HasBuyHeroSkinsFromCollectionManagerEnabled || packet.BuyHeroSkinsFromCollectionManagerEnabled;
    netCacheObject.Store.LargeItemBundleDetailsEnabled = !packet.HasLargeItemBundleDetailsEnabled || packet.LargeItemBundleDetailsEnabled;
    netCacheObject.Heroes.Hunter = !packet.HasHunter || packet.Hunter;
    netCacheObject.Heroes.Mage = !packet.HasMage || packet.Mage;
    netCacheObject.Heroes.Paladin = !packet.HasPaladin || packet.Paladin;
    netCacheObject.Heroes.Priest = !packet.HasPriest || packet.Priest;
    netCacheObject.Heroes.Rogue = !packet.HasRogue || packet.Rogue;
    netCacheObject.Heroes.Shaman = !packet.HasShaman || packet.Shaman;
    netCacheObject.Heroes.Warlock = !packet.HasWarlock || packet.Warlock;
    netCacheObject.Heroes.Warrior = !packet.HasWarrior || packet.Warrior;
    netCacheObject.Misc.ClientOptionsUpdateIntervalSeconds = packet.HasClientOptionsUpdateIntervalSeconds ? packet.ClientOptionsUpdateIntervalSeconds : 0;
    netCacheObject.Misc.AllowLiveFPSGathering = packet.HasAllowLiveFpsGathering && packet.AllowLiveFpsGathering;
    netCacheObject.CaisEnabledNonMobile = !packet.HasCaisEnabledNonMobile || packet.CaisEnabledNonMobile;
    netCacheObject.CaisEnabledMobileChina = packet.HasCaisEnabledMobileChina && packet.CaisEnabledMobileChina;
    netCacheObject.CaisEnabledMobileSouthKorea = packet.HasCaisEnabledMobileSouthKorea && packet.CaisEnabledMobileSouthKorea;
    netCacheObject.SendTelemetryPresence = packet.HasSendTelemetryPresence && packet.SendTelemetryPresence;
    netCacheObject.XPSoloLimit = packet.XpSoloLimit;
    netCacheObject.MaxHeroLevel = packet.MaxHeroLevel;
    netCacheObject.SpecialEventTimingMod = packet.EventTimingMod;
    netCacheObject.FriendWeekConcederMaxDefense = packet.FriendWeekConcederMaxDefense;
    netCacheObject.FriendWeekConcededGameMinTotalTurns = packet.FriendWeekConcededGameMinTotalTurns;
    netCacheObject.FriendWeekAllowsTavernBrawlRecordUpdate = packet.FriendWeekAllowsTavernBrawlRecordUpdate;
    netCacheObject.FSGEnabled = packet.HasFsgEnabled && packet.FsgEnabled;
    netCacheObject.FSGLoginScanEnabled = packet.HasFsgLoginScanEnabled && packet.FsgLoginScanEnabled;
    netCacheObject.FSGAutoCheckinEnabled = packet.HasFsgAutoCheckinEnabled && packet.FsgAutoCheckinEnabled;
    netCacheObject.FSGShowBetaLabel = packet.HasFsgShowBetaLabel && packet.FsgShowBetaLabel;
    netCacheObject.FSGFriendListPatronCountLimit = packet.HasFsgFriendListPatronCountLimit ? packet.FsgFriendListPatronCountLimit : -1;
    netCacheObject.ArenaClosedToNewSessionsSeconds = packet.HasArenaClosedToNewSessionsSeconds ? packet.ArenaClosedToNewSessionsSeconds : 0U;
    netCacheObject.PVPDRClosedToNewSessionsSeconds = packet.HasPvpdrClosedToNewSessionsSeconds ? packet.PvpdrClosedToNewSessionsSeconds : 0U;
    netCacheObject.FsgMaxPresencePubscribedPatronCount = packet.HasFsgMaxPresencePubscribedPatronCount ? packet.FsgMaxPresencePubscribedPatronCount : -1;
    netCacheObject.QuickOpenEnabled = packet.HasQuickOpenEnabled && packet.QuickOpenEnabled;
    netCacheObject.ForceIosLowRes = packet.HasAllowIosHighres && !packet.AllowIosHighres;
    netCacheObject.AllowOfflineClientActivity = packet.HasAllowOfflineClientActivityDesktop && packet.AllowOfflineClientActivityDesktop;
    netCacheObject.EnableSmartDeckCompletion = packet.HasEnableSmartDeckCompletion && packet.EnableSmartDeckCompletion;
    netCacheObject.AllowOfflineClientDeckDeletion = packet.HasAllowOfflineClientDeckDeletion && packet.AllowOfflineClientDeckDeletion;
    netCacheObject.BattlegroundsEarlyAccessLicense = packet.HasBattlegroundsEarlyAccessLicense ? packet.BattlegroundsEarlyAccessLicense : 0;
    netCacheObject.BattlegroundsMaxRankedPartySize = packet.HasBattlegroundsMaxRankedPartySize ? packet.BattlegroundsMaxRankedPartySize : PartyManager.BATTLEGROUNDS_MAX_RANKED_PARTY_SIZE_FALLBACK;
    netCacheObject.JournalButtonDisabled = packet.JournalButtonDisabled;
    netCacheObject.AchievementToastDisabled = packet.AchievementToastDisabled;
    netCacheObject.DuelsEarlyAccessLicense = packet.HasDuelsEarlyAccessLicense ? packet.DuelsEarlyAccessLicense : 0U;
    netCacheObject.ContentstackEnabled = !packet.HasContentstackEnabled || packet.ContentstackEnabled;
    netCacheObject.PersonalizedMessagesEnabled = !packet.HasPersonalizeMessagesEnabled || packet.PersonalizeMessagesEnabled;
    netCacheObject.AppRatingEnabled = !packet.HasAppRatingEnabled || packet.AppRatingEnabled;
    netCacheObject.AppRatingSamplingPercentage = packet.AppRatingSamplingPercentage;
    netCacheObject.DuelsCardDenylist = packet.DuelsCardDenylist;
    netCacheObject.ConstructedCardDenylist = packet.ConstructedCardDenylist;
    netCacheObject.BattlegroundsSkinsEnabled = packet.BattlegroundsSkinsEnabled;
    netCacheObject.BattlegroundsBoardSkinsEnabled = packet.BattlegroundsBoardSkinsEnabled;
    netCacheObject.BattlegroundsFinishersEnabled = packet.BattlegroundsFinishersEnabled;
    netCacheObject.BattlegroundsEmotesEnabled = packet.BattlegroundsEmotesEnabled;
    netCacheObject.BattlegroundsRewardTrackEnabled = packet.BattlegroundsRewardTrackEnabled;
    switch (PlatformSettings.OS)
    {
      case OSCategory.PC:
      case OSCategory.Mac:
        netCacheObject.TutorialPreviewVideosEnabled = packet.HasTutorialPreviewVideosEnabledDesktop && packet.TutorialPreviewVideosEnabledDesktop;
        break;
      case OSCategory.iOS:
        netCacheObject.TutorialPreviewVideosEnabled = packet.HasTutorialPreviewVideosEnabledIos && packet.TutorialPreviewVideosEnabledIos;
        break;
      case OSCategory.Android:
        netCacheObject.TutorialPreviewVideosEnabled = packet.HasTutorialPreviewVideosEnabledAndroid && packet.TutorialPreviewVideosEnabledAndroid;
        break;
    }
    netCacheObject.TutorialPreviewVideosTimeout = packet.HasTutorialPreviewVideosTimeout ? packet.TutorialPreviewVideosTimeout : NetCache.NetCacheFeatures.Defaults.TutorialPreviewVideosTimeout;
    netCacheObject.SkippableTutorialEnabled = packet.HasSkippableTutorialEnabled && packet.SkippableTutorialEnabled;
    netCacheObject.MinHPForProgressAfterConcede = packet.HasMinHpForProgressAfterConcede ? packet.MinHpForProgressAfterConcede : 0;
    netCacheObject.MinTurnsForProgressAfterConcede = packet.HasMinTurnsForProgressAfterConcede ? packet.MinTurnsForProgressAfterConcede : 0;
    netCacheObject.EnablePlayingFromMiniHand = packet.HasEnablePlayFromMiniHand && packet.EnablePlayFromMiniHand;
    netCacheObject.EnableUpgradeToGolden = packet.HasUpgradeToGoldenEnabled && packet.UpgradeToGoldenEnabled;
    netCacheObject.ShouldPrevalidatePastedDeckCodes = packet.HasPrevalidatePastedDeckCodesOnClient && packet.PrevalidatePastedDeckCodesOnClient;
    netCacheObject.LegacyCardValueCacheEnabled = packet.HasLegacyCachedCardValuesEnabled && packet.LegacyCachedCardValuesEnabled;
    netCacheObject.OvercappedDecksEnabled = packet.HasOvercappedDecksEnabled && packet.OvercappedDecksEnabled;
    netCacheObject.ReportPlayerEnabled = packet.HasReportPlayerEnabled && packet.ReportPlayerEnabled;
    netCacheObject.LuckyDrawEnabled = packet.HasLuckyDrawEnabled && packet.LuckyDrawEnabled;
    netCacheObject.BattlenetBillingFlowDisableOverride = packet.HasBattlenetBillingFlowDisableOverride && packet.BattlenetBillingFlowDisableOverride;
    netCacheObject.BattlegroundsLuckyDrawDisabledCountryCode = packet.HasBattlegroundsLuckyDrawDisabledCountryCode ? packet.BattlegroundsLuckyDrawDisabledCountryCode : "";
    netCacheObject.ContinuousQuickOpenEnabled = packet.ContinuousQuickOpenEnabled && packet.ContinuousQuickOpenEnabled;
    netCacheObject.MercenariesEnableVillages = packet.HasMercenariesEnableVillage && packet.MercenariesEnableVillage;
    netCacheObject.MercenariesPackOpeningEnabled = packet.HasMercenariesPackOpeningEnabled && packet.MercenariesPackOpeningEnabled;
    netCacheObject.Mercenaries.FullyUpgradedStatBoostAttack = packet.HasMercenariesFullyUpgradedStatBoostAttack ? packet.MercenariesFullyUpgradedStatBoostAttack : 0;
    netCacheObject.Mercenaries.FullyUpgradedStatBoostHealth = packet.HasMercenariesFullyUpgradedStatBoostHealth ? packet.MercenariesFullyUpgradedStatBoostHealth : 0;
    netCacheObject.MercenariesTeamMaxSize = packet.HasMercenariesMaxTeamSize ? packet.MercenariesMaxTeamSize : 6;
    netCacheObject.TracerouteEnabled = !packet.HasTracerouteEnabled || packet.TracerouteEnabled;
    netCacheObject.Traceroute.MaxHops = packet.HasTracerouteMaxHops ? packet.TracerouteMaxHops : 30;
    netCacheObject.Traceroute.MessageSize = packet.HasTracerouteMessageSize ? packet.TracerouteMessageSize : 32;
    netCacheObject.Traceroute.MaxRetries = packet.HasTracerouteMaxRetries ? packet.TracerouteMaxRetries : 3;
    netCacheObject.Traceroute.TimeoutMs = packet.HasTracerouteTimeoutMs ? packet.TracerouteTimeoutMs : 3000;
    netCacheObject.Traceroute.ResolveHost = packet.HasTracerouteResolveHost && packet.TracerouteResolveHost;
    netCacheObject.BattlegroundsMedalFriendListDisplayEnabled = packet.HasBattlegroundsMedalFriendListDisplayEnabled && packet.BattlegroundsMedalFriendListDisplayEnabled;
    netCacheObject.RecentFriendListDisplayEnabled = packet.HasRecentFriendListDisplayEnabled && packet.RecentFriendListDisplayEnabled;
    if (packet.HasFsgEnabled && packet.FsgEnabled)
      Network.Get().EnsureSubscribedTo(UtilSystemId.FIRESIDE_GATHERINGS);
    this.OnNetCacheObjReceived<NetCache.NetCacheFeatures>(netCacheObject);
  }

  public void OnCurrencyState(GameCurrencyStates currencyState)
  {
    if (!currencyState.HasCurrencyVersion || this.m_currencyVersion > currencyState.CurrencyVersion)
    {
      Log.Net.PrintDebug("Ignoring currency state: {0}, (cached currency version: {1})", (object) currencyState.ToHumanReadableString(), (object) this.m_currencyVersion);
    }
    else
    {
      Log.Net.PrintDebug("Caching currency state: {0}", (object) currencyState.ToHumanReadableString());
      this.m_currencyVersion = currencyState.CurrencyVersion;
      if (currencyState.HasArcaneDustBalance)
      {
        NetCache.NetCacheArcaneDustBalance netCacheObject = this.GetNetObject<NetCache.NetCacheArcaneDustBalance>() ?? new NetCache.NetCacheArcaneDustBalance();
        netCacheObject.Balance = currencyState.ArcaneDustBalance;
        this.OnNetCacheObjReceived<NetCache.NetCacheArcaneDustBalance>(netCacheObject);
      }
      if (currencyState.HasCappedGoldBalance && currencyState.HasBonusGoldBalance)
      {
        NetCache.NetCacheGoldBalance cacheGoldBalance = this.GetNetObject<NetCache.NetCacheGoldBalance>() ?? new NetCache.NetCacheGoldBalance();
        cacheGoldBalance.CappedBalance = currencyState.CappedGoldBalance;
        cacheGoldBalance.BonusBalance = currencyState.BonusGoldBalance;
        this.OnNetCacheObjReceived<NetCache.NetCacheGoldBalance>(cacheGoldBalance);
        foreach (NetCache.DelGoldBalanceListener goldBalanceListener in this.m_goldBalanceListeners.ToArray())
          goldBalanceListener(cacheGoldBalance);
      }
      if (currencyState.HasRenownBalance)
      {
        NetCache.NetCacheRenownBalance netCacheObject = this.GetNetObject<NetCache.NetCacheRenownBalance>() ?? new NetCache.NetCacheRenownBalance();
        netCacheObject.Balance = currencyState.RenownBalance;
        this.OnNetCacheObjReceived<NetCache.NetCacheRenownBalance>(netCacheObject);
      }
      Shop shop = Shop.Get();
      if (!((UnityEngine.Object) shop != (UnityEngine.Object) null))
        return;
      shop.RefreshWallet();
    }
  }

  public void OnBoosterModifications(BoosterModifications packet)
  {
    NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
    if (netObject == null)
      return;
    foreach (BoosterInfo modification in packet.Modifications)
    {
      NetCache.BoosterStack boosterStack = netObject.GetBoosterStack(modification.Type);
      if (boosterStack == null)
      {
        boosterStack = new NetCache.BoosterStack()
        {
          Id = modification.Type
        };
        netObject.BoosterStacks.Add(boosterStack);
      }
      if (modification.Count > 0)
        boosterStack.EverGrantedCount += modification.EverGrantedCount;
      boosterStack.Count += modification.Count;
    }
    this.OnNetCacheObjReceived<NetCache.NetCacheBoosters>(netObject);
  }

  public bool AddExpectedCollectionModification(long version)
  {
    if (this.m_handledCardModifications.Contains(version))
      return false;
    this.m_expectedCardModifications.Add(version);
    return true;
  }

  public void OnCollectionModification(ClientStateNotification packet)
  {
    CollectionModifications collectionModifications = packet.CollectionModifications;
    if (this.m_handledCardModifications.Contains(collectionModifications.CollectionVersion) || this.m_initialCollectionVersion >= collectionModifications.CollectionVersion)
    {
      Log.Net.PrintDebug("Ignoring redundant coolection modification (modification was v.{0}; we are v.{1})", (object) collectionModifications.CollectionVersion, (object) Math.Max(this.m_handledCardModifications.DefaultIfEmpty<long>(0L).Max(), this.m_initialCollectionVersion));
    }
    else
    {
      this.OnCollectionModificationInternal(collectionModifications);
      if (packet.HasAchievementNotifications)
        AchieveManager.Get().OnAchievementNotifications(packet.AchievementNotifications.AchievementNotifications_);
      if (packet.HasNoticeNotifications)
        Network.Get().OnNoticeNotifications(packet.NoticeNotifications);
      if (packet.HasBoosterModifications)
        this.OnBoosterModifications(packet.BoosterModifications);
      if (collectionModifications.CardModifications.Count <= 0 || !((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() != (UnityEngine.Object) null) || !((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay().GetPageManager() != (UnityEngine.Object) null))
        return;
      CollectionManager.Get().GetCollectibleDisplay().GetPageManager().RefreshCurrentPageContents();
      CollectionManager.Get().GetCollectibleDisplay().UpdateCurrentPageCardLocks();
    }
  }

  private void OnCollectionModificationInternal(CollectionModifications packet)
  {
    this.m_handledCardModifications.Add(packet.CollectionVersion);
    this.m_expectedCardModifications.Remove(packet.CollectionVersion);
    foreach (CardModification cardModification in packet.CardModifications)
    {
      Log.Net.PrintDebug("Handling card collection modification (collection version {0}): {1}", (object) packet.CollectionVersion, (object) cardModification.ToHumanReadableString());
      string cardId = GameUtils.TranslateDbIdToCardId(cardModification.AssetCardId);
      if (cardModification.Quantity > 0)
      {
        int num = 0;
        int count1 = Math.Min(cardModification.AmountSeen, cardModification.Quantity);
        if (cardModification.AmountSeen > 0)
        {
          CollectionManager.Get().OnCardAdded(cardId, (TAG_PREMIUM) cardModification.Premium, count1, true);
          num = count1;
        }
        int count2 = cardModification.Quantity - num;
        if (count2 > 0)
          CollectionManager.Get().OnCardAdded(cardId, (TAG_PREMIUM) cardModification.Premium, count2, false);
      }
      else if (cardModification.Quantity < 0)
        CollectionManager.Get().OnCardRemoved(cardId, (TAG_PREMIUM) cardModification.Premium, -1 * cardModification.Quantity);
    }
    AchieveManager.Get().ValidateAchievesNow();
  }

  public void OnCardBackModifications(CardBackModifications packet)
  {
    NetCache.NetCacheCardBacks netObject = this.GetNetObject<NetCache.NetCacheCardBacks>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.OnCardBackModifications(): trying to access NetCacheCardBacks before it's been loaded"));
    }
    else
    {
      foreach (CardBackModification backModification in packet.CardBackModifications_)
      {
        netObject.CardBacks.Add(backModification.AssetCardBackId);
        if (backModification.HasAutoSetAsFavorite && backModification.AutoSetAsFavorite)
          this.ProcessNewFavoriteCardBack(backModification.AssetCardBackId);
      }
    }
  }

  public void OnBattlegroundsGuideSkinModifications(BattlegroundsGuideSkinModifications packet)
  {
    NetCache.NetCacheBattlegroundsGuideSkins netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.OnBattlegroundsGuideSkinModifications(): trying to access NetCacheBattlegroundsGuideSkins before it has been loaded."));
    }
    else
    {
      bool flag = false;
      foreach (BattlegroundsGuideSkinModification skinModification in packet.BattlegroundsGuideSkinModifications_)
      {
        if (!skinModification.HasBattlegroundsGuideSkinId)
        {
          Debug.LogWarning((object) "NetCache.OnBattlegroundsGuideSkinModifications(): received BattlegroundsGuideSkinModification message has no BattlegroundsGuideSkinId.");
        }
        else
        {
          BattlegroundsGuideSkinId? nullable = BattlegroundsGuideSkinId.FromUntrustedValue(skinModification.BattlegroundsGuideSkinId);
          if (!nullable.HasValue)
            Debug.LogWarning((object) "NetCache.OnBattlegroundsGuideSkinModifications(): received BattlegroundsGuideSkinModification message has invalid BattlegroundsGuideSkinId.");
          else if (skinModification.HasAddBattlegroundsGuideSkin && skinModification.AddBattlegroundsGuideSkin)
          {
            netObject.OwnedBattlegroundsGuideSkins.Add(nullable.Value);
            if (skinModification.HasAutoSetAsFavorite && skinModification.AutoSetAsFavorite)
              this.ProcessNewFavoriteBattlegroundsGuideSkin(nullable.Value);
            netObject.UnseenSkinIds.Add(nullable.Value);
            flag = true;
          }
          else if (skinModification.HasRemoveBattlegroundsGuideSkin && skinModification.RemoveBattlegroundsGuideSkin)
          {
            netObject.OwnedBattlegroundsGuideSkins.Remove(nullable.Value);
            netObject.UnseenSkinIds.Remove(nullable.Value);
            flag = true;
          }
        }
      }
      if (!flag || this.OwnedBattlegroundsSkinsChanged == null)
        return;
      this.OwnedBattlegroundsSkinsChanged();
    }
  }

  public void OnBattlegroundsHeroSkinModifications(BattlegroundsHeroSkinModifications packet)
  {
    NetCache.NetCacheBattlegroundsHeroSkins netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.OnBattlegroundsHeroSkinModifications(): trying to access NetCacheBattlegroundsHeroSkins before it has been loaded."));
    }
    else
    {
      bool flag = false;
      foreach (BattlegroundsHeroSkinModification skinModification in packet.BattlegroundsHeroSkinModifications_)
      {
        if (!skinModification.HasBattlegroundsHeroSkinId)
        {
          Debug.LogWarning((object) "NetCache.OnBattlegroundsHeroSkinModifications(): received BattlegroundsHeroSkinModification message has no HasBattlegroundsHeroSkinId.");
        }
        else
        {
          BattlegroundsHeroSkinId? nullable = BattlegroundsHeroSkinId.FromUntrustedValue(skinModification.BattlegroundsHeroSkinId);
          if (!nullable.HasValue)
            Debug.LogWarning((object) "NetCache.OnBattlegroundsHeroSkinModifications(): received BattlegroundsHeroSkinModification message has invalid HasBattlegroundsHeroSkinId.");
          else if (skinModification.HasAddBattlegroundsHeroSkin && skinModification.AddBattlegroundsHeroSkin)
          {
            netObject.OwnedBattlegroundsSkins.Add(nullable.Value);
            if (skinModification.HasAutoSetAsFavorite && skinModification.AutoSetAsFavorite)
              this.ProcessNewFavoriteBattlegroundsHeroSkin(nullable.Value);
            netObject.UnseenSkinIds.Add(nullable.Value);
            flag = true;
          }
          else if (skinModification.HasRemoveBattlegroundsHeroSkin && skinModification.RemoveBattlegroundsHeroSkin)
          {
            netObject.OwnedBattlegroundsSkins.Remove(nullable.Value);
            netObject.UnseenSkinIds.Remove(nullable.Value);
            flag = true;
          }
        }
      }
      if (!flag || this.OwnedBattlegroundsSkinsChanged == null)
        return;
      this.OwnedBattlegroundsSkinsChanged();
    }
  }

  public void OnBattlegroundsBoardSkinModifications(BattlegroundsBoardSkinModifications packet)
  {
    NetCache.NetCacheBattlegroundsBoardSkins netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.OnBattlegroundsBoardSkinModifications(): trying to access NetCacheBattlegroundsBoardSkins before it has been loaded."));
    }
    else
    {
      bool flag = false;
      foreach (BattlegroundsBoardSkinModification skinModification in packet.BattlegroundsBoardSkinModifications_)
      {
        if (!skinModification.HasBattlegroundsBoardSkinId)
        {
          Debug.LogWarning((object) "NetCache.OnBattlegroundsBoardSkinModifications(): received BattlegroundsBoardSkinModification message has no HasBattlegroundsBoardSkinId.");
        }
        else
        {
          BattlegroundsBoardSkinId? nullable = BattlegroundsBoardSkinId.FromUntrustedValue(skinModification.BattlegroundsBoardSkinId);
          if (!nullable.HasValue || nullable.Value.IsDefaultBoard())
            Debug.LogWarning((object) "NetCache.OnBattlegroundsBoardSkinModifications(): received BattlegroundsBoardSkinModification message has invalid HasBattlegroundsBoardSkinId.");
          else if (skinModification.HasAddBattlegroundsBoardSkin && skinModification.AddBattlegroundsBoardSkin)
          {
            netObject.OwnedBattlegroundsBoardSkins.Add(nullable.Value);
            if (skinModification.HasAutoSetAsFavorite && skinModification.AutoSetAsFavorite)
              this.ProcessNewFavoriteBattlegroundsBoardSkin(nullable.Value);
            netObject.UnseenSkinIds.Add(nullable.Value);
            flag = true;
          }
          else if (skinModification.HasRemoveBattlegroundsBoardSkin && skinModification.RemoveBattlegroundsBoardSkin)
          {
            netObject.OwnedBattlegroundsBoardSkins.Remove(nullable.Value);
            netObject.UnseenSkinIds.Remove(nullable.Value);
            flag = true;
          }
        }
      }
      if (!flag || this.OwnedBattlegroundsSkinsChanged == null)
        return;
      this.OwnedBattlegroundsSkinsChanged();
    }
  }

  private void OnSetBattlegroundsEmoteLoadoutResponse()
  {
    SetBattlegroundsEmoteLoadoutResponse emoteLoadoutResponse = Network.Get().GetSetBattlegroundsEmoteLoadoutResponse();
    if (!emoteLoadoutResponse.Success)
      return;
    NetCache.NetCacheBattlegroundsEmotes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
    if (netObject == null)
      return;
    Hearthstone.BattlegroundsEmoteLoadout newLoadout = Hearthstone.BattlegroundsEmoteLoadout.MakeFromNetwork(emoteLoadoutResponse.Loadout);
    if (!(newLoadout != (Hearthstone.BattlegroundsEmoteLoadout) null) || !(newLoadout != netObject.CurrentLoadout))
      return;
    netObject.CurrentLoadout = newLoadout;
    // ISSUE: reference to a compiler-generated field
    if (this.BattlegroundsEmoteLoadoutChangedListener == null)
      return;
    // ISSUE: reference to a compiler-generated field
    this.BattlegroundsEmoteLoadoutChangedListener(newLoadout);
  }

  public void OnBattlegroundsFinisherModifications(BattlegroundsFinisherModifications packet)
  {
    NetCache.NetCacheBattlegroundsFinishers netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.OnBattlegroundsFinisherModifications(): trying to access NetCacheBattlegroundsFinishers before it has been loaded."));
    }
    else
    {
      bool flag = false;
      foreach (BattlegroundsFinisherModification finisherModification in packet.BattlegroundsFinisherModifications_)
      {
        if (!finisherModification.HasBattlegroundsFinisherId)
        {
          Debug.LogWarning((object) "NetCache.OnBattlegroundsFinisherModifications(): received BattlegroundsFinisherModification message has no HasBattlegroundsFinisherId.");
        }
        else
        {
          BattlegroundsFinisherId? nullable = BattlegroundsFinisherId.FromUntrustedValue(finisherModification.BattlegroundsFinisherId);
          if (!nullable.HasValue || nullable.Value.IsDefaultFinisher())
            Debug.LogWarning((object) "NetCache.OnBattlegroundsFinisherModifications(): received BattlegroundsFinisherModification message has invalid HasBattlegroundsFinisherId.");
          else if (finisherModification.HasAddBattlegroundsFinisher && finisherModification.AddBattlegroundsFinisher)
          {
            netObject.OwnedBattlegroundsFinishers.Add(nullable.Value);
            if (finisherModification.HasAutoSetAsFavorite && finisherModification.AutoSetAsFavorite)
              this.ProcessNewFavoriteBattlegroundsFinisher(nullable.Value);
            netObject.UnseenSkinIds.Add(nullable.Value);
            flag = true;
          }
          else if (finisherModification.HasRemoveBattlegroundsFinisher && finisherModification.RemoveBattlegroundsFinisher)
          {
            netObject.OwnedBattlegroundsFinishers.Remove(nullable.Value);
            netObject.UnseenSkinIds.Remove(nullable.Value);
            flag = true;
          }
        }
      }
      if (!flag || this.OwnedBattlegroundsSkinsChanged == null)
        return;
      this.OwnedBattlegroundsSkinsChanged();
    }
  }

  private void OnBattlegroundsEmotesResponse()
  {
    BattlegroundsEmotesResponse battlegroundsEmotesResponse = Network.Get().GetBattlegroundsEmotesResponse();
    NetCache.NetCacheBattlegroundsEmotes netCacheObject = new NetCache.NetCacheBattlegroundsEmotes();
    foreach (BattlegroundsEmoteInfo ownedEmote in battlegroundsEmotesResponse.OwnedEmotes)
    {
      BattlegroundsEmoteId? nullable = BattlegroundsEmoteId.FromUntrustedValue(ownedEmote.EmoteId);
      if (!nullable.HasValue)
        Log.Net.PrintError("OnBattlegroundsEmotesResponse FAILED (packetOwnedEmote = {0} due to negative ID)", (object) ownedEmote);
      else if (nullable.Value.IsDefaultEmote())
        Log.Net.PrintError("OnBattlegroundsEmotesResponse FAILED (packetOwnedEmote = {0} due to default)", (object) ownedEmote);
      else if (!CollectionManager.Get().IsValidBattlegroundsEmoteId(nullable.Value))
      {
        Log.Net.PrintError("OnBattlegroundsEmotesResponse FAILED (packetOwnedEmote = {0} due to not present in Hearthedit)", (object) ownedEmote);
      }
      else
      {
        netCacheObject.OwnedBattlegroundsEmotes.Add(nullable.Value);
        if (!ownedEmote.HasSeen)
          netCacheObject.UnseenEmoteIds.Add(nullable.Value);
      }
    }
    Hearthstone.BattlegroundsEmoteLoadout battlegroundsEmoteLoadout = Hearthstone.BattlegroundsEmoteLoadout.MakeFromNetwork(battlegroundsEmotesResponse.Loadout);
    if (battlegroundsEmoteLoadout == (Hearthstone.BattlegroundsEmoteLoadout) null)
      Log.Net.PrintError("OnBattlegroundsEmotesResponse FAILED due to invalid loadout.");
    else
      netCacheObject.CurrentLoadout = battlegroundsEmoteLoadout;
    this.OnNetCacheObjReceived<NetCache.NetCacheBattlegroundsEmotes>(netCacheObject);
  }

  public void OnBattlegroundsEmoteModifications(BattlegroundsEmoteModifications packet)
  {
    NetCache.NetCacheBattlegroundsEmotes netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.OnBattlegroundsEmoteModifications(): trying to access NetCacheBattlegroundsEmotes before it has been loaded."));
    }
    else
    {
      bool flag = false;
      foreach (BattlegroundsEmoteModification emoteModification in packet.BattlegroundsEmoteModifications_)
      {
        if (!emoteModification.HasBattlegroundsEmoteId)
        {
          Debug.LogWarning((object) "NetCache.OnBattlegroundsEmoteModifications(): received BattlegroundsEmoteModification message has no HasBattlegroundsEmoteId.");
        }
        else
        {
          BattlegroundsEmoteId? nullable = BattlegroundsEmoteId.FromUntrustedValue(emoteModification.BattlegroundsEmoteId);
          if (!nullable.HasValue)
            Debug.LogWarning((object) "NetCache.OnBattlegroundsEmoteModifications(): received BattlegroundsEmoteModification message has invalid HasBattlegroundsEmoteId.");
          else if (emoteModification.HasRemoveBattlegroundsEmote && emoteModification.AddBattlegroundsEmote)
          {
            netObject.OwnedBattlegroundsEmotes.Add(nullable.Value);
            netObject.UnseenEmoteIds.Add(nullable.Value);
            flag = true;
          }
          else if (emoteModification.HasRemoveBattlegroundsEmote && emoteModification.RemoveBattlegroundsEmote)
          {
            netObject.OwnedBattlegroundsEmotes.Remove(nullable.Value);
            netObject.UnseenEmoteIds.Remove(nullable.Value);
            flag = true;
          }
        }
      }
      if (!flag || this.OwnedBattlegroundsSkinsChanged == null)
        return;
      this.OwnedBattlegroundsSkinsChanged();
    }
  }

  private void OnSetFavoriteCardBackResponse()
  {
    Network.CardBackResponse cardBackResponse = Network.Get().GetCardBackResponse();
    if (!cardBackResponse.Success)
      Log.CardbackMgr.PrintError("SetFavoriteCardBack FAILED (cardBack = {0})", (object) cardBackResponse.CardBack);
    else
      this.ProcessNewFavoriteCardBack(cardBackResponse.CardBack, cardBackResponse.IsFavorite);
  }

  public void ProcessNewFavoriteCardBack(int cardBackId, bool isFavorite = true)
  {
    NetCache.NetCacheCardBacks netObject = this.GetNetObject<NetCache.NetCacheCardBacks>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.ProcessNewFavoriteCardBack(): trying to access NetCacheCardBacks before it's been loaded"));
    }
    else
    {
      if (isFavorite)
        netObject.FavoriteCardBacks.Add(cardBackId);
      else
        netObject.FavoriteCardBacks.Remove(cardBackId);
      if (this.FavoriteCardBackChanged == null)
        return;
      this.FavoriteCardBackChanged(cardBackId, isFavorite);
    }
  }

  public void ProcessNewFavoriteBattlegroundsGuideSkin(
    BattlegroundsGuideSkinId newFavoriteBattlegroundsGuideSkinID)
  {
    NetCache.NetCacheBattlegroundsGuideSkins netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.ProcessNewFavoriteBattlegroundsGuideSkin(): trying to access NetCacheBattlegroundsGuideSkins before it has been loaded."));
    }
    else
    {
      BattlegroundsGuideSkinId? favoriteGuideSkin = netObject.BattlegroundsFavoriteGuideSkin;
      BattlegroundsGuideSkinId battlegroundsGuideSkinId = newFavoriteBattlegroundsGuideSkinID;
      if ((favoriteGuideSkin.HasValue ? (favoriteGuideSkin.HasValue ? (favoriteGuideSkin.GetValueOrDefault() == battlegroundsGuideSkinId ? 1 : 0) : 1) : 0) != 0)
        return;
      netObject.BattlegroundsFavoriteGuideSkin = new BattlegroundsGuideSkinId?(newFavoriteBattlegroundsGuideSkinID);
      if (this.FavoriteBattlegroundsGuideSkinChanged == null)
        return;
      this.FavoriteBattlegroundsGuideSkinChanged(new BattlegroundsGuideSkinId?(newFavoriteBattlegroundsGuideSkinID));
    }
  }

  public void ProcessNewFavoriteBattlegroundsHeroSkin(
    BattlegroundsHeroSkinId newFavoriteBattlegroundsHeroSkinID)
  {
    NetCache.NetCacheBattlegroundsHeroSkins netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
    int baseHeroCardId;
    CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(newFavoriteBattlegroundsHeroSkinID, out baseHeroCardId);
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.ProcessNewFavoriteBattlegroundsHeroSkin(): trying to access NetCacheBattlegroundsHeroSkins before it has been loaded."));
    }
    else
    {
      if (netObject.BattlegroundsFavoriteHeroSkins[baseHeroCardId] == newFavoriteBattlegroundsHeroSkinID)
        return;
      netObject.BattlegroundsFavoriteHeroSkins[baseHeroCardId] = newFavoriteBattlegroundsHeroSkinID;
      if (this.FavoriteBattlegroundsHeroSkinChanged == null)
        return;
      this.FavoriteBattlegroundsHeroSkinChanged(baseHeroCardId, new BattlegroundsHeroSkinId?(newFavoriteBattlegroundsHeroSkinID));
    }
  }

  public void ProcessNewFavoriteBattlegroundsBoardSkin(
    BattlegroundsBoardSkinId newFavoriteBattlegroundsBoardSkinID)
  {
    NetCache.NetCacheBattlegroundsBoardSkins netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.ProcessNewFavoriteBattlegroundsBoardSkin(): trying to access NetCacheBattlegroundsBoardSkins before it has been loaded."));
    }
    else
    {
      BattlegroundsBoardSkinId? favoriteBoardSkin = netObject.BattlegroundsFavoriteBoardSkin;
      BattlegroundsBoardSkinId battlegroundsBoardSkinId = newFavoriteBattlegroundsBoardSkinID;
      if ((favoriteBoardSkin.HasValue ? (favoriteBoardSkin.HasValue ? (favoriteBoardSkin.GetValueOrDefault() == battlegroundsBoardSkinId ? 1 : 0) : 1) : 0) != 0)
        return;
      netObject.BattlegroundsFavoriteBoardSkin = new BattlegroundsBoardSkinId?(newFavoriteBattlegroundsBoardSkinID);
      if (this.FavoriteBattlegroundsBoardSkinChanged == null)
        return;
      this.FavoriteBattlegroundsBoardSkinChanged(new BattlegroundsBoardSkinId?(newFavoriteBattlegroundsBoardSkinID));
    }
  }

  public void ProcessNewFavoriteBattlegroundsFinisher(
    BattlegroundsFinisherId newFavoriteBattlegroundsFinisherID)
  {
    NetCache.NetCacheBattlegroundsFinishers netObject = this.GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.ProcessNewFavoriteBattlegroundsFinisher(): trying to access NetCacheBattlegroundsFinishers before it has been loaded."));
    }
    else
    {
      BattlegroundsFinisherId? favoriteFinisher = netObject.BattlegroundsFavoriteFinisher;
      BattlegroundsFinisherId battlegroundsFinisherId = newFavoriteBattlegroundsFinisherID;
      if ((favoriteFinisher.HasValue ? (favoriteFinisher.HasValue ? (favoriteFinisher.GetValueOrDefault() == battlegroundsFinisherId ? 1 : 0) : 1) : 0) != 0)
        return;
      netObject.BattlegroundsFavoriteFinisher = new BattlegroundsFinisherId?(newFavoriteBattlegroundsFinisherID);
      if (this.FavoriteBattlegroundsFinisherChanged == null)
        return;
      this.FavoriteBattlegroundsFinisherChanged(new BattlegroundsFinisherId?(newFavoriteBattlegroundsFinisherID));
    }
  }

  private void OnSetFavoriteCoinResponse()
  {
    Network.CoinResponse coinResponse = Network.Get().GetCoinResponse();
    if (!coinResponse.Success)
      Log.Net.PrintError("SetFavoriteCardBack FAILED (coin = {0})", (object) coinResponse.Coin);
    else
      this.ProcessNewFavoriteCoin(coinResponse.Coin);
  }

  public void ProcessNewFavoriteCoin(int newFavoriteCoinID)
  {
    NetCache.NetCacheCoins netObject = this.GetNetObject<NetCache.NetCacheCoins>();
    if (netObject == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.ProcessNewFavoriteCoin(): trying to accessNetCacheCoins before it's been loaded"));
    }
    else
    {
      if (netObject.FavoriteCoin == newFavoriteCoinID)
        return;
      netObject.FavoriteCoin = newFavoriteCoinID;
      if (this.FavoriteCoinChanged == null)
        return;
      this.FavoriteCoinChanged(newFavoriteCoinID);
    }
  }

  private void OnGamesInfo()
  {
    NetCache.NetCacheGamesPlayed gamesInfo = Network.Get().GetGamesInfo();
    if (gamesInfo == null)
      Debug.LogWarning((object) "error getting games info");
    else
      this.OnNetCacheObjReceived<NetCache.NetCacheGamesPlayed>(gamesInfo);
  }

  private void OnProfileProgress() => this.OnNetCacheObjReceived<NetCache.NetCacheProfileProgress>(Network.Get().GetProfileProgress());

  private void OnHearthstoneUnavailableGame() => this.OnHearthstoneUnavailable(true);

  private void OnHearthstoneUnavailableUtil() => this.OnHearthstoneUnavailable(false);

  private void OnHearthstoneUnavailable(bool gamePacket)
  {
    Network.UnavailableReason hearthstoneUnavailable = Network.Get().GetHearthstoneUnavailable(gamePacket);
    Debug.Log((object) ("Hearthstone Unavailable!  Reason: " + hearthstoneUnavailable.mainReason));
    string mainReason = hearthstoneUnavailable.mainReason;
    if (!(mainReason == "VERSION"))
    {
      if (mainReason == "OFFLINE")
      {
        Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_OFFLINE");
      }
      else
      {
        TelemetryManager.Client().SendNetworkError(NetworkError.ErrorType.SERVICE_UNAVAILABLE, string.Format("{0} - {1} - {2}", (object) hearthstoneUnavailable.mainReason, (object) hearthstoneUnavailable.subReason, (object) hearthstoneUnavailable.extraData), 0);
        Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UNKNOWN");
      }
    }
    else
    {
      ErrorParams parms = new ErrorParams();
      if (PlatformSettings.IsMobile() && GameDownloadManagerProvider.Get() != null && !GameDownloadManagerProvider.Get().IsNewMobileVersionReleased)
      {
        parms.m_message = GameStrings.Format("GLOBAL_ERROR_NETWORK_UNAVAILABLE_NEW_VERSION");
        parms.m_reason = FatalErrorReason.UNAVAILABLE_NEW_VERSION;
      }
      else
      {
        parms.m_message = GameStrings.Format("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UPGRADE");
        if ((bool) Error.HAS_APP_STORE)
          parms.m_redirectToStore = true;
        parms.m_reason = FatalErrorReason.UNAVAILABLE_UPGRADE;
      }
      Error.AddFatal(parms);
      ReconnectMgr.Get().FullResetRequired = true;
      ReconnectMgr.Get().UpdateRequired = true;
    }
  }

  private void OnCardBacks()
  {
    Network network = Network.Get();
    this.OnNetCacheObjReceived<NetCache.NetCacheCardBacks>(network.GetCardBacks());
    CardBacks cardBacksPacket = network.GetCardBacksPacket();
    if (cardBacksPacket == null)
      return;
    List<int> favoriteCardBacks = cardBacksPacket.FavoriteCardBacks;
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    List<SetFavoriteCardBack> cardBackFromDiff = OfflineDataCache.GenerateSetFavoriteCardBackFromDiff(data, favoriteCardBacks);
    if (cardBackFromDiff != null && cardBackFromDiff.Count > 0)
    {
      foreach (SetFavoriteCardBack favoriteCardBack in cardBackFromDiff)
        network.SetFavoriteCardBack(favoriteCardBack.CardBack, favoriteCardBack.IsFavorite);
    }
    OfflineDataCache.ClearCardBackDirtyFlag(ref data);
    OfflineDataCache.CacheCardBacks(ref data, cardBacksPacket);
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  private void OnCoins()
  {
    Network network = Network.Get();
    this.OnNetCacheObjReceived<NetCache.NetCacheCoins>(network.GetCoins());
    Coins coinsPacket = network.GetCoinsPacket();
    if (coinsPacket == null)
      return;
    int favoriteCoin = coinsPacket.FavoriteCoin;
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    SetFavoriteCoin favoriteCoinFromDiff = OfflineDataCache.GenerateSetFavoriteCoinFromDiff(data, favoriteCoin);
    if (favoriteCoinFromDiff != null)
      network.SetFavoriteCoin(ref data, favoriteCoinFromDiff.Coin);
    OfflineDataCache.ClearCoinDirtyFlag(ref data);
    OfflineDataCache.CacheCoins(ref data, coinsPacket);
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  private void OnBattlegroundsHeroSkinsResponse()
  {
    BattlegroundsHeroSkinsResponse heroSkinsResponse = Network.Get().GetBattlegroundsHeroSkinsResponse();
    NetCache.NetCacheBattlegroundsHeroSkins netCacheObject = new NetCache.NetCacheBattlegroundsHeroSkins();
    foreach (BattlegroundsHeroSkinInfo ownedSkin in heroSkinsResponse.OwnedSkins)
    {
      BattlegroundsHeroSkinId? nullable = BattlegroundsHeroSkinId.FromUntrustedValue(ownedSkin.HeroSkinId);
      if (!nullable.HasValue)
        Log.Net.PrintError("OnBattlegroundsHeroSkinsResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      else if (!CollectionManager.Get().IsValidBattlegroundsHeroSkinId(nullable.Value))
      {
        Log.Net.PrintError("OnBattlegroundsHeroSkinsResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      }
      else
      {
        netCacheObject.OwnedBattlegroundsSkins.Add(nullable.Value);
        if (ownedSkin.IsFavorite)
        {
          int baseHeroCardId = 0;
          if (!CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(nullable.Value, out baseHeroCardId))
          {
            Log.Net.PrintError("OnBattlegroundsHeroSkinsResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
            continue;
          }
          netCacheObject.BattlegroundsFavoriteHeroSkins[baseHeroCardId] = nullable.Value;
        }
        if (!ownedSkin.HasSeen)
          netCacheObject.UnseenSkinIds.Add(nullable.Value);
      }
    }
    this.OnNetCacheObjReceived<NetCache.NetCacheBattlegroundsHeroSkins>(netCacheObject);
  }

  private void OnSetBattlegroundsFavoriteHeroSkinResponse()
  {
    SetBattlegroundsFavoriteHeroSkinResponse heroSkinResponse = Network.Get().GetSetBattlegroundsFavoriteHeroSkinResponse();
    if (!heroSkinResponse.Success)
      return;
    BattlegroundsHeroSkinId? newFavoriteBattlegroundsHeroSkinID = BattlegroundsHeroSkinId.FromUntrustedValue(heroSkinResponse.HeroSkinId);
    if (!newFavoriteBattlegroundsHeroSkinID.HasValue || !CollectionManager.Get().IsValidBattlegroundsHeroSkinId(newFavoriteBattlegroundsHeroSkinID.Value))
      Log.Net.PrintError("OnSetBattlegroundsFavoriteHeroSkinResponse FAILED - invalid skin ID (HeroSkinId = {0})", (object) newFavoriteBattlegroundsHeroSkinID);
    NetCache.NetCacheBattlegroundsHeroSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
    if (netObject == null)
      return;
    int baseHeroCardId = 0;
    if (newFavoriteBattlegroundsHeroSkinID.HasValue && CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(newFavoriteBattlegroundsHeroSkinID.Value, out baseHeroCardId))
    {
      netObject.BattlegroundsFavoriteHeroSkins[baseHeroCardId] = newFavoriteBattlegroundsHeroSkinID.Value;
      if (this.FavoriteBattlegroundsHeroSkinChanged == null)
        return;
      this.FavoriteBattlegroundsHeroSkinChanged(baseHeroCardId, newFavoriteBattlegroundsHeroSkinID);
    }
    else
      Log.Net.PrintError("OnSetBattlegroundsFavoriteHeroSkinResponse FAILED - could not find base ID (HeroSkinId = {0})", (object) newFavoriteBattlegroundsHeroSkinID);
  }

  private void OnClearBattlegroundsFavoriteHeroSkinResponse()
  {
    ClearBattlegroundsFavoriteHeroSkinResponse heroSkinResponse = Network.Get().GetClearBattlegroundsFavoriteHeroSkinResponse();
    if (!heroSkinResponse.Success)
      return;
    BattlegroundsHeroSkinId? nullable = BattlegroundsHeroSkinId.FromUntrustedValue(heroSkinResponse.HeroSkinId);
    if (!nullable.HasValue || !CollectionManager.Get().IsValidBattlegroundsHeroSkinId(nullable.Value))
      Log.Net.PrintError("OnClearBattlegroundsFavoriteHeroSkinResponse FAILED - invalid skin ID (HeroSkinId = {0})", (object) nullable);
    NetCache.NetCacheBattlegroundsHeroSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
    if (netObject == null)
      return;
    int baseHeroCardId = 0;
    if (nullable.HasValue && CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(nullable.Value, out baseHeroCardId))
    {
      netObject.BattlegroundsFavoriteHeroSkins.Remove(baseHeroCardId);
      if (this.FavoriteBattlegroundsHeroSkinChanged == null)
        return;
      this.FavoriteBattlegroundsHeroSkinChanged(baseHeroCardId, new BattlegroundsHeroSkinId?());
    }
    else
      Log.Net.PrintError("OnClearBattlegroundsFavoriteHeroSkinResponse FAILED - could not find base ID (HeroSkinId = {0})", (object) nullable);
  }

  private void OnBattlegroundsGuideSkinsResponse()
  {
    BattlegroundsGuideSkinsResponse guideSkinsResponse = Network.Get().GetBattlegroundsGuideSkinsResponse();
    NetCache.NetCacheBattlegroundsGuideSkins netCacheObject = new NetCache.NetCacheBattlegroundsGuideSkins();
    netCacheObject.BattlegroundsFavoriteGuideSkin = new BattlegroundsGuideSkinId?();
    foreach (BattlegroundsGuideSkinInfo ownedSkin in guideSkinsResponse.OwnedSkins)
    {
      BattlegroundsGuideSkinId? nullable = BattlegroundsGuideSkinId.FromUntrustedValue(ownedSkin.GuideSkinId);
      if (!nullable.HasValue)
        Log.Net.PrintError("OnBattlegroundsGuideSkinsResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      else if (!CollectionManager.Get().IsValidBattlegroundsGuideSkinId(nullable.Value))
      {
        Log.Net.PrintError("OnBattlegroundsGuideSkinsResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      }
      else
      {
        netCacheObject.OwnedBattlegroundsGuideSkins.Add(nullable.Value);
        if (ownedSkin.IsFavorite)
        {
          if (netCacheObject.BattlegroundsFavoriteGuideSkin.HasValue)
            Log.Net.PrintError("OnBattlegroundsGuideSkinsResponse FAILED (multiple favorite skins)");
          else
            netCacheObject.BattlegroundsFavoriteGuideSkin = nullable;
        }
        if (!ownedSkin.HasSeen)
          netCacheObject.UnseenSkinIds.Add(nullable.Value);
      }
    }
    this.OnNetCacheObjReceived<NetCache.NetCacheBattlegroundsGuideSkins>(netCacheObject);
  }

  private void OnSetBattlegroundsFavoriteGuideSkinResponse()
  {
    SetBattlegroundsFavoriteGuideSkinResponse guideSkinResponse = Network.Get().GetSetBattlegroundsFavoriteGuideSkinResponse();
    if (!guideSkinResponse.Success)
      return;
    BattlegroundsGuideSkinId? newFavoriteBattlegroundsGuideSkinID = BattlegroundsGuideSkinId.FromUntrustedValue(guideSkinResponse.GuideSkinId);
    if (!newFavoriteBattlegroundsGuideSkinID.HasValue || !CollectionManager.Get().IsValidBattlegroundsGuideSkinId(newFavoriteBattlegroundsGuideSkinID.Value))
      Log.Net.PrintError("OnSetBattlegroundsFavoriteGuideSkinResponse FAILED - invalid skin ID (GuideSkinId = {0})", (object) newFavoriteBattlegroundsGuideSkinID);
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    if (netObject == null)
      return;
    netObject.BattlegroundsFavoriteGuideSkin = newFavoriteBattlegroundsGuideSkinID;
    if (this.FavoriteBattlegroundsGuideSkinChanged == null)
      return;
    this.FavoriteBattlegroundsGuideSkinChanged(newFavoriteBattlegroundsGuideSkinID);
  }

  private void OnClearBattlegroundsFavoriteGuideSkinResponse()
  {
    if (!Network.Get().GetClearBattlegroundsFavoriteGuideSkinResponse().Success)
      return;
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    if (netObject == null)
      return;
    netObject.BattlegroundsFavoriteGuideSkin = new BattlegroundsGuideSkinId?();
    if (this.FavoriteBattlegroundsGuideSkinChanged == null)
      return;
    this.FavoriteBattlegroundsGuideSkinChanged(new BattlegroundsGuideSkinId?());
  }

  private void OnBattlegroundsBoardSkinsResponse()
  {
    BattlegroundsBoardSkinsResponse boardSkinsResponse = Network.Get().GetBattlegroundsBoardSkinsResponse();
    NetCache.NetCacheBattlegroundsBoardSkins netCacheObject = new NetCache.NetCacheBattlegroundsBoardSkins();
    netCacheObject.BattlegroundsFavoriteBoardSkin = new BattlegroundsBoardSkinId?();
    foreach (BattlegroundsBoardSkinInfo ownedSkin in boardSkinsResponse.OwnedSkins)
    {
      BattlegroundsBoardSkinId? nullable = BattlegroundsBoardSkinId.FromUntrustedValue(ownedSkin.BoardSkinId);
      if (!nullable.HasValue)
        Log.Net.PrintError("OnBattlegroundsBoardSkinsResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      else if (nullable.Value.IsDefaultBoard())
        Log.Net.PrintError("OnBattlegroundsBoardSkinsResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      else if (!CollectionManager.Get().IsValidBattlegroundsBoardSkinId(nullable.Value))
      {
        Log.Net.PrintError("OnBattlegroundsBoardSkinsResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      }
      else
      {
        netCacheObject.OwnedBattlegroundsBoardSkins.Add(nullable.Value);
        if (ownedSkin.IsFavorite)
        {
          if (netCacheObject.BattlegroundsFavoriteBoardSkin.HasValue)
            Log.Net.PrintError("OnBattlegroundsBoardSkinsResponse FAILED (multiple favorite skins)");
          else
            netCacheObject.BattlegroundsFavoriteBoardSkin = nullable;
        }
        if (!ownedSkin.HasSeen)
          netCacheObject.UnseenSkinIds.Add(nullable.Value);
      }
    }
    this.OnNetCacheObjReceived<NetCache.NetCacheBattlegroundsBoardSkins>(netCacheObject);
  }

  private void OnSetBattlegroundsFavoriteBoardSkinResponse()
  {
    SetBattlegroundsFavoriteBoardSkinResponse boardSkinResponse = Network.Get().GetSetBattlegroundsFavoriteBoardSkinResponse();
    if (!boardSkinResponse.Success)
      return;
    BattlegroundsBoardSkinId? newFavoriteBattlegroundsBoardSkinID = BattlegroundsBoardSkinId.FromUntrustedValue(boardSkinResponse.BoardSkinId);
    if (!newFavoriteBattlegroundsBoardSkinID.HasValue || !CollectionManager.Get().IsValidBattlegroundsBoardSkinId(newFavoriteBattlegroundsBoardSkinID.Value))
      Log.Net.PrintError("OnSetBattlegroundsFavoriteBoardSkinResponse FAILED - invalid skin ID (BoardSkinId = {0})", (object) newFavoriteBattlegroundsBoardSkinID);
    NetCache.NetCacheBattlegroundsBoardSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
    if (netObject == null)
      return;
    netObject.BattlegroundsFavoriteBoardSkin = newFavoriteBattlegroundsBoardSkinID;
    if (this.FavoriteBattlegroundsBoardSkinChanged == null)
      return;
    this.FavoriteBattlegroundsBoardSkinChanged(newFavoriteBattlegroundsBoardSkinID);
  }

  private void OnClearBattlegroundsFavoriteBoardSkinResponse()
  {
    if (!Network.Get().GetClearBattlegroundsFavoriteBoardSkinResponse().Success)
      return;
    NetCache.NetCacheBattlegroundsBoardSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
    if (netObject == null)
      return;
    netObject.BattlegroundsFavoriteBoardSkin = new BattlegroundsBoardSkinId?();
    if (this.FavoriteBattlegroundsBoardSkinChanged == null)
      return;
    this.FavoriteBattlegroundsBoardSkinChanged(new BattlegroundsBoardSkinId?());
  }

  private void OnBattlegroundsFinishersResponse()
  {
    BattlegroundsFinishersResponse finishersResponse = Network.Get().GetBattlegroundsFinishersResponse();
    NetCache.NetCacheBattlegroundsFinishers netCacheObject = new NetCache.NetCacheBattlegroundsFinishers();
    netCacheObject.BattlegroundsFavoriteFinisher = new BattlegroundsFinisherId?();
    foreach (BattlegroundsFinisherInfo ownedSkin in finishersResponse.OwnedSkins)
    {
      BattlegroundsFinisherId? nullable = BattlegroundsFinisherId.FromUntrustedValue(ownedSkin.FinisherId);
      if (!nullable.HasValue)
        Log.Net.PrintError("OnBattlegroundsFinishersResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      else if (nullable.Value.IsDefaultFinisher())
        Log.Net.PrintError("OnBattlegroundsFinishersResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      else if (!CollectionManager.Get().IsValidBattlegroundsFinisherId(nullable.Value))
      {
        Log.Net.PrintError("OnBattlegroundsFinishersResponse FAILED (packetOwnedSkin = {0})", (object) ownedSkin);
      }
      else
      {
        netCacheObject.OwnedBattlegroundsFinishers.Add(nullable.Value);
        if (ownedSkin.IsFavorite)
        {
          if (netCacheObject.BattlegroundsFavoriteFinisher.HasValue)
            Log.Net.PrintError("OnBattlegroundsFinishersResponse FAILED (multiple favorite skins)");
          else
            netCacheObject.BattlegroundsFavoriteFinisher = nullable;
        }
        if (!ownedSkin.HasSeen)
          netCacheObject.UnseenSkinIds.Add(nullable.Value);
      }
    }
    this.OnNetCacheObjReceived<NetCache.NetCacheBattlegroundsFinishers>(netCacheObject);
  }

  private void OnSetBattlegroundsFavoriteFinisherResponse()
  {
    SetBattlegroundsFavoriteFinisherResponse finisherResponse = Network.Get().GetSetBattlegroundsFavoriteFinisherResponse();
    if (!finisherResponse.Success)
      return;
    BattlegroundsFinisherId? newFavoriteBattlegroundsFinisherID = BattlegroundsFinisherId.FromUntrustedValue(finisherResponse.FinisherId);
    if (!newFavoriteBattlegroundsFinisherID.HasValue || !CollectionManager.Get().IsValidBattlegroundsFinisherId(newFavoriteBattlegroundsFinisherID.Value))
      Log.Net.PrintError("OnSetBattlegroundsFavoriteFinisherResponse FAILED - invalid skin ID (FinisherId = {0})", (object) newFavoriteBattlegroundsFinisherID);
    NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    if (netObject == null)
      return;
    netObject.BattlegroundsFavoriteFinisher = newFavoriteBattlegroundsFinisherID;
    if (this.FavoriteBattlegroundsFinisherChanged == null)
      return;
    this.FavoriteBattlegroundsFinisherChanged(newFavoriteBattlegroundsFinisherID);
  }

  private void OnClearBattlegroundsFavoriteFinisherResponse()
  {
    if (!Network.Get().GetClearBattlegroundsFavoriteFinisherResponse().Success)
      return;
    NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    if (netObject == null)
      return;
    netObject.BattlegroundsFavoriteFinisher = new BattlegroundsFinisherId?();
    if (this.FavoriteBattlegroundsFinisherChanged == null)
      return;
    this.FavoriteBattlegroundsFinisherChanged(new BattlegroundsFinisherId?());
  }

  private void OnPlayerRecords() => this.OnPlayerRecordsPacket(Network.Get().GetPlayerRecordsPacket());

  public void OnPlayerRecordsPacket(PlayerRecords packet) => this.OnNetCacheObjReceived<NetCache.NetCachePlayerRecords>(Network.GetPlayerRecords(packet));

  private void OnRewardProgress() => this.OnNetCacheObjReceived<NetCache.NetCacheRewardProgress>(Network.Get().GetRewardProgress());

  private NetCache.NetCacheHeroLevels GetAllHeroXP(HeroXP packet)
  {
    if (packet == null)
      return new NetCache.NetCacheHeroLevels();
    NetCache.NetCacheHeroLevels allHeroXp = new NetCache.NetCacheHeroLevels();
    for (int index = 0; index < packet.XpInfos.Count; ++index)
    {
      HeroXPInfo xpInfo = packet.XpInfos[index];
      allHeroXp.Levels.Add(new NetCache.HeroLevel()
      {
        Class = (TAG_CLASS) xpInfo.ClassId,
        CurrentLevel = {
          Level = xpInfo.Level,
          XP = xpInfo.CurrXp,
          MaxXP = xpInfo.MaxXp
        }
      });
    }
    return allHeroXp;
  }

  public void OnHeroXP(HeroXP packet)
  {
    NetCache.NetCacheHeroLevels allHeroXp = this.GetAllHeroXP(packet);
    if (this.m_prevHeroLevels != null)
    {
      foreach (NetCache.HeroLevel level in allHeroXp.Levels)
      {
        NetCache.HeroLevel newHeroLevel = level;
        NetCache.HeroLevel heroLevel = this.m_prevHeroLevels.Levels.Find((Predicate<NetCache.HeroLevel>) (obj => obj.Class == newHeroLevel.Class));
        if (heroLevel != null)
        {
          if (newHeroLevel != null && newHeroLevel.CurrentLevel != null && newHeroLevel.CurrentLevel.Level != heroLevel.CurrentLevel.Level && (newHeroLevel.CurrentLevel.Level == 20 || newHeroLevel.CurrentLevel.Level == 30 || newHeroLevel.CurrentLevel.Level == 40 || newHeroLevel.CurrentLevel.Level == 50 || newHeroLevel.CurrentLevel.Level == 60))
          {
            if (newHeroLevel.Class == TAG_CLASS.DRUID)
              BnetPresenceMgr.Get().SetGameField(5U, newHeroLevel.CurrentLevel.Level);
            else if (newHeroLevel.Class == TAG_CLASS.HUNTER)
              BnetPresenceMgr.Get().SetGameField(6U, newHeroLevel.CurrentLevel.Level);
            else if (newHeroLevel.Class == TAG_CLASS.MAGE)
              BnetPresenceMgr.Get().SetGameField(7U, newHeroLevel.CurrentLevel.Level);
            else if (newHeroLevel.Class == TAG_CLASS.PALADIN)
              BnetPresenceMgr.Get().SetGameField(8U, newHeroLevel.CurrentLevel.Level);
            else if (newHeroLevel.Class == TAG_CLASS.PRIEST)
              BnetPresenceMgr.Get().SetGameField(9U, newHeroLevel.CurrentLevel.Level);
            else if (newHeroLevel.Class == TAG_CLASS.ROGUE)
              BnetPresenceMgr.Get().SetGameField(10U, newHeroLevel.CurrentLevel.Level);
            else if (newHeroLevel.Class == TAG_CLASS.SHAMAN)
              BnetPresenceMgr.Get().SetGameField(11U, newHeroLevel.CurrentLevel.Level);
            else if (newHeroLevel.Class == TAG_CLASS.WARLOCK)
              BnetPresenceMgr.Get().SetGameField(12U, newHeroLevel.CurrentLevel.Level);
            else if (newHeroLevel.Class == TAG_CLASS.WARRIOR)
              BnetPresenceMgr.Get().SetGameField(13U, newHeroLevel.CurrentLevel.Level);
          }
          newHeroLevel.PrevLevel = heroLevel.CurrentLevel;
        }
      }
    }
    this.m_prevHeroLevels = allHeroXp;
    this.OnNetCacheObjReceived<NetCache.NetCacheHeroLevels>(allHeroXp);
  }

  private void OnAllHeroXP() => this.OnHeroXP(Network.Get().GetHeroXP());

  private void OnInitialClientState_ProfileNotices(ProfileNotices profileNotices)
  {
    List<NetCache.ProfileNotice> result = new List<NetCache.ProfileNotice>();
    Network.Get().HandleProfileNotices(profileNotices.List, ref result);
    this.m_receivedInitialProfileNotices = true;
    this.HandleIncomingProfileNotices(result, true);
    this.HandleIncomingProfileNotices(this.m_queuedProfileNotices, true);
    this.m_queuedProfileNotices.Clear();
  }

  public void HandleIncomingProfileNotices(
    List<NetCache.ProfileNotice> receivedNotices,
    bool isInitialNoticeList)
  {
    if (!this.m_receivedInitialProfileNotices)
    {
      this.m_queuedProfileNotices.AddRange((IEnumerable<NetCache.ProfileNotice>) receivedNotices);
    }
    else
    {
      if (receivedNotices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.GAINED_MEDAL)) != null)
      {
        this.m_previousMedalInfo = (NetCache.NetCacheMedalInfo) null;
        NetCache.NetCacheMedalInfo netObject = this.GetNetObject<NetCache.NetCacheMedalInfo>();
        if (netObject != null)
          netObject.PreviousMedalInfo = (NetCache.NetCacheMedalInfo) null;
      }
      List<NetCache.ProfileNotice> newNotices = this.FindNewNotices(receivedNotices);
      NetCache.NetCacheProfileNotices netCacheObject = this.GetNetObject<NetCache.NetCacheProfileNotices>() ?? new NetCache.NetCacheProfileNotices();
      for (int index = 0; index < newNotices.Count; ++index)
      {
        if (!this.m_ackedNotices.Contains(newNotices[index].NoticeID))
          netCacheObject.Notices.Add(newNotices[index]);
      }
      this.OnNetCacheObjReceived<NetCache.NetCacheProfileNotices>(netCacheObject);
      NetCache.DelNewNoticesListener[] array = this.m_newNoticesListeners.ToArray();
      foreach (NetCache.ProfileNotice profileNotice in newNotices)
        Log.Achievements.Print("NetCache.OnProfileNotices() sending {0} to {1} listeners", (object) profileNotice, (object) array.Length);
      foreach (NetCache.DelNewNoticesListener newNoticesListener in array)
      {
        Log.Achievements.Print("NetCache.OnProfileNotices(): sending notices to {0}::{1}", (object) newNoticesListener.Method.ReflectedType.Name, (object) newNoticesListener.Method.Name);
        newNoticesListener(newNotices, isInitialNoticeList);
      }
    }
  }

  private List<NetCache.ProfileNotice> FindNewNotices(
    List<NetCache.ProfileNotice> receivedNotices)
  {
    List<NetCache.ProfileNotice> newNotices = new List<NetCache.ProfileNotice>();
    NetCache.NetCacheProfileNotices netObject = this.GetNetObject<NetCache.NetCacheProfileNotices>();
    if (netObject == null)
    {
      newNotices.AddRange((IEnumerable<NetCache.ProfileNotice>) receivedNotices);
    }
    else
    {
      foreach (NetCache.ProfileNotice receivedNotice1 in receivedNotices)
      {
        NetCache.ProfileNotice receivedNotice = receivedNotice1;
        if (netObject.Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.NoticeID == receivedNotice.NoticeID)) == null)
          newNotices.Add(receivedNotice);
      }
    }
    return newNotices;
  }

  public void OnClientOptions(ClientOptions packet)
  {
    NetCache.NetCacheClientOptions netCacheObject = this.GetNetObject<NetCache.NetCacheClientOptions>();
    bool flag = netCacheObject == null;
    if (flag)
      netCacheObject = new NetCache.NetCacheClientOptions();
    if (packet.HasFailed && packet.Failed)
    {
      Debug.LogError((object) "ReadClientOptions: packet.Failed=true. Unable to retrieve client options from UtilServer.");
      Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_GENERIC");
    }
    else
    {
      foreach (PegasusUtil.ClientOption option in packet.Options)
      {
        ServerOption index = (ServerOption) option.Index;
        if (option.HasAsInt32)
          netCacheObject.ClientState[index] = (NetCache.ClientOptionBase) new NetCache.ClientOptionInt(option.AsInt32);
        else if (option.HasAsInt64)
          netCacheObject.ClientState[index] = (NetCache.ClientOptionBase) new NetCache.ClientOptionLong(option.AsInt64);
        else if (option.HasAsFloat)
          netCacheObject.ClientState[index] = (NetCache.ClientOptionBase) new NetCache.ClientOptionFloat(option.AsFloat);
        else if (option.HasAsUint64)
          netCacheObject.ClientState[index] = (NetCache.ClientOptionBase) new NetCache.ClientOptionULong(option.AsUint64);
      }
      netCacheObject.UpdateServerState();
      this.OnNetCacheObjReceived<NetCache.NetCacheClientOptions>(netCacheObject);
      if (flag)
        OptionsMigration.UpgradeServerOptions();
      netCacheObject.RemoveInvalidOptions();
    }
  }

  private void SetClientOption(ServerOption type, NetCache.ClientOptionBase newVal)
  {
    object obj;
    if (!this.m_netCache.TryGetValue(typeof (NetCache.NetCacheClientOptions), out obj) || !(obj is NetCache.NetCacheClientOptions))
    {
      Debug.LogWarning((object) "NetCache.OnClientOptions: Attempting to set an option before initializing the options cache.");
    }
    else
    {
      NetCache.NetCacheClientOptions cacheClientOptions = (NetCache.NetCacheClientOptions) obj;
      cacheClientOptions.ClientState[type] = newVal;
      cacheClientOptions.CheckForDispatchToServer();
      this.NetCacheChanged<NetCache.NetCacheClientOptions>();
    }
  }

  public void SetIntOption(ServerOption type, int val) => this.SetClientOption(type, (NetCache.ClientOptionBase) new NetCache.ClientOptionInt(val));

  public void SetLongOption(ServerOption type, long val) => this.SetClientOption(type, (NetCache.ClientOptionBase) new NetCache.ClientOptionLong(val));

  public void SetFloatOption(ServerOption type, float val) => this.SetClientOption(type, (NetCache.ClientOptionBase) new NetCache.ClientOptionFloat(val));

  public void SetULongOption(ServerOption type, ulong val) => this.SetClientOption(type, (NetCache.ClientOptionBase) new NetCache.ClientOptionULong(val));

  public void DeleteClientOption(ServerOption type) => this.SetClientOption(type, (NetCache.ClientOptionBase) null);

  public bool ClientOptionExists(ServerOption type)
  {
    NetCache.NetCacheClientOptions netObject = this.GetNetObject<NetCache.NetCacheClientOptions>();
    return netObject != null && netObject.ClientState.ContainsKey(type) && netObject.ClientState[type] != null;
  }

  public void DispatchClientOptionsToServer() => NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>()?.DispatchClientOptionsToServer();

  private void OnFavoriteHeroesResponse()
  {
    FavoriteHeroesResponse favoriteHeroesResponse = Network.Get().GetFavoriteHeroesResponse();
    NetCache.NetCacheFavoriteHeroes cacheFavoriteHeroes = new NetCache.NetCacheFavoriteHeroes();
    foreach (FavoriteHero favoriteHero in favoriteHeroesResponse.FavoriteHeroes)
    {
      TAG_CLASS outVal1;
      if (!Blizzard.T5.Core.Utils.EnumUtils.TryCast<TAG_CLASS>((object) favoriteHero.ClassId, out outVal1))
      {
        Debug.LogWarning((object) string.Format("NetCache.OnFavoriteHeroesResponse() unrecognized hero class {0}", (object) favoriteHero.ClassId));
      }
      else
      {
        TAG_PREMIUM outVal2;
        if (!Blizzard.T5.Core.Utils.EnumUtils.TryCast<TAG_PREMIUM>((object) favoriteHero.Hero.Premium, out outVal2))
        {
          Debug.LogWarning((object) string.Format("NetCache.OnFavoriteHeroesResponse() unrecognized hero premium {0} for hero class {1}", (object) favoriteHero.Hero.Premium, (object) outVal1));
        }
        else
        {
          NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition()
          {
            Name = GameUtils.TranslateDbIdToCardId(favoriteHero.Hero.Asset),
            Premium = outVal2
          };
          cacheFavoriteHeroes.FavoriteHeroes.Add((outVal1, cardDefinition));
        }
      }
    }
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    List<SetFavoriteHero> favoriteHeroFromDiff = OfflineDataCache.GenerateSetFavoriteHeroFromDiff(data, cacheFavoriteHeroes);
    if (favoriteHeroFromDiff.Any<SetFavoriteHero>())
    {
      foreach (SetFavoriteHero setFavoriteHero in favoriteHeroFromDiff)
      {
        NetCache.CardDefinition hero = new NetCache.CardDefinition()
        {
          Name = GameUtils.TranslateDbIdToCardId(setFavoriteHero.FavoriteHero.Hero.Asset),
          Premium = (TAG_PREMIUM) setFavoriteHero.FavoriteHero.Hero.Premium
        };
        Network.Get().SetFavoriteHero((TAG_CLASS) setFavoriteHero.FavoriteHero.ClassId, hero, setFavoriteHero.IsFavorite);
      }
      OfflineDataCache.ClearFavoriteHeroesDirtyFlag();
    }
    this.OnNetCacheObjReceived<NetCache.NetCacheFavoriteHeroes>(cacheFavoriteHeroes);
    OfflineDataCache.CacheFavoriteHeroes(ref data, favoriteHeroesResponse);
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  private void OnSetFavoriteHeroResponse()
  {
    Network.SetFavoriteHeroResponse favoriteHeroResponse = Network.Get().GetSetFavoriteHeroResponse();
    if (!favoriteHeroResponse.Success)
      return;
    if (TAG_CLASS.NEUTRAL == favoriteHeroResponse.HeroClass || favoriteHeroResponse.Hero == null)
    {
      Debug.LogWarning((object) string.Format("NetCache.OnSetFavoriteHeroResponse: setting hero was a success, but message contains invalid class ({0}) and/or hero ({1})", (object) favoriteHeroResponse.HeroClass, (object) favoriteHeroResponse.Hero));
    }
    else
    {
      NetCache.NetCacheFavoriteHeroes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFavoriteHeroes>();
      if (netObject != null)
      {
        if (favoriteHeroResponse.IsFavorite)
          netObject.FavoriteHeroes.Add((favoriteHeroResponse.HeroClass, favoriteHeroResponse.Hero));
        else
          netObject.FavoriteHeroes.Remove((favoriteHeroResponse.HeroClass, favoriteHeroResponse.Hero));
        Log.CollectionManager.Print("CollectionManager.OnSetFavoriteHeroResponse: favorite hero status for {0} updated to {1}", (object) favoriteHeroResponse.Hero, (object) favoriteHeroResponse.IsFavorite);
      }
      CollectionManager.Get()?.UpdateFavoriteHero(favoriteHeroResponse.HeroClass, favoriteHeroResponse.Hero.Name, favoriteHeroResponse.Hero.Premium, favoriteHeroResponse.IsFavorite);
      PegasusShared.CardDef cardDef = new PegasusShared.CardDef()
      {
        Asset = GameUtils.TranslateCardIdToDbId(favoriteHeroResponse.Hero.Name),
        Premium = (int) favoriteHeroResponse.Hero.Premium
      };
      OfflineDataCache.SetFavoriteHero((int) favoriteHeroResponse.HeroClass, cardDef, false, favoriteHeroResponse.IsFavorite);
    }
  }

  private void OnAccountLicensesInfoResponse()
  {
    AccountLicensesInfoResponse licensesInfoResponse = Network.Get().GetAccountLicensesInfoResponse();
    NetCache.NetCacheAccountLicenses netCacheObject = new NetCache.NetCacheAccountLicenses();
    foreach (AccountLicenseInfo accountLicenseInfo in licensesInfoResponse.List)
      netCacheObject.AccountLicenses[accountLicenseInfo.License] = accountLicenseInfo;
    this.OnNetCacheObjReceived<NetCache.NetCacheAccountLicenses>(netCacheObject);
  }

  private void OnClientStaticAssetsResponse()
  {
    ClientStaticAssetsResponse staticAssetsResponse = Network.Get().GetClientStaticAssetsResponse();
    if (staticAssetsResponse == null)
      return;
    this.OnNetCacheObjReceived<ClientStaticAssetsResponse>(staticAssetsResponse);
  }

  private void OnFSGFeatureConfig()
  {
    FSGFeatureConfig fsgFeatureConfig = Network.Get().GetFSGFeatureConfig();
    if (fsgFeatureConfig == null)
      return;
    this.OnNetCacheObjReceived<FSGFeatureConfig>(fsgFeatureConfig);
  }

  private void OnMercenariesTeamListResponse()
  {
    MercenariesTeamListResponse teamListResponse = Network.Get().MercenariesTeamListResponse();
    if (teamListResponse == null || !teamListResponse.HasTeamList)
      return;
    this.OnNetCacheObjReceived<LettuceTeamList>(teamListResponse.TeamList);
  }

  private void RegisterNetCacheHandlers()
  {
    Network network = Network.Get();
    network.RegisterNetHandler((object) PegasusUtil.DBAction.PacketID.ID, new Network.NetHandler(this.OnDBAction));
    network.RegisterNetHandler((object) PegasusUtil.GenericResponse.PacketID.ID, new Network.NetHandler(this.OnGenericResponse));
    network.RegisterNetHandler((object) InitialClientState.PacketID.ID, new Network.NetHandler(this.OnInitialClientState));
    network.RegisterNetHandler((object) MedalInfo.PacketID.ID, new Network.NetHandler(this.OnMedalInfo));
    network.RegisterNetHandler((object) BattlegroundsRatingInfoResponse.PacketID.ID, new Network.NetHandler(this.OnBaconRatingInfo));
    network.RegisterNetHandler((object) ProfileProgress.PacketID.ID, new Network.NetHandler(this.OnProfileProgress));
    network.RegisterNetHandler((object) GamesInfo.PacketID.ID, new Network.NetHandler(this.OnGamesInfo));
    network.RegisterNetHandler((object) CardValues.PacketID.ID, new Network.NetHandler(this.OnCardValues));
    network.RegisterNetHandler((object) GuardianVars.PacketID.ID, new Network.NetHandler(this.OnGuardianVars));
    network.RegisterNetHandler((object) PlayerRecords.PacketID.ID, new Network.NetHandler(this.OnPlayerRecords));
    network.RegisterNetHandler((object) RewardProgress.PacketID.ID, new Network.NetHandler(this.OnRewardProgress));
    network.RegisterNetHandler((object) HeroXP.PacketID.ID, new Network.NetHandler(this.OnAllHeroXP));
    network.RegisterNetHandler((object) CardBacks.PacketID.ID, new Network.NetHandler(this.OnCardBacks));
    network.RegisterNetHandler((object) SetFavoriteCardBackResponse.PacketID.ID, new Network.NetHandler(this.OnSetFavoriteCardBackResponse));
    network.RegisterNetHandler((object) FavoriteHeroesResponse.PacketID.ID, new Network.NetHandler(this.OnFavoriteHeroesResponse));
    network.RegisterNetHandler((object) PegasusUtil.SetFavoriteHeroResponse.PacketID.ID, new Network.NetHandler(this.OnSetFavoriteHeroResponse));
    network.RegisterNetHandler((object) AccountLicensesInfoResponse.PacketID.ID, new Network.NetHandler(this.OnAccountLicensesInfoResponse));
    network.RegisterNetHandler((object) DeckList.PacketID.ID, new Network.NetHandler(this.OnReceivedDeckHeaders));
    network.RegisterNetHandler((object) PVPDRStatsInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRStatsInfo));
    network.RegisterNetHandler((object) Coins.PacketID.ID, new Network.NetHandler(this.OnCoins));
    network.RegisterNetHandler((object) SetFavoriteCoinResponse.PacketID.ID, new Network.NetHandler(this.OnSetFavoriteCoinResponse));
    network.RegisterNetHandler((object) BattlegroundsHeroSkinsResponse.PacketID.ID, new Network.NetHandler(this.OnBattlegroundsHeroSkinsResponse));
    network.RegisterNetHandler((object) SetBattlegroundsFavoriteHeroSkinResponse.PacketID.ID, new Network.NetHandler(this.OnSetBattlegroundsFavoriteHeroSkinResponse));
    network.RegisterNetHandler((object) ClearBattlegroundsFavoriteHeroSkinResponse.PacketID.ID, new Network.NetHandler(this.OnClearBattlegroundsFavoriteHeroSkinResponse));
    network.RegisterNetHandler((object) BattlegroundsGuideSkinsResponse.PacketID.ID, new Network.NetHandler(this.OnBattlegroundsGuideSkinsResponse));
    network.RegisterNetHandler((object) SetBattlegroundsFavoriteGuideSkinResponse.PacketID.ID, new Network.NetHandler(this.OnSetBattlegroundsFavoriteGuideSkinResponse));
    network.RegisterNetHandler((object) ClearBattlegroundsFavoriteGuideSkinResponse.PacketID.ID, new Network.NetHandler(this.OnClearBattlegroundsFavoriteGuideSkinResponse));
    network.RegisterNetHandler((object) BattlegroundsBoardSkinsResponse.PacketID.ID, new Network.NetHandler(this.OnBattlegroundsBoardSkinsResponse));
    network.RegisterNetHandler((object) SetBattlegroundsFavoriteBoardSkinResponse.PacketID.ID, new Network.NetHandler(this.OnSetBattlegroundsFavoriteBoardSkinResponse));
    network.RegisterNetHandler((object) ClearBattlegroundsFavoriteBoardSkinResponse.PacketID.ID, new Network.NetHandler(this.OnClearBattlegroundsFavoriteBoardSkinResponse));
    network.RegisterNetHandler((object) BattlegroundsFinishersResponse.PacketID.ID, new Network.NetHandler(this.OnBattlegroundsFinishersResponse));
    network.RegisterNetHandler((object) SetBattlegroundsFavoriteFinisherResponse.PacketID.ID, new Network.NetHandler(this.OnSetBattlegroundsFavoriteFinisherResponse));
    network.RegisterNetHandler((object) ClearBattlegroundsFavoriteFinisherResponse.PacketID.ID, new Network.NetHandler(this.OnClearBattlegroundsFavoriteFinisherResponse));
    network.RegisterNetHandler((object) BattlegroundsEmotesResponse.PacketID.ID, new Network.NetHandler(this.OnBattlegroundsEmotesResponse));
    network.RegisterNetHandler((object) SetBattlegroundsEmoteLoadoutResponse.PacketID.ID, new Network.NetHandler(this.OnSetBattlegroundsEmoteLoadoutResponse));
    network.RegisterNetHandler((object) Deadend.PacketID.ID, new Network.NetHandler(this.OnHearthstoneUnavailableGame));
    network.RegisterNetHandler((object) DeadendUtil.PacketID.ID, new Network.NetHandler(this.OnHearthstoneUnavailableUtil));
    network.RegisterNetHandler((object) ClientStaticAssetsResponse.PacketID.ID, new Network.NetHandler(this.OnClientStaticAssetsResponse));
    network.RegisterNetHandler((object) FSGFeatureConfig.PacketID.ID, new Network.NetHandler(this.OnFSGFeatureConfig));
    network.RegisterNetHandler((object) MercenariesTeamListResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesTeamListResponse));
    network.RegisterNetHandler((object) LettuceMapResponse.PacketID.ID, new Network.NetHandler(this.OnLettuceMapResponse));
    network.RegisterNetHandler((object) MercenariesPlayerInfoResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesPlayerInfoResponse));
    network.RegisterNetHandler((object) MercenariesPvPWinUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesPvPWinUpdate));
    network.RegisterNetHandler((object) MercenariesPlayerBountyInfoUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesPlayerBountyInfoUpdate));
    network.RegisterNetHandler((object) MercenariesBountyAcknowledgeResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesBountyAcknowledgeResponse));
    network.RegisterNetHandler((object) MercenariesGetVillageResponse.PacketID.ID, new Network.NetHandler(this.OnVillageDataResponse));
    network.RegisterNetHandler((object) MercenariesBuildingStateUpdate.PacketID.ID, new Network.NetHandler(this.OnVillageBuildingStateUpdated));
    network.RegisterNetHandler((object) MercenariesVisitorStateUpdate.PacketID.ID, new Network.NetHandler(this.OnVillageVisitorStateUpdated));
    network.RegisterNetHandler((object) MercenariesRefreshVisitorsResponse.PacketID.ID, new Network.NetHandler(this.OnRefreshVisitorDataResponse));
  }

  public void RegisterCollectionManager(NetCache.NetCacheCallback callback) => this.RegisterCollectionManager(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterCollectionManager(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest request = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterCollectionManager));
    this.AddCollectionManagerToRequest(ref request);
    this.NetCacheMakeBatchRequest(request);
  }

  public void RegisterScreenCollectionManager(NetCache.NetCacheCallback callback) => this.RegisterScreenCollectionManager(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenCollectionManager(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest request = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenCollectionManager));
    this.AddCollectionManagerToRequest(ref request);
    request.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCacheCollection)),
      new NetCache.Request(typeof (NetCache.NetCacheFeatures)),
      new NetCache.Request(typeof (NetCache.NetCacheHeroLevels))
    });
    this.NetCacheMakeBatchRequest(request);
  }

  public void RegisterScreenForge(NetCache.NetCacheCallback callback) => this.RegisterScreenForge(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenForge(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest request = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenForge));
    this.AddCollectionManagerToRequest(ref request);
    request.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCacheFeatures)),
      new NetCache.Request(typeof (NetCache.NetCacheHeroLevels))
    });
    this.NetCacheMakeBatchRequest(request);
  }

  public void RegisterScreenTourneys(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenTourneys));
    batchRequest.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCachePlayerRecords)),
      new NetCache.Request(typeof (NetCache.NetCacheDecks)),
      new NetCache.Request(typeof (NetCache.NetCacheFeatures)),
      new NetCache.Request(typeof (NetCache.NetCacheHeroLevels))
    });
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  public void RegisterScreenFriendly(NetCache.NetCacheCallback callback) => this.RegisterScreenFriendly(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenFriendly(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenFriendly));
    batchRequest.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCacheDecks)),
      new NetCache.Request(typeof (NetCache.NetCacheHeroLevels))
    });
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  public void RegisterScreenPractice(NetCache.NetCacheCallback callback) => this.RegisterScreenPractice(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenPractice(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenPractice));
    batchRequest.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCacheDecks)),
      new NetCache.Request(typeof (NetCache.NetCacheFeatures)),
      new NetCache.Request(typeof (NetCache.NetCacheHeroLevels)),
      new NetCache.Request(typeof (NetCache.NetCacheRewardProgress))
    });
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  public void RegisterScreenEndOfGame(NetCache.NetCacheCallback callback) => this.RegisterScreenEndOfGame(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenEndOfGame(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    GameMgr service;
    if (ServiceManager.TryGet<GameMgr>(out service) && service.IsSpectator())
    {
      Processor.ScheduleCallback(0.0f, false, (Processor.ScheduledCallback) (userData => callback()));
    }
    else
    {
      NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenEndOfGame));
      batchRequest.AddRequests(new List<NetCache.Request>()
      {
        new NetCache.Request(typeof (NetCache.NetCacheMedalInfo), true),
        new NetCache.Request(typeof (NetCache.NetCacheHeroLevels), true)
      });
      this.NetCacheMakeBatchRequest(batchRequest);
      int num = service != null ? (int) service.GetGameType() : 0;
      bool flag = GameUtils.IsTavernBrawlGameType((PegasusShared.GameType) num);
      if (num == 2 && FriendChallengeMgr.Get().IsChallengeTavernBrawl())
      {
        NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
        if (netObject != null && netObject.FriendWeekAllowsTavernBrawlRecordUpdate && SpecialEventManager.Get().IsEventActive(SpecialEventType.FRIEND_WEEK, false))
          flag = true;
      }
      if (flag)
        TavernBrawlManager.Get().RefreshPlayerRecord();
      if (!GameUtils.IsFiresideGatheringGameType((PegasusShared.GameType) num))
        return;
      Network.Get().RequestFSGPatronListUpdate();
    }
  }

  public void RegisterScreenPackOpening(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenPackOpening));
    batchRequest.AddRequest(new NetCache.Request(typeof (NetCache.NetCacheBoosters)));
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  public void RegisterScreenBox(NetCache.NetCacheCallback callback) => this.RegisterScreenBox(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenBox(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenBox));
    Debug.Log((object) ("RegisterScreenBox tempGuardianVars=" + (object) this.GetNetObject<NetCache.NetCacheFeatures>()));
    batchRequest.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCacheBoosters)),
      new NetCache.Request(typeof (NetCache.NetCacheClientOptions)),
      new NetCache.Request(typeof (NetCache.NetCacheProfileProgress)),
      new NetCache.Request(typeof (NetCache.NetCacheFeatures)),
      new NetCache.Request(typeof (NetCache.NetCacheMedalInfo)),
      new NetCache.Request(typeof (NetCache.NetCacheHeroLevels))
    });
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  public void RegisterScreenStartup(NetCache.NetCacheCallback callback) => this.RegisterScreenStartup(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenStartup(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenStartup));
    batchRequest.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCacheProfileProgress))
    });
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  public void RegisterScreenLogin(NetCache.NetCacheCallback callback) => this.RegisterScreenLogin(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenLogin(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenLogin));
    batchRequest.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCacheRewardProgress)),
      new NetCache.Request(typeof (NetCache.NetCachePlayerRecords)),
      new NetCache.Request(typeof (NetCache.NetCacheGoldBalance)),
      new NetCache.Request(typeof (NetCache.NetCacheHeroLevels)),
      new NetCache.Request(typeof (NetCache.NetCacheCardBacks), true),
      new NetCache.Request(typeof (NetCache.NetCacheFavoriteHeroes), true),
      new NetCache.Request(typeof (NetCache.NetCacheAccountLicenses)),
      new NetCache.Request(typeof (ClientStaticAssetsResponse)),
      new NetCache.Request(typeof (NetCache.NetCacheClientOptions)),
      new NetCache.Request(typeof (NetCache.NetCacheCoins)),
      new NetCache.Request(typeof (NetCache.NetCacheBattlegroundsHeroSkins)),
      new NetCache.Request(typeof (NetCache.NetCacheBattlegroundsGuideSkins)),
      new NetCache.Request(typeof (NetCache.NetCacheBattlegroundsBoardSkins)),
      new NetCache.Request(typeof (NetCache.NetCacheBattlegroundsFinishers)),
      new NetCache.Request(typeof (NetCache.NetCacheBattlegroundsEmotes))
    });
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  public void RegisterTutorialEndGameScreen(NetCache.NetCacheCallback callback) => this.RegisterTutorialEndGameScreen(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterTutorialEndGameScreen(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    GameMgr service;
    if (ServiceManager.TryGet<GameMgr>(out service) && service.IsSpectator())
    {
      Processor.ScheduleCallback(0.0f, false, (Processor.ScheduledCallback) (userData => callback()));
    }
    else
    {
      NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterTutorialEndGameScreen));
      batchRequest.AddRequests(new List<NetCache.Request>()
      {
        new NetCache.Request(typeof (NetCache.NetCacheProfileProgress))
      });
      this.NetCacheMakeBatchRequest(batchRequest);
    }
  }

  public void RegisterFriendChallenge(NetCache.NetCacheCallback callback) => this.RegisterFriendChallenge(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterFriendChallenge(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterFriendChallenge));
    batchRequest.AddRequest(new NetCache.Request(typeof (NetCache.NetCacheProfileProgress)));
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  public void RegisterScreenBattlegrounds(NetCache.NetCacheCallback callback) => this.RegisterScreenBattlegrounds(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));

  public void RegisterScreenBattlegrounds(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback)
  {
    NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenBattlegrounds));
    batchRequest.AddRequests(new List<NetCache.Request>()
    {
      new NetCache.Request(typeof (NetCache.NetCacheFeatures))
    });
    this.NetCacheMakeBatchRequest(batchRequest);
  }

  private void AddCollectionManagerToRequest(ref NetCache.NetCacheBatchRequest request) => request.AddRequests(new List<NetCache.Request>()
  {
    new NetCache.Request(typeof (NetCache.NetCacheProfileNotices)),
    new NetCache.Request(typeof (NetCache.NetCacheDecks)),
    new NetCache.Request(typeof (NetCache.NetCacheCollection)),
    new NetCache.Request(typeof (NetCache.NetCacheCardValues)),
    new NetCache.Request(typeof (NetCache.NetCacheArcaneDustBalance)),
    new NetCache.Request(typeof (NetCache.NetCacheClientOptions))
  });

  private void OnPacketThrottled(int packetID, long retryMillis)
  {
    if (packetID != 201)
      return;
    float num = Time.realtimeSinceStartup + (float) retryMillis / 1000f;
    foreach (NetCache.NetCacheBatchRequest cacheRequest in this.m_cacheRequests)
      cacheRequest.m_timeAdded = num;
  }

  public void Cheat_AddNotice(NetCache.ProfileNotice notice)
  {
    if (!HearthstoneApplication.IsInternal())
      return;
    this.UnloadNetObject<NetCache.NetCacheProfileNotices>();
    PopupDisplayManager.Get().RewardPopups.ClearSeenNotices();
    notice.NoticeID = 9999L;
    this.m_ackedNotices.Remove(notice.NoticeID);
    this.HandleIncomingProfileNotices(new List<NetCache.ProfileNotice>()
    {
      notice
    }, false);
  }

  public delegate void DelNewNoticesListener(
    List<NetCache.ProfileNotice> newNotices,
    bool isInitialNoticeList);

  public delegate void DelGoldBalanceListener(NetCache.NetCacheGoldBalance balance);

  public delegate void DelFavoriteCardBackChangedListener(
    int newFavoriteCardBackID,
    bool isFavorite);

  public delegate void DelFavoriteBattlegroundsHeroSkinChangedListener(
    int baseSkinId,
    BattlegroundsHeroSkinId? newFavoriteBattlegroundsHeroSkinID);

  public delegate void DelFavoriteBattlegroundsGuideSkinChangedListener(
    BattlegroundsGuideSkinId? newFavoriteBattlegroundsGuideSkinID);

  public delegate void DelFavoriteBattlegroundsBoardSkinChangedListener(
    BattlegroundsBoardSkinId? newFavoriteBattlegroundsBoardSkinID);

  public delegate void DelFavoriteBattlegroundsFinisherChangedListener(
    BattlegroundsFinisherId? newFavoriteBattlegroundsFinisherID);

  public delegate void DelBattlegroundsEmoteLoadoutChangedListener(
    Hearthstone.BattlegroundsEmoteLoadout newLoadout);

  public delegate void DelFavoriteCoinChangedListener(int newFavoriteCoinID);

  public delegate void DelOwnedBattlegroundsSkinsChanged();

  public class NetCacheGamesPlayed
  {
    public int GamesStarted { get; set; }

    public int GamesWon { get; set; }

    public int GamesLost { get; set; }
  }

  public class NetCacheFeatures
  {
    public bool CaisEnabledNonMobile;
    public bool CaisEnabledMobileChina;
    public bool CaisEnabledMobileSouthKorea;
    public bool SendTelemetryPresence;

    public NetCache.NetCacheFeatures.CacheMisc Misc { get; set; }

    public NetCache.NetCacheFeatures.CacheGames Games { get; set; }

    public NetCache.NetCacheFeatures.CacheCollection Collection { get; set; }

    public NetCache.NetCacheFeatures.CacheStore Store { get; set; }

    public NetCache.NetCacheFeatures.CacheHeroes Heroes { get; set; }

    public NetCache.NetCacheFeatures.CacheMercenaries Mercenaries { get; set; }

    public NetCache.NetCacheFeatures.CacheTraceroute Traceroute { get; set; }

    public int XPSoloLimit { get; set; }

    public int MaxHeroLevel { get; set; }

    public float SpecialEventTimingMod
    {
      set => this.\u003CSpecialEventTimingMod\u003Ek__BackingField = value;
    }

    public int FriendWeekConcederMaxDefense { get; set; }

    public int FriendWeekConcededGameMinTotalTurns { get; set; }

    public bool FriendWeekAllowsTavernBrawlRecordUpdate { get; set; }

    public bool FSGEnabled { get; set; }

    public bool FSGAutoCheckinEnabled { get; set; }

    public bool FSGLoginScanEnabled { get; set; }

    public bool FSGShowBetaLabel { get; set; }

    public int FSGFriendListPatronCountLimit { get; set; }

    public uint ArenaClosedToNewSessionsSeconds { get; set; }

    public uint PVPDRClosedToNewSessionsSeconds { get; set; }

    public int FsgMaxPresencePubscribedPatronCount { get; set; }

    public bool QuickOpenEnabled { get; set; }

    public bool ForceIosLowRes { get; set; }

    public bool EnableSmartDeckCompletion { get; set; }

    public bool AllowOfflineClientActivity { get; set; }

    public bool AllowOfflineClientDeckDeletion { get; set; }

    public int BattlegroundsEarlyAccessLicense
    {
      set => this.\u003CBattlegroundsEarlyAccessLicense\u003Ek__BackingField = value;
    }

    public int BattlegroundsMaxRankedPartySize { get; set; }

    public bool JournalButtonDisabled { get; set; }

    public bool AchievementToastDisabled { get; set; }

    public uint DuelsEarlyAccessLicense { get; set; }

    public bool ContentstackEnabled { get; set; }

    public bool PersonalizedMessagesEnabled { get; set; }

    public bool AppRatingEnabled { get; set; }

    public float AppRatingSamplingPercentage { get; set; }

    public List<int> DuelsCardDenylist { get; set; }

    public List<int> ConstructedCardDenylist { get; set; }

    public bool BattlegroundsSkinsEnabled { get; set; }

    public bool BattlegroundsBoardSkinsEnabled { get; set; }

    public bool BattlegroundsFinishersEnabled { get; set; }

    public bool BattlegroundsEmotesEnabled { get; set; }

    public bool BattlegroundsRewardTrackEnabled { get; set; }

    public bool TutorialPreviewVideosEnabled { get; set; }

    public float TutorialPreviewVideosTimeout { get; set; }

    public bool MercenariesEnableVillages { get; set; }

    public bool MercenariesPackOpeningEnabled { get; set; }

    public int MercenariesTeamMaxSize { get; set; }

    public int MinHPForProgressAfterConcede { get; set; }

    public int MinTurnsForProgressAfterConcede { get; set; }

    public bool EnablePlayingFromMiniHand { get; set; }

    public bool BattlegroundsMedalFriendListDisplayEnabled { get; set; }

    public bool EnableUpgradeToGolden { get; set; }

    public bool ShouldPrevalidatePastedDeckCodes { get; set; }

    public bool RecentFriendListDisplayEnabled { get; set; }

    public bool OvercappedDecksEnabled { get; set; }

    public bool ReportPlayerEnabled { get; set; }

    public bool LuckyDrawEnabled { get; set; }

    public bool ContinuousQuickOpenEnabled { get; set; }

    public bool LegacyCardValueCacheEnabled { get; set; }

    public bool BattlenetBillingFlowDisableOverride { get; set; }

    public string BattlegroundsLuckyDrawDisabledCountryCode { get; set; }

    public bool SkippableTutorialEnabled { get; set; }

    public bool TracerouteEnabled { get; set; }

    public NetCacheFeatures()
    {
      this.Misc = new NetCache.NetCacheFeatures.CacheMisc();
      this.Games = new NetCache.NetCacheFeatures.CacheGames();
      this.Collection = new NetCache.NetCacheFeatures.CacheCollection();
      this.Store = new NetCache.NetCacheFeatures.CacheStore();
      this.Heroes = new NetCache.NetCacheFeatures.CacheHeroes();
      this.Mercenaries = new NetCache.NetCacheFeatures.CacheMercenaries();
      this.Traceroute = new NetCache.NetCacheFeatures.CacheTraceroute();
    }

    public class CacheMisc
    {
      public int ClientOptionsUpdateIntervalSeconds { get; set; }

      public bool AllowLiveFPSGathering { get; set; }
    }

    public class CacheGames
    {
      public bool Tournament { get; set; }

      public bool Practice { get; set; }

      public bool Casual { get; set; }

      public bool Forge { get; set; }

      public bool Friendly { get; set; }

      public bool TavernBrawl { get; set; }

      public bool Battlegrounds { get; set; }

      public bool BattlegroundsFriendlyChallenge { get; set; }

      public bool BattlegroundsTutorial { get; set; }

      public int ShowUserUI { get; set; }

      public bool Duels { get; set; }

      public bool PaidDuels { get; set; }

      public bool Mercenaries { get; set; }

      public bool MercenariesAI { get; set; }

      public bool MercenariesCoOp { get; set; }

      public bool MercenariesFriendly { get; set; }

      public bool GetFeatureFlag(
        NetCache.NetCacheFeatures.CacheGames.FeatureFlags flag)
      {
        switch (flag)
        {
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Tournament:
            return this.Tournament;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Practice:
            return this.Practice;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Casual:
            return this.Casual;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Forge:
            return this.Forge;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Friendly:
            return this.Friendly;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.TavernBrawl:
            return this.TavernBrawl;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Battlegrounds:
            return this.Battlegrounds;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.BattlegroundsFriendlyChallenge:
            return this.BattlegroundsFriendlyChallenge;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.BattlegroundsTutorial:
            return this.BattlegroundsTutorial;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Duels:
            return this.Duels;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.PaidDuels:
            return this.PaidDuels;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Mercenaries:
            return this.Mercenaries;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.MercenariesAI:
            return this.MercenariesAI;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.MercenariesCoOp:
            return this.MercenariesCoOp;
          case NetCache.NetCacheFeatures.CacheGames.FeatureFlags.MercenariesFriendly:
            return this.MercenariesFriendly;
          default:
            return false;
        }
      }

      public enum FeatureFlags
      {
        Invalid,
        Tournament,
        Practice,
        Casual,
        Forge,
        Friendly,
        TavernBrawl,
        Battlegrounds,
        BattlegroundsFriendlyChallenge,
        BattlegroundsTutorial,
        Duels,
        PaidDuels,
        Mercenaries,
        MercenariesAI,
        MercenariesCoOp,
        MercenariesFriendly,
      }
    }

    public class CacheCollection
    {
      public bool Manager { get; set; }

      public bool Crafting
      {
        set => this.\u003CCrafting\u003Ek__BackingField = value;
      }

      public bool DeckReordering { get; set; }

      public bool MultipleFavoriteCardBacks { get; set; }
    }

    public class CacheStore
    {
      public bool Store { get; set; }

      public bool BattlePay { get; set; }

      public bool BuyWithGold { get; set; }

      public bool SimpleCheckout { get; set; }

      public bool SoftAccountPurchasing { get; set; }

      public bool VirtualCurrencyEnabled { get; set; }

      public int NumClassicPacksUntilDeprioritize { get; set; }

      public bool SimpleCheckoutIOS { get; set; }

      public bool SimpleCheckoutAndroidAmazon { get; set; }

      public bool SimpleCheckoutAndroidGoogle { get; set; }

      public bool SimpleCheckoutAndroidGlobal { get; set; }

      public bool SimpleCheckoutWin { get; set; }

      public bool SimpleCheckoutMac { get; set; }

      public int BoosterRotatingSoonWarnDaysWithoutSale { get; set; }

      public int BoosterRotatingSoonWarnDaysWithSale { get; set; }

      public bool VintageStore { get; set; }

      public bool BuyCardBacksFromCollectionManager { get; set; }

      public bool BuyHeroSkinsFromCollectionManager { get; set; }

      public bool LargeItemBundleDetailsEnabled { get; set; }
    }

    public class CacheHeroes
    {
      public bool Hunter
      {
        set => this.\u003CHunter\u003Ek__BackingField = value;
      }

      public bool Mage
      {
        set => this.\u003CMage\u003Ek__BackingField = value;
      }

      public bool Paladin
      {
        set => this.\u003CPaladin\u003Ek__BackingField = value;
      }

      public bool Priest
      {
        set => this.\u003CPriest\u003Ek__BackingField = value;
      }

      public bool Rogue
      {
        set => this.\u003CRogue\u003Ek__BackingField = value;
      }

      public bool Shaman
      {
        set => this.\u003CShaman\u003Ek__BackingField = value;
      }

      public bool Warlock
      {
        set => this.\u003CWarlock\u003Ek__BackingField = value;
      }

      public bool Warrior
      {
        set => this.\u003CWarrior\u003Ek__BackingField = value;
      }
    }

    public class CacheMercenaries
    {
      public int FullyUpgradedStatBoostAttack { get; set; }

      public int FullyUpgradedStatBoostHealth { get; set; }
    }

    public class CacheTraceroute
    {
      public int MaxHops { get; set; }

      public int MessageSize { get; set; }

      public int MaxRetries { get; set; }

      public int TimeoutMs { get; set; }

      public bool ResolveHost { get; set; }
    }

    public class Defaults
    {
      public static readonly float TutorialPreviewVideosTimeout = 7f;
    }
  }

  public class NetCacheArcaneDustBalance
  {
    public long Balance { get; set; }
  }

  public class NetCacheGoldBalance
  {
    public long CappedBalance { get; set; }

    public long BonusBalance { get; set; }

    public long GetTotal() => this.CappedBalance + this.BonusBalance;
  }

  public class NetCacheRenownBalance
  {
    public long Balance { get; set; }
  }

  public class NetPlayerArenaTickets
  {
    public int Balance { get; set; }
  }

  public class HeroLevel
  {
    public TAG_CLASS Class { get; set; }

    public NetCache.HeroLevel.LevelInfo PrevLevel { get; set; }

    public NetCache.HeroLevel.LevelInfo CurrentLevel { get; set; }

    public HeroLevel()
    {
      this.Class = TAG_CLASS.INVALID;
      this.PrevLevel = (NetCache.HeroLevel.LevelInfo) null;
      this.CurrentLevel = new NetCache.HeroLevel.LevelInfo();
    }

    public override string ToString() => string.Format("[HeroLevel: Class={0}, PrevLevel={1}, CurrentLevel={2}]", (object) this.Class, (object) this.PrevLevel, (object) this.CurrentLevel);

    public class LevelInfo
    {
      public int Level { get; set; }

      public int MaxLevel { get; set; }

      public long XP { get; set; }

      public long MaxXP { get; set; }

      public LevelInfo()
      {
        this.Level = 0;
        this.MaxLevel = 60;
        this.XP = 0L;
        this.MaxXP = 0L;
      }

      public bool IsMaxLevel() => this.Level == this.MaxLevel;

      public override string ToString() => string.Format("[LevelInfo: Level={0}, XP={1}, MaxXP={2}]", (object) this.Level, (object) this.XP, (object) this.MaxXP);
    }
  }

  public class NetCacheHeroLevels
  {
    public NetCacheHeroLevels() => this.Levels = new List<NetCache.HeroLevel>();

    public override string ToString()
    {
      string str = "[START NetCacheHeroLevels]\n";
      foreach (NetCache.HeroLevel level in this.Levels)
        str += string.Format("{0}\n", (object) level);
      return str + "[END NetCacheHeroLevels]";
    }

    public List<NetCache.HeroLevel> Levels { get; set; }
  }

  public class NetCacheProfileProgress
  {
    public TutorialProgress CampaignProgress { get; set; }

    public int BestForgeWins { get; set; }

    public long LastForgeDate { get; set; }
  }

  public class NetCacheDisplayBanner
  {
    public int Id { get; set; }
  }

  public class NetCacheCardBacks
  {
    public NetCacheCardBacks()
    {
      this.FavoriteCardBacks = new HashSet<int>();
      this.CardBacks = new HashSet<int>();
    }

    public HashSet<int> FavoriteCardBacks { get; set; }

    public HashSet<int> CardBacks { get; set; }
  }

  public class NetCacheCoins
  {
    public NetCacheCoins() => this.Coins = new HashSet<int>();

    public int FavoriteCoin { get; set; }

    public HashSet<int> Coins { get; set; }
  }

  public class BoosterStack
  {
    public int Id { get; set; }

    public int Count { get; set; }

    public int EverGrantedCount { get; set; }
  }

  public class NetCacheBoosters
  {
    public NetCacheBoosters() => this.BoosterStacks = new List<NetCache.BoosterStack>();

    public List<NetCache.BoosterStack> BoosterStacks { get; set; }

    public NetCache.BoosterStack GetBoosterStack(int id) => this.BoosterStacks.Find((Predicate<NetCache.BoosterStack>) (obj => obj.Id == id));

    public int GetTotalNumBoosters()
    {
      int totalNumBoosters = 0;
      foreach (NetCache.BoosterStack boosterStack in this.BoosterStacks)
        totalNumBoosters += boosterStack.Count;
      return totalNumBoosters;
    }
  }

  public class DeckHeader
  {
    public long ID { get; set; }

    public string Name { get; set; }

    public int? CardBack { get; set; }

    public string Hero { get; set; }

    public string UIHeroOverride { get; set; }

    public TAG_PREMIUM UIHeroOverridePremium { get; set; }

    public string HeroPower { get; set; }

    public DeckType Type { get; set; }

    public bool HeroOverridden { get; set; }

    public bool RandomHeroUseFavorite { get; set; }

    public int SeasonId { get; set; }

    public int BrawlLibraryItemId { get; set; }

    public bool NeedsName { get; set; }

    public long SortOrder { get; set; }

    public RuneType Rune1 { get; set; }

    public RuneType Rune2 { get; set; }

    public RuneType Rune3 { get; set; }

    public PegasusShared.FormatType FormatType { get; set; }

    public bool Locked { get; set; }

    public DeckSourceType SourceType { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? LastModified { get; set; }

    public override string ToString() => string.Format("[DeckHeader: ID={0} Name={1} Hero={2} HeroPower={3} DeckType={4} ", (object) this.ID, (object) this.Name, (object) this.Hero, (object) this.HeroPower, (object) this.Type) + string.Format("CardBack={0} HeroOverridden={1} RandomHeroUseFavorite={2}", (object) this.CardBack, (object) this.HeroOverridden, (object) this.RandomHeroUseFavorite) + string.Format("NeedsName={0} SortOrder={1} SourceType={2} Rune1={3} Rune2={4} Rune3={5}", (object) this.NeedsName, (object) this.SortOrder, (object) this.SourceType, (object) this.Rune1, (object) this.Rune2, (object) this.Rune3);
  }

  public class NetCacheDecks
  {
    public NetCacheDecks() => this.Decks = new List<NetCache.DeckHeader>();

    public List<NetCache.DeckHeader> Decks { get; set; }
  }

  public class CardDefinition
  {
    public override bool Equals(object obj) => obj is NetCache.CardDefinition cardDefinition && this.Premium == cardDefinition.Premium && this.Name.Equals(cardDefinition.Name);

    public override int GetHashCode() => (int) (this.Name.GetHashCode() + this.Premium);

    public override string ToString() => string.Format("[CardDefinition: Name={0}, Premium={1}]", (object) this.Name, (object) this.Premium);

    public string Name { get; set; }

    public TAG_PREMIUM Premium { get; set; }
  }

  public class CardValue
  {
    public int BaseBuyValue { get; set; }

    public int BaseSellValue { get; set; }

    public int BaseUpgradeValue { get; set; }

    public int BuyValueOverride { get; set; }

    public int SellValueOverride { get; set; }

    public SpecialEventType OverrideEvent { get; set; }

    public int GetBuyValue() => !this.IsOverrideActive() ? this.BaseBuyValue : this.BuyValueOverride;

    public int GetSellValue() => !this.IsOverrideActive() ? this.BaseSellValue : this.SellValueOverride;

    public int GetUpgradeValue() => this.BaseUpgradeValue;

    public bool IsOverrideActive() => SpecialEventManager.Get().IsEventActive(this.OverrideEvent, false);
  }

  public class NetCacheCardValues
  {
    public Dictionary<NetCache.CardDefinition, NetCache.CardValue> Values { get; set; }

    public NetCacheCardValues() => this.Values = new Dictionary<NetCache.CardDefinition, NetCache.CardValue>();

    public NetCacheCardValues(int initialSize) => this.Values = new Dictionary<NetCache.CardDefinition, NetCache.CardValue>(initialSize);
  }

  public class NetCacheDisconnectedGame
  {
    public GameServerInfo ServerInfo { get; set; }

    public PegasusShared.GameType GameType { get; set; }

    public PegasusShared.FormatType FormatType { get; set; }

    public bool LoadGameState { get; set; }
  }

  public class BoosterCard
  {
    public NetCache.CardDefinition Def { get; set; }

    public long Date
    {
      set => this.\u003CDate\u003Ek__BackingField = value;
    }

    public BoosterCard() => this.Def = new NetCache.CardDefinition();
  }

  public class CardStack
  {
    public NetCache.CardDefinition Def { get; set; }

    public long Date { get; set; }

    public int Count { get; set; }

    public int NumSeen { get; set; }

    public CardStack() => this.Def = new NetCache.CardDefinition();
  }

  public class NetCacheCollection
  {
    public int TotalCardsOwned;
    public Map<TAG_CLASS, HashSet<string>> CoreCardsUnlockedPerClass = new Map<TAG_CLASS, HashSet<string>>();

    public NetCacheCollection()
    {
      this.Stacks = new List<NetCache.CardStack>();
      foreach (TAG_CLASS key in Enum.GetValues(typeof (TAG_CLASS)))
        this.CoreCardsUnlockedPerClass[key] = new HashSet<string>();
    }

    public List<NetCache.CardStack> Stacks { get; set; }
  }

  public class PlayerRecord
  {
    public PegasusShared.GameType RecordType { get; set; }

    public int Data { get; set; }

    public int Wins { get; set; }

    public int Losses
    {
      set => this.\u003CLosses\u003Ek__BackingField = value;
    }

    public int Ties
    {
      set => this.\u003CTies\u003Ek__BackingField = value;
    }
  }

  public class NetCachePlayerRecords
  {
    public NetCachePlayerRecords() => this.Records = new List<NetCache.PlayerRecord>();

    public List<NetCache.PlayerRecord> Records { get; set; }
  }

  public class NetCacheRewardProgress
  {
    public int Season { get; set; }

    public long SeasonEndDate { get; set; }

    public long NextQuestCancelDate { get; set; }
  }

  public class NetCacheMedalInfo
  {
    public Map<PegasusShared.FormatType, MedalInfoData> MedalData = new Map<PegasusShared.FormatType, MedalInfoData>();
    private static Map<PegasusShared.FormatType, int> m_cheatLocalOverrideStarLevelData = new Map<PegasusShared.FormatType, int>();
    private static Map<PegasusShared.FormatType, int> m_cheatLocalOverrideLegendRankData = new Map<PegasusShared.FormatType, int>();

    public NetCache.NetCacheMedalInfo PreviousMedalInfo { get; set; }

    public NetCacheMedalInfo()
    {
    }

    public NetCacheMedalInfo(MedalInfo packet)
    {
      foreach (MedalInfoData medalInfoData in packet.MedalData)
        this.MedalData.Add(medalInfoData.FormatType, medalInfoData);
      foreach (KeyValuePair<PegasusShared.FormatType, int> keyValuePair in NetCache.NetCacheMedalInfo.m_cheatLocalOverrideStarLevelData)
        this.MedalData[keyValuePair.Key].StarLevel = keyValuePair.Value;
      foreach (KeyValuePair<PegasusShared.FormatType, int> keyValuePair in NetCache.NetCacheMedalInfo.m_cheatLocalOverrideLegendRankData)
        this.MedalData[keyValuePair.Key].LegendRank = keyValuePair.Value;
    }

    public NetCache.NetCacheMedalInfo Clone()
    {
      NetCache.NetCacheMedalInfo netCacheMedalInfo = new NetCache.NetCacheMedalInfo();
      foreach (KeyValuePair<PegasusShared.FormatType, MedalInfoData> keyValuePair in this.MedalData)
        netCacheMedalInfo.MedalData.Add(keyValuePair.Key, NetCache.NetCacheMedalInfo.CloneMedalInfoData(keyValuePair.Value));
      return netCacheMedalInfo;
    }

    public MedalInfoData GetMedalInfoData(PegasusShared.FormatType formatType)
    {
      MedalInfoData medalInfoData;
      if (!this.MedalData.TryGetValue(formatType, out medalInfoData))
        Debug.LogError((object) ("NetCacheMedalInfo.GetMedalInfoData failed to find data for the format type " + formatType.ToString() + ". Returning null"));
      return medalInfoData;
    }

    public void CheatLocalOverrideStarLevel(PegasusShared.FormatType formatType, int starLevel)
    {
      NetCache.NetCacheMedalInfo.m_cheatLocalOverrideStarLevelData[formatType] = starLevel;
      this.MedalData[formatType].StarLevel = starLevel;
    }

    public void CheatLocalOverrideLegendRank(PegasusShared.FormatType formatType, int legendRank)
    {
      NetCache.NetCacheMedalInfo.m_cheatLocalOverrideLegendRankData[formatType] = legendRank;
      this.MedalData[formatType].LegendRank = legendRank;
    }

    public static void CheatLocalOverrideClear()
    {
      NetCache.NetCacheMedalInfo.m_cheatLocalOverrideStarLevelData.Clear();
      NetCache.NetCacheMedalInfo.m_cheatLocalOverrideLegendRankData.Clear();
    }

    public static MedalInfoData CloneMedalInfoData(MedalInfoData original)
    {
      MedalInfoData medalInfoData = new MedalInfoData();
      medalInfoData.LeagueId = original.LeagueId;
      medalInfoData.SeasonWins = original.SeasonWins;
      medalInfoData.Stars = original.Stars;
      medalInfoData.Streak = original.Streak;
      medalInfoData.StarLevel = original.StarLevel;
      medalInfoData.HasLegendRank = original.HasLegendRank;
      medalInfoData.LegendRank = original.LegendRank;
      medalInfoData.HasBestStarLevel = original.HasBestStarLevel;
      medalInfoData.BestStarLevel = original.BestStarLevel;
      medalInfoData.HasSeasonGames = original.HasSeasonGames;
      medalInfoData.SeasonGames = original.SeasonGames;
      medalInfoData.StarsPerWin = original.StarsPerWin;
      if (original.HasRatingId)
        medalInfoData.RatingId = original.RatingId;
      if (original.HasSeasonId)
        medalInfoData.SeasonId = original.SeasonId;
      if (original.HasRating)
        medalInfoData.Rating = original.Rating;
      if (original.HasVariance)
        medalInfoData.Variance = original.Variance;
      if (original.HasBestStars)
        medalInfoData.BestStars = original.BestStars;
      if (original.HasBestEverLeagueId)
        medalInfoData.BestEverLeagueId = original.BestEverLeagueId;
      if (original.HasBestEverStarLevel)
        medalInfoData.BestEverStarLevel = original.BestEverStarLevel;
      if (original.HasBestRating)
        medalInfoData.BestRating = original.BestRating;
      if (original.HasPublicRating)
        medalInfoData.PublicRating = original.PublicRating;
      if (original.HasFormatType)
        medalInfoData.FormatType = original.FormatType;
      return medalInfoData;
    }

    public override string ToString() => string.Format("[NetCacheMedalInfo] \n MedalData={0}", (object) this.MedalData.ToString());
  }

  public class NetCacheBaconRatingInfo
  {
    public int Rating { get; set; }

    public override string ToString() => string.Format("[NetCacheBaconRatingInfo] \n Rating={0}", (object) this.Rating);
  }

  public class NetCachePVPDRStatsInfo
  {
    public int Rating { get; set; }

    public int PaidRating { get; set; }

    public int HighWatermark { get; set; }

    public override string ToString() => string.Format("[NetCachePVPDRStatsInfo] \n Rating={0} PaidRating={1} HighWatermark={2}", (object) this.Rating, (object) this.PaidRating, (object) this.HighWatermark);
  }

  public class NetCacheMercenariesPlayerInfo
  {
    public Dictionary<MercenaryBuilding.Mercenarybuildingtype, bool> BuildingEnabledMap;
    public List<int> DisabledMercenaryList;
    public HashSet<int> DisabledVisitorList;

    public int PvpRating { get; set; }

    public uint PvpRewardChestWinsProgress { get; set; }

    public uint PvpRewardChestWinsRequired { get; set; }

    public Dictionary<int, NetCache.NetCacheMercenariesPlayerInfo.BountyInfo> BountyInfoMap { get; set; }

    public int PvpSeasonHighestRating { get; set; }

    public int PvpSeasonId { get; set; }

    public override string ToString() => string.Format("[NetCacheMercenariesPlayerInfo] \n PvpRating={0}, PvpRewardChestWinsProgress={1}, PvpRewardChestWinsRequired={2}", (object) this.PvpRating, (object) this.PvpRewardChestWinsProgress, (object) this.PvpRewardChestWinsRequired);

    public class BountyInfo
    {
      public int FewestTurns { get; set; }

      public int Completions { get; set; }

      public bool IsComplete { get; set; }

      public bool IsAcknowledged { get; set; }
    }
  }

  public class NetCacheMercenariesVillageInfo
  {
    private readonly List<int> m_emptyTierList = new List<int>();
    private Dictionary<int, List<int>> m_tierTreeCache = new Dictionary<int, List<int>>();
    private Dictionary<int, int> m_unbuiltTierLookup = new Dictionary<int, int>();
    private Dictionary<TAG_RARITY, int> m_renownConversionLookup = new Dictionary<TAG_RARITY, int>();

    public bool Initialized { get; set; }

    public List<MercenariesBuildingState> BuildingStates { get; set; }

    public List<MercenariesBuildingState> LastBuildingUpdate
    {
      set => this.\u003CLastBuildingUpdate\u003Ek__BackingField = value;
    }

    public List<MercenariesRenownConvertRate> ConversionRates { get; private set; }

    public int UnlockedBountyDifficultyLevel { get; private set; }

    public void TrySetDifficultyUnlock(MercenariesBuildingState bldgState)
    {
      if (GameDbf.MercenaryBuilding.GetRecord(bldgState.BuildingId).MercenaryBuildingType != MercenaryBuilding.Mercenarybuildingtype.PVEZONES)
        return;
      foreach (TierPropertiesDbfRecord buildingTierProperty in GameDbf.BuildingTier.GetRecord(bldgState.CurrentTierId).MercenaryBuildingTierProperties)
      {
        if (buildingTierProperty.TierPropertyType == TierProperties.Buildingtierproperty.PVEMODE)
        {
          this.UnlockedBountyDifficultyLevel = buildingTierProperty.TierPropertyValue;
          break;
        }
      }
    }

    public List<int> GetNextTierListByTierId(int tierId)
    {
      List<int> intList;
      return this.m_tierTreeCache.TryGetValue(tierId, out intList) ? intList : this.m_emptyTierList;
    }

    public bool BuildingIsBuilt(MercenariesBuildingState bldgState)
    {
      int num;
      return this.m_unbuiltTierLookup.TryGetValue(bldgState.BuildingId, out num) && bldgState.CurrentTierId != num;
    }

    public void CacheTierTree()
    {
      if (this.m_tierTreeCache.Count > 0)
        this.m_tierTreeCache.Clear();
      foreach (MercenaryBuildingDbfRecord record1 in GameDbf.MercenaryBuilding.GetRecords())
      {
        MercenaryBuildingDbfRecord bldg = record1;
        BuildingTierDbfRecord record2 = GameDbf.BuildingTier.GetRecord((Predicate<BuildingTierDbfRecord>) (r => r.MercenaryBuildingId == bldg.ID));
        this.m_unbuiltTierLookup.Add(bldg.ID, record2.ID);
        this.AddTierToTierTreeCache(bldg.DefaultTier);
      }
    }

    private void AddTierToTierTreeCache(int tierId)
    {
      if (this.m_tierTreeCache.ContainsKey(tierId))
        return;
      List<int> intList = new List<int>();
      this.m_tierTreeCache.Add(tierId, intList);
      List<NextTiersDbfRecord> records = GameDbf.NextTiers.GetRecords((Predicate<NextTiersDbfRecord>) (r => r.BuildingTierId == tierId));
      if (records == null || records.Count == 0)
        return;
      foreach (NextTiersDbfRecord nextTiersDbfRecord in records)
      {
        intList.Add(nextTiersDbfRecord.NextTierId);
        this.AddTierToTierTreeCache(nextTiersDbfRecord.NextTierId);
      }
    }

    public void CacheRenownConversionRates(List<MercenariesRenownConvertRate> conversionRates)
    {
      this.ConversionRates = conversionRates;
      this.m_renownConversionLookup.Clear();
      if (this.ConversionRates == null || this.ConversionRates.Count == 0)
        return;
      foreach (MercenariesRenownConvertRate conversionRate in conversionRates)
      {
        TAG_RARITY coinRarityId = (TAG_RARITY) conversionRate.CoinRarityId;
        if (this.m_renownConversionLookup.ContainsKey(coinRarityId))
          Log.Lettuce.PrintError(string.Format("Duplicate rarity {0} in renown conversion rates - Skipping value", (object) coinRarityId));
        else if (conversionRate.CoinConversionRate > 0U)
          this.m_renownConversionLookup[coinRarityId] = (int) conversionRate.CoinConversionRate;
      }
    }

    public bool TryGetRenownRate(TAG_RARITY rarity, out int conversionRate) => this.m_renownConversionLookup.TryGetValue(rarity, out conversionRate);
  }

  public class NetCacheMercenariesVillageVisitorInfo
  {
    public List<MercenariesVisitorState> VisitorStates { get; set; }

    public int[] VisitingMercenaries { get; set; }

    public List<MercenariesTaskState> CompletedTasks { get; set; }

    public List<MercenariesCompletedVisitorState> CompletedVisitorStates { get; set; }

    public List<MercenariesRenownOfferData> ActiveRenownOffers { get; set; }
  }

  public abstract class ProfileNotice
  {
    private NetCache.ProfileNotice.NoticeType m_type;

    protected ProfileNotice(NetCache.ProfileNotice.NoticeType init)
    {
      this.m_type = init;
      this.NoticeID = 0L;
      this.Origin = NetCache.ProfileNotice.NoticeOrigin.UNKNOWN;
      this.OriginData = 0L;
      this.Date = 0L;
    }

    public long NoticeID { get; set; }

    public NetCache.ProfileNotice.NoticeType Type => this.m_type;

    public NetCache.ProfileNotice.NoticeOrigin Origin { get; set; }

    public long OriginData { get; set; }

    public long Date { get; set; }

    public override string ToString() => string.Format("[{0}: NoticeID={1}, Type={2}, Origin={3}, OriginData={4}, Date={5}]", (object) this.GetType(), (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date);

    public enum NoticeType
    {
      GAINED_MEDAL = 1,
      REWARD_BOOSTER = 2,
      REWARD_CARD = 3,
      DISCONNECTED_GAME = 4,
      PRECON_DECK = 5,
      REWARD_DUST = 6,
      REWARD_MOUNT = 7,
      REWARD_FORGE = 8,
      REWARD_CURRENCY = 9,
      PURCHASE = 10, // 0x0000000A
      REWARD_CARD_BACK = 11, // 0x0000000B
      BONUS_STARS = 12, // 0x0000000C
      ADVENTURE_PROGRESS = 14, // 0x0000000E
      HERO_LEVEL_UP = 15, // 0x0000000F
      ACCOUNT_LICENSE = 16, // 0x00000010
      TAVERN_BRAWL_REWARDS = 17, // 0x00000011
      TAVERN_BRAWL_TICKET = 18, // 0x00000012
      EVENT = 19, // 0x00000013
      GENERIC_REWARD_CHEST = 20, // 0x00000014
      LEAGUE_PROMOTION_REWARDS = 21, // 0x00000015
      CARD_REPLACEMENT = 22, // 0x00000016
      DISCONNECTED_GAME_NEW = 23, // 0x00000017
      DECK_REMOVED = 25, // 0x00000019
      DECK_GRANTED = 26, // 0x0000001A
      MINI_SET_GRANTED = 27, // 0x0000001B
      SELLABLE_DECK_GRANTED = 28, // 0x0000001C
      REWARD_BATTLEGROUNDS_GUIDE = 29, // 0x0000001D
      REWARD_BATTLEGROUNDS_HERO = 30, // 0x0000001E
      MERCENARIES_REWARDS_CURRENCY = 31, // 0x0000001F
      MERCENARIES_REWARDS_EXPERIENCE = 32, // 0x00000020
      MERCENARIES_REWARDS_EQUIPMENT = 33, // 0x00000021
      MERCENARIES_REWARDS = 34, // 0x00000022
      MERCENARIES_ABILITY_UNLOCK = 35, // 0x00000023
      MERCENARIES_MERC_FULL_UPGRADE = 36, // 0x00000024
      MERCENARIES_MERC_LICENSE = 37, // 0x00000025
      MERCENARIES_CURRENCY_LICENSE = 38, // 0x00000026
      MERCENARIES_BOOSTER_LICENSE = 39, // 0x00000027
      MERCENARIES_RANDOM_REWARD_LICENSE = 40, // 0x00000028
      MERCENARIES_SEASON_ROLL = 41, // 0x00000029
      MERCENARIES_SEASON_REWARDS = 42, // 0x0000002A
      MERCENARIES_ZONE_UNLOCK = 43, // 0x0000002B
      REWARD_BATTLEGROUNDS_BOARD_SKIN = 44, // 0x0000002C
      REWARD_BATTLEGROUNDS_FINISHER = 45, // 0x0000002D
      REWARD_BATTLEGROUNDS_EMOTE = 46, // 0x0000002E
      REWARD_LUCKY_DRAW = 47, // 0x0000002F
      REDUNDANT_NDE_REROLL = 48, // 0x00000030
      REDUNDANT_NDE_REROLL_RESULT = 49, // 0x00000031
    }

    public enum NoticeOrigin
    {
      UNKNOWN = -1, // 0xFFFFFFFF
      SEASON = 1,
      BETA_REIMBURSE = 2,
      FORGE = 3,
      TOURNEY = 4,
      PRECON_DECK = 5,
      ACK = 6,
      ACHIEVEMENT = 7,
      LEVEL_UP = 8,
      PURCHASE_COMPLETE = 10, // 0x0000000A
      PURCHASE_FAILED = 11, // 0x0000000B
      PURCHASE_CANCELED = 12, // 0x0000000C
      BLIZZCON = 13, // 0x0000000D
      EVENT = 14, // 0x0000000E
      DISCONNECTED_GAME = 15, // 0x0000000F
      OUT_OF_BAND_LICENSE = 16, // 0x00000010
      IGR = 17, // 0x00000011
      ADVENTURE_PROGRESS = 18, // 0x00000012
      ADVENTURE_FLAGS = 19, // 0x00000013
      TAVERN_BRAWL_REWARD = 20, // 0x00000014
      ACCOUNT_LICENSE_FLAGS = 21, // 0x00000015
      FROM_PURCHASE = 22, // 0x00000016
      HOF_COMPENSATION = 23, // 0x00000017
      GENERIC_REWARD_CHEST_ACHIEVE = 24, // 0x00000018
      GENERIC_REWARD_CHEST = 25, // 0x00000019
      LEAGUE_PROMOTION = 26, // 0x0000001A
      CARD_REPLACEMENT = 27, // 0x0000001B
      NOTICE_ORIGIN_LEVEL_UP_MULTIPLE = 28, // 0x0000001C
      NOTICE_ORIGIN_DUELS = 29, // 0x0000001D
      NOTICE_ORIGIN_MERCENARIES = 30, // 0x0000001E
      NOTICE_ORIGIN_LUCKY_DRAW = 31, // 0x0000001F
      NOTICE_ORIGIN_NDE_REDUNDANT_REROLL = 32, // 0x00000020
    }
  }

  public class ProfileNoticeMedal : NetCache.ProfileNotice
  {
    public ProfileNoticeMedal()
      : base(NetCache.ProfileNotice.NoticeType.GAINED_MEDAL)
    {
    }

    public int LeagueId { get; set; }

    public int StarLevel { get; set; }

    public int LegendRank { get; set; }

    public int BestStarLevel { get; set; }

    public PegasusShared.FormatType FormatType { get; set; }

    public Network.RewardChest Chest { get; set; }

    public bool WasLimitedByBestEverStarLevel { get; set; }

    public override string ToString() => string.Format("{0} [LeagueId={1} StarLevel={2}, LegendRank={3}, BestStarLevel={4}, FormatType={5}, Chest={6}, WasLimitedByBestEverStarLevel={7}]", (object) base.ToString(), (object) this.LeagueId, (object) this.StarLevel, (object) this.LegendRank, (object) this.BestStarLevel, (object) this.FormatType, (object) this.Chest, (object) this.WasLimitedByBestEverStarLevel);
  }

  public class ProfileNoticeRewardBooster : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardBooster()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_BOOSTER)
    {
      this.Id = 0;
      this.Count = 0;
    }

    public int Id { get; set; }

    public int Count { get; set; }

    public override string ToString() => string.Format("{0} [Id={1}, Count={2}]", (object) base.ToString(), (object) this.Id, (object) this.Count);
  }

  public class ProfileNoticeRewardCard : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardCard()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_CARD)
    {
    }

    public string CardID { get; set; }

    public TAG_PREMIUM Premium { get; set; }

    public int Quantity { get; set; }

    public override string ToString() => string.Format("{0} [CardID={1}, Premium={2}, Quantity={3}]", (object) base.ToString(), (object) this.CardID, (object) this.Premium, (object) this.Quantity);
  }

  public class ProfileNoticeRewardBattlegroundsGuideSkin : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardBattlegroundsGuideSkin()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_GUIDE)
    {
    }

    public string CardID { get; set; }

    public int FixedRewardMapID { get; set; }

    public override string ToString() => string.Format("{0}", (object) base.ToString(), (object) this.CardID);
  }

  public class ProfileNoticeRewardBattlegroundsHeroSkin : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardBattlegroundsHeroSkin()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_HERO)
    {
    }

    public string CardID { get; set; }

    public int FixedRewardMapID { get; set; }

    public override string ToString() => string.Format("{0}", (object) base.ToString(), (object) this.CardID);
  }

  public class ProfileNoticePreconDeck : NetCache.ProfileNotice
  {
    public ProfileNoticePreconDeck()
      : base(NetCache.ProfileNotice.NoticeType.PRECON_DECK)
    {
    }

    public long DeckID { get; set; }

    public int HeroAsset { get; set; }

    public override string ToString() => string.Format("{0} [DeckID={1}, HeroAsset={2}]", (object) base.ToString(), (object) this.DeckID, (object) this.HeroAsset);
  }

  public class ProfileNoticeDeckRemoved : NetCache.ProfileNotice
  {
    public ProfileNoticeDeckRemoved()
      : base(NetCache.ProfileNotice.NoticeType.DECK_REMOVED)
    {
    }

    public long DeckID { get; set; }

    public override string ToString() => string.Format("{0} [DeckID={1}]", (object) base.ToString(), (object) this.DeckID);
  }

  public class ProfileNoticeDeckGranted : NetCache.ProfileNotice
  {
    public ProfileNoticeDeckGranted()
      : base(NetCache.ProfileNotice.NoticeType.DECK_GRANTED)
    {
    }

    public int DeckDbiID { get; set; }

    public int ClassId { get; set; }

    public long PlayerDeckID { get; set; }

    public override string ToString() => string.Format("{0} [DeckDbiID={1}, ClassId={2}]", (object) base.ToString(), (object) this.DeckDbiID, (object) this.ClassId);
  }

  public class ProfileNoticeMiniSetGranted : NetCache.ProfileNotice
  {
    public ProfileNoticeMiniSetGranted()
      : base(NetCache.ProfileNotice.NoticeType.MINI_SET_GRANTED)
    {
    }

    public int MiniSetID { get; set; }

    public int Premium { get; set; }

    public override string ToString() => string.Format("{0} [CardsRewardID={1}]", (object) base.ToString(), (object) this.MiniSetID);
  }

  public class ProfileNoticeSellableDeckGranted : NetCache.ProfileNotice
  {
    public ProfileNoticeSellableDeckGranted()
      : base(NetCache.ProfileNotice.NoticeType.SELLABLE_DECK_GRANTED)
    {
    }

    public int SellableDeckID { get; set; }

    public long PlayerDeckID { get; set; }

    public TAG_PREMIUM Premium { get; set; }

    public override string ToString() => string.Format("{0} [SellableDeckID={1}]", (object) base.ToString(), (object) this.SellableDeckID);
  }

  public class ProfileNoticeRewardDust : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardDust()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_DUST)
    {
    }

    public int Amount { get; set; }

    public override string ToString() => string.Format("{0} [Amount={1}]", (object) base.ToString(), (object) this.Amount);
  }

  public class ProfileNoticeRewardMount : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardMount()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_MOUNT)
    {
    }

    public int MountID { get; set; }

    public override string ToString() => string.Format("{0} [MountID={1}]", (object) base.ToString(), (object) this.MountID);
  }

  public class ProfileNoticeRewardForge : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardForge()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_FORGE)
    {
    }

    public int Quantity { get; set; }

    public override string ToString() => string.Format("{0} [Quantity={1}]", (object) base.ToString(), (object) this.Quantity);
  }

  public class ProfileNoticeRewardCurrency : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardCurrency()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_CURRENCY)
    {
    }

    public int Amount { get; set; }

    public PegasusShared.CurrencyType CurrencyType { get; set; }

    public override string ToString() => string.Format("{0} [CurrencyType={1}, Amount={2}]", (object) base.ToString(), (object) this.CurrencyType.ToString(), (object) this.Amount);
  }

  public class ProfileNoticePurchase : NetCache.ProfileNotice
  {
    public ProfileNoticePurchase()
      : base(NetCache.ProfileNotice.NoticeType.PURCHASE)
    {
    }

    public long? PMTProductID { get; set; }

    public string CurrencyCode { get; set; }

    public long Data { get; set; }

    public override string ToString() => string.Format("[ProfileNoticePurchase: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} PMTProductID='{5}', Data={6} Currency={7}]", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.PMTProductID, (object) this.Data, (object) this.CurrencyCode);
  }

  public class ProfileNoticeRewardCardBack : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardCardBack()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_CARD_BACK)
    {
    }

    public int CardBackID { get; set; }

    public override string ToString() => string.Format("[ProfileNoticePurchase: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} CardBackID={5}]", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.CardBackID);
  }

  public class ProfileNoticeBonusStars : NetCache.ProfileNotice
  {
    public ProfileNoticeBonusStars()
      : base(NetCache.ProfileNotice.NoticeType.BONUS_STARS)
    {
    }

    public int StarLevel { get; set; }

    public int Stars { get; set; }

    public override string ToString() => string.Format("{0} [StarLevel={1}, Stars={2}]", (object) base.ToString(), (object) this.StarLevel, (object) this.Stars);
  }

  public class ProfileNoticeEvent : NetCache.ProfileNotice
  {
    public int EventType { get; }
  }

  public class ProfileNoticeDisconnectedGame : NetCache.ProfileNotice
  {
    public ProfileNoticeDisconnectedGame()
      : base(NetCache.ProfileNotice.NoticeType.DISCONNECTED_GAME)
    {
    }

    public PegasusShared.GameType GameType { get; set; }

    public PegasusShared.FormatType FormatType { get; set; }

    public int MissionId { get; set; }

    public ProfileNoticeDisconnectedGameResult.GameResult GameResult { get; set; }

    public ProfileNoticeDisconnectedGameResult.PlayerResult YourResult { get; set; }

    public ProfileNoticeDisconnectedGameResult.PlayerResult OpponentResult { get; set; }

    public int PlayerIndex { get; set; }

    public override string ToString() => string.Format("{0} [GameType={1}, FormatType={2}, MissionId={3} GameResult={4}, YourResult={5}, OpponentResult={6}, PlayerIndex={7}]", (object) base.ToString(), (object) this.GameType, (object) this.FormatType, (object) this.MissionId, (object) this.GameResult, (object) this.YourResult, (object) this.OpponentResult, (object) this.PlayerIndex);
  }

  public class ProfileNoticeAdventureProgress : NetCache.ProfileNotice
  {
    public ProfileNoticeAdventureProgress()
      : base(NetCache.ProfileNotice.NoticeType.ADVENTURE_PROGRESS)
    {
    }

    public int Wing { get; set; }

    public int? Progress { get; set; }

    public ulong? Flags { get; set; }

    public override string ToString() => string.Format("{0} [Wing={1}, Progress={2}, Flags={3}]", (object) base.ToString(), (object) this.Wing, (object) this.Progress, (object) this.Flags);
  }

  public class ProfileNoticeLevelUp : NetCache.ProfileNotice
  {
    public ProfileNoticeLevelUp()
      : base(NetCache.ProfileNotice.NoticeType.HERO_LEVEL_UP)
    {
    }

    public int HeroClass { get; set; }

    public int NewLevel { get; set; }

    public int TotalLevel { get; set; }

    public override string ToString() => string.Format("{0} [HeroClass={1}, NewLevel={2}], TotalLevel={3}", (object) base.ToString(), (object) this.HeroClass, (object) this.NewLevel, (object) this.TotalLevel);
  }

  public class ProfileNoticeAcccountLicense : NetCache.ProfileNotice
  {
    public ProfileNoticeAcccountLicense()
      : base(NetCache.ProfileNotice.NoticeType.ACCOUNT_LICENSE)
    {
    }

    public long License { get; set; }

    public long CasID { get; set; }

    public override string ToString() => string.Format("{0} [License={1}, CasID={2}]", (object) base.ToString(), (object) this.License, (object) this.CasID);
  }

  public class ProfileNoticeTavernBrawlRewards : NetCache.ProfileNotice
  {
    public ProfileNoticeTavernBrawlRewards()
      : base(NetCache.ProfileNotice.NoticeType.TAVERN_BRAWL_REWARDS)
    {
    }

    public PegasusShared.RewardChest Chest { get; set; }

    public int Wins { get; set; }

    public TavernBrawlMode Mode { get; set; }

    public override string ToString() => string.Format("{0} [Chest={1}, Wins={2}, Mode={3}]", (object) base.ToString(), (object) this.Chest, (object) this.Wins, (object) this.Mode);
  }

  public class ProfileNoticeTavernBrawlTicket : NetCache.ProfileNotice
  {
    public ProfileNoticeTavernBrawlTicket()
      : base(NetCache.ProfileNotice.NoticeType.TAVERN_BRAWL_TICKET)
    {
    }

    public int TicketType
    {
      set => this.\u003CTicketType\u003Ek__BackingField = value;
    }

    public int Quantity
    {
      set => this.\u003CQuantity\u003Ek__BackingField = value;
    }
  }

  public class ProfileNoticeGenericRewardChest : NetCache.ProfileNotice
  {
    public ProfileNoticeGenericRewardChest()
      : base(NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST)
    {
    }

    public int RewardChestAssetId { get; set; }

    public PegasusShared.RewardChest RewardChest { get; set; }

    public uint RewardChestByteSize { get; set; }

    public byte[] RewardChestHash { get; set; }
  }

  public class NetCacheProfileNotices
  {
    public NetCacheProfileNotices() => this.Notices = new List<NetCache.ProfileNotice>();

    public List<NetCache.ProfileNotice> Notices { get; set; }
  }

  public class ProfileNoticeLeaguePromotionRewards : NetCache.ProfileNotice
  {
    public ProfileNoticeLeaguePromotionRewards()
      : base(NetCache.ProfileNotice.NoticeType.LEAGUE_PROMOTION_REWARDS)
    {
    }

    public PegasusShared.RewardChest Chest { get; set; }

    public int LeagueId { get; set; }

    public override string ToString() => string.Format("{0} [Chest={1}, LeagueId={2}]", (object) base.ToString(), (object) this.Chest, (object) this.LeagueId);
  }

  public class ProfileNoticeMercenariesRewards : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesRewards()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_REWARDS)
    {
    }

    public PegasusShared.ProfileNoticeMercenariesRewards.RewardType RewardType { get; set; }

    public PegasusShared.RewardChest Chest { get; set; }

    public override string ToString() => string.Format("{0} [Chest={1}]", (object) base.ToString(), (object) this.Chest);
  }

  public class ProfileNoticeMercenariesAbilityUnlock : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesAbilityUnlock()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_ABILITY_UNLOCK)
    {
    }

    public int MercenaryId { get; set; }

    public int AbilityId { get; set; }

    public override string ToString() => string.Format("[ProfileNoticeMercenariesAbilityUnlock: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} MercenaryId={5} AbilityId={6}]", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.MercenaryId, (object) this.AbilityId);
  }

  public class ProfileNoticeMercenariesZoneUnlock : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesZoneUnlock()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_ZONE_UNLOCK)
    {
    }

    public int ZoneId { get; set; }

    public override string ToString() => string.Format("[ProfileNoticeMercenariesZoneUnlock: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} ZoneId={5}]", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.ZoneId);
  }

  public class ProfileNoticeRewardBattlegroundsBoard : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardBattlegroundsBoard()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_BOARD_SKIN)
    {
    }

    public long BoardSkinID { get; set; }

    public int FixedRewardMapID { get; set; }

    public override string ToString() => string.Format("{0} [BoardSkinID={1}]", (object) base.ToString(), (object) this.BoardSkinID);
  }

  public class ProfileNoticeRewardBattlegroundsFinisher : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardBattlegroundsFinisher()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_FINISHER)
    {
    }

    public long FinisherID { get; set; }

    public int FixedRewardMapID { get; set; }

    public override string ToString() => string.Format("{0} [FinisherID={1}]", (object) base.ToString(), (object) this.FinisherID);
  }

  public class ProfileNoticeRewardBattlegroundsEmote : NetCache.ProfileNotice
  {
    public ProfileNoticeRewardBattlegroundsEmote()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_EMOTE)
    {
    }

    public long EmoteID { get; set; }

    public int FixedRewardMapID { get; set; }

    public override string ToString() => string.Format("{0} [EmoteID={1}]", (object) base.ToString(), (object) this.EmoteID);
  }

  public class ProfileNoticeMercenariesSeasonRoll : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesSeasonRoll()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_SEASON_ROLL)
    {
    }

    public int EndedSeasonId { get; set; }

    public int HighestSeasonRating { get; set; }
  }

  public class ProfileNoticeMercenariesBoosterLicense : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesBoosterLicense()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_BOOSTER_LICENSE)
    {
    }

    public int Count { get; set; }

    public override string ToString() => string.Format("[ProfileNoticeMercenariesBoosterLicense: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} Count={5}]", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.Count);
  }

  public class ProfileNoticeMercenariesCurrencyLicense : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesCurrencyLicense()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_CURRENCY_LICENSE)
    {
    }

    public int MercenaryId { get; set; }

    public long CurrencyAmount { get; set; }

    public override string ToString() => string.Format("[ProfileNoticeMercenariesBoosterLicense: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} MercenaryId={5} CurrencyAmount={6}]", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.MercenaryId, (object) this.CurrencyAmount);
  }

  public class ProfileNoticeMercenariesMercenaryLicense : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesMercenaryLicense()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_MERC_LICENSE)
    {
    }

    public int MercenaryId { get; set; }

    public int ArtVariationId { get; set; }

    public uint ArtVariationPremium { get; set; }

    public long CurrencyAmount { get; set; }

    public override string ToString() => string.Format("[ProfileNoticeMercenariesMercenaryLicense: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} MercenaryId={5}, ArtVariationId={6}, ArtVariationPremium={7} CurrencyAmount={8}]", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.MercenaryId, (object) this.ArtVariationId, (object) this.ArtVariationPremium, (object) this.CurrencyAmount);
  }

  public class ProfileNoticeMercenariesRandomRewardLicense : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesRandomRewardLicense()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_RANDOM_REWARD_LICENSE)
    {
    }

    public int MercenaryId { get; set; }

    public int ArtVariationId { get; set; }

    public uint ArtVariationPremium { get; set; }

    public long CurrencyAmount { get; set; }

    public bool IsConvertedMercenary { get; set; }

    public override string ToString() => string.Format("[ProfileNoticeMercenariesRandomRewardLicense: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} MercenaryId={5}, ArtVariationId={6}, ArtVariationPremium={7} CurrencyAmount={8}]", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.MercenaryId, (object) this.ArtVariationId, (object) this.ArtVariationPremium, (object) this.CurrencyAmount);
  }

  public class ProfileNoticeMercenariesMercenaryFullyUpgraded : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesMercenaryFullyUpgraded()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_MERC_FULL_UPGRADE)
    {
    }

    public int MercenaryId { get; set; }

    public override string ToString() => string.Format("[ProfileNoticeMercenariesAbilityUnlock: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} MercenaryId={5}", (object) this.NoticeID, (object) this.Type, (object) this.Origin, (object) this.OriginData, (object) this.Date, (object) this.MercenaryId);
  }

  public class ProfileNoticeMercenariesSeasonRewards : NetCache.ProfileNotice
  {
    public ProfileNoticeMercenariesSeasonRewards()
      : base(NetCache.ProfileNotice.NoticeType.MERCENARIES_SEASON_REWARDS)
    {
    }

    public PegasusShared.RewardChest Chest { get; set; }

    public int RewardAssetId { get; set; }

    public override string ToString() => string.Format("[Chest={0}, RewardAssetId={1}]", (object) this.Chest, (object) this.RewardAssetId);
  }

  public class ProfileNoticeLuckyDrawReward : NetCache.ProfileNotice
  {
    public ProfileNoticeLuckyDrawReward()
      : base(NetCache.ProfileNotice.NoticeType.REWARD_LUCKY_DRAW)
    {
    }

    public int LuckyDrawRewardId { get; set; }

    public PegasusShared.ProfileNoticeLuckyDrawReward.OriginType LuckyDrawOrigin { get; set; }

    public override string ToString() => string.Format("[ProfileNoticeLuckyDrawReward: LuckyDrawRewardAssetId={0}, LuckyDrawOrigin={1}]", (object) this.LuckyDrawRewardId, (object) this.LuckyDrawOrigin);
  }

  public class ProfileNoticeRedundantNDEReroll : NetCache.ProfileNotice
  {
    public ProfileNoticeRedundantNDEReroll()
      : base(NetCache.ProfileNotice.NoticeType.REDUNDANT_NDE_REROLL)
    {
    }

    public string CardID { get; set; }

    public TAG_PREMIUM Premium { get; set; }

    public override string ToString() => string.Format("{0} [CardID={1}, Premium={2}]", (object) base.ToString(), (object) this.CardID, (object) this.Premium);
  }

  public class ProfileNoticeRedundantNDERerollResult : NetCache.ProfileNotice
  {
    public ProfileNoticeRedundantNDERerollResult()
      : base(NetCache.ProfileNotice.NoticeType.REDUNDANT_NDE_REROLL_RESULT)
    {
    }

    public int RerolledCardID { get; set; }

    public int GrantedCardID { get; set; }

    public TAG_PREMIUM Premium { get; set; }

    public override string ToString() => string.Format("{0}, [RerolledCardID={1}, GrantedCardID={2}, Premium={3}]", (object) base.ToString(), (object) this.RerolledCardID, (object) this.GrantedCardID, (object) this.Premium);
  }

  public abstract class ClientOptionBase : ICloneable
  {
    public abstract void PopulateIntoPacket(ServerOption type, SetOptions packet);

    public override bool Equals(object other) => other != null && !(other.GetType() != this.GetType());

    public override int GetHashCode() => base.GetHashCode();

    public object Clone() => this.MemberwiseClone();
  }

  public class ClientOptionInt : NetCache.ClientOptionBase
  {
    public ClientOptionInt(int val) => this.OptionValue = val;

    public int OptionValue { get; set; }

    public override void PopulateIntoPacket(ServerOption type, SetOptions packet) => packet.Options.Add(new PegasusUtil.ClientOption()
    {
      Index = (int) type,
      AsInt32 = this.OptionValue
    });

    public override bool Equals(object other) => base.Equals(other) && ((NetCache.ClientOptionInt) other).OptionValue == this.OptionValue;

    public override int GetHashCode() => this.OptionValue.GetHashCode();
  }

  public class ClientOptionLong : NetCache.ClientOptionBase
  {
    public ClientOptionLong(long val) => this.OptionValue = val;

    public long OptionValue { get; set; }

    public override void PopulateIntoPacket(ServerOption type, SetOptions packet) => packet.Options.Add(new PegasusUtil.ClientOption()
    {
      Index = (int) type,
      AsInt64 = this.OptionValue
    });

    public override bool Equals(object other) => base.Equals(other) && ((NetCache.ClientOptionLong) other).OptionValue == this.OptionValue;

    public override int GetHashCode() => this.OptionValue.GetHashCode();
  }

  public class ClientOptionFloat : NetCache.ClientOptionBase
  {
    public ClientOptionFloat(float val) => this.OptionValue = val;

    public float OptionValue { get; set; }

    public override void PopulateIntoPacket(ServerOption type, SetOptions packet) => packet.Options.Add(new PegasusUtil.ClientOption()
    {
      Index = (int) type,
      AsFloat = this.OptionValue
    });

    public override bool Equals(object other) => base.Equals(other) && (double) ((NetCache.ClientOptionFloat) other).OptionValue == (double) this.OptionValue;

    public override int GetHashCode() => this.OptionValue.GetHashCode();
  }

  public class ClientOptionULong : NetCache.ClientOptionBase
  {
    public ClientOptionULong(ulong val) => this.OptionValue = val;

    public ulong OptionValue { get; set; }

    public override void PopulateIntoPacket(ServerOption type, SetOptions packet) => packet.Options.Add(new PegasusUtil.ClientOption()
    {
      Index = (int) type,
      AsUint64 = this.OptionValue
    });

    public override bool Equals(object other) => base.Equals(other) && (long) ((NetCache.ClientOptionULong) other).OptionValue == (long) this.OptionValue;

    public override int GetHashCode() => this.OptionValue.GetHashCode();
  }

  public class NetCacheClientOptions
  {
    private DateTime? m_mostRecentDispatchToServer;
    private DateTime? m_currentScheduledDispatchTime;

    public NetCacheClientOptions()
    {
      this.ClientState = new Map<ServerOption, NetCache.ClientOptionBase>();
      this.ServerState = new Map<ServerOption, NetCache.ClientOptionBase>();
    }

    public void UpdateServerState()
    {
      foreach (KeyValuePair<ServerOption, NetCache.ClientOptionBase> keyValuePair in this.ClientState)
        this.ServerState[keyValuePair.Key] = keyValuePair.Value == null ? (NetCache.ClientOptionBase) null : (NetCache.ClientOptionBase) keyValuePair.Value.Clone();
    }

    private int ClientOptionsUpdateIntervalSeconds
    {
      get
      {
        NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
        return netObject != null && netObject.Misc != null ? netObject.Misc.ClientOptionsUpdateIntervalSeconds : 180;
      }
    }

    public void OnUpdateIntervalElasped(object userData)
    {
      this.m_currentScheduledDispatchTime = new DateTime?();
      this.DispatchClientOptionsToServer();
    }

    public void CancelScheduledDispatchToServer()
    {
      Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.OnUpdateIntervalElasped));
      this.m_currentScheduledDispatchTime = new DateTime?();
    }

    public void DispatchClientOptionsToServer()
    {
      this.CancelScheduledDispatchToServer();
      bool flag = false;
      SetOptions packet = new SetOptions();
      foreach (KeyValuePair<ServerOption, NetCache.ClientOptionBase> keyValuePair in this.ClientState)
      {
        NetCache.ClientOptionBase clientOptionBase;
        if (this.ServerState.TryGetValue(keyValuePair.Key, out clientOptionBase))
        {
          if (keyValuePair.Value != null || clientOptionBase != null)
          {
            if (keyValuePair.Value == null && clientOptionBase != null || keyValuePair.Value != null && clientOptionBase == null)
            {
              flag = true;
              break;
            }
            if (!clientOptionBase.Equals((object) keyValuePair.Value))
            {
              flag = true;
              break;
            }
          }
        }
        else
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return;
      foreach (KeyValuePair<ServerOption, NetCache.ClientOptionBase> keyValuePair in this.ClientState)
      {
        if (keyValuePair.Value != null)
          keyValuePair.Value.PopulateIntoPacket(keyValuePair.Key, packet);
      }
      Network.Get().SetClientOptions(packet);
      this.m_mostRecentDispatchToServer = new DateTime?(DateTime.UtcNow);
      this.UpdateServerState();
    }

    public void RemoveInvalidOptions()
    {
      List<ServerOption> serverOptionList = new List<ServerOption>();
      foreach (KeyValuePair<ServerOption, NetCache.ClientOptionBase> keyValuePair in this.ClientState)
      {
        ServerOption key = keyValuePair.Key;
        NetCache.ClientOptionBase clientOptionBase = keyValuePair.Value;
        System.Type serverOptionType = Options.Get().GetServerOptionType(key);
        if (clientOptionBase != null)
        {
          System.Type type = clientOptionBase.GetType();
          if (serverOptionType == typeof (int))
          {
            if (type == typeof (NetCache.ClientOptionInt))
              continue;
          }
          else if (serverOptionType == typeof (long))
          {
            if (type == typeof (NetCache.ClientOptionLong))
              continue;
          }
          else if (serverOptionType == typeof (float))
          {
            if (type == typeof (NetCache.ClientOptionFloat))
              continue;
          }
          else if (serverOptionType == typeof (ulong) && type == typeof (NetCache.ClientOptionULong))
            continue;
          if (serverOptionType == (System.Type) null)
            Log.Net.Print("NetCacheClientOptions.RemoveInvalidOptions() - Option {0} has type {1}, but value is type {2}. Removing it.", (object) key, (object) serverOptionType, (object) type);
          else
            Log.Net.Print("NetCacheClientOptions.RemoveInvalidOptions() - Option {0} has type {1}, but value is type {2}. Removing it.", (object) Blizzard.T5.Core.Utils.EnumUtils.GetString<ServerOption>(key), (object) serverOptionType, (object) type);
        }
        serverOptionList.Add(key);
      }
      foreach (ServerOption key in serverOptionList)
      {
        this.ClientState.Remove(key);
        this.ServerState.Remove(key);
      }
    }

    public void CheckForDispatchToServer()
    {
      float updateIntervalSeconds = (float) this.ClientOptionsUpdateIntervalSeconds;
      if ((double) updateIntervalSeconds <= 0.0)
        return;
      DateTime utcNow = DateTime.UtcNow;
      bool flag1 = false;
      bool flag2 = false;
      if (!this.m_mostRecentDispatchToServer.HasValue)
        flag1 = true;
      else if (!this.m_currentScheduledDispatchTime.HasValue)
      {
        TimeSpan timeSpan = utcNow - this.m_mostRecentDispatchToServer.Value;
        if (timeSpan.TotalSeconds >= (double) updateIntervalSeconds)
        {
          flag1 = true;
        }
        else
        {
          flag2 = true;
          updateIntervalSeconds -= (float) timeSpan.TotalSeconds;
        }
      }
      if (!flag1 && !flag2 && this.m_currentScheduledDispatchTime.HasValue && (this.m_currentScheduledDispatchTime.Value - utcNow).TotalSeconds > (double) updateIntervalSeconds)
        flag2 = true;
      if (!(flag1 | flag2))
        return;
      double secondsToWait = flag1 ? 0.0 : (double) updateIntervalSeconds;
      this.m_currentScheduledDispatchTime = new DateTime?(utcNow);
      Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.OnUpdateIntervalElasped));
      Processor.ScheduledCallback cb = new Processor.ScheduledCallback(this.OnUpdateIntervalElasped);
      Processor.ScheduleCallback((float) secondsToWait, true, cb);
    }

    public Map<ServerOption, NetCache.ClientOptionBase> ClientState { get; private set; }

    private Map<ServerOption, NetCache.ClientOptionBase> ServerState { get; set; }
  }

  public class NetCacheFavoriteHeroes
  {
    public List<(TAG_CLASS, NetCache.CardDefinition)> FavoriteHeroes { get; set; }

    public NetCacheFavoriteHeroes() => this.FavoriteHeroes = new List<(TAG_CLASS, NetCache.CardDefinition)>();
  }

  public class NetCacheAccountLicenses
  {
    public NetCacheAccountLicenses() => this.AccountLicenses = new Map<long, AccountLicenseInfo>();

    public Map<long, AccountLicenseInfo> AccountLicenses { get; set; }
  }

  public class NetCacheBattlegroundsHeroSkins
  {
    public Map<int, BattlegroundsHeroSkinId> BattlegroundsFavoriteHeroSkins { get; set; }

    public HashSet<BattlegroundsHeroSkinId> OwnedBattlegroundsSkins { get; }

    public HashSet<BattlegroundsHeroSkinId> UnseenSkinIds { get; }

    public NetCacheBattlegroundsHeroSkins()
    {
      this.OwnedBattlegroundsSkins = new HashSet<BattlegroundsHeroSkinId>();
      this.BattlegroundsFavoriteHeroSkins = new Map<int, BattlegroundsHeroSkinId>();
      this.UnseenSkinIds = new HashSet<BattlegroundsHeroSkinId>();
    }
  }

  public class NetCacheBattlegroundsGuideSkins
  {
    public BattlegroundsGuideSkinId? BattlegroundsFavoriteGuideSkin { get; set; }

    public HashSet<BattlegroundsGuideSkinId> OwnedBattlegroundsGuideSkins { get; }

    public HashSet<BattlegroundsGuideSkinId> UnseenSkinIds { get; }

    public NetCacheBattlegroundsGuideSkins()
    {
      this.OwnedBattlegroundsGuideSkins = new HashSet<BattlegroundsGuideSkinId>();
      this.BattlegroundsFavoriteGuideSkin = new BattlegroundsGuideSkinId?();
      this.UnseenSkinIds = new HashSet<BattlegroundsGuideSkinId>();
    }
  }

  public class NetCacheBattlegroundsBoardSkins
  {
    public BattlegroundsBoardSkinId? BattlegroundsFavoriteBoardSkin { get; set; }

    public HashSet<BattlegroundsBoardSkinId> OwnedBattlegroundsBoardSkins { get; set; }

    public HashSet<BattlegroundsBoardSkinId> UnseenSkinIds { get; }

    public NetCacheBattlegroundsBoardSkins()
    {
      this.OwnedBattlegroundsBoardSkins = new HashSet<BattlegroundsBoardSkinId>();
      this.BattlegroundsFavoriteBoardSkin = new BattlegroundsBoardSkinId?();
      this.UnseenSkinIds = new HashSet<BattlegroundsBoardSkinId>();
    }
  }

  public class NetCacheBattlegroundsFinishers
  {
    public BattlegroundsFinisherId? BattlegroundsFavoriteFinisher { get; set; }

    public HashSet<BattlegroundsFinisherId> OwnedBattlegroundsFinishers { get; set; }

    public HashSet<BattlegroundsFinisherId> UnseenSkinIds { get; }

    public NetCacheBattlegroundsFinishers()
    {
      this.OwnedBattlegroundsFinishers = new HashSet<BattlegroundsFinisherId>();
      this.BattlegroundsFavoriteFinisher = new BattlegroundsFinisherId?();
      this.UnseenSkinIds = new HashSet<BattlegroundsFinisherId>();
    }
  }

  public class NetCacheBattlegroundsEmotes
  {
    private Hearthstone.BattlegroundsEmoteLoadout _currentLoadout = new Hearthstone.BattlegroundsEmoteLoadout();

    public HashSet<BattlegroundsEmoteId> OwnedBattlegroundsEmotes { get; set; }

    public HashSet<BattlegroundsEmoteId> UnseenEmoteIds { get; }

    public Hearthstone.BattlegroundsEmoteLoadout CurrentLoadout
    {
      get => new Hearthstone.BattlegroundsEmoteLoadout(this._currentLoadout);
      set => this._currentLoadout = new Hearthstone.BattlegroundsEmoteLoadout(value);
    }

    public NetCacheBattlegroundsEmotes()
    {
      this.OwnedBattlegroundsEmotes = new HashSet<BattlegroundsEmoteId>();
      this.UnseenEmoteIds = new HashSet<BattlegroundsEmoteId>();
      this.CurrentLoadout = new Hearthstone.BattlegroundsEmoteLoadout();
    }
  }

  public class NetCacheLettuceMap
  {
    public NetCacheLettuceMap() => this.Map = (PegasusLettuce.LettuceMap) null;

    public PegasusLettuce.LettuceMap Map { get; set; }
  }

  public delegate void ErrorCallback(NetCache.ErrorInfo info);

  public enum ErrorCode
  {
    NONE,
    TIMEOUT,
    SERVER,
  }

  public class ErrorInfo
  {
    public NetCache.ErrorCode Error { get; set; }

    public uint ServerError
    {
      set => this.\u003CServerError\u003Ek__BackingField = value;
    }

    public NetCache.RequestFunc RequestingFunction { get; set; }

    public Map<System.Type, NetCache.Request> RequestedTypes { get; set; }

    public string RequestStackTrace { get; set; }
  }

  public delegate void NetCacheCallback();

  public delegate void RequestFunc(
    NetCache.NetCacheCallback callback,
    NetCache.ErrorCallback errorCallback);

  public enum RequestResult
  {
    UNKNOWN,
    PENDING,
    IN_PROCESS,
    GENERIC_COMPLETE,
    DATA_COMPLETE,
    ERROR,
    MIGRATION_REQUIRED,
  }

  public class Request
  {
    public System.Type m_type;
    public bool m_reload;
    public NetCache.RequestResult m_result;

    public Request(System.Type rt, bool rl = false)
    {
      this.m_type = rt;
      this.m_reload = rl;
      this.m_result = NetCache.RequestResult.UNKNOWN;
    }
  }

  private class NetCacheBatchRequest
  {
    public Map<System.Type, NetCache.Request> m_requests = new Map<System.Type, NetCache.Request>();
    public NetCache.NetCacheCallback m_callback;
    public NetCache.ErrorCallback m_errorCallback;
    public bool m_canTimeout = true;
    public float m_timeAdded = Time.realtimeSinceStartup;
    public NetCache.RequestFunc m_requestFunc;
    public string m_requestStackTrace;

    public NetCacheBatchRequest(
      NetCache.NetCacheCallback reply,
      NetCache.ErrorCallback errorCallback,
      NetCache.RequestFunc requestFunc)
    {
      this.m_callback = reply;
      this.m_errorCallback = errorCallback;
      this.m_requestFunc = requestFunc;
      this.m_requestStackTrace = Environment.StackTrace;
    }

    public void AddRequests(List<NetCache.Request> requests)
    {
      foreach (NetCache.Request request in requests)
        this.AddRequest(request);
    }

    public void AddRequest(NetCache.Request r)
    {
      if (this.m_requests.ContainsKey(r.m_type))
        return;
      this.m_requests.Add(r.m_type, r);
    }
  }
}
