using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckHelper : MonoBehaviour
{
  public UberText m_instructionText;
  public UberText m_replaceText;
  public GameObject m_rootObject;
  public UIBButton m_suggestDoneButton;
  public UIBButton m_replaceDoneButton;
  public PegUIElement m_inputBlocker;
  public Vector3 m_deckCardLocalScale = new Vector3(5.75f, 5.75f, 5.75f);
  public GameObject m_3choiceContainer;
  public GameObject m_replaceContainer;
  public GameObject m_2choiceContainer;
  public Vector3 m_cardSpacing;
  public GameObject m_suggestACardPane;
  public GameObject m_replaceACardPane;
  public string m_replaceACardSound;
  public UIBButton m_innkeeperPopup;
  private static DeckHelper s_instance;
  private Actor m_replaceCardActor;
  private List<Actor> m_choiceActors = new List<Actor>();
  private bool m_shown;
  private DeckTrayDeckTileVisual m_highlightedTile;
  private CollectionDeckSlot m_nextSlotToReplace;
  private bool m_replaceSingleSlotOnly;
  private Vector3 m_innkeeperFullScale;
  private bool m_innkeeperPopupShown;
  private const float INNKEEPER_POPUP_DURATION = 7f;
  private List<EntityDef> m_chosenCards = new List<EntityDef>();
  private DeckHelper.DelCompleteCallback m_onCompleteCallback;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    DeckHelper.s_instance = this;
    this.m_rootObject.SetActive(false);
    this.m_replaceDoneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.EndButtonClick));
    this.m_suggestDoneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.EndButtonClick));
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if ((UnityEngine.Object) this.m_innkeeperPopup != (UnityEngine.Object) null)
      {
        this.m_innkeeperFullScale = this.m_innkeeperPopup.gameObject.transform.localScale;
        this.m_innkeeperPopup.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.InnkeeperPopupClicked));
      }
    }
    else
      this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.EndButtonClick));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy() => DeckHelper.s_instance = (DeckHelper) null;

  private void EndButtonClick(UIEvent e) => Navigation.GoBack();

  public static DeckHelper Get()
  {
    if ((UnityEngine.Object) DeckHelper.s_instance == (UnityEngine.Object) null)
    {
      string assetRef = (bool) UniversalInputManager.UsePhoneUI ? "DeckHelper_phone.prefab:e2c93e38a85f44eadb1aee945b1c4636" : "DeckHelper.prefab:69e71904d55994cc28b41f5950e6608f";
      DeckHelper.s_instance = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef).GetComponent<DeckHelper>();
    }
    return DeckHelper.s_instance;
  }

  public bool IsActive() => this.m_shown;

  public void OnCardAdded(CollectionDeck deck)
  {
    if (!this.IsActive())
      return;
    this.HandleDeckChanged(deck);
  }

  public static bool HasChoicesToOffer(CollectionDeck deck) => DeckMaker.GetFillCardChoices(deck, (EntityDef) null, 1).m_addChoices.Count > 0;

  public void UpdateChoices(CollectionDeckSlot slotToReplace)
  {
    this.CleanOldChoices();
    if (!this.IsActive())
      return;
    EntityDef entityDef = slotToReplace?.GetEntityDef();
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    DeckMaker.DeckChoiceFill cardsToShow = DeckMaker.GetFillCardChoices(editedDeck, entityDef, 3);
    if (entityDef == null && cardsToShow.m_removeTemplate != null)
      entityDef = cardsToShow.m_removeTemplate;
    string reason = cardsToShow.m_reason;
    if (cardsToShow == null || cardsToShow.m_addChoices.Count == 0)
    {
      Debug.LogError((object) "DeckHelper.GetChoices() - Can't find choices!!!!");
    }
    else
    {
      if ((UnityEngine.Object) this.m_instructionText != (UnityEngine.Object) null)
      {
        bool flag = !this.m_instructionText.Text.Equals(reason);
        this.m_instructionText.Text = reason;
        if ((bool) UniversalInputManager.UsePhoneUI & flag)
        {
          if (NotificationManager.Get().IsQuotePlaying)
            this.m_instructionText.Text = "";
          else
            this.ShowInnkeeperPopup();
        }
      }
      this.m_replaceACardPane.SetActive(slotToReplace != null);
      this.m_suggestACardPane.SetActive(slotToReplace == null);
      if (slotToReplace != null && entityDef != null)
      {
        GhostCard.Type ghostTypeFromSlot = GhostCard.GetGhostTypeFromSlot(editedDeck, slotToReplace);
        this.m_replaceCardActor = this.LoadBestCardActor(entityDef, TAG_PREMIUM.NORMAL, ghostTypeFromSlot);
        if ((UnityEngine.Object) this.m_replaceCardActor != (UnityEngine.Object) null)
        {
          GameUtils.SetParent((Component) this.m_replaceCardActor, this.m_replaceContainer);
          if (ghostTypeFromSlot == GhostCard.Type.MISSING)
          {
            RenderToTexture component = this.m_replaceCardActor.m_ghostCardGameObject.GetComponent<RenderToTexture>();
            BoxCollider boxCollider = this.m_replaceCardActor.m_ghostCardGameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(component.m_Width, 2f, component.m_Height);
            boxCollider.gameObject.AddComponent<PegUIElement>().AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnGhostCardRelease(this.m_replaceCardActor)));
          }
        }
        if ((UnityEngine.Object) this.m_replaceText != (UnityEngine.Object) null)
        {
          switch (ghostTypeFromSlot)
          {
            case GhostCard.Type.MISSING:
              this.m_replaceText.Text = GameStrings.Get("GLUE_COLLECTION_DECK_HELPER_REPLACE_UNOWNED_CARD");
              break;
            case GhostCard.Type.NOT_VALID:
              this.m_replaceText.Text = GameUtils.IsCardGameplayEventActive(entityDef) ? (!CollectionManager.Get().ShouldAccountSeeStandardWild() ? (!GameUtils.IsBanned(editedDeck, entityDef) ? GameStrings.Get("GLUE_COLLECTION_DECK_HELPER_REPLACE_INVALID_CARD_NPR") : GameStrings.Get("GLUE_COLLECTION_DECK_HELPER_REPLACE_CARD")) : GameStrings.Get("GLUE_COLLECTION_DECK_HELPER_REPLACE_INVALID_CARD")) : GameStrings.Get("GLUE_COLLECTION_DECK_HELPER_REPLACE_UNPLAYABLE_CARD");
              break;
            default:
              this.m_replaceText.Text = GameStrings.Get("GLUE_COLLECTION_DECK_HELPER_REPLACE_CARD");
              break;
          }
        }
        if (slotToReplace.Owned && !Options.Get().GetBool(Option.HAS_SEEN_DECK_TEMPLATE_GHOST_CARD, false))
          Options.Get().SetBool(Option.HAS_SEEN_DECK_TEMPLATE_GHOST_CARD, true);
        if (!editedDeck.IsValidSlot(slotToReplace) && !Options.Get().GetBool(Option.HAS_SEEN_INVALID_ROTATED_CARD, false))
          Options.Get().SetBool(Option.HAS_SEEN_INVALID_ROTATED_CARD, true);
        if (this.m_replaceACardSound != string.Empty)
          SoundManager.Get().LoadAndPlay((AssetReference) this.m_replaceACardSound);
      }
      int num1 = entityDef != null ? 1 : 0;
      int num2 = Mathf.Min(num1 != 0 ? 2 : 3, cardsToShow.m_addChoices.Count);
      GameObject parent = num1 != 0 ? this.m_2choiceContainer : this.m_3choiceContainer;
      for (int index = 0; index < num2; ++index)
      {
        EntityDef addChoice = cardsToShow.m_addChoices[index];
        TAG_PREMIUM? premiumThatCanBeAdded = editedDeck.GetPreferredPremiumThatCanBeAdded(addChoice.GetCardId());
        if (premiumThatCanBeAdded.HasValue)
        {
          Actor actor = this.LoadBestCardActor(addChoice, premiumThatCanBeAdded.Value);
          if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
          {
            GameUtils.SetParent((Component) actor, parent);
            PegUIElement pegUiElement = actor.GetCollider().gameObject.AddComponent<PegUIElement>();
            pegUiElement.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnVisualRelease(actor, cardsToShow.m_removeTemplate)));
            pegUiElement.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.OnVisualOver(actor)));
            pegUiElement.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.OnVisualOut(actor)));
            this.m_choiceActors.Add(actor);
          }
        }
      }
      this.PositionAndShowChoices(slotToReplace);
    }
  }

  private Actor LoadBestCardActor(
    EntityDef entityDef,
    TAG_PREMIUM premiumToUse,
    GhostCard.Type ghostCard = GhostCard.Type.NONE)
  {
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(entityDef.GetCardId(), new CardPortraitQuality(3, premiumToUse)))
    {
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(entityDef, premiumToUse), AssetLoadingOptions.IgnorePrefabPosition);
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("DeckHelper - FAILED to load actor \"{0}\"", (object) this.name));
        return (Actor) null;
      }
      Actor component = gameObject.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("DeckHelper - ERROR actor \"{0}\" has no Actor component", (object) this.name));
        return (Actor) null;
      }
      component.transform.parent = this.transform;
      LayerUtils.SetLayer((Component) component, this.gameObject.layer);
      component.SetEntityDef(entityDef);
      component.SetCardDef(cardDef);
      component.SetPremium(premiumToUse);
      component.GhostCardEffect(ghostCard, premiumToUse);
      component.UpdateAllComponents();
      component.Hide();
      component.gameObject.name = cardDef.CardDef.name + "_actor";
      return component;
    }
  }

  private void CleanOldChoices()
  {
    foreach (Component choiceActor in this.m_choiceActors)
      UnityEngine.Object.Destroy((UnityEngine.Object) choiceActor.gameObject);
    this.m_choiceActors.Clear();
    if (!((UnityEngine.Object) this.m_replaceCardActor != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_replaceCardActor.gameObject);
    this.m_replaceCardActor = (Actor) null;
  }

  private void PositionAndShowChoices(CollectionDeckSlot slotToReplace)
  {
    for (int index = 0; index < this.m_choiceActors.Count; ++index)
    {
      Actor choiceActor = this.m_choiceActors[index];
      choiceActor.transform.localPosition = this.m_cardSpacing * (float) index;
      choiceActor.Show();
      CollectionCardVisual.ShowActorShadow(choiceActor, true);
    }
    if ((UnityEngine.Object) this.m_replaceCardActor != (UnityEngine.Object) null)
      this.m_replaceCardActor.Show();
    if ((UnityEngine.Object) this.m_highlightedTile != (UnityEngine.Object) null)
      this.m_highlightedTile.SetHighlight(false);
    if (slotToReplace != null)
    {
      DeckTrayCardListContent cardsContent = CollectionDeckTray.Get().GetCardsContent();
      if ((UnityEngine.Object) cardsContent != (UnityEngine.Object) null)
      {
        this.m_highlightedTile = cardsContent.GetCardTileVisual(slotToReplace.Index);
        if ((UnityEngine.Object) this.m_highlightedTile != (UnityEngine.Object) null)
          this.m_highlightedTile.SetHighlight(true);
      }
    }
    this.StartCoroutine(this.WaitAndAnimateChoices());
  }

  private IEnumerator WaitAndAnimateChoices()
  {
    yield return (object) new WaitForEndOfFrame();
    for (int index = 0; index < this.m_choiceActors.Count; ++index)
    {
      if (this.m_choiceActors[index].isActiveAndEnabled)
        this.m_choiceActors[index].ActivateSpellBirthState(SpellType.SUMMON_IN_FORGE);
    }
    if ((UnityEngine.Object) this.m_replaceCardActor != (UnityEngine.Object) null && this.m_replaceContainer.activeInHierarchy)
      this.m_replaceCardActor.ActivateSpellBirthState(SpellType.SUMMON_IN_FORGE);
  }

  public void Show(
    CollectionDeckSlot slotToReplace,
    bool replaceSingleSlotOnly,
    DeckHelper.DelCompleteCallback onCompleteCallback)
  {
    if (this.m_shown)
      return;
    Navigation.PushUnique(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.m_shown = true;
    this.m_rootObject.SetActive(true);
    if (!Options.Get().GetBool(Option.HAS_SEEN_DECK_HELPER, false) && UserAttentionManager.CanShowAttentionGrabber("DeckHelper.Show:" + (object) Option.HAS_SEEN_DECK_HELPER))
    {
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_ANNOUNCER_CM_HELP_DECK_50"), "VO_ANNOUNCER_CM_HELP_DECK_50.prefab:450881875d33d094e9a27f6260fb06d9");
      Options.Get().SetBool(Option.HAS_SEEN_DECK_HELPER, true);
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 0.1f
      });
    this.m_replaceSingleSlotOnly = replaceSingleSlotOnly;
    this.m_onCompleteCallback = onCompleteCallback;
    this.UpdateChoices(slotToReplace);
    NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1"));
    NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2"));
    NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS"));
    NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS_NPR"));
  }

  private bool OnNavigateBack()
  {
    this.Hide(false);
    return true;
  }

  public void Hide(bool popnavigation = true)
  {
    if (!this.m_shown)
      return;
    if (popnavigation)
      Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.m_shown = false;
    this.CleanOldChoices();
    this.m_rootObject.SetActive(false);
    if ((UnityEngine.Object) this.m_highlightedTile != (UnityEngine.Object) null)
      this.m_highlightedTile.SetHighlight(false);
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_screenEffectsHandle.StopEffect();
    if (this.m_onCompleteCallback == null)
      return;
    this.m_onCompleteCallback(this.m_chosenCards);
  }

  private void ShowInnkeeperPopup()
  {
    if ((UnityEngine.Object) this.m_innkeeperPopup == (UnityEngine.Object) null)
      return;
    this.m_innkeeperPopup.gameObject.SetActive(true);
    this.m_innkeeperPopupShown = true;
    this.m_innkeeperPopup.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    iTween.ScaleTo(this.m_innkeeperPopup.gameObject, iTween.Hash((object) "scale", (object) this.m_innkeeperFullScale, (object) "easetype", (object) iTween.EaseType.easeOutElastic, (object) "time", (object) 1f));
    this.StopCoroutine("WaitThenHidePopup");
    this.StartCoroutine("WaitThenHidePopup");
  }

  private IEnumerator WaitThenHidePopup()
  {
    yield return (object) new WaitForSeconds(7f);
    this.HideInnkeeperPopup();
  }

  private void InnkeeperPopupClicked(UIEvent e) => this.HideInnkeeperPopup();

  private void HideInnkeeperPopup()
  {
    if ((UnityEngine.Object) this.m_innkeeperPopup == (UnityEngine.Object) null || !this.m_innkeeperPopupShown)
      return;
    this.m_innkeeperPopupShown = false;
    iTween.ScaleTo(this.m_innkeeperPopup.gameObject, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "easetype", (object) iTween.EaseType.easeInExpo, (object) "time", (object) 0.2f, (object) "oncomplete", (object) "FinishHidePopup", (object) "oncompletetarget", (object) this.gameObject));
  }

  private void FinishHidePopup() => this.m_innkeeperPopup.gameObject.SetActive(false);

  public void OnVisualRelease(Actor addCardActor, EntityDef cardToReplace)
  {
    TooltipPanelManager.Get().HideKeywordHelp();
    addCardActor.GetSpell(SpellType.DEATHREVERSE).ActivateState(SpellStateType.BIRTH);
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if (cardToReplace != null)
    {
      string cardId = cardToReplace.GetCardId();
      CollectionDeckSlot cardIdAndValidity = collectionDeckTray.GetCardsContent().GetEditingDeck().FindFirstSlotByCardIdAndValidity(cardId, false, false, true);
      if (cardIdAndValidity != null)
      {
        if (!collectionDeckTray.RemoveCard(cardId, cardIdAndValidity.UnPreferredPremium, false, true))
          return;
        this.m_nextSlotToReplace = cardIdAndValidity.Count <= 0 ? (CollectionDeckSlot) null : cardIdAndValidity;
      }
    }
    if (!collectionDeckTray.AddCard(addCardActor.GetEntityDef(), addCardActor.GetPremium(), false, addCardActor))
      return;
    this.m_chosenCards.Add(addCardActor.GetEntityDef());
  }

  private void OnGhostCardRelease(Actor addCardActor)
  {
    GhostCard ghostCard = addCardActor.m_ghostCardGameObject.GetComponent<GhostCard>();
    foreach (Renderer componentsInChild in ghostCard.GetComponentsInChildren<MeshRenderer>())
      componentsInChild.enabled = false;
    if ((bool) UniversalInputManager.UsePhoneUI)
      LayerUtils.SetLayer(this.gameObject, GameLayer.Default);
    CraftingManager.Get().EnterCraftMode(addCardActor, (Action) (() =>
    {
      if ((UnityEngine.Object) addCardActor == (UnityEngine.Object) null)
        return;
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.StartCoroutine(this.WaitThenSetLayer(GameLayer.IgnoreFullScreenEffects));
      ghostCard.ShowRenderers();
    }));
  }

  private IEnumerator WaitThenSetLayer(GameLayer layer)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DeckHelper deckHelper = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      LayerUtils.SetLayer(deckHelper.gameObject, layer);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(0.25f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void OnVisualOver(Actor actor)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_mouse_over.prefab:0d4e20bc78956bc48b5e2963ec39211c");
    actor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
    TooltipPanelManager.Get().UpdateKeywordHelpForDeckHelper(actor.GetEntityDef(), actor);
  }

  private void OnVisualOut(Actor actor)
  {
    actor.SetActorState(ActorStateType.CARD_IDLE);
    TooltipPanelManager.Get().HideKeywordHelp();
  }

  private void HandleDeckChanged(CollectionDeck deck)
  {
    if (this.m_replaceSingleSlotOnly && this.m_nextSlotToReplace == null)
      this.Hide();
    else if (deck.CountCardsByStatus().MissingPlusInvalid == 0)
      this.Hide();
    else
      this.UpdateChoices(this.m_nextSlotToReplace ?? deck.FindInvalidSlot());
  }

  public delegate void DelCompleteCallback(List<EntityDef> chosenCards);
}
