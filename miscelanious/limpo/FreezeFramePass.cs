using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FreezeFramePass : ScriptableRenderPass
{
  private string m_ProfilerTag;
  private FreezeFrame m_FreezeFrame;
  private RenderTargetIdentifier m_CameraColorTarget;

  public void Setup(
    string profilerTag,
    RenderTargetIdentifier cameraColorTarget,
    FreezeFrame freezeFrame)
  {
    this.m_ProfilerTag = profilerTag;
    this.m_CameraColorTarget = cameraColorTarget;
    this.m_FreezeFrame = freezeFrame;
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    if (this.m_FreezeFrame.m_CaptureFrozenImage && !this.m_FreezeFrame.m_FrozenState)
    {
      CommandBuffer commandBuffer = CommandBufferPool.Get(this.m_ProfilerTag);
      commandBuffer.Blit(this.m_CameraColorTarget, (RenderTargetIdentifier) (Texture) this.m_FreezeFrame.m_FrozenScreenTexture);
      context.ExecuteCommandBuffer(commandBuffer);
      CommandBufferPool.Release(commandBuffer);
      this.m_FreezeFrame.m_CaptureFrozenImage = false;
      this.m_FreezeFrame.m_FrozenState = true;
      this.m_FreezeFrame.m_DeactivateFrameCount = 0;
    }
    if (!this.m_FreezeFrame.m_FrozenState)
      return;
    CommandBuffer commandBuffer1 = CommandBufferPool.Get(this.m_ProfilerTag);
    this.Blit(commandBuffer1, (RenderTargetIdentifier) (Texture) this.m_FreezeFrame.m_FrozenScreenTexture, this.m_CameraColorTarget);
    context.ExecuteCommandBuffer(commandBuffer1);
    CommandBufferPool.Release(commandBuffer1);
    this.m_FreezeFrame.m_DeactivateFrameCount = 0;
  }
}
