using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenEffectsPass : ScriptableRenderPass
{
  private ProfilingSampler m_profilingSampler;
  private string m_passTag;
  private LayerMask m_layerMask;
  private RenderTargetIdentifier m_cameraColorTarget;
  private RenderTargetIdentifier m_effectsMaskTexture;
  private ScreenEffectsRender m_screenEffectsRender;
  private RenderStateBlock m_depthStencilState;
  private Material m_opaqueOverrideMaterial;
  private Material m_glowMaterial;
  private int m_bloomBuffer1 = Shader.PropertyToID("_BloomBuffer1");
  private int m_bloomBuffer2 = Shader.PropertyToID("_BloomBuffer2");
  private int m_blurOffsetID = Shader.PropertyToID("_BlurOffset");
  private int m_mipLevelID = Shader.PropertyToID("_MipLevel");
  private int m_blurTexID = Shader.PropertyToID("_BlurTex");
  private RenderTargetIdentifier m_bloomBuffer1RTI;
  private RenderTargetIdentifier m_bloomBuffer2RTI;
  private List<ShaderTagId> m_opaqueShaderTags = new List<ShaderTagId>()
  {
    new ShaderTagId("UniversalForward"),
    new ShaderTagId("LightweightForward"),
    new ShaderTagId("SRPDefaultUnlit")
  };
  private List<ShaderTagId> m_glowPrepassTag = new List<ShaderTagId>()
  {
    new ShaderTagId("GlowPrepass")
  };
  private List<ShaderTagId> m_opaqueGlowShaderTags = new List<ShaderTagId>()
  {
    new ShaderTagId("Glow")
  };
  private List<ShaderTagId> m_shaderTags = new List<ShaderTagId>()
  {
    new ShaderTagId("Glow"),
    new ShaderTagId("GlowTransparent"),
    new ShaderTagId("GlowAdditive"),
    new ShaderTagId("GlowDissolveEdge"),
    new ShaderTagId("GlowCutoutDissolve")
  };

  public ScreenEffectsPass(
    string profilerTag,
    LayerMask layerMask,
    Material overrideMaterial,
    Material glowMaterial)
  {
    this.m_passTag = profilerTag;
    this.m_profilingSampler = new ProfilingSampler(profilerTag);
    this.m_layerMask = layerMask;
    this.m_opaqueOverrideMaterial = overrideMaterial;
    this.m_glowMaterial = glowMaterial;
  }

  public void Setup(
    RenderTargetIdentifier cameraColorTarget,
    ScreenEffectsRender screenEffectsRender)
  {
    this.m_cameraColorTarget = cameraColorTarget;
    this.m_screenEffectsRender = screenEffectsRender;
    this.m_effectsMaskTexture = new RenderTargetIdentifier((Texture) this.m_screenEffectsRender.m_MaskRenderTexture);
  }

  public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
  {
    if (!(bool) (Object) this.m_screenEffectsRender.m_MaskRenderTexture)
      return;
    this.ConfigureTarget(this.m_effectsMaskTexture);
    int width = this.m_screenEffectsRender.m_MaskRenderTexture.width;
    int height = this.m_screenEffectsRender.m_MaskRenderTexture.height;
    cmd.GetTemporaryRT(this.m_bloomBuffer1, width, height, 0, FilterMode.Bilinear);
    cmd.GetTemporaryRT(this.m_bloomBuffer2, width, height, 0, FilterMode.Bilinear);
    this.m_bloomBuffer1RTI = new RenderTargetIdentifier(this.m_bloomBuffer1);
    this.m_bloomBuffer2RTI = new RenderTargetIdentifier(this.m_bloomBuffer2);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    if (!(bool) (Object) this.m_screenEffectsRender.m_MaskRenderTexture)
      return;
    CommandBuffer commandBuffer = CommandBufferPool.Get(this.m_passTag);
    commandBuffer.ClearRenderTarget(true, true, Color.black);
    context.ExecuteCommandBuffer(commandBuffer);
    commandBuffer.Clear();
    FilteringSettings filteringSettings1 = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), LayerMask.GetMask("Default", "CardRaycast"), uint.MaxValue, 0);
    SortingCriteria defaultOpaqueSortFlags1 = renderingData.cameraData.defaultOpaqueSortFlags;
    DrawingSettings drawingSettings1 = this.CreateDrawingSettings(this.m_opaqueShaderTags, ref renderingData, defaultOpaqueSortFlags1) with
    {
      overrideMaterial = this.m_opaqueOverrideMaterial,
      overrideMaterialPassIndex = 5
    };
    context.DrawRenderers(renderingData.cullResults, ref drawingSettings1, ref filteringSettings1, ref this.m_depthStencilState);
    FilteringSettings filteringSettings2 = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), LayerMask.GetMask("ScreenEffects"), uint.MaxValue, 0);
    SortingCriteria defaultOpaqueSortFlags2 = renderingData.cameraData.defaultOpaqueSortFlags;
    DrawingSettings drawingSettings2 = this.CreateDrawingSettings(this.m_glowPrepassTag, ref renderingData, defaultOpaqueSortFlags2);
    context.DrawRenderers(renderingData.cullResults, ref drawingSettings2, ref filteringSettings2, ref this.m_depthStencilState);
    DrawingSettings drawingSettings3 = this.CreateDrawingSettings(this.m_opaqueGlowShaderTags, ref renderingData, defaultOpaqueSortFlags2);
    context.DrawRenderers(renderingData.cullResults, ref drawingSettings3, ref filteringSettings2, ref this.m_depthStencilState);
    FilteringSettings filteringSettings3 = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.transparent), (int) this.m_layerMask, uint.MaxValue, 0);
    SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
    DrawingSettings drawingSettings4 = this.CreateDrawingSettings(this.m_glowPrepassTag, ref renderingData, sortingCriteria);
    context.DrawRenderers(renderingData.cullResults, ref drawingSettings4, ref filteringSettings3, ref this.m_depthStencilState);
    DrawingSettings drawingSettings5 = this.CreateDrawingSettings(this.m_shaderTags, ref renderingData, sortingCriteria);
    context.DrawRenderers(renderingData.cullResults, ref drawingSettings5, ref filteringSettings3, ref this.m_depthStencilState);
    commandBuffer.SetGlobalFloat(this.m_blurOffsetID, 1f);
    commandBuffer.SetGlobalInt(this.m_mipLevelID, 2);
    this.Blit(commandBuffer, (RenderTargetIdentifier) (Texture) this.m_screenEffectsRender.m_MaskRenderTexture, (RenderTargetIdentifier) this.m_bloomBuffer1, this.m_glowMaterial);
    commandBuffer.SetGlobalFloat(this.m_blurOffsetID, 2f);
    commandBuffer.SetGlobalInt(this.m_mipLevelID, 0);
    this.Blit(commandBuffer, (RenderTargetIdentifier) this.m_bloomBuffer1, (RenderTargetIdentifier) this.m_bloomBuffer2, this.m_glowMaterial);
    if (!this.m_screenEffectsRender.m_Debug)
    {
      this.Blit(commandBuffer, this.m_cameraColorTarget, this.m_bloomBuffer1RTI);
      commandBuffer.SetGlobalTexture(this.m_blurTexID, (RenderTargetIdentifier) this.m_bloomBuffer2);
      this.Blit(commandBuffer, this.m_bloomBuffer1RTI, this.m_cameraColorTarget, this.m_glowMaterial, 1);
    }
    else
      this.Blit(commandBuffer, this.m_bloomBuffer2RTI, this.m_cameraColorTarget, this.m_glowMaterial, 2);
    context.ExecuteCommandBuffer(commandBuffer);
    CommandBufferPool.Release(commandBuffer);
  }

  public override void FrameCleanup(CommandBuffer cmd)
  {
    if (!(bool) (Object) this.m_screenEffectsRender.m_MaskRenderTexture)
      return;
    cmd.ReleaseTemporaryRT(this.m_bloomBuffer1);
    cmd.ReleaseTemporaryRT(this.m_bloomBuffer2);
  }
}
