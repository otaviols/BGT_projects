using Blizzard.T5.Services;
using UnityEngine;

public class FatalErrorScene : PegasusScene
{
  protected override void Awake()
  {
    AssetLoader.Get().InstantiatePrefab((AssetReference) "FatalErrorScreen.prefab:b1524cacda5324547ac72995309dad14", new PrefabCallback<GameObject>(this.OnFatalErrorScreenLoaded));
    base.Awake();
    Navigation.Clear();
    Network service;
    if (ServiceManager.TryGet<Network>(out service))
      service.AppAbort();
    UserAttentionManager.StartBlocking(UserAttentionBlocker.FATAL_ERROR_SCENE);
    if ((Object) DialogManager.Get() != (Object) null)
      DialogManager.Get().ClearAllImmediately();
    foreach (Component allCamera in Camera.allCameras)
    {
      FullScreenEffects component = allCamera.GetComponent<FullScreenEffects>();
      if (!((Object) component == (Object) null))
        component.Disable();
    }
  }

  private void Start() => SceneMgr.Get().NotifySceneLoaded();

  public override void Unload() => UserAttentionManager.StopBlocking(UserAttentionBlocker.FATAL_ERROR_SCENE);

  private void OnFatalErrorScreenLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if (!((Object) go == (Object) null))
      return;
    this.gameObject.AddComponent<FatalErrorDialog>();
  }
}
