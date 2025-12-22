using System.Collections.Generic;

public class TagMap
{
  private Dictionary<int, int> m_values;

  public TagMap() => this.m_values = new Dictionary<int, int>();

  public TagMap(int size) => this.m_values = new Dictionary<int, int>(size);

  public void SetTag(int tag, int tagValue) => this.m_values[tag] = tagValue;

  public void SetTags(Dictionary<int, int> tagMap)
  {
    foreach (KeyValuePair<int, int> tag in tagMap)
      this.SetTag(tag.Key, tag.Value);
  }

  public void SetTags(List<Network.Entity.Tag> tags)
  {
    foreach (Network.Entity.Tag tag in tags)
      this.SetTag(tag.Name, tag.Value);
  }

  public Dictionary<int, int> GetMap() => this.m_values;

  public int GetTag(int tag)
  {
    int tag1 = 0;
    this.m_values.TryGetValue(tag, out tag1);
    return tag1;
  }

  public void Replace(TagMap tags)
  {
    this.Clear();
    this.SetTags(tags.m_values);
  }

  public void Clear() => this.m_values = new Dictionary<int, int>();

  public TagDeltaList CreateDeltas(List<Network.Entity.Tag> comp)
  {
    TagDeltaList deltas = new TagDeltaList();
    foreach (Network.Entity.Tag tag in comp)
    {
      int name = tag.Name;
      int prev = 0;
      this.m_values.TryGetValue(name, out prev);
      int curr = tag.Value;
      if (prev != curr)
        deltas.Add(name, prev, curr);
    }
    return deltas;
  }
}
