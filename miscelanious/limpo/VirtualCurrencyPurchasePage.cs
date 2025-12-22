using Hearthstone.DataModels;
using System;

public class VirtualCurrencyPurchasePage : ProductPage
{
  private bool m_closeOnPurchase;

  protected override void Start()
  {
    base.Start();
    StoreManager.Get()?.RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.HandleSuccessfulPurchaseAck));
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    StoreManager.Get()?.RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.HandleSuccessfulPurchaseAck));
  }

  public void OpenToSKU(float desiredAmount, bool closeOnPurchase = false)
  {
    ProductDataModel vcVariant = (ProductDataModel) null;
    CurrencyType currencyType;
    if (ShopUtils.TryGetMainVirtualCurrencyType(out currencyType))
      vcVariant = ShopUtils.FindCurrencyProduct(currencyType, desiredAmount);
    this.OpenToSKU(vcVariant, closeOnPurchase);
  }

  public void OpenToSKU(ProductDataModel vcVariant, bool closeOnPurchase = false)
  {
    this.Open();
    this.m_closeOnPurchase = closeOnPurchase;
    if (vcVariant == null)
    {
      Log.Store.PrintError("Invalid Virtual Currency variant was provided.");
    }
    else
    {
      ProductDataModel currencyProductItem = StoreManager.Get().Catalog.VirtualCurrencyProductItem;
      if (!currencyProductItem.Variants.Contains(vcVariant))
        Log.Store.PrintError("Attempted to display Product PMT ID = {0}, Name = {1} as Virtual Currency", (object) vcVariant.PmtId, (object) vcVariant.Name);
      else
        this.SetProduct(currencyProductItem, vcVariant);
    }
  }

  private void HandleSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (!this.IsOpen || !this.m_closeOnPurchase)
      return;
    this.Close();
  }
}
