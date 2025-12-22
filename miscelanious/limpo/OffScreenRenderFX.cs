using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class OffScreenRenderFX : MonoBehaviour
{
  public GameObject ObjectToRender;
  public bool UseBounds = true;
  public int RenderResolutionX = 256;
  public int RenderResolutionY = 256;
  public float AboveClip = 1f;
  public float BelowClip = 1f;
  public float ForceSize;
  public Rect CameraRect = new Rect(0.0f, 0.0f, 1f, 1f);
  private Camera offscreenFXCamera;
  private GameObject offscreenFXCameraGO;
  private RenderTexture tempRenderBuffer;
  private const int IgnoreLayer = 21;
  private float Yoffset;
  private Bounds RenderBounds;
  private static float s_Yoffset = 250f;
  private static float s_Xoffset = 250f;

  private void Start()
  {
    if (this.UseBounds)
      this.RenderBounds = this.GetComponent<MeshFilter>().mesh.bounds;
    this.Yoffset = OffScreenRenderFX.s_Yoffset;
    OffScreenRenderFX.s_Yoffset += 10f;
    this.PositionObjectToRender();
    this.CreateCamera();
    this.CreateRenderTexture();
    this.SetupCamera();
    this.SetupMaterial();
    this.GetComponent<Renderer>().enabled = true;
  }

  private void OnDestroy()
  {
    if (!((Object) this.tempRenderBuffer != (Object) null) || !this.tempRenderBuffer.IsCreated())
      return;
    RenderTexture.ReleaseTemporary(this.tempRenderBuffer);
  }

  private void CreateCamera()
  {
    if (!((Object) this.offscreenFXCameraGO == (Object) null))
      return;
    if ((Object) this.offscreenFXCamera != (Object) null)
      Object.Destroy((Object) this.offscreenFXCamera);
    this.offscreenFXCameraGO = new GameObject();
    this.offscreenFXCamera = this.offscreenFXCameraGO.AddComponent<Camera>();
    this.offscreenFXCameraGO.name = this.name + "_OffScreenFXCamera";
    GameObjectUtils.SetHideFlags((Object) this.offscreenFXCameraGO, HideFlags.HideAndDontSave);
    UniversalInputManager.Get().AddIgnoredCamera(this.offscreenFXCamera);
  }

  private void SetupCamera()
  {
    this.offscreenFXCamera.orthographic = true;
    this.UpdateOffScreenCamera();
    this.offscreenFXCamera.transform.parent = this.transform;
    this.offscreenFXCamera.nearClipPlane = -this.AboveClip;
    this.offscreenFXCamera.farClipPlane = this.BelowClip;
    this.offscreenFXCamera.targetTexture = this.tempRenderBuffer;
    this.offscreenFXCamera.depth = Camera.main.depth - 1f;
    this.offscreenFXCamera.backgroundColor = Color.black;
    this.offscreenFXCamera.clearFlags = CameraClearFlags.Color;
    this.offscreenFXCamera.rect = this.CameraRect;
    this.offscreenFXCamera.enabled = true;
    this.offscreenFXCamera.allowHDR = false;
  }

  private void UpdateOffScreenCamera()
  {
    if ((Object) this.ObjectToRender == (Object) null)
      return;
    if ((double) this.ForceSize == 0.0)
    {
      float x = this.transform.localScale.x;
      if (this.UseBounds)
        x *= this.RenderBounds.size.x;
      this.offscreenFXCamera.orthographicSize = x / 2f;
    }
    else
      this.offscreenFXCamera.orthographicSize = this.ForceSize;
    this.offscreenFXCameraGO.transform.position = this.ObjectToRender.transform.position;
    this.offscreenFXCameraGO.transform.rotation = this.ObjectToRender.transform.rotation;
    this.offscreenFXCameraGO.transform.Rotate(90f, 180f, 0.0f);
  }

  private void CreateRenderTexture() => this.tempRenderBuffer = RenderTexture.GetTemporary(this.RenderResolutionX, this.RenderResolutionY);

  private void SetupMaterial() => this.gameObject.GetComponent<Renderer>().GetMaterial().mainTexture = (Texture) this.tempRenderBuffer;

  private void PositionObjectToRender()
  {
    Vector3 vector3_1 = Vector3.up * this.Yoffset;
    Vector3 vector3_2 = Vector3.right * OffScreenRenderFX.s_Xoffset;
    if ((Object) this.ObjectToRender != (Object) null)
      this.ObjectToRender.transform.position += vector3_1;
    this.ObjectToRender.transform.position += vector3_2;
  }
}
