using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusLuckyDraw;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class LuckyDrawWidget : MonoBehaviour, IStore
{
  public static AssetReference LUCKY_DRAW_MANAGER_POPUP_PREFAB = new AssetReference("LuckyDrawManagerPopup.prefab:7411ab66e5e09ed408bc291d20af76d6");
  public AsyncReference m_hammerManagerReference;
  public AsyncReference m_boardDetailsDisplayReference;
  public AsyncReference m_finisherDetailsDisplayReference;
  public AsyncReference m_portraitDetailsDisplayReference;
  public AsyncReference m_emoteDetailsDisplayReference;
  [SerializeField]
  private AsyncReference m_LuckyDrawShopWidgetReference;
  private LuckyDrawBoardSkinDetails m_boardDetailsDisplay;
  private LuckyDrawFinisherDetails m_finisherDetailsDisplay;
  private LuckyDrawPortraitDetails m_portraitDetailsDisplay;
  private LuckyDrawEmoteDetails m_emoteDetailsDisplay;
  private RewardPresenter m_rewardPresenter = new RewardPresenter();
  private LuckyDrawHammerSlot m_luckyDrawHammerSlot;
  private Widget m_LuckyDrawShopPopupWidget;
  [SerializeField]
  private WidgetInstance m_luckyDrawLayout;
  [SerializeField]
  private Renderer m_FrameRenderer;
  private Widget m_widget;
  private GameObject m_owner;
  private int m_rewardTileSelected;
  private bool m_usingFirstHammer;
  private const string CLOSE_EVENT = "CODE_CLOSE";
  private const string SPEND_HAMMER = "CODE_HAMMER_USED";
  private const string TILE_ANTICIPATION = "CODE_TILE_ANTICIPATION";
  private const string TILE_ANTICIPATION_FINISHED = "CODE_TILE_ANTICIPATION_FINISHED";
  private const string MOUSE_OFF_HAMMER_BUTTON = "HAMMER_BUTTON_MOUSE_OFF";
  private const string HAMMER_SMASH_TILE = "HAMMER_FSM_SMASH_TILE";
  private const string USE_FIRST_HAMMER = "CODE_USE_FIRST_HAMMER";
  private const string START_FIRST_HAMMER_ANIM = "CODE_START_FIRST_HAMMER_ANIM";
  private const string kRewardDetailsEventName = "REWARD_clicked";
  private const string kRewardGrantedEventName = "CODE_SEND_REWARD_GRANTED";
  private const string kShowDetailViewEventName = "LUCKY_DRAW_SHOW";
  private const string kShowDetailRewardViewEventName = "LUCKY_DRAW_SHOW_REWARD";
  private const string HIDE_BATTLEPASS_SHOP = "CODE_HIDE_BATTLEBASH_SHOP";
  private const string SHOW_BATTLEBASH_SHOP = "CODE_BATTLEBASH_PURCHASE";
  private const string SHOP_SHOW_INFO = "SHOP_SHOW_INFO";
  private const string SHOW_NO_HAMMER_POPUP = "SHOW_NO_HAMMER_POPUP";
  private bool m_isStoreOpen;
  private PopupSwitcher m_popupSwitcher = new PopupSwitcher();
  private LuckyDrawManager m_luckyDrawManager;

  public event Action OnOpened;

  public event Action<StoreClosedArgs> OnClosed;

  public event Action OnReady;

  public event Action<BuyProductEventArgs> OnProductPurchaseAttempt;

  public event Action OnProductOpened;

  private void OnDestroy()
  {
    this.ShutDownLuckyDrawStore();
    this.m_luckyDrawManager?.UnregisterOnInitOrUpdateFinishedCallback(new Action(this.OnLuckyDrawDataInitializedOrUpdated));
  }

  private void Awake()
  {
    this.m_luckyDrawManager = LuckyDrawManager.Get();
    if (this.m_luckyDrawManager == null)
    {
      Error.AddDevWarning("Error", "[LuckyDrawWidget] Awake() LuckyDrawManager is null!");
    }
    else
    {
      this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
      if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      {
        Error.AddDevWarning("UI Error", "[LuckyDrawWidget] Awake() WidgetTemplate not found on {0}", (object) this.gameObject.name);
      }
      else
      {
        this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
        this.m_popupSwitcher.RegisterPopupWidgetInstance(this.m_boardDetailsDisplayReference, "OffDialogClick_code", (Action<IDataModel>) (dataModel => this.m_boardDetailsDisplay.OnShow(dataModel)), (Action) (() => this.m_boardDetailsDisplay.OnHide()));
        this.m_popupSwitcher.RegisterPopupWidgetInstance(this.m_finisherDetailsDisplayReference, "OffDialogClick_code", (Action<IDataModel>) (dataModel => this.m_finisherDetailsDisplay.OnShow(dataModel)), (Action) (() => this.m_finisherDetailsDisplay.OnHide()));
        this.m_popupSwitcher.RegisterPopupWidgetInstance(this.m_portraitDetailsDisplayReference, "OffDialogClick_code", (Action<IDataModel>) (dataModel => this.m_portraitDetailsDisplay.Show()), (Action) (() => this.m_portraitDetailsDisplay.Hide()));
        this.m_popupSwitcher.RegisterPopupWidgetInstance(this.m_emoteDetailsDisplayReference, "OffDialogClick_code", (Action<IDataModel>) (dataModel => this.m_emoteDetailsDisplay.Show()), (Action) (() => this.m_emoteDetailsDisplay.Hide()));
        this.m_popupSwitcher.RegisterPopupWidgetInstance(this.m_LuckyDrawShopWidgetReference, "CODE_HIDE_BATTLEBASH_SHOP", (Action<IDataModel>) (dataModel => this.m_LuckyDrawShopPopupWidget.BindDataModel((IDataModel) ((ProductSelectionDataModel) dataModel).Variant)));
        this.m_owner = this.gameObject;
        if ((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null && (UnityEngine.Object) this.transform.parent.GetComponent<WidgetInstance>() != (UnityEngine.Object) null)
          this.m_owner = this.transform.parent.gameObject;
        if (this.m_hammerManagerReference != null)
          return;
        Error.AddDevWarning("UI Error", "[LuckyDrawWidget] Awake() Hammer Manager not assigned! LuckyDrawHammerSlot instance should be assigned");
      }
    }
  }

  private void Start()
  {
    this.m_boardDetailsDisplayReference.RegisterReadyListener<WidgetInstance>(new Action<WidgetInstance>(this.OnBoardDetailsDisplayReady));
    this.m_finisherDetailsDisplayReference.RegisterReadyListener<WidgetInstance>(new Action<WidgetInstance>(this.OnFinisherDetailsDisplayReady));
    this.m_portraitDetailsDisplayReference.RegisterReadyListener<WidgetInstance>(new Action<WidgetInstance>(this.OnPortraitDetailsDisplayReady));
    this.m_emoteDetailsDisplayReference.RegisterReadyListener<WidgetInstance>(new Action<WidgetInstance>(this.OnEmoteDetailsDisplayReady));
    this.m_hammerManagerReference.RegisterReadyListener<WidgetInstance>(new Action<WidgetInstance>(this.OnHammerPlaymakerReady));
    this.m_LuckyDrawShopWidgetReference.RegisterReadyListener<WidgetInstance>(new Action<WidgetInstance>(this.OnLuckyDrawShopWidgetReady));
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.RewardDetailsEventListener));
    Action onReady = this.OnReady;
    if (onReady != null)
      onReady();
    Action onProductOpened = this.OnProductOpened;
    if (onProductOpened == null)
      return;
    onProductOpened();
  }

  private void HandleEvent(string eventName)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(eventName))
    {
      case 355460073:
        if (!(eventName == "CODE_HAMMER_USED"))
          break;
        this.InitiateSpendHammerFlow();
        break;
      case 499991365:
        if (!(eventName == "CODE_BATTLEBASH_PURCHASE"))
          break;
        TelemetryManager.Client()?.SendLuckyDrawEventMessage("LuckyDrawBuyButtonClicked");
        this.ShowBattleBashShop();
        break;
      case 2074662346:
        if (!(eventName == "CODE_TILE_ANTICIPATION_FINISHED"))
          break;
        this.TileAnticipationFinished();
        break;
      case 3069241055:
        if (!(eventName == "CODE_CLOSE"))
          break;
        this.Close();
        break;
      case 3149635636:
        if (!(eventName == "CODE_USE_FIRST_HAMMER"))
          break;
        LuckyDrawManager.Get().UseLuckyDrawHammer(this);
        break;
      case 3226942546:
        if (!(eventName == "HAMMER_BUTTON_MOUSE_OFF"))
          break;
        this.TrySetHammerButtonIdleAnimation();
        break;
      case 3248951211:
        if (!(eventName == "CODE_TILE_ANTICIPATION"))
          break;
        this.PlayTileAnticipationAnim();
        break;
      case 4038938703:
        if (!(eventName == "CODE_START_FIRST_HAMMER_ANIM"))
          break;
        this.m_usingFirstHammer = LuckyDrawManager.Get().HasUnclamedFree();
        this.InitializeHammerFSMVariables();
        this.m_luckyDrawHammerSlot.GetComponent<Widget>().TriggerEvent("CODE_DO_FIRST_HAMMER_CLAIM_ANIMATION");
        break;
      case 4166486004:
        if (!(eventName == "SHOW_NO_HAMMER_POPUP"))
          break;
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_BATTLEBASH_ERROR_NO_HAMMERS"),
          m_text = GameStrings.Get("GLUE_BATTLEBASH_EARN_MORE_HAMMERS"),
          m_responseDisplay = AlertPopup.ResponseDisplay.OK,
          m_showAlertIcon = true,
          m_okText = GameStrings.Get("GLOBAL_OKAY")
        };
        DialogManager.Get().ShowPopup(info);
        break;
      case 4214625578:
        if (!(eventName == "HAMMER_FSM_SMASH_TILE"))
          break;
        this.PlayTileSmashAnim();
        break;
    }
  }

  private void HandleShopPopupEvent(string eventName)
  {
    if (!(eventName == "SHOP_BUY_WITH_FIRST_CURRENCY"))
    {
      if (!(eventName == "SHOP_BUY_WITH_ALT_CURRENCY"))
      {
        if (!(eventName == "CODE_HIDE_BATTLEBASH_SHOP"))
        {
          if (!(eventName == "SHOP_SHOW_INFO"))
            return;
          StoreManager.Get().ShowStoreInfo();
        }
        else
          this.m_popupSwitcher.HidePopup(this.m_LuckyDrawShopWidgetReference);
      }
      else
        this.StartLuckyDrawTransaction(1);
    }
    else
      this.StartLuckyDrawTransaction(0);
  }

  public void DisplayFirstHammerPopup()
  {
    if ((UnityEngine.Object) this.m_luckyDrawHammerSlot == (UnityEngine.Object) null)
      Error.AddDevWarning("Error", "[LuckyDrawWidget] DisplayFirstHammerPopup() m_luckyDrawHammerSlot was null!");
    else
      this.m_luckyDrawHammerSlot.DisplayFirstHammer();
  }

  public void Close() => UnityEngine.Object.Destroy((UnityEngine.Object) this.m_owner);

  public void Show() => this.m_luckyDrawLayout.RegisterReadyListener((Action<object>) (_ => this.InitializeLuckyDrawLayoutWidget()), (object) null, true);

  private void ShowFinisherDetailDisplay(
    LuckyDrawRewardDataModel dataModel,
    bool showRewardGrantVFX)
  {
    if (dataModel == null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawWidget] ShowFinisherDetailDisplay() dataModel was null!");
    }
    else
    {
      this.m_finisherDetailsDisplay.ShowingRewardGrantVFX = showRewardGrantVFX;
      this.m_popupSwitcher.ShowPopup(this.m_finisherDetailsDisplayReference, (IDataModel) dataModel);
    }
  }

  private void ShowBoardDetailDisplay(LuckyDrawRewardDataModel dataModel, bool showRewardVFX)
  {
    if (dataModel == null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawWidget] ShowBoardDetailDisplay() dataModel was null!");
    }
    else
    {
      this.m_boardDetailsDisplay.ShowingRewardGrantVFX = showRewardVFX;
      this.m_popupSwitcher.ShowPopup(this.m_boardDetailsDisplayReference, (IDataModel) dataModel);
    }
  }

  private void ShowPortraitDetailsDisplay(
    LuckyDrawRewardDataModel dataModel,
    bool showRewardGrantVFX)
  {
    if (dataModel == null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawWidget] ShowPortraitDetailDisplay() datamodel was null!");
    }
    else
    {
      this.m_popupSwitcher.ShowPopup(this.m_portraitDetailsDisplayReference, (IDataModel) dataModel);
      if (showRewardGrantVFX)
        EventFunctions.TriggerEvent(this.m_portraitDetailsDisplay.transform.parent, "LUCKY_DRAW_SHOW_REWARD");
      else
        EventFunctions.TriggerEvent(this.m_portraitDetailsDisplay.transform.parent, "LUCKY_DRAW_SHOW");
    }
  }

  private void ShowEmoteDetailsDisplay(LuckyDrawRewardDataModel dataModel, bool showRewardGrantVFX)
  {
    if (dataModel == null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawWidget] ShowEmoteDetailsDisplay() datamodel was null!");
    }
    else
    {
      this.m_popupSwitcher.ShowPopup(this.m_emoteDetailsDisplayReference, (IDataModel) dataModel);
      if (showRewardGrantVFX)
        EventFunctions.TriggerEvent(this.m_emoteDetailsDisplay.transform.parent, "LUCKY_DRAW_SHOW_REWARD");
      else
        EventFunctions.TriggerEvent(this.m_emoteDetailsDisplay.transform.parent, "LUCKY_DRAW_SHOW");
    }
  }

  private void ShowBattleBashShop()
  {
    if ((UnityEngine.Object) this.m_LuckyDrawShopPopupWidget == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawWidget] ShowBattleBashShop() Cannot open Lucky Draw shop popup. m_LuckyDrawShopPopupWidget was null.");
    }
    else
    {
      Network.Bundle product = this.m_luckyDrawManager.GetProduct();
      if ((Record) product == (Record) null)
      {
        Error.AddDevWarning("UI Error", "[LuckyDrawWidget] ShowBattleBashShop() Cannot open Lucky Draw shop popup. luckyDrawBundle was null.");
      }
      else
      {
        this.SetUpLuckyDrawStore();
        ProductDataModel productByPmtId = StoreManager.Get().Catalog.GetProductByPmtId(ProductId.CreateFrom(product.PMTProductID.Value));
        if (productByPmtId == null)
        {
          AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
          {
            m_showAlertIcon = false,
            m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM,
            m_confirmText = GameStrings.Get("GLOBAL_BUTTON_OK"),
            m_alertTextAlignment = UberText.AlignmentOptions.Center,
            m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
            m_headerText = GameStrings.Get("GLUE_BATTLEBASH_SHOP_UNAVAILABLE_HEADER"),
            m_text = GameStrings.Get("GLUE_BATTLEBASH_SHOP_UNAVAILABLE_BODY")
          };
          DialogManager.Get().ShowPopup(info);
        }
        else
        {
          this.m_popupSwitcher.ShowPopup(this.m_LuckyDrawShopWidgetReference, (IDataModel) new ProductSelectionDataModel()
          {
            MaxQuantity = 1,
            Quantity = 1,
            Variant = productByPmtId
          });
          EventFunctions.TriggerEvent(this.m_LuckyDrawShopPopupWidget.transform, "LUCKY_DRAW_SHOW");
        }
      }
    }
  }

  private void InitializeLuckyDrawLayoutWidget()
  {
    LuckyDrawLayout componentInChildren = this.m_luckyDrawLayout.GetComponentInChildren<LuckyDrawLayout>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error", "[LuckyDrawWidget] InitializeLuckyDrawLayoutWidget() could not find LuckydrawLayout on object {0}", (object) this.m_luckyDrawLayout.name);
    else if (!this.m_luckyDrawManager.IsIntialized())
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawWidget] InitializeLuckyDrawLayoutWidget() Lucky Draw Data not initialized!");
    }
    else
    {
      this.m_luckyDrawManager.BindLuckyDrawDataModelToWidget((Widget) this.m_luckyDrawLayout);
      componentInChildren.InitializeRewardTileWidgets(this.m_luckyDrawManager.GetBattlegroundsLuckyDrawDataModel().Rewards);
    }
  }

  private void InitiateSpendHammerFlow()
  {
    if (this.m_luckyDrawManager.GetBattlegroundsLuckyDrawDataModel().Hammers <= 0)
    {
      Log.All.PrintError("Error [LuckyDrawWidget] InitiateSpendHammerFlow() The player attempted to spend a hammer when no hammers are available!");
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else
    {
      this.InitializeHammerFSMVariables();
      this.m_luckyDrawManager.UseLuckyDrawHammer(this);
    }
  }

  private void InitializeHammerFSMVariables() => this.m_luckyDrawHammerSlot.GetComponent<Widget>().TriggerEvent("CODE_INITIALIZE_HAMMER");

  private void PlayTileAnticipationAnim() => this.m_luckyDrawLayout.GetComponentInChildren<LuckyDrawLayout>().AnimateTiles();

  private void TileAnticipationFinished() => this.m_luckyDrawHammerSlot.GetComponent<Widget>().TriggerEvent("CODE_ANTICIPATION_FINISHED");

  private void PlayTileSmashAnim() => this.m_luckyDrawLayout.GetComponentInChildren<LuckyDrawLayout>().PlayTileSmashAnim(this.m_rewardTileSelected);

  private void OnBoardDetailsDisplayReady(WidgetInstance widget) => this.m_boardDetailsDisplay = widget.Widget.GetComponent<LuckyDrawBoardSkinDetails>();

  private void OnFinisherDetailsDisplayReady(WidgetInstance widget) => this.m_finisherDetailsDisplay = widget.Widget.GetComponent<LuckyDrawFinisherDetails>();

  private void OnPortraitDetailsDisplayReady(WidgetInstance widget) => this.m_portraitDetailsDisplay = widget.Widget.GetComponent<LuckyDrawPortraitDetails>();

  private void OnEmoteDetailsDisplayReady(WidgetInstance widget) => this.m_emoteDetailsDisplay = widget.Widget.GetComponent<LuckyDrawEmoteDetails>();

  private void OnHammerPlaymakerReady(WidgetInstance widget)
  {
    LuckyDrawHammerSlot componentInChildren = widget.GetComponentInChildren<LuckyDrawHammerSlot>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawWidget] OnHammerPlaymakerReady() hammerSlot was null!");
    }
    else
    {
      this.m_luckyDrawHammerSlot = componentInChildren;
      if (!this.m_luckyDrawManager.IsIntialized())
        Error.AddDevWarning("UI Error", "[LuckyDrawWidget] OnHammerPlaymakerReady() luckyDrawManager not initialized!");
      else
        this.m_luckyDrawManager.BindAllLuckyDrawDataModelToWidget((Widget) widget);
    }
  }

  private void OnLuckyDrawShopWidgetReady(WidgetInstance widget)
  {
    this.m_LuckyDrawShopPopupWidget = (Widget) widget;
    widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleShopPopupEvent));
  }

  private void RewardDetailsEventListener(string eventName)
  {
    if (eventName == "REWARD_clicked")
    {
      this.ShowDetailDisplay(false);
    }
    else
    {
      if (!(eventName == "CODE_SEND_REWARD_GRANTED"))
        return;
      this.ShowDetailDisplay(true);
    }
  }

  private void ShowDetailDisplay(bool playRewardGrantVFX)
  {
    EventDataModel dataModel = this.m_widget.GetDataModel<EventDataModel>();
    if (dataModel == null)
    {
      Error.AddDevWarning("Error", "[LuckyDrawWidget] ShowDetailDisplay() No eventDataModel found from event call");
    }
    else
    {
      LuckyDrawRewardDataModel payload = (LuckyDrawRewardDataModel) dataModel.Payload;
      if (payload == null)
      {
        Error.AddDevWarning("Error", "[LuckyDrawWidget] ShowDetailDisplay() No eventDataModel Payload in event call");
      }
      else
      {
        RewardListDataModel rewardList = payload.RewardList;
        if (rewardList == null || rewardList.Items.Count <= 0)
        {
          Error.AddDevWarning("Error", "[LuckyDrawWidget] ShowDetailDisplay() Reward list has no valid data!");
        }
        else
        {
          switch (rewardList.Items[0].ItemType)
          {
            case RewardItemType.BATTLEGROUNDS_HERO_SKIN:
            case RewardItemType.BATTLEGROUNDS_GUIDE_SKIN:
              this.ShowPortraitDetailsDisplay(payload, playRewardGrantVFX);
              break;
            case RewardItemType.BATTLEGROUNDS_BOARD_SKIN:
              this.ShowBoardDetailDisplay(payload, playRewardGrantVFX);
              break;
            case RewardItemType.BATTLEGROUNDS_FINISHER:
              this.ShowFinisherDetailDisplay(payload, playRewardGrantVFX);
              break;
            case RewardItemType.BATTLEGROUNDS_EMOTE:
            case RewardItemType.BATTLEGROUNDS_EMOTE_PILE:
              this.ShowEmoteDetailsDisplay(payload, playRewardGrantVFX);
              break;
          }
        }
      }
    }
  }

  private void TrySetHammerButtonIdleAnimation()
  {
    if (this.m_luckyDrawHammerSlot.HammerPlaymaker.FsmVariables.GetFsmBool("HammerAnimationInProgress").Value)
      return;
    this.m_luckyDrawHammerSlot.HammerPlaymaker.SendEvent("Button_MouseOff");
  }

  public void OnRewardResponseReceived(LuckyDrawUseHammerResponse rewardResponse)
  {
    if (rewardResponse.HasErrorCode && rewardResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.All.PrintError("Error [LuckyDrawWidget] OnRewardResponseReceived() response had error {0}", (object) rewardResponse.ErrorCode);
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else
    {
      this.m_rewardTileSelected = this.m_luckyDrawLayout.GetComponentInChildren<LuckyDrawLayout>().GetTileFromRewardID(rewardResponse.GrantedRewardId);
      if (this.m_rewardTileSelected < 0)
      {
        Log.All.PrintError("Error [LuckyDrawWidget] OnRewardResponseReceived() selected reward not found!");
        LuckyDrawUtils.ShowErrorAndReturnToLobby();
      }
      this.SetupHammerSmashAnimation();
      if (!this.m_usingFirstHammer || rewardResponse.NumUnusedFreeHammersRemaining >= 1)
        return;
      this.m_usingFirstHammer = false;
      this.m_luckyDrawManager.UsedFreeHammer(rewardResponse);
    }
  }

  private void SetupHammerSmashAnimation()
  {
    Vector3 worldPositionOfTile = this.m_luckyDrawLayout.GetComponentInChildren<LuckyDrawLayout>().GetWorldPositionOfTile(this.m_rewardTileSelected);
    this.m_luckyDrawHammerSlot.GetComponent<Widget>().TriggerEvent("CODE_HAMMER_SMASH_READY");
  }

  private void SetUpLuckyDrawStore()
  {
    StoreManager storeManager = StoreManager.Get();
    storeManager.StartLuckyDrawStore(this);
    storeManager.RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    storeManager.RegisterFailedPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnFailedPurchaseAck));
    storeManager.RegisterSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchase));
    this.m_isStoreOpen = true;
    Action onOpened = this.OnOpened;
    if (onOpened != null)
      onOpened();
    BnetBar.Get()?.RefreshCurrency();
  }

  private void ShutDownLuckyDrawStore()
  {
    Action<StoreClosedArgs> onClosed = this.OnClosed;
    if (onClosed != null)
      onClosed(new StoreClosedArgs());
    StoreManager storeManager = StoreManager.Get();
    storeManager.RemoveFailedPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnFailedPurchaseAck));
    storeManager.RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    storeManager.ShutDownLuckyDrawStore();
    this.m_isStoreOpen = false;
  }

  private void StartLuckyDrawTransaction(int priceIndex)
  {
    LuckyDrawManager luckyDrawManager = LuckyDrawManager.Get();
    StoreManager storeManager = StoreManager.Get();
    Shop shop = Shop.Get();
    Network.Bundle product = this.m_luckyDrawManager.GetProduct();
    if ((Record) product == (Record) null)
    {
      Error.AddDevWarning("Error", "[LuckyDrawWidget] StartLuckyDrawTransaction() Cannot start Lucky Draw transaction. No product found. bundle was null");
    }
    else
    {
      ProductId from = ProductId.CreateFrom(product.PMTProductID.Value);
      ProductDataModel productDataModel = storeManager.Catalog.GetProductByPmtId(from);
      if (productDataModel == null)
      {
        Error.AddDevWarning("UI Error", "[LuckyDrawWidget] StartLuckyDrawTransaction() Cannot start Lucky Draw transaction. No product data model found. productDataModel was null");
      }
      else
      {
        PriceDataModel priceDataModel = productDataModel.Prices.ElementAtOrDefault<PriceDataModel>(priceIndex);
        if (priceDataModel == null)
        {
          Error.AddDevWarning("UI Error", "[LuckyDrawWidget] StartLuckyDrawTransaction() Cannot start Lucky Draw transaction. Product data model has no price at index {0}.", (object) priceIndex);
        }
        else
        {
          TimeSpan drawTimeRemaining = LuckyDrawUtils.GetLuckyDrawTimeRemaining(luckyDrawManager.GetActiveLuckyDrawBoxID());
          AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
          {
            m_showAlertIcon = false,
            m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
            m_confirmText = GameStrings.Get("GLOBAL_BUTTON_OK"),
            m_cancelText = GameStrings.Get("GLOBAL_CANCEL"),
            m_alertTextAlignment = UberText.AlignmentOptions.Center,
            m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
            m_headerText = GameStrings.Get("GLUE_BACON_BATTLEBASH_PURCHASE_HEADER"),
            m_text = GameStrings.Format("GLUE_BACON_BATTLEBASH_PURCHASE_BODY", (object) drawTimeRemaining.Days),
            m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
            {
              if (response != AlertPopup.Response.CONFIRM)
                return;
              shop.AttemptToPurchaseProduct(productDataModel, priceDataModel, 1, true);
            })
          };
          DialogManager.Get().ShowPopup(info);
        }
      }
    }
  }

  private void OnSuccessfulPurchase(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (bundle.Items.Count<Network.BundleItem>((Func<Network.BundleItem, bool>) (item => item.ItemType == ProductType.PRODUCT_TYPE_LUCKY_DRAW)) <= 0)
      return;
    TelemetryManager.Client()?.SendLuckyDrawEventMessage("LuckyDrawPurchaseSucceeded");
  }

  private void OnSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (bundle.Items.Count<Network.BundleItem>((Func<Network.BundleItem, bool>) (item => item.ItemType == ProductType.PRODUCT_TYPE_LUCKY_DRAW)) <= 0)
      return;
    this.StartCoroutine(this.WaitForLicenseUpdate(onFinishedCallback: new Action(this.UpdateDataAndShowUnacknowledgedHammersPopup)));
  }

  private void OnFailedPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (bundle.Items.Count<Network.BundleItem>((Func<Network.BundleItem, bool>) (item => item.ItemType == ProductType.PRODUCT_TYPE_LUCKY_DRAW)) <= 0)
      return;
    TelemetryManager.Client()?.SendLuckyDrawEventMessage("LuckyDrawPurchaseFailed");
  }

  private IEnumerator WaitForLicenseUpdate(float timeout = 15f, Action onFinishedCallback = null)
  {
    float cancelTime = Time.time + timeout;
    while (!this.m_luckyDrawManager.GetLuckyDrawButtonDataModel().BattlePassPurchased)
    {
      if ((double) Time.time > (double) cancelTime)
      {
        Log.All.PrintError("Error [LuckyDrawWidget] WaitForLicenseUpdate() timeout triggered while waiting for license after successful purchase.");
        LuckyDrawUtils.ShowErrorAndReturnToLobby();
        yield break;
      }
      else
        yield return (object) null;
    }
    this.m_popupSwitcher.HidePopup(this.m_LuckyDrawShopWidgetReference);
    if (onFinishedCallback != null)
      onFinishedCallback();
  }

  private void UpdateDataAndShowUnacknowledgedHammersPopup() => this.m_luckyDrawManager.InitializeOrUpdateData(new Action(this.OnLuckyDrawDataInitializedOrUpdated));

  private void OnLuckyDrawDataInitializedOrUpdated() => this.StartCoroutine(this.ShowBattlegroundsUnacknowledgedBonusHammersPopUp());

  private IEnumerator ShowBattlegroundsUnacknowledgedBonusHammersPopUp()
  {
    LuckyDrawWidget luckyDrawWidget = this;
    if (luckyDrawWidget.m_luckyDrawManager.NumUnacknowledgedBonusHammers() > 0 && luckyDrawWidget.m_rewardPresenter != null)
    {
      while (luckyDrawWidget.m_rewardPresenter.IsShowingReward())
        yield return (object) new WaitForSeconds(0.1f);
      RewardScrollDataModel dataModel = new RewardScrollDataModel()
      {
        DisplayName = GameStrings.Get("GLUE_BACON_REWARD_BATTLE_BASH_BONUS_HAMMERS"),
        Description = GameStrings.Get("GLUE_BACON_REWARD_BATTLE_BASH_EARN_MORE_HAMMERS_DESC"),
        RewardList = new RewardListDataModel()
        {
          Items = new DataModelList<RewardItemDataModel>()
          {
            new RewardItemDataModel()
            {
              Quantity = 1,
              ItemType = RewardItemType.BATTLEGROUNDS_BATTLE_BASH_HAMMER,
              BattlegroundsBattleBashHammer = new BattlegroundsBattleBashHammerDataModel()
              {
                NumHammers = luckyDrawWidget.m_luckyDrawManager.NumUnacknowledgedBonusHammers()
              }
            }
          }
        }
      };
      luckyDrawWidget.m_rewardPresenter.EnqueueReward(dataModel, (Action) (() => { }));
      luckyDrawWidget.m_rewardPresenter.ShowNextReward(new Action(luckyDrawWidget.OnLuckyDrawPopupDismissed));
    }
  }

  private void OnLuckyDrawPopupDismissed() => this.m_luckyDrawManager.AcknowledgeAllHammers();

  public void Open() => Shop.Get().RefreshDataModel();

  public void BlockInterface(bool blocked)
  {
  }

  public bool IsReady() => true;

  public bool IsOpen() => this.m_isStoreOpen;

  public void Unload()
  {
  }

  public IEnumerable<CurrencyType> GetVisibleCurrencies()
  {
    CurrencyType currencyType;
    if (!ShopUtils.TryGetMainVirtualCurrencyType(out currencyType))
      return (IEnumerable<CurrencyType>) new CurrencyType[0];
    return (IEnumerable<CurrencyType>) new CurrencyType[1]
    {
      currencyType
    };
  }
}
