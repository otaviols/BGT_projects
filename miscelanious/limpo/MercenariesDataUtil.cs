using Hearthstone.DataModels;
using UnityEngine;

public static class MercenariesDataUtil
{
  public static MercenariesDataUtil.MercenariesBountyLockedReason GetBountyUnlockStatus(
    int bountyRecordID)
  {
    return MercenariesDataUtil.GetBountyUnlockStatus(GameDbf.LettuceBounty.GetRecord(bountyRecordID));
  }

  public static MercenariesDataUtil.MercenariesBountyLockedReason GetBountyUnlockStatus(
    LettuceBountyDbfRecord bountyRecord)
  {
    if (bountyRecord == null || !bountyRecord.Enabled)
      return MercenariesDataUtil.MercenariesBountyLockedReason.INVALID;
    if (bountyRecord.RequiredCompletedBounty > 0 && !MercenariesDataUtil.IsBountyComplete(bountyRecord.RequiredCompletedBounty))
      return MercenariesDataUtil.MercenariesBountyLockedReason.PREVIOUS_ZONES_INCOMPLETE;
    SpecialEventType eventType = bountyRecord.Event;
    int num = SpecialEventManager.Get().IsEventActive(eventType, false) ? 1 : 0;
    SpecialEventType availableAfterEvent = bountyRecord.AvailableAfterEvent;
    bool flag = SpecialEventManager.Get().HasEventEnded(availableAfterEvent);
    if (num != 0 || flag)
      return MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED;
    return eventType != SpecialEventType.SPECIAL_EVENT_NEVER && eventType != SpecialEventType.UNKNOWN ? MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_ACTIVE : MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_COMPLETE;
  }

  public static MercenariesDataUtil.MercenariesBountyLockedReason GetBountySetUnlockStatus(
    LettuceBountySetDbfRecord bountySetRecord)
  {
    if (bountySetRecord == null)
      return MercenariesDataUtil.MercenariesBountyLockedReason.INVALID;
    if (bountySetRecord.RequiredCompletedBounty > 0 && !MercenariesDataUtil.IsBountyComplete(bountySetRecord.RequiredCompletedBounty))
      return MercenariesDataUtil.MercenariesBountyLockedReason.PREVIOUS_ZONES_INCOMPLETE;
    SpecialEventType eventType = bountySetRecord.Event;
    int num = SpecialEventManager.Get().IsEventActive(eventType, false) ? 1 : 0;
    SpecialEventType availableAfterEvent = bountySetRecord.AvailableAfterEvent;
    bool flag = SpecialEventManager.Get().HasEventEnded(availableAfterEvent);
    if (num != 0 || flag)
      return MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED;
    return eventType != SpecialEventType.SPECIAL_EVENT_NEVER && eventType != SpecialEventType.UNKNOWN ? MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_ACTIVE : MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_COMPLETE;
  }

  public static bool IsBountyComplete(
    int bountyId,
    NetCache.NetCacheMercenariesPlayerInfo mercenariesPlayerInfo = null)
  {
    if (mercenariesPlayerInfo == null)
      mercenariesPlayerInfo = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    if (mercenariesPlayerInfo == null || mercenariesPlayerInfo.BountyInfoMap == null || !mercenariesPlayerInfo.BountyInfoMap.ContainsKey(bountyId) || mercenariesPlayerInfo.BountyInfoMap[bountyId] == null)
      return false;
    return mercenariesPlayerInfo.BountyInfoMap[bountyId].IsComplete || mercenariesPlayerInfo.BountyInfoMap[bountyId].Completions > 0;
  }

  public static bool IsAbilityorEquipmentAvailableToUse(
    LettuceAbility ability,
    LettuceMercenary merc)
  {
    return ability.m_cardType != CollectionUtils.MercenariesModeCardType.Equipment ? !merc.IsAbilityLocked(ability) : ability.Owned;
  }

  public static void UpdateMercenaryDataModelWithNewData(
    LettuceMercenaryDataModel mercData,
    LettuceAbility ability,
    LettuceMercenary merc)
  {
    if (mercData == null || ability == null || merc == null)
      Log.Lettuce.PrintWarning("MercenaryDetailDisplay.UpdateMercenaryDataModelWithNewData - Invalid null parameter");
    else if (mercData.MercenaryId != merc.ID)
    {
      Log.Lettuce.PrintWarning("MercenaryDetailDisplay.UpdateDataModelsAfterTransaction - " + string.Format("Mercenary display data model merc Id {0} does not match response merc ID {1}", (object) mercData.MercenaryId, (object) merc.ID));
    }
    else
    {
      if (ability.m_cardType == CollectionUtils.MercenariesModeCardType.Equipment && !ability.Owned)
      {
        ability.Owned = true;
        ability.m_tier = ability.GetBaseTier();
      }
      else
        ability.m_tier = ability.GetNextTier();
      foreach (LettuceAbilityDataModel abilityDataModel in ability.m_cardType == CollectionUtils.MercenariesModeCardType.Ability ? mercData.AbilityList : mercData.EquipmentList)
      {
        if (abilityDataModel.AbilityId == ability.ID)
        {
          abilityDataModel.CurrentTier = ability.m_tier;
          abilityDataModel.Owned = ability.Owned;
          abilityDataModel.IsNew = !ability.IsAcknowledged(merc);
        }
      }
      CollectionUtils.UpdateReadyForUpgradeStatus(mercData, merc);
      CollectionUtils.UpdateMercenaryCardStats(mercData, merc);
      mercData.ChildUpgradeAvailable = false;
      LettuceAbility slottedEquipment = merc.GetSlottedEquipment();
      if (slottedEquipment == null || ability == null || slottedEquipment.ID != ability.ID)
        return;
      CollectionUtils.UpdateAbilityAffectedBySlottedEquipment(mercData, merc);
    }
  }

  public static void UpdateMercenaryDataModelNewStatus(LettuceMercenaryDataModel mercData)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return;
    LettuceMercenary mercenary = collectionManager.GetMercenary((long) mercData.MercenaryId);
    if (mercenary == null)
      return;
    bool beAcknowledged = collectionManager.DoesMercenaryNeedToBeAcknowledged(mercenary);
    CollectibleDisplay collectibleDisplay = collectionManager.GetCollectibleDisplay();
    if (!((Object) collectibleDisplay != (Object) null))
      return;
    LettuceCollectionPageManager pageManager = collectibleDisplay.GetPageManager() as LettuceCollectionPageManager;
    if (!((Object) pageManager != (Object) null))
      return;
    pageManager.UpdateAcknowledgedStatusForPageMercenary(mercenary.ID, beAcknowledged);
  }

  public enum MercenariesBountyLockedReason
  {
    INVALID = -1, // 0xFFFFFFFF
    UNLOCKED = 0,
    COMING_SOON = 1,
    EVENT_NOT_STARTED = 2,
    EVENT_NOT_ACTIVE = 3,
    EVENT_ENDED = 4,
    PVE_BUILDING_NEEDS_UPGRADE = 5,
    PREVIOUS_ZONES_INCOMPLETE = 6,
    CURRENT_BOUNTY_UNFINISHED = 7,
    EVENT_NOT_COMPLETE = 8,
  }
}
