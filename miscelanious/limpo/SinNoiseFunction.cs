using UnityEngine;

public class SinNoiseFunction : ScriptableObject
{
  public Vector4 Frequency;
  public Vector4 RelativeAmplitude;
  public Vector4 OffsetRate;

  public Vector4 GetAmplitude(float maxAmplitude)
  {
    Vector4 relativeAmplitude = this.RelativeAmplitude;
    float num = Mathf.Abs(this.RelativeAmplitude.x) + Mathf.Abs(this.RelativeAmplitude.y) + Mathf.Abs(this.RelativeAmplitude.z) + Mathf.Abs(this.RelativeAmplitude.w);
    if ((double) num > (double) Mathf.Epsilon)
      relativeAmplitude /= num;
    return relativeAmplitude * maxAmplitude;
  }
}
