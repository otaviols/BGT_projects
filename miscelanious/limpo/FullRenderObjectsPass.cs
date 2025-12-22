using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullRenderObjectsPass : ScriptableRenderPass
{
  private FilteringSettings m_filteringSettings;
  private int m_layerMask;
  private bool clearDepth;
  private List<ShaderTagId> m_shaderTagIdList = new List<ShaderTagId>()
  {
    new ShaderTagId("UniversalForward"),
    new ShaderTagId("LightweightForward"),
    new ShaderTagId("SRPDefaultUnlit")
  };

  public FullRenderObjectsPass(
    FullRenderObjectsFeature.RenderObjectsSettings settings)
  {
    this.renderPassEvent = settings.Event;
    this.m_layerMask = (int) settings.FilterSettings.LayerMask;
    this.clearDepth = settings.ClearDepth;
  }

  private void RenderOpaques(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    FilteringSettings filteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), this.m_layerMask, uint.MaxValue, 0);
    SortingCriteria defaultOpaqueSortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
    DrawingSettings drawingSettings = this.CreateDrawingSettings(this.m_shaderTagIdList, ref renderingData, defaultOpaqueSortFlags);
    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
  }

  private void RenderTransparents(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    FilteringSettings filteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.transparent), this.m_layerMask, uint.MaxValue, 0);
    SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
    DrawingSettings drawingSettings = this.CreateDrawingSettings(this.m_shaderTagIdList, ref renderingData, sortingCriteria);
    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
  }

  public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
  {
    if (!this.clearDepth)
      return;
    this.ConfigureClear(ClearFlag.Depth, Color.black);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    this.RenderOpaques(context, ref renderingData);
    this.RenderTransparents(context, ref renderingData);
  }
}
