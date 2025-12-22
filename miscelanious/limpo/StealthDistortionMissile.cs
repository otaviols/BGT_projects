using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class StealthDistortionMissile : MonoBehaviour
{
  public GameObject ObjectToRender;
  public int ParticleResolutionX = 256;
  public int ParticleResolutionY = 256;
  public float ParticleAboveClip = 1f;
  public float ParticleBelowClip = 1f;
  public float RenderOffsetX;
  public float RenderOffsetY;
  public float RenderOffsetZ;
  public int DistortionResolutionX = 256;
  public int DistortionResolutionY = 256;
  public float DistortionAboveClip = -0.1f;
  public float DistortionBelowClip = 10f;
  private Camera particleCamera;
  private GameObject particleCameraGO;
  private RenderTexture particleRenderBuffer;
  private Camera boardCamera;
  private GameObject boardCameraGO;
  private RenderTexture boardRenderBuffer;
  private const int IgnoreLayer = 21;
  private float Yoffset;
  private static float s_Yoffset = 50f;

  private void Start()
  {
    this.Yoffset = StealthDistortionMissile.s_Yoffset;
    StealthDistortionMissile.s_Yoffset += 10f;
    this.PositionObjectToRender();
    this.CreateCameras();
    this.CreateRenderTextures();
    this.SetupCameras();
    this.SetupMaterial();
    this.GetComponent<Renderer>().enabled = true;
  }

  private void OnDestroy()
  {
    RenderTexture.ReleaseTemporary(this.particleRenderBuffer);
    RenderTexture.ReleaseTemporary(this.boardRenderBuffer);
  }

  private void CreateCameras()
  {
    if ((Object) this.particleCameraGO == (Object) null)
    {
      if ((Object) this.particleCamera != (Object) null)
        Object.Destroy((Object) this.particleCamera);
      this.particleCameraGO = new GameObject();
      this.particleCamera = this.particleCameraGO.AddComponent<Camera>();
      this.particleCameraGO.name = this.name + "_DistortionParticleFXCamera";
    }
    if (!((Object) this.boardCameraGO == (Object) null))
      return;
    if ((Object) this.boardCamera != (Object) null)
      Object.Destroy((Object) this.boardCamera);
    this.boardCameraGO = new GameObject();
    this.boardCamera = this.boardCameraGO.AddComponent<Camera>();
    this.boardCameraGO.name = this.name + "_DistortionBoardFXCamera";
  }

  private void SetupCameras()
  {
    this.particleCamera.orthographic = true;
    this.particleCamera.orthographicSize = this.transform.localScale.x / 2f;
    this.particleCameraGO.transform.position = this.ObjectToRender.transform.position;
    this.particleCameraGO.transform.Translate(this.RenderOffsetX, this.RenderOffsetY, this.RenderOffsetZ);
    this.particleCameraGO.transform.rotation = this.ObjectToRender.transform.rotation;
    this.particleCameraGO.transform.Rotate(90f, 180f, 0.0f);
    this.particleCamera.transform.parent = this.transform;
    this.particleCamera.nearClipPlane = -this.ParticleAboveClip;
    this.particleCamera.farClipPlane = this.ParticleBelowClip;
    this.particleCamera.targetTexture = this.particleRenderBuffer;
    this.particleCamera.depth = Camera.main.depth - 1f;
    this.particleCamera.backgroundColor = Color.black;
    this.particleCamera.clearFlags = CameraClearFlags.Color;
    this.particleCamera.cullingMask &= -2097153;
    this.particleCamera.enabled = true;
    this.particleCamera.allowHDR = false;
    this.boardCamera.orthographic = true;
    this.boardCamera.orthographicSize = this.transform.localScale.x / 2f;
    this.boardCameraGO.transform.position = this.transform.position;
    this.boardCameraGO.transform.rotation = this.transform.rotation;
    this.boardCameraGO.transform.Rotate(90f, 180f, 0.0f);
    this.boardCamera.transform.parent = this.transform;
    this.boardCamera.nearClipPlane = -this.DistortionAboveClip;
    this.boardCamera.farClipPlane = this.DistortionBelowClip;
    this.boardCamera.targetTexture = this.boardRenderBuffer;
    this.boardCamera.depth = Camera.main.depth - 1f;
    this.boardCamera.backgroundColor = Color.black;
    this.boardCamera.clearFlags = CameraClearFlags.Color;
    this.boardCamera.cullingMask &= -2097153;
    this.boardCamera.enabled = true;
    this.boardCamera.allowHDR = false;
  }

  private void CreateRenderTextures()
  {
    this.particleRenderBuffer = RenderTexture.GetTemporary(this.ParticleResolutionX, this.ParticleResolutionY);
    this.boardRenderBuffer = RenderTexture.GetTemporary(this.DistortionResolutionX, this.DistortionResolutionY);
  }

  private void SetupMaterial()
  {
    Material material = this.gameObject.GetComponent<Renderer>().GetMaterial();
    material.mainTexture = (Texture) this.boardRenderBuffer;
    material.SetTexture("_ParticleTex", (Texture) this.particleRenderBuffer);
  }

  private void PositionObjectToRender() => this.ObjectToRender.transform.position += Vector3.up * this.Yoffset;
}
