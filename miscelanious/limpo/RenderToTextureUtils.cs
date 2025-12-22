using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class RenderToTextureUtils
{
  private static ProfilerMarker s_RTTRenderCamera = new ProfilerMarker("RTT_RenderCamera");

  public static Bounds CalcRendererBounds(Renderer[] toInclude)
  {
    Bounds bounds = new Bounds();
    foreach (Renderer renderer in toInclude)
    {
      if (bounds.size == Vector3.zero)
        bounds = renderer.bounds;
      else
        bounds.Encapsulate(renderer.bounds);
    }
    return bounds;
  }

  private static void DrawCommand(CommandBuffer cmd, LayerMask cullingMask, RenderCommand command)
  {
    int num = 1 << command.Renderer.gameObject.layer;
    if (((int) cullingMask & num) == 0 || !command.Renderer.enabled)
      return;
    cmd.DrawRenderer(command.Renderer, command.Material, command.MeshIndex, command.passIndex);
  }

  private static void DrawCommandReplacement(
    CommandBuffer cmd,
    LayerMask cullingMask,
    RenderCommand command,
    Material replacementShaderMaterial,
    string replacementTag,
    string replacementVal)
  {
    int num = 1 << command.Renderer.gameObject.layer;
    if (((int) cullingMask & num) == 0 || !command.Renderer.enabled || !(command.Material.GetTag(replacementTag, false) == replacementVal))
      return;
    cmd.DrawRenderer(command.Renderer, replacementShaderMaterial, command.MeshIndex);
  }

  public static void RenderCamera(
    CommandBuffer cmd,
    RenderTexture rt,
    RenderToTextureUtils.LightWeightCamera camera,
    RenderCommandLists renderCommands,
    Shader replacementShader = null,
    string replacementTag = "")
  {
    cmd.SetRenderTarget((RenderTargetIdentifier) (Texture) rt);
    cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
    cmd.ClearRenderTarget(true, true, camera.backgroundColor);
    if ((bool) (Object) replacementShader)
    {
      Material replacementShaderMaterial = new Material(replacementShader);
      string tag = replacementShaderMaterial.GetTag(replacementTag, false);
      foreach (RenderCommand opaqueRenderCommand in renderCommands.OpaqueRenderCommands)
        RenderToTextureUtils.DrawCommandReplacement(cmd, camera.cullingMask, opaqueRenderCommand, replacementShaderMaterial, replacementTag, tag);
      foreach (RenderCommand transparentRenderCommand in renderCommands.TransparentRenderCommands)
        RenderToTextureUtils.DrawCommandReplacement(cmd, camera.cullingMask, transparentRenderCommand, replacementShaderMaterial, replacementTag, tag);
    }
    else
    {
      foreach (RenderCommand opaqueRenderCommand in renderCommands.OpaqueRenderCommands)
        RenderToTextureUtils.DrawCommand(cmd, camera.cullingMask, opaqueRenderCommand);
      foreach (RenderCommand transparentRenderCommand in renderCommands.TransparentRenderCommands)
        RenderToTextureUtils.DrawCommand(cmd, camera.cullingMask, transparentRenderCommand);
    }
  }

  public class RenderCommandListPool
  {
    private static ObjectPool<RenderCommandLists> s_commandListPool = new ObjectPool<RenderCommandLists>((UnityAction<RenderCommandLists>) null, (UnityAction<RenderCommandLists>) (x => x.Clear()));

    public static RenderCommandLists Get() => RenderToTextureUtils.RenderCommandListPool.s_commandListPool.Get();

    public static RenderCommandLists Get(
      Renderer[] toDraw,
      RenderCommandLists.MatOverrideDictionary overrides = null)
    {
      RenderCommandLists renderCommandLists = RenderToTextureUtils.RenderCommandListPool.Get();
      renderCommandLists.AppendRenderCommands(toDraw, overrides);
      return renderCommandLists;
    }

    public static RenderCommandLists Get(
      GameObject objectToDraw,
      bool includeInactiveRenderers = false,
      RenderCommandLists.MatOverrideDictionary overrides = null)
    {
      RenderCommandLists renderCommandLists = RenderToTextureUtils.RenderCommandListPool.Get();
      renderCommandLists.AppendRenderCommands(objectToDraw, includeInactiveRenderers, overrides);
      return renderCommandLists;
    }

    public static void Release(RenderCommandLists list)
    {
      if (list == null)
        return;
      RenderToTextureUtils.RenderCommandListPool.s_commandListPool.Release(list);
    }
  }

  public struct LightWeightCamera
  {
    public Color backgroundColor;
    public LayerMask cullingMask;
    public float aspectRatio;

    public Matrix4x4 worldToCameraMatrix { get; set; }

    public Matrix4x4 projectionMatrix { get; set; }

    public LightWeightCamera(RenderToTextureUtils.LightWeightCamera rhs)
    {
      this.worldToCameraMatrix = rhs.worldToCameraMatrix;
      this.projectionMatrix = rhs.projectionMatrix;
      this.backgroundColor = rhs.backgroundColor;
      this.cullingMask = rhs.cullingMask;
      this.aspectRatio = rhs.aspectRatio;
    }

    public void SetOrthoProjectionMatrix(float orthographicSize, float nearClip, float farClip) => this.projectionMatrix = Matrix4x4.Ortho(-orthographicSize * this.aspectRatio, orthographicSize * this.aspectRatio, -orthographicSize, orthographicSize, nearClip, farClip);

    public void SetWorldToCameraMatrix(Transform obj) => this.worldToCameraMatrix = Matrix4x4.Inverse(Matrix4x4.TRS(obj.position, obj.rotation, new Vector3(1f, 1f, -1f)));
  }
}
