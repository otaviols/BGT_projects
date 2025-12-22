using System;

[Serializable]
public class ScriptableAssetVariantCatalog : ScriptableAssetCatalog<VariantAssetCatalogItem>
{
  public bool TryAddVariant(string variantGuid, string variantBundle, string baseGuid)
  {
    if (!this.TryAddAsset(variantGuid, variantBundle))
      return false;
    this.m_assets[this.m_assets.Count - 1].baseGuid = baseGuid;
    return true;
  }
}
