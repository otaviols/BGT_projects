using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceTreasureDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_requiredAbilityId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "REQUIRED_ABILITY" ? (object) this.m_requiredAbilityId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "REQUIRED_ABILITY"))
        return;
      this.m_requiredAbilityId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "REQUIRED_ABILITY" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceTreasureDbfRecords loadRecords = new LoadLettuceTreasureDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceTreasureDbfAsset treasureDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceTreasureDbfAsset)) as LettuceTreasureDbfAsset;
    if ((UnityEngine.Object) treasureDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceTreasureDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < treasureDbfAsset.Records.Count; ++index)
      treasureDbfAsset.Records[index].StripUnusedLocales();
    records = treasureDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
