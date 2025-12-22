using UnityEngine;

public class Spinner : MonoBehaviour
{
  public float SpeedX;
  public float SpeedY;

  public void Update()
  {
    this.transform.Rotate(Vector3.right, Time.deltaTime * this.SpeedX);
    this.transform.Rotate(Vector3.up, Time.deltaTime * this.SpeedY, Space.World);
  }
}
