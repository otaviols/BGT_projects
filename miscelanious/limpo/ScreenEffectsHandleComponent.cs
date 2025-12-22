using UnityEngine;

public class ScreenEffectsHandleComponent : MonoBehaviour
{
  public ScreenEffectsHandle Handle;

  private void Awake() => this.Handle = new ScreenEffectsHandle((object) this);

  public void StartEffect(ScreenEffectParameters parameters) => this.Handle.StartEffect(parameters);

  public void StopEffect(ScreenEffectParameters? parameters)
  {
    if (parameters.HasValue)
      this.Handle.StopEffect(parameters.Value.Time, parameters.Value.EaseType);
    else
      this.Handle.StopEffect();
  }
}
