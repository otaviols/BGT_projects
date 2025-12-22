using Hearthstone.UI;
using System;
using UnityEngine;

public class MercenariesKnockoutReward : Reward
{
  [Header("Mercenaries")]
  public RewardBanner m_mercenariesRewardBannerPrefab;
  public WidgetInstance m_mercenaryRewardItem;
  public WidgetInstance m_knockoutRewardItem;

  protected override RewardBanner RewardBannerPrefab => this.m_mercenariesRewardBannerPrefab;

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
    if (this.Data is MercenariesKnockoutRewardData data)
    {
      this.m_mercenaryRewardItem.BindDataModel((IDataModel) data.MercenaryDataModel, false);
      this.m_knockoutRewardItem.BindDataModel((IDataModel) data.KnockoutDataModel, false);
    }
    this.SetRewardText(GameStrings.Get("GLOBAL_LETTUCE_REWARD_BANNER_TEXT"), "", "");
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
