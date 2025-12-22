using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLettuce;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryDetailDisplay : MonoBehaviour
{
  public const string SHOW_EVENT_FULL = "SHOW_FULL";
  public const string SHOW_EVENT_PARTIAL = "SHOW_PARTIAL";
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_abilityUpgradePopupReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_abilityInfoPopupReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_mercPromotionPopupReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_equipmentCraftingPopupReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_popupHandlerReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_equipmentExplanationPopupReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_abilityUpgradeCardReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_mercenariesListReference;
  [CustomEditField(Sections = "Objects")]
  public TooltipZone m_tooltipZone;
  [CustomEditField(Sections = "Objects")]
  public GameObject m_equipmentSlotCollider;
  [CustomEditField(Sections = "Bones")]
  public List<Transform> m_showLoadEquipmentInSlotTutorialBones;
  [CustomEditField(Sections = "Bones")]
  public Transform m_upgradeAbilityTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_AppearanceMercTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_AppearanceTutorialBone;
  [CustomEditField(Sections = "Settings")]
  public float m_secondsDelayBeforeTutorialPopups = 1f;
  protected VisualController m_mercDetailsDisplayVisualController;
  protected VisualController m_abilityUpgradePopupVisualController;
  protected Widget m_abilityInfoPopupWidget;
  protected Widget m_equipmentExplanationPopupWidget;
  protected VisualController m_mercPromotionPopupVisualController;
  protected VisualController m_equipmentCraftingPopupVisualController;
  protected VisualController m_popupHandlerVisualController;
  protected Hearthstone.UI.Card m_abilityUpgradeCard;
  protected MaterialDataModel m_abilityMaterialData = new MaterialDataModel();
  protected VisualController m_mercenaryListVisualController;
  protected Notification m_helpPopup;
  protected Option m_helpPopupType;
  protected LettuceAbilityDataModel m_currentlyDisplayedAbility;
  protected LettuceAbilityDataModel m_draggingEquipmentDataModel;
  protected List<MercenaryAcknowledgeData> m_mercenaryAcknowledgements = new List<MercenaryAcknowledgeData>();
  protected LettuceTeam m_currentTeam;
  protected int m_mercIdBeingViewed = -1;
  protected const string STOP_DRAG_EQUIPMENT_EVENT = "STOP_DRAG_EQUIPMENT";
  protected const string STOP_DRAG_EQUIPMENT_SLOTTED_EVENT = "STOP_DRAG_EQUIPMENT_SLOTTED";
  protected const string START_DRAG_EQUIPMENT_EVENT = "START_DRAG_EQUIPMENT";
  protected const float TUTORIAL_PULSE_RATE = 3f;
  protected Coroutine m_tutorialCoroutine;
  private readonly List<MercenaryDetailDisplay.OnHideDelegate> m_onHideCallbacks = new List<MercenaryDetailDisplay.OnHideDelegate>();

  protected virtual void Start()
  {
    this.m_mercDetailsDisplayVisualController = this.gameObject.GetComponent<VisualController>();
    this.m_mercDetailsDisplayVisualController.GetComponent<Widget>().RegisterEventListener(new Widget.EventListenerDelegate(this.MercDetailsEventListener));
    this.m_abilityUpgradePopupReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnAbilityUpgradePopupReady));
    this.m_abilityInfoPopupReference.RegisterReadyListener<Widget>((Action<Widget>) (w => this.m_abilityInfoPopupWidget = w));
    this.m_mercPromotionPopupReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnMercPromotionPopupReady));
    this.m_equipmentCraftingPopupReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnEquipmentCraftingPopupReady));
    this.m_popupHandlerReference.RegisterReadyListener<VisualController>((Action<VisualController>) (vc => this.m_popupHandlerVisualController = vc));
    this.m_equipmentExplanationPopupReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnEquipmentExplanationPopupReady));
    this.m_abilityUpgradeCardReference.RegisterReadyListener<Hearthstone.UI.Card>((Action<Hearthstone.UI.Card>) (card => this.m_abilityUpgradeCard = card));
    this.m_mercenariesListReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnMercenaryListReady));
    Network network = Network.Get();
    network.RegisterNetHandler((object) UpgradeMercenaryAbilityResponse.PacketID.ID, new Network.NetHandler(this.OnAbilityUpgradeNetworkResponse));
    network.RegisterNetHandler((object) UpgradeMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(this.OnEquipmentUpgradeNetworkResponse));
    network.RegisterNetHandler((object) CraftMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(this.OnCraftEquipmentNetworkResponse));
    InputMgr.Get().OnDropMercenariesModeCard += new Action<CollectionUtils.MercenariesModeCardType, string>(this.OnDropMercenariesModeCard);
    CollectionManager.Get().MercenaryArtVariationChangedEvent += new Action<int, int, TAG_PREMIUM>(this.OnMercenaryArtVariationChanged);
  }

  public virtual void Unload()
  {
    InputMgr inputMgr = InputMgr.Get();
    if ((UnityEngine.Object) inputMgr != (UnityEngine.Object) null)
      inputMgr.OnDropMercenariesModeCard -= new Action<CollectionUtils.MercenariesModeCardType, string>(this.OnDropMercenariesModeCard);
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager != null)
      collectionManager.MercenaryArtVariationChangedEvent -= new Action<int, int, TAG_PREMIUM>(this.OnMercenaryArtVariationChanged);
    Network network = Network.Get();
    if (network == null)
      return;
    network.RemoveNetHandler((object) UpgradeMercenaryAbilityResponse.PacketID.ID, new Network.NetHandler(this.OnAbilityUpgradeNetworkResponse));
    network.RemoveNetHandler((object) UpgradeMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(this.OnEquipmentUpgradeNetworkResponse));
    network.RemoveNetHandler((object) CraftMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(this.OnCraftEquipmentNetworkResponse));
    Network.Get().RemoveNetHandler((object) MercenariesCollectionAcknowledgeResponse.PacketID.ID, new Network.NetHandler(this.OnCollectionAcknowledgeResponse));
  }

  public void OnDestroy()
  {
    this.Unload();
    this.HideHelpPopups();
  }

  public virtual void Show(LettuceMercenary merc, string showEvent = "SHOW_FULL", LettuceTeam editingTeam = null)
  {
    if ((UnityEngine.Object) this.m_mercDetailsDisplayVisualController == (UnityEngine.Object) null || merc == null || this.m_mercIdBeingViewed == merc.ID)
      return;
    this.m_equipmentSlotCollider.SetActive(false);
    if (editingTeam != null)
    {
      this.m_currentTeam = editingTeam;
      CollectionManager.Get().SetEditingTeam(editingTeam);
    }
    this.m_mercIdBeingViewed = merc.ID;
    this.SetupActiveMercDataModel(this.GetMercenaryDisplayDataModel(), merc);
    this.ShowRequiredTutorialIfNeeded();
    this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent(showEvent, new Widget.TriggerEventParameters()
    {
      IgnorePlaymaker = true,
      NoDownwardPropagation = true
    });
    MercenaryInputMgr.Get().MouseOverTargetEvaluator = new Func<bool>(this.IsMouseOverEquipmentSlot);
  }

  public virtual void Hide()
  {
    this.AcknowledgeAbilityorEquipment(0, true);
    this.SendAcknowledgements();
    MercenaryInputMgr.Get().MouseOverTargetEvaluator = (Func<bool>) null;
    this.OnHide();
    if (this.m_currentTeam != null)
    {
      this.m_currentTeam = (LettuceTeam) null;
      CollectionManager.Get().ClearEditingTeam();
    }
    if ((UnityEngine.Object) this.m_mercDetailsDisplayVisualController == (UnityEngine.Object) null)
      return;
    this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent("HIDE_FULL", new Widget.TriggerEventParameters()
    {
      IgnorePlaymaker = true,
      NoDownwardPropagation = true
    });
    this.m_mercIdBeingViewed = -1;
    this.HideHelpPopups();
  }

  public bool DisplayVisible => this.m_mercIdBeingViewed != -1;

  public void HideHelpPopups()
  {
    if (this.m_tutorialCoroutine != null)
    {
      this.StopCoroutine(this.m_tutorialCoroutine);
      this.m_tutorialCoroutine = (Coroutine) null;
    }
    if ((UnityEngine.Object) this.m_helpPopup != (UnityEngine.Object) null)
      NotificationManager.Get()?.DestroyNotificationNowWithNoAnim(this.m_helpPopup);
    this.m_helpPopupType = Option.INVALID;
  }

  public void UpdateMercenaryData(LettuceMercenary merc)
  {
    LettuceMercenary displayedMercenary = this.GetCurrentlyDisplayedMercenary();
    if (merc == null || displayedMercenary == null || displayedMercenary.ID != merc.ID)
      return;
    this.SetupActiveMercDataModel(this.GetMercenaryDisplayDataModel(), merc);
  }

  protected void OnAbilityUpgradePopupReady(VisualController visualController)
  {
    this.m_abilityUpgradePopupVisualController = visualController;
    this.m_abilityUpgradePopupVisualController.BindDataModel((IDataModel) this.m_abilityMaterialData);
  }

  protected void OnMercPromotionPopupReady(VisualController visualController) => this.m_mercPromotionPopupVisualController = visualController;

  protected void OnEquipmentCraftingPopupReady(VisualController visualController) => this.m_equipmentCraftingPopupVisualController = visualController;

  protected void OnEquipmentExplanationPopupReady(Widget widget) => this.m_equipmentExplanationPopupWidget = widget;

  protected void OnMercenaryListReady(VisualController visualController)
  {
    this.m_mercenaryListVisualController = visualController;
    this.m_mercenaryListVisualController.SetState("SHOW");
  }

  protected virtual void MercDetailsEventListener(string eventName)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(eventName))
    {
      case 203562326:
        if (!(eventName == "ABILITY_UPGRADE_code"))
          break;
        this.OnUpgradeAbility();
        break;
      case 244308127:
        if (!(eventName == "MERCENARY_released"))
          break;
        this.OnAppearanceResetPage();
        break;
      case 367365572:
        if (!(eventName == "ART_VARIATION_HOVERED_code"))
          break;
        LettuceMercenaryArtVariationDataModel payload1 = WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController).Payload as LettuceMercenaryArtVariationDataModel;
        this.AcknowledgeArtVariation(payload1.ArtVariationId, payload1.Card.Premium);
        break;
      case 815448185:
        if (!(eventName == "ABILITY_INFO_POPUP_REVEAL_COMPLETED_code"))
          break;
        this.ShowExplanationPopup();
        break;
      case 955402008:
        if (!(eventName == "ART_VARIATION_NEXT_code"))
          break;
        this.OnAppearanceArrowClicked(1);
        break;
      case 1509391998:
        if (!(eventName == "BACK_BUTTON_PRESSED"))
          break;
        this.Hide();
        break;
      case 1735098527:
        if (!(eventName == "POPUP_ACTIVATED_code"))
          break;
        this.HideHelpPopups();
        break;
      case 1817687672:
        if (!(eventName == "ART_VARIATION_PREV_code"))
          break;
        this.OnAppearanceArrowClicked(-1);
        break;
      case 1835464463:
        if (!(eventName == "APPEARANCE_SELECTED_code"))
          break;
        this.OnAppearanceClicked();
        break;
      case 2363403039:
        if (!(eventName == "MERC_COIN_OUT"))
          break;
        this.HideCoinTooltip();
        break;
      case 2760077547:
        if (!(eventName == "ABILITY_HOVERED_code"))
          break;
        this.AcknowledgeAbilityorEquipment((WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController).Payload as LettuceAbilityDataModel).AbilityId);
        break;
      case 3295263903:
        if (!(eventName == "LOADOUT_ABILITY_drag_started"))
          break;
        this.OnEquipmentLoadoutDragStart();
        break;
      case 3296195180:
        if (!(eventName == "POPUP_DEACTIVATED_code") || this.ShowFullyUpgradeMercIfNeeded())
          break;
        this.ShowRequiredTutorialIfNeeded();
        break;
      case 3298161644:
        if (!(eventName == "APPEARANCE_HIDE_code"))
          break;
        this.AcknowledgeArtVariation(0, TAG_PREMIUM.NORMAL, true);
        this.HideHelpPopups();
        break;
      case 3337835730:
        if (!(eventName == "DEBUG_UNLOCK_EQUIPMENT_code"))
          break;
        this.OnCraftEquipment();
        break;
      case 3515474770:
        if (!(eventName == "ABILITY_drag_started"))
          break;
        this.OnAbilityDragStart();
        break;
      case 3586013055:
        if (!(eventName == "APPEARANCE_SHOW_code"))
          break;
        this.ShowAppearancePart2TutorialIfNeeded();
        break;
      case 3868469129:
        if (!(eventName == "ABILITY_CLICKED_code"))
          break;
        this.OnAbilityClicked();
        break;
      case 3935194620:
        if (!(eventName == "OnUninspectMerc"))
          break;
        this.Hide();
        break;
      case 4148893411:
        if (!(eventName == "MERC_LOADOUT_RELEASED") || !(WidgetUtils.GetEventDataModel(this.m_mercenaryListVisualController).Payload is LettuceMercenaryDataModel payload2))
          break;
        this.Show(CollectionManager.Get().GetMercenary((long) payload2.MercenaryId), "SHOW_PARTIAL", this.m_currentTeam);
        break;
      case 4156600111:
        if (!(eventName == "MERC_COIN_OVER"))
          break;
        this.ShowCoinTooltip();
        break;
    }
  }

  protected void HideHoverCards()
  {
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
      Log.Lettuce.PrintWarning("MercenaryDetailDisplay.HideHoverCards - LettuceCollectionDisplay is null!");
    else
      collectibleDisplay.HideHoverCards();
  }

  public void SlotSelectedEquipment(string cardId)
  {
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) displayDataModel.MercenaryId);
    if (mercenary == null)
      return;
    LettuceAbility lettuceEquipment = mercenary.GetLettuceEquipment(cardId);
    if (lettuceEquipment == null)
    {
      Log.Lettuce.PrintError("MercenaryDetailDisplay.SlotSelectedEquipment - equipment with card ID {0} not found on Mercenary ID {1}", (object) cardId, (object) mercenary.ID);
    }
    else
    {
      mercenary.SlotEquipment(lettuceEquipment.ID);
      if (mercenary.m_equipmentSelectionChanged)
        CollectionManager.Get().SendEquippedMercenaryEquipment(mercenary.ID);
      this.m_equipmentSlotCollider.SetActive(false);
      if (this.m_helpPopupType == Option.HAS_SEEN_LOAD_EQUIPMENT_IN_SLOT_TUTORIAL)
      {
        this.HideHelpPopups();
        Options.Get().SetBool(Option.HAS_SEEN_LOAD_EQUIPMENT_IN_SLOT_TUTORIAL, true);
      }
      this.SetupActiveMercDataModel(displayDataModel, mercenary);
      this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent("STOP_DRAG_EQUIPMENT_SLOTTED", new Widget.TriggerEventParameters());
    }
  }

  protected virtual void OnEquipmentLoadoutDragStart()
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
      InputMgr.Get().GrabMercenariesModeCard((IDataModel) payload, lettuceEquipment.m_cardType, new InputMgr.OnCardDroppedCallback(this.OnEquipmentDropped));
      if (mercenary.CanUnslotEquipment(payload.AbilityId))
        this.m_draggingEquipmentDataModel = payload;
      this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent("START_DRAG_EQUIPMENT", new Widget.TriggerEventParameters());
    }
  }

  protected void OnAbilityClicked()
  {
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("MercenaryDetailDisplay.OnAbilityClicked - no event data model attached to clicked ability!");
    }
    else
    {
      LettuceAbilityDataModel payload = (LettuceAbilityDataModel) eventDataModel.Payload;
      this.m_currentlyDisplayedAbility = payload;
      if (payload == null)
      {
        Log.Lettuce.PrintError("MercenaryDetailDisplay.OnAbilityClicked - data model attached to ability was not correct type!");
      }
      else
      {
        LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) payload.ParentMercId);
        if (mercenary == null)
          Log.Lettuce.PrintError("MercenaryDetailDisplay.OnAbilityClicked - no parent mercenary with ID {0} found for Ability ID {1}", (object) payload.ParentMercId, (object) payload.AbilityId);
        else if (this.ShouldShowAbilityInfoPopup(payload, mercenary))
        {
          LettuceAbilityDataModel abilityDataModel = payload.IsEquipment ? payload.CloneDataModel<LettuceAbilityDataModel>() : payload;
          if (abilityDataModel.IsEquipment)
            abilityDataModel.IsEquipped = false;
          this.m_abilityInfoPopupWidget.BindDataModel((IDataModel) abilityDataModel);
          this.m_popupHandlerVisualController.SetState("SHOW_ABILITY_INFO_POPUP");
        }
        else
        {
          LettuceAbilityUpgradeDisplayDataModel displayDataModel = new LettuceAbilityUpgradeDisplayDataModel();
          displayDataModel.CurrentTierAbility = payload.CloneDataModel<LettuceAbilityDataModel>();
          displayDataModel.NextTierAbility = payload.CloneDataModel<LettuceAbilityDataModel>();
          int num = ++displayDataModel.NextTierAbility.CurrentTier;
          CardDataModel abilityTierCard = payload.AbilityTiers[num - 1].AbilityTierCard;
          if (abilityTierCard != null)
          {
            string cardId = abilityTierCard.CardId;
            displayDataModel.IsMinion = GameUtils.GetCardTagValue(cardId, GAME_TAG.LETTUCE_ABILITY_SUMMONED_MINION) > 0;
          }
          displayDataModel.CurrentTierAbility.IsEquipped = false;
          displayDataModel.NextTierAbility.IsEquipped = false;
          if (payload.AbilityTiers.Count < displayDataModel.CurrentTierAbility.CurrentTier || payload.AbilityTiers.Count < displayDataModel.NextTierAbility.CurrentTier)
          {
            Log.Lettuce.PrintError(string.Format("MercenaryDetailDisplay.OnAbilityClicked - current tier {0} or {1}", (object) displayDataModel.CurrentTierAbility.CurrentTier, (object) displayDataModel.NextTierAbility.CurrentTier) + string.Format(" are greater than the number of tiers {0}", (object) payload.AbilityTiers.Count));
          }
          else
          {
            displayDataModel.NextTierAbilityChanges = new LettuceAbilityModifiedValuesDataModel();
            this.PopulateAbilityModifiedValues(displayDataModel.NextTierAbilityChanges, payload.AbilityTiers[displayDataModel.CurrentTierAbility.CurrentTier - 1].AbilityTierCard?.CardId, payload.AbilityTiers[displayDataModel.NextTierAbility.CurrentTier - 1].AbilityTierCard?.CardId);
            this.m_abilityUpgradePopupVisualController.Owner.BindDataModel((IDataModel) displayDataModel, false);
            this.m_popupHandlerVisualController.SetState("SHOW_ABILITY_UPGRADE_POPUP");
          }
        }
      }
    }
  }

  protected void PopulateAbilityModifiedValues(
    LettuceAbilityModifiedValuesDataModel dataModel,
    string baseCardId,
    string upgradeCardId)
  {
    if (string.IsNullOrEmpty(baseCardId))
      Log.Lettuce.PrintWarning("PopulateAbilityModifiedValues - invalid base card Id = " + baseCardId);
    else if (string.IsNullOrEmpty(upgradeCardId))
    {
      Log.Lettuce.PrintWarning("PopulateAbilityModifiedValues - invalid upgrade card Id = " + upgradeCardId);
    }
    else
    {
      EntityDef entityDef1 = DefLoader.Get().GetEntityDef(baseCardId);
      if (entityDef1 == null)
      {
        Log.Lettuce.PrintWarning("PopulateAbilityModifiedValues - failed to load baseEntityDef for cardID = " + baseCardId);
      }
      else
      {
        EntityDef entityDef2 = DefLoader.Get().GetEntityDef(upgradeCardId);
        if (entityDef2 == null)
        {
          Log.Lettuce.PrintWarning("PopulateAbilityModifiedValues - failed to load upgradeEntityDef for cardID = " + upgradeCardId);
        }
        else
        {
          dataModel.IsAttackChanging = entityDef1.GetATK() != entityDef2.GetATK();
          dataModel.IsHealthChanging = entityDef1.GetHealth() != entityDef2.GetHealth();
          dataModel.IsSpeedChanging = entityDef1.GetCost() != entityDef2.GetCost();
          dataModel.IsCooldownChanging = entityDef1.GetTag(GAME_TAG.LETTUCE_COOLDOWN_CONFIG) != entityDef2.GetTag(GAME_TAG.LETTUCE_COOLDOWN_CONFIG);
          dataModel.IsDescriptionChanging = this.GetCardTextInHand(entityDef1) != this.GetCardTextInHand(entityDef2);
        }
      }
    }
  }

  protected string GetCardTextInHand(EntityDef entityDef)
  {
    CardTextBuilder cardTextBuilder = entityDef.GetCardTextBuilder();
    return cardTextBuilder != null ? cardTextBuilder.BuildCardTextInHand(entityDef) : CardTextBuilder.GetDefaultCardTextInHand(entityDef);
  }

  protected bool ShouldShowAbilityInfoPopup(
    LettuceAbilityDataModel abilityData,
    LettuceMercenary parentMercenary)
  {
    return abilityData.IsEquipment ? !abilityData.Owned || abilityData.CurrentTier >= abilityData.MaxTier : parentMercenary.m_level < abilityData.UnlockLevel || abilityData.CurrentTier >= abilityData.MaxTier;
  }

  protected void ShowCoinTooltip()
  {
    if (!((UnityEngine.Object) this.m_tooltipZone != (UnityEngine.Object) null))
      return;
    this.m_tooltipZone.ShowTooltip(GameStrings.Get("GLUE_LETTUCE_COIN_TOOLTIP_HEADER"), GameStrings.Format("GLUE_LETTUCE_COIN_TOOLTIP_BODY"), 4f);
  }

  protected void HideCoinTooltip()
  {
    if (!((UnityEngine.Object) this.m_tooltipZone != (UnityEngine.Object) null))
      return;
    this.m_tooltipZone.HideTooltip();
  }

  protected void OnUpgradeAbility()
  {
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("No event data model attached to the LettuceMercDetailAbilityUpdragePopup");
    }
    else
    {
      LettuceAbilityDataModel payload = (LettuceAbilityDataModel) eventDataModel.Payload;
      if (payload == null)
        return;
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
      if (!payload.ReadyForUpgrade)
      {
        info.m_headerText = GameStrings.Get("GLUE_LETTUCE_ABILITY_UPGRADE_NOT_ENOUGH_COINS_HEADER");
        info.m_text = GameStrings.Get("GLUE_LETTUCE_ABILITY_UPGRADE_NOT_ENOUGH_COINS_BODY");
        info.m_showAlertIcon = true;
        info.m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle;
        info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
      }
      else
      {
        info.m_headerText = GameStrings.Get("GLUE_LETTUCE_ABILITY_UPGRADE_CONFIRMATION_HEADER");
        info.m_text = !payload.IsEquipment ? GameStrings.Get("GLUE_LETTUCE_ABILITY_UPGRADE_CONFIRMATION_BODY") : GameStrings.Get("GLUE_LETTUCE_EQUIPMENT_UPGRADE_CONFIRMATION_BODY");
        info.m_showAlertIcon = false;
        info.m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle;
        info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
        info.m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES");
        info.m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO");
        info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnAbilityUpgradePopupResponse);
        info.m_responseUserData = (object) payload;
      }
      DialogManager.Get().ShowPopup(info);
    }
  }

  protected void OnAbilityUpgradePopupResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL || !(userData is LettuceAbilityDataModel abilityDataModel))
      return;
    LettuceMercenary displayedMercenary = this.GetCurrentlyDisplayedMercenary();
    if (displayedMercenary == null)
    {
      Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnAbilityUpgradePopupResponse - no currently displayed mercenary!");
    }
    else
    {
      LettuceAbility lettuceAbility = abilityDataModel.IsEquipment ? displayedMercenary.GetLettuceEquipment(abilityDataModel.AbilityId) : displayedMercenary.GetLettuceAbility(abilityDataModel.AbilityId);
      if (lettuceAbility == null)
        Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnAbilityUpgradePopupResponse - No ability found on merc {0} for ability Id {1}.", (object) displayedMercenary.ID, (object) abilityDataModel.AbilityId);
      else if (lettuceAbility.m_tier != abilityDataModel.CurrentTier)
        Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnAbilityUpgradePopupResponse - Ability {0} is currently tier {1}, and cannot upgrade to tier {2}", (object) lettuceAbility.ID, (object) lettuceAbility.m_tier, (object) (abilityDataModel.CurrentTier + 1));
      else if (displayedMercenary.m_currencyAmount < (long) lettuceAbility.GetNextUpgradeCost())
      {
        Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnAbilityUpgradePopupResponse - Ability {0} requires {1} coins to upgrade to tier {2}, but only has {3} coins.", (object) lettuceAbility.ID, (object) lettuceAbility.GetNextUpgradeCost(), (object) (lettuceAbility.m_tier + 1), (object) displayedMercenary.m_currencyAmount);
      }
      else
      {
        if (Network.IsLoggedIn())
        {
          if (lettuceAbility.m_cardType == CollectionUtils.MercenariesModeCardType.Ability)
            Network.Get().UpgradeMercenaryAbility(displayedMercenary.ID, lettuceAbility.ID);
          else
            Network.Get().UpgradeMercenaryEquipment(displayedMercenary.ID, lettuceAbility.ID);
        }
        this.m_abilityMaterialData.Material = this.m_abilityUpgradeCard.CardActor.GetPortraitMaterial();
        if (!((UnityEngine.Object) this.m_abilityUpgradePopupVisualController != (UnityEngine.Object) null))
          return;
        this.m_abilityUpgradePopupVisualController.OwningWidget.TriggerEvent("PLAY_ANIMATION", new Widget.TriggerEventParameters());
      }
    }
  }

  protected virtual void OnAbilityUpgradeNetworkResponse()
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
      LettuceTutorialUtils.FireEvent(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_UPGRADE_ABILITY_END, this.gameObject);
    }
  }

  protected void OnEquipmentUpgradeNetworkResponse()
  {
    UpgradeMercenaryEquipmentResponse equipmentResponse = Network.Get().UpgradeMercenaryEquipmentResponse();
    if (equipmentResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.Lettuce.PrintError("MercenaryDetailDisplay.OnEquipmentUpgradeNetworkResponse() - Error upgrading equipment: {0} for equipment {1} on mercenary {2}", (object) equipmentResponse.ErrorCode, (object) equipmentResponse.EquipmentId, (object) equipmentResponse.MercenaryId);
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) equipmentResponse.MercenaryId);
      if (mercenary == null)
      {
        Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnEquipmentUpgradeNetworkResponse - No mercenary found with Id {0}.", (object) equipmentResponse.MercenaryId);
      }
      else
      {
        LettuceAbility lettuceEquipment = mercenary.GetLettuceEquipment(equipmentResponse.EquipmentId);
        if (lettuceEquipment == null)
          Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnEquipmentUpgradeNetworkResponse - No ability found on mercenary {0} with equipment Id {1}.", (object) equipmentResponse.MercenaryId, (object) equipmentResponse.EquipmentId);
        else
          this.UpdateDataModelsAfterTransaction(lettuceEquipment, mercenary);
      }
    }
  }

  protected void UpdateAbilityText(
    LettuceMercenaryDataModel dataModel,
    LettuceMercenary merc,
    LettuceAbility slottedEquip)
  {
    CardDbfRecord cardRecord = merc.GetCardRecord();
    string cardTextInHand = DefLoader.Get().GetEntityDef(cardRecord.ID).GetCardTextInHand();
    LettuceEquipmentTierDbfRecord currentTierRecord = slottedEquip?.GetCurrentTierRecord();
    if (currentTierRecord != null && currentTierRecord.ShowTextOnMerc && !string.IsNullOrEmpty((string) currentTierRecord.CardRecord?.TextInHand))
      cardTextInHand = DefLoader.Get().GetEntityDef(currentTierRecord.CardId).GetCardTextInHand();
    dataModel.ShowAbilityText = true;
    dataModel.AbilityText = cardTextInHand;
  }

  protected virtual LettuceMercenaryDataModel UpdateDataModelsAfterTransaction(
    LettuceAbility ability,
    LettuceMercenary merc)
  {
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    MercenariesDataUtil.UpdateMercenaryDataModelWithNewData(displayDataModel, ability, merc);
    this.UpdateAbilityText(displayDataModel, merc, merc.GetSlottedEquipment());
    return displayDataModel;
  }

  public LettuceMercenary GetCurrentlyDisplayedMercenary() => !this.DisplayVisible ? (LettuceMercenary) null : CollectionManager.Get().GetMercenary((long) this.m_mercIdBeingViewed);

  public bool IsMouseOverEquipmentSlot() => UniversalInputManager.Get().ForcedUnblockableInputIsOver(Camera.main, this.m_equipmentSlotCollider, out RaycastHit _);

  public void OnCraftEquipment()
  {
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_equipmentCraftingPopupVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("No event data model attached to the LettuceEquipmentCraftingPopup");
    }
    else
    {
      LettuceAbilityDataModel payload = (LettuceAbilityDataModel) eventDataModel.Payload;
      if (payload == null)
      {
        Log.Lettuce.PrintError("Event data attached to LettuceEquipmentCraftingPopup not of expected type!");
      }
      else
      {
        LettuceMercenary displayedMercenary = this.GetCurrentlyDisplayedMercenary();
        LettuceAbility lettuceEquipment = displayedMercenary.GetLettuceEquipment(payload.AbilityId);
        if (lettuceEquipment == null)
          Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnEquipmentCraftingPopupResponse - no equipment found with ID {0} on Mercenary ID {1}", (object) payload.AbilityId, (object) this.m_mercIdBeingViewed);
        else if (lettuceEquipment.Owned)
        {
          Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnEquipmentCraftingPopupResponse - equipment ID {0} on mercenary ID {1} in craft request already owned!", (object) lettuceEquipment.ID, (object) this.m_mercIdBeingViewed);
        }
        else
        {
          if (Network.IsLoggedIn())
            Network.Get().CraftMercenaryEquipment(displayedMercenary.ID, lettuceEquipment.ID);
          if (!((UnityEngine.Object) this.m_abilityInfoPopupWidget != (UnityEngine.Object) null))
            return;
          this.m_abilityInfoPopupWidget.TriggerEvent("HIDE_REGULAR");
        }
      }
    }
  }

  public virtual bool ShowFullyUpgradeMercIfNeeded() => PopupDisplayManager.Get().RewardPopups.ShowMercenariesFullyUpgraded((Action) (() =>
  {
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) displayDataModel.MercenaryId);
    this.SetupActiveMercDataModel(displayDataModel, mercenary);
    this.ShowRequiredTutorialIfNeeded();
  }));

  public void ShowExplanationPopup()
  {
    if (this.m_currentlyDisplayedAbility == null || !this.m_currentlyDisplayedAbility.IsEquipment)
      return;
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager.GetHasOpenedDetailsDisplay() || Options.Get().GetBool(Option.HAS_UNLOCKED_FIRST_EQUIPMENT))
      return;
    if ((bool) (UnityEngine.Object) this.m_equipmentExplanationPopupWidget)
    {
      collectionManager.SetHasVisitedDetailsDisplayTrue();
      this.m_equipmentExplanationPopupWidget.TriggerEvent("SHOW");
    }
    else
      Log.Lettuce.PrintWarning("MercenaryDetailDisplay.ShowExplanationPopup - Equipment explanation popup widget not loaded on request");
  }

  protected void OnAppearanceClicked()
  {
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("MercenaryDetailDisplay.OnAppearanceClicked - no event data model attached to clicked ability!");
    }
    else
    {
      LettuceMercenaryArtVariationDataModel payload = (LettuceMercenaryArtVariationDataModel) eventDataModel.Payload;
      if (payload == null)
      {
        Log.Lettuce.PrintError("MercenaryDetailDisplay.OnAppearanceClicked - data model attached to appearance was not correct type!");
      }
      else
      {
        LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
        for (int index = 0; index < displayDataModel.ArtVariationList.Count; ++index)
        {
          LettuceMercenaryArtVariationDataModel artVariation = displayDataModel.ArtVariationList[index];
          artVariation.Selected = artVariation == payload;
        }
        CollectionManager.Get().SendSelectedMercenaryArtVariation(displayDataModel.MercenaryId, payload.ArtVariationId, payload.Card.Premium);
        if (Options.Get().GetBool(Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL, false))
          return;
        this.HideHelpPopups();
        Options.Get().SetBool(Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL, true);
      }
    }
  }

  protected void OnAppearanceResetPage() => this.GetMercenaryDisplayDataModel().ArtVariationPageIndex = 0;

  protected void OnAppearanceArrowClicked(int dir)
  {
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    int num = displayDataModel.ArtVariationPageIndex + dir;
    if (num < 0)
      num = 0;
    else if (num >= displayDataModel.ArtVariationPageList.Count)
      num = displayDataModel.ArtVariationPageList.Count - 1;
    displayDataModel.ArtVariationPageIndex = num;
  }

  public LettuceMercenaryDataModel GetMercenaryDisplayDataModel()
  {
    if ((UnityEngine.Object) this.m_mercDetailsDisplayVisualController == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintError("MercenaryDetailDisplay.GetMercenaryDisplayDataModel - Missing required VisualController reference");
      return (LettuceMercenaryDataModel) null;
    }
    Widget owner = (Widget) this.m_mercDetailsDisplayVisualController.Owner;
    IDataModel model;
    if (!owner.GetDataModel(216, out model))
    {
      model = (IDataModel) MercenaryFactory.CreateEmptyMercenaryDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceMercenaryDataModel;
  }

  public void RegisterOnHideEvent(MercenaryDetailDisplay.OnHideDelegate callback)
  {
    if (this.m_onHideCallbacks.Contains(callback))
      return;
    this.m_onHideCallbacks.Add(callback);
  }

  public void UnregisterOnHideEvent(MercenaryDetailDisplay.OnHideDelegate callback) => this.m_onHideCallbacks.Remove(callback);

  private void OnHide()
  {
    foreach (MercenaryDetailDisplay.OnHideDelegate onHideDelegate in this.m_onHideCallbacks.ToArray())
      onHideDelegate();
  }

  protected void OnAbilityDragStart()
  {
    this.HideHoverCards();
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_mercDetailsDisplayVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("No event data model attached to LettuceMercDetailDisplay");
    }
    else
    {
      LettuceAbilityDataModel payload = (LettuceAbilityDataModel) eventDataModel.Payload;
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) this.m_mercIdBeingViewed);
      LettuceAbility ability = payload.IsEquipment ? mercenary.GetLettuceEquipment(payload.AbilityId) : mercenary.GetLettuceAbility(payload.AbilityId);
      if (!this.CanPickUpAbility(mercenary, ability))
        return;
      this.m_equipmentSlotCollider.SetActive(true);
      InputMgr.Get().GrabMercenariesModeCard((IDataModel) payload, ability.m_cardType, new InputMgr.OnCardDroppedCallback(this.OnEquipmentDropped));
      this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent("START_DRAG_EQUIPMENT", new Widget.TriggerEventParameters());
    }
  }

  protected virtual bool CanPickUpAbility(LettuceMercenary merc, LettuceAbility ability)
  {
    if (this.m_mercIdBeingViewed == -1)
      return true;
    return ability.m_cardType == CollectionUtils.MercenariesModeCardType.Equipment && ability.Owned;
  }

  protected void OnEquipmentDropped()
  {
    if (this.m_draggingEquipmentDataModel != null)
    {
      LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) displayDataModel.MercenaryId);
      if (mercenary.UnslotEquipment(this.m_draggingEquipmentDataModel.AbilityId))
      {
        this.m_draggingEquipmentDataModel.IsEquipped = false;
        CollectionManager.Get().SendEquippedMercenaryEquipment(mercenary.ID);
        this.SetupActiveMercDataModel(displayDataModel, mercenary);
      }
    }
    this.m_equipmentSlotCollider.SetActive(false);
    this.m_mercDetailsDisplayVisualController.OwningWidget.TriggerEvent("STOP_DRAG_EQUIPMENT", new Widget.TriggerEventParameters());
    this.m_draggingEquipmentDataModel = (LettuceAbilityDataModel) null;
  }

  protected void SetupActiveMercDataModel(
    LettuceMercenaryDataModel mercData,
    LettuceMercenary collectionMerc)
  {
    CollectionUtils.PopulateMercenaryDataModel(mercData, collectionMerc, CollectionUtils.MercenaryDataPopluateExtra.Abilities | CollectionUtils.MercenaryDataPopluateExtra.Coin | CollectionUtils.MercenaryDataPopluateExtra.Appearances | CollectionUtils.MercenaryDataPopluateExtra.UpdateValuesWithSlottedEquipment);
    mercData.MercenarySelected = true;
    mercData.ChildUpgradeAvailable = false;
    this.UpdateAbilityText(mercData, collectionMerc, collectionMerc.GetSlottedEquipment());
  }

  protected void OnCraftEquipmentNetworkResponse()
  {
    CraftMercenaryEquipmentResponse equipmentResponse = Network.Get().CraftMercenaryEquipmentResponse();
    if (equipmentResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.Lettuce.PrintError("MercenaryDetailDisplay.OnCraftEquipmentNetworkResponse - Error Code {0} crafting equipment ID {1} on mercenary ID {2}", (object) equipmentResponse.ErrorCode, (object) equipmentResponse.EquipmentId, (object) equipmentResponse.MercenaryId);
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) equipmentResponse.MercenaryId);
      if (mercenary == null)
      {
        Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnCraftEquipmentNetworkResponse - No mercenary found with ID {0}.", (object) equipmentResponse.MercenaryId);
      }
      else
      {
        LettuceAbility lettuceEquipment = mercenary.GetLettuceEquipment(equipmentResponse.EquipmentId);
        if (lettuceEquipment == null)
          Log.Lettuce.PrintWarning("MercenaryDetailDisplay.OnCraftEquipmentNetworkResponse - No equipment found with ID {0} on Mercenary ID {1}.", (object) equipmentResponse.EquipmentId, (object) equipmentResponse.MercenaryId);
        else
          this.UpdateDataModelsAfterTransaction(lettuceEquipment, mercenary);
      }
    }
  }

  protected bool TutorialShouldShowAbilityUpgrade()
  {
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    return (UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.TutorialShouldShowAbilityUpgrade();
  }

  protected void ShowRequiredTutorialIfNeeded()
  {
    LettuceMercenary displayedMercenary = this.GetCurrentlyDisplayedMercenary();
    if (!Options.Get().GetBool(Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL, false) && UserAttentionManager.CanShowAttentionGrabber("MercenaryDetailDisplay.ShowAppearanceTutorialIfNeeded:" + (object) Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL) && displayedMercenary != null && displayedMercenary.ID == 18 && displayedMercenary.HasUnlockedGoldenOrBetter())
    {
      this.HideHelpPopups();
      this.m_helpPopupType = Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL;
      this.m_tutorialCoroutine = this.StartCoroutine(this.ShowAppearancePart1TutorialWhenReady());
    }
    else
    {
      bool flag = displayedMercenary != null && displayedMercenary.CanAnyAbilityBeUpgraded();
      if (flag || displayedMercenary != null && displayedMercenary.ID == 69)
        LettuceTutorialUtils.FireEvent(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_UPGRADE_ABILITY_START, this.gameObject);
      if (this.TutorialShouldShowAbilityUpgrade() & flag && UserAttentionManager.CanShowAttentionGrabber("MercenaryDetailDisplay.ShowEquipmentSlotTutorialIfNeeded:HAS_SEEN_ABILITY_UPGRADE"))
      {
        this.HideHelpPopups();
        this.m_tutorialCoroutine = this.StartCoroutine(this.ShowUpgradeAbilityTutorialWhenReady(displayedMercenary));
      }
      else
      {
        if (Options.Get().GetBool(Option.HAS_SEEN_LOAD_EQUIPMENT_IN_SLOT_TUTORIAL, false) || !UserAttentionManager.CanShowAttentionGrabber("MercenaryDetailDisplay.ShowEquipmentSlotTutorialIfNeeded:" + (object) Option.HAS_SEEN_LOAD_EQUIPMENT_IN_SLOT_TUTORIAL))
          return;
        this.HideHelpPopups();
        this.m_helpPopupType = Option.HAS_SEEN_LOAD_EQUIPMENT_IN_SLOT_TUTORIAL;
        this.m_tutorialCoroutine = this.StartCoroutine(this.ShowEquipmentSlotTutorialWhenReady());
      }
    }
  }

  protected void ShowAppearancePart2TutorialIfNeeded()
  {
    if (Options.Get().GetBool(Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL, false) || !UserAttentionManager.CanShowAttentionGrabber("MercenaryDetailDisplay.ShowAppearanceTutorialIfNeeded:" + (object) Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL) || this.GetCurrentlyDisplayedMercenary().ID != 18 || !CollectionManager.Get().GetMercenary(18L).HasUnlockedGoldenOrBetter())
      return;
    this.HideHelpPopups();
    this.m_helpPopupType = Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL;
    this.m_tutorialCoroutine = this.StartCoroutine(this.ShowAppearancePart2TutorialWhenReady());
  }

  protected IEnumerator ShowEquipmentSlotTutorialWhenReady()
  {
    yield return (object) new WaitForSeconds(this.m_secondsDelayBeforeTutorialPopups);
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    if (displayDataModel != null)
    {
      int ownedEquipmentIndex = CollectionUtils.GetFirstOwnedEquipmentIndex(displayDataModel);
      if (ownedEquipmentIndex != -1)
      {
        this.m_helpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_showLoadEquipmentInSlotTutorialBones[ownedEquipmentIndex].position, this.m_showLoadEquipmentInSlotTutorialBones[ownedEquipmentIndex].localScale, GameStrings.Get("GLUE_LETTUCE_COLLECTION_TUTORIAL02"));
        this.m_helpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
        this.m_helpPopup.PulseReminderEveryXSeconds(3f);
      }
    }
  }

  protected IEnumerator ShowUpgradeAbilityTutorialWhenReady(LettuceMercenary merc)
  {
    yield return (object) new WaitForSeconds(this.m_secondsDelayBeforeTutorialPopups);
    this.m_helpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_upgradeAbilityTutorialBone.position, this.m_upgradeAbilityTutorialBone.localScale, GameStrings.Get("GLUE_LETTUCE_COLLECTION_UPGRADE_ABILITY_TOOLTIP"));
    for (int index = 0; index < merc.m_abilityList.Count; ++index)
    {
      if (merc.IsCardReadyForUpgrade(merc.m_abilityList[index]))
      {
        switch (index)
        {
          case 0:
            this.m_helpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.LeftUp);
            continue;
          case 1:
            this.m_helpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
            continue;
          case 2:
            this.m_helpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.RightUp);
            continue;
          default:
            continue;
        }
      }
    }
    this.m_helpPopup.PulseReminderEveryXSeconds(3f);
  }

  protected IEnumerator ShowAppearancePart1TutorialWhenReady()
  {
    yield return (object) new WaitForSeconds(this.m_secondsDelayBeforeTutorialPopups);
    this.m_helpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_AppearanceMercTutorialBone.position, this.m_AppearanceMercTutorialBone.localScale, GameStrings.Get("GLUE_LETTUCE_COLLECTION_TUTORIAL_PORTRAIT_02"));
    this.m_helpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
    this.m_helpPopup.PulseReminderEveryXSeconds(3f);
  }

  protected IEnumerator ShowAppearancePart2TutorialWhenReady()
  {
    yield return (object) new WaitForSeconds(this.m_secondsDelayBeforeTutorialPopups);
    this.m_helpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_AppearanceTutorialBone.position, this.m_AppearanceMercTutorialBone.localScale, GameStrings.Get("GLUE_LETTUCE_COLLECTION_TUTORIAL_PORTRAIT_03"));
    this.m_helpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
    this.m_helpPopup.PulseReminderEveryXSeconds(3f);
  }

  public void Dev_ShowTutorialPopups()
  {
    List<Transform> transformList = new List<Transform>();
    transformList.AddRange((IEnumerable<Transform>) this.m_showLoadEquipmentInSlotTutorialBones);
    transformList.Add(this.m_upgradeAbilityTutorialBone);
    transformList.Add(this.m_AppearanceMercTutorialBone);
    foreach (Transform transform in transformList)
      NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, transform.position, transform.localScale, transform.name);
  }

  private void OnDropMercenariesModeCard(
    CollectionUtils.MercenariesModeCardType cardType,
    string mercenariesCardID)
  {
    if (cardType == CollectionUtils.MercenariesModeCardType.Equipment)
    {
      if (!this.IsMouseOverEquipmentSlot())
        return;
      this.SlotSelectedEquipment(mercenariesCardID);
    }
    else
      Debug.LogError((object) string.Format("{0}.{1} could not handle type {2}", (object) nameof (MercenaryDetailDisplay), (object) nameof (OnDropMercenariesModeCard), (object) cardType));
  }

  private void OnMercenaryArtVariationChanged(
    int mercenaryDbId,
    int artVariationId,
    TAG_PREMIUM premium)
  {
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenaryDbId);
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    CollectionUtils.PopulateMercenaryCardDataModel(displayDataModel, mercenary.GetEquippedArtVariation());
    CollectionUtils.UpdateMercenaryCardStats(displayDataModel, mercenary);
  }

  protected void AcknowledgeAbilityorEquipment(int itemID, bool acknowledgeAll = false)
  {
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) displayDataModel.MercenaryId);
    if (mercenary == null)
      return;
    foreach (LettuceAbilityDataModel ability in displayDataModel.AbilityList)
    {
      LettuceAbilityDataModel abilityData = ability;
      LettuceAbility lettuceAbility = mercenary.GetLettuceAbility(abilityData.AbilityId);
      if (lettuceAbility != null && !mercenary.IsAbilityLocked(lettuceAbility) && itemID == abilityData.AbilityId | acknowledgeAll && this.m_mercenaryAcknowledgements.FindIndex((Predicate<MercenaryAcknowledgeData>) (i => i.AssetId == abilityData.AbilityId)) <= 0)
      {
        abilityData.IsNew = false;
        MercenaryAcknowledgeData ackData = new MercenaryAcknowledgeData();
        ackData.Type = MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_ABILITY_ALL;
        ackData.AssetId = abilityData.AbilityId;
        ackData.Acknowledged = true;
        ackData.MercenaryId = displayDataModel.MercenaryId;
        this.m_mercenaryAcknowledgements.Add(ackData);
        CollectionManager.Get().MarkMercenaryAsAcknowledgedinCollection(ackData);
      }
    }
    foreach (LettuceAbilityDataModel equipment in displayDataModel.EquipmentList)
    {
      LettuceAbilityDataModel equipmentData = equipment;
      LettuceAbility lettuceEquipment = mercenary.GetLettuceEquipment(equipmentData.AbilityId);
      if (lettuceEquipment != null && lettuceEquipment.Owned && itemID == equipmentData.AbilityId | acknowledgeAll && this.m_mercenaryAcknowledgements.FindIndex((Predicate<MercenaryAcknowledgeData>) (i => i.AssetId == equipmentData.AbilityId)) <= 0)
      {
        equipmentData.IsNew = false;
        MercenaryAcknowledgeData ackData = new MercenaryAcknowledgeData();
        ackData.Type = MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_EQUIPMENT_ALL;
        ackData.AssetId = equipmentData.AbilityId;
        ackData.Acknowledged = true;
        ackData.MercenaryId = displayDataModel.MercenaryId;
        this.m_mercenaryAcknowledgements.Add(ackData);
        CollectionManager.Get().MarkMercenaryAsAcknowledgedinCollection(ackData);
      }
    }
  }

  protected void AcknowledgeArtVariation(int artID, TAG_PREMIUM premium, bool acknowledgeAll = false)
  {
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDisplayDataModel();
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) displayDataModel.MercenaryId);
    if (mercenary == null)
      return;
    foreach (LettuceMercenaryArtVariationDataModel artVariation1 in displayDataModel.ArtVariationList)
    {
      LettuceMercenaryArtVariationDataModel artVariation = artVariation1;
      if (artVariation.Unlocked && ((artVariation.ArtVariationId != artID ? 0 : (artVariation.Card.Premium == premium ? 1 : 0)) | (acknowledgeAll ? 1 : 0)) != 0 && this.m_mercenaryAcknowledgements.FindIndex((Predicate<MercenaryAcknowledgeData>) (i => i.AssetId == artVariation.ArtVariationId && (TAG_PREMIUM) i.Premium == premium)) < 0)
      {
        artVariation.NewlyUnlocked = false;
        MercenaryAcknowledgeData ackData = new MercenaryAcknowledgeData()
        {
          Type = MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_ART_VARIATION_ACQUIRED,
          MercenaryId = displayDataModel.MercenaryId,
          AssetId = artVariation.ArtVariationId,
          Premium = (uint) artVariation.Card.Premium,
          Acknowledged = true
        };
        ackData.MercenaryId = displayDataModel.MercenaryId;
        this.m_mercenaryAcknowledgements.Add(ackData);
        CollectionManager.Get().MarkMercenaryAsAcknowledgedinCollection(ackData);
      }
    }
    displayDataModel.NumNewPortraits = CollectionManager.Get().GetNumNewPortraitsToAcknowledgeForMercenary(mercenary);
  }

  protected void SendAcknowledgements()
  {
    MercenariesDataUtil.UpdateMercenaryDataModelNewStatus(this.GetMercenaryDisplayDataModel());
    Network.Get().RegisterNetHandler((object) MercenariesCollectionAcknowledgeResponse.PacketID.ID, new Network.NetHandler(this.OnCollectionAcknowledgeResponse));
    Network.Get().AcknowledgeMercenaryCollection(this.m_mercenaryAcknowledgements);
    this.m_mercenaryAcknowledgements.Clear();
  }

  protected void OnCollectionAcknowledgeResponse()
  {
    Network.Get().RemoveNetHandler((object) MercenariesCollectionAcknowledgeResponse.PacketID.ID, new Network.NetHandler(this.OnCollectionAcknowledgeResponse));
    if (Network.Get().AcknowledgeMercenaryCollectionResponse().Success)
      return;
    Debug.LogWarning((object) "Error acknowledging collection");
  }

  public delegate void OnHideDelegate();
}
