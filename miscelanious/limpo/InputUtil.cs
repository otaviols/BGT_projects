using Blizzard.T5.Core.Utils;
using UnityEngine;

public class InputUtil
{
  public static InputScheme GetInputScheme()
  {
    switch (Application.platform)
    {
      case RuntimePlatform.IPhonePlayer:
      case RuntimePlatform.Android:
        return InputScheme.TOUCH;
      default:
        return InputScheme.KEYBOARD_MOUSE;
    }
  }

  public static bool IsMouseOnScreen()
  {
    if (UniversalInputManager.Get() == null)
      return false;
    Vector3 mousePosition = InputCollection.GetMousePosition();
    return (double) mousePosition.x >= 0.0 && (double) mousePosition.x <= (double) Screen.width && (double) mousePosition.y >= 0.0 && (double) mousePosition.y <= (double) Screen.height;
  }

  public static bool IsPlayMakerMouseInputAllowed(GameObject go)
  {
    if (UniversalInputManager.Get() == null)
      return false;
    if (InputUtil.ShouldCheckGameplayForPlayMakerMouseInput(go))
    {
      GameState gameState = GameState.Get();
      if (gameState != null && gameState.IsMulliganManagerActive())
        return false;
      TargetReticleManager targetReticleManager = TargetReticleManager.Get();
      if ((Object) targetReticleManager != (Object) null && targetReticleManager.IsLocalArrowActive())
        return false;
    }
    return true;
  }

  private static bool ShouldCheckGameplayForPlayMakerMouseInput(GameObject go) => SceneMgr.Get() != null && SceneMgr.Get().IsInGame() && (!((Object) LoadingScreen.Get() != (Object) null) || !LoadingScreen.Get().IsPreviousSceneActive() || !((Object) GameObjectUtils.FindComponentInThisOrParents<LoadingScreen>(go) != (Object) null)) && !((Object) GameObjectUtils.FindComponentInThisOrParents<BaseUI>(go) != (Object) null);
}
