using System;
using UnityEngine;
using UnityEngine.Rendering;

public class FreezeFrame : MonoBehaviour
{
  private const int NO_WORK_FRAMES_BEFORE_DEACTIVATE = 2;
  public bool m_FrozenState;
  public bool m_CaptureFrozenImage;
  public int m_DeactivateFrameCount;
  public RenderTexture m_FrozenScreenTexture;
  private UniversalInputManager m_UniversalInputManager;
  private Camera m_Camera;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void OnEnable() => RenderPipelineManager.endCameraRendering += new Action<ScriptableRenderContext, Camera>(this.EndCameraRendering);

  protected void OnDisable()
  {
    if (this.m_FrozenState)
      this.Unfreeze();
    FullScreenFXMgr fullScreenFxMgr = FullScreenFXMgr.Get();
    if (fullScreenFxMgr != null && (bool) (UnityEngine.Object) fullScreenFxMgr.ActiveCameraFullScreenEffects)
      fullScreenFxMgr.ActiveCameraFullScreenEffects.Disable();
    RenderPipelineManager.endCameraRendering -= new Action<ScriptableRenderContext, Camera>(this.EndCameraRendering);
  }

  protected void Awake()
  {
    this.m_Camera = this.GetComponent<Camera>();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected void Start() => this.gameObject.GetComponent<Camera>().clearFlags = CameraClearFlags.Color;

  public void Disable()
  {
    this.enabled = false;
    FullScreenFXMgr fullScreenFxMgr = FullScreenFXMgr.Get();
    if (fullScreenFxMgr == null)
      return;
    if ((bool) (UnityEngine.Object) fullScreenFxMgr.ActiveCameraFullScreenEffects)
      fullScreenFxMgr.ActiveCameraFullScreenEffects.Disable();
    fullScreenFxMgr.ForceReset();
  }

  [ContextMenu("Freeze")]
  public void Freeze()
  {
    this.enabled = true;
    if (this.m_FrozenState)
      return;
    FullScreenFXMgr fullScreenFxMgr = FullScreenFXMgr.Get();
    if (fullScreenFxMgr != null && (bool) (UnityEngine.Object) fullScreenFxMgr.ActiveCameraFullScreenEffects && !fullScreenFxMgr.ActiveCameraFullScreenEffects.HasActiveEffects)
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurDesaturatePerspective with
      {
        Time = 0.0f,
        Blur = new BlurParameters(1.5f, 1f)
      });
    this.m_CaptureFrozenImage = true;
    this.m_FrozenScreenTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
    this.m_FrozenScreenTexture.filterMode = FilterMode.Point;
    this.m_FrozenScreenTexture.wrapMode = TextureWrapMode.Clamp;
  }

  [ContextMenu("Unfreeze")]
  public void Unfreeze()
  {
    this.m_screenEffectsHandle.StopEffect();
    this.m_FrozenState = false;
    if ((UnityEngine.Object) this.m_FrozenScreenTexture != (UnityEngine.Object) null)
    {
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_FrozenScreenTexture);
      this.m_FrozenScreenTexture = (RenderTexture) null;
    }
    this.Disable();
  }

  public bool isActive() => this.enabled && this.m_FrozenState;

  private void EndCameraRendering(ScriptableRenderContext context, Camera camera)
  {
    if ((UnityEngine.Object) this.m_Camera != (UnityEngine.Object) camera || this.m_FrozenState)
      return;
    if (this.m_DeactivateFrameCount > 2)
    {
      this.m_DeactivateFrameCount = 0;
      this.Disable();
    }
    else
      ++this.m_DeactivateFrameCount;
  }
}
