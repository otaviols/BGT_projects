using UnityEngine;

public class TimeDelete : MonoBehaviour
{
  public float m_SecondsToDelete = 10f;
  private float m_StartTime;

  private void Start() => this.m_StartTime = Time.time;

  private void Update()
  {
    if ((double) Time.time <= (double) this.m_StartTime + (double) this.m_SecondsToDelete)
      return;
    Object.Destroy((Object) this.gameObject);
  }
}
