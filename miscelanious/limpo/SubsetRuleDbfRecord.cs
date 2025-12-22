using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubsetRuleDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_subsetId;
  [SerializeField]
  private SubsetRule.Type m_ruleType = SubsetRule.ParseTypeValue("invalid");
  [SerializeField]
  private bool m_ruleIsNot;
  [SerializeField]
  private int m_tagId;
  [SerializeField]
  private int m_minValue;
  [SerializeField]
  private int m_maxValue;

  [DbfField("SUBSET_ID")]
  public int SubsetId => this.m_subsetId;

  [DbfField("RULE_IS_NOT")]
  public bool RuleIsNot => this.m_ruleIsNot;

  [DbfField("TAG")]
  public int Tag => this.m_tagId;

  [DbfField("MIN_VALUE")]
  public int MinValue => this.m_minValue;

  [DbfField("MAX_VALUE")]
  public int MaxValue => this.m_maxValue;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ID":
        return (object) this.ID;
      case "MAX_VALUE":
        return (object) this.m_maxValue;
      case "MIN_VALUE":
        return (object) this.m_minValue;
      case "RULE_IS_NOT":
        return (object) this.m_ruleIsNot;
      case "RULE_TYPE":
        return (object) this.m_ruleType;
      case "SUBSET_ID":
        return (object) this.m_subsetId;
      case "TAG":
        return (object) this.m_tagId;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 406049971:
        if (!(name == "TAG"))
          break;
        this.m_tagId = (int) val;
        break;
      case 699650505:
        if (!(name == "SUBSET_ID"))
          break;
        this.m_subsetId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 3868906878:
        if (!(name == "RULE_IS_NOT"))
          break;
        this.m_ruleIsNot = (bool) val;
        break;
      case 3917898540:
        if (!(name == "RULE_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_ruleType = SubsetRule.Type.INVALID;
            return;
          case SubsetRule.Type _:
          case int _:
            this.m_ruleType = (SubsetRule.Type) val;
            return;
          case string _:
            this.m_ruleType = SubsetRule.ParseTypeValue((string) val);
            return;
          default:
            return;
        }
      case 3988915625:
        if (!(name == "MIN_VALUE"))
          break;
        this.m_minValue = (int) val;
        break;
      case 4234312411:
        if (!(name == "MAX_VALUE"))
          break;
        this.m_maxValue = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ID":
        return typeof (int);
      case "MAX_VALUE":
        return typeof (int);
      case "MIN_VALUE":
        return typeof (int);
      case "RULE_IS_NOT":
        return typeof (bool);
      case "RULE_TYPE":
        return typeof (SubsetRule.Type);
      case "SUBSET_ID":
        return typeof (int);
      case "TAG":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadSubsetRuleDbfRecords loadRecords = new LoadSubsetRuleDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    SubsetRuleDbfAsset subsetRuleDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (SubsetRuleDbfAsset)) as SubsetRuleDbfAsset;
    if ((UnityEngine.Object) subsetRuleDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("SubsetRuleDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < subsetRuleDbfAsset.Records.Count; ++index)
      subsetRuleDbfAsset.Records[index].StripUnusedLocales();
    records = subsetRuleDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
