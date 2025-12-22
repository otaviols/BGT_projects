using PegasusGame;
using UnityEngine;

public class DebugMessageManager : MonoBehaviour
{
  private static DebugMessageManager s_instance;

  public static DebugMessageManager Get()
  {
    if ((Object) DebugMessageManager.s_instance == (Object) null)
    {
      GameObject gameObject = new GameObject();
      DebugMessageManager.s_instance = gameObject.AddComponent<DebugMessageManager>();
      gameObject.name = "DebugMessageManager (Dynamically created)";
    }
    return DebugMessageManager.s_instance;
  }

  public void OnDebugMessage(DebugMessage debugMessage) => Log.Gameplay.PrintAndForcePrintToScreen(Blizzard.T5.Logging.LogLevel.Info, false, debugMessage.Message);
}
