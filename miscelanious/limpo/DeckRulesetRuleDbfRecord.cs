using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckRulesetRuleDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_deckRulesetId;
  [SerializeField]
  private int m_appliesToSubsetId;
  [SerializeField]
  private bool m_appliesToIsNot;
  [SerializeField]
  private DeckRulesetRule.RuleType m_ruleType = DeckRulesetRule.ParseRuleTypeValue("invalid_rule_type");
  [SerializeField]
  private bool m_ruleIsNot;
  [SerializeField]
  private int m_minValue;
  [SerializeField]
  private int m_maxValue;
  [SerializeField]
  private int m_tagId;
  [SerializeField]
  private int m_tagMinValue;
  [SerializeField]
  private int m_tagMaxValue;
  [SerializeField]
  private string m_stringValue;
  [SerializeField]
  private DbfLocValue m_errorString;
  [SerializeField]
  private bool m_showInvalidCards;

  [DbfField("DECK_RULESET_ID")]
  public int DeckRulesetId => this.m_deckRulesetId;

  [DbfField("APPLIES_TO_SUBSET_ID")]
  public int AppliesToSubsetId => this.m_appliesToSubsetId;

  [DbfField("APPLIES_TO_IS_NOT")]
  public bool AppliesToIsNot => this.m_appliesToIsNot;

  [DbfField("RULE_TYPE")]
  public DeckRulesetRule.RuleType RuleType => this.m_ruleType;

  [DbfField("RULE_IS_NOT")]
  public bool RuleIsNot => this.m_ruleIsNot;

  [DbfField("MIN_VALUE")]
  public int MinValue => this.m_minValue;

  [DbfField("MAX_VALUE")]
  public int MaxValue => this.m_maxValue;

  [DbfField("TAG")]
  public int Tag => this.m_tagId;

  [DbfField("TAG_MIN_VALUE")]
  public int TagMinValue => this.m_tagMinValue;

  [DbfField("TAG_MAX_VALUE")]
  public int TagMaxValue => this.m_tagMaxValue;

  [DbfField("STRING_VALUE")]
  public string StringValue => this.m_stringValue;

  [DbfField("ERROR_STRING")]
  public DbfLocValue ErrorString => this.m_errorString;

  [DbfField("SHOW_INVALID_CARDS")]
  public bool ShowInvalidCards => this.m_showInvalidCards;

  public void SetDeckRulesetId(int v) => this.m_deckRulesetId = v;

  public void SetAppliesToSubsetId(int v) => this.m_appliesToSubsetId = v;

  public void SetAppliesToIsNot(bool v) => this.m_appliesToIsNot = v;

  public void SetRuleType(DeckRulesetRule.RuleType v) => this.m_ruleType = v;

  public void SetRuleIsNot(bool v) => this.m_ruleIsNot = v;

  public void SetMinValue(int v) => this.m_minValue = v;

  public void SetMaxValue(int v) => this.m_maxValue = v;

  public void SetTag(int v) => this.m_tagId = v;

  public void SetTagMinValue(int v) => this.m_tagMinValue = v;

  public void SetTagMaxValue(int v) => this.m_tagMaxValue = v;

  public void SetStringValue(string v) => this.m_stringValue = v;

  public void SetShowInvalidCards(bool v) => this.m_showInvalidCards = v;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "APPLIES_TO_IS_NOT":
        return (object) this.m_appliesToIsNot;
      case "APPLIES_TO_SUBSET_ID":
        return (object) this.m_appliesToSubsetId;
      case "DECK_RULESET_ID":
        return (object) this.m_deckRulesetId;
      case "ERROR_STRING":
        return (object) this.m_errorString;
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
      case "SHOW_INVALID_CARDS":
        return (object) this.m_showInvalidCards;
      case "STRING_VALUE":
        return (object) this.m_stringValue;
      case "TAG":
        return (object) this.m_tagId;
      case "TAG_MAX_VALUE":
        return (object) this.m_tagMaxValue;
      case "TAG_MIN_VALUE":
        return (object) this.m_tagMinValue;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 349684874:
        if (!(name == "STRING_VALUE"))
          break;
        this.m_stringValue = (string) val;
        break;
      case 406049971:
        if (!(name == "TAG"))
          break;
        this.m_tagId = (int) val;
        break;
      case 451320876:
        if (!(name == "TAG_MAX_VALUE"))
          break;
        this.m_tagMaxValue = (int) val;
        break;
      case 463699011:
        if (!(name == "ERROR_STRING"))
          break;
        this.m_errorString = (DbfLocValue) val;
        break;
      case 730241252:
        if (!(name == "APPLIES_TO_SUBSET_ID"))
          break;
        this.m_appliesToSubsetId = (int) val;
        break;
      case 779072232:
        if (!(name == "APPLIES_TO_IS_NOT"))
          break;
        this.m_appliesToIsNot = (bool) val;
        break;
      case 1144720106:
        if (!(name == "TAG_MIN_VALUE"))
          break;
        this.m_tagMinValue = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 3306344503:
        if (!(name == "DECK_RULESET_ID"))
          break;
        this.m_deckRulesetId = (int) val;
        break;
      case 3419090408:
        if (!(name == "SHOW_INVALID_CARDS"))
          break;
        this.m_showInvalidCards = (bool) val;
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
            this.m_ruleType = DeckRulesetRule.RuleType.INVALID_RULE_TYPE;
            return;
          case DeckRulesetRule.RuleType _:
          case int _:
            this.m_ruleType = (DeckRulesetRule.RuleType) val;
            return;
          case string _:
            this.m_ruleType = DeckRulesetRule.ParseRuleTypeValue((string) val);
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
      case "APPLIES_TO_IS_NOT":
        return typeof (bool);
      case "APPLIES_TO_SUBSET_ID":
        return typeof (int);
      case "DECK_RULESET_ID":
        return typeof (int);
      case "ERROR_STRING":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "MAX_VALUE":
        return typeof (int);
      case "MIN_VALUE":
        return typeof (int);
      case "RULE_IS_NOT":
        return typeof (bool);
      case "RULE_TYPE":
        return typeof (DeckRulesetRule.RuleType);
      case "SHOW_INVALID_CARDS":
        return typeof (bool);
      case "STRING_VALUE":
        return typeof (string);
      case "TAG":
        return typeof (int);
      case "TAG_MAX_VALUE":
        return typeof (int);
      case "TAG_MIN_VALUE":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDeckRulesetRuleDbfRecords loadRecords = new LoadDeckRulesetRuleDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DeckRulesetRuleDbfAsset rulesetRuleDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DeckRulesetRuleDbfAsset)) as DeckRulesetRuleDbfAsset;
    if ((UnityEngine.Object) rulesetRuleDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DeckRulesetRuleDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rulesetRuleDbfAsset.Records.Count; ++index)
      rulesetRuleDbfAsset.Records[index].StripUnusedLocales();
    records = rulesetRuleDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_errorString.StripUnusedLocales();
}
