using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptableAssetCatalog<T> : ScriptableObject where T : BaseAssetCatalogItem, new()
{
  [SerializeField]
  public int m_TotalAssets = 230000;
  [SerializeField]
  public List<T> m_assets = new List<T>();
  [SerializeField]
  public List<string> m_bundleNames = new List<string>();

  public bool TryAddAsset(string guid, string bundleName)
  {
    List<T> assets = this.m_assets;
    T obj = new T();
    obj.guid = guid;
    obj.bundleId = string.IsNullOrEmpty(bundleName) ? -1 : this.GetOrAssignBundleId(bundleName);
    assets.Add(obj);
    return true;
  }

  protected int GetOrAssignBundleId(string bundleName)
  {
    int orAssignBundleId = this.m_bundleNames.IndexOf(bundleName);
    if (orAssignBundleId >= 0)
      return orAssignBundleId;
    this.m_bundleNames.Add(bundleName);
    return this.m_bundleNames.Count - 1;
  }
}
