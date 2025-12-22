using UnityEngine;

public class RandomWeather : MonoBehaviour
{
  public float m_StartDelayMinMinutes = 1f;
  public float m_StartDelayMaxMinutes = 10f;
  public float m_WeatherMinMinutes = 2f;
  public float m_WeatherMaxMinutes = 5f;
  private ParticleSystem[] m_particleSystems;
  private float m_startTime;
  private float m_runEndTime;
  private bool m_active;

  private void Start()
  {
    this.m_particleSystems = this.GetComponentsInChildren<ParticleSystem>();
    this.m_startTime = Random.Range(Time.timeSinceLevelLoad + this.m_StartDelayMinMinutes * 60f, Time.timeSinceLevelLoad + this.m_StartDelayMaxMinutes * 60f);
  }

  private void Update()
  {
    if (this.m_active)
    {
      if ((double) Time.timeSinceLevelLoad <= (double) this.m_runEndTime)
        return;
      this.StopWeather();
    }
    else
    {
      if ((double) Time.timeSinceLevelLoad <= (double) this.m_startTime)
        return;
      this.StartWeather();
      this.m_startTime = Random.Range(Time.timeSinceLevelLoad + this.m_StartDelayMinMinutes * 60f, Time.timeSinceLevelLoad + this.m_StartDelayMaxMinutes * 60f);
    }
  }

  [ContextMenu("Start Weather")]
  private void StartWeather()
  {
    this.m_active = true;
    this.m_runEndTime = Random.Range(Time.timeSinceLevelLoad + this.m_WeatherMinMinutes * 60f, Time.timeSinceLevelLoad + this.m_WeatherMaxMinutes * 60f);
    foreach (ParticleSystem particleSystem in this.m_particleSystems)
    {
      if (!((Object) particleSystem == (Object) null))
        particleSystem.Play();
    }
  }

  [ContextMenu("Stop Weather")]
  private void StopWeather()
  {
    this.m_active = false;
    foreach (ParticleSystem particleSystem in this.m_particleSystems)
    {
      if (!((Object) particleSystem == (Object) null))
        particleSystem.Stop();
    }
  }
}
