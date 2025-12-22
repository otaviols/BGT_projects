using UnityEngine;

public class EventReward : Reward
{
  protected override void InitData() => this.SetData((RewardData) new EventRewardData(), false);

  protected override void ShowReward(bool updateCacheValues) => this.m_root.SetActive(true);

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    if (!(this.Data is EventRewardData data))
    {
      Debug.LogWarning((object) string.Format("EventRewardData.SetData() - data {0} is not EventRewardData", (object) this.Data));
    }
    else
    {
      string empty1 = string.Empty;
      if (data.EventType == 0)
        empty1 = GameStrings.Get("GLUE_2X_GOLD_EVENT_BANNER_HEADLINE");
      string empty2 = string.Empty;
      string empty3 = string.Empty;
      this.SetRewardText(empty1, empty2, empty3);
    }
  }
}
