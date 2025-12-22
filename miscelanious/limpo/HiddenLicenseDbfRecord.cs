using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HiddenLicenseDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_accountLicenseId;
  [SerializeField]
  private bool m_isBlocking = true;

  [DbfField("ACCOUNT_LICENSE_ID")]
  public int AccountLicenseId => this.m_accountLicenseId;

  [DbfField("IS_BLOCKING")]
  public bool IsBlocking => this.m_isBlocking;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "ACCOUNT_LICENSE_ID")
      return (object) this.m_accountLicenseId;
    return name == "IS_BLOCKING" ? (object) this.m_isBlocking : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ACCOUNT_LICENSE_ID"))
      {
        if (!(name == "IS_BLOCKING"))
          return;
        this.m_isBlocking = (bool) val;
      }
      else
        this.m_accountLicenseId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "ACCOUNT_LICENSE_ID")
      return typeof (int);
    return name == "IS_BLOCKING" ? typeof (bool) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadHiddenLicenseDbfRecords loadRecords = new LoadHiddenLicenseDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    HiddenLicenseDbfAsset hiddenLicenseDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (HiddenLicenseDbfAsset)) as HiddenLicenseDbfAsset;
    if ((UnityEngine.Object) hiddenLicenseDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("HiddenLicenseDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < hiddenLicenseDbfAsset.Records.Count; ++index)
      hiddenLicenseDbfAsset.Records[index].StripUnusedLocales();
    records = hiddenLicenseDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
