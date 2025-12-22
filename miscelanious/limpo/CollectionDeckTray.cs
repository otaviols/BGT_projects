using Cysharp.Threading.Tasks;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionDeckTray : EditableDeckTray
{
  public DeckTrayDeckListContent m_decksContent;
  [SerializeField]
  private WidgetInstance m_cardBackWidget;
  [SerializeField]
  private WidgetInstance m_heroSkinWidget;
  [SerializeField]
  private GameObject m_coinContent;
  [SerializeField]
  private DeckTrayTeamListContent m_teamsContent;
  [SerializeField]
  private DeckTrayMercListContent m_mercContent;
  [SerializeField]
  private RuneIndicatorVisual m_runeIndicatorVisual;
  [SerializeField]
  private List<CollectionDeckTray.CollectionCardEventHandlerData> m_cardEventHandlers;
  [SerializeField]
  private List<CollectionDeckTray.CollectionTagEventHandlerData> m_tagCardEventHandlers;
  public GameObject TrayContentsContainer;
  public GameObject TrayContentsDuelsBone;
  public Transform m_removeCardTutorialBone;
  public PlayMakerFSM m_deckTemplateChosenGlow;
  private static CollectionDeckTray s_instance;
  private DeckTrayCardBackContent m_cardBackContent;
  private DeckTrayHeroSkinContent m_heroSkinContent;
  private CollectionCardEventHandler m_defaultCardEventHandler;

  public static event Action<CollectionDeck, RunePattern> DeckTrayCardAdded;

  private void Awake()
  {
    CollectionDeckTray.s_instance = this;
    if ((UnityEngine.Object) this.gameObject.GetComponent<AudioSource>() == (UnityEngine.Object) null)
      this.gameObject.AddComponent<AudioSource>();
    if ((UnityEngine.Object) this.m_scrollbar != (UnityEngine.Object) null)
    {
      if (SceneMgr.Get().IsInTavernBrawlMode() && !(bool) UniversalInputManager.UsePhoneUI)
      {
        this.m_scrollbar.m_ScrollBounds.center = this.m_scrollbar.m_ScrollBounds.center with
        {
          z = 3f
        };
        this.m_scrollbar.m_ScrollBounds.size = this.m_scrollbar.m_ScrollBounds.size with
        {
          z = 47.67f
        };
        if ((UnityEngine.Object) this.m_cardsContent != (UnityEngine.Object) null && (UnityEngine.Object) this.m_cardsContent.m_deckCompleteHighlight != (UnityEngine.Object) null)
          this.m_cardsContent.m_deckCompleteHighlight.transform.localPosition = this.m_cardsContent.m_deckCompleteHighlight.transform.localPosition with
          {
            z = -34.15f
          };
      }
      this.m_scrollbar.Enable(false);
      this.m_scrollbar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(((DeckTray) this).OnTouchScrollStarted));
      this.m_scrollbar.AddTouchScrollEndedListener(new UIBScrollable.OnTouchScrollEnded(((DeckTray) this).OnTouchScrollEnded));
    }
    if ((UnityEngine.Object) this.m_decksContent != (UnityEngine.Object) null)
    {
      this.m_contents[DeckTray.DeckContentTypes.Decks] = (DeckTrayContent) this.m_decksContent;
      this.m_decksContent.RegisterBusyWithDeck(new DeckTrayDeckListContent.BusyWithDeck(((DeckTray) this).OnBusyWithDeck));
      if (!SceneMgr.Get().IsInTavernBrawlMode())
        this.m_decksContent.RegisterDeckCountUpdated(new DeckTrayDeckListContent.DeckCountChanged(this.OnDeckCountUpdated));
    }
    if ((UnityEngine.Object) this.m_heroSkinWidget != (UnityEngine.Object) null)
      this.m_heroSkinWidget.RegisterReadyListener((Action<object>) (_ =>
      {
        this.m_heroSkinContent = this.m_heroSkinWidget.gameObject.GetComponentInChildren<DeckTrayHeroSkinContent>();
        if (!((UnityEngine.Object) this.m_heroSkinContent != (UnityEngine.Object) null))
          return;
        this.m_contents[DeckTray.DeckContentTypes.HeroSkin] = (DeckTrayContent) this.m_heroSkinContent;
        this.m_heroSkinContent.OnHeroChanged += new Action<string>(this.OnHeroAssigned);
      }), (object) null, true);
    if ((UnityEngine.Object) this.m_cardBackWidget != (UnityEngine.Object) null)
      this.m_cardBackWidget.RegisterReadyListener((Action<object>) (_ =>
      {
        this.m_cardBackContent = this.m_cardBackWidget.gameObject.GetComponentInChildren<DeckTrayCardBackContent>();
        if (!((UnityEngine.Object) this.m_cardBackContent != (UnityEngine.Object) null))
          return;
        this.m_contents[DeckTray.DeckContentTypes.CardBack] = (DeckTrayContent) this.m_cardBackContent;
      }), (object) null, true);
    if ((UnityEngine.Object) this.m_cardsContent != (UnityEngine.Object) null)
    {
      this.m_contents[DeckTray.DeckContentTypes.Cards] = (DeckTrayContent) this.m_cardsContent;
      this.m_cardsContent.RegisterCardTileHeldListener(new DeckTrayCardListContent.CardTileHeld(this.OnCardTileHeld));
      this.m_cardsContent.RegisterCardTilePressListener(new DeckTrayCardListContent.CardTilePress(((DeckTray) this).OnCardTilePress));
      this.m_cardsContent.RegisterCardTileTapListener(new DeckTrayCardListContent.CardTileTap(this.OnCardTileTap));
      this.m_cardsContent.RegisterCardTileOverListener(new DeckTrayCardListContent.CardTileOver(((DeckTray) this).OnCardTileOver));
      this.m_cardsContent.RegisterCardTileOutListener(new DeckTrayCardListContent.CardTileOut(((DeckTray) this).OnCardTileOut));
      this.m_cardsContent.RegisterCardTileReleaseListener(new DeckTrayCardListContent.CardTileRelease(((DeckTray) this).OnCardTileRelease));
      this.m_cardsContent.RegisterCardCountUpdated(new DeckTrayCardListContent.CardCountChanged(this.OnCardCountUpdated));
    }
    if ((UnityEngine.Object) this.m_teamsContent != (UnityEngine.Object) null)
    {
      this.m_contents[DeckTray.DeckContentTypes.Teams] = (DeckTrayContent) this.m_teamsContent;
      this.m_teamsContent.RegisterBusyWithTeam(new DeckTrayTeamListContent.BusyWithTeam(((DeckTray) this).OnBusyWithDeck));
      this.m_teamsContent.RegisterTeamCountUpdated(new DeckTrayTeamListContent.TeamCountChanged(this.OnTeamCountUpdated));
    }
    if ((UnityEngine.Object) this.m_mercContent != (UnityEngine.Object) null)
    {
      this.m_contents[DeckTray.DeckContentTypes.Mercs] = (DeckTrayContent) this.m_mercContent;
      this.m_mercContent.RegisterMercCountUpdated(new DeckTrayMercListContent.MercCountChanged(this.OnMercCountUpdated));
    }
    string key = "GLUE_COLLECTION_MY_DECKS";
    if (SceneMgr.Get().IsInDuelsMode())
      key = "GLUE_PVPDR_DECK_TRAY_HEADER";
    else if (SceneMgr.Get().IsInTavernBrawlMode())
      key = TavernBrawlManager.Get().CurrentSeasonBrawlMode == TavernBrawlMode.TB_MODE_HEROIC ? "GLUE_HEROIC_BRAWL_DECK" : "GLUE_COLLECTION_DECK";
    else if (SceneMgr.Get().IsInLettuceMode())
      key = "GLUE_COLLECTION_MY_TEAMS";
    this.SetMyDecksLabelText(GameStrings.Get(key));
    this.m_doneButton.SetText(GameStrings.Get("GLOBAL_BACK"));
    this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.DoneButtonPress));
    if (SceneMgr.Get().IsInLettuceMode())
      CollectionManager.Get().RegisterEditingTeamChanged(new CollectionManager.OnEditingTeamChanged(((DeckTray) this).OnEditingTeamChanged));
    else
      CollectionManager.Get().RegisterEditedDeckChanged(new CollectionManager.OnEditedDeckChanged(((DeckTray) this).OnEditedDeckChanged));
    SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    CollectionInputMgr.Get().SetScrollbar(this.m_scrollbar);
    foreach (DeckTray.DeckContentScroll scrollable in this.m_scrollables)
      scrollable.SaveStartPosition();
    this.m_defaultCardEventHandler = this.gameObject.AddComponent<CollectionCardEventHandler>();
    if (!SceneMgr.Get().IsInDuelsMode() || !((UnityEngine.Object) this.TrayContentsContainer != (UnityEngine.Object) null) || !((UnityEngine.Object) this.TrayContentsDuelsBone != (UnityEngine.Object) null))
      return;
    this.TrayContentsContainer.transform.localPosition = this.TrayContentsDuelsBone.transform.localPosition;
  }

  protected override void Start()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
    {
      Log.CollectionManager.PrintError("CollectionDeckTray.Start - CollectionManager is null!");
    }
    else
    {
      CollectibleDisplay collectibleDisplay = collectionManager.GetCollectibleDisplay();
      if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
      {
        Log.CollectionManager.PrintError("CollectionDeckTray.Start - CollectibleDisplay is null!");
      }
      else
      {
        collectibleDisplay.UpdateCurrentPageCardLocks(true);
        collectibleDisplay.OnViewModeChanged += new CollectibleDisplay.ViewModeChangedListener(this.OnCMViewModeChanged);
        if (SceneMgr.Get().GetMode() != SceneMgr.Mode.FIRESIDE_GATHERING && !SceneMgr.Get().IsInDuelsMode())
          Navigation.Push(new Navigation.NavigateBackHandler(this.OnBackOutOfCollectionScreen));
        collectionManager.RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
        collectionManager.RegisterTeamCreatedListener(new CollectionManager.DelOnTeamCreated(this.OnTeamCreated));
        base.Start();
      }
    }
  }

  private void OnDestroy()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager != null && SceneMgr.Get() != null)
    {
      if (SceneMgr.Get().IsInLettuceMode())
        collectionManager.RemoveEditingTeamChanged(new CollectionManager.OnEditingTeamChanged(((DeckTray) this).OnEditingTeamChanged));
      else
        collectionManager.RemoveEditedDeckChanged(new CollectionManager.OnEditedDeckChanged(((DeckTray) this).OnEditedDeckChanged));
      collectionManager.DoneEditing();
    }
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    CollectionDeckTray.s_instance = (CollectionDeckTray) null;
  }

  private void OnEnable() => RuneIndicatorVisual.RunePatternChanged += new Action<RunePattern>(this.RuneIndicatorVisualOnRunePatternChanged);

  private void OnDisable() => RuneIndicatorVisual.RunePatternChanged -= new Action<RunePattern>(this.RuneIndicatorVisualOnRunePatternChanged);

  private void RuneIndicatorVisualOnRunePatternChanged(RunePattern runes)
  {
    this.m_cardsContent.UpdateTileVisuals();
    if (!SceneMgr.Get().IsInDuelsMode())
      return;
    AdventureDungeonCrawlDisplay.Get().SyncDeckList();
    this.UpdateDoneButtonText();
  }

  protected override void OnEditedDeckChanged(
    CollectionDeck newDeck,
    CollectionDeck oldDeck,
    object callbackData)
  {
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    CollectionUtils.ViewMode viewMode = collectibleDisplay.GetViewMode();
    CollectionPageManager pageManager = collectibleDisplay.GetPageManager() as CollectionPageManager;
    bool flag = newDeck != null;
    if (viewMode == CollectionUtils.ViewMode.HERO_SKINS && !flag && !pageManager.IsSearching())
      collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.HERO_PICKER, false);
    else if (viewMode == CollectionUtils.ViewMode.HERO_PICKER & flag)
      collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.HERO_SKINS, false);
    base.OnEditedDeckChanged(newDeck, oldDeck, callbackData);
  }

  public bool CanPickupCard()
  {
    DeckTray.DeckContentTypes currentContentType = this.GetCurrentContentType();
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    if ((currentContentType == DeckTray.DeckContentTypes.Cards || currentContentType == DeckTray.DeckContentTypes.Mercs) && viewMode == CollectionUtils.ViewMode.CARDS || currentContentType == DeckTray.DeckContentTypes.CardBack && viewMode == CollectionUtils.ViewMode.CARD_BACKS || currentContentType == DeckTray.DeckContentTypes.HeroSkin && viewMode == CollectionUtils.ViewMode.HERO_SKINS)
      return true;
    return currentContentType == DeckTray.DeckContentTypes.Coin && viewMode == CollectionUtils.ViewMode.COINS;
  }

  public static CollectionDeckTray Get() => CollectionDeckTray.s_instance;

  public void Unload()
  {
    CollectionManager.Get().RemoveDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
    CollectionManager.Get().RemoveTeamCreatedListener(new CollectionManager.DelOnTeamCreated(this.OnTeamCreated));
    CollectionInputMgr.Get().SetScrollbar((UIBScrollable) null);
  }

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    if (UniversalInputManager.Get() != null && UniversalInputManager.Get().IsTextInputActive())
      UniversalInputManager.Get().CancelTextInput(this.gameObject, true);
    if (CollectionManager.Get().IsInEditMode())
      CollectionManager.Get().GetEditedDeck()?.SendChanges(CollectionDeck.ChangeSource.OnScenePreUnload);
    else if (CollectionManager.Get().IsInEditTeamMode())
      CollectionManager.Get().GetEditingTeam()?.SendChanges();
    this.Exit();
  }

  public bool AddCard(
    EntityDef cardEntityDef,
    TAG_PREMIUM premium,
    bool playSound,
    Actor animateActor = null,
    params DeckRule.RuleType[] ignoreRules)
  {
    return SceneMgr.Get().IsInLettuceMode() ? this.AddCardToTeam(cardEntityDef, playSound) : this.AddCardToDeck(cardEntityDef, premium, playSound, animateActor, false, ignoreRules);
  }

  public bool AddCardToDeck(
    EntityDef cardEntityDef,
    TAG_PREMIUM premium,
    bool playSound,
    Actor animateActor = null,
    bool allowInvalid = false,
    params DeckRule.RuleType[] ignoreRules)
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    RunePattern runes = editedDeck.Runes;
    CollectionCardEventHandler cardEventHandler = this.GetCardEventHandler(cardEntityDef.GetCardId());
    bool updateVisuals = cardEventHandler.ShouldUpdateVisuals();
    int num = this.GetCardsContent().AddCard(cardEntityDef, premium, playSound, animateActor, updateVisuals, ignoreRules) ? 1 : 0;
    if (num != 0)
    {
      cardEventHandler.OnCardAdded(this, editedDeck, cardEntityDef, premium, animateActor);
      Action<CollectionDeck, RunePattern> deckTrayCardAdded = CollectionDeckTray.DeckTrayCardAdded;
      if (deckTrayCardAdded != null)
        deckTrayCardAdded(editedDeck, cardEntityDef.GetRuneCost());
    }
    if (SceneMgr.Get().IsInDuelsMode())
    {
      AdventureDungeonCrawlDisplay.Get().SyncDeckList();
      this.UpdateDoneButtonText();
    }
    if (editedDeck.CreatedFromShareableDeck != null)
      return num != 0;
    if (editedDeck.IsCreatedWithDeckComplete)
      return num != 0;
    this.ShowExtraCardsPopupIfNeeded(editedDeck, (EntityBase) cardEntityDef);
    CollectionDeckTray.ShowExtraRunesPopupIfNeeded(editedDeck, (EntityBase) cardEntityDef);
    return num != 0;
  }

  private void ShowExtraCardsPopupIfNeeded(CollectionDeck deck, EntityBase cardBeingAdded)
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if ((netObject != null ? (netObject.OvercappedDecksEnabled ? 1 : 0) : 0) == 0 || CollectionManager.Get().HasSeenOvercappedDeckInfoPopup)
      return;
    int num = !cardBeingAdded.HasTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE) ? CollectionManager.Get().GetDeckSize() : cardBeingAdded.GetTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE);
    if (num < 30 || deck.GetTotalCardCount() != num + 1)
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_EXTRA_CARD_WARNING_HEADER"),
      m_showAlertIcon = true,
      m_text = GameStrings.Format("GLUE_COLLECTION_DECK_EXTRA_CARD_WARNING_BODY", (object) num),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM,
      m_confirmText = GameStrings.Get("GLUE_COLLECTION_DECK_EXTRA_CARD_WARNING_CONFIRM"),
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => CollectionManager.Get().HasSeenOvercappedDeckInfoPopup = true)
    });
  }

  private static void ShowExtraRunesPopupIfNeeded(CollectionDeck deck, EntityBase cardBeingAdded)
  {
    if (CollectionManager.Get().HasSeenExtraRunesDeckInfoPopup || deck.CanAddRunes(cardBeingAdded.GetRuneCost(), DeckRule_DeathKnightRuneLimit.MaxRuneSlots))
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_EXTRA_RUNES_WARNING_HEADER"),
      m_showAlertIcon = true,
      m_text = GameStrings.Format("GLUE_COLLECTION_DECK_EXTRA_RUNES_WARNING_BODY", (object) DeckRule_DeathKnightRuneLimit.MaxRuneSlots),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM,
      m_confirmText = GameStrings.Get("GLUE_COLLECTION_DECK_EXTRA_RUNES_WARNING_CONFIRM"),
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => CollectionManager.Get().HasSeenExtraRunesDeckInfoPopup = true)
    };
    DialogManager.Get().ShowPopup(info);
  }

  public bool AddCardToTeam(EntityDef cardEntityDef, bool playSound, int index = -1) => this.GetMercsContent().AddMerc(cardEntityDef, playSound, index: index);

  private bool AddCardWithPreferredPremium(EntityDef cardEntityDef, bool playSound)
  {
    TAG_PREMIUM? nullable = new TAG_PREMIUM?((TAG_PREMIUM) ((int) CollectionManager.Get().GetEditedDeck().GetPreferredPremiumThatCanBeAdded(cardEntityDef.GetCardId()) ?? 0));
    return this.AddCard(cardEntityDef, nullable.Value, playSound, (Actor) null);
  }

  public void OnCardManuallyAddedByUser_CheckSuggestions(EntityDef cardEntityDef) => this.OnCardManuallyAddedByUser_CheckSuggestions((IEnumerable<EntityDef>) new EntityDef[1]
  {
    cardEntityDef
  });

  public void OnCardManuallyAddedByUser_CheckSuggestions(IEnumerable<EntityDef> cardEntityDefs)
  {
    EntityDef entityDef = cardEntityDefs.FirstOrDefault<EntityDef>((Func<EntityDef, bool>) (def => def.IsCollectionManagerFilterManaCostByEven || def.IsCollectionManagerFilterManaCostByOdd));
    if (entityDef == null)
      return;
    CollectionManagerDisplay cmDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    bool flag1 = entityDef.IsCollectionManagerFilterManaCostByEven && (UnityEngine.Object) cmDisplay != (UnityEngine.Object) null && !cmDisplay.IsManaFilterEvenValues;
    bool flag2 = entityDef.IsCollectionManagerFilterManaCostByOdd && (UnityEngine.Object) cmDisplay != (UnityEngine.Object) null && !cmDisplay.IsManaFilterOddValues;
    if (!(flag1 | flag2))
      return;
    string key = flag1 ? "GLUE_COLLECTION_MANAGER_MANA_FILTER_PROMPT_BODY_EVEN_CARDS" : "GLUE_COLLECTION_MANAGER_MANA_FILTER_PROMPT_BODY_ODD_CARDS";
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_MANAGER_MANA_FILTER_PROMPT_HEADER"),
      m_text = GameStrings.Get(key),
      m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
      m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response != AlertPopup.Response.CONFIRM || (UnityEngine.Object) cmDisplay == (UnityEngine.Object) null)
          return;
        cmDisplay.FilterBySearchText(CollectibleCardFilter.CreateSearchTerm_Mana_OddEven((bool) userData));
      }),
      m_responseUserData = (object) flag2
    };
    DialogManager.Get().ShowPopup(info);
  }

  public bool AnimateInCardBack(Actor actor)
  {
    CollectionCardBack component = actor.gameObject.GetComponent<CollectionCardBack>();
    return !((UnityEngine.Object) component == (UnityEngine.Object) null) && this.GetCardBackContent().AnimateInCardBack(component.GetCardBackId(), actor.gameObject);
  }

  public void FlashDeckTemplateHighlight()
  {
    if (!((UnityEngine.Object) this.m_deckTemplateChosenGlow != (UnityEngine.Object) null))
      return;
    this.m_deckTemplateChosenGlow.SendEvent("Flash");
  }

  public void SetHeroSkin(Actor actor) => this.GetHeroSkinContent().AnimateInHeroSkin(actor);

  public void HandleAddedCardDeckUpdate(EntityDef entityDef, TAG_PREMIUM premium, int newCount)
  {
    if (!this.IsShowingDeckContents())
      return;
    CollectionDeck editingDeck = this.GetCardsContent().GetEditingDeck();
    if (editingDeck == null)
    {
      Debug.LogWarning((object) "null editing deck returned during HandleAddedCardDeckUpdate");
    }
    else
    {
      CollectionDeckSlot ownedSlotByCardId = editingDeck.FindFirstOwnedSlotByCardId(entityDef.GetCardId(), false);
      for (int index = 0; ownedSlotByCardId != null && index < newCount; ++index)
      {
        this.AddCard(entityDef, premium, true, (Actor) null);
        ownedSlotByCardId = editingDeck.FindFirstOwnedSlotByCardId(entityDef.GetCardId(), false);
      }
    }
  }

  public bool HandleDeletedCardDeckUpdate(string cardID)
  {
    if (!this.IsShowingDeckContents())
      return false;
    CollectionDeck editingDeck = this.GetCardsContent().GetEditingDeck();
    this.GetCardEventHandler(cardID).OnCardRemoved(this, editingDeck);
    this.GetCardsContent().UpdateCardList(cardID);
    CollectionManager.Get().GetCollectibleDisplay().UpdateCurrentPageCardLocks(true);
    return true;
  }

  public bool RemoveCard(
    string cardID,
    TAG_PREMIUM premium,
    bool valid,
    bool enforceRemainingDeckRuleset = false)
  {
    bool flag = false;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck != null)
    {
      flag = editedDeck.RemoveCard(cardID, premium, valid, enforceRemainingDeckRuleset);
      if (flag)
        this.HandleDeletedCardDeckUpdate(cardID);
    }
    if (SceneMgr.Get().IsInDuelsMode())
    {
      AdventureDungeonCrawlDisplay.Get().SyncDeckList();
      this.UpdateDoneButtonText();
    }
    return flag;
  }

  public bool RemoveAllCopiesOfCard(string cardID)
  {
    bool flag = false;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    for (int index = editedDeck.GetSlots().Count - 1; index >= 0; --index)
    {
      CollectionDeckSlot slot = editedDeck.GetSlots()[index];
      if (!(slot.CardID != cardID))
      {
        while (slot.GetCount(TAG_PREMIUM.NORMAL) > 0)
          flag = flag | this.RemoveCard(slot.CardID, TAG_PREMIUM.NORMAL, true) | this.RemoveCard(slot.CardID, TAG_PREMIUM.NORMAL, false);
        while (slot.GetCount(TAG_PREMIUM.GOLDEN) > 0)
          flag = flag | this.RemoveCard(slot.CardID, TAG_PREMIUM.GOLDEN, true) | this.RemoveCard(slot.CardID, TAG_PREMIUM.GOLDEN, false);
        while (slot.GetCount(TAG_PREMIUM.SIGNATURE) > 0)
          flag = flag | this.RemoveCard(slot.CardID, TAG_PREMIUM.SIGNATURE, true) | this.RemoveCard(slot.CardID, TAG_PREMIUM.SIGNATURE, false);
        while (slot.GetCount(TAG_PREMIUM.DIAMOND) > 0)
          flag = flag | this.RemoveCard(slot.CardID, TAG_PREMIUM.DIAMOND, true) | this.RemoveCard(slot.CardID, TAG_PREMIUM.DIAMOND, false);
      }
    }
    return flag;
  }

  public void ShowDeck(CollectionUtils.ViewMode viewMode)
  {
    Log.CollectionManager.Print("mode={0}", (object) viewMode);
    this.SetTrayMode(this.GetContentTypeFromViewMode(viewMode));
    if (!CollectionManagerDisplay.IsSpecialOneDeckMode())
      Navigation.PushUnique(new Navigation.NavigateBackHandler(((DeckTray) this).OnBackOutOfContainerContents));
    if (CollectionManager.Get().ShouldShowWildToStandardTutorial(false) && CollectionManager.Get().GetEditedDeck().FormatType == FormatType.FT_WILD)
    {
      CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
      if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.ViewModeHasVisibleDeckList())
        collectibleDisplay.ShowConvertTutorial(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    this.UpdateEditedDeckBoxColliderHeightForDeathKnight();
  }

  public void ShowTeam(CollectionUtils.ViewMode viewMode)
  {
    Log.CollectionManager.Print("mode={0}", (object) viewMode);
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && viewMode != CollectionUtils.ViewMode.CARDS)
    {
      viewMode = CollectionUtils.ViewMode.CARDS;
      collectibleDisplay.SetViewMode(viewMode);
    }
    this.SetTrayMode(DeckTray.DeckContentTypes.Mercs);
    Navigation.PushUnique(new Navigation.NavigateBackHandler(((DeckTray) this).OnBackOutOfContainerContents));
  }

  public void EnterEditDeckModeForTavernBrawl(CollectionDeck deck, bool isNewDeck)
  {
    Navigation.Push(new Navigation.NavigateBackHandler(((DeckTray) this).OnBackOutOfContainerContents));
    this.UpdateDoneButtonText();
    this.UpdateRuneIndicatorVisual(deck);
    CollectionDeckBoxVisual editingDeckBox = this.GetEditingDeckBox();
    if ((UnityEngine.Object) editingDeckBox != (UnityEngine.Object) null)
      editingDeckBox.UpdateRuneSlotVisual(deck);
    this.m_runeIndicatorVisual.EnableRuneButtons();
    this.m_cardsContent.UpdateCardList();
    this.CheckNumCardsNeededToBuildDeck(deck);
    CollectionManager.Get().StartEditingDeck(deck, (object) isNewDeck);
  }

  public void ExitEditDeckModeForTavernBrawl() => this.UpdateDoneButtonText();

  public void EnterDeckEditForPVPDR(CollectionDeck deck)
  {
    CollectionManager.Get().SetEditedDeck(deck);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    collectibleDisplay.ShowDuelsDeckHeader();
    collectibleDisplay.ShowCurrentEditedDeck();
    this.UpdateDoneButtonText();
    this.UpdateRuneIndicatorVisual(deck);
    CollectionDeckBoxVisual editingDeckBox = this.GetEditingDeckBox();
    if (!((UnityEngine.Object) editingDeckBox != (UnityEngine.Object) null))
      return;
    editingDeckBox.UpdateRuneSlotVisual(deck);
  }

  private void CheckNumCardsNeededToBuildDeck(CollectionDeck deck)
  {
    int reachMinimumDeckSize = this.CalculateNumCardsNeededToCraftToReachMinimumDeckSize(deck);
    if (reachMinimumDeckSize <= 0)
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_INVALID_POPUP_HEADER"),
      m_text = GameStrings.Format("GLUE_COLLECTION_DECK_RULE_NOT_ENOUGH_CARDS", (object) reachMinimumDeckSize),
      m_okText = GameStrings.Get("GLOBAL_OKAY"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  public bool IsWaitingToDeleteDeck() => (bool) (UnityEngine.Object) this.m_decksContent && this.m_decksContent.IsWaitingToDeleteDeck();

  public void DeleteEditingDeck(bool popNavigation = true)
  {
    if (popNavigation)
      Navigation.Pop();
    this.m_decksContent.DeleteEditingDeck();
    this.SetTrayMode(DeckTray.DeckContentTypes.Decks);
  }

  public void CancelRenamingDeck() => this.m_decksContent.CancelRenameEditingDeck();

  public void SetMyDecksLabelText(string text) => this.m_myDecksLabel.Text = text;

  public DeckTrayDeckListContent GetDecksContent() => this.m_decksContent;

  public DeckTrayCardBackContent GetCardBackContent() => this.m_cardBackContent;

  public DeckTrayHeroSkinContent GetHeroSkinContent() => this.m_heroSkinContent;

  public DeckTrayTeamListContent GetTeamsContent() => this.m_teamsContent;

  public DeckTrayMercListContent GetMercsContent() => this.m_mercContent;

  public DeckTrayReorderableContent GetReorderableContent()
  {
    if ((bool) (UnityEngine.Object) this.m_decksContent)
      return (DeckTrayReorderableContent) this.m_decksContent;
    return (bool) (UnityEngine.Object) this.m_teamsContent ? (DeckTrayReorderableContent) this.m_teamsContent : (DeckTrayReorderableContent) null;
  }

  public void Exit()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    this.HideUnseenDeckTrays();
  }

  public CollectionDeckBoxVisual GetEditingDeckBox()
  {
    TraySection editingTraySection = this.GetDecksContent().GetEditingTraySection();
    return (UnityEngine.Object) editingTraySection == (UnityEngine.Object) null ? (CollectionDeckBoxVisual) null : editingTraySection.m_deckBox;
  }

  public void InitializeRuneIndicatorVisual(CollectionDeck deck)
  {
    if (deck == null || (UnityEngine.Object) this.m_runeIndicatorVisual == (UnityEngine.Object) null)
      return;
    this.m_runeIndicatorVisual.Initialize(deck, this);
  }

  public void DisableRuneIndicatorVisualButtons() => this.m_runeIndicatorVisual.DisableRuneButtons();

  public void EnableRuneIndicatorVisualButtons() => this.m_runeIndicatorVisual.EnableRuneButtons();

  private void DoneButtonPress(UIEvent e)
  {
    if (!SceneMgr.Get().IsInDuelsMode())
    {
      if ((UnityEngine.Object) this.m_runeIndicatorVisual != (UnityEngine.Object) null)
      {
        if (TavernBrawlDisplay.IsTavernBrawlOpen())
        {
          this.m_runeIndicatorVisual.DisableRuneButtons();
        }
        else
        {
          this.m_runeIndicatorVisual.Hide();
          this.m_cardsContent.SetRuneIndicatorSpacerVisible(false);
          this.m_runeIndicatorVisual.EnableRuneButtons();
        }
      }
      if (!(bool) UniversalInputManager.UsePhoneUI && CollectionManager.Get().IsEditingDeathKnightDeck())
      {
        CollectionDeckBoxVisual editingDeckBox = this.GetEditingDeckBox();
        if ((bool) (UnityEngine.Object) editingDeckBox)
          editingDeckBox.ResetColliderHeight();
      }
    }
    if ((UnityEngine.Object) this.m_cardBackContent != (UnityEngine.Object) null && this.m_cardBackContent.WaitingForCardbackAnimation)
      this.StartCoroutine(this.CompleteDoneButtonPressAfterAnimations(e));
    else
      Navigation.GoBack();
  }

  private IEnumerator CompleteDoneButtonPressAfterAnimations(UIEvent e)
  {
    while (this.m_cardBackContent.WaitingForCardbackAnimation)
      yield return (object) null;
    this.DoneButtonPress(e);
  }

  public override bool OnBackOutOfContainerContents()
  {
    if (SceneMgr.Get().IsInDuelsMode())
      return this.OnBackOutOfDeckContentsDuel();
    return SceneMgr.Get().IsInLettuceMode() ? this.OnBackOutOfMercenariesContents() : this.OnBackOutOfDeckContentsImpl(false);
  }

  public bool OnBackOutOfDeckContentsImpl(bool deleteDeck)
  {
    if (this.GetCurrentContentType() != DeckTray.DeckContentTypes.INVALID && (UnityEngine.Object) this.GetCurrentContent() != (UnityEngine.Object) null && !this.GetCurrentContent().IsModeActive() || !this.IsShowingDeckContents())
      return false;
    Log.DeckTray.Print("backing out of deck contents " + deleteDeck.ToString());
    DeckHelper.Get().Hide();
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.HideConvertTutorial();
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (deleteDeck)
      this.m_decksContent.DeleteDeck(editedDeck.ID);
    DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
    (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as CollectionPageManager).HideNonDeckTemplateTabs(false);
    bool flag = true;
    if (deckRuleset != null)
      flag = deckRuleset.IsDeckValid(editedDeck);
    if (editedDeck.FormatType == FormatType.FT_STANDARD & flag && CollectionManager.Get().ShouldShowWildToStandardTutorial(false) && UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, "CollectionDeckTray.OnBackOutOfDeckContentsImpl:ShowSetRotationTutorial"))
    {
      Options.Get().SetBool(Option.NEEDS_TO_MAKE_STANDARD_DECK, false);
      Options.Get().SetLong(Option.LAST_CUSTOM_DECK_CHOSEN, editedDeck.ID);
      Vector3 position = OverlayUI.Get().GetRelativePosition(this.m_doneButton.transform.position) + ((bool) UniversalInputManager.UsePhoneUI ? new Vector3(-56.5f, 0.0f, 35f) : new Vector3(-30.8f, 0.0f, 17.8f));
      Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, position, NotificationManager.NOTIFICATITON_WORLD_SCALE, GameStrings.Get("GLUE_COLLECTION_TUTORIAL16"), false);
      popupText.ShowPopUpArrow(Notification.PopUpArrowDirection.RightDown);
      popupText.PulseReminderEveryXSeconds(3f);
      UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
      this.m_doneButton.GetComponentInChildren<HighlightState>().ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    }
    this.SaveCurrentDeckAndEnterDeckListMode();
    return true;
  }

  public bool OnBackOutOfMercenariesContents()
  {
    if (this.GetCurrentContentType() != DeckTray.DeckContentTypes.INVALID && !this.GetCurrentContent().IsModeActive() || !this.IsShowingTeamContents())
      return false;
    LettuceCollectionDisplay lcd = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if ((UnityEngine.Object) lcd != (UnityEngine.Object) null && lcd.IsMercenaryDetailsDisplayActive())
    {
      Log.DeckTray.Print("backing out of merc detail display");
      lcd.HideMercenaryDetailsDisplay();
      return true;
    }
    Log.DeckTray.Print("backing out of team contents");
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (!editingTeam.IsBeingDeleted())
    {
      if (!editingTeam.IsValid())
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_LETTUCE_COLLECTION_INCOMPLETE_TEAM_HEADER"),
          m_text = GameStrings.Get("GLUE_LETTUCE_COLLECTION_INCOMPLETE_TEAM_DESC"),
          m_confirmText = GameStrings.Get("GLOBAL_CONTINUE"),
          m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
          m_showAlertIcon = true,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, o) =>
          {
            if (response == AlertPopup.Response.CONFIRM)
              this.SaveCurrentTeamAndEnterTeamListMode();
            else
              Navigation.Push(new Navigation.NavigateBackHandler(((DeckTray) this).OnBackOutOfContainerContents));
          })
        };
        DialogManager.Get().ShowPopup(info);
        return true;
      }
      LettuceMercenary unequippedMerc = editingTeam.GetMercs().FirstOrDefault<LettuceMercenary>((Func<LettuceMercenary, bool>) (m => m.IsEquipmentSlotUnassigned() && m.m_equipmentList.Any<LettuceAbility>((Func<LettuceAbility, bool>) (e => e.Owned))));
      if (unequippedMerc != null)
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_LETTUCE_COLLECTION_EQUIPMENT_AVAILABLE_HEADER"),
          m_text = GameStrings.Get("GLUE_LETTUCE_COLLECTION_EQUIPMENT_AVAILABLE_DESC"),
          m_confirmText = GameStrings.Get("GLOBAL_CONTINUE"),
          m_cancelText = GameStrings.Get("GLUE_LETTUCE_COLLECTION_EQUIPMENT_AVAILABLE_BUTTON_EQUIP"),
          m_showAlertIcon = true,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, o) =>
          {
            if (response == AlertPopup.Response.CONFIRM)
            {
              this.SaveCurrentTeamAndEnterTeamListMode();
            }
            else
            {
              Navigation.Push(new Navigation.NavigateBackHandler(((DeckTray) this).OnBackOutOfContainerContents));
              lcd.ShowMercenaryDetailsDisplay(unequippedMerc);
            }
          })
        };
        DialogManager.Get().ShowPopup(info);
        return true;
      }
    }
    this.SaveCurrentTeamAndEnterTeamListMode();
    return true;
  }

  public bool OnBackOutOfDeckContentsDuel() => AdventureDungeonCrawlDisplay.Get().BackFromDeckEdit(this.m_cardsContent.GetEditingDeck()) && this.OnConfirmBackOutOfDeckContentsDuel();

  public bool OnConfirmBackOutOfDeckContentsDuel()
  {
    if (this.GetCurrentContentType() != DeckTray.DeckContentTypes.INVALID && (UnityEngine.Object) this.GetCurrentContent() != (UnityEngine.Object) null && !this.GetCurrentContent().IsModeActive() || !this.IsShowingDeckContents())
      return false;
    DeckHelper.Get().Hide();
    CollectionManager.Get().DoneEditing();
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
    {
      collectibleDisplay.HideConvertTutorial();
      collectibleDisplay.OnDoneEditingDeck();
      collectibleDisplay.EnableInput(false);
    }
    return true;
  }

  private bool OnBackOutOfCollectionScreen()
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
      return true;
    if ((UnityEngine.Object) NotificationManager.Get() != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL16"));
    if ((UnityEngine.Object) this.m_doneButton != (UnityEngine.Object) null)
      this.m_doneButton.GetComponentInChildren<HighlightState>().ChangeState(ActorStateType.HIGHLIGHT_OFF);
    if (this.GetCurrentContentType() != DeckTray.DeckContentTypes.INVALID && (UnityEngine.Object) this.GetCurrentContent() != (UnityEngine.Object) null && !this.GetCurrentContent().IsModeActive() || SceneMgr.Get() != null && !SceneMgr.Get().IsInTavernBrawlMode() && !SceneMgr.Get().IsInLettuceMode() && this.IsShowingDeckContents())
      return false;
    if ((!SceneMgr.Get().IsInTavernBrawlMode() || SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAME_MODE) && !SceneMgr.Get().IsInLettuceMode())
      AnimationUtil.DelayedActivate(this.gameObject, 0.25f, false);
    if ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() != (UnityEngine.Object) null)
      CollectionManager.Get().GetCollectibleDisplay().Exit();
    return true;
  }

  public static void SaveCurrentDeck()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
      return;
    editedDeck.SendChanges(CollectionDeck.ChangeSource.SaveCurrentDeck);
    if (Network.IsLoggedIn())
      CollectionManager.Get().SetTimeOfLastPlayerDeckSave(new DateTime?(DateTime.Now));
    Log.Decks.PrintInfo("Finished Editing Deck:");
    editedDeck.LogDeckStringInformation();
    FiresideGatheringManager.Get().UpdateDeckValidity();
  }

  private void SaveCurrentDeckAndEnterDeckListMode()
  {
    CollectionDeckTray.SaveCurrentDeck();
    if (SceneMgr.Get().IsInTavernBrawlMode())
    {
      if ((UnityEngine.Object) TavernBrawlDisplay.Get() != (UnityEngine.Object) null)
        TavernBrawlDisplay.Get().BackFromDeckEdit(true);
      this.m_cardsContent.UpdateCardList();
    }
    else
    {
      this.SetTrayMode(DeckTray.DeckContentTypes.Decks);
      CollectionManager.Get().DoneEditing();
      this.UpdateDoneButtonText();
      CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
      if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
        return;
      collectibleDisplay.OnDoneEditingDeck();
    }
  }

  private void SaveCurrentTeamAndEnterTeamListMode()
  {
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam != null)
    {
      editingTeam.SendChanges();
      if (Network.IsLoggedIn())
        CollectionManager.Get().SetTimeOfLastPlayerDeckSave(new DateTime?(DateTime.Now));
    }
    this.SetTrayMode(DeckTray.DeckContentTypes.Teams);
    CollectionManager.Get().DoneEditingTeam();
    this.UpdateDoneButtonText();
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.OnDoneEditingTeam();
  }

  public void CompleteMyDeckButtonPress()
  {
    if (!Network.IsLoggedIn())
    {
      CollectionManager.ShowFeatureDisabledWhileOfflinePopup();
    }
    else
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_COMPLETE_POPUP_HEADER"),
        m_text = GameStrings.Get("GLUE_COLLECTION_DECK_RULE_FINISH_AUTOMATICALLY"),
        m_confirmText = GameStrings.Get("GLUE_COLLECTION_DECK_COMPLETE_POPUP_CONFIRM"),
        m_cancelText = GameStrings.Get("GLUE_COLLECTION_DECK_COMPLETE_POPUP_CANCEL"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response != AlertPopup.Response.CONFIRM)
            return;
          this.FinishMyDeck(false);
        })
      };
      DialogManager.Get().ShowPopup(info);
    }
  }

  public void FinishMyDeck(bool backOutWhenComplete)
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    bool allowSmartDeckCompletion = SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER;
    CollectionManager.Get().AutoFillDeck(editedDeck, allowSmartDeckCompletion, (CollectionManager.DeckAutoFillCallback) ((filledDeck, fillCards) => this.AutoAddCardsAndTryToBackOut(fillCards, filledDeck.GetRuleset(), backOutWhenComplete)));
  }

  private void AutoAddCardsAndTryToBackOut(
    IEnumerable<DeckMaker.DeckFill> fillCards,
    DeckRuleset deckRuleset,
    bool backOutWhenComplete)
  {
    CollectionDeckTray.PopuplateDeckCompleteCallback completedCallback = (CollectionDeckTray.PopuplateDeckCompleteCallback) null;
    if (backOutWhenComplete)
      completedCallback = (CollectionDeckTray.PopuplateDeckCompleteCallback) ((addedCards, removedCards) => this.OnBackOutOfContainerContents());
    this.StartCoroutine(this.AutoAddCardsWithTiming(fillCards, deckRuleset, false, completedCallback));
  }

  public void PopulateDeck(
    IEnumerable<DeckMaker.DeckFill> fillCards,
    CollectionDeckTray.PopuplateDeckCompleteCallback completedCallback)
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck.HasClass(TAG_CLASS.DEATHKNIGHT))
    {
      editedDeck.ClearRuneOrder();
      this.m_runeIndicatorVisual.ResetRuneButtons();
    }
    editedDeck.ClearSlotContents();
    this.GetCardsContent().UpdateCardList();
    this.StartCoroutine(this.AutoAddCardsWithTiming(fillCards, (DeckRuleset) null, true, completedCallback));
  }

  private IEnumerator AutoAddCardsWithTiming(
    IEnumerable<DeckMaker.DeckFill> fillCards,
    DeckRuleset deckRuleset,
    bool allowInvalid,
    CollectionDeckTray.PopuplateDeckCompleteCallback completedCallback)
  {
    CollectionDeckTray collectionDeckTray = this;
    collectionDeckTray.AllowInput(false);
    CollectionManager.Get().GetCollectibleDisplay().EnableInput(false);
    List<EntityDef> addedCards = (List<EntityDef>) null;
    List<EntityDef> removedCards = (List<EntityDef>) null;
    if (completedCallback != null)
    {
      addedCards = new List<EntityDef>();
      removedCards = new List<EntityDef>();
    }
    if (CollectionManager.Get().IsInEditMode())
    {
      CollectionDeck deck = CollectionManager.Get().GetEditedDeck();
      int maxDeckSize = deckRuleset == null ? int.MaxValue : deckRuleset.GetDeckSize(deck);
      foreach (DeckMaker.DeckFill fillCard in fillCards)
      {
        if (deck != null && deck.IsBeingEdited() && (deck.HasReplaceableSlot() || deck.GetTotalCardCount() < maxDeckSize))
        {
          EntityDef addCard = fillCard.m_addCard;
          EntityDef removeTemplate = fillCard.m_removeTemplate;
          if (removeTemplate != null)
          {
            bool flag = collectionDeckTray.RemoveCard(removeTemplate.GetCardId(), TAG_PREMIUM.NORMAL, false);
            if (!flag)
              flag = collectionDeckTray.RemoveCard(removeTemplate.GetCardId(), TAG_PREMIUM.GOLDEN, false);
            if (!flag)
              flag = collectionDeckTray.RemoveCard(removeTemplate.GetCardId(), TAG_PREMIUM.SIGNATURE, false);
            if (!flag)
              flag = collectionDeckTray.RemoveCard(removeTemplate.GetCardId(), TAG_PREMIUM.DIAMOND, false);
            if (flag && removedCards != null)
              removedCards.Add(removeTemplate);
          }
          if (addCard != null && (0 | (collectionDeckTray.AddCardWithPreferredPremium(addCard, true) ? 1 : 0)) != 0)
          {
            addedCards?.Add(addCard);
            yield return (object) new WaitForSeconds(0.2f);
          }
        }
        else
          break;
      }
      deck = (CollectionDeck) null;
    }
    CollectionManager.Get().GetCollectibleDisplay().EnableInput(true);
    collectionDeckTray.AllowInput(true);
    if (completedCallback != null)
      completedCallback(addedCards, removedCards);
  }

  public void PopulateTeam(
    IEnumerable<LettuceCollectionDisplay.TeamCopyingModule.TeamFill> fillCards,
    CollectionDeckTray.PopuplateDeckCompleteCallback completedCallback)
  {
    this.StartCoroutine(this.AutoAddMercenariesToTeamWithTiming(fillCards, completedCallback));
  }

  private IEnumerator AutoAddMercenariesToTeamWithTiming(
    IEnumerable<LettuceCollectionDisplay.TeamCopyingModule.TeamFill> fillCards,
    CollectionDeckTray.PopuplateDeckCompleteCallback completedCallback)
  {
    CollectionDeckTray collectionDeckTray = this;
    collectionDeckTray.AllowInput(false);
    CollectionManager.Get().GetCollectibleDisplay().EnableInput(false);
    List<EntityDef> addedCards = (List<EntityDef>) null;
    List<EntityDef> removedCards = (List<EntityDef>) null;
    if (completedCallback != null)
    {
      addedCards = new List<EntityDef>();
      removedCards = new List<EntityDef>();
    }
    if (CollectionManager.Get().IsInEditTeamMode())
    {
      LettuceTeam team = CollectionManager.Get().GetEditingTeam();
      if (team != null && team.IsBeingEdited())
      {
        team.ClearContents();
        int maxTeamSize = CollectionManager.Get().GetTeamSize();
        foreach (LettuceCollectionDisplay.TeamCopyingModule.TeamFill fillCard in fillCards)
        {
          if (team.GetMercCount() < maxTeamSize)
          {
            EntityDef addCard = fillCard.m_addCard;
            if (addCard != null && collectionDeckTray.GetMercsContent().AddMerc(addCard, true, loadout: fillCard.m_addLoadout))
            {
              addedCards?.Add(addCard);
              yield return (object) new WaitForSeconds(0.2f);
            }
          }
          else
            break;
        }
      }
      team = (LettuceTeam) null;
    }
    CollectionManager.Get().GetCollectibleDisplay().EnableInput(true);
    collectionDeckTray.AllowInput(true);
    CollectionDeckTray.PopuplateDeckCompleteCallback completeCallback = completedCallback;
    if (completeCallback != null)
      completeCallback(addedCards, removedCards);
  }

  public override void UpdateDoneButtonText()
  {
    bool flag1 = !CollectionManager.Get().IsInEditMode() && !CollectionManager.Get().IsInEditTeamMode() || CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE;
    if (SceneMgr.Get().IsInTavernBrawlMode())
    {
      TavernBrawlDisplay tavernBrawlDisplay = TavernBrawlDisplay.Get();
      flag1 = (UnityEngine.Object) tavernBrawlDisplay != (UnityEngine.Object) null && !tavernBrawlDisplay.IsInDeckEditMode() && !(bool) UniversalInputManager.UsePhoneUI;
    }
    if (SceneMgr.Get().IsInDuelsMode())
      flag1 = !((UnityEngine.Object) AdventureDungeonCrawlDisplay.Get() != (UnityEngine.Object) null) || !AdventureDungeonCrawlDisplay.Get().IsDuelsDeckValid();
    if (SceneMgr.Get().IsInLettuceMode() && CollectionManager.Get().GetEditingTeam() != null)
      flag1 = false;
    bool flag2 = (UnityEngine.Object) this.m_backArrow != (UnityEngine.Object) null;
    if (flag1)
    {
      this.m_doneButton.SetText(flag2 ? "" : GameStrings.Get("GLOBAL_BACK"));
      if (!flag2)
        return;
      this.m_backArrow.gameObject.SetActive(true);
    }
    else
    {
      this.m_doneButton.SetText(GameStrings.Get("GLOBAL_DONE"));
      if (!flag2)
        return;
      this.m_backArrow.gameObject.SetActive(false);
    }
  }

  protected override void HideUnseenDeckTrays()
  {
    base.HideUnseenDeckTrays();
    if (!((UnityEngine.Object) this.m_decksContent != (UnityEngine.Object) null))
      return;
    this.m_decksContent.HideTraySectionsNotInBounds(this.m_scrollbar.m_ScrollBounds.bounds);
  }

  protected override void OnCardTilePress(DeckTrayDeckTileVisual cardTile)
  {
    if (UniversalInputManager.Get().IsTouchMode())
    {
      this.ShowDeckBigCard(cardTile, 0.2f);
    }
    else
    {
      if (!((UnityEngine.Object) CollectionInputMgr.Get() != (UnityEngine.Object) null) || SceneMgr.Get().IsInDuelsMode() && DuelsConfig.IsCardLoadoutTreasure(cardTile.GetCardID()))
        return;
      this.HideDeckBigCard(cardTile, false);
    }
  }

  protected override IEnumerator UpdateTrayMode()
  {
    yield return (object) base.UpdateTrayMode();
    this.UpdateRuneIndicatorVisual();
  }

  private void UpdateRuneIndicatorVisual(CollectionDeck deck)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (this.m_currentContent == DeckTray.DeckContentTypes.Cards && collectionManager != null && collectionManager.IsEditingDeathKnightDeck())
    {
      if (deck == null)
      {
        Log.ErrorReporter.PrintError("UpdateTrayMode::CollectionDeckTray deck is null!");
      }
      else
      {
        this.m_runeIndicatorVisual.Show();
        this.m_runeIndicatorVisual.InitializeWithTilePool(deck, this);
        this.m_cardsContent.SetRuneIndicatorSpacerVisible(true);
        if (TavernBrawlDisplay.IsTavernBrawlViewing())
          this.m_runeIndicatorVisual.DisableRuneButtons();
        else
          CollectionDeckTray.TryToShowDeathKnightDeckBuildingTutorial();
      }
    }
    else
    {
      this.m_runeIndicatorVisual.Hide();
      this.m_cardsContent.SetRuneIndicatorSpacerVisible(false);
    }
  }

  private void UpdateRuneIndicatorVisual()
  {
    if ((UnityEngine.Object) this.m_runeIndicatorVisual == (UnityEngine.Object) null)
      return;
    CollectionManager collectionManager = CollectionManager.Get();
    bool flag = false;
    CollectibleDisplay collectibleDisplay = collectionManager.GetCollectibleDisplay();
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE)
      flag = true;
    this.UpdateRuneIndicatorVisual(!flag ? collectionManager.GetEditedDeck() : this.m_cardsContent.GetEditingDeck());
  }

  private void UpdateEditedDeckBoxColliderHeightForDeathKnight()
  {
    CollectionDeckBoxVisual editingDeckBox = this.GetEditingDeckBox();
    if (!(bool) (UnityEngine.Object) editingDeckBox)
      return;
    editingDeckBox.UpdateColliderHeightForDeathKnight();
  }

  private static void TryToShowDeathKnightDeckBuildingTutorial()
  {
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    bool flag = false;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
    {
      DeckTemplatePicker deckTemplatePicker = (bool) UniversalInputManager.UsePhoneUI ? collectibleDisplay.GetPhoneDeckTemplateTray() : collectibleDisplay.m_pageManager.GetDeckTemplatePicker();
      if ((UnityEngine.Object) deckTemplatePicker != (UnityEngine.Object) null)
        flag = deckTemplatePicker.IsShowingPacks();
    }
    if (flag)
      return;
    TutorialDeathKnightDeckBuilding.ShowTutorial(UIVoiceLinesManager.TriggerType.STARTED_EDITING_DEATH_KNIGHT_DECK);
  }

  private void OnCardTileTap(DeckTrayDeckTileVisual cardTile)
  {
    if ((UnityEngine.Object) cardTile == (UnityEngine.Object) null || (UnityEngine.Object) this.m_cardsContent == (UnityEngine.Object) null)
      return;
    UniversalInputManager universalInputManager = UniversalInputManager.Get();
    if (universalInputManager == null || !universalInputManager.IsTouchMode())
      return;
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return;
    CollectibleDisplay collectibleDisplay = collectionManager.GetCollectibleDisplay();
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null || collectibleDisplay.GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE)
      return;
    CollectionDeck editedDeck = collectionManager.GetEditedDeck();
    if (editedDeck == null)
      return;
    CollectionDeckSlot slot = cardTile.GetSlot();
    if (editedDeck.IsValidSlot(slot, enforceRemainingDeckRuleset: true))
      return;
    this.m_cardsContent.ShowDeckHelper(slot, true);
  }

  protected override void OnCardTileOver(DeckTrayDeckTileVisual cardTile)
  {
    if (UniversalInputManager.Get().IsTouchMode() || !((UnityEngine.Object) CollectionInputMgr.Get() == (UnityEngine.Object) null) && CollectionInputMgr.Get().HasHeldCard())
      return;
    this.ShowDeckBigCard(cardTile, 0.0f);
  }

  private void OnCardTileHeld(DeckTrayDeckTileVisual cardTile)
  {
    if (!((UnityEngine.Object) CollectionInputMgr.Get() != (UnityEngine.Object) null) || TavernBrawlDisplay.IsTavernBrawlViewing() || CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE || !CollectionInputMgr.Get().GrabCardTile(cardTile) || !((UnityEngine.Object) this.m_deckBigCard != (UnityEngine.Object) null))
      return;
    this.HideDeckBigCard(cardTile, true);
  }

  protected override void OnCardTileRelease(DeckTrayDeckTileVisual cardTile)
  {
    if (DuelsConfig.IsCardLoadoutTreasure(cardTile.GetCardID()))
      return;
    this.RemoveCardTile(cardTile);
  }

  public void RemoveCardTile(DeckTrayDeckTileVisual cardTile)
  {
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE || CollectionInputMgr.Get().HasHeldCard())
      return;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (UniversalInputManager.Get().IsTouchMode())
    {
      this.HideDeckBigCard(cardTile, false);
    }
    else
    {
      CollectionDeckSlot slot = cardTile.GetSlot();
      if (!editedDeck.IsValidSlot(slot, enforceRemainingDeckRuleset: true))
      {
        this.m_cardsContent.ShowDeckHelper(slot, true);
      }
      else
      {
        if ((UnityEngine.Object) CollectionInputMgr.Get() == (UnityEngine.Object) null || TavernBrawlDisplay.IsTavernBrawlViewing())
          return;
        CollectionDeckTileActor actor = cardTile.GetActor();
        Spell spell1 = actor.GetSpell(SpellType.SUMMON_IN);
        Spell spell2 = SpellManager.Get().GetSpell(spell1);
        Transform transform = spell2.transform;
        transform.position = actor.transform.position + new Vector3(-2f, 0.0f, 0.0f);
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        spell2.ActivateState(SpellStateType.BIRTH);
        this.DestroySpellAfterSeconds(spell2).Forget();
        if ((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null)
          CollectionDeckTray.Get().RemoveCard(cardTile.GetCardID(), cardTile.GetSlot().UnPreferredPremium, editedDeck.IsValidSlot(cardTile.GetSlot()));
        iTween.MoveTo(spell2.gameObject, new Vector3(transform.position.x - 10f, transform.position.y + 10f, transform.position.z), 4f);
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_remove_from_deck_instant.prefab:bcee588ddfc73844ea3a24beb63bc53f", this.gameObject);
      }
    }
  }

  private async UniTaskVoid DestroySpellAfterSeconds(Spell spell)
  {
    await UniTask.Delay(TimeSpan.FromSeconds(5.0));
    SpellManager.Get().ReleaseSpell(spell);
  }

  protected override void ShowDeckBigCard(DeckTrayDeckTileVisual cardTile, float delay = 0.0f)
  {
    CollectionDeckTileActor actor = cardTile.GetActor();
    if ((UnityEngine.Object) this.m_deckBigCard == (UnityEngine.Object) null)
      return;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    EntityDef entityDef = actor.GetEntityDef();
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(entityDef.GetCardId()))
    {
      GhostCard.Type ghostTypeFromSlot = GhostCard.GetGhostTypeFromSlot(editedDeck, cardTile.GetSlot());
      this.m_deckBigCard.Show(entityDef, actor.GetPremium(), cardDef, actor.gameObject.transform.position, ghostTypeFromSlot, delay);
      if (UniversalInputManager.Get().IsTouchMode())
        cardTile.SetHighlight(true);
      CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
      if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null) || !((UnityEngine.Object) collectibleDisplay.m_deckTemplateCardReplacePopup != (UnityEngine.Object) null))
        return;
      collectibleDisplay.m_deckTemplateCardReplacePopup.Shrink(0.1f);
    }
  }

  protected override void HideDeckBigCard(DeckTrayDeckTileVisual cardTile, bool force = false)
  {
    CollectionDeckTileActor actor = cardTile.GetActor();
    if (!((UnityEngine.Object) this.m_deckBigCard != (UnityEngine.Object) null))
      return;
    if (force)
      this.m_deckBigCard.ForceHide();
    else
      this.m_deckBigCard.Hide(actor.GetEntityDef(), actor.GetPremium());
    if (UniversalInputManager.Get().IsTouchMode())
      cardTile.SetHighlight(false);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null) || !((UnityEngine.Object) collectibleDisplay.m_deckTemplateCardReplacePopup != (UnityEngine.Object) null))
      return;
    collectibleDisplay.m_deckTemplateCardReplacePopup.Unshrink(0.1f);
  }

  private void OnCardCountUpdated(int cardCount)
  {
    string str1 = GameStrings.Get("GLUE_DECK_TRAY_CARD_COUNT_LABEL");
    string str2 = GameStrings.Format("GLUE_DECK_TRAY_COUNT", (object) cardCount, (object) CollectionManager.Get().GetDeckSize());
    this.m_countLabelText.Text = str1;
    this.m_countText.Text = str2;
  }

  private void OnDeckCountUpdated(int deckCount)
  {
    string str1 = GameStrings.Get("GLUE_DECK_TRAY_DECK_COUNT_LABEL");
    string str2 = GameStrings.Format("GLUE_DECK_TRAY_COUNT", (object) deckCount, (object) 27);
    this.m_countLabelText.Text = str1;
    this.m_countText.Text = str2;
  }

  private void OnTeamCountUpdated(int teamCount)
  {
    string str1 = GameStrings.Get("GLUE_DECK_TRAY_TEAM_COUNT_LABEL");
    string str2 = GameStrings.Format("GLUE_DECK_TRAY_COUNT", (object) teamCount, (object) 9);
    this.m_countLabelText.Text = str1;
    this.m_countText.Text = str2;
  }

  private void OnMercCountUpdated(int mercCount)
  {
    string str1 = GameStrings.Get("GLUE_DECK_TRAY_MERC_COUNT_LABEL");
    string str2 = GameStrings.Format("GLUE_DECK_TRAY_COUNT", (object) mercCount, (object) CollectionManager.Get().GetTeamSize());
    this.m_countLabelText.Text = str1;
    this.m_countText.Text = str2;
  }

  private void OnDeckCreated(long deckID, string name) => this.ResetDeckTrayScroll();

  private void OnTeamCreated(long deckID) => this.ResetDeckTrayScroll();

  private void OnCMViewModeChanged(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse)
  {
    DeckTray.DeckContentTypes typeFromViewMode = this.GetContentTypeFromViewMode(mode);
    this.m_cardsContent.ShowFakeDeck(mode == CollectionUtils.ViewMode.DECK_TEMPLATE);
    if (!triggerResponse)
      return;
    this.m_decksContent.UpdateDeckName();
    if (this.m_currentContent == DeckTray.DeckContentTypes.Decks)
      return;
    this.SetTrayMode(typeFromViewMode);
  }

  private DeckTray.DeckContentTypes GetContentTypeFromViewMode(
    CollectionUtils.ViewMode viewMode)
  {
    switch (viewMode)
    {
      case CollectionUtils.ViewMode.HERO_SKINS:
        return DeckTray.DeckContentTypes.HeroSkin;
      case CollectionUtils.ViewMode.CARD_BACKS:
        return DeckTray.DeckContentTypes.CardBack;
      case CollectionUtils.ViewMode.COINS:
        return DeckTray.DeckContentTypes.Coin;
      default:
        return DeckTray.DeckContentTypes.Cards;
    }
  }

  private void OnHeroAssigned(string cardID) => this.m_decksContent.UpdateEditingDeckBoxVisual(cardID);

  private CollectionCardEventHandler GetCardEventHandler(string cardID)
  {
    CollectionDeckTray.CollectionCardEventHandlerData eventHandlerData = this.m_cardEventHandlers.Find((Predicate<CollectionDeckTray.CollectionCardEventHandlerData>) (data => data.CardID == cardID));
    if (eventHandlerData != null)
    {
      if ((UnityEngine.Object) eventHandlerData.GetInstance() == (UnityEngine.Object) null)
      {
        CollectionCardEventHandler cardEventHandler = UnityEngine.Object.Instantiate<CollectionCardEventHandler>(eventHandlerData.CardHandlerPrefab);
        cardEventHandler.transform.parent = this.transform;
        TransformUtil.Identity((Component) cardEventHandler);
        eventHandlerData.SetInstance(cardEventHandler);
      }
      return eventHandlerData.GetInstance();
    }
    int dbId = GameUtils.TranslateCardIdToDbId(cardID);
    for (int index = 0; index < this.m_tagCardEventHandlers.Count; ++index)
    {
      CollectionDeckTray.CollectionTagEventHandlerData cardEventHandler = this.m_tagCardEventHandlers[index];
      if (GameUtils.GetCardTagValue(dbId, cardEventHandler.Tag) != 0)
        return cardEventHandler.cardHandlerInstance;
    }
    return this.m_defaultCardEventHandler;
  }

  private int CalculateNumCardsNeededToCraftToReachMinimumDeckSize(CollectionDeck deck)
  {
    if (deck == null)
    {
      Log.CollectionManager.PrintWarning("GetNumCardsNeededToCraftToReachMinimumDeckSize - No deck to check ruleset against.");
      return 0;
    }
    CollectionDeck deck1 = new CollectionDeck();
    deck1.CopyFrom(deck);
    deck1.ClearSlotContents();
    int minimumAllowedDeckSize = deck1.GetRuleset().GetMinimumAllowedDeckSize(deck1);
    IEnumerable<DeckMaker.DeckFill> fillCards = DeckMaker.GetFillCards(deck1, deck1.GetRuleset());
    int num = 0;
    foreach (DeckMaker.DeckFill deckFill in fillCards)
    {
      TAG_PREMIUM? premiumThatCanBeAdded = deck1.GetPreferredPremiumThatCanBeAdded(deckFill.m_addCard.GetCardId());
      if (premiumThatCanBeAdded.HasValue)
      {
        deck1.AddCard(deckFill.m_addCard.GetCardId(), premiumThatCanBeAdded.Value, false);
        ++num;
        if (num >= minimumAllowedDeckSize)
          return 0;
      }
    }
    return minimumAllowedDeckSize - num;
  }

  public void HighlightBackButton() => this.m_doneButton.GetComponentInChildren<HighlightState>().ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);

  public Vector3 GetFirstRuneIndicatorButtonPosition() => this.m_runeIndicatorVisual.runeButtons[0].transform.position;

  public void SetRuneIndicatorHighlighted(bool highlighted) => this.m_runeIndicatorVisual.HighlightAllRunes(highlighted);

  [Serializable]
  public class CollectionCardEventHandlerData
  {
    public string CardID;
    public CollectionCardEventHandler CardHandlerPrefab;
    private CollectionCardEventHandler cardHandlerInstance;

    public CollectionCardEventHandler GetInstance() => this.cardHandlerInstance;

    public void SetInstance(CollectionCardEventHandler instance) => this.cardHandlerInstance = instance;
  }

  [Serializable]
  public class CollectionTagEventHandlerData
  {
    public GAME_TAG Tag;
    public CollectionCardEventHandler cardHandlerInstance;
  }

  public delegate void PopuplateDeckCompleteCallback(
    List<EntityDef> addedCards,
    List<EntityDef> removedCards);
}
