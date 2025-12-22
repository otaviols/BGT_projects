using Hearthstone.UI;
using UnityEngine;

[CustomEditClass]
public class AdventureChooserDescription : MonoBehaviour
{
  [SerializeField]
  private Color32 m_WarningTextColor = new Color32(byte.MaxValue, (byte) 210, (byte) 23, byte.MaxValue);
  [CustomEditField(Sections = "Description")]
  public UberText m_DescriptionObject;
  public AsyncReference m_WidgetElement;
  private string m_RequiredText;
  private string m_DescText;

  [CustomEditField(Sections = "Description")]
  public Color WarningTextColor
  {
    get => (Color) this.m_WarningTextColor;
    set
    {
      this.m_WarningTextColor = (Color32) value;
      this.RefreshText();
    }
  }

  public string GetText() => this.m_DescriptionObject.Text;

  public void SetText(string requiredText, string descText)
  {
    this.m_RequiredText = requiredText;
    this.m_DescText = descText;
    this.RefreshText();
  }

  private void RefreshText()
  {
    string str;
    if (!string.IsNullOrEmpty(this.m_RequiredText))
      str = "<color=#" + (this.m_WarningTextColor.r.ToString("X2") + this.m_WarningTextColor.g.ToString("X2") + this.m_WarningTextColor.b.ToString("X2")) + ">" + this.m_RequiredText + "</color>\n" + this.m_DescText;
    else
      str = this.m_DescText;
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_DescriptionObject.CharacterSize = 70f;
    this.m_DescriptionObject.Text = str;
  }
}
