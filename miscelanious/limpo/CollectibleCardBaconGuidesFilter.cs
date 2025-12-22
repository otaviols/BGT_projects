using System.Collections.Generic;
using System.Linq;

public class CollectibleCardBaconGuidesFilter : CollectibleCardFilter
{
  private int m_guidesPerPage = 6;
  private int m_guideCount;
  private int m_totalPages;
  private List<CollectibleCard> m_allBGGuides = new List<CollectibleCard>();
  private static CollectibleCardBaconGuidesFilter.GuideSkinComparer s_GuideSkinComparer = new CollectibleCardBaconGuidesFilter.GuideSkinComparer();

  public void Init(int guidesPerPage) => this.m_guidesPerPage = guidesPerPage;

  public override void UpdateResults()
  {
    this.m_allBGGuides.Clear();
    List<CollectionManager.CollectibleCardFilterFunc> collectibleCardFilterFuncList = new List<CollectionManager.CollectibleCardFilterFunc>();
    if (!string.IsNullOrEmpty(this.m_filterText))
      collectibleCardFilterFuncList.AddRange((IEnumerable<CollectionManager.CollectibleCardFilterFunc>) this.FiltersFromSearchString(this.m_filterText));
    List<string> battlegroundsGuideCardIds = CollectionManager.Get().GetAllBattlegroundsGuideCardIds();
    for (int index1 = 0; index1 < battlegroundsGuideCardIds.Count; ++index1)
    {
      string str = battlegroundsGuideCardIds[index1];
      EntityDef entityDef = DefLoader.Get().GetEntityDef(str);
      CollectibleCard card = new CollectibleCard(GameUtils.GetCardRecord(str), entityDef, TAG_PREMIUM.NORMAL);
      card.OwnedCount = !entityDef.HasTag(GAME_TAG.BACON_BOB_SKIN) ? 1 : (CollectionManager.Get().OwnsBattlegroundsGuideSkin(card.CardId) ? 1 : 0);
      card.SeenCount = card.OwnedCount;
      if (card.SeenCount > 0 && CollectionManager.Get().ShouldShowNewBattlegroundsGuideSkinGlow(str))
        card.SeenCount = 0;
      bool flag = false;
      for (int index2 = 0; index2 < collectibleCardFilterFuncList.Count; ++index2)
      {
        if (!collectibleCardFilterFuncList[index2](card))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        this.m_allBGGuides.Add(card);
    }
    this.m_allBGGuides.Sort((IComparer<CollectibleCard>) CollectibleCardBaconGuidesFilter.s_GuideSkinComparer);
    this.m_guideCount = this.m_allBGGuides.Count;
    this.m_totalPages = this.m_guideCount / this.m_guidesPerPage + (this.m_guideCount % this.m_guidesPerPage > 0 ? 1 : 0);
  }

  public override List<CollectibleCard> GetPageContents(int page) => this.m_allBGGuides.Skip<CollectibleCard>(this.m_guidesPerPage * (page - 1)).Take<CollectibleCard>(this.m_guidesPerPage).ToList<CollectibleCard>();

  public override List<CollectibleCard> GetFirstNonEmptyPage(
    out int collectionPage)
  {
    collectionPage = 0;
    for (int currentPageNumber = 0; currentPageNumber < this.GetTotalNumPages(); ++currentPageNumber)
    {
      List<CollectibleCard> pageContents = this.GetPageContents(currentPageNumber);
      if (pageContents.Count > 0)
      {
        collectionPage = currentPageNumber;
        return pageContents;
      }
    }
    return new List<CollectibleCard>();
  }

  public override int GetTotalNumPages() => this.m_totalPages;

  private class GuideSkinComparer : IComparer<CollectibleCard>
  {
    public int Compare(CollectibleCard card1, CollectibleCard card2)
    {
      bool flag1 = !card1.GetEntityDef().HasTag(GAME_TAG.BACON_BOB_SKIN);
      bool flag2 = !card2.GetEntityDef().HasTag(GAME_TAG.BACON_BOB_SKIN);
      if (flag1 && !flag2)
        return -1;
      if (flag2 && !flag1)
        return 1;
      bool flag3 = CollectionManager.Get().OwnsBattlegroundsGuideSkin(card1.CardDbId);
      bool flag4 = CollectionManager.Get().OwnsBattlegroundsGuideSkin(card2.CardDbId);
      if (flag3 && !flag4)
        return -1;
      return flag4 && !flag3 ? 1 : GameStrings.Get(card1.Name).CompareTo(GameStrings.Get(card2.Name));
    }
  }
}
