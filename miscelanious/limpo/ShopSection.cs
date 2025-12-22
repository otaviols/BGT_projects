using Blizzard.T5.Core.Utils;
using Hearthstone.DataModels;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopSection : ShopBrowserElement, IPopupRendering
{
  private static Comparison<ShopSlot> SortSlots = (Comparison<ShopSlot>) ((A, B) => ShopBrowserElement.ComparePosition((ShopBrowserElement) A, (ShopBrowserElement) B));
  protected Widget m_widget;
  protected UIBScrollableItem m_scrollableItem;
  protected List<ShopSlot> m_slots = new List<ShopSlot>();
  protected bool m_slotsDirty;
  private IPopupRoot m_popupRoot;
  private HashSet<IPopupRendering> m_popupRenderingComponents = new HashSet<IPopupRendering>();

  private void Start()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_scrollableItem = this.GetComponentInChildren<UIBScrollableItem>();
    ShopBrowser componentInParents = GameObjectUtils.FindComponentInParents<ShopBrowser>(this.gameObject);
    if ((UnityEngine.Object) componentInParents != (UnityEngine.Object) null)
      componentInParents.RegisterSection(this);
    if (this.SuppressSelfRefresh)
      return;
    ProductTierDataModel tierDataModel = this.GetTierDataModel();
    if (tierDataModel == null || tierDataModel.BrowserButtons.Count <= 0)
      return;
    this.ScheduleRefresh();
  }

  private void OnEnable()
  {
    if (!this.m_slotsDirty || this.SuppressSelfRefresh)
      return;
    this.StartCoroutine(this.RefreshSlotsWhenReady());
  }

  private void Update()
  {
    if (this.SuppressSelfRefresh)
      return;
    this.CheckDoneResolvingLayout();
  }

  public bool IsResolvingLayout { get; private set; }

  public bool IsResolvingSlotVisuals { get; private set; }

  public bool SuppressResolvingSlots { get; set; }

  public bool SuppressSelfRefresh { get; set; }

  public Widget widget => this.m_widget;

  public void ScheduleRefresh()
  {
    if (this.m_slotsDirty)
      return;
    this.m_slotsDirty = true;
    if (!this.gameObject.activeInHierarchy)
      return;
    this.StartCoroutine(this.RefreshSlotsWhenReady());
  }

  public void RegisterSlot(ShopSlot slot)
  {
    if (!this.m_slots.Contains(slot))
      this.m_slots.Add(slot);
    if (this.SuppressSelfRefresh)
      return;
    this.ScheduleRefresh();
  }

  public void ResizeHeightForStacking()
  {
    List<ShopBrowserElement> elements = new List<ShopBrowserElement>();
    foreach (Component component in this.transform)
      ShopSection.GetActiveElementsExcludeShopSlots(component.gameObject, elements);
    if (elements.Count <= 0)
      return;
    float a1 = float.MaxValue;
    float a2 = float.MinValue;
    foreach (ShopBrowserElement shopBrowserElement in elements)
    {
      a1 = Mathf.Min(a1, shopBrowserElement.Bounds.yMin);
      a2 = Mathf.Max(a2, shopBrowserElement.Bounds.yMax);
    }
    this.Bounds.yMin = a1;
    this.Bounds.yMax = a2;
    this.OnElementBoundsChanged();
  }

  public ProductTierDataModel GetTierDataModel() => this.m_widget.GetDataModel<ProductTierDataModel>();

  public List<ShopSlot> GetSortedEnabledSlots()
  {
    List<ShopSlot> sortedEnabledSlots = new List<ShopSlot>();
    foreach (ShopSlot slot in this.m_slots)
    {
      if (this.IsSlotEnabled(slot))
        sortedEnabledSlots.Add(slot);
    }
    sortedEnabledSlots.Sort(ShopSection.SortSlots);
    return sortedEnabledSlots;
  }

  public void BindDataModelsToSlots()
  {
    this.m_slotsDirty = false;
    this.IsResolvingSlotVisuals = true;
    ProductTierDataModel tierDataModel = this.GetTierDataModel();
    if (tierDataModel == null || tierDataModel.BrowserButtons == null)
    {
      this.IsElementEnabled = false;
      this.IsResolvingSlotVisuals = false;
    }
    else
    {
      DataModelList<ShopBrowserButtonDataModel> browserButtons = tierDataModel.BrowserButtons;
      List<ShopSlot> sortedEnabledSlots = this.GetSortedEnabledSlots();
      bool flag = StoreManager.Get().Catalog.CurrentTestDataMode == ProductCatalog.TestDataMode.TIER_TEST_DATA;
      this.IsElementEnabled = ShopUtils.ShouldDisplayTier(tierDataModel, this.GetSortedEnabledSlots().Count) || flag && browserButtons.Count > 0;
      if (!this.IsElementEnabled)
      {
        this.IsResolvingSlotVisuals = false;
      }
      else
      {
        for (int index = 0; index < sortedEnabledSlots.Count; ++index)
        {
          ShopSlot shopSlot = sortedEnabledSlots[index];
          if (index < browserButtons.Count)
            shopSlot.SetBrowserButton(browserButtons.ElementAt<ShopBrowserButtonDataModel>(index));
          else
            shopSlot.Reset();
        }
        this.m_widget.RegisterDoneChangingStatesListener((Action<object>) (_ => this.IsResolvingSlotVisuals = false), (object) null, true, true);
      }
    }
  }

  private IEnumerator RefreshSlotsWhenReady()
  {
    ShopSection shopSection = this;
    Log.Store.PrintDebug("{0} resolving layout...", (object) shopSection.gameObject.name);
    shopSection.IsResolvingLayout = true;
    while (!shopSection.CheckDoneResolvingLayout())
      yield return (object) null;
    Log.Store.PrintDebug("{0} layout resolved", (object) shopSection.gameObject.name);
    if (shopSection.m_slotsDirty)
    {
      if (shopSection.SuppressResolvingSlots)
      {
        Log.Store.PrintDebug("{0} suppressing slot visuals...", (object) shopSection.gameObject.name);
        shopSection.IsResolvingSlotVisuals = true;
        shopSection.m_slots.ForEach((Action<ShopSlot>) (s => s.Reset()));
        while (shopSection.SuppressResolvingSlots)
          yield return (object) null;
      }
      shopSection.BindDataModelsToSlots();
    }
  }

  private bool CheckDoneResolvingLayout()
  {
    if (this.IsResolvingLayout && !this.m_widget.GetIsChangingStates((Func<GameObject, bool>) (go => (UnityEngine.Object) go.GetComponent<ShopSlot>() == (UnityEngine.Object) null)))
    {
      Log.Store.PrintDebug("ShopSection done resolving layout");
      this.IsResolvingLayout = false;
    }
    return !this.IsResolvingLayout;
  }

  private bool IsSlotEnabled(ShopSlot slot) => slot.IsElementEnabled && slot.gameObject.activeInHierarchy;

  private float GetFarthestSlotSide(ShopBrowserElement.Side side)
  {
    ShopSection.SlotCertainty slotCertainty = ShopSection.SlotCertainty.UNKNOWN;
    float a = (side == ShopBrowserElement.Side.TOP ? 1 : (side == ShopBrowserElement.Side.RIGHT ? 1 : 0)) != 0 ? float.MinValue : float.MaxValue;
    foreach (ShopSlot slot in this.m_slots)
    {
      if (slot.IsFilled)
        slotCertainty = ShopSection.SlotCertainty.KNOWN;
      else if (slotCertainty < ShopSection.SlotCertainty.KNOWN)
        slotCertainty = ShopSection.SlotCertainty.MAYBE;
      else
        continue;
      switch (side)
      {
        case ShopBrowserElement.Side.TOP:
          a = Mathf.Max(a, slot.Top);
          continue;
        case ShopBrowserElement.Side.BOTTOM:
          a = Mathf.Min(a, slot.Bottom);
          continue;
        case ShopBrowserElement.Side.LEFT:
          a = Mathf.Min(a, slot.Left);
          continue;
        case ShopBrowserElement.Side.RIGHT:
          a = Mathf.Max(a, slot.Right);
          continue;
        default:
          continue;
      }
    }
    if (slotCertainty < ShopSection.SlotCertainty.KNOWN && (side == ShopBrowserElement.Side.BOTTOM || side == ShopBrowserElement.Side.RIGHT))
      return this.GetFarthestSlotSide(side == ShopBrowserElement.Side.BOTTOM ? ShopBrowserElement.Side.TOP : ShopBrowserElement.Side.LEFT);
    if (slotCertainty == ShopSection.SlotCertainty.UNKNOWN)
      a = 0.0f;
    return a;
  }

  protected override void OnElementBoundsChanged()
  {
    if (!((UnityEngine.Object) this.m_scrollableItem != (UnityEngine.Object) null))
      return;
    this.m_scrollableItem.m_offset.x = this.BoundsX + this.Width / 2f;
    this.m_scrollableItem.m_offset.z = this.BoundsY + this.Height / 2f;
    this.m_scrollableItem.m_size.x = this.Width;
    this.m_scrollableItem.m_size.z = this.Height;
  }

  protected override void OnElementEnabled()
  {
    if (!((UnityEngine.Object) this.m_scrollableItem != (UnityEngine.Object) null))
      return;
    if (this.IsElementEnabled)
      this.m_scrollableItem.m_active = UIBScrollableItem.ActiveState.Active;
    else
      this.m_scrollableItem.m_active = UIBScrollableItem.ActiveState.Inactive;
  }

  protected static void GetActiveElementsExcludeShopSlots(
    GameObject gameObj,
    List<ShopBrowserElement> elements)
  {
    if (!gameObj.activeInHierarchy || (UnityEngine.Object) gameObj.GetComponent<ShopSlot>() != (UnityEngine.Object) null)
      return;
    elements.AddRange(((IEnumerable<ShopBrowserElement>) gameObj.GetComponents<ShopBrowserElement>()).Where<ShopBrowserElement>((Func<ShopBrowserElement, bool>) (comp => comp.enabled)));
    foreach (Component component in gameObj.transform)
      ShopSection.GetActiveElementsExcludeShopSlots(component.gameObject, elements);
  }

  public void EnablePopupRendering(IPopupRoot popupRoot)
  {
    this.m_popupRoot = popupRoot;
    this.StartCoroutine(this.EnablePopupRenderingInternal());
  }

  private IEnumerator EnablePopupRenderingInternal()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    ShopSection shopSection = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      shopSection.m_popupRoot.ApplyPopupRendering(shopSection.transform, shopSection.m_popupRenderingComponents);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u003E2__current = (object) new WaitUntil(new Func<bool>(shopSection.\u003CEnablePopupRenderingInternal\u003Eb__43_0));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void DisablePopupRendering()
  {
    if (this.m_popupRoot == null)
      return;
    this.m_popupRoot.CleanupPopupRendering(this.m_popupRenderingComponents);
    this.m_popupRoot = (IPopupRoot) null;
  }

  public bool HandlesChildPropagation() => true;

  private enum SlotCertainty
  {
    UNKNOWN,
    MAYBE,
    KNOWN,
  }
}
