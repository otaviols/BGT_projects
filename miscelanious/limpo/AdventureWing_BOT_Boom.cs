using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdventureWing_BOT_Boom : AdventureWing
{
  public List<UIBButton> CheatPlateOpenButtons;
  public List<AdventureWing_BOT_Boom.PlateCoverData> m_plateCoverData;

  protected override void Awake()
  {
    base.Awake();
    foreach (AdventureWing_BOT_Boom.PlateCoverData plateCoverData in this.m_plateCoverData)
    {
      AdventureWing_BOT_Boom.PlateCoverData data = plateCoverData;
      data.PlateCoverHitbox.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.OnPlateCoverRollover(data)));
      data.PlateCoverHitbox.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.OnPlateCoverRollout(data)));
    }
    if (!this.IsDevMode)
      return;
    this.CheatPlateOpenButtons[0].AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.m_WingEventTable.DoStatePlateOpen(0)));
    this.CheatPlateOpenButtons[1].AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.m_WingEventTable.DoStatePlateOpen(1)));
    this.CheatPlateOpenButtons[2].AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.m_WingEventTable.DoStatePlateOpen(2)));
    this.CheatPlateOpenButtons[3].AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.m_WingEventTable.DoStatePlateOpen(3)));
  }

  private void OnPlateCoverRollover(AdventureWing_BOT_Boom.PlateCoverData data) => data.PlateCoverHitbox.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get("GLUE_ADVENTURE_BOT_BOOM_TOOLTIP_HEADER"), GameStrings.Get(data.TooltipText), 5f);

  private void OnPlateCoverRollout(AdventureWing_BOT_Boom.PlateCoverData data) => data.PlateCoverHitbox.GetComponent<TooltipZone>().HideTooltip();

  protected override void DoOpenPlate(float unlockDelay)
  {
    List<AdventureMissionDbfRecord> recordsForThisWing = this.GetSortedAdventureMissionRecordsForThisWing();
    bool flag = false;
    foreach (AdventureMissionDbfRecord record in recordsForThisWing)
    {
      int reqProgress = record.ReqProgress;
      int progressValueForWing = AdventureProgressMgr.Get().GetProgressValueForWing(record.ReqWingId);
      int ack;
      AdventureProgressMgr.Get().GetWingAck(record.ReqWingId, out ack);
      if (progressValueForWing == reqProgress && (ack < progressValueForWing || this.HasDependentWingJustAckedRequiredProgress(record)))
      {
        this.m_WingEventTable.DoStatePlateOpen(GameDbf.Scenario.GetRecord(record.ScenarioId).SortOrder - 1, unlockDelay);
        flag = true;
      }
    }
    if (flag)
      return;
    this.FireOpenPlateEndEvent((Spell) null);
  }

  protected override bool InitializePlateOpenState()
  {
    List<AdventureMissionDbfRecord> recordsForThisWing = this.GetSortedAdventureMissionRecordsForThisWing();
    bool flag = false;
    foreach (AdventureMissionDbfRecord missionDbfRecord in recordsForThisWing)
    {
      int reqProgress = missionDbfRecord.ReqProgress;
      int progressValueForWing = AdventureProgressMgr.Get().GetProgressValueForWing(missionDbfRecord.ReqWingId);
      int ack;
      AdventureProgressMgr.Get().GetWingAck(missionDbfRecord.ReqWingId, out ack);
      if (progressValueForWing >= reqProgress && ack >= progressValueForWing && !this.HasDependentWingJustAckedRequiredProgress())
      {
        this.m_WingEventTable.DoStatePlateAlreadyOpen(GameDbf.Scenario.GetRecord(missionDbfRecord.ScenarioId).SortOrder - 1);
        flag = true;
      }
    }
    return flag;
  }

  private List<AdventureMissionDbfRecord> GetSortedAdventureMissionRecordsForThisWing()
  {
    List<AdventureMissionDbfRecord> records = GameDbf.AdventureMission.GetRecords((Predicate<AdventureMissionDbfRecord>) (r => (WingDbId) r.GrantsWingId == this.m_WingDef.GetWingId()));
    records.OrderBy<AdventureMissionDbfRecord, int>((Func<AdventureMissionDbfRecord, int>) (r => GameDbf.Scenario.GetRecord(r.ScenarioId).SortOrder));
    return records;
  }

  [Serializable]
  public class PlateCoverData
  {
    [SerializeField]
    public PegUIElement PlateCoverHitbox;
    [SerializeField]
    public string TooltipText;
  }
}
