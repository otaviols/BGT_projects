using Cysharp.Threading.Tasks;
using UnityEngine.LowLevel;

public class CustomPlayerLoop
{
  public static void SetupCustomPlayerLoop()
  {
    PlayerLoopSystem defaultPlayerLoop = PlayerLoop.GetDefaultPlayerLoop();
    PlayerLoopHelper.Initialize(ref defaultPlayerLoop);
    PlayerLoop.SetPlayerLoop(defaultPlayerLoop);
  }
}
