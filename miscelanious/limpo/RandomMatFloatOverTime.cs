using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class RandomMatFloatOverTime : MonoBehaviour
{
  public float minIntensity = 0.25f;
  public float maxIntensity = 0.5f;
  public float m_timeScale = 1f;
  public string m_property;
  public int m_matIndex;
  public bool m_sync;
  public float m_syncSeed;
  private float random;
  private Renderer m_renderer;

  private void Start()
  {
    this.random = !this.m_sync ? Random.Range(0.0f, (float) ushort.MaxValue) : this.m_syncSeed;
    this.m_renderer = this.GetComponent<Renderer>();
  }

  private void Update()
  {
    float t = Mathf.PerlinNoise(this.random, Time.time * this.m_timeScale);
    this.m_renderer.GetMaterial(this.m_matIndex).SetFloat(this.m_property, Mathf.Lerp(this.minIntensity, this.maxIntensity, t));
  }
}
