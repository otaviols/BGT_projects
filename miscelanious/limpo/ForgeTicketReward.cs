using System;
using UnityEngine;

[Obsolete("use ArenaTicketReward")]
public class ForgeTicketReward : Reward
{
  public GameObject m_rotateParent;

  protected override void InitData() => this.SetData((RewardData) new ForgeTicketRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    string empty1 = string.Empty;
    string empty2 = string.Empty;
    string empty3 = string.Empty;
    string headline;
    string source;
    if (this.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.OUT_OF_BAND_LICENSE)
    {
      ForgeTicketRewardData data = this.Data as ForgeTicketRewardData;
      headline = GameStrings.Get("GLOBAL_REWARD_FORGE_HEADLINE");
      source = GameStrings.Format("GLOBAL_REWARD_BOOSTER_DETAILS_OUT_OF_BAND", (object) data.Quantity);
    }
    else
    {
      headline = GameStrings.Get("GLOBAL_REWARD_FORGE_UNLOCKED_HEADLINE");
      source = GameStrings.Get("GLOBAL_REWARD_FORGE_UNLOCKED_SOURCE");
    }
    this.SetRewardText(headline, empty2, source);
    this.m_root.SetActive(true);
    this.m_rotateParent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
    iTween.RotateAdd(this.m_rotateParent, iTween.Hash((object) "amount", (object) new Vector3(0.0f, 0.0f, 540f), (object) "time", (object) 1.5f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self));
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }
}
