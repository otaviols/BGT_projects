using UnityEngine;

public static class PegasusUtils
{
  private static void UseStackTraceLoggingMinimum()
  {
    Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
    Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
    Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
  }

  public static void SetStackTraceLoggingOptions(bool forceUseMinimumLogging)
  {
    if (forceUseMinimumLogging)
      PegasusUtils.UseStackTraceLoggingMinimum();
    else
      PegasusUtils.UseStackTraceLoggingMinimum();
  }
}
