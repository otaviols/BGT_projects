using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MiniSetDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_deckId;
  [SerializeField]
  private int m_boosterId;
  [SerializeField]
  private DbfLocValue m_goldenName;
  [SerializeField]
  private bool m_hideOnClient;

  public DeckDbfRecord DeckRecord => GameDbf.Deck.GetRecord(this.m_deckId);

  [DbfField("BOOSTER_ID")]
  public int BoosterId => this.m_boosterId;

  public BoosterDbfRecord BoosterRecord => GameDbf.Booster.GetRecord(this.m_boosterId);

  [DbfField("GOLDEN_NAME")]
  public DbfLocValue GoldenName => this.m_goldenName;

  [DbfField("HIDE_ON_CLIENT")]
  public bool HideOnClient => this.m_hideOnClient;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "DECK_ID")
      return (object) this.m_deckId;
    if (name == "BOOSTER_ID")
      return (object) this.m_boosterId;
    if (name == "GOLDEN_NAME")
      return (object) this.m_goldenName;
    return name == "HIDE_ON_CLIENT" ? (object) this.m_hideOnClient : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "DECK_ID"))
      {
        if (!(name == "BOOSTER_ID"))
        {
          if (!(name == "GOLDEN_NAME"))
          {
            if (!(name == "HIDE_ON_CLIENT"))
              return;
            this.m_hideOnClient = (bool) val;
          }
          else
            this.m_goldenName = (DbfLocValue) val;
        }
        else
          this.m_boosterId = (int) val;
      }
      else
        this.m_deckId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "DECK_ID")
      return typeof (int);
    if (name == "BOOSTER_ID")
      return typeof (int);
    if (name == "GOLDEN_NAME")
      return typeof (DbfLocValue);
    return name == "HIDE_ON_CLIENT" ? typeof (bool) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMiniSetDbfRecords loadRecords = new LoadMiniSetDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MiniSetDbfAsset miniSetDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MiniSetDbfAsset)) as MiniSetDbfAsset;
    if ((UnityEngine.Object) miniSetDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MiniSetDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < miniSetDbfAsset.Records.Count; ++index)
      miniSetDbfAsset.Records[index].StripUnusedLocales();
    records = miniSetDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_goldenName.StripUnusedLocales();
}
