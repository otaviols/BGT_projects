using UnityEngine;

public class RotateByMovement : MonoBehaviour
{
  private Vector3 m_previousPos;
  public GameObject mParent;

  private void Update()
  {
    Transform transform = this.mParent.transform;
    if (this.m_previousPos == transform.localPosition)
      return;
    if (this.m_previousPos == Vector3.zero)
    {
      this.m_previousPos = transform.localPosition;
    }
    else
    {
      Vector3 localPosition = transform.localPosition;
      float f = localPosition.z - this.m_previousPos.z;
      float num = Mathf.Sqrt(Mathf.Pow(localPosition.x - this.m_previousPos.x, 2f) + Mathf.Pow(f, 2f));
      this.transform.localEulerAngles = new Vector3(90f, (float) ((double) Mathf.Asin(f / num) * 180.0 / 3.14159274101257) - 90f, 0.0f);
      this.m_previousPos = localPosition;
    }
  }
}
