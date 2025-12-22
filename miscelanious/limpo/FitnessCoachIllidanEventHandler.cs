using System.Collections.Generic;
using UnityEngine;

public class FitnessCoachIllidanEventHandler : CollectionCardEventHandler
{
  public PlayMakerFSM m_cardAddedGlow;
  public PlayMakerFSM m_cardRemovedGlow;

  public override void OnCardAdded(
    CollectionDeckTray collectionDeckTray,
    CollectionDeck deck,
    EntityDef cardEntityDef,
    TAG_PREMIUM premium,
    Actor animateActor)
  {
    if (cardEntityDef.GetCardId() != "BT_900proto")
      Log.CollectionManager.PrintError("{0}.OnCardAdded(): Added card's ID is {1} and not Fitness Coach Illidan's ({2})!", (object) this, (object) cardEntityDef.GetCardId(), (object) "BT_900proto");
    else if (deck.GetTotalCardCount() <= 1)
    {
      this.AddIllidan(collectionDeckTray, deck, cardEntityDef, premium, animateActor);
    }
    else
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = "Demon Hunter?",
        m_text = "Adding Fitness Coach Illidan to your deck will remove all other cards. Continue?",
        m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
        m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
        m_showAlertIcon = false,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response == AlertPopup.Response.CONFIRM)
          {
            for (int index = deck.GetSlots().Count - 1; index >= 0; --index)
            {
              CollectionDeckSlot slot = deck.GetSlots()[index];
              if (slot.CardID != "BT_900proto")
                collectionDeckTray.RemoveAllCopiesOfCard(slot.CardID);
            }
            this.AddIllidan(collectionDeckTray, deck, cardEntityDef, premium, animateActor);
          }
          else
            collectionDeckTray.RemoveAllCopiesOfCard("BT_900proto");
        })
      };
      DialogManager.Get().ShowPopup(info);
    }
  }

  public override bool ShouldUpdateVisuals() => false;

  private void AddIllidan(
    CollectionDeckTray collectionDeckTray,
    CollectionDeck deck,
    EntityDef cardEntityDef,
    TAG_PREMIUM premium,
    Actor animateActor)
  {
    if ((Object) this.m_cardAddedGlow != (Object) null)
      this.m_cardAddedGlow.SendEvent("DoAnim");
    collectionDeckTray.GetDecksContent().UpdateEditingDeckBoxVisual("HERO_10", new TAG_PREMIUM?(TAG_PREMIUM.NORMAL));
    deck.HeroCardID = "HERO_10";
    deck.HeroOverridden = true;
    collectionDeckTray.RemoveAllCopiesOfCard("BT_900proto");
    CollectionManager.Get().GetCollectibleDisplay().ResetFilters();
    List<TAG_CLASS> tagClassList = new List<TAG_CLASS>()
    {
      TAG_CLASS.DEMONHUNTER
    };
    CollectionPageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as CollectionPageManager;
    collectionDeckTray.GetCardsContent().UpdateCardList(cardEntityDef, animateFromActor: animateActor);
    CollectionDeck deck1 = deck;
    List<TAG_CLASS> deckClasses = tagClassList;
    pageManager.UpdateFiltersForDeck(deck1, deckClasses, false);
    CollectionManager.Get().GetCollectibleDisplay().UpdateCurrentPageCardLocks(true);
  }
}
