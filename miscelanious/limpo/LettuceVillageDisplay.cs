using Assets;
using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

[CustomEditClass]
[RequireComponent(typeof (WidgetTemplate))]
public class LettuceVillageDisplay : AbsSceneDisplay
{
  private const string PLAY_INTRO_ANIMATION = "PLAY_INTRO_ANIMATION";
  public const string UPDATE_VILLAGE_BUILDINGS = "UPDATE_VILLAGE_BUILDINGS";
  private bool m_mapReceived;
  private WidgetTemplate m_widget;
  public AsyncReference m_popupManagerReference;
  public AsyncReference m_LettuceVillagePC;
  public AsyncReference m_LettuceVillagePhone;
  private LettuceVillagePopupManager m_VillagePopupManager;
  private LettuceVillage m_Village;
  private VisualController m_VillageController;
  private bool m_villagePopupManagerFinishedLoading;
  private bool m_lettuceVillageFinishedChangingStates;

  private void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
  }

  public override void Start()
  {
    base.Start();
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheLettuceMap), new Action(this.OnLettuceMapNetCacheUpdated));
    this.InitializeLettuceMap();
    this.SetupVillage();
    this.InitializeMercenaryVillageShopData();
  }

  public void OnDestroy() => NetCache.Get()?.RemoveUpdatedListener(typeof (NetCache.NetCacheLettuceMap), new Action(this.OnLettuceMapNetCacheUpdated));

  public MercenaryVillageShopDataModel GetMercenaryVillageShopDataModel()
  {
    VisualController componentInChildren = this.GetComponentInChildren<VisualController>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return (MercenaryVillageShopDataModel) null;
    Widget owner = (Widget) componentInChildren.Owner;
    IDataModel model;
    if (!owner.GetDataModel(526, out model))
    {
      model = (IDataModel) new MercenaryVillageShopDataModel();
      owner.BindDataModel(model);
    }
    return model as MercenaryVillageShopDataModel;
  }

  private void OnVillagePopupManagerReady(LettuceVillagePopupManager popupManager)
  {
    if ((UnityEngine.Object) popupManager == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Failed to load village popup manager");
    }
    else
    {
      this.m_VillagePopupManager = popupManager;
      this.m_villagePopupManagerFinishedLoading = true;
      if (!((UnityEngine.Object) this.m_Village != (UnityEngine.Object) null))
        return;
      this.m_Village.SetUIReferences(this, this.m_VillagePopupManager);
    }
  }

  private void OnVillageReady(VisualController controller)
  {
    this.m_clickBlocker.SetActive(true);
    this.SetVillageController(controller);
    LettuceBountyDbfRecord bountyRecord = this.m_sceneTransitionPayload is LettuceVillageDisplay.LettuceSceneTransitionPayload transitionPayload ? transitionPayload.m_SelectedBounty : (LettuceBountyDbfRecord) null;
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.HUB || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.LETTUCE_MAP && LettuceVillageDataUtil.IsBountyTutorial(bountyRecord))
      controller.SetState("PLAY_INTRO_ANIMATION");
    else
      this.m_Village.OnVillageEntered();
  }

  private void SetVillageController(VisualController controller)
  {
    if ((UnityEngine.Object) this.m_VillageController != (UnityEngine.Object) null)
    {
      if (!((UnityEngine.Object) controller != (UnityEngine.Object) this.m_VillageController))
        return;
      Debug.LogErrorFormat("Both {0} and {1} are active! (only one is allowed)", (object) this.m_VillageController.transform.parent.name, (object) controller.transform.parent.name);
    }
    else
    {
      this.m_VillageController = controller;
      this.m_Village = controller.GetComponent<LettuceVillage>();
      if (this.m_villagePopupManagerFinishedLoading)
        this.m_Village.SetUIReferences(this, this.m_VillagePopupManager);
      this.m_VillageController.GetComponent<Widget>().RegisterDoneChangingStatesListener(new Action<object>(this.OnVillageWidgetDoneChangingStates), (object) null, true, false);
    }
  }

  private void HandleEvent(string eventName)
  {
    if (!(eventName == "TRANSITION_SCENE"))
    {
      if (!(eventName == "UPDATE_VILLAGE_BUILDINGS"))
        return;
      this.m_Village.UpdateBuildingStates();
    }
    else
    {
      object payload = this.m_widget.GetDataModel<EventDataModel>().Payload;
      if (!(payload is LettuceVillageDisplay.ZoneTransitionInfo))
        return;
      LettuceVillageDisplay.ZoneTransitionInfo zoneTransitionInfo = payload as LettuceVillageDisplay.ZoneTransitionInfo;
      if (!Network.IsLoggedIn() && !this.CanNavigateToSceneWhileOffline(zoneTransitionInfo.mode))
        DialogManager.Get().ShowReconnectHelperDialog();
      else
        this.SetNextModeAndHandleTransition(zoneTransitionInfo.mode, SceneMgr.TransitionHandlerType.NEXT_SCENE, (object) zoneTransitionInfo.transitionPayload);
    }
  }

  private void InitializeMercenaryVillageShopData()
  {
    MercenaryVillageShopDataModel villageShopDataModel = this.GetMercenaryVillageShopDataModel();
    if (villageShopDataModel == null)
      return;
    villageShopDataModel.ShopOpen = StoreManager.Get().IsOpen();
    villageShopDataModel.HasNewItems = LettuceVillageDataUtil.HasNewMercShopProducts();
  }

  private void OnVillageWidgetDoneChangingStates(object widget) => this.m_lettuceVillageFinishedChangingStates = true;

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.m_villagePopupManagerFinishedLoading)
    {
      failureMessage = "LettuceDisplay - Village popup manager never loaded.";
      return false;
    }
    this.m_VillagePopupManager.VillageIsReady = false;
    if ((UnityEngine.Object) this.m_Village == (UnityEngine.Object) null)
    {
      failureMessage = "LettuceDisplay - Village never loaded";
      return false;
    }
    if (!this.m_Village.VillageIsReady)
    {
      failureMessage = "LettuceDisplay - Village never became ready";
      return false;
    }
    if (!CollectionManager.Get().IsLettuceLoaded())
    {
      failureMessage = "LettuceDisplay - Lettuce Collection Manager never loaded.";
      return false;
    }
    if (!LettuceVillageDataUtil.Initialized)
    {
      failureMessage = "LettuceDisplay - Village Data not initialized";
      return false;
    }
    if (!this.m_mapReceived)
    {
      failureMessage = "LettuceDisplay - Map not received";
      return false;
    }
    if (!this.m_lettuceVillageFinishedChangingStates)
    {
      failureMessage = "LettuceVillageDisplay - Village never finished changing states.";
      return false;
    }
    this.m_VillagePopupManager.VillageIsReady = true;
    failureMessage = string.Empty;
    return true;
  }

  public LettuceVillage GetVillage() => this.m_Village;

  public void ShowPvEZonePortal() => this.m_VillagePopupManager.Show(LettuceVillagePopupManager.PopupType.PVE);

  protected override bool ShouldStartShown() => SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_BOUNTY_BOARD && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_MAP && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_COLLECTION && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_PLAY;

  private void SetupVillage()
  {
    this.m_popupManagerReference.RegisterReadyListener<LettuceVillagePopupManager>(new Action<LettuceVillagePopupManager>(this.OnVillagePopupManagerReady));
    this.m_LettuceVillagePC.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnVillageReady));
    this.m_LettuceVillagePhone.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnVillageReady));
  }

  private void InitializeLettuceMap()
  {
    if (Network.IsLoggedIn())
      Network.Get().RequestLettuceMap();
    else
      this.m_mapReceived = true;
  }

  private void OnLettuceMapNetCacheUpdated() => this.m_mapReceived = true;

  private bool CanNavigateToSceneWhileOffline(SceneMgr.Mode nextMode)
  {
    if (nextMode != SceneMgr.Mode.LETTUCE_MAP)
      return true;
    NetCache.NetCacheLettuceMap netObject = NetCache.Get().GetNetObject<NetCache.NetCacheLettuceMap>();
    return netObject != null && netObject.Map != null && netObject.Map.Active;
  }

  public class LettuceSceneTransitionPayload
  {
    public LettuceBountySetDbfRecord m_SelectedBountySet;
    public LettuceBounty.MercenariesBountyDifficulty m_DifficultyMode;
    public LettuceBountyDbfRecord m_SelectedBounty;
    public long m_TeamId;
    public long m_CoOpPartnerTeamId;
  }

  public class ZoneTransitionInfo
  {
    public SceneMgr.Mode mode;
    public LettuceVillageDisplay.LettuceSceneTransitionPayload transitionPayload;
  }
}
