using UnityEngine;

public class BoxMenuButton : PegUIElement
{
  public UberText m_TextMesh;
  public Spell m_Spell;
  public HighlightState m_HighlightState;

  public string GetText() => this.m_TextMesh.Text;

  public void SetText(string text) => this.m_TextMesh.Text = text;

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if ((Object) this.m_Spell == (Object) null)
      return;
    this.m_Spell.ActivateState(SpellStateType.BIRTH);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    if ((Object) this.m_Spell == (Object) null)
      return;
    this.m_Spell.ActivateState(SpellStateType.DEATH);
  }

  protected override void OnPress()
  {
    if ((Object) this.m_Spell == (Object) null || DialogManager.Get().ShowingDialog())
      return;
    this.m_Spell.ActivateState(SpellStateType.IDLE);
  }

  protected override void OnRelease()
  {
    if ((Object) this.m_Spell == (Object) null)
      return;
    this.m_Spell.ActivateState(SpellStateType.ACTION);
  }
}
