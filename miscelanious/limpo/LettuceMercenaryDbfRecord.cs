using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMercenaryDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_rarityId;
  [SerializeField]
  private bool m_collectible;
  [SerializeField]
  private bool m_craftable = true;
  [SerializeField]
  private Assets.LettuceMercenary.Acquiretype m_acquireType;
  [SerializeField]
  private DbfLocValue m_howToAcquireText;
  [SerializeField]
  private int m_coinCraftCost = 50;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("RARITY")]
  public int Rarity => this.m_rarityId;

  [DbfField("COLLECTIBLE")]
  public bool Collectible => this.m_collectible;

  [DbfField("CRAFTABLE")]
  public bool Craftable => this.m_craftable;

  [DbfField("ACQUIRE_TYPE")]
  public Assets.LettuceMercenary.Acquiretype AcquireType => this.m_acquireType;

  [DbfField("HOW_TO_ACQUIRE_TEXT")]
  public DbfLocValue HowToAcquireText => this.m_howToAcquireText;

  [DbfField("COIN_CRAFT_COST")]
  public int CoinCraftCost => this.m_coinCraftCost;

  public List<LettuceMercenaryEquipmentDbfRecord> LettuceMercenaryEquipment
  {
    get
    {
      int id = this.ID;
      List<LettuceMercenaryEquipmentDbfRecord> mercenaryEquipment = new List<LettuceMercenaryEquipmentDbfRecord>();
      List<LettuceMercenaryEquipmentDbfRecord> records = GameDbf.LettuceMercenaryEquipment.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LettuceMercenaryEquipmentDbfRecord equipmentDbfRecord = records[index];
        if (equipmentDbfRecord.LettuceMercenaryId == id)
          mercenaryEquipment.Add(equipmentDbfRecord);
      }
      return mercenaryEquipment;
    }
  }

  public List<LettuceMercenarySpecializationDbfRecord> LettuceMercenarySpecializations
  {
    get
    {
      int id = this.ID;
      List<LettuceMercenarySpecializationDbfRecord> mercenarySpecializations = new List<LettuceMercenarySpecializationDbfRecord>();
      List<LettuceMercenarySpecializationDbfRecord> records = GameDbf.LettuceMercenarySpecialization.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LettuceMercenarySpecializationDbfRecord specializationDbfRecord = records[index];
        if (specializationDbfRecord.LettuceMercenaryId == id)
          mercenarySpecializations.Add(specializationDbfRecord);
      }
      return mercenarySpecializations;
    }
  }

  public List<MercenaryArtVariationDbfRecord> MercenaryArtVariations
  {
    get
    {
      int id = this.ID;
      List<MercenaryArtVariationDbfRecord> mercenaryArtVariations = new List<MercenaryArtVariationDbfRecord>();
      List<MercenaryArtVariationDbfRecord> records = GameDbf.MercenaryArtVariation.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        MercenaryArtVariationDbfRecord variationDbfRecord = records[index];
        if (variationDbfRecord.LettuceMercenaryId == id)
          mercenaryArtVariations.Add(variationDbfRecord);
      }
      return mercenaryArtVariations;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACQUIRE_TYPE":
        return (object) this.m_acquireType;
      case "COIN_CRAFT_COST":
        return (object) this.m_coinCraftCost;
      case "COLLECTIBLE":
        return (object) this.m_collectible;
      case "CRAFTABLE":
        return (object) this.m_craftable;
      case "HOW_TO_ACQUIRE_TEXT":
        return (object) this.m_howToAcquireText;
      case "ID":
        return (object) this.ID;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "RARITY":
        return (object) this.m_rarityId;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 124892663:
        if (!(name == "COIN_CRAFT_COST"))
          break;
        this.m_coinCraftCost = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2508960993:
        if (!(name == "CRAFTABLE"))
          break;
        this.m_craftable = (bool) val;
        break;
      case 2610547710:
        if (!(name == "HOW_TO_ACQUIRE_TEXT"))
          break;
        this.m_howToAcquireText = (DbfLocValue) val;
        break;
      case 2777904777:
        if (!(name == "COLLECTIBLE"))
          break;
        this.m_collectible = (bool) val;
        break;
      case 2975427914:
        if (!(name == "RARITY"))
          break;
        this.m_rarityId = (int) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 4001942604:
        if (!(name == "ACQUIRE_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_acquireType = Assets.LettuceMercenary.Acquiretype.NONE;
            return;
          case Assets.LettuceMercenary.Acquiretype _:
          case int _:
            this.m_acquireType = (Assets.LettuceMercenary.Acquiretype) val;
            return;
          case string _:
            this.m_acquireType = Assets.LettuceMercenary.ParseAcquiretypeValue((string) val);
            return;
          default:
            return;
        }
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACQUIRE_TYPE":
        return typeof (Assets.LettuceMercenary.Acquiretype);
      case "COIN_CRAFT_COST":
        return typeof (int);
      case "COLLECTIBLE":
        return typeof (bool);
      case "CRAFTABLE":
        return typeof (bool);
      case "HOW_TO_ACQUIRE_TEXT":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "RARITY":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMercenaryDbfRecords loadRecords = new LoadLettuceMercenaryDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMercenaryDbfAsset mercenaryDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMercenaryDbfAsset)) as LettuceMercenaryDbfAsset;
    if ((UnityEngine.Object) mercenaryDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMercenaryDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < mercenaryDbfAsset.Records.Count; ++index)
      mercenaryDbfAsset.Records[index].StripUnusedLocales();
    records = mercenaryDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_howToAcquireText.StripUnusedLocales();
}
