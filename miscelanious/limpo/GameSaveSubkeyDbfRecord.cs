using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveSubkeyDbfRecord : DbfRecord
{
  public override object GetVar(string name) => name == "ID" ? (object) this.ID : (object) null;

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
      return;
    this.SetID((int) val);
  }

  public override System.Type GetVarType(string name) => name == "ID" ? typeof (int) : (System.Type) null;

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadGameSaveSubkeyDbfRecords loadRecords = new LoadGameSaveSubkeyDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    GameSaveSubkeyDbfAsset saveSubkeyDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (GameSaveSubkeyDbfAsset)) as GameSaveSubkeyDbfAsset;
    if ((UnityEngine.Object) saveSubkeyDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("GameSaveSubkeyDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < saveSubkeyDbfAsset.Records.Count; ++index)
      saveSubkeyDbfAsset.Records[index].StripUnusedLocales();
    records = saveSubkeyDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
