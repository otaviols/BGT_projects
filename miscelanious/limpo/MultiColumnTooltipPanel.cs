using System.Collections.Generic;
using UnityEngine;

public class MultiColumnTooltipPanel : ResizableTooltipPanel
{
  public List<UberText> m_textColumns = new List<UberText>();

  public override void Initialize(string keywordName, string keywordText)
  {
    base.Initialize(keywordName, keywordText);
    float num = 0.0f;
    foreach (UberText textColumn in this.m_textColumns)
    {
      if ((Object) textColumn != (Object) null && (double) textColumn.Height > (double) num)
        num = textColumn.Height;
    }
    this.SetBackgroundSize((this.m_name.Height + this.m_bodyTextHeight + num) * this.m_heightPadding);
  }
}
