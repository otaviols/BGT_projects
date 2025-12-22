using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardDiscoverStringDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteMiniGuid;
  [SerializeField]
  private string m_stringId;

  [DbfField("NOTE_MINI_GUID")]
  public string NoteMiniGuid => this.m_noteMiniGuid;

  [DbfField("STRING_ID")]
  public string StringId => this.m_stringId;

  public override object GetVar(string name)
  {
    if (name == "NOTE_MINI_GUID")
      return (object) this.m_noteMiniGuid;
    return name == "STRING_ID" ? (object) this.m_stringId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "NOTE_MINI_GUID"))
    {
      if (!(name == "STRING_ID"))
        return;
      this.m_stringId = (string) val;
    }
    else
      this.m_noteMiniGuid = (string) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "NOTE_MINI_GUID")
      return typeof (string);
    return name == "STRING_ID" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardDiscoverStringDbfRecords loadRecords = new LoadCardDiscoverStringDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardDiscoverStringDbfAsset discoverStringDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardDiscoverStringDbfAsset)) as CardDiscoverStringDbfAsset;
    if ((UnityEngine.Object) discoverStringDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardDiscoverStringDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < discoverStringDbfAsset.Records.Count; ++index)
      discoverStringDbfAsset.Records[index].StripUnusedLocales();
    records = discoverStringDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
