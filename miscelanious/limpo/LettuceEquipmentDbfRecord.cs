using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceEquipmentDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  public List<LettuceEquipmentTierDbfRecord> LettuceEquipmentTiers
  {
    get
    {
      int id = this.ID;
      List<LettuceEquipmentTierDbfRecord> lettuceEquipmentTiers = new List<LettuceEquipmentTierDbfRecord>();
      List<LettuceEquipmentTierDbfRecord> records = GameDbf.LettuceEquipmentTier.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LettuceEquipmentTierDbfRecord equipmentTierDbfRecord = records[index];
        if (equipmentTierDbfRecord.LettuceEquipmentId == id)
          lettuceEquipmentTiers.Add(equipmentTierDbfRecord);
      }
      return lettuceEquipmentTiers;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "NOTE_DESC" ? (object) this.m_noteDesc : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
        return;
      this.m_noteDesc = (string) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "NOTE_DESC" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceEquipmentDbfRecords loadRecords = new LoadLettuceEquipmentDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceEquipmentDbfAsset equipmentDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceEquipmentDbfAsset)) as LettuceEquipmentDbfAsset;
    if ((UnityEngine.Object) equipmentDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceEquipmentDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < equipmentDbfAsset.Records.Count; ++index)
      equipmentDbfAsset.Records[index].StripUnusedLocales();
    records = equipmentDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
