using Blizzard.T5.Core;
using UnityEngine;

public class ParticleSystemScaler : MonoBehaviour
{
  public float ParticleSystemScale = 1f;
  public GameObject ObjectToInherit;
  private float m_unitMagnitude;
  private Map<ParticleSystem, ParticleSystemSizes> m_initialValues = new Map<ParticleSystem, ParticleSystemSizes>();

  private void Awake() => this.m_unitMagnitude = Vector3.one.magnitude;

  private void Update()
  {
    if ((Object) this.ObjectToInherit != (Object) null)
      this.ParticleSystemScale = this.ObjectToInherit.transform.lossyScale.magnitude / this.m_unitMagnitude;
    foreach (ParticleSystem componentsInChild in this.GetComponentsInChildren<ParticleSystem>())
    {
      ParticleSystem.MainModule main = componentsInChild.main;
      if (!this.m_initialValues.ContainsKey(componentsInChild))
      {
        this.m_initialValues.Add(componentsInChild, new ParticleSystemSizes());
        ParticleSystemSizes initialValue1 = this.m_initialValues[componentsInChild];
        ParticleSystem.MinMaxCurve minMaxCurve = main.startSpeed;
        double constant1 = (double) minMaxCurve.constant;
        initialValue1.startSpeed = (float) constant1;
        ParticleSystemSizes initialValue2 = this.m_initialValues[componentsInChild];
        minMaxCurve = main.startSize;
        double constant2 = (double) minMaxCurve.constant;
        initialValue2.startSize = (float) constant2;
        ParticleSystemSizes initialValue3 = this.m_initialValues[componentsInChild];
        minMaxCurve = main.gravityModifier;
        double constant3 = (double) minMaxCurve.constant;
        initialValue3.gravityModifier = (float) constant3;
      }
      main.startSize = (ParticleSystem.MinMaxCurve) (this.m_initialValues[componentsInChild].startSize * this.ParticleSystemScale);
      main.startSpeed = (ParticleSystem.MinMaxCurve) (this.m_initialValues[componentsInChild].startSpeed * this.ParticleSystemScale);
      main.gravityModifier = (ParticleSystem.MinMaxCurve) (this.m_initialValues[componentsInChild].gravityModifier * this.ParticleSystemScale);
    }
  }

  private void ScaleParticleSystems(float scaleFactor)
  {
    foreach (ParticleSystem componentsInChild in this.GetComponentsInChildren<ParticleSystem>())
    {
      ParticleSystem.MainModule main = componentsInChild.main;
      main.startSpeed = (ParticleSystem.MinMaxCurve) (main.startSpeed.constant * scaleFactor);
      main.startSize = (ParticleSystem.MinMaxCurve) (main.startSize.constant * scaleFactor);
      main.gravityModifier = (ParticleSystem.MinMaxCurve) (main.gravityModifier.constant * scaleFactor);
    }
  }
}
