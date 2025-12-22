using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeagueDbfRecord : DbfRecord
{
  [SerializeField]
  private League.LeagueType m_leagueType = League.ParseLeagueTypeValue("unknown");
  [SerializeField]
  private int m_leagueLevel;
  [SerializeField]
  private int m_leagueVersion;
  [SerializeField]
  private int m_initialSeasonId;
  [SerializeField]
  private League.LeagueType m_promoteToLeagueType;
  [SerializeField]
  private bool m_canPromoteSelfManually;
  [SerializeField]
  private bool m_lockWildBoosters;
  [SerializeField]
  private bool m_lockWildCards;
  [SerializeField]
  private int m_lockCardsFromSubsetId;
  [SerializeField]
  private DbfLocValue m_lockedBoosterText;
  [SerializeField]
  private DbfLocValue m_lockedCardUnplayableText;
  [SerializeField]
  private DbfLocValue m_lockedCardPopupTitleText;
  [SerializeField]
  private DbfLocValue m_lockedCardPopupBodyText;
  [SerializeField]
  private int m_seasonRollRewardMinWins;
  [SerializeField]
  private int m_seasonEndRewardChestId;
  [SerializeField]
  private int m_seasonCardBackMinWins;
  [SerializeField]
  private int m_rankedIntroSeenRequirement;
  [SerializeField]
  private int m_bonusStarsPopupSeenRequirement;
  [SerializeField]
  private int m_rewardsVersion;

  [DbfField("LEAGUE_TYPE")]
  public League.LeagueType LeagueType => this.m_leagueType;

  [DbfField("LEAGUE_LEVEL")]
  public int LeagueLevel => this.m_leagueLevel;

  [DbfField("LEAGUE_VERSION")]
  public int LeagueVersion => this.m_leagueVersion;

  [DbfField("INITIAL_SEASON_ID")]
  public int InitialSeasonId => this.m_initialSeasonId;

  [DbfField("CAN_PROMOTE_SELF_MANUALLY")]
  public bool CanPromoteSelfManually => this.m_canPromoteSelfManually;

  [DbfField("LOCK_WILD_CARDS")]
  public bool LockWildCards => this.m_lockWildCards;

  [DbfField("LOCK_CARDS_FROM_SUBSET_ID")]
  public int LockCardsFromSubsetId => this.m_lockCardsFromSubsetId;

  [DbfField("LOCKED_CARD_UNPLAYABLE_TEXT")]
  public DbfLocValue LockedCardUnplayableText => this.m_lockedCardUnplayableText;

  [DbfField("LOCKED_CARD_POPUP_TITLE_TEXT")]
  public DbfLocValue LockedCardPopupTitleText => this.m_lockedCardPopupTitleText;

  [DbfField("LOCKED_CARD_POPUP_BODY_TEXT")]
  public DbfLocValue LockedCardPopupBodyText => this.m_lockedCardPopupBodyText;

  [DbfField("SEASON_ROLL_REWARD_MIN_WINS")]
  public int SeasonRollRewardMinWins => this.m_seasonRollRewardMinWins;

  [DbfField("SEASON_CARD_BACK_MIN_WINS")]
  public int SeasonCardBackMinWins => this.m_seasonCardBackMinWins;

  [DbfField("RANKED_INTRO_SEEN_REQUIREMENT")]
  public int RankedIntroSeenRequirement => this.m_rankedIntroSeenRequirement;

  [DbfField("REWARDS_VERSION")]
  public int RewardsVersion => this.m_rewardsVersion;

  public List<LeagueGameTypeDbfRecord> LeagueGameType
  {
    get
    {
      int id = this.ID;
      List<LeagueGameTypeDbfRecord> leagueGameType = new List<LeagueGameTypeDbfRecord>();
      List<LeagueGameTypeDbfRecord> records = GameDbf.LeagueGameType.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LeagueGameTypeDbfRecord gameTypeDbfRecord = records[index];
        if (gameTypeDbfRecord.LeagueId == id)
          leagueGameType.Add(gameTypeDbfRecord);
      }
      return leagueGameType;
    }
  }

  public List<LeagueRankDbfRecord> Ranks
  {
    get
    {
      int id = this.ID;
      List<LeagueRankDbfRecord> ranks = new List<LeagueRankDbfRecord>();
      List<LeagueRankDbfRecord> records = GameDbf.LeagueRank.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LeagueRankDbfRecord leagueRankDbfRecord = records[index];
        if (leagueRankDbfRecord.LeagueId == id)
          ranks.Add(leagueRankDbfRecord);
      }
      return ranks;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BONUS_STARS_POPUP_SEEN_REQUIREMENT":
        return (object) this.m_bonusStarsPopupSeenRequirement;
      case "CAN_PROMOTE_SELF_MANUALLY":
        return (object) this.m_canPromoteSelfManually;
      case "ID":
        return (object) this.ID;
      case "INITIAL_SEASON_ID":
        return (object) this.m_initialSeasonId;
      case "LEAGUE_LEVEL":
        return (object) this.m_leagueLevel;
      case "LEAGUE_TYPE":
        return (object) this.m_leagueType;
      case "LEAGUE_VERSION":
        return (object) this.m_leagueVersion;
      case "LOCKED_BOOSTER_TEXT":
        return (object) this.m_lockedBoosterText;
      case "LOCKED_CARD_POPUP_BODY_TEXT":
        return (object) this.m_lockedCardPopupBodyText;
      case "LOCKED_CARD_POPUP_TITLE_TEXT":
        return (object) this.m_lockedCardPopupTitleText;
      case "LOCKED_CARD_UNPLAYABLE_TEXT":
        return (object) this.m_lockedCardUnplayableText;
      case "LOCK_CARDS_FROM_SUBSET_ID":
        return (object) this.m_lockCardsFromSubsetId;
      case "LOCK_WILD_BOOSTERS":
        return (object) this.m_lockWildBoosters;
      case "LOCK_WILD_CARDS":
        return (object) this.m_lockWildCards;
      case "PROMOTE_TO_LEAGUE_TYPE":
        return (object) this.m_promoteToLeagueType;
      case "RANKED_INTRO_SEEN_REQUIREMENT":
        return (object) this.m_rankedIntroSeenRequirement;
      case "REWARDS_VERSION":
        return (object) this.m_rewardsVersion;
      case "SEASON_CARD_BACK_MIN_WINS":
        return (object) this.m_seasonCardBackMinWins;
      case "SEASON_END_REWARD_CHEST_ID":
        return (object) this.m_seasonEndRewardChestId;
      case "SEASON_ROLL_REWARD_MIN_WINS":
        return (object) this.m_seasonRollRewardMinWins;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 132552325:
        if (!(name == "LEAGUE_LEVEL"))
          break;
        this.m_leagueLevel = (int) val;
        break;
      case 361397132:
        if (!(name == "LOCKED_CARD_POPUP_TITLE_TEXT"))
          break;
        this.m_lockedCardPopupTitleText = (DbfLocValue) val;
        break;
      case 746782651:
        if (!(name == "LOCK_WILD_CARDS"))
          break;
        this.m_lockWildCards = (bool) val;
        break;
      case 845898672:
        if (!(name == "REWARDS_VERSION"))
          break;
        this.m_rewardsVersion = (int) val;
        break;
      case 849258464:
        if (!(name == "PROMOTE_TO_LEAGUE_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_promoteToLeagueType = League.LeagueType.UNKNOWN;
            return;
          case League.LeagueType _:
          case int _:
            this.m_promoteToLeagueType = (League.LeagueType) val;
            return;
          case string _:
            this.m_promoteToLeagueType = League.ParseLeagueTypeValue((string) val);
            return;
          default:
            return;
        }
      case 953089362:
        if (!(name == "LOCK_CARDS_FROM_SUBSET_ID"))
          break;
        this.m_lockCardsFromSubsetId = (int) val;
        break;
      case 1238047468:
        if (!(name == "LOCKED_CARD_UNPLAYABLE_TEXT"))
          break;
        this.m_lockedCardUnplayableText = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1682456859:
        if (!(name == "RANKED_INTRO_SEEN_REQUIREMENT"))
          break;
        this.m_rankedIntroSeenRequirement = (int) val;
        break;
      case 1701516602:
        if (!(name == "LOCKED_CARD_POPUP_BODY_TEXT"))
          break;
        this.m_lockedCardPopupBodyText = (DbfLocValue) val;
        break;
      case 1949384501:
        if (!(name == "LEAGUE_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_leagueType = League.LeagueType.UNKNOWN;
            return;
          case League.LeagueType _:
          case int _:
            this.m_leagueType = (League.LeagueType) val;
            return;
          case string _:
            this.m_leagueType = League.ParseLeagueTypeValue((string) val);
            return;
          default:
            return;
        }
      case 2290669199:
        if (!(name == "INITIAL_SEASON_ID"))
          break;
        this.m_initialSeasonId = (int) val;
        break;
      case 2303752987:
        if (!(name == "BONUS_STARS_POPUP_SEEN_REQUIREMENT"))
          break;
        this.m_bonusStarsPopupSeenRequirement = (int) val;
        break;
      case 2526651862:
        if (!(name == "SEASON_END_REWARD_CHEST_ID"))
          break;
        this.m_seasonEndRewardChestId = (int) val;
        break;
      case 2657574562:
        if (!(name == "LOCKED_BOOSTER_TEXT"))
          break;
        this.m_lockedBoosterText = (DbfLocValue) val;
        break;
      case 3281464495:
        if (!(name == "LEAGUE_VERSION"))
          break;
        this.m_leagueVersion = (int) val;
        break;
      case 3426902054:
        if (!(name == "SEASON_CARD_BACK_MIN_WINS"))
          break;
        this.m_seasonCardBackMinWins = (int) val;
        break;
      case 3511510115:
        if (!(name == "CAN_PROMOTE_SELF_MANUALLY"))
          break;
        this.m_canPromoteSelfManually = (bool) val;
        break;
      case 3534282901:
        if (!(name == "LOCK_WILD_BOOSTERS"))
          break;
        this.m_lockWildBoosters = (bool) val;
        break;
      case 3884215779:
        if (!(name == "SEASON_ROLL_REWARD_MIN_WINS"))
          break;
        this.m_seasonRollRewardMinWins = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "BONUS_STARS_POPUP_SEEN_REQUIREMENT":
        return typeof (int);
      case "CAN_PROMOTE_SELF_MANUALLY":
        return typeof (bool);
      case "ID":
        return typeof (int);
      case "INITIAL_SEASON_ID":
        return typeof (int);
      case "LEAGUE_LEVEL":
        return typeof (int);
      case "LEAGUE_TYPE":
        return typeof (League.LeagueType);
      case "LEAGUE_VERSION":
        return typeof (int);
      case "LOCKED_BOOSTER_TEXT":
        return typeof (DbfLocValue);
      case "LOCKED_CARD_POPUP_BODY_TEXT":
        return typeof (DbfLocValue);
      case "LOCKED_CARD_POPUP_TITLE_TEXT":
        return typeof (DbfLocValue);
      case "LOCKED_CARD_UNPLAYABLE_TEXT":
        return typeof (DbfLocValue);
      case "LOCK_CARDS_FROM_SUBSET_ID":
        return typeof (int);
      case "LOCK_WILD_BOOSTERS":
        return typeof (bool);
      case "LOCK_WILD_CARDS":
        return typeof (bool);
      case "PROMOTE_TO_LEAGUE_TYPE":
        return typeof (League.LeagueType);
      case "RANKED_INTRO_SEEN_REQUIREMENT":
        return typeof (int);
      case "REWARDS_VERSION":
        return typeof (int);
      case "SEASON_CARD_BACK_MIN_WINS":
        return typeof (int);
      case "SEASON_END_REWARD_CHEST_ID":
        return typeof (int);
      case "SEASON_ROLL_REWARD_MIN_WINS":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLeagueDbfRecords loadRecords = new LoadLeagueDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LeagueDbfAsset leagueDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LeagueDbfAsset)) as LeagueDbfAsset;
    if ((UnityEngine.Object) leagueDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LeagueDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < leagueDbfAsset.Records.Count; ++index)
      leagueDbfAsset.Records[index].StripUnusedLocales();
    records = leagueDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_lockedBoosterText.StripUnusedLocales();
    this.m_lockedCardUnplayableText.StripUnusedLocales();
    this.m_lockedCardPopupTitleText.StripUnusedLocales();
    this.m_lockedCardPopupBodyText.StripUnusedLocales();
  }
}
