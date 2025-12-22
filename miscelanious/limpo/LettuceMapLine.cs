using UnityEngine;

[RequireComponent(typeof (LineRenderer))]
public class LettuceMapLine : MonoBehaviour
{
  public LineRenderer m_GlowLineRenderer;
  public float m_VerticalOffsetFromStart = 7f;
  public float m_HorizontalOffsetPerLineConnectingToEndPoint = 2f;
  public float m_VerticalOffsetPerLineFromSameDirection = 2f;
  public float m_CornerRadius = 1f;

  public LineRenderer m_LineRenderer => this.GetComponent<LineRenderer>();

  public Transform m_StartBone { get; set; }

  public Transform m_EndBone { get; set; }

  public LettuceMapLine.ConnectionType m_ConnectionType { get; set; }

  public int m_ConnectionIndex { get; set; }

  public int m_NumParentConnectionsComingFromLeft { get; set; }

  public int m_NumParentConnectionsComingFromRight { get; set; }

  public void RefreshLine()
  {
    Vector3[] linePoints = this.CalculateLinePoints();
    this.m_LineRenderer.positionCount = linePoints.Length;
    this.m_LineRenderer.SetPositions(linePoints);
    if (!((Object) this.m_GlowLineRenderer != (Object) null))
      return;
    this.m_GlowLineRenderer.positionCount = linePoints.Length;
    this.m_GlowLineRenderer.SetPositions(linePoints);
  }

  private Vector3[] CalculateLinePoints()
  {
    int num1 = this.m_NumParentConnectionsComingFromLeft + this.m_NumParentConnectionsComingFromRight;
    Vector3 vector3_1 = this.transform.InverseTransformPoint(this.m_StartBone.position);
    Vector3 vector3_2 = this.transform.InverseTransformPoint(this.m_EndBone.position);
    Vector3[] linePoints = new Vector3[0];
    float x = (num1 % 2 == 0 ? this.m_HorizontalOffsetPerLineConnectingToEndPoint / 2f : 0.0f) + this.m_HorizontalOffsetPerLineConnectingToEndPoint * (float) (this.m_ConnectionIndex - num1 / 2);
    float num2 = this.m_ConnectionIndex >= this.m_NumParentConnectionsComingFromLeft ? (float) (this.m_ConnectionIndex - this.m_NumParentConnectionsComingFromLeft) * this.m_VerticalOffsetPerLineFromSameDirection : (float) (this.m_NumParentConnectionsComingFromLeft - 1 - this.m_ConnectionIndex) * this.m_VerticalOffsetPerLineFromSameDirection;
    switch (this.m_ConnectionType)
    {
      case LettuceMapLine.ConnectionType.NEXT_ROW:
        Vector3 vector3_3 = vector3_2 + new Vector3(x, 0.0f, 0.0f);
        if ((double) Mathf.Abs(vector3_1.x - vector3_3.x) < (double) this.m_CornerRadius)
        {
          linePoints = new Vector3[2]
          {
            vector3_1,
            vector3_3
          };
          break;
        }
        int num3 = (double) vector3_1.x > (double) vector3_3.x ? -1 : 1;
        Vector3 vector3_4 = vector3_1 + new Vector3(0.0f, 0.0f, this.m_VerticalOffsetFromStart + num2) + new Vector3(this.m_CornerRadius * (float) num3, 0.0f, -this.m_CornerRadius);
        Vector3 vector3_5 = vector3_4 + Vector3.left * this.m_CornerRadius * (float) num3;
        Vector3 vector3_6 = vector3_4 + Vector3.forward * this.m_CornerRadius;
        Vector3 vector3_7 = new Vector3(vector3_3.x, vector3_1.y, vector3_1.z + this.m_VerticalOffsetFromStart + num2) + new Vector3(-this.m_CornerRadius * (float) num3, 0.0f, this.m_CornerRadius);
        Vector3 vector3_8 = vector3_7 + Vector3.back * this.m_CornerRadius;
        Vector3 vector3_9 = vector3_7 + Vector3.right * this.m_CornerRadius * (float) num3;
        linePoints = new Vector3[10]
        {
          vector3_1,
          vector3_4 + Vector3.Slerp(vector3_5 - vector3_4, vector3_6 - vector3_4, 0.0f),
          vector3_4 + Vector3.Slerp(vector3_5 - vector3_4, vector3_6 - vector3_4, 0.33f),
          vector3_4 + Vector3.Slerp(vector3_5 - vector3_4, vector3_6 - vector3_4, 0.66f),
          vector3_4 + Vector3.Slerp(vector3_5 - vector3_4, vector3_6 - vector3_4, 1f),
          vector3_7 + Vector3.Slerp(vector3_8 - vector3_7, vector3_9 - vector3_7, 0.0f),
          vector3_7 + Vector3.Slerp(vector3_8 - vector3_7, vector3_9 - vector3_7, 0.33f),
          vector3_7 + Vector3.Slerp(vector3_8 - vector3_7, vector3_9 - vector3_7, 0.66f),
          vector3_7 + Vector3.Slerp(vector3_8 - vector3_7, vector3_9 - vector3_7, 1f),
          vector3_3
        };
        break;
      case LettuceMapLine.ConnectionType.SAME_ROW:
        linePoints = new Vector3[2]{ vector3_1, vector3_2 };
        break;
    }
    return linePoints;
  }

  public enum ConnectionType
  {
    INVALID,
    NEXT_ROW,
    SAME_ROW,
  }
}
