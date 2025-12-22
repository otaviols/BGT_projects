using Assets;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using PegasusClient;
using PegasusFSG;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TavernBrawlManager : IService
{
  private TavernBrawlMission[] m_missions = new TavernBrawlMission[3];
  private bool[] m_downloadableDbfAssetsPendingLoad = new bool[3];
  private TavernBrawlPlayerRecord[] m_playerRecords = new TavernBrawlPlayerRecord[3];
  private DateTime?[] m_scheduledRefreshTimes = new DateTime?[3];
  private DateTime?[] m_nextSeasonStartDates = new DateTime?[3];
  private int?[] m_latestSeenSeasonThisSession = new int?[3];
  private int?[] m_latestSeenChalkboardThisSession = new int?[3];
  private BrawlType m_currentBrawlType = BrawlType.BRAWL_TYPE_TAVERN_BRAWL;
  private List<TavernBrawlManager.CallbackEnsureServerDataReady> m_serverDataReadyCallbacks;
  private bool m_hasGottenClientOptionsAtLeastOnce;
  private bool m_isFirstTimeSeeingThisFeature;
  private bool m_isFirstTimeSeeingCurrentSeason;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TavernBrawlManager tavernBrawlManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Network network = serviceLocator.Get<Network>();
    NetCache netCache = NetCache.Get();
    network.RegisterNetHandler((object) TavernBrawlRequestSessionBeginResponse.PacketID.ID, new Network.NetHandler(tavernBrawlManager.OnBeginSession));
    network.RegisterNetHandler((object) TavernBrawlRequestSessionRetireResponse.PacketID.ID, new Network.NetHandler(tavernBrawlManager.OnRetireSession));
    network.RegisterNetHandler((object) TavernBrawlSessionAckRewardsResponse.PacketID.ID, new Network.NetHandler(tavernBrawlManager.OnAckRewards));
    network.RegisterNetHandler((object) TavernBrawlPlayerRecordResponse.PacketID.ID, new Network.NetHandler(tavernBrawlManager.OnTavernBrawlRecord));
    network.RegisterNetHandler((object) TavernBrawlInfo.PacketID.ID, new Network.NetHandler(tavernBrawlManager.OnTavernBrawlInfo));
    network.RegisterNetHandler((object) CheckInToFSGResponse.PacketID.ID, new Network.NetHandler(tavernBrawlManager.OnCheckInToFSGResponse));
    netCache.RegisterUpdatedListener(typeof (NetCache.NetCacheHeroLevels), new Action(tavernBrawlManager.NetCache_OnClientOptions));
    serviceLocator.Get<GameMgr>().RegisterFindGameEvent(new GameMgr.FindGameCallback(tavernBrawlManager.OnFindGameEvent));
    tavernBrawlManager.RegisterOptionsListeners(true);
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[3]
  {
    typeof (Network),
    typeof (GameMgr),
    typeof (NetCache)
  };

  public void Shutdown()
  {
  }

  public static TavernBrawlManager Get() => ServiceManager.Get<TavernBrawlManager>();

  public event Action OnTavernBrawlUpdated;

  public event TavernBrawlManager.TavernBrawlSessionLimitRaisedCallback OnSessionLimitRaised;

  public BrawlType CurrentBrawlType
  {
    get => this.m_currentBrawlType;
    set
    {
      if (value < BrawlType.BRAWL_TYPE_TAVERN_BRAWL || value >= BrawlType.BRAWL_TYPE_COUNT)
        return;
      this.m_currentBrawlType = value;
    }
  }

  public bool IsCurrentBrawlTypeActive => this.IsTavernBrawlActive(this.m_currentBrawlType);

  public TavernBrawlMission CurrentMission() => this.GetMission(this.m_currentBrawlType);

  public TavernBrawlMission GetMission(BrawlType brawlType) => brawlType < BrawlType.BRAWL_TYPE_TAVERN_BRAWL || brawlType >= BrawlType.BRAWL_TYPE_COUNT ? (TavernBrawlMission) null : this.m_missions[(int) brawlType];

  public bool SelectHeroBeforeMission() => this.SelectHeroBeforeMission(this.m_currentBrawlType);

  public bool SelectHeroBeforeMission(BrawlType brawlType) => this.GetMission(brawlType) != null && this.GetMission(brawlType).canSelectHeroForDeck && !this.GetMission(brawlType).canCreateDeck;

  public static bool IsInTavernBrawlFriendlyChallenge() => (SceneMgr.Get().IsInTavernBrawlMode() || SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY) && FriendChallengeMgr.Get().IsChallengeTavernBrawl();

  public bool IsFirstTimeSeeingThisFeature => this.m_isFirstTimeSeeingThisFeature && this.IsCurrentBrawlTypeActive;

  public bool IsFirstTimeSeeingCurrentSeason => this.IsCurrentBrawlTypeActive && this.m_isFirstTimeSeeingCurrentSeason;

  public int LatestSeenTavernBrawlSeason
  {
    get
    {
      if (this.m_latestSeenSeasonThisSession[(int) this.m_currentBrawlType].HasValue)
        return this.m_latestSeenSeasonThisSession[(int) this.m_currentBrawlType].Value;
      Option option = Option.LATEST_SEEN_TAVERNBRAWL_SEASON;
      if (this.m_currentBrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
        option = Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON;
      return Options.Get().GetInt(option);
    }
    set
    {
      this.m_latestSeenSeasonThisSession[(int) this.m_currentBrawlType] = new int?(value);
      if (value > 100000)
        return;
      Option option = Option.LATEST_SEEN_TAVERNBRAWL_SEASON;
      if (this.m_currentBrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
        option = Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON;
      Options.Get().SetInt(option, value);
    }
  }

  public int LatestSeenTavernBrawlChalkboard
  {
    get
    {
      if (this.m_latestSeenChalkboardThisSession[(int) this.m_currentBrawlType].HasValue)
        return this.m_latestSeenChalkboardThisSession[(int) this.m_currentBrawlType].Value;
      Option option = Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD;
      if (this.m_currentBrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
        option = Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON_CHALKBOARD;
      return Options.Get().GetInt(option);
    }
    set
    {
      this.m_latestSeenChalkboardThisSession[(int) this.m_currentBrawlType] = new int?(value);
      if (value > 100000)
        return;
      Option option = Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD;
      if (this.m_currentBrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
        option = Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON_CHALKBOARD;
      Options.Get().SetInt(option, value);
    }
  }

  public long CurrentTavernBrawlSeasonEndInSeconds => this.TavernBrawlSeasonEndInSeconds(this.m_currentBrawlType);

  public long NextTavernBrawlSeasonStartInSeconds => this.TavernBrawlSeasonStartInSeconds(this.m_currentBrawlType);

  public float CurrentScheduledSecondsToRefresh => this.ScheduledSecondsToRefresh(this.m_currentBrawlType);

  public bool IsDeckLocked
  {
    get
    {
      CollectionDeck collectionDeck = this.CurrentDeck();
      return collectionDeck != null && collectionDeck.Locked;
    }
  }

  public bool IsCurrentSeasonSessionBased => this.IsSeasonSessionBased(this.m_currentBrawlType);

  public TavernBrawlMode CurrentSeasonBrawlMode => this.GetBrawlModeForBrawlType(this.m_currentBrawlType);

  public long CurrentTavernBrawlSeasonNewSessionsClosedInSeconds => this.TavernBrawlSeasonNewSessionsClosedInSeconds(this.CurrentBrawlType);

  public TavernBrawlPlayerRecord GetRecord(BrawlType brawlType) => brawlType < BrawlType.BRAWL_TYPE_TAVERN_BRAWL || brawlType >= BrawlType.BRAWL_TYPE_COUNT ? (TavernBrawlPlayerRecord) null : this.m_playerRecords[(int) brawlType];

  public bool IsCurrentTavernBrawlSeasonClosedToPlayer => this.CurrentTavernBrawlSeasonNewSessionsClosedInSeconds < 0L && this.MyRecord != null && (!this.MyRecord.HasNumTicketsOwned || this.MyRecord.NumTicketsOwned <= 0) && this.PlayerStatus != TavernBrawlStatus.TB_STATUS_ACTIVE && this.PlayerStatus != TavernBrawlStatus.TB_STATUS_IN_REWARDS;

  public bool IsPlayerAtSessionMaxForCurrentTavernBrawl
  {
    get
    {
      int num1 = this.IsCurrentSeasonSessionBased ? 1 : 0;
      bool flag1 = this.NumSessionsAvailableForPurchase == 0;
      bool flag2 = this.NumSessionsAllowedThisSeason == 0;
      bool flag3 = this.PlayerStatus == TavernBrawlStatus.TB_STATUS_TICKET_REQUIRED;
      bool flag4 = this.NumTicketsOwned == 0;
      int num2 = flag1 ? 1 : 0;
      return (((num1 & num2) == 0 ? 0 : (!flag2 ? 1 : 0)) & (flag3 ? 1 : 0) & (flag4 ? 1 : 0)) != 0;
    }
  }

  public TavernBrawlStatus PlayerStatus => this.MyRecord != null && this.MyRecord.HasSessionStatus ? this.MyRecord.SessionStatus : TavernBrawlStatus.TB_STATUS_INVALID;

  public int NumTicketsOwned => this.MyRecord != null && this.MyRecord.HasNumTicketsOwned ? this.MyRecord.NumTicketsOwned : 0;

  public int NumSessionsAllowedThisSeason => this.CurrentMission() != null ? this.CurrentMission().maxSessions : -1;

  public int NumSessionsAvailableForPurchase => this.MyRecord != null && this.MyRecord.HasNumSessionsPurchasable ? this.MyRecord.NumSessionsPurchasable : 0;

  public TavernBrawlPlayerSession CurrentSession => this.MyRecord != null && this.MyRecord.HasSession ? this.MyRecord.Session : (TavernBrawlPlayerSession) null;

  public int GamesWon => this.CurrentMission().IsSessionBased ? (this.CurrentSession != null ? this.CurrentSession.Wins : 0) : (this.MyRecord != null ? this.MyRecord.GamesWon : 0);

  public int GamesLost
  {
    get
    {
      if (!this.CurrentMission().IsSessionBased)
        return this.GamesPlayed - this.GamesWon;
      return this.CurrentSession != null ? this.CurrentSession.Losses : 0;
    }
  }

  public int GamesPlayed => this.MyRecord != null && this.MyRecord.HasGamesPlayed ? this.MyRecord.GamesPlayed : 0;

  public int RewardProgress => this.MyRecord != null ? this.MyRecord.RewardProgress : 0;

  public string EndingTimeText
  {
    get
    {
      long seconds = this.CurrentMission() == null ? -1L : this.CurrentTavernBrawlSeasonEndInSeconds;
      if (seconds < 0L)
        return (string) null;
      TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
      {
        m_seconds = "GLUE_TAVERN_BRAWL_LABEL_ENDING_SECONDS",
        m_minutes = "GLUE_TAVERN_BRAWL_LABEL_ENDING_MINUTES",
        m_hours = "GLUE_TAVERN_BRAWL_LABEL_ENDING_HOURS",
        m_yesterday = (string) null,
        m_days = "GLUE_TAVERN_BRAWL_LABEL_ENDING_DAYS",
        m_weeks = "GLUE_TAVERN_BRAWL_LABEL_ENDING_WEEKS",
        m_monthAgo = "GLUE_TAVERN_BRAWL_LABEL_ENDING_OVER_1_MONTH"
      };
      return TimeUtils.GetElapsedTimeString((int) seconds, stringSet, true);
    }
  }

  public List<TavernBrawlMission> Missions => ((IEnumerable<TavernBrawlMission>) this.m_missions).Where<TavernBrawlMission>((Func<TavernBrawlMission, bool>) (m => m != null)).ToList<TavernBrawlMission>();

  public bool CanEnterStandardTavernBrawl(out string reason)
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject == null)
    {
      Debug.LogWarning((object) "TavernBrawlManager:CanEnterStandardTavernBrawl: NetCacheFeatures have not finished loading prior to this function call");
      reason = GameStrings.Get("GLUE_TOOLTIP_GAME_MODE_DATA_NOT_LOADED");
      return false;
    }
    if (NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>() == null)
    {
      Debug.LogWarning((object) "TavernBrawlManager:CanEnterStandardTavernBrawl: NetCacheHeroLevels have not finished loading prior to this function call");
      reason = GameStrings.Get("GLUE_TOOLTIP_GAME_MODE_DATA_NOT_LOADED");
      return false;
    }
    if (!GameUtils.IsTraditionalTutorialComplete())
    {
      reason = GameStrings.Get("GLUE_TAVERN_BRAWL_TRADITIONAL_TUTORIAL_INCOMPLETE");
      return false;
    }
    if (!netObject.Games.TavernBrawl)
    {
      reason = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
      return false;
    }
    if (!this.HasUnlockedTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
    {
      reason = GameStrings.Format("GLUE_TAVERN_BRAWL_NO_HERO_AT_MINIMUM_LEVEL", (object) 20);
      return false;
    }
    if (!this.IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
    {
      reason = GameStrings.Get("GLUE_HEROIC_BRAWL_SIGNUPS_CLOSED");
      return false;
    }
    if (this.IsCurrentTavernBrawlSeasonClosedToPlayer)
    {
      reason = GameStrings.Get("GLUE_HEROIC_BRAWL_SIGNUPS_CLOSED");
      return false;
    }
    if (this.IsPlayerAtSessionMaxForCurrentTavernBrawl)
    {
      reason = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_LIMIT_ALERT_LIMIT_HIT");
      return false;
    }
    reason = "";
    return true;
  }

  public string GetStartingTimeText(bool singleLine = false)
  {
    long seasonStartInSeconds = this.NextTavernBrawlSeasonStartInSeconds;
    if (seasonStartInSeconds < 0L)
      return GameStrings.Get("GLUE_TAVERN_BRAWL_RETURNS_UNKNOWN" + (singleLine ? "_SINGLE_LINE" : ""));
    TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
    {
      m_seconds = "GLUE_TAVERN_BRAWL_RETURNS_LESS_THAN_1_HOUR",
      m_minutes = "GLUE_TAVERN_BRAWL_RETURNS_LESS_THAN_1_HOUR",
      m_hours = "GLUE_TAVERN_BRAWL_RETURNS_HOURS",
      m_yesterday = (string) null,
      m_days = "GLUE_TAVERN_BRAWL_RETURNS_DAYS",
      m_weeks = "GLUE_TAVERN_BRAWL_RETURNS_WEEKS",
      m_monthAgo = "GLUE_TAVERN_BRAWL_RETURNS_OVER_1_MONTH"
    };
    string startingTimeText = TimeUtils.GetElapsedTimeString(seasonStartInSeconds, stringSet);
    if (singleLine)
      startingTimeText = startingTimeText.Replace("\n", " ").Replace("\r", "");
    return startingTimeText;
  }

  public DeckRuleset GetCurrentDeckRuleset() => this.GetDeckRuleset(this.m_currentBrawlType);

  public DeckRuleset GetDeckRuleset(BrawlType brawlType, int brawlLibraryItemId = 0)
  {
    TavernBrawlMission mission = this.GetMission(brawlType);
    if (mission == null)
      return (DeckRuleset) null;
    if (brawlLibraryItemId == 0)
      brawlLibraryItemId = mission.SelectedBrawlLibraryItemId;
    return mission.GetDeckRuleset(brawlLibraryItemId) ?? DeckRuleset.GetRuleset(mission.formatType);
  }

  public List<RewardData> CurrentSessionRewards => this.CurrentSession != null && this.CurrentSession.Chest != null ? Network.ConvertRewardChest(this.CurrentSession.Chest).Rewards : new List<RewardData>();

  public void StartGame(long deckId = 0)
  {
    if (this.CurrentMission() == null)
    {
      Error.AddDevFatal("TB: m_currentMission is null");
    }
    else
    {
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_QUEUE);
      GameType gameType = this.CurrentMission().GameType;
      GameMgr.Get().FindGame(gameType, PegasusShared.FormatType.FT_WILD, this.CurrentMission().missionId, deckId: deckId);
    }
  }

  public void StartGameWithHero(int heroCardDbId)
  {
    TavernBrawlMission tavernBrawlMission = this.CurrentMission();
    if (tavernBrawlMission == null)
    {
      Error.AddDevFatal("TB: m_currentMission is null");
    }
    else
    {
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_QUEUE);
      GameMgr.Get().FindGameWithHero(tavernBrawlMission.GameType, PegasusShared.FormatType.FT_WILD, tavernBrawlMission.missionId, tavernBrawlMission.SelectedBrawlLibraryItemId, heroCardDbId);
    }
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    if (!GameMgr.Get().IsNextTavernBrawl() || GameMgr.Get().IsNextSpectator())
      return false;
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        if (PresenceMgr.Get().CurrentStatus == Global.PresenceStatus.TAVERN_BRAWL_QUEUE)
        {
          PresenceMgr.Get().SetPrevStatus();
          break;
        }
        break;
      case FindGameState.SERVER_GAME_CONNECTING:
        if (GameMgr.Get().IsNextTavernBrawl() && GameMgr.Get().IsNextReconnect() && this.IsCurrentSeasonSessionBased)
        {
          BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
          {
            Wins = (uint) this.GamesWon,
            Losses = (uint) this.GamesLost,
            RunFinished = false,
            SessionRecordType = (this.CurrentSeasonBrawlMode == TavernBrawlMode.TB_MODE_NORMAL ? SessionRecordType.TAVERN_BRAWL : SessionRecordType.HEROIC_BRAWL)
          });
          break;
        }
        break;
    }
    return false;
  }

  private void ShowSessionLimitWarning()
  {
    int allowedThisSeason = TavernBrawlManager.Get().NumSessionsAllowedThisSeason;
    int availableForPurchase = TavernBrawlManager.Get().NumSessionsAvailableForPurchase;
    if (allowedThisSeason == 0)
      return;
    string str;
    if (availableForPurchase == 0)
    {
      str = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_LIMIT_ALERT_DESCRIPTION_FINAL");
    }
    else
    {
      if (allowedThisSeason - availableForPurchase <= 1)
        return;
      str = GameStrings.Format("GLUE_HEROIC_BRAWL_SESSION_LIMIT_ALERT_DESCRIPTION_NORMAL", (object) availableForPurchase, (object) availableForPurchase);
    }
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
      m_headerText = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_LIMIT_ALERT_TITLE"),
      m_text = str
    });
  }

  public bool HasCreatedDeck() => this.CurrentDeck() != null;

  public CollectionDeck CurrentDeck() => this.GetDeck(this.m_currentBrawlType);

  public CollectionDeck GetDeck(BrawlType brawlType, int brawlLibraryItemId = 0)
  {
    TavernBrawlMission mission = this.GetMission(brawlType);
    if (mission == null)
      return (CollectionDeck) null;
    if (brawlLibraryItemId == 0)
      brawlLibraryItemId = mission.SelectedBrawlLibraryItemId;
    foreach (CollectionDeck deck in CollectionManager.Get().GetDecks().Values)
    {
      if (TavernBrawlManager.TranslateDeckTypeToBrawlType(deck.Type) == brawlType && mission.seasonId == deck.SeasonId && brawlLibraryItemId == deck.BrawlLibraryItemId)
        return deck;
    }
    return (CollectionDeck) null;
  }

  public bool HasValidDeckForCurrent() => this.HasValidDeck(this.m_currentBrawlType);

  public bool HasValidDeck(BrawlType brawlType, int brawlLibraryItemId = 0)
  {
    TavernBrawlMission mission = this.GetMission(brawlType);
    if (mission == null || !mission.CanCreateDeck(brawlLibraryItemId))
      return false;
    CollectionDeck deck = this.GetDeck(brawlType, brawlLibraryItemId);
    if (deck == null)
      return false;
    if (!deck.NetworkContentsLoaded())
    {
      CollectionManager.Get().RequestDeckContents(deck.ID);
      return false;
    }
    DeckRuleset deckRuleset = this.GetDeckRuleset(brawlType, brawlLibraryItemId);
    return deckRuleset == null || deckRuleset.IsDeckValid(deck);
  }

  public static bool IsBrawlDeckType(DeckType deckType)
  {
    switch (deckType)
    {
      case DeckType.TAVERN_BRAWL_DECK:
      case DeckType.FSG_BRAWL_DECK:
        return true;
      default:
        return false;
    }
  }

  public DeckType DeckTypeForCurrentBrawlType
  {
    get
    {
      switch (this.m_currentBrawlType)
      {
        case BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING:
          return DeckType.FSG_BRAWL_DECK;
        default:
          return DeckType.TAVERN_BRAWL_DECK;
      }
    }
  }

  private static BrawlType TranslateDeckTypeToBrawlType(DeckType deckType)
  {
    if (deckType == DeckType.TAVERN_BRAWL_DECK)
      return BrawlType.BRAWL_TYPE_TAVERN_BRAWL;
    return deckType == DeckType.FSG_BRAWL_DECK ? BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING : BrawlType.BRAWL_TYPE_UNKNOWN;
  }

  public bool IsTavernBrawlActiveByDeckType(DeckType deckType)
  {
    BrawlType brawlType = TavernBrawlManager.TranslateDeckTypeToBrawlType(deckType);
    return brawlType != BrawlType.BRAWL_TYPE_UNKNOWN && this.IsTavernBrawlActive(brawlType);
  }

  public bool IsSeasonActive(DeckType deckType, int seasonId, int brawlLibraryItemId)
  {
    BrawlType brawlType = TavernBrawlManager.TranslateDeckTypeToBrawlType(deckType);
    if (brawlType == BrawlType.BRAWL_TYPE_UNKNOWN || !this.IsSeasonActive(brawlType, seasonId))
      return false;
    if (brawlLibraryItemId != 0)
    {
      TavernBrawlMission mission = this.GetMission(brawlType);
      if (mission == null || !mission.BrawlList.Any<GameContentScenario>((Func<GameContentScenario, bool>) (scen => scen.LibraryItemId == brawlLibraryItemId)))
        return false;
    }
    return true;
  }

  public bool IsSeasonActive(BrawlType brawlType, int seasonId)
  {
    if (!this.IsTavernBrawlActive(brawlType))
      return false;
    TavernBrawlMission mission = this.m_missions[(int) brawlType];
    return mission != null && mission.seasonId == seasonId && (mission.BrawlType != BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING || FiresideGatheringManager.Get().IsCheckedIn);
  }

  public void EnsureAllDataReady(
    TavernBrawlManager.CallbackEnsureServerDataReady callback = null)
  {
    this.EnsureAllDataReady(this.m_currentBrawlType, callback);
  }

  public void EnsureAllDataReady(
    BrawlType brawlType,
    TavernBrawlManager.CallbackEnsureServerDataReady callback = null)
  {
    TavernBrawlMission mission = this.GetMission(brawlType);
    if (mission == null)
      return;
    if (this.IsAllDataReady(brawlType))
    {
      if (callback == null)
        return;
      callback();
    }
    else
    {
      if (callback != null)
      {
        if (this.m_serverDataReadyCallbacks == null)
          this.m_serverDataReadyCallbacks = new List<TavernBrawlManager.CallbackEnsureServerDataReady>();
        this.m_serverDataReadyCallbacks.Add(callback);
      }
      TavernBrawlSeasonSpec tavernBrawlSpec = mission.tavernBrawlSpec;
      List<AssetRecordInfo> assetRecordInfoList1 = new List<AssetRecordInfo>();
      foreach (GameContentScenario brawl in (IEnumerable<GameContentScenario>) mission.BrawlList)
      {
        AssetRecordInfo assetRecordInfo = new AssetRecordInfo()
        {
          Asset = new AssetKey()
        };
        assetRecordInfo.Asset.Type = AssetType.ASSET_TYPE_SCENARIO;
        assetRecordInfo.Asset.AssetId = brawl.ScenarioId;
        assetRecordInfo.RecordByteSize = brawl.ScenarioRecordByteSize;
        assetRecordInfo.RecordHash = brawl.ScenarioRecordHash;
        assetRecordInfoList1.Add(assetRecordInfo);
        if (brawl.AdditionalAssets != null && brawl.AdditionalAssets.Count > 0)
          assetRecordInfoList1.AddRange((IEnumerable<AssetRecordInfo>) brawl.AdditionalAssets);
      }
      if (DownloadableDbfCache.Get().IsAssetRequestInProgress(mission.missionId, AssetType.ASSET_TYPE_SCENARIO))
        DownloadableDbfCache.Get().LoadCachedAssets(false, (DownloadableDbfCache.LoadCachedAssetCallback) ((requestedKey, code, assetBytes) => this.OnDownloadableDbfAssetsLoaded(requestedKey, code, assetBytes, brawlType)), assetRecordInfoList1.ToArray());
      else if (HearthstoneApplication.IsInternal())
        Processor.ScheduleCallback(Mathf.Max(0.0f, UnityEngine.Random.Range(-3f, 3f)), false, (Processor.ScheduledCallback) (userData =>
        {
          TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
          if (tavernBrawlManager.IsAllDataReady(brawlType))
          {
            if (callback == null)
              return;
            if (tavernBrawlManager.m_serverDataReadyCallbacks != null)
              tavernBrawlManager.m_serverDataReadyCallbacks.Remove(callback);
            callback();
          }
          else
          {
            GameContentSeasonSpec gameContentSeason = mission.tavernBrawlSpec.GameContentSeason;
            List<AssetRecordInfo> assetRecordInfoList2 = new List<AssetRecordInfo>();
            foreach (GameContentScenario scenario in gameContentSeason.Scenarios)
            {
              AssetRecordInfo assetRecordInfo = new AssetRecordInfo()
              {
                Asset = new AssetKey()
              };
              assetRecordInfo.Asset.Type = AssetType.ASSET_TYPE_SCENARIO;
              assetRecordInfo.Asset.AssetId = scenario.ScenarioId;
              assetRecordInfo.RecordByteSize = scenario.ScenarioRecordByteSize;
              assetRecordInfo.RecordHash = scenario.ScenarioRecordHash;
              assetRecordInfoList2.Add(assetRecordInfo);
              if (scenario.AdditionalAssets != null && scenario.AdditionalAssets.Count > 0)
                assetRecordInfoList2.AddRange((IEnumerable<AssetRecordInfo>) scenario.AdditionalAssets);
            }
            DownloadableDbfCache.Get().LoadCachedAssets(true, (DownloadableDbfCache.LoadCachedAssetCallback) ((requestedKey, code, assetBytes) => this.OnDownloadableDbfAssetsLoaded(requestedKey, code, assetBytes, brawlType)), assetRecordInfoList2.ToArray());
          }
        }));
      else
        DownloadableDbfCache.Get().LoadCachedAssets(true, (DownloadableDbfCache.LoadCachedAssetCallback) ((requestedKey, code, assetBytes) => this.OnDownloadableDbfAssetsLoaded(requestedKey, code, assetBytes, brawlType)), assetRecordInfoList1.ToArray());
    }
  }

  public bool IsCurrentBrawlInfoReady
  {
    get
    {
      NetCache.NetCacheClientOptions netObject1 = NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>();
      NetCache.NetCacheHeroLevels netObject2 = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
      return netObject1 != null && this.CurrentMission() != null && netObject2 != null;
    }
  }

  public bool IsCurrentBrawlAllDataReady => this.IsAllDataReady(this.m_currentBrawlType);

  private bool IsAllDataReady(BrawlType brawlType)
  {
    if (brawlType < BrawlType.BRAWL_TYPE_TAVERN_BRAWL || brawlType >= BrawlType.BRAWL_TYPE_COUNT)
      return true;
    TavernBrawlMission mission = this.GetMission(brawlType);
    return mission == null || !this.m_downloadableDbfAssetsPendingLoad[(int) brawlType] && !mission.BrawlList.Any<GameContentScenario>((Func<GameContentScenario, bool>) (brawl => GameDbf.Scenario.GetRecord(brawl.ScenarioId) == null));
  }

  public void RefreshServerData(BrawlType brawlType = BrawlType.BRAWL_TYPE_UNKNOWN)
  {
    brawlType = brawlType == BrawlType.BRAWL_TYPE_UNKNOWN ? this.m_currentBrawlType : brawlType;
    Network.Get().RequestTavernBrawlInfo(brawlType);
  }

  public bool HasUnlockedTavernBrawl(BrawlType brawlType)
  {
    NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
    switch (brawlType)
    {
      case BrawlType.BRAWL_TYPE_TAVERN_BRAWL:
        return netObject != null && netObject.Levels.Any<NetCache.HeroLevel>((Func<NetCache.HeroLevel, bool>) (l => l.CurrentLevel.Level >= 20));
      case BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING:
        return netObject != null && netObject.Levels.Any<NetCache.HeroLevel>((Func<NetCache.HeroLevel, bool>) (l => l.CurrentLevel.Level >= 1));
      default:
        return true;
    }
  }

  public bool CanChallengeToTavernBrawl(BrawlType brawlType)
  {
    if (!GameUtils.IsTraditionalTutorialComplete() || !this.IsTavernBrawlActive(brawlType))
      return false;
    TavernBrawlMission mission = this.GetMission(brawlType);
    return !GameUtils.IsAIMission(mission.missionId) && !mission.friendlyChallengeDisabled;
  }

  public bool IsEligibleForFreeTicket()
  {
    if (this.CurrentSession == null || this.CurrentMission() == null)
      return false;
    uint sessionCount = this.CurrentSession.SessionCount;
    uint freeSessions = this.CurrentMission().FreeSessions;
    return freeSessions > 0U && sessionCount < freeSessions;
  }

  public bool IsTavernBrawlActive(BrawlType brawlType)
  {
    TavernBrawlMission mission = this.m_missions[(int) brawlType];
    return (brawlType != BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING || FiresideGatheringManager.Get().IsCheckedIn) && mission != null && this.TavernBrawlSeasonEndInSeconds(brawlType) > 0L;
  }

  public void RefreshPlayerRecord() => Network.Get().RequestTavernBrawlPlayerRecord(this.m_currentBrawlType);

  public long TavernBrawlSeasonStartInSeconds(BrawlType brawlType)
  {
    DateTime? nextSeasonStartDate = this.m_nextSeasonStartDates[(int) brawlType];
    return !nextSeasonStartDate.HasValue || !nextSeasonStartDate.HasValue ? -1L : (long) (nextSeasonStartDate.Value - DateTime.Now).TotalSeconds;
  }

  public float ScheduledSecondsToRefresh(BrawlType brawlType)
  {
    DateTime? scheduledRefreshTime = this.m_scheduledRefreshTimes[(int) brawlType];
    return !scheduledRefreshTime.HasValue || !scheduledRefreshTime.HasValue ? -1f : (float) (scheduledRefreshTime.Value - DateTime.Now).TotalSeconds;
  }

  public long TavernBrawlSeasonNewSessionsClosedInSeconds(BrawlType brawlType)
  {
    TavernBrawlMission mission = this.GetMission(brawlType);
    if (mission != null)
    {
      DateTime? sessionsDateLocal = mission.closedToNewSessionsDateLocal;
      if (sessionsDateLocal.HasValue)
      {
        sessionsDateLocal = mission.closedToNewSessionsDateLocal;
        return (long) (sessionsDateLocal.Value - DateTime.Now).TotalSeconds;
      }
    }
    return (long) int.MaxValue;
  }

  public bool IsSeasonSessionBased(BrawlType brawlType)
  {
    TavernBrawlMission mission = this.GetMission(brawlType);
    return mission != null && mission.IsSessionBased;
  }

  public TavernBrawlMode GetBrawlModeForBrawlType(BrawlType brawlType)
  {
    TavernBrawlMission mission = this.GetMission(brawlType);
    return mission != null ? mission.brawlMode : TavernBrawlMode.TB_MODE_NORMAL;
  }

  public void RequestSessionBegin() => Network.Get().RequestTavernBrawlSessionBegin();

  private TavernBrawlPlayerRecord MyRecord => this.m_playerRecords[(int) this.m_currentBrawlType];

  private void RegisterOptionsListeners(bool register)
  {
    if (register)
    {
      NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheClientOptions), new Action(this.NetCache_OnClientOptions));
      Options.Get().RegisterChangedListener(Option.LATEST_SEEN_TAVERNBRAWL_SEASON, new Options.ChangedCallback(this.OnOptionChangedCallback));
      Options.Get().RegisterChangedListener(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD, new Options.ChangedCallback(this.OnOptionChangedCallback));
      Options.Get().RegisterChangedListener(Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON, new Options.ChangedCallback(this.OnOptionChangedCallback));
      Options.Get().RegisterChangedListener(Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON_CHALKBOARD, new Options.ChangedCallback(this.OnOptionChangedCallback));
    }
    else
    {
      NetCache.Get().RemoveUpdatedListener(typeof (NetCache.NetCacheClientOptions), new Action(this.NetCache_OnClientOptions));
      Options.Get().UnregisterChangedListener(Option.LATEST_SEEN_TAVERNBRAWL_SEASON, new Options.ChangedCallback(this.OnOptionChangedCallback));
      Options.Get().UnregisterChangedListener(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD, new Options.ChangedCallback(this.OnOptionChangedCallback));
      Options.Get().UnregisterChangedListener(Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON, new Options.ChangedCallback(this.OnOptionChangedCallback));
      Options.Get().UnregisterChangedListener(Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON_CHALKBOARD, new Options.ChangedCallback(this.OnOptionChangedCallback));
    }
  }

  private void NetCache_OnClientOptions()
  {
    this.RegisterOptionsListeners(false);
    this.CheckLatestSessionLimit(this.CheckLatestSeenSeason(true));
    this.RegisterOptionsListeners(true);
  }

  private void OnOptionChangedCallback(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    this.RegisterOptionsListeners(false);
    this.CheckLatestSessionLimit(this.CheckLatestSeenSeason(false));
    this.RegisterOptionsListeners(true);
  }

  private bool CheckLatestSeenSeason(bool canSetOption)
  {
    bool flag1 = false;
    if (!this.IsCurrentBrawlInfoReady)
      return flag1;
    int num = !this.m_hasGottenClientOptionsAtLeastOnce ? 1 : 0;
    this.m_hasGottenClientOptionsAtLeastOnce = true;
    bool seeingThisFeature = this.IsFirstTimeSeeingThisFeature;
    bool flag2 = this.CurrentMission() != null && this.LatestSeenTavernBrawlSeason < this.CurrentMission().seasonId;
    this.m_isFirstTimeSeeingThisFeature = false;
    this.m_isFirstTimeSeeingCurrentSeason = false;
    TavernBrawlMission tavernBrawlMission = this.CurrentMission();
    if (tavernBrawlMission != null)
    {
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      bool flag3 = netObject != null && netObject.Games.TavernBrawl && this.HasUnlockedTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
      int tavernBrawlSeason = this.LatestSeenTavernBrawlSeason;
      if (tavernBrawlSeason == 0 & flag3 && tavernBrawlMission.BrawlType != BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
      {
        this.m_isFirstTimeSeeingThisFeature = true;
        NotificationManager.Get().ForceRemoveSoundFromPlayedList("VO_INNKEEPER_TAVERNBRAWL_PUSH_32.prefab:4f57cd2af5fe5194fbc46c91171ab135");
        NotificationManager.Get().ForceRemoveSoundFromPlayedList("VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27.prefab:094070b7fecad8548b0b8fdb02bde052");
      }
      if (tavernBrawlSeason < tavernBrawlMission.seasonId & flag3)
      {
        this.m_isFirstTimeSeeingCurrentSeason = true;
        NotificationManager.Get().ForceRemoveSoundFromPlayedList("VO_INNKEEPER_TAVERNBRAWL_DESC2_30.prefab:498657df8d08bc1468bfd1ad9f74ccac");
        if (canSetOption)
          this.LatestSeenTavernBrawlSeason = tavernBrawlMission.seasonId;
        flag1 = true;
      }
    }
    if ((num != 0 || seeingThisFeature != this.IsFirstTimeSeeingThisFeature || flag2 != this.IsFirstTimeSeeingCurrentSeason) && this.OnTavernBrawlUpdated != null)
      this.OnTavernBrawlUpdated();
    return flag1;
  }

  private void CheckLatestSessionLimit(bool seasonHasChanged)
  {
    if (!this.IsCurrentBrawlInfoReady)
      return;
    TavernBrawlMission tavernBrawlMission = this.CurrentMission();
    if (tavernBrawlMission == null)
      return;
    if (seasonHasChanged)
    {
      Options.Get().SetInt(Option.LATEST_SEEN_TAVERNBRAWL_SESSION_LIMIT, tavernBrawlMission.maxSessions);
    }
    else
    {
      int oldLimit = Options.Get().GetInt(Option.LATEST_SEEN_TAVERNBRAWL_SESSION_LIMIT);
      if (oldLimit == tavernBrawlMission.maxSessions)
        return;
      if (oldLimit == 0)
      {
        Options.Get().SetInt(Option.LATEST_SEEN_TAVERNBRAWL_SESSION_LIMIT, tavernBrawlMission.maxSessions);
      }
      else
      {
        if (tavernBrawlMission.maxSessions > oldLimit && this.OnSessionLimitRaised != null)
          this.OnSessionLimitRaised(oldLimit, tavernBrawlMission.maxSessions);
        Options.Get().SetInt(Option.LATEST_SEEN_TAVERNBRAWL_SESSION_LIMIT, tavernBrawlMission.maxSessions);
      }
    }
  }

  private void ScheduleTimedCallbacksForBrawl(TavernBrawlInfo serverInfo)
  {
    this.m_nextSeasonStartDates[(int) serverInfo.BrawlType] = !serverInfo.HasNextStartSecondsFromNow ? new DateTime?() : new DateTime?(DateTime.Now + new TimeSpan(0, 0, (int) serverInfo.NextStartSecondsFromNow));
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.ScheduledEndOfCurrentTBCallback), (object) serverInfo.BrawlType);
    long secondsToWait1 = this.TavernBrawlSeasonEndInSeconds(serverInfo.BrawlType);
    if (this.IsTavernBrawlActive(serverInfo.BrawlType) && secondsToWait1 > 0L)
    {
      Log.EventTiming.Print("Scheduling end of current {0} {1} secs from now.", (object) serverInfo.BrawlType, (object) secondsToWait1);
      Processor.ScheduleCallback((float) secondsToWait1, true, new Processor.ScheduledCallback(this.ScheduledEndOfCurrentTBCallback), (object) serverInfo.BrawlType);
    }
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.ScheduledRefreshTBSpecCallback), (object) serverInfo.BrawlType);
    long num = this.TavernBrawlSeasonStartInSeconds(serverInfo.BrawlType);
    if (num >= 0L)
    {
      this.m_scheduledRefreshTimes[(int) serverInfo.BrawlType] = new DateTime?(DateTime.Now + new TimeSpan(0, 0, 0, (int) num, 0));
      Log.EventTiming.Print("Scheduling {0} refresh for {1} secs from now.", (object) serverInfo.BrawlType, (object) num);
      Processor.ScheduleCallback((float) num, true, new Processor.ScheduledCallback(this.ScheduledRefreshTBSpecCallback), (object) serverInfo.BrawlType);
    }
    long secondsToWait2 = this.TavernBrawlSeasonNewSessionsClosedInSeconds(serverInfo.BrawlType);
    if (!this.IsSeasonSessionBased(serverInfo.BrawlType) || secondsToWait2 <= 0L)
      return;
    Log.EventTiming.Print("Scheduling {0} Closed Update for {1} secs from now.", (object) serverInfo.BrawlType, (object) secondsToWait2);
    Processor.ScheduleCallback((float) secondsToWait2, true, new Processor.ScheduledCallback(this.ScheduleTBClosedUpdateCallback), (object) serverInfo.BrawlType);
  }

  private void OnCheckInToFSGResponse()
  {
    CheckInToFSGResponse checkInToFsgResponse = Network.Get().GetCheckInToFSGResponse();
    if (checkInToFsgResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
      return;
    if (checkInToFsgResponse.HasPlayerRecord)
      this.m_playerRecords[2] = checkInToFsgResponse.PlayerRecord;
    if (this.m_currentBrawlType != BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING || this.OnTavernBrawlUpdated == null)
      return;
    this.OnTavernBrawlUpdated();
  }

  private void ScheduledEndOfCurrentTBCallback(object userData)
  {
    Log.EventTiming.Print("ScheduledEndOfCurrentTBCallback: ending current TB now.");
    bool flag = (UnityEngine.Object) TavernBrawlDisplay.Get() != (UnityEngine.Object) null && TavernBrawlDisplay.Get().IsInRewards();
    BrawlType userData1 = (BrawlType) userData;
    TavernBrawlMission mission = this.m_missions[(int) this.m_currentBrawlType];
    TavernBrawlPlayerRecord playerRecord = this.m_playerRecords[(int) this.m_currentBrawlType];
    if (mission != null && mission.IsSessionBased && (playerRecord.SessionStatus == TavernBrawlStatus.TB_STATUS_ACTIVE || playerRecord.SessionStatus == TavernBrawlStatus.TB_STATUS_IN_REWARDS) && (userData1 != this.m_currentBrawlType || !flag))
    {
      int num = 2;
      Processor.ScheduleCallback(mission.SeasonEndSecondsSpreadCount <= 0 ? (float) (num + UnityEngine.Random.Range(0, 30)) : (float) (num + mission.SeasonEndSecondsSpreadCount), true, new Processor.ScheduledCallback(this.ScheduledEndOfCurrentTBCallback_AfterSpreadWhenRewardsExpected), (object) userData1);
    }
    if (userData1 != this.m_currentBrawlType)
      return;
    this.m_missions[(int) userData1] = (TavernBrawlMission) null;
    if (GameMgr.Get().IsFindingGame())
      GameMgr.Get().CancelFindGame();
    if (this.OnTavernBrawlUpdated == null)
      return;
    this.OnTavernBrawlUpdated();
  }

  private void ScheduledEndOfCurrentTBCallback_AfterSpreadWhenRewardsExpected(object userData)
  {
    BrawlType brawlType = (BrawlType) userData;
    Network.Get().RequestTavernBrawlPlayerRecord(brawlType);
  }

  private void ScheduledRefreshTBSpecCallback(object userData)
  {
    BrawlType brawlType = (BrawlType) userData;
    Log.EventTiming.Print("ScheduledRefreshTBSpecCallback: refreshing now.");
    this.RefreshServerData(brawlType);
  }

  private void ScheduleTBClosedUpdateCallback(object userData)
  {
    int num = (int) userData;
    Log.EventTiming.Print("ScheduledUpdateTBCallback: updating now.");
    int currentBrawlType = (int) this.m_currentBrawlType;
    if (num != currentBrawlType || this.OnTavernBrawlUpdated == null)
      return;
    this.OnTavernBrawlUpdated();
  }

  private void OnDownloadableDbfAssetsLoaded(
    AssetKey requestedKey,
    PegasusShared.ErrorCode code,
    byte[] assetBytes,
    BrawlType brawlType)
  {
    if (requestedKey == null || requestedKey.Type != AssetType.ASSET_TYPE_SCENARIO)
      Log.TavernBrawl.Print("OnDownloadableDbfAssetsLoaded bad AssetType assetId={0} assetType={1} {2}", (object) (requestedKey == null ? 0 : requestedKey.AssetId), (object) (requestedKey == null ? 0 : (int) requestedKey.Type), requestedKey == null ? (object) "(null)" : (object) requestedKey.Type.ToString());
    else if (assetBytes == null || assetBytes.Length == 0)
    {
      Log.TavernBrawl.PrintError("OnDownloadableDbfAssetsLoaded failed to load Asset: assetId={0} assetType={1} {2} error={3}", (object) (requestedKey == null ? 0 : requestedKey.AssetId), (object) (requestedKey == null ? 0 : (int) requestedKey.Type), requestedKey == null ? (object) "(null)" : (object) requestedKey.Type.ToString(), (object) code);
    }
    else
    {
      TavernBrawlMission mission = this.m_missions[(int) brawlType];
      if (mission == null)
        return;
      ScenarioDbRecord from = ProtobufUtil.ParseFrom<ScenarioDbRecord>(assetBytes, length: assetBytes.Length);
      if (mission.BrawlList.Count == 0 || mission.BrawlList.First<GameContentScenario>().ScenarioId != from.Id)
        return;
      this.m_downloadableDbfAssetsPendingLoad[(int) brawlType] = false;
      if (this.m_currentBrawlType != brawlType)
        return;
      Processor.RunCoroutine(this.OnDownloadableDbfAssetsLoaded_EnsureCurrentBrawlDeckContentsLoaded());
    }
  }

  private IEnumerator OnDownloadableDbfAssetsLoaded_EnsureCurrentBrawlDeckContentsLoaded()
  {
    foreach (CollectionDeck collectionDeck in CollectionManager.Get().GetDecks().Values)
    {
      if (TavernBrawlManager.TranslateDeckTypeToBrawlType(collectionDeck.Type) == this.m_currentBrawlType && !collectionDeck.NetworkContentsLoaded())
        CollectionManager.Get().RequestDeckContents(collectionDeck.ID);
    }
    if (this.CurrentMission() != null && !this.CurrentBrawlDeckContentsLoaded)
    {
      float timeAtStart = Time.realtimeSinceStartup;
      bool done = false;
      while (!done)
      {
        yield return (object) null;
        if ((double) Time.realtimeSinceStartup - (double) timeAtStart > 30.0)
          done = true;
        else if (!this.IsCurrentBrawlAllDataReady)
          done = true;
        else if (this.CurrentMission() == null)
          done = true;
        else if (this.CurrentBrawlDeckContentsLoaded)
          done = true;
      }
    }
    if (this.IsCurrentBrawlAllDataReady)
    {
      if (this.OnTavernBrawlUpdated != null)
        this.OnTavernBrawlUpdated();
      if (this.m_serverDataReadyCallbacks != null)
      {
        TavernBrawlManager.CallbackEnsureServerDataReady[] array = this.m_serverDataReadyCallbacks.ToArray();
        this.m_serverDataReadyCallbacks.Clear();
        for (int index = 0; index < array.Length; ++index)
          array[index]();
      }
    }
  }

  private bool CurrentBrawlDeckContentsLoaded
  {
    get
    {
      TavernBrawlMission tavernBrawlMission = this.CurrentMission();
      if (tavernBrawlMission == null)
        return true;
      BrawlType currentBrawlType = this.m_currentBrawlType;
      int seasonId = tavernBrawlMission.seasonId;
      foreach (CollectionDeck collectionDeck in CollectionManager.Get().GetDecks().Values)
      {
        if (TavernBrawlManager.TranslateDeckTypeToBrawlType(collectionDeck.Type) == currentBrawlType && collectionDeck.SeasonId == seasonId && !collectionDeck.NetworkContentsLoaded())
          return false;
      }
      return true;
    }
  }

  private long TavernBrawlSeasonEndInSeconds(BrawlType brawlType)
  {
    TavernBrawlMission mission = this.m_missions[(int) brawlType];
    if (mission == null)
      return -1;
    return !mission.endDateLocal.HasValue ? (long) int.MaxValue : (long) (mission.endDateLocal.Value - DateTime.Now).TotalSeconds;
  }

  private void OnTavernBrawlRecord_Internal(TavernBrawlPlayerRecord record)
  {
    if (record == null)
      return;
    this.m_playerRecords[(int) record.BrawlType] = record;
    if (this.m_currentBrawlType != record.BrawlType || this.OnTavernBrawlUpdated == null)
      return;
    this.OnTavernBrawlUpdated();
  }

  private void OnTavernBrawlInfo_Internal(TavernBrawlInfo serverInfo)
  {
    if (serverInfo == null)
      return;
    int brawlType = (int) serverInfo.BrawlType;
    if (brawlType < 0 || brawlType >= this.m_missions.Length)
    {
      Log.TavernBrawl.PrintError("OnTavernBrawlInfo_Internal: received invalid index for BrawlType={0} arrayLength={1}", (object) brawlType, (object) this.m_missions.Length);
    }
    else
    {
      if (!serverInfo.HasCurrentTavernBrawl)
      {
        this.m_missions[brawlType] = (TavernBrawlMission) null;
      }
      else
      {
        if (this.m_missions[brawlType] == null)
          this.m_missions[brawlType] = new TavernBrawlMission();
        this.m_missions[brawlType].SetSeasonSpec(serverInfo.CurrentTavernBrawl, serverInfo.BrawlType);
        this.m_downloadableDbfAssetsPendingLoad[brawlType] = true;
        if (this.OnTavernBrawlUpdated != null)
          this.EnsureAllDataReady(serverInfo.BrawlType);
      }
      this.CheckLatestSessionLimit(this.CheckLatestSeenSeason(true));
      this.ScheduleTimedCallbacksForBrawl(serverInfo);
      if (serverInfo.HasMyRecord)
        this.OnTavernBrawlRecord_Internal(serverInfo.MyRecord);
      if (this.m_currentBrawlType != serverInfo.BrawlType || this.OnTavernBrawlUpdated == null)
        return;
      this.OnTavernBrawlUpdated();
    }
  }

  private void OnBeginSession()
  {
    Log.TavernBrawl.Print(string.Format("TavernBrawlManager.OnBeginSession"));
    TavernBrawlRequestSessionBeginResponse brawlSessionBegin = Network.Get().GetTavernBrawlSessionBegin();
    if (brawlSessionBegin.HasErrorCode && brawlSessionBegin.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      string str = brawlSessionBegin.ErrorCode.ToString();
      Debug.LogWarning((object) ("TavernBrawlManager.OnBeginSession: Got Error " + (object) brawlSessionBegin.ErrorCode + " : " + str));
      if (!SceneMgr.Get().IsSceneLoaded() || (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.TAVERN_BRAWL) || SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FIRESIDE_GATHERING)) && TavernBrawlManager.Get().PlayerStatus == TavernBrawlStatus.TB_STATUS_ACTIVE)
        return;
      if ((UnityEngine.Object) TavernBrawlStore.Get() != (UnityEngine.Object) null)
        TavernBrawlStore.Get().Hide();
      if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
      if (this.CurrentMission().brawlMode == TavernBrawlMode.TB_MODE_HEROIC)
      {
        info.m_headerText = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_ERROR_TITLE");
        info.m_text = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_ERROR");
      }
      else
      {
        info.m_headerText = GameStrings.Get("GLUE_BRAWLISEUM_SESSION_ERROR_TITLE");
        info.m_text = GameStrings.Get("GLUE_BRAWLISEUM_SESSION_ERROR");
      }
      info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
      DialogManager.Get().ShowPopup(info);
    }
    else
    {
      BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
      {
        Wins = 0U,
        Losses = 0U,
        RunFinished = false,
        SessionRecordType = SessionRecordType.TAVERN_BRAWL
      });
      if (brawlSessionBegin.HasPlayerRecord)
        this.OnTavernBrawlRecord_Internal(brawlSessionBegin.PlayerRecord);
      this.ShowSessionLimitWarning();
      if (this.OnTavernBrawlUpdated == null)
        return;
      this.OnTavernBrawlUpdated();
    }
  }

  private void OnRetireSession()
  {
    Log.TavernBrawl.Print(string.Format("TavernBrawlManager.OnRetireSession"));
    CollectionManager.Get()?.DoneEditing();
    TavernBrawlRequestSessionRetireResponse brawlSessionRetired = Network.Get().GetTavernBrawlSessionRetired();
    if (brawlSessionRetired.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      string str = brawlSessionRetired.ErrorCode.ToString();
      Debug.LogWarning((object) ("TavernBrawlManager.OnRetireSession: Got Error " + (object) brawlSessionRetired.ErrorCode + " : " + str));
    }
    else
    {
      if (brawlSessionRetired.HasPlayerRecord)
        this.OnTavernBrawlRecord_Internal(brawlSessionRetired.PlayerRecord);
      this.MyRecord.SessionStatus = TavernBrawlStatus.TB_STATUS_IN_REWARDS;
      this.CurrentSession.Chest = brawlSessionRetired.Chest;
      if (this.OnTavernBrawlUpdated == null)
        return;
      this.OnTavernBrawlUpdated();
    }
  }

  private void OnAckRewards()
  {
    Log.TavernBrawl.Print(string.Format("TavernBrawlManager.OnAckRewards"));
    BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
    {
      Wins = (uint) this.GamesWon,
      Losses = (uint) this.GamesLost,
      RunFinished = true,
      SessionRecordType = (this.CurrentSeasonBrawlMode == TavernBrawlMode.TB_MODE_NORMAL ? SessionRecordType.TAVERN_BRAWL : SessionRecordType.HEROIC_BRAWL)
    });
    Network.Get().RequestTavernBrawlPlayerRecord(this.m_currentBrawlType);
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
      return;
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
  }

  private void OnTavernBrawlRecord() => this.OnTavernBrawlRecord_Internal(Network.Get().GetTavernBrawlRecord());

  private void OnTavernBrawlInfo() => this.OnTavernBrawlInfo_Internal(Network.Get().GetTavernBrawlInfo());

  public bool IsCheated { get; private set; }

  public void Cheat_SetScenario(int scenarioId, BrawlType brawlType = BrawlType.BRAWL_TYPE_TAVERN_BRAWL)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.IsCheated = true;
    int index = (int) brawlType;
    if (this.m_missions[index] == null)
      this.m_missions[index] = new TavernBrawlMission();
    this.m_missions[index].SetSeasonSpec(new TavernBrawlSeasonSpec(), brawlType);
    this.m_missions[index].tavernBrawlSpec.GameContentSeason.Scenarios.Add(new GameContentScenario());
    this.m_missions[index].tavernBrawlSpec.GameContentSeason.Scenarios[0].ScenarioId = scenarioId;
    this.m_downloadableDbfAssetsPendingLoad[(int) brawlType] = true;
    if (this.OnTavernBrawlUpdated != null)
      this.OnTavernBrawlUpdated();
    AssetRecordInfo assetRecordInfo = new AssetRecordInfo()
    {
      Asset = new AssetKey()
    };
    assetRecordInfo.Asset.Type = AssetType.ASSET_TYPE_SCENARIO;
    assetRecordInfo.Asset.AssetId = scenarioId;
    assetRecordInfo.RecordByteSize = 0U;
    assetRecordInfo.RecordHash = (byte[]) null;
    DownloadableDbfCache.Get().LoadCachedAssets(true, (DownloadableDbfCache.LoadCachedAssetCallback) ((requestedKey, code, assetBytes) => this.OnDownloadableDbfAssetsLoaded(requestedKey, code, assetBytes, brawlType)), assetRecordInfo);
  }

  public void Cheat_ResetToServerData()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.IsCheated = false;
    this.OnTavernBrawlInfo();
    if (this.CurrentMission() == null)
      return;
    AssetRecordInfo assetRecordInfo = new AssetRecordInfo()
    {
      Asset = new AssetKey()
    };
    assetRecordInfo.Asset.Type = AssetType.ASSET_TYPE_SCENARIO;
    assetRecordInfo.Asset.AssetId = this.CurrentMission().missionId;
    assetRecordInfo.RecordByteSize = 0U;
    assetRecordInfo.RecordHash = (byte[]) null;
    DownloadableDbfCache.Get().LoadCachedAssets(true, (DownloadableDbfCache.LoadCachedAssetCallback) ((requestedKey, code, assetBytes) => this.OnDownloadableDbfAssetsLoaded(requestedKey, code, assetBytes, BrawlType.BRAWL_TYPE_TAVERN_BRAWL)), assetRecordInfo);
  }

  public void Cheat_ResetSeenStuff(int newValue)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.RegisterOptionsListeners(false);
    this.LatestSeenTavernBrawlChalkboard = newValue;
    this.LatestSeenTavernBrawlSeason = newValue;
    Options.Get().SetInt(Option.TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE, 0);
    this.CheckLatestSessionLimit(this.CheckLatestSeenSeason(false));
    this.RegisterOptionsListeners(true);
  }

  public void Cheat_SetWins(int numWins)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.CurrentSession.Wins = numWins;
    if (this.OnTavernBrawlUpdated == null)
      return;
    this.OnTavernBrawlUpdated();
  }

  public void Cheat_SetLosses(int numLosses)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.CurrentSession.Losses = numLosses;
    if (this.OnTavernBrawlUpdated == null)
      return;
    this.OnTavernBrawlUpdated();
  }

  public void Cheat_SetActiveSession(int status)
  {
    this.MyRecord.SessionStatus = (TavernBrawlStatus) status;
    this.MyRecord.Session = new TavernBrawlPlayerSession();
  }

  public void Cheat_DoHeroicRewards(int wins, TavernBrawlMode mode)
  {
    this.MyRecord.SessionStatus = TavernBrawlStatus.TB_STATUS_IN_REWARDS;
    this.CurrentSession.Chest = RewardUtils.GenerateTavernBrawlRewardChest_CHEAT(wins, mode);
    this.CurrentSession.Wins = wins;
    if (this.OnTavernBrawlUpdated == null)
      return;
    this.OnTavernBrawlUpdated();
  }

  public delegate void CallbackEnsureServerDataReady();

  public delegate void TavernBrawlSessionLimitRaisedCallback(int oldLimit, int newLimit);
}
