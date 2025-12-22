using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections;
using UnityEngine;

public class SpellSystemBootstrap : MonoBehaviour
{
  public SpellSystemTest test;

  private IEnumerator Start()
  {
    while (!ServiceManager.AreDependenciesSet())
      yield return (object) null;
    IJobDependency[] serviceDependencies = (IJobDependency[]) null;
    ServiceManager.InitializeDynamicServicesIfNeeded(out serviceDependencies, typeof (IAssetLoader), typeof (SpellManager));
    while (SpellManager.Get() == null)
      yield return (object) null;
    yield return (object) new WaitForSeconds(2f);
    if ((Object) this.test != (Object) null)
      this.test.BeginTest();
  }

  private void OnDestroy() => ServiceManager.Shutdown();
}
