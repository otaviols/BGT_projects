using Assets;
using Blizzard.T5.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsDeckPickerTrayDisplay : MonoBehaviour
{
  protected static readonly PlatformDependentValue<bool> HIGHLIGHT_SELECTED_DECK = new PlatformDependentValue<bool>(PlatformCategory.Screen)
  {
    Phone = false,
    Tablet = true,
    PC = true
  };
  private const string PLAYBUTTON_FIRESIDE_LATERN_EVENT = "LANTERN";
  public GameObject m_randomDeckPickerTray;
  public Transform m_Hero_Bone;
  public Transform m_Hero_BoneDown;
  public Transform m_HeroName_Bone;
  public Transform m_Ranked_Hero_Bone;
  public Transform m_Ranked_Hero_BoneDown;
  public Transform m_Ranked_HeroName_Bone;
  public Transform m_HeroPower_Bone;
  public Transform m_HeroPower_BoneDown;
  public AsyncReference m_playButtonWidgetReference;
  public UberText m_heroName;
  [CustomEditField(Sections = "Hero Button Placement")]
  public List<GameObject> m_heroPickerButtonBonesByHeroCount;
  public float m_heroPickerButtonHorizontalSpacing;
  public float m_heroPickerButtonVerticalSpacing;
  public GameObject m_hierarchyDetails;
  public GameObject m_basicDeckPageContainer;
  public GameObject m_tooltipPrefab;
  public Transform m_tooltipBone;
  public UberText m_modeName;
  public UIBButton m_backButton;
  public UIBButton m_collectionButton;
  public GameObject m_basicDeckPage;
  public GameObject m_trayFrame;
  public GameObject m_randomDecksShownBone;
  public GameObject m_heroPowerContainer;
  public GameObject m_heroPowerShadowQuad;
  [CustomEditField(Sections = "Prefab References", T = EditType.GAME_OBJECT)]
  public string m_heroButtonWidgetPrefab;
  [CustomEditField(Sections = "Prefab References", T = EditType.GAME_OBJECT)]
  public string m_heroPickerCrownPrefab;
  protected PegasusShared.FormatType m_PreviousFormatType;
  protected bool m_PreviousInRankedPlayMode;
  protected bool m_isMouseOverHeroPower;
  private bool m_playButtonEnabled;
  private bool m_heroRaised = true;
  protected int m_heroDefsLoading = int.MaxValue;
  protected int m_HeroPickerButtonCount;
  protected List<HeroPickerButton> m_heroButtons = new List<HeroPickerButton>();
  protected Map<string, DefLoader.DisposableFullDef> m_heroPowerDefs = new Map<string, DefLoader.DisposableFullDef>();
  protected List<AbsDeckPickerTrayDisplay.DeckTrayLoaded> m_DeckTrayLoadedListeners = new List<AbsDeckPickerTrayDisplay.DeckTrayLoaded>();
  protected List<AbsDeckPickerTrayDisplay.FormatTypePickerClosed> m_FormatTypePickerClosedListeners = new List<AbsDeckPickerTrayDisplay.FormatTypePickerClosed>();
  protected string m_selectedHeroName;
  protected bool m_Loaded;
  protected LockedHeroTooltipPanel m_heroLockedTooltip;
  protected DefLoader.DisposableFullDef m_selectedHeroPowerFullDef;
  protected HeroPickerButton m_selectedHeroButton;
  protected SlidingTray m_slidingTray;
  protected PlayButton m_playButton;
  private AudioSource m_lastPickLine;
  protected PegUIElement m_heroPower;
  protected PegUIElement m_goldenHeroPower;
  protected Actor m_heroActor;
  protected Actor m_heroPowerActor;
  protected Actor m_goldenHeroPowerActor;
  protected Actor m_heroPowerBigCard;
  protected Actor m_goldenHeroPowerBigCard;
  protected List<Transform> m_heroBones;
  protected List<TAG_CLASS> m_validClasses = new List<TAG_CLASS>();
  public Transform empty;

  public virtual void Awake()
  {
    this.m_randomDeckPickerTray.transform.localPosition = this.m_randomDecksShownBone.transform.localPosition;
    DeckPickerTray.Get().SetDeckPickerTrayDisplayReference(this);
    DeckPickerTray.Get().RegisterHandlers();
    if ((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null)
    {
      this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonReleased));
    }
    this.m_playButtonWidgetReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPlayButtonWidgetReady));
    if ((UnityEngine.Object) this.m_heroPowerShadowQuad != (UnityEngine.Object) null)
      this.m_heroPowerShadowQuad.SetActive(false);
    if ((UnityEngine.Object) this.m_heroName != (UnityEngine.Object) null)
    {
      this.m_heroName.RichText = false;
      this.m_heroName.Text = "";
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_slidingTray = this.gameObject.GetComponentInChildren<SlidingTray>();
      this.m_slidingTray.RegisterTrayToggleListener(new SlidingTray.TrayToggledListener(this.OnSlidingTrayToggled));
    }
    PopupDisplayManager.Get().AddPopupShownListener(new Action(this.OnPopupShown));
  }

  protected virtual void OnDestroy()
  {
    PopupDisplayManager.Get()?.RemovePopupShownListener(new Action(this.OnPopupShown));
    if (DeckPickerTray.IsInitialized())
      DeckPickerTray.Get().UnregisterHandlers();
    this.m_heroPowerDefs.DisposeValuesAndClear<string, DefLoader.DisposableFullDef>();
    this.m_selectedHeroPowerFullDef?.Dispose();
    this.m_selectedHeroPowerFullDef = (DefLoader.DisposableFullDef) null;
  }

  private void OnApplicationPause(bool pauseStatus)
  {
    if (!GameMgr.Get().IsFindingGame())
      return;
    GameMgr.Get().CancelFindGame();
  }

  public virtual void HandleGameStartupFailure()
  {
    this.SetPlayButtonEnabled(true);
    this.SetBackButtonEnabled(true);
    this.SetHeroButtonsEnabled(true);
    this.SetHeroRaised(true);
  }

  public virtual void OnServerGameStarted() => FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));

  public virtual void OnServerGameCanceled()
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY || TavernBrawlManager.IsInTavernBrawlFriendlyChallenge())
      return;
    this.HandleGameStartupFailure();
  }

  public bool IsChoosingHero()
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.COLLECTIONMANAGER:
      case SceneMgr.Mode.TAVERN_BRAWL:
        return true;
      default:
        if (!this.IsChoosingHeroForTavernBrawlChallenge() && !this.IsInFiresideGatheringAndInBrawlMode() && !this.IsChoosingHeroForDungeonCrawlAdventure())
          return this.IsChoosingHeroForPvPDungeonRunDeck();
        goto case SceneMgr.Mode.COLLECTIONMANAGER;
    }
  }

  protected virtual bool OnNavigateBackImplementation()
  {
    if (!this.m_backButton.IsEnabled())
      return false;
    if ((SceneMgr.Get() != null ? (int) SceneMgr.Get().GetMode() : 0) == 8)
    {
      if (FiresideGatheringManager.Get() != null && FiresideGatheringManager.Get().CurrentFiresideGatheringMode != FiresideGatheringManager.FiresideGatheringMode.NONE)
        this.BackOutToFiresideGathering();
      else
        this.BackOutToHub();
      if (FriendChallengeMgr.Get() != null)
        FriendChallengeMgr.Get().CancelChallenge();
    }
    this.SetPlayButtonEnabled(false);
    this.SetBackButtonEnabled(false);
    this.SetHeroButtonsEnabled(false);
    GameMgr.Get().CancelFindGame();
    SoundManager.Get().Stop(this.m_lastPickLine);
    return true;
  }

  protected virtual void OnHeroActorLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.OnHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        go.transform.parent = this.m_hierarchyDetails.transform;
        this.UpdateHeroActorOrientation();
        this.m_heroActor.SetUnlit();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_heroActor.m_healthObject);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_heroActor.m_attackObject);
        this.m_heroActor.Hide();
      }
    }
  }

  protected virtual void OnHeroFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef fullDef,
    object userData)
  {
    using (fullDef)
    {
      EntityDef entityDef = fullDef?.EntityDef;
      if (entityDef != null)
      {
        AbsDeckPickerTrayDisplay.HeroFullDefLoadedCallbackData loadedCallbackData = userData as AbsDeckPickerTrayDisplay.HeroFullDefLoadedCallbackData;
        TAG_PREMIUM premium = GameUtils.IsVanillaHero(cardId) ? CollectionManager.Get().GetBestCardPremium(cardId) : TAG_PREMIUM.GOLDEN;
        loadedCallbackData.HeroPickerButton.UpdateDisplay(fullDef, premium);
        Vector3 pos = (UnityEngine.Object) loadedCallbackData.HeroPickerButton.m_raiseAndLowerRoot != (UnityEngine.Object) null ? loadedCallbackData.HeroPickerButton.m_raiseAndLowerRoot.transform.localPosition : loadedCallbackData.HeroPickerButton.transform.localPosition;
        loadedCallbackData.HeroPickerButton.SetOriginalLocalPosition(pos);
        if (entityDef.GetClass() != TAG_CLASS.WHIZBANG)
        {
          string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(entityDef.GetCardId());
          if (!string.IsNullOrEmpty(powerCardIdFromHero))
            this.LoadHeroPowerDef(powerCardIdFromHero, premium);
          else
            Debug.LogErrorFormat("No hero power set up for hero {0}", (object) entityDef.GetCardId());
        }
      }
      --this.m_heroDefsLoading;
    }
  }

  protected virtual void OnSlidingTrayToggled(bool isShowing)
  {
    if (isShowing || !((UnityEngine.Object) PracticePickerTrayDisplay.Get() != (UnityEngine.Object) null) || !PracticePickerTrayDisplay.Get().IsShown())
      return;
    Navigation.GoBack();
  }

  public virtual void ResetCurrentMode()
  {
    if ((UnityEngine.Object) this.m_selectedHeroButton != (UnityEngine.Object) null)
    {
      this.SetPlayButtonEnabled(true);
      this.SetHeroRaised(true);
    }
    else
      this.SetPlayButtonEnabled(false);
    this.SetHeroButtonsEnabled(true);
  }

  public virtual void PreUnload()
  {
  }

  public virtual void InitAssets()
  {
    Log.PlayModeInvestigation.PrintInfo("AbsDeckPickerTrayDisplay.InitAssets() called");
    this.m_PreviousFormatType = Options.GetFormatType();
    this.m_PreviousInRankedPlayMode = Options.GetInRankedPlayMode();
    this.m_HeroPickerButtonCount = this.ValidateHeroCount();
    this.SetupHeroLayout();
    this.LoadHero();
    if (this.ShouldShowHeroPower())
    {
      this.LoadHeroPower();
      this.LoadGoldenHeroPower();
    }
    this.StartCoroutine(this.LoadHeroButtons());
    this.StartCoroutine(this.InitModeWhenReady());
  }

  protected virtual IEnumerator WaitForHeroPickerButtonsLoaded()
  {
    while (this.m_heroButtons.Count < this.m_HeroPickerButtonCount)
      yield return (object) null;
    foreach (HeroPickerButton button in this.m_heroButtons)
    {
      while (button.GetComponent<WidgetTemplate>().IsChangingStates)
        yield return (object) null;
    }
  }

  protected virtual IEnumerator InitDeckDependentElements()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AbsDeckPickerTrayDisplay pickerTrayDisplay = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      pickerTrayDisplay.InitForMode(SceneMgr.Get().GetMode());
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.WaitForHeroPickerButtonsLoaded());
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected virtual IEnumerator InitHeroPickerElements()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AbsDeckPickerTrayDisplay pickerTrayDisplay = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      pickerTrayDisplay.InitHeroPickerButtons();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.WaitForHeroPickerButtonsLoaded());
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected virtual IEnumerator InitModeWhenReady()
  {
    AbsDeckPickerTrayDisplay deckPickerTrayDisplay = this;
    while (deckPickerTrayDisplay.m_heroDefsLoading > 0 || (UnityEngine.Object) deckPickerTrayDisplay.m_heroActor == (UnityEngine.Object) null || ((UnityEngine.Object) deckPickerTrayDisplay.m_heroPowerActor == (UnityEngine.Object) null || (UnityEngine.Object) deckPickerTrayDisplay.m_goldenHeroPowerActor == (UnityEngine.Object) null) && deckPickerTrayDisplay.ShouldShowHeroPower())
      yield return (object) null;
    deckPickerTrayDisplay.m_Loaded = true;
    PlayGameScene scene = SceneMgr.Get().GetScene() as PlayGameScene;
    if ((UnityEngine.Object) scene != (UnityEngine.Object) null)
      scene.OnDeckPickerLoaded(deckPickerTrayDisplay);
    deckPickerTrayDisplay.FireDeckTrayLoadedEvent();
    deckPickerTrayDisplay.InitRichPresence();
    deckPickerTrayDisplay.SetBackButtonEnabled(true);
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY || TavernBrawlManager.IsInTavernBrawlFriendlyChallenge())
    {
      if (FriendChallengeMgr.Get().HasChallenge())
        FriendChallengeMgr.Get().AddChangedListener(new FriendChallengeMgr.ChangedCallback(deckPickerTrayDisplay.OnFriendChallengeChanged));
      else
        SceneMgr.Get().SetNextMode(SceneMgr.Get().GetPrevMode());
    }
  }

  protected virtual void InitForMode(SceneMgr.Mode mode)
  {
    switch (mode)
    {
      case SceneMgr.Mode.COLLECTIONMANAGER:
        this.SetPlayButtonText(GameStrings.Get("GLUE_CHOOSE"));
        break;
      case SceneMgr.Mode.FRIENDLY:
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        if (FiresideGatheringManager.Get().InBrawlMode())
        {
          this.SetHeaderForTavernBrawl();
          break;
        }
        this.SetPlayButtonText(GameStrings.Get(mode == SceneMgr.Mode.FIRESIDE_GATHERING ? "GLUE_CHOOSE_OPPONENT" : "GLUE_CHOOSE"));
        break;
      case SceneMgr.Mode.ADVENTURE:
        if (!((UnityEngine.Object) AdventureConfig.Get() != (UnityEngine.Object) null) || !AdventureConfig.Get().IsHeroSelectedBeforeDungeonCrawlScreenForSelectedAdventure())
          break;
        this.SetPlayButtonText(GameStrings.Get("GLUE_CHOOSE"));
        break;
    }
  }

  protected virtual void InitHeroPickerButtons()
  {
  }

  protected virtual void InitRichPresence(Global.PresenceStatus? newStatus = null)
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.FRIENDLY:
        newStatus = new Global.PresenceStatus?(Global.PresenceStatus.FRIENDLY_DECKPICKER);
        if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
        {
          newStatus = new Global.PresenceStatus?(Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING);
          break;
        }
        break;
      case SceneMgr.Mode.ADVENTURE:
        if (AdventureConfig.Get().CurrentSubScene == AdventureData.Adventuresubscene.PRACTICE)
        {
          newStatus = new Global.PresenceStatus?(Global.PresenceStatus.PRACTICE_DECKPICKER);
          break;
        }
        break;
      case SceneMgr.Mode.TAVERN_BRAWL:
        if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
        {
          newStatus = new Global.PresenceStatus?(Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING);
          break;
        }
        break;
    }
    if (!newStatus.HasValue)
      return;
    PresenceMgr.Get().SetStatus((Enum) newStatus.Value);
  }

  protected virtual void TransitionToFormatType(
    PegasusShared.FormatType formatType,
    bool inRankedPlayMode,
    float transitionSpeed = 2f)
  {
    if (formatType == PegasusShared.FormatType.FT_UNKNOWN)
    {
      RankMgr.LogMessage("formatType being passed in = FT_UNKOWN", nameof (TransitionToFormatType), "D:\\builders\\work\\source\\25.0.0\\Pegasus\\Client\\Assets\\Game\\DeckPickerTray\\AbsDeckPickerTrayDisplay.cs", 602);
    }
    else
    {
      Options.SetFormatType(formatType);
      Options.SetInRankedPlayMode(inRankedPlayMode);
      this.UpdateHeroActorOrientation();
    }
  }

  protected virtual void PlayGame()
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.FRIENDLY:
      case SceneMgr.Mode.TAVERN_BRAWL:
        if (TavernBrawlManager.Get().SelectHeroBeforeMission())
        {
          if ((UnityEngine.Object) this.m_selectedHeroButton == (UnityEngine.Object) null)
          {
            Debug.LogError((object) "Trying to play Tavern Brawl game with no m_selectedHeroButton!");
            return;
          }
          int dbId = GameUtils.TranslateCardIdToDbId(this.m_selectedHeroButton.GetEntityDef().GetCardId());
          if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
          {
            FriendChallengeMgr.Get().SelectHero((long) dbId);
            FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_TAVERN_BRAWL_OPPONENT_WAITING_READY", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
            break;
          }
          TavernBrawlManager.Get().StartGameWithHero(dbId);
          break;
        }
        break;
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        bool flag1 = FiresideGatheringManager.Get().InBrawlMode();
        bool flag2 = TavernBrawlManager.Get().SelectHeroBeforeMission();
        bool flag3 = false;
        if (TavernBrawlManager.Get().CurrentMission() != null)
          flag3 = GameUtils.IsAIMission(TavernBrawlManager.Get().CurrentMission().missionId);
        if (flag1 & flag3)
        {
          if (TavernBrawlManager.Get().SelectHeroBeforeMission())
          {
            int dbId = GameUtils.TranslateCardIdToDbId(this.m_selectedHeroButton.GetEntityDef().GetCardId());
            TavernBrawlManager.Get().StartGameWithHero(dbId);
            break;
          }
          break;
        }
        if (flag1 & flag2)
        {
          int dbId = GameUtils.TranslateCardIdToDbId(this.m_selectedHeroButton.GetEntityDef().GetCardId());
          FriendChallengeMgr.Get().SelectHeroBeforeSendingChallenge((long) dbId);
        }
        if (!flag3 && (flag1 & flag2 || !flag1))
        {
          FiresideGatheringDisplay.Get().ShowOpponentPickerTray((Action) (() => this.SetPlayButtonEnabled(true)));
          this.SetPlayButtonEnabled(false);
          break;
        }
        break;
    }
    SoundManager.Get().Stop(this.m_lastPickLine);
  }

  protected virtual void ShowHero()
  {
    this.m_heroActor.Show();
    string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(this.m_heroActor.GetEntityDef().GetCardId());
    if (this.ShouldShowHeroPower() && !string.IsNullOrEmpty(powerCardIdFromHero))
    {
      this.ShowHeroPower(this.m_heroActor.GetPremium());
    }
    else
    {
      this.m_heroPowerShadowQuad.SetActive(false);
      if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
        this.m_heroPowerActor.Hide();
      if ((UnityEngine.Object) this.m_goldenHeroPower != (UnityEngine.Object) null)
        this.m_goldenHeroPowerActor.Hide();
    }
    if (this.m_selectedHeroName != null)
      return;
    this.m_heroName.Text = "";
  }

  protected virtual void SelectHero(HeroPickerButton button, bool showTrayForPhone = true)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) this.m_selectedHeroButton && !(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.DeselectLastSelectedHero();
    if ((bool) AbsDeckPickerTrayDisplay.HIGHLIGHT_SELECTED_DECK)
      button.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    else
      button.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
    this.m_selectedHeroButton = button;
    this.UpdateHeroInfo(button);
    button.SetSelected(true);
    this.ShowPreconHero(true);
    this.RemoveHeroLockedTooltip();
    if ((bool) UniversalInputManager.UsePhoneUI & showTrayForPhone)
      this.m_slidingTray.ToggleTraySlider(true);
    bool enable = this.IsHeroPlayable(button);
    if (enable && !NotificationManager.Get().IsQuotePlaying && button.HasCardDef)
      SoundManager.Get().LoadAndPlay((AssetReference) button.HeroPickerSelectedPrefab, button.gameObject, 1f, new SoundManager.LoadedCallback(this.OnLastPickLineLoaded));
    this.SetPlayButtonEnabled(enable);
  }

  protected virtual void UpdateHeroInfo(HeroPickerButton button)
  {
  }

  protected virtual void BackOutToHub()
  {
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
      return;
    if (FriendChallengeMgr.Get() != null)
      FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
  }

  protected virtual void BackOutToFiresideGathering()
  {
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FIRESIDE_GATHERING))
      return;
    if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null)
      FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.FIRESIDE_GATHERING);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_slidingTray.ToggleTraySlider(false);
  }

  protected void UpdateValidHeroClasses()
  {
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    this.m_validClasses = Options.GetFormatType() != PegasusShared.FormatType.FT_CLASSIC || mode == SceneMgr.Mode.ADVENTURE ? new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.ORDERED_HERO_CLASSES) : new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.CLASSIC_ORDERED_HERO_CLASSES);
    if (!this.IsChoosingHero())
      this.m_validClasses.Add(TAG_CLASS.WHIZBANG);
    List<ClassExclusionsDbfRecord> classExclusions = this.GetClassExclusions(mode);
    for (int index = 0; index < classExclusions.Count; ++index)
      this.m_validClasses.Remove((TAG_CLASS) classExclusions[index].ClassId);
  }

  protected List<ClassExclusionsDbfRecord> GetClassExclusions(
    SceneMgr.Mode mode)
  {
    List<ClassExclusionsDbfRecord> classExclusions = new List<ClassExclusionsDbfRecord>();
    ScenarioDbId? nullable1 = new ScenarioDbId?();
    if (mode == SceneMgr.Mode.ADVENTURE)
      nullable1 = new ScenarioDbId?(AdventureConfig.Get().GetMission());
    if (mode == SceneMgr.Mode.TAVERN_BRAWL || mode == SceneMgr.Mode.FIRESIDE_GATHERING && FiresideGatheringManager.Get().InBrawlMode() || FriendChallengeMgr.Get().IsChallengeTavernBrawl())
      nullable1 = new ScenarioDbId?((ScenarioDbId) TavernBrawlManager.Get().CurrentMission().missionId);
    if (nullable1.HasValue)
    {
      ScenarioDbId? nullable2 = nullable1;
      ScenarioDbId scenarioDbId = ScenarioDbId.INVALID;
      if (!(nullable2.GetValueOrDefault() == scenarioDbId & nullable2.HasValue))
      {
        ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) nullable1.Value);
        classExclusions.AddRange((IEnumerable<ClassExclusionsDbfRecord>) record.ClassExclusions);
      }
    }
    return classExclusions;
  }

  protected virtual int ValidateHeroCount()
  {
    this.UpdateValidHeroClasses();
    return this.m_validClasses.Count;
  }

  protected virtual bool ShouldShowHeroPower() => false;

  private bool DeckPickerInRankedPlayMode() => SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && Options.GetInRankedPlayMode();

  private Transform GetActiveHeroBone()
  {
    bool flag = this.DeckPickerInRankedPlayMode();
    return this.m_heroRaised ? (!flag ? this.m_Hero_Bone : this.m_Ranked_Hero_Bone) : (!flag ? this.m_Hero_BoneDown : this.m_Ranked_Hero_BoneDown);
  }

  private Transform GetActiveHeroNameBone() => !this.DeckPickerInRankedPlayMode() ? this.m_HeroName_Bone : this.m_Ranked_HeroName_Bone;

  private void UpdateHeroActorOrientation()
  {
    if ((UnityEngine.Object) this.m_heroActor != (UnityEngine.Object) null)
    {
      iTween.Stop(this.m_heroActor.gameObject);
      Transform activeHeroBone = this.GetActiveHeroBone();
      if ((UnityEngine.Object) activeHeroBone != (UnityEngine.Object) null)
      {
        this.m_heroActor.transform.localScale = activeHeroBone.localScale;
        this.m_heroActor.transform.localPosition = activeHeroBone.localPosition;
      }
    }
    if (!((UnityEngine.Object) this.m_heroName != (UnityEngine.Object) null))
      return;
    Transform activeHeroNameBone = this.GetActiveHeroNameBone();
    if (!((UnityEngine.Object) activeHeroNameBone != (UnityEngine.Object) null))
      return;
    this.m_heroName.transform.localScale = activeHeroNameBone.localScale;
    this.m_heroName.transform.localPosition = activeHeroNameBone.localPosition;
  }

  protected virtual void SetHeroRaised(bool raised)
  {
    this.m_heroRaised = raised;
    Transform activeHeroBone = this.GetActiveHeroBone();
    if ((UnityEngine.Object) activeHeroBone == (UnityEngine.Object) null || (UnityEngine.Object) this.m_HeroPower_Bone == (UnityEngine.Object) null || (UnityEngine.Object) this.m_HeroPower_BoneDown == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "SetHeroRaised tried using transforms that were undefined!");
    }
    else
    {
      Vector3 localPosition = activeHeroBone.localPosition;
      Vector3 position = raised ? this.m_HeroPower_Bone.localPosition : this.m_HeroPower_BoneDown.localPosition;
      this.MoveToRaisedPosition(localPosition, this.m_heroActor, raised);
      if (!this.ShouldShowHeroPower())
        return;
      this.m_heroPowerShadowQuad.SetActive(raised);
      this.MoveToRaisedPosition(position, this.m_heroPowerActor, raised, this.m_heroPower);
      this.MoveToRaisedPosition(position, this.m_goldenHeroPowerActor, raised, this.m_goldenHeroPower);
    }
  }

  private void MoveToRaisedPosition(
    Vector3 position,
    Actor actor,
    bool raised,
    PegUIElement pegUiElement = null)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    Hashtable args = iTween.Hash((object) nameof (position), (object) position, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutExpo, (object) "islocal", (object) true);
    iTween.MoveTo(actor.gameObject, args);
    if (!((UnityEngine.Object) pegUiElement != (UnityEngine.Object) null))
      return;
    Collider component = pegUiElement.GetComponent<Collider>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.enabled = raised;
    else
      Debug.LogWarning((object) ("Could not locate Collider for " + pegUiElement.name + " when trying to SetHeroRaised"));
  }

  protected virtual void SetPlayButtonEnabled(bool enable)
  {
    if (enable && SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY && !FriendChallengeMgr.Get().HasChallenge())
      return;
    this.m_playButtonEnabled = enable;
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null) || this.m_playButton.IsEnabled() == enable)
      return;
    if (enable)
      this.m_playButton.Enable();
    else
      this.m_playButton.Disable();
  }

  protected virtual void SetCollectionButtonEnabled(bool enable)
  {
    if (!((UnityEngine.Object) this.m_collectionButton != (UnityEngine.Object) null))
      return;
    this.m_collectionButton.SetEnabled(enable);
    this.m_collectionButton.Flip(enable);
  }

  protected virtual void SetHeroButtonsEnabled(bool enable)
  {
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
    {
      if (!heroButton.IsLocked() || !enable)
        heroButton.SetEnabled(enable);
    }
  }

  protected virtual void SetHeaderForTavernBrawl()
  {
    string key = "GLUE_CHOOSE";
    if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
      key = TavernBrawlManager.Get().CurrentBrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING ? "GLUE_BRAWL_PATRON" : "GLUE_BRAWL_FRIEND";
    else if (TavernBrawlManager.Get().SelectHeroBeforeMission())
    {
      key = "GLUE_BRAWL";
      if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FIRESIDE_GATHERING)
      {
        key = "GLUE_CHOOSE_OPPONENT";
        TavernBrawlMission mission1 = TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING);
        TavernBrawlMission mission2 = TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
        if (((FiresideGatheringManager.Get().CurrentFiresideGatheringMode != FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL || mission1 == null ? 0 : (mission1.GameType == GameType.GT_FSG_BRAWL_1P_VS_AI ? 1 : 0)) | (FiresideGatheringManager.Get().CurrentFiresideGatheringMode != FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE_BRAWL || mission2 == null ? (false ? 1 : 0) : (mission2.GameType == GameType.GT_TB_1P_VS_AI ? 1 : 0))) != 0)
          key = "GLUE_CHOOSE";
      }
    }
    this.SetPlayButtonText(GameStrings.Get(key));
  }

  protected virtual bool IsHeroPlayable(HeroPickerButton button) => !button.IsLocked();

  public virtual int GetSelectedHeroID()
  {
    if ((UnityEngine.Object) this.m_selectedHeroButton != (UnityEngine.Object) null)
    {
      GuestHeroDbfRecord guestHero = this.m_selectedHeroButton.GetGuestHero();
      if (guestHero != null)
        return guestHero.CardId;
    }
    return 0;
  }

  public virtual long GetSelectedDeckID() => !((UnityEngine.Object) this.m_selectedHeroButton == (UnityEngine.Object) null) ? this.m_selectedHeroButton.GetPreconDeckID() : 0L;

  protected abstract void GoBackUntilOnNavigateBackCalled();

  protected virtual void OnBackButtonReleased(UIEvent e) => Navigation.GoBack();

  protected virtual void OnPlayGameButtonReleased(UIEvent e)
  {
    if (!Network.IsLoggedIn() && SceneMgr.Get().GetMode() != SceneMgr.Mode.COLLECTIONMANAGER)
    {
      DialogManager.Get().ShowReconnectHelperDialog();
    }
    else
    {
      if (SetRotationManager.Get().CheckForSetRotationRollover() || PlayerMigrationManager.Get() != null && PlayerMigrationManager.Get().CheckForPlayerMigrationRequired())
        return;
      this.SetPlayButtonEnabled(false);
      this.SetHeroButtonsEnabled(false);
      this.PlayGame();
    }
  }

  protected virtual void OnHeroButtonReleased(UIEvent e)
  {
    this.SelectHero((HeroPickerButton) e.GetElement());
    SoundManager.Get().LoadAndPlay((AssetReference) "tournament_screen_select_hero.prefab:2b9bdf587ac07084b8f7d5c4bce33ecf");
  }

  protected virtual void OnHeroMouseOver(UIEvent e)
  {
    if (e == null)
      return;
    ((HeroPickerButton) e.GetElement()).SetHighlightState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_hero_mouse_over.prefab:653cc8000b988cd468d2210a209adce6");
  }

  protected virtual void OnHeroMouseOut(UIEvent e)
  {
    HeroPickerButton element = (HeroPickerButton) e.GetElement();
    if ((bool) UniversalInputManager.UsePhoneUI && element.IsSelected())
      return;
    element.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
  }

  protected virtual void OnHeroPowerMouseOver(UIEvent e)
  {
    this.m_isMouseOverHeroPower = true;
    if (this.m_heroActor.GetPremium() == TAG_PREMIUM.GOLDEN)
    {
      if ((UnityEngine.Object) this.m_goldenHeroPowerBigCard == (UnityEngine.Object) null)
        AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER, TAG_PREMIUM.GOLDEN), new PrefabCallback<GameObject>(this.OnGoldenHeroPowerLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      else
        this.ShowHeroPowerBigCard(true);
    }
    else if ((UnityEngine.Object) this.m_heroPowerBigCard == (UnityEngine.Object) null)
      AssetLoader.Get().InstantiatePrefab((AssetReference) "History_HeroPower.prefab:e73edf8ccea2b11429093f7a448eef53", new PrefabCallback<GameObject>(this.OnHeroPowerLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    else
      this.ShowHeroPowerBigCard();
  }

  protected virtual void OnHeroPowerMouseOut(UIEvent e)
  {
    this.m_isMouseOverHeroPower = false;
    if ((UnityEngine.Object) this.m_heroPowerBigCard != (UnityEngine.Object) null)
    {
      iTween.Stop(this.m_heroPowerBigCard.gameObject);
      this.m_heroPowerBigCard.Hide();
    }
    if (!((UnityEngine.Object) this.m_goldenHeroPowerBigCard != (UnityEngine.Object) null))
      return;
    iTween.Stop(this.m_goldenHeroPowerBigCard.gameObject);
    this.m_goldenHeroPowerBigCard.Hide();
  }

  protected void LoadHeroPowerDef(string heroPowerCardId, TAG_PREMIUM premium = TAG_PREMIUM.NORMAL)
  {
    DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(heroPowerCardId, new CardPortraitQuality(3, premium));
    this.m_heroPowerDefs.SetOrReplaceDisposable<string, DefLoader.DisposableFullDef>(heroPowerCardId, fullDef);
  }

  protected void OnPlayButtonWidgetReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButton = visualController.GetComponent<PlayButton>();
      if ((UnityEngine.Object) this.m_playButton == (UnityEngine.Object) null)
        return;
      if (FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL)
        visualController.Owner.TriggerEvent("LANTERN", new Widget.TriggerEventParameters());
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayGameButtonReleased));
      this.SetPlayButtonEnabled(this.m_playButtonEnabled);
    }
  }

  protected void OnHeroPickerButtonWidgetReady(WidgetInstance widget)
  {
    HeroPickerButton componentInChildren = widget.GetComponentInChildren<HeroPickerButton>();
    this.m_heroButtons.Add(componentInChildren);
    this.SetUpHeroPickerButton(componentInChildren, this.m_heroButtons.Count - 1);
    componentInChildren.Lock();
    componentInChildren.Activate(false);
    componentInChildren.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnHeroButtonReleased));
    componentInChildren.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnHeroMouseOver));
    componentInChildren.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHeroMouseOut));
    Vector3 pos = (UnityEngine.Object) componentInChildren.m_raiseAndLowerRoot != (UnityEngine.Object) null ? componentInChildren.m_raiseAndLowerRoot.transform.localPosition : this.transform.localPosition;
    componentInChildren.SetOriginalLocalPosition(pos);
  }

  protected void OnHeroPowerActorLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_heroPower = go.AddComponent<PegUIElement>();
        go.AddComponent<BoxCollider>();
        GameUtils.SetParent(go, this.m_heroPowerContainer);
        go.transform.localScale = this.m_HeroPower_Bone.localScale;
        go.transform.localPosition = this.m_HeroPower_Bone.localPosition;
        this.m_heroPowerActor.SetUnlit();
        this.m_heroPower.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnHeroPowerMouseOver));
        this.m_heroPower.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHeroPowerMouseOut));
        this.m_heroPowerActor.Hide();
        this.m_heroPower.GetComponent<Collider>().enabled = false;
        this.m_heroName.Text = "";
        this.StartCoroutine(this.UpdateHeroSkinHeroPower());
      }
    }
  }

  protected void OnGoldenHeroPowerActorLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_goldenHeroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_goldenHeroPowerActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_goldenHeroPower = go.AddComponent<PegUIElement>();
        go.AddComponent<BoxCollider>();
        GameUtils.SetParent(go, this.m_heroPowerContainer);
        go.transform.localScale = this.m_HeroPower_Bone.localScale;
        go.transform.localPosition = this.m_HeroPower_Bone.localPosition;
        this.m_goldenHeroPowerActor.SetUnlit();
        this.m_goldenHeroPowerActor.SetPremium(TAG_PREMIUM.GOLDEN);
        this.m_goldenHeroPower.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnHeroPowerMouseOver));
        this.m_goldenHeroPower.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHeroPowerMouseOut));
        this.m_goldenHeroPowerActor.Hide();
        this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
        this.m_heroName.Text = "";
      }
    }
  }

  protected void OnHeroPowerLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.LoadHeroPowerCallback() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.LoadHeroPowerCallback() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        component.transform.parent = this.m_heroPower.transform;
        component.TurnOffCollider();
        LayerUtils.SetLayer(component.gameObject, this.m_heroPower.gameObject.layer);
        this.m_heroPowerBigCard = component;
        if (!this.m_isMouseOverHeroPower)
          return;
        this.ShowHeroPowerBigCard();
      }
    }
  }

  protected void OnGoldenHeroPowerLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.LoadHeroPowerCallback() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AbsDeckPickerTrayDisplay.LoadHeroPowerCallback() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        component.transform.parent = this.m_heroPower.transform;
        component.TurnOffCollider();
        LayerUtils.SetLayer(component.gameObject, this.m_heroPower.gameObject.layer);
        this.m_goldenHeroPowerBigCard = component;
        if (!this.m_isMouseOverHeroPower)
          return;
        this.ShowHeroPowerBigCard(true);
      }
    }
  }

  protected void OnPopupShown()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_slidingTray.ToggleTraySlider(false, animate: false);
  }

  private void OnLastPickLineLoaded(AudioSource source, object callbackData)
  {
    SoundManager.Get().Stop(this.m_lastPickLine);
    this.m_lastPickLine = source;
  }

  protected virtual void OnFriendChallengeWaitingForOpponentDialogResponse(
    AlertPopup.Response response,
    object userData)
  {
    if (response != AlertPopup.Response.CANCEL || FriendChallengeMgr.Get().AmIInGameState())
      return;
    this.ResetCurrentMode();
    FriendChallengeMgr.Get().DeselectDeckOrHero();
    FriendlyChallengeHelper.Get().StopWaitingForFriendChallenge();
  }

  protected virtual void OnFriendChallengeChanged(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData,
    object userData)
  {
    switch (challengeEvent)
    {
      case FriendChallengeEvent.SELECTED_DECK_OR_HERO:
        if (SceneMgr.Get().IsInTavernBrawlMode() || player == BnetPresenceMgr.Get().GetMyPlayer() || !FriendChallengeMgr.Get().DidISelectDeckOrHero())
          break;
        FriendlyChallengeHelper.Get().HideFriendChallengeWaitingForOpponentDialog();
        break;
      case FriendChallengeEvent.DESELECTED_DECK_OR_HERO:
        if (player == BnetPresenceMgr.Get().GetMyPlayer())
          break;
        if (FriendChallengeMgr.Get().DidISelectDeckOrHero())
        {
          FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_DECK", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
          break;
        }
        this.ResetCurrentMode();
        this.SetBackButtonEnabled(true);
        break;
      case FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE:
      case FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS:
      case FriendChallengeEvent.QUEUE_CANCELED:
        FriendlyChallengeHelper.Get().StopWaitingForFriendChallenge();
        this.GoBackUntilOnNavigateBackCalled();
        break;
    }
  }

  protected IEnumerator LoadHeroButtons(int? m_cheatOverrideHeroPickerButtonCount = null)
  {
    AbsDeckPickerTrayDisplay pickerTrayDisplay1 = this;
    pickerTrayDisplay1.m_HeroPickerButtonCount = !m_cheatOverrideHeroPickerButtonCount.HasValue ? pickerTrayDisplay1.ValidateHeroCount() : m_cheatOverrideHeroPickerButtonCount.Value;
    pickerTrayDisplay1.SetupHeroLayout();
    foreach (Component heroButton in pickerTrayDisplay1.m_heroButtons)
      UnityEngine.Object.Destroy((UnityEngine.Object) heroButton.gameObject);
    pickerTrayDisplay1.m_heroButtons.Clear();
    HeroPickerDataModel heroPickerDataModel = pickerTrayDisplay1.GetHeroPickerDataModel();
    for (int index = 0; index < pickerTrayDisplay1.m_HeroPickerButtonCount; ++index)
    {
      AbsDeckPickerTrayDisplay pickerTrayDisplay = pickerTrayDisplay1;
      WidgetInstance heroPickerButtonWidget = WidgetInstance.Create(pickerTrayDisplay1.m_heroButtonWidgetPrefab);
      if (heroPickerDataModel != null)
        heroPickerButtonWidget.BindDataModel((IDataModel) heroPickerDataModel, false);
      heroPickerButtonWidget.RegisterReadyListener((Action<object>) (_ => pickerTrayDisplay.OnHeroPickerButtonWidgetReady(heroPickerButtonWidget)), (object) null, true);
    }
    yield return (object) pickerTrayDisplay1.StartCoroutine(pickerTrayDisplay1.InitDeckDependentElements());
    pickerTrayDisplay1.StartCoroutine(pickerTrayDisplay1.InitHeroPickerElements());
  }

  protected void SetupHeroLayout()
  {
    if (this.m_HeroPickerButtonCount <= 0 || this.m_HeroPickerButtonCount > this.m_heroPickerButtonBonesByHeroCount.Count || (UnityEngine.Object) this.m_heroPickerButtonBonesByHeroCount[this.m_HeroPickerButtonCount] == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintWarning("Deck/Class Picker Instantiated with an unsupported amount of heroes: " + (object) this.m_HeroPickerButtonCount);
    }
    else
    {
      GameObject gameObject = this.m_heroPickerButtonBonesByHeroCount[this.m_HeroPickerButtonCount];
      this.m_heroBones = new List<Transform>();
      this.m_heroBones.AddRange((IEnumerable<Transform>) gameObject.GetComponentsInChildren<Transform>());
      this.m_heroBones.RemoveAt(0);
      if (this.m_heroBones.Count == this.m_HeroPickerButtonCount)
        return;
      Log.Adventures.PrintWarning("Layout for {0} heroes yielded an incorrect amount of transforms. This will result in errors when displaying heroes!", (object) this.m_HeroPickerButtonCount);
    }
  }

  protected void LoadHero() => AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d", new PrefabCallback<GameObject>(this.OnHeroActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);

  protected void LoadHeroPower() => AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);

  protected void LoadGoldenHeroPower() => AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_HERO_POWER, TAG_PREMIUM.GOLDEN), new PrefabCallback<GameObject>(this.OnGoldenHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);

  protected IEnumerator UpdateHeroSkinHeroPower()
  {
    while ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null || !this.m_heroActor.HasCardDef)
      yield return (object) null;
    HeroSkinHeroPower componentInChildren = this.m_heroPowerActor.gameObject.GetComponentInChildren<HeroSkinHeroPower>();
    if (!((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null))
    {
      componentInChildren.m_Actor.AlwaysRenderPremiumPortrait = !GameUtils.IsVanillaHero(this.m_heroActor.GetEntityDef().GetCardId());
      componentInChildren.m_Actor.UpdateMaterials();
      componentInChildren.m_Actor.UpdateTextures();
    }
  }

  protected void UpdateHeroPowerInfo(DefLoader.DisposableFullDef def, TAG_PREMIUM premium)
  {
    this.SetHeroPowerActorColliderEnabled();
    this.m_heroPowerActor.SetFullDef(def);
    this.m_selectedHeroPowerFullDef?.Dispose();
    this.m_selectedHeroPowerFullDef = def.Share();
    this.m_heroPowerActor.SetUnlit();
    def.CardDef.m_AlwaysRenderPremiumPortrait = false;
    this.m_heroPowerActor.UpdateAllComponents();
    this.m_goldenHeroPowerActor.SetFullDef(def);
    this.m_goldenHeroPowerActor.UpdateAllComponents();
    this.m_goldenHeroPowerActor.SetUnlit();
    this.ShowHeroPower(premium);
    if (premium == TAG_PREMIUM.GOLDEN || GameUtils.IsVanillaHero(this.m_heroActor.GetEntityDef().GetCardId()))
      return;
    this.StartCoroutine(this.UpdateHeroSkinHeroPower());
  }

  protected void UpdateCustomHeroPowerBigCard(GameObject heroPowerBigCard)
  {
    if (!this.m_heroActor.HasCardDef)
    {
      Debug.LogWarning((object) "AbsDeckPickerTrayDisplay.UpdateCustomHeroPowerBigCard heroCardDef = null!");
    }
    else
    {
      Actor componentInChildren = heroPowerBigCard.GetComponentInChildren<Actor>();
      componentInChildren.AlwaysRenderPremiumPortrait = this.m_heroActor.GetEntityDef().GetCardSet() == TAG_CARD_SET.HERO_SKINS;
      componentInChildren.UpdateMaterials();
    }
  }

  protected void ShowHeroPowerBigCard(bool isGolden = false)
  {
    Actor actor1 = isGolden ? this.m_goldenHeroPowerBigCard : this.m_heroPowerBigCard;
    Actor actor2 = !isGolden ? this.m_goldenHeroPowerBigCard : this.m_heroPowerBigCard;
    if ((UnityEngine.Object) this.m_selectedHeroPowerFullDef?.CardDef == (UnityEngine.Object) null)
      return;
    actor1.SetCardDef(this.m_selectedHeroPowerFullDef.DisposableCardDef);
    actor1.SetEntityDef(this.m_selectedHeroPowerFullDef.EntityDef);
    actor1.UpdateAllComponents();
    actor1.Show();
    if ((UnityEngine.Object) actor2 != (UnityEngine.Object) null)
      actor2.Hide();
    this.UpdateCustomHeroPowerBigCard(actor1.gameObject);
    float num1 = 1f;
    float num2 = 1.5f;
    Vector3 vector3_1 = UniversalInputManager.Get().IsTouchMode() ? new Vector3(0.019f, 0.54f, 3f) : new Vector3(0.019f, 0.54f, -1.12f);
    GameObject gameObject = actor1.gameObject;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      gameObject.transform.localPosition = new Vector3(-11.4f, 0.6f, -0.14f);
      gameObject.transform.localScale = Vector3.one * 3.2f;
      AnimationUtil.GrowThenDrift(gameObject, this.m_HeroPower_Bone.transform.position, 2f);
    }
    else
    {
      Vector3 vector3_2 = PlatformSettings.IsTablet ? new Vector3(0.0f, 0.1f, 0.1f) : new Vector3(0.1f, 0.1f, 0.1f);
      gameObject.transform.localPosition = vector3_1;
      gameObject.transform.localScale = Vector3.one * num1;
      iTween.ScaleTo(gameObject, Vector3.one * num2, 0.15f);
      iTween.MoveTo(gameObject, iTween.Hash((object) "position", (object) (vector3_1 + vector3_2), (object) "isLocal", (object) true, (object) "time", (object) 10));
    }
  }

  protected void ShowHeroPower(TAG_PREMIUM premium)
  {
    if ((UnityEngine.Object) this.m_heroPowerShadowQuad != (UnityEngine.Object) null)
      this.m_heroPowerShadowQuad.SetActive(true);
    if (premium == TAG_PREMIUM.GOLDEN)
    {
      this.m_heroPowerActor.Hide();
      this.m_goldenHeroPowerActor.Show();
      this.m_goldenHeroPower.GetComponent<Collider>().enabled = true;
    }
    else
    {
      this.m_goldenHeroPowerActor.Hide();
      this.m_heroPowerActor.Show();
      this.m_heroPower.GetComponent<Collider>().enabled = true;
    }
  }

  protected void ShowPreconHero(bool show)
  {
    if (show && SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE && AdventureConfig.Get().CurrentSubScene == AdventureData.Adventuresubscene.PRACTICE && (UnityEngine.Object) PracticePickerTrayDisplay.Get() != (UnityEngine.Object) null && PracticePickerTrayDisplay.Get().IsShown())
      return;
    if (show)
    {
      this.ShowHero();
    }
    else
    {
      if ((bool) (UnityEngine.Object) this.m_heroActor)
        this.m_heroActor.Hide();
      if ((bool) (UnityEngine.Object) this.m_heroPowerActor)
        this.m_heroPowerActor.Hide();
      if ((bool) (UnityEngine.Object) this.m_goldenHeroPowerActor)
        this.m_goldenHeroPowerActor.Hide();
      if ((bool) (UnityEngine.Object) this.m_heroPower)
        this.m_heroPower.GetComponent<Collider>().enabled = false;
      if ((bool) (UnityEngine.Object) this.m_goldenHeroPower)
        this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
      this.m_heroName.Text = "";
    }
  }

  protected void HideHeroPowerActor()
  {
    this.m_heroPowerShadowQuad.SetActive(false);
    if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
      this.m_heroPowerActor.Hide();
    if (!((UnityEngine.Object) this.m_goldenHeroPower != (UnityEngine.Object) null))
      return;
    this.m_goldenHeroPowerActor.Hide();
  }

  protected void SetUpHeroPickerButton(HeroPickerButton button, int heroCount)
  {
    GameObject gameObject = button.gameObject;
    Transform parent = gameObject.transform.parent;
    gameObject.name = string.Format("{0}_{1}", (object) gameObject.name, (object) heroCount);
    parent.transform.SetParent(this.m_heroBones[heroCount], false);
    parent.transform.localScale = Vector3.one;
    parent.transform.localPosition = Vector3.zero;
    parent.SetParent(this.m_basicDeckPageContainer.transform, true);
  }

  protected void AddHeroLockedTooltip(string name, string description, TAG_CLASS lockedClass = TAG_CLASS.INVALID)
  {
    this.RemoveHeroLockedTooltip();
    GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.m_tooltipPrefab);
    LayerUtils.SetLayer(go, (bool) UniversalInputManager.UsePhoneUI ? GameLayer.IgnoreFullScreenEffects : GameLayer.Default);
    this.m_heroLockedTooltip = go.GetComponent<LockedHeroTooltipPanel>();
    this.m_heroLockedTooltip.Reset();
    this.m_heroLockedTooltip.Initialize(name, description);
    this.m_heroLockedTooltip.SetLockedClass(lockedClass);
    GameUtils.SetParent((Component) this.m_heroLockedTooltip, (Component) this.m_tooltipBone);
  }

  protected void RemoveHeroLockedTooltip()
  {
    if (!((UnityEngine.Object) this.m_heroLockedTooltip != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_heroLockedTooltip.gameObject);
  }

  protected void DeselectLastSelectedHero()
  {
    if ((UnityEngine.Object) this.m_selectedHeroButton == (UnityEngine.Object) null)
      return;
    this.m_selectedHeroButton.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
    this.m_selectedHeroButton.SetSelected(false);
  }

  protected void FireDeckTrayLoadedEvent()
  {
    foreach (AbsDeckPickerTrayDisplay.DeckTrayLoaded deckTrayLoaded in this.m_DeckTrayLoadedListeners.ToArray())
      deckTrayLoaded();
  }

  protected void FireFormatTypePickerClosedEvent()
  {
    foreach (AbsDeckPickerTrayDisplay.FormatTypePickerClosed typePickerClosed in this.m_FormatTypePickerClosedListeners.ToArray())
      typePickerClosed();
  }

  protected bool IsChoosingHeroForTavernBrawlChallenge() => SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY && FriendChallengeMgr.Get().IsChallengeTavernBrawl();

  protected bool IsInFiresideGatheringAndInBrawlMode() => SceneMgr.Get().GetMode() == SceneMgr.Mode.FIRESIDE_GATHERING && FiresideGatheringManager.Get().InBrawlMode();

  protected bool IsChoosingHeroForDungeonCrawlAdventure() => SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE && GameUtils.DoesAdventureModeUseDungeonCrawlFormat(AdventureConfig.Get().GetSelectedMode());

  protected bool IsChoosingHeroForPvPDungeonRunDeck() => SceneMgr.Get().GetMode() == SceneMgr.Mode.PVP_DUNGEON_RUN;

  protected bool OnPlayButtonPressed_SaveHeroAndAdvanceToDungeonRunIfNecessary()
  {
    AdventureConfig adventureConfig = AdventureConfig.Get();
    AdventureDataDbfRecord adventureDataRecord = adventureConfig.GetSelectedAdventureDataRecord();
    if (!GameUtils.DoesAdventureModeUseDungeonCrawlFormat(AdventureConfig.Get().GetSelectedMode()) || !adventureDataRecord.DungeonCrawlPickHeroFirst)
      return false;
    adventureConfig.SelectedHeroCardDbId = (long) this.m_selectedHeroButton.HeroCardDbId;
    adventureConfig.ChangeSubScene(AdventureData.Adventuresubscene.DUNGEON_CRAWL);
    return true;
  }

  protected void SetBackButtonEnabled(bool enable)
  {
    if (DemoMgr.Get().IsExpoDemo())
    {
      if (enable)
        return;
      enable = false;
    }
    if (!((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null) || this.m_backButton.IsEnabled() == enable)
      return;
    this.m_backButton.SetEnabled(enable);
    this.m_backButton.Flip(enable);
  }

  protected void SetHeroPowerActorColliderEnabled(bool enable = true)
  {
    if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
      this.m_heroPowerActor.GetComponent<Collider>().enabled = enable;
    if (!((UnityEngine.Object) this.m_goldenHeroPower != (UnityEngine.Object) null))
      return;
    this.m_goldenHeroPowerActor.GetComponent<Collider>().enabled = enable;
  }

  protected void SetUpHeroCrowns()
  {
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) AdventureConfig.Get().GetSelectedAdventure(), (int) AdventureConfig.Get().GetSelectedMode());
    GameSaveKeyId saveDataServerKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey;
    if (!GameSaveDataManager.Get().ValidateIfKeyCanBeAccessed(saveDataServerKey, (string) adventureDataRecord.Name))
      return;
    if (adventureDataRecord != null && adventureDataRecord.DungeonCrawlDisplayHeroWinsPerChapter)
    {
      WingDbfRecord recordFromMissionId = GameUtils.GetWingRecordFromMissionId((int) AdventureConfig.Get().GetMission());
      if (recordFromMissionId == null)
      {
        Log.Adventures.PrintError("SetUpHeroCrowns() - No WingRecord found for mission {0}, so cannot set up hero crowns.", (object) AdventureConfig.Get().GetMission());
      }
      else
      {
        GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys progressSubkeys;
        if (!GameSaveDataManager.GetProgressSubkeysForDungeonCrawlWing(recordFromMissionId, out progressSubkeys))
        {
          Log.Adventures.PrintError("GetProgressSubkeysForDungeonCrawlWing could not find progress subkeys for Wing {0}, so we don't know which Heroes to show crowns over.", (object) recordFromMissionId.ID);
        }
        else
        {
          List<long> values;
          if (!GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, progressSubkeys.heroCardWins, out values) || values == null)
            return;
          this.ActivateCrownsForHeroCardDbIds(values);
        }
      }
    }
    else
    {
      long num = 0;
      List<TAG_CLASS> classes = new List<TAG_CLASS>();
      foreach (TAG_CLASS dungeonCrawlProgress in GameSaveDataManager.GetClassesFromDungeonCrawlProgressMap())
      {
        GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys progressSubkeys;
        if (GameSaveDataManager.GetProgressSubkeyForDungeonCrawlClass(dungeonCrawlProgress, out progressSubkeys) && GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, progressSubkeys.runWins, out num) && num > 0L)
          classes.Add(dungeonCrawlProgress);
      }
      this.ActivateCrownsForClasses(classes);
    }
  }

  protected List<Transform> ActivateCrownsForClasses(List<TAG_CLASS> classes)
  {
    List<Transform> transformList = new List<Transform>();
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
    {
      if (classes.Contains(heroButton.m_heroClass))
        heroButton.m_crown.SetActive(true);
    }
    return transformList;
  }

  protected void ActivateCrownsForHeroCardDbIds(List<long> cardDbIds)
  {
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
    {
      EntityDef entityDef = heroButton.GetEntityDef();
      if (entityDef != null)
      {
        int dbId = GameUtils.TranslateCardIdToDbId(entityDef.GetCardId());
        if (cardDbIds.Contains((long) dbId))
          heroButton.m_crown.SetActive(true);
      }
    }
  }

  public void Unload()
  {
    DeckPickerTray.Get().UnregisterHandlers();
    if (FriendChallengeMgr.Get() == null)
      return;
    FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
  }

  public bool IsLoaded() => this.m_Loaded;

  public void AddDeckTrayLoadedListener(AbsDeckPickerTrayDisplay.DeckTrayLoaded dlg) => this.m_DeckTrayLoadedListeners.Add(dlg);

  public void RemoveDeckTrayLoadedListener(AbsDeckPickerTrayDisplay.DeckTrayLoaded dlg) => this.m_DeckTrayLoadedListeners.Remove(dlg);

  public void AddFormatTypePickerClosedListener(
    AbsDeckPickerTrayDisplay.FormatTypePickerClosed dlg)
  {
    this.m_FormatTypePickerClosedListeners.Add(dlg);
  }

  public void RemoveFormatTypePickerClosedListener(
    AbsDeckPickerTrayDisplay.FormatTypePickerClosed dlg)
  {
    this.m_FormatTypePickerClosedListeners.Remove(dlg);
  }

  public void SetPlayButtonText(string text)
  {
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null))
      return;
    this.m_playButton.SetText(text);
  }

  public void SetPlayButtonTextAlpha(float alpha)
  {
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null))
      return;
    this.m_playButton.m_newPlayButtonText.TextAlpha = alpha;
  }

  public void AddPlayButtonListener(UIEventType type, UIEvent.Handler handler)
  {
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null))
      return;
    this.m_playButton.AddEventListener(type, handler);
  }

  public void RemovePlayButtonListener(UIEventType type, UIEvent.Handler handler)
  {
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null))
      return;
    this.m_playButton.RemoveEventListener(type, handler);
  }

  public void SetHeaderText(string text)
  {
    if (!((UnityEngine.Object) this.m_modeName != (UnityEngine.Object) null))
      return;
    this.m_modeName.Text = text;
  }

  public HeroPickerDataModel GetHeroPickerDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (HeroPickerDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(13, out model))
    {
      model = (IDataModel) new HeroPickerDataModel();
      owner.BindDataModel(model);
    }
    return model as HeroPickerDataModel;
  }

  public void CheatLoadHeroButtons(int buttonsToDisplay) => this.StartCoroutine(this.LoadHeroButtons(new int?(buttonsToDisplay)));

  public delegate void DeckTrayLoaded();

  public delegate void FormatTypePickerClosed();

  protected class HeroFullDefLoadedCallbackData
  {
    public HeroFullDefLoadedCallbackData(HeroPickerButton button, TAG_PREMIUM premium)
    {
      this.HeroPickerButton = button;
      this.Premium = premium;
    }

    public HeroPickerButton HeroPickerButton { get; private set; }

    private TAG_PREMIUM Premium
    {
      set => this.\u003CPremium\u003Ek__BackingField = value;
    }
  }
}
