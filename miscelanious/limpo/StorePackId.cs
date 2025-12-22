using PegasusUtil;

public struct StorePackId
{
  public StorePackType Type;
  public int Id;

  public static bool operator ==(StorePackId a, StorePackId b) => a.Type == b.Type && a.Id == b.Id;

  public override bool Equals(object obj) => ((StorePackId) obj).Type == this.Type && ((StorePackId) obj).Id == this.Id;

  public override int GetHashCode() => this.Type.GetHashCode() ^ this.Id;

  public static ProductType GetProductTypeFromStorePackType(StorePackId storePackId)
  {
    switch (storePackId.Type)
    {
      case StorePackType.BOOSTER:
        return !GameUtils.IsHiddenLicenseBundleBooster(storePackId) ? ProductType.PRODUCT_TYPE_BOOSTER : ProductType.PRODUCT_TYPE_HIDDEN_LICENSE;
      case StorePackType.MODULAR_BUNDLE:
        return ProductType.PRODUCT_TYPE_HIDDEN_LICENSE;
      default:
        return ProductType.PRODUCT_TYPE_UNKNOWN;
    }
  }
}
