using System.Collections;
using System.Collections.Generic;

public class QueueList<T> : IEnumerable<T>, IEnumerable
{
  protected List<T> m_list = new List<T>();

  public int Enqueue(T item)
  {
    int count = this.m_list.Count;
    this.m_list.Add(item);
    return count;
  }

  public T Dequeue()
  {
    T obj = this.m_list[0];
    this.m_list.RemoveAt(0);
    return obj;
  }

  public T Peek() => this.m_list[0];

  public int Count => this.m_list.Count;

  public T GetItem(int index) => this.m_list[index];

  public T this[int index] => this.m_list[index];

  public void Clear() => this.m_list.Clear();

  public T RemoveAt(int position)
  {
    if (this.m_list.Count <= position)
      return default (T);
    T obj = this.m_list[position];
    this.m_list.RemoveAt(position);
    return obj;
  }

  public bool Remove(T item) => this.m_list.Remove(item);

  public List<T> GetList() => this.m_list;

  public bool Contains(T item) => this.m_list.Contains(item);

  public IEnumerator<T> GetEnumerator() => this.Enumerate().GetEnumerator();

  public override string ToString() => string.Format("Count={0}", (object) this.Count);

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  protected IEnumerable<T> Enumerate()
  {
    for (int i = 0; i < this.m_list.Count; ++i)
      yield return this.m_list[i];
  }
}
