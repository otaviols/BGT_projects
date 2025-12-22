using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleCardHeroesFilter : CollectibleCardFilter
{
  private static readonly Map<int, int> s_forcedPairs = new Map<int, int>()
  {
    {
      7,
      57751
    },
    {
      1066,
      57753
    },
    {
      930,
      57755
    },
    {
      671,
      57757
    },
    {
      31,
      57759
    },
    {
      274,
      57761
    },
    {
      893,
      57763
    },
    {
      637,
      57765
    },
    {
      813,
      57767
    },
    {
      56550,
      60238
    }
  };
  private static readonly Map<TAG_CLASS, int> s_classOrder = new Map<TAG_CLASS, int>()
  {
    {
      TAG_CLASS.DEATHKNIGHT,
      0
    },
    {
      TAG_CLASS.DEMONHUNTER,
      100
    },
    {
      TAG_CLASS.DRUID,
      200
    },
    {
      TAG_CLASS.HUNTER,
      300
    },
    {
      TAG_CLASS.MAGE,
      400
    },
    {
      TAG_CLASS.PALADIN,
      500
    },
    {
      TAG_CLASS.PRIEST,
      600
    },
    {
      TAG_CLASS.ROGUE,
      700
    },
    {
      TAG_CLASS.SHAMAN,
      800
    },
    {
      TAG_CLASS.WARLOCK,
      900
    },
    {
      TAG_CLASS.WARRIOR,
      1000
    },
    {
      TAG_CLASS.NEUTRAL,
      1100
    }
  };
  private static Comparison<CollectibleCard> SortHeroResults = (Comparison<CollectibleCard>) ((a, b) =>
  {
    int num = b.OwnedCount.CompareTo(a.OwnedCount);
    if (num == 0)
    {
      num = ((int) a.Class).CompareTo((int) b.Class);
      if (num == 0)
        num = string.Compare(a.Name, b.Name, false, Localization.GetCultureInfo());
    }
    return num;
  });
  private const int UNLOCKABLE_SORT_VALUE = 1;
  private const int UNFAVORITED_SORT_VALUE = 1200;
  private const int UNOWNED_PURCHASABLE_SORT_VALUE = 10000;
  private const int UNOWNED_UNPURCHASABLE_SORT_VALUE = 20000;
  private TAG_CLASS[] m_classTabOrder;
  private int m_heroesPerPage = 6;
  private List<CollectibleCard> m_results = new List<CollectibleCard>();
  private List<CollectibleCard> m_unfilteredResults = new List<CollectibleCard>();
  private Map<TAG_CLASS, List<CollectibleCard>> m_currentResultsByClass = new Map<TAG_CLASS, List<CollectibleCard>>();

  public void Init(int heroesPerPage)
  {
    this.m_heroesPerPage = heroesPerPage;
    this.FilterHero(true);
    this.FilterOnlyOwned(false);
  }

  public override void UpdateResults()
  {
    this.m_unfilteredResults = this.GenerateUnOrderedResults().m_cards;
    this.FilterGoldenHeroes();
    this.SortResults();
    this.FilterHeroesByActiveClass();
  }

  public void SortResults()
  {
    this.m_unfilteredResults.Sort(CollectibleCardHeroesFilter.SortHeroResults);
    this.m_unfilteredResults = this.m_unfilteredResults.OrderBy<CollectibleCard, int>(new Func<CollectibleCard, int>(this.HeroSkinSortValue)).ToList<CollectibleCard>();
    this.EnforcePairingPositions();
  }

  public void FilterHeroesByActiveClass()
  {
    this.m_results = new List<CollectibleCard>((IEnumerable<CollectibleCard>) this.m_unfilteredResults);
    CollectionDeck editedDeck = CollectionManager.Get()?.GetEditedDeck();
    if (editedDeck != null)
    {
      TAG_CLASS tagClass = editedDeck.GetClass();
      for (int index = this.m_results.Count - 1; index > -1; --index)
      {
        if (this.m_results[index].Class != tagClass)
          this.m_results.RemoveAt(index);
      }
    }
    else
    {
      TAG_CLASS? nullable1 = CollectionManager.Get()?.GetCollectibleDisplay() is CollectionManagerDisplay collectibleDisplay ? collectibleDisplay.GetHeroSkinClass() : new TAG_CLASS?();
      if (!nullable1.HasValue || this.m_filterText != null)
        return;
      for (int index = this.m_results.Count - 1; index > -1; --index)
      {
        int num = (int) this.m_results[index].Class;
        TAG_CLASS? nullable2 = nullable1;
        int valueOrDefault = (int) nullable2.GetValueOrDefault();
        if (!(num == valueOrDefault & nullable2.HasValue))
          this.m_results.RemoveAt(index);
      }
    }
  }

  public List<CollectibleCard> GetAllResults() => this.m_results;

  public override List<CollectibleCard> GetPageContents(int page) => this.GetHeroesContents(page);

  public List<CollectibleCard> GetHeroesContents(int currentPage)
  {
    currentPage = Mathf.Min(currentPage, this.GetTotalNumPages());
    return this.m_results.Skip<CollectibleCard>(this.m_heroesPerPage * (currentPage - 1)).Take<CollectibleCard>(this.m_heroesPerPage).ToList<CollectibleCard>();
  }

  public override List<CollectibleCard> GetFirstNonEmptyPage(
    out int collectionPage)
  {
    collectionPage = 0;
    for (int currentPage = 0; currentPage < this.GetTotalNumPages(); ++currentPage)
    {
      List<CollectibleCard> heroesContents = this.GetHeroesContents(currentPage);
      if (heroesContents.Count > 0)
      {
        collectionPage = currentPage;
        return heroesContents;
      }
    }
    return new List<CollectibleCard>();
  }

  public override int GetTotalNumPages()
  {
    int count = this.m_results.Count;
    return count / this.m_heroesPerPage + (count % this.m_heroesPerPage > 0 ? 1 : 0);
  }

  private int HeroSkinSortValue(CollectibleCard card)
  {
    int num1 = 0;
    TAG_CLASS key = card.Class;
    int num2 = CollectibleCardHeroesFilter.s_classOrder[TAG_CLASS.NEUTRAL];
    if (CollectibleCardHeroesFilter.s_classOrder.ContainsKey(key))
      num2 = CollectibleCardHeroesFilter.s_classOrder[key];
    int num3 = num1 + num2;
    if (CardBackManager.Get().MultipleFavoriteCardBacksEnabled())
    {
      if (!GameUtils.IsVanillaHero(card.CardId))
        ++num3;
      if (!CollectionManager.Get().IsFavoriteHero(card.CardId))
        num3 += 1200;
    }
    if (card.OwnedCount == 0)
      num3 += HeroSkinUtils.CanBuyHeroSkinFromCollectionManager(card.CardId) ? 10000 : 20000;
    return num3;
  }

  private void FilterGoldenHeroes()
  {
    for (int index = this.m_unfilteredResults.Count - 1; index > -1; --index)
    {
      CollectibleCard unfilteredResult = this.m_unfilteredResults[index];
      if (unfilteredResult.PremiumType == TAG_PREMIUM.GOLDEN)
      {
        if (unfilteredResult.OwnedCount == 0)
          this.m_unfilteredResults.RemoveAt(index);
      }
      else
      {
        CollectibleCard card = CollectionManager.Get()?.GetCard(unfilteredResult.CardId, TAG_PREMIUM.GOLDEN);
        if (card != null && card.OwnedCount > 0)
          this.m_unfilteredResults.RemoveAt(index);
      }
    }
  }

  private void EnforcePairingPositions()
  {
    foreach (KeyValuePair<int, int> forcedPair in CollectibleCardHeroesFilter.s_forcedPairs)
    {
      int index1 = -1;
      int index2 = -1;
      int index3 = 0;
      for (int count = this.m_unfilteredResults.Count; index3 < count; ++index3)
      {
        if (forcedPair.Key == this.m_unfilteredResults[index3].CardDbId)
          index1 = index3;
        if (forcedPair.Value == this.m_unfilteredResults[index3].CardDbId)
          index2 = index3;
        if (index1 != -1 && index2 != -1)
          break;
      }
      if (index1 != -1 && index2 != -1)
      {
        CollectibleCard unfilteredResult1 = this.m_unfilteredResults[index1];
        CollectibleCard unfilteredResult2 = this.m_unfilteredResults[index2];
        if ((unfilteredResult1.OwnedCount != 0 || unfilteredResult2.OwnedCount != 0 ? (unfilteredResult1.OwnedCount <= 0 ? 0 : (unfilteredResult2.OwnedCount > 0 ? 1 : 0)) : 1) != 0)
        {
          CollectionManager collectionManager = CollectionManager.Get();
          if (collectionManager == null || collectionManager.IsFavoriteHero(unfilteredResult1.CardId) == collectionManager.IsFavoriteHero(unfilteredResult2.CardId))
          {
            this.m_unfilteredResults.RemoveAt(index2);
            this.m_unfilteredResults.Insert(index1 + (index1 > index2 ? 0 : 1), unfilteredResult2);
          }
        }
      }
    }
  }
}
