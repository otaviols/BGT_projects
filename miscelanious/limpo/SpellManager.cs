using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : IService
{
  private static Dictionary<int, Pool<Spell>> m_spellPools = new Dictionary<int, Pool<Spell>>();
  private static Dictionary<int, Transform> m_spellPoolParents = new Dictionary<int, Transform>();
  private SpellCache m_spellCache;
  private HashSet<int> m_spellPoolingSet = new HashSet<int>();
  private Vector3 m_rootTransformPosition = new Vector3(100000f, 10000f, 10000f);

  public static SpellManager Get()
  {
    SpellManager service;
    return !ServiceManager.TryGet<SpellManager>(out service) && !Application.isEditor ? (SpellManager) null : service;
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    SpellManager spellManager = this;
    yield return (IAsyncJobResult) new ServiceSoftDependency(typeof (SceneMgr), serviceLocator);
    SceneMgr service;
    if (serviceLocator.TryGetService<SceneMgr>(out service))
      service.RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(spellManager.OnScenePreLoad));
    spellManager.m_spellCache = new SpellCache();
    spellManager.BuildManifestPoolingSet();
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (IAssetLoader)
  };

  public void Shutdown() => this.Clear();

  public void Clear()
  {
    SpellManager.m_spellPools.Clear();
    this.m_spellCache.Clear();
    foreach (KeyValuePair<int, Transform> spellPoolParent in SpellManager.m_spellPoolParents)
      UnityEngine.Object.Destroy((UnityEngine.Object) spellPoolParent.Value.gameObject);
    SpellManager.m_spellPoolParents.Clear();
  }

  private void BuildManifestPoolingSet() => this.BuildManifestPoolingSet(SpellPoolingManifest.PoolingEnabledSpells);

  public void BuildManifestPoolingSet(string[] spellPoolingManifest)
  {
    this.m_spellPoolingSet.Clear();
    foreach (object obj in spellPoolingManifest)
      this.m_spellPoolingSet.Add(obj.GetHashCode());
  }

  private void PreloadSpells(SpellPreloadConfiguration configuration)
  {
    if (configuration == null)
      return;
    foreach (SpellConfiguration spellConfiguration in configuration.SpellsToPreload)
    {
      string spellAssetRef = spellConfiguration.SpellAssetRef;
      int hashCode = spellAssetRef.GetHashCode();
      this.GetSpellPool(spellAssetRef, hashCode, 5, spellConfiguration.MaxPoolSize, spellConfiguration.PoolPrepopulateCount);
    }
    foreach (SpellPreloadMode subConfiguration in configuration.SubConfigurations)
      this.PreloadSpells(this.GetPreloadConfig(subConfiguration));
  }

  private SpellPreloadConfiguration GetPreloadConfig(SpellPreloadMode mode)
  {
    SpellPreloadConfiguration preloadConfiguration;
    return !SpellPreloadManifest.PreloadManifest.TryGetValue(mode, out preloadConfiguration) ? (SpellPreloadConfiguration) null : preloadConfiguration;
  }

  public Spell GetSpell(string spellAssetRef, bool useCache = false)
  {
    int hashCode = spellAssetRef.GetHashCode();
    if (!this.ShouldPoolSpell(hashCode))
    {
      SpellStatistics.IncreaseUnpooledSpellAcquisitionCount(spellAssetRef);
      return this.CreateSpell(spellAssetRef, false, useCache: useCache);
    }
    Pool<Spell> spellPool;
    if (!SpellManager.m_spellPools.TryGetValue(hashCode, out spellPool))
      spellPool = this.GetSpellPool(spellAssetRef, hashCode, prePopulationCount: 3);
    SpellStatistics.IncreasePooledSpellAcquisitionCount(hashCode);
    SpellStatistics.CheckPoolSizeStats(hashCode, spellPool);
    return spellPool.Acquire();
  }

  public Spell GetSpell(Spell spell)
  {
    Pool<Spell> spellPool;
    if (!spell.IsPooled || !SpellManager.m_spellPools.TryGetValue(spell.PrefabHash, out spellPool))
      return this.CloneSpell(spell);
    SpellStatistics.IncreasePooledSpellAcquisitionCount(spell.PrefabHash);
    SpellStatistics.CheckPoolSizeStats(spell.PrefabHash, spellPool);
    return spellPool.Acquire();
  }

  private Spell CloneSpell(Spell spell)
  {
    SpellStatistics.IncreaseUnpooledSpellAcquisitionCount(spell.name);
    return UnityEngine.Object.Instantiate<Spell>(spell);
  }

  public bool ReleaseSpell(Spell spell, bool resetNonPooled = false)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      Error.AddDevWarningNonRepeating("Spell Pooling", "Spell was null and could not be released.");
      return false;
    }
    Pool<Spell> pool;
    if (spell.IsPooled && SpellManager.m_spellPools.TryGetValue(spell.PrefabHash, out pool) && pool.Release(spell))
    {
      this.SetSpellsParentToPoolSceneObject(spell, spell.PrefabHash);
      spell.ReleaseSpell();
      return true;
    }
    if (resetNonPooled)
      spell.ReleaseSpell();
    SpellManager.DestroySpell(spell);
    return true;
  }

  public SpellTable GetSpellTable(string prefabPath) => this.m_spellCache.GetSpellTable(prefabPath);

  private void OnScenePreLoad(SceneMgr.Mode previousMode, SceneMgr.Mode nextMode, object userData)
  {
    if (previousMode == SceneMgr.Mode.GAMEPLAY && nextMode != SceneMgr.Mode.GAMEPLAY)
      this.Clear();
    switch (nextMode)
    {
      case SceneMgr.Mode.HUB:
        this.Clear();
        break;
      case SceneMgr.Mode.GAMEPLAY:
        if (previousMode != SceneMgr.Mode.BACON)
        {
          if (previousMode == SceneMgr.Mode.LETTUCE_MAP)
          {
            this.PreloadSpells(this.GetPreloadConfig(SpellPreloadMode.MERCENARIES));
            break;
          }
          this.PreloadSpells(this.GetPreloadConfig(SpellPreloadMode.DEFAULT_GAMEPLAY));
          break;
        }
        this.PreloadSpells(this.GetPreloadConfig(SpellPreloadMode.BATTLEGROUNDS));
        break;
      case SceneMgr.Mode.COLLECTIONMANAGER:
      case SceneMgr.Mode.TAVERN_BRAWL:
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        this.PreloadSpells(this.GetPreloadConfig(SpellPreloadMode.DEFAULT_COLLECTION));
        break;
    }
  }

  private Pool<Spell> GetSpellPool(
    string spellAssetRef,
    int spellPrefabHash,
    int extensionCount = 3,
    int maxSize = 14,
    int prePopulationCount = 0)
  {
    if (SpellManager.m_spellPools.ContainsKey(spellPrefabHash))
      return SpellManager.m_spellPools[spellPrefabHash];
    Pool<Spell> spellPool = new Pool<Spell>((Pool<Spell>.CreateItemCallback) (_ => this.CreateSpell(spellAssetRef, true, spellPrefabHash)), new Pool<Spell>.DestroyItemCallback(SpellManager.DestroySpell), extensionCount, maxSize);
    SpellStatistics.LogNewPool(spellPrefabHash, spellAssetRef);
    SpellManager.m_spellPools.Add(spellPrefabHash, spellPool);
    spellPool.SetMaxReleasedItemCount(maxSize);
    spellPool.AddFreeItems(prePopulationCount);
    return spellPool;
  }

  private Spell CreateSpell(
    string spellAssetRef,
    bool isPooled,
    int prefabHash = -1,
    bool useCache = true)
  {
    Spell spell = (Spell) null;
    if (useCache)
    {
      spell = this.m_spellCache.GetSpell(spellAssetRef);
    }
    else
    {
      GameObject gameObject = AssetLoader.Get()?.InstantiatePrefab((AssetReference) spellAssetRef);
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        spell = gameObject.GetComponent<Spell>();
    }
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("Spell Manager", "Spell could not be found with spellAssetRef '{0}'", (object) spellAssetRef);
      return (Spell) null;
    }
    SpellStatistics.AddSpell(spell);
    if (isPooled)
    {
      spell.InitializePooledSpell(prefabHash);
      this.SetSpellsParentToPoolSceneObject(spell, prefabHash);
      spell.ForceDeactivate();
    }
    return spell;
  }

  private static void DestroySpell(Spell spell)
  {
    SpellStatistics.RemoveSpell(spell);
    spell.ResetSpellHash();
    UnityEngine.Object.Destroy((UnityEngine.Object) spell.gameObject);
  }

  private bool ShouldPoolSpell(int spellPrefabHash) => this.m_spellPoolingSet.Contains(spellPrefabHash);

  private void SetSpellsParentToPoolSceneObject(Spell spell, int spellPrefabHash)
  {
    Transform newParentTransform;
    if (!SpellManager.m_spellPoolParents.TryGetValue(spellPrefabHash, out newParentTransform))
    {
      newParentTransform = this.CreateNewParentTransform(spell.name);
      SpellManager.m_spellPoolParents.Add(spellPrefabHash, newParentTransform.transform);
    }
    else if ((UnityEngine.Object) newParentTransform == (UnityEngine.Object) null)
    {
      newParentTransform = this.CreateNewParentTransform(spell.name);
      SpellManager.m_spellPoolParents[spellPrefabHash] = newParentTransform.transform;
    }
    TransformUtil.AttachAndPreserveLocalTransform(spell.transform, newParentTransform.transform);
  }

  private Transform CreateNewParentTransform(string name)
  {
    GameObject gameObject = new GameObject(name + "_Pool", new System.Type[1]
    {
      typeof (HSDontDestroyOnLoad)
    });
    gameObject.transform.position = this.m_rootTransformPosition;
    gameObject.SetActive(false);
    return gameObject.transform;
  }
}
