using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class DeckTrayMercListContent : DeckTrayReorderableContent
{
  public AsyncReference m_mercLoadoutDisplay;
  [CustomEditField(Sections = "Scroll Settings")]
  public BoxCollider m_LockedScrollBounds;
  [CustomEditField(Sections = "Scroll Settings")]
  public float m_scrollHeightExtraBuffer = 20f;
  [CustomEditField(Sections = "Other Objects")]
  public GameObject m_teamCompleteHighlight;
  [CustomEditField(Sections = "Other Objects")]
  public Transform m_bigCardTopPosition;
  [CustomEditField(Sections = "Other Objects")]
  public Transform m_bigCardBottomPosition;
  [CustomEditField(Sections = "Other Objects")]
  public PlayMakerFSM m_doneButtonPlayMaker;
  [Tooltip("Sensitivity of dragging towards the left direction that the merc would be removed from the list content")]
  [CustomEditField(Sections = "Interaction Settings")]
  [Range(-1f, 1f)]
  public float m_dragRemoveDirectionSensitivity = 0.9f;
  private VisualController m_mercLoadoutVisualController;
  private LettuceTeamDataModel m_selectedTeamDataModel;
  private bool m_mercLoadoutDisplayFinishedLoading;
  private const string ADD_CARD_TO_TEAM_SOUND = "collection_manager_card_add_to_deck_instant.prefab:06df359c4026d7e47b06a4174f33e3ef";
  private const float CARD_MOVEMENT_TIME = 0.3f;
  private const string DECK_INCOMPLETE_STATE = "Deck_incomplete Idle";
  private const string DECK_INCOMPLETE_EVENT = "Deck_incomplete";
  private const string DECK_COMPLETE_STATE = "Deck_complete Idle";
  private const string DECK_COMPLETE_EVENT = "Deck_complete";
  private Vector3 m_originalLocalPosition;
  private List<DeckTrayMercListContent.MercCountChanged> m_mercCountChangedListeners = new List<DeckTrayMercListContent.MercCountChanged>();
  private bool m_animating;
  private bool m_hasFinishedEntering;
  private bool m_hasFinishedExiting = true;

  public Listable MercListable { get; private set; }

  public bool IsReorderingAllowed { get; private set; }

  protected override void Awake()
  {
    base.Awake();
    this.m_mercLoadoutDisplay.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnMercLoadoutDisplayReady));
    this.StartCoroutine(this.InitializeWhenReady());
    this.m_originalLocalPosition = this.transform.localPosition;
    this.m_hasFinishedEntering = false;
  }

  protected override void OnDestroy() => base.OnDestroy();

  public void OnMercLoadoutDisplayReady(VisualController visualController)
  {
    Widget component = visualController.GetComponent<Widget>();
    component.RegisterEventListener(new Widget.EventListenerDelegate(this.MercLoadoutEventListener));
    this.m_mercLoadoutVisualController = visualController;
    this.MercListable = component.gameObject.GetComponentInChildren<Listable>(true);
    this.m_mercLoadoutDisplayFinishedLoading = true;
  }

  public override bool AnimateContentEntranceStart()
  {
    if (!this.IsContentLoaded())
      return false;
    this.ResetReoderingState();
    this.m_animating = true;
    this.m_hasFinishedEntering = false;
    Action<object> action = (Action<object>) (_1 =>
    {
      this.UpdateTeamCompleteHighlight();
      this.m_animating = false;
    });
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam != null)
    {
      this.transform.localPosition = this.GetOffscreenLocalPosition();
      iTween.StopByName(this.gameObject, "position");
      iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.m_originalLocalPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.3f, (object) "easeType", (object) iTween.EaseType.easeOutQuad, (object) "oncomplete", (object) action, (object) "name", (object) "position"));
      if (editingTeam.GetMercCount() > 0)
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_new_deck_moves_up_tray.prefab:13650cd587089e14d9a297c8de6057f1", this.gameObject);
      this.UpdateMercList(false);
    }
    else
      action((object) null);
    return true;
  }

  public override bool PreAnimateContentEntrance()
  {
    CollectionUtils.PopulateMercenariesTeamDataModel(this.SelectedTeamDataModel, CollectionManager.Get().GetEditingTeam());
    if ((UnityEngine.Object) this.m_scrollbar != (UnityEngine.Object) null)
    {
      this.m_scrollbar.m_HeightMode = UIBScrollable.HeightMode.UseHeightCallback;
      this.m_scrollbar.SetScrollHeightCallback(new UIBScrollable.ScrollHeightCallback(this.ScrollHeightCallback));
    }
    return true;
  }

  public override bool AnimateContentEntranceEnd()
  {
    if (this.m_animating)
      return false;
    this.m_hasFinishedEntering = true;
    this.FireMercCountChangedEvent();
    return true;
  }

  public override bool PreAnimateContentExit()
  {
    if ((UnityEngine.Object) this.m_scrollbar != (UnityEngine.Object) null)
    {
      this.m_scrollbar.m_HeightMode = UIBScrollable.HeightMode.UseScrollableItem;
      this.m_scrollbar.SetScrollHeightCallback((UIBScrollable.ScrollHeightCallback) null);
    }
    return base.PreAnimateContentExit();
  }

  public override bool AnimateContentExitStart()
  {
    if (this.m_animating)
      return false;
    this.m_animating = true;
    this.m_hasFinishedExiting = false;
    this.m_teamCompleteHighlight.SetActive(false);
    iTween.StopByName(this.gameObject, "position");
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.GetOffscreenLocalPosition(), (object) "isLocal", (object) true, (object) "time", (object) 0.3f, (object) "easeType", (object) iTween.EaseType.easeInQuad, (object) "name", (object) "position"));
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

  public override void OnEditingTeamChanged(
    LettuceTeam newTeam,
    LettuceTeam oldTeam,
    bool isNewTeam)
  {
    base.OnEditingTeamChanged(newTeam, oldTeam, isNewTeam);
  }

  public override bool IsContentLoaded() => this.m_mercLoadoutDisplayFinishedLoading;

  public GameObject GetMercVisual(string cardID) => (GameObject) null;

  public override void Show(bool showAll = false) => base.Show(showAll);

  public override void Hide(bool hideAll = false) => base.Hide(hideAll);

  public bool AddMerc(
    EntityDef cardEntityDef,
    bool playSound,
    Actor animateFromActor = null,
    bool updateVisuals = true,
    int index = -1,
    LettuceMercenary.Loadout loadout = null)
  {
    if (!this.IsModeActive())
      return false;
    if (cardEntityDef == null)
    {
      Debug.LogError((object) "Trying to add card EntityDef that is null.");
      return false;
    }
    string cardId = cardEntityDef.GetCardId();
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam == null)
      return false;
    if (editingTeam.GetMercCount() == CollectionManager.Get().GetTeamSize())
    {
      GameplayErrorManager.Get().DisplayMessage(GameStrings.Get("GLUE_LETTUCE_COLLECTION_ON_ADD_FULL_TEAM_ERROR"));
      return false;
    }
    if (editingTeam.IsMercInTeam(cardId))
      return false;
    if (!editingTeam.AddMerc(cardId, index, loadout))
    {
      Log.Lettuce.PrintWarning("DecktrayMercListContent.AddMerc({0}): team.AddMerc failed!", (object) cardId);
      return false;
    }
    if (updateVisuals)
    {
      this.UpdateMercList(cardEntityDef, animateFromActor: animateFromActor);
      LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
      collectibleDisplay.UpdateCurrentPageCardLocks(true);
      CollectionDeckTray.Get().GetTeamsContent().UpdateTeamTrayVisuals();
      if (collectibleDisplay.IsMercenaryDetailsDisplayActive())
        this.ChangeCurrentlySelectedMercenary(CollectionManager.Get().GetMercenary(cardId).ID, true);
    }
    if (playSound)
      SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_place_card_in_deck.prefab:df069ffaea9dfb24b96accc95bc434a7", this.gameObject);
    this.ResetReoderingState();
    return true;
  }

  public void RemoveMerc(int mercID, bool playSound, bool updateVisuals = true)
  {
    if (!this.IsModeActive())
      return;
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam == null)
      return;
    if (!editingTeam.RemoveMerc(mercID))
    {
      Log.Lettuce.PrintWarning("DeckTrayMercListContent.RemoveMerc - attempted to remove merc ({0}) that is not in team.", (object) mercID);
    }
    else
    {
      if (playSound)
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_remove_from_deck_instant.prefab:bcee588ddfc73844ea3a24beb63bc53f", this.gameObject);
      if (!updateVisuals)
        return;
      this.UpdateMercList();
      CollectionManager.Get().GetCollectibleDisplay().UpdateCurrentPageCardLocks(true);
      CollectionDeckTray.Get().GetTeamsContent().UpdateTeamTrayVisuals();
      LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
      if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
        return;
      LettuceCollectionPageManager pageManager = collectibleDisplay.GetPageManager() as LettuceCollectionPageManager;
      if (!((UnityEngine.Object) pageManager != (UnityEngine.Object) null))
        return;
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercID);
      if (mercenary == null)
        return;
      pageManager.UpdatePageMercenary(MercenaryFactory.CreateMercenaryDataModelWithCoin(mercenary));
    }
  }

  [ContextMenu("Update Card List")]
  public void UpdateMercList() => this.UpdateMercList(true);

  public void UpdateMercList(
    bool updateHighlight,
    Actor animateFromActor = null,
    Action onCompleteCallback = null)
  {
    this.UpdateMercList(string.Empty, updateHighlight, animateFromActor, onCompleteCallback);
  }

  public void UpdateMercList(
    EntityDef justChangedCardEntityDef,
    bool updateHighlight = true,
    Actor animateFromActor = null,
    Action onCompleteCallback = null)
  {
    this.UpdateMercList(justChangedCardEntityDef != null ? justChangedCardEntityDef.GetCardId() : string.Empty, updateHighlight, animateFromActor, onCompleteCallback);
  }

  public void UpdateMercList(
    string justChangedCardID,
    bool updateHighlight = true,
    Actor animateFromActor = null,
    Action onCompleteCallback = null)
  {
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam == null)
      return;
    CollectionUtils.PopulateMercenariesTeamDataModel(this.SelectedTeamDataModel, editingTeam);
    this.m_mercLoadoutVisualController.SetState("SHOW");
    this.FireMercCountChangedEvent();
    if ((UnityEngine.Object) this.m_scrollbar != (UnityEngine.Object) null)
      this.m_scrollbar.UpdateScroll();
    if (!updateHighlight)
      return;
    this.UpdateTeamCompleteHighlight();
  }

  public LettuceMercenaryDataModel GetMercenaryDataModel(int mercId)
  {
    foreach (LettuceMercenaryDataModel mercenary in this.SelectedTeamDataModel.MercenaryList)
    {
      if (mercenary.MercenaryId == mercId)
        return mercenary;
    }
    return (LettuceMercenaryDataModel) null;
  }

  public void ChangeCurrentlySelectedMercenary(int mercId, bool selected)
  {
    foreach (LettuceMercenaryDataModel mercenary in this.SelectedTeamDataModel.MercenaryList)
      mercenary.MercenarySelected = mercenary.MercenaryId == mercId && selected;
  }

  public void ChangeMercenaryArtVariation(int mercId, LettuceMercenary.ArtVariation artVariation)
  {
    foreach (LettuceMercenaryDataModel mercenary in this.SelectedTeamDataModel.MercenaryList)
    {
      if (mercenary.MercenaryId == mercId)
      {
        CollectionUtils.PopulateMercenaryCardDataModel(mercenary, artVariation);
        break;
      }
    }
  }

  public void UpdateTeamCompleteHighlight()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    LettuceTeam editingTeam = collectionManager.GetEditingTeam();
    if (editingTeam == null)
      return;
    bool flag = editingTeam.GetMercCount() == collectionManager.GetTeamSize();
    this.m_teamCompleteHighlight.SetActive(flag);
    if (!((UnityEngine.Object) this.m_doneButtonPlayMaker != (UnityEngine.Object) null))
      return;
    string activeStateName = this.m_doneButtonPlayMaker.ActiveStateName;
    if (!(activeStateName == "Deck_incomplete Idle"))
    {
      if (!(activeStateName == "Deck_complete Idle") || flag)
        return;
      this.m_doneButtonPlayMaker.SendEvent("Deck_incomplete");
    }
    else
    {
      if (!flag)
        return;
      this.m_doneButtonPlayMaker.SendEvent("Deck_complete");
    }
  }

  public LettuceTeamDataModel SelectedTeamDataModel
  {
    get
    {
      if (this.m_selectedTeamDataModel == null)
      {
        if ((UnityEngine.Object) this.m_mercLoadoutVisualController == (UnityEngine.Object) null)
          return (LettuceTeamDataModel) null;
        Widget owner = (Widget) this.m_mercLoadoutVisualController.Owner;
        if (!owner.GetDataModel(217, out IDataModel _))
        {
          IDataModel dataModel = (IDataModel) new LettuceTeamDataModel();
          owner.BindDataModel(dataModel);
          this.m_selectedTeamDataModel = dataModel as LettuceTeamDataModel;
        }
      }
      return this.m_selectedTeamDataModel;
    }
  }

  public override void StartDragToReorder(IDraggableCollectionVisual draggingDeckBox)
  {
    base.StartDragToReorder(draggingDeckBox);
    this.AllowMercReordering();
  }

  public override void StopDragToReorder()
  {
    base.StopDragToReorder();
    this.IsReorderingAllowed = false;
  }

  private void AllowMercReordering()
  {
    this.m_scrollbar.StopScroll();
    this.IsReorderingAllowed = true;
  }

  private void MercLoadoutEventListener(string eventName)
  {
    if (!(eventName == "MERC_LOADOUT_RELEASED"))
    {
      if (eventName == "TEAM_MERC_drag_started")
      {
        if (this.IsReorderingAllowed)
        {
          this.OnMercDragStarted();
        }
        else
        {
          if ((double) Vector3.Dot(Vector3.left, PegUI.Get().GetDragDelta().normalized) <= (double) this.m_dragRemoveDirectionSensitivity)
            return;
          this.AllowMercReordering();
          this.OnMercDragStarted();
        }
      }
      else
      {
        LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
        if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
          Log.Lettuce.PrintWarning("DeckTrayMercListContent.MercLoadoutEventListener - LettuceCollectionDisplay is null!");
        else
          collectibleDisplay.HandleTileHoverEvents(eventName, this.m_mercLoadoutVisualController);
      }
    }
    else
      this.OnMercReleased();
  }

  private void HideHoverCards()
  {
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
      Log.Lettuce.PrintWarning("DeckTrayMercListContent.HideHoverCards - LettuceCollectionDisplay is null!");
    else
      collectibleDisplay.HideHoverCards();
  }

  private void OnMercReleased()
  {
    this.ResetReoderingState();
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintWarning("DeckTrayMercListContent.OnMercReleased - LettuceCollectionDisplay is null!");
    }
    else
    {
      collectibleDisplay.HideHoverCards();
      LettuceMercenaryDataModel payload = WidgetUtils.GetEventDataModel(this.m_mercLoadoutVisualController).Payload as LettuceMercenaryDataModel;
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) payload.MercenaryId);
      if (payload.IsDisabled)
        this.RemoveMerc(payload.MercenaryId, true);
      else if (collectibleDisplay.IsMercenaryDetailsDisplayActive() && collectibleDisplay.GetMercenaryDetailsDisplay().GetCurrentlyDisplayedMercenary().ID == mercenary.ID)
      {
        collectibleDisplay.GetMercenaryDetailsDisplay().HideHelpPopups();
        Navigation.GoBack();
      }
      else
        collectibleDisplay.ShowMercenaryDetailsDisplay(mercenary);
    }
  }

  private void OnMercDragStarted()
  {
    this.HideHoverCards();
    LettuceMercenaryDataModel payload = WidgetUtils.GetEventDataModel(this.m_mercLoadoutVisualController).Payload as LettuceMercenaryDataModel;
    if (!this.CanPickupCard() || !CollectionInputMgr.Get().GrabMercenariesModeCard((IDataModel) payload, CollectionUtils.MercenariesModeCardType.Mercenary, (InputMgr.OnCardDroppedCallback) null))
      return;
    this.m_draggingDeckBox = (IDraggableCollectionVisual) null;
    this.RemoveMerc(payload.MercenaryId, true);
  }

  private IEnumerator InitializeWhenReady()
  {
    DeckTrayMercListContent trayMercListContent = this;
    while (!trayMercListContent.IsContentLoaded())
      yield return (object) null;
  }

  private bool CanPickupCard() => !this.ShouldIgnoreAllInput() && CollectionDeckTray.Get().GetCurrentContentType() == DeckTray.DeckContentTypes.Mercs && CollectionDeckTray.Get().CanPickupCard();

  private bool ShouldIgnoreAllInput() => (UnityEngine.Object) CollectionInputMgr.Get() != (UnityEngine.Object) null && CollectionInputMgr.Get().IsDraggingScrollbar() || (UnityEngine.Object) CraftingManager.Get() != (UnityEngine.Object) null && CraftingManager.Get().IsCardShowing() || CollectionManager.Get().GetCollectibleDisplay().GetPageManager().ArePagesTurning();

  private float ScrollHeightCallback() => 0.0f;

  private Vector3 GetOffscreenLocalPosition()
  {
    Vector3 originalLocalPosition = this.m_originalLocalPosition;
    CollectionManager.Get().GetEditingTeam()?.GetMercCount();
    originalLocalPosition.z -= 100f;
    return originalLocalPosition;
  }

  private void ResetReoderingState()
  {
    this.IsReorderingAllowed = false;
    this.m_draggingDeckBox = (IDraggableCollectionVisual) null;
  }

  public void RegisterMercCountUpdated(DeckTrayMercListContent.MercCountChanged dlg) => this.m_mercCountChangedListeners.Add(dlg);

  public void UnregisterMercCountUpdated(DeckTrayMercListContent.MercCountChanged dlg) => this.m_mercCountChangedListeners.Remove(dlg);

  private void FireMercCountChangedEvent()
  {
    DeckTrayMercListContent.MercCountChanged[] array = this.m_mercCountChangedListeners.ToArray();
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    int mercCount = 0;
    if (editingTeam != null)
      mercCount = editingTeam.GetMercCount();
    foreach (DeckTrayMercListContent.MercCountChanged mercCountChanged in array)
      mercCountChanged(mercCount);
  }

  public delegate void MercCountChanged(int mercCount);
}
