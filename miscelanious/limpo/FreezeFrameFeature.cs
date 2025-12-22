using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FreezeFrameFeature : ScriptableRendererFeature
{
  private FreezeFramePass m_freezeFramePass = new FreezeFramePass();
  private Camera m_camera;
  private FreezeFrame m_freezeFrame;

  public override void Create() => this.m_freezeFramePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
    if ((Object) this.m_freezeFrame == (Object) null || (Object) this.m_camera != (Object) renderingData.cameraData.camera)
    {
      this.m_camera = renderingData.cameraData.camera;
      this.m_freezeFrame = this.m_camera.GetComponent<FreezeFrame>();
    }
    this.m_freezeFramePass.Setup("Freeze Frame Pass", renderer.cameraColorTarget, this.m_freezeFrame);
    renderer.EnqueuePass((ScriptableRenderPass) this.m_freezeFramePass);
  }
}
