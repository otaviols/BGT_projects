using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core.Utils;
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
using System.Diagnostics;
using System.IO;
using Unity.Profiling;
using UnityEngine;

public class AssetLoader : IAssetLoader, IService
{
  private readonly Vector3 SPAWN_POS_CAMERA_OFFSET = new Vector3(0.0f, 0.0f, -5000f);
  private List<GameObject> m_waitingOnObjects = new List<GameObject>();
  private int m_framesSinceLastDeadHandlesCheck;
  private IAssetManager m_assetManager;
  private IPrefabInstantiator m_prefabInstantiator;
  private static readonly ProfilerMarker s_assetLoaderUpdateProfiler = new ProfilerMarker("AssetLoader.Update");
  private bool m_prefabInstantiatorCheckForDeadHandlesCompleted = true;
  private bool m_assetManagerForDeadHandlesCompleted = true;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    AssetLoader assetLoader = this;
    if (serviceLocator.Exists(typeof (GameDownloadManager), true))
      yield return (IAsyncJobResult) new WaitForGameDownloadManagerState();
    IAssetBank appropriateAssetBank = assetLoader.CreateAppropriateAssetBank();
    assetLoader.m_assetManager = (IAssetManager) new Blizzard.T5.AssetManager.AssetManager((Blizzard.T5.Core.ILogger) Log.Asset, appropriateAssetBank);
    assetLoader.m_assetManager.OnAssetHandleOrphaned += new System.Action<string, string>(AssetLoader.SendAssetHandleOrphanedTelemetry);
    assetLoader.m_prefabInstantiator = (IPrefabInstantiator) new PrefabInstantiator((Blizzard.T5.Core.ILogger) Log.Asset);
    assetLoader.m_prefabInstantiator.OnSharedPrefabHandleOrphaned += new System.Action<string, string>(AssetLoader.SendSharedPrefabHandleOrphanedTelemetry);
    serviceLocator.SetJobResultHandler<Hearthstone.Core.InstantiatePrefab>(new System.Action<IAsyncJobResult>(assetLoader.OnInstantiatePrefabResultHandler));
    serviceLocator.SetJobResultHandler<Hearthstone.Core.LoadPrefab>(new System.Action<IAsyncJobResult>(assetLoader.OnLoadAssetResultHandle<GameObject>));
    serviceLocator.SetJobResultHandler<LoadUIScreen>(new System.Action<IAsyncJobResult>(assetLoader.OnInstantiatePrefabResultHandler));
    serviceLocator.SetJobResultHandler<LoadFontDef>(new System.Action<IAsyncJobResult>(assetLoader.OnLoadAssetResultHandle<FontDefinition>));
    Processor.RegisterUpdateDelegate(new System.Action(assetLoader.Update));
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown() => Processor.UnregisterUpdateDelegate(new System.Action(this.Update));

  public void Update()
  {
    this.m_assetManager.CheckPendingRequests();
    this.m_prefabInstantiator.ReleaseUnreferencedAssets();
    this.m_assetManager.ReleaseUnreferencedAssets();
    this.m_assetManager.CloseUnreferencedBundles();
    if (++this.m_framesSinceLastDeadHandlesCheck <= 30)
      return;
    if (this.m_prefabInstantiatorCheckForDeadHandlesCompleted)
    {
      this.m_prefabInstantiatorCheckForDeadHandlesCompleted = false;
      this.StartCheckForDeadHandlesWorker(new Func<double, bool>(this.m_prefabInstantiator.CheckForDeadHandles), 0.2, new System.Action(this.OnPrefabInstantiatorDeadHandleCheckComplete)).Forget();
    }
    if (this.m_assetManagerForDeadHandlesCompleted)
    {
      this.m_assetManagerForDeadHandlesCompleted = false;
      this.StartCheckForDeadHandlesWorker(new Func<double, bool>(this.m_assetManager.CheckForDeadHandles), 0.2, new System.Action(this.OnAssetManagerDeadHandleCheckComplete)).Forget();
    }
    this.m_framesSinceLastDeadHandlesCheck = 0;
  }

