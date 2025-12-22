using UnityEngine;

public abstract class PegUICustomBehavior : MonoBehaviour
{
  protected virtual void Awake() => PegUI.Get().RegisterCustomBehavior(this);

  protected virtual void OnDestroy()
  {
    if (!((Object) PegUI.Get() != (Object) null))
      return;
    PegUI.Get().UnregisterCustomBehavior(this);
  }

  public abstract bool UpdateUI();
}
