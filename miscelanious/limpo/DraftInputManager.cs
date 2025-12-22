using Hearthstone;
using System.Collections.Generic;
using UnityEngine;

public class DraftInputManager : MonoBehaviour
{
  private static DraftInputManager s_instance;
  private int m_selectedIndex = -1;

  private void Awake() => DraftInputManager.s_instance = this;

  private void OnDestroy() => DraftInputManager.s_instance = (DraftInputManager) null;

  public static DraftInputManager Get() => DraftInputManager.s_instance;

  public void Unload()
  {
  }

  public bool HandleKeyboardInput()
  {
    DraftDisplay draftDisplay = DraftDisplay.Get();
    if ((Object) draftDisplay == (Object) null)
      return false;
    bool flag = draftDisplay.IsInHeroSelectMode();
    if (InputCollection.GetKeyUp(KeyCode.Escape) & flag)
    {
      draftDisplay.DoHeroCancelAnimation();
      return true;
    }
    CollectionDeck draftDeck = DraftManager.Get().GetDraftDeck();
    if (draftDisplay.GetDraftMode() == DraftDisplay.DraftMode.ACTIVE_DRAFT_DECK && InputCollection.GetKeyDown(KeyCode.C) && (InputCollection.GetKey(KeyCode.LeftCommand) || InputCollection.GetKey(KeyCode.LeftControl)))
    {
      ClipboardUtils.CopyToClipboard(draftDeck.GetShareableDeck().Serialize());
      UIStatus.Get().AddInfo(GameStrings.Get("GLUE_COLLECTION_DECK_COPIED_TOAST"));
    }
    if (!HearthstoneApplication.IsInternal())
      return false;
    List<DraftCardVisual> cardVisuals = DraftDisplay.Get().GetCardVisuals();
    if (cardVisuals == null || cardVisuals.Count == 0)
      return false;
    int index = -1;
    if (InputCollection.GetKeyUp(KeyCode.Alpha1))
      index = 0;
    else if (InputCollection.GetKeyUp(KeyCode.Alpha2))
      index = 1;
    else if (InputCollection.GetKeyUp(KeyCode.Alpha3))
      index = 2;
    if (index == -1 || cardVisuals.Count < index + 1)
      return false;
    if (flag && this.m_selectedIndex == index)
    {
      draftDisplay.ClickConfirmButton();
      this.m_selectedIndex = -1;
      return true;
    }
    DraftCardVisual draftCardVisual = cardVisuals[index];
    if ((Object) draftCardVisual == (Object) null)
      return false;
    if (flag)
      draftDisplay.DoHeroCancelAnimation();
    this.m_selectedIndex = index;
    draftCardVisual.ChooseThisCard();
    return true;
  }
}
