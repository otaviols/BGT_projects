using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLettuce;
using System;

public class CollectionMercenaryDetailDisplay : MercenaryDetailDisplay
{
  protected override void Start()
  {
    this.m_mercDetailsDisplayVisualController = this.gameObject.GetComponent<VisualController>();
    this.m_mercDetailsDisplayVisualController.GetComponent<Widget>().RegisterEventListener(new Widget.EventListenerDelegate(((MercenaryDetailDisplay) this).MercDetailsEventListener));
    this.m_abilityUpgradePopupReference.RegisterReadyListener<VisualController>(new Action<VisualController>(((MercenaryDetailDisplay) this).OnAbilityUpgradePopupReady));
    this.m_abilityInfoPopupReference.RegisterReadyListener<Widget>((Action<Widget>) (w => this.m_abilityInfoPopupWidget = w));
    this.m_mercPromotionPopupReference.RegisterReadyListener<VisualController>(new Action<VisualController>(((MercenaryDetailDisplay) this).OnMercPromotionPopupReady));
    this.m_equipmentCraftingPopupReference.RegisterReadyListener<VisualController>(new Action<VisualController>(((MercenaryDetailDisplay) this).OnEquipmentCraftingPopupReady));
    this.m_popupHandlerReference.RegisterReadyListener<VisualController>((Action<VisualController>) (vc => this.m_popupHandlerVisualController = vc));
    this.m_equipmentExplanationPopupReference.RegisterReadyListener<Widget>(new Action<Widget>(((MercenaryDetailDisplay) this).OnEquipmentExplanationPopupReady));
    this.m_abilityUpgradeCardReference.RegisterReadyListener<Hearthstone.UI.Card>((Action<Hearthstone.UI.Card>) (card => this.m_abilityUpgradeCard = card));
    Network network = Network.Get();
    network.RegisterNetHandler((object) UpgradeMercenaryAbilityResponse.PacketID.ID, new Network.NetHandler(((MercenaryDetailDisplay) this).OnAbilityUpgradeNetworkResponse));
    network.RegisterNetHandler((object) UpgradeMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(((MercenaryDetailDisplay) this).OnEquipmentUpgradeNetworkResponse));
    network.RegisterNetHandler((object) CraftMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(((MercenaryDetailDisplay) this).OnCraftEquipmentNetworkResponse));
  }

  public override void Unload()
  {
    Network network = Network.Get();
    if (network == null)
      return;
    network.RemoveNetHandler((object) UpgradeMercenaryAbilityResponse.PacketID.ID, new Network.NetHandler(((MercenaryDetailDisplay) this).OnAbilityUpgradeNetworkResponse));
    network.RemoveNetHandler((object) UpgradeMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(((MercenaryDetailDisplay) this).OnEquipmentUpgradeNetworkResponse));
    network.RemoveNetHandler((object) CraftMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(((MercenaryDetailDisplay) this).OnCraftEquipmentNetworkResponse));
  }

  public override void Show(LettuceMercenary merc, string showEvent = "SHOW_FULL", LettuceTeam editingTeam = null)
  {
    if ((UnityEngine.Object) this.m_mercDetailsDisplayVisualController == (UnityEngine.Object) null || merc == null)
      return;
    if ((UnityEngine.Object) (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as LettuceCollectionPageManager) == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintWarning("MercenaryDetailDisplay.Show - no LettuceCollectionPageManager found!");
    }
    else
    {
      if (editingTeam == null)
        editingTeam = CollectionManager.Get().GetEditingTeam();
      if (editingTeam != null && editingTeam.IsMercInTeam(merc.ID))
      {
        this.m_currentTeam = editingTeam;
        showEvent = "SHOW_PARTIAL";
        if (this.m_mercIdBeingViewed == -1)
          Navigation.Push(new Navigation.NavigateBackHandler(((DeckTray) CollectionDeckTray.Get()).OnBackOutOfContainerContents));
        CollectionDeckTray.Get().GetMercsContent().ChangeCurrentlySelectedMercenary(merc.ID, true);
      }
      this.m_equipmentSlotCollider.SetActive(false);
      this.m_mercIdBeingViewed = merc.ID;
      this.SetupActiveMercDataModel(this.GetMercenaryDisplayDataModel(), merc);
      this.ShowRequiredTutorialIfNeeded();
      if (!string.IsNullOrEmpty(showEvent))
        this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent(showEvent, new Widget.TriggerEventParameters()
        {
          IgnorePlaymaker = true,
          NoDownwardPropagation = true
        });
      CollectiblePageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager();
      if (!((UnityEngine.Object) pageManager != (UnityEngine.Object) null))
        return;
      pageManager.EnablePageTurn(false);
      pageManager.EnablePageTurnArrows(false);
    }
  }

