using Assets;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public static class HeroSkinUtils
{
  public static bool IsHeroSkinOwned(string cardId)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    return collectionManager != null && collectionManager.IsCardOwned(cardId);
  }

  public static bool CanToggleFavoriteHeroSkin(TAG_CLASS heroClass, string cardId)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return false;
    bool flag1 = collectionManager.GetCountOfOwnedHeroesForClass(heroClass) > 1 && !collectionManager.IsFavoriteHero(cardId);
    bool flag2 = collectionManager.GetFavoriteHeroesForClass(heroClass).Count > 1;
    return HeroSkinUtils.IsHeroSkinOwned(cardId) && flag1 | flag2;
  }

  public static HeroSkinUtils.HeroSkinProductData GetCollectionManagerHeroSkinPurchaseProductData(
    string cardId)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardId);
    if (cardRecord?.CardHero == null)
    {
      string str = cardRecord == null ? "to be found in card database" : "to get hero card from card record";
      Debug.LogError((object) ("HeroSkinUtils:GetCollectionManagerHeroSkinPurchaseProductData card " + cardId + " failed " + str));
      return new HeroSkinUtils.HeroSkinProductData()
      {
        CurrencyType = CardHero.PortraitCurrency.UNKNOWN,
        ProductId = 0
      };
    }
    return new HeroSkinUtils.HeroSkinProductData()
    {
      CurrencyType = cardRecord.CardHero.CollectionManagerPurchaseCurrency,
      ProductId = cardRecord.CardHero.CollectionManagerPurchaseProductId,
      IsDelayedRelease = cardRecord.CardHero.IsCollectionManagerPurchaseDelayed
    };
  }

  public static bool CanBuyHeroSkinFromCollectionManager(string cardId)
  {
    PriceDataModel skinPriceDataModel = HeroSkinUtils.GetCollectionManagerHeroSkinPriceDataModel(cardId);
    return skinPriceDataModel != null && HeroSkinUtils.CanBuyHeroSkinFromCollectionManager(cardId, skinPriceDataModel.Currency, skinPriceDataModel);
  }

  public static bool CanBuyHeroSkinFromCollectionManager(
    string cardId,
    CurrencyType currencyType,
    PriceDataModel priceDataModel = null)
  {
    return !HeroSkinUtils.IsHeroSkinOwned(cardId) && HeroSkinUtils.IsHeroSkinPurchasableFromCollectionManager(cardId, priceDataModel) && HeroSkinUtils.HasEnoughBalanceForHeroSkin(cardId, currencyType);
  }

  public static bool IsHeroSkinPurchasableFromCollectionManager(
    string cardId,
    PriceDataModel priceDataModel = null)
  {
    StoreManager storeManager = StoreManager.Get();
    if (!storeManager.IsOpen(false) || !storeManager.IsBuyHeroSkinsFromCollectionManagerEnabled())
      return false;
    HeroSkinUtils.HeroSkinProductData purchaseProductData = HeroSkinUtils.GetCollectionManagerHeroSkinPurchaseProductData(cardId);
    if (!ProductId.IsValid((long) purchaseProductData.ProductId))
      return false;
    if (priceDataModel != null || HeroSkinUtils.GetCollectionManagerHeroSkinPriceDataModel(cardId, purchaseProductData) != null)
      return true;
    if (purchaseProductData.IsDelayedRelease)
      Debug.LogWarning((object) ("GetCollectionManagerHeroSkinProductBundle failed to get price data model for " + cardId + " - Error skipped as delayed released was true..."));
    else
      Debug.LogError((object) ("HeroSkinUtils:IsHeroSkinPurchasableFromCollectionManager failed to get the price data model for Hero card " + cardId));
    return false;
  }

  public static Network.Bundle GetCollectionManagerHeroSkinProductBundle(string cardId)
  {
    HeroSkinUtils.HeroSkinProductData purchaseProductData = HeroSkinUtils.GetCollectionManagerHeroSkinPurchaseProductData(cardId);
    if (!ProductId.IsValid((long) purchaseProductData.ProductId))
      return (Network.Bundle) null;
    int dbId = GameUtils.TranslateCardIdToDbId(cardId);
    Network.Bundle fromPmtProductId = StoreManager.Get().GetBundleFromPmtProductId(ProductId.CreateFrom((long) purchaseProductData.ProductId));
    if ((Record) fromPmtProductId == (Record) null)
    {
      if (purchaseProductData.IsDelayedRelease)
      {
        Debug.LogWarning((object) ("GetCollectionManagerHeroSkinProductBundle failed to get bundle for Hero card " + cardId + " - Error skipped as delayed released was true..."));
        return (Network.Bundle) null;
      }
      Debug.LogError((object) string.Format("HeroSkinUtils:GetCollectionManagerHeroSkinProductBundle: Did not find a bundle with pmtProductId {0} for Hero card {1}", (object) purchaseProductData.ProductId, (object) cardId));
      return (Network.Bundle) null;
    }
    List<Network.BundleItem> items = fromPmtProductId.Items;
    int index = 0;
    for (int count = items.Count; index < count; ++index)
    {
      Network.BundleItem bundleItem = items[index];
      if (bundleItem.ItemType == ProductType.PRODUCT_TYPE_HERO && bundleItem.ProductData == dbId)
        return fromPmtProductId;
    }
    Debug.LogError((object) string.Format("HeroSkinUtils:GetCollectionManagerHeroSkinProductBundle: Did not find any items with type PRODUCT_TYPE_HERO for bundle with pmtProductId {0} for Hero card {1}", (object) purchaseProductData.ProductId, (object) cardId));
    return (Network.Bundle) null;
  }

  public static PriceDataModel GetCollectionManagerHeroSkinPriceDataModel(
    string cardId)
  {
    HeroSkinUtils.HeroSkinProductData purchaseProductData = HeroSkinUtils.GetCollectionManagerHeroSkinPurchaseProductData(cardId);
    return !ProductId.IsValid((long) purchaseProductData.ProductId) ? (PriceDataModel) null : HeroSkinUtils.GetCollectionManagerHeroSkinPriceDataModel(cardId, purchaseProductData);
  }

  public static PriceDataModel GetCollectionManagerHeroSkinPriceDataModel(
    string cardId,
    HeroSkinUtils.HeroSkinProductData productData)
  {
    Network.Bundle skinProductBundle = HeroSkinUtils.GetCollectionManagerHeroSkinProductBundle(cardId);
    if ((Record) skinProductBundle == (Record) null)
    {
      if (productData.IsDelayedRelease)
      {
        Debug.LogWarning((object) ("HeroSkinUtils:GetCollectionManagerHeroSkinPriceDataModel failed to get bundle for Hero card " + cardId + " - Error skipped as delayed released was true..."));
        return (PriceDataModel) null;
      }
      Debug.LogError((object) ("HeroSkinUtils:GetCollectionManagerHeroSkinPriceDataModel failed to get bundle for Hero card " + cardId));
      return (PriceDataModel) null;
    }
    CurrencyType currencyType;
    long amount;
    switch (productData.CurrencyType)
    {
      case CardHero.PortraitCurrency.GOLD:
        currencyType = CurrencyType.GOLD;
        if (!ShopUtils.TryGetBundlePrice(skinProductBundle, currencyType, out amount))
        {
          Debug.LogError((object) ("HeroSkinUtils:GetCollectionManagerHeroSkinPriceDataModel bundle for Hero card " + cardId + " has no GTAPP gold cost"));
          return (PriceDataModel) null;
        }
        break;
      case CardHero.PortraitCurrency.VIRTUAL_CURRENCY:
        currencyType = ShopUtils.GetBundleVirtualCurrencyPriceType(skinProductBundle);
        if (currencyType == CurrencyType.NONE)
        {
          Debug.LogError((object) ("HeroSkinUtils:GetCollectionManagerHeroSkinPriceDataModel failed to pull VC type for card " + cardId + "."));
          return (PriceDataModel) null;
        }
        if (!ShopUtils.TryGetBundlePrice(skinProductBundle, currencyType, out amount))
        {
          Debug.LogError((object) ("HeroSkinUtils:GetCollectionManagerHeroSkinPriceDataModel bundle for Hero card " + cardId + " has no virutal currency cost"));
          return (PriceDataModel) null;
        }
        break;
      case CardHero.PortraitCurrency.REAL_MONEY:
        currencyType = CurrencyType.REAL_MONEY;
        if (!ShopUtils.TryGetBundlePrice(skinProductBundle, currencyType, out amount))
        {
          Debug.LogError((object) ("HeroSkinUtils:GetCollectionManagerHeroSkinPriceDataModel bundle for Hero card " + cardId + " has no real money cost"));
          return (PriceDataModel) null;
        }
        break;
      default:
        Debug.LogError((object) string.Format("{0} bundle for Hero card {1} do to unhandled currency type {2}!", (object) "HeroSkinUtils:GetCollectionManagerHeroSkinPriceDataModel", (object) cardId, (object) productData.CurrencyType));
        return (PriceDataModel) null;
    }
    PriceDataModel priceModel = new PriceDataModel();
    priceModel.Currency = currencyType;
    priceModel.Amount = (float) amount;
    priceModel.FormatPriceDataModelPrice(skinProductBundle);
    return priceModel;
  }

  public static bool HasEnoughBalanceForHeroSkin(string cardId, CurrencyType currencyType = CurrencyType.GOLD)
  {
    Network.Bundle skinProductBundle = HeroSkinUtils.GetCollectionManagerHeroSkinProductBundle(cardId);
    if ((Record) skinProductBundle == (Record) null)
    {
      Debug.LogError((object) ("HeroSkinUtils:HasEnoughBalanceForHeroSkin called for a card with no valid product bundle. Hero card Id = " + cardId));
      return false;
    }
    if (currencyType == CurrencyType.REAL_MONEY)
      return true;
    long amount;
    return ShopUtils.TryGetBundlePrice(skinProductBundle, currencyType, out amount) && ShopUtils.GetCachedBalance(currencyType) >= amount;
  }

  public struct HeroSkinProductData
  {
    public int ProductId { get; set; }

    public CardHero.PortraitCurrency CurrencyType { get; set; }

    public bool IsDelayedRelease { get; set; }
  }
}
