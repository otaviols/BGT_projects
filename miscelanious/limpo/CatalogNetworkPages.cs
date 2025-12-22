using System;
using System.Collections.Generic;
using System.Linq;

public class CatalogNetworkPages
{
  private readonly Dictionary<ShopType, CatalogNetworkPage> m_pages = new Dictionary<ShopType, CatalogNetworkPage>();

  public Dictionary<ShopType, CatalogNetworkPage> Pages => this.m_pages;

  public bool Contains(IEnumerable<ShopType> shopTypes) => shopTypes.All<ShopType>((Func<ShopType, bool>) (requested => this.m_pages.ContainsKey(requested)));

  public CatalogNetworkPage GetOrCreatePage(ShopType shopType)
  {
    CatalogNetworkPage page;
    if (!this.m_pages.TryGetValue(shopType, out page))
    {
      page = new CatalogNetworkPage();
      this.m_pages[shopType] = page;
    }
    return page;
  }

  public bool HasPages() => this.m_pages.Count > 0;
}
