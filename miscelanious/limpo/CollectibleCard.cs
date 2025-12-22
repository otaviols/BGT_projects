using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CollectibleCard : ICollectible, IComparable
{
  private int m_CardDbId = -1;
  private DateTime m_LatestInsertDate = new DateTime(0L);
  private HashSet<string> m_SearchableTokens;
  private SearchableString m_LongSearchableName;
  private EntityDef m_EntityDef;
  private TAG_PREMIUM m_PremiumType;
  private CardDbfRecord m_CardRecord;
  private string m_CardName;

  public int CardDbId => this.m_CardDbId;

  public string CardId => this.m_EntityDef.GetCardId();

  public string Name => this.m_CardName;

  public string CardInHandText
  {
    get
    {
      CardTextBuilder cardTextBuilder = this.m_EntityDef.GetCardTextBuilder();
      return cardTextBuilder != null ? cardTextBuilder.BuildCardTextInHand(this.m_EntityDef) : CardTextBuilder.GetDefaultCardTextInHand(this.m_EntityDef);
    }
  }

  public string ArtistName => this.m_EntityDef.GetArtistName(TAG_PREMIUM.NORMAL);

  public string SignatureArtistName => this.m_EntityDef.GetArtistName(TAG_PREMIUM.SIGNATURE);

  public int ManaCost => this.m_EntityDef.GetCost();

  public int Attack => this.m_EntityDef.GetATK();

  public int Health => this.m_EntityDef.GetHealth();

  public TAG_CARD_SET Set => this.m_EntityDef.GetCardSet();

  public TAG_CLASS Class => this.m_EntityDef.GetClass();

  public TAG_RARITY Rarity => this.m_EntityDef.GetRarity();

  public List<TAG_RACE> Races => this.m_EntityDef.GetRaces();

  public TAG_CARDTYPE CardType => this.m_EntityDef.GetCardType();

  public bool IsHeroSkin => this.m_EntityDef.IsHeroSkin();

  public bool IsSpell => this.m_EntityDef.IsSpell();

  public TAG_PREMIUM PremiumType => this.m_PremiumType;

  public TAG_ROLE Role => this.m_EntityDef.GetMercenaryRole();

  public bool IsMercenaryAbility => this.m_EntityDef.IsLettuceAbility();

  public RunePattern Runes => this.m_EntityDef.GetRuneCost();

  public int SeenCount { get; set; }

  public int OwnedCount { get; set; }

  public int DisenchantCount => !this.IsCraftable || GameUtils.IsClassicCard(this.m_EntityDef) ? 0 : Mathf.Max(this.OwnedCount - this.DefaultMaxCopiesPerDeck, 0);

  public int IsCraftableDisenchantCount => Mathf.Max(this.OwnedCount - this.DefaultMaxCopiesPerDeck, 0);

  public TAG_SPELL_SCHOOL SpellSchool => this.m_EntityDef.GetSpellSchool();

  public int DefaultMaxCopiesPerDeck => !this.m_EntityDef.IsElite() ? 2 : 1;

  public int CraftBuyCost => CraftingManager.Get().GetCardValue(this.CardId, this.PremiumType).GetBuyValue();

  public bool IsRefundable
  {
    get
    {
      if (!this.IsCraftable)
        return false;
      NetCache.CardValue cardValue = CraftingManager.Get().GetCardValue(this.CardId, this.PremiumType);
      return cardValue != null && cardValue.SellValueOverride != 0 && cardValue.IsOverrideActive();
    }
  }

  public bool IsCraftable
  {
    get
    {
      string cardId = this.CardId;
      return CraftingManager.Get().GetCardValue(cardId, this.PremiumType) != null && !this.IsHeroSkin && FixedRewardsMgr.Get().CanCraftCard(cardId, this.PremiumType) && CraftingUI.IsCraftingEventForCardActive(cardId, this.PremiumType, out bool _);
    }
  }

  public bool IsNewCard => this.OwnedCount > 0 && this.SeenCount < this.OwnedCount && this.SeenCount < this.DefaultMaxCopiesPerDeck;

  public bool IsNewCollectible => this.IsNewCard;

  public int SuggestWeight => this.m_CardRecord.SuggestionWeight;

  public DateTime LatestInsertDate
  {
    set
    {
      if (!(value > this.m_LatestInsertDate))
        return;
      this.m_LatestInsertDate = value;
    }
  }

  public CollectibleCard(CardDbfRecord cardRecord, EntityDef refEntityDef, TAG_PREMIUM premiumType)
  {
    this.m_CardDbId = cardRecord.ID;
    this.m_EntityDef = refEntityDef;
    this.m_PremiumType = premiumType;
    this.m_CardRecord = cardRecord;
    this.m_CardName = CardTextBuilder.GetDefaultCardName(this.m_EntityDef);
  }

  public HashSet<string> GetSearchableTokens()
  {
    if (this.m_SearchableTokens == null)
    {
      this.m_SearchableTokens = new HashSet<string>();
      if (GameUtils.IsLegacySet(this.Set))
      {
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(TAG_CARD_SET.LEGACY, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetName), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetName), this.m_SearchableTokens);
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(TAG_CARD_SET.LEGACY, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetNameShortened), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetNameShortened), this.m_SearchableTokens);
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(TAG_CARD_SET.LEGACY, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetNameInitials), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetNameInitials), this.m_SearchableTokens);
      }
      else
      {
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(this.Set, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetName), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetName), this.m_SearchableTokens);
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(this.Set, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetNameShortened), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetNameShortened), this.m_SearchableTokens);
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(this.Set, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetNameInitials), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetNameInitials), this.m_SearchableTokens);
      }
      if (!this.IsMercenaryAbility)
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_RARITY>(this.Rarity, new Func<TAG_RARITY, bool>(GameStrings.HasRarityText), new Func<TAG_RARITY, string>(GameStrings.GetRarityText), this.m_SearchableTokens);
      foreach (TAG_RACE race in this.Races)
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_RACE>(race, new Func<TAG_RACE, bool>(GameStrings.HasRaceName), new Func<TAG_RACE, string>(GameStrings.GetRaceName), this.m_SearchableTokens);
      CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARDTYPE>(this.CardType, new Func<TAG_CARDTYPE, bool>(GameStrings.HasCardTypeName), new Func<TAG_CARDTYPE, string>(GameStrings.GetCardTypeName), this.m_SearchableTokens);
      CollectibleCardFilter.AddSearchableTokensToSet<TAG_MULTI_CLASS_GROUP>(this.m_EntityDef.GetMultiClassGroup(), new Func<TAG_MULTI_CLASS_GROUP, bool>(GameStrings.HasMultiClassGroupName), new Func<TAG_MULTI_CLASS_GROUP, string>(GameStrings.GetMultiClassGroupName), this.m_SearchableTokens);
      CollectibleCardFilter.AddSearchableTokensToSet<TAG_SPELL_SCHOOL>(this.SpellSchool, new Func<TAG_SPELL_SCHOOL, bool>(GameStrings.HasSpellSchoolName), new Func<TAG_SPELL_SCHOOL, string>(GameStrings.GetSpellSchoolName), this.m_SearchableTokens);
      if (this.m_EntityDef.HasTag(GAME_TAG.MINI_SET))
        CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(this.Set, new Func<TAG_CARD_SET, bool>(GameStrings.HasMiniSetName), new Func<TAG_CARD_SET, string>(GameStrings.GetMiniSetName), this.m_SearchableTokens);
      if (this.m_EntityDef.IsMultiClass())
      {
        List<TAG_CLASS> classes = new List<TAG_CLASS>();
        this.m_EntityDef.GetClasses((IList<TAG_CLASS>) classes);
        foreach (TAG_CLASS tag in classes)
        {
          if (GameStrings.HasClassName(tag))
            CollectibleCardFilter.AddSingleSearchableTokenToSet(GameStrings.GetClassName(tag), this.m_SearchableTokens);
        }
      }
      if (this.m_EntityDef.HasCharge() && GameStrings.HasKeywordName(GAME_TAG.CHARGE))
        CollectibleCardFilter.AddSingleSearchableTokenToSet(GameStrings.GetKeywordName(GAME_TAG.CHARGE), this.m_SearchableTokens);
      if (this.Races.Contains(TAG_RACE.ALL))
      {
        foreach (TAG_RACE structType in Enum.GetValues(typeof (TAG_RACE)))
          CollectibleCardFilter.AddSearchableTokensToSet<TAG_RACE>(structType, new Func<TAG_RACE, bool>(GameStrings.HasRaceName), new Func<TAG_RACE, string>(GameStrings.GetRaceName), this.m_SearchableTokens);
      }
    }
    return this.m_SearchableTokens;
  }

  public SearchableString GetSearchableString()
  {
    if (this.m_LongSearchableName == null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.Name);
      stringBuilder.Append(" ");
      stringBuilder.Append(this.CardInHandText);
      foreach (CardAdditonalSearchTermsDbfRecord searchTerm in this.m_CardRecord.SearchTerms)
      {
        stringBuilder.Append(" ");
        stringBuilder.Append(searchTerm.SearchTerm.GetString());
      }
      this.m_LongSearchableName = new SearchableString(stringBuilder.ToString());
    }
    return this.m_LongSearchableName;
  }

  public bool FindTextInCard(string searchStr) => CollectionUtils.FindTextInCollectible((ICollectible) this, searchStr);

  public void AddCounts(int addOwnedCount, int addSeenCount, DateTime latestInsertDate)
  {
    this.OwnedCount += addOwnedCount;
    this.SeenCount += addSeenCount;
    this.LatestInsertDate = latestInsertDate;
  }

  public void RemoveCounts(int removeOwnedCount) => this.OwnedCount = Mathf.Max(this.OwnedCount - removeOwnedCount, 0);

  public void ClearCounts()
  {
    this.OwnedCount = 0;
    this.SeenCount = 0;
  }

  public EntityDef GetEntityDef() => this.m_EntityDef;

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;
    if (this == obj)
      return true;
    return this.CardDbId == ((CollectibleCard) obj).CardDbId && this.PremiumType == ((CollectibleCard) obj).PremiumType;
  }

  public override int GetHashCode() => (int) (this.CardId.GetHashCode() + this.PremiumType);

  public int CompareTo(object other) => !(other is CollectibleCard collectibleCard) ? -1 : CollectionManager.EntityDefSortComparison(this.m_EntityDef, collectibleCard.m_EntityDef);

  public bool HasCardTag(GAME_TAG tag) => this.m_EntityDef.GetTag(tag) > 0;
}
