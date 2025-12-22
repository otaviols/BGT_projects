using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ParticleEffects : MonoBehaviour
{
  public List<ParticleSystem> m_ParticleSystems;
  public bool m_WorldSpace;
  public ParticleEffectsOrientation m_ParticleOrientation;
  public List<ParticleEffectsAttractor> m_ParticleAttractors;
  public List<ParticleEffectsRepulser> m_ParticleRepulsers;

  private void Update()
  {
    if (this.m_ParticleSystems == null)
      return;
    if (this.m_ParticleSystems.Count == 0)
    {
      ParticleSystem component = this.GetComponent<ParticleSystem>();
      if ((Object) component == (Object) null)
        this.enabled = false;
      this.m_ParticleSystems.Add(component);
    }
    for (int index = 0; index < this.m_ParticleSystems.Count; ++index)
    {
      ParticleSystem particleSystem = this.m_ParticleSystems[index];
      if (!((Object) particleSystem == (Object) null))
      {
        int particleCount = particleSystem.particleCount;
        if (particleCount == 0)
          break;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        particleSystem.GetParticles(particles);
        if (this.m_ParticleAttractors != null)
          this.ParticleAttractor(particleSystem, particles, particleCount);
        if (this.m_ParticleRepulsers != null)
          this.ParticleRepulser(particleSystem, particles, particleCount);
        if (this.m_ParticleOrientation != null && this.m_ParticleOrientation.m_OrientToDirection)
          this.OrientParticlesToDirection(particleSystem, particles, particleCount);
        particleSystem.SetParticles(particles, particleCount);
      }
    }
  }

  private void OnDrawGizmos()
  {
    if (this.m_ParticleAttractors != null)
    {
      foreach (ParticleEffectsAttractor particleAttractor in this.m_ParticleAttractors)
      {
        if (!((Object) particleAttractor.m_Transform == (Object) null))
        {
          Gizmos.color = Color.green;
          float radius = particleAttractor.m_Radius * (float) (((double) particleAttractor.m_Transform.lossyScale.x + (double) particleAttractor.m_Transform.lossyScale.y + (double) particleAttractor.m_Transform.lossyScale.z) * 0.333000004291534);
          Gizmos.DrawWireSphere(particleAttractor.m_Transform.position, radius);
        }
      }
    }
    if (this.m_ParticleRepulsers == null)
      return;
    foreach (ParticleEffectsRepulser particleRepulser in this.m_ParticleRepulsers)
    {
      if (!((Object) particleRepulser.m_Transform == (Object) null))
      {
        Gizmos.color = Color.red;
        float radius = particleRepulser.m_Radius * (float) (((double) particleRepulser.m_Transform.lossyScale.x + (double) particleRepulser.m_Transform.lossyScale.y + (double) particleRepulser.m_Transform.lossyScale.z) * 0.333000004291534);
        Gizmos.DrawWireSphere(particleRepulser.m_Transform.position, radius);
      }
    }
  }

  private void OrientParticlesToDirection(
    ParticleSystem particleSystem,
    ParticleSystem.Particle[] particles,
    int particleCount)
  {
    for (int index = 0; index < particleCount; ++index)
    {
      particles[index].angularVelocity = 0.0f;
      Vector3 targetVector = particles[index].velocity;
      if (!this.m_WorldSpace)
        targetVector = particleSystem.transform.TransformDirection(particles[index].velocity);
      if (this.m_ParticleOrientation.m_UpVector == ParticleEffectsOrientUpVectors.Horizontal)
        particles[index].rotation = ParticleEffects.VectorAngle(Vector3.forward, targetVector, Vector3.up);
      else if (this.m_ParticleOrientation.m_UpVector == ParticleEffectsOrientUpVectors.Vertical)
        particles[index].rotation = ParticleEffects.VectorAngle(Vector3.up, targetVector, Vector3.forward);
    }
  }

  private void ParticleAttractor(
    ParticleSystem particleSystem,
    ParticleSystem.Particle[] particles,
    int particleCount)
  {
    for (int index = 0; index < particleCount; ++index)
    {
      foreach (ParticleEffectsAttractor particleAttractor in this.m_ParticleAttractors)
      {
        if (!((Object) particleAttractor.m_Transform == (Object) null) && (double) particleAttractor.m_Radius > 0.0 && (double) particleAttractor.m_Power > 0.0)
        {
          Vector3 vector3_1 = particles[index].position;
          if (!this.m_WorldSpace)
            vector3_1 = particleSystem.transform.TransformPoint(particles[index].position);
          Vector3 vector3_2 = particleAttractor.m_Transform.position - vector3_1;
          float num1 = particleAttractor.m_Radius * (float) (((double) particleAttractor.m_Transform.lossyScale.x + (double) particleAttractor.m_Transform.lossyScale.y + (double) particleAttractor.m_Transform.lossyScale.z) * 0.333000004291534);
          float num2 = (float) (1.0 - (double) vector3_2.magnitude / (double) num1) * particleAttractor.m_Power;
          Vector3 b = vector3_2 * particles[index].velocity.magnitude;
          if (!this.m_WorldSpace)
            b = particleSystem.transform.InverseTransformDirection(vector3_2 * particles[index].velocity.magnitude);
          Vector3 vector3_3 = Vector3.Lerp(particles[index].velocity, b, num2 * Time.deltaTime);
          Vector3 normalized = vector3_3.normalized;
          vector3_3 = particles[index].velocity;
          double magnitude = (double) vector3_3.magnitude;
          Vector3 vector3_4 = normalized * (float) magnitude;
          particles[index].velocity = vector3_4;
        }
      }
    }
  }

  private void ParticleRepulser(
    ParticleSystem particleSystem,
    ParticleSystem.Particle[] particles,
    int particleCount)
  {
    for (int index = 0; index < particleCount; ++index)
    {
      foreach (ParticleEffectsRepulser particleRepulser in this.m_ParticleRepulsers)
      {
        if (!((Object) particleRepulser.m_Transform == (Object) null) && (double) particleRepulser.m_Radius > 0.0 && (double) particleRepulser.m_Power > 0.0)
        {
          Vector3 vector3_1 = particles[index].position;
          if (!this.m_WorldSpace)
            vector3_1 = particleSystem.transform.TransformPoint(particles[index].position);
          Vector3 vector3_2 = particleRepulser.m_Transform.position - vector3_1;
          float num1 = particleRepulser.m_Radius * (float) (((double) particleRepulser.m_Transform.lossyScale.x + (double) particleRepulser.m_Transform.lossyScale.y + (double) particleRepulser.m_Transform.lossyScale.z) * 0.333000004291534);
          float num2 = (float) (1.0 - (double) vector3_2.magnitude / (double) num1) * particleRepulser.m_Power + particleRepulser.m_Power;
          Vector3 vector3_3 = -vector3_2;
          Vector3 vector3_4 = particles[index].velocity;
          double magnitude1 = (double) vector3_4.magnitude;
          Vector3 b = vector3_3 * (float) magnitude1;
          if (!this.m_WorldSpace)
          {
            Transform transform = particleSystem.transform;
            Vector3 vector3_5 = -vector3_2;
            vector3_4 = particles[index].velocity;
            double magnitude2 = (double) vector3_4.magnitude;
            Vector3 direction = vector3_5 * (float) magnitude2;
            b = transform.InverseTransformDirection(direction);
          }
          vector3_4 = Vector3.Lerp(particles[index].velocity, b, num2 * Time.deltaTime);
          Vector3 normalized = vector3_4.normalized;
          vector3_4 = particles[index].velocity;
          double magnitude3 = (double) vector3_4.magnitude;
          Vector3 vector3_6 = normalized * (float) magnitude3;
          particles[index].velocity = vector3_6;
        }
      }
    }
  }

  private static float VectorAngle(Vector3 forwardVector, Vector3 targetVector, Vector3 upVector)
  {
    float num = Vector3.Angle(forwardVector, targetVector);
    return (double) Vector3.Dot(Vector3.Cross(forwardVector, targetVector), upVector) < 0.0 ? 360f - num : num;
  }
}
