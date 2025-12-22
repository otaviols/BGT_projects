using Blizzard.Commerce;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Logging;
using Blizzard.T5.Services;
using Blizzard.Telemetry.WTCG.Client;
using BobNetProto;
using Hearthstone;
using Hearthstone.Commerce;
using Hearthstone.Login;
using Hearthstone.Networking.BattleNet;
using Hearthstone.Streaming;
using Hearthstone.Util;
using HSCachedDeckCompletion;
using Networking;
using PegasusFSG;
using PegasusGame;
using PegasusLettuce;
using PegasusLuckyDraw;
using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Game.Shop.Product;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class Network : IHasUpdate, IService
{
  public static string TutorialServer = "01";
  private static readonly float PROCESS_WARNING = 15f;
  private static readonly float PROCESS_WARNING_REPORT_GAP = 1f;
  public static readonly PlatformDependentValue<bool> LAUNCHES_WITH_BNET_APP = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    PC = true,
    Mac = true,
    iOS = false,
    Android = false
  };
  public static readonly PlatformDependentValue<bool> CONNECT_TO_AURORA_BY_DEFAULT = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    PC = true,
    Mac = true,
    iOS = false,
    Android = false
  };
  private static readonly Map<BnetRegion, string> RegionToTutorialName = new Map<BnetRegion, string>()
  {
    {
      BnetRegion.REGION_US,
      "us-tutorial{0}.actual.battle.net"
    },
    {
      BnetRegion.REGION_EU,
      "eu-tutorial{0}.actual.battle.net"
    },
    {
      BnetRegion.REGION_KR,
      "kr-tutorial{0}.actual.battle.net"
    },
    {
      BnetRegion.REGION_CN,
      "cn-tutorial{0}.actual.battlenet.com.cn"
    }
  };
  private static readonly SortedDictionary<int, int> m_deferredMessageResponseMap = new SortedDictionary<int, int>()
  {
    {
      305,
      306
    },
    {
      303,
      304
    },
    {
      205,
      307
    },
    {
      314,
      315
    }
  };
  private static readonly SortedDictionary<int, int> m_deferredGetAccountInfoMessageResponseMap = new SortedDictionary<int, int>()
  {
    {
      11,
      233
    },
    {
      18,
      264
    },
    {
      4,
      232
    },
    {
      2,
      202
    },
    {
      10,
      231
    },
    {
      15,
      260
    },
    {
      19,
      271
    },
    {
      8,
      270
    },
    {
      21,
      283
    },
    {
      7,
      236
    },
    {
      27,
      318
    },
    {
      28,
      325
    },
    {
      29,
      608
    },
    {
      30,
      621
    },
    {
      31,
      626
    },
    {
      32,
      637
    },
    {
      33,
      642
    },
    {
      34,
      649
    }
  };
  private IDispatcher m_dispatcherImpl;
  private Map<int, List<Network.NetHandler>> m_netHandlers = new Map<int, List<Network.NetHandler>>();
  private Network.QueueInfoHandler m_queueInfoHandler;
  private Network.GameQueueHandler m_gameQueueHandler;
  private int m_numConnectionFailures;
  private ConnectAPI m_connectApi;
  private uint m_gameServerKeepAliveFrequencySeconds;
  private uint m_gameServerKeepAliveRetry;
  private uint m_gameServerKeepAliveWaitForInternetSeconds;
  private bool m_gameConceded;
  private bool m_disconnectRequested;
  private double m_timeInternetUnreachable;
  private AckCardSeen m_ackCardSeenPacket = new AckCardSeen();
  private AckBattlegroundsSkinsSeen m_ackBattlegroundsSkinsSeenPacket = new AckBattlegroundsSkinsSeen();
  private readonly List<Network.ConnectErrorParams> m_errorList = new List<Network.ConnectErrorParams>();
  private List<Network.ThrottledPacketListener> m_throttledPacketListeners = new List<Network.ThrottledPacketListener>();
  private List<Network.RequestContext> m_inTransitRequests = new List<Network.RequestContext>();
  private static float m_maxDeferredWait = 120f;
  private static bool s_shouldBeConnectedToAurora = (bool) Network.CONNECT_TO_AURORA_BY_DEFAULT;
  private static bool s_running;
  private static UnityUrlDownloader s_urlDownloader = new UnityUrlDownloader();
  private static UrlDownloaderAsync s_urlDownloaderAsync = new UrlDownloaderAsync();
  private Network.NetworkState m_state;
  private NetworkReachabilityManager m_networkReachabilityManager;
  private Hearthstone.BreakingNews.BreakingNews m_breakingNews;
  private List<BnetEvent> m_bnetEvents = new List<BnetEvent>();
  private List<BnetWhisper> m_bnetWhispers = new List<BnetWhisper>();
  private List<BnetNotification> m_bnetNotifications = new List<BnetNotification>();
  private List<FriendsUpdate> m_friendsUpdates = new List<FriendsUpdate>();
  private List<PresenceUpdate> m_presenceUpdates = new List<PresenceUpdate>();
  private List<BnetErrorInfo> m_bnetErrors = new List<BnetErrorInfo>();

  public void MercenariesPlayerInfoRequest() => this.m_connectApi.MercenariesPlayerInfoRequest(new PegasusLettuce.MercenariesPlayerInfoRequest());

  public PegasusLettuce.MercenariesPlayerInfoResponse MercenariesPlayerInfoResponse() => this.m_connectApi.MercenariesPlayerInfoResponse();

  public void MercenariesCollectionRequest() => this.m_connectApi.MercenariesCollectionRequest(new PegasusLettuce.MercenariesCollectionRequest());

  public PegasusLettuce.MercenariesCollectionResponse MercenariesCollectionResponse() => this.m_connectApi.MercenariesCollectionResponse();

  public PegasusLettuce.MercenariesCollectionUpdate MercenariesCollectionUpdate() => this.m_connectApi.MercenariesCollectionUpdate();

  public PegasusLettuce.MercenariesCurrencyUpdate MercenariesCurrencyUpdate() => this.m_connectApi.MercenariesCurrencyUpdate();

  public PegasusLettuce.MercenariesExperienceUpdate MercenariesExperienceUpdate() => this.m_connectApi.MercenariesExperienceUpdate();

  public PegasusLettuce.MercenariesRewardUpdate MercenariesRewardUpdate() => this.m_connectApi.MercenariesRewardUpdate();

  public void MercenariesTeamListRequest() => this.m_connectApi.MercenariesTeamListRequest(new PegasusLettuce.MercenariesTeamListRequest());

  public PegasusLettuce.MercenariesTeamListResponse MercenariesTeamListResponse() => this.m_connectApi.MercenariesTeamListResponse();

  public void CreateMercenariesTeamRequest(string name, PegasusLettuce.LettuceTeam.Type type, out int? requestId)
  {
    if (!Network.IsLoggedIn())
    {
      requestId = new int?();
    }
    else
    {
      requestId = new int?(this.GetNextCreateTeamRequestId());
      Log.Net.Print("Network.CreateMercenariesTeamRequest");
      uint a = 0;
      List<LettuceTeam> teams = CollectionManager.Get().GetTeams();
      for (int index = 0; index < teams.Count; ++index)
        a = (uint) Mathf.Max((float) a, (float) (teams[index].SortOrder + 1U));
      PegasusLettuce.CreateMercenariesTeamRequest request = new PegasusLettuce.CreateMercenariesTeamRequest()
      {
        Team = new PegasusLettuce.LettuceTeam()
        {
          Name = name,
          Type_ = type,
          SortOrder = a
        }
      };
      request.RequestId = requestId.Value;
      this.m_connectApi.CreateMercenariesTeamRequest(request);
    }
  }

  public PegasusLettuce.CreateMercenariesTeamResponse CreateMercenariesTeamResponse() => this.m_connectApi.CreateMercenariesTeamResponse();

  public void UpdateMercenariesTeamRequest(LettuceTeam team)
  {
    PegasusLettuce.UpdateMercenariesTeamRequest request = new PegasusLettuce.UpdateMercenariesTeamRequest();
    if (team.Name == null)
    {
      Log.Net.PrintError("Network.UpdateMercenariesTeamRequest - Team name is null!");
    }
    else
    {
      request.Team = new PegasusLettuce.LettuceTeam()
      {
        TeamId = team.ID,
        Name = team.Name,
        Type_ = team.TeamType,
        SortOrder = team.SortOrder,
        MercenaryList = new LettuceTeamMercenaryList()
      };
      foreach (LettuceMercenary merc in team.GetMercs())
      {
        LettuceMercenary.Loadout loadout = team.GetLoadout(merc);
        if (loadout == null || !loadout.IsValid())
        {
          Log.Net.PrintError(string.Format("Network.UpdateMercenariesTeamRequest - Loadout was null or invalid mercenary{0}!", (object) merc.ID));
        }
        else
        {
          LettuceTeamMercenary lettuceTeamMercenary = new LettuceTeamMercenary()
          {
            MercenaryId = merc.ID,
            SelectedArtVariationId = loadout.m_artVariationRecord.ID,
            SelectedArtVariationPremium = (int) loadout.m_artVariationPremium
          };
          if (loadout.m_equipmentRecord != null)
            lettuceTeamMercenary.SelectedEquipmentId = loadout.m_equipmentRecord.ID;
          request.Team.MercenaryList.Mercenaries.Add(lettuceTeamMercenary);
        }
      }
      this.m_connectApi.UpdateMercenariesTeamRequest(request);
    }
  }

  public PegasusLettuce.UpdateMercenariesTeamResponse UpdateMercenariesTeamResponse() => this.m_connectApi.UpdateMercenariesTeamResponse();

  public void MercenariesTeamReorderRequest(LettuceTeam team) => this.m_connectApi.MercenariesTeamReorderRequest(new PegasusLettuce.MercenariesTeamReorderRequest()
  {
    TeamId = team.ID,
    SortOrder = team.SortOrder
  });

  public void DeleteTeam(long teamId) => this.m_connectApi.DeleteMercenariesTeamRequest(new DeleteMercenariesTeamRequest()
  {
    TeamId = teamId
  });

  public PegasusLettuce.DeleteMercenariesTeamResponse DeleteMercenariesTeamResponse() => this.m_connectApi.DeleteMercenariesTeamResponse();

  private int GetNextCreateTeamRequestId() => ++this.m_state.CurrentCreateTeamRequestId;

  public void UpdateEquippedMercenaryEquipment(int mercenaryId, int? equipmentId)
  {
    UpdateEquippedMercenaryEquipmentRequest request = new UpdateEquippedMercenaryEquipmentRequest()
    {
      MercenaryId = mercenaryId
    };
    if (equipmentId.HasValue)
      request.EquipmentId = equipmentId.Value;
    this.m_connectApi.UpdateEquippedMercenaryEquipmentRequest(request);
  }

  public PegasusLettuce.UpdateEquippedMercenaryEquipmentResponse UpdateEquippedMercenaryEquipmentResponse() => this.m_connectApi.UpdateEquippedMercenaryEquipmentResponse();

  public void CraftMercenary(int mercenaryId) => this.m_connectApi.CraftMercenaryRequest(new CraftMercenaryRequest()
  {
    MercenaryId = mercenaryId
  });

  public PegasusLettuce.CraftMercenaryResponse CraftMercenaryResponse() => this.m_connectApi.CraftMercenaryResponse();

  public void UpgradeMercenaryAbility(int mercenaryId, int abilityId) => this.m_connectApi.UpgradeMercenaryAbilityRequest(new UpgradeMercenaryAbilityRequest()
  {
    MercenaryId = mercenaryId,
    AbilityId = abilityId
  });

  public PegasusLettuce.UpgradeMercenaryAbilityResponse UpgradeMercenaryAbilityResponse() => this.m_connectApi.UpgradeMercenaryAbilityResponse();

  public void CraftMercenaryEquipment(int mercenaryId, int equipmentId) => this.m_connectApi.CraftMercenaryEquipmentRequest(new CraftMercenaryEquipmentRequest()
  {
    MercenaryId = mercenaryId,
    EquipmentId = equipmentId
  });

  public PegasusLettuce.CraftMercenaryEquipmentResponse CraftMercenaryEquipmentResponse() => this.m_connectApi.CraftMercenaryEquipmentResponse();

  public void UpgradeMercenaryEquipment(int mercenaryId, int equipmentId) => this.m_connectApi.UpgradeMercenaryEquipmentRequest(new UpgradeMercenaryEquipmentRequest()
  {
    MercenaryId = mercenaryId,
    EquipmentId = equipmentId
  });

  public void UpdateEquippedMercenaryArtVariation(
    int mercenaryId,
    int artVariationId,
    TAG_PREMIUM premium)
  {
    this.m_connectApi.UpdateEquippedMercenaryArtVariationRequest(new UpdateEquippedMercenaryArtVariationRequest()
    {
      MercenaryId = mercenaryId,
      EquippedArtVariation = new MercenaryArtVariation()
      {
        AssetId = artVariationId,
        Premium = (uint) premium
      }
    });
  }

  public PegasusLettuce.UpdateEquippedMercenaryArtVariationResponse UpdateEquippedMercenaryArtVariationResponse() => this.m_connectApi.GetUpdateEquippedMercenaryArtVariationResponse();

  public PegasusLettuce.UpgradeMercenaryEquipmentResponse UpgradeMercenaryEquipmentResponse() => this.m_connectApi.UpgradeMercenaryEquipmentResponse();

  public void OpenMercenariesPackRequest() => this.m_connectApi.OpenMercenariesPackRequest(new PegasusLettuce.OpenMercenariesPackRequest());

  public PegasusLettuce.OpenMercenariesPackResponse OpenMercenariesPackResponse() => this.m_connectApi.OpenMercenariesPackResponse();

  public PegasusLettuce.MercenariesPvPRatingUpdate MercenariesPvPRatingUpdate() => this.m_connectApi.MercenariesPvPRatingUpdate();

  public PegasusLettuce.MercenariesPvPWinUpdate MercenariesPvPWinUpdate() => this.m_connectApi.MercenariesPvPWinUpdate();

  public PegasusLettuce.MercenariesPlayerBountyInfoUpdate MercenariesPlayerBountyInfoUpdate() => this.m_connectApi.MercenariesPlayerBountyInfoUpdate();

  public PegasusLettuce.MercenariesTeamUpdate MercenariesTeamUpdate() => this.m_connectApi.MercenariesTeamUpdate();

  public void MercenariesTrainingAddRequest(int mercenaryID) => this.m_connectApi.MercenariesTrainingAddRequest(mercenaryID);

  public void MercenariesTrainingRemoveRequest(int mercenaryID) => this.m_connectApi.MercenariesTrainingRemoveRequest(mercenaryID);

  public void MercenariesTrainingCollectRequest(int mercenaryID) => this.m_connectApi.MercenariesTrainingCollectRequest(mercenaryID);

  public PegasusLettuce.MercenariesTrainingAddResponse MercenariesTrainingAddResponse() => this.m_connectApi.MercenariesTrainingAddResponse();

  public PegasusLettuce.MercenariesTrainingRemoveResponse MercenariesTrainingRemoveResponse() => this.m_connectApi.MercenariesTrainingRemoveResponse();

  public PegasusLettuce.MercenariesTrainingCollectResponse MercenariesTrainingCollectResponse() => this.m_connectApi.MercenariesTrainingCollectResponse();

  public void SendMercenariesDebugCommandRequest(MercenariesDebugCommandRequest request) => this.m_connectApi.RequestMercenaryDebugCommand(request);

  public PegasusLettuce.MercenariesDebugCommandResponse MercenariesDebugCommandResponse() => this.m_connectApi.MercenariesDebugCommandResponse();

  public void MercenariesVillageStatusRequest() => this.m_connectApi.RequestMercenaryVillageStatus(new MercenariesGetVillageRequest());

  public MercenariesGetVillageResponse MercenariesVillageStatusResponse() => this.m_connectApi.GetMercenaryVillageStatusResponse();

  public void MercenariesVisitorRefreshRequest() => this.m_connectApi.RequestMercenaryVisitorRefresh(new MercenariesRefreshVisitorsRequest());

  public MercenariesRefreshVisitorsResponse MercenariesVisitorRefreshResponse() => this.m_connectApi.GetMercenaryVisitorRefreshResponse();

  public PegasusLettuce.MercenariesVisitorStateUpdate MercenariesVisitorStateUpdate() => this.m_connectApi.GetMercenaryVisitorStateUpdate();

  public PegasusLettuce.MercenariesBuildingStateUpdate MercenariesBuildingStateUpdate() => this.m_connectApi.GetMercenaryBuildingStateUpdate();

  public void UpgradeMercenaryBuilding(int buildingId, int requestedTierId) => this.m_connectApi.RequestMercenaryBuildingUpgrade(new MercenariesBuildingUpgradeRequest()
  {
    BuildingId = buildingId,
    RequestedTier = requestedTierId
  });

  public MercenariesBuildingUpgradeResponse UpgradeMercenaryBuildingResponse() => this.m_connectApi.GetMercenaryBuildingUpgradeResponse();

  public void ClaimMercenaryTask(int taskId) => this.m_connectApi.RequestMercenaryClaimTask(new MercenariesClaimTaskRequest()
  {
    TaskId = taskId
  });

  public MercenariesClaimTaskResponse ClaimMercenaryTaskResponse() => this.m_connectApi.GetMercenaryClaimTaskResponse();

  public void DismissMercenaryTask(int visitorId) => this.m_connectApi.RequestMercenaryDismissTask(new MercenariesDismissTaskRequest()
  {
    VisitorId = visitorId
  });

  public MercenariesDismissTaskResponse DismissMercenaryTaskResponse() => this.m_connectApi.GetMercenaryDismissTaskResponse();

  public void AcknowledgeBounties(List<int> bountyIds) => this.m_connectApi.RequestMercenaryBountyAcknowledge(new MercenariesBountyAcknowledgeRequest()
  {
    BountyIds = bountyIds
  });

  public MercenariesBountyAcknowledgeResponse AcknowledgeBountiesResponse() => this.m_connectApi.GetMercenaryBountyAcknowledgeResponse();

  public void AcknowledgeMercenaryCollection(List<MercenaryAcknowledgeData> acknowledgeData) => this.m_connectApi.RequestMercenaryCollectionAcknowledge(new MercenariesCollectionAcknowledgeRequest()
  {
    Acknowledgments = acknowledgeData
  });

  public MercenariesCollectionAcknowledgeResponse AcknowledgeMercenaryCollectionResponse() => this.m_connectApi.GetMercenaryCollectionAcknowledgeResponse();

  public void ConvertExcessCoinsToRenown(List<int> mercenaryIds) => this.m_connectApi.RequestConvertExcessCoinsToRenown(new MercenariesConvertExcessCoinsRequest()
  {
    MercenaryIds = mercenaryIds
  });

  public MercenariesConvertExcessCoinsResponse ConvertExcessCoinsToRenownResponse() => this.m_connectApi.GetMercenariesConvertExcessCoinsResponse();

  public void PurchaseRenownOffer(int renownOfferId) => this.m_connectApi.RequestPurchaseRenownOffer(new MercenariesPurchaseRenownOfferRequest()
  {
    RenownOfferId = (long) renownOfferId
  });

  public MercenariesPurchaseRenownOfferResponse PurchaseRenownOfferResponse() => this.m_connectApi.GetPurchaseRenownOfferResponse();

  public void DismissRenownOffer(int renownOfferId) => this.m_connectApi.RequestDismissRenownOffer(new MercenariesDismissRenownOfferRequest()
  {
    RenownOfferId = (long) renownOfferId
  });

  public MercenariesDismissRenownOfferResponse DismissRenownOfferResponse() => this.m_connectApi.GetDismissRenownOfferResponse();

  public event System.Action<BattleNetErrors> OnConnectedToBattleNet;

  public event System.Action<BattleNetErrors> OnDisconnectedFromBattleNet;

  public static string BranchName => string.Format("{0}.{1}{2}", (object) "25.0", (object) "0", (object) "");

  private static List<BattleNetErrors> GameServerDisconnectEvents { get; set; }

  private long FakeIdWaitingForResponse { get; set; }

  private string GameServerIPv6 { get; set; }

  private string GameServerIPv4 { get; set; }

  private bool IsOSSupportIPv6 { get; set; } = Socket.OSSupportsIPv6;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    Network network = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    network.m_networkReachabilityManager = ServiceManager.Get<NetworkReachabilityManager>();
    network.m_breakingNews = ServiceManager.Get<Hearthstone.BreakingNews.BreakingNews>();
    network.m_state.SetDefaults();
    if (PlatformSettings.s_isDeviceSupported)
    {
      HearthstoneApplication.Get().WillReset += new System.Action(network.WillReset);
      HearthstoneApplication.Get().Resetting += new System.Action(network.OnReset);
      Network.s_running = true;
      network.CreateNewDispatcher();
      network.InitBattleNet(network.m_dispatcherImpl);
      network.RegisterNetHandler((object) SubscribeResponse.PacketID.ID, new Network.NetHandler(network.OnSubscribeResponse));
      network.RegisterNetHandler((object) ClientStateNotification.PacketID.ID, new Network.NetHandler(network.OnClientStateNotification));
      network.RegisterNetHandler((object) PegasusUtil.GenericResponse.PacketID.ID, new Network.NetHandler(network.OnGenericResponse));
      network.RegisterNetHandler((object) PegasusUtil.GetDeckContentsResponse.PacketID.ID, new Network.NetHandler(network.OnDeckContentsResponse));
      network.OnConnectedToBattleNet += new System.Action<BattleNetErrors>(network.OnConnectedToBattleNetCallback);
      network.OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(network.OnDisconnectedFromBattleNetCallback);
      if (!(bool) Network.CONNECT_TO_AURORA_BY_DEFAULT)
        Network.SetShouldBeConnectedToAurora(global::Options.Get().GetBool(Option.CONNECT_TO_AURORA));
    }
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[3]
  {
    typeof (GameDbf),
    typeof (NetworkReachabilityManager),
    typeof (Hearthstone.BreakingNews.BreakingNews)
  };

  public void Shutdown()
  {
    if (!Network.s_running)
      return;
    NetCache.Get().DispatchClientOptionsToServer();
    PresenceMgr.Get().OnShutdown();
    if (Network.IsLoggedIn())
      this.CancelFindGame();
    this.CloseAll();
    this.ClearTransientBnetPresence();
    Blizzard.GameService.SDK.Client.Integration.BattleNet.AppQuit();
    BnetRecentPlayerMgr.Get().Shutdown();
    BnetNearbyPlayerMgr.Get().Shutdown();
    Network.s_running = false;
  }

  private void ClearTransientBnetPresence()
  {
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceBlob(17U, (byte[]) null);
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceString(19U, string.Empty);
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceString(20U, string.Empty);
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceBlob(21U, (byte[]) null);
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceBlob(23U, (byte[]) null);
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceBlob(24U, (byte[]) null);
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceBlob(25U, (byte[]) null);
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceEntityId(26U, new BnetEntityId(0UL, 0UL));
    Blizzard.GameService.SDK.Client.Integration.BattleNet.SetPresenceBool(1U, false);
  }

  private void WillReset()
  {
    NetCache.Get().DispatchClientOptionsToServer();
    NetCache.Get().Clear();
    this.m_state.DelayedError = (string) null;
    this.m_state.TimeBeforeAllowReset = 0.0f;
    if (this.m_connectApi == null)
      return;
    this.RemoveConnectApiConnectionListeners();
  }

  public void OnReset()
  {
    this.m_state = new Network.NetworkState();
    this.m_state.SetDefaults();
    if (this.m_connectApi != null)
      this.RegisterConnectApiConnectionListeners();
    Network.s_running = true;
    this.ResetForNewAuroraConnection();
  }

  public bool ResetForNewAuroraConnection()
  {
    Log.Offline.PrintDebug("Resetting for new Aurora Connection");
    NetCache.Get().ClearForNewAuroraConnection();
    this.m_state.QueuedClientStateNotifications.Clear();
    this.CloseAll();
    this.m_dispatcherImpl.ResetForNewConnection();
    this.m_inTransitRequests.Clear();
    bool flag = false;
    Blizzard.GameService.SDK.Client.Integration.BattleNet.RequestCloseAurora();
    if (Network.ShouldBeConnectedToAurora())
    {
      string targetServer = Network.GetTargetServer();
      uint port1 = Network.GetPort();
      SslParameters sslParams1 = Network.GetSSLParams();
      int port2 = (int) port1;
      SslParameters sslParams2 = sslParams1;
      flag = Blizzard.GameService.SDK.Client.Integration.BattleNet.Connect(targetServer, (uint) port2, sslParams2);
      Log.Offline.PrintDebug("ResetForNewAuroraConnection: ResetOk={0}", (object) flag);
    }
    if (flag || !Network.ShouldBeConnectedToAurora())
    {
      BnetParty.SetDisconnectedFromBattleNet();
      this.m_connectApi.SetDisconnectedFromBattleNet();
      this.InitializeConnectApi(this.m_dispatcherImpl);
    }
    return flag;
  }

  public static Network Get() => ServiceManager.Get<Network>();

  public static float GetMaxDeferredWait() => Network.m_maxDeferredWait;

  public static string ProductVersion() => 25.ToString() + "." + 0.ToString() + "." + 0.ToString() + "." + 0.ToString();

  private void CreateNewDispatcher()
  {
    IDebugConnectionManager debugConnectionManager = (IDebugConnectionManager) new DebugConnectionManager();
    this.m_dispatcherImpl = (IDispatcher) new QueueDispatcher(debugConnectionManager, (IClientRequestManager) new ClientRequestManager(), (IPacketDecoderManager) new PacketDecoderManager(debugConnectionManager.AllowDebugConnections()), (ISocketEventListener) TelemetryManager.NetworkComponent);
  }

  private void ProcessRequestTimeouts()
  {
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    for (int index = 0; index < this.m_inTransitRequests.Count; ++index)
    {
      Network.RequestContext inTransitRequest = this.m_inTransitRequests[index];
      if (inTransitRequest.m_timeoutHandler != null && (double) inTransitRequest.m_waitUntil < (double) realtimeSinceStartup)
      {
        Debug.LogWarning((object) string.Format("Encountered timeout waiting for {0} {1} {2}", (object) inTransitRequest.m_pendingResponseId, (object) inTransitRequest.m_requestId, (object) inTransitRequest.m_requestSubId));
        inTransitRequest.m_timeoutHandler(inTransitRequest.m_pendingResponseId, inTransitRequest.m_requestId, inTransitRequest.m_requestSubId);
      }
    }
    for (int index = this.m_inTransitRequests.Count - 1; index >= 0; --index)
    {
      if ((double) this.m_inTransitRequests[index].m_waitUntil < (double) realtimeSinceStartup)
        this.m_inTransitRequests.RemoveAt(index);
    }
  }

  public void AddPendingRequestTimeout(int requestId, int requestSubId)
  {
    if (!Network.ShouldBeConnectedToAurora())
      return;
    int num = 0;
    if ((201 != requestId || !Network.m_deferredGetAccountInfoMessageResponseMap.TryGetValue(requestSubId, out num) ? (Network.m_deferredMessageResponseMap.TryGetValue(requestId, out num) ? 1 : 0) : 1) == 0)
      return;
    Network.TimeoutHandler timeoutHandler = (Network.TimeoutHandler) null;
    if (this.m_state.NetTimeoutHandlers.TryGetValue(num, out timeoutHandler))
      this.m_inTransitRequests.Add(new Network.RequestContext(num, requestId, requestSubId, timeoutHandler));
    else
      this.m_inTransitRequests.Add(new Network.RequestContext(num, requestId, requestSubId, new Network.TimeoutHandler(Network.OnRequestTimeout)));
  }

  private void RemovePendingRequestTimeout(int pendingResponseId) => this.m_inTransitRequests.RemoveAll((Predicate<Network.RequestContext>) (pc => pc.m_pendingResponseId == pendingResponseId));

  private static void OnRequestTimeout(int pendingResponseId, int requestId, int requestSubId)
  {
    if (Network.m_deferredMessageResponseMap.ContainsValue(pendingResponseId) || Network.m_deferredGetAccountInfoMessageResponseMap.ContainsValue(pendingResponseId))
    {
      Debug.LogError((object) string.Format("OnRequestTimeout pending ID {0} {1} {2}", (object) pendingResponseId, (object) requestId, (object) requestSubId));
      FatalErrorMgr.Get().SetErrorCode("HS", "NT" + pendingResponseId.ToString(), requestId.ToString(), requestSubId.ToString());
      TelemetryManager.Client().SendNetworkError(NetworkError.ErrorType.TIMEOUT_DEFERRED_RESPONSE, FatalErrorMgr.Get().GetFormattedErrorCode(), 0);
      Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UNKNOWN");
    }
    else
    {
      Debug.LogError((object) string.Format("Unhandled OnRequestTimeout pending ID {0} {1} {2}", (object) pendingResponseId, (object) requestId, (object) requestSubId));
      FatalErrorMgr.Get().SetErrorCode("HS", "NU" + pendingResponseId.ToString(), requestId.ToString(), requestSubId.ToString());
      TelemetryManager.Client().SendNetworkError(NetworkError.ErrorType.TIMEOUT_NOT_DEFERRED_RESPONSE, FatalErrorMgr.Get().GetFormattedErrorCode(), 0);
      Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UNKNOWN");
    }
  }

  private void OnGenericResponse()
  {
    Network.GenericResponse genericResponse = this.GetGenericResponse();
    if (genericResponse == null)
    {
      Debug.LogError((object) string.Format("Login - GenericResponse parse error"));
    }
    else
    {
      int num1 = 201 != genericResponse.RequestId ? 0 : (Network.m_deferredGetAccountInfoMessageResponseMap.ContainsKey(genericResponse.RequestSubId) ? 1 : 0);
      bool flag = Network.m_deferredMessageResponseMap.ContainsKey(genericResponse.RequestId);
      if (num1 == 0 && !flag || Network.GenericResponse.Result.RESULT_REQUEST_IN_PROCESS == genericResponse.ResultCode || Network.GenericResponse.Result.RESULT_DATA_MIGRATION_REQUIRED == genericResponse.ResultCode)
        return;
      Debug.LogError((object) string.Format("Unhandled resultCode {0} for requestId {1}:{2}", (object) genericResponse.ResultCode, (object) genericResponse.RequestId, (object) genericResponse.RequestSubId));
      FatalErrorMgr fatalErrorMgr = FatalErrorMgr.Get();
      string errorSubset1 = "NG" + genericResponse.ResultCode.ToString();
      int num2 = genericResponse.RequestId;
      string errorSubset2 = num2.ToString();
      num2 = genericResponse.RequestSubId;
      string errorSubset3 = num2.ToString();
      fatalErrorMgr.SetErrorCode("HS", errorSubset1, errorSubset2, errorSubset3);
      TelemetryManager.Client().SendNetworkError(NetworkError.ErrorType.REQUEST_ERROR, FatalErrorMgr.Get().GetFormattedErrorCode(), 0);
      this.ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UNKNOWN");
    }
  }

  public static bool IsRunning() => Network.s_running;

  public double TimeSinceLastPong() => !this.IsConnectedToGameServer() || this.m_gameServerKeepAliveFrequencySeconds == 0U || this.m_connectApi.GetTimeLastPingSent() <= this.m_connectApi.GetTimeLastPingReceieved() ? 0.0 : (double) Time.realtimeSinceStartup - this.m_connectApi.GetTimeLastPingReceieved();

  private void OnSubscribeResponse()
  {
    SubscribeResponse subscribeResponse = this.m_connectApi.GetSubscribeResponse();
    if (subscribeResponse == null || !subscribeResponse.HasRequestMaxWaitSecs || subscribeResponse.RequestMaxWaitSecs < 30UL)
      return;
    Network.m_maxDeferredWait = (float) subscribeResponse.RequestMaxWaitSecs;
  }

  private void OnClientStateNotification()
  {
    ClientStateNotification stateNotification = this.m_connectApi.GetClientStateNotification();
    if (!NetCache.Get().HasReceivedInitialClientState)
    {
      this.m_state.QueuedClientStateNotifications.Add(stateNotification);
      TelemetryManager.Client().SendInitialClientStateOutOfOrder(stateNotification.HasAchievementNotifications ? stateNotification.AchievementNotifications.AchievementNotifications_.Count : 0, stateNotification.HasNoticeNotifications ? stateNotification.NoticeNotifications.NoticeNotifications_.Count : 0, stateNotification.HasCollectionModifications ? stateNotification.CollectionModifications.CardModifications.Sum<CardModification>((Func<CardModification, int>) (m => m.Quantity)) : 0, stateNotification.HasCurrencyState ? 1 : 0, stateNotification.HasBoosterModifications ? stateNotification.BoosterModifications.Modifications.Sum<BoosterInfo>((Func<BoosterInfo, int>) (m => m.Count)) : 0, stateNotification.HasHeroXp ? stateNotification.HeroXp.XpInfos.Count : 0, stateNotification.HasPlayerRecords ? stateNotification.PlayerRecords.Records.Count : 0, stateNotification.HasArenaSessionResponse ? 1 : 0, stateNotification.HasCardBackModifications ? stateNotification.CardBackModifications.CardBackModifications_.Count : 0);
    }
    else
      Network.ProcessClientStateNotification(stateNotification);
  }

  public static void ProcessClientStateNotification(ClientStateNotification packet)
  {
    if (packet.HasCurrencyState)
      NetCache.Get().OnCurrencyState(packet.CurrencyState);
    if (packet.HasCollectionModifications)
    {
      NetCache.Get().OnCollectionModification(packet);
    }
    else
    {
      if (packet.HasAchievementNotifications)
        AchieveManager.Get().OnAchievementNotifications(packet.AchievementNotifications.AchievementNotifications_);
      if (packet.HasNoticeNotifications)
        Network.Get().OnNoticeNotifications(packet.NoticeNotifications);
      if (packet.HasBoosterModifications)
        NetCache.Get().OnBoosterModifications(packet.BoosterModifications);
    }
    if (packet.HasBattlegroundsGuideSkinModifications)
      NetCache.Get().OnBattlegroundsGuideSkinModifications(packet.BattlegroundsGuideSkinModifications);
    if (packet.HasBattlegroundsHeroSkinModifications)
      NetCache.Get().OnBattlegroundsHeroSkinModifications(packet.BattlegroundsHeroSkinModifications);
    if (packet.HasBattlegroundsBoardSkinModifications)
      NetCache.Get().OnBattlegroundsBoardSkinModifications(packet.BattlegroundsBoardSkinModifications);
    if (packet.HasBattlegroundsFinisherModifications)
      NetCache.Get().OnBattlegroundsFinisherModifications(packet.BattlegroundsFinisherModifications);
    if (packet.HasBattlegroundsEmoteModifications)
      NetCache.Get().OnBattlegroundsEmoteModifications(packet.BattlegroundsEmoteModifications);
    if (packet.HasHeroXp)
      NetCache.Get().OnHeroXP(packet.HeroXp);
    if (packet.HasPlayerRecords)
      NetCache.Get().OnPlayerRecordsPacket(packet.PlayerRecords);
    if (packet.HasArenaSessionResponse)
      DraftManager.Get().OnArenaSessionResponsePacket(packet.ArenaSessionResponse);
    if (packet.HasCardBackModifications)
      NetCache.Get().OnCardBackModifications(packet.CardBackModifications);
    if (!packet.HasPlayerDraftTickets)
      return;
    NetCache.Get().OnPlayerDraftTickets(packet.PlayerDraftTickets);
  }

  public void OnInitialClientStateProcessed()
  {
    List<ClientStateNotification> stateNotificationList = new List<ClientStateNotification>((IEnumerable<ClientStateNotification>) this.m_state.QueuedClientStateNotifications);
    this.m_state.QueuedClientStateNotifications.Clear();
    foreach (ClientStateNotification packet in stateNotificationList)
      Network.ProcessClientStateNotification(packet);
  }

  public void OnNoticeNotifications(NoticeNotifications packet)
  {
    List<PegasusUtil.ProfileNotice> notices = new List<PegasusUtil.ProfileNotice>();
    List<NetCache.ProfileNotice> result = new List<NetCache.ProfileNotice>();
    for (int index = 0; index < packet.NoticeNotifications_.Count; ++index)
    {
      NoticeNotification noticeNotification = packet.NoticeNotifications_[index];
      notices.Add(noticeNotification.Notice);
    }
    this.HandleProfileNotices(notices, ref result);
    NetCache.Get().HandleIncomingProfileNotices(result, false);
  }

  private void RegisterConnectApiConnectionListeners()
  {
    this.m_connectApi.RegisterGameServerConnectEventListener(new System.Action<BattleNetErrors>(this.OnGameServerConnectEvent));
    this.m_connectApi.RegisterGameServerDisconnectEventListener(new System.Action<BattleNetErrors>(this.OnGameServerDisconnectEvent));
    this.m_connectApi.RegisterIPv6ConversionEventListener(new System.Action<string, string>(this.OnIPv6ConversionEvent));
  }

  private void RemoveConnectApiConnectionListeners()
  {
    this.m_connectApi.RemoveGameServerConnectEventListener(new System.Action<BattleNetErrors>(this.OnGameServerConnectEvent));
    this.m_connectApi.RemoveGameServerDisconnectEventListener(new System.Action<BattleNetErrors>(this.OnGameServerDisconnectEvent));
    this.m_connectApi.RemoveIPv6ConversionEventListener(new System.Action<string, string>(this.OnIPv6ConversionEvent));
  }

  public void UpdateCachedBnetValues()
  {
    this.m_state.CachedGameAccountId = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetMyGameAccountId();
    this.m_state.CachedRegion = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetCurrentRegion();
  }

  public void OverrideKeepAliveSeconds(uint value)
  {
    if (!HearthstoneApplication.IsInternal())
      return;
    this.m_gameServerKeepAliveFrequencySeconds = value;
  }

  public BnetGameAccountId GetMyGameAccountId()
  {
    BnetGameAccountId myGameAccountId = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetMyGameAccountId();
    return myGameAccountId.High == 0UL && myGameAccountId.Low == 0UL ? this.m_state.CachedGameAccountId : myGameAccountId;
  }

  public BnetRegion GetCurrentRegion()
  {
    BnetRegion currentRegion = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetCurrentRegion();
    return currentRegion == BnetRegion.REGION_UNINITIALIZED ? this.m_state.CachedRegion : currentRegion;
  }

  private void InitializeConnectApi(IDispatcher dispatcher)
  {
    this.m_errorList.Clear();
    if (this.m_connectApi == null)
    {
      Network.GameServerDisconnectEvents = new List<BattleNetErrors>();
      this.m_connectApi = new ConnectAPI(dispatcher);
      this.RegisterConnectApiConnectionListeners();
    }
    this.m_connectApi.SetGameStartState(GameStartState.Invalid);
  }

  public static void ApplicationPaused()
  {
    if (NetCache.Get() != null)
      NetCache.Get().DispatchClientOptionsToServer();
    Network service;
    if (ServiceManager.TryGet<Network>(out service) && service.m_connectApi != null)
      service.m_connectApi.ProcessUtilPackets();
    Blizzard.GameService.SDK.Client.Integration.BattleNet.ApplicationWasPaused();
  }

  public void CloseAll()
  {
    if (this.m_ackCardSeenPacket.CardDefs.Count != 0)
      this.SendAckCardsSeen();
    this.CheckForSendingBattlegroundsSkinsSeenPacket(1);
    if (this.m_connectApi == null)
      return;
    this.m_connectApi.Close();
  }

  public static void ApplicationUnpaused() => Blizzard.GameService.SDK.Client.Integration.BattleNet.ApplicationWasUnpaused();

  public void Update()
  {
    if (!Network.s_running)
      return;
    this.ProcessRequestTimeouts();
    this.ProcessNetworkReachability();
    this.ProcessConnectApiHeartbeat();
    StoreManager.Get().Heartbeat();
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    float sec = realtimeSinceStartup - this.m_state.LastCall;
    if ((double) sec < (double) Network.PROCESS_WARNING || (double) realtimeSinceStartup - (double) this.m_state.LastCallReport < (double) Network.PROCESS_WARNING_REPORT_GAP)
      return;
    this.m_state.LastCallReport = realtimeSinceStartup;
    Debug.LogWarning((object) string.Format("Network.ProcessNetwork not called for {0}", (object) TimeUtils.GetDevElapsedTimeString(sec)));
  }

  private void ProcessConnectApiHeartbeat()
  {
    this.GetBattleNetPackets();
    int count = this.m_errorList.Count;
    for (int index = 0; index < count; ++index)
    {
      Network.ConnectErrorParams error = this.m_errorList[index];
      if (error == null)
        Debug.LogError((object) ("null error! " + (object) this.m_errorList.Count));
      else if ((double) Time.realtimeSinceStartup >= (double) error.m_creationTime + 0.400000005960464)
      {
        this.m_errorList.RemoveAt(index);
        --index;
        count = this.m_errorList.Count;
        Error.AddFatal((ErrorParams) error);
      }
    }
    if (this.m_connectApi == null)
      return;
    if (this.m_connectApi.HasGameServerConnection())
    {
      this.m_connectApi.UpdateGameServerConnection();
      this.UpdatePingPong();
    }
    this.m_connectApi.ProcessUtilPackets();
    if (!this.m_connectApi.TryConnectDebugConsole())
      return;
    this.m_connectApi.UpdateDebugConsole();
  }

  private void ProcessNetworkReachability()
  {
    if (!Network.IsLoggedIn())
      return;
    if (!this.m_networkReachabilityManager.InternetAvailable_Cached)
    {
      if (this.IsInGame())
      {
        double totalSeconds = TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;
        if (this.m_timeInternetUnreachable == 0.0)
        {
          this.m_timeInternetUnreachable = totalSeconds;
          return;
        }
        if (totalSeconds - this.m_timeInternetUnreachable < (double) this.m_gameServerKeepAliveWaitForInternetSeconds)
          return;
      }
      Log.Offline.PrintError("Network.ProcessInternetReachability(): Access to the Internet has been lost.");
      Error.AddFatal(FatalErrorReason.NO_INTERNET_ACCESS, "GLOBAL_ERROR_NETWORK_DISCONNECT");
    }
    else
    {
      if (this.m_timeInternetUnreachable != 0.0)
      {
        double totalSeconds = TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;
        TelemetryManager.Client().SendNetworkUnreachableRecovered((int) (totalSeconds - this.m_timeInternetUnreachable));
        if (this.IsInGame())
          this.DisconnectFromGameServer();
      }
      this.m_timeInternetUnreachable = 0.0;
    }
  }

  public void AddErrorToList(Network.ConnectErrorParams errorParams) => this.m_errorList.Add(errorParams);

  public void SetShouldIgnorePong(bool value) => this.m_connectApi.SetShouldIgnorePong(value);

  public void SetSpoofDisconnected(bool value) => this.m_connectApi.SetSpoofDisconnected(value);

  private bool IsInGame() => GameState.Get() != null;

  private void UpdatePingPong()
  {
    if (this.m_gameServerKeepAliveFrequencySeconds <= 0U)
      return;
    double totalSeconds = TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;
    if (!this.m_connectApi.IsConnectedToGameServer() || totalSeconds - this.m_connectApi.GetTimeLastPingSent() <= (double) this.m_gameServerKeepAliveFrequencySeconds)
      return;
    int pingsSinceLastPong = this.m_connectApi.GetPingsSinceLastPong();
    if (this.m_connectApi.GetTimeLastPingSent() <= this.m_connectApi.GetTimeLastPingReceieved())
      this.m_connectApi.SetTimeLastPingReceived(totalSeconds - 0.001);
    this.m_connectApi.SetTimeLastPingSent(totalSeconds);
    this.m_connectApi.SendPing();
    if ((long) pingsSinceLastPong >= (long) this.m_gameServerKeepAliveRetry)
    {
      this.DisconnectFromGameServer();
      this.SetShouldIgnorePong(false);
    }
    this.m_connectApi.SetPingsSinceLastPong(pingsSinceLastPong + 1);
  }

  private void GetBattleNetPackets()
  {
    UtilResponse utilResponse;
    while ((utilResponse = Blizzard.GameService.SDK.Client.Integration.BattleNet.NextUtilPacket()) != null)
      this.m_connectApi.DecodeAndProcessPacket(new PegasusPacket(utilResponse.type, utilResponse.bytes.Length, (object) utilResponse.bytes)
      {
        Context = utilResponse.context
      });
  }

  public void AppAbort()
  {
    if (!Network.s_running)
      return;
    NetCache.Get().DispatchClientOptionsToServer();
    PresenceMgr.Get().OnShutdown();
    this.CancelFindGame();
    this.CloseAll();
    this.ClearTransientBnetPresence();
    Blizzard.GameService.SDK.Client.Integration.BattleNet.AppQuit();
    BnetRecentPlayerMgr.Get().Shutdown();
    BnetNearbyPlayerMgr.Get().Shutdown();
    Network.s_running = false;
  }

  public void ResetConnectionFailureCount() => this.m_numConnectionFailures = 0;

  public bool RegisterNetHandler(
    object enumId,
    Network.NetHandler handler,
    Network.TimeoutHandler timeoutHandler = null)
  {
    int key = (int) enumId;
    if (timeoutHandler != null)
    {
      if (this.m_state.NetTimeoutHandlers.ContainsKey(key))
        return false;
      this.m_state.NetTimeoutHandlers.Add(key, timeoutHandler);
    }
    List<Network.NetHandler> netHandlerList;
    if (this.m_netHandlers.TryGetValue(key, out netHandlerList))
    {
      if (netHandlerList.Contains(handler))
        return false;
    }
    else
    {
      netHandlerList = new List<Network.NetHandler>();
      this.m_netHandlers.Add(key, netHandlerList);
    }
    netHandlerList.Add(handler);
    return true;
  }

  public bool RemoveNetHandler(object enumId, Network.NetHandler handler)
  {
    List<Network.NetHandler> netHandlerList;
    return this.m_netHandlers.TryGetValue((int) enumId, out netHandlerList) && netHandlerList.Remove(handler);
  }

  public void RegisterThrottledPacketListener(Network.ThrottledPacketListener listener)
  {
    if (this.m_throttledPacketListeners.Contains(listener))
      return;
    this.m_throttledPacketListeners.Add(listener);
  }

  public void RegisterGameQueueHandler(Network.GameQueueHandler handler)
  {
    if (this.m_gameQueueHandler != null)
      Log.Net.Print("handler {0} would bash game queue handler {1}", (object) handler, (object) this.m_gameQueueHandler);
    else
      this.m_gameQueueHandler = handler;
  }

  public void RegisterQueueInfoHandler(Network.QueueInfoHandler handler)
  {
    if (this.m_queueInfoHandler != null)
      Log.Net.Print("handler {0} would bash queue info handler {1}", (object) handler, (object) this.m_queueInfoHandler);
    else
      this.m_queueInfoHandler = handler;
  }

  public bool FakeHandleType(System.Enum enumId) => this.FakeHandleType(Convert.ToInt32((object) enumId));

  public bool FakeHandleType(int id)
  {
    if (Network.ShouldBeConnectedToAurora())
      return false;
    this.HandleType(id);
    return true;
  }

  private bool HandleType(int id)
  {
    this.RemovePendingRequestTimeout(id);
    List<Network.NetHandler> netHandlerList;
    if (!this.m_netHandlers.TryGetValue(id, out netHandlerList) || netHandlerList.Count == 0)
    {
      if (!this.CanIgnoreUnhandledPacket(id))
        Debug.LogError((object) string.Format("Network.HandleType() - Received packet {0}, but there are no handlers for it.", (object) id));
      return false;
    }
    foreach (Network.NetHandler netHandler in netHandlerList.ToArray())
      netHandler();
    return true;
  }

  private bool CanIgnoreUnhandledPacket(int id) => id == 15 || id == 116 || id == 254;

  private bool ProcessGameQueue()
  {
    QueueEvent queueEvent = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetQueueEvent();
    if (queueEvent == null)
      return false;
    switch (queueEvent.EventType)
    {
      case QueueEvent.Type.QUEUE_LEAVE:
      case QueueEvent.Type.QUEUE_DELAY_ERROR:
      case QueueEvent.Type.QUEUE_AMM_ERROR:
      case QueueEvent.Type.QUEUE_CANCEL:
      case QueueEvent.Type.QUEUE_GAME_STARTED:
      case QueueEvent.Type.ABORT_CLIENT_DROPPED:
        this.m_state.FindingBnetGameType = BnetGameType.BGT_UNKNOWN;
        break;
    }
    if (this.m_gameQueueHandler == null)
      Debug.LogWarningFormat("m_gameQueueHandler is null in Network.ProcessGameQueue! event={0} server={1}:{2} gameHandle={3} clientHandle={4}", (object) queueEvent.EventType, queueEvent.GameServer == null ? (object) "null" : (object) queueEvent.GameServer.Address, (object) (uint) (queueEvent.GameServer == null ? 0 : (int) queueEvent.GameServer.Port), (object) (uint) (queueEvent.GameServer == null ? 0 : (int) queueEvent.GameServer.GameHandle), (object) (queueEvent.GameServer == null ? 0L : queueEvent.GameServer.ClientHandle));
    else
      this.m_gameQueueHandler(queueEvent);
    return true;
  }

  private bool ProcessGameServer()
  {
    int num = this.HandleType(this.NextGamePacketType()) ? 1 : 0;
    this.m_connectApi.DropGamePacket();
    return num != 0;
  }

  private bool ProcessUtilServer()
  {
    int num = this.HandleType(this.m_connectApi.NextUtilPacketType()) ? 1 : 0;
    this.m_connectApi.DropUtilPacket();
    return num != 0;
  }

  private bool ProcessConsole()
  {
    int num = this.HandleType(this.m_connectApi.NextDebugPacketType()) ? 1 : 0;
    this.m_connectApi.DropDebugPacket();
    return num != 0;
  }

  public Network.UnavailableReason GetHearthstoneUnavailable(bool gamePacket)
  {
    Network.UnavailableReason hearthstoneUnavailable = new Network.UnavailableReason();
    if (gamePacket)
    {
      Deadend deadendGame = this.m_connectApi.GetDeadendGame();
      hearthstoneUnavailable.mainReason = deadendGame.Reply1;
      hearthstoneUnavailable.subReason = deadendGame.Reply2;
      hearthstoneUnavailable.extraData = deadendGame.Reply3;
    }
    else
    {
      DeadendUtil deadendUtil = this.m_connectApi.GetDeadendUtil();
      hearthstoneUnavailable.mainReason = deadendUtil.Reply1;
      hearthstoneUnavailable.subReason = deadendUtil.Reply2;
      hearthstoneUnavailable.extraData = deadendUtil.Reply3;
    }
    return hearthstoneUnavailable;
  }

  public void CraftingTransaction(
    CraftingPendingTransaction transaction,
    int expectedTransactionCost,
    int normalOwned,
    int goldenOwned)
  {
    PegasusShared.CardDef cardDef = new PegasusShared.CardDef()
    {
      Asset = GameUtils.TranslateCardIdToDbId(transaction.CardID),
      Premium = (int) transaction.Premium
    };
    this.m_connectApi.CraftingTransaction(transaction, cardDef, expectedTransactionCost, normalOwned, goldenOwned);
  }

  public void SetClientOptions(SetOptions packet) => this.m_connectApi.SetClientOptions(packet);

  public static ConnectionState BattleNetStatus() => Blizzard.GameService.SDK.Client.Integration.BattleNet.BattleNetStatus();

  public static bool IsLoggedIn() => Blizzard.GameService.SDK.Client.Integration.BattleNet.IsInitialized() && Blizzard.GameService.SDK.Client.Integration.BattleNet.BattleNetStatus() == ConnectionState.Ready;

  public bool HaveUnhandledPackets() => this.m_connectApi.HasUtilPackets() || this.m_connectApi.HasGamePackets() || this.m_connectApi.HasDebugPackets() || Blizzard.GameService.SDK.Client.Integration.BattleNet.GetNotificationCount() > 0;

  public int NextGamePacketType() => this.m_connectApi.NextGamePacketType();

  public void ProcessNetwork()
  {
    if (!Network.s_running || this.m_state.LastCallFrame == Time.frameCount)
      return;
    this.m_state.LastCallFrame = Time.frameCount;
    this.m_state.LastCall = Time.realtimeSinceStartup;
    Network.s_urlDownloader.Process();
    if (Network.ShouldBeConnectedToAurora())
      this.ProcessAurora();
    else
      this.ProcessDelayedError();
    if (this.ProcessGameQueue())
      return;
    if (this.m_connectApi.HasGamePackets())
    {
      this.ProcessGameServer();
    }
    else
    {
      if (Network.GameServerDisconnectEvents != null && Network.GameServerDisconnectEvents.Count > 0)
      {
        foreach (BattleNetErrors errorCode in Network.GameServerDisconnectEvents.ToArray())
        {
          Network.GameServerDisconnectEvent disconnectEventListener = this.m_state.GameServerDisconnectEventListener;
          if (disconnectEventListener != null)
            disconnectEventListener(errorCode);
        }
        Network.GameServerDisconnectEvents.Clear();
      }
      if (this.m_connectApi.HasUtilPackets())
        this.ProcessUtilServer();
      else if (this.m_connectApi.HasDebugPackets())
        this.ProcessConsole();
      else
        this.ProcessQueuePosition();
    }
  }

  public static void StartInitalBattleNetConnection()
  {
    if (Blizzard.GameService.SDK.Client.Integration.BattleNet.IsInitialized())
    {
      Log.Net.PrintDebug("Tried to connect to battle.net when already initialized");
    }
    else
    {
      string targetServer = Network.GetTargetServer();
      uint port1 = Network.GetPort();
      SslParameters sslParams1 = Network.GetSSLParams();
      Blizzard.GameService.SDK.Client.Integration.BattleNet.SetImpl(Network.CreateBattleNetImplementation());
      int port2 = (int) port1;
      SslParameters sslParams2 = sslParams1;
      Blizzard.GameService.SDK.Client.Integration.BattleNet.Connect(targetServer, (uint) port2, sslParams2);
    }
  }

  private void InitBattleNet(IDispatcher dispatcher)
  {
    if (Blizzard.GameService.SDK.Client.Integration.BattleNet.IsInitialized())
      return;
    if (Blizzard.GameService.SDK.Client.Integration.BattleNet.Get() == null)
      Blizzard.GameService.SDK.Client.Integration.BattleNet.SetImpl(Network.CreateBattleNetImplementation());
    this.AddBnetErrorListener(BnetFeature.Auth, new Network.BnetErrorCallback(this.OnBnetAuthError));
    this.InitializeConnectApi(dispatcher);
  }

  private static IBattleNet CreateBattleNetImplementation()
  {
    ClientInterface clientInterface = (ClientInterface) new Network.HSClientInterface();
    LoggerInterface loggerInterface = Network.BuildLoggerInterface();
    BattleNetCSharp netImplementation = new BattleNetCSharp(clientInterface, loggerInterface, (ISocketEventListener) TelemetryManager.NetworkComponent);
    Debug.LogFormat("*** BattleNet version: Product = {0}, Data = {1}", (object) clientInterface.GetVersion(), (object) clientInterface.GetDataVersion());
    return (IBattleNet) netImplementation;
  }

  private static LoggerInterface BuildLoggerInterface()
  {
    Logger fullLogger = LogSystem.Get().GetFullLogger("BattleNet");
    return BattleNetLoggerBuilder.BuildLoggerInterface(TelemetryManager.Client(), fullLogger, (IBnetErrorReporter) new BnetErrorAdaptor());
  }

  private void OnConnectedToBattleNetCallback(BattleNetErrors error) => TelemetryManager.OnBattleNetConnect(Blizzard.GameService.SDK.Client.Integration.BattleNet.GetEnvironment(), (int) Blizzard.GameService.SDK.Client.Integration.BattleNet.GetPort(), error);

  private void OnDisconnectedFromBattleNetCallback(BattleNetErrors error) => TelemetryManager.OnBattleNetDisconnect(Blizzard.GameService.SDK.Client.Integration.BattleNet.GetEnvironment(), (int) Blizzard.GameService.SDK.Client.Integration.BattleNet.GetPort(), error);

  public static bool ShouldBeConnectedToAurora() => Network.s_shouldBeConnectedToAurora;

  public static void SetShouldBeConnectedToAurora(bool shouldBeConnected) => Network.s_shouldBeConnectedToAurora = shouldBeConnected;

  public bool ShouldBeConnectedToAurora_NONSTATIC() => Network.s_shouldBeConnectedToAurora;

  public void ProcessQueuePosition()
  {
    Blizzard.GameService.SDK.Client.Integration.QueueInfo queueInfo = new Blizzard.GameService.SDK.Client.Integration.QueueInfo();
    Blizzard.GameService.SDK.Client.Integration.BattleNet.GetQueueInfo(ref queueInfo);
    if (!queueInfo.changed || this.m_queueInfoHandler == null)
      return;
    this.m_queueInfoHandler(new Network.QueueInfo()
    {
      position = queueInfo.position,
      secondsTilEnd = queueInfo.end,
      stdev = queueInfo.stdev
    });
  }

  public void SetFriendsHandler(Network.FriendsHandler handler) => this.m_state.CurrentFriendsHandler = handler;

  public void SetWhisperHandler(Network.WhisperHandler handler) => this.m_state.CurrentWhisperHandler = handler;

  public void SetPresenceHandler(Network.PresenceHandler handler) => this.m_state.CurrentPresenceHandler = handler;

  public void SetShutdownHandler(Network.ShutdownHandler handler) => this.m_state.CurrentShutdownHandler = handler;

  public void SetGameServerDisconnectEventListener(Network.GameServerDisconnectEvent handler) => this.m_state.GameServerDisconnectEventListener = handler;

  public void RemoveGameServerDisconnectEventListener(Network.GameServerDisconnectEvent handler)
  {
    if (!(this.m_state.GameServerDisconnectEventListener == handler))
      return;
    this.m_state.GameServerDisconnectEventListener = (Network.GameServerDisconnectEvent) null;
  }

  public void AddBnetErrorListener(BnetFeature feature, Network.BnetErrorCallback callback) => this.AddBnetErrorListener(feature, callback, (object) null);

  public void AddBnetErrorListener(
    BnetFeature feature,
    Network.BnetErrorCallback callback,
    object userData)
  {
    Network.BnetErrorListener bnetErrorListener = new Network.BnetErrorListener();
    bnetErrorListener.SetCallback(callback);
    bnetErrorListener.SetUserData(userData);
    List<Network.BnetErrorListener> bnetErrorListenerList;
    if (!this.m_state.FeatureBnetErrorListeners.TryGetValue(feature, out bnetErrorListenerList))
    {
      bnetErrorListenerList = new List<Network.BnetErrorListener>();
      this.m_state.FeatureBnetErrorListeners.Add(feature, bnetErrorListenerList);
    }
    else if (bnetErrorListenerList.Contains(bnetErrorListener))
      return;
    bnetErrorListenerList.Add(bnetErrorListener);
  }

  public void AddBnetErrorListener(Network.BnetErrorCallback callback) => this.AddBnetErrorListener(callback, (object) null);

  public void AddBnetErrorListener(Network.BnetErrorCallback callback, object userData)
  {
    Network.BnetErrorListener bnetErrorListener = new Network.BnetErrorListener();
    bnetErrorListener.SetCallback(callback);
    bnetErrorListener.SetUserData(userData);
    if (this.m_state.GlobalBnetErrorListeners.Contains(bnetErrorListener))
      return;
    this.m_state.GlobalBnetErrorListeners.Add(bnetErrorListener);
  }

  public bool RemoveBnetErrorListener(BnetFeature feature, Network.BnetErrorCallback callback) => this.RemoveBnetErrorListener(feature, callback, (object) null);

  public bool RemoveBnetErrorListener(
    BnetFeature feature,
    Network.BnetErrorCallback callback,
    object userData)
  {
    List<Network.BnetErrorListener> bnetErrorListenerList;
    if (!this.m_state.FeatureBnetErrorListeners.TryGetValue(feature, out bnetErrorListenerList))
      return false;
    Network.BnetErrorListener bnetErrorListener = new Network.BnetErrorListener();
    bnetErrorListener.SetCallback(callback);
    bnetErrorListener.SetUserData(userData);
    return bnetErrorListenerList.Remove(bnetErrorListener);
  }

  public bool RemoveBnetErrorListener(Network.BnetErrorCallback callback) => this.RemoveBnetErrorListener(callback, (object) null);

  public bool RemoveBnetErrorListener(Network.BnetErrorCallback callback, object userData)
  {
    Network.BnetErrorListener bnetErrorListener = new Network.BnetErrorListener();
    bnetErrorListener.SetCallback(callback);
    bnetErrorListener.SetUserData(userData);
    return this.m_state.GlobalBnetErrorListeners.Remove(bnetErrorListener);
  }

  public void SendUnsubcribeRequest(Unsubscribe packet, UtilSystemId systemChannel) => this.m_connectApi.SendUnsubscribeRequest(packet, systemChannel);

  public void ProcessAurora()
  {
    this.ProcessBnetEvents();
    if (Network.IsLoggedIn())
    {
      this.ProcessPresence();
      this.ProcessFriends();
      this.ProcessWhispers();
      this.ProcessParties();
      this.ProcessBroadcasts();
      this.ProcessNotifications();
      BnetRecentPlayerMgr.Get().Update();
      BnetNearbyPlayerMgr.Get().Update();
    }
    this.ProcessErrors();
  }

  private void ProcessBnetEvents()
  {
    Blizzard.GameService.SDK.Client.Integration.BattleNet.TakeBnetEvents(this.m_bnetEvents);
    foreach (BnetEvent bnetEvent in this.m_bnetEvents)
    {
      switch (bnetEvent.EventType)
      {
        case ConnectionState.Disconnected:
          this.OnDisconnectedFromBattleNet(bnetEvent.EventData);
          continue;
        case ConnectionState.Ready:
          this.OnConnectedToBattleNet(bnetEvent.EventData);
          continue;
        default:
          continue;
      }
    }
    this.m_bnetEvents.Clear();
  }

  private void ProcessWhispers()
  {
    if (this.m_state.CurrentWhisperHandler == null)
      return;
    Blizzard.GameService.SDK.Client.Integration.BattleNet.TakeWhispers(this.m_bnetWhispers);
    if (this.m_bnetWhispers.Count <= 0)
      return;
    this.m_state.CurrentWhisperHandler(this.m_bnetWhispers.ToArray());
    this.m_bnetWhispers.Clear();
  }

  private void ProcessParties() => BnetParty.Process();

  private void ProcessBroadcasts()
  {
    int shutdownMinutes = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetShutdownMinutes();
    if (shutdownMinutes <= 0 || this.m_state.CurrentShutdownHandler == null)
      return;
    this.m_state.CurrentShutdownHandler(shutdownMinutes);
  }

  private void ProcessNotifications()
  {
    Blizzard.GameService.SDK.Client.Integration.BattleNet.TakeNotifications(ref this.m_bnetNotifications);
    for (int index = 0; index < this.m_bnetNotifications.Count; ++index)
    {
      BnetNotification bnetNotification = this.m_bnetNotifications[index];
      if (bnetNotification.NotificationType == "WTCG.UtilNotificationMessage")
        this.m_connectApi.DecodeAndProcessPacket(new PegasusPacket(bnetNotification.MessageType, 0, bnetNotification.MessageSize, (object) bnetNotification.BlobMessage));
    }
    this.m_bnetNotifications.Clear();
  }

  private void ProcessFriends()
  {
    if (this.m_state.CurrentFriendsHandler == null)
      return;
    Blizzard.GameService.SDK.Client.Integration.BattleNet.TakeFriendsUpdates(this.m_friendsUpdates);
    if (this.m_friendsUpdates.Count <= 0)
      return;
    this.m_state.CurrentFriendsHandler(this.m_friendsUpdates.ToArray());
    this.m_friendsUpdates.Clear();
  }

  private void ProcessPresence()
  {
    if (this.m_state.CurrentPresenceHandler == null)
      return;
    Blizzard.GameService.SDK.Client.Integration.BattleNet.TakePresence(this.m_presenceUpdates);
    if (this.m_presenceUpdates.Count <= 0)
      return;
    this.m_state.CurrentPresenceHandler(this.m_presenceUpdates.ToArray());
    this.m_presenceUpdates.Clear();
  }

  private void ProcessErrors()
  {
    this.ProcessDelayedError();
    if (this.m_connectApi.HasErrors())
    {
      this.m_bnetErrors.Add(new BnetErrorInfo(BnetFeature.Games, BnetFeatureEvent.Games_OnClientRequest, BattleNetErrors.ERROR_GAME_UTILITY_SERVER_NO_SERVER));
    }
    else
    {
      Blizzard.GameService.SDK.Client.Integration.BattleNet.TakeErrors(this.m_bnetErrors);
      if (this.m_bnetErrors.Count == 0)
        return;
    }
    for (int index = 0; index < this.m_bnetErrors.Count; ++index)
    {
      BnetErrorInfo bnetError = this.m_bnetErrors[index];
      BattleNetErrors error = bnetError.GetError();
      if (error == (BattleNetErrors) 1003013)
      {
        Blizzard.GameService.SDK.Client.Integration.BattleNet.ClearErrors();
        HearthstoneApplication.Get().Reset();
        return;
      }
      string str = HearthstoneApplication.IsPublic() ? "" : error.ToString();
      if (!this.m_connectApi.HasErrors() && this.m_connectApi.ShouldIgnoreError(bnetError))
      {
        if (!HearthstoneApplication.IsPublic())
          Log.BattleNet.PrintDebug("BattleNet/ConnectDLL generated error={0} {1} (can ignore)", (object) (int) error, (object) str);
      }
      else if (!this.FireErrorListeners(bnetError) && (this.m_connectApi.HasErrors() || !this.OnIgnorableBnetError(bnetError)))
        this.OnFatalBnetError(bnetError);
    }
    this.m_bnetErrors.Clear();
  }

  private bool FireErrorListeners(BnetErrorInfo info)
  {
    bool flag = false;
    List<Network.BnetErrorListener> bnetErrorListenerList;
    if (this.m_state.FeatureBnetErrorListeners.TryGetValue(info.GetFeature(), out bnetErrorListenerList) && bnetErrorListenerList.Count > 0)
    {
      foreach (Network.BnetErrorListener bnetErrorListener in bnetErrorListenerList.ToArray())
        flag = bnetErrorListener.Fire(info) | flag;
    }
    foreach (Network.BnetErrorListener bnetErrorListener in this.m_state.GlobalBnetErrorListeners.ToArray())
      flag = bnetErrorListener.Fire(info) | flag;
    return flag;
  }

  public void ShowConnectionFailureError(string error) => this.ShowBreakingNewsOrError(error, this.DelayForConnectionFailures(this.m_numConnectionFailures++));

  public void ShowBreakingNewsOrError(string error, float timeBeforeAllowReset = 0.0f)
  {
    this.m_state.DelayedError = error;
    this.m_state.TimeBeforeAllowReset = timeBeforeAllowReset;
    Debug.LogError((object) string.Format("Setting delayed error for Error Message: {0} and prevent reset for {1} seconds", (object) error, (object) timeBeforeAllowReset));
    this.ProcessDelayedError();
  }

  private bool ProcessDelayedError()
  {
    if (this.m_state.DelayedError == null)
      return false;
    bool flag = false;
    if (this.m_breakingNews.GetStatus() != 0)
    {
      ErrorParams parms = new ErrorParams();
      parms.m_delayBeforeNextReset = this.m_state.TimeBeforeAllowReset;
      string text = this.m_breakingNews.GetText();
      if (string.IsNullOrEmpty(text))
      {
        if (this.m_breakingNews.GetError() != null && this.m_state.DelayedError == "GLOBAL_ERROR_NETWORK_NO_GAME_SERVER")
          parms.m_message = GameStrings.Format("GLOBAL_ERROR_NETWORK_NO_CONNECTION");
        else if (HearthstoneApplication.IsInternal() && this.m_state.DelayedError == "GLOBAL_ERROR_UNKNOWN_ERROR")
        {
          parms.m_message = "Dev Message: Could not connect to Battle.net, and there was no breaking news to display. Maybe Battle.net is down?";
        }
        else
        {
          parms.m_message = GameStrings.Format(this.m_state.DelayedError);
          if (this.m_state.DelayedError == "GLOBAL_MOBILE_ERROR_GAMESERVER_CONNECT")
            parms.m_reason = FatalErrorReason.MOBILE_GAME_SERVER_RPC_ERROR;
        }
      }
      else
      {
        parms.m_message = GameStrings.Format("GLOBAL_MOBILE_ERROR_BREAKING_NEWS", (object) text);
        parms.m_reason = FatalErrorReason.BREAKING_NEWS;
      }
      Error.AddFatal(parms);
      this.m_state.DelayedError = (string) null;
      this.m_state.TimeBeforeAllowReset = 0.0f;
      flag = true;
    }
    return flag;
  }

  public bool OnIgnorableBnetError(BnetErrorInfo info)
  {
    BattleNetErrors error = info.GetError();
    bool flag = false;
    switch (error)
    {
      case BattleNetErrors.ERROR_OK:
        flag = true;
        break;
      case BattleNetErrors.ERROR_INVALID_ARGS:
      case BattleNetErrors.ERROR_REPORT_UNAVAILABLE:
        flag = info.GetFeature() == BnetFeature.Report;
        break;
      case BattleNetErrors.ERROR_INVALID_TARGET_ID:
        flag = info.GetFeature() == BnetFeature.Friends && info.GetFeatureEvent() == BnetFeatureEvent.Friends_OnSendInvitation;
        break;
      case BattleNetErrors.ERROR_API_NOT_READY:
        flag = info.GetFeature() == BnetFeature.Presence;
        break;
      case BattleNetErrors.ERROR_TARGET_OFFLINE:
        flag = true;
        break;
      case BattleNetErrors.ERROR_FRIENDS_FRIENDSHIP_ALREADY_EXISTS:
      case BattleNetErrors.ERROR_FRIENDS_INVITATION_ALREADY_EXISTS:
      case BattleNetErrors.ERROR_FRIENDS_INVITEE_AT_MAX_FRIENDS:
      case BattleNetErrors.ERROR_FRIENDS_INVITER_AT_MAX_FRIENDS:
      case BattleNetErrors.ERROR_FRIENDS_INVITER_IS_BLOCKED_BY_INVITEE:
        flag = true;
        break;
      case BattleNetErrors.ERROR_GAME_UTILITY_SERVER_NO_SERVER:
        this.m_state.LogSource.LogError("Network.IgnoreBnetError() - error={0}", (object) info);
        flag = true;
        break;
    }
    if (error > BattleNetErrors.ERROR_OK & flag)
      TelemetryManager.Client().SendIgnorableBattleNetError((int) error, error.ToString());
    return flag;
  }

  public void OnFatalBnetError(BnetErrorInfo info)
  {
    BattleNetErrors error1 = info.GetError();
    this.m_state.LogSource.LogError("Network.OnFatalBnetError() - error={0}", (object) info);
    TelemetryManager.Client().SendFatalBattleNetError((int) error1, error1.ToString());
    switch (error1)
    {
      case BattleNetErrors.ERROR_DENIED:
        this.ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_LOGIN_FAILURE");
        break;
      case BattleNetErrors.ERROR_PARENTAL_CONTROL_RESTRICTION:
        ServiceManager.Get<ILoginService>()?.ClearAuthentication();
        Error.AddFatal(FatalErrorReason.ADMIN_KICK_OR_BAN, "GLOBAL_ERROR_NETWORK_PARENTAL_CONTROLS");
        break;
      case BattleNetErrors.ERROR_BAD_VERSION:
        if (PlatformSettings.IsMobile() && GameDownloadManagerProvider.Get() != null && !GameDownloadManagerProvider.Get().IsNewMobileVersionReleased)
          Error.AddFatal(FatalErrorReason.UNAVAILABLE_NEW_VERSION, "GLOBAL_ERROR_NETWORK_UNAVAILABLE_NEW_VERSION");
        else
          Error.AddFatal(new ErrorParams()
          {
            m_message = GameStrings.Format("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UPGRADE"),
            m_redirectToStore = (bool) Error.HAS_APP_STORE,
            m_reason = FatalErrorReason.UNAVAILABLE_UPGRADE
          });
        ReconnectMgr.Get().FullResetRequired = true;
        ReconnectMgr.Get().UpdateRequired = true;
        break;
      case BattleNetErrors.ERROR_SERVER_IS_PRIVATE:
        TelemetryManager.Client().SendNetworkError(NetworkError.ErrorType.PRIVATE_SERVER, info.ToString(), 33);
        this.ShowConnectionFailureError("GLOBAL_ERROR_UNKNOWN_ERROR");
        Log.Net.PrintWarning("ERROR_SERVER_IS_PRIVATE - {0} connection failures.", (object) this.m_numConnectionFailures);
        break;
      case BattleNetErrors.ERROR_PHONE_LOCK:
        Error.AddFatal(FatalErrorReason.BNET_PHONE_LOCK, "GLOBAL_ERROR_NETWORK_PHONE_LOCK");
        break;
      case BattleNetErrors.ERROR_GAME_ACCOUNT_BANNED:
      case BattleNetErrors.ERROR_BATTLENET_ACCOUNT_BANNED:
        ServiceManager.Get<ILoginService>()?.ClearAuthentication();
        Error.AddFatal(FatalErrorReason.ADMIN_KICK_OR_BAN, "GLOBAL_ERROR_NETWORK_ACCOUNT_BANNED");
        break;
      case BattleNetErrors.ERROR_GAME_ACCOUNT_SUSPENDED:
        ServiceManager.Get<ILoginService>()?.ClearAuthentication();
        Error.AddFatal(FatalErrorReason.ADMIN_KICK_OR_BAN, "GLOBAL_ERROR_NETWORK_ACCOUNT_SUSPENDED");
        break;
      case BattleNetErrors.ERROR_SESSION_DUPLICATE:
        Error.AddFatal(FatalErrorReason.LOGIN_FROM_ANOTHER_DEVICE, "GLOBAL_ERROR_NETWORK_DUPLICATE_LOGIN");
        break;
      case BattleNetErrors.ERROR_SESSION_DISCONNECTED:
        Error.AddFatal(FatalErrorReason.BNET_NETWORK_DISCONNECT, "GLOBAL_ERROR_NETWORK_DISCONNECT");
        break;
      case BattleNetErrors.ERROR_ADMIN_KICK:
      case BattleNetErrors.ERROR_SESSION_ADMIN_KICK:
        Error.AddFatal(FatalErrorReason.ADMIN_KICK_OR_BAN, "GLOBAL_ERROR_NETWORK_ADMIN_KICKED");
        break;
      case BattleNetErrors.ERROR_LOGON_WEB_VERIFY_TIMEOUT:
        this.ShowConnectionFailureError("GLOBAL_MOBILE_ERROR_LOGON_WEB_TIMEOUT");
        break;
      case BattleNetErrors.ERROR_RPC_PEER_UNAVAILABLE:
        TelemetryManager.Client().SendNetworkError(NetworkError.ErrorType.PEER_UNAVAILABLE, info.ToString(), 3004);
        this.ShowConnectionFailureError("GLOBAL_ERROR_UNKNOWN_ERROR");
        Log.Net.PrintWarning("ERROR_RPC_PEER_UNAVAILABLE - {0} connection failures.", (object) this.m_numConnectionFailures);
        break;
      case BattleNetErrors.ERROR_RPC_PEER_DISCONNECTED:
        this.ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_DISCONNECT");
        break;
      case BattleNetErrors.ERROR_RPC_REQUEST_TIMED_OUT:
        this.ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_UTIL_TIMEOUT");
        break;
      case BattleNetErrors.ERROR_RPC_QUOTA_EXCEEDED:
        Error.AddFatal(FatalErrorReason.BNET_NETWORK_SPAM, "GLOBAL_ERROR_NETWORK_SPAM");
        break;
      case BattleNetErrors.ERROR_SESSION_CAIS_PLAYTIME_EXCEEDED:
        ServiceManager.Get<ILoginService>()?.ClearAuthentication();
        Error.AddFatal(FatalErrorReason.ADMIN_KICK_OR_BAN, "GLOBAL_ERROR_NETWORK_ACCOUNT_PLAYTIME_EXCEEDED");
        break;
      case BattleNetErrors.ERROR_SESSION_CAIS_CURFEW:
        ServiceManager.Get<ILoginService>()?.ClearAuthentication();
        Error.AddFatal(FatalErrorReason.ADMIN_KICK_OR_BAN, "GLOBAL_ERROR_NETWORK_ACCOUNT_CURFEW_REACHED");
        break;
      case BattleNetErrors.ERROR_SESSION_INVALID_NID:
        ServiceManager.Get<ILoginService>()?.ClearAuthentication();
        Error.AddFatal(FatalErrorReason.ADMIN_KICK_OR_BAN, "GLOBAL_ERROR_NETWORK_ACCOUNT_INVALID_NID");
        break;
      default:
        string error2;
        if (HearthstoneApplication.IsInternal())
        {
          error2 = string.Format("Unhandled Bnet Error: {0}", (object) info);
        }
        else
        {
          Debug.LogError((object) string.Format("Unhandled Bnet Error: {0}", (object) info));
          error2 = GameStrings.Format("GLOBAL_ERROR_UNKNOWN_ERROR");
        }
        TelemetryManager.Client().SendNetworkError(NetworkError.ErrorType.OTHER_UNKNOWN, info.ToString(), (int) info.GetError());
        this.ShowConnectionFailureError(error2);
        break;
    }
  }

  private float DelayForConnectionFailures(int numFailures)
  {
    float num = (float) (new System.Random().NextDouble() * 3.0) + 3.5f;
    return (float) Math.Min(numFailures, 3) * num;
  }

  public void EnsureSubscribedTo(UtilSystemId systemChannel) => this.m_connectApi.EnsureSubscribedTo(systemChannel);

  private bool OnBnetAuthError(BnetErrorInfo info, object userData) => false;

  public static void AcceptFriendInvite(BnetInvitationId inviteid) => Blizzard.GameService.SDK.Client.Integration.BattleNet.ManageFriendInvite(1, inviteid.GetVal());

  public static void IgnoreFriendInvite(BnetInvitationId inviteid) => Blizzard.GameService.SDK.Client.Integration.BattleNet.ManageFriendInvite(4, inviteid.GetVal());

  private static void SendFriendInvite(string sender, string target, bool byEmail) => Blizzard.GameService.SDK.Client.Integration.BattleNet.SendFriendInvite(sender, target, byEmail);

  public static void SendFriendInviteByEmail(string sender, string target) => Network.SendFriendInvite(sender, target, true);

  public static void SendFriendInviteByBattleTag(string sender, string target) => Network.SendFriendInvite(sender, target, false);

  public static void RemoveFriend(BnetAccountId id) => Blizzard.GameService.SDK.Client.Integration.BattleNet.RemoveFriend(id);

  public static void SendWhisper(BnetAccountId account, string message) => Blizzard.GameService.SDK.Client.Integration.BattleNet.SendWhisper(account, message);

  public void GotoGameServer(GameServerInfo info, bool reconnecting)
  {
    this.m_state.LastGameServerInfo = info;
    if (this.m_connectApi.GetGameStartState() != GameStartState.Invalid && !ReconnectMgr.Get().IsRestoringGameStateFromDatabase())
    {
      Error.AddDevFatal("GotoGameServer() was called when we're already waiting for a game to start.");
    }
    else
    {
      string address = info.Address;
      uint port = Vars.Key("Application.GameServerPortOverride").GetUInt(info.Port);
      Debug.LogFormat("Network.GotoGameServer -- address= " + address + ":" + (object) port + ", game=" + (object) info.GameHandle + ", client=" + (object) info.ClientHandle + ", spectateKey=" + info.SpectatorPassword + " reconnecting=" + reconnecting.ToString());
      if (address == null)
        return;
      if (string.IsNullOrEmpty(address) || port == 0U || info.GameHandle == 0U && Network.ShouldBeConnectedToAurora())
        Debug.LogWarning((object) ("Network.GotoGameServer: ERROR in ServerInfo address= " + address + ":" + (object) port + ",    game=" + (object) info.GameHandle + ", client=" + (object) info.ClientHandle + " reconnecting=" + reconnecting.ToString()));
      this.m_gameServerKeepAliveFrequencySeconds = 0U;
      this.m_gameServerKeepAliveRetry = 3U;
      this.m_gameConceded = false;
      this.m_disconnectRequested = false;
      this.m_connectApi.SetTimeLastPingSent(0.0);
      this.m_connectApi.SetTimeLastPingReceived(0.0);
      this.m_connectApi.SetPingsSinceLastPong(0);
      if (Network.GameServerDisconnectEvents != null)
        Network.GameServerDisconnectEvents.Clear();
      this.m_state.LastConnectToGameServerInfo = new ConnectToGameServer();
      this.m_state.LastConnectToGameServerInfo.TimeSpentMilliseconds = (long) TimeUtils.GetElapsedTimeSinceEpoch().TotalMilliseconds;
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo = new Blizzard.Telemetry.WTCG.Client.GameSessionInfo();
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.GameServerIpAddress = info.Address;
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.GameServerPort = info.Port;
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.Version = info.Version;
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.GameHandle = info.GameHandle;
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.ScenarioId = GameMgr.Get().GetNextMissionId();
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.GameType = (Blizzard.Telemetry.WTCG.Client.GameType) GameMgr.Get().GetNextGameType();
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.FormatType = (Blizzard.Telemetry.WTCG.Client.FormatType) GameMgr.Get().GetNextFormatType();
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.IsReconnect = GameMgr.Get().IsNextReconnect();
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.IsSpectating = GameMgr.Get().IsNextSpectator();
      this.m_state.LastConnectToGameServerInfo.GameSessionInfo.ClientHandle = info.ClientHandle;
      long? lastDeckId = GameMgr.Get().LastDeckId;
      if (lastDeckId.HasValue)
      {
        Blizzard.Telemetry.WTCG.Client.GameSessionInfo gameSessionInfo = this.m_state.LastConnectToGameServerInfo.GameSessionInfo;
        lastDeckId = GameMgr.Get().LastDeckId;
        long num = lastDeckId.Value;
        gameSessionInfo.ClientDeckId = num;
      }
      int? lastHeroCardDbId = GameMgr.Get().LastHeroCardDbId;
      if (lastHeroCardDbId.HasValue)
      {
        Blizzard.Telemetry.WTCG.Client.GameSessionInfo gameSessionInfo = this.m_state.LastConnectToGameServerInfo.GameSessionInfo;
        lastHeroCardDbId = GameMgr.Get().LastHeroCardDbId;
        long num = (long) lastHeroCardDbId.Value;
        gameSessionInfo.ClientHeroCardId = num;
      }
      if (!this.m_connectApi.GotoGameServer(address, port))
        return;
      this.SendGameServerHandshake(info);
      this.m_connectApi.SetGameStartState(reconnecting ? GameStartState.Reconnecting : GameStartState.InitialStart);
    }
  }

  private void SendIPv6ConversionResult(bool connectSuccess)
  {
    if (string.IsNullOrEmpty(this.GameServerIPv6) || string.IsNullOrEmpty(this.GameServerIPv4))
      return;
    bool onCellular = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
    TelemetryManager.Client().SendIPv6Conversion(this.GameServerIPv6, this.GameServerIPv4, connectSuccess, onCellular, this.IsOSSupportIPv6);
  }

  private void OnGameServerConnectEvent(BattleNetErrors error)
  {
    Log.GameMgr.Print("Connecting to game server with error code " + (object) error);
    if (this.m_state.LastConnectToGameServerInfo != null)
    {
      long num = (long) (TimeUtils.GetElapsedTimeSinceEpoch().TotalMilliseconds - (double) this.m_state.LastConnectToGameServerInfo.TimeSpentMilliseconds);
      this.m_state.LastConnectToGameServerInfo.ResultBnetCode = (uint) error;
      this.m_state.LastConnectToGameServerInfo.ResultBnetCodeString = error.ToString();
      this.m_state.LastConnectToGameServerInfo.TimeSpentMilliseconds = num;
      TelemetryManager.Client().SendConnectToGameServer(this.m_state.LastConnectToGameServerInfo);
      this.m_state.LastConnectToGameServerInfo = (ConnectToGameServer) null;
    }
    GameServerInfo gameServerJoined = this.GetLastGameServerJoined();
    this.SendIPv6ConversionResult(error == BattleNetErrors.ERROR_OK);
    if (error == BattleNetErrors.ERROR_OK)
    {
      TelemetryManager.Client().SendConnectSuccess("GAME", gameServerJoined == null ? (string) null : gameServerJoined.Address, gameServerJoined == null ? new uint?() : new uint?(gameServerJoined.Port));
      TelemetryManager.RegisterShutdownListener(new System.Action(this.SendDefaultDisconnectTelemetry));
    }
    else
    {
      TelemetryManager.Client().SendConnectFail("GAME", error.ToString(), gameServerJoined == null ? (string) null : gameServerJoined.Address, gameServerJoined == null ? new uint?() : new uint?(gameServerJoined.Port));
      GameStartState gameStartState = this.m_connectApi.GetGameStartState();
      this.m_connectApi.SetGameStartState(GameStartState.Invalid);
      if (Network.ShouldBeConnectedToAurora())
      {
        if (gameStartState == GameStartState.Reconnecting)
          return;
        if (error == BattleNetErrors.ERROR_RPC_PEER_UNKNOWN && NetworkReachabilityManager.OnCellular)
          this.ShowBreakingNewsOrError("GLOBAL_MOBILE_ERROR_GAMESERVER_CONNECT");
        else
          this.ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_NO_GAME_SERVER");
        Debug.LogError((object) ("Failed to connect to game server with error " + (object) error));
      }
      else
      {
        this.ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_NO_GAME_SERVER");
        Debug.LogError((object) ("Failed to connect to game server with error " + (object) error));
      }
    }
  }

  private void OnGameServerDisconnectEvent(BattleNetErrors error)
  {
    Log.GameMgr.Print("Disconnected from game server with error {0} {1}", (object) (int) error, (object) error.ToString());
    TelemetryManager.UnregisterShutdownListener(new System.Action(this.SendDefaultDisconnectTelemetry));
    GameServerInfo gameServerJoined = this.GetLastGameServerJoined();
    TelemetryManager.Client().SendDisconnect("GAME", TelemetryUtil.GetReasonFromBnetError(error), error == BattleNetErrors.ERROR_OK ? (string) null : error.ToString(), gameServerJoined == null ? (string) null : gameServerJoined.Address, gameServerJoined == null ? new uint?() : new uint?(gameServerJoined.Port));
    this.m_state.LastConnectToGameServerInfo = (ConnectToGameServer) null;
    bool flag = false;
    if (error != BattleNetErrors.ERROR_OK)
    {
      switch (this.m_connectApi.GetGameStartState())
      {
        case GameStartState.InitialStart:
          if ((gameServerJoined == null ? 0 : (gameServerJoined.SpectatorMode ? 1 : 0)) == 0)
          {
            Debug.LogError((object) ("Disconnected from game server with error " + (object) error));
            Network.ConnectErrorParams errorParams = new Network.ConnectErrorParams();
            errorParams.m_message = GameStrings.Format("GLOBAL_ERROR_NETWORK_DISCONNECT_GAME_SERVER");
            this.AddErrorToList(errorParams);
            flag = true;
            break;
          }
          break;
        case GameStartState.Reconnecting:
          flag = true;
          break;
      }
      this.m_connectApi.SetGameStartState(GameStartState.Invalid);
    }
    if (flag)
      return;
    this.AddGameServerDisconnectEvent(error);
  }

  private void OnIPv6ConversionEvent(string ipv6, string ipv4)
  {
    this.GameServerIPv6 = ipv6;
    this.GameServerIPv4 = ipv4;
    this.IsOSSupportIPv6 = Socket.OSSupportsIPv6;
  }

  private void SendDefaultDisconnectTelemetry()
  {
    GameServerInfo gameServerJoined = this.GetLastGameServerJoined();
    TelemetryManager.Client().SendDisconnect("GAME", TelemetryUtil.GetReasonFromBnetError(BattleNetErrors.ERROR_OK), host: (gameServerJoined == null ? (string) null : gameServerJoined.Address), port: (gameServerJoined == null ? new uint?() : new uint?(gameServerJoined.Port)));
  }

  private void AddGameServerDisconnectEvent(BattleNetErrors error)
  {
    if (Network.GameServerDisconnectEvents == null)
      Network.GameServerDisconnectEvents = new List<BattleNetErrors>();
    Network.GameServerDisconnectEvents.Add(error);
  }

  public void SpectateSecondPlayer(GameServerInfo info)
  {
    info.SpectatorMode = true;
    if (!this.IsConnectedToGameServer())
      this.GotoGameServer(info, false);
    else
      this.SendGameServerHandshake(info);
  }

  public bool RetryGotoGameServer()
  {
    if (this.m_connectApi.GetGameStartState() == GameStartState.Invalid)
      return false;
    this.SendGameServerHandshake(this.m_state.LastGameServerInfo);
    return true;
  }

  public GameServerInfo GetLastGameServerJoined() => this.m_state.LastGameServerInfo;

  public void ClearLastGameServerJoined() => this.m_state.LastGameServerInfo = (GameServerInfo) null;

  public static string GetUsername()
  {
    string username = (string) null;
    try
    {
      username = Network.GetStoredUserName();
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ("Exception while loading settings: " + ex.Message));
    }
    if (username == null)
      username = Vars.Key("Aurora.Username").GetStr("NOT_PROVIDED_PLEASE_PROVIDE_VIA_CONFIG");
    if (username != null && username.IndexOf("@") == -1)
      username += "@blizzard.com";
    return username;
  }

  public static string GetTargetServer()
  {
    int num = Vars.Key("Aurora.Env.Override").GetInt(0) != 0 ? 1 : 0;
    string def = "default";
    string targetServer = (string) null;
    if (num != 0)
    {
      targetServer = Vars.Key("Aurora.Env").GetStr(def);
      if (string.IsNullOrEmpty(targetServer))
        targetServer = (string) null;
    }
    if (targetServer == null)
      targetServer = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetConnectionString();
    if (targetServer == null)
    {
      string launchOption = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetLaunchOption("REGION", false);
      if (!string.IsNullOrEmpty(launchOption))
        targetServer = launchOption == "US" ? "us.actual.battle.net" : (launchOption == "XX" ? "beta.actual.battle.net" : (launchOption == "EU" ? "eu.actual.battle.net" : (launchOption == "CN" ? "cn.actual.battle.net" : (launchOption == "KR" ? "kr.actual.battle.net" : def))));
    }
    if (targetServer.ToLower() == def)
      targetServer = "bn11-01.battle.net";
    return targetServer;
  }

  public static uint GetPort()
  {
    uint port = 0;
    if (Vars.Key("Aurora.Env.Override").GetUInt(0U) > 0U)
      port = Vars.Key("Aurora.Port").GetUInt(0U);
    if (port == 0U)
      port = 1119U;
    return port;
  }

  private static SslParameters GetSSLParams()
  {
    SslParameters sslParams = new SslParameters();
    TextAsset textAsset = (TextAsset) Resources.Load("SSLCert/ssl_cert_bundle");
    if ((UnityEngine.Object) textAsset != (UnityEngine.Object) null)
      sslParams.bundleSettings.bundle = new SslCertBundle(textAsset.bytes);
    sslParams.bundleSettings.bundleDownloadConfig.numRetries = 3;
    sslParams.bundleSettings.bundleDownloadConfig.timeoutMs = -1;
    return sslParams;
  }

  public static string GetVersion() => Network.GetVersionFromConfig();

  private static string GetVersionFromConfig()
  {
    string str = Vars.Key("Aurora.Version.Source").GetStr("undefined");
    if (str == "undefined")
      str = "product";
    string versionFromConfig;
    if (str == "product")
      versionFromConfig = Network.ProductVersion();
    else if (str == "string")
    {
      string def = "undefined";
      versionFromConfig = Vars.Key("Aurora.Version.String").GetStr(def);
      if (versionFromConfig == def)
        Debug.LogError((object) "Aurora.Version.String undefined");
    }
    else
    {
      Debug.LogError((object) ("unknown version source: " + str));
      versionFromConfig = "0";
    }
    foreach (string commandLineArg in HearthstoneApplication.CommandLineArgs)
    {
      if (commandLineArg.Equals("hsc") || commandLineArg.Equals("-hsc"))
      {
        versionFromConfig = "6969ef511a6cabbc24c5";
        break;
      }
      if (commandLineArg.Equals("hse1") || commandLineArg.Equals("-hse1"))
      {
        versionFromConfig = "707cb136922d1f294c4f";
        break;
      }
    }
    return versionFromConfig;
  }

  public void OnLoginStarted() => this.m_connectApi.OnLoginStarted();

  public void DoLoginUpdate()
  {
    string referralSource = Vars.Key("Application.Referral").GetStr("none");
    if (referralSource.Equals("none"))
    {
      switch (PlatformSettings.OS)
      {
        case OSCategory.PC:
        case OSCategory.Mac:
          referralSource = "Battle.net";
          break;
        case OSCategory.iOS:
          referralSource = "AppleAppStore";
          break;
        case OSCategory.Android:
          switch (AndroidDeviceSettings.Get().GetAndroidStore())
          {
            case AndroidStore.BLIZZARD:
              if (PlatformSettings.LocaleVariant == LocaleVariant.China)
              {
                referralSource = "JV-Android";
                break;
              }
              break;
            case AndroidStore.GOOGLE:
              referralSource = "GooglePlay";
              break;
            case AndroidStore.AMAZON:
              referralSource = "AmazonAppStore";
              break;
            case AndroidStore.HUAWEI:
              referralSource = "HuaweiAppStore";
              break;
            case AndroidStore.ONE_STORE:
              referralSource = "OneStore";
              break;
          }
          break;
      }
    }
    this.m_connectApi.DoLoginUpdate(referralSource);
  }

  public void OnStartupPacketSequenceComplete() => this.m_connectApi.OnStartupPacketSequenceComplete();

  public bool IsFindingGame() => this.m_state.FindingBnetGameType != 0;

  public BnetGameType GetFindingBnetGameType() => this.m_state.FindingBnetGameType;

  public static BnetGameType TranslateGameTypeToBnet(
    PegasusShared.GameType gameType,
    PegasusShared.FormatType formatType,
    int missionId)
  {
    switch (gameType)
    {
      case PegasusShared.GameType.GT_VS_AI:
        return BnetGameType.BGT_VS_AI;
      case PegasusShared.GameType.GT_VS_FRIEND:
        return BnetGameType.BGT_FRIENDS;
      case PegasusShared.GameType.GT_TUTORIAL:
        return BnetGameType.BGT_TUTORIAL;
      case PegasusShared.GameType.GT_ARENA:
        return BnetGameType.BGT_ARENA;
      case PegasusShared.GameType.GT_RANKED:
      case PegasusShared.GameType.GT_CASUAL:
        return RankMgr.Get().GetBnetGameTypeForLeague(gameType == PegasusShared.GameType.GT_RANKED, formatType);
      case PegasusShared.GameType.GT_TAVERNBRAWL:
        if (GameUtils.IsAIMission(missionId))
          return BnetGameType.BGT_TAVERNBRAWL_1P_VERSUS_AI;
        return GameUtils.IsCoopMission(missionId) ? BnetGameType.BGT_TAVERNBRAWL_2P_COOP : BnetGameType.BGT_TAVERNBRAWL_PVP;
      case PegasusShared.GameType.GT_FSG_BRAWL_VS_FRIEND:
        return BnetGameType.BGT_FSG_BRAWL_VS_FRIEND;
      case PegasusShared.GameType.GT_FSG_BRAWL:
        return BnetGameType.BGT_FSG_BRAWL_PVP;
      case PegasusShared.GameType.GT_FSG_BRAWL_1P_VS_AI:
        return BnetGameType.BGT_FSG_BRAWL_1P_VERSUS_AI;
      case PegasusShared.GameType.GT_FSG_BRAWL_2P_COOP:
        return BnetGameType.BGT_FSG_BRAWL_2P_COOP;
      case PegasusShared.GameType.GT_BATTLEGROUNDS:
        return BnetGameType.BGT_BATTLEGROUNDS;
      case PegasusShared.GameType.GT_BATTLEGROUNDS_FRIENDLY:
        return BnetGameType.BGT_BATTLEGROUNDS_FRIENDLY;
      case PegasusShared.GameType.GT_PVPDR_PAID:
        return BnetGameType.BGT_PVPDR_PAID;
      case PegasusShared.GameType.GT_PVPDR:
        return BnetGameType.BGT_PVPDR;
      case PegasusShared.GameType.GT_MERCENARIES_PVP:
        return BnetGameType.BGT_MERCENARIES_PVP;
      case PegasusShared.GameType.GT_MERCENARIES_PVE:
        return BnetGameType.BGT_MERCENARIES_PVE;
      case PegasusShared.GameType.GT_MERCENARIES_PVE_COOP:
        return BnetGameType.BGT_MERCENARIES_PVE_COOP;
      case PegasusShared.GameType.GT_BATTLEGROUNDS_PLAYER_VS_AI:
        return BnetGameType.BGT_BATTLEGROUNDS_PLAYER_VS_AI;
      default:
        Error.AddDevFatal("Network.TranslateGameTypeToBnet() - do not know how to translate {0}", (object) gameType);
        return BnetGameType.BGT_UNKNOWN;
    }
  }

  public void FindGame(
    PegasusShared.GameType gameType,
    PegasusShared.FormatType formatType,
    int scenarioId,
    int brawlLibraryItemId,
    long deckId,
    string aiDeck,
    int heroCardDbId,
    int? seasonId,
    bool restoredSavedGameState,
    byte[] snapshot,
    int? lettuceMapNodeId,
    long lettuceTeamId,
    PegasusShared.GameType progFilterOverride = PegasusShared.GameType.GT_UNKNOWN,
    int deckTemplateId = 0)
  {
    if (gameType == PegasusShared.GameType.GT_VS_FRIEND || gameType == PegasusShared.GameType.GT_FSG_BRAWL_VS_FRIEND)
    {
      Error.AddDevFatal("Network.FindGame - friendly challenges must call EnterFriendlyChallengeGame instead.");
    }
    else
    {
      BnetGameType bnet = Network.TranslateGameTypeToBnet(gameType, formatType, scenarioId);
      if (bnet == BnetGameType.BGT_UNKNOWN)
      {
        Error.AddDevFatal(string.Format("FindGame: no bnetGameType for {0} {1}", (object) gameType, (object) formatType));
      }
      else
      {
        this.m_state.FindingBnetGameType = bnet;
        if (this.IsNoAccountTutorialGame(bnet))
        {
          this.GoToNoAccountTutorialServer(scenarioId);
        }
        else
        {
          bool flag = Network.RequiresScenarioIdAttribute(gameType);
          byte[] byteArray = Guid.NewGuid().ToByteArray();
          long currentFsgId = FiresideGatheringManager.Get().CurrentFsgId;
          Log.BattleNet.PrintInfo("FindGame type={0} scenario={1} deck={2} aideck={3} setScenId={4} request_guid={5}", (object) (int) bnet, (object) scenarioId, (object) deckId, (object) aiDeck, (object) (flag ? 1 : 0), byteArray == null ? (object) "null" : (object) byteArray.ToHexString());
          BnetMatchmakingPlayer matchmakingPlayer = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(BnetPresenceMgr.Get().GetMyGameAccountId()), new Blizzard.GameService.Protocol.V2.Client.Attribute[7]
          {
            BnetAttribute.CreateAttribute("type", (long) bnet),
            BnetAttribute.CreateAttribute("scenario", scenarioId),
            BnetAttribute.CreateAttribute("brawl_library_item_id", brawlLibraryItemId),
            BnetAttribute.CreateAttribute("aideck", aiDeck ?? ""),
            BnetAttribute.CreateAttribute("request_guid", byteArray),
            BnetAttribute.CreateAttribute("fsg_id", currentFsgId),
            BnetAttribute.CreateAttribute("mercenaries_team", lettuceTeamId)
          });
          if (deckTemplateId != 0)
            matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("deck_template", deckTemplateId));
          else
            matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("deck", deckId));
          if (!string.IsNullOrEmpty(Cheats.Get().GetPlayerTags()))
            matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("cheat_player_tags", Cheats.Get().GetPlayerTags()));
          Cheats.Get().ClearAllPlayerTags();
          int cardBackToUse;
          int? deckCardBack;
          CardBackManager.Get().FindCardBackToUse(deckId, out cardBackToUse, out deckCardBack);
          int num = cardBackToUse;
          int? nullable = deckCardBack;
          int valueOrDefault = nullable.GetValueOrDefault();
          if (!(num == valueOrDefault & nullable.HasValue))
            matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("card_back_id", cardBackToUse));
          if (heroCardDbId != 0)
            matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("hero_card_id", heroCardDbId));
          else if (deckId != 0L || deckTemplateId > 0)
          {
            CollectionDeck collectionDeck = CollectionManager.Get().GetDeck(deckId) ?? FreeDeckMgr.Get().GetLoanerDeckFromDeckTemplateId(deckTemplateId);
            if (collectionDeck != null && !collectionDeck.HeroOverridden)
            {
              int heroIdOwnedByPlayer = CollectionManager.Get().GetRandomHeroIdOwnedByPlayer(collectionDeck.GetClass(), collectionDeck.RandomHeroUseFavorite);
              matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("random_hero_card_id", heroIdOwnedByPlayer));
            }
          }
          if (seasonId.HasValue)
            matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("season_id", seasonId.Value));
          List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection1 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("GameType", (long) bnet));
          if (flag)
            attributeCollection1.Add(BnetAttribute.CreateAttribute("ScenarioId", scenarioId));
          List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection2 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("type", (long) bnet), BnetAttribute.CreateAttribute("scenario", scenarioId), BnetAttribute.CreateAttribute("brawl_library_item_id", brawlLibraryItemId), BnetAttribute.CreateAttribute("prog_filter_override", (long) progFilterOverride));
          if (Cheats.Get().GetBoardId() > 0)
            attributeCollection2.Add(BnetAttribute.CreateAttribute("cheat_board_override", Cheats.Get().GetBoardId()));
          Cheats.Get().ClearBoardId();
          if (ReconnectMgr.Get().GetBypassReconnect())
          {
            attributeCollection2.Add(BnetAttribute.CreateAttribute("bypass", true));
            ReconnectMgr.Get().SetBypassReconnect(false);
          }
          if (seasonId.HasValue)
            attributeCollection2.Add(BnetAttribute.CreateAttribute("season_id", seasonId.Value));
          if (snapshot != null)
            attributeCollection2.Add(BnetAttribute.CreateAttribute(nameof (snapshot), snapshot));
          if (restoredSavedGameState)
            attributeCollection2.Add(BnetAttribute.CreateAttribute("load_game", true));
          if (lettuceMapNodeId.HasValue)
            attributeCollection2.Add(BnetAttribute.CreateAttribute("lettuce_map_node_id", lettuceMapNodeId.Value));
          Blizzard.GameService.SDK.Client.Integration.BattleNet.QueueMatchmaking(attributeCollection1, attributeCollection2, matchmakingPlayer);
          this.m_state.LastFindGameParameters = new FindGameResult();
          this.m_state.LastFindGameParameters.TimeSpentMilliseconds = (long) TimeUtils.GetElapsedTimeSinceEpoch().TotalMilliseconds;
          this.m_state.LastFindGameParameters.GameSessionInfo = new Blizzard.Telemetry.WTCG.Client.GameSessionInfo();
          this.m_state.LastFindGameParameters.GameSessionInfo.Version = Network.GetVersion();
          this.m_state.LastFindGameParameters.GameSessionInfo.ScenarioId = scenarioId;
          this.m_state.LastFindGameParameters.GameSessionInfo.BrawlLibraryItemId = brawlLibraryItemId;
          if (seasonId.HasValue)
            this.m_state.LastFindGameParameters.GameSessionInfo.SeasonId = seasonId.Value;
          this.m_state.LastFindGameParameters.GameSessionInfo.GameType = (Blizzard.Telemetry.WTCG.Client.GameType) gameType;
          this.m_state.LastFindGameParameters.GameSessionInfo.FormatType = (Blizzard.Telemetry.WTCG.Client.FormatType) formatType;
          this.m_state.LastFindGameParameters.GameSessionInfo.ClientDeckId = deckId;
          this.m_state.LastFindGameParameters.GameSessionInfo.ClientHeroCardId = (long) heroCardDbId;
        }
      }
    }
  }

  public void EnterFriendlyChallengeGame(
    PegasusShared.FormatType formatType,
    BrawlType brawlType,
    int scenarioId,
    int seasonId,
    int brawlLibraryItemId,
    BnetGameAccountId player2GameAccountId,
    DeckShareState player1DeckShareState,
    long player1DeckId,
    DeckShareState player2DeckShareState,
    long player2DeckId,
    long? player1HeroCardDbId,
    long? player2HeroCardDbId,
    long? player1RandomHeroCardDbId,
    long? player2RandomHeroCardDbId,
    long? player1CardBackId,
    long? player2CardBackId)
  {
    long val = 1;
    PegasusShared.GameType gameType = PegasusShared.GameType.GT_VS_FRIEND;
    if (brawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
    {
      val = 40L;
      gameType = PegasusShared.GameType.GT_FSG_BRAWL_VS_FRIEND;
    }
    long currentFsgId = FiresideGatheringManager.Get().CurrentFsgId;
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection1 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("GameType", val), BnetAttribute.CreateAttribute("ScenarioId", scenarioId));
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection2 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("type", val), BnetAttribute.CreateAttribute("scenario", scenarioId), BnetAttribute.CreateAttribute("format", (long) formatType), BnetAttribute.CreateAttribute("season_id", seasonId), BnetAttribute.CreateAttribute("brawl_library_item_id", brawlLibraryItemId));
    if (Cheats.Get().GetBoardId() > 0)
      attributeCollection2.Add(BnetAttribute.CreateAttribute("cheat_board_override", Cheats.Get().GetBoardId()));
    Cheats.Get().ClearBoardId();
    BnetMatchmakingPlayer matchmakingPlayer1 = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(BnetPresenceMgr.Get().GetMyGameAccountId()), new Blizzard.GameService.Protocol.V2.Client.Attribute[7]
    {
      BnetAttribute.CreateAttribute("type", val),
      BnetAttribute.CreateAttribute("scenario", scenarioId),
      BnetAttribute.CreateAttribute("deck_share_state", (long) player1DeckShareState),
      BnetAttribute.CreateAttribute("deck", player1DeckId),
      BnetAttribute.CreateAttribute("player_type", 1L),
      BnetAttribute.CreateAttribute("fsg_id", currentFsgId),
      BnetAttribute.CreateAttribute("season_id", seasonId)
    });
    if (player1HeroCardDbId.HasValue)
      matchmakingPlayer1.AddAttributes(BnetAttribute.CreateAttribute("hero_card_id", player1HeroCardDbId.Value));
    else if (player1RandomHeroCardDbId.HasValue)
      matchmakingPlayer1.AddAttributes(BnetAttribute.CreateAttribute("random_hero_card_id", player1RandomHeroCardDbId.Value));
    if (player1CardBackId.HasValue)
      matchmakingPlayer1.AddAttributes(BnetAttribute.CreateAttribute("card_back_id", player1CardBackId.Value));
    if (!string.IsNullOrEmpty(Cheats.Get().GetPlayerTags()))
      matchmakingPlayer1.AddAttributes(BnetAttribute.CreateAttribute("cheat_player_tags", Cheats.Get().GetPlayerTags()));
    Cheats.Get().ClearAllPlayerTags();
    BnetMatchmakingPlayer matchmakingPlayer2 = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(player2GameAccountId), new Blizzard.GameService.Protocol.V2.Client.Attribute[7]
    {
      BnetAttribute.CreateAttribute("type", val),
      BnetAttribute.CreateAttribute("scenario", scenarioId),
      BnetAttribute.CreateAttribute("deck_share_state", (long) player2DeckShareState),
      BnetAttribute.CreateAttribute("deck", player2DeckId),
      BnetAttribute.CreateAttribute("player_type", 2L),
      BnetAttribute.CreateAttribute("fsg_id", currentFsgId),
      BnetAttribute.CreateAttribute("season_id", seasonId)
    });
    if (player2HeroCardDbId.HasValue)
      matchmakingPlayer2.AddAttributes(BnetAttribute.CreateAttribute("hero_card_id", player2HeroCardDbId.Value));
    else if (player2RandomHeroCardDbId.HasValue)
      matchmakingPlayer2.AddAttributes(BnetAttribute.CreateAttribute("random_hero_card_id", player2RandomHeroCardDbId.Value));
    if (player2CardBackId.HasValue)
      matchmakingPlayer2.AddAttributes(BnetAttribute.CreateAttribute("card_back_id", player2CardBackId.Value));
    Blizzard.GameService.SDK.Client.Integration.BattleNet.QueueMatchmaking(attributeCollection1, attributeCollection2, matchmakingPlayer1, matchmakingPlayer2);
    this.m_state.LastFindGameParameters = new FindGameResult();
    this.m_state.LastFindGameParameters.TimeSpentMilliseconds = (long) TimeUtils.GetElapsedTimeSinceEpoch().TotalMilliseconds;
    this.m_state.LastFindGameParameters.GameSessionInfo = new Blizzard.Telemetry.WTCG.Client.GameSessionInfo();
    this.m_state.LastFindGameParameters.GameSessionInfo.Version = Network.GetVersion();
    this.m_state.LastFindGameParameters.GameSessionInfo.ScenarioId = scenarioId;
    this.m_state.LastFindGameParameters.GameSessionInfo.BrawlLibraryItemId = brawlLibraryItemId;
    this.m_state.LastFindGameParameters.GameSessionInfo.SeasonId = seasonId;
    this.m_state.LastFindGameParameters.GameSessionInfo.GameType = (Blizzard.Telemetry.WTCG.Client.GameType) gameType;
    this.m_state.LastFindGameParameters.GameSessionInfo.FormatType = (Blizzard.Telemetry.WTCG.Client.FormatType) formatType;
    this.m_state.LastFindGameParameters.GameSessionInfo.ClientDeckId = player1DeckId;
    if (!player1HeroCardDbId.HasValue)
      return;
    this.m_state.LastFindGameParameters.GameSessionInfo.ClientHeroCardId = player1HeroCardDbId.Value;
  }

  public void EnterBattlegroundsWithFriend(BnetGameAccountId player2GameAccountId, int scenarioId)
  {
    long val = 50;
    this.m_state.FindingBnetGameType = BnetGameType.BGT_BATTLEGROUNDS;
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection1 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("GameType", val), BnetAttribute.CreateAttribute("ScenarioId", scenarioId));
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection2 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("type", val), BnetAttribute.CreateAttribute("scenario", scenarioId), BnetAttribute.CreateAttribute("format", 2L));
    BnetMatchmakingPlayer matchmakingPlayer1 = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(BnetPresenceMgr.Get().GetMyGameAccountId()), new Blizzard.GameService.Protocol.V2.Client.Attribute[3]
    {
      BnetAttribute.CreateAttribute("type", val),
      BnetAttribute.CreateAttribute("scenario", scenarioId),
      BnetAttribute.CreateAttribute("player_type", 1L)
    });
    if (!string.IsNullOrEmpty(Cheats.Get().GetPlayerTags()))
      matchmakingPlayer1.AddAttributes(BnetAttribute.CreateAttribute("cheat_player_tags", Cheats.Get().GetPlayerTags()));
    Cheats.Get().ClearAllPlayerTags();
    BnetMatchmakingPlayer matchmakingPlayer2 = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(player2GameAccountId), new Blizzard.GameService.Protocol.V2.Client.Attribute[3]
    {
      BnetAttribute.CreateAttribute("type", val),
      BnetAttribute.CreateAttribute("scenario", scenarioId),
      BnetAttribute.CreateAttribute("player_type", 2L)
    });
    Blizzard.GameService.SDK.Client.Integration.BattleNet.QueueMatchmaking(attributeCollection1, attributeCollection2, matchmakingPlayer1, matchmakingPlayer2);
  }

  public void EnterBattlegroundsWithParty(BnetParty.PartyMember[] members, int scenarioId)
  {
    long val1;
    if (members.Length <= PartyManager.Get().GetBattlegroundsMaxRankedPartySize())
    {
      val1 = 50L;
      this.m_state.FindingBnetGameType = BnetGameType.BGT_BATTLEGROUNDS;
    }
    else
    {
      val1 = 51L;
      this.m_state.FindingBnetGameType = BnetGameType.BGT_BATTLEGROUNDS_FRIENDLY;
    }
    int currentPartySize = PartyManager.Get().GetCurrentPartySize();
    BnetEntityId bnetEntityId1 = (BnetEntityId) PartyManager.Get().GetLeader();
    if ((object) bnetEntityId1 == null)
      bnetEntityId1 = new BnetEntityId(0UL, 0UL);
    BnetEntityId bnetEntityId2 = bnetEntityId1;
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection1 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("GameType", val1), BnetAttribute.CreateAttribute("ScenarioId", scenarioId));
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection2 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("type", val1), BnetAttribute.CreateAttribute("scenario", scenarioId), BnetAttribute.CreateAttribute("format", 2L));
    List<BnetMatchmakingPlayer> matchmakingPlayerList = new List<BnetMatchmakingPlayer>();
    foreach (OnlinePlayer member in members)
    {
      BnetGameAccountId gameAccountId = member.GameAccountId;
      BnetMatchmakingPlayer matchmakingPlayer = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(gameAccountId), new Blizzard.GameService.Protocol.V2.Client.Attribute[6]
      {
        BnetAttribute.CreateAttribute("type", val1),
        BnetAttribute.CreateAttribute("scenario", scenarioId),
        BnetAttribute.CreateAttribute("player_type", 2L),
        BnetAttribute.CreateAttribute("party_size", currentPartySize),
        BnetAttribute.CreateAttribute("party_leader_game_account_id_hi", bnetEntityId2.High),
        BnetAttribute.CreateAttribute("party_leader_game_account_id_lo", bnetEntityId2.Low)
      });
      if ((BnetEntityId) gameAccountId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId() && !string.IsNullOrEmpty(Cheats.Get().GetPlayerTags()))
      {
        matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("cheat_player_tags", Cheats.Get().GetPlayerTags()));
        Cheats.Get().ClearAllPlayerTags();
      }
      else
      {
        string val2;
        if (Blizzard.GameService.SDK.Client.Integration.BattleNet.GetMemberAttribute<string>(PartyManager.Get().GetCurrentPartyId(), gameAccountId, "cheat_player_tags", out val2))
          matchmakingPlayer.AddAttributes(BnetAttribute.CreateAttribute("cheat_player_tags", val2));
      }
      matchmakingPlayerList.Add(matchmakingPlayer);
    }
    Blizzard.GameService.SDK.Client.Integration.BattleNet.QueueMatchmaking(attributeCollection1, attributeCollection2, matchmakingPlayerList.ToArray());
  }

  public void EnterMercenariesCoOpWithFriend(
    BnetGameAccountId player2GameAccountId,
    int scenarioId,
    int? mapNodeId)
  {
    long val = 60;
    this.m_state.FindingBnetGameType = BnetGameType.BGT_MERCENARIES_PVE_COOP;
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection1 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("GameType", val), BnetAttribute.CreateAttribute("ScenarioId", scenarioId));
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection2 = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("type", val), BnetAttribute.CreateAttribute("scenario", scenarioId), BnetAttribute.CreateAttribute("format", 2L));
    if (mapNodeId.HasValue)
      attributeCollection2.Add(BnetAttribute.CreateAttribute("lettuce_map_node_id", mapNodeId.Value));
    BnetEntityId bnetEntityId1 = (BnetEntityId) PartyManager.Get().GetLeader();
    if ((object) bnetEntityId1 == null)
      bnetEntityId1 = new BnetEntityId(0UL, 0UL);
    BnetEntityId bnetEntityId2 = bnetEntityId1;
    BnetMatchmakingPlayer matchmakingPlayer1 = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(BnetPresenceMgr.Get().GetMyGameAccountId()), new Blizzard.GameService.Protocol.V2.Client.Attribute[3]
    {
      BnetAttribute.CreateAttribute("type", val),
      BnetAttribute.CreateAttribute("scenario", scenarioId),
      BnetAttribute.CreateAttribute("player_type", 1L)
    });
    if (!string.IsNullOrEmpty(Cheats.Get().GetPlayerTags()))
      matchmakingPlayer1.AddAttributes(BnetAttribute.CreateAttribute("cheat_player_tags", Cheats.Get().GetPlayerTags()));
    matchmakingPlayer1.AddAttributes(BnetAttribute.CreateAttribute("party_leader_game_account_id_hi", bnetEntityId2.High));
    matchmakingPlayer1.AddAttributes(BnetAttribute.CreateAttribute("party_leader_game_account_id_lo", bnetEntityId2.Low));
    Cheats.Get().ClearAllPlayerTags();
    BnetMatchmakingPlayer matchmakingPlayer2 = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(player2GameAccountId), new Blizzard.GameService.Protocol.V2.Client.Attribute[5]
    {
      BnetAttribute.CreateAttribute("type", val),
      BnetAttribute.CreateAttribute("scenario", scenarioId),
      BnetAttribute.CreateAttribute("player_type", 2L),
      BnetAttribute.CreateAttribute("party_leader_game_account_id_hi", bnetEntityId2.High),
      BnetAttribute.CreateAttribute("party_leader_game_account_id_lo", bnetEntityId2.Low)
    });
    Blizzard.GameService.SDK.Client.Integration.BattleNet.QueueMatchmaking(attributeCollection1, attributeCollection2, matchmakingPlayer1, matchmakingPlayer2);
  }

  public void EnterMercenariesFriendlyChallenge(
    int scenarioId,
    long player1TeamId,
    bool player1Sharing,
    long player2TeamId,
    bool player2Sharing,
    BnetGameAccountId player2GameAccountId)
  {
    long val = 61;
    PegasusShared.GameType gameType = PegasusShared.GameType.GT_MERCENARIES_FRIENDLY;
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection1 = BnetAttribute.CreateAttributeCollection();
    attributeCollection1.Add(BnetAttribute.CreateAttribute("GameType", val));
    attributeCollection1.Add(BnetAttribute.CreateAttribute("ScenarioId", scenarioId));
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection2 = BnetAttribute.CreateAttributeCollection();
    attributeCollection2.Add(BnetAttribute.CreateAttribute("type", val));
    attributeCollection2.Add(BnetAttribute.CreateAttribute("scenario", scenarioId));
    BnetMatchmakingPlayer matchmakingPlayer1 = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(BnetPresenceMgr.Get().GetMyGameAccountId()), new Blizzard.GameService.Protocol.V2.Client.Attribute[5]
    {
      BnetAttribute.CreateAttribute("type", val),
      BnetAttribute.CreateAttribute("scenario", scenarioId),
      BnetAttribute.CreateAttribute("deck_share_state", player1Sharing ? 2 : 0),
      BnetAttribute.CreateAttribute("mercenaries_team", player1TeamId),
      BnetAttribute.CreateAttribute("player_type", 1L)
    });
    BnetMatchmakingPlayer matchmakingPlayer2 = new BnetMatchmakingPlayer(BnetGameAccountId.GetGameAccountHandle(player2GameAccountId), new Blizzard.GameService.Protocol.V2.Client.Attribute[5]
    {
      BnetAttribute.CreateAttribute("type", val),
      BnetAttribute.CreateAttribute("scenario", scenarioId),
      BnetAttribute.CreateAttribute("deck_share_state", player2Sharing ? 2 : 0),
      BnetAttribute.CreateAttribute("mercenaries_team", player2TeamId),
      BnetAttribute.CreateAttribute("player_type", 2L)
    });
    Blizzard.GameService.SDK.Client.Integration.BattleNet.QueueMatchmaking(attributeCollection1, attributeCollection2, matchmakingPlayer1, matchmakingPlayer2);
    this.m_state.LastFindGameParameters = new FindGameResult();
    this.m_state.LastFindGameParameters.TimeSpentMilliseconds = (long) TimeUtils.GetElapsedTimeSinceEpoch().TotalMilliseconds;
    this.m_state.LastFindGameParameters.GameSessionInfo = new Blizzard.Telemetry.WTCG.Client.GameSessionInfo();
    this.m_state.LastFindGameParameters.GameSessionInfo.Version = Network.GetVersion();
    this.m_state.LastFindGameParameters.GameSessionInfo.ScenarioId = scenarioId;
    this.m_state.LastFindGameParameters.GameSessionInfo.GameType = (Blizzard.Telemetry.WTCG.Client.GameType) gameType;
    this.m_state.LastFindGameParameters.GameSessionInfo.ClientDeckId = player1TeamId;
  }

  public void OnFindGameStateChanged(
    FindGameState prevState,
    FindGameState newState,
    uint errorCode)
  {
    switch (newState)
    {
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CONNECTING:
        this.SendTelemetry_FindGameResult(errorCode);
        break;
    }
  }

  private void SendTelemetry_FindGameResult(uint errorCode)
  {
    if (this.m_state.LastFindGameParameters == null)
      return;
    string str = errorCode < 1000000U ? ((BattleNetErrors) errorCode).ToString() : ((PegasusShared.ErrorCode) errorCode).ToString();
    long num = (long) (TimeUtils.GetElapsedTimeSinceEpoch().TotalMilliseconds - (double) this.m_state.LastFindGameParameters.TimeSpentMilliseconds);
    this.m_state.LastFindGameParameters.ResultCode = errorCode;
    this.m_state.LastFindGameParameters.ResultCodeString = str;
    this.m_state.LastFindGameParameters.TimeSpentMilliseconds = num;
    TelemetryManager.Client().SendFindGameResult(this.m_state.LastFindGameParameters);
    this.m_state.LastFindGameParameters = (FindGameResult) null;
  }

  private static bool RequiresScenarioIdAttribute(PegasusShared.GameType gameType) => gameType == PegasusShared.GameType.GT_VS_FRIEND || GameUtils.IsTavernBrawlGameType(gameType);

  public void CancelFindGame()
  {
    if (this.m_state.FindingBnetGameType == BnetGameType.BGT_UNKNOWN)
      return;
    if (!Network.IsLoggedIn())
    {
      this.m_state.FindingBnetGameType = BnetGameType.BGT_UNKNOWN;
    }
    else
    {
      if (!this.IsNoAccountTutorialGame(this.GetFindingBnetGameType()))
        Blizzard.GameService.SDK.Client.Integration.BattleNet.CancelMatchmaking();
      this.m_state.FindingBnetGameType = BnetGameType.BGT_UNKNOWN;
    }
  }

  private bool IsNoAccountTutorialGame(BnetGameType gameType) => !Network.ShouldBeConnectedToAurora() && gameType == BnetGameType.BGT_TUTORIAL;

  private void SendGameServerHandshake(GameServerInfo gameInfo)
  {
    NetCache.Get().DispatchClientOptionsToServer();
    if (gameInfo.SpectatorMode)
    {
      BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
      this.m_connectApi.SendSpectatorGameHandshake(Blizzard.GameService.SDK.Client.Integration.BattleNet.GetVersion(), this.GetPlatformBuilder(), gameInfo, new BnetId()
      {
        Hi = myGameAccountId.High,
        Lo = myGameAccountId.Low
      });
    }
    else
      this.m_connectApi.SendGameHandshake(gameInfo, this.GetPlatformBuilder());
  }

  private void GoToNoAccountTutorialServer(int scenario)
  {
    GameServerInfo gameServer = new GameServerInfo();
    gameServer.Version = Blizzard.GameService.SDK.Client.Integration.BattleNet.GetVersion();
    if (Vars.Key("GameServerOverride.Active").GetBool(false))
    {
      gameServer.Address = Vars.Key("GameServerOverride.Address").GetStr("");
      gameServer.Port = Vars.Key("GameServerOverride.Port").GetUInt(0U);
      gameServer.AuroraPassword = "";
    }
    else
    {
      BnetRegion currentRegionId = MobileDeviceLocale.GetCurrentRegionId();
      if (HearthstoneApplication.GetMobileEnvironment() == MobileEnv.PRODUCTION)
      {
        string format;
        try
        {
          format = Network.RegionToTutorialName[currentRegionId];
        }
        catch (KeyNotFoundException ex)
        {
          Debug.LogWarning((object) ("No matching tutorial server name found for region " + (object) currentRegionId));
          format = "us";
        }
        gameServer.Address = string.Format(format, (object) Network.TutorialServer);
        gameServer.Port = 1119U;
      }
      else
      {
        MobileDeviceLocale.ConnectionData dataFromRegionId = MobileDeviceLocale.GetConnectionDataFromRegionId(currentRegionId, true);
        gameServer.Port = dataFromRegionId.tutorialPort;
        gameServer.Address = string.IsNullOrEmpty(dataFromRegionId.gameServerAddress) ? "10.130.126.28" : dataFromRegionId.gameServerAddress;
        gameServer.Version = dataFromRegionId.version;
      }
      Log.Net.Print(string.Format("Connecting to account-free tutorial server for region {0}.  Address: {1}  Port: {2}  Version: {3}", (object) currentRegionId, (object) gameServer.Address, (object) gameServer.Port, (object) gameServer.Version));
      gameServer.AuroraPassword = "";
    }
    gameServer.GameHandle = 0U;
    gameServer.ClientHandle = 0L;
    gameServer.Mission = scenario;
    gameServer.BrawlLibraryItemId = 0;
    this.ResolveAddressAndGotoGameServer(gameServer);
  }

  private void ResolveAddressAndGotoGameServer(GameServerInfo gameServer)
  {
    IPAddress address1;
    if (IPAddress.TryParse(gameServer.Address, out address1))
    {
      gameServer.Address = address1.ToString();
      Network.Get().GotoGameServer(gameServer, false);
    }
    else
    {
      try
      {
        IPHostEntry hostEntry = Dns.GetHostEntry(gameServer.Address);
        if (hostEntry.AddressList.Length != 0)
        {
          IPAddress address2 = hostEntry.AddressList[0];
          gameServer.Address = address2.ToString();
          Network.Get().GotoGameServer(gameServer, false);
          return;
        }
      }
      catch (Exception ex)
      {
        this.m_state.LogSource.LogError("Exception within ResolveAddressAndGotoGameServer: " + ex.Message);
      }
      this.ThrowDnsResolveError(gameServer.Address);
    }
  }

  private void ThrowDnsResolveError(string environment)
  {
    if (HearthstoneApplication.IsInternal())
      Error.AddDevFatal("Environment " + environment + " could not be resolved! Please check your environment and Internet connection!");
    else
      Error.AddFatal(FatalErrorReason.DNS_RESOLVE, "GLOBAL_ERROR_NETWORK_NO_CONNECTION");
  }

  public Network.GameCancelInfo GetGameCancelInfo()
  {
    GameCanceled gameCancelInfo = this.m_connectApi.GetGameCancelInfo();
    if (gameCancelInfo == null)
      return (Network.GameCancelInfo) null;
    return new Network.GameCancelInfo()
    {
      CancelReason = (Network.GameCancelInfo.Reason) gameCancelInfo.Reason_
    };
  }

  public void GetGameState() => this.m_connectApi.GetGameState();

  public void UpdateBattlegroundInfo() => this.m_connectApi.UpdateBattlegroundInfo();

  public void UpdateBattlegroundHeroArmorTierList() => this.m_connectApi.UpdateBattlegroundHeroArmorTierList();

  public void SetBattlegroundHeroBuddyGained(int value, int playerId) => this.m_connectApi.SetBattlegroundHeroBuddyGained(value, playerId);

  public void SetBattlegroundHeroBuddyProgress(int progress, int playerId) => this.m_connectApi.SetBattlegroundHeroBuddyProgress(progress, playerId);

  public void ReplaceBattlegroundHero(int heroID, int playerId) => this.m_connectApi.ReplaceBattlegroundHero(heroID, playerId);

  public void RequestGameRoundHistory() => this.m_connectApi.RequestGameRoundHistory();

  public void RequestRealtimeBattlefieldRaces() => this.m_connectApi.RequestRealtimeBattlefieldRaces();

  public Network.TurnTimerInfo GetTurnTimerInfo()
  {
    PegasusGame.TurnTimer turnTimerInfo = this.m_connectApi.GetTurnTimerInfo();
    if (turnTimerInfo == null)
      return (Network.TurnTimerInfo) null;
    return new Network.TurnTimerInfo()
    {
      Seconds = (float) turnTimerInfo.Seconds,
      Turn = turnTimerInfo.Turn,
      Show = turnTimerInfo.Show
    };
  }

  public int GetNAckOption()
  {
    NAckOption nackOption = this.m_connectApi.GetNAckOption();
    return nackOption == null ? 0 : nackOption.Id;
  }

  public SpectatorNotify GetSpectatorNotify() => this.m_connectApi.GetSpectatorNotify();

  public AIDebugInformation GetAIDebugInformation() => this.m_connectApi.GetAIDebugInformation();

  public RopeTimerDebugInformation GetRopeTimerDebugInformation() => this.m_connectApi.GetRopeTimerDebugInformation();

  public ScriptDebugInformation GetScriptDebugInformation() => this.m_connectApi.GetScriptDebugInformation();

  public GameRoundHistory GetGameRoundHistory() => this.m_connectApi.GetGameRoundHistory();

  public GameRealTimeBattlefieldRaces GetGameRealTimeBattlefieldRaces() => this.m_connectApi.GetGameRealTimeBattlefieldRaces();

  public BattlegroundsRatingChange GetBattlegroundsRatingChange() => this.m_connectApi.GetBattlegroundsRatingChange();

  public GameGuardianVars GetGameGuardianVars() => this.m_connectApi.GetGameGuardianVars();

  public PegasusGame.UpdateBattlegroundInfo GetBattlegroundInfo() => this.m_connectApi.GetBattlegroundInfo();

  public PegasusGame.GetBattlegroundHeroArmorTierList GetBattlegroundHeroArmorTierList() => this.m_connectApi.GetBattlegroundHeroArmorTierList();

  public DebugMessage GetDebugMessage() => this.m_connectApi.GetDebugMessage();

  public ScriptLogMessage GetScriptLogMessage() => this.m_connectApi.GetScriptLogMessage();

  public AchievementProgress GetAchievementInGameProgress() => this.m_connectApi.GetAchievementInGameProgress();

  public AchievementComplete GetAchievementComplete() => this.m_connectApi.GetAchievementComplete();

  public void SendClientScriptGameEvent(ClientScriptGameEventType eventType, int data) => this.m_connectApi.SendClientScriptGameEvent(eventType, data);

  public void DisconnectFromGameServer()
  {
    if (!this.IsConnectedToGameServer())
      return;
    this.m_disconnectRequested = true;
    this.m_connectApi.DisconnectFromGameServer();
  }

  public bool WasDisconnectRequested() => this.m_disconnectRequested;

  public bool IsConnectedToGameServer() => this.m_connectApi.IsConnectedToGameServer();

  public bool GameServerHasEvents() => this.m_connectApi.GameServerHasEvents();

  public bool WasGameConceded() => this.m_gameConceded;

  public void Concede()
  {
    this.m_gameConceded = true;
    this.m_connectApi.Concede();
  }

  public void AutoConcede()
  {
    if (!this.IsConnectedToGameServer() || this.WasGameConceded())
      return;
    this.Concede();
  }

  public Network.EntityChoices GetEntityChoices()
  {
    PegasusGame.EntityChoices entityChoices = this.m_connectApi.GetEntityChoices();
    if (entityChoices == null)
      return (Network.EntityChoices) null;
    return new Network.EntityChoices()
    {
      ID = entityChoices.Id,
      ChoiceType = (CHOICE_TYPE) entityChoices.ChoiceType,
      CountMax = entityChoices.CountMax,
      CountMin = entityChoices.CountMin,
      Entities = this.CopyIntList((IList<int>) entityChoices.Entities),
      Source = entityChoices.Source,
      PlayerId = entityChoices.PlayerId,
      HideChosen = entityChoices.HideChosen
    };
  }

  public Network.EntitiesChosen GetEntitiesChosen()
  {
    PegasusGame.EntitiesChosen entitiesChosen = this.m_connectApi.GetEntitiesChosen();
    if (entitiesChosen == null)
      return (Network.EntitiesChosen) null;
    return new Network.EntitiesChosen()
    {
      ID = entitiesChosen.ChooseEntities.Id,
      Entities = this.CopyIntList((IList<int>) entitiesChosen.ChooseEntities.Entities),
      PlayerId = entitiesChosen.PlayerId,
      ChoiceType = (CHOICE_TYPE) entitiesChosen.ChoiceType
    };
  }

  public void SendChoices(int id, List<int> picks) => this.m_connectApi.SendChoices(id, picks);

  public void SendPreRefreshBGHeroes() => this.m_connectApi.SendPreRefreshBGHeroes();

  public void SendOption(int id, int index, int target, int sub, int pos) => this.m_connectApi.SendOption(id, index, target, sub, pos);

  public void SendFreeDeckChoice(int deckTemplateId) => this.m_connectApi.SendFreeDeckChoice(deckTemplateId);

  public Network.Options GetOptions()
  {
    AllOptions allOptions = this.m_connectApi.GetAllOptions();
    Network.Options options = new Network.Options()
    {
      ID = allOptions.Id
    };
    for (int index1 = 0; index1 < allOptions.Options.Count; ++index1)
    {
      PegasusGame.Option option1 = allOptions.Options[index1];
      Network.Options.Option option2 = new Network.Options.Option();
      option2.Type = (Network.Options.Option.OptionType) option1.Type_;
      if (option1.HasMainOption)
      {
        option2.Main.ID = option1.MainOption.Id;
        option2.Main.PlayErrorInfo.PlayError = (PlayErrors.ErrorType) option1.MainOption.PlayError;
        option2.Main.PlayErrorInfo.PlayErrorParam = option1.MainOption.HasPlayErrorParam ? new int?(option1.MainOption.PlayErrorParam) : new int?();
        option2.Main.Targets = this.CopyTargetOptionList((IList<PegasusGame.TargetOption>) option1.MainOption.Targets);
      }
      for (int index2 = 0; index2 < option1.SubOptions.Count; ++index2)
      {
        PegasusGame.SubOption subOption = option1.SubOptions[index2];
        option2.Subs.Add(new Network.Options.Option.SubOption()
        {
          ID = subOption.Id,
          PlayErrorInfo = {
            PlayError = (PlayErrors.ErrorType) subOption.PlayError,
            PlayErrorParam = subOption.HasPlayErrorParam ? new int?(subOption.PlayErrorParam) : new int?()
          },
          Targets = this.CopyTargetOptionList((IList<PegasusGame.TargetOption>) subOption.Targets)
        });
      }
      options.List.Add(option2);
    }
    return options;
  }

  private List<Network.Options.Option.TargetOption> CopyTargetOptionList(
    IList<PegasusGame.TargetOption> originalList)
  {
    List<Network.Options.Option.TargetOption> targetOptionList = new List<Network.Options.Option.TargetOption>();
    for (int index = 0; index < originalList.Count; ++index)
    {
      PegasusGame.TargetOption original = originalList[index];
      Network.Options.Option.TargetOption targetOption = new Network.Options.Option.TargetOption();
      targetOption.CopyFrom(original);
      targetOptionList.Add(targetOption);
    }
    return targetOptionList;
  }

  private List<int> CopyIntList(IList<int> intList)
  {
    int[] numArray = new int[intList.Count];
    intList.CopyTo(numArray, 0);
    return new List<int>((IEnumerable<int>) numArray);
  }

  public void SendUserUI(int overCard, int heldCard, int arrowOrigin, int x, int y)
  {
    if (NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.ShowUserUI == 0)
      return;
    this.m_connectApi.SendUserUi(overCard, heldCard, arrowOrigin, x, y);
  }

  public void SendEmote(EmoteType emote) => this.m_connectApi.SendEmote((int) emote);

  public void SendBattlegroundsEmote(EmoteType emote, int battlegroundsEmoteId) => this.m_connectApi.SendBattlegroundsEmote((int) emote, battlegroundsEmoteId);

  public void SendSelection(int selectedEntityId) => this.m_connectApi.SendSelection(selectedEntityId);

  public void SendRemoveSpectators(
    bool regenerateSpectatorPassword,
    params BnetGameAccountId[] bnetGameAccountIds)
  {
    List<BnetId> spectators = new List<BnetId>();
    for (int index = 0; index < bnetGameAccountIds.Length; ++index)
      spectators.Add(new BnetId()
      {
        Hi = bnetGameAccountIds[index].High,
        Lo = bnetGameAccountIds[index].Low
      });
    this.m_connectApi.SendRemoveSpectators(regenerateSpectatorPassword, spectators);
  }

  public Network.UserUI GetUserUI()
  {
    PegasusGame.UserUI userUi1 = this.m_connectApi.GetUserUi();
    if (userUi1 == null)
      return (Network.UserUI) null;
    Network.UserUI userUi2 = new Network.UserUI();
    if (userUi1.HasPlayerId)
      userUi2.playerId = new int?(userUi1.PlayerId);
    if (userUi1.HasMouseInfo)
    {
      PegasusGame.MouseInfo mouseInfo = userUi1.MouseInfo;
      userUi2.mouseInfo = new Network.UserUI.MouseInfo();
      userUi2.mouseInfo.ArrowOriginID = mouseInfo.ArrowOrigin;
      userUi2.mouseInfo.HeldCardID = mouseInfo.HeldCard;
      userUi2.mouseInfo.OverCardID = mouseInfo.OverCard;
      userUi2.mouseInfo.X = mouseInfo.X;
      userUi2.mouseInfo.Y = mouseInfo.Y;
    }
    else if (userUi1.HasEmote)
    {
      userUi2.emoteInfo = new Network.UserUI.EmoteInfo();
      userUi2.emoteInfo.Emote = userUi1.Emote;
      if (userUi1.HasBattlegroundsEmoteId)
        userUi2.emoteInfo.BattlegroundsEmoteId = userUi1.BattlegroundsEmoteId;
    }
    else if (userUi1.HasSelectedEntityId)
    {
      userUi2.selectionInfo = new Network.UserUI.SelectionInfo();
      userUi2.selectionInfo.SelectedEntityID = userUi1.SelectedEntityId;
    }
    return userUi2;
  }

  public Network.GameSetup GetGameSetupInfo()
  {
    PegasusGame.GameSetup gameSetup = this.m_connectApi.GetGameSetup();
    if (gameSetup == null)
      return (Network.GameSetup) null;
    Network.GameSetup gameSetupInfo = new Network.GameSetup();
    gameSetupInfo.Board = gameSetup.Board;
    gameSetupInfo.BoardLayout = gameSetup.BoardLayout;
    gameSetupInfo.BaconFavoriteBoardSkin = gameSetup.BaconFavoriteBoardSkin;
    gameSetupInfo.MaxSecretZoneSizePerPlayer = gameSetup.MaxSecretZoneSizePerPlayer;
    gameSetupInfo.MaxSecretsPerPlayer = gameSetup.MaxSecretsPerPlayer;
    gameSetupInfo.MaxQuestsPerPlayer = gameSetup.MaxQuestsPerPlayer;
    gameSetupInfo.MaxFriendlyMinionsPerPlayer = gameSetup.MaxFriendlyMinionsPerPlayer;
    this.m_gameServerKeepAliveFrequencySeconds = !gameSetup.HasKeepAliveFrequencySeconds ? 0U : gameSetup.KeepAliveFrequencySeconds;
    this.m_gameServerKeepAliveRetry = !gameSetup.HasKeepAliveRetry ? 1U : gameSetup.KeepAliveRetry;
    this.m_gameServerKeepAliveWaitForInternetSeconds = !gameSetup.HasKeepAliveWaitForInternetSeconds ? 20U : gameSetup.KeepAliveWaitForInternetSeconds;
    if (gameSetup.HasDisconnectWhenStuckSeconds)
      gameSetupInfo.DisconnectWhenStuckSeconds = gameSetup.DisconnectWhenStuckSeconds;
    return gameSetupInfo;
  }

  public List<Network.PowerHistory> GetPowerHistory()
  {
    PegasusGame.PowerHistory powerHistory1 = this.m_connectApi.GetPowerHistory();
    if (powerHistory1 == null)
      return (List<Network.PowerHistory>) null;
    List<Network.PowerHistory> powerHistory2 = new List<Network.PowerHistory>();
    for (int index = 0; index < powerHistory1.List.Count; ++index)
    {
      PowerHistoryData powerHistoryData = powerHistory1.List[index];
      Network.PowerHistory powerHistory3 = (Network.PowerHistory) null;
      if (powerHistoryData.HasFullEntity)
        powerHistory3 = (Network.PowerHistory) Network.GetFullEntity(powerHistoryData.FullEntity);
      else if (powerHistoryData.HasShowEntity)
        powerHistory3 = (Network.PowerHistory) Network.GetShowEntity(powerHistoryData.ShowEntity);
      else if (powerHistoryData.HasHideEntity)
        powerHistory3 = (Network.PowerHistory) Network.GetHideEntity(powerHistoryData.HideEntity);
      else if (powerHistoryData.HasChangeEntity)
        powerHistory3 = (Network.PowerHistory) Network.GetChangeEntity(powerHistoryData.ChangeEntity);
      else if (powerHistoryData.HasTagChange)
        powerHistory3 = (Network.PowerHistory) Network.GetTagChange(powerHistoryData.TagChange);
      else if (powerHistoryData.HasPowerStart)
        powerHistory3 = (Network.PowerHistory) Network.GetBlockStart(powerHistoryData.PowerStart);
      else if (powerHistoryData.HasPowerEnd)
        powerHistory3 = (Network.PowerHistory) Network.GetBlockEnd(powerHistoryData.PowerEnd);
      else if (powerHistoryData.HasCreateGame)
        powerHistory3 = (Network.PowerHistory) Network.GetCreateGame(powerHistoryData.CreateGame);
      else if (powerHistoryData.HasResetGame)
        powerHistory3 = (Network.PowerHistory) Network.GetResetGame(powerHistoryData.ResetGame);
      else if (powerHistoryData.HasMetaData)
        powerHistory3 = (Network.PowerHistory) Network.GetMetaData(powerHistoryData.MetaData);
      else if (powerHistoryData.HasSubSpellStart)
        powerHistory3 = (Network.PowerHistory) Network.GetSubSpellStart(powerHistoryData.SubSpellStart);
      else if (powerHistoryData.HasSubSpellEnd)
        powerHistory3 = (Network.PowerHistory) Network.GetSubSpellEnd(powerHistoryData.SubSpellEnd);
      else if (powerHistoryData.HasVoSpell)
        powerHistory3 = (Network.PowerHistory) Network.GetVoSpell(powerHistoryData.VoSpell);
      else if (powerHistoryData.HasVoBanter)
        powerHistory3 = (Network.PowerHistory) Network.GetVoBanter(powerHistoryData.VoBanter);
      else if (powerHistoryData.HasCachedTagForDormantChange)
        powerHistory3 = (Network.PowerHistory) Network.GetCachedTagForDormantChange(powerHistoryData.CachedTagForDormantChange);
      else if (powerHistoryData.HasShuffleDeck)
        powerHistory3 = (Network.PowerHistory) Network.GetShuffleDeck(powerHistoryData.ShuffleDeck);
      else
        Debug.LogError((object) "Network.GetPowerHistory() - received invalid PowerHistoryData packet");
      if (powerHistory3 != null)
        powerHistory2.Add(powerHistory3);
    }
    return powerHistory2;
  }

  private static Network.HistFullEntity GetFullEntity(PowerHistoryEntity entity) => new Network.HistFullEntity()
  {
    Entity = Network.Entity.CreateFromProto(entity)
  };

  private static Network.HistShowEntity GetShowEntity(PowerHistoryEntity entity) => new Network.HistShowEntity()
  {
    Entity = Network.Entity.CreateFromProto(entity)
  };

  private static Network.HistHideEntity GetHideEntity(PowerHistoryHide hide) => new Network.HistHideEntity()
  {
    Entity = hide.Entity,
    Zone = hide.Zone
  };

  private static Network.HistChangeEntity GetChangeEntity(PowerHistoryEntity entity) => new Network.HistChangeEntity()
  {
    Entity = Network.Entity.CreateFromProto(entity)
  };

  private static Network.HistTagChange GetTagChange(PowerHistoryTagChange tagChange) => new Network.HistTagChange()
  {
    Entity = tagChange.Entity,
    Tag = tagChange.Tag,
    Value = tagChange.Value,
    ChangeDef = tagChange.ChangeDef
  };

  private static Network.HistBlockStart GetBlockStart(PowerHistoryStart start) => new Network.HistBlockStart(start.Type)
  {
    Entities = new List<int>() { start.Source },
    Target = start.Target,
    SubOption = start.SubOption,
    EffectCardId = new List<string>() { start.EffectCardId },
    IsEffectCardIdClientCached = new List<bool>() { false },
    EffectIndex = start.EffectIndex,
    TriggerKeyword = start.TriggerKeyword,
    ShowInHistory = start.ShowInHistory,
    IsDeferrable = start.IsDeferrable,
    IsBatchable = start.IsBatchable,
    IsDeferBlocker = start.IsDeferBlocker,
    ForceShowBigCard = start.ForceShowBigCard
  };

  private static Network.HistBlockEnd GetBlockEnd(PowerHistoryEnd end) => new Network.HistBlockEnd();

  private static Network.HistCreateGame GetCreateGame(PowerHistoryCreateGame createGame) => Network.HistCreateGame.CreateFromProto(createGame);

  private static Network.HistResetGame GetResetGame(PowerHistoryResetGame resetGame) => Network.HistResetGame.CreateFromProto(resetGame);

  private static Network.HistMetaData GetMetaData(PowerHistoryMetaData metaData)
  {
    Network.HistMetaData metaData1 = new Network.HistMetaData();
    metaData1.MetaType = metaData.HasType ? metaData.Type : HistoryMeta.Type.TARGET;
    metaData1.Data = metaData.HasData ? metaData.Data : 0;
    for (int index = 0; index < metaData.Info.Count; ++index)
    {
      int num = metaData.Info[index];
      metaData1.Info.Add(num);
    }
    for (int index = 0; index < metaData.AdditionalData.Count; ++index)
    {
      int num = metaData.AdditionalData[index];
      metaData1.AdditionalData.Add(num);
    }
    return metaData1;
  }

  private static Network.HistSubSpellStart GetSubSpellStart(
    PowerHistorySubSpellStart subSpellStart)
  {
    return Network.HistSubSpellStart.CreateFromProto(subSpellStart);
  }

  private static Network.HistSubSpellEnd GetSubSpellEnd(PowerHistorySubSpellEnd subSpellEnd) => new Network.HistSubSpellEnd();

  private static Network.HistVoSpell GetVoSpell(PowerHistoryVoTask voSubspellTask) => Network.HistVoSpell.CreateFromProto(voSubspellTask);

  private static Network.HistVoBanter GetVoBanter(PowerHistoryVoBanter voBanterTask) => Network.HistVoBanter.CreateFromProto(voBanterTask);

  private static Network.HistCachedTagForDormantChange GetCachedTagForDormantChange(
    PowerHistoryCachedTagForDormantChange tagChange)
  {
    return Network.HistCachedTagForDormantChange.CreateFromProto(tagChange);
  }

  private static Network.HistShuffleDeck GetShuffleDeck(PowerHistoryShuffleDeck shuffleDeck) => Network.HistShuffleDeck.CreateFromProto(shuffleDeck);

  public void ValidateAchieve(int achieveID)
  {
    Log.Achievements.Print("Validating achieve: " + (object) achieveID);
    this.m_connectApi.ValidateAchieve(achieveID);
  }

  public ValidateAchieveResponse GetValidatedAchieve() => this.m_connectApi.GetValidateAchieveResponse();

  public void RequestCancelQuest(int achieveID) => this.m_connectApi.RequestCancelQuest(achieveID);

  public Network.CanceledQuest GetCanceledQuest()
  {
    CancelQuestResponse canceledQuestResponse = this.m_connectApi.GetCanceledQuestResponse();
    if (canceledQuestResponse == null)
      return (Network.CanceledQuest) null;
    return new Network.CanceledQuest()
    {
      AchieveID = canceledQuestResponse.QuestId,
      Canceled = canceledQuestResponse.Success,
      NextQuestCancelDate = canceledQuestResponse.HasNextQuestCancel ? TimeUtils.PegDateToFileTimeUtc(canceledQuestResponse.NextQuestCancel) : 0L
    };
  }

  public Network.TriggeredEvent GetTriggerEventResponse()
  {
    TriggerEventResponse triggerEventResponse = this.m_connectApi.GetTriggerEventResponse();
    if (triggerEventResponse == null)
      return (Network.TriggeredEvent) null;
    return new Network.TriggeredEvent()
    {
      EventID = triggerEventResponse.EventId,
      Success = triggerEventResponse.Success
    };
  }

  public void RequestAdventureProgress() => this.m_connectApi.RequestAdventureProgress();

  public List<Network.AdventureProgress> GetAdventureProgressResponse()
  {
    AdventureProgressResponse progressResponse1 = this.m_connectApi.GetAdventureProgressResponse();
    if (progressResponse1 == null)
      return (List<Network.AdventureProgress>) null;
    List<Network.AdventureProgress> progressResponse2 = new List<Network.AdventureProgress>();
    for (int index = 0; index < progressResponse1.List.Count; ++index)
    {
      PegasusShared.AdventureProgress adventureProgress = progressResponse1.List[index];
      progressResponse2.Add(new Network.AdventureProgress()
      {
        Wing = adventureProgress.WingId,
        Progress = adventureProgress.Progress,
        Ack = adventureProgress.Ack,
        Flags = adventureProgress.Flags_
      });
    }
    return progressResponse2;
  }

  public Network.BeginDraft GetBeginDraft()
  {
    DraftBeginning draftBeginning = this.m_connectApi.GetDraftBeginning();
    if (draftBeginning == null)
      return (Network.BeginDraft) null;
    Network.BeginDraft beginDraft = new Network.BeginDraft();
    beginDraft.DeckID = draftBeginning.DeckId;
    for (int index = 0; index < draftBeginning.ChoiceList.Count; ++index)
    {
      PegasusShared.CardDef choice = draftBeginning.ChoiceList[index];
      NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition()
      {
        Name = GameUtils.TranslateDbIdToCardId(choice.Asset),
        Premium = (TAG_PREMIUM) choice.Premium
      };
      beginDraft.Heroes.Add(cardDefinition);
    }
    beginDraft.Wins = draftBeginning.HasCurrentSession ? draftBeginning.CurrentSession.Wins : 0;
    beginDraft.MaxSlot = draftBeginning.MaxSlot;
    if (draftBeginning.HasCurrentSession)
      beginDraft.Session = draftBeginning.CurrentSession;
    beginDraft.SlotType = draftBeginning.SlotType;
    beginDraft.UniqueSlotTypesForDraft = draftBeginning.UniqueSlotTypes;
    return beginDraft;
  }

  public DraftError GetDraftError() => this.m_connectApi.DraftGetError();

  public Network.DraftChoicesAndContents GetDraftChoicesAndContents()
  {
    PegasusUtil.DraftChoicesAndContents choicesAndContents1 = this.m_connectApi.GetDraftChoicesAndContents();
    if (choicesAndContents1 == null)
      return (Network.DraftChoicesAndContents) null;
    Network.DraftChoicesAndContents choicesAndContents2 = new Network.DraftChoicesAndContents();
    choicesAndContents2.DeckInfo.Deck = choicesAndContents1.DeckId;
    choicesAndContents2.Slot = choicesAndContents1.Slot;
    choicesAndContents2.Hero.Name = choicesAndContents1.HeroDef.Asset == 0 ? string.Empty : GameUtils.TranslateDbIdToCardId(choicesAndContents1.HeroDef.Asset);
    choicesAndContents2.Hero.Premium = (TAG_PREMIUM) choicesAndContents1.HeroDef.Premium;
    choicesAndContents2.Wins = choicesAndContents1.CurrentSession.Wins;
    choicesAndContents2.Losses = choicesAndContents1.CurrentSession.Losses;
    choicesAndContents2.MaxWins = choicesAndContents1.HasMaxWins ? choicesAndContents1.MaxWins : int.MaxValue;
    choicesAndContents2.MaxSlot = choicesAndContents1.MaxSlot;
    if (choicesAndContents1.HasCurrentSession)
      choicesAndContents2.Session = choicesAndContents1.CurrentSession;
    if (choicesAndContents1.HasHeroPowerDef)
      choicesAndContents2.HeroPower.Name = choicesAndContents1.HeroPowerDef.Asset == 0 ? string.Empty : GameUtils.TranslateDbIdToCardId(choicesAndContents1.HeroPowerDef.Asset);
    for (int index = 0; index < choicesAndContents1.ChoiceList.Count; ++index)
    {
      PegasusShared.CardDef choice = choicesAndContents1.ChoiceList[index];
      if (choice.Asset != 0)
      {
        NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition()
        {
          Name = GameUtils.TranslateDbIdToCardId(choice.Asset),
          Premium = (TAG_PREMIUM) choice.Premium
        };
        choicesAndContents2.Choices.Add(cardDefinition);
      }
    }
    for (int index = 0; index < choicesAndContents1.Cards.Count; ++index)
    {
      DeckCardData card = choicesAndContents1.Cards[index];
      choicesAndContents2.DeckInfo.Cards.Add(new Network.CardUserData()
      {
        DbId = card.Def.Asset,
        Count = card.HasQty ? card.Qty : 1,
        Premium = card.Def.HasPremium ? (TAG_PREMIUM) card.Def.Premium : TAG_PREMIUM.NORMAL
      });
    }
    choicesAndContents2.Chest = choicesAndContents1.HasChest ? Network.ConvertRewardChest(choicesAndContents1.Chest) : (Network.RewardChest) null;
    choicesAndContents2.SlotType = choicesAndContents1.SlotType;
    choicesAndContents2.UniqueSlotTypesForDraft.AddRange((IEnumerable<DraftSlotType>) choicesAndContents1.UniqueSlotTypes);
    return choicesAndContents2;
  }

  public Network.DraftChosen GetDraftChosen()
  {
    PegasusUtil.DraftChosen draftChosen = this.m_connectApi.GetDraftChosen();
    if (draftChosen == null)
      return (Network.DraftChosen) null;
    NetCache.CardDefinition cardDefinition1 = new NetCache.CardDefinition()
    {
      Name = GameUtils.TranslateDbIdToCardId(draftChosen.Chosen.Asset),
      Premium = (TAG_PREMIUM) draftChosen.Chosen.Premium
    };
    List<NetCache.CardDefinition> cardDefinitionList = new List<NetCache.CardDefinition>();
    for (int index = 0; index < draftChosen.NextChoiceList.Count; ++index)
    {
      PegasusShared.CardDef nextChoice = draftChosen.NextChoiceList[index];
      NetCache.CardDefinition cardDefinition2 = new NetCache.CardDefinition()
      {
        Name = GameUtils.TranslateDbIdToCardId(nextChoice.Asset),
        Premium = (TAG_PREMIUM) nextChoice.Premium
      };
      cardDefinitionList.Add(cardDefinition2);
    }
    return new Network.DraftChosen()
    {
      ChosenCard = cardDefinition1,
      NextChoices = cardDefinitionList,
      SlotType = draftChosen.SlotType
    };
  }

  public void MakeDraftChoice(long deckID, int slot, int index, int premium) => this.m_connectApi.DraftMakePick(deckID, slot, index, premium);

  public void RequestDraftChoicesAndContents() => this.m_connectApi.RequestDraftChoicesAndContents();

  public void SendArenaSessionRequest() => this.m_connectApi.SendArenaSessionRequest();

  public ArenaSessionResponse GetArenaSessionResponse() => this.m_connectApi.GetArenaSessionResponse();

  public void DraftBegin() => this.m_connectApi.DraftBegin();

  public void DraftRetire(long deckID, int slot, int seasonId) => this.m_connectApi.DraftRetire(deckID, slot, seasonId);

  public Network.DraftRetired GetRetiredDraft()
  {
    PegasusUtil.DraftRetired draftRetired = this.m_connectApi.GetDraftRetired();
    if (draftRetired == null)
      return (Network.DraftRetired) null;
    return new Network.DraftRetired()
    {
      Deck = draftRetired.DeckId,
      Chest = Network.ConvertRewardChest(draftRetired.Chest)
    };
  }

  public void AckDraftRewards(long deckID, int slot) => this.m_connectApi.DraftAckRewards(deckID, slot);

  public long GetRewardsAckDraftID()
  {
    DraftRewardsAcked draftRewardsAcked = this.m_connectApi.DraftRewardsAcked();
    return draftRewardsAcked == null ? 0L : draftRewardsAcked.DeckId;
  }

  public void DraftRequestDisablePremiums() => this.m_connectApi.DraftRequestDisablePremiums();

  public Network.DraftChoicesAndContents GetDraftRemovePremiumsResponse()
  {
    DraftRemovePremiumsResponse premiumsResponse1 = this.m_connectApi.GetDraftDisablePremiumsResponse();
    Network.DraftChoicesAndContents premiumsResponse2 = new Network.DraftChoicesAndContents();
    for (int index = 0; index < premiumsResponse1.ChoiceList.Count; ++index)
    {
      PegasusShared.CardDef choice = premiumsResponse1.ChoiceList[index];
      if (choice.Asset != 0)
      {
        NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition()
        {
          Name = GameUtils.TranslateDbIdToCardId(choice.Asset),
          Premium = (TAG_PREMIUM) choice.Premium
        };
        premiumsResponse2.Choices.Add(cardDefinition);
      }
    }
    for (int index = 0; index < premiumsResponse1.Cards.Count; ++index)
    {
      DeckCardData card = premiumsResponse1.Cards[index];
      premiumsResponse2.DeckInfo.Cards.Add(new Network.CardUserData()
      {
        DbId = card.Def.Asset,
        Count = card.HasQty ? card.Qty : 1,
        Premium = card.Def.HasPremium ? (TAG_PREMIUM) card.Def.Premium : TAG_PREMIUM.NORMAL
      });
    }
    return premiumsResponse2;
  }

  public static Network.RewardChest ConvertRewardChest(PegasusShared.RewardChest chest)
  {
    Network.RewardChest rewardChest = new Network.RewardChest();
    for (int index = 0; index < chest.Bag.Count; ++index)
      rewardChest.Rewards.Add(Network.ConvertRewardBag(chest.Bag[index]));
    return rewardChest;
  }

  public static RewardData ConvertRewardBag(RewardBag bag)
  {
    if (bag.HasRewardBooster)
      return (RewardData) new BoosterPackRewardData(bag.RewardBooster.BoosterType, bag.RewardBooster.BoosterCount);
    if (bag.HasRewardCard)
      return (RewardData) new CardRewardData(GameUtils.TranslateDbIdToCardId(bag.RewardCard.Card.Asset), (TAG_PREMIUM) bag.RewardCard.Card.Premium, bag.RewardCard.Quantity);
    if (bag.HasRewardDust)
      return (RewardData) new ArcaneDustRewardData(bag.RewardDust.Amount);
    if (bag.HasRewardGold)
      return (RewardData) new GoldRewardData((long) bag.RewardGold.Amount);
    if (bag.HasRewardCardBack)
      return (RewardData) new CardBackRewardData(bag.RewardCardBack.CardBack);
    if (bag.HasRewardArenaTicket)
      return (RewardData) new ForgeTicketRewardData(bag.RewardArenaTicket.Quantity);
    if (bag.HasRewardMercenariesCurrency)
      return (RewardData) new MercenaryCoinRewardData(bag.RewardMercenariesCurrency.MercenaryId, (int) bag.RewardMercenariesCurrency.CurrencyDelta);
    if (bag.HasRewardMercenariesExperience)
      return (RewardData) new MercenaryExpRewardData(bag.RewardMercenariesExperience.MercenaryId, (int) bag.RewardMercenariesExperience.PreExp, (int) bag.RewardMercenariesExperience.PostExp, (int) bag.RewardMercenariesExperience.ExpDelta);
    if (bag.HasRewardMercenariesEquipment)
      return (RewardData) new MercenariesEquipmentRewardData(bag.RewardMercenariesEquipment.MercenaryId, bag.RewardMercenariesEquipment.EquipmentId, (int) bag.RewardMercenariesEquipment.EquipmentTier);
    if (bag.HasRewardRandomMercenary)
      return RewardUtils.CreateMercenaryOrKnockoutRewardData(bag.RewardRandomMercenary.MercenaryId, bag.RewardRandomMercenary.ArtVariationId, (TAG_PREMIUM) bag.RewardRandomMercenary.ArtVariationPremium, (int) bag.RewardRandomMercenary.CurrencyAmount);
    if (bag.HasRewardRenown)
      return (RewardData) RewardUtils.CreateMercenaryRenownRewardData(bag.RewardRenown.Amount);
    Debug.LogError((object) "Unrecognized reward bag reward");
    return (RewardData) null;
  }

  public void MassDisenchant() => this.m_connectApi.MassDisenchant();

  public Network.MassDisenchantResponse GetMassDisenchantResponse()
  {
    PegasusUtil.MassDisenchantResponse disenchantResponse = this.m_connectApi.GetMassDisenchantResponse();
    if (disenchantResponse == null)
      return (Network.MassDisenchantResponse) null;
    if (disenchantResponse.HasCollectionVersion)
      NetCache.Get().AddExpectedCollectionModification(disenchantResponse.CollectionVersion);
    return new Network.MassDisenchantResponse()
    {
      Amount = disenchantResponse.Amount
    };
  }

  public void SetFavoriteHero(TAG_CLASS heroClass, NetCache.CardDefinition hero, bool isFavorite)
  {
    PegasusShared.CardDef cardDef = new PegasusShared.CardDef()
    {
      Asset = GameUtils.TranslateCardIdToDbId(hero.Name),
      Premium = (int) hero.Premium
    };
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.SetFavoriteHero((int) heroClass, cardDef, isFavorite);
    }
    else
    {
      OfflineDataCache.SetFavoriteHero((int) heroClass, cardDef, true, isFavorite);
      NetCache.NetCacheFavoriteHeroes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFavoriteHeroes>();
      if (netObject == null)
        return;
      if (isFavorite)
        netObject.FavoriteHeroes.Add((heroClass, hero));
      else
        netObject.FavoriteHeroes.Remove((heroClass, hero));
    }
  }

  public void SetTag(int tagID, int entityID, int tagValue) => this.SendDebugConsoleCommand(string.Format("settag {0} {1} {2}", (object) tagID, (object) entityID, (object) tagValue));

  public void SetTag(int tagID, string entityIdentifier, int tagValue) => this.SendDebugConsoleCommand(string.Format("settag {0} {1} {2}", (object) tagID, (object) entityIdentifier, (object) tagValue));

  public void PrintPersistentList(int entityID) => this.SendDebugConsoleCommand(string.Format("printpersistentlist {0}", (object) entityID));

  public void DebugScript(string powerGUID) => this.SendDebugConsoleCommand(string.Format("debugscript {0}", (object) powerGUID));

  public void DisableScriptDebug() => this.SendDebugConsoleCommand("disablescriptdebug");

  public void DebugRopeTimer() => this.SendDebugConsoleCommand("debugropetimer");

  public void DisableDebugRopeTimer() => this.SendDebugConsoleCommand("disabledebugropetimer");

  public Network.SetFavoriteHeroResponse GetSetFavoriteHeroResponse()
  {
    PegasusUtil.SetFavoriteHeroResponse favoriteHeroResponse1 = this.m_connectApi.GetSetFavoriteHeroResponse();
    if (favoriteHeroResponse1 == null)
      return (Network.SetFavoriteHeroResponse) null;
    Network.SetFavoriteHeroResponse favoriteHeroResponse2 = new Network.SetFavoriteHeroResponse();
    favoriteHeroResponse2.Success = favoriteHeroResponse1.Success;
    favoriteHeroResponse2.IsFavorite = favoriteHeroResponse1.IsFavorite;
    if (favoriteHeroResponse1.HasFavoriteHero)
    {
      if (!Blizzard.T5.Core.Utils.EnumUtils.TryCast<TAG_CLASS>((object) favoriteHeroResponse1.FavoriteHero.ClassId, out favoriteHeroResponse2.HeroClass))
        Debug.LogWarning((object) string.Format("Network.GetSetFavoriteHeroResponse() invalid class {0}", (object) favoriteHeroResponse1.FavoriteHero.ClassId));
      TAG_PREMIUM outVal;
      if (!Blizzard.T5.Core.Utils.EnumUtils.TryCast<TAG_PREMIUM>((object) favoriteHeroResponse1.FavoriteHero.Hero.Premium, out outVal))
        Debug.LogWarning((object) string.Format("Network.GetSetFavoriteHeroResponse() invalid heroPremium {0}", (object) favoriteHeroResponse1.FavoriteHero.Hero.Premium));
      favoriteHeroResponse2.Hero = new NetCache.CardDefinition()
      {
        Name = GameUtils.TranslateDbIdToCardId(favoriteHeroResponse1.FavoriteHero.Hero.Asset),
        Premium = outVal
      };
    }
    return favoriteHeroResponse2;
  }

  public void RequestRecruitAFriendUrl() => this.m_connectApi.RequestRecruitAFriendUrl(this.GetPlatformBuilder());

  public RecruitAFriendURLResponse GetRecruitAFriendUrlResponse() => this.m_connectApi.GetRecruitAFriendUrlResponse();

  public void RequestRecruitAFriendData() => this.m_connectApi.RequestRecruitAFriendData();

  public RecruitAFriendDataResponse GetRecruitAFriendDataResponse() => this.m_connectApi.GetRecruitAFriendDataResponse();

  public void RequestProcessRecruitAFriend() => this.m_connectApi.RequestProcessRecruitAFriend();

  public Network.PurchaseCanceledResponse GetPurchaseCanceledResponse()
  {
    CancelPurchaseResponse purchaseResponse = this.m_connectApi.GetCancelPurchaseResponse();
    if (purchaseResponse == null)
      return (Network.PurchaseCanceledResponse) null;
    Network.PurchaseCanceledResponse canceledResponse = new Network.PurchaseCanceledResponse()
    {
      TransactionID = purchaseResponse.HasTransactionId ? purchaseResponse.TransactionId : 0L,
      PMTProductID = purchaseResponse.HasPmtProductId ? new long?(purchaseResponse.PmtProductId) : new long?(),
      CurrencyCode = purchaseResponse.CurrencyCode
    };
    switch (purchaseResponse.Result)
    {
      case CancelPurchaseResponse.CancelResult.CR_SUCCESS:
        canceledResponse.Result = Network.PurchaseCanceledResponse.CancelResult.SUCCESS;
        break;
      case CancelPurchaseResponse.CancelResult.CR_NOT_ALLOWED:
        canceledResponse.Result = Network.PurchaseCanceledResponse.CancelResult.NOT_ALLOWED;
        break;
      case CancelPurchaseResponse.CancelResult.CR_NOTHING_TO_CANCEL:
        canceledResponse.Result = Network.PurchaseCanceledResponse.CancelResult.NOTHING_TO_CANCEL;
        break;
    }
    return canceledResponse;
  }

  public Network.BattlePayStatus GetBattlePayStatusResponse()
  {
    BattlePayStatusResponse payStatusResponse1 = this.m_connectApi.GetBattlePayStatusResponse();
    if (payStatusResponse1 == null)
      return (Network.BattlePayStatus) null;
    Network.BattlePayStatus payStatusResponse2 = new Network.BattlePayStatus()
    {
      State = (Network.BattlePayStatus.PurchaseState) payStatusResponse1.Status,
      BattlePayAvailable = payStatusResponse1.BattlePayAvailable,
      CurrencyCode = payStatusResponse1.CurrencyCode
    };
    if (payStatusResponse1.HasTransactionId)
      payStatusResponse2.TransactionID = payStatusResponse1.TransactionId;
    if (payStatusResponse1.HasPmtProductId)
      payStatusResponse2.PMTProductID = new long?(payStatusResponse1.PmtProductId);
    if (payStatusResponse1.HasPurchaseError)
      payStatusResponse2.PurchaseError = this.ConvertPurchaseError(payStatusResponse1.PurchaseError);
    if (payStatusResponse1.HasThirdPartyId)
      payStatusResponse2.ThirdPartyID = payStatusResponse1.ThirdPartyId;
    if (payStatusResponse1.HasProvider)
      payStatusResponse2.Provider = new BattlePayProvider?(payStatusResponse1.Provider);
    return payStatusResponse2;
  }

  private Network.PurchaseErrorInfo ConvertPurchaseError(PurchaseError purchaseError)
  {
    Network.PurchaseErrorInfo purchaseErrorInfo = new Network.PurchaseErrorInfo()
    {
      Error = (Network.PurchaseErrorInfo.ErrorType) purchaseError.Error_
    };
    if (purchaseError.HasPurchaseInProgress)
      purchaseErrorInfo.PurchaseInProgressProductID = purchaseError.PurchaseInProgress;
    if (purchaseError.HasErrorCode)
      purchaseErrorInfo.ErrorCode = purchaseError.ErrorCode;
    return purchaseErrorInfo;
  }

  public Network.BattlePayConfig GetBattlePayConfigResponse()
  {
    BattlePayConfigResponse payConfigResponse1 = this.m_connectApi.GetBattlePayConfigResponse();
    if (payConfigResponse1 == null)
      return (Network.BattlePayConfig) null;
    Network.BattlePayConfig payConfigResponse2 = new Network.BattlePayConfig()
    {
      Available = !payConfigResponse1.HasUnavailable || !payConfigResponse1.Unavailable,
      SecondsBeforeAutoCancel = payConfigResponse1.HasSecsBeforeAutoCancel ? payConfigResponse1.SecsBeforeAutoCancel : StoreManager.DEFAULT_SECONDS_BEFORE_AUTO_CANCEL
    };
    if (payConfigResponse1.HasCheckoutKrOnestoreKey)
      payConfigResponse2.CheckoutKrOnestoreKey = payConfigResponse1.CheckoutKrOnestoreKey;
    foreach (PegasusShared.Currency currency1 in payConfigResponse1.Currencies)
    {
      Currency currency2 = new Currency(currency1);
      payConfigResponse2.Currencies.Add(currency2);
      if (currency2.Code == payConfigResponse1.DefaultCurrencyCode)
        payConfigResponse2.Currency = currency2;
    }
    foreach (PegasusUtil.Bundle bundle in payConfigResponse1.Bundles)
      payConfigResponse2.Bundles.Add(bundle.ToNetBundle(payConfigResponse2.Currency));
    foreach (PegasusUtil.GoldCostBooster goldCostBooster1 in payConfigResponse1.GoldCostBoosters)
    {
      Network.GoldCostBooster goldCostBooster2 = new Network.GoldCostBooster()
      {
        ID = goldCostBooster1.PackType
      };
      goldCostBooster2.Cost = goldCostBooster1.Cost <= 0L ? new long?() : new long?(goldCostBooster1.Cost);
      if (goldCostBooster1.HasBuyWithGoldEventName)
        goldCostBooster2.BuyWithGoldEvent = SpecialEventManager.Get().GetEventType(goldCostBooster1.BuyWithGoldEventName);
      payConfigResponse2.GoldCostBoosters.Add(goldCostBooster2);
    }
    payConfigResponse2.GoldCostArena = !payConfigResponse1.HasGoldCostArena || payConfigResponse1.GoldCostArena <= 0L ? new long?() : new long?(payConfigResponse1.GoldCostArena);
    if (payConfigResponse1.HasCheckoutOauthClientId && !string.IsNullOrEmpty(payConfigResponse1.CheckoutOauthClientId))
      payConfigResponse2.CommerceClientID = payConfigResponse1.CheckoutOauthClientId;
    payConfigResponse2.PersonalizedShopPages = payConfigResponse1.PersonalizedShopPages.Where<BattlePayConfigShopPage>((Func<BattlePayConfigShopPage, bool>) (p => !string.IsNullOrEmpty(p.PersonalizedShopPageId))).ToList<BattlePayConfigShopPage>();
    if (payConfigResponse1.LocaleMap != null)
    {
      foreach (LocaleMapEntry locale in payConfigResponse1.LocaleMap)
        payConfigResponse2.CatalogLocaleToGameLocale.Add(locale.CatalogLocaleId, (Locale) locale.GameLocaleId);
    }
    foreach (Locale locale in System.Enum.GetValues(typeof (Locale)))
    {
      if (locale != Locale.UNKNOWN && !payConfigResponse2.CatalogLocaleToGameLocale.ContainsValue(locale))
        Log.Store.PrintError("BattlePayConfig includes no catalog locale ID mapping for {0}", (object) locale.ToString());
    }
    payConfigResponse2.SaleList = CatalogDeserializer.DeserializeShopSaleList(payConfigResponse1.SaleList);
    payConfigResponse2.IgnoreProductTiming = payConfigResponse1.IgnoreProductTiming;
    return payConfigResponse2;
  }

  public void PurchaseViaGold(int quantity, ProductType productItemType, int data)
  {
    if (!Network.IsLoggedIn())
      Log.All.PrintError("Client attempted to make a gold purchase while offline!");
    else
      this.m_connectApi.PurchaseViaGold(quantity, productItemType, data);
  }

  public void GetPurchaseMethod(long? pmtProductId, int quantity, Currency currency) => this.m_connectApi.RequestPurchaseMethod(pmtProductId, quantity, currency.toProto(), SystemInfo.deviceUniqueIdentifier, this.GetPlatformBuilder());

  public void ConfirmPurchase() => this.m_connectApi.ConfirmPurchase();

  public void CancelBlizzardPurchase(
    bool isAutoCanceled,
    CancelPurchase.CancelReason? reason,
    string error)
  {
    this.m_connectApi.AbortBlizzardPurchase(SystemInfo.deviceUniqueIdentifier, isAutoCanceled, reason, error);
  }

  public Network.PurchaseMethod GetPurchaseMethodResponse()
  {
    PegasusUtil.PurchaseMethod purchaseMethodResponse1 = this.m_connectApi.GetPurchaseMethodResponse();
    if (purchaseMethodResponse1 == null)
      return (Network.PurchaseMethod) null;
    Network.PurchaseMethod purchaseMethodResponse2 = new Network.PurchaseMethod();
    if (purchaseMethodResponse1.HasTransactionId)
      purchaseMethodResponse2.TransactionID = purchaseMethodResponse1.TransactionId;
    if (purchaseMethodResponse1.HasPmtProductId)
      purchaseMethodResponse2.PMTProductID = new long?(purchaseMethodResponse1.PmtProductId);
    if (purchaseMethodResponse1.HasQuantity)
      purchaseMethodResponse2.Quantity = purchaseMethodResponse1.Quantity;
    purchaseMethodResponse2.CurrencyCode = purchaseMethodResponse1.CurrencyCode;
    if (purchaseMethodResponse1.HasWalletName)
      purchaseMethodResponse2.WalletName = purchaseMethodResponse1.WalletName;
    if (purchaseMethodResponse1.HasUseEbalance)
      purchaseMethodResponse2.UseEBalance = purchaseMethodResponse1.UseEbalance;
    purchaseMethodResponse2.IsZeroCostLicense = purchaseMethodResponse1.HasIsZeroCostLicense && purchaseMethodResponse1.IsZeroCostLicense;
    if (purchaseMethodResponse1.HasChallengeId)
      purchaseMethodResponse2.ChallengeID = purchaseMethodResponse1.ChallengeId;
    if (purchaseMethodResponse1.HasChallengeUrl)
      purchaseMethodResponse2.ChallengeURL = purchaseMethodResponse1.ChallengeUrl;
    if (purchaseMethodResponse1.HasError)
      purchaseMethodResponse2.PurchaseError = this.ConvertPurchaseError(purchaseMethodResponse1.Error);
    return purchaseMethodResponse2;
  }

  public Network.PurchaseResponse GetPurchaseResponse()
  {
    PegasusUtil.PurchaseResponse purchaseResponse = this.m_connectApi.GetPurchaseResponse();
    if (purchaseResponse == null)
      return (Network.PurchaseResponse) null;
    return new Network.PurchaseResponse()
    {
      PurchaseError = this.ConvertPurchaseError(purchaseResponse.Error),
      TransactionID = purchaseResponse.HasTransactionId ? purchaseResponse.TransactionId : 0L,
      PMTProductID = purchaseResponse.HasPmtProductId ? new long?(purchaseResponse.PmtProductId) : new long?(),
      ThirdPartyID = purchaseResponse.HasThirdPartyId ? purchaseResponse.ThirdPartyId : string.Empty,
      CurrencyCode = purchaseResponse.CurrencyCode
    };
  }

  public Network.PurchaseViaGoldResponse GetPurchaseWithGoldResponse()
  {
    PurchaseWithGoldResponse withGoldResponse1 = this.m_connectApi.GetPurchaseWithGoldResponse();
    if (withGoldResponse1 == null)
      return (Network.PurchaseViaGoldResponse) null;
    Network.PurchaseViaGoldResponse withGoldResponse2 = new Network.PurchaseViaGoldResponse()
    {
      Error = (Network.PurchaseViaGoldResponse.ErrorType) withGoldResponse1.Result
    };
    if (withGoldResponse1.HasGoldUsed)
      withGoldResponse2.GoldUsed = withGoldResponse1.GoldUsed;
    return withGoldResponse2;
  }

  public Network.CardBackResponse GetCardBackResponse()
  {
    SetFavoriteCardBackResponse cardBackResponse = this.m_connectApi.GetSetFavoriteCardBackResponse();
    if (cardBackResponse == null)
      return (Network.CardBackResponse) null;
    return new Network.CardBackResponse()
    {
      Success = cardBackResponse.Success,
      CardBack = cardBackResponse.CardBack,
      IsFavorite = cardBackResponse.IsFavorite
    };
  }

  public void SetFavoriteCardBack(int cardBack, bool isFavorite = true)
  {
    NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
    if (netObject != null)
      this.m_connectApi.SetFavoriteCardBack(cardBack, isFavorite);
    if (Network.IsLoggedIn())
      return;
    OfflineDataCache.SetFavoriteCardBack(cardBack, isFavorite);
    if (netObject == null)
      return;
    NetCache.Get().ProcessNewFavoriteCardBack(cardBack);
  }

  public NetCache.NetCacheCardBacks GetCardBacks()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return new NetCache.NetCacheCardBacks();
    CardBacks cardBacksPacket = this.GetCardBacksPacket();
    if (cardBacksPacket == null)
      return (NetCache.NetCacheCardBacks) null;
    NetCache.NetCacheCardBacks cardBacks = new NetCache.NetCacheCardBacks();
    for (int index = 0; index < cardBacksPacket.CardBacks_.Count; ++index)
    {
      int cardBack = cardBacksPacket.CardBacks_[index];
      cardBacks.CardBacks.Add(cardBack);
    }
    for (int index = 0; index < cardBacksPacket.FavoriteCardBacks.Count; ++index)
    {
      int favoriteCardBack = cardBacksPacket.FavoriteCardBacks[index];
      cardBacks.FavoriteCardBacks.Add(favoriteCardBack);
    }
    return cardBacks;
  }

  public CardBacks GetCardBacksPacket() => !Network.ShouldBeConnectedToAurora() ? (CardBacks) null : this.m_connectApi.GetCardBacks();

  public Network.CoinResponse GetCoinResponse()
  {
    SetFavoriteCoinResponse favoriteCoinResponse = this.m_connectApi.GetSetFavoriteCoinResponse();
    if (favoriteCoinResponse == null)
      return (Network.CoinResponse) null;
    return new Network.CoinResponse()
    {
      Success = favoriteCoinResponse.Success,
      Coin = favoriteCoinResponse.CoinId
    };
  }

  public void SetFavoriteCoin(ref OfflineDataCache.OfflineData data, int coin)
  {
    NetCache.NetCacheCoins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCoins>();
    if (netObject != null && coin != netObject.FavoriteCoin)
      this.m_connectApi.SetFavoriteCoin(coin);
    if (Network.IsLoggedIn())
      return;
    OfflineDataCache.SetFavoriteCoin(ref data, coin);
    if (netObject == null)
      return;
    NetCache.Get().ProcessNewFavoriteCoin(coin);
  }

  public NetCache.NetCacheCoins GetCoins()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return new NetCache.NetCacheCoins();
    Coins coinsPacket = this.GetCoinsPacket();
    if (coinsPacket == null)
      return (NetCache.NetCacheCoins) null;
    NetCache.NetCacheCoins coins = new NetCache.NetCacheCoins();
    coins.FavoriteCoin = coinsPacket.FavoriteCoin;
    for (int index = 0; index < coinsPacket.Coins_.Count; ++index)
    {
      int coin = coinsPacket.Coins_[index];
      coins.Coins.Add(coin);
    }
    return coins;
  }

  public Coins GetCoinsPacket() => !Network.ShouldBeConnectedToAurora() ? (Coins) null : this.m_connectApi.GetCoins();

  public CoinUpdate GetCoinUpdate() => this.m_connectApi.GetCoinUpdate();

  public CardValues GetCardValues() => !Network.ShouldBeConnectedToAurora() ? (CardValues) null : this.m_connectApi.GetCardValues();

  public void SetDebugRatingInfo(int ratingId)
  {
    if (!Network.ShouldBeConnectedToAurora())
      return;
    this.m_connectApi.DebugRatingInfoRequest(new DebugRatingInfoRequest()
    {
      RatingId = ratingId
    });
  }

  public DebugRatingInfoResponse GetDebugRatingInfoResponse() => this.m_connectApi.GetDebugRatingInfoResponse();

  public InitialClientState GetInitialClientState()
  {
    if (Network.ShouldBeConnectedToAurora())
      return this.m_connectApi.GetInitialClientState();
    InitialClientState initialClientState = new InitialClientState()
    {
      HasClientOptions = true,
      ClientOptions = new ClientOptions(),
      HasCollection = true,
      Collection = new Collection(),
      HasAchievements = true,
      Achievements = new Achieves(),
      HasNotices = true,
      Notices = new ProfileNotices(),
      HasGameCurrencyStates = true,
      GameCurrencyStates = new GameCurrencyStates()
    };
    initialClientState.GameCurrencyStates.HasCurrencyVersion = true;
    initialClientState.GameCurrencyStates.CurrencyVersion = 0L;
    initialClientState.GameCurrencyStates.HasArcaneDustBalance = true;
    initialClientState.GameCurrencyStates.HasCappedGoldBalance = true;
    initialClientState.GameCurrencyStates.HasBonusGoldBalance = true;
    initialClientState.GameCurrencyStates.HasRenownBalance = true;
    initialClientState.HasBoosters = true;
    initialClientState.Boosters = new Boosters();
    if (initialClientState.Decks == null)
      initialClientState.Decks = new List<DeckInfo>();
    return initialClientState;
  }

  public void OpenBooster(int id)
  {
    Log.Net.Print("Network.OpenBooster");
    long fsgId = FiresideGatheringManager.Get().IsCheckedIn ? FiresideGatheringManager.Get().CurrentFsgId : 0L;
    this.m_connectApi.OpenBooster(id, fsgId);
  }

  public void CreateDeck(
    DeckType deckType,
    string name,
    int heroDatabaseAssetID,
    PegasusShared.FormatType formatType,
    long sortOrder,
    DeckSourceType sourceType,
    out int? requestId,
    string pastedDeckHash = null,
    int brawlLibraryItemId = 0)
  {
    if (!Network.IsLoggedIn())
    {
      requestId = new int?();
    }
    else
    {
      requestId = new int?(this.GetNextCreateDeckRequestId());
      long? fsgId = FiresideGatheringManager.Get().IsCheckedIn ? new long?(FiresideGatheringManager.Get().CurrentFsgId) : new long?();
      Log.Net.Print(string.Format("Network.CreateDeck hero={0}", (object) heroDatabaseAssetID));
      this.m_connectApi.CreateDeck(deckType, name, heroDatabaseAssetID, formatType, sortOrder, sourceType, pastedDeckHash, fsgId, FiresideGatheringManager.Get().CurrentFsgSharedSecretKey, brawlLibraryItemId, requestId);
    }
  }

  private int GetNextCreateDeckRequestId() => ++this.m_state.CurrentCreateDeckRequestId;

  public void RenameDeck(long deck, string name)
  {
    if (Network.IsLoggedIn())
    {
      Log.Net.Print(string.Format("Network.RenameDeck {0}", (object) deck));
      CollectionManager.Get().AddPendingDeckRename(deck, name);
      this.m_connectApi.RenameDeck(deck, name);
    }
    else
      OfflineDataCache.RenameDeck(deck, name);
  }

  public void SendDeckData(
    CollectionDeck.ChangeSource changeSource,
    int changeNumber,
    long deck,
    List<Network.CardUserData> cards,
    int newHeroAssetID,
    bool? newHeroOverridenStatus,
    int uiHeroOverrideAssetID,
    TAG_PREMIUM uiHeroOverridePremium,
    int? newCardBackID,
    PegasusShared.FormatType formatType,
    long sortOrder,
    bool? randomHeroUseFavorite,
    RuneType[] runeOrder,
    string pastedDeckHash = null)
  {
    DeckSetData packet = new DeckSetData()
    {
      ChangeSource = (int) changeSource,
      ChangeNumber = changeNumber,
      Deck = deck,
      FormatType = formatType,
      TaggedStandard = formatType == PegasusShared.FormatType.FT_STANDARD,
      SortOrder = sortOrder
    };
    for (int index = 0; index < cards.Count; ++index)
    {
      Network.CardUserData card = cards[index];
      DeckCardData deckCardData = new DeckCardData();
      PegasusShared.CardDef cardDef = new PegasusShared.CardDef();
      cardDef.Asset = card.DbId;
      if (card.Premium != TAG_PREMIUM.NORMAL)
        cardDef.Premium = (int) card.Premium;
      deckCardData.Def = cardDef;
      deckCardData.Qty = card.Count;
      packet.Cards.Add(deckCardData);
    }
    if (newHeroOverridenStatus.HasValue)
    {
      packet.HasHeroOverridden = true;
      packet.HeroOverridden = newHeroOverridenStatus.Value;
    }
    if (-1 != newHeroAssetID)
      packet.Hero = newHeroAssetID;
    if (-1 != uiHeroOverrideAssetID)
      packet.UiHeroOverride = new PegasusShared.CardDef()
      {
        Asset = uiHeroOverrideAssetID,
        Premium = (int) uiHeroOverridePremium
      };
    int? nullable1 = newCardBackID;
    if (!(-1 == nullable1.GetValueOrDefault() & nullable1.HasValue))
    {
      if (!newCardBackID.HasValue)
      {
        packet.HasCardBack = false;
        packet.RemovingCardBack = true;
      }
      else
      {
        packet.HasCardBack = true;
        packet.CardBack = newCardBackID.Value;
      }
    }
    if (randomHeroUseFavorite.HasValue)
    {
      packet.HasRandomHeroUseFavorite = true;
      packet.RandomHeroUseFavorite = randomHeroUseFavorite.Value;
    }
    if (runeOrder != null && runeOrder.Length == DeckRule_DeathKnightRuneLimit.MaxRuneSlots)
    {
      packet.HasRune1 = true;
      packet.Rune1 = runeOrder[0];
      packet.HasRune2 = true;
      packet.Rune2 = runeOrder[1];
      packet.HasRune3 = true;
      packet.Rune3 = runeOrder[2];
    }
    if (!string.IsNullOrEmpty(pastedDeckHash))
      packet.PastedDeckHash = pastedDeckHash;
    long? nullable2 = FiresideGatheringManager.Get().IsCheckedIn ? new long?(FiresideGatheringManager.Get().CurrentFsgId) : new long?();
    if (nullable2.HasValue)
      packet.FsgId = nullable2.Value;
    packet.FsgSharedSecretKey = FiresideGatheringManager.Get().CurrentFsgSharedSecretKey;
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.SendDeckData(packet);
      OfflineDataCache.ApplyDeckSetDataToOriginalDeck(packet);
      CollectionManager.Get().AddPendingDeckEdit(deck);
    }
    OfflineDataCache.ApplyDeckSetDataLocally(packet);
  }

  public void DeleteDeck(long deck, DeckType deckType)
  {
    OfflineDataCache.DeleteDeck(deck);
    if (!Network.IsLoggedIn())
      return;
    Log.Net.Print(string.Format("Network.DeleteDeck {0}", (object) deck));
    if (deck <= 0L)
      Log.Offline.PrintError("Network.DeleteDeck Error: Attempting to delete fake deck ID={0} on server.", (object) deck);
    else
      this.m_connectApi.DeleteDeck(deck, deckType);
  }

  public void RequestDeckContents(params long[] deckIds)
  {
    if (!Network.IsLoggedIn())
      return;
    Log.Net.Print("Network.GetDeckContents {0}", (object) string.Join(", ", ((IEnumerable<long>) deckIds).Select<long, string>((Func<long, string>) (id => id.ToString())).ToArray<string>()));
    this.m_connectApi.RequestDeckContents(deckIds);
  }

  public void SetDeckTemplateSource(long deck, int templateID)
  {
    if (!Network.IsLoggedIn() || deck < 0L)
      return;
    Log.Net.Print(string.Format("Network.SendDeckTemplateSource {0}, {1}", (object) deck, (object) templateID));
    this.m_connectApi.SendDeckTemplateSource(deck, templateID);
  }

  public PegasusUtil.GetDeckContentsResponse GetDeckContentsResponse()
  {
    PegasusUtil.GetDeckContentsResponse contentsResponse;
    if (Network.IsLoggedIn())
    {
      contentsResponse = this.m_connectApi.GetDeckContentsResponse();
    }
    else
    {
      contentsResponse = new PegasusUtil.GetDeckContentsResponse()
      {
        Decks = new List<PegasusUtil.DeckContents>()
      };
      contentsResponse.Decks = OfflineDataCache.GetLocalDeckContentsFromCache();
    }
    return contentsResponse;
  }

  public FreeDeckChoiceResponse GetFreeDeckChoiceResponse()
  {
    FreeDeckChoiceResponse deckChoiceResponse;
    if (Network.IsLoggedIn())
      deckChoiceResponse = this.m_connectApi.GetFreeDeckChoiceResponse();
    else
      deckChoiceResponse = new FreeDeckChoiceResponse()
      {
        Success = false
      };
    return deckChoiceResponse;
  }

  public FreeDeckStateUpdate GetFreeDeckStateUpdate() => this.m_connectApi.GetFreeDeckStateUpdate();

  public static SmartDeckRequest GenerateSmartDeckRequestMessage(CollectionDeck deck)
  {
    List<SmartDeckCardData> smartDeckCardDataList = new List<SmartDeckCardData>();
    Dictionary<long, SmartDeckCardData> dictionary = new Dictionary<long, SmartDeckCardData>();
    foreach (CollectionDeckSlot slot in deck.GetSlots())
    {
      if (slot.Owned)
      {
        int dbId = GameUtils.TranslateCardIdToDbId(slot.CardID);
        if (!dictionary.ContainsKey((long) dbId))
          dictionary.Add((long) dbId, new SmartDeckCardData()
          {
            Asset = dbId
          });
        dictionary[(long) dbId].QtyGolden += slot.GetCount(TAG_PREMIUM.GOLDEN);
        dictionary[(long) dbId].QtyNormal += slot.GetCount(TAG_PREMIUM.NORMAL);
      }
    }
    foreach (long key in dictionary.Keys)
      smartDeckCardDataList.Add(dictionary[key]);
    HSCachedDeckCompletionRequest completionRequest = new HSCachedDeckCompletionRequest()
    {
      HeroClass = (int) deck.GetClass(),
      InsertedCard = smartDeckCardDataList,
      DeckId = deck.ID,
      FormatType = deck.FormatType
    };
    return new SmartDeckRequest()
    {
      RequestMessage = completionRequest
    };
  }

  public void RequestSmartDeckCompletion(CollectionDeck deck) => this.m_connectApi.SendSmartDeckRequest(Network.GenerateSmartDeckRequestMessage(deck));

  public void RequestBaconRatingInfo() => this.m_connectApi.RequestBaconRatingInfo();

  public void SendPVPDRSessionStartRequest(bool paidEntry) => this.m_connectApi.SendPVPDRSessionStartRequest(paidEntry);

  public PVPDRSessionStartResponse GetPVPDRSessionStartResponse() => this.m_connectApi.GetPVPDRSessionStartResponse();

  public void SendPVPDRSessionEndRequest() => this.m_connectApi.SendPVPDRSessionEndRequest();

  public PVPDRSessionEndResponse GetPVPDRSessionEndResponse() => this.m_connectApi.GetPVPDRSessionEndResponse();

  public void SendPVPDRSessionInfoRequest() => this.m_connectApi.SendPVPDRSessionInfoRequest();

  public PVPDRSessionInfoResponse GetPVPDRSessionInfoResponse() => this.m_connectApi.GetPVPDRSessionInfoResponse();

  public void SendPVPDRRetireRequest() => this.m_connectApi.SendPVPDRRetireRequest();

  public PVPDRRetireResponse GetPVPDRRetireResponse() => this.m_connectApi.GetPVPDRRetireResponse();

  public void RequestPVPDRStatsInfo() => this.m_connectApi.RequestPVPDRStatsInfo();

  public void RequestLettuceMap(
    uint bountyId = 0,
    List<LettuceMapPlayerData> playerDataList = null,
    BnetGameAccountId coopLeaderGameAccountId = null)
  {
    LettuceMapRequest request = new LettuceMapRequest()
    {
      LettuceBountyRecordId = bountyId
    };
    if ((BnetEntityId) coopLeaderGameAccountId != (BnetEntityId) null)
      request.CoopMapOwnerId = BnetUtils.CreatePegasusBnetId((BnetEntityId) coopLeaderGameAccountId);
    if (playerDataList != null)
      request.PlayerData = playerDataList;
    this.m_connectApi.RequestLettuceMap(request);
  }

  public void ChooseLettuceMapNode(uint nodeId) => this.m_connectApi.ChooseLettuceMapNode(new LettuceMapChooseNodeRequest()
  {
    NodeId = nodeId
  });

  public void RetireLettuceMap() => this.m_connectApi.RetireLettuceMap(new LettuceMapRetireRequest());

  public void MakeMercenariesMapTreasureSelection(int optionIndex) => this.m_connectApi.MakeMercenariesMapTreasureSelection(new MercenariesMapTreasureSelectionRequest()
  {
    SelectedOptionIndex = optionIndex
  });

  public void MakeMercenariesMapVisitorSelection(int optionIndex) => this.m_connectApi.MakeMercenariesMapVisitorSelection(new MercenariesMapVisitorSelectionRequest()
  {
    SelectedOptionIndex = optionIndex
  });

  public void RequestLuckyDrawBoxState(int luckyDrawBoxId) => this.m_connectApi.RequestLuckyDrawBoxState(new LuckyDrawBoxStateRequest()
  {
    LuckyDrawBoxId = luckyDrawBoxId
  });

  public LuckyDrawBoxStateResponse GetLuckyDrawBoxStateResponse() => this.m_connectApi.GetLuckyDrawBoxStateResponse();

  public void UseLuckyDrawHammer() => this.m_connectApi.UseLuckyDrawHammer(new LuckyDrawUseHammerRequest());

  public LuckyDrawUseHammerResponse GetUseLuckyDrawHammerResponse() => this.m_connectApi.GetUseLuckyDrawHammerResponse();

  public void AcknowledgeLuckyDrawHammers() => this.m_connectApi.AcknowledgeLuckyDrawHammers(new LuckyDrawAcknowledgeAllHammersRequest());

  public void AcknowledgeLuckyDrawRewards() => this.m_connectApi.AcknowledgeLuckyDrawRewards(new LuckyDrawAcknowledgeAllRewardsRequest());

  public List<NetCache.BoosterCard> OpenedBooster()
  {
    BoosterContent openedBooster = this.m_connectApi.GetOpenedBooster();
    if (openedBooster == null)
      return (List<NetCache.BoosterCard>) null;
    List<NetCache.BoosterCard> boosterCardList = new List<NetCache.BoosterCard>();
    for (int index = 0; index < openedBooster.List.Count; ++index)
    {
      PegasusUtil.BoosterCard boosterCard = openedBooster.List[index];
      boosterCardList.Add(new NetCache.BoosterCard()
      {
        Def = {
          Name = GameUtils.TranslateDbIdToCardId(boosterCard.CardDef.Asset),
          Premium = (TAG_PREMIUM) boosterCard.CardDef.Premium
        },
        Date = TimeUtils.PegDateToFileTimeUtc(boosterCard.InsertDate)
      });
    }
    if (openedBooster.HasCollectionVersion)
      NetCache.Get().AddExpectedCollectionModification(openedBooster.CollectionVersion);
    return boosterCardList;
  }

  public Network.DBAction GetDeckResponse() => this.GetDbAction();

  public Network.DBAction GetDbAction()
  {
    PegasusUtil.DBAction dbAction = this.m_connectApi.GetDbAction();
    if (dbAction == null)
      return (Network.DBAction) null;
    return new Network.DBAction()
    {
      Action = (Network.DBAction.ActionType) dbAction.Action,
      Result = (Network.DBAction.ResultType) dbAction.Result,
      MetaData = dbAction.MetaData
    };
  }

  public void ReconcileDeckContentsForChangedOfflineDecks(
    ref OfflineDataCache.OfflineData data,
    List<DeckInfo> remoteDecks,
    List<PegasusUtil.DeckContents> remoteContents,
    List<long> validDeckIds)
  {
    List<long> longList1 = new List<long>();
    foreach (DeckInfo remoteDeck in remoteDecks)
    {
      bool flag = false;
      long id = remoteDeck.Id;
      foreach (long validDeckId in validDeckIds)
      {
        if (validDeckId == id)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        DeckInfo infoFromDeckList1 = OfflineDataCache.GetDeckInfoFromDeckList(id, data.OriginalDeckList);
        DeckInfo infoFromDeckList2 = OfflineDataCache.GetDeckInfoFromDeckList(id, data.LocalDeckList);
        if (infoFromDeckList2 == null && infoFromDeckList1 != null)
        {
          NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
          if (netObject != null && netObject.AllowOfflineClientDeckDeletion)
            Network.Get().DeleteDeck(id, remoteDeck.DeckType);
        }
        else if (infoFromDeckList2 == null && infoFromDeckList1 == null)
          longList1.Add(id);
        else if (infoFromDeckList2 != null && infoFromDeckList1 != null && remoteDeck.LastModified != infoFromDeckList2.LastModified)
        {
          if (remoteDeck.LastModified != infoFromDeckList1.LastModified)
            longList1.Add(id);
          else
            longList1.Add(id);
        }
      }
    }
    if (data.LocalDeckList != null)
    {
      foreach (DeckInfo localDeck in data.LocalDeckList)
      {
        long id = localDeck.Id;
        bool flag1 = false;
        foreach (DeckInfo remoteDeck in remoteDecks)
        {
          if (remoteDeck.Id == id)
          {
            flag1 = true;
            break;
          }
        }
        if (!flag1)
        {
          bool flag2 = false;
          foreach (DeckInfo originalDeck in data.OriginalDeckList)
          {
            if (originalDeck.Id == id)
            {
              flag2 = true;
              break;
            }
          }
          if (flag2)
            CollectionManager.Get().OnDeckDeletedWhileOffline(id);
        }
      }
    }
    if (longList1.Count > 0)
    {
      List<long> longList2 = new List<long>();
      foreach (long num in longList1)
      {
        this.m_state.DeckIdsWaitingToDiffAgainstOfflineCache.Add(num);
        DeckInfo deckInfo = (DeckInfo) null;
        foreach (DeckInfo remoteDeck in remoteDecks)
        {
          if (remoteDeck.Id == num)
          {
            deckInfo = remoteDeck;
            break;
          }
        }
        bool flag3 = deckInfo != null && deckInfo.DeckType == DeckType.PRECON_DECK;
        bool flag4 = false;
        if (remoteContents != null)
        {
          foreach (PegasusUtil.DeckContents remoteContent in remoteContents)
          {
            if (remoteContent.DeckId == num)
            {
              flag4 = true;
              break;
            }
          }
        }
        if (!flag3 && !flag4)
          longList2.Add(num);
      }
      if (longList2.Count > 0)
        this.RequestDeckContents(longList2.ToArray());
    }
    this.RegisterNetHandler((object) DeckCreated.PacketID.ID, new Network.NetHandler(this.OnDeckCreatedResponse_SendOfflineDeckSetData));
    this.CreateDeckFromOfflineDeckCache(ref data);
    if (remoteContents == null)
      return;
    this.UpdateDecksFromContent(ref data, remoteContents);
  }

  public NetCache.NetCacheDecks GetDeckHeaders()
  {
    NetCache.NetCacheDecks deckHeaders1 = new NetCache.NetCacheDecks();
    if (!Network.ShouldBeConnectedToAurora())
      return deckHeaders1;
    DeckList deckHeaders2 = this.m_connectApi.GetDeckHeaders();
    return deckHeaders2 == null ? (NetCache.NetCacheDecks) null : Network.GetDeckHeaders(deckHeaders2.Decks);
  }

  public static NetCache.NetCacheDecks GetDeckHeaders(List<DeckInfo> deckHeaders)
  {
    NetCache.NetCacheDecks deckHeaders1 = new NetCache.NetCacheDecks();
    if (deckHeaders == null)
      return deckHeaders1;
    for (int index = 0; index < deckHeaders.Count; ++index)
      deckHeaders1.Decks.Add(Network.GetDeckHeaderFromDeckInfo(deckHeaders[index]));
    return deckHeaders1;
  }

  private void OnDeckContentsResponse()
  {
    PegasusUtil.GetDeckContentsResponse contentsResponse = this.GetDeckContentsResponse();
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    this.UpdateDecksFromContent(ref data, contentsResponse.Decks);
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  private void UpdateDecksFromContent(
    ref OfflineDataCache.OfflineData data,
    List<PegasusUtil.DeckContents> decksContents)
  {
    List<DeckSetData> deckSetDataToSend = new List<DeckSetData>();
    List<PegasusUtil.RenameDeck> deckRenameToSend = new List<PegasusUtil.RenameDeck>();
    List<DeckInfo> listFromNetCache = NetCache.Get().GetDeckListFromNetCache();
    foreach (PegasusUtil.DeckContents decksContent in decksContents)
    {
      if (this.m_state.DeckIdsWaitingToDiffAgainstOfflineCache.Contains(decksContent.DeckId))
      {
        this.m_state.DeckIdsWaitingToDiffAgainstOfflineCache.Remove(decksContent.DeckId);
        this.DiffRemoteDeckContentsAgainstOfflineDataCache(ref data, decksContent, listFromNetCache, ref deckSetDataToSend, ref deckRenameToSend);
      }
      else
        OfflineDataCache.CacheLocalAndOriginalDeckContents(ref data, decksContent, decksContent);
    }
    List<long> decksToRequest = new List<long>();
    foreach (DeckSetData packet in deckSetDataToSend)
    {
      this.m_connectApi.SendDeckData(packet);
      decksToRequest.Add(packet.Deck);
    }
    CollectionManager.Get().RegisterDecksToRequestContentsAfterDeckSetDataResponse(decksToRequest);
    foreach (PegasusUtil.RenameDeck renameDeck in deckRenameToSend)
      this.m_connectApi.RenameDeck(renameDeck.Deck, renameDeck.Name);
    OfflineDataCache.CacheLocalAndOriginalDeckList(ref data, listFromNetCache, listFromNetCache);
  }

  private void DiffRemoteDeckContentsAgainstOfflineDataCache(
    ref OfflineDataCache.OfflineData data,
    PegasusUtil.DeckContents remoteDeckContents,
    List<DeckInfo> currentNetCacheDeckList,
    ref List<DeckSetData> deckSetDataToSend,
    ref List<PegasusUtil.RenameDeck> deckRenameToSend)
  {
    DeckInfo infoFromDeckList = OfflineDataCache.GetDeckInfoFromDeckList(remoteDeckContents.DeckId, data.LocalDeckList);
    PegasusUtil.DeckContents deckContentsList = OfflineDataCache.GetDeckContentsFromDeckContentsList(remoteDeckContents.DeckId, data.LocalDeckContents);
    DeckInfo originalDeckInfo = (DeckInfo) null;
    foreach (DeckInfo currentNetCacheDeck in currentNetCacheDeckList)
    {
      if (currentNetCacheDeck.Id == remoteDeckContents.DeckId)
      {
        originalDeckInfo = currentNetCacheDeck;
        break;
      }
    }
    if (originalDeckInfo == null)
      return;
    if (infoFromDeckList != null && originalDeckInfo.LastModified < infoFromDeckList.LastModified)
    {
      DeckSetData deckSetData;
      if (OfflineDataCache.GenerateDeckSetDataFromDiff(remoteDeckContents.DeckId, infoFromDeckList, originalDeckInfo, deckContentsList, remoteDeckContents, out deckSetData))
        deckSetDataToSend.Add(deckSetData);
      PegasusUtil.RenameDeck renameDeckFromDiff = OfflineDataCache.GenerateRenameDeckFromDiff(remoteDeckContents.DeckId, infoFromDeckList, originalDeckInfo);
      if (renameDeckFromDiff == null || renameDeckFromDiff.Name == null)
        return;
      deckRenameToSend.Add(renameDeckFromDiff);
    }
    else
      OfflineDataCache.CacheLocalAndOriginalDeckContents(ref data, remoteDeckContents, remoteDeckContents);
  }

  private void CreateDeckFromOfflineDeckCache(ref OfflineDataCache.OfflineData data)
  {
    int num = 0;
    List<long> fakeDeckIds = OfflineDataCache.GetFakeDeckIds(data);
    if (fakeDeckIds.Contains(this.FakeIdWaitingForResponse))
      num = fakeDeckIds.IndexOf(Network.Get().FakeIdWaitingForResponse) + 1;
    DeckInfo deckInfo = (DeckInfo) null;
    for (int index = num; index < fakeDeckIds.Count && deckInfo == null; ++index)
    {
      this.FakeIdWaitingForResponse = fakeDeckIds[index];
      deckInfo = OfflineDataCache.GetDeckInfoFromDeckList(this.FakeIdWaitingForResponse, data.LocalDeckList);
    }
    if (deckInfo == null)
    {
      this.RemoveNetHandler((object) DeckCreated.PacketID.ID, new Network.NetHandler(this.OnDeckCreatedResponse_SendOfflineDeckSetData));
      this.OnFinishedCreatingDecksFromOfflineDataCache(ref data);
    }
    else
    {
      int? requestId;
      this.CreateDeck(deckInfo.DeckType, deckInfo.Name, deckInfo.Hero, deckInfo.FormatType, deckInfo.SortOrder, deckInfo.SourceType, out requestId, deckInfo.PastedDeckHash);
      if (!requestId.HasValue)
        return;
      this.m_state.InTransitOfflineCreateDeckRequestIds.Add(requestId.Value);
    }
  }

  private void OnFinishedCreatingDecksFromOfflineDataCache(ref OfflineDataCache.OfflineData data)
  {
    OfflineDataCache.ClearFakeDeckIds(ref data);
    OfflineDataCache.RemoveAllOldDecksContents(ref data);
    this.FakeIdWaitingForResponse = 0L;
  }

  private void OnDeckCreatedResponse_SendOfflineDeckSetData()
  {
    int? requestId;
    NetCache.DeckHeader createdDeck = this.GetCreatedDeck(out requestId);
    if (createdDeck == null || !requestId.HasValue || !this.m_state.InTransitOfflineCreateDeckRequestIds.Contains(requestId.Value))
      return;
    this.m_state.InTransitOfflineCreateDeckRequestIds.Remove(requestId.Value);
    long waitingForResponse = Network.Get().FakeIdWaitingForResponse;
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    DeckSetData deckSetData;
    if (OfflineDataCache.GenerateDeckSetDataFromDiff(waitingForResponse, data.LocalDeckList, data.OriginalDeckList, data.LocalDeckContents, data.OriginalDeckContents, out deckSetData))
    {
      deckSetData.Deck = createdDeck.ID;
      CollectionManager.Get().RegisterDecksToRequestContentsAfterDeckSetDataResponse(new List<long>()
      {
        createdDeck.ID
      });
      this.m_connectApi.SendDeckData(deckSetData);
    }
    if (!OfflineDataCache.UpdateDeckWithNewId(waitingForResponse, createdDeck.ID))
    {
      Log.Offline.PrintDebug("OnDeckCreatedResponse_SendOfflineDeckSetData() - Deleting deck id={0} because it's fake id={1}  was not found in the offline cache.", (object) createdDeck.ID, (object) waitingForResponse);
      this.DeleteDeck(createdDeck.ID, createdDeck.Type);
    }
    else
    {
      CollectionManager.Get().UpdateDeckWithNewId(waitingForResponse, createdDeck.ID);
      this.CreateDeckFromOfflineDeckCache(ref data);
      OfflineDataCache.WriteOfflineDataToFile(data);
    }
  }

  public static bool DeckNeedsName(ulong deckValidityFlags) => (deckValidityFlags & 512UL) > 0UL;

  public static bool AreDeckFlagsLocked(ulong deckValidityFlags) => (deckValidityFlags & 1024UL) > 0UL;

  public NetCache.DeckHeader GetCreatedDeck(out int? requestId)
  {
    DeckCreated deckCreated = this.m_connectApi.DeckCreated();
    if (deckCreated == null)
    {
      requestId = new int?();
      return (NetCache.DeckHeader) null;
    }
    NetCache.DeckHeader headerFromDeckInfo = Network.GetDeckHeaderFromDeckInfo(deckCreated.Info);
    requestId = new int?(deckCreated.RequestId);
    return headerFromDeckInfo;
  }

  public static NetCache.DeckHeader GetDeckHeaderFromDeckInfo(DeckInfo deck)
  {
    NetCache.DeckHeader headerFromDeckInfo = new NetCache.DeckHeader()
    {
      ID = deck.Id,
      Name = deck.Name,
      Hero = GameUtils.TranslateDbIdToCardId(deck.Hero),
      HeroPower = GameUtils.GetHeroPowerCardIdFromHero(deck.Hero),
      RandomHeroUseFavorite = deck.RandomHeroUseFavorite,
      Type = deck.DeckType,
      HeroOverridden = deck.HeroOverride,
      SeasonId = deck.SeasonId,
      BrawlLibraryItemId = deck.BrawlLibraryItemId,
      NeedsName = Network.DeckNeedsName(deck.Validity),
      SortOrder = deck.HasSortOrder ? deck.SortOrder : deck.Id,
      FormatType = deck.FormatType,
      Rune1 = deck.Rune1,
      Rune2 = deck.Rune2,
      Rune3 = deck.Rune3,
      Locked = Network.AreDeckFlagsLocked(deck.Validity),
      SourceType = deck.HasSourceType ? deck.SourceType : DeckSourceType.DECK_SOURCE_TYPE_UNKNOWN,
      UIHeroOverride = !deck.HasUiHeroOverride || deck.UiHeroOverride == 0 ? string.Empty : GameUtils.TranslateDbIdToCardId(deck.UiHeroOverride),
      UIHeroOverridePremium = deck.HasUiHeroOverridePremium ? (TAG_PREMIUM) deck.UiHeroOverridePremium : TAG_PREMIUM.NORMAL
    };
    if (deck.HasCardBack)
      headerFromDeckInfo.CardBack = new int?(deck.CardBack);
    headerFromDeckInfo.CreateDate = !deck.HasCreateDate ? new DateTime?() : new DateTime?(TimeUtils.UnixTimeStampToDateTimeUtc(deck.CreateDate));
    headerFromDeckInfo.LastModified = !deck.HasLastModified ? new DateTime?() : new DateTime?(TimeUtils.UnixTimeStampToDateTimeUtc(deck.LastModified));
    return headerFromDeckInfo;
  }

  public static DeckInfo GetDeckInfoFromDeckHeader(NetCache.DeckHeader deckHeader)
  {
    if (deckHeader == null)
      return (DeckInfo) null;
    DeckInfo infoFromDeckHeader = new DeckInfo()
    {
      Id = deckHeader.ID,
      Name = deckHeader.Name,
      Hero = GameUtils.TranslateCardIdToDbId(deckHeader.Hero),
      DeckType = deckHeader.Type,
      HeroOverride = deckHeader.HeroOverridden,
      BrawlLibraryItemId = deckHeader.BrawlLibraryItemId,
      SortOrder = deckHeader.SortOrder,
      SourceType = deckHeader.SourceType
    };
    int? cardBack = deckHeader.CardBack;
    if (cardBack.HasValue)
    {
      DeckInfo deckInfo = infoFromDeckHeader;
      cardBack = deckHeader.CardBack;
      int num = cardBack.Value;
      deckInfo.CardBack = num;
    }
    if (deckHeader.SeasonId != 0)
      infoFromDeckHeader.SeasonId = deckHeader.SeasonId;
    if (!string.IsNullOrEmpty(deckHeader.UIHeroOverride))
    {
      infoFromDeckHeader.UiHeroOverride = GameUtils.TranslateCardIdToDbId(deckHeader.UIHeroOverride);
      infoFromDeckHeader.UiHeroOverridePremium = (int) deckHeader.UIHeroOverridePremium;
    }
    DateTime? nullable = deckHeader.CreateDate;
    if (nullable.HasValue)
    {
      DeckInfo deckInfo = infoFromDeckHeader;
      nullable = deckHeader.CreateDate;
      long unixTimeStamp = (long) TimeUtils.DateTimeToUnixTimeStamp(nullable.Value);
      deckInfo.CreateDate = unixTimeStamp;
    }
    nullable = deckHeader.LastModified;
    if (nullable.HasValue)
    {
      DeckInfo deckInfo = infoFromDeckHeader;
      nullable = deckHeader.LastModified;
      long unixTimeStamp = (long) TimeUtils.DateTimeToUnixTimeStamp(nullable.Value);
      deckInfo.LastModified = unixTimeStamp;
    }
    return infoFromDeckHeader;
  }

  public long GetDeletedDeckID()
  {
    DeckDeleted deckDeleted = this.m_connectApi.DeckDeleted();
    return deckDeleted == null ? 0L : deckDeleted.Deck;
  }

  public Network.DeckName GetRenamedDeck()
  {
    DeckRenamed deckRenamed = this.m_connectApi.DeckRenamed();
    if (deckRenamed == null)
      return (Network.DeckName) null;
    return new Network.DeckName()
    {
      Deck = deckRenamed.Deck,
      Name = deckRenamed.Name
    };
  }

  public Network.GenericResponse GetGenericResponse()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return new Network.GenericResponse()
      {
        RequestId = 0,
        RequestSubId = 1,
        ResultCode = Network.GenericResponse.Result.RESULT_OK
      };
    PegasusUtil.GenericResponse genericResponse = this.m_connectApi.GetGenericResponse();
    if (genericResponse == null)
      return (Network.GenericResponse) null;
    return new Network.GenericResponse()
    {
      ResultCode = (Network.GenericResponse.Result) genericResponse.ResultCode,
      RequestId = genericResponse.RequestId,
      RequestSubId = genericResponse.HasRequestSubId ? genericResponse.RequestSubId : 0,
      GenericData = (object) genericResponse.GenericData
    };
  }

  public void RequestNetCacheObject(GetAccountInfo.Request request) => this.m_connectApi.RequestAccountInfoNetCacheObject(request);

  public void RequestNetCacheObjectList(
    List<GetAccountInfo.Request> requestList,
    List<GenericRequest> genericRequests)
  {
    this.m_connectApi.RequestNetCacheObjectList(requestList, genericRequests);
  }

  public NetCache.NetCacheProfileProgress GetProfileProgress()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return new NetCache.NetCacheProfileProgress()
      {
        CampaignProgress = global::Options.Get().GetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS)
      };
    ProfileProgress profileProgress = this.m_connectApi.GetProfileProgress();
    if (profileProgress == null)
      return (NetCache.NetCacheProfileProgress) null;
    return new NetCache.NetCacheProfileProgress()
    {
      CampaignProgress = (TutorialProgress) profileProgress.Progress,
      BestForgeWins = profileProgress.BestForge,
      LastForgeDate = profileProgress.HasLastForge ? TimeUtils.PegDateToFileTimeUtc(profileProgress.LastForge) : 0L
    };
  }

  public void SetProgress(long value) => this.m_connectApi.SetProgress(value);

  public SetProgressResponse GetSetProgressResponse() => this.m_connectApi.GetSetProgressResponse();

  public void HandleProfileNotices(
    List<PegasusUtil.ProfileNotice> notices,
    ref List<NetCache.ProfileNotice> result)
  {
    for (int index = 0; index < notices.Count; ++index)
    {
      PegasusUtil.ProfileNotice notice = notices[index];
      NetCache.ProfileNotice profileNotice = (NetCache.ProfileNotice) null;
      if (notice.HasMedal)
      {
        Map<PegasusShared.ProfileNoticeMedal.MedalType, PegasusShared.FormatType> map = new Map<PegasusShared.ProfileNoticeMedal.MedalType, PegasusShared.FormatType>()
        {
          {
            PegasusShared.ProfileNoticeMedal.MedalType.UNKNOWN_MEDAL,
            PegasusShared.FormatType.FT_UNKNOWN
          },
          {
            PegasusShared.ProfileNoticeMedal.MedalType.WILD_MEDAL,
            PegasusShared.FormatType.FT_WILD
          },
          {
            PegasusShared.ProfileNoticeMedal.MedalType.STANDARD_MEDAL,
            PegasusShared.FormatType.FT_STANDARD
          },
          {
            PegasusShared.ProfileNoticeMedal.MedalType.CLASSIC_MEDAL,
            PegasusShared.FormatType.FT_CLASSIC
          }
        };
        PegasusShared.FormatType formatType = PegasusShared.FormatType.FT_UNKNOWN;
        if (notice.Medal.HasMedalType_)
          map.TryGetValue(notice.Medal.MedalType_, out formatType);
        NetCache.ProfileNoticeMedal profileNoticeMedal = new NetCache.ProfileNoticeMedal()
        {
          LeagueId = notice.Medal.LeagueId,
          StarLevel = notice.Medal.StarLevel,
          LegendRank = notice.Medal.HasLegendRank ? notice.Medal.LegendRank : 0,
          BestStarLevel = notice.Medal.HasBestStarLevel ? notice.Medal.BestStarLevel : 0,
          FormatType = formatType,
          WasLimitedByBestEverStarLevel = notice.Medal.HasWasLimitedByBestEverStarLevel && notice.Medal.WasLimitedByBestEverStarLevel
        };
        if (notice.Medal.HasChest)
          profileNoticeMedal.Chest = Network.ConvertRewardChest(notice.Medal.Chest);
        profileNotice = (NetCache.ProfileNotice) profileNoticeMedal;
      }
      else if (notice.HasRewardBooster)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardBooster()
        {
          Id = notice.RewardBooster.BoosterType,
          Count = notice.RewardBooster.BoosterCount
        };
      else if (notice.HasRewardCard)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardCard()
        {
          CardID = GameUtils.TranslateDbIdToCardId(notice.RewardCard.Card.Asset),
          Premium = (notice.RewardCard.Card.HasPremium ? (TAG_PREMIUM) notice.RewardCard.Card.Premium : TAG_PREMIUM.NORMAL),
          Quantity = (notice.RewardCard.HasQuantity ? notice.RewardCard.Quantity : 1)
        };
      else if (notice.HasPreconDeck)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticePreconDeck()
        {
          DeckID = notice.PreconDeck.Deck,
          HeroAsset = notice.PreconDeck.Hero
        };
      else if (notice.HasRewardDust)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardDust()
        {
          Amount = notice.RewardDust.Amount
        };
      else if (notice.HasRewardMount)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardMount()
        {
          MountID = notice.RewardMount.MountId
        };
      else if (notice.HasRewardForge)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardForge()
        {
          Quantity = notice.RewardForge.Quantity
        };
      else if (notice.HasRewardCurrency)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardCurrency()
        {
          Amount = notice.RewardCurrency.Amount,
          CurrencyType = (notice.HasRewardCurrency ? notice.RewardCurrency.CurrencyType : PegasusShared.CurrencyType.CURRENCY_TYPE_GOLD)
        };
      else if (notice.HasPurchase)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticePurchase()
        {
          PMTProductID = (notice.Purchase.HasPmtProductId ? new long?(notice.Purchase.PmtProductId) : new long?()),
          Data = (notice.Purchase.HasData ? notice.Purchase.Data : 0L),
          CurrencyCode = notice.Purchase.CurrencyCode
        };
      else if (notice.HasRewardCardBack)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardCardBack()
        {
          CardBackID = notice.RewardCardBack.CardBack
        };
      else if (notice.HasBonusStars)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeBonusStars()
        {
          StarLevel = notice.BonusStars.StarLevel,
          Stars = notice.BonusStars.Stars
        };
      else if (notice.HasDcGameResult)
      {
        if (!notice.DcGameResult.HasGameType)
        {
          Debug.LogError((object) "Network.GetProfileNotices(): Missing GameType");
          continue;
        }
        if (!notice.DcGameResult.HasMissionId)
        {
          Debug.LogError((object) "Network.GetProfileNotices(): Missing GameType");
          continue;
        }
        if (!notice.DcGameResult.HasGameResult_)
        {
          Debug.LogError((object) "Network.GetProfileNotices(): Missing GameResult");
          continue;
        }
        NetCache.ProfileNoticeDisconnectedGame disconnectedGame = new NetCache.ProfileNoticeDisconnectedGame()
        {
          GameType = notice.DcGameResult.GameType,
          FormatType = notice.DcGameResult.FormatType,
          MissionId = notice.DcGameResult.MissionId,
          GameResult = notice.DcGameResult.GameResult_
        };
        if (disconnectedGame.GameResult == ProfileNoticeDisconnectedGameResult.GameResult.GR_WINNER)
        {
          if (!notice.DcGameResult.HasYourResult || !notice.DcGameResult.HasOpponentResult)
          {
            Debug.LogError((object) "Network.GetProfileNotices(): Missing PlayerResult");
            continue;
          }
          disconnectedGame.YourResult = notice.DcGameResult.YourResult;
          disconnectedGame.OpponentResult = notice.DcGameResult.OpponentResult;
        }
        profileNotice = (NetCache.ProfileNotice) disconnectedGame;
      }
      else if (notice.HasDcGameResultNew)
      {
        if (!notice.DcGameResultNew.HasGameType)
        {
          Debug.LogError((object) "Network.GetProfileNotices(): Missing GameType");
          continue;
        }
        if (!notice.DcGameResultNew.HasMissionId)
        {
          Debug.LogError((object) "Network.GetProfileNotices(): Missing GameType");
          continue;
        }
        if (!notice.DcGameResultNew.HasGameResult_)
        {
          Debug.LogError((object) "Network.GetProfileNotices(): Missing GameResult");
          continue;
        }
        NetCache.ProfileNoticeDisconnectedGame disconnectedGame = new NetCache.ProfileNoticeDisconnectedGame()
        {
          GameType = notice.DcGameResultNew.GameType,
          FormatType = notice.DcGameResultNew.FormatType,
          MissionId = notice.DcGameResultNew.MissionId,
          GameResult = (ProfileNoticeDisconnectedGameResult.GameResult) notice.DcGameResultNew.GameResult_
        };
        if (disconnectedGame.GameResult == ProfileNoticeDisconnectedGameResult.GameResult.GR_WINNER)
        {
          if (!notice.DcGameResultNew.HasYourResult)
            Debug.LogError((object) "Network.GetProfileNotices(): Missing New PlayerResult");
          disconnectedGame.YourResult = (ProfileNoticeDisconnectedGameResult.PlayerResult) notice.DcGameResultNew.YourResult;
        }
        profileNotice = (NetCache.ProfileNotice) disconnectedGame;
      }
      else if (notice.HasAdventureProgress)
      {
        NetCache.ProfileNoticeAdventureProgress adventureProgress = new NetCache.ProfileNoticeAdventureProgress()
        {
          Wing = notice.AdventureProgress.WingId
        };
        switch ((NetCache.ProfileNotice.NoticeOrigin) notice.Origin)
        {
          case NetCache.ProfileNotice.NoticeOrigin.ADVENTURE_PROGRESS:
            adventureProgress.Progress = new int?(notice.HasOriginData ? (int) notice.OriginData : 0);
            break;
          case NetCache.ProfileNotice.NoticeOrigin.ADVENTURE_FLAGS:
            adventureProgress.Flags = new ulong?(notice.HasOriginData ? (ulong) notice.OriginData : 0UL);
            break;
        }
        profileNotice = (NetCache.ProfileNotice) adventureProgress;
      }
      else if (notice.HasLevelUp)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeLevelUp()
        {
          HeroClass = notice.LevelUp.HeroClass,
          NewLevel = notice.LevelUp.NewLevel,
          TotalLevel = notice.LevelUp.TotalLevel
        };
      else if (notice.HasAccountLicense)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeAcccountLicense()
        {
          License = notice.AccountLicense.License,
          CasID = notice.AccountLicense.CasId
        };
      else if (notice.HasTavernBrawlRewards)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeTavernBrawlRewards()
        {
          Chest = notice.TavernBrawlRewards.RewardChest,
          Wins = notice.TavernBrawlRewards.NumWins,
          Mode = (notice.TavernBrawlRewards.HasBrawlMode ? notice.TavernBrawlRewards.BrawlMode : TavernBrawlMode.TB_MODE_NORMAL)
        };
      else if (notice.HasTavernBrawlTicket)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeTavernBrawlTicket()
        {
          TicketType = notice.TavernBrawlTicket.TicketType,
          Quantity = notice.TavernBrawlTicket.Quantity
        };
      else if (notice.HasGenericRewardChest)
      {
        NetCache.ProfileNoticeGenericRewardChest genericRewardChest = new NetCache.ProfileNoticeGenericRewardChest();
        genericRewardChest.RewardChestAssetId = notice.GenericRewardChest.RewardChestAssetId;
        genericRewardChest.RewardChest = notice.GenericRewardChest.RewardChest;
        genericRewardChest.RewardChestByteSize = 0U;
        genericRewardChest.RewardChestHash = (byte[]) null;
        if (notice.GenericRewardChest.HasRewardChestByteSize)
          genericRewardChest.RewardChestByteSize = notice.GenericRewardChest.RewardChestByteSize;
        if (notice.GenericRewardChest.HasRewardChestHash)
          genericRewardChest.RewardChestHash = notice.GenericRewardChest.RewardChestHash;
        profileNotice = (NetCache.ProfileNotice) genericRewardChest;
      }
      else if (notice.HasLeaguePromotionRewards)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeLeaguePromotionRewards()
        {
          Chest = notice.LeaguePromotionRewards.RewardChest,
          LeagueId = notice.LeaguePromotionRewards.LeagueId
        };
      else if (notice.HasDeckRemoved)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeDeckRemoved()
        {
          DeckID = notice.DeckRemoved.DeckId
        };
      else if (notice.HasDeckGranted)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeDeckGranted()
        {
          DeckDbiID = notice.DeckGranted.DeckDbiId,
          ClassId = notice.DeckGranted.ClassId,
          PlayerDeckID = notice.DeckGranted.PlayerDeckId
        };
      else if (notice.HasMiniSetGranted)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMiniSetGranted()
        {
          MiniSetID = notice.MiniSetGranted.MiniSetId,
          Premium = notice.MiniSetGranted.Premium
        };
      else if (notice.HasSellableDeckGranted)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeSellableDeckGranted()
        {
          SellableDeckID = notice.SellableDeckGranted.SellableDeckId,
          PlayerDeckID = notice.SellableDeckGranted.PlayerDeckId,
          Premium = (TAG_PREMIUM) notice.SellableDeckGranted.Premium
        };
      else if (notice.HasBattlegroundsGuideSkinGranted)
      {
        int battlegroundsGuideSkinId = (int) notice.BattlegroundsGuideSkinGranted.BattlegroundsGuideSkinId;
        string cardId = GameUtils.TranslateDbIdToCardId(GameDbf.BattlegroundsGuideSkin.GetRecord(battlegroundsGuideSkinId).SkinCardId);
        FixedRewardDbfRecord fixedRewardRecord = GameDbf.FixedReward.GetRecord((Predicate<FixedRewardDbfRecord>) (x => x.BattlegroundsGuideSkinId == battlegroundsGuideSkinId));
        FixedRewardMapDbfRecord rewardMapDbfRecord = fixedRewardRecord != null ? GameDbf.FixedRewardMap.GetRecord((Predicate<FixedRewardMapDbfRecord>) (x => x.RewardId == fixedRewardRecord.ID)) : (FixedRewardMapDbfRecord) null;
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardBattlegroundsGuideSkin()
        {
          CardID = cardId,
          FixedRewardMapID = (rewardMapDbfRecord != null ? rewardMapDbfRecord.ID : 0)
        };
      }
      else if (notice.HasBattlegroundsHeroSkinGranted)
      {
        int battlegroundsHeroSkinId = (int) notice.BattlegroundsHeroSkinGranted.BattlegroundsHeroSkinId;
        int skinCardId = GameDbf.BattlegroundsHeroSkin.GetRecord(battlegroundsHeroSkinId).SkinCardId;
        GameUtils.TranslateDbIdToCardId(skinCardId);
        FixedRewardDbfRecord fixedRewardRecord = GameDbf.FixedReward.GetRecord((Predicate<FixedRewardDbfRecord>) (x => x.BattlegroundsHeroSkinId == battlegroundsHeroSkinId));
        FixedRewardMapDbfRecord rewardMapDbfRecord = fixedRewardRecord != null ? GameDbf.FixedRewardMap.GetRecord((Predicate<FixedRewardMapDbfRecord>) (x => x.RewardId == fixedRewardRecord.ID)) : (FixedRewardMapDbfRecord) null;
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardBattlegroundsHeroSkin()
        {
          CardID = GameUtils.TranslateDbIdToCardId(skinCardId),
          FixedRewardMapID = (rewardMapDbfRecord != null ? rewardMapDbfRecord.ID : 0)
        };
      }
      else if (notice.HasMercenariesRewards)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesRewards()
        {
          RewardType = notice.MercenariesRewards.RewardType_,
          Chest = notice.MercenariesRewards.RewardChest
        };
      else if (notice.HasMercenariesAbilityUnlock)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesAbilityUnlock()
        {
          MercenaryId = notice.MercenariesAbilityUnlock.MercenaryId,
          AbilityId = notice.MercenariesAbilityUnlock.AbilityId
        };
      else if (notice.HasMercenariesSeasonRoll)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesSeasonRoll()
        {
          EndedSeasonId = notice.MercenariesSeasonRoll.EndedSeasonId,
          HighestSeasonRating = notice.MercenariesSeasonRoll.HighestSeasonRating
        };
      else if (notice.HasMercenariesBoosterLicense)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesBoosterLicense()
        {
          Count = notice.MercenariesBoosterLicense.Count
        };
      else if (notice.HasMercenariesCurrencyLicense)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesCurrencyLicense()
        {
          MercenaryId = notice.MercenariesCurrencyLicense.MercenaryId,
          CurrencyAmount = notice.MercenariesCurrencyLicense.CurrencyAmount
        };
      else if (notice.HasMercenariesMercenaryLicense)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesMercenaryLicense()
        {
          MercenaryId = notice.MercenariesMercenaryLicense.MercenaryId,
          ArtVariationId = notice.MercenariesMercenaryLicense.ArtVariationId,
          ArtVariationPremium = notice.MercenariesMercenaryLicense.ArtVariationPremium,
          CurrencyAmount = notice.MercenariesMercenaryLicense.CurrencyAmount
        };
      else if (notice.HasMercenariesRandomRewardLicense)
      {
        PegasusShared.ProfileNoticeMercenariesRandomRewardLicense randomRewardLicense1 = notice.MercenariesRandomRewardLicense;
        NetCache.ProfileNoticeMercenariesRandomRewardLicense randomRewardLicense2 = new NetCache.ProfileNoticeMercenariesRandomRewardLicense();
        randomRewardLicense2.MercenaryId = randomRewardLicense1.MercenaryId;
        randomRewardLicense2.ArtVariationId = randomRewardLicense1.ArtVariationId;
        randomRewardLicense2.ArtVariationPremium = randomRewardLicense1.ArtVariationPremium;
        randomRewardLicense2.CurrencyAmount = randomRewardLicense1.CurrencyAmount;
        if (randomRewardLicense1.HasArtVariationId && randomRewardLicense1.HasCurrencyAmount)
          randomRewardLicense2.IsConvertedMercenary = true;
        profileNotice = (NetCache.ProfileNotice) randomRewardLicense2;
      }
      else if (notice.HasMercenariesMercenaryFullyUpgraded)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesMercenaryFullyUpgraded()
        {
          MercenaryId = notice.MercenariesMercenaryFullyUpgraded.MercenaryId
        };
      else if (notice.HasMercenariesSeasonRewards)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesSeasonRewards()
        {
          Chest = notice.MercenariesSeasonRewards.RewardChest,
          RewardAssetId = notice.MercenariesSeasonRewards.RewardAssetId
        };
      else if (notice.HasMercenariesZoneUnlock)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesZoneUnlock()
        {
          ZoneId = notice.MercenariesZoneUnlock.ZoneId
        };
      else if (notice.HasBattlegroundsBoardSkinGranted)
      {
        long boardSkinID = notice.BattlegroundsBoardSkinGranted.BattlegroundsBoardSkinId;
        FixedRewardDbfRecord fixedRewardRecord = GameDbf.FixedReward.GetRecord((Predicate<FixedRewardDbfRecord>) (x => (long) x.BattlegroundsBoardSkinId == boardSkinID));
        FixedRewardMapDbfRecord rewardMapDbfRecord = fixedRewardRecord != null ? GameDbf.FixedRewardMap.GetRecord((Predicate<FixedRewardMapDbfRecord>) (x => x.RewardId == fixedRewardRecord.ID)) : (FixedRewardMapDbfRecord) null;
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardBattlegroundsBoard()
        {
          BoardSkinID = boardSkinID,
          FixedRewardMapID = (rewardMapDbfRecord != null ? rewardMapDbfRecord.ID : 0)
        };
      }
      else if (notice.HasBattlegroundsFinisherGranted)
      {
        long finisherID = notice.BattlegroundsFinisherGranted.BattlegroundsFinisherId;
        FixedRewardDbfRecord fixedRewardRecord = GameDbf.FixedReward.GetRecord((Predicate<FixedRewardDbfRecord>) (x => (long) x.BattlegroundsFinisherId == finisherID));
        FixedRewardMapDbfRecord rewardMapDbfRecord = fixedRewardRecord != null ? GameDbf.FixedRewardMap.GetRecord((Predicate<FixedRewardMapDbfRecord>) (x => x.RewardId == fixedRewardRecord.ID)) : (FixedRewardMapDbfRecord) null;
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardBattlegroundsFinisher()
        {
          FinisherID = finisherID,
          FixedRewardMapID = (rewardMapDbfRecord != null ? rewardMapDbfRecord.ID : 0)
        };
      }
      else if (notice.HasBattlegroundsEmoteGranted)
      {
        long emoteID = notice.BattlegroundsEmoteGranted.BattlegroundsEmoteId;
        FixedRewardDbfRecord fixedRewardRecord = GameDbf.FixedReward.GetRecord((Predicate<FixedRewardDbfRecord>) (x => (long) x.BattlegroundsEmoteId == emoteID));
        FixedRewardMapDbfRecord rewardMapDbfRecord = fixedRewardRecord != null ? GameDbf.FixedRewardMap.GetRecord((Predicate<FixedRewardMapDbfRecord>) (x => x.RewardId == fixedRewardRecord.ID)) : (FixedRewardMapDbfRecord) null;
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardBattlegroundsEmote()
        {
          EmoteID = emoteID,
          FixedRewardMapID = (rewardMapDbfRecord != null ? rewardMapDbfRecord.ID : 0)
        };
      }
      else if (notice.HasLuckyDrawReward)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeLuckyDrawReward()
        {
          LuckyDrawRewardId = notice.LuckyDrawReward.LuckyDrawRewardId,
          LuckyDrawOrigin = notice.LuckyDrawReward.LuckyDrawOrigin
        };
      else if (notice.HasRedundantNdeReroll)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRedundantNDEReroll()
        {
          CardID = GameUtils.TranslateDbIdToCardId(notice.RedundantNdeReroll.NdeCard.Asset),
          Premium = (TAG_PREMIUM) notice.RedundantNdeReroll.NdeCard.Premium
        };
      else if (notice.HasRedundantNdeRerollResult)
        profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRedundantNDERerollResult()
        {
          RerolledCardID = notice.RedundantNdeRerollResult.NdeCard.Asset,
          GrantedCardID = notice.RedundantNdeRerollResult.GrantedCard.Asset,
          Premium = (TAG_PREMIUM) notice.RedundantNdeRerollResult.GrantedCard.Premium
        };
      else
        Debug.LogError((object) "Network.GetProfileNotices(): Unrecognized profile notice");
      if (profileNotice == null)
      {
        Debug.LogError((object) "Network.GetProfileNotices(): Unhandled notice type! This notice will be lost!");
      }
      else
      {
        profileNotice.NoticeID = notice.Entry;
        profileNotice.Origin = (NetCache.ProfileNotice.NoticeOrigin) notice.Origin;
        profileNotice.OriginData = notice.HasOriginData ? notice.OriginData : 0L;
        profileNotice.Date = TimeUtils.PegDateToFileTimeUtc(notice.When);
        result.Add(profileNotice);
      }
    }
  }

  public NetCache.NetCacheMedalInfo GetMedalInfo()
  {
    if (!Network.ShouldBeConnectedToAurora())
    {
      NetCache.NetCacheMedalInfo medalInfo = new NetCache.NetCacheMedalInfo();
      foreach (PegasusShared.FormatType key in System.Enum.GetValues(typeof (PegasusShared.FormatType)))
      {
        if (key != PegasusShared.FormatType.FT_UNKNOWN)
        {
          MedalInfoData medalInfoData = new MedalInfoData()
          {
            FormatType = key
          };
          medalInfo.MedalData.Add(key, medalInfoData);
        }
      }
      return medalInfo;
    }
    MedalInfo medalInfo1 = this.m_connectApi.GetMedalInfo();
    return medalInfo1 == null ? (NetCache.NetCacheMedalInfo) null : new NetCache.NetCacheMedalInfo(medalInfo1);
  }

  public NetCache.NetCacheBaconRatingInfo GetBaconRatingInfo()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return new NetCache.NetCacheBaconRatingInfo()
      {
        Rating = 0
      };
    ResponseWithRequest<BattlegroundsRatingInfoResponse, BattlegroundsRatingInfoRequest> responseWithRequest = this.m_connectApi.BattlegroundsRatingInfoResponse();
    if (responseWithRequest == null)
      return (NetCache.NetCacheBaconRatingInfo) null;
    BattlegroundsRatingInfoResponse response = responseWithRequest.Response;
    if (response == null)
      return (NetCache.NetCacheBaconRatingInfo) null;
    return new NetCache.NetCacheBaconRatingInfo()
    {
      Rating = response.PlayerInfo.Rating
    };
  }

  public NetCache.NetCachePVPDRStatsInfo GetPVPDRStatsInfo()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return new NetCache.NetCachePVPDRStatsInfo()
      {
        Rating = 0,
        PaidRating = 0,
        HighWatermark = 0
      };
    ResponseWithRequest<PVPDRStatsInfoResponse, PVPDRStatsInfoRequest> responseWithRequest = this.m_connectApi.PVPDRStatsInfoResponse();
    if (responseWithRequest == null)
      return (NetCache.NetCachePVPDRStatsInfo) null;
    PVPDRStatsInfoResponse response = responseWithRequest.Response;
    if (response == null)
      return (NetCache.NetCachePVPDRStatsInfo) null;
    return new NetCache.NetCachePVPDRStatsInfo()
    {
      Rating = response.Rating,
      PaidRating = response.PaidRating,
      HighWatermark = response.HighWatermark
    };
  }

  public GuardianVars GetGuardianVars() => !Network.ShouldBeConnectedToAurora() ? new GuardianVars() : this.m_connectApi.GetGuardianVars();

  public PlayerRecords GetPlayerRecordsPacket() => !Network.ShouldBeConnectedToAurora() ? new PlayerRecords() : this.m_connectApi.GetPlayerRecords();

  public static NetCache.NetCachePlayerRecords GetPlayerRecords(PlayerRecords packet)
  {
    if (packet == null)
      return (NetCache.NetCachePlayerRecords) null;
    NetCache.NetCachePlayerRecords playerRecords = new NetCache.NetCachePlayerRecords();
    for (int index = 0; index < packet.Records.Count; ++index)
    {
      PegasusUtil.PlayerRecord record = packet.Records[index];
      playerRecords.Records.Add(new NetCache.PlayerRecord()
      {
        RecordType = record.Type,
        Data = record.HasData ? record.Data : 0,
        Wins = record.Wins,
        Losses = record.Losses,
        Ties = record.Ties
      });
    }
    return playerRecords;
  }

  public NetCache.NetCacheRewardProgress GetRewardProgress()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return new NetCache.NetCacheRewardProgress();
    RewardProgress rewardProgress = this.m_connectApi.GetRewardProgress();
    if (rewardProgress == null)
      return (NetCache.NetCacheRewardProgress) null;
    return new NetCache.NetCacheRewardProgress()
    {
      Season = rewardProgress.SeasonNumber,
      SeasonEndDate = TimeUtils.PegDateToFileTimeUtc(rewardProgress.SeasonEnd),
      NextQuestCancelDate = TimeUtils.PegDateToFileTimeUtc(rewardProgress.NextQuestCancel)
    };
  }

  public NetCache.NetCacheGamesPlayed GetGamesInfo()
  {
    GamesInfo gamesInfo = this.m_connectApi.GetGamesInfo();
    if (gamesInfo == null)
      return (NetCache.NetCacheGamesPlayed) null;
    return new NetCache.NetCacheGamesPlayed()
    {
      GamesStarted = gamesInfo.GamesStarted,
      GamesWon = gamesInfo.GamesWon,
      GamesLost = gamesInfo.GamesLost
    };
  }

  public ClientStaticAssetsResponse GetClientStaticAssetsResponse() => !Network.ShouldBeConnectedToAurora() ? new ClientStaticAssetsResponse() : this.m_connectApi.GetClientStaticAssetsResponse();

  public void RequestTavernBrawlInfo(BrawlType brawlType)
  {
    long? fsgId = FiresideGatheringManager.Get().IsCheckedIn ? new long?(FiresideGatheringManager.Get().CurrentFsgId) : new long?();
    this.m_connectApi.RequestTavernBrawlInfo(brawlType, fsgId, FiresideGatheringManager.Get().CurrentFsgSharedSecretKey);
  }

  public void RequestTavernBrawlPlayerRecord(BrawlType brawlType)
  {
    long? fsgId = FiresideGatheringManager.Get().IsCheckedIn ? new long?(FiresideGatheringManager.Get().CurrentFsgId) : new long?();
    this.m_connectApi.RequestTavernBrawlPlayerRecord(brawlType, fsgId, FiresideGatheringManager.Get().CurrentFsgSharedSecretKey);
  }

  public TavernBrawlInfo GetTavernBrawlInfo() => !Network.ShouldBeConnectedToAurora() ? new TavernBrawlInfo() : this.m_connectApi.GetTavernBrawlInfo();

  public TavernBrawlRequestSessionBeginResponse GetTavernBrawlSessionBegin() => this.m_connectApi.GetTavernBrawlSessionBeginResponse();

  public void TavernBrawlRetire() => this.m_connectApi.TavernBrawlRetire();

  public TavernBrawlRequestSessionRetireResponse GetTavernBrawlSessionRetired() => this.m_connectApi.GetTavernBrawlSessionRetired();

  public void RequestTavernBrawlSessionBegin() => this.m_connectApi.RequestTavernBrawlSessionBegin();

  public void AckTavernBrawlSessionRewards() => this.m_connectApi.AckTavernBrawlSessionRewards();

  public TavernBrawlPlayerRecord GetTavernBrawlRecord()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return new TavernBrawlPlayerRecord();
    return this.m_connectApi.GeTavernBrawlPlayerRecordResponse()?.Record;
  }

  public FavoriteHeroesResponse GetFavoriteHeroesResponse() => !Network.ShouldBeConnectedToAurora() ? new FavoriteHeroesResponse() : this.m_connectApi.GetFavoriteHeroesResponse();

  public AccountLicensesInfoResponse GetAccountLicensesInfoResponse() => !Network.ShouldBeConnectedToAurora() ? new AccountLicensesInfoResponse() : this.m_connectApi.GetAccountLicensesInfoResponse();

  public void RequestAccountLicensesUpdate() => this.m_connectApi.RequestAccountLicensesUpdate();

  public UpdateAccountLicensesResponse GetUpdateAccountLicensesResponse() => this.m_connectApi.GetUpdateAccountLicensesResponse();

  public HeroXP GetHeroXP() => !Network.ShouldBeConnectedToAurora() ? new HeroXP() : this.m_connectApi.GetHeroXP();

  public void AckNotice(long id)
  {
    if (!NetCache.Get().RemoveNotice(id))
      return;
    Log.Achievements.Print("acking notice: {0}", (object) id);
    this.m_connectApi.AckNotice(id);
  }

  public void AckAchieveProgress(int id, int ackProgress)
  {
    Log.Achievements.Print("AckAchieveProgress: Achieve={0} Progress={1}", (object) id, (object) ackProgress);
    this.m_connectApi.AckAchieveProgress(id, ackProgress);
  }

  public void AckQuest(int questId) => this.m_connectApi.AckQuest(questId);

  public void CheckForNewQuests() => this.m_connectApi.CheckForNewQuests();

  public void CheckForExpiredQuests() => this.m_connectApi.CheckForExpiredQuests();

  public void RerollQuest(int questId) => this.m_connectApi.RerollQuest(questId);

  public void AbandonQuest(int questId) => this.m_connectApi.AbandonQuest(questId);

  public void AckAchievement(int achievementId) => this.m_connectApi.AckAchievement(achievementId);

  public void ClaimAchievementReward(int achievementId, int chooseOneRewardId = 0) => this.m_connectApi.ClaimAchievementReward(achievementId, chooseOneRewardId);

  public void AckRewardTrackReward(int rewardTrackId, int level, bool forPaidTrack) => this.m_connectApi.AckRewardTrackReward(rewardTrackId, level, forPaidTrack);

  public void ClaimRewardTrackReward(
    int rewardTrackId,
    int level,
    bool forPaidTrack,
    int chooseOneRewardItemId)
  {
    this.m_connectApi.ClaimRewardTrackReward(rewardTrackId, level, forPaidTrack, chooseOneRewardItemId);
  }

  public void CheckForRewardTrackSeasonRoll() => this.m_connectApi.CheckForRewardTrackSeasonRoll();

  public void CheckAccountLicenseAchieve(int achieveID) => this.m_connectApi.CheckAccountLicenseAchieve(achieveID);

  public Network.AccountLicenseAchieveResponse GetAccountLicenseAchieveResponse()
  {
    PegasusUtil.AccountLicenseAchieveResponse licenseAchieveResponse = this.m_connectApi.GetAccountLicenseAchieveResponse();
    if (licenseAchieveResponse == null)
      return (Network.AccountLicenseAchieveResponse) null;
    return new Network.AccountLicenseAchieveResponse()
    {
      Achieve = licenseAchieveResponse.Achieve,
      Result = (Network.AccountLicenseAchieveResponse.AchieveResult) licenseAchieveResponse.Result_
    };
  }

  public void RespondToRedundantNDEReroll(List<long> noticeIds, bool didReroll) => this.m_connectApi.RespondToRedundantNDEReroll(noticeIds, didReroll);

  public void AckCardSeenBefore(int assetId, TAG_PREMIUM premium)
  {
    PegasusShared.CardDef cardDef = new PegasusShared.CardDef()
    {
      Asset = assetId
    };
    if (premium != TAG_PREMIUM.NORMAL)
      cardDef.Premium = (int) premium;
    this.m_ackCardSeenPacket.CardDefs.Add(cardDef);
    if (this.m_ackCardSeenPacket.CardDefs.Count <= 15)
      return;
    this.SendAckCardsSeen();
  }

  public void AckWingProgress(int wingId, int ackId) => this.m_connectApi.AckWingProgress(wingId, ackId);

  public void AcknowledgeBanner(int banner) => this.m_connectApi.AcknowledgeBanner(banner);

  public void SendAckCardsSeen()
  {
    this.m_connectApi.AckCardSeen(this.m_ackCardSeenPacket);
    this.m_ackCardSeenPacket.CardDefs.Clear();
  }

  public void RequestNearbyFSGs(
    double latitude,
    double longitude,
    double accuracy,
    List<string> bssids)
  {
    PegasusShared.Platform platformBuilder = this.GetPlatformBuilder();
    this.m_connectApi.RequestNearbyFSGs(latitude, longitude, accuracy, bssids, platformBuilder);
  }

  public void RequestNearbyFSGs(List<string> bssids)
  {
    PegasusShared.Platform platformBuilder = this.GetPlatformBuilder();
    this.m_connectApi.RequestNearbyFSGs(bssids, platformBuilder);
  }

  public void CheckInToFSG(
    long gatheringID,
    double latitude,
    double longitude,
    double accuracy,
    List<string> bssids)
  {
    PegasusShared.Platform platformBuilder = this.GetPlatformBuilder();
    this.m_connectApi.CheckInToFSG(gatheringID, latitude, longitude, accuracy, bssids, platformBuilder);
  }

  public void CheckInToFSG(long gatheringID, List<string> bssids)
  {
    PegasusShared.Platform platformBuilder = this.GetPlatformBuilder();
    this.m_connectApi.CheckInToFSG(gatheringID, bssids, platformBuilder);
  }

  public void CheckOutOfFSG(long gatheringID)
  {
    Log.FiresideGatherings.Print("CheckOutOfFSG: sending check out to server for {0}", (object) gatheringID);
    PegasusShared.Platform platformBuilder = this.GetPlatformBuilder();
    this.m_connectApi.CheckOutOfFSG(gatheringID, platformBuilder);
  }

  public void InnkeeperSetupFSG(
    double latitude,
    double longitude,
    double accuracy,
    List<string> bssids,
    long fsgId)
  {
    PegasusShared.Platform platformBuilder = this.GetPlatformBuilder();
    ConnectAPI connectApi = this.m_connectApi;
    List<string> bssids1 = bssids;
    long fsgId1 = fsgId;
    GPSCoords location = new GPSCoords();
    location.Latitude = latitude;
    location.Longitude = longitude;
    location.Accuracy = accuracy;
    PegasusShared.Platform platform = platformBuilder;
    connectApi.InnkeeperSetupFSG(bssids1, fsgId1, location, platform);
  }

  public void InnkeeperSetupFSG(List<string> bssids, long fsgId)
  {
    PegasusShared.Platform platformBuilder = this.GetPlatformBuilder();
    this.m_connectApi.InnkeeperSetupFSG(bssids, fsgId, platformBuilder);
  }

  public void RequestFSGPatronListUpdate() => this.m_connectApi.RequestFSGPatronListUpdate();

  public void RequestLeaguePromoteSelf() => this.m_connectApi.RequestLeaguePromoteSelf();

  public RequestNearbyFSGsResponse GetRequestNearbyFSGsResponse() => this.m_connectApi.GetRequestNearbyFSGsResponse();

  public CheckInToFSGResponse GetCheckInToFSGResponse() => this.m_connectApi.GetCheckInToFSGResponse();

  public CheckOutOfFSGResponse GetCheckOutOfFSGResponse() => this.m_connectApi.GetCheckOutOfFSGResponse();

  public InnkeeperSetupGatheringResponse GetInnkeeperSetupGatheringResponse() => this.m_connectApi.GetInnkeeperSetupGatheringResponse();

  public FSGPatronListUpdate GetFSGPatronListUpdate() => this.m_connectApi.GetFSGPatronListUpdate();

  public FSGFeatureConfig GetFSGFeatureConfig() => this.m_connectApi.GetFSGFeatureConfig();

  public LeaguePromoteSelfResponse GetLeaguePromoteSelfResponse() => this.m_connectApi.GetLeaguePromoteSelfResponse();

  public SmartDeckResponse GetSmartDeckResponse() => this.m_connectApi.GetSmartDeckResponse();

  public PlayerQuestStateUpdate GetPlayerQuestStateUpdate() => this.m_connectApi.GetPlayerQuestStateUpdate();

  public PlayerQuestPoolStateUpdate GetPlayerQuestPoolStateUpdate() => this.m_connectApi.GetPlayerQuestPoolStateUpdate();

  public PlayerAchievementStateUpdate GetPlayerAchievementStateUpdate() => this.m_connectApi.GetPlayerAchievementStateUpdate();

  public PlayerRewardTrackStateUpdate GetPlayerRewardTrackStateUpdate() => this.m_connectApi.GetPlayerRewardTrackStateUpdate();

  public RerollQuestResponse GetRerollQuestResponse() => this.m_connectApi.GetRerollQuestResponse();

  public RewardTrackXpNotification GetRewardTrackXpNotification() => this.m_connectApi.GetRewardTrackXpNotification();

  public RewardTrackUnclaimedNotification GetRewardTrackUnclaimedNotification() => this.m_connectApi.GetRewardTrackUnclaimedNotification();

  public BattlegroundsHeroSkinsResponse GetBattlegroundsHeroSkinsResponse() => !Network.ShouldBeConnectedToAurora() ? new BattlegroundsHeroSkinsResponse() : this.m_connectApi.GetBattlegroundsHeroSkinsResponse();

  public SetBattlegroundsFavoriteHeroSkinResponse GetSetBattlegroundsFavoriteHeroSkinResponse() => this.m_connectApi.GetSetBattlegroundsFavoriteHeroSkinResponse();

  public void SetBattlegroundsFavoriteHeroSkin(BattlegroundsHeroSkinId bgFavoriteSkinID)
  {
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.SetBattlegroundsFavoriteHeroSkin(bgFavoriteSkinID.ToValue());
    }
    else
    {
      NetCache.NetCacheBattlegroundsHeroSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
      if (netObject == null)
        return;
      int baseHeroCardId = 0;
      if (CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(bgFavoriteSkinID, out baseHeroCardId))
        netObject.BattlegroundsFavoriteHeroSkins[baseHeroCardId] = bgFavoriteSkinID;
      else
        Log.Net.PrintError("Network.SetBattlegroundsFavoriteHeroSkin(): could not find base card ID (skin ID = {0})", (object) bgFavoriteSkinID);
    }
  }

  public void ClearBattlegroundsFavoriteHeroSkin(BattlegroundsHeroSkinId bgFavoriteSkinID)
  {
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.ClearBattlegroundsFavoriteHeroSkin(bgFavoriteSkinID.ToValue());
    }
    else
    {
      NetCache.NetCacheBattlegroundsHeroSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
      if (netObject == null)
        return;
      int baseHeroCardId = 0;
      if (CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(bgFavoriteSkinID, out baseHeroCardId))
        netObject.BattlegroundsFavoriteHeroSkins.Remove(baseHeroCardId);
      else
        Log.Net.PrintError("Network.ClearBattlegroundsFavoriteHeroSkin(): could not find base card ID (skin ID = {0})", (object) bgFavoriteSkinID);
    }
  }

  public ClearBattlegroundsFavoriteHeroSkinResponse GetClearBattlegroundsFavoriteHeroSkinResponse() => this.m_connectApi.GetClearBattlegroundsFavoriteHeroSkinResponse();

  public BattlegroundsGuideSkinsResponse GetBattlegroundsGuideSkinsResponse() => !Network.ShouldBeConnectedToAurora() ? new BattlegroundsGuideSkinsResponse() : this.m_connectApi.GetBattlegroundsGuideSkinsResponse();

  public SetBattlegroundsFavoriteGuideSkinResponse GetSetBattlegroundsFavoriteGuideSkinResponse() => this.m_connectApi.GetSetBattlegroundsFavoriteGuideSkinResponse();

  public void SetBattlegroundsFavoriteGuideSkin(BattlegroundsGuideSkinId bgFavoriteGuideSkinID)
  {
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.SetBattlegroundsFavoriteGuideSkin(bgFavoriteGuideSkinID.ToValue());
    }
    else
    {
      NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
      if (netObject == null)
        return;
      netObject.BattlegroundsFavoriteGuideSkin = new BattlegroundsGuideSkinId?(bgFavoriteGuideSkinID);
    }
  }

  public void ClearBattlegroundsFavoriteGuideSkin()
  {
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.ClearBattlegroundsFavoriteGuideSkin();
    }
    else
    {
      NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
      if (netObject == null)
        return;
      netObject.BattlegroundsFavoriteGuideSkin = new BattlegroundsGuideSkinId?();
    }
  }

  public ClearBattlegroundsFavoriteGuideSkinResponse GetClearBattlegroundsFavoriteGuideSkinResponse() => this.m_connectApi.GetClearBattlegroundsFavoriteGuideSkinResponse();

  public bool TryAddSeenBattlegroundsHeroSkin(BattlegroundsHeroSkinId skinId)
  {
    NetCache.NetCacheBattlegroundsHeroSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
    if (netObject == null || !netObject.UnseenSkinIds.Remove(skinId))
      return false;
    this.m_ackBattlegroundsSkinsSeenPacket.HeroSkins.Add(skinId.ToValue());
    this.CheckForSendingBattlegroundsSkinsSeenPacket();
    return true;
  }

  public bool TryAddSeenBattlegroundsGuideSkin(BattlegroundsGuideSkinId skinId)
  {
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    if (netObject == null || !netObject.UnseenSkinIds.Remove(skinId))
      return false;
    this.m_ackBattlegroundsSkinsSeenPacket.GuideSkins.Add(skinId.ToValue());
    this.CheckForSendingBattlegroundsSkinsSeenPacket();
    return true;
  }

  public bool TryAddSeenBattlegroundsBoardSkin(BattlegroundsBoardSkinId skinId)
  {
    NetCache.NetCacheBattlegroundsBoardSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
    if (netObject == null || !netObject.UnseenSkinIds.Remove(skinId))
      return false;
    this.m_ackBattlegroundsSkinsSeenPacket.BoardSkins.Add(skinId.ToValue());
    this.CheckForSendingBattlegroundsSkinsSeenPacket();
    return true;
  }

  public bool TryAddSeenBattlegroundsFinisher(BattlegroundsFinisherId finisherId)
  {
    NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    if (netObject == null || !netObject.UnseenSkinIds.Remove(finisherId))
      return false;
    this.m_ackBattlegroundsSkinsSeenPacket.Finishers.Add(finisherId.ToValue());
    this.CheckForSendingBattlegroundsSkinsSeenPacket();
    return true;
  }

  public bool TryAddSeenBattlegroundsEmote(BattlegroundsEmoteId emoteId)
  {
    NetCache.NetCacheBattlegroundsEmotes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
    if (netObject == null || !netObject.UnseenEmoteIds.Remove(emoteId))
      return false;
    this.m_ackBattlegroundsSkinsSeenPacket.Emotes.Add(emoteId.ToValue());
    this.CheckForSendingBattlegroundsSkinsSeenPacket();
    return true;
  }

  public void CheckForSendingBattlegroundsSkinsSeenPacket(int minToSend = 16)
  {
    if (this.m_ackBattlegroundsSkinsSeenPacket.HeroSkins.Count + this.m_ackBattlegroundsSkinsSeenPacket.GuideSkins.Count + this.m_ackBattlegroundsSkinsSeenPacket.BoardSkins.Count + this.m_ackBattlegroundsSkinsSeenPacket.Finishers.Count + this.m_ackBattlegroundsSkinsSeenPacket.Emotes.Count < minToSend || !Network.IsLoggedIn())
      return;
    this.m_connectApi.SendAckBattlegroundSkinsSeenPacket(this.m_ackBattlegroundsSkinsSeenPacket);
    this.m_ackBattlegroundsSkinsSeenPacket.HeroSkins.Clear();
    this.m_ackBattlegroundsSkinsSeenPacket.GuideSkins.Clear();
    this.m_ackBattlegroundsSkinsSeenPacket.BoardSkins.Clear();
    this.m_ackBattlegroundsSkinsSeenPacket.Finishers.Clear();
    this.m_ackBattlegroundsSkinsSeenPacket.Emotes.Clear();
  }

  public BattlegroundsBoardSkinsResponse GetBattlegroundsBoardSkinsResponse() => !Network.ShouldBeConnectedToAurora() ? new BattlegroundsBoardSkinsResponse() : this.m_connectApi.GetBattlegroundsBoardSkinsResponse();

  public SetBattlegroundsFavoriteBoardSkinResponse GetSetBattlegroundsFavoriteBoardSkinResponse() => this.m_connectApi.GetSetBattlegroundsFavoriteBoardSkinResponse();

  public void SetBattlegroundsFavoriteBoardSkin(BattlegroundsBoardSkinId bgFavoriteBoardSkinID)
  {
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.SetBattlegroundsFavoriteBoardSkin(bgFavoriteBoardSkinID.ToValue());
    }
    else
    {
      NetCache.NetCacheBattlegroundsBoardSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
      if (netObject == null)
        return;
      netObject.BattlegroundsFavoriteBoardSkin = new BattlegroundsBoardSkinId?(bgFavoriteBoardSkinID);
    }
  }

  public void ClearBattlegroundsFavoriteBoardSkin()
  {
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.ClearBattlegroundsFavoriteBoardSkin();
    }
    else
    {
      NetCache.NetCacheBattlegroundsBoardSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
      if (netObject == null)
        return;
      netObject.BattlegroundsFavoriteBoardSkin = new BattlegroundsBoardSkinId?();
    }
  }

  public ClearBattlegroundsFavoriteBoardSkinResponse GetClearBattlegroundsFavoriteBoardSkinResponse() => this.m_connectApi.GetClearBattlegroundsFavoriteBoardSkinResponse();

  public BattlegroundsFinishersResponse GetBattlegroundsFinishersResponse() => !Network.ShouldBeConnectedToAurora() ? new BattlegroundsFinishersResponse() : this.m_connectApi.GetBattlegroundsFinishersResponse();

  public SetBattlegroundsFavoriteFinisherResponse GetSetBattlegroundsFavoriteFinisherResponse() => this.m_connectApi.GetSetBattlegroundsFavoriteFinisherResponse();

  public void SetBattlegroundsFavoriteFinisher(BattlegroundsFinisherId bgFavoriteFinisherID)
  {
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.SetBattlegroundsFavoriteFinisher(bgFavoriteFinisherID.ToValue());
    }
    else
    {
      NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
      if (netObject == null)
        return;
      netObject.BattlegroundsFavoriteFinisher = new BattlegroundsFinisherId?(bgFavoriteFinisherID);
    }
  }

  public void ClearBattlegroundsFavoriteFinisher()
  {
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.ClearBattlegroundsFavoriteFinisher();
    }
    else
    {
      NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
      if (netObject == null)
        return;
      netObject.BattlegroundsFavoriteFinisher = new BattlegroundsFinisherId?();
    }
  }

  public ClearBattlegroundsFavoriteFinisherResponse GetClearBattlegroundsFavoriteFinisherResponse() => this.m_connectApi.GetClearBattlegroundsFavoriteFinisherResponse();

  public BattlegroundsEmotesResponse GetBattlegroundsEmotesResponse() => !Network.ShouldBeConnectedToAurora() ? new BattlegroundsEmotesResponse() : this.m_connectApi.GetBattlegroundsEmotesResponse();

  public SetBattlegroundsEmoteLoadoutResponse GetSetBattlegroundsEmoteLoadoutResponse() => this.m_connectApi.GetSetBattlegroundsEmoteLoadoutResponse();

  public void SetBattlegroundsEmoteLoadout(Hearthstone.BattlegroundsEmoteLoadout loadout)
  {
    int num = 0;
    while (num < loadout.Emotes.Length)
      ++num;
    if (Network.IsLoggedIn())
    {
      this.m_connectApi.SetBattlegroundsEmoteLoadout(loadout);
    }
    else
    {
      NetCache.NetCacheBattlegroundsEmotes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
      if (netObject == null)
        return;
      netObject.CurrentLoadout = loadout;
    }
  }

  public LettuceMapResponse GetLettuceMapResponse() => this.m_connectApi.GetLettuceMapResponse();

  public LettuceMapChooseNodeResponse GetLettuceMapChooseNodeResponse() => this.m_connectApi.GetLettuceMapChooseNodeResponse();

  public LettuceMapRetireResponse GetLettuceMapRetireResponse() => this.m_connectApi.GetLettuceMapRetireResponse();

  public MercenariesMapTreasureSelectionResponse GetMercenariesMapTreasureSelectionResponse() => this.m_connectApi.GetMercenariesMapTreasureSelectionResponse();

  public MercenariesMapVisitorSelectionResponse GetMercenariesMapVisitorSelectionResponse() => this.m_connectApi.GetMercenariesMapVisitorSelectionResponse();

  public void RequestGameSaveData(List<long> keys, int clientToken) => this.m_connectApi.RequestGameSaveData(keys, clientToken);

  public GameSaveDataResponse GetGameSaveDataResponse() => this.m_connectApi.GetGameSaveDataResponse();

  public void SetGameSaveData(List<GameSaveDataUpdate> dataUpdates, int clientToken) => this.m_connectApi.SetGameSaveData(dataUpdates, clientToken);

  public SetGameSaveDataResponse GetSetGameSaveDataResponse() => this.m_connectApi.GetSetGameSaveDataResponse();

  public GameSaveDataStateUpdate GetGameSaveDataStateUpdate() => this.m_connectApi.GetGameSaveDataStateUpdate();

  public Network.CardSaleResult GetCardSaleResult()
  {
    BoughtSoldCard cardSaleResult1 = this.m_connectApi.GetCardSaleResult();
    if (cardSaleResult1 == null)
      return (Network.CardSaleResult) null;
    Network.CardSaleResult cardSaleResult2 = new Network.CardSaleResult();
    cardSaleResult2.AssetID = cardSaleResult1.Def.Asset;
    cardSaleResult2.AssetName = GameUtils.TranslateDbIdToCardId(cardSaleResult1.Def.Asset);
    cardSaleResult2.Premium = cardSaleResult1.Def.HasPremium ? (TAG_PREMIUM) cardSaleResult1.Def.Premium : TAG_PREMIUM.NORMAL;
    cardSaleResult2.Action = (Network.CardSaleResult.SaleResult) cardSaleResult1.Result_;
    if (!cardSaleResult1.HasCollectionVersion)
      return cardSaleResult2;
    NetCache.Get().AddExpectedCollectionModification(cardSaleResult1.CollectionVersion);
    return cardSaleResult2;
  }

  public void TriggerPlayedNearbyPlayerOnSubnet(
    BnetGameAccountId lastOpponentHSGameAccountID,
    ulong lastOpponentSessionStartTime,
    BnetGameAccountId otherPlayerHSGameAccountID,
    ulong otherPlayerSessionStartTime)
  {
    this.m_connectApi.TriggerPlayedNearbyPlayerOnSubnet(lastOpponentHSGameAccountID.High, lastOpponentHSGameAccountID.Low, lastOpponentSessionStartTime, otherPlayerHSGameAccountID.High, otherPlayerHSGameAccountID.Low, otherPlayerSessionStartTime);
  }

  public void RequestAssetsVersion()
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    this.m_connectApi.RequestAssetsVersion(this.GetPlatformBuilder(), OfflineDataCache.GetCachedCollectionVersion(data), OfflineDataCache.GetCachedDeckContentsTimes(data), OfflineDataCache.GetCachedCollectionVersionLastModified(data));
  }

  public void LoginOk() => this.m_connectApi.OnLoginComplete();

  public AssetsVersionResponse GetAssetsVersion() => this.m_connectApi.GetAssetsVersionResponse();

  public PegasusUtil.GetAssetResponse GetAssetResponse() => this.m_connectApi.GetAssetResponse();

  public void SendAssetRequest(int clientToken, List<AssetKey> requestKeys)
  {
    if (requestKeys == null || requestKeys.Count == 0)
      return;
    long? fsgId = FiresideGatheringManager.Get().IsCheckedIn ? new long?(FiresideGatheringManager.Get().CurrentFsgId) : new long?();
    this.m_connectApi.SendAssetRequest(clientToken, requestKeys, fsgId, FiresideGatheringManager.Get().CurrentFsgSharedSecretKey);
  }

  public ServerResult GetServerResult() => this.m_connectApi.GetServerResult();

  private PegasusShared.Platform GetPlatformBuilder()
  {
    PegasusShared.Platform platformBuilder = new PegasusShared.Platform()
    {
      Os = (int) PlatformSettings.OS,
      Screen = (int) PlatformSettings.Screen,
      Name = PlatformSettings.DeviceName,
      UniqueDeviceIdentifier = SystemInfo.deviceUniqueIdentifier
    };
    AndroidStore androidStore = AndroidDeviceSettings.Get().GetAndroidStore();
    if (androidStore != AndroidStore.NONE)
      platformBuilder.Store = (int) androidStore;
    return platformBuilder;
  }

  public bool SendDebugConsoleCommand(string command)
  {
    if (!this.IsConnectedToGameServer())
    {
      Log.Net.Print(string.Format("Cannot send command '{0}' to server; no game server is active.", (object) command));
      return false;
    }
    if (this.m_connectApi.AllowDebugConnections() && command != null)
      this.m_connectApi.SendDebugConsoleCommand(command);
    return true;
  }

  public void SendDebugConsoleResponse(int responseType, string message) => this.m_connectApi.SendDebugConsoleResponse(responseType, message);

  public string GetDebugConsoleCommand()
  {
    DebugConsoleCommand debugConsoleCommand = this.m_connectApi.GetDebugConsoleCommand();
    return debugConsoleCommand == null ? string.Empty : debugConsoleCommand.Command;
  }

  public Network.DebugConsoleResponse GetDebugConsoleResponse()
  {
    BobNetProto.DebugConsoleResponse debugConsoleResponse = this.m_connectApi.GetDebugConsoleResponse();
    if (debugConsoleResponse == null)
      return (Network.DebugConsoleResponse) null;
    return new Network.DebugConsoleResponse()
    {
      Type = (int) debugConsoleResponse.ResponseType_,
      Response = debugConsoleResponse.Response
    };
  }

  public void SendDebugCommandRequest(DebugCommandRequest packet) => this.m_connectApi.SendDebugCommandRequest(packet);

  public DebugCommandResponse GetDebugCommandResponse() => this.m_connectApi.GetDebugCommandResponse();

  public void SendLocateCheatServerRequest() => this.m_connectApi.SendLocateCheatServerRequest();

  public LocateCheatServerResponse GetLocateCheatServerResponse() => this.m_connectApi.GetLocateCheatServerResponse();

  public GameToConnectNotification GetGameToConnectNotification() => this.m_connectApi.GetGameToConnectNotification();

  public void GetServerTimeRequest() => this.m_connectApi.GetServerTimeRequest((long) TimeUtils.DateTimeToUnixTimeStamp(DateTime.Now));

  public void ReportBlizzardCheckoutStatus(BlizzardCheckoutStatus status, TransactionData data = null) => this.m_connectApi.ReportBlizzardCheckoutStatus(status, data, (long) TimeUtils.DateTimeToUnixTimeStamp(DateTime.Now));

  public ResponseWithRequest<PegasusUtil.GetServerTimeResponse, PegasusUtil.GetServerTimeRequest> GetServerTimeResponse() => this.m_connectApi.GetServerTimeResponse();

  public void SimulateUncleanDisconnectFromGameServer()
  {
    if (!this.m_connectApi.HasGameServerConnection())
      return;
    this.m_connectApi.DisconnectFromGameServer();
  }

  public void SimulateReceivedPacketFromServer(PegasusPacket packet) => this.m_dispatcherImpl.NotifyUtilResponseReceived(packet);

  private static string GetStoredUserName() => (string) null;

  public void ReportTokenFetchFailure()
  {
    this.ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_LOGIN_FAILURE");
    ReconnectMgr.Get().FullResetRequired = true;
  }

  private class HSClientInterface : ClientInterface
  {
    private string s_tempCachePath = Application.temporaryCachePath;

    public string GetVersion() => Network.GetVersion();

    public string GetUserAgent()
    {
      string str1 = "Hearthstone/" + "25.0." + (object) 158725 + " (";
      string str2;
      switch (PlatformSettings.OS)
      {
        case OSCategory.PC:
          str2 = str1 + "PC;";
          break;
        case OSCategory.Mac:
          str2 = str1 + "Mac;";
          break;
        case OSCategory.iOS:
          str2 = str1 + "iOS;";
          break;
        case OSCategory.Android:
          str2 = str1 + "Android;";
          break;
        default:
          str2 = str1 + "UNKNOWN;";
          break;
      }
      object[] objArray = new object[73];
      objArray[0] = (object) str2;
      objArray[1] = (object) this.CleanUserAgentString(SystemInfo.deviceModel);
      objArray[2] = (object) ";";
      objArray[3] = (object) SystemInfo.deviceType;
      objArray[4] = (object) ";";
      objArray[5] = (object) this.CleanUserAgentString(SystemInfo.deviceUniqueIdentifier);
      objArray[6] = (object) ";";
      objArray[7] = (object) SystemInfo.graphicsDeviceID;
      objArray[8] = (object) ";";
      objArray[9] = (object) this.CleanUserAgentString(SystemInfo.graphicsDeviceName);
      objArray[10] = (object) ";";
      objArray[11] = (object) this.CleanUserAgentString(SystemInfo.graphicsDeviceVendor);
      objArray[12] = (object) ";";
      objArray[13] = (object) SystemInfo.graphicsDeviceVendorID;
      objArray[14] = (object) ";";
      objArray[15] = (object) this.CleanUserAgentString(SystemInfo.graphicsDeviceVersion);
      objArray[16] = (object) ";";
      objArray[17] = (object) SystemInfo.graphicsMemorySize;
      objArray[18] = (object) ";";
      objArray[19] = (object) SystemInfo.graphicsShaderLevel;
      objArray[20] = (object) ";";
      objArray[21] = (object) SystemInfo.npotSupport;
      objArray[22] = (object) ";";
      objArray[23] = (object) this.CleanUserAgentString(SystemInfo.operatingSystem);
      objArray[24] = (object) ";";
      objArray[25] = (object) SystemInfo.processorCount;
      objArray[26] = (object) ";";
      objArray[27] = (object) this.CleanUserAgentString(SystemInfo.processorType);
      objArray[28] = (object) ";";
      objArray[29] = (object) SystemInfo.supportedRenderTargetCount;
      objArray[30] = (object) ";";
      objArray[31] = (object) SystemInfo.supports3DTextures.ToString();
      objArray[32] = (object) ";";
      objArray[33] = (object) SystemInfo.supportsAccelerometer.ToString();
      objArray[34] = (object) ";";
      bool flag = SystemInfo.supportsComputeShaders;
      objArray[35] = (object) flag.ToString();
      objArray[36] = (object) ";";
      flag = SystemInfo.supportsGyroscope;
      objArray[37] = (object) flag.ToString();
      objArray[38] = (object) ";";
      flag = SystemInfo.supportsImageEffects;
      objArray[39] = (object) flag.ToString();
      objArray[40] = (object) ";";
      flag = SystemInfo.supportsInstancing;
      objArray[41] = (object) flag.ToString();
      objArray[42] = (object) ";";
      flag = SystemInfo.supportsLocationService;
      objArray[43] = (object) flag.ToString();
      objArray[44] = (object) ";";
      flag = SystemInfo.supportsRenderTextures;
      objArray[45] = (object) flag.ToString();
      objArray[46] = (object) ";";
      flag = SystemInfo.supportsRenderToCubemap;
      objArray[47] = (object) flag.ToString();
      objArray[48] = (object) ";";
      flag = SystemInfo.supportsShadows;
      objArray[49] = (object) flag.ToString();
      objArray[50] = (object) ";";
      flag = SystemInfo.supportsSparseTextures;
      objArray[51] = (object) flag.ToString();
      objArray[52] = (object) ";";
      objArray[53] = (object) SystemInfo.supportsStencil;
      objArray[54] = (object) ";";
      flag = SystemInfo.supportsVibration;
      objArray[55] = (object) flag.ToString();
      objArray[56] = (object) ";";
      objArray[57] = (object) SystemInfo.systemMemorySize;
      objArray[58] = (object) ";";
      flag = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
      objArray[59] = (object) flag.ToString();
      objArray[60] = (object) ";";
      flag = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB4444);
      objArray[61] = (object) flag.ToString();
      objArray[62] = (object) ";";
      flag = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth);
      objArray[63] = (object) flag.ToString();
      objArray[64] = (object) ";";
      flag = SystemInfo.graphicsDeviceVersion.StartsWith("Metal");
      objArray[65] = (object) flag.ToString();
      objArray[66] = (object) ";";
      objArray[67] = (object) Screen.currentResolution.width;
      objArray[68] = (object) ";";
      objArray[69] = (object) Screen.currentResolution.height;
      objArray[70] = (object) ";";
      objArray[71] = (object) Screen.dpi;
      objArray[72] = (object) ";";
      string str3 = string.Concat(objArray);
      string str4 = !PlatformSettings.IsMobile() ? str3 + "Desktop;" : (!(bool) UniversalInputManager.UsePhoneUI ? str3 + "Tablet;" : str3 + "Phone;");
      flag = Application.genuine;
      string str5 = flag.ToString();
      string format = str4 + str5 + ") Battle.net/CSharp";
      Log.Net.Print(format);
      return format;
    }

    public int GetApplicationVersion() => 158725;

    private string CleanUserAgentString(string data) => Regex.Replace(data, "[^a-zA-Z0-9_.]+", "_");

    public string GetBasePersistentDataPath() => PlatformFilePaths.PersistentDataPath;

    public string GetTemporaryCachePath() => this.s_tempCachePath;

    public bool GetDisableConnectionMetering() => Vars.Key("Aurora.DisableConnectionMetering").GetBool(false);

    public Blizzard.GameService.SDK.Client.Integration.MobileEnv GetMobileEnvironment() => HearthstoneApplication.GetMobileEnvironment() == MobileEnv.PRODUCTION ? Blizzard.GameService.SDK.Client.Integration.MobileEnv.PRODUCTION : Blizzard.GameService.SDK.Client.Integration.MobileEnv.DEVELOPMENT;

    public string GetAuroraVersionName() => 158725.ToString();

    public string GetLocaleName() => Localization.GetLocaleName();

    public string GetPlatformName() => "Win";

    public RuntimeEnvironment GetRuntimeEnvironment() => RuntimeEnvironment.Mono;

    public IUrlDownloader GetUrlDownloader() => (IUrlDownloader) Network.s_urlDownloader;

    public int GetDataVersion() => GameDbf.GetDataVersion();
  }

  public class ConnectErrorParams : ErrorParams
  {
    public float m_creationTime;

    public ConnectErrorParams() => this.m_creationTime = Time.realtimeSinceStartup;
  }

  private class RequestContext
  {
    public float m_waitUntil;
    public int m_pendingResponseId;
    public int m_requestId;
    public int m_requestSubId;
    public Network.TimeoutHandler m_timeoutHandler;

    public RequestContext(
      int pendingResponseId,
      int requestId,
      int requestSubId,
      Network.TimeoutHandler timeoutHandler)
    {
      this.m_waitUntil = Time.realtimeSinceStartup + Network.GetMaxDeferredWait();
      this.m_pendingResponseId = pendingResponseId;
      this.m_requestId = requestId;
      this.m_requestSubId = requestSubId;
      this.m_timeoutHandler = timeoutHandler;
    }
  }

  public class UnavailableReason
  {
    public string mainReason;
    public string subReason;
    public string extraData;
  }

  private class BnetErrorListener : EventListener<Network.BnetErrorCallback>
  {
    public bool Fire(BnetErrorInfo info) => this.m_callback(info, this.m_userData);
  }

  public delegate void NetHandler();

  public delegate void ThrottledPacketListener(int packetID, long retryMillis);

  public delegate void QueueInfoHandler(Network.QueueInfo queueInfo);

  public delegate void GameQueueHandler(QueueEvent queueEvent);

  public delegate void TimeoutHandler(int pendingResponseId, int requestId, int requestSubId);

  public delegate void BnetEventHandler(BnetEvent[] updates);

  public delegate void FriendsHandler(FriendsUpdate[] updates);

  public delegate void WhisperHandler(BnetWhisper[] whispers);

  public delegate void PresenceHandler(PresenceUpdate[] updates);

  public delegate void ShutdownHandler(int minutes);

  public delegate bool BnetErrorCallback(BnetErrorInfo info, object userData);

  public delegate void GameServerDisconnectEvent(BattleNetErrors errorCode);

  private struct NetworkState
  {
    public BattleNetLogSource LogSource { get; set; }

    public BnetGameType FindingBnetGameType { get; set; }

    public float LastCall { get; set; }

    public float LastCallReport { get; set; }

    public int LastCallFrame { get; set; }

    public Network.FriendsHandler CurrentFriendsHandler { get; set; }

    public Network.WhisperHandler CurrentWhisperHandler { get; set; }

    public Network.PresenceHandler CurrentPresenceHandler { get; set; }

    public Network.ShutdownHandler CurrentShutdownHandler { get; set; }

    public Map<BnetFeature, List<Network.BnetErrorListener>> FeatureBnetErrorListeners { get; set; }

    public List<Network.BnetErrorListener> GlobalBnetErrorListeners { get; set; }

    public Network.GameServerDisconnectEvent GameServerDisconnectEventListener { get; set; }

    public FindGameResult LastFindGameParameters { get; set; }

    public ConnectToGameServer LastConnectToGameServerInfo { get; set; }

    public GameServerInfo LastGameServerInfo { get; set; }

    public string DelayedError { get; set; }

    public float TimeBeforeAllowReset { get; set; }

    public List<ClientStateNotification> QueuedClientStateNotifications { get; set; }

    public BnetGameAccountId CachedGameAccountId { get; set; }

    public BnetRegion CachedRegion { get; set; }

    public int CurrentCreateDeckRequestId { get; set; }

    public HashSet<int> InTransitOfflineCreateDeckRequestIds { get; set; }

    public HashSet<long> DeckIdsWaitingToDiffAgainstOfflineCache { get; set; }

    public int CurrentCreateTeamRequestId { get; set; }

    public HashSet<int> InTransitOfflineCreateTeamRequestIds
    {
      set => this.\u003CInTransitOfflineCreateTeamRequestIds\u003Ek__BackingField = value;
    }

    public HashSet<long> TeamIdsWaitingToDiffAgainstOfflineCache
    {
      set => this.\u003CTeamIdsWaitingToDiffAgainstOfflineCache\u003Ek__BackingField = value;
    }

    public Map<int, Network.TimeoutHandler> NetTimeoutHandlers { get; set; }

    public void SetDefaults()
    {
      this.LogSource = new BattleNetLogSource(nameof (Network));
      this.FindingBnetGameType = BnetGameType.BGT_UNKNOWN;
      this.LastCall = Time.realtimeSinceStartup;
      this.LastCallReport = Time.realtimeSinceStartup;
      this.LastCallFrame = 0;
      this.FeatureBnetErrorListeners = new Map<BnetFeature, List<Network.BnetErrorListener>>();
      this.GlobalBnetErrorListeners = new List<Network.BnetErrorListener>();
      this.QueuedClientStateNotifications = new List<ClientStateNotification>();
      this.InTransitOfflineCreateDeckRequestIds = new HashSet<int>();
      this.DeckIdsWaitingToDiffAgainstOfflineCache = new HashSet<long>();
      this.InTransitOfflineCreateTeamRequestIds = new HashSet<int>();
      this.TeamIdsWaitingToDiffAgainstOfflineCache = new HashSet<long>();
      this.NetTimeoutHandlers = new Map<int, Network.TimeoutHandler>();
    }
  }

  public class QueueInfo
  {
    public int position;
    public long secondsTilEnd;
    public long stdev;
  }

  public class CanceledQuest
  {
    public CanceledQuest()
    {
      this.AchieveID = 0;
      this.Canceled = false;
      this.NextQuestCancelDate = 0L;
    }

    public int AchieveID { get; set; }

    public bool Canceled { get; set; }

    public long NextQuestCancelDate { get; set; }

    public override string ToString() => string.Format("[CanceledQuest AchieveID={0} Canceled={1} NextQuestCancelDate={2}]", (object) this.AchieveID, (object) this.Canceled, (object) this.NextQuestCancelDate);
  }

  public class TriggeredEvent
  {
    public TriggeredEvent()
    {
      this.EventID = 0;
      this.Success = false;
    }

    public int EventID
    {
      set => this.\u003CEventID\u003Ek__BackingField = value;
    }

    public bool Success
    {
      set => this.\u003CSuccess\u003Ek__BackingField = value;
    }
  }

  public class AdventureProgress
  {
    public AdventureProgress()
    {
      this.Wing = 0;
      this.Progress = 0;
      this.Ack = 0;
      this.Flags = 0UL;
    }

    public int Wing { get; set; }

    public int Progress { get; set; }

    public int Ack { get; set; }

    public ulong Flags { get; set; }
  }

  public class CardSaleResult
  {
    public Network.CardSaleResult.SaleResult Action { get; set; }

    public int AssetID { get; set; }

    public string AssetName { get; set; }

    public TAG_PREMIUM Premium { get; set; }

    public override string ToString() => string.Format("[CardSaleResult Action={0} assetName={1} premium={2}]", (object) this.Action, (object) this.AssetName, (object) this.Premium);

    public enum SaleResult
    {
      GENERIC_FAILURE = 1,
      CARD_WAS_SOLD = 2,
      CARD_WAS_BOUGHT = 3,
      SOULBOUND = 4,
      FAILED_WRONG_SELL_PRICE = 5,
      FAILED_WRONG_BUY_PRICE = 6,
      FAILED_NO_PERMISSION = 7,
      FAILED_EVENT_NOT_ACTIVE = 8,
      COUNT_MISMATCH = 9,
      CARD_WAS_UPGRADED = 10, // 0x0000000A
    }
  }

  public class BeginDraft
  {
    public BeginDraft() => this.Heroes = new List<NetCache.CardDefinition>();

    public long DeckID { get; set; }

    public List<NetCache.CardDefinition> Heroes { get; }

    public int Wins { get; set; }

    public int MaxSlot { get; set; }

    public ArenaSession Session { get; set; }

    public DraftSlotType SlotType { get; set; }

    public List<DraftSlotType> UniqueSlotTypesForDraft { get; set; }
  }

  public class DraftChoicesAndContents
  {
    public int Slot { get; set; }

    public List<NetCache.CardDefinition> Choices { get; }

    public NetCache.CardDefinition Hero { get; }

    public NetCache.CardDefinition HeroPower { get; }

    public Network.DeckContents DeckInfo { get; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public Network.RewardChest Chest { get; set; }

    public int MaxWins { get; set; }

    public int MaxSlot { get; set; }

    public ArenaSession Session { get; set; }

    public DraftSlotType SlotType { get; set; }

    public List<DraftSlotType> UniqueSlotTypesForDraft { get; }

    public DraftChoicesAndContents()
    {
      this.Choices = new List<NetCache.CardDefinition>();
      this.Hero = new NetCache.CardDefinition();
      this.HeroPower = new NetCache.CardDefinition();
      this.DeckInfo = new Network.DeckContents();
      this.Chest = (Network.RewardChest) null;
      this.UniqueSlotTypesForDraft = new List<DraftSlotType>();
    }
  }

  public class DraftChosen
  {
    public DraftChosen()
    {
      this.ChosenCard = new NetCache.CardDefinition();
      this.NextChoices = new List<NetCache.CardDefinition>();
    }

    public NetCache.CardDefinition ChosenCard { get; set; }

    public List<NetCache.CardDefinition> NextChoices { get; set; }

    public DraftSlotType SlotType { get; set; }
  }

  public class RewardChest
  {
    public RewardChest() => this.Rewards = new List<RewardData>();

    public List<RewardData> Rewards { get; }
  }

  public class DraftRetired
  {
    public DraftRetired()
    {
      this.Deck = 0L;
      this.Chest = new Network.RewardChest();
    }

    public long Deck { get; set; }

    public Network.RewardChest Chest { get; set; }
  }

  public class MassDisenchantResponse
  {
    public MassDisenchantResponse() => this.Amount = 0;

    public int Amount { get; set; }
  }

  public class SetFavoriteHeroResponse
  {
    public bool Success;
    public TAG_CLASS HeroClass;
    public NetCache.CardDefinition Hero;
    public bool IsFavorite;
  }

  public class PurchaseErrorInfo
  {
    public PurchaseErrorInfo()
    {
      this.Error = Network.PurchaseErrorInfo.ErrorType.UNKNOWN;
      this.PurchaseInProgressProductID = string.Empty;
      this.ErrorCode = string.Empty;
    }

    public Network.PurchaseErrorInfo.ErrorType Error { get; set; }

    public string PurchaseInProgressProductID
    {
      set => this.\u003CPurchaseInProgressProductID\u003Ek__BackingField = value;
    }

    public string ErrorCode { get; set; }

    public enum ErrorType
    {
      UNKNOWN = -1, // 0xFFFFFFFF
      SUCCESS = 0,
      STILL_IN_PROGRESS = 1,
      INVALID_BNET = 2,
      SERVICE_NA = 3,
      PURCHASE_IN_PROGRESS = 4,
      DATABASE = 5,
      INVALID_QUANTITY = 6,
      DUPLICATE_LICENSE = 7,
      REQUEST_NOT_SENT = 8,
      NO_ACTIVE_BPAY = 9,
      FAILED_RISK = 10, // 0x0000000A
      CANCELED = 11, // 0x0000000B
      WAIT_MOP = 12, // 0x0000000C
      WAIT_CONFIRM = 13, // 0x0000000D
      WAIT_RISK = 14, // 0x0000000E
      PRODUCT_NA = 15, // 0x0000000F
      RISK_TIMEOUT = 16, // 0x00000010
      PRODUCT_ALREADY_OWNED = 17, // 0x00000011
      WAIT_THIRD_PARTY_RECEIPT = 18, // 0x00000012
      PRODUCT_EVENT_HAS_ENDED = 19, // 0x00000013
      BP_GENERIC_FAIL = 100, // 0x00000064
      BP_INVALID_CC_EXPIRY = 101, // 0x00000065
      BP_RISK_ERROR = 102, // 0x00000066
      BP_NO_VALID_PAYMENT = 103, // 0x00000067
      BP_PAYMENT_AUTH = 104, // 0x00000068
      BP_PROVIDER_DENIED = 105, // 0x00000069
      BP_PURCHASE_BAN = 106, // 0x0000006A
      BP_SPENDING_LIMIT = 107, // 0x0000006B
      BP_PARENTAL_CONTROL = 108, // 0x0000006C
      BP_THROTTLED = 109, // 0x0000006D
      BP_THIRD_PARTY_BAD_RECEIPT = 110, // 0x0000006E
      BP_THIRD_PARTY_RECEIPT_USED = 111, // 0x0000006F
      BP_PRODUCT_UNIQUENESS_VIOLATED = 112, // 0x00000070
      BP_REGION_IS_DOWN = 113, // 0x00000071
      E_BP_GENERIC_FAIL_RETRY_CONTACT_CS_IF_PERSISTS = 115, // 0x00000073
      E_BP_CHALLENGE_ID_FAILED_VERIFICATION = 116, // 0x00000074
    }
  }

  public class PurchaseCanceledResponse
  {
    public Network.PurchaseCanceledResponse.CancelResult Result { get; set; }

    public long TransactionID { get; set; }

    public long? PMTProductID { get; set; }

    public string CurrencyCode { get; set; }

    public enum CancelResult
    {
      SUCCESS,
      NOT_ALLOWED,
      NOTHING_TO_CANCEL,
    }
  }

  public class BattlePayStatus
  {
    public BattlePayStatus()
    {
      this.State = Network.BattlePayStatus.PurchaseState.UNKNOWN;
      this.TransactionID = 0L;
      this.ThirdPartyID = string.Empty;
      this.PMTProductID = new long?();
      this.PurchaseError = new Network.PurchaseErrorInfo();
      this.BattlePayAvailable = false;
      this.Provider = MoneyOrGTAPPTransaction.UNKNOWN_PROVIDER;
    }

    public Network.BattlePayStatus.PurchaseState State { get; set; }

    public long TransactionID { get; set; }

    public string ThirdPartyID { get; set; }

    public long? PMTProductID { get; set; }

    public Network.PurchaseErrorInfo PurchaseError { get; set; }

    public bool BattlePayAvailable { get; set; }

    public string CurrencyCode { get; set; }

    public BattlePayProvider? Provider { get; set; }

    public enum PurchaseState
    {
      UNKNOWN = -1, // 0xFFFFFFFF
      READY = 0,
      CHECK_RESULTS = 1,
      ERROR = 2,
    }
  }

  public class BundleItem : Record
  {
    public BundleItem()
    {
      this.ItemType = ProductType.PRODUCT_TYPE_UNKNOWN;
      this.ProductData = 0;
      this.Quantity = 0;
      this.BaseQuantity = 0;
      this.Attributes = new AttributeSet();
    }

    public ProductType ItemType { get; set; }

    public int ProductData { get; set; }

    public int Quantity { get; set; }

    public int BaseQuantity { get; set; }

    public AttributeSet Attributes { get; set; }

    public bool IsBlocking { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return (object) this.ItemType;
      yield return (object) this.ProductData;
      yield return (object) this.Quantity;
      yield return (object) this.BaseQuantity;
      yield return (object) this.Attributes;
    }
  }

  public class Bundle : Record
  {
    public Bundle()
    {
      this.Items = new List<Network.BundleItem>();
      this.SaleIds = new List<int>();
    }

    public long? Cost { get; set; }

    public double? CostDisplay { get; set; }

    public long? GtappGoldCost { get; set; }

    public long? VirtualCurrencyCost { get; set; }

    public string VirtualCurrencyCode { get; set; }

    public List<Network.BundleItem> Items { get; set; }

    public string ProductEvent { get; set; }

    public bool IsPrePurchase { get; set; }

    public long? PMTProductID { get; set; }

    public DbfLocValue DisplayName { get; set; }

    public DbfLocValue DisplayDescription { get; set; }

    public AttributeSet Attributes { get; set; }

    public List<int> SaleIds { get; set; }

    public bool VisibleOnSalePeriodOnly { get; set; }

    public MobileShopType DisableRealMoneyShopFlags { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return (object) this.Cost;
      yield return (object) this.CostDisplay;
      foreach (object obj in (IEnumerable<Network.BundleItem>) this.Items.OrderBy<Network.BundleItem, ProductType>((Func<Network.BundleItem, ProductType>) (item => item.ItemType)).ThenBy<Network.BundleItem, int>((Func<Network.BundleItem, int>) (item => item.ProductData)))
        yield return obj;
      yield return (object) this.IsPrePurchase;
      yield return (object) this.PMTProductID;
      foreach (int saleId in this.SaleIds)
        yield return (object) saleId;
      yield return (object) this.VisibleOnSalePeriodOnly;
    }
  }

  public class ShopSection
  {
    public string InternalName { get; set; }

    public DbfLocValue Label { get; set; }

    public string Style { get; set; }

    public string FillerTags { get; set; }

    public int SortOrder { get; set; }

    public List<Network.ShopSection.ProductRef> Products { get; set; }

    public AttributeSet Attributes { get; set; }

    public class ProductRef
    {
      public long PmtId { get; set; }

      public int OrderId { get; set; }
    }
  }

  public class ShopSale : Record
  {
    public int SaleId { get; set; }

    public DateTime? StartUtc { get; set; }

    public DateTime? SoftEndUtc { get; set; }

    public DateTime? HardEndUtc { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return (object) this.SaleId;
      yield return (object) this.StartUtc;
      yield return (object) this.SoftEndUtc;
      yield return (object) this.HardEndUtc;
    }
  }

  public class GoldCostBooster
  {
    public GoldCostBooster()
    {
      this.Cost = new long?();
      this.ID = 0;
      this.BuyWithGoldEvent = SpecialEventType.UNKNOWN;
    }

    public long? Cost { get; set; }

    public int ID { get; set; }

    public SpecialEventType BuyWithGoldEvent { get; set; }
  }

  public class BattlePayConfig
  {
    public BattlePayConfig()
    {
      this.Available = false;
      this.Currencies = new List<Currency>();
      this.Bundles = new List<Network.Bundle>();
      this.GoldCostBoosters = new List<Network.GoldCostBooster>();
      this.GoldCostArena = new long?();
      this.SecondsBeforeAutoCancel = StoreManager.DEFAULT_SECONDS_BEFORE_AUTO_CANCEL;
      this.CommerceClientID = "df5787f96b2b46c49c66dd45bcb05490";
      this.PersonalizedShopPages = (List<BattlePayConfigShopPage>) null;
      this.CatalogLocaleToGameLocale = new Map<long, Locale>();
      this.SaleList = new List<Network.ShopSale>();
      this.IgnoreProductTiming = false;
      this.CheckoutKrOnestoreKey = (string) null;
    }

    public bool Available { get; set; }

    public Currency Currency { get; set; }

    public List<Currency> Currencies { get; }

    public List<Network.Bundle> Bundles { get; }

    public List<Network.GoldCostBooster> GoldCostBoosters { get; }

    public long? GoldCostArena { get; set; }

    public int SecondsBeforeAutoCancel { get; set; }

    public string CommerceClientID { get; set; }

    public List<BattlePayConfigShopPage> PersonalizedShopPages { get; set; }

    public Map<long, Locale> CatalogLocaleToGameLocale { get; }

    public List<Network.ShopSale> SaleList { get; set; }

    public bool IgnoreProductTiming { get; set; }

    public string CheckoutKrOnestoreKey { get; set; }
  }

  public class PurchaseViaGoldResponse
  {
    public PurchaseViaGoldResponse()
    {
      this.Error = Network.PurchaseViaGoldResponse.ErrorType.UNKNOWN;
      this.GoldUsed = 0L;
    }

    public Network.PurchaseViaGoldResponse.ErrorType Error { get; set; }

    public long GoldUsed { get; set; }

    public enum ErrorType
    {
      UNKNOWN = -1, // 0xFFFFFFFF
      SUCCESS = 1,
      INSUFFICIENT_GOLD = 2,
      PRODUCT_NA = 3,
      FEATURE_NA = 4,
      INVALID_QUANTITY = 5,
    }
  }

  public class PurchaseMethod
  {
    public PurchaseMethod()
    {
      this.TransactionID = 0L;
      this.PMTProductID = new long?();
      this.Quantity = 0;
      this.CurrencyCode = string.Empty;
      this.WalletName = string.Empty;
      this.UseEBalance = false;
      this.IsZeroCostLicense = false;
      this.ChallengeID = string.Empty;
      this.ChallengeURL = string.Empty;
      this.PurchaseError = (Network.PurchaseErrorInfo) null;
    }

    public long TransactionID { get; set; }

    public long? PMTProductID { get; set; }

    public int Quantity { get; set; }

    public string CurrencyCode { get; set; }

    public string WalletName { get; set; }

    public bool UseEBalance { get; set; }

    public bool IsZeroCostLicense { get; set; }

    public string ChallengeID { get; set; }

    public string ChallengeURL { get; set; }

    public Network.PurchaseErrorInfo PurchaseError { get; set; }
  }

  public class PurchaseResponse
  {
    public PurchaseResponse()
    {
      this.PurchaseError = new Network.PurchaseErrorInfo();
      this.TransactionID = 0L;
      this.PMTProductID = new long?();
      this.ThirdPartyID = string.Empty;
    }

    public Network.PurchaseErrorInfo PurchaseError { get; set; }

    public long TransactionID { get; set; }

    public long? PMTProductID { get; set; }

    public string ThirdPartyID { get; set; }

    public string CurrencyCode { get; set; }
  }

  public class CardBackResponse
  {
    public CardBackResponse()
    {
      this.Success = false;
      this.CardBack = 0;
    }

    public bool Success { get; set; }

    public bool IsFavorite { get; set; }

    public int CardBack { get; set; }
  }

  public class CoinResponse
  {
    public CoinResponse()
    {
      this.Success = false;
      this.Coin = 1;
    }

    public bool Success { get; set; }

    public int Coin { get; set; }
  }

  public class GameCancelInfo
  {
    public Network.GameCancelInfo.Reason CancelReason { get; set; }

    public enum Reason
    {
      OPPONENT_TIMEOUT = 1,
      PLAYER_LOADING_TIMEOUT = 2,
      PLAYER_LOADING_DISCONNECTED = 3,
    }
  }

  public class Entity
  {
    public Entity()
    {
      this.Tags = new List<Network.Entity.Tag>();
      this.DefTags = new List<Network.Entity.Tag>();
    }

    public int ID { get; set; }

    public List<Network.Entity.Tag> Tags { get; set; }

    public List<Network.Entity.Tag> DefTags { get; set; }

    public string CardID { get; set; }

    public static Network.Entity CreateFromProto(PegasusGame.Entity src) => new Network.Entity()
    {
      ID = src.Id,
      CardID = string.Empty,
      Tags = Network.Entity.CreateTagsFromProto((IList<PegasusGame.Tag>) src.Tags)
    };

    public static Network.Entity CreateFromProto(PowerHistoryEntity src) => new Network.Entity()
    {
      ID = src.Entity,
      CardID = src.Name,
      Tags = Network.Entity.CreateTagsFromProto((IList<PegasusGame.Tag>) src.Tags),
      DefTags = Network.Entity.CreateTagsFromProto((IList<PegasusGame.Tag>) src.DefTags)
    };

    public static List<Network.Entity.Tag> CreateTagsFromProto(IList<PegasusGame.Tag> tagList)
    {
      List<Network.Entity.Tag> tagsFromProto = new List<Network.Entity.Tag>();
      for (int index = 0; index < tagList.Count; ++index)
      {
        PegasusGame.Tag tag = tagList[index];
        tagsFromProto.Add(new Network.Entity.Tag()
        {
          Name = tag.Name,
          Value = tag.Value
        });
      }
      return tagsFromProto;
    }

    public override string ToString() => string.Format("id={0} cardId={1} tags={2}", (object) this.ID, (object) this.CardID, (object) this.Tags.Count);

    public class Tag
    {
      public int Name { get; set; }

      public int Value { get; set; }
    }
  }

  public class Options
  {
    public Options() => this.List = new List<Network.Options.Option>();

    public int ID { get; set; }

    public List<Network.Options.Option> List { get; set; }

    public bool HasValidOption()
    {
      foreach (Network.Options.Option option in this.List)
      {
        if (option.Main.PlayErrorInfo.IsValid())
          return true;
      }
      return false;
    }

    public Network.Options.Option GetOptionFromEntityID(int entityID, bool wantTradeOption = false)
    {
      for (int index = 0; index < this.List.Count; ++index)
      {
        bool flag = this.List[index].Main.IsTradeOption();
        if (this.List[index].Main.ID == entityID && flag == wantTradeOption)
          return this.List[index];
      }
      return (Network.Options.Option) null;
    }

    public void CopyFrom(Network.Options options)
    {
      this.ID = options.ID;
      if (options.List == null)
      {
        this.List = (List<Network.Options.Option>) null;
      }
      else
      {
        if (this.List != null)
          this.List.Clear();
        else
          this.List = new List<Network.Options.Option>();
        for (int index = 0; index < options.List.Count; ++index)
        {
          Network.Options.Option option = new Network.Options.Option();
          option.CopyFrom(options.List[index]);
          this.List.Add(option);
        }
      }
    }

    public class Option
    {
      public Option()
      {
        this.Main = new Network.Options.Option.SubOption();
        this.Subs = new List<Network.Options.Option.SubOption>();
      }

      public Network.Options.Option.OptionType Type { get; set; }

      public Network.Options.Option.SubOption Main { get; set; }

      public List<Network.Options.Option.SubOption> Subs { get; set; }

      public Network.Options.Option.SubOption GetSubOptionFromEntityID(int entityID)
      {
        foreach (Network.Options.Option.SubOption sub in this.Subs)
        {
          if (sub.ID == entityID)
            return sub;
        }
        return (Network.Options.Option.SubOption) null;
      }

      public bool HasValidSubOption()
      {
        foreach (Network.Options.Option.SubOption sub in this.Subs)
        {
          if (sub.PlayErrorInfo.IsValid())
            return true;
        }
        return false;
      }

      public void CopyFrom(Network.Options.Option option)
      {
        this.Type = option.Type;
        if (this.Main == null)
          this.Main = new Network.Options.Option.SubOption();
        this.Main.CopyFrom(option.Main);
        if (option.Subs == null)
        {
          this.Subs = (List<Network.Options.Option.SubOption>) null;
        }
        else
        {
          if (this.Subs == null)
            this.Subs = new List<Network.Options.Option.SubOption>();
          else
            this.Subs.Clear();
          for (int index = 0; index < option.Subs.Count; ++index)
          {
            Network.Options.Option.SubOption subOption = new Network.Options.Option.SubOption();
            subOption.CopyFrom(option.Subs[index]);
            this.Subs.Add(subOption);
          }
        }
      }

      public enum OptionType
      {
        PASS = 1,
        END_TURN = 2,
        POWER = 3,
      }

      public class PlayErrorInfo
      {
        public PlayErrorInfo()
        {
          this.PlayError = PlayErrors.ErrorType.INVALID;
          this.PlayErrorParam = new int?();
        }

        public PlayErrors.ErrorType PlayError { get; set; }

        public int? PlayErrorParam { get; set; }

        public bool IsValid() => this.PlayError == PlayErrors.ErrorType.NONE;
      }

      public class TargetOption
      {
        public TargetOption()
        {
          this.ID = 0;
          this.PlayErrorInfo = new Network.Options.Option.PlayErrorInfo();
        }

        public int ID { get; set; }

        public Network.Options.Option.PlayErrorInfo PlayErrorInfo { get; set; }

        public void CopyFrom(Network.Options.Option.TargetOption targetOption)
        {
          this.ID = targetOption.ID;
          this.PlayErrorInfo = targetOption.PlayErrorInfo;
        }

        public void CopyFrom(PegasusGame.TargetOption targetOption)
        {
          this.ID = targetOption.Id;
          this.PlayErrorInfo.PlayError = (PlayErrors.ErrorType) targetOption.PlayError;
          this.PlayErrorInfo.PlayErrorParam = targetOption.HasPlayErrorParam ? new int?(targetOption.PlayErrorParam) : new int?();
        }
      }

      public class SubOption
      {
        public SubOption()
        {
          this.ID = 0;
          this.PlayErrorInfo = new Network.Options.Option.PlayErrorInfo();
        }

        public int ID { get; set; }

        public List<Network.Options.Option.TargetOption> Targets { get; set; }

        public Network.Options.Option.PlayErrorInfo PlayErrorInfo { get; set; }

        public bool IsTradeOption() => this.IsValidTarget(this.ID) && this.Targets.Count == 1;

        public bool IsValidTarget(int entityID)
        {
          if (this.Targets == null)
            return false;
          foreach (Network.Options.Option.TargetOption target in this.Targets)
          {
            if (target.ID == entityID && target.PlayErrorInfo.IsValid())
              return true;
          }
          return false;
        }

        public PlayErrors.ErrorType GetErrorForTarget(int entityID)
        {
          if (this.Targets == null)
            return PlayErrors.ErrorType.INVALID;
          foreach (Network.Options.Option.TargetOption target in this.Targets)
          {
            if (target.ID == entityID)
              return target.PlayErrorInfo.PlayError;
          }
          return PlayErrors.ErrorType.INVALID;
        }

        public int? GetErrorParamForTarget(int entityID)
        {
          if (this.Targets == null)
            return new int?();
          foreach (Network.Options.Option.TargetOption target in this.Targets)
          {
            if (target.ID == entityID)
              return target.PlayErrorInfo.PlayErrorParam;
          }
          return new int?();
        }

        public bool HasValidTarget()
        {
          if (this.Targets == null)
            return false;
          foreach (Network.Options.Option.TargetOption target in this.Targets)
          {
            if (target.PlayErrorInfo.IsValid())
              return true;
          }
          return false;
        }

        public void CopyFrom(Network.Options.Option.SubOption subOption)
        {
          this.ID = subOption.ID;
          this.PlayErrorInfo = subOption.PlayErrorInfo;
          if (subOption.Targets == null)
          {
            this.Targets = (List<Network.Options.Option.TargetOption>) null;
          }
          else
          {
            if (this.Targets == null)
              this.Targets = new List<Network.Options.Option.TargetOption>();
            else
              this.Targets.Clear();
            for (int index = 0; index < subOption.Targets.Count; ++index)
            {
              Network.Options.Option.TargetOption targetOption = new Network.Options.Option.TargetOption();
              targetOption.CopyFrom(subOption.Targets[index]);
              this.Targets.Add(targetOption);
            }
          }
        }
      }
    }
  }

  public class EntityChoices
  {
    public int ID { get; set; }

    public CHOICE_TYPE ChoiceType { get; set; }

    public int CountMin { get; set; }

    public int CountMax { get; set; }

    public List<int> Entities { get; set; }

    public int Source { get; set; }

    public int PlayerId { get; set; }

    public bool HideChosen { get; set; }

    public bool IsSingleChoice()
    {
      if (this.CountMax == 0)
        return true;
      return this.CountMin == 1 && this.CountMax == 1;
    }
  }

  public class EntitiesChosen
  {
    public int ID { get; set; }

    public List<int> Entities { get; set; }

    public int PlayerId { get; set; }

    public CHOICE_TYPE ChoiceType { get; set; }
  }

  public class GameSetup
  {
    public int Board { get; set; }

    public int BoardLayout { get; set; }

    public int BaconFavoriteBoardSkin { get; set; }

    public int MaxSecretZoneSizePerPlayer { get; set; }

    public int MaxSecretsPerPlayer { get; set; }

    public int MaxQuestsPerPlayer { get; set; }

    public int MaxFriendlyMinionsPerPlayer { get; set; }

    public uint DisconnectWhenStuckSeconds { get; set; }
  }

  public class UserUI
  {
    public Network.UserUI.MouseInfo mouseInfo;
    public Network.UserUI.EmoteInfo emoteInfo;
    public Network.UserUI.SelectionInfo selectionInfo;
    public int? playerId;

    public class MouseInfo
    {
      public int OverCardID { get; set; }

      public int HeldCardID { get; set; }

      public int ArrowOriginID { get; set; }

      public int X
      {
        set => this.\u003CX\u003Ek__BackingField = value;
      }

      public int Y
      {
        set => this.\u003CY\u003Ek__BackingField = value;
      }
    }

    public class EmoteInfo
    {
      public int Emote { get; set; }

      public int BattlegroundsEmoteId { get; set; }
    }

    public class SelectionInfo
    {
      public int SelectedEntityID { get; set; }
    }
  }

  public enum PowerType
  {
    FULL_ENTITY = 1,
    SHOW_ENTITY = 2,
    HIDE_ENTITY = 3,
    TAG_CHANGE = 4,
    BLOCK_START = 5,
    BLOCK_END = 6,
    CREATE_GAME = 7,
    META_DATA = 8,
    CHANGE_ENTITY = 9,
    RESET_GAME = 10, // 0x0000000A
    SUB_SPELL_START = 11, // 0x0000000B
    SUB_SPELL_END = 12, // 0x0000000C
    VO_SPELL = 13, // 0x0000000D
    CACHED_TAG_FOR_DORMANT_CHANGE = 14, // 0x0000000E
    SHUFFLE_DECK = 15, // 0x0000000F
    VO_BANTER = 16, // 0x00000010
  }

  public class PowerHistory
  {
    public PowerHistory(Network.PowerType init) => this.Type = init;

    public Network.PowerType Type { get; }

    public override string ToString() => string.Format("type={0}", (object) this.Type);
  }

  public class HistBlockStart : Network.PowerHistory
  {
    public HistBlockStart(HistoryBlock.Type type)
      : base(Network.PowerType.BLOCK_START)
    {
      this.BlockType = type;
    }

    public override string ToString() => string.Format("type={0} blockType={1} entity={2} target={3} b={4} d={5} xd={6} bigCard={7}", (object) this.Type, (object) this.BlockType, (object) this.Entities, (object) this.Target, (object) this.IsBatchable, (object) this.IsDeferrable, (object) this.IsDeferBlocker, (object) this.ForceShowBigCard);

    public HistoryBlock.Type BlockType { get; set; }

    public List<int> Entities { get; set; }

    public int Target { get; set; }

    public int SubOption { get; set; }

    public List<string> EffectCardId { get; set; }

    public List<bool> IsEffectCardIdClientCached { get; set; }

    public int EffectIndex { get; set; }

    public int TriggerKeyword { get; set; }

    public bool ShowInHistory { get; set; }

    public bool IsDeferrable { get; set; }

    public bool IsBatchable { get; set; }

    public bool IsDeferBlocker { get; set; }

    public bool ForceShowBigCard { get; set; }
  }

  public class HistBlockEnd : Network.PowerHistory
  {
    public HistBlockEnd()
      : base(Network.PowerType.BLOCK_END)
    {
    }
  }

  public class HistCreateGame : Network.PowerHistory
  {
    public static Network.HistCreateGame CreateFromProto(PowerHistoryCreateGame src)
    {
      Network.HistCreateGame fromProto1 = new Network.HistCreateGame()
      {
        Uuid = src.GameUuid,
        Game = Network.Entity.CreateFromProto(src.GameEntity)
      };
      if (src.Players != null)
      {
        fromProto1.Players = new List<Network.HistCreateGame.PlayerData>();
        foreach (PegasusGame.Player player in src.Players)
        {
          Network.HistCreateGame.PlayerData fromProto2 = Network.HistCreateGame.PlayerData.CreateFromProto(player);
          fromProto1.Players.Add(fromProto2);
        }
      }
      if (src.PlayerInfos == null)
        return fromProto1;
      fromProto1.PlayerInfos = new List<Network.HistCreateGame.SharedPlayerInfo>();
      foreach (PegasusGame.SharedPlayerInfo playerInfo in src.PlayerInfos)
      {
        Network.HistCreateGame.SharedPlayerInfo fromProto3 = Network.HistCreateGame.SharedPlayerInfo.CreateFromProto(playerInfo);
        fromProto1.PlayerInfos.Add(fromProto3);
      }
      if (src.ActionInfos != null)
      {
        fromProto1.ActionInfos = new List<Network.HistCreateGame.ActionInfo>();
        for (int index = 0; index < src.ActionInfos.Count; ++index)
        {
          Network.HistCreateGame.ActionInfo fromProto4 = Network.HistCreateGame.ActionInfo.CreateFromProto(src.ActionInfos[index]);
          fromProto1.ActionInfos.Add(fromProto4);
        }
      }
      return fromProto1;
    }

    public HistCreateGame()
      : base(Network.PowerType.CREATE_GAME)
    {
    }

    public Network.Entity Game { get; set; }

    public string Uuid { get; set; }

    public List<Network.HistCreateGame.PlayerData> Players { get; set; }

    public List<Network.HistCreateGame.SharedPlayerInfo> PlayerInfos { get; set; }

    public List<Network.HistCreateGame.ActionInfo> ActionInfos { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("game={0}", (object) this.Game);
      if (this.Players == null)
        stringBuilder.Append(" players=(null)");
      else if (this.Players.Count == 0)
      {
        stringBuilder.Append(" players=0");
      }
      else
      {
        for (int index = 0; index < this.Players.Count; ++index)
          stringBuilder.AppendFormat(" players[{0}]=[{1}]", (object) index, (object) this.Players[index]);
      }
      if (this.PlayerInfos == null)
        stringBuilder.Append(" playerInfos=(null)");
      else if (this.PlayerInfos.Count == 0)
      {
        stringBuilder.Append(" playerInfos=0");
      }
      else
      {
        for (int index = 0; index < this.PlayerInfos.Count; ++index)
          stringBuilder.AppendFormat(" playerInfos[{0}]=[{1}]", (object) index, (object) this.PlayerInfos[index]);
      }
      if (this.ActionInfos == null)
      {
        stringBuilder.Append(" ActionInfos=(null)");
      }
      else
      {
        for (int index = 0; index < this.ActionInfos.Count; ++index)
          stringBuilder.AppendFormat(" ActionInfos[{0}]=[{1}]", (object) index, (object) this.ActionInfos[index]);
      }
      return stringBuilder.ToString();
    }

    public class PlayerData
    {
      public int ID { get; set; }

      public BnetGameAccountId GameAccountId { get; set; }

      public Network.Entity Player { get; set; }

      public int CardBackID { get; set; }

      public static Network.HistCreateGame.PlayerData CreateFromProto(PegasusGame.Player src) => new Network.HistCreateGame.PlayerData()
      {
        ID = src.Id,
        GameAccountId = BnetGameAccountId.CreateFromNet(src.GameAccountId),
        Player = Network.Entity.CreateFromProto(src.Entity),
        CardBackID = src.CardBack
      };

      public override string ToString() => string.Format("ID={0} GameAccountId={1} Player={2} CardBackID={3}", (object) this.ID, (object) this.GameAccountId, (object) this.Player, (object) this.CardBackID);
    }

    public class SharedPlayerInfo
    {
      public int ID { get; set; }

      public BnetGameAccountId GameAccountId { get; set; }

      public static Network.HistCreateGame.SharedPlayerInfo CreateFromProto(
        PegasusGame.SharedPlayerInfo src)
      {
        return new Network.HistCreateGame.SharedPlayerInfo()
        {
          ID = src.Id,
          GameAccountId = BnetGameAccountId.CreateFromNet(src.GameAccountId)
        };
      }

      public override string ToString() => string.Format("ID={0} GameAccountId={1}", (object) this.ID, (object) this.GameAccountId);
    }

    public class ActionInfo
    {
      public int PlayerID { get; set; }

      public int SelectedEntityID { get; set; }

      public static Network.HistCreateGame.ActionInfo CreateFromProto(PegasusGame.ActionInfo src) => new Network.HistCreateGame.ActionInfo()
      {
        PlayerID = src.PlayerId,
        SelectedEntityID = src.SelectedEntityId
      };

      public override string ToString() => string.Format("PlayerID={0} SelectedEntityID={1}", (object) this.PlayerID, (object) this.SelectedEntityID);
    }
  }

  public class HistResetGame : Network.PowerHistory
  {
    public HistResetGame()
      : base(Network.PowerType.RESET_GAME)
    {
    }

    public Network.HistCreateGame CreateGame { get; set; }

    public override string ToString() => string.Format("type={0}", (object) this.Type);

    public static Network.HistResetGame CreateFromProto(PowerHistoryResetGame proto) => new Network.HistResetGame()
    {
      CreateGame = Network.HistCreateGame.CreateFromProto(proto.CreateGame)
    };
  }

  public class HistFullEntity : Network.PowerHistory
  {
    public HistFullEntity()
      : base(Network.PowerType.FULL_ENTITY)
    {
    }

    public Network.Entity Entity { get; set; }

    public override string ToString() => string.Format("type={0} entity=[{1}]", (object) this.Type, (object) this.Entity);
  }

  public class HistShowEntity : Network.PowerHistory
  {
    public HistShowEntity()
      : base(Network.PowerType.SHOW_ENTITY)
    {
    }

    public Network.Entity Entity { get; set; }

    public override string ToString() => string.Format("type={0} entity=[{1}]", (object) this.Type, (object) this.Entity);
  }

  public class HistHideEntity : Network.PowerHistory
  {
    public HistHideEntity()
      : base(Network.PowerType.HIDE_ENTITY)
    {
    }

    public int Entity { get; set; }

    public int Zone { get; set; }

    public override string ToString() => string.Format("type={0} entity={1} zone={2}", (object) this.Type, (object) this.Entity, (object) this.Zone);
  }

  public class HistChangeEntity : Network.PowerHistory
  {
    public HistChangeEntity()
      : base(Network.PowerType.CHANGE_ENTITY)
    {
    }

    public Network.Entity Entity { get; set; }

    public override string ToString() => string.Format("type={0} entity=[{1}]", (object) this.Type, (object) this.Entity);
  }

  public class HistTagChange : Network.PowerHistory
  {
    public HistTagChange()
      : base(Network.PowerType.TAG_CHANGE)
    {
    }

    public int Entity { get; set; }

    public int Tag { get; set; }

    public int Value { get; set; }

    public bool ChangeDef { get; set; }

    public override string ToString() => string.Format("type={0} entity={1} tag={2} value={3}", (object) this.Type, (object) this.Entity, (object) this.Tag, (object) this.Value);
  }

  public class HistMetaData : Network.PowerHistory
  {
    public HistMetaData()
      : base(Network.PowerType.META_DATA)
    {
      this.Info = new List<int>();
      this.AdditionalData = new List<int>();
    }

    public HistoryMeta.Type MetaType { get; set; }

    public List<int> Info { get; }

    public int Data { get; set; }

    public List<int> AdditionalData { get; }

    public override string ToString() => string.Format("type={0} metaType={1} infoCount={2} data={3}", (object) this.Type, (object) this.MetaType, (object) this.Info.Count, (object) this.Data);
  }

  public class HistSubSpellStart : Network.PowerHistory
  {
    public HistSubSpellStart()
      : base(Network.PowerType.SUB_SPELL_START)
    {
    }

    public static Network.HistSubSpellStart CreateFromProto(PowerHistorySubSpellStart proto) => new Network.HistSubSpellStart()
    {
      SpellPrefabGUID = proto.SpellPrefabGuid,
      SourceEntityID = proto.HasSourceEntityId ? proto.SourceEntityId : 0,
      TargetEntityIDS = proto.TargetEntityIds
    };

    public string SpellPrefabGUID { get; set; }

    public int SourceEntityID { get; set; }

    public List<int> TargetEntityIDS { get; set; }
  }

  public class HistSubSpellEnd : Network.PowerHistory
  {
    public HistSubSpellEnd()
      : base(Network.PowerType.SUB_SPELL_END)
    {
    }
  }

  public class HistVoSpell : Network.PowerHistory
  {
    public HistVoSpell()
      : base(Network.PowerType.VO_SPELL)
    {
    }

    public static Network.HistVoSpell CreateFromProto(PowerHistoryVoTask proto) => new Network.HistVoSpell()
    {
      SpellPrefabGUID = proto.SpellPrefabGuid,
      Speaker = proto.SpeakingEntity,
      Blocking = proto.Blocking,
      AdditionalDelayMs = proto.AdditionalDelayMs,
      BrassRingGUID = proto.BrassRingPrefabGuid
    };

    public string SpellPrefabGUID { get; set; }

    public int Speaker { get; set; }

    public bool Blocking { get; set; }

    public int AdditionalDelayMs { get; set; }

    public string BrassRingGUID { get; set; }

    public AudioSource m_audioSource { get; set; }

    public bool m_ableToLoad { get; set; }
  }

  public class HistVoBanter : Network.PowerHistory
  {
    public HistVoBanter()
      : base(Network.PowerType.VO_BANTER)
    {
    }

    public static Network.HistVoBanter CreateFromProto(PowerHistoryVoBanter proto)
    {
      Network.HistVoBanter fromProto = new Network.HistVoBanter();
      fromProto.EmoteEvent = proto.EmoteEvent;
      if (proto.Teams.Count > 0)
        fromProto.Teams = proto.Teams;
      if (proto.HasSpeaker)
        fromProto.Speaker = proto.Speaker;
      return fromProto;
    }

    public PowerHistoryVoBanter.ClientEmoteEvent EmoteEvent { get; private set; }

    public List<int> Teams { get; private set; } = new List<int>();

    public int Speaker { get; private set; }
  }

  public class HistCachedTagForDormantChange : Network.PowerHistory
  {
    public HistCachedTagForDormantChange()
      : base(Network.PowerType.CACHED_TAG_FOR_DORMANT_CHANGE)
    {
    }

    public static Network.HistCachedTagForDormantChange CreateFromProto(
      PowerHistoryCachedTagForDormantChange proto)
    {
      return new Network.HistCachedTagForDormantChange()
      {
        Entity = proto.Entity,
        Tag = proto.Tag,
        Value = proto.Value
      };
    }

    public int Entity { get; set; }

    public int Tag { get; set; }

    public int Value { get; set; }

    public override string ToString() => string.Format("type={0} entity={1} tag={2} value={3}", (object) this.Type, (object) this.Entity, (object) this.Tag, (object) this.Value);
  }

  public class HistShuffleDeck : Network.PowerHistory
  {
    public HistShuffleDeck()
      : base(Network.PowerType.SHUFFLE_DECK)
    {
    }

    public static Network.HistShuffleDeck CreateFromProto(PowerHistoryShuffleDeck proto) => new Network.HistShuffleDeck()
    {
      PlayerID = proto.PlayerId
    };

    public int PlayerID { get; set; }

    public override string ToString() => string.Format("type={0} player_id={1}", (object) this.Type, (object) this.PlayerID);
  }

  public class CardUserData
  {
    public int DbId { get; set; }

    public int Count { get; set; }

    public TAG_PREMIUM Premium { get; set; }
  }

  public class DeckContents
  {
    public DeckContents() => this.Cards = new List<Network.CardUserData>();

    public long Deck { get; set; }

    public List<Network.CardUserData> Cards { get; }

    public static Network.DeckContents FromPacket(PegasusUtil.DeckContents packet)
    {
      Network.DeckContents deckContents = new Network.DeckContents()
      {
        Deck = packet.DeckId
      };
      foreach (DeckCardData card in packet.Cards)
      {
        Network.CardUserData cardUserData = new Network.CardUserData()
        {
          DbId = card.Def.Asset,
          Count = card.HasQty ? card.Qty : 1,
          Premium = card.Def.HasPremium ? (TAG_PREMIUM) card.Def.Premium : TAG_PREMIUM.NORMAL
        };
        deckContents.Cards.Add(cardUserData);
      }
      return deckContents;
    }
  }

  public class DeckName
  {
    public long Deck { get; set; }

    public string Name { get; set; }
  }

  public class GenericResponse
  {
    public int RequestId { get; set; }

    public int RequestSubId { get; set; }

    public Network.GenericResponse.Result ResultCode { get; set; }

    public object GenericData { get; set; }

    public enum Result
    {
      RESULT_OK = 0,
      RESULT_REQUEST_IN_PROCESS = 1,
      RESULT_REQUEST_COMPLETE = 2,
      RESULT_UNKNOWN_ERROR = 100, // 0x00000064
      RESULT_INTERNAL_ERROR = 101, // 0x00000065
      RESULT_DB_ERROR = 102, // 0x00000066
      RESULT_INVALID_REQUEST = 103, // 0x00000067
      RESULT_LOGIN_LOAD = 104, // 0x00000068
      RESULT_DATA_MIGRATION_OR_PLAYER_ID_ERROR = 105, // 0x00000069
      RESULT_INTERNAL_RPC_ERROR = 106, // 0x0000006A
      RESULT_DATA_MIGRATION_REQUIRED = 107, // 0x0000006B
    }
  }

  public class DBAction
  {
    public Network.DBAction.ActionType Action { get; set; }

    public Network.DBAction.ResultType Result { get; set; }

    public long MetaData { get; set; }

    public enum ActionType
    {
      UNKNOWN,
      GET_DECK,
      CREATE_DECK,
      RENAME_DECK,
      DELETE_DECK,
      SET_DECK,
      OPEN_BOOSTER,
      GAMES_INFO,
    }

    public enum ResultType
    {
      UNKNOWN,
      SUCCESS,
      NOT_OWNED,
      CONSTRAINT,
    }
  }

  public class TurnTimerInfo
  {
    public float Seconds { get; set; }

    public int Turn { get; set; }

    public bool Show { get; set; }
  }

  public class GameEnd
  {
    public GameEnd() => this.Notices = new List<NetCache.ProfileNotice>();

    public List<NetCache.ProfileNotice> Notices { get; }
  }

  public class AccountLicenseAchieveResponse
  {
    public int Achieve { get; set; }

    public Network.AccountLicenseAchieveResponse.AchieveResult Result { get; set; }

    public enum AchieveResult
    {
      INVALID_ACHIEVE = 1,
      NOT_ACTIVE = 2,
      IN_PROGRESS = 3,
      COMPLETE = 4,
      STATUS_UNKNOWN = 5,
    }
  }

  public class DebugConsoleResponse
  {
    public DebugConsoleResponse() => this.Response = "";

    public int Type { get; set; }

    public string Response { get; set; }
  }
}
