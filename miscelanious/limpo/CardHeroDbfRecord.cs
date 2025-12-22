using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardHeroDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_cardBackId;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private DbfLocValue m_storeDesc;
  [SerializeField]
  private DbfLocValue m_storeDescPhone;
  [SerializeField]
  private string m_storeBannerPrefab;
  [SerializeField]
  private string m_storeBackgroundTexture;
  [SerializeField]
  private int m_storeSortOrder;
  [SerializeField]
  private DbfLocValue m_purchaseCompleteMsg;
  [SerializeField]
  private CardHero.HeroType m_heroType;
  [SerializeField]
  private int m_collectionManagerPurchaseProductId;
  [SerializeField]
  private CardHero.PortraitCurrency m_collectionManagerPurchaseCurrency = CardHero.PortraitCurrency.GOLD;
  [SerializeField]
  private bool m_isCollectionManagerPurchaseDelayed;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  [DbfField("CARD_BACK_ID")]
  public int CardBackId => this.m_cardBackId;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("STORE_DESC")]
  public DbfLocValue StoreDesc => this.m_storeDesc;

  [DbfField("STORE_DESC_PHONE")]
  public DbfLocValue StoreDescPhone => this.m_storeDescPhone;

  [DbfField("STORE_BANNER_PREFAB")]
  public string StoreBannerPrefab => this.m_storeBannerPrefab;

  [DbfField("STORE_BACKGROUND_TEXTURE")]
  public string StoreBackgroundTexture => this.m_storeBackgroundTexture;

  [DbfField("STORE_SORT_ORDER")]
  public int StoreSortOrder => this.m_storeSortOrder;

  [DbfField("PURCHASE_COMPLETE_MSG")]
  public DbfLocValue PurchaseCompleteMsg => this.m_purchaseCompleteMsg;

  [DbfField("HERO_TYPE")]
  public CardHero.HeroType HeroType => this.m_heroType;

  [DbfField("COLLECTION_MANAGER_PURCHASE_PRODUCT_ID")]
  public int CollectionManagerPurchaseProductId => this.m_collectionManagerPurchaseProductId;

  [DbfField("COLLECTION_MANAGER_PURCHASE_CURRENCY")]
  public CardHero.PortraitCurrency CollectionManagerPurchaseCurrency => this.m_collectionManagerPurchaseCurrency;

  [DbfField("IS_COLLECTION_MANAGER_PURCHASE_DELAYED")]
  public bool IsCollectionManagerPurchaseDelayed => this.m_isCollectionManagerPurchaseDelayed;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "CARD_BACK_ID":
        return (object) this.m_cardBackId;
      case "CARD_ID":
        return (object) this.m_cardId;
      case "COLLECTION_MANAGER_PURCHASE_CURRENCY":
        return (object) this.m_collectionManagerPurchaseCurrency;
      case "COLLECTION_MANAGER_PURCHASE_PRODUCT_ID":
        return (object) this.m_collectionManagerPurchaseProductId;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "HERO_TYPE":
        return (object) this.m_heroType;
      case "ID":
        return (object) this.ID;
      case "IS_COLLECTION_MANAGER_PURCHASE_DELAYED":
        return (object) this.m_isCollectionManagerPurchaseDelayed;
      case "PURCHASE_COMPLETE_MSG":
        return (object) this.m_purchaseCompleteMsg;
      case "STORE_BACKGROUND_TEXTURE":
        return (object) this.m_storeBackgroundTexture;
      case "STORE_BANNER_PREFAB":
        return (object) this.m_storeBannerPrefab;
      case "STORE_DESC":
        return (object) this.m_storeDesc;
      case "STORE_DESC_PHONE":
        return (object) this.m_storeDescPhone;
      case "STORE_SORT_ORDER":
        return (object) this.m_storeSortOrder;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 7265668:
        if (!(name == "STORE_SORT_ORDER"))
          break;
        this.m_storeSortOrder = (int) val;
        break;
      case 439569377:
        if (!(name == "STORE_DESC_PHONE"))
          break;
        this.m_storeDescPhone = (DbfLocValue) val;
        break;
      case 451390141:
        if (!(name == "CARD_ID"))
          break;
        this.m_cardId = (int) val;
        break;
      case 874236059:
        if (!(name == "STORE_BACKGROUND_TEXTURE"))
          break;
        this.m_storeBackgroundTexture = (string) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1263240540:
        if (!(name == "STORE_BANNER_PREFAB"))
          break;
        this.m_storeBannerPrefab = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1560548161:
        if (!(name == "CARD_BACK_ID"))
          break;
        this.m_cardBackId = (int) val;
        break;
      case 1640535074:
        if (!(name == "HERO_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_heroType = CardHero.HeroType.UNKNOWN;
            return;
          case CardHero.HeroType _:
          case int _:
            this.m_heroType = (CardHero.HeroType) val;
            return;
          case string _:
            this.m_heroType = CardHero.ParseHeroTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1672933598:
        if (!(name == "STORE_DESC"))
          break;
        this.m_storeDesc = (DbfLocValue) val;
        break;
      case 2635266015:
        if (!(name == "COLLECTION_MANAGER_PURCHASE_PRODUCT_ID"))
          break;
        this.m_collectionManagerPurchaseProductId = (int) val;
        break;
      case 3212067681:
        if (!(name == "IS_COLLECTION_MANAGER_PURCHASE_DELAYED"))
          break;
        this.m_isCollectionManagerPurchaseDelayed = (bool) val;
        break;
      case 3899252054:
        if (!(name == "PURCHASE_COMPLETE_MSG"))
          break;
        this.m_purchaseCompleteMsg = (DbfLocValue) val;
        break;
      case 4265965577:
        if (!(name == "COLLECTION_MANAGER_PURCHASE_CURRENCY"))
          break;
        switch (val)
        {
          case null:
            this.m_collectionManagerPurchaseCurrency = CardHero.PortraitCurrency.UNKNOWN;
            return;
          case CardHero.PortraitCurrency _:
          case int _:
            this.m_collectionManagerPurchaseCurrency = (CardHero.PortraitCurrency) val;
            return;
          case string _:
            this.m_collectionManagerPurchaseCurrency = CardHero.ParsePortraitCurrencyValue((string) val);
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
      case "CARD_BACK_ID":
        return typeof (int);
      case "CARD_ID":
        return typeof (int);
      case "COLLECTION_MANAGER_PURCHASE_CURRENCY":
        return typeof (CardHero.PortraitCurrency);
      case "COLLECTION_MANAGER_PURCHASE_PRODUCT_ID":
        return typeof (int);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "HERO_TYPE":
        return typeof (CardHero.HeroType);
      case "ID":
        return typeof (int);
      case "IS_COLLECTION_MANAGER_PURCHASE_DELAYED":
        return typeof (bool);
      case "PURCHASE_COMPLETE_MSG":
        return typeof (DbfLocValue);
      case "STORE_BACKGROUND_TEXTURE":
        return typeof (string);
      case "STORE_BANNER_PREFAB":
        return typeof (string);
      case "STORE_DESC":
        return typeof (DbfLocValue);
      case "STORE_DESC_PHONE":
        return typeof (DbfLocValue);
      case "STORE_SORT_ORDER":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardHeroDbfRecords loadRecords = new LoadCardHeroDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardHeroDbfAsset cardHeroDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardHeroDbfAsset)) as CardHeroDbfAsset;
    if ((UnityEngine.Object) cardHeroDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardHeroDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardHeroDbfAsset.Records.Count; ++index)
      cardHeroDbfAsset.Records[index].StripUnusedLocales();
    records = cardHeroDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_description.StripUnusedLocales();
    this.m_storeDesc.StripUnusedLocales();
    this.m_storeDescPhone.StripUnusedLocales();
    this.m_purchaseCompleteMsg.StripUnusedLocales();
  }
}
