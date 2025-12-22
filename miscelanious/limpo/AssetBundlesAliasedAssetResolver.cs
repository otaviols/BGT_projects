using Hearthstone.Core;
using System.IO;
using UnityEngine;

public class AssetBundlesAliasedAssetResolver
{
  private ScriptableAssetMap m_cardsMap;
  private ScriptableAssetMap m_spriteAtlasMap;

  public void Shutdown() => this.m_cardsMap = (ScriptableAssetMap) null;

  public AssetReference GetCardDefAssetRefFromCardId(string cardId)
  {
    this.InitCardsMapIfNeeded();
    return AssetBundlesAliasedAssetResolver.Resolve(cardId, this.m_cardsMap);
  }

  public AssetReference GetSpriteAtlasAssetRefFromTag(string atlasTag)
  {
    this.InitSpriteAtlasMapIfNeeded();
    return AssetBundlesAliasedAssetResolver.Resolve(atlasTag, this.m_spriteAtlasMap);
  }

  private static AssetReference Resolve(string alias, ScriptableAssetMap assetMap)
  {
    if ((Object) assetMap == (Object) null || assetMap.map == null)
    {
      Log.Asset.PrintError("[AssetBundlesAliasedAssetResolver] Cannot resolve {0}. Missing map", (object) alias);
      return (AssetReference) null;
    }
    string assetString;
    if (assetMap.map.TryGetValue(alias, out assetString))
      return AssetReference.CreateFromAssetString(assetString);
    Log.Asset.PrintError("[AssetBundlesAliasedAssetResolver] Cannot resolve {0} among {1} entries", (object) alias, (object) assetMap.map.Count);
    return (AssetReference) null;
  }

  private void InitCardsMapIfNeeded()
  {
    if (!((Object) this.m_cardsMap == (Object) null))
      return;
    this.LoadFromBundle();
  }

  private void InitSpriteAtlasMapIfNeeded()
  {
    if (!((Object) this.m_spriteAtlasMap == (Object) null))
      return;
    this.LoadFromBundle();
  }

  public void LoadFromBundle()
  {
    string assetBundlePath = AssetBundleInfo.GetAssetBundlePath(ScriptableAssetManifest.MainManifestBundleName);
    if (!File.Exists(assetBundlePath))
    {
      Log.Asset.PrintError("[AssetBundlesAliasedAssetResolver] Cannot find asset bundle for ScriptableAssetMaps '{0}', editor {1}, playing {2}", (object) assetBundlePath, (object) Application.isEditor, (object) Application.isPlaying);
    }
    else
    {
      AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
      if ((Object) assetBundle == (Object) null)
      {
        Log.Asset.PrintError("[AssetBundlesAliasedAssetResolver] Failed to open manifest bundle at {0}", (object) assetBundlePath);
      }
      else
      {
        this.m_cardsMap = assetBundle.LoadAsset<ScriptableAssetMap>("Assets/AssetManifest/AssetMaps/cards_map.asset");
        if ((Object) this.m_cardsMap == (Object) null)
          Error.AddDevFatal("Failed to load cards map at {0} from {1}", (object) "Assets/AssetManifest/AssetMaps/cards_map.asset", (object) assetBundlePath);
        this.m_spriteAtlasMap = assetBundle.LoadAsset<ScriptableAssetMap>("Assets/AssetManifest/AssetMaps/sprite_atlas_map.asset");
        if ((Object) this.m_spriteAtlasMap == (Object) null)
          Error.AddDevFatal("Failed to sprite atlas map at {0} from {1}", (object) "Assets/AssetManifest/AssetMaps/sprite_atlas_map.asset", (object) assetBundlePath);
        assetBundle.Unload(false);
      }
    }
  }
}
