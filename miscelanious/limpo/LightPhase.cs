using UnityEngine;

public class LightPhase : MonoBehaviour
{
  public float duration = 1f;
  public float minPower = 3f;
  public float maxPower = 8f;
  public float speed = 0.01f;
  private float targetIntensity;
  private float lastTargetTimestamp;
  private float timeToWaitForNewTarget = 1f;

  public void Update()
  {
    float time = Time.time;
    if ((double) time - (double) this.lastTargetTimestamp > (double) this.timeToWaitForNewTarget)
    {
      this.targetIntensity = Random.Range(this.minPower, this.maxPower);
      this.lastTargetTimestamp = time;
    }
    Light component = this.GetComponent<Light>();
    double f = (double) this.targetIntensity - (double) component.intensity;
    float num = (float) f / Mathf.Abs((float) f);
    if ((double) component.intensity == (double) this.targetIntensity)
      return;
    component.intensity += num * this.speed;
  }
}
