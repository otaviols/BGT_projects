using Hearthstone.UI;

public class MiniSetReward : Reward
{
  public WidgetInstance m_rewardItem;

  protected override void InitData() => this.SetData((RewardData) new CardBackRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_root.SetActive(true);
    this.m_rewardItem.BindDataModel((IDataModel) (this.Data as MiniSetRewardData).DataModel, false);
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }
}
