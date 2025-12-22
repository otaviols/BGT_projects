using System;

public class ReactiveNetCacheObject<T> : ReactiveObject<T> where T : new()
{
  private ReactiveNetCacheObject()
  {
  }

  public static ReactiveNetCacheObject<T> CreateInstance() => (ReactiveObject<T>.GetExistingInstance() ?? (ReactiveObject<T>) new ReactiveNetCacheObject<T>()) as ReactiveNetCacheObject<T>;

  protected override T FetchValue()
  {
    NetCache netCache = NetCache.Get();
    return netCache == null ? default (T) : netCache.GetNetObject<T>();
  }

  protected override bool RegisterChangeCallback()
  {
    NetCache netCache = NetCache.Get();
    if (netCache == null)
      return false;
    netCache.RegisterUpdatedListener(typeof (T), new Action(((ReactiveObject<T>) this).OnObjectChanged));
    return true;
  }
}
