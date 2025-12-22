using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProductExtensions
{
  private static Dictionary<RewardItemType, string> s_rewardTypeTags = Enum.GetValues(typeof (RewardItemType)).Cast<RewardItemType>().ToDictionary<RewardItemType, RewardItemType, string>((Func<RewardItemType, RewardItemType>) (v => v), (Func<RewardItemType, string>) (v => v.ToString().ToLowerInvariant()));

  public static ProductId GetProductId(this ProductDataModel product) => ProductId.CreateFrom(product.PmtId);

  public static int GetMaxBulkPurchaseCount(this ProductDataModel product)
  {
    if (!product.ProductSupportsQuantitySelect())
      return 1;
    if (!product.Tags.Contains("cn_arcane_orbs"))
      return 50;
    long cachedBalance = ShopUtils.GetCachedBalance(CurrencyType.CN_ARCANE_ORBS);
    if (cachedBalance >= 9999L)
      return 0;
    float currencyInProduct = ShopUtils.GetAmountOfCurrencyInProduct(product, CurrencyType.CN_ARCANE_ORBS);
    return Mathf.Min(488, Mathf.FloorToInt((float) (9999L - cachedBalance) / currencyInProduct));
  }

  public static bool ProductSupportsQuantitySelect(this ProductDataModel product)
  {
    if (product.Prices.Count != 1)
      return false;
    return product.Tags.Contains("cn_arcane_orbs") || product.GetBuyProductArgs(product.Prices.First<PriceDataModel>(), 1) is BuyNoGTAPPEventArgs;
  }

  public static BuyProductEventArgs GetBuyProductArgs(
    this ProductDataModel product,
    PriceDataModel price,
    int quantity)
  {
    if (product.PmtId == 0L && price.Currency == CurrencyType.GOLD)
    {
      if (product.Items.Count != 1)
      {
        Log.Store.PrintError(string.Format("Cannot buy product for gold where item count != 1. Name = {0}, Item Count = {1}", (object) product.Name, (object) product.Items.Count));
        return (BuyProductEventArgs) null;
      }
      RewardItemDataModel rewardItemDataModel = product.Items.First<RewardItemDataModel>();
      ProductType netProductType = ProductExtensions.RewardItemTypeToNetProductType(rewardItemDataModel.ItemType);
      if (netProductType == ProductType.PRODUCT_TYPE_UNKNOWN)
      {
        Log.Store.PrintError(string.Format("Cannot buy gold product with unsupported item type {0}. Name = {1}", (object) rewardItemDataModel.ItemType, (object) product.Name));
        return (BuyProductEventArgs) null;
      }
      return (BuyProductEventArgs) new BuyNoGTAPPEventArgs(new NoGTAPPTransactionData()
      {
        Product = netProductType,
        ProductData = rewardItemDataModel.ItemId,
        Quantity = quantity
      });
    }
    if (!ProductId.IsValid(product.PmtId))
    {
      Log.Store.PrintError(string.Format("Product data model has invalid product ID. Name = {0}, PmtId = {1}", (object) product.Name, (object) product.PmtId));
      return (BuyProductEventArgs) null;
    }
    Network.Bundle fromPmtProductId = StoreManager.Get().GetBundleFromPmtProductId(ProductId.CreateFrom(product.PmtId));
    if (!((Record) fromPmtProductId == (Record) null))
      return (BuyProductEventArgs) new BuyPmtProductEventArgs(fromPmtProductId, price.Currency, quantity);
    Log.Store.PrintError(string.Format("Cannot buy product with no matching Network.Bundle PMT ID = {0}, Name = {1}", (object) product.PmtId, (object) product.Name));
    return (BuyProductEventArgs) null;
  }

  public static void GenerateRewardList(this ProductDataModel product)
  {
    product.RewardList = new RewardListDataModel();
    product.RewardList.Items.AddRange((IEnumerable<RewardItemDataModel>) product.Items);
  }

  public static void FormatProductPrices(this ProductDataModel product, Network.Bundle netBundle = null)
  {
    foreach (PriceDataModel price in product.Prices)
    {
      if (price.Currency == CurrencyType.REAL_MONEY && (Record) netBundle == (Record) null && ProductId.IsValid(product.PmtId))
        netBundle = StoreManager.Get().GetBundleFromPmtProductId(ProductId.CreateFrom(product.PmtId));
      price.FormatPriceDataModelPrice(netBundle);
    }
  }

  public static void FormatPriceDataModelPrice(
    this PriceDataModel priceModel,
    Network.Bundle netBundle = null)
  {
    if (priceModel.Currency == CurrencyType.REAL_MONEY)
    {
      if ((Record) netBundle != (Record) null)
      {
        priceModel.DisplayText = StoreManager.Get().FormatCostBundle(netBundle);
      }
      else
      {
        Log.Store.PrintWarning("Failed to find bundle for formatting cost. May appear wrong on third party store.");
        priceModel.DisplayText = StoreManager.Get().FormatCost(new double?((double) priceModel.Amount));
      }
    }
    else
      priceModel.DisplayText = Mathf.RoundToInt(priceModel.Amount).ToString();
  }

  public static void SetupProductStrings(this ProductDataModel product)
  {
    product.DescriptionHeader = (string) null;
    product.VariantName = (string) null;
    switch (product.GetPrimaryProductTag())
    {
      case "booster":
        product.SetupBoosterProductStrings();
        break;
      case "bundle":
        if (product.IsSingleItemProduct() && (product.Items[0].ItemType == RewardItemType.CN_RUNESTONES || product.Items[0].ItemType == RewardItemType.ROW_RUNESTONES))
        {
          product.SetupRunestoneProductString();
          break;
        }
        break;
      case "cn_runestones":
      case "row_runestones":
        product.SetupRunestoneProductString();
        break;
      case "mercenary_booster":
        product.SetupMercenaryBoosterProductStrings();
        break;
      case "mini_set":
        product.SetupMiniSetProductStrings();
        break;
      case "sellable_deck":
      case "sellable_deck_bundle":
        product.SetupSellableDeckProductStrings();
        break;
    }
    ProductDataModel productDataModel = product;
    string str = product.DescriptionHeader;
    if (str == null)
      str = GameStrings.Format("GLUE_SHOP_DESCRIPTION_HEADER", (object) product.Name);
    productDataModel.DescriptionHeader = str;
    product.VariantName = product.VariantName ?? product.Name;
    string productLegalDisclaimer = product.GetProductLegalDisclaimer();
    if (string.IsNullOrEmpty(productLegalDisclaimer))
      return;
    product.Description = product.Description + "\n" + productLegalDisclaimer;
  }

  public static BoosterDbId GetProductBoosterId(this ProductDataModel product)
  {
    if (!product.Tags.Contains("booster") && !product.Tags.Contains("mercenary_booster"))
      return BoosterDbId.INVALID;
    BoosterDbId productBoosterId = BoosterDbId.INVALID;
    foreach (RewardItemDataModel rewardItemDataModel in product.Items)
    {
      if (rewardItemDataModel.ItemType != RewardItemType.DUST)
      {
        if (rewardItemDataModel.Booster == null || productBoosterId != BoosterDbId.INVALID && rewardItemDataModel.Booster.Type != productBoosterId)
          return BoosterDbId.INVALID;
        productBoosterId = rewardItemDataModel.Booster.Type;
      }
    }
    return productBoosterId;
  }

  public static AdventureDbId GetProductAdventureId(this ProductDataModel product)
  {
    RewardItemDataModel rewardItemDataModel = product.Items.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (i => i.ItemType == RewardItemType.ADVENTURE_WING));
    return rewardItemDataModel != null ? GameUtils.GetAdventureIdByWingId(rewardItemDataModel.ItemId) : AdventureDbId.INVALID;
  }

  public static string GetPrimaryProductTag(this ProductDataModel product)
  {
    DataModelList<string> tags = product.Tags;
    int index = 0;
    for (int count = tags.Count; index < count; ++index)
    {
      string tag = tags[index];
      if (CatalogUtils.IsPrimaryProductTag(tag))
        return tag;
    }
    return (string) null;
  }

  public static bool IsFree(this ProductDataModel product) => product.Tags.Contains("free");

  public static ShopBrowserButtonDataModel ToButton(
    this ProductDataModel product,
    bool isFiller = false)
  {
    return new ShopBrowserButtonDataModel()
    {
      DisplayProduct = product,
      DisplayText = product.Name,
      IsFiller = isFiller,
      Hovered = false
    };
  }

  public static void SetProductTagPresence(
    this ProductDataModel product,
    string tag,
    bool shouldHave)
  {
    bool flag = product.Tags.Contains(tag);
    if (!flag & shouldHave)
    {
      product.Tags.Add(tag);
    }
    else
    {
      if (!flag || shouldHave)
        return;
      product.Tags.Remove(tag);
    }
  }

  public static bool AddAutomaticTagsAndItems(
    this ProductDataModel product,
    Network.Bundle netBundle)
  {
    if (product.Tags.Contains("collapse_wings"))
    {
      List<RewardItemDataModel> list = product.Items.ToList<RewardItemDataModel>();
      while (true)
      {
        AdventureDbId adventureId;
        do
        {
          RewardItemDataModel rewardItemDataModel = list.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (i => i.ItemType == RewardItemType.ADVENTURE_WING));
          if (rewardItemDataModel != null)
          {
            adventureId = GameUtils.GetAdventureIdByWingId(rewardItemDataModel.ItemId);
            list.RemoveAll((Predicate<RewardItemDataModel>) (i => i.ItemType == RewardItemType.ADVENTURE_WING && GameUtils.GetAdventureIdByWingId(i.ItemId) == adventureId));
          }
          else
            goto label_5;
        }
        while (adventureId == AdventureDbId.INVALID);
        list.Add(new RewardItemDataModel()
        {
          ItemType = RewardItemType.ADVENTURE,
          ItemId = (int) adventureId,
          Quantity = 1
        });
      }
label_5:
      list.Sort(new Comparison<RewardItemDataModel>(RewardUtils.CompareItemsForSort));
      product.Items.Clear();
      product.Items.AddRange((IEnumerable<RewardItemDataModel>) list);
    }
    else if (product.Items.Count > 1)
    {
      AdventureDbId adventureId = product.GetProductAdventureId();
      if (adventureId != AdventureDbId.INVALID && product.Items.All<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.ItemType == RewardItemType.ADVENTURE_WING && GameUtils.GetAdventureIdByWingId(item.ItemId) == adventureId)))
        product.Items.Insert(0, new RewardItemDataModel()
        {
          ItemType = RewardItemType.ADVENTURE,
          ItemId = (int) adventureId,
          Quantity = 1
        });
    }
    string str1 = product.GetPrimaryProductTag();
    if (str1 == null)
    {
      str1 = product.DetermineProductPrimaryTagFromItems(netBundle);
      if (str1 == null)
      {
        ProductIssues.LogError(product, "Could not determine a primary tag");
        return false;
      }
      product.Tags.Add(str1);
    }
    if (product.IsSingleItemProduct())
    {
      RewardItemType itemType = product.Items[0].ItemType;
      if (ShopUtils.IsVirtualCurrencyRewardItemType(itemType))
      {
        product.Tags.Add("vc");
        string str2;
        if (str1 == "bundle" && ProductExtensions.s_rewardTypeTags.TryGetValue(itemType, out str2) && !product.Tags.Contains(str2))
          product.Tags.Add(str2);
      }
    }
    if (netBundle.IsPrePurchase && !product.Tags.Contains("prepurchase"))
      product.Tags.Add("prepurchase");
    return true;
  }

  public static bool IsSingleItemProduct(this ProductDataModel product) => product.Items.Count == 1 && !product.Tags.Contains("bundle") || product.Items.Count == 1 && product.Tags.Contains("bundle") && ShopUtils.IsVirtualCurrencyRewardItemType(product.Items[0].ItemType);

  public static bool MatchesItemType(this ProductDataModel product, RewardItemType itemType) => product.Items.First<RewardItemDataModel>().ItemType == itemType;

  public static bool MatchesItemId(this ProductDataModel product, int itemId) => product.Items.First<RewardItemDataModel>().ItemId == itemId;

  public static int CountPacks(this ProductDataModel product)
  {
    int num = 0;
    DataModelList<RewardItemDataModel> items = product.Items;
    int index = 0;
    for (int count = items.Count; index < count; ++index)
    {
      RewardItemDataModel rewardItemDataModel = items[index];
      if (rewardItemDataModel.ItemType == RewardItemType.BOOSTER || rewardItemDataModel.ItemType == RewardItemType.MERCENARY_BOOSTER)
        num += rewardItemDataModel.Quantity;
    }
    return num;
  }

  private static string DetermineProductPrimaryTagFromItems(
    this ProductDataModel product,
    Network.Bundle netBundle)
  {
    if (product.IsEmptyProduct())
      return "bundle";
    if (product.IsAdventureProduct())
      return "adventure";
    if (product.ContainsSellableDeck())
      return "sellable_deck";
    if (product.ContainsSellableDeckBundle())
      return "sellable_deck_bundle";
    if ((Record) netBundle != (Record) null && netBundle.ContainsHiddenLicense() || !product.IsSingleItemProduct())
      return "bundle";
    RewardItemDataModel rewardItemDataModel = product.Items[0];
    if (rewardItemDataModel.ItemType == RewardItemType.UNDEFINED)
    {
      ProductIssues.LogError(product, "Single-item product has reward of undefined type");
      return (string) null;
    }
    if (!Enum.IsDefined(typeof (RewardItemType), (object) rewardItemDataModel.ItemType))
    {
      ProductIssues.LogError(product, string.Format("Single-item product has reward of unsupported type {0}", (object) rewardItemDataModel.ItemType));
      return (string) null;
    }
    string primaryTagFromItems;
    ProductExtensions.s_rewardTypeTags.TryGetValue(rewardItemDataModel.ItemType, out primaryTagFromItems);
    return primaryTagFromItems;
  }

  private static void SetupBoosterProductStrings(this ProductDataModel product)
  {
    BoosterDbId productBoosterId = product.GetProductBoosterId();
    if (productBoosterId != BoosterDbId.INVALID)
    {
      BoosterDbfRecord record = GameDbf.Booster.GetRecord((int) productBoosterId);
      string name = (string) record?.Name;
      if (record != null && !product.Tags.Contains("has_description") && !product.Tags.Contains("has_pmtdescription"))
      {
        if (productBoosterId == BoosterDbId.MERCENARIES)
          product.Description = GameStrings.Format("GLUE_STORE_PRODUCT_DETAILS_MERCENARY_PACK");
        else
          product.Description = GameStrings.Format("GLUE_STORE_PRODUCT_DETAILS_PACK", (object) name);
        if (GameUtils.IsBoosterWild(record))
          product.Description = product.Description + "\n" + GameStrings.Get("GLUE_SHOP_WILD_CARDS_DISCLAIMER");
      }
      if (record != null && !product.Tags.Contains("has_description"))
      {
        product.Name = !string.IsNullOrEmpty((string) record.ShortName) ? (string) record.ShortName : name;
        product.DescriptionHeader = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_PACK");
      }
    }
    product.VariantName = GameStrings.Format("GLUE_SHOP_BOOSTER_SKU_BUTTON", (object) product.CountPacks());
  }

  private static void SetupMercenaryBoosterProductStrings(this ProductDataModel product)
  {
    BoosterDbId productBoosterId = product.GetProductBoosterId();
    if (productBoosterId != BoosterDbId.INVALID)
    {
      string name = (string) GameDbf.Booster.GetRecord((int) productBoosterId)?.Name;
      product.Name = name;
      product.DescriptionHeader = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_PACK");
      product.Description = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_MERCENARY_PACK");
    }
    product.VariantName = GameStrings.Format("GLUE_SHOP_BOOSTER_SKU_BUTTON", (object) product.CountPacks());
  }

  private static void SetupRunestoneProductString(this ProductDataModel product)
  {
    RewardItemDataModel rewardItemDataModel = product.Items.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.ItemType == RewardItemType.CN_RUNESTONES || item.ItemType == RewardItemType.ROW_RUNESTONES));
    if (rewardItemDataModel != null)
      product.VariantName = GameStrings.Format("GLUE_SHOP_RUNESTONE_SKU_BUTTON", (object) rewardItemDataModel.Quantity);
    product.DescriptionHeader = GameStrings.Get("GLUE_SHOP_RUNESTONES_DETAILS_HEADER");
  }

  private static void SetupMiniSetProductStrings(this ProductDataModel product)
  {
    RewardItemDataModel rewardItemDataModel = product.Items.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.ItemType == RewardItemType.MINI_SET));
    if (rewardItemDataModel == null)
      return;
    MiniSetDbfRecord record = GameDbf.MiniSet.GetRecord(rewardItemDataModel.ItemId);
    product.FlavorText = string.Format(GameStrings.Get("GLUE_STORE_MINI_SET_CARD_COUNT"), (object) record.DeckRecord.Cards.Count);
  }

  private static void SetupSellableDeckProductStrings(this ProductDataModel product)
  {
    RewardItemDataModel rewardItemDataModel = product.Items.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.ItemType == RewardItemType.SELLABLE_DECK));
    if (rewardItemDataModel == null)
      return;
    DeckTemplateDbfRecord deckTemplateRecord = GameDbf.SellableDeck.GetRecord(rewardItemDataModel.ItemId)?.DeckTemplateRecord;
    if (deckTemplateRecord == null)
      return;
    product.FlavorText = string.Format(GameStrings.Get("GLUE_STORE_SELLABLEDECKS_FLAVOR"), (object) GameStrings.GetClassName((TAG_CLASS) deckTemplateRecord.ClassId));
  }

  private static string GetProductLegalDisclaimer(this ProductDataModel product)
  {
    if (!StoreManager.Get().IsKoreanCustomer())
      return (string) null;
    if (product.Tags.Contains("non_refundable"))
      return ProductExtensions.GetGenericKoreanAgreementString();
    if (product.Tags.Contains("non_refundable_pack"))
      return GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_EXPERT_PACK");
    if (product.Tags.Contains("prepurchase"))
      return GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_PACK_PREORDER");
    if (product.ContainsRunestones())
      return GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_RUNESTONES");
    if (product.ContainsAdventureChapter())
      return product.Items.Count == 1 ? GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_ADVENTURE_SINGLE") : GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_ADVENTURE_BUNDLE");
    if (product.ContainsAnyBoosterPack())
      return product.IsWelcomeBundle() ? GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_FIRST_PURCHASE_BUNDLE") : GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_EXPERT_PACK");
    if (product.ContainsBattlegroundsPerk())
      return GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_BATTLEGROUNDS_PERKS");
    if (product.ContainsProgressionBonus())
      return GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_PROGRESSION_BONUS");
    return product.ContainsArenaTicket() ? GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_FORGE_TICKET") : (string) null;
  }

  private static bool IsEmptyProduct(this ProductDataModel product) => product.Items.Count == 0;

  private static bool IsAdventureProduct(this ProductDataModel product)
  {
    if (product.IsEmptyProduct())
      return false;
    DataModelList<RewardItemDataModel> items = product.Items;
    if (items[0].ItemType != RewardItemType.ADVENTURE)
      return false;
    int index = 1;
    for (int count = items.Count; index < count; ++index)
    {
      switch (items[index].ItemType)
      {
        case RewardItemType.ADVENTURE_WING:
        case RewardItemType.ADVENTURE:
          continue;
        default:
          return false;
      }
    }
    return true;
  }

  private static bool ContainsRunestones(this ProductDataModel product) => ProductExtensions.ProductContainsItemType(product, RewardItemType.CN_RUNESTONES) || ProductExtensions.ProductContainsItemType(product, RewardItemType.ROW_RUNESTONES);

  private static bool ContainsAdventureChapter(this ProductDataModel product) => ProductExtensions.ProductContainsItemType(product, RewardItemType.ADVENTURE_WING);

  private static bool ContainsAnyBoosterPack(this ProductDataModel product) => product.Tags.Contains("booster") || product.Tags.Contains("mercenary_booster");

  private static bool IsWelcomeBundle(this ProductDataModel product) => product.Tags.Contains("welcome_bundle");

  private static bool ContainsBattlegroundsPerk(this ProductDataModel product) => ProductExtensions.ProductContainsItemType(product, RewardItemType.BATTLEGROUNDS_BONUS);

  private static bool ContainsProgressionBonus(this ProductDataModel product) => ProductExtensions.ProductContainsItemType(product, RewardItemType.PROGRESSION_BONUS);

  private static bool ContainsArenaTicket(this ProductDataModel product) => ProductExtensions.ProductContainsItemType(product, RewardItemType.ARENA_TICKET);

  private static bool ContainsHiddenLicense(this Network.Bundle bundle)
  {
    List<Network.BundleItem> items = bundle.Items;
    int index = 0;
    for (int count = items.Count; index < count; ++index)
    {
      if (items[index].ItemType == ProductType.PRODUCT_TYPE_HIDDEN_LICENSE)
        return true;
    }
    return false;
  }

  private static bool ContainsSellableDeck(this ProductDataModel product) => ProductExtensions.ProductContainsItemType(product, RewardItemType.SELLABLE_DECK);

  private static bool ContainsSellableDeckBundle(this ProductDataModel product) => ProductExtensions.ProductContainsItemType(product, RewardItemType.SELLABLE_DECK_BUNDLE);

  private static bool ProductContainsItemType(
    ProductDataModel product,
    RewardItemType rewardItemType)
  {
    DataModelList<RewardItemDataModel> items = product.Items;
    int index = 0;
    for (int count = items.Count; index < count; ++index)
    {
      if (items[index].ItemType == rewardItemType)
        return true;
    }
    return false;
  }

  private static string GetGenericKoreanAgreementString() => GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_HERO");

  private static ProductType RewardItemTypeToNetProductType(RewardItemType itemType)
  {
    switch (itemType)
    {
      case RewardItemType.BOOSTER:
        return ProductType.PRODUCT_TYPE_BOOSTER;
      case RewardItemType.DUST:
        return ProductType.PRODUCT_TYPE_CURRENCY;
      case RewardItemType.HERO_SKIN:
        return ProductType.PRODUCT_TYPE_HERO;
      case RewardItemType.CARD_BACK:
        return ProductType.PRODUCT_TYPE_CARD_BACK;
      case RewardItemType.ADVENTURE_WING:
        return ProductType.PRODUCT_TYPE_WING;
      case RewardItemType.ARENA_TICKET:
        return ProductType.PRODUCT_TYPE_DRAFT;
      case RewardItemType.RANDOM_CARD:
        return ProductType.PRODUCT_TYPE_RANDOM_CARD;
      case RewardItemType.BATTLEGROUNDS_BONUS:
        return ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BONUS;
      case RewardItemType.TAVERN_BRAWL_TICKET:
        return ProductType.PRODUCT_TYPE_TAVERN_BRAWL_TICKET;
      case RewardItemType.PROGRESSION_BONUS:
        return ProductType.PRODUCT_TYPE_PROGRESSION_BONUS;
      case RewardItemType.MINI_SET:
        return ProductType.PRODUCT_TYPE_MINI_SET;
      case RewardItemType.SELLABLE_DECK:
        return ProductType.PRODUCT_TYPE_SELLABLE_DECK;
      case RewardItemType.MERCENARY_BOOSTER:
        return ProductType.PRODUCT_TYPE_MERCENARIES_BOOSTER;
      case RewardItemType.BATTLEGROUNDS_HERO_SKIN:
        return ProductType.PRODUCT_TYPE_HERO;
      case RewardItemType.BATTLEGROUNDS_GUIDE_SKIN:
        return ProductType.PRODUCT_TYPE_HERO;
      case RewardItemType.LUCKY_DRAW:
        return ProductType.PRODUCT_TYPE_LUCKY_DRAW;
      case RewardItemType.SELLABLE_DECK_BUNDLE:
        return ProductType.PRODUCT_TYPE_SELLABLE_DECK;
      default:
        return ProductType.PRODUCT_TYPE_UNKNOWN;
    }
  }
}
