using UnityEngine;

public class AdventureUITestScene : MonoBehaviour
{
  private void Start() => PegUI.Get().AddInputCamera(Box.Get().m_Camera.GetComponent<Camera>());

  private void OnDestroy()
  {
    if (!((Object) PegUI.Get() != (Object) null))
      return;
    PegUI.Get().RemoveInputCamera(Box.Get().m_Camera.GetComponent<Camera>());
  }
}
