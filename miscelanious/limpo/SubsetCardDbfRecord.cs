using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubsetCardDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_subsetId;
  [SerializeField]
  private int m_cardId;

  [DbfField("SUBSET_ID")]
  public int SubsetId => this.m_subsetId;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public void SetSubsetId(int v) => this.m_subsetId = v;

  public void SetCardId(int v) => this.m_cardId = v;

  public override object GetVar(string name)
  {
    if (name == "SUBSET_ID")
      return (object) this.m_subsetId;
    return name == "CARD_ID" ? (object) this.m_cardId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "SUBSET_ID"))
    {
      if (!(name == "CARD_ID"))
        return;
      this.m_cardId = (int) val;
    }
    else
      this.m_subsetId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "SUBSET_ID")
      return typeof (int);
    return name == "CARD_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadSubsetCardDbfRecords loadRecords = new LoadSubsetCardDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    SubsetCardDbfAsset subsetCardDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (SubsetCardDbfAsset)) as SubsetCardDbfAsset;
    if ((UnityEngine.Object) subsetCardDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("SubsetCardDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < subsetCardDbfAsset.Records.Count; ++index)
      subsetCardDbfAsset.Records[index].StripUnusedLocales();
    records = subsetCardDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
