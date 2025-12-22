using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
[ExecuteAlways]
public class NineSliceElement : MonoBehaviour
{
  [CustomEditField(Sections = "Top Row")]
  public MultiSliceElement.Slice m_topRow;
  [CustomEditField(Sections = "Middle Row")]
  public MultiSliceElement.Slice m_midRow;
  [CustomEditField(Sections = "Bottom Row")]
  public MultiSliceElement.Slice m_btmRow;
  [CustomEditField(Sections = "Top Row")]
  public MultiSliceElement.Slice m_topLeft;
  [CustomEditField(Sections = "Top Row")]
  public MultiSliceElement.Slice m_top;
  [CustomEditField(Sections = "Top Row")]
  public MultiSliceElement.Slice m_topRight;
  [CustomEditField(Sections = "Middle Row")]
  public MultiSliceElement.Slice m_left;
  [CustomEditField(Sections = "Middle Row")]
  public MultiSliceElement.Slice m_middle;
  [CustomEditField(Sections = "Middle Row")]
  public MultiSliceElement.Slice m_right;
  [CustomEditField(Sections = "Bottom Row")]
  public MultiSliceElement.Slice m_bottomLeft;
  [CustomEditField(Sections = "Bottom Row")]
  public MultiSliceElement.Slice m_bottom;
  [CustomEditField(Sections = "Bottom Row")]
  public MultiSliceElement.Slice m_bottomRight;
  public List<GameObject> m_ignore = new List<GameObject>();
  public MultiSliceElement.Direction m_WidthDirection;
  public MultiSliceElement.Direction m_HeightDirection = MultiSliceElement.Direction.Z;
  public Vector3 m_localPinnedPointOffset = Vector3.zero;
  public MultiSliceElement.XAxisAlign m_XAlign;
  public MultiSliceElement.YAxisAlign m_YAlign = MultiSliceElement.YAxisAlign.BOTTOM;
  public MultiSliceElement.ZAxisAlign m_ZAlign = MultiSliceElement.ZAxisAlign.BACK;
  public Vector3 m_localSliceSpacing = Vector3.zero;
  public bool m_reverse;
  public bool m_useUberText;

  public void SetEntireWidth(float width)
  {
    int widthDirection = (int) this.m_WidthDirection;
    OrientedBounds sliceBounds1 = this.GetSliceBounds((GameObject) this.m_topLeft);
    OrientedBounds sliceBounds2 = this.GetSliceBounds((GameObject) this.m_left);
    OrientedBounds sliceBounds3 = this.GetSliceBounds((GameObject) this.m_bottomLeft);
    OrientedBounds sliceBounds4 = this.GetSliceBounds((GameObject) this.m_topRight);
    OrientedBounds sliceBounds5 = this.GetSliceBounds((GameObject) this.m_right);
    OrientedBounds sliceBounds6 = this.GetSliceBounds((GameObject) this.m_bottomRight);
    float num1 = Mathf.Max(sliceBounds1.Extents[widthDirection].magnitude, sliceBounds2.Extents[widthDirection].magnitude, sliceBounds3.Extents[widthDirection].magnitude) * 2f;
    float num2 = Mathf.Max(sliceBounds4.Extents[widthDirection].magnitude, sliceBounds5.Extents[widthDirection].magnitude, sliceBounds6.Extents[widthDirection].magnitude) * 2f;
    this.SetWidth(width - num1 - num2);
  }

  public void SetEntireHeight(float height)
  {
    int heightDirection = (int) this.m_HeightDirection;
    OrientedBounds sliceBounds1 = this.GetSliceBounds((GameObject) this.m_topLeft);
    OrientedBounds sliceBounds2 = this.GetSliceBounds((GameObject) this.m_top);
    OrientedBounds sliceBounds3 = this.GetSliceBounds((GameObject) this.m_topRight);
    OrientedBounds sliceBounds4 = this.GetSliceBounds((GameObject) this.m_bottomLeft);
    OrientedBounds sliceBounds5 = this.GetSliceBounds((GameObject) this.m_bottom);
    OrientedBounds sliceBounds6 = this.GetSliceBounds((GameObject) this.m_bottomRight);
    float num1 = Mathf.Max(sliceBounds1.Extents[heightDirection].magnitude, sliceBounds2.Extents[heightDirection].magnitude, sliceBounds3.Extents[heightDirection].magnitude) * 2f;
    float num2 = Mathf.Max(sliceBounds4.Extents[heightDirection].magnitude, sliceBounds5.Extents[heightDirection].magnitude, sliceBounds6.Extents[heightDirection].magnitude) * 2f;
    this.SetHeight(height - num1 - num2);
  }

