using Assets;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone;
using Hearthstone.Core;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class DeckTrayDeckListContent : DeckTrayReorderableContent
{
  [CustomEditField(Sections = "Deck Tray Settings")]
  public Transform m_deckEditTopPos;
  [CustomEditField(Sections = "Deck Tray Settings")]
  public Transform m_traySectionStartPos;
  [CustomEditField(Sections = "Deck Tray Settings")]
  public GameObject m_deckInfoTooltipBone;
  [CustomEditField(Sections = "Deck Tray Settings")]
  public GameObject m_deckOptionsBone;
  [CustomEditField(Sections = "Prefabs")]
  public TraySection m_traySectionPrefab;
  [CustomEditField(Sections = "Prefabs")]
  public DeckTray m_deckTray;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_deckInfoActorPrefab;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_deckOptionsPrefab;
  [CustomEditField(Sections = "Deck Button Settings")]
  public ParticleSystem m_deleteDeckPoof;
  [CustomEditField(Sections = "Deck Button Settings")]
  public Vector3 m_deleteDeckPoofVisualOffset;
  [SerializeField]
  private Vector3 m_deckButtonOffset;
  [CustomEditField(Sections = "Deck Button Settings")]
  public GameObject m_newDeckButtonContainer;
  [CustomEditField(Sections = "Deck Button Settings")]
  public CollectionDeckTrayButton m_newDeckButton;
  protected const float TIME_BETWEEN_TRAY_DOOR_ANIMS = 0.015f;
  protected const int MAX_NUM_DECKBOXES_AVAILABLE = 27;
  protected const int NUM_DECKBOXES_TO_DISPLAY = 29;
  protected CollectionDeckInfo m_deckInfoTooltip;
  protected List<TraySection> m_traySections = new List<TraySection>();
  protected TraySection m_editingTraySection;
  protected int m_centeringDeckList = -1;
  protected DeckOptionsMenu m_deckOptionsMenu;
  protected bool m_initialized;
  protected bool m_animatingExit;
  protected bool m_doneEntering;
  private bool m_wasTouchModeEnabled;
  protected string m_previousDeckName;
  protected const float DELETE_DECK_ANIM_TIME = 0.5f;
  protected bool m_initializedDeckHeroes;
  protected bool m_deletingDecks;
  protected bool m_waitingToDeleteDeck;
  protected List<CollectionDeck> m_decksToDelete = new List<CollectionDeck>();
  protected TraySection m_newlyCreatedTraySection;
  private List<DeckTrayDeckListContent.DeckCountChanged> m_deckCountChangedListeners = new List<DeckTrayDeckListContent.DeckCountChanged>();
  private List<DeckTrayDeckListContent.BusyWithDeck> m_busyWithDeckListeners = new List<DeckTrayDeckListContent.BusyWithDeck>();
  private static PegasusShared.FormatType s_PreHeroPickerFormat = PegasusShared.FormatType.FT_STANDARD;
  public static PegasusShared.FormatType s_HeroPickerFormat = PegasusShared.FormatType.FT_STANDARD;
  private ScreenEffectsHandle m_screenEffectsHandle;
  private const float TRAY_MATERIAL_Y_OFFSET = -0.0825f;

  [CustomEditField(Sections = "Deck Button Settings")]
  public Vector3 DeckButtonOffset
  {
    set
    {
      this.m_deckButtonOffset = value;
      this.UpdateNewDeckButton();
    }
    get => this.m_deckButtonOffset;
  }

  protected void Update()
  {
    this.UpdateDragToReorder();
    if (this.m_wasTouchModeEnabled == UniversalInputManager.Get().IsTouchMode())
      return;
    this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
    if (!UniversalInputManager.Get().IsTouchMode() || !((UnityEngine.Object) this.m_deckInfoTooltip != (UnityEngine.Object) null))
      return;
    this.HideDeckInfo();
  }

  protected override void Awake()
  {
    base.Awake();
    CollectionManager collectionManager = CollectionManager.Get();
    collectionManager.RegisterFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
    collectionManager.RegisterOnUIHeroOverrideCardRemovedListener(new CollectionManager.OnUIHeroOverrideCardRemovedCallback(this.OnUIHeroOverrideCardRemoved));
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.WillReset += new Action(this.WillReset);
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected override void OnDestroy()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    collectionManager.RemoveFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
    collectionManager.RemoveDeckDeletedListener(new CollectionManager.DelOnDeckDeleted(this.OnDeckDeleted));
    collectionManager.RemoveOnUIHeroOverrideCardRemovedListener(new CollectionManager.OnUIHeroOverrideCardRemovedCallback(this.OnUIHeroOverrideCardRemoved));
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.WillReset -= new Action(this.WillReset);
    if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null)
      Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    base.OnDestroy();
  }

  private void WillReset()
  {
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.BeginAnimation));
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.EndAnimation));
  }

  private void OnNewDeckButtonPress()
  {
    if (!this.IsModeActive() || this.IsTouchDragging)
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "Hub_Click.prefab:cc2cf2b5507827149b13d12210c0f323");
    this.StartCreateNewDeck();
  }

  protected void StartCreateNewDeck()
  {
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DECKEDITOR);
    this.ShowNewDeckButton(false);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    PegasusShared.FormatType formatType = Options.GetFormatType();
    if (formatType == PegasusShared.FormatType.FT_UNKNOWN)
    {
      RankMgr.LogMessage("Options.GetFormatType() = FT_UNKOWN", nameof (StartCreateNewDeck), "D:\\builders\\work\\source\\25.0.0\\Pegasus\\Client\\Assets\\Shared\\Scripts\\Game\\DeckTrayDeckListContent.cs", 198);
    }
    else
    {
      DeckTrayDeckListContent.s_PreHeroPickerFormat = formatType;
      DeckTrayDeckListContent.s_HeroPickerFormat = !Options.GetInRankedPlayMode() ? Options.Get().GetEnum<PegasusShared.FormatType>(Option.FORMAT_TYPE_LAST_PLAYED) : Options.GetFormatType();
      collectibleDisplay.EnterSelectNewDeckHeroMode();
    }
  }

  protected void EndCreateNewDeck(bool newDeck)
  {
    Options.SetFormatType(DeckTrayDeckListContent.s_PreHeroPickerFormat);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.ExitSelectNewDeckHeroMode();
    this.ShowNewDeckButton(true, (CollectionDeckTrayButton.DelOnAnimationFinished) (o =>
    {
      if (!newDeck)
        return;
      this.UpdateAllTrays(true, true);
    }));
  }

  private void DeleteQueuedDecks(bool force = false)
  {
    if (this.m_decksToDelete.Count == 0 || !this.IsModeActive() && !force)
      return;
    foreach (CollectionDeck collectionDeck in this.m_decksToDelete)
    {
      Network.Get().DeleteDeck(collectionDeck.ID, collectionDeck.Type);
      CollectionManager.Get().AddPendingDeckDelete(collectionDeck.ID);
      if (!Network.IsLoggedIn() || collectionDeck.ID <= 0L)
        CollectionManager.Get().OnDeckDeletedWhileOffline(collectionDeck.ID);
    }
    this.m_decksToDelete.Clear();
  }

  private void OnDeckDeleted(CollectionDeck removedDeck)
  {
    if (removedDeck == null)
      return;
    this.m_waitingToDeleteDeck = false;
    this.StartCoroutine(this.DeleteDeckAnimation(removedDeck.ID));
  }

  private void OnFavoriteHeroChanged(
    TAG_CLASS heroClass,
    NetCache.CardDefinition favoriteHero,
    bool isFavorite,
    object userData)
  {
    this.UpdateDeckTrayVisuals(false, heroClass);
  }

  private void OnUIHeroOverrideCardRemoved() => this.UpdateDeckTrayVisuals();

  private IEnumerator DeleteDeckAnimation(long deckID, Action callback = null)
  {
    DeckTrayDeckListContent trayDeckListContent = this;
    while (trayDeckListContent.m_deletingDecks)
      yield return (object) null;
    int index1 = 0;
    TraySection traySection1 = (TraySection) null;
    TraySection setNewDeckButtonPosition = trayDeckListContent.m_traySections[0];
    for (int index2 = 0; index2 < trayDeckListContent.m_traySections.Count; ++index2)
    {
      TraySection traySection2 = trayDeckListContent.m_traySections[index2];
      long deckId = traySection2.m_deckBox.GetDeckID();
      if (deckId == deckID)
      {
        index1 = index2;
        traySection1 = traySection2;
      }
      else if (deckId == -1L)
        break;
      setNewDeckButtonPosition = traySection2;
    }
    if ((UnityEngine.Object) traySection1 == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "Unable to delete deck with ID {0}. Not found in tray sections.", (UnityEngine.Object) trayDeckListContent.gameObject);
    }
    else
    {
      trayDeckListContent.FireBusyWithDeckEvent(true);
      trayDeckListContent.m_deletingDecks = true;
      trayDeckListContent.FireDeckCountChangedEvent();
      trayDeckListContent.m_traySections.RemoveAt(index1);
      bool newDeckBtnActive;
      Vector3 outPosition;
      trayDeckListContent.GetIdealNewDeckButtonLocalPosition(setNewDeckButtonPosition, out outPosition, out newDeckBtnActive);
      Vector3 vector3_1 = traySection1.transform.localPosition;
      if ((UnityEngine.Object) HeroPickerDisplay.Get() == (UnityEngine.Object) null || !HeroPickerDisplay.Get().IsShown())
      {
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_delete_deck.prefab:5ca16bec63041b741a4fb33706ed9cb1", trayDeckListContent.gameObject);
        trayDeckListContent.m_deleteDeckPoof.transform.position = traySection1.m_deckBox.transform.position + trayDeckListContent.m_deleteDeckPoofVisualOffset;
        trayDeckListContent.m_deleteDeckPoof.Play(true);
      }
      traySection1.ClearDeckInfo();
      traySection1.gameObject.SetActive(false);
      List<GameObject> animatingTraySections = new List<GameObject>();
      Action<object> action = (Action<object>) (obj => animatingTraySections.Remove((GameObject) obj));
      Vector3 vector3_2 = Vector3.zero;
      for (int index3 = index1; index3 < trayDeckListContent.m_traySections.Count; ++index3)
      {
        TraySection traySection3 = trayDeckListContent.m_traySections[index3];
        Vector3 localPosition = traySection3.transform.localPosition;
        iTween.MoveTo(traySection3.gameObject, iTween.Hash((object) "position", (object) vector3_1, (object) "isLocal", (object) true, (object) "time", (object) 0.5f, (object) "easeType", (object) iTween.EaseType.easeOutBounce, (object) "oncomplete", (object) action, (object) "oncompleteparams", (object) traySection3.gameObject, (object) "name", (object) "position"));
        animatingTraySections.Add(traySection3.gameObject);
        if (index3 <= 25)
          vector3_2 = localPosition;
        vector3_1 = localPosition;
      }
      if (index1 == 26)
        vector3_2 = traySection1.transform.localPosition;
      trayDeckListContent.m_traySections.Insert(26, traySection1);
      trayDeckListContent.m_newDeckButton.SetIsUsable(trayDeckListContent.CanShowNewDeckButton());
      traySection1.gameObject.SetActive(true);
      traySection1.HideDeckBox(true);
      traySection1.transform.localPosition = vector3_2;
      if (trayDeckListContent.m_newDeckButton.gameObject.activeSelf)
      {
        iTween.MoveTo(trayDeckListContent.m_newDeckButtonContainer, iTween.Hash((object) "position", (object) outPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.5f, (object) "easeType", (object) iTween.EaseType.easeOutBounce, (object) "oncomplete", (object) action, (object) "oncompleteparams", (object) trayDeckListContent.m_newDeckButtonContainer, (object) "name", (object) "position"));
        animatingTraySections.Add(trayDeckListContent.m_newDeckButtonContainer);
      }
      else
        trayDeckListContent.m_newDeckButtonContainer.transform.localPosition = outPosition;
      while (animatingTraySections.Count > 0)
      {
        animatingTraySections.RemoveAll((Predicate<GameObject>) (obj => (UnityEngine.Object) obj == (UnityEngine.Object) null || !obj.activeInHierarchy));
        yield return (object) null;
      }
      if (!CollectionManager.Get().IsInEditMode())
        trayDeckListContent.ShowNewDeckButton(newDeckBtnActive);
      trayDeckListContent.FireBusyWithDeckEvent(false);
      if (callback != null)
        callback();
      trayDeckListContent.m_deletingDecks = false;
    }
  }

  private void UpdateNewDeckButton(TraySection setNewDeckButtonPosition = null) => this.ShowNewDeckButton(this.UpdateNewDeckButtonPosition(setNewDeckButtonPosition) && this.CanShowNewDeckButton());

  private bool UpdateNewDeckButtonPosition(TraySection setNewDeckButtonPosition = null)
  {
    bool outActive = false;
    Vector3 outPosition;
    this.GetIdealNewDeckButtonLocalPosition(setNewDeckButtonPosition, out outPosition, out outActive);
    this.m_newDeckButtonContainer.transform.localPosition = outPosition;
    return outActive;
  }

  private void GetIdealNewDeckButtonLocalPosition(
    TraySection setNewDeckButtonPosition,
    out Vector3 outPosition,
    out bool outActive)
  {
    TraySection unusedTraySection = this.GetLastUnusedTraySection();
    TraySection traySection = (UnityEngine.Object) setNewDeckButtonPosition == (UnityEngine.Object) null ? unusedTraySection : setNewDeckButtonPosition;
    outActive = (UnityEngine.Object) unusedTraySection != (UnityEngine.Object) null;
    outPosition = ((UnityEngine.Object) traySection != (UnityEngine.Object) null ? traySection.transform.localPosition : this.m_traySectionStartPos.localPosition) + this.m_deckButtonOffset;
  }

  public void ShowNewDeckButton(
    bool newDeckButtonActive,
    CollectionDeckTrayButton.DelOnAnimationFinished callback = null)
  {
    this.ShowNewDeckButton(newDeckButtonActive, new float?(), callback);
  }

  public void ShowNewDeckButton(
    bool newDeckButtonActive,
    float? speed,
    CollectionDeckTrayButton.DelOnAnimationFinished callback = null)
  {
    if (this.m_newDeckButton.IsPoppedUp() != newDeckButtonActive)
    {
      if (newDeckButtonActive)
      {
        this.m_newDeckButton.gameObject.SetActive(true);
        this.m_newDeckButton.PlayPopUpAnimation((CollectionDeckTrayButton.DelOnAnimationFinished) (o =>
        {
          if (callback == null)
            return;
          callback((object) this);
        }), (object) null, speed);
      }
      else
        this.m_newDeckButton.PlayPopDownAnimation((CollectionDeckTrayButton.DelOnAnimationFinished) (o =>
        {
          this.m_newDeckButton.gameObject.SetActive(false);
          if (callback == null)
            return;
          callback((object) this);
        }), (object) null, speed);
    }
    else
    {
      if (callback == null)
        return;
      callback((object) this);
    }
  }

  public override bool AnimateContentEntranceStart()
  {
    this.Initialize();
    long editDeckID = -1;
    if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      editDeckID = this.m_editingTraySection.m_deckBox.GetDeckID();
    this.UpdateDeckTrayVisuals(!this.m_initializedDeckHeroes);
    this.m_initializedDeckHeroes = true;
    this.SwapEditTrayIfNeeded(editDeckID);
    this.UpdateAllTrays(CollectionManagerDisplay.IsSpecialOneDeckMode(), false);
    if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
    {
      this.FinishRenamingEditingDeck();
      this.m_editingTraySection.MoveDeckBoxBackToOriginalPosition(0.25f, (TraySection.DelOnDoorStateChangedCallback) (o => this.m_editingTraySection = (TraySection) null));
    }
    this.m_newDeckButton.SetIsUsable(this.CanShowNewDeckButton());
    this.FireBusyWithDeckEvent(true);
    this.FireDeckCountChangedEvent();
    CollectionManager.Get().DoneEditing();
    return true;
  }

  public override bool AnimateContentEntranceEnd()
  {
    if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      return false;
    this.m_newDeckButton.SetEnabled(true);
    this.FireBusyWithDeckEvent(false);
    this.DeleteQueuedDecks(true);
    return true;
  }

  public override bool AnimateContentExitStart()
  {
    this.m_animatingExit = true;
    this.FireBusyWithDeckEvent(true);
    float? speed = new float?();
    if (SceneMgr.Get().IsInTavernBrawlMode())
      speed = new float?(500f);
    this.ShowNewDeckButton(false, speed);
    Processor.ScheduleCallback(0.5f, false, new Processor.ScheduledCallback(this.BeginAnimation));
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
      this.UpdateNewDeckButtonPosition(animateTraySection);
      this.ShowNewDeckButton(true, (CollectionDeckTrayButton.DelOnAnimationFinished) (_1 => animateTraySection.ShowDeckBox(true, (TraySection.DelOnDoorStateChangedCallback) (_2 =>
      {
        animateTraySection.m_deckBox.gameObject.SetActive(false);
        this.m_newDeckButton.FlipHalfOverAndHide(0.1f, (CollectionDeckTrayButton.DelOnAnimationFinished) (_3 => animateTraySection.FlipDeckBoxHalfOverToShow(0.1f, (TraySection.DelOnDoorStateChangedCallback) (_4 => animateTraySection.MoveDeckBoxToEditPosition(this.m_deckEditTopPos.position, 0.25f)))));
      }))));
      this.m_editingTraySection = this.m_newlyCreatedTraySection;
      this.m_newlyCreatedTraySection = (TraySection) null;
      secondsToWait += 0.7f;
    }
    else if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      this.m_editingTraySection.MoveDeckBoxToEditPosition(this.m_deckEditTopPos.position, 0.25f);
    Processor.ScheduleCallback(secondsToWait, false, new Processor.ScheduledCallback(this.EndAnimation));
  }

  private void EndAnimation(object userData)
  {
    this.m_animatingExit = false;
    this.FireBusyWithDeckEvent(false);
  }

  private CollectionDeck UpdateRenamingEditingDeck(string newDeckName)
  {
    CollectionDeck editingDeck = this.m_deckTray.GetCardsContent().GetEditingDeck();
    if (editingDeck != null && !string.IsNullOrEmpty(newDeckName))
      editingDeck.Name = newDeckName;
    return editingDeck;
  }

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

  public void CreateNewDeckFromUserSelection(
    TAG_CLASS heroClass,
    string heroCardID,
    string customDeckName = null,
    DeckSourceType deckSourceType = DeckSourceType.DECK_SOURCE_TYPE_NORMAL,
    string pastedDeckHashString = null)
  {
    int num = SceneMgr.Get().IsInTavernBrawlMode() ? 1 : 0;
    bool flag = SceneMgr.Get().GetMode() == SceneMgr.Mode.PVP_DUNGEON_RUN;
    DeckType deckType = DeckType.NORMAL_DECK;
    string name = customDeckName;
    if (num != 0)
    {
      name = GameStrings.Get("GLUE_COLLECTION_TAVERN_BRAWL_DECKNAME");
      if (TavernBrawlManager.Get().CurrentBrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
      {
        deckType = DeckType.FSG_BRAWL_DECK;
        name = GameStrings.Get("GLUE_COLLECTION_FSG_BRAWL_DECKNAME");
      }
      else
        deckType = DeckType.TAVERN_BRAWL_DECK;
    }
    else if (flag)
    {
      deckType = DeckType.PVPDR_DECK;
      name = GameStrings.Get("GLUE_COLLECTION_DUEL_DECKNAME");
    }
    else if (string.IsNullOrEmpty(name))
      name = CollectionManager.Get().AutoGenerateDeckName(heroClass);
    CollectionManager.Get().SendCreateDeck(deckType, name, heroCardID, deckSourceType, pastedDeckHashString);
    this.EndCreateNewDeck(true);
  }

  public void CreateNewDeckCancelled() => this.EndCreateNewDeck(false);

  public bool IsWaitingToDeleteDeck() => this.m_waitingToDeleteDeck;

  public int NumDecksToDelete() => this.m_decksToDelete.Count;

  public bool IsDeletingDecks() => this.m_deletingDecks;

  public void DeleteDeck(long deckID)
  {
    CollectionDeck deck = CollectionManager.Get().GetDeck(deckID);
    if (deck == null)
    {
      Log.All.PrintError("Unable to delete deck id={0} - not found in cache.", (object) deckID);
    }
    else
    {
      if (Network.IsLoggedIn() && deckID <= 0L)
        Log.Offline.PrintDebug("DeleteDeck() - Attempting to delete fake deck while online.");
      deck.MarkBeingDeleted();
      this.m_decksToDelete.Add(deck);
      this.DeleteQueuedDecks();
    }
  }

  public void DeleteEditingDeck()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
    {
      Debug.LogWarning((object) "No deck currently being edited!");
    }
    else
    {
      this.m_waitingToDeleteDeck = true;
      this.DeleteDeck(editedDeck.ID);
    }
  }

  public void CancelRenameEditingDeck() => this.FinishRenamingEditingDeck();

  public Vector3 GetNewDeckButtonPosition() => this.m_newDeckButton.transform.localPosition;

  public void UpdateDeckName(string deckName = null)
  {
    if (deckName == null)
    {
      CollectionDeck editingDeck = this.m_deckTray.GetCardsContent().GetEditingDeck();
      if (editingDeck == null)
        return;
      deckName = editingDeck.Name;
    }
    this.FinishRenamingEditingDeck(deckName);
  }

  public void RenameCurrentlyEditingDeck()
  {
    if ((UnityEngine.Object) this.m_editingTraySection == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "Unable to rename deck. No deck currently being edited.", (UnityEngine.Object) this.gameObject);
    }
    else
    {
      if (CollectionManagerDisplay.IsSpecialOneDeckMode())
        return;
      CollectionDeckBoxVisual deckBox = this.m_editingTraySection.m_deckBox;
      deckBox.HideDeckName();
      Camera camera = Box.Get().GetCamera();
      Bounds bounds = deckBox.GetDeckNameText().GetBounds();
      Vector3 min = bounds.min;
      Vector3 max = bounds.max;
      Rect guiViewportRect = CameraUtils.CreateGUIViewportRect(camera, min, max);
      Font localizedFont = deckBox.GetDeckNameText().GetLocalizedFont();
      this.m_previousDeckName = deckBox.GetDeckNameText().Text;
      UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams()
      {
        m_owner = this.gameObject,
        m_rect = guiViewportRect,
        m_updatedCallback = (UniversalInputManager.TextInputUpdatedCallback) (newName => this.UpdateRenamingEditingDeck(newName)),
        m_completedCallback = (UniversalInputManager.TextInputCompletedCallback) (newName => this.FinishRenamingEditingDeck(newName)),
        m_canceledCallback = (UniversalInputManager.TextInputCanceledCallback) ((a1, a2) => this.FinishRenamingEditingDeck(this.m_previousDeckName)),
        m_maxCharacters = CollectionDeck.DefaultMaxDeckNameCharacters,
        m_font = localizedFont,
        m_text = deckBox.GetDeckNameText().Text
      };
      UniversalInputManager.Get().UseTextInput(parms);
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

  public override bool AnimateContentExitEnd() => !this.m_animatingExit;

  public override bool PreAnimateContentExit()
  {
    if ((UnityEngine.Object) this.m_scrollbar == (UnityEngine.Object) null)
      return true;
    if (this.m_centeringDeckList != -1 && (UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
    {
      BoxCollider component = this.m_editingTraySection.m_deckBox.GetComponent<BoxCollider>();
      if (this.m_scrollbar.ScrollObjectIntoView(this.m_editingTraySection.m_deckBox.gameObject, component.center.y, component.size.y / 2f, (UIBScrollable.OnScrollComplete) (f => this.m_animatingExit = false), iTween.EaseType.linear, this.m_scrollbar.m_ScrollTweenTime, true))
      {
        this.m_animatingExit = true;
        this.m_centeringDeckList = -1;
      }
    }
    this.StartCoroutine(this.ShowTrayDoors(false));
    return !this.m_animatingExit;
  }

  public override bool PreAnimateContentEntrance()
  {
    this.m_doneEntering = false;
    this.StartCoroutine(this.ShowTrayDoors(true));
    return true;
  }

  public override void OnEditedDeckChanged(
    CollectionDeck newDeck,
    CollectionDeck oldDeck,
    bool isNewDeck)
  {
    if (newDeck != null && (UnityEngine.Object) this.m_deckInfoTooltip != (UnityEngine.Object) null)
    {
      this.m_deckInfoTooltip.SetDeck(newDeck);
      if ((UnityEngine.Object) this.m_deckOptionsMenu != (UnityEngine.Object) null)
        this.m_deckOptionsMenu.SetDeck(newDeck);
    }
    if (this.IsModeActive())
      this.UpdateDeckTrayVisuals();
    if (!isNewDeck || newDeck == null)
      return;
    this.m_newlyCreatedTraySection = this.GetExistingTrayFromDeck(newDeck);
    if (!((UnityEngine.Object) this.m_newlyCreatedTraySection != (UnityEngine.Object) null))
      return;
    this.m_centeringDeckList = this.m_newlyCreatedTraySection.m_deckBox.GetPositionIndex();
  }

  public void UpdateEditingDeckBoxVisual(string heroCardId, TAG_PREMIUM? premiumOverride = null)
  {
    if ((UnityEngine.Object) this.m_editingTraySection == (UnityEngine.Object) null)
      return;
    this.m_editingTraySection.m_deckBox.SetHeroCardPremiumOverride(premiumOverride);
    if (heroCardId != string.Empty)
      this.m_editingTraySection.m_deckBox.SetHeroCardID(heroCardId);
    else
      this.m_editingTraySection.m_deckBox.SetHeroCardIdFromDeck();
  }

  private void OnDrawGizmos()
  {
    if ((UnityEngine.Object) this.m_editingTraySection == (UnityEngine.Object) null)
      return;
    Bounds bounds = this.m_editingTraySection.m_deckBox.GetDeckNameText().GetBounds();
    Gizmos.DrawWireSphere(bounds.min, 0.1f);
    Gizmos.DrawWireSphere(bounds.max, 0.1f);
  }

  public void RegisterDeckCountUpdated(DeckTrayDeckListContent.DeckCountChanged dlg) => this.m_deckCountChangedListeners.Add(dlg);

  public void UnregisterDeckCountUpdated(DeckTrayDeckListContent.DeckCountChanged dlg) => this.m_deckCountChangedListeners.Remove(dlg);

  public void RegisterBusyWithDeck(DeckTrayDeckListContent.BusyWithDeck dlg) => this.m_busyWithDeckListeners.Add(dlg);

  public void UnregisterBusyWithDeck(DeckTrayDeckListContent.BusyWithDeck dlg) => this.m_busyWithDeckListeners.Remove(dlg);

  public virtual void HideTraySectionsNotInBounds(Bounds bounds)
  {
    int num = 0;
    foreach (TraySection traySection in this.m_traySections)
    {
      if (traySection.HideIfNotInBounds(bounds))
        ++num;
    }
    Log.DeckTray.Print("Hid {0} tray sections that were not visible.", (object) num);
    UIBScrollableItem component = this.m_newDeckButtonContainer.GetComponent<UIBScrollableItem>();
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
      this.m_newDeckButton.gameObject.SetActive(false);
    }
  }

  protected void Initialize()
  {
    if (this.m_initialized)
      return;
    this.m_newDeckButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnNewDeckButtonPress()));
    CollectionManager.Get().RegisterDeckDeletedListener(new CollectionManager.DelOnDeckDeleted(this.OnDeckDeleted));
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_deckInfoActorPrefab, AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("Unable to load actor {0}: null", (object) this.m_deckInfoActorPrefab), (UnityEngine.Object) this.gameObject);
    }
    else
    {
      this.m_deckInfoTooltip = gameObject.GetComponent<CollectionDeckInfo>();
      if ((UnityEngine.Object) this.m_deckInfoTooltip == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("Actor {0} does not contain CollectionDeckInfo component.", (object) this.m_deckInfoActorPrefab), (UnityEngine.Object) this.gameObject);
      }
      else
      {
        GameUtils.SetParent((Component) this.m_deckInfoTooltip, this.m_deckInfoTooltipBone);
        this.m_deckInfoTooltip.RegisterHideListener(new CollectionDeckInfo.HideListener(this.HideDeckInfoListener));
        this.m_deckOptionsMenu = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_deckOptionsPrefab).GetComponent<DeckOptionsMenu>();
        GameUtils.SetParent(this.m_deckOptionsMenu.gameObject, this.m_deckOptionsBone);
        this.m_deckOptionsMenu.SetDeckInfo(this.m_deckInfoTooltip);
        this.HideDeckInfo();
        this.CreateTraySections();
        this.m_initialized = true;
      }
    }
  }

  protected void HideDeckInfoListener()
  {
    if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
    {
      LayerUtils.SetLayer(this.m_editingTraySection.m_deckBox.gameObject, GameLayer.Default);
      LayerUtils.SetLayer(this.m_deckOptionsMenu.gameObject, GameLayer.Default);
      this.m_editingTraySection.m_deckBox.HideRenameVisuals();
    }
    this.m_screenEffectsHandle.StopEffect();
    if (UniversalInputManager.Get().IsTouchMode())
    {
      if ((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      {
        this.m_editingTraySection.m_deckBox.SetHighlightState(ActorStateType.NONE);
        this.m_editingTraySection.m_deckBox.ShowDeckName();
      }
      this.FinishRenamingEditingDeck();
    }
    this.m_deckOptionsMenu.Hide();
    if (!((UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null))
      return;
    this.m_editingTraySection.m_deckBox.UpdateColliderHeightForDeathKnight();
  }

  protected virtual void ShowDeckInfo()
  {
    if (!UniversalInputManager.Get().IsTouchMode() && (UnityEngine.Object) this.m_editingTraySection != (UnityEngine.Object) null)
      this.m_editingTraySection.m_deckBox.ShowRenameVisuals();
    LayerUtils.SetLayer(this.m_editingTraySection.m_deckBox.gameObject, GameLayer.IgnoreFullScreenEffects);
    LayerUtils.SetLayer(this.m_deckInfoTooltip.gameObject, GameLayer.IgnoreFullScreenEffects);
    LayerUtils.SetLayer(this.m_deckOptionsMenu.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.DesaturatePerspective with
    {
      Time = 0.25f
    });
    this.m_deckInfoTooltip.UpdateManaCurve();
    if (CollectionManagerDisplay.ShouldShowDeckHeaderInfo())
      this.m_deckInfoTooltip.Show();
    if (!CollectionManagerDisplay.ShouldShowDeckOptionsMenu())
      return;
    this.m_deckOptionsMenu.Show();
  }

  protected void HideDeckInfo() => this.m_deckInfoTooltip.Hide();

  public bool IsShowingDeckOptions => (UnityEngine.Object) this.m_deckOptionsMenu != (UnityEngine.Object) null && this.m_deckOptionsMenu.IsShown;

  protected void CreateTraySections()
  {
    Vector3 localScale = this.m_traySectionStartPos.localScale;
    Vector3 localEulerAngles = this.m_traySectionStartPos.localEulerAngles;
    for (int idx = 0; idx < 29; ++idx)
    {
      TraySection traySection = (TraySection) GameUtils.Instantiate((Component) this.m_traySectionPrefab, this.gameObject);
      traySection.m_deckBox.SetPositionIndex(idx);
      traySection.transform.localScale = localScale;
      traySection.transform.localEulerAngles = localEulerAngles;
      traySection.EnableDoors(idx < 27);
      CollectionDeckBoxVisual deckBox = traySection.m_deckBox;
      deckBox.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.OnDeckBoxVisualOver(deckBox)));
      deckBox.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.OnDeckBoxVisualOut(deckBox)));
      deckBox.AddEventListener(UIEventType.TAP, (UIEvent.Handler) (e => this.OnDeckBoxVisualRelease(traySection)));
      deckBox.SetIsLocked(this.ShouldDeckBoxesBeLocked());
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

  protected TraySection GetExistingTrayFromDeck(CollectionDeck deck) => this.GetExistingTrayFromDeck(deck.ID);

  private TraySection GetExistingTrayFromDeck(long deckID)
  {
    foreach (TraySection traySection in this.m_traySections)
    {
      if (traySection.m_deckBox.GetDeckID() == deckID)
        return traySection;
    }
    return (TraySection) null;
  }

  public TraySection GetEditingTraySection() => this.m_editingTraySection;

  protected void InitializeTraysFromDecks() => this.UpdateDeckTrayVisuals(true);

  protected void UpdateAllTrays(bool immediate, bool updateVisuals)
  {
    if (updateVisuals)
      this.UpdateDeckTrayVisuals();
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
    this.UpdateNewDeckButton();
    this.m_doneEntering = true;
  }

  public TraySection GetLastUnusedTraySection()
  {
    int num = 0;
    foreach (TraySection traySection in this.m_traySections)
    {
      if (num < 27)
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
      if (num < 27)
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

  public bool CanShowNewDeckButton() => CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK).Count < 27 && !SceneMgr.Get().IsInDuelsMode() && !SceneMgr.Get().IsInTavernBrawlMode() && GameUtils.IsTraditionalTutorialComplete();

  public bool ShouldDeckBoxesBeLocked() => !GameUtils.IsTraditionalTutorialComplete();

  public void SetEditingTraySection(int index)
  {
    this.m_editingTraySection = this.m_traySections[index];
    this.m_centeringDeckList = this.m_editingTraySection.m_deckBox.GetPositionIndex();
  }

  protected bool IsEditingCards() => CollectionManager.Get().GetEditedDeck() != null;

  protected virtual void OnDeckBoxVisualOver(CollectionDeckBoxVisual deckBox)
  {
    if (deckBox.IsLocked() || UniversalInputManager.Get().IsTouchMode())
      return;
    if (CollectionManager.Get().IsEditingDeathKnightDeck())
      deckBox.ResetColliderHeight();
    if (this.IsEditingCards() && (UnityEngine.Object) this.m_deckInfoTooltip != (UnityEngine.Object) null)
    {
      this.ShowDeckInfo();
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
      if (!((UnityEngine.Object) this.m_deckInfoTooltip != (UnityEngine.Object) null) || !this.m_deckInfoTooltip.IsShown())
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
    if (!GameUtils.IsCardGameplayEventActive(deckBox.GetHeroCardID()))
    {
      DialogManager.Get().ShowClassUpcomingPopup();
    }
    else
    {
      deckBox.enabled = true;
      if (this.IsTouchDragging || this.m_deckTray.IsUpdatingTrayMode())
        return;
      long deckId = deckBox.GetDeckID();
      CollectionDeck deck = CollectionManager.Get().GetDeck(deckId);
      if (deck != null)
      {
        if (deck.IsBeingDeleted())
        {
          Log.DeckTray.Print(string.Format("CollectionDeckTrayDeckListContent.OnDeckBoxVisualRelease(): cannot edit deck {0}; it is being deleted", (object) deck));
          return;
        }
        if (deck.IsSavingChanges())
        {
          Log.DeckTray.PrintWarning("CollectionDeckTrayDeckListContent.OnDeckBoxVisualRelease(): cannot edit deck {0}; waiting for changes to be saved", (object) deck);
          return;
        }
      }
      if (this.IsEditingCards())
      {
        if (!UniversalInputManager.Get().IsTouchMode())
        {
          this.RenameCurrentlyEditingDeck();
        }
        else
        {
          if (!((UnityEngine.Object) this.m_deckInfoTooltip != (UnityEngine.Object) null) || this.m_deckInfoTooltip.IsShown())
            return;
          this.ShowDeckInfo();
        }
      }
      else
      {
        if (!this.IsModeActive())
          return;
        this.m_editingTraySection = traySection;
        this.m_centeringDeckList = this.m_editingTraySection.m_deckBox.GetPositionIndex();
        this.m_newDeckButton.SetEnabled(false);
        CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
        if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
        {
          collectibleDisplay.RequestContentsToShowDeck(deckId);
          collectibleDisplay.HideDeckHelpPopup();
          collectibleDisplay.HideSetFilterTutorial();
        }
        Options.Get().SetBool(Option.HAS_STARTED_A_DECK, true);
      }
    }
  }

  protected void FireDeckCountChangedEvent()
  {
    DeckTrayDeckListContent.DeckCountChanged[] array = this.m_deckCountChangedListeners.ToArray();
    int count = CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK).Count;
    foreach (DeckTrayDeckListContent.DeckCountChanged deckCountChanged in array)
      deckCountChanged(count);
  }

  protected void FireBusyWithDeckEvent(bool busy)
  {
    foreach (DeckTrayDeckListContent.BusyWithDeck busyWithDeck in this.m_busyWithDeckListeners.ToArray())
      busyWithDeck(busy);
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

  protected int UpdateDeckTrayVisuals() => this.UpdateDeckTrayVisuals(false);

  protected int UpdateDeckTrayVisuals(bool rerollAllHeroes, TAG_CLASS heroClassToReroll = TAG_CLASS.INVALID)
  {
    List<CollectionDeck> activeDecks = this.GetActiveDecks();
    for (int index = 0; index < activeDecks.Count && index < this.m_traySections.Count; ++index)
    {
      if (index < activeDecks.Count)
      {
        CollectionDeck deck = activeDecks[index];
        bool rerollFavoriteHero = rerollAllHeroes || deck.GetClass() == heroClassToReroll;
        this.m_traySections[index].m_deckBox.AssignFromCollectionDeck(deck, rerollFavoriteHero);
      }
    }
    return activeDecks.Count;
  }

  protected List<CollectionDeck> GetActiveDecks()
  {
    List<CollectionDeck> activeDecks;
    if (SceneMgr.Get().IsInTavernBrawlMode())
    {
      activeDecks = CollectionManager.Get().GetDecks(TavernBrawlManager.Get().DeckTypeForCurrentBrawlType);
      TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
      int brawlLibraryItemId = tavernBrawlMission == null ? 0 : tavernBrawlMission.SelectedBrawlLibraryItemId;
      activeDecks.RemoveAll((Predicate<CollectionDeck>) (deck => deck.BrawlLibraryItemId != brawlLibraryItemId));
    }
    else if (SceneMgr.Get().IsInDuelsMode())
    {
      activeDecks = new List<CollectionDeck>();
      CollectionDeck collectionDeck = CollectionManager.Get().GetEditedDeck() ?? CollectionManager.Get().GetDuelsDeck();
      if (collectionDeck != null)
        activeDecks.Add(collectionDeck);
    }
    else
      activeDecks = CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK);
    return activeDecks;
  }

  public void OnDeckContentsUpdated(long deckID)
  {
    foreach (TraySection traySection in this.m_traySections)
    {
      if ((UnityEngine.Object) traySection.m_deckBox != (UnityEngine.Object) null)
      {
        CollectionDeck collectionDeck = traySection.m_deckBox.GetCollectionDeck();
        if (collectionDeck != null)
          traySection.m_deckBox.AssignFromCollectionDeck(collectionDeck, false);
      }
    }
  }

  protected void SwapEditTrayIfNeeded(long editDeckID)
  {
    if (editDeckID < 0L)
      return;
    TraySection traySection1 = (TraySection) null;
    foreach (TraySection traySection2 in this.m_traySections)
    {
      if (traySection2.m_deckBox.GetDeckID() == editDeckID)
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
    traySection1.m_deckBox.transform.position = this.m_deckEditTopPos.position;
    traySection1.ShowDeckBoxNoAnim();
    traySection1.m_deckBox.SetEnabled(true, false);
    this.m_editingTraySection = traySection1;
  }

  public bool CanDragToReorderDecks
  {
    get
    {
      NetCache.NetCacheFeatures netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheFeatures>();
      return (netObject == null || netObject.Collection.DeckReordering) && !CollectionManagerDisplay.IsSpecialOneDeckMode() && !this.m_animatingExit;
    }
  }

  protected override void UpdateDragToReorder()
  {
    if (this.m_draggingDeckBox == null)
      return;
    if (!InputCollection.GetMouseButton(0) || !this.CanDragToReorderDecks)
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
      int count = CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK).Count;
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
        CollectionDeck collectionDeck = traySection3.m_deckBox.GetCollectionDeck();
        if (collectionDeck != null)
          collectionDeck.SortOrder = (long) (index3 - 100);
      }
      this.RefreshTraySectionPositions(true);
    }
  }

  private void RefreshTraySectionPositions(bool animateToNewPositions)
  {
    Vector3 localPosition = this.m_traySectionStartPos.localPosition;
    Vector3 vector3_1 = Vector3.zero;
    Transform parent = this.m_traySectionStartPos.parent;
    for (int index = 0; index < 29; ++index)
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
      UnityEngine.Vector2 vector2 = new UnityEngine.Vector2(0.0f, -0.0825f * (float) index);
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

  public delegate void BusyWithDeck(bool busy);

  public delegate void DeckCountChanged(int deckCount);
}
