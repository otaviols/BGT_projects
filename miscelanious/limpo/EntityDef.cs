using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityDef : EntityBase
{
  protected TagMap m_referencedTags = new TagMap();
  private CardTextBuilder m_cardTextBuilder;
  private static readonly CardPortraitQuality s_noTextureQuality = new CardPortraitQuality(0, TAG_PREMIUM.NORMAL);
  private EntityDef.CachedEntityName m_cachedEntityName;
  private EntityDef.CachedEntityDebugName m_cachedEntityDebugName;
  private EntityDef.CachedEntityCardId m_cachedLettuceAbilitySummonedMinion;

  public EntityDef()
  {
  }

  public EntityDef(int size)
    : base(size)
  {
  }

  public override string ToString() => this.GetDebugName();

  public EntityDef Clone()
  {
    EntityDef entityDef = new EntityDef();
    entityDef.m_cardId = this.m_cardId;
    entityDef.ReplaceTags(this.m_tags);
    entityDef.m_referencedTags.Replace(this.m_referencedTags);
    return entityDef;
  }

  public bool UseTechLevelManaGem()
  {
    if (!this.IsMinion())
      return false;
    GameEntity gameEntity = GameState.Get()?.GetGameEntity();
    return gameEntity != null && gameEntity.HasTag(GAME_TAG.TECH_LEVEL_MANA_GEM);
  }

  public override int GetReferencedTag(int tag) => this.m_referencedTags.GetTag(tag);

  public void SetReferencedTag(int tag, int val) => this.m_referencedTags.SetTag(tag, val);

  public TAG_ENCHANTMENT_VISUAL GetEnchantmentBirthVisual() => this.GetTag<TAG_ENCHANTMENT_VISUAL>(GAME_TAG.ENCHANTMENT_BIRTH_VISUAL);

  public TAG_ENCHANTMENT_VISUAL GetEnchantmentIdleVisual() => this.GetTag<TAG_ENCHANTMENT_VISUAL>(GAME_TAG.ENCHANTMENT_IDLE_VISUAL);

  public TAG_RARITY GetRarity() => (TAG_RARITY) this.GetTag(GAME_TAG.RARITY);

  public (bool valid, int attack, int health) GetSummonedMinionStats()
  {
    int tag = this.GetTag(GAME_TAG.LETTUCE_ABILITY_SUMMONED_MINION);
    if (this.m_cachedLettuceAbilitySummonedMinion.CardDBId != tag)
    {
      this.m_cachedLettuceAbilitySummonedMinion.CardDBId = tag;
      this.m_cachedLettuceAbilitySummonedMinion.CardId = GameUtils.TranslateDbIdToCardId(this.m_cachedLettuceAbilitySummonedMinion.CardDBId);
    }
    EntityDef entityDef = DefLoader.Get().GetEntityDef(this.m_cachedLettuceAbilitySummonedMinion.CardId);
    return entityDef != null ? (true, entityDef.GetATK(), entityDef.GetHealth()) : (false, 0, 0);
  }

  public bool HasValidDisplayName()
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardId);
    return cardRecord != null && cardRecord.Name != null && cardRecord.Name.GetString() != null;
  }

  public string GetName()
  {
    if (!this.IsValidEntityName())
      this.UpdateEntityName();
    return this.m_cachedEntityName.Name;
  }

  public string GetShortName()
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardId);
    return cardRecord != null && cardRecord.ShortName != null ? cardRecord.ShortName.GetString() : (string) null;
  }

  public string GetDebugName()
  {
    if (!this.IsValidEntityDebugName())
      this.UpdateEntityDebugName();
    return this.m_cachedEntityDebugName.Name;
  }

  public string GetArtistName(TAG_PREMIUM premium)
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardId);
    if (cardRecord == null)
      return "ERROR: NO ARTIST NAME";
    return premium == TAG_PREMIUM.SIGNATURE ? cardRecord.SignatureArtistName ?? string.Empty : cardRecord.ArtistName ?? string.Empty;
  }

  public string GetFlavorText()
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardId);
    return cardRecord == null || cardRecord.FlavorText == null ? string.Empty : cardRecord.FlavorText.GetString() ?? string.Empty;
  }

  public string GetHowToEarnText(TAG_PREMIUM premium)
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardId);
    if (cardRecord == null)
      return string.Empty;
    switch (premium)
    {
      case TAG_PREMIUM.GOLDEN:
        if (cardRecord.HowToGetGoldCard != null)
          return cardRecord.HowToGetGoldCard.GetString() ?? string.Empty;
        break;
      case TAG_PREMIUM.DIAMOND:
        if (cardRecord.HowToGetDiamondCard != null)
          return cardRecord.HowToGetDiamondCard.GetString() ?? string.Empty;
        break;
      case TAG_PREMIUM.SIGNATURE:
        if (cardRecord.HowToGetSignatureCard != null)
          return cardRecord.HowToGetSignatureCard.GetString() ?? string.Empty;
        break;
      default:
        if (cardRecord.HowToGetCard != null)
          return cardRecord.HowToGetCard.GetString() ?? string.Empty;
        break;
    }
    return string.Empty;
  }

  public string GetCardTextInHand()
  {
    if (this.GetCardTextBuilder() != null)
      return this.GetCardTextBuilder().BuildCardTextInHand(this);
    Debug.LogWarning((object) string.Format("EntityDef.GetCardTextInHand: No textbuilder found for {0}, returning default text", (object) this.m_cardId));
    return CardTextBuilder.GetDefaultCardTextInHand(this);
  }

  public string GetRaceText()
  {
    if (this.IsMinion() && this.HasTag(GAME_TAG.CARDRACE))
    {
      List<TAG_RACE> races = this.GetRaces();
      if (races.Count<TAG_RACE>() > 0)
      {
        string str = "";
        foreach (TAG_RACE tag in races)
        {
          str += GameStrings.GetRaceName(tag);
          str += "\n";
        }
        return str.Remove(str.Length - 1);
      }
    }
    return (this.IsSpell() ? 1 : (this.IsLettuceAbility() ? 1 : 0)) != 0 && this.HasTag(GAME_TAG.SPELL_SCHOOL) ? GameStrings.GetSpellSchoolName(this.GetSpellSchool()) : "";
  }

  public CardTextBuilder GetCardTextBuilder()
  {
    if (this.m_cardTextBuilder == null)
    {
      if (this.HasTag(GAME_TAG.OVERRIDECARDTEXTBUILDER))
      {
        this.m_cardTextBuilder = CardTextBuilderFactory.Create((Assets.Card.CardTextBuilderType) this.GetTag(GAME_TAG.OVERRIDECARDTEXTBUILDER));
      }
      else
      {
        CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardId);
        if (cardRecord != null)
          this.m_cardTextBuilder = CardTextBuilderFactory.Create(cardRecord.CardTextBuilderType);
      }
    }
    return this.m_cardTextBuilder;
  }

  public void ClearCardTextBuilder() => this.m_cardTextBuilder = (CardTextBuilder) null;

  public string GetWatermarkTextureOverride() => GameDbf.GetIndex().GetCardRecord(this.m_cardId)?.WatermarkTextureOverride;

  public bool LoadTagFromDBF(string designCode, List<CardTagDbfRecord> tags)
  {
    this.m_cardId = designCode;
    return this.LoadTagFromDBF_SetTags(tags);
  }

  public static Dictionary<string, EntityDef> LoadBatchCardEntityDefs(
    List<string> cardIds,
    out List<string> failedCardIds)
  {
    Dictionary<string, EntityDef> dictionary = new Dictionary<string, EntityDef>(cardIds.Count + 1);
    failedCardIds = new List<string>();
    foreach (string cardId in cardIds)
    {
      EntityDef entityDef = (EntityDef) null;
      List<CardTagDbfRecord> tagDbfRecords = (List<CardTagDbfRecord>) null;
      bool flag;
      if (GameUtils.TryGetCardTagRecords(cardId, out tagDbfRecords))
      {
        entityDef = new EntityDef(tagDbfRecords.Count);
        flag = entityDef.LoadTagFromDBF(cardId, tagDbfRecords);
      }
      else
        flag = false;
      if (!flag)
        failedCardIds.Add(cardId);
      else
        dictionary.Add(cardId, entityDef);
    }
    return dictionary;
  }

  public bool IsValidEntityName() => this.m_cachedEntityName.OverrideCardNameValue == this.GetTag(GAME_TAG.OVERRIDECARDNAME) && this.m_cachedEntityName.CardId == this.m_cardId && !string.IsNullOrEmpty(this.m_cachedEntityName.Name);

  public bool IsValidEntityDebugName() => this.m_cachedEntityDebugName.CardId == this.m_cardId && this.m_cachedEntityDebugName.CardType == this.GetCardType() && !string.IsNullOrEmpty(this.m_cachedEntityName.Name);

  private bool LoadTagFromDBF_SetTags(List<CardTagDbfRecord> tags)
  {
    if (tags == null)
    {
      Debug.LogError((object) string.Format("EntityDef.LoadDataFromCardXml() - No tags found for the card: {0}", (object) this.m_cardId));
      return false;
    }
    foreach (CardTagDbfRecord tag in tags)
    {
      if (tag.IsReferenceTag)
      {
        this.SetReferencedTag(tag.TagId, tag.TagValue);
        if (tag.IsPowerKeywordTag)
          this.SetTag(tag.TagId, tag.TagValue);
      }
      else
        this.SetTag(tag.TagId, tag.TagValue);
    }
    return true;
  }

  private void UpdateEntityName()
  {
    int tag = this.GetTag(GAME_TAG.OVERRIDECARDNAME);
    this.m_cachedEntityName.OverrideCardNameValue = tag;
    this.m_cachedEntityName.CardId = this.m_cardId;
    if (tag > 0)
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
      if (entityDef != null)
      {
        this.m_cachedEntityName.Name = entityDef.GetName();
        return;
      }
    }
    if (this.GetCardTextBuilder() != null)
      this.m_cachedEntityName.Name = this.GetCardTextBuilder().BuildCardName(this);
    else
      this.m_cachedEntityName.Name = CardTextBuilder.GetDefaultCardName(this);
  }

  private void UpdateEntityDebugName()
  {
    string str = (string) null;
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(this.m_cardId);
    if (cardRecord != null && cardRecord.Name != null)
      str = cardRecord.Name.GetString();
    TAG_CARDTYPE cardType = this.GetCardType();
    this.m_cachedEntityDebugName.CardId = this.m_cardId;
    this.m_cachedEntityDebugName.CardType = cardType;
    if (str != null)
      this.m_cachedEntityDebugName.Name = string.Format("[name={0} cardId={1} type={2}]", (object) str, (object) this.m_cardId, (object) cardType);
    else if (this.m_cardId != null)
      this.m_cachedEntityDebugName.Name = string.Format("[cardId={0} type={1}]", (object) this.m_cardId, (object) cardType);
    else
      this.m_cachedEntityDebugName.Name = string.Format("UNKNOWN ENTITY [cardType={0}]", (object) cardType);
  }

  private struct CachedEntityName
  {
    public string Name;
    public int OverrideCardNameValue;
    public string CardId;
  }

  private struct CachedEntityDebugName
  {
    public string Name;
    public string CardId;
    public TAG_CARDTYPE CardType;
  }

  private struct CachedEntityCardId
  {
    public int CardDBId;
    public string CardId;
  }
}
