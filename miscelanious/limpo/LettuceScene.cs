using Hearthstone;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class LettuceScene : BasicScene
{
  protected override void Start()
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject != null && netObject.MercenariesEnableVillages)
    {
      if (LettuceVillageDataUtil.Initialized)
      {
        LettuceVillageDataUtil.RefreshDataIfNecessary();
      }
      else
      {
        CollectionManager.Get().StartInitialMercenaryLoadIfRequired();
        this.StartCoroutine(this.InitializeDataWhenInitialMercenaryDataIsReady());
      }
    }
    base.Start();
  }

  protected override IEnumerator NotifySceneLoadedWhenReady()
  {
    LettuceScene lettuceScene = this;
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject != null && netObject.MercenariesEnableVillages)
    {
      while (!LettuceVillageDataUtil.Initialized)
      {
        lettuceScene.m_isFinishedLoadingTimer += Time.unscaledDeltaTime;
        if ((double) lettuceScene.m_isFinishedLoadingTimer > 15.0)
        {
          Error.AddFatal(FatalErrorReason.LOAD_SCENE_NETWORK_TIMEOUT, "GLOBAL_ERROR_NETWORK_DISCONNECT");
          yield break;
        }
        else
          yield return (object) null;
      }
    }
    // ISSUE: reference to a compiler-generated method
    yield return (object) lettuceScene.\u003C\u003En__0();
  }

  protected IEnumerator InitializeDataWhenInitialMercenaryDataIsReady()
  {
    while (!CollectionManager.Get().IsLettuceLoaded())
      yield return (object) null;
    LettuceVillageDataUtil.InitializeData();
  }

  private void OnDestroy() => HearthstoneApplication.Get()?.UnloadUnusedAssets();
}
