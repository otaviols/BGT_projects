using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using PegasusClient;
using PegasusFSG;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class FiresideGatheringManager : IService, IHasUpdate, IHasFixedUpdate
{
  public const long INVALID_FSG_ID = 0;
  private FSGConfig m_currentFSG;
  private byte[] m_currentFSGSharedSecretKey;
  private HashSet<int> m_innkeeperSelectedBrawlLibraryItemIds = new HashSet<int>();
  private List<FSGConfig> m_nearbyFSGs = new List<FSGConfig>();
  private HashSet<BnetPlayer> m_knownPatronsFromServer = new HashSet<BnetPlayer>();
  private HashSet<BnetPlayer> m_knownPatronsFromPresence = new HashSet<BnetPlayer>();
  private HashSet<BnetPlayer> m_displayablePatrons = new HashSet<BnetPlayer>();
  private HashSet<BnetPlayer> m_pendingPatrons = new HashSet<BnetPlayer>();
  private bool m_isAppendingPatronList;
  private Map<long, BnetPlayer> m_innkeepers = new Map<long, BnetPlayer>();
  private bool m_checkInRequestPending;
  private bool m_checkInDialogShown;
  private bool m_nearbyFSGsFoundEventSent;
  private Notification m_tooltipShowing;
  private ClientLocationData m_locationData;
  private Map<string, AccessPointInfo> m_accumulatedAccessPoints = new Map<string, AccessPointInfo>();
  private bool m_hasBegunLocationDataGatheringForLogin;
  private FiresideGatheringSign m_currentSign;
  private Transform m_smallSignContainer;
  private FiresideGatheringManager.OnCloseSign m_currentSignCallback;
  private bool m_haltFSGNotificationsAndCheckins;
  private bool m_fsgSignShown;
  private bool m_doAutoInnkeeperSetup = true;
  private bool m_haltAutoCheckinWhileInnkeeperSetup;
  private bool m_errorOccuredOnCheckin;
  private bool m_waitingForCheckIn;
  private FSGConfig m_innkeeperFSG;
  private bool m_fsgAvailableToCheckin;
  private bool m_isRequestNearbyFSGsPending;
  private double m_gpsCheatOffset;
  private static bool m_isEnabledByPrivacySettings = false;
  private static bool s_cacheFSGEnabled = false;
  private static bool s_cacheGPSEnabled = false;
  private static bool s_cacheWifiEnabled = false;
  private bool m_gpsCheatingLocation;
  private double m_gpsCheatLatitude;
  private double m_gpsCheatLongitude;
  private FSGConfig m_cachedFakeCheatFsg;
  private const string BACKGROUND_TEXTURE_SHADER_VAL = "_BackgroundTex";
  private const string MAJOR_TEXTURE_SHADER_VAL = "_MajorTex";
  private const string MINOR_TEXTURE_SHADER_VAL = "_MinorTex";
  private static readonly AssetReference m_tavernSignAsset = new AssetReference("FSG_TavernSign.prefab:8ce9cae2230ceda45a5f20996b704a9b");
  private GameObject m_sceneObject;
  private static readonly AssetReference[] m_fsgShields = new AssetReference[8]
  {
    new AssetReference("shield_01.prefab:78363d95f6d2de34fbc560266fea640d"),
    new AssetReference("shield_02.prefab:c377b1e43c7e56940b5976606e4c204d"),
    new AssetReference("shield_03.prefab:1aff0388b4ec9a541914c6001d89a1a4"),
    new AssetReference("shield_04.prefab:b8c72df88e9b2a346be4921349e95d69"),
    new AssetReference("shield_05.prefab:8b4ff22e9b7e20a44afdce7d38c71179"),
    new AssetReference("shield_06.prefab:e0adca09a959f1c4ea1921958d4b7b88"),
    new AssetReference("shield_07.prefab:62ae59fe1ee4edb41ab4e2f56f2c4c9d"),
    new AssetReference("shield_08.prefab:2ac01bcf753502d4391495e1ba01297f")
  };
  private static readonly AssetReference[] m_backgroundTextures = new AssetReference[15]
  {
    new AssetReference("FSG_BG_01.psd:e688e3dbcd82aa540bd5a237b8046087"),
    new AssetReference("FSG_BG_02.psd:ae5f9d676c6184d41b976845d4131392"),
    new AssetReference("FSG_BG_03.psd:2bfa2796138e44b4db4ffd7d7c35048c"),
    new AssetReference("FSG_BG_04.psd:4968e80d9d5570a49bf34a477953c463"),
    new AssetReference("FSG_BG_05.psd:8b2ce0acdd997df4d9a235d10b0b0245"),
    new AssetReference("FSG_BG_06.psd:50c2055eec0ae094e8c44d98a86ec997"),
    new AssetReference("FSG_BG_07.psd:2e9be5a80e6fb8c4ab5e8f30ecb529b5"),
    new AssetReference("FSG_BG_08.psd:3413b3c98ad07944b923e5b475c5cb71"),
    new AssetReference("FSG_BG_09.psd:495ad5978abcac5428426c9422b34f54"),
    new AssetReference("FSG_BG_10.psd:6840b0525caaafc46ada6c96aec606c7"),
    new AssetReference("FSG_BG_11.psd:183ea8ead0f840b458c3b6b6feaecd9e"),
    new AssetReference("FSG_BG_12.psd:36143d8a02d74644a95f4f875b084687"),
    new AssetReference("FSG_BG_13.psd:e86b53637b4c7f940af65cf93f139ad7"),
    new AssetReference("FSG_BG_14.psd:ca315657cd75a3d4183070b7620eea46"),
    new AssetReference("FSG_BG_15.psd:ba08c85d3825071429b4452a05e1f869")
  };
  private static readonly AssetReference[] m_majorTextures = new AssetReference[85]
  {
    new AssetReference("FSG_major_icon_01.psd:07f39638ef5fac0409bceafcfe91a017"),
    new AssetReference("FSG_major_icon_02.psd:ba033ce365731044dbd0a6447b927516"),
    new AssetReference("FSG_major_icon_03.psd:4e63ef4c0d31305449bc692cd8ed4296"),
    new AssetReference("FSG_major_icon_04.psd:1600613c07c2b894db3985ae0d058df6"),
    new AssetReference("FSG_major_icon_05.psd:2b3cec372d669a14bac09798de77f4aa"),
    new AssetReference("FSG_major_icon_06.psd:7f3790b88769cd745bae9b0bca991a42"),
    new AssetReference("FSG_major_icon_07.psd:d6de41cea2ada024e8e4f561f7604691"),
    new AssetReference("FSG_major_icon_08.psd:7511ad8cbc11b8f4abc097895bf36f72"),
    new AssetReference("FSG_major_icon_09.psd:52a8139050006ee4f8a713228ec0680e"),
    new AssetReference("FSG_major_icon_10.psd:783574c20786759499bd49291d09dd0b"),
    new AssetReference("FSG_major_icon_11.psd:abec391ccf583f2409e4274963e11fb7"),
    new AssetReference("FSG_major_icon_12.psd:ef67cc5c43169bd4a9fbf9e429fff2c1"),
    new AssetReference("FSG_major_icon_13.psd:bec11e064c7f3fd408c59174e001a566"),
    new AssetReference("FSG_major_icon_14.psd:db89aa7c56a75e542b41b4b68934150b"),
    new AssetReference("FSG_major_icon_15.psd:7227d20dda7e8b743b5b3429065b94cb"),
    new AssetReference("FSG_major_icon_16.psd:9a95f2321a81b034bb21bdd8af813dd7"),
    new AssetReference("FSG_major_icon_17.psd:7e3e3309328a8d24ca4d73a819455026"),
    new AssetReference("FSG_major_icon_18.psd:664f85016c825ae4d87b32f1e2aee030"),
    new AssetReference("FSG_major_icon_19.psd:585e794cb0197db47bf865aa7342bfce"),
    new AssetReference("FSG_major_icon_20.psd:fed552b35702f944eb3d71eaaebd811b"),
    new AssetReference("FSG_major_icon_21.psd:53a8ce78f94668b419be52058eb62744"),
    new AssetReference("FSG_major_icon_22.psd:e3f2458e7131899489ca0cd3e96202ce"),
    new AssetReference("FSG_major_icon_23.psd:061bd991c06d66b45a3e6e85ee917085"),
    new AssetReference("FSG_major_icon_24.psd:968f4450552f1234786ebc685dcfab37"),
    new AssetReference("FSG_major_icon_25.psd:0e7774e4335227148b30c32a319dcd91"),
    new AssetReference("FSG_major_icon_26.psd:9d7beaf2c0b180b4b836ce5d3ba213f0"),
    new AssetReference("FSG_major_icon_27.psd:7055b2b8640998f478e59eb17063d434"),
    new AssetReference("FSG_major_icon_28.psd:3b0eae9bf1035f943a83b5da9ea06265"),
    new AssetReference("FSG_major_icon_29.psd:733546c7388d3db4da3875a12d2dbb04"),
    new AssetReference("FSG_major_icon_30.psd:b3189578cae8a2a418edd28b18871e5a"),
    new AssetReference("FSG_major_icon_31.psd:440512e280677784e9d203d183bd4b3b"),
    new AssetReference("FSG_major_icon_32.psd:d6ba8937a47ae3443bf494d7081df118"),
    new AssetReference("FSG_major_icon_33.psd:8817a57cf03ca3b459f8179201e9ef61"),
    new AssetReference("FSG_major_icon_34.psd:11244accdd35fcf4eadff77a526e674c"),
    new AssetReference("FSG_major_icon_35.psd:9b5ab5a32b35f9744869f20eda9a5a3f"),
    new AssetReference("FSG_major_icon_36.psd:b63250e48235c3446aaed3d3eda8a039"),
    new AssetReference("FSG_major_icon_37.psd:06d29c693b13e4341bdccc98879318f3"),
    new AssetReference("FSG_major_icon_38.psd:b595fe851a209284090902687ef4719a"),
    new AssetReference("FSG_major_icon_39.psd:b1a27e92316e6154695947477b692f31"),
    new AssetReference("FSG_major_icon_40.psd:aed8fc8c49d256b4a9bdbb8d1f2653b4"),
    new AssetReference("FSG_major_icon_41.psd:d0c01bd273040e54fa1e37b4b968e21d"),
    new AssetReference("FSG_major_icon_42.psd:cad47cc1621a9a74aacb7538e62ed968"),
    new AssetReference("FSG_major_icon_43.psd:6a62aa135e1d7f24e9d16f232c819771"),
    new AssetReference("FSG_major_icon_44.psd:e1fa19d8f78ed604986f3e3b08da86e3"),
    new AssetReference("FSG_major_icon_45.psd:94a2d34daafb0db48aeee1bb29f30d94"),
    new AssetReference("FSG_major_icon_46.psd:eae1bb9b57e5d2d46b76c2da550d9361"),
    new AssetReference("FSG_major_icon_47.psd:f6d114a4c539da7409ed64c04a6b4d1c"),
    new AssetReference("FSG_major_icon_48.psd:13482644107b8124baaa27c4b69f7f40"),
    new AssetReference("FSG_major_icon_49.psd:60f5e08764889bd4891230677667c06a"),
    new AssetReference("FSG_major_icon_50.psd:ae3b236c6985ed149a8e11cae889891d"),
    new AssetReference("FSG_major_icon_51.psd:be89f941faf7e884f9517b3cc758cc95"),
    new AssetReference("FSG_major_icon_52.psd:d64ccacd8e3575f4f8934868aeeebac0"),
    new AssetReference("FSG_major_icon_53.psd:85e1dfeb648dd25478c278b10452ea04"),
    new AssetReference("FSG_major_icon_54.psd:3639a3461412bcd468f36bd0d8808194"),
    new AssetReference("FSG_major_icon_55.psd:6dfe240913765e7439864ae59e906021"),
    new AssetReference("FSG_major_icon_56.psd:50542c563f4226746948db1227e041a8"),
    new AssetReference("FSG_major_icon_57.psd:c35838a239a1a9d4fbe40886c1836151"),
    new AssetReference("FSG_major_icon_58.psd:0153d881bf26d904eafe57c0f4069b68"),
    new AssetReference("FSG_major_icon_59.psd:7ea63d9dc56429843ba7ad8e662d6be4"),
    new AssetReference("FSG_major_icon_60.psd:2da17d5f4dd8218458a8a47cc5a5315b"),
    new AssetReference("FSG_major_icon_61.psd:7138870a6a857f5439fda6dbf723ee8a"),
    new AssetReference("FSG_major_icon_62.psd:56c1d251d05be7849b21a92236292231"),
    new AssetReference("FSG_major_icon_63.psd:956d1ae251106c043aeea63cc82a8dc0"),
    new AssetReference("FSG_major_icon_64.psd:8507174dedd9fbf46ad2a3eb3608d3d0"),
    new AssetReference("FSG_major_icon_65.psd:f1c75986e3593584ab58710c5b9afc16"),
    new AssetReference("FSG_major_icon_66.psd:547d68ca4a4d9a847ba684b31d672754"),
    new AssetReference("FSG_major_icon_67.psd:c302ac3c5c6208b4d82db58ec2534840"),
    new AssetReference("FSG_major_icon_68.psd:999a9c18e17735246b975f538f5f39e9"),
    new AssetReference("FSG_major_icon_69.psd:d3bd17b65f76e734bb02b6201576613f"),
    new AssetReference("FSG_major_icon_70.psd:df022a496a17dd64c95a5f4f6d9e1dbf"),
    new AssetReference("FSG_major_icon_71.psd:390ccf576a2fc464ab1c3cdf9a01c05f"),
    new AssetReference("FSG_major_icon_72.psd:13c3d60992523af418244d648bec2927"),
    new AssetReference("FSG_major_icon_73.psd:bc28bce291ff2284395f1504ebd4c352"),
    new AssetReference("FSG_major_icon_74.psd:a7d9b83cf0f7ebf45abac1b19bd64f25"),
    new AssetReference("FSG_major_icon_75.psd:74d90f06be30baa4aba2c7d629d56edd"),
    new AssetReference("FSG_major_icon_76.psd:bf7de29836133584cb1529079c315956"),
    new AssetReference("FSG_major_icon_77.psd:1126fe28c57f50c42af8de4e64016dbe"),
    new AssetReference("FSG_major_icon_78.psd:f3d8a417df5cce244802d22ee22fb8f3"),
    new AssetReference("FSG_major_icon_79.psd:705c5ab1b294713469d61cb9b719c21f"),
    new AssetReference("FSG_major_icon_80.psd:7e4b7b718714ecc43b757f31b6f35b5a"),
    new AssetReference("FSG_major_icon_81.psd:16c7b39fdde347e4a80f6912a6a9c20e"),
    new AssetReference("FSG_major_icon_82.psd:5cd2aa06a7003ae449270d3f57698eaf"),
    new AssetReference("FSG_major_icon_83.psd:4e038124f13272e4197dca5ded30e3ec"),
    new AssetReference("FSG_major_icon_84.psd:66c8e00ffd94eaa4bb91ef35c8bfab63"),
    new AssetReference("FSG_major_icon_85.psd:79cc45177c1bbe3438a8404770221fbd")
  };
  private static readonly AssetReference[] m_minorTextures = new AssetReference[43]
  {
    new AssetReference("FSG_minor_icon_01.psd:76f1d2c44969469479ca1d22ed4bb2c5"),
    new AssetReference("FSG_minor_icon_02.psd:5720e69f56a33f343893cbe0bdb83328"),
    new AssetReference("FSG_minor_icon_03.psd:581cd97bcda285d469ea061c2a1b65e0"),
    new AssetReference("FSG_minor_icon_04.psd:eb9a4735aaee11047b850c1639180cc0"),
    new AssetReference("FSG_minor_icon_05.psd:73ffd45b96f36984683d94002aeb687f"),
    new AssetReference("FSG_minor_icon_06.psd:fb79ddcfa27651141a69fe57d707fd31"),
    new AssetReference("FSG_minor_icon_07.psd:2dc56df138ffca54c99c2823d8b8c230"),
    new AssetReference("FSG_minor_icon_08.psd:aa6b9c693dba947459171534c501561c"),
    new AssetReference("FSG_minor_icon_09.psd:1223eaead151e0442b39d44b79d2fe99"),
    new AssetReference("FSG_minor_icon_10.psd:e35d16e74556b824a82a24fdef18ab6e"),
    new AssetReference("FSG_minor_icon_11.psd:686a7f1c092367a4ebefcf0be60a5025"),
    new AssetReference("FSG_minor_icon_12.psd:f6951bde51d82f94b95dd49a17a9acde"),
    new AssetReference("FSG_minor_icon_13.psd:c22f4029bc9a09c47b05ce79910a85c9"),
    new AssetReference("FSG_minor_icon_14.psd:a19fd3d9b22bd5f439df4ceb1bf65b3d"),
    new AssetReference("FSG_minor_icon_15.psd:ef282300e35d6114682429509c1ec6be"),
    new AssetReference("FSG_minor_icon_16.psd:5a72a33d18d433442a381db9aa9c5eae"),
    new AssetReference("FSG_minor_icon_17.psd:b13ef82f1b931c741bbffc35b55ce244"),
    new AssetReference("FSG_minor_icon_18.psd:ba5c373f08f049848b950f3c547edc00"),
    new AssetReference("FSG_minor_icon_19.psd:fd556fad5a1adc0448aa85e14ae0b33b"),
    new AssetReference("FSG_minor_icon_20.psd:ec061cb081c39a749a3d7cc07aa4c5af"),
    new AssetReference("FSG_minor_icon_21.psd:6d47b3264df5bad40847b6a0b2c763ff"),
    new AssetReference("FSG_minor_icon_22.psd:4b21f492cc667104b9b4fc87dabca71f"),
    new AssetReference("FSG_minor_icon_23.psd:4cd0570a850886d41a962273726efc86"),
    new AssetReference("FSG_minor_icon_24.psd:cc11c8161f16f1a4298cc85629fd8f24"),
    new AssetReference("FSG_minor_icon_25.psd:f86b6ce99de7dba48a6a3f9617d9e37a"),
    new AssetReference("FSG_minor_icon_26.psd:b85753f42615a3442a328f41fb214a8d"),
    new AssetReference("FSG_minor_icon_27.psd:76ed51a0cbdac4a4885b6a9a6f35db34"),
    new AssetReference("FSG_minor_icon_28.psd:4e7c0cbabe0df1a4aa75ffb7ad4ecf3d"),
    new AssetReference("FSG_minor_icon_29.psd:45af214ee19ff79408336758bfbbd400"),
    new AssetReference("FSG_minor_icon_30.psd:c3670d1a631e2054984bd6ade942600b"),
    new AssetReference("FSG_minor_icon_31.psd:c7f21d37679fc6d4b980876ece61e1f4"),
    new AssetReference("FSG_minor_icon_32.psd:10746cf72967e2541b7978ff5e23ef79"),
    new AssetReference("FSG_minor_icon_33.psd:af1b2ce747ad74143aff763617ba9691"),
    new AssetReference("FSG_minor_icon_34.psd:c79f56adcf6621b4f9574c1e18adb146"),
    new AssetReference("FSG_minor_icon_35.psd:aed7c2408cd63c94991f2f5dc91a046b"),
    new AssetReference("FSG_minor_icon_36.psd:6409f5c8977ba1b4bbf65d919b35f860"),
    new AssetReference("FSG_minor_icon_37.psd:b4654929917a9b340ac6da0e51c2093b"),
    new AssetReference("FSG_minor_icon_38.psd:2285c681967265847a6b583271ffc132"),
    new AssetReference("FSG_minor_icon_39.psd:a0dc328dcc11e3049905c29c85ffa1fe"),
    new AssetReference("FSG_minor_icon_40.psd:8a69a6cc8757a0643a06083a1c7b4b3d"),
    new AssetReference("FSG_minor_icon_41.psd:4c48d1ce608b0b24684545ce45a08db8"),
    new AssetReference("FSG_minor_icon_42.psd:61224edd16eba5e47bdde26683514baa"),
    new AssetReference("FSG_minor_icon_43.psd:785e9e8832f639647a0ca0a3da6ca9f2")
  };
  private const int MAX_SIGN_INDEX = 8;
  private const int MAX_BACKGROUND_INDEX = 15;
  private const int MAX_MAJOR_INDEX = 85;
  private const int MAX_MINOR_INDEX = 43;
  private GameObject m_transitionInputBlocker;
  private static ReactiveObject<NetCache.NetCacheFeatures> s_guardianVars = (ReactiveObject<NetCache.NetCacheFeatures>) ReactiveNetCacheObject<NetCache.NetCacheFeatures>.CreateInstance();
  private static ReactiveObject<FSGFeatureConfig> s_fsgFeaturesConfig = (ReactiveObject<FSGFeatureConfig>) ReactiveNetCacheObject<FSGFeatureConfig>.CreateInstance();
  private static ReactiveObject<NetCache.NetCacheProfileProgress> s_profileProgress = (ReactiveObject<NetCache.NetCacheProfileProgress>) ReactiveNetCacheObject<NetCache.NetCacheProfileProgress>.CreateInstance();
  private static ReactiveObject<NetCache.NetCacheClientOptions> s_clientOptions = (ReactiveObject<NetCache.NetCacheClientOptions>) ReactiveNetCacheObject<NetCache.NetCacheClientOptions>.CreateInstance();
  private ReactiveEnumOption<FormatType> m_FormatType = ReactiveEnumOption<FormatType>.CreateInstance(Option.FORMAT_TYPE);
  public long m_activeFSGMenu = -1;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public static event FiresideGatheringManager.OnPatronListUpdatedCallback OnPatronListUpdated;

  public FiresideGatheringManager.FiresideGatheringMode CurrentFiresideGatheringMode { get; set; }

  private GameObject SceneObject
  {
    get
    {
      if ((UnityEngine.Object) this.m_sceneObject == (UnityEngine.Object) null)
        this.m_sceneObject = new GameObject("FiresideGatheringManagerSceneObject", new System.Type[1]
        {
          typeof (HSDontDestroyOnLoad)
        });
      return this.m_sceneObject;
    }
  }

  private FiresideGatheringManagerData Data { get; set; }

  public bool HasSeenReturnToFSGSceneTooltip
  {
    get => this.Data.m_hasSeenReturnToFSGSceneTooltip;
    set => this.Data.m_hasSeenReturnToFSGSceneTooltip = value;
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    FiresideGatheringManager owner = this;
    owner.m_screenEffectsHandle = new ScreenEffectsHandle((object) owner);
    LoadResource loadData = new LoadResource("ServiceData/FiresideGatheringManagerData", LoadResourceFlags.FailOnError);
    yield return (IAsyncJobResult) loadData;
    owner.Data = loadData.LoadedAsset as FiresideGatheringManagerData;
    BnetPresenceMgr.Get();
    HearthstoneApplication.Get().WillReset += new System.Action(owner.WillReset);
    serviceLocator.Get<SceneMgr>().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(owner.SceneMgr_OnScenePreUnload));
    // ISSUE: reference to a compiler-generated method
    System.Action action1 = new System.Action(owner.\u003CInitialize\u003Eb__84_0);
    if ((UnityEngine.Object) ChatMgr.Get() == (UnityEngine.Object) null)
      ChatMgr.OnStarted += action1;
    else
      action1();
    // ISSUE: reference to a compiler-generated method
    System.Action action2 = new System.Action(owner.\u003CInitialize\u003Eb__84_1);
    if ((UnityEngine.Object) DialogManager.Get() == (UnityEngine.Object) null)
      DialogManager.OnStarted += action2;
    else
      action2();
    Network network = serviceLocator.Get<Network>();
    network.RegisterNetHandler((object) RequestNearbyFSGsResponse.PacketID.ID, new Network.NetHandler(owner.OnRequestNearbyFSGsResponse));
    network.RegisterNetHandler((object) CheckInToFSGResponse.PacketID.ID, new Network.NetHandler(owner.OnCheckInToFSGResponse));
    network.RegisterNetHandler((object) CheckOutOfFSGResponse.PacketID.ID, new Network.NetHandler(owner.OnCheckOutOfFSGResponse));
    network.RegisterNetHandler((object) InnkeeperSetupGatheringResponse.PacketID.ID, new Network.NetHandler(owner.OnInnkeeperSetupGatheringResponse));
    network.RegisterNetHandler((object) FSGPatronListUpdate.PacketID.ID, new Network.NetHandler(owner.OnPatronListUpdateReceivedFromServer));
    FiresideGatheringManager.s_guardianVars.Init();
    FiresideGatheringManager.s_clientOptions.Init();
    FiresideGatheringManager.s_profileProgress.Init();
    FiresideGatheringManager.s_fsgFeaturesConfig.Init();
    NetCache netCache = serviceLocator.Get<NetCache>();
    netCache.RegisterUpdatedListener(typeof (NetCache.NetCacheFeatures), new System.Action(owner.OnNetCache_GuardianVars));
    netCache.RegisterUpdatedListener(typeof (FSGFeatureConfig), new System.Action(owner.OnNetCache_FSGFeatureConfig));
    netCache.RegisterUpdatedListener(typeof (NetCache.NetCacheProfileProgress), new System.Action(owner.CheckCanBeginLocationDataGatheringForLogin));
    netCache.RegisterUpdatedListener(typeof (NetCache.NetCacheClientOptions), new System.Action(owner.CheckCanBeginLocationDataGatheringForLogin));
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(owner.OnPlayersPresenceChanged));
    // ISSUE: reference to a compiler-generated method
    System.Action action3 = new System.Action(owner.\u003CInitialize\u003Eb__84_2);
    if (CollectionManager.Get() == null)
      CollectionManager.OnCollectionManagerReady += new CollectionManager.DelCollectionManagerReady(action3.Invoke);
    else
      action3();
    owner.CheckCanBeginLocationDataGatheringForLogin();
  }

  public System.Type[] GetDependencies() => new System.Type[4]
  {
    typeof (Network),
    typeof (NetCache),
    typeof (FullScreenFXMgr),
    typeof (SceneMgr)
  };

  public void Shutdown()
  {
    HearthstoneApplication.Get().WillReset -= new System.Action(this.WillReset);
    if (!this.IsCheckedIn)
      return;
    Log.FiresideGatherings.Print("OnApplicationQuit: calling check out.");
    this.CheckOutOfFSG();
  }

  public void Update()
  {
    if (!this.m_waitingForCheckIn)
      return;
    if (this.IsCheckedIn)
    {
      this.TransitionToFSGSceneIfSafe();
      if (SceneMgr.Get().GetMode() != SceneMgr.Mode.FIRESIDE_GATHERING && SceneMgr.Get().GetNextMode() != SceneMgr.Mode.FIRESIDE_GATHERING)
        return;
      this.m_waitingForCheckIn = false;
    }
    else
    {
      if (this.CanAutoCheckInEventually && this.m_nearbyFSGs.Count >= 1)
        return;
      this.m_waitingForCheckIn = false;
      DialogManager.Get().ShowFiresideGatheringCheckInFailedDialog();
    }
  }

  public void FixedUpdate()
  {
    if (!this.m_haltFSGNotificationsAndCheckins && GameUtils.IsTraditionalTutorialComplete())
    {
      this.AutoInnkeeperSetup();
      this.AutoCheckIn();
      this.NotifyFSGNearbyIfNeeded();
    }
    this.DoStartAndEndTimingEvents();
    this.m_haltFSGNotificationsAndCheckins = false;
  }

  public float GetSecondsBetweenUpdates() => 1f;

  private void WillReset()
  {
    if (this.IsCheckedIn)
    {
      this.CheckOutOfFSG();
      this.LeaveFSG();
    }
    this.m_nearbyFSGs.Clear();
    this.m_hasBegunLocationDataGatheringForLogin = false;
    this.m_fsgSignShown = false;
    this.m_tooltipShowing = (Notification) null;
    this.HasSeenReturnToFSGSceneTooltip = false;
    this.m_waitingForCheckIn = false;
    this.m_errorOccuredOnCheckin = false;
    ChatMgr.Get().OnFriendListToggled -= new ChatMgr.FriendListToggled(this.ShowReturnToFSGSceneTooltip_OnFriendListToggled_ShowNextTooltip);
  }

  public static FiresideGatheringManager Get() => ServiceManager.Get<FiresideGatheringManager>();

  public void SetFSGFeatureStatus(bool isEnabled) => FiresideGatheringManager.m_isEnabledByPrivacySettings = isEnabled;

  public static bool IsFSGFeatureEnabled
  {
    get
    {
      if (!FiresideGatheringManager.m_isEnabledByPrivacySettings || TemporaryAccountManager.IsTemporaryAccount())
        return false;
      NetCache.NetCacheFeatures netCacheFeatures = FiresideGatheringManager.s_guardianVars.Value;
      bool flag = netCacheFeatures != null && netCacheFeatures.FSGEnabled;
      return GameUtils.IsTraditionalTutorialComplete() && flag;
    }
  }

  public static bool IsGpsFeatureEnabled
  {
    get
    {
      if (!FiresideGatheringManager.IsFSGFeatureEnabled)
        return false;
      FSGFeatureConfig fsgFeatureConfig = FiresideGatheringManager.s_fsgFeaturesConfig.Value;
      return fsgFeatureConfig == null || fsgFeatureConfig.Gps;
    }
  }

  public static bool IsWifiFeatureEnabled
  {
    get
    {
      if (!FiresideGatheringManager.IsFSGFeatureEnabled)
        return false;
      FSGFeatureConfig fsgFeatureConfig = FiresideGatheringManager.s_fsgFeaturesConfig.Value;
      return fsgFeatureConfig == null || fsgFeatureConfig.Wifi;
    }
  }

  public static bool CanRequestNearbyFSG => FiresideGatheringManager.IsFSGFeatureEnabled && (FiresideGatheringManager.IsGpsFeatureEnabled || FiresideGatheringManager.IsWifiFeatureEnabled);

  public bool IsRequestNearbyFSGsPending => this.m_isRequestNearbyFSGsPending;

  private string GetTavernName_TavernSign(FSGConfig fsg) => string.IsNullOrEmpty(fsg.TavernName) ? GameStrings.Get("GLOBAL_FIRESIDE_GATHERING_DEFAULT_TAVERN_NAME") : fsg.TavernName;

  public string GetTavernName_FriendsList(FSGConfig fsg)
  {
    if (!string.IsNullOrEmpty(fsg.TavernName))
      return fsg.TavernName;
    BnetPlayer bnetPlayer;
    if (!this.m_innkeepers.TryGetValue(fsg.FsgId, out bnetPlayer) || !(bnetPlayer.GetBattleTag() != (BnetBattleTag) null))
      return GameStrings.Get("GLOBAL_FIRESIDE_GATHERING_DEFAULT_TAVERN_NAME");
    return GameStrings.Format("GLOBAL_FIRESIDE_GATHERING_FIRST_TIME_TAVERN_NAME", (object) bnetPlayer.GetBattleTag().ToString());
  }

  public bool HasFSGToInnkeeperSetup => this.m_innkeeperFSG != null;

  public FSGConfig FSGToInnkeeperSetup => this.m_innkeeperFSG;

  public TavernSignData LastSign { get; private set; }

  public event FiresideGatheringManager.CheckedInToFSGCallback OnJoinFSG;

  public event FiresideGatheringManager.CheckedOutOfFSGCallback OnLeaveFSG;

  public event FiresideGatheringManager.RequestNearbyFSGsCallback OnNearbyFSGs;

  public event FiresideGatheringManager.NearbyFSGsChangedCallback OnNearbyFSGsChanged;

  public event FiresideGatheringManager.OnInnkeeperSetupFinishedCallback OnInnkeeperSetupFinished;

  public event FiresideGatheringManager.FSGSignClosedCallback OnSignClosed;

  public event FiresideGatheringManager.FSGSignShownCallback OnSignShown;

  public void CheckInToFSG(long fsgId)
  {
    this.m_checkInRequestPending = true;
    this.m_nearbyFSGsFoundEventSent = true;
    if (BnetPresenceMgr.Get().GetMyPlayer().IsAppearingOffline())
    {
      this.PromptPlayerToAppearOnline(fsgId);
    }
    else
    {
      FSGConfig fsgConfig = this.m_nearbyFSGs.FirstOrDefault<FSGConfig>((Func<FSGConfig, bool>) (f => f.FsgId == fsgId));
      string str = fsgConfig == null ? "<notfound>" : fsgConfig.TavernName;
      Log.FiresideGatherings.Print("CheckInToFSG: sending check in to server for {0}-{1}", (object) fsgId, string.IsNullOrEmpty(str) ? (object) "<no name>" : (object) str);
      if (this.m_gpsCheatingLocation)
        Network.Get().CheckInToFSG(fsgId, this.m_gpsCheatLatitude, this.m_gpsCheatLongitude, 0.0, FiresideGatheringManager.IsWifiFeatureEnabled ? this.BSSIDS : (List<string>) null);
      else if (this.IsGpsLocationValid)
        Network.Get().CheckInToFSG(fsgId, this.Latitude, this.Longitude, this.GpsAccuracy, FiresideGatheringManager.IsWifiFeatureEnabled ? this.BSSIDS : (List<string>) null);
      else if (FiresideGatheringManager.IsWifiFeatureEnabled)
      {
        Network.Get().CheckInToFSG(fsgId, this.BSSIDS);
      }
      else
      {
        if (!this.m_waitingForCheckIn)
          return;
        this.m_waitingForCheckIn = false;
        this.ShowNoGPSOrWifiAlertPopup();
      }
    }
  }

  public void SetWaitingForCheckIn() => this.m_waitingForCheckIn = true;

  public void ClearErrorOccuredOnCheckIn() => this.m_errorOccuredOnCheckin = false;

  public void BeginLocationDataGatheringForLogin()
  {
    if (this.m_hasBegunLocationDataGatheringForLogin)
      return;
    Log.FiresideGatherings.Print("FiresideGatheringManager.BeginLocationDataGathering");
    if (!FiresideGatheringManager.IsFSGFeatureEnabled)
    {
      Log.FiresideGatherings.Print("FiresideGatheringManager.BeginLocationDataGathering FEATURE DISABLED");
    }
    else
    {
      if (!this.HasManuallyInitiatedFSGScanBefore.Value)
        return;
      if (!ClientLocationManager.Get().GPSServicesReady)
      {
        Processor.RunCoroutine(this.WaitThenBeginLocationDataGatheringForLogin());
      }
      else
      {
        bool flag1 = Vars.Key("Location.Latitude").HasValue & Vars.Key("Location.Longitude").HasValue;
        string[] source = Vars.Key("Location.BSSID").GetStr(string.Empty).Split(new char[3]
        {
          ' ',
          ',',
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        bool flag2 = source != null && source.Length != 0;
        bool flag3 = FiresideGatheringManager.IsGpsFeatureEnabled && (flag1 || ClientLocationManager.Get().GPSAvailable);
        this.m_hasBegunLocationDataGatheringForLogin = true;
        if (flag1 | flag2)
        {
          ClientLocationData locationData1 = (ClientLocationData) null;
          ClientLocationData locationData2 = (ClientLocationData) null;
          if (flag3 & flag1)
          {
            double latitude = Vars.Key("Location.Latitude").GetDouble(0.0);
            double longitude = Vars.Key("Location.Longitude").GetDouble(0.0);
            locationData1 = new ClientLocationData();
            locationData1.location = new GpsCoordinate(latitude, longitude, 0.0, TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds);
            this.OnLocationDataGPSUpdate(locationData1);
          }
          if (FiresideGatheringManager.IsWifiFeatureEnabled & flag2)
          {
            locationData2 = new ClientLocationData();
            locationData2.accessPointSamples = ((IEnumerable<string>) source).Select<string, AccessPointInfo>((Func<string, AccessPointInfo>) (bssid => new AccessPointInfo()
            {
              bssid = bssid
            })).ToList<AccessPointInfo>();
            this.OnLocationDataWIFIUpdate(locationData2);
          }
          if (locationData1 != null || locationData2 != null)
          {
            this.OnLocationDataComplete();
            return;
          }
        }
        if (flag3)
        {
          if (FiresideGatheringManager.IsWifiFeatureEnabled)
            ClientLocationManager.Get().RequestGPSAndWifiData(new System.Action<ClientLocationData>(this.OnLocationDataGPSUpdate), new System.Action<ClientLocationData>(this.OnLocationDataWIFIUpdate), new System.Action(this.OnLocationDataComplete));
          else
            ClientLocationManager.Get().RequestGPSData(new System.Action<ClientLocationData>(this.OnLocationDataGPSUpdate), new System.Action(this.OnLocationDataComplete));
        }
        else if (FiresideGatheringManager.IsWifiFeatureEnabled)
          ClientLocationManager.Get().RequestWifiData(new System.Action<ClientLocationData>(this.OnLocationDataWIFIUpdate), new System.Action(this.OnLocationDataComplete));
        else
          this.RequestNearbyFSGs(true);
      }
    }
  }

  private IEnumerator WaitThenBeginLocationDataGatheringForLogin()
  {
    Log.FiresideGatherings.Print("FiresideGatheringManager.WaitThenBeginLocationDataGatheringForLogin");
    yield return (object) new WaitForSeconds(1f);
    this.BeginLocationDataGatheringForLogin();
  }

  public FSGConfig CurrentFSG => this.m_currentFSG;

  public long CurrentFsgId => this.m_currentFSG != null ? this.m_currentFSG.FsgId : 0L;

  public bool CurrentFsgIsLargeScale => this.m_currentFSG != null && this.m_currentFSG.HasIsLargeScaleFsg && this.m_currentFSG.IsLargeScaleFsg;

  public byte[] CurrentFsgSharedSecretKey => this.m_currentFSGSharedSecretKey;

  public List<GameContentScenario> CurrentFsgBrawls
  {
    get
    {
      TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING);
      if (mission == null)
        return new List<GameContentScenario>();
      bool useFallbackBrawls = this.m_innkeeperSelectedBrawlLibraryItemIds.Count == 0;
      return mission.BrawlList.Where<GameContentScenario>((Func<GameContentScenario, bool>) (scen =>
      {
        if (scen.IsRequired)
          return true;
        if (useFallbackBrawls)
        {
          if (scen.IsFallback)
            return true;
        }
        else if (this.m_innkeeperSelectedBrawlLibraryItemIds.Contains(scen.LibraryItemId))
          return true;
        return false;
      })).ToList<GameContentScenario>();
    }
  }

  public void CheckOutOfFSG(bool optOut = false)
  {
    if (!this.IsCheckedIn)
      return;
    if (optOut)
      this.PlayerAccountShouldAutoCheckin.Set(false);
    FSGConfig currentFsg = this.m_currentFSG;
    this.BackOutOfFSGScene();
    Network.Get().CheckOutOfFSG(currentFsg.FsgId);
  }

  private void BackOutOfFSGScene()
  {
    if (FiresideGatheringManager.Get().CurrentFiresideGatheringMode != FiresideGatheringManager.FiresideGatheringMode.NONE && SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
    {
      Navigation.Clear();
      if (!HearthstoneApplication.Get().IsResetting())
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
    this.CurrentFiresideGatheringMode = FiresideGatheringManager.FiresideGatheringMode.NONE;
  }

  private void CollectionManager_DeckDeleted(CollectionDeck removedDeck)
  {
    if (removedDeck.Type != DeckType.FSG_BRAWL_DECK)
      return;
    this.UpdateDeckValidity();
  }

  public void UpdateDeckValidity()
  {
    if (!this.IsCheckedIn)
    {
      BnetPresenceMgr.Get().SetDeckValidity((DeckValidity) null);
    }
    else
    {
      DeckValidity deckValidity = BnetPresenceMgr.Get().GetMyPlayer().GetHearthstoneGameAccount().GetDeckValidity() ?? new DeckValidity();
      foreach (FormatType formatType in Enum.GetValues(typeof (FormatType)))
      {
        switch (formatType)
        {
          case FormatType.FT_UNKNOWN:
          case FormatType.FT_WILD:
            continue;
          default:
            deckValidity.ValidFormatDecks.Add(new FormatDeckValidity()
            {
              FormatType = formatType,
              ValidDeck = CollectionManager.Get().AccountHasValidDeck(formatType)
            });
            continue;
        }
      }
      deckValidity.ValidFormatDecks.Add(new FormatDeckValidity()
      {
        FormatType = FormatType.FT_WILD,
        ValidDeck = CollectionManager.Get().AccountHasValidDeck(FormatType.FT_STANDARD) || CollectionManager.Get().AccountHasValidDeck(FormatType.FT_WILD)
      });
      deckValidity.ValidTavernBrawlDeck = this.GenerateBrawlDeckValidity(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
      deckValidity.ValidFiresideBrawlDeck = this.GenerateBrawlDeckValidity(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING);
      BnetPresenceMgr.Get().SetDeckValidity(deckValidity);
    }
  }

  private List<BrawlDeckValidity> GenerateBrawlDeckValidity(BrawlType brawlType)
  {
    List<BrawlDeckValidity> brawlDeckValidity = new List<BrawlDeckValidity>();
    if (!TavernBrawlManager.Get().IsTavernBrawlActive(brawlType))
      return brawlDeckValidity;
    TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(brawlType);
    if (mission == null)
      return brawlDeckValidity;
    int seasonId = mission.seasonId;
    foreach (GameContentScenario brawl in (IEnumerable<GameContentScenario>) mission.BrawlList)
    {
      bool flag = !mission.CanCreateDeck(brawl.LibraryItemId) || TavernBrawlManager.Get().HasValidDeck(brawlType, brawl.LibraryItemId);
      brawlDeckValidity.Add(new BrawlDeckValidity()
      {
        SeasonId = seasonId,
        BrawlLibraryItemId = brawl.LibraryItemId,
        ValidDeck = flag
      });
    }
    return brawlDeckValidity;
  }

  public bool OpponentHasValidDeckForSelectedPlaymode(BnetPlayer opponent)
  {
    DeckValidity deckValidity = opponent.GetHearthstoneGameAccount().GetDeckValidity();
    switch (this.CurrentFiresideGatheringMode)
    {
      case FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE:
        return deckValidity.ValidFormatDecks.Exists((Predicate<FormatDeckValidity>) (x => x.ValidDeck && x.FormatType == this.m_FormatType.Value));
      case FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE_BRAWL:
        return this.OpponentHasValidTavernBrawlDeck(BrawlType.BRAWL_TYPE_TAVERN_BRAWL, deckValidity == null ? (List<BrawlDeckValidity>) null : deckValidity.ValidTavernBrawlDeck);
      case FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL:
        return this.OpponentHasValidTavernBrawlDeck(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING, deckValidity == null ? (List<BrawlDeckValidity>) null : deckValidity.ValidFiresideBrawlDeck);
      default:
        return false;
    }
  }

  private bool OpponentHasValidTavernBrawlDeck(
    BrawlType brawlType,
    List<BrawlDeckValidity> brawlDeckValidity)
  {
    TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(brawlType);
    if (mission == null)
      return false;
    if (!mission.CanCreateDeck(mission.SelectedBrawlLibraryItemId))
      return true;
    if (brawlDeckValidity == null)
      return false;
    BrawlDeckValidity brawlDeckValidity1 = brawlDeckValidity.FirstOrDefault<BrawlDeckValidity>((Func<BrawlDeckValidity, bool>) (brawlInfo => brawlInfo.SeasonId == mission.seasonId && brawlInfo.BrawlLibraryItemId == mission.SelectedBrawlLibraryItemId));
    return brawlDeckValidity1 != null && brawlDeckValidity1.ValidDeck;
  }

  private void JoinFSG(
    long fsgID,
    List<FSGPatron> patrons,
    byte[] sharedSecretKey,
    List<int> innkeeperSelectedBrawlLibraryItemIds)
  {
    this.m_currentFSG = (FSGConfig) null;
    this.m_currentFSGSharedSecretKey = (byte[]) null;
    this.m_innkeeperSelectedBrawlLibraryItemIds.Clear();
    foreach (FSGConfig nearbyFsG in this.m_nearbyFSGs)
    {
      if (nearbyFsG.FsgId == fsgID)
      {
        this.m_currentFSG = nearbyFsG;
        this.m_currentFSGSharedSecretKey = sharedSecretKey;
        this.m_innkeeperSelectedBrawlLibraryItemIds = new HashSet<int>((IEnumerable<int>) innkeeperSelectedBrawlLibraryItemIds);
        break;
      }
    }
    if (this.m_currentFSG == null)
    {
      Log.FiresideGatherings.PrintError("FiresideGatheringManager.OnCheckInToGatheringResponse: Error: Didn't have a corresponding FSG for checkin");
      this.m_errorOccuredOnCheckin = true;
    }
    else
    {
      this.LastTavernID.Set(this.m_currentFSG.FsgId);
      this.m_pendingPatrons.Clear();
      this.m_displayablePatrons.Clear();
      this.m_knownPatronsFromServer.Clear();
      this.m_isAppendingPatronList = true;
      this.RebuildKnownPatronsFromPresence();
      if (!this.CurrentFsgIsLargeScale && patrons != null)
      {
        foreach (FSGPatron patron in patrons)
          this.AddKnownPatron(patron, false);
        FiresideGatheringPresenceManager.Get().AddRemovePatronSubscriptions(patrons, (List<FSGPatron>) null);
      }
      this.PlayerAccountShouldAutoCheckin.Set(true);
      this.m_isAppendingPatronList = false;
      this.UpdateMyPresence();
      Processor.ScheduleCallback((float) FiresideGatheringPresenceManager.PERIODIC_SUBSCRIBE_CHECK_SECONDS, true, new Processor.ScheduledCallback(this.PeriodicCheckForMoreSubscribeOpportunities));
      this.TransitionToFSGSceneIfSafe();
      if (this.OnJoinFSG == null)
        return;
      this.OnJoinFSG(this.m_currentFSG);
    }
  }

  private void OnSceneLoadedDuringAutoCheckin(
    SceneMgr.Mode mode,
    PegasusScene scene,
    object userData)
  {
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoadedDuringAutoCheckin));
    this.TransitionToFSGSceneIfSafe();
  }

  private void PromptPlayerToAppearOnline(long fsgId) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLOBAL_FIRESIDE_GATHERING"),
    m_text = GameStrings.Get("GLUE_FIRESIDE_GATHERING_APPEAR_ONLINE_PROMPT"),
    m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
    m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
    m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
    m_alertTextAlignment = UberText.AlignmentOptions.Center,
    m_responseCallback = new AlertPopup.ResponseCallback(this.OnAppearOnlineResponse),
    m_responseUserData = (object) fsgId
  });

  private void OnAppearOnlineResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CONFIRM)
    {
      BnetPresenceMgr.Get().SetAccountField(12U, false);
      this.CheckInToFSG((long) userData);
    }
    else
    {
      this.m_waitingForCheckIn = false;
      this.BackOutOfFSGScene();
      this.LeaveFSG();
    }
  }

  private void LeaveFSG()
  {
    FSGConfig currentFsg = this.m_currentFSG;
    this.m_currentFSG = (FSGConfig) null;
    this.m_currentFSGSharedSecretKey = (byte[]) null;
    this.m_innkeeperSelectedBrawlLibraryItemIds.Clear();
    this.m_nearbyFSGsFoundEventSent = true;
    this.HideFSGSign(this.m_fsgSignShown);
    this.m_fsgSignShown = false;
    this.HasSeenReturnToFSGSceneTooltip = false;
    this.m_pendingPatrons.Clear();
    this.m_displayablePatrons.Clear();
    this.m_knownPatronsFromServer.Clear();
    this.m_knownPatronsFromPresence.Clear();
    FiresideGatheringPresenceManager.Get().ClearSubscribedPatrons();
    this.PlayerAccountShouldAutoCheckin.Set(false);
    this.UpdateMyPresence();
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.PeriodicCheckForMoreSubscribeOpportunities));
    if (this.OnLeaveFSG == null)
      return;
    this.OnLeaveFSG(currentFsg);
  }

  [CustomEditField(Hide = true)]
  public bool IsCheckedIn => this.m_currentFSG != null;

  public bool IsPrerelease
  {
    get
    {
      if (this.m_currentFSG == null)
        return false;
      TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING);
      return mission != null && mission.IsPrerelease;
    }
  }

  public bool IsCheckedInToFSG(long gatheringID) => this.m_currentFSG != null && this.m_currentFSG.FsgId == gatheringID;

  public bool IsPlayerInMyFSG(BnetPlayer player)
  {
    if (player == null)
      return false;
    if (this.IsPlayerInMyFSGAndDisplayable(player))
      return true;
    foreach (BnetPlayer pendingPatron in this.m_pendingPatrons)
    {
      BnetAccountId accountId1 = pendingPatron.GetAccountId();
      BnetAccountId accountId2 = player.GetAccountId();
      if ((BnetEntityId) accountId1 != (BnetEntityId) null && (BnetEntityId) accountId2 != (BnetEntityId) null && (long) accountId1.Low == (long) accountId2.Low)
        return true;
      BnetGameAccountId bestGameAccountId1 = pendingPatron.GetBestGameAccountId();
      BnetGameAccountId bestGameAccountId2 = player.GetBestGameAccountId();
      if ((BnetEntityId) bestGameAccountId1 != (BnetEntityId) null && (BnetEntityId) bestGameAccountId2 != (BnetEntityId) null && (long) bestGameAccountId1.Low == (long) bestGameAccountId2.Low)
        return true;
    }
    return false;
  }

  public bool IsPlayerInMyFSGAndDisplayable(BnetPlayer player)
  {
    if (player == null || this.CurrentFsgIsLargeScale)
      return false;
    foreach (BnetPlayer displayablePatron in this.m_displayablePatrons)
    {
      BnetAccountId accountId1 = displayablePatron.GetAccountId();
      BnetAccountId accountId2 = player.GetAccountId();
      if ((BnetEntityId) accountId1 != (BnetEntityId) null && (BnetEntityId) accountId2 != (BnetEntityId) null && (long) accountId1.Low == (long) accountId2.Low)
        return true;
      BnetGameAccountId bestGameAccountId1 = displayablePatron.GetBestGameAccountId();
      BnetGameAccountId bestGameAccountId2 = player.GetBestGameAccountId();
      if ((BnetEntityId) bestGameAccountId1 != (BnetEntityId) null && (BnetEntityId) bestGameAccountId2 != (BnetEntityId) null && (long) bestGameAccountId1.Low == (long) bestGameAccountId2.Low)
        return true;
    }
    return false;
  }

  public List<FSGConfig> GetFSGs() => this.m_nearbyFSGs;

  public List<BnetPlayer> DisplayablePatronList => !this.IsCheckedIn || this.CurrentFsgIsLargeScale ? new List<BnetPlayer>() : this.m_displayablePatrons.ToList<BnetPlayer>();

  public int DisplayablePatronCount => !this.IsCheckedIn ? 0 : this.DisplayablePatronList.Count;

  public List<BnetPlayer> FullPatronList
  {
    get
    {
      List<BnetPlayer> fullPatronList = new List<BnetPlayer>();
      if (this.IsCheckedIn && !this.CurrentFsgIsLargeScale)
      {
        fullPatronList.AddRange((IEnumerable<BnetPlayer>) this.m_displayablePatrons);
        fullPatronList.AddRange((IEnumerable<BnetPlayer>) this.m_pendingPatrons);
      }
      return fullPatronList;
    }
  }

  public int FiresideGatheringSort(FSGConfig fsg1, FSGConfig fsg2)
  {
    if (this.IsCheckedInToFSG(fsg1.FsgId))
      return 1;
    if (this.IsCheckedInToFSG(fsg2.FsgId))
      return -1;
    int num = string.Compare(fsg1.TavernName, fsg2.TavernName);
    return num != 0 ? num : (int) (fsg1.FsgId - fsg2.FsgId);
  }

  public int FiresideGatheringPlayerSort(BnetPlayer patron1, BnetPlayer patron2)
  {
    int result = 0;
    bool lhsflag = BnetFriendMgr.Get().IsFriend(patron1);
    bool rhsflag = BnetFriendMgr.Get().IsFriend(patron2);
    return FriendUtils.FriendFlagSort(patron1, patron2, lhsflag, rhsflag, out result) ? result : FriendUtils.FriendNameSort(patron1, patron2);
  }

  public bool ShowSignIfNeeded(FiresideGatheringManager.OnCloseSign callback = null)
  {
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    if (!this.IsCheckedIn || this.m_fsgSignShown || SceneMgr.Get().IsTransitioning() || mode != SceneMgr.Mode.FIRESIDE_GATHERING)
      return false;
    this.m_fsgSignShown = true;
    this.ShowSign(this.m_currentFSG.SignData, this.GetTavernName_TavernSign(this.m_currentFSG), callback);
    return true;
  }

  public bool ShowSmallSignIfNeeded(Transform smallSignContainer)
  {
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    if (!this.IsCheckedIn || !this.m_fsgSignShown || mode != SceneMgr.Mode.FIRESIDE_GATHERING)
      return false;
    this.m_fsgSignShown = true;
    this.m_smallSignContainer = smallSignContainer;
    this.ShowSmallSign(this.m_currentFSG.SignData, this.GetTavernName_TavernSign(this.m_currentFSG));
    return true;
  }

  public void GotoFSGLink() => Application.OpenURL(ExternalUrlService.Get().GetFSGLink());

  public void ShowFindFSGDialog()
  {
    if (this.HasManuallyInitiatedFSGScanBefore.Value)
      ClientLocationManager.Get().RequestGPSData(new System.Action<ClientLocationData>(this.OnLocationDataGPSUpdate));
    DialogManager.Get().ShowFiresideGatheringFindEventDialog(new FiresideGatheringFindEventDialog.ResponseCallback(this.OnFindEventDialogCallBack));
  }

  public void OnLocationDataGPSUpdate(ClientLocationData locationData)
  {
    if (this.m_locationData == null)
      this.m_locationData = locationData;
    this.m_locationData.location = locationData.location;
  }

  public void OnLocationDataWIFIUpdate(ClientLocationData locationData)
  {
    if (this.m_locationData == null)
      this.m_locationData = locationData;
    this.m_locationData.accessPointSamples = locationData.accessPointSamples;
    this.m_accumulatedAccessPoints.Clear();
    foreach (AccessPointInfo accessPointSample in this.m_locationData.accessPointSamples)
    {
      if (FiresideGatheringManager.IsValidBSSID(accessPointSample.bssid))
        this.m_accumulatedAccessPoints[accessPointSample.bssid] = accessPointSample;
    }
  }

  public void AddWIFIAccessPoints(ClientLocationData locationData)
  {
    if (this.m_locationData == null)
      this.m_locationData = locationData;
    if (locationData == null)
      return;
    foreach (AccessPointInfo accessPointSample in locationData.accessPointSamples)
    {
      if (FiresideGatheringManager.IsValidBSSID(accessPointSample.bssid))
        this.m_accumulatedAccessPoints[accessPointSample.bssid] = accessPointSample;
    }
  }

  public void RequestNearbyFSGs(bool isStateCheck = false)
  {
    if (!FiresideGatheringManager.IsFSGFeatureEnabled)
    {
      Log.FiresideGatherings.Print("Not requesting Nearby FSGs because feature is disabled for me.");
    }
    else
    {
      Log.FiresideGatherings.Print("Requesting Nearby FSGS: gps={0} wifi={1} accuracy={2}", (object) FiresideGatheringManager.IsGpsFeatureEnabled, (object) FiresideGatheringManager.IsWifiFeatureEnabled, (object) this.GpsAccuracy);
      this.m_isRequestNearbyFSGsPending = true;
      if (this.m_gpsCheatingLocation)
        Network.Get().RequestNearbyFSGs(this.m_gpsCheatLatitude, this.m_gpsCheatLongitude, 0.0, FiresideGatheringManager.IsWifiFeatureEnabled ? this.BSSIDS : (List<string>) null);
      else if (isStateCheck)
        Network.Get().RequestNearbyFSGs((List<string>) null);
      else if (this.IsGpsLocationValid)
      {
        Network.Get().RequestNearbyFSGs(this.Latitude, this.Longitude, this.GpsAccuracy, FiresideGatheringManager.IsWifiFeatureEnabled ? this.BSSIDS : (List<string>) null);
      }
      else
      {
        if (!FiresideGatheringManager.IsWifiFeatureEnabled)
          return;
        Network.Get().RequestNearbyFSGs(this.BSSIDS);
      }
    }
  }

  public void InnkeeperSetupFSG(bool provideWifiForTavern)
  {
    Log.FiresideGatherings.Print("Doing Innkeeper FSG Setup");
    if (this.m_innkeeperFSG == null)
    {
      Log.FiresideGatherings.PrintError("FiresideGatheringManager.InnkeeperSetupFSG tried to setup an FSG but no valid FSG exists");
    }
    else
    {
      long fsgId = this.m_innkeeperFSG.FsgId;
      if (this.m_gpsCheatingLocation)
        Network.Get().InnkeeperSetupFSG(this.m_gpsCheatLatitude, this.m_gpsCheatLongitude, 0.0, FiresideGatheringManager.IsWifiFeatureEnabled & provideWifiForTavern ? this.BSSIDS : (List<string>) null, fsgId);
      else if (this.IsGpsLocationValid)
      {
        Network.Get().InnkeeperSetupFSG(this.Latitude, this.Longitude, this.GpsAccuracy, FiresideGatheringManager.IsWifiFeatureEnabled & provideWifiForTavern ? this.BSSIDS : (List<string>) null, fsgId);
      }
      else
      {
        if (!FiresideGatheringManager.IsWifiFeatureEnabled)
          return;
        Network.Get().InnkeeperSetupFSG(provideWifiForTavern ? this.BSSIDS : (List<string>) null, fsgId);
      }
    }
  }

  public void RequestFSGNotificationAndCheckinsHalt() => this.m_haltFSGNotificationsAndCheckins = true;

  public void ShowFiresideGatheringInnkeeperSetupDialog()
  {
    ChatMgr.Get().CloseChatUI();
    string tavernNameTavernSign = this.GetTavernName_TavernSign(this.m_innkeeperFSG);
    DialogManager.Get().ShowFiresideGatheringInnkeeperSetupDialog(new FiresideGatheringInnkeeperSetupDialog.ResponseCallback(this.ShowFiresideGatheringInnkeeperSetup_OnResponse), tavernNameTavernSign);
  }

  public void ShowInnkeeperSetupTooltip() => this.ShowTooltip(GameStrings.Get("GLUE_FIRESIDE_GATHERING_INNKEEPER_TOOLTIP"));

  public bool InBrawlMode() => this.CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL || this.CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE_BRAWL;

  public static bool IsValidBSSID(string bssid)
  {
    bool flag1 = false;
    for (int index = 0; index < bssid.Length; ++index)
    {
      char ch = bssid[index];
      bool flag2 = ch == ':';
      bool flag3 = ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f' || ch >= 'A' && ch <= 'F';
      if (!(flag2 | flag3))
        return false;
      flag1 = flag1 || !flag2 && ch != '0';
    }
    return flag1;
  }

  public void EnableTransitionInputBlocker(bool enabled)
  {
    if ((UnityEngine.Object) this.m_transitionInputBlocker == (UnityEngine.Object) null)
      this.InitializeTransitionInputBlocker(enabled);
    else
      this.m_transitionInputBlocker.gameObject.SetActive(enabled);
  }

  public void TransitionToFSGSceneIfSafe()
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.LOGIN:
        SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoadedDuringAutoCheckin));
        break;
      case SceneMgr.Mode.HUB:
      case SceneMgr.Mode.DRAFT:
      case SceneMgr.Mode.ADVENTURE:
      case SceneMgr.Mode.TAVERN_BRAWL:
        if (PopupDisplayManager.Get().IsShowing || StoreManager.Get().IsShownOrWaitingToShow())
          break;
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.FIRESIDE_GATHERING);
        this.EnableTransitionInputBlocker(true);
        break;
    }
  }

  private double Latitude
  {
    get
    {
      if (this.m_locationData == null || this.m_locationData.location == null)
        return 0.0;
      double latitude = this.m_locationData.location.Latitude;
      if (this.m_gpsCheatOffset != 0.0)
        latitude += 57.2957763671875 * (this.m_gpsCheatOffset / 6378137.0);
      return latitude;
    }
  }

  private double Longitude => this.m_locationData == null || this.m_locationData.location == null ? 0.0 : this.m_locationData.location.Longitude;

  private double GpsAccuracy => this.m_locationData == null || this.m_locationData.location == null ? -1.0 : this.m_locationData.location.Accuracy;

  public bool IsGpsLocationValid
  {
    get
    {
      if (!FiresideGatheringManager.IsGpsFeatureEnabled || this.m_locationData == null || this.m_locationData.location == null)
        return false;
      FSGFeatureConfig fsgFeatureConfig = FiresideGatheringManager.s_fsgFeaturesConfig.Value;
      return fsgFeatureConfig != null && this.m_locationData.location.Accuracy <= (double) fsgFeatureConfig.MaxAccuracy;
    }
  }

  private List<string> BSSIDS => this.m_accumulatedAccessPoints.Select<KeyValuePair<string, AccessPointInfo>, string>((Func<KeyValuePair<string, AccessPointInfo>, string>) (kv => kv.Key)).ToList<string>();

  [CustomEditField(Hide = true)]
  public bool AutoCheckInEnabled
  {
    get
    {
      NetCache.NetCacheFeatures netCacheFeatures = FiresideGatheringManager.s_guardianVars.Value;
      if ((netCacheFeatures == null ? 0 : (netCacheFeatures.FSGAutoCheckinEnabled ? 1 : 0)) == 0)
        return false;
      FSGFeatureConfig fsgFeatureConfig = FiresideGatheringManager.s_fsgFeaturesConfig.Value;
      return fsgFeatureConfig != null && fsgFeatureConfig.AutoCheckin;
    }
  }

  [CustomEditField(Hide = true)]
  public int FriendListPatronCountLimit
  {
    get
    {
      NetCache.NetCacheFeatures netCacheFeatures = FiresideGatheringManager.s_guardianVars.Value;
      return netCacheFeatures == null || netCacheFeatures.FSGFriendListPatronCountLimit < 0 ? 30 : netCacheFeatures.FSGFriendListPatronCountLimit;
    }
  }

  public ReactiveBoolOption PlayerAccountShouldAutoCheckin { get; set; } = ReactiveBoolOption.CreateInstance(Option.SHOULD_AUTO_CHECK_IN_TO_FIRESIDE_GATHERINGS);

  public ReactiveBoolOption HasManuallyInitiatedFSGScanBefore { get; set; } = ReactiveBoolOption.CreateInstance(Option.HAS_INITIATED_FIRESIDE_GATHERING_SCAN);

  public ReactiveLongOption LastTavernID { get; set; } = new ReactiveLongOption(Option.LAST_TAVERN_JOINED);

  private void OnLocationDataComplete()
  {
    NetCache netCache = NetCache.Get();
    if (netCache == null)
      return;
    NetCache.NetCacheFeatures netObject = netCache.GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject == null || !netObject.FSGLoginScanEnabled)
      return;
    this.RequestNearbyFSGs();
  }

  private void AutoInnkeeperSetup()
  {
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    if (!this.m_doAutoInnkeeperSetup || this.IsCheckedIn || this.m_innkeeperFSG == null || this.m_innkeeperFSG.IsSetupComplete || mode != SceneMgr.Mode.HUB || SceneMgr.Get().IsTransitioning() || this.m_checkInDialogShown || PopupDisplayManager.Get().IsShowing)
      return;
    this.m_doAutoInnkeeperSetup = false;
    this.m_haltAutoCheckinWhileInnkeeperSetup = true;
    this.ShowFiresideGatheringInnkeeperSetupDialog();
  }

  private bool CanAutoCheckInEventually => this.AutoCheckInEnabled && this.PlayerAccountShouldAutoCheckin.Value && !this.m_errorOccuredOnCheckin && GameUtils.IsTraditionalTutorialComplete();

  private void AutoCheckIn()
  {
    if (this.IsCheckedIn || !this.CanAutoCheckInEventually)
      return;
    FSGConfig preferredFsg = this.GetPreferredFSG();
    if (preferredFsg == null || this.m_checkInRequestPending || this.m_checkInDialogShown || this.m_haltAutoCheckinWhileInnkeeperSetup || preferredFsg.IsInnkeeper && !preferredFsg.IsSetupComplete)
      return;
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    switch (mode)
    {
      case SceneMgr.Mode.GAMEPLAY:
        break;
      case SceneMgr.Mode.COLLECTIONMANAGER:
        break;
      default:
        if (StoreManager.Get().IsShownOrWaitingToShow() || PopupDisplayManager.Get().IsShowing)
          break;
        if (mode == SceneMgr.Mode.LOGIN || mode == SceneMgr.Mode.STARTUP || mode == SceneMgr.Mode.HUB)
        {
          this.CheckInToFSG(preferredFsg.FsgId);
          break;
        }
        DialogManager.Get().ShowFiresideGatheringNearbyDialog(new FiresideGatheringJoinDialog.ResponseCallback(this.OnJoinFSGDialogResponse));
        this.m_checkInDialogShown = true;
        break;
    }
  }

  private FSGConfig GetPreferredFSG()
  {
    if (this.m_nearbyFSGs.Count < 1)
      return (FSGConfig) null;
    FSGConfig fsgConfig = (FSGConfig) null;
    for (int index = 1; index < this.m_nearbyFSGs.Count; ++index)
    {
      FSGConfig nearbyFsG = this.m_nearbyFSGs[0];
      if (nearbyFsG.IsInnkeeper && nearbyFsG.IsSetupComplete)
        return nearbyFsG;
      if (nearbyFsG.FsgId == this.LastTavernID.Value)
        fsgConfig = nearbyFsG;
    }
    return fsgConfig ?? this.m_nearbyFSGs[0];
  }

  private void NotifyFSGNearbyIfNeeded()
  {
    if (this.IsCheckedIn || this.m_checkInRequestPending || this.AutoCheckInEnabled || this.PlayerAccountShouldAutoCheckin.Value || this.m_nearbyFSGsFoundEventSent || this.m_haltAutoCheckinWhileInnkeeperSetup || !this.m_fsgAvailableToCheckin || (UnityEngine.Object) this.m_tooltipShowing != (UnityEngine.Object) null || this.m_nearbyFSGs.Count <= 0)
      return;
    this.NotifyFSGNearby();
  }

  private void NotifyFSGNearby()
  {
    this.m_nearbyFSGsFoundEventSent = true;
    this.ShowNearbyFSGsTooltip();
    if (this.OnNearbyFSGs == null)
      return;
    this.OnNearbyFSGs();
  }

  private void ShowNearbyFSGsTooltip() => this.ShowTooltip(GameStrings.Get("GLUE_FSG_NEARBY_TOOLTIP"));

  private void ShowTooltip(string text, float? durationSeconds = 6f)
  {
    Vector3 vector3 = BaseUI.Get().m_BnetBar.m_friendButton.transform.position + (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_nearbyFiresidePopupOffset;
    Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, Vector3.zero, (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_nearbyFiresidePopupScale, text);
    Notification.PopUpArrowDirection direction = (bool) UniversalInputManager.UsePhoneUI ? Notification.PopUpArrowDirection.LeftUp : Notification.PopUpArrowDirection.LeftDown;
    popupText.ShowPopUpArrow(direction);
    popupText.PulseReminderEveryXSeconds(2f);
    popupText.transform.position = vector3;
    popupText.transform.localEulerAngles = (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_nearbyFiresidePopupRotation;
    LayerUtils.SetLayer(popupText.gameObject, GameLayer.BattleNet);
    this.m_tooltipShowing = popupText;
    if (!durationSeconds.HasValue)
      return;
    Processor.RunCoroutine(this.Tooltip_End(durationSeconds.Value, popupText));
  }

  private IEnumerator Tooltip_End(float secondsTillDeath, Notification notice)
  {
    FiresideGatheringManager gatheringManager = this;
    if (!((UnityEngine.Object) notice == (UnityEngine.Object) null))
    {
      if ((double) secondsTillDeath > 0.0)
        yield return (object) new WaitForSeconds(secondsTillDeath);
      PegUI.OnReleasePreTrigger -= new System.Action<PegUIElement>(gatheringManager.PegUI_OnReleasePreTrigger);
      if ((UnityEngine.Object) notice != (UnityEngine.Object) null)
      {
        notice.PlayDeath();
        if ((UnityEngine.Object) notice == (UnityEngine.Object) gatheringManager.m_tooltipShowing)
          gatheringManager.m_tooltipShowing = (Notification) null;
      }
    }
  }

  public void ShowReturnToFSGSceneTooltip()
  {
    if (Box.Get().IsTransitioningToSceneMode())
      Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.ShowReturnToFSGSceneTooltipOnTransitionToBoxFinished));
    else
      this.ShowReturnToFSGSceneTooltipOnTransitionToBoxFinished((object) null);
  }

  private void ShowReturnToFSGSceneTooltipOnTransitionToBoxFinished(object data)
  {
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.ShowReturnToFSGSceneTooltipOnTransitionToBoxFinished));
    if (this.HasSeenReturnToFSGSceneTooltip)
    {
      SocialToastMgr.Get().AddToast(UserAttentionBlocker.NONE, string.Empty, SocialToastMgr.TOAST_TYPE.FIRESIDE_GATHERING_IS_HERE_REMINDER, true);
    }
    else
    {
      this.HasSeenReturnToFSGSceneTooltip = true;
      this.ShowTooltip(GameStrings.Get("GLUE_FIRESIDE_GATHERING_RETURN_TO_SCENE_HERE"), new float?());
      ChatMgr.Get().OnFriendListToggled += new ChatMgr.FriendListToggled(this.ShowReturnToFSGSceneTooltip_OnFriendListToggled_ShowNextTooltip);
      PegUI.OnReleasePreTrigger += new System.Action<PegUIElement>(this.PegUI_OnReleasePreTrigger);
    }
  }

  private void ShowReturnToFSGSceneTooltip_OnFriendListToggled_ShowNextTooltip(bool open)
  {
    if (!open)
      return;
    ChatMgr.Get().OnFriendListToggled -= new ChatMgr.FriendListToggled(this.ShowReturnToFSGSceneTooltip_OnFriendListToggled_ShowNextTooltip);
    this.CloseTooltip();
    if (!this.IsCheckedIn)
      return;
    System.Action action = (System.Action) (() =>
    {
      FriendListFSGFrame firstRenderedItem = ChatMgr.Get().FriendListFrame.FindFirstRenderedItem<FriendListFSGFrame>();
      if ((UnityEngine.Object) firstRenderedItem == (UnityEngine.Object) null)
        return;
      PegUI.OnReleasePreTrigger += new System.Action<PegUIElement>(this.PegUI_OnReleasePreTrigger);
      ChatMgr.Get().FriendListFrame.items.Scrolled += new System.Action(this.CloseTooltip);
      this.m_tooltipShowing = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, Vector3.zero, (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_nearbyFiresidePopupScale, GameStrings.Get("GLUE_FIRESIDE_GATHERING_RETURN_TO_SCENE_HERE"));
      this.m_tooltipShowing.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
      this.m_tooltipShowing.PulseReminderEveryXSeconds(2f);
      this.m_tooltipShowing.transform.position = firstRenderedItem.transform.position + (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_returnToFsgFriendListPopupOffset;
      this.m_tooltipShowing.transform.localEulerAngles = (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_nearbyFiresidePopupRotation;
      LayerUtils.SetLayer(this.m_tooltipShowing.gameObject, GameLayer.BattleNet);
      Processor.RunCoroutine(this.Tooltip_End(6f, this.m_tooltipShowing));
    });
    if (ChatMgr.Get().FriendListFrame.IsStarted)
      action();
    else
      ChatMgr.Get().FriendListFrame.OnStarted += action;
  }

  private void CloseTooltip()
  {
    PegUI.OnReleasePreTrigger -= new System.Action<PegUIElement>(this.PegUI_OnReleasePreTrigger);
    if ((UnityEngine.Object) ChatMgr.Get().FriendListFrame != (UnityEngine.Object) null)
      ChatMgr.Get().FriendListFrame.items.Scrolled -= new System.Action(this.CloseTooltip);
    if (!((UnityEngine.Object) this.m_tooltipShowing != (UnityEngine.Object) null))
      return;
    this.m_tooltipShowing.CloseWithoutAnimation();
  }

  private void PegUI_OnReleasePreTrigger(PegUIElement elem) => this.CloseTooltip();

  private void OnFriendListClosed_CloseTooltip(bool opened)
  {
    if (opened)
      return;
    this.CloseTooltip();
  }

  private void SceneMgr_OnScenePreUnload(
    SceneMgr.Mode prevMode,
    PegasusScene prevScene,
    object userData)
  {
    if (prevMode == SceneMgr.Mode.FIRESIDE_GATHERING)
      return;
    this.CloseTooltip();
  }

  private void DoStartAndEndTimingEvents()
  {
    if (this.m_nearbyFSGs.Count == 0)
      return;
    long timestampSeconds = TimeUtils.UnixTimestampSeconds;
    for (int index = this.m_nearbyFSGs.Count - 1; index >= 0; --index)
    {
      FSGConfig nearbyFsG = this.m_nearbyFSGs[index];
      if (nearbyFsG.UnixEndTimeWithSlush < timestampSeconds)
      {
        this.m_nearbyFSGs.RemoveAt(index);
        if (nearbyFsG == this.m_currentFSG)
          this.CheckOutOfFSG();
      }
    }
  }

  private FiresideGatheringSign GenerateCustomTavernSign(
    int sign,
    int background,
    int major,
    int minor,
    string tavernName)
  {
    FiresideGatheringSign signObject = this.GetSignObject(sign);
    Material material = RendererExtension.GetMaterial((Renderer) signObject.GetShieldMeshRenderer());
    signObject.GetComponentInChildren<UberText>().Text = tavernName;
    AssetHandle<Texture> assetHandle1 = AssetLoader.Get().LoadAsset<Texture>(FiresideGatheringManager.m_backgroundTextures[background - 1]);
    AssetHandle<Texture> assetHandle2 = AssetLoader.Get().LoadAsset<Texture>(FiresideGatheringManager.m_majorTextures[major - 1]);
    AssetHandle<Texture> assetHandle3 = AssetLoader.Get().LoadAsset<Texture>(FiresideGatheringManager.m_minorTextures[minor - 1]);
    material.SetTexture("_BackgroundTex", (Texture) assetHandle1);
    material.SetTexture("_MajorTex", (Texture) assetHandle2);
    material.SetTexture("_MinorTex", (Texture) assetHandle3);
    DisposablesCleaner disposablesCleaner = ServiceManager.Get<DisposablesCleaner>();
    disposablesCleaner?.Attach((Component) signObject, (IDisposable) assetHandle1);
    disposablesCleaner?.Attach((Component) signObject, (IDisposable) assetHandle2);
    disposablesCleaner?.Attach((Component) signObject, (IDisposable) assetHandle3);
    return signObject;
  }

  private void ShowSign(
    TavernSignData signData,
    string tavernName,
    FiresideGatheringManager.OnCloseSign callback)
  {
    if ((UnityEngine.Object) this.m_currentSign != (UnityEngine.Object) null)
      this.HideFSGSign();
    this.ShowSign(signData, tavernName, callback, new PrefabCallback<GameObject>(this.OnSignAssetLoaded));
  }

  private void ShowSmallSign(TavernSignData signData, string tavernName) => this.ShowSign(signData, tavernName, (FiresideGatheringManager.OnCloseSign) null, new PrefabCallback<GameObject>(this.OnSmallSignAssetLoaded));

  private void ShowSign(
    TavernSignData signData,
    string tavernName,
    FiresideGatheringManager.OnCloseSign callback,
    PrefabCallback<GameObject> onSignAssetLoadedCallback)
  {
    this.m_currentSignCallback = callback;
    this.LastSign = signData;
    if (signData.SignType == TavernSignType.TAVERN_SIGN_TYPE_CUSTOM)
    {
      FiresideGatheringSign customTavernSign = this.GenerateCustomTavernSign(signData.Sign, signData.Background, signData.Major, signData.Minor, tavernName);
      onSignAssetLoadedCallback((AssetReference) "", customTavernSign.gameObject, (object) null);
    }
    else
    {
      FiresideGatheringManagerData.SignTypeMapping signTypeMapping = this.Data.m_signTypeMapping.Find((Predicate<FiresideGatheringManagerData.SignTypeMapping>) (x => x.m_type == signData.SignType));
      if (signTypeMapping == null || signTypeMapping.m_prefabName == null)
        Error.AddDevFatal("FiresideGatheringManager.ShowSign() - unhandled sign type {0}", (object) signData.SignType);
      else
        AssetLoader.Get().InstantiatePrefab((AssetReference) signTypeMapping.m_prefabName, onSignAssetLoadedCallback);
    }
  }

  private void OnSignAssetLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    FiresideGatheringSign component = go.GetComponent<FiresideGatheringSign>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    this.m_currentSign = component;
    component.OnDestroyEvent += new FiresideGatheringSign.OnDestroyCallback(this.OnSignHidden);
    go.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_signPosition;
    go.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_signScale;
    LayerUtils.SetLayer(go, GameLayer.IgnoreFullScreenEffects);
    SoundManager.Get().LoadAndPlay((AssetReference) "GVG_sign_enter.prefab:68c9d25c4da293b4dba44c37615c0ae0");
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective with
    {
      Blur = new BlurParameters(brightness: 1f)
    });
    this.PlaySignTween(go);
  }

  private void OnSmallSignAssetLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    FiresideGatheringSign component = go.GetComponent<FiresideGatheringSign>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    this.m_currentSign = component;
    if ((UnityEngine.Object) this.m_smallSignContainer == (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) component);
    go.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_signScale;
    go.transform.SetParent(this.m_smallSignContainer, false);
    go.transform.localPosition = Vector3.zero;
    LayerUtils.SetLayer(go, GameLayer.Default);
    component.SetSignShadowEnabled(true);
  }

  private FiresideGatheringSign GetSignObject(int signIndex)
  {
    if (signIndex < 1 || signIndex > 8)
    {
      Log.FiresideGatherings.PrintError("FiresideGatheringManager.GetSignObject passed an invalid sign index: {0}. Using default of 1", (object) signIndex);
      signIndex = 1;
    }
    FiresideGatheringSign component = ((GameObject) GameUtils.InstantiateGameObject(FiresideGatheringManager.m_tavernSignAsset.ToString())).GetComponent<FiresideGatheringSign>();
    GameObject child = (GameObject) GameUtils.InstantiateGameObject(FiresideGatheringManager.m_fsgShields[signIndex - 1].ToString());
    GameUtils.SetParent(child, (Component) component.m_shieldContainer);
    component.SetSignShield(child.GetComponentInChildren<FiresideGatheringSignShield>());
    return component;
  }

  private void PlaySignTween(GameObject signObject)
  {
    Hashtable args = iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) 0.25f, (object) "oncomplete", (object) (System.Action<object>) (e => this.PlaySignAnimation(e)), (object) "oncompleteparams", (object) iTween.Hash((object) "sign", (object) signObject));
    iTween.ScaleFrom(signObject, args);
    Processor.RunCoroutine(this.CreateSignInputBlocker(signObject));
  }

  private void PlaySignAnimation(object args)
  {
    Animator componentInChildren = ((GameObject) ((Hashtable) args)[(object) "sign"]).GetComponentInChildren<Animator>();
    componentInChildren.enabled = true;
    componentInChildren.Play("FSG_SignSwing");
    if (this.OnSignShown == null)
      return;
    this.OnSignShown();
  }

  private IEnumerator CreateSignInputBlocker(GameObject signObject)
  {
    GameObject inputBlockerObject = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(GameLayer.UI), "FSGSign");
    inputBlockerObject.transform.parent = signObject.transform;
    inputBlockerObject.transform.localPosition = new Vector3(0.0f, 1f, 0.0f);
    PegUIElement fsgSignBlocker = inputBlockerObject.AddComponent<PegUIElement>();
    yield return (object) new WaitForSeconds(2f);
    fsgSignBlocker.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (_ =>
    {
      this.HideFSGSign();
      UnityEngine.Object.Destroy((UnityEngine.Object) inputBlockerObject);
    }));
  }

  private void HideFSGSign(bool hideImmediately = false)
  {
    if ((UnityEngine.Object) this.m_currentSign == (UnityEngine.Object) null)
    {
      this.OnSignHidden();
    }
    else
    {
      this.m_currentSign.gameObject.SetActive(!hideImmediately);
      this.m_currentSign.m_fxMotes.gameObject.SetActive(false);
      if (!hideImmediately)
        SoundManager.Get().LoadAndPlay((AssetReference) "GVG_sign_exit.prefab:697b23cceecfd154dacf14bc58b75af2");
      this.HideSignAnim(this.m_currentSign.gameObject);
    }
  }

  public void OnTavernSignAnimationComplete()
  {
    if ((UnityEngine.Object) this.m_currentSign != (UnityEngine.Object) null)
      this.m_currentSign.UnregisterSignSocketAnimationCompleteListener(new System.Action(this.OnTavernSignAnimationComplete));
    if (this.OnSignClosed == null)
      return;
    this.OnSignClosed();
  }

  private void HideSignAnim(GameObject sign)
  {
    Animator componentInChildren = sign.GetComponentInChildren<Animator>();
    componentInChildren.enabled = true;
    componentInChildren.Play((bool) UniversalInputManager.UsePhoneUI ? "FSG_SignSocketIn_phone" : "FSG_SignSocketIn");
    LayerUtils.SetLayer(sign, GameLayer.Default);
    this.OnSignHidden();
    sign.GetComponent<FiresideGatheringSign>().RegisterSignSocketAnimationCompleteListener(new System.Action(this.OnTavernSignAnimationComplete));
  }

  private void OnSignHidden()
  {
    this.m_currentSign = (FiresideGatheringSign) null;
    if (this.m_currentSignCallback != null)
    {
      this.m_currentSignCallback();
      this.m_currentSignCallback = (FiresideGatheringManager.OnCloseSign) null;
    }
    this.HideBlur();
  }

  private void HideBlur() => this.m_screenEffectsHandle.StopEffect();

  private void OnJoinFSGDialogResponse(bool joinFSG)
  {
    if (!joinFSG)
    {
      this.PlayerAccountShouldAutoCheckin.Set(false);
    }
    else
    {
      FSGConfig preferredFsg = this.GetPreferredFSG();
      if (preferredFsg == null)
        return;
      this.CheckInToFSG(preferredFsg.FsgId);
      if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        return;
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
  }

  private void OnFindEventDialogCallBack(bool searchForGatherings)
  {
    if (searchForGatherings)
    {
      if (PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.GEOLOCATION))
      {
        this.OnFSGAllowed();
      }
      else
      {
        PrivacyFeaturesPopup privacyPopup = AssetLoader.Get().InstantiatePrefab((AssetReference) "PrivacyPopups.prefab:99a8f571a8a35a54e90790c904bc94f8").GetComponent<PrivacyFeaturesPopup>();
        privacyPopup.Set(PrivacyFeatures.GEOLOCATION, false, (System.Action) (() => PrivacyGate.Get().SetFeature(PrivacyFeatures.GEOLOCATION, true)), (System.Action) (() =>
        {
          this.OnFSGAllowed();
          this.ClosePrivacyPopup(privacyPopup);
        }), (System.Action) (() => this.ClosePrivacyPopup(privacyPopup)));
        privacyPopup.Show();
      }
    }
    else
      this.GotoFSGLink();
  }

  private void OnFSGAllowed()
  {
    this.HasManuallyInitiatedFSGScanBefore.Set(true);
    if (!ClientLocationManager.Get().GPSOrWifiServicesAvailable)
      this.ShowNoGPSOrWifiAlertPopup();
    else
      DialogManager.Get().ShowFiresideGatheringLocationHelperDialog((System.Action) null);
  }

  private void ClosePrivacyPopup(PrivacyFeaturesPopup privacyPopup)
  {
    privacyPopup.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) privacyPopup.gameObject, 1f);
  }

  private void ShowNoGPSOrWifiAlertPopup() => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLOBAL_FIRESIDE_GATHERING"),
    m_text = GameStrings.Get("GLUE_FIRESIDE_GATHERING_SCAN_NO_GPS_OR_WIFI"),
    m_responseDisplay = AlertPopup.ResponseDisplay.OK,
    m_alertTextAlignment = UberText.AlignmentOptions.Center
  });

  private void OnFailedToFindFSGDialogResponse(AlertPopup.Response response, object userData)
  {
    if (response != AlertPopup.Response.CONFIRM)
      return;
    this.GotoFSGLink();
  }

  private void ShowFiresideGatheringInnkeeperSetup_OnResponse(bool doSetup)
  {
    this.m_haltAutoCheckinWhileInnkeeperSetup = false;
    if (doSetup)
      DialogManager.Get().ShowFiresideGatheringInnkeeperSetupHelperDialog((System.Action) null);
    else
      this.PlayerAccountShouldAutoCheckin.Set(false);
  }

  private void OnRequestNearbyFSGsResponse()
  {
    this.m_isRequestNearbyFSGsPending = false;
    RequestNearbyFSGsResponse response = Network.Get().GetRequestNearbyFSGsResponse();
    if (response.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.FiresideGatherings.PrintError("NearbyFSGsResponse: code={0} {1} fsgCount={2}", (object) (int) response.ErrorCode, (object) response.ErrorCode, (object) response.FSGs.Count);
      if (this.OnNearbyFSGsChanged == null)
        return;
      this.OnNearbyFSGsChanged();
    }
    else
    {
      Log.FiresideGatherings.Print("NearbyFSGsResponse: code={0} {1} fsgCount={2}", (object) (int) response.ErrorCode, (object) response.ErrorCode, (object) response.FSGs.Count);
      this.m_nearbyFSGs.Clear();
      this.m_innkeeperFSG = (FSGConfig) null;
      this.m_fsgAvailableToCheckin = false;
      for (int index = 0; index < response.FSGs.Count; ++index)
      {
        FSGConfig fsG = response.FSGs[index];
        this.m_nearbyFSGs.Add(fsG);
        if (fsG.IsInnkeeper)
          this.m_innkeeperFSG = fsG;
        else
          this.m_fsgAvailableToCheckin = true;
        this.AddKnownInnkeeper(fsG.FsgId, fsG.FsgInnkeeperAccountId);
      }
      if (response.HasCheckedInFsgId)
      {
        FSGConfig fsgConfig = this.m_nearbyFSGs.FirstOrDefault<FSGConfig>((Func<FSGConfig, bool>) (fsg => fsg.FsgId == response.CheckedInFsgId));
        if (fsgConfig == null)
        {
          Log.FiresideGatherings.PrintError("NearbyFSGsResponse: Error: already checked into FSG (id={0}) but no corresponding FSGConfig found in nearby list - ignoring. patronCount={1}", (object) response.CheckedInFsgId, (object) response.FsgAttendees.Count);
        }
        else
        {
          Log.FiresideGatherings.Print("NearbyFSGsResponse: already checked into {0}-{1}, showing FSG UI. patronCount={2}", (object) response.CheckedInFsgId, string.IsNullOrEmpty(fsgConfig.TavernName) ? (object) "<no name>" : (object) fsgConfig.TavernName, (object) response.FsgAttendees.Count);
          this.JoinFSG(response.CheckedInFsgId, response.FsgAttendees, response.FsgSharedSecretKey, response.InnkeeperSelectedBrawlLibraryItemId);
        }
      }
      if (this.OnNearbyFSGsChanged == null)
        return;
      this.OnNearbyFSGsChanged();
    }
  }

  private void OnCheckInToFSGResponse()
  {
    this.m_checkInRequestPending = false;
    CheckInToFSGResponse checkInToFsgResponse = Network.Get().GetCheckInToFSGResponse();
    if (checkInToFsgResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK && checkInToFsgResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_FSG_ALREADY_CHECKED_IN_FETCH_FSG_INFO)
    {
      Log.FiresideGatherings.PrintError("CheckInResponse: code={0} {1} fsgId={2} patronCount={3}", (object) (int) checkInToFsgResponse.ErrorCode, (object) checkInToFsgResponse.ErrorCode, (object) checkInToFsgResponse.FsgId, (object) checkInToFsgResponse.FsgAttendees.Count);
      this.m_errorOccuredOnCheckin = true;
    }
    else
    {
      Log.FiresideGatherings.Print("CheckInResponse: code={0} {1} fsgId={2} patronCount={3}", (object) (int) checkInToFsgResponse.ErrorCode, (object) checkInToFsgResponse.ErrorCode, (object) checkInToFsgResponse.FsgId, checkInToFsgResponse.FsgAttendees == null ? (object) "null" : (object) checkInToFsgResponse.FsgAttendees.Count.ToString());
      FriendChallengeMgr.Get().UpdateMyFsgSharedSecret(checkInToFsgResponse.FsgSharedSecretKey);
      this.JoinFSG(checkInToFsgResponse.FsgId, checkInToFsgResponse.FsgAttendees, checkInToFsgResponse.FsgSharedSecretKey, checkInToFsgResponse.InnkeeperSelectedBrawlLibraryItemId);
    }
  }

  private void OnCheckOutOfFSGResponse()
  {
    CheckOutOfFSGResponse outOfFsgResponse = Network.Get().GetCheckOutOfFSGResponse();
    if (outOfFsgResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.FiresideGatherings.PrintError("CheckOutResponse: code={0} {1} fsgId={2}", (object) (int) outOfFsgResponse.ErrorCode, (object) outOfFsgResponse.ErrorCode, (object) outOfFsgResponse.FsgId);
    }
    else
    {
      Log.FiresideGatherings.Print("CheckOutResponse: code={0} {1} fsgId={2}", (object) (int) outOfFsgResponse.ErrorCode, (object) outOfFsgResponse.ErrorCode, (object) outOfFsgResponse.FsgId);
      FriendChallengeMgr.Get().UpdateMyFsgSharedSecret((byte[]) null);
      this.LeaveFSG();
    }
  }

  private void CheckCanBeginLocationDataGatheringForLogin()
  {
    if (this.m_hasBegunLocationDataGatheringForLogin)
      return;
    Log.FiresideGatherings.PrintDebug("FiresideGatheringManager.CheckCanBeginLocationGathering");
    if (FiresideGatheringManager.s_guardianVars.Value == null)
      Log.FiresideGatherings.PrintDebug("FiresideGatheringManager.CheckCanBeginLocationGathering NO GUARDIAN");
    else if (FiresideGatheringManager.s_fsgFeaturesConfig.Value == null)
      Log.FiresideGatherings.PrintDebug("FiresideGatheringManager.CheckCanBeginLocationGathering NO FEATURE CONFIG");
    else if (FiresideGatheringManager.s_profileProgress.Value == null)
      Log.FiresideGatherings.PrintDebug("FiresideGatheringManager.CheckCanBeginLocationGathering NO PROFILE PROGRESS");
    else if (FiresideGatheringManager.s_clientOptions.Value == null)
      Log.FiresideGatherings.PrintDebug("FiresideGatheringManager.CheckCanBeginLocationGathering NO CLIENT OPTIONS");
    else
      this.BeginLocationDataGatheringForLogin();
  }

  private void OnNetCache_GuardianVars()
  {
    NetCache.NetCacheFeatures netCacheFeatures = FiresideGatheringManager.s_guardianVars.Value;
    if (netCacheFeatures.FSGEnabled == FiresideGatheringManager.s_cacheFSGEnabled)
      return;
    if (!netCacheFeatures.FSGEnabled && this.IsCheckedIn)
    {
      this.CheckOutOfFSG();
      this.LeaveFSG();
      this.m_nearbyFSGs.Clear();
    }
    FiresideGatheringManager.s_cacheFSGEnabled = netCacheFeatures.FSGEnabled;
    this.CheckCanBeginLocationDataGatheringForLogin();
  }

  private void OnNetCache_FSGFeatureConfig()
  {
    FSGFeatureConfig fsgFeatureConfig = FiresideGatheringManager.s_fsgFeaturesConfig.Value;
    if (fsgFeatureConfig.Gps == FiresideGatheringManager.s_cacheGPSEnabled && fsgFeatureConfig.Wifi == FiresideGatheringManager.s_cacheWifiEnabled)
      return;
    FiresideGatheringManager.s_cacheGPSEnabled = fsgFeatureConfig.Gps;
    FiresideGatheringManager.s_cacheWifiEnabled = fsgFeatureConfig.Wifi;
    this.CheckCanBeginLocationDataGatheringForLogin();
  }

  private void RebuildKnownPatronsFromPresence()
  {
    this.m_knownPatronsFromPresence.Clear();
    if (!this.IsCheckedIn)
      return;
    long num = this.m_currentFSG == null ? 0L : this.m_currentFSG.FsgId;
    foreach (BnetPlayer friend in BnetFriendMgr.Get().GetFriends())
    {
      FiresideGatheringInfo playerFsgInfo = FiresideGatheringManager.GetPlayerFSGInfo(friend);
      if (playerFsgInfo != null && playerFsgInfo.FsgId == num)
        this.AddKnownPatronFromPresence(friend);
    }
  }

  private void AddKnownPatronFromPresence(BnetPlayer player)
  {
    if (this.m_knownPatronsFromPresence.Contains(player))
      return;
    this.AddKnownPatron(BnetUtils.CreatePegasusBnetId((BnetEntityId) player.GetAccountId()), BnetUtils.CreatePegasusBnetId((BnetEntityId) player.GetHearthstoneGameAccountId()), true, out bool _);
  }

  private void PlayersPresenceChanged(
    BnetPlayerChangelist changelist,
    out List<BnetPlayer> addedToDisplayablePatronList,
    out List<BnetPlayer> removedFromDisplayablePatronList)
  {
    addedToDisplayablePatronList = (List<BnetPlayer>) null;
    removedFromDisplayablePatronList = (List<BnetPlayer>) null;
    List<BnetPlayer> bnetPlayerList = new List<BnetPlayer>();
    BnetAccountId accountId = BnetPresenceMgr.Get().GetMyPlayer().GetAccountId();
    foreach (BnetPlayer pendingPatron in this.m_pendingPatrons)
    {
      if ((BnetEntityId) pendingPatron.GetAccountId() != (BnetEntityId) accountId && FiresideGatheringPresenceManager.IsDisplayable(pendingPatron) && !this.IsPlayerInMyFSGAndDisplayable(pendingPatron))
      {
        int num = this.m_displayablePatrons.Add(pendingPatron) ? 1 : 0;
        bnetPlayerList.Add(pendingPatron);
        if (num != 0)
        {
          if (addedToDisplayablePatronList == null)
            addedToDisplayablePatronList = new List<BnetPlayer>();
          addedToDisplayablePatronList.Add(pendingPatron);
        }
      }
    }
    foreach (BnetPlayer bnetPlayer in bnetPlayerList)
      this.m_pendingPatrons.Remove(bnetPlayer);
    bnetPlayerList.Clear();
    long num1 = this.m_currentFSG == null ? 0L : this.m_currentFSG.FsgId;
    List<BnetPlayerChange> bnetPlayerChangeList = changelist == null ? (List<BnetPlayerChange>) null : changelist.GetChanges();
    if (bnetPlayerChangeList == null)
      return;
    for (int index = 0; index < bnetPlayerChangeList.Count; ++index)
    {
      BnetPlayerChange bnetPlayerChange = bnetPlayerChangeList[index];
      BnetPlayer newPlayer = bnetPlayerChange.GetNewPlayer();
      int num2 = (BnetEntityId) newPlayer.GetAccountId() == (BnetEntityId) accountId ? 1 : 0;
      if (num2 == 0)
      {
        bool flag = false;
        FiresideGatheringInfo playerFsgInfo = FiresideGatheringManager.GetPlayerFSGInfo(newPlayer);
        if (playerFsgInfo != null && playerFsgInfo.FsgId == num1)
          this.AddKnownPatronFromPresence(newPlayer);
        else if (this.m_knownPatronsFromPresence.Contains(newPlayer))
        {
          if (!this.m_knownPatronsFromServer.Contains(newPlayer))
          {
            flag = this.m_displayablePatrons.Remove(newPlayer) | flag;
            this.m_pendingPatrons.Remove(newPlayer);
          }
          this.m_knownPatronsFromPresence.Remove(newPlayer);
        }
        if (this.IsPlayerInMyFSGAndDisplayable(newPlayer) && !FiresideGatheringPresenceManager.IsDisplayable(newPlayer))
        {
          flag = this.m_displayablePatrons.Remove(newPlayer) | flag;
          this.m_pendingPatrons.Add(newPlayer);
        }
        if (flag)
        {
          if (removedFromDisplayablePatronList == null)
            removedFromDisplayablePatronList = new List<BnetPlayer>();
          removedFromDisplayablePatronList.Add(newPlayer);
        }
      }
      if (num2 != 0 && !bnetPlayerChange.GetOldPlayer().IsAppearingOffline() && bnetPlayerChange.GetNewPlayer().IsAppearingOffline() && this.IsCheckedIn)
        this.PromptPlayerToAppearOnline(this.CurrentFsgId);
    }
  }

  private void OnInnkeeperSetupGatheringResponse()
  {
    InnkeeperSetupGatheringResponse gatheringResponse = Network.Get().GetInnkeeperSetupGatheringResponse();
    bool success = true;
    if (gatheringResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.FiresideGatherings.PrintError("InnkeeperSetupResponse: code={0} {1} fsgId={2}", (object) (int) gatheringResponse.ErrorCode, (object) gatheringResponse.ErrorCode, (object) gatheringResponse.FsgId);
      success = false;
    }
    Log.FiresideGatherings.Print("InnkeeperSetupResponse: code={0} {1} fsgId={2}", (object) (int) gatheringResponse.ErrorCode, (object) gatheringResponse.ErrorCode, (object) gatheringResponse.FsgId);
    if (success)
      this.m_innkeeperFSG.IsSetupComplete = true;
    if (this.OnInnkeeperSetupFinished != null)
      this.OnInnkeeperSetupFinished(success);
    if (!success)
      return;
    this.CheckInToFSG(this.m_innkeeperFSG.FsgId);
  }

  private BnetPlayer AddKnownPatron(FSGPatron patron, bool isKnownFromPresence) => this.AddKnownPatron(patron.BnetAccount, patron.GameAccount, isKnownFromPresence, out bool _);

  private BnetPlayer AddKnownPatron(
    BnetId bnetAccountId,
    BnetId gameAccountId,
    bool isKnownFromPresence,
    out bool isNewDisplayablePatron)
  {
    isNewDisplayablePatron = false;
    BnetAccountId accountId = BnetPresenceMgr.Get().GetMyPlayer().GetAccountId();
    if ((long) bnetAccountId.Lo == (long) accountId.Low)
      return (BnetPlayer) null;
    BnetAccountId fromNet = BnetAccountId.CreateFromNet(bnetAccountId);
    BnetPlayer player = BnetPresenceMgr.Get().RegisterPlayer(BnetPlayerSource.FSG_PATRON, fromNet, BnetGameAccountId.CreateFromNet(gameAccountId), BnetProgramId.HEARTHSTONE);
    if (player == null)
      return (BnetPlayer) null;
    if (this.m_displayablePatrons.Contains(player))
      isNewDisplayablePatron = false;
    else if (FiresideGatheringPresenceManager.IsDisplayable(player))
    {
      this.m_displayablePatrons.Add(player);
      isNewDisplayablePatron = true;
      this.m_pendingPatrons.Remove(player);
    }
    else
    {
      this.m_pendingPatrons.Add(player);
      isNewDisplayablePatron = false;
    }
    if (isKnownFromPresence)
      this.m_knownPatronsFromPresence.Add(player);
    else
      this.m_knownPatronsFromServer.Add(player);
    return player;
  }

  private void AddKnownInnkeeper(long fsgId, BnetId bnetAccountId)
  {
    if (bnetAccountId == null)
      return;
    BnetAccountId fromNet = BnetAccountId.CreateFromNet(bnetAccountId);
    BnetPlayer bnetPlayer = BnetPresenceMgr.Get().RegisterPlayer(BnetPlayerSource.FSG_PATRON, fromNet, programId: BnetProgramId.HEARTHSTONE);
    if (bnetPlayer == null)
      return;
    if (!this.m_innkeepers.ContainsKey(fsgId))
      this.m_innkeepers.Add(fsgId, bnetPlayer);
    BnetAccountId accountId = BnetPresenceMgr.Get().GetMyPlayer().GetAccountId();
    if ((long) bnetAccountId.Lo == (long) accountId.Low)
      return;
    BnetPresenceMgr.RequestPlayerBattleTag(fromNet);
  }

  private void OnPatronListUpdateReceivedFromServer()
  {
    if (this.m_currentFSG == null || this.CurrentFsgIsLargeScale)
      return;
    bool appendingPatronList = this.m_isAppendingPatronList;
    this.m_isAppendingPatronList = true;
    FSGPatronListUpdate patronListUpdate = Network.Get().GetFSGPatronListUpdate();
    ulong myselfGameAccountLo = BnetPresenceMgr.Get().GetMyPlayer().GetBestGameAccountId().Low;
    patronListUpdate.AddedPatrons.RemoveAll((Predicate<FSGPatron>) (patron => (long) myselfGameAccountLo == (long) patron.GameAccount.Lo));
    List<BnetPlayer> addedToDisplayablePatronList = (List<BnetPlayer>) null;
    List<BnetPlayer> removedFromDisplayablePatronList = (List<BnetPlayer>) null;
    foreach (FSGPatron addedPatron in patronListUpdate.AddedPatrons)
    {
      bool isNewDisplayablePatron;
      BnetPlayer bnetPlayer = this.AddKnownPatron(addedPatron.BnetAccount, addedPatron.GameAccount, false, out isNewDisplayablePatron);
      if (isNewDisplayablePatron)
      {
        if (addedToDisplayablePatronList == null)
          addedToDisplayablePatronList = new List<BnetPlayer>();
        addedToDisplayablePatronList.Add(bnetPlayer);
      }
    }
    foreach (FSGPatron removedPatron in patronListUpdate.RemovedPatrons)
    {
      BnetGameAccountId fromNet = BnetGameAccountId.CreateFromNet(removedPatron.GameAccount);
      BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(fromNet);
      this.m_knownPatronsFromServer.Remove(player);
      bool flag = false;
      if (!this.m_knownPatronsFromPresence.Contains(player))
      {
        flag = this.m_displayablePatrons.Remove(player);
        this.m_pendingPatrons.Remove(player);
      }
      if (flag)
      {
        if (removedFromDisplayablePatronList == null)
          removedFromDisplayablePatronList = new List<BnetPlayer>();
        removedFromDisplayablePatronList.Add(player);
      }
    }
    FiresideGatheringPresenceManager.Get().AddRemovePatronSubscriptions(patronListUpdate.AddedPatrons, patronListUpdate.RemovedPatrons);
    this.m_isAppendingPatronList = appendingPatronList;
    if (FiresideGatheringManager.OnPatronListUpdated == null)
      return;
    FiresideGatheringManager.OnPatronListUpdated(addedToDisplayablePatronList, removedFromDisplayablePatronList);
  }

  private void OnPlayersPresenceChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (!this.IsCheckedIn || this.m_isAppendingPatronList)
      return;
    List<BnetPlayer> addedToDisplayablePatronList;
    List<BnetPlayer> removedFromDisplayablePatronList;
    this.PlayersPresenceChanged(changelist, out addedToDisplayablePatronList, out removedFromDisplayablePatronList);
    FiresideGatheringPresenceManager.Get().CheckForMoreSubscribeOpportunities(removedFromDisplayablePatronList, (IEnumerable<BnetPlayer>) this.m_pendingPatrons);
    if (FiresideGatheringManager.OnPatronListUpdated == null)
      return;
    FiresideGatheringManager.OnPatronListUpdated(addedToDisplayablePatronList, removedFromDisplayablePatronList);
  }

  private void PeriodicCheckForMoreSubscribeOpportunities(object userData)
  {
    if (!this.IsCheckedIn || HearthstoneApplication.Get().IsResetting() || HearthstoneApplication.Get().IsExiting())
      return;
    FiresideGatheringPresenceManager.Get().CheckForMoreSubscribeOpportunities((List<BnetPlayer>) null, (IEnumerable<BnetPlayer>) this.m_pendingPatrons);
    Processor.ScheduleCallback((float) FiresideGatheringPresenceManager.PERIODIC_SUBSCRIBE_CHECK_SECONDS, true, new Processor.ScheduledCallback(this.PeriodicCheckForMoreSubscribeOpportunities));
  }

  private void InitializeTransitionInputBlocker(bool enabled)
  {
    if (!((UnityEngine.Object) this.m_transitionInputBlocker == (UnityEngine.Object) null))
      return;
    this.m_transitionInputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(GameLayer.BattleNetDialog), "FSGTransitionInputBlocker");
    this.m_transitionInputBlocker.transform.SetParent(this.SceneObject.transform);
    TransformUtil.SetPosZ(this.m_transitionInputBlocker, 1f);
    this.m_transitionInputBlocker.gameObject.SetActive(enabled);
  }

  private static FiresideGatheringInfo GetPlayerFSGInfo(BnetPlayer player)
  {
    BnetGameAccount bnetGameAccount = player == null ? (BnetGameAccount) null : player.GetHearthstoneGameAccount();
    if (bnetGameAccount == (BnetGameAccount) null)
      return (FiresideGatheringInfo) null;
    byte[] gameFieldBytes = bnetGameAccount.GetGameFieldBytes(25U);
    return gameFieldBytes != null && gameFieldBytes.Length != 0 ? ProtobufUtil.ParseFrom<FiresideGatheringInfo>(gameFieldBytes) : (FiresideGatheringInfo) null;
  }

  private FiresideGatheringInfo GetMyFSGInfoForPresence()
  {
    if (this.m_currentFSG == null)
      return (FiresideGatheringInfo) null;
    return new FiresideGatheringInfo()
    {
      FsgId = this.m_currentFSG.FsgId
    };
  }

  private void UpdateMyPresence()
  {
    FiresideGatheringInfo fsgInfoForPresence = this.GetMyFSGInfoForPresence();
    BnetPresenceMgr.Get().SetGameFieldBlob(25U, (IProtoBuf) fsgInfoForPresence);
  }

  public void Cheat_CheckInToFakeFSG(FSGConfig fsg)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.m_cachedFakeCheatFsg = fsg;
    Network.Get().RegisterNetHandler((object) TavernBrawlInfo.PacketID.ID, new Network.NetHandler(this.Cheat_OnTavernBrawlInfoCheckInToFakeFSG));
    Network.Get().RequestTavernBrawlInfo(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING);
  }

  private void Cheat_OnTavernBrawlInfoCheckInToFakeFSG()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    Network.Get().RemoveNetHandler((object) TavernBrawlInfo.PacketID.ID, new Network.NetHandler(this.Cheat_OnTavernBrawlInfoCheckInToFakeFSG));
    CheckInToFSGResponse body = new CheckInToFSGResponse()
    {
      ErrorCode = PegasusShared.ErrorCode.ERROR_OK,
      PlayerRecord = new TavernBrawlPlayerRecord()
    };
    body.PlayerRecord.SessionStatus = TavernBrawlStatus.TB_STATUS_ACTIVE;
    body.FsgId = this.m_cachedFakeCheatFsg.FsgId;
    PegasusPacket packet = new PegasusPacket(505, 0, (object) body);
    Network.Get().SimulateReceivedPacketFromServer(packet);
    this.m_cachedFakeCheatFsg = (FSGConfig) null;
  }

  public void Cheat_CheckInToFakeFSG()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    FSGConfig fsg = new FSGConfig()
    {
      FsgId = -1,
      TavernName = "Fake Gathering",
      UnixOfficialStartTime = TimeUtils.UnixTimestampSeconds - 7200L,
      UnixOfficialEndTime = TimeUtils.UnixTimestampSeconds + 14400L
    };
    fsg.UnixStartTimeWithSlush = fsg.UnixOfficialStartTime - 28800L;
    fsg.UnixEndTimeWithSlush = fsg.UnixOfficialEndTime + 28800L;
    this.Cheat_CheckInToFakeFSG(fsg);
  }

  public void Cheat_CheckOutOfFakeFSG()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    PegasusPacket packet = new PegasusPacket(506, 0, (object) new CheckOutOfFSGResponse()
    {
      ErrorCode = PegasusShared.ErrorCode.ERROR_OK
    });
    Network.Get().SimulateReceivedPacketFromServer(packet);
  }

  public void Cheat_NearbyFSGNotice() => this.NotifyFSGNearby();

  public void Cheat_CreateFakeGatherings(int numGatherings, bool innkeeper = false)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    RequestNearbyFSGsResponse body = new RequestNearbyFSGsResponse();
    body.ErrorCode = PegasusShared.ErrorCode.ERROR_OK;
    body.FSGs = new List<FSGConfig>();
    for (int index = 0; index < numGatherings; ++index)
    {
      FSGConfig fsgConfig = new FSGConfig()
      {
        FsgId = (long) (-index - 2),
        TavernName = "Fake Gathering " + (object) index,
        UnixOfficialStartTime = TimeUtils.UnixTimestampSeconds - 7200L,
        UnixOfficialEndTime = TimeUtils.UnixTimestampSeconds + 14400L
      };
      fsgConfig.UnixStartTimeWithSlush = fsgConfig.UnixOfficialStartTime - 28800L;
      fsgConfig.UnixEndTimeWithSlush = fsgConfig.UnixOfficialEndTime + 28800L;
      fsgConfig.SignData = new TavernSignData()
      {
        Sign = UnityEngine.Random.Range(1, 8),
        Background = UnityEngine.Random.Range(1, 15),
        Major = UnityEngine.Random.Range(1, 85),
        Minor = UnityEngine.Random.Range(1, 43),
        SignType = TavernSignType.TAVERN_SIGN_TYPE_CUSTOM
      };
      if (innkeeper && index == 0)
      {
        fsgConfig.IsInnkeeper = true;
        fsgConfig.IsSetupComplete = false;
      }
      body.FSGs.Add(fsgConfig);
    }
    PegasusPacket packet = new PegasusPacket(504, 0, (object) body);
    Network.Get().SimulateReceivedPacketFromServer(packet);
  }

  public void Cheat_RemoveFakeGatherings()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    RequestNearbyFSGsResponse body = new RequestNearbyFSGsResponse();
    body.ErrorCode = PegasusShared.ErrorCode.ERROR_OK;
    body.FSGs = new List<FSGConfig>();
    foreach (FSGConfig nearbyFsG in this.m_nearbyFSGs)
    {
      if (nearbyFsG.FsgId >= 0L)
        body.FSGs.Add(nearbyFsG);
    }
    PegasusPacket packet = new PegasusPacket(504, 0, (object) body);
    Network.Get().SimulateReceivedPacketFromServer(packet);
  }

  public void Cheat_MockInnkeeperSetup()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    PegasusPacket packet = new PegasusPacket(508, 0, (object) new InnkeeperSetupGatheringResponse()
    {
      ErrorCode = PegasusShared.ErrorCode.ERROR_OK,
      FsgId = this.m_innkeeperFSG.FsgId
    });
    Network.Get().SimulateReceivedPacketFromServer(packet);
  }

  public void Cheat_ShowSign(
    TavernSignType type,
    int sign,
    int background,
    int major,
    int minor,
    string tavernName)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.ShowSign(new TavernSignData()
    {
      SignType = type,
      Sign = sign,
      Background = background,
      Major = major,
      Minor = minor
    }, tavernName, (FiresideGatheringManager.OnCloseSign) null);
  }

  public void Cheat_GPSOffset(double offset)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.m_gpsCheatOffset = offset;
  }

  public void Cheat_GPSSet(double latitude, double longitude)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.m_gpsCheatingLocation = true;
    this.m_gpsCheatLatitude = latitude;
    this.m_gpsCheatLongitude = longitude;
  }

  public void Cheat_ResetGPSCheating()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.m_gpsCheatingLocation = false;
    this.m_gpsCheatLatitude = 0.0;
    this.m_gpsCheatLongitude = 0.0;
    this.m_gpsCheatOffset = 0.0;
  }

  public void Cheat_GetGPSCheats(
    out bool isCheatingGPS,
    out double latitude,
    out double longitude,
    out double offset)
  {
    isCheatingGPS = this.m_gpsCheatingLocation;
    latitude = this.m_gpsCheatLatitude;
    longitude = this.m_gpsCheatLongitude;
    offset = this.m_gpsCheatOffset;
  }

  public void Cheat_ToggleLargeScaleFSG()
  {
    if (this.m_currentFSG == null)
      return;
    this.m_currentFSG.IsLargeScaleFsg = !this.m_currentFSG.IsLargeScaleFsg;
    if (this.OnJoinFSG == null)
      return;
    this.OnJoinFSG(this.m_currentFSG);
  }

  public BnetPlayer Cheat_CreateFSGPatron(
    string fullName,
    int leagueId,
    int starLevel,
    BnetProgramId programId,
    bool isFriend,
    bool isOnline)
  {
    BnetPlayer player = BnetFriendMgr.Get().Cheat_CreatePlayer(fullName, leagueId, starLevel, programId, isFriend, isOnline);
    this.m_displayablePatrons.Add(player);
    return player;
  }

  public int Cheat_RemoveCheatFriends() => this.m_displayablePatrons.RemoveWhere((Predicate<BnetPlayer>) (player => player.IsCheatPlayer));

  public enum FiresideGatheringMode
  {
    NONE,
    MAIN_SCREEN,
    FRIENDLY_CHALLENGE,
    FRIENDLY_CHALLENGE_BRAWL,
    FIRESIDE_BRAWL,
  }

  public delegate void CheckedInToFSGCallback(FSGConfig gathering);

  public delegate void CheckedOutOfFSGCallback(FSGConfig gathering);

  public delegate void RequestNearbyFSGsCallback();

  public delegate void NearbyFSGsChangedCallback();

  public delegate void OnCloseSign();

  public delegate void OnInnkeeperSetupFinishedCallback(bool success);

  public delegate void FSGSignClosedCallback();

  public delegate void FSGSignShownCallback();

  public delegate void OnPatronListUpdatedCallback(
    List<BnetPlayer> addedToDisplayablePatronList,
    List<BnetPlayer> removedFromDisplayablePatronList);
}