  public void SetEntireSize(Vector2 size) => this.SetEntireSize(size.x, size.y);

  public void SetEntireSize(float width, float height)
  {
    int widthDirection = (int) this.m_WidthDirection;
    int heightDirection = (int) this.m_HeightDirection;
    OrientedBounds sliceBounds1 = this.GetSliceBounds((GameObject) this.m_topLeft);
    OrientedBounds sliceBounds2 = this.GetSliceBounds((GameObject) this.m_top);
    OrientedBounds sliceBounds3 = this.GetSliceBounds((GameObject) this.m_topRight);
    OrientedBounds sliceBounds4 = this.GetSliceBounds((GameObject) this.m_left);
    OrientedBounds sliceBounds5 = this.GetSliceBounds((GameObject) this.m_right);
    OrientedBounds sliceBounds6 = this.GetSliceBounds((GameObject) this.m_bottomLeft);
    OrientedBounds sliceBounds7 = this.GetSliceBounds((GameObject) this.m_bottom);
    OrientedBounds sliceBounds8 = this.GetSliceBounds((GameObject) this.m_bottomRight);
    float num1 = Mathf.Max(sliceBounds1.Extents[widthDirection].magnitude, sliceBounds4.Extents[widthDirection].magnitude, sliceBounds6.Extents[widthDirection].magnitude) * 2f;
    float num2 = Mathf.Max(sliceBounds3.Extents[widthDirection].magnitude, sliceBounds5.Extents[widthDirection].magnitude, sliceBounds8.Extents[widthDirection].magnitude) * 2f;
    float num3 = Mathf.Max(sliceBounds1.Extents[heightDirection].magnitude, sliceBounds2.Extents[heightDirection].magnitude, sliceBounds3.Extents[heightDirection].magnitude) * 2f;
    float num4 = Mathf.Max(sliceBounds6.Extents[heightDirection].magnitude, sliceBounds7.Extents[heightDirection].magnitude, sliceBounds8.Extents[heightDirection].magnitude) * 2f;
    this.SetSize(width - num1 - num2, height - num3 - num4);
  }

  public void SetWidth(float width)
  {
    width = Mathf.Max(width, 0.0f);
    int widthDirection = (int) this.m_WidthDirection;
    this.SetSliceSize((GameObject) this.m_top, new WorldDimensionIndex(width, widthDirection));
    this.SetSliceSize((GameObject) this.m_bottom, new WorldDimensionIndex(width, widthDirection));
    this.SetSliceSize((GameObject) this.m_middle, new WorldDimensionIndex(width, widthDirection));
    this.UpdateAllSlices();
  }

  public void SetHeight(float height)
  {
    height = Mathf.Max(height, 0.0f);
    int heightDirection = (int) this.m_HeightDirection;
    this.SetSliceSize((GameObject) this.m_left, new WorldDimensionIndex(height, heightDirection));
    this.SetSliceSize((GameObject) this.m_right, new WorldDimensionIndex(height, heightDirection));
    this.SetSliceSize((GameObject) this.m_middle, new WorldDimensionIndex(height, heightDirection));
    this.UpdateAllSlices();
  }

  public void SetSize(Vector2 size) => this.SetSize(size.x, size.y);

  public void SetSize(float width, float height)
  {
    width = Mathf.Max(width, 0.0f);
    height = Mathf.Max(height, 0.0f);
    int widthDirection = (int) this.m_WidthDirection;
    int heightDirection = (int) this.m_HeightDirection;
    this.SetSliceSize((GameObject) this.m_top, new WorldDimensionIndex(width, widthDirection));
    this.SetSliceSize((GameObject) this.m_bottom, new WorldDimensionIndex(width, widthDirection));
    this.SetSliceSize((GameObject) this.m_left, new WorldDimensionIndex(height, heightDirection));
    this.SetSliceSize((GameObject) this.m_right, new WorldDimensionIndex(height, heightDirection));
    this.SetSliceSize((GameObject) this.m_middle, new WorldDimensionIndex(width, widthDirection), new WorldDimensionIndex(height, heightDirection));
    this.UpdateAllSlices();
  }

  public void SetMiddleScale(float scaleWidth, float scaleHeight)
  {
    Vector3 localScale = this.m_middle.m_slice.transform.localScale;
    localScale[(int) this.m_WidthDirection] = scaleWidth;
    localScale[(int) this.m_HeightDirection] = scaleHeight;
    this.m_middle.m_slice.transform.localScale = localScale;
    this.UpdateSegmentsToMatchMiddle();
    this.UpdateAllSlices();
  }

