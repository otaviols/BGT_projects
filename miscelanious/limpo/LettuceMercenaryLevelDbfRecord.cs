using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMercenaryLevelDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_level;
  [SerializeField]
  private int m_totalXpRequired;

  [DbfField("LEVEL")]
  public int Level => this.m_level;

  [DbfField("TOTAL_XP_REQUIRED")]
  public int TotalXpRequired => this.m_totalXpRequired;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LEVEL")
      return (object) this.m_level;
    return name == "TOTAL_XP_REQUIRED" ? (object) this.m_totalXpRequired : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LEVEL"))
      {
        if (!(name == "TOTAL_XP_REQUIRED"))
          return;
        this.m_totalXpRequired = (int) val;
      }
      else
        this.m_level = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LEVEL")
      return typeof (int);
    return name == "TOTAL_XP_REQUIRED" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMercenaryLevelDbfRecords loadRecords = new LoadLettuceMercenaryLevelDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMercenaryLevelDbfAsset mercenaryLevelDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMercenaryLevelDbfAsset)) as LettuceMercenaryLevelDbfAsset;
    if ((UnityEngine.Object) mercenaryLevelDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMercenaryLevelDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < mercenaryLevelDbfAsset.Records.Count; ++index)
      mercenaryLevelDbfAsset.Records[index].StripUnusedLocales();
    records = mercenaryLevelDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
