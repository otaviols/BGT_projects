using System;
using System.Collections.Generic;

public class TagCombinatorHelper
{
  private List<string> m_tempQualityTags = new List<string>();
  private List<string> m_tempContentTags = new List<string>();

  public bool ForEachCombination(
    string[] inputTags,
    List<string> qualityTags,
    List<string> contentTags,
    Func<string, string, bool> action)
  {
    this.m_tempContentTags.Clear();
    this.m_tempQualityTags.Clear();
    UpdateUtils.ResizeListIfNeeded(this.m_tempQualityTags, qualityTags.Count);
    UpdateUtils.ResizeListIfNeeded(this.m_tempContentTags, contentTags.Count);
    foreach (string inputTag in inputTags)
    {
      if (qualityTags.Contains(inputTag))
        this.m_tempQualityTags.Add(inputTag);
      if (contentTags.Contains(inputTag))
        this.m_tempContentTags.Add(inputTag);
    }
    bool flag1 = false;
    bool flag2 = true;
    foreach (string tempQualityTag in this.m_tempQualityTags)
    {
      foreach (string tempContentTag in this.m_tempContentTags)
      {
        flag1 = true;
        if (!action(tempQualityTag, tempContentTag))
        {
          flag2 = false;
          break;
        }
      }
    }
    return flag1 & flag2;
  }
}
