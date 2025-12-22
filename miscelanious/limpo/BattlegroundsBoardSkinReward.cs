using Hearthstone.UI;
using UnityEngine;

public class BattlegroundsBoardSkinReward : Reward
{
  [SerializeField]
  private WidgetInstance m_rewardItem;

  protected override void InitData() => this.SetData((RewardData) new BattlegroundsBoardSkinRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_root.SetActive(true);
    this.m_rewardItem.BindDataModel((IDataModel) (this.Data as BattlegroundsBoardSkinRewardData).DataModel, false);
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }
}
