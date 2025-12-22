using System;
using UnityEngine;

public class GameStateFrameTimeTracker : IGameStateTimeTracker
{
  protected float[] m_frameTimeBuffer;
  protected int m_lastBufferPos = -1;
  protected float m_desiredFrameTimeReal;
  protected float m_accruedLostFrameTimeReal;

  private GameStateFrameTimeTracker()
    : this(15)
  {
  }

  public GameStateFrameTimeTracker(int bufferSize, float desiredFrameTimeInSeconds = 0.0f)
  {
    this.m_frameTimeBuffer = new float[bufferSize];
    for (int index = 0; index < bufferSize; ++index)
      this.m_frameTimeBuffer[index] = 0.016667f;
    if ((double) desiredFrameTimeInSeconds <= 0.0)
      return;
    this.m_desiredFrameTimeReal = Math.Max(desiredFrameTimeInSeconds, 0.016667f);
  }

  public void Update()
  {
    this.m_lastBufferPos = (this.m_lastBufferPos + 1) % this.m_frameTimeBuffer.Length;
    this.m_frameTimeBuffer[this.m_lastBufferPos] = Time.unscaledDeltaTime;
    if ((double) this.m_desiredFrameTimeReal <= 0.0 || (double) Time.unscaledDeltaTime <= (double) this.m_desiredFrameTimeReal)
      return;
    this.m_accruedLostFrameTimeReal += Time.unscaledDeltaTime - this.m_desiredFrameTimeReal;
  }

  public void AdjustAccruedLostTime(float deltaSeconds)
  {
    this.m_accruedLostFrameTimeReal += deltaSeconds;
    this.m_accruedLostFrameTimeReal = Math.Max(this.m_accruedLostFrameTimeReal, 0.0f);
  }

  public void ResetAccruedLostTime() => this.m_accruedLostFrameTimeReal = 0.0f;

  private float GetAverageFrameTimeInSeconds()
  {
    float frameTimeInSeconds = 0.0f;
    float num = 1f / (float) this.m_frameTimeBuffer.Length;
    for (int index = 0; index < this.m_frameTimeBuffer.Length; ++index)
      frameTimeInSeconds += this.m_frameTimeBuffer[index] * num;
    return frameTimeInSeconds;
  }

  public float GetAverageFPS() => 1f / this.GetAverageFrameTimeInSeconds();

  public float GetAccruedLostTimeInSeconds() => this.m_accruedLostFrameTimeReal;
}
