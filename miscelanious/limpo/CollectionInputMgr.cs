using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectionInputMgr : InputMgr
{
  private static CollectionInputMgr s_instance;
  private UIBScrollable m_scrollBar;

  public static event Action<CollectionCardVisual> CollectionDraggableCardGrabbed;

  public static event Action CollectionDraggableCardDropped;

  public static CollectionInputMgr Get() => CollectionInputMgr.s_instance;

  protected override void Awake()
  {
    base.Awake();
    CollectionInputMgr.s_instance = this;
  }

  protected override void OnDestroy()
  {
    CollectionInputMgr.s_instance = (CollectionInputMgr) null;
    base.OnDestroy();
  }

  public override bool HandleKeyboardInput()
  {
    if (CollectionManager.Get() == null || SceneMgr.Get() == null)
      return false;
    if (SceneMgr.Get().IsInLettuceMode())
      this.HandleMercTeamCopying();
    else
      this.HandleDeckCopying();
    if (InputCollection.GetKeyUp(KeyCode.Escape))
    {
      if (CardBackInfoManager.IsLoadedAndShowingPreview())
      {
        CardBackInfoManager.Get().CancelPreview();
        return true;
      }
      if ((UnityEngine.Object) CraftingManager.Get() != (UnityEngine.Object) null && CraftingManager.Get().IsCardShowing() && !CraftingManager.Get().IsCancelling())
      {
        Navigation.GoBack();
        return true;
      }
    }
    if (HearthstoneApplication.GetMode() != ApplicationMode.INTERNAL || !InputCollection.GetKeyUp(KeyCode.P))
      return false;
    TAG_PREMIUM premium = TAG_PREMIUM.GOLDEN;
    if (CollectionManager.Get().GetPreferredPremium() == TAG_PREMIUM.GOLDEN)
      premium = TAG_PREMIUM.NORMAL;
    Debug.Log((object) ("setting premium preference " + (object) premium));
    CollectionManager.Get().SetPremiumPreference(premium);
    return true;
  }

  public static void PasteDeckFromClipboard()
  {
    ShareableDeck shareableDeck = ShareableDeck.DeserializeFromClipboard();
    if (shareableDeck == null)
      return;
    CollectionInputMgr.PasteDeckInEditModeFromShareableDeck(shareableDeck);
  }

  public static void PasteDeckInEditModeFromShareableDeck(ShareableDeck shareableDeck)
  {
    if (!CollectionManager.Get().IsInEditMode())
    {
      Debug.LogError((object) "Error trying to paste deck. Collection Manager is not in edit mode.");
    }
    else
    {
      CollectionDeck editedDeck1 = CollectionManager.Get().GetEditedDeck();
      if (!string.IsNullOrEmpty(shareableDeck.DeckName))
      {
        editedDeck1.Name = shareableDeck.DeckName;
        CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
        if ((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null)
          collectionDeckTray.GetDecksContent().UpdateDeckName(shareableDeck.DeckName);
      }
      editedDeck1.SetShareableDeckCreatedFrom(shareableDeck);
      DefLoader defLoader = DefLoader.Get();
      List<DeckMaker.DeckFill> fillCards = new List<DeckMaker.DeckFill>();
      if (SceneMgr.Get().IsInDuelsMode())
      {
        int num = editedDeck1.GetMaxCardCount() - 1;
        for (int index = 0; index < shareableDeck.DeckContents.Cards.Count; ++index)
        {
          DeckCardData card = shareableDeck.DeckContents.Cards[index];
          EntityDef entityDef = defLoader.GetEntityDef(card.Def.Asset);
          if (editedDeck1.FindFirstSlotByCardId(entityDef.GetCardId()) != null || editedDeck1.CanAddCard(entityDef, TAG_PREMIUM.NORMAL) || editedDeck1.CanAddCard(entityDef, TAG_PREMIUM.GOLDEN))
            fillCards.Add(new DeckMaker.DeckFill()
            {
              m_addCard = entityDef
            });
          if (fillCards.Count >= num)
            break;
        }
        List<string> cardsWithCardId = editedDeck1.GetCardsWithCardID();
        if (cardsWithCardId != null)
        {
          for (int index = 0; index < cardsWithCardId.Count; ++index)
          {
            if (DuelsConfig.IsCardLoadoutTreasure(cardsWithCardId[index]))
            {
              int dbId = GameUtils.TranslateCardIdToDbId(cardsWithCardId[index]);
              fillCards.Add(new DeckMaker.DeckFill()
              {
                m_addCard = defLoader.GetEntityDef(dbId)
              });
              break;
            }
          }
        }
      }
      else
      {
        for (int index1 = 0; index1 < shareableDeck.DeckContents.Cards.Count; ++index1)
        {
          DeckCardData card = shareableDeck.DeckContents.Cards[index1];
          EntityDef cardDef = defLoader.GetEntityDef(card.Def.Asset);
          if (CollectionManager.Get().GetTotalOwnedCount(cardDef.GetCardId()) < card.Qty)
          {
            string counterpartCardIdForFormat = GameUtils.GetOwnedCounterpartCardIDForFormat(cardDef, shareableDeck.FormatType, card.Qty);
            if (counterpartCardIdForFormat != null)
            {
              EntityDef entityDef = defLoader.GetEntityDef(counterpartCardIdForFormat);
              if (entityDef != null && editedDeck1.CanAddCard(entityDef, (TAG_PREMIUM) card.Def.Premium))
                cardDef = entityDef;
            }
          }
          for (int index2 = 0; index2 < card.Qty; ++index2)
            fillCards.Add(new DeckMaker.DeckFill()
            {
              m_addCard = cardDef
            });
        }
      }
      CollectionDeckTray.PopuplateDeckCompleteCallback completedCallback = (CollectionDeckTray.PopuplateDeckCompleteCallback) ((addedCards, removedCards) =>
      {
        CollectionDeck editedDeck2 = CollectionManager.Get().GetEditedDeck();
        DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
        int num = deckRuleset == null ? int.MinValue : deckRuleset.GetDeckSize(editedDeck2);
        if (editedDeck2 == null || !editedDeck2.HasReplaceableSlot() && editedDeck2.GetTotalCardCount() >= num)
          return;
        CollectionDeckTray.Get().OnCardManuallyAddedByUser_CheckSuggestions((IEnumerable<EntityDef>) addedCards);
      });
      CollectionDeckTray.Get().PopulateDeck((IEnumerable<DeckMaker.DeckFill>) fillCards, completedCallback);
    }
  }

  public static void AlertPlayerOnInvalidDeckPaste(string errorReason)
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_INVALID_POPUP_HEADER"),
      m_text = errorReason,
      m_okText = GameStrings.Get("GLOBAL_OKAY"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  public bool StartDragWithActor(
    Actor actor,
    CollectionUtils.ViewMode viewMode,
    bool showVisual = true,
    CollectionDeckSlot slot = null)
  {
    if (!this.CanGrabItem(actor) || (UnityEngine.Object) this.m_heldCardVisual == (UnityEngine.Object) null)
      return false;
    this.m_heldCardVisual.SetSlot(slot);
    TAG_PREMIUM premium = slot != null ? slot.UnPreferredPremium : actor.GetPremium();
    if (!this.m_heldCardVisual.ChangeActor(actor, viewMode, premium))
      return false;
    if ((UnityEngine.Object) this.m_scrollBar != (UnityEngine.Object) null)
      this.m_scrollBar.Pause(true);
    PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
    this.m_heldCardVisual.transform.position = actor.transform.position;
    this.m_heldCardVisual.Show(showVisual);
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_pick_up_card.prefab:f7fb595cdc26f2f4997b4a10eaf1d0e1", this.m_heldCardVisual.gameObject);
    return true;
  }

  public bool GrabCardVisual(CollectionCardVisual cardVisual)
  {
    Actor preferredActor = cardVisual.GetCollectionCardActors().GetPreferredActor();
    CollectionUtils.ViewMode visualType = cardVisual.GetVisualType();
    if (!this.StartDragWithActor(preferredActor, cardVisual.GetVisualType(), this.MouseIsOverDeck))
      return false;
    switch (visualType)
    {
      case CollectionUtils.ViewMode.CARDS:
        CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
        if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
          collectibleDisplay.HideFilterTrayOnStartDragCard();
        Action<CollectionCardVisual> draggableCardGrabbed = CollectionInputMgr.CollectionDraggableCardGrabbed;
        if (draggableCardGrabbed != null)
        {
          draggableCardGrabbed(cardVisual);
          break;
        }
        break;
      case CollectionUtils.ViewMode.HERO_SKINS:
        CollectionDeckTray collectionDeckTray1 = CollectionDeckTray.Get();
        if (collectionDeckTray1 != null)
        {
          DeckTrayHeroSkinContent heroSkinContent = collectionDeckTray1.GetHeroSkinContent();
          if (heroSkinContent != null)
          {
            heroSkinContent.ToggleSparkleEffects(true);
            break;
          }
          break;
        }
        break;
      case CollectionUtils.ViewMode.CARD_BACKS:
        CollectionCardBack component = preferredActor.GetComponent<CollectionCardBack>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          this.m_heldCardVisual.SetCardBackId(component.GetCardBackId());
          CollectionDeckTray collectionDeckTray2 = CollectionDeckTray.Get();
          if (collectionDeckTray2 != null)
          {
            DeckTrayCardBackContent cardBackContent = collectionDeckTray2.GetCardBackContent();
            if (cardBackContent != null)
            {
              cardBackContent.ToggleSparkleEffects(true);
              break;
            }
            break;
          }
          break;
        }
        break;
    }
    return true;
  }

  public bool GrabCardTile(
    DeckTrayDeckTileVisual deckTileVisual,
    InputMgr.OnCardDroppedCallback callback,
    bool removeCard = true)
  {
    this.m_cardDroppedCallback = callback;
    return this.GrabCardTile(deckTileVisual, removeCard);
  }

  public bool GrabCardTile(DeckTrayDeckTileVisual deckTileVisual, bool removeCard = true)
  {
    Actor actor = (Actor) deckTileVisual.GetActor();
    CollectionDeckSlot slot = deckTileVisual.GetSlot();
    if (!this.StartDragWithActor(actor, CollectionUtils.ViewMode.CARDS, this.MouseIsOverDeck, slot))
      return false;
    if (DuelsConfig.IsCardLoadoutTreasure(this.m_heldCardVisual.GetCardID()))
      removeCard = false;
    if (removeCard)
    {
      CollectionDeck editingDeck = CollectionDeckTray.Get().GetCardsContent().GetEditingDeck();
      CollectionDeckTray.Get().RemoveCard(this.m_heldCardVisual.GetCardID(), slot.UnPreferredPremium, editingDeck.IsValidSlot(slot));
      if (!Options.Get().GetBool(Option.HAS_REMOVED_CARD_FROM_DECK, false))
      {
        CollectionDeckTray.Get().GetCardsContent().HideDeckHelpPopup();
        Options.Get().SetBool(Option.HAS_REMOVED_CARD_FROM_DECK, true);
      }
    }
    return true;
  }

  public bool GrabCardBackFromSlot(Actor actor, int cardBackId)
  {
    if (!this.StartDragWithActor(actor, CollectionUtils.ViewMode.CARD_BACKS))
      return false;
    this.m_heldCardVisual.SetCardBackId(cardBackId);
    return true;
  }

  public bool GrabHeroSkinFromSlot(Actor actor, int cardBackId) => this.StartDragWithActor(actor, CollectionUtils.ViewMode.HERO_SKINS);

  public override bool GrabMercenariesModeCard(
    IDataModel dataModel,
    CollectionUtils.MercenariesModeCardType cardType,
    InputMgr.OnCardDroppedCallback callback = null)
  {
    RaycastHit hitInfo;
    if (dataModel == null || !this.CanGrabMercenariesModeItem(cardType) || (UnityEngine.Object) this.m_mercenariesDraggablesWidget == (UnityEngine.Object) null || !UniversalInputManager.Get().GetInputHitInfo(Box.Get().GetCamera(), (LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
      return false;
    this.m_cardDroppedCallback = callback;
    this.m_mercenariesDraggablesWidget.BindDataModel(dataModel);
    if ((UnityEngine.Object) this.m_scrollBar != (UnityEngine.Object) null)
      this.m_scrollBar.Pause(true);
    PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
    string eventName1 = (string) null;
    string eventName2 = (string) null;
    switch (cardType)
    {
      case CollectionUtils.MercenariesModeCardType.Mercenary:
        eventName1 = "START_MERC_OVER_COLLECTION_code";
        eventName2 = "HOLD_MERC_OVER_TEAM_TRAY_code";
        break;
      case CollectionUtils.MercenariesModeCardType.Equipment:
        eventName1 = "HOLD_ABILITY_OVER_COLLECTION_code";
        eventName2 = "HOLD_ABILITY_OVER_TEAM_TRAY_code";
        break;
    }
    this.SetHeldMercenaryCard(dataModel, cardType);
    this.m_mercenariesDraggablesWidget.TriggerEvent(eventName1);
    this.DisableDraggableColliders();
    bool flag = CollectionDeckTray.Get().MouseIsOver(Box.Get().GetCamera());
    if (flag)
      this.m_mercenariesDraggablesWidget.TriggerEvent(eventName2);
    else
      this.m_mercenariesDraggablesWidget.TriggerEvent(eventName1);
    this.MouseIsOverDeck = flag;
    this.m_offScreenPosition = this.m_mercenariesDraggablesWidget.gameObject.transform.position;
    this.m_mercenariesDraggablesWidget.gameObject.transform.position = hitInfo.point;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.HideFilterTrayOnStartDragCard();
    return true;
  }

  public override void SetHeldMercenaryCard(
    IDataModel dataModel,
    CollectionUtils.MercenariesModeCardType cardType)
  {
    if (dataModel == null)
    {
      Log.Lettuce.PrintWarning("CollectionInputMgr.SetHeldMercenaryCard - input data model is not valid!");
    }
    else
    {
      this.m_heldType = cardType;
      switch (cardType)
      {
        case CollectionUtils.MercenariesModeCardType.Mercenary:
          if (!(dataModel is LettuceMercenaryDataModel mercenaryDataModel))
          {
            Log.Lettuce.PrintWarning("CollectionInputMgr.SetHeldMercenaryCard - mercenary data model is not valid!");
            break;
          }
          this.m_heldMercenariesModeCardId = CollectionManager.Get().GetMercenary((long) mercenaryDataModel.MercenaryId).GetCardId();
          break;
        case CollectionUtils.MercenariesModeCardType.Equipment:
          if (!(dataModel is LettuceAbilityDataModel abilityDataModel) || abilityDataModel.AbilityTiers == null)
          {
            Log.Lettuce.PrintWarning("CollectionInputMgr.SetHeldMercenaryCard - ability data model is not valid!");
            break;
          }
          LettuceAbilityTierDataModel abilityTier = abilityDataModel.AbilityTiers[abilityDataModel.CurrentTier - 1];
          if (abilityTier == null || abilityTier.AbilityTierCard == null)
          {
            Log.Lettuce.PrintWarning("CollectionInputMgr.SetHeldMercenaryCard - ability tier data model is not valid!");
            break;
          }
          this.m_heldMercenariesModeCardId = abilityTier.AbilityTierCard.CardId;
          break;
      }
    }
  }

  public bool GrabBattlegroundsEmote(
    IDataModel dataModel,
    CollectionUtils.BattlegroundsModeDraggableType bgType,
    InputMgr.OnCardDroppedCallback callback = null,
    Widget sourceWidget = null)
  {
    RaycastHit hitInfo;
    if (dataModel == null || bgType == CollectionUtils.BattlegroundsModeDraggableType.None || (UnityEngine.Object) this.m_battlegroundsDraggablesWidget == (UnityEngine.Object) null || !UniversalInputManager.Get().GetInputHitInfo(Box.Get().GetCamera(), (LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
      return false;
    this.m_cardDroppedCallback = callback;
    if (!this.SetHeldBattlegroundsEmote(dataModel, bgType))
      return false;
    this.m_battlegroundsDraggablesWidget.Hide();
    this.m_battlegroundsDraggablesWidget.BindDataModel(dataModel);
    this.m_battlegroundsDraggablesWidget.TriggerEvent("START_EMOTE_DRAG_code");
    this.m_battlegroundsDraggablesWidget.RegisterDoneChangingStatesListener((Action<object>) (_ =>
    {
      if (!(this.m_heldBattlegroundsEmoteCardId == (dataModel as BattlegroundsEmoteDataModel).EmoteDbiId.ToString()))
        return;
      if (bgType == CollectionUtils.BattlegroundsModeDraggableType.TrayEmote && (UnityEngine.Object) sourceWidget != (UnityEngine.Object) null)
        sourceWidget.Hide();
      this.m_battlegroundsDraggablesWidget.Show();
    }), (object) null, true, true);
    if ((UnityEngine.Object) this.m_scrollBar != (UnityEngine.Object) null)
      this.m_scrollBar.Pause(true);
    PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
    this.DisableBattlegroundsDraggableColliders();
    this.m_offScreenPosition = this.m_battlegroundsDraggablesWidget.gameObject.transform.position;
    this.m_battlegroundsDraggablesWidget.gameObject.transform.position = hitInfo.point;
    CollectionManagerDisplay collectibleDisplay1 = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay1 != (UnityEngine.Object) null)
      collectibleDisplay1.HideFilterTrayOnStartDragCard();
    BaconCollectionDisplay collectibleDisplay2 = CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay;
    if ((UnityEngine.Object) collectibleDisplay2 == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("Unable to access BaconCollectionDisplay");
      return false;
    }
    collectibleDisplay2.m_pageManager.EnableEmoteHoverHighlights(false);
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_pick_up_card.prefab:f7fb595cdc26f2f4997b4a10eaf1d0e1", this.m_battlegroundsDraggablesWidget.gameObject);
    return true;
  }

  public bool SetHeldBattlegroundsEmote(
    IDataModel dataModel,
    CollectionUtils.BattlegroundsModeDraggableType bgType)
  {
    if (!(dataModel is BattlegroundsEmoteDataModel battlegroundsEmoteDataModel))
    {
      Log.CollectionManager.PrintWarning("CollectionInputMgr.SetHeldBattlegroundsEmote - emote data model is not valid!");
      return false;
    }
    if (bgType == CollectionUtils.BattlegroundsModeDraggableType.CollectionEmote && this.m_battlegroundsEmoteTray.IsEmoteInLoadout(battlegroundsEmoteDataModel.EmoteDbiId))
      return false;
    this.m_bgHeldType = bgType;
    this.m_heldBattlegroundsEmoteCardId = battlegroundsEmoteDataModel.EmoteDbiId.ToString();
    return true;
  }

  public void SetScrollbar(UIBScrollable scrollbar) => this.m_scrollBar = scrollbar;

  public bool IsDraggingScrollbar() => (UnityEngine.Object) this.m_scrollBar != (UnityEngine.Object) null && this.m_scrollBar.IsDragging();

  public bool HasHeldCard() => (UnityEngine.Object) this.m_heldCardVisual != (UnityEngine.Object) null && this.m_heldCardVisual.IsShown() || (UnityEngine.Object) this.m_mercenariesDraggablesWidget != (UnityEngine.Object) null && this.m_heldType != CollectionUtils.MercenariesModeCardType.None;

  public bool HasHeldEmote() => (UnityEngine.Object) this.m_battlegroundsDraggablesWidget != (UnityEngine.Object) null && this.m_bgHeldType != CollectionUtils.BattlegroundsModeDraggableType.None;

  private bool CanGrabItem(Actor actor) => !this.IsDraggingScrollbar() && !((UnityEngine.Object) this.m_heldCardVisual == (UnityEngine.Object) null) && !this.m_heldCardVisual.IsShown() && !((UnityEngine.Object) actor == (UnityEngine.Object) null);

  protected override bool CanGrabMercenariesModeItem(
    CollectionUtils.MercenariesModeCardType itemType)
  {
    return !this.IsDraggingScrollbar() && this.m_heldType == CollectionUtils.MercenariesModeCardType.None;
  }

  protected override void UpdateHeldCardVisual()
  {
    RaycastHit hitInfo;
    if (!UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
      return;
    if ((UnityEngine.Object) this.m_heldCardVisual != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI)
    {
      foreach (Component componentsInChild in this.m_heldCardVisual.GetComponentsInChildren<Transform>())
        componentsInChild.gameObject.layer = 19;
    }
    Vector3 point = hitInfo.point;
    if ((bool) UniversalInputManager.UsePhoneUI)
      point.y += (float) InputMgr.PHONE_HEIGHT_OFFSET;
    this.m_heldCardVisual.transform.position = point;
    if ((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null)
    {
      this.MouseIsOverDeck = CollectionDeckTray.Get().MouseIsOver(Box.Get().GetCamera());
      this.m_heldCardVisual.UpdateVisual(this.MouseIsOverDeck);
    }
    if ((UnityEngine.Object) DraftPhoneDeckTray.Get() != (UnityEngine.Object) null)
    {
      this.MouseIsOverDeck = DraftPhoneDeckTray.Get().MouseIsOver();
      this.m_heldCardVisual.UpdateVisual(this.MouseIsOverDeck);
    }
    if (!InputCollection.GetMouseButtonUp(0))
      return;
    this.DropCard(false);
  }

  protected override void UpdateMercenariesHeldVisual(
    CollectionUtils.MercenariesModeCardType heldType)
  {
    string eventName = "";
    bool flag = false;
    switch (heldType)
    {
      case CollectionUtils.MercenariesModeCardType.Mercenary:
        if ((bool) (UnityEngine.Object) CollectionDeckTray.Get())
          flag = CollectionDeckTray.Get().MouseIsOver(Box.Get().GetCamera());
        if (flag && !this.MouseIsOverDeck)
        {
          eventName = "HOLD_MERC_OVER_TEAM_TRAY_code";
          break;
        }
        if (!flag && this.MouseIsOverDeck)
        {
          eventName = "HOLD_MERC_OVER_COLLECTION_code";
          break;
        }
        break;
      case CollectionUtils.MercenariesModeCardType.Equipment:
        flag = (CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay).GetMercenaryDetailsDisplay().IsMouseOverEquipmentSlot();
        if (flag && !this.MouseIsOverDeck)
        {
          eventName = "HOLD_ABILITY_OVER_TEAM_TRAY_code";
          break;
        }
        if (!flag && this.MouseIsOverDeck)
        {
          eventName = "HOLD_ABILITY_OVER_COLLECTION_code";
          break;
        }
        break;
    }
    if (!string.IsNullOrEmpty(eventName))
      this.m_mercenariesDraggablesWidget.TriggerEvent(eventName);
    this.MouseIsOverDeck = flag;
  }

  protected override void DropCard(bool dragCanceled)
  {
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    if ((UnityEngine.Object) this.m_heldCardVisual == (UnityEngine.Object) null)
      return;
    CollectionUtils.ViewMode visualType = this.m_heldCardVisual.GetVisualType();
    if (!dragCanceled)
    {
      if (this.MouseIsOverDeck)
      {
        switch (visualType)
        {
          case CollectionUtils.ViewMode.CARDS:
            if ((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null && !DuelsConfig.IsCardLoadoutTreasure(this.m_heldCardVisual.GetCardID()))
            {
              if (CollectionDeckTray.Get().AddCard(this.m_heldCardVisual.GetEntityDef(), this.m_heldCardVisual.GetPremium(), true, (Actor) null, DeckRule.RuleType.DEATHKNIGHT_RUNE_LIMIT))
              {
                CollectionDeckTray.Get().OnCardManuallyAddedByUser_CheckSuggestions(this.m_heldCardVisual.GetEntityDef());
                break;
              }
              break;
            }
            break;
          case CollectionUtils.ViewMode.HERO_SKINS:
            EntityDef entityDef = this.m_heldCardVisual.GetEntityDef();
            TAG_PREMIUM premium = this.m_heldCardVisual.GetPremium();
            if (entityDef != null)
            {
              CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
              if (collectionDeckTray != null)
              {
                DeckTrayHeroSkinContent heroSkinContent = collectionDeckTray.GetHeroSkinContent();
                if (heroSkinContent != null)
                {
                  heroSkinContent.UpdateHeroSkin(entityDef.GetCardId(), premium, true);
                  break;
                }
                break;
              }
              break;
            }
            break;
          case CollectionUtils.ViewMode.CARD_BACKS:
            int cardBackId = this.m_heldCardVisual.GetCardBackId();
            if (cardBackId != -1)
            {
              CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
              if (collectionDeckTray != null)
              {
                DeckTrayCardBackContent cardBackContent = collectionDeckTray.GetCardBackContent();
                if (cardBackContent != null)
                {
                  cardBackContent.UpdateCardBack(cardBackId, true);
                  break;
                }
                break;
              }
              break;
            }
            Debug.LogWarning((object) "Cardback ID not set for dragging card back.");
            break;
          case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
            Debug.LogWarning((object) "DropCard called in guide skins view mode. Should not be possible to pick up card in this mode.");
            break;
          case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
            Debug.LogWarning((object) "DropCard called in battlegrounds hero skins view mode. Should not be possible to pick up card in this mode.");
            break;
        }
      }
      else
      {
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_drop_card.prefab:8275e45efb8280347b35c2548e706d84", this.m_heldCardVisual.gameObject);
        switch (visualType)
        {
          case CollectionUtils.ViewMode.HERO_SKINS:
            CollectionDeckTray collectionDeckTray1 = CollectionDeckTray.Get();
            if (collectionDeckTray1 != null)
            {
              DeckTrayHeroSkinContent heroSkinContent = collectionDeckTray1.GetHeroSkinContent();
              if (heroSkinContent != null)
              {
                heroSkinContent.ToggleSparkleEffects(false);
                break;
              }
              break;
            }
            break;
          case CollectionUtils.ViewMode.CARD_BACKS:
            CollectionDeckTray collectionDeckTray2 = CollectionDeckTray.Get();
            if (collectionDeckTray2 != null)
            {
              DeckTrayCardBackContent cardBackContent = collectionDeckTray2.GetCardBackContent();
              if (cardBackContent != null)
              {
                cardBackContent.ToggleSparkleEffects(false);
                break;
              }
              break;
            }
            break;
        }
        if (this.m_cardDroppedCallback != null)
        {
          this.m_cardDroppedCallback();
          this.m_cardDroppedCallback = (InputMgr.OnCardDroppedCallback) null;
        }
      }
    }
    this.m_heldCardVisual.Hide();
    if ((UnityEngine.Object) this.m_scrollBar != (UnityEngine.Object) null)
      this.m_scrollBar.Pause(false);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
    {
      if (!dragCanceled && this.MouseIsOverDeck)
        collectibleDisplay.WaitThenUnhideFilterTrayOnStopDragCard();
      else
        collectibleDisplay.UnhideFilterTrayOnStopDragCard();
    }
    if (visualType != CollectionUtils.ViewMode.CARDS)
      return;
    Action draggableCardDropped = CollectionInputMgr.CollectionDraggableCardDropped;
    if (draggableCardDropped == null)
      return;
    draggableCardDropped();
  }

  public override void DropMercenariesModeCard(bool dragCanceled)
  {
    if (this.m_heldType == CollectionUtils.MercenariesModeCardType.None)
      return;
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    if ((UnityEngine.Object) this.m_mercenariesDraggablesWidget == (UnityEngine.Object) null)
      return;
    if (!dragCanceled)
    {
      if (this.MouseIsOverDeck)
      {
        if (this.m_heldType == CollectionUtils.MercenariesModeCardType.Mercenary)
        {
          if ((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null)
          {
            EntityDef entityDef = DefLoader.Get().GetEntityDef(this.m_heldMercenariesModeCardId);
            int index = -1;
            bool flag = false;
            DeckTrayMercListContent mercsContent = CollectionDeckTray.Get().GetMercsContent();
            if ((UnityEngine.Object) mercsContent != (UnityEngine.Object) null && (UnityEngine.Object) mercsContent.MercListable != (UnityEngine.Object) null && mercsContent.MercListable.WidgetItems != null)
            {
              foreach (WidgetInstance widgetItem in mercsContent.MercListable.WidgetItems)
              {
                ++index;
                GameObject gameObject = widgetItem.GetComponentInChildren<BoxCollider>(false).gameObject;
                if ((bool) (UnityEngine.Object) gameObject && UniversalInputManager.Get().ForcedUnblockableInputIsOver(Camera.main, gameObject.gameObject, out RaycastHit _))
                {
                  flag = true;
                  break;
                }
              }
            }
            if (!flag)
              index = -1;
            if (CollectionDeckTray.Get().AddCardToTeam(entityDef, true, index))
              CollectionDeckTray.Get().OnCardManuallyAddedByUser_CheckSuggestions(entityDef);
          }
        }
        else if (this.m_heldType == CollectionUtils.MercenariesModeCardType.Equipment)
        {
          LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
          if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
          {
            Log.Lettuce.PrintWarning("CollectionInputMgr.DropMercenariesModeCard - unable to find LettuceCollectionDisplay!");
            return;
          }
          collectibleDisplay.SlotEquipmentOnActiveMercenary(this.m_heldMercenariesModeCardId);
        }
      }
      else
      {
        this.m_mercenariesDraggablesWidget.TriggerEvent("END_MERC_OVER_COLLECTION_code");
        if (this.m_cardDroppedCallback != null)
        {
          this.m_cardDroppedCallback();
          this.m_cardDroppedCallback = (InputMgr.OnCardDroppedCallback) null;
        }
      }
    }
    this.m_mercenariesDraggablesWidget.gameObject.transform.position = this.m_offScreenPosition;
    this.m_heldMercenariesModeCardId = string.Empty;
    this.m_heldType = CollectionUtils.MercenariesModeCardType.None;
    if ((UnityEngine.Object) this.m_scrollBar != (UnityEngine.Object) null)
      this.m_scrollBar.Pause(false);
    CollectionManagerDisplay collectibleDisplay1 = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectibleDisplay1 != (UnityEngine.Object) null))
      return;
    if (!dragCanceled && this.MouseIsOverDeck)
      collectibleDisplay1.WaitThenUnhideFilterTrayOnStopDragCard();
    else
      collectibleDisplay1.UnhideFilterTrayOnStopDragCard();
  }

  public override void DropBattlegroundsEmote(bool dragCanceled, bool trayDropCanceled = false)
  {
    if (this.m_bgHeldType == CollectionUtils.BattlegroundsModeDraggableType.None)
      return;
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    if ((UnityEngine.Object) this.m_battlegroundsDraggablesWidget == (UnityEngine.Object) null)
      return;
    this.m_battlegroundsDraggablesWidget.TriggerEvent("END_EMOTE_DRAG_code");
    this.m_battlegroundsDraggablesWidget.gameObject.transform.position = this.m_offScreenPosition;
    if (!dragCanceled)
    {
      BattlegroundsEmoteDataModel dataModel = this.m_battlegroundsDraggablesWidget.GetDataModel<BattlegroundsEmoteDataModel>();
      if (this.m_battlegroundsEmoteTray.IsEmoteOverTray() && !trayDropCanceled)
        this.m_battlegroundsEmoteTray.DropOverEmoteTray(dataModel);
      else if (this.m_bgHeldType == CollectionUtils.BattlegroundsModeDraggableType.TrayEmote)
        this.m_battlegroundsEmoteTray.RemoveEmote(dataModel);
      else if (this.m_cardDroppedCallback != null)
      {
        this.m_cardDroppedCallback();
        this.m_cardDroppedCallback = (InputMgr.OnCardDroppedCallback) null;
      }
    }
    this.m_heldBattlegroundsEmoteCardId = string.Empty;
    this.m_bgHeldType = CollectionUtils.BattlegroundsModeDraggableType.None;
    if ((UnityEngine.Object) this.m_scrollBar != (UnityEngine.Object) null)
      this.m_scrollBar.Pause(false);
    BaconCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay;
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("Unable to access BaconCollectionDisplay");
    }
    else
    {
      collectibleDisplay.m_pageManager.EnableEmoteHoverHighlights(true);
      SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_drop_card.prefab:8275e45efb8280347b35c2548e706d84", this.m_battlegroundsDraggablesWidget.gameObject);
      this.m_battlegroundsEmoteTray.UpdateTrayHighlight(false);
    }
  }

  protected override void OnMouseOnOrOffScreen(bool onScreen)
  {
    if ((UnityEngine.Object) this.m_heldCardVisual == (UnityEngine.Object) null || (UnityEngine.Object) this.m_heldCardVisual.gameObject == (UnityEngine.Object) null)
      return;
    if (onScreen)
    {
      if (!this.m_heldCardOffscreen)
        return;
      this.m_heldCardOffscreen = false;
      if (InputCollection.GetMouseButton(0))
        this.m_heldCardVisual.Show(this.MouseIsOverDeck);
      else
        this.DropCard(true);
    }
    else
    {
      if (!this.m_heldCardVisual.IsShown())
        return;
      this.m_heldCardVisual.Hide();
      this.m_heldCardOffscreen = true;
    }
  }

  private void HandleDeckCopying()
  {
    if (!((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null) || !InputCollection.GetKey(KeyCode.LeftCommand) && !InputCollection.GetKey(KeyCode.LeftControl) && !InputCollection.GetKey(KeyCode.RightCommand) && !InputCollection.GetKey(KeyCode.RightControl))
      return;
    bool flag1 = CollectionDeckTray.Get().IsShowingDeckContents();
    if (InputCollection.GetKeyDown(KeyCode.C) & flag1)
    {
      CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
      if (editedDeck != null && (UnityEngine.Object) UIStatus.Get() != (UnityEngine.Object) null)
      {
        DeckRuleViolation topViolation = (DeckRuleViolation) null;
        bool flag2 = false;
        if (SceneMgr.Get().IsInDuelsMode())
        {
          if ((UnityEngine.Object) AdventureDungeonCrawlDisplay.Get() != (UnityEngine.Object) null)
            flag2 = AdventureDungeonCrawlDisplay.Get().IsDuelsDeckValid();
        }
        else
          flag2 = editedDeck.CanCopyAsShareableDeck(out topViolation);
        if (topViolation != null)
        {
          string deckRuleViolation = CollectionDeck.GetUserFriendlyCopyErrorMessageFromDeckRuleViolation(topViolation);
          if (!string.IsNullOrEmpty(deckRuleViolation))
            UIStatus.Get().AddInfo(deckRuleViolation);
        }
        if (flag2)
        {
          ClipboardUtils.CopyToClipboard(editedDeck.GetShareableDeck().Serialize());
          UIStatus.Get().AddInfo(GameStrings.Get("GLUE_COLLECTION_DECK_COPIED_TOAST"));
        }
      }
    }
    if (!InputCollection.GetKeyDown(KeyCode.V))
      return;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null) || !((UnityEngine.Object) DialogManager.Get() != (UnityEngine.Object) null))
      return;
    int num = DialogManager.Get().ShowingDialog() ? 1 : 0;
    bool flag3 = collectibleDisplay.IsSelectingNewDeckHero();
    if (num != 0 || flag1 || flag3)
      return;
    collectibleDisplay.PasteFromClipboardIfValidOrShowStatusMessage();
  }

  private void HandleMercTeamCopying()
  {
    CollectionDeckTray.Get().GetTeamsContent();
    if ((InputCollection.GetKey(KeyCode.LeftCommand) || InputCollection.GetKey(KeyCode.LeftControl) || InputCollection.GetKey(KeyCode.RightCommand) ? 1 : (InputCollection.GetKey(KeyCode.RightControl) ? 1 : 0)) == 0)
      return;
    if (CollectionDeckTray.Get().IsShowingTeamContents() && InputCollection.GetKeyDown(KeyCode.C))
    {
      LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
      if (editingTeam != null && (UnityEngine.Object) UIStatus.Get() != (UnityEngine.Object) null)
      {
        ClipboardUtils.CopyToClipboard(new ShareableMercenariesTeam(editingTeam).Serialize(true));
        UIStatus.Get().AddInfo(GameStrings.Get("GLUE_COLLECTION_DECK_COPIED_TOAST"));
      }
    }
    if (!InputCollection.GetKeyDown(KeyCode.V))
      return;
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    DialogManager dialogManager = DialogManager.Get();
    if (!((UnityEngine.Object) dialogManager != (UnityEngine.Object) null) || dialogManager.ShowingDialog())
      return;
    collectibleDisplay.TeamCopying.CheckClipboardAndPromptPlayerToPaste();
  }
}
