using Hearthstone.Core;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class DeckTrayCardListContent : DeckTrayContent
{
  [CustomEditField(Sections = "Card Tile Settings")]
  public float m_cardTileHeight = 2.45f;
  [CustomEditField(Sections = "Card Tile Settings")]
  public float m_cardHelpButtonHeight = 3f;
  [CustomEditField(Sections = "Card Tile Settings")]
  public float m_deckCardBarFlareUpInterval = 0.075f;
  [CustomEditField(Sections = "Card Tile Settings")]
  public GameObject m_phoneDeckTileBone;
  [CustomEditField(Sections = "Card Tile Settings")]
  public Vector3 m_cardTileOffset = Vector3.zero;
  [CustomEditField(Sections = "Card Tile Settings")]
  public float m_cardTileSlotLocalHeight;
  [CustomEditField(Sections = "Card Tile Settings")]
  public Vector3 m_cardTileSlotLocalScaleVec3 = new Vector3(0.01f, 0.02f, 0.01f);
  [CustomEditField(Sections = "Card Tile Settings")]
  public bool m_forceUseFullScaleDeckTileActors;
  [CustomEditField(Sections = "Card Tile Name Text PC Settings")]
  public Vector3 m_cardNameTextDefaultPositionPC = new Vector3(1.2083f, 0.2267f, 0.0303f);
  [CustomEditField(Sections = "Card Tile Name Text PC Settings")]
  public Vector3 m_cardNameTextDeathKnightPositionPC = new Vector3(0.8f, 0.2267f, 0.0303f);
  [CustomEditField(Sections = "Card Tile Name Text PC Settings")]
  public float m_cardNameTextDefaultWidthPC = 17.43f;
  [CustomEditField(Sections = "Card Tile Name Text PC Settings")]
  public float m_cardNameTextDeathKnightWidthPC = 16f;
  [CustomEditField(Sections = "Card Tile Name Text Phone Settings")]
  public Vector3 m_cardNameTextDefaultPositionPhone = new Vector3(5.24f, 0.23f, 0.03f);
  [CustomEditField(Sections = "Card Tile Name Text Phone Settings")]
  public Vector3 m_cardNameTextDeathKnightPositionPhone = new Vector3(4f, 0.23f, 0.03f);
  [CustomEditField(Sections = "Card Tile Name Text Phone Settings")]
  public float m_cardNameTextDefaultWidthPhone = 8.42f;
  [CustomEditField(Sections = "Card Tile Name Text Phone Settings")]
  public float m_cardNameTextDeathKnightWidthPhone = 8.42f;
  [CustomEditField(Sections = "Deck Help")]
  public UIBButton m_smartDeckCompleteButton;
  [CustomEditField(Sections = "Deck Help")]
  public UIBButton m_deckTemplateHelpButton;
  [CustomEditField(Sections = "Deck Help")]
  public float m_deckTemplateHelpButtonDeathKnightPosY = -0.13f;
  [CustomEditField(Sections = "Other Objects")]
  public GameObject m_deckCompleteHighlight;
  [CustomEditField(Sections = "Other Objects")]
  public GameObject m_runeIndicatorSpacer;
  [CustomEditField(Sections = "Scroll Settings")]
  public UIBScrollable m_scrollbar;
  [CustomEditField(Sections = "Scroll Settings")]
  public BoxCollider m_LockedScrollBounds;
  private const string ADD_CARD_TO_DECK_SOUND = "collection_manager_card_add_to_deck_instant.prefab:06df359c4026d7e47b06a4174f33e3ef";
  private const float CARD_MOVEMENT_TIME = 0.3f;
  private Vector3 m_originalLocalPosition;
  private List<DeckTrayDeckTileVisual> m_cardTiles = new List<DeckTrayDeckTileVisual>();
  private List<DeckTrayCardListContent.CardTileHeld> m_cardTileHeldListeners = new List<DeckTrayCardListContent.CardTileHeld>();
  private List<DeckTrayCardListContent.CardTilePress> m_cardTilePressListeners = new List<DeckTrayCardListContent.CardTilePress>();
  private List<DeckTrayCardListContent.CardTileTap> m_cardTileTapListeners = new List<DeckTrayCardListContent.CardTileTap>();
  private List<DeckTrayCardListContent.CardTileOver> m_cardTileOverListeners = new List<DeckTrayCardListContent.CardTileOver>();
  private List<DeckTrayCardListContent.CardTileOut> m_cardTileOutListeners = new List<DeckTrayCardListContent.CardTileOut>();
  private List<DeckTrayCardListContent.CardTileRelease> m_cardTileReleaseListeners = new List<DeckTrayCardListContent.CardTileRelease>();
  private List<DeckTrayCardListContent.CardTileRightClicked> m_cardTileRightClickedListeners = new List<DeckTrayCardListContent.CardTileRightClicked>();
  private List<DeckTrayCardListContent.CardCountChanged> m_cardCountChangedListeners = new List<DeckTrayCardListContent.CardCountChanged>();
  private List<DefLoader.DisposableCardDef> m_cardDefs = new List<DefLoader.DisposableCardDef>();
  private bool m_animating;
  private bool m_loading;
  private const float DECK_HELP_BUTTON_EMPTY_DECK_Y_LOCAL_POS = -0.01194457f;
  private const float DECK_HELP_BUTTON_Y_TILE_OFFSET = -0.04915909f;
  private bool m_inArena;
  private CollectionDeck m_templateFakeDeck = new CollectionDeck();
  private bool m_isShowingFakeDeck;
  private bool m_hasFinishedEntering;
  private bool m_hasFinishedExiting = true;
  private Notification m_deckHelpPopup;
  private Vector3 m_deckTemplateHelpButtonOriginalLocalPosition;

  private float TemplateDeckHelpButtonHeight => this.m_deckTemplateHelpButton.GetComponent<UIBScrollableItem>().m_size.z * this.m_cardTileSlotLocalScaleVec3.z;

  private float RuneIndicatorSpacerHeight => !(bool) (UnityEngine.Object) this.m_runeIndicatorSpacer ? 0.0f : this.m_runeIndicatorSpacer.GetComponent<UIBScrollableItem>().m_size.z * this.m_cardTileSlotLocalScaleVec3.z;

  protected override void Awake()
  {
    base.Awake();
    if ((UnityEngine.Object) this.m_smartDeckCompleteButton != (UnityEngine.Object) null)
    {
      this.m_smartDeckCompleteButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeckCompleteButtonPress));
      this.m_smartDeckCompleteButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDeckCompleteButtonOver));
      this.m_smartDeckCompleteButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnDeckCompleteButtonOut));
    }
    if ((UnityEngine.Object) this.m_deckTemplateHelpButton != (UnityEngine.Object) null)
    {
      this.m_deckTemplateHelpButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeckTemplateHelpButtonPress));
      this.m_deckTemplateHelpButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDeckTemplateHelpButtonOver));
      this.m_deckTemplateHelpButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnDeckTemplateHelpButtonOut));
      this.m_deckTemplateHelpButtonOriginalLocalPosition = this.m_deckTemplateHelpButton.transform.localPosition;
    }
    this.m_originalLocalPosition = this.transform.localPosition;
    this.m_hasFinishedEntering = false;
  }

  protected override void OnDestroy()
  {
    this.m_cardDefs.DisposeValuesAndClear<DefLoader.DisposableCardDef>();
    base.OnDestroy();
  }

  public override bool AnimateContentEntranceStart()
  {
    if (this.m_loading)
      return false;
    this.m_animating = true;
    this.m_hasFinishedEntering = false;
    Action<object> action = (Action<object>) (_1 =>
    {
      this.UpdateDeckCompleteHighlight();
      this.ShowDeckEditingTipsIfNeeded();
      this.m_animating = false;
    });
    CollectionDeck editingDeck = this.GetEditingDeck();
    if (editingDeck != null)
    {
      this.transform.localPosition = this.GetOffscreenLocalPosition();
      iTween.StopByName(this.gameObject, "position");
      iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.m_originalLocalPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.3f, (object) "easeType", (object) iTween.EaseType.easeOutQuad, (object) "oncomplete", (object) action, (object) "name", (object) "position"));
      if (editingDeck.GetTotalCardCount() > 0)
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_new_deck_moves_up_tray.prefab:13650cd587089e14d9a297c8de6057f1", this.gameObject);
      this.UpdateCardList(false);
    }
    else
      action((object) null);
    return true;
  }

  public override bool AnimateContentEntranceEnd()
  {
    if (this.m_animating)
      return false;
    this.m_hasFinishedEntering = true;
    this.FireCardCountChangedEvent();
    return true;
  }

  public override bool AnimateContentExitStart()
  {
    if (this.m_animating)
      return false;
    this.m_animating = true;
    this.m_hasFinishedExiting = false;
    if ((UnityEngine.Object) this.m_deckCompleteHighlight != (UnityEngine.Object) null)
      this.m_deckCompleteHighlight.SetActive(false);
    iTween.StopByName(this.gameObject, "position");
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.GetOffscreenLocalPosition(), (object) "isLocal", (object) true, (object) "time", (object) 0.3f, (object) "easeType", (object) iTween.EaseType.easeInQuad, (object) "name", (object) "position"));
    if ((UnityEngine.Object) HeroPickerDisplay.Get() == (UnityEngine.Object) null || !HeroPickerDisplay.Get().IsShown())
      SoundManager.Get().LoadAndPlay((AssetReference) "panel_slide_off_deck_creation_screen.prefab:b0d25fc984ec05d4fbea7480b611e5ad", this.gameObject);
    Processor.ScheduleCallback(0.5f, false, (Processor.ScheduledCallback) (o => this.m_animating = false));
    return true;
  }

  public override bool AnimateContentExitEnd()
  {
    this.m_hasFinishedExiting = true;
    return !this.m_animating;
  }

  public bool HasFinishedEntering() => this.m_hasFinishedEntering;

  public bool HasFinishedExiting() => this.m_hasFinishedExiting;

  public override void OnEditedDeckChanged(
    CollectionDeck newDeck,
    CollectionDeck oldDeck,
    bool isNewDeck)
  {
    if (newDeck == null)
      return;
    this.LoadCardPrefabs(newDeck.GetSlots());
    if (!this.IsModeActive())
      return;
    this.ShowDeckHelpButtonIfNeeded();
  }

  public void ShowDeckHelper(CollectionDeckSlot slotToReplace, bool replaceSingleSlotOnly)
  {
    if (!CollectionManager.Get().IsInEditMode() || !(bool) (UnityEngine.Object) DeckHelper.Get())
      return;
    if (!Network.IsLoggedIn())
    {
      CollectionManager.ShowFeatureDisabledWhileOfflinePopup();
    }
    else
    {
      DeckHelper.DelCompleteCallback onCompleteCallback = (DeckHelper.DelCompleteCallback) (chosenCards =>
      {
        if (!((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null))
          return;
        CollectionDeckTray.Get().OnCardManuallyAddedByUser_CheckSuggestions((IEnumerable<EntityDef>) chosenCards);
      });
      DeckHelper.Get().Show(slotToReplace, replaceSingleSlotOnly, onCompleteCallback);
    }
  }

  public bool MouseIsOverDeckHelperButton(Camera camera) => (UnityEngine.Object) this.m_smartDeckCompleteButton != (UnityEngine.Object) null && this.m_smartDeckCompleteButton.gameObject.activeInHierarchy && UniversalInputManager.Get().InputIsOver(camera, this.m_smartDeckCompleteButton.gameObject);

  public bool MouseIsOverDeckCardTile()
  {
    foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
    {
      if (UniversalInputManager.Get().InputIsOver(cardTile.gameObject))
        return true;
    }
    return false;
  }

  public DeckTrayDeckTileVisual GetCardTileVisual(string cardID)
  {
    foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
    {
      if (!((UnityEngine.Object) cardTile == (UnityEngine.Object) null) && !((UnityEngine.Object) cardTile.GetActor() == (UnityEngine.Object) null) && cardTile.GetActor().GetEntityDef() != null && cardTile.GetActor().GetEntityDef().GetCardId() == cardID)
        return cardTile;
    }
    return (DeckTrayDeckTileVisual) null;
  }

  public DeckTrayDeckTileVisual GetCardTileVisual(int index) => index < this.m_cardTiles.Count ? this.m_cardTiles[index] : (DeckTrayDeckTileVisual) null;

  public DeckTrayDeckTileVisual CreateCardTileVisual(
    string cardTileName,
    Transform parent)
  {
    string name = cardTileName;
    if (string.IsNullOrEmpty(name))
      name = "DeckTileVisual";
    GameObject child = new GameObject(name);
    GameUtils.SetParent(child, (Component) parent);
    child.transform.localScale = this.m_cardTileSlotLocalScaleVec3;
    bool useFullScaleDeckTileActor = !(bool) UniversalInputManager.UsePhoneUI || this.m_forceUseFullScaleDeckTileActors;
    DeckTrayDeckTileVisual cardTileVisual = child.AddComponent<DeckTrayDeckTileVisual>();
    cardTileVisual.Initialize(useFullScaleDeckTileActor);
    return cardTileVisual;
  }

  public DeckTrayDeckTileVisual GetOrAddCardTileVisual(int index)
  {
    DeckTrayDeckTileVisual newTileVisual = this.GetCardTileVisual(index);
    if ((UnityEngine.Object) newTileVisual != (UnityEngine.Object) null)
      return newTileVisual;
    newTileVisual = this.CreateCardTileVisual("DeckTileVisual" + (object) index, this.transform);
    newTileVisual.AddEventListener(UIEventType.DRAG, (UIEvent.Handler) (e => this.FireCardTileDragEvent(newTileVisual)));
    newTileVisual.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.FireCardTilePressEvent(newTileVisual)));
    newTileVisual.AddEventListener(UIEventType.TAP, (UIEvent.Handler) (e => this.FireCardTileTapEvent(newTileVisual)));
    newTileVisual.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.FireCardTileOverEvent(newTileVisual)));
    newTileVisual.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.FireCardTileOutEvent(newTileVisual)));
    newTileVisual.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.FireCardTileReleaseEvent(newTileVisual)));
    newTileVisual.AddEventListener(UIEventType.RIGHTCLICK, (UIEvent.Handler) (e => this.FireCardTileRightClickedEvent(newTileVisual)));
    this.m_cardTiles.Insert(index, newTileVisual);
    Vector3 extents = new Vector3(this.m_cardTileHeight, this.m_cardTileHeight, this.m_cardTileHeight);
    if ((UnityEngine.Object) this.m_scrollbar != (UnityEngine.Object) null)
      this.m_scrollbar.AddVisibleAffectedObject(newTileVisual.gameObject, extents, true, new UIBScrollable.VisibleAffected(this.IsCardTileVisible));
    return newTileVisual;
  }

  public List<string> GetCardIdsMatchingOrAboveRuneCost(
    RuneType runeType,
    int cost,
    List<EntityDef> remainingCards)
  {
    if (remainingCards == null)
      remainingCards = new List<EntityDef>();
    else
      remainingCards.Clear();
    List<string> matchingOrAboveRuneCost = new List<string>();
    foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
    {
      CollectionDeckTileActor actor = cardTile.GetActor();
      if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
      {
        EntityDef entityDef = actor.GetEntityDef();
        if (entityDef != null && cardTile.IsInUse())
        {
          int cost1 = entityDef.GetRuneCost().GetCost(runeType);
          if (cost1 > 0 && cost1 >= cost)
            matchingOrAboveRuneCost.Add(cardTile.GetCardID());
          else
            remainingCards.Add(entityDef);
        }
      }
    }
    return matchingOrAboveRuneCost;
  }

  public void UpdateTileVisuals()
  {
    foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
      cardTile.UpdateGhostedState();
  }

  public override void Show(bool showAll = false)
  {
    foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
    {
      if (showAll || cardTile.IsInUse())
        cardTile.Show();
    }
  }

  public override void Hide(bool hideAll = false)
  {
    foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
    {
      if (hideAll || !cardTile.IsInUse())
        cardTile.Hide();
    }
  }

  public void CommitFakeDeckChanges()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    editedDeck.CopyContents(this.m_templateFakeDeck);
    editedDeck.Name = this.m_templateFakeDeck.Name;
  }

  public CollectionDeck GetEditingDeck()
  {
    if (this.m_isShowingFakeDeck)
    {
      if (CollectionManager.Get() != null)
      {
        CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
        if (editedDeck != null)
          this.m_templateFakeDeck.FormatType = editedDeck.FormatType;
      }
      if (this.m_templateFakeDeck.FormatType == FormatType.FT_UNKNOWN)
        Debug.LogError((object) ("CollectionDeck.GetEditingDeck could not determine the format type for the fake deck " + this.m_templateFakeDeck.ToString()));
    }
    return !this.m_isShowingFakeDeck ? CollectionManager.Get().GetEditedDeck() : this.m_templateFakeDeck;
  }

  public void ShowFakeDeck(bool show)
  {
    if (this.m_isShowingFakeDeck == show)
      return;
    this.m_isShowingFakeDeck = show;
    this.UpdateCardList();
  }

  public void ResetFakeDeck()
  {
    if (this.m_templateFakeDeck == null)
      return;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
      return;
    this.m_templateFakeDeck.CopyContents(editedDeck);
    this.m_templateFakeDeck.Name = editedDeck.Name;
  }

  public void ShowDeckCompleteEffects() => this.StartCoroutine(this.ShowDeckCompleteEffectsWithInterval(this.m_deckCardBarFlareUpInterval));

  public void SetInArena(bool inArena) => this.m_inArena = inArena;

  public bool AddCard(
    EntityDef cardEntityDef,
    TAG_PREMIUM premium,
    bool playSound,
    Actor animateFromActor = null,
    bool updateVisuals = true,
    params DeckRule.RuleType[] ignoreRules)
  {
    if (!this.IsModeActive())
      return false;
    if (cardEntityDef == null)
    {
      Debug.LogError((object) "Trying to add card EntityDef that is null.");
      return false;
    }
    string cardId = cardEntityDef.GetCardId();
    CollectionDeck editingDeck = this.GetEditingDeck();
    if (editingDeck == null)
      return false;
    if (editingDeck.GetTotalCardCount() >= CollectionManager.Get().GetDeckSizeWhileEditing(cardEntityDef))
    {
      GameplayErrorManager.Get().DisplayMessage(GameStrings.Get("GLUE_COLLECTION_MANAGER_ON_ADD_FULL_DECK_ERROR_TEXT"));
      return false;
    }
    if (playSound)
      SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_place_card_in_deck.prefab:df069ffaea9dfb24b96accc95bc434a7", this.gameObject);
    if (!editingDeck.AddCard(cardId, premium, false, ignoreRules))
    {
      Debug.LogWarningFormat("DeckTrayCardListContent.AddCard({0},{1}): deck.AddCard failed!", (object) cardId, (object) premium);
      return false;
    }
    if (updateVisuals)
    {
      this.UpdateCardList(cardEntityDef, animateFromActor: animateFromActor);
      CollectionManager.Get().GetCollectibleDisplay().UpdateCurrentPageCardLocks(true);
    }
    DeckHelper.Get().OnCardAdded(editingDeck);
    if (!Options.Get().GetBool(Option.HAS_ADDED_CARDS_TO_DECK, false) && editingDeck.GetTotalCardCount() >= 2 && !DeckHelper.Get().IsActive() && editingDeck.GetTotalCardCount() < 15 && UserAttentionManager.CanShowAttentionGrabber("DeckTrayCardListContent.AddCard:" + (object) Option.HAS_ADDED_CARDS_TO_DECK))
    {
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_PAGEFLIP_28"), "VO_INNKEEPER_CM_PAGEFLIP_28.prefab:47bb7bdb89ad93443ab7d031bbe666fb");
      Options.Get().SetBool(Option.HAS_ADDED_CARDS_TO_DECK, true);
    }
    return true;
  }

  [ContextMenu("Update Card List")]
  public void UpdateCardList() => this.UpdateCardList(true);

  public void UpdateCardList(
    bool updateHighlight,
    Actor animateFromActor = null,
    Action onCompleteCallback = null)
  {
    this.UpdateCardList(string.Empty, updateHighlight, animateFromActor, onCompleteCallback);
  }

  public void UpdateCardList(
    EntityDef justChangedCardEntityDef,
    bool updateHighlight = true,
    Actor animateFromActor = null,
    Action onCompleteCallback = null)
  {
    this.UpdateCardList(justChangedCardEntityDef != null ? justChangedCardEntityDef.GetCardId() : string.Empty, updateHighlight, animateFromActor, onCompleteCallback);
  }

  public void UpdateCardList(
    string justChangedCardID,
    bool updateHighlight = true,
    Actor animateFromActor = null,
    Action onCompleteCallback = null)
  {
    CollectionDeck editingDeck = this.GetEditingDeck();
    if (editingDeck == null)
      return;
    foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
      cardTile.MarkAsUnused();
    List<CollectionDeckSlot> slots = editingDeck.GetSlots();
    int num = 0;
    Vector3 cardTileOffset = this.GetCardTileOffset(editingDeck);
    bool offsetCardNameForRunes = editingDeck.ContainsDeathKnightRuneCards();
    for (int index = 0; index < slots.Count; ++index)
    {
      CollectionDeckSlot s = slots[index];
      if (s.Count == 0)
      {
        Log.DeckTray.Print(string.Format("DeckTrayCardListContent.UpdateCardList(): Slot {0} of deck is empty! Skipping...", (object) index));
      }
      else
      {
        num += s.Count;
        DeckTrayDeckTileVisual addCardTileVisual = this.GetOrAddCardTileVisual(index);
        addCardTileVisual.SetInArena(this.m_inArena);
        addCardTileVisual.gameObject.transform.localPosition = cardTileOffset + Vector3.down * (this.m_cardTileSlotLocalHeight * (float) index);
        addCardTileVisual.MarkAsUsed();
        addCardTileVisual.Show();
        addCardTileVisual.SetSlot(editingDeck, s, justChangedCardID.Equals(s.CardID), offsetCardNameForRunes);
      }
    }
    this.Hide(false);
    this.ShowDeckHelpButtonIfNeeded();
    this.FireCardCountChangedEvent();
    if ((UnityEngine.Object) this.m_scrollbar != (UnityEngine.Object) null)
      this.m_scrollbar.UpdateScroll();
    if (updateHighlight)
      this.UpdateDeckCompleteHighlight();
    if ((UnityEngine.Object) animateFromActor != (UnityEngine.Object) null && this.gameObject.activeInHierarchy)
    {
      this.StartCoroutine(this.ShowAddCardAnimationAfterTrayLoads(animateFromActor, onCompleteCallback));
    }
    else
    {
      if (onCompleteCallback == null)
        return;
      onCompleteCallback();
    }
  }

  private Vector3 GetCardTileOffset(CollectionDeck currentDeck)
  {
    Vector3 zero = Vector3.zero;
    float num = 0.0f;
    if (currentDeck != null && CollectionManager.Get().IsEditingDeathKnightDeck())
      num += this.RuneIndicatorSpacerHeight;
    if (!this.m_isShowingFakeDeck && currentDeck != null && (UnityEngine.Object) this.m_deckTemplateHelpButton != (UnityEngine.Object) null)
    {
      bool flag = true;
      if (SceneMgr.Get().IsInTavernBrawlMode())
        flag = TavernBrawlDisplay.IsTavernBrawlEditing();
      if (flag && currentDeck.GetTotalInvalidCardCount() > 0)
        num += this.TemplateDeckHelpButtonHeight;
    }
    return Vector3.down * num + this.m_cardTileOffset;
  }

  public void TriggerCardCountUpdate() => this.FireCardCountChangedEvent();

  public void SetRuneIndicatorSpacerVisible(bool visible)
  {
    if (!((UnityEngine.Object) this.m_runeIndicatorSpacer != (UnityEngine.Object) null))
      return;
    this.m_runeIndicatorSpacer.SetActive(visible);
  }

  public void HideDeckHelpPopup()
  {
    if (!((UnityEngine.Object) this.m_deckHelpPopup != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_deckHelpPopup);
  }

  public CollectionDeckSlot FindInvalidSlot() => this.GetEditingDeck()?.FindInvalidSlot();

  private static bool ShouldShowDeckCompleteHighlight(CollectionDeck deck)
  {
    switch (deck.Type)
    {
      case DeckType.CLIENT_ONLY_DECK:
      case DeckType.DRAFT_DECK:
        return false;
      default:
        return true;
    }
  }

  public void UpdateDeckCompleteHighlight()
  {
    CollectionDeck editingDeck = this.GetEditingDeck();
    if (editingDeck == null || !DeckTrayCardListContent.ShouldShowDeckCompleteHighlight(editingDeck))
      return;
    CollectionDeck.CardCountByStatus cardCountByStatus = editingDeck.CountCardsByStatus();
    bool flag = cardCountByStatus.Valid == cardCountByStatus.Max && cardCountByStatus.Extra == 0;
    if ((UnityEngine.Object) this.m_scrollbar != (UnityEngine.Object) null && (UnityEngine.Object) this.m_LockedScrollBounds != (UnityEngine.Object) null && editingDeck.Locked)
    {
      this.m_scrollbar.m_ScrollBounds.center = this.m_LockedScrollBounds.center;
      this.m_scrollbar.m_ScrollBounds.size = this.m_LockedScrollBounds.size;
    }
    if ((UnityEngine.Object) this.m_deckCompleteHighlight != (UnityEngine.Object) null)
    {
      if (editingDeck.Locked)
        this.m_deckCompleteHighlight.SetActive(false);
      else
        this.m_deckCompleteHighlight.SetActive(flag);
    }
    if (!flag || Options.Get().GetBool(Option.HAS_FINISHED_A_DECK, false))
      return;
    Options.Get().SetBool(Option.HAS_FINISHED_A_DECK, true);
  }

  public Notification GetDeckHelpPopup() => this.m_deckHelpPopup;

  private IEnumerator ShowAddCardAnimationAfterTrayLoads(
    Actor cardToAnimate,
    Action onCompleteCallback)
  {
    DeckTrayCardListContent trayCardListContent = this;
    string cardID = cardToAnimate.GetEntityDef().GetCardId();
    DeckTrayDeckTileVisual tile = trayCardListContent.GetCardTileVisual(cardID);
    Vector3 cardPos = cardToAnimate.transform.position;
    for (; (UnityEngine.Object) tile == (UnityEngine.Object) null; tile = trayCardListContent.GetCardTileVisual(cardID))
      yield return (object) null;
    GameObject cardTileObject = UnityEngine.Object.Instantiate<GameObject>(tile.GetActor().gameObject);
    Actor movingCardTile = cardTileObject.GetComponent<Actor>();
    if (trayCardListContent.GetEditingDeck().GetCardCountAllMatchingSlots(cardID) == 1)
      tile.Hide();
    else
      tile.Show();
    movingCardTile.transform.position = new Vector3(cardPos.x, cardPos.y + 2.5f, cardPos.z);
    if ((bool) UniversalInputManager.UsePhoneUI)
      movingCardTile.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
    else
      movingCardTile.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    movingCardTile.ActivateAllSpellsDeathStates();
    movingCardTile.ActivateSpellBirthState(SpellType.SUMMON_IN_LARGE);
    if ((bool) UniversalInputManager.UsePhoneUI && (UnityEngine.Object) trayCardListContent.m_phoneDeckTileBone != (UnityEngine.Object) null)
    {
      iTween.MoveTo(cardTileObject, iTween.Hash((object) "position", (object) trayCardListContent.m_phoneDeckTileBone.transform.position, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) (Action<object>) (v =>
      {
        tile.ShowAndSetupActor();
        tile.GetActor().GetSpell(SpellType.SUMMON_IN).ActivateState(SpellStateType.BIRTH);
        this.StartCoroutine(this.FinishPhoneMovingCardTile(cardTileObject, movingCardTile, 1f));
        if (onCompleteCallback == null)
          return;
        onCompleteCallback();
      })));
      iTween.ScaleTo(cardTileObject, iTween.Hash((object) "scale", (object) new Vector3(0.5f, 1.1f, 1.1f), (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeInCubic));
    }
    else
    {
      Vector3[] newPath = new Vector3[3];
      Vector3 startSpot = movingCardTile.transform.position;
      newPath[0] = startSpot;
      iTween.ValueTo(cardTileObject, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) 0.75f, (object) "easetype", (object) iTween.EaseType.easeOutCirc, (object) "onupdate", (object) (Action<object>) (val =>
      {
        Vector3 position = tile.transform.position;
        newPath[1] = new Vector3((float) (((double) startSpot.x + (double) position.x) * 0.5), (float) (((double) startSpot.y + (double) position.y) * 0.5 + 60.0), (float) (((double) startSpot.z + (double) position.z) * 0.5));
        newPath[2] = position;
        iTween.PutOnPath(cardTileObject, newPath, (float) val);
      }), (object) "oncomplete", (object) (Action<object>) (v =>
      {
        tile.ShowAndSetupActor();
        tile.GetActor().GetSpell(SpellType.SUMMON_IN).ActivateState(SpellStateType.BIRTH);
        movingCardTile.Hide();
        UnityEngine.Object.Destroy((UnityEngine.Object) cardTileObject);
        if (onCompleteCallback == null)
          return;
        onCompleteCallback();
      })));
    }
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_add_to_deck_instant.prefab:06df359c4026d7e47b06a4174f33e3ef", trayCardListContent.gameObject);
  }

  private IEnumerator FinishPhoneMovingCardTile(
    GameObject obj,
    Actor movingCardTile,
    float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    movingCardTile.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) obj);
  }

  private IEnumerator ShowDeckCompleteEffectsWithInterval(float interval)
  {
    if (!((UnityEngine.Object) this.m_scrollbar == (UnityEngine.Object) null))
    {
      bool needScroll = this.m_scrollbar.IsScrollNeeded();
      if (needScroll)
      {
        this.m_scrollbar.Enable(false);
        this.m_scrollbar.ForceVisibleAffectedObjectsShow(true);
        this.m_scrollbar.SetScroll(0.0f, iTween.EaseType.easeOutSine, 0.25f, true);
        yield return (object) new WaitForSeconds(0.3f);
        this.m_scrollbar.SetScroll(1f, iTween.EaseType.easeInOutQuart, interval * (float) this.m_cardTiles.Count, true);
      }
      foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
      {
        if (!((UnityEngine.Object) cardTile == (UnityEngine.Object) null) && cardTile.IsInUse())
        {
          cardTile.GetActor().ActivateSpellBirthState(SpellType.SUMMON_IN_FORGE);
          yield return (object) new WaitForSeconds(interval);
        }
      }
      foreach (DeckTrayDeckTileVisual cardTile in this.m_cardTiles)
      {
        DeckTrayDeckTileVisual tile = cardTile;
        if (!((UnityEngine.Object) tile == (UnityEngine.Object) null) && tile.IsInUse())
        {
          yield return (object) new WaitForSeconds(interval);
          tile.GetActor().DeactivateAllSpells();
          tile = (DeckTrayDeckTileVisual) null;
        }
      }
      if (needScroll)
      {
        this.m_scrollbar.ForceVisibleAffectedObjectsShow(false);
        this.m_scrollbar.EnableIfNeeded();
      }
    }
  }

  private void IsCardTileVisible(GameObject obj, bool visible)
  {
    DeckTrayDeckTileVisual component;
    if (obj.activeSelf == visible || !obj.TryGetComponent<DeckTrayDeckTileVisual>(out component))
      return;
    if (visible && component.IsInUse())
      component.ShowAndSetupActor();
    else
      component.Hide();
  }

  private void ShowDeckEditingTipsIfNeeded()
  {
    if (Options.Get().GetBool(Option.HAS_REMOVED_CARD_FROM_DECK, false) || SceneMgr.Get().IsInTavernBrawlMode() || (UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() == (UnityEngine.Object) null || CollectionManager.Get().GetCollectibleDisplay().GetViewMode() != CollectionUtils.ViewMode.CARDS || this.m_cardTiles.Count <= 0)
      return;
    Transform cardTutorialBone = CollectionDeckTray.Get().m_removeCardTutorialBone;
    if (!((UnityEngine.Object) this.m_deckHelpPopup == (UnityEngine.Object) null))
      return;
    this.m_deckHelpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, cardTutorialBone.position, cardTutorialBone.localScale, GameStrings.Get("GLUE_COLLECTION_TUTORIAL08"));
    if (!((UnityEngine.Object) this.m_deckHelpPopup != (UnityEngine.Object) null))
      return;
    this.m_deckHelpPopup.PulseReminderEveryXSeconds(3f);
  }

  private void ShowDeckHelpButtonIfNeeded()
  {
    bool flag1 = false;
    if ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() == (UnityEngine.Object) null)
      return;
    CollectionDeck editingDeck = this.GetEditingDeck();
    if (editingDeck != null && (UnityEngine.Object) DeckHelper.Get() != (UnityEngine.Object) null && editingDeck.GetTotalValidCardCount() < CollectionManager.Get().GetDeckSize())
      flag1 = true;
    bool flag2;
    if (editingDeck.GetTotalInvalidCardCount() > 0)
    {
      flag1 = false;
      flag2 = true;
    }
    else
      flag2 = false;
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE)
    {
      flag2 = false;
      flag1 = false;
    }
    if (TavernBrawlDisplay.IsTavernBrawlViewing() || SceneMgr.Get().IsInDuelsMode())
    {
      flag2 = false;
      flag1 = false;
    }
    if (!DeckHelper.HasChoicesToOffer(editingDeck))
      flag1 = false;
    if ((UnityEngine.Object) this.m_smartDeckCompleteButton != (UnityEngine.Object) null)
    {
      this.m_smartDeckCompleteButton.gameObject.SetActive(flag1);
      if (flag1)
      {
        Vector3 cardTileOffset = this.GetCardTileOffset(editingDeck);
        cardTileOffset.y -= this.m_cardTileSlotLocalHeight * (float) editingDeck.GetSlots().Count;
        this.m_smartDeckCompleteButton.transform.localPosition = cardTileOffset;
      }
    }
    if ((UnityEngine.Object) this.m_deckTemplateHelpButton != (UnityEngine.Object) null)
      this.m_deckTemplateHelpButton.gameObject.SetActive(flag2);
    if (flag2)
    {
      Vector3 originalLocalPosition = this.m_deckTemplateHelpButtonOriginalLocalPosition;
      if (CollectionManager.Get().IsEditingDeathKnightDeck())
        originalLocalPosition.y = this.m_deckTemplateHelpButtonDeathKnightPosY;
      else
        originalLocalPosition = this.m_deckTemplateHelpButtonOriginalLocalPosition;
      this.m_deckTemplateHelpButton.transform.localPosition = originalLocalPosition;
    }
    if (Options.Get().GetBool(Option.HAS_FINISHED_A_DECK, false))
      return;
    if ((UnityEngine.Object) this.m_smartDeckCompleteButton != (UnityEngine.Object) null)
    {
      HighlightState componentInChildren = this.m_smartDeckCompleteButton.GetComponentInChildren<HighlightState>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
        componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    }
    if (!((UnityEngine.Object) this.m_deckTemplateHelpButton != (UnityEngine.Object) null))
      return;
    HighlightState componentInChildren1 = this.m_deckTemplateHelpButton.GetComponentInChildren<HighlightState>();
    if (!((UnityEngine.Object) componentInChildren1 != (UnityEngine.Object) null))
      return;
    componentInChildren1.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }

  private void OnDeckTemplateHelpButtonPress(UIEvent e)
  {
    Options.Get().SetBool(Option.HAS_CLICKED_DECK_TEMPLATE_REPLACE, true);
    this.ShowDeckHelper(this.FindInvalidSlot(), false);
  }

  private void OnDeckTemplateHelpButtonOver(UIEvent e)
  {
    HighlightState componentInChildren = this.m_deckTemplateHelpButton.GetComponentInChildren<HighlightState>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
    {
      if (!Options.Get().GetBool(Option.HAS_FINISHED_A_DECK, false))
        componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
      else
        componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
    }
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9", this.gameObject);
  }

  private void OnDeckTemplateHelpButtonOut(UIEvent e)
  {
    HighlightState componentInChildren = this.m_deckTemplateHelpButton.GetComponentInChildren<HighlightState>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    if (!Options.Get().GetBool(Option.HAS_CLICKED_DECK_TEMPLATE_REPLACE, false))
      componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    else
      componentInChildren.ChangeState(ActorStateType.NONE);
  }

  private void OnDeckCompleteButtonPress(UIEvent e)
  {
    if (!((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null))
      return;
    CollectionDeckTray.Get().CompleteMyDeckButtonPress();
  }

  private void OnDeckCompleteButtonOver(UIEvent e)
  {
    if (CollectionInputMgr.Get().HasHeldCard())
      return;
    HighlightState componentInChildren = this.m_smartDeckCompleteButton.GetComponentInChildren<HighlightState>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9", this.gameObject);
  }

  private void OnDeckCompleteButtonOut(UIEvent e)
  {
    HighlightState componentInChildren = this.m_smartDeckCompleteButton.GetComponentInChildren<HighlightState>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    componentInChildren.ChangeState(ActorStateType.NONE);
  }

  private void LoadCardPrefabs(List<CollectionDeckSlot> deckSlots)
  {
    if (deckSlots.Count == 0)
      return;
    int prefabsToLoad = deckSlots.Count;
    this.m_loading = true;
    this.m_cardDefs.DisposeValuesAndClear<DefLoader.DisposableCardDef>();
    for (int index = 0; index < deckSlots.Count; ++index)
    {
      CollectionDeckSlot deckSlot = deckSlots[index];
      if (deckSlot.Count == 0)
        Log.DeckTray.Print(string.Format("DeckTrayCardListContent.LoadCardPrefabs(): Slot {0} of deck is empty! Skipping...", (object) index));
      else
        DefLoader.Get().LoadCardDef(deckSlot.CardID, (DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>) ((cardId, def, userData) =>
        {
          this.m_cardDefs.Add(def);
          --prefabsToLoad;
          if (prefabsToLoad != 0)
            return;
          this.m_loading = false;
        }), quality: new CardPortraitQuality(1, TAG_PREMIUM.NORMAL));
    }
  }

  private Vector3 GetOffscreenLocalPosition()
  {
    Vector3 originalLocalPosition = this.m_originalLocalPosition;
    CollectionDeck editingDeck = this.GetEditingDeck();
    int num = editingDeck != null ? editingDeck.GetSlotCount() + 2 : 0;
    originalLocalPosition.z -= (float) ((double) this.m_cardTileHeight * (double) num - (double) this.GetCardTileOffset(editingDeck).y / (double) this.m_cardTileSlotLocalScaleVec3.y);
    return originalLocalPosition;
  }

  public void RegisterCardTileHeldListener(DeckTrayCardListContent.CardTileHeld dlg) => this.m_cardTileHeldListeners.Add(dlg);

  public void RegisterCardTilePressListener(DeckTrayCardListContent.CardTilePress dlg) => this.m_cardTilePressListeners.Add(dlg);

  public void RegisterCardTileTapListener(DeckTrayCardListContent.CardTileTap dlg) => this.m_cardTileTapListeners.Add(dlg);

  public void RegisterCardTileOverListener(DeckTrayCardListContent.CardTileOver dlg) => this.m_cardTileOverListeners.Add(dlg);

  public void RegisterCardTileOutListener(DeckTrayCardListContent.CardTileOut dlg) => this.m_cardTileOutListeners.Add(dlg);

  public void RegisterCardTileReleaseListener(DeckTrayCardListContent.CardTileRelease dlg) => this.m_cardTileReleaseListeners.Add(dlg);

  public void RegisterCardTileRightClickedListener(DeckTrayCardListContent.CardTileRightClicked dlg) => this.m_cardTileRightClickedListeners.Add(dlg);

  public void RegisterCardCountUpdated(DeckTrayCardListContent.CardCountChanged dlg) => this.m_cardCountChangedListeners.Add(dlg);

  public void UnregisterCardTileHeldListener(DeckTrayCardListContent.CardTileHeld dlg) => this.m_cardTileHeldListeners.Remove(dlg);

  public void UnregisterCardTileTapListener(DeckTrayCardListContent.CardTileTap dlg) => this.m_cardTileTapListeners.Remove(dlg);

  public void UnregisterCardTilePressListener(DeckTrayCardListContent.CardTilePress dlg) => this.m_cardTilePressListeners.Remove(dlg);

  public void UnregisterCardTileOverListener(DeckTrayCardListContent.CardTileOver dlg) => this.m_cardTileOverListeners.Remove(dlg);

  public void UnregisterCardTileOutListener(DeckTrayCardListContent.CardTileOut dlg) => this.m_cardTileOutListeners.Remove(dlg);

  public void UnregisterCardTileReleaseListener(DeckTrayCardListContent.CardTileRelease dlg) => this.m_cardTileReleaseListeners.Remove(dlg);

  public void UnregisterCardTileRightClickedListener(
    DeckTrayCardListContent.CardTileRightClicked dlg)
  {
    this.m_cardTileRightClickedListeners.Remove(dlg);
  }

  public void UnregisterCardCountUpdated(DeckTrayCardListContent.CardCountChanged dlg) => this.m_cardCountChangedListeners.Remove(dlg);

  private void FireCardTileDragEvent(DeckTrayDeckTileVisual cardTile)
  {
    foreach (DeckTrayCardListContent.CardTileHeld cardTileHeld in this.m_cardTileHeldListeners.ToArray())
      cardTileHeld(cardTile);
  }

  private void FireCardTilePressEvent(DeckTrayDeckTileVisual cardTile)
  {
    foreach (DeckTrayCardListContent.CardTilePress cardTilePress in this.m_cardTilePressListeners.ToArray())
      cardTilePress(cardTile);
  }

  private void FireCardTileTapEvent(DeckTrayDeckTileVisual cardTile)
  {
    foreach (DeckTrayCardListContent.CardTileTap cardTileTap in this.m_cardTileTapListeners.ToArray())
      cardTileTap(cardTile);
  }

  private void FireCardTileOverEvent(DeckTrayDeckTileVisual cardTile)
  {
    foreach (DeckTrayCardListContent.CardTileOver cardTileOver in this.m_cardTileOverListeners.ToArray())
      cardTileOver(cardTile);
  }

  private void FireCardTileOutEvent(DeckTrayDeckTileVisual cardTile)
  {
    foreach (DeckTrayCardListContent.CardTileOut cardTileOut in this.m_cardTileOutListeners.ToArray())
      cardTileOut(cardTile);
  }

  private void FireCardTileReleaseEvent(DeckTrayDeckTileVisual cardTile)
  {
    foreach (DeckTrayCardListContent.CardTileRelease cardTileRelease in this.m_cardTileReleaseListeners.ToArray())
      cardTileRelease(cardTile);
  }

  private void FireCardTileRightClickedEvent(DeckTrayDeckTileVisual cardTile)
  {
    foreach (DeckTrayCardListContent.CardTileRightClicked tileRightClicked in this.m_cardTileRightClickedListeners.ToArray())
      tileRightClicked(cardTile);
  }

  private void FireCardCountChangedEvent()
  {
    CollectionDeck editingDeck = this.GetEditingDeck();
    int cardCount = editingDeck != null ? editingDeck.GetTotalCardCount() : 0;
    foreach (DeckTrayCardListContent.CardCountChanged cardCountChanged in this.m_cardCountChangedListeners.ToArray())
      cardCountChanged(cardCount);
  }

  public delegate void CardTileHeld(DeckTrayDeckTileVisual cardTile);

  public delegate void CardTilePress(DeckTrayDeckTileVisual cardTile);

  public delegate void CardTileTap(DeckTrayDeckTileVisual cardTile);

  public delegate void CardTileOver(DeckTrayDeckTileVisual cardTile);

  public delegate void CardTileOut(DeckTrayDeckTileVisual cardTile);

  public delegate void CardTileRelease(DeckTrayDeckTileVisual cardTile);

  public delegate void CardTileRightClicked(DeckTrayDeckTileVisual cardTile);

  public delegate void CardCountChanged(int cardCount);
}
