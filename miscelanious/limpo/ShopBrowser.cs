using Hearthstone.DataModels;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopBrowser : MonoBehaviour, IPopupRendering
{
  [SerializeField]
  protected ShopProductData[] m_testData;
  [SerializeField]
  protected ProductCatalog.TestDataMode m_testDataMode = ProductCatalog.TestDataMode.ADD_PRODUCT_TEST_DATA;
  [SerializeField]
  private bool m_dataDirty;
  [SerializeField]
  protected float m_stackingMargins;
  [SerializeField]
  private bool m_layoutDirty;
  [SerializeField]
  private bool m_loadSynchronously = true;
  [SerializeField]
  private bool m_loadSectionsSequentially = true;
  private const string c_tierWidgetPrefab = "ShopMasterTier.prefab:28b5d7137297f234ebe64c4499d41901";
  private const string TIER_POSITIONED_EVENT = "TIER_POSITIONED";
  private const string TIER_SLOTS_LOADED_EVENT = "TIER_SLOTS_LOADED";
  protected List<ShopSection> m_shopSections = new List<ShopSection>();
  protected List<WidgetInstance> m_tierInstances = new List<WidgetInstance>();
  private Widget m_widget;
  private ProductCatalog.TestDataMode? m_appliedTestData;
  private bool m_isLoading;
  private int m_framesSpentLoading;
  private DateTime m_timeStartedLoading;
  private TimeSpan m_timeSpentLoading;
  private IPopupRoot m_popupRoot;

  public bool IsLoading => this.m_isLoading;

  private void Start()
  {
    this.m_widget = this.GetComponent<Widget>();
    if (StoreManager.Get().Catalog.CurrentTestDataMode != ProductCatalog.TestDataMode.NO_TEST_DATA)
      this.m_appliedTestData = new ProductCatalog.TestDataMode?(StoreManager.Get().Catalog.CurrentTestDataMode);
    else
      this.ApplyTestData();
    Shop shop = Shop.Get();
    if (!((UnityEngine.Object) shop != (UnityEngine.Object) null))
      return;
    shop.OnCloseCompleted += new Action(this.HandleShopCloseCompleted);
  }

  public void RefreshContents() => this.m_dataDirty = true;

  public void EnableInput(bool enabled)
  {
    foreach (ShopSlot componentsInChild in this.GetComponentsInChildren<ShopSlot>())
      componentsInChild.EnableInput(enabled);
  }

  public void RegisterSection(ShopSection section)
  {
    section.SuppressSelfRefresh = this.m_loadSectionsSequentially;
    if (this.m_shopSections.Contains(section))
      return;
    this.m_shopSections.Add(section);
  }

  public bool IsReady() => this.AreLayoutsResolved() && !this.m_shopSections.Any<ShopSection>((Func<ShopSection, bool>) (s => s.IsResolvingSlotVisuals));

  public bool IsLayoutDirty() => this.m_layoutDirty || this.m_dataDirty;

  public List<ShopSection> GetActiveSections() => this.m_shopSections.Where<ShopSection>((Func<ShopSection, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.isActiveAndEnabled)).ToList<ShopSection>();

  private void ApplyTestData()
  {
    ProductCatalog catalog = StoreManager.Get().Catalog;
    if ((this.m_appliedTestData.HasValue ? this.m_appliedTestData.Value : ProductCatalog.TestDataMode.NO_TEST_DATA) == this.m_testDataMode)
      return;
    this.m_appliedTestData = new ProductCatalog.TestDataMode?(this.m_testDataMode);
    catalog.SetTestDataMode(this.m_testDataMode);
    if (this.HasTestData())
      this.LoadTestData();
    this.m_dataDirty = true;
  }

  private void Update()
  {
    this.ApplyTestData();
    Shop shop = Shop.Get();
    // ISSUE: explicit non-virtual call
    if ((shop != null ? (__nonvirtual (shop.IsOpen()) ? 1 : 0) : 0) != 0 && StoreManager.Get().Catalog.TryRefreshStaleProductAvailability())
      this.m_dataDirty = true;
    if (this.m_loadSectionsSequentially)
    {
      if (this.m_dataDirty && Shop.Get().IsOpen())
        this.StartCoroutine(this.LoadSectionsSequentiallyCoroutine());
    }
    else
    {
      if (this.m_dataDirty && Shop.Get().IsOpen())
        this.BindData();
      if (this.m_layoutDirty && this.AreLayoutsResolved())
        this.StackSections();
    }
    this.UpdateLoadingStats();
  }

  private void BindData()
  {
    this.m_dataDirty = false;
    ShopDataModel dataModel = this.m_widget.GetDataModel<ShopDataModel>();
    if (dataModel == null)
    {
      this.ResizeTierCount(0);
    }
    else
    {
      this.RecordStartLoading();
      this.ResizeTierCount(dataModel.Tiers.Count);
      for (int index = 0; index < this.m_tierInstances.Count; ++index)
      {
        WidgetInstance tierInstance = this.m_tierInstances[index];
        ProductTierDataModel productTierDataModel = dataModel.Tiers.ElementAtOrDefault<ProductTierDataModel>(index);
        tierInstance.BindDataModel((IDataModel) (productTierDataModel ?? ProductFactory.CreateEmptyProductTier()), false);
        if (tierInstance.WillLoadSynchronously)
          tierInstance.Initialize();
      }
      foreach (ShopSection shopSection in this.m_shopSections)
        shopSection.ScheduleRefresh();
      this.m_layoutDirty = true;
    }
  }

  private void ResizeTierCount(int targetCount)
  {
    while (this.m_tierInstances.Count > targetCount)
    {
      WidgetInstance instance = this.m_tierInstances.LastOrDefault<WidgetInstance>();
      this.m_tierInstances.Remove(instance);
      if ((UnityEngine.Object) instance != (UnityEngine.Object) null)
      {
        this.m_shopSections.RemoveAll((Predicate<ShopSection>) (s => (UnityEngine.Object) s == (UnityEngine.Object) null || s.transform.IsChildOf(instance.transform)));
        UnityEngine.Object.Destroy((UnityEngine.Object) instance.gameObject);
      }
    }
    while (this.m_tierInstances.Count < targetCount)
    {
      WidgetInstance widgetInstance = WidgetInstance.Create("ShopMasterTier.prefab:28b5d7137297f234ebe64c4499d41901");
      widgetInstance.SetLayerOverride((GameLayer) this.gameObject.layer);
      widgetInstance.transform.SetParent(this.transform, false);
      widgetInstance.name = string.Format("tier {0}", (object) this.m_tierInstances.Count);
      widgetInstance.WillLoadSynchronously = this.m_loadSynchronously;
      this.m_tierInstances.Add(widgetInstance);
    }
  }

  private void StackSections()
  {
    Log.Store.PrintDebug("Shop ready to stack sections at {0} seconds {1} frames", (object) this.m_timeSpentLoading.TotalSeconds, (object) this.m_framesSpentLoading);
    Widget.TriggerEventParameters parameters = new Widget.TriggerEventParameters()
    {
      IgnorePlaymaker = true,
      NoDownwardPropagation = true
    };
    ShopSection shopSection1 = (ShopSection) null;
    foreach (ShopSection shopSection2 in this.m_shopSections)
    {
      if (!((UnityEngine.Object) shopSection2 == (UnityEngine.Object) null))
      {
        shopSection2.ResizeHeightForStacking();
        shopSection2.gameObject.SetActive(shopSection2.IsElementEnabled);
        if (shopSection2.IsElementEnabled)
        {
          if ((UnityEngine.Object) shopSection1 == (UnityEngine.Object) null)
            shopSection2.Top = 0.0f;
          else
            shopSection2.Top = shopSection1.Bottom - this.m_stackingMargins;
          shopSection2.widget.TriggerEvent("TIER_POSITIONED", parameters);
          shopSection1 = shopSection2;
        }
      }
    }
    this.m_layoutDirty = false;
    this.StartCoroutine(this.LoadSectionSlotsCoroutine());
  }

  private IEnumerator LoadSectionSlotsCoroutine()
  {
    Log.Store.PrintDebug("Shop start loading buttons on all sections at {0} seconds {1} frames", (object) this.m_timeSpentLoading.TotalSeconds, (object) this.m_framesSpentLoading);
    Widget.TriggerEventParameters triggerParams = new Widget.TriggerEventParameters()
    {
      IgnorePlaymaker = true,
      NoDownwardPropagation = true
    };
    int sectionIndex = 0;
    foreach (ShopSection section in this.m_shopSections)
    {
      section.SuppressResolvingSlots = false;
      while (section.IsResolvingSlotVisuals)
      {
        yield return (object) null;
        if ((UnityEngine.Object) section == (UnityEngine.Object) null)
        {
          Log.Store.PrintDebug("Shop ABORTED loading");
          this.RecordStopLoading();
          yield break;
        }
      }
      Log.Store.PrintDebug("Shop finished loading buttons on section {0} at {1} seconds {2} frames", (object) sectionIndex, (object) this.m_timeSpentLoading.TotalSeconds, (object) this.m_framesSpentLoading);
      section.widget.TriggerEvent("TIER_SLOTS_LOADED", triggerParams);
      ++sectionIndex;
    }
    this.RecordStopLoading();
  }

  private bool AreLayoutsResolved() => this.m_shopSections.Count == this.m_tierInstances.Count && !this.m_shopSections.Any<ShopSection>((Func<ShopSection, bool>) (s => s.IsElementEnabled && s.IsResolvingLayout));

  private bool HasTestData() => this.m_testData.Length != 0;

  private void LoadTestData()
  {
    ShopProductData instance = ScriptableObject.CreateInstance<ShopProductData>();
    List<ShopProductData.ProductData> productDataList = new List<ShopProductData.ProductData>();
    List<ShopProductData.ProductItemData> productItemDataList = new List<ShopProductData.ProductItemData>();
    List<ShopProductData.ProductTierData> productTierDataList = new List<ShopProductData.ProductTierData>();
    foreach (ShopProductData shopProductData in this.m_testData)
    {
      productDataList.AddRange((IEnumerable<ShopProductData.ProductData>) shopProductData.productCatalog);
      productItemDataList.AddRange((IEnumerable<ShopProductData.ProductItemData>) shopProductData.productItemCatalog);
      productTierDataList.AddRange((IEnumerable<ShopProductData.ProductTierData>) shopProductData.productTierCatalog);
    }
    instance.productCatalog = productDataList.ToArray();
    instance.productItemCatalog = productItemDataList.ToArray();
    instance.productTierCatalog = productTierDataList.ToArray();
    StoreManager.Get().Catalog.PopulateWithTestData(instance);
    UnityEngine.Object.Destroy((UnityEngine.Object) instance);
  }

  private void HandleShopCloseCompleted()
  {
    if (this.m_loadSectionsSequentially)
    {
      this.StopCoroutine(this.LoadSectionsSequentiallyCoroutine());
      if (this.m_isLoading)
        this.RecordStopLoading();
    }
    this.ResizeTierCount(0);
  }

  private void RecordStartLoading()
  {
    Log.Store.PrintDebug("Shop load start");
    this.m_isLoading = true;
    this.m_framesSpentLoading = 0;
    this.m_timeSpentLoading = new TimeSpan();
    this.m_timeStartedLoading = DateTime.UtcNow;
  }

  private void RecordStopLoading()
  {
    this.m_isLoading = false;
    this.m_timeSpentLoading = DateTime.UtcNow - this.m_timeStartedLoading;
    Log.Store.PrintDebug("Shop load done at {0} seconds {1} frames", (object) this.m_timeSpentLoading.TotalSeconds, (object) this.m_framesSpentLoading);
  }

  private void UpdateLoadingStats()
  {
    if (!this.m_isLoading)
      return;
    this.m_timeSpentLoading = DateTime.UtcNow - this.m_timeStartedLoading;
    ++this.m_framesSpentLoading;
  }

  private IEnumerator LoadSectionsSequentiallyCoroutine()
  {
    ShopBrowser shopBrowser = this;
    shopBrowser.RecordStartLoading();
    shopBrowser.m_dataDirty = false;
    shopBrowser.m_layoutDirty = false;
    foreach (Component component in shopBrowser.m_tierInstances.ToArray())
      component.gameObject.SetActive(false);
    ShopDataModel shopData = shopBrowser.m_widget.GetDataModel<ShopDataModel>();
    if (shopData == null)
    {
      Log.Store.PrintError("Failed to load sections: no shop data model");
      shopBrowser.ResizeTierCount(0);
      shopBrowser.RecordStopLoading();
    }
    else
    {
      shopBrowser.ResizeTierCount(shopData.Tiers.Count);
      ShopSection previousSection = (ShopSection) null;
      int sectionIndex = 0;
      bool aborted = false;
      WidgetInstance[] widgetInstanceArray = shopBrowser.m_tierInstances.ToArray();
      for (int index = 0; index < widgetInstanceArray.Length; ++index)
      {
        WidgetInstance inst = widgetInstanceArray[index];
        if ((UnityEngine.Object) inst == (UnityEngine.Object) null)
        {
          aborted = true;
          break;
        }
        ProductTierDataModel tierData = shopData.Tiers.ElementAtOrDefault<ProductTierDataModel>(sectionIndex) ?? ProductFactory.CreateEmptyProductTier();
        Log.Store.PrintDebug("Loading section {0} with style = {1}, header = {2} at frame = {3}, time = {4}", (object) sectionIndex, (object) tierData.Style, (object) tierData.Header, (object) shopBrowser.m_framesSpentLoading, (object) shopBrowser.m_timeSpentLoading.TotalSeconds);
        ShopSection section = (ShopSection) null;
        yield return (object) shopBrowser.StartCoroutine(shopBrowser.InitSectionCoroutine(sectionIndex, inst, tierData, previousSection, (Action<ShopSection>) (result => section = result)));
        if ((UnityEngine.Object) section == (UnityEngine.Object) null)
        {
          aborted = true;
          break;
        }
        bool populateSucceeded = false;
        yield return (object) shopBrowser.StartCoroutine(shopBrowser.PopulateSectionCoroutine(sectionIndex, section, (Action<bool>) (result => populateSucceeded = result)));
        if (!populateSucceeded)
        {
          aborted = true;
          break;
        }
        if (section.IsElementEnabled)
          previousSection = section;
        ++sectionIndex;
      }
      widgetInstanceArray = (WidgetInstance[]) null;
      if (aborted)
        Log.Store.PrintDebug("Aborted loading sections: ShopSection destroyed");
      shopBrowser.RecordStopLoading();
    }
  }

  private IEnumerator InitSectionCoroutine(
    int sectionIndex,
    WidgetInstance inst,
    ProductTierDataModel tierData,
    ShopSection previousSection,
    Action<ShopSection> onComplete)
  {
    Log.Store.PrintDebug("Loading section {0} with style = {1}, header = {2} at frame = {3}, time = {4}", (object) sectionIndex, (object) tierData.Style, (object) tierData.Header, (object) this.m_framesSpentLoading, (object) this.m_timeSpentLoading.TotalSeconds);
    inst.BindDataModel((IDataModel) tierData, false);
    inst.gameObject.SetActive(true);
    if (inst.WillLoadSynchronously)
      inst.Initialize();
    while (!inst.IsReady)
    {
      yield return (object) null;
      if ((UnityEngine.Object) inst == (UnityEngine.Object) null)
      {
        onComplete((ShopSection) null);
        yield break;
      }
    }
    if ((UnityEngine.Object) inst.Widget == (UnityEngine.Object) null)
    {
      Log.Store.PrintError("Aborted loading sections: tier instance failed to load template");
      onComplete((ShopSection) null);
    }
    else
    {
      ShopSection section = inst.Widget.GetComponent<ShopSection>();
      if ((UnityEngine.Object) section == (UnityEngine.Object) null)
      {
        Log.Store.PrintError("Aborted loading sections: tier widget has no ShopSection component");
        onComplete((ShopSection) null);
      }
      else
      {
        section.gameObject.SetActive(true);
        while (inst.Widget.GetIsChangingStates((Func<GameObject, bool>) (go => (UnityEngine.Object) go.GetComponent<ShopSlot>() == (UnityEngine.Object) null)))
        {
          yield return (object) null;
          if ((UnityEngine.Object) inst == (UnityEngine.Object) null)
          {
            onComplete((ShopSection) null);
            yield break;
          }
        }
        section.ResizeHeightForStacking();
        if ((UnityEngine.Object) previousSection == (UnityEngine.Object) null)
          section.Top = 0.0f;
        else
          section.Top = previousSection.Bottom - this.m_stackingMargins;
        inst.Widget.TriggerEvent("TIER_POSITIONED", new Widget.TriggerEventParameters()
        {
          IgnorePlaymaker = true,
          NoDownwardPropagation = true
        });
        Log.Store.PrintDebug("Finished loading section {0} style at frame = {1}, time = {2}", (object) sectionIndex, (object) this.m_framesSpentLoading, (object) this.m_timeSpentLoading.TotalSeconds);
        onComplete(section);
      }
    }
  }

  private IEnumerator PopulateSectionCoroutine(
    int sectionIndex,
    ShopSection section,
    Action<bool> onComplete)
  {
    section.BindDataModelsToSlots();
    if (!section.IsElementEnabled)
    {
      Log.Store.PrintDebug("Section {0} disabled itself", (object) sectionIndex);
      section.gameObject.SetActive(false);
      onComplete(true);
    }
    else
    {
      while (section.IsResolvingSlotVisuals)
      {
        yield return (object) null;
        if ((UnityEngine.Object) section == (UnityEngine.Object) null)
        {
          onComplete(false);
          yield break;
        }
      }
      section.widget.TriggerEvent("TIER_SLOTS_LOADED");
      if (this.m_popupRoot != null)
        section.EnablePopupRendering(this.m_popupRoot);
      Log.Store.PrintDebug("Finished loading section {0} contents at frame = {1}, time = {2}", (object) sectionIndex, (object) this.m_framesSpentLoading, (object) this.m_timeSpentLoading.TotalSeconds);
      onComplete(true);
    }
  }

  public void EnablePopupRendering(IPopupRoot popupRoot) => this.m_popupRoot = popupRoot;

  public void DisablePopupRendering() => this.m_popupRoot = (IPopupRoot) null;

  public bool HandlesChildPropagation() => true;
}
