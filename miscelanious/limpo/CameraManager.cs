using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraManager : IService
{
  private static CameraManager s_instance;
  private Camera m_pegUICamera;
  private UniversalAdditionalCameraData m_pegUICameraData;
  private bool m_haveUICamera;
  private int m_cameraCount;
  private Camera m_baseCamera;

  public Camera BaseCamera
  {
    set
    {
      this.m_baseCamera = value;
      if ((UnityEngine.Object) this.m_baseCamera != (UnityEngine.Object) null)
      {
        UniversalAdditionalCameraData additionalCameraData = this.m_baseCamera.GetUniversalAdditionalCameraData();
        if ((UnityEngine.Object) additionalCameraData != (UnityEngine.Object) null && !additionalCameraData.cameraStack.Contains(this.m_pegUICamera))
          additionalCameraData.cameraStack.Add(this.m_pegUICamera);
        ++this.m_cameraCount;
      }
      else
        --this.m_cameraCount;
      if (!this.m_haveUICamera)
        return;
      if (this.m_cameraCount > 0)
        this.m_pegUICameraData.renderType = CameraRenderType.Overlay;
      else
        this.m_pegUICameraData.renderType = CameraRenderType.Base;
    }
  }

  public static CameraManager Get()
  {
    if (CameraManager.s_instance == null)
      CameraManager.s_instance = ServiceManager.Get<CameraManager>();
    return CameraManager.s_instance;
  }

  public static bool IsInitialized() => CameraManager.s_instance != null;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    PegUI pegUi = PegUI.Get();
    if ((UnityEngine.Object) pegUi != (UnityEngine.Object) null && (UnityEngine.Object) pegUi.orthographicUICam != (UnityEngine.Object) null)
    {
      this.m_pegUICamera = pegUi.orthographicUICam;
      UniversalAdditionalCameraData additionalCameraData = pegUi.orthographicUICam.GetUniversalAdditionalCameraData();
      if ((UnityEngine.Object) additionalCameraData != (UnityEngine.Object) null)
      {
        this.m_pegUICameraData = additionalCameraData;
        this.m_haveUICamera = true;
      }
    }
    if (!this.m_haveUICamera)
    {
      Debug.LogError((object) "Couldn't find orthographic UI camera");
      yield break;
    }
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown() => CameraManager.s_instance = (CameraManager) null;
}
