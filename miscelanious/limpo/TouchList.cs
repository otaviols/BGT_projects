using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof (BoxCollider))]
public class TouchList : PegUIElement
{
  public TouchList.Orientation orientation;
  public TouchList.Alignment alignment = TouchList.Alignment.Mid;
  public TouchList.LayoutPlane layoutPlane;
  public float elementSpacing;
  public Vector2 padding = Vector2.zero;
  public int breadth = 1;
  public float itemDragFinishDistance;
  public TiledBackground background;
  public float scrollWheelIncrement = 30f;
  public Float_MobileOverride maxKineticScrollSpeed = new Float_MobileOverride();
  private GameObject content;
  private List<ITouchListItem> renderedItems = new List<ITouchListItem>();
  private Map<ITouchListItem, TouchList.ItemInfo> itemInfos = new Map<ITouchListItem, TouchList.ItemInfo>();
  private int layoutDimension1;
  private int layoutDimension2;
  private int layoutDimension3;
  private float contentSize;
  private float excessContentSize;
  private float m_fullListContentSize;
  private Vector2? touchBeginScreenPosition;
  private Vector3? dragBeginOffsetFromContent;
  private Vector3 dragBeginContentPosition = Vector3.zero;
  private Vector3 lastTouchPosition = Vector3.zero;
  private float lastContentPosition;
  private ITouchListItem touchBeginItem;
  private bool m_isHoveredOverTouchList;
  private PegUIElement m_hoveredOverItem;
  private TouchList.ILongListBehavior longListBehavior;
  private bool allowModification = true;
  private Vector3? dragItemBegin;
  private bool layoutSuspended;
  private int? selection;
  private bool scrollEnabled = true;
  private const float ScrollDragThreshold = 0.05f;
  private const float ItemDragThreshold = 0.05f;
  private const float KineticScrollFriction = 10000f;
  private const float MinKineticScrollSpeed = 0.01f;
  private const float ScrollBoundsSpringK = 400f;
  private static readonly float ScrollBoundsSpringB = Mathf.Sqrt(1600f);
  private const float MinOutOfBoundsDistance = 0.05f;
  private static readonly Func<float, float> OutOfBoundsDistReducer = (Func<float, float>) (dist => (float) (30.0 * ((double) Mathf.Log(dist + 30f) - (double) Mathf.Log(30f))));
  private const float CLIPSIZE_EPSILON = 0.0001f;

  public event Action Scrolled;

  public event TouchList.SelectedIndexChangingEvent SelectedIndexChanging;

  public event TouchList.ScrollingEnabledChangedEvent ScrollingEnabledChanged;

  public event Action ClipSizeChanged;

  public event TouchList.ItemDragEvent ItemDragStarted;

  public event TouchList.ItemDragEvent ItemDragged;

  public event TouchList.ItemDragEvent ItemDragFinished;

  public IEnumerable<ITouchListItem> RenderedItems
  {
    get
    {
      this.EnforceInitialized();
      return (IEnumerable<ITouchListItem>) this.renderedItems;
    }
  }

  public bool IsReadOnly
  {
    get
    {
      this.EnforceInitialized();
      return false;
    }
  }

  public bool IsInitialized => (UnityEngine.Object) this.content != (UnityEngine.Object) null;

  public TouchList.ILongListBehavior LongListBehavior
  {
    get
    {
      this.EnforceInitialized();
      return this.longListBehavior;
    }
    set
    {
      this.EnforceInitialized();
      if (value == this.longListBehavior)
        return;
      this.allowModification = true;
      this.Clear();
      if (this.longListBehavior != null)
        this.longListBehavior.ReleaseAllItems();
      this.longListBehavior = value;
      if (this.longListBehavior == null)
        return;
      this.RefreshList(0, false);
      this.allowModification = false;
    }
  }

