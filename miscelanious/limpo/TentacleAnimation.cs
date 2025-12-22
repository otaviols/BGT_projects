using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TentacleAnimation : MonoBehaviour
{
  private const int RANDOM_INIT_COUNT = 5;
  public float m_MaxAngle = 45f;
  public float m_AnimSpeed = 0.5f;
  public float m_Secondary = 10f;
  public float m_Smooth = 3f;
  [Range(0.0f, 1f)]
  public float m_X_Intensity = 1f;
  [Range(0.0f, 1f)]
  public float m_Y_Intensity = 1f;
  [Range(0.0f, 1f)]
  public float m_Z_Intensity = 1f;
  public AnimationCurve m_IntensityCurve;
  public List<Transform> m_Bones;
  public List<Transform> m_ControlBones;
  private float[] m_intensityValues;
  private float[] m_angleX;
  private float[] m_angleY;
  private float[] m_angleZ;
  private float[] m_velocityX;
  private float[] m_velocityY;
  private float[] m_velocityZ;
  private float[] m_lastX;
  private float[] m_lastY;
  private float[] m_lastZ;
  private Quaternion[] m_orgRotation;
  private float m_jointStep;
  private float m_secondaryAnim = 0.05f;
  private float m_smoothing = 0.3f;
  private float m_seedX;
  private float m_seedY;
  private float m_seedZ;
  private float m_timeSeed;
  private int[] m_randomNumbers;
  private int m_randomCount;

  private void Start() => this.Init();

  private void Update()
  {
    if (this.m_Bones == null)
      return;
    this.CalculateBoneAngles();
    if (this.m_ControlBones.Count > 0)
    {
      for (int index = 0; index < this.m_Bones.Count; ++index)
      {
        this.m_Bones[index].localRotation = this.m_ControlBones[index].localRotation;
        this.m_Bones[index].localPosition = this.m_ControlBones[index].localPosition;
        this.m_Bones[index].localScale = this.m_ControlBones[index].localScale;
        this.m_Bones[index].Rotate(this.m_angleX[index], this.m_angleY[index], this.m_angleZ[index]);
      }
    }
    else
    {
      for (int index = 0; index < this.m_Bones.Count; ++index)
      {
        this.m_Bones[index].rotation = this.m_orgRotation[index];
        this.m_Bones[index].Rotate(this.m_angleX[index], this.m_angleY[index], this.m_angleZ[index]);
      }
    }
    this.m_secondaryAnim = this.m_Secondary * 0.01f;
    this.m_smoothing = this.m_Smooth * 0.1f;
  }

  private void Init()
  {
    if (this.m_Bones == null)
      return;
    if (this.m_IntensityCurve == null || this.m_IntensityCurve.length < 1)
      this.m_IntensityCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
    this.m_randomNumbers = new int[513];
    for (int index = 0; index < 5; ++index)
      this.m_randomNumbers[index] = Random.Range(0, (int) byte.MaxValue);
    this.m_randomCount = 4;
    this.m_secondaryAnim = this.m_Secondary * 0.01f;
    this.m_smoothing = 0.0f;
    this.m_jointStep = 1f / (float) this.m_Bones.Count;
    this.m_timeSeed = (float) Random.Range(1, 100);
    this.m_seedX = (float) Random.Range(1, 10);
    this.m_seedY = (float) Random.Range(1, 10);
    this.m_seedZ = (float) Random.Range(1, 10);
    this.m_intensityValues = new float[this.m_Bones.Count];
    this.m_angleX = new float[this.m_Bones.Count];
    this.m_angleY = new float[this.m_Bones.Count];
    this.m_angleZ = new float[this.m_Bones.Count];
    this.m_velocityX = new float[this.m_Bones.Count];
    this.m_velocityY = new float[this.m_Bones.Count];
    this.m_velocityZ = new float[this.m_Bones.Count];
    this.m_lastX = new float[this.m_Bones.Count];
    this.m_lastY = new float[this.m_Bones.Count];
    this.m_lastZ = new float[this.m_Bones.Count];
    this.InitBones();
  }

  private void InitBones()
  {
    if (this.m_ControlBones.Count < this.m_Bones.Count)
    {
      this.m_orgRotation = new Quaternion[this.m_Bones.Count];
      for (int index = 0; index < this.m_Bones.Count; ++index)
        this.m_orgRotation[index] = this.m_Bones[index].rotation;
    }
    else
    {
      for (int index = 0; index < this.m_Bones.Count; ++index)
        this.m_Bones[index].rotation = this.m_ControlBones[index].rotation;
    }
    for (int index = 0; index < this.m_Bones.Count; ++index)
    {
      this.m_lastX[index] = this.m_Bones[index].eulerAngles.x;
      this.m_lastY[index] = this.m_Bones[index].eulerAngles.y;
      this.m_lastZ[index] = this.m_Bones[index].eulerAngles.z;
      this.m_velocityX[index] = 0.0f;
      this.m_velocityY[index] = 0.0f;
      this.m_velocityZ[index] = 0.0f;
      this.m_intensityValues[index] = this.m_IntensityCurve.Evaluate((float) index * this.m_jointStep);
    }
  }

  private void CalculateBoneAngles()
  {
    for (int index = 0; index < this.m_Bones.Count; ++index)
    {
      this.m_angleX[index] = this.CalculateAngle(index, this.m_lastX, this.m_velocityX, this.m_seedX) * this.m_X_Intensity;
      this.m_angleY[index] = this.CalculateAngle(index, this.m_lastY, this.m_velocityY, this.m_seedY) * this.m_Y_Intensity;
      this.m_angleZ[index] = this.CalculateAngle(index, this.m_lastZ, this.m_velocityZ, this.m_seedZ) * this.m_Z_Intensity;
    }
  }

  private float CalculateAngle(int index, float[] last, float[] velocity, float offset)
  {
    float target = this.Simplex1D((float) ((double) Time.timeSinceLevelLoad * (double) this.m_AnimSpeed + (double) this.m_timeSeed - (double) index * (double) this.m_secondaryAnim) + offset) * this.m_intensityValues[index] * this.m_MaxAngle;
    float currentVelocity = velocity[index];
    float angle = Mathf.SmoothDamp(last[index], target, ref currentVelocity, this.m_smoothing);
    velocity[index] = currentVelocity;
    last[index] = angle;
    return angle;
  }

  private float Simplex1D(float x)
  {
    int num1 = (int) Mathf.Floor(x);
    int num2 = num1 + 1;
    float x1 = x - (float) num1;
    float x2 = x1 - 1f;
    return (float) (((double) Mathf.Pow((float) (1.0 - (double) x1 * (double) x1), 4f) * (double) this.Interpolate(this.GetRandomNumber(num1 & (int) byte.MaxValue), x1) + (double) (Mathf.Pow((float) (1.0 - (double) x2 * (double) x2), 4f) * this.Interpolate(this.GetRandomNumber(num2 & (int) byte.MaxValue), x2))) * 0.395000010728836);
  }

  private float Interpolate(int h, float x)
  {
    h &= 15;
    float num = 1f + (float) (h & 7);
    return (h & 8) != 0 ? -num * x : num * x;
  }

  private int GetRandomNumber(int index)
  {
    if (index > this.m_randomCount)
    {
      for (int index1 = this.m_randomCount + 1; index1 <= index + 1; ++index1)
        this.m_randomNumbers[index1] = Random.Range(0, (int) byte.MaxValue);
      this.m_randomCount = index + 1;
    }
    return this.m_randomNumbers[index];
  }
}
