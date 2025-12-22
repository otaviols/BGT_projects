using Blizzard.T5.Jobs;
using Blizzard.T5.Services;

public class WaitForCheckoutConfiguration : IJobDependency, IAsyncJobResult
{
  public bool IsReady()
  {
    HearthstoneCheckout service;
    return ServiceManager.TryGet<HearthstoneCheckout>(out service) && service.HasProductCatalog && service.HasClientID && service.HasCurrencyCode;
  }
}
