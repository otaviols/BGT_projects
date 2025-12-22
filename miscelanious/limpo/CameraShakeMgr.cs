using UnityEngine;

public class CameraShakeMgr : MonoBehaviour
{
  private Vector3 m_amount;
  private AnimationCurve m_intensityCurve;
  private float? m_holdAtSec;
  private bool m_isOverride;
  private bool m_started;
  private Vector3 m_initialPos;
  private float m_progressSec;
  private float m_durationSec;

  public bool IsCameraShaking => this.m_started;

  private void Update()
  {
    if (!this.m_started)
      return;
    if ((double) this.m_progressSec >= (double) this.m_durationSec && !this.IsHolding())
      this.StopShake();
    else
      this.UpdateShake();
  }

  public static void Shake(
    Camera camera,
    Vector3 amount,
    AnimationCurve intensityCurve,
    float? holdAtTime = null,
    bool isOverride = false)
  {
    if (!(bool) (Object) camera || !Options.Get().GetBool(Option.SCREEN_SHAKE_ENABLED))
      return;
    CameraShakeMgr component;
    if (camera.TryGetComponent<CameraShakeMgr>(out component))
    {
      if (CameraShakeMgr.DoesCurveHaveZeroTime(intensityCurve))
      {
        component.StopShake();
        return;
      }
    }
    else
    {
      if (CameraShakeMgr.DoesCurveHaveZeroTime(intensityCurve))
        return;
      component = camera.gameObject.AddComponent<CameraShakeMgr>();
    }
    component.StartShake(amount, intensityCurve, holdAtTime, isOverride);
  }

  public static void Shake(Camera camera, Vector3 amount, float time)
  {
    AnimationCurve intensityCurve = AnimationCurve.Linear(0.0f, 1f, time, 0.0f);
    CameraShakeMgr.Shake(camera, amount, intensityCurve);
  }

  public static void Stop(Camera camera, float time = 0.0f)
  {
    CameraShakeMgr component;
    if (!(bool) (Object) camera || !Options.Get().GetBool(Option.SCREEN_SHAKE_ENABLED) || !camera.TryGetComponent<CameraShakeMgr>(out component))
      return;
    if ((double) time <= 0.0)
    {
      component.StopShake();
    }
    else
    {
      AnimationCurve intensityCurve = AnimationCurve.Linear(0.0f, component.ComputeIntensity(), time, 0.0f);
      component.StartShake(component.m_amount, intensityCurve);
    }
  }

  public static bool IsShaking(Camera camera)
  {
    CameraShakeMgr component;
    return (bool) (Object) camera && camera.TryGetComponent<CameraShakeMgr>(out component) && component.IsCameraShaking;
  }

  private static bool DoesCurveHaveZeroTime(AnimationCurve intensityCurve) => intensityCurve == null || intensityCurve.length == 0 || (double) intensityCurve[intensityCurve.length - 1].time <= 0.0;

  private void StartShake(
    Vector3 amount,
    AnimationCurve intensityCurve,
    float? holdAtSec = null,
    bool isOverride = false)
  {
    if (!isOverride && (double) amount.sqrMagnitude < (double) this.m_amount.sqrMagnitude || this.m_isOverride)
      return;
    this.m_amount = amount;
    this.m_intensityCurve = intensityCurve;
    this.m_holdAtSec = holdAtSec;
    this.m_isOverride = isOverride;
    if (!this.m_started)
    {
      this.m_started = true;
      this.m_initialPos = this.transform.position;
    }
    this.m_progressSec = 0.0f;
    this.m_durationSec = intensityCurve[intensityCurve.length - 1].time;
  }

  private void StopShake()
  {
    this.transform.position = this.m_initialPos;
    this.m_amount = Vector3.zero;
    this.m_intensityCurve = (AnimationCurve) null;
    this.m_holdAtSec = new float?();
    this.m_isOverride = false;
    this.m_started = false;
  }

  private void UpdateShake()
  {
    float intensity = this.ComputeIntensity();
    this.transform.position = this.m_initialPos + new Vector3()
    {
      x = Random.Range(-this.m_amount.x * intensity, this.m_amount.x * intensity),
      y = Random.Range(-this.m_amount.y * intensity, this.m_amount.y * intensity),
      z = Random.Range(-this.m_amount.z * intensity, this.m_amount.z * intensity)
    };
    if (this.IsHolding())
      return;
    this.m_progressSec = Mathf.Min(this.m_progressSec + Time.deltaTime, this.m_durationSec);
  }

  private float ComputeIntensity() => this.m_intensityCurve != null ? this.m_intensityCurve.Evaluate(this.m_progressSec) : 0.0f;

  private bool IsHolding()
  {
    if (!this.m_holdAtSec.HasValue)
      return false;
    double progressSec = (double) this.m_progressSec;
    float? holdAtSec = this.m_holdAtSec;
    double valueOrDefault = (double) holdAtSec.GetValueOrDefault();
    return progressSec >= valueOrDefault & holdAtSec.HasValue;
  }
}
