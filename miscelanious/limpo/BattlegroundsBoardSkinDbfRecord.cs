using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattlegroundsBoardSkinDbfRecord : DbfRecord
{
  [SerializeField]
  private bool m_enabled = true;
  [SerializeField]
  private int m_rarityId = 2;
  [SerializeField]
  private string m_fullBoardPrefab;
  [SerializeField]
  private string m_fullBoardPrefabPhone;
  [SerializeField]
  private string m_fullTavernBoardPrefab;
  [SerializeField]
  private string m_fullTavernBoardPrefabPhone;
  [SerializeField]
  private DbfLocValue m_collectionName;
  [SerializeField]
  private DbfLocValue m_collectionShortName;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private string m_detailsTexture;
  [SerializeField]
  private string m_detailsTexturePhone;
  [SerializeField]
  private string m_detailsMovie;
  [SerializeField]
  private string m_detailsMoviePhone;
  [SerializeField]
  private string m_detailsRenderConfig;
  [SerializeField]
  private BattlegroundsBoardSkin.Bordertype m_borderType;

  [DbfField("RARITY")]
  public int Rarity => this.m_rarityId;

  [DbfField("FULL_BOARD_PREFAB")]
  public string FullBoardPrefab => this.m_fullBoardPrefab;

  [DbfField("FULL_BOARD_PREFAB_PHONE")]
  public string FullBoardPrefabPhone => this.m_fullBoardPrefabPhone;

  [DbfField("FULL_TAVERN_BOARD_PREFAB")]
  public string FullTavernBoardPrefab => this.m_fullTavernBoardPrefab;

  [DbfField("FULL_TAVERN_BOARD_PREFAB_PHONE")]
  public string FullTavernBoardPrefabPhone => this.m_fullTavernBoardPrefabPhone;

  [DbfField("COLLECTION_NAME")]
  public DbfLocValue CollectionName => this.m_collectionName;

  [DbfField("COLLECTION_SHORT_NAME")]
  public DbfLocValue CollectionShortName => this.m_collectionShortName;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("DETAILS_TEXTURE")]
  public string DetailsTexture => this.m_detailsTexture;

  [DbfField("DETAILS_TEXTURE_PHONE")]
  public string DetailsTexturePhone => this.m_detailsTexturePhone;

  [DbfField("DETAILS_MOVIE")]
  public string DetailsMovie => this.m_detailsMovie;

  [DbfField("DETAILS_MOVIE_PHONE")]
  public string DetailsMoviePhone => this.m_detailsMoviePhone;

  [DbfField("BORDER_TYPE")]
  public BattlegroundsBoardSkin.Bordertype BorderType => this.m_borderType;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BORDER_TYPE":
        return (object) this.m_borderType;
      case "COLLECTION_NAME":
        return (object) this.m_collectionName;
      case "COLLECTION_SHORT_NAME":
        return (object) this.m_collectionShortName;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "DETAILS_MOVIE":
        return (object) this.m_detailsMovie;
      case "DETAILS_MOVIE_PHONE":
        return (object) this.m_detailsMoviePhone;
      case "DETAILS_RENDER_CONFIG":
        return (object) this.m_detailsRenderConfig;
      case "DETAILS_TEXTURE":
        return (object) this.m_detailsTexture;
      case "DETAILS_TEXTURE_PHONE":
        return (object) this.m_detailsTexturePhone;
      case "ENABLED":
        return (object) this.m_enabled;
      case "FULL_BOARD_PREFAB":
        return (object) this.m_fullBoardPrefab;
      case "FULL_BOARD_PREFAB_PHONE":
        return (object) this.m_fullBoardPrefabPhone;
      case "FULL_TAVERN_BOARD_PREFAB":
        return (object) this.m_fullTavernBoardPrefab;
      case "FULL_TAVERN_BOARD_PREFAB_PHONE":
        return (object) this.m_fullTavernBoardPrefabPhone;
      case "ID":
        return (object) this.ID;
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
      case 337942481:
        if (!(name == "DETAILS_TEXTURE"))
          break;
        this.m_detailsTexture = (string) val;
        break;
      case 434695882:
        if (!(name == "DETAILS_TEXTURE_PHONE"))
          break;
        this.m_detailsTexturePhone = (string) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1378010714:
        if (!(name == "FULL_BOARD_PREFAB"))
          break;
        this.m_fullBoardPrefab = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1691910384:
        if (!(name == "BORDER_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_borderType = BattlegroundsBoardSkin.Bordertype.DEFAULT;
            return;
          case BattlegroundsBoardSkin.Bordertype _:
          case int _:
            this.m_borderType = (BattlegroundsBoardSkin.Bordertype) val;
            return;
          case string _:
            this.m_borderType = BattlegroundsBoardSkin.ParseBordertypeValue((string) val);
            return;
          default:
            return;
        }
      case 1878857912:
        if (!(name == "FULL_TAVERN_BOARD_PREFAB_PHONE"))
          break;
        this.m_fullTavernBoardPrefabPhone = (string) val;
        break;
      case 2082281355:
        if (!(name == "FULL_TAVERN_BOARD_PREFAB"))
          break;
        this.m_fullTavernBoardPrefab = (string) val;
        break;
      case 2189518749:
        if (!(name == "FULL_BOARD_PREFAB_PHONE"))
          break;
        this.m_fullBoardPrefabPhone = (string) val;
        break;
      case 2294480894:
        if (!(name == "ENABLED"))
          break;
        this.m_enabled = (bool) val;
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
      case 2944561473:
        if (!(name == "DETAILS_MOVIE_PHONE"))
          break;
        this.m_detailsMoviePhone = (string) val;
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
      case "BORDER_TYPE":
        return typeof (BattlegroundsBoardSkin.Bordertype);
      case "COLLECTION_NAME":
        return typeof (DbfLocValue);
      case "COLLECTION_SHORT_NAME":
        return typeof (DbfLocValue);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "DETAILS_MOVIE":
        return typeof (string);
      case "DETAILS_MOVIE_PHONE":
        return typeof (string);
      case "DETAILS_RENDER_CONFIG":
        return typeof (string);
      case "DETAILS_TEXTURE":
        return typeof (string);
      case "DETAILS_TEXTURE_PHONE":
        return typeof (string);
      case "ENABLED":
        return typeof (bool);
      case "FULL_BOARD_PREFAB":
        return typeof (string);
      case "FULL_BOARD_PREFAB_PHONE":
        return typeof (string);
      case "FULL_TAVERN_BOARD_PREFAB":
        return typeof (string);
      case "FULL_TAVERN_BOARD_PREFAB_PHONE":
        return typeof (string);
      case "ID":
        return typeof (int);
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
    LoadBattlegroundsBoardSkinDbfRecords loadRecords = new LoadBattlegroundsBoardSkinDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BattlegroundsBoardSkinDbfAsset boardSkinDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BattlegroundsBoardSkinDbfAsset)) as BattlegroundsBoardSkinDbfAsset;
    if ((UnityEngine.Object) boardSkinDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BattlegroundsBoardSkinDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < boardSkinDbfAsset.Records.Count; ++index)
      boardSkinDbfAsset.Records[index].StripUnusedLocales();
    records = boardSkinDbfAsset.Records as List<T>;
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
