using System.Collections.Generic;
using UnityEngine;

internal class SpellCache
{
  private Dictionary<string, SpellTable> m_spellTableCache = new Dictionary<string, SpellTable>();
  private Dictionary<int, Spell> m_spellCache = new Dictionary<int, Spell>();
  private GameObject m_sceneObject;

  private GameObject SceneObject
  {
    get
    {
      if ((UnityEngine.Object) this.m_sceneObject == (UnityEngine.Object) null)
      {
        this.m_sceneObject = new GameObject("SpellCacheSceneObject", new System.Type[1]
        {
          typeof (HSDontDestroyOnLoad)
        });
        this.m_sceneObject.SetActive(false);
      }
      return this.m_sceneObject;
    }
  }

  public SpellTable GetSpellTable(string prefabPath)
  {
    SpellTable spellTable;
    if (!this.m_spellTableCache.TryGetValue(prefabPath, out spellTable))
      spellTable = this.LoadSpellTable(prefabPath);
    return spellTable;
  }

  public Spell GetSpell(string spellPrefabName)
  {
    int hashCode = spellPrefabName.GetHashCode();
    Spell original;
    if (!this.m_spellCache.TryGetValue(hashCode, out original))
      original = this.LoadSpell(spellPrefabName, hashCode);
    return UnityEngine.Object.Instantiate<Spell>(original);
  }

  public void Clear()
  {
    foreach (KeyValuePair<string, SpellTable> keyValuePair in this.m_spellTableCache)
      keyValuePair.Value.ReleaseAllSpells();
    foreach (KeyValuePair<int, Spell> keyValuePair in this.m_spellCache)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) keyValuePair.Value.gameObject);
    this.m_spellCache.Clear();
  }

  private SpellTable LoadSpellTable(string prefabPath)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) prefabPath, AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("SpellCache.LoadSpellTable() - SpellCache GameObject failed to load");
      return (SpellTable) null;
    }
    SpellTable component = gameObject.GetComponent<SpellTable>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("SpellCache.LoadSpellTable() - SpellCache has no SpellTable component ");
      return (SpellTable) null;
    }
    component.transform.parent = this.SceneObject.transform;
    this.m_spellTableCache.Add(prefabPath, component);
    return component;
  }

  private Spell LoadSpell(string prefabPath, int prefabHash)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) prefabPath);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("SpellCache.LoadSpell() - Spell GameObject failed to load");
      return (Spell) null;
    }
    Spell component = gameObject.GetComponent<Spell>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("SpellCache.LoadSpell() - GameObject has no Spell component ");
      return (Spell) null;
    }
    TransformUtil.AttachAndPreserveLocalTransform(component.transform, this.SceneObject.transform);
    component.Hide();
    this.m_spellCache.Add(prefabHash, component);
    return component;
  }
}
