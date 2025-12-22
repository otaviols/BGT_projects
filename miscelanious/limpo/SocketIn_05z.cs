using UnityEngine;

[ExecuteInEditMode]
public class SocketIn_05z : MonoBehaviour
{
  [Header("Control Points")]
  public Vector3 StartPosition;
  public Vector3 EndPosition;
  public float Radius1;
  public float Radius2;
  public bool InvertDirection;
  public AnimationCurve Elevation;
  public AnimationCurve Pitch;
  public AnimationCurve Roll;
  [Header("Timeline Hooks")]
  public float Time;
  public float ElevationScale = 1f;

  private void UpdateAnimation()
  {
    float num1 = Mathf.Clamp01(this.Time);
    float num2 = Mathf.LerpUnclamped(this.Radius1, this.Radius2, num1);
    Vector3 vector3_1 = Vector3.LerpUnclamped(this.StartPosition + Vector3.right * this.Radius1, this.EndPosition + Vector3.right * this.Radius2, num1);
    float f = (float) (6.28318548202515 * (double) num1 * (this.InvertDirection ? -1.0 : 1.0));
    Vector3 vector3_2 = new Vector3(-Mathf.Cos(f), 0.0f, Mathf.Sin(f)) * num2;
    Vector3 vector3_3 = vector3_1 + vector3_2;
    if (this.Elevation != null)
      vector3_3.y = Mathf.LerpUnclamped(this.EndPosition.y, this.StartPosition.y, this.Elevation.Evaluate(num1));
    vector3_3.y *= this.ElevationScale;
    float z = 0.0f;
    if (this.Roll != null)
      z = this.Roll.Evaluate(num1);
    float x = 0.0f;
    if (this.Pitch != null)
      x = this.Pitch.Evaluate(num1);
    this.transform.localPosition = vector3_3;
    this.transform.localRotation = Quaternion.Euler(new Vector3(x, (float) (360.0 * (double) num1 * (this.InvertDirection ? -1.0 : 1.0)), z));
  }

  private void LateUpdate() => this.UpdateAnimation();
}
