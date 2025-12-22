using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardAdditonalSearchTermsDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private DbfLocValue m_searchTerm;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  [DbfField("SEARCH_TERM")]
  public DbfLocValue SearchTerm => this.m_searchTerm;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    return name == "SEARCH_TERM" ? (object) this.m_searchTerm : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "CARD_ID"))
      {
        if (!(name == "SEARCH_TERM"))
          return;
        this.m_searchTerm = (DbfLocValue) val;
      }
      else
        this.m_cardId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "CARD_ID")
      return typeof (int);
    return name == "SEARCH_TERM" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardAdditonalSearchTermsDbfRecords loadRecords = new LoadCardAdditonalSearchTermsDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardAdditonalSearchTermsDbfAsset searchTermsDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardAdditonalSearchTermsDbfAsset)) as CardAdditonalSearchTermsDbfAsset;
    if ((UnityEngine.Object) searchTermsDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardAdditonalSearchTermsDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < searchTermsDbfAsset.Records.Count; ++index)
      searchTermsDbfAsset.Records[index].StripUnusedLocales();
    records = searchTermsDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_searchTerm.StripUnusedLocales();
}
