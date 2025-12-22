using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomViewsRenderFeature : ScriptableRendererFeature
{
  public CustomViewEntryPoint entryPoint;

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
    List<CustomViewPass> queue = CustomViewPass.GetQueue(this.entryPoint);
    if (queue == null)
      return;
    foreach (CustomViewPass pass in queue)
      renderer.EnqueuePass((ScriptableRenderPass) pass);
  }

  public override void Create()
  {
    if (this.entryPoint != CustomViewEntryPoint.Count)
      return;
    Debug.LogError((object) "invalid entrypoint selected");
    this.SetActive(false);
  }
}
