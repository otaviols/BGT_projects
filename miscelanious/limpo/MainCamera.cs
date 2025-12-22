using Blizzard.T5.Services;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MainCamera : MonoBehaviour
{
  public Camera m_camera;

  private void Awake()
  {
    if ((Object) this.m_camera == (Object) null)
      return;
    Camera firstByLayer = CameraUtils.FindFirstByLayer(GameLayer.BattleNet);
    if ((Object) firstByLayer != (Object) null)
    {
      UniversalAdditionalCameraData additionalCameraData = this.m_camera.GetUniversalAdditionalCameraData();
      if (!additionalCameraData.cameraStack.Contains(firstByLayer))
        additionalCameraData.cameraStack.Add(firstByLayer);
    }
    this.m_camera.allowMSAA = ServiceManager.Get<IGraphicsManager>().AllowMSAA();
    CameraManager.Get().BaseCamera = this.m_camera;
  }

  private void OnDestroy()
  {
    if (!CameraManager.IsInitialized())
      return;
    CameraManager.Get().BaseCamera = (Camera) null;
  }
}
