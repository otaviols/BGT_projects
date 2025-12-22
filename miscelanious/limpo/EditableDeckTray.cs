using System.Collections;
using UnityEngine;

public abstract class EditableDeckTray : DeckTray
{
  public UIBButton m_doneButton;
  public GameObject m_backArrow;
  public UberText m_myDecksLabel;
  public UberText m_countLabelText;
  public UberText m_countText;
  public TooltipZone m_deckHeaderTooltip;

  protected override IEnumerator UpdateTrayMode()
  {
    EditableDeckTray editableDeckTray = this;
    DeckTray.DeckContentTypes oldContentType = editableDeckTray.m_currentContent;
    DeckTray.DeckContentTypes newContentType = editableDeckTray.m_contentToSet;
    if (editableDeckTray.m_settingNewMode || editableDeckTray.m_currentContent == editableDeckTray.m_contentToSet || editableDeckTray.m_contentToSet == DeckTray.DeckContentTypes.INVALID)
    {
      editableDeckTray.m_updatingTrayMode = false;
    }
    else
    {
      editableDeckTray.AllowInput(false);
      editableDeckTray.m_contentToSet = DeckTray.DeckContentTypes.INVALID;
      editableDeckTray.m_currentContent = DeckTray.DeckContentTypes.INVALID;
      editableDeckTray.m_settingNewMode = true;
      editableDeckTray.m_updatingTrayMode = true;
      DeckTrayContent oldContent = (DeckTrayContent) null;
      DeckTrayContent newContent = (DeckTrayContent) null;
      editableDeckTray.m_contents.TryGetValue(oldContentType, out oldContent);
      editableDeckTray.m_contents.TryGetValue(newContentType, out newContent);
      if ((Object) oldContent != (Object) null)
      {
        while (!oldContent.PreAnimateContentExit())
          yield return (object) null;
      }
      if ((Object) newContent != (Object) null)
      {
        while (!newContent.PreAnimateContentEntrance())
          yield return (object) null;
      }
      editableDeckTray.SaveScrollbarPosition(oldContentType);
      editableDeckTray.TryDisableScrollbar();
      if ((Object) oldContent != (Object) null)
      {
        oldContent.SetModeActive(false);
        while (!oldContent.AnimateContentExitStart())
          yield return (object) null;
        Log.DeckTray.Print("OLD: {0} AnimateContentExitStart - finished", (object) oldContentType);
        while (!oldContent.AnimateContentExitEnd())
          yield return (object) null;
        Log.DeckTray.Print("OLD: {0} AnimateContentExitEnd - finished", (object) oldContentType);
      }
      editableDeckTray.m_currentContent = newContentType;
      if ((Object) newContent != (Object) null)
      {
        newContent.SetModeTrying(true);
        while (!newContent.AnimateContentEntranceStart())
          yield return (object) null;
        Log.DeckTray.Print("NEW: {0} AnimateContentEntranceStart - finished", (object) newContentType);
        while (!newContent.AnimateContentEntranceEnd())
          yield return (object) null;
        Log.DeckTray.Print("NEW: {0} AnimateContentEntranceEnd - finished", (object) newContentType);
        newContent.SetModeActive(true);
        newContent.SetModeTrying(false);
      }
      editableDeckTray.TryEnableScrollbar();
      if ((Object) newContent != (Object) null)
      {
        while (!newContent.PostAnimateContentEntrance())
          yield return (object) null;
      }
      if ((Object) oldContent != (Object) null)
      {
        while (!oldContent.PostAnimateContentExit())
          yield return (object) null;
      }
      if (editableDeckTray.m_currentContent != DeckTray.DeckContentTypes.Decks && editableDeckTray.m_currentContent != DeckTray.DeckContentTypes.Teams && editableDeckTray.m_currentContent != DeckTray.DeckContentTypes.Mercs)
        editableDeckTray.m_cardsContent.TriggerCardCountUpdate();
      editableDeckTray.m_settingNewMode = false;
      editableDeckTray.FireModeSwitchedEvent();
      editableDeckTray.UpdateDoneButtonText();
      if (editableDeckTray.m_contentToSet != DeckTray.DeckContentTypes.INVALID)
      {
        editableDeckTray.StartCoroutine(editableDeckTray.UpdateTrayMode());
      }
      else
      {
        editableDeckTray.m_updatingTrayMode = false;
        editableDeckTray.AllowInput(true);
      }
    }
  }

  public abstract void UpdateDoneButtonText();

  protected override void HideUnseenDeckTrays()
  {
    int currentContent = (int) this.m_currentContent;
  }

  protected override void OnCardTileOut(DeckTrayDeckTileVisual cardTile) => this.HideDeckBigCard(cardTile);
}
