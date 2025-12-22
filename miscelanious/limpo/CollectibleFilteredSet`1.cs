using Blizzard.T5.Core.Utils;
using Hearthstone.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class CollectibleFilteredSet<T> : FilteredSortedSet<T> where T : ICollectible
{
  protected int m_remainingItemCount;
  protected string m_ownedTag = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_OWNED");
  protected string m_newToken = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_NEW");
  protected string m_hasTag = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_HAS");
  protected string m_evenTag = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EVEN_CARDS").ToLower();
  protected string m_oddTag = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ODD_CARDS").ToLower();
  protected string m_missingToken = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING");
  public static readonly char[] SearchTagColons = new char[2]
  {
    ':',
    '：'
  };
  public static readonly char[] SearchTokenDelimiters = new char[2]
  {
    ' ',
    '\t'
  };
  private List<T> m_itemsRemaining_memoized;

  public int TotalPages { get; protected set; }

  public int ItemsPerPage { get; set; }

  public string SearchString { get; set; }

  protected virtual ICollection<Filter<T>> CreateValuelessFilters(string token)
  {
    List<Filter<T>> valuelessFilters = new List<Filter<T>>();
    if (string.Equals(token, this.m_newToken, StringComparison.OrdinalIgnoreCase))
    {
      Filter<T> filter = new Filter<T>((Func<T, bool>) (c => c.IsNewCollectible));
      valuelessFilters.Add(filter);
      return (ICollection<Filter<T>>) valuelessFilters;
    }
    if (string.Equals(token, this.m_ownedTag, StringComparison.OrdinalIgnoreCase) && !((IEnumerable<char>) CollectibleFilteredSet<T>.SearchTagColons).Any<char>(new Func<char, bool>(((StringUtils) token).Contains)))
    {
      Filter<T> filter = new Filter<T>((Func<T, bool>) (c => c.OwnedCount > 0));
      valuelessFilters.Add(filter);
      return (ICollection<Filter<T>>) valuelessFilters;
    }
    if (!string.Equals(token, this.m_missingToken, StringComparison.OrdinalIgnoreCase))
      return (ICollection<Filter<T>>) valuelessFilters;
    Filter<T> filter1 = new Filter<T>((Func<T, bool>) (c => c.OwnedCount <= 0));
    valuelessFilters.Add(filter1);
    return (ICollection<Filter<T>>) valuelessFilters;
  }

  protected virtual ICollection<Filter<T>> CreateNumericFilters(
    string tag,
    int minVal,
    int maxVal)
  {
    List<Filter<T>> numericFilters = new List<Filter<T>>();
    if (!string.Equals(tag, this.m_ownedTag, StringComparison.OrdinalIgnoreCase))
      return (ICollection<Filter<T>>) numericFilters;
    Filter<T> filter = new Filter<T>((Func<T, bool>) (c => minVal <= c.OwnedCount && c.OwnedCount <= maxVal));
    numericFilters.Add(filter);
    return (ICollection<Filter<T>>) numericFilters;
  }

  protected virtual ICollection<Filter<T>> CreateTagValueFilters(
    string tag,
    string value)
  {
    List<Filter<T>> tagValueFilters = new List<Filter<T>>();
    if (string.Equals(tag, this.m_ownedTag, StringComparison.OrdinalIgnoreCase))
    {
      Filter<T> filter;
      if (this.TryCreateOddEvenParityFilter((Func<T, int>) (c => c.OwnedCount), value, out filter))
        tagValueFilters.Add(filter);
      return (ICollection<Filter<T>>) tagValueFilters;
    }
    if (!string.Equals(tag, this.m_hasTag, StringComparison.OrdinalIgnoreCase))
      return (ICollection<Filter<T>>) tagValueFilters;
    Filter<T> filter1 = new Filter<T>((Func<T, bool>) (c => CollectionUtils.FindTextInCollectible((ICollectible) c, value)));
    tagValueFilters.Add(filter1);
    return (ICollection<Filter<T>>) tagValueFilters;
  }

  protected virtual bool ShouldAppendToRegularSearchTokens(
    string token,
    ICollection<Filter<T>> generatedFilters)
  {
    return !generatedFilters.Any<Filter<T>>();
  }

  protected bool TryCreateOddEvenParityFilter(
    Func<T, int> fieldGetter,
    string value,
    out Filter<T> filter)
  {
    string lower = value.ToLower();
    if (lower == this.m_evenTag)
    {
      filter = new Filter<T>((Func<T, bool>) (c => fieldGetter(c) % 2 == 0));
      return true;
    }
    if (lower == this.m_oddTag)
    {
      filter = new Filter<T>((Func<T, bool>) (c => fieldGetter(c) % 2 == 1));
      return true;
    }
    filter = (Filter<T>) null;
    return false;
  }

  protected Filter<T> CreateMinMaxFilter(Func<T, int> fieldGetter, int minVal, int maxVal) => new Filter<T>((Func<T, bool>) (c => fieldGetter(c) >= minVal && fieldGetter(c) <= maxVal));

  protected ICollection<Filter<T>> CreateFiltersFromToken(string token)
  {
    ICollection<Filter<T>> valuelessFilters = this.CreateValuelessFilters(token);
    if (valuelessFilters.Any<Filter<T>>() || !((IEnumerable<char>) CollectibleFilteredSet<T>.SearchTagColons).Any<char>(new Func<char, bool>(((StringUtils) token).Contains)))
      return valuelessFilters;
    string[] strArray = token.Split(CollectibleFilteredSet<T>.SearchTagColons);
    if (strArray.Length == 2)
    {
      string tag = strArray[0].Trim();
      string val = strArray[1].Trim();
      bool isNumericalValue;
      int minVal;
      int maxVal;
      GeneralUtils.ParseNumericRange(val, out isNumericalValue, out minVal, out maxVal);
      if (isNumericalValue)
      {
        ICollection<Filter<T>> numericFilters = this.CreateNumericFilters(tag, minVal, maxVal);
        if (numericFilters.Any<Filter<T>>())
        {
          valuelessFilters.AddRange<Filter<T>>((IEnumerable<Filter<T>>) numericFilters);
          return valuelessFilters;
        }
      }
      ICollection<Filter<T>> tagValueFilters = this.CreateTagValueFilters(tag, val);
      if (tagValueFilters.Any<Filter<T>>())
      {
        valuelessFilters.AddRange<Filter<T>>((IEnumerable<Filter<T>>) tagValueFilters);
        return valuelessFilters;
      }
    }
    return valuelessFilters;
  }

  public CollectibleFilteredSet()
  {
  }

  public CollectibleFilteredSet(IComparer<T> comparer)
    : base(comparer)
  {
  }

  public void UpdateFilters()
  {
    HashSet<Filter<T>> allFilters1 = this.GetAllFilters();
    ISet<Filter<T>> allFilters2 = this.CreateAllFilters();
    IEnumerable<Filter<T>> source1 = allFilters1.Except<Filter<T>>((IEnumerable<Filter<T>>) allFilters2);
    IEnumerable<Filter<T>> source2 = allFilters2.Except<Filter<T>>((IEnumerable<Filter<T>>) allFilters1);
    bool flag = source1.Any<Filter<T>>() || source2.Any<Filter<T>>();
    foreach (Filter<T> filter in source1)
      this.RemoveFilter(filter);
    foreach (Filter<T> filter in source2)
      this.AddFilter(filter);
    if (!flag && this.m_itemsRemaining_memoized != null)
      return;
    this.UpdateMemoizedFields();
  }

  public virtual List<T> GetPageContents(int currentPageNumber)
  {
    int ofFirstItemOnPage = this.GetIndexOfFirstItemOnPage(currentPageNumber);
    int count = Math.Min(this.ItemsPerPage, this.m_itemsRemaining_memoized.Count - ofFirstItemOnPage);
    return this.m_itemsRemaining_memoized.GetRange(ofFirstItemOnPage, count);
  }

  protected ISet<Filter<T>> CreateFiltersFromSearchString(string searchString)
  {
    HashSet<Filter<T>> fromSearchString = new HashSet<Filter<T>>();
    if (string.IsNullOrWhiteSpace(searchString))
      return (ISet<Filter<T>>) fromSearchString;
    string[] strArray = searchString.ToLower().Split(CollectibleFilteredSet<T>.SearchTokenDelimiters, StringSplitOptions.RemoveEmptyEntries);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (string token in strArray)
    {
      ICollection<Filter<T>> filtersFromToken = this.CreateFiltersFromToken(token);
      fromSearchString.UnionWith((IEnumerable<Filter<T>>) filtersFromToken);
      if (this.ShouldAppendToRegularSearchTokens(token, filtersFromToken))
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(" ");
        stringBuilder.Append(token);
      }
    }
    string regularTokensString = stringBuilder.ToString();
    if (!string.IsNullOrWhiteSpace(regularTokensString))
    {
      Filter<T> filter = new Filter<T>((Func<T, bool>) (collectible => CollectionUtils.FindTextInCollectible((ICollectible) collectible, regularTokensString)));
      fromSearchString.Add(filter);
    }
    return (ISet<Filter<T>>) fromSearchString;
  }

  private ISet<Filter<T>> CreateAllFilters() => this.CreateFiltersFromSearchString(this.SearchString);

  private void UpdateMemoizedFields()
  {
    this.m_itemsRemaining_memoized = this.m_itemsRemaining.ToList<T>();
    this.m_remainingItemCount = this.m_itemsRemaining_memoized.Count<T>();
    this.TotalPages = this.m_remainingItemCount / this.ItemsPerPage + (this.m_remainingItemCount % this.ItemsPerPage > 0 ? 1 : 0);
  }

  private int GetIndexOfFirstItemOnPage(int currentPageNumber) => (currentPageNumber - 1) * this.ItemsPerPage;
}
