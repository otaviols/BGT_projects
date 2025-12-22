using UnityEngine;

public class AudioSourceSettings
{
  public bool m_bypassEffects;
  public bool m_loop;
  public int m_priority;
  public float m_volume;
  public float m_pitch;
  public float m_stereoPan;
  public float m_spatialBlend;
  public float m_reverbZoneMix;
  public AudioRolloffMode m_rolloffMode;
  public float m_dopplerLevel;
  public float m_minDistance;
  public float m_maxDistance;
  public float m_spread;

  public AudioSourceSettings() => this.LoadDefaults();

  public void LoadDefaults()
  {
    this.m_bypassEffects = false;
    this.m_loop = false;
    this.m_priority = 128;
    this.m_volume = 1f;
    this.m_pitch = 1f;
    this.m_stereoPan = 0.0f;
    this.m_spatialBlend = 1f;
    this.m_reverbZoneMix = 1f;
    this.m_rolloffMode = AudioRolloffMode.Linear;
    this.m_dopplerLevel = 1f;
    this.m_minDistance = 100f;
    this.m_maxDistance = 500f;
    this.m_spread = 0.0f;
  }
}
