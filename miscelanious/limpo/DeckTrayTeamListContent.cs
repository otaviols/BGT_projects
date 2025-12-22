using Blizzard.T5.MaterialService.Extensions;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class DeckTrayTeamListContent : DeckTrayReorderableContent
{
  [CustomEditField(Sections = "Team Tray Settings")]
  public Transform m_teamEditTopPos;
  [CustomEditField(Sections = "Team Tray Settings")]
  public Transform m_traySectionStartPos;
  [CustomEditField(Sections = "Team Tray Settings")]
  public GameObject m_teamInfoTooltipBone;
  [CustomEditField(Sections = "Team Tray Settings")]
  public GameObject m_teamOptionsBone;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_teamOptionsPrefab;
  [CustomEditField(Sections = "Prefabs")]
  public TraySection m_traySectionPrefab;
  [CustomEditField(Sections = "Prefabs")]
  public DeckTray m_deckTray;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_teamInfoActorPrefab;
  [CustomEditField(Sections = "Team Button Settings")]
  public ParticleSystem m_deleteTeamPoof;
  [CustomEditField(Sections = "Team Button Settings")]
  public Vector3 m_deleteTeamPoofVisualOffset;
  [SerializeField]
  private Vector3 m_teamButtonOffset;
  [CustomEditField(Sections = "Team Button Settings")]
  public GameObject m_newTeamButtonContainer;
  [CustomEditField(Sections = "Team Button Settings")]
  public CollectionDeckTrayButton m_newTeamButton;
  protected const float TIME_BETWEEN_TRAY_DOOR_ANIMS = 0.015f;
  protected const int MAX_NUM_DECKBOXES_AVAILABLE = 9;
  protected const int NUM_DECKBOXES_TO_DISPLAY = 11;
  protected static readonly Vector3 DELETE_DECKBOX_POSITION_OFFSET = Vector3.down;
  protected CollectionTeamInfo m_teamInfoTooltip;
  protected List<TraySection> m_traySections = new List<TraySection>();
  protected TraySection m_editingTraySection;
  protected int m_centeringTeamList = -1;
  protected TeamOptionsMenu m_teamOptionsMenu;
  protected bool m_initialized;
  protected bool m_animatingExit;
  protected bool m_doneEntering;
  private bool m_wasTouchModeEnabled;
  private List<DeckTrayTeamListContent.TeamCountChanged> m_teamCountChangedListeners = new List<DeckTrayTeamListContent.TeamCountChanged>();
  private List<DeckTrayTeamListContent.BusyWithTeam> m_busyWithTeamListeners = new List<DeckTrayTeamListContent.BusyWithTeam>();
  protected string m_previousTeamName;
  protected const float DELETE_TEAM_ANIM_TIME = 0.5f;
  protected bool m_deletingTeams;
  protected bool m_waitingToDeleteTeam;
  protected List<LettuceTeam> m_teamsToDelete = new List<LettuceTeam>();
  protected TraySection m_newlyCreatedTraySection;
  private ScreenEffectsHandle m_screenEffectsHandle;
  private const float TRAY_MATERIAL_Y_OFFSET = -0.0825f;

  [CustomEditField(Sections = "Team Button Settings")]
  public Vector3 TeamButtonOffset
  {
    set
    {
      this.m_teamButtonOffset = value;
      this.UpdateNewTeamButton();
    }
    get => this.m_teamButtonOffset;
  }

  protected override void Awake()
  {
    base.Awake();
    CollectionManager.Get();
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.WillReset += new Action(this.WillReset);
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected void Update()
  {
    this.UpdateDragToReorder();
    if (this.m_wasTouchModeEnabled == UniversalInputManager.Get().IsTouchMode())
      return;
    this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
    if (!UniversalInputManager.Get().IsTouchMode() || !((UnityEngine.Object) this.m_teamInfoTooltip != (UnityEngine.Object) null))
      return;
    this.HideTeamInfo();
  }

  protected override void OnDestroy()
  {
    CollectionManager.Get().RemoveTeamDeletedListener(new CollectionManager.DelOnTeamDeleted(this.OnTeamDeleted));
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.WillReset -= new Action(this.WillReset);
    if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null)
      Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    base.OnDestroy();
  }

  protected void Initialize()
  {
    if (this.m_initialized)
      return;
    this.m_newTeamButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnNewTeamButtonPress()));
    CollectionManager.Get().RegisterTeamDeletedListener(new CollectionManager.DelOnTeamDeleted(this.OnTeamDeleted));
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_teamInfoActorPrefab, AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("Unable to load actor {0}: null", (object) this.m_teamInfoActorPrefab), (UnityEngine.Object) this.gameObject);
    }
    else
    {
      this.m_teamInfoTooltip = gameObject.GetComponent<CollectionTeamInfo>();
      if ((UnityEngine.Object) this.m_teamInfoTooltip == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("Actor {0} does not contain CollectionDeckInfo component.", (object) this.m_teamInfoActorPrefab), (UnityEngine.Object) this.gameObject);
      }
      else
      {
        GameUtils.SetParent((Component) this.m_teamInfoTooltip, this.m_teamInfoTooltipBone);
        this.m_teamInfoTooltip.RegisterHideListener(new CollectionTeamInfo.HideListener(this.HideTeamInfoListener));
        this.m_teamOptionsMenu = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_teamOptionsPrefab).GetComponent<TeamOptionsMenu>();
        GameUtils.SetParent(this.m_teamOptionsMenu.gameObject, this.m_teamOptionsBone);
        this.m_teamOptionsMenu.SetTeamInfo(this.m_teamInfoTooltip);
        this.HideTeamInfo();
        this.CreateTraySections();
        this.m_initialized = true;
      }
    }
  }

  protected void HideTeamInfoListener()
  {
    if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
    {
      LayerUtils.SetLayer(this.m_editingTraySection.m_deckBox.gameObject, GameLayer.Default);
      LayerUtils.SetLayer(this.m_teamOptionsMenu.gameObject, GameLayer.Default);
      this.m_editingTraySection.m_deckBox.HideRenameVisuals();
    }
    this.m_screenEffectsHandle.StopEffect(0.25f);
    if (UniversalInputManager.Get().IsTouchMode())
    {
      if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      {
        this.m_editingTraySection.m_deckBox.SetHighlightState(ActorStateType.NONE);
        this.m_editingTraySection.m_deckBox.ShowDeckName();
      }
      this.FinishRenamingEditingDeck();
    }
    this.m_teamOptionsMenu.Hide();
  }

  protected virtual void ShowTeamInfo()
  {
    if (!UniversalInputManager.Get().IsTouchMode() && (UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      this.m_editingTraySection.m_deckBox.ShowRenameVisuals();
    LayerUtils.SetLayer(this.m_editingTraySection.m_deckBox.gameObject, GameLayer.IgnoreFullScreenEffects);
    LayerUtils.SetLayer(this.m_teamInfoTooltip.gameObject, GameLayer.IgnoreFullScreenEffects);
    LayerUtils.SetLayer(this.m_teamOptionsMenu.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.DesaturatePerspective with
    {
      Time = 0.25f
    });
    this.m_teamInfoTooltip.Show();
    this.m_teamOptionsMenu.Show();
  }

  protected void HideTeamInfo() => this.m_teamInfoTooltip.Hide();

  public bool IsShowingTeamOptions => (UnityEngine.Object) this.m_teamOptionsMenu != (UnityEngine.Object) null && this.m_teamOptionsMenu.IsShown;

  private void FinishRenamingEditingDeck(string newDeckName = null)
  {
    if ((UnityEngine.Object) this.m_editingTraySection == (UnityEngine.Object) null)
      return;
    CollectionDeckBoxVisual deckBox = this.m_editingTraySection.m_deckBox;
    CollectionDeck collectionDeck = this.UpdateRenamingEditingDeck(newDeckName);
    if (collectionDeck != null && (UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      deckBox.SetDeckName(collectionDeck.Name);
    if (UniversalInputManager.Get() != null && UniversalInputManager.Get().IsTextInputActive())
      UniversalInputManager.Get().CancelTextInput(this.gameObject);
    deckBox.ShowDeckName();
  }

  private CollectionDeck UpdateRenamingEditingDeck(string newDeckName)
  {
    CollectionDeck editingDeck = this.m_deckTray.GetCardsContent().GetEditingDeck();
    if (editingDeck != null && !string.IsNullOrEmpty(newDeckName))
      editingDeck.Name = newDeckName;
    return editingDeck;
  }

  private void WillReset()
  {
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.BeginAnimation));
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.EndAnimation));
  }

  private void OnNewTeamButtonPress()
  {
    if (!this.IsModeActive() || this.IsTouchDragging)
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "Hub_Click.prefab:cc2cf2b5507827149b13d12210c0f323");
    this.StartCoroutine(this.StartCreateNewTeam());
  }

  protected IEnumerator StartCreateNewTeam()
  {
    this.ShowNewTeamButton(false, new float?(), disableOnHide: false);
    this.CreateNewTeam();
    if ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() != (UnityEngine.Object) null)
      CollectionManager.Get().GetCollectibleDisplay().EnableInput(true);
    CollectionDeckTray deckTray = CollectionDeckTray.Get();
    while ((UnityEngine.Object) deckTray != (UnityEngine.Object) null && deckTray.IsUpdatingTrayMode())
      yield return (object) null;
    if ((UnityEngine.Object) deckTray != (UnityEngine.Object) null)
      deckTray.m_doneButton.SetEnabled(true);
  }

  protected void EndCreateNewTeam(bool newTeam) => this.ShowNewTeamButton(true, (CollectionDeckTrayButton.DelOnAnimationFinished) (o =>
  {
    if (!newTeam)
      return;
    this.UpdateAllTrays(true);
  }));

  private void DeleteQueuedTeams(bool force = false)
  {
    if (this.m_teamsToDelete.Count == 0 || !this.IsModeActive() && !force)
      return;
    foreach (LettuceTeam lettuceTeam in this.m_teamsToDelete)
    {
      Network.Get().DeleteTeam(lettuceTeam.ID);
      CollectionManager.Get().AddPendingTeamDelete(lettuceTeam.ID);
      if (!Network.IsLoggedIn() || lettuceTeam.ID <= 0L)
        CollectionManager.Get().OnTeamDeletedWhileOffline(lettuceTeam.ID);
    }
    this.m_teamsToDelete.Clear();
  }

  private void OnTeamDeleted(LettuceTeam removedTeam)
  {
    if (removedTeam == null)
      return;
    this.m_waitingToDeleteTeam = false;
    this.StartCoroutine(this.DeleteTeamAnimation(removedTeam.ID));
  }

  public override void OnEditingTeamChanged(
    LettuceTeam newTeam,
    LettuceTeam oldTeam,
    bool isNewTeam)
  {
    if (newTeam != null && (UnityEngine.Object) this.m_teamInfoTooltip != (UnityEngine.Object) null && (UnityEngine.Object) this.m_teamOptionsMenu != (UnityEngine.Object) null)
      this.m_teamOptionsMenu.SetTeam(newTeam);
    if (isNewTeam && newTeam != null)
      this.InitializeSortOrderFromTraysIfNeeded();
    if (this.IsModeActive())
      this.InitializeTraysFromTeams();
    if (isNewTeam && newTeam != null)
    {
      CollectionUtils.PopulateMercenariesTeamDataModel(new LettuceTeamDataModel(), newTeam);
      this.m_newlyCreatedTraySection = this.GetExistingTrayFromTeam(newTeam);
      if ((UnityEngine.Object) this.m_newlyCreatedTraySection != (UnityEngine.Object) null)
        this.m_centeringTeamList = this.m_newlyCreatedTraySection.m_deckBox.GetPositionIndex();
      newTeam.SendChanges();
    }
    List<LettuceMercenary> lettuceMercenaryList = (List<LettuceMercenary>) null;
    if (newTeam == null && oldTeam != null)
      lettuceMercenaryList = oldTeam.GetMercs();
    else if (newTeam != null)
      lettuceMercenaryList = newTeam.GetMercs();
    if (lettuceMercenaryList == null)
      return;
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    LettuceCollectionPageManager pageManager = collectibleDisplay.GetPageManager() as LettuceCollectionPageManager;
    if (!((UnityEngine.Object) pageManager != (UnityEngine.Object) null))
      return;
    foreach (LettuceMercenary mercenary in lettuceMercenaryList)
    {
      pageManager.UpdatePageMercenary(MercenaryFactory.CreateMercenaryDataModelWithCoin(mercenary));
      pageManager.UpdateCurrentPageCardLocks(false);
    }
  }

  public void UpdateEditingTeamBoxVisual(string mercCardId, TAG_PREMIUM? premiumOverride = null)
  {
    if ((UnityEngine.Object) this.m_editingTraySection == (UnityEngine.Object) null)
      return;
    this.m_editingTraySection.m_deckBox.SetHeroCardPremiumOverride(premiumOverride);
    this.m_editingTraySection.m_deckBox.SetHeroCardID(mercCardId);
  }

  private IEnumerator DeleteTeamAnimation(long teamID, Action callback = null)
  {
    DeckTrayTeamListContent trayTeamListContent = this;
    while (trayTeamListContent.m_deletingTeams)
      yield return (object) null;
    int index1 = 0;
    TraySection delTraySection = (TraySection) null;
    TraySection setNewTeamButtonPosition = trayTeamListContent.m_traySections[0];
    for (int index2 = 0; index2 < trayTeamListContent.m_traySections.Count; ++index2)
    {
      TraySection traySection = trayTeamListContent.m_traySections[index2];
      long deckId = traySection.m_deckBox.GetDeckID();
      if (deckId == teamID)
      {
        index1 = index2;
        delTraySection = traySection;
      }
      else if (deckId == -1L)
        break;
      setNewTeamButtonPosition = traySection;
    }
    if ((UnityEngine.Object) delTraySection == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "Unable to delete team with ID {0}. Not found in tray sections.", (UnityEngine.Object) trayTeamListContent.gameObject);
    }
    else
    {
      trayTeamListContent.FireBusyWithTeamEvent(true);
      trayTeamListContent.m_deletingTeams = true;
      trayTeamListContent.FireTeamCountChangedEvent();
      trayTeamListContent.m_traySections.RemoveAt(index1);
      bool newTeamBtnActive;
      Vector3 outPosition;
      trayTeamListContent.GetIdealNewTeamButtonLocalPosition(setNewTeamButtonPosition, out outPosition, out newTeamBtnActive);
      Vector3 vector3 = delTraySection.transform.localPosition;
      SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_delete_deck.prefab:5ca16bec63041b741a4fb33706ed9cb1", trayTeamListContent.gameObject);
      trayTeamListContent.m_deleteTeamPoof.transform.position = delTraySection.m_deckBox.transform.position + trayTeamListContent.m_deleteTeamPoofVisualOffset;
      trayTeamListContent.m_deleteTeamPoof.Play(true);
      delTraySection.ClearDeckInfo();
      delTraySection.gameObject.SetActive(false);
      List<GameObject> animatingTraySections = new List<GameObject>();
      Action<object> action = (Action<object>) (obj => animatingTraySections.Remove((GameObject) obj));
      Vector3 delTraySectionPosition = Vector3.zero;
      for (int index3 = index1; index3 < trayTeamListContent.m_traySections.Count; ++index3)
      {
        TraySection traySection = trayTeamListContent.m_traySections[index3];
        Vector3 localPosition = traySection.transform.localPosition;
        iTween.MoveTo(traySection.gameObject, iTween.Hash((object) "position", (object) vector3, (object) "isLocal", (object) true, (object) "time", (object) 0.5f, (object) "easeType", (object) iTween.EaseType.easeOutBounce, (object) "oncomplete", (object) action, (object) "oncompleteparams", (object) traySection.gameObject, (object) "name", (object) "position"));
        animatingTraySections.Add(traySection.gameObject);
        if (index3 <= 7)
          delTraySectionPosition = localPosition;
        vector3 = localPosition;
      }
      if (index1 == 8)
        delTraySectionPosition = delTraySection.transform.localPosition;
      trayTeamListContent.m_traySections.Insert(8, delTraySection);
      trayTeamListContent.m_newTeamButton.SetIsUsable(trayTeamListContent.CanShowNewTeamButton());
      delTraySection.gameObject.SetActive(true);
      delTraySection.HideDeckBox(true);
      delTraySection.transform.localPosition = DeckTrayTeamListContent.DELETE_DECKBOX_POSITION_OFFSET + delTraySectionPosition;
      if (trayTeamListContent.m_newTeamButton.gameObject.activeSelf)
      {
        iTween.MoveTo(trayTeamListContent.m_newTeamButtonContainer, iTween.Hash((object) "position", (object) outPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.5f, (object) "easeType", (object) iTween.EaseType.easeOutBounce, (object) "oncomplete", (object) action, (object) "oncompleteparams", (object) trayTeamListContent.m_newTeamButtonContainer, (object) "name", (object) "position"));
        animatingTraySections.Add(trayTeamListContent.m_newTeamButtonContainer);
      }
      else
        trayTeamListContent.m_newTeamButtonContainer.transform.localPosition = outPosition;
      while (animatingTraySections.Count > 0)
      {
        animatingTraySections.RemoveAll((Predicate<GameObject>) (obj => (UnityEngine.Object) obj == (UnityEngine.Object) null || !obj.activeInHierarchy));
        yield return (object) null;
      }
      delTraySection.transform.localPosition = delTraySectionPosition;
      if (!CollectionManager.Get().IsInEditTeamMode())
        trayTeamListContent.ShowNewTeamButton(newTeamBtnActive);
      trayTeamListContent.FireBusyWithTeamEvent(false);
      if (callback != null)
        callback();
      trayTeamListContent.m_deletingTeams = false;
    }
  }

  private void UpdateNewTeamButton(TraySection setNewTeamButtonPosition = null) => this.ShowNewTeamButton(this.UpdateNewTeamButtonPosition(setNewTeamButtonPosition) && this.CanShowNewTeamButton());

  private bool UpdateNewTeamButtonPosition(TraySection setNewTeamButtonPosition = null)
  {
    bool outActive = false;
    Vector3 outPosition;
    this.GetIdealNewTeamButtonLocalPosition(setNewTeamButtonPosition, out outPosition, out outActive);
    this.m_newTeamButtonContainer.transform.localPosition = outPosition;
    return outActive;
  }

  private void GetIdealNewTeamButtonLocalPosition(
    TraySection setNewTeamButtonPosition,
    out Vector3 outPosition,
    out bool outActive)
  {
    TraySection unusedTraySection = this.GetLastUnusedTraySection();
    TraySection traySection = (UnityEngine.Object) setNewTeamButtonPosition == (UnityEngine.Object) null ? unusedTraySection : setNewTeamButtonPosition;
    outActive = (UnityEngine.Object) unusedTraySection != (UnityEngine.Object) null;
    outPosition = ((UnityEngine.Object) traySection != (UnityEngine.Object) null ? traySection.transform.localPosition : this.m_traySectionStartPos.localPosition) + this.m_teamButtonOffset;
  }

  public void ShowNewTeamButton(
    bool newTeamButtonActive,
    CollectionDeckTrayButton.DelOnAnimationFinished callback = null)
  {
    this.ShowNewTeamButton(newTeamButtonActive, new float?(), callback);
  }

  public void ShowNewTeamButton(
    bool newTeamButtonActive,
    float? speed,
    CollectionDeckTrayButton.DelOnAnimationFinished callback = null,
    bool disableOnHide = true)
  {
    if (this.m_newTeamButton.IsPoppedUp() != newTeamButtonActive)
    {
      if (newTeamButtonActive)
      {
        this.m_newTeamButton.gameObject.SetActive(true);
        this.m_newTeamButton.PlayPopUpAnimation((CollectionDeckTrayButton.DelOnAnimationFinished) (o =>
        {
          if (callback == null)
            return;
          callback((object) this);
        }), (object) null, speed);
      }
      else
        this.m_newTeamButton.PlayPopDownAnimation((CollectionDeckTrayButton.DelOnAnimationFinished) (o =>
        {
          if (disableOnHide)
            this.m_newTeamButton.gameObject.SetActive(false);
          CollectionDeckTrayButton.DelOnAnimationFinished animationFinished = callback;
          if (animationFinished == null)
            return;
          animationFinished((object) this);
        }), (object) null, speed);
    }
    else
    {
      if (callback == null)
        return;
      callback((object) this);
    }
  }

  public bool IsDoneEntering() => this.m_doneEntering;

  public IEnumerator ShowTrayDoors(bool show)
  {
    foreach (TraySection traySection in this.m_traySections)
    {
      traySection.EnableDoors(show);
      traySection.ShowDoor(show);
    }
    yield return (object) null;
  }

  public override bool AnimateContentEntranceStart()
  {
    this.Initialize();
    long editTeamID = -1;
    if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      editTeamID = this.m_editingTraySection.m_deckBox.GetDeckID();
    this.InitializeTraysFromTeams();
    this.SwapEditTrayIfNeeded(editTeamID);
    this.UpdateAllTrays(SceneMgr.Get().IsInTavernBrawlMode(), false);
    if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
    {
      this.FinishRenamingEditingTeam();
      this.m_editingTraySection.MoveDeckBoxBackToOriginalPosition(0.25f, (TraySection.DelOnDoorStateChangedCallback) (o => this.m_editingTraySection = (TraySection) null));
    }
    this.m_newTeamButton.SetIsUsable(this.CanShowNewTeamButton());
    this.FireBusyWithTeamEvent(true);
    this.FireTeamCountChangedEvent();
    CollectionManager.Get().DoneEditing();
    return true;
  }

  public override bool AnimateContentEntranceEnd()
  {
    if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      return false;
    this.m_newTeamButton.SetEnabled(true);
    this.FireBusyWithTeamEvent(false);
    this.DeleteQueuedTeams(true);
    return true;
  }

  public override bool AnimateContentExitStart()
  {
    this.m_animatingExit = true;
    this.FireBusyWithTeamEvent(true);
    float? speed = new float?();
    if (SceneMgr.Get().IsInTavernBrawlMode())
      speed = new float?(500f);
    if ((UnityEngine.Object) this.m_newlyCreatedTraySection == (UnityEngine.Object) null)
      this.ShowNewTeamButton(false, speed);
    Processor.ScheduleCallback(0.5f, false, new Processor.ScheduledCallback(this.BeginAnimation));
    return true;
  }

  public override bool AnimateContentExitEnd() => !this.m_animatingExit;

  public override bool PreAnimateContentExit()
  {
    if ((UnityEngine.Object) this.m_scrollbar == (UnityEngine.Object) null)
      return true;
    if (this.m_centeringTeamList != -1 && (UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
    {
      BoxCollider component = this.m_editingTraySection.m_deckBox.GetComponent<BoxCollider>();
      if (this.m_scrollbar.ScrollObjectIntoView(this.m_editingTraySection.m_deckBox.gameObject, component.center.y, component.size.y / 2f, (UIBScrollable.OnScrollComplete) (f => this.m_animatingExit = false), iTween.EaseType.linear, this.m_scrollbar.m_ScrollTweenTime, true))
      {
        this.m_animatingExit = true;
        this.m_centeringTeamList = -1;
      }
    }
    this.StartCoroutine(this.ShowTrayDoors(false));
    return !this.m_animatingExit;
  }

  public override bool PreAnimateContentEntrance()
  {
    this.m_doneEntering = false;
    this.StartCoroutine(this.ShowTrayDoors(false));
    return true;
  }

  private void BeginAnimation(object userData)
  {
    float secondsToWait = 0.5f;
    foreach (TraySection traySection in this.m_traySections)
    {
      if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) traySection)
        traySection.HideDeckBox();
    }
    if ((UnityEngine.Object) this.m_newlyCreatedTraySection != (UnityEngine.Object) null)
    {
      TraySection animateTraySection = this.m_newlyCreatedTraySection;
      this.UpdateNewTeamButtonPosition(animateTraySection);
      this.ShowNewTeamButton(true, (CollectionDeckTrayButton.DelOnAnimationFinished) (_1 => this.m_newTeamButton.FlipHalfOverAndHide(0.1f, (CollectionDeckTrayButton.DelOnAnimationFinished) (_2 =>
      {
        animateTraySection.ShowDeckBox(true);
        animateTraySection.FlipDeckBoxHalfOverToShow(0.1f, (TraySection.DelOnDoorStateChangedCallback) (_3 => animateTraySection.MoveDeckBoxToEditPosition(this.m_teamEditTopPos.position, 0.25f)));
      }))));
      this.m_editingTraySection = this.m_newlyCreatedTraySection;
      this.m_newlyCreatedTraySection = (TraySection) null;
      secondsToWait += 0.7f;
    }
    else if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      this.m_editingTraySection.MoveDeckBoxToEditPosition(this.m_teamEditTopPos.position, 0.25f);
    Processor.ScheduleCallback(secondsToWait, false, new Processor.ScheduledCallback(this.EndAnimation));
  }

  private void EndAnimation(object userData)
  {
    this.m_animatingExit = false;
    this.FireBusyWithTeamEvent(false);
  }

  private LettuceTeam UpdateRenamingEditingTeam(string newTeamName)
  {
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam != null && !string.IsNullOrEmpty(newTeamName))
      editingTeam.Name = newTeamName;
    return editingTeam;
  }

  private void FinishRenamingEditingTeam(string newTeamName = null)
  {
    if ((UnityEngine.Object) this.m_editingTraySection == (UnityEngine.Object) null)
      return;
    CollectionDeckBoxVisual deckBox = this.m_editingTraySection.m_deckBox;
    LettuceTeam lettuceTeam = this.UpdateRenamingEditingTeam(newTeamName);
    if (lettuceTeam != null && (UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      deckBox.SetDeckName(lettuceTeam.Name);
    if (UniversalInputManager.Get() != null && UniversalInputManager.Get().IsTextInputActive())
      UniversalInputManager.Get().CancelTextInput(this.gameObject);
    deckBox.ShowDeckName();
  }

  private void OnDrawGizmos()
  {
    if ((UnityEngine.Object) this.m_editingTraySection == (UnityEngine.Object) null)
      return;
    Bounds bounds = this.m_editingTraySection.m_deckBox.GetDeckNameText().GetBounds();
    Gizmos.DrawWireSphere(bounds.min, 0.1f);
    Gizmos.DrawWireSphere(bounds.max, 0.1f);
  }

  public void RegisterTeamCountUpdated(DeckTrayTeamListContent.TeamCountChanged dlg) => this.m_teamCountChangedListeners.Add(dlg);

  public void UnregisterTeamCountUpdated(DeckTrayTeamListContent.TeamCountChanged dlg) => this.m_teamCountChangedListeners.Remove(dlg);

  public void RegisterBusyWithTeam(DeckTrayTeamListContent.BusyWithTeam dlg) => this.m_busyWithTeamListeners.Add(dlg);

  public void UnregisterBusyWithTeam(DeckTrayTeamListContent.BusyWithTeam dlg) => this.m_busyWithTeamListeners.Remove(dlg);

  public virtual void HideTraySectionsNotInBounds(Bounds bounds)
  {
    int num = 0;
    foreach (TraySection traySection in this.m_traySections)
    {
      if (traySection.HideIfNotInBounds(bounds))
        ++num;
    }
    Log.DeckTray.Print("Hid {0} tray sections that were not visible.", (object) num);
    UIBScrollableItem component = this.m_newTeamButtonContainer.GetComponent<UIBScrollableItem>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "UIBScrollableItem not found on m_newDeckButtonContainer! This button may not be hidden properly while exiting Collection Manager!");
    }
    else
    {
      Bounds bounds1 = new Bounds();
      Vector3 min;
      Vector3 max;
      component.GetWorldBounds(out min, out max);
      bounds1.SetMinMax(min, max);
      if (bounds.Intersects(bounds1))
        return;
      Log.DeckTray.Print("Hiding the New Deck button because it's out of the visible scroll area.");
      this.m_newTeamButton.gameObject.SetActive(false);
    }
  }

  public void CreateNewTeam(string customTeamName = null, string pastedTeamHashString = null)
  {
    PegasusLettuce.LettuceTeam.Type type = PegasusLettuce.LettuceTeam.Type.TYPE_SOLO;
    string name = customTeamName;
    if (string.IsNullOrEmpty(name))
      name = CollectionManager.Get().AutoGenerateTeamName();
    CollectionManager.Get().SendCreateTeam(name, type, pastedTeamHashString);
  }

  public void CreateNewTeamCancelled() => this.EndCreateNewTeam(false);

  public bool IsWaitingToDeleteTeam() => this.m_waitingToDeleteTeam;

  public int NumTeamsToDelete() => this.m_teamsToDelete.Count;

  public bool IsDeletingTeams() => this.m_deletingTeams;

  public void DeleteTeam(long teamID)
  {
    LettuceTeam team = CollectionManager.Get().GetTeam(teamID);
    if (team == null)
    {
      Log.All.PrintError("Unable to delete team id={0} - not found in cache.", (object) teamID);
    }
    else
    {
      if (Network.IsLoggedIn() && teamID <= 0L)
        Log.Offline.PrintDebug("DeleteTeam() - Attempting to delete fake team while online.");
      team.MarkBeingDeleted();
      this.m_teamsToDelete.Add(team);
      if (PlatformSettings.IsMobile())
      {
        LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
        if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.IsMercenaryDetailsDisplayActive())
          Navigation.GoBack();
        Navigation.GoBack();
      }
      this.DeleteQueuedTeams();
    }
  }

  public void DeleteEditingTeam()
  {
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam == null)
    {
      Debug.LogWarning((object) "No team currently being edited!");
    }
    else
    {
      this.m_waitingToDeleteTeam = true;
      this.DeleteTeam(editingTeam.ID);
    }
  }

  public void CancelRenameEditingTeam() => this.FinishRenamingEditingTeam();

  public Vector3 GetNewTeamButtonPosition() => this.m_newTeamButton.transform.localPosition;

  public void UpdateTeamName(string teamName = null)
  {
    if (teamName == null)
    {
      LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
      if (editingTeam == null)
        return;
      teamName = editingTeam.Name;
    }
    this.FinishRenamingEditingTeam(teamName);
  }

  public void RenameCurrentlyEditingTeam()
  {
    if ((UnityEngine.Object) this.m_editingTraySection == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "Unable to rename team. No team currently being edited.", (UnityEngine.Object) this.gameObject);
    }
    else
    {
      CollectionDeckBoxVisual deckBox = this.m_editingTraySection.m_deckBox;
      deckBox.HideDeckName();
      Camera camera = Box.Get().GetCamera();
      Bounds bounds = deckBox.GetDeckNameText().GetBounds();
      Vector3 min = bounds.min;
      Vector3 max = bounds.max;
      Rect guiViewportRect = CameraUtils.CreateGUIViewportRect(camera, min, max);
      Font localizedFont = deckBox.GetDeckNameText().GetLocalizedFont();
      this.m_previousTeamName = deckBox.GetDeckNameText().Text;
      UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams()
      {
        m_owner = this.gameObject,
        m_rect = guiViewportRect,
        m_updatedCallback = (UniversalInputManager.TextInputUpdatedCallback) (newName => this.UpdateRenamingEditingTeam(newName)),
        m_completedCallback = (UniversalInputManager.TextInputCompletedCallback) (newName => this.FinishRenamingEditingTeam(newName)),
        m_canceledCallback = (UniversalInputManager.TextInputCanceledCallback) ((a1, a2) => this.FinishRenamingEditingTeam(this.m_previousTeamName)),
        m_maxCharacters = CollectionDeck.DefaultMaxDeckNameCharacters,
        m_font = localizedFont,
        m_text = deckBox.GetDeckNameText().Text
      };
      UniversalInputManager.Get().UseTextInput(parms);
    }
  }

  protected void CreateTraySections()
  {
    Vector3 localScale = this.m_traySectionStartPos.localScale;
    Vector3 localEulerAngles = this.m_traySectionStartPos.localEulerAngles;
    for (int idx = 0; idx < 11; ++idx)
    {
      TraySection traySection = (TraySection) GameUtils.Instantiate((Component) this.m_traySectionPrefab, this.gameObject);
      traySection.m_deckBox.SetPositionIndex(idx);
      traySection.transform.localScale = localScale;
      traySection.transform.localEulerAngles = localEulerAngles;
      traySection.EnableDoors(false);
      CollectionDeckBoxVisual deckBox = traySection.m_deckBox;
      deckBox.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.OnDeckBoxVisualOver(deckBox)));
      deckBox.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.OnDeckBoxVisualOut(deckBox)));
      deckBox.AddEventListener(UIEventType.TAP, (UIEvent.Handler) (e => this.OnDeckBoxVisualRelease(traySection)));
      deckBox.StoreOriginalButtonPositionAndRotation();
      deckBox.HideBanner();
      this.m_traySections.Add(traySection);
    }
    this.RefreshTraySectionPositions(false);
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    this.HideTraySectionsNotInBounds(this.m_deckTray.m_scrollbar.m_ScrollBounds.bounds);
    Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
  }

  private void OnBoxTransitionFinished(object userData)
  {
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    foreach (Component traySection in this.m_traySections)
      traySection.gameObject.SetActive(true);
  }

  protected TraySection GetExistingTrayFromTeam(LettuceTeam team) => this.GetExistingTrayFromTeam(team.ID);

  private TraySection GetExistingTrayFromTeam(long teamID)
  {
    foreach (TraySection traySection in this.m_traySections)
    {
      if (traySection.m_deckBox.GetDeckID() == teamID)
        return traySection;
    }
    return (TraySection) null;
  }

  public TraySection GetEditingTraySection() => this.m_editingTraySection;

  protected void InitializeTraysFromTeams()
  {
    this.InitializeSortOrderFromTraysIfNeeded();
    this.UpdateTeamTrayVisuals();
  }

  protected void InitializeSortOrderFromTraysIfNeeded()
  {
    foreach (LettuceTeam team in CollectionManager.Get().GetTeams())
    {
      if (team.SortOrder != 0U)
        return;
    }
    int num = 0;
    for (int index = 0; index < this.m_traySections.Count; ++index)
    {
      TraySection traySection = this.m_traySections[index];
      LettuceTeam team = CollectionManager.Get().GetTeam(traySection.m_deckBox.GetDeckID());
      if (team != null)
      {
        if ((long) team.SortOrder != (long) num)
          team.SortOrder = (uint) num;
        ++num;
      }
    }
  }

  protected void UpdateAllTrays(bool immediate = false, bool initializeTrays = true)
  {
    if (initializeTrays)
      this.InitializeTraysFromTeams();
    List<TraySection> showTraySections = new List<TraySection>();
    foreach (TraySection traySection in this.m_traySections)
    {
      if (traySection.m_deckBox.GetDeckID() == -1L && !traySection.m_deckBox.IsLocked())
        traySection.HideDeckBox(immediate);
      else if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) traySection && !traySection.IsOpen())
        showTraySections.Add(traySection);
    }
    this.StartCoroutine(this.UpdateAllTraysAnimation(showTraySections, immediate));
  }

  protected virtual IEnumerator UpdateAllTraysAnimation(
    List<TraySection> showTraySections,
    bool immediate)
  {
    foreach (TraySection showTraySection in showTraySections)
    {
      showTraySection.ShowDeckBox(immediate);
      if (!immediate)
        yield return (object) new WaitForSeconds(0.015f);
    }
    this.UpdateNewTeamButton();
    this.m_doneEntering = true;
  }

  public TraySection GetLastUnusedTraySection()
  {
    int num = 0;
    foreach (TraySection traySection in this.m_traySections)
    {
      if (num < 9)
      {
        if (traySection.m_deckBox.GetDeckID() == -1L)
          return traySection;
        ++num;
      }
      else
        break;
    }
    return (TraySection) null;
  }

  public TraySection GetLastUsedTraySection()
  {
    int num = 0;
    TraySection lastUsedTraySection = (TraySection) null;
    foreach (TraySection traySection in this.m_traySections)
    {
      if (num < 9)
      {
        if (traySection.m_deckBox.GetDeckID() == -1L)
          return lastUsedTraySection;
        lastUsedTraySection = traySection;
        ++num;
      }
      else
        break;
    }
    return lastUsedTraySection;
  }

  public TraySection GetTraySection(int index) => index >= 0 && index < this.m_traySections.Count ? this.m_traySections[index] : (TraySection) null;

  public bool CanShowNewTeamButton() => CollectionManager.Get().GetTeams().Count < 9 && CollectionDeckTray.Get().GetCurrentContentType() == DeckTray.DeckContentTypes.Teams;

  public void SetEditingTraySection(int index)
  {
    this.m_editingTraySection = this.m_traySections[index];
    this.m_centeringTeamList = this.m_editingTraySection.m_deckBox.GetPositionIndex();
  }

  protected bool IsEditingTeam() => CollectionManager.Get().GetEditingTeam() != null;

  protected virtual void OnDeckBoxVisualOver(CollectionDeckBoxVisual deckBox)
  {
    if (deckBox.IsLocked() || UniversalInputManager.Get().IsTouchMode())
      return;
    if (this.IsEditingTeam() && (UnityEngine.Object) this.m_teamInfoTooltip != (UnityEngine.Object) null)
    {
      this.ShowTeamInfo();
    }
    else
    {
      if (UniversalInputManager.Get().IsTouchMode() || !this.IsModeTryingOrActive() || this.DraggingDeckBox != null)
        return;
      deckBox.ShowDeleteButton(true);
    }
  }

  private void OnDeckBoxVisualOut(CollectionDeckBoxVisual deckBox)
  {
    if (deckBox.IsLocked())
      return;
    if (UniversalInputManager.Get().IsTouchMode())
    {
      if (!((UnityEngine.Object) this.m_teamInfoTooltip != (UnityEngine.Object) null) || !this.m_teamInfoTooltip.IsShown())
        return;
      deckBox.SetHighlightState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
    }
    else
    {
      if (UniversalInputManager.Get().InputIsOver(deckBox.m_deleteButton.gameObject))
        return;
      deckBox.ShowDeleteButton(false);
    }
  }

  protected void OnDeckBoxVisualRelease(TraySection traySection)
  {
    CollectionDeckBoxVisual deckBox = traySection.m_deckBox;
    if (deckBox.IsLocked())
      return;
    deckBox.enabled = true;
    if (this.IsTouchDragging || this.m_deckTray.IsUpdatingTrayMode())
      return;
    long deckId = deckBox.GetDeckID();
    LettuceTeam team = CollectionManager.Get().GetTeam(deckId);
    if (team != null)
    {
      if (team.IsBeingDeleted())
      {
        Log.DeckTray.Print(string.Format("DeckTrayTeamListContent.OnDeckBoxVisualRelease(): cannot edit team {0}; it is being deleted", (object) team));
        return;
      }
      if (team.IsSavingChanges())
      {
        Log.DeckTray.PrintWarning("DeckTrayTeamListContent.OnDeckBoxVisualRelease(): cannot edit team {0}; waiting for changes to be saved", (object) team);
        return;
      }
    }
    if (this.IsEditingTeam())
    {
      if (!UniversalInputManager.Get().IsTouchMode())
      {
        this.RenameCurrentlyEditingTeam();
      }
      else
      {
        if (!((UnityEngine.Object) this.m_teamInfoTooltip != (UnityEngine.Object) null) || this.m_teamInfoTooltip.IsShown())
          return;
        this.ShowTeamInfo();
      }
    }
    else
    {
      if (!this.IsModeActive())
        return;
      this.m_editingTraySection = traySection;
      this.m_centeringTeamList = this.m_editingTraySection.m_deckBox.GetPositionIndex();
      this.m_newTeamButton.SetEnabled(false);
      LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
      if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
        collectibleDisplay.RequestContentsToShowTeam(deckId);
      Options.Get().SetBool(Option.HAS_STARTED_A_DECK, true);
    }
  }

  protected void FireTeamCountChangedEvent()
  {
    DeckTrayTeamListContent.TeamCountChanged[] array = this.m_teamCountChangedListeners.ToArray();
    int count = CollectionManager.Get().GetTeams().Count;
    foreach (DeckTrayTeamListContent.TeamCountChanged teamCountChanged in array)
      teamCountChanged(count);
  }

  protected void FireBusyWithTeamEvent(bool busy)
  {
    foreach (DeckTrayTeamListContent.BusyWithTeam busyWithTeam in this.m_busyWithTeamListeners.ToArray())
      busyWithTeam(busy);
  }

  private int GetTotalDeckBoxesInUse()
  {
    int totalDeckBoxesInUse = 0;
    foreach (TraySection traySection in this.m_traySections)
    {
      if (traySection.m_deckBox.GetDeckID() > -1L)
        ++totalDeckBoxesInUse;
    }
    return totalDeckBoxesInUse;
  }

  public int UpdateTeamTrayVisuals(bool suppressFX = false)
  {
    List<LettuceTeam> teams = CollectionManager.Get().GetTeams();
    int count = teams.Count;
    CollectionManager.SortTeams(teams);
    for (int index = 0; index < count && index < this.m_traySections.Count; ++index)
    {
      if (index < teams.Count)
      {
        LettuceTeam team = teams[index];
        this.m_traySections[index].m_deckBox.AssignFromMercenariesTeam(team, suppressFX);
        this.m_traySections[index].m_deckBox.ShowNotificationButton(team.DoesContainDisabledMerc());
      }
      this.m_traySections[index].m_deckBox.SetIsLocked(index >= teams.Count);
    }
    return teams.Count;
  }

  public void OnTeamContentsUpdated(long teamID)
  {
    foreach (TraySection traySection in this.m_traySections)
    {
      if ((UnityEngine.Object) traySection.m_deckBox != (UnityEngine.Object) null)
      {
        LettuceTeam team = CollectionManager.Get().GetTeam(traySection.m_deckBox.GetDeckID());
        if (team != null)
          traySection.m_deckBox.AssignFromMercenariesTeam(team);
      }
    }
  }

  protected void SwapEditTrayIfNeeded(long editTeamID)
  {
    if (editTeamID < 0L)
      return;
    TraySection traySection1 = (TraySection) null;
    foreach (TraySection traySection2 in this.m_traySections)
    {
      if (traySection2.m_deckBox.GetDeckID() == editTeamID)
      {
        traySection1 = traySection2;
        break;
      }
    }
    if ((UnityEngine.Object) traySection1 == (UnityEngine.Object) this.m_editingTraySection)
      return;
    this.m_deckTray.TryEnableScrollbar();
    this.m_scrollbar.SetScrollImmediate((float) traySection1.m_deckBox.GetPositionIndex() / (float) (this.GetTotalDeckBoxesInUse() - 1));
    this.m_deckTray.SaveScrollbarPosition(DeckTray.DeckContentTypes.Decks);
    this.m_editingTraySection.m_deckBox.transform.localScale = CollectionDeckBoxVisual.SCALED_DOWN_LOCAL_SCALE;
    this.m_editingTraySection.m_deckBox.transform.localPosition = Vector3.zero with
    {
      y = 1.273138f
    };
    this.m_editingTraySection.m_deckBox.Hide();
    this.m_editingTraySection.m_deckBox.EnableButtonAnimation();
    traySection1.m_deckBox.transform.localScale = CollectionDeckBoxVisual.SCALED_UP_LOCAL_SCALE;
    traySection1.m_deckBox.transform.parent = (Transform) null;
    traySection1.m_deckBox.transform.position = this.m_teamEditTopPos.position;
    traySection1.ShowDeckBoxNoAnim();
    traySection1.m_deckBox.SetEnabled(true, false);
    this.m_editingTraySection = traySection1;
  }

  public override void StopDragToReorder()
  {
    if (this.m_draggingDeckBox != null)
    {
      foreach (LettuceTeam team in CollectionManager.Get().GetTeams())
        team.SendTeamOrderChanges();
      this.m_draggingDeckBox.OnStopDragToReorder();
    }
    this.m_draggingDeckBox = (IDraggableCollectionVisual) null;
    this.m_scrollbar.Pause(false);
    this.m_scrollbar.PauseUpdateScrollHeight(false);
  }

  protected override void UpdateDragToReorder()
  {
    if (this.m_draggingDeckBox == null)
      return;
    if (!InputCollection.GetMouseButton(0))
    {
      this.StopDragToReorder();
    }
    else
    {
      int index1 = this.m_traySections.FindIndex((Predicate<TraySection>) (section => section.m_deckBox == this.m_draggingDeckBox));
      if (index1 < 0)
        return;
      TraySection traySection1 = this.m_traySections[index1];
      if ((UnityEngine.Object) traySection1 == (UnityEngine.Object) null)
        return;
      Ray ray = Camera.main.ScreenPointToRay(InputCollection.GetMousePosition());
      float enter;
      if (!new Plane(-Camera.main.transform.forward, this.m_traySectionStartPos.position).Raycast(ray, out enter))
        return;
      Vector3 point = ray.GetPoint(enter);
      Vector3 size = TransformUtil.ComputeSetPointBounds((Component) this.m_traySections[0], false).size;
      float z = this.m_traySectionStartPos.position.z;
      int num1 = Mathf.FloorToInt((float) -((double) point.z - (double) z) / size.z);
      int count = CollectionManager.Get().GetTeams().Count;
      if (count < 1)
        return;
      double num2 = (double) this.m_scrollbar.m_ScrollBounds.bounds.min.z - (double) z;
      int num3 = Mathf.FloorToInt((float) -((double) this.m_scrollbar.m_ScrollBounds.bounds.max.z - (double) z) / size.z) - 1;
      int num4 = Mathf.FloorToInt((float) -num2 / size.z) + 1;
      int min = Mathf.Clamp(num3, 0, count - 1);
      int max = Mathf.Clamp(num4, 0, count - 1);
      int index2 = Mathf.Clamp(num1, min, max);
      if (index2 >= this.m_traySections.Count || index2 == index1)
        return;
      float tweenTime = 1f;
      TraySection traySection2 = this.m_traySections[index2];
      Bounds setPointBounds = TransformUtil.ComputeSetPointBounds(traySection2.gameObject, false);
      if (!this.m_scrollbar.ScrollObjectIntoView(traySection2.gameObject, setPointBounds.center.z - traySection2.gameObject.transform.position.z, setPointBounds.extents.z * 1.25f, (UIBScrollable.OnScrollComplete) null, iTween.EaseType.linear, tweenTime, true))
        this.m_scrollbar.StopScroll();
      this.m_traySections.RemoveAt(index1);
      this.m_traySections.Insert(index2, traySection1);
      for (int index3 = 0; index3 < count; ++index3)
      {
        TraySection traySection3 = this.m_traySections[index3];
        traySection3.m_deckBox.SetPositionIndex(index3);
        LettuceTeam team = CollectionManager.Get().GetTeam(traySection3.m_deckBox.GetDeckID());
        if (team != null)
          team.SortOrder = (uint) index3;
      }
      this.RefreshTraySectionPositions(true);
    }
  }

  private void RefreshTraySectionPositions(bool animateToNewPositions)
  {
    Vector3 localPosition = this.m_traySectionStartPos.localPosition;
    Vector3 vector3_1 = Vector3.zero;
    Transform parent = this.m_traySectionStartPos.parent;
    for (int index = 0; index < 11; ++index)
    {
      TraySection traySection = this.m_traySections[index];
      Bounds setPointBounds = TransformUtil.ComputeSetPointBounds(traySection.gameObject, false);
      Vector3 position = traySection.transform.position;
      if (index > 0)
      {
        Vector3 vector3_2 = position - TransformUtil.ComputeWorldPoint(setPointBounds, TransformUtil.GetUnitAnchor(Anchor.FRONT));
        Vector3 vector = vector3_1 + vector3_2;
        Vector3 vector3_3 = (UnityEngine.Object) parent != (UnityEngine.Object) null ? parent.InverseTransformVector(vector) : vector;
        localPosition += vector3_3;
      }
      if (animateToNewPositions)
      {
        Hashtable args = iTween.Hash((object) "position", (object) localPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutCubic);
        iTween.MoveTo(traySection.gameObject, args);
      }
      else
        traySection.gameObject.transform.localPosition = localPosition;
      Material material1 = (Material) null;
      foreach (Material material2 in RendererExtension.GetMaterials(traySection.m_door.GetComponent<Renderer>()))
      {
        if (material2.name.Equals("DeckTray", StringComparison.OrdinalIgnoreCase) || material2.name.Equals("DeckTray (Instance)", StringComparison.OrdinalIgnoreCase))
        {
          material1 = material2;
          break;
        }
      }
      Vector2 vector2 = new Vector2(0.0f, -0.0825f * (float) index);
      RendererExtension.GetMaterial(traySection.GetComponent<Renderer>()).mainTextureOffset = vector2;
      if ((UnityEngine.Object) material1 != (UnityEngine.Object) null)
        material1.mainTextureOffset = vector2;
      vector3_1 = TransformUtil.ComputeWorldPoint(setPointBounds, TransformUtil.GetUnitAnchor(Anchor.BACK)) - position;
    }
  }

  public bool UpdateDeckBoxWithNewId(long oldId, long newId)
  {
    foreach (TraySection traySection in this.m_traySections)
    {
      if (traySection.m_deckBox.GetDeckID() == oldId)
      {
        traySection.m_deckBox.SetDeckID(newId);
        return true;
      }
    }
    return false;
  }

  public void RefreshMissingCardIndicators()
  {
    foreach (TraySection traySection in this.m_traySections)
      traySection.m_deckBox.UpdateInvalidCardCountIndicator();
  }

  public delegate void BusyWithTeam(bool busy);

  public delegate void TeamCountChanged(int teamCount);
}
