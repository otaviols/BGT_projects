using UnityEngine;

public class DropdownMenuItem : PegUIElement
{
  public GameObject m_selected;
  public UberText m_text;
  private object m_value;

  public object GetValue() => this.m_value;

  public void SetValue(object val, string text)
  {
    this.m_value = val;
    this.m_text.Text = text;
  }

  public void SetSelected(bool selected)
  {
    if ((Object) this.m_selected == (Object) null)
      return;
    this.m_selected.SetActive(selected);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState) => this.m_text.TextColor = Color.white;

  protected override void OnOut(PegUIElement.InteractionState oldState) => this.m_text.TextColor = Color.black;
}
