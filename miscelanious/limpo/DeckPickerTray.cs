using UnityEngine;

public class DeckPickerTray
{
  private static DeckPickerTray s_instance;
  private bool m_registeredHandlers;
  private AbsDeckPickerTrayDisplay m_deckPickerTrayDisplay;

  public static DeckPickerTray Get()
  {
    if (DeckPickerTray.s_instance == null)
      DeckPickerTray.s_instance = new DeckPickerTray();
    return DeckPickerTray.s_instance;
  }

  public static bool IsInitialized() => DeckPickerTray.s_instance != null;

  public static AbsDeckPickerTrayDisplay GetTray() => DeckPickerTray.s_instance == null ? (AbsDeckPickerTrayDisplay) null : DeckPickerTray.s_instance.m_deckPickerTrayDisplay;

  public void SetDeckPickerTrayDisplayReference(AbsDeckPickerTrayDisplay deckPickerTrayDisplay) => this.m_deckPickerTrayDisplay = deckPickerTrayDisplay;

  public void RegisterHandlers()
  {
    if (this.m_registeredHandlers)
      return;
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.m_registeredHandlers = true;
  }

  public void UnregisterHandlers()
  {
    if (!this.m_registeredHandlers)
      return;
    GameMgr.Get()?.UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.m_registeredHandlers = false;
  }

  public void Unload() => this.UnregisterHandlers();

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    if ((Object) this.m_deckPickerTrayDisplay == (Object) null)
    {
      if ((Object) DeckPickerTrayDisplay.Get() != (Object) null)
        this.m_deckPickerTrayDisplay = (AbsDeckPickerTrayDisplay) DeckPickerTrayDisplay.Get();
      else if ((Object) GuestHeroPickerTrayDisplay.Get() != (Object) null)
      {
        this.m_deckPickerTrayDisplay = (AbsDeckPickerTrayDisplay) GuestHeroPickerTrayDisplay.Get();
      }
      else
      {
        Debug.LogError((object) "DeckPickerTray has OnFindGameEvent registered but the HeroPickerTrayDisplay does not exist. Exiting...");
        return false;
      }
    }
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
        this.m_deckPickerTrayDisplay.HandleGameStartupFailure();
        break;
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
        this.m_deckPickerTrayDisplay.HandleGameStartupFailure();
        break;
      case FindGameState.SERVER_GAME_STARTED:
        this.m_deckPickerTrayDisplay.OnServerGameStarted();
        break;
      case FindGameState.SERVER_GAME_CANCELED:
        this.m_deckPickerTrayDisplay.OnServerGameCanceled();
        break;
    }
    return false;
  }
}
