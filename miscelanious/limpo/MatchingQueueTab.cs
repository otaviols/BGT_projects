using UnityEngine;

public class MatchingQueueTab : MonoBehaviour
{
  public GameObject m_root;
  public UberText m_waitTime;
  public UberText m_queueTime;
  private TimeUtils.ElapsedStringSet m_timeStringSet;
  private float m_timeInQueue;
  private const string TIME_RANGE_STRING = "GLOBAL_APPROXIMATE_DATETIME_RANGE";
  private const string TIME_STRING = "GLOBAL_APPROXIMATE_DATETIME";
  private const int SUPPRESS_TIME = 30;

  private void Update()
  {
    this.InitTimeStringSet();
    this.m_timeInQueue += Time.deltaTime;
    this.m_waitTime.Text = TimeUtils.GetElapsedTimeString(Mathf.RoundToInt(this.m_timeInQueue), this.m_timeStringSet, false);
  }

  public void Show() => this.m_root.SetActive(true);

  public void Hide() => this.m_root.SetActive(false);

  public void ResetTimer()
  {
    this.m_timeInQueue = 0.0f;
    this.UpdateDisplay(0, 0);
  }

  public void UpdateDisplay(int minSeconds, int maxSeconds)
  {
    this.InitTimeStringSet();
    int num = Mathf.RoundToInt(this.m_timeInQueue);
    maxSeconds += num;
    if (maxSeconds <= 30)
    {
      this.Hide();
    }
    else
    {
      this.m_queueTime.Text = this.GetElapsedTimeString(minSeconds + num, maxSeconds);
      this.Show();
    }
  }

  private void InitTimeStringSet()
  {
    if (this.m_timeStringSet != null)
      return;
    this.m_timeStringSet = new TimeUtils.ElapsedStringSet()
    {
      m_seconds = "GLOBAL_DATETIME_SPINNER_SECONDS",
      m_minutes = "GLOBAL_DATETIME_SPINNER_MINUTES",
      m_hours = "GLOBAL_DATETIME_SPINNER_HOURS",
      m_yesterday = "GLOBAL_DATETIME_SPINNER_DAY",
      m_days = "GLOBAL_DATETIME_SPINNER_DAYS",
      m_weeks = "GLOBAL_DATETIME_SPINNER_WEEKS",
      m_monthAgo = "GLOBAL_DATETIME_SPINNER_MONTH"
    };
  }

  private string GetElapsedTimeString(int minSeconds, int maxSeconds)
  {
    TimeUtils.ElapsedTimeType timeType1;
    int time1;
    TimeUtils.GetElapsedTime((long) minSeconds, out timeType1, out time1);
    if (minSeconds == maxSeconds)
      return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME", (object) TimeUtils.GetElapsedTimeString(minSeconds, this.m_timeStringSet, false));
    TimeUtils.ElapsedTimeType timeType2;
    int time2;
    TimeUtils.GetElapsedTime((long) maxSeconds, out timeType2, out time2);
    if (timeType1 == timeType2)
    {
      switch (timeType1)
      {
        case TimeUtils.ElapsedTimeType.SECONDS:
          return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", (object) time1, (object) GameStrings.Format(this.m_timeStringSet.m_seconds, (object) time2));
        case TimeUtils.ElapsedTimeType.MINUTES:
          return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", (object) time1, (object) GameStrings.Format(this.m_timeStringSet.m_minutes, (object) time2));
        case TimeUtils.ElapsedTimeType.HOURS:
          return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", (object) time1, (object) GameStrings.Format(this.m_timeStringSet.m_hours, (object) time2));
        case TimeUtils.ElapsedTimeType.YESTERDAY:
          return GameStrings.Get(this.m_timeStringSet.m_yesterday);
        case TimeUtils.ElapsedTimeType.DAYS:
          return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", (object) time1, (object) GameStrings.Format(this.m_timeStringSet.m_days, (object) time2));
        case TimeUtils.ElapsedTimeType.WEEKS:
          return GameStrings.Format(this.m_timeStringSet.m_weeks, (object) time1, (object) time2);
        default:
          return GameStrings.Get(this.m_timeStringSet.m_monthAgo);
      }
    }
    else
      return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", (object) TimeUtils.GetElapsedTimeString(timeType1, time1, this.m_timeStringSet), (object) TimeUtils.GetElapsedTimeString(timeType2, time2, this.m_timeStringSet));
  }
}
