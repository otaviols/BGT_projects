using Blizzard.T5.Services;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BoxCamera : MonoBehaviour
{
  private Box m_parent;
  private BoxCameraStateInfo m_info;
  private BoxCamera.State m_state;
  private bool m_disableAccelerometer = true;
  private bool m_applyAccelerometer;
  private Vector2 m_currentAngle;
  private Vector3 m_basePosition;
  private Vector2 m_gyroRotation;
  private float m_offset;
  private float MAX_GYRO_RANGE = 2.1f;
  private float ROTATION_SCALE = 0.085f;
  private Vector3 m_lookAtPoint;
  private Camera m_camera;

  private void Awake()
  {
    this.m_camera = this.GetComponent<Camera>();
    if (!(bool) (Object) this.m_camera)
      Log.All.PrintError("BoxCamera: m_camera is null.");
    Camera firstByLayer = CameraUtils.FindFirstByLayer(GameLayer.BattleNet);
    if ((Object) firstByLayer != (Object) null)
    {
      UniversalAdditionalCameraData additionalCameraData = this.m_camera.GetUniversalAdditionalCameraData();
      if (!additionalCameraData.cameraStack.Contains(firstByLayer))
        additionalCameraData.cameraStack.Add(firstByLayer);
    }
    if (!((Object) this.m_camera != (Object) null))
      return;
    this.m_camera.allowMSAA = ServiceManager.Get<IGraphicsManager>().AllowMSAA();
    CameraManager.Get().BaseCamera = this.m_camera;
  }

  public void SetParent(Box parent) => this.m_parent = parent;

  public Box GetParent() => this.m_parent;

  public BoxCameraStateInfo GetInfo() => this.m_info;

  public void SetInfo(BoxCameraStateInfo info) => this.m_info = info;

  public Vector3 GetCameraPosition(BoxCamera.State state)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      Vector3 position1;
      Vector3 position2;
      Vector3 position3;
      switch (state)
      {
        case BoxCamera.State.CLOSED:
          position1 = this.m_info.m_ClosedMinAspectRatioBone.transform.position;
          position2 = this.m_info.m_ClosedBone.transform.position;
          position3 = this.m_info.m_ClosedExtraWideAspectRatioBone.transform.position;
          break;
        case BoxCamera.State.CLOSED_TUTORIAL_VIDEO_PREVIEW:
          position1 = this.m_info.m_ClosedTutorialPreviewRightMinAspectRatioBone.transform.position;
          position2 = this.m_info.m_ClosedTutorialPreviewRightBone.transform.position;
          position3 = this.m_info.m_ClosedTutorialPreviewRightExtraWideAspectRatioBone.transform.position;
          break;
        case BoxCamera.State.CLOSED_WITH_DRAWER:
          position1 = this.m_info.m_ClosedWithDrawerMinAspectRatioBone.transform.position;
          position2 = this.m_info.m_ClosedWithDrawerBone.transform.position;
          position3 = this.m_info.m_ClosedWithDrawerExtraWideAspectRatioBone.transform.position;
          break;
        default:
          position1 = this.m_info.m_OpenedMinAspectRatioBone.transform.position;
          position2 = this.m_info.m_OpenedBone.transform.position;
          position3 = this.m_info.m_OpenedExtraWideAspectRatioBone.transform.position;
          break;
      }
      return TransformUtil.GetAspectRatioDependentPosition(position1, position2, position3);
    }
    switch (state)
    {
      case BoxCamera.State.CLOSED:
        return this.m_info.m_ClosedBone.transform.position;
      case BoxCamera.State.CLOSED_TUTORIAL:
        return this.m_info.m_ClosedTutorialBone.transform.position;
      case BoxCamera.State.CLOSED_TUTORIAL_VIDEO_PREVIEW:
        return this.m_info.m_ClosedTutorialPreviewRightBone.transform.position;
      case BoxCamera.State.CLOSED_WITH_DRAWER:
        return this.m_info.m_ClosedWithDrawerBone.transform.position;
      default:
        return this.m_info.m_OpenedBone.transform.position;
    }
  }

  public BoxCamera.State GetState() => this.m_state;

  public bool ChangeState(BoxCamera.State state)
  {
    if (this.m_state == state)
      return false;
    Vector3 cameraPosition = this.GetCameraPosition(state);
    this.m_parent.OnAnimStarted();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_applyAccelerometer = false;
      this.m_basePosition = this.transform.parent.InverseTransformPoint(cameraPosition);
      this.m_lookAtPoint = this.transform.parent.InverseTransformPoint(new Vector3(cameraPosition.x, 1.5f, cameraPosition.z));
      if (cameraPosition == this.gameObject.transform.position)
      {
        this.OnAnimFinished(state);
        return true;
      }
    }
    Hashtable args = (Hashtable) null;
    switch (state)
    {
      case BoxCamera.State.CLOSED:
        args = iTween.Hash((object) "position", (object) cameraPosition, (object) "delay", (object) this.m_info.m_ClosedDelaySec, (object) "time", (object) this.m_info.m_ClosedMoveSec, (object) "easeType", (object) this.m_info.m_ClosedMoveEaseType, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompleteparams", (object) state, (object) "oncompletetarget", (object) this.gameObject);
        break;
      case BoxCamera.State.CLOSED_TUTORIAL:
        args = iTween.Hash((object) "position", (object) cameraPosition, (object) "delay", (object) this.m_info.m_ClosedDelaySec, (object) "time", (object) this.m_info.m_ClosedMoveSec, (object) "easeType", (object) this.m_info.m_ClosedMoveEaseType, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompleteparams", (object) state, (object) "oncompletetarget", (object) this.gameObject);
        break;
      case BoxCamera.State.CLOSED_TUTORIAL_VIDEO_PREVIEW:
        args = iTween.Hash((object) "position", (object) cameraPosition, (object) "delay", (object) this.m_info.m_ClosedDelaySec, (object) "time", (object) this.m_info.m_ClosedMoveSec, (object) "easeType", (object) this.m_info.m_ClosedMoveEaseType, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompleteparams", (object) state, (object) "oncompletetarget", (object) this.gameObject);
        break;
      case BoxCamera.State.CLOSED_WITH_DRAWER:
        args = iTween.Hash((object) "position", (object) cameraPosition, (object) "delay", (object) this.m_info.m_ClosedWithDrawerDelaySec, (object) "time", (object) this.m_info.m_ClosedWithDrawerMoveSec, (object) "easeType", (object) this.m_info.m_ClosedWithDrawerMoveEaseType, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompleteparams", (object) state, (object) "oncompletetarget", (object) this.gameObject);
        break;
      case BoxCamera.State.OPENED:
        args = iTween.Hash((object) "position", (object) cameraPosition, (object) "delay", (object) this.m_info.m_OpenedDelaySec, (object) "time", (object) this.m_info.m_OpenedMoveSec, (object) "easeType", (object) this.m_info.m_OpenedMoveEaseType, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompleteparams", (object) state, (object) "oncompletetarget", (object) this.gameObject);
        break;
      case BoxCamera.State.SET_ROTATION_OPENED:
        args = iTween.Hash((object) "position", (object) cameraPosition, (object) "delay", (object) this.m_info.m_OpenedDelaySec, (object) "time", (object) 1.5f, (object) "easeType", (object) this.m_info.m_OpenedMoveEaseType, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompleteparams", (object) state, (object) "oncompletetarget", (object) this.gameObject);
        break;
    }
    CameraShakeMgr.Stop(this.m_camera);
    iTween.MoveTo(this.gameObject, args);
    return true;
  }

  public void EnableAccelerometer()
  {
  }

  public void Update()
  {
    if (this.m_disableAccelerometer || (Object) this.transform.parent.gameObject.GetComponent<LoadingScreen>() != (Object) null || !(bool) UniversalInputManager.UsePhoneUI)
      return;
    if (this.m_applyAccelerometer)
    {
      this.m_gyroRotation.x = Input.gyro.rotationRateUnbiased.x;
      this.m_gyroRotation.y = -Input.gyro.rotationRateUnbiased.y;
      this.m_currentAngle.x += this.m_gyroRotation.y * this.ROTATION_SCALE;
      this.m_currentAngle.y += this.m_gyroRotation.x * this.ROTATION_SCALE;
      this.m_currentAngle.x = Mathf.Clamp(this.m_currentAngle.x, -this.MAX_GYRO_RANGE, this.MAX_GYRO_RANGE);
      this.m_currentAngle.y = Mathf.Clamp(this.m_currentAngle.y, -this.MAX_GYRO_RANGE, this.MAX_GYRO_RANGE);
      this.gameObject.transform.localPosition = new Vector3(this.m_basePosition.x, this.m_basePosition.y, this.m_basePosition.z + this.m_currentAngle.y);
    }
    Vector3 worldUp = new Vector3(0.0f, 0.0f, 1f);
    this.gameObject.transform.LookAt(this.gameObject.transform.parent.TransformPoint(this.m_lookAtPoint), worldUp);
  }

  private void OnDestroy()
  {
    if (!CameraManager.IsInitialized())
      return;
    CameraManager.Get().BaseCamera = (Camera) null;
  }

  public void OnAnimFinished(BoxCamera.State state)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_applyAccelerometer = this.m_state != BoxCamera.State.OPENED;
      this.m_currentAngle = new Vector2(0.0f, 0.0f);
    }
    this.m_state = state;
    this.m_parent.OnAnimFinished();
  }

  public void UpdateState(BoxCamera.State state)
  {
    this.m_state = state;
    this.transform.position = this.GetCameraPosition(state);
  }

  public enum State
  {
    UNKNOWN = -1, // 0xFFFFFFFF
    CLOSED = 0,
    CLOSED_TUTORIAL = 1,
    CLOSED_TUTORIAL_VIDEO_PREVIEW = 2,
    CLOSED_WITH_DRAWER = 3,
    OPENED = 4,
    SET_ROTATION_OPENED = 5,
  }
}
