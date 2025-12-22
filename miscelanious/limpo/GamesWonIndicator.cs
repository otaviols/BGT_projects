using System.Collections.Generic;
using UnityEngine;

public class GamesWonIndicator : MonoBehaviour
{
  public GameObject m_root;
  public GameObject m_segmentContainer;
  public UberText m_winCountText;
  public GamesWonIndicatorSegment m_gamesWonSegmentPrefab;
  private List<GamesWonIndicatorSegment> m_segments = new List<GamesWonIndicatorSegment>();
  private int m_numActiveSegments;
  private GamesWonIndicator.InnKeeperTrigger m_innkeeperTrigger;
  private const float FUDGE_FACTOR = 0.01f;

  public void Init(
    Reward.Type rewardType,
    int rewardAmount,
    int numSegments,
    int numActiveSegments,
    GamesWonIndicator.InnKeeperTrigger trigger)
  {
    this.m_innkeeperTrigger = trigger;
    this.m_numActiveSegments = numActiveSegments;
    Vector3 position1 = this.m_segmentContainer.transform.position;
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int index = 0; index < numSegments; ++index)
    {
      GamesWonIndicatorSegment.Type segmentType = index != 0 ? (index != numSegments - 1 ? GamesWonIndicatorSegment.Type.MIDDLE : GamesWonIndicatorSegment.Type.RIGHT) : GamesWonIndicatorSegment.Type.LEFT;
      bool hideCrown = index >= numActiveSegments - 1;
      GamesWonIndicatorSegment indicatorSegment = Object.Instantiate<GamesWonIndicatorSegment>(this.m_gamesWonSegmentPrefab);
      indicatorSegment.Init(segmentType, rewardType, rewardAmount, hideCrown);
      indicatorSegment.transform.parent = this.m_segmentContainer.transform;
      indicatorSegment.transform.localScale = Vector3.one;
      float num3 = indicatorSegment.GetWidth() - 0.01f;
      if (segmentType != GamesWonIndicatorSegment.Type.RIGHT)
        position1.x += num3;
      else
        position1.x -= 0.01f;
      indicatorSegment.transform.position = position1;
      indicatorSegment.transform.rotation = Quaternion.identity;
      num1 = num3;
      num2 += num3;
      this.m_segments.Add(indicatorSegment);
    }
    Vector3 position2 = this.m_segmentContainer.transform.position;
    position2.x -= num2 / 2f;
    position2.x += num1 / 5f;
    this.m_segmentContainer.transform.position = position2;
    this.m_winCountText.Text = GameStrings.Format("GAMEPLAY_WIN_REWARD_PROGRESS", (object) this.m_numActiveSegments, (object) numSegments);
  }

  public void Show()
  {
    this.m_root.SetActive(true);
    if (this.m_numActiveSegments <= 0)
      Debug.LogError((object) string.Format("GamesWonIndicator.Show(): cannot do animation; numActiveSegments={0} but should be greater than zero", (object) this.m_numActiveSegments));
    else if (this.m_numActiveSegments > this.m_segments.Count)
    {
      Debug.LogError((object) string.Format("GamesWonIndicator.Show(): cannot do animation; numActiveSegments = {0} but m_segments.Count = {1}", (object) this.m_numActiveSegments, (object) this.m_segments.Count));
    }
    else
    {
      this.m_segments[this.m_numActiveSegments - 1].AnimateReward();
      int innkeeperTrigger = (int) this.m_innkeeperTrigger;
    }
  }

  public void Hide() => this.m_root.SetActive(false);

  public enum InnKeeperTrigger
  {
    NONE,
  }
}
