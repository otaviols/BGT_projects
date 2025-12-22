using System;

public class ModularBundleStorePackDef : IStorePackDef
{
  private ModularBundleDbfRecord m_modularBundleRecord;

  public ModularBundleStorePackDef(ModularBundleDbfRecord modularBundleRecord) => this.m_modularBundleRecord = modularBundleRecord;

  public string GetSelectorButtonPrefab() => this.m_modularBundleRecord.SelectorPrefab;

  public string GetLowPolyPrefab() => "";

  public string GetLowPolyDustPrefab() => "";

  public string GetLogoTextureName() => this.m_modularBundleRecord.LogoTexture;

  public string GetLogoTextureGlowName() => this.m_modularBundleRecord.LogoTextureGlow;

  public string GetAccentTextureName() => "";

  public string GetBackgroundMaterial() => "";

  public string GetBackgroundTexture() => this.m_modularBundleRecord.Background;

  public MusicPlaylistType GetPlaylist()
  {
    object obj = Enum.Parse(typeof (MusicPlaylistType), this.m_modularBundleRecord.Playlist, true);
    return obj == null ? MusicPlaylistType.Invalid : (MusicPlaylistType) obj;
  }
}
