using Blizzard.T5.Core.Time;
using HearthstoneTelemetry;

public class FlowPerformanceGame : FlowPerformance
{
  private PegasusShared.GameType GameType;
  private PegasusShared.FormatType FormatType;
  private int BoardId;
  private int ScenarioId;

  public string GameUuid { get; set; }

  public FlowPerformanceGame(
    ITimeProvider timeProvider,
    ITelemetryClient telemetryClient,
    FlowPerformanceGame.GameSetupConfig setupConfig)
    : base(timeProvider, telemetryClient, (FlowPerformance.SetupConfig) setupConfig)
  {
    this.GameType = setupConfig.GameType;
    this.FormatType = setupConfig.FormatType;
    this.BoardId = setupConfig.BoardId;
    this.ScenarioId = setupConfig.ScenarioId;
  }

  protected override void OnStop() => this.m_telemetryClient.SendFlowPerformanceGame(this.GetId(), this.GameUuid, (Blizzard.Telemetry.WTCG.Client.GameType) this.GameType, (Blizzard.Telemetry.WTCG.Client.FormatType) this.FormatType, this.BoardId, this.ScenarioId);

  public class GameSetupConfig : FlowPerformance.SetupConfig
  {
    public PegasusShared.GameType GameType;
    public PegasusShared.FormatType FormatType;
    public int BoardId;
    public int ScenarioId;

    public GameSetupConfig() => this.FlowType = Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType.GAME;
  }
}