  private async UniTaskVoid StartCheckForDeadHandlesWorker(
    Func<double, bool> checkForDeadHandles,
    double maxProcessingTime,
    System.Action completedCallback)
  {
    while (!checkForDeadHandles(maxProcessingTime))
      await UniTask.NextFrame();
    completedCallback();
  }

  private void OnPrefabInstantiatorDeadHandleCheckComplete() => this.m_prefabInstantiatorCheckForDeadHandlesCompleted = true;

  private void OnAssetManagerDeadHandleCheckComplete() => this.m_assetManagerForDeadHandlesCompleted = true;

  private void OnInstantiatePrefabResultHandler(IAsyncJobResult result)
  {
    if (!(result is Hearthstone.Core.InstantiatePrefab instantiatePrefab))
      return;
    AssetLoadingOptions options = instantiatePrefab.UsePrefabPosition ? AssetLoadingOptions.None : AssetLoadingOptions.IgnorePrefabPosition;
    this.InstantiatePrefab(instantiatePrefab.AssetRef, new PrefabCallback<GameObject>(instantiatePrefab.OnPrefabInstantiated), (object) null, options);
  }

  private void OnLoadAssetResultHandle<T>(IAsyncJobResult result) where T : UnityEngine.Object
  {
    if (!(result is Hearthstone.Core.LoadAsset<T> loadAsset))
      return;
    this.LoadAsset<T>(loadAsset.AssetRef, new AssetHandleCallback<T>(loadAsset.OnAssetLoaded), (object) null, AssetLoadingOptions.None);
  }

  public static IAssetLoader Get() => ServiceManager.Get<IAssetLoader>();

  public bool IsWaitingOnObject(GameObject go) => this.m_waitingOnObjects.Contains(go) || this.m_prefabInstantiator.IsWaitingOnObject(go);

  public bool IsSharedPrefabInstance(GameObject go) => this.m_prefabInstantiator.IsSharedPrefabInstance(go);

  private Asset GetAppropriateAsset(AssetReference assetRef, AssetLoadingOptions options)
  {
    AssetVariantTags.Quality quality = options.HasFlag((Enum) AssetLoadingOptions.UseLowQuality) ? AssetVariantTags.Quality.Low : AssetVariantTags.Quality.Normal;
    AssetReference assetRef1 = assetRef;
    bool flag = options.HasFlag((Enum) AssetLoadingOptions.DisableLocalization);
    int num1 = (int) quality;
    int num2 = flag ? 1 : 0;
    return this.GetAppropriateAsset(assetRef1, (AssetVariantTags.Quality) num1, num2 != 0);
  }

  private Asset GetAppropriateAsset(
    AssetReference assetRef,
    AssetVariantTags.Quality quality = AssetVariantTags.Quality.Normal,
    bool disableLocalization = false)
  {
    if (assetRef == null)
      return (Asset) null;
    Locale locale = Locale.enUS;
    if (!disableLocalization)
    {
      locale = Localization.GetLocale();
      if (Network.IsRunning() && BattleNet.GetAccountCountry() == "CHN")
        locale = Locale.zhCN;
    }
    return this.GetAppropriateAssetInternal(assetRef, quality, locale);
  }

