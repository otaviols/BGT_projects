using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SellableDeckDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_deckTemplateId;
  [SerializeField]
  private int m_boosterId;
  [SerializeField]
  private DbfLocValue m_goldenName;

  public DeckTemplateDbfRecord DeckTemplateRecord => GameDbf.DeckTemplate.GetRecord(this.m_deckTemplateId);

  [DbfField("BOOSTER_ID")]
  public int BoosterId => this.m_boosterId;

  public BoosterDbfRecord BoosterRecord => GameDbf.Booster.GetRecord(this.m_boosterId);

  [DbfField("GOLDEN_NAME")]
  public DbfLocValue GoldenName => this.m_goldenName;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "DECK_TEMPLATE_ID")
      return (object) this.m_deckTemplateId;
    if (name == "BOOSTER_ID")
      return (object) this.m_boosterId;
    return name == "GOLDEN_NAME" ? (object) this.m_goldenName : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "DECK_TEMPLATE_ID"))
      {
        if (!(name == "BOOSTER_ID"))
        {
          if (!(name == "GOLDEN_NAME"))
            return;
          this.m_goldenName = (DbfLocValue) val;
        }
        else
          this.m_boosterId = (int) val;
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
    if (name == "BOOSTER_ID")
      return typeof (int);
    return name == "GOLDEN_NAME" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadSellableDeckDbfRecords loadRecords = new LoadSellableDeckDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    SellableDeckDbfAsset sellableDeckDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (SellableDeckDbfAsset)) as SellableDeckDbfAsset;
    if ((UnityEngine.Object) sellableDeckDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("SellableDeckDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < sellableDeckDbfAsset.Records.Count; ++index)
      sellableDeckDbfAsset.Records[index].StripUnusedLocales();
    records = sellableDeckDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_goldenName.StripUnusedLocales();
}
