using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TavernBrawlStore : Store
{
  public UIBButton m_ContinueButton;
  public UIBButton m_backButton;
  public GameObject m_storeClosed;
  public PlayMakerFSM m_ButtonFlipper;
  public PlayMakerFSM m_PaperEffect;
  public UberText m_EndsInTextPaper;
  public UberText m_EndsInTextChalk;
  public UberText m_ChalkboardTitleText;
  public UberText m_ChalkboardDescriptionText;
  public MeshRenderer m_ChalkboardMesh;
  private static readonly int NUM_BUNDLE_ITEMS_REQUIRED = 1;
  private Network.Bundle m_bundle;
  private static TavernBrawlStore s_instance;

  protected override void Start()
  {
    base.Start();
    this.m_ContinueButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnContinuePressed));
    this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackPressed));
  }

  protected override void Awake()
  {
    TavernBrawlStore.s_instance = this;
    this.m_destroyOnSceneLoad = false;
    base.Awake();
    this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
    this.m_infoButton.GetComponent<BoxCollider>().enabled = false;
  }

  protected override void OnDestroy() => TavernBrawlStore.s_instance = (TavernBrawlStore) null;

  public static TavernBrawlStore Get() => TavernBrawlStore.s_instance;

  public override void Hide()
  {
    if (ShownUIMgr.Get() != null)
      ShownUIMgr.Get().ClearShownUI();
    FriendChallengeMgr.Get().OnStoreClosed();
    StoreManager.Get().RemoveAuthorizationExitListener(new Action(this.OnAuthExit));
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.EnableFullScreenEffects(false);
    base.Hide();
  }

  public override void OnMoneySpent() => this.UpdateMoneyButtonState();

  public override void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance) => this.UpdateGoldButtonState(balance);

  protected override void ShowImpl(bool isTotallyFake)
  {
    this.m_shown = true;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    StoreManager.Get().RegisterAuthorizationExitListener(new Action(this.OnAuthExit));
    this.EnableFullScreenEffects(true);
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(TavernBrawlManager.Get().CurrentMission().missionId);
    this.m_ChalkboardTitleText.Text = (string) record.Name;
    this.m_ChalkboardDescriptionText.Text = (string) (!(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty((string) record.ShortDescription) ? record.Description : record.ShortDescription);
    string endingTimeText = TavernBrawlManager.Get().EndingTimeText;
    this.m_EndsInTextPaper.Text = endingTimeText;
    this.m_EndsInTextChalk.Text = endingTimeText;
    if ((UnityEngine.Object) this.m_ChalkboardMesh != (UnityEngine.Object) null)
    {
      Material sharedMaterial = this.m_ChalkboardMesh.GetSharedMaterial();
      if ((UnityEngine.Object) sharedMaterial != (UnityEngine.Object) null)
        sharedMaterial.SetTexture("_MainTex", TavernBrawlDisplay.Get().m_chalkboardTexture);
    }
    this.BindTavernBrawlData();
    this.BindTicketProduct();
    this.SetUpBuyButtons();
    ShownUIMgr.Get().SetShownUI(ShownUIMgr.UI_WINDOW.TAVERN_BRAWL_STORE);
    FriendChallengeMgr.Get().OnStoreOpened();
    this.DoShowAnimation((UIBPopup.OnAnimationComplete) (() =>
    {
      if (isTotallyFake)
      {
        this.SilenceBuyButtons();
        this.m_infoButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(((Store) this).OnInfoPressed));
      }
      this.FireOpenedEvent();
    }));
  }

  protected override void BuyWithGold(UIEvent e)
  {
    if ((Record) this.m_bundle == (Record) null)
      Log.Store.PrintError("TavernBrawlStore.BuyWithGold failed. Brawl ticket product not found");
    else
      this.FireBuyWithGoldEventGTAPP(this.m_bundle, 1);
  }

  protected override void BuyWithMoney(UIEvent e)
  {
    if ((Record) this.m_bundle == (Record) null)
      Log.Store.PrintError("TavernBrawlStore.BuyWithMoney failed. Brawl ticket product not found");
    else
      this.FireBuyWithMoneyEvent(this.m_bundle, 1);
  }

  protected override void BuyWithVirtualCurrency(UIEvent e)
  {
    if ((Record) this.m_bundle == (Record) null)
      Log.Store.PrintError("TavernBrawlStore.BuyWithVirtualCurrency failed. Brawl ticket product not found");
    else
      this.FireBuyWithVirtualCurrencyEvent(this.m_bundle, ShopUtils.GetBundleVirtualCurrencyPriceType(this.m_bundle));
  }

  private void OnAuthExit()
  {
    Navigation.Pop();
    this.ExitTavernBrawlStore(true);
  }

  private void OnBackPressed(UIEvent e) => Navigation.GoBack();

  private void OnContinuePressed(UIEvent e)
  {
    this.m_ButtonFlipper.SendEvent("Flip");
    this.m_PaperEffect.SendEvent("BurnAway");
    this.m_infoButton.GetComponent<BoxCollider>().enabled = true;
    int sessionCount = (int) TavernBrawlManager.Get().CurrentSession.SessionCount;
    int freeSessions = (int) TavernBrawlManager.Get().CurrentMission().FreeSessions;
    if (!TavernBrawlManager.Get().IsEligibleForFreeTicket())
      return;
    this.SetMoneyButtonState(Store.BuyButtonState.DISABLED);
    this.SetGoldButtonState(Store.BuyButtonState.DISABLED);
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_BRAWLISEUM"),
      m_text = GameStrings.Get("GLUE_BRAWLISEUM_FREE_TICKET_BODY"),
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnFreePopupClosed)
    });
  }

  private void OnFreePopupClosed(AlertPopup.Response response, object userData) => TavernBrawlManager.Get().RequestSessionBegin();

  private bool OnNavigateBack()
  {
    this.ExitTavernBrawlStore(false);
    return true;
  }

  private void ExitTavernBrawlStore(bool authorizationBackButtonPressed)
  {
    this.BlockInterface(false);
    LayerUtils.SetLayer(this.gameObject, GameLayer.Default);
    this.EnableFullScreenEffects(false);
    StoreManager.Get().RemoveAuthorizationExitListener(new Action(this.OnAuthExit));
    this.FireExitEvent(authorizationBackButtonPressed);
  }

  private void UpdateMoneyButtonState()
  {
    Store.BuyButtonState state = Store.BuyButtonState.ENABLED;
    if ((Record) this.m_bundle == (Record) null || !StoreManager.Get().IsOpen())
    {
      state = Store.BuyButtonState.DISABLED;
      this.m_storeClosed.SetActive(true);
    }
    else if (!StoreManager.Get().IsBattlePayFeatureEnabled())
      state = Store.BuyButtonState.DISABLED_FEATURE;
    else if (StoreManager.Get().IsPromptShowing)
    {
      state = Store.BuyButtonState.DISABLED;
      this.SetGoldButtonState(state);
    }
    else
      this.m_storeClosed.SetActive(false);
    this.SetMoneyButtonState(state);
  }

  private void UpdateGoldButtonState(NetCache.NetCacheGoldBalance balance)
  {
    Store.BuyButtonState state = Store.BuyButtonState.ENABLED;
    if (StoreManager.Get().IsPromptShowing)
    {
      state = Store.BuyButtonState.DISABLED;
      this.SetMoneyButtonState(state);
    }
    else if ((Record) this.m_bundle == (Record) null)
      state = Store.BuyButtonState.DISABLED;
    else if (!StoreManager.Get().IsOpen())
      state = Store.BuyButtonState.DISABLED;
    else if (!StoreManager.Get().IsBuyWithGoldFeatureEnabled())
      state = Store.BuyButtonState.DISABLED_FEATURE;
    else if (!ShopUtils.BundleHasPrice(this.m_bundle, CurrencyType.GOLD) || !StoreManager.Get().IsBundleAvailableNow(this.m_bundle))
      state = Store.BuyButtonState.DISABLED_NO_TOOLTIP;
    else if (balance == null)
      state = Store.BuyButtonState.DISABLED;
    else if (balance.GetTotal() < this.m_bundle.GtappGoldCost.Value)
      state = Store.BuyButtonState.DISABLED_NOT_ENOUGH_GOLD;
    this.SetGoldButtonState(state);
  }

  private void BindTavernBrawlData()
  {
    WidgetTemplate componentOnSelfOrParent = GameObjectUtils.GetComponentOnSelfOrParent<WidgetTemplate>(this.transform);
    if (!((UnityEngine.Object) componentOnSelfOrParent != (UnityEngine.Object) null))
      return;
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(TavernBrawlManager.Get().CurrentMission().missionId);
    TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
    TavernBrawlDetailsDataModel detailsDataModel = new TavernBrawlDetailsDataModel()
    {
      BrawlType = mission.BrawlType,
      BrawlMode = mission.brawlMode,
      FormatType = mission.formatType,
      TicketType = mission.ticketType,
      MaxWins = mission.maxWins,
      MaxLosses = mission.maxLosses,
      PopupType = mission.tavernBrawlSpec.StorePopupType,
      Title = (string) record.Name,
      RulesDesc = (string) (!(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty((string) record.ShortDescription) ? record.Description : record.ShortDescription),
      RewardDesc = mission.tavernBrawlSpec.RewardDesc,
      MinRewardDesc = mission.tavernBrawlSpec.MinRewardDesc,
      MaxRewardDesc = mission.tavernBrawlSpec.MaxRewardDesc,
      EndConditionDesc = mission.tavernBrawlSpec.EndConditionDesc
    };
    componentOnSelfOrParent.BindDataModel((IDataModel) detailsDataModel, false);
  }

  private void BindTicketProduct()
  {
    int ticketType = TavernBrawlManager.Get().CurrentMission().ticketType;
    List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAvailableBundlesForProduct(ProductType.PRODUCT_TYPE_TAVERN_BRAWL_TICKET, true, ticketType, TavernBrawlStore.NUM_BUNDLE_ITEMS_REQUIRED);
    if (bundlesForProduct.Count == 0)
    {
      this.m_bundle = (Network.Bundle) null;
    }
    else
    {
      this.m_bundle = bundlesForProduct[0];
      this.BindProductDataModel(this.m_bundle);
    }
  }

  private void SetUpBuyButtons()
  {
    this.SetUpBuyWithGoldButton();
    this.SetUpBuyWithMoneyButton();
  }

  private void SetUpBuyWithGoldButton()
  {
    long? gtappGoldCost;
    if ((Record) this.m_bundle != (Record) null)
    {
      gtappGoldCost = this.m_bundle.GtappGoldCost;
      if (gtappGoldCost.HasValue)
      {
        this.UpdateGoldButtonState(NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>());
        return;
      }
    }
    object[] objArray = new object[2]
    {
      (Record) this.m_bundle == (Record) null ? (object) "<null>" : (object) "<not null>",
      null
    };
    string str;
    if (!((Record) this.m_bundle == (Record) null))
    {
      gtappGoldCost = this.m_bundle.GtappGoldCost;
      if (gtappGoldCost.HasValue)
      {
        gtappGoldCost = this.m_bundle.GtappGoldCost;
        str = gtappGoldCost.Value.ToString();
        goto label_7;
      }
    }
    str = "<no value>";
label_7:
    objArray[1] = (object) str;
    Debug.LogWarningFormat("TavernBrawlStore.SetUpBuyWithGoldButton(): no gold cost (bundle={0} hasGoldCost={1})", objArray);
    this.SetGoldButtonState(Store.BuyButtonState.DISABLED);
  }

  private void SetUpBuyWithMoneyButton()
  {
    if ((Record) this.m_bundle != (Record) null)
    {
      this.UpdateMoneyButtonState();
    }
    else
    {
      Debug.LogWarning((object) "TavernBrawlStore.SetUpBuyWithMoneyButton(): m_bundle is null");
      this.SetMoneyButtonState(Store.BuyButtonState.DISABLED);
    }
  }
}
