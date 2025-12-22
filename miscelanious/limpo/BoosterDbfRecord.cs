using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoosterDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_latestExpansionOrder;
  [SerializeField]
  private int m_listDisplayOrder;
  [SerializeField]
  private int m_listDisplayOrderCategory;
  [SerializeField]
  private SpecialEventType m_openPackEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("none");
  [SerializeField]
  private SpecialEventType m_prereleaseOpenPackEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");
  [SerializeField]
  private SpecialEventType m_buyWithGoldEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");
  [SerializeField]
  private SpecialEventType m_rewardableEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("none");
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_shortName;
  [SerializeField]
  private string m_packOpeningPrefab;
  [SerializeField]
  private string m_packOpeningFxPrefab;
  [SerializeField]
  private string m_storePrefab;
  [SerializeField]
  private string m_arenaPrefab;
  [SerializeField]
  private bool m_leavingSoon;
  [SerializeField]
  private DbfLocValue m_leavingSoonText;
  [SerializeField]
  private SpecialEventType m_standardEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");
  [SerializeField]
  private bool m_showInStore;
  [SerializeField]
  private int m_rankedRewardInitialSeason;
  [SerializeField]
  private string m_questIconPath;
  [SerializeField]
  private double m_questIconOffsetX;
  [SerializeField]
  private double m_questIconOffsetY;

  [DbfField("LATEST_EXPANSION_ORDER")]
  public int LatestExpansionOrder => this.m_latestExpansionOrder;

  [DbfField("LIST_DISPLAY_ORDER")]
  public int ListDisplayOrder => this.m_listDisplayOrder;

  [DbfField("LIST_DISPLAY_ORDER_CATEGORY")]
  public int ListDisplayOrderCategory => this.m_listDisplayOrderCategory;

  [DbfField("OPEN_PACK_EVENT")]
  public SpecialEventType OpenPackEvent => this.m_openPackEvent;

  [DbfField("PRERELEASE_OPEN_PACK_EVENT")]
  public SpecialEventType PrereleaseOpenPackEvent => this.m_prereleaseOpenPackEvent;

  [DbfField("BUY_WITH_GOLD_EVENT")]
  public SpecialEventType BuyWithGoldEvent => this.m_buyWithGoldEvent;

  [DbfField("REWARDABLE_EVENT")]
  public SpecialEventType RewardableEvent => this.m_rewardableEvent;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("SHORT_NAME")]
  public DbfLocValue ShortName => this.m_shortName;

  [DbfField("PACK_OPENING_PREFAB")]
  public string PackOpeningPrefab => this.m_packOpeningPrefab;

  [DbfField("PACK_OPENING_FX_PREFAB")]
  public string PackOpeningFxPrefab => this.m_packOpeningFxPrefab;

  [DbfField("STORE_PREFAB")]
  public string StorePrefab => this.m_storePrefab;

  [DbfField("ARENA_PREFAB")]
  public string ArenaPrefab => this.m_arenaPrefab;

  [DbfField("LEAVING_SOON")]
  public bool LeavingSoon => this.m_leavingSoon;

  [DbfField("LEAVING_SOON_TEXT")]
  public DbfLocValue LeavingSoonText => this.m_leavingSoonText;

  [DbfField("STANDARD_EVENT")]
  public SpecialEventType StandardEvent => this.m_standardEvent;

  [DbfField("RANKED_REWARD_INITIAL_SEASON")]
  public int RankedRewardInitialSeason => this.m_rankedRewardInitialSeason;

  [DbfField("QUEST_ICON_PATH")]
  public string QuestIconPath => this.m_questIconPath;

  [DbfField("QUEST_ICON_OFFSET_X")]
  public double QuestIconOffsetX => this.m_questIconOffsetX;

  [DbfField("QUEST_ICON_OFFSET_Y")]
  public double QuestIconOffsetY => this.m_questIconOffsetY;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ARENA_PREFAB":
        return (object) this.m_arenaPrefab;
      case "BUY_WITH_GOLD_EVENT":
        return (object) this.m_buyWithGoldEvent;
      case "ID":
        return (object) this.ID;
      case "LATEST_EXPANSION_ORDER":
        return (object) this.m_latestExpansionOrder;
      case "LEAVING_SOON":
        return (object) this.m_leavingSoon;
      case "LEAVING_SOON_TEXT":
        return (object) this.m_leavingSoonText;
      case "LIST_DISPLAY_ORDER":
        return (object) this.m_listDisplayOrder;
      case "LIST_DISPLAY_ORDER_CATEGORY":
        return (object) this.m_listDisplayOrderCategory;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "OPEN_PACK_EVENT":
        return (object) this.m_openPackEvent;
      case "PACK_OPENING_FX_PREFAB":
        return (object) this.m_packOpeningFxPrefab;
      case "PACK_OPENING_PREFAB":
        return (object) this.m_packOpeningPrefab;
      case "PRERELEASE_OPEN_PACK_EVENT":
        return (object) this.m_prereleaseOpenPackEvent;
      case "QUEST_ICON_OFFSET_X":
        return (object) this.m_questIconOffsetX;
      case "QUEST_ICON_OFFSET_Y":
        return (object) this.m_questIconOffsetY;
      case "QUEST_ICON_PATH":
        return (object) this.m_questIconPath;
      case "RANKED_REWARD_INITIAL_SEASON":
        return (object) this.m_rankedRewardInitialSeason;
      case "REWARDABLE_EVENT":
        return (object) this.m_rewardableEvent;
      case "SHORT_NAME":
        return (object) this.m_shortName;
      case "SHOW_IN_STORE":
        return (object) this.m_showInStore;
      case "STANDARD_EVENT":
        return (object) this.m_standardEvent;
      case "STORE_PREFAB":
        return (object) this.m_storePrefab;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 5044059:
        if (!(name == "QUEST_ICON_OFFSET_Y"))
          break;
        this.m_questIconOffsetY = (double) val;
        break;
      case 207194469:
        if (!(name == "RANKED_REWARD_INITIAL_SEASON"))
          break;
        this.m_rankedRewardInitialSeason = (int) val;
        break;
      case 389673047:
        if (!(name == "LEAVING_SOON_TEXT"))
          break;
        this.m_leavingSoonText = (DbfLocValue) val;
        break;
      case 554784477:
        if (!(name == "STORE_PREFAB"))
          break;
        this.m_storePrefab = (string) val;
        break;
      case 756535192:
        if (!(name == "LIST_DISPLAY_ORDER_CATEGORY"))
          break;
        this.m_listDisplayOrderCategory = (int) val;
        break;
      case 778560520:
        if (!(name == "BUY_WITH_GOLD_EVENT"))
          break;
        this.m_buyWithGoldEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 918639297:
        if (!(name == "REWARDABLE_EVENT"))
          break;
        this.m_rewardableEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 1164573759:
        if (!(name == "LEAVING_SOON"))
          break;
        this.m_leavingSoon = (bool) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1425399448:
        if (!(name == "SHOW_IN_STORE"))
          break;
        this.m_showInStore = (bool) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1461476575:
        if (!(name == "LIST_DISPLAY_ORDER"))
          break;
        this.m_listDisplayOrder = (int) val;
        break;
      case 1998723901:
        if (!(name == "PACK_OPENING_FX_PREFAB"))
          break;
        this.m_packOpeningFxPrefab = (string) val;
        break;
      case 2191767247:
        if (!(name == "STANDARD_EVENT"))
          break;
        this.m_standardEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 2343696819:
        if (!(name == "ARENA_PREFAB"))
          break;
        this.m_arenaPrefab = (string) val;
        break;
      case 2830500322:
        if (!(name == "PACK_OPENING_PREFAB"))
          break;
        this.m_packOpeningPrefab = (string) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3226467965:
        if (!(name == "SHORT_NAME"))
          break;
        this.m_shortName = (DbfLocValue) val;
        break;
      case 3328465851:
        if (!(name == "LATEST_EXPANSION_ORDER"))
          break;
        this.m_latestExpansionOrder = (int) val;
        break;
      case 3430609171:
        if (!(name == "QUEST_ICON_PATH"))
          break;
        this.m_questIconPath = (string) val;
        break;
      case 3577915015:
        if (!(name == "PRERELEASE_OPEN_PACK_EVENT"))
          break;
        this.m_prereleaseOpenPackEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 4145311956:
        if (!(name == "OPEN_PACK_EVENT"))
          break;
        this.m_openPackEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 4283233736:
        if (!(name == "QUEST_ICON_OFFSET_X"))
          break;
        this.m_questIconOffsetX = (double) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ARENA_PREFAB":
        return typeof (string);
      case "BUY_WITH_GOLD_EVENT":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "LATEST_EXPANSION_ORDER":
        return typeof (int);
      case "LEAVING_SOON":
        return typeof (bool);
      case "LEAVING_SOON_TEXT":
        return typeof (DbfLocValue);
      case "LIST_DISPLAY_ORDER":
        return typeof (int);
      case "LIST_DISPLAY_ORDER_CATEGORY":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_DESC":
        return typeof (string);
      case "OPEN_PACK_EVENT":
        return typeof (string);
      case "PACK_OPENING_FX_PREFAB":
        return typeof (string);
      case "PACK_OPENING_PREFAB":
        return typeof (string);
      case "PRERELEASE_OPEN_PACK_EVENT":
        return typeof (string);
      case "QUEST_ICON_OFFSET_X":
        return typeof (double);
      case "QUEST_ICON_OFFSET_Y":
        return typeof (double);
      case "QUEST_ICON_PATH":
        return typeof (string);
      case "RANKED_REWARD_INITIAL_SEASON":
        return typeof (int);
      case "REWARDABLE_EVENT":
        return typeof (string);
      case "SHORT_NAME":
        return typeof (DbfLocValue);
      case "SHOW_IN_STORE":
        return typeof (bool);
      case "STANDARD_EVENT":
        return typeof (string);
      case "STORE_PREFAB":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBoosterDbfRecords loadRecords = new LoadBoosterDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BoosterDbfAsset boosterDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BoosterDbfAsset)) as BoosterDbfAsset;
    if ((UnityEngine.Object) boosterDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BoosterDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < boosterDbfAsset.Records.Count; ++index)
      boosterDbfAsset.Records[index].StripUnusedLocales();
    records = boosterDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_shortName.StripUnusedLocales();
    this.m_leavingSoonText.StripUnusedLocales();
  }
}
