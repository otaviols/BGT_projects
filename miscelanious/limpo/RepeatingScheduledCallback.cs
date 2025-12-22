using Hearthstone.Core;
using System;

public class RepeatingScheduledCallback
{
  private Func<bool> m_callback;
  private float m_initialDelaySecs;
  private float m_baseIntervalSecs;
  private float m_currIntervalSecs;
  private float m_backoffFactor;
  private float m_jitterSecs;

  public bool IsRunning { get; private set; }

  public int CallbackCount { get; private set; }

  public DateTime NextCallbackTime { get; private set; }

  public void Start(
    Func<bool> callback,
    float initialDelaySecs,
    float intervalSecs,
    float backoffFactor = 1f,
    float jitterSecs = 0.0f)
  {
    if (callback == null)
      throw new ArgumentNullException(nameof (callback));
    this.Stop();
    this.m_callback = callback;
    this.m_initialDelaySecs = initialDelaySecs;
    this.m_baseIntervalSecs = intervalSecs;
    this.m_backoffFactor = backoffFactor;
    this.m_jitterSecs = jitterSecs;
    this.ScheduleNextCallback();
  }

  public void Stop()
  {
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.InternalScheduledCallback));
    this.IsRunning = false;
    this.CallbackCount = 0;
    this.NextCallbackTime = DateTime.MinValue;
  }

  private void ScheduleNextCallback()
  {
    if (this.CallbackCount == 0)
      this.m_currIntervalSecs = this.m_initialDelaySecs;
    else if (this.CallbackCount == 1)
    {
      this.m_currIntervalSecs = this.m_baseIntervalSecs;
    }
    else
    {
      this.m_baseIntervalSecs *= this.m_backoffFactor;
      this.m_currIntervalSecs = this.m_baseIntervalSecs;
    }
    this.m_currIntervalSecs += UnityEngine.Random.Range(0.0f, this.m_jitterSecs);
    this.NextCallbackTime = DateTime.Now.AddSeconds((double) this.m_currIntervalSecs);
    this.IsRunning = true;
    Processor.ScheduleCallback(this.m_currIntervalSecs, true, new Processor.ScheduledCallback(this.InternalScheduledCallback));
  }

  private void InternalScheduledCallback(object userData)
  {
    if (!this.m_callback())
      return;
    ++this.CallbackCount;
    this.ScheduleNextCallback();
  }
}
