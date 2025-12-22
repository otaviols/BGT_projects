using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MercenaryArtVariationDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceMercenaryId;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private bool m_defaultVariation;

  [DbfField("LETTUCE_MERCENARY_ID")]
  public int LettuceMercenaryId => this.m_lettuceMercenaryId;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public CardDbfRecord CardRecord => GameDbf.Card.GetRecord(this.m_cardId);

  [DbfField("DEFAULT_VARIATION")]
  public bool DefaultVariation => this.m_defaultVariation;

  public List<MercenaryArtVariationPremiumDbfRecord> MercenaryArtVariationPremiums
  {
    get
    {
      int id = this.ID;
      List<MercenaryArtVariationPremiumDbfRecord> variationPremiums = new List<MercenaryArtVariationPremiumDbfRecord>();
      List<MercenaryArtVariationPremiumDbfRecord> records = GameDbf.MercenaryArtVariationPremium.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        MercenaryArtVariationPremiumDbfRecord premiumDbfRecord = records[index];
        if (premiumDbfRecord.MercenaryArtVariationId == id)
          variationPremiums.Add(premiumDbfRecord);
      }
      return variationPremiums;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_MERCENARY_ID")
      return (object) this.m_lettuceMercenaryId;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    return name == "DEFAULT_VARIATION" ? (object) this.m_defaultVariation : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_MERCENARY_ID"))
      {
        if (!(name == "CARD_ID"))
        {
          if (!(name == "DEFAULT_VARIATION"))
            return;
          this.m_defaultVariation = (bool) val;
        }
        else
          this.m_cardId = (int) val;
      }
      else
        this.m_lettuceMercenaryId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_MERCENARY_ID")
      return typeof (int);
    if (name == "CARD_ID")
      return typeof (int);
    return name == "DEFAULT_VARIATION" ? typeof (bool) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMercenaryArtVariationDbfRecords loadRecords = new LoadMercenaryArtVariationDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MercenaryArtVariationDbfAsset variationDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MercenaryArtVariationDbfAsset)) as MercenaryArtVariationDbfAsset;
    if ((UnityEngine.Object) variationDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MercenaryArtVariationDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < variationDbfAsset.Records.Count; ++index)
      variationDbfAsset.Records[index].StripUnusedLocales();
    records = variationDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
