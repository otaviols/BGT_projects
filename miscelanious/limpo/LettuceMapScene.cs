using Hearthstone;
using UnityEngine;

[CustomEditClass]
public class LettuceMapScene : BasicScene
{
  private void OnDestroy()
  {
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if (!((Object) hearthstoneApplication != (Object) null))
      return;
    hearthstoneApplication.UnloadUnusedAssets();
  }
}
