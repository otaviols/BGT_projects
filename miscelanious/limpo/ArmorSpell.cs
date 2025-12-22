using UnityEngine;

public class ArmorSpell : Spell
{
  public UberText m_ArmorText;
  private int m_armor;

  public int GetArmor() => this.m_armor;

  public void SetArmor(int armor)
  {
    this.m_armor = armor;
    this.UpdateArmorText();
  }

  private void UpdateArmorText()
  {
    if ((Object) this.m_ArmorText == (Object) null)
      return;
    string str = this.m_armor.ToString();
    if (this.m_armor == 0)
      str = "";
    this.m_ArmorText.Text = str;
  }
}
