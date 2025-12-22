using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LettuceMercenary
{
  public int ID;
  public long m_experience;
  public int m_level = 1;
  public bool m_isFullyUpgraded;
  public int m_attack;
  public int m_health;
  public long m_currencyAmount;
  public string m_mercName;
  public string m_mercShortName;
  public bool m_owned;
  public TAG_ROLE m_role;
  public TAG_RARITY m_rarity;
  public TAG_PREMIUM m_premium;
  public TAG_ACQUIRE_TYPE m_acquireType;
  public string m_customAcquireText;
  public bool m_equipmentSelectionChanged;
  public List<string> m_abilitySpecializations = new List<string>();
  public List<LettuceAbility> m_abilityList = new List<LettuceAbility>();
  public List<LettuceAbility> m_equipmentList = new List<LettuceAbility>();
  public List<LettuceMercenary.ArtVariation> m_artVariations = new List<LettuceMercenary.ArtVariation>();
  private LettuceMercenary.Loadout m_loadout = new LettuceMercenary.Loadout();
  public Date m_trainingStartDate;

  public static MercenaryArtVariationDbfRecord GetDefaultArtVariationRecord(
    int mercId)
  {
    LettuceMercenaryDbfRecord record = GameDbf.LettuceMercenary.GetRecord(mercId);
    if (record != null)
    {
      foreach (MercenaryArtVariationDbfRecord mercenaryArtVariation in record.MercenaryArtVariations)
      {
        if (mercenaryArtVariation.DefaultVariation)
          return mercenaryArtVariation;
      }
    }
    return (MercenaryArtVariationDbfRecord) null;
  }

  public static LettuceMercenary.ArtVariation CreateDefaultArtVariation(int mercId) => new LettuceMercenary.ArtVariation(LettuceMercenary.GetDefaultArtVariationRecord(mercId), TAG_PREMIUM.NORMAL, true);

  public LettuceMercenary.ArtVariation GetDefaultOrFirstAvailableArtVariation()
  {
    foreach (LettuceMercenary.ArtVariation artVariation in this.m_artVariations)
    {
      if (artVariation.m_default)
        return artVariation;
    }
    if (this.m_artVariations.Count > 0)
      return this.m_artVariations[0];
    Debug.LogWarning((object) "GetDefaultOrFirstAvailableArtVariation: Unable to find any art variations on this mercenary, generating the default variation as a fallback");
    return LettuceMercenary.CreateDefaultArtVariation(this.ID);
  }

  public LettuceMercenary.ArtVariation GetEquippedArtVariation()
  {
    LettuceMercenary.Loadout currentLoadout = this.GetCurrentLoadout();
    foreach (LettuceMercenary.ArtVariation artVariation in this.m_artVariations)
    {
      if (artVariation.m_record == currentLoadout.m_artVariationRecord && artVariation.m_premium == currentLoadout.m_artVariationPremium)
        return artVariation;
    }
    return this.GetDefaultOrFirstAvailableArtVariation();
  }

  public LettuceMercenary.ArtVariation GetOwnedArtVariation(
    int ArtVariationId,
    TAG_PREMIUM premium)
  {
    LettuceMercenary.ArtVariation ownedArtVariation = (LettuceMercenary.ArtVariation) null;
    foreach (LettuceMercenary.ArtVariation artVariation in this.m_artVariations)
    {
      if (artVariation.m_record.ID == ArtVariationId && artVariation.m_premium == premium)
        ownedArtVariation = artVariation;
    }
    if (ownedArtVariation == null)
      ownedArtVariation = this.GetDefaultOrFirstAvailableArtVariation();
    return ownedArtVariation;
  }

  public void SetEquippedArtVariation(int ArtVariationId, TAG_PREMIUM premium)
  {
    LettuceMercenary.ArtVariation ownedArtVariation = this.GetOwnedArtVariation(ArtVariationId, premium);
    this.m_loadout.SetArtVariation(ownedArtVariation.m_record, premium, true);
    CollectionManager.Get().GetEditingTeam()?.GetLoadout(this)?.SetArtVariation(ownedArtVariation.m_record, premium, true);
  }

  public bool IsArtVariationUnlocked(int ArtVariationId, TAG_PREMIUM premium)
  {
    foreach (LettuceMercenary.ArtVariation artVariation in this.m_artVariations)
    {
      if (artVariation.m_record.ID == ArtVariationId && artVariation.m_premium == premium)
        return true;
    }
    return false;
  }

  public bool IsArtVariationNew(int ArtVariationId, TAG_PREMIUM premium)
  {
    foreach (LettuceMercenary.ArtVariation artVariation in this.m_artVariations)
    {
      if (artVariation.m_record.ID == ArtVariationId && artVariation.m_premium == premium && !artVariation.m_acknowledged)
        return true;
    }
    return false;
  }

  public static List<MercenaryArtVariationDbfRecord> GetArtVariations(
    int mercId)
  {
    return GameDbf.LettuceMercenary.GetRecord(mercId).MercenaryArtVariations;
  }

  public bool HasUnlockedGoldenOrBetter()
  {
    foreach (LettuceMercenary.ArtVariation artVariation in this.m_artVariations)
    {
      if (artVariation.m_premium >= TAG_PREMIUM.GOLDEN)
        return true;
    }
    return false;
  }

  public LettuceMercenary.Loadout GetBaseLoadout() => this.m_loadout;

  public LettuceMercenary.Loadout GetTeamLoadout(LettuceTeam team)
  {
    if (team != null)
    {
      LettuceMercenary.Loadout loadout = team.GetLoadout(this);
      if (loadout != null)
        return loadout;
    }
    return this.GetBaseLoadout();
  }

  public LettuceMercenary.Loadout GetCurrentLoadout() => this.GetTeamLoadout(CollectionManager.Get().GetEditingTeam());

  public CardDbfRecord GetCardRecord() => this.GetEquippedArtVariation().m_record.CardRecord;

  public string GetCardId() => this.GetCardRecord().NoteMiniGuid;

  public CollectibleCard GetCollectibleCard() => CollectionManager.Get().GetCard(this.GetCardId(), TAG_PREMIUM.NORMAL);

  public LettuceAbility GetLettuceAbility(int abilityId)
  {
    int abilityIndex = this.GetAbilityIndex(abilityId);
    if (abilityIndex >= 0)
      return this.m_abilityList[abilityIndex];
    Log.Lettuce.PrintWarning("No ability found on mercenary {0} with ability Id {1}.", (object) this.ID, (object) abilityId);
    return (LettuceAbility) null;
  }

  public LettuceAbility GetLettuceEquipment(int equipmentId)
  {
    int equipmentIndex = this.GetEquipmentIndex(equipmentId);
    if (equipmentIndex >= 0)
      return this.m_equipmentList[equipmentIndex];
    Log.Lettuce.PrintWarning("No equipment found on mercenary {0} with equipment Id {1}.", (object) this.ID, (object) equipmentId);
    return (LettuceAbility) null;
  }

  public LettuceAbility GetLettuceEquipment(string cardId)
  {
    int equipmentIndex = this.GetEquipmentIndex(cardId);
    if (equipmentIndex >= 0)
      return this.m_equipmentList[equipmentIndex];
    Log.Lettuce.PrintWarning("No equipment found on mercenary {0} at equipment index {1}.", (object) this.ID, (object) equipmentIndex);
    return (LettuceAbility) null;
  }

  public int GetAbilityIndex(int abilityDbId) => this.m_abilityList.FindIndex((Predicate<LettuceAbility>) (a => a.ID == abilityDbId));

  public int GetEquipmentIndex(int equipmentDbId) => this.m_equipmentList.FindIndex((Predicate<LettuceAbility>) (e => e.ID == equipmentDbId));

  public int GetEquipmentIndex(string cardId) => this.m_equipmentList.FindIndex((Predicate<LettuceAbility>) (e => e.ContainsCardId(cardId)));

  public bool IsEquipmentSlotUnassigned() => this.GetCurrentLoadout().m_equipmentRecord == null;

  public LettuceAbility GetSlottedEquipment() => this.IsEquipmentSlotUnassigned() ? (LettuceAbility) null : this.m_equipmentList[this.GetEquipmentIndex(this.GetCurrentLoadout().m_equipmentRecord.ID)];

  public bool CanSlotEquipment(int equipmentId)
  {
    LettuceAbility lettuceEquipment = this.GetLettuceEquipment(equipmentId);
    if (lettuceEquipment != null)
      return lettuceEquipment.Owned;
    Log.Lettuce.PrintWarning(string.Format("LettuceMercenary.CanSlotEquipment: Equipment ID {0} is not in Equipment list for Mercenary {1}", (object) equipmentId, (object) this.ID));
    return false;
  }

  public bool SlotEquipment(int equipmentId)
  {
    bool flag = false;
    if (!this.CanSlotEquipment(equipmentId))
      return false;
    LettuceEquipmentDbfRecord record = GameDbf.LettuceEquipment.GetRecord(equipmentId);
    if (this.m_loadout.SetSlottedEquipment(record, true))
    {
      this.m_equipmentSelectionChanged = true;
      flag = true;
    }
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam != null)
    {
      LettuceMercenary.Loadout loadout = editingTeam.GetLoadout(this);
      if (loadout != null && loadout.SetSlottedEquipment(record, true))
      {
        this.m_equipmentSelectionChanged = true;
        flag = true;
      }
    }
    return flag;
  }

  public bool CanUnslotEquipment(int equipmentId)
  {
    if (this.GetLettuceEquipment(equipmentId) == null)
    {
      Log.Lettuce.PrintWarning(string.Format("LettuceMercenary.UnslotEquipment: Equipment ID {0} is not in Equipment list for Mercenary {1}", (object) equipmentId, (object) this.ID));
      return false;
    }
    return this.GetCurrentLoadout().m_equipmentRecord.ID == equipmentId;
  }

  public bool UnslotEquipment(int equipmentId)
  {
    if (!this.CanUnslotEquipment(equipmentId))
      return false;
    this.m_loadout.SetSlottedEquipment((LettuceEquipmentDbfRecord) null, true);
    CollectionManager.Get().GetEditingTeam()?.GetLoadout(this)?.SetSlottedEquipment((LettuceEquipmentDbfRecord) null, true);
    return true;
  }

  public void SetExperience(long experience)
  {
    this.m_experience = experience;
    int level = this.m_level;
    this.m_level = GameUtils.GetMercenaryLevelFromExperience((int) experience);
    this.GetCurrentMercStats(out this.m_attack, out this.m_health);
  }

  public bool IsAcquiredByCrafting() => GameDbf.LettuceMercenary.GetRecord(this.ID).Craftable;

  public int GetCraftingCost() => GameDbf.LettuceMercenary.GetRecord(this.ID).CoinCraftCost;

  public bool IsReadyForCrafting() => this.IsAcquiredByCrafting() && !this.m_owned && this.m_currencyAmount >= (long) this.GetCraftingCost();

  public bool CanAnyAbilityBeUpgraded()
  {
    foreach (LettuceAbility ability in this.m_abilityList)
    {
      if (this.IsLettuceAbilityUpgradeable(ability))
        return true;
    }
    return false;
  }

  public bool CanAnyCardBeUpgraded()
  {
    if (this.CanAnyAbilityBeUpgraded())
      return true;
    foreach (LettuceAbility equipment in this.m_equipmentList)
    {
      if (equipment.Owned && this.IsLettuceAbilityUpgradeable(equipment))
        return true;
    }
    return false;
  }

  public bool IsCardReadyForUpgrade(int abilityId, CollectionUtils.MercenariesModeCardType cardType)
  {
    List<LettuceAbility> lettuceAbilityList = (List<LettuceAbility>) null;
    switch (cardType)
    {
      case CollectionUtils.MercenariesModeCardType.Ability:
        lettuceAbilityList = this.m_abilityList;
        break;
      case CollectionUtils.MercenariesModeCardType.Equipment:
        lettuceAbilityList = this.m_equipmentList;
        break;
    }
    int index = lettuceAbilityList.FindIndex((Predicate<LettuceAbility>) (a => a.ID == abilityId));
    LettuceAbility ability = index >= 0 ? lettuceAbilityList[index] : (LettuceAbility) null;
    if (ability != null)
      return this.IsLettuceAbilityUpgradeable(ability);
    Log.Lettuce.PrintWarning("LettuceMercenary.IsCardReadyForUpgrade - Ability type {0} with ID {1} does not belong to mercenary ID {2}", (object) cardType, (object) abilityId, (object) this.ID);
    return false;
  }

  public bool IsCardReadyForUpgrade(LettuceAbility ability)
  {
    List<LettuceAbility> lettuceAbilityList;
    switch (ability.m_cardType)
    {
      case CollectionUtils.MercenariesModeCardType.Ability:
        lettuceAbilityList = this.m_abilityList;
        break;
      case CollectionUtils.MercenariesModeCardType.Equipment:
        lettuceAbilityList = this.m_equipmentList;
        break;
      default:
        Log.Lettuce.PrintWarning("LettuceMercenary.Unexpected card type: {0}", (object) ability.m_cardType);
        return false;
    }
    if (lettuceAbilityList.Contains(ability))
      return this.IsLettuceAbilityUpgradeable(ability);
    Log.Lettuce.PrintWarning("LettuceMercenary.IsAbilityReadyForUpgrade: Ability ID {0} of type {1} does not belong to Merc ID {2}!", (object) ability.ID, (object) ability.m_cardType, (object) this.ID);
    return false;
  }

  public bool IsMaxLevel() => this.m_level == GameUtils.GetMaxMercenaryLevel();

  public bool IsAbilityLocked(LettuceAbility ability) => ability.m_cardType != CollectionUtils.MercenariesModeCardType.Equipment && this.m_level < ability.m_unlockLevel;

  public bool FindTextInCard(string searchStr)
  {
    if (this.GetCollectibleCard().FindTextInCard(searchStr))
      return true;
    foreach (LettuceAbility ability in this.m_abilityList)
    {
      string cardId = ability.GetCardId();
      if (!string.IsNullOrEmpty(cardId))
      {
        CollectibleCard card = CollectionManager.Get().GetCard(cardId, TAG_PREMIUM.NORMAL);
        if (card != null && card.FindTextInCard(searchStr))
          return true;
      }
    }
    foreach (LettuceAbility equipment in this.m_equipmentList)
    {
      string cardId = equipment.GetCardId();
      if (!string.IsNullOrEmpty(cardId))
      {
        CollectibleCard card = CollectionManager.Get().GetCard(cardId, TAG_PREMIUM.NORMAL);
        if (card != null && card.FindTextInCard(searchStr))
          return true;
      }
    }
    return false;
  }

  private bool IsLettuceAbilityUpgradeable(LettuceAbility ability)
  {
    bool flag;
    switch (ability.m_cardType)
    {
      case CollectionUtils.MercenariesModeCardType.Ability:
        flag = false;
        break;
      case CollectionUtils.MercenariesModeCardType.Equipment:
        flag = true;
        break;
      default:
        Log.Lettuce.PrintWarning("LettuceMercenary.Unexpected card type: {0}", (object) ability.m_cardType);
        return false;
    }
    return this.m_owned && (!flag || ability.Owned) && !this.IsAbilityLocked(ability) && ability.m_tier < ability.GetMaxTier() && this.m_currencyAmount >= (long) ability.GetNextUpgradeCost();
  }

  public void GetCurrentMercStats(out int attack, out int health) => CollectionUtils.GetMercenaryStatsByLevel(this.ID, this.m_level, this.m_isFullyUpgraded, out attack, out health);

  public class ArtVariation
  {
    public readonly MercenaryArtVariationDbfRecord m_record;
    public readonly TAG_PREMIUM m_premium;
    public readonly bool m_default;
    public bool m_acknowledged;

    public ArtVariation(
      MercenaryArtVariationDbfRecord record,
      TAG_PREMIUM premium,
      bool isDefault,
      bool acknowledged = true)
    {
      this.m_record = record;
      this.m_premium = premium;
      this.m_default = isDefault;
      this.m_acknowledged = acknowledged;
    }
  }

  public class Loadout
  {
    public LettuceEquipmentDbfRecord m_equipmentRecord;
    public MercenaryArtVariationDbfRecord m_artVariationRecord;
    public TAG_PREMIUM m_artVariationPremium;
    private bool m_dirty;

    public Loadout()
    {
    }

    public Loadout(LettuceMercenary.Loadout src)
    {
      if (src == null)
        return;
      this.m_equipmentRecord = src.m_equipmentRecord;
      this.m_artVariationRecord = src.m_artVariationRecord;
      this.m_artVariationPremium = src.m_artVariationPremium;
    }

    public void SetArtVariation(
      MercenaryArtVariationDbfRecord record,
      TAG_PREMIUM premium,
      bool markDirty = false)
    {
      if (this.m_artVariationRecord == record && this.m_artVariationPremium == premium)
        return;
      this.m_artVariationRecord = record;
      this.m_artVariationPremium = premium;
      this.m_dirty |= markDirty;
    }

    public bool SetSlottedEquipment(LettuceEquipmentDbfRecord record, bool markDirty = false)
    {
      if (this.m_equipmentRecord == record)
        return false;
      this.m_equipmentRecord = record;
      this.m_dirty |= markDirty;
      return true;
    }

    public bool IsValid() => this.m_artVariationRecord != null;

    public string GetCardId() => this.m_artVariationRecord.CardRecord.NoteMiniGuid;

    public bool IsDirty() => this.m_dirty;

    public void ClearDirty() => this.m_dirty = false;
  }
}
