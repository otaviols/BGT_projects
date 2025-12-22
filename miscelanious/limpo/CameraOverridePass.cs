using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraOverridePass : CustomViewPass
{
  public readonly string passName;
  public LayerMask layerMask;
  public RenderStateBlock depthStencilState;
  private static readonly List<ShaderTagId> s_ShaderTagIdList = new List<ShaderTagId>()
  {
    new ShaderTagId("UniversalForward"),
    new ShaderTagId("LightweightForward"),
    new ShaderTagId("SRPDefaultUnlit")
  };
  private ProfilingSampler m_ProfilingSampler;

  public CameraOverridePass.OverrideFlags toOverride { get; private set; }

  public uint renderLayerMaskOverride { get; private set; }

  public Matrix4x4 projectionOverride { get; private set; }

  public Matrix4x4 viewMatrixOverride { get; private set; }

  public Rect scissorOverride { get; private set; }

  public CameraOverridePass(string name, LayerMask layers)
  {
    this.passName = name;
    this.layerMask = layers;
    this.m_ProfilingSampler = new ProfilingSampler(name);
    this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }

  public void OverrideRenderLayerMask(uint renderLayerMask)
  {
    this.renderLayerMaskOverride = renderLayerMask;
    this.toOverride |= CameraOverridePass.OverrideFlags.RenderLayerMask;
  }

  public void OverrideProjectionMatrix(Matrix4x4 projectionMtx)
  {
    this.projectionOverride = projectionMtx;
    this.toOverride |= CameraOverridePass.OverrideFlags.ProjectionMatrix;
  }

  public void OverrideViewMatrix(Matrix4x4 viewMtx)
  {
    this.viewMatrixOverride = viewMtx;
    this.toOverride |= CameraOverridePass.OverrideFlags.ViewMatrix;
  }

  public void OverrideScissor(Rect scissor)
  {
    this.scissorOverride = scissor;
    this.toOverride |= CameraOverridePass.OverrideFlags.Scissor;
  }

  public void ClearOverrides(CameraOverridePass.OverrideFlags toClear) => this.toOverride &= ~toClear;

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    ref CameraData local = ref renderingData.cameraData;
    CommandBuffer commandBuffer = CommandBufferPool.Get(this.passName);
    using (new ProfilingScope(commandBuffer, this.m_ProfilingSampler))
    {
      if (this.toOverride.HasFlag((Enum) (CameraOverridePass.OverrideFlags.ProjectionMatrix | CameraOverridePass.OverrideFlags.ViewMatrix)))
      {
        Matrix4x4 projectionMatrix = local.GetGPUProjectionMatrix();
        Matrix4x4 viewMatrix = local.GetViewMatrix();
        if (this.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.ProjectionMatrix))
          projectionMatrix = GL.GetGPUProjectionMatrix(this.projectionOverride, local.IsCameraProjectionMatrixFlipped());
        if (this.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.ViewMatrix))
          viewMatrix = this.viewMatrixOverride;
        RenderingUtils.SetViewAndProjectionMatrices(commandBuffer, viewMatrix, projectionMatrix, false);
      }
      if (this.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.Scissor))
        commandBuffer.EnableScissorRect(this.scissorOverride);
      context.ExecuteCommandBuffer(commandBuffer);
      commandBuffer.Clear();
      FilteringSettings filteringSettings1 = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), (int) this.layerMask, uint.MaxValue, 0);
      if (this.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.RenderLayerMask))
        filteringSettings1.renderingLayerMask = this.renderLayerMaskOverride;
      SortingCriteria defaultOpaqueSortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
      DrawingSettings drawingSettings1 = this.CreateDrawingSettings(CameraOverridePass.s_ShaderTagIdList, ref renderingData, defaultOpaqueSortFlags);
      context.DrawRenderers(renderingData.cullResults, ref drawingSettings1, ref filteringSettings1, ref this.depthStencilState);
      FilteringSettings filteringSettings2 = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.transparent), (int) this.layerMask, uint.MaxValue, 0);
      if (this.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.RenderLayerMask))
        filteringSettings2.renderingLayerMask = this.renderLayerMaskOverride;
      SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
      DrawingSettings drawingSettings2 = this.CreateDrawingSettings(CameraOverridePass.s_ShaderTagIdList, ref renderingData, sortingCriteria);
      context.DrawRenderers(renderingData.cullResults, ref drawingSettings2, ref filteringSettings2, ref this.depthStencilState);
      if (this.toOverride.HasFlag((Enum) (CameraOverridePass.OverrideFlags.ProjectionMatrix | CameraOverridePass.OverrideFlags.ViewMatrix)))
        RenderingUtils.SetViewAndProjectionMatrices(commandBuffer, local.GetViewMatrix(), local.GetGPUProjectionMatrix(), false);
      if (this.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.Scissor))
        commandBuffer.DisableScissorRect();
    }
    context.ExecuteCommandBuffer(commandBuffer);
    CommandBufferPool.Release(commandBuffer);
  }

  [System.Flags]
  public enum OverrideFlags
  {
    Scissor = 1,
    ProjectionMatrix = 2,
    RenderLayerMask = 4,
    ViewMatrix = 8,
  }
}
