using System;
using UnityEngine;

[Serializable]
public class ScriptableAssetMap : ScriptableObject
{
  public const string ASSET_MAPS_PATH = "Assets/AssetManifest/AssetMaps/";
  public const string CARDS_MAP_PATH = "Assets/AssetManifest/AssetMaps/cards_map.asset";
  public const string SPRITE_ATLAS_MAP_PATH = "Assets/AssetManifest/AssetMaps/sprite_atlas_map.asset";
  public ScriptableAssetMap.SerializableMap map = new ScriptableAssetMap.SerializableMap();

  [Serializable]
  public class SerializableMap : SerializableDictionary<string, string>
  {
  }
}
