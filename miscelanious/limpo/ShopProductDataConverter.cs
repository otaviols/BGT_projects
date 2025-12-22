using Assets;
using Hearthstone.DataModels;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopProductDataConverter : MonoBehaviour
{
  [SerializeField]
  private ShopProductData m_data;
  [SerializeField]
  private bool m_captureCurrentCatalog;
  [SerializeField]
  private bool m_generateTestData;
  [SerializeField]
  private bool m_flushData;
  private List<ShopProductData.ProductItemData> m_itemCatalog;
  private List<ShopProductData.ProductData> m_productCatalog;
  private long m_fakeLicenseId;

  private void Update()
  {
    if (this.m_captureCurrentCatalog && StoreManager.Get().Catalog.HasData)
    {
      this.m_captureCurrentCatalog = false;
      this.SnapshotCurrentCatalog();
    }
    if (this.m_generateTestData)
    {
      this.m_generateTestData = false;
      this.BuildTestData();
    }
    if (!this.m_flushData)
      return;
    this.m_flushData = false;
    this.FlushData();
  }

  private void SnapshotCurrentCatalog()
  {
    ProductCatalog catalog = StoreManager.Get().Catalog;
    this.m_itemCatalog = new List<ShopProductData.ProductItemData>();
    this.m_productCatalog = new List<ShopProductData.ProductData>();
    this.m_fakeLicenseId = 404000L;
    List<ShopProductData.ProductTierData> productTierDataList = new List<ShopProductData.ProductTierData>();
    foreach (ProductTierDataModel productTierDataModel in catalog.GetTiers_Current())
    {
      List<long> longList = new List<long>();
      foreach (ShopBrowserButtonDataModel browserButton in productTierDataModel.BrowserButtons)
        longList.Add(browserButton.DisplayProduct.PmtId);
      ShopProductData.ProductTierData productTierData = new ShopProductData.ProductTierData()
      {
        tierId = productTierDataModel.Style,
        tags = string.Join(",", productTierDataModel.Tags.ToArray<string>()),
        header = productTierDataModel.Header,
        productIds = longList.ToArray()
      };
      productTierDataList.Add(productTierData);
    }
    foreach (ProductDataModel product in catalog.Products)
    {
      ShopProductData.ProductData productData = new ShopProductData.ProductData()
      {
        name = product.Name,
        description = product.Description,
        productId = product.PmtId,
        tags = string.Join(",", product.Tags.ToArray<string>())
      };
      List<ShopProductData.PriceData> priceDataList = new List<ShopProductData.PriceData>();
      foreach (PriceDataModel price in product.Prices)
      {
        ShopProductData.PriceData priceData = new ShopProductData.PriceData()
        {
          amount = (double) price.Amount,
          currencyType = price.Currency
        };
        priceDataList.Add(priceData);
      }
      productData.prices = priceDataList.ToArray();
      List<long> longList = new List<long>();
      foreach (RewardItemDataModel rewardItemDataModel in product.Items)
      {
        ShopProductData.ProductItemData itemData = new ShopProductData.ProductItemData()
        {
          itemId = rewardItemDataModel.ItemId,
          itemType = rewardItemDataModel.ItemType,
          licenseId = rewardItemDataModel.PmtLicenseId == 0L ? this.GetUniqueFakeId() : rewardItemDataModel.PmtLicenseId,
          quantity = rewardItemDataModel.Quantity
        };
        this.FillInDebugItemName(ref itemData);
        this.m_itemCatalog.Add(itemData);
        longList.Add(itemData.licenseId);
      }
      productData.licenseIds = longList.ToArray();
      this.m_productCatalog.Add(productData);
    }
    this.m_data.productTierCatalog = productTierDataList.ToArray();
    this.m_data.productCatalog = this.m_productCatalog.ToArray();
    this.m_data.productItemCatalog = this.m_itemCatalog.ToArray();
  }

  private void BuildTestData()
  {
    this.m_itemCatalog = new List<ShopProductData.ProductItemData>();
    this.m_productCatalog = new List<ShopProductData.ProductData>();
    this.m_fakeLicenseId = 404000L;
    foreach (ModularBundleDbfRecord record in GameDbf.ModularBundle.GetRecords())
    {
      if (!((string) record.Name == ""))
      {
        StorePackId storePackId = new StorePackId();
        storePackId.Type = StorePackType.MODULAR_BUNDLE;
        storePackId.Id = record.ID;
        if (!this.AddItemsAndProductsFromStorePack(storePackId))
        {
          Log.Store.PrintWarning("Could not add test data from Network.Bundles for bundle '{0}' (storePackId: {1})", (object) record.Name, (object) storePackId);
          string[] tagsOverride = new string[1]{ "bundle" };
          this.AddDummyItemAndProduct(RewardItemType.BOOSTER, 1, (string) record.Name, tagsOverride);
        }
      }
    }
    foreach (BoosterDbfRecord record in GameDbf.Booster.GetRecords())
    {
      if (!((string) record.Name == "") && record.StorePrefab != null)
      {
        StorePackId storePackId = new StorePackId();
        storePackId.Type = StorePackType.BOOSTER;
        storePackId.Id = record.ID;
        if (!this.AddItemsAndProductsFromStorePack(storePackId))
        {
          Log.Store.PrintWarning("Could not add test data from Network.Bundles for booster '{0}' (storePackId: {1})", (object) record.Name, (object) storePackId);
          this.AddDummyItemAndProduct(RewardItemType.BOOSTER, record.ID, (string) record.Name);
        }
      }
    }
    int num = 10;
    foreach (CardBackDbfRecord record in GameDbf.CardBack.GetRecords())
    {
      if (!((string) record.Name == ""))
      {
        this.AddDummyItemAndProduct(RewardItemType.CARD_BACK, record.ID, (string) record.Name);
        if (--num <= 0)
          break;
      }
    }
    foreach (CardHeroDbfRecord record1 in GameDbf.CardHero.GetRecords())
    {
      CardDbfRecord record2 = GameDbf.Card.GetRecord(record1.CardId);
      if (record2 != null && !((string) record2.Name == "") && record1.HeroType != CardHero.HeroType.BATTLEGROUNDS_GUIDE && record1.HeroType != CardHero.HeroType.BATTLEGROUNDS_HERO)
      {
        Network.Bundle heroBundle = (Network.Bundle) null;
        StoreManager.Get().GetHeroBundleByCardDbId(record1.CardId, out heroBundle);
        if ((Record) heroBundle == (Record) null || !this.AddItemsAndProductsFromNetBundle(heroBundle, (string) record2.Name))
        {
          Log.Store.PrintWarning("Could not add test data from Network.Bundles for card '{0}' (hero CardId: {1})", (object) record2.Name, (object) record1.CardId);
          this.AddDummyItemAndProduct(RewardItemType.HERO_SKIN, record2.ID, (string) record2.Name);
        }
      }
    }
    this.m_data.productItemCatalog = this.m_itemCatalog.ToArray();
    this.m_data.productCatalog = this.m_productCatalog.ToArray();
  }

  private bool AddItemsAndProductsFromStorePack(StorePackId storePackId)
  {
    ProductType fromStorePackType = StorePackId.GetProductTypeFromStorePackType(storePackId);
    int countFromStorePackId = GameUtils.GetProductDataCountFromStorePackId(storePackId);
    List<Network.Bundle> first = new List<Network.Bundle>();
    for (int selectedIndex = 0; selectedIndex < countFromStorePackId; ++selectedIndex)
    {
      List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(fromStorePackType, false, GameUtils.GetProductDataFromStorePackId(storePackId, selectedIndex), checkAvailability: false);
      first = first.Concat<Network.Bundle>((IEnumerable<Network.Bundle>) bundlesForProduct).ToList<Network.Bundle>();
    }
    int num1 = 0;
    foreach (Network.Bundle netBundle in first)
    {
      // ISSUE: variable of a boxed type
      __Boxed<StorePackType> type = (Enum) storePackId.Type;
      // ISSUE: variable of a boxed type
      __Boxed<int> id = (ValueType) storePackId.Id;
      long? pmtProductId = netBundle.PMTProductID;
      long num2;
      if (!pmtProductId.HasValue)
      {
        num2 = -1L;
      }
      else
      {
        pmtProductId = netBundle.PMTProductID;
        num2 = pmtProductId.Value;
      }
      // ISSUE: variable of a boxed type
      __Boxed<long> local = (ValueType) num2;
      string debugName = string.Format("(DEBUG) Type: {0}-{1}; productId:{2}", (object) type, (object) id, (object) local);
      List<string> overrideTags = (List<string>) null;
      if (storePackId.Type == StorePackType.MODULAR_BUNDLE)
      {
        overrideTags = new List<string>();
        overrideTags.Add("bundle");
        if (netBundle.IsPrePurchase)
          overrideTags.Add("prepurchase");
      }
      if (this.AddItemsAndProductsFromNetBundle(netBundle, debugName, overrideTags))
        ++num1;
    }
    return num1 > 0;
  }

  private bool AddItemsAndProductsFromNetBundle(
    Network.Bundle netBundle,
    string debugName,
    List<string> overrideTags = null)
  {
    long productId = netBundle.PMTProductID.HasValue ? netBundle.PMTProductID.Value : this.GetUniqueFakeId();
    if (this.m_productCatalog.Exists((Predicate<ShopProductData.ProductData>) (p => p.productId == productId)))
      return false;
    ShopProductData.ProductData productData = new ShopProductData.ProductData();
    productData.name = StoreManager.Get().GetProductName(netBundle);
    productData.description = debugName;
    List<string> tags = new List<string>();
    List<long> longList = new List<long>();
    foreach (Network.BundleItem bundleItem in netBundle.Items)
    {
      ShopProductData.ProductItemData itemData = this.GenerateProductItemData(bundleItem);
      if (itemData.itemType != RewardItemType.UNDEFINED)
      {
        if (!this.m_itemCatalog.Exists((Predicate<ShopProductData.ProductItemData>) (i => i.licenseId == itemData.licenseId)))
          this.m_itemCatalog.Add(itemData);
        longList.Add(itemData.licenseId);
        this.GetTags(itemData, ref tags);
      }
    }
    productData.licenseIds = longList.ToArray();
    if (overrideTags != null)
      tags = overrideTags;
    productData.tags = this.SerializeTags(tags);
    productData.productId = productId;
    this.AddPricesFromNetBundle(ref productData, netBundle);
    this.m_productCatalog.Add(productData);
    return true;
  }

  private ShopProductData.ProductItemData GenerateProductItemData(
    Network.BundleItem bundleItem)
  {
    ShopProductData.ProductItemData itemData = new ShopProductData.ProductItemData();
    switch (bundleItem.ItemType)
    {
      case ProductType.PRODUCT_TYPE_BOOSTER:
        itemData.itemType = RewardItemType.BOOSTER;
        break;
      case ProductType.PRODUCT_TYPE_CARD_BACK:
        itemData.itemType = RewardItemType.CARD_BACK;
        break;
      case ProductType.PRODUCT_TYPE_HERO:
        RewardItemType rewardItemType = RewardItemType.HERO_SKIN;
        int productData = bundleItem.ProductData;
        if (CollectionManager.Get().IsBattlegroundsHeroSkinCard(productData))
          rewardItemType = RewardItemType.BATTLEGROUNDS_HERO_SKIN;
        else if (CollectionManager.Get().IsBattlegroundsGuideSkinCard(productData))
          rewardItemType = RewardItemType.BATTLEGROUNDS_GUIDE_SKIN;
        itemData.itemType = rewardItemType;
        break;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BOARD_SKIN:
        itemData.itemType = RewardItemType.BATTLEGROUNDS_BOARD_SKIN;
        break;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_FINISHER:
        itemData.itemType = RewardItemType.BATTLEGROUNDS_FINISHER;
        break;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_EMOTE:
        itemData.itemType = RewardItemType.BATTLEGROUNDS_EMOTE;
        break;
      case ProductType.PRODUCT_TYPE_LUCKY_DRAW:
        itemData.itemType = RewardItemType.LUCKY_DRAW;
        break;
      default:
        itemData.itemType = RewardItemType.UNDEFINED;
        break;
    }
    itemData.itemId = bundleItem.ProductData;
    itemData.quantity = bundleItem.Quantity;
    itemData.licenseId = this.GetUniqueFakeId();
    this.FillInDebugItemName(ref itemData);
    return itemData;
  }

  private void FillInDebugItemName(ref ShopProductData.ProductItemData itemData)
  {
    string str;
    switch (itemData.itemType)
    {
      case RewardItemType.BOOSTER:
        str = (string) GameDbf.Booster.GetRecord(itemData.itemId).Name;
        break;
      case RewardItemType.HERO_SKIN:
      case RewardItemType.CARD:
      case RewardItemType.BATTLEGROUNDS_HERO_SKIN:
      case RewardItemType.BATTLEGROUNDS_GUIDE_SKIN:
        str = (string) GameDbf.Card.GetRecord(itemData.itemId).Name;
        break;
      case RewardItemType.CARD_BACK:
        str = (string) GameDbf.CardBack.GetRecord(itemData.itemId).Name;
        break;
      case RewardItemType.BATTLEGROUNDS_BOARD_SKIN:
        str = (string) GameDbf.BattlegroundsBoardSkin.GetRecord(itemData.itemId).CollectionName;
        break;
      case RewardItemType.BATTLEGROUNDS_FINISHER:
        str = (string) GameDbf.BattlegroundsFinisher.GetRecord(itemData.itemId).CollectionName;
        break;
      case RewardItemType.BATTLEGROUNDS_EMOTE:
        str = (string) GameDbf.BattlegroundsEmote.GetRecord(itemData.itemId).CollectionShortName;
        break;
      default:
        str = string.Format("{0}-{1}", (object) itemData.itemType, (object) itemData.itemId);
        break;
    }
    if (itemData.quantity == 1)
      itemData.debugName = string.Format("{0} ({1})", (object) str, (object) itemData.itemType);
    else
      itemData.debugName = string.Format("{0} x{1}", (object) str, (object) itemData.quantity);
  }

  private void AddDummyItemAndProduct(
    RewardItemType itemType,
    int itemId,
    string debugName,
    string[] tagsOverride = null)
  {
    ShopProductData.ProductItemData itemData = new ShopProductData.ProductItemData();
    itemData.itemType = itemType;
    itemData.itemId = itemId;
    itemData.debugName = "[PH] " + debugName;
    itemData.licenseId = this.GetUniqueFakeId();
    itemData.quantity = 1;
    this.m_itemCatalog.Add(itemData);
    ShopProductData.ProductData productData = new ShopProductData.ProductData();
    productData.name = itemData.debugName;
    productData.description = itemData.ToString();
    productData.licenseIds = new long[1]
    {
      itemData.licenseId
    };
    productData.productId = this.GetUniqueFakeId();
    productData.prices = new ShopProductData.PriceData[1]
    {
      new ShopProductData.PriceData()
      {
        currencyType = CurrencyType.GOLD,
        amount = 404.0
      }
    };
    List<string> tags = new List<string>();
    if (tagsOverride != null)
      tags = ((IEnumerable<string>) tagsOverride).ToList<string>();
    else
      this.GetTags(itemData, ref tags);
    productData.tags = this.SerializeTags(tags);
    this.m_productCatalog.Add(productData);
  }

  private void AddPricesFromNetBundle(
    ref ShopProductData.ProductData productData,
    Network.Bundle netBundle)
  {
    List<ShopProductData.PriceData> priceDataList = new List<ShopProductData.PriceData>();
    ShopProductData.PriceData priceData = new ShopProductData.PriceData();
    if (netBundle.CostDisplay.HasValue)
    {
      priceData.currencyType = CurrencyType.REAL_MONEY;
      priceData.amount = netBundle.CostDisplay.Value;
      priceDataList.Add(priceData);
    }
    long? gtappGoldCost = netBundle.GtappGoldCost;
    if (gtappGoldCost.HasValue)
    {
      priceData.currencyType = CurrencyType.GOLD;
      ref ShopProductData.PriceData local = ref priceData;
      gtappGoldCost = netBundle.GtappGoldCost;
      double num = (double) gtappGoldCost.Value;
      local.amount = num;
      priceDataList.Add(priceData);
    }
    productData.prices = priceDataList.ToArray();
  }

  private void FlushData()
  {
  }

  private string GetTags(ShopProductData.ProductItemData itemData)
  {
    List<string> tags = new List<string>();
    this.GetTags(itemData, ref tags);
    return this.SerializeTags(tags);
  }

  private void GetTags(ShopProductData.ProductItemData itemData, ref List<string> tags)
  {
    List<string> stringList = new List<string>();
    switch (itemData.itemType)
    {
      case RewardItemType.BOOSTER:
        stringList.Add("booster");
        break;
      case RewardItemType.HERO_SKIN:
      case RewardItemType.BATTLEGROUNDS_HERO_SKIN:
      case RewardItemType.BATTLEGROUNDS_GUIDE_SKIN:
      case RewardItemType.BATTLEGROUNDS_BOARD_SKIN:
      case RewardItemType.BATTLEGROUNDS_FINISHER:
        stringList.Add("skin");
        break;
      case RewardItemType.CARD_BACK:
        stringList.Add("cardback");
        break;
      case RewardItemType.BATTLEGROUNDS_EMOTE:
      case RewardItemType.BATTLEGROUNDS_EMOTE_PILE:
        stringList.Add("emote");
        break;
    }
    if (itemData.itemType == RewardItemType.BOOSTER)
    {
      int itemId = itemData.itemId;
      if (itemId <= 11)
      {
        if (itemId != 1)
        {
          if (itemId != 9 && itemId != 11)
            goto label_18;
        }
        else
        {
          stringList.Add("classic");
          goto label_18;
        }
      }
      else if (itemId <= 21)
      {
        if (itemId != 17)
        {
          if ((uint) (itemId - 19) > 2U)
            goto label_18;
        }
        else
        {
          stringList.Add("welcome_bundle");
          stringList.Add("bad_prefab");
          goto label_18;
        }
      }
      else if (itemId != 30)
      {
        if (itemId == 256)
        {
          stringList.Add("theme_pack");
          stringList.Add("rogue_theme");
          goto label_18;
        }
        else
          goto label_18;
      }
      stringList.Add("wild");
    }
label_18:
    foreach (string str in stringList)
    {
      if (!tags.Contains(str))
        tags.Add(str);
    }
  }

  private string SerializeTags(List<string> tags) => string.Join(",", tags.ToArray());

  private long GetUniqueFakeId() => this.m_fakeLicenseId++;
}
