using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NextTiersDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_buildingTierId;
  [SerializeField]
  private int m_nextTierId;

  [DbfField("BUILDING_TIER_ID")]
  public int BuildingTierId => this.m_buildingTierId;

  [DbfField("NEXT_TIER_ID")]
  public int NextTierId => this.m_nextTierId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "BUILDING_TIER_ID")
      return (object) this.m_buildingTierId;
    return name == "NEXT_TIER_ID" ? (object) this.m_nextTierId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "BUILDING_TIER_ID"))
      {
        if (!(name == "NEXT_TIER_ID"))
          return;
        this.m_nextTierId = (int) val;
      }
      else
        this.m_buildingTierId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "BUILDING_TIER_ID")
      return typeof (int);
    return name == "NEXT_TIER_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadNextTiersDbfRecords loadRecords = new LoadNextTiersDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    NextTiersDbfAsset nextTiersDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (NextTiersDbfAsset)) as NextTiersDbfAsset;
    if ((UnityEngine.Object) nextTiersDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("NextTiersDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < nextTiersDbfAsset.Records.Count; ++index)
      nextTiersDbfAsset.Records[index].StripUnusedLocales();
    records = nextTiersDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
