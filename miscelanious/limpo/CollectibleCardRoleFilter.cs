using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CollectibleCardRoleFilter : CollectibleCardFilter
{
  private int m_cardsPerPage = 8;
  private TAG_ROLE[] m_roleTabOrder;
  private Map<TAG_ROLE, List<LettuceMercenary>> m_currentResultsByRole = new Map<TAG_ROLE, List<LettuceMercenary>>();
  private bool? m_filterOwned = new bool?(true);
  private bool? m_filterOnlyUpgradeable;

  public CollectionManager.FindMercenariesResult FindMercenariesResult { get; protected set; }

  public void Init(TAG_ROLE[] roleTabOrder, int cardsPerPage)
  {
    this.m_roleTabOrder = roleTabOrder;
    this.m_cardsPerPage = cardsPerPage;
    for (int index = 0; index < roleTabOrder.Length; ++index)
      this.m_currentResultsByRole[roleTabOrder[index]] = new List<LettuceMercenary>();
  }

  public CollectionManager.FindMercenariesResult GenerateMercenariesResults() => CollectionManager.Get().FindMercenaries(this.m_filterText, this.m_filterOwned, this.m_filterOnlyUpgradeable);

  public override void UpdateResults()
  {
    this.FindMercenariesResult = this.GenerateMercenariesResults();
    List<LettuceMercenary> mercenaries = this.FindMercenariesResult.m_mercenaries;
    foreach (KeyValuePair<TAG_ROLE, List<LettuceMercenary>> keyValuePair in this.m_currentResultsByRole)
      keyValuePair.Value.Clear();
    foreach (LettuceMercenary lettuceMercenary in mercenaries)
    {
      if (this.m_filterRoles == null || ((IEnumerable<TAG_ROLE>) this.m_filterRoles).Contains<TAG_ROLE>(lettuceMercenary.m_role))
      {
        if (!this.m_currentResultsByRole.ContainsKey(lettuceMercenary.m_role))
        {
          Error.AddDevFatal("Mercenary: {0} ({1}) has an invalid role: {2}. Cannot render page.", (object) lettuceMercenary.m_mercName, (object) lettuceMercenary.ID, (object) lettuceMercenary.m_role);
          break;
        }
        this.m_currentResultsByRole[lettuceMercenary.m_role].Add(lettuceMercenary);
      }
    }
  }

  public override void FilterOnlyOwned(bool owned)
  {
    base.FilterOnlyOwned(owned);
    this.m_filterOwned = new bool?();
    if (!owned)
      return;
    this.m_filterOwned = new bool?(owned);
  }

  public void FilterOnlyUpgradeableMercs(bool onlyUpgradeable)
  {
    this.m_filterOnlyUpgradeable = new bool?();
    if (!onlyUpgradeable)
      return;
    this.m_filterOnlyUpgradeable = new bool?(onlyUpgradeable);
  }

  public int GetNumPagesForRole(TAG_ROLE cardRole)
  {
    List<LettuceMercenary> lettuceMercenaryList;
    if (!this.m_currentResultsByRole.TryGetValue(cardRole, out lettuceMercenaryList))
      return 0;
    int count = lettuceMercenaryList.Count;
    return count / this.m_cardsPerPage + (count % this.m_cardsPerPage > 0 ? 1 : 0);
  }

  public int GetNumNewCardsForRole(TAG_ROLE cardRole) => CollectionManager.Get().GetNumMercenariesToAcknowledgeForRole(cardRole);

  public override int GetTotalNumPages()
  {
    int totalNumPages = 0;
    foreach (TAG_ROLE cardRole in this.m_roleTabOrder)
      totalNumPages += this.GetNumPagesForRole(cardRole);
    return totalNumPages;
  }

  public List<LettuceMercenary> GetMercenariesPageContents(int page)
  {
    if (page < 0 || page > this.GetTotalNumPages())
      return new List<LettuceMercenary>();
    int num1 = 0;
    for (int index = 0; index < this.m_roleTabOrder.Length; ++index)
    {
      int num2 = num1;
      TAG_ROLE tagRole = this.m_roleTabOrder[index];
      num1 += this.GetNumPagesForRole(tagRole);
      if (page <= num1)
      {
        int pageWithinRole = page - num2;
        return this.GetPageContentsForRole(tagRole, pageWithinRole, false, out int _);
      }
    }
    return new List<LettuceMercenary>();
  }

  public override List<CollectibleCard> GetPageContents(int page) => new List<CollectibleCard>();

  public TAG_ROLE GetCurrentRoleFromPage(int page)
  {
    if (page < 0 || page > this.GetTotalNumPages())
      return TAG_ROLE.INVALID;
    int num = 0;
    for (int index = 0; index < this.m_roleTabOrder.Length; ++index)
    {
      TAG_ROLE cardRole = this.m_roleTabOrder[index];
      num += this.GetNumPagesForRole(cardRole);
      if (page <= num)
        return cardRole;
    }
    return TAG_ROLE.INVALID;
  }

  public List<LettuceMercenary> GetFirstNonEmptyMercenaryPage(
    out int collectionPage)
  {
    collectionPage = 0;
    TAG_ROLE pageRole = TAG_ROLE.FIGHTER;
    for (int index = 0; index < this.m_roleTabOrder.Length; ++index)
    {
      if (this.m_currentResultsByRole[this.m_roleTabOrder[index]].Count > 0)
      {
        pageRole = this.m_roleTabOrder[index];
        break;
      }
    }
    return this.GetPageContentsForRole(pageRole, 1, true, out collectionPage);
  }

  public override List<CollectibleCard> GetFirstNonEmptyPage(
    out int collectionPage)
  {
    collectionPage = 0;
    return new List<CollectibleCard>();
  }

  public List<LettuceMercenary> GetPageContentsForRole(
    TAG_ROLE pageRole,
    int pageWithinRole,
    bool calculateCollectionPage,
    out int collectionPage)
  {
    collectionPage = 0;
    if (pageWithinRole <= 0 || pageWithinRole > this.GetNumPagesForRole(pageRole))
      return new List<LettuceMercenary>();
    if (calculateCollectionPage)
    {
      for (int index = 0; index < this.m_roleTabOrder.Length; ++index)
      {
        TAG_ROLE cardRole = this.m_roleTabOrder[index];
        if (cardRole != pageRole)
          collectionPage += this.GetNumPagesForRole(cardRole);
        else
          break;
      }
      collectionPage += pageWithinRole;
    }
    List<LettuceMercenary> source = this.m_currentResultsByRole[pageRole];
    return source == null ? new List<LettuceMercenary>() : source.Skip<LettuceMercenary>(this.m_cardsPerPage * (pageWithinRole - 1)).Take<LettuceMercenary>(this.m_cardsPerPage).ToList<LettuceMercenary>();
  }

  public List<LettuceMercenary> GetPageContentsForMercenary(
    LettuceMercenary merc,
    out int collectionPage)
  {
    collectionPage = 0;
    TAG_ROLE role = merc.m_role;
    int index = this.m_currentResultsByRole[role].FindIndex((Predicate<LettuceMercenary>) (m => m.ID == merc.ID));
    if (index < 0)
      return new List<LettuceMercenary>();
    int num = index + 1;
    int pageWithinRole = num / this.m_cardsPerPage + (num % this.m_cardsPerPage > 0 ? 1 : 0);
    return this.GetPageContentsForRole(role, pageWithinRole, true, out collectionPage);
  }

  public List<LettuceMercenary> GetAllRoleResults()
  {
    List<LettuceMercenary> allRoleResults = new List<LettuceMercenary>();
    foreach (KeyValuePair<TAG_ROLE, List<LettuceMercenary>> keyValuePair in this.m_currentResultsByRole)
      allRoleResults.AddRange((IEnumerable<LettuceMercenary>) keyValuePair.Value);
    return allRoleResults;
  }

  public static List<CollectionManager.MercenaryFilterFunc> FilterMercsFromSearchString(
    string searchString,
    ref CollectibleCardRoleFilter.SearchTerms setSearchTerms)
  {
    List<CollectionManager.MercenaryFilterFunc> mercenaryFilterFuncList = new List<CollectionManager.MercenaryFilterFunc>();
    string str1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ARTIST");
    string str2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_HEALTH");
    string str3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ATTACK");
    string str4 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_OWNED");
    string str5 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_TYPE");
    string str6 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING");
    string str7 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_NEW");
    string str8 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_HAS");
    string str9 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MAX_LEVEL");
    string str10 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MAX_LEVEL_ALT");
    string str11 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_GOLDEN");
    string str12 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_DIAMOND");
    string str13 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_CRAFTABLE");
    string str14 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_UPGRADABLE");
    string[] strArray1 = searchString.ToLower().Split(CollectibleFilteredSet<ICollectible>.SearchTokenDelimiters, StringSplitOptions.RemoveEmptyEntries);
    StringBuilder regularTokens = new StringBuilder();
    for (int index = 0; index < strArray1.Length; ++index)
    {
      if (strArray1[index] == str6)
        setSearchTerms.Missing = true;
      else if (strArray1[index] == str4)
        setSearchTerms.Owned = true;
      else if (strArray1[index] == str11)
        mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.GetEquippedArtVariation().m_premium == TAG_PREMIUM.GOLDEN));
      else if (strArray1[index] == str12)
        mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.GetEquippedArtVariation().m_premium == TAG_PREMIUM.DIAMOND));
      else if (strArray1[index] == str9 || strArray1[index] == str10)
        mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.IsMaxLevel()));
      else if (strArray1[index].Contains(str13))
        mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.IsReadyForCrafting()));
      else if (strArray1[index].Contains(str14))
        mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.CanAnyCardBeUpgraded()));
      else if (strArray1[index] == str7)
      {
        mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => CollectionManager.Get().DoesMercenaryNeedToBeAcknowledged(card)));
      }
      else
      {
        bool flag = false;
        if (((IEnumerable<char>) CollectibleFilteredSet<ICollectible>.SearchTagColons).Any<char>(new Func<char, bool>(((StringUtils) strArray1[index]).Contains)))
        {
          string[] strArray2 = strArray1[index].Split(CollectibleFilteredSet<ICollectible>.SearchTagColons);
          if (strArray2.Length == 2)
          {
            string str15 = strArray2[0].Trim();
            string val = strArray2[1].Trim();
            bool isNumericalValue;
            int minVal;
            int maxVal;
            GeneralUtils.ParseNumericRange(val, out isNumericalValue, out minVal, out maxVal);
            if (isNumericalValue)
            {
              if (str15 == str3)
              {
                mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.m_attack >= minVal && card.m_attack <= maxVal));
                flag = true;
              }
              if (str15 == str2)
              {
                mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.m_health >= minVal && card.m_health <= maxVal));
                flag = true;
              }
            }
            else
            {
              if (str15 == str1)
              {
                mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => SearchableString.SearchInternationalText(val, card.GetCollectibleCard().ArtistName)));
                mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => SearchableString.SearchInternationalText(val, card.GetCollectibleCard().SignatureArtistName)));
                flag = true;
              }
              if (str15 == str5)
              {
                mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card =>
                {
                  string cardTypeName = GameStrings.GetCardTypeName(card.GetCollectibleCard().CardType);
                  return cardTypeName != null && SearchableString.SearchInternationalText(val, cardTypeName);
                }));
                flag = true;
              }
              if (str15 == str8)
              {
                mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.FindTextInCard(val)));
                flag = true;
              }
              if (str15 == str3)
              {
                string lower1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EVEN_ATTACK").ToLower();
                string lower2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ODD_ATTACK").ToLower();
                string lower3 = val.ToLower();
                if (lower3 == lower1)
                {
                  mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.m_attack % 2 == 0));
                  flag = true;
                }
                else if (lower3 == lower2)
                {
                  mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.m_attack % 2 == 1));
                  flag = true;
                }
              }
              if (str15 == str2)
              {
                string lower4 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EVEN_HEALTH").ToLower();
                string lower5 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ODD_HEALTH").ToLower();
                string lower6 = val.ToLower();
                if (lower6 == lower4)
                {
                  mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.m_health % 2 == 0));
                  flag = true;
                }
                else if (lower6 == lower5)
                {
                  mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.m_health % 2 == 1));
                  flag = true;
                }
              }
            }
          }
        }
        if (!flag)
        {
          regularTokens.Append(strArray1[index]);
          regularTokens.Append(" ");
        }
      }
    }
    mercenaryFilterFuncList.Add((CollectionManager.MercenaryFilterFunc) (card => card.FindTextInCard(regularTokens.ToString())));
    return mercenaryFilterFuncList;
  }

  public struct SearchTerms
  {
    public bool Missing;
    public bool Owned;
  }
}
