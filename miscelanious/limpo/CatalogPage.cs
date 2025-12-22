using Hearthstone.DataModels;
using System.Collections.Generic;

public class CatalogPage
{
  private readonly List<ProductTierDataModel> m_tiers = new List<ProductTierDataModel>();

  public List<ProductTierDataModel> Tiers => this.m_tiers;

  public void AddTier(ProductTierDataModel tier) => this.m_tiers.Add(tier);

  public void AddTiers(IEnumerable<ProductTierDataModel> tiers) => this.m_tiers.AddRange(tiers);
}
