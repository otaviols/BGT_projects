using Blizzard.T5.Core;
using Hearthstone.DataModels;
using PegasusClient;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MedalInfoTranslator
{
  private Map<FormatType, TranslatedMedalInfo> m_currMedalInfo = new Map<FormatType, TranslatedMedalInfo>();
  private Map<FormatType, TranslatedMedalInfo> m_prevMedalInfo = new Map<FormatType, TranslatedMedalInfo>();

  public int TotalRankedWins => this.m_currMedalInfo.Sum<KeyValuePair<FormatType, TranslatedMedalInfo>>((Func<KeyValuePair<FormatType, TranslatedMedalInfo>, int>) (x => x.Value.seasonWins));

  public int TotalRankedWinsPrevious => this.m_prevMedalInfo.Sum<KeyValuePair<FormatType, TranslatedMedalInfo>>((Func<KeyValuePair<FormatType, TranslatedMedalInfo>, int>) (x => x.Value.seasonWins));

  public bool IsDisplayable() => this.m_currMedalInfo.Any<KeyValuePair<FormatType, TranslatedMedalInfo>>((Func<KeyValuePair<FormatType, TranslatedMedalInfo>, bool>) (x => x.Value.IsValid() && GameDbf.League.HasRecord(x.Value.leagueId)));

  public MedalInfoTranslator()
  {
    foreach (FormatType formatType in Enum.GetValues(typeof (FormatType)))
    {
      if (formatType != FormatType.FT_UNKNOWN)
      {
        this.m_currMedalInfo.Add(formatType, MedalInfoTranslator.CreateTranslatedMedalInfo(formatType, 0, 0, 0));
        this.m_prevMedalInfo.Add(formatType, MedalInfoTranslator.CreateTranslatedMedalInfo(formatType, 0, 0, 0));
      }
    }
  }

  public static MedalInfoTranslator CreateMedalInfoForLeagueId(
    int leagueId,
    int starLevel,
    int legendIndex)
  {
    MedalInfoTranslator medalInfoForLeagueId = new MedalInfoTranslator();
    foreach (FormatType formatType in Enum.GetValues(typeof (FormatType)))
    {
      if (formatType != FormatType.FT_UNKNOWN)
      {
        medalInfoForLeagueId.m_currMedalInfo[formatType] = MedalInfoTranslator.CreateTranslatedMedalInfo(formatType, leagueId, starLevel, legendIndex);
        medalInfoForLeagueId.m_prevMedalInfo[formatType] = medalInfoForLeagueId.m_currMedalInfo[formatType].ShallowCopy();
      }
    }
    return medalInfoForLeagueId;
  }

  public static MedalInfoTranslator CreateMedalInfoForGamePresenceRank(
    GamePresenceRank gamePresenceRank)
  {
    MedalInfoTranslator gamePresenceRank1 = new MedalInfoTranslator();
    foreach (FormatType formatType1 in Enum.GetValues(typeof (FormatType)))
    {
      FormatType formatType = formatType1;
      if (formatType != FormatType.FT_UNKNOWN)
      {
        GamePresenceRankData presenceRankData = gamePresenceRank.Values.Where<GamePresenceRankData>((Func<GamePresenceRankData, bool>) (x => x.FormatType == formatType)).FirstOrDefault<GamePresenceRankData>();
        gamePresenceRank1.m_currMedalInfo[formatType] = presenceRankData == null ? MedalInfoTranslator.CreateTranslatedMedalInfo(formatType, 0, 0, 0) : MedalInfoTranslator.CreateTranslatedMedalInfo(formatType, presenceRankData.LeagueId, presenceRankData.StarLevel, presenceRankData.LegendRank);
        gamePresenceRank1.m_prevMedalInfo[formatType] = gamePresenceRank1.m_currMedalInfo[formatType].ShallowCopy();
      }
    }
    return gamePresenceRank1;
  }

  public static TranslatedMedalInfo CreateTranslatedMedalInfo(
    FormatType format,
    int leagueId,
    int starLevel,
    int legendIndex)
  {
    return new TranslatedMedalInfo()
    {
      format = format,
      leagueId = leagueId,
      starLevel = starLevel,
      legendIndex = legendIndex
    };
  }

  public MedalInfoTranslator(
    NetCache.NetCacheMedalInfo currMedalInfo,
    NetCache.NetCacheMedalInfo prevMedalInfo = null)
  {
    if (currMedalInfo == null)
      return;
    Map<FormatType, MedalInfoData> medalData = currMedalInfo.MedalData;
    foreach (KeyValuePair<FormatType, MedalInfoData> keyValuePair in medalData)
    {
      MedalInfoData medalInfoData = keyValuePair.Value;
      FormatType formatType = medalInfoData.FormatType;
      this.m_currMedalInfo[formatType] = this.Translate(formatType, medalInfoData);
    }
    if (prevMedalInfo != null)
    {
      foreach (KeyValuePair<FormatType, MedalInfoData> keyValuePair in prevMedalInfo.MedalData)
      {
        MedalInfoData medalInfoData = keyValuePair.Value;
        FormatType formatType = medalInfoData.FormatType;
        this.m_prevMedalInfo[formatType] = this.Translate(formatType, medalInfoData);
      }
    }
    else
    {
      foreach (KeyValuePair<FormatType, MedalInfoData> keyValuePair in medalData)
      {
        FormatType formatType = keyValuePair.Value.FormatType;
        this.m_prevMedalInfo[formatType] = this.m_currMedalInfo[formatType].ShallowCopy();
      }
    }
  }

  private TranslatedMedalInfo Translate(
    FormatType format,
    MedalInfoData medalInfoData)
  {
    if (medalInfoData == null)
      return MedalInfoTranslator.CreateTranslatedMedalInfo(format, 0, 0, 0);
    TranslatedMedalInfo translatedMedalInfo = MedalInfoTranslator.CreateTranslatedMedalInfo(format, medalInfoData.LeagueId, medalInfoData.StarLevel, medalInfoData.HasLegendRank ? medalInfoData.LegendRank : 0);
    translatedMedalInfo.bestStarLevel = medalInfoData.BestStarLevel;
    translatedMedalInfo.earnedStars = medalInfoData.Stars;
    translatedMedalInfo.winStreak = medalInfoData.Streak;
    translatedMedalInfo.seasonId = medalInfoData.SeasonId;
    translatedMedalInfo.seasonWins = medalInfoData.SeasonWins;
    translatedMedalInfo.seasonGames = medalInfoData.SeasonGames;
    translatedMedalInfo.starsPerWin = medalInfoData.StarsPerWin;
    return translatedMedalInfo;
  }

  public static MedalInfoTranslator DebugCreateMedalInfo(
    int leagueId,
    int starLevel,
    int stars,
    int starsPerWin,
    FormatType formatType,
    bool isWinStreak,
    bool showWin)
  {
    MedalInfoTranslator medalInfoForLeagueId = MedalInfoTranslator.CreateMedalInfoForLeagueId(leagueId, starLevel, 1337);
    TranslatedMedalInfo previousMedal = medalInfoForLeagueId.GetPreviousMedal(formatType);
    TranslatedMedalInfo currentMedal = medalInfoForLeagueId.GetCurrentMedal(formatType);
    previousMedal.earnedStars = stars;
    previousMedal.starsPerWin = starsPerWin;
    currentMedal.earnedStars = stars;
    currentMedal.starsPerWin = starsPerWin;
    ++currentMedal.seasonGames;
    if (showWin)
    {
      ++currentMedal.seasonWins;
      int num1 = starsPerWin;
      if (isWinStreak)
      {
        previousMedal.winStreak = previousMedal.RankConfig.WinStreakThreshold;
        currentMedal.winStreak = previousMedal.RankConfig.WinStreakThreshold;
        num1 *= 2;
      }
      while (num1 > 0 && currentMedal.RankConfig.Stars > 0)
      {
        int num2 = Mathf.Max(currentMedal.RankConfig.Stars - currentMedal.earnedStars, 0);
        if (num1 <= num2)
        {
          currentMedal.earnedStars += num1;
          num1 = 0;
        }
        else
        {
          currentMedal.earnedStars += num2;
          num1 -= num2;
          ++currentMedal.starLevel;
          currentMedal.earnedStars = 0;
        }
      }
      ++currentMedal.legendIndex;
    }
    else
    {
      if (currentMedal.RankConfig.CanLoseStars)
      {
        if (currentMedal.earnedStars > 0)
          --currentMedal.earnedStars;
        else if (currentMedal.starLevel > 1 && currentMedal.RankConfig.CanLoseLevel)
        {
          currentMedal.earnedStars = currentMedal.GetMaxStarsAtRank() - 1;
          --currentMedal.starLevel;
        }
      }
      --currentMedal.legendIndex;
    }
    return medalInfoForLeagueId;
  }

  public TranslatedMedalInfo GetCurrentMedal(FormatType formatType)
  {
    TranslatedMedalInfo currentMedal;
    if (!this.m_currMedalInfo.TryGetValue(formatType, out currentMedal))
    {
      Debug.LogError((object) ("MedalInfoTranslator.GetCurrentMedal called for unsupported format type " + formatType.ToString() + ". Returning default TranslatedMedalInfo"));
      currentMedal = new TranslatedMedalInfo();
    }
    return currentMedal;
  }

  public TranslatedMedalInfo GetCurrentMedalForCurrentFormatType() => this.GetCurrentMedal(Options.GetFormatType());

  public TranslatedMedalInfo GetPreviousMedal(FormatType formatType)
  {
    TranslatedMedalInfo previousMedal;
    if (!this.m_prevMedalInfo.TryGetValue(formatType, out previousMedal))
    {
      Debug.LogError((object) ("MedalInfoTranslator.GetPreviousMedal called for unsupported format type " + formatType.ToString() + ". Returning default TranslatedMedalInfo"));
      previousMedal = new TranslatedMedalInfo();
    }
    return previousMedal;
  }

  public FormatType GetBestCurrentRankFormatType()
  {
    if (this.m_currMedalInfo == null || this.m_currMedalInfo.Count == 0)
    {
      Debug.LogError((object) "MedalInfoTranslator.GetBestCurrentRankFormatType had a null or empty m_currMedalInfo. Returning FT_STANDARD. Was this called before the ctor?");
      return FormatType.FT_STANDARD;
    }
    List<KeyValuePair<FormatType, TranslatedMedalInfo>> list = this.m_currMedalInfo.ToList<KeyValuePair<FormatType, TranslatedMedalInfo>>();
    list.Sort((Comparison<KeyValuePair<FormatType, TranslatedMedalInfo>>) ((f1, f2) =>
    {
      if (!f1.Value.IsValid() || !f2.Value.IsValid())
        return f1.Value.starLevel.CompareTo(f2.Value.starLevel);
      int currentRankFormatType1 = f1.Value.LeagueConfig.LeagueLevel.CompareTo(f2.Value.LeagueConfig.LeagueLevel);
      if (currentRankFormatType1 != 0)
        return currentRankFormatType1;
      if (f1.Value.IsLegendRank() && f2.Value.IsLegendRank())
      {
        int num = f1.Value.legendIndex.CompareTo(f2.Value.legendIndex);
        if (num != 0)
          return -num;
      }
      int currentRankFormatType2 = f1.Value.starLevel.CompareTo(f2.Value.starLevel);
      if (currentRankFormatType2 != 0)
        return currentRankFormatType2;
      int num1 = f1.Value.earnedStars.CompareTo(f2.Value.earnedStars);
      return num1 != 0 ? num1 : this.CompareFormatTypes(f1.Value.format, f2.Value.format);
    }));
    return list.Last<KeyValuePair<FormatType, TranslatedMedalInfo>>().Key;
  }

  private int CompareFormatTypes(FormatType f1, FormatType f2)
  {
    List<FormatType> formatTypeList = new List<FormatType>();
    formatTypeList.Add(FormatType.FT_CLASSIC);
    formatTypeList.Add(FormatType.FT_WILD);
    formatTypeList.Add(FormatType.FT_STANDARD);
    return formatTypeList.IndexOf(f1).CompareTo(formatTypeList.IndexOf(f2));
  }

  public int GetCurrentSeasonId() => this.GetCurrentMedal(FormatType.FT_STANDARD).seasonId;

  public int GetSeasonCardBackMinWins()
  {
    int a = this.GetPreviousMedal(FormatType.FT_WILD).LeagueConfig.SeasonCardBackMinWins;
    foreach (FormatType formatType in Enum.GetValues(typeof (FormatType)))
    {
      switch (formatType)
      {
        case FormatType.FT_UNKNOWN:
        case FormatType.FT_WILD:
          continue;
        default:
          a = Mathf.Min(a, this.GetPreviousMedal(formatType).LeagueConfig.SeasonCardBackMinWins);
          continue;
      }
    }
    return a;
  }

  public int GetSeasonCardBackWinsRemaining() => Mathf.Max(0, this.GetSeasonCardBackMinWins() - this.TotalRankedWins);

  public bool HasEarnedSeasonCardBack() => this.GetSeasonCardBackWinsRemaining() == 0;

  public bool ShouldShowCardBackProgress() => this.TotalRankedWins > this.TotalRankedWinsPrevious && this.TotalRankedWinsPrevious < this.GetSeasonCardBackMinWins();

  public bool GetRankedRewardsEarned(
    FormatType formatType,
    ref List<List<RewardData>> rewardsEarned)
  {
    TranslatedMedalInfo previousMedal = this.GetPreviousMedal(formatType);
    TranslatedMedalInfo currentMedal = this.GetCurrentMedal(formatType);
    if (previousMedal == null || currentMedal == null)
      return false;
    int val1 = 0;
    foreach (FormatType formatType1 in Enum.GetValues(typeof (FormatType)))
    {
      if (formatType1 != FormatType.FT_UNKNOWN && formatType1 != formatType)
        val1 = Math.Max(val1, this.GetCurrentMedal(formatType).bestStarLevel);
    }
    if (previousMedal.bestStarLevel >= currentMedal.bestStarLevel | val1 > currentMedal.bestStarLevel)
      return false;
    rewardsEarned.Clear();
    int starLevel = previousMedal.starLevel;
    while (starLevel < currentMedal.starLevel)
    {
      ++starLevel;
      LeagueRankDbfRecord leagueRankRecord = RankMgr.Get().GetLeagueRankRecord(previousMedal.leagueId, starLevel);
      List<RewardData> rewardData = new List<RewardData>();
      RewardUtils.AddRewardDataStubForBag(leagueRankRecord.RewardBagId, currentMedal.seasonId, ref rewardData);
      if (rewardData.Count > 0)
        rewardsEarned.Add(rewardData);
    }
    return true;
  }

  public RankChangeType GetChangeType(FormatType formatType)
  {
    TranslatedMedalInfo previousMedal = this.GetPreviousMedal(formatType);
    TranslatedMedalInfo currentMedal = this.GetCurrentMedal(formatType);
    if (previousMedal == null || currentMedal == null)
      return RankChangeType.UNKNOWN;
    if (currentMedal.seasonId == previousMedal.seasonId && currentMedal.seasonGames == previousMedal.seasonGames)
      return RankChangeType.NO_GAME_PLAYED;
    if (currentMedal.LeagueConfig.LeagueLevel < previousMedal.LeagueConfig.LeagueLevel)
      return RankChangeType.RANK_DOWN;
    if (currentMedal.LeagueConfig.LeagueLevel > previousMedal.LeagueConfig.LeagueLevel)
      return RankChangeType.RANK_UP;
    if (currentMedal.starLevel < previousMedal.starLevel)
      return RankChangeType.RANK_DOWN;
    return currentMedal.starLevel > previousMedal.starLevel ? RankChangeType.RANK_UP : RankChangeType.RANK_SAME;
  }

  public RankedPlayDataModel CreateDataModel(
    FormatType formatType,
    RankedMedal.DisplayMode mode,
    bool isTooltipEnabled = false,
    bool hasEarnedCardBack = false)
  {
    return this.GetCurrentMedal(formatType).CreateDataModel(mode, isTooltipEnabled, hasEarnedCardBack);
  }

  public void CreateOrUpdateDataModel(
    FormatType formatType,
    ref RankedPlayDataModel dataModel,
    RankedMedal.DisplayMode mode,
    bool isTooltipEnabled = false,
    bool hasEarnedCardBack = false)
  {
    this.GetCurrentMedal(formatType).CreateOrUpdateDataModel(ref dataModel, mode, isTooltipEnabled, hasEarnedCardBack);
  }
}
