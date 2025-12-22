using System.Collections.Generic;

public class TagDeltaList
{
  private List<TagDelta> m_deltas = new List<TagDelta>();

  public void Add(int tag, int prev, int curr) => this.m_deltas.Add(new TagDelta()
  {
    tag = tag,
    oldValue = prev,
    newValue = curr
  });

  public int Count => this.m_deltas.Count;

  public TagDelta this[int index] => this.m_deltas[index];
}
