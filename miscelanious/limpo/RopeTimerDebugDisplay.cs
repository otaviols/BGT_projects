using Hearthstone;
using PegasusGame;
using UnityEngine;

public class RopeTimerDebugDisplay : MonoBehaviour
{
  private static RopeTimerDebugDisplay s_instance;
  private RopeTimerDebugInformation m_debugInformation;
  public bool m_isDisplayed;
  private const float MICROSECONDS_IN_SECOND = 1000000f;

  public static RopeTimerDebugDisplay Get()
  {
    if ((Object) RopeTimerDebugDisplay.s_instance == (Object) null)
    {
      GameObject gameObject = new GameObject();
      RopeTimerDebugDisplay.s_instance = gameObject.AddComponent<RopeTimerDebugDisplay>();
      gameObject.name = "RopeTimerDebugDisplay (Dynamically created)";
    }
    return RopeTimerDebugDisplay.s_instance;
  }

  private void Start()
  {
    if (HearthstoneApplication.IsPublic() || GameState.Get() == null)
      return;
    GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent), (object) null);
  }

  private void GameState_CreateGameEvent(GameState.CreateGamePhase createGamePhase, object userData)
  {
  }

  public bool EnableDebugDisplay(string func, string[] args, string rawArgs)
  {
    Network.Get().DebugRopeTimer();
    this.m_isDisplayed = true;
    return true;
  }

  public bool DisableDebugDisplay(string func, string[] args, string rawArgs)
  {
    Network.Get().DisableDebugRopeTimer();
    this.m_isDisplayed = false;
    return true;
  }

  private void Update()
  {
    if (HearthstoneApplication.IsPublic() || GameState.Get() == null || !this.m_isDisplayed)
      return;
    this.UpdateDisplay();
  }

  private string AppendLine(string inputString, string stringToAppend) => string.Format("{0}\n{1}", (object) inputString, (object) stringToAppend);

  private void UpdateDisplay()
  {
    if (this.m_debugInformation == null)
      return;
    string text = string.Format("Rope Timer\n Time remaining in turn: {0:F1}\n Base turn time: {1:F1}\n SlushTime: {2:F1}\n Total turn time: {3:F1}\nSlush time for opponent: {4:F1}", (object) ((float) this.m_debugInformation.MicrosecondsRemainingInTurn / 1000000f), (object) ((float) this.m_debugInformation.BaseMicrosecondsInTurn / 1000000f), (object) ((float) this.m_debugInformation.SlushTimeInMicroseconds / 1000000f), (object) ((float) this.m_debugInformation.TotalMicrosecondsInTurn / 1000000f), (object) ((float) this.m_debugInformation.OpponentSlushTimeInMicroseconds / 1000000f));
    Vector3 position = new Vector3((float) Screen.width, (float) Screen.height, 0.0f);
    DebugTextManager.Get().DrawDebugText(text, position, 0.0f, true);
  }

  public void OnRopeTimerDebugInformation(RopeTimerDebugInformation debugInfo) => this.m_debugInformation = debugInfo;
}
