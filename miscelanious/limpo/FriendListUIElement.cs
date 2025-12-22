using Blizzard.T5.Core.Utils;
using UnityEngine;

public class FriendListUIElement : PegUIElement
{
  public FriendListUIElement m_ParentElement;
  public GameObject m_Highlight;
  private bool m_selected;

  protected override void Awake()
  {
    base.Awake();
    this.UpdateHighlight();
  }

  public bool IsSelected() => this.m_selected;

  public void SetSelected(bool enable)
  {
    if (enable == this.m_selected)
      return;
    this.m_selected = enable;
    this.UpdateHighlight();
  }

  protected virtual bool ShouldBeHighlighted() => this.m_selected || this.GetInteractionState() == PegUIElement.InteractionState.Over;

  protected void UpdateHighlight()
  {
    bool shouldHighlight = this.ShouldBeHighlighted();
    if (!shouldHighlight)
      shouldHighlight = this.ShouldChildBeHighlighted();
    this.UpdateSelfHighlight(shouldHighlight);
    if (!((Object) this.m_ParentElement != (Object) null))
      return;
    this.m_ParentElement.UpdateHighlight();
  }

  protected bool ShouldChildBeHighlighted()
  {
    foreach (FriendListUIElement friendListUiElement in GameObjectUtils.GetComponentsInChildrenOnly<FriendListUIElement>((Component) this, true))
    {
      if (friendListUiElement.ShouldBeHighlighted())
        return true;
    }
    return false;
  }

  protected void UpdateSelfHighlight(bool shouldHighlight)
  {
    if ((Object) this.m_Highlight == (Object) null || this.m_Highlight.activeSelf == shouldHighlight)
      return;
    this.m_Highlight.SetActive(shouldHighlight);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState) => this.UpdateHighlight();

  protected override void OnOut(PegUIElement.InteractionState oldState) => this.UpdateHighlight();
}
