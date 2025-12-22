using Assets;
using Hearthstone.DataModels;
using UnityEngine;

public class TranslatedMedalInfo
{
  public int leagueId;
  public int earnedStars;
  public int starLevel;
  public int bestStarLevel;
  public int winStreak;
  public int legendIndex;
  public int seasonId;
  public int seasonWins;
  public int seasonGames;
  public int bestEverLeagueId;
  public int bestEverStarLevel;
  public int starsPerWin;
  public PegasusShared.FormatType format;

  public LeagueDbfRecord LeagueConfig => RankMgr.Get().GetLeagueRecord(this.leagueId);

  public LeagueRankDbfRecord RankConfig => RankMgr.Get().GetLeagueRankRecord(this.leagueId, this.starLevel);

  public TranslatedMedalInfo ShallowCopy() => this.MemberwiseClone() as TranslatedMedalInfo;

  public bool IsLegendRank() => this.RankConfig.ShowIndividualRanking;

  public bool IsNewPlayer() => this.LeagueConfig.LeagueType == League.LeagueType.NEW_PLAYER;

  public PegasusShared.FormatType GetFormatType() => this.format;

  public bool CanLoseStars() => this.RankConfig.CanLoseStars;

  public bool CanLoseLevel() => this.RankConfig.CanLoseLevel;

  public string GetRankName() => this.RankConfig.RankName != null ? this.RankConfig.RankName.GetString() : string.Empty;

  public string GetMedalText() => this.RankConfig.MedalText != null ? this.RankConfig.MedalText.GetString() : string.Empty;

  public int GetMaxStarsAtRank() => this.RankConfig.Stars;

  public bool IsValid() => this.starLevel >= 1;

  public void CreateOrUpdateDataModel(
    ref RankedPlayDataModel dataModel,
    RankedMedal.DisplayMode mode,
    bool isTooltipEnabled = false,
    bool hasEarnedCardBack = false)
  {
    if (dataModel == null)
      dataModel = this.CreateDataModel(mode, isTooltipEnabled, hasEarnedCardBack);
    else
      this.UpdateDataModel(dataModel, mode, isTooltipEnabled, hasEarnedCardBack);
  }

  public RankedPlayDataModel CreateDataModel(
    RankedMedal.DisplayMode mode,
    bool isTooltipEnabled = false,
    bool hasEarnedCardBack = false)
  {
    RankedPlayDataModel dataModel = new RankedPlayDataModel();
    this.UpdateDataModel(dataModel, mode, isTooltipEnabled, hasEarnedCardBack);
    return dataModel;
  }

  public void UpdateDataModel(
    RankedPlayDataModel dataModel,
    RankedMedal.DisplayMode mode,
    bool isTooltipEnabled,
    bool hasEarnedCardBack)
  {
    if (dataModel == null)
    {
      Debug.LogError((object) "TranslatedMedalInfo.UpdateDataModel - ranked play data model was null!");
    }
    else
    {
      dataModel.DisplayMode = mode;
      dataModel.IsTooltipEnabled = isTooltipEnabled;
      dataModel.HasEarnedCardBack = hasEarnedCardBack;
      dataModel.Stars = this.earnedStars;
      dataModel.MaxStars = this.RankConfig.Stars;
      dataModel.StarMultiplier = this.starsPerWin;
      dataModel.StarLevel = this.starLevel;
      dataModel.MedalText = this.GetMedalText();
      dataModel.RankName = this.GetRankName();
      dataModel.IsNewPlayer = this.IsNewPlayer();
      dataModel.IsLegend = this.IsLegendRank();
      dataModel.LegendRank = this.legendIndex;
      dataModel.FormatType = this.GetFormatType();
      if (mode == RankedMedal.DisplayMode.Chest || string.IsNullOrEmpty(this.RankConfig.MedalTexture))
        return;
      ObjectCallback callback = (ObjectCallback) ((assetRef, textureObj, data) => dataModel.MedalTexture = textureObj as Texture);
      AssetLoader.Get().LoadTexture((AssetReference) this.RankConfig.MedalTexture, callback);
    }
  }

  public override string ToString() => string.Format("[leagueId={0} starLevel={1} earnedStars={2} starsPerWin={3}]", (object) this.leagueId, (object) this.starLevel, (object) this.earnedStars, (object) this.starsPerWin);
}
