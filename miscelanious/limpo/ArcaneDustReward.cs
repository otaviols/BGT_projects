using UnityEngine;

public class ArcaneDustReward : Reward
{
  public GameObject m_dustJar;
  public UberText m_dustCount;

  protected override void InitData() => this.SetData((RewardData) new ArcaneDustRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    if (!(this.Data is ArcaneDustRewardData data))
    {
      Debug.LogWarning((object) string.Format("ArcaneDustReward.ShowReward() - Data {0} is not ArcaneDustRewardData", (object) this.Data));
    }
    else
    {
      this.m_root.SetActive(true);
      this.m_dustCount.Text = data.Amount.ToString();
      Vector3 localScale = this.m_dustJar.transform.localScale;
      this.m_dustJar.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
      iTween.ScaleTo(this.m_dustJar.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    }
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    this.SetRewardText(GameStrings.Get("GLOBAL_REWARD_ARCANE_DUST_HEADLINE"), "", "");
  }
}
