using Hearthstone.UI;
using UnityEngine;

public class BattlegroundsEmoteReward : Reward
{
  [SerializeField]
  private WidgetInstance m_rewardItem;
  [SerializeField]
  private string m_emoteEventToTrigger = "DEFAULT_BOTTOM_LEFT";

  protected override void InitData() => this.SetData((RewardData) new BattlegroundsEmoteRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_root.SetActive(true);
    this.m_rewardItem.BindDataModel((IDataModel) (this.Data as BattlegroundsEmoteRewardData).DataModel, false);
    this.m_rewardItem.TriggerEvent(this.m_emoteEventToTrigger, new Widget.TriggerEventParameters());
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }
}
