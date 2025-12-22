using Hearthstone;
using UnityEngine;

[CustomEditClass]
public class LettuceBountyBoardScene : BasicScene
{
  private void OnDestroy()
  {
    if (!((Object) HearthstoneApplication.Get() != (Object) null))
      return;
    HearthstoneApplication.Get().UnloadUnusedAssets();
  }
}
