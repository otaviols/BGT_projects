using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class StoreAdventureDef : MonoBehaviour
{
  [CustomEditField(Label = "Logo Main Texture", T = EditType.TEXTURE)]
  public string m_logoTextureName;
  [CustomEditField(Label = "Logo Shadow Texture", T = EditType.TEXTURE)]
  public string m_logoShadowTextureName;
  [CustomEditField(Label = "Logo Glow Texture", T = EditType.TEXTURE)]
  public string m_logoTextureGlowName;
  [CustomEditField(Label = "Accent Texture", T = EditType.TEXTURE)]
  public string m_accentTextureName;
  [CustomEditField(Label = "Music Playlist")]
  public MusicPlaylistType m_playlist;
  [CustomEditField(Label = "Preview Cards")]
  public List<string> m_previewCards = new List<string>();
  [CustomEditField(Sections = "Deprecated (Might be removed eventually.)", T = EditType.GAME_OBJECT)]
  public string m_storeButtonPrefab;
  [CustomEditField(Sections = "Deprecated (Might be removed eventually.)")]
  public Material m_keyArt;
  [CustomEditField(Sections = "Deprecated (Might be removed eventually.)")]
  public int m_preorderCardBackId;
  [CustomEditField(Sections = "Deprecated (Might be removed eventually.)")]
  public string m_preorderCardBackTextName;

  public string GetLogoTextureName() => this.m_logoTextureName;

  public string GetLogoShadowTextureName() => this.m_logoShadowTextureName;

  public string GetAccentTextureName() => this.m_accentTextureName;

  public MusicPlaylistType GetPlaylist() => this.m_playlist;
}
