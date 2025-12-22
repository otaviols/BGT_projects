using Blizzard.T5.Core;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ParticlePlaybackSpeed : MonoBehaviour
{
  public float m_ParticlePlaybackSpeed = 1f;
  public bool m_RestoreSpeedOnDisable = true;
  private float m_PreviousPlaybackSpeed = 1f;
  private Map<ParticleSystem, float> m_OrgPlaybackSpeed;
  private List<ParticleSystem> m_ParticleSystems;

  private void Start() => this.Init();

  private void Update()
  {
    if ((double) this.m_ParticlePlaybackSpeed == (double) this.m_PreviousPlaybackSpeed)
      return;
    this.m_PreviousPlaybackSpeed = this.m_ParticlePlaybackSpeed;
    int index = 0;
    while (index < this.m_ParticleSystems.Count)
    {
      ParticleSystem particleSystem = this.m_ParticleSystems[index];
      if ((bool) (Object) particleSystem)
      {
        particleSystem.main.simulationSpeed = this.m_ParticlePlaybackSpeed;
        ++index;
      }
      else
      {
        this.m_OrgPlaybackSpeed.Remove(particleSystem);
        this.m_ParticleSystems.RemoveAt(index);
      }
    }
  }

  private void OnDisable()
  {
    if (this.m_RestoreSpeedOnDisable)
    {
      foreach (KeyValuePair<ParticleSystem, float> keyValuePair in this.m_OrgPlaybackSpeed)
      {
        ParticleSystem key = keyValuePair.Key;
        float num = keyValuePair.Value;
        if ((bool) (Object) key)
          key.main.simulationSpeed = num;
      }
    }
    this.m_PreviousPlaybackSpeed = -1E+07f;
    this.m_ParticleSystems.Clear();
    this.m_OrgPlaybackSpeed.Clear();
  }

  private void OnEnable() => this.Init();

  private void Init()
  {
    if (this.m_ParticleSystems == null)
      this.m_ParticleSystems = new List<ParticleSystem>();
    else
      this.m_ParticleSystems.Clear();
    if (this.m_OrgPlaybackSpeed == null)
      this.m_OrgPlaybackSpeed = new Map<ParticleSystem, float>();
    else
      this.m_OrgPlaybackSpeed.Clear();
    foreach (ParticleSystem componentsInChild in this.gameObject.GetComponentsInChildren<ParticleSystem>())
    {
      this.m_OrgPlaybackSpeed.Add(componentsInChild, componentsInChild.main.simulationSpeed);
      this.m_ParticleSystems.Add(componentsInChild);
    }
  }
}
