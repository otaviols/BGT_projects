using System.Collections;
using UnityEngine;

public class DeckTemplatePhoneTray : MonoBehaviour
{
  public DeckTrayCardListContent m_cardsContent;
  public UIBScrollable m_scrollbar;
  public TooltipZone m_deckHeaderTooltip;
  public DeckBigCard m_deckBigCard;
  public UberText m_countLabelText;
  public UberText m_countText;
  public GameObject m_headerLabel;
  public PlayMakerFSM m_deckTemplateChosenGlow;
  private static DeckTemplatePhoneTray s_instance;

  private void Awake()
  {
    DeckTemplatePhoneTray.s_instance = this;
    if ((Object) this.m_scrollbar != (Object) null)
    {
      this.m_scrollbar.Enable(false);
      this.m_scrollbar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.OnTouchScrollStarted));
    }
    if ((Object) this.m_deckBigCard != (Object) null)
      this.m_deckBigCard.SetHideBigHeroPower(true);
    this.m_cardsContent.RegisterCardTilePressListener(new DeckTrayCardListContent.CardTilePress(this.OnCardTilePress));
    this.m_cardsContent.RegisterCardTileOverListener(new DeckTrayCardListContent.CardTileOver(this.OnCardTileOver));
    this.m_cardsContent.RegisterCardTileOutListener(new DeckTrayCardListContent.CardTileOut(this.OnCardTileOut));
    this.m_cardsContent.RegisterCardTileReleaseListener(new DeckTrayCardListContent.CardTileRelease(this.OnCardTileRelease));
    this.m_cardsContent.ShowFakeDeck(true);
  }

  private void OnDestroy() => DeckTemplatePhoneTray.s_instance = (DeckTemplatePhoneTray) null;

  public static DeckTemplatePhoneTray Get() => DeckTemplatePhoneTray.s_instance;

  public bool MouseIsOver() => UniversalInputManager.Get().InputIsOver(this.gameObject);

  public DeckTrayCardListContent GetCardsContent() => this.m_cardsContent;

  public TooltipZone GetTooltipZone() => this.m_deckHeaderTooltip;

  private void OnCardCountUpdated(int cardCount)
  {
    string empty = string.Empty;
    string count = string.Empty;
    if (cardCount > 0)
    {
      if ((Object) this.m_headerLabel != (Object) null)
        this.m_headerLabel.SetActive(true);
      if (cardCount < CollectionManager.Get().GetDeckSize())
      {
        empty = GameStrings.Get("GLUE_DECK_TRAY_CARD_COUNT_LABEL");
        count = GameStrings.Format("GLUE_DECK_TRAY_COUNT", (object) cardCount, (object) CollectionManager.Get().GetDeckSize());
      }
    }
    this.m_countLabelText.Text = empty;
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.StartCoroutine(this.DelayCardCountUpdate(count));
    else
      this.m_countText.Text = count;
  }

  private IEnumerator DelayCardCountUpdate(string count)
  {
    yield return (object) new WaitForSeconds(0.5f);
    this.m_countText.Text = count;
  }

  private void ShowDeckBigCard(DeckTrayDeckTileVisual cardTile, float delay = 0.0f)
  {
    CollectionDeckTileActor actor = cardTile.GetActor();
    if ((Object) this.m_deckBigCard == (Object) null)
      return;
    EntityDef entityDef = actor.GetEntityDef();
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(entityDef.GetCardId(), new CardPortraitQuality(3, actor.GetPremium())))
    {
      GhostCard.Type ghostTypeFromSlot = GhostCard.GetGhostTypeFromSlot(this.m_cardsContent.GetEditingDeck(), cardTile.GetSlot());
      this.m_deckBigCard.Show(entityDef, actor.GetPremium(), cardDef, actor.gameObject.transform.position, ghostTypeFromSlot, delay);
      if (!UniversalInputManager.Get().IsTouchMode())
        return;
      cardTile.SetHighlight(true);
    }
  }

  private void HideDeckBigCard(DeckTrayDeckTileVisual cardTile, bool force = false)
  {
    CollectionDeckTileActor actor = cardTile.GetActor();
    if (!((Object) this.m_deckBigCard != (Object) null))
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
    if (!((Object) this.m_deckBigCard != (Object) null))
      return;
    this.m_deckBigCard.ForceHide();
  }

  private void OnCardTilePress(DeckTrayDeckTileVisual cardTile)
  {
    if (UniversalInputManager.Get().IsTouchMode())
    {
      this.ShowDeckBigCard(cardTile, 0.2f);
    }
    else
    {
      if (!((Object) CollectionInputMgr.Get() != (Object) null))
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

  public void FlashDeckTemplateHighlight()
  {
    if (!((Object) this.m_deckTemplateChosenGlow != (Object) null))
      return;
    this.m_deckTemplateChosenGlow.SendEvent("Flash");
  }
}
