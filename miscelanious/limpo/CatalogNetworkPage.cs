using System.Collections.Generic;

public class CatalogNetworkPage
{
  private List<Network.ShopSection> m_sections = new List<Network.ShopSection>();

  public List<Network.ShopSection> Sections => this.m_sections;

  public int SectionsCount => this.m_sections.Count;

  public void Clear() => this.m_sections.Clear();

  public Network.ShopSection GetSectionBySortOrder(int sortOrder)
  {
    int index = 0;
    for (int count = this.m_sections.Count; index < count; ++index)
    {
      Network.ShopSection section = this.m_sections[index];
      if (section.SortOrder == sortOrder)
        return section;
    }
    return (Network.ShopSection) null;
  }

  public void AddSection(Network.ShopSection section) => this.m_sections.Add(section);
}
