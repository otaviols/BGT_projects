using Blizzard.T5.Jobs;
using Blizzard.T5.Services;

public class WaitForNetCacheObject<T> : IJobDependency, IAsyncJobResult
{
  public bool IsReady()
  {
    NetCache service;
    return ServiceManager.TryGet<NetCache>(out service) && (object) service.GetNetObject<T>() != null;
  }
}
