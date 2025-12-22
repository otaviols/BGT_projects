using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Blizzard.T5.Fonts;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FontLoader : IFontLoader
{
  private LoadResource m_resourceData;
  private Map<string, AssetHandle<FontDefinition>> m_defs;
  private Logger m_logger;

  public FontTableData ResourceData => this.m_resourceData.LoadedAsset as FontTableData;

  public FontLoader(Logger logger) => this.m_logger = logger;

  public IUnreliableJobDependency LoadFontTableData()
  {
    this.m_resourceData = new LoadResource("ServiceData/FontTableData", LoadResourceFlags.FailOnError);
    return (IUnreliableJobDependency) this.m_resourceData;
  }

  public void LoadFontDefinition(
    Action<Map<string, AssetHandle<FontDefinition>>> onLoadDone = null)
  {
    Processor.QueueJob("loadDefs", this.Job_LoadFontDefinition(onLoadDone), (IJobDependency) new WaitForGameDownloadManagerState(), ServiceManager.CreateServiceDependency(typeof (IAssetLoader)));
  }

  private IEnumerator<IAsyncJobResult> Job_LoadFontDefinition(
    Action<Map<string, AssetHandle<FontDefinition>>> onLoadDone = null)
  {
    this.m_defs = new Map<string, AssetHandle<FontDefinition>>();
    JobResultCollection loadFontDefJobs = new JobResultCollection(Array.Empty<IAsyncJobResult>());
    foreach (FontTableData.FontTableEntry entry in this.ResourceData.m_Entries)
      loadFontDefJobs.Add((IAsyncJobResult) new LoadFontDef((AssetReference) string.Format("{0}:{1}", (object) entry.m_FontName, (object) entry.m_FontGuid)));
    yield return (IAsyncJobResult) loadFontDefJobs;
    for (int i = 0; i < loadFontDefJobs.Results.Count; ++i)
    {
      LoadFontDef loadFontDefJob = loadFontDefJobs.Results[i] as LoadFontDef;
      string assetName = loadFontDefJob.AssetRef.GetLegacyAssetName();
      this.m_logger.Log(Blizzard.T5.Core.LogLevel.Debug, "OnFontDefLoaded " + assetName, Array.Empty<object>());
      if (loadFontDefJob.loadedAsset == null)
      {
        ServiceManager.Get<IErrorService>()?.AddFatal(FatalErrorReason.ASSET_INCORRECT_DATA, "GLOBAL_ERROR_ASSET_INCORRECT_DATA", (object) assetName);
        string str = string.Format("FontLoader.Job_LoadFontDefinition() - name={0} message={1}", (object) assetName, (object) ServiceManager.Get<IGameStringsService>().Format("GLOBAL_ERROR_ASSET_INCORRECT_DATA", (object) assetName));
        Debug.LogError((object) str);
        yield return (IAsyncJobResult) new JobFailedResult(str, Array.Empty<object>());
      }
      this.m_defs.SetOrReplaceDisposable<string, AssetHandle<FontDefinition>>(assetName, loadFontDefJob.loadedAsset);
      loadFontDefJob = (LoadFontDef) null;
      assetName = (string) null;
    }
    Action<Map<string, AssetHandle<FontDefinition>>> action = onLoadDone;
    if (action != null)
      action(this.m_defs);
  }
}
