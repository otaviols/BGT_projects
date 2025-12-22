using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeagueRankDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_leagueId;
  [SerializeField]
  private int m_starLevel;
  [SerializeField]
  private int m_stars;
  [SerializeField]
  private bool m_showIndividualRanking;
  [SerializeField]
  private DbfLocValue m_rankName;
  [SerializeField]
  private DbfLocValue m_medalText;
  [SerializeField]
  private string m_medalTexture;
  [SerializeField]
  private string m_medalMaterial;
  [SerializeField]
  private string m_cheatName;
  [SerializeField]
  private bool m_canLoseStars;
  [SerializeField]
  private bool m_canLoseLevel;
  [SerializeField]
  private int m_maxBestEverStarLevel;
  [SerializeField]
  private int m_winStreakThreshold;
  [SerializeField]
  private int m_rewardChestIdV1Id;
  [SerializeField]
  private int m_rewardBagId;
  [SerializeField]
  private int m_rewardChestVisualIndex;
  [SerializeField]
  private bool m_showToastOnAttained;
  [SerializeField]
  private bool m_showOpponentRankInGame;

  [DbfField("LEAGUE_ID")]
  public int LeagueId => this.m_leagueId;

  [DbfField("STAR_LEVEL")]
  public int StarLevel => this.m_starLevel;

  [DbfField("STARS")]
  public int Stars => this.m_stars;

  [DbfField("SHOW_INDIVIDUAL_RANKING")]
  public bool ShowIndividualRanking => this.m_showIndividualRanking;

  [DbfField("RANK_NAME")]
  public DbfLocValue RankName => this.m_rankName;

  [DbfField("MEDAL_TEXT")]
  public DbfLocValue MedalText => this.m_medalText;

  [DbfField("MEDAL_TEXTURE")]
  public string MedalTexture => this.m_medalTexture;

  [DbfField("CHEAT_NAME")]
  public string CheatName => this.m_cheatName;

  [DbfField("CAN_LOSE_STARS")]
  public bool CanLoseStars => this.m_canLoseStars;

  [DbfField("CAN_LOSE_LEVEL")]
  public bool CanLoseLevel => this.m_canLoseLevel;

  [DbfField("WIN_STREAK_THRESHOLD")]
  public int WinStreakThreshold => this.m_winStreakThreshold;

  [DbfField("REWARD_BAG_ID")]
  public int RewardBagId => this.m_rewardBagId;

  [DbfField("REWARD_CHEST_VISUAL_INDEX")]
  public int RewardChestVisualIndex => this.m_rewardChestVisualIndex;

  [DbfField("SHOW_TOAST_ON_ATTAINED")]
  public bool ShowToastOnAttained => this.m_showToastOnAttained;

  [DbfField("SHOW_OPPONENT_RANK_IN_GAME")]
  public bool ShowOpponentRankInGame => this.m_showOpponentRankInGame;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "CAN_LOSE_LEVEL":
        return (object) this.m_canLoseLevel;
      case "CAN_LOSE_STARS":
        return (object) this.m_canLoseStars;
      case "CHEAT_NAME":
        return (object) this.m_cheatName;
      case "ID":
        return (object) this.ID;
      case "LEAGUE_ID":
        return (object) this.m_leagueId;
      case "MAX_BEST_EVER_STAR_LEVEL":
        return (object) this.m_maxBestEverStarLevel;
      case "MEDAL_MATERIAL":
        return (object) this.m_medalMaterial;
      case "MEDAL_TEXT":
        return (object) this.m_medalText;
      case "MEDAL_TEXTURE":
        return (object) this.m_medalTexture;
      case "RANK_NAME":
        return (object) this.m_rankName;
      case "REWARD_BAG_ID":
        return (object) this.m_rewardBagId;
      case "REWARD_CHEST_ID_V1":
        return (object) this.m_rewardChestIdV1Id;
      case "REWARD_CHEST_VISUAL_INDEX":
        return (object) this.m_rewardChestVisualIndex;
      case "SHOW_INDIVIDUAL_RANKING":
        return (object) this.m_showIndividualRanking;
      case "SHOW_OPPONENT_RANK_IN_GAME":
        return (object) this.m_showOpponentRankInGame;
      case "SHOW_TOAST_ON_ATTAINED":
        return (object) this.m_showToastOnAttained;
      case "STARS":
        return (object) this.m_stars;
      case "STAR_LEVEL":
        return (object) this.m_starLevel;
      case "WIN_STREAK_THRESHOLD":
        return (object) this.m_winStreakThreshold;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 65678001:
        if (!(name == "CAN_LOSE_STARS"))
          break;
        this.m_canLoseStars = (bool) val;
        break;
      case 177512838:
        if (!(name == "STARS"))
          break;
        this.m_stars = (int) val;
        break;
      case 258923658:
        if (!(name == "MEDAL_TEXT"))
          break;
        this.m_medalText = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1655746952:
        if (!(name == "STAR_LEVEL"))
          break;
        this.m_starLevel = (int) val;
        break;
      case 1800900461:
        if (!(name == "REWARD_BAG_ID"))
          break;
        this.m_rewardBagId = (int) val;
        break;
      case 1810449186:
        if (!(name == "CHEAT_NAME"))
          break;
        this.m_cheatName = (string) val;
        break;
      case 2451523201:
        if (!(name == "SHOW_TOAST_ON_ATTAINED"))
          break;
        this.m_showToastOnAttained = (bool) val;
        break;
      case 2628940820:
        if (!(name == "REWARD_CHEST_ID_V1"))
          break;
        this.m_rewardChestIdV1Id = (int) val;
        break;
      case 2639515172:
        if (!(name == "MEDAL_MATERIAL"))
          break;
        this.m_medalMaterial = (string) val;
        break;
      case 2748631019:
        if (!(name == "MAX_BEST_EVER_STAR_LEVEL"))
          break;
        this.m_maxBestEverStarLevel = (int) val;
        break;
      case 2854743859:
        if (!(name == "SHOW_INDIVIDUAL_RANKING"))
          break;
        this.m_showIndividualRanking = (bool) val;
        break;
      case 2875816430:
        if (!(name == "CAN_LOSE_LEVEL"))
          break;
        this.m_canLoseLevel = (bool) val;
        break;
      case 3100184203:
        if (!(name == "RANK_NAME"))
          break;
        this.m_rankName = (DbfLocValue) val;
        break;
      case 3353298088:
        if (!(name == "LEAGUE_ID"))
          break;
        this.m_leagueId = (int) val;
        break;
      case 3518094994:
        if (!(name == "REWARD_CHEST_VISUAL_INDEX"))
          break;
        this.m_rewardChestVisualIndex = (int) val;
        break;
      case 3826267352:
        if (!(name == "MEDAL_TEXTURE"))
          break;
        this.m_medalTexture = (string) val;
        break;
      case 4013976586:
        if (!(name == "SHOW_OPPONENT_RANK_IN_GAME"))
          break;
        this.m_showOpponentRankInGame = (bool) val;
        break;
      case 4137139096:
        if (!(name == "WIN_STREAK_THRESHOLD"))
          break;
        this.m_winStreakThreshold = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "CAN_LOSE_LEVEL":
        return typeof (bool);
      case "CAN_LOSE_STARS":
        return typeof (bool);
      case "CHEAT_NAME":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "LEAGUE_ID":
        return typeof (int);
      case "MAX_BEST_EVER_STAR_LEVEL":
        return typeof (int);
      case "MEDAL_MATERIAL":
        return typeof (string);
      case "MEDAL_TEXT":
        return typeof (DbfLocValue);
      case "MEDAL_TEXTURE":
        return typeof (string);
      case "RANK_NAME":
        return typeof (DbfLocValue);
      case "REWARD_BAG_ID":
        return typeof (int);
      case "REWARD_CHEST_ID_V1":
        return typeof (int);
      case "REWARD_CHEST_VISUAL_INDEX":
        return typeof (int);
      case "SHOW_INDIVIDUAL_RANKING":
        return typeof (bool);
      case "SHOW_OPPONENT_RANK_IN_GAME":
        return typeof (bool);
      case "SHOW_TOAST_ON_ATTAINED":
        return typeof (bool);
      case "STARS":
        return typeof (int);
      case "STAR_LEVEL":
        return typeof (int);
      case "WIN_STREAK_THRESHOLD":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLeagueRankDbfRecords loadRecords = new LoadLeagueRankDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LeagueRankDbfAsset leagueRankDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LeagueRankDbfAsset)) as LeagueRankDbfAsset;
    if ((UnityEngine.Object) leagueRankDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LeagueRankDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < leagueRankDbfAsset.Records.Count; ++index)
      leagueRankDbfAsset.Records[index].StripUnusedLocales();
    records = leagueRankDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_rankName.StripUnusedLocales();
    this.m_medalText.StripUnusedLocales();
  }
}
