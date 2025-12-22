using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInstanceLoadTracker : IService
{
  private Dictionary<PrefabInstanceLoadTracker.Context, List<GameObject>> m_TrackedPrefabs = new Dictionary<PrefabInstanceLoadTracker.Context, List<GameObject>>();

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
  }

  public static PrefabInstanceLoadTracker Get() => ServiceManager.Get<PrefabInstanceLoadTracker>();

  public GameObject InstantiatePrefab(
    PrefabInstanceLoadTracker.Context context,
    AssetReference assetRef,
    AssetLoadingOptions options = AssetLoadingOptions.None)
  {
    GameObject instance = AssetLoader.Get().InstantiatePrefab(assetRef, options);
    if ((UnityEngine.Object) instance != (UnityEngine.Object) null)
      this.AddInstantiatedPrefabToContext(context, instance);
    return instance;
  }

  public bool InstantiatePrefab(
    PrefabInstanceLoadTracker.Context context,
    AssetReference assetRef,
    PrefabCallback<GameObject> callback,
    object callbackData = null,
    AssetLoadingOptions options = AssetLoadingOptions.None)
  {
    PrefabInstanceLoadTracker.InstantiatePrefabCallbackData<GameObject> callbackData1 = new PrefabInstanceLoadTracker.InstantiatePrefabCallbackData<GameObject>()
    {
      context = context,
      callerCallback = callback,
      callerData = callbackData,
      requestedAssetRef = assetRef
    };
    return AssetLoader.Get().InstantiatePrefab(assetRef, new PrefabCallback<GameObject>(this.OnPrefabInstantiated), (object) callbackData1, options);
  }

  public void DestroyContext(PrefabInstanceLoadTracker.Context context)
  {
    if (!context.Active)
      return;
    context.MarkDestroyed();
    List<GameObject> gameObjectList;
    if (this.m_TrackedPrefabs.TryGetValue(context, out gameObjectList))
    {
      foreach (UnityEngine.Object @object in gameObjectList)
        UnityEngine.Object.Destroy(@object);
      this.m_TrackedPrefabs.Clear();
    }
    this.m_TrackedPrefabs.Remove(context);
  }

  private void OnPrefabInstantiated(
    AssetReference assetRef,
    GameObject instance,
    object callbackData)
  {
    PrefabInstanceLoadTracker.InstantiatePrefabCallbackData<GameObject> prefabCallbackData = callbackData as PrefabInstanceLoadTracker.InstantiatePrefabCallbackData<GameObject>;
    if (prefabCallbackData.context != null && !prefabCallbackData.context.Active)
    {
      if (!((UnityEngine.Object) instance != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) instance);
    }
    else
    {
      this.AddInstantiatedPrefabToContext(prefabCallbackData.context, instance);
      if (!GeneralUtils.IsCallbackValid((Delegate) prefabCallbackData.callerCallback))
        return;
      prefabCallbackData.callerCallback(prefabCallbackData.requestedAssetRef, instance, prefabCallbackData.callerData);
    }
  }

  private void AddInstantiatedPrefabToContext(
    PrefabInstanceLoadTracker.Context context,
    GameObject instance)
  {
    if (context == null || !((UnityEngine.Object) instance != (UnityEngine.Object) null))
      return;
    List<GameObject> gameObjectList;
    if (!this.m_TrackedPrefabs.TryGetValue(context, out gameObjectList))
    {
      gameObjectList = new List<GameObject>();
      this.m_TrackedPrefabs.Add(context, gameObjectList);
    }
    gameObjectList.Add(instance);
  }

  public class Context
  {
    public bool Active { private set; get; } = true;

    public void MarkDestroyed() => this.Active = false;
  }

  private class InstantiatePrefabCallbackData<T>
  {
    public PrefabInstanceLoadTracker.Context context;
    public AssetReference requestedAssetRef;
    public PrefabCallback<T> callerCallback;
    public object callerData;
  }
}
