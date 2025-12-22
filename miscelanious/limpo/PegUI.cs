using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PegUI : MonoBehaviour
{
  public const float DEFAULT_SCREEN_DPI = 96f;
  public Camera orthographicUICam;
  private static readonly GameLayer[] HIT_TEST_PRIORITY = new GameLayer[11]
  {
    GameLayer.IgnoreFullScreenEffects,
    GameLayer.BackgroundUI,
    GameLayer.PerspectiveUI,
    GameLayer.CameraMask,
    GameLayer.UI,
    GameLayer.BattleNet,
    GameLayer.BattleNetFriendList,
    GameLayer.BattleNetChat,
    GameLayer.BattleNetDialog,
    GameLayer.HighPriorityUI,
    GameLayer.Reserved29
  };
  private List<Camera> m_UICams = new List<Camera>();
  private PegUIElement m_prevElement;
  private PegUIElement m_currentElement;
  private PegUIElement m_mouseDownElement;
  private static PegUI s_instance;
  private float m_mouseDownTimer;
  private float m_lastClickTimer;
  private Vector3 m_lastClickPosition;
  private const float PRESS_VS_TAP_TOLERANCE = 0.4f;
  private const float HOLD_TOLERANCE = 0.45f;
  private const float DOUBLECLICK_TOLERANCE = 0.7f;
  private const float DOUBLECLICK_COUNT_DISABLED = -1f;
  private const float MOUSEDOWN_COUNT_DISABLED = -1f;
  private List<PegUICustomBehavior> m_customBehaviors = new List<PegUICustomBehavior>();
  private List<Component> m_newHitDetectionComponents = new List<Component>();
  private PegUI.DelSwipeListener m_swipeListener;
  private bool m_hasFocus = true;
  private bool m_uguiActive;
  private float m_dragToleranceDpiAdjustmentFactor = 1f;
  private Camera m_cameraPriorityHitCamera;

  public static event Action<PegUIElement> OnReleasePreTrigger;

  private void Awake()
  {
    PegUI.s_instance = this;
    this.m_lastClickPosition = Vector3.zero;
    this.gameObject.AddComponent<HSDontDestroyOnLoad>();
    if ((double) Screen.dpi <= 0.0)
      return;
    this.m_dragToleranceDpiAdjustmentFactor = Screen.dpi / 96f;
  }

  private void OnDestroy()
  {
    if (UniversalInputManager.Get() != null)
      UniversalInputManager.Get().UnregisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.RemoveFocusChangedListener(new HearthstoneApplication.FocusChangedCallback(this.OnAppFocusChanged));
    PegUI.s_instance = (PegUI) null;
  }

  private void Start()
  {
    Processor.QueueJob(HearthstoneJobs.CreateJobFromAction("PegUI.RegisterMouseOnOrOffScreenListener", new Action(this.RegisterMouseListener), new IJobDependency[1]
    {
      ServiceManager.CreateServiceDependency(typeof (UniversalInputManager))
    }));
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if (!((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null))
      return;
    hearthstoneApplication.AddFocusChangedListener(new HearthstoneApplication.FocusChangedCallback(this.OnAppFocusChanged));
  }

  private void Update() => this.MouseInputUpdate();

  public static PegUI Get() => PegUI.s_instance;

  public static bool IsInitialized() => (UnityEngine.Object) PegUI.s_instance != (UnityEngine.Object) null;

  public PegUIElement GetMousedOverElement() => this.m_currentElement;

  public PegUIElement GetMouseDownElement() => this.m_mouseDownElement;

  public PegUIElement GetPrevMousedOverElement() => this.m_prevElement;

  public void AddInputCamera(Camera cam)
  {
    if ((UnityEngine.Object) cam == (UnityEngine.Object) null)
      Debug.Log((object) "Trying to add a null camera!");
    else
      this.m_UICams.Add(cam);
  }

  public void RemoveInputCamera(Camera cam)
  {
    if (!((UnityEngine.Object) cam != (UnityEngine.Object) null))
      return;
    this.m_UICams.Remove(cam);
  }

  public PegUIElement FindHitElement() => this.FindHitElement(out RaycastHit _);

  public PegUIElement FindHitElement(out RaycastHit hit)
  {
    UniversalInputManager universalInputManager = UniversalInputManager.Get();
    if (universalInputManager.IsTouchMode() && !InputCollection.GetMouseButton(0) && !InputCollection.GetMouseButtonUp(0))
    {
      hit = new RaycastHit();
      return (PegUIElement) null;
    }
    SceneDebugger service = (SceneDebugger) null;
    if (ServiceManager.TryGet<SceneDebugger>(out service) && service.IsMouseOverGui())
    {
      hit = new RaycastHit();
      return (PegUIElement) null;
    }
    if (this.m_newHitDetectionComponents.Count > 0 && universalInputManager.GetInputHitInfoByRenderPass(out hit, out this.m_cameraPriorityHitCamera) || universalInputManager.GetInputHitInfo(PegUI.HIT_TEST_PRIORITY, out hit))
      return this.TryGetPegUIElementFromHit(hit);
    for (int index = this.m_UICams.Count - 1; index >= 0; --index)
    {
      Camera uiCam = this.m_UICams[index];
      if ((UnityEngine.Object) uiCam == (UnityEngine.Object) null)
        this.m_UICams.RemoveAt(index);
      else if (universalInputManager.GetInputHitInfo(uiCam, out hit))
        return this.TryGetPegUIElementFromHit(hit);
    }
    hit = new RaycastHit();
    return (PegUIElement) null;
  }

  private PegUIElement TryGetPegUIElementFromHit(RaycastHit hit)
  {
    PegUIElement component1 = hit.transform.GetComponent<PegUIElement>();
    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      return component1;
    PegUIElementProxy component2 = hit.transform.GetComponent<PegUIElementProxy>();
    return (UnityEngine.Object) component2 != (UnityEngine.Object) null ? component2.Owner : (PegUIElement) null;
  }

  public void DoMouseDown(PegUIElement element, Vector3 mouseDownPos)
  {
    this.m_currentElement = element;
    this.m_mouseDownElement = element;
    this.m_currentElement.TriggerPress();
    this.m_lastClickPosition = mouseDownPos;
    if (!this.IsDragAmountAboveDragTolerance(this.m_currentElement))
      return;
    this.m_currentElement.TriggerDrag();
  }

  public void RemoveAsMouseDownElement(PegUIElement element)
  {
    if ((UnityEngine.Object) this.m_mouseDownElement == (UnityEngine.Object) null || (UnityEngine.Object) element != (UnityEngine.Object) this.m_mouseDownElement)
      return;
    this.m_mouseDownElement.TriggerReleaseAll((UnityEngine.Object) this.m_currentElement == (UnityEngine.Object) this.m_mouseDownElement);
    this.m_mouseDownElement = (PegUIElement) null;
  }

  public Vector3 GetDragDelta() => InputCollection.GetMousePosition() - this.m_lastClickPosition;

  private void MouseInputUpdate()
  {
    if (UniversalInputManager.Get() == null || !this.m_hasFocus || this.m_uguiActive)
      return;
    bool flag1 = false;
    foreach (PegUICustomBehavior customBehavior in this.m_customBehaviors)
    {
      if (customBehavior.UpdateUI())
      {
        flag1 = true;
        break;
      }
    }
    if (flag1)
    {
      if ((UnityEngine.Object) this.m_mouseDownElement != (UnityEngine.Object) null)
        this.m_mouseDownElement.TriggerOut();
      this.m_mouseDownElement = (PegUIElement) null;
    }
    else
    {
      if (InputCollection.GetMouseButton(0) && (UnityEngine.Object) this.m_mouseDownElement != (UnityEngine.Object) null && this.m_lastClickPosition != Vector3.zero && this.IsDragAmountAboveDragTolerance(this.m_mouseDownElement))
        this.m_mouseDownElement.TriggerDrag();
      if ((double) this.m_lastClickTimer != -1.0)
        this.m_lastClickTimer += Time.deltaTime;
      if ((double) this.m_mouseDownTimer != -1.0)
        this.m_mouseDownTimer += Time.deltaTime;
      PegUIElement hitElement = this.FindHitElement();
      if ((UnityEngine.Object) hitElement != (UnityEngine.Object) null && HearthstoneApplication.IsInternal() && Options.Get().GetInt(Option.PEGUI_DEBUG) >= 3)
        Debug.Log((object) string.Format("{0,-7} {1}", (object) "HIT:", (object) DebugUtils.GetHierarchyPath((UnityEngine.Object) hitElement, '/')));
      bool flag2 = !UniversalInputManager.Get().IsTouchMode() || InputCollection.GetMouseButton(0) || InputCollection.GetMouseButtonUp(0);
      if (flag2)
        this.m_prevElement = this.m_currentElement;
      if ((bool) (UnityEngine.Object) hitElement && hitElement.IsEnabled())
        this.m_currentElement = hitElement;
      else if (flag2)
        this.m_currentElement = (PegUIElement) null;
      if ((bool) (UnityEngine.Object) this.m_prevElement && (UnityEngine.Object) this.m_currentElement != (UnityEngine.Object) this.m_prevElement)
      {
        if ((UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
          PegCursor.Get().SetMode(PegCursor.Mode.UP);
        this.m_prevElement.TriggerOut();
        this.m_lastClickTimer = -1f;
      }
      if (InputCollection.GetMouseButton(0) && (UnityEngine.Object) this.m_mouseDownElement != (UnityEngine.Object) null && (UnityEngine.Object) this.m_currentElement == (UnityEngine.Object) this.m_mouseDownElement && (double) this.m_mouseDownTimer > 0.449999988079071)
        this.m_mouseDownElement.TriggerHold();
      if ((UnityEngine.Object) this.m_currentElement == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
        {
          if (InputCollection.GetMouseButtonDown(0))
            PegCursor.Get().SetMode(PegCursor.Mode.DOWN);
          else if (!InputCollection.GetMouseButton(0))
            PegCursor.Get().SetMode(PegCursor.Mode.UP);
        }
        if (!(bool) (UnityEngine.Object) this.m_mouseDownElement || !InputCollection.GetMouseButtonUp(0))
          return;
        this.m_mouseDownElement.TriggerReleaseAll(false);
        this.m_mouseDownElement = (PegUIElement) null;
      }
      else
      {
        if (!this.UpdateMouseLeftClick())
          this.UpdateMouseLeftHold();
        this.UpdateMouseRightClick();
        this.UpdateMouseOver();
      }
    }
  }

  private void RegisterMouseListener() => UniversalInputManager.Get().RegisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));

  private bool UpdateMouseLeftClick()
  {
    bool flag = false;
    if (InputCollection.GetMouseButtonDown(0))
    {
      flag = true;
      if ((UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
      {
        if (this.m_currentElement.GetCursorDown() != PegCursor.Mode.NONE)
          PegCursor.Get().SetMode(this.m_currentElement.GetCursorDown());
        else
          PegCursor.Get().SetMode(PegCursor.Mode.DOWN);
      }
      this.m_mouseDownTimer = 0.0f;
      if (UniversalInputManager.Get().IsTouchMode() && this.m_currentElement.GetReceiveOverWithMouseDown())
        this.m_currentElement.TriggerOver();
      this.m_currentElement.TriggerPress();
      this.m_lastClickPosition = InputCollection.GetMousePosition();
      this.m_mouseDownElement = this.m_currentElement;
    }
    if (InputCollection.GetMouseButtonUp(0))
    {
      flag = true;
      if ((double) this.m_lastClickTimer > 0.0 && (double) this.m_lastClickTimer <= 0.699999988079071 && this.m_currentElement.DoubleClickEnabled)
      {
        this.m_currentElement.TriggerDoubleClick();
        this.m_lastClickTimer = -1f;
      }
      else
      {
        if ((UnityEngine.Object) this.m_mouseDownElement == (UnityEngine.Object) this.m_currentElement || this.m_currentElement.GetReceiveReleaseWithoutMouseDown())
        {
          if ((double) this.m_mouseDownTimer <= 0.400000005960464)
            this.m_currentElement.TriggerTap();
          if (PegUI.OnReleasePreTrigger != null)
            PegUI.OnReleasePreTrigger(this.m_currentElement);
          this.m_currentElement.TriggerRelease();
        }
        if ((bool) (UnityEngine.Object) this.m_mouseDownElement)
        {
          this.m_lastClickTimer = 0.0f;
          this.m_mouseDownElement.TriggerReleaseAll((UnityEngine.Object) this.m_currentElement == (UnityEngine.Object) this.m_mouseDownElement);
          this.m_mouseDownElement = (PegUIElement) null;
        }
      }
      if (this.m_currentElement.GetReceiveOverWithMouseDown())
        this.m_currentElement.TriggerOut();
      if ((UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
        PegCursor.Get().SetMode(this.m_currentElement.GetCursorOver() != PegCursor.Mode.NONE ? this.m_currentElement.GetCursorOver() : PegCursor.Mode.OVER);
      this.m_mouseDownTimer = -1f;
      this.m_lastClickPosition = Vector3.zero;
      if (UniversalInputManager.Get().IsTouchMode())
      {
        this.m_currentElement = (PegUIElement) null;
        this.m_prevElement = (PegUIElement) null;
      }
    }
    return flag;
  }

  private bool UpdateMouseLeftHold()
  {
    if (!InputCollection.GetMouseButton(0))
      return false;
    if (this.m_currentElement.GetReceiveOverWithMouseDown() && (UnityEngine.Object) this.m_currentElement != (UnityEngine.Object) this.m_prevElement)
    {
      if ((UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
      {
        if (this.m_currentElement.GetCursorOver() != PegCursor.Mode.NONE)
          PegCursor.Get().SetMode(this.m_currentElement.GetCursorOver());
        else
          PegCursor.Get().SetMode(PegCursor.Mode.OVER);
      }
      this.m_currentElement.TriggerOver();
    }
    return true;
  }

  private bool UpdateMouseRightClick()
  {
    bool flag = false;
    if (InputCollection.GetMouseButtonDown(1))
    {
      flag = true;
      if ((UnityEngine.Object) this.m_currentElement != (UnityEngine.Object) null)
        this.m_currentElement.TriggerRightClick();
    }
    return flag;
  }

  private void UpdateMouseOver()
  {
    if ((UnityEngine.Object) this.m_currentElement == (UnityEngine.Object) null || UniversalInputManager.Get().IsTouchMode() && (!InputCollection.GetMouseButton(0) || !this.m_currentElement.GetReceiveOverWithMouseDown()) || (UnityEngine.Object) this.m_currentElement == (UnityEngine.Object) this.m_prevElement)
      return;
    if ((UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
    {
      if (this.m_currentElement.GetCursorOver() != PegCursor.Mode.NONE)
        PegCursor.Get().SetMode(this.m_currentElement.GetCursorOver());
      else
        PegCursor.Get().SetMode(PegCursor.Mode.OVER);
    }
    this.m_currentElement.TriggerOver();
  }

  private void OnAppFocusChanged(bool focus, object userData) => this.m_hasFocus = focus;

  public void OnUGUIActiveChanged(bool active) => this.m_uguiActive = active;

  private void OnMouseOnOrOffScreen(bool onScreen)
  {
    if (onScreen)
      return;
    this.m_lastClickPosition = Vector3.zero;
    if ((UnityEngine.Object) this.m_currentElement != (UnityEngine.Object) null)
    {
      this.m_currentElement.TriggerOut();
      this.m_currentElement = (PegUIElement) null;
    }
    if ((UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
      PegCursor.Get().SetMode(PegCursor.Mode.UP);
    if ((UnityEngine.Object) this.m_prevElement != (UnityEngine.Object) null)
    {
      this.m_prevElement.TriggerOut();
      this.m_prevElement = (PegUIElement) null;
    }
    this.m_lastClickTimer = -1f;
  }

  private bool IsDragAmountAboveDragTolerance(PegUIElement draggedElement)
  {
    Vector3 vector3 = InputCollection.GetMousePosition() - this.m_lastClickPosition;
    Vector3 dragTolerance = draggedElement.GetDragTolerance();
    if ((double) dragTolerance.x != 0.0 && (double) Mathf.Abs(vector3.x) > (double) Mathf.Abs(dragTolerance.x * this.m_dragToleranceDpiAdjustmentFactor))
      return true;
    return (double) dragTolerance.y != 0.0 && (double) Mathf.Abs(vector3.y) > (double) Mathf.Abs(dragTolerance.y * this.m_dragToleranceDpiAdjustmentFactor);
  }

  public void EnableSwipeDetection(Rect swipeRect, PegUI.DelSwipeListener listener) => this.m_swipeListener = listener;

  public void CancelSwipeDetection(PegUI.DelSwipeListener listener)
  {
    if (!(listener == this.m_swipeListener))
      return;
    this.m_swipeListener = (PegUI.DelSwipeListener) null;
  }

  public void RegisterCustomBehavior(PegUICustomBehavior behavior) => this.m_customBehaviors.Add(behavior);

  public void UnregisterCustomBehavior(PegUICustomBehavior behavior) => this.m_customBehaviors.Remove(behavior);

  public bool IsUsingRenderPassPriorityHitTest => this.m_newHitDetectionComponents.Count > 0;

  public Camera LastCameraPriorityHitCamera => this.m_cameraPriorityHitCamera;

  public void RegisterForRenderPassPriorityHitTest(Component component) => this.m_newHitDetectionComponents.Add(component);

  public void UnregisterFromRenderPassPriorityHitTest(Component component) => this.m_newHitDetectionComponents.Remove(component);

  public enum SWIPE_DIRECTION
  {
    RIGHT,
    LEFT,
  }

  public delegate void DelSwipeListener(PegUI.SWIPE_DIRECTION direction);
}
