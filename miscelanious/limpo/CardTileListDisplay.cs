using System;
using UnityEngine;

public class CardTileListDisplay : MonoBehaviour
{
  public SoundDucker m_SoundDucker;
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected virtual void Awake() => this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);

  protected virtual void Start()
  {
  }

  protected virtual void OnDestroy()
  {
  }

  protected void AnimateVignetteIn() => this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.VignetteDesaturatePerspective);

  protected void AnimateVignetteOut() => this.m_screenEffectsHandle.StopEffect(0.1f, new Action(this.OnFullScreenEffectOutFinished));

  protected void AnimateBlurVignetteIn() => this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);

  protected void AnimateBlurVignetteOut() => this.m_screenEffectsHandle.StopEffect(0.1f, new Action(this.OnFullScreenEffectOutFinished));

  protected virtual void OnFullScreenEffectOutFinished()
  {
  }
}
