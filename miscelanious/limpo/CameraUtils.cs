using System;
using UnityEngine;

public class CameraUtils
{
  private static Camera[] s_cameras = new Camera[20];

  public static Camera FindFirstByLayer(int layer) => CameraUtils.FindFirstByLayerMask((LayerMask) (1 << layer));

  public static Camera FindFirstByLayer(GameLayer layer) => CameraUtils.FindFirstByLayerMask((LayerMask) layer.LayerBit());

  public static Camera FindFirstByLayerMask(LayerMask mask)
  {
    int length = CameraUtils.s_cameras.Length;
    int allCamerasCount = Camera.allCamerasCount;
    int num = allCamerasCount;
    if (length < num)
      CameraUtils.s_cameras = new Camera[allCamerasCount];
    int allCameras = Camera.GetAllCameras(CameraUtils.s_cameras);
    for (int index = 0; index < allCameras; ++index)
    {
      Camera camera = CameraUtils.s_cameras[index];
      if ((camera.cullingMask & (int) mask) != 0)
        return camera;
    }
    return (Camera) null;
  }

  public static Camera FindProjectionCameraForObject(GameObject obj) => OverlayUI.Get().HasObject(obj) ? OverlayUI.Get().m_UICamera : CameraUtils.GetMainCamera();

  public static Camera FindFullScreenEffectsCamera(bool activeOnly)
  {
    GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("MainCamera");
    if ((UnityEngine.Object) gameObjectWithTag == (UnityEngine.Object) null)
      return (Camera) null;
    FullScreenEffects component = gameObjectWithTag.GetComponent<FullScreenEffects>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (Camera) null;
    return !activeOnly || component.IsActive ? component.Camera : (Camera) null;
  }

