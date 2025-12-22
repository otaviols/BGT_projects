using UnityEngine;

public static class SwrveLog
{
  public static SwrveLog.LogLevel Level = SwrveLog.LogLevel.Error;

  public static void Log(object message) => SwrveLog.Log(message, "activity");

  public static void LogInfo(object message) => SwrveLog.LogInfo(message, "activity");

  public static void LogWarning(object message) => SwrveLog.LogWarning(message, "activity");

  public static void LogError(object message) => SwrveLog.LogError(message, "activity");

  public static void Log(object message, string tag)
  {
    if (SwrveLog.Level != SwrveLog.LogLevel.Verbose)
      return;
    Debug.Log(message);
    // ISSUE: reference to a compiler-generated field
    if (SwrveLog.OnLog == null)
      return;
    // ISSUE: reference to a compiler-generated field
    SwrveLog.OnLog(SwrveLog.LogLevel.Verbose, message, tag);
  }

  public static void LogInfo(object message, string tag)
  {
    if (SwrveLog.Level != SwrveLog.LogLevel.Verbose && SwrveLog.Level != SwrveLog.LogLevel.Info)
      return;
    Debug.Log(message);
    // ISSUE: reference to a compiler-generated field
    if (SwrveLog.OnLog == null)
      return;
    // ISSUE: reference to a compiler-generated field
    SwrveLog.OnLog(SwrveLog.LogLevel.Info, message, tag);
  }

  public static void LogWarning(object message, string tag)
  {
    if (SwrveLog.Level != SwrveLog.LogLevel.Verbose && SwrveLog.Level != SwrveLog.LogLevel.Info && SwrveLog.Level != SwrveLog.LogLevel.Warning)
      return;
    Debug.LogWarning(message);
    // ISSUE: reference to a compiler-generated field
    if (SwrveLog.OnLog == null)
      return;
    // ISSUE: reference to a compiler-generated field
    SwrveLog.OnLog(SwrveLog.LogLevel.Warning, message, tag);
  }

  public static void LogError(object message, string tag)
  {
    if (SwrveLog.Level != SwrveLog.LogLevel.Verbose && SwrveLog.Level != SwrveLog.LogLevel.Info && SwrveLog.Level != SwrveLog.LogLevel.Warning && SwrveLog.Level != SwrveLog.LogLevel.Error)
      return;
    Debug.LogError(message);
    // ISSUE: reference to a compiler-generated field
    if (SwrveLog.OnLog == null)
      return;
    // ISSUE: reference to a compiler-generated field
    SwrveLog.OnLog(SwrveLog.LogLevel.Error, message, tag);
  }

  public enum LogLevel
  {
    Verbose,
    Info,
    Warning,
    Error,
    Disabled,
  }

  public delegate void SwrveLogEventHandler(SwrveLog.LogLevel level, object message, string tag);
}
