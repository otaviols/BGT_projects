using UnityEngine;

public class ReticlePerspectiveAdjust : MonoBehaviour
{
  public float m_HorizontalAdjustment = 20f;
  public float m_VertialAdjustment = 20f;

  private void Update()
  {
    Camera main = Camera.main;
    if ((Object) main == (Object) null)
      return;
    Vector3 screenPoint = main.WorldToScreenPoint(this.transform.position);
    float num1 = (float) ((double) screenPoint.x / (double) main.pixelWidth - 0.5);
    float num2 = (float) -((double) screenPoint.y / (double) main.pixelHeight - 0.5);
    this.transform.rotation = Quaternion.identity;
    this.transform.Rotate(new Vector3(this.m_VertialAdjustment * num2, 0.0f, this.m_HorizontalAdjustment * num1), Space.World);
  }
}
