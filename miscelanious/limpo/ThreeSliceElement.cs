using UnityEngine;

[ExecuteAlways]
public class ThreeSliceElement : MonoBehaviour
{
  public GameObject m_left;
  public GameObject m_middle;
  public GameObject m_right;
  public ThreeSliceElement.PinnedPoint m_pinnedPoint;
  public Vector3 m_pinnedPointOffset;
  public ThreeSliceElement.Direction m_direction;
  public float m_width;
  public float m_middleScale = 1f;
  public Vector3_MobileOverride m_leftOffset;
  public Vector3_MobileOverride m_middleOffset;
  public Vector3_MobileOverride m_rightOffset;
  private Bounds m_initialMiddleBounds;
  private Vector3 m_initialScale = Vector3.zero;
  private Renderer m_leftRenderer;
  private Renderer m_middleRenderer;
  private Renderer m_rightRenderer;

  private void Awake()
  {
    bool flag = false;
    if ((Object) null == (Object) this.m_left)
    {
      flag = true;
      Debug.LogError((object) "m_left not set");
    }
    if ((Object) null == (Object) this.m_middle)
    {
      flag = true;
      Debug.LogError((object) "m_middle not set");
    }
    if ((Object) null == (Object) this.m_right)
    {
      flag = true;
      Debug.LogError((object) "m_right not set");
    }
    if (flag)
      return;
    this.m_leftRenderer = this.m_left.GetComponent<Renderer>();
    this.m_middleRenderer = this.m_middle.GetComponent<Renderer>();
    this.m_rightRenderer = this.m_right.GetComponent<Renderer>();
    if (!(bool) (Object) this.m_middle)
      return;
    this.SetInitialValues();
  }

  public void UpdateDisplay()
  {
    if (!this.enabled)
      return;
    if (this.m_initialMiddleBounds.size == Vector3.zero)
      this.m_initialMiddleBounds = this.m_middleRenderer.bounds;
    float num = this.m_width - (this.m_leftRenderer.bounds.size.x + this.m_rightRenderer.bounds.size.x);
    switch (this.m_direction)
    {
      case ThreeSliceElement.Direction.X:
        TransformUtil.SetWorldScale((Component) this.m_middle.transform, TransformUtil.ComputeWorldScale((Component) this.m_middle.transform) with
        {
          x = this.m_initialScale.x * num / this.m_initialMiddleBounds.size.x
        });
        break;
    }
    switch (this.m_pinnedPoint)
    {
      case ThreeSliceElement.PinnedPoint.LEFT:
        this.m_left.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT, (Vector3) (MobileOverrideValue<Vector3>) this.m_middleOffset);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT, (Vector3) (MobileOverrideValue<Vector3>) this.m_rightOffset);
        break;
      case ThreeSliceElement.PinnedPoint.MIDDLE:
        this.m_middle.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_left, Anchor.RIGHT, this.m_middle, Anchor.LEFT, (Vector3) (MobileOverrideValue<Vector3>) this.m_leftOffset);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT, (Vector3) (MobileOverrideValue<Vector3>) this.m_rightOffset);
        break;
      case ThreeSliceElement.PinnedPoint.RIGHT:
        this.m_right.transform.localPosition = this.m_pinnedPointOffset;
        TransformUtil.SetPoint(this.m_middle, Anchor.RIGHT, this.m_right, Anchor.LEFT, (Vector3) (MobileOverrideValue<Vector3>) this.m_middleOffset);
        TransformUtil.SetPoint(this.m_left, Anchor.RIGHT, this.m_middle, Anchor.LEFT, (Vector3) (MobileOverrideValue<Vector3>) this.m_leftOffset);
        break;
    }
  }

  public void SetWidth(float globalWidth)
  {
    this.m_width = globalWidth;
    this.UpdateDisplay();
  }

  public void SetMiddleWidth(float globalWidth)
  {
    this.m_width = globalWidth + this.m_leftRenderer.bounds.size.x + this.m_rightRenderer.bounds.size.x;
    this.UpdateDisplay();
  }

  public Vector3 GetLeftSize() => this.m_leftRenderer.bounds.size;

  public Vector3 GetMiddleSize() => this.m_middleRenderer.bounds.size;

  public Vector3 GetRightSize() => this.m_rightRenderer.bounds.size;

  public Vector3 GetSize() => this.GetSize(true);

  public Vector3 GetSize(bool zIsHeight)
  {
    Bounds bounds = this.m_leftRenderer.bounds;
    Vector3 size1 = bounds.size;
    bounds = this.m_middleRenderer.bounds;
    Vector3 size2 = bounds.size;
    bounds = this.m_rightRenderer.bounds;
    Vector3 size3 = bounds.size;
    float x = size1.x + size3.x + size2.x;
    float num1 = Mathf.Max(Mathf.Max(size1.z, size2.z), size3.z);
    float num2 = Mathf.Max(Mathf.Max(size1.y, size2.y), size3.y);
    return zIsHeight ? new Vector3(x, num1, num2) : new Vector3(x, num2, num1);
  }

  public void SetInitialValues()
  {
    this.m_initialMiddleBounds = this.m_middleRenderer.bounds;
    this.m_initialScale = this.m_middle.transform.lossyScale;
    Bounds bounds = this.m_middleRenderer.bounds;
    double x1 = (double) bounds.size.x;
    bounds = this.m_leftRenderer.bounds;
    double x2 = (double) bounds.size.x;
    double num = x1 + x2;
    bounds = this.m_rightRenderer.bounds;
    double x3 = (double) bounds.size.x;
    this.m_width = (float) (num + x3);
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
}
