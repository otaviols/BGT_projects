using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LuckyDrawBoxDbfRecord : DbfRecord
{
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private string m_theme;
  [SerializeField]
  private string m_layout;
  [SerializeField]
  private int m_accountLicenseId;
  [SerializeField]
  private int m_freeCount = 1;
  [SerializeField]
  private int m_bonusCount = 2;
  [SerializeField]
  private int m_earnCount = 1;
  [SerializeField]
  private LuckyDrawBox.LuckyDrawEarnHammerCondition m_earnCondition;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  [DbfField("THEME")]
  public string Theme => this.m_theme;

  [DbfField("LAYOUT")]
  public string Layout => this.m_layout;

  [DbfField("ACCOUNT_LICENSE_ID")]
  public int AccountLicenseId => this.m_accountLicenseId;

  public AccountLicenseDbfRecord AccountLicenseRecord => GameDbf.AccountLicense.GetRecord(this.m_accountLicenseId);

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACCOUNT_LICENSE_ID":
        return (object) this.m_accountLicenseId;
      case "BONUS_COUNT":
        return (object) this.m_bonusCount;
      case "EARN_CONDITION":
        return (object) this.m_earnCondition;
      case "EARN_COUNT":
        return (object) this.m_earnCount;
      case "EVENT":
        return (object) this.m_event;
      case "FREE_COUNT":
        return (object) this.m_freeCount;
      case "ID":
        return (object) this.ID;
      case "LAYOUT":
        return (object) this.m_layout;
      case "NAME":
        return (object) this.m_name;
      case "THEME":
        return (object) this.m_theme;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 236776447:
        if (!(name == "EVENT"))
          break;
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 484759303:
        if (!(name == "EARN_COUNT"))
          break;
        this.m_earnCount = (int) val;
        break;
      case 1225950840:
        if (!(name == "BONUS_COUNT"))
          break;
        this.m_bonusCount = (int) val;
        break;
      case 1370899602:
        if (!(name == "THEME"))
          break;
        this.m_theme = (string) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1423153279:
        if (!(name == "LAYOUT"))
          break;
        this.m_layout = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1794579923:
        if (!(name == "FREE_COUNT"))
          break;
        this.m_freeCount = (int) val;
        break;
      case 2437470029:
        if (!(name == "EARN_CONDITION"))
          break;
        switch (val)
        {
          case null:
            this.m_earnCondition = LuckyDrawBox.LuckyDrawEarnHammerCondition.BATTLEGROUNDS_WIN;
            return;
          case LuckyDrawBox.LuckyDrawEarnHammerCondition _:
          case int _:
            this.m_earnCondition = (LuckyDrawBox.LuckyDrawEarnHammerCondition) val;
            return;
          case string _:
            this.m_earnCondition = LuckyDrawBox.ParseLuckyDrawEarnHammerConditionValue((string) val);
            return;
          default:
            return;
        }
      case 3365816664:
        if (!(name == "ACCOUNT_LICENSE_ID"))
          break;
        this.m_accountLicenseId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACCOUNT_LICENSE_ID":
        return typeof (int);
      case "BONUS_COUNT":
        return typeof (int);
      case "EARN_CONDITION":
        return typeof (LuckyDrawBox.LuckyDrawEarnHammerCondition);
      case "EARN_COUNT":
        return typeof (int);
      case "EVENT":
        return typeof (string);
      case "FREE_COUNT":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "LAYOUT":
        return typeof (string);
      case "NAME":
        return typeof (DbfLocValue);
      case "THEME":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLuckyDrawBoxDbfRecords loadRecords = new LoadLuckyDrawBoxDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LuckyDrawBoxDbfAsset luckyDrawBoxDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LuckyDrawBoxDbfAsset)) as LuckyDrawBoxDbfAsset;
    if ((UnityEngine.Object) luckyDrawBoxDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LuckyDrawBoxDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < luckyDrawBoxDbfAsset.Records.Count; ++index)
      luckyDrawBoxDbfAsset.Records[index].StripUnusedLocales();
    records = luckyDrawBoxDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_name.StripUnusedLocales();
}
