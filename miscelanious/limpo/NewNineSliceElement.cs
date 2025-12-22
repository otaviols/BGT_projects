using UnityEngine;

[ExecuteAlways]
public class NewNineSliceElement : MonoBehaviour
{
  public GameObject m_topLeft;
  public GameObject m_top;
  public GameObject m_topRight;
  public GameObject m_left;
  public GameObject m_right;
  public GameObject m_middle;
  public GameObject m_bottomLeft;
  public GameObject m_bottom;
  public GameObject m_bottomRight;
  public GameObject m_anchorBone;
  public NewNineSliceElement.PinnedPoint m_pinnedPoint = NewNineSliceElement.PinnedPoint.TOP;
  public NewNineSliceElement.PlaneAxis m_planeAxis = NewNineSliceElement.PlaneAxis.XZ;
  public Vector3 m_pinnedPointOffset;
  public Vector2 m_middleScale;
  public Vector2 m_size;
  public NewNineSliceElement.Mode m_mode;

  public virtual void SetSize(float width, float height)
  {
    if (this.m_mode == NewNineSliceElement.Mode.UseSize)
    {
      this.m_size = new Vector2(width, height);
      Vector3 size1 = this.m_topLeft.GetComponent<Renderer>().bounds.size;
      Vector3 size2 = this.m_bottomLeft.GetComponent<Renderer>().bounds.size;
      width = Mathf.Max(width - (size1.x + size2.x), 1f);
      height = Mathf.Max(height - (size1.y + size2.y), 1f);
      this.SetPieceWidth(this.m_middle, width);
      this.SetPieceHeight(this.m_middle, height);
      this.SetPieceWidth(this.m_top, width);
      this.SetPieceWidth(this.m_bottom, width);
      this.SetPieceHeight(this.m_left, height);
      this.SetPieceHeight(this.m_right, height);
    }
    else
    {
      TransformUtil.SetLocalScaleXZ(this.m_middle, new Vector2(width, height));
      TransformUtil.SetLocalScaleX(this.m_top, width);
      TransformUtil.SetLocalScaleX(this.m_bottom, width);
      TransformUtil.SetLocalScaleZ(this.m_left, height);
      TransformUtil.SetLocalScaleZ(this.m_right, height);
    }
    Vector3 vector3_1;
    Vector3 vector3_2;
    Vector3 vector3_3;
    Vector3 vector3_4;
    Vector3 vector3_5;
    if (this.m_planeAxis == NewNineSliceElement.PlaneAxis.XZ)
    {
      vector3_1 = new Vector3(0.0f, 0.0f, 1f);
      vector3_2 = new Vector3(0.0f, 0.0f, 0.0f);
      vector3_3 = new Vector3(0.0f, 0.0f, 0.5f);
      vector3_4 = new Vector3(1f, 0.0f, 0.5f);
      vector3_5 = new Vector3(0.5f, 0.0f, 0.5f);
    }
    else
    {
      vector3_1 = new Vector3(0.0f, 1f, 0.0f);
      vector3_2 = new Vector3(0.0f, 0.0f, 0.0f);
      vector3_3 = new Vector3(0.0f, 0.5f, 0.0f);
      vector3_4 = new Vector3(1f, 0.5f, 0.0f);
      vector3_5 = new Vector3(0.5f, 0.5f, 0.0f);
    }
    switch (this.m_pinnedPoint)
    {
      case NewNineSliceElement.PinnedPoint.TOPLEFT:
        TransformUtil.SetPoint(this.m_topLeft, Anchor.TOP_LEFT, this.m_anchorBone, Anchor.TOP_LEFT);
        TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_left, vector3_1, this.m_topLeft, vector3_2);
        TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomLeft, vector3_1, this.m_left, vector3_2);
        TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
        break;
      case NewNineSliceElement.PinnedPoint.TOP:
        TransformUtil.SetPoint(this.m_top, Anchor.TOP, this.m_anchorBone, Anchor.TOP);
        TransformUtil.SetPoint(this.m_topLeft, Anchor.RIGHT, this.m_top, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_left, vector3_1, this.m_topLeft, vector3_2);
        TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomLeft, vector3_1, this.m_left, vector3_2);
        TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
        break;
      case NewNineSliceElement.PinnedPoint.TOPRIGHT:
        TransformUtil.SetPoint(this.m_topRight, Anchor.TOP_RIGHT, this.m_anchorBone, Anchor.TOP_RIGHT);
        TransformUtil.SetPoint(this.m_top, Anchor.RIGHT, this.m_topRight, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_topLeft, Anchor.RIGHT, this.m_top, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_left, vector3_1, this.m_topLeft, vector3_2);
        TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomLeft, vector3_1, this.m_left, vector3_2);
        TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
        break;
      case NewNineSliceElement.PinnedPoint.LEFT:
        TransformUtil.SetPoint(this.m_left, vector3_3, this.m_anchorBone, vector3_3);
        TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topLeft, vector3_2, this.m_left, vector3_1);
        TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomLeft, vector3_1, this.m_left, vector3_2);
        TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
        break;
      case NewNineSliceElement.PinnedPoint.MIDDLE:
        TransformUtil.SetPoint(this.m_middle, vector3_5, this.m_anchorBone, vector3_5);
        TransformUtil.SetPoint(this.m_left, Anchor.RIGHT, this.m_middle, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topLeft, vector3_2, this.m_left, vector3_1);
        TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomLeft, vector3_1, this.m_left, vector3_2);
        TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
        break;
      case NewNineSliceElement.PinnedPoint.RIGHT:
        TransformUtil.SetPoint(this.m_right, vector3_4, this.m_anchorBone, vector3_4);
        TransformUtil.SetPoint(this.m_middle, Anchor.RIGHT, this.m_right, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_left, Anchor.RIGHT, this.m_middle, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_topLeft, vector3_2, this.m_left, vector3_1);
        TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomLeft, vector3_1, this.m_left, vector3_2);
        TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
        break;
      case NewNineSliceElement.PinnedPoint.BOTTOMLEFT:
        TransformUtil.SetPoint(this.m_bottomLeft, Anchor.BOTTOM_LEFT, this.m_anchorBone, Anchor.BOTTOM_LEFT);
        TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_left, vector3_2, this.m_bottomLeft, vector3_1);
        TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topLeft, vector3_2, this.m_left, vector3_1);
        TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
        break;
      case NewNineSliceElement.PinnedPoint.BOTTOM:
        TransformUtil.SetPoint(this.m_bottom, Anchor.BOTTOM, this.m_anchorBone, Anchor.BOTTOM);
        TransformUtil.SetPoint(this.m_bottomLeft, Anchor.RIGHT, this.m_bottom, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_left, vector3_2, this.m_bottomLeft, vector3_1);
        TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topLeft, vector3_2, this.m_left, vector3_1);
        TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
        break;
      case NewNineSliceElement.PinnedPoint.BOTTOMRIGHT:
        TransformUtil.SetPoint(this.m_bottomRight, Anchor.BOTTOM_RIGHT, this.m_anchorBone, Anchor.BOTTOM_RIGHT);
        TransformUtil.SetPoint(this.m_bottom, Anchor.RIGHT, this.m_bottomRight, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_bottomLeft, Anchor.RIGHT, this.m_bottom, Anchor.LEFT);
        TransformUtil.SetPoint(this.m_left, vector3_2, this.m_bottomLeft, vector3_1);
        TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topLeft, vector3_2, this.m_left, vector3_1);
        TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
        TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
        break;
    }
  }

  private void SetPieceWidth(GameObject piece, float width)
  {
    Vector3 localScale = piece.transform.localScale with
    {
      x = width * piece.transform.localScale.x / piece.GetComponent<Renderer>().bounds.size.x
    };
    piece.transform.localScale = localScale;
  }

  private void SetPieceHeight(GameObject piece, float height)
  {
    Vector3 localScale = piece.transform.localScale with
    {
      z = height * piece.transform.localScale.z / piece.GetComponent<Renderer>().bounds.size.y
    };
    piece.transform.localScale = localScale;
  }

  public enum PinnedPoint
  {
    TOPLEFT,
    TOP,
    TOPRIGHT,
    LEFT,
    MIDDLE,
    RIGHT,
    BOTTOMLEFT,
    BOTTOM,
    BOTTOMRIGHT,
  }

  public enum PlaneAxis
  {
    XY,
    XZ,
  }

  public enum Mode
  {
    UseMiddleScale,
    UseSize,
  }
}
