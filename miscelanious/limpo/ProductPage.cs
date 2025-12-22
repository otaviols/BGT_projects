using Blizzard.T5.Core.Utils;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductPage : MonoBehaviour
{
  public const string SHOP_BUY_WITH_FIRST_CURRENCY = "SHOP_BUY_WITH_FIRST_CURRENCY";
  public const string SHOP_BUY_WITH_ALT_CURRENCY = "SHOP_BUY_WITH_ALT_CURRENCY";
  protected Widget m_widget;
  protected ProductPageContainer m_container;
  protected Shop m_parentShop;
  protected ProductDataModel m_productImmutable;
  protected ProductDataModel m_productMutable;
  protected Dictionary<int, int> m_variantQuantities = new Dictionary<int, int>();
  protected ProductSelectionDataModel m_productSelection = new ProductSelectionDataModel();
  protected AlertPopup.PopupInfo m_preBuyPopupInfo;
  private Coroutine m_openWhenReadyCoroutine;

  protected virtual void Awake()
  {
    this.m_parentShop = GameObjectUtils.FindComponentInParents<Shop>(this.gameObject);
    this.m_container = GameObjectUtils.FindComponentInParents<ProductPageContainer>(this.gameObject);
    if (!((UnityEngine.Object) this.m_container != (UnityEngine.Object) null))
      return;
    this.m_container.RegisterProductPage(this);
  }

  protected virtual void Start()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnWidgetEvent));
  }

  protected virtual void OnDestroy()
  {
    if (!((UnityEngine.Object) this.m_container != (UnityEngine.Object) null))
      return;
    this.m_container.UnregisterProductPage(this);
  }

  public event EventHandler OnOpened;

  public event EventHandler OnClosed;

  public event Action OnProductVariantSet;

  public Widget WidgetComponent => this.m_widget;

  public ProductDataModel Product => this.m_productMutable ?? this.m_productImmutable;

  public ProductSelectionDataModel Selection => this.m_productSelection;

  public virtual void SelectVariant(ProductDataModel product)
  {
    product = product ?? ProductFactory.CreateEmptyProductDataModel();
    Log.Store.PrintDebug("Selecting Product PMT ID = {0}, Name = {1}", (object) product.PmtId, (object) product.Name);
    if (this.m_productSelection.Variant != product)
    {
      this.m_productSelection.Variant = product;
      this.m_productSelection.VariantIndex = this.m_productImmutable.Variants.IndexOf(this.GetImmutableVariant(product));
      this.m_productSelection.Quantity = this.GetVariantQuantityByIndex(this.m_productSelection.VariantIndex);
    }
    if ((UnityEngine.Object) this.m_container != (UnityEngine.Object) null)
      this.m_container.Variant = product;
    this.m_productSelection.MaxQuantity = product.GetMaxBulkPurchaseCount();
    if (this.m_widget.GetDataModel<ProductSelectionDataModel>() != this.m_productSelection)
      this.m_widget.BindDataModel((IDataModel) this.m_productSelection);
    if (this.OnProductVariantSet == null)
      return;
    this.OnProductVariantSet();
  }

  public ProductDataModel GetSelectedVariant() => this.m_productSelection.Variant;

  public ProductDataModel GetVariantByIndex(int index)
  {
    ProductDataModel product = this.Product;
    return product == null ? (ProductDataModel) null : product.Variants.ElementAtOrDefault<ProductDataModel>(index);
  }

  public void SelectVariantByIndex(int index)
  {
    ProductDataModel variantByIndex = this.GetVariantByIndex(index);
    if (variantByIndex != null)
      this.SelectVariant(variantByIndex);
    else
      Log.Store.PrintWarning("SelectVariantByIndex failed. Product missing variant index {0}", (object) index);
  }

  public int GetVariantQuantityByIndex(int index)
  {
    int num;
    return this.m_variantQuantities.TryGetValue(index, out num) ? num : 1;
  }

  public bool ShowQuantityPromptForVariant(int variantIndex)
  {
    ProductDataModel variant = this.GetVariantByIndex(variantIndex);
    if (variant == null)
    {
      Log.Store.PrintError("ShowQuantityPromptForVariant failed. No variant at index {0}.", (object) variantIndex);
      return false;
    }
    if (!variant.ProductSupportsQuantitySelect())
    {
      Log.Store.Print("ShowQuantityPromptForVariant failed. Product {0} [{1}] does not support quantity select.", (object) variant.PmtId, (object) variant.Name);
      return false;
    }
    if ((UnityEngine.Object) this.m_parentShop.QuantityPrompt == (UnityEngine.Object) null)
    {
      Log.Store.PrintError("ShowQuantityPromptForVariant failed. Shop.QuantityPrompt is null.");
      return false;
    }
    this.m_parentShop.BlockInterface(true);
    this.m_parentShop.QuantityPrompt.Show(this.m_productSelection.MaxQuantity, (StoreQuantityPrompt.OkayListener) (quantity =>
    {
      this.SetVariantQuantityAndUpdateDataModel(variant, quantity);
      this.m_parentShop.BlockInterface(false);
    }), (StoreQuantityPrompt.CancelListener) (() => this.m_parentShop.BlockInterface(false)));
    return true;
  }

  protected virtual void OnProductSet()
  {
  }

  protected virtual ProductDataModel GetFirstVariantToDisplay(
    ProductDataModel chosenProduct,
    ProductDataModel chosenVariant)
  {
    return chosenVariant;
  }

  protected void SetProduct(ProductDataModel product, ProductDataModel variant)
  {
    this.m_productImmutable = product ?? ProductFactory.CreateEmptyProductDataModel();
    this.m_productMutable = (ProductDataModel) null;
    this.m_variantQuantities.Clear();
    this.BindProductDataModel();
    this.OnProductSet();
    this.SelectVariant(variant ?? this.m_productImmutable);
  }

  protected bool OnNavigateBack()
  {
    if (this.IsAnimating)
      return false;
    this.Close();
    return true;
  }

  protected void OnWidgetEvent(string eventName)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(eventName))
    {
      case 968010480:
        if (!(eventName == "SHOP_SKU_DOUBLE_CLICKED_0"))
          break;
        this.ShowQuantityPromptForVariant(0);
        break;
      case 1318030528:
        if (!(eventName == "SHOP_BUY_WITH_FIRST_CURRENCY"))
          break;
        this.TryBuy(0);
        break;
      case 2754205096:
        if (!(eventName == "SHOP_SKU_CLICKED_2"))
          break;
        this.SelectVariantByIndex(2);
        break;
      case 2770982715:
        if (!(eventName == "SHOP_SKU_CLICKED_3"))
          break;
        this.SelectVariantByIndex(3);
        break;
      case 2787760334:
        if (!(eventName == "SHOP_SKU_CLICKED_0"))
          break;
        this.SelectVariantByIndex(0);
        break;
      case 2804537953:
        if (!(eventName == "SHOP_SKU_CLICKED_1"))
          break;
        this.SelectVariantByIndex(1);
        break;
      case 2821315572:
        if (!(eventName == "SHOP_SKU_CLICKED_6"))
          break;
        this.SelectVariantByIndex(6);
        break;
      case 2854870810:
        if (!(eventName == "SHOP_SKU_CLICKED_4"))
          break;
        this.SelectVariantByIndex(4);
        break;
      case 2871648429:
        if (!(eventName == "SHOP_SKU_CLICKED_5"))
          break;
        this.SelectVariantByIndex(5);
        break;
      case 3078142591:
        if (!(eventName == "SHOP_BUY_WITH_ALT_CURRENCY"))
          break;
        this.TryBuy(1);
        break;
    }
  }

  public bool IsOpen { get; private set; }

  public bool IsAnimating { get; set; }

  public virtual void Open()
  {
    if (this.IsOpen)
      return;
    this.IsOpen = true;
    if ((UnityEngine.Object) this.m_container != (UnityEngine.Object) null)
      this.SetProduct(this.m_container.Product, this.GetFirstVariantToDisplay(this.m_container.Product, this.m_container.Variant));
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.m_openWhenReadyCoroutine = this.StartCoroutine(this.OpenWhenReadyRoutine());
  }

  public virtual void Close()
  {
    if (!this.IsOpen)
      return;
    if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null && this.m_parentShop.QuantityPrompt.IsShown())
      this.m_parentShop.QuantityPrompt.Cancel();
    this.IsOpen = false;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    if ((bool) (UnityEngine.Object) this.m_widget)
      this.m_widget.TriggerEvent("CLOSED");
    if (this.m_openWhenReadyCoroutine != null)
    {
      this.StopCoroutine(this.m_openWhenReadyCoroutine);
      this.m_openWhenReadyCoroutine = (Coroutine) null;
    }
    if (this.OnClosed == null)
      return;
    this.OnClosed((object) this, new EventArgs());
  }

  protected ProductDataModel GetImmutableVariant(ProductDataModel variant)
  {
    if (this.Product == variant)
      return this.m_productImmutable;
    if (this.m_productImmutable.Variants.Contains(variant))
      return variant;
    if (this.m_productMutable != null)
    {
      int index = this.m_productMutable.Variants.IndexOf(variant);
      if (index >= 0)
        return this.m_productImmutable.Variants.ElementAtOrDefault<ProductDataModel>(index);
    }
    return (ProductDataModel) null;
  }

  protected virtual void TryBuy(int priceOption)
  {
    if (this.Product == null)
    {
      Log.Store.PrintError("TryBuy failed where no Product is bound to ProductPage");
    }
    else
    {
      ProductDataModel selectedVariant = this.GetSelectedVariant();
      if (selectedVariant == null)
        Log.Store.PrintError("Attempted to purchase, but no selected variant found.");
      else if (!this.ValidateMutableProduct())
      {
        Log.Store.PrintError("Attempted to purchase, but mutable product mismatches immutable product on ProductPage. PMT ID = {0}, Name = {1}", (object) selectedVariant.PmtId, (object) selectedVariant.Name);
      }
      else
      {
        ProductDataModel immuatableSelectedProduct = this.GetImmutableVariant(selectedVariant);
        if (immuatableSelectedProduct == null)
        {
          Log.Store.PrintError("Attempted to purchase but failed to get immutable version of product. PMT ID = {0}, Name = {1}", (object) selectedVariant.PmtId, (object) selectedVariant.Name);
        }
        else
        {
          int quantity = 1;
          if (immuatableSelectedProduct != selectedVariant)
          {
            int index = this.m_productImmutable.Variants.IndexOf(immuatableSelectedProduct);
            if (index < 0)
            {
              Log.Store.PrintError("Attempted to purchase but failed to get index of product. PMT ID = {0}, Name = {1}", (object) selectedVariant.PmtId, (object) selectedVariant.Name);
              return;
            }
            quantity = this.GetVariantQuantityByIndex(index);
            if (quantity < 1)
            {
              Log.Store.PrintError("Attempted to purchase, but selected product quantity is invalid. PMT ID = {0}, Name = {1}, Quantity = {2}", (object) selectedVariant.PmtId, (object) selectedVariant.Name, (object) quantity);
              return;
            }
          }
          if (priceOption < 0 || priceOption >= immuatableSelectedProduct.Prices.Count)
          {
            Log.Store.PrintError("Attempted to purchase, but price index {0} is out of bounds. Num Prices = {1}, PMT ID = {2}, Name = {3}", (object) priceOption, (object) immuatableSelectedProduct.Prices.Count, (object) immuatableSelectedProduct.PmtId, (object) immuatableSelectedProduct.Name);
          }
          else
          {
            PriceDataModel price = immuatableSelectedProduct.Prices[priceOption];
            if (price == null)
              Log.Store.PrintError("Attempted to purchase, but PriceDataModel is null at index {0}. PMT ID = {1}, Name = {2}", (object) priceOption, (object) immuatableSelectedProduct.PmtId, (object) immuatableSelectedProduct.Name);
            else if (this.m_preBuyPopupInfo == null)
            {
              Shop.Get().AttemptToPurchaseProduct(immuatableSelectedProduct, price, quantity);
            }
            else
            {
              this.m_preBuyPopupInfo.m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
              {
                if (response != AlertPopup.Response.CONFIRM && response != AlertPopup.Response.OK)
                  return;
                Shop.Get().AttemptToPurchaseProduct(immuatableSelectedProduct, price, quantity);
              });
              DialogManager.Get().ShowPopup(this.m_preBuyPopupInfo);
            }
          }
        }
      }
    }
  }

  private void CreateMutableProduct()
  {
    this.m_productMutable = this.m_productImmutable.CloneDataModel<ProductDataModel>();
    this.m_productMutable.Variants = new DataModelList<ProductDataModel>();
    this.m_productMutable.Variants.AddRange(this.m_productImmutable.Variants.Select<ProductDataModel, ProductDataModel>((Func<ProductDataModel, ProductDataModel>) (v => this.CreateMutableVariant(v))));
    this.m_variantQuantities.Clear();
  }

  private ProductDataModel CreateMutableVariant(ProductDataModel immutableVariant)
  {
    ProductDataModel mutableVariant = immutableVariant.CloneDataModel<ProductDataModel>();
    mutableVariant.Items = new DataModelList<RewardItemDataModel>();
    mutableVariant.Items.AddRange(immutableVariant.Items.Select<RewardItemDataModel, RewardItemDataModel>((Func<RewardItemDataModel, RewardItemDataModel>) (i => i.CloneDataModel<RewardItemDataModel>())));
    mutableVariant.Prices = new DataModelList<PriceDataModel>();
    mutableVariant.Prices.AddRange(immutableVariant.Prices.Select<PriceDataModel, PriceDataModel>((Func<PriceDataModel, PriceDataModel>) (p => p.CloneDataModel<PriceDataModel>())));
    return mutableVariant;
  }

  private bool ValidateMutableProduct()
  {
    if (this.m_productMutable != null)
    {
      if (this.m_productImmutable == null)
      {
        Log.Store.PrintError("ProductPage has a m_productMutable but no m_productImmutable. Mutable Product PMT ID = {0}, Name = {1}", (object) this.m_productMutable.PmtId, (object) this.m_productMutable.Name);
        return false;
      }
      if (this.m_productMutable.PmtId != this.m_productImmutable.PmtId)
      {
        Log.Store.PrintError("ProductPage Mutable and Immutable products have mismatching PMT id's. Mutable Product PMT ID = {0}, Name = {1}", (object) this.m_productMutable.PmtId, (object) this.m_productMutable.Name);
        return false;
      }
      if (this.m_productMutable.Variants.Count != this.m_productImmutable.Variants.Count)
      {
        Log.Store.PrintError("ProductPage Mutable and Immutable products have mismatching variant counts. Mutable Product PMT ID = {0}, Name = {1}", (object) this.m_productMutable.PmtId, (object) this.m_productMutable.Name);
        return false;
      }
      for (int index = 0; index < this.m_productMutable.Variants.Count; ++index)
      {
        if (this.m_productMutable.Variants.ElementAt<ProductDataModel>(index).PmtId != this.m_productImmutable.Variants.ElementAt<ProductDataModel>(index).PmtId)
        {
          Log.Store.PrintError("ProductPage Mutable and Immutable products have mismatching variant. Mutable Product PMT ID = {0}, Name = {1}", (object) this.m_productMutable.PmtId, (object) this.m_productMutable.Name);
          return false;
        }
      }
    }
    return true;
  }

  protected void SetVariantQuantityAndUpdateDataModel(ProductDataModel variant, int quantity)
  {
    if (variant == null)
      Log.Store.PrintError("Cannot set product quantity. variant is null.");
    else if (!this.ValidateMutableProduct())
    {
      Log.Store.PrintError("Cannot set product quantity. ProductPage has an invalid mutable product.");
    }
    else
    {
      ProductDataModel immutableVariant = this.GetImmutableVariant(variant);
      if (immutableVariant == null)
        Log.Store.PrintError("Cannot set product quantity. No matching immutable variant found. PMT ID = {0}, Name = {1}.", (object) variant.PmtId, (object) variant.Name);
      else if (quantity < 1 || quantity > this.m_productSelection.MaxQuantity)
      {
        Log.Store.PrintError("Cannot set product quantity. Invalid input {0}", (object) quantity);
      }
      else
      {
        int num = this.m_productImmutable.Variants.IndexOf(immutableVariant);
        if (num < 0)
          Log.Store.PrintError("Cannot set product quantity. Variant not found in product. PMT ID = {0}, Name = {1}.", (object) variant.PmtId, (object) variant.Name);
        else if (this.GetVariantQuantityByIndex(num) == quantity)
          Log.Store.Print("SetVariantQuantityAndUpdateDataModel value matches current quantity. Quantity = {0}, ", (object) quantity);
        else if (!immutableVariant.ProductSupportsQuantitySelect())
        {
          Log.Store.PrintError("Cannot set product quantity. Product does not support variable quantity. PMT ID = {0}, Name = {1}", (object) immutableVariant.PmtId, (object) immutableVariant.Name);
        }
        else
        {
          if (this.m_productMutable == null && quantity == 1)
            return;
          if (this.m_productMutable == null)
            this.CreateMutableProduct();
          this.m_variantQuantities[num] = quantity;
          this.m_productSelection.Quantity = quantity;
          ProductDataModel product = this.m_productMutable.Variants.ElementAt<ProductDataModel>(num);
          for (int index = 0; index < product.Items.Count; ++index)
          {
            RewardItemDataModel rewardItemDataModel1 = immutableVariant.Items.ElementAtOrDefault<RewardItemDataModel>(index);
            RewardItemDataModel rewardItemDataModel2 = product.Items.ElementAtOrDefault<RewardItemDataModel>(index);
            if (rewardItemDataModel1 != null && rewardItemDataModel2 != null)
            {
              rewardItemDataModel2.Quantity = rewardItemDataModel1.Quantity * quantity;
              RewardUtils.InitializeRewardItemDataModelForShop(rewardItemDataModel2, (Network.BundleItem) null, (Network.Bundle) null);
            }
            else
              Log.Store.PrintError("Error modifying product item {0}, where immutable product = [{1}], mutable product = [{2}]", (object) index, (object) immutableVariant.Name, (object) product.Name);
          }
          for (int index = 0; index < product.Prices.Count; ++index)
          {
            PriceDataModel priceDataModel1 = immutableVariant.Prices.ElementAtOrDefault<PriceDataModel>(index);
            PriceDataModel priceDataModel2 = product.Prices.ElementAtOrDefault<PriceDataModel>(index);
            if (priceDataModel1 != null && priceDataModel2 != null && priceDataModel1.Currency == priceDataModel2.Currency)
              priceDataModel2.Amount = priceDataModel1.Amount * (float) quantity;
            else
              Log.Store.PrintError("Error modifying product price {0}, where immutable product = [{1}], mutable product = [{2}]", (object) index, (object) immutableVariant.Name, (object) product.Name);
          }
          product.FormatProductPrices();
          product.SetupProductStrings();
          this.BindProductDataModel();
          this.SelectVariant(product);
        }
      }
    }
  }

  protected void BindProductDataModel() => this.m_widget.BindDataModel((IDataModel) this.Product);

  private IEnumerator OpenWhenReadyRoutine()
  {
    ProductPage sender = this;
    try
    {
      while ((bool) (UnityEngine.Object) sender.m_widget && sender.m_widget.IsChangingStates)
        yield return (object) null;
      if ((bool) (UnityEngine.Object) sender.m_widget)
        sender.m_widget.TriggerEvent("OPEN");
      sender.OnOpened((object) sender, EventArgs.Empty);
    }
    finally
    {
      this.m_openWhenReadyCoroutine = (Coroutine) null;
    }
  }
}
