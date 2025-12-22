using UnityEngine;

[ExecuteAlways]
public class ScreenEffectsRender : MonoBehaviour
{
  private const int GLOW_RANDER_BUFFER_RESOLUTION = 256;
  public Camera m_EffectsObjectsCamera;
  public bool m_Debug;
  public RenderTexture m_MaskRenderTexture;
  private int m_width;
  private int m_height;
  private int m_previousWidth;
  private int m_previousHeight;

  private void Awake()
  {
    if (ScreenEffectsMgr.Get() == null)
      this.enabled = false;
    this.m_EffectsObjectsCamera = this.GetComponent<Camera>();
  }

  private void Update()
  {
    if ((Object) this.m_EffectsObjectsCamera == (Object) null)
    {
      this.enabled = false;
    }
    else
    {
      int width = (int) (256.0 * (double) ((float) Screen.width / (float) Screen.height));
      int height = 256;
      if (width != this.m_previousWidth || height != this.m_previousHeight)
      {
        RenderTextureTracker.Get().DestroyRenderTexture(this.m_MaskRenderTexture);
        this.m_MaskRenderTexture = (RenderTexture) null;
      }
      if (!((Object) this.m_MaskRenderTexture == (Object) null))
        return;
      this.m_MaskRenderTexture = RenderTextureTracker.Get().CreateNewTexture(width, height, RenderTextureTracker.TEXTURE_DEPTH, RenderTextureFormat.ARGB32);
      this.m_MaskRenderTexture.filterMode = FilterMode.Bilinear;
      this.m_MaskRenderTexture.useMipMap = true;
      this.m_previousWidth = width;
      this.m_previousHeight = height;
    }
  }

  private void OnDisable()
  {
    if (!((Object) this.m_MaskRenderTexture != (Object) null))
      return;
    RenderTextureTracker.Get().DestroyRenderTexture(this.m_MaskRenderTexture);
    this.m_MaskRenderTexture = (RenderTexture) null;
  }
}
