using UnityEngine;

[CustomEditClass]
public class UIBHighlightStateControl : MonoBehaviour
{
  [CustomEditField(Sections = "Highlight State Reference")]
  public HighlightState m_HighlightState;
  [CustomEditField(Sections = "Highlight State Type")]
  public ActorStateType m_MouseOverStateType = ActorStateType.HIGHLIGHT_MOUSE_OVER;
  [CustomEditField(Sections = "Highlight State Type")]
  public ActorStateType m_PrimarySelectedStateType = ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE;
  [CustomEditField(Sections = "Highlight State Type")]
  public ActorStateType m_SecondarySelectedStateType = ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE;
  [CustomEditField(Sections = "Behavior Settings")]
  public bool m_UseMouseOver;
  [CustomEditField(Sections = "Behavior Settings")]
  public bool m_AllowSelection;
  [CustomEditField(Sections = "Behavior Settings")]
  public bool m_EnableResponse = true;
  private PegUIElement m_PegUIElement;
  private bool m_MouseOver;

  private void Awake()
  {
    PegUIElement component = this.gameObject.GetComponent<PegUIElement>();
    if (!((Object) component != (Object) null))
      return;
    component.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e =>
    {
      if (!this.m_EnableResponse)
        return;
      this.OnRollOver();
    }));
    component.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e =>
    {
      if (!this.m_EnableResponse)
        return;
      this.OnRollOut();
    }));
    component.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      if (!this.m_EnableResponse)
        return;
      this.OnRelease();
    }));
  }

  public void Select(bool selected, bool primary = false)
  {
    if (selected)
      this.m_HighlightState.ChangeState(primary ? this.m_PrimarySelectedStateType : this.m_SecondarySelectedStateType);
    else if (this.m_MouseOver)
      this.m_HighlightState.ChangeState(this.m_MouseOverStateType);
    else
      this.m_HighlightState.ChangeState(ActorStateType.NONE);
  }

  public bool IsReady() => this.m_HighlightState.IsReady();

  private void OnRollOver()
  {
    if (!this.m_UseMouseOver)
      return;
    this.m_MouseOver = true;
    this.m_HighlightState.ChangeState(this.m_MouseOverStateType);
  }

  private void OnRollOut()
  {
    if (!this.m_UseMouseOver)
      return;
    this.m_MouseOver = false;
    if (this.m_AllowSelection)
      return;
    this.m_HighlightState.ChangeState(ActorStateType.NONE);
  }

  private void OnRelease()
  {
    if (this.m_AllowSelection)
      this.Select(true);
    else if (this.m_MouseOver)
      this.m_HighlightState.ChangeState(this.m_MouseOverStateType);
    else
      this.m_HighlightState.ChangeState(ActorStateType.NONE);
  }
}
