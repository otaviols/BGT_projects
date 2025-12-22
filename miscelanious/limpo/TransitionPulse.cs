using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class TransitionPulse : MonoBehaviour
{
  public float frequencyMin = 0.0001f;
  public float frequencyMax = 1f;
  public float magnitude = 0.0001f;
  private float m_interval;

  private void Start() => this.m_interval = Random.Range(this.frequencyMin, this.frequencyMax);

  private void Update()
  {
    float num = Mathf.Sin(Time.time * this.m_interval) * this.magnitude;
    this.gameObject.GetComponent<Renderer>().GetMaterial().SetFloat("_Transistion", num);
  }
}
