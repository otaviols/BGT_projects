using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Game.Shop;
using Hearthstone.Commerce;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;

public class ProductCatalog
{
  private readonly List<ProductDataModel> m_products = new List<ProductDataModel>();
  private readonly CatalogPages m_catalogPages = new CatalogPages();
  private readonly Dictionary<ProductTierDataModel, Network.ShopSection> m_tierSectionMapping = new Dictionary<ProductTierDataModel, Network.ShopSection>();
  private ProductDataModel m_virtualCurrencyProduct;
  private ProductDataModel m_boosterCurrencyProduct;
  private readonly HashSet<ProductDataModel> m_productsFromTestData = new HashSet<ProductDataModel>();
  private readonly HashSet<ProductTierDataModel> m_tiersFromTestData = new HashSet<ProductTierDataModel>();
  private BoosterDbId m_latestBoosterId;
  private AdventureDbId m_latestAdventureId;
  private int m_rotationWarningThreshold;
  private DateTime? m_nextCatalogChangeTimeUtc;
  private long m_tiersChangeCount;
  private bool m_hasUpdatedProductStatusOnce;
  private readonly VariantUtils.ISortOrder m_sortOrder = (VariantUtils.ISortOrder) new ProductCatalog.VariantSortOrder();
  private ProductCatalog.TestDataMode m_testDataMode;
  private static Comparison<Network.ShopSection.ProductRef> SortProducts = (Comparison<Network.ShopSection.ProductRef>) ((a, b) => a.OrderId.CompareTo(b.OrderId));
  private static Comparison<Network.ShopSection> SortSections = (Comparison<Network.ShopSection>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder));

  public List<ProductDataModel> Products => this.m_products;

  public long TiersChangeCount => this.m_tiersChangeCount;

  public ProductDataModel VirtualCurrencyProductItem => this.m_virtualCurrencyProduct;

  public ProductDataModel BoosterCurrencyProductItem => this.m_boosterCurrencyProduct;

  public bool HasTestData { get; private set; }

  public bool HasNetData { get; private set; }

  public bool HasData => this.HasTestData || this.HasNetData;

  public ProductCatalog.TestDataMode CurrentTestDataMode => this.m_testDataMode;

  public void SetTestDataMode(ProductCatalog.TestDataMode mode)
  {
    if (this.m_testDataMode == mode)
      return;
    switch (mode)
    {
      case ProductCatalog.TestDataMode.NO_TEST_DATA:
        this.ClearTestData();
        break;
      case ProductCatalog.TestDataMode.ONLY_TEST_DATA:
        this.ClearNonTestData();
        break;
    }
    this.m_testDataMode = mode;
  }

  public ProductCatalog(StoreManager storeManager) => storeManager.RegisterStatusChangedListener(new System.Action<bool>(this.OnStoreStatusChanged));

  public Network.ShopSection GetNetworkSection(ProductTierDataModel tier)
  {
    Network.ShopSection networkSection;
    this.m_tierSectionMapping.TryGetValue(tier, out networkSection);
    return networkSection;
  }

  public void PopulateWithNetData(
    List<Network.Bundle> netBundles,
    List<Network.GoldCostBooster> netGoldBoosters,
    CatalogNetworkPages networkPages)
  {
    if (this.CurrentTestDataMode == ProductCatalog.TestDataMode.ONLY_TEST_DATA)
      return;
    this.ClearNonTestData();
    this.HasNetData = true;
    this.m_products.Capacity = Math.Max(this.m_products.Capacity, netBundles.Count<Network.Bundle>() + netGoldBoosters.Count<Network.GoldCostBooster>());
    this.AddNetGoldBoosterProducts((IEnumerable<Network.GoldCostBooster>) netGoldBoosters);
    this.AddNetBundleProducts((IEnumerable<Network.Bundle>) netBundles);
    this.UpdateProductStatus();
    this.PopulateTiers(networkPages);
    this.PopulateProductVariants();
  }

  public ProductDataModel GetProductByPmtId(ProductId productId)
  {
    int index = 0;
    for (int count = this.m_products.Count; index < count; ++index)
    {
      if (this.m_products[index].PmtId == productId.Value)
        return this.m_products[index];
    }
    return (ProductDataModel) null;
  }

  public List<ProductTierDataModel> GetTiers(ShopType shopType) => this.m_catalogPages.GetTiers(shopType);

  public List<ProductTierDataModel> GetTiers_Current() => this.GetTiers(StoreManager.Get().CurrentShopType);

  public List<ProductTierDataModel> GetTiers_All() => this.m_catalogPages.GetTiers_All();

  public void PopulateWithTestData(ShopProductData testData)
  {
    bool flag1 = false;
    bool flag2 = false;
    switch (this.CurrentTestDataMode)
    {
      case ProductCatalog.TestDataMode.ONLY_TEST_DATA:
        flag1 = true;
        flag2 = true;
        break;
      case ProductCatalog.TestDataMode.ADD_PRODUCT_TEST_DATA:
        flag1 = true;
        break;
      case ProductCatalog.TestDataMode.TIER_TEST_DATA:
        flag2 = true;
        break;
    }
    if (!(flag1 | flag2))
      return;
    Log.Store.Print("=== Begin populate ProductCatalog with test data ===");
    this.ClearTiers();
    this.ClearTestData();
    this.HasTestData = true;
    if (flag1 && testData.productCatalog != null)
    {
      Dictionary<long, RewardItemDataModel> dictionary = new Dictionary<long, RewardItemDataModel>();
      if (testData.productItemCatalog != null)
      {
        foreach (ShopProductData.ProductItemData productItemData in testData.productItemCatalog)
        {
          RewardItemDataModel rewardItemDataModel = RewardFactory.CreateShopProductRewardItemDataModel(productItemData);
          RewardUtils.InitializeRewardItemDataModelForShop(rewardItemDataModel, (Network.BundleItem) null, (Network.Bundle) null);
          if (dictionary.ContainsKey(rewardItemDataModel.PmtLicenseId))
            Log.Store.PrintWarning(string.Format("[ProductCatalog.PopulateWithTestData] duplicate ProductItem ID {0}", (object) rewardItemDataModel.PmtLicenseId));
          dictionary[rewardItemDataModel.PmtLicenseId] = rewardItemDataModel;
        }
      }
      bool flag3 = StoreManager.Get().IsOpen();
      foreach (ShopProductData.ProductData productData in testData.productCatalog)
      {
        ProductDataModel productDataModel = ProductFactory.CreateProductDataModel(productData);
        List<RewardItemDataModel> collection = new List<RewardItemDataModel>();
        if (productData.licenseIds != null)
        {
          foreach (long licenseId in productData.licenseIds)
          {
            RewardItemDataModel rewardItemDataModel;
            if (!dictionary.TryGetValue(licenseId, out rewardItemDataModel))
              Log.Store.PrintWarning(string.Format("[ProductCatalog.PopulateWithTestData] Product {0} referencing license {1} with no ProductItem", (object) productDataModel.PmtId, (object) licenseId));
            else
              collection.Add(rewardItemDataModel);
          }
        }
        foreach (ShopProductData.PriceData price in productData.prices)
        {
          PriceDataModel priceDataModel = new PriceDataModel()
          {
            Currency = price.currencyType,
            Amount = (float) price.amount
          };
          productDataModel.Prices.Add(priceDataModel);
        }
        if (flag3)
          productDataModel.FormatProductPrices();
        productDataModel.Items.AddRange((IEnumerable<RewardItemDataModel>) collection);
        productDataModel.RewardList = new RewardListDataModel();
        productDataModel.RewardList.Items.AddRange((IEnumerable<RewardItemDataModel>) collection);
        productDataModel.SetupProductStrings();
        if (ProductId.IsValid(productDataModel.PmtId))
        {
          ProductDataModel productByPmtId = this.GetProductByPmtId(ProductId.CreateFrom(productDataModel.PmtId));
          if (productByPmtId != null)
          {
            Log.Store.Print(string.Format("[ProductCatalog.PopulateWithTestData] Replacing existing product with conflicting Product PMT = ID {0}", (object) productData.productId));
            this.RemoveProduct(productByPmtId);
          }
        }
        this.m_products.Add(productDataModel);
        this.m_productsFromTestData.Add(productDataModel);
      }
      this.SortPrices();
      this.UpdateProductStatus();
    }
    if (flag2 && testData.productTierCatalog != null)
    {
      foreach (ShopProductData.ProductTierData productTierData in testData.productTierCatalog)
      {
        ProductTierDataModel tier = new ProductTierDataModel()
        {
          Style = productTierData.tierId,
          Header = productTierData.header
        };
        tier.Tags.AddRange(CatalogUtils.ParseTagsString(productTierData.tags));
        foreach (ProductDataModel product in ((IEnumerable<long>) productTierData.productIds).Where<long>(new Func<long, bool>(ProductId.IsValid)).Select<long, ProductId>(new Func<long, ProductId>(ProductId.CreateFrom)).Where<ProductId>((Func<ProductId, bool>) (productId => productId.IsValid())).Select<ProductId, ProductDataModel>(new Func<ProductId, ProductDataModel>(this.GetProductByPmtId)))
        {
          if (product != null)
          {
            ShopBrowserButtonDataModel button = product.ToButton();
            tier.BrowserButtons.Add(button);
          }
        }
        if (tier.BrowserButtons.Count > 0)
        {
          this.m_catalogPages.AddTier(ShopType.GENERAL_STORE, tier);
          this.m_tiersFromTestData.Add(tier);
        }
      }
      ++this.m_tiersChangeCount;
    }
    else
      this.PopulateTiers(StoreManager.Get().CatalogNetworkPages);
    this.PopulateProductVariants();
    Log.Store.Print("=== End populate ProductCatalog with test data ===");
  }

  public void UpdateProductStatus()
  {
    Log.Store.PrintDebug(string.Format("Updating Product Status at {0:g}", (object) DateTime.Now));
    string reason;
    if (!CatalogUtils.CanUpdateProductStatus(out reason))
    {
      Log.Store.PrintWarning(reason);
    }
    else
    {
      this.m_hasUpdatedProductStatusOnce = true;
      this.m_latestBoosterId = GameUtils.GetLatestRewardableBooster();
      this.m_latestAdventureId = GameUtils.GetLatestActiveAdventure();
      bool shouldSeeWild = CollectionManager.Get() != null && CollectionManager.Get().ShouldAccountSeeStandardWild();
      this.UpdateWarningThreshold(StoreManager.Get().CatalogNetworkPages);
      foreach (ProductDataModel product in this.m_products)
      {
        if (product.PmtId == 0L)
        {
          product.Availability = ProductAvailability.UNDEFINED;
          if (product.Prices.Count == 1 && product.GetBuyProductArgs(product.Prices[0], 1) is BuyNoGTAPPEventArgs buyProductArgs && StoreManager.Get().GetGoldCostNoGTAPP(buyProductArgs.transactionData, out long _))
            product.Availability = ProductAvailability.CAN_PURCHASE;
        }
        else
        {
          Network.Bundle fromPmtProductId = StoreManager.Get().GetBundleFromPmtProductId(product.GetProductId());
          product.Availability = StoreManager.Get().GetNetworkBundleProductAvailability(fromPmtProductId, shouldSeeWild);
          if (!product.Tags.Contains("booster_allow_no_gold") && product.Availability == ProductAvailability.CAN_PURCHASE && !StoreManager.Get().IgnoreProductTiming && product.GetPrimaryProductTag() == "booster")
          {
            BoosterDbId productBoosterId = product.GetProductBoosterId();
            if (productBoosterId != BoosterDbId.INVALID)
            {
              BoosterDbfRecord record = GameDbf.Booster.GetRecord((int) productBoosterId);
              if (record != null && record.BuyWithGoldEvent != SpecialEventType.UNKNOWN && !SpecialEventManager.Get().IsEventActive(record.BuyWithGoldEvent, false))
                product.Availability = ProductAvailability.SALE_NOT_ACTIVE;
            }
          }
        }
        this.UpdateProductFreshness(product);
      }
      if (this.HasTestData)
      {
        foreach (ProductDataModel productDataModel in this.m_productsFromTestData.Where<ProductDataModel>((Func<ProductDataModel, bool>) (p => p.Availability == ProductAvailability.UNDEFINED)))
          productDataModel.Availability = ProductAvailability.CAN_PURCHASE;
      }
      BnetBar.Get().RefreshCurrency();
      this.UpdateNextCatalogChangeTime();
    }
  }

  public bool TryRefreshStaleProductAvailability()
  {
    if (!CatalogUtils.CanUpdateProductStatus(out string _) || !this.IsProductAvailabilityStale())
      return false;
    this.m_nextCatalogChangeTimeUtc = new DateTime?();
    this.UpdateProductStatus();
    this.PopulateTiers(StoreManager.Get().CatalogNetworkPages);
    this.PopulateProductVariants();
    return true;
  }

  public string DebugFillShopWithProduct(ProductId pmtProductId)
  {
    ProductDataModel productByPmtId = this.GetProductByPmtId(pmtProductId);
    if (productByPmtId == null)
      return (Record) StoreManager.Get().GetBundleFromPmtProductId(pmtProductId) == (Record) null ? string.Format("Product {0} not received from server.", (object) pmtProductId) : string.Format("Product {0} failed client validation. See Store log.", (object) pmtProductId);
    this.PopulateVariantsForProduct(productByPmtId);
    this.ClearTiers();
    this.SetTestDataMode(ProductCatalog.TestDataMode.TIER_TEST_DATA);
    this.HasTestData = true;
    string[] strArray = new string[4]
    {
      "S",
      "BigSmall",
      "Standard",
      "Mammoth"
    };
    foreach (string str in strArray)
    {
      ProductTierDataModel tier = new ProductTierDataModel()
      {
        Header = "Test Tier: " + str,
        Style = str
      };
      for (int index = 0; index < 4; ++index)
      {
        ShopBrowserButtonDataModel button = productByPmtId.ToButton();
        tier.BrowserButtons.Add(button);
      }
      this.m_catalogPages.AddTier(ShopType.GENERAL_STORE, tier);
      this.m_catalogPages.AddTier(ShopType.BATTLEGROUNDS_STORE, tier);
      this.m_catalogPages.AddTier(ShopType.MERCENARIES_STORE, tier);
      this.m_tiersFromTestData.Add(tier);
    }
    ++this.m_tiersChangeCount;
    return (string) null;
  }

  public bool TryGetPmtIdWithTagContainingType(string tag, RewardItemType type, out long pmtId)
  {
    pmtId = 0L;
    IEnumerable<long> source = this.m_products.Where<ProductDataModel>((Func<ProductDataModel, bool>) (product => product.Tags.Contains(tag))).Where<ProductDataModel>((Func<ProductDataModel, bool>) (product => product.Items.Any<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.ItemType == type)))).Select<ProductDataModel, long>((Func<ProductDataModel, long>) (product => product.PmtId));
    if (source.Count<long>() == 0)
      return false;
    pmtId = source.First<long>();
    return true;
  }

  public int DebugFillShopWithProductsByTag(string tag)
  {
    IEnumerable<ProductDataModel> source = this.m_products.Where<ProductDataModel>((Func<ProductDataModel, bool>) (p => p.Tags.Contains(tag)));
    foreach (ProductDataModel product in source)
      this.PopulateVariantsForProduct(product);
    this.ClearTiers();
    this.SetTestDataMode(ProductCatalog.TestDataMode.TIER_TEST_DATA);
    this.HasTestData = true;
    string str = "BigSmall";
    int b = 2;
    int a = source.Count<ProductDataModel>();
    int result;
    int num = Math.DivRem(a, b, out result) + (result != 0 ? 1 : 0);
    int index1 = 0;
    for (int index2 = 0; index2 < num; ++index2)
    {
      ProductTierDataModel tier = new ProductTierDataModel()
      {
        Header = string.Format("Test Tier {0}:", (object) (index2 + 1)),
        Style = str
      };
      for (int index3 = 0; index3 < b && index1 < a; ++index1)
      {
        ShopBrowserButtonDataModel button = source.ElementAt<ProductDataModel>(index1).ToButton();
        tier.BrowserButtons.Add(button);
        ++index3;
      }
      this.m_catalogPages.AddTier(ShopType.GENERAL_STORE, tier);
      this.m_catalogPages.AddTier(ShopType.BATTLEGROUNDS_STORE, tier);
      this.m_catalogPages.AddTier(ShopType.MERCENARIES_STORE, tier);
      this.m_tiersFromTestData.Add(tier);
    }
    ++this.m_tiersChangeCount;
    return a;
  }

  private void Clear()
  {
    this.HasTestData = false;
    this.HasNetData = false;
    this.ClearTiers();
    this.m_products.Clear();
    this.m_productsFromTestData.Clear();
    this.m_virtualCurrencyProduct = (ProductDataModel) null;
    this.m_boosterCurrencyProduct = (ProductDataModel) null;
  }

  private void ClearTiers()
  {
    if (this.m_catalogPages.HasTiers)
    {
      ++this.m_tiersChangeCount;
      this.m_catalogPages.Clear();
    }
    this.m_tiersFromTestData.Clear();
    this.m_tierSectionMapping.Clear();
  }

  private void ClearTestData()
  {
    if (!this.HasTestData)
      return;
    if (!this.HasNetData)
    {
      this.Clear();
    }
    else
    {
      foreach (ProductDataModel product in this.m_productsFromTestData.ToArray<ProductDataModel>())
        this.RemoveProduct(product);
      this.HasTestData = false;
    }
  }

  private void ClearNonTestData()
  {
    if (!this.HasNetData)
      return;
    if (!this.HasTestData)
    {
      this.Clear();
    }
    else
    {
      ProductDataModel[] array1 = this.m_productsFromTestData.ToArray<ProductDataModel>();
      ProductTierDataModel[] array2 = this.m_tiersFromTestData.ToArray<ProductTierDataModel>();
      this.Clear();
      this.m_products.AddRange((IEnumerable<ProductDataModel>) array1);
      this.m_catalogPages.AddTiers(ShopType.GENERAL_STORE, (IEnumerable<ProductTierDataModel>) array2);
      ((IEnumerable<ProductDataModel>) array1).ForEach<ProductDataModel>((System.Action<ProductDataModel>) (x => this.m_productsFromTestData.Add(x)));
      ((IEnumerable<ProductTierDataModel>) array2).ForEach<ProductTierDataModel>((System.Action<ProductTierDataModel>) (x => this.m_tiersFromTestData.Add(x)));
      this.HasTestData = true;
      this.PopulateProductVariants();
    }
  }

  private void RemoveProduct(ProductDataModel product)
  {
    this.m_products.Remove(product);
    if (this.HasTestData)
      this.m_productsFromTestData.Remove(product);
    foreach (ProductTierDataModel productTierDataModel in this.GetTiers_All())
    {
      foreach (ShopBrowserButtonDataModel browserButtonDataModel in productTierDataModel.BrowserButtons.Where<ShopBrowserButtonDataModel>((Func<ShopBrowserButtonDataModel, bool>) (b => b.DisplayProduct == product)))
        productTierDataModel.BrowserButtons.Remove(browserButtonDataModel);
      foreach (ShopBrowserButtonDataModel browserButton in productTierDataModel.BrowserButtons)
      {
        browserButton.DisplayProduct.Variants.Remove(product);
        ++this.m_tiersChangeCount;
      }
    }
    if (this.m_virtualCurrencyProduct != null)
    {
      if (this.m_virtualCurrencyProduct == product)
        this.m_virtualCurrencyProduct = (ProductDataModel) null;
      else
        this.m_virtualCurrencyProduct.Variants.Remove(product);
    }
    if (this.m_boosterCurrencyProduct == null)
      return;
    if (this.m_boosterCurrencyProduct == product)
      this.m_boosterCurrencyProduct = (ProductDataModel) null;
    else
      this.m_boosterCurrencyProduct.Variants.Remove(product);
  }

  private void OnStoreStatusChanged(bool isStoreOpen)
  {
    if (!isStoreOpen)
      return;
    Processor.QueueJob(new JobDefinition("ProductCatalog.PopulateInitialNetData", this.Job_PopulateInitialNetData(), Array.Empty<IJobDependency>()));
  }

  private bool IsProductAvailabilityStale()
  {
    if (!this.m_hasUpdatedProductStatusOnce)
      return true;
    return this.m_nextCatalogChangeTimeUtc.HasValue && this.m_nextCatalogChangeTimeUtc.Value < DateTime.UtcNow;
  }

  private IEnumerator<IAsyncJobResult> Job_PopulateInitialNetData()
  {
    string lastFailReason = string.Empty;
    string reason;
    while (!CatalogUtils.CanUpdateProductStatus(out reason))
    {
      if (reason != lastFailReason)
      {
        Log.Store.PrintWarning("Could not update product status: {0}", (object) reason);
        lastFailReason = reason;
      }
      yield return (IAsyncJobResult) new WaitForDuration(0.1f);
    }
    StoreManager storeMgr = StoreManager.Get();
    while (!storeMgr.HasReceivedAllShopTypeSections())
      yield return (IAsyncJobResult) new WaitForDuration(0.1f);
    this.PopulateWithNetData(storeMgr.AllBundles.ToList<Network.Bundle>(), storeMgr.AllGoldCostBoosters.ToList<Network.GoldCostBooster>(), storeMgr.CatalogNetworkPages);
    foreach (ProductDataModel product in this.m_productsFromTestData)
      product.FormatProductPrices();
    Log.Store.PrintDebug("ProductCatalog initial population complete");
  }

  private void AddNetGoldBoosterProducts(
    IEnumerable<Network.GoldCostBooster> netGoldBoosters)
  {
    foreach (Network.GoldCostBooster netGoldBooster in netGoldBoosters)
    {
      ProductDataModel product = CatalogUtils.NetGoldCostBoosterToProduct(netGoldBooster);
      if (product != null)
        this.m_products.Add(product);
    }
  }

  private void AddNetBundleProducts(IEnumerable<Network.Bundle> netBundles)
  {
    foreach (Network.Bundle netBundle in netBundles)
    {
      long? pmtProductId = netBundle.PMTProductID;
      if (pmtProductId.HasValue)
      {
        pmtProductId = netBundle.PMTProductID;
        if (ProductId.IsValid(pmtProductId.Value))
        {
          pmtProductId = netBundle.PMTProductID;
          ProductId from = ProductId.CreateFrom(pmtProductId.Value);
          ProductDataModel productByPmtId = this.GetProductByPmtId(from);
          if (productByPmtId != null)
          {
            if (!this.m_productsFromTestData.Contains(productByPmtId))
            {
              string title = netBundle.GetTitle();
              Log.Store.PrintError("Ignoring Network.Bundle with PMTProductID that is already in use. PMT ID = {0}, Exiting Product Name = {1}, Ignored Product Name = {2}", (object) from.Value, (object) productByPmtId.Name, (object) title);
              continue;
            }
            continue;
          }
        }
      }
      ProductDataModel productDataModel = ProductFactory.CreateProductDataModel(netBundle);
      if (productDataModel != null)
        this.m_products.Add(productDataModel);
    }
  }

  private void TryAssignProductToSlot(
    List<ShopBrowserButtonDataModel> buttons,
    ProductDataModel product,
    bool isFiller)
  {
    switch (product.Availability)
    {
      case ProductAvailability.CAN_PURCHASE:
        ShopBrowserButtonDataModel button = product.ToButton(isFiller);
        buttons.Add(button);
        break;
      case ProductAvailability.ALREADY_OWNED:
        if (product.Tags.Contains("hide_owned"))
        {
          ProductIssues.LogHidden(product, "Hidden due to hide_owned tag and status is ALREADY_OWNED");
          break;
        }
        goto case ProductAvailability.CAN_PURCHASE;
      case ProductAvailability.SALE_NOT_ACTIVE:
        ProductId from = ProductId.CreateFrom(product.PmtId);
        Network.Bundle fromPmtProductId = StoreManager.Get().GetBundleFromPmtProductId(from);
        if (!((Record) fromPmtProductId != (Record) null))
          break;
        ProductAvailabilityRange availabilityRange = StoreManager.Get().GetBundleAvailabilityRange(fromPmtProductId);
        string str = availabilityRange != null ? availabilityRange.ToString() : "<unknown sale>";
        ProductIssues.LogHidden(product, "Hidden because sale is not active. Range = " + str + " (May be shifted by server cheats)");
        break;
      default:
        ProductIssues.LogHidden(product, string.Format("Hidden because status is {0}", (object) product.Availability));
        break;
    }
  }

  private void PopulateTiersFromNetSections(CatalogNetworkPages networkPages)
  {
    this.ClearTiers();
    if (!this.HasData)
      return;
    List<string> stringList = new List<string>();
    bool flag = BattleNet.GetCurrentRegion() == BnetRegion.REGION_CN;
    foreach (KeyValuePair<ShopType, CatalogNetworkPage> page in networkPages.Pages)
    {
      ShopType key = page.Key;
      List<Network.ShopSection> sections = page.Value.Sections;
      sections.Sort(ProductCatalog.SortSections);
      foreach (Network.ShopSection shopSection in sections)
      {
        stringList.Clear();
        if (!string.IsNullOrEmpty(shopSection.FillerTags))
          stringList.AddRange(CatalogUtils.ParseTagsString(shopSection.FillerTags));
        List<ShopBrowserButtonDataModel> browserButtonDataModelList = new List<ShopBrowserButtonDataModel>();
        List<Network.ShopSection.ProductRef> products = shopSection.Products;
        products.Sort(ProductCatalog.SortProducts);
        bool result = false;
        string str1;
        if (flag && shopSection.Attributes.GetValue("ispersonalizedoffer").TryGetValue(out str1))
          bool.TryParse(str1, out result);
        int index = 0;
        for (int count = products.Count; index < count; ++index)
        {
          Network.ShopSection.ProductRef productRef = products[index];
          if (ProductId.IsValid(productRef.PmtId))
          {
            ProductId from = ProductId.CreateFrom(productRef.PmtId);
            ProductDataModel productByPmtId = this.GetProductByPmtId(from);
            if (productByPmtId == null)
            {
              ProductIssues.LogError(from, "Referenced in section [" + shopSection.InternalName + "] but client has no valid product data model.");
            }
            else
            {
              if (result)
                productByPmtId.Tags.Add("personalized");
              bool isFiller = false;
              foreach (string str2 in stringList)
              {
                if (productByPmtId.Tags.Contains(str2))
                {
                  isFiller = true;
                  break;
                }
              }
              this.TryAssignProductToSlot(browserButtonDataModelList, productByPmtId, isFiller);
            }
          }
        }
        if (browserButtonDataModelList.Count == 0)
        {
          Log.Store.Print("Tier [" + shopSection.InternalName + "] is hidden because it has no products");
        }
        else
        {
          ProductTierDataModel productTierDataModel = new ProductTierDataModel()
          {
            Header = shopSection.Label.GetString(),
            Style = shopSection.Style
          };
          if (result)
            productTierDataModel.Tags.Add("personalized");
          productTierDataModel.BrowserButtons.AddRange((IEnumerable<ShopBrowserButtonDataModel>) browserButtonDataModelList);
          this.m_catalogPages.AddTier(key, productTierDataModel);
          this.m_tierSectionMapping.Add(productTierDataModel, shopSection);
        }
      }
      ++this.m_tiersChangeCount;
    }
  }

  private void PopulateTiers(CatalogNetworkPages networkPages)
  {
    if (this.m_testDataMode == ProductCatalog.TestDataMode.ONLY_TEST_DATA || this.m_testDataMode == ProductCatalog.TestDataMode.TIER_TEST_DATA || !this.m_hasUpdatedProductStatusOnce)
      return;
    this.PopulateTiersFromNetSections(networkPages);
  }

  private void SortItemsOfProductAndVariants(ProductDataModel product)
  {
    foreach (ProductDataModel variant in product.Variants)
      variant.Items.Sort((Comparison<RewardItemDataModel>) ((a, b) => a.ItemId.CompareTo(b.ItemId)));
  }

  private void PopulateProductVariants()
  {
    foreach (ProductTierDataModel productTierDataModel in this.GetTiers_All())
    {
      foreach (ShopBrowserButtonDataModel browserButton in productTierDataModel.BrowserButtons)
      {
        ProductDataModel displayProduct = browserButton.DisplayProduct;
        this.PopulateVariantsForProduct(displayProduct);
        DataModelList<ProductDataModel> variants = displayProduct.Variants;
        // ISSUE: explicit non-virtual call
        if ((variants != null ? __nonvirtual (variants.Count) : 0) > 1)
        {
          DataModelList<RewardItemDataModel> items = displayProduct.Items;
          // ISSUE: explicit non-virtual call
          if ((items != null ? __nonvirtual (items.Count) : 0) > 1)
          {
            DataModelList<string> tags = displayProduct.Tags;
            // ISSUE: explicit non-virtual call
            if ((tags != null ? (__nonvirtual (tags.Contains("sellable_deck")) ? 1 : 0) : 0) != 0)
              this.SortItemsOfProductAndVariants(displayProduct);
          }
        }
      }
    }
    this.m_virtualCurrencyProduct = (ProductDataModel) null;
    this.m_boosterCurrencyProduct = (ProductDataModel) null;
    if (!ShopUtils.IsVirtualCurrencyEnabled())
      return;
    CurrencyType currencyType1;
    if (ShopUtils.TryGetMainVirtualCurrencyType(out currencyType1))
    {
      this.m_virtualCurrencyProduct = this.GetPrimaryProductForItemAndPopulateVariants(ShopUtils.GetRewardItemTypeFromCurrencyType(currencyType1), 0);
      if (this.m_virtualCurrencyProduct == null)
        Log.Store.PrintError(string.Format("Failed to find any Virtual Currency products for Currency Type - {0}.", (object) currencyType1));
    }
    else
      Log.Store.PrintError("Failed to find any Virtual Currency products due to no related Currency Type found while Virtual Currency is enabled.");
    CurrencyType currencyType2;
    if (!ShopUtils.TryGetBoosterVirtualCurrencyType(out currencyType2))
      return;
    this.m_boosterCurrencyProduct = this.GetPrimaryProductForItemAndPopulateVariants(ShopUtils.GetRewardItemTypeFromCurrencyType(currencyType2), 0);
    if (this.m_boosterCurrencyProduct != null)
      return;
    Log.Store.PrintError(string.Format("Failed to find any Booster Currency products for Currency Type - {0}.", (object) currencyType2));
  }

  private void PopulateVariantsForProduct(ProductDataModel product)
  {
    product.Variants.Clear();
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    bool flag4 = product.Items.Count > 1;
    foreach (string tag in product.Tags)
    {
      if (!(tag == "bundle") && !(tag == "large_item_bundle_details"))
      {
        if (!(tag == "sellable_deck") && !(tag == "sellable_deck_bundle"))
        {
          if (tag == "standalone_variant")
            flag3 = true;
        }
        else
          flag1 = true;
      }
      else
        flag2 = true;
    }
    if (((!flag4 ? 0 : (!flag1 ? 1 : 0)) | (flag2 ? 1 : 0) | (flag3 ? 1 : 0)) != 0)
    {
      product.Variants.Add(product);
    }
    else
    {
      List<ProductDataModel> collection;
      if (flag4)
      {
        collection = VariantUtils.GetVariantsWithAllItemsMatching(product, this.m_products);
      }
      else
      {
        RewardItemDataModel rewardItemDataModel = product.Items[0];
        collection = VariantUtils.GetVariantsByItemType(rewardItemDataModel.ItemType, rewardItemDataModel.ItemId, (IEnumerable<ProductDataModel>) this.m_products, this.m_sortOrder);
      }
      if (collection == null)
        return;
      product.Variants.AddRange((IEnumerable<ProductDataModel>) collection);
    }
  }

  private ProductDataModel GetPrimaryProductForItemAndPopulateVariants(
    RewardItemType itemType,
    int itemId)
  {
    List<ProductDataModel> variantsByItemType = VariantUtils.GetVariantsByItemType(itemType, itemId, (IEnumerable<ProductDataModel>) this.m_products, this.m_sortOrder);
    if (variantsByItemType.Count == 0)
      return (ProductDataModel) null;
    ProductDataModel populateVariants = variantsByItemType[0];
    populateVariants.Variants.Clear();
    if (populateVariants.Tags.Contains("standalone_variant"))
      populateVariants.Variants.Add(populateVariants);
    else
      populateVariants.Variants.AddRange((IEnumerable<ProductDataModel>) variantsByItemType);
    return populateVariants;
  }

  private void SortPrices()
  {
    foreach (ProductDataModel product in this.m_products)
    {
      List<PriceDataModel> list = product.Prices.ToList<PriceDataModel>();
      list.Sort(new Comparison<PriceDataModel>(CatalogUtils.ComparePricesForSort));
      product.Prices.Clear();
      product.Prices.AddRange((IEnumerable<PriceDataModel>) list);
    }
  }

  private void UpdateWarningThreshold(CatalogNetworkPages networkPages)
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    StoreManager storeManager = StoreManager.Get();
    if (netObject == null || storeManager == null)
      return;
    bool flag = false;
    Dictionary<ShopType, CatalogNetworkPage>.ValueCollection.Enumerator enumerator1 = networkPages.Pages.Values.GetEnumerator();
    while (!flag && enumerator1.MoveNext())
    {
      List<Network.ShopSection>.Enumerator enumerator2 = enumerator1.Current.Sections.GetEnumerator();
label_12:
      if (!flag && enumerator2.MoveNext())
      {
        using (List<Network.ShopSection.ProductRef>.Enumerator enumerator3 = enumerator2.Current.Products.GetEnumerator())
        {
          while (enumerator3.MoveNext())
          {
            Network.ShopSection.ProductRef current = enumerator3.Current;
            if (ProductId.IsValid(current.PmtId))
            {
              ProductId from = ProductId.CreateFrom(current.PmtId);
              ProductDataModel productByPmtId = this.GetProductByPmtId(from);
              if (productByPmtId != null && productByPmtId.Tags.Contains("prepurchase"))
              {
                Network.Bundle fromPmtProductId = storeManager.GetBundleFromPmtProductId(from);
                ProductAvailabilityRange availabilityRange = (Record) fromPmtProductId != (Record) null ? storeManager.GetBundleAvailabilityRange(fromPmtProductId) : (ProductAvailabilityRange) null;
                if (availabilityRange != null && availabilityRange.IsVisibleAtTime(DateTime.Now))
                {
                  flag = true;
                  break;
                }
              }
            }
          }
          goto label_12;
        }
      }
    }
    if (flag)
      this.m_rotationWarningThreshold = netObject.Store.BoosterRotatingSoonWarnDaysWithSale;
    else
      this.m_rotationWarningThreshold = netObject.Store.BoosterRotatingSoonWarnDaysWithoutSale;
  }

  private void UpdateProductFreshness(ProductDataModel product)
  {
    bool shouldHave1 = product.Tags.Contains("latest_expansion");
    bool shouldHave2 = product.Tags.Contains("new");
    string primaryProductTag = product.GetPrimaryProductTag();
    if (primaryProductTag == "booster")
    {
      BoosterDbId productBoosterId = product.GetProductBoosterId();
      if (productBoosterId == this.m_latestBoosterId)
      {
        shouldHave1 = true;
        shouldHave2 = true;
      }
      else if (!shouldHave2)
      {
        BoosterDbfRecord record = GameDbf.Booster.GetRecord((int) productBoosterId);
        shouldHave2 = record != null && record.LatestExpansionOrder == 0;
      }
      bool flag = GameUtils.IsBoosterWild(productBoosterId);
      if (!product.Tags.Contains("wild") & flag)
        product.Tags.Add("wild");
      else if (!product.Tags.Contains("rotating_soon") && !flag && GameUtils.IsBoosterRotated(productBoosterId, DateTime.UtcNow.AddDays((double) this.m_rotationWarningThreshold)))
        product.Tags.Add("rotating_soon");
    }
    else if (primaryProductTag == "adventure")
    {
      AdventureDbId productAdventureId = product.GetProductAdventureId();
      shouldHave1 |= productAdventureId == this.m_latestAdventureId;
      shouldHave2 |= shouldHave1;
      if (!product.Tags.Contains("wild") && GameUtils.IsAdventureWild(productAdventureId))
        product.Tags.Add("wild");
    }
    else
      shouldHave2 = true;
    if (shouldHave2 && product.Availability == ProductAvailability.ALREADY_OWNED)
      shouldHave2 = false;
    if (shouldHave2)
    {
      string str = product.PmtId.ToString();
      shouldHave2 = Options.Get().GetString(Option.LATEST_SEEN_SHOP_PRODUCT_LIST).IndexOf(str) < 0;
    }
    product.SetProductTagPresence("new", shouldHave2);
    product.SetProductTagPresence("latest_expansion", shouldHave1);
  }

  private void UpdateNextCatalogChangeTime()
  {
    StoreManager storeManager = StoreManager.Get();
    if (storeManager == null)
      return;
    DateTime utcNow = DateTime.UtcNow;
    if (this.m_nextCatalogChangeTimeUtc.HasValue && this.m_nextCatalogChangeTimeUtc.Value <= utcNow)
      return;
    ProductDataModel productDataModel = (ProductDataModel) null;
    ProductAvailabilityRange availabilityRange1 = (ProductAvailabilityRange) null;
    foreach (ProductDataModel product in this.m_products)
    {
      if (product.PmtId != 0L)
      {
        Network.Bundle fromPmtProductId = storeManager.GetBundleFromPmtProductId(ProductId.CreateFrom(product.PmtId));
        if (!((Record) fromPmtProductId == (Record) null))
        {
          ProductAvailabilityRange availabilityRange2 = storeManager.GetBundleAvailabilityRange(fromPmtProductId);
          if (availabilityRange2 != null && !availabilityRange2.IsNever)
          {
            DateTime? nullable = availabilityRange2.StartDateTime;
            if (nullable.HasValue)
            {
              nullable = availabilityRange2.StartDateTime;
              if (nullable.Value > utcNow)
              {
                if (this.m_nextCatalogChangeTimeUtc.HasValue)
                {
                  nullable = availabilityRange2.StartDateTime;
                  if (!(nullable.Value < this.m_nextCatalogChangeTimeUtc.Value))
                    goto label_14;
                }
                nullable = availabilityRange2.StartDateTime;
                this.m_nextCatalogChangeTimeUtc = new DateTime?(nullable.Value);
                productDataModel = product;
                availabilityRange1 = availabilityRange2;
              }
            }
label_14:
            nullable = availabilityRange2.SoftEndDateTime;
            if (nullable.HasValue)
            {
              nullable = availabilityRange2.SoftEndDateTime;
              if (nullable.Value > utcNow)
              {
                if (this.m_nextCatalogChangeTimeUtc.HasValue)
                {
                  nullable = availabilityRange2.SoftEndDateTime;
                  if (!(nullable.Value < this.m_nextCatalogChangeTimeUtc.Value))
                    continue;
                }
                nullable = availabilityRange2.SoftEndDateTime;
                this.m_nextCatalogChangeTimeUtc = new DateTime?(nullable.Value);
                productDataModel = product;
                availabilityRange1 = availabilityRange2;
              }
            }
          }
        }
      }
    }
    if (this.m_nextCatalogChangeTimeUtc.HasValue)
    {
      Log.Store.PrintDebug(string.Format("Next product availability change at {0:g}", (object) this.m_nextCatalogChangeTimeUtc.Value.ToLocalTime()));
      if (productDataModel == null)
        return;
      Log.Store.PrintDebug(string.Format("Next product to change availability is PMT ID = {0}, Name = [{1}], range = {2}", (object) productDataModel.PmtId, (object) productDataModel.Name, (object) availabilityRange1));
    }
    else
      Log.Store.PrintDebug("No known incoming product availability changes");
  }

  private class VariantSortOrder : VariantUtils.ISortOrder
  {
    public int Grouped(ProductDataModel product) => RewardUtils.GetSortOrderFromItems(product.Items);

    public int Ungrouped(ProductDataModel product)
    {
      DataModelList<RewardItemDataModel> items = product.Items;
      int num = 0;
      if (items.Count > 0)
      {
        num = items[0].Quantity;
        switch (items[0].ItemType)
        {
          case RewardItemType.MINI_SET:
            num = product.Tags.Contains("golden") ? 0 : 1;
            break;
          case RewardItemType.SELLABLE_DECK:
            bool flag = false;
            foreach (ProductDataModel variant in product.Variants)
            {
              if (variant.Tags.Contains("show_class_variants"))
              {
                flag = true;
                break;
              }
            }
            num = flag ? (product.Tags.Contains("show_class_variants") ? 0 : items[0].Quantity) : (product.Tags.Contains("golden") ? 0 : 1);
            break;
        }
      }
      return num;
    }
  }

  public enum TestDataMode
  {
    NO_TEST_DATA,
    ONLY_TEST_DATA,
    ADD_PRODUCT_TEST_DATA,
    TIER_TEST_DATA,
  }
}
