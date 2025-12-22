using UnityEngine;

[ExecuteAlways]
public class ScreenEffectGlow : ScreenEffect
{
  public bool m_RenderGlowOnly;
  private bool m_PreviousRenderGlowOnly;
  private int m_PreviousLayer;

  private void Awake() => this.m_PreviousLayer = this.gameObject.layer;

  private void Start() => this.SetLayer();

  private void Update()
  {
  }

  private void SetLayer()
  {
    if (this.m_PreviousRenderGlowOnly == this.m_RenderGlowOnly)
      return;
    this.m_PreviousRenderGlowOnly = this.m_RenderGlowOnly;
    if (this.m_RenderGlowOnly)
    {
      this.m_PreviousLayer = this.gameObject.layer;
      LayerUtils.SetLayer(this.gameObject, GameLayer.ScreenEffects);
    }
    else
      LayerUtils.SetLayer(this.gameObject, this.m_PreviousLayer);
  }
}
