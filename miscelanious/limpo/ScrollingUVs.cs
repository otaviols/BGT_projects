using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class ScrollingUVs : MonoBehaviour
{
  public int materialIndex;
  public Vector2 uvAnimationRate = new Vector2(1f, 1f);
  private Material m_material;
  private Vector2 m_offset = Vector2.zero;
  private Renderer m_renderer;

  private void Start()
  {
    this.m_renderer = this.GetComponent<Renderer>();
    this.m_material = this.m_renderer.GetMaterial(this.materialIndex);
  }

  private void LateUpdate()
  {
    if (!this.m_renderer.enabled)
      return;
    if ((Object) this.m_material == (Object) null)
      this.m_material = this.m_renderer.GetMaterial(this.materialIndex);
    this.m_offset += this.uvAnimationRate * Time.deltaTime;
    this.m_material.SetTextureOffset("_MainTex", this.m_offset);
  }
}
