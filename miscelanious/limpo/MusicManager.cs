using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : IService
{
  private MusicPlaylistType m_currentPlaylist;

  public MusicConfig Config { get; private set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    MusicManager musicManager = this;
    InstantiatePrefab loadMusicConfig = new InstantiatePrefab((AssetReference) "MusicConfig.prefab:0af92217368c85f42ae37bec9a4e3625");
    yield return (IAsyncJobResult) loadMusicConfig;
    musicManager.Config = loadMusicConfig.InstantiatedPrefab.GetComponent<MusicConfig>();
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().WillReset += new Action(musicManager.WillReset);
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (IAssetLoader)
  };

  public void Shutdown()
  {
  }

  public static MusicManager Get() => ServiceManager.Get<MusicManager>();

  public bool StartPlaylist(MusicPlaylistType type)
  {
    if (this.m_currentPlaylist == type)
      return true;
    SoundManager service;
    if (!ServiceManager.TryGet<SoundManager>(out service))
    {
      Debug.LogError((object) "MusicManager.StartPlaylist() - SoundManager does not exist.");
      return false;
    }
    MusicPlaylist playlist = this.FindPlaylist(type);
    if (playlist == null)
    {
      Debug.LogWarning((object) string.Format("MusicManager.StartPlaylist() - failed to find playlist for type {0}", (object) type));
      return false;
    }
    List<MusicTrack> musicTracks = playlist.GetMusicTracks();
    List<MusicTrack> currentMusicTracks = service.GetCurrentMusicTracks();
    if (!this.AreTracksEqual(musicTracks, currentMusicTracks))
    {
      service.NukeMusicAndStopPlayingCurrentTrack();
      if (musicTracks != null && musicTracks.Count > 0)
        service.AddMusicTracks(musicTracks);
    }
    List<MusicTrack> ambienceTracks = playlist.GetAmbienceTracks();
    List<MusicTrack> currentAmbienceTracks = service.GetCurrentAmbienceTracks();
    if (!this.AreTracksEqual(ambienceTracks, currentAmbienceTracks))
    {
      service.NukeAmbienceAndStopPlayingCurrentTrack();
      if (ambienceTracks != null && ambienceTracks.Count > 0)
        service.AddAmbienceTracks(ambienceTracks);
    }
    this.m_currentPlaylist = playlist.m_type;
    return true;
  }

  public bool StopPlaylist()
  {
    SoundManager soundManager = SoundManager.Get();
    if (soundManager == null)
    {
      Debug.LogError((object) "MusicManager.StopPlaylist() - SoundManager does not exist.");
      return false;
    }
    if (this.m_currentPlaylist == MusicPlaylistType.Invalid)
      return false;
    this.m_currentPlaylist = MusicPlaylistType.Invalid;
    soundManager.NukePlaylistsAndStopPlayingCurrentTracks();
    return true;
  }

  public MusicPlaylistBookmark CreateBookmarkOfCurrentPlaylist()
  {
    SoundManager soundManager = SoundManager.Get();
    if (soundManager == null)
    {
      Debug.LogError((object) "MusicManager.CreateBookmarkOfCurrentPlaylist() - SoundManager does not exist.");
      return new MusicPlaylistBookmark();
    }
    MusicPlaylistBookmark ofCurrentPlaylist = new MusicPlaylistBookmark();
    ofCurrentPlaylist.m_playListType = this.m_currentPlaylist;
    ofCurrentPlaylist.m_playListIndex = soundManager.GetCurrentMusicTrackIndex();
    ofCurrentPlaylist.m_timeStamp = Time.unscaledTime;
    AudioSource currentMusicTrack = soundManager.GetCurrentMusicTrack();
    if ((bool) (UnityEngine.Object) currentMusicTrack)
    {
      ofCurrentPlaylist.m_trackTime = currentMusicTrack.time;
      ofCurrentPlaylist.m_currentTrack = currentMusicTrack;
    }
    return ofCurrentPlaylist;
  }

  public bool PlayFromBookmark(MusicPlaylistBookmark bookmark)
  {
    if (bookmark == null || bookmark.m_playListType == MusicPlaylistType.Invalid)
      return false;
    SoundManager sndMgr = SoundManager.Get();
    if (sndMgr == null)
    {
      Debug.LogError((object) "MusicManager.PlayFromBookmark() - SoundManager does not exist.");
      return false;
    }
    Action syncMusic = (Action) null;
    syncMusic = (Action) (() =>
    {
      sndMgr.OnMusicStarted -= syncMusic;
      if (this.m_currentPlaylist != bookmark.m_playListType || sndMgr.GetCurrentMusicTrackIndex() != bookmark.m_playListIndex)
        return;
      if ((UnityEngine.Object) bookmark.m_currentTrack != (UnityEngine.Object) null)
        sndMgr.SetCurrentMusicTrackTime(bookmark.m_currentTrack.time);
      else
        sndMgr.SetCurrentMusicTrackTime(bookmark.m_trackTime);
    });
    sndMgr.OnMusicStarted += syncMusic;
    this.StartPlaylist(bookmark.m_playListType);
    sndMgr.SetCurrentMusicTrackIndex(bookmark.m_playListIndex);
    return true;
  }

  public MusicPlaylistType GetCurrentPlaylist() => this.m_currentPlaylist;

  private void WillReset()
  {
    SoundManager soundManager = SoundManager.Get();
    if (soundManager == null)
    {
      Debug.LogError((object) "MusicManager.WillReset() - SoundManager does not exist.");
    }
    else
    {
      this.m_currentPlaylist = MusicPlaylistType.Invalid;
      soundManager.ImmediatelyKillMusicAndAmbience();
    }
  }

  private MusicPlaylist FindPlaylist(MusicPlaylistType type)
  {
    if ((UnityEngine.Object) this.Config == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "MusicManager.FindPlaylist() - MusicConfig does not exist.");
      return (MusicPlaylist) null;
    }
    MusicPlaylist playlist = this.Config.FindPlaylist(type);
    if (playlist != null)
      return playlist;
    Debug.LogWarning((object) string.Format("MusicManager.FindPlaylist() - {0} playlist is not defined.", (object) type));
    return (MusicPlaylist) null;
  }

  private bool AreTracksEqual(List<MusicTrack> newTracks, List<MusicTrack> curTracks)
  {
    if (newTracks.Count != curTracks.Count)
      return false;
    foreach (MusicTrack newTrack in newTracks)
    {
      MusicTrack newT = newTrack;
      if (curTracks.Find((Predicate<MusicTrack>) (curT => (AssetLoader.Get().IsAssetAvailable((AssetReference) curT.m_name) ? curT.m_name : curT.m_fallback) == (AssetLoader.Get().IsAssetAvailable((AssetReference) newT.m_name) ? newT.m_name : newT.m_fallback))) == null)
        return false;
    }
    return true;
  }
}
