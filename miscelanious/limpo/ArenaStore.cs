using Hearthstone.Commerce;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ArenaStore : Store
{
  public UIBButton m_backButton;
  public GameObject m_storeClosed;
  private static readonly int NUM_BUNDLE_ITEMS_REQUIRED = 1;
  private NoGTAPPTransactionData m_goldTransactionData;
  private Network.Bundle m_bundle;
  private static ArenaStore s_instance;

  protected override void Start()
  {
    base.Start();
    this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackPressed));
  }

  protected override void Awake()
  {
    ArenaStore.s_instance = this;
    this.m_destroyOnSceneLoad = false;
    base.Awake();
    this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
  }

  protected override void OnDestroy()
  {
    ArenaStore.s_instance = (ArenaStore) null;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
  }

  public static ArenaStore Get() => ArenaStore.s_instance;

  public static Network.Bundle GetDraftTicketProduct()
  {
    List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAvailableBundlesForProduct(ProductType.PRODUCT_TYPE_DRAFT, true, numItemsRequired: ArenaStore.NUM_BUNDLE_ITEMS_REQUIRED);
    if (bundlesForProduct.Count == 1)
    {
      Log.Store.PrintDebug("Arena Ticket Product found. PMT ID = {0}, Name = {1}", (object) bundlesForProduct[0].PMTProductID.GetValueOrDefault(), (object) bundlesForProduct[0].GetTitle());
      return bundlesForProduct[0];
    }
    if (bundlesForProduct.Count == 0)
      Log.Store.PrintError("Arena Ticket Product not found!");
    else
      Log.Store.PrintError("Multiple Arena Ticket Products found!");
    return (Network.Bundle) null;
  }

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

  public override void OnMoneySpent() => this.RefreshBuyButtonStates(this.m_bundle, this.m_goldTransactionData);

  public override void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance) => this.RefreshBuyButtonStates(this.m_bundle, this.m_goldTransactionData);

  public override void OnCurrencyBalanceChanged(CurrencyBalanceChangedEventArgs args)
  {
    if (!ShopUtils.IsCurrencyVirtual(args.Currency))
      return;
    this.RefreshBuyButtonStates(this.m_bundle, this.m_goldTransactionData);
  }

  protected override void ShowImpl(bool isTotallyFake)
  {
    this.m_shown = true;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    StoreManager.Get().RegisterAuthorizationExitListener(new Action(this.OnAuthExit));
    this.EnableFullScreenEffects(true);
    this.FindTicketProduct();
    this.SetUpBuyButtons();
    ShownUIMgr.Get().SetShownUI(ShownUIMgr.UI_WINDOW.ARENA_STORE);
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
    if (this.m_goldTransactionData == null)
      return;
    this.FireBuyWithGoldEventNoGTAPP(this.m_goldTransactionData);
  }

  protected override void BuyWithMoney(UIEvent e)
  {
    if ((Record) this.m_bundle == (Record) null)
      Log.Store.PrintError("ArenaStore.BuyWithMoney failed. Arena ticket product not found");
    else
      this.FireBuyWithMoneyEvent(this.m_bundle, 1);
  }

  protected override void BuyWithVirtualCurrency(UIEvent e)
  {
    if ((Record) this.m_bundle == (Record) null)
      Log.Store.PrintError("ArenaStore.BuyWithVirtualCurrency failed. Arena ticket product not found");
    else
      this.FireBuyWithVirtualCurrencyEvent(this.m_bundle, ShopUtils.GetBundleVirtualCurrencyPriceType(this.m_bundle));
  }

  private void OnAuthExit()
  {
    Navigation.Pop();
    this.ExitForgeStore(true);
  }

  private void OnBackPressed(UIEvent e) => Navigation.GoBack();

  private bool OnNavigateBack()
  {
    this.ExitForgeStore(false);
    return true;
  }

  private void ExitForgeStore(bool authorizationBackButtonPressed)
  {
    this.BlockInterface(false);
    LayerUtils.SetLayer(this.gameObject, GameLayer.Default);
    this.EnableFullScreenEffects(false);
    StoreManager.Get().RemoveAuthorizationExitListener(new Action(this.OnAuthExit));
    this.FireExitEvent(authorizationBackButtonPressed);
  }

  private void SetUpBuyButtons()
  {
    this.SetUpBuyWithGoldButton();
    this.SetUpBuyWithMoneyButton();
    this.RefreshBuyButtonStates(this.m_bundle, this.m_goldTransactionData);
  }

  private void SetUpBuyWithGoldButton()
  {
    NoGTAPPTransactionData noGTAPPTransactionData = new NoGTAPPTransactionData()
    {
      Product = ProductType.PRODUCT_TYPE_DRAFT,
      ProductData = 0,
      Quantity = 1
    };
    long cost;
    string text;
    if (StoreManager.Get().GetGoldCostNoGTAPP(noGTAPPTransactionData, out cost))
    {
      this.m_goldTransactionData = noGTAPPTransactionData;
      text = cost.ToString();
    }
    else
    {
      Debug.LogWarning((object) "ForgeStore.SetUpBuyWithGoldButton(): no gold price for purchase Arena without GTAPP");
      text = GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA");
    }
    this.m_buyWithGoldButton.SetText(text);
  }

  private void FindTicketProduct()
  {
    this.m_bundle = ArenaStore.GetDraftTicketProduct();
    if ((Record) this.m_bundle == (Record) null)
      return;
    this.BindProductDataModel(this.m_bundle);
  }

  private void SetUpBuyWithMoneyButton() => this.m_buyWithMoneyButton.SetText(!((Record) this.m_bundle != (Record) null) ? GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA") : StoreManager.Get().FormatCostBundle(this.m_bundle));
}
