using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class XpOnPlacementDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_rewardTrackId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "REWARD_TRACK_ID" ? (object) this.m_rewardTrackId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "REWARD_TRACK_ID"))
        return;
      this.m_rewardTrackId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "REWARD_TRACK_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadXpOnPlacementDbfRecords loadRecords = new LoadXpOnPlacementDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    XpOnPlacementDbfAsset placementDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (XpOnPlacementDbfAsset)) as XpOnPlacementDbfAsset;
    if ((UnityEngine.Object) placementDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("XpOnPlacementDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < placementDbfAsset.Records.Count; ++index)
      placementDbfAsset.Records[index].StripUnusedLocales();
    records = placementDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
