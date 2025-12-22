using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Blizzard.Telemetry.WTCG.Client;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour, IStore
{
  [SerializeField]
  protected UIBScrollable m_browserScroller;
  [SerializeField]
  protected VisualController m_shopStateController;
  [SerializeField]
  protected AsyncReference m_shopBrowserRef;
  [SerializeField]
  protected AsyncReference m_vcPageRef;
  [SerializeField]
  protected AsyncReference m_bcPageRef;
  [SerializeField]
  protected AsyncReference m_productPageContainerRef;
  [SerializeField]
  protected AsyncReference m_quantityPromptRef;
  private static Shop s_instance;
  private ShopBrowser m_browser;
  private WidgetTemplate m_browserWidgetTemplate;
  private VirtualCurrencyPurchasePage m_vcPage;
  private CurrencyConversionPage m_bcPage;
  private ProductPageContainer m_productPageContainer;
  private ProductPage m_currentProductPage;
  private StoreQuantityPrompt m_quantityPrompt;
  private WidgetTemplate m_widget;
  private ShopDataModel m_shopData;
  private Stack<Shop.PurchaseOrder> m_autoPurchaseStack = new Stack<Shop.PurchaseOrder>();
  private Shop.PageReopenInfo m_pageReopenInfo;
  private bool m_suppressBoxOpen;
  private bool m_isOpeningVCPage;
  private bool m_isReopeningPage;
  protected bool m_isOpen;
  private long m_tiersChangeCountAtLastRefresh;
  private Maskable[] m_cameraMasks;
  private bool m_isAnimatingOpenOrClose;
  private Coroutine m_currentPurchaseOrderCoroutine;
  private const string OPEN = "OPEN";
  private const string CLOSED = "CLOSED";
  private const string SHOP_GO_BACK = "SHOP_GO_BACK";
  private const string SHOP_SHOW_INFO = "SHOP_SHOW_INFO";
  private const string SHOP_BUY_VC = "SHOP_BUY_VC";
  private const string SHOP_TOGGLE_AUTOCONVERT = "SHOP_TOGGLE_AUTOCONVERT";
  private const string SHOP_BLOCK_INTERFACE = "SHOP_BLOCK_INTERFACE";
  private const string SHOP_UNBLOCK_INTERFACE = "SHOP_UNBLOCK_INTERFACE";
  private readonly PlatformDependentValue<bool> UnloadUnusedAssetsOnClose = new PlatformDependentValue<bool>(PlatformCategory.Memory)
  {
    LowMemory = true,
    MediumMemory = true,
    HighMemory = false
  };

  protected virtual void Start()
  {
    Shop.s_instance = this;
    this.m_cameraMasks = this.GetComponentsInChildren<Maskable>(true);
    StoreManager storeManager = StoreManager.Get();
    storeManager.RegisterSuccessfulPurchaseListener(new System.Action<Network.Bundle, PaymentMethod>(this.HandleSuccessfulPurchase));
    storeManager.RegisterSuccessfulPurchaseAckListener(new System.Action<Network.Bundle, PaymentMethod>(this.HandleSuccessfulPurchaseAck));
    this.m_shopBrowserRef.RegisterReadyListener<ShopBrowser>((System.Action<ShopBrowser>) (page =>
    {
      this.m_browser = page;
      this.m_browserWidgetTemplate = page.GetComponent<WidgetTemplate>();
    }));
    this.m_vcPageRef.RegisterReadyListener<VirtualCurrencyPurchasePage>((System.Action<VirtualCurrencyPurchasePage>) (page => this.RegisterProductPage<VirtualCurrencyPurchasePage>(page, out this.m_vcPage)));
    this.m_bcPageRef.RegisterReadyListener<CurrencyConversionPage>((System.Action<CurrencyConversionPage>) (page => this.RegisterProductPage<CurrencyConversionPage>(page, out this.m_bcPage)));
    this.m_productPageContainerRef.RegisterReadyListener<ProductPageContainer>((System.Action<ProductPageContainer>) (page =>
    {
      this.m_productPageContainer = page;
      this.m_productPageContainer.OnOpened += new EventHandler(this.HandlePageOpened);
      this.m_productPageContainer.OnClosed += new EventHandler(this.HandlePageClosed);
    }));
    this.m_quantityPromptRef.RegisterReadyListener<StoreQuantityPrompt>((System.Action<StoreQuantityPrompt>) (page => this.m_quantityPrompt = page));
    this.m_shopData = new ShopDataModel();
    Network.Get().OnConnectedToBattleNet += new System.Action<BattleNetErrors>(this.OnBattleNetConnectionStateChanged);
    this.OnBattleNetConnectionStateChanged(BattleNetErrors.ERROR_OK);
    this.m_shopData.GoldBalance = this.GetCurrencyBalanceDataModel(CurrencyType.GOLD);
    this.m_shopData.DustBalance = this.GetCurrencyBalanceDataModel(CurrencyType.DUST);
    this.m_shopData.RenownBalance = this.GetCurrencyBalanceDataModel(CurrencyType.RENOWN);
    this.m_shopData.VirtualCurrency = ProductFactory.CreateEmptyProductDataModel();
    this.m_shopData.BoosterCurrency = ProductFactory.CreateEmptyProductDataModel();
    GlobalDataContext.Get().BindDataModel((IDataModel) this.m_shopData);
    this.m_widget = this.GetComponent<WidgetTemplate>();
    if ((UnityEngine.Object) this.m_widget != (UnityEngine.Object) null)
    {
      this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleWidgetEvent));
      this.m_widget.BindDataModel((IDataModel) this.m_shopData, false);
    }
    NetCache.Get().RegisterGoldBalanceListener(new NetCache.DelGoldBalanceListener(this.HandleGoldBalanceUpdate));
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheArcaneDustBalance), new System.Action(this.HandleDustBalanceUpdate));
    this.CurrencyBalanceChanged += (System.Action<CurrencyBalanceChangedEventArgs>) (_ => this.TryNextAutoPurchase());
    if (StoreManager.Get() != null)
      StoreManager.Get().RegisterAmazingNewShop(this);
    if (this.OnReady == null)
      return;
    this.OnReady();
  }

  private void OnBattleNetConnectionStateChanged(BattleNetErrors bnetErrors)
  {
    CurrencyType currencyType1;
    this.m_shopData.VirtualCurrencyBalance = !ShopUtils.TryGetMainVirtualCurrencyType(out currencyType1) ? this.GetCurrencyBalanceDataModel(CurrencyType.NONE) : this.GetCurrencyBalanceDataModel(currencyType1);
    CurrencyType currencyType2;
    this.m_shopData.BoosterCurrencyBalance = !ShopUtils.TryGetBoosterVirtualCurrencyType(out currencyType2) ? this.GetCurrencyBalanceDataModel(CurrencyType.NONE) : this.GetCurrencyBalanceDataModel(currencyType2);
    if (!BattleNet.IsConnected())
      return;
    System.Action action1 = new System.Action(this.HandleOnCurrencyFirstCached);
    System.Action<CurrencyBalanceChangedEventArgs> action2 = new System.Action<CurrencyBalanceChangedEventArgs>(this.HandleOnCurrencyBalanceChanged);
    IEnumerable<CurrencyCache> allCurrencyCaches = this.GetAllCurrencyCaches(true);
    bool flag = false;
    foreach (CurrencyCache currencyCache in allCurrencyCaches)
    {
      currencyCache.OnFirstCache -= action1;
      currencyCache.OnFirstCache += action1;
      currencyCache.OnBalanceChanged -= action2;
      currencyCache.OnBalanceChanged += action2;
      flag |= currencyCache.IsCached();
    }
    if (!flag)
      return;
    this.HandleOnCurrencyFirstCached();
  }

  protected virtual void OnDestroy()
  {
    if ((UnityEngine.Object) Shop.s_instance == (UnityEngine.Object) this)
      Shop.s_instance = (Shop) null;
    foreach (CurrencyCache allCurrencyCach in this.GetAllCurrencyCaches(true))
    {
      allCurrencyCach.OnFirstCache -= new System.Action(this.HandleOnCurrencyFirstCached);
      allCurrencyCach.OnBalanceChanged -= new System.Action<CurrencyBalanceChangedEventArgs>(this.HandleOnCurrencyBalanceChanged);
    }
    StoreManager storeManager = StoreManager.Get();
    if (storeManager != null)
    {
      storeManager.RemoveSuccessfulPurchaseListener(new System.Action<Network.Bundle, PaymentMethod>(this.HandleSuccessfulPurchase));
      storeManager.RemoveSuccessfulPurchaseAckListener(new System.Action<Network.Bundle, PaymentMethod>(this.HandleSuccessfulPurchaseAck));
    }
    NetCache netCache = NetCache.Get();
    if (netCache != null)
    {
      netCache.RemoveGoldBalanceListener(new NetCache.DelGoldBalanceListener(this.HandleGoldBalanceUpdate));
      netCache.RemoveUpdatedListener(typeof (NetCache.NetCacheArcaneDustBalance), new System.Action(this.HandleDustBalanceUpdate));
    }
    GlobalDataContext.Get().UnbindDataModel(this.m_shopData.DataModelId);
    Network network = Network.Get();
    if (network == null)
      return;
    network.OnConnectedToBattleNet -= new System.Action<BattleNetErrors>(this.OnBattleNetConnectionStateChanged);
    network.OnDisconnectedFromBattleNet -= new System.Action<BattleNetErrors>(this.OnBattleNetConnectionStateChanged);
  }

  protected virtual void Update()
  {
    if (this.IsReadyToRequestVirtualCurrencyBalances())
    {
      CurrencyType currencyType1;
      if (ShopUtils.TryGetMainVirtualCurrencyType(out currencyType1))
        this.RequestVirtualBalanceIfNeeded(currencyType1);
      CurrencyType currencyType2;
      if (ShopUtils.TryGetBoosterVirtualCurrencyType(out currencyType2))
        this.RequestVirtualBalanceIfNeeded(currencyType2);
    }
    if (StoreManager.Get().Catalog.TiersChangeCount == this.m_tiersChangeCountAtLastRefresh || !StoreManager.Get().IsOpen(false))
      return;
    this.RefreshContent();
  }

  public bool IsReady() => (UnityEngine.Object) Shop.s_instance != (UnityEngine.Object) null;

  public static Shop Get() => Shop.s_instance;

  public event System.Action OnOpened;

  public event System.Action OnProductOpened;

  public event System.Action<StoreClosedArgs> OnClosed;

  public event System.Action OnOpenCompleted;

  public event System.Action OnCloseCompleted;

  public event System.Action OnReady;

  public event System.Action<ProductPage> OnProductPageChanged;

  public event System.Action<BuyProductEventArgs> OnProductPurchaseAttempt;

  public event System.Action<CurrencyBalanceChangedEventArgs> CurrencyBalanceChanged;

  public event System.Action OnProductPageClosed;

  public bool IsOpen() => this.m_isOpen;

  public ProductDataModel CurrentProduct { get; private set; }

  public ProductPage CurrentProductPage
  {
    get => this.m_currentProductPage;
    private set
    {
      if ((UnityEngine.Object) this.m_currentProductPage == (UnityEngine.Object) value)
        return;
      this.m_currentProductPage = value;
      if (this.OnProductPageChanged == null)
        return;
      this.OnProductPageChanged(this.m_currentProductPage);
    }
  }

  public ShopDataModel ShopData => this.m_shopData;

  public ShopBrowser Browser => this.m_browser;

  private PriceDataModel GetCurrencyBalanceDataModel(CurrencyType currency) => this.GetCurrencyCache(currency).PriceDataModel;

  private bool IsCloseDisabled()
  {
    HearthstoneCheckout service;
    return ServiceManager.TryGet<HearthstoneCheckout>(out service) && service.IsInProgress || !StoreManager.Get().CanTapOutConfirmationUI();
  }

  public void Open()
  {
    if (this.m_isOpen)
      return;
    this.m_isOpen = true;
    ShownUIMgr.Get().SetShownUI(ShownUIMgr.UI_WINDOW.GENERAL_STORE);
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.STORE);
    StoreManager.Get().Catalog.TryRefreshStaleProductAvailability();
    this.RefreshContent();
    this.gameObject.SetActive(true);
    this.m_browser.gameObject.SetActive(!this.m_suppressBoxOpen);
    this.EnsureShopMovedToOverlayUI();
    if (!this.m_suppressBoxOpen)
    {
      this.m_isAnimatingOpenOrClose = true;
      UIContext.GetRoot().ShowPopup(this.m_widget.gameObject, projection: UIContext.ProjectionType.Perspective);
      this.SetMasking(true);
      this.m_shopStateController.SetState("OPEN");
      this.UpdateScrollerEnabled();
      this.StartCoroutine(this.SendShopVisitTelemetry());
    }
    if (this.OnOpened == null)
      return;
    this.OnOpened();
  }

  public void Close() => this.Close(false);

  public void Close(bool forceClose)
  {
    if (!forceClose && this.IsCloseDisabled() || !this.m_isOpen)
      return;
    if ((UnityEngine.Object) this.m_productPageContainer != (UnityEngine.Object) null)
      this.m_productPageContainer.Close();
    if ((UnityEngine.Object) this.m_vcPage != (UnityEngine.Object) null && this.m_vcPage.IsOpen)
      this.m_vcPage.Close();
    this.CurrentProduct = (ProductDataModel) null;
    this.CancelAutoPurchases();
    this.MarkVirtualCurrencyDirty();
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    if (ShownUIMgr.Get() != null)
      ShownUIMgr.Get().ClearShownUI();
    PresenceMgr.Get().SetPrevStatus();
    this.m_isOpen = false;
    if (!this.m_suppressBoxOpen)
    {
      this.m_isAnimatingOpenOrClose = true;
      this.m_shopStateController.SetState("CLOSED");
      this.OnCloseCompleted -= new System.Action(this.DismissPopup);
      this.OnCloseCompleted += new System.Action(this.DismissPopup);
    }
    this.m_suppressBoxOpen = false;
    this.UpdateScrollerEnabled();
    if (this.OnClosed == null)
      return;
    this.OnClosed(new StoreClosedArgs());
  }

  private void EnsureShopMovedToOverlayUI()
  {
    OverlayUI overlayUi = OverlayUI.Get();
    if (!(bool) (UnityEngine.Object) overlayUi || !(bool) (UnityEngine.Object) this.m_widget || overlayUi.HasObject(this.m_widget.gameObject))
      return;
    overlayUi.AddGameObject(this.m_widget.gameObject);
  }

  private void DismissPopup()
  {
    this.SetMasking(false);
    UIContext.GetRoot().DismissPopup(this.m_widget.gameObject);
    this.OnCloseCompleted -= new System.Action(this.DismissPopup);
  }

  public void BlockInterface(bool blocked)
  {
    if (blocked)
      this.m_widget.TriggerEvent("SHOP_BLOCK_INTERFACE", new Widget.TriggerEventParameters()
      {
        IgnorePlaymaker = true,
        NoDownwardPropagation = false
      });
    else
      this.m_widget.TriggerEvent("SHOP_UNBLOCK_INTERFACE", new Widget.TriggerEventParameters()
      {
        IgnorePlaymaker = true,
        NoDownwardPropagation = false
      });
  }

  public bool CanSafelyOpenCurrencyPage()
  {
    if (this.m_vcPage.IsAnimating)
    {
      Log.Store.PrintDebug("Cannot open currency page while VC page is still animating.");
      return false;
    }
    if (this.m_bcPage.IsAnimating)
    {
      Log.Store.PrintDebug("Cannot open currency page while BC page is still animating.");
      return false;
    }
    if (PopupDisplayManager.Get() != null && PopupDisplayManager.Get().IsShowing)
    {
      Log.Store.PrintDebug("Cannot open currency page while PopupDisplayManager is showing popup.");
      return false;
    }
    if (StoreManager.Get() == null || !StoreManager.Get().IsPromptShowing)
      return true;
    Log.Store.PrintDebug("Cannot open currency page while StoreManager is showing popup.");
    return false;
  }

  public void OpenVirtualCurrencyPurchase(float desiredPurchaseAmount = 0.0f, bool rememberLastPage = false)
  {
    string errorMessage;
    if (!this.ValidateReadinessForCurrencyPurchase(out errorMessage))
    {
      Log.Store.PrintError(errorMessage);
    }
    else
    {
      this.EnsureShopMovedToOverlayUI();
      this.m_isOpeningVCPage = true;
      this.CleanUpPagesForCurrencyPage(rememberLastPage);
      UIContext.GetRoot().ShowPopup(this.m_vcPage.gameObject, projection: UIContext.ProjectionType.Perspective);
      this.m_vcPage.OpenToSKU(desiredPurchaseAmount, rememberLastPage);
    }
  }

  public void OpenVirtualCurrencyPurchase(
    ProductDataModel vcVariant,
    ProductDataModel vcVariantToMarkAsSeen = null,
    bool rememberLastPage = false)
  {
    string errorMessage;
    if (!this.ValidateReadinessForCurrencyPurchase(out errorMessage))
    {
      Log.Store.PrintError(errorMessage);
    }
    else
    {
      this.EnsureShopMovedToOverlayUI();
      this.m_isOpeningVCPage = true;
      this.CleanUpPagesForCurrencyPage(rememberLastPage);
      UIContext.GetRoot().ShowPopup(this.m_vcPage.gameObject, projection: UIContext.ProjectionType.Perspective);
      if (vcVariantToMarkAsSeen != null)
        this.MarkProductAsSeen(vcVariantToMarkAsSeen);
      this.m_vcPage.OpenToSKU(vcVariant, rememberLastPage);
    }
  }

  public void OpenBoosterCurrencyPurchase(float desiredPurchaseAmount = 0.0f, bool rememberLastPage = false)
  {
    CurrencyType currencyType;
    if (!ShopUtils.TryGetMainVirtualCurrencyType(out currencyType))
      Log.Store.PrintError("Cannot get BC type used in this region.");
    else if (this.m_shopData.BoosterCurrency == null || this.m_shopData.BoosterCurrency == ProductFactory.CreateEmptyProductDataModel())
      Log.Store.PrintError("No valid BC product received.");
    else if (this.m_shopData.BoosterCurrency.Availability != ProductAvailability.CAN_PURCHASE)
      Log.Store.PrintError("BC not available for purchase");
    else if (this.m_bcPage.IsOpen)
    {
      Log.Store.PrintDebug("Cannot open BC purchase page while already open.");
    }
    else
    {
      CurrencyCache currencyCache = this.GetCurrencyCache(currencyType);
      if (currencyCache.NeedsRefresh())
      {
        if (!((UnityEngine.Object) DialogManager.Get() != (UnityEngine.Object) null))
          return;
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_text = currencyCache.HasError() ? GameStrings.Format("GLUE_STORE_FAIL_CURRENCY_BALANCE") : GameStrings.Format("GLUE_STORE_UPDATING_CURRENCY_BALANCE"),
          m_showAlertIcon = true,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK
        };
        DialogManager.Get().ShowPopup(info);
      }
      else if (currencyType == CurrencyType.CN_ARCANE_ORBS && (double) ShopUtils.GetCachedBalance(CurrencyType.CN_ARCANE_ORBS) + (double) ShopUtils.GetAmountOfCurrencyInProduct(this.m_shopData.BoosterCurrency, CurrencyType.CN_ARCANE_ORBS) > 9999.0)
      {
        if (!((UnityEngine.Object) DialogManager.Get() != (UnityEngine.Object) null))
          return;
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Format("GLUE_ARCANE_ORBS_CAP_HEADER"),
          m_text = GameStrings.Format("GLUE_ARCANE_ORBS_CAP_BODY", (object) 9999),
          m_showAlertIcon = true,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK
        });
      }
      else
      {
        this.CleanUpPagesForCurrencyPage(rememberLastPage);
        if (!((UnityEngine.Object) this.m_bcPage != (UnityEngine.Object) null))
          return;
        this.EnsureShopMovedToOverlayUI();
        this.m_bcPage.OpenToSKU(desiredPurchaseAmount);
      }
    }
  }

  public static void OpenToProductPageWhenReady(
    long pmtProductId,
    bool suppressBox,
    ShopType shopType = ShopType.GENERAL_STORE)
  {
    Shop.s_instance.m_suppressBoxOpen = suppressBox;
    Processor.QueueJob("OpenToProductPage", Shop.Job_OpenToProductPage(pmtProductId, shopType));
  }

  public static void OpenToMercProductPageWhenReady(long pmtProductId, bool suppressBox)
  {
    Shop.s_instance.m_suppressBoxOpen = suppressBox;
    Processor.QueueJob("OpenToProductPage", Shop.Job_OpenToProductPage(pmtProductId, ShopType.MERCENARIES_STORE));
  }

  public static void OpenToSeasonPassPageWhenReady(Global.RewardTrackType trackType)
  {
    ShopType shopType = ShopType.GENERAL_STORE;
    string tag;
    switch (trackType)
    {
      case Global.RewardTrackType.GLOBAL:
        tag = "tavern_pass";
        break;
      case Global.RewardTrackType.BATTLEGROUNDS:
        tag = "battlegrounds_season_pass";
        shopType = ShopType.BATTLEGROUNDS_STORE;
        break;
      default:
        Debug.LogError((object) "Attempted to open invalid season pass.");
        Shop.OpenTavernPassErrorPopup();
        return;
    }
    long pmtId;
    if (StoreManager.Get().Catalog.TryGetPmtIdWithTagContainingType(tag, RewardItemType.PROGRESSION_BONUS, out pmtId))
      Shop.OpenToProductPageWhenReady(pmtId, true, shopType);
    else
      Shop.OpenTavernPassErrorPopup(trackType);
  }

  public static void OpenTavernPassErrorPopup(Global.RewardTrackType trackType = Global.RewardTrackType.NONE)
  {
    AlertPopup.PopupInfo info;
    if (trackType == Global.RewardTrackType.BATTLEGROUNDS)
      info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_PROGRESSION_BATTLEGROUNDS_BONUS_ERROR_HEADER"),
        m_text = GameStrings.Get("GLUE_PROGRESSION_BATTLEGROUNDS_BONUS_ERROR_BODY"),
        m_showAlertIcon = false,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
    else
      info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_PROGRESSION_BONUS_ERROR_HEADER"),
        m_text = GameStrings.Get("GLUE_PROGRESSION_BONUS_ERROR_BODY"),
        m_showAlertIcon = false,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
    DialogManager.Get().ShowPopup(info);
  }

  internal void OpenProductPage(ProductDataModel product, ProductDataModel variant = null)
  {
    System.Action onProductOpened = this.OnProductOpened;
    if (onProductOpened != null)
      onProductOpened();
    if (product == null || product == ProductFactory.CreateEmptyProductDataModel())
    {
      Log.Store.PrintError("Shop cannot open null or empty product");
    }
    else
    {
      Log.Store.PrintDebug("[Shop.OpenProductPage] display product {0}", (object) product.Name);
      this.CurrentProduct = product;
      if ((UnityEngine.Object) this.m_productPageContainer != (UnityEngine.Object) null)
      {
        this.m_productPageContainer.InitializeTempInstances();
        this.m_productPageContainer.Open(product, variant);
      }
      this.MarkProductAsSeen(product);
    }
  }

  public void AttemptToPurchaseProduct(
    ProductDataModel product,
    PriceDataModel price,
    int quantity = 1)
  {
    this.AttemptToPurchaseProduct(product, price, quantity, false);
  }

  public void AttemptToPurchaseProduct(
    ProductDataModel product,
    PriceDataModel price,
    int quantity,
    bool suppressConfirmSpendVC)
  {
    this.CancelAutoPurchases();
    Shop.PurchaseOrder pendingPurchase = new Shop.PurchaseOrder()
    {
      m_product = product,
      m_price = price,
      m_quantity = quantity
    };
    if (ShopUtils.IsCurrencyVirtual(price.Currency) && !ShopUtils.IsVirtualCurrencyTypeEnabled(price.Currency))
    {
      Log.Store.PrintError(string.Format("Cannot start purchase for item with VC price when it is not enabled - {0}", (object) price.Currency));
    }
    else
    {
      while (pendingPurchase != null)
      {
        this.m_autoPurchaseStack.Push(pendingPurchase);
        pendingPurchase = this.GetPrerequisitePurchase(pendingPurchase);
        if (pendingPurchase != null)
        {
          if (pendingPurchase.m_product == null)
          {
            Log.Store.PrintError("Purchase could not be started");
            return;
          }
        }
        else
          break;
      }
      this.ExecutePurchaseOrder(this.m_autoPurchaseStack.Pop(), suppressConfirmSpendVC);
    }
  }

  public void RefreshWallet()
  {
    this.UpdateCurrencyBalance(CurrencyType.GOLD, ShopUtils.GetCachedBalance(CurrencyType.GOLD));
    this.UpdateCurrencyBalance(CurrencyType.DUST, ShopUtils.GetCachedBalance(CurrencyType.DUST));
    this.UpdateCurrencyBalance(CurrencyType.RENOWN, ShopUtils.GetCachedBalance(CurrencyType.RENOWN));
  }

  public void DisplayCurrencyBalance(CurrencyType currency, long balance) => this.GetCurrencyCache(currency).UpdateDisplayText(balance.ToString());

  public void Unload() => this.Close(true);

  public StoreQuantityPrompt QuantityPrompt => this.m_quantityPrompt;

  public bool WillAutoPurchase() => this.m_autoPurchaseStack.Count > 0;

  public IEnumerable<CurrencyType> GetVisibleCurrencies()
  {
    HashSet<CurrencyType> visibleCurrencies = new HashSet<CurrencyType>()
    {
      CurrencyType.GOLD
    };
    if (ShopUtils.IsVirtualCurrencyEnabled())
    {
      CurrencyType currencyType1;
      if (ShopUtils.TryGetMainVirtualCurrencyType(out currencyType1))
        visibleCurrencies.Add(currencyType1);
      CurrencyType currencyType2;
      if (ShopUtils.TryGetBoosterVirtualCurrencyType(out currencyType2))
        visibleCurrencies.Add(currencyType2);
    }
    return (IEnumerable<CurrencyType>) visibleCurrencies;
  }

  private bool ValidateReadinessForCurrencyPurchase(out string errorMessage)
  {
    if (!ShopUtils.TryGetMainVirtualCurrencyType(out CurrencyType _))
    {
      errorMessage = "Cannot get VC type used in this region.";
      return false;
    }
    if (this.m_shopData.VirtualCurrency == null)
    {
      errorMessage = "No valid VC products received.";
      return false;
    }
    if (this.m_shopData.VirtualCurrency.Availability != ProductAvailability.CAN_PURCHASE)
    {
      errorMessage = "VC product is not available for purchase";
      return false;
    }
    if ((UnityEngine.Object) this.m_vcPage == (UnityEngine.Object) null)
    {
      errorMessage = "VC purchase page is not set";
      return false;
    }
    if (this.m_vcPage.IsOpen)
    {
      errorMessage = "Cannot open VC purchase page while already open.";
      return false;
    }
    errorMessage = (string) null;
    return true;
  }

  private void MarkProductAsSeen(ProductDataModel product)
  {
    if (!product.Tags.Remove("new"))
      return;
    List<string> stringList = new List<string>((IEnumerable<string>) Options.Get().GetString(Option.LATEST_SEEN_SHOP_PRODUCT_LIST).Split(':'));
    string str = product.PmtId.ToString();
    if (!stringList.Contains(str))
      stringList.Add(str);
    List<ProductTierDataModel> tiersAll = StoreManager.Get().Catalog.GetTiers_All();
    if (tiersAll.Any<ProductTierDataModel>((Func<ProductTierDataModel, bool>) (t => t.BrowserButtons.Count > 0)))
    {
      for (int index = 0; index < stringList.Count; ++index)
      {
        long pmtId = 0;
        if (!long.TryParse(stringList[index], out pmtId) || !tiersAll.Any<ProductTierDataModel>((Func<ProductTierDataModel, bool>) (t => t.BrowserButtons.Any<ShopBrowserButtonDataModel>((Func<ShopBrowserButtonDataModel, bool>) (b => b.DisplayProduct.PmtId == pmtId)))))
          stringList.RemoveAt(index--);
      }
    }
    string val = string.Join(":", stringList.ToArray());
    Options.Get().SetString(Option.LATEST_SEEN_SHOP_PRODUCT_LIST, val);
  }

  private bool ContainsNewlyDisplayedItems()
  {
    if (StoreManager.Get() == null || StoreManager.Get().IsVintageStoreEnabled() || (UnityEngine.Object) Box.Get() == (UnityEngine.Object) null || !GameUtils.IsAnyTutorialComplete())
      return false;
    List<string> listOfNewProducts = this.GetListOfNewProducts();
    string str1 = Options.Get().GetString(Option.LATEST_DISPLAYED_SHOP_PRODUCT_LIST);
    List<string> stringList = new List<string>();
    stringList.AddRange((IEnumerable<string>) str1.Split(':'));
    foreach (string str2 in listOfNewProducts)
    {
      if (!stringList.Contains(str2))
        return true;
    }
    return false;
  }

  private void MarkShopAsSeen()
  {
    if (!this.m_shopData.HasNewItems)
      return;
    this.m_shopData.HasNewItems = false;
    string val = string.Join(":", this.GetListOfNewProducts().ToArray());
    Options.Get().SetString(Option.LATEST_DISPLAYED_SHOP_PRODUCT_LIST, val);
  }

  private List<string> GetListOfNewProducts()
  {
    List<string> productIds = new List<string>();
    this.m_shopData.Tiers.ForEach<ProductTierDataModel>((System.Action<ProductTierDataModel>) (t => t.BrowserButtons.ForEach<ShopBrowserButtonDataModel>((System.Action<ShopBrowserButtonDataModel>) (button =>
    {
      if (!button.DisplayProduct.Tags.Contains("new"))
        return;
      productIds.Add(button.DisplayProduct.PmtId.ToString());
    }))));
    return productIds;
  }

  private void ExecutePurchaseOrder(Shop.PurchaseOrder purchase, bool suppressConfirmSpendVC)
  {
    if (this.m_currentPurchaseOrderCoroutine != null)
    {
      Log.Store.PrintError("Stopping purchase execution - A purchase order has already been executed");
    }
    else
    {
      IEnumerator enumerator = this.Internal_ExecutePurchaseOrder(purchase, suppressConfirmSpendVC);
      if (purchase.m_hasFinished)
        return;
      this.m_currentPurchaseOrderCoroutine = this.StartCoroutine(enumerator);
    }
  }

  private void StopCurrentExecutedPurchaseOrder(string error = null)
  {
    if (this.m_currentPurchaseOrderCoroutine != null)
    {
      this.StopCoroutine(this.m_currentPurchaseOrderCoroutine);
      this.m_currentPurchaseOrderCoroutine = (Coroutine) null;
    }
    if (error != null)
      Log.Store.PrintError("Executed purchase was stopped unexpectedly - Reason: " + error);
    this.CancelAutoPurchases();
  }

  private IEnumerator Internal_ExecutePurchaseOrder(
    Shop.PurchaseOrder purchase,
    bool suppressConfirmSpendVC)
  {
    Shop shop1 = this;
    if (purchase == null || purchase.m_product == null || purchase.m_price == null)
    {
      if (purchase != null)
        purchase.m_hasFinished = true;
      shop1.StopCurrentExecutedPurchaseOrder("PurchaseOrder invalid.");
    }
    else if (shop1.OnProductPurchaseAttempt == null)
    {
      purchase.m_hasFinished = true;
      shop1.StopCurrentExecutedPurchaseOrder("No OnProductPurchaseAttempt event handler registered.");
    }
    else
    {
      BuyProductEventArgs args = purchase.m_product.GetBuyProductArgs(purchase.m_price, purchase.m_quantity);
      if (args == null)
      {
        purchase.m_hasFinished = true;
        shop1.StopCurrentExecutedPurchaseOrder("No valid BuyProductEventArgs for product");
      }
      else if ((purchase.m_product.Tags.Contains("row_runestones") ? 1 : (purchase.m_product.Tags.Contains("cn_runestones") ? 1 : 0)) != 0 && shop1.m_autoPurchaseStack.Count > 0)
      {
        Shop shop = shop1;
        Shop.PurchaseOrder nextBuyWithVC = shop1.m_autoPurchaseStack.LastOrDefault<Shop.PurchaseOrder>((Func<Shop.PurchaseOrder, bool>) (p => p.m_price != null && ShopUtils.IsMainVirtualCurrencyType(p.m_price.Currency)));
        if (nextBuyWithVC == null)
        {
          Log.Store.PrintError("Unnecessary VC purchase planned; skipping");
          yield return (object) shop1.Internal_ExecutePurchaseOrder(shop1.m_autoPurchaseStack.Pop(), suppressConfirmSpendVC);
        }
        else
        {
          AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
          {
            m_showAlertIcon = false,
            m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
            m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
            m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NOT_NOW"),
            m_alertTextAlignment = UberText.AlignmentOptions.Center,
            m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
            m_headerText = GameStrings.Get("GLUE_SHOP_GET_MORE_RUNESTONES_HEADER"),
            m_text = GameStrings.Get("GLUE_SHOP_GET_MORE_RUNESTONES"),
            m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
            {
              // ISSUE: explicit non-virtual call
              __nonvirtual (shop.BlockInterface(false));
              if (response != AlertPopup.Response.CONFIRM)
                return;
              shop.OpenVirtualCurrencyPurchase((float) shop.GetDeficitForPurchase(nextBuyWithVC), true);
            })
          };
          shop1.CancelAutoPurchases();
          // ISSUE: explicit non-virtual call
          __nonvirtual (shop1.BlockInterface(true));
          DialogManager.Get().ShowPopup(info);
          purchase.m_hasFinished = true;
          shop1.StopCurrentExecutedPurchaseOrder();
        }
      }
      else
      {
        if (purchase.m_price.Currency == CurrencyType.ROW_RUNESTONES && !suppressConfirmSpendVC)
        {
          string str = GameStrings.Get(purchase.m_product.Name ?? string.Empty);
          AlertPopup.Response? nullableResponse = new AlertPopup.Response?();
          AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
          {
            m_showAlertIcon = false,
            m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
            m_confirmText = GameStrings.Get("GLOBAL_CONFIRM"),
            m_cancelText = GameStrings.Get("GLOBAL_CANCEL"),
            m_alertTextAlignment = UberText.AlignmentOptions.Center,
            m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
            m_headerText = GameStrings.Get("GLUE_SHOP_RUNESTONES_PURCHASE_WARNING_HEADER"),
            m_text = GameStrings.Format("GLUE_SHOP_RUNESTONES_PURCHASE_WARNING", (object) str),
            m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => nullableResponse = new AlertPopup.Response?(response))
          };
          // ISSUE: explicit non-virtual call
          __nonvirtual (shop1.BlockInterface(true));
          DialogManager.Get().ShowPopup(info);
          while (!nullableResponse.HasValue)
            yield return (object) null;
          // ISSUE: explicit non-virtual call
          __nonvirtual (shop1.BlockInterface(false));
          if (nullableResponse.Value != AlertPopup.Response.CONFIRM)
          {
            purchase.m_hasFinished = true;
            shop1.StopCurrentExecutedPurchaseOrder(nullableResponse.Value != AlertPopup.Response.CANCEL ? string.Format("Unknown response value - {0} at ROW-VC confirmation popup", (object) nullableResponse.Value) : (string) null);
            yield break;
          }
        }
        if (purchase.m_product.Items.Any<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.Mercenary != null)))
        {
          List<RewardItemDataModel> list = purchase.m_product.Items.Where<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.ItemType == RewardItemType.MERCENARY_KNOCKOUT_SPECIFIC)).ToList<RewardItemDataModel>();
          bool flag = false;
          if (list.Count > 0)
          {
            for (int index = 0; index < list.Count; ++index)
            {
              int mercenaryId = list[index].Mercenary.MercenaryId;
              int num1 = GameDbf.LettuceMercenary.GetRecord(mercenaryId).MercenaryArtVariations.SelectMany<MercenaryArtVariationDbfRecord, MercenaryArtVariationPremiumDbfRecord>((Func<MercenaryArtVariationDbfRecord, IEnumerable<MercenaryArtVariationPremiumDbfRecord>>) (art => art.MercenaryArtVariationPremiums.Where<MercenaryArtVariationPremiumDbfRecord>((Func<MercenaryArtVariationPremiumDbfRecord, bool>) (premium => premium.Collectible)))).Count<MercenaryArtVariationPremiumDbfRecord>();
              LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenaryId);
              int num2 = mercenary != null ? mercenary.m_artVariations.Count : 0;
              if (num1 == num2)
              {
                flag = true;
                break;
              }
            }
          }
          if (flag)
          {
            AlertPopup.Response? nullableResponse = new AlertPopup.Response?();
            AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
            {
              m_showAlertIcon = false,
              m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
              m_confirmText = GameStrings.Get("GLOBAL_CONFIRM"),
              m_cancelText = GameStrings.Get("GLOBAL_CANCEL"),
              m_alertTextAlignment = UberText.AlignmentOptions.Center,
              m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
              m_headerText = GameStrings.Get("GLUE_LETTUCE_OWNED_MERCENARY_WARNING_TITLE"),
              m_text = GameStrings.Get("GLUE_LETTUCE_OWNED_MERCENARY_WARNING_DESC"),
              m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => nullableResponse = new AlertPopup.Response?(response))
            };
            // ISSUE: explicit non-virtual call
            __nonvirtual (shop1.BlockInterface(true));
            DialogManager.Get().ShowPopup(info);
            while (!nullableResponse.HasValue)
              yield return (object) null;
            // ISSUE: explicit non-virtual call
            __nonvirtual (shop1.BlockInterface(false));
            if (nullableResponse.Value != AlertPopup.Response.CONFIRM)
            {
              purchase.m_hasFinished = true;
              shop1.StopCurrentExecutedPurchaseOrder(nullableResponse.Value != AlertPopup.Response.CANCEL ? string.Format("Unknown response value - {0} at duplicate merc popup", (object) nullableResponse.Value) : (string) null);
              yield break;
            }
          }
        }
        purchase.m_hasFinished = true;
        shop1.m_currentPurchaseOrderCoroutine = (Coroutine) null;
        shop1.OnProductPurchaseAttempt(args);
      }
    }
  }

  private void CleanUpPagesForCurrencyPage(bool rememberLastPage)
  {
    if (!rememberLastPage)
      this.m_pageReopenInfo.Clear();
    this.CloseCurrentPage(rememberLastPage);
  }

  private void CloseCurrentPage(bool reopenLater)
  {
    ProductPage currentProductPage = this.CurrentProductPage;
    if (!(bool) (UnityEngine.Object) currentProductPage)
      return;
    if ((UnityEngine.Object) currentProductPage == (UnityEngine.Object) this.m_vcPage || (UnityEngine.Object) currentProductPage == (UnityEngine.Object) this.m_bcPage)
    {
      Shop.PageReopenInfo pageReopenInfo = Shop.PageReopenInfo.Capture(currentProductPage);
      currentProductPage.Close();
      if (currentProductPage.GetComponent<IPopupRoot>() != null)
        UIContext.GetRoot().DismissPopup(currentProductPage.gameObject);
      if (!reopenLater)
        return;
      this.m_pageReopenInfo = pageReopenInfo;
    }
    else
    {
      if (!((UnityEngine.Object) currentProductPage == (UnityEngine.Object) this.m_productPageContainer.GetCurrentProductPage()))
        return;
      Shop.PageReopenInfo pageReopenInfo = Shop.PageReopenInfo.Capture(this.m_productPageContainer);
      this.m_productPageContainer.Close();
      if (!reopenLater)
        return;
      this.m_pageReopenInfo = pageReopenInfo;
    }
  }

  private IEnumerator ReopenClosedPageAfterAFrame(Shop.PageReopenInfo pageReopeInfo)
  {
    this.m_isReopeningPage = true;
    bool flag;
    try
    {
      yield return (object) new WaitForEndOfFrame();
      if (!this.IsOpen())
        flag = false;
      else if (!pageReopeInfo.IsValid)
      {
        flag = false;
      }
      else
      {
        pageReopeInfo.ReopenPage();
        yield break;
      }
    }
    finally
    {
      this.m_isReopeningPage = false;
    }
    return flag;
  }

  private void ReopenClosedPage()
  {
    if (!this.m_pageReopenInfo.IsValid)
      return;
    this.StartCoroutine(this.ReopenClosedPageAfterAFrame(this.m_pageReopenInfo));
    this.m_pageReopenInfo.Clear();
  }

  private long GetDeficitForPurchase(Shop.PurchaseOrder purchase) => ShopUtils.GetDeficit(new PriceDataModel()
  {
    Currency = purchase.m_price.Currency,
    Amount = purchase.m_price.Amount * (float) purchase.m_quantity
  });

  private void HandleWidgetEvent(string eventName)
  {
    if (!(eventName == "SHOP_GO_BACK"))
    {
      if (!(eventName == "SHOP_SHOW_INFO"))
      {
        if (!(eventName == "SHOP_TOGGLE_AUTOCONVERT"))
        {
          if (!(eventName == "SHOP_BUY_VC"))
            return;
          this.OpenVirtualCurrencyPurchase(rememberLastPage: true);
        }
        else
        {
          this.m_shopData.AutoconvertCurrency = !this.m_shopData.AutoconvertCurrency;
          Options.Get().SetBool(Option.AUTOCONVERT_VIRTUAL_CURRENCY, this.m_shopData.AutoconvertCurrency);
        }
      }
      else
        StoreManager.Get().ShowStoreInfo();
    }
    else if (this.IsOpen() && !Navigation.BackStackContainsHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack)))
      this.Close();
    else
      Navigation.GoBack();
  }

  private void RefreshContent()
  {
    this.m_browserScroller.SetScroll(0.0f);
    this.RefreshDataModel();
    if (!((UnityEngine.Object) this.m_browser != (UnityEngine.Object) null))
      return;
    this.m_browser.RefreshContents();
  }

  private void CompleteOpen()
  {
    this.m_isAnimatingOpenOrClose = false;
    this.MarkShopAsSeen();
    if (this.OnOpenCompleted != null)
      this.OnOpenCompleted();
    this.UpdateScrollerEnabled();
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Store);
  }

  private void CompleteClose()
  {
    this.m_isAnimatingOpenOrClose = false;
    if (this.OnCloseCompleted != null)
      this.OnCloseCompleted();
    this.UpdateScrollerEnabled();
    this.SetMasking(false);
    if ((bool) this.UnloadUnusedAssetsOnClose && (UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().UnloadUnusedAssets();
    Box.Get().PlayBoxMusic();
  }

  private void SetMasking(bool maskingEnabled)
  {
    if (this.m_cameraMasks == null)
      return;
    foreach (Maskable cameraMask in this.m_cameraMasks)
    {
      cameraMask.enabled = maskingEnabled;
      cameraMask.SetVisibility(maskingEnabled, false);
    }
  }

  private void HandleSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod) => StoreManager.Get().Catalog.UpdateProductStatus();

  private void HandleSuccessfulPurchase(Network.Bundle bundle, PaymentMethod paymentMethod) => this.RefreshWallet();

  private void HandleGoldBalanceUpdate(NetCache.NetCacheGoldBalance balance)
  {
    Log.Store.PrintDebug("Gold balance updated to {0}", (object) balance.GetTotal());
    this.UpdateCurrencyBalance(CurrencyType.GOLD, balance.GetTotal());
  }

  private void HandleDustBalanceUpdate()
  {
    if (NetCache.Get() == null)
      return;
    long arcaneDustBalance = NetCache.Get().GetArcaneDustBalance();
    Log.Store.PrintDebug("Arcane Dust balance updated to {0}", (object) arcaneDustBalance);
    this.UpdateCurrencyBalance(CurrencyType.DUST, arcaneDustBalance);
  }

  public void RefreshDataModel()
  {
    ProductCatalog catalog = StoreManager.Get().Catalog;
    this.m_shopData.IsWild = CollectionManager.Get() != null && CollectionManager.Get().ShouldAccountSeeStandardWild();
    this.m_shopData.Tiers.Clear();
    List<ProductTierDataModel> tiersCurrent = catalog.GetTiers_Current();
    if (catalog.HasTestData)
    {
      this.m_shopData.Tiers.AddRange((IEnumerable<ProductTierDataModel>) tiersCurrent);
    }
    else
    {
      foreach (ProductTierDataModel tier in tiersCurrent)
      {
        if (ShopUtils.ShouldDisplayTier(tier, tier.BrowserButtons.Count))
          this.m_shopData.Tiers.Add(tier);
      }
    }
    this.m_shopData.VirtualCurrency = catalog.VirtualCurrencyProductItem ?? ProductFactory.CreateEmptyProductDataModel();
    this.m_shopData.BoosterCurrency = catalog.BoosterCurrencyProductItem ?? ProductFactory.CreateEmptyProductDataModel();
    this.m_shopData.AutoconvertCurrency = Options.Get().GetBool(Option.AUTOCONVERT_VIRTUAL_CURRENCY);
    this.m_shopData.HasNewItems = this.ContainsNewlyDisplayedItems();
    this.m_tiersChangeCountAtLastRefresh = catalog.TiersChangeCount;
    this.m_shopData.TavernTicketBalance = NetCache.Get().GetArenaTicketBalance();
    this.m_shopData.DebugShowProductIds = Options.Get().GetBool(Option.DEBUG_SHOW_PRODUCT_IDS);
    this.RefreshWallet();
  }

  private Shop.PurchaseOrder GetPrerequisitePurchase(Shop.PurchaseOrder pendingPurchase)
  {
    PriceDataModel price = pendingPurchase.m_price;
    if (price.Currency == CurrencyType.REAL_MONEY)
      return (Shop.PurchaseOrder) null;
    long deficitForPurchase = this.GetDeficitForPurchase(pendingPurchase);
    if (deficitForPurchase <= 0L)
      return (Shop.PurchaseOrder) null;
    Shop.PurchaseOrder prerequisitePurchase = new Shop.PurchaseOrder();
    prerequisitePurchase.m_product = ShopUtils.FindCurrencyProduct(price.Currency, (float) deficitForPurchase);
    if (prerequisitePurchase.m_product == null)
    {
      Log.Store.PrintError("Unable to find product with {0} of currency {1}", (object) deficitForPurchase, (object) price.Currency.ToString());
      return prerequisitePurchase;
    }
    if (prerequisitePurchase.m_product.Items.Count == 0)
    {
      Log.Store.PrintError("Invalid currency product '" + prerequisitePurchase.m_product.Name + "': No items found.");
      prerequisitePurchase.m_product = (ProductDataModel) null;
      return prerequisitePurchase;
    }
    if (prerequisitePurchase.m_product.Items[0].ItemType == RewardItemType.CN_ARCANE_ORBS && !this.m_shopData.AutoconvertCurrency)
    {
      Log.Store.PrintError("Unable to convert Booster Currency; autoconversion required");
      prerequisitePurchase.m_product = (ProductDataModel) null;
      return prerequisitePurchase;
    }
    float currencyInProduct = ShopUtils.GetAmountOfCurrencyInProduct(prerequisitePurchase.m_product, price.Currency);
    if ((double) currencyInProduct <= 0.0)
    {
      Log.Store.PrintError("Invalid currency product; contains no currency");
      return prerequisitePurchase;
    }
    prerequisitePurchase.m_quantity = Mathf.CeilToInt((float) deficitForPurchase / currencyInProduct);
    prerequisitePurchase.m_price = prerequisitePurchase.m_product.Prices.FirstOrDefault<PriceDataModel>();
    return prerequisitePurchase;
  }

  private void CancelAutoPurchases() => this.m_autoPurchaseStack.Clear();

  private void HandlePageOpened(object sender, EventArgs e)
  {
    if ((bool) (UnityEngine.Object) this.m_vcPage && this.m_vcPage == sender)
      this.m_isOpeningVCPage = false;
    this.UpdateScrollerEnabled();
    this.UpdateCurrentProductPage();
  }

  private void HandlePageClosed(object sender, EventArgs e)
  {
    this.CancelAutoPurchases();
    this.ReopenClosedPage();
    this.UpdateScrollerEnabled();
    this.UpdateCurrentProductPage();
    this.MarkVirtualCurrencyDirty();
    if (this.m_suppressBoxOpen && !this.m_isOpeningVCPage && !this.m_isReopeningPage)
    {
      SoundManager.Get().LoadAndPlay((AssetReference) "Store_window_shrink.prefab:b68247126e211224e8a904142d2a9895", this.gameObject);
      this.Close();
    }
    if (this.OnProductPageClosed == null)
      return;
    this.OnProductPageClosed();
  }

  private void UpdateCurrentProductPage()
  {
    ProductPage productPage = (ProductPage) null;
    if (this.m_productPageContainer.IsOpen)
      productPage = this.m_productPageContainer.GetCurrentProductPage();
    else if (this.m_vcPage.IsOpen)
      productPage = (ProductPage) this.m_vcPage;
    else if (this.m_bcPage.IsOpen)
      productPage = (ProductPage) this.m_bcPage;
    this.CurrentProductPage = productPage;
  }

  private void UpdateScrollerEnabled()
  {
    bool flag1 = (UnityEngine.Object) this.m_bcPage != (UnityEngine.Object) null && this.m_bcPage.IsOpen || (UnityEngine.Object) this.m_vcPage != (UnityEngine.Object) null && this.m_vcPage.IsOpen || (UnityEngine.Object) this.m_productPageContainer != (UnityEngine.Object) null && this.m_productPageContainer.IsOpen;
    bool flag2 = this.IsOpen() && !flag1 && !this.m_isAnimatingOpenOrClose;
    this.m_browserScroller.enabled = flag2;
    this.m_browserScroller.SetHideThumb(!flag2);
  }

  private void TryNextAutoPurchase()
  {
    if (this.m_autoPurchaseStack.Count == 0)
      return;
    Shop.PurchaseOrder purchase = this.m_autoPurchaseStack.Peek();
    if (purchase == null || this.GetDeficitForPurchase(purchase) != 0L)
      return;
    this.m_autoPurchaseStack.Pop();
    this.ExecutePurchaseOrder(purchase, false);
  }

  private void RequestVirtualBalanceIfNeeded(CurrencyType currencyType)
  {
    if (!ShopUtils.IsVirtualCurrencyEnabled())
      return;
    if (!ShopUtils.IsCurrencyVirtual(currencyType))
    {
      Log.Store.PrintError("{0} is not a virtual currency", (object) currencyType);
    }
    else
    {
      CurrencyCache currencyCache = this.GetCurrencyCache(currencyType);
      if (!currencyCache.NeedsRefresh())
        return;
      currencyCache.TryRefresh();
    }
  }

  private void MarkVirtualCurrencyDirty()
  {
    if (!ShopUtils.IsVirtualCurrencyEnabled())
      return;
    CurrencyType currencyType1;
    if (ShopUtils.TryGetMainVirtualCurrencyType(out currencyType1))
      this.GetCurrencyCache(currencyType1).MarkDirty();
    CurrencyType currencyType2;
    if (!ShopUtils.TryGetBoosterVirtualCurrencyType(out currencyType2))
      return;
    this.GetCurrencyCache(currencyType2).MarkDirty();
  }

  private void UpdateCurrencyBalance(CurrencyType type, long balance) => this.GetCurrencyCache(type).UpdateBalance(balance);

  private CurrencyCache GetCurrencyCache(CurrencyType type) => StoreManager.Get().GetCurrencyCache(type);

  private IEnumerable<CurrencyCache> GetAllCurrencyCaches(
    bool forceIncludeVc = false)
  {
    List<CurrencyCache> allCurrencyCaches = new List<CurrencyCache>();
    allCurrencyCaches.Add(this.GetCurrencyCache(CurrencyType.GOLD));
    allCurrencyCaches.Add(this.GetCurrencyCache(CurrencyType.DUST));
    allCurrencyCaches.Add(this.GetCurrencyCache(CurrencyType.RENOWN));
    if (forceIncludeVc || ShopUtils.IsVirtualCurrencyEnabled())
    {
      CurrencyType currencyType1;
      if (ShopUtils.TryGetMainVirtualCurrencyType(out currencyType1))
        allCurrencyCaches.Add(this.GetCurrencyCache(currencyType1));
      CurrencyType currencyType2;
      if (ShopUtils.TryGetBoosterVirtualCurrencyType(out currencyType2))
        allCurrencyCaches.Add(this.GetCurrencyCache(currencyType2));
    }
    return (IEnumerable<CurrencyCache>) allCurrencyCaches;
  }

  private void HandleOnCurrencyFirstCached()
  {
    List<Balance> balances = new List<Balance>();
    foreach (CurrencyCache allCurrencyCach in this.GetAllCurrencyCaches())
    {
      if (!allCurrencyCach.IsCached())
        return;
      balances.Add(new Balance()
      {
        Name = Enum.GetName(typeof (CurrencyType), (object) allCurrencyCach.Type).ToLowerInvariant(),
        Amount = (double) allCurrencyCach.PriceDataModel.Amount
      });
    }
    TelemetryManager.Client().SendShopBalanceAvailable(balances);
  }

  private void HandleOnCurrencyBalanceChanged(CurrencyBalanceChangedEventArgs args)
  {
    if (this.CurrencyBalanceChanged == null)
      return;
    this.CurrencyBalanceChanged(args);
  }

  private bool OnNavigateBack()
  {
    this.Close();
    return true;
  }

  private void RegisterProductPage<T>(T page, out T member) where T : ProductPage
  {
    member = page;
    member.OnOpened += new EventHandler(this.HandlePageOpened);
    member.OnClosed += new EventHandler(this.HandlePageClosed);
  }

  private static IEnumerator<IAsyncJobResult> Job_OpenToProductPage(
    long pmtProductId,
    ShopType shopType = ShopType.GENERAL_STORE)
  {
    StoreManager storeManager = StoreManager.Get();
    if (storeManager == null)
      yield return (IAsyncJobResult) new JobFailedResult("[Shop.OpenToProductPage] Cannot open product because StoreManager is unavailable", Array.Empty<object>());
    while (!storeManager.IsOpen(false))
      yield return (IAsyncJobResult) null;
    switch (shopType)
    {
      case ShopType.GENERAL_STORE:
        storeManager.StartGeneralTransaction();
        break;
      case ShopType.BATTLEGROUNDS_STORE:
        storeManager.StartBattlegroundsTransaction((Store.ExitCallback) ((authorizationBackButtonPressed, userData) => { }), false);
        break;
      case ShopType.MERCENARIES_STORE:
        storeManager.StartMercenariesTransaction((Store.ExitCallback) ((authorizationBackButtonPressed, userData) => { }), false);
        break;
      default:
        yield return (IAsyncJobResult) new JobFailedResult("[Shop.OpenToProductPage] Must provide a valid ShopType", Array.Empty<object>());
        break;
    }
    if (pmtProductId == 0L)
      yield return (IAsyncJobResult) new JobFailedResult("[Shop.OpenToProductPage] Must provide a PMT product Id", Array.Empty<object>());
    while (storeManager.Catalog.TiersChangeCount == 0L)
      yield return (IAsyncJobResult) null;
    while ((UnityEngine.Object) Shop.s_instance == (UnityEngine.Object) null)
      yield return (IAsyncJobResult) null;
    SoundManager.Get().LoadAndPlay((AssetReference) "Store_window_expand.prefab:050bf879a3e32d04999427c262baaf09", Shop.s_instance.gameObject);
    ProductDataModel product = storeManager.Catalog.Products.FirstOrDefault<ProductDataModel>((Func<ProductDataModel, bool>) (p => p.PmtId == pmtProductId));
    if (product == null)
      yield return (IAsyncJobResult) new JobFailedResult("[Shop.OpenToProductPage] Unable to find product {0} in catalog", new object[1]
      {
        (object) pmtProductId
      });
    while (!Shop.s_instance.IsOpen())
      yield return (IAsyncJobResult) null;
    ProductDataModel fromPmtProductId = Shop.s_instance.GetBaseProductFromPmtProductId(pmtProductId);
    CurrencyType vcType;
    if (fromPmtProductId != null)
    {
      if (storeManager.Catalog.VirtualCurrencyProductItem == fromPmtProductId)
      {
        if (!ShopUtils.IsVirtualCurrencyEnabled())
          yield return (IAsyncJobResult) new JobFailedResult("[Shop.OpenToProductPage] Cannot handle VC product when VC mode is disabled", Array.Empty<object>());
        if (!ShopUtils.TryGetMainVirtualCurrencyType(out vcType))
          yield return (IAsyncJobResult) new JobFailedResult("[Shop.OpenToProductPage] Cannot handle VC product with no valid Currency Type", Array.Empty<object>());
        Shop.s_instance.OpenVirtualCurrencyPurchase(ShopUtils.GetAmountOfCurrencyInProduct(product, vcType));
      }
      else if (storeManager.Catalog.BoosterCurrencyProductItem == fromPmtProductId)
      {
        if (!ShopUtils.IsVirtualCurrencyEnabled())
          yield return (IAsyncJobResult) new JobFailedResult("[Shop.OpenToProductPage] Cannot handle BC product when VC mode is disabled", Array.Empty<object>());
        if (!ShopUtils.TryGetBoosterVirtualCurrencyType(out vcType))
          yield return (IAsyncJobResult) new JobFailedResult("[Shop.OpenToProductPage] Cannot handle BC product with no valid Currency Type", Array.Empty<object>());
        Shop.s_instance.OpenBoosterCurrencyPurchase(ShopUtils.GetAmountOfCurrencyInProduct(product, vcType));
      }
      else
        Shop.s_instance.OpenProductPage(fromPmtProductId, product);
    }
    else
      Shop.s_instance.OpenProductPage(product);
    if (!Shop.s_instance.m_suppressBoxOpen)
    {
      while (!Shop.s_instance.m_browser.IsReady() || Shop.s_instance.m_browser.IsLayoutDirty())
        yield return (IAsyncJobResult) null;
      if (Shop.s_instance.IsOpen())
      {
        ShopSlot shopSlot = (ShopSlot) null;
        foreach (ShopSection activeSection in Shop.s_instance.m_browser.GetActiveSections())
        {
          ProductTierDataModel tierDataModel = activeSection.GetTierDataModel();
          ShopBrowserButtonDataModel browserButtonDataModel = tierDataModel != null ? tierDataModel.BrowserButtons.FirstOrDefault<ShopBrowserButtonDataModel>((Func<ShopBrowserButtonDataModel, bool>) (b => b.DisplayProduct == product || b.DisplayProduct.Variants.Contains(product))) : (ShopBrowserButtonDataModel) null;
          if (browserButtonDataModel != null)
          {
            int index = activeSection.GetTierDataModel().BrowserButtons.IndexOf(browserButtonDataModel);
            shopSlot = activeSection.GetSortedEnabledSlots().ElementAtOrDefault<ShopSlot>(index);
            break;
          }
        }
        if ((UnityEngine.Object) shopSlot == (UnityEngine.Object) null)
          Log.Store.PrintWarning("Product {0} not found on landing page", (object) pmtProductId);
        else
          Shop.s_instance.m_browserScroller.CenterObjectInView(shopSlot.gameObject, 0.0f, (UIBScrollable.OnScrollComplete) null, iTween.EaseType.easeInExpo, 0.2f, true);
      }
    }
  }

  private ProductDataModel GetBaseProductFromPmtProductId(long pmtProductId)
  {
    ProductDataModel fromPmtProductId = (ProductDataModel) null;
    foreach (ProductTierDataModel productTierDataModel in StoreManager.Get().Catalog.GetTiers_All())
    {
      fromPmtProductId = productTierDataModel.BrowserButtons.FirstOrDefault<ShopBrowserButtonDataModel>((Func<ShopBrowserButtonDataModel, bool>) (b => b.DisplayProduct.Variants.Any<ProductDataModel>((Func<ProductDataModel, bool>) (v => v.PmtId == pmtProductId))))?.DisplayProduct;
      if (fromPmtProductId != null)
        break;
    }
    if (fromPmtProductId == null)
    {
      foreach (ProductDataModel productDataModel in new List<ProductDataModel>()
      {
        StoreManager.Get().Catalog.VirtualCurrencyProductItem,
        StoreManager.Get().Catalog.BoosterCurrencyProductItem
      })
      {
        if (productDataModel != null && productDataModel.Variants.Any<ProductDataModel>((Func<ProductDataModel, bool>) (v => v.PmtId == pmtProductId)))
        {
          fromPmtProductId = productDataModel;
          break;
        }
      }
    }
    return fromPmtProductId;
  }

  private IEnumerator SendShopVisitTelemetry()
  {
    Shop shop = this;
    float startTime = Time.time;
    while (!((UnityEngine.Object) shop == (UnityEngine.Object) null) && !((UnityEngine.Object) shop.m_browser == (UnityEngine.Object) null))
    {
      if ((!shop.m_browser.IsReady() || shop.m_browser.IsLayoutDirty()) && (double) Time.time - (double) startTime < 20.0)
      {
        yield return (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        yield return (object) new WaitUntil(new Func<bool>(shop.\u003CSendShopVisitTelemetry\u003Eb__153_0));
        List<ShopCard> cards = new List<ShopCard>();
        foreach (ShopSection activeSection in shop.m_browser.GetActiveSections())
        {
          foreach (ShopSlot sortedEnabledSlot in activeSection.GetSortedEnabledSlots())
          {
            ShopCard shopCardTelemetry = sortedEnabledSlot.GetShopCardTelemetry();
            if (shopCardTelemetry.HasProduct)
              cards.Add(shopCardTelemetry);
          }
        }
        TelemetryManager.Client().SendShopVisit(cards, StoreManager.Get().CurrentShopType.ToString());
        break;
      }
    }
  }

  private bool IsReadyToRequestVirtualCurrencyBalances()
  {
    if (!ShopUtils.IsVirtualCurrencyEnabled())
      return false;
    HearthstoneCheckout hearthstoneCheckout1 = ServiceManager.Get<HearthstoneCheckout>();
    if ((hearthstoneCheckout1 != null ? (!hearthstoneCheckout1.IsAvailable() ? 1 : 0) : 1) == 0)
    {
      HearthstoneCheckout hearthstoneCheckout2 = ServiceManager.Get<HearthstoneCheckout>();
      if ((hearthstoneCheckout2 != null ? (hearthstoneCheckout2.IsClientCreationInProgress() ? 1 : 0) : 0) == 0 && (StoreManager.Get().GetCurrentStore() != null || !((UnityEngine.Object) Box.Get() == (UnityEngine.Object) null) && Box.Get().GetState() != Box.State.OPEN))
        return true;
    }
    return false;
  }

  private class PurchaseOrder
  {
    public ProductDataModel m_product;
    public PriceDataModel m_price;
    public int m_quantity = 1;
    public bool m_hasFinished;
  }

  private struct PageReopenInfo
  {
    private ProductDataModel m_product;
    private ProductDataModel m_variant;
    private ProductPage m_page;
    private ProductPageContainer m_pageContainer;

    public static Shop.PageReopenInfo Capture(ProductPage page) => new Shop.PageReopenInfo()
    {
      m_page = page
    };

    public static Shop.PageReopenInfo Capture(ProductPageContainer pageContainer) => new Shop.PageReopenInfo()
    {
      m_pageContainer = pageContainer,
      m_product = (bool) (UnityEngine.Object) pageContainer ? pageContainer.Product : (ProductDataModel) null,
      m_variant = (bool) (UnityEngine.Object) pageContainer ? pageContainer.Variant : (ProductDataModel) null
    };

    public bool IsValid => (bool) (UnityEngine.Object) this.m_page || (bool) (UnityEngine.Object) this.m_pageContainer;

    public unsafe void Clear() => *(Shop.PageReopenInfo*) ref this = new Shop.PageReopenInfo();

    public void ReopenPage()
    {
      if ((bool) (UnityEngine.Object) this.m_page)
      {
        this.m_page.Open();
      }
      else
      {
        if (!(bool) (UnityEngine.Object) this.m_pageContainer)
          return;
        this.m_pageContainer.SetProduct(this.m_product, this.m_variant);
        this.m_pageContainer.Open();
      }
    }
  }
}
