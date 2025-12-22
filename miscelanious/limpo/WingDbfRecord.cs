using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WingDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_adventureId;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private int m_unlockOrder;
  [SerializeField]
  private SpecialEventType m_requiredEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("none");
  [SerializeField]
  private int m_ownershipPrereqWingId;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_nameShort;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private DbfLocValue m_classChallengeRewardSource;
  [SerializeField]
  private string m_adventureWingDefPrefab;
  [SerializeField]
  private DbfLocValue m_comingSoonLabel;
  [SerializeField]
  private DbfLocValue m_requiresLabel;
  [SerializeField]
  private int m_openPrereqWingId;
  [SerializeField]
  private DbfLocValue m_openDiscouragedLabel;
  [SerializeField]
  private DbfLocValue m_openDiscouragedWarning;
  [SerializeField]
  private bool m_mustCompleteOpenPrereq;
  [SerializeField]
  private bool m_unlocksAutomatically;
  [SerializeField]
  private bool m_useUnlockCountdown;
  [SerializeField]
  private DbfLocValue m_storeBuyWingButtonLabel;
  [SerializeField]
  private DbfLocValue m_storeBuyWingDesc;
  [SerializeField]
  private int m_dungeonCrawlBosses = 8;
  [SerializeField]
  private string m_visualStateName;
  [SerializeField]
  private int m_plotTwistCardId;
  [SerializeField]
  private bool m_displayRaidBossHealth;
  [SerializeField]
  private int m_raidBossCardId;
  [SerializeField]
  private bool m_allowsAnomaly = true;
  [SerializeField]
  private int m_pmtProductIdForSingleWingPurchase;
  [SerializeField]
  private int m_pmtProductIdForThisAndRestOfAdventure;
  [SerializeField]
  private int m_bookSection;

  [DbfField("ADVENTURE_ID")]
  public int AdventureId => this.m_adventureId;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("UNLOCK_ORDER")]
  public int UnlockOrder => this.m_unlockOrder;

  [DbfField("REQUIRED_EVENT")]
  public SpecialEventType RequiredEvent => this.m_requiredEvent;

  [DbfField("OWNERSHIP_PREREQ_WING_ID")]
  public int OwnershipPrereqWingId => this.m_ownershipPrereqWingId;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("NAME_SHORT")]
  public DbfLocValue NameShort => this.m_nameShort;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("CLASS_CHALLENGE_REWARD_SOURCE")]
  public DbfLocValue ClassChallengeRewardSource => this.m_classChallengeRewardSource;

  [DbfField("ADVENTURE_WING_DEF_PREFAB")]
  public string AdventureWingDefPrefab => this.m_adventureWingDefPrefab;

  [DbfField("COMING_SOON_LABEL")]
  public DbfLocValue ComingSoonLabel => this.m_comingSoonLabel;

  [DbfField("REQUIRES_LABEL")]
  public DbfLocValue RequiresLabel => this.m_requiresLabel;

  [DbfField("OPEN_PREREQ_WING_ID")]
  public int OpenPrereqWingId => this.m_openPrereqWingId;

  [DbfField("OPEN_DISCOURAGED_LABEL")]
  public DbfLocValue OpenDiscouragedLabel => this.m_openDiscouragedLabel;

  [DbfField("OPEN_DISCOURAGED_WARNING")]
  public DbfLocValue OpenDiscouragedWarning => this.m_openDiscouragedWarning;

  [DbfField("MUST_COMPLETE_OPEN_PREREQ")]
  public bool MustCompleteOpenPrereq => this.m_mustCompleteOpenPrereq;

  [DbfField("UNLOCKS_AUTOMATICALLY")]
  public bool UnlocksAutomatically => this.m_unlocksAutomatically;

  [DbfField("USE_UNLOCK_COUNTDOWN")]
  public bool UseUnlockCountdown => this.m_useUnlockCountdown;

  [DbfField("STORE_BUY_WING_BUTTON_LABEL")]
  public DbfLocValue StoreBuyWingButtonLabel => this.m_storeBuyWingButtonLabel;

  [DbfField("STORE_BUY_WING_DESC")]
  public DbfLocValue StoreBuyWingDesc => this.m_storeBuyWingDesc;

  [DbfField("DUNGEON_CRAWL_BOSSES")]
  public int DungeonCrawlBosses => this.m_dungeonCrawlBosses;

  [DbfField("VISUAL_STATE_NAME")]
  public string VisualStateName => this.m_visualStateName;

  [DbfField("PLOT_TWIST_CARD_ID")]
  public int PlotTwistCardId => this.m_plotTwistCardId;

  [DbfField("DISPLAY_RAID_BOSS_HEALTH")]
  public bool DisplayRaidBossHealth => this.m_displayRaidBossHealth;

  [DbfField("RAID_BOSS_CARD_ID")]
  public int RaidBossCardId => this.m_raidBossCardId;

  [DbfField("ALLOWS_ANOMALY")]
  public bool AllowsAnomaly => this.m_allowsAnomaly;

  [DbfField("PMT_PRODUCT_ID_FOR_SINGLE_WING_PURCHASE")]
  public int PmtProductIdForSingleWingPurchase => this.m_pmtProductIdForSingleWingPurchase;

  [DbfField("PMT_PRODUCT_ID_FOR_THIS_AND_REST_OF_ADVENTURE")]
  public int PmtProductIdForThisAndRestOfAdventure => this.m_pmtProductIdForThisAndRestOfAdventure;

  [DbfField("BOOK_SECTION")]
  public int BookSection => this.m_bookSection;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return (object) this.m_adventureId;
      case "ADVENTURE_WING_DEF_PREFAB":
        return (object) this.m_adventureWingDefPrefab;
      case "ALLOWS_ANOMALY":
        return (object) this.m_allowsAnomaly;
      case "BOOK_SECTION":
        return (object) this.m_bookSection;
      case "CLASS_CHALLENGE_REWARD_SOURCE":
        return (object) this.m_classChallengeRewardSource;
      case "COMING_SOON_LABEL":
        return (object) this.m_comingSoonLabel;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "DISPLAY_RAID_BOSS_HEALTH":
        return (object) this.m_displayRaidBossHealth;
      case "DUNGEON_CRAWL_BOSSES":
        return (object) this.m_dungeonCrawlBosses;
      case "ID":
        return (object) this.ID;
      case "MUST_COMPLETE_OPEN_PREREQ":
        return (object) this.m_mustCompleteOpenPrereq;
      case "NAME":
        return (object) this.m_name;
      case "NAME_SHORT":
        return (object) this.m_nameShort;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "OPEN_DISCOURAGED_LABEL":
        return (object) this.m_openDiscouragedLabel;
      case "OPEN_DISCOURAGED_WARNING":
        return (object) this.m_openDiscouragedWarning;
      case "OPEN_PREREQ_WING_ID":
        return (object) this.m_openPrereqWingId;
      case "OWNERSHIP_PREREQ_WING_ID":
        return (object) this.m_ownershipPrereqWingId;
      case "PLOT_TWIST_CARD_ID":
        return (object) this.m_plotTwistCardId;
      case "PMT_PRODUCT_ID_FOR_SINGLE_WING_PURCHASE":
        return (object) this.m_pmtProductIdForSingleWingPurchase;
      case "PMT_PRODUCT_ID_FOR_THIS_AND_REST_OF_ADVENTURE":
        return (object) this.m_pmtProductIdForThisAndRestOfAdventure;
      case "RAID_BOSS_CARD_ID":
        return (object) this.m_raidBossCardId;
      case "REQUIRED_EVENT":
        return (object) this.m_requiredEvent;
      case "REQUIRES_LABEL":
        return (object) this.m_requiresLabel;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "STORE_BUY_WING_BUTTON_LABEL":
        return (object) this.m_storeBuyWingButtonLabel;
      case "STORE_BUY_WING_DESC":
        return (object) this.m_storeBuyWingDesc;
      case "UNLOCKS_AUTOMATICALLY":
        return (object) this.m_unlocksAutomatically;
      case "UNLOCK_ORDER":
        return (object) this.m_unlockOrder;
      case "USE_UNLOCK_COUNTDOWN":
        return (object) this.m_useUnlockCountdown;
      case "VISUAL_STATE_NAME":
        return (object) this.m_visualStateName;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 91939326:
        if (!(name == "OWNERSHIP_PREREQ_WING_ID"))
          break;
        this.m_ownershipPrereqWingId = (int) val;
        break;
      case 190718801:
        if (!(name == "ADVENTURE_ID"))
          break;
        this.m_adventureId = (int) val;
        break;
      case 265019175:
        if (!(name == "REQUIRED_EVENT"))
          break;
        this.m_requiredEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 595659997:
        if (!(name == "ALLOWS_ANOMALY"))
          break;
        this.m_allowsAnomaly = (bool) val;
        break;
      case 827198106:
        if (!(name == "ADVENTURE_WING_DEF_PREFAB"))
          break;
        this.m_adventureWingDefPrefab = (string) val;
        break;
      case 864975350:
        if (!(name == "UNLOCK_ORDER"))
          break;
        this.m_unlockOrder = (int) val;
        break;
      case 933277803:
        if (!(name == "DUNGEON_CRAWL_BOSSES"))
          break;
        this.m_dungeonCrawlBosses = (int) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1311799363:
        if (!(name == "MUST_COMPLETE_OPEN_PREREQ"))
          break;
        this.m_mustCompleteOpenPrereq = (bool) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1439689743:
        if (!(name == "PMT_PRODUCT_ID_FOR_SINGLE_WING_PURCHASE"))
          break;
        this.m_pmtProductIdForSingleWingPurchase = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1555399727:
        if (!(name == "STORE_BUY_WING_BUTTON_LABEL"))
          break;
        this.m_storeBuyWingButtonLabel = (DbfLocValue) val;
        break;
      case 2014266179:
        if (!(name == "PLOT_TWIST_CARD_ID"))
          break;
        this.m_plotTwistCardId = (int) val;
        break;
      case 2290477324:
        if (!(name == "BOOK_SECTION"))
          break;
        this.m_bookSection = (int) val;
        break;
      case 2360313627:
        if (!(name == "VISUAL_STATE_NAME"))
          break;
        this.m_visualStateName = (string) val;
        break;
      case 2449846983:
        if (!(name == "OPEN_PREREQ_WING_ID"))
          break;
        this.m_openPrereqWingId = (int) val;
        break;
      case 2490864483:
        if (!(name == "PMT_PRODUCT_ID_FOR_THIS_AND_REST_OF_ADVENTURE"))
          break;
        this.m_pmtProductIdForThisAndRestOfAdventure = (int) val;
        break;
      case 2503143781:
        if (!(name == "OPEN_DISCOURAGED_WARNING"))
          break;
        this.m_openDiscouragedWarning = (DbfLocValue) val;
        break;
      case 2610363275:
        if (!(name == "OPEN_DISCOURAGED_LABEL"))
          break;
        this.m_openDiscouragedLabel = (DbfLocValue) val;
        break;
      case 2694664299:
        if (!(name == "STORE_BUY_WING_DESC"))
          break;
        this.m_storeBuyWingDesc = (DbfLocValue) val;
        break;
      case 2756755729:
        if (!(name == "USE_UNLOCK_COUNTDOWN"))
          break;
        this.m_useUnlockCountdown = (bool) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3070501611:
        if (!(name == "CLASS_CHALLENGE_REWARD_SOURCE"))
          break;
        this.m_classChallengeRewardSource = (DbfLocValue) val;
        break;
      case 3152384434:
        if (!(name == "RAID_BOSS_CARD_ID"))
          break;
        this.m_raidBossCardId = (int) val;
        break;
      case 3753829705:
        if (!(name == "COMING_SOON_LABEL"))
          break;
        this.m_comingSoonLabel = (DbfLocValue) val;
        break;
      case 3782964816:
        if (!(name == "UNLOCKS_AUTOMATICALLY"))
          break;
        this.m_unlocksAutomatically = (bool) val;
        break;
      case 4080209141:
        if (!(name == "DISPLAY_RAID_BOSS_HEALTH"))
          break;
        this.m_displayRaidBossHealth = (bool) val;
        break;
      case 4127906772:
        if (!(name == "REQUIRES_LABEL"))
          break;
        this.m_requiresLabel = (DbfLocValue) val;
        break;
      case 4136070939:
        if (!(name == "NAME_SHORT"))
          break;
        this.m_nameShort = (DbfLocValue) val;
        break;
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
      case "ADVENTURE_ID":
        return typeof (int);
      case "ADVENTURE_WING_DEF_PREFAB":
        return typeof (string);
      case "ALLOWS_ANOMALY":
        return typeof (bool);
      case "BOOK_SECTION":
        return typeof (int);
      case "CLASS_CHALLENGE_REWARD_SOURCE":
        return typeof (DbfLocValue);
      case "COMING_SOON_LABEL":
        return typeof (DbfLocValue);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "DISPLAY_RAID_BOSS_HEALTH":
        return typeof (bool);
      case "DUNGEON_CRAWL_BOSSES":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "MUST_COMPLETE_OPEN_PREREQ":
        return typeof (bool);
      case "NAME":
        return typeof (DbfLocValue);
      case "NAME_SHORT":
        return typeof (DbfLocValue);
      case "NOTE_DESC":
        return typeof (string);
      case "OPEN_DISCOURAGED_LABEL":
        return typeof (DbfLocValue);
      case "OPEN_DISCOURAGED_WARNING":
        return typeof (DbfLocValue);
      case "OPEN_PREREQ_WING_ID":
        return typeof (int);
      case "OWNERSHIP_PREREQ_WING_ID":
        return typeof (int);
      case "PLOT_TWIST_CARD_ID":
        return typeof (int);
      case "PMT_PRODUCT_ID_FOR_SINGLE_WING_PURCHASE":
        return typeof (int);
      case "PMT_PRODUCT_ID_FOR_THIS_AND_REST_OF_ADVENTURE":
        return typeof (int);
      case "RAID_BOSS_CARD_ID":
        return typeof (int);
      case "REQUIRED_EVENT":
        return typeof (string);
      case "REQUIRES_LABEL":
        return typeof (DbfLocValue);
      case "SORT_ORDER":
        return typeof (int);
      case "STORE_BUY_WING_BUTTON_LABEL":
        return typeof (DbfLocValue);
      case "STORE_BUY_WING_DESC":
        return typeof (DbfLocValue);
      case "UNLOCKS_AUTOMATICALLY":
        return typeof (bool);
      case "UNLOCK_ORDER":
        return typeof (int);
      case "USE_UNLOCK_COUNTDOWN":
        return typeof (bool);
      case "VISUAL_STATE_NAME":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadWingDbfRecords loadRecords = new LoadWingDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    WingDbfAsset wingDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (WingDbfAsset)) as WingDbfAsset;
    if ((UnityEngine.Object) wingDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("WingDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < wingDbfAsset.Records.Count; ++index)
      wingDbfAsset.Records[index].StripUnusedLocales();
    records = wingDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_nameShort.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
    this.m_classChallengeRewardSource.StripUnusedLocales();
    this.m_comingSoonLabel.StripUnusedLocales();
    this.m_requiresLabel.StripUnusedLocales();
    this.m_openDiscouragedLabel.StripUnusedLocales();
    this.m_openDiscouragedWarning.StripUnusedLocales();
    this.m_storeBuyWingButtonLabel.StripUnusedLocales();
    this.m_storeBuyWingDesc.StripUnusedLocales();
  }
}
