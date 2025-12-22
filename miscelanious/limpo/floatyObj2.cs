using UnityEngine;

public class floatyObj2 : MonoBehaviour
{
  public float frequencyMin = 0.0001f;
  public float frequencyMax = 1f / 1000f;
  public float magnitude = 0.0001f;
  public float frequencyMinRot = 0.0001f;
  public float frequencyMaxRot = 1f / 1000f;
  public float magnitudeRot;
  private float m_interval;
  private float m_rotationInterval;

  private void Start()
  {
    this.m_interval = Random.Range(this.frequencyMin, this.frequencyMax);
    this.m_rotationInterval = Random.Range(this.frequencyMinRot, this.frequencyMaxRot);
  }

  private void Update()
  {
    float num1 = Mathf.Sin(Time.time * this.m_interval) * this.magnitude;
    this.transform.position += new Vector3(num1, num1, num1);
    float num2 = Mathf.Sin(Time.time * this.m_rotationInterval) * this.magnitudeRot;
    this.transform.eulerAngles += new Vector3(num2, num2, num2);
  }
}
