using Hearthstone.UI.Core;
using UnityEngine;

public class floatyObj : MonoBehaviour
{
  public float frequencyMin = 0.0001f;
  public float frequencyMax = 1f / 1000f;
  public float magnitude = 0.0001f;
  private float m_interval;

  [Overridable]
  public bool Enabled
  {
    get => this.enabled;
    set => this.enabled = value;
  }

  private void Start() => this.m_interval = Random.Range(this.frequencyMin, this.frequencyMax);

  private void Update()
  {
    float num = Mathf.Sin(Time.time * this.m_interval) * this.magnitude;
    Vector3 vector3 = new Vector3(num, num, num);
    this.transform.position += vector3;
    this.transform.eulerAngles += vector3;
  }
}
