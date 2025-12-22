using System;
using UnityEngine;

[Serializable]
public class RightGamesWonSegment : GamesWonSegment
{
  public GameObject m_boosterRoot;
  public GameObject m_dustRoot;
  public GameObject m_goldRoot;
  public UberText m_dustAmountText;
  public UberText m_goldAmountText;
  private Reward.Type m_rewardType;

  public override void Init(Reward.Type rewardType, int rewardAmount, bool hideCrown)
  {
    base.Init(rewardType, rewardAmount, hideCrown);
    this.m_rewardType = rewardType;
    switch (this.m_rewardType)
    {
      case Reward.Type.ARCANE_DUST:
        this.m_boosterRoot.SetActive(false);
        this.m_goldRoot.SetActive(false);
        this.m_dustRoot.SetActive(true);
        this.m_dustAmountText.Text = rewardAmount.ToString();
        break;
      case Reward.Type.BOOSTER_PACK:
        this.m_dustRoot.SetActive(false);
        this.m_goldRoot.SetActive(false);
        this.m_boosterRoot.SetActive(true);
        break;
      case Reward.Type.GOLD:
        this.m_boosterRoot.SetActive(false);
        this.m_dustRoot.SetActive(false);
        this.m_goldRoot.SetActive(true);
        this.m_goldAmountText.Text = rewardAmount.ToString();
        break;
      default:
        Debug.LogError((object) string.Format("GamesWonIndicatorSegment(): don't know how to init right segment with reward type {0}", (object) this.m_rewardType));
        this.m_boosterRoot.SetActive(false);
        this.m_dustRoot.SetActive(false);
        this.m_goldRoot.SetActive(false);
        break;
    }
  }

  public override void AnimateReward()
  {
    this.m_crown.Animate();
    PlayMakerFSM playMakerFsm = (PlayMakerFSM) null;
    switch (this.m_rewardType)
    {
      case Reward.Type.ARCANE_DUST:
        playMakerFsm = this.m_dustRoot.GetComponent<PlayMakerFSM>();
        break;
      case Reward.Type.BOOSTER_PACK:
        playMakerFsm = this.m_boosterRoot.GetComponent<PlayMakerFSM>();
        break;
      case Reward.Type.GOLD:
        playMakerFsm = this.m_goldRoot.GetComponent<PlayMakerFSM>();
        break;
    }
    if ((UnityEngine.Object) playMakerFsm == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("GamesWonIndicatorSegment(): missing playMaker component for reward type {0}", (object) this.m_rewardType));
    else
      playMakerFsm.SendEvent("Birth");
  }

  public override float GetWidth()
  {
    switch (this.m_rewardType)
    {
      case Reward.Type.ARCANE_DUST:
        return this.m_dustRoot.GetComponent<Renderer>().bounds.size.x;
      case Reward.Type.BOOSTER_PACK:
        return this.m_boosterRoot.GetComponent<Renderer>().bounds.size.x;
      case Reward.Type.GOLD:
        return this.m_goldRoot.GetComponent<Renderer>().bounds.size.x;
      default:
        return 0.0f;
    }
  }
}
