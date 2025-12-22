using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.AssetManager;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ShopUtils
{
  private static Dbf<BoosterDbfRecord> s_boosters;
  private static Dbf<AdventureDbfRecord> s_adventures;

  public static Dbf<BoosterDbfRecord> Boosters => GameDbf.Booster != null && Application.isPlaying ? GameDbf.Booster : ShopUtils.s_boosters ?? (ShopUtils.s_boosters = Dbf<BoosterDbfRecord>.Load("BOOSTER", DbfFormat.XML));

  public static Dbf<AdventureDbfRecord> Adventures => GameDbf.Adventure != null && Application.isPlaying ? GameDbf.Adventure : ShopUtils.s_adventures ?? (ShopUtils.s_adventures = Dbf<AdventureDbfRecord>.Load("ADVENTURE", DbfFormat.XML));

  public static bool ShouldDisplayTier(ProductTierDataModel tier, int count)
  {
    if (tier.BrowserButtons.Count <= 0)
      return false;
    return ShopUtils.TierHasShowIfAllOwnedTag(tier) || ShopUtils.ShouldDisplayButtons(tier.BrowserButtons.Take<ShopBrowserButtonDataModel>(count));
  }

  public static AssetHandle<GameObject> LoadStorePackPrefab(
    BoosterDbId boosterId)
  {
    BoosterDbfRecord record = ShopUtils.Boosters.GetRecord((int) boosterId);
    return record == null || string.IsNullOrEmpty(record.StorePrefab) ? (AssetHandle<GameObject>) null : AssetLoader.Get().LoadAsset<GameObject>((AssetReference) record.StorePrefab);
  }

  public static AssetHandle<GameObject> LoadStoreAdventurePrefab(
    AdventureDbId adventureId)
  {
    AdventureDbfRecord record = ShopUtils.Adventures.GetRecord((int) adventureId);
    return record == null || string.IsNullOrEmpty(record.StorePrefab) ? (AssetHandle<GameObject>) null : AssetLoader.Get().LoadAsset<GameObject>((AssetReference) record.StorePrefab);
  }

  public static long GetCachedBalance(CurrencyType currencyType)
  {
    if (ShopUtils.IsCurrencyVirtual(currencyType) && (!ShopUtils.IsVirtualCurrencyEnabled() || !ShopUtils.IsVirtualCurrencyTypeEnabled(currencyType)))
      return 0;
    switch (currencyType)
    {
      case CurrencyType.GOLD:
        return NetCache.Get() == null ? 0L : NetCache.Get().GetGoldBalance();
      case CurrencyType.DUST:
        return NetCache.Get() == null ? 0L : NetCache.Get().GetArcaneDustBalance();
      case CurrencyType.CN_RUNESTONES:
      case CurrencyType.CN_ARCANE_ORBS:
      case CurrencyType.ROW_RUNESTONES:
        IDataModel model;
        GlobalDataContext.Get().GetDataModel(24, out model);
        if (!(model is ShopDataModel shopDataModel))
          return 0;
        PriceDataModel priceDataModel = (PriceDataModel) null;
        if (ShopUtils.IsMainVirtualCurrencyType(currencyType))
          priceDataModel = shopDataModel.VirtualCurrencyBalance;
        else if (ShopUtils.IsBoosterVirtualCurrencyType(currencyType))
          priceDataModel = shopDataModel.BoosterCurrencyBalance;
        return priceDataModel != null ? (long) priceDataModel.Amount : 0L;
      case CurrencyType.RENOWN:
        return NetCache.Get() == null ? 0L : NetCache.Get().GetRenownBalance();
      default:
        Log.Store.PrintWarning("Unsupported currency type: {0}", (object) currencyType.ToString());
        return 0;
    }
  }

  public static long GetDeficit(PriceDataModel price)
  {
    if (price.Currency == CurrencyType.REAL_MONEY)
      return 0;
    long cachedBalance = ShopUtils.GetCachedBalance(price.Currency);
    long amount = (long) price.Amount;
    return amount > cachedBalance ? amount - cachedBalance : 0L;
  }

  public static ProductDataModel FindCurrencyProduct(CurrencyType currencyType)
  {
    ProductCatalog catalog = StoreManager.Get().Catalog;
    ProductDataModel currencyProduct = (ProductDataModel) null;
    if (ShopUtils.IsCurrencyVirtual(currencyType) && ShopUtils.IsVirtualCurrencyEnabled())
    {
      CurrencyType currencyType1;
      ShopUtils.TryGetMainVirtualCurrencyType(out currencyType1);
      if (currencyType == currencyType1)
      {
        currencyProduct = catalog.VirtualCurrencyProductItem;
      }
      else
      {
        CurrencyType currencyType2;
        ShopUtils.TryGetBoosterVirtualCurrencyType(out currencyType2);
        if (currencyType == currencyType2)
          currencyProduct = catalog.BoosterCurrencyProductItem;
      }
    }
    if (currencyProduct == null)
      Log.Store.PrintError(string.Format("Couldn't find product for currency type {0}", (object) currencyType));
    return currencyProduct;
  }

  public static ProductDataModel FindCurrencyProduct(
    CurrencyType currencyType,
    float requiredAmount)
  {
    ProductDataModel currencyProduct = ShopUtils.FindCurrencyProduct(currencyType);
    if (currencyProduct == null)
      return (ProductDataModel) null;
    ProductDataModel productDataModel1 = currencyProduct;
    ProductDataModel productDataModel2 = (ProductDataModel) null;
    float num1 = float.MinValue;
    float num2 = float.MinValue;
    foreach (ProductDataModel variant in currencyProduct.Variants)
    {
      if (variant.Availability == ProductAvailability.CAN_PURCHASE)
      {
        float currencyInProduct = ShopUtils.GetAmountOfCurrencyInProduct(variant, currencyType);
        float num3 = currencyInProduct - requiredAmount;
        if ((double) num3 >= 0.0 && (double) num1 < 0.0)
        {
          num1 = num3;
          productDataModel1 = variant;
        }
        else if ((double) Math.Abs(num3) < (double) Math.Abs(num1))
        {
          num1 = num3;
          productDataModel1 = variant;
        }
        if (variant.Tags.Contains("special_offer") && (double) currencyInProduct >= (double) requiredAmount)
        {
          float num4 = currencyInProduct - requiredAmount;
          if (productDataModel2 == null || (double) num4 < (double) num2)
          {
            productDataModel2 = variant;
            num2 = num4;
          }
        }
      }
    }
    return productDataModel2 ?? productDataModel1;
  }

  public static float GetAmountOfCurrencyInProduct(
    ProductDataModel product,
    CurrencyType currencyType)
  {
    RewardItemDataModel rewardItemDataModel = product.Items.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (i => i.Currency != null && i.Currency.Currency == currencyType));
    return rewardItemDataModel != null ? rewardItemDataModel.Currency.Amount : 0.0f;
  }

  public static string GetCurrencyCode(CurrencyType currency)
  {
    switch (currency)
    {
      case CurrencyType.GOLD:
        return "XSG";
      case CurrencyType.CN_RUNESTONES:
        return "XSA";
      case CurrencyType.REAL_MONEY:
        return StoreManager.Get().GetCurrencyCode();
      case CurrencyType.CN_ARCANE_ORBS:
        return "XSB";
      case CurrencyType.ROW_RUNESTONES:
        return "XSR";
      default:
        return "invalid";
    }
  }

  public static CurrencyType GetCurrencyTypeFromCode(string code)
  {
    switch (code)
    {
      case "":
      case null:
        return CurrencyType.NONE;
      case "XSA":
        return CurrencyType.CN_RUNESTONES;
      case "XSB":
        return CurrencyType.CN_ARCANE_ORBS;
      case "XSR":
        return CurrencyType.ROW_RUNESTONES;
      case "XSG":
        return CurrencyType.GOLD;
      default:
        return CurrencyType.REAL_MONEY;
    }
  }

  public static VirtualCurrencyMode GetVirtualCurrencyMode() => BattleNet.GetCurrentRegion() == BnetRegion.REGION_CN ? VirtualCurrencyMode.China : VirtualCurrencyMode.Default;

  public static bool IsVirtualCurrencyEnabled()
  {
    NetCache netCache = NetCache.Get();
    if (netCache == null)
      return false;
    NetCache.NetCacheFeatures netObject = netCache.GetNetObject<NetCache.NetCacheFeatures>();
    return netObject != null && netObject.Store.VirtualCurrencyEnabled;
  }

  public static bool IsCurrencyVirtual(CurrencyType currency)
  {
    switch (currency)
    {
      case CurrencyType.CN_RUNESTONES:
      case CurrencyType.CN_ARCANE_ORBS:
      case CurrencyType.ROW_RUNESTONES:
        return true;
      default:
        return false;
    }
  }

  public static bool IsVirtualCurrencyTypeEnabled(CurrencyType currencyType)
  {
    if (!ShopUtils.IsCurrencyVirtual(currencyType))
      return false;
    VirtualCurrencyMode virtualCurrencyMode = ShopUtils.GetVirtualCurrencyMode();
    switch (currencyType)
    {
      case CurrencyType.CN_RUNESTONES:
      case CurrencyType.CN_ARCANE_ORBS:
        return virtualCurrencyMode == VirtualCurrencyMode.China;
      case CurrencyType.ROW_RUNESTONES:
        return virtualCurrencyMode == VirtualCurrencyMode.Default;
      default:
        Log.Store.PrintError(string.Format("Cannot determine if Virtual Currency is enabled. Unknown Currency type - {0}.", (object) currencyType));
        return false;
    }
  }

  public static bool TryGetMainVirtualCurrencyType(out CurrencyType currencyType)
  {
    if (!BattleNet.IsConnected())
    {
      currencyType = CurrencyType.NONE;
      return false;
    }
    switch (ShopUtils.GetVirtualCurrencyMode())
    {
      case VirtualCurrencyMode.Default:
        currencyType = CurrencyType.ROW_RUNESTONES;
        return true;
      case VirtualCurrencyMode.China:
        currencyType = CurrencyType.CN_RUNESTONES;
        return true;
      default:
        currencyType = CurrencyType.NONE;
        return false;
    }
  }

  public static bool IsMainVirtualCurrencyType(CurrencyType currencyType) => currencyType == CurrencyType.CN_RUNESTONES || currencyType == CurrencyType.ROW_RUNESTONES;

  public static bool TryGetBoosterVirtualCurrencyType(out CurrencyType currencyType)
  {
    if (!BattleNet.IsConnected())
    {
      currencyType = CurrencyType.NONE;
      return false;
    }
    if (ShopUtils.GetVirtualCurrencyMode() == VirtualCurrencyMode.China)
    {
      currencyType = CurrencyType.CN_ARCANE_ORBS;
      return true;
    }
    currencyType = CurrencyType.NONE;
    return false;
  }

  public static bool IsBoosterVirtualCurrencyType(CurrencyType currencyType) => currencyType == CurrencyType.CN_ARCANE_ORBS;

  public static bool IsVirtualCurrencyRewardItemType(RewardItemType rewardItemType)
  {
    switch (rewardItemType)
    {
      case RewardItemType.CN_RUNESTONES:
      case RewardItemType.CN_ARCANE_ORBS:
      case RewardItemType.ROW_RUNESTONES:
        return true;
      default:
        return false;
    }
  }

  public static RewardItemType GetRewardItemTypeFromCurrencyType(
    CurrencyType currencyType)
  {
    switch (currencyType)
    {
      case CurrencyType.GOLD:
        return RewardItemType.GOLD;
      case CurrencyType.DUST:
        return RewardItemType.DUST;
      case CurrencyType.CN_RUNESTONES:
        return RewardItemType.CN_RUNESTONES;
      case CurrencyType.CN_ARCANE_ORBS:
        return RewardItemType.CN_ARCANE_ORBS;
      case CurrencyType.ROW_RUNESTONES:
        return RewardItemType.ROW_RUNESTONES;
      case CurrencyType.RENOWN:
        return RewardItemType.MERCENARY_RENOWN;
      default:
        return RewardItemType.UNDEFINED;
    }
  }

  public static bool BundleHasPrice(Network.Bundle bundle, CurrencyType currencyType)
  {
    switch (currencyType)
    {
      case CurrencyType.GOLD:
        return bundle.GtappGoldCost.HasValue;
      case CurrencyType.CN_RUNESTONES:
        return bundle.VirtualCurrencyCode == "XSA";
      case CurrencyType.REAL_MONEY:
        return bundle.Cost.HasValue;
      case CurrencyType.CN_ARCANE_ORBS:
        return bundle.VirtualCurrencyCode == "XSB";
      case CurrencyType.ROW_RUNESTONES:
        return bundle.VirtualCurrencyCode == "XSR";
      default:
        return false;
    }
  }

  public static bool BundleHasNonGoldPrice(Network.Bundle bundle) => bundle.Cost.HasValue || ShopUtils.IsCurrencyVirtual(ShopUtils.GetCurrencyTypeFromCode(bundle.VirtualCurrencyCode));

  public static CurrencyType GetBundleVirtualCurrencyPriceType(Network.Bundle bundle)
  {
    if ((Record) bundle == (Record) null)
      return CurrencyType.NONE;
    CurrencyType currencyTypeFromCode = ShopUtils.GetCurrencyTypeFromCode(bundle.VirtualCurrencyCode);
    return ShopUtils.IsCurrencyVirtual(currencyTypeFromCode) ? currencyTypeFromCode : CurrencyType.NONE;
  }

  public static bool TryGetBundlePrice(
    Network.Bundle bundle,
    CurrencyType currencyType,
    out long amount)
  {
    amount = 0L;
    if ((Record) bundle == (Record) null)
      return false;
    switch (currencyType)
    {
      case CurrencyType.GOLD:
        long? gtappGoldCost = bundle.GtappGoldCost;
        if (!gtappGoldCost.HasValue)
          return false;
        ref long local1 = ref amount;
        gtappGoldCost = bundle.GtappGoldCost;
        long num1 = gtappGoldCost.Value;
        local1 = num1;
        return true;
      case CurrencyType.CN_RUNESTONES:
      case CurrencyType.CN_ARCANE_ORBS:
      case CurrencyType.ROW_RUNESTONES:
        if (bundle.VirtualCurrencyCode != ShopUtils.GetCurrencyCode(currencyType))
          return false;
        amount = bundle.VirtualCurrencyCost.GetValueOrDefault();
        return true;
      case CurrencyType.REAL_MONEY:
        long? cost = bundle.Cost;
        if (!cost.HasValue)
          return false;
        ref long local2 = ref amount;
        cost = bundle.Cost;
        long num2 = cost.Value;
        local2 = num2;
        return true;
      default:
        return false;
    }
  }

  public static bool TryGetPriceFromBundleOrTransaction(
    Network.Bundle bundle,
    NoGTAPPTransactionData transaction,
    CurrencyType currencyType,
    out long price)
  {
    return transaction != null && currencyType == CurrencyType.GOLD ? StoreManager.Get().GetGoldCostNoGTAPP(transaction, out price) : ShopUtils.TryGetBundlePrice(bundle, currencyType, out price);
  }

  public static bool TryDecomposeBuyProductEventArgs(
    BuyProductEventArgs args,
    out ProductId productId,
    out string currencyCode,
    out long totalPrice,
    out int quantity,
    out string productItemType,
    out int productItemId)
  {
    productId = ProductId.InvalidProduct;
    currencyCode = (string) null;
    totalPrice = 0L;
    quantity = 0;
    productItemType = "";
    productItemId = 0;
    Network.Bundle bundle = (Network.Bundle) null;
    if (args == null)
      return false;
    quantity = args.quantity;
    ProductType productType;
    if (args is BuyPmtProductEventArgs productEventArgs)
    {
      productId = ProductId.CreateFrom(productEventArgs.pmtProductId);
      currencyCode = ShopUtils.GetCurrencyCode(productEventArgs.paymentCurrency);
      if (productId.IsValid())
        bundle = StoreManager.Get().GetBundleFromPmtProductId(productId);
    }
    else
    {
      if (!(args is BuyNoGTAPPEventArgs noGtappEventArgs))
        return false;
      ref string local = ref productItemType;
      productType = noGtappEventArgs.transactionData.Product;
      string lowerInvariant = productType.ToString().ToLowerInvariant();
      local = lowerInvariant;
      productItemId = noGtappEventArgs.transactionData.ProductData;
      currencyCode = ShopUtils.GetCurrencyCode(CurrencyType.GOLD);
      StoreManager.Get().GetGoldCostNoGTAPP(noGtappEventArgs.transactionData, out totalPrice);
    }
    if ((Record) bundle != (Record) null)
    {
      productId = ProductId.CreateFrom(bundle.PMTProductID.GetValueOrDefault());
      CurrencyType currencyTypeFromCode = ShopUtils.GetCurrencyTypeFromCode(currencyCode);
      ShopUtils.TryGetBundlePrice(bundle, currencyTypeFromCode, out totalPrice);
      if (bundle.Items.Count == 1)
      {
        ref string local = ref productItemType;
        productType = bundle.Items[0].ItemType;
        string lowerInvariant = productType.ToString().ToLowerInvariant();
        local = lowerInvariant;
        productItemId = bundle.Items[0].ProductData;
      }
      totalPrice *= (long) quantity;
    }
    return true;
  }

  public static CurrencyType GetCurrencyTypeFromProto(PegasusShared.CurrencyType protoCurrencyType)
  {
    switch (protoCurrencyType)
    {
      case PegasusShared.CurrencyType.CURRENCY_TYPE_GOLD:
        return CurrencyType.GOLD;
      case PegasusShared.CurrencyType.CURRENCY_TYPE_DUST:
        return CurrencyType.DUST;
      case PegasusShared.CurrencyType.CURRENCY_TYPE_CN_RUNESTONES:
        return CurrencyType.CN_RUNESTONES;
      case PegasusShared.CurrencyType.CURRENCY_TYPE_CN_ARCANE_ORBS:
        return CurrencyType.CN_ARCANE_ORBS;
      case PegasusShared.CurrencyType.CURRENCY_TYPE_RENOWN:
        return CurrencyType.RENOWN;
      case PegasusShared.CurrencyType.CURRENCY_TYPE_ROW_RUNESTONES:
        return CurrencyType.ROW_RUNESTONES;
      default:
        return CurrencyType.NONE;
    }
  }

  private static bool TierHasShowIfAllOwnedTag(ProductTierDataModel tier)
  {
    Network.ShopSection networkSection = StoreManager.Get().Catalog.GetNetworkSection(tier);
    return networkSection != null && networkSection.Attributes.GetTags().Contains<string>("show_if_all_owned");
  }

  private static bool ShouldDisplayButtons(IEnumerable<ShopBrowserButtonDataModel> buttons) => buttons.Any<ShopBrowserButtonDataModel>((Func<ShopBrowserButtonDataModel, bool>) (button => !button.IsFiller && ShopUtils.AreProductOrVariantsPurchasable(button.DisplayProduct)));

  private static bool AreProductOrVariantsPurchasable(ProductDataModel product) => product.Availability == ProductAvailability.CAN_PURCHASE || product.Variants.Any<ProductDataModel>((Func<ProductDataModel, bool>) (p => p.Availability == ProductAvailability.CAN_PURCHASE));
}
