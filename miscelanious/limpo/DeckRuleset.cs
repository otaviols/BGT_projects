using Blizzard.T5.Core;
using Hearthstone.DataModels;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckRuleset
{
  private int m_id;
  private List<DeckRule> m_rules;
  private static Map<FormatType, DeckRuleset> s_FormatRulesets = new Map<FormatType, DeckRuleset>();
  private static DeckRuleset s_PVPDRRuleset;
  private static DeckRuleset s_PVPDRDisplayRuleset;

  public List<DeckRule> Rules => this.m_rules;

  public static DeckRuleset GetDeckRuleset(int id)
  {
    FormatType formatType;
    return !new Map<int, FormatType>()
    {
      {
        1,
        FormatType.FT_WILD
      },
      {
        2,
        FormatType.FT_STANDARD
      },
      {
        482,
        FormatType.FT_CLASSIC
      }
    }.TryGetValue(id, out formatType) ? DeckRuleset.GetDeckRulesetFromDBF(id) : DeckRuleset.GetRuleset(formatType);
  }

  private static DeckRuleset GetDeckRulesetFromDBF(int id)
  {
    if (id <= 0)
      return (DeckRuleset) null;
    if (!GameDbf.DeckRuleset.HasRecord(id))
    {
      Debug.LogErrorFormat("DeckRuleset not found for id {0}", (object) id);
      return (DeckRuleset) null;
    }
    DeckRuleset deckRulesetFromDbf = new DeckRuleset();
    deckRulesetFromDbf.m_id = id;
    deckRulesetFromDbf.m_rules = new List<DeckRule>();
    foreach (DeckRulesetRuleDbfRecord record in GameDbf.GetIndex().GetRulesForDeckRuleset(id))
    {
      DeckRule fromDbf = DeckRule.CreateFromDBF(record);
      deckRulesetFromDbf.m_rules.Add(fromDbf);
    }
    deckRulesetFromDbf.m_rules.Sort(new Comparison<DeckRule>(DeckRuleViolation.SortComparison_Rule));
    return deckRulesetFromDbf;
  }

  public static DeckRuleset GetRuleset(FormatType formatType)
  {
    DeckRuleset deckRulesetFromDbf;
    if (!DeckRuleset.s_FormatRulesets.TryGetValue(formatType, out deckRulesetFromDbf))
    {
      int id;
      if (new Map<FormatType, int>()
      {
        {
          FormatType.FT_WILD,
          1
        },
        {
          FormatType.FT_STANDARD,
          2
        },
        {
          FormatType.FT_CLASSIC,
          482
        }
      }.TryGetValue(formatType, out id))
      {
        if (GameDbf.DeckRuleset.HasRecord(id))
        {
          deckRulesetFromDbf = DeckRuleset.GetDeckRulesetFromDBF(id);
          DeckRuleset.s_FormatRulesets.Add(formatType, deckRulesetFromDbf);
        }
        else
        {
          Debug.LogError((object) ("Error generating ruleset for id " + id.ToString() + ", could not find ruleset DBF"));
          return (DeckRuleset) null;
        }
      }
      else
      {
        Debug.LogError((object) ("DeckRuleset.GetRuleset called with invalid format type " + formatType.ToString()));
        return (DeckRuleset) null;
      }
    }
    return deckRulesetFromDbf;
  }

  public static DeckRuleset GetPVPDRRuleset()
  {
    if (DeckRuleset.s_PVPDRRuleset == null)
      DeckRuleset.s_PVPDRRuleset = DeckRuleset.BuildPVPDRRuleset();
    return DeckRuleset.s_PVPDRRuleset;
  }

  public static DeckRuleset GetPVPDRDisplayRuleset()
  {
    if (DeckRuleset.s_PVPDRDisplayRuleset == null)
      DeckRuleset.s_PVPDRDisplayRuleset = DeckRuleset.BuildPVPDRDisplayRuleset();
    return DeckRuleset.s_PVPDRDisplayRuleset;
  }

  private static DeckRuleset BuildPVPDRRuleset()
  {
    PvPDungeonRunDisplay pdungeonRunDisplay = PvPDungeonRunDisplay.Get();
    if ((UnityEngine.Object) pdungeonRunDisplay == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unable to get PVPDR DeckRuleset; PvPDungeonRunDisplay unavailable");
      return (DeckRuleset) null;
    }
    PVPDRLobbyDataModel pvpdrLobbyDataModel = pdungeonRunDisplay.GetPVPDRLobbyDataModel();
    PvpdrSeasonDbfRecord record1 = GameDbf.PvpdrSeason.GetRecord(pvpdrLobbyDataModel.Season);
    if (record1 == null)
    {
      Debug.LogErrorFormat("Unable to get PVPDR DeckRuleset; unknown PVPDRSeason {0}", (object) pvpdrLobbyDataModel.Season);
      return (DeckRuleset) null;
    }
    ScenarioDbfRecord record2 = GameDbf.Scenario.GetRecord(record1.ScenarioId);
    if (record2 == null)
    {
      Debug.LogErrorFormat("Unable to get PVPDR DeckRuleset; No scenario specified for season {0}", (object) record1.ID);
      return (DeckRuleset) null;
    }
    DeckRuleset deckRulesetFromDbf = DeckRuleset.GetDeckRulesetFromDBF(record2.DeckRulesetId);
    if (deckRulesetFromDbf == null)
      Debug.LogErrorFormat("Unable to get PVPDR DeckRuleset; no DeckRuleset found with id {0}}", (object) record2.DeckRulesetId);
    return deckRulesetFromDbf;
  }

  private static DeckRuleset BuildPVPDRDisplayRuleset()
  {
    PvPDungeonRunDisplay pdungeonRunDisplay = PvPDungeonRunDisplay.Get();
    if ((UnityEngine.Object) pdungeonRunDisplay == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unable to get PVPDR DeckRuleset; PvPDungeonRunDisplay unavailable");
      return (DeckRuleset) null;
    }
    PVPDRLobbyDataModel pvpdrLobbyDataModel = pdungeonRunDisplay.GetPVPDRLobbyDataModel();
    PvpdrSeasonDbfRecord record = GameDbf.PvpdrSeason.GetRecord(pvpdrLobbyDataModel.Season);
    if (record == null)
    {
      Debug.LogErrorFormat("Unable to get PVPDR DeckRuleset; unknown PVPDRSeason {0}", (object) pvpdrLobbyDataModel.Season);
      return (DeckRuleset) null;
    }
    DeckRuleset deckRulesetFromDbf = DeckRuleset.GetDeckRulesetFromDBF(record.DeckDisplayRulesetId);
    if (deckRulesetFromDbf == null)
      Debug.LogErrorFormat("Unable to get PVPDR DeckRuleset; no DeckRuleset found with id {0}}", (object) record.DeckDisplayRulesetId);
    return deckRulesetFromDbf;
  }

  public bool Filter(EntityDef entity, CollectionDeck deck, DeckRule.RuleType[] ignoreRules = null)
  {
    if (this.EntityIgnoresRuleset(entity) || this.EntityInDeckIgnoresRuleset(deck))
      return true;
    foreach (DeckRule rule in this.m_rules)
    {
      if ((ignoreRules == null || !((IEnumerable<DeckRule.RuleType>) ignoreRules).Contains<DeckRule.RuleType>(rule.Type)) && !rule.Filter(entity, deck))
        return false;
    }
    return true;
  }

  public bool CanAddToDeck(
    EntityDef def,
    TAG_PREMIUM premium,
    CollectionDeck deck,
    params DeckRule.RuleType[] ignoreRules)
  {
    return this.CanAddToDeck(def, premium, deck, out RuleInvalidReason _, out DeckRule _, ignoreRules);
  }

  public bool CanAddToDeck(
    EntityDef def,
    TAG_PREMIUM premium,
    CollectionDeck deck,
    out RuleInvalidReason reason,
    out DeckRule brokenRule,
    params DeckRule.RuleType[] ignoreRules)
  {
    reason = (RuleInvalidReason) null;
    brokenRule = (DeckRule) null;
    if (this.EntityIgnoresRuleset(def) || this.EntityInDeckIgnoresRuleset(deck))
      return true;
    foreach (DeckRule rule in this.m_rules)
    {
      if ((ignoreRules == null || !((IEnumerable<DeckRule.RuleType>) ignoreRules).Contains<DeckRule.RuleType>(rule.Type)) && !rule.CanAddToDeck(def, premium, deck, out reason))
      {
        brokenRule = rule;
        return false;
      }
    }
    return true;
  }

  public bool CanAddToDeck(
    EntityDef def,
    TAG_PREMIUM premium,
    CollectionDeck deck,
    out List<RuleInvalidReason> reasons,
    out List<DeckRule> brokenRules,
    params DeckRule.RuleType[] ignoreRules)
  {
    if (this.EntityIgnoresRuleset(def) || this.EntityInDeckIgnoresRuleset(deck))
    {
      reasons = (List<RuleInvalidReason>) null;
      brokenRules = (List<DeckRule>) null;
      return true;
    }
    reasons = new List<RuleInvalidReason>();
    brokenRules = new List<DeckRule>();
    foreach (DeckRule rule in this.m_rules)
    {
      RuleInvalidReason reason;
      if ((ignoreRules == null || !((IEnumerable<DeckRule.RuleType>) ignoreRules).Contains<DeckRule.RuleType>(rule.Type)) && !rule.CanAddToDeck(def, premium, deck, out reason))
      {
        reasons.Add(reason);
        brokenRules.Add(rule);
      }
    }
    return brokenRules.Count == 0;
  }

  public bool IsDeckValid(CollectionDeck deck, params DeckRule.RuleType[] ignoreRules) => this.IsDeckValid(deck, out IList<DeckRuleViolation> _, ignoreRules);

  public bool IsDeckValid(
    CollectionDeck deck,
    out IList<DeckRuleViolation> violations,
    params DeckRule.RuleType[] ignoreRules)
  {
    List<DeckRuleViolation> deckRuleViolationList = new List<DeckRuleViolation>();
    violations = (IList<DeckRuleViolation>) deckRuleViolationList;
    List<RuleInvalidReason> reasons = new List<RuleInvalidReason>();
    if (this.EntityInDeckIgnoresRuleset(deck))
      return true;
    bool flag1 = true;
    foreach (DeckRule rule in this.m_rules)
    {
      if (ignoreRules == null || !((IEnumerable<DeckRule.RuleType>) ignoreRules).Contains<DeckRule.RuleType>(rule.Type))
      {
        RuleInvalidReason reason;
        bool flag2 = rule.IsDeckValid(deck, out reason);
        if (!flag2)
        {
          reasons.Add(reason);
          DeckRuleViolation deckRuleViolation = new DeckRuleViolation(rule, reason.DisplayError);
          violations.Add(deckRuleViolation);
          flag1 = false;
        }
        Log.DeckRuleset.Print("validating rule={0} deck={1} result={2} reason={3}", (object) rule, (object) deck, (object) flag2, (object) reason);
      }
    }
    deckRuleViolationList.Sort(new Comparison<DeckRuleViolation>(DeckRuleViolation.SortComparison_Violation));
    this.CollapseSpecialBrokenRules(violations, reasons);
    return flag1;
  }

  private void CollapseSpecialBrokenRules(
    IList<DeckRuleViolation> violations,
    List<RuleInvalidReason> reasons)
  {
    if (reasons.Count <= 1)
      return;
    DeckRule rule1 = (DeckRule) null;
    int countParam = 0;
    List<int> intList = (List<int>) null;
    for (int index = 0; index < violations.Count; ++index)
    {
      DeckRule rule2 = violations[index].Rule;
      if (rule2.Type == DeckRule.RuleType.PLAYER_OWNS_EACH_COPY && !rule2.RuleIsNot)
      {
        if (intList == null)
          intList = new List<int>();
        if (rule1 == null)
          rule1 = rule2;
        intList.Add(index);
        countParam += reasons[index].CountParam;
      }
      else if (rule2.Type == DeckRule.RuleType.DECK_SIZE && reasons[index].IsMinimum)
      {
        if (intList == null)
          intList = new List<int>();
        if (rule1 == null)
          rule1 = rule2;
        intList.Add(index);
        countParam += reasons[index].CountParam;
      }
    }
    if (intList == null || intList.Count <= 1)
      return;
    for (int index1 = intList == null ? -1 : intList.Count - 1; index1 >= 0; --index1)
    {
      int index2 = intList[index1];
      violations.RemoveAt(index2);
      reasons.RemoveAt(index2);
    }
    string str = GameStrings.Format("GLUE_COLLECTION_DECK_RULE_MISSING_CARDS", (object) countParam);
    RuleInvalidReason ruleInvalidReason = new RuleInvalidReason(str, countParam);
    reasons.Add(ruleInvalidReason);
    DeckRuleViolation deckRuleViolation = new DeckRuleViolation(rule1, str);
    violations.Add(deckRuleViolation);
  }

  private DeckRule_DeckSize GetDeckSizeRule(CollectionDeck deck)
  {
    DeckRule deckRule = this.m_rules == null ? (DeckRule) null : this.m_rules.FirstOrDefault<DeckRule>((Func<DeckRule, bool>) (r => r is DeckRule_DeckSize));
    return deckRule != null ? deckRule as DeckRule_DeckSize : (DeckRule_DeckSize) null;
  }

  public int GetDeckSize(CollectionDeck deck)
  {
    DeckRule_DeckSize deckSizeRule = this.GetDeckSizeRule(deck);
    return deckSizeRule != null ? deckSizeRule.GetMaximumDeckSize(deck) : 30;
  }

  private DeckRule_EditingDeckExtraCardCount GetEditingDeckExtraCardCountRule()
  {
    List<DeckRule> rules = this.m_rules;
    DeckRule deckRule = rules != null ? rules.FirstOrDefault<DeckRule>((Func<DeckRule, bool>) (r => r is DeckRule_EditingDeckExtraCardCount)) : (DeckRule) null;
    return deckRule != null ? deckRule as DeckRule_EditingDeckExtraCardCount : (DeckRule_EditingDeckExtraCardCount) null;
  }

  public int GetDeckSizeWhileEditing(CollectionDeck deck, EntityDef cardBeingAdded = null)
  {
    int sizeWhileEditing = this.GetDeckSize(deck);
    if (sizeWhileEditing < 30)
      return sizeWhileEditing;
    if (cardBeingAdded != null && cardBeingAdded.HasTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE))
      sizeWhileEditing = cardBeingAdded.GetTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE);
    if (this.IsOvercappedDecksEnabled())
    {
      DeckRule_EditingDeckExtraCardCount extraCardCountRule = this.GetEditingDeckExtraCardCountRule();
      if (extraCardCountRule != null)
        sizeWhileEditing += extraCardCountRule.GetEditingDeckExtraCardCount();
      else
        sizeWhileEditing = sizeWhileEditing;
    }
    return sizeWhileEditing;
  }

  public int GetMinimumAllowedDeckSize(CollectionDeck deck)
  {
    DeckRule_DeckSize deckSizeRule = this.GetDeckSizeRule(deck);
    return deckSizeRule != null ? deckSizeRule.GetMinimumDeckSize(deck) : 30;
  }

  public bool HasOwnershipOrRotatedRule() => (this.m_rules == null ? (DeckRule) null : this.m_rules.FirstOrDefault<DeckRule>((Func<DeckRule, bool>) (r =>
  {
    if (r.Type == DeckRule.RuleType.IS_NOT_ROTATED)
      return true;
    return r.Type == DeckRule.RuleType.PLAYER_OWNS_EACH_COPY && !r.RuleIsNot;
  }))) != null;

  public bool FilterFailsOnShowInvalidRule(EntityDef entity, CollectionDeck deck)
  {
    bool flag = false;
    foreach (DeckRule rule in this.m_rules)
    {
      if (!rule.Filter(entity, deck))
      {
        if (rule.ShowInvalidCards)
        {
          flag = true;
        }
        else
        {
          flag = false;
          break;
        }
      }
    }
    return flag;
  }

  public bool HasIsPlayableRule()
  {
    foreach (DeckRule rule in this.m_rules)
    {
      if (rule.Type == DeckRule.RuleType.IS_CARD_PLAYABLE && !rule.RuleIsNot)
        return true;
    }
    return false;
  }

  public int GetMaxCopiesOfCardAllowed(EntityDef entity)
  {
    int a = int.MaxValue;
    foreach (DeckRule rule in this.m_rules)
    {
      int maxCopies;
      if (rule is DeckRule_CountCopiesOfEachCard && ((DeckRule_CountCopiesOfEachCard) rule).GetMaxCopies(entity, out maxCopies))
        a = Mathf.Min(a, maxCopies);
    }
    return a;
  }

  public HashSet<TAG_CARD_SET> GetAllowedCardSets()
  {
    HashSet<TAG_CARD_SET> allowedCardSets = new HashSet<TAG_CARD_SET>();
    foreach (DeckRule rule in this.m_rules)
    {
      if (rule is DeckRule_IsInAnySubset)
      {
        foreach (int num in GameDbf.GetIndex().GetCardSetIdsForSubsetRule(rule.GetID()))
          allowedCardSets.Add((TAG_CARD_SET) num);
      }
    }
    return allowedCardSets;
  }

  public bool EntityIgnoresRuleset(EntityDef def) => def.HasTag(GAME_TAG.IGNORE_DECK_RULESET);

  public bool EntityInDeckIgnoresRuleset(CollectionDeck deck)
  {
    DefLoader defLoader = DefLoader.Get();
    List<CollectionDeckSlot> slots = deck.GetSlots();
    int index = 0;
    for (int count = slots.Count; index < count; ++index)
    {
      if (this.EntityIgnoresRuleset(defLoader.GetEntityDef(slots[index].CardID)))
        return true;
    }
    return false;
  }

  private bool IsOvercappedDecksEnabled()
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheFeatures>();
    return netObject != null && netObject.OvercappedDecksEnabled;
  }
}
