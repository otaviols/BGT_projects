using Assets;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusLettuce;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollectionUtils
{
  public static bool IsHeroSkinDisplayMode(CollectionUtils.ViewMode viewMode) => viewMode == CollectionUtils.ViewMode.HERO_SKINS || viewMode == CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS || viewMode == CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS;

  public static void PopulateMercenariesTeamListDataModel(
    LettuceTeamListDataModel dataModel,
    bool setAutoSelectedTeam,
    List<LettuceTeam> teamList = null,
    bool isRemote = false,
    bool showLevelInList = false,
    bool hideInvalidTeams = false)
  {
    if (dataModel == null)
      return;
    DataModelList<LettuceTeamDataModel> dataModelList = new DataModelList<LettuceTeamDataModel>();
    if (teamList == null)
      teamList = CollectionManager.Get().GetTeams();
    CollectionManager.SortTeams(teamList);
    foreach (LettuceTeam team in teamList)
    {
      LettuceTeamDataModel teamModel = new LettuceTeamDataModel();
      if (!hideInvalidTeams || team.IsValid())
      {
        CollectionUtils.PopulateMercenariesTeamDataModel(teamModel, team, isRemote, showLevelInList);
        dataModelList.Add(teamModel);
      }
    }
    dataModel.TeamList = dataModelList;
    if (!setAutoSelectedTeam)
      return;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_LAST_SELECTED_PVP_TEAM, out num);
    dataModel.AutoSelectedTeamId = (int) num;
  }

  public static void PopulateMercenariesTeamDataModel(
    LettuceTeamDataModel teamModel,
    LettuceTeam team,
    bool isRemote = false,
    bool showLevelInList = false)
  {
    teamModel.TeamId = team.ID;
    teamModel.TeamName = team.Name;
    DataModelList<LettuceMercenaryDataModel> dataModelList = new DataModelList<LettuceMercenaryDataModel>();
    foreach (LettuceMercenary merc in team.GetMercs())
      dataModelList.Add(CollectionUtils.GetMercDataModelForTeam(merc, team, isRemote, showLevelInList));
    teamModel.MercenaryList = dataModelList;
    teamModel.Valid = team.IsValid();
    teamModel.IsDisabled = team.DoesContainDisabledMerc();
  }

  private static LettuceMercenaryDataModel GetMercDataModelForTeam(
    LettuceMercenary merc,
    LettuceTeam team,
    bool isRemote,
    bool showLevelInList)
  {
    LettuceMercenary.Loadout loadout = team.GetLoadout(merc);
    LettuceMercenaryDataModel dataModelForTeam = !isRemote ? MercenaryFactory.CreateMercenaryDataModel(merc, new LettuceMercenary.ArtVariation(loadout.m_artVariationRecord, loadout.m_artVariationPremium, loadout.m_artVariationRecord.DefaultVariation)) : MercenaryFactory.CreateMercenaryDataModel(merc.ID, loadout.m_artVariationRecord.ID, loadout.m_artVariationPremium, merc);
    dataModelForTeam.IsRemote = isRemote;
    dataModelForTeam.ShowLevelInList = showLevelInList;
    return dataModelForTeam;
  }

  public static float CalculateXPBarFillAmountFromExp(int exp) => GameUtils.GetExperiencePercentageFromExperienceValue(exp) % 1f;

  public static void PopulateMercenaryDataModel(
    LettuceMercenaryDataModel dataModel,
    LettuceMercenary merc,
    CollectionUtils.MercenaryDataPopluateExtra extraRequests,
    LettuceMercenary.ArtVariation desiredArtVariation = null)
  {
    LettuceEquipmentModifierDataDbfRecord slottedMods = (LettuceEquipmentModifierDataDbfRecord) null;
    dataModel.MercenaryId = merc.ID;
    dataModel.MercenaryName = merc.m_mercName;
    dataModel.MercenaryShortName = string.IsNullOrWhiteSpace(merc.m_mercShortName) ? merc.m_mercName : merc.m_mercShortName;
    dataModel.MercenaryRole = merc.m_role;
    dataModel.MercenaryRarity = merc.m_rarity;
    dataModel.MercenaryLevel = merc.m_level;
    dataModel.IsMaxLevel = merc.m_level >= GameUtils.GetMaxMercenaryLevel();
    dataModel.ReadyForCrafting = merc.IsReadyForCrafting();
    dataModel.ChildUpgradeAvailable = merc.CanAnyCardBeUpgraded();
    dataModel.MercenarySelected = false;
    dataModel.Owned = merc.m_owned;
    dataModel.ExperienceInitial = (int) merc.m_experience;
    dataModel.FullyUpgradedInitial = merc.m_isFullyUpgraded;
    dataModel.CraftingCost = merc.GetCraftingCost();
    dataModel.IsAcquiredByCrafting = merc.IsAcquiredByCrafting();
    dataModel.AcquireType = merc.m_acquireType;
    dataModel.CustomAcquireText = merc.m_customAcquireText;
    dataModel.ShowCustomAcquireText = !string.IsNullOrEmpty(merc.m_customAcquireText);
    dataModel.IsAffectedBySlottedEquipment = false;
    dataModel.EquipmentSlotIndex = -1;
    dataModel.ShowLevelInList = true;
    dataModel.ShowAsNew = CollectionManager.Get().DoesMercenaryNeedToBeAcknowledged(merc);
    dataModel.NumNewPortraits = CollectionManager.Get().GetNumNewPortraitsToAcknowledgeForMercenary(merc);
    dataModel.XPBarPercentage = CollectionUtils.CalculateXPBarFillAmountFromExp((int) merc.m_experience);
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    if (netObject == null)
      Log.Lettuce.PrintError("PopulateMercenaryDataModel - Can't access NetCacheMercenariesPlayerInfo");
    else
      dataModel.IsDisabled = netObject.DisabledMercenaryList.Contains(merc.ID);
    if (desiredArtVariation == null)
      desiredArtVariation = merc.GetEquippedArtVariation();
    CollectionUtils.PopulateMercenaryCardDataModel(dataModel, desiredArtVariation);
    CollectionUtils.SetMercenaryStatsByLevel(dataModel, merc.ID, merc.m_level, merc.m_isFullyUpgraded);
    if (extraRequests.HasFlag((Enum) CollectionUtils.MercenaryDataPopluateExtra.UpdateValuesWithSlottedEquipment))
    {
      LettuceAbility slottedEquipment = merc.GetSlottedEquipment();
      if (slottedEquipment != null)
      {
        slottedMods = slottedEquipment.GetEquipmentModifiers();
        if (slottedMods != null)
        {
          dataModel.IsAffectedBySlottedEquipment = slottedMods.ModifiedLettuceAbilityValues.Count == 0;
          dataModel.Card.Attack += slottedMods.MercenaryAttackChange;
          dataModel.Card.Health += slottedMods.MercenaryHealthChange;
        }
      }
    }
    if (extraRequests.HasFlag((Enum) CollectionUtils.MercenaryDataPopluateExtra.Coin))
      dataModel.MercenaryCoin = new LettuceMercenaryCoinDataModel()
      {
        MercenaryId = merc.ID,
        MercenaryName = merc.m_mercName,
        Quantity = (int) merc.m_currencyAmount,
        GlowActive = false
      };
    if (extraRequests.HasFlag((Enum) CollectionUtils.MercenaryDataPopluateExtra.Appearances))
      CollectionUtils.PopulateMercenaryDataModelArtVariations(dataModel, merc);
    if (!extraRequests.HasFlag((Enum) CollectionUtils.MercenaryDataPopluateExtra.Abilities))
      return;
    CollectionUtils.PopulateMercenaryDataModelAbilities(dataModel, merc, slottedMods);
    if (!extraRequests.HasFlag((Enum) CollectionUtils.MercenaryDataPopluateExtra.UpdateValuesWithSlottedEquipment))
      return;
    LettuceAbility slottedEquipment1 = merc.GetSlottedEquipment();
    if (slottedEquipment1 == null)
      return;
    for (int index = 0; index < dataModel.EquipmentList.Count; ++index)
    {
      if (dataModel.EquipmentList[index].AbilityId == slottedEquipment1.ID)
      {
        dataModel.EquipmentSlotIndex = index;
        break;
      }
    }
  }

  private static void PopulateMercenaryDataModelAbilities(
    LettuceMercenaryDataModel dataModel,
    LettuceMercenary merc,
    LettuceEquipmentModifierDataDbfRecord slottedMods)
  {
    DataModelList<LettuceAbilityDataModel> dataModelList1 = new DataModelList<LettuceAbilityDataModel>();
    foreach (LettuceAbility ability in merc.m_abilityList)
    {
      LettuceAbilityDataModel dataModel1 = new LettuceAbilityDataModel();
      CollectionUtils.PopulateAbilityDataModel(dataModel1, ability, merc, slottedMods);
      dataModelList1.Add(dataModel1);
    }
    dataModel.AbilityList = dataModelList1;
    DataModelList<LettuceAbilityDataModel> dataModelList2 = new DataModelList<LettuceAbilityDataModel>();
    foreach (LettuceAbility equipment in merc.m_equipmentList)
    {
      LettuceAbilityDataModel dataModel2 = new LettuceAbilityDataModel();
      CollectionUtils.PopulateAbilityDataModel(dataModel2, equipment, merc, (LettuceEquipmentModifierDataDbfRecord) null);
      dataModel2.IsEquipment = true;
      dataModelList2.Add(dataModel2);
    }
    if (merc.m_equipmentList.Count == 0)
    {
      for (int index = 0; index < 3; ++index)
        dataModelList2.Add(new LettuceAbilityDataModel()
        {
          IsEquipment = true,
          Owned = false
        });
    }
    dataModel.EquipmentList = dataModelList2;
  }

  private static void PopulateMercenaryDataModelArtVariations(
    LettuceMercenaryDataModel dataModel,
    LettuceMercenary merc)
  {
    dataModel.ArtVariationList = new DataModelList<LettuceMercenaryArtVariationDataModel>();
    foreach (MercenaryArtVariationDbfRecord artVariation in LettuceMercenary.GetArtVariations(merc.ID))
    {
      foreach (MercenaryArtVariationPremiumDbfRecord variationPremium in artVariation.MercenaryArtVariationPremiums)
      {
        if (variationPremium.Collectible)
        {
          TAG_PREMIUM tagPremium = TAG_PREMIUM.NORMAL;
          switch (variationPremium.Premium)
          {
            case MercenaryArtVariationPremium.MercenariesPremium.PREMIUM_GOLDEN:
              tagPremium = TAG_PREMIUM.GOLDEN;
              break;
            case MercenaryArtVariationPremium.MercenariesPremium.PREMIUM_DIAMOND:
              tagPremium = TAG_PREMIUM.DIAMOND;
              break;
          }
          bool flag = merc.IsArtVariationUnlocked(artVariation.ID, tagPremium);
          string str = (string) null;
          if (!flag)
          {
            MercenaryUnlock artVariationUnlock = GameDbf.GetIndex().GetArtVariationUnlock(artVariation.ID, tagPremium);
            switch (artVariationUnlock.m_unlockType)
            {
              case MercenaryUnlock.UnlockType.Packs:
                str = GameStrings.Get("GLUE_LETTUCE_VANITY_LOCKEDPLATE_MESSAGE");
                break;
              case MercenaryUnlock.UnlockType.VisitorTask:
                str = GameStrings.Format("GLUE_LETTUCE_VANITY_LOCKEDPLATE_MESSAGE_TASK", (object) GameStrings.Format(artVariationUnlock.m_visitorTask.TaskTitle.GetString()));
                break;
              case MercenaryUnlock.UnlockType.RewardTrack:
                str = GameStrings.Get("GLUE_LETTUCE_VANITY_LOCKEDPLATE_MESSAGE_REWARD_TRACK");
                break;
              case MercenaryUnlock.UnlockType.Custom:
                str = artVariationUnlock.m_customAcquireText;
                break;
            }
          }
          dataModel.ArtVariationList.Add(new LettuceMercenaryArtVariationDataModel()
          {
            Card = new CardDataModel()
            {
              CardId = artVariation.CardRecord.NoteMiniGuid,
              Premium = tagPremium,
              FlavorText = (string) artVariation.CardRecord.FlavorText
            },
            ArtVariationId = artVariation.ID,
            Unlocked = flag,
            Selected = dataModel.Card.CardId == artVariation.CardRecord.NoteMiniGuid && dataModel.Card.Premium == tagPremium,
            LockedText = str,
            NewlyUnlocked = merc.IsArtVariationNew(artVariation.ID, tagPremium)
          });
        }
      }
    }
    dataModel.ArtVariationList.Sort((Comparison<LettuceMercenaryArtVariationDataModel>) ((a, b) =>
    {
      if (a.Card.Premium != b.Card.Premium)
        return a.Card.Premium <= b.Card.Premium ? 1 : -1;
      if (a.ArtVariationId == b.ArtVariationId)
        return 0;
      return a.ArtVariationId <= b.ArtVariationId ? 1 : -1;
    }));
    dataModel.ArtVariationPageList = new DataModelList<LettuceMercenaryArtVariationPageDataModel>();
    for (int count = dataModel.ArtVariationList.Count; count > 0; count -= 4)
      dataModel.ArtVariationPageList.Add(new LettuceMercenaryArtVariationPageDataModel()
      {
        ArtVatiationsOnPageCount = Mathf.Min(count, 4),
        ShowLeftArrow = true,
        ShowRightArrow = true
      });
    dataModel.ArtVariationPageIndex = 0;
    if (dataModel.ArtVariationPageList.Count <= 0)
      return;
    LettuceMercenaryArtVariationPageDataModel artVariationPage = dataModel.ArtVariationPageList[0];
    artVariationPage.ShowLeftArrow = false;
    artVariationPage.ShowRightArrow = dataModel.ArtVariationPageList.Count > 1;
    dataModel.ArtVariationPageList[dataModel.ArtVariationPageList.Count - 1].ShowRightArrow = false;
  }

  public static int GetFirstOwnedEquipmentIndex(LettuceMercenaryDataModel mercData)
  {
    for (int index = 0; index < mercData.EquipmentList.Count; ++index)
    {
      if (mercData.EquipmentList[index].Owned)
        return index;
    }
    return -1;
  }

  public static void PopulateMercenaryCardDataModel(
    LettuceMercenaryDataModel dataModel,
    LettuceMercenary.ArtVariation artVariation)
  {
    if (dataModel == null)
    {
      Log.Lettuce.PrintError("PopulateMercenaryCardDataModel - Data model was null");
    }
    else
    {
      if (dataModel.Card == null)
        dataModel.Card = new CardDataModel();
      if (artVariation != null)
      {
        CardDbfRecord cardRecord = artVariation.m_record.CardRecord;
        string noteMiniGuid = cardRecord.NoteMiniGuid;
        dataModel.Card.CardId = noteMiniGuid;
        dataModel.Card.Premium = artVariation.m_premium;
        dataModel.Card.FlavorText = (string) cardRecord.FlavorText;
      }
      else
        Log.Lettuce.PrintError("PopulateMercenaryCardDataModel - art variation was null");
      dataModel.HideXp = false;
      dataModel.HideWatermark = true;
      dataModel.HideStats = false;
      dataModel.Label = string.Empty;
    }
  }

  public static void SetMercenaryStatsByLevel(
    LettuceMercenaryDataModel dataModel,
    int mercenaryId,
    int level,
    bool isFullyUpgraded)
  {
    int attack;
    int health;
    CollectionUtils.GetMercenaryStatsByLevel(mercenaryId, level, isFullyUpgraded, out attack, out health);
    dataModel.Card.Attack = attack;
    dataModel.Card.Health = health;
  }

  public static void GetMercenaryStatsByLevel(
    int mercenaryId,
    int level,
    bool isFullyUpgraded,
    out int attack,
    out int health)
  {
    attack = 0;
    health = 0;
    bool isMaxLevel;
    LettuceMercenaryLevelStatsDbfRecord mercenaryStatsByLevel = GameUtils.GetMercenaryStatsByLevel(mercenaryId, level, out isMaxLevel);
    if (mercenaryStatsByLevel == null)
      return;
    attack = mercenaryStatsByLevel.Attack;
    health = mercenaryStatsByLevel.Health;
    if (!(isFullyUpgraded & isMaxLevel))
      return;
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    attack += netObject.Mercenaries.FullyUpgradedStatBoostAttack;
    health += netObject.Mercenaries.FullyUpgradedStatBoostHealth;
  }

  public static void PopulateAbilityTierDataModel(
    LettuceAbilityTierDataModel dataModel,
    LettuceAbility.AbilityTier abilityTier,
    LettuceMercenary parentMerc,
    int parentAbilityId,
    ModifiedLettuceAbilityValueDbfRecord abilityMods)
  {
    dataModel.Tier = abilityTier.m_tier;
    dataModel.ParentAbilityId = parentAbilityId;
    dataModel.ValidTier = abilityTier.m_validTier;
    if (!abilityTier.m_validTier)
      return;
    dataModel.CoinCraftCost = string.Format("{0}", (object) abilityTier.m_coinCost);
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(abilityTier.m_cardId);
    if (cardRecord == null)
    {
      Log.Lettuce.PrintWarning("CollectionUtils.PopulateLettuceAbilityTierDataModel - unable to load card record for cardID = {0}", (object) abilityTier.m_cardId);
    }
    else
    {
      dataModel.AbilityName = (string) cardRecord.Name;
      dataModel.AbilityTierCard = new CardDataModel()
      {
        CardId = abilityTier.m_cardId,
        Premium = TAG_PREMIUM.NORMAL,
        FlavorText = (string) cardRecord.FlavorText
      };
      EntityDef entityDef = DefLoader.Get().GetEntityDef(abilityTier.m_cardId);
      if (entityDef == null)
      {
        Log.Lettuce.PrintWarning("CollectionUtils.PopulateLettuceAbilityTierDataModel - unable to load entity for cardID = {0}", (object) abilityTier.m_cardId);
      }
      else
      {
        (bool valid, int attack, int health) summonedMinionStats = entityDef.GetSummonedMinionStats();
        if (!summonedMinionStats.valid)
        {
          summonedMinionStats.attack = entityDef.GetATK();
          summonedMinionStats.health = entityDef.GetHealth();
        }
        if (abilityMods != null)
        {
          dataModel.AbilityTierCard.Attack = summonedMinionStats.attack + abilityMods.AttackChange;
          dataModel.AbilityTierCard.Health = summonedMinionStats.health + abilityMods.HealthChange;
          dataModel.AbilityTierCard.Mana = entityDef.GetTag(GAME_TAG.COST) + abilityMods.SpeedChange;
          dataModel.AbilityTierCard.Cooldown = entityDef.GetTag(GAME_TAG.LETTUCE_COOLDOWN_CONFIG) + abilityMods.CooldownChange;
          if (abilityMods.ScriptDataNum1Change != 0)
            dataModel.AbilityTierCard.GameTagOverrides.Add(new GameTagValueDataModel()
            {
              GameTag = GAME_TAG.TAG_SCRIPT_DATA_NUM_1,
              Value = entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) + abilityMods.ScriptDataNum1Change
            });
          if (abilityMods.ScriptDataNum2Change != 0)
            dataModel.AbilityTierCard.GameTagOverrides.Add(new GameTagValueDataModel()
            {
              GameTag = GAME_TAG.TAG_SCRIPT_DATA_NUM_2,
              Value = entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2) + abilityMods.ScriptDataNum2Change
            });
          if (abilityMods.Tags != null && abilityMods.Tags.Count > 0)
          {
            foreach (ModifiedLettuceAbilityCardTagDbfRecord tag in abilityMods.Tags)
              dataModel.AbilityTierCard.GameTagOverrides.Add(new GameTagValueDataModel()
              {
                GameTag = (GAME_TAG) tag.TagId,
                Value = tag.TagValue,
                IsReferenceValue = tag.IsReferenceTag,
                IsPowerKeywordTag = tag.IsPowerKeywordTag
              });
          }
          LettuceAbility slottedEquipment = parentMerc.GetSlottedEquipment();
          if (slottedEquipment == null)
            return;
          foreach (CardEquipmentAltTextDbfRecord altTextDbfRecord in cardRecord.EquipmentAltText)
          {
            if (altTextDbfRecord.AltTextIndex != 0 && altTextDbfRecord.EquipmentCardRecord.NoteMiniGuid == slottedEquipment.GetCardId())
            {
              dataModel.AbilityTierCard.GameTagOverrides.Add(new GameTagValueDataModel()
              {
                GameTag = GAME_TAG.USE_ALTERNATE_CARD_TEXT,
                Value = altTextDbfRecord.AltTextIndex
              });
              break;
            }
          }
        }
        else
        {
          dataModel.AbilityTierCard.Attack = summonedMinionStats.attack;
          dataModel.AbilityTierCard.Health = summonedMinionStats.health;
          dataModel.AbilityTierCard.Mana = entityDef.GetTag(GAME_TAG.COST);
          dataModel.AbilityTierCard.Cooldown = entityDef.GetTag(GAME_TAG.LETTUCE_COOLDOWN_CONFIG);
        }
      }
    }
  }

  private static void FillInAbilityTierData(
    LettuceAbilityDataModel dataModel,
    LettuceAbility ability,
    LettuceMercenary parentMerc,
    ModifiedLettuceAbilityValueDbfRecord abilityMods)
  {
    DataModelList<LettuceAbilityTierDataModel> dataModelList = new DataModelList<LettuceAbilityTierDataModel>();
    if (ability != null)
    {
      foreach (LettuceAbility.AbilityTier tier in ability.m_tierList)
      {
        LettuceAbilityTierDataModel dataModel1 = new LettuceAbilityTierDataModel();
        CollectionUtils.PopulateAbilityTierDataModel(dataModel1, tier, parentMerc, ability.ID, abilityMods);
        dataModelList.Add(dataModel1);
        dataModel.MaxTier = Mathf.Max(dataModel.MaxTier, dataModel1.Tier);
      }
    }
    dataModel.AbilityTiers = dataModelList;
  }

  public static void PopulateDefaultAbilityDataModelWithTier(
    LettuceAbilityDataModel dataModel,
    LettuceAbility ability,
    LettuceMercenary parentMerc,
    int desiredTier = 1)
  {
    if (ability == null)
    {
      Debug.LogErrorFormat("PopulateDefaultAbilityDataModelWithTier has null ability param.  parentMerc = {0}", parentMerc != null ? (object) parentMerc.m_mercName : (object) "None");
    }
    else
    {
      dataModel.AbilityId = ability.ID;
      dataModel.AbilityName = ability.m_abilityName;
    }
    dataModel.CurrentTier = desiredTier;
    dataModel.ParentMercId = parentMerc.ID;
    dataModel.AbilityRole = parentMerc.m_role;
    dataModel.ReadyForUpgrade = false;
    if (ability != null && ability.m_cardType == CollectionUtils.MercenariesModeCardType.Equipment)
    {
      dataModel.IsEquipment = true;
      dataModel.IsEquipped = false;
      dataModel.Owned = true;
    }
    CollectionUtils.FillInAbilityTierData(dataModel, ability, parentMerc, (ModifiedLettuceAbilityValueDbfRecord) null);
  }

  public static void PopulateAbilityDataModel(
    LettuceAbilityDataModel dataModel,
    LettuceAbility ability,
    LettuceMercenary parentMerc,
    LettuceEquipmentModifierDataDbfRecord slottedMods)
  {
    if (ability == null)
    {
      Debug.LogErrorFormat("PopulateAbilityDataModel has null ability param.  parentMerc = {0}", parentMerc != null ? (object) parentMerc.m_mercName : (object) "None");
    }
    else
    {
      dataModel.AbilityId = ability.ID;
      dataModel.AbilityName = ability.m_abilityName;
      dataModel.CurrentTier = ability.m_tier;
    }
    dataModel.ParentMercId = parentMerc.ID;
    dataModel.AbilityRole = parentMerc.m_role;
    dataModel.ReadyForUpgrade = parentMerc.IsCardReadyForUpgrade(ability);
    LettuceMercenary.Loadout currentLoadout = parentMerc.GetCurrentLoadout();
    ModifiedLettuceAbilityValueDbfRecord abilityMods = (ModifiedLettuceAbilityValueDbfRecord) null;
    if (ability != null)
    {
      dataModel.IsNew = !ability.IsAcknowledged(parentMerc);
      if (ability.m_cardType == CollectionUtils.MercenariesModeCardType.Ability)
      {
        dataModel.UnlockLevel = ability.m_unlockLevel;
        dataModel.LockPlateText = GameStrings.Format("GLUE_LETTUCE_ABILITY_REACH_LEVEL", (object) ability.m_unlockLevel);
        if (slottedMods != null)
        {
          foreach (ModifiedLettuceAbilityValueDbfRecord lettuceAbilityValue in slottedMods.ModifiedLettuceAbilityValues)
          {
            if (ability.ID == lettuceAbilityValue.LettuceAbilityId)
            {
              abilityMods = lettuceAbilityValue;
              dataModel.IsAffectedBySlottedEquipment = true;
              break;
            }
          }
        }
      }
      else
      {
        dataModel.IsEquipment = true;
        dataModel.IsEquipped = currentLoadout.m_equipmentRecord != null && currentLoadout.m_equipmentRecord.ID == ability.ID;
        dataModel.Owned = ability.Owned;
        if (!ability.Owned)
        {
          MercenaryUnlock unlockFromEquipmentId = GameDbf.GetIndex().GetEquipmentUnlockFromEquipmentID(ability.ID);
          if (unlockFromEquipmentId != null)
          {
            switch (unlockFromEquipmentId.m_unlockType)
            {
              case MercenaryUnlock.UnlockType.VisitorTask:
                dataModel.LockPlateText = GameStrings.Format("GLUE_LETTUCE_COLLECTION_EQUIPMENT_UNLOCK_DESC", (object) (unlockFromEquipmentId.m_visitorTaskIndex + 1));
                break;
              case MercenaryUnlock.UnlockType.Achievement:
                AchievementDataModel achievementDataModel = AchievementManager.Get().GetAchievementDataModel(unlockFromEquipmentId.m_achievement.ID);
                if (achievementDataModel.Progress >= achievementDataModel.Quota)
                {
                  dataModel.LockPlateText = GameStrings.Get("GLUE_LETTUCE_COLLECTION_EQUIPMENT_REDEEM_ACHIEVEMENT");
                  break;
                }
                string description = achievementDataModel.Description;
                if (achievementDataModel.Quota > 1)
                {
                  string str = ProgressUtils.FormatDescription(description, achievementDataModel.Quota);
                  dataModel.LockPlateText = str + "\n" + (object) achievementDataModel.Progress + "/" + (object) achievementDataModel.Quota;
                  break;
                }
                dataModel.LockPlateText = description;
                break;
              case MercenaryUnlock.UnlockType.Bounty:
                dataModel.LockPlateText = GameStrings.Format("GLUE_LETTUCE_COLLECTION_EQUIPMENT_UNLOCK_BY_BOUNTY_DESC", (object) LettuceVillageDataUtil.GenerateBountyName(unlockFromEquipmentId.m_bounty));
                break;
              default:
                Log.CollectionManager.PrintError("CollectionManager_Lettuce.RegisterMercenary(): Ability [{0}] missing required unlock info!", (object) ability.m_abilityName);
                break;
            }
          }
        }
      }
    }
    CollectionUtils.FillInAbilityTierData(dataModel, ability, parentMerc, abilityMods);
  }

  public static void PopulateTeamPreviewData(
    LettuceTeamDataModel dataModel,
    LettuceTeam team,
    List<int> deadMercs,
    bool populateCards,
    bool isRemote = false)
  {
    dataModel.MercenaryList = new DataModelList<LettuceMercenaryDataModel>();
    if (team == null)
      return;
    foreach (LettuceMercenary merc in team.GetMercs())
    {
      if (merc == null)
      {
        Log.CollectionManager.PrintError(string.Format("CollectionManager_Lettuce.PopulateTeamPreviewData(): There was an error displaying a mercenary for team {0}", (object) team.ID));
      }
      else
      {
        LettuceMercenaryDataModel dataModelForTeam = CollectionUtils.GetMercDataModelForTeam(merc, team, isRemote, true);
        if (populateCards)
        {
          dataModelForTeam.ExperienceInitial = (int) merc.m_experience;
          dataModelForTeam.ExperienceFinal = (int) merc.m_experience;
          dataModelForTeam.FullyUpgradedInitial = merc.m_isFullyUpgraded;
          dataModelForTeam.FullyUpgradedFinal = merc.m_isFullyUpgraded;
        }
        if (deadMercs != null)
          dataModelForTeam.DeadInMapRun = deadMercs.Contains(merc.ID);
        dataModel.MercenaryList.Add(dataModelForTeam);
      }
    }
  }

  public static void PopulateTeamTreasures(
    LettuceTeamDataModel dataModel,
    List<LettuceMapTreasureAssignment> treasureList)
  {
    if (treasureList == null)
      return;
    foreach (LettuceMercenaryDataModel mercenary in dataModel.MercenaryList)
    {
      LettuceMercenaryDataModel merc = mercenary;
      LettuceMapTreasureAssignment treasureAssignment = treasureList.FirstOrDefault<LettuceMapTreasureAssignment>((Func<LettuceMapTreasureAssignment, bool>) (e => e.AssignedMercenary == merc.MercenaryId));
      if (treasureAssignment != null)
        merc.TreasureCard = new CardDataModel()
        {
          CardId = GameUtils.TranslateDbIdToCardId(treasureAssignment.TreasureCard)
        };
    }
  }

  public static void UpdateMercenaryCardStats(
    LettuceMercenaryDataModel dataModel,
    LettuceMercenary merc)
  {
    CollectionUtils.SetMercenaryStatsByLevel(dataModel, merc.ID, merc.m_level, merc.m_isFullyUpgraded);
    LettuceAbility slottedEquipment = merc.GetSlottedEquipment();
    if (slottedEquipment == null)
      return;
    LettuceEquipmentModifierDataDbfRecord equipmentModifiers = slottedEquipment.GetEquipmentModifiers();
    if (equipmentModifiers == null)
      return;
    dataModel.IsAffectedBySlottedEquipment = equipmentModifiers.ModifiedLettuceAbilityValues.Count == 0;
    dataModel.Card.Attack += equipmentModifiers.MercenaryAttackChange;
    dataModel.Card.Health += equipmentModifiers.MercenaryHealthChange;
  }

  public static void UpdateAbilityAffectedBySlottedEquipment(
    LettuceMercenaryDataModel dataModel,
    LettuceMercenary merc)
  {
    LettuceAbility slottedEquipment = merc.GetSlottedEquipment();
    if (slottedEquipment == null)
      return;
    LettuceEquipmentModifierDataDbfRecord equipmentModifiers = slottedEquipment.GetEquipmentModifiers();
    if (equipmentModifiers == null)
      return;
    foreach (LettuceAbilityDataModel ability in dataModel.AbilityList)
    {
      if (ability.IsAffectedBySlottedEquipment)
      {
        foreach (ModifiedLettuceAbilityValueDbfRecord lettuceAbilityValue in equipmentModifiers.ModifiedLettuceAbilityValues)
        {
          if (ability.AbilityId == lettuceAbilityValue.LettuceAbilityId)
          {
            LettuceAbility lettuceAbility = merc.GetLettuceAbility(ability.AbilityId);
            CollectionUtils.FillInAbilityTierData(ability, lettuceAbility, merc, lettuceAbilityValue);
          }
        }
      }
    }
  }

  public static void UpdateReadyForUpgradeStatus(
    LettuceMercenaryDataModel dataModel,
    LettuceMercenary merc)
  {
    dataModel.ReadyForCrafting = merc.IsReadyForCrafting();
    bool flag = false;
    foreach (LettuceAbilityDataModel ability in dataModel.AbilityList)
    {
      ability.ReadyForUpgrade = merc.IsCardReadyForUpgrade(ability.AbilityId, CollectionUtils.MercenariesModeCardType.Ability);
      flag = ability.ReadyForUpgrade || flag;
    }
    foreach (LettuceAbilityDataModel equipment in dataModel.EquipmentList)
    {
      equipment.ReadyForUpgrade = merc.IsCardReadyForUpgrade(equipment.AbilityId, CollectionUtils.MercenariesModeCardType.Equipment);
      flag = equipment.ReadyForUpgrade || flag;
    }
    dataModel.ChildUpgradeAvailable = flag;
  }

  public static bool FindTextInCollectible(ICollectible collectible, string searchText)
  {
    searchText = searchText.Trim();
    return collectible.GetSearchableTokens().Contains(searchText) || collectible.GetSearchableString().Search(searchText);
  }

  public enum ViewMode
  {
    CARDS,
    HERO_SKINS,
    CARD_BACKS,
    DECK_TEMPLATE,
    MASS_DISENCHANT,
    COINS,
    BATTLEGROUNDS_GUIDE_SKINS,
    BATTLEGROUNDS_HERO_SKINS,
    HERO_PICKER,
    BATTLEGROUNDS_BOARD_SKINS,
    BATTLEGROUNDS_FINISHERS,
    BATTLEGROUNDS_EMOTES,
    COUNT,
  }

  public enum BattlegroundsHeroSkinFilterMode
  {
    DEFAULT,
    ALL,
    COUNT,
  }

  public enum MercenariesModeCardType
  {
    None,
    Mercenary,
    Ability,
    Equipment,
  }

  public enum BattlegroundsModeDraggableType
  {
    None,
    CollectionEmote,
    TrayEmote,
  }

  public class ViewModeData
  {
    public TAG_CLASS? m_setPageByClass;
    public TAG_ROLE? m_setPageByRole;
    public string m_setPageByCard;
    public TAG_PREMIUM m_setPageByPremium;
    public BookPageManager.DelOnPageTransitionComplete m_pageTransitionCompleteCallback;
    public object m_pageTransitionCompleteData;
  }

  [Serializable]
  public class CollectionPageLayoutSettings
  {
    [CustomEditField(ListTable = true)]
    public List<CollectionUtils.CollectionPageLayoutSettings.Variables> m_layoutVariables = new List<CollectionUtils.CollectionPageLayoutSettings.Variables>();

    public CollectionUtils.CollectionPageLayoutSettings.Variables GetVariables(
      CollectionUtils.ViewMode mode)
    {
      return this.m_layoutVariables.Find((Predicate<CollectionUtils.CollectionPageLayoutSettings.Variables>) (v => mode == v.m_ViewMode)) ?? new CollectionUtils.CollectionPageLayoutSettings.Variables();
    }

    [Serializable]
    public class Variables
    {
      public CollectionUtils.ViewMode m_ViewMode;
      public int m_ColumnCount = 4;
      public int m_RowCount = 2;
      public float m_Scale;
      public float m_ColumnSpacing;
      public float m_RowSpacing;
      public Vector3 m_Offset;
    }
  }

  [Flags]
  public enum MercenaryDataPopluateExtra
  {
    None = 0,
    Abilities = 1,
    Coin = 2,
    Appearances = 8,
    UpdateValuesWithSlottedEquipment = 16, // 0x00000010
  }
}
