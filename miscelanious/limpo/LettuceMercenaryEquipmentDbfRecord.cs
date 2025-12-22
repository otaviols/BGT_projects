using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMercenaryEquipmentDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceMercenaryId;
  [SerializeField]
  private int m_lettuceEquipmentId;

  [DbfField("LETTUCE_MERCENARY_ID")]
  public int LettuceMercenaryId => this.m_lettuceMercenaryId;

  public LettuceEquipmentDbfRecord LettuceEquipmentRecord => GameDbf.LettuceEquipment.GetRecord(this.m_lettuceEquipmentId);

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_MERCENARY_ID")
      return (object) this.m_lettuceMercenaryId;
    return name == "LETTUCE_EQUIPMENT_ID" ? (object) this.m_lettuceEquipmentId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_MERCENARY_ID"))
      {
        if (!(name == "LETTUCE_EQUIPMENT_ID"))
          return;
        this.m_lettuceEquipmentId = (int) val;
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
    return name == "LETTUCE_EQUIPMENT_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMercenaryEquipmentDbfRecords loadRecords = new LoadLettuceMercenaryEquipmentDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMercenaryEquipmentDbfAsset equipmentDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMercenaryEquipmentDbfAsset)) as LettuceMercenaryEquipmentDbfAsset;
    if ((UnityEngine.Object) equipmentDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMercenaryEquipmentDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
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
