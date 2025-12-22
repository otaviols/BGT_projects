using System;
using UnityEngine;

[ExecuteInEditMode]
public class SocketIn_05z_ArrowController : MonoBehaviour
{
  [Header("Control Parameters")]
  public float ElevationAngle = 50f;
  public float Velocity = 10f;
  public Quaternion SpriteRotation;
  [Header("Arrows")]
  public SpriteRenderer[] Arrows;
  [Header("Timeline Properties")]
  public float Time;
  public float AlphaOverride;
  private SocketIn_05z_ArrowController.Instance[] m_instances;
  private const float c_alphaDistance = 0.1f;
  private const float c_minDistance = 0.4f;
  private const float c_angleVariation = 10f;
  private const float c_distanceVariation = 2f;

  private void OnEnable()
  {
    int length = this.Arrows != null ? this.Arrows.Length : 0;
    if (length > 0)
    {
      this.m_instances = new SocketIn_05z_ArrowController.Instance[length];
      float num1 = UnityEngine.Random.Range(0.0f, 360f);
      for (int index = 0; index < length; ++index)
      {
        float num2 = num1 + 360f * (float) index / (float) length;
        float elevationAngle = this.ElevationAngle;
        float y = num2 + UnityEngine.Random.Range(-10f, 10f);
        float z = elevationAngle + UnityEngine.Random.Range(-10f, 10f);
        Quaternion spriteRotation = this.SpriteRotation;
        Quaternion quaternion1 = Quaternion.Euler(0.0f, 0.0f, z) * spriteRotation;
        Quaternion quaternion2 = Quaternion.Euler(0.0f, y, 0.0f) * quaternion1;
        float num3 = UnityEngine.Random.Range(-2f, 0.0f);
        this.m_instances[index] = new SocketIn_05z_ArrowController.Instance()
        {
          Renderer = this.Arrows[index],
          Rotation = quaternion2,
          DistanceOffset = num3
        };
      }
    }
    else
      this.m_instances = Array.Empty<SocketIn_05z_ArrowController.Instance>();
  }

  private void UpdateAnimation(SocketIn_05z_ArrowController.Instance instance)
  {
    float num = this.Time * this.Velocity + instance.DistanceOffset;
    Quaternion rotation = instance.Rotation;
    Vector3 vector3 = rotation * new Vector3(num + 0.4f, 0.0f, 0.0f);
    if ((UnityEngine.Object) instance.Renderer != (UnityEngine.Object) null)
    {
      float a = Mathf.Clamp01(num / 0.1f);
      instance.Renderer.color = new Color(1f, 1f, 1f, Mathf.Min(a, this.AlphaOverride));
    }
    Transform transform = instance.Renderer.transform;
    transform.localPosition = vector3;
    transform.localRotation = rotation;
  }

  private void LateUpdate()
  {
    if (this.m_instances == null)
      return;
    foreach (SocketIn_05z_ArrowController.Instance instance in this.m_instances)
      this.UpdateAnimation(instance);
  }

  private struct Instance
  {
    public SpriteRenderer Renderer;
    public Quaternion Rotation;
    public float DistanceOffset;
  }
}
