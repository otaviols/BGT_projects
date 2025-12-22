using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceAbilityDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private DbfLocValue m_abilityName;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("ABILITY_NAME")]
  public DbfLocValue AbilityName => this.m_abilityName;

  public List<LettuceAbilityTierDbfRecord> LettuceAbilityTiers
  {
    get
    {
      int id = this.ID;
      List<LettuceAbilityTierDbfRecord> lettuceAbilityTiers = new List<LettuceAbilityTierDbfRecord>();
      List<LettuceAbilityTierDbfRecord> records = GameDbf.LettuceAbilityTier.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LettuceAbilityTierDbfRecord abilityTierDbfRecord = records[index];
        if (abilityTierDbfRecord.LettuceAbilityId == id)
          lettuceAbilityTiers.Add(abilityTierDbfRecord);
      }
      return lettuceAbilityTiers;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    return name == "ABILITY_NAME" ? (object) this.m_abilityName : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
      {
        if (!(name == "ABILITY_NAME"))
          return;
        this.m_abilityName = (DbfLocValue) val;
      }
      else
        this.m_noteDesc = (string) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "NOTE_DESC")
      return typeof (string);
    return name == "ABILITY_NAME" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceAbilityDbfRecords loadRecords = new LoadLettuceAbilityDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceAbilityDbfAsset lettuceAbilityDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceAbilityDbfAsset)) as LettuceAbilityDbfAsset;
    if ((UnityEngine.Object) lettuceAbilityDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceAbilityDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < lettuceAbilityDbfAsset.Records.Count; ++index)
      lettuceAbilityDbfAsset.Records[index].StripUnusedLocales();
    records = lettuceAbilityDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_abilityName.StripUnusedLocales();
}
