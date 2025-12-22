using Hearthstone.DataModels;
using System;
using System.Collections.Generic;

public static class CatalogUtils
{
  public static IEnumerable<string> ParseTagsString(string tagsString)
  {
    if (string.IsNullOrEmpty(tagsString))
      return (IEnumerable<string>) new string[0];
    HashSet<string> tagsString1 = new HashSet<string>();
    string str1 = tagsString;
    char[] chArray = new char[1]{ ',' };
    foreach (string str2 in str1.Split(chArray))
      tagsString1.Add(str2.Trim().ToLowerInvariant());
    return (IEnumerable<string>) tagsString1;
  }

  public static bool CanUpdateProductStatus(out string reason)
  {
    if (!StoreManager.Get().HasCatalogNetworkPages())
    {
      reason = "Cannot update product status before populating sections";
      return false;
    }
    if (SpecialEventManager.Get() == null || !SpecialEventManager.Get().HasReceivedEventTimingsFromServer)
    {
      reason = "Cannot update product status before HasReceivedEventTimingsFromServer";
      return false;
    }
    if (NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>() == null)
    {
      reason = "Cannot update product status before NetCacheCardBacks received";
      return false;
    }
    if (NetCache.Get().GetNetObject<NetCache.NetCacheCollection>() == null)
    {
      reason = "Cannot update product status before NetCacheCollection received";
      return false;
    }
    if (CollectionManager.Get() == null)
    {
      reason = "Cannot update product status before CollectionManager initialized";
      return false;
    }
    if (FixedRewardsMgr.Get() == null || !FixedRewardsMgr.Get().IsStartupFinished())
    {
      reason = "Cannot update product status before FixedRewardsMgr initialized";
      return false;
    }
    AccountLicenseMgr.LicenseUpdateState licenseUpdateState = AccountLicenseMgr.Get() != null ? AccountLicenseMgr.Get().FixedLicensesState : AccountLicenseMgr.LicenseUpdateState.UNKNOWN;
    if (licenseUpdateState != AccountLicenseMgr.LicenseUpdateState.SUCCESS)
    {
      reason = string.Format("Cannot update product status when AccountLicenseMgr FixedLicensesState is {0}.", (object) licenseUpdateState);
      return false;
    }
    reason = (string) null;
    return true;
  }

  public static int ComparePricesForSort(PriceDataModel xPrice, PriceDataModel yPrice)
  {
    if (xPrice == null && yPrice == null)
      return 0;
    if (xPrice == null)
      return 1;
    if (yPrice == null || xPrice.Currency < yPrice.Currency)
      return -1;
    if (xPrice.Currency > yPrice.Currency)
      return 1;
    if ((double) xPrice.Amount < (double) yPrice.Amount)
      return -1;
    return (double) xPrice.Amount > (double) yPrice.Amount ? 1 : 0;
  }

  public static ProductDataModel NetGoldCostBoosterToProduct(
    Network.GoldCostBooster goldCostBooster)
  {
    if (!goldCostBooster.Cost.HasValue)
    {
      Log.Store.PrintError("GoldCostBooster has no cost value. Booster ID = {0}", (object) goldCostBooster.ID);
      return (ProductDataModel) null;
    }
    if (goldCostBooster.Cost.Value < 0L)
    {
      Log.Store.PrintError("GoldCostBooster has invalid cost value {0}. Booster ID = {1}", (object) goldCostBooster.Cost.Value, (object) goldCostBooster.ID);
      return (ProductDataModel) null;
    }
    BoosterDbfRecord record = GameDbf.Booster.GetRecord(goldCostBooster.ID);
    if (record == null)
    {
      Log.Store.PrintError("GoldCostBooster has unknown booster ID {0}", (object) goldCostBooster.ID);
      return (ProductDataModel) null;
    }
    ProductDataModel product = new ProductDataModel()
    {
      Name = (string) record.Name,
      Availability = ProductAvailability.CAN_PURCHASE
    };
    product.Prices.Add(new PriceDataModel()
    {
      Currency = CurrencyType.GOLD,
      Amount = (float) goldCostBooster.Cost.Value,
      DisplayText = goldCostBooster.Cost.Value.ToString()
    });
    RewardItemType rewardItemType = goldCostBooster.ID == 629 ? RewardItemType.MERCENARY_BOOSTER : RewardItemType.BOOSTER;
    product.Items.Add(new RewardItemDataModel()
    {
      ItemType = rewardItemType,
      ItemId = goldCostBooster.ID,
      Quantity = 1,
      Booster = new PackDataModel()
      {
        Type = (BoosterDbId) goldCostBooster.ID,
        Quantity = 1
      }
    });
    product.Tags.Add("booster");
    product.RewardList = new RewardListDataModel();
    product.RewardList.Items.AddRange((IEnumerable<RewardItemDataModel>) product.Items);
    product.SetupProductStrings();
    return product;
  }

  public static bool IsPrimaryProductTag(string tag)
  {
    if (tag == "bundle" || tag == "adventure")
      return true;
    return tag != "undefined" && Enum.TryParse<RewardItemType>(tag, true, out RewardItemType _);
  }
}
