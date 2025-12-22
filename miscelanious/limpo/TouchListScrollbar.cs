using System;
using System.Collections;
using UnityEngine;

public class TouchListScrollbar : PegUIElement
{
  public TouchList list;
  public PegUIElement thumb;
  public Transform thumbMin;
  public Transform thumbMax;
  public GameObject cover;
  public PegUIElement track;
  public TouchListScrollbar.ScrollDirection scrollPlane = TouchListScrollbar.ScrollDirection.Y;
  private bool isActive;

  protected override void Awake()
  {
    if (this.list.orientation == TouchList.Orientation.Horizontal)
    {
      Debug.LogError((object) "Horizontal TouchListScrollbar not implemented");
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }
    else
    {
      base.Awake();
      this.ShowThumb(this.isActive);
      this.list.ClipSizeChanged += new Action(this.UpdateLayout);
      this.list.ScrollingEnabledChanged += new TouchList.ScrollingEnabledChangedEvent(this.UpdateActive);
      this.list.Scrolled += new Action(this.UpdateThumb);
      this.thumb.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ThumbPressed));
      this.track.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.TrackPressed));
      this.UpdateLayout();
    }
  }

  private void UpdateActive(bool canScroll)
  {
    if (this.isActive == canScroll)
      return;
    this.isActive = canScroll;
    this.thumb.GetComponent<Collider>().enabled = this.isActive;
    if (this.isActive)
      this.UpdateThumb();
    this.ShowThumb(this.isActive);
  }

  private void UpdateLayout()
  {
    TransformUtil.SetPosX((Component) this.thumb, this.thumbMin.position.x);
    this.UpdateThumb();
  }

  private void ShowThumb(bool show)
  {
    Transform transform = this.thumb.transform.Find("Mesh");
    if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      transform.gameObject.SetActive(show);
    if (!((UnityEngine.Object) this.cover != (UnityEngine.Object) null))
      return;
    this.cover.SetActive(!show);
  }

  private void UpdateThumb()
  {
    if (!this.isActive)
      return;
    Collider component1 = this.GetComponent<Collider>();
    if (this.list.layoutPlane == TouchList.LayoutPlane.XZ)
      TransformUtil.SetPosY((Component) this.thumb, component1.bounds.min.y);
    else
      TransformUtil.SetPosZ((Component) this.thumb, component1.bounds.min.z);
    float scrollValue = this.list.ScrollValue;
    float num1 = this.thumbMin.position[(int) this.scrollPlane] + (this.thumbMax.position[(int) this.scrollPlane] - this.thumbMin.position[(int) this.scrollPlane]) * Mathf.Clamp01(scrollValue);
    Vector3 position = this.thumb.transform.position;
    position[(int) this.scrollPlane] = num1;
    this.thumb.transform.position = position;
    this.thumb.transform.localScale = Vector3.one;
    if ((double) scrollValue >= 0.0 && (double) scrollValue <= 1.0)
      return;
    float num2 = (float) (1.0 / ((double) scrollValue < 0.0 ? -(double) scrollValue + 1.0 : (double) scrollValue));
    Collider component2 = this.thumb.GetComponent<Collider>();
    float num3 = (float) (((double) this.thumb.transform.position[(int) this.scrollPlane] - (double) ((double) scrollValue < 0.0 ? component2.bounds.max : component2.bounds.min)[(int) this.scrollPlane]) * ((double) num2 - 1.0));
    position = this.thumb.transform.position;
    position[(int) this.scrollPlane] += num3;
    this.thumb.transform.position = position;
  }

  private void ThumbPressed(UIEvent e) => this.StartCoroutine(this.UpdateThumbDrag());

  private void TrackPressed(UIEvent e)
  {
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    double num1 = (double) this.GetTouchPoint(new Plane(-firstByLayer.transform.forward, this.track.transform.position), firstByLayer)[(int) this.scrollPlane];
    Vector3 position1 = this.thumbMax.position;
    double min = (double) position1[(int) this.scrollPlane];
    position1 = this.thumbMin.position;
    double max = (double) position1[(int) this.scrollPlane];
    float num2 = Mathf.Clamp((float) num1, (float) min, (float) max);
    TouchList list = this.list;
    double num3 = (double) num2 - (double) this.thumbMin.position[(int) this.scrollPlane];
    Vector3 position2 = this.thumbMax.position;
    double num4 = (double) position2[(int) this.scrollPlane];
    position2 = this.thumbMin.position;
    double num5 = (double) position2[(int) this.scrollPlane];
    double num6 = num4 - num5;
    double num7 = num3 / num6;
    list.ScrollValue = (float) num7;
  }

  private IEnumerator UpdateThumbDrag()
  {
    TouchListScrollbar touchListScrollbar = this;
    Camera camera = CameraUtils.FindFirstByLayer(touchListScrollbar.gameObject.layer);
    Plane dragPlane = new Plane(-camera.transform.forward, touchListScrollbar.thumb.transform.position);
    Vector3 vector3 = touchListScrollbar.thumb.transform.position - touchListScrollbar.GetTouchPoint(dragPlane, camera);
    float dragOffset = vector3[(int) touchListScrollbar.scrollPlane];
    while (!InputCollection.GetMouseButtonUp(0))
    {
      vector3 = touchListScrollbar.GetTouchPoint(dragPlane, camera);
      double num1 = (double) (vector3[(int) touchListScrollbar.scrollPlane] + dragOffset);
      vector3 = touchListScrollbar.thumbMax.position;
      double min = (double) vector3[(int) touchListScrollbar.scrollPlane];
      vector3 = touchListScrollbar.thumbMin.position;
      double max = (double) vector3[(int) touchListScrollbar.scrollPlane];
      float num2 = Mathf.Clamp((float) num1, (float) min, (float) max);
      TouchList list = touchListScrollbar.list;
      double num3 = (double) num2;
      vector3 = touchListScrollbar.thumbMin.position;
      double num4 = (double) vector3[(int) touchListScrollbar.scrollPlane];
      double num5 = num3 - num4;
      vector3 = touchListScrollbar.thumbMax.position;
      double num6 = (double) vector3[(int) touchListScrollbar.scrollPlane];
      vector3 = touchListScrollbar.thumbMin.position;
      double num7 = (double) vector3[(int) touchListScrollbar.scrollPlane];
      double num8 = num6 - num7;
      double num9 = num5 / num8;
      list.ScrollValue = (float) num9;
      yield return (object) null;
    }
  }

  private Vector3 GetTouchPoint(Plane dragPlane, Camera camera)
  {
    Ray ray = camera.ScreenPointToRay(InputCollection.GetMousePosition());
    float enter;
    dragPlane.Raycast(ray, out enter);
    return ray.GetPoint(enter);
  }

  public enum ScrollDirection
  {
    X,
    Y,
    Z,
  }
}
