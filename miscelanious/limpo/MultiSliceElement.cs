using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
[ExecuteAlways]
public class MultiSliceElement : MonoBehaviour
{
  [CustomEditField(ListTable = true)]
  public List<MultiSliceElement.Slice> m_slices = new List<MultiSliceElement.Slice>();
  public List<GameObject> m_ignore = new List<GameObject>();
  public Vector3 m_localPinnedPointOffset = Vector3.zero;
  public MultiSliceElement.XAxisAlign m_XAlign;
  public MultiSliceElement.YAxisAlign m_YAlign = MultiSliceElement.YAxisAlign.BOTTOM;
  public MultiSliceElement.ZAxisAlign m_ZAlign = MultiSliceElement.ZAxisAlign.BACK;
  public Vector3 m_localSliceSpacing = Vector3.zero;
  public MultiSliceElement.Direction m_direction;
  public bool m_reverse;
  public bool m_useUberText;
  public bool m_weldable;
  private GeometryWeld m_sliceWeld;

  public void AddSlice(GameObject obj) => this.AddSlice(obj, Vector3.zero, Vector3.zero);

  public void AddSlice(
    GameObject obj,
    Vector3 minLocalPadding,
    Vector3 maxLocalPadding,
    bool reverse = false)
  {
    this.m_slices.Add(new MultiSliceElement.Slice()
    {
      m_slice = obj,
      m_minLocalPadding = minLocalPadding,
      m_maxLocalPadding = maxLocalPadding,
      m_reverse = reverse
    });
  }

  public void ClearSlices() => this.m_slices.Clear();

  public void UpdateSlices()
  {
    if (this.m_sliceWeld != null)
      this.m_sliceWeld.Unweld();
    MultiSliceElement.PositionSlices(this.transform, this.m_slices, this.m_reverse, this.m_direction, this.m_useUberText, this.m_localSliceSpacing, this.m_localPinnedPointOffset, this.m_XAlign, this.m_YAlign, this.m_ZAlign, this.m_ignore);
    if (!this.m_weldable)
      return;
    this.m_sliceWeld = new GeometryWeld(this.gameObject, this.m_slices.Select<MultiSliceElement.Slice, GameObject>((Func<MultiSliceElement.Slice, GameObject>) (x => x.m_slice)).ToArray<GameObject>());
  }

