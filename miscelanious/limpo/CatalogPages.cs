using Hearthstone.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

public class CatalogPages
{
  private readonly Dictionary<ShopType, CatalogPage> m_pages = new Dictionary<ShopType, CatalogPage>();

  public bool HasTiers => this.GetTiers_All().Any<ProductTierDataModel>();

  public List<ProductTierDataModel> GetTiers(ShopType shopType)
  {
    CatalogPage catalogPage;
    return this.m_pages.TryGetValue(shopType, out catalogPage) ? catalogPage.Tiers : new List<ProductTierDataModel>();
  }

  public List<ProductTierDataModel> GetTiers_All() => this.m_pages.Values.SelectMany<CatalogPage, ProductTierDataModel>((Func<CatalogPage, IEnumerable<ProductTierDataModel>>) (page => (IEnumerable<ProductTierDataModel>) page.Tiers)).ToList<ProductTierDataModel>();

  public void AddTier(ShopType shopType, ProductTierDataModel tier) => this.GetOrAddPage(shopType).AddTier(tier);

  public void AddTiers(
    ShopType shopType,
    IEnumerable<ProductTierDataModel> productTierDataModels)
  {
    this.GetOrAddPage(shopType).AddTiers(productTierDataModels);
  }

  public void Clear() => this.m_pages.Clear();

  private CatalogPage GetOrAddPage(ShopType shopType)
  {
    CatalogPage orAddPage;
    if (!this.m_pages.TryGetValue(shopType, out orAddPage))
    {
      orAddPage = new CatalogPage();
      this.m_pages[shopType] = orAddPage;
    }
    return orAddPage;
  }
}
