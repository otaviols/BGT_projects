using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using PegasusClient;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RankMgr
{
  private static RankMgr s_instance;
  private Map<int, int> m_maxStarLevelByLeagueId;
  private int m_maxChestVisualIndex;
  private Map<int, Map<int, LeagueRankDbfRecord>> m_rankConfigByLeagueAndStarLevel;
  private Map<int, Map<LeagueBgPublicRatingEquiv.FormatType, List<LeagueBgPublicRatingEquivDbfRecord>>> m_bgPublicRatingEquiv;
  public static readonly AssetReference RANK_CHANGE_TWO_SCOOP_PREFAB_LEGACY = new AssetReference("RankChangeTwoScoop_LEGACY.prefab:c10232b70104d6e42b2dd9e6e1233495");
  public static readonly AssetReference RANK_CHANGE_TWO_SCOOP_PREFAB_NEW = new AssetReference("RankChangeTwoScoop_NEW.prefab:606c949d2ac1a8643a5ab70f4d8f67e6");
  public static readonly AssetReference RANKED_REWARD_DISPLAY_PREFAB = new AssetReference("RankedRewardDisplay.prefab:f95c6e7ec80adde4aa6c2f6df24262ea");
  public static readonly AssetReference RANKED_CARDBACK_PROGRESS_DISPLAY_PREFAB = new AssetReference("RankedCardBackProgressDisplay.prefab:b7a7de3cdf473fe4784b100111f02cbb");
  public static readonly AssetReference RANKED_REWARD_LIST_POPUP = new AssetReference("RankedRewardListPopup.prefab:6ee69b3ca628c0047b9016ffda861c5c");
  public static readonly AssetReference BONUS_STAR_POPUP_PREFAB = new AssetReference("RankedBonusStarsPopUp.prefab:d3e043ebff5163846a986cb55a69760c");
  public static readonly AssetReference RANKED_INTRO_POPUP_PREFAB = new AssetReference("RankedIntroPopUp.prefab:b0edfa4af7328bc4d92b637af2f1c32d");
  private NetCache.NetCacheMedalInfo m_cachedMedalInfo;
  private MedalInfoTranslator m_medalInfoTranslator;

  public static RankMgr Get()
  {
    if (RankMgr.s_instance == null)
      RankMgr.s_instance = new RankMgr();
    return RankMgr.s_instance;
  }

  public static void LogMessage(
    string message,
    [CallerMemberName] string methodName = null,
    [CallerFilePath] string sourceFile = null,
    [CallerLineNumber] int lineNumber = 0)
  {
    string str = string.Format("[{0} -- {1}] - {2} ({3}:{4})", (object) DateTime.Now, (object) methodName, (object) message, (object) sourceFile, (object) lineNumber);
    Debug.LogError((object) str);
    TelemetryManager.Client().SendLiveIssue("Gameplay-Option.FormatType", str);
  }

  public void PostProcessDbfLoad_League()
  {
    this.m_maxStarLevelByLeagueId = new Map<int, int>();
    this.m_maxChestVisualIndex = 0;
    this.m_rankConfigByLeagueAndStarLevel = new Map<int, Map<int, LeagueRankDbfRecord>>();
    foreach (LeagueRankDbfRecord record in GameDbf.LeagueRank.GetRecords())
    {
      int num;
      if (!this.m_maxStarLevelByLeagueId.TryGetValue(record.LeagueId, out num))
        this.m_maxStarLevelByLeagueId.Add(record.LeagueId, record.StarLevel);
      else if (record.StarLevel > num)
        this.m_maxStarLevelByLeagueId[record.LeagueId] = record.StarLevel;
      if (record.RewardChestVisualIndex > this.m_maxChestVisualIndex)
        this.m_maxChestVisualIndex = record.RewardChestVisualIndex;
      Map<int, LeagueRankDbfRecord> map;
      if (!this.m_rankConfigByLeagueAndStarLevel.TryGetValue(record.LeagueId, out map) || map == null)
        this.m_rankConfigByLeagueAndStarLevel[record.LeagueId] = map = new Map<int, LeagueRankDbfRecord>();
      map[record.StarLevel] = record;
    }
    BnetRegion currentRegion = BattleNet.GetCurrentRegion();
    this.m_bgPublicRatingEquiv = new Map<int, Map<LeagueBgPublicRatingEquiv.FormatType, List<LeagueBgPublicRatingEquivDbfRecord>>>();
    foreach (LeagueBgPublicRatingEquivDbfRecord record in GameDbf.LeagueBgPublicRatingEquiv.GetRecords())
    {
      if (record.Region == LeagueBgPublicRatingEquiv.Region.REGION_UNKNOWN || record.Region == (LeagueBgPublicRatingEquiv.Region) currentRegion)
      {
        Map<LeagueBgPublicRatingEquiv.FormatType, List<LeagueBgPublicRatingEquivDbfRecord>> map;
        if (!this.m_bgPublicRatingEquiv.TryGetValue(record.LeagueId, out map))
        {
          map = new Map<LeagueBgPublicRatingEquiv.FormatType, List<LeagueBgPublicRatingEquivDbfRecord>>();
          this.m_bgPublicRatingEquiv[record.LeagueId] = map;
        }
        List<LeagueBgPublicRatingEquivDbfRecord> ratingEquivDbfRecordList;
        if (!map.TryGetValue(record.FormatType, out ratingEquivDbfRecordList))
        {
          ratingEquivDbfRecordList = new List<LeagueBgPublicRatingEquivDbfRecord>();
          map[record.FormatType] = ratingEquivDbfRecordList;
        }
        ratingEquivDbfRecordList.Add(record);
      }
    }
    foreach (Map<LeagueBgPublicRatingEquiv.FormatType, List<LeagueBgPublicRatingEquivDbfRecord>> map in this.m_bgPublicRatingEquiv.Values)
    {
      foreach (List<LeagueBgPublicRatingEquivDbfRecord> ratingEquivDbfRecordList in map.Values)
        ratingEquivDbfRecordList.Sort((Comparison<LeagueBgPublicRatingEquivDbfRecord>) ((a, b) => a.StarLevel != b.StarLevel ? a.StarLevel.CompareTo(b.StarLevel) : b.LegendRank.CompareTo(a.LegendRank)));
    }
  }

  public bool UseLegacyRankedPlay()
  {
    LeagueDbfRecord standardLeagueConfig = this.GetLocalPlayerStandardLeagueConfig();
    return standardLeagueConfig != null && this.UseLegacyRankedPlay(standardLeagueConfig.ID);
  }

  public bool UseLegacyRankedPlay(int leagueId)
  {
    LeagueDbfRecord leagueRecord = this.GetLeagueRecord(leagueId);
    if (leagueRecord != null)
    {
      switch (leagueRecord.LeagueType)
      {
        case League.LeagueType.NORMAL:
          return leagueRecord.LeagueVersion <= 2;
        case League.LeagueType.NEW_PLAYER:
          return leagueRecord.LeagueVersion <= 2;
      }
    }
    return false;
  }

  public bool DidPromoteSelfThisSession { get; set; }

  public bool HasLocalPlayerMedalInfo => NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>() != null;

  public MedalInfoTranslator GetLocalPlayerMedalInfo()
  {
    NetCache.NetCacheMedalInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>();
    if (netObject == null)
    {
      Log.All.PrintError("NetCacheMedalInfo not yet available!");
      return new MedalInfoTranslator();
    }
    if (this.m_cachedMedalInfo == netObject)
      return this.m_medalInfoTranslator;
    this.m_cachedMedalInfo = netObject;
    this.m_medalInfoTranslator = new MedalInfoTranslator(netObject, netObject.PreviousMedalInfo);
    return this.m_medalInfoTranslator;
  }

  public bool WildCardsAllowedInCurrentLeague() => !this.GetLocalPlayerStandardLeagueConfig().LockWildCards;

  public bool CanPromoteSelfManually() => this.GetLocalPlayerStandardLeagueConfig().CanPromoteSelfManually;

  public PegasusShared.BnetGameType GetBnetGameTypeForLeague(
    bool inRankedMode,
    PegasusShared.FormatType format)
  {
    LeagueDbfRecord playerLeagueConfig = this.GetLocalPlayerLeagueConfig(format);
    List<LeagueGameType.BnetGameType> gameTypesToExclude = new List<LeagueGameType.BnetGameType>();
    if (inRankedMode)
      gameTypesToExclude.AddRange((IEnumerable<LeagueGameType.BnetGameType>) new LeagueGameType.BnetGameType[3]
      {
        LeagueGameType.BnetGameType.BGT_CASUAL_STANDARD,
        LeagueGameType.BnetGameType.BGT_CASUAL_WILD,
        LeagueGameType.BnetGameType.BGT_CASUAL_CLASSIC
      });
    else
      gameTypesToExclude.AddRange((IEnumerable<LeagueGameType.BnetGameType>) new LeagueGameType.BnetGameType[3]
      {
        LeagueGameType.BnetGameType.BGT_RANKED_STANDARD,
        LeagueGameType.BnetGameType.BGT_RANKED_WILD,
        LeagueGameType.BnetGameType.BGT_RANKED_CLASSIC
      });
    LeagueGameTypeDbfRecord gameTypeDbfRecord = playerLeagueConfig.LeagueGameType.Where<LeagueGameTypeDbfRecord>((Func<LeagueGameTypeDbfRecord, bool>) (x => x.FormatType == (LeagueGameType.FormatType) format && !gameTypesToExclude.Contains(x.BnetGameType))).FirstOrDefault<LeagueGameTypeDbfRecord>();
    return gameTypeDbfRecord == null ? PegasusShared.BnetGameType.BGT_UNKNOWN : (PegasusShared.BnetGameType) gameTypeDbfRecord.BnetGameType;
  }

  public bool IsFormatAllowedInLeague(PegasusShared.FormatType format) => this.GetBnetGameTypeForLeague(false, format) != PegasusShared.BnetGameType.BGT_UNKNOWN || this.GetBnetGameTypeForLeague(true, format) != 0;

  public bool IsLegendRankInAnyFormat
  {
    get
    {
      foreach (PegasusShared.FormatType formatType in Enum.GetValues(typeof (PegasusShared.FormatType)))
      {
        if (formatType != PegasusShared.FormatType.FT_UNKNOWN && this.IsLegendRank(formatType))
          return true;
      }
      return false;
    }
  }

  public bool IsLegendRank(PegasusShared.FormatType formatType) => this.GetLocalPlayerMedalInfo().GetCurrentMedal(formatType).IsLegendRank();

  public bool IsNewPlayer() => Network.ShouldBeConnectedToAurora() && NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>() != null && RankMgr.Get().GetLocalPlayerStandardLeagueConfig().LeagueType == League.LeagueType.NEW_PLAYER;

  public void SetRankPresenceField()
  {
    GamePresenceRank gamePresenceRank = this.CalculateGamePresenceRank();
    BnetPresenceMgr.Get().SetGameFieldBlob(18U, (IProtoBuf) gamePresenceRank);
  }

  private GamePresenceRank CalculateGamePresenceRank()
  {
    GamePresenceRank gamePresenceRank = new GamePresenceRank();
    NetCache netCache = NetCache.Get();
    if (netCache == null)
      return gamePresenceRank;
    SceneMgr sceneMgr = SceneMgr.Get();
    GameMgr gameMgr = GameMgr.Get();
    NetCache.NetCacheFeatures netObject1 = netCache.GetNetObject<NetCache.NetCacheFeatures>();
    bool flag1 = netObject1 == null || !netObject1.BattlegroundsMedalFriendListDisplayEnabled;
    bool flag2 = flag1 || sceneMgr != null && sceneMgr.GetMode() == SceneMgr.Mode.TOURNAMENT || sceneMgr != null && sceneMgr.GetMode() == SceneMgr.Mode.GAMEPLAY && gameMgr != null && gameMgr.IsRankedPlay();
    bool flag3 = !flag2 && (sceneMgr != null && sceneMgr.GetMode() == SceneMgr.Mode.BACON || sceneMgr != null && sceneMgr.GetMode() == SceneMgr.Mode.GAMEPLAY && gameMgr != null && gameMgr.IsBattlegrounds());
    MedalInfoData medalInfoData1 = (MedalInfoData) null;
    int num1 = 0;
    if (flag2)
    {
      PegasusShared.FormatType formatType = !flag1 ? Options.GetFormatType() : this.GetLocalPlayerMedalInfo().GetBestCurrentRankFormatType();
      medalInfoData1 = netCache.GetNetObject<NetCache.NetCacheMedalInfo>()?.GetMedalInfoData(formatType);
    }
    if (medalInfoData1 == null && !flag3)
    {
      foreach (PegasusShared.FormatType formatType in Enum.GetValues(typeof (PegasusShared.FormatType)))
      {
        if (formatType != PegasusShared.FormatType.FT_UNKNOWN)
        {
          MedalInfoData medalInfoData2 = netCache.GetNetObject<NetCache.NetCacheMedalInfo>()?.GetMedalInfoData(formatType);
          int tempBgPublicRatingEquiv;
          if (medalInfoData2 != null && TryCalculateBgPublicRatingEquiv(medalInfoData2, out tempBgPublicRatingEquiv) && tempBgPublicRatingEquiv >= num1)
          {
            medalInfoData1 = medalInfoData2;
            num1 = tempBgPublicRatingEquiv;
          }
        }
      }
    }
    int num2 = 0;
    NetCache.NetCacheBaconRatingInfo netObject2 = netCache.GetNetObject<NetCache.NetCacheBaconRatingInfo>();
    if (netObject2 != null)
      num2 = netObject2.Rating;
    if (!flag2 && !flag3 && num1 == 0 && num2 == 0)
      return gamePresenceRank;
    GamePresenceRankData presenceRankData = new GamePresenceRankData();
    if (flag2 || !flag3 && num1 >= num2)
    {
      if (medalInfoData1 == null)
        return gamePresenceRank;
      presenceRankData = new GamePresenceRankData()
      {
        FormatType = medalInfoData1.FormatType,
        LeagueId = medalInfoData1.LeagueId,
        StarLevel = medalInfoData1.StarLevel,
        LegendRank = medalInfoData1.LegendRank,
        GameType = GameType.GT_RANKED,
        Rating = -1
      };
    }
    else if (flag3 || num1 < num2)
      presenceRankData = new GamePresenceRankData()
      {
        FormatType = PegasusShared.FormatType.FT_UNKNOWN,
        LeagueId = 0,
        StarLevel = 0,
        LegendRank = 0,
        GameType = GameType.GT_BATTLEGROUNDS,
        Rating = num2
      };
    gamePresenceRank.Values.Add(presenceRankData);
    return gamePresenceRank;

    bool TryCalculateBgPublicRatingEquiv(
      MedalInfoData medalInfoData,
      out int tempBgPublicRatingEquiv)
    {
      tempBgPublicRatingEquiv = 0;
      if (this.GetLeagueRecord(medalInfoData.LeagueId) == null)
        return false;
      Map<LeagueBgPublicRatingEquiv.FormatType, List<LeagueBgPublicRatingEquivDbfRecord>> map;
      if (!this.m_bgPublicRatingEquiv.TryGetValue(medalInfoData.LeagueId, out map))
      {
        Debug.LogError((object) string.Format("No LEAGUE_BG_PUBLIC_RATING_EQUIV record found for League={0}", (object) medalInfoData.LeagueId));
        return false;
      }
      List<LeagueBgPublicRatingEquivDbfRecord> ratingEquivDbfRecordList;
      if (!map.TryGetValue((LeagueBgPublicRatingEquiv.FormatType) medalInfoData.FormatType, out ratingEquivDbfRecordList) && !map.TryGetValue(LeagueBgPublicRatingEquiv.FormatType.FT_UNKNOWN, out ratingEquivDbfRecordList))
      {
        Debug.LogError((object) ("No LEAGUE_BG_PUBLIC_RATING_EQUIV record found for " + string.Format("League={0} Format={1}", (object) medalInfoData.LeagueId, (object) medalInfoData.FormatType)));
        return false;
      }
      if (ratingEquivDbfRecordList.Count == 0 || ratingEquivDbfRecordList[0].StarLevel != 1)
      {
        Debug.LogError((object) ("No LEAGUE_BG_PUBLIC_RATING_EQUIV record found for StarLevel 1 for" + string.Format("League={0} Format={1}", (object) medalInfoData.LeagueId, (object) medalInfoData.FormatType)));
        return false;
      }
      bool flag = false;
      LeagueBgPublicRatingEquivDbfRecord ratingEquivDbfRecord1 = (LeagueBgPublicRatingEquivDbfRecord) null;
      foreach (LeagueBgPublicRatingEquivDbfRecord ratingEquivDbfRecord2 in ratingEquivDbfRecordList)
      {
        if (ratingEquivDbfRecord2.StarLevel < medalInfoData.StarLevel || ratingEquivDbfRecord2.StarLevel == medalInfoData.StarLevel && ratingEquivDbfRecord2.LegendRank > medalInfoData.LegendRank)
        {
          ratingEquivDbfRecord1 = ratingEquivDbfRecord2;
        }
        else
        {
          if (ratingEquivDbfRecord2.StarLevel == medalInfoData.StarLevel && ratingEquivDbfRecord2.LegendRank == medalInfoData.LegendRank)
          {
            tempBgPublicRatingEquiv = ratingEquivDbfRecord2.BgPublicRatingEquiv;
            flag = true;
            break;
          }
          LeagueBgPublicRatingEquivDbfRecord ratingEquivDbfRecord3 = ratingEquivDbfRecord1;
          LeagueBgPublicRatingEquivDbfRecord ratingEquivDbfRecord4 = ratingEquivDbfRecord2;
          float t = ratingEquivDbfRecord2.StarLevel <= medalInfoData.StarLevel ? (float) (medalInfoData.LegendRank - ratingEquivDbfRecord3.LegendRank) / (float) (ratingEquivDbfRecord4.LegendRank - ratingEquivDbfRecord3.LegendRank) : (float) (medalInfoData.StarLevel - ratingEquivDbfRecord3.StarLevel) / (float) (ratingEquivDbfRecord4.StarLevel - ratingEquivDbfRecord3.StarLevel);
          tempBgPublicRatingEquiv = (int) Mathf.Lerp((float) ratingEquivDbfRecord3.BgPublicRatingEquiv, (float) ratingEquivDbfRecord4.BgPublicRatingEquiv, t);
          flag = true;
          break;
        }
      }
      if (!flag && ratingEquivDbfRecord1 != null)
        tempBgPublicRatingEquiv = ratingEquivDbfRecord1.BgPublicRatingEquiv;
      return true;
    }
  }

  public MedalInfoTranslator GetRankedMedalFromRankPresenceField(
    BnetPlayer player)
  {
    return player == null ? (MedalInfoTranslator) null : this.GetRankedMedalFromRankPresenceField(player.GetHearthstoneGameAccount());
  }

  public MedalInfoTranslator GetRankedMedalFromRankPresenceField(
    BnetGameAccount gameAccount)
  {
    if (gameAccount != (BnetGameAccount) null)
    {
      byte[] val;
      if (gameAccount.TryGetGameFieldBytes(18U, out val))
      {
        try
        {
          return MedalInfoTranslator.CreateMedalInfoForGamePresenceRank(ProtobufUtil.ParseFrom<GamePresenceRank>(val));
        }
        catch (Exception ex)
        {
          Log.Presence.PrintInfo(ex.ToString());
        }
      }
    }
    return new MedalInfoTranslator();
  }

  public bool GetBattlegroundsMedalFromRankPresenceField(
    BnetGameAccount gameAccount,
    out int bgRating)
  {
    if (gameAccount != (BnetGameAccount) null)
    {
      byte[] val;
      if (gameAccount.TryGetGameFieldBytes(18U, out val))
      {
        try
        {
          GamePresenceRankData presenceRankData = ProtobufUtil.ParseFrom<GamePresenceRank>(val).Values.Where<GamePresenceRankData>((Func<GamePresenceRankData, bool>) (x => x.GameType == GameType.GT_BATTLEGROUNDS)).FirstOrDefault<GamePresenceRankData>();
          if (presenceRankData != null)
          {
            bgRating = presenceRankData.Rating;
            return true;
          }
        }
        catch (Exception ex)
        {
          Log.Presence.PrintInfo(ex.ToString());
        }
      }
    }
    bgRating = -1;
    return false;
  }

  public LeagueDbfRecord GetLeagueRecord(int leagueId)
  {
    List<LeagueDbfRecord> records = GameDbf.League.GetRecords();
    LeagueDbfRecord leagueRecord = (LeagueDbfRecord) null;
    int index = 0;
    for (int count = records.Count; index < count; ++index)
    {
      LeagueDbfRecord leagueDbfRecord = records[index];
      if (leagueDbfRecord.ID == leagueId)
      {
        leagueRecord = leagueDbfRecord;
        break;
      }
    }
    if (leagueRecord != null)
      return leagueRecord;
    Log.All.PrintError("No record for leagueId={0}", (object) leagueId);
    return new LeagueDbfRecord();
  }

  public LeagueRankDbfRecord GetLeagueRankRecord(int leagueId, int starLevel)
  {
    Map<int, LeagueRankDbfRecord> map;
    LeagueRankDbfRecord leagueRankRecord;
    if (this.m_rankConfigByLeagueAndStarLevel.TryGetValue(leagueId, out map) && map != null && map.TryGetValue(starLevel, out leagueRankRecord) && leagueRankRecord != null)
      return leagueRankRecord;
    Log.All.PrintError("No record for leagueId={0} starLevel={1}", (object) leagueId, (object) starLevel);
    return new LeagueRankDbfRecord();
  }

  public LeagueDbfRecord GetLeagueRecordForType(
    League.LeagueType leagueType,
    int seasonId)
  {
    LeagueDbfRecord leagueRecordForType = (LeagueDbfRecord) null;
    int num = 0;
    foreach (LeagueDbfRecord record in GameDbf.League.GetRecords())
    {
      if (record.LeagueType == leagueType && record.InitialSeasonId <= seasonId && record.LeagueVersion > num)
      {
        num = record.LeagueVersion;
        leagueRecordForType = record;
      }
    }
    if (leagueRecordForType == null)
      Log.All.PrintError("No record for leagueType={0}", (object) leagueType);
    return leagueRecordForType;
  }

  public LeagueRankDbfRecord GetLeagueRankRecordByCheatName(string cheatName)
  {
    LeagueRankDbfRecord recordByCheatName = (LeagueRankDbfRecord) null;
    int num = 0;
    foreach (LeagueRankDbfRecord record in GameDbf.LeagueRank.GetRecords())
    {
      if (record.CheatName == cheatName)
      {
        LeagueDbfRecord leagueRecord = this.GetLeagueRecord(record.LeagueId);
        if (leagueRecord.LeagueVersion > num)
        {
          num = leagueRecord.LeagueVersion;
          recordByCheatName = record;
        }
      }
    }
    if (recordByCheatName == null)
      Log.All.PrintError("No record for cheatName={0}", (object) cheatName);
    return recordByCheatName;
  }

  public LeagueDbfRecord GetLocalPlayerStandardLeagueConfig() => this.GetLocalPlayerLeagueConfig(PegasusShared.FormatType.FT_STANDARD);

  public LeagueDbfRecord GetLocalPlayerLeagueConfig(PegasusShared.FormatType format) => this.GetLocalPlayerMedalInfo().GetCurrentMedal(format).LeagueConfig;

  public int GetMaxStarLevel(int leagueId)
  {
    int maxStarLevel;
    if (!this.m_maxStarLevelByLeagueId.TryGetValue(leagueId, out maxStarLevel))
      maxStarLevel = 0;
    return maxStarLevel;
  }

  public int GetMaxRewardChestVisualIndex() => this.m_maxChestVisualIndex;

  public bool IsCardLockedInCurrentLeague(EntityDef entityDef)
  {
    LeagueDbfRecord standardLeagueConfig = this.GetLocalPlayerStandardLeagueConfig();
    return standardLeagueConfig != null && (standardLeagueConfig.LockWildCards && GameUtils.IsWildCard(entityDef) || this.IsCardBannedInCurrentLeague(entityDef));
  }

  public HashSet<string> GetBannedCardsInCurrentLeague()
  {
    int cardsFromSubsetId = this.GetLocalPlayerStandardLeagueConfig().LockCardsFromSubsetId;
    return GameDbf.GetIndex().GetSubsetById(cardsFromSubsetId);
  }

  public bool IsCardBannedInCurrentLeague(EntityDef entityDef) => this.GetBannedCardsInCurrentLeague().Contains(entityDef.GetCardId());

  public int GetRankedRewardBoosterIdForSeasonId(int seasonId)
  {
    List<BoosterDbfRecord> records = GameDbf.Booster.GetRecords((Predicate<BoosterDbfRecord>) (r => r.RankedRewardInitialSeason > 0));
    records.Sort((Comparison<BoosterDbfRecord>) ((a, b) => b.RankedRewardInitialSeason - a.RankedRewardInitialSeason));
    foreach (BoosterDbfRecord boosterDbfRecord in records)
    {
      if (seasonId >= boosterDbfRecord.RankedRewardInitialSeason)
        return boosterDbfRecord.ID;
    }
    return 1;
  }

  public int GetRankedCardBackIdForSeasonId(int seasonId)
  {
    int backIdForSeasonId = 0;
    foreach (CardBackDbfRecord record in GameDbf.CardBack.GetRecords())
    {
      if (record.Source == Assets.CardBack.Source.SEASON && record.Data1 == (long) seasonId)
      {
        backIdForSeasonId = record.ID;
        break;
      }
    }
    return backIdForSeasonId;
  }
}
