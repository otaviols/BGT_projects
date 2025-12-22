using UnityEngine;

public class ScrollbarControl : MonoBehaviour
{
  public GameObject m_Thumb;
  public PegUIElement m_PressElement;
  public Collider m_DragCollider;
  public Transform m_LeftBone;
  public Transform m_RightBone;
  private bool m_dragging;
  private float m_thumbUnitPos;
  private float m_prevThumbUnitPos;
  private ScrollbarControl.UpdateHandler m_updateHandler;
  private ScrollbarControl.FinishHandler m_finishHandler;

  private void Awake()
  {
    this.m_PressElement.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnPressElementPress));
    this.m_PressElement.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.OnPressElementReleaseAll));
    this.m_DragCollider.enabled = false;
  }

  private void Update() => this.UpdateDrag();

  private void OnDestroy()
  {
    if (UniversalInputManager.Get() == null)
      return;
    UniversalInputManager.Get().UnregisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
  }

  public float GetValue() => this.m_thumbUnitPos;

  public void SetValue(float val)
  {
    this.m_thumbUnitPos = Mathf.Clamp01(val);
    this.m_prevThumbUnitPos = this.m_thumbUnitPos;
    this.UpdateThumb();
  }

  public ScrollbarControl.UpdateHandler GetUpdateHandler() => this.m_updateHandler;

  public void SetUpdateHandler(ScrollbarControl.UpdateHandler handler) => this.m_updateHandler = handler;

  public ScrollbarControl.FinishHandler GetFinishHandler() => this.m_finishHandler;

  public void SetFinishHandler(ScrollbarControl.FinishHandler handler) => this.m_finishHandler = handler;

  private void OnPressElementPress(UIEvent e) => this.HandlePress();

  private void OnPressElementReleaseAll(UIEvent e)
  {
    this.HandleRelease();
    this.FireFinishEvent();
  }

  private void OnMouseOnOrOffScreen(bool onScreen)
  {
    if (onScreen)
      return;
    this.HandleOutOfBounds();
  }

  private void UpdateDrag()
  {
    if (!this.m_dragging)
      return;
    RaycastHit hitInfo;
    if (UniversalInputManager.Get().GetInputHitInfo((LayerMask) (1 << this.m_DragCollider.gameObject.layer), out hitInfo) && (Object) hitInfo.collider == (Object) this.m_DragCollider)
    {
      float x = this.m_LeftBone.position.x;
      float num = this.m_RightBone.position.x - x;
      this.m_thumbUnitPos = Mathf.Clamp01((hitInfo.point.x - x) / num);
      this.UpdateThumb();
      this.HandleThumbUpdate();
    }
    else
    {
      this.m_thumbUnitPos = this.m_prevThumbUnitPos;
      this.HandleOutOfBounds();
    }
  }

  private void UpdateThumb() => this.m_Thumb.transform.localPosition = Vector3.Lerp(this.m_LeftBone.localPosition, this.m_RightBone.localPosition, this.m_thumbUnitPos);

  private void HandlePress()
  {
    this.m_dragging = true;
    UniversalInputManager.Get().RegisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
    this.m_PressElement.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.OnPressElementReleaseAll));
    this.m_PressElement.GetComponent<Collider>().enabled = false;
    this.m_DragCollider.enabled = true;
  }

  private void HandleRelease()
  {
    this.m_DragCollider.enabled = false;
    this.m_PressElement.GetComponent<Collider>().enabled = true;
    this.m_PressElement.RemoveEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.OnPressElementReleaseAll));
    UniversalInputManager.Get().UnregisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
    this.m_dragging = false;
  }

  private void HandleThumbUpdate()
  {
    float prevThumbUnitPos = this.m_prevThumbUnitPos;
    this.m_prevThumbUnitPos = this.m_thumbUnitPos;
    if (Mathf.Approximately(this.m_thumbUnitPos, prevThumbUnitPos))
      return;
    this.FireUpdateEvent();
  }

  private void HandleOutOfBounds()
  {
    this.UpdateThumb();
    this.HandleThumbUpdate();
    this.HandleRelease();
    this.FireFinishEvent();
  }

  private void FireUpdateEvent()
  {
    if (this.m_updateHandler == null)
      return;
    this.m_updateHandler(this.GetValue());
  }

  private void FireFinishEvent()
  {
    if (this.m_finishHandler == null)
      return;
    this.m_finishHandler();
  }

  public delegate void UpdateHandler(float val);

  public delegate void FinishHandler();
}
