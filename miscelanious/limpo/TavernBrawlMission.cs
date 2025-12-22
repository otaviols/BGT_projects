using Assets;
using Blizzard.T5.Core;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;

public class TavernBrawlMission
{
  private CharacterDialogSequence m_firstTimeSeenCharacterDialogSequence;
  private int m_selectedBrawlIndex = -1;
  private Map<int, DeckRuleset> m_cachedSelectedDeckRuleset = new Map<int, DeckRuleset>();

  public TavernBrawlSeasonSpec tavernBrawlSpec { get; private set; }

  public int seasonId => this.tavernBrawlSpec.GameContentSeason.SeasonId;

  public GameContentScenario SelectedBrawl
  {
    get
    {
      if (this.tavernBrawlSpec != null && this.m_selectedBrawlIndex >= 0 && this.m_selectedBrawlIndex < this.tavernBrawlSpec.GameContentSeason.Scenarios.Count)
        return this.tavernBrawlSpec.GameContentSeason.Scenarios[this.m_selectedBrawlIndex];
      return this.tavernBrawlSpec != null && this.tavernBrawlSpec.GameContentSeason.Scenarios.Count == 1 ? this.tavernBrawlSpec.GameContentSeason.Scenarios[0] : (GameContentScenario) null;
    }
  }

  public IList<GameContentScenario> BrawlList => this.tavernBrawlSpec != null ? (IList<GameContentScenario>) this.tavernBrawlSpec.GameContentSeason.Scenarios : (IList<GameContentScenario>) new List<GameContentScenario>();

