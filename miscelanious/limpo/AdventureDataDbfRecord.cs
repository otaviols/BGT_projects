using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdventureDataDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_adventureId;
  [SerializeField]
  private int m_modeId;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_shortName;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private DbfLocValue m_shortDescription;
  [SerializeField]
  private DbfLocValue m_lockedShortName;
  [SerializeField]
  private DbfLocValue m_lockedDescription;
  [SerializeField]
  private DbfLocValue m_lockedShortDescription;
  [SerializeField]
  private DbfLocValue m_requirementsDescription;
  [SerializeField]
  private DbfLocValue m_rewardsDescription;
  [SerializeField]
  private DbfLocValue m_completeBannerText;
  [SerializeField]
  private bool m_showPlayableScenariosCount = true;
  [SerializeField]
  private AdventureData.Adventuresubscene m_startingSubscene;
  [SerializeField]
  private string m_subsceneTransitionDirection = "INVALID";
  [SerializeField]
  private string m_adventureSubDefPrefab;
  [SerializeField]
  private int m_gameSaveDataServerKeyId;
  [SerializeField]
  private int m_gameSaveDataClientKeyId;
  [SerializeField]
  private bool m_dungeonCrawlSaveHeroUsingHeroDbId = true;
  [SerializeField]
  private string m_dungeonCrawlBossCardPrefab;
  [SerializeField]
  private bool m_dungeonCrawlPickHeroFirst;
  [SerializeField]
  private bool m_dungeonCrawlSkipHeroSelect;
  [SerializeField]
  private bool m_dungeonCrawlMustPickShrine;
  [SerializeField]
  private bool m_dungeonCrawlSelectChapter;
  [SerializeField]
  private bool m_dungeonCrawlDisplayHeroWinsPerChapter = true;
  [SerializeField]
  private bool m_dungeonCrawlIsRetireSupported;
  [SerializeField]
  private bool m_dungeonCrawlShowBossKillCount = true;
  [SerializeField]
  private bool m_dungeonCrawlDefaultToDeckFromUpcomingScenario;
  [SerializeField]
  private bool m_ignoreHeroUnlockRequirement;
  [SerializeField]
  private int m_bossCardBackId;
  [SerializeField]
  private string m_hasSeenFeaturedModeOption;
  [SerializeField]
  private string m_hasSeenNewModePopupOption;
  [SerializeField]
  private string m_prefabShownOnComplete;
  [SerializeField]
  private int m_anomalyModeDefaultCardId;
  [SerializeField]
  private AdventureData.Adventurebooklocation m_adventureBookMapPageLocation = AdventureData.ParseAdventurebooklocationValue("Beginning");
  [SerializeField]
  private AdventureData.Adventurebooklocation m_adventureBookRewardPageLocation = AdventureData.ParseAdventurebooklocationValue("End");

  [DbfField("ADVENTURE_ID")]
  public int AdventureId => this.m_adventureId;

  [DbfField("MODE_ID")]
  public int ModeId => this.m_modeId;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("SHORT_NAME")]
  public DbfLocValue ShortName => this.m_shortName;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("SHORT_DESCRIPTION")]
  public DbfLocValue ShortDescription => this.m_shortDescription;

  [DbfField("LOCKED_SHORT_NAME")]
  public DbfLocValue LockedShortName => this.m_lockedShortName;

  [DbfField("LOCKED_DESCRIPTION")]
  public DbfLocValue LockedDescription => this.m_lockedDescription;

  [DbfField("LOCKED_SHORT_DESCRIPTION")]
  public DbfLocValue LockedShortDescription => this.m_lockedShortDescription;

  [DbfField("REQUIREMENTS_DESCRIPTION")]
  public DbfLocValue RequirementsDescription => this.m_requirementsDescription;

  [DbfField("REWARDS_DESCRIPTION")]
  public DbfLocValue RewardsDescription => this.m_rewardsDescription;

  [DbfField("COMPLETE_BANNER_TEXT")]
  public DbfLocValue CompleteBannerText => this.m_completeBannerText;

  [DbfField("SHOW_PLAYABLE_SCENARIOS_COUNT")]
  public bool ShowPlayableScenariosCount => this.m_showPlayableScenariosCount;

  [DbfField("STARTING_SUBSCENE")]
  public AdventureData.Adventuresubscene StartingSubscene => this.m_startingSubscene;

  [DbfField("SUBSCENE_TRANSITION_DIRECTION")]
  public string SubsceneTransitionDirection => this.m_subsceneTransitionDirection;

  [DbfField("ADVENTURE_SUB_DEF_PREFAB")]
  public string AdventureSubDefPrefab => this.m_adventureSubDefPrefab;

  [DbfField("GAME_SAVE_DATA_SERVER_KEY")]
  public int GameSaveDataServerKey => this.m_gameSaveDataServerKeyId;

  [DbfField("GAME_SAVE_DATA_CLIENT_KEY")]
  public int GameSaveDataClientKey => this.m_gameSaveDataClientKeyId;

  [DbfField("DUNGEON_CRAWL_SAVE_HERO_USING_HERO_DB_ID")]
  public bool DungeonCrawlSaveHeroUsingHeroDbId => this.m_dungeonCrawlSaveHeroUsingHeroDbId;

  [DbfField("DUNGEON_CRAWL_BOSS_CARD_PREFAB")]
  public string DungeonCrawlBossCardPrefab => this.m_dungeonCrawlBossCardPrefab;

  [DbfField("DUNGEON_CRAWL_PICK_HERO_FIRST")]
  public bool DungeonCrawlPickHeroFirst => this.m_dungeonCrawlPickHeroFirst;

  [DbfField("DUNGEON_CRAWL_SKIP_HERO_SELECT")]
  public bool DungeonCrawlSkipHeroSelect => this.m_dungeonCrawlSkipHeroSelect;

  [DbfField("DUNGEON_CRAWL_MUST_PICK_SHRINE")]
  public bool DungeonCrawlMustPickShrine => this.m_dungeonCrawlMustPickShrine;

  [DbfField("DUNGEON_CRAWL_SELECT_CHAPTER")]
  public bool DungeonCrawlSelectChapter => this.m_dungeonCrawlSelectChapter;

  [DbfField("DUNGEON_CRAWL_DISPLAY_HERO_WINS_PER_CHAPTER")]
  public bool DungeonCrawlDisplayHeroWinsPerChapter => this.m_dungeonCrawlDisplayHeroWinsPerChapter;

  [DbfField("DUNGEON_CRAWL_IS_RETIRE_SUPPORTED")]
  public bool DungeonCrawlIsRetireSupported => this.m_dungeonCrawlIsRetireSupported;

  [DbfField("DUNGEON_CRAWL_SHOW_BOSS_KILL_COUNT")]
  public bool DungeonCrawlShowBossKillCount => this.m_dungeonCrawlShowBossKillCount;

  [DbfField("DUNGEON_CRAWL_DEFAULT_TO_DECK_FROM_UPCOMING_SCENARIO")]
  public bool DungeonCrawlDefaultToDeckFromUpcomingScenario => this.m_dungeonCrawlDefaultToDeckFromUpcomingScenario;

  [DbfField("IGNORE_HERO_UNLOCK_REQUIREMENT")]
  public bool IgnoreHeroUnlockRequirement => this.m_ignoreHeroUnlockRequirement;

  [DbfField("BOSS_CARD_BACK")]
  public int BossCardBack => this.m_bossCardBackId;

  [DbfField("HAS_SEEN_FEATURED_MODE_OPTION")]
  public string HasSeenFeaturedModeOption => this.m_hasSeenFeaturedModeOption;

  [DbfField("HAS_SEEN_NEW_MODE_POPUP_OPTION")]
  public string HasSeenNewModePopupOption => this.m_hasSeenNewModePopupOption;

  [DbfField("PREFAB_SHOWN_ON_COMPLETE")]
  public string PrefabShownOnComplete => this.m_prefabShownOnComplete;

  [DbfField("ANOMALY_MODE_DEFAULT_CARD_ID")]
  public int AnomalyModeDefaultCardId => this.m_anomalyModeDefaultCardId;

  [DbfField("ADVENTURE_BOOK_MAP_PAGE_LOCATION")]
  public AdventureData.Adventurebooklocation AdventureBookMapPageLocation => this.m_adventureBookMapPageLocation;

  [DbfField("ADVENTURE_BOOK_REWARD_PAGE_LOCATION")]
  public AdventureData.Adventurebooklocation AdventureBookRewardPageLocation => this.m_adventureBookRewardPageLocation;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ADVENTURE_BOOK_MAP_PAGE_LOCATION":
        return (object) this.m_adventureBookMapPageLocation;
      case "ADVENTURE_BOOK_REWARD_PAGE_LOCATION":
        return (object) this.m_adventureBookRewardPageLocation;
      case "ADVENTURE_ID":
        return (object) this.m_adventureId;
      case "ADVENTURE_SUB_DEF_PREFAB":
        return (object) this.m_adventureSubDefPrefab;
      case "ANOMALY_MODE_DEFAULT_CARD_ID":
        return (object) this.m_anomalyModeDefaultCardId;
      case "BOSS_CARD_BACK":
        return (object) this.m_bossCardBackId;
      case "COMPLETE_BANNER_TEXT":
        return (object) this.m_completeBannerText;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "DUNGEON_CRAWL_BOSS_CARD_PREFAB":
        return (object) this.m_dungeonCrawlBossCardPrefab;
      case "DUNGEON_CRAWL_DEFAULT_TO_DECK_FROM_UPCOMING_SCENARIO":
        return (object) this.m_dungeonCrawlDefaultToDeckFromUpcomingScenario;
      case "DUNGEON_CRAWL_DISPLAY_HERO_WINS_PER_CHAPTER":
        return (object) this.m_dungeonCrawlDisplayHeroWinsPerChapter;
      case "DUNGEON_CRAWL_IS_RETIRE_SUPPORTED":
        return (object) this.m_dungeonCrawlIsRetireSupported;
      case "DUNGEON_CRAWL_MUST_PICK_SHRINE":
        return (object) this.m_dungeonCrawlMustPickShrine;
      case "DUNGEON_CRAWL_PICK_HERO_FIRST":
        return (object) this.m_dungeonCrawlPickHeroFirst;
      case "DUNGEON_CRAWL_SAVE_HERO_USING_HERO_DB_ID":
        return (object) this.m_dungeonCrawlSaveHeroUsingHeroDbId;
      case "DUNGEON_CRAWL_SELECT_CHAPTER":
        return (object) this.m_dungeonCrawlSelectChapter;
      case "DUNGEON_CRAWL_SHOW_BOSS_KILL_COUNT":
        return (object) this.m_dungeonCrawlShowBossKillCount;
      case "DUNGEON_CRAWL_SKIP_HERO_SELECT":
        return (object) this.m_dungeonCrawlSkipHeroSelect;
      case "GAME_SAVE_DATA_CLIENT_KEY":
        return (object) this.m_gameSaveDataClientKeyId;
      case "GAME_SAVE_DATA_SERVER_KEY":
        return (object) this.m_gameSaveDataServerKeyId;
      case "HAS_SEEN_FEATURED_MODE_OPTION":
        return (object) this.m_hasSeenFeaturedModeOption;
      case "HAS_SEEN_NEW_MODE_POPUP_OPTION":
        return (object) this.m_hasSeenNewModePopupOption;
      case "ID":
        return (object) this.ID;
      case "IGNORE_HERO_UNLOCK_REQUIREMENT":
        return (object) this.m_ignoreHeroUnlockRequirement;
      case "LOCKED_DESCRIPTION":
        return (object) this.m_lockedDescription;
      case "LOCKED_SHORT_DESCRIPTION":
        return (object) this.m_lockedShortDescription;
      case "LOCKED_SHORT_NAME":
        return (object) this.m_lockedShortName;
      case "MODE_ID":
        return (object) this.m_modeId;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "PREFAB_SHOWN_ON_COMPLETE":
        return (object) this.m_prefabShownOnComplete;
      case "REQUIREMENTS_DESCRIPTION":
        return (object) this.m_requirementsDescription;
      case "REWARDS_DESCRIPTION":
        return (object) this.m_rewardsDescription;
      case "SHORT_DESCRIPTION":
        return (object) this.m_shortDescription;
      case "SHORT_NAME":
        return (object) this.m_shortName;
      case "SHOW_PLAYABLE_SCENARIOS_COUNT":
        return (object) this.m_showPlayableScenariosCount;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "STARTING_SUBSCENE":
        return (object) this.m_startingSubscene;
      case "SUBSCENE_TRANSITION_DIRECTION":
        return (object) this.m_subsceneTransitionDirection;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 177881879:
        if (!(name == "DUNGEON_CRAWL_DISPLAY_HERO_WINS_PER_CHAPTER"))
          break;
        this.m_dungeonCrawlDisplayHeroWinsPerChapter = (bool) val;
        break;
      case 190718801:
        if (!(name == "ADVENTURE_ID"))
          break;
        this.m_adventureId = (int) val;
        break;
      case 370156007:
        if (!(name == "LOCKED_SHORT_DESCRIPTION"))
          break;
        this.m_lockedShortDescription = (DbfLocValue) val;
        break;
      case 738995217:
        if (!(name == "SUBSCENE_TRANSITION_DIRECTION"))
          break;
        this.m_subsceneTransitionDirection = (string) val;
        break;
      case 938193592:
        if (!(name == "STARTING_SUBSCENE"))
          break;
        switch (val)
        {
          case null:
            this.m_startingSubscene = AdventureData.Adventuresubscene.CHOOSER;
            return;
          case AdventureData.Adventuresubscene _:
          case int _:
            this.m_startingSubscene = (AdventureData.Adventuresubscene) val;
            return;
          case string _:
            this.m_startingSubscene = AdventureData.ParseAdventuresubsceneValue((string) val);
            return;
          default:
            return;
        }
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
      case 1525009154:
        if (!(name == "DUNGEON_CRAWL_DEFAULT_TO_DECK_FROM_UPCOMING_SCENARIO"))
          break;
        this.m_dungeonCrawlDefaultToDeckFromUpcomingScenario = (bool) val;
        break;
      case 1567271427:
        if (!(name == "HAS_SEEN_NEW_MODE_POPUP_OPTION"))
          break;
        this.m_hasSeenNewModePopupOption = (string) val;
        break;
      case 1789602423:
        if (!(name == "BOSS_CARD_BACK"))
          break;
        this.m_bossCardBackId = (int) val;
        break;
      case 1954707640:
        if (!(name == "GAME_SAVE_DATA_CLIENT_KEY"))
          break;
        this.m_gameSaveDataClientKeyId = (int) val;
        break;
      case 2011973942:
        if (!(name == "REWARDS_DESCRIPTION"))
          break;
        this.m_rewardsDescription = (DbfLocValue) val;
        break;
      case 2203352005:
        if (!(name == "ADVENTURE_BOOK_REWARD_PAGE_LOCATION"))
          break;
        switch (val)
        {
          case null:
            this.m_adventureBookRewardPageLocation = AdventureData.Adventurebooklocation.BEGINNING;
            return;
          case AdventureData.Adventurebooklocation _:
          case int _:
            this.m_adventureBookRewardPageLocation = (AdventureData.Adventurebooklocation) val;
            return;
          case string _:
            this.m_adventureBookRewardPageLocation = AdventureData.ParseAdventurebooklocationValue((string) val);
            return;
          default:
            return;
        }
      case 2336483396:
        if (!(name == "GAME_SAVE_DATA_SERVER_KEY"))
          break;
        this.m_gameSaveDataServerKeyId = (int) val;
        break;
      case 2346086551:
        if (!(name == "DUNGEON_CRAWL_SKIP_HERO_SELECT"))
          break;
        this.m_dungeonCrawlSkipHeroSelect = (bool) val;
        break;
      case 2418820992:
        if (!(name == "SHORT_DESCRIPTION"))
          break;
        this.m_shortDescription = (DbfLocValue) val;
        break;
      case 2742593543:
        if (!(name == "SHOW_PLAYABLE_SCENARIOS_COUNT"))
          break;
        this.m_showPlayableScenariosCount = (bool) val;
        break;
      case 2758586749:
        if (!(name == "IGNORE_HERO_UNLOCK_REQUIREMENT"))
          break;
        this.m_ignoreHeroUnlockRequirement = (bool) val;
        break;
      case 2794963964:
        if (!(name == "REQUIREMENTS_DESCRIPTION"))
          break;
        this.m_requirementsDescription = (DbfLocValue) val;
        break;
      case 2832700627:
        if (!(name == "PREFAB_SHOWN_ON_COMPLETE"))
          break;
        this.m_prefabShownOnComplete = (string) val;
        break;
      case 2879260603:
        if (!(name == "ANOMALY_MODE_DEFAULT_CARD_ID"))
          break;
        this.m_anomalyModeDefaultCardId = (int) val;
        break;
      case 2994298469:
        if (!(name == "DUNGEON_CRAWL_PICK_HERO_FIRST"))
          break;
        this.m_dungeonCrawlPickHeroFirst = (bool) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3030925245:
        if (!(name == "DUNGEON_CRAWL_MUST_PICK_SHRINE"))
          break;
        this.m_dungeonCrawlMustPickShrine = (bool) val;
        break;
      case 3109662305:
        if (!(name == "ADVENTURE_SUB_DEF_PREFAB"))
          break;
        this.m_adventureSubDefPrefab = (string) val;
        break;
      case 3226467965:
        if (!(name == "SHORT_NAME"))
          break;
        this.m_shortName = (DbfLocValue) val;
        break;
      case 3357947825:
        if (!(name == "DUNGEON_CRAWL_BOSS_CARD_PREFAB"))
          break;
        this.m_dungeonCrawlBossCardPrefab = (string) val;
        break;
      case 3511152538:
        if (!(name == "DUNGEON_CRAWL_SHOW_BOSS_KILL_COUNT"))
          break;
        this.m_dungeonCrawlShowBossKillCount = (bool) val;
        break;
      case 3731604237:
        if (!(name == "DUNGEON_CRAWL_IS_RETIRE_SUPPORTED"))
          break;
        this.m_dungeonCrawlIsRetireSupported = (bool) val;
        break;
      case 3793523264:
        if (!(name == "ADVENTURE_BOOK_MAP_PAGE_LOCATION"))
          break;
        switch (val)
        {
          case null:
            this.m_adventureBookMapPageLocation = AdventureData.Adventurebooklocation.BEGINNING;
            return;
          case AdventureData.Adventurebooklocation _:
          case int _:
            this.m_adventureBookMapPageLocation = (AdventureData.Adventurebooklocation) val;
            return;
          case string _:
            this.m_adventureBookMapPageLocation = AdventureData.ParseAdventurebooklocationValue((string) val);
            return;
          default:
            return;
        }
      case 3959141178:
        if (!(name == "MODE_ID"))
          break;
        this.m_modeId = (int) val;
        break;
      case 3986679374:
        if (!(name == "LOCKED_DESCRIPTION"))
          break;
        this.m_lockedDescription = (DbfLocValue) val;
        break;
      case 4059986364:
        if (!(name == "HAS_SEEN_FEATURED_MODE_OPTION"))
          break;
        this.m_hasSeenFeaturedModeOption = (string) val;
        break;
      case 4157405553:
        if (!(name == "COMPLETE_BANNER_TEXT"))
          break;
        this.m_completeBannerText = (DbfLocValue) val;
        break;
      case 4181342401:
        if (!(name == "DUNGEON_CRAWL_SAVE_HERO_USING_HERO_DB_ID"))
          break;
        this.m_dungeonCrawlSaveHeroUsingHeroDbId = (bool) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
      case 4219270968:
        if (!(name == "DUNGEON_CRAWL_SELECT_CHAPTER"))
          break;
        this.m_dungeonCrawlSelectChapter = (bool) val;
        break;
      case 4221424440:
        if (!(name == "LOCKED_SHORT_NAME"))
          break;
        this.m_lockedShortName = (DbfLocValue) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ADVENTURE_BOOK_MAP_PAGE_LOCATION":
        return typeof (AdventureData.Adventurebooklocation);
      case "ADVENTURE_BOOK_REWARD_PAGE_LOCATION":
        return typeof (AdventureData.Adventurebooklocation);
      case "ADVENTURE_ID":
        return typeof (int);
      case "ADVENTURE_SUB_DEF_PREFAB":
        return typeof (string);
      case "ANOMALY_MODE_DEFAULT_CARD_ID":
        return typeof (int);
      case "BOSS_CARD_BACK":
        return typeof (int);
      case "COMPLETE_BANNER_TEXT":
        return typeof (DbfLocValue);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "DUNGEON_CRAWL_BOSS_CARD_PREFAB":
        return typeof (string);
      case "DUNGEON_CRAWL_DEFAULT_TO_DECK_FROM_UPCOMING_SCENARIO":
        return typeof (bool);
      case "DUNGEON_CRAWL_DISPLAY_HERO_WINS_PER_CHAPTER":
        return typeof (bool);
      case "DUNGEON_CRAWL_IS_RETIRE_SUPPORTED":
        return typeof (bool);
      case "DUNGEON_CRAWL_MUST_PICK_SHRINE":
        return typeof (bool);
      case "DUNGEON_CRAWL_PICK_HERO_FIRST":
        return typeof (bool);
      case "DUNGEON_CRAWL_SAVE_HERO_USING_HERO_DB_ID":
        return typeof (bool);
      case "DUNGEON_CRAWL_SELECT_CHAPTER":
        return typeof (bool);
      case "DUNGEON_CRAWL_SHOW_BOSS_KILL_COUNT":
        return typeof (bool);
      case "DUNGEON_CRAWL_SKIP_HERO_SELECT":
        return typeof (bool);
      case "GAME_SAVE_DATA_CLIENT_KEY":
        return typeof (int);
      case "GAME_SAVE_DATA_SERVER_KEY":
        return typeof (int);
      case "HAS_SEEN_FEATURED_MODE_OPTION":
        return typeof (string);
      case "HAS_SEEN_NEW_MODE_POPUP_OPTION":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "IGNORE_HERO_UNLOCK_REQUIREMENT":
        return typeof (bool);
      case "LOCKED_DESCRIPTION":
        return typeof (DbfLocValue);
      case "LOCKED_SHORT_DESCRIPTION":
        return typeof (DbfLocValue);
      case "LOCKED_SHORT_NAME":
        return typeof (DbfLocValue);
      case "MODE_ID":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_DESC":
        return typeof (string);
      case "PREFAB_SHOWN_ON_COMPLETE":
        return typeof (string);
      case "REQUIREMENTS_DESCRIPTION":
        return typeof (DbfLocValue);
      case "REWARDS_DESCRIPTION":
        return typeof (DbfLocValue);
      case "SHORT_DESCRIPTION":
        return typeof (DbfLocValue);
      case "SHORT_NAME":
        return typeof (DbfLocValue);
      case "SHOW_PLAYABLE_SCENARIOS_COUNT":
        return typeof (bool);
      case "SORT_ORDER":
        return typeof (int);
      case "STARTING_SUBSCENE":
        return typeof (AdventureData.Adventuresubscene);
      case "SUBSCENE_TRANSITION_DIRECTION":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAdventureDataDbfRecords loadRecords = new LoadAdventureDataDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AdventureDataDbfAsset adventureDataDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AdventureDataDbfAsset)) as AdventureDataDbfAsset;
    if ((UnityEngine.Object) adventureDataDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AdventureDataDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < adventureDataDbfAsset.Records.Count; ++index)
      adventureDataDbfAsset.Records[index].StripUnusedLocales();
    records = adventureDataDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_shortName.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
    this.m_shortDescription.StripUnusedLocales();
    this.m_lockedShortName.StripUnusedLocales();
    this.m_lockedDescription.StripUnusedLocales();
    this.m_lockedShortDescription.StripUnusedLocales();
    this.m_requirementsDescription.StripUnusedLocales();
    this.m_rewardsDescription.StripUnusedLocales();
    this.m_completeBannerText.StripUnusedLocales();
  }
}
