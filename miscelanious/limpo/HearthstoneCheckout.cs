using Blizzard.Commerce;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using com.blizzard.commerce.Model;
using Hearthstone;
using Hearthstone.Commerce;
using Hearthstone.Core;
using Hearthstone.Core.Deeplinking;
using Hearthstone.Login;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HearthstoneCheckout : 
  blz_commerce_log_hook,
  ISceneEventObserver,
  IVirtualCurrencyEventObserver,
  IPurchaseEventObserver,
  ICatalogEventObserver,
  IHasUpdate,
  IService,
  IDeeplinkCallback
{
  private const string kTitleCode = "WTCG";
  private const string kHearthstoneCheckoutPrefab = "HearthstoneCheckout.prefab:da1b8fa18876ab5468bd2aa04a3f2539";
  private const int kInitializationRetryCount = 3;
  private const float kResolutionUpdateInterval = 1f;
  private const int kMaxAttempts = 10;
  private const string kCommerceSDKScheme = "blizzard";
  private const int kStoreId = 6;
  private const int kMaxProductLoadCount = 300;
  private PurchaseHandle m_purchaseHandle;
  private readonly Dictionary<ProductId, HearthstoneCheckout.ProductInfo> m_productMap = new Dictionary<ProductId, HearthstoneCheckout.ProductInfo>();
  private readonly Dictionary<(ProductId, PlatformId), string> m_externalIds = new Dictionary<(ProductId, PlatformId), string>();
  private HearthstoneCheckoutUI m_checkoutUI;
  private bool m_closeRequested;
  private TransactionData m_currentTransaction;
  private HearthstoneCheckout.State m_currentState;
  private Vector2 m_screenResolution;
  private float m_elapsedTimeSinceResolutionCheck;
  private float m_elapsedTimeSinceShown;
  private const float FAIL_WAIT_TIME = 0.5f;
  private const float MAX_FAIL_WAIT_TIME = 5f;
  private const float kInProgressBackgroundableDelay = 10f;
  private DateTime m_transactionStart = DateTime.Now;
  private int m_retriesRemaining = 3;
  private long[] m_productCatalog;
  private string m_currencyCode;
  private readonly List<HearthstoneCheckout.VirtualCurrencyRequest> m_virtualCurrencyRequests = new List<HearthstoneCheckout.VirtualCurrencyRequest>();
  private string m_clientID;
  private HearthstoneCheckout.ClientInitializationResponse m_clientInitializationResponse;
  private bool _shouldCallCSDKUpdate;
  private Queue<System.Action> _isOpenCallbacks = new Queue<System.Action>();
  private bool? m_overrideEndpointToProduction;
  private EventListenerObserverImpl commerceObserverImpl;
  private static bool m_receivedSdkProducts;
  private float m_loadProductsStartTime;
  private static string s_oneStoreKey;

  public static bool ReceivedSdkProducts => HearthstoneCheckout.m_receivedSdkProducts;

  public static string OneStoreKey
  {
    set
    {
    }
  }

  public HearthstoneCheckoutUI CheckoutUi => this.m_checkoutUI;

  public bool CheckoutIsReady { get; private set; }

  public bool IsSystemEnabled { get; private set; }

  public bool HasProductCatalog => this.m_productCatalog != null;

  public bool HasClientID => this.m_clientID != null;

  public bool HasCurrencyCode => this.m_currencyCode != null;

  public bool IsIdle => this.CurrentState == HearthstoneCheckout.State.Idle;

  public bool IsInProgress => this.CurrentState == HearthstoneCheckout.State.InProgress;

  public bool IsUnavailable => this.CurrentState == HearthstoneCheckout.State.Unavailable;

  private HearthstoneCheckout.State CurrentState => this.m_currentState;

  public bool IsUIShown => (UnityEngine.Object) this.m_checkoutUI != (UnityEngine.Object) null && this.m_checkoutUI.IsShown();

  public float ShownTime => this.m_elapsedTimeSinceShown;

  public bool ShouldBlockInput => this.IsUIShown;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    HearthstoneCheckout hearthstoneCheckout = this;
    serviceLocator.Get<Network>().RegisterNetHandler((object) InitialClientState.PacketID.ID, new Network.NetHandler(hearthstoneCheckout.OnInitialClientState));
    if (Vars.Key("Commerce.OverrideEndpointToProduction").HasValue)
      hearthstoneCheckout.m_overrideEndpointToProduction = new bool?(Vars.Key("Commerce.OverrideEndpointToProduction").GetBool(true));
    string libraryError = (string) null;
    string error = (string) null;
    DateTime lastTry = DateTime.UtcNow + TimeSpan.FromSeconds(5.0);
    do
    {
      error = (string) null;
      try
      {
        if (CommerceWrapper.Instance.IsUninit)
        {
          CommerceWrapper.Instance.Dispose();
          error = "Unknown";
        }
      }
      catch (Exception ex)
      {
        error = ex.Message;
      }
      if (error != null)
      {
        Log.Store.PrintWarning("[HearthstoneCheckout.Initialize] Create SDK error: (" + error + "). Retrying...");
        DateTime nextTry = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);
        while (DateTime.UtcNow < nextTry)
          yield return (IAsyncJobResult) null;
      }
    }
    while (!string.IsNullOrEmpty(error) && DateTime.UtcNow < lastTry);
    if (!string.IsNullOrEmpty(error))
    {
      hearthstoneCheckout.m_currentState = HearthstoneCheckout.State.Unavailable;
      Log.Store.PrintError("[HearthstoneCheckout.Initialize] Create SDK Total Failure: (" + error + ").");
      TelemetryManager.Client().SendBlizzardCheckoutInitializationResult(false, "Commerce SDK not Valid.", error);
      yield return (IAsyncJobResult) new JobFailedResult(error, Array.Empty<object>());
    }
    try
    {
      hearthstoneCheckout.commerceObserverImpl = new EventListenerObserverImpl((blz_commerce_log_hook) hearthstoneCheckout);
      hearthstoneCheckout.commerceObserverImpl.AddSceneObserver((ISceneEventObserver) hearthstoneCheckout);
      hearthstoneCheckout.commerceObserverImpl.AddCatalogObserver((ICatalogEventObserver) hearthstoneCheckout);
      hearthstoneCheckout.commerceObserverImpl.AddPurchaseObserver((IPurchaseEventObserver) hearthstoneCheckout);
      hearthstoneCheckout.commerceObserverImpl.AddVirtualCurrencyObserver((IVirtualCurrencyEventObserver) hearthstoneCheckout);
    }
    catch (Exception ex)
    {
      hearthstoneCheckout.m_currentState = HearthstoneCheckout.State.Unavailable;
      libraryError = string.Format("Failed to initialize HearthstoneCheckout: {0}", (object) ex);
      TelemetryManager.Client().SendBlizzardCheckoutInitializationResult(false, "Checkout Library Exception.", ex.ToString());
    }
    if (!string.IsNullOrEmpty(libraryError))
    {
      TelemetryManager.Client().SendBlizzardCheckoutInitializationResult(false, "Commerce SDK Interface error.", libraryError);
      yield return (IAsyncJobResult) new JobFailedResult(libraryError, Array.Empty<object>());
    }
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.WillReset += new System.Action(hearthstoneCheckout.OnReset);
    DeeplinkService service;
    if (ServiceManager.TryGet<DeeplinkService>(out service))
    {
      if (!service.RegisterDeeplink("blizzard", (IDeeplinkCallback) hearthstoneCheckout))
        Log.Store.PrintError("Unable register with deeplink service");
    }
    else
      Log.Store.PrintError("Deeplink service is not available");
    JobDefinition job = new JobDefinition("HearthstoneCheckout.LoadCheckoutUI", hearthstoneCheckout.Job_CreateCSDK(), JobFlags.StartImmediately, Array.Empty<IJobDependency>());
    Processor.QueueJob(job);
    Processor.QueueJob(new JobDefinition("HearthstoneCheckout.InitializeCheckoutClient", hearthstoneCheckout.Job_InitializeCheckoutClient(), new IJobDependency[2]
    {
      (IJobDependency) job.CreateDependency(),
      (IJobDependency) new WaitForCheckoutConfiguration()
    }));
  }

  public System.Type[] GetDependencies() => new System.Type[5]
  {
    typeof (Network),
    typeof (LoginManager),
    typeof (IAssetLoader),
    typeof (ILoginService),
    typeof (DeeplinkService)
  };

  public void Update()
  {
    if ((UnityEngine.Object) this.m_checkoutUI != (UnityEngine.Object) null && this.m_checkoutUI.HasCheckoutMesh)
    {
      this.m_elapsedTimeSinceResolutionCheck += Time.deltaTime;
      if ((double) this.m_elapsedTimeSinceResolutionCheck > 1.0)
      {
        this.ScreenResolutionUpdate();
        this.m_elapsedTimeSinceResolutionCheck = 0.0f;
      }
    }
    if (this.IsUIShown)
      this.m_elapsedTimeSinceShown += Time.deltaTime;
    if (this.m_closeRequested)
    {
      if (this.m_currentTransaction != null && !this.m_currentTransaction.IsVCPurchase && !CommerceWrapper.Instance.SendBrowserCloseEvent())
        Log.Store.PrintWarning("[HearthstoneCheckout.Update] SendBrowserCloseEvent failed");
      if (this.m_currentState != HearthstoneCheckout.State.InProgress && this.m_currentState != HearthstoneCheckout.State.InProgress_Backgroundable)
        this.ClearTransaction();
      else if (this.IsUIShown)
        this.m_checkoutUI.Hide();
      this.m_closeRequested = false;
      StoreManager.Get()?.HandleCommerceCloseEvent();
    }
    else
    {
      if (this._shouldCallCSDKUpdate && CommerceWrapper.Instance.IsIdle && !CommerceWrapper.Instance.Update())
        Log.Store.PrintWarning("[HearthstoneCheckout.Update] Update failed");
      if (this.m_currentState != HearthstoneCheckout.State.InProgress || (double) (DateTime.Now - this.m_transactionStart).Seconds < 10.0)
        return;
      this.m_currentState = HearthstoneCheckout.State.InProgress_Backgroundable;
    }
  }

  public IEnumerator<IAsyncJobResult> LoadProducts()
  {
    if (BattleNet.GetCurrentRegion() == BnetRegion.REGION_UNINITIALIZED)
    {
      Log.Store.PrintError("[HearthstoneCheckout.LoadProducts] Tried to load products without a valid region!");
      yield return (IAsyncJobResult) new JobFailedResult("Tried to load products without a valid region!", Array.Empty<object>());
    }
    List<string> currencyCodes = this.GetCurrencyCodes();
    bool ranSucceeded = false;
    this._shouldCallCSDKUpdate = false;
    yield return (IAsyncJobResult) new HearthstoneCheckout.WaitForIdle();
    this.m_loadProductsStartTime = Time.realtimeSinceStartup;
    int remainingRuns = 10;
    do
    {
      if (!CommerceWrapper.Instance.LoadProducts(currencyCodes, 6, 300))
      {
        Log.Store.PrintWarning("[HearthstoneCheckout.LoadProducts] LoadProducts failed");
        if (!CommerceWrapper.Instance.Update())
          Log.Store.PrintWarning("[HearthstoneCheckout.LoadProducts] Update failed");
      }
      else
      {
        ranSucceeded = true;
        remainingRuns = 0;
      }
      yield return (IAsyncJobResult) null;
      --remainingRuns;
    }
    while (remainingRuns > 0);
    this._shouldCallCSDKUpdate = true;
    if (!ranSucceeded)
      yield return (IAsyncJobResult) new JobFailedResult("LoadProducts failed", Array.Empty<object>());
  }

  public void Shutdown()
  {
    Log.Store.PrintDebug("[HearthstoneCheckout.Shutdown]");
    DeeplinkService service;
    if (ServiceManager.TryGet<DeeplinkService>(out service))
      service.RemoveDeeplinkCallback("blizzard");
    this.m_currentState = HearthstoneCheckout.State.Startup;
    this.m_currentTransaction = (TransactionData) null;
    this.m_productCatalog = (long[]) null;
    this._shouldCallCSDKUpdate = true;
    this.m_virtualCurrencyRequests.Clear();
    this._isOpenCallbacks.Clear();
    this.DestroyCheckoutUI();
    this.DisposeCurrentCheckoutClient();
    this.DisposeListeners();
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if (!((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null))
      return;
    hearthstoneApplication.WillReset -= new System.Action(this.OnReset);
  }

  public void ShowCheckout(ProductId productID, string currencyCode, uint quantity) => Processor.QueueJob(nameof (ShowCheckout), this.Job_ShowCheckout(productID, currencyCode, quantity)).AddJobFinishedEventListener(new JobDefinition.JobFinishedEventListener(this.OnPurchaseJobFinished));

  public void PurchaseWithVirtualCurrency(ProductId productID, string currencyCode, uint quantity) => Processor.QueueJob(nameof (PurchaseWithVirtualCurrency), this.Job_PurchaseWithVirtualCurrency(productID, currencyCode, quantity)).AddJobFinishedEventListener(new JobDefinition.JobFinishedEventListener(this.OnPurchaseJobFinished));

  public bool RegisterReadyCallback(System.Action callback)
  {
    if (this.IsAvailable())
    {
      callback();
      return false;
    }
    this._isOpenCallbacks.Enqueue(callback);
    return true;
  }

  public IEnumerator GetVirtualCurrencyBalance(
    string currencyCode,
    HearthstoneCheckout.VirtualCurrencyBalanceCallback callback,
    System.Action<bool> resultCallback)
  {
    bool successful = false;
    if (!CommerceWrapper.Instance.IsValid)
      Log.Store.PrintError("[HearthstoneCheckout.GetVirtualCurrencyBalance] Cannot get virtual currency balance because the checkout client isn't initialized.");
    this._shouldCallCSDKUpdate = false;
    int waitCount = 100;
    while (waitCount > 0)
    {
      if (!CommerceWrapper.Instance.IsIdle)
        waitCount = 0;
      --waitCount;
      yield return (object) null;
    }
    if (CommerceWrapper.Instance.GetVCBalance(currencyCode))
    {
      this.m_virtualCurrencyRequests.Add(new HearthstoneCheckout.VirtualCurrencyRequest(currencyCode, callback));
      successful = true;
    }
    this._shouldCallCSDKUpdate = true;
    if (!successful)
      Log.Store.PrintWarning("[HearthstoneCheckout.GetVirtualCurrencyBalance] GetVCBalance failed");
    resultCallback(successful);
  }

  public bool TryGetProductTitle(ProductId productId, out string title)
  {
    HearthstoneCheckout.ProductInfo productInfo;
    if (!this.TryGetProductInfo(productId, out productInfo))
    {
      title = string.Empty;
      return false;
    }
    title = productInfo.title;
    return true;
  }

  public bool TryGetProductDescription(ProductId productId, out string description)
  {
    HearthstoneCheckout.ProductInfo productInfo;
    if (!this.TryGetProductInfo(productId, out productInfo))
    {
      description = string.Empty;
      return false;
    }
    description = productInfo.description;
    return true;
  }

  public bool TryGetProductPrice(ProductId productId, out string price)
  {
    HearthstoneCheckout.ProductInfo productInfo;
    if (!this.TryGetProductInfo(productId, out productInfo) || string.IsNullOrEmpty(productInfo.price))
    {
      price = string.Empty;
      return false;
    }
    price = productInfo.price;
    return true;
  }

  public bool IsProductValid(ProductId productId) => this.m_productMap.ContainsKey(productId);

  public bool IsAvailable() => this.CurrentState == HearthstoneCheckout.State.Idle || this.CurrentState == HearthstoneCheckout.State.Ready || this.CurrentState == HearthstoneCheckout.State.InProgress || this.CurrentState == HearthstoneCheckout.State.InProgress_Backgroundable || this.CurrentState == HearthstoneCheckout.State.Finished;

  public bool IsClientCreationInProgress() => this.CurrentState == HearthstoneCheckout.State.Startup;

  public IEnumerable<ProductId> FindMissingProducts(
    IEnumerable<ProductId> products)
  {
    return this.m_productMap.Keys.Where<ProductId>((Func<ProductId, bool>) (productId => !products.Contains<ProductId>(productId)));
  }

  public IEnumerator<IAsyncJobResult> GetPersonalizedShopData(
    string[] pageId,
    HearthstoneCheckout.PersonalizedShopResponseCallback callback)
  {
    if (callback == null)
    {
      Log.Store.PrintError("[HearthstoneCheckout.GetPersonalizedShopData] Callback cannot be null.");
      yield return (IAsyncJobResult) new JobFailedResult("Callback cannot be null.", Array.Empty<object>());
    }
    if (!Network.IsLoggedIn())
    {
      Log.Store.PrintError("[HearthstoneCheckout.GetPersonalizedShopData] Cannot get personalized shop data because the user is off-line.");
      yield return (IAsyncJobResult) new JobFailedResult("Cannot get personalized shop data because the user is off-line.", Array.Empty<object>());
    }
    this._shouldCallCSDKUpdate = false;
    int runCount = 100;
    while (runCount > 0)
    {
      if (!CommerceWrapper.Instance.IsIdle)
        runCount = 0;
      --runCount;
      yield return (IAsyncJobResult) null;
    }
    if (!CommerceWrapper.Instance.LoadPersonalizedShop(((IEnumerable<string>) pageId).ToList<string>()))
    {
      this._shouldCallCSDKUpdate = true;
      Log.Store.PrintError("[HearthstoneCheckout.GetPersonalizedShopData] LoadPersonalizedShop failed");
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.GetPersonalizedShopData] LoadPersonalizedShop failed.", Array.Empty<object>());
    }
    this._shouldCallCSDKUpdate = true;
  }

  public void RequestClose()
  {
    switch (this.m_currentState)
    {
      case HearthstoneCheckout.State.Ready:
        ((ISceneEventObserver) this).OnCancel();
        break;
      case HearthstoneCheckout.State.Idle:
        Log.Store.PrintWarning("[HearthstoneCheckout.RequestClose] HearthstoneCheckout received a request close when it should already be closed.  Attempting to close again...");
        this.SignalCloseNextFrame();
        break;
      case HearthstoneCheckout.State.InProgress_Backgroundable:
      case HearthstoneCheckout.State.Finished:
        this.SignalCloseNextFrame();
        break;
    }
  }

  public void CancelCurrentTransaction()
  {
    string transactionId = this.m_currentTransaction?.TransactionID;
    if (string.IsNullOrEmpty(transactionId))
      return;
    CommerceWrapper.Instance.CancelPurchase(transactionId);
    if (this.IsIdle)
      return;
    ((IPurchaseEventObserver) this).OnCancel(this.m_currentTransaction);
  }

  public void SetProductCatalog(long[] productCatalog) => this.m_productCatalog = productCatalog;

  public void SetClientID(string clientID) => this.m_clientID = clientID;

  public void SetCurrencyCode(string currencyCode) => this.m_currencyCode = currencyCode;

  private List<string> GetCurrencyCodes()
  {
    if (!(this.m_currencyCode == "CPT"))
      return (List<string>) null;
    return new List<string>() { "XSA", "XSB" };
  }

  private bool TryGetProductInfo(
    ProductId productId,
    out HearthstoneCheckout.ProductInfo productInfo)
  {
    if (!this.m_productMap.ContainsKey(productId))
    {
      productInfo = new HearthstoneCheckout.ProductInfo((string) null, (string) null, (string) null);
      return false;
    }
    productInfo = this.m_productMap[productId];
    return true;
  }

  private void OnReset()
  {
    this.IsSystemEnabled = false;
    this.m_currentState = HearthstoneCheckout.State.Startup;
    this.m_productCatalog = (long[]) null;
    this.m_clientID = (string) null;
    this._shouldCallCSDKUpdate = false;
    this._isOpenCallbacks.Clear();
    this.m_virtualCurrencyRequests.Clear();
    this.DestroyCheckoutUI();
    this.DisposeCurrentCheckoutClient();
    JobDefinition job = new JobDefinition("HearthstoneCheckout.LoadCheckoutUI", this.Job_CreateCSDK(), JobFlags.StartImmediately, new IJobDependency[1]
    {
      (IJobDependency) new WaitForGameDownloadManagerState()
    });
    Processor.QueueJob(job);
    Processor.QueueJob(new JobDefinition("HearthstoneCheckout.InitializeCheckoutClient", this.Job_InitializeCheckoutClient(), new IJobDependency[2]
    {
      (IJobDependency) job.CreateDependency(),
      (IJobDependency) new WaitForCheckoutConfiguration()
    }));
  }

  private void OnInitialClientState()
  {
    InitialClientState initialClientState = Network.Get().GetInitialClientState();
    if (initialClientState == null || !initialClientState.HasGuardianVars)
      return;
    this.IsSystemEnabled = initialClientState.GuardianVars.ProductsFromCommerceEnabled;
  }

  private string GetTitleVersionString() => string.Format("{0}.{1}.{2}-{3}", (object) "25.0", (object) "0", (object) 158725, (object) HearthstoneCheckout.GetPlatformString());

  private static string GetPlatformString()
  {
    switch (PlatformSettings.RuntimeOS)
    {
      case OSCategory.PC:
        return "Windows";
      case OSCategory.Mac:
        return "MacOS";
      case OSCategory.iOS:
        return HearthstoneCheckout.GetIOSPlatformString();
      case OSCategory.Android:
        return HearthstoneCheckout.GetAndroidPlatformString();
      default:
        return "UnknownOS";
    }
  }

  private static string GetAndroidPlatformString()
  {
    switch (AndroidDeviceSettings.Get().GetAndroidStore())
    {
      case AndroidStore.BLIZZARD:
        return "AndroidBattlenet";
      case AndroidStore.GOOGLE:
        return "Google";
      case AndroidStore.AMAZON:
        return "Amazon";
      case AndroidStore.HUAWEI:
        return "Huawei";
      default:
        return "UnkownAndroid";
    }
  }

  private static string GetIOSPlatformString() => PlatformSettings.LocaleVariant != LocaleVariant.China ? "iOS" : "iOSCN";

  private string GenerateExternalTransactionID()
  {
    if (!CommerceWrapper.Instance.IsValid)
    {
      Log.Store.PrintError("[HearthstoneCheckout.GenerateExternalTransactionID] Checkout Client must exists to generate an external transaction ID.");
      return (string) null;
    }
    BnetRegion region = BattleNet.GetAccountRegion();
    switch (region)
    {
      case BnetRegion.REGION_US:
      case BnetRegion.REGION_EU:
      case BnetRegion.REGION_KR:
      case BnetRegion.REGION_TW:
      case BnetRegion.REGION_CN:
        return CommerceWrapper.Instance.GenerateTransactionID(14, (int) region);
      default:
        region = BnetRegion.REGION_PTR;
        goto case BnetRegion.REGION_US;
    }
  }

  private static string GetBrowserPath()
  {
    if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
      return Application.dataPath;
    CommerceConfig commerceConfig = CommerceConfig.RetrieveConfig();
    if (commerceConfig != null)
      return commerceConfig.RuntimeDataPath;
    Debug.LogErrorFormat("Could not retrieve the commerce config file! We can not load the checkout window appropriately and sales will not be possible.");
    return Application.dataPath;
  }

  private void ScreenResolutionUpdate()
  {
    if (!((UnityEngine.Object) this.m_checkoutUI != (UnityEngine.Object) null) || !this.m_checkoutUI.IsShown() || (double) this.m_screenResolution.x == (double) Screen.width && (double) this.m_screenResolution.y == (double) Screen.height)
      return;
    this.m_checkoutUI.DetermineBrowserSize();
    Log.Store.PrintDebug("Browser Width: {0}\nBrowser Height: {1}", (object) this.m_checkoutUI.BrowserWidth, (object) this.m_checkoutUI.BrowserHeight);
    if (!CommerceWrapper.Instance.SendResizeEvent(this.m_checkoutUI.BrowserWidth, this.m_checkoutUI.BrowserHeight))
      Log.Store.PrintWarning("[HearthstoneCheckout.ScreenResolutionUpdate] Unable to send resize event");
    this.m_screenResolution.x = (float) Screen.width;
    this.m_screenResolution.y = (float) Screen.height;
  }

  private void UpdateTransactionData(TransactionData response)
  {
    if (!this.IsTransactionInProgress())
    {
      Log.Store.PrintDebug("Cannot perform a new transation while current in progress");
    }
    else
    {
      if (response == null || response.ErrorCodes == null)
        return;
      this.m_currentTransaction.ErrorCodes = response.ErrorCodes;
    }
  }

  private void LogPurchaseResponse(string tag, TransactionData data)
  {
    Log.Store.PrintDebug("{0} Status - {1}", (object) tag, (object) data.Status);
    if (string.IsNullOrEmpty(data.ErrorCodes))
      return;
    Log.Store.PrintError("[HearthstoneCheckout] CHECKOUT ERROR: {0}", (object) data.ErrorCodes);
  }

  private void SignalCloseNextFrame() => this.m_closeRequested = true;

  private void ClearTransaction()
  {
    if (this.IsUIShown)
      this.m_checkoutUI.Hide();
    this.m_closeRequested = false;
    this.m_currentTransaction = (TransactionData) null;
    if (this.m_currentState == HearthstoneCheckout.State.Unavailable)
      return;
    this.m_currentState = HearthstoneCheckout.State.Idle;
    this._shouldCallCSDKUpdate = true;
  }

  private void DestroyCheckoutUI()
  {
    if (!((UnityEngine.Object) this.m_checkoutUI != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_checkoutUI.gameObject != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_checkoutUI.gameObject);
    this.m_checkoutUI = (HearthstoneCheckoutUI) null;
  }

  private void OnTransactionProcessCompleted()
  {
    if (this.IsUIShown)
      return;
    this.SignalCloseNextFrame();
  }

  private void OnPurchaseJobFinished(JobDefinition job, bool success)
  {
    if (success || this.m_currentState == HearthstoneCheckout.State.InProgress || this.m_currentState == HearthstoneCheckout.State.InProgress_Backgroundable || this.m_currentState == HearthstoneCheckout.State.Finished)
      return;
    StoreManager.Get()?.HandleCommerceSubmitFailure();
    this.RequestClose();
  }

  private void OnOutsideClick()
  {
    if (!StoreManager.Get().CanTapOutConfirmationUI())
      return;
    this.RequestClose();
  }

  private void DisposeCurrentCheckoutClient()
  {
    if (this.m_purchaseHandle == null)
      return;
    this.m_purchaseHandle.Dispose();
    this.m_purchaseHandle = (PurchaseHandle) null;
  }

  private void DisposeListeners()
  {
    this.commerceObserverImpl.RemoveSceneObserver((ISceneEventObserver) this);
    this.commerceObserverImpl.RemoveCatalogObserver((ICatalogEventObserver) this);
    this.commerceObserverImpl.RemovePurchaseObserver((IPurchaseEventObserver) this);
    this.commerceObserverImpl.RemoveVirtualCurrencyObserver((IVirtualCurrencyEventObserver) this);
    CommerceWrapper.Instance.Dispose();
  }

  private bool IsTransactionInProgress()
  {
    if (this.m_currentTransaction == null)
      return false;
    return this.m_currentState == HearthstoneCheckout.State.Ready || this.m_currentState == HearthstoneCheckout.State.InProgress || this.m_currentState == HearthstoneCheckout.State.InProgress_Backgroundable;
  }

  private IEnumerator<IAsyncJobResult> Job_ShowCheckout(
    ProductId productID,
    string currencyCode,
    uint quantity)
  {
    HearthstoneCheckout checkoutClient = this;
    if (!CommerceWrapper.Instance.IsValid)
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.ShowCheckout] Cannot show checkout because the checkout client isn't available.", Array.Empty<object>());
    if ((UnityEngine.Object) checkoutClient.m_checkoutUI == (UnityEngine.Object) null)
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.ShowCheckout] Cannot show checkout because the UI isn't loaded.", Array.Empty<object>());
    Log.Store.PrintDebug("[HearthstoneCheckout.ShowCheckout] Started");
    checkoutClient.m_elapsedTimeSinceResolutionCheck = 0.0f;
    checkoutClient.m_elapsedTimeSinceShown = 0.0f;
    checkoutClient.m_checkoutUI.GenerateMeshes();
    yield return (IAsyncJobResult) new WaitForLogin();
    GenerateSSOToken generateToken = new GenerateSSOToken();
    yield return (IAsyncJobResult) generateToken;
    checkoutClient.m_currentTransaction = new TransactionData(productID, currencyCode, quantity, false)
    {
      TransactionID = checkoutClient.GenerateExternalTransactionID()
    };
    checkoutClient._shouldCallCSDKUpdate = false;
    yield return (IAsyncJobResult) new HearthstoneCheckout.WaitForIdle();
    checkoutClient.m_currentState = HearthstoneCheckout.State.InProgress;
    bool flowDisableOverride = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().BattlenetBillingFlowDisableOverride;
    checkoutClient.m_purchaseHandle = CommerceWrapper.Instance.PurchaseCheckout(checkoutClient.m_currentTransaction.ProductID, checkoutClient.m_currentTransaction.CurrencyCode, generateToken.Token, checkoutClient.m_currentTransaction.TransactionID, flowDisableOverride);
    checkoutClient._shouldCallCSDKUpdate = true;
    if (checkoutClient.m_purchaseHandle == null)
    {
      Log.Store.PrintError("[HearthstoneCheckout.ShowCheckout] Failed to obtain purchase handle.");
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.ShowCheckout] Failed to obtain purchase handle.", Array.Empty<object>());
    }
    Log.Store.PrintDebug("[HearthstoneCheckout.ShowCheckout]Purchase was successfully initiated.");
    checkoutClient.m_checkoutUI.InitiateCheckout(checkoutClient);
  }

  private IEnumerator<IAsyncJobResult> Job_PurchaseWithVirtualCurrency(
    ProductId productID,
    string currencyCode,
    uint quantity)
  {
    HearthstoneCheckout hearthstoneCheckout = this;
    if (!CommerceWrapper.Instance.HasLoadedCatalog)
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.PurchaseWithVirtualCurrency] Cannot initiate purchase because catalog has not loaded.", Array.Empty<object>());
    GenerateSSOToken generateSSOToken = new GenerateSSOToken();
    yield return (IAsyncJobResult) generateSSOToken;
    if (!generateSSOToken.HasToken)
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.PurchaseWithVirtualCurrency] Cannot show checkout because it didn't receive an SSO token.", Array.Empty<object>());
    yield return (IAsyncJobResult) new WaitForLogin();
    hearthstoneCheckout.m_currentState = HearthstoneCheckout.State.InProgress;
    hearthstoneCheckout.m_currentTransaction = new TransactionData(productID, currencyCode, quantity, true)
    {
      TransactionID = hearthstoneCheckout.GenerateExternalTransactionID()
    };
    hearthstoneCheckout._shouldCallCSDKUpdate = false;
    yield return (IAsyncJobResult) new HearthstoneCheckout.WaitForIdle();
    if (!CommerceWrapper.Instance.PurchaseWithVC(hearthstoneCheckout.m_currentTransaction.CurrencyCode, hearthstoneCheckout.m_currentTransaction.TransactionID, (int) hearthstoneCheckout.m_currentTransaction.ProductID.Value, (int) hearthstoneCheckout.m_currentTransaction.Quantity, 1465140039U))
    {
      hearthstoneCheckout._shouldCallCSDKUpdate = true;
      Log.Store.PrintWarning("[HearthstoneCheckout.Job_PurchaseWithVirtualCurrency] PurchaseVC failed.");
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.PurchaseWithVirtualCurrency] Purchase with VC failed from CSDK", Array.Empty<object>());
    }
    hearthstoneCheckout._shouldCallCSDKUpdate = true;
  }

  private IEnumerator<IAsyncJobResult> Job_CreateCSDK()
  {
    HearthstoneCheckout logger = this;
    if ((UnityEngine.Object) logger.m_checkoutUI != (UnityEngine.Object) null)
    {
      Log.Store.PrintError("[HearthstoneCheckout.Job_CreateCSDK] Checkout UI already exists!  Please destroy the existing UI before creating a new one.");
    }
    else
    {
      while (!Network.IsLoggedIn())
        yield return (IAsyncJobResult) null;
      InstantiatePrefab loadCheckoutUI = new InstantiatePrefab((AssetReference) "HearthstoneCheckout.prefab:da1b8fa18876ab5468bd2aa04a3f2539");
      yield return (IAsyncJobResult) loadCheckoutUI;
      logger.m_checkoutUI = loadCheckoutUI.InstantiatedPrefab.GetComponent<HearthstoneCheckoutUI>();
      loadCheckoutUI.InstantiatedPrefab.AddComponent<HSDontDestroyOnLoad>();
      logger.m_checkoutUI.Hide();
      logger.m_checkoutUI.DetermineBrowserSize();
      while (string.IsNullOrEmpty(logger.m_clientID))
        yield return (IAsyncJobResult) null;
      GenerateSSOToken generateSSOToken = new GenerateSSOToken();
      yield return (IAsyncJobResult) generateSSOToken;
      if (!generateSSOToken.HasToken)
      {
        TelemetryManager.Client().SendBlizzardCheckoutInitializationResult(false, "CommerceWrapper.InitListener Failed", "SSO Token failed");
        yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.Job_CreateCSDK] Cannot show checkout because it didn't receive an SSO token.", Array.Empty<object>());
      }
      logger.m_checkoutUI.AddOutsideClickListener(new HearthstoneCheckoutUI.OutsideClickListener(logger.OnOutsideClick));
      Vec2D browserSize = new Vec2D(logger.CheckoutUi.BrowserWidth, logger.CheckoutUi.BrowserHeight);
      CommerceWrapper.Instance.VerboseLogging = false;
      CommerceWrapper.Instance.Runner = new CommerceWrapper.AsyncRunner(Processor.RunCoroutine);
      RequestedModules modules = (RequestedModules) (28 | 3);
      AccountInitializationValues accountInitializationValues = new AccountInitializationValues()
      {
        Region = (int) BattleNet.GetCurrentRegion(),
        Locale = Localization.GetBnetLocaleName(),
        AccountId = string.Empty
      };
      SystemInitializationValues systemInitializationValue = new SystemInitializationValues()
      {
        TitleCode = "WTCG",
        TitleVersion = logger.GetTitleVersionString(),
        ClientId = logger.m_clientID,
        IsProduction = HearthstoneApplication.IsPublic(),
        OverrideProduction = ((int) logger.m_overrideEndpointToProduction ?? (HearthstoneApplication.IsPublic() ? 1 : 0)) != 0,
        DeviceId = SystemInfo.deviceUniqueIdentifier,
        BrowserPath = HearthstoneCheckout.GetBrowserPath(),
        CheckoutURL = string.Format("https://nydus-qa.web.blizzard.net/Bnet/{0}/client/checkout", (object) Localization.GetLocaleName()),
        MaxBrowserSize = new Vec2D(Screen.width, Screen.height),
        LogDir = Log.LogsPath,
        IsLegacyStyle = true
      };
      if (!CommerceWrapper.Instance.InitListener((EventListenerObserver) logger.commerceObserverImpl, (ITokenManager) generateSSOToken, modules, systemInitializationValue, accountInitializationValues, browserSize, (blz_commerce_log_hook) logger))
      {
        TelemetryManager.Client().SendBlizzardCheckoutInitializationResult(false, "CommerceWrapper.InitListener Failed", "CSDK error");
        Log.Store.PrintError("[HearthstoneCheckout.Job_CreateCSDK]: CommerceWrapper.InitListener Failed");
        yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.Job_CreateCSDK] The commerce SDK failed to initialize internally!", Array.Empty<object>());
      }
      Log.Store.PrintDebug("[HearthstoneCheckout.Job_CreateCSDK] CSDK is now Ready");
      logger.m_currentState = HearthstoneCheckout.State.Ready;
      logger.m_clientInitializationResponse = HearthstoneCheckout.ClientInitializationResponse.Success;
      logger.m_checkoutUI?.HandleCommerceReadyEvent();
      TelemetryManager.Client().SendBlizzardCheckoutIsReady((double) logger.ShownTime, true);
      while (!StoreManager.Get().BattlePayAvailable)
        yield return (IAsyncJobResult) null;
      logger._shouldCallCSDKUpdate = true;
      StoreManager.Get().QueueGetPersonalizedShopJobs();
    }
  }

  private IEnumerator<IAsyncJobResult> Job_CreateCheckoutClient()
  {
    if (this.m_currentState != HearthstoneCheckout.State.Startup)
      Log.Store.PrintDebug("[HearthstoneCheckout.CreateCheckoutClient] Initialize");
    else
      Log.Store.PrintDebug("[HearthstoneCheckout.CreateCheckoutClient] Reinit");
    this.m_currentState = HearthstoneCheckout.State.Startup;
    GenerateSSOToken generateSSOToken = new GenerateSSOToken();
    yield return (IAsyncJobResult) generateSSOToken;
    if (!generateSSOToken.HasToken)
    {
      this.m_clientInitializationResponse = HearthstoneCheckout.ClientInitializationResponse.Fail;
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.CreateCheckoutClient] Didn't receive a SSO token from request.", Array.Empty<object>());
    }
    yield return (IAsyncJobResult) new WaitForLogin();
    if (this.m_clientInitializationResponse == HearthstoneCheckout.ClientInitializationResponse.Fail)
      this.m_clientInitializationResponse = HearthstoneCheckout.ClientInitializationResponse.Waiting;
    Log.Store.PrintDebug("[HearthstoneCheckout.CreateCheckoutClient] Scene Checkout was successfully created.");
  }

  private IEnumerator<IAsyncJobResult> Job_InitializeCheckoutClient()
  {
    HearthstoneCheckout hearthstoneCheckout1 = this;
    hearthstoneCheckout1.m_retriesRemaining = 3;
    bool success = false;
    while (!success)
    {
      HearthstoneCheckout hearthstoneCheckout2 = hearthstoneCheckout1;
      int retriesRemaining = hearthstoneCheckout1.m_retriesRemaining;
      int num = retriesRemaining - 1;
      hearthstoneCheckout2.m_retriesRemaining = num;
      if (retriesRemaining > 0)
      {
        Log.Store.PrintDebug("[HearthstoneCheckout.InitializeCheckoutClient] Creating client");
        hearthstoneCheckout1.m_currentState = HearthstoneCheckout.State.Startup;
        yield return (IAsyncJobResult) new JobDefinition("HearthstoneCheckout.CreateCheckoutClient", hearthstoneCheckout1.Job_CreateCheckoutClient(), new IJobDependency[1]
        {
          (IJobDependency) new WaitForLogin()
        });
        Log.Store.PrintDebug("[HearthstoneCheckout.InitializeCheckoutClient] Client response: {0}", (object) hearthstoneCheckout1.m_clientInitializationResponse);
        if (hearthstoneCheckout1.m_clientInitializationResponse == HearthstoneCheckout.ClientInitializationResponse.Waiting)
          yield return (IAsyncJobResult) new HearthstoneCheckout.WaitForClientInitializationResponse(hearthstoneCheckout1, 60f);
        switch (hearthstoneCheckout1.m_clientInitializationResponse)
        {
          case HearthstoneCheckout.ClientInitializationResponse.Waiting:
            Log.Store.PrintError("[HearthstoneCheckout.InitializeCheckoutClient] Client timed out");
            TelemetryManager.Client().SendBlizzardCheckoutInitializationResult(false, "Checkout Client Initialization Timeout", string.Format("Attempt {0} of {1}", (object) (3 - hearthstoneCheckout1.m_retriesRemaining), (object) 3));
            continue;
          case HearthstoneCheckout.ClientInitializationResponse.Success:
            Log.Store.PrintDebug("[HearthstoneCheckout.InitializeCheckoutClient] Client initialized");
            TelemetryManager.Client().SendBlizzardCheckoutInitializationResult(true, "", "");
            success = true;
            continue;
          case HearthstoneCheckout.ClientInitializationResponse.Fail:
            Log.Store.PrintError("[HearthstoneCheckout.InitializeCheckoutClient] Client failed");
            TelemetryManager.Client().SendBlizzardCheckoutInitializationResult(false, "Checkout Client Initialization Unsuccessful", "");
            continue;
          default:
            Log.Store.PrintError("[HearthstoneCheckout.InitializeCheckoutClient] Unrecognized initialization response: {0}", (object) hearthstoneCheckout1.m_clientInitializationResponse);
            continue;
        }
      }
      else
        break;
    }
    if (success)
    {
      hearthstoneCheckout1.m_currentState = HearthstoneCheckout.State.Idle;
    }
    else
    {
      hearthstoneCheckout1.m_currentState = HearthstoneCheckout.State.Unavailable;
      yield return (IAsyncJobResult) new JobFailedResult("[HearthstoneCheckout.InitializeCheckoutClient] Failed to initialize checkout client.", Array.Empty<object>());
    }
  }

  void ISceneEventObserver.OnReady()
  {
    Log.Store.PrintDebug("[HearthstoneCheckout.OnReady] Showing checkout UI");
    this.CheckoutIsReady = true;
    this.m_checkoutUI.Show();
  }

  void ISceneEventObserver.OnDisconnect()
  {
    Log.Store.PrintDebug("[HearthstoneCheckout.OnDisconnect]");
    this.SignalCloseNextFrame();
  }

  void ISceneEventObserver.OnCancel()
  {
    Log.Store.PrintDebug("[HearthstoneCheckout.OnCancel]");
    this.SignalCloseNextFrame();
    this.m_currentState = HearthstoneCheckout.State.Finished;
    StoreManager.Get()?.HandleCommerceCancelEvent();
    TelemetryManager.Client().SendBlizzardCheckoutIsReady((double) this.ShownTime, false);
  }

  void ISceneEventObserver.OnWindowResize(int sizeX, int sizeY)
  {
    Log.Store.PrintDebug("[HearthstoneCheckout.OnWindowResized] (x:{0}, y:{1})", (object) sizeX, (object) sizeY);
    if (!((UnityEngine.Object) this.m_checkoutUI != (UnityEngine.Object) null))
      return;
    this.m_checkoutUI.ResizeTexture(sizeX, sizeY);
  }

  void ISceneEventObserver.OnBufferUpdate(byte[] data)
  {
    Log.Store.PrintDebug("[HearthstoneCheckout.OnBufferUpdate]");
    if (!((UnityEngine.Object) this.m_checkoutUI != (UnityEngine.Object) null))
      return;
    this.m_checkoutUI.UpdateTexture(data);
  }

  void ISceneEventObserver.OnWindowResizeRequested(int requestX, int requestY) => Log.Store.PrintDebug("[HearthstoneCheckout.OnWindowResizeRequested] Requested Size (x: {0}, y:{1})", (object) requestX, (object) requestY);

  void ISceneEventObserver.OnWindowCloseRequest()
  {
    Log.Store.PrintDebug("[HearthstoneCheckout.OnWindowCloseRequested]");
    this.SignalCloseNextFrame();
  }

  void ISceneEventObserver.OnCursorChanged()
  {
  }

  void ISceneEventObserver.OnExternalLink(string url)
  {
    Log.Store.PrintDebug("[HearthstoneCheckout.OnExternalLink] URL: {0}", (object) url);
    Application.OpenURL(url);
  }

  void ISceneEventObserver.OnImeCompsoitionRangeChanged(int from, int to)
  {
  }

  void ISceneEventObserver.OnImeStateChanged()
  {
  }

  void ISceneEventObserver.OnImeCompositionCanceled()
  {
  }

  void ISceneEventObserver.OnImeTextSelectionChanged(
    string text,
    int offset,
    int from,
    int to)
  {
  }

  void ISceneEventObserver.OnImeTextBoundsChanged(bool isAnchorRect, Rect2D rect)
  {
  }

  void IPurchaseEventObserver.OnCancel(TransactionData data)
  {
    Log.Store.PrintInfo("[HearthstoneCheckout.OnPurchaseCanceledBeforeSubmit]");
    ((ISceneEventObserver) this).OnCancel();
    StoreManager.Get()?.HandleCommerceCancelEvent();
    TelemetryManager.Client().SendBlizzardCheckoutIsReady((double) this.ShownTime, false);
  }

  void IPurchaseEventObserver.OnFailure(TransactionData data)
  {
    if (!this.IsTransactionInProgress())
    {
      this.LogPurchaseResponse("[HearthstoneCheckout.OnOrderFailure: Canceled Before Response]", data);
    }
    else
    {
      this.LogPurchaseResponse("[HearthstoneCheckout.OnOrderFailure]", data);
      this.m_currentState = HearthstoneCheckout.State.Finished;
      this.UpdateTransactionData(data);
      StoreManager.Get()?.HandleCommerceOrderFailure(this.m_currentTransaction);
      if ((UnityEngine.Object) this.m_checkoutUI != (UnityEngine.Object) null && this.m_checkoutUI.IsShown())
        this.m_checkoutUI.Hide();
      this.OnTransactionProcessCompleted();
    }
  }

  void IPurchaseEventObserver.OnSuccessful(TransactionData data)
  {
    if (!this.IsTransactionInProgress())
    {
      this.LogPurchaseResponse("[HearthstoneCheckout.OnOrderComplete: Canceled Before Response]", data);
    }
    else
    {
      this.LogPurchaseResponse("[HearthstoneCheckout.OnOrderComplete]", data);
      this.m_currentState = HearthstoneCheckout.State.Finished;
      this.UpdateTransactionData(data);
      StoreManager.Get()?.HandleCommerceOrderComplete(this.m_currentTransaction);
      if ((UnityEngine.Object) this.m_checkoutUI != (UnityEngine.Object) null && this.m_checkoutUI.IsShown())
        this.m_checkoutUI.Hide();
      this.OnTransactionProcessCompleted();
    }
  }

  void IPurchaseEventObserver.OnPending(
    TransactionData data,
    bool isCancelable)
  {
    if (!this.IsTransactionInProgress())
    {
      this.LogPurchaseResponse("[HearthstoneCheckout.OnOrderPending: Canceled Before Response]", data);
    }
    else
    {
      this.LogPurchaseResponse("[HearthstoneCheckout.OnOrderPending]", data);
      if (!isCancelable)
        this.m_currentState = HearthstoneCheckout.State.InProgress;
      this.m_transactionStart = DateTime.Now;
      this.UpdateTransactionData(data);
      StoreManager.Get()?.HandleCommerceOrderPending(this.m_currentTransaction);
    }
  }

  void IVirtualCurrencyEventObserver.OnPurchaseEvent(
    bool isError,
    Blizzard.Commerce.State state,
    string errorCode)
  {
    HearthstoneCheckout.State currentState = this.m_currentState;
    if (isError)
    {
      this.m_currentState = HearthstoneCheckout.State.Finished;
      Log.Store.PrintError("[HearthstoneCheckout.OnVirtualCurrencyResponse] Http error occurred: {0}", (object) errorCode);
      StoreManager.Get()?.HandleCommerceOrderFailure(this.m_currentTransaction);
    }
    else
    {
      HearthstoneCheckout.State state1;
      switch (state)
      {
        case Blizzard.Commerce.State.InProgress:
          state1 = HearthstoneCheckout.State.InProgress;
          break;
        case Blizzard.Commerce.State.Finished:
          state1 = HearthstoneCheckout.State.Finished;
          break;
        default:
          isError = true;
          state1 = HearthstoneCheckout.State.Finished;
          this.m_currentTransaction.ErrorCodes = errorCode;
          Log.Store.PrintError("[HearthstoneCheckout.OnVirtualCurrencyResponse] OrderWithVCRequest failed: {0}", (object) errorCode);
          break;
      }
      if (!this.IsTransactionInProgress())
      {
        Log.Store.PrintDebug("[HearthstoneCheckout.OnVirtualCurrencyResponse: Canceled Before Response] Status: {0}, Response: {1}", (object) state, (object) errorCode);
      }
      else
      {
        if (this.m_currentState == state1)
          return;
        this.m_currentState = state1;
        if (this.m_currentState != HearthstoneCheckout.State.Finished)
          return;
        if (!isError)
          StoreManager.Get()?.HandleCommerceOrderComplete(this.m_currentTransaction);
        else
          StoreManager.Get()?.HandleCommerceOrderFailure(this.m_currentTransaction);
        this.OnTransactionProcessCompleted();
      }
    }
  }

  void IVirtualCurrencyEventObserver.OnGetBalance(
    bool isError,
    string errorCode,
    CurrencyBalance balance)
  {
    if (isError)
    {
      Log.Store.PrintError("[HearthstoneCheckout.OnGetBalance]There was an error with the virtual currency 'GetBalance' call! (Http Result Status: {0}", (object) errorCode);
    }
    else
    {
      Log.Store.PrintInfo("[HearthstoneCheckout.OnGetBalance] Received balance response.  Currency - {0}   Balance - {1}", (object) balance.CurrencyCode, (object) balance.Balance);
      if (this.m_virtualCurrencyRequests.Count <= 0)
        return;
      HearthstoneCheckout.VirtualCurrencyBalanceResult vcBalanceResult = new HearthstoneCheckout.VirtualCurrencyBalanceResult(balance.IsOk, errorCode, balance.CurrencyCode, (long) balance.Balance);
      int index = 0;
      while (index < this.m_virtualCurrencyRequests.Count)
      {
        if (this.m_virtualCurrencyRequests[index].currencyCode == balance.CurrencyCode)
        {
          HearthstoneCheckout.VirtualCurrencyBalanceCallback callback = this.m_virtualCurrencyRequests[index].callback;
          if (callback != null)
            callback(vcBalanceResult);
          this.m_virtualCurrencyRequests.RemoveAt(index);
        }
        else
          ++index;
      }
    }
  }

  void ICatalogEventObserver.ProductsLoaded(
    IList<Product> products,
    float deserializeDuration)
  {
    float timeToLoadProducts = Time.realtimeSinceStartup - this.m_loadProductsStartTime;
    TelemetryManager.Client().SendLoadProducts(timeToLoadProducts, deserializeDuration);
    if (products == null)
    {
      Log.Store.PrintError("Received a product from server that was not defined!");
    }
    else
    {
      foreach (Product product in (IEnumerable<Product>) products)
      {
        Product curProduct = product;
        if (!ProductId.IsValid((long) curProduct.productId))
          Log.Store.PrintError(string.Format("The product received had invalid product ID ({0}).", (object) curProduct.productId));
        else if (!string.IsNullOrEmpty(curProduct.localization.name) || !string.IsNullOrEmpty(curProduct.localization.description) || curProduct.prices.Count != 0)
        {
          ProductId productId = ProductId.CreateFrom((long) curProduct.productId);
          string empty = string.Empty;
          string price = curProduct.externalPlatformSetting == null || curProduct.externalPlatformSetting.prices == null || curProduct.externalPlatformSetting.prices.Count <= 0 ? curProduct.prices.FirstOrDefault<ProductPrice>()?.localizedCurrentPrice : curProduct.externalPlatformSetting.prices.FirstOrDefault<ProductPrice>()?.localizedCurrentPrice;
          this.m_productMap[productId] = new HearthstoneCheckout.ProductInfo(curProduct.localization.name, curProduct.localization.description, price);
          PlatformId.CreateFrom(curProduct.externalPlatformSetting.externalPlatformId).Match((System.Action<PlatformId>) (platformId => this.m_externalIds[(productId, platformId)] = curProduct.externalPlatformSetting.externalPlatformProductId));
        }
      }
      this._shouldCallCSDKUpdate = false;
      HearthstoneCheckout.m_receivedSdkProducts = true;
      if (!CommerceWrapper.Instance.ResumeCommerceAPI())
        Log.Store.PrintWarning("[HearthstoneCheckout.ProductsLoaded] ResumeCommerceAPI failed.");
      this._shouldCallCSDKUpdate = true;
      Processor.QueueJob("HearthstoneCheckout.InvokeIsOpenCallbacks", this.InvokeIsOpenCallbacks(), JobFlags.StartImmediately);
    }
  }

  private IEnumerator<IAsyncJobResult> InvokeIsOpenCallbacks()
  {
    StoreManager manager = (StoreManager) null;
    do
    {
      manager = StoreManager.Get();
    }
    while (manager == null);
    while (!manager.IsOpen())
      yield return (IAsyncJobResult) null;
    while (this._isOpenCallbacks.Count<System.Action>() > 0)
    {
      this._isOpenCallbacks.Dequeue()();
      yield return (IAsyncJobResult) null;
    }
  }

  void ICatalogEventObserver.PersonalizedShopReceived(
    GetPagesResponse personalizedShopEvent)
  {
    Log.Store.PrintInfo("[HearthstoneCheckout.OnGetPersonalizedShopEvent] Received shop personalization data.");
    try
    {
      StoreManager.Get().OnHearthstoneGetPersonalizedShopData(personalizedShopEvent);
    }
    catch (Exception ex)
    {
      Log.Store.PrintError(ex.Message);
    }
  }

  public override void OnLogEvent(
    IntPtr owner,
    CommerceLogLevel logLevel,
    string subsystem,
    string message)
  {
    Log.Store.Log(HearthstoneCheckout.ConvertCommerceLogToILoggerLevel(logLevel), "[COMMMERCE(" + subsystem + ")] " + message, Array.Empty<object>());
  }

  private static Blizzard.T5.Core.LogLevel ConvertCommerceLogToILoggerLevel(
    CommerceLogLevel logLevel)
  {
    switch (logLevel)
    {
      case CommerceLogLevel.INFO:
        return Blizzard.T5.Core.LogLevel.Information;
      case CommerceLogLevel.WARNING:
        return Blizzard.T5.Core.LogLevel.Warning;
      case CommerceLogLevel.ERROR:
        return Blizzard.T5.Core.LogLevel.Error;
      case CommerceLogLevel.FATAL:
        return Blizzard.T5.Core.LogLevel.Critical;
      default:
        return Blizzard.T5.Core.LogLevel.Debug;
    }
  }

  public void ProcessDeeplink(string url)
  {
    if (string.IsNullOrEmpty(url) || this.m_currentTransaction == null || !this.IsTransactionInProgress())
    {
      Log.Store.Print("invalid state for deep link {0}", (object) url);
    }
    else
    {
      url = url.ToLower();
      if (url.EndsWith("nativepurchase"))
        this.m_purchaseHandle = CommerceWrapper.Instance.PurchaseCheckout(this.m_currentTransaction.ProductID, this.m_currentTransaction.CurrencyCode, new GenerateSSOToken().Token, this.GenerateExternalTransactionID(), true);
      else if (url.EndsWith("canceledpurchase"))
        ((IPurchaseEventObserver) this).OnCancel(this.m_currentTransaction);
      else if (url.EndsWith("finishedpurchase"))
        ((IPurchaseEventObserver) this).OnSuccessful(this.m_currentTransaction);
      Log.Store.Print("Deep link recieved {0}", (object) url);
    }
  }

  public delegate void VirtualCurrencyBalanceCallback(
    HearthstoneCheckout.VirtualCurrencyBalanceResult vcBalanceResult);

  public delegate void PersonalizedShopResponseCallback(GetPagesResponse response);

  public readonly struct VirtualCurrencyBalanceResult
  {
    public readonly bool isSuccess;
    public readonly string errorMessage;
    public readonly string currencyCode;
    public readonly long balance;

    public VirtualCurrencyBalanceResult(
      bool isSuccess,
      string errorMessage,
      string currencyCode,
      long balance)
    {
      this.isSuccess = isSuccess;
      this.errorMessage = errorMessage;
      this.currencyCode = currencyCode;
      this.balance = balance;
    }
  }

  private class WaitForIdle : IJobDependency, IAsyncJobResult
  {
    public bool IsReady() => CommerceWrapper.Instance.IsIdle;
  }

  private readonly struct ProductInfo
  {
    public readonly string title;
    public readonly string description;
    public readonly string price;

    public ProductInfo(string title, string description, string price)
    {
      this.title = title;
      this.description = description;
      this.price = price;
    }
  }

  private enum State
  {
    Uninit,
    Startup,
    Ready,
    Idle,
    InProgress,
    InProgress_Backgroundable,
    Finished,
    Unavailable,
  }

  private readonly struct VirtualCurrencyRequest
  {
    public readonly string currencyCode;
    public readonly HearthstoneCheckout.VirtualCurrencyBalanceCallback callback;

    public VirtualCurrencyRequest(
      string currencyCode,
      HearthstoneCheckout.VirtualCurrencyBalanceCallback callback)
    {
      this.currencyCode = currencyCode;
      this.callback = callback;
    }
  }

  private class WaitForClientInitializationResponse : IJobDependency, IAsyncJobResult
  {
    private readonly HearthstoneCheckout m_hearthstoneCheckout;
    private readonly float m_timeoutTimestamp;

    public WaitForClientInitializationResponse(
      HearthstoneCheckout hearthstoneCheckout,
      float timeoutDuration)
    {
      this.m_hearthstoneCheckout = hearthstoneCheckout;
      this.m_timeoutTimestamp = Time.realtimeSinceStartup + timeoutDuration;
    }

    public bool IsReady() => this.m_hearthstoneCheckout.m_clientInitializationResponse != HearthstoneCheckout.ClientInitializationResponse.Waiting || (double) this.m_timeoutTimestamp <= (double) Time.realtimeSinceStartup;
  }

  private enum ClientInitializationResponse
  {
    Waiting,
    Success,
    Fail,
  }
}
