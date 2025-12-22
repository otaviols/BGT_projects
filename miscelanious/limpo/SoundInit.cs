using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Hearthstone/Services/Sound")]
public class SoundInit : MonoBehaviour
{
  public bool m_ready;

  private void Start()
  {
    this.m_ready = false;
    IJobDependency[] serviceDependencies;
    ServiceManager.InitializeDynamicServicesIfEditor(out serviceDependencies, typeof (SoundManager));
    Processor.QueueJob("SoundInit.Initialize", this.Job_Initialize(), serviceDependencies);
  }

  private IEnumerator<IAsyncJobResult> Job_Initialize()
  {
    this.m_ready = true;
    yield break;
  }
}
