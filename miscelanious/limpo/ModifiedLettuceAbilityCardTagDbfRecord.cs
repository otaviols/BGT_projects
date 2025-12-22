using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifiedLettuceAbilityCardTagDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_modifiedLettuceAbilityValueId;
  [SerializeField]
  private int m_tagId;
  [SerializeField]
  private int m_tagValue;
  [SerializeField]
  private bool m_isReferenceTag;
  [SerializeField]
  private bool m_isPowerKeywordTag;

  [DbfField("MODIFIED_LETTUCE_ABILITY_VALUE_ID")]
  public int ModifiedLettuceAbilityValueId => this.m_modifiedLettuceAbilityValueId;

  [DbfField("TAG_ID")]
  public int TagId => this.m_tagId;

  [DbfField("TAG_VALUE")]
  public int TagValue => this.m_tagValue;

  [DbfField("IS_REFERENCE_TAG")]
  public bool IsReferenceTag => this.m_isReferenceTag;

  [DbfField("IS_POWER_KEYWORD_TAG")]
  public bool IsPowerKeywordTag => this.m_isPowerKeywordTag;

  public override object GetVar(string name)
  {
    if (name == "MODIFIED_LETTUCE_ABILITY_VALUE_ID")
      return (object) this.m_modifiedLettuceAbilityValueId;
    if (name == "TAG_ID")
      return (object) this.m_tagId;
    if (name == "TAG_VALUE")
      return (object) this.m_tagValue;
    if (name == "IS_REFERENCE_TAG")
      return (object) this.m_isReferenceTag;
    return name == "IS_POWER_KEYWORD_TAG" ? (object) this.m_isPowerKeywordTag : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "MODIFIED_LETTUCE_ABILITY_VALUE_ID"))
    {
      if (!(name == "TAG_ID"))
      {
        if (!(name == "TAG_VALUE"))
        {
          if (!(name == "IS_REFERENCE_TAG"))
          {
            if (!(name == "IS_POWER_KEYWORD_TAG"))
              return;
            this.m_isPowerKeywordTag = (bool) val;
          }
          else
            this.m_isReferenceTag = (bool) val;
        }
        else
          this.m_tagValue = (int) val;
      }
      else
        this.m_tagId = (int) val;
    }
    else
      this.m_modifiedLettuceAbilityValueId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "MODIFIED_LETTUCE_ABILITY_VALUE_ID")
      return typeof (int);
    if (name == "TAG_ID")
      return typeof (int);
    if (name == "TAG_VALUE")
      return typeof (int);
    if (name == "IS_REFERENCE_TAG")
      return typeof (bool);
    return name == "IS_POWER_KEYWORD_TAG" ? typeof (bool) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadModifiedLettuceAbilityCardTagDbfRecords loadRecords = new LoadModifiedLettuceAbilityCardTagDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ModifiedLettuceAbilityCardTagDbfAsset abilityCardTagDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ModifiedLettuceAbilityCardTagDbfAsset)) as ModifiedLettuceAbilityCardTagDbfAsset;
    if ((UnityEngine.Object) abilityCardTagDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ModifiedLettuceAbilityCardTagDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < abilityCardTagDbfAsset.Records.Count; ++index)
      abilityCardTagDbfAsset.Records[index].StripUnusedLocales();
    records = abilityCardTagDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
