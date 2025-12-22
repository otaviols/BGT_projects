using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattlegroundsSeasonDbfRecord : DbfRecord
{
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;

  public override object GetVar(string name) => name == "EVENT" ? (object) this.m_event : (object) null;

  public override void SetVar(string name, object val)
  {
    if (!(name == "EVENT"))
      return;
    this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
  }

  public override System.Type GetVarType(string name) => name == "EVENT" ? typeof (string) : (System.Type) null;

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBattlegroundsSeasonDbfRecords loadRecords = new LoadBattlegroundsSeasonDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BattlegroundsSeasonDbfAsset battlegroundsSeasonDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BattlegroundsSeasonDbfAsset)) as BattlegroundsSeasonDbfAsset;
    if ((UnityEngine.Object) battlegroundsSeasonDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BattlegroundsSeasonDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < battlegroundsSeasonDbfAsset.Records.Count; ++index)
      battlegroundsSeasonDbfAsset.Records[index].StripUnusedLocales();
    records = battlegroundsSeasonDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
