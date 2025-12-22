using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class CameraMask : MonoBehaviour
{
  [CustomEditField(Sections = "Mask Settings")]
  public GameObject m_ClipObjects;
  [CustomEditField(Sections = "Mask Settings")]
  public CameraMask.CAMERA_MASK_UP_VECTOR m_UpVector;
  [CustomEditField(Sections = "Mask Settings")]
  public float m_Width = 1f;
  [CustomEditField(Sections = "Mask Settings")]
  public float m_Height = 1f;
  [CustomEditField(Sections = "Mask Settings")]
  public bool m_RealtimeUpdate;
  [CustomEditField(Sections = "Render Camera")]
  public bool m_UseCameraFromLayer;
  [CustomEditField(Parent = "m_UseCameraFromLayer", Sections = "Render Camera")]
  public GameLayer m_CameraFromLayer;
  [CustomEditField(Sections = "Render Camera")]
  public List<GameLayer> m_CullingMasks = new List<GameLayer>()
  {
    GameLayer.Default,
    GameLayer.IgnoreFullScreenEffects
  };
  [CustomEditField(Sections = "Render Camera")]
  public CustomViewEntryPoint RenderEntryPoint = CustomViewEntryPoint.PerspectivePostFullscreenFX;
  private Camera m_renderCamera;
  private CameraOverridePass m_cameraMaskPass;

  private void OnEnable()
  {
    this.Init();
    this.ActivateMask();
  }

  private void OnDisable() => this.DeactivateMask();

  private void Update()
  {
    if (!this.m_RealtimeUpdate)
      return;
    this.UpdateCameraClipping();
  }

  private void OnDrawGizmos()
  {
    Matrix4x4 matrix4x4 = new Matrix4x4();
    if (this.m_UpVector == CameraMask.CAMERA_MASK_UP_VECTOR.Z)
      matrix4x4.SetTRS(this.transform.position, Quaternion.identity, this.transform.lossyScale);
    else
      matrix4x4.SetTRS(this.transform.position, Quaternion.Euler(90f, 0.0f, 0.0f), this.transform.lossyScale);
    Gizmos.matrix = matrix4x4;
    Gizmos.color = Color.magenta;
    Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.m_Width, this.m_Height, 0.0f));
    Gizmos.matrix = Matrix4x4.identity;
  }

  [ContextMenu("UpdateMask")]
  public void UpdateMask() => this.UpdateCameraClipping();

  private bool Init()
  {
    this.m_renderCamera = !this.m_UseCameraFromLayer ? CameraUtils.FindProjectionCameraForObject(this.gameObject) : CameraUtils.FindFirstByLayer(this.m_CameraFromLayer);
    if ((Object) this.m_renderCamera == (Object) null)
      return false;
    int layers = GameLayer.CameraMask.LayerBit();
    foreach (GameLayer cullingMask in this.m_CullingMasks)
      layers |= cullingMask.LayerBit();
    this.m_cameraMaskPass = new CameraOverridePass("CameraMask: " + this.gameObject.name, (LayerMask) layers);
    return true;
  }

  private void UpdateCameraClipping()
  {
    if ((Object) this.m_renderCamera == (Object) null && !this.Init())
      return;
    Vector3 position1 = Vector3.zero;
    Vector3 position2 = Vector3.zero;
    if (this.m_UpVector == CameraMask.CAMERA_MASK_UP_VECTOR.Y)
    {
      position1 = new Vector3(this.transform.position.x - this.m_Width * 0.5f * this.transform.lossyScale.x, this.transform.position.y, this.transform.position.z - this.m_Height * 0.5f * this.transform.lossyScale.z);
      position2 = new Vector3(this.transform.position.x + this.m_Width * 0.5f * this.transform.lossyScale.x, this.transform.position.y, this.transform.position.z + this.m_Height * 0.5f * this.transform.lossyScale.z);
    }
    else
    {
      position1 = new Vector3(this.transform.position.x - this.m_Width * 0.5f * this.transform.lossyScale.x, this.transform.position.y - this.m_Height * 0.5f * this.transform.lossyScale.y, this.transform.position.z);
      position2 = new Vector3(this.transform.position.x + this.m_Width * 0.5f * this.transform.lossyScale.x, this.transform.position.y + this.m_Height * 0.5f * this.transform.lossyScale.y, this.transform.position.z);
    }
    Vector3 viewportPoint1 = this.m_renderCamera.WorldToViewportPoint(position1);
    Vector3 viewportPoint2 = this.m_renderCamera.WorldToViewportPoint(position2);
    float x = Mathf.Clamp(viewportPoint1.x, 0.0f, 1f);
    float y = Mathf.Clamp(viewportPoint1.y, 0.0f, 1f);
    float num1 = Mathf.Clamp(viewportPoint2.x, 0.0f, 1f);
    float num2 = Mathf.Clamp(viewportPoint2.y, 0.0f, 1f);
    Rect scissor = new Rect(x, y, num1 - x, num2 - y);
    if (Mathf.Approximately(0.0f, scissor.height) || Mathf.Approximately(0.0f, scissor.width))
      return;
    scissor.Set(scissor.x * (float) this.m_renderCamera.pixelWidth, scissor.y * (float) this.m_renderCamera.pixelHeight, scissor.width * (float) this.m_renderCamera.pixelWidth, scissor.height * (float) this.m_renderCamera.pixelHeight);
    this.m_cameraMaskPass.OverrideScissor(scissor);
  }

  private void ActivateMask()
  {
    if (this.m_cameraMaskPass == null)
      return;
    if ((Object) this.m_ClipObjects != (Object) null)
      LayerUtils.SetLayer(this.m_ClipObjects, GameLayer.CameraMask);
    this.m_cameraMaskPass.Schedule(this.RenderEntryPoint);
  }

  private void DeactivateMask()
  {
    if (this.m_cameraMaskPass == null)
      return;
    this.m_cameraMaskPass.Unschedule();
    this.m_cameraMaskPass = (CameraOverridePass) null;
  }

  public enum CAMERA_MASK_UP_VECTOR
  {
    Y,
    Z,
  }
}
