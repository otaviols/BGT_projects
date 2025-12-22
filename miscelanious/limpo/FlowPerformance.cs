using Blizzard.T5.Core.Time;
using HearthstoneTelemetry;
using System;
using UnityEngine;

public class FlowPerformance
{
  protected ITimeProvider m_timeProvider;
  protected ITelemetryClient m_telemetryClient;
  private Guid m_id;
  private float m_startingFlowTime;
  private int m_frameCount;
  private float m_averageFps;
  private bool m_thresholdActive;
  private float m_thresholdStartTime;
  private int m_totalTriggeredThreshold;
  private float m_totalTimeUnderThreshold;
  private float m_averageTimeBelowThreshold;
  private float m_maxTimeBelowThreshold;
  private bool m_hasAverage;
  private float m_pauseStartTime;
  private float m_totalPausedTime;

  public Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType FlowType { get; }

  public bool IsActive { get; private set; }

  public bool IsPaused { get; private set; }

  public FlowPerformance(
    ITimeProvider timeProvider,
    ITelemetryClient telemetryClient,
    FlowPerformance.SetupConfig setupConfig)
  {
    this.m_timeProvider = timeProvider;
    this.m_telemetryClient = telemetryClient;
    this.FlowType = setupConfig.FlowType;
    this.IsActive = false;
  }

  public void Start()
  {
    Log.FlowPerformance.PrintDebug("Starting flow: {0}", (object) this.FlowType);
    this.m_id = Guid.NewGuid();
    this.IsActive = true;
    this.m_hasAverage = false;
    this.m_startingFlowTime = this.m_timeProvider.TimeSinceStartup;
    this.m_frameCount = 0;
    this.m_totalTriggeredThreshold = 0;
    this.m_totalTimeUnderThreshold = 0.0f;
    this.m_averageTimeBelowThreshold = 0.0f;
    this.m_maxTimeBelowThreshold = 0.0f;
    this.m_thresholdActive = false;
    this.m_totalPausedTime = 0.0f;
    this.OnStart();
  }

  public void Update()
  {
    if (this.IsPaused)
      return;
    ++this.m_frameCount;
    float fps = this.CalculateFps();
    this.IncrementAverageFps(fps);
    this.UpdateThresholdValues(fps);
    this.OnUpdate();
  }

  public void Pause()
  {
    Log.FlowPerformance.PrintDebug("Pausing flow: {0}", (object) this.FlowType);
    if (!this.IsActive)
      return;
    this.IsPaused = true;
    this.m_pauseStartTime = this.m_timeProvider.TimeSinceStartup;
    this.OnPause();
  }

  public void Resume()
  {
    Log.FlowPerformance.PrintDebug("Resuming flow: {0}", (object) this.FlowType);
    if (!this.IsActive || !this.IsPaused)
      return;
    float num = this.m_timeProvider.TimeSinceStartup - this.m_pauseStartTime;
    this.IsPaused = false;
    this.m_totalPausedTime += num;
    this.OnResume();
  }

  public void Stop()
  {
    Log.FlowPerformance.PrintDebug("Stopping flow: {0}", (object) this.FlowType);
    this.IsActive = false;
    this.CloseThresholdPeriod();
    float totalDuration = this.CalculateTotalDuration();
    this.m_telemetryClient.SendFlowPerformance(this.m_id.ToString(), this.FlowType, this.m_averageFps, totalDuration, 20f, this.m_totalTriggeredThreshold, this.m_totalTimeUnderThreshold, this.m_averageTimeBelowThreshold, this.m_maxTimeBelowThreshold);
    this.OnStop();
  }

  protected virtual void OnStart()
  {
  }

  protected virtual void OnUpdate()
  {
  }

  protected virtual void OnPause()
  {
  }

  protected virtual void OnResume()
  {
  }

  protected virtual void OnStop()
  {
  }

  protected string GetId() => this.m_id.ToString();

  private float CalculateFps() => 1f / this.m_timeProvider.UnscaledDeltaTime;

  private void IncrementAverageFps(float currentFps)
  {
    if (!this.m_hasAverage)
    {
      this.m_averageFps = currentFps;
      this.m_hasAverage = true;
    }
    this.m_averageFps += (currentFps - this.m_averageFps) / (float) this.m_frameCount;
  }

  private void UpdateThresholdValues(float currentFps)
  {
    bool flag = (double) currentFps <= 20.0;
    if (flag && !this.m_thresholdActive)
    {
      this.m_thresholdActive = true;
      this.m_thresholdStartTime = this.m_timeProvider.TimeSinceStartup;
      ++this.m_totalTriggeredThreshold;
    }
    else
    {
      if (flag || !this.m_thresholdActive)
        return;
      this.CloseThresholdPeriod();
    }
  }

  private void CloseThresholdPeriod()
  {
    if (!this.m_thresholdActive)
      return;
    this.m_thresholdActive = false;
    float b = this.m_timeProvider.TimeSinceStartup - this.m_thresholdStartTime;
    this.m_totalTimeUnderThreshold += b;
    this.m_averageTimeBelowThreshold += (b - this.m_averageTimeBelowThreshold) / (float) this.m_totalTriggeredThreshold;
    this.m_maxTimeBelowThreshold = Mathf.Max(this.m_maxTimeBelowThreshold, b);
  }

  private float CalculateTotalDuration() => this.IsPaused ? this.m_pauseStartTime - this.m_startingFlowTime : this.m_timeProvider.TimeSinceStartup - (this.m_startingFlowTime + this.m_totalPausedTime);

  public class SetupConfig
  {
    public Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType FlowType;
  }
}
