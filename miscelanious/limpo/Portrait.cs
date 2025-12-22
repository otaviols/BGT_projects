using Blizzard.T5.AssetManager;
using UnityEngine;

[CreateAssetMenu]
[CustomEditClass]
public class Portrait : ScriptableObject
{
  [CustomEditField(Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_PortraitTexturePath;
  [CustomEditField(Sections = "Portrait", T = EditType.MATERIAL)]
  public string m_PremiumPortraitMaterialPath;
  [CustomEditField(Sections = "Portrait", T = EditType.UBERANIMATION)]
  public string m_PremiumUberShaderAnimationPath;
  [CustomEditField(Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_PremiumPortraitTexturePath;
  [CustomEditField(Sections = "Portrait", T = EditType.MATERIAL)]
  public string m_DiamondPortraitMaterialPath;
  [CustomEditField(Sections = "Portrait", T = EditType.UBERANIMATION)]
  public string m_DiamondUberShaderAnimationPath;
  [CustomEditField(Sections = "Portrait", T = EditType.MESH)]
  public string m_DiamondPlaneRTT_Play;
  [CustomEditField(Sections = "Portrait", T = EditType.MESH)]
  public string m_DiamondPlaneRTT_Hand;
  [CustomEditField(Sections = "Portrait")]
  public Color m_DiamondPlaneRTT_CearColor = Color.clear;
  [CustomEditField(Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_DiamondPortraitTexturePath;
  [CustomEditField(Sections = "Portrait", T = EditType.GAME_OBJECT)]
  public string m_DiamondModel;
  [CustomEditField(Sections = "Portrait")]
  public Material m_DeckCardBarPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material m_EnchantmentPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material m_HistoryTileHalfPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material m_HistoryTileFullPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material m_LeaderboardTileFullPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material_MobileOverride m_CustomDeckPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material_MobileOverride m_DeckPickerPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material m_PracticeAIPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material m_DeckBoxPortrait;
  [CustomEditField(Sections = "Portrait")]
  public Material_MobileOverride m_ClassPickerPortrait;
  public CardPortraitQuality m_portraitQuality = CardPortraitQuality.GetUnloaded();
  public CardDefSpecialEvent m_currentSpecialEvent;
  private AssetHandle<Texture> m_loadedPortraitTexture;
  private AssetHandle<Texture> m_loadedPremiumPortraitTexture;
  private AssetHandle<Material> m_premiumMaterialHandle;
  private AssetHandle<UberShaderAnimation> m_premiumPortraitAnimation;
  private AssetHandle<Texture> m_lowQualityPortrait;

  public AssetHandle<Texture> LoadedPortraitTexture => this.m_loadedPortraitTexture;

  public Material m_LoadedPremiumPortraitMaterial { get; private set; }

  public Material m_LoadedDeckCardBarPortrait { get; private set; }

  public Material m_LoadedEnchantmentPortrait { get; private set; }

  public Material m_LoadedHistoryTileFullPortrait { get; private set; }

  public Material m_LoadedHistoryTileHalfPortrait { get; private set; }

  public Material m_LoadedLeaderboardTileFullPortrait { get; private set; }

  public Material m_LoadedCustomDeckPortrait { get; private set; }

  public Material m_LoadedDeckPickerPortrait { get; private set; }

  public Material m_LoadedPracticeAIPortrait { get; private set; }

  public Material m_LoadedDeckBoxPortrait { get; private set; }

  public AssetHandle<UberShaderAnimation> PremiumPortraitAnimation => this.m_premiumPortraitAnimation;

  public Material m_LoadedPremiumClassMaterial { get; set; }

  public Material m_LoadedClassPickerPortrait { get; set; }

  public void OnDestroy()
  {
    if ((bool) (Object) this.m_LoadedPremiumPortraitMaterial)
      Object.Destroy((Object) this.m_LoadedPremiumPortraitMaterial);
    if ((bool) (Object) this.m_LoadedDeckCardBarPortrait)
      Object.Destroy((Object) this.m_LoadedDeckCardBarPortrait);
    if ((bool) (Object) this.m_LoadedEnchantmentPortrait)
      Object.Destroy((Object) this.m_LoadedEnchantmentPortrait);
    if ((bool) (Object) this.m_LoadedHistoryTileFullPortrait)
      Object.Destroy((Object) this.m_LoadedHistoryTileFullPortrait);
    if ((bool) (Object) this.m_LoadedHistoryTileHalfPortrait)
      Object.Destroy((Object) this.m_LoadedHistoryTileHalfPortrait);
    if ((bool) (Object) this.m_LoadedLeaderboardTileFullPortrait)
      Object.Destroy((Object) this.m_LoadedLeaderboardTileFullPortrait);
    if ((bool) (Object) this.m_LoadedCustomDeckPortrait)
      Object.Destroy((Object) this.m_LoadedCustomDeckPortrait);
    if ((bool) (Object) this.m_LoadedDeckPickerPortrait)
      Object.Destroy((Object) this.m_LoadedDeckPickerPortrait);
    if ((bool) (Object) this.m_LoadedPracticeAIPortrait)
      Object.Destroy((Object) this.m_LoadedPracticeAIPortrait);
    if ((bool) (Object) this.m_LoadedDeckBoxPortrait)
      Object.Destroy((Object) this.m_LoadedDeckBoxPortrait);
    if ((bool) (Object) this.m_LoadedPremiumClassMaterial)
      Object.Destroy((Object) this.m_LoadedPremiumClassMaterial);
    if ((bool) (Object) this.m_LoadedClassPickerPortrait)
      Object.Destroy((Object) this.m_LoadedClassPickerPortrait);
    AssetHandle.SafeDispose<Texture>(ref this.m_loadedPortraitTexture);
    AssetHandle.SafeDispose<Texture>(ref this.m_loadedPremiumPortraitTexture);
    AssetHandle.SafeDispose<Material>(ref this.m_premiumMaterialHandle);
    AssetHandle.SafeDispose<UberShaderAnimation>(ref this.m_premiumPortraitAnimation);
    AssetHandle.SafeDispose<Texture>(ref this.m_lowQualityPortrait);
  }

  private void SetTextureIfNotNull(Material baseMat, Material targetMat, Texture tex)
  {
    if ((Object) baseMat == (Object) null)
      return;
    if ((Object) targetMat == (Object) null)
      targetMat = Object.Instantiate<Material>(baseMat);
    targetMat.mainTexture = tex;
  }

  public void OnPortraitLoaded(AssetHandle<Texture> portrait, int quality)
  {
    if (quality <= this.m_portraitQuality.TextureQuality)
    {
      Debug.LogWarning((object) string.Format("Loaded texture of quality lower or equal to what was was already available ({0} <= {1}), texture={2}", (object) quality, (object) this.m_portraitQuality, (object) portrait));
    }
    else
    {
      this.m_portraitQuality.TextureQuality = quality;
      if ((bool) this.m_loadedPortraitTexture)
        AssetHandle.Set<Texture>(ref this.m_lowQualityPortrait, this.m_loadedPortraitTexture);
      AssetHandle.Set<Texture>(ref this.m_loadedPortraitTexture, portrait);
      if ((Object) this.m_LoadedPremiumPortraitMaterial != (Object) null && string.IsNullOrEmpty(this.m_PremiumPortraitTexturePath))
      {
        this.m_LoadedPremiumPortraitMaterial.mainTexture = (Texture) portrait;
        this.m_portraitQuality.PremiumType = TAG_PREMIUM.GOLDEN;
        if ((Object) this.m_LoadedClassPickerPortrait != (Object) null)
          this.m_LoadedClassPickerPortrait.mainTexture = (Texture) portrait;
      }
      this.SetTextureIfNotNull(this.m_DeckCardBarPortrait, this.m_LoadedDeckCardBarPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull(this.m_EnchantmentPortrait, this.m_LoadedEnchantmentPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull(this.m_HistoryTileFullPortrait, this.m_LoadedHistoryTileFullPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull(this.m_HistoryTileHalfPortrait, this.m_LoadedHistoryTileHalfPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull(this.m_LeaderboardTileFullPortrait, this.m_LoadedLeaderboardTileFullPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull((Material) (MobileOverrideValue<Material>) this.m_CustomDeckPortrait, this.m_LoadedCustomDeckPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull((Material) (MobileOverrideValue<Material>) this.m_DeckPickerPortrait, this.m_LoadedDeckPickerPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull(this.m_PracticeAIPortrait, this.m_LoadedPracticeAIPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull(this.m_DeckBoxPortrait, this.m_LoadedDeckBoxPortrait, (Texture) this.m_loadedPortraitTexture);
      this.SetTextureIfNotNull((Material) (MobileOverrideValue<Material>) this.m_ClassPickerPortrait, this.m_LoadedClassPickerPortrait, (Texture) this.m_loadedPortraitTexture);
    }
  }

  public void OnPremiumMaterialLoaded(
    AssetHandle<Material> material,
    AssetHandle<Texture> portrait,
    AssetHandle<UberShaderAnimation> portraitAnimation)
  {
    if ((Object) this.m_LoadedPremiumPortraitMaterial != (Object) null)
    {
      if (!Application.isPlaying)
        return;
      Debug.LogWarning((object) string.Format("Loaded premium material twice: {0}", (object) material));
    }
    else
    {
      if ((bool) material)
      {
        AssetHandle.Set<Material>(ref this.m_premiumMaterialHandle, material);
        this.m_LoadedPremiumPortraitMaterial = (Material) Object.Instantiate((Object) (Material) material);
      }
      AssetHandle.Set<UberShaderAnimation>(ref this.m_premiumPortraitAnimation, portraitAnimation);
      if ((bool) this.m_loadedPortraitTexture)
      {
        if ((Object) this.m_LoadedPremiumPortraitMaterial != (Object) null)
          this.m_LoadedPremiumPortraitMaterial.mainTexture = (Texture) this.m_loadedPortraitTexture;
        this.m_portraitQuality.PremiumType = TAG_PREMIUM.GOLDEN;
      }
      if (!(bool) portrait)
        return;
      AssetHandle.Set<Texture>(ref this.m_loadedPremiumPortraitTexture, portrait);
      if ((Object) this.m_LoadedPremiumPortraitMaterial != (Object) null)
        this.m_LoadedPremiumPortraitMaterial.mainTexture = (Texture) portrait;
      this.m_portraitQuality.PremiumType = TAG_PREMIUM.GOLDEN;
    }
  }

  public bool IsPremiumLoaded() => this.m_portraitQuality.PremiumType == TAG_PREMIUM.GOLDEN;

  public AssetReference GetPortraitRef() => this.m_currentSpecialEvent != null && !string.IsNullOrEmpty(this.m_currentSpecialEvent.m_PortraitTextureOverride) ? (AssetReference) this.m_currentSpecialEvent.m_PortraitTextureOverride : (AssetReference) this.m_PortraitTexturePath;

  public AssetReference GetPremiumMaterialRef() => this.m_currentSpecialEvent != null && !string.IsNullOrEmpty(this.m_currentSpecialEvent.m_PremiumPortraitMaterialOverride) ? (AssetReference) this.m_currentSpecialEvent.m_PremiumPortraitMaterialOverride : (AssetReference) this.m_PremiumPortraitMaterialPath;

  public AssetReference GetPremiumPortraitRef() => this.m_currentSpecialEvent != null && !string.IsNullOrEmpty(this.m_currentSpecialEvent.m_PremiumPortraitTextureOverride) ? (AssetReference) this.m_currentSpecialEvent.m_PremiumPortraitTextureOverride : (AssetReference) this.m_PremiumPortraitTexturePath;

  public AssetReference GetPremiumAnimationRef() => this.m_currentSpecialEvent != null && !string.IsNullOrEmpty(this.m_currentSpecialEvent.m_PremiumUberShaderAnimationOverride) ? (AssetReference) this.m_currentSpecialEvent.m_PremiumUberShaderAnimationOverride : (AssetReference) this.m_PremiumUberShaderAnimationPath;
}
