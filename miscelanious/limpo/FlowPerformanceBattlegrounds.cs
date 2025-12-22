using Blizzard.T5.Core.Time;
using HearthstoneTelemetry;

public class FlowPerformanceBattlegrounds : FlowPerformanceGame
{
  private int m_numberOfRounds;

  public FlowPerformanceBattlegrounds(
    ITimeProvider timeProvider,
    ITelemetryClient telemetryClient,
    FlowPerformanceGame.GameSetupConfig setupConfig)
    : base(timeProvider, telemetryClient, setupConfig)
  {
    this.m_numberOfRounds = 0;
  }

  public void OnNewRoundStart() => ++this.m_numberOfRounds;

  protected override void OnStop()
  {
    base.OnStop();
    this.m_telemetryClient.SendFlowPerformanceBattlegrounds(this.GetId(), this.GameUuid, this.m_numberOfRounds);
  }
}
