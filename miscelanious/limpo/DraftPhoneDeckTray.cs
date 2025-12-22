using System.Collections;
using UnityEngine;

public class DraftPhoneDeckTray : BasePhoneDeckTray
{
  private static DraftPhoneDeckTray s_instance;
  private bool m_showDisablePremiumPrompt = true;

  protected override void Awake()
  {
    base.Awake();
    DraftPhoneDeckTray.s_instance = this;
    DraftManager.Get().RegisterDraftDeckSetListener(new DraftManager.DraftDeckSet(this.OnDraftDeckInitialized));
    this.m_cardsContent.RegisterCardTileHeldListener(new DeckTrayCardListContent.CardTileHeld(this.OnCardTileHeld));
    this.m_cardsContent.RegisterCardTileReleaseListener(new DeckTrayCardListContent.CardTileRelease(this.OnCardTileRelease));
    this.m_cardsContent.RegisterCardCountUpdated(new DeckTrayCardListContent.CardCountChanged(((BasePhoneDeckTray) this).OnCardCountUpdated));
    CollectionInputMgr collectionInputMgr = CollectionInputMgr.Get();
    if (!((Object) collectionInputMgr != (Object) null))
      return;
    collectionInputMgr.SetScrollbar(this.m_scrollbar);
  }

  private void OnDestroy()
  {
    DraftManager.Get()?.RemoveDraftDeckSetListener(new DraftManager.DraftDeckSet(this.OnDraftDeckInitialized));
    CollectionManager.Get()?.ClearEditedDeck();
    DraftPhoneDeckTray.s_instance = (DraftPhoneDeckTray) null;
  }

  public static DraftPhoneDeckTray Get() => DraftPhoneDeckTray.s_instance;

  public void Initialize()
  {
    CollectionDeck draftDeck = DraftManager.Get().GetDraftDeck();
    if (draftDeck == null)
      return;
    this.OnDraftDeckInitialized(draftDeck);
  }

  private void OnDraftDeckInitialized(CollectionDeck draftDeck)
  {
    if (draftDeck == null)
    {
      Debug.LogError((object) "Draft deck is null.");
    }
    else
    {
      CollectionManager.Get().SetEditedDeck(draftDeck);
      this.OnCardCountUpdated(draftDeck.GetTotalCardCount());
      this.m_cardsContent.UpdateCardList();
    }
  }

  private IEnumerator ShowBigCard(DeckTrayDeckTileVisual cardTile, float delay)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DraftPhoneDeckTray draftPhoneDeckTray = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      draftPhoneDeckTray.m_showDisablePremiumPrompt = false;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    draftPhoneDeckTray.ShowDeckBigCard(cardTile, delay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(delay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override void OnCardTilePress(DeckTrayDeckTileVisual cardTile)
  {
    if (UniversalInputManager.Get().IsTouchMode())
    {
      this.m_showDisablePremiumPrompt = true;
      this.StartCoroutine(this.ShowBigCard(cardTile, 0.2f));
    }
    else
    {
      if (!((Object) CollectionInputMgr.Get() != (Object) null))
        return;
      this.HideDeckBigCard(cardTile);
    }
  }

  private void OnCardTileHeld(DeckTrayDeckTileVisual cardTile)
  {
    if (!((Object) CollectionInputMgr.Get() != (Object) null) || cardTile.GetActor().GetPremium() == TAG_PREMIUM.NORMAL)
      return;
    CollectionInputMgr.Get().GrabCardTile(cardTile, new InputMgr.OnCardDroppedCallback(this.OnDeckTileDropped), false);
  }

  private void OnCardTileRelease(DeckTrayDeckTileVisual cardTile)
  {
    if (this.m_isScrolling)
      return;
    this.StopCoroutine("ShowBigCard");
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.DRAFT || cardTile.GetActor().GetPremium() == TAG_PREMIUM.NORMAL || !this.m_showDisablePremiumPrompt)
      return;
    DraftManager.Get().PromptToDisablePremium();
  }

  private void OnDeckTileDropped()
  {
    if (this.m_isScrolling)
      return;
    DraftManager.Get().PromptToDisablePremium();
  }
}
