using UnityEngine;

[ExecuteAlways]
public class NewThreeSliceElement : MonoBehaviour
{
  public GameObject m_leftOrTop;
  public GameObject m_middle;
  public GameObject m_rightOrBottom;
  public NewThreeSliceElement.PinnedPoint m_pinnedPoint;
  public NewThreeSliceElement.PlaneAxis m_planeAxis = NewThreeSliceElement.PlaneAxis.XZ;
  public Vector3 m_pinnedPointOffset;
  public NewThreeSliceElement.Direction m_direction;
  public Vector3 m_middleScale = Vector3.one;
  public Vector3 m_leftOffset;
  public Vector3 m_middleOffset;
  public Vector3 m_rightOffset;
  private Vector3 m_leftAnchor;
  private Vector3 m_rightAnchor;
  private Vector3 m_topAnchor;
  private Vector3 m_bottomAnchor;
  private Transform m_identity;

  private void Awake() => this.SetSize(this.m_middleScale);

  private void OnDestroy()
  {
    if (!((Object) this.m_identity != (Object) null) || !((Object) this.m_identity.gameObject != (Object) null))
      return;
    Object.DestroyImmediate((Object) this.m_identity.gameObject);
  }

  public virtual void SetSize(Vector3 size)
  {
    this.m_middle.transform.localScale = size;
    if ((Object) this.m_identity == (Object) null)
    {
      this.m_identity = new GameObject().transform;
      this.m_identity.gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
    }
    this.m_identity.position = Vector3.zero;
    if (this.m_planeAxis == NewThreeSliceElement.PlaneAxis.XZ)
    {
      this.m_leftAnchor = new Vector3(0.0f, 0.0f, 0.5f);
      this.m_rightAnchor = new Vector3(1f, 0.0f, 0.5f);
      this.m_topAnchor = new Vector3(0.5f, 0.0f, 1f);
      this.m_bottomAnchor = new Vector3(0.5f, 0.0f, 0.0f);
    }
    else
    {
      this.m_leftAnchor = new Vector3(0.0f, 0.5f, 0.0f);
      this.m_rightAnchor = new Vector3(1f, 0.5f, 0.0f);
      this.m_topAnchor = new Vector3(0.5f, 0.0f, 0.0f);
      this.m_bottomAnchor = new Vector3(0.5f, 1f, 0.0f);
    }
    switch (this.m_direction)
    {
      case NewThreeSliceElement.Direction.X:
        this.DisplayOnXAxis();
        break;
      case NewThreeSliceElement.Direction.Z:
        this.DisplayOnZAxis();
        break;
    }
  }

  private void DisplayOnXAxis()
  {
    switch (this.m_pinnedPoint)
    {
      case NewThreeSliceElement.PinnedPoint.LEFT:
        this.m_leftOrTop.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_middle, this.m_leftAnchor, this.m_leftOrTop, this.m_rightAnchor, this.m_identity.transform.TransformPoint(this.m_middleOffset));
        TransformUtil.SetPoint(this.m_rightOrBottom, this.m_leftAnchor, this.m_middle, this.m_rightAnchor, this.m_identity.transform.TransformPoint(this.m_rightOffset));
        break;
      case NewThreeSliceElement.PinnedPoint.MIDDLE:
        this.m_middle.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_leftOrTop, this.m_rightAnchor, this.m_middle, this.m_leftAnchor, this.m_identity.transform.TransformPoint(this.m_leftOffset));
        TransformUtil.SetPoint(this.m_rightOrBottom, this.m_leftAnchor, this.m_middle, this.m_rightAnchor, this.m_identity.transform.TransformPoint(this.m_rightOffset));
        break;
      case NewThreeSliceElement.PinnedPoint.RIGHT:
        this.m_rightOrBottom.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_middle, this.m_rightAnchor, this.m_rightOrBottom, this.m_leftAnchor, this.m_identity.transform.TransformPoint(this.m_middleOffset));
        TransformUtil.SetPoint(this.m_leftOrTop, this.m_rightAnchor, this.m_middle, this.m_leftAnchor, this.m_identity.transform.TransformPoint(this.m_leftOffset));
        break;
    }
  }

  private void DisplayOnZAxis()
  {
    switch (this.m_pinnedPoint)
    {
      case NewThreeSliceElement.PinnedPoint.MIDDLE:
        this.m_middle.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_leftOrTop, this.m_bottomAnchor, this.m_middle, this.m_topAnchor, this.m_identity.transform.TransformPoint(this.m_leftOffset));
        TransformUtil.SetPoint(this.m_rightOrBottom, this.m_topAnchor, this.m_middle, this.m_bottomAnchor, this.m_identity.transform.TransformPoint(this.m_rightOffset));
        break;
      case NewThreeSliceElement.PinnedPoint.TOP:
        this.m_leftOrTop.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_middle, this.m_topAnchor, this.m_leftOrTop, this.m_bottomAnchor, this.m_identity.transform.TransformPoint(this.m_middleOffset));
        TransformUtil.SetPoint(this.m_rightOrBottom, this.m_topAnchor, this.m_middle, this.m_bottomAnchor, this.m_identity.transform.TransformPoint(this.m_rightOffset));
        break;
      case NewThreeSliceElement.PinnedPoint.BOTTOM:
        this.m_rightOrBottom.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_middle, this.m_bottomAnchor, this.m_rightOrBottom, this.m_topAnchor, this.m_identity.transform.TransformPoint(this.m_middleOffset));
        TransformUtil.SetPoint(this.m_leftOrTop, this.m_bottomAnchor, this.m_middle, this.m_topAnchor, this.m_identity.transform.TransformPoint(this.m_leftOffset));
        break;
    }
  }

  public enum PinnedPoint
  {
    LEFT,
    MIDDLE,
    RIGHT,
    TOP,
    BOTTOM,
  }

  public enum Direction
  {
    X,
    Y,
    Z,
  }

  public enum PlaneAxis
  {
    XY,
    XZ,
  }
}
