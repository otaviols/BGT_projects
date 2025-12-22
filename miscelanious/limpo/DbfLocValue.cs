using System;
using UnityEngine;

[Serializable]
public class DbfLocValue
{
  [SerializeField]
  private int m_locId;
  private int m_recordId;
  private string m_recordColumn;
  [SerializeField]
  private string m_currentLocaleValue = string.Empty;
  private bool m_stripped;
  private bool m_hideDebugInfo = true;

  public DbfLocValue()
  {
  }

  public DbfLocValue(bool hideDebugInfo) => this.m_hideDebugInfo = hideDebugInfo;

  public string GetString() => this.GetString(Localization.GetLocale());

  public string GetString(Locale loc)
  {
    if (this.m_stripped)
      return this.m_currentLocaleValue;
    return !this.m_hideDebugInfo ? string.Format("ID={0} COLUMN={1}", (object) this.m_recordId, (object) this.m_recordColumn) : string.Empty;
  }

  public void SetString(Locale loc, string value)
  {
    if (!this.m_stripped)
    {
      Locale actualLocale = Localization.GetActualLocale(loc);
      if (Localization.SupportedLocales.IndexOf(actualLocale) >= 0)
      {
        this.m_currentLocaleValue = value;
        this.m_stripped = true;
      }
      else
        Debug.LogWarning((object) string.Format("Locale {0} is unsupported. Unable to set localization string {1}", (object) loc, (object) value));
    }
    else
    {
      if (loc != Localization.GetActualLocale())
        return;
      this.m_currentLocaleValue = value;
    }
  }

  public void SetString(string value) => this.SetString(Localization.GetLocale(), value);

  public void SetLocId(int locId) => this.m_locId = locId;

  public void SetDebugInfo(int recordId, string recordColumn)
  {
    this.m_recordId = recordId;
    this.m_recordColumn = recordColumn;
  }

  public static implicit operator string(DbfLocValue v) => v == null ? string.Empty : v.GetString();

  public void StripUnusedLocales() => this.m_stripped = true;
}
