using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

[RequireComponent(typeof (Material))]
public class MaterialSoftFlicker : MonoBehaviour
{
  public float minIntensity = 0.25f;
  public float maxIntensity = 0.5f;
  public float m_timeScale = 1f;
  public Color m_color = new Color(1f, 1f, 1f, 1f);
  private float random;
  private Renderer m_renderer;

  private void Start()
  {
    this.random = Random.Range(0.0f, (float) ushort.MaxValue);
    this.m_renderer = this.gameObject.GetComponent<Renderer>();
  }

  private void Update()
  {
    float t = Mathf.PerlinNoise(this.random, Time.time * this.m_timeScale);
    this.m_renderer.GetMaterial().SetColor("_TintColor", new Color(this.m_color.r, this.m_color.g, this.m_color.b, Mathf.Lerp(this.minIntensity, this.maxIntensity, t)));
  }
}
