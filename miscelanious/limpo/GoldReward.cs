using UnityEngine;

public class GoldReward : Reward
{
  public GameObject m_coin;
  public bool m_RotateIn = true;

  protected override void InitData() => this.SetData((RewardData) new GoldRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_root.SetActive(true);
    Vector3 localScale = this.m_coin.transform.localScale;
    this.m_coin.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    iTween.ScaleTo(this.m_coin.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    if (!this.m_RotateIn)
      return;
    this.m_coin.transform.localEulerAngles = new Vector3(0.0f, 180f, 180f);
    iTween.RotateAdd(this.m_coin.gameObject, iTween.Hash((object) "amount", (object) new Vector3(0.0f, 0.0f, 540f), (object) "time", (object) 1.5f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self));
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
    if (!(this.Data is GoldRewardData data))
    {
      Debug.LogWarning((object) string.Format("goldRewardData.SetData() - data {0} is not GoldRewardData", (object) this.Data));
    }
    else
    {
      string headline = GameStrings.Get("GLOBAL_REWARD_GOLD_HEADLINE");
      string details = data.Amount.ToString();
      string source = string.Empty;
      UberText componentInChildren = this.m_coin.GetComponentInChildren<UberText>(true);
      if ((Object) componentInChildren != (Object) null)
      {
        this.m_rewardBanner.m_detailsText = componentInChildren;
        this.m_rewardBanner.AlignHeadlineToCenterBone();
      }
      switch (this.Data.Origin)
      {
        case NetCache.ProfileNotice.NoticeOrigin.BETA_REIMBURSE:
          headline = GameStrings.Get("GLOBAL_BETA_REIMBURSEMENT_HEADLINE");
          source = GameStrings.Get("GLOBAL_BETA_REIMBURSEMENT_DETAILS");
          break;
        case NetCache.ProfileNotice.NoticeOrigin.IGR:
          if (data.Date.HasValue)
          {
            source = GameStrings.Format("GLOBAL_REWARD_GOLD_SOURCE_IGR_DATED", (object) GameStrings.Format("GLOBAL_CURRENT_DATE", (object) data.Date));
            break;
          }
          source = GameStrings.Get("GLOBAL_REWARD_GOLD_SOURCE_IGR");
          break;
      }
      this.SetRewardText(headline, details, source);
    }
  }
}
