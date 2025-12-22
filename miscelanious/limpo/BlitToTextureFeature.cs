using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class BlitToTextureFeature : ScriptableRendererFeature
{
  private static BlitToTextureFeature s_instance;
  private BlitToTexturePass m_pass;
  private BlitToTextureRenderLatePass m_renderLatePass;
  private readonly List<BlitToTextureService.Request> m_persistentRequests = new List<BlitToTextureService.Request>();

  public static BlitToTextureFeature Get() => BlitToTextureFeature.s_instance;

  public override void Create()
  {
    BlitToTextureFeature.s_instance = this;
    BlitToTextureRenderLatePass textureRenderLatePass = new BlitToTextureRenderLatePass();
    textureRenderLatePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    this.m_renderLatePass = textureRenderLatePass;
    BlitToTexturePass blitToTexturePass = new BlitToTexturePass(this.m_renderLatePass);
    blitToTexturePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    this.m_pass = blitToTexturePass;
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
    foreach (BlitToTextureService.Request persistentRequest in this.m_persistentRequests)
      this.EnqueueRequest(persistentRequest);
    this.m_pass.CameraColorTarget = renderer.cameraColorTarget;
    this.m_renderLatePass.CameraColorTarget = renderer.cameraColorTarget;
    renderer.EnqueuePass((ScriptableRenderPass) this.m_pass);
    renderer.EnqueuePass((ScriptableRenderPass) this.m_renderLatePass);
  }

  public void EnqueueRequest(BlitToTextureService.Request request) => this.m_pass.EnqueueRequest(request);

  public void AddPersistentRequest(BlitToTextureService.Request request) => this.m_persistentRequests.Add(request);

  public void RemovePersistentRequest(BlitToTextureService.Request request) => this.m_persistentRequests.Remove(request);
}
