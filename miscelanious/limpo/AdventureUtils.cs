using Hearthstone.DataModels;
using Hearthstone.DungeonCrawl;
using Hearthstone.Progression;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdventureUtils
{
  public static List<AdventureLoadoutTreasuresDbfRecord> GetLoadoutTreasuresForAdventureAndClass(
    AdventureDbId adventure,
    TAG_CLASS classId)
  {
    List<AdventureLoadoutTreasuresDbfRecord> records = GameDbf.AdventureLoadoutTreasures.GetRecords((Predicate<AdventureLoadoutTreasuresDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure && (TAG_CLASS) r.ClassId == classId));
    records.Sort((Comparison<AdventureLoadoutTreasuresDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  public static List<AdventureLoadoutTreasuresDbfRecord> GetLoadoutTreasuresForAdventureAndGuestHero(
    AdventureDbId adventure,
    int guestHeroId)
  {
    int guestHeroIdToUse = AdventureUtils.GetBaseGuestHeroIdForAdventure(adventure, guestHeroId);
    if (guestHeroIdToUse == 0)
      guestHeroIdToUse = guestHeroId;
    List<AdventureLoadoutTreasuresDbfRecord> records = GameDbf.AdventureLoadoutTreasures.GetRecords((Predicate<AdventureLoadoutTreasuresDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure && r.GuestHeroId == guestHeroIdToUse));
    records.Sort((Comparison<AdventureLoadoutTreasuresDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  public static List<AdventureHeroPowerDbfRecord> GetHeroPowersForAdventureAndClass(
    AdventureDbId adventure,
    TAG_CLASS classId)
  {
    List<AdventureHeroPowerDbfRecord> records = GameDbf.AdventureHeroPower.GetRecords((Predicate<AdventureHeroPowerDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure && (TAG_CLASS) r.ClassId == classId));
    records.Sort((Comparison<AdventureHeroPowerDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  public static List<AdventureDeckDbfRecord> GetDecksForAdventureAndClass(
    AdventureDbId adventure,
    TAG_CLASS classId)
  {
    List<AdventureDeckDbfRecord> records = GameDbf.AdventureDeck.GetRecords((Predicate<AdventureDeckDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure && (TAG_CLASS) r.ClassId == classId));
    records.Sort((Comparison<AdventureDeckDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  public static List<AdventureHeroPowerDbfRecord> GetHeroPowersForAdventureAndGuestHero(
    AdventureDbId adventure,
    int guestHeroId)
  {
    int guestHeroIdToUse = AdventureUtils.GetBaseGuestHeroIdForAdventure(adventure, guestHeroId);
    if (guestHeroIdToUse == 0)
      guestHeroIdToUse = guestHeroId;
    List<AdventureHeroPowerDbfRecord> records = GameDbf.AdventureHeroPower.GetRecords((Predicate<AdventureHeroPowerDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure && r.GuestHeroId == guestHeroIdToUse));
    records.Sort((Comparison<AdventureHeroPowerDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  public static int GetBaseGuestHeroIdForAdventure(AdventureDbId adventure, int guestHeroId)
  {
    AdventureGuestHeroesDbfRecord record = GameDbf.AdventureGuestHeroes.GetRecord((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure && r.GuestHeroRecord.ID == guestHeroId));
    return record == null ? 0 : record.BaseGuestHeroId;
  }

  public static bool AdventureHeroPowerIsUnlocked(
    GameSaveKeyId gameSaveServerKey,
    AdventureHeroPowerDbfRecord heroPowerRecord,
    out long unlockProgress,
    out bool hasUnlockCriteria)
  {
    return AdventureUtils.AdventureRewardIsUnlocked(gameSaveServerKey, (GameSaveKeySubkeyId) heroPowerRecord.UnlockGameSaveSubkey, heroPowerRecord.UnlockValue, heroPowerRecord.UnlockAchievement, out unlockProgress, out hasUnlockCriteria);
  }

  public static bool AdventureDeckIsUnlocked(
    GameSaveKeyId gameSaveServerKey,
    AdventureDeckDbfRecord deckRecord,
    out long unlockProgress,
    out bool hasUnlockCriteria)
  {
    return AdventureUtils.AdventureRewardIsUnlocked(gameSaveServerKey, (GameSaveKeySubkeyId) deckRecord.UnlockGameSaveSubkey, deckRecord.UnlockValue, 0, out unlockProgress, out hasUnlockCriteria);
  }

  public static bool AdventureTreasureIsUnlocked(
    GameSaveKeyId gameSaveServerKey,
    AdventureLoadoutTreasuresDbfRecord treasureLoadoutRecord,
    out long unlockProgress,
    out bool hasUnlockCriteria)
  {
    return AdventureUtils.AdventureRewardIsUnlocked(gameSaveServerKey, (GameSaveKeySubkeyId) treasureLoadoutRecord.UnlockGameSaveSubkey, treasureLoadoutRecord.UnlockValue, treasureLoadoutRecord.UnlockAchievement, out unlockProgress, out hasUnlockCriteria);
  }

  public static bool AdventureTreasureIsUpgraded(
    GameSaveKeyId gameSaveServerKey,
    AdventureLoadoutTreasuresDbfRecord treasureLoadoutRecord,
    out long upgradeProgress)
  {
    return AdventureUtils.AdventureRewardIsUnlocked(gameSaveServerKey, (GameSaveKeySubkeyId) treasureLoadoutRecord.UpgradeGameSaveSubkey, treasureLoadoutRecord.UpgradeValue, 0, out upgradeProgress, out bool _);
  }

  public static bool AdventureRewardIsUnlocked(
    GameSaveKeyId gameSaveServerKey,
    GameSaveKeySubkeyId unlockGameSaveSubkey,
    int unlockValue,
    int unlockAchievement,
    out long unlockProgress,
    out bool hasUnlockCriteria)
  {
    unlockProgress = 0L;
    hasUnlockCriteria = true;
    if (unlockAchievement <= 0 && unlockGameSaveSubkey <= ~GameSaveKeySubkeyId.INVALID)
    {
      hasUnlockCriteria = false;
      return true;
    }
    bool flag1 = false;
    int num = 0;
    if (unlockAchievement > 0)
    {
      flag1 = AchievementManager.Get().IsAchievementComplete(unlockAchievement);
      num = AchievementManager.Get().GetAchievementDataModel(unlockAchievement).Progress;
    }
    bool flag2 = false;
    if (unlockGameSaveSubkey > ~GameSaveKeySubkeyId.INVALID)
    {
      GameSaveDataManager.Get().GetSubkeyValue(gameSaveServerKey, unlockGameSaveSubkey, out unlockProgress);
      flag2 = unlockProgress >= (long) unlockValue;
    }
    if (unlockGameSaveSubkey > ~GameSaveKeySubkeyId.INVALID && unlockAchievement > 0)
    {
      unlockProgress += (long) num;
      return flag1 & flag2;
    }
    if (unlockAchievement > 0)
    {
      unlockProgress = (long) num;
      return flag1;
    }
    return unlockGameSaveSubkey > ~GameSaveKeySubkeyId.INVALID && flag2;
  }

  public static int GetFinalAdventureWing(
    int adventureId,
    bool excludeOwnedWings,
    bool excludeInactiveWings = false)
  {
    int num = -1;
    int finalAdventureWing = 0;
    foreach (WingDbfRecord record in GameDbf.Wing.GetRecords())
    {
      if (record.AdventureId == adventureId && record.UnlockOrder > num && (!excludeOwnedWings || !AdventureProgressMgr.Get().OwnsWing(record.ID)) && (!excludeInactiveWings || AdventureProgressMgr.IsWingEventActive(record.ID)))
      {
        num = record.UnlockOrder;
        finalAdventureWing = record.ID;
      }
    }
    return finalAdventureWing;
  }

  public static bool IsAnomalyModeAvailable(
    AdventureDbId adventureDbId,
    AdventureModeDbId modeDbId,
    WingDbId wingDbId)
  {
    return !AdventureUtils.IsAnomalyModeLocked(adventureDbId, modeDbId) && AdventureUtils.IsAnomalyModeAllowed(wingDbId);
  }

  public static bool IsAnomalyModeLocked(AdventureDbId adventureDbId, AdventureModeDbId modeDbId)
  {
    foreach (ScenarioDbfRecord record in GameDbf.Scenario.GetRecords((Predicate<ScenarioDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId && (AdventureModeDbId) r.ModeId == modeDbId && r.WingId != 0)))
    {
      if (!AdventureProgressMgr.Get().OwnsWing(record.WingId))
        return true;
    }
    return false;
  }

  public static bool IsAnomalyModeAllowed(WingDbId wingDbId)
  {
    WingDbfRecord record = GameDbf.Wing.GetRecord((int) wingDbId);
    return record != null && record.AllowsAnomaly;
  }

  public static AdventureDbId GetAdventureIdForWing(WingDbId wingDbId)
  {
    WingDbfRecord record = GameDbf.Wing.GetRecord((int) wingDbId);
    return record == null ? AdventureDbId.INVALID : (AdventureDbId) record.AdventureId;
  }

  public static bool IsProductTypeAnAdventureWing(ProductType type)
  {
    switch (type)
    {
      case ProductType.PRODUCT_TYPE_NAXX:
      case ProductType.PRODUCT_TYPE_BRM:
      case ProductType.PRODUCT_TYPE_LOE:
      case ProductType.PRODUCT_TYPE_WING:
        return true;
      default:
        return false;
    }
  }

  public static bool IsAdventureBundle(Network.Bundle bundle) => bundle.Items.Any<Network.BundleItem>((Func<Network.BundleItem, bool>) (item => AdventureUtils.IsProductTypeAnAdventureWing(item.ItemType)));

  public static bool DoesBundleIncludeWingForAdventure(
    Network.Bundle bundle,
    AdventureDbId adventure)
  {
    return bundle.Items.Any<Network.BundleItem>((Func<Network.BundleItem, bool>) (item => AdventureUtils.IsProductTypeAnAdventureWing(item.ItemType) && AdventureUtils.GetAdventureIdForWing((WingDbId) item.ProductData) == adventure));
  }

  public static bool DoesBundleIncludeWing(Network.Bundle bundle, int wingId) => bundle.Items.Any<Network.BundleItem>((Func<Network.BundleItem, bool>) (item => AdventureUtils.IsProductTypeAnAdventureWing(item.ItemType) && item.ProductData == wingId));

  public static int GetHeroCardDbIdFromClassForDungeonCrawl(
    IDungeonCrawlData dungeonCrawlData,
    TAG_CLASS cardClass)
  {
    return GameUtils.TranslateCardIdToDbId(AdventureUtils.GetHeroCardIdFromClassForDungeonCrawl(dungeonCrawlData, cardClass));
  }

  public static string GetHeroCardIdFromClassForDungeonCrawl(
    IDungeonCrawlData dungeonCrawlData,
    TAG_CLASS cardClass)
  {
    List<GuestHero> currentAdventure = dungeonCrawlData?.GetGuestHeroesForCurrentAdventure();
    if (currentAdventure == null || currentAdventure.Count <= 0)
    {
      NetCache.CardDefinition randomFavoriteHero = CollectionManager.Get().GetRandomFavoriteHero(cardClass);
      if (randomFavoriteHero != null)
        return randomFavoriteHero.Name;
      Log.Adventures.PrintError("GameUtils.GetHeroCardIdFromClassForAdventure - could not get Hero Card Id from {0}", (object) cardClass);
      return (string) null;
    }
    if (cardClass == TAG_CLASS.INVALID)
      cardClass = TAG_CLASS.NEUTRAL;
    foreach (GuestHero guestHero in currentAdventure)
    {
      if (GameUtils.GetTagClassFromCardDbId(guestHero.cardDbId) == cardClass)
      {
        CardDbfRecord record = GameDbf.Card.GetRecord(guestHero.cardDbId);
        if (record != null)
          return record.NoteMiniGuid;
      }
    }
    return (string) null;
  }

  public static void DisplayFirstChapterFreePopup(
    ChapterPageData chapterPageData,
    AdventureUtils.FirstChapterFreePopupCompleteCallback callback = null)
  {
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_ADVENTURE_ADVENTUREBOOK_DAL_FIRST_TIME_FLOW_HEADER"),
      m_text = GameStrings.Get("GLUE_ADVENTURE_ADVENTUREBOOK_DAL_FIRST_TIME_FLOW"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        AdventureConfig.Get().MarkHasSeenFirstTimeFlowComplete();
        AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) AdventureConfig.Get().GetSelectedAdventure());
        if (record != null && record.MapPageHasButtonsToChapters)
          AdventureBookPageManager.NavigateToMapPage();
        else
          AdventureConfig.AckCurrentWingProgress(chapterPageData.WingRecord.ID);
        if (callback == null)
          return;
        callback();
      })
    });
  }

  public static bool IsEntireAdventureFree(AdventureDbId adventureID) => !GameDbf.Wing.HasRecord((Predicate<WingDbfRecord>) (r =>
  {
    if ((AdventureDbId) r.AdventureId != adventureID)
      return false;
    return r.PmtProductIdForSingleWingPurchase != 0 || r.PmtProductIdForThisAndRestOfAdventure != 0;
  }));

  public static bool DoesAdventureRequireAllHeroesUnlocked(AdventureDbId adventureId) => AdventureUtils.DoesAdventureRequireAllHeroesUnlocked(adventureId, AdventureConfig.GetDefaultModeDbIdForAdventure(adventureId));

  public static bool DoesAdventureRequireAllHeroesUnlocked(
    AdventureDbId adventureId,
    AdventureModeDbId modeId)
  {
    if (adventureId == AdventureDbId.INVALID || modeId == AdventureModeDbId.INVALID)
      return true;
    AdventureDataDbfRecord adventureDataRecord = AdventureConfig.GetAdventureDataRecord(adventureId, modeId);
    return adventureDataRecord == null || !adventureDataRecord.IgnoreHeroUnlockRequirement;
  }

  public static bool IsDuelsAdventure(AdventureDbId adventure)
  {
    if (adventure <= AdventureDbId.PVPDR_NEUTRAL_HEROES)
    {
      if (adventure != AdventureDbId.PVPDR && adventure != AdventureDbId.PVPDR_SEASON_2 && adventure != AdventureDbId.PVPDR_NEUTRAL_HEROES)
        goto label_4;
    }
    else if (adventure != AdventureDbId.PVPDR_LOH_HEROES && adventure != AdventureDbId.PVPDR_WITCHWOOD_HEROES && adventure != AdventureDbId.PVPDR_DEATH_KNIGHT)
      goto label_4;
    return true;
label_4:
    return false;
  }

  public static List<GuestHero> GetGuestHeroesForAdventure(
    AdventureDbId currentAdventure)
  {
    List<AdventureGuestHeroesDbfRecord> recordsForAdventures = AdventureUtils.GetSortedGuestHeroRecordsForAdventures(currentAdventure);
    List<GuestHero> heroesForAdventure = new List<GuestHero>();
    foreach (AdventureGuestHeroesDbfRecord guestHeroesDbfRecord in recordsForAdventures)
    {
      if (guestHeroesDbfRecord.GuestHeroId != 0)
        heroesForAdventure.Add(new GuestHero()
        {
          guestHeroId = guestHeroesDbfRecord.GuestHeroId,
          cardDbId = GameUtils.GetCardIdFromGuestHeroDbId(guestHeroesDbfRecord.GuestHeroId)
        });
    }
    return heroesForAdventure;
  }

  public static List<AdventureGuestHeroesDbfRecord> GetSortedGuestHeroRecordsForAdventures(
    AdventureDbId currentAdventure)
  {
    List<AdventureGuestHeroesDbfRecord> records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == currentAdventure));
    records.Sort((Comparison<AdventureGuestHeroesDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  public static CardListDataModel GetAvailableGuestHeroesAsCardListSortedByReleaseDate(
    AdventureDbId adventure)
  {
    CardListDataModel sortedByReleaseDate = new CardListDataModel();
    List<AdventureGuestHeroesDbfRecord> records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure && AdventureProgressMgr.IsWingEventActive(r.WingId)));
    DateTime defaultDateIfNoRecordFound = DateTime.MinValue;
    records.Sort((Comparison<AdventureGuestHeroesDbfRecord>) ((a, b) => DateTime.Compare(AdventureUtils.ReleaseDateForAdventureGuestHero(b, defaultDateIfNoRecordFound), AdventureUtils.ReleaseDateForAdventureGuestHero(a, defaultDateIfNoRecordFound))));
    foreach (AdventureGuestHeroesDbfRecord guestHeroesDbfRecord in records)
    {
      CardDbfRecord record = GameDbf.Card.GetRecord(GameUtils.GetCardIdFromGuestHeroDbId(guestHeroesDbfRecord.GuestHeroId));
      if (record != null)
      {
        CardDataModel cardDataModel = new CardDataModel()
        {
          CardId = record.NoteMiniGuid,
          Premium = TAG_PREMIUM.NORMAL
        };
        sortedByReleaseDate.Cards.Add(cardDataModel);
      }
    }
    return sortedByReleaseDate;
  }

  public static DateTime ReleaseDateForAdventureGuestHero(
    AdventureGuestHeroesDbfRecord adventureGuestHero,
    DateTime defaultDate)
  {
    if (adventureGuestHero.WingRecord == null)
      return defaultDate;
    SpecialEventType requiredEvent = adventureGuestHero.WingRecord.RequiredEvent;
    if (requiredEvent == SpecialEventType.UNKNOWN)
      return defaultDate;
    DateTime? eventStartTimeUtc = SpecialEventManager.Get().GetEventStartTimeUtc(requiredEvent);
    return eventStartTimeUtc.HasValue ? eventStartTimeUtc.Value : defaultDate;
  }

  public static bool DoesAdventureShowNewlyUnlockedGuestHeroTreatment(AdventureDbId adventure)
  {
    switch (adventure)
    {
      case AdventureDbId.GIL:
        return false;
      case AdventureDbId.BLACKROCK_CRASH:
      case AdventureDbId.LOE_REVIVAL:
      case AdventureDbId.TB_BUCKET_BRAWL:
      case AdventureDbId.NAXX_CRASH:
      case AdventureDbId.TEMPLE_OUTRUN:
      case AdventureDbId.ROAD_TO_NORTHREND:
        return false;
      default:
        return true;
    }
  }

  public static bool DoesAdventureHaveUnseenGuestHeroes(
    AdventureDbId adventure,
    AdventureModeDbId mode)
  {
    List<long> values = (List<long>) null;
    AdventureDataDbfRecord adventureDataRecord = AdventureConfig.GetAdventureDataRecord(adventure, mode);
    GameSaveDataManager.Get().GetSubkeyValue((GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_UNLOCKED_HEROES, out values);
    foreach (AdventureGuestHeroesDbfRecord record in GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure)))
    {
      if (record.GuestHeroId != 0 && (values == null || !values.Contains((long) GameUtils.GetCardIdFromGuestHeroDbId(record.GuestHeroId))) && AdventureProgressMgr.IsWingEventActive(record.WingId) && AdventureProgressMgr.Get().OwnsWing(record.WingId))
        return true;
    }
    return false;
  }

  private static AdventureGuestHeroesDbfRecord GetGuestHeroRecordForAdventure(
    AdventureDbId adventure,
    int heroCardDbId)
  {
    return GameDbf.AdventureGuestHeroes.GetRecord((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure && r.GuestHeroRecord != null && r.GuestHeroRecord.CardId == heroCardDbId));
  }

  public static List<AdventureHeroPowerDbfRecord> GetHeroPowersForDungeonCrawlHero(
    IDungeonCrawlData dungeonCrawl,
    int heroCardDbId)
  {
    List<AdventureHeroPowerDbfRecord> dungeonCrawlHero;
    if (AdventureConfig.Get().GetSelectedAdventureDataRecord().DungeonCrawlSaveHeroUsingHeroDbId)
    {
      int fromHeroCardDbId = AdventureUtils.GetGuestHeroIdFromHeroCardDbId(dungeonCrawl, heroCardDbId);
      dungeonCrawlHero = dungeonCrawl.GetHeroPowersForGuestHero(fromHeroCardDbId);
    }
    else
    {
      TAG_CLASS heroClassFromHeroId = AdventureUtils.GetHeroClassFromHeroId(heroCardDbId);
      dungeonCrawlHero = dungeonCrawl.GetHeroPowersForClass(heroClassFromHeroId);
    }
    return dungeonCrawlHero;
  }

  public static List<AdventureLoadoutTreasuresDbfRecord> GetTreasuresForDungeonCrawlHero(
    IDungeonCrawlData dungeonCrawl,
    int heroCardDbId)
  {
    List<AdventureLoadoutTreasuresDbfRecord> dungeonCrawlHero;
    if (AdventureConfig.Get().GetSelectedAdventureDataRecord().DungeonCrawlSaveHeroUsingHeroDbId)
    {
      int fromHeroCardDbId = AdventureUtils.GetGuestHeroIdFromHeroCardDbId(dungeonCrawl, heroCardDbId);
      dungeonCrawlHero = dungeonCrawl.GetLoadoutTreasuresForGuestHero(fromHeroCardDbId);
    }
    else
    {
      TAG_CLASS heroClassFromHeroId = AdventureUtils.GetHeroClassFromHeroId(heroCardDbId);
      dungeonCrawlHero = dungeonCrawl.GetLoadoutTreasuresForClass(heroClassFromHeroId);
    }
    return dungeonCrawlHero;
  }

  public static int GetGuestHeroIdFromHeroCardDbId(IDungeonCrawlData dungeonCrawl, int heroCardDbId)
  {
    List<GuestHero> currentAdventure = dungeonCrawl.GetGuestHeroesForCurrentAdventure();
    if (currentAdventure.Count == 0)
    {
      Debug.LogError((object) string.Format("No guest heroes were found for adventure: {0}", (object) dungeonCrawl?.GetSelectedAdventureDataRecord()?.AdventureId));
      return 0;
    }
    foreach (GuestHero guestHero in currentAdventure)
    {
      GuestHero guest = guestHero;
      if (guest.cardDbId == heroCardDbId)
        return GameDbf.GuestHero.GetRecord((Predicate<GuestHeroDbfRecord>) (r => r.ID == guest.guestHeroId)).ID;
    }
    return 0;
  }

  public static bool SelectableLoadoutTreasuresExistForAdventure(AdventureDbId adventure) => GameDbf.AdventureLoadoutTreasures.HasRecord((Predicate<AdventureLoadoutTreasuresDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure));

  public static bool SelectableHeroPowersExistForAdventure(AdventureDbId adventure) => GameDbf.AdventureHeroPower.HasRecord((Predicate<AdventureHeroPowerDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure));

  public static bool SelectableDecksExistForAdventure(AdventureDbId adventure) => GameDbf.AdventureDeck.HasRecord((Predicate<AdventureDeckDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventure));

  public static bool SelectableHeroPowersAndDecksExistForAdventure(AdventureDbId adventure)
  {
    bool flag1 = AdventureUtils.SelectableHeroPowersExistForAdventure(adventure);
    bool flag2 = AdventureUtils.SelectableDecksExistForAdventure(adventure);
    if (!flag1 && !flag2)
      return false;
    if (flag1 & flag2 || flag1 && SceneMgr.Get().GetMode() == SceneMgr.Mode.PVP_DUNGEON_RUN)
      return true;
    Debug.LogError((object) string.Format("Adventure {0} has ADVENTURE_HERO_POWER or ADVENTURE_DECK entries defined, but not both! This is not currently suported - you must have entries for both tables, so a Hero Power can be selected first, then a Deck.", (object) adventure));
    return false;
  }

  public static bool IsMissionValidForAdventureMode(
    AdventureDbId adventureId,
    AdventureModeDbId modeId,
    ScenarioDbId missionId)
  {
    return adventureId == AdventureDbId.PVPDR && missionId == ScenarioDbId.PVPDR_Tavern || adventureId != AdventureDbId.INVALID && modeId != AdventureModeDbId.INVALID && missionId != ScenarioDbId.INVALID && GameDbf.Scenario.GetRecord((Predicate<ScenarioDbfRecord>) (r => (ScenarioDbId) r.ID == missionId && (AdventureDbId) r.AdventureId == adventureId && (AdventureModeDbId) r.ModeId == modeId)) != null;
  }

  private static bool IsHeroValidForAdventure(AdventureDbId adventureId, int heroCardDbId)
  {
    if (heroCardDbId == 0)
      return false;
    foreach (GuestHero guestHero in AdventureUtils.GetGuestHeroesForAdventure(adventureId))
    {
      if (heroCardDbId == guestHero.cardDbId)
        return true;
    }
    return false;
  }

  private static bool IsHeroClassValidForAdventure(AdventureDbId adventureId, TAG_CLASS heroClass)
  {
    if (heroClass == TAG_CLASS.INVALID)
      return false;
    foreach (GuestHero guestHero in AdventureUtils.GetGuestHeroesForAdventure(adventureId))
    {
      if (heroClass == AdventureUtils.GetHeroClassFromHeroId(guestHero.cardDbId))
        return true;
    }
    return false;
  }

  public static bool IsHeroPowerValidForClassAndAdventure(
    AdventureDbId adventureId,
    TAG_CLASS heroClass,
    int heroPowerDbId)
  {
    return adventureId != AdventureDbId.INVALID && heroPowerDbId > 0 && GameDbf.AdventureHeroPower.HasRecord((Predicate<AdventureHeroPowerDbfRecord>) (r => r.CardId == heroPowerDbId && (AdventureDbId) r.AdventureId == adventureId && heroClass == (TAG_CLASS) r.ClassId));
  }

  public static bool IsHeroPowerValidForHeroAndAdventure(
    AdventureDbId adventureId,
    int heroCardDbId,
    int heroPowerDbId)
  {
    if (adventureId == AdventureDbId.INVALID || heroPowerDbId <= 0)
      return false;
    AdventureGuestHeroesDbfRecord recordForAdventure = AdventureUtils.GetGuestHeroRecordForAdventure(adventureId, heroCardDbId);
    int guestHeroIdToUse = recordForAdventure != null ? recordForAdventure.BaseGuestHeroId : 0;
    if (guestHeroIdToUse == 0)
      guestHeroIdToUse = recordForAdventure != null ? recordForAdventure.GuestHeroId : 0;
    return guestHeroIdToUse != 0 && GameDbf.AdventureHeroPower.HasRecord((Predicate<AdventureHeroPowerDbfRecord>) (r => r.CardId == heroPowerDbId && (AdventureDbId) r.AdventureId == adventureId && r.GuestHeroId == guestHeroIdToUse));
  }

  public static bool IsDeckValidForClassAndAdventure(
    AdventureDbId adventureId,
    TAG_CLASS heroClass,
    int deckDbId)
  {
    return adventureId != AdventureDbId.INVALID && deckDbId > 0 && GameDbf.AdventureDeck.HasRecord((Predicate<AdventureDeckDbfRecord>) (r => r.DeckId == deckDbId && (AdventureDbId) r.AdventureId == adventureId && heroClass == (TAG_CLASS) r.ClassId));
  }

  public static bool IsLoadoutTreasureValidForClassAndAdventure(
    AdventureDbId adventureId,
    TAG_CLASS heroClass,
    int treasureDbId)
  {
    return adventureId != AdventureDbId.INVALID && treasureDbId > 0 && GameDbf.AdventureLoadoutTreasures.HasRecord((Predicate<AdventureLoadoutTreasuresDbfRecord>) (r => (r.CardId == treasureDbId || r.UpgradedCardId == treasureDbId) && (AdventureDbId) r.AdventureId == adventureId && heroClass == (TAG_CLASS) r.ClassId));
  }

  public static bool IsLoadoutTreasureValidForHeroAndAdventure(
    AdventureDbId adventureId,
    int heroCardDbId,
    int treasureDbId)
  {
    if (adventureId == AdventureDbId.INVALID || treasureDbId <= 0)
      return false;
    AdventureGuestHeroesDbfRecord recordForAdventure = AdventureUtils.GetGuestHeroRecordForAdventure(adventureId, heroCardDbId);
    int guestHeroIdToUse = recordForAdventure != null ? recordForAdventure.BaseGuestHeroId : 0;
    if (guestHeroIdToUse == 0)
      guestHeroIdToUse = recordForAdventure != null ? recordForAdventure.GuestHeroId : 0;
    return guestHeroIdToUse != 0 && GameDbf.AdventureLoadoutTreasures.HasRecord((Predicate<AdventureLoadoutTreasuresDbfRecord>) (r => (r.CardId == treasureDbId || r.UpgradedCardId == treasureDbId) && (AdventureDbId) r.AdventureId == adventureId && r.GuestHeroId == guestHeroIdToUse));
  }

  public static TAG_CLASS GetHeroClassFromHeroId(int heroCardDbId) => GameUtils.GetTagClassFromCardDbId(heroCardDbId);

  public static bool IsValidLoadoutForSelectedAdventureAndClass(
    AdventureDbId adventureDbId,
    AdventureModeDbId modeDbId,
    ScenarioDbId scenarioDbId,
    TAG_CLASS heroClass,
    int heroPowerDbId,
    int deckDbId,
    int treasureDbId)
  {
    if (!AdventureUtils.IsMissionValidForAdventureMode(adventureDbId, modeDbId, scenarioDbId))
    {
      Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndClass - invalid scenario ID: {0} for adventure ID: {1} with mode ID: {2}.", (object) scenarioDbId, (object) adventureDbId, (object) modeDbId);
      return false;
    }
    if (!AdventureUtils.IsHeroClassValidForAdventure(adventureDbId, heroClass))
    {
      Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndClass - invalid hero class {0} for adventure ID: {1}.", (object) heroClass, (object) adventureDbId);
      return false;
    }
    if (AdventureUtils.SelectableHeroPowersAndDecksExistForAdventure(adventureDbId))
    {
      if (!AdventureUtils.IsHeroPowerValidForClassAndAdventure(adventureDbId, heroClass, heroPowerDbId))
      {
        Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndClass - invalid loadout hero power ID: {0} for adventure ID: {1}", (object) heroPowerDbId, (object) adventureDbId);
        return false;
      }
      if (SceneMgr.Get().GetMode() != SceneMgr.Mode.PVP_DUNGEON_RUN && !AdventureUtils.IsDeckValidForClassAndAdventure(adventureDbId, heroClass, deckDbId))
      {
        Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndClass - invalid loadout deck ID: {0} for adventure ID: {1}.", (object) deckDbId, (object) adventureDbId);
        return false;
      }
    }
    if (!AdventureUtils.SelectableLoadoutTreasuresExistForAdventure(adventureDbId) || AdventureUtils.IsLoadoutTreasureValidForClassAndAdventure(adventureDbId, heroClass, treasureDbId))
      return true;
    Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndClass - invalid loadout treasure ID: {0} for adventure ID: {1}.", (object) treasureDbId, (object) adventureDbId);
    return false;
  }

  public static bool IsValidLoadoutForSelectedAdventureAndHero(
    AdventureDbId adventureDbId,
    AdventureModeDbId modeDbId,
    ScenarioDbId scenarioDbId,
    int heroCardDbId,
    int heroPowerDbId,
    int treasureDbId)
  {
    if (!AdventureUtils.IsMissionValidForAdventureMode(adventureDbId, modeDbId, scenarioDbId))
    {
      Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndHero - invalid scenario ID: {0} for adventure ID: {1} with mode ID: {2}.", (object) scenarioDbId, (object) adventureDbId, (object) modeDbId);
      return false;
    }
    if (!AdventureUtils.IsHeroValidForAdventure(adventureDbId, heroCardDbId))
    {
      Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndHero - invalid hero {0} for adventure ID: {1}.", (object) heroCardDbId, (object) adventureDbId);
      return false;
    }
    if (AdventureUtils.SelectableHeroPowersAndDecksExistForAdventure(adventureDbId))
    {
      if (!AdventureUtils.IsHeroPowerValidForHeroAndAdventure(adventureDbId, heroCardDbId, heroPowerDbId))
      {
        Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndHero - invalid loadout hero power ID: {0} for adventure ID: {1}", (object) heroPowerDbId, (object) adventureDbId);
        return false;
      }
      if (AdventureUtils.SelectableDecksExistForAdventure(adventureDbId))
        Debug.LogError((object) "AdventureUtils.IsValidLoadoutForSelectedAdventureAndHero - Adventure decks referenced by Hero DB ID is not currently supported!");
    }
    if (!AdventureUtils.SelectableLoadoutTreasuresExistForAdventure(adventureDbId) || AdventureUtils.IsLoadoutTreasureValidForHeroAndAdventure(adventureDbId, heroCardDbId, treasureDbId))
      return true;
    Debug.LogFormat("AdventureUtils.IsValidLoadoutForSelectedAdventureAndHero - invalid loadout treasure ID: {0} for adventure ID: {1}.", (object) treasureDbId, (object) adventureDbId);
    return false;
  }

  public static bool CanPlayWingOpenQuote(AdventureWingDef wingDef)
  {
    if (!((UnityEngine.Object) wingDef != (UnityEngine.Object) null) || string.IsNullOrEmpty(wingDef.m_OpenQuotePrefab) || string.IsNullOrEmpty(wingDef.m_OpenQuoteVOLine))
      return false;
    return wingDef.m_PlayOpenQuoteInHeroic || !GameUtils.IsModeHeroic(AdventureConfig.Get().GetSelectedMode());
  }

  public static bool CanPlayWingCompleteQuote(AdventureWingDef wingDef)
  {
    if (!((UnityEngine.Object) wingDef != (UnityEngine.Object) null) || string.IsNullOrEmpty(wingDef.m_CompleteQuotePrefab) || string.IsNullOrEmpty(wingDef.m_CompleteQuoteVOLine))
      return false;
    return wingDef.m_PlayCompleteQuoteInHeroic || !GameUtils.IsModeHeroic(AdventureConfig.Get().GetSelectedMode());
  }

  public static void PlayMissionQuote(AdventureBossDef bossDef, Vector3 position)
  {
    if ((UnityEngine.Object) bossDef == (UnityEngine.Object) null)
      return;
    string introLine = bossDef.GetIntroLine();
    if (string.IsNullOrEmpty(introLine))
      return;
    AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(AdventureConfig.Get().GetSelectedAdventure());
    string prefabPath = (string) null;
    if ((UnityEngine.Object) adventureDef != (UnityEngine.Object) null)
      prefabPath = adventureDef.m_DefaultQuotePrefab;
    if (!string.IsNullOrEmpty(bossDef.m_quotePrefabOverride))
      prefabPath = bossDef.m_quotePrefabOverride;
    string legacyAssetName = new AssetReference(introLine).GetLegacyAssetName();
    if (string.IsNullOrEmpty(prefabPath))
      return;
    bool allowRepeatDuringSession = (UnityEngine.Object) AdventureScene.Get() != (UnityEngine.Object) null && AdventureScene.Get().IsDevMode;
    NotificationManager.Get().CreateCharacterQuote(prefabPath, position, GameStrings.Get(legacyAssetName), introLine, allowRepeatDuringSession);
  }

  public delegate void FirstChapterFreePopupCompleteCallback();
}
