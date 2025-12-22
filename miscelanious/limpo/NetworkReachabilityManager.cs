using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

public class NetworkReachabilityManager : IService, IHasUpdate
{
  private float m_internetReachabilityPollTimer;
  private bool m_internetReachabilityForceDisabled;

  public static bool OnCellular => Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Options.Get().GetBool(Option.SIMULATE_CELLULAR);

  public static bool InternetAvailable => Application.internetReachability != 0;

  public bool InternetAvailable_Cached
  {
    get
    {
      NetworkReachability networkReachability = NetworkReachability.NotReachable;
      if (!this.m_internetReachabilityForceDisabled)
        networkReachability = this.CachedReachability;
      return networkReachability != 0;
    }
  }

  public NetworkReachability CachedReachability { get; private set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    this.CachedReachability = Application.internetReachability;
    yield break;
  }

  public System.Type[] GetDependencies() => new System.Type[0];

  public void Shutdown()
  {
  }

  public void SetForceUnreachable(bool value) => this.m_internetReachabilityForceDisabled = value;

  public bool GetForceUnreachable() => this.m_internetReachabilityForceDisabled;

  void IHasUpdate.Update() => this.PollInternetReachability();

  private void PollInternetReachability()
  {
    this.m_internetReachabilityPollTimer += Time.unscaledDeltaTime;
    if ((double) this.m_internetReachabilityPollTimer < 1.0)
      return;
    this.m_internetReachabilityPollTimer = 0.0f;
    this.CachedReachability = Application.internetReachability;
  }
}
