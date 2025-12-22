using Hearthstone;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OverlayUI : MonoBehaviour
{
  public CanvasAnchors m_heightScale;
  public CanvasAnchors m_widthScale;
  public RectTransform m_inputFieldOverlayRect;
  public InputFieldUI m_inputFieldUI;
  public Transform m_BoneParent;
  public GameObject m_clickBlocker;
  public GameObject m_QuestProgressToastBone;
  public Camera m_UICamera;
  private static OverlayUI s_instance;
  private HashSet<GameObject> m_destroyOnSceneLoad = new HashSet<GameObject>();
  private bool m_clickBlockerRequested;

  private void Awake()
  {
    OverlayUI.s_instance = this;
    UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneChange));
    HearthstoneApplication.Get().WillReset += new Action(this.WillReset);
    UniversalInputManager.Get().SetTextInputField(this.m_inputFieldUI);
    this.m_UICamera = CameraUtils.FindFirstByLayer(GameLayer.BattleNet);
  }

  private void Update()
  {
    if ((UnityEngine.Object) this.m_clickBlocker != (UnityEngine.Object) null)
      this.m_clickBlocker.SetActive(this.m_clickBlockerRequested);
    this.m_clickBlockerRequested = false;
  }

  private void OnDestroy()
  {
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().WillReset -= new Action(this.WillReset);
    OverlayUI.s_instance = (OverlayUI) null;
  }

  public static OverlayUI Get() => OverlayUI.s_instance;

  public void AddGameObject(
    GameObject go,
    CanvasAnchor anchor = CanvasAnchor.CENTER,
    bool destroyOnSceneLoad = false,
    CanvasScaleMode scaleMode = CanvasScaleMode.HEIGHT)
  {
    CanvasAnchors canvasAnchors = scaleMode == CanvasScaleMode.HEIGHT ? this.m_heightScale : this.m_widthScale;
    TransformUtil.AttachAndPreserveLocalTransform(go.transform, canvasAnchors.GetAnchor(anchor));
    if (!destroyOnSceneLoad)
      return;
    this.DestroyOnSceneLoad(go);
  }

  public bool HasObject(GameObject gameObject) => !((UnityEngine.Object) gameObject == (UnityEngine.Object) null) && gameObject.transform.IsChildOf(this.transform);

  public Vector3 GetRelativePosition(
    Vector3 worldPosition,
    Camera camera = null,
    Transform bone = null,
    float depth = 0.0f)
  {
    if ((UnityEngine.Object) camera == (UnityEngine.Object) null)
      camera = SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY ? Box.Get().GetBoxCamera().GetComponent<Camera>() : BoardCameras.Get().GetComponentInChildren<Camera>();
    if ((UnityEngine.Object) bone == (UnityEngine.Object) null)
      bone = this.m_heightScale.m_Center;
    Vector3 worldPoint = this.m_UICamera.ScreenToWorldPoint(camera.WorldToScreenPoint(worldPosition)) with
    {
      y = depth
    };
    return bone.InverseTransformPoint(worldPoint);
  }

  public Rect GetInputFieldRect(Rect normalizedInputRect)
  {
    Vector2 min = normalizedInputRect.min;
    Vector2 max = normalizedInputRect.max;
    Vector2 point1 = Rect.NormalizedToPoint(this.m_inputFieldOverlayRect.rect, min);
    Vector2 point2 = Rect.NormalizedToPoint(this.m_inputFieldOverlayRect.rect, max);
    return Rect.MinMaxRect(point1.x, point1.y, point2.x, point2.y);
  }

  public void DestroyOnSceneLoad(GameObject go)
  {
    if (this.m_destroyOnSceneLoad.Contains(go))
      return;
    this.m_destroyOnSceneLoad.Add(go);
  }

  public void DontDestroyOnSceneLoad(GameObject go)
  {
    if (!this.m_destroyOnSceneLoad.Contains(go))
      return;
    this.m_destroyOnSceneLoad.Remove(go);
  }

  public Transform FindBone(string name)
  {
    if ((UnityEngine.Object) this.m_BoneParent != (UnityEngine.Object) null)
    {
      Transform bone = this.m_BoneParent.Find(name);
      if ((UnityEngine.Object) bone != (UnityEngine.Object) null)
        return bone;
    }
    return this.transform;
  }

  public void RequestActivateClickBlocker() => this.m_clickBlockerRequested = true;

  private void OnSceneChange(SceneMgr.Mode mode, PegasusScene scene, object userData) => this.m_destroyOnSceneLoad.RemoveWhere((Predicate<GameObject>) (go =>
  {
    if (!((UnityEngine.Object) go != (UnityEngine.Object) null))
      return false;
    UnityEngine.Object.Destroy((UnityEngine.Object) go);
    return true;
  }));

  private void WillReset()
  {
    this.m_widthScale.WillReset();
    this.m_heightScale.WillReset();
  }
}
