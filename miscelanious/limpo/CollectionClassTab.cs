using UnityEngine;

public class CollectionClassTab : BookTab
{
  private CollectionTabInfo m_tabInfo;

  public CollectionTabInfo TabInfo => this.m_tabInfo;

  public void Init(TAG_CLASS classTag)
  {
    this.m_tabInfo = new CollectionTabInfo()
    {
      tagClass = classTag
    };
    this.Init();
  }

  protected override Vector2 GetTextureOffset()
  {
    if (CollectionPageManager.s_classTextureOffsets.ContainsKey(this.m_tabInfo.tagClass))
      return CollectionPageManager.s_classTextureOffsets[this.m_tabInfo.tagClass];
    Debug.LogWarning((object) string.Format("CollectionClassTab.GetTextureOffset(): No class texture offsets exist for class {0}", (object) this.TabInfo.tagClass));
    return Vector2.zero;
  }
}
