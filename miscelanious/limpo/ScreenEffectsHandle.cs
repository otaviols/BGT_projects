using System;

public class ScreenEffectsHandle
{
  public object Owner;
  private FullScreenFXMgr.ScreenEffectsInstance m_fxInstance;
  private bool m_isSet;

  public FullScreenFXMgr.ScreenEffectsInstance ScreenEffectsInstance => this.m_fxInstance;

  public ScreenEffectsHandle(object owner)
  {
    this.Owner = owner;
    this.m_fxInstance = new FullScreenFXMgr.ScreenEffectsInstance(owner);
    this.m_isSet = false;
  }

  ~ScreenEffectsHandle() => this.StopEffect();

  public void StartEffect(ScreenEffectParameters parameters, Action onFinishedCallback = null)
  {
    FullScreenFXMgr fullScreenFxMgr = FullScreenFXMgr.Get();
    if (fullScreenFxMgr == null)
    {
      Log.FullScreenFX.PrintError("FullscreenFXMgr is missing!");
    }
    else
    {
      fullScreenFxMgr.AddEffect(this, parameters, onFinishedCallback);
      this.m_isSet = true;
    }
  }

  public void StopEffect(Action callback = null)
  {
    if (this.HasBeenResetOrReleased())
      return;
    FullScreenFXMgr fullScreenFxMgr = FullScreenFXMgr.Get();
    if (fullScreenFxMgr == null)
    {
      Log.FullScreenFX.PrintError("FullscreenFXMgr is missing!");
    }
    else
    {
      this.m_fxInstance.OnFinishedCallback = callback;
      fullScreenFxMgr.StopEffect(this.m_fxInstance, this.m_fxInstance == null);
      this.m_isSet = false;
    }
  }

  public void StopEffect(float time, iTween.EaseType easeType, Action callback = null)
  {
    if (this.HasBeenResetOrReleased())
      return;
    this.m_fxInstance.Parameters.Time = time;
    this.m_fxInstance.Parameters.EaseType = easeType;
    this.StopEffect(callback);
  }

  public void StopEffect(float time, Action callback = null)
  {
    if (this.HasBeenResetOrReleased())
      return;
    this.StopEffect(time, this.m_fxInstance.Parameters.EaseType, callback);
  }

  public void SetFinishedCallback(Action onFinishedCallback) => this.m_fxInstance.OnFinishedCallback = onFinishedCallback;

  public void ClearCallbacks()
  {
    if (this.m_fxInstance == null)
      return;
    this.m_fxInstance.OnFinishedCallback = (Action) null;
  }

  private bool HasBeenResetOrReleased() => !this.m_isSet || this.m_fxInstance.Released;
}
