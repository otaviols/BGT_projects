using Blizzard.T5.Services;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureStore : Store
{
  [CustomEditField(Sections = "UI")]
  public UIBButton m_BuyDungeonButton;
  [CustomEditField(Sections = "UI")]
  public UberText m_Headline;
  [CustomEditField(Sections = "UI")]
  public UberText m_DetailsText1;
  [CustomEditField(Sections = "UI")]
  public UberText m_DetailsText2;
  [CustomEditField(Sections = "UI")]
  public GameObject m_BuyWithMoneyButtonOpaqueCover;
  [CustomEditField(Sections = "UI")]
  public GameObject m_BuyWithGoldButtonOpaqueCover;
  [CustomEditField(Sections = "UI")]
  public GameObject m_BuyDungeonButtonOpaqueCover;
  [CustomEditField(Sections = "UI")]
  public UIBButton m_BackButton;
  [CustomEditField(Sections = "UI")]
  public WidgetInstance m_FullAdventureBundleCurrencyIcon;
  private bool m_animating;
  private Network.Bundle m_bundle;
  private Network.Bundle m_fullAdventureBundle;

  protected override void Start()
  {
    base.Start();
    if ((UnityEngine.Object) this.m_BuyDungeonButton != (UnityEngine.Object) null)
      this.m_BuyDungeonButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBuyDungeonButtonReleased));
    if ((UnityEngine.Object) this.m_offClicker != (UnityEngine.Object) null)
      this.m_offClicker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonReleased));
    if (!((UnityEngine.Object) this.m_BackButton != (UnityEngine.Object) null))
      return;
    this.m_BackButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonReleased));
  }

  public void SetAdventureProduct(
    ProductType productItemType,
    int productData,
    int numItemsRequired,
    ProductId productId)
  {
    if (productId.IsValid())
    {
      this.m_bundle = (Network.Bundle) null;
      Network.Bundle fromPmtProductId = StoreManager.Get().GetBundleFromPmtProductId(productId);
      if ((Record) fromPmtProductId == (Record) null)
        Log.Store.PrintWarning("AdventureStore.SetAdventureProduct(): could not find bundle with PMT Product ID {0}", (object) productId);
      else if (!StoreManager.DoesBundleContainProduct(fromPmtProductId, productItemType, productData, numItemsRequired))
        Log.Store.PrintWarning("AdventureStore.SetAdventureProduct(): bundle with PMT product ID {0} does not meet the expected criteria! productItemType: {1}  productData: {2}  numItemsRequired: {3}", (object) productId, (object) productItemType, (object) productData, (object) numItemsRequired);
      else if (!StoreManager.Get().IsBundleAvailableNow(fromPmtProductId))
        Log.Store.PrintWarning("AdventureStore.SetAdventureProduct(): bundle with PMT product ID {0} is not available now!", (object) productId);
      else if (StoreManager.Get().IsProductAlreadyOwned(fromPmtProductId))
        Log.Store.PrintWarning("AdventureStore.SetAdventureProduct(): bundle with PMT product ID {0} contains already owned content!", (object) productId);
      else
        this.m_bundle = fromPmtProductId;
    }
    else
    {
      List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAvailableBundlesForProduct(productItemType, numItemsRequired > 1, productData, numItemsRequired);
      if (bundlesForProduct.Count == 1)
      {
        this.m_bundle = bundlesForProduct[0];
      }
      else
      {
        Debug.LogWarningFormat("AdventureStore.SetAdventureProduct(): expected to find 1 available bundle for productItemType {0} productData {1} numItemsRequired {2}, found {3}", (object) productItemType, (object) productData, (object) numItemsRequired, (object) bundlesForProduct.Count);
        this.m_bundle = (Network.Bundle) null;
      }
    }
    string productName = StoreManager.Get().GetProductName(this.m_bundle);
    if ((UnityEngine.Object) this.m_Headline != (UnityEngine.Object) null)
      this.m_Headline.Text = productName;
    string str1 = string.Empty;
    switch (productItemType)
    {
      case ProductType.PRODUCT_TYPE_NAXX:
        str1 = "NAXX";
        break;
      case ProductType.PRODUCT_TYPE_BRM:
        str1 = "BRM";
        break;
      case ProductType.PRODUCT_TYPE_LOE:
        str1 = "LOE";
        break;
      case ProductType.PRODUCT_TYPE_WING:
        str1 = GameUtils.GetAdventureProductStringKey(productData);
        break;
    }
    string nameShort = (string) GameDbf.Wing.GetRecord(productData).NameShort;
    string str2 = string.IsNullOrEmpty(nameShort) ? productName : nameShort;
    if ((UnityEngine.Object) this.m_DetailsText1 != (UnityEngine.Object) null)
      this.m_DetailsText1.Text = GameStrings.Format(string.Format("GLUE_STORE_PRODUCT_DETAILS_{0}_PART_1", (object) str1), (object) str2);
    if ((UnityEngine.Object) this.m_DetailsText2 != (UnityEngine.Object) null)
      this.m_DetailsText2.Text = GameStrings.Format(string.Format("GLUE_STORE_PRODUCT_DETAILS_{0}_PART_2", (object) str1));
    AdventureDbId adventureIdByWingId = GameUtils.GetAdventureIdByWingId(productData);
    StoreManager.Get().GetAvailableAdventureBundle(adventureIdByWingId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out this.m_fullAdventureBundle);
    if ((Record) this.m_fullAdventureBundle == (Record) null)
      Log.Store.PrintWarning("Full adventure bundle not available.");
    this.BindProductDataModel(this.m_bundle);
  }

  public override void Hide()
  {
    this.m_shown = false;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    StoreManager.Get().RemoveAuthorizationExitListener(new Action(this.OnAuthExit));
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchase));
    this.EnableFullScreenEffects(false);
    this.DoHideAnimation();
  }

  public override void OnMoneySpent()
  {
    this.RefreshBuyButtonStates(this.m_bundle, (NoGTAPPTransactionData) null);
    this.RefreshBuyFullAdventureButton();
  }

  public override void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance) => this.RefreshBuyButtonStates(this.m_bundle, (NoGTAPPTransactionData) null);

  public override void OnCurrencyBalanceChanged(CurrencyBalanceChangedEventArgs args)
  {
    if (!ShopUtils.IsCurrencyVirtual(args.Currency))
      return;
    this.RefreshBuyButtonStates(this.m_bundle, (NoGTAPPTransactionData) null);
    this.RefreshBuyFullAdventureButton();
  }

  public override void Close()
  {
    this.Hide();
    this.FireExitEvent(false);
  }

  protected override void ShowImpl(bool isTotallyFake)
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    StoreManager.Get().RegisterAuthorizationExitListener(new Action(this.OnAuthExit));
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchase));
    this.EnableFullScreenEffects(true);
    this.SetUpBuyButtons();
    this.m_animating = true;
    this.DoShowAnimation((UIBPopup.OnAnimationComplete) (() =>
    {
      this.m_animating = false;
      this.FireOpenedEvent();
    }));
  }

  protected override void BuyWithGold(UIEvent e)
  {
    if (this.m_animating)
      Log.Store.Print("AdventureStore.BuyWithGold failed: still animating");
    else if ((Record) this.m_bundle == (Record) null)
      Log.Store.PrintError("AdventureStore.BuyWithGold failed: Bundle is null");
    else
      this.FireBuyWithGoldEventGTAPP(this.m_bundle, 1);
  }

  protected override void BuyWithMoney(UIEvent e)
  {
    if (this.m_animating)
      Log.Store.Print("AdventureStore.BuyWithMoney failed: still animating");
    else if ((Record) this.m_bundle == (Record) null)
      Log.Store.PrintError("AdventureStore.BuyWithMoney failed: Bundle is null");
    else
      this.FireBuyWithMoneyEvent(this.m_bundle, 1);
  }

  protected override void BuyWithVirtualCurrency(UIEvent e)
  {
    if (this.m_animating)
      Log.Store.Print("AdventureStore.BuyWithVirtualCurrency failed: still animating");
    else if ((Record) this.m_bundle == (Record) null)
      Log.Store.PrintError("AdventureStore.BuyWithVirtualCurrency failed: Bundle is null");
    else
      this.FireBuyWithVirtualCurrencyEvent(this.m_bundle, ShopUtils.GetBundleVirtualCurrencyPriceType(this.m_bundle));
  }

  protected override void RefreshBuyButtonStates(
    Network.Bundle bundle,
    NoGTAPPTransactionData transaction)
  {
    base.RefreshBuyButtonStates(bundle, transaction);
    if ((UnityEngine.Object) this.m_BuyWithMoneyButtonOpaqueCover != (UnityEngine.Object) null)
    {
      int num = !((UnityEngine.Object) this.m_buyWithMoneyButton != (UnityEngine.Object) null) ? 0 : (this.m_buyWithMoneyButton.gameObject.activeInHierarchy ? 1 : 0);
      bool flag1 = (UnityEngine.Object) this.m_buyWithVCButton != (UnityEngine.Object) null && this.m_buyWithVCButton.gameObject.activeInHierarchy;
      bool flag2 = false;
      if (num != 0 && this.GetMoneyButtonState() == Store.BuyButtonState.DISABLED_NO_TOOLTIP)
        flag2 = true;
      if (flag1 && this.GetVCButtonState() == Store.BuyButtonState.DISABLED_NO_TOOLTIP)
        flag2 = true;
      this.m_BuyWithMoneyButtonOpaqueCover.SetActive(flag2);
    }
    if ((UnityEngine.Object) this.m_BuyWithGoldButtonOpaqueCover != (UnityEngine.Object) null)
      this.m_BuyWithGoldButtonOpaqueCover.SetActive(this.GetGoldButtonState() == Store.BuyButtonState.DISABLED_NO_TOOLTIP);
    this.RefreshBuyFullAdventureButton();
  }

  private void OnAuthExit()
  {
    this.BlockInterface(false);
    LayerUtils.SetLayer(this.gameObject, GameLayer.Default);
    this.EnableFullScreenEffects(false);
    StoreManager.Get().RemoveAuthorizationExitListener(new Action(this.OnAuthExit));
    this.FireExitEvent(true);
    this.Hide();
  }

  private void OnSuccessfulPurchase(Network.Bundle bundle, PaymentMethod method)
  {
    this.BlockInterface(false);
    this.EnableFullScreenEffects(false);
    this.FireExitEvent(true);
    this.Hide();
  }

  private void SetUpBuyButtons()
  {
    this.SetUpBuyWithGoldButton();
    this.SetUpBuyWithMoneyButton();
    this.SetUpBuyFullAdventureButton();
    this.RefreshBuyButtonStates(this.m_bundle, (NoGTAPPTransactionData) null);
  }

  private void SetUpBuyWithGoldButton()
  {
    string empty = string.Empty;
    string text;
    if ((Record) this.m_bundle != (Record) null)
    {
      text = this.m_bundle.GtappGoldCost.ToString();
    }
    else
    {
      Debug.LogWarning((object) "AdventureStore.SetUpBuyWithGoldButton(): m_bundle is null");
      text = GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA");
    }
    if (!((UnityEngine.Object) this.m_buyWithGoldButton != (UnityEngine.Object) null))
      return;
    this.m_buyWithGoldButton.SetText(text);
  }

  private void SetUpBuyWithMoneyButton()
  {
    string empty = string.Empty;
    string text;
    if ((Record) this.m_bundle != (Record) null)
    {
      text = StoreManager.Get().FormatCostBundle(this.m_bundle);
    }
    else
    {
      Debug.LogWarning((object) "AdventureStore.SetUpBuyWithMoneyButton(): m_bundle is null");
      text = GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA");
    }
    this.m_buyWithMoneyButton.SetText(text);
  }

  private void SetUpBuyFullAdventureButton() => this.RefreshBuyFullAdventureButton();

  private void RefreshBuyFullAdventureButton()
  {
    long? nullable;
    if ((Record) this.m_fullAdventureBundle != (Record) null && !StoreManager.Get().CanBuyBundle(this.m_fullAdventureBundle))
    {
      Logger store = Log.Store;
      object[] objArray = new object[1];
      nullable = this.m_fullAdventureBundle.PMTProductID;
      objArray[0] = (object) nullable.GetValueOrDefault();
      store.PrintWarning("CanBuyBundle is false for m_fullAdventureBundle, PMTProductID = {0}", objArray);
      this.m_fullAdventureBundle = (Network.Bundle) null;
    }
    string empty = string.Empty;
    bool flag = false;
    string str = (string) null;
    CurrencyType currencyType = CurrencyType.NONE;
    long num = 0;
    if ((Record) this.m_fullAdventureBundle != (Record) null)
    {
      currencyType = ShopUtils.GetBundleVirtualCurrencyPriceType(this.m_fullAdventureBundle);
      if (currencyType != CurrencyType.NONE)
      {
        nullable = this.m_fullAdventureBundle.VirtualCurrencyCost;
        num = nullable.GetValueOrDefault();
        switch (currencyType - 3)
        {
          case CurrencyType.NONE:
          case CurrencyType.CN_RUNESTONES:
            str = GameStrings.Format("GLUE_SHOP_PRICE_RUNESTONES", (object) num);
            break;
          case CurrencyType.DUST:
            str = GameStrings.Format("GLUE_SHOP_PRICE_ARCANE_ORBS", (object) num);
            break;
        }
      }
      else
      {
        nullable = this.m_fullAdventureBundle.Cost;
        if (nullable.HasValue)
          str = StoreManager.Get().FormatCostBundle(this.m_fullAdventureBundle);
      }
    }
    string text;
    if (str != null)
    {
      text = string.Format("{0}\n{1}", (object) GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_TEXT"), (object) GameStrings.Format("GLUE_STORE_DUNGEON_BUTTON_COST_TEXT", (object) this.m_fullAdventureBundle.Items.Count, (object) str));
    }
    else
    {
      flag = true;
      text = string.Empty;
    }
    if ((UnityEngine.Object) this.m_FullAdventureBundleCurrencyIcon != (UnityEngine.Object) null)
      this.m_FullAdventureBundleCurrencyIcon.BindDataModel((IDataModel) new PriceDataModel()
      {
        Currency = currencyType,
        Amount = (float) num,
        DisplayText = num.ToString()
      }, false);
    if ((UnityEngine.Object) this.m_BuyDungeonButton != (UnityEngine.Object) null)
      this.m_BuyDungeonButton.SetText(text);
    if ((UnityEngine.Object) this.m_BuyDungeonButtonOpaqueCover != (UnityEngine.Object) null)
      this.m_BuyDungeonButtonOpaqueCover.SetActive(flag);
    if (!((UnityEngine.Object) this.m_BuyDungeonButton != (UnityEngine.Object) null))
      return;
    this.m_BuyDungeonButton.SetEnabled(!flag);
  }

  private void OnBuyDungeonButtonReleased(UIEvent e)
  {
    if (this.m_animating)
      Log.Store.Print("AdventureStore.OnBuyDungeonButtonReleased failed: still animating");
    else if ((Record) this.m_fullAdventureBundle == (Record) null)
    {
      Log.Store.PrintError("AdventureStore.OnBuyDungeonButtonReleased failed: m_fullAdventureBundle is null");
    }
    else
    {
      CurrencyType currencyPriceType = ShopUtils.GetBundleVirtualCurrencyPriceType(this.m_fullAdventureBundle);
      if (currencyPriceType != CurrencyType.NONE)
        this.FireBuyWithVirtualCurrencyEvent(this.m_fullAdventureBundle, currencyPriceType);
      else if (this.m_fullAdventureBundle.Cost.HasValue)
        this.FireBuyWithMoneyEvent(this.m_fullAdventureBundle, 1);
      else
        Log.Store.PrintError("AdventureStore.OnBuyDungeonButtonReleased failed: no valid price on m_fullAdventureBundle. PMT ID = {0}", (object) this.m_fullAdventureBundle.PMTProductID.GetValueOrDefault());
    }
  }

  private void OnBackButtonReleased(UIEvent e)
  {
    HearthstoneCheckout service;
    if (ServiceManager.TryGet<HearthstoneCheckout>(out service) && service.IsInProgress)
      return;
    this.Close();
  }

  private bool OnNavigateBack()
  {
    this.Close();
    return true;
  }
}
