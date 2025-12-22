using UnityEngine;

public class MulliganTimer : MonoBehaviour
{
  public UberText m_timeText;
  private bool m_remainingTimeSet;
  private float m_endTimeStamp;

  private void Start()
  {
    if ((Object) MulliganManager.Get() == (Object) null)
      return;
    this.transform.position = this.GetNewPosition();
  }

  private void Update()
  {
    if (!this.m_remainingTimeSet)
      return;
    Vector3 newPosition = this.GetNewPosition();
    if (newPosition != this.transform.position)
      this.transform.position = newPosition;
    double countdownRemainingSec = (double) this.ComputeCountdownRemainingSec();
    int num = Mathf.RoundToInt((float) countdownRemainingSec);
    if (num < 0)
      num = 0;
    this.m_timeText.Text = string.Format(":{0:D2}", (object) num);
    if (countdownRemainingSec > 0.0)
      return;
    if ((bool) (Object) MulliganManager.Get())
      MulliganManager.Get().AutomaticContinueMulligan();
    else
      this.SelfDestruct();
  }

  private Vector3 GetNewPosition()
  {
    if ((Object) MulliganManager.Get() == (Object) null)
      return new Vector3(100f, 0.0f, 0.0f);
    Vector3 newPosition = MulliganManager.Get().GetMulliganTimerPosition();
    newPosition = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(newPosition.x, newPosition.y, newPosition.z - 1f) : new Vector3(newPosition.x + 1.8f, newPosition.y, newPosition.z);
    return newPosition;
  }

  private float ComputeCountdownRemainingSec()
  {
    float num = this.m_endTimeStamp - Time.realtimeSinceStartup;
    return (double) num < 0.0 ? 0.0f : num;
  }

  public void SetEndTime(float endTimeStamp)
  {
    this.m_endTimeStamp = endTimeStamp;
    this.m_remainingTimeSet = true;
  }

  public void SelfDestruct() => Object.Destroy((Object) this.gameObject);
}
