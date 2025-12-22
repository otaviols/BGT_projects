using Assets;
using System.Collections.Generic;

public abstract class DeckRule
{
  protected int m_id;
  protected int m_deckRulesetId;
  protected int m_appliesToSubsetId;
  protected HashSet<string> m_appliesToSubset;
  protected bool m_appliesToIsNot;
  protected DeckRule.RuleType m_ruleType;
  protected bool m_ruleIsNot;
  protected int m_minValue;
  protected int m_maxValue;
  protected int m_tag;
  protected int m_tagMinValue;
  protected int m_tagMaxValue;
  protected string m_stringValue;
  protected string m_errorString;
  protected bool m_showInvalidCards;
  protected List<HashSet<string>> m_subsets;

  public DeckRule(DeckRule.RuleType ruleType, DeckRulesetRuleDbfRecord record)
  {
    this.m_ruleType = ruleType;
    this.m_id = record.ID;
    this.m_deckRulesetId = record.DeckRulesetId;
    this.m_appliesToSubsetId = record.AppliesToSubsetId;
    this.m_appliesToIsNot = record.AppliesToIsNot;
    this.m_ruleIsNot = record.RuleIsNot;
    this.m_minValue = record.MinValue;
    this.m_maxValue = record.MaxValue;
    this.m_tag = record.Tag;
    this.m_tagMinValue = record.TagMinValue;
    this.m_tagMaxValue = record.TagMaxValue;
    this.m_stringValue = record.StringValue;
    this.m_errorString = record.ErrorString != null ? record.ErrorString.GetString() : "";
    this.m_showInvalidCards = record.ShowInvalidCards;
    this.m_subsets = new List<HashSet<string>>();
    if (this.m_appliesToSubsetId != 0)
      this.m_appliesToSubset = GameDbf.GetIndex().GetSubsetById(this.m_appliesToSubsetId);
    this.m_subsets = GameDbf.GetIndex().GetSubsetsForRule(this.m_id);
  }

  public int GetID() => this.m_id;

  public static DeckRule CreateFromDBF(DeckRulesetRuleDbfRecord record) => DeckRule.GetRule(record);

  public static DeckRule GetRule(DeckRulesetRuleDbfRecord record)
  {
    switch (record.RuleType)
    {
      case DeckRulesetRule.RuleType.HAS_TAG_VALUE:
        return (DeckRule) new DeckRule_HasTagValue(record);
      case DeckRulesetRule.RuleType.COUNT_CARDS_IN_DECK:
        return (DeckRule) new DeckRule_CountCardsInDeck(record);
      case DeckRulesetRule.RuleType.COUNT_COPIES_OF_EACH_CARD:
        return (DeckRule) new DeckRule_CountCopiesOfEachCard(record);
      case DeckRulesetRule.RuleType.IS_IN_ANY_SUBSET:
        return (DeckRule) new DeckRule_IsInAnySubset(record);
      case DeckRulesetRule.RuleType.IS_IN_ALL_SUBSETS:
        return (DeckRule) new DeckRule_IsInAllSubsets(record);
      case DeckRulesetRule.RuleType.PLAYER_OWNS_EACH_COPY:
        return (DeckRule) new DeckRule_PlayerOwnsEachCopy(record);
      case DeckRulesetRule.RuleType.IS_NOT_ROTATED:
        return (DeckRule) new DeckRule_IsNotRotated(record);
      case DeckRulesetRule.RuleType.DECK_SIZE:
        return (DeckRule) new DeckRule_DeckSize(record);
      case DeckRulesetRule.RuleType.IS_CLASS_OR_NEUTRAL_CARD:
        return (DeckRule) new DeckRule_IsClassCardOrNeutral(record);
      case DeckRulesetRule.RuleType.IS_CARD_PLAYABLE:
        return (DeckRule) new DeckRule_IsCardPlayable(record);
      case DeckRulesetRule.RuleType.IS_NOT_BANNED_IN_LEAGUE:
        return (DeckRule) new DeckRule_IsNotBannedInLeague(record);
      case DeckRulesetRule.RuleType.IS_IN_CARDSET:
        return (DeckRule) new DeckRule_IsInCardset(record);
      case DeckRulesetRule.RuleType.IS_IN_FORMAT:
        return (DeckRule) new DeckRule_IsInFormat(record);
      case DeckRulesetRule.RuleType.EDITING_DECK_EXTRA_CARD_COUNT:
        return (DeckRule) new DeckRule_EditingDeckExtraCardCount(record);
      case DeckRulesetRule.RuleType.DEATHKNIGHT_RUNE_LIMIT:
        return (DeckRule) new DeckRule_DeathKnightRuneLimit(record);
      default:
        return (DeckRule) new DeckRule_DefaultType(record.RuleType.ToString(), record);
    }
  }

  public DeckRule.RuleType Type => this.m_ruleType;

  public bool RuleIsNot => this.m_ruleIsNot;

  public bool ShowInvalidCards => this.m_showInvalidCards;

  public virtual bool Filter(EntityDef def, CollectionDeck deck) => true;

  public virtual bool CanAddToDeck(
    EntityDef def,
    TAG_PREMIUM premium,
    CollectionDeck deck,
    out RuleInvalidReason reason)
  {
    return this.DefaultYes(out reason);
  }

  public abstract bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason);

  public override string ToString() => string.Format("{0}, id:{1}, deckruleset:{2}", (object) this.m_ruleType, (object) this.m_id, (object) this.m_deckRulesetId);

  protected bool GetResult(bool val) => val == !this.m_ruleIsNot;

  protected bool AppliesTo(string cardId) => this.m_appliesToSubset == null || this.m_appliesToSubset.Contains(cardId) == !this.m_appliesToIsNot;

  protected bool DefaultYes(out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    return true;
  }

  public enum RuleType
  {
    IS_IN_ANY_SUBSET,
    IS_IN_ALL_SUBSETS,
    IS_NOT_ROTATED,
    COUNT_COPIES_OF_EACH_CARD,
    PLAYER_OWNS_EACH_COPY,
    IS_CLASS_CARD_OR_NEUTRAL,
    COUNT_CARDS_IN_DECK,
    HAS_TAG_VALUE,
    DECK_SIZE,
    IS_CARD_PLAYABLE,
    IS_NOT_BANNED_IN_LEAGUE,
    IS_IN_CARDSET,
    IS_IN_FORMAT,
    EDITING_DECK_EXTRA_CARD_COUNT,
    DEATHKNIGHT_RUNE_LIMIT,
    UNKNOWN,
  }
}
