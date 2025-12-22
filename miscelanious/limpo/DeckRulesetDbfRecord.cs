using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckRulesetDbfRecord : DbfRecord
{
  [SerializeField]
  private Assets.DeckRuleset.AssetFlags m_assetFlags = Assets.DeckRuleset.AssetFlags.NOT_PACKAGED_IN_CLIENT;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "ASSET_FLAGS" ? (object) this.m_assetFlags : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ASSET_FLAGS"))
        return;
      switch (val)
      {
        case null:
          this.m_assetFlags = Assets.DeckRuleset.AssetFlags.NONE;
          break;
        case Assets.DeckRuleset.AssetFlags _:
        case int _:
          this.m_assetFlags = (Assets.DeckRuleset.AssetFlags) val;
          break;
        case string _:
          this.m_assetFlags = Assets.DeckRuleset.ParseAssetFlagsValue((string) val);
          break;
      }
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "ASSET_FLAGS" ? typeof (Assets.DeckRuleset.AssetFlags) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDeckRulesetDbfRecords loadRecords = new LoadDeckRulesetDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DeckRulesetDbfAsset deckRulesetDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DeckRulesetDbfAsset)) as DeckRulesetDbfAsset;
    if ((UnityEngine.Object) deckRulesetDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DeckRulesetDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < deckRulesetDbfAsset.Records.Count; ++index)
      deckRulesetDbfAsset.Records[index].StripUnusedLocales();
    records = deckRulesetDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
