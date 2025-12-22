using System;
using UnityEngine;

[Serializable]
public class MiddleGamesWonSegment : GamesWonSegment
{
  public GameObject m_root1;
  public GameObject m_root2;
  private GameObject m_activeRoot;

  public override void Init(Reward.Type rewardType, int rewardAmount, bool hideCrown)
  {
    base.Init(rewardType, rewardAmount, hideCrown);
    if ((double) UnityEngine.Random.value < 0.5)
    {
      this.m_activeRoot = this.m_root1;
      this.m_root2.SetActive(false);
    }
    else
    {
      this.m_activeRoot = this.m_root2;
      this.m_root1.SetActive(false);
    }
    this.m_activeRoot.SetActive(true);
  }

  public override float GetWidth() => this.m_activeRoot.GetComponent<Renderer>().bounds.size.x;
}
