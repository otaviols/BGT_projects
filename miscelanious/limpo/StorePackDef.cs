using UnityEngine;

[CustomEditClass]
public class StorePackDef : MonoBehaviour, IStorePackDef
{
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_buttonPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_lowPolyPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_lowPolyPrefabDust;
  [CustomEditField(T = EditType.TEXTURE)]
  public string m_logoTextureName;
  [CustomEditField(T = EditType.TEXTURE)]
  public string m_logoTextureGlowName;
  [CustomEditField(T = EditType.TEXTURE)]
  public string m_accentTextureName;
  [CustomEditField(T = EditType.TEXTURE)]
  public string m_miniSetLogoTextureName;
  [CustomEditField(T = EditType.TEXTURE)]
  public string m_miniSetAccentTextureName;
  [Tooltip("Defaults to logo texture if left blank")]
  [CustomEditField(T = EditType.TEXTURE)]
  public string m_sellableDeckLogoTextureName;
  [CustomEditField(T = EditType.MATERIAL)]
  public string m_background;
  public MusicPlaylistType m_playlist;
  public MusicPlaylistType m_miniSetPlaylist;
  public string m_preorderAvailableDateString;
  public string m_preorderDustAvailableDateString;

  public string GetSelectorButtonPrefab() => this.m_buttonPrefab;

  public string GetLowPolyPrefab() => this.m_lowPolyPrefab;

  public string GetLowPolyDustPrefab() => this.m_lowPolyPrefabDust;

  public string GetLogoTextureName() => this.m_logoTextureName;

  public string GetLogoTextureGlowName() => this.m_logoTextureGlowName;

  public string GetAccentTextureName() => this.m_accentTextureName;

  public string GetBackgroundMaterial() => this.m_background;

  public string GetBackgroundTexture() => "";

  public MusicPlaylistType GetPlaylist() => this.m_playlist;

  public MusicPlaylistType GetMiniSetPlaylist() => this.m_miniSetPlaylist;

  public string GetPreorderAvailableDateString() => this.m_preorderAvailableDateString;

  public string GetPreorderDustAvailableDateString() => this.m_preorderDustAvailableDateString;

  public string GetMiniSetTextureName() => this.m_miniSetLogoTextureName;

  public string GetMiniSetAccentTextureName() => this.m_miniSetAccentTextureName;

  public string GetSellableDeckTextureName() => string.IsNullOrEmpty(this.m_sellableDeckLogoTextureName) ? this.m_logoTextureName : this.m_sellableDeckLogoTextureName;
}