  public static Plane CreateTopPlane(Camera camera)
  {
    Vector3 worldPoint1 = camera.ViewportToWorldPoint(new Vector3(0.0f, 1f, camera.nearClipPlane));
    Vector3 worldPoint2 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.nearClipPlane));
    Vector3 inNormal = Vector3.Cross(camera.ViewportToWorldPoint(new Vector3(0.0f, 1f, camera.farClipPlane)) - worldPoint1, worldPoint2 - worldPoint1);
    inNormal.Normalize();
    return new Plane(inNormal, worldPoint1);
  }

  public static Plane CreateBottomPlane(Camera camera)
  {
    Vector3 worldPoint1 = camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, camera.nearClipPlane));
    Vector3 worldPoint2 = camera.ViewportToWorldPoint(new Vector3(1f, 0.0f, camera.nearClipPlane));
    Vector3 inNormal = Vector3.Cross(camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, camera.farClipPlane)) - worldPoint1, worldPoint2 - worldPoint1);
    inNormal.Normalize();
    return new Plane(inNormal, worldPoint1);
  }

  public static Plane CreateLeftPlane(Camera camera)
  {
    Vector3 worldPoint1 = camera.ViewportToWorldPoint(new Vector3(0.0f, 1f, camera.nearClipPlane));
    Vector3 worldPoint2 = camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, camera.nearClipPlane));
    Vector3 inNormal = Vector3.Cross(camera.ViewportToWorldPoint(new Vector3(0.0f, 1f, camera.farClipPlane)) - worldPoint1, worldPoint2 - worldPoint1);
    inNormal.Normalize();
    return new Plane(inNormal, worldPoint1);
  }

  public static Plane CreateRightPlane(Camera camera)
  {
    Vector3 worldPoint1 = camera.ViewportToWorldPoint(new Vector3(1f, 0.0f, camera.nearClipPlane));
    Vector3 worldPoint2 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.nearClipPlane));
    Vector3 inNormal = Vector3.Cross(camera.ViewportToWorldPoint(new Vector3(1f, 0.0f, camera.farClipPlane)) - worldPoint1, worldPoint2 - worldPoint1);
    inNormal.Normalize();
    return new Plane(inNormal, worldPoint1);
  }

  public static Bounds GetNearClipBounds(Camera camera)
  {
    Vector3 worldPoint1 = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.nearClipPlane));
    Vector3 worldPoint2 = camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, camera.nearClipPlane));
    Vector3 worldPoint3 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.nearClipPlane));
    Vector3 size = new Vector3(worldPoint3.x - worldPoint2.x, worldPoint3.y - worldPoint2.y, worldPoint3.z - worldPoint2.z);
    return new Bounds(worldPoint1, size);
  }

  public static Bounds GetFarClipBounds(Camera camera)
  {
    Vector3 worldPoint1 = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.farClipPlane));
    Vector3 worldPoint2 = camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, camera.farClipPlane));
    Vector3 worldPoint3 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.farClipPlane));
    Vector3 size = new Vector3(worldPoint3.x - worldPoint2.x, worldPoint3.y - worldPoint2.y, worldPoint3.z - worldPoint2.z);
    return new Bounds(worldPoint1, size);
  }

  public static Rect CreateGUIViewportRect(
    Camera camera,
    Component topLeft,
    Component bottomRight)
  {
    return CameraUtils.CreateGUIViewportRect(camera, topLeft.transform.position, bottomRight.transform.position);
  }

  public static Rect CreateGUIViewportRect(
    Camera camera,
    Vector3 worldTopLeft,
    Vector3 worldBottomRight)
  {
    Vector3 viewportPoint1 = camera.WorldToViewportPoint(worldTopLeft);
    Vector3 viewportPoint2 = camera.WorldToViewportPoint(worldBottomRight);
    return new Rect(viewportPoint1.x, 1f - viewportPoint1.y, viewportPoint2.x - viewportPoint1.x, viewportPoint1.y - viewportPoint2.y);
  }

  public static Rect CreateGUIScreenRect(
    Camera camera,
    Component topLeft,
    Component bottomRight)
  {
    return CameraUtils.CreateGUIScreenRect(camera, topLeft.transform.position, bottomRight.transform.position);
  }

  public static Rect CreateGUIScreenRect(
    Camera camera,
    Vector3 worldTopLeft,
    Vector3 worldBottomRight)
  {
    Vector3 screenPoint1 = camera.WorldToScreenPoint(worldTopLeft);
    Vector3 screenPoint2 = camera.WorldToScreenPoint(worldBottomRight);
    return new Rect(screenPoint1.x, screenPoint2.y, screenPoint2.x - screenPoint1.x, screenPoint1.y - screenPoint2.y);
  }

  public static bool Raycast(
    Camera camera,
    Vector3 screenPoint,
    LayerMask layerMask,
    out RaycastHit hitInfo,
    CameraOverridePass cameraOverride = null)
  {
    hitInfo = new RaycastHit();
    if (cameraOverride != null && cameraOverride.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.Scissor))
    {
      if (!cameraOverride.scissorOverride.Contains(screenPoint))
        return false;
    }
    else if (!camera.pixelRect.Contains(screenPoint))
      return false;
    Ray ray;
    if (cameraOverride != null && cameraOverride.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.ProjectionMatrix))
      ray = CameraUtils.GetMainCamera().ScreenPointToRay(screenPoint) with
      {
        origin = camera.transform.position
      };
    else
      ray = camera.ScreenPointToRay(screenPoint);
    return Physics.Raycast(ray, out hitInfo, camera.farClipPlane, (int) layerMask);
  }

  public static int RaycastAll(
    Camera camera,
    Vector3 screenPoint,
    LayerMask layerMask,
    ref RaycastHit[] hitInfos)
  {
    if (!camera.pixelRect.Contains(screenPoint))
      return 0;
    Ray ray = camera.ScreenPointToRay(screenPoint);
    int num;
    for (num = Physics.RaycastNonAlloc(ray, hitInfos, camera.farClipPlane, (int) layerMask); num == hitInfos.Length; num = Physics.RaycastNonAlloc(ray, hitInfos, camera.farClipPlane, (int) layerMask))
    {
      int length = hitInfos.Length * 2;
      hitInfos = new RaycastHit[length];
    }
    return num;
  }

  public static GameObject CreateInputBlocker(Camera camera, string name) => CameraUtils.CreateInputBlocker(camera, name, (Component) null, (Component) null, 0.0f);

  public static GameObject CreateInputBlocker(
    Camera camera,
    string name,
    Component parent)
  {
    return CameraUtils.CreateInputBlocker(camera, name, parent, parent, 0.0f);
  }

  public static GameObject CreateInputBlocker(
    Camera camera,
    string name,
    Component parent,
    float worldOffset)
  {
    return CameraUtils.CreateInputBlocker(camera, name, parent, parent, worldOffset);
  }

  public static GameObject CreateInputBlocker(
    Camera camera,
    string name,
    Component parent,
    Component relative,
    float worldOffset)
  {
    GameObject inputBlocker = new GameObject(name);
    inputBlocker.layer = camera.gameObject.layer;
    inputBlocker.transform.parent = (UnityEngine.Object) parent == (UnityEngine.Object) null ? (Transform) null : parent.transform;
    inputBlocker.transform.localScale = Vector3.one;
    inputBlocker.transform.rotation = Quaternion.Inverse(camera.transform.rotation);
    inputBlocker.transform.position = !((UnityEngine.Object) relative == (UnityEngine.Object) null) ? CameraUtils.GetPosInFrontOfCamera(camera, relative.transform.position, worldOffset) : CameraUtils.GetPosInFrontOfCamera(camera, camera.nearClipPlane + worldOffset);
    Bounds farClipBounds = CameraUtils.GetFarClipBounds(camera);
    Vector3 vector3 = !((UnityEngine.Object) parent == (UnityEngine.Object) null) ? TransformUtil.ComputeWorldScale(parent) : Vector3.one;
    inputBlocker.AddComponent<BoxCollider>().size = new Vector3()
    {
      x = farClipBounds.size.x / vector3.x,
      y = (double) farClipBounds.size.z <= 0.0 ? farClipBounds.size.y / vector3.y : farClipBounds.size.z / vector3.z
    };
    return inputBlocker;
  }

  public static float ScreenToWorldDist(Camera camera, float screenDist) => CameraUtils.ScreenToWorldDist(camera, screenDist, camera.nearClipPlane);

  public static float ScreenToWorldDist(Camera camera, float screenDist, float worldDist)
  {
    Vector3 worldPoint = camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, worldDist));
    return camera.ScreenToWorldPoint(new Vector3(screenDist, 0.0f, worldDist)).x - worldPoint.x;
  }

  public static float ScreenToWorldDist(Camera camera, float screenDist, Vector3 worldPoint)
  {
    float worldDist = Vector3.Distance(camera.transform.position, worldPoint);
    return CameraUtils.ScreenToWorldDist(camera, screenDist, worldDist);
  }

  public static Vector3 GetPosInFrontOfCamera(Camera camera, float worldDistance)
  {
    Vector3 position1 = camera.transform.position + new Vector3(0.0f, 0.0f, worldDistance);
    Vector3 position2 = new Vector3(0.0f, 0.0f, camera.transform.InverseTransformPoint(position1).magnitude);
    return camera.transform.TransformPoint(position2);
  }

  public static Vector3 GetPosInFrontOfCamera(
    Camera camera,
    Vector3 worldPoint,
    float worldOffset)
  {
    Vector3 position = camera.transform.position;
    Vector3 forward = camera.transform.forward;
    Vector3 vector3 = (new Plane(-forward, worldPoint).GetDistanceToPoint(position) + worldOffset) * forward;
    return position + vector3;
  }

  public static Camera GetMainCamera()
  {
    if (Application.isPlaying && (UnityEngine.Object) Box.Get() != (UnityEngine.Object) null)
      return Box.Get().GetCamera();
    return Application.isPlaying && (UnityEngine.Object) BoardCameras.Get() != (UnityEngine.Object) null ? BoardCameras.Get().GetComponentInChildren<Camera>() : Camera.main;
  }

  public static Ray ScreenPointToRayWithCameraPass(
    Camera cam,
    Vector2 mousePosition,
    CameraOverridePass cameraPass)
  {
    if (cameraPass == null || !cameraPass.toOverride.HasFlag((Enum) CameraOverridePass.OverrideFlags.ProjectionMatrix))
      return cam.ScreenPointToRay((Vector3) mousePosition);
    return CameraUtils.GetMainCamera().ScreenPointToRay((Vector3) mousePosition) with
    {
      origin = cam.transform.position
    };
  }
}
