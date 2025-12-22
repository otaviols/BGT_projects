using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Services;
using Hearthstone.Progression;
using PegasusLettuce;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackOpening : MonoBehaviour
{
  public PackOpeningBones m_Bones;
  public PackOpeningDirector m_DirectorPrefab;
  public PackOpeningSocket m_Socket;
  public PackOpeningSocket m_SocketAccent;
  public UberText m_HeaderText;
  public UIBButton m_BackButton;
  public GameObject m_DragPlane;
  public Vector3 m_DragTolerance;
  public GameObject m_InputBlocker;
  public UIBObjectSpacing m_UnopenedPackContainer;
  public UIBScrollable m_UnopenedPackScroller;
  public CameraMask m_PackTrayCameraMask;
  public float m_UnopenedPackPadding;
  public bool m_OnePackCentered = true;
  private const int MAX_OPENED_PACKS_BEFORE_CARD_CACHE_RESET = 10;
  private static PackOpening s_instance;
  private bool m_waitingForInitialNetData = true;
  private bool m_waitingForInitialMercenaryData;
  private bool m_shown;
  private readonly Map<int, UnopenedPack> m_unopenedPacks = new Map<int, UnopenedPack>();
  private readonly Map<int, bool> m_unopenedPacksLoading = new Map<int, bool>();
  private PackOpeningDirector m_director;
  private UnopenedPack m_draggedPack;
  private Notification m_hintArrow;
  private GameObject m_PackOpeningCardFX;
  private GameObject m_PackOpeningPortraitFX;
  private GameObject m_PackOpeningCoinFX;
  private bool m_autoOpenPending;
  private int m_lastOpenedBoosterId;
  private bool m_enableBackButton;
  private bool m_entryTransitionFinished;
  private static bool m_hasAcknowledgedKoreanWarning;
  private Coroutine m_autoOpenPackCoroutine;
  private float m_packOpeningStartTime;
  private int m_packOpeningId;
  private VillagePackOpeningDisplay m_villageDisplay;
  private const float m_holdToOpenDelay = 0.5f;
  private bool m_holdToOpenPackReady;
  private bool m_spaceBarIsDown;
  private static LettucePackComponent m_mockLettucePackComponent;

  private void Awake()
  {
    PackOpening.s_instance = this;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      AssetLoader.Get().InstantiatePrefab((AssetReference) "PackOpeningCardFX_Phone.prefab:0ef32a20a9e7843c3ba360e49527dbfa", new PrefabCallback<GameObject>(this.OnPackOpeningCardFXLoaded));
      AssetLoader.Get().InstantiatePrefab((AssetReference) "PackOpeningPortraitFX_Phone.prefab:0608d65f5479515409f3ff3e905a7343", new PrefabCallback<GameObject>(this.OnPackOpeningPortraitFXLoaded));
      AssetLoader.Get().InstantiatePrefab((AssetReference) "PackOpeningCoinFX_Phone.prefab:1da77448f9a695f4faa3101c383c7770", new PrefabCallback<GameObject>(this.OnPackOpeningCoinFXLoaded));
    }
    else
    {
      AssetLoader.Get().InstantiatePrefab((AssetReference) "PackOpeningCardFX.prefab:b32177fb14f134edfb891dc93501b1ce", new PrefabCallback<GameObject>(this.OnPackOpeningCardFXLoaded));
      AssetLoader.Get().InstantiatePrefab((AssetReference) "PackOpeningPortraitFX.prefab:abb645e2796976845ab335be65d26618", new PrefabCallback<GameObject>(this.OnPackOpeningPortraitFXLoaded));
      AssetLoader.Get().InstantiatePrefab((AssetReference) "PackOpeningCoinFX.prefab:a3ff192999fcb13478dd78b1e5c80576", new PrefabCallback<GameObject>(this.OnPackOpeningCoinFXLoaded));
    }
    this.InitializeNet();
    this.InitializeUI();
    TelemetryWatcher.WatchFor(TelemetryWatcherWatchType.StoreFromPackOpening);
    if (SceneMgr.Get() != null && SceneMgr.Get().GetMode() != SceneMgr.Mode.LETTUCE_PACK_OPENING)
    {
      Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
      SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    }
    FiresideGatheringManager.Get().OnJoinFSG += new FiresideGatheringManager.CheckedInToFSGCallback(this.OnFiresideGatheringCheckinStatusChanged);
    FiresideGatheringManager.Get().OnLeaveFSG += new FiresideGatheringManager.CheckedOutOfFSGCallback(this.OnFiresideGatheringCheckinStatusChanged);
    GameSaveDataManager.Get().Request(GameSaveKeyId.COLLECTION_MANAGER, new GameSaveDataManager.OnRequestDataResponseDelegate(this.OnGameSaveDataReady));
  }

  private void Start()
  {
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
  }

  private void Update() => this.UpdateDraggedPack();

  private void OnDestroy()
  {
    if ((UnityEngine.Object) this.m_draggedPack != (UnityEngine.Object) null && (UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
      PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    this.ShutdownNet();
    PackOpening.s_instance = (PackOpening) null;
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    if (FiresideGatheringManager.Get() != null)
    {
      FiresideGatheringManager.Get().OnJoinFSG -= new FiresideGatheringManager.CheckedInToFSGCallback(this.OnFiresideGatheringCheckinStatusChanged);
      FiresideGatheringManager.Get().OnLeaveFSG -= new FiresideGatheringManager.CheckedOutOfFSGCallback(this.OnFiresideGatheringCheckinStatusChanged);
    }
    FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    this.m_director.OnDoneOpeningPack -= new System.Action(this.OnDonePackOpening);
  }

  public static PackOpening Get() => PackOpening.s_instance;

  public GameObject GetPackOpeningCardEffects() => this.m_PackOpeningCardFX;

  public GameObject GetPackOpeningPortraitEffects() => this.m_PackOpeningPortraitFX;

  public GameObject GetPackOpeningCoinEffects() => this.m_PackOpeningCoinFX;

  private bool SpaceToOpenPack()
  {
    if ((UnityEngine.Object) this.m_director == (UnityEngine.Object) null)
      return false;
    if (this.CanOpenPackAutomatically())
    {
      this.m_autoOpenPending = true;
      this.m_director.FinishPackOpen();
      this.m_autoOpenPackCoroutine = this.StartCoroutine(this.OpenNextPackWhenReady());
    }
    else if (PackOpeningDirector.QuickPackOpeningAllowed)
      this.m_director.ForceRevealRandomCard();
    return true;
  }

  public bool HandleKeyboardInput()
  {
    if (InputCollection.GetKeyUp(KeyCode.Space))
    {
      this.StopCoroutine(this.StartHoldToOpenCooldown());
      this.m_spaceBarIsDown = false;
      return this.SpaceToOpenPack();
    }
    if (InputCollection.GetKeyDown(KeyCode.Space))
    {
      this.m_spaceBarIsDown = true;
      this.StartCoroutine(this.StartHoldToOpenCooldown());
    }
    if (!this.HoldSpaceToOpenPacksEnabled() || !this.m_spaceBarIsDown || !this.m_holdToOpenPackReady)
      return false;
    this.StartCoroutine(this.StartHoldToOpenCooldown());
    return this.SpaceToOpenPack();
  }

  public void PreUnload()
  {
    FullScreenFXMgr.Get().StopAllEffects();
    if ((UnityEngine.Object) this.m_director != (UnityEngine.Object) null)
      this.m_director.HideCardsAndDoneButton();
    this.Hide();
  }

  public bool CreateMockLettucePackComponent(
    int mercId,
    int artVariantId,
    int currenyAmount,
    bool acquired,
    TAG_PREMIUM premium)
  {
    if (artVariantId != 0 && !LettuceMercenary.GetArtVariations(mercId).Exists((Predicate<MercenaryArtVariationDbfRecord>) (variation => variation.ID == artVariantId)))
      return false;
    PackOpening.m_mockLettucePackComponent = new LettucePackComponent();
    PackOpening.m_mockLettucePackComponent.MercenaryId = mercId;
    PackOpening.m_mockLettucePackComponent.HasMercenaryId = mercId != 0;
    PackOpening.m_mockLettucePackComponent.MercenaryArtVariationId = artVariantId;
    PackOpening.m_mockLettucePackComponent.HasMercenaryArtVariationId = artVariantId != 0;
    PackOpening.m_mockLettucePackComponent.MercenaryArtVariationPremium = (int) premium;
    PackOpening.m_mockLettucePackComponent.HasMercenaryArtVariationPremium = premium != 0;
    PackOpening.m_mockLettucePackComponent.CurrencyAmount = (long) currenyAmount;
    PackOpening.m_mockLettucePackComponent.HasCurrencyAmount = currenyAmount != 0;
    PackOpening.m_mockLettucePackComponent.MercenaryAlreadyAcquired = acquired;
    PackOpening.m_mockLettucePackComponent.HasMercenaryAlreadyAcquired = acquired;
    return true;
  }

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData) => this.PreUnload();

  public bool IsReady() => !this.m_waitingForInitialNetData;

  public void SetVillageDisplay(VillagePackOpeningDisplay display) => this.m_villageDisplay = display;

  public bool HoldSpaceToOpenPacksEnabled() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().ContinuousQuickOpenEnabled;

  public IEnumerator StartHoldToOpenCooldown()
  {
    this.m_holdToOpenPackReady = false;
    yield return (object) new WaitForSeconds(0.5f);
    this.m_holdToOpenPackReady = true;
  }

  private void OnBoxTransitionFinished(object userData)
  {
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    this.Show();
    this.m_entryTransitionFinished = true;
  }

  public void Show()
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_PACK_OPENING)
    {
      this.m_entryTransitionFinished = true;
      MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesPackOpening);
    }
    else
    {
      if (!Options.Get().GetBool(Option.HAS_SEEN_PACK_OPENING, false) && BoosterPackUtils.GetTotalBoosterCount() > 0)
        Options.Get().SetBool(Option.HAS_SEEN_PACK_OPENING, true);
      MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_PackOpening);
    }
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PACKOPENING);
    this.CreateDirector();
    BnetBar.Get().RefreshCurrency();
    if (BoosterPackUtils.GetBoosterStackCount() < 2 || SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_PACK_OPENING)
      this.ShowHintOnUnopenedPack();
    this.UpdateUIEvents();
    this.DisablePackTrayMask();
  }

  private void Hide()
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    this.DestroyHint();
    this.m_InputBlocker.SetActive(false);
    this.EnablePackTrayMask();
    this.UnregisterUIEvents();
    this.ShutdownNet();
  }

  private bool OnNavigateBack()
  {
    if (!this.m_enableBackButton || this.m_InputBlocker.activeSelf)
      return false;
    this.GoBack();
    return true;
  }

  private void GoBack()
  {
    this.Hide();
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_PACK_OPENING && (UnityEngine.Object) this.m_villageDisplay != (UnityEngine.Object) null)
      this.m_villageDisplay.NavigateBack();
    else
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
  }

  private void InitializeNet()
  {
    this.m_waitingForInitialNetData = true;
    this.m_waitingForInitialMercenaryData = BoosterPackUtils.LoadMercenaryCollectionIfRequired();
    NetCache.Get().RegisterScreenPackOpening(new NetCache.NetCacheCallback(this.OnNetDataReceived), new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
    Network.Get().RegisterNetHandler((object) BoosterContent.PacketID.ID, new Network.NetHandler(this.OnBoosterOpened));
    Network.Get().RegisterNetHandler((object) OpenMercenariesPackResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesBoosterOpened));
    Network.Get().RegisterNetHandler((object) PegasusUtil.DBAction.PacketID.ID, new Network.NetHandler(this.OnDBAction));
    LoginManager.Get().OnAchievesLoaded += new System.Action(this.OnReloginComplete);
  }

  private void ShutdownNet()
  {
    NetCache service1;
    if (ServiceManager.TryGet<NetCache>(out service1))
      service1.UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetDataReceived));
    Network service2;
    if (ServiceManager.TryGet<Network>(out service2))
    {
      service2.RemoveNetHandler((object) BoosterContent.PacketID.ID, new Network.NetHandler(this.OnBoosterOpened));
      service2.RemoveNetHandler((object) OpenMercenariesPackResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesBoosterOpened));
      service2.RemoveNetHandler((object) PegasusUtil.DBAction.PacketID.ID, new Network.NetHandler(this.OnDBAction));
    }
    if (!ServiceManager.TryGet<LoginManager>(out LoginManager _))
      return;
    LoginManager.Get().OnAchievesLoaded -= new System.Action(this.OnReloginComplete);
  }

  private void OnNetDataReceived()
  {
    if (this.m_waitingForInitialNetData)
    {
      this.m_waitingForInitialNetData = false;
      this.StartCoroutine(this.WaitForMercenaryData());
    }
    this.UpdatePacks();
    this.UpdateUIEvents();
  }

  private IEnumerator WaitForMercenaryData()
  {
    while (this.m_waitingForInitialMercenaryData && !CollectionManager.Get().IsLettuceLoaded())
      yield return (object) null;
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.LETTUCE_PACK_OPENING)
      SceneMgr.Get().NotifySceneLoaded();
  }

  private void OnReloginComplete()
  {
    this.UpdatePacks();
    this.UpdateUIEvents();
  }

  private void UpdatePacks()
  {
    NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
    if (netObject == null)
    {
      Debug.LogError((object) string.Format("PackOpening.UpdatePacks() - boosters are null"));
    }
    else
    {
      if (SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_PACK_OPENING)
        this.UpdateBoosterPacks(netObject, true);
      else
        this.UpdateBoosterPacks(netObject);
      if (!((UnityEngine.Object) this.m_director == (UnityEngine.Object) null) && this.m_director.IsPlaying())
        return;
      this.LayoutPacks();
    }
  }

  private void UpdateBoosterPacks(NetCache.NetCacheBoosters netBoosters, bool mercPacksOnly = false)
  {
    foreach (NetCache.BoosterStack boosterStack in netBoosters.BoosterStacks)
    {
      if (!mercPacksOnly || boosterStack.Id == 629)
      {
        int id = boosterStack.Id;
        if (this.m_unopenedPacks.ContainsKey(id) && (UnityEngine.Object) this.m_unopenedPacks[id] != (UnityEngine.Object) null)
        {
          if (netBoosters.GetBoosterStack(id) == null)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_unopenedPacks[id]);
            this.m_unopenedPacks[id] = (UnopenedPack) null;
          }
          else
            this.UpdatePack(this.m_unopenedPacks[id], netBoosters.GetBoosterStack(id));
        }
        else if (netBoosters.GetBoosterStack(id) != null && netBoosters.GetBoosterStack(id).Count > 0 && (!this.m_unopenedPacksLoading.ContainsKey(id) || !this.m_unopenedPacksLoading[id]))
        {
          this.m_unopenedPacksLoading[id] = true;
          BoosterDbfRecord record = GameDbf.Booster.GetRecord(id);
          if (record == null)
            Debug.LogErrorFormat("PackOpening.UpdatePacks() - No DBF record for booster {0}", (object) id);
          else if (string.IsNullOrEmpty(record.PackOpeningPrefab))
            Debug.LogError((object) string.Format("PackOpening.UpdatePacks() - no prefab found for booster {0}!", (object) id));
          else
            AssetLoader.Get().InstantiatePrefab((AssetReference) record.PackOpeningPrefab, new PrefabCallback<GameObject>(this.OnUnopenedPackLoaded), (object) boosterStack, AssetLoadingOptions.IgnorePrefabPosition);
        }
      }
    }
  }

  private void OnBoosterOpened()
  {
    this.m_director.Play(this.m_lastOpenedBoosterId, Time.realtimeSinceStartup - this.m_packOpeningStartTime, this.m_packOpeningId);
    this.m_autoOpenPending = false;
    this.m_director.OnBoosterOpened(Network.Get().OpenedBooster());
  }

  private void OnMercenariesBoosterOpened()
  {
    OpenMercenariesPackResponse mercenariesPackResponse = Network.Get().OpenMercenariesPackResponse();
    if (mercenariesPackResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Error.AddWarning(GameStrings.Get("GLUE_LETTUCE_ERROR_OPENING_PACK_TITLE"), GameStrings.Get("GLUE_LETTUCE_ERROR_OPENING_PACK_DESCRIPTION"));
      this.RevertBoosterOpeningAfterFailure();
    }
    else
    {
      this.m_director.Play(this.m_lastOpenedBoosterId, Time.realtimeSinceStartup - this.m_packOpeningStartTime, this.m_packOpeningId);
      this.m_autoOpenPending = false;
      if (PackOpening.m_mockLettucePackComponent != null)
      {
        mercenariesPackResponse.PackContents.Components[0] = PackOpening.m_mockLettucePackComponent;
        PackOpening.m_mockLettucePackComponent = (LettucePackComponent) null;
      }
      this.m_director.OnMercenariesBoosterOpened(mercenariesPackResponse.PackContents.Components);
    }
  }

  private void OnDBAction()
  {
    Network.DBAction dbAction = Network.Get().GetDbAction();
    if (dbAction.Action != Network.DBAction.ActionType.OPEN_BOOSTER || dbAction.Result == Network.DBAction.ResultType.SUCCESS)
      return;
    this.OnDBAction_OpenBoosterFailed(dbAction);
  }

  private void OnDBAction_OpenBoosterFailed(Network.DBAction response)
  {
    Debug.LogError((object) string.Format("PackOpening.OnDBAction_OpenBoosterFailed - Error while opening packs: {0}", (object) response));
    this.RevertBoosterOpeningAfterFailure();
  }

  private void RevertBoosterOpeningAfterFailure()
  {
    this.m_UnopenedPackScroller.Pause(false);
    this.m_InputBlocker.SetActive(false);
    this.m_autoOpenPending = false;
    this.m_unopenedPacks[this.m_lastOpenedBoosterId].AddBooster();
    this.m_unopenedPacksLoading[this.m_lastOpenedBoosterId] = false;
    BnetBar.Get().RefreshCurrency();
  }

  private void OnFiresideGatheringCheckinStatusChanged(FSGConfig gathering)
  {
    foreach (KeyValuePair<int, UnopenedPack> unopenedPack in this.m_unopenedPacks)
    {
      if (!((UnityEngine.Object) unopenedPack.Value == (UnityEngine.Object) null))
        unopenedPack.Value.UpdateState();
    }
  }

  private void OnGameSaveDataReady(bool dataLoadSuccess)
  {
    if (!dataLoadSuccess)
      Log.CollectionManager.PrintError("Error retrieving Game Save Key for Collection Manager!");
    else
      CardBackManager.Get().LoadRandomCardBackIntoFavoriteSlot(true);
  }

  private void CreateDirector()
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_DirectorPrefab.gameObject);
    this.m_director = gameObject.GetComponent<PackOpeningDirector>();
    gameObject.transform.parent = this.transform;
    TransformUtil.CopyWorld((Component) this.m_director, (Component) this.m_Bones.m_Director);
    this.m_director.OnDoneOpeningPack += new System.Action(this.OnDonePackOpening);
  }

  private void PickUpBooster()
  {
    UnopenedPack creatorPack = this.m_draggedPack.GetCreatorPack();
    creatorPack.RemoveBooster();
    this.m_draggedPack.SetBoosterId(creatorPack.GetBoosterId());
    this.m_draggedPack.SetCount(1);
  }

  private void OpenBooster(UnopenedPack pack)
  {
    AchievementManager.Get().PauseToastNotifications();
    PopupDisplayManager.Get().RedundantNDERerollPopups.SuppressNDEPopups = true;
    int boosterDbId = 1;
    if (!GameUtils.IsFakePackOpeningEnabled())
    {
      boosterDbId = pack.GetBoosterId();
      this.m_packOpeningStartTime = Time.realtimeSinceStartup;
      this.m_packOpeningId = boosterDbId;
      BoosterPackUtils.OpenBooster(boosterDbId);
    }
    this.m_InputBlocker.SetActive(true);
    if (this.m_autoOpenPackCoroutine != null)
    {
      this.StopCoroutine(this.m_autoOpenPackCoroutine);
      this.m_autoOpenPackCoroutine = (Coroutine) null;
    }
    this.m_director.OnFinishedEvent += new EventHandler(this.OnDirectorFinished);
    this.m_lastOpenedBoosterId = boosterDbId;
    BnetBar.Get().HideCurrencyFrames();
    if (GameUtils.IsFakePackOpeningEnabled())
      this.StartCoroutine(this.OnFakeBoosterOpened());
    this.m_UnopenedPackScroller.Pause(true);
  }

  private IEnumerator OnFakeBoosterOpened()
  {
    yield return (object) new WaitForSeconds(UnityEngine.Random.Range(0.0f, 1f));
    this.m_director.OnBoosterOpened(new List<NetCache.BoosterCard>()
    {
      new NetCache.BoosterCard()
      {
        Def = {
          Name = "CS1_042",
          Premium = TAG_PREMIUM.NORMAL
        }
      },
      new NetCache.BoosterCard()
      {
        Def = {
          Name = "CS1_129",
          Premium = TAG_PREMIUM.NORMAL
        }
      },
      new NetCache.BoosterCard()
      {
        Def = {
          Name = "EX1_050",
          Premium = TAG_PREMIUM.NORMAL
        }
      },
      new NetCache.BoosterCard()
      {
        Def = {
          Name = "EX1_105",
          Premium = TAG_PREMIUM.NORMAL
        }
      },
      new NetCache.BoosterCard()
      {
        Def = {
          Name = "EX1_350",
          Premium = TAG_PREMIUM.NORMAL
        }
      }
    });
  }

  private void PutBackBooster()
  {
    UnopenedPack creatorPack = this.m_draggedPack.GetCreatorPack();
    this.m_draggedPack.RemoveBooster();
    creatorPack.AddBooster();
  }

  private void UpdatePack(UnopenedPack pack, NetCache.BoosterStack boosterStack)
  {
    pack.SetBoosterId(boosterStack.Id);
    pack.SetCount(boosterStack.Count);
  }

  private void OnUnopenedPackLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if (callbackData is NetCache.BoosterStack)
    {
      int id = ((NetCache.BoosterStack) callbackData).Id;
      this.m_unopenedPacksLoading[id] = false;
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("PackOpening.OnUnopenedPackLoaded() - FAILED to load {0}", (object) assetRef));
      }
      else
      {
        UnopenedPack component = go.GetComponent<UnopenedPack>();
        go.SetActive(false);
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogError((object) string.Format("PackOpening.OnUnopenedPackLoaded() - asset {0} did not have a {1} script on it", (object) this.name, (object) typeof (UnopenedPack)));
        }
        else
        {
          this.m_unopenedPacks.Add(id, component);
          component.gameObject.SetActive(true);
          GameUtils.SetParent((Component) component, (Component) this.m_UnopenedPackContainer);
          component.transform.localScale = Vector3.one;
          component.SetDragTolerance(this.m_DragTolerance);
          component.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnUnopenedPackPress));
          component.AddEventListener(UIEventType.DRAG, new UIEvent.Handler(this.OnUnopenedPackDrag));
          component.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnUnopenedPackRollover));
          component.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnUnopenedPackRollout));
          component.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.OnUnopenedPackReleaseAll));
          this.UpdatePack(component, (NetCache.BoosterStack) callbackData);
          AchieveManager.Get().NotifyOfPacksReadyToOpen(component);
          if (BoosterPackUtils.GetBoosterStackCount() < 2 || SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_PACK_OPENING)
          {
            this.LayoutPacks();
            this.ShowHintOnUnopenedPack();
          }
          else
          {
            this.StopHintOnUnopenedPack();
            this.LayoutPacks();
          }
          this.UpdateUIEvents();
        }
      }
    }
    else
      Debug.LogErrorFormat("OnUnopenedPackLoaded() - Unable to get booster id for gameobject {0}", (object) go?.name);
  }

  private void LayoutPacks(bool animate = false)
  {
    IEnumerable<int> sortedPackIds = GameUtils.GetSortedPackIds(false);
    this.m_UnopenedPackContainer.ClearObjects();
    if (!this.m_entryTransitionFinished)
      this.DisablePackTrayMask();
    int key1 = 18;
    if (TemporaryAccountManager.IsTemporaryAccount())
    {
      UnopenedPack comp;
      this.m_unopenedPacks.TryGetValue(key1, out comp);
      if ((UnityEngine.Object) comp != (UnityEngine.Object) null && comp.GetCount() > 0)
      {
        comp.gameObject.SetActive(true);
        this.m_UnopenedPackContainer.AddObject((Component) comp);
      }
    }
    foreach (int key2 in sortedPackIds)
    {
      if (key1 != key2)
      {
        UnopenedPack comp;
        this.m_unopenedPacks.TryGetValue(key2, out comp);
        if (!((UnityEngine.Object) comp == (UnityEngine.Object) null) && comp.GetCount() != 0)
        {
          comp.gameObject.SetActive(true);
          this.m_UnopenedPackContainer.AddObject((Component) comp);
        }
      }
    }
    if (this.m_OnePackCentered && this.m_UnopenedPackContainer.m_Objects.Count == 1)
      this.m_UnopenedPackContainer.AddSpace(0, new Vector3(0.0f, 0.0f, 0.5f));
    else if (this.m_OnePackCentered && this.m_UnopenedPackContainer.m_Objects.Count < 1)
      this.m_UnopenedPackContainer.AddSpace(0);
    if (animate)
      this.m_UnopenedPackContainer.AnimateUpdatePositions(0.25f);
    else
      this.m_UnopenedPackContainer.UpdatePositions();
    if (this.m_entryTransitionFinished)
      return;
    this.EnablePackTrayMask();
  }

  private void CreateDraggedPack(UnopenedPack creatorPack)
  {
    this.m_draggedPack = creatorPack.AcquireDraggedPack();
    Vector3 vector3 = this.m_draggedPack.transform.position;
    RaycastHit hitInfo;
    if (UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
      vector3 = hitInfo.point;
    float f = Vector3.Dot(Camera.main.transform.forward, Vector3.up);
    float num = -f / Mathf.Abs(f);
    Bounds bounds = this.m_draggedPack.GetComponent<Collider>().bounds;
    vector3.y += num * bounds.extents.y * this.m_draggedPack.transform.lossyScale.y;
    this.m_draggedPack.transform.position = vector3;
  }

  private void DestroyDraggedPack()
  {
    this.m_UnopenedPackScroller.Pause(false);
    this.m_draggedPack.GetCreatorPack().ReleaseDraggedPack();
    this.m_draggedPack = (UnopenedPack) null;
  }

  private void UpdateDraggedPack()
  {
    if ((UnityEngine.Object) this.m_draggedPack == (UnityEngine.Object) null)
      return;
    Vector3 position = this.m_draggedPack.transform.position;
    RaycastHit hitInfo;
    if (UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
    {
      position.x = hitInfo.point.x;
      position.z = hitInfo.point.z;
      this.m_draggedPack.transform.position = position;
    }
    if (!InputCollection.GetMouseButtonUp(0))
      return;
    this.DropPack();
  }

  private IEnumerator HideAfterNoMorePacks()
  {
    while (!((UnityEngine.Object) this.m_director == (UnityEngine.Object) null) && !((UnityEngine.Object) this.m_director.gameObject == (UnityEngine.Object) null))
      yield return (object) new WaitForSeconds(0.2f);
    this.GoBack();
  }

  private void OnDirectorFinished(object sender, EventArgs eventArgs)
  {
    this.m_UnopenedPackScroller.Pause(false);
    int num = 0;
    foreach (KeyValuePair<int, UnopenedPack> unopenedPack in this.m_unopenedPacks)
    {
      if (!((UnityEngine.Object) unopenedPack.Value == (UnityEngine.Object) null))
      {
        int count = unopenedPack.Value.GetCount();
        num += count;
        unopenedPack.Value.gameObject.SetActive(count > 0);
      }
    }
    if (num == 0)
    {
      this.StartCoroutine(this.HideAfterNoMorePacks());
    }
    else
    {
      this.m_InputBlocker.SetActive(false);
      this.CreateDirector();
      this.LayoutPacks(true);
    }
    BnetBar.Get().RefreshCurrency();
  }

  private void ShowHintOnUnopenedPack()
  {
    if (!this.m_shown || Options.Get().GetBool(Option.HAS_OPENED_BOOSTER, false) || !UserAttentionManager.CanShowAttentionGrabber("PackOpening.ShowHintOnUnopenedPack"))
      return;
    List<UnopenedPack> unopenedPackList = new List<UnopenedPack>();
    foreach (KeyValuePair<int, UnopenedPack> unopenedPack in this.m_unopenedPacks)
    {
      if (!((UnityEngine.Object) unopenedPack.Value == (UnityEngine.Object) null) && unopenedPack.Value.CanOpenPack() && unopenedPack.Value.GetCount() > 0)
        unopenedPackList.Add(unopenedPack.Value);
    }
    if (unopenedPackList.Count < 1 || (UnityEngine.Object) unopenedPackList[0] == (UnityEngine.Object) null || unopenedPackList[0].GetBoosterId() == 18)
      return;
    unopenedPackList[0].PlayAlert();
    if (!((UnityEngine.Object) this.m_hintArrow == (UnityEngine.Object) null))
      return;
    Vector3 center = unopenedPackList[0].GetComponent<Collider>().bounds.center;
    this.m_hintArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, center, new Vector3(0.0f, 90f, 0.0f), false);
    if (!((UnityEngine.Object) this.m_hintArrow != (UnityEngine.Object) null))
      return;
    this.FixArrowScale(unopenedPackList[0].transform);
    Vector3 vector3 = new Vector3(this.m_hintArrow.bounceObject.GetComponent<Renderer>().bounds.extents.x, center.y, 0.0f);
    this.m_hintArrow.gameObject.transform.SetParent(unopenedPackList[0].gameObject.transform);
    this.m_hintArrow.transform.localPosition = vector3;
  }

  private void StopHintOnUnopenedPack()
  {
    foreach (KeyValuePair<int, UnopenedPack> unopenedPack in this.m_unopenedPacks)
    {
      if (!((UnityEngine.Object) unopenedPack.Value == (UnityEngine.Object) null) && unopenedPack.Value.CanOpenPack() && unopenedPack.Value.GetCount() > 0)
      {
        unopenedPack.Value.StopAlert();
        break;
      }
    }
  }

  private void ShowHintOnSlot()
  {
    if (Options.Get().GetBool(Option.HAS_OPENED_BOOSTER, false) || !UserAttentionManager.CanShowAttentionGrabber("PackOpening.ShowHintOnSlot"))
      return;
    if ((UnityEngine.Object) this.m_hintArrow == (UnityEngine.Object) null)
      this.m_hintArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, false);
    if (!((UnityEngine.Object) this.m_hintArrow != (UnityEngine.Object) null))
      return;
    this.FixArrowScale(this.m_draggedPack.transform);
    Bounds bounds = this.m_hintArrow.bounceObject.GetComponent<Renderer>().bounds;
    Vector3 position = this.m_Bones.m_Hint.position;
    position.z += bounds.extents.z;
    this.m_hintArrow.transform.position = position;
  }

  private void FixArrowScale(Transform parent)
  {
    Transform parent1 = this.m_hintArrow.transform.parent;
    this.m_hintArrow.transform.parent = parent;
    this.m_hintArrow.transform.localScale = Vector3.one;
    this.m_hintArrow.transform.parent = parent1;
  }

  private void HideHint()
  {
    if ((UnityEngine.Object) this.m_hintArrow == (UnityEngine.Object) null)
      return;
    Options.Get().SetBool(Option.HAS_OPENED_BOOSTER, true);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_hintArrow.gameObject);
    this.m_hintArrow = (Notification) null;
  }

  private void DestroyHint()
  {
    if ((UnityEngine.Object) this.m_hintArrow == (UnityEngine.Object) null)
      return;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_hintArrow.gameObject);
    this.m_hintArrow = (Notification) null;
  }

  private void InitializeUI()
  {
    this.m_HeaderText.Text = GameStrings.Get("GLUE_PACK_OPENING_HEADER");
    this.m_BackButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonPressed));
    this.m_DragPlane.SetActive(false);
    this.m_InputBlocker.SetActive(false);
  }

  private void UpdateUIEvents()
  {
    if (!this.m_shown)
      this.UnregisterUIEvents();
    else if ((UnityEngine.Object) this.m_draggedPack != (UnityEngine.Object) null)
    {
      this.UnregisterUIEvents();
    }
    else
    {
      this.m_enableBackButton = true;
      this.m_BackButton.SetEnabled(true);
      foreach (KeyValuePair<int, UnopenedPack> unopenedPack in this.m_unopenedPacks)
      {
        if ((UnityEngine.Object) unopenedPack.Value != (UnityEngine.Object) null)
          unopenedPack.Value.SetEnabled(true);
      }
    }
  }

  private void UnregisterUIEvents()
  {
    this.m_enableBackButton = false;
    this.m_BackButton.SetEnabled(false);
    foreach (KeyValuePair<int, UnopenedPack> unopenedPack in this.m_unopenedPacks)
    {
      if ((UnityEngine.Object) unopenedPack.Value != (UnityEngine.Object) null)
        unopenedPack.Value.SetEnabled(false);
    }
  }

  private void OnBackButtonPressed(UIEvent e) => Navigation.GoBack();

  private void HoldPack(UnopenedPack selectedPack)
  {
    bool flag = UniversalInputManager.Get().InputIsOver(selectedPack.gameObject);
    if (!selectedPack.CanOpenPack() || !flag)
      return;
    this.DestroyHint();
    this.HideUnopenedPackTooltip();
    PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
    this.m_DragPlane.SetActive(true);
    this.CreateDraggedPack(selectedPack);
    if ((UnityEngine.Object) this.m_draggedPack != (UnityEngine.Object) null)
    {
      TooltipPanel componentInChildren = this.m_draggedPack.GetComponentInChildren<TooltipPanel>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) componentInChildren.gameObject);
    }
    this.PickUpBooster();
    selectedPack.StopAlert();
    this.ShowHintOnSlot();
    this.m_Socket.OnPackHeld();
    this.m_SocketAccent.OnPackHeld();
    this.UpdateUIEvents();
    this.m_UnopenedPackScroller.Pause(true);
  }

  private void DropPack()
  {
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    this.m_Socket.OnPackReleased();
    this.m_SocketAccent.OnPackReleased();
    if (UniversalInputManager.Get().InputIsOver(this.m_Socket.gameObject))
    {
      if (BattleNet.GetAccountCountry() == "KOR")
        PackOpening.m_hasAcknowledgedKoreanWarning = true;
      this.OpenBooster(this.m_draggedPack);
      this.HideHint();
    }
    else
    {
      this.PutBackBooster();
      this.DestroyHint();
    }
    this.DestroyDraggedPack();
    this.UpdateUIEvents();
    this.m_DragPlane.SetActive(false);
    this.ShowHintOnUnopenedPack();
  }

  private void AutomaticallyOpenPack()
  {
    this.HideUnopenedPackTooltip();
    UnopenedPack unopenedPack1 = (UnopenedPack) null;
    if (!this.m_unopenedPacks.TryGetValue(this.m_lastOpenedBoosterId, out unopenedPack1) || unopenedPack1.GetCount() == 0)
    {
      foreach (KeyValuePair<int, UnopenedPack> unopenedPack2 in this.m_unopenedPacks)
      {
        if (!((UnityEngine.Object) unopenedPack2.Value == (UnityEngine.Object) null) && unopenedPack2.Value.GetCount() > 0)
        {
          unopenedPack1 = unopenedPack2.Value;
          break;
        }
      }
    }
    if ((UnityEngine.Object) unopenedPack1 == (UnityEngine.Object) null || !unopenedPack1.CanOpenPack())
      return;
    if ((UnityEngine.Object) this.m_draggedPack != (UnityEngine.Object) null || this.m_InputBlocker.activeSelf)
    {
      this.m_autoOpenPending = false;
    }
    else
    {
      this.m_draggedPack = unopenedPack1.AcquireDraggedPack();
      this.PickUpBooster();
      unopenedPack1.StopAlert();
      this.OpenBooster(this.m_draggedPack);
      this.DestroyDraggedPack();
      this.UpdateUIEvents();
      this.m_DragPlane.SetActive(false);
    }
  }

  private void OnUnopenedPackPress(UIEvent e)
  {
    if ((e.GetElement() as UnopenedPack).GetBoosterId() != 18)
      return;
    TemporaryAccountManager.Get().ShowHealUpDialog(GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_HEADER_01"), GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_BODY_03"), TemporaryAccountManager.HealUpReason.LOCKED_PACK, true, (TemporaryAccountManager.OnHealUpDialogDismissed) null);
  }

  private void OnUnopenedPackDrag(UIEvent e) => this.HoldPack(e.GetElement() as UnopenedPack);

  private void OnUnopenedPackRollover(UIEvent e)
  {
    if (PackOpening.m_hasAcknowledgedKoreanWarning || BattleNet.GetAccountCountry() != "KOR")
      return;
    TooltipZone component = (e.GetElement() as UnopenedPack).GetComponent<TooltipZone>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.ShowTooltip(string.Empty, GameStrings.Get("GLUE_PACK_OPENING_TOOLTIP"), 5f);
  }

  private void OnUnopenedPackRollout(UIEvent e) => this.HideUnopenedPackTooltip();

  private void OnUnopenedPackReleaseAll(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_draggedPack == (UnityEngine.Object) null)
    {
      if (UniversalInputManager.Get().IsTouchMode() || !((UIReleaseAllEvent) e).GetMouseIsOver())
        return;
      if ((e.GetElement() as UnopenedPack).GetBoosterId() == 18)
        TemporaryAccountManager.Get().ShowHealUpDialog(GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_HEADER_01"), GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_BODY_03"), TemporaryAccountManager.HealUpReason.LOCKED_PACK, true, (TemporaryAccountManager.OnHealUpDialogDismissed) null);
      else
        this.HoldPack(e.GetElement() as UnopenedPack);
    }
    else
      this.DropPack();
  }

  private void HideUnopenedPackTooltip()
  {
    foreach (KeyValuePair<int, UnopenedPack> unopenedPack in this.m_unopenedPacks)
    {
      if (!((UnityEngine.Object) unopenedPack.Value == (UnityEngine.Object) null))
        unopenedPack.Value.GetComponent<TooltipZone>().HideTooltip();
    }
  }

  private bool CanOpenPackAutomatically() => Application.isFocused && !PopupDisplayManager.Get().IsShowing && !this.m_autoOpenPending && this.m_shown && BoosterPackUtils.GetTotalBoosterCount() != 0 && (!this.m_director.IsPlaying() || this.m_director.IsDoneButtonShown) && !this.m_DragPlane.activeSelf && !StoreManager.Get().IsShownOrWaitingToShow();

  private IEnumerator OpenNextPackWhenReady()
  {
    float waitTime = 0.0f;
    if (this.m_director.IsPlaying())
      waitTime = 1f;
    while (this.m_director.IsPlaying())
      yield return (object) null;
    yield return (object) new WaitForSeconds(waitTime);
    this.AutomaticallyOpenPack();
  }

  private void OnPackOpeningCardFXLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_PackOpeningCardFX = go;
  }

  private void OnPackOpeningPortraitFXLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_PackOpeningPortraitFX = go;
  }

  private void OnPackOpeningCoinFXLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_PackOpeningCoinFX = go;
  }

  private void EnablePackTrayMask()
  {
    if ((UnityEngine.Object) this.m_PackTrayCameraMask == (UnityEngine.Object) null)
      return;
    this.m_PackTrayCameraMask.enabled = true;
  }

  private void DisablePackTrayMask()
  {
    if ((UnityEngine.Object) this.m_PackTrayCameraMask == (UnityEngine.Object) null)
      return;
    foreach (Component componentsInChild in this.m_PackTrayCameraMask.m_ClipObjects.GetComponentsInChildren<Transform>())
    {
      GameObject gameObject = componentsInChild.gameObject;
      if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
        LayerUtils.SetLayer(gameObject, GameLayer.Default);
    }
    this.m_PackTrayCameraMask.enabled = false;
  }

  private void OnFatalError(FatalErrorMessage message, object userData)
  {
    if (!this.m_director.IsPlaying())
      this.NavigateToBoxAfterDisconnect();
    else
      this.m_director.OnDoneOpeningPack += new System.Action(this.OnDonePackOpening_FatalError);
  }

  private void OnDonePackOpening_FatalError()
  {
    this.m_director.OnDoneOpeningPack -= new System.Action(this.OnDonePackOpening_FatalError);
    if (Network.IsLoggedIn())
      return;
    this.NavigateToBoxAfterDisconnect();
  }

  private void OnDonePackOpening() => PopupDisplayManager.Get().RedundantNDERerollPopups.SuppressNDEPopups = false;

  private void NavigateToBoxAfterDisconnect()
  {
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    DialogManager.Get().ShowReconnectHelperDialog();
    Navigation.Clear();
  }
}
