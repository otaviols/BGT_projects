using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardEquipmentAltTextDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_equipmentCardId;
  [SerializeField]
  private int m_altTextIndex;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public CardDbfRecord EquipmentCardRecord => GameDbf.Card.GetRecord(this.m_equipmentCardId);

  [DbfField("ALT_TEXT_INDEX")]
  public int AltTextIndex => this.m_altTextIndex;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "EQUIPMENT_CARD_ID")
      return (object) this.m_equipmentCardId;
    return name == "ALT_TEXT_INDEX" ? (object) this.m_altTextIndex : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "CARD_ID"))
      {
        if (!(name == "EQUIPMENT_CARD_ID"))
        {
          if (!(name == "ALT_TEXT_INDEX"))
            return;
          this.m_altTextIndex = (int) val;
        }
        else
          this.m_equipmentCardId = (int) val;
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
    if (name == "EQUIPMENT_CARD_ID")
      return typeof (int);
    return name == "ALT_TEXT_INDEX" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardEquipmentAltTextDbfRecords loadRecords = new LoadCardEquipmentAltTextDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardEquipmentAltTextDbfAsset equipmentAltTextDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardEquipmentAltTextDbfAsset)) as CardEquipmentAltTextDbfAsset;
    if ((UnityEngine.Object) equipmentAltTextDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardEquipmentAltTextDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < equipmentAltTextDbfAsset.Records.Count; ++index)
      equipmentAltTextDbfAsset.Records[index].StripUnusedLocales();
    records = equipmentAltTextDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
