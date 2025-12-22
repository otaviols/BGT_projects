using System;
using UnityEngine;

public class CustomVisualReward : MonoBehaviour
{
  private Action m_callback;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public virtual void Start()
  {
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
  }

  public void SetCompleteCallback(Action c) => this.m_callback = c;

  public void Complete()
  {
    if (this.m_callback != null)
      this.m_callback();
    this.m_screenEffectsHandle.StopEffect();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }
}
