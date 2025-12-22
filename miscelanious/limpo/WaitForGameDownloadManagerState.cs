using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Streaming;

public class WaitForGameDownloadManagerState : IJobDependency, IAsyncJobResult
{
  public bool IsReady() => ServiceManager.IsAvailable<GameDownloadManager>() && ServiceManager.Get<GameDownloadManager>().IsReadyToPlay;
}
