using Blizzard.T5.Core.Time;
using HearthstoneTelemetry;

public class FlowPerformanceFactory
{
  private ITimeProvider m_timeProvider;
  private ITelemetryClient m_telemetryClient;

  public FlowPerformanceFactory()
  {
    this.m_timeProvider = (ITimeProvider) new UnityTimeProvider();
    this.m_telemetryClient = TelemetryManager.Client();
  }

  public FlowPerformance CreatePerformanceFlow(
    FlowPerformance.SetupConfig setupConfig)
  {
    if (this.m_telemetryClient == null)
    {
      ITelemetryClient telemetryClient = TelemetryManager.Client();
      if (telemetryClient == null)
        return (FlowPerformance) null;
      this.m_telemetryClient = telemetryClient;
    }
    FlowPerformance performanceFlow;
    switch (setupConfig.FlowType)
    {
      case Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType.SHOP:
        performanceFlow = (FlowPerformance) new FlowPerformanceShop(this.m_timeProvider, this.m_telemetryClient, setupConfig as FlowPerformanceShop.ShopSetupConfig);
        break;
      case Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType.GAME:
        FlowPerformanceGame.GameSetupConfig setupConfig1 = setupConfig as FlowPerformanceGame.GameSetupConfig;
        performanceFlow = setupConfig1.GameType != PegasusShared.GameType.GT_BATTLEGROUNDS ? (FlowPerformance) new FlowPerformanceGame(this.m_timeProvider, this.m_telemetryClient, setupConfig1) : (FlowPerformance) new FlowPerformanceBattlegrounds(this.m_timeProvider, this.m_telemetryClient, setupConfig1);
        break;
      default:
        performanceFlow = new FlowPerformance(this.m_timeProvider, this.m_telemetryClient, setupConfig);
        break;
    }
    return performanceFlow;
  }
}
