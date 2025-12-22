using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMercenarySpecializationDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceMercenaryId;
  [SerializeField]
  private DbfLocValue m_name;

  [DbfField("LETTUCE_MERCENARY_ID")]
  public int LettuceMercenaryId => this.m_lettuceMercenaryId;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  public List<LettuceMercenaryAbilityDbfRecord> LettuceMercenaryAbilities
  {
    get
    {
      int id = this.ID;
      List<LettuceMercenaryAbilityDbfRecord> mercenaryAbilities = new List<LettuceMercenaryAbilityDbfRecord>();
      List<LettuceMercenaryAbilityDbfRecord> records = GameDbf.LettuceMercenaryAbility.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LettuceMercenaryAbilityDbfRecord abilityDbfRecord = records[index];
        if (abilityDbfRecord.LettuceMercenarySpecializationId == id)
          mercenaryAbilities.Add(abilityDbfRecord);
      }
      return mercenaryAbilities;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_MERCENARY_ID")
      return (object) this.m_lettuceMercenaryId;
    return name == "NAME" ? (object) this.m_name : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_MERCENARY_ID"))
      {
        if (!(name == "NAME"))
          return;
        this.m_name = (DbfLocValue) val;
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
    return name == "NAME" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMercenarySpecializationDbfRecords loadRecords = new LoadLettuceMercenarySpecializationDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMercenarySpecializationDbfAsset specializationDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMercenarySpecializationDbfAsset)) as LettuceMercenarySpecializationDbfAsset;
    if ((UnityEngine.Object) specializationDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMercenarySpecializationDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < specializationDbfAsset.Records.Count; ++index)
      specializationDbfAsset.Records[index].StripUnusedLocales();
    records = specializationDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_name.StripUnusedLocales();
}
