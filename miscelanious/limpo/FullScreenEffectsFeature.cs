using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenEffectsFeature : ScriptableRendererFeature
{
  private FullScreenEffectsPass m_FullScreenEffectsPass;
  private FullScreenEffectsFeature.ClearDepthPass m_ClearDepthPass;
  public FullScreenEffectsFeature.Settings settings = new FullScreenEffectsFeature.Settings();
  private Camera m_currentCamera;
  private FullScreenEffects m_fullScreenEffects;

  public override void Create()
  {
    this.m_FullScreenEffectsPass = new FullScreenEffectsPass();
    this.m_ClearDepthPass = new FullScreenEffectsFeature.ClearDepthPass();
    this.m_FullScreenEffectsPass.renderPassEvent = this.settings.renderPassEvent;
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
    if ((UnityEngine.Object) this.m_fullScreenEffects == (UnityEngine.Object) null || (UnityEngine.Object) this.m_currentCamera != (UnityEngine.Object) renderingData.cameraData.camera)
    {
      this.m_currentCamera = renderingData.cameraData.camera;
      this.m_fullScreenEffects = this.m_currentCamera.GetComponent<FullScreenEffects>();
    }
    this.settings.cameraColorTarget = renderer.cameraColorTarget;
    this.settings.cameraDepthTarget = renderer.cameraDepth;
    this.m_FullScreenEffectsPass.Setup("Full Screen Effects Pass", this.settings, this.m_fullScreenEffects);
    renderer.EnqueuePass((ScriptableRenderPass) this.m_FullScreenEffectsPass);
    renderer.EnqueuePass((ScriptableRenderPass) this.m_ClearDepthPass);
  }

  [Serializable]
  public class Settings
  {
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    public Material m_blurMaterial;
    public Material m_blurBlendMaterial;
    public Material m_desaturationMaterial;
    public Material m_vignettingMaterial;
    public Material m_blendToColorMaterial;
    public RenderTargetIdentifier cameraColorTarget;
    public RenderTargetIdentifier cameraDepthTarget;
  }

  private class ClearDepthPass : ScriptableRenderPass
  {
    public ClearDepthPass() => this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

    public override void Configure(
      CommandBuffer cmd,
      RenderTextureDescriptor cameraTextureDescriptor)
    {
      this.ConfigureClear(ClearFlag.Depth, Color.black);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
    }
  }
}
