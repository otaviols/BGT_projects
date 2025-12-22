using Blizzard.GameService.SDK.Client.Integration;

public static class ChatUtils
{
  public static string GetMessage(BnetWhisper whisper) => ChatUtils.GetMessage(whisper.GetMessage());

  public static string GetMessage(string message) => Localization.GetLocale() == Locale.zhCN ? BattleNet.FilterProfanity(message) : message;

  public static bool TryGetFormattedDeckcodeMessage(
    string message,
    bool showHint,
    out string formattedDeckcodeMessage)
  {
    formattedDeckcodeMessage = string.Empty;
    if (message == null)
      return false;
    string deckName;
    if (ShareableMercenariesTeam.ParseDeckCode(message, out deckName) != null)
    {
      string str1;
      if (string.IsNullOrWhiteSpace(deckName))
        str1 = GameStrings.Format("GLOBAL_CHAT_MERCENARIES_PARTY_CODE_MESSAGE", (object) string.Empty);
      else
        str1 = GameStrings.Format("GLOBAL_CHAT_MERCENARIES_PARTY_CODE_WITH_NAME_MESSAGE", (object) deckName, (object) string.Empty);
      string str2 = str1;
      formattedDeckcodeMessage = str2;
      return true;
    }
    ShareableDeck deckCode = ShareableDeck.ParseDeckCode(message, out deckName);
    if (deckCode != null)
    {
      TAG_CLASS classFromDeck = ShareableDeck.ExtractClassFromDeck(deckCode);
      if (classFromDeck != TAG_CLASS.INVALID)
      {
        string className = GameStrings.GetClassName(classFromDeck);
        string str3;
        if (string.IsNullOrWhiteSpace(deckName))
          str3 = GameStrings.Format("GLOBAL_CHAT_DECK_CODE_MESSAGE", (object) className, (object) string.Empty);
        else
          str3 = GameStrings.Format("GLOBAL_CHAT_DECK_CODE_WITH_NAME_MESSAGE", (object) className, (object) deckName, (object) string.Empty);
        string str4 = str3;
        formattedDeckcodeMessage = str4;
        return true;
      }
    }
    return false;
  }

  public static bool TrySendDeckcodeFromClipboard(System.Action<string> onConfirmationCallback)
  {
    ShareableMercenariesTeam shareableMercenariesTeam = ShareableMercenariesTeam.DeserializeFromClipboard();
    if (shareableMercenariesTeam != null)
    {
      ChatUtils.ShowDeckcodePopup(shareableMercenariesTeam.Serialize(false), shareableMercenariesTeam.DeckName, onConfirmationCallback);
      return true;
    }
    ShareableDeck shareableDeck = ShareableDeck.DeserializeFromClipboard();
    if (shareableDeck == null)
      return false;
    ChatUtils.ShowDeckcodePopup(shareableDeck.Serialize(false), shareableDeck.DeckName, onConfirmationCallback);
    return true;
  }

  private static void ShowDeckcodePopup(
    string deckCode,
    string deckName,
    System.Action<string> onConfirmationCallback)
  {
    string deckCodeMessage = ShareableDeck.GenerateDeckCodeMessage(deckCode, deckName);
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_CHAT_SEND_DECK_TITLE"),
      m_text = GameStrings.Get("GLOBAL_CHAT_SEND_DECK_MESSAGE"),
      m_showAlertIcon = false,
      m_attentionCategory = UserAttentionBlocker.NONE,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response == AlertPopup.Response.CONFIRM)
          onConfirmationCallback(deckCodeMessage);
        ClipboardUtils.CopyToClipboard(string.Empty);
      })
    };
    DialogManager.Get().ShowPopup(info);
  }
}
