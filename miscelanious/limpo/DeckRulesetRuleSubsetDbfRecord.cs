using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckRulesetRuleSubsetDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_deckRulesetRuleId;
  [SerializeField]
  private int m_subsetId;

  [DbfField("DECK_RULESET_RULE_ID")]
  public int DeckRulesetRuleId => this.m_deckRulesetRuleId;

  [DbfField("SUBSET_ID")]
  public int SubsetId => this.m_subsetId;

  public void SetDeckRulesetRuleId(int v) => this.m_deckRulesetRuleId = v;

  public void SetSubsetId(int v) => this.m_subsetId = v;

  public override object GetVar(string name)
  {
    if (name == "DECK_RULESET_RULE_ID")
      return (object) this.m_deckRulesetRuleId;
    return name == "SUBSET_ID" ? (object) this.m_subsetId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "DECK_RULESET_RULE_ID"))
    {
      if (!(name == "SUBSET_ID"))
        return;
      this.m_subsetId = (int) val;
    }
    else
      this.m_deckRulesetRuleId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "DECK_RULESET_RULE_ID")
      return typeof (int);
    return name == "SUBSET_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDeckRulesetRuleSubsetDbfRecords loadRecords = new LoadDeckRulesetRuleSubsetDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DeckRulesetRuleSubsetDbfAsset ruleSubsetDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DeckRulesetRuleSubsetDbfAsset)) as DeckRulesetRuleSubsetDbfAsset;
    if ((UnityEngine.Object) ruleSubsetDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DeckRulesetRuleSubsetDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < ruleSubsetDbfAsset.Records.Count; ++index)
      ruleSubsetDbfAsset.Records[index].StripUnusedLocales();
    records = ruleSubsetDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
