using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusUtil;
using System;
using System.Collections.Generic;

public static class ProductFactory
{
  public static ProductDataModel CreateEmptyProductDataModel() => new ProductDataModel();

  public static ProductDataModel CreateProductDataModel(
    ShopProductData.ProductData productData)
  {
    return new ProductDataModel()
    {
      PmtId = productData.productId,
      Name = productData.name,
      Tags = CatalogUtils.ParseTagsString(productData.tags).ToDataModelList<string>()
    };
  }

  public static ProductDataModel CreateProductDataModel(Network.Bundle netBundle)
  {
    if (!netBundle.PMTProductID.HasValue || !ProductId.IsValid(netBundle.PMTProductID.Value))
    {
      Log.Store.PrintError("A product Network.Bundle has no PMTProductID");
      return (ProductDataModel) null;
    }
    string title = netBundle.GetTitle();
    ProductDataModel product = new ProductDataModel()
    {
      PmtId = netBundle.PMTProductID.Value,
      Name = title,
      Description = netBundle.GetDescription()
    };
    netBundle.Attributes.GetValue("tags").Match((Action<string>) (netBundleTagString => product.Tags.AddRange(CatalogUtils.ParseTagsString(netBundleTagString))));
    if (!StoreManager.Get().IsLargeItemBundleDetailsEnabled())
      product.Tags.Remove("large_item_bundle_details");
    if (!netBundle.Cost.HasValue && !netBundle.GtappGoldCost.HasValue && !netBundle.VirtualCurrencyCost.HasValue && !product.IsFree())
    {
      ProductIssues.LogError(product, "No prices and no free tag");
      return (ProductDataModel) null;
    }
    if (netBundle.Items.Count == 0)
    {
      ProductIssues.LogError(product, "No licenses and no VC grants");
      return (ProductDataModel) null;
    }
    if (PlatformSettings.IsMobile() && product.Tags.Contains("hide_on_mobile"))
    {
      ProductIssues.LogHidden(product, "Tagged to be hidden from mobile");
      return (ProductDataModel) null;
    }
    if (product.IsFree())
    {
      PriceDataModel priceDataModel = new PriceDataModel()
      {
        Currency = CurrencyType.GOLD,
        Amount = 0.0f
      };
      product.Prices.Add(priceDataModel);
    }
    else if (netBundle.GtappGoldCost.HasValue)
    {
      PriceDataModel priceDataModel = new PriceDataModel()
      {
        Currency = CurrencyType.GOLD,
        Amount = (float) netBundle.GtappGoldCost.Value
      };
      product.Prices.Add(priceDataModel);
    }
    if (netBundle.VirtualCurrencyCost.HasValue)
    {
      CurrencyType currencyTypeFromCode = ShopUtils.GetCurrencyTypeFromCode(netBundle.VirtualCurrencyCode);
      if (!ShopUtils.IsCurrencyVirtual(currencyTypeFromCode))
      {
        ProductIssues.LogError(product, "VC price with unrecognized currency code " + netBundle.VirtualCurrencyCode);
      }
      else
      {
        PriceDataModel priceDataModel = new PriceDataModel()
        {
          Currency = currencyTypeFromCode,
          Amount = (float) netBundle.VirtualCurrencyCost.Value
        };
        product.Prices.Add(priceDataModel);
      }
    }
    if (ProductFactory.ShouldShowRealMoneyPrice(netBundle))
    {
      PriceDataModel priceDataModel = new PriceDataModel()
      {
        Currency = CurrencyType.REAL_MONEY,
        Amount = netBundle.CostDisplay.HasValue ? (float) netBundle.CostDisplay.Value : 0.0f
      };
      product.Prices.Add(priceDataModel);
    }
    if (product.Prices.Count == 0)
    {
      ProductIssues.LogError(product, "No valid prices");
      return (ProductDataModel) null;
    }
    product.FormatProductPrices(netBundle);
    bool flag = false;
    List<RewardItemDataModel> collection = new List<RewardItemDataModel>();
    foreach (Network.BundleItem netBundleItem in netBundle.Items)
    {
      if (netBundleItem.ItemType == ProductType.PRODUCT_TYPE_MINI_SET)
      {
        MiniSetDbfRecord record = GameDbf.MiniSet.GetRecord(netBundleItem.ProductData);
        if ((record != null ? (record.HideOnClient ? 1 : 0) : 0) != 0)
        {
          if (netBundle.Items.Count == 1)
          {
            flag = true;
            ProductIssues.LogError(product, string.Format("Hidden Mini-Set Cannot be the only item in a product!! ProductId={0}", (object) product.PmtId));
            break;
          }
          continue;
        }
      }
      bool isValidItem;
      RewardItemDataModel rewardItemDataModel = RewardFactory.CreateShopRewardItemDataModel(netBundle, netBundleItem, out isValidItem);
      if (!isValidItem)
      {
        flag = true;
        ProductIssues.LogError(product, string.Format("Invalid reward Type={0}, ID={1}", (object) netBundleItem.ItemType, (object) netBundleItem.ProductData));
      }
      if (rewardItemDataModel != null)
        collection.Add(rewardItemDataModel);
    }
    if (flag)
      return (ProductDataModel) null;
    if (collection.Count == 0)
    {
      ProductIssues.LogError(product, "No valid reward items");
      return (ProductDataModel) null;
    }
    collection.Sort(new Comparison<RewardItemDataModel>(RewardUtils.CompareItemsForSort));
    product.Items.AddRange((IEnumerable<RewardItemDataModel>) collection);
    if (!product.AddAutomaticTagsAndItems(netBundle))
    {
      ProductIssues.LogError(product, "Failed to add automatic tags and reward items");
      return (ProductDataModel) null;
    }
    product.GenerateRewardList();
    product.SetupProductStrings();
    return product;
  }

  public static ProductTierDataModel CreateEmptyProductTier() => new ProductTierDataModel();

  private static bool ShouldShowRealMoneyPrice(Network.Bundle netBundle) => netBundle.Cost.HasValue && (!PlatformSettings.IsMobile() || netBundle.DisableRealMoneyShopFlags != (MobileShopType.MOBILE_SHOP_TYPE_APPLE | MobileShopType.MOBILE_SHOP_TYPE_GOOGLE | MobileShopType.MOBILE_SHOP_TYPE_AMAZON | MobileShopType.MOBILE_SHOP_TYPE_ONE_STORE));
}
