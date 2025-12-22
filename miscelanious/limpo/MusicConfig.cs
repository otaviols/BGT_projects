using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class MusicConfig : MonoBehaviour
{
  [CustomEditField(Sections = "Playlists")]
  public List<MusicPlaylist> m_playlists = new List<MusicPlaylist>();

  public MusicPlaylist GetPlaylist(MusicPlaylistType type) => this.FindPlaylist(type) ?? new MusicPlaylist();

  public MusicPlaylist FindPlaylist(MusicPlaylistType type)
  {
    for (int index = 0; index < this.m_playlists.Count; ++index)
    {
      MusicPlaylist playlist = this.m_playlists[index];
      if (playlist.m_type == type)
        return playlist;
    }
    return (MusicPlaylist) null;
  }

  private void Awake() => this.gameObject.AddComponent<HSDontDestroyOnLoad>();
}
