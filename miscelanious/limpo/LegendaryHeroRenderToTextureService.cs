using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LegendaryHeroRenderToTextureService : IService
{
  private Dictionary<(string, Player.Side), GameObject> m_assetPathToObjectMap = new Dictionary<(string, Player.Side), GameObject>();
  private Dictionary<int, (string, Player.Side)> m_objectIDToAssetPathMap = new Dictionary<int, (string, Player.Side)>();
  private Dictionary<int, int> m_objectIDReferenceCounts = new Dictionary<int, int>();

  System.Type[] IService.GetDependencies() => (System.Type[]) null;

  IEnumerator<IAsyncJobResult> IService.Initialize(
    ServiceLocator serviceLocator)
  {
    LegendaryHeroRenderToTextureService toTextureService = this;
    SceneMgr service = (SceneMgr) null;
    while (!serviceLocator.TryGetService<SceneMgr>(out service))
      yield return (IAsyncJobResult) null;
    LegendarySkin.DynamicResolutionEnabled = true;
    LegendarySkin.DynamicResolutionScale = !(bool) UniversalInputManager.UsePhoneUI ? 1.5f : 1.1f;
    service.RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(toTextureService.OnSceneLoaded));
  }

  void IService.Shutdown()
  {
    foreach (KeyValuePair<(string, Player.Side), GameObject> assetPathToObject in this.m_assetPathToObjectMap)
      UnityEngine.Object.Destroy((UnityEngine.Object) assetPathToObject.Value);
    this.m_assetPathToObjectMap.Clear();
    this.m_objectIDToAssetPathMap.Clear();
    this.m_objectIDReferenceCounts.Clear();
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    foreach (KeyValuePair<(string, Player.Side), GameObject> assetPathToObject in this.m_assetPathToObjectMap)
    {
      GameObject gameObject = assetPathToObject.Value;
      if ((bool) (UnityEngine.Object) gameObject)
      {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
      }
    }
  }

  private GameObject GetOrCreateGameObject(string assetPath, Player.Side playerSide)
  {
    if (string.IsNullOrEmpty(assetPath))
      return (GameObject) null;
    GameObject target;
    if (this.m_assetPathToObjectMap.TryGetValue((assetPath, playerSide), out target))
    {
      this.m_objectIDReferenceCounts[target.GetInstanceID()]++;
    }
    else
    {
      target = AssetLoader.Get().InstantiatePrefab((AssetReference) assetPath);
      if ((UnityEngine.Object) target != (UnityEngine.Object) null)
      {
        UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) target);
        int instanceId = target.GetInstanceID();
        this.m_assetPathToObjectMap[(assetPath, playerSide)] = target;
        this.m_objectIDReferenceCounts[instanceId] = 1;
        this.m_objectIDToAssetPathMap[instanceId] = (assetPath, playerSide);
      }
    }
    return target;
  }

  private void ReleaseGameObject(GameObject gameObject)
  {
    int instanceId = gameObject.GetInstanceID();
    int num;
    if (!this.m_objectIDReferenceCounts.TryGetValue(instanceId, out num))
      return;
    if (num > 1)
    {
      this.m_objectIDReferenceCounts[instanceId] = num - 1;
    }
    else
    {
      (string, Player.Side) objectIdToAssetPath = this.m_objectIDToAssetPathMap[instanceId];
      this.m_objectIDReferenceCounts.Remove(instanceId);
      this.m_objectIDToAssetPathMap.Remove(instanceId);
      this.m_assetPathToObjectMap.Remove(objectIdToAssetPath);
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
    }
  }

  public ILegendaryHeroPortrait CreatePortrait(
    string assetPath,
    Player.Side playerSide)
  {
    return (ILegendaryHeroPortrait) new LegendaryHeroRenderToTextureService.HeroPortraitHandleInternal(this, assetPath, playerSide);
  }

  private class HeroPortraitHandleInternal : ILegendaryHeroPortrait, IDisposable
  {
    private readonly LegendaryHeroRenderToTextureService m_service;
    private readonly string m_assetPath;
    private readonly Player.Side m_playerSide;
    private readonly HashSet<LegendarySkinDynamicResController> m_dynamicResControllers;
    private GameObject m_legendaryPrefabInstance;
    private LegendarySkin m_legendarySkin;
    private ForwardEmoteEventsToFSM m_forwardEventsToFSM;

    public HeroPortraitHandleInternal(
      LegendaryHeroRenderToTextureService service,
      string assetPath,
      Player.Side playerSide)
    {
      this.m_service = service;
      this.m_assetPath = assetPath;
      this.m_playerSide = playerSide;
      this.m_dynamicResControllers = new HashSet<LegendarySkinDynamicResController>();
      this.m_legendaryPrefabInstance = this.m_service.GetOrCreateGameObject(assetPath, playerSide);
      this.m_legendarySkin = this.m_legendaryPrefabInstance.GetComponentInChildren<LegendarySkin>();
      this.m_forwardEventsToFSM = this.m_legendaryPrefabInstance.GetComponentInChildren<ForwardEmoteEventsToFSM>();
    }

    Texture ILegendaryHeroPortrait.PortraitTexture => this.m_legendarySkin?.PortraitTexture;

    bool ILegendaryHeroPortrait.IsValidForPath(
      string assetPath,
      Player.Side playerSide)
    {
      return this.m_assetPath == assetPath && this.m_playerSide == playerSide;
    }

    void ILegendaryHeroPortrait.AttachToActor(Actor actor)
    {
      if (!((UnityEngine.Object) this.m_forwardEventsToFSM != (UnityEngine.Object) null))
        return;
      this.m_forwardEventsToFSM.OnAttachedToActor(actor);
    }

    void ILegendaryHeroPortrait.RaiseAnimationEvent(string eventName)
    {
      if (!((UnityEngine.Object) this.m_forwardEventsToFSM != (UnityEngine.Object) null))
        return;
      this.m_forwardEventsToFSM.RaiseFSMEvent(eventName);
    }

    void ILegendaryHeroPortrait.RaiseEmoteAnimationEvent(EmoteType emote)
    {
      if (!((UnityEngine.Object) this.m_forwardEventsToFSM != (UnityEngine.Object) null))
        return;
      this.m_forwardEventsToFSM.EmotePlayCallback(emote);
    }

    void ILegendaryHeroPortrait.ClearDynamicResolutionControllers()
    {
      foreach (LegendarySkinDynamicResController dynamicResController in this.m_dynamicResControllers)
        dynamicResController.Skin = (LegendarySkin) null;
      this.m_dynamicResControllers.Clear();
    }

    void ILegendaryHeroPortrait.ConnectDynamicResolutionController(
      LegendarySkinDynamicResController controller)
    {
      if (!((UnityEngine.Object) controller != (UnityEngine.Object) null))
        return;
      this.m_dynamicResControllers.Add(controller);
      controller.Skin = this.m_legendarySkin;
    }

    void IDisposable.Dispose()
    {
      ((ILegendaryHeroPortrait) this).ClearDynamicResolutionControllers();
      if (!((UnityEngine.Object) this.m_legendaryPrefabInstance != (UnityEngine.Object) null))
        return;
      this.m_service.ReleaseGameObject(this.m_legendaryPrefabInstance);
      this.m_legendaryPrefabInstance = (GameObject) null;
    }
  }
}
