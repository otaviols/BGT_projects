using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdventureDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private DbfLocValue m_storeBuyButtonLabel;
  [SerializeField]
  private DbfLocValue m_storeBuyWings1Headline;
  [SerializeField]
  private DbfLocValue m_storeBuyWings2Headline;
  [SerializeField]
  private DbfLocValue m_storeBuyWings3Headline;
  [SerializeField]
  private DbfLocValue m_storeBuyWings4Headline;
  [SerializeField]
  private DbfLocValue m_storeBuyWings5Headline;
  [SerializeField]
  private DbfLocValue m_storeOwnedHeadline;
  [SerializeField]
  private DbfLocValue m_storePreorderHeadline;
  [SerializeField]
  private DbfLocValue m_storeBuyWings1Desc;
  [SerializeField]
  private DbfLocValue m_storeBuyWings2Desc;
  [SerializeField]
  private DbfLocValue m_storeBuyWings3Desc;
  [SerializeField]
  private DbfLocValue m_storeBuyWings4Desc;
  [SerializeField]
  private DbfLocValue m_storeBuyWings5Desc;
  [SerializeField]
  private DbfLocValue m_storeBuyRemainingWingsDescTimelockedTrue;
  [SerializeField]
  private DbfLocValue m_storeBuyRemainingWingsDescTimelockedFalse;
  [SerializeField]
  private DbfLocValue m_storeOwnedDesc;
  [SerializeField]
  private DbfLocValue m_storePreorderWings1Desc;
  [SerializeField]
  private DbfLocValue m_storePreorderWings2Desc;
  [SerializeField]
  private DbfLocValue m_storePreorderWings3Desc;
  [SerializeField]
  private DbfLocValue m_storePreorderWings4Desc;
  [SerializeField]
  private DbfLocValue m_storePreorderWings5Desc;
  [SerializeField]
  private DbfLocValue m_storePreorderRadioText;
  [SerializeField]
  private DbfLocValue m_storePreviewRewardsText;
  [SerializeField]
  private string m_adventureDefPrefab;
  [SerializeField]
  private string m_storePrefab;
  [SerializeField]
  private bool m_leavingSoon;
  [SerializeField]
  private DbfLocValue m_leavingSoonText;
  [SerializeField]
  private string m_gameModeIcon;
  [SerializeField]
  private string m_productStringKey;
  [SerializeField]
  private SpecialEventType m_standardEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");
  [SerializeField]
  private SpecialEventType m_comingSoonEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");
  [SerializeField]
  private DbfLocValue m_comingSoonText;
  [SerializeField]
  private bool m_mapPageHasButtonsToChapters;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("STORE_BUY_BUTTON_LABEL")]
  public DbfLocValue StoreBuyButtonLabel => this.m_storeBuyButtonLabel;

  [DbfField("STORE_OWNED_HEADLINE")]
  public DbfLocValue StoreOwnedHeadline => this.m_storeOwnedHeadline;

  [DbfField("STORE_PREORDER_HEADLINE")]
  public DbfLocValue StorePreorderHeadline => this.m_storePreorderHeadline;

  [DbfField("STORE_BUY_REMAINING_WINGS_DESC_TIMELOCKED_TRUE")]
  public DbfLocValue StoreBuyRemainingWingsDescTimelockedTrue => this.m_storeBuyRemainingWingsDescTimelockedTrue;

  [DbfField("STORE_BUY_REMAINING_WINGS_DESC_TIMELOCKED_FALSE")]
  public DbfLocValue StoreBuyRemainingWingsDescTimelockedFalse => this.m_storeBuyRemainingWingsDescTimelockedFalse;

  [DbfField("STORE_OWNED_DESC")]
  public DbfLocValue StoreOwnedDesc => this.m_storeOwnedDesc;

  [DbfField("STORE_PREORDER_RADIO_TEXT")]
  public DbfLocValue StorePreorderRadioText => this.m_storePreorderRadioText;

  [DbfField("STORE_PREVIEW_REWARDS_TEXT")]
  public DbfLocValue StorePreviewRewardsText => this.m_storePreviewRewardsText;

  [DbfField("ADVENTURE_DEF_PREFAB")]
  public string AdventureDefPrefab => this.m_adventureDefPrefab;

  [DbfField("STORE_PREFAB")]
  public string StorePrefab => this.m_storePrefab;

  [DbfField("LEAVING_SOON")]
  public bool LeavingSoon => this.m_leavingSoon;

  [DbfField("LEAVING_SOON_TEXT")]
  public DbfLocValue LeavingSoonText => this.m_leavingSoonText;

  [DbfField("GAME_MODE_ICON")]
  public string GameModeIcon => this.m_gameModeIcon;

  [DbfField("PRODUCT_STRING_KEY")]
  public string ProductStringKey => this.m_productStringKey;

  [DbfField("STANDARD_EVENT")]
  public SpecialEventType StandardEvent => this.m_standardEvent;

  [DbfField("COMING_SOON_EVENT")]
  public SpecialEventType ComingSoonEvent => this.m_comingSoonEvent;

  [DbfField("COMING_SOON_TEXT")]
  public DbfLocValue ComingSoonText => this.m_comingSoonText;

  [DbfField("MAP_PAGE_HAS_BUTTONS_TO_CHAPTERS")]
  public bool MapPageHasButtonsToChapters => this.m_mapPageHasButtonsToChapters;

  public List<AdventureHeroPowerDbfRecord> AdventureHeroPowers
  {
    get
    {
      int id = this.ID;
      List<AdventureHeroPowerDbfRecord> adventureHeroPowers = new List<AdventureHeroPowerDbfRecord>();
      List<AdventureHeroPowerDbfRecord> records = GameDbf.AdventureHeroPower.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        AdventureHeroPowerDbfRecord heroPowerDbfRecord = records[index];
        if (heroPowerDbfRecord.AdventureId == id)
          adventureHeroPowers.Add(heroPowerDbfRecord);
      }
      return adventureHeroPowers;
    }
  }

  public List<AdventureLoadoutTreasuresDbfRecord> AdventureLoadoutTreasures
  {
    get
    {
      int id = this.ID;
      List<AdventureLoadoutTreasuresDbfRecord> loadoutTreasures = new List<AdventureLoadoutTreasuresDbfRecord>();
      List<AdventureLoadoutTreasuresDbfRecord> records = GameDbf.AdventureLoadoutTreasures.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        AdventureLoadoutTreasuresDbfRecord treasuresDbfRecord = records[index];
        if (treasuresDbfRecord.AdventureId == id)
          loadoutTreasures.Add(treasuresDbfRecord);
      }
      return loadoutTreasures;
    }
  }

  public List<WingDbfRecord> Wings
  {
    get
    {
      int id = this.ID;
      List<WingDbfRecord> wings = new List<WingDbfRecord>();
      List<WingDbfRecord> records = GameDbf.Wing.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        WingDbfRecord wingDbfRecord = records[index];
        if (wingDbfRecord.AdventureId == id)
          wings.Add(wingDbfRecord);
      }
      return wings;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ADVENTURE_DEF_PREFAB":
        return (object) this.m_adventureDefPrefab;
      case "COMING_SOON_EVENT":
        return (object) this.m_comingSoonEvent;
      case "COMING_SOON_TEXT":
        return (object) this.m_comingSoonText;
      case "GAME_MODE_ICON":
        return (object) this.m_gameModeIcon;
      case "ID":
        return (object) this.ID;
      case "LEAVING_SOON":
        return (object) this.m_leavingSoon;
      case "LEAVING_SOON_TEXT":
        return (object) this.m_leavingSoonText;
      case "MAP_PAGE_HAS_BUTTONS_TO_CHAPTERS":
        return (object) this.m_mapPageHasButtonsToChapters;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "PRODUCT_STRING_KEY":
        return (object) this.m_productStringKey;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "STANDARD_EVENT":
        return (object) this.m_standardEvent;
      case "STORE_BUY_BUTTON_LABEL":
        return (object) this.m_storeBuyButtonLabel;
      case "STORE_BUY_REMAINING_WINGS_DESC_TIMELOCKED_FALSE":
        return (object) this.m_storeBuyRemainingWingsDescTimelockedFalse;
      case "STORE_BUY_REMAINING_WINGS_DESC_TIMELOCKED_TRUE":
        return (object) this.m_storeBuyRemainingWingsDescTimelockedTrue;
      case "STORE_BUY_WINGS_1_DESC":
        return (object) this.m_storeBuyWings1Desc;
      case "STORE_BUY_WINGS_1_HEADLINE":
        return (object) this.m_storeBuyWings1Headline;
      case "STORE_BUY_WINGS_2_DESC":
        return (object) this.m_storeBuyWings2Desc;
      case "STORE_BUY_WINGS_2_HEADLINE":
        return (object) this.m_storeBuyWings2Headline;
      case "STORE_BUY_WINGS_3_DESC":
        return (object) this.m_storeBuyWings3Desc;
      case "STORE_BUY_WINGS_3_HEADLINE":
        return (object) this.m_storeBuyWings3Headline;
      case "STORE_BUY_WINGS_4_DESC":
        return (object) this.m_storeBuyWings4Desc;
      case "STORE_BUY_WINGS_4_HEADLINE":
        return (object) this.m_storeBuyWings4Headline;
      case "STORE_BUY_WINGS_5_DESC":
        return (object) this.m_storeBuyWings5Desc;
      case "STORE_BUY_WINGS_5_HEADLINE":
        return (object) this.m_storeBuyWings5Headline;
      case "STORE_OWNED_DESC":
        return (object) this.m_storeOwnedDesc;
      case "STORE_OWNED_HEADLINE":
        return (object) this.m_storeOwnedHeadline;
      case "STORE_PREFAB":
        return (object) this.m_storePrefab;
      case "STORE_PREORDER_HEADLINE":
        return (object) this.m_storePreorderHeadline;
      case "STORE_PREORDER_RADIO_TEXT":
        return (object) this.m_storePreorderRadioText;
      case "STORE_PREORDER_WINGS_1_DESC":
        return (object) this.m_storePreorderWings1Desc;
      case "STORE_PREORDER_WINGS_2_DESC":
        return (object) this.m_storePreorderWings2Desc;
      case "STORE_PREORDER_WINGS_3_DESC":
        return (object) this.m_storePreorderWings3Desc;
      case "STORE_PREORDER_WINGS_4_DESC":
        return (object) this.m_storePreorderWings4Desc;
      case "STORE_PREORDER_WINGS_5_DESC":
        return (object) this.m_storePreorderWings5Desc;
      case "STORE_PREVIEW_REWARDS_TEXT":
        return (object) this.m_storePreviewRewardsText;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 72252647:
        if (!(name == "STORE_BUY_WINGS_4_DESC"))
          break;
        this.m_storeBuyWings4Desc = (DbfLocValue) val;
        break;
      case 72904445:
        if (!(name == "STORE_BUY_WINGS_2_DESC"))
          break;
        this.m_storeBuyWings2Desc = (DbfLocValue) val;
        break;
      case 389673047:
        if (!(name == "LEAVING_SOON_TEXT"))
          break;
        this.m_leavingSoonText = (DbfLocValue) val;
        break;
      case 397921193:
        if (!(name == "STORE_PREORDER_WINGS_5_DESC"))
          break;
        this.m_storePreorderWings5Desc = (DbfLocValue) val;
        break;
      case 499019123:
        if (!(name == "STORE_BUY_WINGS_1_HEADLINE"))
          break;
        this.m_storeBuyWings1Headline = (DbfLocValue) val;
        break;
      case 554784477:
        if (!(name == "STORE_PREFAB"))
          break;
        this.m_storePrefab = (string) val;
        break;
      case 598991083:
        if (!(name == "STORE_PREORDER_WINGS_3_DESC"))
          break;
        this.m_storePreorderWings3Desc = (DbfLocValue) val;
        break;
      case 634862270:
        if (!(name == "STORE_PREORDER_RADIO_TEXT"))
          break;
        this.m_storePreorderRadioText = (DbfLocValue) val;
        break;
      case 646730901:
        if (!(name == "STORE_OWNED_HEADLINE"))
          break;
        this.m_storeOwnedHeadline = (DbfLocValue) val;
        break;
      case 864600850:
        if (!(name == "COMING_SOON_TEXT"))
          break;
        this.m_comingSoonText = (DbfLocValue) val;
        break;
      case 1164573759:
        if (!(name == "LEAVING_SOON"))
          break;
        this.m_leavingSoon = (bool) val;
        break;
      case 1240037012:
        if (!(name == "STORE_BUY_WINGS_2_HEADLINE"))
          break;
        this.m_storeBuyWings2Headline = (DbfLocValue) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1424749021:
        if (!(name == "GAME_MODE_ICON"))
          break;
        this.m_gameModeIcon = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1491069082:
        if (!(name == "STORE_BUY_WINGS_5_DESC"))
          break;
        this.m_storeBuyWings5Desc = (DbfLocValue) val;
        break;
      case 1553026664:
        if (!(name == "PRODUCT_STRING_KEY"))
          break;
        this.m_productStringKey = (string) val;
        break;
      case 1688767887:
        if (!(name == "STORE_BUY_BUTTON_LABEL"))
          break;
        this.m_storeBuyButtonLabel = (DbfLocValue) val;
        break;
      case 2143204526:
        if (!(name == "STORE_PREORDER_WINGS_2_DESC"))
          break;
        this.m_storePreorderWings2Desc = (DbfLocValue) val;
        break;
      case 2191767247:
        if (!(name == "STANDARD_EVENT"))
          break;
        this.m_standardEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 2269500855:
        if (!(name == "STORE_BUY_REMAINING_WINGS_DESC_TIMELOCKED_FALSE"))
          break;
        this.m_storeBuyRemainingWingsDescTimelockedFalse = (DbfLocValue) val;
        break;
      case 2278395484:
        if (!(name == "STORE_PREVIEW_REWARDS_TEXT"))
          break;
        this.m_storePreviewRewardsText = (DbfLocValue) val;
        break;
      case 2377316180:
        if (!(name == "STORE_OWNED_DESC"))
          break;
        this.m_storeOwnedDesc = (DbfLocValue) val;
        break;
      case 2450510477:
        if (!(name == "STORE_PREORDER_HEADLINE"))
          break;
        this.m_storePreorderHeadline = (DbfLocValue) val;
        break;
      case 2462803332:
        if (!(name == "STORE_BUY_REMAINING_WINGS_DESC_TIMELOCKED_TRUE"))
          break;
        this.m_storeBuyRemainingWingsDescTimelockedTrue = (DbfLocValue) val;
        break;
      case 2558737930:
        if (!(name == "STORE_BUY_WINGS_4_HEADLINE"))
          break;
        this.m_storeBuyWings4Headline = (DbfLocValue) val;
        break;
      case 2609858387:
        if (!(name == "MAP_PAGE_HAS_BUTTONS_TO_CHAPTERS"))
          break;
        this.m_mapPageHasButtonsToChapters = (bool) val;
        break;
      case 2658528061:
        if (!(name == "STORE_PREORDER_WINGS_1_DESC"))
          break;
        this.m_storePreorderWings1Desc = (DbfLocValue) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3034224585:
        if (!(name == "STORE_BUY_WINGS_3_HEADLINE"))
          break;
        this.m_storeBuyWings3Headline = (DbfLocValue) val;
        break;
      case 3425700768:
        if (!(name == "STORE_BUY_WINGS_3_DESC"))
          break;
        this.m_storeBuyWings3Desc = (DbfLocValue) val;
        break;
      case 3687130011:
        if (!(name == "COMING_SOON_EVENT"))
          break;
        this.m_comingSoonEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 3958914583:
        if (!(name == "STORE_BUY_WINGS_5_HEADLINE"))
          break;
        this.m_storeBuyWings5Headline = (DbfLocValue) val;
        break;
      case 3977533932:
        if (!(name == "STORE_PREORDER_WINGS_4_DESC"))
          break;
        this.m_storePreorderWings4Desc = (DbfLocValue) val;
        break;
      case 3991768310:
        if (!(name == "ADVENTURE_DEF_PREFAB"))
          break;
        this.m_adventureDefPrefab = (string) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
      case 4277005678:
        if (!(name == "STORE_BUY_WINGS_1_DESC"))
          break;
        this.m_storeBuyWings1Desc = (DbfLocValue) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ADVENTURE_DEF_PREFAB":
        return typeof (string);
      case "COMING_SOON_EVENT":
        return typeof (string);
      case "COMING_SOON_TEXT":
        return typeof (DbfLocValue);
      case "GAME_MODE_ICON":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "LEAVING_SOON":
        return typeof (bool);
      case "LEAVING_SOON_TEXT":
        return typeof (DbfLocValue);
      case "MAP_PAGE_HAS_BUTTONS_TO_CHAPTERS":
        return typeof (bool);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_DESC":
        return typeof (string);
      case "PRODUCT_STRING_KEY":
        return typeof (string);
      case "SORT_ORDER":
        return typeof (int);
      case "STANDARD_EVENT":
        return typeof (string);
      case "STORE_BUY_BUTTON_LABEL":
        return typeof (DbfLocValue);
      case "STORE_BUY_REMAINING_WINGS_DESC_TIMELOCKED_FALSE":
        return typeof (DbfLocValue);
      case "STORE_BUY_REMAINING_WINGS_DESC_TIMELOCKED_TRUE":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_1_DESC":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_1_HEADLINE":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_2_DESC":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_2_HEADLINE":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_3_DESC":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_3_HEADLINE":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_4_DESC":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_4_HEADLINE":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_5_DESC":
        return typeof (DbfLocValue);
      case "STORE_BUY_WINGS_5_HEADLINE":
        return typeof (DbfLocValue);
      case "STORE_OWNED_DESC":
        return typeof (DbfLocValue);
      case "STORE_OWNED_HEADLINE":
        return typeof (DbfLocValue);
      case "STORE_PREFAB":
        return typeof (string);
      case "STORE_PREORDER_HEADLINE":
        return typeof (DbfLocValue);
      case "STORE_PREORDER_RADIO_TEXT":
        return typeof (DbfLocValue);
      case "STORE_PREORDER_WINGS_1_DESC":
        return typeof (DbfLocValue);
      case "STORE_PREORDER_WINGS_2_DESC":
        return typeof (DbfLocValue);
      case "STORE_PREORDER_WINGS_3_DESC":
        return typeof (DbfLocValue);
      case "STORE_PREORDER_WINGS_4_DESC":
        return typeof (DbfLocValue);
      case "STORE_PREORDER_WINGS_5_DESC":
        return typeof (DbfLocValue);
      case "STORE_PREVIEW_REWARDS_TEXT":
        return typeof (DbfLocValue);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAdventureDbfRecords loadRecords = new LoadAdventureDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AdventureDbfAsset adventureDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AdventureDbfAsset)) as AdventureDbfAsset;
    if ((UnityEngine.Object) adventureDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AdventureDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < adventureDbfAsset.Records.Count; ++index)
      adventureDbfAsset.Records[index].StripUnusedLocales();
    records = adventureDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_storeBuyButtonLabel.StripUnusedLocales();
    this.m_storeBuyWings1Headline.StripUnusedLocales();
    this.m_storeBuyWings2Headline.StripUnusedLocales();
    this.m_storeBuyWings3Headline.StripUnusedLocales();
    this.m_storeBuyWings4Headline.StripUnusedLocales();
    this.m_storeBuyWings5Headline.StripUnusedLocales();
    this.m_storeOwnedHeadline.StripUnusedLocales();
    this.m_storePreorderHeadline.StripUnusedLocales();
    this.m_storeBuyWings1Desc.StripUnusedLocales();
    this.m_storeBuyWings2Desc.StripUnusedLocales();
    this.m_storeBuyWings3Desc.StripUnusedLocales();
    this.m_storeBuyWings4Desc.StripUnusedLocales();
    this.m_storeBuyWings5Desc.StripUnusedLocales();
    this.m_storeBuyRemainingWingsDescTimelockedTrue.StripUnusedLocales();
    this.m_storeBuyRemainingWingsDescTimelockedFalse.StripUnusedLocales();
    this.m_storeOwnedDesc.StripUnusedLocales();
    this.m_storePreorderWings1Desc.StripUnusedLocales();
    this.m_storePreorderWings2Desc.StripUnusedLocales();
    this.m_storePreorderWings3Desc.StripUnusedLocales();
    this.m_storePreorderWings4Desc.StripUnusedLocales();
    this.m_storePreorderWings5Desc.StripUnusedLocales();
    this.m_storePreorderRadioText.StripUnusedLocales();
    this.m_storePreviewRewardsText.StripUnusedLocales();
    this.m_leavingSoonText.StripUnusedLocales();
    this.m_comingSoonText.StripUnusedLocales();
  }
}
