using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
  public Transform m_StartPosition;
  public Transform m_TargetObject;
  public MoveToTarget.MoveType m_MoveType;
  public float m_Time = 1f;
  public float m_Speed = 1f;
  public float m_SnapDistance = 0.1f;
  public bool m_OrientToPath;
  public bool m_AnimateOnStart;
  private bool m_Animate;
  private bool m_isDone;
  private Vector3 m_LastTargetPosition;
  private float m_LerpPosition;

  private void Start()
  {
    if (!this.m_AnimateOnStart)
      return;
    this.StartAnimation();
  }

  private void Update()
  {
    if (this.m_MoveType == MoveToTarget.MoveType.MoveByTime)
      this.MoveTime();
    else
      this.MoveSpeed();
  }

  private void MoveTime()
  {
    if (this.m_isDone)
      this.transform.position = this.m_TargetObject.position;
    if (!this.m_Animate)
      return;
    Vector3 position = this.m_TargetObject.position;
    this.m_LerpPosition += 1f / this.m_Time * Time.deltaTime;
    if ((double) this.m_LerpPosition > 1.0)
    {
      this.m_isDone = true;
      this.transform.position = this.m_TargetObject.position;
    }
    else
      this.transform.position = Vector3.Lerp(this.m_StartPosition.position, position, this.m_LerpPosition);
  }

  private void MoveSpeed()
  {
    if (this.m_isDone)
      this.transform.position = this.m_TargetObject.position;
    if (!this.m_Animate)
      return;
    if ((double) Vector3.Distance(this.transform.position, this.m_TargetObject.position) < (double) this.m_SnapDistance)
    {
      this.m_isDone = true;
      this.transform.position = this.m_TargetObject.position;
    }
    else
    {
      Vector3 vector3 = this.m_TargetObject.position - this.transform.position;
      vector3.Normalize();
      float num = this.m_Speed * Time.deltaTime;
      this.transform.position = this.transform.position + vector3 * num;
    }
  }

  private void StartAnimation()
  {
    if ((bool) (Object) this.m_StartPosition)
      this.transform.position = this.m_StartPosition.position;
    this.m_Animate = true;
    this.m_LerpPosition = 0.0f;
  }

  public enum MoveType
  {
    MoveByTime,
    MoveBySpeed,
  }
}
