using UnityEngine;

public class DamageCapPanel : MonoBehaviour
{
  public UberText m_text;

  public void SetText(string text)
  {
    if (text == "")
      this.ClearText();
    else
      this.m_text.gameObject.SetActive(true);
    this.m_text.Text = text;
  }

  public void ClearText()
  {
    this.m_text.Text = "";
    this.m_text.gameObject.SetActive(false);
  }
}
