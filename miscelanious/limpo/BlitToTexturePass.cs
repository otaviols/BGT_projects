using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitToTexturePass : ScriptableRenderPass
{
  private const string ProfilerTag = "BlitToTexture";
  private readonly Material m_blitMaterial;
  private static readonly int s_offsetAndScale = Shader.PropertyToID("uvOffsetAndScale");
  private static readonly int s_uvRotateMtx = Shader.PropertyToID("uvRotateMtx");
  private readonly List<BlitToTextureService.Request> m_requests = new List<BlitToTextureService.Request>();
  private readonly BlitToTextureRenderLatePass m_blitToTextureRenderLatePass;
  public RenderTargetIdentifier CameraColorTarget;

  public BlitToTexturePass(
    BlitToTextureRenderLatePass blitToTextureRenderLatePass)
  {
    this.m_blitToTextureRenderLatePass = blitToTextureRenderLatePass;
    Shader shader = Shader.Find("Hidden/HS_URP/BlitToTexture");
    if (!(bool) (UnityEngine.Object) shader)
      return;
    this.m_blitMaterial = new Material(shader);
  }

  public void EnqueueRequest(BlitToTextureService.Request request) => this.m_requests.Add(request);

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    if (!(bool) (UnityEngine.Object) this.m_blitMaterial)
      return;
    CommandBuffer commandBuffer = CommandBufferPool.Get("BlitToTexture");
    Camera camera = renderingData.cameraData.camera;
    foreach (BlitToTextureService.Request request in this.m_requests)
    {
      Vector3 viewportPoint1 = camera.ScreenToViewportPoint((Vector3) request.Offset);
      Vector3 viewportPoint2 = camera.ScreenToViewportPoint((Vector3) request.Size);
      this.m_blitMaterial.SetVector(BlitToTexturePass.s_offsetAndScale, new Vector4(viewportPoint1.x, viewportPoint1.y, viewportPoint2.x, viewportPoint2.y));
      float f = (float) Math.PI / 180f * request.RotationDeg;
      Vector4 vector4 = new Vector4(Mathf.Cos(f), -Mathf.Sin(f), Mathf.Sin(f), Mathf.Cos(f));
      this.m_blitMaterial.SetVector(BlitToTexturePass.s_uvRotateMtx, vector4);
      this.Blit(commandBuffer, this.CameraColorTarget, (RenderTargetIdentifier) (Texture) request.TargetTexture, this.m_blitMaterial);
      if ((UnityEngine.Object) request.DrawAfterRenderer != (UnityEngine.Object) null)
        this.m_blitToTextureRenderLatePass.EnqueueRenderer(request.DrawAfterRenderer);
    }
    this.m_requests.Clear();
    context.ExecuteCommandBuffer(commandBuffer);
    CommandBufferPool.Release(commandBuffer);
  }
}
