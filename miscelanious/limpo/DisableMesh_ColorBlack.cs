using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class DisableMesh_ColorBlack : MonoBehaviour
{
  private Material m_material;
  private bool m_tintColor;
  private Color m_color = Color.black;
  private Renderer m_renderer;

  private void Start()
  {
    this.m_renderer = this.GetComponent<Renderer>();
    this.HandleMaterialChanged();
  }

  private void Update()
  {
    this.m_color = !this.m_tintColor ? this.m_material.color : this.m_material.GetColor("_TintColor");
    if (!this.MaterialColorMeetsThreadhold())
      return;
    this.m_renderer.enabled = false;
    this.enabled = false;
  }

  private bool MaterialColorMeetsThreadhold() => (double) this.m_color.r < 0.00999999977648258 && (double) this.m_color.g < 0.00999999977648258 && (double) this.m_color.b < 0.00999999977648258;

  public void HandleMaterialChanged()
  {
    this.m_material = this.m_renderer.GetMaterial();
    if ((Object) this.m_material == (Object) null)
      this.enabled = false;
    else if (!this.m_material.HasProperty("_Color") && !this.m_material.HasProperty("_TintColor"))
    {
      this.enabled = false;
    }
    else
    {
      if (this.m_material.HasProperty("_TintColor"))
        this.m_tintColor = true;
      if (this.MaterialColorMeetsThreadhold())
      {
        this.m_renderer.enabled = false;
        this.enabled = false;
      }
      else
      {
        this.enabled = true;
        this.m_renderer.enabled = true;
      }
    }
  }
}