  private Asset GetAppropriateAssetInternal(
    AssetReference originalAssetRef,
    AssetVariantTags.Quality quality = AssetVariantTags.Quality.Normal,
    Locale locale = Locale.enUS)
  {
    if (originalAssetRef == null || originalAssetRef.guid == null)
    {
      Log.Asset.PrintError("Invalid assetRef: {0} is null\n{1}", originalAssetRef == null ? (object) "assetRef" : (object) "guid", (object) new StackTrace());
      return (Asset) null;
    }
    AssetVariantTags.Locale variantTagForLocale = AssetVariantTags.GetLocaleVariantTagForLocale(locale);
    AssetVariantTags.Platform platform = (bool) UniversalInputManager.UsePhoneUI ? AssetVariantTags.Platform.Phone : AssetVariantTags.Platform.Any;
    if (AssetManifest.Get() == null)
    {
      Log.Asset.PrintWarning("[AssetLoader.GetAppropriateAssetInternal] AssetManifest isn't initialized.");
      return (Asset) null;
    }
    string resolvedGuid;
    if (!AssetManifest.Get().TryResolveAsset(originalAssetRef.guid, out resolvedGuid, out string _, variantTagForLocale, quality, platform))
    {
      Log.Asset.PrintWarning("[AssetLoader.GetAppropriateAssetInternal] Unable to find {0} in asset manifest.", (object) originalAssetRef);
      return (Asset) null;
    }
    if (this.IsAssetWithGuidAvailable(resolvedGuid))
      return new Asset(resolvedGuid);
    Log.Asset.PrintWarning("[AssetLoader.GetAppropriateAssetInternal] Appropriate asset {0} to original asset {1} is not available. quality={2}, locale={3}({4}), platform={5}", (object) resolvedGuid, (object) originalAssetRef, (object) quality, (object) locale, (object) variantTagForLocale, (object) platform);
    return (Asset) null;
  }

  public bool LoadMaterial(
    AssetReference assetRef,
    ObjectCallback callback,
    object callbackData = null,
    bool persistent = false,
    bool disableLocalization = false)
  {
    return this.LoadObject<Material>(assetRef, callback, callbackData, persistent, disableLocalization);
  }

  public Material LoadMaterial(
    AssetReference assetRef,
    bool persistent = false,
    bool disableLocalization = false)
  {
    Asset appropriateAsset = this.GetAppropriateAsset(assetRef, disableLocalization: disableLocalization);
    return this.LoadObjectImmediately<Material>(assetRef, appropriateAsset);
  }

  public bool LoadTexture(
    AssetReference assetRef,
    ObjectCallback callback,
    object callbackData = null,
    bool persistent = false,
    bool disableLocalization = false)
  {
    return this.LoadObject<Texture>(assetRef, callback, callbackData, persistent, disableLocalization);
  }

  public Texture LoadTexture(
    AssetReference assetRef,
    bool persistent = false,
    bool disableLocalization = false)
  {
    Log.AsyncLoading.PrintWarning("warning CS0618: `LoadTexture(Asset, bool, bool)' is obsolete: from now on, always use async loading instead (i.e. LoadTexture with callback).");
    if (assetRef == null)
    {
      Error.AddDevFatal("AssetLoader.LoadTexture() - An asset request was made but no file name was given.");
      return (Texture) null;
    }
    Asset appropriateAsset = this.GetAppropriateAsset(assetRef, disableLocalization: disableLocalization);
    return this.LoadObjectImmediately<Texture>(assetRef, appropriateAsset);
  }

