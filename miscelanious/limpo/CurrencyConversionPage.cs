using Hearthstone.DataModels;
using System;
using System.Linq;
using UnityEngine;

public class CurrencyConversionPage : ProductPage
{
  [SerializeField]
  private PegUIElement m_buttonIncrease;
  [SerializeField]
  private PegUIElement m_buttonDecrease;
  [SerializeField]
  private ScrollbarControl m_slider;
  [Tooltip("Widget event when the player can afford to convert")]
  [SerializeField]
  private string m_affordableEventName = "AFFORDABLE";
  [SerializeField]
  [Tooltip("Widget event when the player cannot afford conversion")]
  private string m_unaffordableEventName = "UNAFFORDABLE";
  private const int MINIMUM_SELECTION = 1;
  private float m_baseQuantity;
  private int m_selectedQuantity = 1;
  private int m_maxAffordable;
  private RangeInt m_sliderRange;

  protected override void Start()
  {
    base.Start();
    this.SetupIncrementerButton(this.m_buttonIncrease, 1);
    this.SetupIncrementerButton(this.m_buttonDecrease, -1);
    if ((UnityEngine.Object) this.m_slider != (UnityEngine.Object) null)
    {
      this.m_slider.SetUpdateHandler(new ScrollbarControl.UpdateHandler(this.OnSliderUpdated));
      this.m_slider.SetFinishHandler(new ScrollbarControl.FinishHandler(this.OnSliderFinished));
    }
    Shop shop = Shop.Get();
    if ((UnityEngine.Object) shop != (UnityEngine.Object) null)
      shop.CurrencyBalanceChanged += new Action<CurrencyBalanceChangedEventArgs>(this.HandleCurrencyBalanceChanged);
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.HandleSuccessfulPurchaseAck));
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    Shop shop = Shop.Get();
    if ((UnityEngine.Object) shop != (UnityEngine.Object) null)
      shop.CurrencyBalanceChanged -= new Action<CurrencyBalanceChangedEventArgs>(this.HandleCurrencyBalanceChanged);
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.HandleSuccessfulPurchaseAck));
  }

  public void OpenToSKU(float desiredAmount)
  {
    this.Open();
    if ((double) this.m_baseQuantity <= 0.0)
      return;
    this.m_selectedQuantity = this.ClampSelection(Mathf.CeilToInt(desiredAmount / this.m_baseQuantity));
    this.UpdateQuantity();
  }

  public override void Open()
  {
    base.Open();
    if (this.m_productImmutable == null)
    {
      ProductDataModel dataModel = this.m_widget.GetDataModel<ProductDataModel>();
      this.SetProduct(dataModel, dataModel);
    }
    RewardItemDataModel currencyItem = this.GetCurrencyItem();
    this.m_baseQuantity = currencyItem != null ? currencyItem.Currency.Amount : 0.0f;
    this.UpdateConstraints();
  }

  private void OnSliderUpdated(float val)
  {
    int num = Mathf.RoundToInt((float) this.m_sliderRange.start + val * (float) this.m_sliderRange.length);
    if (this.m_selectedQuantity == num)
      return;
    this.m_selectedQuantity = num;
    this.UpdateModel();
  }

  private void OnSliderFinished() => this.UpdateSlider();

  private void HandleCurrencyBalanceChanged(CurrencyBalanceChangedEventArgs args)
  {
    if (!this.IsOpen)
      return;
    RewardItemDataModel currencyItem = this.GetCurrencyItem();
    PriceDataModel price = this.GetPrice();
    if ((currencyItem == null || args.Currency != currencyItem.Currency.Currency) && (price == null || args.Currency != price.Currency))
      return;
    this.UpdateConstraints();
  }

  private void HandleSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod purchaseMethod) => this.Close();

  private RewardItemDataModel GetCurrencyItem()
  {
    ProductDataModel productImmutable = this.m_productImmutable;
    if (productImmutable == null)
    {
      Log.Store.PrintError("No currency conversion product set");
      return (RewardItemDataModel) null;
    }
    RewardItemDataModel currencyItem = productImmutable.Items.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (i => i.Currency != null));
    if (currencyItem != null && currencyItem.Currency != null && (double) currencyItem.Currency.Amount != 0.0)
      return currencyItem;
    Log.Store.PrintError("No currency found on product {0}", (object) productImmutable.Name);
    return (RewardItemDataModel) null;
  }

  private PriceDataModel GetPrice() => this.m_productImmutable == null ? (PriceDataModel) null : this.m_productImmutable.Prices.FirstOrDefault<PriceDataModel>();

  private void SetupIncrementerButton(PegUIElement ui, int increment)
  {
    if ((UnityEngine.Object) ui == (UnityEngine.Object) null)
      return;
    ui.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (_ => this.IncrementQuantity(increment)));
    ui.AddEventListener(UIEventType.HOLD, (UIEvent.Handler) (_ => this.IncrementQuantity(increment)));
  }

  private void IncrementQuantity(int delta)
  {
    this.m_selectedQuantity = this.ClampSelection(this.m_selectedQuantity + delta);
    this.UpdateQuantity();
  }

  private void UpdateConstraints()
  {
    ProductDataModel productImmutable = this.m_productImmutable;
    if (productImmutable == null)
    {
      Log.Store.PrintError("Unable to update VC conversion constraints; no product set");
    }
    else
    {
      PriceDataModel price = this.GetPrice();
      if (price == null || (double) price.Amount == 0.0)
      {
        Log.Store.PrintError("No price on currency product {0}", (object) productImmutable.Name);
      }
      else
      {
        long cachedBalance = ShopUtils.GetCachedBalance(price.Currency);
        this.m_productSelection.MaxQuantity = productImmutable.GetMaxBulkPurchaseCount();
        this.m_maxAffordable = Math.Min(Mathf.FloorToInt((float) cachedBalance / price.Amount), this.m_productSelection.MaxQuantity);
        this.m_sliderRange.start = Math.Min(1, this.m_maxAffordable);
        this.m_sliderRange.length = this.m_maxAffordable - this.m_sliderRange.start;
        this.m_selectedQuantity = this.ClampSelection(this.m_selectedQuantity);
        this.m_widget.TriggerEvent(this.m_maxAffordable > 0 ? this.m_affordableEventName : this.m_unaffordableEventName);
        this.UpdateQuantity();
      }
    }
  }

  private void UpdateQuantity()
  {
    this.UpdateModel();
    this.UpdateSlider();
  }

  private void UpdateModel()
  {
    this.SetVariantQuantityAndUpdateDataModel(this.m_productImmutable, this.m_selectedQuantity);
    this.Selection.MaxQuantity = this.m_maxAffordable;
  }

  private void UpdateSlider()
  {
    if (!((UnityEngine.Object) this.m_slider != (UnityEngine.Object) null))
      return;
    if (this.m_sliderRange.length > 0)
      this.m_slider.SetValue((float) (this.m_selectedQuantity - this.m_sliderRange.start) / (float) this.m_sliderRange.length);
    else
      this.m_slider.SetValue(this.m_selectedQuantity == 0 ? 0.0f : 1f);
  }

  private int ClampSelection(int amount) => Math.Max(1, Mathf.Clamp(amount, this.m_sliderRange.start, this.m_sliderRange.end));
}
