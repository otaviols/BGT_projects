using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class DisableMesh_Intensity : MonoBehaviour
{
  private Material m_material;
  private Renderer m_renderer;

  private void Start()
  {
    this.m_renderer = this.GetComponent<Renderer>();
    this.m_material = this.m_renderer.GetMaterial();
    if ((Object) this.m_material == (Object) null)
      this.enabled = false;
    if (this.m_material.HasProperty("_Intensity"))
      return;
    this.enabled = false;
  }

  private void Update()
  {
    if ((double) this.m_material.GetFloat("_Intensity") == 0.0)
      this.m_renderer.enabled = false;
    else
      this.m_renderer.enabled = true;
  }
}
