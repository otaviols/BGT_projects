using PegasusShared;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class TimeUtils
{
  public static readonly DateTime EPOCH_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
  public static readonly TimeUtils.ElapsedStringSet SPLASHSCREEN_DATETIME_STRINGSET = new TimeUtils.ElapsedStringSet()
  {
    m_seconds = "GLOBAL_DATETIME_SPLASHSCREEN_SECONDS",
    m_minutes = "GLOBAL_DATETIME_SPLASHSCREEN_MINUTES",
    m_hours = "GLOBAL_DATETIME_SPLASHSCREEN_HOURS",
    m_yesterday = "GLOBAL_DATETIME_SPLASHSCREEN_DAY",
    m_days = "GLOBAL_DATETIME_SPLASHSCREEN_DAYS",
    m_weeks = "GLOBAL_DATETIME_SPLASHSCREEN_WEEKS",
    m_monthAgo = "GLOBAL_DATETIME_SPLASHSCREEN_MONTH"
  };

  public static long BinaryStamp() => DateTime.UtcNow.ToBinary();

  public static DateTime ConvertEpochMicrosecToDateTime(long microsec) => TimeUtils.EPOCH_TIME.AddMilliseconds((double) microsec / 1000.0);

  public static TimeSpan GetElapsedTimeSinceEpoch(DateTime? endDateTime = null) => (endDateTime.HasValue ? endDateTime.Value : DateTime.UtcNow) - TimeUtils.EPOCH_TIME;

  public static long UnixTimestampMilliseconds => (long) TimeUtils.GetElapsedTimeSinceEpoch().TotalMilliseconds;

  public static long UnixTimestampSeconds => (long) TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;

  public static string GetElapsedTimeStringFromEpochMicrosec(
    long microsec,
    TimeUtils.ElapsedStringSet stringSet)
  {
    return TimeUtils.GetElapsedTimeString((int) (DateTime.UtcNow - TimeUtils.ConvertEpochMicrosecToDateTime(microsec)).TotalSeconds, stringSet, false);
  }

  public static ulong DateTimeToUnixTimeStamp(DateTime time) => (ulong) (time.ToUniversalTime() - TimeUtils.EPOCH_TIME).TotalSeconds;

  public static ulong DateTimeToUnixTimeStampMilliseconds(DateTime time) => (ulong) (time.ToUniversalTime() - TimeUtils.EPOCH_TIME).TotalMilliseconds;

  public static DateTime UnixTimeStampToDateTimeUtc(long secondsSinceEpoch) => TimeUtils.EPOCH_TIME.AddSeconds((double) secondsSinceEpoch);

  public static DateTime UnixTimeStampMillisecondsToDateTimeUtc(long millisecondsSinceEpoch) => TimeUtils.EPOCH_TIME.AddMilliseconds((double) millisecondsSinceEpoch);

  public static DateTime UnixTimeStampToDateTimeLocal(long secondsSinceEpoch)
  {
    DateTime dateTime = TimeUtils.EPOCH_TIME;
    dateTime = dateTime.AddSeconds((double) secondsSinceEpoch);
    return dateTime.ToLocalTime();
  }

  public static string GetCountdownTimerString(TimeSpan timeRemaining, bool getFinalSeconds = false) => timeRemaining.Days > 0 ? GameStrings.Format("GLOBAL_DATETIME_TIMER_DAYS_HOURS", (object) timeRemaining.Days, (object) timeRemaining.Hours) : (timeRemaining.Hours > 0 ? GameStrings.Format("GLOBAL_DATETIME_TIMER_HOURS_MINUTES", (object) timeRemaining.Hours, (object) timeRemaining.Minutes) : (timeRemaining.Minutes > 0 || !getFinalSeconds ? GameStrings.Format(getFinalSeconds || timeRemaining.Minutes != 0 ? "GLOBAL_DATETIME_TIMER_MINUTES" : "GLOBAL_DATETIME_TIMER_LESS_THAN_X_MINUTES", (object) Math.Max(timeRemaining.Minutes, 1)) : GameStrings.Format("GLOBAL_DATETIME_TIMER_SECONDS", (object) timeRemaining.Seconds)));

  public static string GetElapsedTimeString(
    long seconds,
    TimeUtils.ElapsedStringSet stringSet,
    bool roundUp = false)
  {
    TimeUtils.ElapsedTimeType timeType;
    long time;
    if (roundUp)
      TimeUtils.GetElapsedTimeRoundedUp(seconds, out timeType, out time);
    else
      TimeUtils.GetElapsedTimeRoundedDown(seconds, out timeType, out time);
    return TimeUtils.GetElapsedTimeString(timeType, time, stringSet);
  }

  public static string GetElapsedTimeString(
    int seconds,
    TimeUtils.ElapsedStringSet stringSet,
    bool roundUp = false)
  {
    return TimeUtils.GetElapsedTimeString((long) seconds, stringSet, roundUp);
  }

  public static string GetElapsedTimeString(
    TimeUtils.ElapsedTimeType timeType,
    int time,
    TimeUtils.ElapsedStringSet stringSet)
  {
    return TimeUtils.GetElapsedTimeString(timeType, (long) time, stringSet);
  }

  public static string GetElapsedTimeString(
    TimeUtils.ElapsedTimeType timeType,
    long time,
    TimeUtils.ElapsedStringSet stringSet)
  {
    switch (timeType)
    {
      case TimeUtils.ElapsedTimeType.SECONDS:
        if (stringSet.m_seconds == null)
        {
          time = 1L;
          goto case TimeUtils.ElapsedTimeType.MINUTES;
        }
        else
          return GameStrings.Format(stringSet.m_seconds, (object) time);
      case TimeUtils.ElapsedTimeType.MINUTES:
        if (stringSet.m_minutes == null)
        {
          time = 1L;
          goto case TimeUtils.ElapsedTimeType.HOURS;
        }
        else
          return GameStrings.Format(stringSet.m_minutes, (object) time);
      case TimeUtils.ElapsedTimeType.HOURS:
        if (stringSet.m_hours == null)
        {
          time = 1L;
          goto case TimeUtils.ElapsedTimeType.YESTERDAY;
        }
        else
          return GameStrings.Format(stringSet.m_hours, (object) time);
      case TimeUtils.ElapsedTimeType.YESTERDAY:
        if (stringSet.m_yesterday != null)
          return GameStrings.Get(stringSet.m_yesterday);
        time = 1L;
        goto case TimeUtils.ElapsedTimeType.DAYS;
      case TimeUtils.ElapsedTimeType.DAYS:
        if (stringSet.m_days == null)
        {
          time = 1L;
          goto case TimeUtils.ElapsedTimeType.WEEKS;
        }
        else
          return GameStrings.Format(stringSet.m_days, (object) time);
      case TimeUtils.ElapsedTimeType.WEEKS:
        if (stringSet.m_weeks == null)
        {
          time = 1L;
          break;
        }
        return GameStrings.Format(stringSet.m_weeks, (object) time);
    }
    return GameStrings.Format(stringSet.m_monthAgo, (object) time);
  }

  public static void GetElapsedTime(
    long seconds,
    out TimeUtils.ElapsedTimeType timeType,
    out int time,
    bool roundUp = false)
  {
    long time1;
    if (roundUp)
      TimeUtils.GetElapsedTimeRoundedUp(seconds, out timeType, out time1);
    else
      TimeUtils.GetElapsedTimeRoundedDown(seconds, out timeType, out time1);
    time = (int) time1;
  }

  private static void GetElapsedTimeRoundedDown(
    long seconds,
    out TimeUtils.ElapsedTimeType timeType,
    out long time)
  {
    time = 0L;
    if (seconds < 60L)
    {
      timeType = TimeUtils.ElapsedTimeType.SECONDS;
      time = seconds;
    }
    else if (seconds < 3600L)
    {
      timeType = TimeUtils.ElapsedTimeType.MINUTES;
      time = seconds / 60L;
    }
    else
    {
      long num1 = seconds / 86400L;
      switch (num1)
      {
        case 0:
          timeType = TimeUtils.ElapsedTimeType.HOURS;
          time = seconds / 3600L;
          break;
        case 1:
          timeType = TimeUtils.ElapsedTimeType.YESTERDAY;
          break;
        default:
          long num2 = seconds / 604800L;
          if (num2 == 0L)
          {
            timeType = TimeUtils.ElapsedTimeType.DAYS;
            time = num1;
            break;
          }
          long num3 = seconds / 2592000L;
          if (num3 == 0L)
          {
            timeType = TimeUtils.ElapsedTimeType.WEEKS;
            time = num2;
            break;
          }
          timeType = TimeUtils.ElapsedTimeType.MONTH_AGO;
          time = num3;
          break;
      }
    }
  }

  private static void GetElapsedTimeRoundedUp(
    long seconds,
    out TimeUtils.ElapsedTimeType timeType,
    out long time)
  {
    time = 0L;
    long num1 = seconds / 60L;
    long num2 = seconds / 3600L;
    long num3 = seconds / 86400L;
    long num4 = seconds / 604800L;
    long num5 = seconds / 2592000L;
    if (num5 > 0L)
    {
      timeType = TimeUtils.ElapsedTimeType.MONTH_AGO;
      time = num5 + 1L;
    }
    else if (num4 > 0L)
    {
      timeType = TimeUtils.ElapsedTimeType.WEEKS;
      time = num4 + 1L;
    }
    else if (num3 > 0L)
    {
      timeType = TimeUtils.ElapsedTimeType.DAYS;
      time = num3 + 1L;
    }
    else if (num2 > 0L)
    {
      timeType = TimeUtils.ElapsedTimeType.HOURS;
      time = num2 + 1L;
    }
    else if (num1 > 0L)
    {
      timeType = TimeUtils.ElapsedTimeType.MINUTES;
      time = num1 + 1L;
    }
    else
    {
      timeType = TimeUtils.ElapsedTimeType.SECONDS;
      time = seconds;
    }
  }

  public static string GetDevElapsedTimeString(TimeSpan span) => TimeUtils.GetDevElapsedTimeString((long) span.TotalMilliseconds);

  public static string GetDevElapsedTimeString(long ms)
  {
    StringBuilder builder = new StringBuilder();
    int unitCount = 0;
    if (ms >= 3600000L)
      TimeUtils.AppendDevTimeUnitsString("{0}h", 3600000, builder, ref ms, ref unitCount);
    if (ms >= 60000L)
      TimeUtils.AppendDevTimeUnitsString("{0}m", 60000, builder, ref ms, ref unitCount);
    if (ms >= 1000L)
      TimeUtils.AppendDevTimeUnitsString("{0}s", 1000, builder, ref ms, ref unitCount);
    if (unitCount <= 1)
    {
      if (unitCount > 0)
        builder.Append(' ');
      builder.AppendFormat("{0}ms", (object) ms);
    }
    return builder.ToString();
  }

  public static string GetDevElapsedTimeString(float sec)
  {
    StringBuilder builder = new StringBuilder();
    int unitCount = 0;
    if ((double) sec >= 3600.0)
      TimeUtils.AppendDevTimeUnitsString("{0}h", 3600f, builder, ref sec, ref unitCount);
    if ((double) sec >= 60.0)
      TimeUtils.AppendDevTimeUnitsString("{0}m", 60f, builder, ref sec, ref unitCount);
    if ((double) sec >= 1.0)
      TimeUtils.AppendDevTimeUnitsString("{0}s", 1f, builder, ref sec, ref unitCount);
    if (unitCount <= 1)
    {
      if (unitCount > 0)
        builder.Append(' ');
      float num = sec * 1000f;
      if ((double) num > 0.0)
        builder.AppendFormat("{0:f0}ms", (object) num);
      else
        builder.AppendFormat("{0}ms", (object) num);
    }
    return builder.ToString();
  }

  public static bool TryParseDevSecFromElapsedTimeString(string timeStr, out float sec)
  {
    sec = 0.0f;
    MatchCollection matchCollection = Regex.Matches(timeStr, "(?<number>(?:[0-9]+,)*[0-9]+)\\s*(?<units>[a-zA-Z]+)");
    if (matchCollection.Count == 0)
      return false;
    Match match = matchCollection[0];
    if (!match.Groups[0].Success)
      return false;
    Group group1 = match.Groups["number"];
    Group group2 = match.Groups["units"];
    if (!group1.Success || !group2.Success)
      return false;
    string s = group1.Value;
    string unitsStr = group2.Value;
    ref float local = ref sec;
    if (!float.TryParse(s, out local))
      return false;
    string timeUnitsStr = TimeUtils.ParseTimeUnitsStr(unitsStr);
    if (timeUnitsStr == "min")
      sec *= 60f;
    else if (timeUnitsStr == "hour")
      sec *= 3600f;
    return true;
  }

  public static long PegDateToFileTimeUtc(Date date) => new DateTime(date.Year, date.Month, date.Day, date.Hours, date.Min, date.Sec).ToFileTimeUtc();

  public static Date FileTimeUtcToPegDate(long fileTimeUtc)
  {
    DateTime dateTime = DateTime.FromFileTimeUtc(fileTimeUtc);
    return new Date()
    {
      Year = dateTime.Year,
      Month = dateTime.Month,
      Day = dateTime.Day,
      Hours = dateTime.Hour,
      Min = dateTime.Minute,
      Sec = dateTime.Second
    };
  }

  public static string GetComingSoonText(SpecialEventType comingSoonEvent)
  {
    DateTime? eventEndTimeUtc = SpecialEventManager.Get().GetEventEndTimeUtc(comingSoonEvent);
    DateTime utcNow = DateTime.UtcNow;
    TimeSpan? nullable = eventEndTimeUtc.HasValue ? new TimeSpan?(eventEndTimeUtc.GetValueOrDefault() - utcNow) : new TimeSpan?();
    if (!nullable.HasValue)
      return GameStrings.Get("GLOBAL_DATETIME_COMING_SOON");
    TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
    {
      m_minutes = "GLOBAL_DATETIME_COMING_SOON_MINUTES",
      m_hours = "GLOBAL_DATETIME_COMING_SOON_HOURS",
      m_days = "GLOBAL_DATETIME_COMING_SOON_DAYS",
      m_weeks = "GLOBAL_DATETIME_COMING_SOON_WEEKS",
      m_monthAgo = "GLOBAL_DATETIME_COMING_SOON"
    };
    return TimeUtils.GetElapsedTimeString((long) nullable.Value.TotalSeconds, stringSet, true);
  }

  private static void AppendDevTimeUnitsString(
    string formatString,
    int msPerUnit,
    StringBuilder builder,
    ref long ms,
    ref int unitCount)
  {
    long num = ms / (long) msPerUnit;
    if (num > 0L)
    {
      if (unitCount > 0)
        builder.Append(' ');
      builder.AppendFormat(formatString, (object) num);
      ++unitCount;
    }
    ms -= num * (long) msPerUnit;
  }

  private static void AppendDevTimeUnitsString(
    string formatString,
    float secPerUnit,
    StringBuilder builder,
    ref float sec,
    ref int unitCount)
  {
    float num = Mathf.Floor(sec / secPerUnit);
    if ((double) num > 0.0)
    {
      if (unitCount > 0)
        builder.Append(' ');
      builder.AppendFormat(formatString, (object) num);
      ++unitCount;
    }
    sec -= num * secPerUnit;
  }

  private static string ParseTimeUnitsStr(string unitsStr)
  {
    if (unitsStr == null)
      return "sec";
    unitsStr = unitsStr.ToLowerInvariant();
    switch (unitsStr)
    {
      case "h":
      case "hour":
      case "hours":
        return "hour";
      case "m":
      case "min":
      case "mins":
      case "minute":
      case "minutes":
        return "min";
      case "s":
      case "sec":
      case "second":
      case "seconds":
      case "secs":
        return "sec";
      default:
        return "sec";
    }
  }

  public enum ElapsedTimeType
  {
    SECONDS,
    MINUTES,
    HOURS,
    YESTERDAY,
    DAYS,
    WEEKS,
    MONTH_AGO,
  }

  public class ElapsedStringSet
  {
    public string m_seconds;
    public string m_minutes;
    public string m_hours;
    public string m_yesterday;
    public string m_days;
    public string m_weeks;
    public string m_monthAgo;
  }
}
