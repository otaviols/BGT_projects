using UnityEngine;

public class TokyoDriftOnActivate : MonoBehaviour
{
  public Transform m_DriftTarget;
  public float m_DriftDuration = 0.5f;
  public float m_DriftScale = 1f;
  private Vector3 m_originalLocalPosition;
  private Vector3 m_originalWorldPosition;
  private Vector3 m_originalLocalScale;

  private void OnDisable()
  {
    this.transform.localPosition = this.m_originalLocalPosition;
    this.transform.localScale = this.m_originalLocalScale;
  }

  private void OnEnable()
  {
    this.m_originalLocalPosition = this.transform.localPosition;
    this.m_originalWorldPosition = this.transform.position;
    this.m_originalLocalScale = this.transform.localScale;
    AnimationUtil.GrowThenDrift(this.gameObject, this.m_originalWorldPosition, this.m_DriftScale);
  }
}
