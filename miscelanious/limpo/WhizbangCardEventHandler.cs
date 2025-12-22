using System;
using System.Linq;

public class WhizbangCardEventHandler : CollectionCardEventHandler
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
    string cardId = cardEntityDef.GetCardId();
    if (!GameDbf.GetIndex().HasCardPlayerDeckOverride(cardId))
    {
      Log.CollectionManager.PrintError("{0}.OnCardAdded(): Added card's ID is {1} and not one of the valid cardIds ({2})!", (object) this, (object) cardId, (object) string.Join(", ", GameDbf.GetIndex().GetAllCardPlayerDeckOverrides().Select<CardPlayerDeckOverrideDbfRecord, string>((Func<CardPlayerDeckOverrideDbfRecord, string>) (r => GameUtils.TranslateDbIdToCardId(r.CardId))).ToArray<string>()));
    }
    else
    {
      CardPlayerDeckOverrideDbfRecord playerDeckOverride = GameDbf.GetIndex().GetCardPlayerDeckOverride(cardId);
      if (deck.GetTotalCardCount() <= 1)
      {
        this.AddWhizbang(playerDeckOverride, collectionDeckTray, deck, cardEntityDef, premium, animateActor);
      }
      else
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = (string) playerDeckOverride.AddToDeckWarningHeader,
          m_text = (string) playerDeckOverride.AddToDeckWarningBody,
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
                if (slot.CardID != cardId)
                  collectionDeckTray.RemoveAllCopiesOfCard(slot.CardID);
              }
              this.AddWhizbang(playerDeckOverride, collectionDeckTray, deck, cardEntityDef, premium, animateActor);
            }
            else
              collectionDeckTray.RemoveAllCopiesOfCard(cardId);
          })
        };
        DialogManager.Get().ShowPopup(info);
      }
    }
  }

  public override void OnCardRemoved(CollectionDeckTray collectionDeckTray, CollectionDeck deck)
  {
    int uiHeroOverrideCardDbId = string.IsNullOrEmpty(deck.UIHeroOverrideCardID) ? 0 : GameUtils.TranslateCardIdToDbId(deck.UIHeroOverrideCardID);
    if (!GameDbf.GetIndex().GetAllCardPlayerDeckOverrides().Any<CardPlayerDeckOverrideDbfRecord>((Func<CardPlayerDeckOverrideDbfRecord, bool>) (r => r.HeroCardId != 0 && r.HeroCardId == uiHeroOverrideCardDbId)))
      return;
    if ((UnityEngine.Object) this.m_cardRemovedGlow != (UnityEngine.Object) null)
      this.m_cardRemovedGlow.SendEvent("DoAnim");
    collectionDeckTray.GetDecksContent().UpdateEditingDeckBoxVisual(deck.HeroCardID);
    deck.UIHeroOverrideCardID = string.Empty;
    deck.UIHeroOverridePremium = TAG_PREMIUM.NORMAL;
    deck.Name = GameStrings.Format("GLOBAL_BASIC_DECK_NAME", (object) GameStrings.GetClassName(deck.GetClass()));
    collectionDeckTray.GetEditingDeckBox().SetDeckName(deck.Name);
    CollectionManager.Get().GetCollectibleDisplay().GetPageManager().UpdateVisibleTabs();
    CollectionManager.Get().OnUIHeroOverrideCardRemoved();
  }

  public override bool ShouldUpdateVisuals() => false;

  private void AddWhizbang(
    CardPlayerDeckOverrideDbfRecord playerDeckOverride,
    CollectionDeckTray collectionDeckTray,
    CollectionDeck deck,
    EntityDef cardEntityDef,
    TAG_PREMIUM premium,
    Actor animateActor)
  {
    if ((UnityEngine.Object) this.m_cardAddedGlow != (UnityEngine.Object) null)
      this.m_cardAddedGlow.SendEvent("DoAnim");
    string cardId = GameUtils.TranslateDbIdToCardId(playerDeckOverride.HeroCardId);
    collectionDeckTray.GetDecksContent().UpdateEditingDeckBoxVisual(cardId, new TAG_PREMIUM?(premium));
    deck.UIHeroOverrideCardID = cardId;
    deck.UIHeroOverridePremium = premium;
    deck.Name = (string) playerDeckOverride.DeckName;
    collectionDeckTray.GetEditingDeckBox().SetDeckName(deck.Name);
    collectionDeckTray.GetCardsContent().UpdateCardList(cardEntityDef, animateFromActor: animateActor);
    CollectionManager.Get().GetCollectibleDisplay().UpdateCurrentPageCardLocks(true);
    CollectionManager.Get().GetCollectibleDisplay().GetPageManager().UpdateVisibleTabs();
  }
}
