using UnityEngine;

public class ArcEnd : MonoBehaviour
{
  private Vector3 s;
  public Light l;

  private void Start() => this.s = this.transform.localScale;

  private void FixedUpdate()
  {
    this.transform.rotation = Quaternion.LookRotation(Vector3.up, Camera.main.transform.position - this.transform.position);
    this.transform.Rotate(Vector3.up, Random.value * 360f);
    if ((double) Random.value > 0.800000011920929)
    {
      this.transform.localScale = this.s * 1.5f;
      if (!((Object) this.l != (Object) null))
        return;
      this.l.range = 100f;
      this.l.intensity = 1.5f;
    }
    else
    {
      this.transform.localScale = this.s;
      if (!((Object) this.l != (Object) null))
        return;
      this.l.range = 50f;
      this.l.intensity = 1f;
    }
  }
}
