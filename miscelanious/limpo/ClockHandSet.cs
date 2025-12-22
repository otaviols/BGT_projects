using System;
using UnityEngine;

public class ClockHandSet : MonoBehaviour
{
  public GameObject m_MinuteHand;
  public GameObject m_HourHand;
  private int m_prevMinute;
  private int m_prevHour;

  private void Update()
  {
    DateTime now = DateTime.Now;
    int minute = now.Minute;
    if (minute != this.m_prevMinute)
    {
      this.m_MinuteHand.transform.Rotate(Vector3.up, this.ComputeMinuteRotation(minute) - this.ComputeMinuteRotation(this.m_prevMinute));
      this.m_prevMinute = minute;
    }
    int hour = now.Hour % 12;
    if (hour == this.m_prevHour)
      return;
    this.m_HourHand.transform.Rotate(Vector3.up, this.ComputeHourRotation(hour) - this.ComputeHourRotation(this.m_prevHour));
    this.m_prevHour = hour;
  }

  private float ComputeMinuteRotation(int minute) => (float) minute * 6f;

  private float ComputeHourRotation(int hour) => (float) hour * 30f;
}
