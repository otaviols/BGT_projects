using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UniversalInputManager : IHasUpdate, IService
{
  private static UniversalInputManager s_instance;
  private const int RAYCAST_MAXHITNUMBER = 20;
  private RaycastHit[] m_cachedRaycastHits = new RaycastHit[20];
  private static readonly PlatformDependentValue<bool> IsTouchDevice = new PlatformDependentValue<bool>(PlatformCategory.Input)
  {
    Mouse = false,
    Touch = true
  };
  private const int MAX_CAMERAS = 20;
  private const float TEXT_INPUT_RECT_HEIGHT_OFFSET = 3f;
  private const int TEXT_INPUT_MAX_FONT_SIZE = 96;
  private const int TEXT_INPUT_MIN_FONT_SIZE = 4;
  private const int TEXT_INPUT_FONT_SIZE_INSET = 4;
  private const int TEXT_INPUT_IME_FONT_SIZE_INSET = 4;
  private const string TEXT_INPUT_NAME = "UniversalInputManagerTextInput";
  private static readonly GameLayer[] s_hitTestPriorityOrder = new GameLayer[11]
  {
    GameLayer.IgnoreFullScreenEffects,
    GameLayer.Reserved29,
    GameLayer.BackgroundUI,
    GameLayer.PerspectiveUI,
    GameLayer.CameraMask,
    GameLayer.UI,
    GameLayer.BattleNet,
    GameLayer.BattleNetFriendList,
    GameLayer.BattleNetDialog,
    GameLayer.BattleNetChat,
    GameLayer.HighPriorityUI
  };
  private static readonly GameLayer[] s_ignoreHitTestLayers = new GameLayer[10]
  {
    GameLayer.TransparentFX,
    GameLayer.IgnoreRaycast,
    GameLayer.Water,
    GameLayer.Tooltip,
    GameLayer.NoLight,
    GameLayer.Effects,
    GameLayer.FXCollide,
    GameLayer.ScreenEffects,
    GameLayer.InvisibleRender,
    GameLayer.CameraFade
  };
  private static readonly LayerMask s_cameraMaskLayer = (LayerMask) GameLayer.CameraMask.LayerBit();
  private static readonly LayerMask s_friendsListMaskLayer = (LayerMask) GameLayer.BattleNetFriendList.LayerBit();
  private static readonly LayerMask s_bnetChatMaskLayer = (LayerMask) GameLayer.BattleNetChat.LayerBit();
  private static readonly LayerMask s_ignoreFullScreenEffectsLayer = (LayerMask) GameLayer.IgnoreFullScreenEffects.LayerBit();
  private static Map<int, int> s_hitTestPriorityMap;
  private static int s_hitTestLayerBits = 0;
  private static bool IsIMEEverUsed = false;
  private bool m_mouseOnScreen;
  private List<UniversalInputManager.MouseOnOrOffScreenCallback> m_mouseOnOrOffScreenListeners = new List<UniversalInputManager.MouseOnOrOffScreenCallback>();
  private bool m_gameDialogActive;
  private bool m_systemDialogActive;
  private Camera m_mainEffectsCamera;
  private FullScreenEffects m_currentFullScreenEffect;
  private List<Camera> m_cameraMaskCameras = new List<Camera>();
  private Camera[] m_allCameras = new Camera[20];
  private int m_numCameras = 20;
  private Vector3 m_mousePosition;
  private List<Camera> m_ignoredCameras = new List<Camera>();
  private GameObject m_inputOwner;
  private UniversalInputManager.TextInputUpdatedCallback m_inputUpdatedCallback;
  private UniversalInputManager.TextInputPreprocessCallback m_inputPreprocessCallback;
  private UniversalInputManager.TextInputCompletedCallback m_inputCompletedCallback;
  private UniversalInputManager.TextInputCanceledCallback m_inputCanceledCallback;
  private UniversalInputManager.TextInputUnfocusedCallback m_inputUnfocusedCallback;
  private bool m_inputPassword;
  private bool m_inputNumber;
  private bool m_inputMultiLine;
  private bool m_inputActive;
  private bool m_inputFocused;
  private bool m_inputKeepFocusOnComplete;
  private string m_inputText;
  private Rect m_inputNormalizedRect;
  private Vector2 m_inputInitialScreenSize;
  private int m_inputMaxCharacters;
  private TextAnchor? m_inputAlignment;
  private Color? m_inputColor;
  private bool m_inputNeedsFocus;
  private bool m_tabKeyDown;
  private bool m_inputNeedsFocusFromTabKeyDown;
  private UniversalInputManager.TextInputIgnoreState m_inputIgnoreState;
  private GameObject m_sceneObject;
  private bool m_hideVirtualKeyboardOnComplete = true;
  private InputFieldUI m_inputFieldUI;
  private HearthstoneCheckout m_commerce;
  private bool m_shouldHandleCheats;
  private SceneMgr m_sceneMgr;
  private bool m_isTouchMode;
  private bool m_inputFieldReady;
  public static readonly PlatformDependentValue<bool> UsePhoneUI = new PlatformDependentValue<bool>(PlatformCategory.Screen)
  {
    Phone = true,
    Tablet = false,
    PC = false
  };

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    UniversalInputManager universalInputManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    UniversalInputManager.CreateHitTestPriorityMap();
    universalInputManager.m_mouseOnScreen = InputUtil.IsMouseOnScreen();
    universalInputManager.m_shouldHandleCheats = !HearthstoneApplication.IsPublic();
    universalInputManager.UpdateIsTouchMode();
    Options.Get().RegisterChangedListener(Option.TOUCH_MODE, new Options.ChangedCallback(universalInputManager.OnTouchModeChangedCallback));
    GameObject gameObject = new GameObject("RaycastCache", new System.Type[2]
    {
      typeof (RaycastCache),
      typeof (HSDontDestroyOnLoad)
    });
    return false;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
    UniversalInputManager.s_instance = (UniversalInputManager) null;
    this.m_cachedRaycastHits = (RaycastHit[]) null;
    Options.Get().UnregisterChangedListener(Option.TOUCH_MODE, new Options.ChangedCallback(this.OnTouchModeChangedCallback));
  }

  public void Update()
  {
    this.UpdateAllCamerasArray();
    this.UpdateMouseOnOrOffScreen();
    this.UpdateInput();
    this.CleanDeadCameras();
    if (!this.m_inputFieldReady)
      return;
    this.IgnoreGUIInput();
    this.HandleGUIInputInactive();
    this.HandleGUIInputActive();
  }

  public static UniversalInputManager Get()
  {
    if (UniversalInputManager.s_instance == null)
      UniversalInputManager.s_instance = ServiceManager.Get<UniversalInputManager>();
    return UniversalInputManager.s_instance;
  }

  public bool IsTouchMode() => this.m_isTouchMode;

  private void UpdateIsTouchMode() => this.m_isTouchMode = (bool) UniversalInputManager.IsTouchDevice || Options.Get().GetBool(Option.TOUCH_MODE);

  public bool UseWindowsTouch() => this.IsTouchMode() && !PlatformSettings.IsEmulating;

  public bool WasTouchCanceled()
  {
    if (!(bool) UniversalInputManager.IsTouchDevice)
      return false;
    int index = 0;
    for (int touchCount = Input.touchCount; index < touchCount; ++index)
    {
      if (Input.GetTouch(index).phase == TouchPhase.Canceled)
        return true;
    }
    return false;
  }

  public bool RegisterMouseOnOrOffScreenListener(
    UniversalInputManager.MouseOnOrOffScreenCallback listener)
  {
    if (this.m_mouseOnOrOffScreenListeners.Contains(listener))
      return false;
    this.m_mouseOnOrOffScreenListeners.Add(listener);
    return true;
  }

  public bool UnregisterMouseOnOrOffScreenListener(
    UniversalInputManager.MouseOnOrOffScreenCallback listener)
  {
    return this.m_mouseOnOrOffScreenListeners.Remove(listener);
  }

  public void SetGameDialogActive(bool active) => this.m_gameDialogActive = active;

  public void SetSystemDialogActive(bool active) => this.m_systemDialogActive = active;

  public bool IsDialogActive() => this.m_gameDialogActive || this.m_systemDialogActive;

  public void UseTextInput(UniversalInputManager.TextInputParams parms, bool force = false)
  {
    if (!this.m_inputFieldReady || !force && (UnityEngine.Object) parms.m_owner == (UnityEngine.Object) this.m_inputOwner)
      return;
    if ((UnityEngine.Object) this.m_inputOwner != (UnityEngine.Object) null && (UnityEngine.Object) this.m_inputOwner != (UnityEngine.Object) parms.m_owner)
      this.ObjectCancelTextInput(parms.m_owner);
    this.m_inputOwner = parms.m_owner;
    this.m_inputUpdatedCallback = parms.m_updatedCallback;
    this.m_inputPreprocessCallback = parms.m_preprocessCallback;
    this.m_inputCompletedCallback = parms.m_completedCallback;
    this.m_inputCanceledCallback = parms.m_canceledCallback;
    this.m_inputUnfocusedCallback = parms.m_unfocusedCallback;
    this.m_inputFieldUI.SetTextInputParams(parms);
    this.m_inputPassword = parms.m_password;
    this.m_inputNumber = parms.m_number;
    this.m_inputMultiLine = parms.m_multiLine;
    this.m_inputActive = true;
    this.m_inputFocused = false;
    this.m_inputText = parms.m_text ?? string.Empty;
    this.m_inputNormalizedRect = parms.m_rect;
    this.m_inputInitialScreenSize.x = (float) Screen.width;
    this.m_inputInitialScreenSize.y = (float) Screen.height;
    this.m_inputMaxCharacters = parms.m_maxCharacters;
    this.m_inputColor = parms.m_color;
    this.m_inputAlignment = parms.m_alignment;
    this.m_inputNeedsFocus = true;
    this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.INVALID;
    this.m_inputKeepFocusOnComplete = parms.m_inputKeepFocusOnComplete;
    Input.imeCompositionMode = !this.IsTextInputPassword() ? IMECompositionMode.On : IMECompositionMode.Off;
    this.m_hideVirtualKeyboardOnComplete = parms.m_hideVirtualKeyboardOnComplete;
    if (!this.UseWindowsTouch() || !parms.m_showVirtualKeyboard)
      return;
    ServiceManager.Get<ITouchScreenService>().ShowKeyboard();
  }

  public void CancelTextInput(GameObject requester, bool force = false)
  {
    if (!this.IsTextInputActive() || !force && (UnityEngine.Object) requester != (UnityEngine.Object) this.m_inputOwner)
      return;
    this.ObjectCancelTextInput(requester);
  }

  public void FocusTextInput(GameObject owner)
  {
    if ((UnityEngine.Object) owner != (UnityEngine.Object) this.m_inputOwner)
      return;
    if (!this.m_tabKeyDown)
      this.m_inputNeedsFocus = true;
    else
      this.m_inputNeedsFocusFromTabKeyDown = true;
  }

  public bool IsTextInputPassword() => this.m_inputPassword;

  public bool IsTextInputActive() => this.m_inputActive;

  public string GetInputText() => this.m_inputText;

  public void SetInputText(string text, bool moveCursorToEnd = false)
  {
    this.m_inputText = text ?? string.Empty;
    this.m_inputFieldUI.Text = this.m_inputText;
    if (!moveCursorToEnd)
      return;
    this.m_inputFieldUI.MoveCursorToEnd();
  }

  public bool InputIsOver(GameObject gameObj) => this.InputIsOver(gameObj, out RaycastHit _);

  public bool InputIsOver(GameObject gameObj, out RaycastHit hitInfo) => this.Raycast((Camera) null, (LayerMask) ((GameLayer) gameObj.layer).LayerBit(), out Camera _, out hitInfo) && (UnityEngine.Object) hitInfo.collider.gameObject == (UnityEngine.Object) gameObj;

  public bool InputIsOver(Camera camera, GameObject gameObj) => this.InputIsOver(camera, gameObj, out RaycastHit _);

  public bool InputIsOver(Camera camera, GameObject gameObj, out RaycastHit hitInfo)
  {
    LayerMask mask = (LayerMask) ((GameLayer) gameObj.layer).LayerBit();
    return this.Raycast(camera, mask, out Camera _, out hitInfo) && (UnityEngine.Object) hitInfo.collider.gameObject == (UnityEngine.Object) gameObj;
  }

  public bool InputIsOverByRenderPass(GameObject gameObj, out RaycastHit hitInfo) => this.GetInputHitInfoByRenderPass(out hitInfo, out Camera _) && (UnityEngine.Object) hitInfo.collider.gameObject == (UnityEngine.Object) gameObj;

  public bool ForcedInputIsOver(Camera camera, GameObject gameObj) => this.ForcedInputIsOver(camera, gameObj, out RaycastHit _);

  public bool ForcedInputIsOver(
    Camera camera,
    GameObject gameObj,
    out RaycastHit hitInfo,
    CameraOverridePass cameraOverride = null)
  {
    LayerMask layerMask = (LayerMask) ((GameLayer) gameObj.layer).LayerBit();
    return CameraUtils.Raycast(camera, this.m_mousePosition, layerMask, out hitInfo, cameraOverride) && (UnityEngine.Object) hitInfo.collider.gameObject == (UnityEngine.Object) gameObj;
  }

  public bool ForcedUnblockableInputIsOver(
    Camera camera,
    GameObject gameObj,
    out RaycastHit hitInfo)
  {
    LayerMask layerMask = (LayerMask) ((GameLayer) gameObj.layer).LayerBit();
    hitInfo = new RaycastHit();
    int num = CameraUtils.RaycastAll(camera, this.m_mousePosition, layerMask, ref this.m_cachedRaycastHits);
    if (num == 0)
      return false;
    for (int index = 0; index < num; ++index)
    {
      if ((UnityEngine.Object) this.m_cachedRaycastHits[index].collider.gameObject == (UnityEngine.Object) gameObj)
      {
        hitInfo = this.m_cachedRaycastHits[index];
        return true;
      }
    }
    return false;
  }

  public bool InputHitAnyObject(GameLayer layer) => this.GetInputHitInfo(layer, out RaycastHit _);

  public bool InputHitAnyObject(Camera requestedCamera, GameLayer layer) => this.GetInputHitInfo(requestedCamera, layer, out RaycastHit _);

  public bool GetInputHitInfo(GameLayer[] gameLayers, out RaycastHit hitInfo)
  {
    bool ignorePriority = false;
    int index = 0;
    for (int length = gameLayers.Length; index < length; ++index)
    {
      int mask = gameLayers[index].LayerBit();
      if (this.Raycast(this.GuessBestHitTestCamera((LayerMask) mask), (LayerMask) mask, out Camera _, out hitInfo, ignorePriority))
        return true;
      ignorePriority = true;
    }
    hitInfo = new RaycastHit();
    return false;
  }

  public bool GetInputHitInfo(out RaycastHit hitInfo) => this.GetInputHitInfo(GameLayer.Default, out hitInfo);

  public bool GetInputHitInfo(GameLayer layer, out RaycastHit hitInfo) => this.GetInputHitInfo((LayerMask) layer.LayerBit(), out hitInfo);

  public bool GetInputHitInfo(LayerMask mask, out RaycastHit hitInfo) => this.GetInputHitInfo(this.GuessBestHitTestCamera(mask), mask, out hitInfo);

  public bool GetInputHitInfo(Camera requestedCamera, out RaycastHit hitInfo) => (UnityEngine.Object) requestedCamera == (UnityEngine.Object) null ? this.GetInputHitInfo(out hitInfo) : this.GetInputHitInfo(requestedCamera, (LayerMask) requestedCamera.cullingMask, out hitInfo);

  public bool GetInputHitInfo(Camera requestedCamera, GameLayer layer, out RaycastHit hitInfo) => this.Raycast(requestedCamera, (LayerMask) layer.LayerBit(), out Camera _, out hitInfo);

  public bool GetInputHitInfo(Camera requestedCamera, LayerMask mask, out RaycastHit hitInfo) => this.Raycast(requestedCamera, mask, out Camera _, out hitInfo);

  public int GetAllInputHitInfo(LayerMask mask, ref RaycastHit[] hitInfo) => this.GetAllInputHitInfo(this.GuessBestHitTestCamera(mask), mask, ref hitInfo);

  public int GetAllInputHitInfo(Camera requestedCamera, LayerMask mask, ref RaycastHit[] hitInfo) => CameraUtils.RaycastAll(requestedCamera, this.m_mousePosition, mask, ref hitInfo);

  public bool GetInputHitInfoByRenderPass(out RaycastHit hitInfo, out Camera hitCamera)
  {
    Camera camera1 = this.GuessBestHitTestCamera((LayerMask) GameLayer.UI.LayerBit());
    if ((UnityEngine.Object) camera1 != (UnityEngine.Object) null)
    {
      for (int index = UniversalInputManager.s_hitTestPriorityOrder.Length - 1; index >= 0; --index)
      {
        GameLayer gameLayer = UniversalInputManager.s_hitTestPriorityOrder[index];
        if (gameLayer != GameLayer.UI)
        {
          int mask = gameLayer.LayerBit();
          List<CustomViewPass> customViewPassList = (List<CustomViewPass>) null;
          if ((mask & (int) UniversalInputManager.s_friendsListMaskLayer) != 0)
            customViewPassList = CustomViewPass.GetQueue(CustomViewEntryPoint.BattleNetFriendList);
          else if ((mask & (int) UniversalInputManager.s_bnetChatMaskLayer) != 0)
            customViewPassList = CustomViewPass.GetQueue(CustomViewEntryPoint.BattleNetChat);
          CameraOverridePass cameraOverride = (CameraOverridePass) null;
          if (customViewPassList != null && customViewPassList.Count > 0)
            cameraOverride = customViewPassList[0] as CameraOverridePass;
          if (this.RaycastAgainstBlockingLayers(camera1, (LayerMask) mask, out hitInfo, cameraOverride))
          {
            hitCamera = camera1;
            return true;
          }
        }
        else
          break;
      }
      LayerMask layerMask = (LayerMask) (camera1.cullingMask & UniversalInputManager.s_hitTestLayerBits & ~(int) UniversalInputManager.s_cameraMaskLayer);
      for (int whenToRender = 3; whenToRender >= 2; --whenToRender)
      {
        List<CustomViewPass> queue = CustomViewPass.GetQueue((CustomViewEntryPoint) whenToRender);
        if (queue != null && this.RaycastByRenderPasses(camera1, layerMask, queue, out hitInfo))
        {
          hitCamera = camera1;
          return true;
        }
      }
      if (this.RaycastAgainstBlockingLayers(camera1, layerMask, out hitInfo))
      {
        hitCamera = camera1;
        return true;
      }
    }
    Camera camera2 = this.GuessBestHitTestCamera((LayerMask) GameLayer.Default.LayerBit());
    if ((UnityEngine.Object) camera2 != (UnityEngine.Object) null)
    {
      LayerMask layerMask = (LayerMask) (camera2.cullingMask & UniversalInputManager.s_hitTestLayerBits & ~(int) UniversalInputManager.s_cameraMaskLayer);
      for (int whenToRender = 1; whenToRender >= 0; --whenToRender)
      {
        List<CustomViewPass> queue = CustomViewPass.GetQueue((CustomViewEntryPoint) whenToRender);
        if (queue != null && this.RaycastByRenderPasses(camera2, layerMask, queue, out hitInfo))
        {
          hitCamera = camera2;
          return true;
        }
      }
      if (this.RaycastAgainstBlockingLayers(camera2, layerMask, out hitInfo))
      {
        hitCamera = camera2;
        return true;
      }
    }
    hitCamera = (Camera) null;
    hitInfo = new RaycastHit();
    return false;
  }

  public bool GetInputPointOnPlane(Vector3 origin, out Vector3 point) => this.GetInputPointOnPlane(GameLayer.Default, origin, out point);

  public bool GetInputPointOnPlane(GameLayer layer, Vector3 origin, out Vector3 point)
  {
    point = Vector3.zero;
    Camera camera;
    if (!this.Raycast((Camera) null, (LayerMask) layer.LayerBit(), out camera, out RaycastHit _))
      return false;
    Ray ray = camera.ScreenPointToRay(this.m_mousePosition);
    float enter;
    if (!new Plane(-camera.transform.forward, origin).Raycast(ray, out enter))
      return false;
    point = ray.GetPoint(enter);
    return true;
  }

  public Ray MousePositionToRay(Camera camera) => camera.ScreenPointToRay(this.m_mousePosition);

  public void SetCurrentFullScreenEffect(FullScreenEffects effect) => this.m_currentFullScreenEffect = effect;

  public bool AddIgnoredCamera(Camera camera)
  {
    if (this.m_ignoredCameras.Contains(camera))
      return false;
    this.m_ignoredCameras.Add(camera);
    return true;
  }

  private static void CreateHitTestPriorityMap()
  {
    UniversalInputManager.s_hitTestPriorityMap = new Map<int, int>();
    int num1 = 1;
    for (int index = 0; index < UniversalInputManager.s_hitTestPriorityOrder.Length; ++index)
    {
      GameLayer gameLayer = UniversalInputManager.s_hitTestPriorityOrder[index];
      UniversalInputManager.s_hitTestPriorityMap.Add(gameLayer.LayerBit(), num1++);
    }
    foreach (GameLayer gameLayer in Enum.GetValues(typeof (GameLayer)))
    {
      int key = gameLayer.LayerBit();
      UniversalInputManager.s_hitTestLayerBits |= key;
      if (!UniversalInputManager.s_hitTestPriorityMap.ContainsKey(key))
        UniversalInputManager.s_hitTestPriorityMap.Add(key, 0);
    }
    int num2 = 0;
    foreach (GameLayer ignoreHitTestLayer in UniversalInputManager.s_ignoreHitTestLayers)
      num2 |= ignoreHitTestLayer.LayerBit();
    UniversalInputManager.s_hitTestLayerBits &= ~num2;
  }

  private void UpdateAllCamerasArray()
  {
    int length = this.m_allCameras.Length;
    int allCamerasCount = Camera.allCamerasCount;
    int num = allCamerasCount;
    if (length < num)
      this.m_allCameras = new Camera[allCamerasCount];
    this.m_numCameras = Camera.GetAllCameras(this.m_allCameras);
  }

  private void CleanDeadCameras()
  {
    GeneralUtils.CleanDeadObjectsFromList<Camera>(this.m_cameraMaskCameras);
    GeneralUtils.CleanDeadObjectsFromList<Camera>(this.m_ignoredCameras);
  }

  public void SetTextInputField(InputFieldUI inputFieldObject)
  {
    this.m_inputFieldUI = inputFieldObject;
    this.m_inputFieldUI.SetCanvasActive(false);
    this.m_inputFieldUI.SetEndEditFunction(new UnityAction<string>(this.EndEdit));
    this.m_inputFieldReady = true;
  }

  private void EndEdit(string text) => this.m_inputText = text;

  private Camera GuessBestHitTestCamera(LayerMask mask)
  {
    Camera camera = (Camera) null;
    BaseUI baseUi = BaseUI.Get();
    if ((UnityEngine.Object) baseUi != (UnityEngine.Object) null)
      camera = baseUi.GetBnetCamera();
    if ((UnityEngine.Object) camera != (UnityEngine.Object) null && (camera.cullingMask & (int) mask) != 0)
      return camera;
    Camera mainCamera = CameraUtils.GetMainCamera();
    if ((UnityEngine.Object) mainCamera != (UnityEngine.Object) null && (mainCamera.cullingMask & (int) mask) != 0)
      return mainCamera;
    for (int index = 0; index < this.m_numCameras; ++index)
    {
      Camera allCamera = this.m_allCameras[index];
      if (!((UnityEngine.Object) allCamera == (UnityEngine.Object) null) && (allCamera.cullingMask & (int) mask) != 0 && !this.m_ignoredCameras.Contains(allCamera))
        return allCamera;
    }
    return (Camera) null;
  }

  private bool Raycast(
    Camera requestedCamera,
    LayerMask mask,
    out Camera camera,
    out RaycastHit hitInfo,
    bool ignorePriority = false)
  {
    hitInfo = new RaycastHit();
    if (!ignorePriority)
    {
      foreach (Camera cameraMaskCamera in this.m_cameraMaskCameras)
      {
        if (this.RaycastWithPriority(cameraMaskCamera, UniversalInputManager.s_cameraMaskLayer, out hitInfo))
        {
          camera = cameraMaskCamera;
          return true;
        }
      }
      if ((UnityEngine.Object) this.m_mainEffectsCamera == (UnityEngine.Object) null)
        this.m_mainEffectsCamera = CameraUtils.FindFullScreenEffectsCamera(false);
      if ((UnityEngine.Object) this.m_mainEffectsCamera != (UnityEngine.Object) null && this.RaycastWithPriority(this.m_mainEffectsCamera, UniversalInputManager.s_ignoreFullScreenEffectsLayer, out hitInfo))
      {
        camera = this.m_mainEffectsCamera;
        return true;
      }
    }
    camera = requestedCamera;
    if ((UnityEngine.Object) camera != (UnityEngine.Object) null)
      return this.RaycastWithPriority(camera, mask, out hitInfo);
    camera = Camera.main;
    return this.RaycastWithPriority(camera, mask, out hitInfo);
  }

  private bool RaycastWithPriority(
    Camera camera,
    LayerMask mask,
    out RaycastHit hitInfo,
    CameraOverridePass cameraOverride = null)
  {
    hitInfo = new RaycastHit();
    return !((UnityEngine.Object) camera == (UnityEngine.Object) null) && this.FilteredRaycast(camera, this.m_mousePosition, mask, out hitInfo, cameraOverride) && !this.HigherPriorityCollisionExists(((GameLayer) hitInfo.collider.gameObject.layer).LayerBit());
  }

  private bool RaycastByRenderPasses(
    Camera camera,
    LayerMask primaryLayerMask,
    List<CustomViewPass> renderPasses,
    out RaycastHit hitInfo)
  {
    if (renderPasses == null)
    {
      hitInfo = new RaycastHit();
      return false;
    }
    for (int index = renderPasses.Count - 1; index >= 0; --index)
    {
      if (renderPasses[index] is CameraOverridePass renderPass)
      {
        LayerMask mask = ((int) renderPass.layerMask & (int) UniversalInputManager.s_cameraMaskLayer) != 0 ? UniversalInputManager.s_cameraMaskLayer : primaryLayerMask;
        if (this.RaycastAgainstBlockingLayers(camera, mask, out hitInfo, renderPass))
          return true;
      }
    }
    hitInfo = new RaycastHit();
    return false;
  }

  private bool RaycastAgainstBlockingLayers(
    Camera camera,
    LayerMask mask,
    out RaycastHit hitInfo,
    CameraOverridePass cameraOverride = null)
  {
    hitInfo = new RaycastHit();
    if ((UnityEngine.Object) camera == (UnityEngine.Object) null || !this.FilteredRaycast(camera, this.m_mousePosition, mask, out hitInfo, cameraOverride))
      return false;
    int key = ((GameLayer) hitInfo.collider.gameObject.layer).LayerBit();
    return (!this.m_systemDialogActive || UniversalInputManager.s_hitTestPriorityMap[key] >= UniversalInputManager.s_hitTestPriorityMap[GameLayer.UI.LayerBit()]) && (!this.m_gameDialogActive || UniversalInputManager.s_hitTestPriorityMap[key] >= UniversalInputManager.s_hitTestPriorityMap[GameLayer.IgnoreFullScreenEffects.LayerBit()]) && (!((UnityEngine.Object) this.m_currentFullScreenEffect != (UnityEngine.Object) null) || !this.m_currentFullScreenEffect.HasActiveEffects || (double) camera.depth >= (double) this.m_currentFullScreenEffect.Camera.depth);
  }

  public void UpdateCachedValues() => this.m_mousePosition = InputCollection.GetMousePosition();

  private bool FilteredRaycast(
    Camera camera,
    Vector3 screenPoint,
    LayerMask mask,
    out RaycastHit hitInfo,
    CameraOverridePass cameraOverride = null)
  {
    return CameraUtils.Raycast(camera, screenPoint, mask, out hitInfo, cameraOverride);
  }

  private bool HigherPriorityCollisionExists(int layerBit)
  {
    if (this.m_systemDialogActive && UniversalInputManager.s_hitTestPriorityMap[layerBit] < UniversalInputManager.s_hitTestPriorityMap[GameLayer.UI.LayerBit()] || this.m_gameDialogActive && UniversalInputManager.s_hitTestPriorityMap[layerBit] < UniversalInputManager.s_hitTestPriorityMap[GameLayer.IgnoreFullScreenEffects.LayerBit()])
      return true;
    LayerMask priorityLayerMask = this.GetHigherPriorityLayerMask(layerBit);
    for (int index = 0; index < this.m_numCameras; ++index)
    {
      Camera allCamera = this.m_allCameras[index];
      RaycastHit hitInfo;
      if (!((UnityEngine.Object) allCamera == (UnityEngine.Object) null) && (allCamera.cullingMask & (int) priorityLayerMask) != 0 && !this.m_ignoredCameras.Contains(allCamera) && this.FilteredRaycast(allCamera, this.m_mousePosition, priorityLayerMask, out hitInfo))
      {
        GameLayer layer = (GameLayer) hitInfo.collider.gameObject.layer;
        if ((allCamera.cullingMask & layer.LayerBit()) != 0)
          return true;
      }
    }
    return false;
  }

  private LayerMask GetHigherPriorityLayerMask(int layerBit)
  {
    int hitTestPriority1 = UniversalInputManager.s_hitTestPriorityMap[layerBit];
    LayerMask priorityLayerMask = (LayerMask) 0;
    foreach (KeyValuePair<int, int> hitTestPriority2 in UniversalInputManager.s_hitTestPriorityMap)
    {
      if (hitTestPriority2.Value > hitTestPriority1)
        priorityLayerMask = (LayerMask) ((int) priorityLayerMask | hitTestPriority2.Key);
    }
    return priorityLayerMask;
  }

  private void UpdateMouseOnOrOffScreen()
  {
    bool onScreen = InputUtil.IsMouseOnScreen();
    if (onScreen == this.m_mouseOnScreen)
      return;
    this.m_mouseOnScreen = onScreen;
    foreach (UniversalInputManager.MouseOnOrOffScreenCallback offScreenCallback in this.m_mouseOnOrOffScreenListeners.ToArray())
      offScreenCallback(onScreen);
  }

  private void UpdateInput()
  {
    if (this.UpdateTextInput())
      return;
    InputManager inputManager = InputManager.Get();
    if ((UnityEngine.Object) inputManager != (UnityEngine.Object) null && inputManager.HandleKeyboardInput() || this.HearthstoneCheckoutBlocksInput())
      return;
    if (this.m_shouldHandleCheats)
    {
      CheatMgr cheatMgr = CheatMgr.Get();
      if (cheatMgr != null && cheatMgr.HandleKeyboardInput())
        return;
      Cheats cheats = Cheats.Get();
      if (cheats != null && cheats.HandleKeyboardInput())
        return;
    }
    DialogManager dialogManager = DialogManager.Get();
    if ((UnityEngine.Object) dialogManager != (UnityEngine.Object) null && dialogManager.HandleKeyboardInput())
      return;
    InputMgr inputMgr = InputMgr.Get();
    if ((UnityEngine.Object) inputMgr != (UnityEngine.Object) null && inputMgr.HandleKeyboardInput())
      return;
    DraftInputManager draftInputManager = DraftInputManager.Get();
    if ((UnityEngine.Object) draftInputManager != (UnityEngine.Object) null && draftInputManager.HandleKeyboardInput())
      return;
    PackOpening packOpening = PackOpening.Get();
    if ((UnityEngine.Object) packOpening != (UnityEngine.Object) null && packOpening.HandleKeyboardInput())
      return;
    if (this.m_sceneMgr != null || ServiceManager.TryGet<SceneMgr>(out this.m_sceneMgr))
    {
      PegasusScene scene = this.m_sceneMgr.GetScene();
      if ((UnityEngine.Object) scene != (UnityEngine.Object) null && scene.HandleKeyboardInput())
        return;
    }
    BaseUI baseUi = BaseUI.Get();
    if (!((UnityEngine.Object) baseUi != (UnityEngine.Object) null))
      return;
    baseUi.HandleKeyboardInput();
  }

  private bool UpdateTextInput()
  {
    if (Input.imeIsSelected || !string.IsNullOrEmpty(Input.compositionString))
      UniversalInputManager.IsIMEEverUsed = true;
    if (this.m_inputNeedsFocusFromTabKeyDown)
    {
      this.m_inputNeedsFocusFromTabKeyDown = false;
      this.m_inputNeedsFocus = true;
    }
    return this.m_inputActive && this.m_inputFocused;
  }

  private void UserCancelTextInput() => this.CancelTextInput(true, (GameObject) null);

  private void ObjectCancelTextInput(GameObject requester) => this.CancelTextInput(false, requester);

  private void CancelTextInput(bool userRequested, GameObject requester)
  {
    if (this.IsTextInputPassword())
      Input.imeCompositionMode = IMECompositionMode.Auto;
    if ((UnityEngine.Object) requester != (UnityEngine.Object) null && (UnityEngine.Object) requester == (UnityEngine.Object) this.m_inputOwner)
    {
      this.ClearTextInputVars();
    }
    else
    {
      UniversalInputManager.TextInputCanceledCallback canceledCallback = this.m_inputCanceledCallback;
      this.ClearTextInputVars();
      if (canceledCallback != null)
        canceledCallback(userRequested, requester);
    }
    if (!this.UseWindowsTouch())
      return;
    ServiceManager.Get<ITouchScreenService>().HideKeyboard();
  }

  private void ResetKeyboard()
  {
    if (!this.UseWindowsTouch() || !this.m_hideVirtualKeyboardOnComplete)
      return;
    ServiceManager.Get<ITouchScreenService>().HideKeyboard();
  }

  private void CompleteTextInput()
  {
    if (this.IsTextInputPassword())
      Input.imeCompositionMode = IMECompositionMode.Auto;
    UniversalInputManager.TextInputCompletedCallback completedCallback = this.m_inputCompletedCallback;
    if (!this.m_inputKeepFocusOnComplete)
      this.ClearTextInputVars();
    try
    {
      if (completedCallback != null)
        completedCallback(this.m_inputText);
      this.m_inputText = string.Empty;
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ex);
      this.ResetKeyboard();
      throw new Exception("Error completing text input", ex);
    }
    this.ResetKeyboard();
  }

  private void ClearTextInputVars()
  {
    this.m_inputActive = false;
    this.m_inputFocused = false;
    this.m_inputOwner = (GameObject) null;
    this.m_inputMaxCharacters = 0;
    this.m_inputUpdatedCallback = (UniversalInputManager.TextInputUpdatedCallback) null;
    this.m_inputCompletedCallback = (UniversalInputManager.TextInputCompletedCallback) null;
    this.m_inputCanceledCallback = (UniversalInputManager.TextInputCanceledCallback) null;
    this.m_inputUnfocusedCallback = (UniversalInputManager.TextInputUnfocusedCallback) null;
    int num = Application.isEditor ? 1 : 0;
  }

  private bool IgnoreGUIInput()
  {
    if (this.m_inputIgnoreState == UniversalInputManager.TextInputIgnoreState.INVALID)
      return false;
    if (Input.GetKeyDown(KeyCode.Return))
    {
      if (this.m_inputIgnoreState == UniversalInputManager.TextInputIgnoreState.COMPLETE_KEY_UP)
        this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.NEXT_CALL;
      return true;
    }
    if (!Input.GetKeyDown(KeyCode.Escape))
      return false;
    if (this.m_inputIgnoreState == UniversalInputManager.TextInputIgnoreState.CANCEL_KEY_UP)
      this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.NEXT_CALL;
    return true;
  }

  private void HandleGUIInputInactive()
  {
    if (this.m_inputActive)
      return;
    this.m_inputFieldUI.SetCanvasActive(false);
    if (this.m_inputIgnoreState != UniversalInputManager.TextInputIgnoreState.INVALID)
    {
      if (this.m_inputIgnoreState != UniversalInputManager.TextInputIgnoreState.NEXT_CALL)
        return;
      this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.INVALID;
    }
    else
    {
      if (this.HearthstoneCheckoutBlocksInput())
        return;
      ChatMgr.Get()?.HandleGUIInput();
    }
  }

  private void HandleGUIInputActive()
  {
    if (!this.m_inputActive || !this.PreprocessGUITextInput())
      return;
    Rect textInputRect = this.ComputeTextInputRect(new Vector2((float) Screen.width, (float) Screen.height));
    string str = this.ShowTextInput(textInputRect);
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    if (this.UseWindowsTouch() && !touchScreenService.IsVirtualKeyboardVisible() && InputCollection.GetMouseButtonDown(0) && textInputRect.Contains(touchScreenService.GetTouchPositionForGUI()))
      touchScreenService.ShowKeyboard();
    this.UpdateTextInputFocus();
    if (!this.m_inputFocused || !(this.m_inputText != str))
      return;
    if (this.m_inputNumber)
      str = StringUtils.StripNonNumbers(str);
    if (!this.m_inputMultiLine)
      str = StringUtils.StripNewlines(str);
    this.m_inputText = str;
    this.m_inputFieldUI.Text = str;
    if (this.m_inputUpdatedCallback == null)
      return;
    this.m_inputUpdatedCallback(str);
  }

  private bool PreprocessGUITextInput()
  {
    this.UpdateTabKeyDown();
    if (this.m_inputPreprocessCallback != null)
    {
      int num = this.m_inputPreprocessCallback() ? 1 : 0;
      if (!this.m_inputActive)
        return false;
    }
    return !this.ProcessTextInputFinishKeys();
  }

  private void UpdateTabKeyDown() => this.m_tabKeyDown = Input.GetKeyDown(KeyCode.Tab);

  private bool ProcessTextInputFinishKeys()
  {
    if (!this.m_inputFocused)
      return false;
    if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
    {
      this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.COMPLETE_KEY_UP;
      this.CompleteTextInput();
      return true;
    }
    if (!Input.GetKeyDown(KeyCode.Escape))
      return false;
    this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.CANCEL_KEY_UP;
    this.UserCancelTextInput();
    return true;
  }

  private string ShowTextInput(Rect inputScreenRect)
  {
    Rect inputFieldRect = OverlayUI.Get().GetInputFieldRect(this.m_inputNormalizedRect);
    inputFieldRect.y -= 1.5f;
    inputFieldRect.height += 1.5f;
    this.m_inputFieldUI.SetupTextProperties(this.ComputeTextInputFontSize(inputScreenRect.height), this.m_inputColor, this.m_inputAlignment);
    this.m_inputFieldUI.SetCanvasActive(true);
    this.m_inputFieldUI.SetInputRect(inputFieldRect);
    return this.m_inputFieldUI.Text;
  }

  private void UpdateTextInputFocus()
  {
    if (this.m_inputNeedsFocus)
    {
      this.m_inputFieldUI.ActivateInputField();
      this.m_inputFocused = this.m_inputFieldUI.IsFocused;
      this.m_inputNeedsFocus = !this.m_inputFocused;
    }
    else
    {
      bool inputFocused = this.m_inputFocused;
      this.m_inputFocused = this.m_inputFieldUI.IsFocused;
      UniversalInputManager.TextInputUnfocusedCallback unfocusedCallback = this.m_inputUnfocusedCallback;
      if (!(!this.m_inputFocused & inputFocused) || unfocusedCallback == null)
        return;
      unfocusedCallback();
    }
  }

  private Rect ComputeTextInputRect(Vector2 screenSize)
  {
    float num1 = this.m_inputInitialScreenSize.x / this.m_inputInitialScreenSize.y / (screenSize.x / screenSize.y);
    float num2 = (0.5f - this.m_inputNormalizedRect.x) * this.m_inputInitialScreenSize.x * (screenSize.y / this.m_inputInitialScreenSize.y);
    return new Rect(screenSize.x * 0.5f - num2, (float) ((double) this.m_inputNormalizedRect.y * (double) screenSize.y - 1.5), this.m_inputNormalizedRect.width * screenSize.x * num1, (float) ((double) this.m_inputNormalizedRect.height * (double) screenSize.y + 1.5));
  }

  private int ComputeTextInputFontSize(float rectHeight)
  {
    int num = Mathf.CeilToInt(rectHeight);
    return Mathf.Clamp(Localization.IsIMELocale() || UniversalInputManager.IsIMEEverUsed ? num - 4 : num - 4, 4, 96);
  }

  private bool HearthstoneCheckoutBlocksInput() => (this.m_commerce != null || ServiceManager.TryGet<HearthstoneCheckout>(out this.m_commerce)) && this.m_commerce.ShouldBlockInput;

  private void OnTouchModeChangedCallback(
    Option option,
    object prevvalue,
    bool existed,
    object userdata)
  {
    this.UpdateIsTouchMode();
  }

  public delegate void MouseOnOrOffScreenCallback(bool onScreen);

  public delegate void TextInputUpdatedCallback(string input);

  public delegate bool TextInputPreprocessCallback();

  public delegate void TextInputCompletedCallback(string input);

  public delegate void TextInputCanceledCallback(bool userRequested, GameObject requester);

  public delegate void TextInputUnfocusedCallback();

  public class TextInputParams
  {
    public GameObject m_owner;
    public bool m_password;
    public bool m_number;
    public bool m_multiLine;
    public Rect m_rect;
    public UniversalInputManager.TextInputUpdatedCallback m_updatedCallback;
    public UniversalInputManager.TextInputPreprocessCallback m_preprocessCallback;
    public UniversalInputManager.TextInputCompletedCallback m_completedCallback;
    public UniversalInputManager.TextInputCanceledCallback m_canceledCallback;
    public UniversalInputManager.TextInputUnfocusedCallback m_unfocusedCallback;
    public int m_maxCharacters;
    public Font m_font;
    public TextAnchor? m_alignment;
    public string m_text;
    public bool m_touchScreenKeyboardHideInput;
    public int m_touchScreenKeyboardType;
    public bool m_inputKeepFocusOnComplete;
    public Color? m_color;
    public bool m_showVirtualKeyboard = true;
    public bool m_hideVirtualKeyboardOnComplete = true;
    public bool m_useNativeKeyboard;
    public bool m_showBackground;
  }

  private enum TextInputIgnoreState
  {
    INVALID,
    COMPLETE_KEY_UP,
    CANCEL_KEY_UP,
    NEXT_CALL,
  }
}
