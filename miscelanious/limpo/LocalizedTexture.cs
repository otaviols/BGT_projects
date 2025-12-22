using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

[CustomEditClass]
public class LocalizedTexture : MonoBehaviour
{
  [CustomEditField(T = EditType.TEXTURE)]
  public string m_textureName;
  private AssetHandle<Texture> m_loadedTexture;

  private void Awake()
  {
    if (string.IsNullOrEmpty(this.m_textureName))
      Debug.LogWarningFormat("LocalizedTexture: skipping load for empty texture! go={0}", (object) this.gameObject);
    else
      AssetLoader.Get().LoadAsset<Texture>((AssetReference) this.m_textureName, new AssetHandleCallback<Texture>(this.OnTextureLoaded));
  }

  private void OnDestroy() => AssetHandle.SafeDispose<Texture>(ref this.m_loadedTexture);

  private void OnTextureLoaded(
    AssetReference assetRef,
    AssetHandle<Texture> texture,
    object callbackData)
  {
    AssetHandle.Take<Texture>(ref this.m_loadedTexture, texture);
    if (this.m_loadedTexture == null)
    {
      if (PlatformSettings.LocaleVariant != LocaleVariant.China && Localization.GetLocale() != Locale.enUS)
        AssetLoader.Get().LoadAsset<Texture>(ref this.m_loadedTexture, (AssetReference) this.m_textureName, AssetLoadingOptions.DisableLocalization);
      if (this.m_loadedTexture == null)
      {
        Debug.LogErrorFormat("Failed to load LocalizedTexture: go={0}, assetRef={1}", (object) this.gameObject, (object) assetRef);
        return;
      }
    }
    RendererExtension.GetMaterial(this.GetComponent<Renderer>()).mainTexture = (Texture) this.m_loadedTexture;
  }
}
