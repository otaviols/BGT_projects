using UnityEngine;

public class Scrollbar : MonoBehaviour
{
  public ScrollBarThumb m_thumb;
  public GameObject m_track;
  public Vector3 m_sliderStartLocalPos;
  public Vector3 m_sliderEndLocalPos;
  public GameObject m_scrollArea;
  public BoxCollider m_scrollWindow;
  protected bool m_isActive = true;
  protected bool m_isDragging;
  protected float m_scrollValue;
  protected Vector3 m_scrollAreaStartPos;
  protected Vector3 m_scrollAreaEndPos;
  protected float m_stepSize;
  protected Vector3 m_thumbPosition;
  protected Bounds m_childrenBounds;
  protected float m_scrollWindowHeight;

  public float ScrollValue => this.m_scrollValue;

  protected virtual void Awake()
  {
    this.m_scrollWindowHeight = this.m_scrollWindow.size.z;
    this.m_scrollWindow.enabled = false;
  }

  public bool IsActive() => this.m_isActive;

  private void Update()
  {
    if (!this.m_isActive)
      return;
    if (this.InputIsOver())
    {
      if ((double) Input.GetAxis("Mouse ScrollWheel") < 0.0)
        this.ScrollDown();
      if ((double) Input.GetAxis("Mouse ScrollWheel") > 0.0)
        this.ScrollUp();
    }
    if (!this.m_thumb.IsDragging())
      return;
    this.Drag();
  }

  public void Drag()
  {
    Vector3 min = this.m_track.GetComponent<BoxCollider>().bounds.min;
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.m_track.layer);
    Plane plane = new Plane(-firstByLayer.transform.forward, min);
    Ray ray = firstByLayer.ScreenPointToRay(InputCollection.GetMousePosition());
    float enter;
    if (!plane.Raycast(ray, out enter))
      return;
    Vector3 vector3 = this.transform.InverseTransformPoint(ray.GetPoint(enter));
    TransformUtil.SetLocalPosZ(this.m_thumb.gameObject, Mathf.Clamp(vector3.z, this.m_sliderStartLocalPos.z, this.m_sliderEndLocalPos.z));
    this.m_scrollValue = Mathf.Clamp01((float) (((double) vector3.z - (double) this.m_sliderStartLocalPos.z) / ((double) this.m_sliderEndLocalPos.z - (double) this.m_sliderStartLocalPos.z)));
    this.UpdateScrollAreaPosition(false);
  }

  public virtual void Show()
  {
    this.m_isActive = true;
    this.ShowThumb(true);
    this.gameObject.SetActive(true);
  }

  public virtual void Hide()
  {
    this.m_isActive = false;
    this.ShowThumb(false);
    this.gameObject.SetActive(false);
  }

  public void Init()
  {
    this.m_scrollValue = 0.0f;
    this.m_stepSize = 1f;
    this.m_thumb.transform.localPosition = this.m_sliderStartLocalPos;
    this.m_scrollAreaStartPos = this.m_scrollArea.transform.position;
    this.UpdateScrollAreaBounds();
  }

  public void UpdateScrollAreaBounds()
  {
    this.GetBoundsOfChildren(this.m_scrollArea);
    float num = this.m_childrenBounds.size.z - this.m_scrollWindowHeight;
    this.m_scrollAreaEndPos = this.m_scrollAreaStartPos;
    if ((double) num <= 0.0)
    {
      this.m_scrollValue = 0.0f;
      this.Hide();
    }
    else
    {
      this.m_stepSize = 1f / (float) (int) Mathf.Ceil(num / 5f);
      this.m_scrollAreaEndPos.z += num;
      this.Show();
    }
    this.UpdateThumbPosition();
    this.UpdateScrollAreaPosition(false);
  }

  public virtual bool InputIsOver() => UniversalInputManager.Get().InputIsOver(this.gameObject);

  protected virtual void GetBoundsOfChildren(GameObject go) => this.m_childrenBounds = TransformUtil.GetBoundsOfChildren(go);

  public void OverrideScrollWindowHeight(float scrollWindowHeight) => this.m_scrollWindowHeight = scrollWindowHeight;

  protected void ShowThumb(bool show)
  {
    if (!((Object) this.m_thumb != (Object) null))
      return;
    this.m_thumb.gameObject.SetActive(show);
  }

  private void UpdateThumbPosition()
  {
    this.m_thumbPosition = Vector3.Lerp(this.m_sliderStartLocalPos, this.m_sliderEndLocalPos, Mathf.Clamp01(this.m_scrollValue));
    this.m_thumb.transform.localPosition = this.m_thumbPosition;
    this.m_thumb.transform.localScale = Vector3.one;
    if ((double) this.m_scrollValue >= 0.0 && (double) this.m_scrollValue <= 1.0)
      return;
    float z = (float) (1.0 / ((double) this.m_scrollValue < 0.0 ? -(double) this.m_scrollValue + 1.0 : (double) this.m_scrollValue));
    Renderer component = this.m_thumb.GetComponent<Renderer>();
    TransformUtil.SetLocalPosZ((Component) this.m_thumb, this.m_thumbPosition.z + (float) (((double) this.m_thumbPosition.z - (double) this.m_thumb.transform.parent.InverseTransformPoint((double) this.m_scrollValue < 0.0 ? component.bounds.max : component.bounds.min).z) * ((double) z - 1.0)));
    TransformUtil.SetLocalScaleZ((Component) this.m_thumb, z);
  }

  private void UpdateScrollAreaPosition(bool tween)
  {
    if ((Object) this.m_scrollArea == (Object) null)
      return;
    Vector3 vector3 = this.m_scrollAreaStartPos + this.m_scrollValue * (this.m_scrollAreaEndPos - this.m_scrollAreaStartPos);
    if (tween)
      iTween.MoveTo(this.m_scrollArea, iTween.Hash((object) "position", (object) vector3, (object) "time", (object) 0.2f, (object) "isLocal", (object) false));
    else
      this.m_scrollArea.transform.position = vector3;
  }

  public void ScrollTo(float value, bool clamp = true, bool lerp = true)
  {
    this.m_scrollValue = clamp ? Mathf.Clamp01(value) : value;
    this.UpdateThumbPosition();
    this.UpdateScrollAreaPosition(lerp);
  }

  private void ScrollUp() => this.Scroll(-this.m_stepSize);

  public void Scroll(float amount, bool lerp = true)
  {
    this.m_scrollValue = Mathf.Clamp01(this.m_scrollValue + amount);
    this.UpdateThumbPosition();
    this.UpdateScrollAreaPosition(lerp);
  }

  private void ScrollDown() => this.Scroll(this.m_stepSize);
}
