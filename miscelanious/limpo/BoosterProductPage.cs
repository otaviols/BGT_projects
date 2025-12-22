using Blizzard.T5.AssetManager;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class BoosterProductPage : ProductPage
{
  [SerializeField]
  private AsyncReference[] m_boosterStackRefs;
  [SerializeField]
  private int m_variableQuantityMax;
  [SerializeField]
  protected float m_packShakeTime = 2f;
  [SerializeField]
  protected float m_packLandShakeDelay = 0.25f;
  [SerializeField]
  protected float m_packLandWeight = 2f;
  [SerializeField]
  protected float m_packLiftWeight = 1f;
  private int m_lastSelectedQuantity;
  private List<BoosterStack> m_boosterStacks;
  private ShakePane m_shakePane;
  private bool m_pendingDistributePacks = true;

  protected override void Awake()
  {
    base.Awake();
    this.OnProductVariantSet += new Action(this.HandleProductVariantSet);
  }

  protected override void Start()
  {
    this.m_boosterStacks = new List<BoosterStack>(this.m_boosterStackRefs.Length);
    foreach (AsyncReference boosterStackRef1 in this.m_boosterStackRefs)
      boosterStackRef1.RegisterReadyListener<BoosterStack>((Action<BoosterStack>) (stack =>
      {
        if (!this.AreBoosterStacksReady())
          return;
        foreach (AsyncReference boosterStackRef2 in this.m_boosterStackRefs)
          this.m_boosterStacks.Add(boosterStackRef2.Object as BoosterStack);
      }));
    this.m_shakePane = this.GetComponentInParent<ShakePane>();
    base.Start();
  }

  protected void Update()
  {
    if (!this.m_pendingDistributePacks || !this.AreBoosterStacksReady() || !this.IsOpen || this.m_widget.IsChangingStates)
      return;
    this.m_pendingDistributePacks = false;
    this.DistributeStacks();
  }

  public override void Open()
  {
    if ((UnityEngine.Object) this.m_container != (UnityEngine.Object) null)
      this.m_container.OverrideMusic(MusicPlaylistType.Invalid);
    base.Open();
  }

  protected override void OnProductSet()
  {
    base.OnProductSet();
    RewardItemDataModel rewardItemDataModel = this.Product.Items.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.Booster != null));
    if (rewardItemDataModel == null)
    {
      Log.Store.PrintError("No Boosters in Product \"{0}\"", (object) this.Product.Name);
    }
    else
    {
      using (AssetHandle<GameObject> assetHandle = ShopUtils.LoadStorePackPrefab(rewardItemDataModel.Booster.Type))
      {
        StorePackDef storePackDef = (bool) assetHandle ? assetHandle.Asset.GetComponent<StorePackDef>() : (StorePackDef) null;
        if ((UnityEngine.Object) this.m_container != (UnityEngine.Object) null)
          this.m_container.OverrideMusic((bool) (UnityEngine.Object) storePackDef ? storePackDef.GetPlaylist() : MusicPlaylistType.Invalid);
      }
      foreach (BoosterStack boosterStack in this.m_boosterStacks)
        boosterStack.SetStacks(0);
      if (this.m_variableQuantityMax > 0)
        this.TEST_PopulateVariantsRange(this.Product, maxQuantity: this.m_variableQuantityMax);
      List<IDataModel> list = this.Product.Variants.Cast<IDataModel>().ToList<IDataModel>();
      list.Sort(new Comparison<IDataModel>(BoosterProductPage.SortProducts));
      int index = list.IndexOf((IDataModel) this.m_productSelection.Variant);
      if (index < 0)
        index = 0;
      this.SelectVariant(list.ElementAtOrDefault<IDataModel>(index) as ProductDataModel);
    }
  }

  private void HandleProductVariantSet() => this.m_pendingDistributePacks = true;

  private bool AreBoosterStacksReady() => ((IEnumerable<AsyncReference>) this.m_boosterStackRefs).All<AsyncReference>((Func<AsyncReference, bool>) (r => r.IsReady));

  private void DistributeStacks()
  {
    ProductDataModel selectedVariant = this.GetSelectedVariant();
    int num1 = 0;
    int num2 = selectedVariant != null ? selectedVariant.CountPacks() : 0;
    int num3 = this.m_boosterStacks.Count<BoosterStack>();
    int num4 = num2 / num3;
    int num5 = num2 % num3;
    bool flag1 = num2 > this.m_lastSelectedQuantity;
    int num6 = (this.m_lastSelectedQuantity + (flag1 ? 0 : -1)) % num3;
    this.m_lastSelectedQuantity = num2;
    for (int index1 = 0; index1 < num3; ++index1)
    {
      int index2 = (num6 + (flag1 ? index1 : -index1) + num3) % num3;
      bool flag2 = index2 < num5;
      int stackSize = num4 + (flag2 ? 1 : 0);
      BoosterStack boosterStack = this.m_boosterStacks[index2];
      num1 += boosterStack.CurrentStackSize;
      boosterStack.StackingDelay = (float) index1 * boosterStack.StackingBaseDuration / (float) num3;
      boosterStack.SetStacks(stackSize, false);
    }
    if (!(bool) (UnityEngine.Object) this.m_shakePane)
      return;
    int num7 = num2 - num1;
    this.m_shakePane.Shake(num7 > 0 ? (float) num7 * this.m_packLandWeight : (float) num7 * this.m_packLiftWeight, this.m_packShakeTime, num7 > 0 ? this.m_packLandShakeDelay : 0.0f);
  }

  private static int SortProducts(IDataModel a, IDataModel b)
  {
    if (!(a is ProductDataModel product1) || !(b is ProductDataModel product2))
      return 0;
    int num1 = product1.CountPacks();
    int num2 = product2.CountPacks();
    if (num1 > num2)
      return 1;
    return num1 < num2 ? -1 : 0;
  }

  private void TEST_PopulateVariantsRange(
    ProductDataModel product,
    int minQuantity = 1,
    int maxQuantity = 100)
  {
    if (product.Variants.Count >= maxQuantity)
      return;
    ProductDataModel productDataModel1 = product.Variants.First<ProductDataModel>();
    RewardItemDataModel rewardItemDataModel1 = productDataModel1.Items.First<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (i => i.Booster != null));
    for (int quantity = minQuantity; quantity <= maxQuantity; quantity++)
    {
      ProductDataModel product1;
      if (!product.Variants.Any<ProductDataModel>((Func<ProductDataModel, bool>) (p => (product1 = p) != null && product1.CountPacks() == quantity)))
      {
        RewardItemDataModel rewardItemDataModel2 = new RewardItemDataModel()
        {
          Booster = rewardItemDataModel1.Booster,
          ItemId = rewardItemDataModel1.ItemId,
          ItemType = rewardItemDataModel1.ItemType,
          Quantity = quantity
        };
        PriceDataModel priceDataModel = new PriceDataModel()
        {
          Currency = CurrencyType.GOLD,
          Amount = (float) (quantity * 100)
        };
        priceDataModel.DisplayText = priceDataModel.Amount.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        ProductDataModel productDataModel2 = new ProductDataModel()
        {
          Name = string.Format("TESTDATA {0}x{1}", (object) rewardItemDataModel1.Booster.Type, (object) quantity)
        };
        productDataModel2.Items.Add(rewardItemDataModel2);
        productDataModel2.Prices.Add(priceDataModel);
        productDataModel2.Tags = productDataModel1.Tags;
        product.Variants.Add(productDataModel2);
      }
    }
  }
}
