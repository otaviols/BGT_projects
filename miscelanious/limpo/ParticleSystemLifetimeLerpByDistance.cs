using System;
using UnityEngine;

public class ParticleSystemLifetimeLerpByDistance : MonoBehaviour
{
  public GameObject targetObject;
  public ParticleSystemLifetimeLerpByDistance.ScaledObject[] properties;

  private void Update()
  {
    float num = Vector3.Distance(this.transform.position, this.targetObject.transform.position);
    foreach (ParticleSystemLifetimeLerpByDistance.ScaledObject property in this.properties)
      property.component.main.startLifetime = (ParticleSystem.MinMaxCurve) Mathf.Lerp(property.startLifetimeMin, property.startLifetimeMax, (float) (((double) Mathf.Clamp(num, property.minDistance, property.maxDistance) - (double) property.minDistance) / ((double) property.maxDistance - (double) property.minDistance)));
  }

  [Serializable]
  public class ScaledObject
  {
    public ParticleSystem component;
    public float startLifetimeMin = 0.6f;
    public float startLifetimeMax = 1.2f;
    public float minDistance = 1f;
    public float maxDistance = 4f;
  }
}
