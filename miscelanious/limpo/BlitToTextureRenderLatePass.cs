using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitToTextureRenderLatePass : ScriptableRenderPass
{
  private const string ProfilerTag = "BlitToTextureRenderLate";
  private readonly List<Renderer> m_requests = new List<Renderer>();
  public RenderTargetIdentifier CameraColorTarget;

  public void EnqueueRenderer(Renderer renderer) => this.m_requests.Add(renderer);

  public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) => this.ConfigureTarget(this.CameraColorTarget);

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    CommandBuffer commandBuffer = CommandBufferPool.Get("BlitToTextureRenderLate");
    foreach (Renderer request in this.m_requests)
    {
      foreach (Material material in request.GetMaterials())
        commandBuffer.DrawRenderer(request, material);
    }
    this.m_requests.Clear();
    context.ExecuteCommandBuffer(commandBuffer);
    CommandBufferPool.Release(commandBuffer);
  }
}
