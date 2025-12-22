using System;
using UnityEngine;

public class AdventureDungeonCrawlDeckTray : BasePhoneDeckTray
{
  public PlayMakerFSM DeckTrayGlow;
  private CollectionDeck m_deck;

  protected override void Awake()
  {
    base.Awake();
    if (!SceneMgr.Get().IsInDuelsMode() || !((UnityEngine.Object) this.m_headerLabel != (UnityEngine.Object) null))
      return;
    this.m_headerLabel.GetComponent<UberText>().Text = GameStrings.Get("GLUE_PVPDR_DECK_TRAY_HEADER");
  }

  private void OnDestroy() => this.ClearEditingDeck();

  public void SetDungeonCrawlDeck(CollectionDeck deck, bool playGlowAnimation)
  {
    if (deck == null)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlDeckTray.SetDungeonCrawlDeck() - deck passed in is null!");
    }
    else
    {
      this.m_deck = deck;
      this.gameObject.SetActive(true);
      this.TagDeckForEditing();
      this.OnCardCountUpdated(deck.GetTotalCardCount());
      this.m_cardsContent.UpdateCardList();
      if (!playGlowAnimation || !((UnityEngine.Object) this.DeckTrayGlow != (UnityEngine.Object) null))
        return;
      this.DeckTrayGlow.SendEvent("Flash");
    }
  }

  public void OffsetDeckBigCardByVector(Vector3 offset) => this.m_deckBigCard.OffsetByVector(offset);

  public override void AddCard(string cardId, Actor animateFromActor, Action onCompleteCallback)
  {
    if (this.m_deck == null)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlDeckTray.AddCard() - no deck set!");
    }
    else
    {
      this.TagDeckForEditing();
      this.m_deck.AddCard(cardId, TAG_PREMIUM.NORMAL, false);
      base.AddCard(cardId, animateFromActor, onCompleteCallback);
    }
  }

  private void TagDeckForEditing()
  {
    if (this.m_deck == null)
      Log.Adventures.PrintError("AdventureDungeonCrawlDeckTray.TagForEdit() - no deck set!");
    else
      CollectionManager.Get().SetEditedDeck(this.m_deck);
  }

  private void ClearEditingDeck()
  {
    if (CollectionManager.Get() == null)
      return;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null || editedDeck != this.m_deck)
      return;
    CollectionManager.Get().ClearEditedDeck();
  }

  protected override void OnCardCountUpdated(int cardCount)
  {
    if (cardCount <= 0)
      return;
    if ((UnityEngine.Object) this.m_countLabelText != (UnityEngine.Object) null)
      this.m_countLabelText.Text = GameStrings.Get("GLUE_DECK_TRAY_CARD_COUNT_LABEL");
    if (!((UnityEngine.Object) this.m_countText != (UnityEngine.Object) null))
      return;
    string count = string.Format("{0}", (object) cardCount);
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.StartCoroutine(this.DelayCardCountUpdate(count));
    else
      this.m_countText.Text = count;
  }
}
