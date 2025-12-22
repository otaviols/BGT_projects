using Blizzard.T5.Core;
using System.Collections.Generic;

public class PowerHistoryTimeline
{
  public int m_firstTaskId;
  public int m_lastTaskId;
  public int m_slushTime;
  public float m_startTime;
  public float m_endTime;
  public List<PowerHistoryTimelineEntry> m_orderedEvents = new List<PowerHistoryTimelineEntry>();
  public Map<int, int> m_orderedEventIndexLookup = new Map<int, int>();

  public void AddTimelineEntry(int taskId, int slushTime)
  {
    PowerHistoryTimelineEntry historyTimelineEntry = new PowerHistoryTimelineEntry();
    historyTimelineEntry.taskId = taskId;
    historyTimelineEntry.expectedTime = slushTime;
    if (this.m_orderedEvents.Count == 0)
    {
      historyTimelineEntry.expectedStartOffset = 0;
    }
    else
    {
      PowerHistoryTimelineEntry orderedEvent = this.m_orderedEvents[this.m_orderedEvents.Count - 1];
      historyTimelineEntry.expectedStartOffset = orderedEvent.expectedStartOffset + orderedEvent.expectedTime;
    }
    this.m_orderedEvents.Add(historyTimelineEntry);
    this.m_orderedEventIndexLookup.Add(taskId, this.m_orderedEvents.Count - 1);
  }
}
