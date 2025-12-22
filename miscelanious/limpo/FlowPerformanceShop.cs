using Blizzard.T5.Core.Time;
using HearthstoneTelemetry;

public class FlowPerformanceShop : FlowPerformance
{
  public Blizzard.Telemetry.WTCG.Client.FlowPerformanceShop.ShopType m_shopType;

  public FlowPerformanceShop(
    ITimeProvider timeProvider,
    ITelemetryClient telemetryClient,
    FlowPerformanceShop.ShopSetupConfig setupConfig)
    : base(timeProvider, telemetryClient, (FlowPerformance.SetupConfig) setupConfig)
  {
    this.SetShopType(setupConfig.shopType);
  }

  protected override void OnStop() => this.m_telemetryClient.SendFlowPerformanceShop(this.GetId(), this.m_shopType);

  private void SetShopType(ShopType shopType)
  {
    switch (shopType)
    {
      case ShopType.ARENA_STORE:
        this.m_shopType = Blizzard.Telemetry.WTCG.Client.FlowPerformanceShop.ShopType.ARENA_STORE;
        break;
      case ShopType.ADVENTURE_STORE:
        this.m_shopType = Blizzard.Telemetry.WTCG.Client.FlowPerformanceShop.ShopType.ADVENTURE_STORE;
        break;
      case ShopType.TAVERN_BRAWL_STORE:
        this.m_shopType = Blizzard.Telemetry.WTCG.Client.FlowPerformanceShop.ShopType.TAVERN_BRAWL_STORE;
        break;
      case ShopType.ADVENTURE_STORE_WING_PURCHASE_WIDGET:
        this.m_shopType = Blizzard.Telemetry.WTCG.Client.FlowPerformanceShop.ShopType.ADVENTURE_STORE_WING_PURCHASE_WIDGET;
        break;
      case ShopType.ADVENTURE_STORE_FULL_PURCHASE_WIDGET:
        this.m_shopType = Blizzard.Telemetry.WTCG.Client.FlowPerformanceShop.ShopType.ADVENTURE_STORE_FULL_PURCHASE_WIDGET;
        break;
      case ShopType.DUELS_STORE:
        this.m_shopType = Blizzard.Telemetry.WTCG.Client.FlowPerformanceShop.ShopType.DUELS_STORE;
        break;
      default:
        this.m_shopType = Blizzard.Telemetry.WTCG.Client.FlowPerformanceShop.ShopType.GENERAL_STORE;
        break;
    }
  }

  public class ShopSetupConfig : FlowPerformance.SetupConfig
  {
    public ShopType shopType;

    public ShopSetupConfig() => this.FlowType = Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType.SHOP;
  }
}
