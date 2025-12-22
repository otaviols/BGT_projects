using Blizzard.T5.AssetManager;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class PrefabInstantiator : IPrefabInstantiator
{
  private readonly Vector3 SPAWN_POS_CAMERA_OFFSET = new Vector3(0.0f, 0.0f, -5000f);
  private readonly List<GameObject> m_waitingOnObjects = new List<GameObject>();
  private readonly AssetHandleCollection m_sharedPrefabHandles;
  private readonly Dictionary<string, GameObject> m_sharedPrefabInstances = new Dictionary<string, GameObject>();
  private readonly Dictionary<string, List<PrefabInstantiator.PendingRequest>> m_pendingSharedInstanceRequests = new Dictionary<string, List<PrefabInstantiator.PendingRequest>>();
  private readonly Blizzard.T5.Core.ILogger m_logger;
  private readonly ProfilerMarker s_releaseUnreferencedAssetsProfiler = new ProfilerMarker("PrefabInstantiator.ReleaseUnreferencedAssets");
  private readonly ProfilerMarker s_checkForDeadHandlesProfiler = new ProfilerMarker("PrefabInstantiator.CheckForDeadHandles");

  public PrefabInstantiator(Blizzard.T5.Core.ILogger logger)
  {
    this.m_logger = logger;
    this.m_sharedPrefabHandles = new AssetHandleCollection(logger);
    this.m_sharedPrefabHandles.OnLastHandleReleased += new Action<string>(this.OnSharedPrefabReleased);
  }

  public event Action<string, string> OnSharedPrefabHandleOrphaned
  {
    add => this.m_sharedPrefabHandles.OnOrphanedHandleDetected += value;
    remove => this.m_sharedPrefabHandles.OnOrphanedHandleDetected -= value;
  }

  public GameObject InstantiatePrefab(
    AssetHandle<GameObject> prefabAsset,
    AssetLoadingOptions options)
  {
    if (!(bool) prefabAsset)
      return (GameObject) null;
    GameObject gameObject = options.HasFlag((Enum) AssetLoadingOptions.IgnorePrefabPosition) ? (GameObject) UnityEngine.Object.Instantiate((UnityEngine.Object) (GameObject) prefabAsset, this.NewGameObjectSpawnPosition(), prefabAsset.Asset.transform.rotation) : (GameObject) UnityEngine.Object.Instantiate((UnityEngine.Object) (GameObject) prefabAsset);
    AssetHandle assetHandle = (AssetHandle) prefabAsset.Share();
    ServiceManager.Get<DisposablesCleaner>()?.Attach(gameObject, (IDisposable) assetHandle);
    return gameObject;
  }

  public bool InstantiatePrefab(
    AssetHandle<GameObject> prefabAsset,
    PrefabInstantiatorCB<GameObject> callback,
    object callbackData,
    AssetLoadingOptions options)
  {
    if (!(bool) prefabAsset)
    {
      if (GeneralUtils.IsCallbackValid((Delegate) callback))
        callback((string) null, (GameObject) null, callbackData);
      return false;
    }
    this.InstantiateAndWaitThenCallGameObjectCallback(prefabAsset.Share(), options, callback, callbackData).Forget();
    return true;
  }

  public AssetHandle<GameObject> GetOrInstantiateSharedPrefab(
    AssetHandle<GameObject> prefabAsset,
    AssetLoadingOptions options)
  {
    if (!(bool) prefabAsset)
      return (AssetHandle<GameObject>) null;
    GameObject instance;
    if (!this.TryGetSharedInstance(prefabAsset.AssetAddress, out instance))
    {
      instance = this.InstantiatePrefab(prefabAsset, options);
      if ((UnityEngine.Object) instance != (UnityEngine.Object) null)
        this.m_sharedPrefabInstances.Add(prefabAsset.AssetAddress, instance);
    }
    if ((UnityEngine.Object) instance == (UnityEngine.Object) null)
      return (AssetHandle<GameObject>) null;
    AssetHandle<GameObject> handleToTrack = new AssetHandle<GameObject>(prefabAsset.AssetAddress, instance);
    this.m_sharedPrefabHandles.StartTrackingHandle((AssetHandle) handleToTrack);
    return handleToTrack;
  }

  public bool IsWaitingOnObject(GameObject go) => this.m_waitingOnObjects.Contains(go);

  public bool IsSharedPrefabInstance(GameObject go)
  {
    if (!(bool) (UnityEngine.Object) go)
      return false;
    foreach (GameObject gameObject in this.m_sharedPrefabInstances.Values)
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) gameObject)
        return true;
    }
    return false;
  }

  public void ReleaseUnreferencedAssets() => this.m_sharedPrefabHandles.ReleaseUnreferencedAssets();

  public bool CheckForDeadHandles(double maximumProcessingTimeMilleseconds = 1000.0)
  {
    using (this.s_checkForDeadHandlesProfiler.Auto())
      return this.m_sharedPrefabHandles.CheckForDeadHandles(maximumProcessingTimeMilleseconds);
  }

  private async UniTaskVoid InstantiateAndWaitThenCallGameObjectCallback(
    AssetHandle<GameObject> prefabHandle,
    AssetLoadingOptions options,
    PrefabInstantiatorCB<GameObject> callback,
    object callbackData)
  {
    using (prefabHandle)
    {
      GameObject instance = options.HasFlag((Enum) AssetLoadingOptions.IgnorePrefabPosition) ? UnityEngine.Object.Instantiate<GameObject>(prefabHandle.Asset, this.NewGameObjectSpawnPosition(), prefabHandle.Asset.transform.rotation) : UnityEngine.Object.Instantiate<GameObject>(prefabHandle.Asset);
      ServiceManager.Get<DisposablesCleaner>()?.Attach(instance, (IDisposable) prefabHandle.Share());
      this.m_waitingOnObjects.Add(instance);
      await UniTask.WaitForEndOfFrame();
      this.m_waitingOnObjects.Remove(instance);
      if (GeneralUtils.IsCallbackValid((Delegate) callback))
        callback(prefabHandle.AssetAddress, instance, callbackData);
      instance = (GameObject) null;
    }
  }

  private void OnSharedPrefabReleased(string prefabAddress)
  {
    GameObject instance;
    if (!this.TryGetSharedInstance(prefabAddress, out instance))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) instance);
    this.m_sharedPrefabInstances.Remove(prefabAddress);
  }

  private bool TryGetSharedInstance(string prefabAddress, out GameObject instance)
  {
    if (!this.m_sharedPrefabInstances.TryGetValue(prefabAddress, out instance))
      return false;
    if ((bool) (UnityEngine.Object) instance)
      return true;
    instance = (GameObject) null;
    this.m_logger.Log(Blizzard.T5.Core.LogLevel.Warning, "PrefabInstantiator found destroyed shared instance. This is unexpected.", (object) prefabAddress);
    this.m_sharedPrefabInstances.Remove(prefabAddress);
    return false;
  }

  private Vector3 NewGameObjectSpawnPosition() => (UnityEngine.Object) Camera.main == (UnityEngine.Object) null ? Vector3.zero : Camera.main.transform.position + this.SPAWN_POS_CAMERA_OFFSET;

  private class PendingRequest
  {
  }
}
