using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardBackDbfRecord : DbfRecord
{
  public const int TotalCollectedToAutoFavorite = 3;
  public const int TheRandomCardBackId = 158;
  [SerializeField]
  private long m_data1;
  [SerializeField]
  private Assets.CardBack.Source m_source = Assets.CardBack.ParseSourceValue("unknown");
  [SerializeField]
  private bool m_enabled;
  [SerializeField]
  private Assets.CardBack.SortCategory m_sortCategory;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private string m_prefabName;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private bool m_isRandomCardBack;
  [SerializeField]
  private int m_collectionManagerPurchaseProductId;

  [DbfField("DATA1")]
  public long Data1 => this.m_data1;

  [DbfField("SOURCE")]
  public Assets.CardBack.Source Source => this.m_source;

  [DbfField("ENABLED")]
  public bool Enabled => this.m_enabled;

  [DbfField("SORT_CATEGORY")]
  public Assets.CardBack.SortCategory SortCategory => this.m_sortCategory;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("PREFAB_NAME")]
  public string PrefabName => this.m_prefabName;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("IS_RANDOM_CARD_BACK")]
  public bool IsRandomCardBack => this.m_isRandomCardBack;

  [DbfField("COLLECTION_MANAGER_PURCHASE_PRODUCT_ID")]
  public int CollectionManagerPurchaseProductId => this.m_collectionManagerPurchaseProductId;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "COLLECTION_MANAGER_PURCHASE_PRODUCT_ID":
        return (object) this.m_collectionManagerPurchaseProductId;
      case "DATA1":
        return (object) this.m_data1;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "ENABLED":
        return (object) this.m_enabled;
      case "ID":
        return (object) this.ID;
      case "IS_RANDOM_CARD_BACK":
        return (object) this.m_isRandomCardBack;
      case "NAME":
        return (object) this.m_name;
      case "PREFAB_NAME":
        return (object) this.m_prefabName;
      case "SORT_CATEGORY":
        return (object) this.m_sortCategory;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "SOURCE":
        return (object) this.m_source;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1821367228:
        if (!(name == "DATA1"))
          break;
        this.m_data1 = (long) val;
        break;
      case 2294480894:
        if (!(name == "ENABLED"))
          break;
        this.m_enabled = (bool) val;
        break;
      case 2300801615:
        if (!(name == "PREFAB_NAME"))
          break;
        this.m_prefabName = (string) val;
        break;
      case 2635266015:
        if (!(name == "COLLECTION_MANAGER_PURCHASE_PRODUCT_ID"))
          break;
        this.m_collectionManagerPurchaseProductId = (int) val;
        break;
      case 2656358914:
        if (!(name == "IS_RANDOM_CARD_BACK"))
          break;
        this.m_isRandomCardBack = (bool) val;
        break;
      case 3111715480:
        if (!(name == "SOURCE"))
          break;
        switch (val)
        {
          case null:
            this.m_source = Assets.CardBack.Source.STARTUP;
            return;
          case Assets.CardBack.Source _:
          case int _:
            this.m_source = (Assets.CardBack.Source) val;
            return;
          case string _:
            this.m_source = Assets.CardBack.ParseSourceValue((string) val);
            return;
          default:
            return;
        }
      case 3923265666:
        if (!(name == "SORT_CATEGORY"))
          break;
        switch (val)
        {
          case null:
            this.m_sortCategory = Assets.CardBack.SortCategory.NONE;
            return;
          case Assets.CardBack.SortCategory _:
          case int _:
            this.m_sortCategory = (Assets.CardBack.SortCategory) val;
            return;
          case string _:
            this.m_sortCategory = Assets.CardBack.ParseSortCategoryValue((string) val);
            return;
          default:
            return;
        }
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "COLLECTION_MANAGER_PURCHASE_PRODUCT_ID":
        return typeof (int);
      case "DATA1":
        return typeof (long);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "ENABLED":
        return typeof (bool);
      case "ID":
        return typeof (int);
      case "IS_RANDOM_CARD_BACK":
        return typeof (bool);
      case "NAME":
        return typeof (DbfLocValue);
      case "PREFAB_NAME":
        return typeof (string);
      case "SORT_CATEGORY":
        return typeof (Assets.CardBack.SortCategory);
      case "SORT_ORDER":
        return typeof (int);
      case "SOURCE":
        return typeof (Assets.CardBack.Source);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardBackDbfRecords loadRecords = new LoadCardBackDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardBackDbfAsset cardBackDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardBackDbfAsset)) as CardBackDbfAsset;
    if ((UnityEngine.Object) cardBackDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardBackDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardBackDbfAsset.Records.Count; ++index)
      cardBackDbfAsset.Records[index].StripUnusedLocales();
    records = cardBackDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
