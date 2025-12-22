using System;
using System.Collections.Generic;

public class FilteredSortedSet<T> : FilteredSet<T, SortedSet<T>> where T : IComparable
{
  public FilteredSortedSet()
  {
  }

  public FilteredSortedSet(IComparer<T> comparer)
  {
    this.m_filtersToExcludedItems = new DictionaryOfHashSets<Filter<T>, T>();
    this.m_itemsToExcludingFilters = new DictionaryOfHashSets<T, Filter<T>>();
    this.m_itemsFilteredOut = new SortedSet<T>(comparer);
    this.m_itemsRemaining = new SortedSet<T>(comparer);
  }
}