  public int missionId
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.ScenarioId : 0;
    }
  }

  public int SelectedBrawlLibraryItemId
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.LibraryItemId : 0;
    }
  }

  public TavernBrawlMode brawlMode
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.BrawlMode : TavernBrawlMode.TB_MODE_NORMAL;
    }
  }

  public PegasusShared.FormatType formatType
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.FormatType : PegasusShared.FormatType.FT_UNKNOWN;
    }
  }

  public DateTime? endDateLocal => !this.tavernBrawlSpec.GameContentSeason.HasEndSecondsFromNow ? new DateTime?() : new DateTime?(DateTime.Now + new TimeSpan(0, 0, (int) this.tavernBrawlSpec.GameContentSeason.EndSecondsFromNow));

  public DateTime? closedToNewSessionsDateLocal
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl == null || !selectedBrawl.HasClosedToNewSessionsSecondsFromNow ? new DateTime?() : new DateTime?(DateTime.Now + new TimeSpan(0, 0, (int) selectedBrawl.ClosedToNewSessionsSecondsFromNow));
    }
  }

  public bool canCreateDeck => this.CanCreateDeck(this.SelectedBrawlLibraryItemId);

  public bool CanCreateDeck(int brawlLibraryItemId)
  {
    if (brawlLibraryItemId == 0)
      brawlLibraryItemId = this.SelectedBrawlLibraryItemId;
    int scenarioId = this.GetScenarioId(brawlLibraryItemId);
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(scenarioId);
    return record != null && record.RuleType == Scenario.RuleType.CHOOSE_DECK;
  }

  public bool canEditDeck
  {
    get
    {
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.missionId);
      return record != null && record.RuleType == Scenario.RuleType.CHOOSE_DECK;
    }
  }

  public bool canSelectHeroForDeck
  {
    get
    {
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.missionId);
      if (record != null)
      {
        switch (record.RuleType)
        {
          case Scenario.RuleType.CHOOSE_HERO:
          case Scenario.RuleType.CHOOSE_DECK:
            return true;
        }
      }
      return false;
    }
  }

  public DeckRuleset GetDeckRuleset(int brawlLibraryItemId)
  {
    DeckRuleset deckRuleset = (DeckRuleset) null;
    if (!this.m_cachedSelectedDeckRuleset.TryGetValue(brawlLibraryItemId, out deckRuleset))
    {
      int scenarioId = this.GetScenarioId(brawlLibraryItemId);
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(scenarioId);
      if (record != null)
      {
        deckRuleset = DeckRuleset.GetDeckRuleset(record.DeckRulesetId);
        this.m_cachedSelectedDeckRuleset[brawlLibraryItemId] = deckRuleset;
      }
    }
    return deckRuleset;
  }

  public int ticketType
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.TicketType : 0;
    }
  }

  public PegasusShared.RewardType rewardType
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.RewardType : PegasusShared.RewardType.REWARD_UNKNOWN;
    }
  }

  public RewardTrigger rewardTrigger
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.RewardTrigger : RewardTrigger.REWARD_TRIGGER_UNKNOWN;
    }
  }

  public long RewardData1
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.RewardData1 : 0L;
    }
  }

  public long RewardData2
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.RewardData2 : 0L;
    }
  }

  public int RewardTriggerQuota
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.RewardTriggerQuota : 0;
    }
  }

  public int maxWins
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.MaxWins : 0;
    }
  }

  public int maxLosses
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.MaxLosses : 0;
    }
  }

  public int maxSessions
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.MaxSessions : 0;
    }
  }

  public int SeasonEndSecondsSpreadCount => this.tavernBrawlSpec.GameContentSeason.SeasonEndSecondSpreadCount;

  public bool friendlyChallengeDisabled
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null && selectedBrawl.FriendlyChallengeDisabled;
    }
  }

  public uint FreeSessions
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl == null || !selectedBrawl.HasFreeSessions ? 0U : selectedBrawl.FreeSessions;
    }
  }

  public bool IsPrerelease => this.BrawlList.Any<GameContentScenario>((Func<GameContentScenario, bool>) (s => s.HasIsPrerelease && s.IsPrerelease));

  public bool IsSessionBased => this.maxWins > 0 || this.maxLosses > 0;

  public BrawlType BrawlType { get; private set; }

  public int FirstTimeSeenCharacterDialogID
  {
    get
    {
      GameContentScenario selectedBrawl = this.SelectedBrawl;
      return selectedBrawl != null ? selectedBrawl.FirstTimeSeenDialogId : 0;
    }
  }

  public bool IsDungeonRun
  {
    get
    {
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.missionId);
      if (record == null)
        return false;
      AdventureModeDbId modeId = (AdventureModeDbId) record.ModeId;
      return modeId == AdventureModeDbId.DUNGEON_CRAWL || modeId == AdventureModeDbId.DUNGEON_CRAWL_HEROIC;
    }
  }

  public CharacterDialogSequence FirstTimeSeenCharacterDialogSequence
  {
    get
    {
      if (this.FirstTimeSeenCharacterDialogID < 1)
        return (CharacterDialogSequence) null;
      if (this.m_firstTimeSeenCharacterDialogSequence == null)
        this.m_firstTimeSeenCharacterDialogSequence = new CharacterDialogSequence(this.FirstTimeSeenCharacterDialogID);
      return this.m_firstTimeSeenCharacterDialogSequence;
    }
  }

  public void SetSeasonSpec(TavernBrawlSeasonSpec spec, BrawlType brawlType)
  {
    this.tavernBrawlSpec = spec != null ? spec : throw new ArgumentNullException("TavernBrawlMissions must have a spec provided");
    this.BrawlType = brawlType;
    this.m_selectedBrawlIndex = -1;
    this.m_firstTimeSeenCharacterDialogSequence = (CharacterDialogSequence) null;
    this.m_cachedSelectedDeckRuleset.Clear();
    spec.GameContentSeason.Scenarios.Sort((Comparison<GameContentScenario>) ((a, b) =>
    {
      if (a.IsRequired != b.IsRequired)
        return !a.IsRequired ? 1 : -1;
      if (a.IsFallback == b.IsFallback)
        return b.ScenarioId - a.ScenarioId;
      return !a.IsFallback ? 1 : -1;
    }));
  }

  public GameType GameType => TavernBrawlMission.GetGameType(this.BrawlType, this.missionId);

  private static GameType GetGameType(
    BrawlType brawlType,
    int scenarioId,
    bool isFriendlyChallenge = false)
  {
    if (brawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
    {
      if (isFriendlyChallenge)
        return GameType.GT_FSG_BRAWL_VS_FRIEND;
      GameType gameType = GameType.GT_FSG_BRAWL;
      if (GameUtils.IsAIMission(scenarioId))
        gameType = GameType.GT_FSG_BRAWL_1P_VS_AI;
      else if (GameUtils.IsCoopMission(scenarioId))
        gameType = GameType.GT_FSG_BRAWL_2P_COOP;
      return gameType;
    }
    return !isFriendlyChallenge ? GameType.GT_TAVERNBRAWL : GameType.GT_VS_FRIEND;
  }

  public void SetSelectedBrawlLibraryItemId(int brawlLibraryItemId)
  {
    this.m_selectedBrawlIndex = -1;
    IList<GameContentScenario> brawlList = this.BrawlList;
    for (int index = 0; index < brawlList.Count; ++index)
    {
      if (brawlList[index].LibraryItemId == brawlLibraryItemId)
      {
        this.m_selectedBrawlIndex = index;
        break;
      }
    }
  }

  public int GetScenarioId(int brawlLibraryItemId)
  {
    if (brawlLibraryItemId == 0)
      brawlLibraryItemId = this.SelectedBrawlLibraryItemId;
    GameContentScenario gameContentScenario = this.BrawlList.FirstOrDefault<GameContentScenario>((Func<GameContentScenario, bool>) (s => s.LibraryItemId == brawlLibraryItemId));
    return gameContentScenario != null ? gameContentScenario.ScenarioId : 0;
  }
}
