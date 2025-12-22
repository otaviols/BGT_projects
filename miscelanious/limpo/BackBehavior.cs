using UnityEngine;

public class BackBehavior : MonoBehaviour
{
  public void Awake()
  {
    PegUIElement component = this.gameObject.GetComponent<PegUIElement>();
    if (!((Object) component != (Object) null))
      return;
    component.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnRelease()));
  }

  public void OnRelease() => Navigation.GoBack();
}
