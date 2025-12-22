using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using Hearthstone.Util.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaconCosmeticPreviewTester : MonoBehaviour
{
  private const string PREVIEW_SCENE_NAME = "BaconCosmeticPreview";
  public BaconCosmeticPreviewRunnerConfig m_config;
  [HideInInspector]
  public bool m_dbfLoaded;
  [HideInInspector]
  public bool m_assetResolverServiceLoaded;

  public void Awake()
  {
    this.m_dbfLoaded = false;
    Processor.QueueJob("CosmeticTester.LoadDBF", this.LoadDBF()).AddJobFinishedEventListener(new JobDefinition.JobFinishedEventListener(this.OnDBFLoadFinished));
    this.StartCoroutine(this.WaitForAssetResolver());
    ServicesBootstrapper.SetupForStandaloneScenes();
    IJobDependency[] serviceDependencies;
    ServiceManager.InitializeDynamicServicesIfNeeded(out serviceDependencies, DynamicServiceSets.UberText());
    ServiceManager.InitializeDynamicServicesIfNeeded(out serviceDependencies, typeof (IAssetLoader), typeof (IAliasedAssetResolver), typeof (SpellManager));
  }

  private void OnDBFLoadFinished(JobDefinition job, bool success) => this.m_dbfLoaded = success;

  public IEnumerator<IAsyncJobResult> LoadDBF()
  {
    yield return (IAsyncJobResult) GameDbf.CreateLoadDbfJob();
  }

  public IEnumerator WaitForAssetResolver()
  {
    yield return (object) new WaitUntil((Func<bool>) (() => ServiceManager.AreDependenciesSet()));
    this.m_assetResolverServiceLoaded = true;
  }

  public void LoadScene() => this.StartCoroutine(this.LoadSceneCoroutine());

  public IEnumerator LoadSceneCoroutine()
  {
    if (SceneManager.GetSceneByName("BaconCosmeticPreview").isLoaded)
    {
      AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("BaconCosmeticPreview");
      while (!unloadOp.isDone)
        yield return (object) null;
      unloadOp = (AsyncOperation) null;
    }
    AsyncOperation loadOp = SceneManager.LoadSceneAsync("BaconCosmeticPreview", LoadSceneMode.Additive);
    while (!loadOp.isDone)
      yield return (object) null;
  }
}
