using PegasusUtil;

public class NoGTAPPTransactionData
{
  public NoGTAPPTransactionData()
  {
    this.Product = ProductType.PRODUCT_TYPE_UNKNOWN;
    this.ProductData = 0;
    this.Quantity = 0;
  }

  public ProductType Product { get; set; }

  public int ProductData { get; set; }

  public int Quantity { get; set; }
}
