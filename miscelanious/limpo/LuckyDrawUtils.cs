using System;
using System.Collections.Generic;

public class LuckyDrawUtils
{
  public static LuckyDrawBoxDbfRecord GetCurrentLuckyDrawRecord()
  {
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    if (specialEventManager == null)
      return (LuckyDrawBoxDbfRecord) null;
    List<LuckyDrawBoxDbfRecord> records = GameDbf.LuckyDrawBox.GetRecords();
    LuckyDrawBoxDbfRecord currentLuckyDrawRecord = (LuckyDrawBoxDbfRecord) null;
    foreach (LuckyDrawBoxDbfRecord drawBoxDbfRecord in records)
    {
      if (specialEventManager.IsEventActive(drawBoxDbfRecord.Event, false))
      {
        if (currentLuckyDrawRecord == null)
        {
          currentLuckyDrawRecord = drawBoxDbfRecord;
        }
        else
        {
          Error.AddDevWarning("Too Many BattleBashes", "There are at least 2 active BattleBash events active at the same time. Only 1 BattleBash can be active. Check HearthEdit 2 and ensure the start/end dates of the events are not overlapping.");
          return (LuckyDrawBoxDbfRecord) null;
        }
      }
    }
    return currentLuckyDrawRecord;
  }

  public static int GetCurrentLuckyDrawID()
  {
    LuckyDrawBoxDbfRecord currentLuckyDrawRecord = LuckyDrawUtils.GetCurrentLuckyDrawRecord();
    return currentLuckyDrawRecord != null ? currentLuckyDrawRecord.ID : -1;
  }

  public static TimeSpan GetLuckyDrawTimeRemaining(int luckyDrawBoxID)
  {
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    if (specialEventManager == null)
      return new TimeSpan(0L);
    LuckyDrawBoxDbfRecord record = GameDbf.LuckyDrawBox.GetRecord(luckyDrawBoxID);
    return record == null ? new TimeSpan(0L) : specialEventManager.GetTimeLeftForEvent(record.Event);
  }

  public static void ShowErrorAndReturnToLobby()
  {
    SceneMgr.Get();
    if (LuckyDrawUtils.InOrTransitioningToLuckyDrawScene() || LuckyDrawUtils.InOrTransitioningToBattlegroundsLobby())
      Error.AddWarningLoc("GLUE_BATTLEBASH_ERROR_HEADER", "GLUE_BATTLEBASH_ERROR_BODY");
    if (!LuckyDrawUtils.InOrTransitioningToLuckyDrawScene())
      return;
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.BACON, SceneMgr.TransitionHandlerType.NEXT_SCENE);
  }

  private static bool InOrTransitioningToLuckyDrawScene()
  {
    SceneMgr sceneMgr = SceneMgr.Get();
    return sceneMgr.GetMode() == SceneMgr.Mode.LUCKY_DRAW || sceneMgr.GetNextMode() == SceneMgr.Mode.LUCKY_DRAW;
  }

  private static bool InOrTransitioningToBattlegroundsLobby()
  {
    SceneMgr sceneMgr = SceneMgr.Get();
    return sceneMgr.GetMode() == SceneMgr.Mode.BACON || sceneMgr.GetNextMode() == SceneMgr.Mode.BACON;
  }
}
