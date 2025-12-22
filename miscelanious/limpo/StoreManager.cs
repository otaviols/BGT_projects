using Blizzard.Commerce;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using com.blizzard.commerce.Model;
using Hearthstone;
using Hearthstone.Commerce;
using Hearthstone.Core;
using Hearthstone.UI;
using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class StoreManager
{
  public static readonly int DEFAULT_SECONDS_BEFORE_AUTO_CANCEL = 600;
  private static readonly PlatformDependentValue<GeneralStoreMode> s_defaultStoreMode = new PlatformDependentValue<GeneralStoreMode>(PlatformCategory.Screen)
  {
    PC = GeneralStoreMode.CARDS,
    Phone = GeneralStoreMode.NONE
  };
  private static readonly int UNKNOWN_TRANSACTION_ID = -1;
  private static readonly double CURRENCY_TRANSACTION_TIMEOUT_SECONDS = 30.0;
  private static readonly Map<AdventureDbId, ProductType> s_adventureToProductMap = new Map<AdventureDbId, ProductType>()
  {
    {
      AdventureDbId.NAXXRAMAS,
      ProductType.PRODUCT_TYPE_NAXX
    },
    {
      AdventureDbId.BRM,
      ProductType.PRODUCT_TYPE_BRM
    },
    {
      AdventureDbId.LOE,
      ProductType.PRODUCT_TYPE_LOE
    }
  };
  private static StoreManager s_instance = (StoreManager) null;
  private readonly ShopView m_view = new ShopView();
  private bool m_featuresReady;
  private bool m_initComplete;
  private bool m_battlePayAvailable;
  private bool m_firstNoticesProcessed;
  private bool m_firstMoneyOrGTAPPTransactionSet;
  private bool m_showAllIsOpenFailures;
  private ShopAvailabilityError m_lastIsOpenFailure = ShopAvailabilityError.NO_ERROR;
  private bool m_isStatusRefreshPending;
  private bool m_isShowingStoreUnavailableAlert;
  private float m_secsBeforeAutoCancel = (float) StoreManager.DEFAULT_SECONDS_BEFORE_AUTO_CANCEL;
  private float m_lastCancelRequestTime;
  private bool m_configLoaded;
  private readonly Map<ProductId, Network.Bundle> m_bundles = new Map<ProductId, Network.Bundle>();
  private readonly Map<int, Network.GoldCostBooster> m_goldCostBooster = new Map<int, Network.GoldCostBooster>();
  private readonly CatalogNetworkPages m_catalogNetworkPages = new CatalogNetworkPages();
  private readonly Dictionary<int, Network.ShopSale> m_sales = new Dictionary<int, Network.ShopSale>();
  private long? m_goldCostArena;
  private Currency m_currency = new Currency();
  private readonly HashSet<long> m_transactionIDsConclusivelyHandled = new HashSet<long>();
  private readonly Map<ShopType, IStore> m_stores = new Map<ShopType, IStore>();
  private ShopType m_currentShopType;
  private bool m_ignoreProductTiming;
  private readonly Map<CurrencyType, CurrencyCache> m_currencyCaches = new Map<CurrencyType, CurrencyCache>();
  private float m_showStoreStart;
  private Network.PurchaseMethod m_challengePurchaseMethod;
  private BnetRegion m_regionId;
  private StorePackId m_currentlySelectedId;
  private bool m_canCloseConfirmation = true;
  private Dictionary<ShopType, string> m_shopPageIds = new Dictionary<ShopType, string>();
  private bool m_openWhenLastEventFired;
  private HashSet<ShopType> m_requestedShopTypeSections = new HashSet<ShopType>();
  private HashSet<ShopType> m_receivedShopTypeSections = new HashSet<ShopType>();
  private readonly Dictionary<PegasusShared.ShopType, ShopType> m_shopTypeBySharedShopType = new Dictionary<PegasusShared.ShopType, ShopType>()
  {
    {
      PegasusShared.ShopType.SHOP_TYPE_GENERAL,
      ShopType.GENERAL_STORE
    },
    {
      PegasusShared.ShopType.SHOP_TYPE_BATTLEGROUNDS,
      ShopType.BATTLEGROUNDS_STORE
    },
    {
      PegasusShared.ShopType.SHOP_TYPE_MERCENARIES,
      ShopType.MERCENARIES_STORE
    }
  };
  private StoreManager.TransactionStatus m_status;
  private bool m_waitingToShowStore;
  private StoreManager.ShowStoreData m_showStoreData;
  private MoneyOrGTAPPTransaction m_activeMoneyOrGTAPPTransaction;
  private BuyProductEventArgs m_pendingProductPurchaseArgs;
  private readonly HashSet<long> m_confirmedTransactionIDs = new HashSet<long>();
  private readonly List<NetCache.ProfileNoticePurchase> m_outstandingPurchaseNotices = new List<NetCache.ProfileNoticePurchase>();
  private List<Achievement> m_completedAchieves = new List<Achievement>();
  private bool m_licenseAchievesListenerRegistered;
  private StoreManager.TransactionStatus m_previousStatusBeforeAutoCancel;
  private static readonly PlatformDependentValue<bool> HAS_THIRD_PARTY_APP_STORE = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    PC = false,
    Mac = false,
    iOS = true,
    Android = true
  };

  public bool IgnoreProductTiming => this.m_ignoreProductTiming;

  public IEnumerable<Network.Bundle> AllBundles => (IEnumerable<Network.Bundle>) this.m_bundles.Values;

  public IEnumerable<Network.GoldCostBooster> AllGoldCostBoosters => (IEnumerable<Network.GoldCostBooster>) this.m_goldCostBooster.Values;

  public ShopType CurrentShopType => this.m_currentShopType;

  public CatalogNetworkPages CatalogNetworkPages => this.m_catalogNetworkPages;

  private StoreManager() => this.Catalog = new ProductCatalog(this);

  public StorePackId CurrentlySelectedId => this.m_currentlySelectedId;

  private event System.Action<bool> OnStatusChanged = isOpen => { };

  private event System.Action<Network.Bundle, PaymentMethod> OnSuccessfulPurchaseAck = (bundle, paymentMethod) => { };

  private event System.Action<Network.Bundle, PaymentMethod> OnSuccessfulPurchase = (bundle, paymentMethod) => { };

  private event System.Action<Network.Bundle, PaymentMethod> OnFailedPurchaseAck = (bundle, paymentMethod) => { };

  private event System.Action OnAuthorizationExit = () => { };

  private event System.Action OnStoreShown = () => { };

  private event System.Action OnStoreHidden = () => { };

  public static StoreManager Get() => StoreManager.s_instance ?? (StoreManager.s_instance = new StoreManager());

  public static bool IsInitialized() => StoreManager.s_instance != null;

  private static void DestroyInstance()
  {
    StoreManager.s_instance.GetStore(ShopType.GENERAL_STORE)?.Unload();
    if (AchieveManager.Get() != null && StoreManager.s_instance != null)
    {
      AchieveManager.Get().RemoveAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(StoreManager.s_instance.OnAchievesUpdated));
      AchieveManager.Get().RemoveLicenseAddedAchievesUpdatedListener(new AchieveManager.LicenseAddedAchievesUpdatedCallback(StoreManager.s_instance.OnLicenseAddedAchievesUpdated));
    }
    StoreManager.s_instance = (StoreManager) null;
  }

  public ProductCatalog Catalog { get; }

  private void NetworkRegistration()
  {
    Network network = Network.Get();
    network.RegisterNetHandler((object) BattlePayStatusResponse.PacketID.ID, new Network.NetHandler(this.OnBattlePayStatusResponse));
    network.RegisterNetHandler((object) BattlePayConfigResponse.PacketID.ID, new Network.NetHandler(this.OnBattlePayConfigResponse));
    network.RegisterNetHandler((object) PegasusUtil.PurchaseMethod.PacketID.ID, new Network.NetHandler(this.OnPurchaseMethod));
    network.RegisterNetHandler((object) PegasusUtil.PurchaseResponse.PacketID.ID, new Network.NetHandler(this.OnPurchaseResponse));
    network.RegisterNetHandler((object) CancelPurchaseResponse.PacketID.ID, new Network.NetHandler(this.OnPurchaseCanceledResponse));
    network.RegisterNetHandler((object) PurchaseWithGoldResponse.PacketID.ID, new Network.NetHandler(this.OnPurchaseViaGoldResponse));
    network.RegisterNetHandler((object) ThirdPartyPurchaseStatusResponse.PacketID.ID, new Network.NetHandler(this.OnThirdPartyPurchaseStatusResponse));
  }

  public void Init()
  {
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheFeatures), new System.Action(this.OnNetCacheFeaturesReady));
    if (this.m_initComplete)
      return;
    SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
    this.NetworkRegistration();
    NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
    NetCache.Get().OwnedBattlegroundsSkinsChanged += new NetCache.DelOwnedBattlegroundsSkinsChanged(this.Catalog.UpdateProductStatus);
    if (netObject != null)
      this.OnNewNotices(netObject.Notices, false);
    NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
    LoginManager.Get().OnFullLoginFlowComplete += new System.Action(this.OnLoginCompleted);
    this.m_regionId = BattleNet.GetCurrentRegion();
    this.RegisterViewListeners();
    AccountLicenseMgr.Get().RegisterAccountLicensesChangedListener(new AccountLicenseMgr.AccountLicensesChangedCallback(this.OnAccountLicensesUpdate));
    this.m_initComplete = true;
    AssetLoader.Get().InstantiatePrefab((AssetReference) (string) ShopPrefabs.ShopPrefab, new PrefabCallback<GameObject>(this.OnGeneralStoreLoaded));
    HearthstoneApplication.Get().WillReset += new System.Action(this.WillReset);
  }

  private void WillReset()
  {
    HearthstoneApplication.Get().WillReset -= new System.Action(this.WillReset);
    AccountLicenseMgr.Get().RemoveAccountLicensesChangedListener(new AccountLicenseMgr.AccountLicensesChangedCallback(this.OnAccountLicensesUpdate));
    this.UnregisterViewListeners();
    Network network = Network.Get();
    network.RemoveNetHandler((object) BattlePayStatusResponse.PacketID.ID, new Network.NetHandler(this.OnBattlePayStatusResponse));
    network.RemoveNetHandler((object) BattlePayConfigResponse.PacketID.ID, new Network.NetHandler(this.OnBattlePayConfigResponse));
    network.RemoveNetHandler((object) PegasusUtil.PurchaseMethod.PacketID.ID, new Network.NetHandler(this.OnPurchaseMethod));
    network.RemoveNetHandler((object) PegasusUtil.PurchaseResponse.PacketID.ID, new Network.NetHandler(this.OnPurchaseResponse));
    network.RemoveNetHandler((object) CancelPurchaseResponse.PacketID.ID, new Network.NetHandler(this.OnPurchaseCanceledResponse));
    network.RemoveNetHandler((object) PurchaseWithGoldResponse.PacketID.ID, new Network.NetHandler(this.OnPurchaseViaGoldResponse));
    network.RemoveNetHandler((object) ThirdPartyPurchaseStatusResponse.PacketID.ID, new Network.NetHandler(this.OnThirdPartyPurchaseStatusResponse));
    NetCache.Get().RemoveUpdatedListener(typeof (NetCache.NetCacheFeatures), new System.Action(this.OnNetCacheFeaturesReady));
    NetCache.Get().OwnedBattlegroundsSkinsChanged -= new NetCache.DelOwnedBattlegroundsSkinsChanged(this.Catalog.UpdateProductStatus);
    foreach (CurrencyCache currencyCache in this.m_currencyCaches.Values)
      currencyCache.BalanceAvailabilityChanged -= new System.Action(this.OnCacheBalanceAvailabilityChanged);
    this.m_currencyCaches.Clear();
    StoreManager.DestroyInstance();
  }

  public void Heartbeat()
  {
    if (!this.m_initComplete)
      return;
    if (this.m_isStatusRefreshPending)
    {
      this.m_isStatusRefreshPending = false;
      this.HandleShopAvailabilityChange();
    }
    this.AutoCancelPurchaseIfNeeded(Time.realtimeSinceStartup);
  }

  public ShopAvailabilityError GetStoreAvailabilityError()
  {
    if (!this.FirstNoticesProcessed)
      return ShopAvailabilityError.FIRST_NOTICES_NOT_PROCESSED;
    if (!this.IsStoreFeatureEnabled())
      return ShopAvailabilityError.STORE_FEATURE_DISABLED;
    if (!this.BattlePayAvailable)
      return ShopAvailabilityError.BATTLEPAY_UNAVAILABLE;
    if (!this.AreVirtualCurrencyBalancesReady())
      return ShopAvailabilityError.VIRTUAL_CURRENCY_BALANCE_UNAVAILABLE;
    if (!this.ConfigLoaded)
      return ShopAvailabilityError.BATTLEPAY_CONFIG_NOT_LOADED;
    if (!this.HaveProductsToSell())
      return ShopAvailabilityError.NO_PRODUCTS_FOR_SALE;
    if (!Network.IsLoggedIn())
      return ShopAvailabilityError.NOT_LOGGED_IN;
    HearthstoneCheckout hearthstoneCheckout1 = ServiceManager.Get<HearthstoneCheckout>();
    if ((hearthstoneCheckout1 != null ? (hearthstoneCheckout1.IsClientCreationInProgress() ? 1 : 0) : 0) != 0)
      return ShopAvailabilityError.CHECKOUT_INITIALIZING;
    HearthstoneCheckout hearthstoneCheckout2 = ServiceManager.Get<HearthstoneCheckout>();
    if ((hearthstoneCheckout2 != null ? (!hearthstoneCheckout2.IsAvailable() ? 1 : 0) : 1) != 0)
      return ShopAvailabilityError.CHECKOUT_UNAVAILABLE;
    if (!this.IsSimpleCheckoutFeatureEnabled())
      return ShopAvailabilityError.CHECKOUT_NOT_ENABLED;
    return this.Status == StoreManager.TransactionStatus.UNKNOWN ? ShopAvailabilityError.TRANSACTION_STATUS_UNKNOWN : ShopAvailabilityError.NO_ERROR;
  }

  public bool IsOpen(bool printStatus = true)
  {
    ShopAvailabilityError availabilityError = this.GetStoreAvailabilityError();
    if (availabilityError == ShopAvailabilityError.NO_ERROR)
    {
      if (printStatus)
        Log.Store.Print("Store is OPEN.");
      return true;
    }
    if (printStatus && (availabilityError != this.m_lastIsOpenFailure || this.m_showAllIsOpenFailures))
    {
      if (availabilityError != this.m_lastIsOpenFailure)
      {
        this.m_lastIsOpenFailure = availabilityError;
        Log.Store.PrintWarning("Store is CLOSED due to: " + availabilityError.ToString());
      }
      else
        Log.Store.Print("Store is CLOSED due to: " + availabilityError.ToString());
    }
    return false;
  }

  private bool IsStoreFeatureEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    return netCacheFeatures != null && netCacheFeatures.Store.Store;
  }

  public bool IsBattlePayFeatureEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    return netCacheFeatures != null && netCacheFeatures.Store.Store && netCacheFeatures.Store.BattlePay;
  }

  public bool IsBuyWithGoldFeatureEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    return netCacheFeatures != null && netCacheFeatures.Store.Store && netCacheFeatures.Store.BuyWithGold;
  }

  private void SetCanTapOutConfirmationUI(bool closeConfirmationUI) => this.m_canCloseConfirmation = closeConfirmationUI;

  public bool CanTapOutConfirmationUI() => this.m_canCloseConfirmation;

  public bool IsSimpleCheckoutFeatureEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    if (netCacheFeatures == null)
      return false;
    bool flag = false;
    switch (PlatformSettings.RuntimeOS)
    {
      case OSCategory.PC:
        flag = netCacheFeatures.Store.SimpleCheckoutWin;
        break;
      case OSCategory.Mac:
        flag = netCacheFeatures.Store.SimpleCheckoutMac;
        break;
      case OSCategory.iOS:
        flag = netCacheFeatures.Store.SimpleCheckoutIOS;
        break;
      case OSCategory.Android:
        switch (AndroidDeviceSettings.Get().GetAndroidStore())
        {
          case AndroidStore.BLIZZARD:
          case AndroidStore.HUAWEI:
          case AndroidStore.ONE_STORE:
            flag = netCacheFeatures.Store.SimpleCheckoutAndroidGlobal;
            break;
          case AndroidStore.GOOGLE:
            flag = netCacheFeatures.Store.SimpleCheckoutAndroidGoogle;
            break;
          case AndroidStore.AMAZON:
            flag = netCacheFeatures.Store.SimpleCheckoutAndroidAmazon;
            break;
          default:
            Log.Store.PrintError("The given store was not accounted for: {0}\nPlease check in '{1}.{2}' class and method for implementation.", (object) AndroidDeviceSettings.Get().GetAndroidStore().ToString(), (object) nameof (StoreManager), (object) nameof (IsSimpleCheckoutFeatureEnabled));
            break;
        }
        break;
    }
    if (!flag || !netCacheFeatures.Store.Store || !netCacheFeatures.Store.SimpleCheckout)
      return false;
    HearthstoneCheckout hearthstoneCheckout = ServiceManager.Get<HearthstoneCheckout>();
    return hearthstoneCheckout != null && hearthstoneCheckout.IsAvailable();
  }

  private bool IsSoftAccountPurchasingEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    return netCacheFeatures != null && netCacheFeatures.Store.Store && netCacheFeatures.Store.SoftAccountPurchasing;
  }

  public bool IsVintageStoreEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    return netCacheFeatures == null || netCacheFeatures.Store.VintageStore;
  }

  public bool IsBuyCardBacksFromCollectionManagerEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    return netCacheFeatures == null || netCacheFeatures.Store.BuyCardBacksFromCollectionManager;
  }

  public bool IsBuyHeroSkinsFromCollectionManagerEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    return netCacheFeatures == null || netCacheFeatures.Store.BuyHeroSkinsFromCollectionManager;
  }

  public bool IsLargeItemBundleDetailsEnabled()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
    return netCacheFeatures == null || netCacheFeatures.Store.LargeItemBundleDetailsEnabled;
  }

  public BattlePayProvider? ActiveTransactionProvider() => this.m_activeMoneyOrGTAPPTransaction?.Provider;

  public void RegisterStatusChangedListener(System.Action<bool> callback)
  {
    this.OnStatusChanged -= callback;
    this.OnStatusChanged += callback;
  }

  public void RemoveStatusChangedListener(System.Action<bool> callback) => this.OnStatusChanged -= callback;

  public void RegisterSuccessfulPurchaseListener(System.Action<Network.Bundle, PaymentMethod> callback)
  {
    this.OnSuccessfulPurchase -= callback;
    this.OnSuccessfulPurchase += callback;
  }

  public void RemoveSuccessfulPurchaseListener(System.Action<Network.Bundle, PaymentMethod> callback) => this.OnSuccessfulPurchase -= callback;

  public void RegisterSuccessfulPurchaseAckListener(System.Action<Network.Bundle, PaymentMethod> callback)
  {
    this.OnSuccessfulPurchaseAck -= callback;
    this.OnSuccessfulPurchaseAck += callback;
  }

  public void RemoveSuccessfulPurchaseAckListener(System.Action<Network.Bundle, PaymentMethod> callback) => this.OnSuccessfulPurchaseAck -= callback;

  public void RegisterFailedPurchaseAckListener(System.Action<Network.Bundle, PaymentMethod> callback)
  {
    this.OnFailedPurchaseAck -= callback;
    this.OnFailedPurchaseAck += callback;
  }

  public void RemoveFailedPurchaseAckListener(System.Action<Network.Bundle, PaymentMethod> callback) => this.OnFailedPurchaseAck -= callback;

  public void RegisterAuthorizationExitListener(System.Action callback)
  {
    this.OnAuthorizationExit -= callback;
    this.OnAuthorizationExit += callback;
  }

  public void RemoveAuthorizationExitListener(System.Action callback) => this.OnAuthorizationExit -= callback;

  public void RegisterStoreShownListener(System.Action callback)
  {
    this.OnStoreShown -= callback;
    this.OnStoreShown += callback;
  }

  public void RemoveStoreShownListener(System.Action callback) => this.OnStoreShown -= callback;

  public void RegisterStoreHiddenListener(System.Action callback)
  {
    this.OnStoreHidden -= callback;
    this.OnStoreHidden += callback;
  }

  public void RemoveStoreHiddenListener(System.Action callback) => this.OnStoreHidden -= callback;

  private void RegisterViewListeners()
  {
    this.m_view.OnComponentReady += new System.Action(this.StoreViewReady);
    this.m_view.PurchaseAuth.OnPurchaseResultAcknowledged += new System.Action<bool, MoneyOrGTAPPTransaction>(this.OnPurchaseResultAcknowledged);
    this.m_view.PurchaseAuth.OnCancelButtonPressed += new System.Action(this.OnPurchaseAuthCancelButtonPressed);
    this.m_view.PurchaseAuth.OnAuthExit += new System.Action(this.OnAuthExit);
    this.m_view.Summary.OnSummaryConfirm += new System.Action<int, object>(this.OnSummaryConfirm);
    this.m_view.Summary.OnSummaryCancel += new System.Action<object>(this.OnSummaryCancel);
    this.m_view.Summary.OnSummaryInfo += new System.Action<object>(this.OnSummaryInfo);
    this.m_view.Summary.OnSummaryPaymentAndTos += new System.Action<object>(this.OnSummaryPaymentAndTOS);
    this.m_view.SendToBam.OnOkay += new System.Action<MoneyOrGTAPPTransaction, StoreSendToBAM.BAMReason>(this.OnSendToBAMOkay);
    this.m_view.SendToBam.OnCancel += new System.Action<MoneyOrGTAPPTransaction>(this.OnSendToBAMCancel);
    this.m_view.LegalBam.OnOkay += new System.Action<StoreLegalBAMLinks.BAMReason>(this.OnSendToBAMLegal);
    this.m_view.LegalBam.OnCancel += new System.Action(this.UnblockStoreInterface);
    this.m_view.DoneWithBam.OnOkay += new System.Action(this.UnblockStoreInterface);
    this.m_view.ChallengePrompt.OnComplete += new System.Action<string, bool, CancelPurchase.CancelReason?, string>(this.OnChallengeComplete);
    this.m_view.ChallengePrompt.OnCancel += new System.Action<string>(this.OnChallengeCancel);
  }

  private void UnregisterViewListeners()
  {
    this.m_view.OnComponentReady -= new System.Action(this.StoreViewReady);
    this.m_view.PurchaseAuth.OnPurchaseResultAcknowledged -= new System.Action<bool, MoneyOrGTAPPTransaction>(this.OnPurchaseResultAcknowledged);
    this.m_view.PurchaseAuth.OnCancelButtonPressed -= new System.Action(this.OnPurchaseAuthCancelButtonPressed);
    this.m_view.PurchaseAuth.OnAuthExit -= new System.Action(this.OnAuthExit);
    this.m_view.Summary.OnSummaryConfirm -= new System.Action<int, object>(this.OnSummaryConfirm);
    this.m_view.Summary.OnSummaryCancel -= new System.Action<object>(this.OnSummaryCancel);
    this.m_view.Summary.OnSummaryInfo -= new System.Action<object>(this.OnSummaryInfo);
    this.m_view.Summary.OnSummaryPaymentAndTos -= new System.Action<object>(this.OnSummaryPaymentAndTOS);
    this.m_view.SendToBam.OnOkay -= new System.Action<MoneyOrGTAPPTransaction, StoreSendToBAM.BAMReason>(this.OnSendToBAMOkay);
    this.m_view.SendToBam.OnCancel -= new System.Action<MoneyOrGTAPPTransaction>(this.OnSendToBAMCancel);
    this.m_view.LegalBam.OnOkay -= new System.Action<StoreLegalBAMLinks.BAMReason>(this.OnSendToBAMLegal);
    this.m_view.LegalBam.OnCancel -= new System.Action(this.UnblockStoreInterface);
    this.m_view.DoneWithBam.OnOkay -= new System.Action(this.UnblockStoreInterface);
    this.m_view.ChallengePrompt.OnComplete -= new System.Action<string, bool, CancelPurchase.CancelReason?, string>(this.OnChallengeComplete);
    this.m_view.ChallengePrompt.OnCancel -= new System.Action<string>(this.OnChallengeCancel);
  }

  private bool IsWaitingToShow() => this.m_waitingToShowStore;

  public IStore GetCurrentStore() => this.GetStore(this.m_currentShopType);

  private IStore GetStore(ShopType shopType)
  {
    IStore store;
    this.m_stores.TryGetValue(shopType, out store);
    return store;
  }

  public bool IsShown()
  {
    IStore currentStore = this.GetCurrentStore();
    return currentStore != null && currentStore.IsOpen();
  }

  public bool IsShownOrWaitingToShow() => this.IsWaitingToShow() || this.IsShown();

  public bool GetGoldCostNoGTAPP(NoGTAPPTransactionData noGTAPPTransactionData, out long cost)
  {
    cost = 0L;
    if (noGTAPPTransactionData == null)
      return false;
    long cost1 = 0;
    switch (noGTAPPTransactionData.Product)
    {
      case ProductType.PRODUCT_TYPE_BOOSTER:
      case ProductType.PRODUCT_TYPE_MERCENARIES_BOOSTER:
        if (!this.GetBoosterGoldCostNoGTAPP(noGTAPPTransactionData.ProductData, out cost1))
          return false;
        break;
      case ProductType.PRODUCT_TYPE_DRAFT:
        if (!this.GetArenaGoldCostNoGTAPP(out cost1))
          return false;
        break;
      case ProductType.PRODUCT_TYPE_HIDDEN_LICENSE:
        return false;
      default:
        Log.Store.PrintWarning(string.Format("StoreManager.GetGoldCostNoGTAPP(): don't have a no-GTAPP gold price for product {0} data {1}", (object) noGTAPPTransactionData.Product, (object) noGTAPPTransactionData.ProductData));
        return false;
    }
    cost = cost1 * (long) noGTAPPTransactionData.Quantity;
    return true;
  }

  public Network.Bundle GetBundleFromPmtProductId(long? productId) => !productId.HasValue ? (Network.Bundle) null : this.GetBundleFromPmtProductId(ProductId.CreateFrom(productId.Value));

  public Network.Bundle GetBundleFromPmtProductId(ProductId productId)
  {
    Network.Bundle bundle;
    return productId.IsValid() && this.m_bundles.TryGetValue(productId, out bundle) ? bundle : (Network.Bundle) null;
  }

  private HashSet<ProductType> GetProductsInItemList(
    List<Network.BundleItem> items)
  {
    HashSet<ProductType> productsInItemList = new HashSet<ProductType>();
    foreach (Network.BundleItem bundleItem in items)
      productsInItemList.Add(bundleItem.ItemType);
    return productsInItemList;
  }

  public HashSet<ProductType> GetProductsInBundle(Network.Bundle bundle) => (Record) null == (Record) bundle ? new HashSet<ProductType>() : this.GetProductsInItemList(bundle.Items);

  public ProductAvailability GetNetworkBundleProductAvailability(
    Network.Bundle bundle,
    bool shouldSeeWild,
    bool checkRange = true)
  {
    if ((Record) null == (Record) bundle)
      return ProductAvailability.UNDEFINED;
    bool flag = false;
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    int num4 = 0;
    foreach (Network.BundleItem bundleItem in bundle.Items)
    {
      if (!shouldSeeWild && !flag)
      {
        switch (bundleItem.ItemType)
        {
          case ProductType.PRODUCT_TYPE_BOOSTER:
            flag = GameUtils.IsBoosterWild((BoosterDbId) bundleItem.ProductData);
            break;
          case ProductType.PRODUCT_TYPE_NAXX:
          case ProductType.PRODUCT_TYPE_BRM:
          case ProductType.PRODUCT_TYPE_LOE:
            flag = true;
            break;
          case ProductType.PRODUCT_TYPE_WING:
            flag = GameUtils.IsAdventureWild(GameUtils.GetAdventureIdByWingId(bundleItem.ProductData));
            break;
        }
        if (flag)
          ProductIssues.LogHidden(bundle, string.Format("Hidden due to wild content: Type={0}, ID = {1}", (object) bundleItem.ItemType, (object) bundleItem.ProductData));
      }
      ItemPurchaseRule itemPurchaseRule = StoreManager.GetProductItemPurchaseRule(bundleItem);
      if (itemPurchaseRule == ItemPurchaseRule.UNDEFINED)
      {
        ProductIssues.LogError(bundle, string.Format("Has license with undefined rule about re-purchase. Type={0} Data={1}", (object) bundleItem.ItemType, (object) bundleItem.ProductData));
        flag = true;
      }
      string failReason;
      switch (StoreManager.GetProductItemOwnershipStatus(bundleItem.ItemType, bundleItem.ProductData, out failReason))
      {
        case ItemOwnershipStatus.IGNORED:
        case ItemOwnershipStatus.UNOWNED:
          if (bundleItem.ItemType != ProductType.PRODUCT_TYPE_HIDDEN_LICENSE)
          {
            ++num2;
            continue;
          }
          continue;
        case ItemOwnershipStatus.OWNED:
          ++num1;
          if (itemPurchaseRule == ItemPurchaseRule.BLOCKING)
          {
            ++num3;
            continue;
          }
          continue;
        default:
          ProductIssues.LogError(bundle, failReason ?? string.Format("Has license with unknown ownership status. Type={0} Data={1}", (object) bundleItem.ItemType, (object) bundleItem.ProductData));
          ++num4;
          continue;
      }
    }
    if (num4 > 0)
      return ProductAvailability.UNDEFINED;
    if (flag)
      return ProductAvailability.RESTRICTED;
    if (num1 > 0 && num2 == 0 || num3 > 0)
      return ProductAvailability.ALREADY_OWNED;
    bundle.GetTitle();
    if (num1 == 0 && num2 == 0)
    {
      ProductIssues.LogError(bundle, "Has no buyable or owned rewards. Availability status will remain UNDEFINED.");
      return ProductAvailability.UNDEFINED;
    }
    if (checkRange)
    {
      ProductAvailabilityRange availabilityRange = this.GetBundleAvailabilityRange(bundle);
      if (availabilityRange == null)
      {
        ProductIssues.LogError(bundle, "Has unknown sale or event timing: event timing = " + bundle.ProductEvent + ", Sale ID = " + string.Join(",", bundle.SaleIds.Select<int, string>((Func<int, string>) (id => id.ToString())).ToArray<string>()));
        return ProductAvailability.SALE_NOT_ACTIVE;
      }
      if (!availabilityRange.IsVisibleAtTime(DateTime.UtcNow))
        return ProductAvailability.SALE_NOT_ACTIVE;
    }
    return ProductAvailability.CAN_PURCHASE;
  }

  public bool IsProductAlreadyOwned(Network.Bundle bundle) => this.GetNetworkBundleProductAvailability(bundle, true, false) == ProductAvailability.ALREADY_OWNED;

  public bool IsProductPrePurchase(Network.Bundle bundle) => !((Record) bundle == (Record) null) && bundle.IsPrePurchase;

  public bool IsProductFirstPurchaseBundle(Network.Bundle bundle) => !((Record) bundle == (Record) null) && this.GetProductsInItemList(bundle.Items).Contains(ProductType.PRODUCT_TYPE_HIDDEN_LICENSE) && (Record) null != (Record) bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_HIDDEN_LICENSE && obj.ProductData == 40));

  public static bool DoesBundleContainProduct(
    Network.Bundle bundle,
    ProductType product,
    int productData = 0,
    int numItemsRequired = 0)
  {
    if (numItemsRequired != 0 && bundle.Items.Count != numItemsRequired)
      return false;
    foreach (Network.BundleItem bundleItem in bundle.Items)
    {
      if (bundleItem.ItemType == product && (productData == 0 || bundleItem.ProductData == productData))
        return true;
    }
    return false;
  }

  public IEnumerable<Network.Bundle> EnumerateBundlesForProductType(
    ProductType product,
    bool requireRealMoneyOption,
    int productData = 0,
    int numItemsRequired = 0,
    bool checkAvailability = true)
  {
    foreach (Network.Bundle bundle in this.m_bundles.Values)
    {
      if ((!requireRealMoneyOption || ShopUtils.BundleHasPrice(bundle, CurrencyType.REAL_MONEY)) && StoreManager.DoesBundleContainProduct(bundle, product, productData, numItemsRequired) && (!checkAvailability || this.IsBundleAvailableNow(bundle)))
        yield return bundle;
    }
  }

  public List<Network.Bundle> GetAllBundlesForProduct(
    ProductType product,
    bool requireRealMoneyOption,
    int productData = 0,
    int numItemsRequired = 0,
    bool checkAvailability = true)
  {
    return this.EnumerateBundlesForProductType(product, requireRealMoneyOption, productData, numItemsRequired, checkAvailability).ToList<Network.Bundle>();
  }

  public Network.Bundle GetLowestCostBundle(
    ProductType product,
    bool requireRealMoneyOption,
    int productData,
    int numItemsRequired = 0)
  {
    List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(product, requireRealMoneyOption, productData, numItemsRequired);
    Network.Bundle lowestCostBundle = (Network.Bundle) null;
    foreach (Network.Bundle bundle in bundlesForProduct)
    {
      if (numItemsRequired == 0 || bundle.Items.Count == numItemsRequired)
      {
        if ((Record) lowestCostBundle == (Record) null)
        {
          lowestCostBundle = bundle;
        }
        else
        {
          long? cost1 = lowestCostBundle.Cost;
          long? cost2 = bundle.Cost;
          if (!(cost1.GetValueOrDefault() <= cost2.GetValueOrDefault() & cost1.HasValue & cost2.HasValue))
            lowestCostBundle = bundle;
        }
      }
    }
    return lowestCostBundle;
  }

  public List<Network.Bundle> GetAvailableBundlesForProduct(
    ProductType productType,
    bool requireNonGoldPriceOption,
    int productData = 0,
    int numItemsRequired = 0)
  {
    List<Network.Bundle> bundlesForProduct = new List<Network.Bundle>();
    foreach (Network.Bundle bundle in this.m_bundles.Values)
    {
      if ((numItemsRequired == 0 || bundle.Items.Count == numItemsRequired) && (!requireNonGoldPriceOption || ShopUtils.BundleHasNonGoldPrice(bundle)) && bundle.Items.Any<Network.BundleItem>((Func<Network.BundleItem, bool>) (item =>
      {
        if (item.ItemType != productType)
          return false;
        return productData == 0 || productData == item.ProductData;
      })) && this.CanBuyBundle(bundle))
        bundlesForProduct.Add(bundle);
    }
    return bundlesForProduct;
  }

  private List<Network.Bundle> GetAllBundlesContainingItem(
    ProductType productType,
    int productData)
  {
    List<Network.Bundle> bundlesContainingItem = new List<Network.Bundle>();
    foreach (Network.Bundle bundle in this.m_bundles.Values)
    {
      bool flag = false;
      foreach (Network.BundleItem bundleItem in bundle.Items)
      {
        if (bundleItem.ItemType == productType && bundleItem.ProductData == productData)
          flag = true;
      }
      if (flag)
        bundlesContainingItem.Add(bundle);
    }
    return bundlesContainingItem;
  }

  public void GetAvailableAdventureBundle(
    AdventureDbId adventureId,
    bool requireNonGoldOption,
    out Network.Bundle bundle)
  {
    bundle = (Network.Bundle) null;
    if (StoreManager.GetAdventureProductType(adventureId) == ProductType.PRODUCT_TYPE_UNKNOWN)
      return;
    List<Network.Bundle> bundlesForProduct;
    switch (adventureId)
    {
      case AdventureDbId.NAXXRAMAS:
        bundlesForProduct = this.GetAvailableBundlesForProduct(ProductType.PRODUCT_TYPE_NAXX, requireNonGoldOption, 5);
        break;
      case AdventureDbId.BRM:
        bundlesForProduct = this.GetAvailableBundlesForProduct(ProductType.PRODUCT_TYPE_BRM, requireNonGoldOption, 10);
        break;
      case AdventureDbId.LOE:
        bundlesForProduct = this.GetAvailableBundlesForProduct(ProductType.PRODUCT_TYPE_LOE, requireNonGoldOption, 14);
        break;
      default:
        int finalAdventureWing = AdventureUtils.GetFinalAdventureWing((int) adventureId, false);
        bundlesForProduct = this.GetAvailableBundlesForProduct(ProductType.PRODUCT_TYPE_WING, requireNonGoldOption, finalAdventureWing);
        break;
    }
    if (bundlesForProduct == null)
      return;
    foreach (Network.Bundle bundle1 in bundlesForProduct)
    {
      int count = bundle1.Items.Count;
      if (count != 0 && (!requireNonGoldOption || ShopUtils.BundleHasNonGoldPrice(bundle1)) && this.IsBundleAvailableNow(bundle1))
      {
        if ((Record) bundle == (Record) null)
          bundle = bundle1;
        else if (bundle.Items.Count <= count)
          bundle = bundle1;
      }
    }
  }

  public bool CanBuyStorePackWithGold(StorePackId storePackId) => storePackId.Type == StorePackType.BOOSTER && this.CanBuyBoosterWithGold(storePackId.Id);

  private bool CanBuyBoosterWithGold(int boosterDbId)
  {
    BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterDbId);
    if (record == null)
      return false;
    SpecialEventType buyWithGoldEvent = record.BuyWithGoldEvent;
    switch (buyWithGoldEvent)
    {
      case SpecialEventType.UNKNOWN:
        return false;
      case SpecialEventType.IGNORE:
        return true;
      default:
        return SpecialEventManager.Get().IsEventActive(buyWithGoldEvent, false);
    }
  }

  public bool IsBoosterPreorderActive(
    int storePackIdData,
    ProductType productType,
    out Network.Bundle preOrderBundle)
  {
    foreach (Network.Bundle bundle in this.GetAllBundlesForProduct(productType, true, storePackIdData))
    {
      if (this.IsProductPrePurchase(bundle))
      {
        preOrderBundle = bundle;
        return true;
      }
    }
    preOrderBundle = (Network.Bundle) null;
    return false;
  }

  public bool IsBoosterHiddenLicenseBundle(
    StorePackId storePackId,
    out Network.Bundle hiddenLicenseBundle)
  {
    if (!GameUtils.IsHiddenLicenseBundleBooster(storePackId))
    {
      hiddenLicenseBundle = (Network.Bundle) null;
      return false;
    }
    IEnumerable<Network.Bundle> source = this.EnumerateBundlesForProductType(ProductType.PRODUCT_TYPE_HIDDEN_LICENSE, true, GameUtils.GetProductDataFromStorePackId(storePackId));
    hiddenLicenseBundle = source.FirstOrDefault<Network.Bundle>();
    return (Record) hiddenLicenseBundle != (Record) null;
  }

  public bool GetHeroBundleByCardDbId(int heroCardDbId, out Network.Bundle heroBundle)
  {
    foreach (Network.Bundle bundle in this.GetAllBundlesContainingItem(ProductType.PRODUCT_TYPE_HERO, heroCardDbId))
    {
      bool flag = false;
      foreach (Network.BundleItem bundleItem in bundle.Items)
      {
        if (bundleItem.ItemType == ProductType.PRODUCT_TYPE_HIDDEN_LICENSE)
          flag = true;
      }
      if (!flag)
      {
        heroBundle = bundle;
        return true;
      }
    }
    heroBundle = (Network.Bundle) null;
    return false;
  }

  public bool IsKoreanCustomer() => this.m_currency.SubRegion == 3;

  public bool IsEuropeanCustomer() => this.m_currency.SubRegion == 2 || this.m_currency.SubRegion == 10;

  public bool IsNorthAmericanCustomer() => this.m_currency.SubRegion == 1;

  public string GetTaxText()
  {
    switch (this.m_currency.TaxText)
    {
      case Currency.Tax.TAX_ADDED:
        return GameStrings.Get("GLUE_STORE_SUMMARY_TAX_DISCLAIMER_USD");
      case Currency.Tax.NO_TAX:
        return string.Empty;
      default:
        return GameStrings.Get("GLUE_STORE_SUMMARY_TAX_DISCLAIMER");
    }
  }

  public int GetCurrencyChangedVersion() => this.m_currency.ChangedVersion;

  public string GetCurrencyCode() => this.m_currency.Code;

  public CurrencyCache GetCurrencyCache(CurrencyType currencyType)
  {
    CurrencyCache currencyCache;
    if (!this.m_currencyCaches.TryGetValue(currencyType, out currencyCache))
    {
      currencyCache = new CurrencyCache(currencyType);
      currencyCache.BalanceAvailabilityChanged += new System.Action(this.OnCacheBalanceAvailabilityChanged);
      this.m_currencyCaches.Add(currencyType, currencyCache);
    }
    return currencyCache;
  }

  public string FormatCostBundle(Network.Bundle bundle)
  {
    if (!bundle.Cost.HasValue)
      return string.Empty;
    if (StoreManager.HasExternalStore && bundle.PMTProductID.HasValue)
    {
      string productPrice = this.GetProductPrice(ProductId.CreateFrom(bundle.PMTProductID.Value));
      if (!string.IsNullOrEmpty(productPrice))
        return productPrice;
    }
    return this.FormatCost(bundle.CostDisplay);
  }

  public string FormatCost(double? costDisplay)
  {
    string format = this.m_currency.GetFormat();
    CultureInfo cultureInfo = Localization.GetCultureInfo();
    cultureInfo.NumberFormat.CurrencySymbol = " " + this.m_currency.Symbol + " ";
    return string.Format((IFormatProvider) cultureInfo, format, (object) costDisplay).Replace("  ", " ").Trim();
  }

  public string GetProductName(Network.Bundle bundle)
  {
    if ((Record) bundle == (Record) null)
      return string.Empty;
    string title = bundle.GetTitle();
    if (!string.IsNullOrEmpty(title))
      return title;
    return bundle.Items.Count == 1 ? this.GetSingleItemProductName(bundle.Items[0]) : this.GetMultiItemProductName(bundle);
  }

  public int GetWingItemCount(List<Network.BundleItem> items)
  {
    int wingItemCount = 0;
    foreach (Network.BundleItem bundleItem in items)
    {
      if (AdventureUtils.IsProductTypeAnAdventureWing(bundleItem.ItemType))
        ++wingItemCount;
    }
    return wingItemCount;
  }

  public string GetProductQuantityText(
    ProductType product,
    int productData,
    int quantity,
    int baseQuantity)
  {
    string productQuantityText = string.Empty;
    switch (product)
    {
      case ProductType.PRODUCT_TYPE_BOOSTER:
        if (baseQuantity > 0)
        {
          int num = Math.Max(quantity - baseQuantity, 0);
          productQuantityText = GameStrings.Format("GLUE_STORE_QUANTITY_PACK_PLUS_BONUS", (object) baseQuantity, (object) num);
          break;
        }
        productQuantityText = GameStrings.Format("GLUE_STORE_QUANTITY_PACK", (object) quantity);
        break;
      case ProductType.PRODUCT_TYPE_DRAFT:
        productQuantityText = GameStrings.Format("GLUE_STORE_SUMMARY_ITEM_ORDERED", (object) quantity, (object) GameStrings.Get("GLUE_STORE_PRODUCT_NAME_FORGE_TICKET"));
        break;
      case ProductType.PRODUCT_TYPE_CURRENCY:
        productQuantityText = GameStrings.Format("GLUE_STORE_QUANTITY_DUST", (object) quantity);
        break;
      default:
        Log.Store.PrintWarning(string.Format("StoreManager.GetProductQuantityText(): don't know how to format quantity for product {0} (data {1})", (object) product, (object) productData));
        break;
    }
    return productQuantityText;
  }

  public void StartGeneralTransaction() => this.StartGeneralTransaction((GeneralStoreMode) StoreManager.s_defaultStoreMode);

  public void StartGeneralTransaction(GeneralStoreMode mode)
  {
    if (this.m_waitingToShowStore)
    {
      Log.Store.Print("StoreManager.StartGeneralTransaction(): already waiting to show store");
    }
    else
    {
      this.m_currentShopType = ShopType.GENERAL_STORE;
      this.m_showStoreData.exitCallback = (Store.ExitCallback) null;
      this.m_showStoreData.exitCallbackUserData = (object) null;
      this.m_showStoreData.isTotallyFake = false;
      this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_UNKNOWN;
      this.m_showStoreData.storeProductData = 0;
      this.m_showStoreData.storeMode = mode;
      this.m_showStoreData.useOverlayUI = true;
      this.m_showStoreData.closeOnTransactionComplete = false;
      this.ShowStoreWhenLoaded();
    }
  }

  public void StartArenaTransaction(
    Store.ExitCallback exitCallback,
    object exitCallbackUserData,
    bool isTotallyFake)
  {
    if (this.m_waitingToShowStore)
    {
      Log.Store.Print("StoreManager.StartArenaTransaction(): already waiting to show store");
    }
    else
    {
      this.m_currentShopType = ShopType.ARENA_STORE;
      this.m_showStoreData.exitCallback = exitCallback;
      this.m_showStoreData.exitCallbackUserData = (object) null;
      this.m_showStoreData.isTotallyFake = isTotallyFake;
      this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_UNKNOWN;
      this.m_showStoreData.storeProductData = 0;
      this.m_showStoreData.useOverlayUI = false;
      this.m_showStoreData.closeOnTransactionComplete = false;
      this.ShowStoreWhenLoaded();
    }
  }

  public void StartTavernBrawlTransaction(Store.ExitCallback exitCallback, bool isTotallyFake)
  {
    if (this.m_waitingToShowStore)
    {
      Log.Store.Print("StoreManager.StartTavernBrawlTransaction(): already waiting to show store");
    }
    else
    {
      this.m_currentShopType = ShopType.TAVERN_BRAWL_STORE;
      this.m_showStoreData.exitCallback = exitCallback;
      this.m_showStoreData.exitCallbackUserData = (object) null;
      this.m_showStoreData.isTotallyFake = isTotallyFake;
      this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_UNKNOWN;
      this.m_showStoreData.storeProductData = 0;
      this.m_showStoreData.useOverlayUI = false;
      this.m_showStoreData.closeOnTransactionComplete = false;
      this.ShowStoreWhenLoaded();
    }
  }

  public void StartBattlegroundsTransaction(Store.ExitCallback exitCallback, bool isTotallyFake)
  {
    if (this.m_waitingToShowStore)
    {
      Log.Store.Print("StartBattlegroundsTransaction(): already waiting to show store");
    }
    else
    {
      this.m_currentShopType = ShopType.BATTLEGROUNDS_STORE;
      this.m_showStoreData.exitCallback = exitCallback;
      this.m_showStoreData.exitCallbackUserData = (object) null;
      this.m_showStoreData.isTotallyFake = isTotallyFake;
      this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_UNKNOWN;
      this.m_showStoreData.storeProductData = 0;
      this.m_showStoreData.useOverlayUI = false;
      this.m_showStoreData.closeOnTransactionComplete = false;
      this.ShowStoreWhenLoaded();
    }
  }

  public void StartAdventureTransaction(
    ProductType product,
    int productData,
    Store.ExitCallback exitCallback,
    object exitCallbackUserData,
    ShopType shopType,
    int numItemsRequired = 0,
    bool useOverlayUI = false,
    IDataModel dataModel = null,
    int pmtProductId = 0)
  {
    if (this.m_waitingToShowStore)
      Log.Store.Print("StoreManager.StartAdventureTransaction(): already waiting to show store");
    else if (!this.CanBuyProductItem(product, productData, StoreManager.InferProductItemPurchaseRuleFromProductType(product)))
    {
      Log.Store.PrintWarning("StoreManager.StartAdventureTransaction(): cannot buy product item");
    }
    else
    {
      this.m_currentShopType = shopType;
      this.m_showStoreData.exitCallback = exitCallback;
      this.m_showStoreData.exitCallbackUserData = exitCallbackUserData;
      this.m_showStoreData.isTotallyFake = false;
      this.m_showStoreData.storeProduct = product;
      this.m_showStoreData.storeProductData = productData;
      this.m_showStoreData.numItemsRequired = numItemsRequired;
      this.m_showStoreData.dataModel = dataModel;
      this.m_showStoreData.useOverlayUI = useOverlayUI;
      this.m_showStoreData.pmtProductId = pmtProductId;
      this.m_showStoreData.closeOnTransactionComplete = false;
      this.ShowStoreWhenLoaded();
    }
  }

  public void StartMercenariesTransaction(Store.ExitCallback exitCallback, bool isTotallyFake)
  {
    if (this.m_waitingToShowStore)
    {
      Log.Store.Print("StartMercenariesTransaction(): already waiting to show store");
    }
    else
    {
      this.m_currentShopType = ShopType.MERCENARIES_STORE;
      this.m_showStoreData.exitCallback = exitCallback;
      this.m_showStoreData.exitCallbackUserData = (object) null;
      this.m_showStoreData.isTotallyFake = isTotallyFake;
      this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_UNKNOWN;
      this.m_showStoreData.storeProductData = 0;
      this.m_showStoreData.useOverlayUI = false;
      this.m_showStoreData.closeOnTransactionComplete = false;
      this.ShowStoreWhenLoaded();
    }
  }

  public void StartFakeStoreForMercenariesWorkshop()
  {
    this.m_showStoreData.storeProductData = (int) this.m_currentShopType;
    this.m_currentShopType = ShopType.MERCENARIES_WORKSHOP;
    this.m_showStoreData.isTotallyFake = false;
    this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_UNKNOWN;
    this.m_showStoreData.storeProductData = 0;
    this.m_showStoreData.useOverlayUI = false;
    this.m_showStoreData.closeOnTransactionComplete = true;
  }

  public void StopFakeMercenariesWorkshopStoreAndRestorePrevious() => this.m_currentShopType = (ShopType) this.m_showStoreData.storeProductData;

  public void SetupDuelsStore(DuelsPopupManager duelsPopupManager)
  {
    this.m_currentShopType = ShopType.DUELS_STORE;
    this.m_showStoreData.isTotallyFake = false;
    this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_UNKNOWN;
    this.m_showStoreData.storeProductData = 0;
    this.m_showStoreData.useOverlayUI = false;
    this.m_showStoreData.closeOnTransactionComplete = true;
    this.m_showStoreData.exitCallback = (Store.ExitCallback) null;
    this.m_showStoreData.exitCallbackUserData = (object) null;
    this.m_stores[ShopType.DUELS_STORE] = (IStore) duelsPopupManager;
    this.SetupLoadedStore((IStore) duelsPopupManager);
    if (this.m_view.HasStartedLoading)
    {
      this.ShowStore();
    }
    else
    {
      this.m_showStoreStart = Time.realtimeSinceStartup;
      this.m_waitingToShowStore = true;
      this.m_view.LoadAssets();
    }
  }

  public void ShutDownDuelsStore()
  {
    if (!this.m_stores.ContainsKey(ShopType.DUELS_STORE))
      return;
    this.m_stores.Remove(ShopType.DUELS_STORE);
  }

  public void SetupCardBackStore(CardBackInfoManager cardBackInfoManager, int productData)
  {
    this.m_currentShopType = ShopType.CARD_BACK_STORE;
    this.m_showStoreData.isTotallyFake = false;
    this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_CARD_BACK;
    this.m_showStoreData.storeProductData = productData;
    this.m_showStoreData.useOverlayUI = false;
    this.m_showStoreData.closeOnTransactionComplete = true;
    this.m_showStoreData.exitCallback = (Store.ExitCallback) null;
    this.m_showStoreData.exitCallbackUserData = (object) null;
    this.m_stores[ShopType.CARD_BACK_STORE] = (IStore) cardBackInfoManager;
    this.SetupLoadedStore((IStore) cardBackInfoManager);
    if (this.m_view.HasStartedLoading)
    {
      this.ShowStore();
    }
    else
    {
      this.m_showStoreStart = Time.realtimeSinceStartup;
      this.m_waitingToShowStore = true;
      this.m_view.LoadAssets();
    }
  }

  public void ShutDownCardBackStore()
  {
    if (!this.m_stores.ContainsKey(ShopType.CARD_BACK_STORE))
      return;
    this.m_stores.Remove(ShopType.CARD_BACK_STORE);
  }

  public void SetupHeroSkinStore(HeroSkinInfoManager heroSkinInfoManager, int productData)
  {
    this.m_currentShopType = ShopType.HERO_SKIN_STORE;
    this.m_showStoreData.isTotallyFake = false;
    this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_HERO;
    this.m_showStoreData.storeProductData = productData;
    this.m_showStoreData.useOverlayUI = false;
    this.m_showStoreData.closeOnTransactionComplete = true;
    this.m_showStoreData.exitCallback = (Store.ExitCallback) null;
    this.m_showStoreData.exitCallbackUserData = (object) null;
    this.m_stores[ShopType.HERO_SKIN_STORE] = (IStore) heroSkinInfoManager;
    this.SetupLoadedStore((IStore) heroSkinInfoManager);
    if (this.m_view.HasStartedLoading)
    {
      this.ShowStore();
    }
    else
    {
      this.m_showStoreStart = Time.realtimeSinceStartup;
      this.m_waitingToShowStore = true;
      this.m_view.LoadAssets();
    }
  }

  public void ShutDownHeroSkinStore()
  {
    if (!this.m_stores.ContainsKey(ShopType.HERO_SKIN_STORE))
      return;
    this.m_stores.Remove(ShopType.HERO_SKIN_STORE);
  }

  public void StartLuckyDrawStore(LuckyDrawWidget luckyDrawWidget)
  {
    this.m_currentShopType = ShopType.LUCKY_DRAW_STORE;
    this.m_showStoreData.isTotallyFake = false;
    this.m_showStoreData.storeProduct = ProductType.PRODUCT_TYPE_LUCKY_DRAW;
    this.m_showStoreData.useOverlayUI = true;
    this.m_showStoreData.closeOnTransactionComplete = false;
    this.m_showStoreData.exitCallback = (Store.ExitCallback) null;
    this.m_showStoreData.exitCallbackUserData = (object) null;
    this.m_stores[ShopType.LUCKY_DRAW_STORE] = (IStore) luckyDrawWidget;
    this.SetupLoadedStore((IStore) luckyDrawWidget);
    this.Status = StoreManager.TransactionStatus.READY;
    if (this.m_view.HasStartedLoading)
    {
      this.ShowStore();
    }
    else
    {
      this.m_showStoreStart = Time.realtimeSinceStartup;
      this.m_waitingToShowStore = true;
      this.m_view.LoadAssets();
    }
  }

  public void ShutDownLuckyDrawStore()
  {
    if (!this.m_stores.ContainsKey(ShopType.LUCKY_DRAW_STORE))
      return;
    this.m_stores.Remove(ShopType.LUCKY_DRAW_STORE);
  }

  public void HandleDisconnect()
  {
    if (this.IsShown() && !this.TransactionInProgress())
    {
      while (this.IsPromptShowing)
        Navigation.GoBack();
      this.GetCurrentStore()?.Close();
      DialogManager.Get().ShowReconnectHelperDialog();
    }
    this.FireStatusChangedEventIfNeeded();
  }

  public void HideStore(ShopType shopType)
  {
    IStore store = this.GetStore(shopType);
    if (store == null)
      return;
    store.Close();
    this.m_view.Hide();
    BnetBar.Get()?.RefreshCurrency();
  }

  public bool TransactionInProgress() => this.Status != StoreManager.TransactionStatus.READY;

  public bool IsPromptShowing => this.m_view.IsPromptShowing() || this.IsCommerceUiShowing();

  public bool HasOutstandingPurchaseNotices(ProductType product) => this.m_outstandingPurchaseNotices.Where<NetCache.ProfileNoticePurchase>((Func<NetCache.ProfileNoticePurchase, bool>) (notice => notice.PMTProductID.HasValue)).Select<NetCache.ProfileNoticePurchase, ProductId>((Func<NetCache.ProfileNoticePurchase, ProductId>) (notice => ProductId.CreateFrom(notice.PMTProductID.Value))).Where<ProductId>((Func<ProductId, bool>) (prodId => prodId.IsValid())).Select<ProductId, Network.Bundle>(new Func<ProductId, Network.Bundle>(this.GetBundleFromPmtProductId)).Where<Network.Bundle>((Func<Network.Bundle, bool>) (bundle => (Record) bundle != (Record) null)).SelectMany<Network.Bundle, Network.BundleItem>((Func<Network.Bundle, IEnumerable<Network.BundleItem>>) (bundle => (IEnumerable<Network.BundleItem>) bundle.Items)).Any<Network.BundleItem>((Func<Network.BundleItem, bool>) (item => item.ItemType == product));

  public static ProductType GetAdventureProductType(AdventureDbId adventureId)
  {
    ProductType adventureProductType;
    if (StoreManager.s_adventureToProductMap.TryGetValue(adventureId, out adventureProductType))
      return adventureProductType;
    return GameUtils.IsExpansionAdventure(adventureId) ? ProductType.PRODUCT_TYPE_WING : ProductType.PRODUCT_TYPE_UNKNOWN;
  }

  public bool IsIdActiveTransaction(long id) => this.m_activeMoneyOrGTAPPTransaction != null && id == this.m_activeMoneyOrGTAPPTransaction.ID;

  public bool IsPMTProductIDActiveTransaction(long id)
  {
    if (this.m_activeMoneyOrGTAPPTransaction == null)
      return false;
    long num = id;
    long? pmtProductId = this.m_activeMoneyOrGTAPPTransaction.PMTProductID;
    long valueOrDefault = pmtProductId.GetValueOrDefault();
    return num == valueOrDefault & pmtProductId.HasValue;
  }

  public static bool IsFirstPurchaseBundleOwned()
  {
    HiddenLicenseDbfRecord record1 = GameDbf.HiddenLicense.GetRecord(40);
    if (record1 == null)
      return false;
    AccountLicenseDbfRecord record2 = GameDbf.AccountLicense.GetRecord(record1.AccountLicenseId);
    return record2 != null && AccountLicenseMgr.Get().OwnsAccountLicense(record2.LicenseId);
  }

  private void OnAccountLicensesUpdate(
    List<AccountLicenseInfo> changedAccountLicenses,
    object userData)
  {
    this.Catalog.UpdateProductStatus();
  }

  private static StoreManager.LicenseStatus GetHiddenLicenseStatus(int hiddenLicenseId)
  {
    HiddenLicenseDbfRecord record1 = GameDbf.HiddenLicense.GetRecord(hiddenLicenseId);
    if (record1 == null)
      return StoreManager.LicenseStatus.UNDEFINED;
    AccountLicenseDbfRecord record2 = GameDbf.AccountLicense.GetRecord(record1.AccountLicenseId);
    if (record2 == null)
      return StoreManager.LicenseStatus.UNDEFINED;
    if (!AccountLicenseMgr.Get().OwnsAccountLicense(record2.LicenseId))
      return StoreManager.LicenseStatus.NOT_OWNED;
    return !record1.IsBlocking ? StoreManager.LicenseStatus.OWNED : StoreManager.LicenseStatus.OWNED_AND_BLOCKING;
  }

  public static bool IsHiddenLicenseBundleOwned(int hiddenLicenseId)
  {
    StoreManager.LicenseStatus hiddenLicenseStatus = StoreManager.GetHiddenLicenseStatus(hiddenLicenseId);
    return hiddenLicenseStatus == StoreManager.LicenseStatus.OWNED || hiddenLicenseStatus == StoreManager.LicenseStatus.OWNED_AND_BLOCKING;
  }

  public void SetCurrentlySelectedStorePack(StorePackId storePackId) => this.m_currentlySelectedId = storePackId;

  private ModularBundleLayoutDbfRecord GetRegionNodeLayoutForHiddenLicense(
    int hiddenLicenseId)
  {
    foreach (ModularBundleLayoutDbfRecord record in GameDbf.ModularBundleLayout.GetRecords())
    {
      if (record.HiddenLicenseId == hiddenLicenseId)
      {
        string regions = record.Regions;
        char[] chArray = new char[1]{ ',' };
        foreach (string str in regions.Split(chArray))
        {
          if (Blizzard.T5.Core.Utils.EnumUtils.SafeParse<BnetRegion>(str) == this.m_regionId)
            return record;
        }
      }
    }
    Log.Store.PrintWarning(string.Format("Unable to load layout for hidden license id={0}, region={1}. Using Default Node Layout.", (object) hiddenLicenseId, (object) this.m_regionId));
    return GameDbf.ModularBundleLayout.GetRecord((Predicate<ModularBundleLayoutDbfRecord>) (r => r.ModularBundleId == hiddenLicenseId));
  }

  public List<ModularBundleLayoutDbfRecord> GetRegionNodeLayoutsForBundle(
    int modularBundleRecordId)
  {
    List<ModularBundleLayoutDbfRecord> layoutsForBundle = new List<ModularBundleLayoutDbfRecord>();
    foreach (ModularBundleLayoutDbfRecord record in GameDbf.ModularBundleLayout.GetRecords())
    {
      if (record.ModularBundleId == modularBundleRecordId)
      {
        string regions = record.Regions;
        char[] chArray = new char[1]{ ',' };
        foreach (string str in regions.Split(chArray))
        {
          if (Blizzard.T5.Core.Utils.EnumUtils.SafeParse<BnetRegion>(str) == this.m_regionId)
            layoutsForBundle.Add(record);
        }
      }
    }
    if (layoutsForBundle.Count == 0)
    {
      Log.Store.PrintWarning(string.Format("Unable to load layout for modular bundle id={0}, region={1}. Using Default Node Layout.", (object) modularBundleRecordId, (object) this.m_regionId));
      layoutsForBundle.Add(GameDbf.ModularBundleLayout.GetRecord((Predicate<ModularBundleLayoutDbfRecord>) (r => r.ModularBundleId == modularBundleRecordId)));
    }
    return layoutsForBundle;
  }

  private void ShowStoreWhenLoaded()
  {
    this.m_showStoreStart = Time.realtimeSinceStartup;
    HearthstonePerformance hearthstonePerformance = HearthstonePerformance.Get();
    if (hearthstonePerformance != null)
      hearthstonePerformance.StartPerformanceFlow((FlowPerformance.SetupConfig) new FlowPerformanceShop.ShopSetupConfig()
      {
        shopType = this.m_currentShopType
      });
    this.m_waitingToShowStore = true;
    if (!this.IsCurrentStoreLoaded())
      this.Load(this.m_currentShopType);
    else
      this.ShowStore();
  }

  private void ShowStore()
  {
    if (!this.m_licenseAchievesListenerRegistered)
    {
      AchieveManager.Get().RegisterLicenseAddedAchievesUpdatedListener(new AchieveManager.LicenseAddedAchievesUpdatedCallback(this.OnLicenseAddedAchievesUpdated));
      this.m_licenseAchievesListenerRegistered = true;
    }
    if (StoreManager.TransactionStatus.READY == this.Status && AchieveManager.Get().HasActiveLicenseAddedAchieves())
      this.Status = StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE;
    IStore currentStore = this.GetCurrentStore();
    bool flag1 = true;
    bool flag2 = false;
    switch (this.m_currentShopType)
    {
      case ShopType.GENERAL_STORE:
        if (this.IsOpen())
        {
          if (this.IsVintageStoreEnabled())
          {
            ((GeneralStore) currentStore).SetMode(this.m_showStoreData.storeMode);
            break;
          }
          break;
        }
        Log.Store.PrintWarning("StoreManager.ShowStore(): Cannot show general store.. Store is not open");
        if (this.m_showStoreData.exitCallback != null)
          this.m_showStoreData.exitCallback(false, this.m_showStoreData.exitCallbackUserData);
        flag1 = false;
        break;
      case ShopType.ADVENTURE_STORE:
      case ShopType.ADVENTURE_STORE_WING_PURCHASE_WIDGET:
      case ShopType.ADVENTURE_STORE_FULL_PURCHASE_WIDGET:
        if (this.IsOpen())
        {
          AdventureStore adventureStore = (AdventureStore) currentStore;
          if ((UnityEngine.Object) adventureStore != (UnityEngine.Object) null)
          {
            ProductId from = ProductId.CreateFrom((long) this.m_showStoreData.pmtProductId);
            adventureStore.SetAdventureProduct(this.m_showStoreData.storeProduct, this.m_showStoreData.storeProductData, this.m_showStoreData.numItemsRequired, from);
            break;
          }
          break;
        }
        Log.Store.PrintWarning("StoreManager.ShowStore(): Cannot show adventure store.. Store is not open");
        if (this.m_showStoreData.exitCallback != null)
          this.m_showStoreData.exitCallback(false, this.m_showStoreData.exitCallbackUserData);
        flag1 = false;
        flag2 = true;
        break;
      case ShopType.DUELS_STORE:
        if (!this.IsOpen())
        {
          flag1 = false;
          break;
        }
        break;
      case ShopType.BATTLEGROUNDS_STORE:
        if (!this.IsOpen())
        {
          flag1 = false;
          break;
        }
        break;
      case ShopType.MERCENARIES_STORE:
        if (!this.IsOpen())
        {
          flag1 = false;
          break;
        }
        break;
      case ShopType.MERCENARIES_WORKSHOP:
        if (!this.IsOpen())
        {
          flag1 = false;
          break;
        }
        break;
      case ShopType.LUCKY_DRAW_STORE:
        if (!this.IsOpen())
        {
          flag1 = false;
          break;
        }
        break;
    }
    if (flag2)
    {
      this.ShowStoreUnavailableAlert();
      this.m_waitingToShowStore = false;
    }
    else
    {
      if (flag1 && currentStore != null)
      {
        if (currentStore is Store store)
          store.Show(this.m_showStoreData.isTotallyFake, this.m_showStoreData.useOverlayUI, this.m_showStoreData.dataModel);
        else
          currentStore.Open();
      }
      bool blocked = false;
      currentStore?.BlockInterface(blocked);
      Log.Store.Print("{0} took {1}s to load", (object) this.m_currentShopType, (object) (float) ((double) Time.realtimeSinceStartup - (double) this.m_showStoreStart));
      this.m_waitingToShowStore = false;
    }
  }

  private void ShowStoreUnavailableAlert()
  {
    if (this.m_isShowingStoreUnavailableAlert)
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_SHOP_CLOSED_ALERT_HEADER"),
      m_text = GameStrings.Get("GLUE_SHOP_CLOSED_ALERT_TEXT"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = (AlertPopup.ResponseCallback) ((resp, data) => this.m_isShowingStoreUnavailableAlert = false)
    };
    DialogManager.Get().ShowPopup(info);
    this.m_isShowingStoreUnavailableAlert = true;
  }

  private void OnLoginCompleted() => this.FireStatusChangedEventIfNeeded();

  private void OnCacheBalanceAvailabilityChanged() => this.m_isStatusRefreshPending = true;

  private void HandleShopAvailabilityChange()
  {
    if (!this.IsOpen() && this.IsShown() && !this.TransactionInProgress())
    {
      while (this.IsPromptShowing)
        Navigation.GoBack();
      this.GetCurrentStore()?.Close();
      this.ShowStoreUnavailableAlert();
    }
    this.FireStatusChangedEventIfNeeded();
  }

  private StorePurchaseAuth.ButtonStyle GetPurchaseAuthButtonStyle(
    ShopType shopType)
  {
    switch (this.m_currentShopType)
    {
      case ShopType.ARENA_STORE:
      case ShopType.TAVERN_BRAWL_STORE:
        return StorePurchaseAuth.ButtonStyle.Back;
      default:
        return StorePurchaseAuth.ButtonStyle.NoButton;
    }
  }

  private bool IsCurrentStoreLoaded()
  {
    IStore currentStore = this.GetCurrentStore();
    return currentStore != null && currentStore.IsReady() && this.m_view.IsLoaded();
  }

  private void Load(ShopType shopType)
  {
    bool flag = true;
    if (this.GetCurrentStore() != null)
      return;
    switch (shopType)
    {
      case ShopType.GENERAL_STORE:
        CollectionManager collectionManager = CollectionManager.Get();
        if (collectionManager.IsLettuceLoaded())
        {
          this.LoadGeneralStore();
          break;
        }
        collectionManager.OnLettuceLoaded += new System.Action(this.OnLettuceCollectionLoaded);
        collectionManager.StartInitialMercenaryLoadIfRequired();
        flag = false;
        break;
      case ShopType.ARENA_STORE:
        WidgetInstance arenaStoreWidget = WidgetInstance.Create((string) ShopPrefabs.ArenaShopPrefab);
        arenaStoreWidget.RegisterReadyListener((System.Action<object>) (_ => this.OnArenaStoreLoaded((AssetReference) null, arenaStoreWidget.gameObject, (object) null)), (object) null, true);
        break;
      case ShopType.ADVENTURE_STORE:
        WidgetInstance adventureStoreWidget = WidgetInstance.Create((string) ShopPrefabs.AdventureShopPrefab);
        adventureStoreWidget.RegisterReadyListener((System.Action<object>) (_ => this.OnAdventureStoreLoaded((AssetReference) null, adventureStoreWidget.gameObject, (object) null)), (object) null, true);
        break;
      case ShopType.TAVERN_BRAWL_STORE:
        WidgetInstance brawlStoreWidget = WidgetInstance.Create((string) ShopPrefabs.TavernBrawlShopPrefab);
        brawlStoreWidget.RegisterReadyListener((System.Action<object>) (_ => this.OnBrawlStoreLoaded((AssetReference) null, brawlStoreWidget.gameObject, (object) null)), (object) null, true);
        break;
      case ShopType.ADVENTURE_STORE_WING_PURCHASE_WIDGET:
        WidgetInstance wingWidget = WidgetInstance.Create("AdventureStorymodeChapterStore.prefab:b797807e5c127af47badd08be121ea16");
        wingWidget.RegisterReadyListener((System.Action<object>) (_ => this.OnAdventureWingStoreLoaded((AssetReference) null, wingWidget.gameObject, (object) null)), (object) null, true);
        break;
      case ShopType.ADVENTURE_STORE_FULL_PURCHASE_WIDGET:
        WidgetInstance bookWidget = WidgetInstance.Create("AdventureStorymodeBookStore.prefab:922203a90d48c1d47b2f6813ff72f160");
        bookWidget.RegisterReadyListener((System.Action<object>) (_ => this.OnAdventureFullStoreLoaded((AssetReference) null, bookWidget.gameObject, (object) null)), (object) null, true);
        break;
      case ShopType.BATTLEGROUNDS_STORE:
        Shop shop1 = Shop.Get();
        this.m_stores[shopType] = (IStore) shop1;
        break;
      case ShopType.MERCENARIES_STORE:
        Shop shop2 = Shop.Get();
        this.m_stores[shopType] = (IStore) shop2;
        break;
    }
    if (!flag)
      return;
    this.m_view.LoadAssets();
  }

  private void LoadGeneralStore()
  {
    if (this.IsVintageStoreEnabled())
      AssetLoader.Get().InstantiatePrefab((AssetReference) (string) ShopPrefabs.ShopPrefab, new PrefabCallback<GameObject>(this.OnGeneralStoreLoaded));
    else
      this.m_stores[ShopType.GENERAL_STORE] = (IStore) Shop.Get();
  }

  private void UnloadAndFreeMemory()
  {
    if ((UnityEngine.Object) Shop.Get() != (UnityEngine.Object) null)
      Shop.Get().Unload();
    foreach (KeyValuePair<ShopType, IStore> store in this.m_stores)
      store.Value?.Unload();
    this.m_stores.Clear();
    this.m_view.UnloadAssets();
  }

  private StoreManager.TransactionStatus Status
  {
    get => this.m_status;
    set
    {
      if (0.0 == (double) this.m_lastCancelRequestTime && this.m_status == StoreManager.TransactionStatus.UNKNOWN)
        this.m_lastCancelRequestTime = Time.realtimeSinceStartup;
      this.m_status = value;
      this.FireStatusChangedEventIfNeeded();
    }
  }

  private bool FirstNoticesProcessed
  {
    get => this.m_firstNoticesProcessed;
    set
    {
      this.m_firstNoticesProcessed = value;
      this.FireStatusChangedEventIfNeeded();
    }
  }

  public bool BattlePayAvailable
  {
    get => this.m_battlePayAvailable;
    set
    {
      this.m_battlePayAvailable = value;
      this.FireStatusChangedEventIfNeeded();
    }
  }

  private bool FeaturesReady
  {
    get => this.m_featuresReady;
    set
    {
      this.m_featuresReady = value;
      this.FireStatusChangedEventIfNeeded();
    }
  }

  private bool ConfigLoaded
  {
    get => this.m_configLoaded;
    set
    {
      this.m_configLoaded = value;
      this.FireStatusChangedEventIfNeeded();
    }
  }

  private void FireStatusChangedEventIfNeeded()
  {
    bool flag = this.IsOpen();
    if (this.m_openWhenLastEventFired == flag)
      return;
    this.OnStatusChanged(flag);
    this.m_openWhenLastEventFired = flag;
  }

  private NetCache.NetCacheFeatures GetNetCacheFeatures()
  {
    if (!this.FeaturesReady)
      return (NetCache.NetCacheFeatures) null;
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject != null)
      return netObject;
    this.FeaturesReady = false;
    return netObject;
  }

  private static ItemPurchaseRule GetProductItemPurchaseRule(Network.BundleItem item) => !item.IsBlocking ? ItemPurchaseRule.NO_LIMIT : ItemPurchaseRule.BLOCKING;

  private static ItemPurchaseRule InferProductItemPurchaseRuleFromProductType(
    ProductType product)
  {
    switch (product)
    {
      case ProductType.PRODUCT_TYPE_BOOSTER:
      case ProductType.PRODUCT_TYPE_DRAFT:
      case ProductType.PRODUCT_TYPE_RANDOM_CARD:
      case ProductType.PRODUCT_TYPE_TAVERN_BRAWL_TICKET:
      case ProductType.PRODUCT_TYPE_CURRENCY:
      case ProductType.PRODUCT_TYPE_MERCENARIES_MERCENARY:
      case ProductType.PRODUCT_TYPE_MERCENARIES_CURRENCY:
      case ProductType.PRODUCT_TYPE_MERCENARIES_BOOSTER:
      case ProductType.PRODUCT_TYPE_MERCENARIES_RANDOM_REWARD:
      case ProductType.PRODUCT_TYPE_MERCENARIES_KNOCKOUT_SPECIFIC:
      case ProductType.PRODUCT_TYPE_MERCENARIES_KNOCKOUT_RANDOM:
        return ItemPurchaseRule.NO_LIMIT;
      case ProductType.PRODUCT_TYPE_NAXX:
      case ProductType.PRODUCT_TYPE_BRM:
      case ProductType.PRODUCT_TYPE_LOE:
      case ProductType.PRODUCT_TYPE_WING:
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BONUS:
      case ProductType.PRODUCT_TYPE_PROGRESSION_BONUS:
        return ItemPurchaseRule.BLOCKING;
      case ProductType.PRODUCT_TYPE_CARD_BACK:
      case ProductType.PRODUCT_TYPE_HERO:
      case ProductType.PRODUCT_TYPE_FIXED_LICENSE:
      case ProductType.PRODUCT_TYPE_MINI_SET:
      case ProductType.PRODUCT_TYPE_SELLABLE_DECK:
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BOARD_SKIN:
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_FINISHER:
      case ProductType.PRODUCT_TYPE_DIAMOND_CARD:
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_EMOTE:
      case ProductType.PRODUCT_TYPE_LUCKY_DRAW:
        return ItemPurchaseRule.NO_LIMIT;
      case ProductType.PRODUCT_TYPE_HIDDEN_LICENSE:
        return ItemPurchaseRule.BLOCKING;
      default:
        return ItemPurchaseRule.UNDEFINED;
    }
  }

  public static ItemOwnershipStatus GetProductItemOwnershipStatus(
    ProductType product,
    int productData,
    out string failReason)
  {
    failReason = (string) null;
    switch (product)
    {
      case ProductType.PRODUCT_TYPE_BOOSTER:
      case ProductType.PRODUCT_TYPE_DRAFT:
      case ProductType.PRODUCT_TYPE_RANDOM_CARD:
      case ProductType.PRODUCT_TYPE_TAVERN_BRAWL_TICKET:
      case ProductType.PRODUCT_TYPE_CURRENCY:
      case ProductType.PRODUCT_TYPE_MINI_SET:
      case ProductType.PRODUCT_TYPE_SELLABLE_DECK:
      case ProductType.PRODUCT_TYPE_MERCENARIES_MERCENARY:
      case ProductType.PRODUCT_TYPE_MERCENARIES_CURRENCY:
      case ProductType.PRODUCT_TYPE_MERCENARIES_BOOSTER:
      case ProductType.PRODUCT_TYPE_MERCENARIES_RANDOM_REWARD:
      case ProductType.PRODUCT_TYPE_MERCENARIES_KNOCKOUT_SPECIFIC:
      case ProductType.PRODUCT_TYPE_MERCENARIES_KNOCKOUT_RANDOM:
        return ItemOwnershipStatus.IGNORED;
      case ProductType.PRODUCT_TYPE_NAXX:
      case ProductType.PRODUCT_TYPE_BRM:
      case ProductType.PRODUCT_TYPE_LOE:
      case ProductType.PRODUCT_TYPE_WING:
        if (!AdventureProgressMgr.Get().IsReady)
        {
          failReason = string.Format("Adventure Progress Manager not ready to determine ownership of chapter Type={0}, ID={1}", (object) product, (object) productData);
          return ItemOwnershipStatus.UNDEFINED;
        }
        return !AdventureProgressMgr.Get().OwnsWing(productData) ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      case ProductType.PRODUCT_TYPE_CARD_BACK:
        return !CardBackManager.Get().IsCardBackOwned(productData) ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      case ProductType.PRODUCT_TYPE_HERO:
        if (NetCache.Get().GetNetObject<NetCache.NetCacheCollection>() == null)
        {
          failReason = string.Format("Collection not received to determine ownership of hero card ID={0}", (object) productData);
          return ItemOwnershipStatus.UNDEFINED;
        }
        string cardId1 = GameUtils.TranslateDbIdToCardId(productData);
        return !(!CollectionManager.Get().IsBattlegroundsHeroSkinCard(productData) ? (!CollectionManager.Get().IsBattlegroundsGuideSkinCard(productData) ? cardId1 != null && CollectionManager.Get().IsCardInCollection(cardId1, TAG_PREMIUM.NORMAL) : CollectionManager.Get().OwnsBattlegroundsGuideSkin(productData)) : CollectionManager.Get().OwnsBattlegroundsHeroSkin(cardId1)) ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      case ProductType.PRODUCT_TYPE_HIDDEN_LICENSE:
        if (AccountLicenseMgr.Get().FixedLicensesState != AccountLicenseMgr.LicenseUpdateState.SUCCESS)
        {
          failReason = string.Format("Fixed licenses not received to determine ownership of hidden license ID={0}", (object) productData);
          return ItemOwnershipStatus.UNDEFINED;
        }
        HiddenLicenseDbfRecord record1 = GameDbf.HiddenLicense.GetRecord(productData);
        if (record1 == null)
        {
          failReason = string.Format("Hidden license has unknown ID in HIDDEN_LICENSE table record ID={0}", (object) productData);
          return ItemOwnershipStatus.UNDEFINED;
        }
        if (GameDbf.AccountLicense.GetRecord(record1.AccountLicenseId) == null)
        {
          failReason = string.Format("HIDDEN_LICENSE record {0} pointing to missing ACCOUNT_LICENSE record ID={1}", (object) productData, (object) record1.AccountLicenseId);
          return ItemOwnershipStatus.UNDEFINED;
        }
        switch (StoreManager.GetHiddenLicenseStatus(productData))
        {
          case StoreManager.LicenseStatus.NOT_OWNED:
            return ItemOwnershipStatus.UNOWNED;
          case StoreManager.LicenseStatus.OWNED:
          case StoreManager.LicenseStatus.OWNED_AND_BLOCKING:
            return ItemOwnershipStatus.OWNED;
          default:
            failReason = string.Format("Hidden license has undefined ownership status. HIDDEN_LIENSE table record ID={0}", (object) productData);
            return ItemOwnershipStatus.UNDEFINED;
        }
      case ProductType.PRODUCT_TYPE_FIXED_LICENSE:
        if (AccountLicenseMgr.Get().FixedLicensesState != AccountLicenseMgr.LicenseUpdateState.SUCCESS)
        {
          failReason = string.Format("Fixed licenses not received to determine ownership of ACCOUNT_LICENSE table record ID={0} for license type={1}", (object) productData, (object) product);
          return ItemOwnershipStatus.UNDEFINED;
        }
        return !AccountLicenseMgr.Get().OwnsAccountLicense((long) productData) ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BONUS:
      case ProductType.PRODUCT_TYPE_PROGRESSION_BONUS:
      case ProductType.PRODUCT_TYPE_LUCKY_DRAW:
        if (AccountLicenseMgr.Get().FixedLicensesState != AccountLicenseMgr.LicenseUpdateState.SUCCESS)
        {
          failReason = string.Format("Fixed licenses not received to determine ownership of ACCOUNT_LICENSE table record ID={0} for license type={1}", (object) productData, (object) product);
          return ItemOwnershipStatus.UNDEFINED;
        }
        AccountLicenseDbfRecord record2 = GameDbf.AccountLicense.GetRecord(productData);
        if (record2 == null)
        {
          failReason = string.Format("Fixed licenses not received to determine ownership of ACCOUNT_LICENSE table record ID={0} for license type={1}", (object) productData, (object) product);
          return ItemOwnershipStatus.UNDEFINED;
        }
        return !AccountLicenseMgr.Get().OwnsAccountLicense(record2.LicenseId) ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BOARD_SKIN:
        BattlegroundsBoardSkinId skinId = BattlegroundsBoardSkinId.FromTrustedValue(productData);
        if (!CollectionManager.Get().IsValidBattlegroundsBoardSkinId(skinId))
          return ItemOwnershipStatus.UNDEFINED;
        return !CollectionManager.Get().OwnsBattlegroundsBoardSkin(skinId) ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_FINISHER:
        BattlegroundsFinisherId finisherId = BattlegroundsFinisherId.FromTrustedValue(productData);
        if (!CollectionManager.Get().IsValidBattlegroundsFinisherId(finisherId))
          return ItemOwnershipStatus.UNDEFINED;
        return !CollectionManager.Get().OwnsBattlegroundsFinisher(finisherId) ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      case ProductType.PRODUCT_TYPE_DIAMOND_CARD:
        if (NetCache.Get().GetNetObject<NetCache.NetCacheCollection>() == null)
        {
          failReason = string.Format("Collection not received to determine ownership of diamond card ID={0}", (object) productData);
          return ItemOwnershipStatus.UNDEFINED;
        }
        string cardId2 = GameUtils.TranslateDbIdToCardId(productData);
        return (cardId2 == null ? 0 : (CollectionManager.Get().IsCardInCollection(cardId2, TAG_PREMIUM.DIAMOND) ? 1 : 0)) == 0 ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_EMOTE:
        BattlegroundsEmoteId emoteId = BattlegroundsEmoteId.FromTrustedValue(productData);
        if (!CollectionManager.Get().IsValidBattlegroundsEmoteId(emoteId))
          return ItemOwnershipStatus.UNDEFINED;
        return !CollectionManager.Get().OwnsBattlegroundsEmote(emoteId) ? ItemOwnershipStatus.UNOWNED : ItemOwnershipStatus.OWNED;
      default:
        failReason = string.Format("Ownership status cannot be determined from license type {0}", (object) product);
        return ItemOwnershipStatus.UNDEFINED;
    }
  }

  private string GetSingleItemProductName(Network.BundleItem item)
  {
    string singleItemProductName = string.Empty;
    switch (item.ItemType)
    {
      case ProductType.PRODUCT_TYPE_BOOSTER:
        string name = (string) GameDbf.Booster.GetRecord(item.ProductData).Name;
        singleItemProductName = GameStrings.Format("GLUE_STORE_PRODUCT_NAME_PACK", (object) item.Quantity, (object) name);
        break;
      case ProductType.PRODUCT_TYPE_DRAFT:
        singleItemProductName = GameStrings.Get("GLUE_STORE_PRODUCT_NAME_FORGE_TICKET");
        break;
      case ProductType.PRODUCT_TYPE_NAXX:
      case ProductType.PRODUCT_TYPE_BRM:
      case ProductType.PRODUCT_TYPE_LOE:
      case ProductType.PRODUCT_TYPE_WING:
        singleItemProductName = AdventureProgressMgr.GetWingName(item.ProductData);
        break;
      case ProductType.PRODUCT_TYPE_CARD_BACK:
        CardBackDbfRecord record1 = GameDbf.CardBack.GetRecord(item.ProductData);
        if (record1 != null)
        {
          singleItemProductName = (string) record1.Name;
          break;
        }
        break;
      case ProductType.PRODUCT_TYPE_HERO:
        EntityDef entityDef = DefLoader.Get().GetEntityDef(item.ProductData);
        if (entityDef != null)
        {
          singleItemProductName = entityDef.GetName();
          break;
        }
        break;
      case ProductType.PRODUCT_TYPE_TAVERN_BRAWL_TICKET:
        TavernBrawlTicketDbfRecord record2 = GameDbf.TavernBrawlTicket.GetRecord(item.ProductData);
        if (record2 != null)
        {
          singleItemProductName = (string) record2.StoreName;
          break;
        }
        break;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BONUS:
        singleItemProductName = GameStrings.Get("GLUE_STORE_PRODUCT_NAME_BATTLEGROUNDS_BONUS");
        break;
      case ProductType.PRODUCT_TYPE_PROGRESSION_BONUS:
        singleItemProductName = GameStrings.Get("GLUE_STORE_PRODUCT_NAME_PROGRESSION_BONUS");
        break;
      case ProductType.PRODUCT_TYPE_LUCKY_DRAW:
        singleItemProductName = "Battle Bash";
        break;
      default:
        Log.Store.PrintWarning(string.Format("StoreManager.GetSingleItemProductName(): don't know how to format name for bundle product {0}", (object) item.ItemType));
        break;
    }
    return singleItemProductName;
  }

  private string GetMultiItemProductName(Network.Bundle bundle)
  {
    HashSet<ProductType> productsInItemList = this.GetProductsInItemList(bundle.Items);
    if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_NAXX))
      return GameStrings.Format("GLUE_STORE_PRODUCT_NAME_NAXX_WING_BUNDLE", (object) bundle.Items.Count);
    if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_BRM))
    {
      if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_CARD_BACK))
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_BRM_PRESALE_BUNDLE");
      return GameStrings.Format("GLUE_STORE_PRODUCT_NAME_BRM_WING_BUNDLE", (object) bundle.Items.Count);
    }
    if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_LOE))
      return GameStrings.Format("GLUE_STORE_PRODUCT_NAME_LOE_WING_BUNDLE", (object) bundle.Items.Count);
    if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_WING))
    {
      int wingID = bundle.Items.Where<Network.BundleItem>((Func<Network.BundleItem, bool>) (r => r.ItemType == ProductType.PRODUCT_TYPE_WING)).Select<Network.BundleItem, int>((Func<Network.BundleItem, int>) (r => r.ProductData)).FirstOrDefault<int>();
      if (wingID == 0)
        Log.Store.PrintError("StoreManager.GetMultiItemProductName: bundle with PRODUCT_TYPE_WING did not contain a valid wing ID in any of its product data.");
      string productStringKey = GameUtils.GetAdventureProductStringKey(wingID);
      if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_CARD_BACK))
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_" + productStringKey + "_PRESALE_BUNDLE");
      int num = bundle.Items.Count<Network.BundleItem>((Func<Network.BundleItem, bool>) (x => x.ItemType == ProductType.PRODUCT_TYPE_WING));
      return GameStrings.Format("GLUE_STORE_PRODUCT_NAME_" + productStringKey + "_WING_BUNDLE", (object) num);
    }
    if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_HIDDEN_LICENSE))
    {
      Network.BundleItem bundleItem = bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_HIDDEN_LICENSE));
      if (bundleItem.ProductData == 40)
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_FIRST_PURCHASE_BUNDLE");
      if (bundleItem.ProductData == 27)
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_MAMMOTH_BUNDLE");
      ModularBundleLayoutDbfRecord forHiddenLicense = this.GetRegionNodeLayoutForHiddenLicense(bundleItem.ProductData);
      if (forHiddenLicense != null)
        return (string) forHiddenLicense.OrderSummaryName;
    }
    if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_HERO))
    {
      Network.BundleItem bundleItem = bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_HERO));
      if ((Record) bundleItem != (Record) null)
        return this.GetSingleItemProductName(bundleItem);
    }
    else if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_BOOSTER) && productsInItemList.Contains(ProductType.PRODUCT_TYPE_CARD_BACK))
    {
      if ((Record) bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER && obj.ProductData == 10)) != (Record) null)
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_TGT_PRESALE_BUNDLE");
      if ((Record) bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER && obj.ProductData == 11)) != (Record) null)
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_OG_PRESALE_BUNDLE");
      if ((Record) bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER && obj.ProductData == 20)) != (Record) null)
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_GORO_PRESALE_BUNDLE");
      if ((Record) bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER && obj.ProductData == 21)) != (Record) null)
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_ICC_PRESALE_BUNDLE");
      if ((Record) bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER && obj.ProductData == 30)) != (Record) null)
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_LOOT_PRESALE_BUNDLE");
      if ((Record) bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER && obj.ProductData == 31)) != (Record) null)
        return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_GIL_PRESALE_BUNDLE");
    }
    else if (productsInItemList.Contains(ProductType.PRODUCT_TYPE_BOOSTER) && productsInItemList.Contains(ProductType.PRODUCT_TYPE_CURRENCY))
    {
      Network.BundleItem bundleItem1 = bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER));
      Network.BundleItem bundleItem2 = bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_CURRENCY && obj.ProductData == 2));
      if ((Record) bundleItem1 != (Record) null && (Record) bundleItem2 != (Record) null)
      {
        string name = (string) GameDbf.Booster.GetRecord(bundleItem1.ProductData).Name;
        return GameStrings.Format("GLUE_STORE_PRODUCT_NAME_DUST", (object) bundleItem2.Quantity, (object) bundleItem1.Quantity, (object) name);
      }
    }
    string empty = string.Empty;
    foreach (Network.BundleItem bundleItem in bundle.Items)
      empty += string.Format("[Product={0},ProductData={1},Quantity={2}],", (object) bundleItem.ItemType, (object) bundleItem.ProductData, (object) bundleItem.Quantity);
    Log.Store.PrintWarning("StoreManager.GetMultiItemProductName(): don't know how to format product name for items '" + empty + "'");
    return string.Empty;
  }

  private bool GetBoosterGoldCostNoGTAPP(int boosterID, out long cost)
  {
    cost = 0L;
    if (!this.m_goldCostBooster.ContainsKey(boosterID) || !this.CanBuyBoosterWithGold(boosterID))
      return false;
    Network.GoldCostBooster goldCostBooster = this.m_goldCostBooster[boosterID];
    long? cost1 = goldCostBooster.Cost;
    if (!cost1.HasValue)
      return false;
    cost1 = goldCostBooster.Cost;
    if (cost1.Value <= 0L)
      return false;
    ref long local = ref cost;
    cost1 = goldCostBooster.Cost;
    long num = cost1.Value;
    local = num;
    return true;
  }

  private bool GetArenaGoldCostNoGTAPP(out long cost)
  {
    cost = 0L;
    if (!this.m_goldCostArena.HasValue)
      return false;
    cost = this.m_goldCostArena.Value;
    return true;
  }

  private bool AutoCancelPurchaseIfNeeded(float now) => (double) now - (double) this.m_lastCancelRequestTime >= (double) this.m_secsBeforeAutoCancel && this.AutoCancelPurchaseIfPossible();

  private bool AutoCancelPurchaseIfPossible()
  {
    MoneyOrGTAPPTransaction gtappTransaction = this.m_activeMoneyOrGTAPPTransaction;
    if ((gtappTransaction != null ? (!gtappTransaction.Provider.HasValue ? 1 : 0) : 1) != 0 || BattlePayProvider.BP_PROVIDER_BLIZZARD != this.m_activeMoneyOrGTAPPTransaction.Provider.Value)
      return false;
    if (!this.IsSimpleCheckoutFeatureEnabled() || this.m_activeMoneyOrGTAPPTransaction.IsGTAPP)
    {
      switch (this.Status)
      {
        case StoreManager.TransactionStatus.IN_PROGRESS_MONEY:
        case StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP:
        case StoreManager.TransactionStatus.WAIT_METHOD_OF_PAYMENT:
        case StoreManager.TransactionStatus.WAIT_CONFIRM:
        case StoreManager.TransactionStatus.WAIT_RISK:
        case StoreManager.TransactionStatus.CHALLENGE_SUBMITTED:
        case StoreManager.TransactionStatus.CHALLENGE_CANCELED:
          Log.Store.Print("StoreManager.AutoCancelPurchaseIfPossible() canceling Blizzard purchase, status={0}", (object) this.Status);
          this.Status = StoreManager.TransactionStatus.AUTO_CANCELING;
          this.m_lastCancelRequestTime = Time.realtimeSinceStartup;
          Network.Get().CancelBlizzardPurchase(true, new CancelPurchase.CancelReason?(), (string) null);
          return true;
      }
    }
    else if (this.Status != StoreManager.TransactionStatus.IN_PROGRESS_BLIZZARD_CHECKOUT)
    {
      HearthstoneCheckout service;
      if (ServiceManager.TryGet<HearthstoneCheckout>(out service))
        service.RequestClose();
      this.Status = StoreManager.TransactionStatus.READY;
      this.m_lastCancelRequestTime = Time.realtimeSinceStartup;
      this.m_activeMoneyOrGTAPPTransaction = (MoneyOrGTAPPTransaction) null;
      return true;
    }
    return false;
  }

  private void CancelBlizzardPurchase(CancelPurchase.CancelReason? reason = null, string errorMessage = null)
  {
    Log.Store.Print("StoreManager.CancelBlizzardPurchase() reason=", reason.HasValue ? (object) reason.Value.ToString() : (object) "null");
    this.Status = StoreManager.TransactionStatus.USER_CANCELING;
    this.m_lastCancelRequestTime = Time.realtimeSinceStartup;
    Network.Get().CancelBlizzardPurchase(false, reason, errorMessage);
  }

  private bool HaveProductsToSell() => this.m_bundles.Count > 0 || this.m_goldCostBooster.Count > 0 || this.m_goldCostArena.HasValue;

  private bool AreVirtualCurrencyBalancesReady()
  {
    bool flag = true;
    foreach (CurrencyCache currencyCache in this.m_currencyCaches.Values)
    {
      if (!currencyCache.IsBalanceAvailable())
      {
        flag = false;
        break;
      }
    }
    return flag;
  }

  public bool IsBundleAvailableNow(Network.Bundle bundle)
  {
    if ((Record) bundle == (Record) null)
      return false;
    ProductAvailabilityRange availabilityRange = this.GetBundleAvailabilityRange(bundle);
    return availabilityRange != null && availabilityRange.IsBuyableAtTime(DateTime.UtcNow);
  }

  public ProductAvailabilityRange GetBundleAvailabilityRange(
    Network.Bundle bundle)
  {
    if (this.m_ignoreProductTiming)
      return new ProductAvailabilityRange();
    ProductAvailabilityRange other = (ProductAvailabilityRange) null;
    if (!string.IsNullOrEmpty(bundle.ProductEvent))
    {
      SpecialEventManager specialEventManager = SpecialEventManager.Get();
      SpecialEventType eventType = specialEventManager.GetEventType(bundle.ProductEvent);
      switch (eventType)
      {
        case SpecialEventType.UNKNOWN:
          return (ProductAvailabilityRange) null;
        case SpecialEventType.IGNORE:
          break;
        case SpecialEventType.SPECIAL_EVENT_NEVER:
          return new ProductAvailabilityRange(bundle.ProductEvent, new DateTime?(), new DateTime?())
          {
            IsNever = true
          };
        default:
          DateTime? start;
          DateTime? end;
          if (!specialEventManager.GetEventRangeUtc(eventType, out start, out end))
            return (ProductAvailabilityRange) null;
          other = new ProductAvailabilityRange(bundle.ProductEvent, start, end);
          if (other.IsNever)
            return other;
          break;
      }
    }
    ProductAvailabilityRange a = (ProductAvailabilityRange) null;
    if (!bundle.VisibleOnSalePeriodOnly)
    {
      a = new ProductAvailabilityRange();
    }
    else
    {
      DateTime utcNow = DateTime.UtcNow;
      foreach (int saleId in bundle.SaleIds)
      {
        Network.ShopSale shopSale;
        this.m_sales.TryGetValue(saleId, out shopSale);
        if (!((Record) shopSale == (Record) null))
        {
          ProductAvailabilityRange availabilityRange = new ProductAvailabilityRange(shopSale);
          if (a == null)
            a = availabilityRange;
          else if (ProductAvailabilityRange.AreOverlapping(a, availabilityRange))
          {
            a.UnionWith(availabilityRange);
          }
          else
          {
            TimeSpan displacement1;
            if (!a.TryGetTimeDisplacementRequiredToBeBuyable(utcNow, out displacement1))
            {
              a = availabilityRange;
            }
            else
            {
              TimeSpan displacement2;
              if (availabilityRange.TryGetTimeDisplacementRequiredToBeBuyable(utcNow, out displacement2) && Math.Abs(displacement2.Ticks) <= Math.Abs(displacement1.Ticks))
                a = availabilityRange;
            }
          }
        }
      }
    }
    if (other != null)
    {
      if (a == null)
        a = other;
      else
        a.IntersectWith(other);
    }
    return a;
  }

  private bool DoesBundleContainDust(Network.Bundle bundle) => (Record) bundle?.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_CURRENCY)) != (Record) null;

  public bool ShouldShowFeaturedDustJar(Network.Bundle bundle) => this.m_regionId == BnetRegion.REGION_CN && this.m_currentlySelectedId.Type == StorePackType.BOOSTER && this.DoesBundleContainDust(bundle);

  public int DustQuantityInBundle(Network.Bundle bundle)
  {
    if ((Record) bundle == (Record) null)
      return 0;
    Network.BundleItem bundleItem = bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_CURRENCY));
    return bundleItem == null ? 0 : bundleItem.Quantity;
  }

  public int DustBaseQuantityInBundle(Network.Bundle bundle)
  {
    if ((Record) bundle == (Record) null)
      return 0;
    Network.BundleItem bundleItem = bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_CURRENCY));
    return bundleItem == null ? 0 : bundleItem.BaseQuantity;
  }

  public int PackQuantityInBundle(Network.Bundle bundle)
  {
    if ((Record) bundle == (Record) null)
      return 0;
    Network.BundleItem bundleItem = bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER));
    return bundleItem == null ? 0 : bundleItem.Quantity;
  }

  private void OnStoreOpen()
  {
    if ((UnityEngine.Object) BnetBar.Get() != (UnityEngine.Object) null)
      BnetBar.Get().RefreshCurrency();
    System.Action onStoreShown = this.OnStoreShown;
    if (onStoreShown == null)
      return;
    onStoreShown();
  }

  private void OnStoreExit(bool authorizationBackButtonPressed, object userData)
  {
    Store.ExitCallback exitCallback = this.m_showStoreData.exitCallback;
    if (exitCallback != null)
      exitCallback(authorizationBackButtonPressed, userData);
    if (this.m_activeMoneyOrGTAPPTransaction != null)
      this.m_activeMoneyOrGTAPPTransaction.ClosedStore = true;
    if (this.m_view.ChallengePrompt.IsLoaded && !this.m_view.ChallengePrompt.Cancel(new System.Action<string>(this.OnChallengeCancel)))
      this.AutoCancelPurchaseIfPossible();
    this.UnblockStoreInterface();
    this.m_view.Hide();
    this.OnStoreHidden();
    if ((UnityEngine.Object) BnetBar.Get() != (UnityEngine.Object) null)
      BnetBar.Get().RefreshCurrency();
    HearthstonePerformance.Get()?.StopCurrentFlow();
  }

  private void OnStoreInfo(object userData) => this.ShowStoreInfo();

  public void ShowStoreInfo()
  {
    this.BlockStoreInterface();
    this.m_view.SendToBam.Show((MoneyOrGTAPPTransaction) null, StoreSendToBAM.BAMReason.PAYMENT_INFO, "", false);
  }

  public bool CanBuyBundle(Network.Bundle bundleToBuy)
  {
    if ((Record) bundleToBuy == (Record) null)
    {
      Log.Store.PrintWarning("Null bundle passed to CanBuyBundle!");
      return false;
    }
    if (AchieveManager.Get() == null || !AchieveManager.Get().IsReady())
      return false;
    if (bundleToBuy.Items.Count < 1)
    {
      Log.Store.PrintWarning(string.Format("Attempting to buy bundle {0}, which does not contain any items!", (object) bundleToBuy.PMTProductID));
      return false;
    }
    if (!this.IsBundleAvailableNow(bundleToBuy))
      return false;
    foreach (Network.BundleItem bundleItem in bundleToBuy.Items)
    {
      if (!this.CanBuyProductItem(bundleItem.ItemType, bundleItem.ProductData, StoreManager.GetProductItemPurchaseRule(bundleItem)))
        return false;
    }
    return true;
  }

  private bool CanBuyProductItem(
    ProductType product,
    int productData,
    ItemPurchaseRule purchaseRule)
  {
    if (AchieveManager.Get() == null || !AchieveManager.Get().IsReady())
      return false;
    switch (purchaseRule)
    {
      case ItemPurchaseRule.BLOCKING:
        return StoreManager.GetProductItemOwnershipStatus(product, productData, out string _) == ItemOwnershipStatus.UNOWNED;
      case ItemPurchaseRule.NO_LIMIT:
        return true;
      default:
        return false;
    }
  }

  private void OnStoreBuyWithMoney(BuyPmtProductEventArgs args)
  {
    if (TemporaryAccountManager.IsTemporaryAccount() && !this.IsSoftAccountPurchasingEnabled())
    {
      TemporaryAccountManager.Get().ShowHealUpDialog(GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_HEADER_01"), GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_BODY_02"), TemporaryAccountManager.HealUpReason.REAL_MONEY, true, (TemporaryAccountManager.OnHealUpDialogDismissed) null);
    }
    else
    {
      Network.Bundle fromPmtProductId = this.GetBundleFromPmtProductId(new long?(args.pmtProductId));
      if ((Record) fromPmtProductId == (Record) null)
        Log.Store.PrintError("OnStoreBuyWithMoney failed: bundle not found for pmtProductID = {0}.", (object) args.pmtProductId);
      else if (!this.CanBuyBundle(fromPmtProductId))
      {
        Log.Store.PrintError("OnStoreBuyWithMoney failed: CanBuyBundle is false for pmtProductID = {0}.", (object) args.pmtProductId);
      }
      else
      {
        if (!this.IsSimpleCheckoutFeatureEnabled())
          return;
        this.OnStoreBuyWithCheckout(args);
      }
    }
  }

  private void OnStoreBuyWithGTAPP(BuyPmtProductEventArgs args)
  {
    if (!this.CanBuyBundle(this.GetBundleFromPmtProductId(new long?(args.pmtProductId))))
    {
      Log.Store.PrintError("Purchase with GTAPP failed (PMT product ID = {0}): CanBuyProductItem is false.", (object) args.pmtProductId);
    }
    else
    {
      this.SetCanTapOutConfirmationUI(true);
      this.BlockStoreInterface();
      this.SetActiveMoneyOrGTAPPTransaction((long) StoreManager.UNKNOWN_TRANSACTION_ID, new long?(args.pmtProductId), new BattlePayProvider?(BattlePayProvider.BP_PROVIDER_BLIZZARD), true, false);
      this.Status = StoreManager.TransactionStatus.WAIT_METHOD_OF_PAYMENT;
      this.m_lastCancelRequestTime = Time.realtimeSinceStartup;
      this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false);
      Network.Get().GetPurchaseMethod(new long?(args.pmtProductId), args.quantity, Currency.GTAPP);
    }
  }

  private void OnStoreBuyWithGoldNoGTAPP(NoGTAPPTransactionData noGTAPPtransactionData)
  {
    if (noGTAPPtransactionData == null)
      Log.Store.PrintError("Purchase failed: null transaction data.");
    else if (!this.CanBuyProductItem(noGTAPPtransactionData.Product, noGTAPPtransactionData.ProductData, StoreManager.InferProductItemPurchaseRuleFromProductType(noGTAPPtransactionData.Product)))
    {
      Log.Store.PrintError("Purchase direct with gold (no GTAPP) failed: CanBuyProductItem is false.");
    }
    else
    {
      this.BlockStoreInterface();
      this.m_view.PurchaseAuth.Show((MoneyOrGTAPPTransaction) null, false, this.GetPurchaseAuthButtonStyle(this.m_currentShopType));
      this.Status = StoreManager.TransactionStatus.IN_PROGRESS_GOLD_NO_GTAPP;
      Network.Get().PurchaseViaGold(noGTAPPtransactionData.Quantity, noGTAPPtransactionData.Product, noGTAPPtransactionData.ProductData);
    }
  }

  private void OnStoreBuyWithCheckout(BuyPmtProductEventArgs args)
  {
    ProductId from = ProductId.CreateFrom(args.pmtProductId);
    if ((Record) this.GetBundleFromPmtProductId(from) == (Record) null)
      Log.Store.PrintError("Cannot buy product PMT ID = {0}. Bundle not found.", (object) args.pmtProductId);
    else if (!this.IsSimpleCheckoutFeatureEnabled())
    {
      Log.Store.PrintError("Purchase failed: Checkout feature is disabled.");
    }
    else
    {
      HearthstoneCheckout service;
      if (!ServiceManager.TryGet<HearthstoneCheckout>(out service))
        Log.Store.PrintError("Purchase failed: Commerce service is not available.");
      else if (args.paymentCurrency == CurrencyType.REAL_MONEY)
      {
        if (TemporaryAccountManager.IsTemporaryAccount() && !this.IsSoftAccountPurchasingEnabled())
        {
          TemporaryAccountManager.Get().ShowHealUpDialog(GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_HEADER_01"), GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_BODY_02"), TemporaryAccountManager.HealUpReason.REAL_MONEY, true, (TemporaryAccountManager.OnHealUpDialogDismissed) null);
        }
        else
        {
          this.Status = StoreManager.TransactionStatus.WAIT_BLIZZARD_CHECKOUT;
          this.SetActiveMoneyOrGTAPPTransaction((long) StoreManager.UNKNOWN_TRANSACTION_ID, new long?(from.Value), new BattlePayProvider?(BattlePayProvider.BP_PROVIDER_BLIZZARD), false, false);
          this.m_lastCancelRequestTime = Time.realtimeSinceStartup;
          this.SetCanTapOutConfirmationUI(true);
          this.BlockStoreInterface();
          service.ShowCheckout(from, ShopUtils.GetCurrencyCode(args.paymentCurrency), (uint) args.quantity);
          if (!StoreManager.HasExternalStore && (!HearthstoneApplication.IsCNMobileBinary || Application.platform != RuntimePlatform.Android))
            return;
          this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false, StorePurchaseAuth.ButtonStyle.Cancel);
        }
      }
      else if (ShopUtils.IsCurrencyVirtual(args.paymentCurrency))
      {
        this.Status = StoreManager.TransactionStatus.WAIT_BLIZZARD_CHECKOUT;
        this.SetActiveMoneyOrGTAPPTransaction((long) StoreManager.UNKNOWN_TRANSACTION_ID, new long?(args.pmtProductId), new BattlePayProvider?(BattlePayProvider.BP_PROVIDER_BLIZZARD), false, false);
        this.m_lastCancelRequestTime = Time.realtimeSinceStartup;
        this.SetCanTapOutConfirmationUI(true);
        this.BlockStoreInterface();
        if (this.m_view.PurchaseAuth.IsShown)
          this.m_view.PurchaseAuth.StartNewTransaction(this.m_activeMoneyOrGTAPPTransaction, false);
        else
          this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false);
        service.PurchaseWithVirtualCurrency(from, ShopUtils.GetCurrencyCode(args.paymentCurrency), (uint) args.quantity);
      }
      else
        Log.Store.PrintError("Buy with checkout failed: Invalid currency type {0}", (object) args.paymentCurrency);
    }
  }

  private void OnSummaryConfirm(int quantity, object userData)
  {
    this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false, this.GetPurchaseAuthButtonStyle(this.m_currentShopType));
    if (this.m_challengePurchaseMethod != null)
      this.m_view.ChallengePrompt.StartChallenge(this.m_challengePurchaseMethod.ChallengeURL);
    else
      this.ConfirmPurchase();
  }

  private void ConfirmPurchase()
  {
    this.Status = this.m_activeMoneyOrGTAPPTransaction.IsGTAPP ? StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP : StoreManager.TransactionStatus.IN_PROGRESS_MONEY;
    Network.Get().ConfirmPurchase();
  }

  private void OnSummaryCancel(object userData)
  {
    this.CancelBlizzardPurchase();
    this.UnblockStoreInterface();
  }

  private void OnSummaryInfo(object userData)
  {
    this.BlockStoreInterface();
    this.AutoCancelPurchaseIfPossible();
    this.m_view.SendToBam.Show((MoneyOrGTAPPTransaction) null, StoreSendToBAM.BAMReason.EULA_AND_TOS, string.Empty, false);
  }

  private void OnSummaryPaymentAndTOS(object userData)
  {
    this.AutoCancelPurchaseIfPossible();
    this.m_view.LegalBam.Show();
  }

  private void OnChallengeComplete(
    string challengeID,
    bool isSuccess,
    CancelPurchase.CancelReason? reason,
    string internalErrorInfo)
  {
    if (!isSuccess)
    {
      this.OnChallengeCancel_Internal(challengeID, reason, internalErrorInfo);
    }
    else
    {
      this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false, this.GetPurchaseAuthButtonStyle(this.m_currentShopType));
      this.Status = StoreManager.TransactionStatus.CHALLENGE_SUBMITTED;
      this.ConfirmPurchase();
    }
  }

  private void OnChallengeCancel(string challengeID) => this.OnChallengeCancel_Internal(challengeID, new CancelPurchase.CancelReason?(), (string) null);

  private void OnChallengeCancel_Internal(
    string challengeID,
    CancelPurchase.CancelReason? reason,
    string errorMessage)
  {
    Debug.LogFormat("Canceling purchase from challengeId={0} reason={1} msg={2}", (object) challengeID, reason.HasValue ? (object) reason.Value.ToString() : (object) "null", (object) errorMessage);
    this.Status = StoreManager.TransactionStatus.CHALLENGE_CANCELED;
    this.CancelBlizzardPurchase(reason, errorMessage);
    this.UnblockStoreInterface();
    this.m_view.Hide();
  }

  private void OnSendToBAMOkay(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    StoreSendToBAM.BAMReason reason)
  {
    if (moneyOrGTAPPTransaction != null)
      this.ConfirmActiveMoneyTransaction(moneyOrGTAPPTransaction.ID);
    if (reason == StoreSendToBAM.BAMReason.PAYMENT_INFO)
      this.UnblockStoreInterface();
    else
      this.m_view.DoneWithBam.Show();
  }

  private void OnSendToBAMCancel(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction)
  {
    if (moneyOrGTAPPTransaction != null)
      this.ConfirmActiveMoneyTransaction(moneyOrGTAPPTransaction.ID);
    this.UnblockStoreInterface();
  }

  private void OnSendToBAMLegal(StoreLegalBAMLinks.BAMReason reason) => this.UnblockStoreInterface();

  private void OnAchievesUpdated(
    List<Achievement> updatedAchives,
    List<Achievement> completedAchives,
    object userData)
  {
    this.m_completedAchieves = AchieveManager.Get().GetNewCompletedAchievesToShow();
    this.ShowCompletedAchieve();
  }

  private void OnLicenseAddedAchievesUpdated(
    List<Achievement> activeLicenseAddedAchieves,
    object userData)
  {
    if (StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE != this.Status || activeLicenseAddedAchieves.Count > 0)
      return;
    Log.Store.Print("StoreManager.OnLicenseAddedAchievesUpdated(): done waiting for licenses!");
    if (this.IsCurrentStoreLoaded())
    {
      this.RemovePurchaseAuthCancelButton();
      Processor.QueueJob("StoreManager.ShowCompletePurchaseSuccessWhenReady", this.Job_ShowCompletePurchaseSuccessWhenReady((MoneyOrGTAPPTransaction) null));
    }
    this.Status = StoreManager.TransactionStatus.READY;
  }

  private void ShowCompletedAchieve()
  {
    bool enabled = this.m_completedAchieves.Count == 0;
    if (this.m_currentShopType == ShopType.GENERAL_STORE)
    {
      GeneralStore currentStore = (GeneralStore) this.GetCurrentStore();
      if ((UnityEngine.Object) currentStore != (UnityEngine.Object) null)
        currentStore.EnableClickCatcher(enabled);
    }
    if (enabled)
      return;
    Achievement completedAchieve = this.m_completedAchieves[0];
    this.m_completedAchieves.RemoveAt(0);
    QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, (QuestToast.DelOnCloseQuestToast) (userData => this.ShowCompletedAchieve()), true, completedAchieve, false);
  }

  private void OnPurchaseResultAcknowledged(
    bool success,
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction)
  {
    Network.Bundle bundle = (Network.Bundle) null;
    PaymentMethod paymentMethod;
    if (moneyOrGTAPPTransaction == null)
    {
      paymentMethod = PaymentMethod.GOLD_NO_GTAPP;
    }
    else
    {
      if (moneyOrGTAPPTransaction.ID > 0L)
        this.m_transactionIDsConclusivelyHandled.Add(moneyOrGTAPPTransaction.ID);
      paymentMethod = moneyOrGTAPPTransaction.IsGTAPP ? PaymentMethod.GOLD_GTAPP : PaymentMethod.MONEY;
      bundle = this.GetBundleFromPmtProductId(new long?(moneyOrGTAPPTransaction.PMTProductID.GetValueOrDefault()));
    }
    if (PaymentMethod.GOLD_NO_GTAPP != paymentMethod)
      this.ConfirmActiveMoneyTransaction(moneyOrGTAPPTransaction.ID);
    if (success)
      this.OnSuccessfulPurchaseAck(bundle, paymentMethod);
    else
      this.OnFailedPurchaseAck(bundle, paymentMethod);
    this.SetCanTapOutConfirmationUI(true);
    this.UnblockStoreInterface();
    IStore currentStore = this.GetCurrentStore();
    if (this.m_currentShopType == ShopType.ADVENTURE_STORE || this.m_currentShopType == ShopType.ADVENTURE_STORE_WING_PURCHASE_WIDGET || this.m_currentShopType == ShopType.ADVENTURE_STORE_FULL_PURCHASE_WIDGET)
      currentStore.Close();
    if (this.BattlePayAvailable || this.m_currentShopType != ShopType.GENERAL_STORE)
      return;
    currentStore.Close();
  }

  private void OnAuthExit() => this.OnAuthorizationExit();

  private void OnPurchaseAuthCancelButtonPressed()
  {
    this.UnblockStoreInterface();
    HearthstoneCheckout service;
    if (!ServiceManager.TryGet<HearthstoneCheckout>(out service))
      return;
    service.CancelCurrentTransaction();
  }

  private void RemovePurchaseAuthCancelButton()
  {
    if (!this.m_view.IsLoaded())
      return;
    this.m_view.PurchaseAuth.HideCancelButton();
  }

  private void BlockStoreInterface() => this.GetCurrentStore()?.BlockInterface(true);

  private void UnblockStoreInterface() => this.GetCurrentStore()?.BlockInterface(false);

  private void HandlePurchaseSuccess(
    StoreManager.PurchaseErrorSource? source,
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    string thirdPartyID,
    TransactionData checkoutTransactionData)
  {
    this.Status = StoreManager.TransactionStatus.READY;
    this.SendShopPurchaseEventTelemetry(true);
    this.m_pendingProductPurchaseArgs = (BuyProductEventArgs) null;
    Network.Bundle bundle = (Network.Bundle) null;
    PaymentMethod paymentMethod;
    if (moneyOrGTAPPTransaction == null)
    {
      paymentMethod = PaymentMethod.GOLD_NO_GTAPP;
    }
    else
    {
      paymentMethod = checkoutTransactionData == null || !ShopUtils.IsCurrencyVirtual(ShopUtils.GetCurrencyTypeFromCode(checkoutTransactionData.CurrencyCode)) ? (moneyOrGTAPPTransaction.IsGTAPP ? PaymentMethod.GOLD_GTAPP : PaymentMethod.MONEY) : PaymentMethod.VIRTUAL_CURRENCY;
      bundle = this.GetBundleFromPmtProductId(ProductId.CreateFrom(moneyOrGTAPPTransaction.PMTProductID.GetValueOrDefault()));
    }
    this.OnSuccessfulPurchase(bundle, paymentMethod);
    if (!this.IsCurrentStoreLoaded())
      return;
    StoreManager.PurchaseErrorSource? nullable = source;
    StoreManager.PurchaseErrorSource purchaseErrorSource = StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE;
    if (nullable.GetValueOrDefault() == purchaseErrorSource & nullable.HasValue)
    {
      this.BlockStoreInterface();
      this.m_view.PurchaseAuth.ShowPreviousPurchaseSuccess(moneyOrGTAPPTransaction, this.GetPurchaseAuthButtonStyle(this.m_currentShopType));
    }
    else
    {
      this.MarkTransactionCurrenciesAsDirty(paymentMethod, bundle);
      this.RemovePurchaseAuthCancelButton();
      Processor.QueueJob("StoreManager.ShowCompletePurchaseSuccessWhenReady", this.Job_ShowCompletePurchaseSuccessWhenReady(moneyOrGTAPPTransaction));
    }
  }

  private void MarkTransactionCurrenciesAsDirty(PaymentMethod paymentMethod, Network.Bundle bundle)
  {
    switch (paymentMethod)
    {
      case PaymentMethod.GOLD_GTAPP:
      case PaymentMethod.GOLD_NO_GTAPP:
        this.GetCurrencyCache(CurrencyType.GOLD).MarkDirty();
        break;
      case PaymentMethod.VIRTUAL_CURRENCY:
        if ((Record) bundle != (Record) null)
        {
          CurrencyType currencyPriceType = ShopUtils.GetBundleVirtualCurrencyPriceType(bundle);
          if (currencyPriceType != CurrencyType.NONE)
          {
            this.GetCurrencyCache(currencyPriceType).MarkDirty();
            break;
          }
          break;
        }
        break;
    }
    if (!((Record) bundle != (Record) null))
      return;
    foreach (Network.BundleItem bundleItem in bundle.Items.Where<Network.BundleItem>((Func<Network.BundleItem, bool>) (i => i.ItemType == ProductType.PRODUCT_TYPE_CURRENCY)))
    {
      CurrencyType currencyTypeFromProto = ShopUtils.GetCurrencyTypeFromProto((PegasusShared.CurrencyType) bundleItem.ProductData);
      if (currencyTypeFromProto != CurrencyType.NONE)
        this.GetCurrencyCache(currencyTypeFromProto).MarkDirty();
    }
  }

  private IEnumerator<IAsyncJobResult> Job_ShowCompletePurchaseSuccessWhenReady(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction)
  {
    DateTime startTime = DateTime.Now;
    double elapsedSeconds = 0.0;
    bool checkCurrency = true;
    while (!this.IsPurchaseSuccessReady(checkCurrency))
    {
      elapsedSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
      if (checkCurrency && elapsedSeconds > StoreManager.CURRENCY_TRANSACTION_TIMEOUT_SECONDS)
        checkCurrency = false;
      yield return (IAsyncJobResult) null;
    }
    if (this.m_currencyCaches.Any<KeyValuePair<CurrencyType, CurrencyCache>>((Func<KeyValuePair<CurrencyType, CurrencyCache>, bool>) (c => c.Value.NeedsRefresh())))
    {
      Log.Store.PrintError("[StoreManager.ShowCompletePurchaseSuccessWhenReady] gave up on waiting for currency balance after {0} seconds", (object) elapsedSeconds);
      if ((UnityEngine.Object) DialogManager.Get() != (UnityEngine.Object) null)
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_text = GameStrings.Format("GLUE_STORE_FAIL_CURRENCY_BALANCE"),
          m_showAlertIcon = true,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK
        };
        DialogManager.Get().ShowPopup(info);
      }
    }
    this.SetCanTapOutConfirmationUI(false);
    if (this.m_view.IsLoaded())
      this.m_view.PurchaseAuth.CompletePurchaseSuccess(moneyOrGTAPPTransaction);
  }

  private bool IsPurchaseSuccessReady(bool checkCurrency = true)
  {
    if (this.Status != StoreManager.TransactionStatus.READY || !((UnityEngine.Object) Shop.Get() == (UnityEngine.Object) null) && Shop.Get().WillAutoPurchase())
      return false;
    return !checkCurrency || !this.m_currencyCaches.Any<KeyValuePair<CurrencyType, CurrencyCache>>((Func<KeyValuePair<CurrencyType, CurrencyCache>, bool>) (c => c.Value.NeedsRefresh()));
  }

  private void HandleFailedRiskError(StoreManager.PurchaseErrorSource source)
  {
    int num = StoreManager.TransactionStatus.CHALLENGE_CANCELED == this.Status ? 1 : 0;
    this.Status = StoreManager.TransactionStatus.READY;
    if (num != 0)
    {
      Log.Store.Print("HandleFailedRiskError for canceled transaction");
      if (this.m_activeMoneyOrGTAPPTransaction != null)
        this.ConfirmActiveMoneyTransaction(this.m_activeMoneyOrGTAPPTransaction.ID);
      this.UnblockStoreInterface();
    }
    else
    {
      if (!this.IsCurrentStoreLoaded() || !this.GetCurrentStore().IsOpen())
        return;
      this.m_view.PurchaseAuth.Hide();
      this.m_view.Summary.Hide();
      this.BlockStoreInterface();
      this.m_view.SendToBam.Show(this.m_activeMoneyOrGTAPPTransaction, StoreSendToBAM.BAMReason.NEED_PASSWORD_RESET, string.Empty, source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE);
    }
  }

  private void HandleSendToBAMError(
    StoreManager.PurchaseErrorSource source,
    StoreSendToBAM.BAMReason reason,
    string errorCode)
  {
    this.Status = StoreManager.TransactionStatus.READY;
    if (!this.IsCurrentStoreLoaded() || !this.GetCurrentStore().IsOpen())
      return;
    this.m_view.PurchaseAuth.Hide();
    this.m_view.Summary.Hide();
    this.BlockStoreInterface();
    this.m_view.SendToBam.Show(this.m_activeMoneyOrGTAPPTransaction, reason, errorCode, source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE);
  }

  private void CompletePurchaseFailure(
    StoreManager.PurchaseErrorSource source,
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    string failDetails,
    string thirdPartyID,
    Network.PurchaseErrorInfo.ErrorType error)
  {
    if (!this.IsCurrentStoreLoaded())
      return;
    switch (source)
    {
      case StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE:
        if (this.m_view.SendToBam.IsShown)
          break;
        this.BlockStoreInterface();
        this.m_view.PurchaseAuth.ShowPreviousPurchaseFailure(moneyOrGTAPPTransaction, failDetails, this.GetPurchaseAuthButtonStyle(this.m_currentShopType), error);
        break;
      case StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE:
        this.BlockStoreInterface();
        this.m_view.PurchaseAuth.ShowPreviousPurchaseFailure(moneyOrGTAPPTransaction, failDetails, this.GetPurchaseAuthButtonStyle(this.m_currentShopType), error);
        break;
      default:
        if (this.m_view.PurchaseAuth.CompletePurchaseFailure(moneyOrGTAPPTransaction, failDetails, error))
          break;
        Log.Store.PrintWarning("StoreManager.CompletePurchaseFailure(): purchased failed (" + failDetails + ") but the store authorization window has been closed.");
        this.UnblockStoreInterface();
        break;
    }
  }

  private void HandlePurchaseError(
    StoreManager.PurchaseErrorSource source,
    Network.PurchaseErrorInfo.ErrorType purchaseErrorType,
    string purchaseErrorCode,
    string thirdPartyID,
    bool isGTAPP)
  {
    if (this.IsConclusiveState(purchaseErrorType) && this.m_activeMoneyOrGTAPPTransaction != null && this.m_transactionIDsConclusivelyHandled.Contains(this.m_activeMoneyOrGTAPPTransaction.ID))
    {
      Log.Store.Print("HandlePurchaseError already handled purchase error for conclusive state on transaction (Transaction: {0}, current purchaseErrorType = {1})", (object) this.m_activeMoneyOrGTAPPTransaction, (object) purchaseErrorType);
    }
    else
    {
      Log.Store.Print(string.Format("HandlePurchaseError source={0} purchaseErrorType={1} purchaseErrorCode={2} thirdPartyID={3}", (object) source, (object) purchaseErrorType, (object) purchaseErrorCode, (object) thirdPartyID));
      string failDetails1 = "";
      switch (purchaseErrorType)
      {
        case Network.PurchaseErrorInfo.ErrorType.UNKNOWN:
          Log.Store.PrintWarning("StoreManager.HandlePurchaseError: purchase error is UNKNOWN, taking no action on this purchase");
          return;
        case Network.PurchaseErrorInfo.ErrorType.SUCCESS:
          if (source == StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE)
          {
            Log.Store.PrintWarning("StoreManager.HandlePurchaseError: received SUCCESS from payment method purchase error.");
            return;
          }
          this.HandlePurchaseSuccess(new StoreManager.PurchaseErrorSource?(source), this.m_activeMoneyOrGTAPPTransaction, thirdPartyID, (TransactionData) null);
          return;
        case Network.PurchaseErrorInfo.ErrorType.STILL_IN_PROGRESS:
          if (source == StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE)
          {
            Log.Store.PrintWarning("StoreManager.HandlePurchaseError: received STILL_IN_PROGRESS from payment method purchase error.");
            return;
          }
          if (source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
            return;
          this.Status = isGTAPP ? StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP : StoreManager.TransactionStatus.IN_PROGRESS_MONEY;
          return;
        case Network.PurchaseErrorInfo.ErrorType.INVALID_BNET:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_BNET_ID");
          break;
        case Network.PurchaseErrorInfo.ErrorType.SERVICE_NA:
          if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
          {
            if (this.Status != StoreManager.TransactionStatus.UNKNOWN)
              this.BattlePayAvailable = false;
            this.Status = StoreManager.TransactionStatus.UNKNOWN;
          }
          string failDetails2 = GameStrings.Get("GLUE_STORE_FAIL_NO_BATTLEPAY");
          this.CompletePurchaseFailure(source, this.m_activeMoneyOrGTAPPTransaction, failDetails2, thirdPartyID, purchaseErrorType);
          return;
        case Network.PurchaseErrorInfo.ErrorType.PURCHASE_IN_PROGRESS:
          if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
            this.Status = isGTAPP ? StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP : StoreManager.TransactionStatus.IN_PROGRESS_MONEY;
          string failDetails3 = GameStrings.Get("GLUE_STORE_FAIL_IN_PROGRESS");
          this.CompletePurchaseFailure(source, this.m_activeMoneyOrGTAPPTransaction, failDetails3, thirdPartyID, purchaseErrorType);
          return;
        case Network.PurchaseErrorInfo.ErrorType.DATABASE:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_DATABASE");
          break;
        case Network.PurchaseErrorInfo.ErrorType.INVALID_QUANTITY:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_QUANTITY");
          break;
        case Network.PurchaseErrorInfo.ErrorType.DUPLICATE_LICENSE:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_LICENSE");
          break;
        case Network.PurchaseErrorInfo.ErrorType.REQUEST_NOT_SENT:
          if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE && this.Status != StoreManager.TransactionStatus.UNKNOWN)
            this.BattlePayAvailable = false;
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_NO_BATTLEPAY");
          break;
        case Network.PurchaseErrorInfo.ErrorType.NO_ACTIVE_BPAY:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_NO_ACTIVE_BPAY");
          break;
        case Network.PurchaseErrorInfo.ErrorType.FAILED_RISK:
          this.HandleFailedRiskError(source);
          return;
        case Network.PurchaseErrorInfo.ErrorType.CANCELED:
          if (source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
            return;
          this.Status = StoreManager.TransactionStatus.READY;
          return;
        case Network.PurchaseErrorInfo.ErrorType.WAIT_MOP:
          Log.Store.Print("StoreManager.HandlePurchaseError: Status is WAIT_MOP.. this probably shouldn't be happening.");
          if (source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
            return;
          if (this.Status == StoreManager.TransactionStatus.UNKNOWN)
          {
            Log.Store.Print(string.Format("StoreManager.HandlePurchaseError: Status is WAIT_MOP, previous Status was UNKNOWN, source = {0}", (object) source));
            return;
          }
          this.Status = StoreManager.TransactionStatus.WAIT_METHOD_OF_PAYMENT;
          return;
        case Network.PurchaseErrorInfo.ErrorType.WAIT_CONFIRM:
          if (source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE || this.Status != StoreManager.TransactionStatus.UNKNOWN)
            return;
          Log.Store.Print(string.Format("StoreManager.HandlePurchaseError: Status is WAIT_CONFIRM, previous Status was UNKNOWN, source = {0}. Going to try to cancel the purchase.", (object) source));
          this.CancelBlizzardPurchase();
          return;
        case Network.PurchaseErrorInfo.ErrorType.WAIT_RISK:
          if (source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
            return;
          Log.Store.Print("StoreManager.HandlePurchaseError: Waiting for client to respond to Risk challenge");
          if (this.Status == StoreManager.TransactionStatus.UNKNOWN)
          {
            Log.Store.Print(string.Format("StoreManager.HandlePurchaseError: Status is WAIT_RISK, previous Status was UNKNOWN, source = {0}", (object) source));
            return;
          }
          if (StoreManager.TransactionStatus.CHALLENGE_SUBMITTED == this.Status || StoreManager.TransactionStatus.CHALLENGE_CANCELED == this.Status)
          {
            Log.Store.Print(string.Format("StoreManager.HandlePurchaseError: Status = {0}; ignoring WAIT_RISK purchase error info", (object) this.Status));
            return;
          }
          this.Status = StoreManager.TransactionStatus.WAIT_RISK;
          return;
        case Network.PurchaseErrorInfo.ErrorType.PRODUCT_NA:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_PRODUCT_NA");
          break;
        case Network.PurchaseErrorInfo.ErrorType.RISK_TIMEOUT:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_CHALLENGE_TIMEOUT");
          break;
        case Network.PurchaseErrorInfo.ErrorType.PRODUCT_ALREADY_OWNED:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_PRODUCT_ALREADY_OWNED");
          break;
        case Network.PurchaseErrorInfo.ErrorType.WAIT_THIRD_PARTY_RECEIPT:
          Log.Store.PrintWarning("StoreManager.HandlePurchaseError: Received WAIT_THIRD_PARTY_RECEIPT response, even though legacy third party purchasing is removed.");
          return;
        case Network.PurchaseErrorInfo.ErrorType.PRODUCT_EVENT_HAS_ENDED:
          ProductId from = ProductId.CreateFrom(this.m_activeMoneyOrGTAPPTransaction.PMTProductID.GetValueOrDefault());
          failDetails1 = this.m_activeMoneyOrGTAPPTransaction == null || !this.IsProductPrePurchase(this.GetBundleFromPmtProductId(from)) ? GameStrings.Get("GLUE_STORE_PRODUCT_EVENT_HAS_ENDED") : GameStrings.Get("GLUE_STORE_PRE_PURCHASE_HAS_ENDED");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_GENERIC_FAIL:
        case Network.PurchaseErrorInfo.ErrorType.BP_RISK_ERROR:
        case Network.PurchaseErrorInfo.ErrorType.BP_PAYMENT_AUTH:
        case Network.PurchaseErrorInfo.ErrorType.BP_PROVIDER_DENIED:
        case Network.PurchaseErrorInfo.ErrorType.E_BP_GENERIC_FAIL_RETRY_CONTACT_CS_IF_PERSISTS:
          if (!isGTAPP)
          {
            StoreSendToBAM.BAMReason reason = StoreSendToBAM.BAMReason.GENERIC_PAYMENT_FAIL;
            if (purchaseErrorType == Network.PurchaseErrorInfo.ErrorType.E_BP_GENERIC_FAIL_RETRY_CONTACT_CS_IF_PERSISTS)
              reason = StoreSendToBAM.BAMReason.GENERIC_PURCHASE_FAIL_RETRY_CONTACT_CS_IF_PERSISTS;
            this.HandleSendToBAMError(source, reason, purchaseErrorCode);
            if (!StoreManager.HasExternalStore)
              return;
            this.CompletePurchaseFailure(source, this.m_activeMoneyOrGTAPPTransaction, failDetails1, thirdPartyID, purchaseErrorType);
            return;
          }
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_GOLD_GENERIC");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_INVALID_CC_EXPIRY:
          if (!isGTAPP)
          {
            this.HandleSendToBAMError(source, StoreSendToBAM.BAMReason.CREDIT_CARD_EXPIRED, string.Empty);
            return;
          }
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_GOLD_GENERIC");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_NO_VALID_PAYMENT:
          if (source == StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE)
          {
            Log.Store.PrintWarning("StoreManager.HandlePurchaseError: received BP_NO_VALID_PAYMENT from payment method purchase error.");
            break;
          }
          if (!isGTAPP)
          {
            this.HandleSendToBAMError(source, StoreSendToBAM.BAMReason.NO_VALID_PAYMENT_METHOD, string.Empty);
            return;
          }
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_GOLD_GENERIC");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_PURCHASE_BAN:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_PURCHASE_BAN");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_SPENDING_LIMIT:
          failDetails1 = isGTAPP ? GameStrings.Get("GLUE_STORE_FAIL_GOLD_GENERIC") : GameStrings.Get("GLUE_STORE_FAIL_SPENDING_LIMIT");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_PARENTAL_CONTROL:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_PARENTAL_CONTROL");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_THROTTLED:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_THROTTLED");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_THIRD_PARTY_BAD_RECEIPT:
        case Network.PurchaseErrorInfo.ErrorType.BP_THIRD_PARTY_RECEIPT_USED:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_THIRD_PARTY_BAD_RECEIPT");
          break;
        case Network.PurchaseErrorInfo.ErrorType.BP_PRODUCT_UNIQUENESS_VIOLATED:
          this.HandleSendToBAMError(source, StoreSendToBAM.BAMReason.PRODUCT_UNIQUENESS_VIOLATED, string.Empty);
          return;
        case Network.PurchaseErrorInfo.ErrorType.BP_REGION_IS_DOWN:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_REGION_IS_DOWN");
          break;
        case Network.PurchaseErrorInfo.ErrorType.E_BP_CHALLENGE_ID_FAILED_VERIFICATION:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_CHALLENGE_ID_FAILED_VERIFICATION");
          break;
        default:
          failDetails1 = GameStrings.Get("GLUE_STORE_FAIL_GENERAL");
          break;
      }
      if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
        this.Status = StoreManager.TransactionStatus.READY;
      this.CompletePurchaseFailure(source, this.m_activeMoneyOrGTAPPTransaction, failDetails1, thirdPartyID, purchaseErrorType);
    }
  }

  private void SetActiveMoneyOrGTAPPTransaction(
    long id,
    long? pmtProductID,
    BattlePayProvider? provider,
    bool isGTAPP,
    bool tryToResolvePreviousTransactionNotices)
  {
    MoneyOrGTAPPTransaction gtappTransaction = new MoneyOrGTAPPTransaction(id, pmtProductID, provider, isGTAPP);
    bool flag = true;
    if (this.m_activeMoneyOrGTAPPTransaction != null)
    {
      if (gtappTransaction.Equals((object) this.m_activeMoneyOrGTAPPTransaction))
        flag = !this.m_activeMoneyOrGTAPPTransaction.Provider.HasValue && provider.HasValue;
      else if ((long) StoreManager.UNKNOWN_TRANSACTION_ID != this.m_activeMoneyOrGTAPPTransaction.ID)
        Log.Store.PrintWarning(string.Format("StoreManager.SetActiveMoneyOrGTAPPTransaction(id={0}, pmtProductId={1}, isGTAPP={2}, provider={3}) does not match active money or GTAPP transaction '{4}'", (object) id, (object) pmtProductID, (object) isGTAPP, provider.HasValue ? (object) provider.Value.ToString() : (object) "UNKNOWN", (object) this.m_activeMoneyOrGTAPPTransaction));
    }
    if (flag)
    {
      Log.Store.Print(string.Format("SetActiveMoneyOrGTAPPTransaction() {0}", (object) gtappTransaction));
      this.m_activeMoneyOrGTAPPTransaction = gtappTransaction;
    }
    if (this.m_firstMoneyOrGTAPPTransactionSet)
      return;
    this.m_firstMoneyOrGTAPPTransactionSet = true;
    if (!tryToResolvePreviousTransactionNotices)
      return;
    this.ResolveFirstMoneyOrGTAPPTransactionIfPossible();
  }

  private void ResolveFirstMoneyOrGTAPPTransactionIfPossible()
  {
    if (!this.m_firstMoneyOrGTAPPTransactionSet || !this.FirstNoticesProcessed || this.m_activeMoneyOrGTAPPTransaction == null || this.m_outstandingPurchaseNotices.Find((Predicate<NetCache.ProfileNoticePurchase>) (obj => obj.OriginData == this.m_activeMoneyOrGTAPPTransaction.ID)) != null)
      return;
    Log.Store.Print(string.Format("StoreManager.ResolveFirstMoneyTransactionIfPossible(): no outstanding notices for transaction {0}; setting m_activeMoneyOrGTAPPTransaction = null", (object) this.m_activeMoneyOrGTAPPTransaction));
    this.m_activeMoneyOrGTAPPTransaction = (MoneyOrGTAPPTransaction) null;
  }

  private void ConfirmActiveMoneyTransaction(long id)
  {
    if (this.m_activeMoneyOrGTAPPTransaction == null || this.m_activeMoneyOrGTAPPTransaction.ID != (long) StoreManager.UNKNOWN_TRANSACTION_ID && this.m_activeMoneyOrGTAPPTransaction.ID != id)
      Log.Store.PrintWarning(string.Format("StoreManager.ConfirmActiveMoneyTransaction(id={0}) does not match active money transaction '{1}'", (object) id, (object) this.m_activeMoneyOrGTAPPTransaction));
    Log.Store.Print(string.Format("ConfirmActiveMoneyTransaction() {0}", (object) id));
    List<NetCache.ProfileNoticePurchase> all = this.m_outstandingPurchaseNotices.FindAll(new Predicate<NetCache.ProfileNoticePurchase>(Predicate));
    this.m_outstandingPurchaseNotices.RemoveAll(new Predicate<NetCache.ProfileNoticePurchase>(Predicate));
    foreach (NetCache.ProfileNoticePurchase profileNoticePurchase in all)
      Network.Get().AckNotice(profileNoticePurchase.NoticeID);
    this.m_confirmedTransactionIDs.Add(id);
    this.m_activeMoneyOrGTAPPTransaction = (MoneyOrGTAPPTransaction) null;

    bool Predicate(NetCache.ProfileNoticePurchase obj) => obj.OriginData == id;
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    Log.Store.Print("StoreManager.OnNewNotices() New Notice");
    List<long> longList = new List<long>();
    foreach (NetCache.ProfileNotice newNotice in newNotices)
    {
      if (newNotice.Type == NetCache.ProfileNotice.NoticeType.PURCHASE)
      {
        if (newNotice.Origin == NetCache.ProfileNotice.NoticeOrigin.PURCHASE_CANCELED)
        {
          Log.Store.Print(string.Format("StoreManager.OnNewNotices() ack'ing purchase canceled notice for bpay ID {0}", (object) newNotice.OriginData));
          longList.Add(newNotice.NoticeID);
        }
        else if (this.m_confirmedTransactionIDs.Contains(newNotice.OriginData))
        {
          Log.Store.Print(string.Format("StoreManager.OnNewNotices() ack'ing purchase notice for already confirmed bpay ID {0}", (object) newNotice.OriginData));
          longList.Add(newNotice.NoticeID);
        }
        else
        {
          NetCache.ProfileNoticePurchase profileNoticePurchase = newNotice as NetCache.ProfileNoticePurchase;
          Log.Store.Print(string.Format("StoreManager.OnNewNotices() adding outstanding purchase notice for bpay ID {0}", (object) newNotice.OriginData));
          this.m_outstandingPurchaseNotices.Add(profileNoticePurchase);
        }
      }
    }
    Network network = Network.Get();
    foreach (long id in longList)
      network.AckNotice(id);
    if (this.FirstNoticesProcessed)
      return;
    this.FirstNoticesProcessed = true;
    if (this.Status != StoreManager.TransactionStatus.READY)
      return;
    this.ResolveFirstMoneyOrGTAPPTransactionIfPossible();
  }

  private void OnNetCacheFeaturesReady() => this.FeaturesReady = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>() != null;

  private void OnPurchaseCanceledResponse()
  {
    Network.PurchaseCanceledResponse canceledResponse = Network.Get().GetPurchaseCanceledResponse();
    switch (canceledResponse.Result)
    {
      case Network.PurchaseCanceledResponse.CancelResult.SUCCESS:
        Log.Store.Print("StoreManager.OnPurchaseCanceledResponse(): purchase successfully canceled.");
        this.ConfirmActiveMoneyTransaction(canceledResponse.TransactionID);
        this.Status = StoreManager.TransactionStatus.READY;
        this.m_previousStatusBeforeAutoCancel = StoreManager.TransactionStatus.UNKNOWN;
        break;
      case Network.PurchaseCanceledResponse.CancelResult.NOT_ALLOWED:
        Log.Store.PrintWarning("StoreManager.OnPurchaseCanceledResponse(): cancel purchase is not allowed right now.");
        bool isGTAPP = Currency.IsGTAPP(canceledResponse.CurrencyCode);
        this.SetActiveMoneyOrGTAPPTransaction(canceledResponse.TransactionID, canceledResponse.PMTProductID, MoneyOrGTAPPTransaction.UNKNOWN_PROVIDER, isGTAPP, true);
        this.Status = isGTAPP ? StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP : StoreManager.TransactionStatus.IN_PROGRESS_MONEY;
        if (this.m_previousStatusBeforeAutoCancel == StoreManager.TransactionStatus.UNKNOWN)
          break;
        this.Status = this.m_previousStatusBeforeAutoCancel;
        this.m_previousStatusBeforeAutoCancel = StoreManager.TransactionStatus.UNKNOWN;
        break;
      case Network.PurchaseCanceledResponse.CancelResult.NOTHING_TO_CANCEL:
        this.m_previousStatusBeforeAutoCancel = StoreManager.TransactionStatus.UNKNOWN;
        if (this.m_activeMoneyOrGTAPPTransaction != null && (long) StoreManager.UNKNOWN_TRANSACTION_ID != this.m_activeMoneyOrGTAPPTransaction.ID)
          this.ConfirmActiveMoneyTransaction(this.m_activeMoneyOrGTAPPTransaction.ID);
        this.Status = StoreManager.TransactionStatus.READY;
        break;
    }
  }

  private bool IsConclusiveState(Network.PurchaseErrorInfo.ErrorType errorType)
  {
    switch (errorType)
    {
      case Network.PurchaseErrorInfo.ErrorType.UNKNOWN:
      case Network.PurchaseErrorInfo.ErrorType.STILL_IN_PROGRESS:
      case Network.PurchaseErrorInfo.ErrorType.WAIT_MOP:
      case Network.PurchaseErrorInfo.ErrorType.WAIT_CONFIRM:
      case Network.PurchaseErrorInfo.ErrorType.WAIT_RISK:
      case Network.PurchaseErrorInfo.ErrorType.WAIT_THIRD_PARTY_RECEIPT:
        return false;
      default:
        return true;
    }
  }

  private void OnBattlePayStatusResponse()
  {
    Network.BattlePayStatus payStatusResponse = Network.Get().GetBattlePayStatusResponse();
    if (payStatusResponse.BattlePayAvailable != this.BattlePayAvailable)
    {
      this.BattlePayAvailable = payStatusResponse.BattlePayAvailable;
      Log.Store.Print("Store server status is now {0}", this.BattlePayAvailable ? (object) "available" : (object) "unavailable");
    }
    switch (payStatusResponse.State)
    {
      case Network.BattlePayStatus.PurchaseState.READY:
        this.Status = StoreManager.TransactionStatus.READY;
        Log.Store.Print("Store PurchaseState is READY.");
        break;
      case Network.BattlePayStatus.PurchaseState.CHECK_RESULTS:
        Log.Store.Print("Store PurchaseState is CHECK_RESULTS.");
        bool isGTAPP = Currency.IsGTAPP(payStatusResponse.CurrencyCode);
        bool tryToResolvePreviousTransactionNotices = this.IsConclusiveState(payStatusResponse.PurchaseError.Error);
        this.SetActiveMoneyOrGTAPPTransaction(payStatusResponse.TransactionID, payStatusResponse.PMTProductID, payStatusResponse.Provider, isGTAPP, tryToResolvePreviousTransactionNotices);
        this.HandlePurchaseError(StoreManager.PurchaseErrorSource.FROM_STATUS_OR_PURCHASE_RESPONSE, payStatusResponse.PurchaseError.Error, payStatusResponse.PurchaseError.ErrorCode, payStatusResponse.ThirdPartyID, isGTAPP);
        break;
      case Network.BattlePayStatus.PurchaseState.ERROR:
        Log.Store.PrintError("Store PurchaseState is ERROR.");
        break;
      default:
        Log.Store.PrintError("Store PurchaseState is unknown value {0}.", (object) payStatusResponse.State);
        break;
    }
  }

  private static string GetExternalStoreProductId(Network.Bundle bundle)
  {
    if ((Record) bundle == (Record) null)
    {
      Log.Store.PrintError("[GetExternalStoreProductId] There was no bundle object properly sent.");
      return (string) null;
    }
    long? pmtProductId = bundle.PMTProductID;
    if (pmtProductId.HasValue)
    {
      pmtProductId = bundle.PMTProductID;
      if (!ProductId.IsValid(pmtProductId.Value))
        return (string) null;
    }
    if (!ServiceManager.TryGet<HearthstoneCheckout>(out HearthstoneCheckout _))
      return (string) null;
    pmtProductId = bundle.PMTProductID;
    ProductId.CreateFrom(pmtProductId.Value);
    return (string) null;
  }

  public static bool HasExternalStore => (bool) StoreManager.HAS_THIRD_PARTY_APP_STORE;

  private void OnBattlePayConfigResponse()
  {
    Network.BattlePayConfig payConfigResponse = Network.Get().GetBattlePayConfigResponse();
    if (!payConfigResponse.Available)
    {
      Log.Store.PrintWarning("Server responds that store is unavailable.");
      this.BattlePayAvailable = false;
    }
    else
    {
      Log.Store.ForceFilePrint(Blizzard.T5.Logging.LogLevel.Info, "Received BattlePayConfig response from the server.");
      HearthstoneCheckout.OneStoreKey = payConfigResponse.CheckoutKrOnestoreKey;
      Log.Store.Print("Server responds that store is available.");
      this.BattlePayAvailable = true;
      this.m_currency = payConfigResponse.Currency;
      this.m_secsBeforeAutoCancel = (float) payConfigResponse.SecondsBeforeAutoCancel;
      this.m_bundles.Clear();
      foreach (Network.Bundle bundle1 in payConfigResponse.Bundles)
      {
        long? pmtProductId = bundle1.PMTProductID;
        if (pmtProductId.HasValue)
        {
          pmtProductId = bundle1.PMTProductID;
          if (ProductId.IsValid(pmtProductId.Value))
          {
            Map<ProductId, Network.Bundle> bundles = this.m_bundles;
            pmtProductId = bundle1.PMTProductID;
            ProductId from = ProductId.CreateFrom(pmtProductId.Value);
            Network.Bundle bundle2 = bundle1;
            bundles.Add(from, bundle2);
          }
        }
      }
      this.m_goldCostBooster.Clear();
      foreach (Network.GoldCostBooster goldCostBooster in payConfigResponse.GoldCostBoosters)
        this.m_goldCostBooster.Add(goldCostBooster.ID, goldCostBooster);
      this.m_goldCostArena = payConfigResponse.GoldCostArena;
      Processor.QueueJob("StoreManage.SetPersonalizedData", this.SetPersonalizedShopPageAndRefreshCatalog(payConfigResponse.PersonalizedShopPages));
      this.m_sales.Clear();
      foreach (Network.ShopSale sale in payConfigResponse.SaleList)
        this.m_sales[sale.SaleId] = sale;
      this.m_ignoreProductTiming = payConfigResponse.IgnoreProductTiming;
      Log.Store.Print("StoreManager.OnBattlePayConfigResponse: Queueing ConfigureCheckout Job.");
      Processor.QueueJob("StoreManager.ConfigureCheckoutFromBattlePayConfig", this.Job_ConfigureCheckoutFromBattlePayConfig(payConfigResponse.CommerceClientID, this.m_currency != null ? this.m_currency.Code : ""), ServiceManager.CreateServiceSoftDependency(typeof (HearthstoneCheckout)));
    }
  }

  private IEnumerator<IAsyncJobResult> Job_ConfigureCheckoutFromBattlePayConfig(
    string clientID,
    string currencyCode)
  {
    HearthstoneCheckout service;
    if (ServiceManager.TryGet<HearthstoneCheckout>(out service))
    {
      long[] productCatalog = new long[this.m_bundles.Count];
      Map<ProductId, Network.Bundle>.ValueCollection values = this.m_bundles.Values;
      int index1 = 0;
      for (int count = this.m_bundles.Count; index1 < count; ++index1)
      {
        Network.Bundle bundle = values.ElementAt<Network.Bundle>(index1);
        long? pmtProductId = bundle.PMTProductID;
        if (pmtProductId.HasValue)
        {
          long[] numArray = productCatalog;
          int index2 = index1;
          pmtProductId = bundle.PMTProductID;
          long num = pmtProductId.Value;
          numArray[index2] = num;
        }
      }
      service.SetClientID(clientID);
      service.SetProductCatalog(productCatalog);
      service.SetCurrencyCode(currencyCode);
      Log.Store.Print("StoreManager.ConfigureCheckoutFromBattlePayConfig: Queueing FireStatusChangeEventForHearthstoneCheckout Job.");
      Processor.QueueJob("StoreManager.FireStatusChangeEventForHearthstoneCheckout", this.OnCheckoutInitializationComplete(), (IJobDependency) new WaitForCheckoutInitialized());
    }
    else
    {
      Log.Store.Print("StoreManager.ConfigureCheckoutFromBattlePayConfig: HearthstoneCheckout is unavailable.");
      this.OnCheckoutInitializationComplete();
      yield break;
    }
  }

  private IEnumerator<IAsyncJobResult> OnCheckoutInitializationComplete()
  {
    Log.Store.Print("StoreManager.OnCheckoutInitializationComplete: OnCheckoutInitializationComplete called.");
    while (!HearthstoneCheckout.ReceivedSdkProducts)
      yield return (IAsyncJobResult) null;
    this.ValidateBundles();
    this.ConfigLoaded = true;
  }

  private void ValidateBundles()
  {
    HearthstoneCheckout commerce;
    if (!ServiceManager.TryGet<HearthstoneCheckout>(out commerce))
    {
      Log.Store.Print("StoreManager.ValidateBundles: Failed to retrieve commerce service.");
    }
    else
    {
      Log.Store.Print("StoreManager.ValidateBundles: Validating bundles.");
      foreach (ProductId key in this.m_bundles.Where<KeyValuePair<ProductId, Network.Bundle>>((Func<KeyValuePair<ProductId, Network.Bundle>, bool>) (kvp => !StoreManager.IsBundleValid(kvp.Value, commerce))).Select<KeyValuePair<ProductId, Network.Bundle>, ProductId>((Func<KeyValuePair<ProductId, Network.Bundle>, ProductId>) (kvp => kvp.Key)).ToList<ProductId>())
        this.m_bundles.Remove(key);
      foreach (ProductId missingProduct in commerce.FindMissingProducts((IEnumerable<ProductId>) this.m_bundles.Keys))
        ProductIssues.LogError(missingProduct, "Missing from BPAY Util Server");
    }
  }

  private static bool IsBundleValid(Network.Bundle bundle, HearthstoneCheckout commerce)
  {
    string str = bundle.DisplayName?.GetString();
    long? pmtProductId = bundle.PMTProductID;
    if (!pmtProductId.HasValue)
    {
      Log.Store.Print(!string.IsNullOrWhiteSpace(str) ? "StoreManager::IsBundleValid - Bundle has no Product ID (" + str + "." : "StoreManager::IsBundleValid - Bundle has no Product ID.");
      return false;
    }
    pmtProductId = bundle.PMTProductID;
    if (!ProductId.IsValid(pmtProductId.Value))
    {
      Network.Bundle netBundle = bundle;
      pmtProductId = bundle.PMTProductID;
      string format = string.Format("Has invalid PMT ID {0}", (object) pmtProductId.Value);
      object[] objArray = Array.Empty<object>();
      ProductIssues.LogError(netBundle, format, objArray);
      return false;
    }
    if (bundle.IsGoldOnly() || bundle.IsFree())
      return true;
    HearthstoneCheckout hearthstoneCheckout = commerce;
    pmtProductId = bundle.PMTProductID;
    ProductId from = ProductId.CreateFrom(pmtProductId.Value);
    if (!hearthstoneCheckout.IsProductValid(from))
    {
      ProductIssues.LogError(bundle, "Missing from Commerce service. You may require a bypassed account.");
      return false;
    }
    if (bundle.HasValidPrices())
      return true;
    ProductIssues.LogError(bundle, "Missing price from Commerce service.");
    return false;
  }

  private void HandleZeroCostLicensePurchaseMethod(Network.PurchaseMethod method)
  {
    if (Network.PurchaseErrorInfo.ErrorType.STILL_IN_PROGRESS != method.PurchaseError.Error)
    {
      Log.Store.PrintWarning(string.Format("StoreManager.HandleZeroCostLicensePurchaseMethod() FAILED error={0}", (object) method.PurchaseError.Error));
      this.Status = StoreManager.TransactionStatus.READY;
    }
    else
      Log.Store.Print("StoreManager.HandleZeroCostLicensePurchaseMethod succeeded, refreshing achieves");
  }

  private void OnPurchaseMethod()
  {
    Network.PurchaseMethod purchaseMethodResponse = Network.Get().GetPurchaseMethodResponse();
    if (purchaseMethodResponse.IsZeroCostLicense)
    {
      this.HandleZeroCostLicensePurchaseMethod(purchaseMethodResponse);
    }
    else
    {
      this.m_challengePurchaseMethod = string.IsNullOrEmpty(purchaseMethodResponse.ChallengeID) || string.IsNullOrEmpty(purchaseMethodResponse.ChallengeURL) ? (Network.PurchaseMethod) null : purchaseMethodResponse;
      bool isGTAPP = Currency.IsGTAPP(purchaseMethodResponse.CurrencyCode);
      this.SetActiveMoneyOrGTAPPTransaction(purchaseMethodResponse.TransactionID, purchaseMethodResponse.PMTProductID, new BattlePayProvider?(BattlePayProvider.BP_PROVIDER_BLIZZARD), isGTAPP, false);
      if (purchaseMethodResponse.PurchaseError != null)
      {
        this.HandlePurchaseError(StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE, purchaseMethodResponse.PurchaseError.Error, purchaseMethodResponse.PurchaseError.ErrorCode, string.Empty, isGTAPP);
      }
      else
      {
        this.BlockStoreInterface();
        if (isGTAPP)
        {
          this.OnSummaryConfirm(purchaseMethodResponse.Quantity, (object) null);
        }
        else
        {
          string paymentMethodName = !purchaseMethodResponse.UseEBalance ? purchaseMethodResponse.WalletName : GameStrings.Get("GLUE_STORE_BNET_BALANCE");
          IStore currentStore = this.GetCurrentStore();
          if (currentStore == null || !currentStore.IsOpen())
          {
            this.AutoCancelPurchaseIfPossible();
          }
          else
          {
            this.m_view.PurchaseAuth.Hide();
            this.Status = StoreManager.TransactionStatus.WAIT_CONFIRM;
            this.m_view.Summary.Show(ProductId.CreateFrom(purchaseMethodResponse.PMTProductID ?? -1L), purchaseMethodResponse.Quantity, paymentMethodName);
          }
        }
      }
    }
  }

  private void OnPurchaseResponse()
  {
    Network.PurchaseResponse purchaseResponse = Network.Get().GetPurchaseResponse();
    bool isGTAPP = Currency.IsGTAPP(purchaseResponse.CurrencyCode);
    this.SetActiveMoneyOrGTAPPTransaction(purchaseResponse.TransactionID, purchaseResponse.PMTProductID, MoneyOrGTAPPTransaction.UNKNOWN_PROVIDER, isGTAPP, false);
    this.HandlePurchaseError(StoreManager.PurchaseErrorSource.FROM_STATUS_OR_PURCHASE_RESPONSE, purchaseResponse.PurchaseError.Error, purchaseResponse.PurchaseError.ErrorCode, purchaseResponse.ThirdPartyID, isGTAPP);
  }

  private void OnPurchaseViaGoldResponse()
  {
    string details;
    switch (Network.Get().GetPurchaseWithGoldResponse().Error)
    {
      case Network.PurchaseViaGoldResponse.ErrorType.SUCCESS:
        this.HandlePurchaseSuccess(new StoreManager.PurchaseErrorSource?(), (MoneyOrGTAPPTransaction) null, string.Empty, (TransactionData) null);
        return;
      case Network.PurchaseViaGoldResponse.ErrorType.INSUFFICIENT_GOLD:
        details = GameStrings.Get("GLUE_STORE_FAIL_NOT_ENOUGH_GOLD");
        break;
      case Network.PurchaseViaGoldResponse.ErrorType.PRODUCT_NA:
        details = GameStrings.Get("GLUE_STORE_FAIL_PRODUCT_NA");
        break;
      case Network.PurchaseViaGoldResponse.ErrorType.FEATURE_NA:
        details = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
        break;
      case Network.PurchaseViaGoldResponse.ErrorType.INVALID_QUANTITY:
        details = GameStrings.Get("GLUE_STORE_FAIL_QUANTITY");
        break;
      default:
        details = GameStrings.Get("GLUE_STORE_FAIL_GENERAL");
        break;
    }
    this.Status = StoreManager.TransactionStatus.READY;
    this.m_view.PurchaseAuth.CompletePurchaseFailure((MoneyOrGTAPPTransaction) null, details, Network.PurchaseErrorInfo.ErrorType.BP_GENERIC_FAIL);
  }

  private void OnThirdPartyPurchaseStatusResponse() => Log.Store.PrintWarning("[StoreManager.OnThirdPartyPurchaseStatusResponse] Received OnThirdPartyPurchaseStatusResponse packet.  Legacy third party purchasing has been removed.");

  private void StoreViewReady()
  {
    if (!this.m_waitingToShowStore || !this.IsCurrentStoreLoaded())
      return;
    this.ShowStore();
  }

  private void OnGeneralStoreLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    GeneralStore generalStore = this.OnStoreLoaded<GeneralStore>(go, ShopType.GENERAL_STORE);
    if (!((UnityEngine.Object) generalStore != (UnityEngine.Object) null))
      return;
    this.SetupLoadedStore((IStore) generalStore);
  }

  private void OnLettuceCollectionLoaded()
  {
    this.LoadGeneralStore();
    this.m_view.LoadAssets();
  }

  private void OnArenaStoreLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    ArenaStore arenaStore = this.OnStoreLoaded<ArenaStore>(go, ShopType.ARENA_STORE);
    if (!((UnityEngine.Object) arenaStore != (UnityEngine.Object) null))
      return;
    this.SetupLoadedStore((IStore) arenaStore);
  }

  private void OnBrawlStoreLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    TavernBrawlStore tavernBrawlStore = this.OnStoreLoaded<TavernBrawlStore>(go, ShopType.TAVERN_BRAWL_STORE);
    if (!((UnityEngine.Object) tavernBrawlStore != (UnityEngine.Object) null))
      return;
    this.SetupLoadedStore((IStore) tavernBrawlStore);
  }

  private void OnAdventureStoreLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    AdventureStore adventureStore = this.OnStoreLoaded<AdventureStore>(go, ShopType.ADVENTURE_STORE);
    if (!((UnityEngine.Object) adventureStore != (UnityEngine.Object) null))
      return;
    this.SetupLoadedStore((IStore) adventureStore);
  }

  private void OnAdventureWingStoreLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    AdventureStore adventureStore = this.OnStoreLoaded<AdventureStore>(go, ShopType.ADVENTURE_STORE_WING_PURCHASE_WIDGET);
    if (!((UnityEngine.Object) adventureStore != (UnityEngine.Object) null))
      return;
    this.SetupLoadedStore((IStore) adventureStore);
  }

  private void OnAdventureFullStoreLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    AdventureStore adventureStore = this.OnStoreLoaded<AdventureStore>(go, ShopType.ADVENTURE_STORE_FULL_PURCHASE_WIDGET);
    if (!((UnityEngine.Object) adventureStore != (UnityEngine.Object) null))
      return;
    this.SetupLoadedStore((IStore) adventureStore);
  }

  private T OnStoreLoaded<T>(GameObject go, ShopType shopType) where T : Store
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("StoreManager.OnStoreLoaded<{0}>(): go is null!", (object) typeof (T)));
      return default (T);
    }
    T obj = go.GetComponent<T>();
    if ((UnityEngine.Object) obj == (UnityEngine.Object) null)
      obj = go.GetComponentInChildren<T>();
    if ((UnityEngine.Object) obj == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("StoreManager.OnStoreLoaded<{0}>(): go has no {1} component!", (object) typeof (T), (object) typeof (T)));
      return default (T);
    }
    this.m_stores[shopType] = (IStore) obj;
    return obj;
  }

  private void SendShopPurchaseEventTelemetry(bool isComplete)
  {
    if (this.m_pendingProductPurchaseArgs == null)
    {
      Log.Store.PrintWarning("No active transaction in progress");
    }
    else
    {
      Blizzard.Telemetry.WTCG.Client.Product product = new Blizzard.Telemetry.WTCG.Client.Product();
      ProductId productId;
      string currencyCode;
      long totalPrice;
      int quantity;
      string productItemType;
      int productItemId;
      if (!ShopUtils.TryDecomposeBuyProductEventArgs(this.m_pendingProductPurchaseArgs, out productId, out currencyCode, out totalPrice, out quantity, out productItemType, out productItemId))
      {
        Log.Store.PrintError("Failed to decompose pending product purchase args for telemetry.");
      }
      else
      {
        BattlePayProvider? nullable = this.ActiveTransactionProvider();
        string storefront = nullable.HasValue ? nullable.Value.ToString().ToLowerInvariant() : "";
        product.ProductId = productId.IsValid() ? productId.Value : -1L;
        product.HsProductType = productItemType;
        product.HsProductId = productItemId;
        TelemetryManager.Client().SendShopPurchaseEvent(product, quantity, currencyCode, (double) totalPrice, false, storefront, isComplete, this.m_currentShopType.ToString());
      }
    }
  }

  public void RegisterAmazingNewShop(Shop amazingNewShop) => this.SetupLoadedStore((IStore) amazingNewShop);

  private void SetupLoadedStore(IStore store)
  {
    if (store == null)
      return;
    store.OnProductPurchaseAttempt += (System.Action<BuyProductEventArgs>) (args =>
    {
      if (args == null)
      {
        Log.Store.PrintError("Cannot attempt purchase due to null BuyProductEventArgs");
      }
      else
      {
        BuyPmtProductEventArgs args1 = args as BuyPmtProductEventArgs;
        this.m_pendingProductPurchaseArgs = args;
        this.SendShopPurchaseEventTelemetry(false);
        switch (args.PaymentCurrency)
        {
          case CurrencyType.GOLD:
            if (args is BuyNoGTAPPEventArgs noGtappEventArgs2)
            {
              this.OnStoreBuyWithGoldNoGTAPP(noGtappEventArgs2.transactionData);
              break;
            }
            this.OnStoreBuyWithGTAPP(args1);
            break;
          case CurrencyType.REAL_MONEY:
            this.OnStoreBuyWithMoney(args1);
            break;
          default:
            if (ShopUtils.IsCurrencyVirtual(args.PaymentCurrency))
            {
              this.OnStoreBuyWithCheckout(args1);
              break;
            }
            Log.Store.PrintError("Attempted purchase with invalid currency type {0}", (object) args.PaymentCurrency);
            break;
        }
      }
    });
    store.OnOpened += new System.Action(this.OnStoreOpen);
    store.OnClosed += (System.Action<StoreClosedArgs>) (e => this.OnStoreExit(e.authorizationBackButtonPressed.GetValueOrDefault(false), (object) null));
    store.OnReady += new System.Action(this.StoreViewReady);
    if (store is Store store1)
      store1.RegisterInfoListener(new Store.InfoCallback(this.OnStoreInfo));
    this.StoreViewReady();
  }

  private void OnSceneUnloaded(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData) => this.UnloadAndFreeMemory();

  private IEnumerator<IAsyncJobResult> RequestPersonalizedShopData()
  {
    HearthstoneCheckout service = (HearthstoneCheckout) null;
    while (!ServiceManager.TryGet<HearthstoneCheckout>(out service) || !service.IsIdle)
      yield return (IAsyncJobResult) null;
    this.m_receivedShopTypeSections.Clear();
    this.m_requestedShopTypeSections.Clear();
    this.m_requestedShopTypeSections.UnionWith((IEnumerable<ShopType>) this.m_shopPageIds.Keys);
    foreach (KeyValuePair<ShopType, string> shopPageId in this.m_shopPageIds)
      Log.Store.PrintDebug(string.Format("Getting personalized shop data for store: {0} with page id: {1}", (object) shopPageId.Key, (object) shopPageId.Value));
  }

  public bool HasReceivedAllShopTypeSections() => this.m_catalogNetworkPages.Contains((IEnumerable<ShopType>) this.m_requestedShopTypeSections);

  public void QueueGetPersonalizedShopJobs()
  {
    HearthstoneCheckout service;
    if (!ServiceManager.TryGet<HearthstoneCheckout>(out service))
    {
      Log.Store.PrintError("[QueueGetPersonalizedShopJobs]Unable to access hearthstone checkout");
    }
    else
    {
      JobDefinition job = new JobDefinition("StoreManager.RequestingPersonalizedShopId", this.RequestPersonalizedShopData(), Array.Empty<IJobDependency>());
      Processor.QueueJob(job);
      Processor.QueueJob("HearthstoneCheckout.GetPersonalizedShopData", service.GetPersonalizedShopData(this.m_shopPageIds.Values.ToArray<string>(), new HearthstoneCheckout.PersonalizedShopResponseCallback(this.OnHearthstoneGetPersonalizedShopData)), (IJobDependency) job.CreateDependency());
    }
  }

  public IEnumerator<IAsyncJobResult> SetPersonalizedShopPageAndRefreshCatalog(
    List<BattlePayConfigShopPage> pages)
  {
    if (pages.Count == 0)
    {
      Log.Store.PrintError("No page id data was found.");
    }
    else
    {
      this.m_shopPageIds.Clear();
      foreach (BattlePayConfigShopPage page in pages)
      {
        ShopType key;
        if (this.m_shopTypeBySharedShopType.TryGetValue(page.ShopType, out key))
          this.m_shopPageIds.Add(key, page.PersonalizedShopPageId);
      }
      this.QueueGetPersonalizedShopJobs();
      yield break;
    }
  }

  public void OnHearthstoneGetPersonalizedShopData(GetPagesResponse response)
  {
    Log.Store.PrintDebug("[StoreManager.OnHearthstoneGetPersonalizedShopData] Recieved responses");
    if (response == null)
      return;
    if (response.pages == null)
    {
      Log.Store.PrintError("No page data was found for page ids \"{0}\"", (object) string.Join(", ", this.m_shopPageIds.Select<KeyValuePair<ShopType, string>, string>((Func<KeyValuePair<ShopType, string>, string>) (kvp => kvp.Value))));
      if (response.error == null || string.IsNullOrEmpty(response.error.code))
        return;
      Log.Store.PrintError("GetPageResponse Error: code:{0}, message:{1}", string.IsNullOrEmpty(response.error.code) ? (object) "?" : (object) response.error.code, string.IsNullOrEmpty(response.error.message) ? (object) string.Empty : (object) response.error.message);
    }
    else
    {
      List<ShopType> shopTypeList = new List<ShopType>();
      foreach (Page page in response.pages)
      {
        foreach (KeyValuePair<ShopType, string> shopPageId in this.m_shopPageIds)
        {
          if (!(shopPageId.Value != page.pageId) && !shopTypeList.Contains(shopPageId.Key))
            shopTypeList.Add(shopPageId.Key);
        }
      }
      if (shopTypeList.Count == 0)
      {
        Log.Store.PrintError("GetPageResponse Error: Page IDs [{0}] correspond to no known Shop Type", (object) string.Join(",", response.pages.Select<Page, string>((Func<Page, string>) (x => x.pageId))));
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder(32);
        for (int index1 = 0; index1 < shopTypeList.Count; ++index1)
        {
          ShopType shopType = shopTypeList[index1];
          if (!this.m_requestedShopTypeSections.Contains(shopType))
          {
            Log.Store.PrintError("GetPageResponse Error: Shop type {0} has no pending request. Page IDs = {1}", (object) shopType, (object) string.Join(",", response.pages.Select<Page, string>((Func<Page, string>) (x => x.pageId))));
          }
          else
          {
            if (!this.m_receivedShopTypeSections.Add(shopType))
              Log.Store.PrintError("GetPageResponse Error: Shop type {0} received page response more than once. Page IDs = {1}", (object) shopType, (object) string.Join(",", response.pages.Select<Page, string>((Func<Page, string>) (x => x.pageId))));
            Log.Store.PrintDebug("Section Data (page Ids {0}):", (object) string.Join(",", response.pages.Select<Page, string>((Func<Page, string>) (x => x.pageId))));
            CatalogNetworkPage page = this.m_catalogNetworkPages.GetOrCreatePage(shopType);
            page.Clear();
            foreach (Section section1 in response.pages[index1].sections)
            {
              stringBuilder.Clear();
              stringBuilder.Append(string.Format("section {0}: {1}", (object) (page.SectionsCount + 1), (object) section1.name));
              Network.ShopSection section2 = new Network.ShopSection();
              section2.InternalName = section1.name;
              if (section1.localization != null)
              {
                section2.Label = new DbfLocValue();
                section2.Label.SetString(section1.localization.name);
              }
              if (section1.orderInPage >= 0)
              {
                section2.SortOrder = section1.orderInPage;
                Network.ShopSection sectionBySortOrder = page.GetSectionBySortOrder(section2.SortOrder);
                if (sectionBySortOrder != null)
                  Log.Store.PrintError("section {0} has the same SortOrder as {1}: {2}. Order may be inconsistent", (object) section1.name, (object) sectionBySortOrder.InternalName, (object) section2.SortOrder);
                stringBuilder.Append(string.Format("\n  sortOrder={0}", (object) section2.SortOrder));
              }
              else
                Log.Store.PrintError("section {0} missing OrderInPage", (object) section1.name);
              List<SectionAttribute> attributes = section1.attributes;
              int index2 = 0;
              for (int count = attributes.Count; index2 < count; ++index2)
              {
                SectionAttribute sectionAttribute = attributes[index2];
                if (sectionAttribute.sectionAttributeKey.Equals("Style"))
                {
                  section2.Style = sectionAttribute.sectionAttributeValue;
                  stringBuilder.Append("\n  Style=" + section2.Style);
                  break;
                }
              }
              int index3 = 0;
              for (int count = attributes.Count; index3 < count; ++index3)
              {
                SectionAttribute sectionAttribute = attributes[index3];
                if (sectionAttribute.sectionAttributeKey.Equals("TreatTagsAsFiller"))
                {
                  section2.FillerTags = sectionAttribute.sectionAttributeValue;
                  break;
                }
              }
              section2.Products = new List<Network.ShopSection.ProductRef>();
              foreach (ProductCollection productCollection in section1.productCollections)
              {
                foreach (ProductCollectionItem productCollectionItem in productCollection.items)
                {
                  Network.ShopSection.ProductRef productRef = new Network.ShopSection.ProductRef()
                  {
                    OrderId = productCollectionItem.orderInProductCollection,
                    PmtId = (long) productCollectionItem.productCollectionItemValue
                  };
                  section2.Products.Add(productRef);
                  stringBuilder.Append(string.Format("\n    [{0}]={1}", (object) productRef.OrderId, (object) productRef.PmtId));
                }
              }
              section2.Attributes = CommerceUtils.ConvertAttributes(section1.attributes);
              page.AddSection(section2);
              Log.Store.PrintDebug(stringBuilder.ToString());
            }
          }
        }
        if (!this.m_requestedShopTypeSections.IsSubsetOf((IEnumerable<ShopType>) this.m_receivedShopTypeSections))
          return;
        Processor.QueueJob("Load_Products", ServiceManager.Get<HearthstoneCheckout>().LoadProducts(), JobFlags.StartImmediately);
      }
    }
  }

  public void HandleCommerceCancelEvent()
  {
    this.Status = StoreManager.TransactionStatus.READY;
    if (this.m_activeMoneyOrGTAPPTransaction != null)
      this.ConfirmActiveMoneyTransaction(this.m_activeMoneyOrGTAPPTransaction.ID);
    this.m_view.PurchaseAuth.Hide();
    Network.Get().ReportBlizzardCheckoutStatus(BlizzardCheckoutStatus.BLIZZARD_CHECKOUT_STATUS_CANCELED);
    TelemetryManager.Client().SendBlizzardCheckoutPurchaseCancel();
  }

  public void HandleCommerceCloseEvent()
  {
    if (!this.m_view.PurchaseAuth.IsShown)
    {
      this.SetCanTapOutConfirmationUI(true);
      this.UnblockStoreInterface();
      if (this.Status != StoreManager.TransactionStatus.IN_PROGRESS_BLIZZARD_CHECKOUT && !this.m_showStoreData.closeOnTransactionComplete)
        return;
      this.GetCurrentStore()?.Close();
    }
    else
    {
      if (this.Status == StoreManager.TransactionStatus.READY)
        return;
      this.m_view.PurchaseAuth.Hide();
      this.UnblockStoreInterface();
    }
  }

  public void HandleCommerceOrderPending(TransactionData data)
  {
    if (this.IsCommerceUiShowing() || this.m_view.PurchaseAuth.IsShown)
      this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false);
    this.Status = StoreManager.TransactionStatus.IN_PROGRESS_BLIZZARD_CHECKOUT;
    if (data == null)
      return;
    Network.Get().ReportBlizzardCheckoutStatus(BlizzardCheckoutStatus.BLIZZARD_CHECKOUT_STATUS_START, data);
    TelemetryManager.Client().SendBlizzardCheckoutPurchaseStart(data.TransactionID, data.ProductID.ToString(), data.CurrencyCode);
  }

  public void HandleCommerceOrderFailure(TransactionData data)
  {
    if (!this.m_view.PurchaseAuth.IsShown)
      this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false);
    this.m_view.PurchaseAuth.CompletePurchaseFailure(this.m_activeMoneyOrGTAPPTransaction, this.GetHearthstoneCheckoutErrorString(data.ErrorCodes), Network.PurchaseErrorInfo.ErrorType.BP_GENERIC_FAIL);
    this.Status = StoreManager.TransactionStatus.READY;
    Network.Get().ReportBlizzardCheckoutStatus(BlizzardCheckoutStatus.BLIZZARD_CHECKOUT_STATUS_COMPLETED_FAILED, data);
    TelemetryManager.Client().SendBlizzardCheckoutPurchaseCompletedFailure(data.TransactionID, data.ProductID.ToString(), data.CurrencyCode, new List<string>()
    {
      data.ErrorCodes ?? string.Empty
    });
    Log.Store.PrintError("Checkout Order Failure: TransactionID={0}, ProductID={1}, CurrencyCode={2}, ErrorCodes={3}", (object) data.TransactionID, (object) data.ProductID, (object) data.CurrencyCode, (object) data.ErrorCodes);
  }

  public void HandleCommerceSubmitFailure()
  {
    if (!this.m_view.PurchaseAuth.IsShown)
      this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false);
    this.m_view.PurchaseAuth.CompletePurchaseFailure(this.m_activeMoneyOrGTAPPTransaction, GameStrings.Get("GLUE_CHECKOUT_ERROR_GENERIC_FAILURE"), Network.PurchaseErrorInfo.ErrorType.BP_GENERIC_FAIL);
    this.Status = StoreManager.TransactionStatus.READY;
  }

  private string GetHearthstoneCheckoutErrorString(string errorCode)
  {
    if (errorCode == "BLZBNTPURJNL42203")
      return GameStrings.Get("GLUE_STORE_FAIL_PRODUCT_ALREADY_OWNED");
    if (errorCode == "BLZBNTPURJNL42208")
      return GameStrings.Get("GLUE_STORE_FAIL_SPENDING_LIMIT");
    if (errorCode == "BLZBNTPUR3000003" || errorCode == "10201001")
      return GameStrings.Get("GLUE_CHECKOUT_ERROR_INSUFFICIENT_FUNDS");
    if (errorCode == "30000101")
      return GameStrings.Get("GLUE_CHECKOUT_ERROR_PRODUCT_UNAVAILABLE");
    if (errorCode == "10010101")
      ;
    Log.Store.PrintWarning("Unhandled checkout error: {0}", (object) errorCode);
    return GameStrings.Get("GLUE_CHECKOUT_ERROR_GENERIC_FAILURE");
  }

  public void HandleCommerceOrderComplete(TransactionData data)
  {
    if (this.IsCommerceUiShowing() && !this.m_view.PurchaseAuth.IsShown)
      this.m_view.PurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false);
    try
    {
      this.SendAttributionPurchaseMessage(data);
    }
    catch (Exception ex)
    {
      Log.Store.PrintError(string.Format("[SendAttributionPurchaseMessage] Error during purchase attribution message send.\nException: {0}", (object) ex));
    }
    AdventureStore currentStore = this.GetCurrentStore() as AdventureStore;
    if ((UnityEngine.Object) currentStore != (UnityEngine.Object) null)
      currentStore.Hide();
    this.HandlePurchaseSuccess(new StoreManager.PurchaseErrorSource?(), this.m_activeMoneyOrGTAPPTransaction, string.Empty, data);
    if (data == null)
      return;
    Network.Get().ReportBlizzardCheckoutStatus(BlizzardCheckoutStatus.BLIZZARD_CHECKOUT_STATUS_COMPLETED_SUCCESS, data);
    TelemetryManager.Client().SendBlizzardCheckoutPurchaseCompletedSuccess(data.TransactionID, data.ProductID.ToString(), data.CurrencyCode);
  }

  private void SendAttributionPurchaseMessage(TransactionData transactionData)
  {
    if (transactionData == null)
    {
      Log.Store.PrintWarning("[SendAttributionPurchaseMessage] No transaction data provided, skipping attribution message.");
    }
    else
    {
      if (transactionData.IsVCPurchase)
        return;
      AdTrackingManager service;
      if (!ServiceManager.TryGet<AdTrackingManager>(out service))
      {
        Log.Store.PrintWarning("[SendAttributionPurchaseMessage] AdTrackingManager unavailable, skipping attribution message.");
      }
      else
      {
        Network.Bundle fromPmtProductId = this.GetBundleFromPmtProductId(transactionData.ProductID);
        if ((Record) fromPmtProductId == (Record) null)
        {
          Log.Store.PrintWarning("[SendAttributionPurchaseMessage] Unable to find bundle for PMT Product ID {0}, skipping attribution message.", (object) transactionData.ProductID);
        }
        else
        {
          double? costDisplay = fromPmtProductId.CostDisplay;
          string currencyCode = transactionData.CurrencyCode;
          string transactionId = transactionData.TransactionID;
          string productId = StoreManager.GetExternalStoreProductId(fromPmtProductId) ?? fromPmtProductId.PMTProductID.ToString();
          service.TrackSale(costDisplay ?? 0.0, currencyCode, productId, transactionId);
        }
      }
    }
  }

  private bool IsCommerceUiShowing()
  {
    HearthstoneCheckout service;
    return ServiceManager.TryGet<HearthstoneCheckout>(out service) && service.IsUIShown;
  }

  public bool WillStoreDisplayNotice(
    NetCache.ProfileNotice.NoticeOrigin noticeOrigin,
    NetCache.ProfileNotice.NoticeType noticeType,
    long noticeOriginData)
  {
    GeneralStore currentStore = this.GetCurrentStore() as GeneralStore;
    if ((UnityEngine.Object) currentStore == (UnityEngine.Object) null)
      return false;
    GeneralStorePacksPane currentPane = currentStore.GetCurrentPane() as GeneralStorePacksPane;
    return !((UnityEngine.Object) currentPane == (UnityEngine.Object) null) && currentPane.WillStoreDisplayNotice(noticeOrigin, noticeType, noticeOriginData);
  }

  private string GetProductPrice(ProductId productId)
  {
    HearthstoneCheckout service;
    string price;
    return ServiceManager.TryGet<HearthstoneCheckout>(out service) && service.TryGetProductPrice(productId, out price) ? price : (string) null;
  }

  public bool HasCatalogNetworkPages() => this.m_catalogNetworkPages.HasPages();

  private enum PurchaseErrorSource
  {
    FROM_PURCHASE_METHOD_RESPONSE,
    FROM_STATUS_OR_PURCHASE_RESPONSE,
    FROM_PREVIOUS_PURCHASE,
  }

  private enum TransactionStatus
  {
    UNKNOWN,
    IN_PROGRESS_MONEY,
    IN_PROGRESS_GOLD_GTAPP,
    IN_PROGRESS_GOLD_NO_GTAPP,
    READY,
    WAIT_ZERO_COST_LICENSE,
    WAIT_METHOD_OF_PAYMENT,
    WAIT_CONFIRM,
    WAIT_RISK,
    CHALLENGE_SUBMITTED,
    CHALLENGE_CANCELED,
    USER_CANCELING,
    AUTO_CANCELING,
    IN_PROGRESS_BLIZZARD_CHECKOUT,
    WAIT_BLIZZARD_CHECKOUT,
  }

  private enum LicenseStatus
  {
    NOT_OWNED,
    OWNED,
    OWNED_AND_BLOCKING,
    UNDEFINED,
  }

  private struct ShowStoreData
  {
    public bool isTotallyFake;
    public Store.ExitCallback exitCallback;
    public object exitCallbackUserData;
    public ProductType storeProduct;
    public int storeProductData;
    public GeneralStoreMode storeMode;
    public int numItemsRequired;
    public bool useOverlayUI;
    public int pmtProductId;
    public bool closeOnTransactionComplete;
    public IDataModel dataModel;
  }
}
