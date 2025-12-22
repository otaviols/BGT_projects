using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelsPopupManager : MonoBehaviour, IStore
{
  private const string HEROIC_STORE_OPENED_EVENT = "HEROIC_STORE_OPENED";
  private const string OPEN_EVENT = "OPEN";
  private const string HIDE_EVENT = "HIDE";
  private const string BLOCK_SCREEN = "BLOCK_SCREEN";
  private const string UNBLOCK_SCREEN = "UNBLOCK_SCREEN";
  private const string OPEN_NOTICE = "OPEN_NOTICE";
  private const string PURCHASE_ACKNOWLEDGED = "PURCHASE_ACKNOWLEDGED";
  private const string SHOW_COIN_COUNTER_EVENT = "SHOW_COIN_COUNTER";
  private const string HIDE_COIN_COUNTER_EVENT = "HIDE_COIN_COUNTER";
  public AsyncReference m_buywithCurrencyReference;
  public AsyncReference m_buyWithGoldReference;
  public AsyncReference m_buyWithVCReference;
  public AsyncReference m_buyWithTicketReference;
  public AsyncReference m_normalButtonReference;
  public AsyncReference m_visualControllerReference;
  public AsyncReference m_fullScreenBlockerWidgetReference;
  public AsyncReference m_noticePopupConfirmReference;
  public AsyncReference m_infoButtonReference;
  public Material m_disabledButtonMaterial;
  private VisualController m_visualController;
  private Widget m_fullScreenBlockerWidget;
  private System.Action m_normalButtonPressedDelegate;
  private System.Action m_purchaseSuccessfulDelegate;
  private bool m_isStoreOpen;
  private bool m_isArenaTicketTransactionActive;
  private bool m_isVCPurchaseTransactionActive;
  private bool m_VCPurchaseSucceeded;
  private bool m_shouldShowCurrency;
  private Network.Bundle m_arenaTicketBundle;
  private System.Action m_noticeConfirmPressedDelegate;
  private Widget m_popupManagerWidget;
  private PVPDRLobbyDataModel m_dataModel;

  private bool IsAnyDuelsTransactionActive => this.m_isArenaTicketTransactionActive || this.m_isVCPurchaseTransactionActive;

  public event System.Action OnOpened;

  public event System.Action<StoreClosedArgs> OnClosed;

  public event System.Action OnReady;

  public event System.Action<BuyProductEventArgs> OnProductPurchaseAttempt;

  public event System.Action OnProductOpened;

  public void Start()
  {
    this.m_buywithCurrencyReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnBuyWithCurrencyButtonReady));
    this.m_buyWithGoldReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnBuyWithGoldButtonReady));
    this.m_buyWithVCReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnBuyWithVCButtonReady));
    this.m_buyWithTicketReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnBuyWithTicketButtonReady));
    this.m_normalButtonReference.RegisterReadyListener<Clickable>(new System.Action<Clickable>(this.OnNormalButtonReady));
    this.m_visualControllerReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnVisualControllerReady));
    this.m_fullScreenBlockerWidgetReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnFullScreenBlockerWidgetReady));
    this.m_noticePopupConfirmReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnNoticePopupButtonReady));
    this.m_infoButtonReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnInfoButtonReady));
    this.m_arenaTicketBundle = ArenaStore.GetDraftTicketProduct();
    this.m_popupManagerWidget = this.GetComponentInParent<Widget>();
    this.m_popupManagerWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnWidgetEvent));
    this.BindProductDataModel();
  }

  private void BindProductDataModel()
  {
    ProductDataModel productDataModel = (ProductDataModel) null;
    Network.Bundle arenaTicketBundle = this.m_arenaTicketBundle;
    long? pmtProductId;
    int num;
    if (arenaTicketBundle == null)
    {
      num = 0;
    }
    else
    {
      pmtProductId = arenaTicketBundle.PMTProductID;
      num = pmtProductId.HasValue ? 1 : 0;
    }
    if (num != 0)
    {
      pmtProductId = this.m_arenaTicketBundle.PMTProductID;
      if (ProductId.IsValid(pmtProductId.Value))
      {
        pmtProductId = this.m_arenaTicketBundle.PMTProductID;
        ProductId from = ProductId.CreateFrom(pmtProductId.Value);
        productDataModel = StoreManager.Get().Catalog.GetProductByPmtId(from);
      }
    }
    this.m_popupManagerWidget.BindDataModel((IDataModel) (productDataModel ?? ProductFactory.CreateEmptyProductDataModel()), true);
  }

  private void OnBuyWithCurrencyButtonReady(UIBButton button)
  {
    button.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnBuyWithCurrencyButtonPressed));
    bool enabled = StoreManager.Get().IsOpen(false);
    button.SetEnabled(enabled);
    if (enabled)
      return;
    button.m_RootObject.GetComponent<MeshRenderer>().SetMaterial(this.m_disabledButtonMaterial);
  }

  private void OnBuyWithCurrencyButtonPressed(UIEvent buttonEvent)
  {
    if ((Record) this.m_arenaTicketBundle == (Record) null)
      Debug.LogError((object) "Failed to perform Heroic Duel transaction because the arena ticket bundle was null");
    else
      this.StartArenaTicketTransaction((BuyProductEventArgs) new BuyPmtProductEventArgs(this.m_arenaTicketBundle, CurrencyType.REAL_MONEY, 1));
  }

  private void OnBuyWithVCButtonReady(UIBButton button)
  {
    button.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnBuyWithVCButtonPressed));
    bool enabled = StoreManager.Get().IsOpen(false);
    button.SetEnabled(enabled);
    if (enabled)
      return;
    button.m_RootObject.GetComponent<MeshRenderer>().SetMaterial(this.m_disabledButtonMaterial);
  }

  private void OnBuyWithVCButtonPressed(UIEvent buttonEvent)
  {
    if ((Record) this.m_arenaTicketBundle == (Record) null)
    {
      Debug.LogError((object) "Failed to perform Heroic Duel transaction because the arena ticket bundle was null");
    }
    else
    {
      CurrencyType currencyPriceType = ShopUtils.GetBundleVirtualCurrencyPriceType(this.m_arenaTicketBundle);
      long deficitForVcPurchase = this.GetDeficitForVCPurchase(currencyPriceType);
      if (deficitForVcPurchase > 0L)
        this.ShowVCPurchaseConfirmationPrompt(currencyPriceType, deficitForVcPurchase);
      else
        this.StartArenaTicketTransaction((BuyProductEventArgs) new BuyPmtProductEventArgs(this.m_arenaTicketBundle, currencyPriceType, 1));
    }
  }

  private void OnBuyWithGoldButtonReady(UIBButton button)
  {
    button.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnBuyWithGoldButtonPressed));
    bool enabled = NetCache.Get().GetGoldBalance() >= (long) DuelsConfig.PAID_GOLD_COST && StoreManager.Get().IsOpen(false);
    button.SetEnabled(enabled);
    if (enabled)
      return;
    button.m_RootObject.GetComponent<MeshRenderer>().SetMaterial(this.m_disabledButtonMaterial);
  }

  private void OnBuyWithGoldButtonPressed(UIEvent buttonEvent) => this.StartArenaTicketTransaction((BuyProductEventArgs) new BuyNoGTAPPEventArgs(new NoGTAPPTransactionData()
  {
    Product = ProductType.PRODUCT_TYPE_DRAFT,
    ProductData = 0,
    Quantity = 1
  }));

  private void OnBuyWithTicketButtonReady(UIBButton button) => button.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnBuyWithTicketButtonPressed));

  private void OnBuyWithTicketButtonPressed(UIEvent buttonEvent)
  {
    this.m_visualController.SetState("PURCHASE_ACKNOWLEDGED");
    this.m_purchaseSuccessfulDelegate();
  }

  private void OnNormalButtonReady(Clickable button) => button.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnNormalButtonPressed));

  private void OnNormalButtonPressed(UIEvent buttonEvent)
  {
    if (PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsEarlyAccess && !DuelsConfig.HasEarlyAccess())
    {
      this.OpenDuelsShop();
      this.Hide();
    }
    else
      this.m_normalButtonPressedDelegate();
  }

  private void OnVisualControllerReady(VisualController visualController)
  {
    this.m_visualController = visualController;
    this.m_visualController.RegisterStateChangedListener(new VisualController.OnStateChangedDelegate(this.OnStateChanged));
    if (this.OnReady == null)
      return;
    this.OnReady();
  }

  private void OnFullScreenBlockerWidgetReady(Widget fullScreenBlockerWidget) => this.m_fullScreenBlockerWidget = fullScreenBlockerWidget;

  private void OnNoticePopupButtonReady(UIBButton confirmButton) => confirmButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnNoticeConfirmPressed));

  private void OnInfoButtonReady(UIBButton infoButton) => infoButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnInfoButtonPressed));

  private void OnInfoButtonPressed(UIEvent e) => StoreManager.Get().ShowStoreInfo();

  private void OnNoticeConfirmPressed(UIEvent buttonEvent)
  {
    this.Hide();
    this.m_noticeConfirmPressedDelegate();
    this.m_noticeConfirmPressedDelegate = (System.Action) null;
  }

  private void OnStateChanged(VisualController visualController)
  {
    this.GetDataModel();
    if (visualController.State == "OPEN")
    {
      this.SetupDuelsStore();
    }
    else
    {
      if (!(visualController.State == "HIDE"))
        return;
      this.ShutdownDuelsStore();
    }
  }

  protected void OnWidgetEvent(string eventName)
  {
    if (eventName == "SHOW_COIN_COUNTER")
    {
      this.m_shouldShowCurrency = true;
      BnetBar.Get()?.RefreshCurrency();
    }
    else
    {
      if (!(eventName == "HIDE_COIN_COUNTER"))
        return;
      this.m_shouldShowCurrency = false;
      BnetBar.Get()?.RefreshCurrency();
    }
  }

  public bool ShouldShowCoinCounter() => this.m_shouldShowCurrency && this.m_isStoreOpen;

  public void Show()
  {
    System.Action onProductOpened = this.OnProductOpened;
    if (onProductOpened != null)
      onProductOpened();
    this.m_visualController.SetState("OPEN");
  }

  public void ShowNotice(string header, string desc, string rating, System.Action callback)
  {
    this.AddOnNoticeConfirmButtonPressedDelegate(callback);
    this.SetNoticeText(header, desc, rating);
    this.m_visualController.SetState("OPEN_NOTICE");
  }

  public void Hide()
  {
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_IDLE);
    this.m_visualController.SetState("HIDE");
  }

  public void AddOnNormalButtonPressedDelegate(System.Action action) => this.m_normalButtonPressedDelegate += action;

  public void RemoveOnNormalButtonPressedDelegate(System.Action action) => this.m_normalButtonPressedDelegate -= action;

  public void AddOnSuccessfulPurchaseDelegate(System.Action action) => this.m_purchaseSuccessfulDelegate += action;

  public void RemoveOnSuccessfulPurchaseDelegate(System.Action action) => this.m_purchaseSuccessfulDelegate -= action;

  public void AddOnNoticeConfirmButtonPressedDelegate(System.Action action) => this.m_noticeConfirmPressedDelegate += action;

  public void RemoveOnNoticeConfirmButtonPressedDelegate(System.Action action) => this.m_noticeConfirmPressedDelegate -= action;

  public void SetNoticeText(string header, string desc, string ratingText = "")
  {
    if (this.GetDataModel() == null)
      return;
    this.m_dataModel.NoticeHeaderString = header;
    this.m_dataModel.NoticeDescString = desc;
    if (string.IsNullOrEmpty(ratingText))
      return;
    this.m_dataModel.NoticeRatingString = ratingText;
  }

  private PVPDRLobbyDataModel GetDataModel()
  {
    if (this.m_dataModel == null && (UnityEngine.Object) PvPDungeonRunDisplay.Get() != (UnityEngine.Object) null && (UnityEngine.Object) this.m_popupManagerWidget != (UnityEngine.Object) null)
    {
      this.m_dataModel = PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel();
      this.m_popupManagerWidget.BindDataModel((IDataModel) this.m_dataModel);
    }
    return this.m_dataModel;
  }

  private void SetupDuelsStore()
  {
    StoreManager.Get().SetupDuelsStore(this);
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new System.Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    StoreManager.Get().RegisterFailedPurchaseAckListener(new System.Action<Network.Bundle, PaymentMethod>(this.OnFailedPurchaseAck));
    this.m_isStoreOpen = true;
    System.Action onOpened = this.OnOpened;
    if (onOpened != null)
      onOpened();
    BnetBar.Get()?.RefreshCurrency();
  }

  private void ShutdownDuelsStore()
  {
    this.CancelArenaTicketTransaction();
    this.CancelVCPurchaseTransaction();
    System.Action<StoreClosedArgs> onClosed = this.OnClosed;
    if (onClosed != null)
      onClosed(new StoreClosedArgs());
    StoreManager.Get().RemoveFailedPurchaseAckListener(new System.Action<Network.Bundle, PaymentMethod>(this.OnFailedPurchaseAck));
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new System.Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    StoreManager.Get().ShutDownDuelsStore();
    this.OnProductPurchaseAttempt = (System.Action<BuyProductEventArgs>) null;
    this.m_isStoreOpen = false;
    this.BlockInputs(false);
    PvPDungeonRunDisplay.Get().EnableButtons();
    BnetBar.Get()?.RefreshCurrency();
  }

  private long GetDeficitForVCPurchase(CurrencyType currency)
  {
    long deficitForVcPurchase = 0;
    if ((Record) this.m_arenaTicketBundle == (Record) null)
    {
      Debug.LogError((object) "Failed to calculate VC deficit because m_arenaTicketBundle was null");
      return deficitForVcPurchase;
    }
    long? virtualCurrencyCost = this.m_arenaTicketBundle.VirtualCurrencyCost;
    if (!virtualCurrencyCost.HasValue)
    {
      Debug.LogError((object) "Failed to calculate VC deficit because m_arenaTicketBundle.VirtualCurrencyCost was null");
      return deficitForVcPurchase;
    }
    PriceDataModel price = new PriceDataModel();
    price.Currency = currency;
    virtualCurrencyCost = this.m_arenaTicketBundle.VirtualCurrencyCost;
    price.Amount = (float) virtualCurrencyCost.Value;
    return ShopUtils.GetDeficit(price);
  }

  private void ShowVCPurchaseConfirmationPrompt(CurrencyType currencyType, long vcAmount)
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
        if (response != AlertPopup.Response.CONFIRM)
          return;
        this.StartVCPurchaseTransaction(currencyType, vcAmount);
      })
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void StartVCPurchaseTransaction(CurrencyType currencyType, long vcAmount)
  {
    if (!ShopUtils.IsVirtualCurrencyTypeEnabled(currencyType))
      Debug.LogError((object) string.Format("Attempted to start a VC purchase transaction where the currency type {0} is not enabled", (object) currencyType));
    else if (this.IsAnyDuelsTransactionActive)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DUELS_HEADLINE"),
        m_text = GameStrings.Get("GLUE_CHECKOUT_ERROR_GENERIC_FAILURE"),
        m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      });
      Debug.LogError((object) "Attempted to start a virtual currency purchase transaction while an existing transaction was in progress");
    }
    else
    {
      this.m_isVCPurchaseTransactionActive = true;
      this.m_VCPurchaseSucceeded = false;
      if (ShopUtils.IsMainVirtualCurrencyType(currencyType))
      {
        Shop.Get().OpenVirtualCurrencyPurchase((float) vcAmount, true);
      }
      else
      {
        if (!ShopUtils.IsBoosterVirtualCurrencyType(currencyType))
          return;
        Shop.Get().OpenBoosterCurrencyPurchase((float) vcAmount, true);
      }
    }
  }

  private void EndVCPurchaseTransaction()
  {
    if (!this.m_isVCPurchaseTransactionActive)
      return;
    this.m_isVCPurchaseTransactionActive = false;
    if (!this.m_VCPurchaseSucceeded)
      return;
    this.m_VCPurchaseSucceeded = false;
    this.StartArenaTicketTransaction((BuyProductEventArgs) new BuyPmtProductEventArgs(this.m_arenaTicketBundle, ShopUtils.GetBundleVirtualCurrencyPriceType(this.m_arenaTicketBundle), 1));
  }

  private void CancelVCPurchaseTransaction()
  {
    if (!this.m_isVCPurchaseTransactionActive)
      return;
    this.m_VCPurchaseSucceeded = false;
    this.EndVCPurchaseTransaction();
  }

  private void StartArenaTicketTransaction(BuyProductEventArgs purchaseEventArgs)
  {
    if (this.IsAnyDuelsTransactionActive)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DUELS_HEADLINE"),
        m_text = GameStrings.Get("GLUE_CHECKOUT_ERROR_GENERIC_FAILURE"),
        m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      });
      Debug.LogError((object) "Attempted to start an arena ticket transaction while an existing transaction was in progress");
    }
    else if (this.OnProductPurchaseAttempt == null)
    {
      Debug.LogError((object) "Attempted to start an arena ticket transaction while OnProductPurchaseAttempt was null");
    }
    else
    {
      this.m_isArenaTicketTransactionActive = true;
      this.OnProductPurchaseAttempt(purchaseEventArgs);
    }
  }

  private void EndArenaTicketTransaction()
  {
    if (!this.m_isArenaTicketTransactionActive)
      return;
    this.m_isArenaTicketTransactionActive = false;
  }

  private void CancelArenaTicketTransaction()
  {
    if (!this.m_isArenaTicketTransactionActive)
      return;
    this.EndArenaTicketTransaction();
  }

  private void OnSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (this.m_isArenaTicketTransactionActive)
    {
      this.EndArenaTicketTransaction();
      this.BlockInputs(false);
      this.m_visualController.SetState("PURCHASE_ACKNOWLEDGED");
      this.m_purchaseSuccessfulDelegate();
    }
    else
    {
      if (!this.m_isVCPurchaseTransactionActive)
        return;
      this.m_VCPurchaseSucceeded = true;
      this.EndVCPurchaseTransaction();
    }
  }

  private void OnFailedPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (this.m_isArenaTicketTransactionActive)
    {
      this.EndArenaTicketTransaction();
    }
    else
    {
      if (!this.m_isVCPurchaseTransactionActive)
        return;
      this.EndVCPurchaseTransaction();
    }
  }

  private void OpenDuelsShop()
  {
    int earlyAccessPMTLicenseId = (int) NetCache.Get().GetDuelsEarlyAccessLicenseId();
    AccountLicenseDbfRecord record = GameDbf.AccountLicense.GetRecord((Predicate<AccountLicenseDbfRecord>) (rec => (long) earlyAccessPMTLicenseId == rec.LicenseId));
    if (record == null)
    {
      Debug.LogWarning((object) "DuelsPopupManager::OpenDuelsShop() - Duels early access account license not found.");
      this.ShowallDuelsBundlesErrorPopup();
    }
    List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(ProductType.PRODUCT_TYPE_FIXED_LICENSE, false, record.ID);
    if (bundlesForProduct == null || bundlesForProduct.Count == 0)
    {
      Debug.LogWarning((object) "DuelsPopupManager::OpenDuelsShop() - No products in the shop have Duels early access.");
      this.ShowallDuelsBundlesErrorPopup();
    }
    else
    {
      int index = (int) (BattleNet.GetMyAccoundId().Low % (ulong) bundlesForProduct.Count);
      Network.Bundle bundle = bundlesForProduct[index];
      if (!((Record) bundle == (Record) null))
      {
        long? pmtProductId = bundle.PMTProductID;
        if (pmtProductId.HasValue)
        {
          pmtProductId = bundle.PMTProductID;
          Shop.OpenToProductPageWhenReady(pmtProductId.Value, true);
          return;
        }
      }
      Debug.LogWarning((object) "DuelsPopupManager::OpenDuelsShop() - Duels product has no PMT Product Id.");
      this.ShowallDuelsBundlesErrorPopup();
    }
  }

  private void ShowallDuelsBundlesErrorPopup()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_PVPDR"),
      m_text = GameStrings.Get("GLUE_PVPDR_EARLY_ACCESS_SHOP_ERROR_BODY"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void BlockInputs(bool blocked)
  {
    if ((UnityEngine.Object) this.m_fullScreenBlockerWidget == (UnityEngine.Object) null)
      Debug.LogError((object) "Failed to toggle interface blocker from Duels Popup Manager");
    else if (blocked)
      this.m_fullScreenBlockerWidget.TriggerEvent("BLOCK_SCREEN");
    else
      this.m_fullScreenBlockerWidget.TriggerEvent("UNBLOCK_SCREEN");
  }

  void IStore.Open() => Shop.Get().RefreshDataModel();

  void IStore.Close()
  {
    this.CancelArenaTicketTransaction();
    this.CancelVCPurchaseTransaction();
  }

  void IStore.BlockInterface(bool blocked) => this.BlockInputs(blocked);

  bool IStore.IsReady() => true;

  bool IStore.IsOpen() => this.m_isStoreOpen;

  void IStore.Unload()
  {
  }

  IEnumerable<CurrencyType> IStore.GetVisibleCurrencies()
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
}
