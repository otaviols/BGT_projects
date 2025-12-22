using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AccountLicenseDbfRecord : DbfRecord
{
  [SerializeField]
  private long m_licenseId;

  [DbfField("LICENSE_ID")]
  public long LicenseId => this.m_licenseId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "LICENSE_ID" ? (object) this.m_licenseId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LICENSE_ID"))
        return;
      this.m_licenseId = (long) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "LICENSE_ID" ? typeof (long) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAccountLicenseDbfRecords loadRecords = new LoadAccountLicenseDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AccountLicenseDbfAsset accountLicenseDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AccountLicenseDbfAsset)) as AccountLicenseDbfAsset;
    if ((UnityEngine.Object) accountLicenseDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AccountLicenseDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < accountLicenseDbfAsset.Records.Count; ++index)
      accountLicenseDbfAsset.Records[index].StripUnusedLocales();
    records = accountLicenseDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
