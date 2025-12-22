using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core.Time;
using Blizzard.T5.Fonts;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Cysharp.Threading.Tasks;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Streaming;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : IService, IHasUpdate
{
  public GameObject m_StartupCamera;
  private const float SCENE_UNLOAD_DELAY = 0.15f;
  private const float SCENE_LOADED_DELAY = 0.15f;
  private static SceneMgr s_instance;
  private int m_startupAssetLoads;
  private SceneMgr.Mode m_mode = SceneMgr.Mode.STARTUP;
  private SceneMgr.Mode m_nextMode;
  private SceneMgr.Mode m_prevMode;
  private bool m_reloadMode;
  private PegasusScene m_scene;
  private PegasusScene m_previousScene;
  private bool m_sceneLoaded;
  private bool m_transitioning;
  private bool m_performFullCleanup;
  private List<SceneMgr.ScenePreUnloadListener> m_scenePreUnloadListeners = new List<SceneMgr.ScenePreUnloadListener>();
  private List<SceneMgr.SceneUnloadedListener> m_sceneUnloadedListeners = new List<SceneMgr.SceneUnloadedListener>();
  private List<SceneMgr.ScenePreLoadListener> m_scenePreLoadListeners = new List<SceneMgr.ScenePreLoadListener>();
  private List<SceneMgr.SceneLoadedListener> m_sceneLoadedListeners = new List<SceneMgr.SceneLoadedListener>();
  private SceneMgr.OnSceneLoadCompleteForSceneDrivenTransition m_onSceneLoadCompleteForSceneDrivenTransitionCallback;
  private SceneMgr.TransitionHandlerType m_transitionHandlerType;
  private object m_sceneTransitionPayload;
  private long m_boxLoadTimestamp;
  private Coroutine m_switchModeCoroutine;
  private UniTask m_switchModeContextTask = UniTask.CompletedTask;
  private GameObject m_sceneObject;

  public bool DisableObjectDestroy { get; }

  public LoadingScreen LoadingScreen { get; private set; }

  public GameObject SceneObject
  {
    get
    {
      if ((UnityEngine.Object) this.m_sceneObject == (UnityEngine.Object) null)
        this.m_sceneObject = new GameObject(nameof (SceneMgr), new System.Type[1]
        {
          typeof (HSDontDestroyOnLoad)
        });
      return this.m_sceneObject;
    }
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    SceneMgr sceneMgr = this;
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(sceneMgr.OnFatalError));
    sceneMgr.m_transitioning = true;
    LoadComponentFromResource<LoadingScreen> loadLoadingScreen = new LoadComponentFromResource<LoadingScreen>("Prefabs/LoadingScreen", LoadResourceFlags.AutoInstantiateOnLoad | LoadResourceFlags.FailOnError);
    yield return (IAsyncJobResult) loadLoadingScreen;
    sceneMgr.LoadingScreen = loadLoadingScreen.LoadedComponent;
    sceneMgr.LoadingScreen.RegisterSceneListeners(sceneMgr);
    sceneMgr.LoadingScreen.transform.parent = sceneMgr.SceneObject.transform;
    HearthstoneApplication.Get().WillReset += new System.Action(sceneMgr.WillReset);
    if (!sceneMgr.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
    {
      sceneMgr.QueueLoadBoxJob();
      sceneMgr.RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(sceneMgr.UpdatePerformanceTrackingFromModeSwitch));
    }
  }

  public System.Type[] GetDependencies() => new System.Type[6]
  {
    typeof (GameDownloadManager),
    typeof (Network),
    typeof (GameDbf),
    typeof (IAssetLoader),
    typeof (IFontTable),
    typeof (CameraManager)
  };

  public void Shutdown()
  {
    SceneMgr.s_instance = (SceneMgr) null;
    this.LoadingScreen.UnregisterSceneListeners(this);
    this.UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.UpdatePerformanceTrackingFromModeSwitch));
    HearthstoneApplication.Get().WillReset -= new System.Action(this.WillReset);
  }

  public void LoadShaderPreCompiler()
  {
    if ((!PlatformSettings.IsMobile() ? 0 : (PlatformSettings.RuntimeOS != OSCategory.Android ? 1 : 0)) == 0)
      return;
    AssetReference assetRef = new AssetReference("ShaderPreCompiler.prefab:380ca3ee11a2643068cfb3d4766f3fd3");
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab(assetRef);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("SceneMgr.LoadShaderPreCompiler() - FAILED to load prefab", (object) assetRef));
    else
      gameObject.transform.parent = this.SceneObject.transform;
  }

  public void Update()
  {
    if (!this.m_reloadMode)
    {
      if (this.m_nextMode == SceneMgr.Mode.INVALID)
        return;
      if (this.m_mode == this.m_nextMode)
      {
        this.m_nextMode = SceneMgr.Mode.INVALID;
        return;
      }
    }
    this.m_transitioning = true;
    this.m_performFullCleanup = !this.m_reloadMode;
    this.m_prevMode = this.m_mode;
    this.m_mode = this.m_nextMode;
    this.m_nextMode = SceneMgr.Mode.INVALID;
    this.m_reloadMode = false;
    if ((UnityEngine.Object) this.m_scene != (UnityEngine.Object) null)
    {
      if (this.m_switchModeCoroutine != null)
      {
        Processor.CancelCoroutine(this.m_switchModeCoroutine);
        if ((UnityEngine.Object) this.m_previousScene != (UnityEngine.Object) null)
          Processor.RunCoroutine(this.ForceUnloadOrphanedScene(this.m_previousScene));
      }
      this.m_switchModeCoroutine = Processor.RunCoroutine(this.IsDoingSceneDrivenTransition() ? this.SwitchModeWithSceneDrivenTransition() : this.SwitchMode(), (object) this);
    }
    else
      this.LoadMode();
  }

  public static SceneMgr Get()
  {
    if (SceneMgr.s_instance == null)
      SceneMgr.s_instance = ServiceManager.Get<SceneMgr>();
    return SceneMgr.s_instance;
  }

  public static bool IsInitialized() => SceneMgr.s_instance != null;

  private void WillReset()
  {
    Log.Reset.Print("SceneMgr.WillReset()");
    if (HearthstoneApplication.IsPublic())
    {
      TimeScaleMgr.Get().SetGameTimeScale(1f);
      TimeScaleMgr.Get().SetTimeScaleMultiplier(1f);
    }
    Processor.StopAllCoroutinesWithObjectRef((object) this);
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    this.m_mode = SceneMgr.Mode.STARTUP;
    this.m_nextMode = SceneMgr.Mode.INVALID;
    this.m_prevMode = SceneMgr.Mode.INVALID;
    this.m_reloadMode = false;
    PegasusScene scene = this.m_scene;
    if ((UnityEngine.Object) scene != (UnityEngine.Object) null)
      scene.PreUnload();
    this.FireScenePreUnloadEvent(scene);
    if ((UnityEngine.Object) this.m_scene != (UnityEngine.Object) null)
    {
      this.m_scene.Unload();
      this.m_scene = (PegasusScene) null;
      this.m_sceneLoaded = false;
    }
    if (this.m_mode != SceneMgr.Mode.FATAL_ERROR)
    {
      this.FireSceneUnloadedEvent(scene);
      this.PostUnloadCleanup();
    }
    this.QueueLoadBoxJob();
    Log.Reset.Print("\tSceneMgr.WillReset() completed");
  }

  public void SetNextMode(
    SceneMgr.Mode mode,
    SceneMgr.TransitionHandlerType transitionHandler = SceneMgr.TransitionHandlerType.SCENEMGR,
    SceneMgr.OnSceneLoadCompleteForSceneDrivenTransition onLoadCompleteCallback = null,
    object sceneTransitionPayload = null)
  {
    if (this.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
      return;
    this.CacheModeForResume(mode);
    this.m_nextMode = mode;
    this.m_reloadMode = false;
    this.m_transitionHandlerType = transitionHandler;
    this.m_sceneTransitionPayload = sceneTransitionPayload;
    if (transitionHandler != SceneMgr.TransitionHandlerType.CURRENT_SCENE && transitionHandler != SceneMgr.TransitionHandlerType.NEXT_SCENE)
      return;
    if (transitionHandler == SceneMgr.TransitionHandlerType.CURRENT_SCENE && onLoadCompleteCallback == null)
      Log.All.PrintError("SceneMgr - SetNextMode did not provide the required callback!");
    this.m_onSceneLoadCompleteForSceneDrivenTransitionCallback = onLoadCompleteCallback;
  }

  public void ReloadMode()
  {
    if (this.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
      return;
    this.m_nextMode = this.m_mode;
    this.m_reloadMode = true;
  }

  public void ReturnToPreviousMode()
  {
    if (this.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
      return;
    this.CacheModeForResume(this.m_prevMode);
    this.m_nextMode = this.m_prevMode;
    this.m_reloadMode = false;
  }

  public SceneMgr.Mode GetPrevMode() => this.m_prevMode;

  public SceneMgr.Mode GetMode() => this.m_mode;

  public SceneMgr.Mode GetNextMode() => this.m_nextMode;

  public PegasusScene GetScene() => this.m_scene;

  public void SetScene(PegasusScene scene)
  {
    this.m_scene = scene;
    this.m_scene.SetSceneTransitionPayload(this.m_sceneTransitionPayload);
  }

  public bool IsSceneLoaded() => this.m_sceneLoaded;

  public bool WillTransition() => this.m_reloadMode || this.m_nextMode != SceneMgr.Mode.INVALID && this.m_nextMode != this.m_mode;

  public bool IsTransitioning() => this.m_transitioning;

  public bool IsTransitionNowOrPending() => this.IsTransitioning() || this.WillTransition();

  public bool IsDoingSceneDrivenTransition() => this.m_transitionHandlerType == SceneMgr.TransitionHandlerType.CURRENT_SCENE || this.m_transitionHandlerType == SceneMgr.TransitionHandlerType.NEXT_SCENE;

  public bool IsModeRequested(SceneMgr.Mode mode) => this.m_mode == mode || this.m_nextMode == mode;

  public bool IsInGame() => this.IsModeRequested(SceneMgr.Mode.GAMEPLAY);

  public bool IsInTavernBrawlMode()
  {
    if (this.GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
      return true;
    return this.GetMode() == SceneMgr.Mode.FIRESIDE_GATHERING && FiresideGatheringManager.Get().InBrawlMode();
  }

  public bool IsInDuelsMode() => this.GetMode() == SceneMgr.Mode.PVP_DUNGEON_RUN;

  public bool IsInArenaDraftMode() => this.GetMode() == SceneMgr.Mode.DRAFT;

  public bool IsInLettuceMode()
  {
    SceneMgr.Mode mode = this.GetMode();
    switch (mode)
    {
      case SceneMgr.Mode.LETTUCE_VILLAGE:
      case SceneMgr.Mode.LETTUCE_BOUNTY_BOARD:
      case SceneMgr.Mode.LETTUCE_MAP:
      case SceneMgr.Mode.LETTUCE_PLAY:
      case SceneMgr.Mode.LETTUCE_COLLECTION:
      case SceneMgr.Mode.LETTUCE_COOP:
      case SceneMgr.Mode.LETTUCE_FRIENDLY:
      case SceneMgr.Mode.LETTUCE_BOUNTY_TEAM_SELECT:
        return true;
      default:
        return mode == SceneMgr.Mode.LETTUCE_PACK_OPENING;
    }
  }

  public void NotifySceneLoaded()
  {
    this.m_sceneLoaded = true;
    if (this.m_mode == SceneMgr.Mode.FATAL_ERROR)
      this.DestroyAllObjectsOnModeSwitch();
    this.m_scene.SetSceneName(this.GetSceneNameFromMode(this.m_mode));
    if (this.ShouldUseSceneLoadDelays())
      Processor.RunCoroutine(this.WaitThenFireSceneLoadedEvent(), (object) this);
    else
      this.FireSceneLoadedEvent();
  }

  public void RegisterScenePreUnloadEvent(SceneMgr.ScenePreUnloadCallback callback) => this.RegisterScenePreUnloadEvent(callback, (object) null);

  public void RegisterScenePreUnloadEvent(SceneMgr.ScenePreUnloadCallback callback, object userData)
  {
    SceneMgr.ScenePreUnloadListener preUnloadListener = new SceneMgr.ScenePreUnloadListener();
    preUnloadListener.SetCallback(callback);
    preUnloadListener.SetUserData(userData);
    if (this.m_scenePreUnloadListeners.Contains(preUnloadListener))
      return;
    this.m_scenePreUnloadListeners.Add(preUnloadListener);
  }

  public bool UnregisterScenePreUnloadEvent(SceneMgr.ScenePreUnloadCallback callback) => this.UnregisterScenePreUnloadEvent(callback, (object) null);

  public bool UnregisterScenePreUnloadEvent(
    SceneMgr.ScenePreUnloadCallback callback,
    object userData)
  {
    SceneMgr.ScenePreUnloadListener preUnloadListener = new SceneMgr.ScenePreUnloadListener();
    preUnloadListener.SetCallback(callback);
    preUnloadListener.SetUserData(userData);
    return this.m_scenePreUnloadListeners.Remove(preUnloadListener);
  }

  public static bool UnregisterScenePreUnloadEventFromInstance(
    SceneMgr.ScenePreUnloadCallback callback)
  {
    return SceneMgr.s_instance != null && SceneMgr.s_instance.UnregisterScenePreUnloadEvent(callback);
  }

  public void RegisterSceneUnloadedEvent(SceneMgr.SceneUnloadedCallback callback) => this.RegisterSceneUnloadedEvent(callback, (object) null);

  public void RegisterSceneUnloadedEvent(SceneMgr.SceneUnloadedCallback callback, object userData)
  {
    SceneMgr.SceneUnloadedListener unloadedListener = new SceneMgr.SceneUnloadedListener();
    unloadedListener.SetCallback(callback);
    unloadedListener.SetUserData(userData);
    if (this.m_sceneUnloadedListeners.Contains(unloadedListener))
      return;
    this.m_sceneUnloadedListeners.Add(unloadedListener);
  }

  public bool UnregisterSceneUnloadedEvent(SceneMgr.SceneUnloadedCallback callback) => this.UnregisterSceneUnloadedEvent(callback, (object) null);

  public bool UnregisterSceneUnloadedEvent(SceneMgr.SceneUnloadedCallback callback, object userData)
  {
    SceneMgr.SceneUnloadedListener unloadedListener = new SceneMgr.SceneUnloadedListener();
    unloadedListener.SetCallback(callback);
    unloadedListener.SetUserData(userData);
    return this.m_sceneUnloadedListeners.Remove(unloadedListener);
  }

  public void RegisterScenePreLoadEvent(SceneMgr.ScenePreLoadCallback callback) => this.RegisterScenePreLoadEvent(callback, (object) null);

  public void RegisterScenePreLoadEvent(SceneMgr.ScenePreLoadCallback callback, object userData)
  {
    SceneMgr.ScenePreLoadListener scenePreLoadListener = new SceneMgr.ScenePreLoadListener();
    scenePreLoadListener.SetCallback(callback);
    scenePreLoadListener.SetUserData(userData);
    if (this.m_scenePreLoadListeners.Contains(scenePreLoadListener))
      return;
    this.m_scenePreLoadListeners.Add(scenePreLoadListener);
  }

  public bool UnregisterScenePreLoadEvent(SceneMgr.ScenePreLoadCallback callback) => this.UnregisterScenePreLoadEvent(callback, (object) null);

  public bool UnregisterScenePreLoadEvent(SceneMgr.ScenePreLoadCallback callback, object userData)
  {
    SceneMgr.ScenePreLoadListener scenePreLoadListener = new SceneMgr.ScenePreLoadListener();
    scenePreLoadListener.SetCallback(callback);
    scenePreLoadListener.SetUserData(userData);
    return this.m_scenePreLoadListeners.Remove(scenePreLoadListener);
  }

  public void RegisterSceneLoadedEvent(SceneMgr.SceneLoadedCallback callback) => this.RegisterSceneLoadedEvent(callback, (object) null);

  public void RegisterSceneLoadedEvent(SceneMgr.SceneLoadedCallback callback, object userData)
  {
    SceneMgr.SceneLoadedListener sceneLoadedListener = new SceneMgr.SceneLoadedListener();
    sceneLoadedListener.SetCallback(callback);
    sceneLoadedListener.SetUserData(userData);
    if (this.m_sceneLoadedListeners.Contains(sceneLoadedListener))
      return;
    this.m_sceneLoadedListeners.Add(sceneLoadedListener);
  }

  public bool UnregisterSceneLoadedEvent(SceneMgr.SceneLoadedCallback callback) => this.UnregisterSceneLoadedEvent(callback, (object) null);

  public bool UnregisterSceneLoadedEvent(SceneMgr.SceneLoadedCallback callback, object userData)
  {
    SceneMgr.SceneLoadedListener sceneLoadedListener = new SceneMgr.SceneLoadedListener();
    sceneLoadedListener.SetCallback(callback);
    sceneLoadedListener.SetUserData(userData);
    return this.m_sceneLoadedListeners.Remove(sceneLoadedListener);
  }

  private IEnumerator WaitThenFireSceneLoadedEvent()
  {
    yield return (object) new WaitForSeconds(0.15f);
    this.FireSceneLoadedEvent();
  }

  private void FireScenePreUnloadEvent(PegasusScene prevScene)
  {
    foreach (SceneMgr.ScenePreUnloadListener preUnloadListener in this.m_scenePreUnloadListeners.ToArray())
      preUnloadListener.Fire(this.m_prevMode, prevScene);
  }

  private void FireSceneUnloadedEvent(PegasusScene prevScene)
  {
    if (this.IsDoingSceneDrivenTransition())
      this.m_transitioning = false;
    foreach (SceneMgr.SceneUnloadedListener unloadedListener in this.m_sceneUnloadedListeners.ToArray())
      unloadedListener.Fire(this.m_prevMode, prevScene);
  }

  private void FireScenePreLoadEvent()
  {
    foreach (SceneMgr.ScenePreLoadListener scenePreLoadListener in this.m_scenePreLoadListeners.ToArray())
      scenePreLoadListener.Fire(this.m_prevMode, this.m_mode);
  }

  private void FireSceneLoadedEvent()
  {
    if (!this.IsDoingSceneDrivenTransition())
      this.m_transitioning = false;
    foreach (SceneMgr.SceneLoadedListener sceneLoadedListener in this.m_sceneLoadedListeners.ToArray())
      sceneLoadedListener.Fire(this.m_mode, this.m_scene);
    HearthstonePerformance.Get()?.SendCustomEvent("SceneLoaded" + Enum.GetName(typeof (SceneMgr.Mode), (object) this.m_mode));
  }

  private void LoadMode()
  {
    this.FireScenePreLoadEvent();
    SceneManager.LoadSceneAsync(Blizzard.T5.Core.Utils.EnumUtils.GetString<SceneMgr.Mode>(this.m_mode), LoadSceneMode.Additive);
  }

  private IEnumerator SwitchMode()
  {
    if (!this.m_scene.IsUnloading())
    {
      this.m_previousScene = this.m_scene;
      this.m_previousScene.PreUnload();
      this.FireScenePreUnloadEvent(this.m_previousScene);
      if (this.LoadingScreen.GetPhase() == LoadingScreen.Phase.WAITING_FOR_SCENE_UNLOAD && (UnityEngine.Object) this.LoadingScreen.GetFreezeFrameCamera() != (UnityEngine.Object) null)
        yield return (object) new WaitForEndOfFrame();
      if (this.ShouldUseSceneUnloadDelays())
      {
        if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null)
        {
          while (Box.Get().HasPendingEffects())
            yield return (object) 0;
        }
        else
          yield return (object) new WaitForSeconds(0.15f);
      }
      while (this.m_switchModeContextTask.Status == UniTaskStatus.Pending)
        yield return (object) null;
      this.m_scene.Unload();
      this.m_scene = (PegasusScene) null;
      this.m_sceneLoaded = false;
      this.FireSceneUnloadedEvent(this.m_previousScene);
      this.PostUnloadCleanup();
      this.LoadModeFromModeSwitch();
      this.m_switchModeCoroutine = (Coroutine) null;
      this.m_switchModeContextTask = UniTask.CompletedTask;
    }
  }

  private IEnumerator ForceUnloadOrphanedScene(PegasusScene scene)
  {
    if (!scene.IsUnloading())
    {
      this.FireScenePreUnloadEvent(scene);
      if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null)
      {
        while (Box.Get().HasPendingEffects())
          yield return (object) null;
      }
      scene.Unload();
      this.FireSceneUnloadedEvent(scene);
    }
  }

  private void OnUnloadPreviousScene()
  {
    this.m_previousScene.PreUnload();
    this.FireScenePreUnloadEvent(this.m_previousScene);
    this.m_previousScene.Unload();
    this.FireSceneUnloadedEvent(this.m_previousScene);
    this.m_previousScene = (PegasusScene) null;
  }

  private IEnumerator SwitchModeWithSceneDrivenTransition()
  {
    SceneMgr sceneMgr = this;
    if (!sceneMgr.m_scene.IsUnloading())
    {
      sceneMgr.m_previousScene = sceneMgr.m_scene;
      sceneMgr.m_sceneLoaded = false;
      sceneMgr.FireScenePreLoadEvent();
      SceneManager.LoadSceneAsync(sceneMgr.GetSceneNameFromMode(sceneMgr.m_mode), LoadSceneMode.Additive);
      while (!sceneMgr.m_sceneLoaded)
        yield return (object) null;
      if (sceneMgr.m_transitionHandlerType == SceneMgr.TransitionHandlerType.CURRENT_SCENE && sceneMgr.m_onSceneLoadCompleteForSceneDrivenTransitionCallback != null)
        sceneMgr.m_onSceneLoadCompleteForSceneDrivenTransitionCallback(new System.Action(sceneMgr.OnUnloadPreviousScene));
      else if (sceneMgr.m_transitionHandlerType == SceneMgr.TransitionHandlerType.NEXT_SCENE)
      {
        sceneMgr.m_scene.ExecuteSceneDrivenTransition(new System.Action(sceneMgr.OnUnloadPreviousScene));
      }
      else
      {
        Log.All.PrintError("No callback for scene driven scene transition.");
        sceneMgr.OnUnloadPreviousScene();
      }
      sceneMgr.m_switchModeCoroutine = (Coroutine) null;
      sceneMgr.m_onSceneLoadCompleteForSceneDrivenTransitionCallback = (SceneMgr.OnSceneLoadCompleteForSceneDrivenTransition) null;
    }
  }

  private bool ShouldUseSceneUnloadDelays() => this.m_prevMode != this.m_mode;

  private bool ShouldUseSceneLoadDelays() => this.m_mode != SceneMgr.Mode.LOGIN && this.m_mode != SceneMgr.Mode.HUB && this.m_mode != SceneMgr.Mode.FATAL_ERROR;

  private void PostUnloadCleanup()
  {
    UnityEngine.Time.captureFramerate = 0;
    this.DestroyAllObjectsOnModeSwitch();
    if (this.m_performFullCleanup)
    {
      HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
      if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
        hearthstoneApplication.UnloadUnusedAssets();
    }
    this.m_previousScene = (PegasusScene) null;
  }

  private void DestroyAllObjectsOnModeSwitch()
  {
    if (this.DisableObjectDestroy)
      return;
    int sceneCount = SceneManager.sceneCount;
    for (int index = 0; index < sceneCount; ++index)
    {
      foreach (GameObject rootGameObject in SceneManager.GetSceneAt(index).GetRootGameObjects())
      {
        if (this.ShouldDestroyOnModeSwitch(rootGameObject))
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) rootGameObject);
      }
    }
  }

  private bool ShouldDestroyOnModeSwitch(GameObject go)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null || (UnityEngine.Object) go.transform.parent != (UnityEngine.Object) null || (UnityEngine.Object) go.GetComponent<HSDontDestroyOnLoad>() != (UnityEngine.Object) null)
      return false;
    if (go.scene.buildIndex == -1)
      Debug.LogErrorFormat("GameObject ({0}) appears to be marked Don't Destroy On Load, but is being destroyed by our code anyway!", (object) go.name);
    return (!((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null) || !((UnityEngine.Object) go == (UnityEngine.Object) PegUI.Get().gameObject)) && (!((UnityEngine.Object) OverlayUI.Get() != (UnityEngine.Object) null) || !((UnityEngine.Object) go == (UnityEngine.Object) OverlayUI.Get().gameObject)) && (!((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null) || !((UnityEngine.Object) go == (UnityEngine.Object) Box.Get().gameObject) || !this.DoesModeShowBox(this.m_mode)) && !AssetLoader.Get().IsSharedPrefabInstance(go) && !AssetLoader.Get().IsWaitingOnObject(go) && !((UnityEngine.Object) go == (UnityEngine.Object) iTweenManager.Get().gameObject);
  }

  private void CacheModeForResume(SceneMgr.Mode mode)
  {
    if (PlatformSettings.OS != OSCategory.iOS && PlatformSettings.OS != OSCategory.Android)
      return;
    switch (mode)
    {
      case SceneMgr.Mode.HUB:
      case SceneMgr.Mode.FRIENDLY:
        Options.Get().SetInt(Option.LAST_SCENE_MODE, 0);
        break;
      case SceneMgr.Mode.COLLECTIONMANAGER:
      case SceneMgr.Mode.TOURNAMENT:
      case SceneMgr.Mode.DRAFT:
      case SceneMgr.Mode.CREDITS:
      case SceneMgr.Mode.ADVENTURE:
      case SceneMgr.Mode.TAVERN_BRAWL:
      case SceneMgr.Mode.FIRESIDE_GATHERING:
      case SceneMgr.Mode.BACON:
      case SceneMgr.Mode.GAME_MODE:
      case SceneMgr.Mode.LETTUCE_VILLAGE:
      case SceneMgr.Mode.LETTUCE_BOUNTY_BOARD:
      case SceneMgr.Mode.LETTUCE_MAP:
      case SceneMgr.Mode.LETTUCE_PLAY:
      case SceneMgr.Mode.LETTUCE_COLLECTION:
      case SceneMgr.Mode.LETTUCE_COOP:
      case SceneMgr.Mode.LETTUCE_BOUNTY_TEAM_SELECT:
      case SceneMgr.Mode.LETTUCE_PACK_OPENING:
        Options.Get().SetInt(Option.LAST_SCENE_MODE, (int) mode);
        break;
    }
  }

  private bool DoesModeShowBox(SceneMgr.Mode mode) => mode != SceneMgr.Mode.STARTUP && mode != SceneMgr.Mode.GAMEPLAY && mode != SceneMgr.Mode.RESET;

  private void LoadModeFromModeSwitch()
  {
    bool flag1 = this.DoesModeShowBox(this.m_prevMode);
    bool flag2 = this.DoesModeShowBox(this.m_mode);
    if (!flag1 & flag2)
    {
      Processor.QueueJob("SceneMgr.Reload", this.Job_ReloadBox());
    }
    else
    {
      if ((!flag1 ? 0 : (!flag2 ? 1 : 0)) != 0)
      {
        this.LoadingScreen.SetAssetLoadStartTimestamp(this.m_boxLoadTimestamp);
        this.m_boxLoadTimestamp = 0L;
      }
      this.LoadMode();
    }
  }

  private void QueueLoadBoxJob()
  {
    IJobDependency[] jobDependencyArray = HearthstoneJobs.BuildDependencies((object) typeof (SceneMgr), (object) typeof (IAssetLoader), (object) typeof (NetCache), (object) new WaitForGameDownloadManagerState(), (object) new WaitForSplashScreen());
    Processor.QueueJob("SceneMgr.LoadBox", this.Job_LoadBox(), jobDependencyArray);
  }

  private IEnumerator<IAsyncJobResult> Job_LoadBox()
  {
    yield return (IAsyncJobResult) new LoadUIScreen((AssetReference) "TheBox.prefab:6b55a928ffdc1b341b5dbe8f8a88e768");
    this.m_nextMode = SceneMgr.Mode.LOGIN;
  }

  private IEnumerator<IAsyncJobResult> Job_ReloadBox()
  {
    yield return (IAsyncJobResult) new LoadUIScreen((AssetReference) "TheBox.prefab:6b55a928ffdc1b341b5dbe8f8a88e768");
    this.LoadMode();
  }

  private void OnFatalError(FatalErrorMessage message, object userData)
  {
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.SET_ROTATION_INTRO))
    {
      Log.Offline.Print("SceneMgr.OnFatalError: Error blocked by set rotation.");
      this.SetNextMode(SceneMgr.Mode.FATAL_ERROR);
    }
    else if (!ReconnectMgr.IsReconnectAllowed(message))
    {
      if (message.m_reason == FatalErrorReason.MOBILE_GAME_SERVER_RPC_ERROR)
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER"),
          m_text = GameStrings.Get("GLOBAL_MOBILE_ERROR_GAMESERVER_CONNECT"),
          m_showAlertIcon = false,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_confirmText = GameStrings.Get("GLOBAL_BUTTON_MORE_INFO"),
          m_cancelText = GameStrings.Get("GLOBAL_BUTTON_GOT_IT"),
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, uData) =>
          {
            bool moreinfoPressed = false;
            bool gotitPressed = false;
            switch (response)
            {
              case AlertPopup.Response.CONFIRM:
                Application.OpenURL(ExternalUrlService.Get().GetMobileGameServerConnectionLink());
                moreinfoPressed = true;
                break;
              case AlertPopup.Response.CANCEL:
                gotitPressed = true;
                break;
            }
            GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
            TelemetryManager.Client().SendMobileFailConnectGameServer(gameServerJoined?.Address ?? (string) null, moreinfoPressed, gotitPressed);
            Log.Telemetry.PrintInfo("{0}, {1}, {2}", (object) (gameServerJoined?.Address ?? (string) null), (object) moreinfoPressed, (object) gotitPressed);
          })
        };
        DialogManager.Get().ShowPopup(info);
      }
      else
      {
        FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
        ReconnectMgr service;
        if (ServiceManager.TryGet<ReconnectMgr>(out service))
          service.FullResetRequired = true;
        this.GoToFatalErrorScreen(message);
      }
    }
    else
    {
      switch (this.m_mode)
      {
        case SceneMgr.Mode.STARTUP:
          break;
        case SceneMgr.Mode.LOGIN:
        case SceneMgr.Mode.GAMEPLAY:
          this.GoToFatalErrorScreen(message);
          break;
        case SceneMgr.Mode.HUB:
          StoreManager.Get().HandleDisconnect();
          break;
        case SceneMgr.Mode.COLLECTIONMANAGER:
          CollectionManager.Get().HandleDisconnect();
          break;
        case SceneMgr.Mode.PACKOPENING:
          break;
        case SceneMgr.Mode.TOURNAMENT:
          break;
        case SceneMgr.Mode.CREDITS:
          break;
        case SceneMgr.Mode.TAVERN_BRAWL:
          CollectionManager collectionManager = CollectionManager.Get();
          if (!collectionManager.IsInEditMode())
            break;
          collectionManager.HandleDisconnect();
          break;
        default:
          Log.Offline.PrintDebug("Bypassing Fatal Error To HUB.");
          Navigation.Clear();
          if (!this.IsTransitionNowOrPending() || this.m_nextMode != SceneMgr.Mode.HUB)
            DialogManager.Get().ShowReconnectHelperDialog();
          this.SetNextMode(SceneMgr.Mode.HUB);
          break;
      }
    }
  }

  private void GoToFatalErrorScreen(FatalErrorMessage message)
  {
    if (HearthstoneApplication.Get().ResetOnErrorIfNecessary())
    {
      Log.Offline.PrintDebug("SceneMgr.GoToFatalErrorScreen() - Auto resetting. Do not display Fatal Error Screen.");
    }
    else
    {
      Log.BattleNet.PrintDebug("Set FatalError mode={0}, m_allowClick={1}, m_redirectToStore={2}", (object) this.m_mode, (object) message.m_allowClick, (object) message.m_redirectToStore);
      FatalErrorMgr.Get().SetUnrecoverable(this.m_mode == SceneMgr.Mode.STARTUP && (!message.m_allowClick || !message.m_redirectToStore));
      this.SetNextMode(SceneMgr.Mode.FATAL_ERROR);
    }
  }

  public bool DoesCurrentSceneSupportOfflineActivity()
  {
    switch (this.m_mode)
    {
      case SceneMgr.Mode.STARTUP:
      case SceneMgr.Mode.HUB:
      case SceneMgr.Mode.COLLECTIONMANAGER:
      case SceneMgr.Mode.PACKOPENING:
      case SceneMgr.Mode.TOURNAMENT:
      case SceneMgr.Mode.CREDITS:
      case SceneMgr.Mode.TAVERN_BRAWL:
      case SceneMgr.Mode.LETTUCE_COLLECTION:
        return true;
      default:
        return false;
    }
  }

  private void UpdatePerformanceTrackingFromModeSwitch(
    SceneMgr.Mode mode,
    PegasusScene scene,
    object userData)
  {
    if (mode != SceneMgr.Mode.GAMEPLAY)
      return;
    HearthstonePerformance hearthstonePerformance = HearthstonePerformance.Get();
    if (hearthstonePerformance == null)
      return;
    hearthstonePerformance.CaptureBoxInteractableTime();
    this.UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.UpdatePerformanceTrackingFromModeSwitch));
  }

  private string GetSceneNameFromMode(SceneMgr.Mode mode) => Blizzard.T5.Core.Utils.EnumUtils.GetString<SceneMgr.Mode>(mode);

  public enum Mode
  {
    INVALID,
    STARTUP,
    [Description("Login")] LOGIN,
    [Description("Hub")] HUB,
    [Description("Gameplay")] GAMEPLAY,
    [Description("CollectionManager")] COLLECTIONMANAGER,
    [Description("PackOpening")] PACKOPENING,
    [Description("Tournament")] TOURNAMENT,
    [Description("Friendly")] FRIENDLY,
    [Description("FatalError")] FATAL_ERROR,
    [Description("Draft")] DRAFT,
    [Description("Credits")] CREDITS,
    [Description("Reset")] RESET,
    [Description("Adventure")] ADVENTURE,
    [Description("TavernBrawl")] TAVERN_BRAWL,
    [Description("FiresideGathering")] FIRESIDE_GATHERING,
    [Description("Bacon")] BACON,
    [Description("GameMode")] GAME_MODE,
    [Description("PvPDungeonRun")] PVP_DUNGEON_RUN,
    [Description("BaconCollection")] BACON_COLLECTION,
    [Description("Lettuce")] LETTUCE_VILLAGE,
    [Description("LettuceBountyBoard")] LETTUCE_BOUNTY_BOARD,
    [Description("LettuceMap")] LETTUCE_MAP,
    [Description("LettucePlay")] LETTUCE_PLAY,
    [Description("LettuceCollection")] LETTUCE_COLLECTION,
    [Description("LettuceCoOp")] LETTUCE_COOP,
    [Description("LettuceFriendly")] LETTUCE_FRIENDLY,
    [Description("LettuceBountyTeamSelect")] LETTUCE_BOUNTY_TEAM_SELECT,
    [Description("VillagePackOpening")] LETTUCE_PACK_OPENING,
    [Description("LuckyDraw")] LUCKY_DRAW,
  }

  public enum TransitionHandlerType
  {
    INVALID,
    SCENEMGR,
    CURRENT_SCENE,
    NEXT_SCENE,
  }

  public delegate void ScenePreUnloadCallback(
    SceneMgr.Mode prevMode,
    PegasusScene prevScene,
    object userData);

  public delegate void SceneUnloadedCallback(
    SceneMgr.Mode prevMode,
    PegasusScene prevScene,
    object userData);

  public delegate void ScenePreLoadCallback(
    SceneMgr.Mode prevMode,
    SceneMgr.Mode mode,
    object userData);

  public delegate void SceneLoadedCallback(SceneMgr.Mode mode, PegasusScene scene, object userData);

  private class ScenePreUnloadListener : EventListener<SceneMgr.ScenePreUnloadCallback>
  {
    public void Fire(SceneMgr.Mode prevMode, PegasusScene prevScene) => this.m_callback(prevMode, prevScene, this.m_userData);
  }

  private class SceneUnloadedListener : EventListener<SceneMgr.SceneUnloadedCallback>
  {
    public void Fire(SceneMgr.Mode prevMode, PegasusScene prevScene) => this.m_callback(prevMode, prevScene, this.m_userData);
  }

  private class ScenePreLoadListener : EventListener<SceneMgr.ScenePreLoadCallback>
  {
    public void Fire(SceneMgr.Mode prevMode, SceneMgr.Mode mode) => this.m_callback(prevMode, mode, this.m_userData);
  }

  private class SceneLoadedListener : EventListener<SceneMgr.SceneLoadedCallback>
  {
    public void Fire(SceneMgr.Mode mode, PegasusScene scene) => this.m_callback(mode, scene, this.m_userData);
  }

  public delegate void OnSceneLoadCompleteForSceneDrivenTransition(System.Action onTransitionComplete);
}
