using Hearthstone;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EntityBase
{
  protected static int DEFAULT_TAG_MAP_SIZE = 15;
  protected TagMap m_tags;
  protected TagMap m_cachedTagsForDormant;
  private List<CardSetTimingDbfRecord> m_cardSetTimingRecords;
  private string m_cardIdInternal;
  private List<TAG_RACE> m_entityRaces;
  private static List<TAG_CLASS> s_allowedClasses = new List<TAG_CLASS>();

  protected string m_cardId
  {
    set
    {
      this.m_cardIdInternal = value;
      this.m_cardSetTimingRecords = (List<CardSetTimingDbfRecord>) null;
    }
    get => this.m_cardIdInternal;
  }

  public EntityBase()
  {
    this.m_tags = new TagMap(EntityBase.DEFAULT_TAG_MAP_SIZE);
    this.m_cachedTagsForDormant = new TagMap();
  }

  public EntityBase(int tagMapSize)
  {
    this.m_tags = new TagMap(tagMapSize);
    this.m_cachedTagsForDormant = new TagMap();
  }

  public bool HasTag(GAME_TAG tag) => this.GetTag(tag) > 0;

  public TagMap GetTags() => this.m_tags;

  public int GetTag(int tag) => this.m_tags.GetTag(tag);

  public int GetTag(GAME_TAG enumTag) => this.m_tags.GetTag((int) enumTag);

  public TagEnum GetTag<TagEnum>(GAME_TAG enumTag) => (TagEnum) Enum.ToObject(typeof (TagEnum), this.GetTag(enumTag));

  public void SetTag(int tag, int tagValue) => this.m_tags.SetTag(tag, tagValue);

  public void SetTag(GAME_TAG tag, int tagValue) => this.SetTag((int) tag, tagValue);

  public void SetTag<TagEnum>(GAME_TAG tag, TagEnum tagValue) => this.SetTag((int) tag, Convert.ToInt32((object) tagValue));

  public void SetTags(List<Network.Entity.Tag> tags) => this.m_tags.SetTags(tags);

  public void ReplaceTags(TagMap tags) => this.m_tags.Replace(tags);

  public bool HasReferencedTag(GAME_TAG enumTag) => this.GetReferencedTag(enumTag) > 0;

  public int GetReferencedTag(GAME_TAG enumTag) => this.GetReferencedTag((int) enumTag);

  public abstract int GetReferencedTag(int tag);

  public bool HasCachedTagForDormant(GAME_TAG tag) => this.GetCachedTagForDormant(tag) > 0;

  public int GetCachedTagForDormant(GAME_TAG enumTag) => this.m_cachedTagsForDormant.GetTag((int) enumTag);

  public void SetCachedTagForDormant(int tag, int tagValue) => this.m_cachedTagsForDormant.SetTag(tag, tagValue);

  public bool HasAvenge() => this.HasTag(GAME_TAG.AVENGE);

  public bool HasCharge() => this.HasTag(GAME_TAG.CHARGE);

  public bool HasBattlecry() => this.HasTag(GAME_TAG.BATTLECRY);

  public bool HasTriggerVisual() => this.HasTag(GAME_TAG.TRIGGER_VISUAL);

  public bool HasInspire() => this.HasTag(GAME_TAG.INSPIRE);

  public bool HasOverKill() => this.HasTag(GAME_TAG.OVERKILL);

  public bool HasSpellburst() => this.HasTag(GAME_TAG.SPELLBURST) || this.HasTag(GAME_TAG.NON_KEYWORD_SPELLBURST);

  public bool HasFrenzy() => this.HasTag(GAME_TAG.FRENZY);

  public bool HasHonorableKill() => this.HasTag(GAME_TAG.HONORABLEKILL);

  public bool HasCounter() => this.HasTag(GAME_TAG.COUNTER);

  public bool IsPoisonous() => this.HasTag(GAME_TAG.POISONOUS) || this.HasTag(GAME_TAG.NON_KEYWORD_POISONOUS);

  public bool HasLifesteal() => this.HasTag(GAME_TAG.LIFESTEAL);

  public bool IsEnraged() => this.HasTag(GAME_TAG.ENRAGED) && this.GetDamage() > 0;

  public int GetDamage() => this.GetTag(GAME_TAG.DAMAGE);

  public bool IsFrozen() => this.HasTag(GAME_TAG.FROZEN);

  public bool IsDormant() => this.HasTag(GAME_TAG.DORMANT);

  public bool IsAsleep() => this.GetNumTurnsInPlay() == 0 && this.GetNumAttacksThisTurn() == 0 && !this.HasCharge() && !this.HasRush() && !this.ReferencesAutoAttack() && !this.HasTag(GAME_TAG.UNTOUCHABLE) && !this.IsLocation() && (GameState.Get() == null || GameState.Get().GetGameEntity() == null || GameState.Get().GetBooleanGameOption(GameEntityOption.ALLOW_SLEEP_FX));

  public bool IsStealthed() => this.HasTag(GAME_TAG.STEALTH);

  public bool HasTaunt() => this.HasTag(GAME_TAG.TAUNT);

  public bool ReferencesAutoAttack() => this.HasReferencedTag(GAME_TAG.AUTOATTACK);

  public bool IsHero() => this.GetTag(GAME_TAG.CARDTYPE) == 3;

  public bool IsHeroPower() => this.GetTag(GAME_TAG.CARDTYPE) == 10;

  public bool IsGameModeButton() => this.GetTag(GAME_TAG.CARDTYPE) == 12;

  public bool IsLettuceAbility() => this.GetTag(GAME_TAG.CARDTYPE) == 23;

  public bool IsLettuceEquipment() => this.IsLettuceAbility() && this.HasTag(GAME_TAG.LETTUCE_IS_EQUPIMENT);

  public bool IsLettuceAbilitySpellCasting() => this.IsLettuceAbility() && !this.HasTag(GAME_TAG.LETTUCE_ABILITY_SUMMONED_MINION);

  public bool IsLettuceAbilityMinionSummoning() => this.IsLettuceAbility() && this.HasTag(GAME_TAG.LETTUCE_ABILITY_SUMMONED_MINION);

  public bool IsLettuceMercenary() => this.GetTag(GAME_TAG.LETTUCE_MERCENARY) > 0;

  public bool IsMinion() => this.GetTag(GAME_TAG.CARDTYPE) == 4;

  public bool IsSpell() => this.GetTag(GAME_TAG.CARDTYPE) == 5;

  public bool IsWeapon() => this.GetTag(GAME_TAG.CARDTYPE) == 7;

  public bool IsLocation() => this.GetTag(GAME_TAG.CARDTYPE) == 39;

  public int GetLocationCooldown() => this.GetTag(GAME_TAG.EXHAUSTED) + this.GetTag(GAME_TAG.LOCATION_ACTION_COOLDOWN);

  public bool IsElite() => this.GetTag(GAME_TAG.ELITE) > 0;

  public bool IsHeroSkin()
  {
    if (!this.IsHero())
      return false;
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardIdInternal);
    return cardRecord != null && cardRecord.CardHero != null;
  }

  public bool IsCardButton() => this.IsHeroPower() || this.IsLocation() || this.IsGameModeButton() || this.IsLettuceAbility();

  public bool IsMoveMinionHoverTarget() => this.GetTag(GAME_TAG.CARDTYPE) == 22;

  public bool IsBattlegroundHeroBuddy() => this.GetTag(GAME_TAG.CARDTYPE) == 24;

  public bool IsBattlegroundQuestReward() => this.GetTag(GAME_TAG.CARDTYPE) == 40;

  public bool IsCustomCoin()
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardIdInternal);
    return GameDbf.Coin.HasRecord((Predicate<CoinDbfRecord>) (coin => coin.CardId == cardRecord.ID));
  }

  public TAG_CARDTYPE GetCardType() => (TAG_CARDTYPE) this.GetTag(GAME_TAG.CARDTYPE);

  public TAG_PUZZLE_TYPE GetPuzzleType() => (TAG_PUZZLE_TYPE) this.GetTag(GAME_TAG.PUZZLE_TYPE);

  public bool IsGame() => this.GetTag(GAME_TAG.CARDTYPE) == 1;

  public bool IsPlayer() => this.GetTag(GAME_TAG.CARDTYPE) == 2;

  public bool IsExhausted() => this.HasTag(GAME_TAG.EXHAUSTED);

  public bool IsAttached() => this.HasTag(GAME_TAG.ATTACHED);

  public bool IsObfuscated() => this.HasTag(GAME_TAG.OBFUSCATED);

  public bool HasSecretDeathrattle() => this.HasTag(GAME_TAG.SECRET_DEATHRATTLE);

  public bool IsSecret() => this.HasTag(GAME_TAG.SECRET);

  public bool IsBobQuest() => this.HasTag(GAME_TAG.BACON_IS_BOB_QUEST);

  public bool IsQuest() => this.HasTag(GAME_TAG.QUEST);

  public bool IsQuestline() => this.HasTag(GAME_TAG.QUESTLINE);

  public bool IsSideQuest() => this.HasTag(GAME_TAG.SIDEQUEST);

  public bool IsSigil() => this.HasTag(GAME_TAG.SIGIL);

  public bool IsObjective() => this.HasTag(GAME_TAG.OBJECTIVE);

  public bool IsPuzzle() => this.HasTag(GAME_TAG.PUZZLE);

  public bool IsRulebook() => this.HasTag(GAME_TAG.RULEBOOK);

  public bool IsSecretLike() => this.IsSecret() || this.IsQuest() || this.IsQuestline() || this.IsSideQuest() || this.IsSigil() || this.IsObjective();

  public bool IsRevealed() => this.HasTag(GAME_TAG.REVEALED);

  public bool IsTwinspell() => this.HasTag(GAME_TAG.TWINSPELL) || this.HasTag(GAME_TAG.TWINSPELL_COPY);

  public int GetNumTurnsInPlay() => this.GetTag(GAME_TAG.NUM_TURNS_IN_PLAY);

  public int GetNumAttacksThisTurn() => this.GetTag(GAME_TAG.NUM_ATTACKS_THIS_TURN);

  public TAG_SPELL_SCHOOL GetSpellPowerSchool()
  {
    if (this.HasTag(GAME_TAG.SPELLPOWER))
      return TAG_SPELL_SCHOOL.NONE;
    if (this.HasTag(GAME_TAG.SPELLPOWER_ARCANE))
      return TAG_SPELL_SCHOOL.ARCANE;
    if (this.HasTag(GAME_TAG.SPELLPOWER_FIRE))
      return TAG_SPELL_SCHOOL.FIRE;
    if (this.HasTag(GAME_TAG.SPELLPOWER_FROST))
      return TAG_SPELL_SCHOOL.FROST;
    if (this.HasTag(GAME_TAG.SPELLPOWER_NATURE))
      return TAG_SPELL_SCHOOL.NATURE;
    if (this.HasTag(GAME_TAG.SPELLPOWER_HOLY))
      return TAG_SPELL_SCHOOL.HOLY;
    if (this.HasTag(GAME_TAG.SPELLPOWER_SHADOW))
      return TAG_SPELL_SCHOOL.SHADOW;
    if (this.HasTag(GAME_TAG.SPELLPOWER_FEL))
      return TAG_SPELL_SCHOOL.FEL;
    return this.HasTag(GAME_TAG.SPELLPOWER_PHYSICAL) ? TAG_SPELL_SCHOOL.PHYSICAL_COMBAT : TAG_SPELL_SCHOOL.NONE;
  }

  public bool HasSpellPower() => this.HasTag(GAME_TAG.SPELLPOWER) || this.HasTag(GAME_TAG.SPELLPOWER_ARCANE) || this.HasTag(GAME_TAG.SPELLPOWER_FIRE) || this.HasTag(GAME_TAG.SPELLPOWER_FROST) || this.HasTag(GAME_TAG.SPELLPOWER_NATURE) || this.HasTag(GAME_TAG.SPELLPOWER_HOLY) || this.HasTag(GAME_TAG.SPELLPOWER_SHADOW) || this.HasTag(GAME_TAG.SPELLPOWER_FEL) || this.HasTag(GAME_TAG.SPELLPOWER_PHYSICAL);

  public bool HasHeroPowerDamage() => this.HasTag(GAME_TAG.HEROPOWER_DAMAGE);

  public bool IsAffectedBySpellPower() => this.HasTag(GAME_TAG.AFFECTED_BY_SPELL_POWER);

  public bool HasSpellPowerDouble() => this.HasTag(GAME_TAG.SPELLPOWER_DOUBLE);

  public bool HasHealingDoesDamageHint() => this.HasTag(GAME_TAG.HEALING_DOES_DAMAGE_HINT);

  public bool HasLifestealDoesDamageHint() => this.HasTag(GAME_TAG.LIFESTEAL_DOES_DAMAGE_HINT);

  public int GetCost() => this.GetTag(GAME_TAG.COST);

  public int GetATK() => this.GetTag(GAME_TAG.ATK);

  public int GetHealth() => this.GetTag(GAME_TAG.HEALTH);

  public int GetDurability() => this.GetTag(GAME_TAG.DURABILITY);

  public int GetArmor() => this.GetTag(GAME_TAG.ARMOR);

  public int GetAttached() => this.GetTag(GAME_TAG.ATTACHED);

  public int GetBloodCost() => this.GetTag(GAME_TAG.COST_BLOOD);

  public int GetFrostCost() => this.GetTag(GAME_TAG.COST_FROST);

  public int GetUnholyCost() => this.GetTag(GAME_TAG.COST_UNHOLY);

  public RunePattern GetRuneCost() => new RunePattern(this.GetBloodCost(), this.GetFrostCost(), this.GetUnholyCost());

  public bool HasRuneCost => this.GetBloodCost() + this.GetFrostCost() + this.GetUnholyCost() > 0;

  public TAG_ZONE GetZone()
  {
    TAG_ZONE tag = (TAG_ZONE) this.GetTag(GAME_TAG.FAKE_ZONE);
    return tag != TAG_ZONE.INVALID ? tag : (TAG_ZONE) this.GetTag(GAME_TAG.ZONE);
  }

  public int GetZonePosition()
  {
    int tag = this.GetTag(GAME_TAG.FAKE_ZONE_POSITION);
    return tag > 0 ? tag : this.GetTag(GAME_TAG.ZONE_POSITION);
  }

  public int GetCreatorId() => this.GetTag(GAME_TAG.CREATOR);

  public int GetCreatorDBID() => this.GetTag(GAME_TAG.CREATOR_DBID);

  public int GetControllerId()
  {
    int tag = this.GetTag(GAME_TAG.FAKE_CONTROLLER);
    return tag > 0 ? tag : this.GetTag(GAME_TAG.CONTROLLER);
  }

  public bool HasWindfury() => this.GetTag(GAME_TAG.WINDFURY) > 0;

  public bool HasCombo() => this.HasTag(GAME_TAG.COMBO);

  public bool HasDeathrattle() => this.HasTag(GAME_TAG.DEATHRATTLE);

  public bool IsSilenced() => this.HasTag(GAME_TAG.SILENCED);

  public int GetEntityId() => this.GetTag(GAME_TAG.ENTITY_ID);

  public bool IsCharacter() => this.IsHero() || this.IsMinion();

  public bool HasRush() => this.HasTag(GAME_TAG.RUSH);

  public int GetTechLevel() => this.GetTag(GAME_TAG.TECH_LEVEL);

  public bool IsCoreCard()
  {
    TAG_CARD_SET cardSet1 = this.GetCardSet();
    if (cardSet1 == TAG_CARD_SET.INVALID)
      return false;
    CardSetDbfRecord cardSet2 = GameDbf.GetIndex().GetCardSet(cardSet1);
    if (cardSet2 != null)
      return cardSet2.IsCoreCardSet;
    Debug.LogWarning((object) string.Format("Got null card set ID: {0}", (object) cardSet1));
    return false;
  }

  public virtual TAG_CLASS GetClass()
  {
    EntityBase.s_allowedClasses.Clear();
    this.GetClasses((IList<TAG_CLASS>) EntityBase.s_allowedClasses);
    int count = EntityBase.s_allowedClasses.Count;
    if (count == 0)
      return TAG_CLASS.INVALID;
    return 1 == count ? EntityBase.s_allowedClasses[0] : TAG_CLASS.NEUTRAL;
  }

  public virtual void GetClasses(IList<TAG_CLASS> classes)
  {
    classes.Clear();
    uint tag1 = (uint) this.GetTag(GAME_TAG.MULTIPLE_CLASSES);
    if (tag1 == 0U)
    {
      TAG_CLASS tag2 = (TAG_CLASS) this.GetTag(GAME_TAG.CLASS);
      classes.Add(tag2);
    }
    else
    {
      int num = 1;
      while (tag1 != 0U)
      {
        if (1 == ((int) tag1 & 1))
          classes.Add((TAG_CLASS) num);
        tag1 >>= 1;
        ++num;
      }
    }
  }

  public bool HasClass(TAG_CLASS tagClass)
  {
    this.GetClasses((IList<TAG_CLASS>) EntityBase.s_allowedClasses);
    return EntityBase.s_allowedClasses.Contains(tagClass);
  }

  public bool IsMultiClass()
  {
    this.GetClasses((IList<TAG_CLASS>) EntityBase.s_allowedClasses);
    return EntityBase.s_allowedClasses.Count > 1;
  }

  public List<TAG_RACE> GetRaces()
  {
    if (this.m_entityRaces != null)
      return this.m_entityRaces;
    this.m_entityRaces = new List<TAG_RACE>();
    if (this.GetTag(GAME_TAG.CARDRACE) != 0)
      this.m_entityRaces.Add((TAG_RACE) this.GetTag(GAME_TAG.CARDRACE));
    foreach (CardRaceDbfRecord record in GameDbf.CardRace.GetRecords())
    {
      if (this.HasTag((GAME_TAG) record.IsRaceTag))
        this.m_entityRaces.Add((TAG_RACE) record.ID);
    }
    this.m_entityRaces = this.m_entityRaces.Distinct<TAG_RACE>().ToList<TAG_RACE>();
    if (this.m_entityRaces.Count > 1)
    {
      TAG_RACE[] order = new TAG_RACE[11]
      {
        TAG_RACE.UNDEAD,
        TAG_RACE.ELEMENTAL,
        TAG_RACE.MECHANICAL,
        TAG_RACE.DEMON,
        TAG_RACE.MURLOC,
        TAG_RACE.QUILBOAR,
        TAG_RACE.NAGA,
        TAG_RACE.PET,
        TAG_RACE.DRAGON,
        TAG_RACE.TOTEM,
        TAG_RACE.PIRATE
      };
      this.m_entityRaces.Sort((Comparison<TAG_RACE>) ((r1, r2) => Array.IndexOf<TAG_RACE>(order, r1).CompareTo(Array.IndexOf<TAG_RACE>(order, r2))));
    }
    return this.m_entityRaces;
  }

  public int GetRaceCount() => this.GetRaces().Count;

  public bool IsTradeable() => this.HasTag(GAME_TAG.TRADEABLE);

  public TAG_MULTI_CLASS_GROUP GetMultiClassGroup() => (TAG_MULTI_CLASS_GROUP) this.GetTag(GAME_TAG.MULTI_CLASS_GROUP);

  public TAG_ROLE GetMercenaryRole() => this.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);

  public string GetCardId() => this.m_cardId;

  public void SetCardId(string cardId)
  {
    this.m_cardId = cardId;
    this.OnUpdateCardId();
  }

  protected virtual void OnUpdateCardId()
  {
  }

  public void InitCardSetTimings(
    Dictionary<int, List<CardSetTimingDbfRecord>> timings)
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardIdInternal);
    if (cardRecord == null)
      return;
    this.SetCardSetTimings(cardRecord.ID, timings);
  }

  public TAG_CARD_SET GetCardSet()
  {
    TAG_CARD_SET tag = (TAG_CARD_SET) this.GetTag(GAME_TAG.CARD_SET);
    if (tag != TAG_CARD_SET.INVALID)
      return tag;
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardIdInternal);
    if (cardRecord != null)
    {
      this.SetCardSetTimings(cardRecord.ID, (Dictionary<int, List<CardSetTimingDbfRecord>>) null);
      bool hearthstoneRunning = HearthstoneApplication.IsHearthstoneRunning;
      SpecialEventManager specialEventManager = SpecialEventManager.Get();
      foreach (CardSetTimingDbfRecord cardSetTimingRecord in this.m_cardSetTimingRecords)
      {
        if (!hearthstoneRunning || specialEventManager.IsEventActive(cardSetTimingRecord.EventTimingEvent, false))
          return (TAG_CARD_SET) cardSetTimingRecord.CardSetId;
      }
    }
    return TAG_CARD_SET.INVALID;
  }

  public TAG_SPELL_SCHOOL GetSpellSchool() => this.GetTag<TAG_SPELL_SCHOOL>(GAME_TAG.SPELL_SCHOOL);

  public bool IsCollectionManagerFilterManaCostByEven => this.GetTag(GAME_TAG.COLLECTIONMANAGER_FILTER_MANA_EVEN) != 0;

  public bool IsCollectionManagerFilterManaCostByOdd => this.GetTag(GAME_TAG.COLLECTIONMANAGER_FILTER_MANA_ODD) != 0;

  private void SetCardSetTimings(
    int cardRecordId,
    Dictionary<int, List<CardSetTimingDbfRecord>> timings)
  {
    if (this.m_cardSetTimingRecords != null)
      return;
    if (timings == null)
    {
      this.m_cardSetTimingRecords = new List<CardSetTimingDbfRecord>();
      List<CardSetTimingDbfRecord> records = GameDbf.CardSetTiming.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        CardSetTimingDbfRecord setTimingDbfRecord = records[index];
        if (setTimingDbfRecord.CardId == cardRecordId)
          this.m_cardSetTimingRecords.Add(setTimingDbfRecord);
      }
    }
    else
      timings.TryGetValue(cardRecordId, out this.m_cardSetTimingRecords);
    if (HearthstoneApplication.IsHearthstoneRunning)
      return;
    this.m_cardSetTimingRecords.Sort((Comparison<CardSetTimingDbfRecord>) ((a, b) => -a.ID.CompareTo(b.ID)));
  }
}
