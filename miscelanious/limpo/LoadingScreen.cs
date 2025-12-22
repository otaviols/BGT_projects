using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
  public float m_FadeOutSec = 1f;
  public iTween.EaseType m_FadeOutEaseType = iTween.EaseType.linear;
  public float m_FadeInSec = 1f;
  public iTween.EaseType m_FadeInEaseType = iTween.EaseType.linear;
  private const float MIDDLE_OF_NOWHERE_X = 5000f;
  private LoadingScreen.Phase m_phase;
  private bool m_previousSceneActive;
  private LoadingScreen.TransitionParams m_prevTransitionParams;
  private LoadingScreen.TransitionParams m_transitionParams = new LoadingScreen.TransitionParams();
  private LoadingScreen.TransitionUnfriendlyData m_transitionUnfriendlyData = new LoadingScreen.TransitionUnfriendlyData();
  private Camera m_fxCamera;
  private List<LoadingScreen.PreviousSceneDestroyedListener> m_prevSceneDestroyedListeners = new List<LoadingScreen.PreviousSceneDestroyedListener>();
  private List<LoadingScreen.FinishedTransitionListener> m_finishedTransitionListeners = new List<LoadingScreen.FinishedTransitionListener>();
  private float m_originalPosX;
  private long m_assetLoadStartTimestamp;
  private long m_assetLoadEndTimestamp;
  private long m_assetLoadNextStartTimestamp;

  public event Action OnFadeInStart;

  private void Awake() => this.InitializeFxCamera();

  public static LoadingScreen Get()
  {
    SceneMgr service;
    return ServiceManager.TryGet<SceneMgr>(out service) ? service.LoadingScreen : (LoadingScreen) null;
  }

  public Camera GetFxCamera() => this.m_fxCamera;

  public CameraFade GetCameraFade() => this.GetComponent<CameraFade>();

  public void RegisterSceneListeners(SceneMgr sceneMgr)
  {
    sceneMgr.RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    sceneMgr.RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
    sceneMgr.RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
  }

  public void UnregisterSceneListeners(SceneMgr sceneMgr)
  {
    sceneMgr.UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    sceneMgr.UnregisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
    sceneMgr.UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
  }

  public static bool DoesShowLoadingScreen(SceneMgr.Mode prevMode, SceneMgr.Mode nextMode) => prevMode == SceneMgr.Mode.GAMEPLAY || nextMode == SceneMgr.Mode.GAMEPLAY;

  public LoadingScreen.Phase GetPhase() => this.m_phase;

  public bool IsTransitioning() => this.m_phase != 0;

  public bool IsWaiting()
  {
    switch (this.m_phase)
    {
      case LoadingScreen.Phase.WAITING_FOR_SCENE_UNLOAD:
      case LoadingScreen.Phase.WAITING_FOR_SCENE_LOAD:
      case LoadingScreen.Phase.WAITING_FOR_BLOCKERS:
        return true;
      default:
        return false;
    }
  }

  public bool IsFadingOut() => this.m_phase == LoadingScreen.Phase.FADING_OUT;

  public bool IsFadingIn() => this.m_phase == LoadingScreen.Phase.FADING_IN;

  public bool IsFading() => this.IsFadingOut() || this.IsFadingIn();

  public bool IsPreviousSceneActive() => this.m_previousSceneActive;

  public void AddTransitionObject(GameObject go) => this.m_transitionParams.AddObject(go);

  public void AddTransitionObject(Component c) => this.m_transitionParams.AddObject(c.gameObject);

  public void AddTransitionBlocker() => this.m_transitionParams.AddBlocker();

  public void AddTransitionBlocker(int count) => this.m_transitionParams.AddBlocker(count);

  public Camera GetFreezeFrameCamera() => this.m_transitionParams.GetFreezeFrameCamera();

  public void SetFreezeFrameCamera(Camera camera) => this.m_transitionParams.SetFreezeFrameCamera(camera);

  public AudioListener GetTransitionAudioListener() => this.m_transitionParams.GetAudioListener();

  public void SetTransitionAudioListener(AudioListener listener)
  {
    Log.LoadingScreen.Print("LoadingScreen.SetTransitionAudioListener() - {0}", (object) listener);
    this.m_transitionParams.SetAudioListener(listener);
  }

  public void EnableFadeOut(bool enable) => this.m_transitionParams.EnableFadeOut(enable);

  public void EnableFadeIn(bool enable) => this.m_transitionParams.EnableFadeIn(enable);

  public Color GetFadeColor() => this.m_transitionParams.GetFadeColor();

  public void SetFadeColor(Color color) => this.m_transitionParams.SetFadeColor(color);

  public void NotifyTransitionBlockerComplete()
  {
    if (this.m_prevTransitionParams == null)
      return;
    this.m_prevTransitionParams.RemoveBlocker();
    this.TransitionIfPossible();
  }

  public void NotifyTransitionBlockerComplete(int count)
  {
    if (this.m_prevTransitionParams == null)
      return;
    this.m_prevTransitionParams.RemoveBlocker(count);
    this.TransitionIfPossible();
  }

  public void NotifyMainSceneObjectAwoke(GameObject mainObject)
  {
    if (!this.IsPreviousSceneActive())
      return;
    this.DisableTransitionUnfriendlyStuff(mainObject);
  }

  public long GetAssetLoadStartTimestamp() => this.m_assetLoadStartTimestamp;

  public void SetAssetLoadStartTimestamp(long timestamp)
  {
    this.m_assetLoadStartTimestamp = Math.Min(this.m_assetLoadStartTimestamp, timestamp);
    Log.LoadingScreen.Print("LoadingScreen.SetAssetLoadStartTimestamp() - m_assetLoadStartTimestamp={0}", (object) this.m_assetLoadStartTimestamp);
  }

  public bool RegisterPreviousSceneDestroyedListener(
    LoadingScreen.PreviousSceneDestroyedCallback callback)
  {
    return this.RegisterPreviousSceneDestroyedListener(callback, (object) null);
  }

  public bool RegisterPreviousSceneDestroyedListener(
    LoadingScreen.PreviousSceneDestroyedCallback callback,
    object userData)
  {
    LoadingScreen.PreviousSceneDestroyedListener destroyedListener = new LoadingScreen.PreviousSceneDestroyedListener();
    destroyedListener.SetCallback(callback);
    destroyedListener.SetUserData(userData);
    if (this.m_prevSceneDestroyedListeners.Contains(destroyedListener))
      return false;
    this.m_prevSceneDestroyedListeners.Add(destroyedListener);
    return true;
  }

  public bool UnregisterPreviousSceneDestroyedListener(
    LoadingScreen.PreviousSceneDestroyedCallback callback)
  {
    return this.UnregisterPreviousSceneDestroyedListener(callback, (object) null);
  }

  public bool UnregisterPreviousSceneDestroyedListener(
    LoadingScreen.PreviousSceneDestroyedCallback callback,
    object userData)
  {
    LoadingScreen.PreviousSceneDestroyedListener destroyedListener = new LoadingScreen.PreviousSceneDestroyedListener();
    destroyedListener.SetCallback(callback);
    destroyedListener.SetUserData(userData);
    return this.m_prevSceneDestroyedListeners.Remove(destroyedListener);
  }

  public bool RegisterFinishedTransitionListener(LoadingScreen.FinishedTransitionCallback callback) => this.RegisterFinishedTransitionListener(callback, (object) null);

  public bool RegisterFinishedTransitionListener(
    LoadingScreen.FinishedTransitionCallback callback,
    object userData)
  {
    LoadingScreen.FinishedTransitionListener transitionListener = new LoadingScreen.FinishedTransitionListener();
    transitionListener.SetCallback(callback);
    transitionListener.SetUserData(userData);
    if (this.m_finishedTransitionListeners.Contains(transitionListener))
      return false;
    this.m_finishedTransitionListeners.Add(transitionListener);
    return true;
  }

  public bool UnregisterFinishedTransitionListener(LoadingScreen.FinishedTransitionCallback callback) => this.UnregisterFinishedTransitionListener(callback, (object) null);

  public bool UnregisterFinishedTransitionListener(
    LoadingScreen.FinishedTransitionCallback callback,
    object userData)
  {
    LoadingScreen.FinishedTransitionListener transitionListener = new LoadingScreen.FinishedTransitionListener();
    transitionListener.SetCallback(callback);
    transitionListener.SetUserData(userData);
    return this.m_finishedTransitionListeners.Remove(transitionListener);
  }

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    Log.LoadingScreen.Print("LoadingScreen.OnScenePreUnload() - prevMode={0} nextMode={1} m_phase={2}", (object) prevMode, (object) SceneMgr.Get().GetMode(), (object) this.m_phase);
    if (!LoadingScreen.DoesShowLoadingScreen(prevMode, SceneMgr.Get().GetMode()))
    {
      this.CutoffTransition();
    }
    else
    {
      if (this.IsTransitioning())
        this.DoInterruptionCleanUp();
      this.m_assetLoadNextStartTimestamp = TimeUtils.BinaryStamp();
      if (this.IsTransitioning())
      {
        this.FireFinishedTransitionListeners(true);
        if (this.IsPreviousSceneActive())
          return;
      }
      this.m_phase = LoadingScreen.Phase.WAITING_FOR_SCENE_UNLOAD;
      this.m_previousSceneActive = true;
      this.ShowFreezeFrame(this.m_transitionParams.GetFreezeFrameCamera());
    }
  }

  private void OnSceneUnloaded(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    Log.LoadingScreen.Print("LoadingScreen.OnSceneUnloaded() - prevMode={0} nextMode={1} m_phase={2}", (object) prevMode, (object) SceneMgr.Get().GetMode(), (object) this.m_phase);
    if (this.m_phase != LoadingScreen.Phase.WAITING_FOR_SCENE_UNLOAD)
      return;
    this.m_assetLoadEndTimestamp = this.m_assetLoadNextStartTimestamp;
    Log.LoadingScreen.Print("LoadingScreen.OnSceneUnloaded() - m_assetLoadEndTimestamp={0}", (object) this.m_assetLoadEndTimestamp);
    this.m_phase = LoadingScreen.Phase.WAITING_FOR_SCENE_LOAD;
    this.m_prevTransitionParams = this.m_transitionParams;
    this.m_transitionParams = new LoadingScreen.TransitionParams();
    this.m_transitionParams.ClearPreviousAssets = prevMode != SceneMgr.Get().GetMode();
    this.m_prevTransitionParams.AutoAddObjects();
    if ((UnityEngine.Object) this.m_fxCamera != (UnityEngine.Object) null)
    {
      float highestCameraDepth = this.m_prevTransitionParams.GetHighestCameraDepth();
      if ((double) highestCameraDepth >= (double) this.m_fxCamera.depth - 1.0)
      {
        Debug.LogWarning((object) "FX Camera's depth was less than or equal to previous transition's camera stack. Moving to a higher depth");
        this.m_fxCamera.depth = highestCameraDepth + 2f;
      }
      this.m_prevTransitionParams.FixCameraTagsAndDepths(this.m_fxCamera.depth);
    }
    this.m_prevTransitionParams.PreserveObjects(this.transform);
    this.m_originalPosX = this.transform.position.x;
    TransformUtil.SetPosX(this.gameObject, 5000f);
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    Log.LoadingScreen.Print("LoadingScreen.OnSceneLoaded() - prevMode={0} currMode={1}", (object) SceneMgr.Get().GetPrevMode(), (object) mode);
    if (mode == SceneMgr.Mode.FATAL_ERROR)
    {
      Log.LoadingScreen.Print("LoadingScreen.OnSceneLoaded() - calling CutoffTransition()", (object) mode);
      this.CutoffTransition();
    }
    else
    {
      if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.STARTUP)
      {
        this.m_assetLoadStartTimestamp = TimeUtils.BinaryStamp();
        Log.LoadingScreen.Print("LoadingScreen.OnSceneLoaded() - m_assetLoadStartTimestamp={0}", (object) this.m_assetLoadStartTimestamp);
      }
      if (this.m_phase != LoadingScreen.Phase.WAITING_FOR_SCENE_LOAD)
      {
        Log.LoadingScreen.Print("LoadingScreen.OnSceneLoaded() - END - {0} != Phase.WAITING_FOR_SCENE_LOAD", (object) this.m_phase);
      }
      else
      {
        this.m_phase = LoadingScreen.Phase.WAITING_FOR_BLOCKERS;
        this.TransitionIfPossible();
      }
    }
  }

  private bool TransitionIfPossible()
  {
    if (this.m_prevTransitionParams.GetBlockerCount() > 0)
      return false;
    this.StartCoroutine("HackWaitThenStartTransitionEffects");
    return true;
  }

  private IEnumerator HackWaitThenStartTransitionEffects()
  {
    Log.LoadingScreen.Print("LoadingScreen.HackWaitThenStartTransitionEffects() - START");
    yield return (object) new WaitForEndOfFrame();
    if (this.m_phase != LoadingScreen.Phase.WAITING_FOR_BLOCKERS)
      Log.LoadingScreen.Print("LoadingScreen.HackWaitThenStartTransitionEffects() - END - {0} != Phase.WAITING_FOR_BLOCKERS", (object) this.m_phase);
    else
      this.FadeOut();
  }

  private void FirePreviousSceneDestroyedListeners()
  {
    foreach (LoadingScreen.PreviousSceneDestroyedListener destroyedListener in this.m_prevSceneDestroyedListeners.ToArray())
      destroyedListener.Fire();
  }

  private void FireFinishedTransitionListeners(bool cutoff)
  {
    foreach (LoadingScreen.FinishedTransitionListener transitionListener in this.m_finishedTransitionListeners.ToArray())
      transitionListener.Fire(cutoff);
  }

  private void FadeOut()
  {
    Log.LoadingScreen.Print("LoadingScreen.FadeOut()");
    this.m_phase = LoadingScreen.Phase.FADING_OUT;
    if (!this.m_prevTransitionParams.IsFadeOutEnabled())
    {
      this.OnFadeOutComplete();
    }
    else
    {
      CameraFade cameraFade = this.GetComponent<CameraFade>();
      if ((UnityEngine.Object) cameraFade == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "LoadingScreen FadeOut(): Failed to find CameraFade component");
      }
      else
      {
        cameraFade.m_Color = this.m_prevTransitionParams.GetFadeColor();
        Action<object> action = (Action<object>) (amount => cameraFade.m_Fade = (float) amount);
        iTween.ValueTo(this.gameObject, iTween.Hash((object) "time", (object) this.m_FadeOutSec, (object) "from", (object) cameraFade.m_Fade, (object) "to", (object) 1f, (object) "onupdate", (object) action, (object) "onupdatetarget", (object) this.gameObject, (object) "oncomplete", (object) "OnFadeOutComplete", (object) "oncompletetarget", (object) this.gameObject, (object) "name", (object) "Fade"));
      }
    }
  }

  private void OnFadeOutComplete()
  {
    Log.LoadingScreen.Print("LoadingScreen.OnFadeOutComplete()");
    this.FinishPreviousScene();
    this.FadeIn();
  }

  private void FadeIn()
  {
    Log.LoadingScreen.Print("LoadingScreen.FadeIn()");
    this.m_phase = LoadingScreen.Phase.FADING_IN;
    Action onFadeInStart = this.OnFadeInStart;
    if (onFadeInStart != null)
      onFadeInStart();
    if (!this.m_prevTransitionParams.IsFadeInEnabled())
    {
      this.OnFadeInComplete();
    }
    else
    {
      CameraFade cameraFade = this.GetComponent<CameraFade>();
      if ((UnityEngine.Object) cameraFade == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "LoadingScreen FadeIn(): Failed to find CameraFade component");
      }
      else
      {
        cameraFade.m_Color = this.m_prevTransitionParams.GetFadeColor();
        Action<object> action = (Action<object>) (amount => cameraFade.m_Fade = (float) amount);
        action((object) 1f);
        iTween.ValueTo(this.gameObject, iTween.Hash((object) "time", (object) this.m_FadeInSec, (object) "from", (object) 1f, (object) "to", (object) 0.0f, (object) "onupdate", (object) action, (object) "onupdatetarget", (object) this.gameObject, (object) "oncomplete", (object) "OnFadeInComplete", (object) "oncompletetarget", (object) this.gameObject, (object) "name", (object) "Fade"));
      }
    }
  }

  private void OnFadeInComplete()
  {
    Log.LoadingScreen.Print("LoadingScreen.OnFadeInComplete()");
    this.FinishFxCamera();
    this.m_prevTransitionParams = (LoadingScreen.TransitionParams) null;
    this.m_phase = LoadingScreen.Phase.INVALID;
    this.FireFinishedTransitionListeners(false);
  }

  private void InitializeFxCamera()
  {
    this.m_fxCamera = this.GetComponent<Camera>();
    if (!((UnityEngine.Object) this.m_fxCamera != (UnityEngine.Object) null))
      return;
    this.m_fxCamera.allowMSAA = ServiceManager.Get<IGraphicsManager>().AllowMSAA();
  }

  private void FinishFxCamera()
  {
    CameraFade cameraFade = this.GetComponent<CameraFade>();
    if ((UnityEngine.Object) cameraFade == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "LoadingScreen.FinishFxCamera(): Failed to find CameraFade component");
    }
    else
    {
      if ((double) cameraFade.m_Fade <= 0.0)
        return;
      Action<object> action = (Action<object>) (amount => cameraFade.m_Fade = (float) amount);
      iTween.ValueTo(this.gameObject, iTween.Hash((object) "time", (object) 0.3f, (object) "from", (object) cameraFade.m_Fade, (object) "to", (object) 0.0f, (object) "onupdate", (object) action, (object) "onupdatetarget", (object) this.gameObject, (object) "oncompletetarget", (object) this.gameObject, (object) "delay", (object) 0.5f, (object) "name", (object) "Fade"));
    }
  }

  private FreezeFrame GetFreezeFrameEffect(Camera camera)
  {
    FreezeFrame component = camera.GetComponent<FreezeFrame>();
    return (UnityEngine.Object) component != (UnityEngine.Object) null ? component : camera.gameObject.AddComponent<FreezeFrame>();
  }

  private void ShowFreezeFrame(Camera camera)
  {
    if ((UnityEngine.Object) camera == (UnityEngine.Object) null)
      return;
    FreezeFrame freezeFrameEffect = this.GetFreezeFrameEffect(camera);
    freezeFrameEffect.enabled = true;
    freezeFrameEffect.Freeze();
  }

  private void CutoffTransition()
  {
    if (!this.IsTransitioning())
    {
      this.m_transitionParams = new LoadingScreen.TransitionParams();
    }
    else
    {
      this.StopFading();
      this.FinishPreviousScene();
      this.FinishFxCamera();
      this.m_prevTransitionParams = (LoadingScreen.TransitionParams) null;
      this.m_transitionParams = new LoadingScreen.TransitionParams();
      this.m_phase = LoadingScreen.Phase.INVALID;
      this.FireFinishedTransitionListeners(true);
    }
  }

  private void StopFading() => iTween.Stop(this.gameObject);

  private void DoInterruptionCleanUp()
  {
    bool flag = this.IsPreviousSceneActive();
    Log.LoadingScreen.Print("LoadingScreen.DoInterruptionCleanUp() - m_phase={0} previousSceneActive={1}", (object) this.m_phase, (object) flag);
    if (this.m_phase == LoadingScreen.Phase.WAITING_FOR_BLOCKERS)
      this.StopCoroutine("HackWaitThenStartTransitionEffects");
    if (this.IsFading())
    {
      this.StopFading();
      if (this.IsFadingIn())
        this.m_prevTransitionParams = (LoadingScreen.TransitionParams) null;
    }
    if (!flag)
      return;
    this.ClearAssets(this.m_assetLoadNextStartTimestamp, TimeUtils.BinaryStamp());
    this.m_transitionUnfriendlyData.Clear();
    this.m_transitionParams = new LoadingScreen.TransitionParams();
    this.m_phase = LoadingScreen.Phase.WAITING_FOR_SCENE_LOAD;
  }

  private void FinishPreviousScene()
  {
    Log.LoadingScreen.Print("LoadingScreen.FinishPreviousScene()");
    if (this.m_prevTransitionParams != null)
    {
      this.m_prevTransitionParams.DestroyObjects();
      TransformUtil.SetPosX(this.gameObject, this.m_originalPosX);
    }
    if (this.m_transitionParams.ClearPreviousAssets)
      this.ClearPreviousSceneAssets();
    this.m_transitionUnfriendlyData.Restore();
    this.m_transitionUnfriendlyData.Clear();
    this.m_previousSceneActive = false;
    this.FirePreviousSceneDestroyedListeners();
  }

  private void ClearPreviousSceneAssets()
  {
    Log.LoadingScreen.Print("LoadingScreen.ClearPreviousSceneAssets() - START m_assetLoadStartTimestamp={0} m_assetLoadEndTimestamp={1}", (object) this.m_assetLoadStartTimestamp, (object) this.m_assetLoadEndTimestamp);
    this.ClearAssets(this.m_assetLoadStartTimestamp, this.m_assetLoadEndTimestamp);
    this.m_assetLoadStartTimestamp = this.m_assetLoadNextStartTimestamp;
    this.m_assetLoadEndTimestamp = 0L;
    this.m_assetLoadNextStartTimestamp = 0L;
    Log.LoadingScreen.Print("LoadingScreen.ClearPreviousSceneAssets() - END m_assetLoadStartTimestamp={0} m_assetLoadEndTimestamp={1}", (object) this.m_assetLoadStartTimestamp, (object) this.m_assetLoadEndTimestamp);
  }

  private void ClearAssets(long startTimestamp, long endTimestamp)
  {
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if (!((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null))
      return;
    hearthstoneApplication.UnloadUnusedAssets();
  }

  private void DisableTransitionUnfriendlyStuff(GameObject mainObject)
  {
    Log.LoadingScreen.Print("LoadingScreen.DisableTransitionUnfriendlyStuff() - {0}", (object) mainObject);
    AudioListener[] componentsInChildren = this.GetComponentsInChildren<AudioListener>();
    bool flag = false;
    foreach (AudioListener audioListener in componentsInChildren)
      flag = ((flag ? 1 : 0) | (!((UnityEngine.Object) audioListener != (UnityEngine.Object) null) ? 0 : (audioListener.enabled ? 1 : 0))) != 0;
    if (flag)
      this.m_transitionUnfriendlyData.SetAudioListener(mainObject.GetComponentInChildren<AudioListener>());
    this.m_transitionUnfriendlyData.AddLights(mainObject.GetComponentsInChildren<Light>());
  }

  public delegate void PreviousSceneDestroyedCallback(object userData);

  public delegate void FinishedTransitionCallback(bool cutoff, object userData);

  public enum Phase
  {
    INVALID,
    WAITING_FOR_SCENE_UNLOAD,
    WAITING_FOR_SCENE_LOAD,
    WAITING_FOR_BLOCKERS,
    FADING_OUT,
    FADING_IN,
  }

  private class PreviousSceneDestroyedListener : 
    EventListener<LoadingScreen.PreviousSceneDestroyedCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }

  private class FinishedTransitionListener : EventListener<LoadingScreen.FinishedTransitionCallback>
  {
    public void Fire(bool cutoff) => this.m_callback(cutoff, this.m_userData);
  }

  private class TransitionParams
  {
    private List<GameObject> m_objects = new List<GameObject>();
    private List<Camera> m_cameras = new List<Camera>();
    private List<Light> m_lights = new List<Light>();
    private Camera m_freezeFrameCamera;
    private AudioListener m_audioListener;
    private int m_blockerCount;
    private bool m_fadeOut = true;
    private bool m_fadeIn = true;
    private Color m_fadeColor = Color.black;
    private bool m_clearPreviousAssets = true;

    public void AddObject(Component c)
    {
      if ((UnityEngine.Object) c == (UnityEngine.Object) null)
        return;
      this.AddObject(c.gameObject);
    }

    public void AddObject(GameObject go)
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
        return;
      for (Transform transform = go.transform; (UnityEngine.Object) transform != (UnityEngine.Object) null; transform = transform.parent)
      {
        if (this.m_objects.Contains(transform.gameObject))
          return;
      }
      foreach (Camera componentsInChild in go.GetComponentsInChildren<Camera>())
      {
        if (!this.m_cameras.Contains(componentsInChild))
          this.m_cameras.Add(componentsInChild);
      }
      this.m_objects.Add(go);
    }

    public void AddBlocker() => ++this.m_blockerCount;

    public void AddBlocker(int count) => this.m_blockerCount += count;

    public void RemoveBlocker() => --this.m_blockerCount;

    public void RemoveBlocker(int count) => this.m_blockerCount -= count;

    public int GetBlockerCount() => this.m_blockerCount;

    public void SetFreezeFrameCamera(Camera camera)
    {
      if ((UnityEngine.Object) camera == (UnityEngine.Object) null)
        return;
      this.m_freezeFrameCamera = camera;
      this.AddObject(camera.gameObject);
    }

    public Camera GetFreezeFrameCamera() => this.m_freezeFrameCamera;

    public AudioListener GetAudioListener() => this.m_audioListener;

    public void SetAudioListener(AudioListener listener)
    {
      if ((UnityEngine.Object) listener == (UnityEngine.Object) null)
        return;
      this.m_audioListener = listener;
      this.AddObject((Component) listener);
    }

    public void EnableFadeOut(bool enable) => this.m_fadeOut = enable;

    public bool IsFadeOutEnabled() => this.m_fadeOut;

    public void EnableFadeIn(bool enable) => this.m_fadeIn = enable;

    public bool IsFadeInEnabled() => this.m_fadeIn;

    public void SetFadeColor(Color color) => this.m_fadeColor = color;

    public Color GetFadeColor() => this.m_fadeColor;

    public bool ClearPreviousAssets
    {
      get => this.m_clearPreviousAssets;
      set => this.m_clearPreviousAssets = value;
    }

    public void FixCameraTagsAndDepths(float fxCameraDepth)
    {
      if (this.m_cameras.Count == 0)
        return;
      this.UntagCameras();
      this.BoostCamerasToJustBelowDepth(fxCameraDepth);
    }

    private void UntagCameras()
    {
      foreach (Camera camera in this.m_cameras)
      {
        if (!((UnityEngine.Object) camera == (UnityEngine.Object) null))
          camera.tag = "Untagged";
      }
    }

    private void BoostCamerasToJustBelowDepth(float targetDepth)
    {
      float highestCameraDepth = this.GetHighestCameraDepth();
      float num = targetDepth - 1f - highestCameraDepth;
      for (int index = 0; index < this.m_cameras.Count; ++index)
      {
        Camera camera = this.m_cameras[index];
        if (!((UnityEngine.Object) camera == (UnityEngine.Object) null))
          camera.depth += num;
      }
    }

    public float GetHighestCameraDepth()
    {
      if (this.m_cameras.Count == 0)
        return 0.0f;
      float highestCameraDepth = 0.0f;
      for (int index = 0; index < this.m_cameras.Count; ++index)
      {
        if (!((UnityEngine.Object) this.m_cameras[index] == (UnityEngine.Object) null))
        {
          float depth = this.m_cameras[index].depth;
          if ((double) highestCameraDepth < (double) depth)
            highestCameraDepth = depth;
        }
      }
      return highestCameraDepth;
    }

    public void AutoAddObjects()
    {
      foreach (Light light in (Light[]) UnityEngine.Object.FindObjectsOfType(typeof (Light)))
      {
        this.AddObject(light.gameObject);
        this.m_lights.Add(light);
      }
    }

    public void PreserveObjects(Transform parent)
    {
      foreach (GameObject gameObject in this.m_objects)
      {
        if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
          gameObject.transform.parent = parent;
      }
    }

    public void DestroyObjects()
    {
      foreach (UnityEngine.Object @object in this.m_objects)
        UnityEngine.Object.DestroyImmediate(@object);
    }
  }

  private class TransitionUnfriendlyData
  {
    private AudioListener m_audioListener;
    private List<Light> m_lights = new List<Light>();

    public void Clear()
    {
      this.m_audioListener = (AudioListener) null;
      this.m_lights.Clear();
    }

    public void SetAudioListener(AudioListener listener)
    {
      if ((UnityEngine.Object) listener == (UnityEngine.Object) null || !listener.enabled)
        return;
      this.m_audioListener = listener;
      this.m_audioListener.enabled = false;
    }

    public void AddLights(Light[] lights)
    {
      foreach (Light light in lights)
      {
        if (light.enabled)
        {
          light.enabled = false;
          Transform transform = light.transform;
          while ((UnityEngine.Object) transform.parent != (UnityEngine.Object) null)
            transform = transform.parent;
          this.m_lights.Add(light);
        }
      }
    }

    public void Restore()
    {
      for (int index = 0; index < this.m_lights.Count; ++index)
      {
        Light light = this.m_lights[index];
        if ((UnityEngine.Object) light == (UnityEngine.Object) null)
        {
          Debug.LogError((object) string.Format("TransitionUnfriendlyData.Restore() - light {0} is null!", (object) index));
        }
        else
        {
          Transform transform = light.transform;
          while ((UnityEngine.Object) transform.parent != (UnityEngine.Object) null)
            transform = transform.parent;
          light.enabled = true;
        }
      }
      if (!((UnityEngine.Object) this.m_audioListener != (UnityEngine.Object) null))
        return;
      this.m_audioListener.enabled = true;
    }
  }
}
