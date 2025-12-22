using Hearthstone.DungeonCrawl;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelsConfig
{
  public static string EARLY_ACCESS_EVENT = "pvpdr_early_access";
  public static string PAID_UNLOCKED_EVENT = "pvpdr_paid_unlocked";
  public static int PAID_GOLD_COST = 150;
  private static DuelsConfig m_instance;
  public static string DOOR_LEVEL_CLICKED = "DOOR_LEVER_CLICKED";
  public static string DOOR_OPENED_EVENT = "DOOR_OPENED";
  public static string LEVER_GLOW_STATE = "GLOW";
  public static string ANIMATE_PAID_STATE = "ANIMATE_PAID";
  public static string ANIMATE_FREE_STATE = "ANIMATE_FREE";
  private bool m_recentLoss;
  private bool m_recentWin;
  private bool m_recentRunEnd;
  private int m_lastRunWins;

  public static DuelsConfig Get()
  {
    if (DuelsConfig.m_instance == null)
      DuelsConfig.m_instance = new DuelsConfig();
    return DuelsConfig.m_instance;
  }

  public void SetLastGameResult(TAG_PLAYSTATE lastGameState)
  {
    this.m_recentLoss = lastGameState == TAG_PLAYSTATE.LOST;
    this.m_recentWin = lastGameState == TAG_PLAYSTATE.WON;
  }

  public void ResetLastGameResult()
  {
    this.m_recentLoss = false;
    this.m_recentWin = false;
  }

  public bool HasRecentWin() => this.m_recentWin;

  public bool HasRecentLoss() => this.m_recentLoss;

  public void SetRecentEnd(bool value) => this.m_recentRunEnd = value;

  public bool RunRecentlyEnded() => this.m_recentRunEnd;

  public int LastRunWins
  {
    get => this.m_lastRunWins;
    set => this.m_lastRunWins = value;
  }

  public NetCache.ProfileNoticeGenericRewardChest GetRewardNoticeToShow() => (NetCache.ProfileNoticeGenericRewardChest) NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>().Notices.Find((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST && obj.Origin == NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_DUELS));

  public bool IsReadyToShowRewards()
  {
    AdventureDungeonCrawlDisplay dungeonCrawlDisplay = AdventureDungeonCrawlDisplay.Get();
    return (UnityEngine.Object) dungeonCrawlDisplay != (UnityEngine.Object) null && (UnityEngine.Object) dungeonCrawlDisplay.m_playMat != (UnityEngine.Object) null && dungeonCrawlDisplay.m_playMat.IsReadyToShowDuelsRewards();
  }

  public void ShowRewardsForNotice(
    NetCache.ProfileNoticeGenericRewardChest notice,
    Action doneCallback,
    Transform bone = null)
  {
    if (notice == null)
      Log.All.PrintError("DuelsConfig.ShowRewards - Trying to display invalid reward notice");
    else
      RewardUtils.ShowRewardBoxes(RewardUtils.GetRewards(new List<NetCache.ProfileNotice>()
      {
        (NetCache.ProfileNotice) notice
      }), (Action) (() =>
      {
        AdventureDungeonCrawlDisplay dungeonCrawlDisplay = AdventureDungeonCrawlDisplay.Get();
        if ((UnityEngine.Object) dungeonCrawlDisplay != (UnityEngine.Object) null)
        {
          dungeonCrawlDisplay.m_playMat.OnDuelsRewardsAccepted();
          dungeonCrawlDisplay.EndDuelsSession(notice.NoticeID);
        }
        doneCallback();
      }), bone);
  }

  public static bool HasEarlyAccess() => AccountLicenseMgr.Get().OwnsAccountLicense(NetCache.Get().GetDuelsEarlyAccessLicenseId());

  public static bool IsEarlyAccess() => SpecialEventManager.Get().IsEventActive(DuelsConfig.EARLY_ACCESS_EVENT, false);

  public static bool IsFreeUnlocked() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.GetFeatureFlag(NetCache.NetCacheFeatures.CacheGames.FeatureFlags.Duels);

  public static bool IsPaidUnlocked() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.GetFeatureFlag(NetCache.NetCacheFeatures.CacheGames.FeatureFlags.PaidDuels) && SpecialEventManager.Get().IsEventActive(DuelsConfig.PAID_UNLOCKED_EVENT, false);

  public static bool IsCardLoadoutTreasure(string cardID) => SceneMgr.Get().IsInDuelsMode() && (UnityEngine.Object) AdventureDungeonCrawlDisplay.Get() != (UnityEngine.Object) null && AdventureDungeonCrawlDisplay.Get().IsCardLoadoutTreasureForCurrentHero(cardID);

  public static List<long> GetDraftHeroesFromGSD()
  {
    List<long> values = (List<long>) null;
    if ((UnityEngine.Object) PvPDungeonRunScene.Get() != (UnityEngine.Object) null)
    {
      GameSaveKeyId gsdKeyForAdventure = PvPDungeonRunScene.Get().GetGSDKeyForAdventure();
      GameSaveDataManager.Get().GetSubkeyValue(gsdKeyForAdventure, GameSaveKeySubkeyId.DUELS_DRAFT_HERO_CHOICES, out values);
    }
    return values;
  }

  public static PvpdrSeasonDbfRecord GetSeasonDBFRecord() => (UnityEngine.Object) PvPDungeonRunScene.Get() != (UnityEngine.Object) null ? GameDbf.PvpdrSeason.GetRecord(PvPDungeonRunScene.Get().GetSeasonID()) : (PvpdrSeasonDbfRecord) null;

  public static bool IsInitialLoadoutComplete()
  {
    if ((UnityEngine.Object) PvPDungeonRunScene.Get() == (UnityEngine.Object) null)
      return false;
    IDungeonCrawlData dungeonCrawlData = PvPDungeonRunScene.Get().GetDungeonCrawlData();
    AdventureDataDbfRecord adventureDataRecord = dungeonCrawlData.GetSelectedAdventureDataRecord();
    GameSaveKeyId saveDataServerKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey;
    long num1;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_ACTIVE, out num1);
    long cardClass;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CLASS, out cardClass);
    long classForDungeonCrawl;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CARD_DB_ID, out classForDungeonCrawl);
    long num2;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_LOADOUT_TREASURE_ID, out num2);
    long num3;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_POWER, out num3);
    if (!adventureDataRecord.DungeonCrawlSaveHeroUsingHeroDbId)
      classForDungeonCrawl = (long) AdventureUtils.GetHeroCardDbIdFromClassForDungeonCrawl(dungeonCrawlData, (TAG_CLASS) cardClass);
    if (num1 > 0L)
      return true;
    return classForDungeonCrawl > 0L && num2 > 0L && num3 > 0L;
  }

  public static bool CanImportDecks() => PvPDungeonRunScene.IsEditingDeck();

  public static List<TAG_CARD_SET> GetDuelsSets()
  {
    List<TAG_CARD_SET> duelsSets = new List<TAG_CARD_SET>();
    DeckRuleset pvpdrRuleset = DeckRuleset.GetPVPDRRuleset();
    if (pvpdrRuleset != null)
    {
      HashSet<TAG_CARD_SET> allowedCardSets = pvpdrRuleset.GetAllowedCardSets();
      allowedCardSets.Remove(TAG_CARD_SET.EXPERT1);
      foreach (TAG_CARD_SET displayableCardSet in CollectionManager.Get().GetDisplayableCardSets())
      {
        if (allowedCardSets.Contains(displayableCardSet))
          duelsSets.Add(displayableCardSet);
      }
      duelsSets.Reverse();
    }
    return duelsSets;
  }

  public static int GetAdventureIdForSeason(int seasonId)
  {
    PvpdrSeasonDbfRecord record = GameDbf.PvpdrSeason.GetRecord(seasonId);
    return record == null ? 0 : record.AdventureId;
  }
}
