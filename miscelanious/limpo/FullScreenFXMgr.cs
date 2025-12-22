using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class FullScreenFXMgr : IHasLateUpdate, IService
{
  private FullScreenEffects m_ActiveCameraFullScreenEffects;
  private FullScreenEffects m_SecondaryCameraFullScreenEffects;
  private List<FullScreenFXMgr.ScreenEffectsInstance> m_screenEffects = new List<FullScreenFXMgr.ScreenEffectsInstance>(10);
  private bool m_stackDirty;
  private ProfilerMarker m_updateProfilerMarker;
  private ProfilerMarker m_resolveProfilerMarker;
  private ProfilerMarker m_addEffectProfilerMarker;
  private ProfilerMarker m_releaseEffectProfilerMarker;

  public FullScreenEffects ActiveCameraFullScreenEffects
  {
    get
    {
      if ((UnityEngine.Object) this.m_ActiveCameraFullScreenEffects == (UnityEngine.Object) null)
      {
        Camera mainCamera = CameraUtils.GetMainCamera();
        if ((UnityEngine.Object) mainCamera == (UnityEngine.Object) null)
        {
          Log.FullScreenFX.PrintError("Could not find Box Camera");
          return (FullScreenEffects) null;
        }
        FullScreenEffects component;
        if (!mainCamera.TryGetComponent<FullScreenEffects>(out component))
        {
          Log.FullScreenFX.PrintError("Could not find Perspective/Active FullScreen Effects component");
          return (FullScreenEffects) null;
        }
        this.m_ActiveCameraFullScreenEffects = component;
      }
      return this.m_ActiveCameraFullScreenEffects;
    }
  }

  public FullScreenEffects SecondaryCameraFullScreenEffects
  {
    get
    {
      if ((UnityEngine.Object) this.m_SecondaryCameraFullScreenEffects == (UnityEngine.Object) null)
      {
        Camera firstByLayer = CameraUtils.FindFirstByLayer(GameLayer.BattleNet);
        if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null)
        {
          Log.FullScreenFX.PrintError("Could not find secondary camera");
          return (FullScreenEffects) null;
        }
        FullScreenEffects component;
        if (!firstByLayer.TryGetComponent<FullScreenEffects>(out component))
        {
          Log.FullScreenFX.PrintError("Could not find Orthographic/Secondary FullScreen Effects component");
          return (FullScreenEffects) null;
        }
        this.m_SecondaryCameraFullScreenEffects = component;
      }
      return this.m_SecondaryCameraFullScreenEffects;
    }
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    FullScreenFXMgr fullScreenFxMgr = this;
    fullScreenFxMgr.m_updateProfilerMarker = new ProfilerMarker("FullscreenFXMgr.LateUpdate");
    fullScreenFxMgr.m_resolveProfilerMarker = new ProfilerMarker("FullscreenFXMgr.ResolveEffects");
    fullScreenFxMgr.m_addEffectProfilerMarker = new ProfilerMarker("FullscreenFXMgr.AddEffect");
    fullScreenFxMgr.m_releaseEffectProfilerMarker = new ProfilerMarker("FullscreenFXMgr.ReleaseEffect");
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().WillReset += new Action(fullScreenFxMgr.OnHSReset);
    yield return (IAsyncJobResult) new ServiceSoftDependency(typeof (SceneMgr), serviceLocator);
    SceneMgr service;
    if (serviceLocator.TryGetService<SceneMgr>(out service))
      service.RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(fullScreenFxMgr.OnScenePreLoad));
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().WillReset -= new Action(this.OnHSReset);
    SceneMgr service;
    if (!ServiceManager.TryGet<SceneMgr>(out service))
      return;
    service.UnregisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(this.OnScenePreLoad));
  }

  public static FullScreenFXMgr Get() => ServiceManager.Get<FullScreenFXMgr>();

  public void LateUpdate()
  {
    using (this.m_updateProfilerMarker.Auto())
    {
      for (int index = this.m_screenEffects.Count - 1; index >= 0; --index)
      {
        if (!this.m_screenEffects[index].Released && (this.m_screenEffects[index].Owner == null || (object) (this.m_screenEffects[index].Owner as UnityEngine.Object) != null && (UnityEngine.Object) this.m_screenEffects[index].Owner == (UnityEngine.Object) null))
          this.StopEffect(this.m_screenEffects[index], (UnityEngine.Object) this.m_screenEffects[index].EffectsComponent == (UnityEngine.Object) null);
        if (this.m_screenEffects.Count > 1 && this.m_screenEffects[index].Released)
          this.m_stackDirty = true;
      }
      if (!this.m_stackDirty)
        return;
      this.ResolveEffects();
    }
  }

  public void AddEffect(
    ScreenEffectsHandle handle,
    ScreenEffectParameters effectParameters,
    Action onFinishedCallback = null)
  {
    using (this.m_addEffectProfilerMarker.Auto())
    {
      if (handle == null)
        Log.FullScreenFX.PrintError("Invalid handle passed in to AddEffect! You should store and pass in a valid reference.");
      else if (handle.Owner == null)
      {
        Log.FullScreenFX.PrintError("Handle has an invalid owner! This effect request will be ignored!");
      }
      else
      {
        if (!this.m_screenEffects.Contains(handle.ScreenEffectsInstance))
          this.m_screenEffects.Add(handle.ScreenEffectsInstance);
        handle.ScreenEffectsInstance.Initialize(effectParameters);
        handle.SetFinishedCallback(onFinishedCallback);
        this.m_stackDirty = true;
      }
    }
  }

  public void StopEffect(FullScreenFXMgr.ScreenEffectsInstance fxInstance, bool immediate = false)
  {
    using (this.m_releaseEffectProfilerMarker.Auto())
    {
      if (!this.m_screenEffects.Contains(fxInstance))
        Log.FullScreenFX.PrintWarning("Attempted to release a fullscreen effect that was not added!");
      else if (fxInstance.Released && !immediate)
      {
        Log.FullScreenFX.PrintWarning("Attempted to release a fullscreen effect that was already released!");
      }
      else
      {
        fxInstance.Parameters.Blur = BlurParameters.None;
        fxInstance.Parameters.Vignette = VignetteParameters.None;
        fxInstance.Parameters.Desaturate = DesaturateParameters.None;
        fxInstance.Parameters.BlendToColor = BlendToColorParameters.None;
        fxInstance.Released = true;
        if (immediate)
          this.RemoveFxInstanceFromStack(fxInstance);
        this.m_stackDirty = true;
      }
    }
  }

  private void RemoveFxInstanceFromStack(
    FullScreenFXMgr.ScreenEffectsInstance screenEffectsInstance)
  {
    if (!this.m_screenEffects.Contains(screenEffectsInstance))
      return;
    if ((UnityEngine.Object) screenEffectsInstance.EffectsComponent != (UnityEngine.Object) null && screenEffectsInstance.EffectsComponent.ActiveEffectsInstance == screenEffectsInstance)
      screenEffectsInstance.EffectsComponent.CleanupEffects(screenEffectsInstance.Parameters.Time);
    screenEffectsInstance.Reset();
    this.m_screenEffects.Remove(screenEffectsInstance);
    this.m_stackDirty = true;
  }

  private void ResolveEffects()
  {
    using (this.m_resolveProfilerMarker.Auto())
    {
      this.m_screenEffects.Sort();
      if (this.m_screenEffects.Count > 1)
      {
        for (int index = this.m_screenEffects.Count - 1; index >= 0; --index)
        {
          if (this.m_screenEffects[index].Released)
            this.RemoveFxInstanceFromStack(this.m_screenEffects[index]);
        }
      }
      if (this.m_screenEffects.Count == 0)
      {
        this.m_stackDirty = false;
      }
      else
      {
        FullScreenFXMgr.ScreenEffectsInstance screenEffect = this.m_screenEffects[this.m_screenEffects.Count - 1];
        if (screenEffect == null)
        {
          Log.FullScreenFX.PrintError("Could not find a ScreenEffectsInstance!");
        }
        else
        {
          FullScreenEffects fullScreenEffects = screenEffect.Parameters.PassLocation == ScreenEffectPassLocation.PERSPECTIVE ? this.ActiveCameraFullScreenEffects : this.SecondaryCameraFullScreenEffects;
          if ((UnityEngine.Object) fullScreenEffects == (UnityEngine.Object) null)
          {
            Log.FullScreenFX.PrintError("Could not find a FullScreenEffects component!");
          }
          else
          {
            screenEffect.EffectsComponent = fullScreenEffects;
            fullScreenEffects.StartEffect(screenEffect);
            this.m_stackDirty = false;
            this.UpdateInputManager();
          }
        }
      }
    }
  }

  public void OnFinishedEffect(
    FullScreenFXMgr.ScreenEffectsInstance screenEffectsInstance)
  {
    if (!screenEffectsInstance.Released)
      return;
    this.RemoveFxInstanceFromStack(screenEffectsInstance);
  }

  public void ForceReset()
  {
    if (!((UnityEngine.Object) this.ActiveCameraFullScreenEffects != (UnityEngine.Object) null))
      return;
    this.ActiveCameraFullScreenEffects.Disable();
  }

  private void OnHSReset()
  {
    for (int index = this.m_screenEffects.Count - 1; index >= 0; --index)
      this.StopEffect(this.m_screenEffects[index]);
    this.ForceReset();
  }

  private void OnScenePreLoad(SceneMgr.Mode prevMode, SceneMgr.Mode nextMode, object userData)
  {
    if (prevMode != SceneMgr.Mode.GAMEPLAY || nextMode == SceneMgr.Mode.HUB)
      return;
    this.StopAllEffects();
  }

  public void StopAllEffects(float delay = 0.0f)
  {
    FullScreenEffects fullScreenEffects = this.ActiveCameraFullScreenEffects;
    if ((UnityEngine.Object) fullScreenEffects == (UnityEngine.Object) null || !fullScreenEffects.IsActive)
      return;
    Log.FullScreenFX.Print(nameof (StopAllEffects));
    Processor.RunCoroutine(this.StopAllEffectsCoroutine(fullScreenEffects, delay));
  }

  private IEnumerator StopAllEffectsCoroutine(FullScreenEffects effects, float delay)
  {
    float stopEffectsTime = 0.25f;
    if ((double) delay > 0.0)
      yield return (object) new WaitForSeconds(delay);
    Log.FullScreenFX.Print("StopAllEffectsCoroutine stopping effects now");
    foreach (FullScreenFXMgr.ScreenEffectsInstance screenEffect in this.m_screenEffects)
      this.StopEffect(screenEffect);
    yield return (object) new WaitForSeconds(stopEffectsTime);
    if (!((UnityEngine.Object) effects == (UnityEngine.Object) null))
      effects.Disable();
  }

  private void UpdateInputManager()
  {
    if (UniversalInputManager.Get() == null)
      return;
    FullScreenEffects highestActiveEffect = this.GetHighestActiveEffect();
    UniversalInputManager.Get().SetCurrentFullScreenEffect(highestActiveEffect);
  }

  private FullScreenEffects GetHighestActiveEffect()
  {
    if (this.m_screenEffects.Count > 0)
    {
      for (int index = 0; index < this.m_screenEffects.Count; ++index)
      {
        FullScreenFXMgr.ScreenEffectsInstance screenEffect = this.m_screenEffects[index];
        if (!((UnityEngine.Object) screenEffect.EffectsComponent == (UnityEngine.Object) null) && screenEffect.EffectsComponent.HasActiveEffects)
          return screenEffect.EffectsComponent;
      }
    }
    return (UnityEngine.Object) this.ActiveCameraFullScreenEffects != (UnityEngine.Object) null && this.ActiveCameraFullScreenEffects.HasActiveEffects ? this.ActiveCameraFullScreenEffects : (FullScreenEffects) null;
  }

  public class ScreenEffectsInstance : IComparable
  {
    public object Owner;
    public FullScreenEffects EffectsComponent;
    public ScreenEffectParameters Parameters;
    public Action OnFinishedCallback;
    public bool Released;

    public Camera Camera => !((UnityEngine.Object) this.EffectsComponent != (UnityEngine.Object) null) ? (Camera) null : this.EffectsComponent.Camera;

    public ScreenEffectsInstance(object owner)
    {
      this.Owner = owner;
      this.EffectsComponent = (FullScreenEffects) null;
      this.Parameters = ScreenEffectParameters.None;
    }

    public void Initialize(ScreenEffectParameters effectParameters)
    {
      this.Parameters = effectParameters;
      this.Released = false;
    }

    public void Reset()
    {
      this.Parameters = ScreenEffectParameters.None;
      this.EffectsComponent = (FullScreenEffects) null;
      this.Released = false;
    }

    public int CompareTo(object obj)
    {
      if (!(obj is FullScreenFXMgr.ScreenEffectsInstance screenEffectsInstance))
        return -1;
      if ((UnityEngine.Object) this.Camera == (UnityEngine.Object) null && (UnityEngine.Object) screenEffectsInstance.Camera == (UnityEngine.Object) null)
        return 0;
      if ((UnityEngine.Object) this.Camera != (UnityEngine.Object) null && (UnityEngine.Object) screenEffectsInstance.Camera == (UnityEngine.Object) null)
        return -1;
      if ((UnityEngine.Object) this.Camera == (UnityEngine.Object) null && (UnityEngine.Object) screenEffectsInstance.Camera != (UnityEngine.Object) null)
        return 1;
      if ((double) this.Camera.depth > (double) screenEffectsInstance.Camera.depth)
        return -1;
      return (double) this.Camera.depth < (double) screenEffectsInstance.Camera.depth ? 1 : 0;
    }
  }
}
