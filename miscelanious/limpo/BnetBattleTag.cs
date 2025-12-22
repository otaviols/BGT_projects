using UnityEngine;

public class BnetBattleTag
{
  private string m_name;
  private string m_number;

  public static BnetBattleTag CreateFromString(string src)
  {
    BnetBattleTag bnetBattleTag = new BnetBattleTag();
    return !bnetBattleTag.SetString(src) ? (BnetBattleTag) null : bnetBattleTag;
  }

  public BnetBattleTag Clone() => (BnetBattleTag) this.MemberwiseClone();

  public string GetName() => this.m_name;

  public void SetName(string name) => this.m_name = name;

  public string GetNumber() => this.m_number;

  public void SetNumber(string number) => this.m_number = number;

  public string GetString() => string.Format("{0}#{1}", (object) this.m_name, (object) this.m_number);

  public bool SetString(string composite)
  {
    if (composite == null)
    {
      Error.AddDevFatal("BnetBattleTag.SetString() - Given null string.");
      return false;
    }
    string[] strArray = composite.Split('#');
    if (strArray.Length < 2)
    {
      Debug.LogWarningFormat("BnetBattleTag.SetString() - Failed to split BattleTag \"{0}\" into 2 parts - this will prevent this player from showing up in Friends list and other places.", (object) composite);
      return false;
    }
    this.m_name = strArray[0];
    this.m_number = strArray[1];
    return true;
  }

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;
    BnetBattleTag bnetBattleTag = obj as BnetBattleTag;
    return (object) bnetBattleTag != null && this.m_name == bnetBattleTag.m_name && this.m_number == bnetBattleTag.m_number;
  }

  public override int GetHashCode() => (17 * 11 + this.m_name.GetHashCode()) * 11 + this.m_number.GetHashCode();

  public static bool operator ==(BnetBattleTag a, BnetBattleTag b)
  {
    if ((object) a == (object) b)
      return true;
    return (object) a != null && (object) b != null && a.m_name == b.m_name && a.m_number == b.m_number;
  }

  public static bool operator !=(BnetBattleTag a, BnetBattleTag b) => !(a == b);

  public override string ToString() => string.Format("{0}#{1}", (object) this.m_name, (object) this.m_number);
}
