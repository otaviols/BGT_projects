using UnityEngine;

public static class BlitToTextureService
{
  public static void AddPersistentRequest(BlitToTextureService.Request request) => BlitToTextureFeature.Get().AddPersistentRequest(request);

  public static void RemovePersistentRequest(BlitToTextureService.Request request) => BlitToTextureFeature.Get().RemovePersistentRequest(request);

  public class Request
  {
    public Vector2 Offset;
    public Vector2 Size;
    public RenderTexture TargetTexture;
    public Renderer DrawAfterRenderer;
    public float RotationDeg;
  }
}
