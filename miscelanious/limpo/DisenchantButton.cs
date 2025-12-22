using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisenchantButton : CraftingButton
{
  private string m_lastwarnedCard;
  private List<AlertPopup.PopupInfo> PendingDisenchantWarnings = new List<AlertPopup.PopupInfo>();

  public override void EnableButton()
  {
    if (CraftingManager.Get().GetPendingClientTransaction().GetLastTransactionWasCrafting())
    {
      this.EnterUndoMode();
    }
    else
    {
      this.SetCraftingState(CraftingButton.CraftingState.Disenchant);
      this.labelText.Text = GameStrings.Get("GLUE_CRAFTING_DISENCHANT");
      base.EnableButton();
    }
  }

  protected override void OnRelease()
  {
    if (!Network.IsLoggedIn())
    {
      CollectionManager.ShowFeatureDisabledWhileOfflinePopup();
    }
    else
    {
      if (CraftingManager.Get().GetPendingServerTransaction() != null)
        return;
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.GetComponent<Animation>().Play("CardExchange_ButtonPress1_phone");
      else
        this.GetComponent<Animation>().Play("CardExchange_ButtonPress1");
      if (CraftingManager.Get().GetPendingClientTransaction().GetLastTransactionWasCrafting())
        this.DoDisenchant();
      else
        CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded(new CollectionManager.DelOnAllDeckContents(this.OnReadyToStartDisenchant));
    }
  }

  private void OnReadyToStartDisenchant()
  {
    if (!CraftingManager.Get().IsCardShowing())
      return;
    string cardId = CraftingManager.Get().GetShownActor().GetEntityDef().GetCardId();
    List<string> invalidDeckNames = this.GetPostDisenchantInvalidDeckNames();
    bool flag = CraftingManager.Get().GetNumOwnedIncludePending(new TAG_PREMIUM?()) > CollectionManager.Get().GetCard(cardId, TAG_PREMIUM.NORMAL).DefaultMaxCopiesPerDeck;
    if (invalidDeckNames.Count == 0)
    {
      if (CraftingManager.Get().GetNumClientTransactions() <= 0 && this.m_lastwarnedCard != cardId && !flag)
      {
        this.m_lastwarnedCard = cardId;
        this.PendingDisenchantWarnings.Add(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM_HEADER"),
          m_text = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM2_DESC"),
          m_showAlertIcon = true,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_responseCallback = new AlertPopup.ResponseCallback(this.OnConfirmDisenchantResponse)
        });
      }
    }
    else
    {
      string str1 = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM_DESC");
      foreach (string str2 in invalidDeckNames)
        str1 = str1 + "\n" + str2;
      this.PendingDisenchantWarnings.Add(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM_HEADER"),
        m_text = str1,
        m_showAlertIcon = false,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = new AlertPopup.ResponseCallback(this.OnConfirmDisenchantResponse)
      });
    }
    int cardDbId = GameUtils.TranslateCardIdToDbId(cardId);
    if (CollectionManager.Get().GetOwnedCards().Where<CollectibleCard>((Func<CollectibleCard, bool>) (x => GameUtils.IsClassicCardSet(x.Set) && x.GetEntityDef() != null && x.GetEntityDef().GetTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID) == cardDbId)).Any<CollectibleCard>() && !flag)
      this.PendingDisenchantWarnings.Add(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM_HEADER"),
        m_text = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM3_DESC"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = new AlertPopup.ResponseCallback(this.OnConfirmDisenchantResponse)
      });
    if (this.PendingDisenchantWarnings.Count > 0)
      this.ShowNextDisenchantWarning();
    else
      this.DoDisenchant();
  }

  private void ShowNextDisenchantWarning()
  {
    if (this.PendingDisenchantWarnings.Count == 0)
      return;
    AlertPopup.PopupInfo disenchantWarning = this.PendingDisenchantWarnings[0];
    this.PendingDisenchantWarnings.RemoveAt(0);
    DialogManager.Get().ShowPopup(disenchantWarning);
  }

  private void OnConfirmDisenchantResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      this.PendingDisenchantWarnings.Clear();
    else if (this.PendingDisenchantWarnings.Count > 0)
      this.ShowNextDisenchantWarning();
    else
      this.DoDisenchant();
  }

  private void DoDisenchant() => CraftingManager.Get().DisenchantButtonPressed();

  private List<string> GetPostDisenchantInvalidDeckNames()
  {
    Actor shownActor = CraftingManager.Get().GetShownActor();
    TAG_PREMIUM premium = shownActor.GetPremium();
    string cardId = shownActor.GetEntityDef().GetCardId();
    int counterpartCardId = GameUtils.GetFixedRewardCounterpartCardID(GameUtils.TranslateCardIdToDbId(cardId));
    if (counterpartCardId != 0 && GameUtils.IsClassicCard(counterpartCardId))
      cardId = GameUtils.TranslateDbIdToCardId(counterpartCardId);
    int copiesInCollection1 = CollectionManager.Get().GetTotalNumCopiesInCollection(cardId);
    int copiesInCollection2 = CollectionManager.Get().GetNumCopiesInCollection(cardId, premium);
    int ownedIncludePending = CraftingManager.Get().GetNumOwnedIncludePending(new TAG_PREMIUM?(premium));
    if (ownedIncludePending > 0)
      --ownedIncludePending;
    int num1 = ownedIncludePending;
    int num2 = copiesInCollection2 - num1;
    int num3 = copiesInCollection1 - num2;
    List<string> invalidDeckNames = new List<string>();
    foreach (CollectionDeck deck in CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK))
    {
      if (deck.GetOwnedCardCountInDeck(cardId, premium) > num3)
        invalidDeckNames.Add(deck.Name);
    }
    return invalidDeckNames;
  }
}
