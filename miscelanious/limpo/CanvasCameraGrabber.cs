using UnityEngine;

public class CanvasCameraGrabber : MonoBehaviour
{
  private void Awake()
  {
    Canvas component = this.GetComponent<Canvas>();
    if (!(bool) (Object) component)
      return;
    component.worldCamera = CameraUtils.FindFirstByLayer(GameLayer.BattleNet);
  }
}
