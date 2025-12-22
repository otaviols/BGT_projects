using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementSectionDbfRecord : DbfRecord
{
  [SerializeField]
  private DbfLocValue m_name;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "NAME" ? (object) this.m_name : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NAME"))
        return;
      this.m_name = (DbfLocValue) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "NAME" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAchievementSectionDbfRecords loadRecords = new LoadAchievementSectionDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AchievementSectionDbfAsset achievementSectionDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AchievementSectionDbfAsset)) as AchievementSectionDbfAsset;
    if ((UnityEngine.Object) achievementSectionDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AchievementSectionDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < achievementSectionDbfAsset.Records.Count; ++index)
      achievementSectionDbfAsset.Records[index].StripUnusedLocales();
    records = achievementSectionDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_name.StripUnusedLocales();
}