  public static void PositionSlices(
    Transform root,
    List<MultiSliceElement.Slice> slices,
    bool reverseDir,
    MultiSliceElement.Direction dir,
    bool useUberText,
    Vector3 localSliceSpacing,
    Vector3 localPinnedPointOffset,
    MultiSliceElement.XAxisAlign xAlign,
    MultiSliceElement.YAxisAlign yAlign,
    MultiSliceElement.ZAxisAlign zAlign,
    List<GameObject> ignoreObjects = null)
  {
    if (slices.Count == 0)
      return;
    float num1 = reverseDir ? -1f : 1f;
    int index1 = (int) dir;
    MultiSliceElement.Slice[] array = slices.FindAll((Predicate<MultiSliceElement.Slice>) (s => TransformUtil.CanComputeOrientedWorldBounds(s.m_slice, useUberText, ignoreObjects))).ToArray();
    if (array.Length == 0)
      return;
    Vector3 zero = Vector3.zero;
    Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
    Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    MultiSliceElement.Slice slice1 = array[0];
    GameObject slice2 = slice1.m_slice;
    slice2.transform.localPosition = Vector3.zero;
    OrientedBounds orientedWorldBounds1 = TransformUtil.ComputeOrientedWorldBounds(slice2, useUberText, slice1.m_minLocalPadding, slice1.m_maxLocalPadding, ignoreObjects);
    float num2 = num1 * (slice1.m_reverse ? -1f : 1f);
    Vector3 vector3_1 = (orientedWorldBounds1.Extents[0] + orientedWorldBounds1.Extents[1] + orientedWorldBounds1.Extents[2]) * num2;
    slice2.transform.position += orientedWorldBounds1.CenterOffset + vector3_1;
    Vector3 vector3_2 = orientedWorldBounds1.Extents[index1] * num2 + vector3_1;
    TransformUtil.GetBoundsMinMax((Vector3) (worldToLocalMatrix * (Vector4) (slice2.transform.position - orientedWorldBounds1.CenterOffset)), (Vector3) (worldToLocalMatrix * (Vector4) orientedWorldBounds1.Extents[0]), (Vector3) (worldToLocalMatrix * (Vector4) orientedWorldBounds1.Extents[1]), (Vector3) (worldToLocalMatrix * (Vector4) orientedWorldBounds1.Extents[2]), ref min, ref max);
    Vector3 vector3_3 = localSliceSpacing * num1;
    for (int index2 = 1; index2 < array.Length; ++index2)
    {
      MultiSliceElement.Slice slice3 = array[index2];
      GameObject slice4 = slice3.m_slice;
      float num3 = num1 * (slice3.m_reverse ? -1f : 1f);
      slice4.transform.localPosition = Vector3.zero;
      OrientedBounds orientedWorldBounds2 = TransformUtil.ComputeOrientedWorldBounds(slice4, useUberText, slice3.m_minLocalPadding, slice3.m_maxLocalPadding, ignoreObjects);
      Vector3 vector3_4 = (Vector3) (slice4.transform.localToWorldMatrix * (Vector4) vector3_3);
      Vector3 vector3_5 = orientedWorldBounds2.Extents[index1] * num3;
      slice4.transform.position += orientedWorldBounds2.CenterOffset + vector3_2 + vector3_5 + vector3_4;
      vector3_2 += vector3_5 * 2f + vector3_4;
      TransformUtil.GetBoundsMinMax((Vector3) (worldToLocalMatrix * (Vector4) (slice4.transform.position - orientedWorldBounds2.CenterOffset)), (Vector3) (worldToLocalMatrix * (Vector4) orientedWorldBounds2.Extents[0]), (Vector3) (worldToLocalMatrix * (Vector4) orientedWorldBounds2.Extents[1]), (Vector3) (worldToLocalMatrix * (Vector4) orientedWorldBounds2.Extents[2]), ref min, ref max);
    }
    Vector3 vector3_6 = new Vector3(min.x, max.y, min.z);
    Vector3 vector3_7 = new Vector3(max.x, min.y, max.z);
    Vector3 vector3_8 = (Vector3) (root.localToWorldMatrix * (Vector4) (vector3_6 + MultiSliceElement.GetAlignmentVector(vector3_7 - vector3_6, xAlign, yAlign, zAlign)));
    Vector3 vector3_9 = (Vector3) (root.localToWorldMatrix * (Vector4) localPinnedPointOffset * num1) + (root.position - vector3_8);
    foreach (MultiSliceElement.Slice slice5 in array)
      slice5.m_slice.transform.position += vector3_9;
  }

  private static Vector3 GetAlignmentVector(
    Vector3 interpolate,
    MultiSliceElement.XAxisAlign x,
    MultiSliceElement.YAxisAlign y,
    MultiSliceElement.ZAxisAlign z)
  {
    return new Vector3(interpolate.x * ((float) x * 0.5f), interpolate.y * ((float) y * 0.5f), interpolate.z * ((float) z * 0.5f));
  }

  public enum Direction
  {
    X,
    Y,
    Z,
  }

  public enum XAxisAlign
  {
    LEFT,
    MIDDLE,
    RIGHT,
  }

  public enum YAxisAlign
  {
    TOP,
    MIDDLE,
    BOTTOM,
  }

  public enum ZAxisAlign
  {
    FRONT,
    MIDDLE,
    BACK,
  }

  [Serializable]
  public class Slice
  {
    public GameObject m_slice;
    public Vector3 m_minLocalPadding;
    public Vector3 m_maxLocalPadding;
    public bool m_reverse;

    public static implicit operator GameObject(MultiSliceElement.Slice slice) => slice?.m_slice;
  }
}
