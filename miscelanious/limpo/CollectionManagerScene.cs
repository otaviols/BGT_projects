using System.Collections;
using UnityEngine;

[CustomEditClass]
public class CollectionManagerScene : PegasusScene
{
  private bool m_unloading;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_CollectionManagerPrefab;

  protected override void Awake()
  {
    base.Awake();
    AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) this.m_CollectionManagerPrefab, new PrefabCallback<GameObject>(this.OnUIScreenLoaded));
  }

  private void Update() => Network.Get().ProcessNetwork();

  public override bool IsUnloading() => this.m_unloading;

  public override void Unload()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      BnetBar.Get().ToggleActive(true);
    this.m_unloading = true;
    CollectionManager.Get().GetCollectibleDisplay().Unload();
    Network.Get().SendAckCardsSeen();
    Network.Get().CheckForSendingBattlegroundsSkinsSeenPacket(1);
    this.m_unloading = false;
  }

  private void OnUIScreenLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
      Debug.LogError((object) string.Format("CollectionManagerScene.OnUIScreenLoaded() - failed to load screen {0}", (object) assetRef));
    else
      this.StartCoroutine(this.NotifySceneLoadedWhenReady());
  }

  private IEnumerator NotifySceneLoadedWhenReady()
  {
    while (!CollectionManager.Get().GetCollectibleDisplay().IsReady())
      yield return (object) null;
    SceneMgr.Get().NotifySceneLoaded();
  }
}
