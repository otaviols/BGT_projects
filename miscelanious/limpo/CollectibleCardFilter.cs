using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class CollectibleCardFilter : CollectibleFilteredSet<CollectibleCard>
{
  protected string m_extraToken = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EXTRA");
  protected string m_favoriteToken = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_FAVORITE");
  private TAG_CARD_SET[] m_filterCardSets;
  protected TAG_CLASS[] m_filterClasses;
  private TAG_CARDTYPE[] m_filterCardTypes;
  protected TAG_ROLE[] m_filterRoles;
  private int? m_filterManaCost;
  private int? m_filterOwnedMinimum = new int?(1);
  private List<CollectibleCardFilter.FilterMask> m_filterMasks;
  private bool? m_craftableFilterValue;
  protected string m_filterText;
  private bool m_filterIsHero;
  private DeckRuleset m_deckRuleset;
  private HashSet<string> m_leagueBannedCardsSubset;
  private List<int> m_specificCards;
  private bool? m_filterCounterpartCards = new bool?(true);

  public static CollectibleCardFilter.FilterMask FilterMaskFromPremiumType(
    TAG_PREMIUM premiumType)
  {
    CollectibleCardFilter.FilterMask filterMask1 = CollectibleCardFilter.FilterMask.NONE;
    CollectibleCardFilter.FilterMask filterMask2;
    switch (premiumType)
    {
      case TAG_PREMIUM.GOLDEN:
        filterMask2 = filterMask1 | CollectibleCardFilter.FilterMask.PREMIUM_GOLDEN;
        break;
      case TAG_PREMIUM.DIAMOND:
        filterMask2 = filterMask1 | CollectibleCardFilter.FilterMask.PREMIUM_DIAMOND;
        break;
      case TAG_PREMIUM.SIGNATURE:
        filterMask2 = filterMask1 | CollectibleCardFilter.FilterMask.PREMIUM_SIGNATURE;
        break;
      default:
        filterMask2 = filterMask1 | CollectibleCardFilter.FilterMask.PREMIUM_NORMAL;
        break;
    }
    return filterMask2;
  }

  public CollectionManager.FindCardsResult FindCardsResult { get; protected set; }

  public abstract void UpdateResults();

  public abstract int GetTotalNumPages();

  public abstract List<CollectibleCard> GetFirstNonEmptyPage(
    out int collectionPage);

  public void SetDeckRuleset(DeckRuleset deckRuleset) => this.m_deckRuleset = deckRuleset;

  public void FilterTheseCardSets(params TAG_CARD_SET[] cardSets)
  {
    this.m_filterCardSets = (TAG_CARD_SET[]) null;
    if (cardSets == null || cardSets.Length == 0)
      return;
    this.m_filterCardSets = cardSets;
  }

  public bool CardSetFilterIncludesWild()
  {
    if (this.m_filterCardSets == null && this.m_specificCards == null)
      return true;
    if (this.m_filterCardSets != null)
    {
      foreach (TAG_CARD_SET filterCardSet in this.m_filterCardSets)
      {
        if (GameUtils.IsWildCardSet(filterCardSet))
          return true;
      }
    }
    if (this.m_specificCards != null)
    {
      foreach (int specificCard in this.m_specificCards)
      {
        if (GameUtils.IsWildCard(specificCard))
          return true;
      }
    }
    return false;
  }

  public bool CardSetFilterIsAllStandardSets() => this.m_filterCardSets != null && new HashSet<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) this.m_filterCardSets).SetEquals((IEnumerable<TAG_CARD_SET>) new List<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) GameUtils.GetStandardSets()));

  public bool CardSetFilterIsClassicSet()
  {
    if (this.m_filterCardSets == null)
      return false;
    return new HashSet<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) this.m_filterCardSets).SetEquals((IEnumerable<TAG_CARD_SET>) new List<TAG_CARD_SET>()
    {
      TAG_CARD_SET.VANILLA
    });
  }

  public void FilterTheseClasses(params TAG_CLASS[] classTypes)
  {
    this.m_filterClasses = (TAG_CLASS[]) null;
    if (classTypes == null || classTypes.Length == 0)
      return;
    this.m_filterClasses = classTypes;
  }

  public void FilterManaCost(int? manaCost) => this.m_filterManaCost = manaCost;

  public bool IsManaCostFilterActive => this.m_filterManaCost.HasValue;

  public virtual void FilterOnlyOwned(bool owned)
  {
    this.m_filterOwnedMinimum = new int?();
    if (!owned)
      return;
    this.m_filterOwnedMinimum = new int?(1);
  }

  public void FilterByMask(List<CollectibleCardFilter.FilterMask> filterMasks)
  {
    if (filterMasks == null)
      filterMasks = new List<CollectibleCardFilter.FilterMask>()
      {
        CollectibleCardFilter.FilterMask.ALL
      };
    this.m_filterMasks = filterMasks;
  }

  public void FilterByCraftability(bool? isCraftable) => this.m_craftableFilterValue = isCraftable;

  public void FilterLeagueBannedCardsSubset(HashSet<string> leagueBannedCardsSubset) => this.m_leagueBannedCardsSubset = leagueBannedCardsSubset;

  public void FilterSearchText(string searchText) => this.m_filterText = searchText;

  public void FilterHero(bool isHero) => this.m_filterIsHero = isHero;

  public void FilterSpecificCards(List<int> specificCards) => this.m_specificCards = specificCards.Where<int>((Func<int, bool>) (x => GameUtils.IsCardCollectible(GameUtils.TranslateDbIdToCardId(x)))).ToList<int>();

  public CollectionManager.FindCardsResult GenerateResults()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    string filterText = this.m_filterText;
    int? filterManaCost = this.m_filterManaCost;
    List<CollectibleCardFilter.FilterMask> filterMasks = this.m_filterMasks;
    int? manaCost = filterManaCost;
    TAG_CARD_SET[] filterCardSets = this.m_filterCardSets;
    TAG_CLASS[] filterClasses = this.m_filterClasses;
    TAG_CARDTYPE[] filterCardTypes = this.m_filterCardTypes;
    TAG_ROLE[] filterRoles = this.m_filterRoles;
    TAG_RARITY? rarity = new TAG_RARITY?();
    TAG_RACE? race = new TAG_RACE?();
    bool? isHero = new bool?(this.m_filterIsHero);
    int? filterOwnedMinimum = this.m_filterOwnedMinimum;
    bool? notSeen = new bool?();
    bool? craftableFilterValue = this.m_craftableFilterValue;
    DeckRuleset deckRuleset = this.m_deckRuleset;
    HashSet<string> bannedCardsSubset = this.m_leagueBannedCardsSubset;
    List<int> specificCards = this.m_specificCards;
    bool? counterpartCards = this.m_filterCounterpartCards;
    return collectionManager.FindOrderedCards(filterText, filterMasks, manaCost, filterCardSets, filterClasses, filterCardTypes, filterRoles, rarity, race, isHero, filterOwnedMinimum, notSeen, craftableFilterValue, deckRuleset: deckRuleset, leagueBannedCardsSubset: bannedCardsSubset, specificCards: specificCards, filterCounterpartCards: counterpartCards);
  }

  public CollectionManager.FindCardsResult GenerateUnOrderedResults()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    string filterText = this.m_filterText;
    int? filterManaCost = this.m_filterManaCost;
    List<CollectibleCardFilter.FilterMask> filterMasks = this.m_filterMasks;
    int? manaCost = filterManaCost;
    TAG_CARD_SET[] filterCardSets = this.m_filterCardSets;
    TAG_CLASS[] filterClasses = this.m_filterClasses;
    TAG_CARDTYPE[] filterCardTypes = this.m_filterCardTypes;
    TAG_ROLE[] filterRoles = this.m_filterRoles;
    TAG_RARITY? rarity = new TAG_RARITY?();
    TAG_RACE? race = new TAG_RACE?();
    bool? isHero = new bool?(this.m_filterIsHero);
    int? filterOwnedMinimum = this.m_filterOwnedMinimum;
    bool? notSeen = new bool?();
    bool? craftableFilterValue = this.m_craftableFilterValue;
    DeckRuleset deckRuleset = this.m_deckRuleset;
    HashSet<string> bannedCardsSubset = this.m_leagueBannedCardsSubset;
    List<int> specificCards = this.m_specificCards;
    bool? counterpartCards = this.m_filterCounterpartCards;
    return collectionManager.FindCards(filterText, filterMasks, manaCost, filterCardSets, filterClasses, filterCardTypes, filterRoles, rarity, race, isHero, filterOwnedMinimum, notSeen, craftableFilterValue, deckRuleset: deckRuleset, leagueBannedCardsSubset: bannedCardsSubset, specificCards: specificCards, filterCoreCounterpartCards: counterpartCards);
  }

  private static void AddSearchableTokensToSet(string str, HashSet<string> addToList, bool split = true)
  {
    string[] strArray1;
    if (!split)
      strArray1 = new string[1]{ str };
    else
      strArray1 = str.Split(CollectibleFilteredSet<ICollectible>.SearchTokenDelimiters, StringSplitOptions.RemoveEmptyEntries);
    string[] strArray2 = strArray1;
    foreach (string token in strArray2)
      CollectibleCardFilter.AddSingleSearchableTokenToSet(token, addToList);
    if (strArray2.Length <= 1)
      return;
    CollectibleCardFilter.AddSingleSearchableTokenToSet(str, addToList);
  }

  public static void AddSearchableTokensToSet<T>(
    T structType,
    Func<T, bool> hasTypeString,
    Func<T, string> getTypeString,
    HashSet<string> addToList)
    where T : struct
  {
    if (!hasTypeString(structType))
      return;
    CollectibleCardFilter.AddSearchableTokensToSet(getTypeString(structType), addToList);
  }

  public static void AddSingleSearchableTokenToSet(string token, HashSet<string> addToList)
  {
    string lower = token.ToLower();
    string str1 = SearchableString.ConvertEuropeanCharacters(lower);
    string str2 = SearchableString.RemoveDiacritics(lower);
    addToList.Add(lower);
    if (!lower.Equals(str1))
      addToList.Add(str1);
    if (lower.Equals(str2))
      return;
    addToList.Add(str2);
  }

  protected override ICollection<Filter<CollectibleCard>> CreateValuelessFilters(
    string token)
  {
    ICollection<Filter<CollectibleCard>> valuelessFilters1 = (ICollection<Filter<CollectibleCard>>) new List<Filter<CollectibleCard>>();
    if (token == this.m_extraToken || token == this.m_favoriteToken && CardBackManager.Get().MultipleFavoriteCardBacksEnabled())
      return valuelessFilters1;
    if (token == this.m_missingToken)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (c => c.OwnedCount <= 0 || c.IsCraftable));
      valuelessFilters1.Add(filter);
      return valuelessFilters1;
    }
    ICollection<Filter<CollectibleCard>> valuelessFilters2 = base.CreateValuelessFilters(token);
    if (valuelessFilters2.Any<Filter<CollectibleCard>>())
      return valuelessFilters2;
    string str1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_GOLDEN");
    string str2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_DIAMOND");
    string str3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SIGNATURE");
    string str4 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_REFUND");
    string whelp = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_WHELP");
    string imp = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_IMP");
    string runeBlood = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_BLOOD");
    string runeFrost = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_FROST");
    string runeUnholy = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_UNHOLY");
    if (token == str1)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.PremiumType == TAG_PREMIUM.GOLDEN));
      valuelessFilters2.Add(filter);
      return valuelessFilters2;
    }
    if (token == str2)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.PremiumType == TAG_PREMIUM.DIAMOND));
      valuelessFilters2.Add(filter);
      return valuelessFilters2;
    }
    if (token == str3)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.PremiumType == TAG_PREMIUM.SIGNATURE));
      valuelessFilters2.Add(filter);
      return valuelessFilters2;
    }
    if (token == str4)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.IsRefundable));
      valuelessFilters2.Add(filter);
      return valuelessFilters2;
    }
    if (token == whelp)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.HasCardTag(GAME_TAG.WHELP) || card.FindTextInCard(whelp)));
      valuelessFilters2.Add(filter);
      return valuelessFilters2;
    }
    if (token == imp)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.HasCardTag(GAME_TAG.IMP) || card.FindTextInCard(imp)));
      valuelessFilters2.Add(filter);
      return valuelessFilters2;
    }
    if (token == runeBlood)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.Runes.Blood > 0 || card.FindTextInCard(runeBlood)));
      valuelessFilters2.Add(filter);
      return valuelessFilters2;
    }
    if (token == runeFrost)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.Runes.Frost > 0 || card.FindTextInCard(runeFrost)));
      valuelessFilters2.Add(filter);
      return valuelessFilters2;
    }
    if (!(token == runeUnholy))
      return valuelessFilters2;
    Filter<CollectibleCard> filter1 = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.Runes.Unholy > 0 || card.FindTextInCard(runeUnholy)));
    valuelessFilters2.Add(filter1);
    return valuelessFilters2;
  }

  protected override ICollection<Filter<CollectibleCard>> CreateNumericFilters(
    string tag,
    int minVal,
    int maxVal)
  {
    ICollection<Filter<CollectibleCard>> numericFilters = base.CreateNumericFilters(tag, minVal, maxVal);
    if (numericFilters.Any<Filter<CollectibleCard>>())
      return numericFilters;
    string str1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_HEALTH");
    string str2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ATTACK");
    string lower = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MANA").ToLower();
    string str3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_BLOOD");
    string str4 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_FROST");
    string str5 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_UNHOLY");
    if (tag == str2)
    {
      Filter<CollectibleCard> minMaxFilter = this.CreateMinMaxFilter((Func<CollectibleCard, int>) (card => card.Attack), minVal, maxVal);
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.CardType == TAG_CARDTYPE.MINION || card.CardType == TAG_CARDTYPE.WEAPON));
      numericFilters.Add(minMaxFilter);
      numericFilters.Add(filter);
      return numericFilters;
    }
    if (tag == str1)
    {
      Filter<CollectibleCard> minMaxFilter = this.CreateMinMaxFilter((Func<CollectibleCard, int>) (card => card.Health), minVal, maxVal);
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.CardType == TAG_CARDTYPE.MINION));
      numericFilters.Add(minMaxFilter);
      numericFilters.Add(filter);
      return numericFilters;
    }
    if (tag == lower)
    {
      Filter<CollectibleCard> minMaxFilter = this.CreateMinMaxFilter((Func<CollectibleCard, int>) (card => card.ManaCost), minVal, maxVal);
      numericFilters.Add(minMaxFilter);
      return numericFilters;
    }
    if (tag == str3)
    {
      Filter<CollectibleCard> minMaxFilter = this.CreateMinMaxFilter((Func<CollectibleCard, int>) (card => card.Runes.Blood), minVal, maxVal);
      numericFilters.Add(minMaxFilter);
      return numericFilters;
    }
    if (tag == str4)
    {
      Filter<CollectibleCard> minMaxFilter = this.CreateMinMaxFilter((Func<CollectibleCard, int>) (card => card.Runes.Frost), minVal, maxVal);
      numericFilters.Add(minMaxFilter);
      return numericFilters;
    }
    if (!(tag == str5))
      return numericFilters;
    Filter<CollectibleCard> minMaxFilter1 = this.CreateMinMaxFilter((Func<CollectibleCard, int>) (card => card.Runes.Unholy), minVal, maxVal);
    numericFilters.Add(minMaxFilter1);
    return numericFilters;
  }

  protected override ICollection<Filter<CollectibleCard>> CreateTagValueFilters(
    string tagKey,
    string value)
  {
    ICollection<Filter<CollectibleCard>> tagValueFilters = base.CreateTagValueFilters(tagKey, value);
    if (tagValueFilters.Any<Filter<CollectibleCard>>())
      return tagValueFilters;
    string str1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ARTIST");
    string str2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_HEALTH");
    string str3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ATTACK");
    string lower = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MANA").ToLower();
    string str4 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RARITY");
    string str5 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_TYPE");
    string str6 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNES");
    string str7 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_TAG");
    string str8 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL");
    if (tagKey == str1)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => SearchableString.SearchInternationalText(value, card.ArtistName)));
      tagValueFilters.Add(filter);
      return tagValueFilters;
    }
    if (tagKey == str4)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => SearchableString.SearchInternationalText(value, GameStrings.GetRarityText(card.Rarity))));
      tagValueFilters.Add(filter);
      return tagValueFilters;
    }
    if (tagKey == str5)
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card =>
      {
        string cardTypeName = GameStrings.GetCardTypeName(card.CardType);
        return cardTypeName != null && SearchableString.SearchInternationalText(value, cardTypeName);
      }));
      tagValueFilters.Add(filter);
      return tagValueFilters;
    }
    if (tagKey == str3)
    {
      Filter<CollectibleCard> filter1;
      if (this.TryCreateOddEvenParityFilter((Func<CollectibleCard, int>) (card => card.Attack), value, out filter1))
      {
        Filter<CollectibleCard> filter2 = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.CardType == TAG_CARDTYPE.MINION || card.CardType == TAG_CARDTYPE.WEAPON));
        tagValueFilters.Add(filter1);
        tagValueFilters.Add(filter2);
      }
      return tagValueFilters;
    }
    if (tagKey == str2)
    {
      Filter<CollectibleCard> filter3;
      if (this.TryCreateOddEvenParityFilter((Func<CollectibleCard, int>) (card => card.Health), value, out filter3))
      {
        Filter<CollectibleCard> filter4 = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.CardType == TAG_CARDTYPE.MINION));
        tagValueFilters.Add(filter3);
        tagValueFilters.Add(filter4);
      }
      return tagValueFilters;
    }
    if (tagKey == lower)
    {
      Filter<CollectibleCard> filter;
      if (this.TryCreateOddEvenParityFilter((Func<CollectibleCard, int>) (card => card.ManaCost), value, out filter))
        tagValueFilters.Add(filter);
      return tagValueFilters;
    }
    if (tagKey == str6)
    {
      RunePattern runePattern = new RunePattern();
      char ch1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_BLOOD_CHAR")[0];
      char ch2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_FROST_CHAR")[0];
      char ch3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_RUNE_UNHOLY_CHAR")[0];
      foreach (char ch4 in value.ToCharArray())
      {
        if ((int) ch4 == (int) ch1)
          runePattern.AddRunes(RuneType.RT_BLOOD, 1);
        else if ((int) ch4 == (int) ch2)
          runePattern.AddRunes(RuneType.RT_FROST, 1);
        else if ((int) ch4 == (int) ch3)
          runePattern.AddRunes(RuneType.RT_UNHOLY, 1);
      }
      if (runePattern.HasRunes)
      {
        Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.Runes.Matches(runePattern)));
        tagValueFilters.Add(filter);
      }
      return tagValueFilters;
    }
    if (tagKey == str7)
    {
      string str9 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_WHELP");
      string str10 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_IMP");
      if (value == str9)
      {
        Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.HasCardTag(GAME_TAG.WHELP)));
        tagValueFilters.Add(filter);
      }
      else if (value == str10)
      {
        Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.HasCardTag(GAME_TAG.IMP)));
        tagValueFilters.Add(filter);
      }
      return tagValueFilters;
    }
    if (!(tagKey == str8))
      return tagValueFilters;
    Dictionary<string, TAG_SPELL_SCHOOL> schoolDictionary = new Dictionary<string, TAG_SPELL_SCHOOL>()
    {
      {
        GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL_NONE"),
        TAG_SPELL_SCHOOL.NONE
      },
      {
        GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL_ARCANE"),
        TAG_SPELL_SCHOOL.ARCANE
      },
      {
        GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL_FIRE"),
        TAG_SPELL_SCHOOL.FIRE
      },
      {
        GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL_FROST"),
        TAG_SPELL_SCHOOL.FROST
      },
      {
        GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL_NATURE"),
        TAG_SPELL_SCHOOL.NATURE
      },
      {
        GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL_HOLY"),
        TAG_SPELL_SCHOOL.HOLY
      },
      {
        GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL_SHADOW"),
        TAG_SPELL_SCHOOL.SHADOW
      },
      {
        GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_SCHOOL_FEL"),
        TAG_SPELL_SCHOOL.FEL
      }
    };
    if (schoolDictionary.ContainsKey(value))
    {
      Filter<CollectibleCard> filter = new Filter<CollectibleCard>((Func<CollectibleCard, bool>) (card => card.IsSpell && card.SpellSchool == schoolDictionary[value]));
      tagValueFilters.Add(filter);
    }
    return tagValueFilters;
  }

  protected override bool ShouldAppendToRegularSearchTokens(
    string token,
    ICollection<Filter<CollectibleCard>> generatedFilters)
  {
    return !(token == this.m_extraToken) && base.ShouldAppendToRegularSearchTokens(token, generatedFilters);
  }

  public List<CollectionManager.CollectibleCardFilterFunc> FiltersFromSearchString(
    string searchString)
  {
    ISet<Filter<CollectibleCard>> fromSearchString = this.CreateFiltersFromSearchString(searchString);
    List<CollectionManager.CollectibleCardFilterFunc> collectibleCardFilterFuncList = new List<CollectionManager.CollectibleCardFilterFunc>();
    foreach (Filter<CollectibleCard> filter1 in (IEnumerable<Filter<CollectibleCard>>) fromSearchString)
    {
      Filter<CollectibleCard> filter = filter1;
      CollectionManager.CollectibleCardFilterFunc collectibleCardFilterFunc = (CollectionManager.CollectibleCardFilterFunc) (card => filter.PassesFilter(card));
      collectibleCardFilterFuncList.Add(collectibleCardFilterFunc);
    }
    return collectibleCardFilterFuncList;
  }

  public static string CreateSearchTerm_Mana_OddEven(bool isOdd) => string.Format("{0}{1}{2}", (object) GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MANA"), (object) ((IEnumerable<char>) CollectibleFilteredSet<ICollectible>.SearchTagColons).First<char>(), (object) GameStrings.Get(isOdd ? "GLUE_COLLECTION_MANAGER_SEARCH_ODD_MANA" : "GLUE_COLLECTION_MANAGER_SEARCH_EVEN_MANA"));

  public void ClearOutFiltersFromSetFilterDropdown()
  {
    this.m_specificCards = (List<int>) null;
    this.m_filterCardSets = (TAG_CARD_SET[]) null;
  }

  [Flags]
  public enum FilterMask
  {
    NONE = 0,
    PREMIUM_NORMAL = 2,
    PREMIUM_GOLDEN = 4,
    PREMIUM_DIAMOND = 8,
    PREMIUM_SIGNATURE = 16, // 0x00000010
    PREMIUM_ALL = PREMIUM_SIGNATURE | PREMIUM_DIAMOND | PREMIUM_GOLDEN | PREMIUM_NORMAL, // 0x0000001E
    OWNED = 32, // 0x00000020
    UNOWNED = 64, // 0x00000040
    ALL = -1, // 0xFFFFFFFF
  }
}
