using System;
using System.Text;

[Serializable]
public class FourCC
{
  protected uint m_value;

  public FourCC()
  {
  }

  public FourCC(string stringVal) => this.SetString(stringVal);

  public uint GetValue() => this.m_value;

  public string GetString()
  {
    StringBuilder stringBuilder = new StringBuilder(4);
    for (int index = 24; index >= 0; index -= 8)
    {
      char ch = (char) (this.m_value >> index & (uint) byte.MaxValue);
      if (ch != char.MinValue)
        stringBuilder.Append(ch);
    }
    return stringBuilder.ToString();
  }

  public void SetString(string str)
  {
    this.m_value = 0U;
    for (int index = 0; index < str.Length && index < 4; ++index)
      this.m_value = this.m_value << 8 | (uint) (byte) str[index];
  }

  public override bool Equals(object obj) => obj != null && obj is FourCC fourCc && (int) this.m_value == (int) fourCc.m_value;

  public override int GetHashCode() => this.m_value.GetHashCode();

  public override string ToString() => this.GetString();
}
