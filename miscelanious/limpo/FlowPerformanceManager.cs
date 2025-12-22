using System.Collections.Generic;

public class FlowPerformanceManager
{
  private FlowPerformanceFactory m_flowPerformanceFactory;
  private Stack<FlowPerformance> m_flowStack;
  private ReactiveObject<NetCache.NetCacheFeatures> m_guardianVars = (ReactiveObject<NetCache.NetCacheFeatures>) ReactiveNetCacheObject<NetCache.NetCacheFeatures>.CreateInstance();

  public FlowPerformanceManager()
  {
    this.m_flowPerformanceFactory = new FlowPerformanceFactory();
    this.m_flowStack = new Stack<FlowPerformance>();
  }

  public void LateUpdate()
  {
    if (this.m_flowStack.Count <= 0 || !this.CanRecordMetrics())
      return;
    this.m_flowStack.Peek().Update();
  }

  public void StartPerformanceFlow(FlowPerformance.SetupConfig setupConfig)
  {
    if (!this.CanRecordMetrics())
      return;
    this.StopExistingFlow(setupConfig.FlowType);
    this.PauseCurrentFlow();
    FlowPerformance performanceFlow = this.m_flowPerformanceFactory.CreatePerformanceFlow(setupConfig);
    performanceFlow.Start();
    this.m_flowStack.Push(performanceFlow);
  }

  public T GetCurrentPerformanceFlow<T>() where T : FlowPerformance => this.m_flowStack.Count == 0 ? default (T) : this.m_flowStack.Peek() as T;

  public void StopCurrentFlow()
  {
    if (this.m_flowStack.Count <= 0 || !this.CanRecordMetrics())
      return;
    this.m_flowStack.Pop().Stop();
    this.ResumeCurrentFlow();
  }

  public void PauseCurrentFlow()
  {
    if (this.m_flowStack.Count <= 0)
      return;
    FlowPerformance flowPerformance = this.m_flowStack.Peek();
    if (flowPerformance.IsActive)
    {
      flowPerformance.Pause();
    }
    else
    {
      this.m_flowStack.Pop();
      this.PauseCurrentFlow();
    }
  }

  public void ResumeCurrentFlow()
  {
    if (this.m_flowStack.Count <= 0)
      return;
    FlowPerformance flowPerformance = this.m_flowStack.Peek();
    if (flowPerformance.IsActive && flowPerformance.IsPaused)
    {
      flowPerformance.Resume();
    }
    else
    {
      if (flowPerformance.IsActive)
        return;
      this.m_flowStack.Pop();
      this.ResumeCurrentFlow();
    }
  }

  private void StopExistingFlow(Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType flowType)
  {
    foreach (FlowPerformance flow in this.m_flowStack)
    {
      if (flow.FlowType == flowType)
      {
        Log.FlowPerformance.PrintWarning("A flow of type {0} has been started without finishing the previous one!", (object) flowType);
        flow.Stop();
        break;
      }
    }
  }

  private bool CanRecordMetrics()
  {
    NetCache.NetCacheFeatures netCacheFeatures = this.m_guardianVars.Value;
    return netCacheFeatures != null && netCacheFeatures.Misc.AllowLiveFPSGathering;
  }
}
