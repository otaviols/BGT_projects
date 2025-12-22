using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceEquipmentTierDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceEquipmentId;
  [SerializeField]
  private int m_tier = 1;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_coinCraftCost = 100;
  [SerializeField]
  private bool m_showTextOnMerc;

  [DbfField("LETTUCE_EQUIPMENT_ID")]
  public int LettuceEquipmentId => this.m_lettuceEquipmentId;

  [DbfField("TIER")]
  public int Tier => this.m_tier;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public CardDbfRecord CardRecord => GameDbf.Card.GetRecord(this.m_cardId);

  [DbfField("COIN_CRAFT_COST")]
  public int CoinCraftCost => this.m_coinCraftCost;

  [DbfField("SHOW_TEXT_ON_MERC")]
  public bool ShowTextOnMerc => this.m_showTextOnMerc;

  public LettuceEquipmentModifierDataDbfRecord EquipmentModifierData
  {
    get
    {
      int id = this.ID;
      List<LettuceEquipmentModifierDataDbfRecord> records = GameDbf.LettuceEquipmentModifierData.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LettuceEquipmentModifierDataDbfRecord equipmentModifierData = records[index];
        if (equipmentModifierData.LettuceEquipmentTierId == id)
          return equipmentModifierData;
      }
      return (LettuceEquipmentModifierDataDbfRecord) null;
    }
  }

  public List<BonusBountyDropChanceDbfRecord> BonusBountyDropChances
  {
    get
    {
      int id = this.ID;
      List<BonusBountyDropChanceDbfRecord> bountyDropChances = new List<BonusBountyDropChanceDbfRecord>();
      List<BonusBountyDropChanceDbfRecord> records = GameDbf.BonusBountyDropChance.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        BonusBountyDropChanceDbfRecord dropChanceDbfRecord = records[index];
        if (dropChanceDbfRecord.LettuceEquipmentTierId == id)
          bountyDropChances.Add(dropChanceDbfRecord);
      }
      return bountyDropChances;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_EQUIPMENT_ID")
      return (object) this.m_lettuceEquipmentId;
    if (name == "TIER")
      return (object) this.m_tier;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "COIN_CRAFT_COST")
      return (object) this.m_coinCraftCost;
    return name == "SHOW_TEXT_ON_MERC" ? (object) this.m_showTextOnMerc : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_EQUIPMENT_ID"))
      {
        if (!(name == "TIER"))
        {
          if (!(name == "CARD_ID"))
          {
            if (!(name == "COIN_CRAFT_COST"))
            {
              if (!(name == "SHOW_TEXT_ON_MERC"))
                return;
              this.m_showTextOnMerc = (bool) val;
            }
            else
              this.m_coinCraftCost = (int) val;
          }
          else
            this.m_cardId = (int) val;
        }
        else
          this.m_tier = (int) val;
      }
      else
        this.m_lettuceEquipmentId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_EQUIPMENT_ID")
      return typeof (int);
    if (name == "TIER")
      return typeof (int);
    if (name == "CARD_ID")
      return typeof (int);
    if (name == "COIN_CRAFT_COST")
      return typeof (int);
    return name == "SHOW_TEXT_ON_MERC" ? typeof (bool) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceEquipmentTierDbfRecords loadRecords = new LoadLettuceEquipmentTierDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceEquipmentTierDbfAsset equipmentTierDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceEquipmentTierDbfAsset)) as LettuceEquipmentTierDbfAsset;
    if ((UnityEngine.Object) equipmentTierDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceEquipmentTierDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < equipmentTierDbfAsset.Records.Count; ++index)
      equipmentTierDbfAsset.Records[index].StripUnusedLocales();
    records = equipmentTierDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
