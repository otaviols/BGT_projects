using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class UberShuriken : MonoBehaviour
{
  private const int VORTEX_NOISE_INVERVAL = 3;
  private const int FOLLOW_CURVE_INVERVAL = 3;
  private const int CURL_NOISE_INVERVAL = 3;
  public bool m_IncludeChildren;
  public UberCurve m_UberCurve;
  public bool m_FollowCurveDirection;
  public bool m_FollowCurvePosition;
  public float m_FollowCurvePositionAttraction = 0.5f;
  public float m_FollowCurvePositionIntensity = 1.7f;
  public AnimationCurve m_FollowCurvePositionOverLifetime = new AnimationCurve(new Keyframe[2]
  {
    new Keyframe(0.0f, 1f),
    new Keyframe(1f, 1f)
  });
  public bool m_CurlNoise;
  public float m_CurlNoisePower = 1f;
  public AnimationCurve m_CurlNoiseOverLifetime = new AnimationCurve(new Keyframe[2]
  {
    new Keyframe(0.0f, 0.0f),
    new Keyframe(1f, 1f)
  });
  public float m_CurlNoiseScale = 1f;
  public Vector3 m_CurlNoiseAnimation = Vector3.zero;
  public float m_CurlNoiseGizmoSize = 1f;
  public bool m_Twinkle;
  public float m_TwinkleRate = 1f;
  [Range(-1f, 1f)]
  public float m_TwinkleBias;
  public AnimationCurve m_TwinkleOverLifetime = new AnimationCurve(new Keyframe[2]
  {
    new Keyframe(0.0f, 0.0f),
    new Keyframe(1f, 1f)
  });
  private List<ParticleSystem> m_particleSystems = new List<ParticleSystem>();
  private ParticleSystem.Particle[] m_particles;
  private float m_time;
  private int m_followCurveIntervalIndex = 1;
  private int m_curlNoiseIntervalIndex = 2;

  private void Awake()
  {
    if ((Object) this.m_UberCurve == (Object) null)
      this.m_UberCurve = this.GetComponent<UberCurve>();
    this.UpdateParticleSystemList();
  }

  private void Update()
  {
    this.m_time = Time.time;
    this.UpdateParticles();
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.color = Color.blue;
    if (this.m_CurlNoise && (double) this.m_CurlNoiseGizmoSize > 0.0)
    {
      int num1 = 10;
      float num2 = Mathf.Max(Mathf.Abs(this.m_CurlNoiseScale * 0.25f), 1f) * this.m_CurlNoiseGizmoSize;
      float num3 = (float) (1.0 / ((double) num1 * 1.20000004768372));
      float num4 = 1f;
      float num5 = 0.0f;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        Gizmos.color = new Color(0.0f, 0.0f, 1f - num4, 1f);
        num4 -= num3;
        float num6 = (float) index1 * 0.75f;
        Vector4[] vector4Array1 = this.GizmoCirclePoints(20 * Mathf.Max(Mathf.FloorToInt(Mathf.Abs(this.m_CurlNoiseScale)), 10), num6 * num2);
        Vector4 from = vector4Array1[vector4Array1.Length - 1];
        for (int index2 = 0; index2 < vector4Array1.Length; ++index2)
        {
          Gizmos.color = new Color(vector4Array1[index2].w * 0.5f, vector4Array1[index2].w, 1f, 1f);
          Gizmos.DrawLine((Vector3) from, (Vector3) vector4Array1[index2]);
          from = vector4Array1[index2];
        }
        Vector4[] vector4Array2 = this.GizmoCircleLines(10, num5 * num2, num6 * num2);
        for (int index3 = 0; index3 < vector4Array2.Length; index3 += 2)
        {
          Gizmos.color = new Color(vector4Array2[index3].w * 0.5f, vector4Array2[index3].w, 1f, 1f);
          Gizmos.DrawLine((Vector3) vector4Array2[index3], (Vector3) vector4Array2[index3 + 1]);
        }
        num5 = num6;
      }
    }
    Gizmos.matrix = Matrix4x4.identity;
  }

  private Vector4[] GizmoCirclePoints(int numOfPoints, float radius)
  {
    Vector4[] vector4Array = new Vector4[numOfPoints];
    float f = 0.0f;
    float num = 6.283185f / (float) numOfPoints;
    for (int index = 0; index < numOfPoints; ++index)
    {
      f += num;
      vector4Array[index] = this.GizmoCurlNoisePoint(new Vector3(Mathf.Cos(f) * radius, Mathf.Sin(f) * radius, 0.0f));
    }
    return vector4Array;
  }

  private Vector4[] GizmoCircleLines(int numOfPoints, float previousRadius, float radius)
  {
    int length = numOfPoints * 2;
    Vector4[] vector4Array = new Vector4[length];
    float f = 0.0f;
    float num = 6.283f / (float) numOfPoints;
    for (int index = 0; index < length; index += 2)
    {
      f += num;
      vector4Array[index] = this.GizmoCurlNoisePoint(new Vector3(Mathf.Cos(f) * previousRadius, Mathf.Sin(f) * previousRadius, 0.0f));
      vector4Array[index + 1] = this.GizmoCurlNoisePoint(new Vector3(Mathf.Cos(f) * radius, Mathf.Sin(f) * radius, 0.0f));
    }
    return vector4Array;
  }

  private Vector4 GizmoCurlNoisePoint(Vector3 point)
  {
    float time = this.m_time;
    float num1 = this.m_CurlNoiseAnimation.x * time;
    float num2 = this.m_CurlNoiseAnimation.y * time;
    float num3 = this.m_CurlNoiseAnimation.z * time;
    Vector3 vector3_1 = point * this.m_CurlNoiseScale * 0.1f;
    float a1 = UberMath.SimplexNoise(5f + vector3_1.x + num1, vector3_1.y + num2, vector3_1.z + num3) * this.m_CurlNoisePower;
    float a2 = UberMath.SimplexNoise(6f + vector3_1.y + num1, vector3_1.z + num2, vector3_1.x + num3) * this.m_CurlNoisePower;
    float b = UberMath.SimplexNoise(7f + vector3_1.z + num1, vector3_1.x + num2, vector3_1.y + num3) * this.m_CurlNoisePower;
    Vector3 vector3_2 = new Vector3(point.x + a1, point.y + a2, point.z + b);
    float w = Mathf.Max(a1, Mathf.Max(a2, b));
    return new Vector4(vector3_2.x, vector3_2.y, vector3_2.z, w);
  }

  private void UpdateParticles()
  {
    this.m_followCurveIntervalIndex = this.m_followCurveIntervalIndex + 1 > 3 ? (this.m_followCurveIntervalIndex = 0) : this.m_followCurveIntervalIndex + 1;
    this.m_curlNoiseIntervalIndex = this.m_curlNoiseIntervalIndex + 1 > 3 ? 0 : this.m_curlNoiseIntervalIndex + 1;
    foreach (ParticleSystem particleSystem in this.m_particleSystems)
    {
      if (!((Object) particleSystem == (Object) null))
      {
        int particleCount = particleSystem.particleCount;
        if (particleCount == 0)
          break;
        if (this.m_particles == null || particleCount > this.m_particles.Length)
          this.ResizeParticlesBuffer(particleCount);
        particleSystem.GetParticles(this.m_particles);
        if (this.m_FollowCurveDirection || this.m_FollowCurvePosition)
          this.FollowCurveOverLife(particleSystem, this.m_particles, particleCount);
        if (this.m_CurlNoise)
          this.ParticleCurlNoise(particleSystem, this.m_particles, particleCount);
        if (this.m_Twinkle)
          this.ParticleTwinkle(particleSystem, this.m_particles, particleCount);
        particleSystem.SetParticles(this.m_particles, particleCount);
      }
    }
  }

  private void UpdateParticleSystemList()
  {
    this.m_particleSystems.Clear();
    if (this.m_IncludeChildren)
    {
      ParticleSystem[] componentsInChildren = this.GetComponentsInChildren<ParticleSystem>();
      if ((Object) this.GetComponent<ParticleSystem>() == (Object) null || componentsInChildren.Length == 0)
        Debug.LogError((object) "Failed to find a ParticleSystem");
      foreach (ParticleSystem particleSystem in componentsInChildren)
        this.m_particleSystems.Add(particleSystem);
    }
    else
    {
      ParticleSystem component = this.GetComponent<ParticleSystem>();
      if ((Object) component == (Object) null)
        Debug.LogError((object) "Failed to find a ParticleSystem");
      this.m_particleSystems.Add(component);
    }
  }

  private void ResizeParticlesBuffer(int newCount) => this.m_particles = new ParticleSystem.Particle[newCount];

  private void FollowCurveOverLife(
    ParticleSystem particleSystem,
    ParticleSystem.Particle[] particles,
    int particleCount)
  {
    if ((Object) this.m_UberCurve == (Object) null)
      this.CreateCurve();
    for (int curveIntervalIndex = this.m_followCurveIntervalIndex; curveIntervalIndex < particleCount; curveIntervalIndex += 3)
    {
      float position = (float) (1.0 - (double) particles[curveIntervalIndex].remainingLifetime / (double) particles[curveIntervalIndex].startLifetime);
      if (this.m_FollowCurvePosition)
      {
        Vector3 zero = Vector3.zero;
        Vector3 b = (particleSystem.main.simulationSpace != ParticleSystemSimulationSpace.World ? this.m_UberCurve.CatmullRomEvaluateLocalPosition(position) : this.m_UberCurve.CatmullRomEvaluateWorldPosition(position)) - particles[curveIntervalIndex].position;
        Vector3 vector3 = Vector3.Lerp(particles[curveIntervalIndex].velocity, b, this.m_FollowCurvePositionAttraction);
        particles[curveIntervalIndex].velocity = vector3 * this.m_FollowCurvePositionIntensity;
      }
      if (this.m_FollowCurveDirection)
      {
        Vector3 vector3 = this.m_UberCurve.CatmullRomEvaluateDirection(position).normalized * particles[curveIntervalIndex].velocity.magnitude;
        particles[curveIntervalIndex].velocity = vector3;
      }
    }
  }

  private void CreateCurve()
  {
    if ((Object) this.m_UberCurve != (Object) null)
      return;
    this.m_UberCurve = this.GetComponent<UberCurve>();
    if ((Object) this.m_UberCurve != (Object) null)
      return;
    this.m_UberCurve = this.gameObject.AddComponent<UberCurve>();
  }

  private void ParticleCurlNoise(
    ParticleSystem particleSystem,
    ParticleSystem.Particle[] particles,
    int particleCount)
  {
    float time = this.m_time;
    float num1 = this.m_CurlNoiseAnimation.x * time;
    float num2 = this.m_CurlNoiseAnimation.y * time;
    float num3 = this.m_CurlNoiseAnimation.z * time;
    for (int noiseIntervalIndex = this.m_curlNoiseIntervalIndex; noiseIntervalIndex < particleCount; noiseIntervalIndex += 3)
    {
      float num4 = this.m_CurlNoiseOverLifetime.Evaluate((float) (1.0 - (double) particles[noiseIntervalIndex].remainingLifetime / (double) particles[noiseIntervalIndex].startLifetime)) * this.m_CurlNoisePower;
      Vector3 velocity = particles[noiseIntervalIndex].velocity;
      Vector3 vector3_1 = particles[noiseIntervalIndex].position * this.m_CurlNoiseScale * 0.1f;
      velocity.x += UberMath.SimplexNoise(5f + vector3_1.x + num1, vector3_1.y + num2, vector3_1.z + num3) * num4;
      velocity.y += UberMath.SimplexNoise(6f + vector3_1.y + num1, vector3_1.z + num2, vector3_1.x + num3) * num4;
      velocity.z += UberMath.SimplexNoise(7f + vector3_1.z + num1, vector3_1.x + num2, vector3_1.y + num3) * num4;
      Vector3 vector3_2 = velocity.normalized * particles[noiseIntervalIndex].velocity.magnitude;
      particles[noiseIntervalIndex].velocity = vector3_2;
    }
  }

  private void ParticleTwinkle(
    ParticleSystem particleSystem,
    ParticleSystem.Particle[] particles,
    int particleCount)
  {
    for (int index = 0; index < particleCount; ++index)
    {
      float time = particles[index].remainingLifetime / particles[index].startLifetime;
      Vector3 position = particles[index].position;
      Color startColor = (Color) particles[index].startColor with
      {
        a = Mathf.Clamp01((float) ((double) UberMath.SimplexNoise((float) ((double) position.x + (double) position.y + (double) position.z - (double) time - (double) index * 3.32999992370605) * this.m_TwinkleRate, 0.5f) + (double) this.m_TwinkleBias + (double) time * (double) this.m_TwinkleOverLifetime.Evaluate(time)))
      };
      particles[index].startColor = (Color32) startColor;
    }
  }
}
