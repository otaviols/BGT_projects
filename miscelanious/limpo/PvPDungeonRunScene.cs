using Assets;
using Hearthstone.DungeonCrawl;
using PegasusUtil;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class PvPDungeonRunScene : PegasusScene
{
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_screenPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_CollectionManagerPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_PopupManagerPrefab;
  [CustomEditField(Sections = "DungeonCrawl")]
  public float m_transitionStartingOffset = 100f;
  [CustomEditField(Sections = "DungeonCrawl")]
  public float m_transitionTime = 1f;
  [CustomEditField(Sections = "DungeonCrawl")]
  public float m_rootDropHeight = 10f;
  [CustomEditField(Sections = "DungeonCrawl", T = EditType.SOUND_PREFAB)]
  public string m_SlideInSound;
  [CustomEditField(Sections = "DungeonCrawl", T = EditType.SOUND_PREFAB)]
  public string m_SlideOutSound;
  private bool m_unloading;
  private bool m_screenPrefabLoaded;
  private bool m_gameSaveDataReceived;
  private bool m_collectionManagerPrefabLoaded;
  private bool m_isEditingDeck;
  private bool m_isTransitioningToCollection;
  private GameObject m_displayRoot;
  private GameObject m_collectionManager;
  private PvPDungeonRunDisplay m_display;
  private GameObject m_popupManagerRoot;
  private DuelsPopupManager m_PopupManager;
  private DungeonCrawlServices m_services;
  private AdventureDbId m_currentAdventure;
  private AdventureDungeonCrawlDisplay m_dungeonCrawlDisplay;
  private GuestHeroPickerTrayDisplay m_guestHeroPickerTrayDisplay;
  private AssetLoadingHelper m_assetLoadingHelper;
  private bool m_hasSession;
  private bool m_hasLatestSessionData;
  private bool m_hasStatsInfo;
  private int m_seasonId;
  private AdventureDefCache m_adventureDefCache;
  private AdventureWingDefCache m_adventureWingDefCache;
  private Vector3 CM_POS = new Vector3(55.5f, -15.5f, -80.9f);
  private Vector3 CM_SCALE = new Vector3(1.05f, 1.05f, 1.05f);
  private static PvPDungeonRunScene m_instance;

  public static PvPDungeonRunScene Get() => PvPDungeonRunScene.m_instance;

  public static bool IsEditingDeck() => (UnityEngine.Object) PvPDungeonRunScene.m_instance != (UnityEngine.Object) null && PvPDungeonRunScene.m_instance.m_isEditingDeck && !PvPDungeonRunScene.m_instance.m_isTransitioningToCollection;

  public void Start()
  {
    PvPDungeonRunScene.m_instance = this;
    AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) this.m_CollectionManagerPrefab, new PrefabCallback<GameObject>(this.OnCollectionManagerLoaded));
    AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) this.m_PopupManagerPrefab, new PrefabCallback<GameObject>(this.OnPopupManagerLoaded));
    Network.Get().RegisterNetHandler((object) PVPDRStatsInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRStatsResponse));
    Network.Get().RequestPVPDRStatsInfo();
    Network.Get().RegisterNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRSessionInfoResponse));
    Network.Get().SendPVPDRSessionInfoRequest();
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Duels);
    this.m_adventureDefCache = new AdventureDefCache(false);
    this.m_adventureWingDefCache = new AdventureWingDefCache(false);
    this.StartCoroutine(this.NotifySceneLoadedWhenReady());
  }

  public void Update() => Network.Get().ProcessNetwork();

  public void OnDestroy() => PvPDungeonRunScene.m_instance = (PvPDungeonRunScene) null;

  public GameSaveKeyId GetGSDKeyForAdventure()
  {
    AdventureDataDbfRecord adventureDataRecord = AdventureConfig.GetAdventureDataRecord(this.m_currentAdventure, AdventureModeDbId.DUNGEON_CRAWL);
    if (adventureDataRecord != null)
      return (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey;
    Debug.LogError((object) ("PvPDungeonRunScene.GetGSDKeyForAdventure called but could not find record for adventureId = " + (object) this.m_currentAdventure));
    return ~GameSaveKeyId.INVALID;
  }

  public int GetSeasonID() => this.m_seasonId;

  private void OnCollectionManagerLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_collectionManagerPrefabLoaded = true;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("PvPDungeonRunScene.OnCollectionManagerLoaded() - failed to load screen {0}", (object) assetRef));
    }
    else
    {
      this.m_collectionManager = go;
      this.m_collectionManager.SetActive(true);
      this.m_collectionManager.transform.SetParent(this.transform, false);
      this.m_collectionManager.transform.localPosition = this.CM_POS;
      this.m_collectionManager.transform.localScale = this.CM_SCALE;
    }
  }

  public void OnGuestHeroSelected(TAG_CLASS classId, GuestHeroDbfRecord record)
  {
    this.m_services.DungeonCrawlData.SelectedHeroCardDbId = (long) record.CardId;
    this.TransitionToDungeonCrawlPlayMat();
  }

  public bool TransitionToGuestHeroPicker()
  {
    int num = AssetLoader.Get().InstantiatePrefab((AssetReference) "GuestHeroPicker.prefab:3ecbc18da1de3ef4fa30532f90b20e59", new PrefabCallback<GameObject>(this.OnGuestHeroPickerLoaded)) ? 1 : 0;
    if (num != 0)
      return num != 0;
    Debug.LogError((object) "PvPDungeonRunDisplay could not load the GuestHeroPicker prefab.");
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DUELS_HEADLINE"),
      m_text = GameStrings.Get("GLUE_CHECKOUT_ERROR_GENERIC_FAILURE"),
      m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
    return num != 0;
  }

  public void TransitionToDungeonCrawlPlayMat()
  {
    if (!((UnityEngine.Object) this.m_dungeonCrawlDisplay == (UnityEngine.Object) null))
      return;
    DungeonCrawlUtil.LoadDungeonRunPrefab((DungeonCrawlUtil.DungeonRunLoadCallback) (go =>
    {
      AdventureDungeonCrawlDisplay component = go.GetComponent<AdventureDungeonCrawlDisplay>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_IDLE);
      this.m_dungeonCrawlDisplay = component;
      GameUtils.SetParent(go, (Component) this.transform);
      component.StartRun(this.m_services);
    }));
  }

  public bool TransitionFromDungeonCrawlPlayMat()
  {
    this.m_displayRoot.SetActive(true);
    Vector3 up = Vector3.up;
    up.x -= this.m_transitionStartingOffset;
    this.m_displayRoot.transform.localPosition = up;
    iTween.MoveTo(this.m_displayRoot.gameObject, iTween.Hash((object) "islocal", (object) true, (object) "position", (object) Vector3.zero, (object) "time", (object) 1, (object) "easeType", (object) "easeOutBounce", (object) "oncomplete", (object) (Action<object>) (e =>
    {
      this.m_display.EnableButtons();
      if (!((UnityEngine.Object) this.m_dungeonCrawlDisplay != (UnityEngine.Object) null))
        return;
      if (DuelsConfig.Get().RunRecentlyEnded())
        this.m_display.CheckForStatsChanged();
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_dungeonCrawlDisplay.gameObject);
      this.m_dungeonCrawlDisplay = (AdventureDungeonCrawlDisplay) null;
      Network.Get().SendPVPDRSessionInfoRequest();
    }), (object) "oncompletetarget", (object) this.gameObject));
    if (!string.IsNullOrEmpty(this.m_SlideInSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_SlideInSound);
    if (CollectionManager.Get().IsInEditMode())
    {
      CollectionManager.Get().DoneEditing();
      CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
      if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
        collectibleDisplay.OnDoneEditingDeck();
    }
    return true;
  }

  public bool NavigateBackFromPlaymat()
  {
    if (!this.m_display.GetPVPDRLobbyDataModel().HasSession | DungeonCrawlUtil.IsPVPDRSessionComplete() || DuelsConfig.IsInitialLoadoutComplete())
    {
      if (!this.TransitionFromDungeonCrawlPlayMat())
        Navigation.Push(new Navigation.NavigateBackHandler(this.NavigateBackFromPlaymat));
    }
    else if (!this.TransitionToGuestHeroPicker())
      Navigation.Push(new Navigation.NavigateBackHandler(this.NavigateBackFromPlaymat));
    return true;
  }

  public void TransitionBackFromGuestHeroPicker()
  {
    GuestHeroPickerDisplay.Get().HideTray();
    this.m_displayRoot.SetActive(true);
    this.m_displayRoot.transform.localPosition = Vector3.up;
    this.m_display.EnableButtons(false);
  }

  public void SetAdventureData()
  {
    AdventureConfig adventureConfig = AdventureConfig.Get();
    adventureConfig.SetSelectedAdventureMode(this.m_services.DungeonCrawlData.GetSelectedAdventure(), AdventureModeDbId.DUNGEON_CRAWL);
    adventureConfig.SetMission(this.m_services.DungeonCrawlData.GetMission());
  }

  public void ShowDungeonCrawlDisplay(Action<object> action)
  {
    iTween.MoveTo(this.m_dungeonCrawlDisplay.gameObject, iTween.Hash((object) "islocal", (object) true, (object) "position", (object) new Vector3(0.0f, (float) ((bool) UniversalInputManager.UsePhoneUI ? 0 : 3), 0.0f), (object) "time", (object) 1, (object) "easeType", (object) "easeOutBounce", (object) "oncomplete", (object) action, (object) "oncompletetarget", (object) this.gameObject));
    this.m_isEditingDeck = false;
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_IDLE);
  }

  public void HideDungeonCrawlDisplay(Action onCompleteCallback = null)
  {
    this.m_isTransitioningToCollection = true;
    iTween.MoveTo(this.m_dungeonCrawlDisplay.gameObject, iTween.Hash((object) "islocal", (object) true, (object) "position", (object) new Vector3((float) ((bool) UniversalInputManager.UsePhoneUI ? -180 : -110), 0.0f, 0.0f), (object) "time", (object) 1, (object) "easeType", (object) "easeOutBounce", (object) "oncomplete", (object) (Action<object>) (e =>
    {
      this.m_isTransitioningToCollection = false;
      if (onCompleteCallback == null)
        return;
      onCompleteCallback();
    }), (object) "oncompletetarget", (object) this.gameObject));
    this.m_isEditingDeck = true;
  }

  public void OnHeroPickerShown()
  {
    this.m_display.OnHeroPickerShown();
    this.m_display.EnableButtons(false);
    if ((UnityEngine.Object) this.m_displayRoot != (UnityEngine.Object) null)
      this.m_displayRoot.SetActive(false);
    if (!((UnityEngine.Object) this.m_dungeonCrawlDisplay != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_dungeonCrawlDisplay.gameObject);
    this.m_dungeonCrawlDisplay = (AdventureDungeonCrawlDisplay) null;
  }

  public void OnHeroPickerHidden() => this.m_display.EnableButtons();

  public AdventureDef GetAdventureDef(AdventureDbId advId) => this.m_adventureDefCache.GetDef(advId);

  public AdventureWingDef GetWingDef(WingDbId wingId) => this.m_adventureWingDefCache.GetDef(wingId);

  public IDungeonCrawlData GetDungeonCrawlData() => this.m_services.DungeonCrawlData;

  public override bool IsUnloading() => this.m_unloading;

  public override void Unload()
  {
    this.m_unloading = true;
    if ((bool) UniversalInputManager.UsePhoneUI)
      BnetBar.Get().ToggleActive(true);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.Unload();
    if ((UnityEngine.Object) this.m_displayRoot != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_displayRoot);
    if ((UnityEngine.Object) this.m_dungeonCrawlDisplay != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_dungeonCrawlDisplay.gameObject);
    if ((UnityEngine.Object) this.m_collectionManager != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_collectionManager.gameObject);
    if ((UnityEngine.Object) this.m_guestHeroPickerTrayDisplay != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_guestHeroPickerTrayDisplay.gameObject);
    if (this.m_adventureDefCache != null)
      this.m_adventureDefCache.Unload();
    if (this.m_adventureWingDefCache != null)
      this.m_adventureWingDefCache.Unload();
    if ((UnityEngine.Object) this.m_popupManagerRoot != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_popupManagerRoot.gameObject);
    Network.Get().RemoveNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRSessionInfoResponse));
    this.m_unloading = false;
  }

  private void DoDungeonRunTransition()
  {
    if ((bool) (UnityEngine.Object) GuestHeroPickerDisplay.Get())
    {
      this.m_services.SubsceneController.OnTransitionComplete();
      GuestHeroPickerDisplay.Get().HideTray();
    }
    else if ((UnityEngine.Object) this.m_displayRoot != (UnityEngine.Object) null && this.m_displayRoot.gameObject.activeInHierarchy)
    {
      Vector3 localPosition = this.transform.localPosition;
      localPosition.x -= this.m_transitionStartingOffset;
      Hashtable args = iTween.Hash((object) "islocal", (object) true, (object) "position", (object) localPosition, (object) "time", (object) this.m_transitionTime, (object) "easeType", (object) "easeOutBounce", (object) "oncomplete", (object) (Action<object>) (e =>
      {
        this.m_displayRoot.SetActive(false);
        this.m_services.SubsceneController.OnTransitionComplete();
        if (!((UnityEngine.Object) this.m_dungeonCrawlDisplay != (UnityEngine.Object) null))
          return;
        this.m_dungeonCrawlDisplay.EnableBackButton(true);
      }), (object) "oncompletetarget", (object) this.gameObject);
      if ((UnityEngine.Object) this.m_dungeonCrawlDisplay != (UnityEngine.Object) null)
        this.m_dungeonCrawlDisplay.EnableBackButton(false);
      iTween.MoveTo(this.m_displayRoot.gameObject, args);
      if (string.IsNullOrEmpty(this.m_SlideOutSound))
        return;
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_SlideOutSound);
    }
    else
      this.m_services.SubsceneController.OnTransitionComplete();
  }

  private void CreateServices(AdventureDbId adventureId)
  {
    this.m_assetLoadingHelper = new AssetLoadingHelper();
    this.m_assetLoadingHelper.AssetLoadingComplete += new EventHandler(this.OnAssetLoadingComplete);
    this.m_services = DungeonCrawlUtil.CreatePvPDungeonCrawlServices(adventureId, this.m_assetLoadingHelper);
  }

  private void OnAssetLoadingComplete(object sender, EventArgs args)
  {
    if (this.m_services == null || !((UnityEngine.Object) this.m_dungeonCrawlDisplay != (UnityEngine.Object) null))
      return;
    this.DoDungeonRunTransition();
  }

  private void OnGameSaveDataReceived(bool success) => this.m_gameSaveDataReceived = true;

  private void OnGuestHeroPickerLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    go.transform.SetParent(this.transform);
    this.m_guestHeroPickerTrayDisplay = go.GetComponentInChildren<GuestHeroPickerTrayDisplay>();
  }

  private void OnPVPDRStatsResponse()
  {
    Network.Get().RemoveNetHandler((object) PVPDRStatsInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRStatsResponse));
    this.m_hasStatsInfo = true;
  }

  private void OnPVPDRSessionInfoResponse()
  {
    PVPDRSessionInfoResponse sessionInfoResponse = Network.Get().GetPVPDRSessionInfoResponse();
    this.m_hasSession = sessionInfoResponse.HasSession;
    this.m_hasLatestSessionData = true;
    if (this.m_services != null)
      return;
    this.m_currentAdventure = AdventureDbId.INVALID;
    if (sessionInfoResponse.HasCurrentSeason)
    {
      this.m_seasonId = sessionInfoResponse.CurrentSeason.Season.GameContentSeason.SeasonId;
      PvpdrSeasonDbfRecord record1 = GameDbf.PvpdrSeason.GetRecord(this.m_seasonId);
      if (record1 != null)
      {
        this.m_currentAdventure = (AdventureDbId) record1.AdventureId;
        AdventureDbfRecord record2 = GameDbf.Adventure.GetRecord((int) this.m_currentAdventure);
        this.m_adventureDefCache.LoadDefForId(this.m_currentAdventure);
        this.m_adventureWingDefCache.LoadDefForId((WingDbId) record2.Wings[0].ID);
      }
    }
    this.CreateServices(this.m_currentAdventure);
    this.SetAdventureData();
  }

  private void OnScreenPrefabLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("PvPDungeonRunScene.OnScreenLoaded() - failed to load screen {0}", (object) assetRef));
    }
    else
    {
      this.m_displayRoot = go;
      this.m_displayRoot.transform.SetParent(this.transform);
      this.m_screenPrefabLoaded = true;
    }
  }

  private IEnumerator NotifySceneLoadedWhenReady()
  {
    PvPDungeonRunScene pdungeonRunScene = this;
    while (!pdungeonRunScene.m_hasStatsInfo)
      yield return (object) null;
    while (!pdungeonRunScene.m_hasLatestSessionData)
      yield return (object) null;
    GameSaveDataManager.Get().Request(pdungeonRunScene.GetGSDKeyForAdventure(), new GameSaveDataManager.OnRequestDataResponseDelegate(pdungeonRunScene.OnGameSaveDataReceived));
    while (!pdungeonRunScene.m_gameSaveDataReceived)
      yield return (object) null;
    AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) pdungeonRunScene.m_screenPrefab, new PrefabCallback<GameObject>(pdungeonRunScene.OnScreenPrefabLoaded));
    while (!pdungeonRunScene.m_screenPrefabLoaded)
      yield return (object) null;
    while ((UnityEngine.Object) pdungeonRunScene.m_display == (UnityEngine.Object) null)
    {
      pdungeonRunScene.m_display = pdungeonRunScene.m_displayRoot.GetComponentInChildren<PvPDungeonRunDisplay>();
      yield return (object) null;
    }
    while (!pdungeonRunScene.m_display.IsFinishedLoading)
      yield return (object) null;
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_IDLE);
    bool flag1 = DuelsConfig.Get().HasRecentLoss();
    bool flag2 = DuelsConfig.Get().HasRecentWin();
    bool flag3 = SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY;
    pdungeonRunScene.m_displayRoot.SetActive(!flag3);
    if (flag3)
    {
      pdungeonRunScene.m_display.GetPVPDRLobbyDataModel().RecentLoss = flag1;
      pdungeonRunScene.m_display.GetPVPDRLobbyDataModel().RecentWin = flag2;
      pdungeonRunScene.TransitionToDungeonCrawlPlayMat();
      while ((UnityEngine.Object) pdungeonRunScene.m_dungeonCrawlDisplay == (UnityEngine.Object) null)
        yield return (object) null;
    }
    while (!pdungeonRunScene.m_collectionManagerPrefabLoaded)
      yield return (object) null;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    collectibleDisplay.EnableInput(false);
    collectibleDisplay.PopulateSetFilters(true);
    SceneMgr.Get().NotifySceneLoaded();
  }

  public DuelsPopupManager GetPopupManager() => this.m_PopupManager;

  private void OnPopupManagerLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("PvPDungeonRunScene.OnPopupManagerLoaded() - failed to load screen {0}", (object) assetRef));
    }
    else
    {
      this.m_popupManagerRoot = go;
      this.m_popupManagerRoot.transform.SetParent(this.gameObject.transform);
      this.m_PopupManager = go.GetComponentInChildren<DuelsPopupManager>();
    }
  }

  public static void ShowDuelsMessagePopup(
    string header,
    string message,
    string rating,
    Action callback)
  {
    DuelsPopupManager popupManager = PvPDungeonRunScene.m_instance.GetPopupManager();
    if (!((UnityEngine.Object) popupManager != (UnityEngine.Object) null))
      return;
    popupManager.ShowNotice(header, message, rating, callback);
  }
}
