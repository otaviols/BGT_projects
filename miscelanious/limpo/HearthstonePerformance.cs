using UnityEngine;

public class HearthstonePerformance
{
  private static HearthstonePerformance s_instance;
  private FlowPerformanceManager m_flowPerformanceManager;
  private string m_testType;
  private string m_changelist;
  private bool m_hasAppStartTime;
  private bool m_hasAppInitializedTime;
  private bool m_hasBoxInteractableTime;

  public float AppStartTime { get; private set; }

  public float AppInitializedTime { get; private set; }

  public float BoxInteractableTime { get; private set; }

  private HearthstonePerformance(string testType, string changelist)
  {
    this.m_testType = testType;
    this.m_changelist = changelist;
    this.m_flowPerformanceManager = new FlowPerformanceManager();
  }

  public static HearthstonePerformance Get() => HearthstonePerformance.s_instance;

  public static void Initialize(string testType, string changelist) => HearthstonePerformance.s_instance = new HearthstonePerformance(testType, changelist);

  public static void Shutdown() => HearthstonePerformance.s_instance = (HearthstonePerformance) null;

  public void DoLateUpdate() => this.m_flowPerformanceManager.LateUpdate();

  public void CaptureAppStartTime()
  {
    if (this.m_hasAppStartTime)
      return;
    this.AppStartTime = Time.realtimeSinceStartup;
    TelemetryManager.Client().SendAppStart(this.m_testType, this.AppStartTime, this.m_changelist);
    this.m_hasAppStartTime = true;
  }

  public void CaptureAppInitializedTime()
  {
    if (this.m_hasAppInitializedTime)
      return;
    this.AppInitializedTime = Time.realtimeSinceStartup - this.AppStartTime;
    TelemetryManager.Client().SendAppInitialized(this.m_testType, this.AppInitializedTime, this.m_changelist);
    this.m_hasAppInitializedTime = true;
  }

  public void CaptureBoxInteractableTime()
  {
    if (this.m_hasBoxInteractableTime)
      return;
    this.BoxInteractableTime = Time.realtimeSinceStartup - this.AppStartTime;
    TelemetryManager.Client().SendBoxInteractable(this.m_testType, this.BoxInteractableTime, this.m_changelist);
    this.m_hasBoxInteractableTime = true;
  }

  public void SendCustomEvent(string eventName)
  {
  }

  public void StartPerformanceFlow(FlowPerformance.SetupConfig setupConfig) => this.m_flowPerformanceManager.StartPerformanceFlow(setupConfig);

  public T GetCurrentPerformanceFlow<T>() where T : FlowPerformance => this.m_flowPerformanceManager.GetCurrentPerformanceFlow<T>();

  public void StopCurrentFlow() => this.m_flowPerformanceManager.StopCurrentFlow();

  public void OnApplicationPause() => this.m_flowPerformanceManager.PauseCurrentFlow();

  public void OnApplicationResume() => this.m_flowPerformanceManager.ResumeCurrentFlow();
}
