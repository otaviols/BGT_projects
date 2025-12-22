using UnityEngine;

public class CameraShaker : MonoBehaviour
{
  public Vector3 m_Amount;
  public AnimationCurve m_IntensityCurve;
  public bool m_Hold;
  public float m_HoldAtSec;
  public bool m_IsOverride;

  public void StartShake()
  {
    float? holdAtTime = new float?();
    if (this.m_Hold)
      holdAtTime = new float?(this.m_HoldAtSec);
    CameraShakeMgr.Shake(Camera.main, this.m_Amount, this.m_IntensityCurve, holdAtTime, this.m_IsOverride);
  }
}
