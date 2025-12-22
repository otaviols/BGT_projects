using UnityEngine;

public class AspectRatioDependentPosition : MonoBehaviour
{
  public Vector3 m_minLocalPosition;
  public Vector3 m_wideLocalPosition;
  public Vector3 m_extraWideLocalPosition;

  private void Awake() => this.transform.localPosition = TransformUtil.GetAspectRatioDependentPosition(this.m_minLocalPosition, this.m_wideLocalPosition, this.m_extraWideLocalPosition);
}
