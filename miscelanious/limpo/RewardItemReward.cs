using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemReward : Reward
{
  private static readonly HashSet<RewardItemType> s_battlegroundsRewards = new HashSet<RewardItemType>()
  {
    RewardItemType.BATTLEGROUNDS_GUIDE_SKIN,
    RewardItemType.BATTLEGROUNDS_HERO_SKIN
  };
  private static readonly HashSet<RewardItemType> s_mercenariesRewards = new HashSet<RewardItemType>()
  {
    RewardItemType.MERCENARY,
    RewardItemType.MERCENARY_COIN,
    RewardItemType.MERCENARY_BOOSTER,
    RewardItemType.MERCENARY_RANDOM_MERCENARY,
    RewardItemType.MERCENARY_EQUIPMENT,
    RewardItemType.MERCENARY_EQUIPMENT_ICON,
    RewardItemType.MERCENARY_XP
  };
  public WidgetInstance m_rewardItem;
  [Header("Battlegrounds")]
  public RewardBanner m_battlegroundsRewardBannerPrefab;
  [Header("Mercenaries")]
  public RewardBanner m_mercenariesRewardBannerPrefab;

  protected override RewardBanner RewardBannerPrefab
  {
    get
    {
      RewardItemDataModel rewardItemDataModel = this.Data is RewardItemRewardData data ? data.DataModel : (RewardItemDataModel) null;
      if (rewardItemDataModel != null)
      {
        if (RewardItemReward.s_mercenariesRewards.Contains(rewardItemDataModel.ItemType))
          return this.m_mercenariesRewardBannerPrefab;
        if (RewardItemReward.s_battlegroundsRewards.Contains(rewardItemDataModel.ItemType))
          return this.m_battlegroundsRewardBannerPrefab;
      }
      return (RewardBanner) null;
    }
  }

  protected override void InitData()
  {
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    base.OnDataSet(updateVisuals);
    this.UpdateBannerObject();
  }

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_root.SetActive(true);
    RewardItemDataModel dataModel = (this.Data as RewardItemRewardData).DataModel;
    this.m_rewardItem.BindDataModel((IDataModel) dataModel, false);
    if (RewardItemReward.s_mercenariesRewards.Contains(dataModel.ItemType))
      this.SetRewardText(GameStrings.Get("GLOBAL_LETTUCE_REWARD_BANNER_TEXT"), "", "");
    else
      this.SetRewardText(GameStrings.Get("GLOBAL_REWARD_CARD_HEADLINE"), "", "");
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  protected override void OnDestroy()
  {
    if (!(this.Data is RewardItemRewardData data))
      return;
    Action onDestroyReward = data.OnDestroyReward;
    if (onDestroyReward == null)
      return;
    onDestroyReward();
  }
}
