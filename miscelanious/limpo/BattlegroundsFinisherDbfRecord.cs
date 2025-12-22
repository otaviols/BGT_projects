using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattlegroundsFinisherDbfRecord : DbfRecord
{
  [SerializeField]
  private bool m_enabled = true;
  [SerializeField]
  private int m_rarityId = 2;
  [SerializeField]
  private DbfLocValue m_collectionName;
  [SerializeField]
  private DbfLocValue m_collectionShortName;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private string m_gameplaySettings;
  [SerializeField]
  private string m_destroyOpponentPrefab;
  [SerializeField]
  private string m_destroyOpponentVictoryPrefab;
  [SerializeField]
  private string m_detailsTexture;
  [SerializeField]
  private string m_detailsMovie;
  [SerializeField]
  private string m_detailsRenderConfig;
  [SerializeField]
  private string m_miniBodyMaterial;
  [SerializeField]
  private string m_miniArtMaterial;
  [SerializeField]
  private bool m_isDefault;
  [SerializeField]
  private BattlegroundsFinisher.CapsuleType m_capsuleType;

  [DbfField("RARITY")]
  public int Rarity => this.m_rarityId;

  [DbfField("COLLECTION_NAME")]
  public DbfLocValue CollectionName => this.m_collectionName;

  [DbfField("COLLECTION_SHORT_NAME")]
  public DbfLocValue CollectionShortName => this.m_collectionShortName;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("GAMEPLAY_SETTINGS")]
  public string GameplaySettings => this.m_gameplaySettings;

  [DbfField("DETAILS_TEXTURE")]
  public string DetailsTexture => this.m_detailsTexture;

  [DbfField("DETAILS_MOVIE")]
  public string DetailsMovie => this.m_detailsMovie;

  [DbfField("MINI_BODY_MATERIAL")]
  public string MiniBodyMaterial => this.m_miniBodyMaterial;

  [DbfField("MINI_ART_MATERIAL")]
  public string MiniArtMaterial => this.m_miniArtMaterial;

  [DbfField("CAPSULE_TYPE")]
  public BattlegroundsFinisher.CapsuleType CapsuleType => this.m_capsuleType;

  public List<DetailsVideoCueDbfRecord> VideoCues
  {
    get
    {
      int id = this.ID;
      List<DetailsVideoCueDbfRecord> videoCues = new List<DetailsVideoCueDbfRecord>();
      List<DetailsVideoCueDbfRecord> records = GameDbf.DetailsVideoCue.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        DetailsVideoCueDbfRecord videoCueDbfRecord = records[index];
        if (videoCueDbfRecord.BattlegroundsFinisherId == id)
          videoCues.Add(videoCueDbfRecord);
      }
      return videoCues;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "CAPSULE_TYPE":
        return (object) this.m_capsuleType;
      case "COLLECTION_NAME":
        return (object) this.m_collectionName;
      case "COLLECTION_SHORT_NAME":
        return (object) this.m_collectionShortName;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "DESTROY_OPPONENT_PREFAB":
        return (object) this.m_destroyOpponentPrefab;
      case "DESTROY_OPPONENT_VICTORY_PREFAB":
        return (object) this.m_destroyOpponentVictoryPrefab;
      case "DETAILS_MOVIE":
        return (object) this.m_detailsMovie;
      case "DETAILS_RENDER_CONFIG":
        return (object) this.m_detailsRenderConfig;
      case "DETAILS_TEXTURE":
        return (object) this.m_detailsTexture;
      case "ENABLED":
        return (object) this.m_enabled;
      case "GAMEPLAY_SETTINGS":
        return (object) this.m_gameplaySettings;
      case "ID":
        return (object) this.ID;
      case "IS_DEFAULT":
        return (object) this.m_isDefault;
      case "MINI_ART_MATERIAL":
        return (object) this.m_miniArtMaterial;
      case "MINI_BODY_MATERIAL":
        return (object) this.m_miniBodyMaterial;
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
      case 266660617:
        if (!(name == "DESTROY_OPPONENT_VICTORY_PREFAB"))
          break;
        this.m_destroyOpponentVictoryPrefab = (string) val;
        break;
      case 337942481:
        if (!(name == "DETAILS_TEXTURE"))
          break;
        this.m_detailsTexture = (string) val;
        break;
      case 587332102:
        if (!(name == "DESTROY_OPPONENT_PREFAB"))
          break;
        this.m_destroyOpponentPrefab = (string) val;
        break;
      case 691244065:
        if (!(name == "CAPSULE_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_capsuleType = BattlegroundsFinisher.CapsuleType.DEFAULT;
            return;
          case BattlegroundsFinisher.CapsuleType _:
          case int _:
            this.m_capsuleType = (BattlegroundsFinisher.CapsuleType) val;
            return;
          case string _:
            this.m_capsuleType = BattlegroundsFinisher.ParseCapsuleTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1312785025:
        if (!(name == "MINI_BODY_MATERIAL"))
          break;
        this.m_miniBodyMaterial = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2232832171:
        if (!(name == "GAMEPLAY_SETTINGS"))
          break;
        this.m_gameplaySettings = (string) val;
        break;
      case 2294480894:
        if (!(name == "ENABLED"))
          break;
        this.m_enabled = (bool) val;
        break;
      case 2401654691:
        if (!(name == "IS_DEFAULT"))
          break;
        this.m_isDefault = (bool) val;
        break;
      case 2552907409:
        if (!(name == "COLLECTION_NAME"))
          break;
        this.m_collectionName = (DbfLocValue) val;
        break;
      case 2637965157:
        if (!(name == "DETAILS_RENDER_CONFIG"))
          break;
        this.m_detailsRenderConfig = (string) val;
        break;
      case 2901151038:
        if (!(name == "DETAILS_MOVIE"))
          break;
        this.m_detailsMovie = (string) val;
        break;
      case 2910661356:
        if (!(name == "MINI_ART_MATERIAL"))
          break;
        this.m_miniArtMaterial = (string) val;
        break;
      case 2975427914:
        if (!(name == "RARITY"))
          break;
        this.m_rarityId = (int) val;
        break;
      case 4011828978:
        if (!(name == "COLLECTION_SHORT_NAME"))
          break;
        this.m_collectionShortName = (DbfLocValue) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "CAPSULE_TYPE":
        return typeof (BattlegroundsFinisher.CapsuleType);
      case "COLLECTION_NAME":
        return typeof (DbfLocValue);
      case "COLLECTION_SHORT_NAME":
        return typeof (DbfLocValue);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "DESTROY_OPPONENT_PREFAB":
        return typeof (string);
      case "DESTROY_OPPONENT_VICTORY_PREFAB":
        return typeof (string);
      case "DETAILS_MOVIE":
        return typeof (string);
      case "DETAILS_RENDER_CONFIG":
        return typeof (string);
      case "DETAILS_TEXTURE":
        return typeof (string);
      case "ENABLED":
        return typeof (bool);
      case "GAMEPLAY_SETTINGS":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "IS_DEFAULT":
        return typeof (bool);
      case "MINI_ART_MATERIAL":
        return typeof (string);
      case "MINI_BODY_MATERIAL":
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
    LoadBattlegroundsFinisherDbfRecords loadRecords = new LoadBattlegroundsFinisherDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BattlegroundsFinisherDbfAsset finisherDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BattlegroundsFinisherDbfAsset)) as BattlegroundsFinisherDbfAsset;
    if ((UnityEngine.Object) finisherDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BattlegroundsFinisherDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < finisherDbfAsset.Records.Count; ++index)
      finisherDbfAsset.Records[index].StripUnusedLocales();
    records = finisherDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_collectionName.StripUnusedLocales();
    this.m_collectionShortName.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
