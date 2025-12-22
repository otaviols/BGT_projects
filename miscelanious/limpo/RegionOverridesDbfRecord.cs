using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RegionOverridesDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_externalUrlId;
  [SerializeField]
  private string m_region;
  [SerializeField]
  private string m_overrideUrl;

  [DbfField("EXTERNAL_URL_ID")]
  public int ExternalUrlId => this.m_externalUrlId;

  [DbfField("REGION")]
  public string Region => this.m_region;

  [DbfField("OVERRIDE_URL")]
  public string OverrideUrl => this.m_overrideUrl;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "EXTERNAL_URL_ID")
      return (object) this.m_externalUrlId;
    if (name == "REGION")
      return (object) this.m_region;
    return name == "OVERRIDE_URL" ? (object) this.m_overrideUrl : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "EXTERNAL_URL_ID"))
      {
        if (!(name == "REGION"))
        {
          if (!(name == "OVERRIDE_URL"))
            return;
          this.m_overrideUrl = (string) val;
        }
        else
          this.m_region = (string) val;
      }
      else
        this.m_externalUrlId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "EXTERNAL_URL_ID")
      return typeof (int);
    if (name == "REGION")
      return typeof (string);
    return name == "OVERRIDE_URL" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRegionOverridesDbfRecords loadRecords = new LoadRegionOverridesDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RegionOverridesDbfAsset overridesDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RegionOverridesDbfAsset)) as RegionOverridesDbfAsset;
    if ((UnityEngine.Object) overridesDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RegionOverridesDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < overridesDbfAsset.Records.Count; ++index)
      overridesDbfAsset.Records[index].StripUnusedLocales();
    records = overridesDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
