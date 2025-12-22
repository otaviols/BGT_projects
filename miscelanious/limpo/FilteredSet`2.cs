using System.Collections.Generic;

public class FilteredSet<T, TSet> where TSet : ISet<T>, new()
{
  protected DictionaryOfHashSets<Filter<T>, T> m_filtersToExcludedItems;
  protected DictionaryOfHashSets<T, Filter<T>> m_itemsToExcludingFilters;
  protected TSet m_itemsFilteredOut;
  protected TSet m_itemsRemaining;

  public FilteredSet()
  {
    this.m_filtersToExcludedItems = new DictionaryOfHashSets<Filter<T>, T>();
    this.m_itemsToExcludingFilters = new DictionaryOfHashSets<T, Filter<T>>();
    this.m_itemsFilteredOut = new TSet();
    this.m_itemsRemaining = new TSet();
  }

  public bool AddFilter(Filter<T> filter)
  {
    if (!this.m_filtersToExcludedItems.AddKey(filter))
      return false;
    foreach (T key in (IEnumerable<T>) this.m_itemsToExcludingFilters.Keys)
    {
      if (!filter.PassesFilter(key))
      {
        this.m_filtersToExcludedItems.Add(filter, key);
        this.m_itemsToExcludingFilters.Add(key, filter);
        this.m_itemsFilteredOut.Add(key);
        this.m_itemsRemaining.Remove(key);
      }
    }
    return true;
  }

  public bool AddItem(T item)
  {
    if (!this.m_itemsToExcludingFilters.AddKey(item))
      return false;
    bool flag = true;
    foreach (Filter<T> key in (IEnumerable<Filter<T>>) this.m_filtersToExcludedItems.Keys)
    {
      if (!key.PassesFilter(item))
      {
        this.m_filtersToExcludedItems.Add(key, item);
        this.m_itemsToExcludingFilters.Add(item, key);
        if (flag)
        {
          this.m_itemsFilteredOut.Add(item);
          flag = false;
        }
      }
    }
    if (this.m_itemsRemaining.Contains(item))
    {
      Log.CollectionManager.PrintError("Duplicate key detected. Check item's comparison function and make sure it cannot have collisions for different items.", (object) item.ToString());
      return false;
    }
    if (flag)
      this.m_itemsRemaining.Add(item);
    return true;
  }

  public int AddItems(IEnumerable<T> items)
  {
    int num = 0;
    foreach (T obj in items)
    {
      if (this.AddItem(obj))
        ++num;
    }
    return num;
  }

  public bool RemoveFilter(Filter<T> filter)
  {
    HashSet<T> values1;
    if (this.m_filtersToExcludedItems.TryGetValues(filter, out values1))
    {
      foreach (T key in values1)
      {
        this.m_itemsToExcludingFilters.Remove(key, filter, false);
        HashSet<Filter<T>> values2;
        this.m_itemsToExcludingFilters.TryGetValues(key, out values2);
        if (values2.Count == 0)
        {
          this.m_itemsFilteredOut.Remove(key);
          this.m_itemsRemaining.Add(key);
        }
      }
    }
    return this.m_filtersToExcludedItems.RemoveKey(filter);
  }

  public HashSet<Filter<T>> GetAllFilters() => new HashSet<Filter<T>>((IEnumerable<Filter<T>>) this.m_filtersToExcludedItems.Keys);
}
