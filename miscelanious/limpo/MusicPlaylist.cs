using System;
using System.Collections.Generic;

[Serializable]
public class MusicPlaylist
{
  [CustomEditField(ListSortable = true)]
  public MusicPlaylistType m_type;
  [CustomEditField(ListTable = true)]
  public List<MusicTrack> m_tracks = new List<MusicTrack>();

  public List<MusicTrack> GetMusicTracks() => this.GetRandomizedTracks(this.m_tracks, MusicTrackType.Music);

  public List<MusicTrack> GetAmbienceTracks() => this.GetRandomizedTracks(this.m_tracks, MusicTrackType.Ambience);

  private List<MusicTrack> GetRandomizedTracks(
    List<MusicTrack> trackList,
    MusicTrackType type)
  {
    List<MusicTrack> randomizedTracks = new List<MusicTrack>();
    List<MusicTrack> musicTrackList = new List<MusicTrack>();
    foreach (MusicTrack track in trackList)
    {
      if (type == track.m_trackType && !string.IsNullOrEmpty(track.m_name))
      {
        if (track.m_shuffle)
          musicTrackList.Add(track.Clone());
        else
          randomizedTracks.Add(track.Clone());
      }
    }
    Random random = new Random();
    while (musicTrackList.Count > 0)
    {
      int index = random.Next(0, musicTrackList.Count);
      randomizedTracks.Add(musicTrackList[index]);
      musicTrackList.RemoveAt(index);
    }
    return randomizedTracks;
  }
}
