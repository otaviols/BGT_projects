using UnityEngine;

public class RandomPlayAnimation : MonoBehaviour
{
  public float m_MinWaitTime;
  public float m_MaxWaitTime = 10f;
  private float m_waitTime = -1f;
  private float m_startTime;
  private Animation m_animation;

  private void Start() => this.m_animation = this.gameObject.GetComponent<Animation>();

  private void Update()
  {
    if ((Object) this.m_animation == (Object) null)
      this.enabled = false;
    if ((double) this.m_waitTime < 0.0)
    {
      if ((double) this.m_MinWaitTime < 0.0)
        this.m_MinWaitTime = 0.0f;
      if ((double) this.m_MaxWaitTime < 0.0)
        this.m_MaxWaitTime = 0.0f;
      if ((double) this.m_MaxWaitTime < (double) this.m_MinWaitTime)
        this.m_MaxWaitTime = this.m_MinWaitTime;
      this.m_waitTime = Random.Range(this.m_MinWaitTime, this.m_MaxWaitTime);
      this.m_startTime = Time.time;
    }
    if ((double) Time.time - (double) this.m_startTime <= (double) this.m_waitTime)
      return;
    this.m_waitTime = -1f;
    this.m_animation.Play();
  }
}
