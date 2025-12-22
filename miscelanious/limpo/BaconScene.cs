using PegasusUtil;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class BaconScene : BasicScene
{
  private bool m_ratingInfoReceived;
  private bool m_gameSaveDataReceived;

  protected override void Start()
  {
    Network.Get().RegisterNetHandler((object) BattlegroundsRatingInfoResponse.PacketID.ID, new Network.NetHandler(this.OnBaconRatingInfo));
    Network.Get().RequestBaconRatingInfo();
    GameSaveDataManager.Get().Request(GameSaveKeyId.BACON, new GameSaveDataManager.OnRequestDataResponseDelegate(this.OnGameSaveDataReceived));
    base.Start();
  }

  public override bool IsUnloading() => false;

  private void OnScreenPrefabLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if (!((Object) go == (Object) null))
      return;
    Debug.LogError((object) string.Format("BaconScene.OnScreenLoaded() - failed to load screen {0}", (object) assetRef));
  }

  private void OnBaconRatingInfo()
  {
    Network.Get().RemoveNetHandler((object) BattlegroundsRatingInfoResponse.PacketID.ID, new Network.NetHandler(this.OnBaconRatingInfo));
    this.m_ratingInfoReceived = true;
  }

  private void OnGameSaveDataReceived(bool success) => this.m_gameSaveDataReceived = true;

  protected override IEnumerator NotifySceneLoadedWhenReady()
  {
    yield return (object) new WaitForSeconds(0.5f);
    while (!this.m_ratingInfoReceived)
      yield return (object) null;
    while (!this.m_gameSaveDataReceived)
      yield return (object) null;
    yield return (object) base.NotifySceneLoadedWhenReady();
  }
}
