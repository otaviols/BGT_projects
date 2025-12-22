using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExternalUrlDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private ExternalUrl.AssetFlags m_assetFlags;
  [SerializeField]
  private ExternalUrl.Endpoint m_endpoint;
  [SerializeField]
  private string m_globalUrl = "False";

  [DbfField("ASSET_FLAGS")]
  public ExternalUrl.AssetFlags AssetFlags => this.m_assetFlags;

  [DbfField("ENDPOINT")]
  public ExternalUrl.Endpoint Endpoint => this.m_endpoint;

  [DbfField("GLOBAL_URL")]
  public string GlobalUrl => this.m_globalUrl;

  public List<RegionOverridesDbfRecord> RegionOverrides
  {
    get
    {
      int id = this.ID;
      List<RegionOverridesDbfRecord> regionOverrides = new List<RegionOverridesDbfRecord>();
      List<RegionOverridesDbfRecord> records = GameDbf.RegionOverrides.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        RegionOverridesDbfRecord overridesDbfRecord = records[index];
        if (overridesDbfRecord.ExternalUrlId == id)
          regionOverrides.Add(overridesDbfRecord);
      }
      return regionOverrides;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    if (name == "ASSET_FLAGS")
      return (object) this.m_assetFlags;
    if (name == "ENDPOINT")
      return (object) this.m_endpoint;
    return name == "GLOBAL_URL" ? (object) this.m_globalUrl : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
      {
        if (!(name == "ASSET_FLAGS"))
        {
          if (!(name == "ENDPOINT"))
          {
            if (!(name == "GLOBAL_URL"))
              return;
            this.m_globalUrl = (string) val;
          }
          else
          {
            switch (val)
            {
              case null:
                this.m_endpoint = ExternalUrl.Endpoint.ACCOUNT;
                break;
              case ExternalUrl.Endpoint _:
              case int _:
                this.m_endpoint = (ExternalUrl.Endpoint) val;
                break;
              case string _:
                this.m_endpoint = ExternalUrl.ParseEndpointValue((string) val);
                break;
            }
          }
        }
        else
        {
          switch (val)
          {
            case null:
              this.m_assetFlags = ExternalUrl.AssetFlags.NONE;
              break;
            case ExternalUrl.AssetFlags _:
            case int _:
              this.m_assetFlags = (ExternalUrl.AssetFlags) val;
              break;
            case string _:
              this.m_assetFlags = ExternalUrl.ParseAssetFlagsValue((string) val);
              break;
          }
        }
      }
      else
        this.m_noteDesc = (string) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "NOTE_DESC")
      return typeof (string);
    if (name == "ASSET_FLAGS")
      return typeof (ExternalUrl.AssetFlags);
    if (name == "ENDPOINT")
      return typeof (ExternalUrl.Endpoint);
    return name == "GLOBAL_URL" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadExternalUrlDbfRecords loadRecords = new LoadExternalUrlDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ExternalUrlDbfAsset externalUrlDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ExternalUrlDbfAsset)) as ExternalUrlDbfAsset;
    if ((UnityEngine.Object) externalUrlDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ExternalUrlDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < externalUrlDbfAsset.Records.Count; ++index)
      externalUrlDbfAsset.Records[index].StripUnusedLocales();
    records = externalUrlDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
