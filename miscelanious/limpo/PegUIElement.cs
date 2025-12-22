using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Hearthstone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PegUIElement : MonoBehaviour
{
  private MeshFilter m_meshFilter;
  private MeshRenderer m_renderer;
  private Map<UIEventType, List<UIEvent.Handler>> m_eventListeners = new Map<UIEventType, List<UIEvent.Handler>>();
  private bool m_enabled = true;
  private bool m_focused;
  private bool m_receiveReleaseWithoutMouseDown;
  private object m_data;
  private Vector3 m_originalLocalPosition;
  private PegUIElement.InteractionState m_interactionState;
  private Vector3 m_dragTolerance = Vector3.one * 40f;
  private PegCursor.Mode m_cursorDownOverride = PegCursor.Mode.NONE;
  private PegCursor.Mode m_cursorOverOverride = PegCursor.Mode.NONE;

  public bool DoubleClickEnabled { get; private set; }

  public float SetEnabledLastCallTime { private set; get; }

  protected virtual void Awake() => this.DoubleClickEnabled = this.DoubleClickEnabled || this.HasOverriddenDoubleClick();

  protected virtual void OnDestroy()
  {
  }

  protected virtual void OnOver(PegUIElement.InteractionState oldState)
  {
  }

  protected virtual void OnOut(PegUIElement.InteractionState oldState)
  {
  }

  protected virtual void OnPress()
  {
  }

  protected virtual void OnTap()
  {
  }

  protected virtual void OnRelease()
  {
  }

  protected virtual void OnReleaseAll(bool mouseIsOver)
  {
  }

  protected virtual void OnDrag()
  {
  }

  protected virtual void OnDoubleClick()
  {
  }

  protected virtual void OnRightClick()
  {
  }

  protected virtual void OnHold()
  {
  }

  public void SetInteractionState(PegUIElement.InteractionState state) => this.m_interactionState = state;

  public virtual void TriggerOver()
  {
    if (!this.m_enabled || this.m_focused)
      return;
    this.PrintLog("OVER", PegUIElement.PegUILogLevel.ALL_EVENTS);
    this.m_focused = true;
    PegUIElement.InteractionState interactionState = this.m_interactionState;
    this.m_interactionState = PegUIElement.InteractionState.Over;
    this.OnOver(interactionState);
    this.DispatchEvent(new UIEvent(UIEventType.ROLLOVER, this));
  }

  public virtual void TriggerOut()
  {
    if (!this.m_enabled)
      return;
    this.PrintLog("OUT", PegUIElement.PegUILogLevel.ALL_EVENTS);
    this.m_focused = false;
    PegUIElement.InteractionState interactionState = this.m_interactionState;
    this.m_interactionState = PegUIElement.InteractionState.Out;
    this.OnOut(interactionState);
    this.DispatchEvent(new UIEvent(UIEventType.ROLLOUT, this));
  }

  public virtual void TriggerPress()
  {
    if (!this.m_enabled)
      return;
    this.PrintLog("PRESS", PegUIElement.PegUILogLevel.PRESS);
    this.m_focused = true;
    this.m_interactionState = PegUIElement.InteractionState.Down;
    this.OnPress();
    this.DispatchEvent(new UIEvent(UIEventType.PRESS, this));
  }

  public virtual void TriggerTap()
  {
    if (!this.m_enabled)
      return;
    this.PrintLog("TAP", PegUIElement.PegUILogLevel.ALL_EVENTS, true);
    this.m_interactionState = PegUIElement.InteractionState.Up;
    this.OnTap();
    this.DispatchEvent(new UIEvent(UIEventType.TAP, this));
  }

  public virtual void TriggerRelease()
  {
    if (!this.m_enabled)
      return;
    this.PrintLog("RELEASE", PegUIElement.PegUILogLevel.ALL_EVENTS);
    this.m_interactionState = PegUIElement.InteractionState.Up;
    if (this.IsScrolling())
      return;
    this.OnRelease();
    this.DispatchEvent(new UIEvent(UIEventType.RELEASE, this));
  }

  public void TriggerReleaseAll(bool mouseIsOver)
  {
    if (!this.m_enabled)
      return;
    this.m_interactionState = PegUIElement.InteractionState.Up;
    this.OnReleaseAll(mouseIsOver);
    this.DispatchEvent((UIEvent) new UIReleaseAllEvent(mouseIsOver, this));
  }

  public void TriggerDrag()
  {
    if (!this.m_enabled)
      return;
    this.PrintLog("DRAG", PegUIElement.PegUILogLevel.ALL_EVENTS);
    this.m_interactionState = PegUIElement.InteractionState.Down;
    this.OnDrag();
    this.DispatchEvent(new UIEvent(UIEventType.DRAG, this));
  }

  public void TriggerHold()
  {
    if (!this.m_enabled)
      return;
    this.PrintLog("HOLD", PegUIElement.PegUILogLevel.ALL_EVENTS);
    this.m_interactionState = PegUIElement.InteractionState.Down;
    this.OnHold();
    this.DispatchEvent(new UIEvent(UIEventType.HOLD, this));
  }

  public void TriggerDoubleClick()
  {
    if (!this.m_enabled)
      return;
    this.PrintLog("DCLICK", PegUIElement.PegUILogLevel.ALL_EVENTS);
    this.m_interactionState = PegUIElement.InteractionState.Down;
    this.OnDoubleClick();
    this.DispatchEvent(new UIEvent(UIEventType.DOUBLECLICK, this));
  }

  public void TriggerRightClick()
  {
    if (!this.m_enabled)
      return;
    this.PrintLog("RCLICK", PegUIElement.PegUILogLevel.ALL_EVENTS);
    this.OnRightClick();
    this.DispatchEvent(new UIEvent(UIEventType.RIGHTCLICK, this));
  }

  public void SetDragTolerance(float newTolerance) => this.SetDragTolerance(Vector3.one * newTolerance);

  public void SetDragTolerance(Vector3 newTolerance) => this.m_dragTolerance = newTolerance;

  public Vector3 GetDragTolerance() => this.m_dragTolerance;

  public virtual bool AddEventListener(UIEventType type, UIEvent.Handler handler)
  {
    this.DoubleClickEnabled |= type == UIEventType.DOUBLECLICK;
    List<UIEvent.Handler> handlerList1;
    if (!this.m_eventListeners.TryGetValue(type, out handlerList1))
    {
      List<UIEvent.Handler> handlerList2 = new List<UIEvent.Handler>();
      this.m_eventListeners.Add(type, handlerList2);
      handlerList2.Add(handler);
      return true;
    }
    if (handlerList1.Contains(handler))
      return false;
    handlerList1.Add(handler);
    return true;
  }

  public virtual bool RemoveEventListener(UIEventType type, UIEvent.Handler handler)
  {
    List<UIEvent.Handler> handlerList;
    return this.m_eventListeners.TryGetValue(type, out handlerList) && handlerList.Remove(handler);
  }

  public void ClearEventListeners() => this.m_eventListeners.Clear();

  public bool HasEventListener(UIEventType type)
  {
    List<UIEvent.Handler> handlerList;
    return this.m_eventListeners.TryGetValue(type, out handlerList) && handlerList.Count > 0;
  }

  public virtual void SetEnabled(bool enabled, bool isInternal = false)
  {
    if (!isInternal)
      this.SetEnabledLastCallTime = Time.realtimeSinceStartup;
    if (enabled)
      this.PrintLog("ENABLE", PegUIElement.PegUILogLevel.ALL_EVENTS);
    else
      this.PrintLog("DISABLE", PegUIElement.PegUILogLevel.ALL_EVENTS);
    bool flag = this.m_enabled != enabled;
    if (!enabled & flag)
      this.DispatchEvent(new UIEvent(UIEventType.DISABLE, this));
    this.m_enabled = enabled;
    if (enabled & flag)
      this.DispatchEvent(new UIEvent(UIEventType.ENABLE, this));
    if (this.m_enabled)
      return;
    this.m_focused = false;
  }

  public bool IsEnabled() => this.m_enabled;

  public void SetReceiveReleaseWithoutMouseDown(bool receiveReleaseWithoutMouseDown) => this.m_receiveReleaseWithoutMouseDown = receiveReleaseWithoutMouseDown;

  public bool GetReceiveReleaseWithoutMouseDown() => this.m_receiveReleaseWithoutMouseDown;

  public bool GetReceiveOverWithMouseDown() => UniversalInputManager.Get().IsTouchMode();

  public PegUIElement.InteractionState GetInteractionState() => this.m_interactionState;

  public void SetData(object data) => this.m_data = data;

  public object GetData() => this.m_data;

  public void SetOriginalLocalPosition() => this.SetOriginalLocalPosition(this.transform.localPosition);

  public void SetOriginalLocalPosition(Vector3 pos) => this.m_originalLocalPosition = pos;

  public Vector3 GetOriginalLocalPosition() => this.m_originalLocalPosition;

  public void SetCursorDown(PegCursor.Mode mode) => this.m_cursorDownOverride = mode;

  public PegCursor.Mode GetCursorDown() => this.m_cursorDownOverride;

  public void SetCursorOver(PegCursor.Mode mode) => this.m_cursorOverOverride = mode;

  public PegCursor.Mode GetCursorOver() => this.m_cursorOverOverride;

  private void DispatchEvent(UIEvent e)
  {
    List<UIEvent.Handler> handlerList;
    if (!this.m_eventListeners.TryGetValue(e.GetEventType(), out handlerList))
      return;
    foreach (UIEvent.Handler handler in handlerList.ToArray())
      handler(e);
  }

  private bool HasOverriddenDoubleClick()
  {
    System.Type type = ((object) this).GetType();
    System.Type rootType = typeof (PegUIElement);
    rootType.GetMethod("OnDoubleClick", BindingFlags.Instance | BindingFlags.NonPublic);
    type.GetMethod("OnDoubleClick", BindingFlags.Instance | BindingFlags.NonPublic);
    return GeneralUtils.IsOverriddenMethod(type, rootType, "OnDoubleClick");
  }

  private void PrintLog(string evt, PegUIElement.PegUILogLevel logLevel, bool printOnScreen = false)
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null || !HearthstoneApplication.IsInternal() || (PegUIElement.PegUILogLevel) Options.Get().GetInt(Option.PEGUI_DEBUG) < logLevel)
      return;
    string hierarchyPath = DebugUtils.GetHierarchyPath((UnityEngine.Object) this.gameObject, '/');
    string str = string.Format("{0,-7} {1}", (object) (evt + ":"), (object) hierarchyPath);
    Log.All.PrintInfo(str);
    if (!printOnScreen)
      return;
    Log.All.ForceScreenPrint(Blizzard.T5.Logging.LogLevel.Info, true, str);
  }

  private bool IsScrolling() => ((IEnumerable<UIBScrollable.IContent>) this.GetComponentsInParent<UIBScrollable.IContent>()).Any<UIBScrollable.IContent>((Func<UIBScrollable.IContent, bool>) (scrollable => scrollable.Scrollable.IsTouchDragging()));

  public enum InteractionState
  {
    None,
    Out,
    Over,
    Down,
    Up,
    Disabled,
  }

  public enum PegUILogLevel
  {
    NONE,
    PRESS,
    ALL_EVENTS,
    HIT_TEST,
  }
}
