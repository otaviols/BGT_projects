using Hearthstone;
using System.Collections;

public class Reset : PegasusScene
{
  private void Start()
  {
    SceneMgr.Get().NotifySceneLoaded();
    this.StartCoroutine("WaitThenReset");
  }

  private IEnumerator WaitThenReset()
  {
    while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
      yield return (object) null;
    HearthstoneApplication.Get().Reset();
  }
}
