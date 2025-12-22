using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoosterCardSetDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_subsetId;
  [SerializeField]
  private int m_cardSetId;
  [SerializeField]
  private string m_watermarkTextureOverride;

  [DbfField("SUBSET_ID")]
  public int SubsetId => this.m_subsetId;

  public SubsetDbfRecord SubsetRecord => GameDbf.Subset.GetRecord(this.m_subsetId);

  public CardSetDbfRecord CardSetRecord => GameDbf.CardSet.GetRecord(this.m_cardSetId);

  [DbfField("WATERMARK_TEXTURE_OVERRIDE")]
  public string WatermarkTextureOverride => this.m_watermarkTextureOverride;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "SUBSET_ID")
      return (object) this.m_subsetId;
    if (name == "CARD_SET_ID")
      return (object) this.m_cardSetId;
    return name == "WATERMARK_TEXTURE_OVERRIDE" ? (object) this.m_watermarkTextureOverride : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "SUBSET_ID"))
      {
        if (!(name == "CARD_SET_ID"))
        {
          if (!(name == "WATERMARK_TEXTURE_OVERRIDE"))
            return;
          this.m_watermarkTextureOverride = (string) val;
        }
        else
          this.m_cardSetId = (int) val;
      }
      else
        this.m_subsetId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "SUBSET_ID")
      return typeof (int);
    if (name == "CARD_SET_ID")
      return typeof (int);
    return name == "WATERMARK_TEXTURE_OVERRIDE" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBoosterCardSetDbfRecords loadRecords = new LoadBoosterCardSetDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BoosterCardSetDbfAsset boosterCardSetDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BoosterCardSetDbfAsset)) as BoosterCardSetDbfAsset;
    if ((UnityEngine.Object) boosterCardSetDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BoosterCardSetDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < boosterCardSetDbfAsset.Records.Count; ++index)
      boosterCardSetDbfAsset.Records[index].StripUnusedLocales();
    records = boosterCardSetDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