  public Vector2 GetWorldDimensions()
  {
    OrientedBounds orientedWorldBounds = TransformUtil.ComputeOrientedWorldBounds((GameObject) this.m_middle, this.m_ignore);
    return new Vector2(orientedWorldBounds.Extents[(int) this.m_WidthDirection].magnitude * 2f, orientedWorldBounds.Extents[(int) this.m_HeightDirection].magnitude * 2f);
  }

  private OrientedBounds GetSliceBounds(GameObject slice)
  {
    if ((Object) slice != (Object) null)
      return TransformUtil.ComputeOrientedWorldBounds(slice, this.m_ignore);
    return new OrientedBounds()
    {
      Extents = new Vector3[3]
      {
        Vector3.zero,
        Vector3.zero,
        Vector3.zero
      },
      Origin = Vector3.zero,
      CenterOffset = Vector3.zero
    };
  }

  private void SetSliceSize(GameObject slice, params WorldDimensionIndex[] dimensions)
  {
    if (!((Object) slice != (Object) null))
      return;
    TransformUtil.SetLocalScaleToWorldDimension(slice, this.m_ignore, dimensions);
  }

  private void UpdateAllSlices()
  {
    Transform transform1 = this.m_topRow.m_slice.transform;
    List<MultiSliceElement.Slice> slices1 = new List<MultiSliceElement.Slice>();
    slices1.Add(this.m_topLeft);
    slices1.Add(this.m_top);
    slices1.Add(this.m_topRight);
    int widthDirection1 = (int) this.m_WidthDirection;
    this.UpdateRowSlices(transform1, slices1, (MultiSliceElement.Direction) widthDirection1);
    Transform transform2 = this.m_midRow.m_slice.transform;
    List<MultiSliceElement.Slice> slices2 = new List<MultiSliceElement.Slice>();
    slices2.Add(this.m_left);
    slices2.Add(this.m_middle);
    slices2.Add(this.m_right);
    int widthDirection2 = (int) this.m_WidthDirection;
    this.UpdateRowSlices(transform2, slices2, (MultiSliceElement.Direction) widthDirection2);
    Transform transform3 = this.m_btmRow.m_slice.transform;
    List<MultiSliceElement.Slice> slices3 = new List<MultiSliceElement.Slice>();
    slices3.Add(this.m_bottomLeft);
    slices3.Add(this.m_bottom);
    slices3.Add(this.m_bottomRight);
    int widthDirection3 = (int) this.m_WidthDirection;
    this.UpdateRowSlices(transform3, slices3, (MultiSliceElement.Direction) widthDirection3);
    Transform transform4 = this.transform;
    List<MultiSliceElement.Slice> slices4 = new List<MultiSliceElement.Slice>();
    slices4.Add(this.m_topRow);
    slices4.Add(this.m_midRow);
    slices4.Add(this.m_btmRow);
    int heightDirection = (int) this.m_HeightDirection;
    this.UpdateRowSlices(transform4, slices4, (MultiSliceElement.Direction) heightDirection);
  }

  private void UpdateRowSlices(
    Transform parent,
    List<MultiSliceElement.Slice> slices,
    MultiSliceElement.Direction direction)
  {
    Vector3 zero1 = Vector3.zero;
    Vector3 zero2 = Vector3.zero;
    zero1[(int) direction] = this.m_localSliceSpacing[(int) direction];
    zero2[(int) direction] = this.m_localPinnedPointOffset[(int) direction];
    MultiSliceElement.PositionSlices(parent, slices, this.m_reverse, direction, this.m_useUberText, zero1, zero2, this.m_XAlign, this.m_YAlign, this.m_ZAlign, this.m_ignore);
  }

  private void UpdateSegmentsToMatchMiddle()
  {
    OrientedBounds orientedWorldBounds = TransformUtil.ComputeOrientedWorldBounds((GameObject) this.m_middle, this.m_ignore);
    if (orientedWorldBounds == null)
      return;
    float dimension1 = orientedWorldBounds.Extents[(int) this.m_WidthDirection].magnitude * 2f;
    float dimension2 = orientedWorldBounds.Extents[(int) this.m_HeightDirection].magnitude * 2f;
    int widthDirection = (int) this.m_WidthDirection;
    int heightDirection = (int) this.m_HeightDirection;
    this.SetSliceSize((GameObject) this.m_top, new WorldDimensionIndex(dimension1, widthDirection));
    this.SetSliceSize((GameObject) this.m_bottom, new WorldDimensionIndex(dimension1, widthDirection));
    this.SetSliceSize((GameObject) this.m_left, new WorldDimensionIndex(dimension2, heightDirection));
    this.SetSliceSize((GameObject) this.m_right, new WorldDimensionIndex(dimension2, heightDirection));
  }
}
