using System.Collections.Generic;

public class DictionaryOfSets<TKey, TValue, TSet, TDictionary>
  where TSet : ISet<TValue>, new()
  where TDictionary : IDictionary<TKey, TSet>, new()
{
  protected TDictionary m_inner;

  public DictionaryOfSets() => this.m_inner = new TDictionary();

  public ICollection<TKey> Keys => this.m_inner.Keys;

  public bool AddKey(TKey key)
  {
    if (this.m_inner.ContainsKey(key))
      return false;
    this.m_inner.Add(key, new TSet());
    return true;
  }

  public bool Add(TKey key, TValue value)
  {
    TSet set;
    if (!this.m_inner.TryGetValue(key, out set))
    {
      set = new TSet();
      this.m_inner.Add(key, set);
    }
    return set.Add(value);
  }

  public bool Remove(TKey key, TValue value, bool removeKeyIfSetBecomesEmpty)
  {
    TSet set;
    if (!this.m_inner.TryGetValue(key, out set))
      return false;
    if ((object) set == null)
    {
      this.RemoveKey(key);
      return false;
    }
    int num = set.Remove(value) ? 1 : 0;
    if (!removeKeyIfSetBecomesEmpty)
      return num != 0;
    if (set.Count != 0)
      return num != 0;
    this.RemoveKey(key);
    return num != 0;
  }

  public bool RemoveKey(TKey key) => this.m_inner.Remove(key);

  public bool TryGetValues(TKey key, out TSet values) => this.m_inner.TryGetValue(key, out values);
}
