using Blizzard.T5.Jobs;
using Blizzard.T5.Services;

public class WaitForCheckoutInitialized : IJobDependency, IAsyncJobResult
{
  public bool IsReady()
  {
    if (!ServiceManager.IsAvailable<HearthstoneCheckout>())
      return false;
    HearthstoneCheckout hearthstoneCheckout = ServiceManager.Get<HearthstoneCheckout>();
    return hearthstoneCheckout.IsIdle || hearthstoneCheckout.IsUnavailable;
  }

  public override string ToString() => "WaitForCheckoutInitialized : (Idle, Unavailable)";
}
