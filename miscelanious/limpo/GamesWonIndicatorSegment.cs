using UnityEngine;

public class GamesWonIndicatorSegment : MonoBehaviour
{
  public GamesWonSegment m_leftSegment;
  public MiddleGamesWonSegment m_middleSegment;
  public RightGamesWonSegment m_rightSegment;
  private GamesWonSegment m_activeSegment;

  public void Init(
    GamesWonIndicatorSegment.Type segmentType,
    Reward.Type rewardType,
    int rewardAmount,
    bool hideCrown)
  {
    switch (segmentType)
    {
      case GamesWonIndicatorSegment.Type.LEFT:
        this.m_activeSegment = this.m_leftSegment;
        this.m_middleSegment.Hide();
        this.m_rightSegment.Hide();
        break;
      case GamesWonIndicatorSegment.Type.MIDDLE:
        this.m_activeSegment = (GamesWonSegment) this.m_middleSegment;
        this.m_leftSegment.Hide();
        this.m_rightSegment.Hide();
        break;
      case GamesWonIndicatorSegment.Type.RIGHT:
        this.m_activeSegment = (GamesWonSegment) this.m_rightSegment;
        this.m_leftSegment.Hide();
        this.m_middleSegment.Hide();
        break;
    }
    this.m_activeSegment.Init(rewardType, rewardAmount, hideCrown);
  }

  public float GetWidth() => this.m_activeSegment.GetWidth();

  public void AnimateReward() => this.m_activeSegment.AnimateReward();

  public enum Type
  {
    LEFT,
    MIDDLE,
    RIGHT,
  }
}
