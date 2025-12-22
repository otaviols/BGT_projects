using Blizzard.T5.Core;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class UIBScrollable : PegUICustomBehavior
{
  [CustomEditField(Sections = "Camera Settings")]
  public bool m_UseCameraFromLayer;
  [CustomEditField(Sections = "Preferences")]
  public float m_ScrollWheelAmount = 0.1f;
  [CustomEditField(Sections = "Preferences")]
  public UIBScrollable.ScrollWheelMode m_ScrollWheelMode;
  [CustomEditField(Sections = "Preferences")]
  public float m_ScrollBottomPadding;
  [CustomEditField(Sections = "Preferences")]
  public iTween.EaseType m_ScrollEaseType = iTween.Defaults.easeType;
  [CustomEditField(Sections = "Preferences")]
  public float m_ScrollTweenTime = 0.2f;
  [CustomEditField(Sections = "Preferences")]
  public UIBScrollable.ScrollDirection m_ScrollPlane = UIBScrollable.ScrollDirection.Z;
  [CustomEditField(Sections = "Preferences")]
  public bool m_ScrollDirectionReverse;
  [CustomEditField(Sections = "Preferences")]
  [Tooltip("If scrolling is active, all PegUI calls will be suppressed")]
  public bool m_OverridePegUI;
  [CustomEditField(Sections = "Preferences")]
  public bool m_ForceScrollAreaHitTest;
  [CustomEditField(Sections = "Preferences")]
  public bool m_ScrollOnMouseDrag;
  [CustomEditField(Sections = "Bounds Settings")]
  public BoxCollider m_ScrollBounds;
  [Tooltip("Determines full area finger is allowed continue scrolling once it has started. Position this behind/below the ScrollBounds.")]
  [CustomEditField(Sections = "Optional Bounds Settings")]
  public BoxCollider m_TouchDragFullArea;
  [CustomEditField(Sections = "Thumb Settings")]
  public BoxCollider m_ScrollTrack;
  [CustomEditField(Sections = "Thumb Settings")]
  public ScrollBarThumb m_ScrollThumb;
  [CustomEditField(Sections = "Thumb Settings")]
  public bool m_HideThumbWhenDisabled;
  [CustomEditField(Sections = "Thumb Settings")]
  public GameObject m_scrollTrackCover;
  [CustomEditField(Sections = "Bounds Settings")]
  [SerializeField]
  private GameObject m_ScrollObject;
  [CustomEditField(Sections = "Bounds Settings")]
  public float m_VisibleObjectThreshold;
  [CustomEditField(Sections = "Preferences")]
  public bool m_UseScrollContentsInHitTest = true;
  [Tooltip("Drag distance required to initiate deck tile dragging (inches)")]
  [CustomEditField(Sections = "Touch Settings")]
  public float m_DeckTileDragThreshold = 0.04f;
  [CustomEditField(Sections = "Touch Settings")]
  [Tooltip("Drag distance required to initiate scroll dragging (inches)")]
  public float m_ScrollDragThreshold = 0.04f;
  [Tooltip("Stopping speed for scrolling after the user has let go")]
  [CustomEditField(Sections = "Touch Settings")]
  public float m_MinKineticScrollSpeed = 0.01f;
  [Tooltip("Resistance for slowing down scrolling after the user has let go")]
  [CustomEditField(Sections = "Touch Settings")]
  public float m_KineticScrollFriction = 6f;
  [Tooltip("Strength of the boundary springs")]
  [CustomEditField(Sections = "Touch Settings")]
  public float m_ScrollBoundsSpringK = 700f;
  [CustomEditField(Sections = "Touch Settings")]
  [Tooltip("Distance at which the out-of-bounds scroll value will snapped to 0 or 1")]
  public float m_MinOutOfBoundsScrollValue = 1f / 1000f;
  [CustomEditField(Sections = "Touch Settings")]
  [Tooltip("Use this to match scaling issues.")]
  public float m_ScrollDeltaMultiplier = 1f;
  [CustomEditField(Sections = "Touch Settings")]
  public List<BoxCollider> m_TouchScrollBlockers = new List<BoxCollider>();
  public UIBScrollable.HeightMode m_HeightMode = UIBScrollable.HeightMode.UseScrollableItem;
  private bool m_Enabled = true;
  private float m_ScrollValue;
  private float m_LastTouchScrollValue;
  private bool m_InputBlocked;
  private bool m_Pause;
  private bool m_PauseUpdateScrollHeight;
  private bool m_overrideHideThumb;
  private Vector2? m_TouchBeginScreenPos;
  private Vector3? m_TouchDragBeginWorldPos;
  private float m_TouchDragBeginScrollValue;
  private float m_prevScrollValue;
  private Vector3 m_ScrollAreaStartPos;
  private float m_ScrollThumbStartYPos;
  private UIBScrollable.ScrollHeightCallback m_ScrollHeightCallback;
  private List<UIBScrollable.EnableScroll> m_EnableScrollListeners = new List<UIBScrollable.EnableScroll>();
  private float m_LastScrollHeightRecorded;
  private float m_PolledScrollHeight;
  private List<UIBScrollable.VisibleAffectedObject> m_VisibleAffectedObjects = new List<UIBScrollable.VisibleAffectedObject>();
  private List<UIBScrollable.FastVisibleAffectedObject> m_fastVisibleAffectedObjects = new List<UIBScrollable.FastVisibleAffectedObject>();
  private List<UIBScrollable.OnTouchScrollStarted> m_TouchScrollStartedListeners = new List<UIBScrollable.OnTouchScrollStarted>();
  private List<UIBScrollable.OnTouchScrollEnded> m_TouchScrollEndedListeners = new List<UIBScrollable.OnTouchScrollEnded>();
  private bool m_ForceShowVisibleAffectedObjects;
  private List<UIBScrollableItem> m_scrollableItems = new List<UIBScrollableItem>();
  private int m_currentHierarchyCount;
  private Camera m_scrollTrackCamera;
  private CameraOverridePass m_scrollTrackCameraOverridePass;
  private static Map<string, float> s_SavedScrollValues = new Map<string, float>();

  [CustomEditField(Sections = "Scroll")]
  public float ScrollValue
  {
    get => this.m_ScrollValue;
    set
    {
      if (Application.isEditor)
        return;
      this.SetScroll(value, clamp: false);
    }
  }

  [Overridable]
  public float ImmediateScrollValue
  {
    get => this.m_ScrollValue;
    set => this.SetScrollImmediate(value);
  }

  [Overridable]
  public float ScrollBottomPadding
  {
    get => this.m_ScrollBottomPadding;
    set => this.m_ScrollBottomPadding = value;
  }

  public GameObject ScrollObject
  {
    get => this.m_ScrollObject;
    set
    {
      this.m_ScrollObject = value;
      this.SetupScrollObject();
    }
  }

  public static void DefaultVisibleAffectedCallback(GameObject obj, bool visible)
  {
    if (obj.activeSelf == visible)
      return;
    obj.SetActive(visible);
  }

  protected override void Awake()
  {
    this.ResetScrollStartPosition();
    this.SaveScrollThumbStartHeight();
    if ((UnityEngine.Object) this.m_ScrollTrack != (UnityEngine.Object) null && !(bool) UniversalInputManager.UsePhoneUI)
    {
      PegUIElement component1 = this.m_ScrollTrack.GetComponent<PegUIElement>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
        component1.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.StartDragging()));
      PegUIElement component2 = this.m_ScrollThumb.GetComponent<PegUIElement>();
      if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
        component2.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.StartDragging()));
    }
    if (this.m_OverridePegUI)
      base.Awake();
    if (!((UnityEngine.Object) this.m_ScrollObject != (UnityEngine.Object) null))
      return;
    this.SetupScrollObject();
  }

  public void RegisterScrollableItem(UIBScrollableItem scrollableItem)
  {
    if (this.m_scrollableItems.Contains(scrollableItem))
      return;
    this.m_scrollableItems.Add(scrollableItem);
  }

  public void RemoveScrollableItem(UIBScrollableItem scrollableItem) => this.m_scrollableItems.Remove(scrollableItem);

  public void Start()
  {
    if (!((UnityEngine.Object) this.m_scrollTrackCover != (UnityEngine.Object) null))
      return;
    this.m_scrollTrackCover.SetActive(false);
  }

  protected override void OnDestroy()
  {
    if (!this.m_OverridePegUI)
      return;
    base.OnDestroy();
  }

  private void Update()
  {
    int hierarchyCount = this.transform.hierarchyCount;
    if (hierarchyCount != this.m_currentHierarchyCount)
    {
      this.m_currentHierarchyCount = hierarchyCount;
      this.SetupScrollObject();
    }
    this.UpdateScroll();
    if (!this.m_Enabled || this.m_InputBlocked || this.m_Pause || (UnityEngine.Object) this.GetScrollCamera() == (UnityEngine.Object) null)
      return;
    if (this.IsInputOverScrollableArea(this.m_ScrollBounds, out RaycastHit _))
    {
      float axis = Input.GetAxis("Mouse ScrollWheel");
      if ((double) axis != 0.0)
      {
        float num = this.m_ScrollWheelMode != UIBScrollable.ScrollWheelMode.FixedRate ? this.m_ScrollWheelAmount * 10f : this.m_ScrollWheelAmount / this.GetTotalWorldScrollHeight();
        this.AddScroll((float) (0.0 - (double) axis * (double) num));
      }
    }
    if ((UnityEngine.Object) this.m_ScrollThumb != (UnityEngine.Object) null && this.m_ScrollThumb.IsDragging())
    {
      this.DragThumb();
    }
    else
    {
      if (!UniversalInputManager.Get().IsTouchMode() && !this.m_ScrollOnMouseDrag)
        return;
      this.DragContent();
    }
  }

  private void SetupScrollObject()
  {
    if ((UnityEngine.Object) this.m_ScrollObject == (UnityEngine.Object) null)
    {
      this.m_scrollableItems.Clear();
    }
    else
    {
      UIBScrollable.ContentComponent parent = this.m_ScrollObject.GetComponent<UIBScrollable.ContentComponent>();
      if ((UnityEngine.Object) parent == (UnityEngine.Object) null)
        parent = this.m_ScrollObject.AddComponent<UIBScrollable.ContentComponent>();
      else if (!((Enum) parent.hideFlags).HasFlag((Enum) HideFlags.DontSave))
      {
        if (Application.IsPlaying((UnityEngine.Object) this))
          UnityEngine.Object.Destroy((UnityEngine.Object) parent);
        else
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) parent);
        parent = this.m_ScrollObject.AddComponent<UIBScrollable.ContentComponent>();
      }
      parent.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
      parent.Scrollable = this;
      this.m_ScrollObject.GetComponentsInChildren<UIBScrollableItem>(true, this.m_scrollableItems);
      int index = 0;
      for (int count = this.m_scrollableItems.Count; index < count; ++index)
        this.m_scrollableItems[index].SetScrollableParent((UIBScrollable.IContent) parent);
    }
  }

  private bool IsInputOverScrollableArea(BoxCollider scrollableBounds, out RaycastHit hitInfo)
  {
    Camera scrollCamera = this.GetScrollCamera();
    if (UniversalInputManager.Get() == null || (UnityEngine.Object) scrollCamera == (UnityEngine.Object) null || (UnityEngine.Object) this.m_ScrollBounds == (UnityEngine.Object) null)
    {
      hitInfo = new RaycastHit();
      return false;
    }
    bool flag = !this.m_ForceScrollAreaHitTest ? (!PegUI.IsInitialized() || !PegUI.Get().IsUsingRenderPassPriorityHitTest ? UniversalInputManager.Get().InputIsOver(scrollCamera, scrollableBounds.gameObject, out hitInfo) : UniversalInputManager.Get().InputIsOverByRenderPass(scrollableBounds.gameObject, out hitInfo)) : UniversalInputManager.Get().ForcedInputIsOver(scrollCamera, scrollableBounds.gameObject, out hitInfo);
    if (this.m_UseScrollContentsInHitTest && (UnityEngine.Object) this.m_ScrollObject != (UnityEngine.Object) null)
      flag = ((flag ? 1 : 0) | (!((UnityEngine.Object) hitInfo.collider != (UnityEngine.Object) null) ? 0 : (hitInfo.collider.transform.IsChildOf(this.m_ScrollObject.transform) ? 1 : 0))) != 0;
    return flag;
  }

  public override bool UpdateUI() => this.IsTouchDragging() && this.m_Enabled;

  public void ResetScrollStartPosition()
  {
    if (!((UnityEngine.Object) this.m_ScrollObject != (UnityEngine.Object) null))
      return;
    this.m_ScrollAreaStartPos = this.m_ScrollObject.transform.localPosition;
  }

  public void ResetScrollStartPosition(Vector3 position)
  {
    if (!((UnityEngine.Object) this.m_ScrollObject != (UnityEngine.Object) null))
      return;
    this.m_ScrollAreaStartPos = position;
  }

  public void AddVisibleAffectedObject(
    GameObject obj,
    Vector3 extents,
    bool visible,
    UIBScrollable.VisibleAffected callback = null)
  {
    this.m_VisibleAffectedObjects.Add(new UIBScrollable.VisibleAffectedObject()
    {
      Obj = obj,
      Extents = extents,
      Visible = visible,
      Callback = callback == null ? new UIBScrollable.VisibleAffected(UIBScrollable.DefaultVisibleAffectedCallback) : callback
    });
  }

  public void AddFastVisibleAffectedObject(
    GameObject obj,
    Vector3 extents,
    bool visible,
    float buffer,
    Action<int, int, bool> callback = null)
  {
    this.m_fastVisibleAffectedObjects.Add(new UIBScrollable.FastVisibleAffectedObject()
    {
      TopObj = obj,
      Extents = extents,
      Buffer = buffer,
      Callback = callback
    });
  }

  public void ChangeExtentsOnFastVisibleObject(GameObject topObj, Vector3 extents)
  {
    if ((UnityEngine.Object) topObj == (UnityEngine.Object) null)
    {
      Log.UIFramework.PrintError("Null TopObj passed into ChangeExtentsOnFastVisibleObject");
    }
    else
    {
      foreach (UIBScrollable.FastVisibleAffectedObject visibleAffectedObject in this.m_fastVisibleAffectedObjects)
      {
        if ((UnityEngine.Object) visibleAffectedObject.TopObj == (UnityEngine.Object) topObj)
        {
          visibleAffectedObject.Extents = extents;
          return;
        }
      }
      Log.UIFramework.PrintError("Fast visible object {0} not registered", (object) topObj.gameObject.name);
    }
  }

  public void RemoveVisibleAffectedObject(GameObject obj, UIBScrollable.VisibleAffected callback) => this.m_VisibleAffectedObjects.RemoveAll((Predicate<UIBScrollable.VisibleAffectedObject>) (o => o.Obj == obj && o.Callback == callback));

  public void RemoveFastVisibleAffectedObject(GameObject obj)
  {
    for (int index = 0; index < this.m_fastVisibleAffectedObjects.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_fastVisibleAffectedObjects[index].TopObj == (UnityEngine.Object) obj)
      {
        this.m_fastVisibleAffectedObjects.RemoveAt(index);
        break;
      }
    }
  }

  public void ClearVisibleAffectObjects() => this.m_VisibleAffectedObjects.Clear();

  public IEnumerable<UIBScrollable.VisibleAffectedObject> GetVisibleAffectedObjects() => (IEnumerable<UIBScrollable.VisibleAffectedObject>) this.m_VisibleAffectedObjects;

  public void ForceVisibleAffectedObjectsShow(bool show)
  {
    if (this.m_ForceShowVisibleAffectedObjects == show)
      return;
    this.m_ForceShowVisibleAffectedObjects = show;
    this.UpdateAndFireVisibleAffectedObjects();
  }

  public void AddEnableScrollListener(UIBScrollable.EnableScroll dlg) => this.m_EnableScrollListeners.Add(dlg);

  public void RemoveEnableScrollListener(UIBScrollable.EnableScroll dlg) => this.m_EnableScrollListeners.Remove(dlg);

  public void AddTouchScrollStartedListener(UIBScrollable.OnTouchScrollStarted dlg) => this.m_TouchScrollStartedListeners.Add(dlg);

  public void RemoveTouchScrollStartedListener(UIBScrollable.OnTouchScrollStarted dlg) => this.m_TouchScrollStartedListeners.Remove(dlg);

  public void AddTouchScrollEndedListener(UIBScrollable.OnTouchScrollEnded dlg) => this.m_TouchScrollEndedListeners.Add(dlg);

  public void RemoveTouchScrollEndedListener(UIBScrollable.OnTouchScrollEnded dlg) => this.m_TouchScrollEndedListeners.Remove(dlg);

  public void Pause(bool pause) => this.m_Pause = pause;

  public void PauseUpdateScrollHeight(bool pause) => this.m_PauseUpdateScrollHeight = pause;

  public void Enable(bool enable)
  {
    if (this.m_Enabled == enable)
      return;
    this.m_Enabled = enable;
    if ((UnityEngine.Object) this.m_scrollTrackCover != (UnityEngine.Object) null)
      this.m_scrollTrackCover.SetActive(!enable);
    this.RefreshShowThumb();
    if (this.m_Enabled)
      this.ResetTouchDrag();
    this.FireEnableScrollEvent();
  }

  public bool IsEnabled() => this.m_Enabled;

  public bool IsEnabledAndScrollable() => this.m_Enabled && this.IsScrollNeeded();

  public float GetScroll() => this.m_ScrollValue;

  public void SaveScroll(string savedName) => UIBScrollable.s_SavedScrollValues[savedName] = this.m_ScrollValue;

  public void LoadScroll(string savedName, bool snap)
  {
    float percentage = 0.0f;
    if (!UIBScrollable.s_SavedScrollValues.TryGetValue(savedName, out percentage))
      return;
    if (snap)
      this.SetScrollSnap(percentage);
    else
      this.SetScroll(percentage);
    this.ResetTouchDrag();
  }

  public bool EnableIfNeeded()
  {
    bool enable = this.IsScrollNeeded();
    this.Enable(enable);
    return enable;
  }

  public bool IsScrollNeeded() => (double) this.GetTotalWorldScrollHeight() > 0.0;

  public float PollScrollHeight()
  {
    switch (this.m_HeightMode)
    {
      case UIBScrollable.HeightMode.UseHeightCallback:
        return this.m_ScrollHeightCallback == null ? this.m_PolledScrollHeight : this.m_ScrollHeightCallback();
      case UIBScrollable.HeightMode.UseScrollableItem:
        return this.GetScrollableItemsHeight();
      default:
        return 0.0f;
    }
  }

  public float GetPolledScrollHeight() => this.m_PolledScrollHeight;

  public void SetScroll(float percentage, bool blockInputWhileScrolling = false, bool clamp = true) => this.SetScroll(percentage, (UIBScrollable.OnScrollComplete) null, blockInputWhileScrolling, clamp);

  public void SetScroll(
    float percentage,
    iTween.EaseType tweenType,
    float tweenTime,
    bool blockInputWhileScrolling = false,
    bool clamp = true)
  {
    this.SetScroll(percentage, (UIBScrollable.OnScrollComplete) null, tweenType, tweenTime, blockInputWhileScrolling, clamp);
  }

  public void SetScrollSnap(float percentage, bool clamp = true) => this.SetScrollSnap(percentage, (UIBScrollable.OnScrollComplete) null, clamp);

  public void SetScroll(
    float percentage,
    UIBScrollable.OnScrollComplete scrollComplete,
    bool blockInputWhileScrolling = false,
    bool clamp = true)
  {
    this.StartCoroutine(this.SetScrollWait(percentage, scrollComplete, blockInputWhileScrolling, true, new iTween.EaseType?(), new float?(), clamp));
  }

  public void SetScroll(
    float percentage,
    UIBScrollable.OnScrollComplete scrollComplete,
    iTween.EaseType tweenType,
    float tweenTime,
    bool blockInputWhileScrolling = false,
    bool clamp = true)
  {
    this.StartCoroutine(this.SetScrollWait(percentage, scrollComplete, blockInputWhileScrolling, true, new iTween.EaseType?(tweenType), new float?(tweenTime), clamp));
  }

  public void SetScrollSnap(
    float percentage,
    UIBScrollable.OnScrollComplete scrollComplete,
    bool clamp = true)
  {
    this.m_PolledScrollHeight = this.PollScrollHeight();
    this.m_LastScrollHeightRecorded = this.m_PolledScrollHeight;
    this.ScrollTo(percentage, scrollComplete, false, false, new iTween.EaseType?(), new float?(), clamp);
    this.ResetTouchDrag();
  }

  public void StopScroll()
  {
    Vector3 scrollAreaStartPos = this.m_ScrollAreaStartPos;
    Vector3 vector3_1 = scrollAreaStartPos + this.GetTotalScrollHeightVector(true) * (this.m_ScrollDirectionReverse ? -1f : 1f);
    Vector3 localPosition = this.m_ScrollObject.transform.localPosition;
    double num;
    if ((double) (vector3_1 - scrollAreaStartPos).magnitude <= 1.40129846432482E-45)
    {
      num = 0.0;
    }
    else
    {
      Vector3 vector3_2 = localPosition - scrollAreaStartPos;
      double magnitude1 = (double) vector3_2.magnitude;
      vector3_2 = vector3_1 - scrollAreaStartPos;
      double magnitude2 = (double) vector3_2.magnitude;
      num = magnitude1 / magnitude2;
    }
    float percentage = (float) num;
    iTween.Stop(this.m_ScrollObject);
    this.SetScrollImmediate(percentage);
  }

  public void SetScrollHeightCallback(
    UIBScrollable.ScrollHeightCallback dlg,
    bool refresh = false,
    bool resetScroll = false)
  {
    float? setResetScroll = new float?();
    if (resetScroll)
      setResetScroll = new float?(0.0f);
    this.SetScrollHeightCallback(dlg, setResetScroll, refresh);
  }

  public void SetScrollHeightCallback(
    UIBScrollable.ScrollHeightCallback dlg,
    float? setResetScroll,
    bool refresh = false)
  {
    this.m_VisibleAffectedObjects.Clear();
    this.m_ScrollHeightCallback = dlg;
    if (setResetScroll.HasValue)
    {
      this.m_ScrollValue = setResetScroll.Value;
      this.ResetTouchDrag();
    }
    if (refresh)
    {
      this.UpdateScroll();
      this.UpdateThumbPosition();
      this.UpdateScrollObjectPosition(true, (UIBScrollable.OnScrollComplete) null, new iTween.EaseType?(), new float?());
    }
    this.m_PolledScrollHeight = this.PollScrollHeight();
    this.m_LastScrollHeightRecorded = this.m_PolledScrollHeight;
  }

  public void SetHeight(float height)
  {
    this.m_ScrollHeightCallback = (UIBScrollable.ScrollHeightCallback) null;
    this.m_PolledScrollHeight = height;
    this.UpdateHeight();
  }

  public void UpdateScroll()
  {
    if (this.m_PauseUpdateScrollHeight)
      return;
    this.m_PolledScrollHeight = this.PollScrollHeight();
    this.UpdateHeight();
  }

  public void CenterWorldPosition(Vector3 position)
  {
    float percentage = (float) ((double) this.m_ScrollObject.transform.InverseTransformPoint(position)[(int) this.m_ScrollPlane] / -((double) this.m_PolledScrollHeight + (double) this.m_ScrollBottomPadding) * 2.0 - 0.5);
    this.StartCoroutine(this.BlockInput(this.m_ScrollTweenTime));
    this.SetScroll(percentage);
  }

  public bool IsObjectVisibleInScrollArea(GameObject obj, Vector3 extents, bool fullyVisible = false)
  {
    int scrollPlane = (int) this.m_ScrollPlane;
    float num1 = obj.transform.position[scrollPlane] - extents[scrollPlane];
    float num2 = obj.transform.position[scrollPlane] + extents[scrollPlane];
    Bounds bounds = this.m_ScrollBounds.bounds;
    float num3 = bounds.min[scrollPlane] - this.m_VisibleObjectThreshold;
    float num4 = bounds.max[scrollPlane] + this.m_VisibleObjectThreshold;
    bool flag1 = (double) num1 >= (double) num3 && (double) num1 <= (double) num4;
    bool flag2 = (double) num2 >= (double) num3 && (double) num2 <= (double) num4;
    bool flag3 = (double) num1 <= (double) num3;
    bool flag4 = (double) num2 >= (double) num4;
    if (fullyVisible)
      return flag1 & flag2;
    return flag1 | flag2 || flag3 & flag4;
  }

  private void GetFastVisibleRange(
    UIBScrollable.FastVisibleAffectedObject item,
    out int startIndex,
    out int endIndex,
    out bool isVisible)
  {
    int scrollPlane = (int) this.m_ScrollPlane;
    Bounds bounds = this.m_ScrollBounds.bounds;
    Vector3 vector3 = bounds.size;
    double num1 = (double) vector3[scrollPlane];
    vector3 = bounds.max;
    float num2 = vector3[scrollPlane];
    vector3 = item.TopObj.transform.position;
    float num3 = vector3[scrollPlane] - item.Extents[scrollPlane] - num2;
    float num4 = item.Extents[scrollPlane] * 2f;
    double num5 = (double) num4;
    int num6 = Mathf.CeilToInt((float) (num1 / num5));
    int num7 = Mathf.CeilToInt((float) num6 * item.Buffer);
    int num8 = num7 * 2;
    startIndex = Mathf.FloorToInt(num3 / num4) - num7 + 1;
    endIndex = startIndex + num6 + num8;
    isVisible = endIndex > 0;
  }

  public bool CenterObjectInView(
    GameObject obj,
    float positionOffset,
    UIBScrollable.OnScrollComplete scrollComplete,
    iTween.EaseType tweenType,
    float tweenTime,
    bool blockInputWhileScrolling = false)
  {
    float z = this.m_ScrollBounds.bounds.extents.z;
    return this.ScrollObjectIntoView(obj, positionOffset, z, scrollComplete, tweenType, tweenTime, blockInputWhileScrolling);
  }

  public bool ScrollObjectIntoView(
    GameObject obj,
    float positionOffset,
    float axisExtent,
    UIBScrollable.OnScrollComplete scrollComplete,
    iTween.EaseType tweenType,
    float tweenTime,
    bool blockInputWhileScrolling = false)
  {
    int scrollPlane = (int) this.m_ScrollPlane;
    float num1 = obj.transform.position[scrollPlane] + positionOffset - axisExtent;
    float num2 = obj.transform.position[scrollPlane] + positionOffset + axisExtent;
    Bounds bounds = this.m_ScrollBounds.bounds;
    float num3 = bounds.min[scrollPlane] - this.m_VisibleObjectThreshold;
    float num4 = bounds.max[scrollPlane] + this.m_VisibleObjectThreshold;
    bool flag1 = (double) num1 >= (double) num3;
    bool flag2 = (double) num2 <= (double) num4;
    if (flag1 & flag2)
      return false;
    float percentage = 0.0f;
    if (!flag1)
    {
      float z = this.GetTotalScrollHeightVector().z;
      if ((double) z == 0.0)
        Debug.LogWarning((object) "UIBScrollable.ScrollObjectIntoView() - scrollHeight calculated as 0, cannot calculate scroll percentage!");
      else
        percentage = this.m_ScrollValue + Mathf.Abs(Math.Abs(num3 - num1) / z);
    }
    else if (!flag2)
    {
      float z = this.GetTotalScrollHeightVector().z;
      if ((double) z == 0.0)
        Debug.LogWarning((object) "UIBScrollable.ScrollObjectIntoView() - scrollHeight calculated as 0, cannot calculate scroll percentage!");
      else
        percentage = this.m_ScrollValue - Mathf.Abs(Math.Abs(num4 - num2) / z);
    }
    this.SetScroll(percentage, scrollComplete, tweenType, tweenTime, blockInputWhileScrolling);
    return true;
  }

  public bool IsDragging() => (UnityEngine.Object) this.m_ScrollThumb != (UnityEngine.Object) null && this.m_ScrollThumb.IsDragging() || this.m_TouchBeginScreenPos.HasValue;

  public bool IsTouchDragging()
  {
    if (!this.m_TouchBeginScreenPos.HasValue || (double) this.m_PolledScrollHeight == 0.0)
    {
      this.m_TouchBeginScreenPos = new Vector2?();
      return false;
    }
    float num = Mathf.Abs(InputCollection.GetMousePosition().y - this.m_TouchBeginScreenPos.Value.y);
    return (double) this.m_ScrollDragThreshold * ((double) Screen.dpi > 0.0 ? (double) Screen.dpi : 96.0) <= (double) num;
  }

  public void SetScrollImmediate(float percentage)
  {
    this.ScrollTo(percentage, (UIBScrollable.OnScrollComplete) null, false, false, new iTween.EaseType?(), new float?(0.0f), true);
    this.ResetTouchDrag();
  }

  public void SetScrollImmediate(
    float percentage,
    UIBScrollable.OnScrollComplete scrollComplete,
    bool blockInputWhileScrolling,
    bool tween,
    iTween.EaseType? tweenType,
    float? tweenTime,
    bool clamp)
  {
    this.ScrollTo(percentage, scrollComplete, blockInputWhileScrolling, tween, tweenType, tweenTime, clamp);
    this.ResetTouchDrag();
  }

  public void SetHideThumb(bool value)
  {
    this.m_overrideHideThumb = value;
    this.RefreshShowThumb();
  }

  private void RefreshShowThumb()
  {
    if (!((UnityEngine.Object) this.m_ScrollThumb != (UnityEngine.Object) null))
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_ScrollThumb.gameObject.SetActive(false);
    }
    else
    {
      bool flag = (this.m_Enabled || !this.m_HideThumbWhenDisabled) && !this.m_overrideHideThumb;
      if (flag == this.m_ScrollThumb.gameObject.activeSelf)
        return;
      this.m_ScrollThumb.gameObject.SetActive(flag);
      if (!flag)
        return;
      this.UpdateThumbPosition();
    }
  }

  private void StartDragging()
  {
    if (this.m_InputBlocked || this.m_Pause || !this.m_Enabled)
      return;
    this.m_ScrollThumb.StartDragging();
  }

  private void UpdateHeight()
  {
    if ((double) Mathf.Abs(this.m_PolledScrollHeight - this.m_LastScrollHeightRecorded) > 1.0 / 1000.0)
    {
      if (!this.EnableIfNeeded())
        this.m_ScrollValue = 0.0f;
      this.UpdateThumbPosition();
      this.UpdateScrollObjectPosition(false, (UIBScrollable.OnScrollComplete) null, new iTween.EaseType?(), new float?());
      this.ResetTouchDrag();
    }
    this.m_LastScrollHeightRecorded = this.m_PolledScrollHeight;
  }

  private void DragContent()
  {
    if (InputCollection.GetMouseButtonDown(0))
    {
      if (this.GetWorldTouchPosition().HasValue)
      {
        this.m_TouchBeginScreenPos = new Vector2?((Vector2) InputCollection.GetMousePosition());
        return;
      }
    }
    else if (InputCollection.GetMouseButtonUp(0))
    {
      this.m_TouchBeginScreenPos = new Vector2?();
      this.m_TouchDragBeginWorldPos = new Vector3?();
      this.FireTouchEndEvent();
    }
    if (this.m_TouchDragBeginWorldPos.HasValue)
    {
      Vector3? positionOnDragArea = this.GetWorldTouchPositionOnDragArea();
      if (!positionOnDragArea.HasValue)
        return;
      int scrollPlane = (int) this.m_ScrollPlane;
      this.m_LastTouchScrollValue = this.m_ScrollValue;
      float num1 = this.m_TouchDragBeginScrollValue + this.GetScrollValueDelta(positionOnDragArea.Value[scrollPlane] - this.m_TouchDragBeginWorldPos.Value[scrollPlane]);
      float outOfBoundsDist = this.GetOutOfBoundsDist(num1);
      if ((double) outOfBoundsDist != 0.0)
      {
        float num2 = Mathf.Log10(Mathf.Abs(outOfBoundsDist) + 1f) * Mathf.Sign(outOfBoundsDist);
        num1 = (double) num2 < 0.0 ? num2 : num2 + 1f;
      }
      this.ScrollTo(Mathf.Lerp(this.m_prevScrollValue, num1, 0.9f), (UIBScrollable.OnScrollComplete) null, false, false, new iTween.EaseType?(), new float?(), false);
    }
    else if (this.m_TouchBeginScreenPos.HasValue)
    {
      float num3 = Mathf.Abs(InputCollection.GetMousePosition().x - this.m_TouchBeginScreenPos.Value.x);
      float num4 = Mathf.Abs(InputCollection.GetMousePosition().y - this.m_TouchBeginScreenPos.Value.y);
      int num5 = (double) num3 > (double) this.m_DeckTileDragThreshold * ((double) Screen.dpi > 0.0 ? (double) Screen.dpi : 96.0) ? 1 : 0;
      bool flag = (double) num4 > (double) this.m_ScrollDragThreshold * ((double) Screen.dpi > 0.0 ? (double) Screen.dpi : 96.0);
      if (num5 != 0 && ((double) num3 >= (double) num4 || !flag))
      {
        this.m_TouchBeginScreenPos = new Vector2?();
      }
      else
      {
        if (!flag)
          return;
        this.m_TouchDragBeginWorldPos = this.GetWorldTouchPositionOnDragArea();
        this.m_TouchDragBeginScrollValue = this.m_ScrollValue;
        this.m_LastTouchScrollValue = this.m_ScrollValue;
        this.FireTouchStartEvent();
      }
    }
    else
    {
      float f1 = (this.m_ScrollValue - this.m_LastTouchScrollValue) / Time.fixedDeltaTime;
      float outOfBoundsDist = this.GetOutOfBoundsDist(this.m_ScrollValue);
      if ((double) outOfBoundsDist != 0.0)
      {
        if ((double) Mathf.Abs(outOfBoundsDist) >= (double) this.m_MinOutOfBoundsScrollValue)
        {
          float num6 = (float) (-(double) this.m_ScrollBoundsSpringK * (double) outOfBoundsDist - (double) Mathf.Sqrt(4f * this.m_ScrollBoundsSpringK) * (double) f1);
          float num7 = f1 + num6 * Time.fixedDeltaTime;
          this.m_LastTouchScrollValue = this.m_ScrollValue;
          this.ScrollTo(this.m_ScrollValue + num7 * Time.fixedDeltaTime, (UIBScrollable.OnScrollComplete) null, false, false, new iTween.EaseType?(), new float?(), false);
        }
        if ((double) Mathf.Abs(this.GetOutOfBoundsDist(this.m_ScrollValue)) >= (double) this.m_MinOutOfBoundsScrollValue)
          return;
        this.ScrollTo(Mathf.Round(this.m_ScrollValue), (UIBScrollable.OnScrollComplete) null, false, false, new iTween.EaseType?(), new float?(), false);
        this.m_LastTouchScrollValue = this.m_ScrollValue;
      }
      else
      {
        if ((double) this.m_LastTouchScrollValue == (double) this.m_ScrollValue)
          return;
        float num = Mathf.Sign(f1);
        float f2 = f1 - num * this.m_KineticScrollFriction * Time.fixedDeltaTime;
        this.m_LastTouchScrollValue = this.m_ScrollValue;
        if ((double) Mathf.Abs(f2) < (double) this.m_MinKineticScrollSpeed || (double) Mathf.Sign(f2) != (double) num)
          return;
        this.ScrollTo(this.m_ScrollValue + f2 * Time.fixedDeltaTime, (UIBScrollable.OnScrollComplete) null, false, false, new iTween.EaseType?(), new float?(), false);
      }
    }
  }

  private void DragThumb()
  {
    Vector3 min = this.m_ScrollTrack.bounds.min;
    if ((UnityEngine.Object) this.m_scrollTrackCamera == (UnityEngine.Object) null)
    {
      this.m_scrollTrackCamera = CameraUtils.FindProjectionCameraForObject(this.m_ScrollTrack.gameObject);
      this.m_scrollTrackCameraOverridePass = this.GetCameraPassInParentHierarchy();
    }
    Plane plane = new Plane(-this.m_scrollTrackCamera.transform.forward, min);
    Ray rayWithCameraPass = CameraUtils.ScreenPointToRayWithCameraPass(this.m_scrollTrackCamera, (Vector2) InputCollection.GetMousePosition(), this.m_scrollTrackCameraOverridePass);
    float enter;
    if (plane.Raycast(rayWithCameraPass, out enter))
    {
      Vector3 point = rayWithCameraPass.GetPoint(enter);
      float scrollTrackTop1D = this.GetScrollTrackTop1D();
      float scrollTrackBtm1D = this.GetScrollTrackBtm1D();
      float num = Mathf.Clamp01((float) (((double) point[(int) this.m_ScrollPlane] - (double) scrollTrackTop1D) / ((double) scrollTrackBtm1D - (double) scrollTrackTop1D)));
      if ((double) Mathf.Abs(this.m_ScrollValue - num) > (double) Mathf.Epsilon)
      {
        this.m_ScrollValue = num;
        this.UpdateThumbPosition();
        this.UpdateScrollObjectPosition(false, (UIBScrollable.OnScrollComplete) null, new iTween.EaseType?(), new float?());
      }
    }
    this.ResetTouchDrag();
  }

  private CameraOverridePass GetCameraPassInParentHierarchy()
  {
    for (Transform transform = this.transform; (UnityEngine.Object) transform != (UnityEngine.Object) null; transform = transform.parent)
    {
      PopupRoot component = transform.GetComponent<PopupRoot>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.PrimaryRenderPass != null)
        return component.PrimaryRenderPass;
    }
    return (CameraOverridePass) null;
  }

  private void ResetTouchDrag()
  {
    int num = this.m_TouchDragBeginWorldPos.HasValue ? 1 : 0;
    this.m_TouchBeginScreenPos = new Vector2?();
    this.m_TouchDragBeginWorldPos = new Vector3?();
    this.m_TouchDragBeginScrollValue = this.m_ScrollValue;
    this.m_LastTouchScrollValue = this.m_ScrollValue;
    if (num == 0)
      return;
    this.FireTouchEndEvent();
  }

  private float GetScrollTrackTop1D() => this.GetScrollTrackTop()[(int) this.m_ScrollPlane];

  private float GetScrollTrackBtm1D() => this.GetScrollTrackBtm()[(int) this.m_ScrollPlane];

  private Vector3 GetScrollTrackTop()
  {
    if ((UnityEngine.Object) this.m_ScrollTrack == (UnityEngine.Object) null)
      return Vector3.zero;
    return this.m_ScrollPlane == UIBScrollable.ScrollDirection.X ? this.m_ScrollTrack.bounds.min : this.m_ScrollTrack.bounds.max;
  }

  private Vector3 GetScrollTrackBtm()
  {
    if ((UnityEngine.Object) this.m_ScrollTrack == (UnityEngine.Object) null)
      return Vector3.zero;
    return this.m_ScrollPlane == UIBScrollable.ScrollDirection.X ? this.m_ScrollTrack.bounds.max : this.m_ScrollTrack.bounds.min;
  }

  private void AddScroll(float amount)
  {
    this.ScrollTo(this.m_ScrollValue + amount, (UIBScrollable.OnScrollComplete) null, false, true, new iTween.EaseType?(), new float?(), true);
    this.ResetTouchDrag();
  }

  private void ScrollTo(
    float percentage,
    UIBScrollable.OnScrollComplete scrollComplete,
    bool blockInputWhileScrolling,
    bool tween,
    iTween.EaseType? tweenType,
    float? tweenTime,
    bool clamp)
  {
    this.m_ScrollValue = clamp ? Mathf.Clamp01(percentage) : percentage;
    this.UpdateThumbPosition();
    this.UpdateScrollObjectPosition(tween, scrollComplete, tweenType, tweenTime, blockInputWhileScrolling);
    this.m_prevScrollValue = percentage;
  }

  private void UpdateThumbPosition()
  {
    if ((UnityEngine.Object) this.m_ScrollThumb == (UnityEngine.Object) null)
      return;
    Vector3 scrollTrackTop = this.GetScrollTrackTop();
    Vector3 scrollTrackBtm = this.GetScrollTrackBtm();
    float num1 = scrollTrackTop[(int) this.m_ScrollPlane];
    float num2 = scrollTrackBtm[(int) this.m_ScrollPlane];
    Vector3 vector3 = scrollTrackTop + (scrollTrackBtm - scrollTrackTop) * 0.5f;
    vector3[(int) this.m_ScrollPlane] = num1 + (num2 - num1) * Mathf.Clamp01(this.m_ScrollValue);
    this.m_ScrollThumb.transform.position = vector3;
    if (this.m_ScrollPlane != UIBScrollable.ScrollDirection.Z)
      return;
    Vector3 localPosition = this.m_ScrollThumb.transform.localPosition;
    this.m_ScrollThumb.transform.localPosition = new Vector3(localPosition.x, this.m_ScrollThumbStartYPos, localPosition.z);
  }

  private void UpdateScrollObjectPosition(
    bool tween,
    UIBScrollable.OnScrollComplete scrollComplete,
    iTween.EaseType? tweenType,
    float? tweenTime,
    bool blockInputWhileScrolling = false)
  {
    if ((UnityEngine.Object) this.m_ScrollObject == (UnityEngine.Object) null)
      return;
    Vector3 scrollAreaStartPos = this.m_ScrollAreaStartPos;
    Vector3 vector3_1 = scrollAreaStartPos + this.GetTotalScrollHeightVector(true) * (this.m_ScrollDirectionReverse ? -1f : 1f);
    Vector3 vector3_2 = scrollAreaStartPos + this.m_ScrollValue * (vector3_1 - scrollAreaStartPos);
    if (float.IsNaN(vector3_2.x) || float.IsNaN(vector3_2.y) || float.IsNaN(vector3_2.z))
      return;
    if (tween)
    {
      iTween.MoveTo(this.m_ScrollObject, iTween.Hash((object) "position", (object) vector3_2, (object) "time", (object) (float) (tweenTime.HasValue ? (double) tweenTime.Value : (double) this.m_ScrollTweenTime), (object) "isLocal", (object) true, (object) "easetype", (object) (iTween.EaseType) (tweenType.HasValue ? (int) tweenType.Value : (int) this.m_ScrollEaseType), (object) "onupdate", (object) (Action<object>) (newVal => this.UpdateAndFireVisibleAffectedObjects()), (object) "oncomplete", (object) (Action<object>) (newVal =>
      {
        this.UpdateAndFireVisibleAffectedObjects();
        if (scrollComplete == null)
          return;
        scrollComplete(this.m_ScrollValue);
      })));
    }
    else
    {
      this.m_ScrollObject.transform.localPosition = this.m_ScrollPlane != UIBScrollable.ScrollDirection.Z ? vector3_2 : new Vector3(vector3_2.x, this.m_ScrollObject.transform.localPosition.y, vector3_2.z);
      this.UpdateAndFireVisibleAffectedObjects();
      if (scrollComplete == null)
        return;
      scrollComplete(this.m_ScrollValue);
    }
  }

  private IEnumerator SetScrollWait(
    float percentage,
    UIBScrollable.OnScrollComplete scrollComplete,
    bool blockInputWhileScrolling,
    bool tween,
    iTween.EaseType? tweenType,
    float? tweenTime,
    bool clamp)
  {
    yield return (object) null;
    this.ScrollTo(percentage, scrollComplete, blockInputWhileScrolling, tween, tweenType, tweenTime, clamp);
    this.ResetTouchDrag();
  }

  private IEnumerator BlockInput(float blockTime)
  {
    this.m_InputBlocked = true;
    yield return (object) new WaitForSeconds(blockTime);
    this.m_InputBlocked = false;
  }

  private Vector3 GetTotalScrollHeightVector(bool convertToLocalSpace = false)
  {
    if ((UnityEngine.Object) this.m_ScrollObject == (UnityEngine.Object) null)
      return Vector3.zero;
    float num = this.m_PolledScrollHeight - this.GetScrollBoundsHeight();
    if ((double) num < 0.0)
      return Vector3.zero;
    Vector3 scrollHeightVector = Vector3.zero;
    scrollHeightVector[(int) this.m_ScrollPlane] = num;
    if (convertToLocalSpace)
      scrollHeightVector = (Vector3) (this.m_ScrollObject.transform.parent.worldToLocalMatrix * (Vector4) scrollHeightVector);
    if ((double) this.m_ScrollBottomPadding > 0.0)
      scrollHeightVector += scrollHeightVector.normalized * this.m_ScrollBottomPadding;
    return scrollHeightVector;
  }

  private float GetTotalWorldScrollHeight() => this.GetTotalScrollHeightVector().magnitude;

  private Vector3? GetWorldTouchPosition() => this.GetWorldTouchPosition(this.m_ScrollBounds);

  private Vector3? GetWorldTouchPositionOnDragArea()
  {
    Vector3? positionOnDragArea = new Vector3?();
    if ((UnityEngine.Object) this.m_TouchDragFullArea != (UnityEngine.Object) null)
      positionOnDragArea = this.GetWorldTouchPosition(this.m_TouchDragFullArea);
    if (!positionOnDragArea.HasValue && (UnityEngine.Object) this.m_ScrollBounds != (UnityEngine.Object) null)
      positionOnDragArea = this.GetWorldTouchPosition(this.m_ScrollBounds);
    return positionOnDragArea;
  }

  private Vector3? GetWorldTouchPosition(BoxCollider bounds)
  {
    Camera scrollCamera = this.GetScrollCamera();
    if ((UnityEngine.Object) scrollCamera == (UnityEngine.Object) null)
      return new Vector3?();
    Ray ray = scrollCamera.ScreenPointToRay(InputCollection.GetMousePosition());
    RaycastHit hitInfo;
    foreach (Collider touchScrollBlocker in this.m_TouchScrollBlockers)
    {
      if (touchScrollBlocker.Raycast(ray, out hitInfo, float.MaxValue))
        return new Vector3?();
    }
    return this.IsInputOverScrollableArea(bounds, out hitInfo) ? new Vector3?(ray.GetPoint(hitInfo.distance)) : new Vector3?();
  }

  private float GetScrollValueDelta(float worldDelta) => this.m_ScrollDeltaMultiplier * worldDelta / this.GetTotalWorldScrollHeight();

  private float GetOutOfBoundsDist(float scrollValue)
  {
    if ((double) scrollValue < 0.0)
      return scrollValue;
    return (double) scrollValue > 1.0 ? scrollValue - 1f : 0.0f;
  }

  private void FireEnableScrollEvent()
  {
    foreach (UIBScrollable.EnableScroll enableScroll in this.m_EnableScrollListeners.ToArray())
      enableScroll(this.m_Enabled);
  }

  public void UpdateAndFireVisibleAffectedObjects()
  {
    foreach (UIBScrollable.VisibleAffectedObject visibleAffectedObject in this.m_VisibleAffectedObjects)
    {
      bool visible = this.IsObjectVisibleInScrollArea(visibleAffectedObject.Obj, visibleAffectedObject.Extents) || this.m_ForceShowVisibleAffectedObjects;
      if (visible != visibleAffectedObject.Visible)
      {
        visibleAffectedObject.Visible = visible;
        visibleAffectedObject.Callback(visibleAffectedObject.Obj, visible);
      }
    }
    foreach (UIBScrollable.FastVisibleAffectedObject visibleAffectedObject in this.m_fastVisibleAffectedObjects)
    {
      if (!((UnityEngine.Object) visibleAffectedObject.TopObj == (UnityEngine.Object) null))
        this.FireFastVisibleCallback(visibleAffectedObject);
    }
  }

  private void FireFastVisibleCallback(UIBScrollable.FastVisibleAffectedObject item)
  {
    int startIndex;
    int endIndex;
    bool isVisible;
    this.GetFastVisibleRange(item, out startIndex, out endIndex, out isVisible);
    if (item.Callback == null)
      return;
    item.Callback(startIndex, endIndex, isVisible);
  }

  public void UpdateVisibilityOnObject(GameObject topObj)
  {
    if ((UnityEngine.Object) topObj == (UnityEngine.Object) null)
      return;
    foreach (UIBScrollable.FastVisibleAffectedObject visibleAffectedObject in this.m_fastVisibleAffectedObjects)
    {
      if ((UnityEngine.Object) visibleAffectedObject.TopObj == (UnityEngine.Object) topObj)
        this.FireFastVisibleCallback(visibleAffectedObject);
    }
  }

  private float GetScrollBoundsHeight() => (UnityEngine.Object) this.m_ScrollBounds == (UnityEngine.Object) null ? 0.0f : this.m_ScrollBounds.bounds.size[(int) this.m_ScrollPlane];

  private void FireTouchStartEvent()
  {
    foreach (UIBScrollable.OnTouchScrollStarted touchScrollStarted in this.m_TouchScrollStartedListeners.ToArray())
      touchScrollStarted();
  }

  private void FireTouchEndEvent()
  {
    foreach (UIBScrollable.OnTouchScrollEnded touchScrollEnded in this.m_TouchScrollEndedListeners.ToArray())
      touchScrollEnded();
  }

  private float GetScrollableItemsHeight()
  {
    Vector3 zero1 = Vector3.zero;
    Vector3 zero2 = Vector3.zero;
    if (this.GetScrollableItemsMinMax(ref zero1, ref zero2).Count == 0)
      return 0.0f;
    int scrollPlane = (int) this.m_ScrollPlane;
    return zero2[scrollPlane] - zero1[scrollPlane];
  }

  private List<UIBScrollableItem> GetScrollableItemsMinMax(
    ref Vector3 min,
    ref Vector3 max)
  {
    if ((UnityEngine.Object) this.m_ScrollObject == (UnityEngine.Object) null)
      return this.m_scrollableItems;
    int count = this.m_scrollableItems.Count;
    if (count == 0)
      return this.m_scrollableItems;
    min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    for (int index = 0; index < count; ++index)
    {
      UIBScrollableItem scrollableItem = this.m_scrollableItems[index];
      if (scrollableItem.IsActive())
      {
        Vector3 min1;
        Vector3 max1;
        scrollableItem.GetWorldBounds(out min1, out max1);
        min.x = Math.Min(min.x, Math.Min(min1.x, max1.x));
        min.y = Math.Min(min.y, Math.Min(min1.y, max1.y));
        min.z = Math.Min(min.z, Math.Min(min1.z, max1.z));
        max.x = Math.Max(max.x, Math.Max(min1.x, max1.x));
        max.y = Math.Max(max.y, Math.Max(min1.y, max1.y));
        max.z = Math.Max(max.z, Math.Max(min1.z, max1.z));
      }
    }
    return this.m_scrollableItems;
  }

  private BoxCollider[] GetBoxCollidersMinMax(ref Vector3 min, ref Vector3 max) => (BoxCollider[]) null;

  private Camera GetScrollCamera()
  {
    if (this.m_UseCameraFromLayer)
      return CameraUtils.FindFirstByLayer(this.gameObject.layer);
    Box box = Box.Get();
    return (UnityEngine.Object) box == (UnityEngine.Object) null ? (Camera) null : box.GetCamera();
  }

  private void SaveScrollThumbStartHeight()
  {
    if (!((UnityEngine.Object) this.m_ScrollThumb != (UnityEngine.Object) null))
      return;
    this.m_ScrollThumbStartYPos = this.m_ScrollThumb.transform.localPosition.y;
  }

  public enum ScrollDirection
  {
    X,
    Y,
    Z,
  }

  public enum HeightMode
  {
    UseHeightCallback,
    UseScrollableItem,
    UseBoxCollider,
  }

  public enum ScrollWheelMode
  {
    ScaledToScrollSize,
    FixedRate,
  }

  public interface IContent
  {
    UIBScrollable Scrollable { get; }
  }

  private class ContentComponent : 
    MonoBehaviour,
    UIBScrollable.IContent,
    IScrollingVisibilityProvider
  {
    public UIBScrollable Scrollable { get; set; }

    public void AddFastVisibleAffectedObject(
      GameObject topObj,
      Vector3 extents,
      bool visible,
      float buffer,
      Action<int, int, bool> callback = null)
    {
      this.Scrollable.AddFastVisibleAffectedObject(topObj, extents, visible, buffer, callback);
    }

    public void RemoveFastVisibleAffectedObject(GameObject obj) => this.Scrollable.RemoveFastVisibleAffectedObject(obj);

    public void ChangeExtentsOnFastVisibleObject(GameObject topObj, Vector3 extents) => this.Scrollable.ChangeExtentsOnFastVisibleObject(topObj, extents);

    public void UpdateVisibility(GameObject topObj) => this.Scrollable.UpdateAndFireVisibleAffectedObjects();
  }

  public delegate void EnableScroll(bool enabled);

  public delegate float ScrollHeightCallback();

  public delegate void OnScrollComplete(float percentage);

  public delegate void OnTouchScrollStarted();

  public delegate void OnTouchScrollEnded();

  public delegate void VisibleAffected(GameObject obj, bool visible);

  public class VisibleAffectedObject
  {
    public GameObject Obj;
    public Vector3 Extents;
    public bool Visible;
    public UIBScrollable.VisibleAffected Callback;
  }

  protected class FastVisibleAffectedObject
  {
    public GameObject TopObj;
    public Vector3 Extents;
    public Action<int, int, bool> Callback;
    public float Buffer;
  }
}
