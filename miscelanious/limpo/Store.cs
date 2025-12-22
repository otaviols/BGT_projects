using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.UI;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public abstract class Store : UIBPopup, IStore
{
  [CustomEditField(Sections = "Store/UI")]
  public GameObject m_root;
  [CustomEditField(Sections = "Store/UI")]
  public GameObject m_cover;
  [CustomEditField(Sections = "Store/UI")]
  public UIBButton m_buyWithMoneyButton;
  [CustomEditField(Sections = "Store/UI")]
  public TooltipZone m_buyWithMoneyTooltip;
  [CustomEditField(Sections = "Store/UI")]
  public PegUIElement m_buyWithMoneyTooltipTrigger;
  [CustomEditField(Sections = "Store/UI")]
  public UIBButton m_buyWithGoldButton;
  [CustomEditField(Sections = "Store/UI")]
  public TooltipZone m_buyWithGoldTooltip;
  [CustomEditField(Sections = "Store/UI")]
  public PegUIElement m_buyWithGoldTooltipTrigger;
  [CustomEditField(Sections = "Store/UI")]
  public UIBButton m_buyWithVCButton;
  [CustomEditField(Sections = "Store/UI")]
  public TooltipZone m_buyWithVCTooltip;
  [CustomEditField(Sections = "Store/UI")]
  public PegUIElement m_buyWithVCTooltipTrigger;
  [CustomEditField(Sections = "Store/UI")]
  public UIBButton m_infoButton;
  [CustomEditField(Sections = "Store/Materials")]
  public List<MeshRenderer> m_goldButtonMeshes = new List<MeshRenderer>();
  [CustomEditField(Sections = "Store/Materials")]
  public Material m_enabledGoldButtonMaterial;
  [CustomEditField(Sections = "Store/Materials")]
  public Material m_disabledGoldButtonMaterial;
  [CustomEditField(Sections = "Store/Materials")]
  public List<MeshRenderer> m_moneyButtonMeshes = new List<MeshRenderer>();
  [CustomEditField(Sections = "Store/Materials")]
  public Material m_enabledMoneyButtonMaterial;
  [CustomEditField(Sections = "Store/Materials")]
  public Material m_disabledMoneyButtonMaterial;
  [CustomEditField(Sections = "Store/Materials")]
  public List<MeshRenderer> m_vcButtonMeshes = new List<MeshRenderer>();
  [CustomEditField(Sections = "Store/Materials")]
  public Material m_enabledVCButtonMaterial;
  [CustomEditField(Sections = "Store/Materials")]
  public Material m_disabledVCButtonMaterial;
  [CustomEditField(Sections = "Store/UI")]
  public PegUIElement m_offClicker;
  protected bool m_hasRequestedFullscreenFX;
  protected bool m_restoreWhenShopHides;
  private Store.BuyButtonInternal m_goldButtonInternal;
  private Store.BuyButtonInternal m_moneyButtonInternal;
  private Store.BuyButtonInternal m_vcButtonInternal;
  private readonly List<Store.BuyButtonInternal> m_buyButtons = new List<Store.BuyButtonInternal>();
  private readonly List<Store.InfoListener> m_infoListeners = new List<Store.InfoListener>();
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected override void Awake()
  {
    this.m_goldButtonInternal = new Store.BuyButtonInternal()
    {
      m_button = this.m_buyWithGoldButton,
      m_tooltipZone = this.m_buyWithGoldTooltip,
      m_tooltipTrigger = this.m_buyWithGoldTooltipTrigger,
      m_meshes = this.m_goldButtonMeshes,
      m_enabledMaterial = this.m_enabledGoldButtonMaterial,
      m_disabledMaterial = this.m_disabledGoldButtonMaterial,
      m_toolTipHeadlineStringId = "GLUE_STORE_GOLD_BUTTON_TOOLTIP_HEADLINE",
      m_buyHandler = new UIEvent.Handler(this.BuyWithGold),
      m_getOwnedTooltipStringHandler = new Func<string>(this.GetOwnedTooltipString)
    };
    this.m_buyButtons.Add(this.m_goldButtonInternal);
    this.m_moneyButtonInternal = new Store.BuyButtonInternal()
    {
      m_button = this.m_buyWithMoneyButton,
      m_tooltipZone = this.m_buyWithMoneyTooltip,
      m_tooltipTrigger = this.m_buyWithMoneyTooltipTrigger,
      m_meshes = this.m_moneyButtonMeshes,
      m_enabledMaterial = this.m_enabledMoneyButtonMaterial,
      m_disabledMaterial = this.m_disabledMoneyButtonMaterial,
      m_toolTipHeadlineStringId = "GLUE_STORE_MONEY_BUTTON_TOOLTIP_HEADLINE",
      m_buyHandler = new UIEvent.Handler(this.BuyWithMoney),
      m_getOwnedTooltipStringHandler = new Func<string>(this.GetOwnedTooltipString)
    };
    this.m_buyButtons.Add(this.m_moneyButtonInternal);
    this.m_vcButtonInternal = new Store.BuyButtonInternal()
    {
      m_button = this.m_buyWithVCButton,
      m_tooltipZone = this.m_buyWithVCTooltip,
      m_tooltipTrigger = this.m_buyWithVCTooltipTrigger,
      m_meshes = this.m_vcButtonMeshes,
      m_enabledMaterial = this.m_enabledVCButtonMaterial,
      m_disabledMaterial = this.m_disabledVCButtonMaterial,
      m_toolTipHeadlineStringId = "GLUE_STORE_VC_BUTTON_TOOLTIP_HEADLINE",
      m_buyHandler = new UIEvent.Handler(this.BuyWithVirtualCurrency),
      m_getOwnedTooltipStringHandler = new Func<string>(this.GetOwnedTooltipString)
    };
    this.m_buyButtons.Add(this.m_vcButtonInternal);
    base.Awake();
    this.m_infoButton.SetText(GameStrings.Get("GLUE_STORE_INFO_BUTTON_TEXT"));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected override void Start()
  {
    base.Start();
    foreach (Store.BuyButtonInternal buyButton in this.m_buyButtons)
      buyButton.Init();
    this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoPressed));
    NetCache.Get().RegisterGoldBalanceListener(new NetCache.DelGoldBalanceListener(this.OnStoreGoldBalanceChanged));
    StoreManager.Get().RegisterSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchase));
    this.StartCoroutine(this.NotifyListenersWhenReady());
    if (!this.m_shown)
      this.Hide();
    if (!((UnityEngine.Object) Shop.Get() != (UnityEngine.Object) null))
      return;
    Shop.Get().CurrencyBalanceChanged += new Action<CurrencyBalanceChangedEventArgs>(this.OnCurrencyBalanceChangedInternal);
    Shop.Get().OnProductPageChanged += new Action<ProductPage>(this.OnShopProductPageChanged);
  }

  private void OnStoreGoldBalanceChanged(NetCache.NetCacheGoldBalance balance)
  {
    if (!this.IsOpen())
      return;
    this.OnGoldBalanceChanged(balance);
  }

  private void OnCurrencyBalanceChangedInternal(CurrencyBalanceChangedEventArgs args)
  {
    if (!this.IsOpen())
      return;
    this.OnCurrencyBalanceChanged(args);
  }

  private void OnSuccessfulPurchase(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (paymentMethod != PaymentMethod.MONEY || !this.IsOpen())
      return;
    this.OnMoneySpent();
  }

  protected virtual void OnDestroy()
  {
    if (NetCache.Get() != null)
      NetCache.Get().RemoveGoldBalanceListener(new NetCache.DelGoldBalanceListener(this.OnStoreGoldBalanceChanged));
    if ((UnityEngine.Object) Shop.Get() != (UnityEngine.Object) null)
    {
      Shop.Get().CurrencyBalanceChanged -= new Action<CurrencyBalanceChangedEventArgs>(this.OnCurrencyBalanceChangedInternal);
      Shop.Get().OnProductPageChanged -= new Action<ProductPage>(this.OnShopProductPageChanged);
    }
    StoreManager.Get().RemoveSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchase));
    foreach (Store.BuyButtonInternal buyButton in this.m_buyButtons)
      buyButton.OnDestroy();
    this.m_buyButtons.Clear();
    this.m_enabledGoldButtonMaterial = (Material) null;
    this.m_disabledGoldButtonMaterial = (Material) null;
    this.m_enabledMoneyButtonMaterial = (Material) null;
    this.m_disabledMoneyButtonMaterial = (Material) null;
    this.m_enabledVCButtonMaterial = (Material) null;
    this.m_disabledVCButtonMaterial = (Material) null;
    if (FullScreenFXMgr.Get() == null || !this.m_hasRequestedFullscreenFX)
      return;
    this.EnableFullScreenEffects(false);
  }

  public virtual void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance)
  {
  }

  public virtual void OnMoneySpent()
  {
  }

  public virtual void OnCurrencyBalanceChanged(CurrencyBalanceChangedEventArgs args)
  {
  }

  public void Show(bool isTotallyFake, bool useOverlayUI, IDataModel dataModel)
  {
    if (dataModel != null)
    {
      WidgetTemplate componentInParent = this.GetComponentInParent<WidgetTemplate>();
      if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
        componentInParent.BindDataModel(dataModel, this.gameObject);
    }
    this.m_useOverlayUI = useOverlayUI;
    this.StartCoroutine(this.ShowWhenReady(isTotallyFake));
  }

  public void Open() => this.StartCoroutine(this.ShowWhenReady(false));

  public bool IsOpen() => this.IsShown();

  public virtual void Close() => this.Hide();

  public virtual bool IsReady() => true;

  public bool IsCovered() => this.m_cover.activeSelf;

  public void BlockInterface(bool blocked)
  {
    if ((UnityEngine.Object) this.m_cover != (UnityEngine.Object) null)
      this.m_cover.SetActive(blocked);
    this.ForceDisableBuyButtons(blocked);
  }

  public void EnableClickCatcher(bool enabled) => this.m_offClicker.gameObject.SetActive(enabled);

  public event Action<BuyProductEventArgs> OnProductPurchaseAttempt;

  public event Action OnOpened;

  public event Action<StoreClosedArgs> OnClosed;

  public event Action OnReady;

  public event Action OnProductOpened;

  public bool RegisterInfoListener(Store.InfoCallback callback) => this.RegisterInfoListener(callback, (object) null);

  public bool RegisterInfoListener(Store.InfoCallback callback, object userData)
  {
    Store.InfoListener infoListener = new Store.InfoListener();
    infoListener.SetCallback(callback);
    infoListener.SetUserData(userData);
    if (this.m_infoListeners.Contains(infoListener))
      return false;
    this.m_infoListeners.Add(infoListener);
    return true;
  }

  public bool RemoveInfoListener(Store.InfoCallback callback, object userData = null)
  {
    Store.InfoListener infoListener = new Store.InfoListener();
    infoListener.SetCallback(callback);
    infoListener.SetUserData(userData);
    return this.m_infoListeners.Remove(infoListener);
  }

  public void Unload() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  protected virtual void BuyWithGold(UIEvent e)
  {
  }

  protected virtual void BuyWithMoney(UIEvent e)
  {
  }

  protected virtual void BuyWithVirtualCurrency(UIEvent e)
  {
  }

  protected abstract void ShowImpl(bool isTotallyFake);

  protected void FireOpenedEvent()
  {
    if (this.OnOpened == null)
      return;
    this.OnOpened();
  }

  public void FireExitEvent(bool authorizationBackButtonPressed)
  {
    if (this.OnClosed == null)
      return;
    this.OnClosed(new StoreClosedArgs(authorizationBackButtonPressed));
  }

  protected void FireProductOpenedEvent()
  {
    Action onProductOpened = this.OnProductOpened;
    if (onProductOpened == null)
      return;
    onProductOpened();
  }

  protected void EnableFullScreenEffects(bool enable)
  {
    if (enable)
    {
      this.m_hasRequestedFullscreenFX = true;
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 1f
      });
    }
    else
      this.m_screenEffectsHandle.StopEffect();
  }

  protected void FireBuyWithMoneyEvent(Network.Bundle bundle, int quantity)
  {
    if (this.OnProductPurchaseAttempt == null)
      return;
    this.OnProductPurchaseAttempt((BuyProductEventArgs) new BuyPmtProductEventArgs(bundle, CurrencyType.REAL_MONEY, quantity));
  }

  protected void FireBuyWithGoldEventGTAPP(Network.Bundle bundle, int quantity)
  {
    if (this.OnProductPurchaseAttempt == null)
      return;
    this.OnProductPurchaseAttempt((BuyProductEventArgs) new BuyPmtProductEventArgs(bundle, CurrencyType.GOLD, quantity));
  }

  protected void FireBuyWithGoldEventNoGTAPP(NoGTAPPTransactionData noGTAPPTransactionData)
  {
    if (this.OnProductPurchaseAttempt == null)
      return;
    this.OnProductPurchaseAttempt((BuyProductEventArgs) new BuyNoGTAPPEventArgs(noGTAPPTransactionData));
  }

  protected void FireBuyWithVirtualCurrencyEvent(
    Network.Bundle bundle,
    CurrencyType currencyType,
    int quantity = 1)
  {
    if (this.OnProductPurchaseAttempt == null)
      return;
    if ((Record) bundle == (Record) null)
      Log.Store.PrintError("FireBuyWithVirtualCurrencyEvent error: bundle is null");
    else if (currencyType == CurrencyType.NONE)
      Log.Store.PrintError("FireBuyWithVirtualCurrencyEvent error: currency type is None, does the bundle have VC?");
    else if (!ShopUtils.IsVirtualCurrencyEnabled())
      Log.Store.PrintError("FireBuyWithVirtualCurrencyEvent error: Virtual Currency is not enabled.");
    else if (!ShopUtils.IsVirtualCurrencyTypeEnabled(currencyType))
    {
      Log.Store.PrintError("FireBuyWithVirtualCurrencyEvent error: Currency Type is not enabled via VC: {0}", (object) currencyType);
    }
    else
    {
      ProductDataModel productByPmtId = StoreManager.Get().Catalog.GetProductByPmtId(ProductId.CreateFrom(bundle.PMTProductID.GetValueOrDefault()));
      PriceDataModel price = productByPmtId.Prices.FirstOrDefault<PriceDataModel>((Func<PriceDataModel, bool>) (p => p.Currency == currencyType));
      Shop.Get().AttemptToPurchaseProduct(productByPmtId, price, quantity);
    }
  }

  protected void SetGoldButtonState(Store.BuyButtonState state) => this.m_goldButtonInternal.State = state;

  protected Store.BuyButtonState GetGoldButtonState() => this.m_goldButtonInternal.State;

  protected void SetMoneyButtonState(Store.BuyButtonState state) => this.m_moneyButtonInternal.State = state;

  protected Store.BuyButtonState GetMoneyButtonState() => this.m_moneyButtonInternal.State;

  protected void SetVCButtonState(Store.BuyButtonState state) => this.m_vcButtonInternal.State = state;

  protected Store.BuyButtonState GetVCButtonState() => this.m_vcButtonInternal.State;

  private IEnumerator ShowWhenReady(bool isTotallyFake)
  {
    Store store = this;
    VisualController visualController = store.GetComponent<VisualController>();
    while ((UnityEngine.Object) visualController != (UnityEngine.Object) null && visualController.IsChangingStates)
      yield return (object) null;
    store.ShowImpl(isTotallyFake);
  }

  private void ForceDisableBuyButtons(bool forceDisable)
  {
    foreach (Store.BuyButtonInternal buyButton in this.m_buyButtons)
      buyButton.ForceDisabled = forceDisable;
  }

  private IEnumerator NotifyListenersWhenReady()
  {
    while (!this.IsReady())
      yield return (object) null;
    Action onReady = this.OnReady;
    if (onReady != null)
      onReady();
  }

  protected void OnInfoPressed(UIEvent e)
  {
    foreach (Store.InfoListener infoListener in this.m_infoListeners.ToArray())
      infoListener.Fire();
  }

  protected virtual string GetOwnedTooltipString() => GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_TEXT_PURCHASED");

  protected void SilenceBuyButtons()
  {
    foreach (Store.BuyButtonInternal buyButton in this.m_buyButtons)
      buyButton.m_silenceReleaseHandler = true;
  }

  protected virtual void RefreshBuyButtonStates(
    Network.Bundle bundle,
    NoGTAPPTransactionData transaction)
  {
    this.SetMoneyButtonState(Store.DetermineBuyButtonState(bundle, transaction, CurrencyType.REAL_MONEY));
    this.SetGoldButtonState(Store.DetermineBuyButtonState(bundle, transaction, CurrencyType.GOLD));
    CurrencyType currencyPriceType = ShopUtils.GetBundleVirtualCurrencyPriceType(bundle);
    this.SetVCButtonState(Store.DetermineBuyButtonState(bundle, transaction, currencyPriceType));
  }

  protected static Store.BuyButtonState DetermineBuyButtonState(
    Network.Bundle bundle,
    NoGTAPPTransactionData transaction,
    CurrencyType currencyType)
  {
    if (currencyType == CurrencyType.NONE || !StoreManager.Get().IsOpen())
      return Store.BuyButtonState.DISABLED;
    if (currencyType == CurrencyType.REAL_MONEY && !StoreManager.Get().IsBattlePayFeatureEnabled() || currencyType == CurrencyType.GOLD && !StoreManager.Get().IsBuyWithGoldFeatureEnabled() || ShopUtils.IsCurrencyVirtual(currencyType) && !ShopUtils.IsVirtualCurrencyEnabled())
      return Store.BuyButtonState.DISABLED_FEATURE;
    long price;
    if (!ShopUtils.TryGetPriceFromBundleOrTransaction(bundle, transaction, currencyType, out price))
      return Store.BuyButtonState.DISABLED_NO_TOOLTIP;
    if (StoreManager.Get().IsProductAlreadyOwned(bundle))
      return Store.BuyButtonState.DISABLED_OWNED;
    if (!StoreManager.Get().IsBundleAvailableNow(bundle))
      return Store.BuyButtonState.DISABLED_NO_TOOLTIP;
    if (currencyType == CurrencyType.REAL_MONEY || ShopUtils.GetCachedBalance(currencyType) >= price)
      return Store.BuyButtonState.ENABLED;
    switch (currencyType)
    {
      case CurrencyType.GOLD:
        return Store.BuyButtonState.DISABLED_NOT_ENOUGH_GOLD;
      case CurrencyType.CN_RUNESTONES:
      case CurrencyType.ROW_RUNESTONES:
        ProductDataModel currencyProductItem1 = StoreManager.Get().Catalog.VirtualCurrencyProductItem;
        return currencyProductItem1 != null && currencyProductItem1.Availability == ProductAvailability.CAN_PURCHASE ? Store.BuyButtonState.ENABLED : Store.BuyButtonState.DISABLED_NOT_ENOUGH_VC;
      case CurrencyType.CN_ARCANE_ORBS:
        ProductDataModel currencyProductItem2 = StoreManager.Get().Catalog.BoosterCurrencyProductItem;
        return currencyProductItem2 != null && currencyProductItem2.Availability == ProductAvailability.CAN_PURCHASE ? Store.BuyButtonState.ENABLED : Store.BuyButtonState.DISABLED_NOT_ENOUGH_BC;
      default:
        return Store.BuyButtonState.DISABLED;
    }
  }

  protected void BindProductDataModel(Network.Bundle bundle)
  {
    WidgetTemplate componentOnSelfOrParent = GameObjectUtils.GetComponentOnSelfOrParent<WidgetTemplate>(this.transform);
    if (!((UnityEngine.Object) componentOnSelfOrParent != (UnityEngine.Object) null))
      return;
    ProductDataModel productDataModel1 = (ProductDataModel) null;
    if ((Record) bundle != (Record) null)
    {
      long? pmtProductId = bundle.PMTProductID;
      if (pmtProductId.HasValue)
      {
        ProductCatalog catalog = StoreManager.Get().Catalog;
        pmtProductId = bundle.PMTProductID;
        ProductId from = ProductId.CreateFrom(pmtProductId.Value);
        productDataModel1 = catalog.GetProductByPmtId(from);
      }
    }
    ProductDataModel productDataModel2 = productDataModel1 ?? ProductFactory.CreateEmptyProductDataModel();
    componentOnSelfOrParent.BindDataModel((IDataModel) productDataModel2, this.gameObject, overrideChildren: true);
  }

  private void OnShopProductPageChanged(ProductPage page)
  {
    bool flag = (UnityEngine.Object) page != (UnityEngine.Object) null;
    if (flag && this.IsShown())
    {
      this.m_restoreWhenShopHides = true;
      this.Hide();
    }
    else
    {
      if (flag || !this.m_restoreWhenShopHides)
        return;
      this.m_restoreWhenShopHides = false;
      this.StartCoroutine(this.ShowWhenReady(false));
    }
  }

  public IEnumerable<CurrencyType> GetVisibleCurrencies()
  {
    HashSet<CurrencyType> visibleCurrencies = new HashSet<CurrencyType>()
    {
      CurrencyType.GOLD
    };
    CurrencyType currencyType;
    if (ShopUtils.IsVirtualCurrencyEnabled() && ShopUtils.TryGetMainVirtualCurrencyType(out currencyType))
      visibleCurrencies.Add(currencyType);
    return (IEnumerable<CurrencyType>) visibleCurrencies;
  }

  protected enum BuyButtonState
  {
    ENABLED,
    DISABLED_NOT_ENOUGH_GOLD,
    DISABLED_NOT_ENOUGH_VC,
    DISABLED_NOT_ENOUGH_BC,
    DISABLED_FEATURE,
    DISABLED,
    DISABLED_OWNED,
    DISABLED_NO_TOOLTIP,
  }

  private class BuyButtonInternal
  {
    public UIBButton m_button;
    public TooltipZone m_tooltipZone;
    public PegUIElement m_tooltipTrigger;
    public List<MeshRenderer> m_meshes;
    public Material m_enabledMaterial;
    public Material m_disabledMaterial;
    public string m_toolTipHeadlineStringId;
    public UIEvent.Handler m_buyHandler;
    public Func<string> m_getOwnedTooltipStringHandler;
    public bool m_silenceReleaseHandler;
    private Store.BuyButtonState m_state;
    private bool m_forceDisabled;

    public void Init()
    {
      if ((UnityEngine.Object) this.m_button != (UnityEngine.Object) null)
        this.m_button.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnReleased));
      if (!((UnityEngine.Object) this.m_tooltipTrigger != (UnityEngine.Object) null))
        return;
      this.m_tooltipTrigger.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnShowTooltip));
      this.m_tooltipTrigger.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHideTooltip));
    }

    public void OnDestroy()
    {
      if ((UnityEngine.Object) this.m_button != (UnityEngine.Object) null)
        this.m_button.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnReleased));
      if ((UnityEngine.Object) this.m_tooltipTrigger != (UnityEngine.Object) null)
      {
        this.m_tooltipTrigger.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnShowTooltip));
        this.m_tooltipTrigger.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHideTooltip));
      }
      this.m_enabledMaterial = (Material) null;
      this.m_disabledMaterial = (Material) null;
    }

    public Store.BuyButtonState State
    {
      get => this.m_state;
      set
      {
        this.m_state = value;
        this.Refresh();
      }
    }

    public bool ForceDisabled
    {
      set
      {
        this.m_forceDisabled = value;
        this.Refresh();
      }
    }

    private void Refresh()
    {
      bool flag = !this.m_forceDisabled && this.m_state == Store.BuyButtonState.ENABLED;
      Material material = flag ? this.m_enabledMaterial : this.m_disabledMaterial;
      foreach (MeshRenderer mesh in this.m_meshes)
      {
        if ((UnityEngine.Object) mesh != (UnityEngine.Object) null)
          mesh.SetSharedMaterial(material);
      }
      if ((UnityEngine.Object) this.m_button != (UnityEngine.Object) null)
      {
        Collider component = this.m_button.GetComponent<Collider>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.enabled = flag;
      }
      if (!((UnityEngine.Object) this.m_tooltipTrigger != (UnityEngine.Object) null))
        return;
      this.m_tooltipTrigger.gameObject.SetActive(!flag && this.m_state != Store.BuyButtonState.DISABLED_NO_TOOLTIP);
    }

    private void OnReleased(UIEvent e)
    {
      if (this.m_forceDisabled || this.m_state != Store.BuyButtonState.ENABLED || this.m_silenceReleaseHandler)
        return;
      this.m_buyHandler(e);
    }

    private void OnShowTooltip(UIEvent e)
    {
      if (this.m_state == Store.BuyButtonState.ENABLED)
        return;
      this.m_tooltipZone.ShowLayerTooltip(GameStrings.Get(this.m_toolTipHeadlineStringId), this.GetBuyButtonTooltipMessage(this.m_state));
    }

    private void OnHideTooltip(UIEvent e) => this.m_tooltipZone.HideTooltip();

    private string GetBuyButtonTooltipMessage(Store.BuyButtonState state)
    {
      switch (state)
      {
        case Store.BuyButtonState.DISABLED_NOT_ENOUGH_GOLD:
          return GameStrings.Get("GLUE_STORE_FAIL_NOT_ENOUGH_GOLD");
        case Store.BuyButtonState.DISABLED_NOT_ENOUGH_VC:
          CurrencyType currencyType1;
          if (ShopUtils.TryGetMainVirtualCurrencyType(out currencyType1) && (currencyType1 == CurrencyType.CN_RUNESTONES || currencyType1 == CurrencyType.ROW_RUNESTONES))
            return GameStrings.Get("GLUE_STORE_FAIL_NOT_ENOUGH_RUNESTONES");
          Log.Store.PrintError(string.Format("Cannot get relating button state for Not Enough VC as the currency type {0} isn't setup (or shouldn't be)", (object) currencyType1));
          return GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
        case Store.BuyButtonState.DISABLED_NOT_ENOUGH_BC:
          CurrencyType currencyType2;
          if (ShopUtils.TryGetBoosterVirtualCurrencyType(out currencyType2) && currencyType2 == CurrencyType.CN_ARCANE_ORBS)
            return GameStrings.Get("GLUE_STORE_FAIL_NOT_ENOUGH_ARCANE_ORBS");
          Log.Store.PrintError(string.Format("Cannot get relating button state for Not Enough VC as the currency type {0} isn't setup (or shouldn't be)", (object) currencyType2));
          return GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
        case Store.BuyButtonState.DISABLED_FEATURE:
          return GameStrings.Get("GLUE_STORE_DISABLED");
        case Store.BuyButtonState.DISABLED_OWNED:
          return this.m_getOwnedTooltipStringHandler();
        case Store.BuyButtonState.DISABLED_NO_TOOLTIP:
          return string.Empty;
        default:
          return GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
      }
    }
  }

  public delegate void ExitCallback(bool authorizationBackButtonPressed, object userData);

  public delegate void InfoCallback(object userData);

  private class InfoListener : EventListener<Store.InfoCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }
}
