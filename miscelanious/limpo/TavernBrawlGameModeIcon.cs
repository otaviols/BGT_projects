using PegasusShared;
using PegasusUtil;
using UnityEngine;

public class TavernBrawlGameModeIcon : GameModeIcon
{
  public float m_tooltipScale = 1f;
  private string m_brawlName;
  private string m_brawlDescription;
  private TooltipZone m_tooltipZone;
  private BrawlType m_brawlType;

  protected override void Awake()
  {
    base.Awake();
    this.m_tooltipZone = this.gameObject.GetComponent<TooltipZone>();
    if (!((Object) this.m_tooltipZone != (Object) null))
      return;
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.MedalOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.MedalOut));
    TavernBrawlManager.Get().CurrentBrawlType = GameUtils.IsFiresideGatheringGameType(GameMgr.Get().GetGameType()) ? BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING : BrawlType.BRAWL_TYPE_TAVERN_BRAWL;
    if (TavernBrawlManager.Get().GetMission(TavernBrawlManager.Get().CurrentBrawlType) == null)
    {
      Network.Get().RegisterNetHandler((object) TavernBrawlInfo.PacketID.ID, new Network.NetHandler(this.EnsureTavernBrawlDataReady));
      Network.Get().RequestTavernBrawlInfo(TavernBrawlManager.Get().CurrentBrawlType);
    }
    else
      this.EnsureTavernBrawlDataReady();
  }

  private void EnsureTavernBrawlDataReady()
  {
    Network.Get().RemoveNetHandler((object) TavernBrawlInfo.PacketID.ID, new Network.NetHandler(this.EnsureTavernBrawlDataReady));
    TavernBrawlManager.Get().EnsureAllDataReady((TavernBrawlManager.CallbackEnsureServerDataReady) (() =>
    {
      int missionId = GameMgr.Get().GetMissionId();
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
      if (record == null)
        return;
      this.m_brawlName = (string) record.Name;
      this.m_brawlDescription = (string) record.Description;
    }));
  }

  public void MedalOver(UIEvent e)
  {
    if (string.IsNullOrEmpty(this.m_brawlName))
      return;
    this.m_tooltipZone.ShowLayerTooltip(this.m_brawlName, this.m_brawlDescription, this.m_tooltipScale);
  }

  private void MedalOut(UIEvent e) => this.m_tooltipZone.HideTooltip();
}
