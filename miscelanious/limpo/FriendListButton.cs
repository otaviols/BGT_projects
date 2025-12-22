using UnityEngine;

public class FriendListButton : FriendListUIElement
{
  public GameObject m_Background;
  public UberText m_Text;
  public GameObject m_ActiveGlow;

  public string GetText() => this.m_Text.Text;

  public void SetText(string text)
  {
    this.m_Text.Text = text;
    this.UpdateAll();
  }

  public void ShowActiveGlow(bool show)
  {
    if (!((Object) this.m_ActiveGlow != (Object) null))
      return;
    HighlightState componentInChildren = this.m_ActiveGlow.GetComponentInChildren<HighlightState>();
    if (!((Object) componentInChildren != (Object) null))
      return;
    if (show)
      componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    else
      componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_OFF);
  }

  private void UpdateAll() => this.UpdateHighlight();
}
