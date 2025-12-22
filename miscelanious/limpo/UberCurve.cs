using System;
using System.Collections.Generic;
using UnityEngine;

public class UberCurve : MonoBehaviour
{
  public bool m_Looping;
  public bool m_Reverse;
  public float m_HandleSize = 0.3f;
  public List<UberCurve.UberCurveControlPoint> m_controlPoints;
  private int m_gizmoSteps = 10;
  private bool m_renderControlPoints;
  private bool m_renderStepPoints;
  private bool m_renderingGizmo;

  private void Awake() => this.CatmullRomInit();

  private void OnDrawGizmos() => this.CatmullRomGizmo();

  public Vector3 CatmullRomEvaluateWorldPosition(float position) => this.transform.TransformPoint(this.CatmullRomEvaluateLocalPosition(position));

  public Vector3 CatmullRomEvaluateLocalPosition(float position)
  {
    if (this.m_controlPoints == null)
      return Vector3.zero;
    int num = this.m_controlPoints.Count;
    if (!this.m_Looping)
      num = this.m_controlPoints.Count - 1;
    if (this.m_Reverse && !this.m_renderingGizmo)
      position = 1f - position;
    position = Mathf.Clamp01(position);
    int index = Mathf.FloorToInt(position * (float) num);
    return this.CatmullRomCalc(position * (float) num - (float) index, this.m_controlPoints[this.ClampIndexCatmullRom(index - 1)].position, this.m_controlPoints[index].position, this.m_controlPoints[this.ClampIndexCatmullRom(index + 1)].position, this.m_controlPoints[this.ClampIndexCatmullRom(index + 2)].position);
  }

  public Vector3 CatmullRomEvaluateDirection(float position)
  {
    if (this.m_controlPoints == null)
      return Vector3.zero;
    Vector3 localPosition = this.CatmullRomEvaluateLocalPosition(position);
    return Vector3.Normalize(this.CatmullRomEvaluateLocalPosition(position + 0.01f) - localPosition);
  }

  private void CatmullRomInit()
  {
    if (this.m_controlPoints != null)
      return;
    this.m_controlPoints = new List<UberCurve.UberCurveControlPoint>();
    for (int index = 0; index < 4; ++index)
      this.m_controlPoints.Add(new UberCurve.UberCurveControlPoint()
      {
        position = new Vector3(0.0f, 0.0f, (float) index * 4f)
      });
  }

  [ContextMenu("Show Curve Steps")]
  private void ShowRenderSteps() => this.m_renderStepPoints = !this.m_renderStepPoints;

  private void CatmullRomGizmo()
  {
    if (this.m_gizmoSteps < 1)
      this.m_gizmoSteps = 1;
    if (this.m_controlPoints == null)
      this.CatmullRomInit();
    if (this.m_controlPoints.Count < 4)
      return;
    this.m_renderingGizmo = true;
    Gizmos.matrix = this.transform.localToWorldMatrix;
    float num = !this.m_Looping ? 1f / (float) (this.m_gizmoSteps * (this.m_controlPoints.Count - 1)) : 1f / (float) (this.m_gizmoSteps * this.m_controlPoints.Count);
    Gizmos.color = Color.cyan;
    Vector3 from = this.m_controlPoints[0].position;
    for (float position = 0.0f; (double) position <= 1.0; position += num)
    {
      Vector3 localPosition = this.CatmullRomEvaluateLocalPosition(position);
      Gizmos.DrawLine(from, localPosition);
      from = localPosition;
    }
    if (this.m_renderStepPoints)
    {
      Gizmos.color = new Color(0.2f, 0.2f, 0.9f, 0.75f);
      for (float position = 0.0f; (double) position <= 1.0; position += num)
        Gizmos.DrawSphere(this.CatmullRomEvaluateLocalPosition(position), this.m_HandleSize * 0.15f);
    }
    if (this.m_renderControlPoints)
    {
      Gizmos.color = new Color(0.3f, 0.3f, 1f, 1f);
      for (int index = 0; index < this.m_controlPoints.Count; ++index)
        Gizmos.DrawSphere(this.m_controlPoints[index].position, 0.25f);
    }
    this.m_renderingGizmo = false;
  }

  private Vector3 CatmullRomCalc(
    float i,
    Vector3 pointA,
    Vector3 pointB,
    Vector3 pointC,
    Vector3 pointD)
  {
    Vector3 vector3_1 = 0.5f * (2f * pointB);
    Vector3 vector3_2 = 0.5f * (pointC - pointA);
    Vector3 vector3_3 = 0.5f * (2f * pointA - 5f * pointB + 4f * pointC - pointD);
    Vector3 vector3_4 = 0.5f * (-pointA + 3f * pointB - 3f * pointC + pointD);
    Vector3 vector3_5 = vector3_2 * i;
    return vector3_1 + vector3_5 + vector3_3 * i * i + vector3_4 * i * i * i;
  }

  private int ClampIndexCatmullRom(int pos)
  {
    if (this.m_Looping)
    {
      if (pos < 0)
        pos = this.m_controlPoints.Count - 1;
      if (pos > this.m_controlPoints.Count)
        pos = 1;
      else if (pos > this.m_controlPoints.Count - 1)
        pos = 0;
    }
    else
    {
      if (pos < 0)
        pos = 0;
      if (pos > this.m_controlPoints.Count - 1)
        pos = this.m_controlPoints.Count - 1;
    }
    return pos;
  }

  [Serializable]
  public class UberCurveControlPoint
  {
    public Vector3 position = Vector3.zero;
  }
}
