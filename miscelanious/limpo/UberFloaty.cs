using UnityEngine;

public class UberFloaty : MonoBehaviour
{
  public float speed = 1f;
  public float positionBlend = 1f;
  public float frequencyMin = 1f;
  public float frequencyMax = 3f;
  public bool localSpace = true;
  public Vector3 magnitude = new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f);
  public float rotationBlend = 1f;
  public float frequencyMinRot = 1f;
  public float frequencyMaxRot = 3f;
  public Vector3 magnitudeRot = new Vector3(0.0f, 0.0f, 0.0f);
  private Vector3 m_interval;
  private Vector3 m_offset;
  private Vector3 m_rotationInterval;
  private Vector3 m_startingPosition;
  private Vector3 m_startingRotation;

  private void Start() => this.Init();

  private void OnEnable() => this.InitTransforms();

  private void Update()
  {
    float num = Time.time * this.speed;
    Vector3 vector3_1;
    vector3_1.x = Mathf.Sin(num * this.m_interval.x + this.m_offset.x) * this.magnitude.x * this.m_interval.x;
    vector3_1.y = Mathf.Sin(num * this.m_interval.y + this.m_offset.y) * this.magnitude.y * this.m_interval.y;
    vector3_1.z = Mathf.Sin(num * this.m_interval.z + this.m_offset.z) * this.magnitude.z * this.m_interval.z;
    Vector3 vector3_2 = Vector3.Lerp(this.m_startingPosition, this.m_startingPosition + vector3_1, this.positionBlend);
    if (this.localSpace)
      this.transform.localPosition = vector3_2;
    else
      this.transform.position = vector3_2;
    Vector3 vector3_3;
    vector3_3.x = Mathf.Sin(num * this.m_rotationInterval.x + this.m_offset.x) * this.magnitudeRot.x * this.m_rotationInterval.x;
    vector3_3.y = Mathf.Sin(num * this.m_rotationInterval.y + this.m_offset.y) * this.magnitudeRot.y * this.m_rotationInterval.y;
    vector3_3.z = Mathf.Sin(num * this.m_rotationInterval.z + this.m_offset.z) * this.magnitudeRot.z * this.m_rotationInterval.z;
    this.transform.eulerAngles = Vector3.Lerp(this.m_startingRotation, this.m_startingRotation + vector3_3, this.rotationBlend);
  }

  private void InitTransforms()
  {
    this.m_startingPosition = !this.localSpace ? this.transform.position : this.transform.localPosition;
    this.m_startingRotation = this.transform.eulerAngles;
  }

  private void Init()
  {
    this.InitTransforms();
    this.m_interval.x = Random.Range(this.frequencyMin, this.frequencyMax);
    this.m_interval.y = Random.Range(this.frequencyMin, this.frequencyMax);
    this.m_interval.z = Random.Range(this.frequencyMin, this.frequencyMax);
    this.m_offset.x = 0.5f * Random.Range(-this.m_interval.x, this.m_interval.x);
    this.m_offset.y = 0.5f * Random.Range(-this.m_interval.y, this.m_interval.y);
    this.m_offset.z = 0.5f * Random.Range(-this.m_interval.z, this.m_interval.z);
    this.m_rotationInterval.x = Random.Range(this.frequencyMinRot, this.frequencyMaxRot);
    this.m_rotationInterval.y = Random.Range(this.frequencyMinRot, this.frequencyMaxRot);
    this.m_rotationInterval.z = Random.Range(this.frequencyMinRot, this.frequencyMaxRot);
  }
}
