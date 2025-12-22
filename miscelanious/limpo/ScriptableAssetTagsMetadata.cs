using Hearthstone.Core.Streaming;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableAssetTagsMetadata : ScriptableObject
{
  [SerializeField]
  private List<string> m_tags = new List<string>();
  [SerializeField]
  private List<string> m_tagGroups = new List<string>();
  [SerializeField]
  private List<int> m_tagIdToTagGroupId = new List<int>();
  [SerializeField]
  private List<int> m_overrideId = new List<int>();
  private int m_TagGroupId;
  private List<string> m_QualityTagsInGroup;
  private List<string> m_ContentTagsInGroup;

  public void Clear()
  {
    this.m_tags.Clear();
    this.m_tagGroups.Clear();
    this.m_tagIdToTagGroupId.Clear();
    this.m_overrideId.Clear();
  }

  public void AddTag(string tag, string tagGroup, string overrideTag)
  {
    if (this.m_tags.Contains(tag))
      return;
    int num1 = this.m_tagGroups.IndexOf(tagGroup);
    if (num1 == -1)
    {
      this.m_tagGroups.Add(tagGroup);
      num1 = this.m_tagGroups.Count - 1;
    }
    this.m_tags.Add(tag);
    this.m_tagIdToTagGroupId.Add(num1);
    int num2 = this.m_tags.IndexOf(overrideTag);
    if (num2 == -1)
      throw new Exception(string.Format("The override tag '{0}' must added before tag '{1}'.", (object) overrideTag, (object) tag));
    this.m_overrideId.Add(num2);
  }

  public string[] GetTagGroups() => this.m_tagGroups.ToArray();

  public void GetTagsInTagGroup(string tagGroup, ref List<string> tags) => this.GetTagsInTagGroup(this.m_tagGroups.IndexOf(tagGroup), ref tags);

  public void GetTagsInTagGroup(int tagGroupId, ref List<string> tags)
  {
    tags.Clear();
    if (tagGroupId == -1)
      return;
    int index = 0;
    for (int count = this.m_tagIdToTagGroupId.Count; index < count; ++index)
    {
      if (tagGroupId == this.m_tagIdToTagGroupId[index])
        tags.Add(this.m_tags[index]);
    }
  }

  public string ConvertToOverrideTag(string tag, string tagGroup) => this.ConvertToOverrideTag(tag, this.m_tagGroups.IndexOf(tagGroup));

  public string ConvertToOverrideTag(string tag, int tagGroupId)
  {
    if (tagGroupId == -1)
      return tag;
    int index = this.m_tags.IndexOf(tag);
    return index == -1 ? tag : this.m_tags[this.m_overrideId[index]];
  }

  public string GetTagGroupForTag(string tag)
  {
    int index = this.m_tags.IndexOf(tag);
    return index >= 0 ? this.m_tagGroups[this.m_tagIdToTagGroupId[index]] : string.Empty;
  }

  public void GetTagsFromAssetBundle(string assetBundleName, List<string> tagList)
  {
    tagList.Clear();
    if (this.m_QualityTagsInGroup == null)
    {
      this.m_TagGroupId = this.m_tagGroups.IndexOf(DownloadTags.GetTagGroupString(DownloadTags.TagGroup.Quality));
      this.m_QualityTagsInGroup = new List<string>();
      this.GetTagsInTagGroup(this.m_TagGroupId, ref this.m_QualityTagsInGroup);
    }
    int index1 = 0;
    for (int count = this.m_QualityTagsInGroup.Count; index1 < count; ++index1)
    {
      string tag = this.m_QualityTagsInGroup[index1];
      if (assetBundleName.IndexOf(tag, StringComparison.Ordinal) >= 0)
        tagList.Add(this.ConvertToOverrideTag(tag, this.m_TagGroupId));
    }
    if (this.m_ContentTagsInGroup == null)
    {
      this.m_ContentTagsInGroup = new List<string>();
      this.GetTagsInTagGroup(DownloadTags.GetTagGroupString(DownloadTags.TagGroup.Content), ref this.m_ContentTagsInGroup);
    }
    int index2 = 0;
    for (int count = this.m_ContentTagsInGroup.Count; index2 < count; ++index2)
    {
      string tag = this.m_ContentTagsInGroup[index2];
      if (assetBundleName.IndexOf(tag, StringComparison.Ordinal) >= 0)
        tagList.Add(this.ConvertToOverrideTag(tag, this.m_TagGroupId));
    }
  }

  public List<string> GetAllTags(string tagGroup, bool excludeOverridenTag)
  {
    List<string> tags = new List<string>();
    this.GetTagsInTagGroup(tagGroup, ref tags);
    List<string> allTags = new List<string>();
    foreach (string str in tags)
    {
      if (!excludeOverridenTag || this.m_overrideId[this.m_tags.IndexOf(str)] == this.m_tags.IndexOf(str))
        allTags.Add(str);
    }
    return allTags;
  }
}
