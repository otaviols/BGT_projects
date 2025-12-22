using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone.Http;
using System.Collections.Generic;
using UnityEngine;

public class UnityUrlDownloader : IUrlDownloader
{
  private HashSet<UnityUrlDownloader.DownloadState> m_downloadsToStart = new HashSet<UnityUrlDownloader.DownloadState>();
  private HashSet<UnityUrlDownloader.DownloadState> m_downloadsRunning = new HashSet<UnityUrlDownloader.DownloadState>();
  private HashSet<UnityUrlDownloader.DownloadState> m_downloadsDone = new HashSet<UnityUrlDownloader.DownloadState>();

  public void Process()
  {
    foreach (UnityUrlDownloader.DownloadState downloadState in this.m_downloadsToStart)
    {
      downloadState.startTime = Time.realtimeSinceStartup;
      downloadState.handle = HttpRequestFactory.Get().CreateGetRequest(downloadState.url);
      downloadState.handle.SendRequest();
      this.m_downloadsRunning.Add(downloadState);
    }
    this.m_downloadsToStart.Clear();
    if (this.m_downloadsRunning.Count <= 0)
      return;
    HashSet<UnityUrlDownloader.DownloadState> downloadStateSet = (HashSet<UnityUrlDownloader.DownloadState>) null;
    foreach (UnityUrlDownloader.DownloadState downloadState in this.m_downloadsRunning)
    {
      bool flag = false;
      if (downloadState.handle.IsDone)
      {
        downloadState.success = !downloadState.handle.IsNetworkError && !downloadState.handle.IsHttpError;
        flag = true;
      }
      else if (downloadState.timeoutMs >= 0 && (double) Time.realtimeSinceStartup - (double) downloadState.startTime > (double) downloadState.timeoutMs / 1000.0)
      {
        downloadState.success = false;
        flag = true;
      }
      if (flag)
      {
        if (downloadStateSet == null)
          downloadStateSet = new HashSet<UnityUrlDownloader.DownloadState>();
        downloadStateSet.Add(downloadState);
      }
    }
    if (downloadStateSet != null)
    {
      foreach (UnityUrlDownloader.DownloadState downloadState in downloadStateSet)
      {
        this.m_downloadsRunning.Remove(downloadState);
        this.m_downloadsDone.Add(downloadState);
      }
    }
    foreach (UnityUrlDownloader.DownloadState downloadState in this.m_downloadsDone)
    {
      if (!downloadState.success && downloadState.numRetriesLeft > 0)
      {
        --downloadState.numRetriesLeft;
        this.m_downloadsToStart.Add(downloadState);
      }
      else if (downloadState.cb != null)
        downloadState.cb(downloadState.success, downloadState.handle.ResponseRaw);
    }
    this.m_downloadsDone.Clear();
  }

  public void Download(string url, UrlDownloadCompletedCallback cb)
  {
    UrlDownloaderConfig config = new UrlDownloaderConfig();
    this.Download(url, cb, config);
  }

  public void Download(string url, UrlDownloadCompletedCallback cb, UrlDownloaderConfig config) => this.m_downloadsToStart.Add(new UnityUrlDownloader.DownloadState()
  {
    url = url,
    timeoutMs = config.timeoutMs,
    numRetriesLeft = config.numRetries,
    cb = cb
  });

  internal class DownloadState
  {
    public string url;
    public int numRetriesLeft;
    public int timeoutMs = -1;
    public IHttpRequest handle;
    public bool success;
    public UrlDownloadCompletedCallback cb;
    public float startTime;
  }
}
