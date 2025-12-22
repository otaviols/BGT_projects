using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DiamondRenderToTextureAtlas : GreedyPacker
{
  private const int RTT_DEPTH_BUFFER_SIZE = 16;
  private const float PADDING = 0.5f;
  private const float MARGIN = 1f;
  private readonly Vector2Int m_size;
  private readonly int m_totalPixelSpace;
  private int m_totalPixelUsed;
  private CommandBuffer m_opaqueCommandBuffer;
  private CommandBuffer m_transparentCommandBuffer;
  private List<RenderToTexturePostProcess> m_renderPostProcessList;

  public RenderTexture Texture { get; }

  public bool IsRealTime { get; private set; }

  public bool Dirty { get; set; }

  public List<DiamondRenderToTextureAtlas.RegisteredTexture> RegisteredTextures { get; private set; }

  public Color ClearColor { get; private set; } = Color.clear;

  public DiamondRenderToTextureAtlas(int index, int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGB32)
    : base(width, height)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CIndex\u003Ek__BackingField = index;
    this.Texture = RenderTextureTracker.Get().CreateNewTexture(width, height, 16, format);
    this.Texture.useMipMap = false;
    this.Texture.autoGenerateMips = false;
    this.m_size = new Vector2Int(width, height);
    this.m_totalPixelSpace = width * height;
    this.RegisteredTextures = new List<DiamondRenderToTextureAtlas.RegisteredTexture>();
    this.m_renderPostProcessList = new List<RenderToTexturePostProcess>();
  }

  public bool Insert(DiamondRenderToTexture r2t)
  {
    int freeSpace = this.GetFreeSpace();
    Vector2Int textureSize = r2t.TextureSize;
    int x1 = textureSize.x;
    textureSize = r2t.TextureSize;
    int y1 = textureSize.y;
    if (x1 * y1 > freeSpace)
      return false;
    bool flag = this.RegisteredTextures.Count == 0;
    if (!flag && !this.UsesSamePostProcess(r2t))
      return false;
    if (flag)
      this.ClearColor = r2t.m_ClearColor;
    RectInt rectInt = this.Insert(r2t.TextureSize.x, r2t.TextureSize.y);
    if (rectInt.x == -1 && rectInt.y == -1)
      return false;
    this.RegisteredTextures.Add(new DiamondRenderToTextureAtlas.RegisteredTexture()
    {
      DiamondRenderToTexture = r2t,
      AtlasPosition = rectInt
    });
    DiamondRenderToTexture diamondRenderToTexture = r2t;
    RenderTexture texture = this.Texture;
    double x2 = (double) rectInt.xMin / (double) this.m_size.x;
    double yMin = (double) rectInt.yMin;
    Vector2Int size = this.m_size;
    double y2 = (double) size.y;
    double y3 = yMin / y2;
    double width1 = (double) rectInt.width;
    size = this.m_size;
    double x3 = (double) size.x;
    double width2 = width1 / x3;
    double height1 = (double) rectInt.height;
    size = this.m_size;
    double y4 = (double) size.y;
    double height2 = height1 / y4;
    Rect atlasUV = new Rect((float) x2, (float) y3, (float) width2, (float) height2);
    diamondRenderToTexture.OnAddedToAtlas(texture, atlasUV);
    this.BuildCommandBuffers();
    if (flag)
      this.SetupPostProcess(r2t);
    this.m_totalPixelUsed += rectInt.width * rectInt.height;
    if (r2t.m_RealtimeRender)
      this.IsRealTime = true;
    this.Dirty = true;
    return true;
  }

  public bool Remove(DiamondRenderToTexture r2t)
  {
    for (int index = this.RegisteredTextures.Count - 1; index >= 0; --index)
    {
      DiamondRenderToTextureAtlas.RegisteredTexture registeredTexture = this.RegisteredTextures[index];
      if (!(bool) (Object) registeredTexture.DiamondRenderToTexture)
        this.RegisteredTextures.RemoveAt(index);
      else if (registeredTexture.DiamondRenderToTexture.GetInstanceID() == r2t.GetInstanceID())
      {
        Vector2Int textureSize = registeredTexture.DiamondRenderToTexture.TextureSize;
        this.Remove(registeredTexture.AtlasPosition);
        this.m_totalPixelUsed -= textureSize.x * textureSize.y;
        this.RegisteredTextures.RemoveAt(index);
        this.BuildCommandBuffers();
        this.Dirty = true;
        return true;
      }
    }
    return false;
  }

  public bool IsEmpty() => this.RegisteredTextures.Count == 0;

  public void Destroy()
  {
    RenderTextureTracker.Get().ReleaseRenderTexture(this.Texture);
    foreach (RenderToTexturePostProcess renderPostProcess in this.m_renderPostProcessList)
      renderPostProcess.End();
  }

  public void Render()
  {
    Camera.SetupCurrent(CameraUtils.GetMainCamera());
    Graphics.ExecuteCommandBuffer(this.m_opaqueCommandBuffer);
    if (this.m_renderPostProcessList.Count > 0)
    {
      foreach (RenderToTexturePostProcess renderPostProcess in this.m_renderPostProcessList)
        renderPostProcess.AddCommandBuffers();
    }
    Graphics.ExecuteCommandBuffer(this.m_transparentCommandBuffer);
  }

  private void BuildCommandBuffers()
  {
    this.m_opaqueCommandBuffer = new CommandBuffer()
    {
      name = "AtlasOpaqueRender"
    };
    this.m_transparentCommandBuffer = new CommandBuffer()
    {
      name = "AtlasTransparentRender"
    };
    Matrix4x4 proj = Matrix4x4.Ortho(-3.45f, 3.45f, -3.45f, 3.45f, -0.3f, 15f);
    Vector3 from = new Vector3(-4000f, -3990f, -4000f);
    Matrix4x4 view = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, 1f, -1f)) * Matrix4x4.LookAt(from, from + Vector3.down, Vector3.forward).inverse;
    this.m_opaqueCommandBuffer.SetViewProjectionMatrices(view, proj);
    this.m_transparentCommandBuffer.SetViewProjectionMatrices(view, proj);
    this.m_opaqueCommandBuffer.SetRenderTarget((RenderTargetIdentifier) (UnityEngine.Texture) this.Texture);
    this.m_opaqueCommandBuffer.ClearRenderTarget(true, true, this.ClearColor);
    this.m_transparentCommandBuffer.SetRenderTarget((RenderTargetIdentifier) (UnityEngine.Texture) this.Texture, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.DontCare);
    foreach (DiamondRenderToTextureAtlas.RegisteredTexture registeredTexture in this.RegisteredTextures)
    {
      if ((bool) (Object) registeredTexture.DiamondRenderToTexture && registeredTexture.DiamondRenderToTexture.enabled)
      {
        RectInt atlasPosition = registeredTexture.AtlasPosition;
        Rect scissor = new Rect((float) atlasPosition.xMin + 0.5f, (float) atlasPosition.yMin + 0.5f, (float) atlasPosition.width - 1f, (float) atlasPosition.height - 1f);
        this.m_opaqueCommandBuffer.EnableScissorRect(scissor);
        this.m_transparentCommandBuffer.EnableScissorRect(scissor);
        foreach (RenderCommand opaqueRenderCommand in registeredTexture.DiamondRenderToTexture.RenderCommands.OpaqueRenderCommands)
          this.m_opaqueCommandBuffer.DrawRenderer(opaqueRenderCommand.Renderer, opaqueRenderCommand.Material, opaqueRenderCommand.MeshIndex, opaqueRenderCommand.passIndex);
        foreach (RenderCommand transparentRenderCommand in registeredTexture.DiamondRenderToTexture.RenderCommands.TransparentRenderCommands)
          this.m_transparentCommandBuffer.DrawRenderer(transparentRenderCommand.Renderer, transparentRenderCommand.Material, transparentRenderCommand.MeshIndex, transparentRenderCommand.passIndex);
        this.m_opaqueCommandBuffer.DisableScissorRect();
        this.m_transparentCommandBuffer.DisableScissorRect();
      }
    }
  }

  private int GetFreeSpace() => this.m_totalPixelSpace - this.m_totalPixelUsed;

  private bool UsesSamePostProcess(DiamondRenderToTexture r2t)
  {
    if (r2t.m_ClearColor != this.ClearColor)
      return false;
    foreach (RenderToTexturePostProcess renderPostProcess in this.m_renderPostProcessList)
    {
      if (!renderPostProcess.IsUsedBy(r2t))
        return false;
    }
    return true;
  }

  private void SetupPostProcess(DiamondRenderToTexture r2t)
  {
    if (!r2t.m_OpaqueObjectAlphaFill)
      return;
    RenderToTextureOpaqueAlphaFill textureOpaqueAlphaFill = new RenderToTextureOpaqueAlphaFill();
    textureOpaqueAlphaFill.Init(this.m_size.x, this.m_size.y, r2t.m_ClearColor, this.Texture);
    this.m_renderPostProcessList.Add((RenderToTexturePostProcess) textureOpaqueAlphaFill);
  }

  public struct RegisteredTexture
  {
    public DiamondRenderToTexture DiamondRenderToTexture;
    public RectInt AtlasPosition;
  }
}
