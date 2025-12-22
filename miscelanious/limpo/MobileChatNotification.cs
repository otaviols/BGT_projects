using System.ComponentModel;
using UnityEngine;

public class MobileChatNotification : MonoBehaviour
{
  private MobileChatNotification.State state;

  public event MobileChatNotification.NotifiedEvent Notified;

  private void OnEnable() => this.state = MobileChatNotification.State.None;

  private void OnDestroy()
  {
    if (GameState.Get() == null || SpectatorManager.Get().IsSpectatingOrWatching)
      return;
    GameState.Get().UnregisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
    GameState.Get().UnregisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
  }

  private void Update()
  {
    if (GameState.Get() == null || SpectatorManager.Get().IsSpectatingOrWatching)
    {
      this.state = MobileChatNotification.State.None;
    }
    else
    {
      GameState.Get().RegisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
      GameState.Get().RegisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
      if (GameState.Get().IsMulliganPhase())
      {
        if (this.state != MobileChatNotification.State.None)
          return;
        this.state = MobileChatNotification.State.GameStarted;
        this.FireNotification();
      }
      else
      {
        if (this.state != MobileChatNotification.State.GameStarted)
          return;
        this.state = GameState.Get().IsFriendlySidePlayerTurn() ? MobileChatNotification.State.YourTurn : MobileChatNotification.State.None;
        this.FireNotification();
      }
    }
  }

  private string GetStateText(MobileChatNotification.State state) => state == MobileChatNotification.State.None ? string.Empty : GameStrings.Get((typeof (MobileChatNotification.State).GetField(state.ToString()).GetCustomAttributes(false)[0] as DescriptionAttribute).Description);

  private void OnTurnChanged(int oldTurn, int newTurn, object userData)
  {
    if (!GameState.Get().IsFriendlySidePlayerTurn())
      return;
    this.state = MobileChatNotification.State.YourTurn;
    this.FireNotification();
  }

  private void OnTurnTimerUpdate(TurnTimerUpdate update, object userData)
  {
    if ((double) update.GetSecondsRemaining() <= 0.0 || !GameState.Get().IsFriendlySidePlayerTurn())
      return;
    this.state = MobileChatNotification.State.TurnCountdown;
    this.FireNotification();
  }

  private void FireNotification()
  {
    if (this.Notified == null || this.state == MobileChatNotification.State.None)
      return;
    this.Notified(this.GetStateText(this.state));
  }

  public delegate void NotifiedEvent(string text);

  private enum State
  {
    None,
    [Description("GLOBAL_MOBILECHAT_NOTIFICATION_MULLIGAIN")] GameStarted,
    [Description("GLOBAL_MOBILECHAT_NOTIFICATION_YOUR_TURN")] YourTurn,
    [Description("GLOBAL_MOBILECHAT_NOTIFICATION_TURN_COUNTDOWN")] TurnCountdown,
  }
}
