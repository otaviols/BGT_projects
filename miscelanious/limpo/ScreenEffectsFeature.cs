using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScreenEffectsFeature : ScriptableRendererFeature
{
  public ScreenEffectsFeature.ScreenEffectsSettings m_settings = new ScreenEffectsFeature.ScreenEffectsSettings();
  private ScreenEffectsPass m_ScreenEffectsPass;
  private Camera m_camera;
  private ScreenEffectsRender m_screenEffectsRender;

  public override void Create()
  {
    this.m_ScreenEffectsPass = new ScreenEffectsPass("ScreenEffectsPass", (LayerMask) (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("ScreenEffects")), this.m_settings.m_opaqueOverrideMaterial, this.m_settings.m_glowMaterial);
    this.m_ScreenEffectsPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
    if ((UnityEngine.Object) this.m_screenEffectsRender == (UnityEngine.Object) null || (UnityEngine.Object) this.m_camera != (UnityEngine.Object) renderingData.cameraData.camera)
    {
      this.m_camera = renderingData.cameraData.camera;
      this.m_screenEffectsRender = this.m_camera.GetComponent<ScreenEffectsRender>();
    }
    this.m_ScreenEffectsPass.Setup(renderer.cameraColorTarget, this.m_screenEffectsRender);
    renderer.EnqueuePass((ScriptableRenderPass) this.m_ScreenEffectsPass);
  }

  [Serializable]
  public class ScreenEffectsSettings
  {
    public Material m_opaqueOverrideMaterial;
    public Material m_glowMaterial;
  }
}
