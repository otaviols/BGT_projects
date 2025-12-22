using Blizzard.T5.Core;
using Hearthstone;
using Hearthstone.DungeonCrawl;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class DungeonCrawlSubDef_VOLines : MonoBehaviour
{
  [CustomEditField(Sections = "Defaults", T = EditType.GAME_OBJECT)]
  public string m_DefaultQuotePrefab;
  [CustomEditField(Sections = "Defaults")]
  public float m_DefaultChanceToPlay = 1f;
  public List<DungeonCrawlSubDef_VOLines.VOEventType> m_TutorialEventTypes;
  public List<DungeonCrawlSubDef_VOLines.VOEventData> m_VOEventDataList = new List<DungeonCrawlSubDef_VOLines.VOEventData>();
  public static readonly DungeonCrawlSubDef_VOLines.VOEventType[] BOSS_REVEAL_EVENTS = new DungeonCrawlSubDef_VOLines.VOEventType[6]
  {
    DungeonCrawlSubDef_VOLines.VOEventType.BOSS_REVEAL_1,
    DungeonCrawlSubDef_VOLines.VOEventType.BOSS_REVEAL_2,
    DungeonCrawlSubDef_VOLines.VOEventType.BOSS_REVEAL_3,
    DungeonCrawlSubDef_VOLines.VOEventType.BOSS_REVEAL_4,
    DungeonCrawlSubDef_VOLines.VOEventType.BOSS_REVEAL_5,
    DungeonCrawlSubDef_VOLines.VOEventType.BOSS_REVEAL_GENERAL
  };
  public static readonly DungeonCrawlSubDef_VOLines.VOEventType[] FINAL_BOSS_LOSS_EVENTS = new DungeonCrawlSubDef_VOLines.VOEventType[3]
  {
    DungeonCrawlSubDef_VOLines.VOEventType.FINAL_BOSS_LOSS_1,
    DungeonCrawlSubDef_VOLines.VOEventType.FINAL_BOSS_LOSS_2,
    DungeonCrawlSubDef_VOLines.VOEventType.FINAL_BOSS_LOSS_GENERAL
  };
  public static readonly DungeonCrawlSubDef_VOLines.VOEventType[] OFFER_TREASURE_EVENTS = new DungeonCrawlSubDef_VOLines.VOEventType[5]
  {
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_TREASURE_1,
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_TREASURE_2,
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_TREASURE_3,
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_TREASURE_4,
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_TREASURE_GENERAL
  };
  public static readonly DungeonCrawlSubDef_VOLines.VOEventType[] OFFER_LOOT_PACKS_EVENTS = new DungeonCrawlSubDef_VOLines.VOEventType[2]
  {
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_LOOT_PACKS_1,
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_LOOT_PACKS_2
  };
  public static readonly DungeonCrawlSubDef_VOLines.VOEventType[] OFFER_HERO_POWER_EVENTS = new DungeonCrawlSubDef_VOLines.VOEventType[1]
  {
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_HERO_POWER_1
  };
  public static readonly DungeonCrawlSubDef_VOLines.VOEventType[] OFFER_DECK_EVENTS = new DungeonCrawlSubDef_VOLines.VOEventType[1]
  {
    DungeonCrawlSubDef_VOLines.VOEventType.OFFER_DECK_1
  };
  public static readonly DungeonCrawlSubDef_VOLines.VOEventType[] WING_COMPLETE_EVENTS = new DungeonCrawlSubDef_VOLines.VOEventType[5]
  {
    DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_FIRST_WING,
    DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_SECOND_WING,
    DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_THIRD_WING,
    DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_FOURTH_WING,
    DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_FIFTH_WING
  };
  public static readonly DungeonCrawlSubDef_VOLines.VOEventType[] CLASS_COMPLETE_EVENTS = new DungeonCrawlSubDef_VOLines.VOEventType[3]
  {
    DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_FIRST_CLASS,
    DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_SECOND_CLASS,
    DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_THIRD_CLASS
  };
  private List<int> m_sortedHeroDbIds = new List<int>();
  private bool m_isWingVO;
  private Map<DungeonCrawlSubDef_VOLines.VOEventType, Map<int, Map<int, DungeonCrawlSubDef_VOLines.VOEventData>>> m_VOEventDataMap = new Map<DungeonCrawlSubDef_VOLines.VOEventType, Map<int, Map<int, DungeonCrawlSubDef_VOLines.VOEventData>>>();
  private Map<DungeonCrawlSubDef_VOLines.VOEventType, List<int>> m_VOTutorialEventRefIdMap = new Map<DungeonCrawlSubDef_VOLines.VOEventType, List<int>>();

  private void Awake()
  {
    this.m_isWingVO = (UnityEngine.Object) this.GetComponent<AdventureWingDef>() != (UnityEngine.Object) null;
    if (!this.m_isWingVO || this.m_TutorialEventTypes.Count <= 0)
      return;
    Debug.LogErrorFormat("Tutorial VO events on wing defs ({0}) are not supported and they will not be considered when deciding to play a VO line.", (object) this.gameObject.name);
    this.m_TutorialEventTypes.Clear();
  }

  private void Start()
  {
    foreach (DungeonCrawlSubDef_VOLines.VOEventData voEventData in this.m_VOEventDataList)
    {
      if (!this.m_VOEventDataMap.ContainsKey(voEventData.m_EventType))
        this.m_VOEventDataMap.Add(voEventData.m_EventType, new Map<int, Map<int, DungeonCrawlSubDef_VOLines.VOEventData>>());
      if (!this.m_VOEventDataMap[voEventData.m_EventType].ContainsKey(voEventData.m_HeroCardID))
        this.m_VOEventDataMap[voEventData.m_EventType].Add(voEventData.m_HeroCardID, new Map<int, DungeonCrawlSubDef_VOLines.VOEventData>());
      if (this.m_VOEventDataMap[voEventData.m_EventType][voEventData.m_HeroCardID].ContainsKey(voEventData.m_AssociatedCardID))
      {
        Debug.LogWarningFormat("DungeonCrawlSubDef_VOLines - Tried to add AssociatedCardID ({0}) with HeroCardID ({1}) for VOEventType ({2}) twice to the m_VOEventDataList. Using latest...", (object) voEventData.m_AssociatedCardID, (object) voEventData.m_HeroCardID, (object) voEventData.m_EventType);
        this.m_VOEventDataMap[voEventData.m_EventType][voEventData.m_HeroCardID][voEventData.m_AssociatedCardID] = voEventData;
      }
      else
        this.m_VOEventDataMap[voEventData.m_EventType][voEventData.m_HeroCardID].Add(voEventData.m_AssociatedCardID, voEventData);
    }
    foreach (DungeonCrawlSubDef_VOLines.VOEventType tutorialEventType in this.m_TutorialEventTypes)
    {
      if (this.m_VOEventDataMap.ContainsKey(tutorialEventType) && this.m_VOEventDataMap[tutorialEventType].ContainsKey(0))
      {
        if (this.m_VOTutorialEventRefIdMap.ContainsKey(tutorialEventType))
          Debug.LogWarningFormat("DungeonCrawlSubDef_VOLines - Tried to add VOEventType ({0}) twice to the m_VOTutorialEventRefIdMap for {1}. Skipping...", (object) tutorialEventType, (object) this.gameObject.name);
        else
          this.m_VOTutorialEventRefIdMap.Add(tutorialEventType, this.m_VOEventDataMap[tutorialEventType][0].Keys.ToList<int>());
      }
    }
  }

  private static AdventureModeDbId GetModeBasedOnCurrentScene()
  {
    AdventureModeDbId modeId = AdventureModeDbId.DUNGEON_CRAWL;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE)
    {
      AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
      modeId = GameUtils.GetNormalModeFromHeroicMode(selectedMode);
      if (GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == AdventureConfig.Get().GetSelectedAdventure() && (AdventureModeDbId) r.ModeId == modeId)) == null)
        modeId = selectedMode;
    }
    return modeId;
  }

  public DungeonCrawlSubDef_VOLines.VOEventData GetVOEventData(
    DungeonCrawlSubDef_VOLines.VOEventType voEventType,
    int heroDbId,
    int referenceID = 0)
  {
    if (!this.m_VOEventDataMap.ContainsKey(voEventType))
      return (DungeonCrawlSubDef_VOLines.VOEventData) null;
    int key = 0;
    if (this.m_VOEventDataMap[voEventType].ContainsKey(heroDbId) && this.m_VOEventDataMap[voEventType][heroDbId].ContainsKey(referenceID))
      key = heroDbId;
    return !this.m_VOEventDataMap[voEventType].ContainsKey(key) || !this.m_VOEventDataMap[voEventType][key].ContainsKey(referenceID) ? (DungeonCrawlSubDef_VOLines.VOEventData) null : this.m_VOEventDataMap[voEventType][key][referenceID];
  }

  private static DungeonCrawlSubDef_VOLines GetAdventureModeVOLines(
    AdventureDbId adventureId)
  {
    AdventureModeDbId basedOnCurrentScene = DungeonCrawlSubDef_VOLines.GetModeBasedOnCurrentScene();
    AdventureDef adventureDef;
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.ADVENTURE:
        AdventureScene adventureScene = AdventureScene.Get();
        if ((UnityEngine.Object) adventureScene == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        adventureDef = adventureScene.GetAdventureDef(adventureId);
        break;
      case SceneMgr.Mode.TAVERN_BRAWL:
        TavernBrawlDisplay tavernBrawlDisplay = TavernBrawlDisplay.Get();
        if ((UnityEngine.Object) tavernBrawlDisplay == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        adventureDef = tavernBrawlDisplay.GetAdventureDef(adventureId);
        break;
      case SceneMgr.Mode.PVP_DUNGEON_RUN:
        PvPDungeonRunScene pdungeonRunScene = PvPDungeonRunScene.Get();
        if ((UnityEngine.Object) pdungeonRunScene == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        adventureDef = pdungeonRunScene.GetAdventureDef(adventureId);
        break;
      default:
        return (DungeonCrawlSubDef_VOLines) null;
    }
    if ((UnityEngine.Object) adventureDef == (UnityEngine.Object) null)
    {
      Debug.LogErrorFormat("No AdventureDef for AdventureDbId {0}!", (object) adventureId);
      return (DungeonCrawlSubDef_VOLines) null;
    }
    AdventureSubDef subDef = adventureDef.GetSubDef(basedOnCurrentScene);
    if (!((UnityEngine.Object) subDef == (UnityEngine.Object) null))
      return subDef.GetComponent<DungeonCrawlSubDef_VOLines>();
    Debug.LogErrorFormat("No AdventureSubDef for AdventureDbId {0} and AdventureModeDbId {1}!", (object) adventureId, (object) basedOnCurrentScene);
    return (DungeonCrawlSubDef_VOLines) null;
  }

  private static DungeonCrawlSubDef_VOLines GetAdventureWingVOLines(
    WingDbId wingId)
  {
    AdventureWingDef adventureWingDef;
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.ADVENTURE:
        AdventureScene adventureScene = AdventureScene.Get();
        if ((UnityEngine.Object) adventureScene == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        adventureWingDef = adventureScene.GetWingDef(wingId);
        if ((UnityEngine.Object) adventureWingDef == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        break;
      case SceneMgr.Mode.TAVERN_BRAWL:
        TavernBrawlDisplay tavernBrawlDisplay = TavernBrawlDisplay.Get();
        if ((UnityEngine.Object) tavernBrawlDisplay == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        adventureWingDef = tavernBrawlDisplay.GetAdventureWingDef(wingId);
        if ((UnityEngine.Object) adventureWingDef == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        break;
      case SceneMgr.Mode.PVP_DUNGEON_RUN:
        PvPDungeonRunScene pdungeonRunScene = PvPDungeonRunScene.Get();
        if ((UnityEngine.Object) pdungeonRunScene == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        adventureWingDef = pdungeonRunScene.GetWingDef(wingId);
        if ((UnityEngine.Object) adventureWingDef == (UnityEngine.Object) null)
          return (DungeonCrawlSubDef_VOLines) null;
        break;
      default:
        return (DungeonCrawlSubDef_VOLines) null;
    }
    return adventureWingDef.GetComponent<DungeonCrawlSubDef_VOLines>();
  }

  private static bool HasEventDataBeenSeen(
    AdventureDbId adventureId,
    WingDbId wingId,
    DungeonCrawlSubDef_VOLines.VOEventData eventData,
    bool isWingVO)
  {
    AdventureModeDbId basedOnCurrentScene = DungeonCrawlSubDef_VOLines.GetModeBasedOnCurrentScene();
    if (eventData == null)
      return false;
    GameSaveKeyId saveDataClientKey = (GameSaveKeyId) GameUtils.GetAdventureDataRecord((int) adventureId, (int) basedOnCurrentScene).GameSaveDataClientKey;
    GameSaveKeySubkeyId fromHasSeenOption = DungeonCrawlSubDef_VOLines.GetSubkeyFromHasSeenOption(eventData.m_EventSeenOption);
    List<long> values;
    if (!GameSaveDataManager.Get().GetSubkeyValue(saveDataClientKey, fromHasSeenOption, out values))
      return false;
    int index = 0;
    if (isWingVO)
    {
      WingDbfRecord record = GameDbf.Wing.GetRecord((int) wingId);
      index = record != null ? GameUtils.GetSortedWingUnlockIndex(record) + 1 : 0;
    }
    return values.Count > index && (values[index] & (long) DungeonCrawlSubDef_VOLines.GetGSDFlagFromHeroCardDbID(adventureId, eventData.m_HeroCardID)) != 0L;
  }

  private bool IsEventDataValid(
    AdventureDbId adventureId,
    WingDbId wingId,
    int heroDbId,
    DungeonCrawlSubDef_VOLines.VOEventData eventData)
  {
    AdventureModeDbId basedOnCurrentScene = DungeonCrawlSubDef_VOLines.GetModeBasedOnCurrentScene();
    if (eventData == null || DungeonCrawlSubDef_VOLines.HasEventDataBeenSeen(adventureId, wingId, eventData, this.m_isWingVO))
      return false;
    if (eventData.m_MinimumRequiredBossesDefeated > 0)
    {
      AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventureId, (int) basedOnCurrentScene);
      if (adventureDataRecord.GameSaveDataServerKey == 0)
      {
        Debug.LogWarningFormat("DungeonCrawlSubDef_VOLines - Event {0} has MinimumRequiredBossesDefeated set, but Adventure {1} Wing {2} has no GameSaveDataServerKey!", (object) eventData.m_EventType, (object) adventureId, (object) wingId);
        return false;
      }
      GameSaveKeyId saveDataServerKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey;
      List<long> values;
      GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSSES_DEFEATED, out values);
      int num = values != null ? values.Count : 0;
      if (!DungeonCrawlUtil.IsDungeonRunActive(saveDataServerKey) || num < eventData.m_MinimumRequiredBossesDefeated)
        return false;
    }
    if (!this.m_isWingVO && !this.IsEventPartOfTutorial(eventData.m_EventType, heroDbId) && !this.IsVOEventTutorialComplete(adventureId))
      return false;
    bool flag = eventData.m_MultiQuoteVO.Count == 0 && eventData.m_RandomQuoteVO.Count == 0;
    return ((!string.IsNullOrEmpty(eventData.m_QuotePrefab) ? 0 : (string.IsNullOrEmpty(this.m_DefaultQuotePrefab) ? 1 : 0)) & (flag ? 1 : 0)) == 0 && !(string.IsNullOrEmpty(eventData.m_QuoteVOSoundPrefab) & flag);
  }

  private bool IsEventPartOfTutorial(DungeonCrawlSubDef_VOLines.VOEventType eventType, int heroDbId) => heroDbId <= 0 && !this.m_isWingVO && this.m_TutorialEventTypes.Contains(eventType);

  private bool IsVOEventTutorialComplete(AdventureDbId adventureId)
  {
    if (this.m_isWingVO)
      return true;
    int heroDbId = 0;
    foreach (DungeonCrawlSubDef_VOLines.VOEventType tutorialEventType in this.m_TutorialEventTypes)
    {
      if (!this.m_VOTutorialEventRefIdMap.ContainsKey(tutorialEventType))
      {
        Debug.LogWarningFormat("DungeonCrawlSubDef_VOLines.IsVOEventTutorialComplete - TutorialEventType ({0}) in Adventure ({1}) was not found in the Ref ID map in {3}. Ensure that this event does not require a specific hero since hero specific tutorial events are not supported. Ignoring...", (object) tutorialEventType, (object) adventureId, (object) this.gameObject.name);
      }
      else
      {
        foreach (int referenceID in this.m_VOTutorialEventRefIdMap[tutorialEventType])
        {
          DungeonCrawlSubDef_VOLines.VOEventData voEventData = this.GetVOEventData(tutorialEventType, heroDbId, referenceID);
          if (!DungeonCrawlSubDef_VOLines.HasEventDataBeenSeen(adventureId, WingDbId.INVALID, voEventData, this.m_isWingVO))
            return false;
        }
      }
    }
    return true;
  }

  public static bool PlayVOLine(
    AdventureDbId adventureId,
    WingDbId wingId,
    int heroDbId,
    DungeonCrawlSubDef_VOLines.VOEventType voEvent,
    int referenceID = 0,
    bool allowRepeatDuringSession = true)
  {
    return DungeonCrawlSubDef_VOLines.PlayVOLine(adventureId, wingId, heroDbId, new DungeonCrawlSubDef_VOLines.VOEventType[1]
    {
      voEvent
    }, referenceID, (allowRepeatDuringSession ? 1 : 0) != 0);
  }

  public static bool PlayVOLine(
    AdventureDbId adventureId,
    WingDbId wingId,
    int heroDbId,
    DungeonCrawlSubDef_VOLines.VOEventType[] voEvents,
    int referenceID = 0,
    bool allowRepeatDuringSession = true)
  {
    AdventureModeDbId basedOnCurrentScene = DungeonCrawlSubDef_VOLines.GetModeBasedOnCurrentScene();
    DungeonCrawlSubDef_VOLines.VOData nextValidVoData = DungeonCrawlSubDef_VOLines.GetNextValidVOData(adventureId, wingId, heroDbId, voEvents, referenceID);
    DungeonCrawlSubDef_VOLines voLines = nextValidVoData.m_VOLines;
    DungeonCrawlSubDef_VOLines.VOEventData eventData = nextValidVoData.m_EventData;
    if ((UnityEngine.Object) voLines == (UnityEngine.Object) null)
    {
      Debug.LogErrorFormat("No DungeonCrawlSubDef_VOLines Component found on AdventureDbId {0}'s AdventureSubDef or on WingDbId {1}'s AdventureWingSubDef!", (object) adventureId, (object) wingId);
      return false;
    }
    if (eventData == null || !DungeonCrawlSubDef_VOLines.EventConstraintsMet(eventData))
      return false;
    double num1 = (double) UnityEngine.Random.Range(0.0f, 1f);
    float num2 = eventData.m_ChanceToPlay;
    if ((double) num2 < 0.0)
      num2 = voLines.m_DefaultChanceToPlay;
    if ((double) num2 < 1.0 && (double) Cheats.VOChanceOverride >= 0.0 && HearthstoneApplication.IsInternal())
      num2 = Cheats.VOChanceOverride;
    double num3 = (double) num2;
    if (num1 > num3)
      return false;
    string str = string.IsNullOrEmpty(eventData.m_QuotePrefab) ? voLines.m_DefaultQuotePrefab : eventData.m_QuotePrefab;
    if (eventData.m_MultiQuoteVO.Count > 0 && !string.IsNullOrEmpty(eventData.m_QuoteVOSoundPrefab))
      Debug.LogErrorFormat("Playing a quote for eventType {0} and have both MultiQuotes and a VO Sound prefab.  Playing MultiQuotes", (object) eventData.m_EventType);
    else if (eventData.m_RandomQuoteVO.Count > 0 && !string.IsNullOrEmpty(eventData.m_QuoteVOSoundPrefab))
      Debug.LogErrorFormat("Playing a quote for eventType {0} and have both RandomQuotes and a VO Sound prefab.  Playing RandomQuotes", (object) eventData.m_EventType);
    if (eventData.m_MultiQuoteVO.Count > 0)
      DungeonCrawlSubDef_VOLines.PlayMultiLines(0, eventData.m_MultiQuoteVO.ToArray(), str, eventData.m_QuotePosition, eventData.m_BlockAllOtherInput, allowRepeatDuringSession);
    else if (eventData.m_RandomQuoteVO.Count > 0)
    {
      DungeonCrawlSubDef_VOLines.PlayRandomLine(eventData.m_RandomQuoteVO.ToArray(), str, eventData.m_QuotePosition, eventData.m_BlockAllOtherInput);
    }
    else
    {
      string legacyAssetName = new AssetReference(eventData.m_QuoteVOSoundPrefab).GetLegacyAssetName();
      NotificationManager.Get().CreateCharacterQuote(str, eventData.m_QuotePosition, GameStrings.Get(legacyAssetName), eventData.m_QuoteVOSoundPrefab, allowRepeatDuringSession, blockAllOtherInput: eventData.m_BlockAllOtherInput);
    }
    GameSaveKeyId saveDataClientKey = (GameSaveKeyId) GameUtils.GetAdventureDataRecord((int) adventureId, (int) basedOnCurrentScene).GameSaveDataClientKey;
    GameSaveKeySubkeyId fromHasSeenOption = DungeonCrawlSubDef_VOLines.GetSubkeyFromHasSeenOption(eventData.m_EventSeenOption);
    if (fromHasSeenOption != GameSaveKeySubkeyId.INVALID)
    {
      List<long> values = (List<long>) null;
      if (!GameSaveDataManager.Get().GetSubkeyValue(saveDataClientKey, fromHasSeenOption, out values))
        values = new List<long>() { 0L };
      long fromHeroCardDbId = (long) DungeonCrawlSubDef_VOLines.GetGSDFlagFromHeroCardDbID(adventureId, eventData.m_HeroCardID);
      int index = 0;
      if (voLines.m_isWingVO)
      {
        WingDbfRecord record = GameDbf.Wing.GetRecord((int) wingId);
        int wingsInAdventure = GameUtils.GetNumWingsInAdventure(adventureId);
        index = record != null ? GameUtils.GetSortedWingUnlockIndex(record) + 1 : 0;
        if (values.Count < wingsInAdventure)
          values.AddRange(Enumerable.Repeat<long>(0L, wingsInAdventure + 1 - values.Count));
      }
      values[index] |= fromHeroCardDbId;
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(saveDataClientKey, fromHasSeenOption, values.ToArray()));
    }
    return true;
  }

  private static void PlayMultiLines(
    int index,
    DungeonCrawlSubDef_VOLines.CharacterQuoteVOObject[] lines,
    string prefab,
    Vector3 quotePosition,
    bool blockAllOtherInput,
    bool allowRepeatDuringSession)
  {
    Action<int> action1 = (Action<int>) null;
    if (index < lines.Length - 1)
      action1 = (Action<int>) (groupId => DungeonCrawlSubDef_VOLines.PlayMultiLines(index + 1, lines, prefab, quotePosition, blockAllOtherInput, allowRepeatDuringSession));
    string legacyAssetName = new AssetReference(lines[index].SoundPrefab).GetLegacyAssetName();
    string str = !string.IsNullOrEmpty(lines[index].CharacterPrefab) ? lines[index].CharacterPrefab : prefab;
    NotificationManager notificationManager = NotificationManager.Get();
    string prefabPath = str;
    Vector3 position = quotePosition;
    string text = GameStrings.Get(legacyAssetName);
    string soundPrefab = lines[index].SoundPrefab;
    Action<int> action2 = action1;
    int num1 = allowRepeatDuringSession ? 1 : 0;
    Action<int> finishCallback = action2;
    int num2 = blockAllOtherInput ? 1 : 0;
    notificationManager.CreateCharacterQuote(prefabPath, position, text, soundPrefab, num1 != 0, finishCallback: finishCallback, blockAllOtherInput: (num2 != 0));
  }

  private static void PlayRandomLine(
    DungeonCrawlSubDef_VOLines.CharacterQuoteVOObject[] lines,
    string prefab,
    Vector3 quotePosition,
    bool blockAllOtherInput)
  {
    int index = UnityEngine.Random.Range(0, lines.Length);
    string legacyAssetName = new AssetReference(lines[index].SoundPrefab).GetLegacyAssetName();
    string prefabPath = !string.IsNullOrEmpty(lines[index].CharacterPrefab) ? lines[index].CharacterPrefab : prefab;
    NotificationManager.Get().CreateCharacterQuote(prefabPath, quotePosition, GameStrings.Get(legacyAssetName), lines[index].SoundPrefab, blockAllOtherInput: blockAllOtherInput);
  }

  public static DungeonCrawlSubDef_VOLines.VOEventType GetNextValidEventType(
    AdventureDbId adventureId,
    WingDbId wingId,
    int heroDbId,
    DungeonCrawlSubDef_VOLines.VOEventType[] events,
    int referenceID = 0)
  {
    DungeonCrawlSubDef_VOLines.VOData nextValidVoData = DungeonCrawlSubDef_VOLines.GetNextValidVOData(adventureId, wingId, heroDbId, events, referenceID);
    return nextValidVoData.m_EventData != null ? nextValidVoData.m_EventData.m_EventType : DungeonCrawlSubDef_VOLines.VOEventType.INVALID;
  }

  private static DungeonCrawlSubDef_VOLines.VOData GetNextValidVOData(
    AdventureDbId adventureId,
    WingDbId wingId,
    int heroDbId,
    DungeonCrawlSubDef_VOLines.VOEventType[] events,
    int referenceID = 0)
  {
    DungeonCrawlSubDef_VOLines.VOData nextValidVoData = new DungeonCrawlSubDef_VOLines.VOData();
    DungeonCrawlSubDef_VOLines adventureWingVoLines = DungeonCrawlSubDef_VOLines.GetAdventureWingVOLines(wingId);
    DungeonCrawlSubDef_VOLines adventureModeVoLines = DungeonCrawlSubDef_VOLines.GetAdventureModeVOLines(adventureId);
    bool flag = (UnityEngine.Object) adventureModeVoLines == (UnityEngine.Object) null || adventureModeVoLines.IsVOEventTutorialComplete(adventureId);
    nextValidVoData.m_VOLines = adventureWingVoLines;
    if ((UnityEngine.Object) nextValidVoData.m_VOLines != (UnityEngine.Object) null & flag)
    {
      nextValidVoData.m_EventData = nextValidVoData.m_VOLines.GetNextValidEventData(adventureId, wingId, heroDbId, events, referenceID);
      if (nextValidVoData.m_EventData != null && nextValidVoData.m_EventData.m_EventType != DungeonCrawlSubDef_VOLines.VOEventType.INVALID)
        return nextValidVoData;
    }
    nextValidVoData.m_VOLines = adventureModeVoLines;
    if ((UnityEngine.Object) nextValidVoData.m_VOLines != (UnityEngine.Object) null)
    {
      nextValidVoData.m_EventData = nextValidVoData.m_VOLines.GetNextValidEventData(adventureId, wingId, heroDbId, events, referenceID);
      if (nextValidVoData.m_EventData != null)
      {
        int eventType = (int) nextValidVoData.m_EventData.m_EventType;
        return nextValidVoData;
      }
    }
    return nextValidVoData;
  }

  private DungeonCrawlSubDef_VOLines.VOEventData GetNextValidEventData(
    AdventureDbId adventureId,
    WingDbId wingId,
    int heroDbId,
    DungeonCrawlSubDef_VOLines.VOEventType[] events,
    int referenceID = 0)
  {
    List<int> intList = new List<int>() { heroDbId, 0 };
    foreach (DungeonCrawlSubDef_VOLines.VOEventType voEventType in events)
    {
      foreach (int heroDbId1 in intList)
      {
        DungeonCrawlSubDef_VOLines.VOEventData eventData = this.GetVOEventData(voEventType, heroDbId1, referenceID) ?? this.GetVOEventData(voEventType, heroDbId1);
        if (this.IsEventDataValid(adventureId, wingId, heroDbId1, eventData))
          return eventData;
      }
    }
    return (DungeonCrawlSubDef_VOLines.VOEventData) null;
  }

  private static GameSaveKeySubkeyId GetSubkeyFromHasSeenOption(
    DungeonCrawlSubDef_VOLines.HasSeenDataGameSaveSubkey hasSeenSubkey)
  {
    if (!Enum.IsDefined(typeof (DungeonCrawlSubDef_VOLines.HasSeenDataGameSaveSubkey), (object) hasSeenSubkey))
    {
      Debug.LogErrorFormat("HasSeenDataGameSaveSubkey {0} is not a valid value!", (object) hasSeenSubkey);
      return GameSaveKeySubkeyId.INVALID;
    }
    string str = hasSeenSubkey.ToString();
    object fromHasSeenOption = Enum.Parse(typeof (GameSaveKeySubkeyId), str, true);
    if (fromHasSeenOption != null)
      return (GameSaveKeySubkeyId) fromHasSeenOption;
    Debug.LogError((object) ("Unable to parse subkey from Dungeon Crawl HasSeenDataGameSaveSubkey: " + str));
    return GameSaveKeySubkeyId.INVALID;
  }

  private static int GetGSDFlagFromHeroCardDbID(AdventureDbId adventureId, int heroDbId)
  {
    DungeonCrawlSubDef_VOLines adventureModeVoLines = DungeonCrawlSubDef_VOLines.GetAdventureModeVOLines(adventureId);
    if ((UnityEngine.Object) adventureModeVoLines == (UnityEngine.Object) null)
    {
      Debug.LogErrorFormat("GetGSDFlagFromHeroCardDbID - unable to get the VO Lines component from the Adventure ({0}) sub def.", (object) adventureId);
      return -1;
    }
    List<int> sortedHeroDbIds = adventureModeVoLines.m_sortedHeroDbIds;
    if (sortedHeroDbIds.Count <= 0)
    {
      List<AdventureGuestHeroesDbfRecord> records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureId));
      records.Sort((Comparison<AdventureGuestHeroesDbfRecord>) ((a, b) => a.ID - b.ID));
      foreach (AdventureGuestHeroesDbfRecord guestHeroesDbfRecord in records)
      {
        GuestHeroDbfRecord record = GameDbf.GuestHero.GetRecord(guestHeroesDbfRecord.GuestHeroId);
        if (record != null)
          sortedHeroDbIds.Add(record.CardId);
      }
    }
    return 1 << sortedHeroDbIds.IndexOf(heroDbId) + 1;
  }

  private static bool EventConstraintsMet(DungeonCrawlSubDef_VOLines.VOEventData eventData)
  {
    if (eventData.m_QuoteConstraints.Count == 0)
      return true;
    foreach (DungeonCrawlSubDef_VOLines.VOConstraintObject quoteConstraint in eventData.m_QuoteConstraints)
    {
      switch (quoteConstraint.Constraint)
      {
        case DungeonCrawlSubDef_VOLines.VOConstraintObject.ConstraintType.WingIsUnlocked:
        case DungeonCrawlSubDef_VOLines.VOConstraintObject.ConstraintType.WingIsLocked:
        case DungeonCrawlSubDef_VOLines.VOConstraintObject.ConstraintType.WingIsCompleted:
          if (!DungeonCrawlSubDef_VOLines.EvaluateWingConstraint(quoteConstraint))
            return false;
          continue;
        default:
          Debug.LogWarningFormat("DungeonCrawlSubDef_VOLines.EventConstraintsMet did not have a case to handle the passed constraint of type {0}.", (object) quoteConstraint.Constraint.ToString());
          continue;
      }
    }
    return true;
  }

  private static bool EvaluateWingConstraint(
    DungeonCrawlSubDef_VOLines.VOConstraintObject quoteConstraint)
  {
    WingDbfRecord record = GameDbf.Wing.GetRecord(quoteConstraint.Value);
    if (record == null)
    {
      Debug.LogWarningFormat("DungeonCrawlSubDef_VOLines.EvaluateWingConstraint was called with invalid wing ID: {0}.", (object) quoteConstraint.Value);
      return false;
    }
    AdventureChapterState adventureChapterState = AdventureProgressMgr.Get().AdventureBookChapterStateForWing(record, AdventureConfig.Get().GetSelectedMode());
    switch (quoteConstraint.Constraint)
    {
      case DungeonCrawlSubDef_VOLines.VOConstraintObject.ConstraintType.WingIsUnlocked:
        return adventureChapterState == AdventureChapterState.UNLOCKED;
      case DungeonCrawlSubDef_VOLines.VOConstraintObject.ConstraintType.WingIsLocked:
        return adventureChapterState == AdventureChapterState.LOCKED;
      case DungeonCrawlSubDef_VOLines.VOConstraintObject.ConstraintType.WingIsCompleted:
        return adventureChapterState == AdventureChapterState.COMPLETED;
      default:
        Debug.LogWarningFormat("DungeonCrawlSubDef_VOLines.EvaluateWingConstraint was called with unsupported Constraint Type: {0}.", (object) quoteConstraint.Constraint.ToString());
        return false;
    }
  }

  public enum VOEventType
  {
    INVALID,
    CHARACTER_SELECT,
    BOSS_REVEAL_1,
    BOSS_REVEAL_2,
    BOSS_REVEAL_3,
    BOSS_REVEAL_GENERAL,
    OFFER_TREASURE_1,
    OFFER_TREASURE_GENERAL,
    TAKE_TREASURE_GENERAL,
    OFFER_LOOT_PACKS_1,
    OFFER_LOOT_PACKS_2,
    WELCOME_BANNER,
    COMPLETE_ALL_CLASSES_FIRST_TIME,
    COMPLETE_ALL_CLASSES,
    COMPLETE_FIRST_CLASS,
    COMPLETE_SECOND_CLASS,
    COMPLETE_THIRD_CLASS,
    OFFER_TREASURE_2,
    OFFER_TREASURE_3,
    OFFER_TREASURE_4,
    OFFER_HERO_POWER_1,
    OFFER_DECK_1,
    BOSS_REVEAL_4,
    BOSS_REVEAL_5,
    BOOK_REVEAL,
    BOOK_REVEAL_HEROIC,
    WING_UNLOCK,
    COMPLETE_ALL_WINGS,
    COMPLETE_ALL_WINGS_HEROIC,
    ANOMALY_UNLOCK,
    REWARD_PAGE_REVEAL,
    FINAL_BOSS_REVEAL,
    FINAL_BOSS_LOSS_1,
    FINAL_BOSS_LOSS_2,
    FINAL_BOSS_LOSS_GENERAL,
    COMPLETE_FIRST_WING,
    COMPLETE_SECOND_WING,
    COMPLETE_THIRD_WING,
    COMPLETE_FOURTH_WING,
    COMPLETE_FIFTH_WING,
    BOSS_LOSS_1,
    WING_COMPLETE_GENERAL,
    CHAPTER_PAGE,
    BOSS_LOSS_1_SECOND_BOOK_SECTION,
    COMPLETE_ALL_WINGS_SECOND_BOOK_SECTION,
    COMPLETE_ALL_WINGS_SECOND_BOOK_SECTION_HEROIC,
    CALL_TO_ACTION,
  }

  public enum HasSeenDataGameSaveSubkey
  {
    INVALID = 0,
    DUNGEON_CRAWL_HAS_SEEN_CHARACTER_SELECT_VO = 3,
    DUNGEON_CRAWL_HAS_SEEN_WELCOME_BANNER_VO = 4,
    DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_1_VO = 5,
    DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_2_VO = 6,
    DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_3_VO = 7,
    DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_1_VO = 8,
    DUNGEON_CRAWL_HAS_SEEN_OFFER_LOOT_PACKS_1_VO = 9,
    DUNGEON_CRAWL_HAS_SEEN_OFFER_LOOT_PACKS_2_VO = 10, // 0x0000000A
    DUNGEON_CRAWL_HAS_SEEN_IN_GAME_WIN_VO = 12, // 0x0000000C
    DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_VO = 13, // 0x0000000D
    DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_1_VO = 14, // 0x0000000E
    DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_2_VO = 15, // 0x0000000F
    DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_2_VO = 18, // 0x00000012
    DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_2_VO = 19, // 0x00000013
    DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_3_VO = 20, // 0x00000014
    DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_4_VO = 21, // 0x00000015
    DUNGEON_CRAWL_HAS_SEEN_OFFER_HERO_POWER_1_VO = 22, // 0x00000016
    DUNGEON_CRAWL_HAS_SEEN_OFFER_DECK_1_VO = 23, // 0x00000017
    DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_4_VO = 24, // 0x00000018
    DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_5_VO = 25, // 0x00000019
    DUNGEON_CRAWL_HAS_SEEN_BOOK_REVEAL_VO = 26, // 0x0000001A
    DUNGEON_CRAWL_HAS_SEEN_BOOK_REVEAL_HEROIC_VO = 27, // 0x0000001B
    DUNGEON_CRAWL_HAS_SEEN_WING_UNLOCK_VO = 28, // 0x0000001C
    DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_WINGS_VO = 29, // 0x0000001D
    DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_WINGS_HEROIC_VO = 30, // 0x0000001E
    DUNGEON_CRAWL_HAS_SEEN_ANOMALY_UNLOCK_VO = 31, // 0x0000001F
    DUNGEON_CRAWL_HAS_SEEN_REWARD_PAGE_REVEAL_VO = 32, // 0x00000020
    DUNGEON_CRAWL_HAS_SEEN_FINAL_BOSS_LOSS_1_VO = 33, // 0x00000021
    DUNGEON_CRAWL_HAS_SEEN_FINAL_BOSS_LOSS_2_VO = 34, // 0x00000022
    DUNGEON_CRAWL_HAS_SEEN_FINAL_BOSS_REVEAL_1_VO = 35, // 0x00000023
    DUNGEON_CRAWL_HAS_SEEN_BOSS_LOSS_1_VO = 36, // 0x00000024
    DUNGEON_CRAWL_HAS_SEEN_CHAPTER_PAGE_VO = 37, // 0x00000025
    DUNGEON_CRAWL_HAS_SEEN_BOSS_LOSS_1_SECOND_BOOK_SECTION_VO = 38, // 0x00000026
    DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_WINGS_SECOND_BOOK_SECTION_VO = 39, // 0x00000027
    DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_WINGS_SECOND_BOOK_SECTION_HEROIC_VO = 40, // 0x00000028
    DUNGEON_CRAWL_HAS_SEEN_CALL_TO_ACTION_VO = 41, // 0x00000029
  }

  [Serializable]
  public class VOEventData
  {
    public DungeonCrawlSubDef_VOLines.VOEventType m_EventType;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string m_QuotePrefab;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string m_QuoteVOSoundPrefab;
    public DungeonCrawlSubDef_VOLines.HasSeenDataGameSaveSubkey m_EventSeenOption;
    public int m_AssociatedCardID;
    public int m_HeroCardID;
    public float m_ChanceToPlay = -1f;
    public int m_MinimumRequiredBossesDefeated;
    public bool m_BlockAllOtherInput;
    public Vector3 m_QuotePosition = NotificationManager.DEFAULT_CHARACTER_POS;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public List<DungeonCrawlSubDef_VOLines.CharacterQuoteVOObject> m_MultiQuoteVO;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public List<DungeonCrawlSubDef_VOLines.CharacterQuoteVOObject> m_RandomQuoteVO;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public List<DungeonCrawlSubDef_VOLines.VOConstraintObject> m_QuoteConstraints;
  }

  [Serializable]
  public class CharacterQuoteVOObject
  {
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string SoundPrefab;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string CharacterPrefab;
  }

  [Serializable]
  public class VOConstraintObject
  {
    public DungeonCrawlSubDef_VOLines.VOConstraintObject.ConstraintType Constraint;
    public int Value;

    public enum ConstraintType
    {
      WingIsUnlocked,
      WingIsLocked,
      WingIsCompleted,
    }
  }

  private class VOData
  {
    public DungeonCrawlSubDef_VOLines m_VOLines;
    public DungeonCrawlSubDef_VOLines.VOEventData m_EventData;
  }
}