  public float ScrollValue
  {
    get
    {
      this.EnforceInitialized();
      float scrollableAmount = this.ScrollableAmount;
      float num = (double) scrollableAmount > 0.0 ? Mathf.Clamp01(-this.content.transform.localPosition[this.layoutDimension1] / scrollableAmount) : 0.0f;
      return (double) num == 0.0 || (double) num == 1.0 ? -this.GetOutOfBoundsDist(this.content.transform.localPosition[this.layoutDimension1]) / Mathf.Max(this.contentSize, this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)]) + num : num;
    }
    set
    {
      this.EnforceInitialized();
      if (this.dragBeginOffsetFromContent.HasValue || Mathf.Approximately(this.ScrollValue, value))
        return;
      float scrollableAmount = this.ScrollableAmount;
      Vector3 localPosition = this.content.transform.localPosition;
      localPosition[this.layoutDimension1] = -Mathf.Clamp01(value) * scrollableAmount;
      this.content.transform.localPosition = localPosition;
      float num = localPosition[this.layoutDimension1] - this.lastContentPosition;
      if ((double) num != 0.0)
        this.PreBufferLongListItems((double) num < 0.0);
      this.lastContentPosition = localPosition[this.layoutDimension1];
      this.FixUpScrolling();
      this.OnScrolled();
    }
  }

  private void FixUpScrolling()
  {
    if (this.longListBehavior == null || this.renderedItems.Count <= 0 || !this.CanScroll)
      return;
    Bounds localClipBounds = this.CalculateLocalClipBounds();
    TouchList.ItemInfo itemInfo1 = this.itemInfos[this.renderedItems[0]];
    if (itemInfo1.LongListIndex == 0 && !this.CanScrollBehind)
    {
      float num = localClipBounds.min[this.layoutDimension1];
      Vector3 min = itemInfo1.Min;
      if ((double) Mathf.Abs(min[this.layoutDimension1] - num) <= 9.99999974737875E-05)
        return;
      Vector3 zero = Vector3.zero;
      zero[this.layoutDimension1] = num - min[this.layoutDimension1];
      zero /= 4f;
      for (int index = 0; index < this.renderedItems.Count; ++index)
        this.renderedItems[index].gameObject.transform.Translate(zero);
    }
    else
    {
      if (this.renderedItems.Count <= 1 || this.CanScrollAhead)
        return;
      float num = localClipBounds.max[this.layoutDimension1];
      TouchList.ItemInfo itemInfo2 = this.itemInfos[this.renderedItems[this.renderedItems.Count - 1]];
      if (itemInfo2.LongListIndex < this.longListBehavior.AllItemsCount - 1)
        return;
      Vector3 max = itemInfo2.Max;
      if ((double) Mathf.Abs(max[this.layoutDimension1] - num) <= 9.99999974737875E-05)
        return;
      Vector3 zero = Vector3.zero;
      zero[this.layoutDimension1] = num - max[this.layoutDimension1];
      zero /= 4f;
      for (int index = 0; index < this.renderedItems.Count; ++index)
        this.renderedItems[index].gameObject.transform.Translate(zero);
    }
  }

  public float ScrollableAmount => this.longListBehavior == null ? this.excessContentSize : Mathf.Max(0.0f, this.m_fullListContentSize - this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)]);

  public bool CanScrollAhead
  {
    get
    {
      if (!this.scrollEnabled)
        return false;
      if ((double) this.ScrollValue < 1.0)
        return true;
      if (this.longListBehavior != null && this.renderedItems.Count > 0)
      {
        for (int allItemsIndex = this.itemInfos[this.renderedItems.Last<ITouchListItem>()].LongListIndex + 1; allItemsIndex < this.longListBehavior.AllItemsCount; ++allItemsIndex)
        {
          if (this.longListBehavior.IsItemShowable(allItemsIndex))
            return true;
        }
      }
      return false;
    }
  }

  public bool CanScrollBehind
  {
    get
    {
      if (!this.scrollEnabled)
        return false;
      if ((double) this.ScrollValue > 0.0)
        return true;
      if (this.longListBehavior != null && this.renderedItems.Count > 0)
      {
        TouchList.ItemInfo itemInfo = this.itemInfos[this.renderedItems.First<ITouchListItem>()];
        if (this.longListBehavior.AllItemsCount > 0)
        {
          for (int allItemsIndex = itemInfo.LongListIndex - 1; allItemsIndex >= 0; --allItemsIndex)
          {
            if (this.longListBehavior.IsItemShowable(allItemsIndex))
              return true;
          }
        }
      }
      return false;
    }
  }

  public bool CanScroll => this.CanScrollAhead || this.CanScrollBehind;

  public float ViewWindowMinValue
  {
    get => -this.content.transform.localPosition[this.layoutDimension1] / this.contentSize;
    set
    {
      Vector3 localPosition = this.content.transform.localPosition;
      localPosition[this.layoutDimension1] = -Mathf.Clamp01(value) * this.contentSize;
      this.content.transform.localPosition = localPosition;
      float num = this.content.transform.localPosition[this.layoutDimension1] - this.lastContentPosition;
      if ((double) num != 0.0)
        this.PreBufferLongListItems((double) num < 0.0);
      this.lastContentPosition = localPosition[this.layoutDimension1];
      this.OnScrolled();
    }
  }

  public float ViewWindowMaxValue
  {
    get => (-this.content.transform.localPosition[this.layoutDimension1] + this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)]) / this.contentSize;
    set
    {
      Vector3 localPosition = this.content.transform.localPosition;
      localPosition[this.layoutDimension1] = -Mathf.Clamp01(value) * this.contentSize + this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)];
      this.content.transform.localPosition = localPosition;
      float num = this.content.transform.localPosition[this.layoutDimension1] - this.lastContentPosition;
      if ((double) num != 0.0)
        this.PreBufferLongListItems((double) num < 0.0);
      this.lastContentPosition = localPosition[this.layoutDimension1];
      this.OnScrolled();
    }
  }

  public Vector2 ClipSize
  {
    get
    {
      this.EnforceInitialized();
      BoxCollider component = this.GetComponent<Collider>() as BoxCollider;
      return new Vector2(component.size.x, this.layoutPlane == TouchList.LayoutPlane.XY ? component.size.y : component.size.z);
    }
    set
    {
      this.EnforceInitialized();
      BoxCollider component = this.GetComponent<Collider>() as BoxCollider;
      Vector3 vector3_1 = new Vector3(value.x, 0.0f, 0.0f);
      vector3_1[1] = this.layoutPlane == TouchList.LayoutPlane.XY ? value.y : component.size.y;
      vector3_1[2] = this.layoutPlane == TouchList.LayoutPlane.XZ ? value.y : component.size.z;
      Vector3 vector3_2 = VectorUtils.Abs(component.size - vector3_1);
      if ((double) vector3_2.x <= 9.99999974737875E-05 && (double) vector3_2.y <= 9.99999974737875E-05 && (double) vector3_2.z <= 9.99999974737875E-05)
        return;
      component.size = vector3_1;
      this.UpdateBackgroundBounds();
      if (this.longListBehavior == null)
        this.RepositionItems(0);
      else
        this.RefreshList(0, true);
      if (this.ClipSizeChanged == null)
        return;
      this.ClipSizeChanged();
    }
  }

  public bool SelectionEnabled
  {
    get
    {
      this.EnforceInitialized();
      return this.selection.HasValue;
    }
    set
    {
      this.EnforceInitialized();
      if (value == this.SelectionEnabled)
        return;
      if (value)
        this.selection = new int?(-1);
      else
        this.selection = new int?();
    }
  }

  public int SelectedIndex
  {
    get
    {
      this.EnforceInitialized();
      return !this.selection.HasValue ? -1 : this.selection.Value;
    }
    set
    {
      this.EnforceInitialized();
      if (!this.SelectionEnabled)
        return;
      int num1 = value;
      int? selection = this.selection;
      int valueOrDefault = selection.GetValueOrDefault();
      if (num1 == valueOrDefault & selection.HasValue || this.SelectedIndexChanging != null && !this.SelectedIndexChanging(value))
        return;
      ISelectableTouchListItem selectedItem = this.SelectedItem as ISelectableTouchListItem;
      ISelectableTouchListItem selectableTouchListItem = (value != -1 ? this.renderedItems[value] : (ITouchListItem) null) as ISelectableTouchListItem;
      if (value == -1 || selectableTouchListItem != null && selectableTouchListItem.Selectable)
        this.selection = new int?(value);
      if (selectedItem != null)
      {
        selection = this.selection;
        int num2 = value;
        if (selection.GetValueOrDefault() == num2 & selection.HasValue)
          selectedItem.Unselected();
      }
      selection = this.selection;
      int num3 = value;
      if (!(selection.GetValueOrDefault() == num3 & selection.HasValue) || selectableTouchListItem == null)
        return;
      selectableTouchListItem.Selected();
      this.ScrollToItem_Internal((ITouchListItem) selectableTouchListItem);
    }
  }

  public ITouchListItem SelectedItem
  {
    get
    {
      this.EnforceInitialized();
      return !this.selection.HasValue || this.selection.Value == -1 ? (ITouchListItem) null : this.renderedItems[this.selection.Value];
    }
    set
    {
      this.EnforceInitialized();
      int num = this.renderedItems.IndexOf(value);
      if (num == -1)
        return;
      this.SelectedIndex = num;
    }
  }

  public void Add(ITouchListItem item) => this.Add(item, true);

  public void Add(ITouchListItem item, bool repositionItems)
  {
    this.EnforceInitialized();
    if (!this.allowModification)
      return;
    this.renderedItems.Add(item);
    Vector3 negatedScale = this.GetNegatedScale(item.transform.localScale);
    item.transform.parent = this.content.transform;
    item.transform.localPosition = Vector3.zero;
    item.transform.localRotation = Quaternion.identity;
    if (this.orientation == TouchList.Orientation.Vertical)
      item.transform.localScale = negatedScale;
    this.itemInfos[item] = new TouchList.ItemInfo(item, this.layoutPlane);
    item.gameObject.SetActive(false);
    int? selection = this.selection;
    int num = -1;
    if (selection.GetValueOrDefault() == num & selection.HasValue && item is ISelectableTouchListItem && ((ISelectableTouchListItem) item).IsSelected())
      this.selection = new int?(this.renderedItems.Count - 1);
    if (!repositionItems)
      return;
    this.RepositionItems(this.renderedItems.Count - 1);
    this.RecalculateLongListContentSize();
  }

  public void Clear()
  {
    this.EnforceInitialized();
    if (!this.allowModification)
      return;
    foreach (ITouchListItem renderedItem in this.renderedItems)
    {
      Vector3 negatedScale = this.GetNegatedScale(renderedItem.transform.localScale);
      renderedItem.transform.parent = (Transform) null;
      if (this.orientation == TouchList.Orientation.Vertical)
        renderedItem.transform.localScale = negatedScale;
    }
    this.content.transform.localPosition = Vector3.zero;
    this.lastContentPosition = 0.0f;
    this.renderedItems.Clear();
    this.RecalculateSize();
    this.UpdateBackgroundScroll();
    this.RecalculateLongListContentSize();
    if (this.SelectionEnabled)
      this.SelectedIndex = -1;
    if (!((UnityEngine.Object) this.m_hoveredOverItem != (UnityEngine.Object) null))
      return;
    this.m_hoveredOverItem.TriggerOut();
    this.m_hoveredOverItem = (PegUIElement) null;
  }

  public bool Contains(ITouchListItem item)
  {
    this.EnforceInitialized();
    return this.renderedItems.Contains(item);
  }

  public void CopyTo(ITouchListItem[] array, int arrayIndex)
  {
    this.EnforceInitialized();
    this.renderedItems.CopyTo(array, arrayIndex);
  }

  private List<ITouchListItem> GetItemsInView()
  {
    this.EnforceInitialized();
    List<ITouchListItem> itemsInView = new List<ITouchListItem>();
    float num = this.CalculateLocalClipBounds().max[this.layoutDimension1];
    for (int numItemsBehindView = this.GetNumItemsBehindView(); numItemsBehindView < this.renderedItems.Count && (double) (this.itemInfos[this.renderedItems[numItemsBehindView]].Min - this.content.transform.localPosition)[this.layoutDimension1] < (double) num; ++numItemsBehindView)
      itemsInView.Add(this.renderedItems[numItemsBehindView]);
    return itemsInView;
  }

  public void SetVisibilityOfAllItems()
  {
    if (this.layoutSuspended)
      return;
    this.EnforceInitialized();
    Bounds localClipBounds = this.CalculateLocalClipBounds();
    for (int index = 0; index < this.renderedItems.Count; ++index)
    {
      ITouchListItem renderedItem = this.renderedItems[index];
      bool flag = this.IsItemVisible_Internal(index, ref localClipBounds);
      if (flag != renderedItem.gameObject.activeSelf)
      {
        renderedItem.gameObject.SetActive(flag);
        if (!flag)
          renderedItem.OnScrollOutOfView();
      }
    }
  }

  private bool IsItemVisible_Internal(int visualizedListIndex, ref Bounds localClipBounds)
  {
    TouchList.ItemInfo itemInfo = this.itemInfos[this.renderedItems[visualizedListIndex]];
    return this.IsWithinClipBounds(itemInfo.Min, itemInfo.Max, ref localClipBounds);
  }

  private bool IsWithinClipBounds(
    Vector3 localBoundsMin,
    Vector3 localBoundsMax,
    ref Bounds localClipBounds)
  {
    float num1 = localClipBounds.min[this.layoutDimension1];
    float num2 = localClipBounds.max[this.layoutDimension1];
    return (double) localBoundsMax[this.layoutDimension1] >= (double) num1 && (double) localBoundsMin[this.layoutDimension1] <= (double) num2;
  }

  private bool IsItemVisible(int visualizedListIndex)
  {
    Bounds localClipBounds = this.CalculateLocalClipBounds();
    return this.IsItemVisible_Internal(visualizedListIndex, ref localClipBounds);
  }

  public int IndexOf(ITouchListItem item)
  {
    this.EnforceInitialized();
    return this.renderedItems.IndexOf(item);
  }

  public void Insert(int index, ITouchListItem item) => this.Insert(index, item, true);

  public void Insert(int index, ITouchListItem item, bool repositionItems)
  {
    this.EnforceInitialized();
    if (!this.allowModification)
      return;
    this.renderedItems.Insert(index, item);
    Vector3 negatedScale = this.GetNegatedScale(item.transform.localScale);
    item.transform.parent = this.content.transform;
    item.transform.localPosition = Vector3.zero;
    item.transform.localRotation = Quaternion.identity;
    if (this.orientation == TouchList.Orientation.Vertical)
      item.transform.localScale = negatedScale;
    this.itemInfos[item] = new TouchList.ItemInfo(item, this.layoutPlane);
    int? selection = this.selection;
    int num = -1;
    if (selection.GetValueOrDefault() == num & selection.HasValue && item is ISelectableTouchListItem && ((ISelectableTouchListItem) item).IsSelected())
      this.selection = new int?(index);
    if (!repositionItems)
      return;
    this.RepositionItems(index);
    this.RecalculateLongListContentSize();
  }

  public bool Remove(ITouchListItem item)
  {
    this.EnforceInitialized();
    if (!this.allowModification)
      return false;
    int index = this.renderedItems.IndexOf(item);
    if (index == -1)
      return false;
    this.RemoveAt(index, true);
    return true;
  }

  public void RemoveAt(int index) => this.RemoveAt(index, true);

  public void RemoveAt(int index, bool repositionItems)
  {
    this.EnforceInitialized();
    if (!this.allowModification)
      return;
    Vector3 negatedScale = this.GetNegatedScale(this.renderedItems[index].transform.localScale);
    ITouchListItem renderedItem = this.renderedItems[index];
    renderedItem.transform.parent = this.transform;
    if (this.orientation == TouchList.Orientation.Vertical)
      this.renderedItems[index].transform.localScale = negatedScale;
    this.itemInfos.Remove(this.renderedItems[index]);
    this.renderedItems.RemoveAt(index);
    int num1 = index;
    int? selection = this.selection;
    int valueOrDefault1 = selection.GetValueOrDefault();
    if (num1 == valueOrDefault1 & selection.HasValue)
    {
      this.selection = new int?(-1);
    }
    else
    {
      int num2 = index;
      selection = this.selection;
      int valueOrDefault2 = selection.GetValueOrDefault();
      if (num2 < valueOrDefault2 & selection.HasValue)
      {
        selection = this.selection;
        this.selection = selection.HasValue ? new int?(selection.GetValueOrDefault() - 1) : new int?();
      }
    }
    if ((UnityEngine.Object) this.m_hoveredOverItem != (UnityEngine.Object) null && (UnityEngine.Object) renderedItem.GetComponent<PegUIElement>() == (UnityEngine.Object) this.m_hoveredOverItem)
    {
      this.m_hoveredOverItem.TriggerOut();
      this.m_hoveredOverItem = (PegUIElement) null;
    }
    if (!repositionItems)
      return;
    this.RepositionItems(index);
    this.RecalculateLongListContentSize();
  }

  public int FindIndex(Predicate<ITouchListItem> match)
  {
    this.EnforceInitialized();
    return this.renderedItems.FindIndex(match);
  }

  public void Sort(Comparison<ITouchListItem> comparison)
  {
    this.EnforceInitialized();
    ITouchListItem selectedItem = this.SelectedItem;
    this.renderedItems.Sort(comparison);
    this.RepositionItems(0);
    this.selection = new int?(this.renderedItems.IndexOf(selectedItem));
  }

  public bool IsLayoutSuspended => this.layoutSuspended;

  public void SuspendLayout()
  {
    this.EnforceInitialized();
    this.layoutSuspended = true;
  }

  public void ResumeLayout(bool repositionItems = true)
  {
    this.EnforceInitialized();
    this.layoutSuspended = false;
    if (!repositionItems)
      return;
    this.RepositionItems(0);
  }

  public void ResetState()
  {
    this.touchBeginScreenPosition = new Vector2?();
    this.dragBeginOffsetFromContent = new Vector3?();
    this.dragBeginContentPosition = Vector3.zero;
    this.lastTouchPosition = Vector3.zero;
    this.lastContentPosition = 0.0f;
    this.dragItemBegin = new Vector3?();
    if (!((UnityEngine.Object) this.content != (UnityEngine.Object) null))
      return;
    this.content.transform.localPosition = Vector3.zero;
  }

  public void SetScrollingEnabled(bool enable)
  {
    this.scrollEnabled = enable;
    this.OnScrollingEnabledChanged();
  }

  public void ScrollToItem(ITouchListItem item) => this.ScrollToItem_Internal(item);

  protected override void Awake()
  {
    base.Awake();
    this.content = new GameObject("Content");
    this.content.transform.parent = this.transform;
    TransformUtil.Identity((Component) this.content.transform);
    this.layoutDimension1 = 0;
    this.layoutDimension2 = this.layoutPlane == TouchList.LayoutPlane.XY ? 1 : 2;
    this.layoutDimension3 = 3 - this.layoutDimension2;
    if (this.orientation == TouchList.Orientation.Vertical)
    {
      GeneralUtils.Swap<int>(ref this.layoutDimension1, ref this.layoutDimension2);
      Vector3 one = Vector3.one;
      one[this.layoutDimension1] = -1f;
      this.transform.localScale = one;
    }
    if (!((UnityEngine.Object) this.background != (UnityEngine.Object) null))
      return;
    if (this.orientation == TouchList.Orientation.Vertical)
      this.background.transform.localScale = this.GetNegatedScale(this.background.transform.localScale);
    this.UpdateBackgroundBounds();
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    this.m_isHoveredOverTouchList = true;
    this.OnHover(true);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    this.m_isHoveredOverTouchList = false;
    if (!((UnityEngine.Object) this.m_hoveredOverItem != (UnityEngine.Object) null))
      return;
    this.m_hoveredOverItem.TriggerOut();
    this.m_hoveredOverItem = (PegUIElement) null;
  }

  private void OnHover(bool isKnownOver)
  {
    if (UniversalInputManager.Get().IsTouchMode())
      return;
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null)
    {
      if (!((UnityEngine.Object) this.m_hoveredOverItem != (UnityEngine.Object) null))
        return;
      this.m_hoveredOverItem.TriggerOut();
      this.m_hoveredOverItem = (PegUIElement) null;
    }
    else
    {
      RaycastHit hitInfo;
      if (!isKnownOver && (!UniversalInputManager.Get().GetInputHitInfo(firstByLayer, out hitInfo) || (UnityEngine.Object) hitInfo.transform != (UnityEngine.Object) this.transform) && (UnityEngine.Object) this.m_hoveredOverItem != (UnityEngine.Object) null)
      {
        this.m_hoveredOverItem.TriggerOut();
        this.m_hoveredOverItem = (PegUIElement) null;
      }
      Collider component = this.GetComponent<Collider>();
      component.enabled = false;
      PegUIElement pegUiElement = (PegUIElement) null;
      if (UniversalInputManager.Get().GetInputHitInfo(firstByLayer, out hitInfo))
        pegUiElement = hitInfo.transform.GetComponent<PegUIElement>();
      component.enabled = true;
      if (!((UnityEngine.Object) pegUiElement != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_hoveredOverItem != (UnityEngine.Object) pegUiElement))
        return;
      if ((UnityEngine.Object) this.m_hoveredOverItem != (UnityEngine.Object) null)
        this.m_hoveredOverItem.TriggerOut();
      pegUiElement.TriggerOver();
      this.m_hoveredOverItem = pegUiElement;
    }
  }

  protected override void OnPress()
  {
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null)
      return;
    this.touchBeginScreenPosition = new Vector2?((Vector2) InputCollection.GetMousePosition());
    if ((double) this.lastContentPosition != (double) this.content.transform.localPosition[this.layoutDimension1])
      return;
    Vector3 point = this.GetTouchPosition() - this.content.transform.localPosition;
    for (int index = 0; index < this.renderedItems.Count; ++index)
    {
      ITouchListItem renderedItem = this.renderedItems[index];
      if ((renderedItem.IsHeader ? 1 : (renderedItem.Visible ? 1 : 0)) != 0 && this.itemInfos[renderedItem].Contains(point, this.layoutPlane))
      {
        this.touchBeginItem = renderedItem;
        break;
      }
    }
    Collider component = this.GetComponent<Collider>();
    component.enabled = false;
    PegUIElement pegUiElement = (PegUIElement) null;
    RaycastHit hitInfo;
    if (UniversalInputManager.Get().GetInputHitInfo(firstByLayer, out hitInfo))
      pegUiElement = hitInfo.transform.GetComponent<PegUIElement>();
    component.enabled = true;
    if (!((UnityEngine.Object) pegUiElement != (UnityEngine.Object) null))
      return;
    pegUiElement.TriggerPress();
  }

  protected override void OnRelease()
  {
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null || this.touchBeginItem == null || this.dragItemBegin.HasValue)
      return;
    this.touchBeginScreenPosition = new Vector2?();
    Collider component = this.GetComponent<Collider>();
    component.enabled = false;
    PegUIElement pegUiElement = (PegUIElement) null;
    RaycastHit hitInfo;
    if (UniversalInputManager.Get().GetInputHitInfo(firstByLayer, out hitInfo))
      pegUiElement = hitInfo.transform.GetComponent<PegUIElement>();
    component.enabled = true;
    if (!((UnityEngine.Object) pegUiElement != (UnityEngine.Object) null))
      return;
    pegUiElement.TriggerRelease();
    this.touchBeginItem = (ITouchListItem) null;
  }

  private void EnforceInitialized()
  {
    if (!this.IsInitialized)
      throw new InvalidOperationException("TouchList must be initialized before using it. Please wait for Awake to finish.");
  }

  private void Update()
  {
    if (UniversalInputManager.Get().IsTouchMode())
      this.UpdateTouchInput();
    else
      this.UpdateMouseInput();
    if (!this.m_isHoveredOverTouchList)
      return;
    this.OnHover(false);
  }

  private void UpdateTouchInput()
  {
    Vector3 touchPosition = this.GetTouchPosition();
    if (InputCollection.GetMouseButtonUp(0))
    {
      if (this.dragItemBegin.HasValue && this.ItemDragFinished != null)
      {
        int num = this.ItemDragFinished(this.touchBeginItem, this.GetItemDragDelta(touchPosition)) ? 1 : 0;
        this.dragItemBegin = new Vector3?();
      }
      this.touchBeginItem = (ITouchListItem) null;
      this.touchBeginScreenPosition = new Vector2?();
      this.dragBeginOffsetFromContent = new Vector3?();
    }
    if (this.touchBeginScreenPosition.HasValue)
    {
      Func<int, float, bool> func = (Func<int, float, bool>) ((dimension, inchThreshold) =>
      {
        int vector2Dimension = this.GetVector2Dimension(dimension);
        double f = (double) this.touchBeginScreenPosition.Value[vector2Dimension] - (double) InputCollection.GetMousePosition()[vector2Dimension];
        float num = inchThreshold * ((double) Screen.dpi > 0.0 ? Screen.dpi : 96f);
        return (double) Mathf.Abs((float) f) > (double) num;
      });
      if (this.ItemDragStarted != null && func(this.layoutDimension2, 0.05f) && this.ItemDragStarted(this.touchBeginItem, this.GetItemDragDelta(touchPosition)))
      {
        this.dragItemBegin = new Vector3?(this.GetTouchPosition());
        this.touchBeginScreenPosition = new Vector2?();
      }
      else if (func(this.layoutDimension1, 0.05f))
      {
        this.dragBeginContentPosition = this.content.transform.localPosition;
        this.dragBeginOffsetFromContent = new Vector3?(this.dragBeginContentPosition - this.lastTouchPosition);
        this.touchBeginItem = (ITouchListItem) null;
        this.touchBeginScreenPosition = new Vector2?();
      }
    }
    Vector3 localPosition1;
    if (this.dragItemBegin.HasValue)
    {
      if (!this.ItemDragged(this.touchBeginItem, this.GetItemDragDelta(touchPosition)))
      {
        this.dragItemBegin = new Vector3?();
        this.touchBeginItem = (ITouchListItem) null;
      }
    }
    else if (this.dragBeginOffsetFromContent.HasValue)
    {
      float contentPosition = touchPosition[this.layoutDimension1] + this.dragBeginOffsetFromContent.Value[this.layoutDimension1];
      float outOfBoundsDist = this.GetOutOfBoundsDist(contentPosition);
      if ((double) outOfBoundsDist != 0.0)
      {
        float num = TouchList.OutOfBoundsDistReducer(Mathf.Abs(outOfBoundsDist)) * Mathf.Sign(outOfBoundsDist);
        contentPosition = (double) num < 0.0 ? -this.excessContentSize + num : num;
      }
      Vector3 localPosition2 = this.content.transform.localPosition;
      this.lastContentPosition = localPosition2[this.layoutDimension1];
      localPosition2[this.layoutDimension1] = contentPosition;
      this.content.transform.localPosition = localPosition2;
      if ((double) this.lastContentPosition != (double) localPosition2[this.layoutDimension1])
        this.OnScrolled();
    }
    else
    {
      localPosition1 = this.content.transform.localPosition;
      float contentPosition = localPosition1[this.layoutDimension1];
      float outOfBoundsDist = this.GetOutOfBoundsDist(contentPosition);
      localPosition1 = this.content.transform.localPosition;
      float num1 = (localPosition1[this.layoutDimension1] - this.lastContentPosition) / Time.fixedDeltaTime;
      if ((double) (float) (MobileOverrideValue<float>) this.maxKineticScrollSpeed > (double) Mathf.Epsilon)
        num1 = (double) num1 <= 0.0 ? Mathf.Max(num1, -(float) (MobileOverrideValue<float>) this.maxKineticScrollSpeed) : Mathf.Min(num1, (float) (MobileOverrideValue<float>) this.maxKineticScrollSpeed);
      if ((double) outOfBoundsDist != 0.0)
      {
        Vector3 localPosition3 = this.content.transform.localPosition;
        this.lastContentPosition = contentPosition;
        float num2 = (float) (-400.0 * (double) outOfBoundsDist - (double) TouchList.ScrollBoundsSpringB * (double) num1);
        float num3 = num1 + num2 * Time.fixedDeltaTime;
        localPosition3[this.layoutDimension1] += num3 * Time.fixedDeltaTime;
        if ((double) Mathf.Abs(this.GetOutOfBoundsDist(localPosition3[this.layoutDimension1])) < 0.0500000007450581)
        {
          float num4 = (double) Mathf.Abs(localPosition3[this.layoutDimension1] + this.excessContentSize) < (double) Mathf.Abs(localPosition3[this.layoutDimension1]) ? -this.excessContentSize : 0.0f;
          localPosition3[this.layoutDimension1] = num4;
          this.lastContentPosition = num4;
        }
        this.content.transform.localPosition = localPosition3;
        this.OnScrolled();
      }
      else if ((double) num1 != 0.0)
      {
        localPosition1 = this.content.transform.localPosition;
        this.lastContentPosition = localPosition1[this.layoutDimension1];
        float num5 = (float) (-(double) Mathf.Sign(num1) * 10000.0);
        float f = num1 + num5 * Time.fixedDeltaTime;
        if ((double) Mathf.Abs(f) >= 0.00999999977648258 && (double) Mathf.Sign(f) == (double) Mathf.Sign(num1))
        {
          Vector3 localPosition4 = this.content.transform.localPosition;
          localPosition4[this.layoutDimension1] += f * Time.fixedDeltaTime;
          this.content.transform.localPosition = localPosition4;
          this.OnScrolled();
        }
      }
      else
        this.FixUpScrolling();
    }
    localPosition1 = this.content.transform.localPosition;
    float num6 = localPosition1[this.layoutDimension1] - this.lastContentPosition;
    if ((double) num6 != 0.0)
      this.PreBufferLongListItems((double) num6 < 0.0);
    this.lastTouchPosition = touchPosition;
  }

  private void PreBufferLongListItems(bool scrolledAhead)
  {
    if (this.LongListBehavior == null)
      return;
    this.allowModification = true;
    if (scrolledAhead && this.GetNumItemsAheadOfView() < this.longListBehavior.MinBuffer)
    {
      bool flag = this.CanScrollAhead;
      if (this.renderedItems.Count > 0 && (double) this.itemInfos[this.renderedItems[this.renderedItems.Count - 1]].Max[this.layoutDimension1] < (double) this.CalculateLocalClipBounds().min[this.layoutDimension1])
      {
        this.RefreshList(0, true);
        flag = false;
      }
      if (flag)
        this.LoadAhead();
    }
    else if (!scrolledAhead && this.GetNumItemsBehindView() < this.longListBehavior.MinBuffer)
    {
      bool flag = this.CanScrollBehind;
      if (this.renderedItems.Count > 0 && (double) this.itemInfos[this.renderedItems[0]].Min[this.layoutDimension1] > (double) this.CalculateLocalClipBounds().max[this.layoutDimension1])
      {
        this.RefreshList(0, true);
        flag = false;
      }
      if (flag)
        this.LoadBehind();
    }
    this.allowModification = false;
  }

  private void UpdateMouseInput()
  {
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null)
      return;
    Ray ray = firstByLayer.ScreenPointToRay(InputCollection.GetMousePosition());
    if (!this.GetComponent<Collider>().Raycast(ray, out RaycastHit _, firstByLayer.farClipPlane))
      return;
    float f = 0.0f;
    if ((double) Input.GetAxis("Mouse ScrollWheel") < 0.0 && this.CanScrollAhead)
      f -= this.scrollWheelIncrement;
    if ((double) Input.GetAxis("Mouse ScrollWheel") > 0.0 && this.CanScrollBehind)
      f += this.scrollWheelIncrement;
    if ((double) Mathf.Abs(f) <= (double) Mathf.Epsilon)
      return;
    float num1 = this.content.transform.localPosition[this.layoutDimension1] + f;
    if ((double) num1 <= -(double) this.excessContentSize)
      num1 = -this.excessContentSize;
    else if ((double) num1 >= 0.0)
      num1 = 0.0f;
    Vector3 localPosition = this.content.transform.localPosition;
    this.lastContentPosition = localPosition[this.layoutDimension1];
    localPosition[this.layoutDimension1] = num1;
    this.content.transform.localPosition = localPosition;
    float num2 = this.content.transform.localPosition[this.layoutDimension1] - this.lastContentPosition;
    this.lastContentPosition = this.content.transform.localPosition[this.layoutDimension1];
    if ((double) num2 != 0.0)
      this.PreBufferLongListItems((double) num2 < 0.0);
    this.FixUpScrolling();
    this.OnScrolled();
  }

  private float GetOutOfBoundsDist(float contentPosition)
  {
    float outOfBoundsDist = 0.0f;
    if ((double) contentPosition < -(double) this.excessContentSize)
      outOfBoundsDist = contentPosition + this.excessContentSize;
    else if ((double) contentPosition > 0.0)
      outOfBoundsDist = contentPosition;
    return outOfBoundsDist;
  }

  private void ScrollToItem_Internal(ITouchListItem item)
  {
    Bounds localClipBounds = this.CalculateLocalClipBounds();
    TouchList.ItemInfo itemInfo = this.itemInfos[item];
    Vector3 max = itemInfo.Max;
    double num1 = (double) max[this.layoutDimension1];
    max = localClipBounds.max;
    double num2 = (double) max[this.layoutDimension1];
    float num3 = (float) (num1 - num2);
    if ((double) num3 > 0.0)
    {
      Vector3 zero = Vector3.zero;
      zero[this.layoutDimension1] = num3;
      this.content.transform.Translate(zero);
      this.lastContentPosition = this.content.transform.localPosition[this.layoutDimension1];
      this.PreBufferLongListItems(true);
      this.OnScrolled();
    }
    double num4 = (double) localClipBounds.min[this.layoutDimension1];
    Vector3 vector3 = itemInfo.Min;
    double num5 = (double) vector3[this.layoutDimension1];
    float num6 = (float) (num4 - num5);
    if ((double) num6 <= 0.0)
      return;
    Vector3 zero1 = Vector3.zero;
    zero1[this.layoutDimension1] = -num6;
    this.content.transform.Translate(zero1);
    vector3 = this.content.transform.localPosition;
    this.lastContentPosition = vector3[this.layoutDimension1];
    this.PreBufferLongListItems(false);
    this.OnScrolled();
  }

  private void OnScrolled()
  {
    this.UpdateBackgroundScroll();
    this.SetVisibilityOfAllItems();
    if (this.Scrolled == null)
      return;
    this.Scrolled();
  }

  private Vector3 GetTouchPosition()
  {
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null)
      return Vector3.zero;
    Bounds bounds = this.GetComponent<Collider>().bounds;
    Transform transform = firstByLayer.transform;
    Vector3 inPoint = (double) Vector3.Distance(transform.position, bounds.min) < (double) Vector3.Distance(transform.position, bounds.max) ? bounds.min : bounds.max;
    Plane plane = new Plane(-transform.forward, inPoint);
    Ray ray = firstByLayer.ScreenPointToRay(InputCollection.GetMousePosition());
    float enter;
    plane.Raycast(ray, out enter);
    return this.transform.InverseTransformPoint(ray.GetPoint(enter));
  }

  private float GetItemDragDelta(Vector3 touchPosition) => this.dragItemBegin.HasValue ? touchPosition[this.layoutDimension2] - this.dragItemBegin.Value[this.layoutDimension2] : 0.0f;

  private void LoadAhead()
  {
    bool allowModification = this.allowModification;
    bool layoutSuspended = this.layoutSuspended;
    this.allowModification = true;
    int startingIndex = -1;
    int num1 = 0;
    int numItemsBehindView = this.GetNumItemsBehindView();
    for (int index = 0; index < numItemsBehindView - this.longListBehavior.MinBuffer; ++index)
    {
      ITouchListItem renderedItem = this.renderedItems[0];
      this.RemoveAt(0, false);
      this.longListBehavior.ReleaseItem(renderedItem);
    }
    float num2 = this.CalculateLocalClipBounds().max[this.layoutDimension1];
    int num3 = 0;
    for (int index = this.renderedItems.Count == 0 ? 0 : this.itemInfos[this.renderedItems.Last<ITouchListItem>()].LongListIndex + 1; index < this.longListBehavior.AllItemsCount && this.renderedItems.Count < this.longListBehavior.MaxAcquiredItems && num3 < this.longListBehavior.MinBuffer; ++index)
    {
      if (this.longListBehavior.IsItemShowable(index))
      {
        if (startingIndex < 0)
          startingIndex = this.renderedItems.Count;
        ITouchListItem key = this.longListBehavior.AcquireItem(index);
        this.Add(key, false);
        TouchList.ItemInfo itemInfo = this.itemInfos[key];
        itemInfo.LongListIndex = index;
        ++num1;
        if ((double) itemInfo.Min[this.layoutDimension1] > (double) num2)
          ++num3;
      }
    }
    if (startingIndex >= 0)
    {
      this.layoutSuspended = false;
      this.RepositionItems(startingIndex);
    }
    this.allowModification = allowModification;
    if (layoutSuspended == this.layoutSuspended)
      return;
    this.layoutSuspended = layoutSuspended;
  }

  private void LoadBehind()
  {
    bool allowModification = this.allowModification;
    this.allowModification = true;
    int num1 = 0;
    int itemsAheadOfView = this.GetNumItemsAheadOfView();
    for (int index = 0; index < itemsAheadOfView - this.longListBehavior.MinBuffer; ++index)
    {
      ITouchListItem renderedItem = this.renderedItems[this.renderedItems.Count - 1];
      this.RemoveAt(this.renderedItems.Count - 1, false);
      this.longListBehavior.ReleaseItem(renderedItem);
    }
    float num2 = this.CalculateLocalClipBounds().min[this.layoutDimension1];
    int num3 = 0;
    for (int index = this.renderedItems.Count == 0 ? this.longListBehavior.AllItemsCount - 1 : this.itemInfos[this.renderedItems.First<ITouchListItem>()].LongListIndex - 1; index >= 0 && this.renderedItems.Count < this.longListBehavior.MaxAcquiredItems && num3 < this.longListBehavior.MinBuffer; --index)
    {
      if (this.longListBehavior.IsItemShowable(index))
      {
        ITouchListItem key = this.longListBehavior.AcquireItem(index);
        this.InsertAndPositionBehind(key, index);
        TouchList.ItemInfo itemInfo = this.itemInfos[key];
        itemInfo.LongListIndex = index;
        ++num1;
        if ((double) itemInfo.Max[this.layoutDimension1] < (double) num2)
          ++num3;
      }
    }
    this.allowModification = allowModification;
  }

  private int GetNumItemsBehindView()
  {
    float num = this.CalculateLocalClipBounds().min[this.layoutDimension1];
    for (int index = 0; index < this.renderedItems.Count; ++index)
    {
      if ((double) this.itemInfos[this.renderedItems[index]].Max[this.layoutDimension1] > (double) num)
        return index;
    }
    return this.renderedItems.Count;
  }

  private int GetNumItemsAheadOfView()
  {
    float num = this.CalculateLocalClipBounds().max[this.layoutDimension1];
    for (int index = this.renderedItems.Count - 1; index >= 0; --index)
    {
      if ((double) this.itemInfos[this.renderedItems[index]].Min[this.layoutDimension1] < (double) num)
        return this.renderedItems.Count - 1 - index;
    }
    return this.renderedItems.Count;
  }

  public void RefreshList(int startingLongListIndex, bool preserveScrolling)
  {
    if (this.longListBehavior == null)
      return;
    bool allowModification = this.allowModification;
    this.allowModification = true;
    int num1 = this.SelectedItem == null ? -1 : this.itemInfos[this.SelectedItem].LongListIndex;
    int index1 = -2;
    int startingIndex = -1;
    if (startingLongListIndex > 0)
    {
      for (int index2 = 0; index2 < this.renderedItems.Count; ++index2)
      {
        if (this.itemInfos[this.renderedItems[index2]].LongListIndex < startingLongListIndex)
        {
          index1 = index2;
        }
        else
        {
          startingIndex = index2;
          break;
        }
      }
    }
    else
      startingIndex = 0;
    int num2 = startingIndex == -1 ? index1 + 1 : startingIndex;
    Bounds bounds = this.GetComponent<Collider>().bounds;
    Vector3? initialItemPosition = new Vector3?();
    Vector3 point1 = Vector3.zero;
    int num3 = this.orientation == TouchList.Orientation.Vertical ? -1 : 1;
    Vector3 vector3;
    if (preserveScrolling)
    {
      point1 = this.content.transform.position;
      point1[this.layoutDimension1] -= (float) num3 * bounds.extents[this.layoutDimension1];
      point1[this.layoutDimension1] += (float) num3 * this.padding[this.GetVector2Dimension(this.layoutDimension1)];
      point1[this.layoutDimension2] = bounds.center[this.layoutDimension2];
      ref Vector3 local1 = ref point1;
      int layoutDimension3 = this.layoutDimension3;
      vector3 = bounds.center;
      double num4 = (double) vector3[this.layoutDimension3];
      local1[layoutDimension3] = (float) num4;
      Vector3 localPosition = this.content.transform.localPosition;
      this.content.transform.localPosition = Vector3.zero;
      Bounds localClipBounds = this.CalculateLocalClipBounds();
      Vector3 min = localClipBounds.min;
      ref Vector3 local2 = ref min;
      int layoutDimension1 = this.layoutDimension1;
      double num5 = -(double) localPosition[this.layoutDimension1];
      vector3 = localClipBounds.min;
      double num6 = (double) vector3[this.layoutDimension1];
      double num7 = num5 + num6;
      local2[layoutDimension1] = (float) num7;
      this.content.transform.localPosition = localPosition;
      initialItemPosition = new Vector3?(min);
      if (index1 >= 0)
      {
        ITouchListItem renderedItem1 = this.renderedItems[index1];
        TouchList.ItemInfo itemInfo1 = this.itemInfos[renderedItem1];
        point1 = renderedItem1.transform.position - itemInfo1.Offset;
        point1[this.layoutDimension1] += (float) num3 * this.elementSpacing;
        ITouchListItem renderedItem2 = this.renderedItems[0];
        TouchList.ItemInfo itemInfo2 = this.itemInfos[renderedItem2];
        initialItemPosition = new Vector3?(renderedItem2.transform.localPosition - itemInfo2.Offset);
      }
    }
    int num8 = 0;
    if (num2 >= 0)
    {
      for (int index3 = this.renderedItems.Count - 1; index3 >= num2; --index3)
      {
        ++num8;
        ITouchListItem renderedItem = this.renderedItems[index3];
        this.RemoveAt(index3, false);
        this.longListBehavior.ReleaseItem(renderedItem);
      }
    }
    if (startingIndex < 0)
    {
      startingIndex = index1 + 1;
      if (startingIndex < 0)
        startingIndex = 0;
    }
    int num9 = 0;
    for (int index4 = startingLongListIndex; index4 < this.longListBehavior.AllItemsCount && this.renderedItems.Count < this.longListBehavior.MaxAcquiredItems; ++index4)
    {
      if (this.longListBehavior.IsItemShowable(index4))
      {
        bool flag = true;
        if (preserveScrolling)
        {
          flag = false;
          Vector3 itemSize = this.longListBehavior.GetItemSize(index4);
          Vector3 point2 = point1;
          point2[this.layoutDimension1] += (float) num3 * itemSize[this.layoutDimension1];
          if (bounds.Contains(point1) || bounds.Contains(point2))
            flag = true;
          point1 = point2;
          point1[this.layoutDimension1] += (float) num3 * this.elementSpacing;
        }
        if (flag)
        {
          ++num9;
          ITouchListItem key = this.longListBehavior.AcquireItem(index4);
          this.Add(key, false);
          this.itemInfos[key].LongListIndex = index4;
        }
      }
    }
    this.RepositionItems(startingIndex, initialItemPosition);
    if (startingIndex == 0)
      this.LoadBehind();
    if (num2 >= 0)
      this.LoadAhead();
    bool flag1 = false;
    vector3 = this.content.transform.localPosition;
    float outOfBoundsDist = this.GetOutOfBoundsDist(vector3[this.layoutDimension1]);
    if ((double) outOfBoundsDist != 0.0 && (double) this.excessContentSize > 0.0)
    {
      Vector3 localPosition = this.content.transform.localPosition;
      localPosition[this.layoutDimension1] -= outOfBoundsDist;
      double num10 = (double) localPosition[this.layoutDimension1];
      vector3 = this.content.transform.localPosition;
      double num11 = (double) vector3[this.layoutDimension1];
      double num12 = num10 - num11;
      this.content.transform.localPosition = localPosition;
      vector3 = this.content.transform.localPosition;
      this.lastContentPosition = vector3[this.layoutDimension1];
      if (num12 < 0.0)
        this.LoadAhead();
      else
        this.LoadBehind();
      flag1 = true;
    }
    if (num1 >= 0 && this.renderedItems.Count > 0 && num1 >= this.itemInfos[this.renderedItems.First<ITouchListItem>()].LongListIndex && num1 <= this.itemInfos[this.renderedItems.Last<ITouchListItem>()].LongListIndex)
    {
      for (int index5 = 0; index5 < this.renderedItems.Count; ++index5)
      {
        if (this.renderedItems[index5] is ISelectableTouchListItem renderedItem && this.itemInfos[(ITouchListItem) renderedItem].LongListIndex == num1)
        {
          this.selection = new int?(index5);
          renderedItem.Selected();
          break;
        }
      }
    }
    bool flag2 = this.RecalculateLongListContentSize(false) | flag1;
    this.allowModification = allowModification;
    if (!flag2)
      return;
    this.OnScrolled();
    this.OnScrollingEnabledChanged();
  }

  private void OnScrollingEnabledChanged()
  {
    if (this.ScrollingEnabledChanged == null)
      return;
    if (this.longListBehavior == null)
      this.ScrollingEnabledChanged((double) this.excessContentSize > 0.0 && this.scrollEnabled);
    else
      this.ScrollingEnabledChanged((double) this.m_fullListContentSize > (double) this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)] && this.scrollEnabled);
  }

  public void RecalculateItemSizeAndOffsets(bool ignoreCurrentPosition)
  {
    for (int index = 0; index < this.renderedItems.Count; ++index)
      this.itemInfos[this.renderedItems[index]].CalculateSizeAndOffset(this.layoutPlane, ignoreCurrentPosition);
    this.RepositionItems(0);
  }

  private void RepositionItems(int startingIndex, Vector3? initialItemPosition = null)
  {
    if (this.layoutSuspended)
      return;
    if (this.orientation == TouchList.Orientation.Vertical)
      this.transform.localScale = Vector3.one;
    Vector3 localPosition = this.content.transform.localPosition;
    this.content.transform.localPosition = Vector3.zero;
    Vector3 vector3_1 = this.CalculateLocalClipBounds().min;
    if (initialItemPosition.HasValue)
      vector3_1 = initialItemPosition.Value;
    vector3_1[this.layoutDimension1] += this.padding[this.GetVector2Dimension(this.layoutDimension1)];
    vector3_1[this.layoutDimension3] = 0.0f;
    this.content.transform.localPosition = localPosition;
    this.ValidateBreadth();
    startingIndex -= startingIndex % this.breadth;
    Vector3 vector3_2;
    if (startingIndex > 0)
    {
      int num = startingIndex - this.breadth;
      float b = float.MinValue;
      for (int index = num; index < startingIndex && index < this.renderedItems.Count; ++index)
      {
        vector3_2 = this.itemInfos[this.renderedItems[index]].Max;
        b = Mathf.Max(vector3_2[this.layoutDimension1], b);
      }
      vector3_1[this.layoutDimension1] = b + this.elementSpacing;
    }
    Vector3 zero = Vector3.zero;
    zero[this.layoutDimension1] = 1f;
    for (int index = startingIndex; index < this.renderedItems.Count; ++index)
    {
      ITouchListItem renderedItem = this.renderedItems[index];
      if ((renderedItem.IsHeader ? 1 : (renderedItem.Visible ? 1 : 0)) == 0)
      {
        this.renderedItems[index].Visible = false;
        this.renderedItems[index].gameObject.SetActive(false);
      }
      else
      {
        TouchList.ItemInfo itemInfo = this.itemInfos[this.renderedItems[index]];
        Vector3 vector3_3 = vector3_1 + itemInfo.Offset;
        ref Vector3 local = ref vector3_3;
        int layoutDimension2 = this.layoutDimension2;
        double breadthPosition = (double) this.GetBreadthPosition(index);
        vector3_2 = itemInfo.Offset;
        double num1 = (double) vector3_2[this.layoutDimension2];
        double num2 = breadthPosition + num1;
        local[layoutDimension2] = (float) num2;
        this.renderedItems[index].transform.localPosition = vector3_3;
        this.renderedItems[index].OnPositionUpdate();
        if ((index + 1) % this.breadth == 0)
        {
          vector3_2 = itemInfo.Max;
          vector3_1 = (vector3_2[this.layoutDimension1] + this.elementSpacing) * zero;
        }
      }
    }
    this.RecalculateSize();
    this.UpdateBackgroundScroll();
    if (this.orientation == TouchList.Orientation.Vertical)
      this.transform.localScale = this.GetNegatedScale(Vector3.one);
    this.SetVisibilityOfAllItems();
  }

  private void InsertAndPositionBehind(ITouchListItem item, int longListIndex)
  {
    if (this.renderedItems.Count == 0)
    {
      this.Add(item, true);
    }
    else
    {
      ITouchListItem key = this.renderedItems.FirstOrDefault<ITouchListItem>();
      if (key == null)
      {
        this.Insert(0, item, true);
      }
      else
      {
        if (this.orientation == TouchList.Orientation.Vertical)
          this.transform.localScale = Vector3.one;
        TouchList.ItemInfo itemInfo1 = this.itemInfos[key];
        Vector3 vector3_1 = key.transform.localPosition - itemInfo1.Offset;
        this.Insert(0, item, false);
        this.itemInfos[item].LongListIndex = longListIndex;
        TouchList.ItemInfo itemInfo2 = this.itemInfos[item];
        Vector3 vector3_2 = vector3_1;
        float num1 = itemInfo2.Size[this.layoutDimension1] + this.elementSpacing;
        vector3_2[this.layoutDimension1] = vector3_2[this.layoutDimension1] - num1;
        vector3_2 += itemInfo2.Offset;
        item.transform.localPosition = vector3_2;
        int? selection = this.selection;
        int num2 = -1;
        if (selection.GetValueOrDefault() == num2 & selection.HasValue && item is ISelectableTouchListItem && ((ISelectableTouchListItem) item).IsSelected())
          this.selection = new int?(0);
        this.RecalculateSize();
        this.UpdateBackgroundScroll();
        if (this.orientation == TouchList.Orientation.Vertical)
          this.transform.localScale = this.GetNegatedScale(Vector3.one);
        bool flag = this.IsItemVisible(0);
        item.gameObject.SetActive(flag);
      }
    }
  }

  private void RecalculateSize()
  {
    float num1 = Math.Abs((this.GetComponent<Collider>() as BoxCollider).size[this.layoutDimension1]);
    float num2 = (float) (-(double) num1 / 2.0);
    float val2 = num2;
    if (this.renderedItems.Any<ITouchListItem>())
    {
      this.ValidateBreadth();
      int num3 = this.renderedItems.Count - 1;
      int num4 = num3 - num3 % this.breadth;
      int num5 = Math.Min(num4 + this.breadth, this.renderedItems.Count);
      for (int index = num4; index < num5; ++index)
        val2 = Math.Max(this.itemInfos[this.renderedItems[index]].Max[this.layoutDimension1], val2);
      this.contentSize = val2 - num2 + this.padding[this.GetVector2Dimension(this.layoutDimension1)];
      this.excessContentSize = Math.Max(this.contentSize - num1, 0.0f);
    }
    else
    {
      this.contentSize = 0.0f;
      this.excessContentSize = 0.0f;
    }
    this.OnScrollingEnabledChanged();
  }

  public bool RecalculateLongListContentSize(bool fireOnScroll = true)
  {
    if (this.longListBehavior == null)
      return false;
    float fullListContentSize = this.m_fullListContentSize;
    this.m_fullListContentSize = 0.0f;
    bool flag = true;
    for (int allItemsIndex = 0; allItemsIndex < this.longListBehavior.AllItemsCount; ++allItemsIndex)
    {
      if (this.longListBehavior.IsItemShowable(allItemsIndex))
      {
        this.m_fullListContentSize += this.longListBehavior.GetItemSize(allItemsIndex)[this.layoutDimension1];
        if (flag)
          flag = false;
        else
          this.m_fullListContentSize += this.elementSpacing;
      }
    }
    if ((double) this.m_fullListContentSize > 0.0)
      this.m_fullListContentSize += 2f * this.padding[this.GetVector2Dimension(this.layoutDimension1)];
    int num = (double) fullListContentSize != (double) this.m_fullListContentSize ? 1 : 0;
    if (num == 0)
      return num != 0;
    if (!fireOnScroll)
      return num != 0;
    this.OnScrolled();
    this.OnScrollingEnabledChanged();
    return num != 0;
  }

  private void UpdateBackgroundBounds()
  {
    if ((UnityEngine.Object) this.background == (UnityEngine.Object) null)
      return;
    Collider component = this.GetComponent<Collider>();
    Vector3 size = (component as BoxCollider).size;
    size[this.layoutDimension1] = Math.Abs(size[this.layoutDimension1]);
    size[this.layoutDimension3] = 0.0f;
    Camera firstByLayer = CameraUtils.FindFirstByLayer((GameLayer) this.gameObject.layer);
    if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null)
      return;
    Vector3 position = (double) Vector3.Distance(firstByLayer.transform.position, component.bounds.min) > (double) Vector3.Distance(firstByLayer.transform.position, component.bounds.max) ? component.bounds.min : component.bounds.max;
    Vector3 zero = Vector3.zero;
    zero[this.layoutDimension3] = this.content.transform.InverseTransformPoint(position)[this.layoutDimension3];
    this.background.SetBounds(new Bounds(zero, size));
    this.UpdateBackgroundScroll();
  }

  private void UpdateBackgroundScroll()
  {
    if ((UnityEngine.Object) this.background == (UnityEngine.Object) null)
      return;
    Vector3 vector3 = (this.GetComponent<Collider>() as BoxCollider).size;
    float num1 = Math.Abs(vector3[this.layoutDimension1]);
    vector3 = this.content.transform.localPosition;
    float num2 = vector3[this.layoutDimension1];
    if (this.orientation == TouchList.Orientation.Vertical)
      num2 *= -1f;
    Vector2 offset = this.background.Offset;
    offset[this.GetVector2Dimension(this.layoutDimension1)] = num2 / num1;
    this.background.Offset = offset;
  }

  private float GetBreadthPosition(int itemIndex)
  {
    float num1 = this.padding[this.GetVector2Dimension(this.layoutDimension2)];
    float num2 = 0.0f;
    int num3 = itemIndex - itemIndex % this.breadth;
    int num4 = Math.Min(num3 + this.breadth, this.renderedItems.Count);
    Vector3 size;
    for (int index = num3; index < num4; ++index)
    {
      if (index == itemIndex)
        num2 = num1;
      double num5 = (double) num1;
      size = this.itemInfos[this.renderedItems[index]].Size;
      double num6 = (double) size[this.layoutDimension2];
      num1 = (float) (num5 + num6);
    }
    float num7 = num1 + this.padding[this.GetVector2Dimension(this.layoutDimension2)];
    float num8 = 0.0f;
    size = (this.GetComponent<Collider>() as BoxCollider).size;
    float num9 = size[this.layoutDimension2];
    TouchList.Alignment alignment = this.alignment;
    if (this.orientation == TouchList.Orientation.Horizontal && this.alignment != TouchList.Alignment.Mid)
      alignment = this.alignment ^ TouchList.Alignment.Max;
    switch (alignment)
    {
      case TouchList.Alignment.Min:
        num8 = (float) (-(double) num9 / 2.0);
        break;
      case TouchList.Alignment.Mid:
        num8 = (float) (-(double) num7 / 2.0);
        break;
      case TouchList.Alignment.Max:
        num8 = num9 / 2f - num7;
        break;
    }
    return num8 + num2;
  }

  private Vector3 GetNegatedScale(Vector3 scale)
  {
    scale[this.layoutPlane == TouchList.LayoutPlane.XY ? 1 : 2] *= -1f;
    return scale;
  }

  private int GetVector2Dimension(int vec3Dimension) => vec3Dimension != 0 ? 1 : vec3Dimension;

  private int GetVector3Dimension(int vec2Dimension) => vec2Dimension == 0 || this.layoutPlane == TouchList.LayoutPlane.XY ? vec2Dimension : 2;

  private void ValidateBreadth()
  {
    if (this.longListBehavior != null)
      this.breadth = 1;
    else
      this.breadth = Math.Max(this.breadth, 1);
  }

  private Bounds CalculateLocalClipBounds()
  {
    Collider component = this.GetComponent<Collider>();
    Vector3 vector3_1 = this.content.transform.InverseTransformPoint(component.bounds.min);
    Vector3 vector3_2 = this.content.transform.InverseTransformPoint(component.bounds.max);
    return new Bounds((vector3_2 + vector3_1) / 2f, VectorUtils.Abs(vector3_2 - vector3_1));
  }

  public enum Orientation
  {
    Horizontal,
    Vertical,
  }

  public enum Alignment
  {
    Min,
    Mid,
    Max,
  }

  public enum LayoutPlane
  {
    XY,
    XZ,
  }

  public delegate bool SelectedIndexChangingEvent(int index);

  public delegate void ScrollingEnabledChangedEvent(bool canScroll);

  public delegate bool ItemDragEvent(ITouchListItem item, float dragAmount);

  public interface ILongListBehavior
  {
    int AllItemsCount { get; }

    int MinBuffer { get; }

    void ReleaseAllItems();

    void ReleaseItem(ITouchListItem item);

    ITouchListItem AcquireItem(int index);

    int MaxAcquiredItems { get; }

    bool IsItemShowable(int allItemsIndex);

    Vector3 GetItemSize(int allItemsIndex);
  }

  private class ItemInfo
  {
    private readonly ITouchListItem item;

    public Vector3 Size { get; private set; }

    public Vector3 Offset { get; private set; }

    public int LongListIndex { get; set; }

    public Vector3 Min => this.item.transform.localPosition + Vector3.Scale(this.item.LocalBounds.min, VectorUtils.Abs(this.item.transform.localScale));

    public Vector3 Max => this.item.transform.localPosition + Vector3.Scale(this.item.LocalBounds.max, VectorUtils.Abs(this.item.transform.localScale));

    public ItemInfo(ITouchListItem item, TouchList.LayoutPlane layoutPlane)
    {
      this.item = item;
      this.CalculateSizeAndOffset(layoutPlane);
    }

    public void CalculateSizeAndOffset(
      TouchList.LayoutPlane layoutPlane,
      bool ignoreCurrentPosition = false)
    {
      Vector3 vector3_1 = Vector3.Scale(this.item.LocalBounds.min, VectorUtils.Abs(this.item.transform.localScale));
      Vector3 vector3_2 = Vector3.Scale(this.item.LocalBounds.max, VectorUtils.Abs(this.item.transform.localScale));
      if (!ignoreCurrentPosition)
      {
        vector3_1 -= this.item.transform.localPosition;
        vector3_2 -= this.item.transform.localPosition;
      }
      this.Size = vector3_2 - vector3_1;
      Vector3 vector3_3 = vector3_1;
      if (layoutPlane == TouchList.LayoutPlane.XZ)
        vector3_3.y = vector3_2.y;
      this.Offset = -vector3_3;
      if (ignoreCurrentPosition)
        return;
      this.Offset += this.item.transform.localPosition;
    }

    public bool Contains(Vector3 point, TouchList.LayoutPlane layoutPlane)
    {
      Vector3 min = this.Min;
      Vector3 max = this.Max;
      int index = layoutPlane == TouchList.LayoutPlane.XY ? 1 : 2;
      return (double) point.x > (double) min.x && (double) point[index] > (double) min[index] && (double) point.x < (double) max.x && (double) point[index] < (double) max[index];
    }
  }
}
