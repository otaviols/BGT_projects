using UnityEngine;

public class CameraSelector : MonoBehaviour
{
  public Vector3 cameraPosition;
  public Vector3 cameraRotation;
  public bool activateOnStart;

  private void Start()
  {
    if (!this.activateOnStart)
      return;
    Camera.main.transform.rotation = Quaternion.Euler(this.cameraRotation);
    Camera.main.transform.position = this.cameraPosition;
  }

  private void OnMouseDown()
  {
    Camera.main.transform.rotation = Quaternion.Euler(this.cameraRotation);
    Camera.main.transform.position = this.cameraPosition;
  }
}
