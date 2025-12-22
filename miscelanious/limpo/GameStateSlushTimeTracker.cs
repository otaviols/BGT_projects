using System;

public class GameStateSlushTimeTracker : IGameStateTimeTracker
{
  protected float m_accruedLostFrameTimeReal;

  public void Update()
  {
  }

  public void AdjustAccruedLostTime(float deltaSeconds)
  {
    this.m_accruedLostFrameTimeReal = deltaSeconds;
    this.m_accruedLostFrameTimeReal = Math.Max(this.m_accruedLostFrameTimeReal, 0.0f);
  }

  public void ResetAccruedLostTime() => this.m_accruedLostFrameTimeReal = 0.0f;

  public float GetAccruedLostTimeInSeconds() => this.m_accruedLostFrameTimeReal;
}
