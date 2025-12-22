using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifiedLettuceAbilityValueDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceModifierDataId;
  [SerializeField]
  private int m_lettuceAbilityId;
  [SerializeField]
  private int m_attackChange;
  [SerializeField]
  private int m_healthChange;
  [SerializeField]
  private int m_speedChange;
  [SerializeField]
  private int m_cooldownChange;
  [SerializeField]
  private int m_scriptDataNum1Change;
  [SerializeField]
  private int m_scriptDataNum2Change;

  [DbfField("LETTUCE_MODIFIER_DATA_ID")]
  public int LettuceModifierDataId => this.m_lettuceModifierDataId;

  [DbfField("LETTUCE_ABILITY_ID")]
  public int LettuceAbilityId => this.m_lettuceAbilityId;

  [DbfField("ATTACK_CHANGE")]
  public int AttackChange => this.m_attackChange;

  [DbfField("HEALTH_CHANGE")]
  public int HealthChange => this.m_healthChange;

  [DbfField("SPEED_CHANGE")]
  public int SpeedChange => this.m_speedChange;

  [DbfField("COOLDOWN_CHANGE")]
  public int CooldownChange => this.m_cooldownChange;

  [DbfField("SCRIPT_DATA_NUM_1_CHANGE")]
  public int ScriptDataNum1Change => this.m_scriptDataNum1Change;

  [DbfField("SCRIPT_DATA_NUM_2_CHANGE")]
  public int ScriptDataNum2Change => this.m_scriptDataNum2Change;

  public List<ModifiedLettuceAbilityCardTagDbfRecord> Tags
  {
    get
    {
      int id = this.ID;
      List<ModifiedLettuceAbilityCardTagDbfRecord> tags = new List<ModifiedLettuceAbilityCardTagDbfRecord>();
      List<ModifiedLettuceAbilityCardTagDbfRecord> records = GameDbf.ModifiedLettuceAbilityCardTag.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        ModifiedLettuceAbilityCardTagDbfRecord cardTagDbfRecord = records[index];
        if (cardTagDbfRecord.ModifiedLettuceAbilityValueId == id)
          tags.Add(cardTagDbfRecord);
      }
      return tags;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ATTACK_CHANGE":
        return (object) this.m_attackChange;
      case "COOLDOWN_CHANGE":
        return (object) this.m_cooldownChange;
      case "HEALTH_CHANGE":
        return (object) this.m_healthChange;
      case "ID":
        return (object) this.ID;
      case "LETTUCE_ABILITY_ID":
        return (object) this.m_lettuceAbilityId;
      case "LETTUCE_MODIFIER_DATA_ID":
        return (object) this.m_lettuceModifierDataId;
      case "SCRIPT_DATA_NUM_1_CHANGE":
        return (object) this.m_scriptDataNum1Change;
      case "SCRIPT_DATA_NUM_2_CHANGE":
        return (object) this.m_scriptDataNum2Change;
      case "SPEED_CHANGE":
        return (object) this.m_speedChange;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 498841165:
        if (!(name == "COOLDOWN_CHANGE"))
          break;
        this.m_cooldownChange = (int) val;
        break;
      case 1086615145:
        if (!(name == "SCRIPT_DATA_NUM_1_CHANGE"))
          break;
        this.m_scriptDataNum1Change = (int) val;
        break;
      case 1131079828:
        if (!(name == "LETTUCE_ABILITY_ID"))
          break;
        this.m_lettuceAbilityId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2023551732:
        if (!(name == "LETTUCE_MODIFIER_DATA_ID"))
          break;
        this.m_lettuceModifierDataId = (int) val;
        break;
      case 2118910437:
        if (!(name == "SPEED_CHANGE"))
          break;
        this.m_speedChange = (int) val;
        break;
      case 2613208986:
        if (!(name == "SCRIPT_DATA_NUM_2_CHANGE"))
          break;
        this.m_scriptDataNum2Change = (int) val;
        break;
      case 2616708978:
        if (!(name == "ATTACK_CHANGE"))
          break;
        this.m_attackChange = (int) val;
        break;
      case 4075840648:
        if (!(name == "HEALTH_CHANGE"))
          break;
        this.m_healthChange = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ATTACK_CHANGE":
        return typeof (int);
      case "COOLDOWN_CHANGE":
        return typeof (int);
      case "HEALTH_CHANGE":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "LETTUCE_ABILITY_ID":
        return typeof (int);
      case "LETTUCE_MODIFIER_DATA_ID":
        return typeof (int);
      case "SCRIPT_DATA_NUM_1_CHANGE":
        return typeof (int);
      case "SCRIPT_DATA_NUM_2_CHANGE":
        return typeof (int);
      case "SPEED_CHANGE":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadModifiedLettuceAbilityValueDbfRecords loadRecords = new LoadModifiedLettuceAbilityValueDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ModifiedLettuceAbilityValueDbfAsset abilityValueDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ModifiedLettuceAbilityValueDbfAsset)) as ModifiedLettuceAbilityValueDbfAsset;
    if ((UnityEngine.Object) abilityValueDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ModifiedLettuceAbilityValueDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < abilityValueDbfAsset.Records.Count; ++index)
      abilityValueDbfAsset.Records[index].StripUnusedLocales();
    records = abilityValueDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
