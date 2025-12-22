using UnityEngine;

public class MountReward : Reward
{
  public GameObject m_mount;

  protected override void InitData() => this.SetData((RewardData) new MountRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    if (!(this.Data is MountRewardData))
    {
      Debug.LogWarning((object) string.Format("MountReward.ShowReward() - Data {0} is not MountRewardData", (object) this.Data));
    }
    else
    {
      this.m_root.SetActive(true);
      Vector3 localScale = this.m_mount.transform.localScale;
      this.m_mount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
      iTween.ScaleTo(this.m_mount.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    }
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals || !(this.Data is MountRewardData data))
      return;
    string empty = string.Empty;
    switch (data.Mount)
    {
      case MountRewardData.MountType.WOW_HEARTHSTEED:
        empty = GameStrings.Get("GLOBAL_REWARD_HEARTHSTEED_HEADLINE");
        break;
      case MountRewardData.MountType.HEROES_MAGIC_CARPET_CARD:
        empty = GameStrings.Get("GLOBAL_REWARD_HEROES_CARD_MOUNT_HEADLINE");
        break;
      case MountRewardData.MountType.WOW_SARGE_TALE:
        empty = GameStrings.Get("GLOBAL_REWARD_SARGE_TALE_HEADLINE");
        break;
    }
    this.SetRewardText(empty, string.Empty, string.Empty);
  }
}
