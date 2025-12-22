using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceEquipmentModifierDataDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceEquipmentTierId;
  [SerializeField]
  private int m_mercenaryAttackChange;
  [SerializeField]
  private int m_mercenaryHealthChange;

  [DbfField("LETTUCE_EQUIPMENT_TIER_ID")]
  public int LettuceEquipmentTierId => this.m_lettuceEquipmentTierId;

  [DbfField("MERCENARY_ATTACK_CHANGE")]
  public int MercenaryAttackChange => this.m_mercenaryAttackChange;

  [DbfField("MERCENARY_HEALTH_CHANGE")]
  public int MercenaryHealthChange => this.m_mercenaryHealthChange;

  public List<ModifiedLettuceAbilityValueDbfRecord> ModifiedLettuceAbilityValues
  {
    get
    {
      int id = this.ID;
      List<ModifiedLettuceAbilityValueDbfRecord> lettuceAbilityValues = new List<ModifiedLettuceAbilityValueDbfRecord>();
      List<ModifiedLettuceAbilityValueDbfRecord> records = GameDbf.ModifiedLettuceAbilityValue.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        ModifiedLettuceAbilityValueDbfRecord abilityValueDbfRecord = records[index];
        if (abilityValueDbfRecord.LettuceModifierDataId == id)
          lettuceAbilityValues.Add(abilityValueDbfRecord);
      }
      return lettuceAbilityValues;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_EQUIPMENT_TIER_ID")
      return (object) this.m_lettuceEquipmentTierId;
    if (name == "MERCENARY_ATTACK_CHANGE")
      return (object) this.m_mercenaryAttackChange;
    return name == "MERCENARY_HEALTH_CHANGE" ? (object) this.m_mercenaryHealthChange : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_EQUIPMENT_TIER_ID"))
      {
        if (!(name == "MERCENARY_ATTACK_CHANGE"))
        {
          if (!(name == "MERCENARY_HEALTH_CHANGE"))
            return;
          this.m_mercenaryHealthChange = (int) val;
        }
        else
          this.m_mercenaryAttackChange = (int) val;
      }
      else
        this.m_lettuceEquipmentTierId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_EQUIPMENT_TIER_ID")
      return typeof (int);
    if (name == "MERCENARY_ATTACK_CHANGE")
      return typeof (int);
    return name == "MERCENARY_HEALTH_CHANGE" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceEquipmentModifierDataDbfRecords loadRecords = new LoadLettuceEquipmentModifierDataDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceEquipmentModifierDataDbfAsset modifierDataDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceEquipmentModifierDataDbfAsset)) as LettuceEquipmentModifierDataDbfAsset;
    if ((UnityEngine.Object) modifierDataDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceEquipmentModifierDataDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < modifierDataDbfAsset.Records.Count; ++index)
      modifierDataDbfAsset.Records[index].StripUnusedLocales();
    records = modifierDataDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
