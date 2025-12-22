using UnityEngine;

public class SimpleReward : Reward
{
  public QuestTileRewardIcon m_icon;

  protected override void InitData() => this.SetData((RewardData) new SimpleRewardData(Reward.Type.NONE), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    if (!(this.Data is SimpleRewardData))
    {
      Debug.LogWarning((object) string.Format("SimpleReward.ShowReward() - Data {0} is not SimpleRewardData", (object) this.Data));
    }
    else
    {
      this.m_root.SetActive(true);
      Vector3 localScale = this.m_icon.transform.localScale;
      this.m_icon.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
      iTween.ScaleTo(this.m_icon.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    }
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals || !(this.Data is SimpleRewardData data) || data.RewardType == Reward.Type.NONE)
      return;
    this.SetRewardText(data.RewardHeadlineText, "", "");
    if (!((Object) this.m_icon != (Object) null))
      return;
    this.m_icon.InitWithRewardData((RewardData) data, false, 3000);
  }
}
