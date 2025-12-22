using Hearthstone;
using UnityEngine;

[CustomEditClass]
public class LettuceCollectionScene : BasicScene
{
  public override void Unload()
  {
    if ((Object) CollectionManager.Get().GetCollectibleDisplay() != (Object) null)
      CollectionManager.Get().GetCollectibleDisplay().Unload();
    Network.Get().SendAckCardsSeen();
    base.Unload();
  }

  private void OnDestroy() => HearthstoneApplication.Get()?.UnloadUnusedAssets();
}
