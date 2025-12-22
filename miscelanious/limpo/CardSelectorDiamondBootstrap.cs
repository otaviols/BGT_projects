using Blizzard.T5.Jobs;
using Blizzard.T5.MaterialService;
using Blizzard.T5.Services;
using UnityEngine;

public class CardSelectorDiamondBootstrap : MonoBehaviour
{
  private void Start()
  {
    ServiceManager.SetDependencies((Blizzard.T5.Core.ILogger) Log.Services, HearthstoneServiceFactory.CreateServiceFactory());
    IJobDependency[] serviceDependencies = (IJobDependency[]) null;
    ServiceManager.SetDependencies((Blizzard.T5.Core.ILogger) Log.Services, HearthstoneServiceFactory.CreateServiceFactory());
    ServiceManager.InitializeDynamicServicesIfNeeded(out serviceDependencies, typeof (IAssetLoader), typeof (DiamondRenderToTextureService), typeof (IMaterialService), typeof (LegendaryHeroRenderToTextureService));
  }

  private void OnDestroy() => ServiceManager.Shutdown();
}
