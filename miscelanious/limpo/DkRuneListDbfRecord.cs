using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DkRuneListDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_deckTemplateId;
  [SerializeField]
  private DkRuneList.DkruneTypes m_rune;

  [DbfField("DECK_TEMPLATE_ID")]
  public int DeckTemplateId => this.m_deckTemplateId;

  [DbfField("RUNE")]
  public DkRuneList.DkruneTypes Rune => this.m_rune;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "DECK_TEMPLATE_ID")
      return (object) this.m_deckTemplateId;
    return name == "RUNE" ? (object) this.m_rune : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "DECK_TEMPLATE_ID"))
      {
        if (!(name == "RUNE"))
          return;
        switch (val)
        {
          case null:
            this.m_rune = DkRuneList.DkruneTypes.NONERUNE;
            break;
          case DkRuneList.DkruneTypes _:
          case int _:
            this.m_rune = (DkRuneList.DkruneTypes) val;
            break;
          case string _:
            this.m_rune = DkRuneList.ParseDkruneTypesValue((string) val);
            break;
        }
      }
      else
        this.m_deckTemplateId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "DECK_TEMPLATE_ID")
      return typeof (int);
    return name == "RUNE" ? typeof (DkRuneList.DkruneTypes) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDkRuneListDbfRecords loadRecords = new LoadDkRuneListDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DkRuneListDbfAsset runeListDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DkRuneListDbfAsset)) as DkRuneListDbfAsset;
    if ((UnityEngine.Object) runeListDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DkRuneListDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < runeListDbfAsset.Records.Count; ++index)
      runeListDbfAsset.Records[index].StripUnusedLocales();
    records = runeListDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
