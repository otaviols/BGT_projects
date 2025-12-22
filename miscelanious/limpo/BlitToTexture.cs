using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class BlitToTexture : MonoBehaviour
{
  [SerializeField]
  private Vector2Int m_textureSize = new Vector2Int(256, 256);
  [SerializeField]
  private RenderTextureFormat m_renderTextureFormat = RenderTextureFormat.Default;
  [SerializeField]
  private Renderer m_drawAfterBlit;
  public bool AssignTextureAsRendererMainTex;
  public Vector2 Offset;
  public bool CenteredOffset;
  public bool OffsetFollowPosition;
  private Camera m_mainCamera;
  public float RotationDegrees;
  public float ZoomFactor = 1f;
  public bool ScaleZoomFrom1080P = true;
  private readonly BlitToTextureService.Request m_request = new BlitToTextureService.Request();

  public Renderer DrawAfterBlit
  {
    get => this.m_drawAfterBlit;
    set
    {
      this.m_drawAfterBlit = value;
      this.m_request.DrawAfterRenderer = this.m_drawAfterBlit;
    }
  }

  public RenderTexture TargetTexture { get; private set; }

  public BlitToTexture()
  {
  }

  public BlitToTexture(Vector2Int textureSize, RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default)
  {
    this.m_textureSize = textureSize;
    this.m_renderTextureFormat = renderTextureFormat;
  }

  protected virtual void Awake()
  {
    this.TargetTexture = RenderTextureTracker.Get().CreateNewTexture(this.m_textureSize.x, this.m_textureSize.y, 0, this.m_renderTextureFormat);
    this.m_request.TargetTexture = this.TargetTexture;
    this.m_request.DrawAfterRenderer = this.m_drawAfterBlit;
    if (!this.AssignTextureAsRendererMainTex || !((Object) this.m_drawAfterBlit != (Object) null))
      return;
    this.m_drawAfterBlit.GetMaterial().mainTexture = (Texture) this.TargetTexture;
  }

  private void OnDestroy() => RenderTextureTracker.Get().DestroyRenderTexture(this.TargetTexture);

  private void OnEnable() => BlitToTextureService.AddPersistentRequest(this.m_request);

  private void OnDisable() => BlitToTextureService.RemovePersistentRequest(this.m_request);

  protected virtual void Update()
  {
    float num = 1f;
    if (this.ScaleZoomFrom1080P)
      num = (float) CameraUtils.GetMainCamera().scaledPixelHeight / 1080f;
    this.m_request.Size = new Vector2((float) this.m_textureSize.x, (float) this.m_textureSize.y) * this.ZoomFactor * num;
    this.m_request.Offset = this.Offset;
    if (this.CenteredOffset)
      this.m_request.Offset -= this.m_request.Size / 2f;
    if (this.OffsetFollowPosition)
    {
      if ((Object) this.m_mainCamera == (Object) null)
        this.m_mainCamera = CameraUtils.GetMainCamera();
      this.Offset = (Vector2) this.m_mainCamera.WorldToScreenPoint(this.transform.position);
    }
    this.m_request.RotationDeg = this.RotationDegrees;
  }
}
