using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePhoneDeckTray : MonoBehaviour
{
  public DeckTrayCardListContent m_cardsContent;
  public UIBScrollable m_scrollbar;
  public TooltipZone m_deckHeaderTooltip;
  public DeckBigCard m_deckBigCard;
  public UberText m_countLabelText;
  public UberText m_countText;
  public GameObject m_headerLabel;
  protected bool m_isScrolling;

  public Dictionary<long, long> CardIdToCreatorMap { get; set; }

  protected virtual void Awake()
  {
    if ((UnityEngine.Object) this.m_scrollbar != (UnityEngine.Object) null)
    {
      this.m_scrollbar.Enable(false);
      this.m_scrollbar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.OnTouchScrollStarted));
      this.m_scrollbar.AddTouchScrollEndedListener(new UIBScrollable.OnTouchScrollEnded(this.OnTouchScrollEnded));
    }
    this.m_cardsContent.SetInArena(true);
    this.m_cardsContent.RegisterCardTilePressListener(new DeckTrayCardListContent.CardTilePress(this.OnCardTilePress));
    this.m_cardsContent.RegisterCardTileOverListener(new DeckTrayCardListContent.CardTileOver(this.OnCardTileOver));
    this.m_cardsContent.RegisterCardTileOutListener(new DeckTrayCardListContent.CardTileOut(this.OnCardTileOut));
    this.m_cardsContent.RegisterCardTileReleaseListener(new DeckTrayCardListContent.CardTileRelease(this.OnCardTileRelease));
    this.m_cardsContent.RegisterCardCountUpdated(new DeckTrayCardListContent.CardCountChanged(this.OnCardCountUpdated));
  }

  public bool MouseIsOver() => UniversalInputManager.Get().InputIsOver(this.gameObject) || this.m_cardsContent.MouseIsOverDeckHelperButton(Box.Get().GetCamera()) || this.m_cardsContent.MouseIsOverDeckCardTile();

  public virtual void AddCard(string cardID, Actor animateFromActor = null, Action onCompleteCallback = null) => this.m_cardsContent.UpdateCardList(cardID, animateFromActor: animateFromActor, onCompleteCallback: onCompleteCallback);

  public DeckTrayCardListContent GetCardsContent() => this.m_cardsContent;

  public TooltipZone GetTooltipZone() => this.m_deckHeaderTooltip;

  protected virtual void OnCardCountUpdated(int cardCount)
  {
    string empty = string.Empty;
    string count = string.Empty;
    if (cardCount > 0)
    {
      if ((UnityEngine.Object) this.m_headerLabel != (UnityEngine.Object) null)
        this.m_headerLabel.SetActive(true);
      if (cardCount < CollectionManager.Get().GetDeckSize())
      {
        empty = GameStrings.Get("GLUE_DECK_TRAY_CARD_COUNT_LABEL");
        count = GameStrings.Format("GLUE_DECK_TRAY_COUNT", (object) cardCount, (object) CollectionManager.Get().GetDeckSize());
      }
    }
    if ((UnityEngine.Object) this.m_countLabelText != (UnityEngine.Object) null)
      this.m_countLabelText.Text = empty;
    if (!((UnityEngine.Object) this.m_countText != (UnityEngine.Object) null))
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.StartCoroutine(this.DelayCardCountUpdate(count));
    else
      this.m_countText.Text = count;
  }

  protected IEnumerator DelayCardCountUpdate(string count)
  {
    yield return (object) new WaitForSeconds(0.5f);
    if (!((UnityEngine.Object) this.m_countText == (UnityEngine.Object) null))
      this.m_countText.Text = count;
  }

  protected void ShowDeckBigCard(DeckTrayDeckTileVisual cardTile, float delay = 0.0f)
  {
    CollectionDeckTileActor actor = cardTile.GetActor();
    if ((UnityEngine.Object) this.m_deckBigCard == (UnityEngine.Object) null)
      return;
    EntityDef entityDef = actor.GetEntityDef();
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(entityDef.GetCardId()))
    {
      this.m_deckBigCard.SetCreatorName(this.GetCreatorNameFromChildCardDbId((long) GameUtils.TranslateCardIdToDbId(entityDef.GetCardId())));
      this.m_deckBigCard.Show(entityDef, actor.GetPremium(), cardDef, actor.gameObject.transform.position, GhostCard.Type.NONE, delay);
      if (!UniversalInputManager.Get().IsTouchMode())
        return;
      cardTile.SetHighlight(true);
    }
  }

  protected void HideDeckBigCard(DeckTrayDeckTileVisual cardTile, bool force = false)
  {
    CollectionDeckTileActor actor = cardTile.GetActor();
    if (!((UnityEngine.Object) this.m_deckBigCard != (UnityEngine.Object) null))
      return;
    if (force)
      this.m_deckBigCard.ForceHide();
    else
      this.m_deckBigCard.Hide(actor.GetEntityDef(), actor.GetPremium());
    if (!UniversalInputManager.Get().IsTouchMode())
      return;
    cardTile.SetHighlight(false);
  }

  private void OnTouchScrollStarted()
  {
    this.m_isScrolling = true;
    if (!((UnityEngine.Object) this.m_deckBigCard != (UnityEngine.Object) null))
      return;
    this.m_deckBigCard.ForceHide();
  }

  private void OnTouchScrollEnded() => this.m_isScrolling = false;

  protected virtual void OnCardTilePress(DeckTrayDeckTileVisual cardTile)
  {
    if (UniversalInputManager.Get().IsTouchMode())
    {
      this.ShowDeckBigCard(cardTile, 0.2f);
    }
    else
    {
      if (!((UnityEngine.Object) CollectionInputMgr.Get() != (UnityEngine.Object) null) || SceneMgr.Get().IsInDuelsMode() && !PvPDungeonRunScene.IsEditingDeck())
        return;
      this.HideDeckBigCard(cardTile);
    }
  }

  private void OnCardTileOver(DeckTrayDeckTileVisual cardTile)
  {
    if (UniversalInputManager.Get().IsTouchMode())
      return;
    this.ShowDeckBigCard(cardTile);
  }

  private void OnCardTileOut(DeckTrayDeckTileVisual cardTile) => this.HideDeckBigCard(cardTile);

  private void OnCardTileRelease(DeckTrayDeckTileVisual cardTile)
  {
    if (!UniversalInputManager.Get().IsTouchMode())
      return;
    this.HideDeckBigCard(cardTile);
  }

  private string GetCreatorNameFromChildCardDbId(long cardId)
  {
    long id;
    if (this.CardIdToCreatorMap == null || !this.CardIdToCreatorMap.TryGetValue(cardId, out id))
      return string.Empty;
    CardDbfRecord record = GameDbf.Card.GetRecord((int) id);
    return record == null ? string.Empty : (string) record.Name;
  }
}
