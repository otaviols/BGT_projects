using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone.DataModels;
using Hearthstone.InGameMessage.UI;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
  [Header("General")]
  public AsyncReference m_boxWidgetRef;
  public GameObject m_rootObject;
  public WeakAssetReference m_defaultInnkeeperGreetings;
  public Widget m_eventBoxDressingWidget;
  public BoxStateInfoList m_StateInfoList;
  [Header("Box Parts")]
  public BoxLogo m_Logo;
  public BoxStartButton m_StartButton;
  public BoxDoor m_LeftDoor;
  public BoxDoor m_RightDoor;
  public BoxDisk m_Disk;
  public GameObject m_DiskCenter;
  public BoxSpinner m_TopSpinner;
  public BoxSpinner m_BottomSpinner;
  public BoxDrawer m_Drawer;
  public BoxCamera m_Camera;
  public GameObject m_OuterFrame;
  public List<Collider> m_outerPanelColliders;
  public GameObject m_letterboxingContainer;
  public GameObject m_tableTop;
  [Header("Buttons")]
  public BoxMenuButton m_PlayButton;
  public BoxScrollButton m_BattleGroundsButton;
  public BoxScrollButton m_MercenariesButton;
  public GameObject m_MercenariesButtonVisual;
  public GameObject m_MercenariesButtonActivateFX;
  public GameObject m_MercenariesButtonDeactivateFX;
  public WeakAssetReference m_MercenariesButtonActivateSound;
  public GameObject m_EmptyFourthButton;
  public GameObject m_bnetBarBackground;
  public BoxMenuButton m_GameModesButton;
  public PackOpeningButton m_OpenPacksButton;
  public BoxMenuButton m_CollectionButton;
  public StoreButton m_StoreButton;
  public QuestLogButton m_QuestLogButton;
  public Widget m_journalButtonWidget;
  public RibbonButtonsUI m_ribbonButtons;
  [Header("Renderers")]
  public Renderer m_SpotLightRenderer;
  public Renderer m_FirstButtonRenderer;
  public Renderer m_SecondButtonRenderer;
  public Renderer m_ThirdButtonRenderer;
  public Renderer m_FourthButtonRenderer;
  [Header("Materials")]
  public Color m_EnabledMaterial;
  public Color m_DisabledMaterial;
  public Color m_EnabledDrawerMaterial;
  public Color m_DisabledDrawerMaterial;
  public Texture2D m_textureCompressionTest;
  [Header("Managers")]
  public Camera m_NoFxCamera;
  public AudioListener m_AudioListener;
  public BoxLightMgr m_LightMgr;
  public BoxEventMgr m_EventMgr;
  [Header("FTUE")]
  public GameObject m_newPlayerModeBanner;
  public WidgetInstance m_tutorialPreview;
  private static Box s_instance;
  private Box.BoxStateConfig[] m_stateConfigs;
  private Box.State m_state = Box.State.STARTUP;
  private int m_pendingEffects;
  private Queue<Box.State> m_stateQueue = new Queue<Box.State>();
  private bool m_transitioningToSceneMode;
  private List<Box.TransitionFinishedListener> m_transitionFinishedListeners = new List<Box.TransitionFinishedListener>();
  private AssetHandle<Texture> m_tableTopTexture;
  private AssetHandle<Texture> m_boxTopTexture;
  private AssetHandle<Texture> m_specialEventTexture;
  private Box.ButtonType? m_queuedButtonFire;
  private bool m_waitingForNetData;
  private GameLayer m_originalLeftDoorLayer;
  private GameLayer m_originalRightDoorLayer;
  private GameLayer m_originalDrawerLayer;
  private bool m_showRibbonButtons;
  private WeakAssetReference m_eventInnkeeperGreetings;
  private GameObject m_tempInputBlocker;
  private TableTopMgr m_tableTopMgr;
  private GameObject m_setRotationDisk;
  private BoxMenuButton m_setRotationButton;
  private JournalButton m_journalButton;
  private VisualController m_boxVisualController;
  private const string SHOW_NEW_GAME_MODE_BADGE_STATE = "NewGameModeOn";
  private const string HIDE_NEW_GAME_MODE_BADGE_STATE = "NewGameModeOff";
  private const string ACTIVATE_EVENT_BOX_DRESSING = "EVENT_BOX_DRESSING_BIRTH";
  private const string DEACTIVATE_EVENT_BOX_DRESSING = "EVENT_BOX_DRESSING_DEATH";
  private bool m_eventBoxDressingActive;
  private DisableMesh_ColorBlack[] m_materialDependentComponents;
  private MusicPlaylistType m_activeMusicPlaylist = MusicPlaylistType.UI_MainTitle;
  protected List<Box.ButtonPressListener> m_buttonPressListeners = new List<Box.ButtonPressListener>();
  private int m_nextMissionId = -1;
  private bool m_waitingForSceneLoad;
  private const string SHOW_LOG_COROUTINE = "ShowQuestLogWhenReady";
  private TutorialPreviewController m_tutorialPreviewController;

  public event Action OnBoxDressingReadyOnce;

  private void Awake()
  {
    Log.LoadingScreen.Print("Box.Awake()");
    Box.s_instance = this;
    this.InitializeStateConfigs();
    if ((UnityEngine.Object) LoadingScreen.Get() != (UnityEngine.Object) null)
      LoadingScreen.Get().NotifyMainSceneObjectAwoke(this.gameObject);
    this.m_originalLeftDoorLayer = (GameLayer) this.m_LeftDoor.gameObject.layer;
    this.m_originalRightDoorLayer = (GameLayer) this.m_RightDoor.gameObject.layer;
    this.m_originalDrawerLayer = (GameLayer) this.m_Drawer.gameObject.layer;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if ((double) TransformUtil.GetAspectRatioDependentValue(0.0f, 1f, 1f) < 0.990000009536743)
        GameUtils.InstantiateGameObject("Letterboxing.prefab:303d7852a40ab4f178a3f97a102a0ea0", this.m_letterboxingContainer);
      GameObject child = AssetLoader.Get().InstantiatePrefab((AssetReference) "RibbonButtons_Phone.prefab:1b805ba741fd649cabb72b2764c755f5");
      this.m_ribbonButtons = child.GetComponent<RibbonButtonsUI>();
      this.m_ribbonButtons.Toggle(false);
      GameUtils.SetParent(child, this.m_rootObject);
      this.m_tableTopMgr = this.m_tableTop.GetComponent<TableTopMgr>();
      AssetLoader.Get().LoadAsset<Texture>((AssetReference) "TheBox_Top_phone.psd:666e602b70e7d6344be3e690de329636", new AssetHandleCallback<Texture>(this.OnBoxTopPhoneTextureLoaded));
    }
    if (RewardTrackManager.Get().HasReceivedRewardTracksFromServer)
      this.m_eventBoxDressingWidget.RegisterDoneChangingStatesListener((Action<object>) (_ => this.UpdateEventBoxDressingWithConfig()), (object) null, true, true);
    else
      RewardTrackManager.Get().OnRewardTracksReceived += new Action(this.OnRewardTracksReceived);
    this.m_journalButtonWidget.RegisterReadyListener((Action<object>) (_ => this.m_journalButton = this.m_journalButtonWidget.GetComponentInChildren<JournalButton>()), (object) null, true);
    this.m_materialDependentComponents = this.GetComponentsInChildren<DisableMesh_ColorBlack>();
  }

  private void Start()
  {
    this.InitializeNet(false);
    this.InitializeComponents();
    this.InitializeState();
    this.InitializeUI();
    if (DemoMgr.Get().IsExpoDemo())
    {
      this.m_StoreButton.gameObject.SetActive(false);
      this.m_Drawer.gameObject.SetActive(false);
      this.m_QuestLogButton.gameObject.SetActive(false);
    }
    if (this.m_state != Box.State.HUB_WITH_DRAWER)
      this.m_journalButtonWidget.Hide();
    StoreManager.Get()?.RegisterStoreShownListener(new Action(this.HideEventBoxDressing));
    StoreManager.Get()?.RegisterStoreHiddenListener(new Action(this.UpdateEventBoxDressingWithConfig));
  }

  private void OnDestroy()
  {
    Log.LoadingScreen.Print("Box.OnDestroy()");
    if ((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null)
      PegUI.Get().RemoveInputCamera(this.m_Camera.GetComponent<Camera>());
    StoreManager.Get()?.RemoveStoreShownListener(new Action(this.HideEventBoxDressing));
    StoreManager.Get()?.RemoveStoreHiddenListener(new Action(this.UpdateEventBoxDressingWithConfig));
    if ((UnityEngine.Object) LoadingScreen.Get() != (UnityEngine.Object) null)
      LoadingScreen.Get().UnregisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnTutorialSceneDestroyed));
    this.ShutdownState();
    AssetHandle.SafeDispose<Texture>(ref this.m_tableTopTexture);
    AssetHandle.SafeDispose<Texture>(ref this.m_boxTopTexture);
    AssetHandle.SafeDispose<Texture>(ref this.m_specialEventTexture);
    this.OnDestroyButton();
    Box.s_instance = (Box) null;
  }

  public static Box Get() => Box.s_instance;

  public Camera GetCamera() => this.m_Camera.GetComponent<Camera>();

  public BoxCamera GetBoxCamera() => this.m_Camera;

  public Camera GetNoFxCamera() => this.m_NoFxCamera;

  public AudioListener GetAudioListener() => this.m_AudioListener;

  public JournalButton GetJournalButton() => this.m_journalButton;

  public Texture2D GetTextureCompressionTestTexture() => this.m_textureCompressionTest;

  public Box.State GetState() => this.m_state;

  public bool ChangeState(Box.State state)
  {
    if (state == Box.State.INVALID || this.m_state == state)
      return false;
    if (this.HasPendingEffects())
      this.QueueStateChange(state);
    else
      this.ChangeStateNow(state);
    return true;
  }

  public void UpdateState()
  {
    if (this.m_state == Box.State.STARTUP)
      this.UpdateState_Startup();
    else if (this.m_state == Box.State.PRESS_START)
      this.UpdateState_PressStart();
    else if (this.m_state == Box.State.LOADING_HUB)
      this.UpdateState_LoadingHub();
    else if (this.m_state == Box.State.LOADING)
      this.UpdateState_Loading();
    else if (this.m_state == Box.State.HUB)
      this.UpdateState_Hub();
    else if (this.m_state == Box.State.HUB_WITH_DRAWER)
      this.UpdateState_HubWithDrawer();
    else if (this.m_state == Box.State.OPEN)
      this.UpdateState_Open();
    else if (this.m_state == Box.State.CLOSED)
      this.UpdateState_Closed();
    else if (this.m_state == Box.State.ERROR)
      this.UpdateState_Error();
    else if (this.m_state == Box.State.SET_ROTATION_LOADING)
      this.UpdateState_SetRotation();
    else if (this.m_state == Box.State.SET_ROTATION)
      this.UpdateState_SetRotation();
    else if (this.m_state == Box.State.SET_ROTATION_OPEN)
      this.UpdateState_SetRotationOpen();
    else
      Debug.LogError((object) string.Format("Box.UpdateState() - unhandled state {0}", (object) this.m_state));
  }

  public BoxLightMgr GetLightMgr() => this.m_LightMgr;

  public BoxLightStateType GetLightState() => this.m_LightMgr.GetActiveState();

  public void ChangeLightState(BoxLightStateType stateType) => this.m_LightMgr.ChangeState(stateType);

  public void SetLightState(BoxLightStateType stateType) => this.m_LightMgr.SetState(stateType);

  public BoxEventMgr GetEventMgr() => this.m_EventMgr;

  public Spell GetEventSpell(BoxEventType eventType) => this.m_EventMgr.GetEventSpell(eventType);

  public bool HasPendingEffects() => this.m_pendingEffects > 0;

  public bool IsBusy() => this.HasPendingEffects() || this.m_stateQueue.Count > 0;

  public bool IsTransitioningToSceneMode() => this.m_transitioningToSceneMode;

  public void OnAnimStarted() => ++this.m_pendingEffects;

  public void OnAnimFinished()
  {
    --this.m_pendingEffects;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_OuterFrame.SetActive(false);
      if (GameUtils.CanCheckTutorialCompletion() && !GameUtils.IsAnyTutorialComplete() && this.m_state == Box.State.OPEN)
        this.m_tableTopMgr.HideTableTop();
    }
    if (this.HasPendingEffects())
      return;
    if (this.m_stateQueue.Count == 0)
    {
      this.UpdateUIEvents();
      if (!this.m_transitioningToSceneMode)
        return;
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        bool show = this.m_state == Box.State.HUB_WITH_DRAWER;
        if (show != this.m_showRibbonButtons)
          this.ToggleRibbonUI(show);
      }
      this.FireTransitionFinishedEvent();
      this.m_transitioningToSceneMode = false;
    }
    else
      this.ChangeStateQueued();
  }

  public void OnLoggedIn() => this.InitializeNet(true);

  public void AddTransitionFinishedListener(Box.TransitionFinishedCallback callback) => this.AddTransitionFinishedListener(callback, (object) null);

  public void AddTransitionFinishedListener(
    Box.TransitionFinishedCallback callback,
    object userData)
  {
    Box.TransitionFinishedListener finishedListener = new Box.TransitionFinishedListener();
    finishedListener.SetCallback(callback);
    finishedListener.SetUserData(userData);
    if (this.m_transitionFinishedListeners.Contains(finishedListener))
      return;
    this.m_transitionFinishedListeners.Add(finishedListener);
  }

  public bool RemoveTransitionFinishedListener(Box.TransitionFinishedCallback callback) => this.RemoveTransitionFinishedListener(callback, (object) null);

  public bool RemoveTransitionFinishedListener(
    Box.TransitionFinishedCallback callback,
    object userData)
  {
    Box.TransitionFinishedListener finishedListener = new Box.TransitionFinishedListener();
    finishedListener.SetCallback(callback);
    finishedListener.SetUserData(userData);
    return this.m_transitionFinishedListeners.Remove(finishedListener);
  }

  public void SetToIgnoreFullScreenEffects(bool ignoreEffects)
  {
    if (ignoreEffects)
    {
      LayerUtils.ReplaceLayer(this.m_LeftDoor.gameObject, GameLayer.IgnoreFullScreenEffects, this.m_originalLeftDoorLayer);
      LayerUtils.ReplaceLayer(this.m_RightDoor.gameObject, GameLayer.IgnoreFullScreenEffects, this.m_originalRightDoorLayer);
      LayerUtils.ReplaceLayer(this.m_Drawer.gameObject, GameLayer.IgnoreFullScreenEffects, this.m_originalDrawerLayer);
    }
    else
    {
      LayerUtils.ReplaceLayer(this.m_LeftDoor.gameObject, this.m_originalLeftDoorLayer, GameLayer.IgnoreFullScreenEffects);
      LayerUtils.ReplaceLayer(this.m_RightDoor.gameObject, this.m_originalRightDoorLayer, GameLayer.IgnoreFullScreenEffects);
      LayerUtils.ReplaceLayer(this.m_Drawer.gameObject, this.m_originalDrawerLayer, GameLayer.IgnoreFullScreenEffects);
    }
  }

  public void PlayBoxMusic() => MusicManager.Get()?.StartPlaylist(this.m_activeMusicPlaylist);

  public void PlayInnkeeperGreetings()
  {
    if (string.IsNullOrEmpty(this.m_eventInnkeeperGreetings.AssetString) && string.IsNullOrEmpty(this.m_defaultInnkeeperGreetings.AssetString))
    {
      Debug.LogError((object) "Innkeeper greetings missing, assign a value to 'Default Innkeeper Greetings' on the Box");
    }
    else
    {
      string assetRef = string.IsNullOrEmpty(this.m_eventInnkeeperGreetings.AssetString) ? this.m_defaultInnkeeperGreetings.AssetString : this.m_eventInnkeeperGreetings.AssetString;
      SoundManager.Get()?.LoadAndPlay((AssetReference) assetRef);
    }
  }

  private void InitializeStateConfigs()
  {
    this.m_stateConfigs = new Box.BoxStateConfig[Enum.GetValues(typeof (Box.State)).Length];
    this.m_stateConfigs[1] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[2] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.SHOWN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.SHOWN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[4] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[3] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_ignore = true
      },
      m_camState = {
        m_ignore = true
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.DISABLED
      }
    };
    this.m_stateConfigs[5] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.MAINMENU
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[6] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.MAINMENU
      },
      m_drawerState = {
        m_state = BoxDrawer.State.OPENED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED_WITH_DRAWER
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[7] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.OPENED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED_BOX_OPENED
      },
      m_camState = {
        m_state = BoxCamera.State.OPENED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.DISABLED
      }
    };
    this.m_stateConfigs[8] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[9] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[10] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[11] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.CLOSED
      },
      m_diskState = {
        m_state = BoxDisk.State.MAINMENU
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.CLOSED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
    this.m_stateConfigs[12] = new Box.BoxStateConfig()
    {
      m_logoState = {
        m_state = BoxLogo.State.HIDDEN
      },
      m_startButtonState = {
        m_state = BoxStartButton.State.HIDDEN
      },
      m_doorState = {
        m_state = BoxDoor.State.OPENED
      },
      m_diskState = {
        m_state = BoxDisk.State.LOADING
      },
      m_drawerState = {
        m_state = BoxDrawer.State.CLOSED
      },
      m_camState = {
        m_state = BoxCamera.State.OPENED
      },
      m_boxDressingState = {
        m_state = EventBoxDressing.State.ENABLED
      }
    };
  }

  private void InitializeState()
  {
    this.m_state = Box.State.STARTUP;
    bool flag = GameMgr.Get().WasTutorial() && !GameMgr.Get().WasSpectator();
    SceneMgr service;
    if (ServiceManager.TryGet<SceneMgr>(out service))
    {
      if (flag)
      {
        this.m_state = Box.State.LOADING;
      }
      else
      {
        service.RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
        service.RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
        this.m_state = this.TranslateSceneModeToBoxState(service.GetMode());
      }
    }
    this.UpdateState();
    this.m_TopSpinner.Spin();
    this.m_BottomSpinner.Spin();
    if (flag)
    {
      LoadingScreen.Get().RegisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnTutorialSceneDestroyed));
      if (GameUtils.IsTraditionalTutorialComplete())
        PopupDisplayManager.Get().HealUpPopup.QueuePopupAterTutorialIfNotSeen(HealUpPopup.HealUpPopupCompletedTutorial.Traditional);
    }
    if (this.m_state != Box.State.HUB_WITH_DRAWER)
      return;
    this.ToggleRibbonUI(true);
    this.m_journalButtonWidget.Show();
    this.m_journalButtonWidget.TriggerEvent("ENABLE_INTERACTION");
  }

  private void OnTutorialSceneDestroyed(object userData)
  {
    LoadingScreen.Get().UnregisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnTutorialSceneDestroyed));
    this.SetHubButtonsActive(false);
    Spell eventSpell = this.GetEventSpell(BoxEventType.TUTORIAL_PLAY);
    eventSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnTutorialPlaySpellStateDeathFinished));
    eventSpell.ActivateState(SpellStateType.DEATH);
  }

  private void OnTutorialPlaySpellStateDeathFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    this.SetHubButtonsActive(true);
    SceneMgr sceneMgr = SceneMgr.Get();
    sceneMgr.RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    sceneMgr.RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    this.ChangeStateToReflectSceneMode(SceneMgr.Get().GetMode(), false);
  }

  private void ShutdownState()
  {
    if ((UnityEngine.Object) this.m_StoreButton != (UnityEngine.Object) null)
      this.m_StoreButton.Unload();
    SceneMgr service;
    if (!ServiceManager.TryGet<SceneMgr>(out service))
      return;
    service.UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    service.UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
  }

  private void QueueStateChange(Box.State state) => this.m_stateQueue.Enqueue(state);

  private void ChangeStateQueued() => this.ChangeStateNow(this.m_stateQueue.Dequeue());

  private void ChangeStateNow(Box.State state)
  {
    bool flag = SetRotationManager.Get().ShouldShowSetRotationIntro();
    if (!flag)
    {
      if ((UnityEngine.Object) this.m_DiskCenter != (UnityEngine.Object) null)
        this.m_DiskCenter.SetActive(true);
      if ((UnityEngine.Object) this.m_setRotationDisk != (UnityEngine.Object) null)
        this.m_setRotationDisk.SetActive(false);
    }
    if (state == Box.State.OPEN & flag)
      state = Box.State.SET_ROTATION_OPEN;
    this.m_state = state;
    this.TrackBoxInteractable();
    switch (state)
    {
      case Box.State.STARTUP:
        this.ChangeState_Startup();
        break;
      case Box.State.PRESS_START:
        this.ChangeState_PressStart();
        break;
      case Box.State.LOADING:
        this.ChangeState_Loading();
        break;
      case Box.State.LOADING_HUB:
        this.ChangeState_LoadingHub();
        break;
      case Box.State.HUB:
        this.ChangeState_Hub();
        break;
      case Box.State.HUB_WITH_DRAWER:
        this.ChangeState_HubWithDrawer();
        break;
      case Box.State.OPEN:
        this.ChangeState_Open();
        break;
      case Box.State.CLOSED:
        this.ChangeState_Closed();
        break;
      case Box.State.ERROR:
        this.ChangeState_Error();
        break;
      case Box.State.SET_ROTATION_LOADING:
        this.ChangeState_SetRotationLoading();
        break;
      case Box.State.SET_ROTATION:
        this.ChangeState_SetRotation();
        break;
      case Box.State.SET_ROTATION_OPEN:
        this.ChangeState_SetRotationOpen();
        break;
      default:
        Debug.LogError((object) string.Format("Box.ChangeStateNow() - unhandled state {0}", (object) state));
        break;
    }
    this.UpdateUIEvents();
  }

  private void ChangeStateToReflectSceneMode(SceneMgr.Mode mode, bool isSceneActuallyLoaded)
  {
    Box.State boxState = this.TranslateSceneModeToBoxState(mode);
    bool flag = SetRotationManager.Get().ShouldShowSetRotationIntro();
    if (mode == SceneMgr.Mode.HUB & flag)
    {
      this.ChangeState(Box.State.SET_ROTATION_LOADING);
      if (isSceneActuallyLoaded)
        this.StartCoroutine(this.SetRotation_StartSetRotationIntro());
    }
    else if (mode == SceneMgr.Mode.TOURNAMENT & flag)
    {
      this.ChangeState(Box.State.SET_ROTATION_OPEN);
      UserAttentionManager.StartBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
      this.m_transitioningToSceneMode = true;
    }
    else if (!SceneMgr.Get().IsDoingSceneDrivenTransition() && this.ChangeState(boxState))
      this.m_transitioningToSceneMode = true;
    this.m_LightMgr.ChangeState(this.TranslateSceneModeToLightState(mode));
    if (boxState != Box.State.HUB)
      return;
    this.StartCoroutine(this.UpdateCameraForTutorialPreview());
  }

  private IEnumerator UpdateCameraForTutorialPreview()
  {
    while (NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>() == null)
      yield return (object) null;
    this.m_Camera.ChangeState(!GameUtils.IsAnyTutorialComplete() ? BoxCamera.State.CLOSED_TUTORIAL : BoxCamera.State.CLOSED);
  }

  public void TryToStartSetRotationFromHub()
  {
    if (!((int) SceneMgr.Get().GetMode() == 3 & SetRotationManager.Get().ShouldShowSetRotationIntro()))
      return;
    this.ChangeState(Box.State.SET_ROTATION_LOADING);
    this.StartCoroutine(this.SetRotation_StartSetRotationIntro());
  }

  private Box.State TranslateSceneModeToBoxState(SceneMgr.Mode mode)
  {
    switch (mode)
    {
      case SceneMgr.Mode.STARTUP:
        return Box.State.STARTUP;
      case SceneMgr.Mode.LOGIN:
        return Box.State.INVALID;
      case SceneMgr.Mode.HUB:
        return !GameUtils.IsAnyTutorialComplete() ? Box.State.HUB : Box.State.HUB_WITH_DRAWER;
      case SceneMgr.Mode.GAMEPLAY:
        return Box.State.INVALID;
      case SceneMgr.Mode.FATAL_ERROR:
        return Box.State.ERROR;
      default:
        return Box.State.OPEN;
    }
  }

  private BoxLightStateType TranslateSceneModeToLightState(SceneMgr.Mode mode)
  {
    switch (mode)
    {
      case SceneMgr.Mode.LOGIN:
      case SceneMgr.Mode.GAMEPLAY:
        return BoxLightStateType.INVALID;
      case SceneMgr.Mode.COLLECTIONMANAGER:
      case SceneMgr.Mode.TAVERN_BRAWL:
      case SceneMgr.Mode.FIRESIDE_GATHERING:
      case SceneMgr.Mode.BACON_COLLECTION:
        return BoxLightStateType.COLLECTION;
      case SceneMgr.Mode.PACKOPENING:
        return BoxLightStateType.PACK_OPENING;
      case SceneMgr.Mode.TOURNAMENT:
        return BoxLightStateType.TOURNAMENT;
      case SceneMgr.Mode.FRIENDLY:
      case SceneMgr.Mode.ADVENTURE:
      case SceneMgr.Mode.BACON:
      case SceneMgr.Mode.GAME_MODE:
      case SceneMgr.Mode.PVP_DUNGEON_RUN:
      case SceneMgr.Mode.LETTUCE_VILLAGE:
      case SceneMgr.Mode.LETTUCE_BOUNTY_BOARD:
      case SceneMgr.Mode.LETTUCE_MAP:
      case SceneMgr.Mode.LETTUCE_PLAY:
      case SceneMgr.Mode.LETTUCE_COLLECTION:
      case SceneMgr.Mode.LETTUCE_PACK_OPENING:
        return BoxLightStateType.ADVENTURE;
      case SceneMgr.Mode.DRAFT:
        return BoxLightStateType.ARENA;
      default:
        return BoxLightStateType.DEFAULT;
    }
  }

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    switch (mode)
    {
      case SceneMgr.Mode.STARTUP:
        break;
      case SceneMgr.Mode.GAMEPLAY:
        break;
      case SceneMgr.Mode.RESET:
        break;
      default:
        if (prevMode == SceneMgr.Mode.HUB)
        {
          this.ChangeState(Box.State.LOADING);
          this.m_StoreButton.Unload();
        }
        else if (mode == SceneMgr.Mode.HUB)
        {
          this.ChangeStateToReflectSceneMode(mode, false);
          this.m_waitingForSceneLoad = true;
        }
        else if (this.ShouldUseLoadingHubState(mode, prevMode))
          this.ChangeState(Box.State.LOADING_HUB);
        else if (!SceneMgr.Get().IsDoingSceneDrivenTransition())
          this.ChangeState(Box.State.LOADING);
        this.ClearQueuedButtonFireEvent();
        this.UpdateUIEvents();
        break;
    }
  }

  private bool ShouldUseLoadingHubState(SceneMgr.Mode mode, SceneMgr.Mode prevMode) => mode == SceneMgr.Mode.FRIENDLY && prevMode != SceneMgr.Mode.HUB || mode == SceneMgr.Mode.FIRESIDE_GATHERING && prevMode != SceneMgr.Mode.HUB || prevMode == SceneMgr.Mode.COLLECTIONMANAGER && (mode == SceneMgr.Mode.ADVENTURE || mode == SceneMgr.Mode.TOURNAMENT) || mode == SceneMgr.Mode.COLLECTIONMANAGER && (prevMode == SceneMgr.Mode.ADVENTURE || prevMode == SceneMgr.Mode.TOURNAMENT || prevMode == SceneMgr.Mode.FIRESIDE_GATHERING) || prevMode == SceneMgr.Mode.BACON_COLLECTION && mode == SceneMgr.Mode.BACON || mode == SceneMgr.Mode.BACON_COLLECTION && prevMode == SceneMgr.Mode.BACON;

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    this.ChangeStateToReflectSceneMode(mode, true);
    if (!this.m_waitingForSceneLoad)
      return;
    this.m_waitingForSceneLoad = false;
    if (!this.m_queuedButtonFire.HasValue)
      return;
    this.FireButtonPressEvent(this.m_queuedButtonFire.Value);
    this.m_queuedButtonFire = new Box.ButtonType?();
  }

  private void ChangeState_Startup()
  {
    this.m_state = Box.State.STARTUP;
    this.ChangeStateUsingConfig();
  }

  private void ChangeState_PressStart()
  {
    this.m_state = Box.State.PRESS_START;
    this.ChangeStateUsingConfig();
  }

  private void ChangeState_SetRotationLoading()
  {
    this.m_state = Box.State.SET_ROTATION_LOADING;
    this.ChangeStateUsingConfig();
  }

  private void ChangeState_SetRotation()
  {
    this.m_state = Box.State.SET_ROTATION;
    this.ChangeStateUsingConfig();
  }

  private void ChangeState_SetRotationOpen()
  {
    this.m_state = Box.State.SET_ROTATION_OPEN;
    this.StartCoroutine(this.SetRotationOpen_ChangeState());
  }

  private void ChangeState_LoadingHub()
  {
    this.m_state = Box.State.LOADING_HUB;
    this.ChangeStateUsingConfig();
  }

  private void ChangeState_Loading()
  {
    this.m_state = Box.State.LOADING;
    this.ChangeStateUsingConfig();
  }

  private void ChangeState_Hub()
  {
    this.m_state = Box.State.HUB;
    this.UpdateUI();
    this.ChangeStateUsingConfig();
    this.InitializeTutorialPreviewController();
  }

  private void ChangeState_HubWithDrawer()
  {
    this.m_state = Box.State.HUB_WITH_DRAWER;
    this.UpdateUI();
    this.m_Camera.EnableAccelerometer();
    this.ChangeStateUsingConfig();
    this.InitializeTutorialPreviewController();
  }

  private void ChangeState_Open()
  {
    this.m_state = Box.State.OPEN;
    this.ChangeStateUsingConfig();
  }

  private void ChangeState_Closed()
  {
    this.m_state = Box.State.CLOSED;
    this.ChangeStateUsingConfig();
  }

  private void ChangeState_Error()
  {
    this.m_state = Box.State.ERROR;
    this.ChangeStateUsingConfig();
  }

  private void UpdateState_Startup()
  {
    this.m_state = Box.State.STARTUP;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_PressStart()
  {
    this.m_state = Box.State.PRESS_START;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_SetRotationLoading()
  {
    this.m_state = Box.State.SET_ROTATION_LOADING;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_SetRotation()
  {
    this.m_state = Box.State.SET_ROTATION;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_SetRotationOpen()
  {
    this.m_state = Box.State.SET_ROTATION_OPEN;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_LoadingHub()
  {
    this.m_state = Box.State.LOADING_HUB;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_Loading()
  {
    this.m_state = Box.State.LOADING;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_Hub()
  {
    this.m_state = Box.State.HUB;
    this.UpdateUI();
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_HubWithDrawer()
  {
    this.m_state = Box.State.HUB_WITH_DRAWER;
    this.m_Camera.EnableAccelerometer();
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_Open()
  {
    this.m_state = Box.State.OPEN;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_Closed()
  {
    this.m_state = Box.State.CLOSED;
    this.UpdateStateUsingConfig();
  }

  private void UpdateState_Error()
  {
    this.m_state = Box.State.ERROR;
    this.UpdateStateUsingConfig();
  }

  private void ChangeStateUsingConfig()
  {
    Box.BoxStateConfig stateConfig = this.m_stateConfigs[(int) this.m_state];
    if (!stateConfig.m_logoState.m_ignore)
      this.m_Logo.ChangeState(stateConfig.m_logoState.m_state);
    if (!stateConfig.m_startButtonState.m_ignore)
      this.m_StartButton.ChangeState(stateConfig.m_startButtonState.m_state);
    if (!stateConfig.m_doorState.m_ignore)
    {
      this.m_LeftDoor.ChangeState(stateConfig.m_doorState.m_state);
      this.m_RightDoor.ChangeState(stateConfig.m_doorState.m_state);
    }
    if (!stateConfig.m_diskState.m_ignore)
      this.m_Disk.ChangeState(stateConfig.m_diskState.m_state);
    if (!stateConfig.m_drawerState.m_ignore)
    {
      if (!(bool) UniversalInputManager.UsePhoneUI)
      {
        this.m_Drawer.ChangeState(stateConfig.m_drawerState.m_state);
      }
      else
      {
        bool show = this.m_state == Box.State.HUB_WITH_DRAWER;
        if (!show && show != this.m_showRibbonButtons)
          this.ToggleRibbonUI(show);
      }
    }
    if (!stateConfig.m_camState.m_ignore)
      this.m_Camera.ChangeState(stateConfig.m_camState.m_state);
    this.UpdateEventBoxDressingWithConfig();
  }

  private void ToggleRibbonUI(bool show)
  {
    if ((UnityEngine.Object) this.m_ribbonButtons == (UnityEngine.Object) null)
      return;
    this.m_ribbonButtons.Toggle(show);
    this.m_showRibbonButtons = show;
  }

  private void UpdateStateUsingConfig()
  {
    Box.BoxStateConfig stateConfig = this.m_stateConfigs[(int) this.m_state];
    if (!stateConfig.m_logoState.m_ignore)
      this.m_Logo.UpdateState(stateConfig.m_logoState.m_state);
    if (!stateConfig.m_startButtonState.m_ignore)
      this.m_StartButton.UpdateState(stateConfig.m_startButtonState.m_state);
    if (!stateConfig.m_storeButtonState.m_ignore)
      this.m_StoreButton.UpdateState(stateConfig.m_storeButtonState.m_state);
    if (!stateConfig.m_doorState.m_ignore)
    {
      this.m_LeftDoor.ChangeState(stateConfig.m_doorState.m_state);
      this.m_RightDoor.ChangeState(stateConfig.m_doorState.m_state);
    }
    if (!stateConfig.m_diskState.m_ignore)
      this.m_Disk.UpdateState(stateConfig.m_diskState.m_state);
    this.m_TopSpinner.Reset();
    this.m_BottomSpinner.Reset();
    if (!stateConfig.m_drawerState.m_ignore)
      this.m_Drawer.UpdateState(stateConfig.m_drawerState.m_state);
    if (stateConfig.m_camState.m_ignore)
      return;
    this.m_Camera.UpdateState(stateConfig.m_camState.m_state);
  }

  private void FireTransitionFinishedEvent()
  {
    foreach (Box.TransitionFinishedListener finishedListener in this.m_transitionFinishedListeners.ToArray())
      finishedListener.Fire();
  }

  private void InitializeUI()
  {
    PegUI.Get().AddInputCamera(this.m_Camera.GetComponent<Camera>());
    this.m_boxWidgetRef.RegisterReadyListener<Widget>(new Action<Widget>(this.BoxWidgetIsReady));
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_Drawer.gameObject.SetActive(false);
      this.m_ribbonButtons.m_collectionManagerRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCollectionButtonPressed));
      this.m_ribbonButtons.m_packOpeningRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOpenPacksButtonPressed));
      this.m_ribbonButtons.m_questLogRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnQuestButtonPressed));
      this.m_ribbonButtons.m_storeRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStoreButtonReleased));
    }
    else
    {
      this.m_OpenPacksButton.SetText(GameStrings.Get("GLUE_OPEN_PACKS"));
      this.m_CollectionButton.SetText(GameStrings.Get("GLUE_MY_COLLECTION"));
      this.m_QuestLogButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnQuestButtonPressed));
      this.m_StoreButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStoreButtonReleased));
    }
    this.RegisterButtonEvents((PegUIElement) this.m_OpenPacksButton);
    this.RegisterButtonEvents((PegUIElement) this.m_CollectionButton);
    this.RegisterButtonEvents((PegUIElement) this.m_PlayButton);
    this.RegisterButtonEvents((PegUIElement) this.m_BattleGroundsButton);
    this.RegisterButtonEvents((PegUIElement) this.m_MercenariesButton);
    this.RegisterButtonEvents((PegUIElement) this.m_GameModesButton);
    this.m_StartButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStartButtonPressed));
    switch (InputUtil.GetInputScheme())
    {
      case InputScheme.KEYBOARD_MOUSE:
        this.m_StartButton.SetText(GameStrings.Get("GLUE_START_CLICK"));
        break;
      case InputScheme.TOUCH:
        this.m_StartButton.SetText(GameStrings.Get("GLUE_START_TOUCH"));
        break;
      case InputScheme.GAMEPAD:
        this.m_StartButton.SetText(GameStrings.Get("GLUE_START_PRESS"));
        break;
    }
    this.SetupButtonText();
    this.UpdateUI();
  }

  private void InitializeComponents()
  {
    this.m_Logo.SetParent(this);
    this.m_Logo.SetInfo(this.m_StateInfoList.m_LogoInfo);
    this.m_StartButton.SetParent(this);
    this.m_StartButton.SetInfo(this.m_StateInfoList.m_StartButtonInfo);
    this.m_LeftDoor.SetParent(this);
    this.m_LeftDoor.SetInfo(this.m_StateInfoList.m_LeftDoorInfo);
    this.m_RightDoor.SetParent(this);
    this.m_RightDoor.SetInfo(this.m_StateInfoList.m_RightDoorInfo);
    this.m_RightDoor.EnableMain(true);
    this.m_Disk.SetParent(this);
    this.m_Disk.SetInfo(this.m_StateInfoList.m_DiskInfo);
    this.m_TopSpinner.SetParent(this);
    this.m_TopSpinner.SetInfo(this.m_StateInfoList.m_SpinnerInfo);
    this.m_BottomSpinner.SetParent(this);
    this.m_BottomSpinner.SetInfo(this.m_StateInfoList.m_SpinnerInfo);
    this.m_Drawer.SetParent(this);
    this.m_Drawer.SetInfo(this.m_StateInfoList.m_DrawerInfo);
    this.m_Camera.SetParent(this);
    this.m_Camera.SetInfo(this.m_StateInfoList.m_CameraInfo);
  }

  public void UpdateUI()
  {
    this.UpdateUIState();
    this.UpdateUIEvents();
  }

  private void UpdateUIState()
  {
    if (this.m_waitingForNetData)
    {
      this.SetPackCount(-1);
      this.HighlightButton((BoxMenuButton) this.m_OpenPacksButton, false);
      this.HighlightButton(this.m_PlayButton, false);
      this.HighlightButton((BoxMenuButton) this.m_BattleGroundsButton, false);
      this.HighlightButton(this.m_CollectionButton, false);
      this.HighlightButton(this.m_GameModesButton, false);
      this.HighlightButton((BoxMenuButton) this.m_MercenariesButton, false);
      this.HideGameModesButton();
      this.m_newPlayerModeBanner.SetActive(false);
    }
    else
    {
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2013)
      {
        netObject.Games.Practice = false;
        netObject.Games.Tournament = false;
      }
      int totalBoosterCount = BoosterPackUtils.GetTotalBoosterCount();
      this.SetPackCount(totalBoosterCount);
      this.HighlightButton((BoxMenuButton) this.m_OpenPacksButton, totalBoosterCount > 0 && !Options.Get().GetBool(Option.HAS_SEEN_PACK_OPENING, false));
      int num = this.UpdateModesButton() ? 1 : 0;
      bool flag = Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false) && !Options.Get().GetBool(Option.HAS_SEEN_COLLECTIONMANAGER_AFTER_PRACTICE, false);
      this.HighlightButton(this.m_CollectionButton, ((num != 0 ? 0 : (netObject.Collection.Manager ? 1 : 0)) & (flag ? 1 : 0)) != 0);
      this.ToggleDrawerButtonState(netObject.Collection.Manager, this.m_CollectionButton);
      this.SetupNewPlayerBanner();
    }
  }

  private void SetupNewPlayerBanner()
  {
    if (this.m_state != Box.State.HUB)
    {
      this.m_newPlayerModeBanner.SetActive(false);
    }
    else
    {
      bool flag1 = !GameUtils.IsAnyTutorialComplete();
      bool flag2 = GameUtils.GetNextTutorial() > 3;
      this.m_newPlayerModeBanner.SetActive(flag1 && !flag2);
    }
  }

  private bool UpdateModesButton()
  {
    if (!GameUtils.IsTraditionalTutorialComplete())
    {
      this.HideGameModesButton();
      return false;
    }
    this.ShowGameModesButton();
    int num1 = Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false) ? 1 : 0;
    int num2 = GameModeUtils.CanAccessGameModes() ? 1 : 0;
    this.m_GameModesButton.SetText(GameStrings.Get(num2 != 0 ? "GLUE_GAME_MODES" : "GLUE_TOOLTIP_BUTTON_ADVENTURE_HEADLINE"));
    bool flag1 = num2 != 0 && GameModeUtils.ShouldSeeSoloAdventuresMovedPopup();
    bool highlightOn = num1 == 0 | flag1;
    this.HighlightButton(this.m_GameModesButton, highlightOn);
    if ((UnityEngine.Object) this.m_boxVisualController == (UnityEngine.Object) null)
      return highlightOn;
    bool flag2 = GameModeDisplay.ShouldSeeNewSoloAdventureBanner();
    bool flag3 = GameModeDisplay.ShouldSeeNewTavernBrawlBanner();
    this.m_boxVisualController.SetState((!GameUtils.IsMercenariesVillageTutorialComplete() || highlightOn ? 0 : (flag2 | flag3 ? 1 : 0)) != 0 ? "NewGameModeOn" : "NewGameModeOff");
    return highlightOn;
  }

  public void PlayMercenariesButtonActivation(bool activate) => this.StartCoroutine(this.DoMercenariesButtonActivationAnimation());

  private IEnumerator DoMercenariesButtonActivationAnimation()
  {
    Animator component = this.m_MercenariesButtonVisual.GetComponent<Animator>();
    component.StopPlayback();
    component.Play("TavernBrawl_ButtonActivate");
    this.m_MercenariesButtonActivateFX.GetComponent<ParticleSystem>().Play();
    this.m_MercenariesButton.TriggerOut();
    if (this.m_MercenariesButtonActivateSound.AssetString != string.Empty)
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_MercenariesButtonActivateSound.AssetString);
    yield return (object) new WaitForSeconds(0.65f);
    CameraShakeMgr.Shake(Camera.main, new Vector3(0.5f, 0.5f, 0.5f), 0.3f);
  }

  private void BoxWidgetIsReady(Widget widget) => this.m_boxVisualController = widget.FindWidgetComponent<VisualController>();

  private bool IsCollectionReady() => CollectionManager.Get() != null && CollectionManager.Get().IsFullyLoaded();

  private IEnumerator UpdateUIWhenCollectionReady()
  {
    while (!this.IsCollectionReady())
      yield return (object) null;
    this.UpdateUI();
  }

  public void DisableAllButtons()
  {
    this.DisableButton((PegUIElement) this.m_PlayButton);
    this.DisableButton((PegUIElement) this.m_BattleGroundsButton);
    this.DisableButton((PegUIElement) this.m_GameModesButton);
    this.DisableButton((PegUIElement) this.m_MercenariesButton);
    this.DisableButton((PegUIElement) this.m_setRotationButton);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.DisableButton(this.m_ribbonButtons.m_collectionManagerRibbon);
      this.DisableButton(this.m_ribbonButtons.m_packOpeningRibbon);
      this.DisableButton(this.m_ribbonButtons.m_questLogRibbon);
      this.DisableButton(this.m_ribbonButtons.m_storeRibbon);
    }
    else
    {
      this.DisableButton((PegUIElement) this.m_OpenPacksButton);
      this.DisableButton((PegUIElement) this.m_CollectionButton);
      this.DisableButton((PegUIElement) this.m_QuestLogButton);
      this.DisableButton((PegUIElement) this.m_StoreButton);
      this.m_journalButtonWidget.TriggerEvent("DISABLE_INTERACTION");
    }
    this.ToggleButtonTextureState(false, this.m_PlayButton);
    this.ToggleButtonTextureState(false, (BoxMenuButton) this.m_BattleGroundsButton);
    this.ToggleButtonTextureState(false, this.m_GameModesButton);
    this.ToggleButtonTextureState(false, (BoxMenuButton) this.m_MercenariesButton);
    this.ToggleDrawerButtonState(false, (BoxMenuButton) this.m_OpenPacksButton);
    this.ToggleDrawerButtonState(false, this.m_CollectionButton);
    this.ToggleButtonTextureState(false, this.m_setRotationButton);
  }

  private void SetHubButtonsActive(bool active)
  {
    if ((UnityEngine.Object) this.m_PlayButton != (UnityEngine.Object) null)
      this.m_PlayButton.gameObject.SetActive(active);
    if ((UnityEngine.Object) this.m_BattleGroundsButton != (UnityEngine.Object) null)
      this.m_BattleGroundsButton.gameObject.SetActive(active);
    if ((UnityEngine.Object) this.m_GameModesButton != (UnityEngine.Object) null)
      this.m_GameModesButton.gameObject.SetActive(active);
    if ((UnityEngine.Object) this.m_MercenariesButton != (UnityEngine.Object) null)
      this.m_MercenariesButton.gameObject.SetActive(active);
    if ((UnityEngine.Object) this.m_Drawer != (UnityEngine.Object) null)
      this.m_Drawer.gameObject.SetActive(!(bool) UniversalInputManager.UsePhoneUI && active);
    if ((UnityEngine.Object) this.m_journalButtonWidget != (UnityEngine.Object) null)
      this.m_journalButtonWidget.gameObject.SetActive(active);
    if ((UnityEngine.Object) this.m_bnetBarBackground != (UnityEngine.Object) null)
      this.m_bnetBarBackground.SetActive(active);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if ((UnityEngine.Object) this.m_ribbonButtons.m_collectionManagerRibbon != (UnityEngine.Object) null)
        this.m_ribbonButtons.m_collectionManagerRibbon.gameObject.SetActive(active);
      if ((UnityEngine.Object) this.m_ribbonButtons.m_packOpeningRibbon != (UnityEngine.Object) null)
        this.m_ribbonButtons.m_packOpeningRibbon.gameObject.SetActive(active);
      if (!((UnityEngine.Object) this.m_ribbonButtons.m_storeRibbon != (UnityEngine.Object) null))
        return;
      this.m_ribbonButtons.m_storeRibbon.gameObject.SetActive(active);
    }
    else
    {
      if ((UnityEngine.Object) this.m_OpenPacksButton != (UnityEngine.Object) null)
        this.m_OpenPacksButton.gameObject.SetActive(active);
      if ((UnityEngine.Object) this.m_CollectionButton != (UnityEngine.Object) null)
        this.m_CollectionButton.gameObject.SetActive(active);
      if (!((UnityEngine.Object) this.m_StoreButton != (UnityEngine.Object) null))
        return;
      this.m_StoreButton.gameObject.SetActive(active);
    }
  }

  private bool CanEnableUIEvents() => !this.HasPendingEffects() && this.m_stateQueue.Count <= 0 && this.m_state != Box.State.INVALID && this.m_state != Box.State.STARTUP && this.m_state != Box.State.LOADING && this.m_state != Box.State.LOADING_HUB && this.m_state != Box.State.OPEN;

  private void ToggleButtonTextureState(bool enabled, BoxMenuButton button)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) null)
      return;
    if (enabled)
      button.m_TextMesh.TextColor = this.m_EnabledMaterial;
    else
      button.m_TextMesh.TextColor = this.m_DisabledMaterial;
  }

  private void ToggleDrawerButtonState(bool enabled, BoxMenuButton button)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) null)
      return;
    if (enabled)
      button.m_TextMesh.TextColor = this.m_EnabledDrawerMaterial;
    else
      button.m_TextMesh.TextColor = this.m_DisabledDrawerMaterial;
  }

  private void HighlightButton(BoxMenuButton button, bool highlightOn)
  {
    if ((UnityEngine.Object) button.m_HighlightState == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("Box.HighlighButton {0} - highlight state is null", (object) button));
    }
    else
    {
      ActorStateType stateType = highlightOn ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.HIGHLIGHT_OFF;
      button.m_HighlightState.ChangeState(stateType);
    }
  }

  private bool IsButtonHighlighted(BoxMenuButton button) => button.m_HighlightState.CurrentState == ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE;

  private void SetRotation_ShowRotationDisk()
  {
    if ((UnityEngine.Object) this.m_DiskCenter != (UnityEngine.Object) null)
      this.m_DiskCenter.SetActive(false);
    if ((UnityEngine.Object) this.m_setRotationDisk != (UnityEngine.Object) null)
    {
      this.m_setRotationDisk.SetActive(true);
    }
    else
    {
      this.m_StoreButton.gameObject.SetActive(false);
      this.m_QuestLogButton.gameObject.SetActive(false);
      this.m_journalButtonWidget.Hide();
      this.m_setRotationDisk = AssetLoader.Get().InstantiatePrefab((AssetReference) "TheBox_CenterDisk_SetRotation.prefab:6f2fa714f0d129e4197fd2922f544816");
      this.m_setRotationDisk.SetActive(true);
      this.m_setRotationDisk.transform.parent = this.m_Disk.transform;
      this.m_setRotationDisk.transform.localPosition = Vector3.zero;
      this.m_setRotationDisk.transform.localRotation = Quaternion.identity;
      EventBoxDressing componentInChildren1 = this.m_eventBoxDressingWidget?.GetComponentInChildren<EventBoxDressing>();
      CenterDiskSetRotation component = this.m_setRotationDisk.GetComponent<CenterDiskSetRotation>();
      if ((UnityEngine.Object) componentInChildren1 != (UnityEngine.Object) null && (UnityEngine.Object) component != (UnityEngine.Object) null)
        component.ApplyBoxDressingMaterials(componentInChildren1.GetBoxDressingMaterials());
      this.m_setRotationButton = this.m_setRotationDisk.GetComponentInChildren<BoxMenuButton>();
      HighlightState componentInChildren2 = this.m_setRotationButton.GetComponentInChildren<HighlightState>();
      if ((UnityEngine.Object) componentInChildren2 != (UnityEngine.Object) null)
        componentInChildren2.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
      this.RegisterButtonEvents((PegUIElement) this.m_setRotationButton);
    }
  }

  private IEnumerator SetRotationOpen_ChangeState()
  {
    Box.BoxStateConfig stateConfig = this.m_stateConfigs[12];
    if (!stateConfig.m_logoState.m_ignore)
      this.m_Logo.ChangeState(stateConfig.m_logoState.m_state);
    if (!stateConfig.m_startButtonState.m_ignore)
      this.m_StartButton.ChangeState(stateConfig.m_startButtonState.m_state);
    if (!stateConfig.m_doorState.m_ignore)
    {
      this.m_LeftDoor.ChangeState(stateConfig.m_doorState.m_state);
      this.m_RightDoor.ChangeState(stateConfig.m_doorState.m_state);
    }
    if (!stateConfig.m_diskState.m_ignore)
      this.m_Disk.ChangeState(stateConfig.m_diskState.m_state);
    if (!stateConfig.m_camState.m_ignore)
      this.m_Camera.ChangeState(BoxCamera.State.SET_ROTATION_OPENED);
    SetRotationClock setRotationClock = SetRotationClock.Get();
    if ((UnityEngine.Object) setRotationClock == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "SetRotationOpen_ChangeState clock = null");
    }
    else
    {
      setRotationClock.StartTheClock();
      yield break;
    }
  }

  private IEnumerator SetRotation_StartSetRotationIntro()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    Box box = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      box.SetRotation_FinishShowingRewards();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    box.ResetSetRotationPopupProgress();
    UserAttentionManager.StartBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
    NotificationManager.Get().DestroyAllPopUps();
    PopupDisplayManager.Get().ReadyToShowPopups();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) box.StartCoroutine(PopupDisplayManager.Get().WaitForAllPopups());
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void SetRotation_ShowNerfedCards_DialogHidden(DialogBase dialog, object userData) => this.SetRotation_FinishShowingRewards();

  private void SetRotation_FinishShowingRewards()
  {
    this.ChangeState(Box.State.SET_ROTATION);
    this.SetRotation_ShowRotationDisk();
  }

  private void ShowEventBoxDressing()
  {
    if ((UnityEngine.Object) this.m_eventBoxDressingWidget == (UnityEngine.Object) null || this.m_eventBoxDressingActive)
      return;
    EventDetailsDataModel detailsForCurrentEvent = RewardTrackManager.Get()?.GetEventDetailsForCurrentEvent();
    Spawnable componentInChildren1 = this.m_eventBoxDressingWidget?.GetComponentInChildren<Spawnable>();
    if ((SceneMgr.Get().GetMode() == SceneMgr.Mode.HUB ? 1 : (SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN ? 1 : 0)) == 0 | (GameUtils.CanCheckTutorialCompletion() && !GameUtils.IsAnyTutorialComplete()) || detailsForCurrentEvent == null || (UnityEngine.Object) componentInChildren1 == (UnityEngine.Object) null)
    {
      Action dressingReadyOnce = this.OnBoxDressingReadyOnce;
      if (dressingReadyOnce != null)
        dressingReadyOnce();
      this.OnBoxDressingReadyOnce = (Action) null;
    }
    else
    {
      componentInChildren1.RegisterDoneChangingStatesListener((Action<object>) (_ =>
      {
        EventBoxDressing componentInChildren2 = this.m_eventBoxDressingWidget?.GetComponentInChildren<EventBoxDressing>();
        if ((UnityEngine.Object) componentInChildren2 != (UnityEngine.Object) null)
        {
          this.ApplyBoxDressingMaterials(componentInChildren2.GetBoxDressingMaterials());
          MusicPlaylistType playlistType = componentInChildren2.GetPlaylistType();
          switch (playlistType)
          {
            case MusicPlaylistType.Invalid:
            case MusicPlaylistType.UI_MainTitle:
              if (!string.IsNullOrEmpty(componentInChildren2.GetInnkeeperGreetings().AssetString))
              {
                this.m_eventInnkeeperGreetings = componentInChildren2.GetInnkeeperGreetings();
                break;
              }
              break;
            default:
              this.m_activeMusicPlaylist = playlistType;
              goto case MusicPlaylistType.Invalid;
          }
        }
        Widget boxDressingWidget = this.m_eventBoxDressingWidget;
        if (boxDressingWidget != null)
          boxDressingWidget.TriggerEvent("EVENT_BOX_DRESSING_BIRTH");
        Action dressingReadyOnce = this.OnBoxDressingReadyOnce;
        if (dressingReadyOnce != null)
          dressingReadyOnce();
        this.OnBoxDressingReadyOnce = (Action) null;
      }), (object) null, true, true);
      this.m_eventBoxDressingWidget?.BindDataModel((IDataModel) detailsForCurrentEvent);
      this.m_eventBoxDressingActive = true;
    }
  }

  private void HideEventBoxDressing()
  {
    if (!this.m_eventBoxDressingActive)
      return;
    this.m_eventBoxDressingWidget.TriggerEvent("EVENT_BOX_DRESSING_DEATH");
    this.m_eventBoxDressingActive = false;
  }

  private void OnRewardTracksReceived()
  {
    this.UpdateEventBoxDressingWithConfig();
    RewardTrackManager.Get().OnRewardTracksReceived -= new Action(this.OnRewardTracksReceived);
  }

  private void UpdateEventBoxDressingWithConfig()
  {
    Box.BoxStateConfig stateConfig = this.m_stateConfigs[(int) this.m_state];
    if (stateConfig.m_boxDressingState.m_ignore)
      return;
    if (stateConfig.m_boxDressingState.m_state == EventBoxDressing.State.ENABLED)
      this.ShowEventBoxDressing();
    else
      this.HideEventBoxDressing();
  }

  private void ApplyMaterialToRenderer(Renderer renderer, Material material)
  {
    if (!((UnityEngine.Object) renderer != (UnityEngine.Object) null) || !((UnityEngine.Object) material != (UnityEngine.Object) null))
      return;
    RendererExtension.SetMaterial(renderer, material);
  }

  private void ApplyBoxDressingMaterials(EventBoxDressing.BoxDressingMaterials materials)
  {
    if (materials == null)
      return;
    if ((UnityEngine.Object) this.m_tableTop != (UnityEngine.Object) null && (UnityEngine.Object) materials.TableMaterial != (UnityEngine.Object) null)
      this.ApplyMaterialToRenderer(this.m_tableTop.GetComponent<Renderer>(), materials.TableMaterial);
    if ((UnityEngine.Object) this.m_BottomSpinner != (UnityEngine.Object) null && (UnityEngine.Object) materials.BottomSpinnerMaterial != (UnityEngine.Object) null)
    {
      this.ApplyMaterialToRenderer(this.m_BottomSpinner.GetComponent<Renderer>(), materials.BottomSpinnerMaterial);
      this.m_BottomSpinner.MaterialChanged();
    }
    this.ApplyMaterialToRenderer(this.m_SpotLightRenderer, materials.SpotLightMaterial);
    if ((UnityEngine.Object) materials.BoxMaterial == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) this.m_LeftDoor != (UnityEngine.Object) null)
      this.ApplyMaterialToRenderer(this.m_LeftDoor.GetComponent<Renderer>(), materials.BoxMaterial);
    if ((UnityEngine.Object) this.m_RightDoor != (UnityEngine.Object) null)
      this.ApplyMaterialToRenderer(this.m_RightDoor.GetComponent<Renderer>(), materials.BoxMaterial);
    if ((UnityEngine.Object) this.m_DiskCenter != (UnityEngine.Object) null)
      this.ApplyMaterialToRenderer(this.m_DiskCenter.GetComponent<Renderer>(), materials.BoxMaterial);
    if ((UnityEngine.Object) this.m_Drawer != (UnityEngine.Object) null)
      this.ApplyMaterialToRenderer(this.m_Drawer.GetComponent<Renderer>(), materials.BoxMaterial);
    if ((UnityEngine.Object) this.m_CollectionButton != (UnityEngine.Object) null)
      this.ApplyMaterialToRenderer(this.m_CollectionButton.GetComponent<Renderer>(), materials.BoxMaterial);
    if ((UnityEngine.Object) this.m_OpenPacksButton != (UnityEngine.Object) null)
      this.ApplyMaterialToRenderer(this.m_OpenPacksButton.GetComponent<Renderer>(), materials.BoxMaterial);
    this.ApplyMaterialToRenderer(this.m_FirstButtonRenderer, materials.BoxMaterial);
    this.ApplyMaterialToRenderer(this.m_SecondButtonRenderer, materials.BoxMaterial);
    this.ApplyMaterialToRenderer(this.m_ThirdButtonRenderer, materials.BoxMaterial);
    this.ApplyMaterialToRenderer(this.m_FourthButtonRenderer, materials.BoxMaterial);
    if ((UnityEngine.Object) this.m_EmptyFourthButton != (UnityEngine.Object) null)
      this.ApplyMaterialToRenderer(this.m_EmptyFourthButton.GetComponent<Renderer>(), materials.BoxMaterial);
    this.OnMaterialsUpdated();
  }

  private void OnBoxTopPhoneTextureLoaded(
    AssetReference assetRef,
    AssetHandle<Texture> newTexture,
    object callbackData)
  {
    AssetHandle.Take<Texture>(ref this.m_boxTopTexture, newTexture);
    foreach (MeshRenderer componentsInChild in this.gameObject.GetComponentsInChildren<MeshRenderer>())
    {
      Material sharedMaterial = RendererExtension.GetSharedMaterial((Renderer) componentsInChild);
      if ((UnityEngine.Object) sharedMaterial != (UnityEngine.Object) null && sharedMaterial.HasProperty("_MainTex"))
      {
        Texture mainTexture = sharedMaterial.mainTexture;
        if ((UnityEngine.Object) mainTexture != (UnityEngine.Object) null && mainTexture.name.Equals("TheBox_Top"))
          RendererExtension.GetMaterial((Renderer) componentsInChild).mainTexture = (Texture) newTexture;
      }
    }
    this.OnMaterialsUpdated();
  }

  private void OnMaterialsUpdated()
  {
    if (this.m_materialDependentComponents == null)
      return;
    foreach (DisableMesh_ColorBlack dependentComponent in this.m_materialDependentComponents)
      dependentComponent.HandleMaterialChanged();
  }

  public void RegisterButtonEvents(PegUIElement button)
  {
    button.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonPressed));
    button.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonMouseOver));
    button.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonMouseOut));
  }

  public void EnableButton(PegUIElement button)
  {
    button.SetEnabled(true);
    PegUIElement buttonFromButton = this.GetRibbonButtonFromButton(button);
    if (!((UnityEngine.Object) buttonFromButton != (UnityEngine.Object) null) || !((UnityEngine.Object) buttonFromButton != (UnityEngine.Object) button))
      return;
    this.EnableButton(buttonFromButton);
  }

  public void DisableButton(PegUIElement button)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) null)
      return;
    button.SetEnabled(false);
    TooltipZone component = button.GetComponent<TooltipZone>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.HideTooltip();
    PegUIElement buttonFromButton = this.GetRibbonButtonFromButton(button);
    if (!((UnityEngine.Object) buttonFromButton != (UnityEngine.Object) null) || !((UnityEngine.Object) buttonFromButton != (UnityEngine.Object) button))
      return;
    this.DisableButton(buttonFromButton);
  }

  private void ShowGameModesButton()
  {
    this.m_GameModesButton.gameObject.SetActive(true);
    this.m_EmptyFourthButton.gameObject.SetActive(false);
  }

  private void HideGameModesButton()
  {
    this.m_GameModesButton.gameObject.SetActive(false);
    this.m_EmptyFourthButton.gameObject.SetActive(true);
  }

  protected virtual void SetupButtonText()
  {
    this.SetupStartButtonText();
    this.m_PlayButton.SetText(GameStrings.Get("GLUE_TRADITIONAL"));
    this.m_BattleGroundsButton.SetText(GameStrings.Get("GLUE_BACON"));
    this.m_MercenariesButton.SetText(GameStrings.Get("GLUE_MERCENARIES"));
    this.m_GameModesButton.SetText(GameStrings.Get("GLUE_GAME_MODES"));
  }

  private void SetupStartButtonText()
  {
    switch (InputUtil.GetInputScheme())
    {
      case InputScheme.KEYBOARD_MOUSE:
        this.m_StartButton.SetText(GameStrings.Get("GLUE_START_CLICK"));
        break;
      case InputScheme.TOUCH:
        this.m_StartButton.SetText(GameStrings.Get("GLUE_START_TOUCH"));
        break;
      case InputScheme.GAMEPAD:
        this.m_StartButton.SetText(GameStrings.Get("GLUE_START_PRESS"));
        break;
    }
  }

  private void OnButtonPressed(UIEvent e)
  {
    PegUIElement element = e.GetElement();
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    bool flag1 = false;
    bool flag2 = false;
    if (netObject != null)
    {
      flag1 = netObject.Games.Tournament;
      flag2 = netObject.Collection.Manager;
    }
    if (this.m_newPlayerModeBanner.activeSelf)
      this.m_newPlayerModeBanner.SetActive(false);
    BoxMenuButton boxMenuButton = (BoxMenuButton) element;
    if ((UnityEngine.Object) boxMenuButton == (UnityEngine.Object) this.m_StartButton)
      this.OnStartButtonPressed(e);
    else if ((UnityEngine.Object) boxMenuButton == (UnityEngine.Object) this.m_PlayButton & flag1)
      this.OnTraditionalModeButtonPressed(e);
    else if ((UnityEngine.Object) boxMenuButton == (UnityEngine.Object) this.m_BattleGroundsButton)
      this.OnBattleGroundsButtonPressed(e);
    else if ((UnityEngine.Object) boxMenuButton == (UnityEngine.Object) this.m_GameModesButton)
      this.OnModesButtonPressed(e);
    else if ((UnityEngine.Object) boxMenuButton == (UnityEngine.Object) this.m_MercenariesButton)
      this.OnMercenariesButtonPressed(e);
    else if ((UnityEngine.Object) boxMenuButton == (UnityEngine.Object) this.m_OpenPacksButton)
      this.OnOpenPacksButtonPressed(e);
    else if ((UnityEngine.Object) boxMenuButton == (UnityEngine.Object) this.m_CollectionButton & flag2)
    {
      this.OnCollectionButtonPressed(e);
    }
    else
    {
      if (!((UnityEngine.Object) boxMenuButton == (UnityEngine.Object) this.m_setRotationButton))
        return;
      this.OnSetRotationButtonPressed(e);
    }
  }

  private void OnButtonMouseOver(UIEvent e)
  {
    TooltipZone component = e.GetElement().gameObject.GetComponent<TooltipZone>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null || (UnityEngine.Object) this.m_tutorialPreviewController != (UnityEngine.Object) null && this.m_tutorialPreviewController.IsPlayingPreview)
      return;
    string bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    bool flag1 = false;
    bool flag2 = false;
    if (netObject != null)
    {
      flag1 = netObject.Games.Tournament;
      flag2 = netObject.Collection.Manager;
    }
    if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_PlayButton.gameObject & flag1)
      bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_TRADITIONAL_DESC");
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_BattleGroundsButton.gameObject)
      bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_BACON_DESC");
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_GameModesButton.gameObject)
      bodytext = !GameModeUtils.CanAccessGameModes() ? GameStrings.Get("GLUE_TOOLTIP_BUTTON_ADVENTURE_DESC") : GameStrings.Get("GLUE_TOOLTIP_BUTTON_GAME_MODES_DESC");
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_MercenariesButton.gameObject)
      bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_MERCENARIES_DESC");
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_OpenPacksButton.gameObject)
      bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_PACKOPEN_DESC");
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_CollectionButton.gameObject & flag2)
      bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_COLLECTION_DESC");
    if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_PlayButton.gameObject)
      component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_TRADITIONAL_HEADLINE"), bodytext);
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_BattleGroundsButton.gameObject)
      component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_BACON_HEADLINE"), bodytext);
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_GameModesButton.gameObject)
    {
      string key = GameModeUtils.CanAccessGameModes() ? "GLUE_TOOLTIP_BUTTON_GAME_MODES_HEADLINE" : "GLUE_TOOLTIP_BUTTON_ADVENTURE_HEADLINE";
      component.ShowBoxTooltip(GameStrings.Get(key), bodytext);
    }
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_MercenariesButton.gameObject)
      component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_MERCENARIES_HEADLINE"), bodytext);
    else if ((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_OpenPacksButton.gameObject)
    {
      component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_PACKOPEN_HEADLINE"), bodytext);
    }
    else
    {
      if (!((UnityEngine.Object) component.targetObject == (UnityEngine.Object) this.m_CollectionButton.gameObject))
        return;
      component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_COLLECTION_HEADLINE"), bodytext);
    }
  }

  private void OnButtonMouseOut(UIEvent e)
  {
    TooltipZone component = e.GetElement().gameObject.GetComponent<TooltipZone>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.HideTooltip();
  }

  public virtual void OnStartButtonPressed(UIEvent e)
  {
    if (!ServiceManager.IsAvailable<SceneMgr>())
      this.ChangeState(Box.State.HUB);
    else
      this.FireButtonPressEvent(Box.ButtonType.START);
  }

  public virtual void OnOpenPacksButtonPressed(UIEvent e)
  {
    if (!Network.IsLoggedIn())
      this.ShowReconnectPopup(e, new Box.ButtonPressFunction(this.OnOpenPacksButtonPressed));
    else if (!ServiceManager.IsAvailable<SceneMgr>())
      this.ChangeState(Box.State.OPEN);
    else
      this.FireButtonPressEvent(Box.ButtonType.OPEN_PACKS);
  }

  public virtual void OnCollectionButtonPressed(UIEvent e)
  {
    if (!ServiceManager.IsAvailable<SceneMgr>())
      this.ChangeState(Box.State.OPEN);
    else
      this.FireButtonPressEvent(Box.ButtonType.COLLECTION);
  }

  public virtual void OnSetRotationButtonPressed(UIEvent e)
  {
    Log.Box.Print("Set Rotation Button Pressed!");
    if (!ServiceManager.IsAvailable<SceneMgr>())
    {
      this.ChangeState(Box.State.SET_ROTATION_OPEN);
    }
    else
    {
      AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_PLAY);
      this.FireButtonPressEvent(Box.ButtonType.SET_ROTATION);
    }
  }

  public virtual void OnQuestButtonPressed(UIEvent e)
  {
    JournalButton componentInChildren = this.m_ribbonButtons.m_journalButtonWidget.GetComponentInChildren<JournalButton>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    componentInChildren.ShowJournal();
  }

  private void SetButtonSelected(BoxMenuButton button)
  {
    this.HighlightButton((BoxMenuButton) this.m_MercenariesButton, false);
    this.HighlightButton(this.m_PlayButton, false);
    this.HighlightButton((BoxMenuButton) this.m_BattleGroundsButton, false);
    if ((UnityEngine.Object) button == (UnityEngine.Object) null)
      return;
    this.HighlightButton(button, true);
  }

  private void OnTraditionalModeButtonPressed(UIEvent e)
  {
    this.m_nextMissionId = GameUtils.GetNextTutorial();
    if (this.m_nextMissionId == 3)
    {
      if (GameUtils.TutorialPreviewVideosEnabled())
      {
        if ((UnityEngine.Object) this.m_tutorialPreviewController == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) "Tutorial preview controller is not loaded yet.");
          return;
        }
        if (this.m_tutorialPreviewController.IsAnimating)
          return;
        this.ShowTraditionalPreviewVideo();
      }
      else
        this.StartTraditionalTutorial();
      TelemetryManager.Client().SendFTUEButtonClicked("traditional");
    }
    else if (this.m_nextMissionId != 0)
      this.StartTraditionalTutorial();
    else
      this.PlayTraditionalMode();
  }

  public void OnBattleGroundsButtonPressed(UIEvent e)
  {
    if (!Network.IsLoggedIn())
      this.ShowReconnectPopup(e, new Box.ButtonPressFunction(this.OnBattleGroundsButtonPressed));
    else if (GameUtils.IsBattleGroundsTutorialComplete())
    {
      this.PlayBattlegroundsMode();
    }
    else
    {
      if (GameUtils.TutorialPreviewVideosEnabled())
      {
        if ((UnityEngine.Object) this.m_tutorialPreviewController == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) "Tutorial preview controller is not loaded yet.");
          return;
        }
        if (this.m_tutorialPreviewController.IsAnimating)
          return;
        this.ShowBattlegroundsPreviewVideo();
      }
      else
        this.StartBattlegroundsTutorial();
      TelemetryManager.Client().SendFTUEButtonClicked("battlegrounds");
    }
  }

  public void OnMercenariesButtonPressed(UIEvent e)
  {
    if (!Network.IsLoggedIn())
    {
      this.ShowReconnectPopup(e, new Box.ButtonPressFunction(this.OnMercenariesButtonPressed));
    }
    else
    {
      NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
      if (netObject == null)
        Debug.LogWarning((object) "Mercenaries Player info has not loaded yet");
      else if (GameUtils.IsMercenariesPrologueBountyComplete(netObject))
      {
        this.PlayMercenariesMode();
      }
      else
      {
        if (GameUtils.TutorialPreviewVideosEnabled())
        {
          if ((UnityEngine.Object) this.m_tutorialPreviewController == (UnityEngine.Object) null)
          {
            Debug.LogWarning((object) "Tutorial preview controller is not loaded yet.");
            return;
          }
          if (this.m_tutorialPreviewController.IsAnimating)
            return;
          this.ShowMercenariesPreviewVideo();
        }
        else
          this.StartMercenariesTutorial();
        TelemetryManager.Client().SendFTUEButtonClicked("mercenaries");
      }
    }
  }

  public void OnModesButtonPressed(UIEvent e)
  {
    if (!Network.IsLoggedIn())
    {
      this.ShowReconnectPopup(e, new Box.ButtonPressFunction(this.OnModesButtonPressed));
    }
    else
    {
      if (SceneMgr.Get() == null || DialogManager.Get().ShowingDialog())
        return;
      if (GameModeUtils.CanAccessGameModes())
        this.FireButtonPressEvent(Box.ButtonType.GAME_MODES);
      else
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.ADVENTURE);
    }
  }

  private void ShowTraditionalPreviewVideo()
  {
    if (!GameUtils.IsAnyTutorialComplete())
    {
      this.m_Camera.ChangeState(BoxCamera.State.CLOSED_TUTORIAL_VIDEO_PREVIEW);
      this.SetButtonSelected(this.m_PlayButton);
    }
    this.m_tutorialPreviewController.StartTraditionalTutorialPreviewVideo(new Action(this.StartTraditionalTutorial));
    this.FireButtonPressEvent(Box.ButtonType.TRADITIONAL, true);
  }

  private void ShowBattlegroundsPreviewVideo()
  {
    if (!GameUtils.IsAnyTutorialComplete())
    {
      this.m_Camera.ChangeState(BoxCamera.State.CLOSED_TUTORIAL_VIDEO_PREVIEW);
      this.SetButtonSelected((BoxMenuButton) this.m_BattleGroundsButton);
    }
    this.m_tutorialPreviewController.StartBattleGroundsTutorialPreviewVideo(new Action(this.StartBattlegroundsTutorial));
    this.FireButtonPressEvent(Box.ButtonType.BACON, true);
  }

  private void ShowMercenariesPreviewVideo()
  {
    if ((UnityEngine.Object) this.m_tutorialPreviewController == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "TutorialPreviewController is null.");
    }
    else
    {
      if (!GameUtils.IsAnyTutorialComplete())
      {
        this.m_Camera.ChangeState(BoxCamera.State.CLOSED_TUTORIAL_VIDEO_PREVIEW);
        this.SetButtonSelected((BoxMenuButton) this.m_MercenariesButton);
      }
      this.m_tutorialPreviewController.StartMercenariesTutorialPreviewVideo(new Action(this.StartMercenariesTutorial));
      this.FireButtonPressEvent(Box.ButtonType.MERCENARIES, true);
    }
  }

  private void StartTraditionalTutorial()
  {
    this.SetButtonSelected((BoxMenuButton) null);
    MusicManager.Get().StopPlaylist();
    this.ChangeState(Box.State.CLOSED);
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    GameMgr.Get().FindGame(GameType.GT_TUTORIAL, FormatType.FT_WILD, this.m_nextMissionId);
  }

  private void StartBattlegroundsTutorial()
  {
    this.SetButtonSelected((BoxMenuButton) null);
    GameMgr.Get().FindGame(GameType.GT_VS_AI, FormatType.FT_WILD, 3539);
  }

  private void StartMercenariesTutorial()
  {
    this.SetButtonSelected((BoxMenuButton) null);
    LettuceBountySetDbfRecord prologueRecord = GameDbf.LettuceBountySet.GetRecord((Predicate<LettuceBountySetDbfRecord>) (r => r.IsTutorial && SpecialEventManager.Get().IsEventActive(r.Event, true)));
    LettuceVillageDisplay.LettuceSceneTransitionPayload sceneTransitionPayload = new LettuceVillageDisplay.LettuceSceneTransitionPayload()
    {
      m_SelectedBountySet = prologueRecord,
      m_SelectedBounty = GameDbf.LettuceBounty.GetRecord((Predicate<LettuceBountyDbfRecord>) (r => r.BountySetId == prologueRecord.ID))
    };
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_MAP, sceneTransitionPayload: ((object) sceneTransitionPayload));
  }

  private void PlayTraditionalMode()
  {
    if (!ServiceManager.IsAvailable<SceneMgr>())
    {
      this.ChangeState(Box.State.OPEN);
    }
    else
    {
      AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_PLAY);
      this.FireButtonPressEvent(Box.ButtonType.TRADITIONAL);
    }
  }

  private void PlayBattlegroundsMode()
  {
    if (SceneMgr.Get() == null)
      this.ChangeState(Box.State.OPEN);
    else
      this.FireButtonPressEvent(Box.ButtonType.BACON);
  }

  private void PlayMercenariesMode()
  {
    if ((bool) (UnityEngine.Object) this.m_tutorialPreviewController && this.m_tutorialPreviewController.IsPlayingPreview)
      this.m_tutorialPreviewController.ClosePortal();
    NarrativeManager.Get().PreloadMercenaryTutorialDialogue();
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE);
  }

  public virtual void OnStoreButtonReleased(UIEvent e)
  {
    if (!Network.IsLoggedIn())
    {
      Log.Store.PrintDebug("Cannot open Shop due to being offline.");
      this.ShowReconnectPopup(e, new Box.ButtonPressFunction(this.OnStoreButtonReleased));
    }
    else if (FriendChallengeMgr.Get().HasChallenge())
    {
      Log.Store.PrintDebug("Cannot open Shop due to having friendly challenge.");
    }
    else
    {
      StoreManager.Get()?.Catalog.TryRefreshStaleProductAvailability();
      string unableToOpenReason;
      Blizzard.T5.Logging.LogLevel reasonLogLevel;
      if (!this.IsShopButtonReadyToOpen(out unableToOpenReason, out reasonLogLevel))
      {
        Log.Store.Print(reasonLogLevel, false, unableToOpenReason);
        SoundManager.Get().LoadAndPlay((AssetReference) "Store_closed_button_click.prefab:a6b74848e2c7e5748a20524b40fe6c1e", this.gameObject);
      }
      else
      {
        this.FireButtonPressEvent(Box.ButtonType.STORE);
        FriendChallengeMgr.Get().OnStoreOpened();
        SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681", this.gameObject);
        StoreManager.Get().RegisterStoreShownListener(new Action(this.OnStoreShown));
        StoreManager.Get().StartGeneralTransaction();
      }
    }
  }

  public bool IsShopButtonReadyToOpen(out string unableToOpenReason, out Blizzard.T5.Logging.LogLevel reasonLogLevel)
  {
    unableToOpenReason = "";
    reasonLogLevel = Blizzard.T5.Logging.LogLevel.None;
    if (FriendChallengeMgr.Get().HasChallenge())
    {
      unableToOpenReason = "Cannot open Shop due to having friendly challenge.";
      reasonLogLevel = Blizzard.T5.Logging.LogLevel.Debug;
      return false;
    }
    StoreManager storeManager = StoreManager.Get();
    storeManager?.Catalog.TryRefreshStaleProductAvailability();
    if (storeManager == null)
    {
      unableToOpenReason = "Cannot open Shop due to null StoreManager.";
      reasonLogLevel = Blizzard.T5.Logging.LogLevel.Debug;
      return false;
    }
    if (!storeManager.IsOpen())
    {
      unableToOpenReason = "Cannot open Shop due to availability error";
      reasonLogLevel = Blizzard.T5.Logging.LogLevel.None;
      return false;
    }
    if (this.m_StoreButton.IsVisualClosed())
    {
      unableToOpenReason = "Cannot open Shop due to button is visually closed.";
      reasonLogLevel = Blizzard.T5.Logging.LogLevel.Debug;
      return false;
    }
    if (SetRotationManager.Get().CheckForSetRotationRollover())
    {
      unableToOpenReason = "Cannot open Shop due to pending set rotation rollover.";
      reasonLogLevel = Blizzard.T5.Logging.LogLevel.Debug;
      return false;
    }
    if (PlayerMigrationManager.Get() != null && PlayerMigrationManager.Get().CheckForPlayerMigrationRequired())
    {
      unableToOpenReason = "Cannot open Shop due to pending player migration.";
      reasonLogLevel = Blizzard.T5.Logging.LogLevel.Debug;
      return false;
    }
    if (!storeManager.IsVintageStoreEnabled() && storeManager.Catalog.GetTiers(ShopType.GENERAL_STORE).Count == 0)
    {
      unableToOpenReason = "Cannot open Shop due to no valid tier data received.";
      reasonLogLevel = Blizzard.T5.Logging.LogLevel.Warning;
      return false;
    }
    if (SceneMgr.Get() != null && SceneMgr.Get().GetMode() == SceneMgr.Mode.HUB && !SceneMgr.Get().IsTransitionNowOrPending())
      return true;
    unableToOpenReason = "Cannot open Shop due to invalid scene state.";
    reasonLogLevel = Blizzard.T5.Logging.LogLevel.Warning;
    return false;
  }

  public virtual void FireButtonPressEvent(Box.ButtonType buttonType, bool isShowingTutorialPreview = false)
  {
    if (this.m_waitingForSceneLoad)
    {
      this.m_queuedButtonFire = new Box.ButtonType?(buttonType);
    }
    else
    {
      foreach (Box.ButtonPressListener buttonPressListener in this.m_buttonPressListeners.ToArray())
        buttonPressListener.Fire(buttonType, isShowingTutorialPreview);
    }
  }

  public void AddButtonPressListener(Box.ButtonPressCallback callback) => this.AddButtonPressListener(callback, (object) null);

  public void AddButtonPressListener(Box.ButtonPressCallback callback, object userData)
  {
    Box.ButtonPressListener buttonPressListener = new Box.ButtonPressListener();
    buttonPressListener.SetCallback(callback);
    buttonPressListener.SetUserData(userData);
    if (this.m_buttonPressListeners.Contains(buttonPressListener))
      return;
    this.m_buttonPressListeners.Add(buttonPressListener);
  }

  public bool RemoveButtonPressListener(Box.ButtonPressCallback callback) => this.RemoveButtonPressListener(callback, (object) null);

  public bool RemoveButtonPressListener(Box.ButtonPressCallback callback, object userData)
  {
    Box.ButtonPressListener buttonPressListener = new Box.ButtonPressListener();
    buttonPressListener.SetCallback(callback);
    buttonPressListener.SetUserData(userData);
    return this.m_buttonPressListeners.Remove(buttonPressListener);
  }

  public void InitializeNet(bool fromLogin)
  {
    SceneMgr service;
    if (!ServiceManager.TryGet<SceneMgr>(out service))
      return;
    this.m_waitingForNetData = true;
    if (service.GetMode() == SceneMgr.Mode.STARTUP && !fromLogin)
      return;
    Network.Get().RequestBaconRatingInfo();
    NetCache.Get().RegisterScreenBox(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheBoosters), new Action(this.OnNetCacheBoostersUpdated));
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheMedalInfo), new Action(RankMgr.Get().SetRankPresenceField));
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheBaconRatingInfo), new Action(RankMgr.Get().SetRankPresenceField));
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.UpdateRankPresence));
    SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.UpdateRankPresence));
  }

  private void ShutdownNet()
  {
    NetCache service;
    if (!ServiceManager.TryGet<NetCache>(out service))
      return;
    service.UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    service.RemoveUpdatedListener(typeof (NetCache.NetCacheBoosters), new Action(this.OnNetCacheBoostersUpdated));
  }

  private void OnNetCacheReady()
  {
    this.m_waitingForNetData = false;
    if (!GameModeUtils.ShouldSeeSoloAdventuresMovedPopup() && (!GameUtils.IsTraditionalTutorialComplete() || !GameModeUtils.CanAccessGameModes()))
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.FTUE, GameSaveKeySubkeyId.FTUE_SHOULD_SEE_SOLO_ADVENTURES_MOVED_POPUP, new long[1]
      {
        1L
      }));
    this.StartCoroutine(this.UpdateUIWhenCollectionReady());
  }

  private void UpdateRankPresence(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.BACON && mode != SceneMgr.Mode.TOURNAMENT && mode != SceneMgr.Mode.GAMEPLAY)
      return;
    RankMgr.Get().SetRankPresenceField();
  }

  private void OnNetCacheBoostersUpdated() => this.UpdateUI();

  private int ComputeBoosterCount() => NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>().GetTotalNumBoosters();

  public void OnStoreShown()
  {
    MessagePopupDisplay service;
    if (ServiceManager.TryGet<MessagePopupDisplay>(out service))
      service.TriggerEvent(PopupEvent.OnShop);
    StoreManager.Get().RemoveStoreShownListener(new Action(this.OnStoreShown));
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    if (eventData.m_state != FindGameState.SERVER_GAME_STARTED || GameMgr.Get().IsNextReconnect() || (UnityEngine.Object) Box.Get() == (UnityEngine.Object) null)
      return false;
    Spell eventSpell = this.GetEventSpell(BoxEventType.TUTORIAL_PLAY);
    eventSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnTutorialPlaySpellStateBirthFinished));
    eventSpell.ActivateState(SpellStateType.BIRTH);
    return true;
  }

  private void OnTutorialPlaySpellStateBirthFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    SpellStateType activeState = spell.GetActiveState();
    if (prevStateType == SpellStateType.BIRTH)
    {
      LoadingScreen.Get().SetFadeColor(Color.white);
      LoadingScreen.Get().EnableFadeOut(false);
      LoadingScreen.Get().AddTransitionObject(Box.Get().gameObject);
      LoadingScreen.Get().AddTransitionBlocker();
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnMissionSceneLoaded));
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAMEPLAY);
    }
    else
    {
      if (activeState != SpellStateType.NONE)
        return;
      LoadingScreen.Get().NotifyTransitionBlockerComplete();
    }
  }

  private void OnMissionSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnMissionSceneLoaded));
    this.GetEventSpell(BoxEventType.TUTORIAL_PLAY).ActivateState(SpellStateType.ACTION);
  }

  private void ShowTutorialProgressScreen() => AssetLoader.Get().InstantiatePrefab((AssetReference) "TutorialProgressScreen.prefab:a78bac9caa971494ea8fac23dc1a9bd8", new PrefabCallback<GameObject>(this.OnTutorialProgressScreenCallback), options: AssetLoadingOptions.IgnorePrefabPosition);

  private void OnTutorialProgressScreenCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    TutorialProgressScreen component = go.GetComponent<TutorialProgressScreen>();
    component.SetCoinPressCallback(new HeroCoin.CoinPressCallback(this.StartTraditionalTutorial));
    component.StartTutorialProgress();
  }

  private PegUIElement GetRibbonButtonFromButton(PegUIElement button)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) null || (UnityEngine.Object) this.m_ribbonButtons == (UnityEngine.Object) null)
      return (PegUIElement) null;
    if ((UnityEngine.Object) button == (UnityEngine.Object) this.m_CollectionButton)
      return this.m_ribbonButtons.m_collectionManagerRibbon;
    if ((UnityEngine.Object) button == (UnityEngine.Object) this.m_QuestLogButton)
      return this.m_ribbonButtons.m_questLogRibbon;
    if ((UnityEngine.Object) button == (UnityEngine.Object) this.m_OpenPacksButton)
      return this.m_ribbonButtons.m_packOpeningRibbon;
    return (UnityEngine.Object) button == (UnityEngine.Object) this.m_StoreButton ? this.m_ribbonButtons.m_storeRibbon : (PegUIElement) null;
  }

  private void ShowReconnectPopup(UIEvent e, Box.ButtonPressFunction onButtonPressed) => DialogManager.Get().ShowReconnectHelperDialog((Action) (() =>
  {
    if (onButtonPressed == null)
      return;
    onButtonPressed(e);
  }));

  private void TrackBoxInteractable()
  {
    if (this.m_state != Box.State.PRESS_START && this.m_state != Box.State.HUB && this.m_state != Box.State.SET_ROTATION && this.m_state != Box.State.HUB_WITH_DRAWER)
      return;
    HearthstonePerformance.Get()?.CaptureBoxInteractableTime();
  }

  private void ResetSetRotationPopupProgress()
  {
    GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
    if (gameSaveDataManager == null)
      return;
    int num1 = gameSaveDataManager.IsDataReady(GameSaveKeyId.SET_ROTATION) ? 1 : 0;
    bool flag = false;
    List<GameSaveDataManager.SubkeySaveRequest> requests = new List<GameSaveDataManager.SubkeySaveRequest>();
    if (num1 == 0)
    {
      flag = true;
      requests.Add(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.ROTATED_BOOSTER_POPUP_PROGRESS, new long[1]));
      requests.Add(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.INNKEEPER_STANDARD_DECKS_VO_PROGRESS, new long[1]));
    }
    else
    {
      long num2 = -1;
      long num3 = -1;
      gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.ROTATED_BOOSTER_POPUP_PROGRESS, out num2);
      gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.INNKEEPER_STANDARD_DECKS_VO_PROGRESS, out num3);
      if (num2 != 0L)
      {
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.ROTATED_BOOSTER_POPUP_PROGRESS, new long[1]));
        flag = true;
      }
      if (num3 != 0L)
      {
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.INNKEEPER_STANDARD_DECKS_VO_PROGRESS, new long[1]));
        flag = true;
      }
    }
    if (!flag)
      return;
    gameSaveDataManager.SaveSubkeys(requests);
  }

  private void SetPackCount(int n)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_ribbonButtons.SetPackCount(n);
    else
      this.m_OpenPacksButton.SetPackCount(n);
  }

  private void ClearQueuedButtonFireEvent() => this.m_queuedButtonFire = new Box.ButtonType?();

  private void InitializeTutorialPreviewController()
  {
    if (GameUtils.AreAllTutorialsComplete() || (bool) (UnityEngine.Object) this.m_tutorialPreviewController || !(bool) (UnityEngine.Object) this.m_tutorialPreview)
      return;
    this.m_tutorialPreview.gameObject.SetActive(true);
    this.m_tutorialPreview.Initialize();
    this.m_tutorialPreview.RegisterReadyListener((Action<object>) (_ => this.m_tutorialPreviewController = this.m_tutorialPreview.GetComponentInChildren<TutorialPreviewController>()), (object) null, true);
  }

  private bool IsIndirectCollectionTransition()
  {
    int num = SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.COLLECTIONMANAGER || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.BACON_COLLECTION || SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER ? 1 : (SceneMgr.Get().GetMode() == SceneMgr.Mode.BACON_COLLECTION ? 1 : 0);
    bool flag = SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.HUB || SceneMgr.Get().GetMode() == SceneMgr.Mode.HUB;
    return num != 0 && !flag;
  }

  private void UpdateUIEvents()
  {
    int num1 = this.m_waitingForNetData ? 0 : (SetRotationManager.Get().ShouldShowSetRotationIntro() ? 1 : 0);
    bool flag = !this.m_waitingForNetData && GameUtils.IsAnyTutorialComplete();
    int num2 = num1 != 0 || !flag || DemoMgr.Get().IsDemo() || this.m_state == Box.State.LOADING_HUB ? 1 : (this.IsIndirectCollectionTransition() ? 1 : 0);
    if (this.CanEnableUIEvents() && this.m_state == Box.State.PRESS_START)
      this.EnableButton((PegUIElement) this.m_StartButton);
    else
      this.DisableButton((PegUIElement) this.m_StartButton);
    NetCache.NetCacheFeatures netCacheFeatures = (NetCache.NetCacheFeatures) null;
    if (!this.m_waitingForNetData)
      netCacheFeatures = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (num2 == 0)
    {
      this.m_StoreButton.gameObject.SetActive(true);
      if (netCacheFeatures != null && !netCacheFeatures.JournalButtonDisabled)
        this.m_journalButtonWidget.Show();
    }
    if (this.CanEnableUIEvents() && (this.m_state == Box.State.HUB || this.m_state == Box.State.HUB_WITH_DRAWER))
    {
      if (this.m_waitingForNetData)
      {
        this.DisableButton((PegUIElement) this.m_BattleGroundsButton);
        this.DisableButton((PegUIElement) this.m_GameModesButton);
        this.DisableButton((PegUIElement) this.m_MercenariesButton);
        this.DisableButton((PegUIElement) this.m_QuestLogButton);
        this.DisableButton((PegUIElement) this.m_StoreButton);
        this.m_journalButtonWidget.TriggerEvent("DISABLE_INTERACTION");
        this.ToggleButtonTextureState(false, this.m_PlayButton);
        this.DisableButton((PegUIElement) this.m_PlayButton);
      }
      else
      {
        this.EnableButton((PegUIElement) this.m_BattleGroundsButton);
        this.EnableButton((PegUIElement) this.m_GameModesButton);
        this.EnableButton((PegUIElement) this.m_MercenariesButton);
        this.EnableButton((PegUIElement) this.m_StoreButton);
        this.EnableButton((PegUIElement) this.m_QuestLogButton);
        this.m_journalButtonWidget.TriggerEvent("ENABLE_INTERACTION");
        if (this.IsCollectionReady())
        {
          this.ToggleButtonTextureState(true, this.m_PlayButton);
          this.EnableButton((PegUIElement) this.m_PlayButton);
        }
        else
        {
          this.ToggleButtonTextureState(false, this.m_PlayButton);
          this.DisableButton((PegUIElement) this.m_PlayButton);
        }
        this.ToggleButtonTextureState(GameUtils.IsTraditionalTutorialComplete(), this.m_GameModesButton);
        if (!this.m_BattleGroundsButton.IsFeatureActive())
          this.m_BattleGroundsButton.SetDisabledVisuals();
        if (!this.m_MercenariesButton.IsFeatureActive() || !GameModeUtils.HasSeenMercenariesButtonActivation())
          this.m_MercenariesButton.SetDisabledVisuals();
      }
      if (this.m_state == Box.State.HUB_WITH_DRAWER)
      {
        if (this.m_waitingForNetData)
        {
          this.DisableButton((PegUIElement) this.m_OpenPacksButton);
          this.DisableButton((PegUIElement) this.m_CollectionButton);
        }
        else
        {
          this.EnableButton((PegUIElement) this.m_OpenPacksButton);
          this.EnableButton((PegUIElement) this.m_CollectionButton);
        }
      }
      else
      {
        this.DisableButton((PegUIElement) this.m_OpenPacksButton);
        this.DisableButton((PegUIElement) this.m_CollectionButton);
      }
    }
    else
    {
      this.DisableButton((PegUIElement) this.m_PlayButton);
      this.DisableButton((PegUIElement) this.m_BattleGroundsButton);
      this.DisableButton((PegUIElement) this.m_GameModesButton);
      this.DisableButton((PegUIElement) this.m_MercenariesButton);
      this.DisableButton((PegUIElement) this.m_OpenPacksButton);
      this.DisableButton((PegUIElement) this.m_CollectionButton);
      this.DisableButton((PegUIElement) this.m_QuestLogButton);
      this.DisableButton((PegUIElement) this.m_StoreButton);
      this.m_journalButtonWidget.TriggerEvent("DISABLE_INTERACTION");
    }
    if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2019_BATTLEGROUNDS)
    {
      this.DisableButton((PegUIElement) this.m_PlayButton);
      this.DisableButton((PegUIElement) this.m_BattleGroundsButton);
      this.DisableButton((PegUIElement) this.m_OpenPacksButton);
      this.DisableButton((PegUIElement) this.m_CollectionButton);
      this.DisableButton((PegUIElement) this.m_QuestLogButton);
      this.DisableButton((PegUIElement) this.m_StoreButton);
      this.DisableButton((PegUIElement) this.m_MercenariesButton);
      this.m_journalButtonWidget.TriggerEvent("DISABLE_INTERACTION");
    }
    if (num2 != 0)
    {
      this.m_StoreButton.gameObject.SetActive(false);
      this.m_QuestLogButton.gameObject.SetActive(false);
      this.m_journalButtonWidget.Hide();
    }
    if (netCacheFeatures == null || !netCacheFeatures.JournalButtonDisabled)
      return;
    this.m_journalButtonWidget.Hide();
    this.m_journalButtonWidget.TriggerEvent("DISABLE_INTERACTION");
  }

  public void Unload()
  {
    GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.m_tutorialPreview.Unload();
    this.m_tutorialPreview.gameObject.SetActive(false);
    this.m_tutorialPreviewController = (TutorialPreviewController) null;
  }

  public Vector3 GetModesButtonPosition() => this.m_GameModesButton.transform.position;

  private void OnDestroyButton()
  {
    StoreManager.Get()?.RemoveStoreShownListener(new Action(this.OnStoreShown));
    this.ShutdownNet();
  }

  public enum State
  {
    INVALID,
    STARTUP,
    PRESS_START,
    LOADING,
    LOADING_HUB,
    HUB,
    HUB_WITH_DRAWER,
    OPEN,
    CLOSED,
    ERROR,
    SET_ROTATION_LOADING,
    SET_ROTATION,
    SET_ROTATION_OPEN,
  }

  public delegate void TransitionFinishedCallback(object userData);

  private class TransitionFinishedListener : EventListener<Box.TransitionFinishedCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }

  private class BoxStateConfig
  {
    public Box.BoxStateConfig.Part<BoxLogo.State> m_logoState = new Box.BoxStateConfig.Part<BoxLogo.State>();
    public Box.BoxStateConfig.Part<BoxStartButton.State> m_startButtonState = new Box.BoxStateConfig.Part<BoxStartButton.State>();
    public Box.BoxStateConfig.Part<StoreButton.State> m_storeButtonState = new Box.BoxStateConfig.Part<StoreButton.State>();
    public Box.BoxStateConfig.Part<BoxDoor.State> m_doorState = new Box.BoxStateConfig.Part<BoxDoor.State>();
    public Box.BoxStateConfig.Part<BoxDisk.State> m_diskState = new Box.BoxStateConfig.Part<BoxDisk.State>();
    public Box.BoxStateConfig.Part<BoxDrawer.State> m_drawerState = new Box.BoxStateConfig.Part<BoxDrawer.State>();
    public Box.BoxStateConfig.Part<BoxCamera.State> m_camState = new Box.BoxStateConfig.Part<BoxCamera.State>();
    public Box.BoxStateConfig.Part<EventBoxDressing.State> m_boxDressingState = new Box.BoxStateConfig.Part<EventBoxDressing.State>();

    public class Part<T>
    {
      public bool m_ignore;
      public T m_state;
    }
  }

  public enum ButtonType
  {
    START,
    TRADITIONAL,
    OPEN_PACKS,
    COLLECTION,
    SET_ROTATION,
    QUEST_LOG,
    STORE,
    GAME_MODES,
    BACON,
    PVP_DUNGEON_RUN,
    MERCENARIES,
  }

  public delegate void ButtonPressFunction(UIEvent e);

  public delegate void ButtonPressCallback(
    Box.ButtonType buttonType,
    bool isShowingTutorialPreview,
    object userData);

  public class ButtonPressListener : EventListener<Box.ButtonPressCallback>
  {
    public void Fire(Box.ButtonType buttonType, bool isShowingTutorialPreview) => this.m_callback(buttonType, isShowingTutorialPreview, this.m_userData);
  }
}
