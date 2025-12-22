using System;
using System.Text;

public static class TextUtils
{
  private static StringBuilder s_textBuffer = new StringBuilder(128);

  public static string DecodeWhitespaces(string text)
  {
    text = text.Replace("\\n", "\n");
    text = text.Replace("\\t", "\t");
    return text;
  }

  public static string TransformCardText(Entity entity, string text)
  {
    TextUtils.TransformCardTextParams parameters = new TextUtils.TransformCardTextParams()
    {
      DamageBonus = entity.GetDamageBonus(),
      DamageBonusDouble = entity.GetDamageBonusDouble(),
      HealingBonus = entity.GetHealingBonus(),
      HealingDouble = entity.GetHealingDouble()
    };
    return TextUtils.TransformCardText(text, parameters);
  }

  public static string TransformCardText(CardTextHistoryData historyData, string text)
  {
    TextUtils.TransformCardTextParams parameters = new TextUtils.TransformCardTextParams()
    {
      DamageBonus = historyData.m_damageBonus,
      DamageBonusDouble = historyData.m_damageBonusDouble,
      HealingBonus = historyData.m_healingBonus,
      HealingDouble = historyData.m_healingDouble
    };
    return TextUtils.TransformCardText(text, parameters);
  }

  public static string TransformCardText(string text, TextUtils.TransformCardTextParams parameters = null)
  {
    int damageBonus = parameters != null ? parameters.DamageBonus : 0;
    int num1 = parameters != null ? parameters.DamageBonusDouble : 0;
    int num2 = parameters != null ? parameters.HealingBonus : 0;
    int num3 = parameters != null ? parameters.HealingDouble : 0;
    int damageBonusDouble = num1;
    int healingBonus = num2;
    int healingDouble = num3;
    string powersText = text;
    return GameStrings.ParseLanguageRules(TextUtils.TransformCardTextImpl(damageBonus, damageBonusDouble, healingBonus, healingDouble, powersText));
  }

  public static string ToHexString(this byte[] bytes)
  {
    char[] chArray = new char[bytes.Length * 2];
    for (int index = 0; index < bytes.Length; ++index)
    {
      int num1 = (int) bytes[index] >> 4;
      chArray[index * 2] = (char) (55 + num1 + (num1 - 10 >> 31 & -7));
      int num2 = (int) bytes[index] & 15;
      chArray[index * 2 + 1] = (char) (55 + num2 + (num2 - 10 >> 31 & -7));
    }
    return new string(chArray);
  }

  public static string ToHexString(string str) => Encoding.UTF8.GetBytes(str).ToHexString();

  public static string FromHexString(string str)
  {
    byte[] bytes = str.Length % 2 != 1 ? new byte[str.Length >> 1] : throw new Exception("Hex string must have an even number of digits");
    for (int index = 0; index < str.Length >> 1; ++index)
      bytes[index] = (byte) ((TextUtils.GetHexValue(str[index << 1]) << 4) + TextUtils.GetHexValue(str[(index << 1) + 1]));
    return Encoding.UTF8.GetString(bytes);
  }

  private static int GetHexValue(char hex)
  {
    int num = (int) hex;
    return num - (num < 58 ? 48 : 55);
  }

  public static bool HasBonusDamage(string powersText) => TextUtils.HasBonusToken(powersText, '$');

  public static bool HasBonusHealing(string powersText) => TextUtils.HasBonusToken(powersText, '#');

  private static bool HasBonusToken(string powersText, char token)
  {
    if (powersText == null)
      return false;
    for (int index1 = 0; index1 < powersText.Length; ++index1)
    {
      if ((int) powersText[index1] == (int) token)
      {
        int index2;
        for (index2 = ++index1; index2 < powersText.Length; ++index2)
        {
          int c = (int) powersText[index2];
          bool flag1 = char.IsDigit((char) c);
          bool flag2 = c == 64;
          bool flag3 = false;
          if (c == 123 && index2 + 1 < powersText.Length)
          {
            switch (powersText[index2 + 1])
            {
              case '0':
              case '1':
                if (index2 + 2 < powersText.Length)
                {
                  flag3 = powersText[index2 + 2] == '}';
                  if (flag3)
                  {
                    index2 += 2;
                    break;
                  }
                  break;
                }
                break;
            }
          }
          if (!flag1 && !flag2 && !flag3)
            break;
        }
        if (index2 != index1)
          return true;
      }
    }
    return false;
  }

  private static string TransformCardTextImpl(
    int damageBonus,
    int damageBonusDouble,
    int healingBonus,
    int healingDouble,
    string powersText)
  {
    if (powersText == null || powersText == string.Empty)
      return string.Empty;
    TextUtils.s_textBuffer.Clear();
    bool flag1 = damageBonus != 0 || damageBonusDouble > 0;
    bool flag2 = healingBonus != 0 || healingDouble > 0;
    for (int index1 = 0; index1 < powersText.Length; ++index1)
    {
      char ch = powersText[index1];
      switch (ch)
      {
        case '#':
        case '$':
          int index2;
          for (index2 = ++index1; index2 < powersText.Length; ++index2)
          {
            switch (powersText[index2])
            {
              case '0':
              case '1':
              case '2':
              case '3':
              case '4':
              case '5':
              case '6':
              case '7':
              case '8':
              case '9':
                continue;
              default:
                goto label_9;
            }
          }
label_9:
          if (index2 != index1)
          {
            int num = Convert.ToInt32(powersText.Substring(index1, index2 - index1));
            switch (ch)
            {
              case '#':
                num += healingBonus;
                for (int index3 = 0; index3 < healingDouble; ++index3)
                  num *= 2;
                break;
              case '$':
                num += damageBonus;
                for (int index4 = 0; index4 < damageBonusDouble; ++index4)
                  num *= 2;
                if (num < 0)
                {
                  num = 0;
                  break;
                }
                break;
            }
            if (flag1 && ch == '$' || flag2 && ch == '#')
            {
              TextUtils.s_textBuffer.Append('*');
              TextUtils.s_textBuffer.Append(num);
              TextUtils.s_textBuffer.Append('*');
            }
            else
              TextUtils.s_textBuffer.Append(num);
            index1 = index2 - 1;
            break;
          }
          break;
        default:
          TextUtils.s_textBuffer.Append(ch);
          break;
      }
    }
    return TextUtils.s_textBuffer.ToString();
  }

  public class TransformCardTextParams
  {
    public int DamageBonus { get; set; }

    public int DamageBonusDouble { get; set; }

    public int HealingBonus { get; set; }

    public int HealingDouble { get; set; }
  }
}