  public bool LoadMesh(
    AssetReference assetRef,
    ObjectCallback callback,
    object callbackData = null,
    bool persistent = false,
    bool disableLocalization = false)
  {
    ObjectCallback callback1 = (ObjectCallback) ((meshAssetRef, meshObj, meshCallbackData) =>
    {
      GameObject gameObject = meshObj as GameObject;
      MeshFilter meshFilter = (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? gameObject.GetComponent<MeshFilter>() : (MeshFilter) null;
      Mesh mesh = (UnityEngine.Object) meshFilter != (UnityEngine.Object) null ? meshFilter.sharedMesh : (Mesh) null;
      callback(meshAssetRef, (UnityEngine.Object) mesh, meshCallbackData);
    });
    return this.LoadObject<GameObject>(assetRef, callback1, callbackData, persistent, disableLocalization);
  }

  public Mesh LoadMesh(AssetReference assetRef, bool persistent = false, bool disableLocalization = false)
  {
    Asset appropriateAsset = this.GetAppropriateAsset(assetRef, disableLocalization: disableLocalization);
    GameObject gameObject = this.LoadObjectImmediately<GameObject>(assetRef, appropriateAsset);
    MeshFilter meshFilter = (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? gameObject.GetComponent<MeshFilter>() : (MeshFilter) null;
    return !((UnityEngine.Object) meshFilter != (UnityEngine.Object) null) ? (Mesh) null : meshFilter.sharedMesh;
  }

  public bool LoadGameObject(
    AssetReference assetRef,
    GameObjectCallback callback,
    object callbackData = null,
    bool persistent = false,
    bool autoInstantiateOnLoad = true,
    bool usePrefabPosition = true)
  {
    return this.LoadPrefab(assetRef, usePrefabPosition, callback, callbackData, persistent, autoInstantiateOnLoad: autoInstantiateOnLoad);
  }

  [Obsolete("from now on, always use async loading instead (i.e. LoadUberAnimation with callback).")]
  public UberShaderAnimation LoadUberAnimation(
    AssetReference assetRef,
    bool usePrefabPosition = true,
    bool persistent = false)
  {
    Asset appropriateAsset = this.GetAppropriateAsset(assetRef);
    return this.LoadObjectImmediately<UberShaderAnimation>(assetRef, appropriateAsset);
  }

  private IAssetBank CreateAppropriateAssetBank()
  {
    IAssetBank decoratedBank = this.CreateAssetBundlesAssetBank();
    if (Vars.Key("Application.CachingAssetBankEnabled").GetBool(false))
      decoratedBank = (IAssetBank) new CachingAssetBank((Blizzard.T5.Core.ILogger) Log.Asset, decoratedBank);
    return decoratedBank;
  }

  private IAssetBank CreateAssetBundlesAssetBank()
  {
    string assetBundlePath = AssetBundleInfo.GetAssetBundlePath(ScriptableAssetManifest.MainManifestBundleName);
    if (!File.Exists(assetBundlePath))
    {
      Log.Asset.PrintError("Cannot find asset bundle for AssetBundleDependencyGraph '{0}', editor {1}, playing {2}", (object) assetBundlePath, (object) Application.isEditor, (object) Application.isPlaying);
      throw new ApplicationException("Could not initialize AssetLoader: missing AssetBundleDependencyGraph");
    }
    AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
    if ((UnityEngine.Object) assetBundle == (UnityEngine.Object) null)
    {
      Log.Asset.PrintError("Failed to load bundle for AssetBundleDependencyGraph '{0}', editor {1}, playing {2}", (object) assetBundlePath, (object) Application.isEditor, (object) Application.isPlaying);
      throw new ApplicationException("Could not initialize AssetLoader: failed to load bundle with AssetBundleDependencyGraph");
    }
    AssetBundleDependencyGraph dependencyGraph = assetBundle.LoadAsset<AssetBundleDependencyGraph>(ScriptableAssetManifest.BundleDepsAssetPath);
    assetBundle.Unload(false);
    if ((UnityEngine.Object) dependencyGraph == (UnityEngine.Object) null)
    {
      Log.Asset.PrintError("Failed to load '{0}' from bundle '{1}'", (object) ScriptableAssetManifest.BundleDepsAssetPath, (object) assetBundlePath);
      throw new ApplicationException("Could not initialize AssetLoader: failed to load AssetBundleDependencyGraph");
    }
    return (IAssetBank) new AssetBundleAssetBank((Blizzard.T5.Core.ILogger) Log.Asset, (IAssetLocator) new AssetLocator(AssetManifest.Get()), dependencyGraph);
  }

  private bool LoadObject<T>(
    AssetReference assetRef,
    ObjectCallback callback,
    object callbackData,
    bool persistent = false,
    bool disableLocalization = false)
    where T : UnityEngine.Object
  {
    Asset asset = this.GetAppropriateAsset(assetRef, disableLocalization: disableLocalization);
    this.m_assetManager.LoadAsync<T>(asset?.GetGuid(), false).OnCompleted += (AssetLoadedCB<T>) (completedRequest =>
    {
      this.OnAssetLoaded<T>(assetRef, asset?.GetGuid(), completedRequest.Result);
      callback(assetRef, (UnityEngine.Object) completedRequest.Result.Asset, callbackData);
    });
    return true;
  }

  private bool LoadPrefab(
    AssetReference assetRef,
    bool usePrefabPosition,
    GameObjectCallback callback,
    object callbackData,
    bool persistent = false,
    UnityEngine.Object fallback = null,
    bool autoInstantiateOnLoad = true)
  {
    Asset appropriateAsset = this.GetAppropriateAsset(assetRef);
    this.LoadPrefabInternal(appropriateAsset, assetRef, usePrefabPosition, callback, callbackData, fallback, autoInstantiateOnLoad);
    return appropriateAsset != null && this.IsAssetWithGuidAvailable(appropriateAsset.GetGuid());
  }

  private GameObject TryGetAsGameObject(string guid, UnityEngine.Object obj)
  {
    GameObject asGameObject = obj as GameObject;
    if (!(bool) (UnityEngine.Object) asGameObject)
    {
      string messageKey = GameStrings.Format("GLOBAL_ERROR_ASSET_INCORRECT_DATA", (object) guid);
      UnityEngine.Debug.LogError((object) string.Format("AssetLoader.WaitThenCallGameObjectCallback() - {0} (prefab={1})", (object) messageKey, (object) obj));
      Error.AddFatal(FatalErrorReason.ASSET_INCORRECT_DATA, messageKey);
    }
    return asGameObject;
  }

  private void LoadPrefabInternal(
    Asset assetToLoad,
    AssetReference requestedReference,
    bool usePrefabPosition,
    GameObjectCallback callback,
    object callbackData,
    UnityEngine.Object fallback,
    bool autoInstantiateOnLoad)
  {
    string guid = assetToLoad?.GetGuid();
    if (guid == null)
    {
      if (!GeneralUtils.IsCallbackValid((Delegate) callback))
        return;
      callback((AssetReference) null, (GameObject) null, callbackData);
    }
    else
      this.m_assetManager.LoadAsync<GameObject>(assetToLoad.GetGuid(), false).OnCompleted += (AssetLoadedCB<GameObject>) (completedRequest =>
      {
        this.OnAssetLoaded<GameObject>(requestedReference, guid, completedRequest.Result);
        GameObject asGameObject = this.TryGetAsGameObject(guid, (UnityEngine.Object) completedRequest.Result.Asset);
        if (autoInstantiateOnLoad)
        {
          Processor.RunCoroutine(this.InstantiateAndWaitThenCallGameObjectCallback(requestedReference, asGameObject, usePrefabPosition, callback, callbackData));
        }
        else
        {
          if (!GeneralUtils.IsCallbackValid((Delegate) callback))
            return;
          callback(requestedReference, asGameObject, callbackData);
        }
      });
  }

  private void OnAssetLoaded<T>(
    AssetReference requestedAsset,
    string resolvedGuid,
    AssetHandle<T> loadedAsset)
    where T : UnityEngine.Object
  {
    if (loadedAsset != null)
      return;
    AssetLoader.LogMissingAsset(requestedAsset, resolvedGuid, typeof (T).Name);
  }

  public bool IsAssetAvailable(AssetReference assetRef) => assetRef != null && this.IsAssetWithGuidAvailable(assetRef.guid);

  public bool IsAppropriateVariantAvailable(AssetReference assetRef, AssetLoadingOptions options) => this.IsAssetWithGuidAvailable(this.GetAppropriateAsset(assetRef, options)?.GetGuid());

  private bool IsAssetWithGuidAvailable(string assetGuid)
  {
    IGameDownloadManager gameDownloadManager = GameDownloadManagerProvider.Get();
    return gameDownloadManager != null && gameDownloadManager.IsAssetDownloaded(assetGuid);
  }

  public AssetHandle<T> LoadAsset<T>(
    AssetReference requestedAsset,
    AssetLoadingOptions options = AssetLoadingOptions.None)
    where T : UnityEngine.Object
  {
    Asset appropriateAsset = this.GetAppropriateAsset(requestedAsset, options);
    if (appropriateAsset == null)
      return (AssetHandle<T>) null;
    AssetHandle<T> loadedAsset = this.m_assetManager.Load<T>(appropriateAsset.GetGuid(), true);
    this.OnAssetLoaded<T>(requestedAsset, appropriateAsset.GetGuid(), loadedAsset);
    return loadedAsset;
  }

  public bool LoadAsset<T>(
    ref AssetHandle<T> assetHandle,
    AssetReference assetRef,
    AssetLoadingOptions options = AssetLoadingOptions.None)
    where T : UnityEngine.Object
  {
    AssetHandle<T> assetHandle1 = this.LoadAsset<T>(assetRef, options);
    AssetHandle.SafeDispose<T>(ref assetHandle);
    assetHandle = assetHandle1;
    return (bool) assetHandle;
  }

  public bool LoadAsset<T>(
    AssetReference assetRef,
    AssetHandleCallback<T> callback,
    object callbackData = null,
    AssetLoadingOptions options = AssetLoadingOptions.None)
    where T : UnityEngine.Object
  {
    Asset appropriateAsset = this.GetAppropriateAsset(assetRef, options);
    if (appropriateAsset == null)
    {
      if (GeneralUtils.IsCallbackValid((Delegate) callback))
        callback((AssetReference) null, (AssetHandle<T>) null, callbackData);
      return false;
    }
    this.m_assetManager.LoadAsync<T>(appropriateAsset.GetGuid(), true).OnCompleted += (AssetLoadedCB<T>) (completedRequest =>
    {
      this.OnAssetLoaded<T>(assetRef, appropriateAsset.GetGuid(), completedRequest.Result);
      if (GeneralUtils.IsCallbackValid((Delegate) callback))
        callback(assetRef, completedRequest.Result, callbackData);
      else
        completedRequest.Result?.Dispose();
    });
    return true;
  }

  public GameObject InstantiatePrefab(
    AssetReference assetRef,
    AssetLoadingOptions options)
  {
    using (AssetHandle<GameObject> prefabAsset = this.LoadAsset<GameObject>(assetRef, options))
      return this.m_prefabInstantiator.InstantiatePrefab(prefabAsset, options);
  }

  public bool InstantiatePrefab(
    AssetReference assetRef,
    PrefabCallback<GameObject> callback,
    object callbackData,
    AssetLoadingOptions options)
  {
    AssetLoader.InstantiatePrefabCallbackData<GameObject> callbackData1 = new AssetLoader.InstantiatePrefabCallbackData<GameObject>()
    {
      callerCallback = callback,
      callerData = callbackData,
      callerOptions = options,
      requestedAssetRef = assetRef
    };
    return this.LoadAsset<GameObject>(assetRef, new AssetHandleCallback<GameObject>(this.OnPrefabLoaded), (object) callbackData1, options);
  }

  public AssetHandle<GameObject> GetOrInstantiateSharedPrefab(
    AssetReference assetRef,
    AssetLoadingOptions options = AssetLoadingOptions.None)
  {
    using (AssetHandle<GameObject> prefabAsset = this.LoadAsset<GameObject>(assetRef, options))
      return this.m_prefabInstantiator.GetOrInstantiateSharedPrefab(prefabAsset, options);
  }

  private void OnPrefabLoaded(
    AssetReference prefabRef,
    AssetHandle<GameObject> prefabHandle,
    object callbackData)
  {
    using (prefabHandle)
    {
      AssetLoader.InstantiatePrefabCallbackData<GameObject> callbackData1 = callbackData as AssetLoader.InstantiatePrefabCallbackData<GameObject>;
      if (!(bool) prefabHandle)
      {
        if (!GeneralUtils.IsCallbackValid((Delegate) callbackData1.callerCallback))
          return;
        callbackData1.callerCallback(prefabRef, (GameObject) null, callbackData1.callerData);
      }
      else
        this.m_prefabInstantiator.InstantiatePrefab(prefabHandle, new PrefabInstantiatorCB<GameObject>(this.OnPrefabInstantiated<GameObject>), (object) callbackData1, callbackData1.callerOptions);
    }
  }

  private void OnPrefabInstantiated<T>(string prefabAddress, T instance, object callbackData)
  {
    AssetLoader.InstantiatePrefabCallbackData<T> prefabCallbackData = callbackData as AssetLoader.InstantiatePrefabCallbackData<T>;
    if (!GeneralUtils.IsCallbackValid((Delegate) prefabCallbackData.callerCallback))
      return;
    prefabCallbackData.callerCallback(prefabCallbackData.requestedAssetRef, instance, prefabCallbackData.callerData);
  }

  private T LoadObjectImmediately<T>(AssetReference requestedAsset, Asset resolvedAsset) where T : UnityEngine.Object
  {
    string guid = resolvedAsset?.GetGuid();
    if (string.IsNullOrEmpty(guid))
      return default (T);
    AssetHandle<T> loadedAsset = this.m_assetManager.Load<T>(guid, false);
    this.OnAssetLoaded<T>(requestedAsset, guid, loadedAsset);
    return loadedAsset.Asset;
  }

  private IEnumerator InstantiateAndWaitThenCallGameObjectCallback(
    AssetReference assetRef,
    GameObject prefab,
    bool usePrefabPosition,
    GameObjectCallback callback,
    object callbackData)
  {
    if ((UnityEngine.Object) prefab == (UnityEngine.Object) null)
    {
      if (GeneralUtils.IsCallbackValid((Delegate) callback))
        callback(assetRef, (GameObject) null, callbackData);
    }
    else
    {
      GameObject instance = usePrefabPosition ? UnityEngine.Object.Instantiate<GameObject>(prefab) : UnityEngine.Object.Instantiate<GameObject>(prefab, this.NewGameObjectSpawnPosition(), prefab.transform.rotation);
      this.m_waitingOnObjects.Add(instance);
      yield return (object) new WaitForEndOfFrame();
      this.m_waitingOnObjects.Remove(instance);
      if (GeneralUtils.IsCallbackValid((Delegate) callback))
        callback(assetRef, instance, callbackData);
    }
  }

  private Vector3 NewGameObjectSpawnPosition() => (UnityEngine.Object) Camera.main == (UnityEngine.Object) null ? Vector3.zero : Camera.main.transform.position + this.SPAWN_POS_CAMERA_OFFSET;

  private static void LogMissingAsset(
    AssetReference requestedAsset,
    string resolvedGuid,
    string assetType)
  {
    AssetLoader.SendMissingAssetTelemetry(requestedAsset, resolvedGuid, assetType);
    Log.MissingAssets.PrintError(string.Format("{0} {1} not found", (object) assetType, (object) requestedAsset?.GetLegacyAssetName()));
  }

  private static void SendMissingAssetTelemetry(
    AssetReference requestedAsset,
    string resolvedGuid,
    string assetType)
  {
    if (string.IsNullOrEmpty(requestedAsset?.guid))
      Log.Telemetry.Print("Missing asset was found, but there was not way to identify it.  No telemetry will be sent.");
    else if (Application.isEditor)
      Log.Telemetry.Print("Missing asset found in editor - not sending missing asset telemetry for requestedGuid={0}, resolvedGuid={1}, name={2}", (object) requestedAsset.guid, (object) resolvedGuid, (object) requestedAsset.GetLegacyAssetName());
    else
      TelemetryManager.Client().SendAssetNotFound(assetType, requestedAsset.guid, resolvedGuid, requestedAsset.GetLegacyAssetName());
  }

  private static void SendSharedPrefabHandleOrphanedTelemetry(string asset, string owner) => TelemetryManager.Client().SendAssetOrphaned(asset ?? string.Empty, owner ?? string.Empty, "prefab_instance");

  private static void SendAssetHandleOrphanedTelemetry(string asset, string owner) => TelemetryManager.Client().SendAssetOrphaned(asset ?? string.Empty, owner ?? string.Empty, nameof (asset));

  private class InstantiatePrefabCallbackData<T>
  {
    public AssetReference requestedAssetRef;
    public AssetLoadingOptions callerOptions;
    public PrefabCallback<T> callerCallback;
    public object callerData;
  }
}
