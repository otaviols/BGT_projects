using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DecalRendererFeature : ScriptableRendererFeature
{
  public static string s_tempDepthName = "QuarterResDecalDepthCopy";
  public static int s_tempDepth = Shader.PropertyToID(DecalRendererFeature.s_tempDepthName);
  public static int s_scaleBiasId = Shader.PropertyToID("_ScaleBiasRT");
  public static List<DecalProjector> s_decals = new List<DecalProjector>();
  private DecalRendererFeature.DecalRendererPass m_pass;
  private DecalRendererFeature.CopyDepthForDecalsPass m_copyPass;

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
    if (DecalRendererFeature.s_decals.Count <= 0)
      return;
    renderer.EnqueuePass((ScriptableRenderPass) this.m_copyPass);
    renderer.EnqueuePass((ScriptableRenderPass) this.m_pass);
  }

  public override void Create()
  {
    this.m_pass = new DecalRendererFeature.DecalRendererPass();
    this.m_copyPass = new DecalRendererFeature.CopyDepthForDecalsPass();
  }

  public class CopyDepthForDecalsPass : ScriptableRenderPass
  {
    private Material m_copyDepthMaterial;

    public CopyDepthForDecalsPass()
    {
      this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
      Shader shader = Shader.Find("Hidden/Universal Render Pipeline/CopyDepth");
      if (!(bool) (Object) shader)
        return;
      this.m_copyDepthMaterial = new Material(shader);
    }

    public override void Configure(
      CommandBuffer cmd,
      RenderTextureDescriptor cameraTextureDescriptor)
    {
      RenderTextureDescriptor desc = cameraTextureDescriptor with
      {
        colorFormat = RenderTextureFormat.Depth,
        depthBufferBits = 16,
        msaaSamples = 1
      };
      desc.width /= 4;
      desc.height /= 4;
      cmd.GetTemporaryRT(DecalRendererFeature.s_tempDepth, desc);
      this.ConfigureTarget((RenderTargetIdentifier) DecalRendererFeature.s_tempDepth);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
      if (!(bool) (Object) this.m_copyDepthMaterial)
        return;
      CommandBuffer commandBuffer = CommandBufferPool.Get("Decal Depth Copy");
      int msaaSamples = renderingData.cameraData.cameraTargetDescriptor.msaaSamples;
      CameraData cameraData = renderingData.cameraData;
      switch (msaaSamples)
      {
        case 2:
          commandBuffer.EnableShaderKeyword(ShaderKeywordStrings.DepthMsaa2);
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa4);
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa8);
          break;
        case 4:
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa2);
          commandBuffer.EnableShaderKeyword(ShaderKeywordStrings.DepthMsaa4);
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa8);
          break;
        case 8:
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa2);
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa4);
          commandBuffer.EnableShaderKeyword(ShaderKeywordStrings.DepthMsaa8);
          break;
        default:
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa2);
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa4);
          commandBuffer.DisableShaderKeyword(ShaderKeywordStrings.DepthMsaa8);
          break;
      }
      float x = cameraData.IsCameraProjectionMatrixFlipped() ? -1f : 1f;
      Vector4 vector4 = (double) x < 0.0 ? new Vector4(x, 1f, -1f, 1f) : new Vector4(x, 0.0f, 1f, 1f);
      commandBuffer.SetGlobalVector(DecalRendererFeature.s_scaleBiasId, vector4);
      commandBuffer.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, this.m_copyDepthMaterial);
      context.ExecuteCommandBuffer(commandBuffer);
      CommandBufferPool.Release(commandBuffer);
    }
  }

  public class DecalRendererPass : ScriptableRenderPass
  {
    public DecalRendererPass() => this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

    public override void Configure(
      CommandBuffer cmd,
      RenderTextureDescriptor cameraTextureDescriptor)
    {
      this.ConfigureClear(ClearFlag.None, Color.black);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
      CommandBuffer commandBuffer = CommandBufferPool.Get("Decal Pass");
      foreach (DecalProjector decal in DecalRendererFeature.s_decals)
      {
        Material material = decal.Material;
        if ((Object) material != (Object) null)
          commandBuffer.DrawRenderer(decal.Renderer, material);
      }
      commandBuffer.ReleaseTemporaryRT(DecalRendererFeature.s_tempDepth);
      context.ExecuteCommandBuffer(commandBuffer);
      commandBuffer.Release();
    }
  }
}
