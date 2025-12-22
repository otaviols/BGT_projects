using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMercenaryAbilityDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceMercenarySpecializationId;
  [SerializeField]
  private int m_lettuceAbilityId;
  [SerializeField]
  private int m_lettuceMercenaryLevelIdRequiredId;

  [DbfField("LETTUCE_MERCENARY_SPECIALIZATION_ID")]
  public int LettuceMercenarySpecializationId => this.m_lettuceMercenarySpecializationId;

  [DbfField("LETTUCE_ABILITY_ID")]
  public int LettuceAbilityId => this.m_lettuceAbilityId;

  public LettuceAbilityDbfRecord LettuceAbilityRecord => GameDbf.LettuceAbility.GetRecord(this.m_lettuceAbilityId);

  [DbfField("LETTUCE_MERCENARY_LEVEL_ID_REQUIRED")]
  public int LettuceMercenaryLevelIdRequired => this.m_lettuceMercenaryLevelIdRequiredId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_MERCENARY_SPECIALIZATION_ID")
      return (object) this.m_lettuceMercenarySpecializationId;
    if (name == "LETTUCE_ABILITY_ID")
      return (object) this.m_lettuceAbilityId;
    return name == "LETTUCE_MERCENARY_LEVEL_ID_REQUIRED" ? (object) this.m_lettuceMercenaryLevelIdRequiredId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_MERCENARY_SPECIALIZATION_ID"))
      {
        if (!(name == "LETTUCE_ABILITY_ID"))
        {
          if (!(name == "LETTUCE_MERCENARY_LEVEL_ID_REQUIRED"))
            return;
          this.m_lettuceMercenaryLevelIdRequiredId = (int) val;
        }
        else
          this.m_lettuceAbilityId = (int) val;
      }
      else
        this.m_lettuceMercenarySpecializationId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_MERCENARY_SPECIALIZATION_ID")
      return typeof (int);
    if (name == "LETTUCE_ABILITY_ID")
      return typeof (int);
    return name == "LETTUCE_MERCENARY_LEVEL_ID_REQUIRED" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMercenaryAbilityDbfRecords loadRecords = new LoadLettuceMercenaryAbilityDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMercenaryAbilityDbfAsset mercenaryAbilityDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMercenaryAbilityDbfAsset)) as LettuceMercenaryAbilityDbfAsset;
    if ((UnityEngine.Object) mercenaryAbilityDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMercenaryAbilityDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < mercenaryAbilityDbfAsset.Records.Count; ++index)
      mercenaryAbilityDbfAsset.Records[index].StripUnusedLocales();
    records = mercenaryAbilityDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
