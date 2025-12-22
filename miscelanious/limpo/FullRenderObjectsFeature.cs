using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FullRenderObjectsFeature : ScriptableRendererFeature
{
  public FullRenderObjectsFeature.RenderObjectsSettings Settings = new FullRenderObjectsFeature.RenderObjectsSettings();
  private FullRenderObjectsPass m_renderObjectsPass;

  public override void Create()
  {
    if (this.Settings.Event < RenderPassEvent.BeforeRenderingPrepasses)
      this.Settings.Event = RenderPassEvent.BeforeRenderingPrepasses;
    this.m_renderObjectsPass = new FullRenderObjectsPass(this.Settings);
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) => renderer.EnqueuePass((ScriptableRenderPass) this.m_renderObjectsPass);

  [Serializable]
  public class RenderObjectsSettings
  {
    public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;
    public bool ClearDepth;
    public FullRenderObjectsFeature.FilterSettings FilterSettings = new FullRenderObjectsFeature.FilterSettings();
  }

  [Serializable]
  public class FilterSettings
  {
    public LayerMask LayerMask;

    public FilterSettings() => this.LayerMask = (LayerMask) 0;
  }
}
