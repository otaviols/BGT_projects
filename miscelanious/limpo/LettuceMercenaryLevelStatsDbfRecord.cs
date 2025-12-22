using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMercenaryLevelStatsDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceMercenaryId;
  [SerializeField]
  private int m_lettuceMercenaryLevelId;
  [SerializeField]
  private int m_attack = 1;
  [SerializeField]
  private int m_health = 1;

  [DbfField("LETTUCE_MERCENARY_ID")]
  public int LettuceMercenaryId => this.m_lettuceMercenaryId;

  [DbfField("LETTUCE_MERCENARY_LEVEL_ID")]
  public int LettuceMercenaryLevelId => this.m_lettuceMercenaryLevelId;

  [DbfField("ATTACK")]
  public int Attack => this.m_attack;

  [DbfField("HEALTH")]
  public int Health => this.m_health;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_MERCENARY_ID")
      return (object) this.m_lettuceMercenaryId;
    if (name == "LETTUCE_MERCENARY_LEVEL_ID")
      return (object) this.m_lettuceMercenaryLevelId;
    if (name == "ATTACK")
      return (object) this.m_attack;
    return name == "HEALTH" ? (object) this.m_health : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_MERCENARY_ID"))
      {
        if (!(name == "LETTUCE_MERCENARY_LEVEL_ID"))
        {
          if (!(name == "ATTACK"))
          {
            if (!(name == "HEALTH"))
              return;
            this.m_health = (int) val;
          }
          else
            this.m_attack = (int) val;
        }
        else
          this.m_lettuceMercenaryLevelId = (int) val;
      }
      else
        this.m_lettuceMercenaryId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_MERCENARY_ID")
      return typeof (int);
    if (name == "LETTUCE_MERCENARY_LEVEL_ID")
      return typeof (int);
    if (name == "ATTACK")
      return typeof (int);
    return name == "HEALTH" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMercenaryLevelStatsDbfRecords loadRecords = new LoadLettuceMercenaryLevelStatsDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMercenaryLevelStatsDbfAsset levelStatsDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMercenaryLevelStatsDbfAsset)) as LettuceMercenaryLevelStatsDbfAsset;
    if ((UnityEngine.Object) levelStatsDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMercenaryLevelStatsDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < levelStatsDbfAsset.Records.Count; ++index)
      levelStatsDbfAsset.Records[index].StripUnusedLocales();
    records = levelStatsDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