  public override void Hide()
  {
    this.AcknowledgeAbilityorEquipment(0, true);
    this.SendAcknowledgements();
    CollectionManager.Get().TriggerNewCardSeenListeners();
    this.m_currentTeam = (LettuceTeam) null;
    if ((UnityEngine.Object) this.m_mercDetailsDisplayVisualController == (UnityEngine.Object) null)
      return;
    string eventName = "HIDE_FULL";
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam != null && editingTeam.IsMercInTeam(this.m_mercIdBeingViewed))
    {
      eventName = "HIDE_PARTIAL";
      CollectionDeckTray.Get().GetMercsContent().ChangeCurrentlySelectedMercenary(this.m_mercIdBeingViewed, false);
    }
    this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent(eventName, new Widget.TriggerEventParameters()
    {
      IgnorePlaymaker = true,
      NoDownwardPropagation = true
    });
    this.m_mercIdBeingViewed = -1;
    this.HideHelpPopups();
    CollectiblePageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager();
    if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
    {
      pageManager.EnablePageTurn(true);
      pageManager.EnablePageTurnArrows(true);
    }
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.OnReturnFromMercenaryDetailsDisplay();
  }

  protected override void OnEquipmentLoadoutDragStart()
  {
    this.HideHoverCards();
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("No event data model attached to the LettuceAbilitySlot");
    }
    else
    {
      LettuceAbilityDataModel payload = (LettuceAbilityDataModel) eventDataModel.Payload;
      if (payload == null)
        return;
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) this.GetMercenaryDisplayDataModel().MercenaryId);
      if (mercenary == null)
        return;
      LettuceAbility lettuceEquipment = mercenary.GetLettuceEquipment(payload.AbilityId);
      if (!this.CanPickUpAbility(mercenary, lettuceEquipment))
        return;
      this.m_equipmentSlotCollider.SetActive(true);
      CollectionInputMgr.Get().GrabMercenariesModeCard((IDataModel) payload, lettuceEquipment.m_cardType, new InputMgr.OnCardDroppedCallback(((MercenaryDetailDisplay) this).OnEquipmentDropped));
      if (mercenary.CanUnslotEquipment(payload.AbilityId))
        this.m_draggingEquipmentDataModel = payload;
      CollectionDeckTray.Get().GetMercsContent().UpdateMercList();
      this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent("START_DRAG_EQUIPMENT", new Widget.TriggerEventParameters());
    }
  }

  protected override void OnAbilityUpgradeNetworkResponse()
  {
    UpgradeMercenaryAbilityResponse mercenaryAbilityResponse = Network.Get().UpgradeMercenaryAbilityResponse();
    if (mercenaryAbilityResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.Lettuce.PrintError("MercenaryDetailDisplay.OnAbilityUpgradeNetworkResponse() - Error upgrading ability: {0} for ability {1} on mercenary {2}", (object) mercenaryAbilityResponse.ErrorCode, (object) mercenaryAbilityResponse.AbilityId, (object) mercenaryAbilityResponse.MercenaryId);
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenaryAbilityResponse.MercenaryId);
      if (mercenary == null)
        return;
      LettuceAbility lettuceAbility = mercenary.GetLettuceAbility(mercenaryAbilityResponse.AbilityId);
      if (lettuceAbility == null)
        return;
      this.UpdateDataModelsAfterTransaction(lettuceAbility, mercenary);
      if (LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_UPGRADE_ABILITY_END))
        return;
      EventFunctions.TriggerEvent(this.transform, "SHOW_TUTORIAL_DONE_BUTTON_HIGHLIGHT");
      CollectionDeckTray.Get().HighlightBackButton();
      LettuceTutorialUtils.FireEvent(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_UPGRADE_ABILITY_END, this.gameObject);
    }
  }

  protected override LettuceMercenaryDataModel UpdateDataModelsAfterTransaction(
    LettuceAbility ability,
    LettuceMercenary merc)
  {
    LettuceMercenaryDataModel mercenaryDataModel1 = base.UpdateDataModelsAfterTransaction(ability, merc);
    if (mercenaryDataModel1 == null)
    {
      Log.Lettuce.PrintWarning("MercenaryDetailDisplay.UpdateDataModelsAfterTransaction - Unable to get Mercenary Data Model!");
      return (LettuceMercenaryDataModel) null;
    }
    LettuceCollectionPageManager pageManager = CollectionManager.Get()?.GetCollectibleDisplay()?.GetPageManager() as LettuceCollectionPageManager;
    if ((UnityEngine.Object) pageManager == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintWarning("MercenaryDetailDisplay.UpdateDataModelsAfterTransaction - Unable to retrieve LettuceCollectionPageManager!");
    }
    else
    {
      LettuceMercenaryDataModel mercenaryOnPage = pageManager.GetMercenaryOnPage(mercenaryDataModel1.MercenaryId);
      if (mercenaryOnPage != null)
        mercenaryOnPage.ChildUpgradeAvailable = mercenaryDataModel1.ChildUpgradeAvailable;
    }
    LettuceMercenaryDataModel mercenaryDataModel2 = CollectionDeckTray.Get().GetMercsContent().GetMercenaryDataModel(merc.ID);
    if (mercenaryDataModel2 != null)
      mercenaryDataModel2.ChildUpgradeAvailable = mercenaryDataModel1.ChildUpgradeAvailable;
    return mercenaryDataModel1;
  }

  protected override void MercDetailsEventListener(string eventName)
  {
    switch (eventName)
    {
      case "ABILITY_CLICKED_code":
        this.OnAbilityClicked();
        break;
      case "ABILITY_HOVERED_code":
        this.AcknowledgeAbilityorEquipment((WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController).Payload as LettuceAbilityDataModel).AbilityId);
        break;
      case "ABILITY_INFO_POPUP_REVEAL_COMPLETED_code":
        this.ShowExplanationPopup();
        break;
      case "ABILITY_UPGRADE_code":
        this.OnUpgradeAbility();
        break;
      case "ABILITY_drag_started":
        this.OnAbilityDragStart();
        break;
      case "APPEARANCE_HIDE_code":
        this.AcknowledgeArtVariation(0, TAG_PREMIUM.NORMAL, true);
        this.HideHelpPopups();
        break;
      case "APPEARANCE_SELECTED_code":
        this.OnAppearanceClicked();
        break;
      case "APPEARANCE_SHOW_code":
        this.ShowAppearancePart2TutorialIfNeeded();
        break;
      case "ART_VARIATION_HOVERED_code":
        if (!(WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController).Payload is LettuceMercenaryArtVariationDataModel payload))
          break;
        this.AcknowledgeArtVariation(payload.ArtVariationId, payload.Card.Premium);
        break;
      case "ART_VARIATION_NEXT_code":
        this.OnAppearanceArrowClicked(1);
        break;
      case "ART_VARIATION_PREV_code":
        this.OnAppearanceArrowClicked(-1);
        break;
      case "DEBUG_UNLOCK_EQUIPMENT_code":
        this.OnCraftEquipment();
        break;
      case "LOADOUT_ABILITY_drag_started":
        this.OnEquipmentLoadoutDragStart();
        break;
      case "MERCENARY_released":
        this.OnAppearanceResetPage();
        break;
      case "MERC_COIN_OUT":
        this.HideCoinTooltip();
        break;
      case "MERC_COIN_OVER":
        this.ShowCoinTooltip();
        break;
      case "OnPress":
        this.Hide();
        break;
      case "POPUP_ACTIVATED_code":
        this.HideHelpPopups();
        break;
      case "POPUP_DEACTIVATED_code":
        if (this.ShowFullyUpgradeMercIfNeeded())
          break;
        this.ShowRequiredTutorialIfNeeded();
        break;
      default:
        LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
        if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
        {
          Log.Lettuce.PrintWarning("MercenaryDetailDisplay.MercDetailsEventListener - LettuceCollectionDisplay is null!");
          break;
        }
        collectibleDisplay.HandleTileHoverEvents(eventName, this.m_mercDetailsDisplayVisualController);
        break;
    }
  }

  public override bool ShowFullyUpgradeMercIfNeeded() => PopupDisplayManager.Get().RewardPopups.ShowMercenariesFullyUpgraded((Action) (() =>
  {
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) displayDataModel.MercenaryId);
    this.SetupActiveMercDataModel(displayDataModel, mercenary);
    LettuceCollectionPageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as LettuceCollectionPageManager;
    if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
      pageManager.UpdatePageMercenary(MercenaryFactory.CreateMercenaryDataModelWithCoin(mercenary));
    this.ShowRequiredTutorialIfNeeded();
  }));

  protected override bool CanPickUpAbility(LettuceMercenary merc, LettuceAbility ability) => CollectionManager.Get().GetCollectibleDisplay().GetPageManager().ArePagesTurning() || base.CanPickUpAbility(merc, ability);
}
