using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AliasedAssetResolver : IAliasedAssetResolver, IService
{
  private readonly AssetBundlesAliasedAssetResolver m_assetBundleResolver = new AssetBundlesAliasedAssetResolver();

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (IAssetLoader)
  };

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    if (AssetLoaderPrefs.AssetLoadingMethod == AssetLoaderPrefs.ASSET_LOADING_METHOD.ASSET_BUNDLES)
    {
      this.m_assetBundleResolver.LoadFromBundle();
      yield break;
    }
  }

  public void Shutdown() => this.m_assetBundleResolver.Shutdown();

  public AssetReference GetCardDefAssetRefFromCardId(string cardId)
  {
    AssetReference assetRefFromCardId = this.m_assetBundleResolver.GetCardDefAssetRefFromCardId(cardId);
    if (assetRefFromCardId != null)
      return assetRefFromCardId;
    AliasedAssetResolver.SendMissingAssetTelemetry(typeof (CardDef), nameof (cardId), cardId, "prefab");
    return assetRefFromCardId;
  }

  public AssetReference GetSpriteAtlasAssetRefFromTag(string atlasTag)
  {
    AssetReference atlasAssetRefFromTag = this.m_assetBundleResolver.GetSpriteAtlasAssetRefFromTag(atlasTag);
    if (atlasAssetRefFromTag != null)
      return atlasAssetRefFromTag;
    AliasedAssetResolver.SendMissingAssetTelemetry(typeof (SpriteAtlas), nameof (atlasTag), atlasTag, "spriteatlas");
    return atlasAssetRefFromTag;
  }

  private static void SendMissingAssetTelemetry(
    System.Type assetType,
    string idLabel,
    string id,
    string fileExtension,
    string filePath = "")
  {
    if (Application.isEditor)
      Log.Telemetry.Print("Missing " + assetType.Name + " in editor - not sending missing asset telemetry for " + idLabel + "=" + id + ", extension=" + fileExtension + ", filepath=" + (string.IsNullOrEmpty(filePath) ? "unknown" : filePath));
    else
      TelemetryManager.Client().SendAssetNotFound(assetType.Name, string.Empty, filePath, id + "." + fileExtension);
  }
}
