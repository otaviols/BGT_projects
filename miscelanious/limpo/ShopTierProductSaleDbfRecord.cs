using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopTierProductSaleDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_tierId;
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_slotIndex = -1;
  [SerializeField]
  private int m_pmtProductId;
  [SerializeField]
  private string m_event = "always";

  public override object GetVar(string name)
  {
    if (name == "TIER_ID")
      return (object) this.m_tierId;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    if (name == "SLOT_INDEX")
      return (object) this.m_slotIndex;
    if (name == "PMT_PRODUCT_ID")
      return (object) this.m_pmtProductId;
    return name == "EVENT" ? (object) this.m_event : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "TIER_ID"))
    {
      if (!(name == "NOTE_DESC"))
      {
        if (!(name == "SLOT_INDEX"))
        {
          if (!(name == "PMT_PRODUCT_ID"))
          {
            if (!(name == "EVENT"))
              return;
            this.m_event = (string) val;
          }
          else
            this.m_pmtProductId = (int) val;
        }
        else
          this.m_slotIndex = (int) val;
      }
      else
        this.m_noteDesc = (string) val;
    }
    else
      this.m_tierId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "TIER_ID")
      return typeof (int);
    if (name == "NOTE_DESC")
      return typeof (string);
    if (name == "SLOT_INDEX")
      return typeof (int);
    if (name == "PMT_PRODUCT_ID")
      return typeof (int);
    return name == "EVENT" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadShopTierProductSaleDbfRecords loadRecords = new LoadShopTierProductSaleDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ShopTierProductSaleDbfAsset productSaleDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ShopTierProductSaleDbfAsset)) as ShopTierProductSaleDbfAsset;
    if ((UnityEngine.Object) productSaleDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ShopTierProductSaleDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < productSaleDbfAsset.Records.Count; ++index)
      productSaleDbfAsset.Records[index].StripUnusedLocales();
    records = productSaleDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
