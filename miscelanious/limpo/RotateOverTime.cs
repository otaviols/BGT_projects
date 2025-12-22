using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
  public float RotateSpeedX;
  public float RotateSpeedY;
  public float RotateSpeedZ;
  public bool RandomStartX;
  public bool RandomStartY;
  public bool RandomStartZ;

  private void Start()
  {
    if (this.RandomStartX)
      this.transform.Rotate(Vector3.left, (float) Random.Range(0, 360));
    if (this.RandomStartY)
      this.transform.Rotate(Vector3.up, (float) Random.Range(0, 360));
    if (!this.RandomStartZ)
      return;
    this.transform.Rotate(Vector3.forward, (float) Random.Range(0, 360));
  }

  private void Update()
  {
    this.transform.Rotate(Vector3.left, Time.deltaTime * this.RotateSpeedX, Space.Self);
    this.transform.Rotate(Vector3.up, Time.deltaTime * this.RotateSpeedY, Space.Self);
    this.transform.Rotate(Vector3.forward, Time.deltaTime * this.RotateSpeedZ, Space.Self);
  }
}
