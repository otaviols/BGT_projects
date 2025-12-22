using UnityEngine;

public class ArenaTicketReward : Reward
{
  public GameObject m_ticketVisual;
  public GameObject m_plusSign;
  public UberText m_countLabel;

  protected override void InitData() => this.SetData((RewardData) new ForgeTicketRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    string empty1 = string.Empty;
    string empty2 = string.Empty;
    string source = string.Empty;
    string headline;
    if (this.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.OUT_OF_BAND_LICENSE)
    {
      ForgeTicketRewardData data = this.Data as ForgeTicketRewardData;
      headline = GameStrings.Get("GLOBAL_REWARD_FORGE_HEADLINE");
      source = GameStrings.Format("GLOBAL_REWARD_FORGE_DETAILS_OUT_OF_BAND", (object) data.Quantity);
    }
    else if (this.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT && this.Data.OriginData == 56L)
    {
      headline = GameStrings.Get("GLOBAL_REWARD_FORGE_UNLOCKED_HEADLINE");
      source = GameStrings.Get("GLOBAL_REWARD_FORGE_UNLOCKED_SOURCE");
    }
    else
      headline = GameStrings.Get("GLOBAL_REWARD_ARENA_TICKET_HEADLINE");
    this.SetRewardText(headline, empty2, source);
    bool flag = false;
    if ((Object) this.m_countLabel != (Object) null)
    {
      ForgeTicketRewardData data = this.Data as ForgeTicketRewardData;
      if (data.Quantity > 9)
      {
        this.m_countLabel.Text = "9";
        flag = true;
      }
      else
        this.m_countLabel.Text = data.Quantity.ToString();
    }
    this.m_root.SetActive(true);
    if ((Object) this.m_plusSign != (Object) null)
      this.m_plusSign.SetActive(flag);
    this.m_ticketVisual.transform.localEulerAngles = new Vector3(this.m_ticketVisual.transform.localEulerAngles.x, this.m_ticketVisual.transform.localEulerAngles.y, 180f);
    iTween.RotateAdd(this.m_ticketVisual, iTween.Hash((object) "amount", (object) new Vector3(0.0f, 0.0f, 540f), (object) "time", (object) 1.5f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self));
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }
}
