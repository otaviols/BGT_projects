using Assets;
using Blizzard.T5.Configuration;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.Telemetry.WTCG.Client;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class DeckPickerTrayDisplay : AbsDeckPickerTrayDisplay
{
  public Transform m_rankedPlayDisplayWidgetBone;
  public Texture m_emptyHeroTexture;
  public NestedPrefab m_leftArrowNestedPrefab;
  public NestedPrefab m_rightArrowNestedPrefab;
  public GameObject m_modeLabelBg;
  public GameObject m_randomDecksHiddenBone;
  public GameObject m_suckedInRandomDecksBone;
  public HeroXPBar m_xpBarPrefab;
  public GameObject m_rankedWinsPlate;
  public UberText m_rankedWins;
  public BoxCollider m_clickBlocker;
  public Animator m_premadeDeckGlowAnimator;
  public GameObject m_hierarchyDeckTray;
  [CustomEditField(Sections = "Deck Pages")]
  public GameObject m_customDeckPagesRoot;
  [CustomEditField(Sections = "Deck Pages")]
  public GameObject m_customDeckPageUpperBone;
  [CustomEditField(Sections = "Deck Pages")]
  public GameObject m_customDeckPageLowerBone;
  [CustomEditField(Sections = "Deck Pages")]
  public GameObject m_customDeckPageHideBone;
  public Widget m_casualPlayDisplayWidget;
  public GameObject m_missingClassicDeck;
  public HighlightState m_collectionButtonGlow;
  public GameObject m_labelDecoration;
  public List<PlayMakerFSM> formatChangeGlowFSMs;
  public List<PlayMakerFSM> newDeckFormatChangeGlowFSMs;
  public List<GameObject> m_premadeDeckGlowBurstObjects;
  public NestedPrefab m_switchFormatButtonContainer;
  public float m_formatTypePickerYOffset;
  private SwitchFormatButton m_switchFormatButton;
  public GameObject m_TheClockButtonBone;
  public string m_leavingWildGlowEvent;
  public string m_leavingClassicGlowEvent;
  public string m_leavingCasualGlowEvent;
  public string m_enteringWildGlowEvent;
  public string m_enteringClassicGlowEvent;
  public string m_enteringCasualGlowEvent;
  public string m_newDeckLeavingClassicGlowEvent;
  public string m_newDeckEnteringClassicGlowEvent;
  public string m_newDeckLeavingWildGlowEvent;
  public string m_newDeckEnteringWildGlowEvent;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_standardTransitionSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_wildTransitionSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_classicTransitionSound;
  [CustomEditField(Sections = "Deck Sharing")]
  public UIBButton m_DeckShareRequestButton;
  [CustomEditField(Sections = "Deck Sharing")]
  public GameObject m_DeckShareGlowOutQuad;
  [CustomEditField(Sections = "Deck Sharing")]
  public float m_DeckShareGlowOutIntensity;
  [CustomEditField(Sections = "Deck Sharing")]
  public ParticleSystem m_DeckShareParticles;
  [CustomEditField(Sections = "Deck Sharing")]
  public float m_DeckShareTransitionTime = 1f;
  [CustomEditField(Sections = "Phone Only")]
  public SlidingTray m_rankedDetailsTray;
  [CustomEditField(Sections = "Phone Only")]
  public GameObject m_detailsTrayFrame;
  [CustomEditField(Sections = "Phone Only")]
  public Transform m_medalBone_phone;
  [CustomEditField(Sections = "Phone Only")]
  public Mesh m_alternateDetailsTrayMesh;
  [CustomEditField(Sections = "Phone Only")]
  public Material m_arrowButtonShadowMaterial;
  [CustomEditField(Sections = "Mode Background Textures")]
  public DeckPickerTrayDisplay.ModeTextures m_adventureTextures;
  [CustomEditField(Sections = "Mode Background Textures")]
  public DeckPickerTrayDisplay.ModeTextures m_collectionTextures;
  [CustomEditField(Sections = "Mode Background Textures")]
  public DeckPickerTrayDisplay.ModeTextures m_tavernBrawlTextures;
  [CustomEditField(Sections = "Mode Background Textures")]
  public DeckPickerTrayDisplay.ModeTextures m_tournamentTextures;
  [CustomEditField(Sections = "Mode Background Textures")]
  public DeckPickerTrayDisplay.ModeTextures m_friendlyTextures;
  public float m_rankedPlayDisplayShowDelay;
  public float m_rankedPlayDisplayHideDelay;
  private const float TRAY_SLIDE_TIME = 0.25f;
  private static readonly Vector3 INNKEEPER_QUOTE_POS = new Vector3(103f, NotificationManager.DEPTH, 42f);
  private static readonly AssetReference CUSTOM_DECK_PAGE = new AssetReference("CustomDeckPage_Top.prefab:650072e121717c04f89ac014eb3dc290");
  private static readonly AssetReference LOANER_DECK_TIMER = new AssetReference("LoanerDeckTimer.prefab:6d916f882c937614f89791cc925a6a9d");
  private static readonly AssetReference FORMAT_TYPE_PICKER_POPUP_PREFAB = new AssetReference("FormatTypePickerPopup.prefab:aa88133d144782b40b3fd8818084006c");
  private const string CREATE_WILD_DECK_STRING_FORMAT = "GLUE_CREATE_WILD_DECK";
  private const string CREATE_STANDARD_DECK_STRING_FORMAT = "GLUE_CREATE_STANDARD_DECK";
  private const string CREATE_CLASSIC_DECK_STRING_FORMAT = "GLUE_CREATE_CLASSIC_DECK";
  private const string WILD_CLICKED_EVENT_NAME = "WILD_BUTTON_CLICKED";
  private const string STANDARD_CLICKED_EVENT_NAME = "STANDARD_BUTTON_CLICKED";
  private const string CLASSIC_CLICKED_EVENT_NAME = "CLASSIC_BUTTON_CLICKED";
  private const string CASUAL_CLICKED_EVENT_NAME = "CASUAL_BUTTON_CLICKED";
  private const string OPEN = "OPEN";
  private const string SET_ROTATION_OPEN = "SET_ROTATION_OPEN";
  private const string HIDE = "HIDE";
  private const string FORMAT_PICKER_4_BUTTONS = "4BUTTONS";
  private const string FORMAT_PICKER_3_BUTTONS = "3BUTTONS";
  private const string FORMAT_PICKER_2_BUTTONS = "2BUTTONS";
  private UIBButton m_leftArrow;
  private UIBButton m_rightArrow;
  private HeroXPBar m_xpBar;
  private CollectionDeckBoxVisual m_selectedCustomDeckBox;
  private DeckPickerTrayDisplay.ModeTextures m_currentModeTextures;
  private bool m_heroChosen;
  private static Coroutine s_selectHeroCoroutine = (Coroutine) null;
  private DeckPickerMode m_deckPickerMode;
  private int m_currentPageIndex;
  private static DeckPickerTrayDisplay s_instance;
  private RankedPlayDisplay m_rankedPlayDisplay;
  private int m_numPagesToShow = 1;
  private List<CustomDeckPage> m_customPages = new List<CustomDeckPage>();
  private Notification m_expoThankQuote;
  private Notification m_expoIntroQuote;
  private Notification m_switchFormatPopup;
  private Notification m_innkeeperQuote;
  private GameLayer m_defaultDetailsLayer;
  private bool m_usingSharedDecks;
  private bool m_doingDeckShareTransition;
  private bool m_isDeckShareRequestButtonHovered;
  private long m_lastSeasonBonusStarPopUpSeen;
  private long m_bonusStarsPopUpSeenCount;
  private TranslatedMedalInfo m_currentMedalInfo;
  private bool m_inHeroPicker;
  private VisualsFormatType m_visualsFormatType;
  private Widget m_formatTypePickerWidget;
  private Widget m_rankedPlayDisplayWidget;
  private bool m_HasSeenPlayStandardToWildVO;
  private bool m_HasSeenPlayStandardToClassicVO;
  private Coroutine m_showLeftArrowCoroutine;
  private Coroutine m_showRightArrowCoroutine;
  private ScreenEffectsHandle m_screenEffectsHandle;
  private Vector3? m_heroPowerContainerOffset;
  private static readonly Dictionary<int, int> s_mysteriousDeck = new Dictionary<int, int>()
  {
    {
      69723,
      1
    },
    {
      78373,
      2
    },
    {
      76981,
      2
    },
    {
      69548,
      1
    },
    {
      76329,
      1
    },
    {
      77491,
      2
    },
    {
      76968,
      2
    },
    {
      69728,
      1
    },
    {
      76319,
      2
    },
    {
      76310,
      1
    },
    {
      77428,
      2
    },
    {
      76320,
      1
    },
    {
      78155,
      1
    },
    {
      78133,
      2
    },
    {
      77308,
      1
    },
    {
      79557,
      2
    },
    {
      87920,
      1
    },
    {
      86550,
      1
    },
    {
      66870,
      1
    },
    {
      79562,
      2
    },
    {
      66868,
      1
    }
  };
  [CustomEditField(Sections = "Set Rotation Tutorial")]
  public GameObject m_formatTutorialPopUpPrefab;
  [CustomEditField(Sections = "Set Rotation Tutorial")]
  public Transform m_formatTutorialPopUpBone;
  [CustomEditField(Sections = "Set Rotation Tutorial")]
  public Transform m_Switch_Format_Notification_Bone;
  [CustomEditField(Sections = "Set Rotation Tutorial")]
  public Animator m_dimQuad;
  [CustomEditField(Sections = "Set Rotation Tutorial")]
  public PegUIElement m_clickCatcher;
  [CustomEditField(Sections = "Set Rotation Tutorial", T = EditType.SOUND_PREFAB)]
  public string m_wildDeckTransitionSound;
  private DeckPickerTrayDisplay.SetRotationTutorialState m_setRotationTutorialState;
  private float m_showQuestPause = 1f;
  private float m_playVOPause = 1f;
  private bool m_shouldContinue;
  private List<long> m_noticeIdsToAck = new List<long>();

  public override void Awake()
  {
    base.Awake();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    SoundManager.Get().Load((AssetReference) "hero_panel_slide_on.prefab:236147a924d7cb442872b46dddd56132");
    SoundManager.Get().Load((AssetReference) "hero_panel_slide_off.prefab:ed410a050e783564384ca51e701ede4d");
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY)
      LoadingScreen.Get().RegisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
    SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    DeckPickerTrayDisplay.s_instance = this;
    if ((UnityEngine.Object) this.m_collectionButton != (UnityEngine.Object) null)
    {
      if (this.IsDeckSharingActive())
      {
        this.m_collectionButton.gameObject.SetActive(false);
      }
      else
      {
        this.m_collectionButton.gameObject.SetActive(true);
        this.SetCollectionButtonEnabled(this.ShouldShowCollectionButton());
        if (this.m_collectionButton.IsEnabled())
        {
          TelemetryWatcher.WatchFor(TelemetryWatcherWatchType.CollectionManagerFromDeckPicker);
          this.m_collectionButton.SetText(GameStrings.Get("GLUE_MY_COLLECTION"));
          this.m_collectionButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CollectionButtonPress));
        }
      }
    }
    if ((UnityEngine.Object) this.m_DeckShareRequestButton != (UnityEngine.Object) null)
    {
      if (this.IsDeckSharingActive())
      {
        this.m_DeckShareRequestButton.gameObject.SetActive(true);
        this.EnableRequestDeckShareButton(true);
        this.m_DeckShareRequestButton.SetText(GameStrings.Get("GLUE_DECK_SHARE_BUTTON_BORROW_DECKS"));
        this.m_DeckShareRequestButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.RequestDeckShareButtonPress));
        this.m_DeckShareRequestButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.RequestDeckShareButtonOver));
        this.m_DeckShareRequestButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.RequestDeckShareButtonOut));
      }
      else
        this.m_DeckShareRequestButton.gameObject.SetActive(false);
    }
    if ((UnityEngine.Object) this.m_DeckShareGlowOutQuad != (UnityEngine.Object) null)
      this.m_DeckShareGlowOutQuad.SetActive(false);
    this.m_xpBar = UnityEngine.Object.Instantiate<HeroXPBar>(this.m_xpBarPrefab);
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    this.m_xpBar.m_soloLevelLimit = netObject == null ? 60 : netObject.XPSoloLimit;
  }

  private void Start()
  {
    Navigation.PushIfNotOnTop(new Navigation.NavigateBackHandler(DeckPickerTrayDisplay.OnNavigateBack));
    this.m_leftArrow = this.m_leftArrowNestedPrefab.PrefabGameObject().GetComponent<UIBButton>();
    this.m_leftArrow.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnShowPreviousPage));
    this.m_rightArrow = this.m_rightArrowNestedPrefab.PrefabGameObject().GetComponent<UIBButton>();
    this.m_rightArrow.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnShowNextPage));
    this.UpdatePageArrows();
    this.m_currentMedalInfo = RankMgr.Get().GetLocalPlayerMedalInfo().GetCurrentMedalForCurrentFormatType();
    this.m_formatTypePickerWidget = (Widget) WidgetInstance.Create((string) DeckPickerTrayDisplay.FORMAT_TYPE_PICKER_POPUP_PREFAB);
    this.m_formatTypePickerWidget.Hide();
    this.m_formatTypePickerWidget.RegisterReadyListener((Action<object>) (_ => this.OnFormatTypePickerPopupReady()), (object) null, true);
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    this.HideDemoQuotes();
    if ((UnityEngine.Object) TournamentDisplay.Get() != (UnityEngine.Object) null)
      TournamentDisplay.Get().RemoveMedalChangedListener(new TournamentDisplay.DelMedalChanged(this.OnMedalChanged));
    if (FriendChallengeMgr.Get() != null && (UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null)
      FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(((AbsDeckPickerTrayDisplay) DeckPickerTrayDisplay.Get()).OnFriendChallengeChanged));
    if (SceneMgr.Get() != null && SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.FRIENDLY && SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
      FriendChallengeMgr.Get().CancelChallenge();
    DeckPickerTrayDisplay.s_instance = (DeckPickerTrayDisplay) null;
  }

  public static DeckPickerTrayDisplay Get() => DeckPickerTrayDisplay.s_instance;

  public void SetInHeroPicker() => this.m_inHeroPicker = true;

  public void OverridePlayButtonCallback(UIEvent.Handler callback)
  {
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null))
      return;
    this.m_playButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(((AbsDeckPickerTrayDisplay) this).OnPlayGameButtonReleased));
    this.m_playButton.AddEventListener(UIEventType.RELEASE, callback);
  }

  private void OnShowNextPage(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "hero_panel_slide_off.prefab:ed410a050e783564384ca51e701ede4d");
    this.ShowNextPage();
  }

  public override void ResetCurrentMode()
  {
    if ((UnityEngine.Object) this.m_selectedCustomDeckBox != (UnityEngine.Object) null)
    {
      this.SetPlayButtonEnabled(true);
      this.SetHeroRaised(true);
    }
    else if ((UnityEngine.Object) this.m_selectedHeroButton != (UnityEngine.Object) null)
    {
      this.SetHeroRaised(true);
      this.SetPlayButtonEnabled(!this.m_selectedHeroButton.IsLocked());
    }
    this.SetHeroButtonsEnabled(true);
  }

  public int GetSelectedHeroLevel() => (UnityEngine.Object) this.m_selectedHeroButton == (UnityEngine.Object) null ? 0 : GameUtils.GetHeroLevel(this.m_selectedHeroButton.GetEntityDef().GetClass()).CurrentLevel.Level;

  public void ToggleRankedDetailsTray(bool shown)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_rankedDetailsTray.ToggleTraySlider(shown);
  }

  public override long GetSelectedDeckID() => (UnityEngine.Object) null != (UnityEngine.Object) this.m_selectedCustomDeckBox ? this.m_selectedCustomDeckBox.GetDeckID() : base.GetSelectedDeckID();

  public int GetSelectedDeckTemplateID() => (UnityEngine.Object) this.m_selectedCustomDeckBox != (UnityEngine.Object) null ? this.m_selectedCustomDeckBox.GetDeckTemplateId() : 0;

  public CollectionDeck GetSelectedCollectionDeck() => !((UnityEngine.Object) this.m_selectedCustomDeckBox == (UnityEngine.Object) null) ? this.m_selectedCustomDeckBox.GetCollectionDeck() : (CollectionDeck) null;

  public void UpdateCreateDeckText()
  {
    string key;
    if (SceneMgr.Get().IsInTavernBrawlMode())
    {
      key = TavernBrawlManager.Get().CurrentSeasonBrawlMode == TavernBrawlMode.TB_MODE_HEROIC ? "GLOBAL_HEROIC_BRAWL" : "GLOBAL_TAVERN_BRAWL";
    }
    else
    {
      PegasusShared.FormatType formatType = Options.GetFormatType();
      switch (formatType)
      {
        case PegasusShared.FormatType.FT_WILD:
          key = "GLUE_CREATE_WILD_DECK";
          break;
        case PegasusShared.FormatType.FT_STANDARD:
          key = "GLUE_CREATE_STANDARD_DECK";
          break;
        case PegasusShared.FormatType.FT_CLASSIC:
          key = "GLUE_CREATE_CLASSIC_DECK";
          break;
        default:
          Debug.LogError((object) ("DeckPickerTrayDisplay.UpdateCreateDeckText called in unsupported format type: " + formatType.ToString()));
          this.SetHeaderText("UNSUPPORTED DECK TEXT " + formatType.ToString());
          return;
      }
    }
    this.SetHeaderText(GameStrings.Get(key));
  }

  public bool UpdateRankedClassWinsPlate()
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && (UnityEngine.Object) this.m_heroActor != (UnityEngine.Object) null && this.m_heroActor.GetEntityDef() != null && Options.GetInRankedPlayMode())
    {
      GameUtils.HeroSkinAchievements skinAchievements;
      if (!GameUtils.HERO_SKIN_ACHIEVEMENTS.TryGetValue(this.m_heroActor.GetEntityDef().GetClass(), out skinAchievements))
      {
        this.m_rankedWinsPlate.SetActive(false);
        return false;
      }
      RankedWinsPlate component = this.m_rankedWinsPlate.GetComponent<RankedWinsPlate>();
      component.TooltipString = GameStrings.Get("GLUE_TOOLTIP_GOLDEN_WINS_DESC");
      AchievementDataModel achievementDataModel1 = AchievementManager.Get().GetAchievementDataModel(skinAchievements.Golden500Win);
      AchievementDataModel achievementDataModel2 = AchievementManager.Get().GetAchievementDataModel(skinAchievements.Honored1kWin);
      int num1 = achievementDataModel1 != null ? achievementDataModel1.Progress : 0;
      int num2 = achievementDataModel1 != null ? achievementDataModel1.Quota : 0;
      if (achievementDataModel1 != null && AchievementManager.Get().IsAchievementComplete(achievementDataModel1.ID))
      {
        num1 = achievementDataModel2 != null ? achievementDataModel2.Progress : num1;
        num2 = achievementDataModel2 != null ? achievementDataModel2.Quota : num2;
        component.TooltipString = GameStrings.Format("GLUE_TOOLTIP_ALTERNATE_WINS_DESC", (object) num2);
      }
      if (num1 == 0)
      {
        this.m_rankedWinsPlate.SetActive(false);
        return false;
      }
      if (num1 >= num2)
      {
        this.m_rankedWins.Text = GameStrings.Format((bool) UniversalInputManager.UsePhoneUI ? "GLOBAL_HERO_WINS_PAST_MAX_PHONE" : "GLOBAL_HERO_WINS_PAST_MAX", (object) num1);
        component.TooltipEnabled = false;
      }
      else
      {
        this.m_rankedWins.Text = GameStrings.Format((bool) UniversalInputManager.UsePhoneUI ? "GLOBAL_HERO_WINS_PHONE" : "GLOBAL_HERO_WINS", (object) num1, (object) num2);
        component.TooltipEnabled = true;
      }
      this.m_rankedWinsPlate.SetActive(true);
      return true;
    }
    this.m_rankedWinsPlate.SetActive(false);
    return false;
  }

  public override void OnServerGameStarted()
  {
    base.OnServerGameStarted();
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE)
      return;
    AdventureConfig adventureConfig = AdventureConfig.Get();
    if (adventureConfig.CurrentSubScene != AdventureData.Adventuresubscene.MISSION_DECK_PICKER || DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2015)
      return;
    adventureConfig.SubSceneGoBack(false);
  }

  public override void HandleGameStartupFailure()
  {
    base.HandleGameStartupFailure();
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.TOURNAMENT:
        if (PresenceMgr.Get().CurrentStatus != Global.PresenceStatus.PLAY_QUEUE)
          break;
        PresenceMgr.Get().SetPrevStatus();
        break;
      case SceneMgr.Mode.ADVENTURE:
        if (AdventureConfig.Get().CurrentSubScene != AdventureData.Adventuresubscene.PRACTICE)
          break;
        PracticePickerTrayDisplay.Get().OnGameDenied();
        break;
    }
  }

  public void SetHeroDetailsTrayToIgnoreFullScreenEffects(bool ignoreEffects)
  {
    if ((UnityEngine.Object) this.m_hierarchyDetails == (UnityEngine.Object) null)
      return;
    if (ignoreEffects)
      LayerUtils.ReplaceLayer(this.m_hierarchyDetails, GameLayer.IgnoreFullScreenEffects, this.m_defaultDetailsLayer);
    else
      LayerUtils.ReplaceLayer(this.m_hierarchyDetails, this.m_defaultDetailsLayer, GameLayer.IgnoreFullScreenEffects);
  }

  public void ShowClickedStandardDeckInClassicPopup()
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TOURNAMENT && SceneMgr.Get().GetMode() != SceneMgr.Mode.FRIENDLY && SceneMgr.Get().GetMode() != SceneMgr.Mode.FIRESIDE_GATHERING || !((UnityEngine.Object) this.m_switchFormatPopup == (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_innkeeperQuote == (UnityEngine.Object) null))
      return;
    if (!this.m_switchFormatButton.IsCovered())
    {
      Action<int> action = (Action<int>) (groupId => this.m_switchFormatPopup = (Notification) null);
      this.m_switchFormatPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_INTRO, this.m_Switch_Format_Notification_Bone.position, this.m_Switch_Format_Notification_Bone.localScale, GameStrings.Get("GLUE_TOURNAMENT_SWITCH_TO_STANDARD"));
      if ((UnityEngine.Object) this.m_switchFormatPopup != (UnityEngine.Object) null)
      {
        this.m_switchFormatPopup.ShowPopUpArrow((bool) UniversalInputManager.UsePhoneUI ? Notification.PopUpArrowDirection.RightUp : Notification.PopUpArrowDirection.Up);
        this.m_switchFormatPopup.OnFinishDeathState += action;
      }
    }
    Action<int> finishCallback = (Action<int>) (groupId =>
    {
      if ((UnityEngine.Object) this.m_switchFormatButton != (UnityEngine.Object) null)
        NotificationManager.Get().DestroyNotification(this.m_switchFormatPopup, 0.0f);
      this.m_innkeeperQuote = (Notification) null;
    });
    this.m_innkeeperQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_INTRO, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_STANDARD_DECK_WARNING"), "", finishCallback: finishCallback);
  }

  public void ShowClickedWildDeckInClassicPopup() => this.ShowClickedWildDeckInStandardPopup();

  public void ShowClickedWildDeckInStandardPopup()
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TOURNAMENT && SceneMgr.Get().GetMode() != SceneMgr.Mode.FRIENDLY && SceneMgr.Get().GetMode() != SceneMgr.Mode.FIRESIDE_GATHERING || !((UnityEngine.Object) this.m_switchFormatPopup == (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_innkeeperQuote == (UnityEngine.Object) null))
      return;
    if (!this.m_switchFormatButton.IsCovered())
    {
      this.StopCoroutine("ShowSwitchToWildTutorialAfterTransitionsComplete");
      Action<int> action = (Action<int>) (groupId => this.m_switchFormatPopup = (Notification) null);
      this.m_switchFormatPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_INTRO, this.m_Switch_Format_Notification_Bone.position, this.m_Switch_Format_Notification_Bone.localScale, GameStrings.Get("GLUE_TOURNAMENT_SWITCH_TO_WILD"));
      if ((UnityEngine.Object) this.m_switchFormatPopup != (UnityEngine.Object) null)
      {
        this.m_switchFormatPopup.ShowPopUpArrow((bool) UniversalInputManager.UsePhoneUI ? Notification.PopUpArrowDirection.RightUp : Notification.PopUpArrowDirection.Up);
        this.m_switchFormatPopup.OnFinishDeathState += action;
      }
    }
    Action<int> finishCallback = (Action<int>) (groupId =>
    {
      if ((UnityEngine.Object) this.m_switchFormatButton != (UnityEngine.Object) null)
        NotificationManager.Get().DestroyNotification(this.m_switchFormatPopup, 0.0f);
      this.m_innkeeperQuote = (Notification) null;
    });
    this.m_innkeeperQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_INTRO, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_WILD_DECK_WARNING"), "VO_INNKEEPER_Male_Dwarf_SetRotation_32.prefab:3377790e79f276a4484ed43edde342c4", finishCallback: finishCallback);
  }

  public void ShowClickedClassicDeckInNonClassicPopup()
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TOURNAMENT && SceneMgr.Get().GetMode() != SceneMgr.Mode.FRIENDLY && SceneMgr.Get().GetMode() != SceneMgr.Mode.FIRESIDE_GATHERING || !((UnityEngine.Object) this.m_switchFormatPopup == (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_innkeeperQuote == (UnityEngine.Object) null))
      return;
    Action<int> finishCallback = (Action<int>) (groupId =>
    {
      if ((UnityEngine.Object) this.m_switchFormatButton != (UnityEngine.Object) null)
        NotificationManager.Get().DestroyNotification(this.m_switchFormatPopup, 0.0f);
      this.m_innkeeperQuote = (Notification) null;
    });
    this.m_innkeeperQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_INTRO, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_CLASSIC_DECK_WARNING"), "VO_Innkeeper_Male_Dwarf_ClassicMode_02.prefab:8cf46784be9929d4d84c40dc428df680", finishCallback: finishCallback);
  }

  public void ShowSwitchToWildTutorialIfNecessary()
  {
    if ((UnityEngine.Object) this.m_switchFormatPopup != (UnityEngine.Object) null || !UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_INTRO, "DeckPickerTrayDisplay.ShowSwitchToWildTutorialIfNecessary"))
      return;
    if (Options.GetFormatType() == PegasusShared.FormatType.FT_WILD)
    {
      Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK, false);
      Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN, false);
    }
    bool flag = false;
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    if (Options.Get().GetBool(Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK) && mode == SceneMgr.Mode.COLLECTIONMANAGER)
    {
      flag = true;
      Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK, false);
    }
    if (Options.Get().GetBool(Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN) && mode == SceneMgr.Mode.TOURNAMENT)
    {
      flag = true;
      Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN, false);
    }
    if (!flag)
      return;
    this.StartCoroutine("ShowSwitchToWildTutorialAfterTransitionsComplete");
  }

  private IEnumerator ShowSwitchToWildTutorialAfterTransitionsComplete()
  {
    yield return (object) new WaitForSeconds(1f);
    this.m_switchFormatPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_INTRO, this.m_Switch_Format_Notification_Bone, GameStrings.Get("GLUE_TOURNAMENT_SWITCH_TO_WILD"));
    this.m_switchFormatPopup.ShowPopUpArrow((bool) UniversalInputManager.UsePhoneUI ? Notification.PopUpArrowDirection.RightUp : Notification.PopUpArrowDirection.Up);
    this.m_switchFormatPopup.PulseReminderEveryXSeconds(3f);
    NotificationManager.Get().DestroyNotification(this.m_switchFormatPopup, 6f);
  }

  public void SkipHeroSelectionAndCloseTray()
  {
    if ((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null)
    {
      this.m_backButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(((AbsDeckPickerTrayDisplay) this).OnBackButtonReleased));
      this.m_playButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(((AbsDeckPickerTrayDisplay) this).OnPlayGameButtonReleased));
    }
    this.SetPlayButtonEnabled(false);
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(DeckPickerTrayDisplay.OnNavigateBack));
    if ((UnityEngine.Object) this.m_slidingTray != (UnityEngine.Object) null)
      this.m_slidingTray.ToggleTraySlider(false);
    if ((UnityEngine.Object) HeroPickerDisplay.Get() != (UnityEngine.Object) null)
      HeroPickerDisplay.Get().HideTray((bool) UniversalInputManager.UsePhoneUI ? 0.25f : 0.0f);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && !collectibleDisplay.GetHeroPickerDisplay().IsShown())
      CollectionManager.Get().GetCollectibleDisplay().EnableInput(false);
    CollectionDeckTray.Get().RegisterModeSwitchedListener(new DeckTray.ModeSwitched(this.OnModeSwitchedAfterSkippingHeroSelection));
  }

  public void ShowBonusStarsPopup()
  {
    this.OnPopupShown();
    DialogManager.Get().ShowBonusStarsPopup(this.GetBonusStarsPopupDataModel(), new Action(this.PlayEnterModeDialogues));
  }

  private bool ShouldShowBonusStarsPopUp()
  {
    if ((SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN ? 1 : (SceneMgr.Get().GetMode() != SceneMgr.Mode.TOURNAMENT ? 0 : (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY ? 1 : 0))) == 0 || this.m_currentMedalInfo.starsPerWin < 2)
      return false;
    int seasonId = this.m_currentMedalInfo.seasonId;
    int introSeenRequirement = this.m_currentMedalInfo.LeagueConfig.RankedIntroSeenRequirement;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_LAST_SEASON_BONUS_STARS_POPUP_SEEN, out this.m_lastSeasonBonusStarPopUpSeen);
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_BONUS_STARS_POPUP_SEEN_COUNT, out this.m_bonusStarsPopUpSeenCount);
    return this.m_lastSeasonBonusStarPopUpSeen < (long) seasonId && this.m_bonusStarsPopUpSeenCount < (long) introSeenRequirement;
  }

  private void OnModeSwitchedAfterSkippingHeroSelection()
  {
    CollectionDeckTray.Get().UnregisterModeSwitchedListener(new DeckTray.ModeSwitched(this.OnModeSwitchedAfterSkippingHeroSelection));
    CollectionManager.Get().GetCollectibleDisplay().EnableInput(true);
  }

  protected override IEnumerator InitDeckDependentElements()
  {
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    Log.PlayModeInvestigation.PrintInfo("DeckPickerTrayDisplay.InitDeckDependentElements() called");
    bool flag = pickerTrayDisplay.IsChoosingHero();
    DeckPickerMode defaultDeckPickerMode = DeckPickerMode.CUSTOM;
    pickerTrayDisplay.m_deckPickerMode = defaultDeckPickerMode;
    pickerTrayDisplay.m_numPagesToShow = 1;
    pickerTrayDisplay.m_basicDeckPageContainer.gameObject.SetActive(flag);
    if (!flag)
    {
      while (!NetCache.Get().IsNetObjectAvailable<NetCache.NetCacheDecks>())
        yield return (object) null;
      CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded();
      while (!CollectionManager.Get().AreAllDeckContentsReady())
        yield return (object) null;
      pickerTrayDisplay.m_usingSharedDecks = FriendChallengeMgr.Get().ShouldUseSharedDecks();
      pickerTrayDisplay.m_deckPickerMode = pickerTrayDisplay.m_usingSharedDecks ? DeckPickerMode.CUSTOM : defaultDeckPickerMode;
      pickerTrayDisplay.UpdateDeckShareRequestButton();
      List<CollectionDeck> decks = CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK);
      if (FriendChallengeMgr.Get().IsChallengeFriendlyDuel)
        decks = !pickerTrayDisplay.m_usingSharedDecks ? decks.FindAll((Predicate<CollectionDeck>) (deck => deck.IsValidForFormat(FriendChallengeMgr.Get().GetFormatType()))) : FriendChallengeMgr.Get().GetSharedDecks();
      pickerTrayDisplay.SetupDeckPages(decks);
    }
    if ((UnityEngine.Object) pickerTrayDisplay.m_rankedPlayDisplay != (UnityEngine.Object) null)
    {
      VisualsFormatType visualsFormatType = VisualsFormatTypeExtensions.GetCurrentVisualsFormatType();
      pickerTrayDisplay.UpdateRankedPlayDisplay(visualsFormatType);
    }
    pickerTrayDisplay.InitSwitchFormatButton();
    // ISSUE: reference to a compiler-generated method
    yield return (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.\u003C\u003En__0());
  }

  private void SetupDeckPages(List<CollectionDeck> decks)
  {
    this.m_numPagesToShow = Mathf.CeilToInt((float) decks.Count / 9f);
    this.m_numPagesToShow = Mathf.Max(this.m_numPagesToShow, 1);
    Log.PlayModeInvestigation.PrintInfo(string.Format("DeckPickerTrayDisplay.SetupDeckPages() called. m_numPagesToShow={0}, decks.Count={1}", (object) this.m_numPagesToShow, (object) decks.Count));
    this.InitDeckPages();
    LoanerDeckDisplay loanerDeckDisplay = LoanerDeckDisplay.Get();
    FreeDeckMgr freeDeckMgr = FreeDeckMgr.Get();
    if (freeDeckMgr != null && (UnityEngine.Object) loanerDeckDisplay != (UnityEngine.Object) null && freeDeckMgr.Status == FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD && loanerDeckDisplay.ShouldLoanerDecksBeDisplayed())
    {
      int num = Mathf.Max(Mathf.CeilToInt((float) freeDeckMgr.GetLoanerDecksCount() / (float) loanerDeckDisplay.MaximumLoanerDecksToDisplay), 1);
      this.m_numPagesToShow += num;
      for (int index = 0; index < num; ++index)
        this.CreateCustomDeckPage(true);
    }
    Log.PlayModeInvestigation.PrintInfo("DeckPickerTrayDisplay.InitDeckPages() -- added page for Loaner decks");
    this.SetPageDecks(decks);
    this.UpdateDeckVisuals();
  }

  private void UpdateDeckVisuals()
  {
    for (int index = 0; index < this.m_customPages.Count; ++index)
      this.m_customPages[index].UpdateDeckVisuals();
  }

  protected override void InitForMode(SceneMgr.Mode mode)
  {
    this.m_missingClassicDeck.SetActive(false);
    switch (mode)
    {
      case SceneMgr.Mode.TOURNAMENT:
        this.m_rankedPlayDisplayWidget = (Widget) WidgetInstance.Create((bool) UniversalInputManager.UsePhoneUI ? "RankedPlayDisplay_phone.prefab:22b0793a4bc044e47a1948619c2aa896" : "RankedPlayDisplay.prefab:1f884a817dbbdd84b9f8713dc21759f1");
        this.m_rankedPlayDisplayWidget.RegisterReadyListener((Action<object>) (_ => this.OnRankedPlayDisplayWidgetReady()), (object) null, true);
        this.SetPlayButtonText(GameStrings.Get("GLOBAL_PLAY"));
        this.ChangePlayButtonTextAlpha();
        this.UpdateRankedClassWinsPlate();
        this.UpdatePageArrows();
        if (Options.GetFormatType() == PegasusShared.FormatType.FT_CLASSIC && CollectionManager.Get().GetNumberOfClassicDecks() == 0)
        {
          this.m_missingClassicDeck.SetActive(true);
          break;
        }
        break;
      case SceneMgr.Mode.TAVERN_BRAWL:
        this.SetHeaderForTavernBrawl();
        break;
    }
    UnityEngine.Vector2 offset = new UnityEngine.Vector2(0.0f, 0.0f);
    this.m_currentModeTextures = this.m_collectionTextures;
    switch (mode - 5)
    {
      case SceneMgr.Mode.INVALID:
        this.m_currentModeTextures = this.m_collectionTextures;
        break;
      case SceneMgr.Mode.LOGIN:
        this.m_currentModeTextures = this.m_tournamentTextures;
        break;
      case SceneMgr.Mode.HUB:
      case SceneMgr.Mode.DRAFT:
        if (mode == SceneMgr.Mode.FIRESIDE_GATHERING && FiresideGatheringManager.Get().InBrawlMode() || FriendChallengeMgr.Get().IsChallengeTavernBrawl())
        {
          this.m_currentModeTextures = this.m_tavernBrawlTextures;
          offset.x = 0.5f;
          offset.y = 0.61f;
          break;
        }
        this.m_currentModeTextures = this.m_friendlyTextures;
        offset.y = 0.61f;
        break;
      case SceneMgr.Mode.FRIENDLY:
        this.m_currentModeTextures = this.m_adventureTextures;
        offset.x = 0.5f;
        break;
      case SceneMgr.Mode.FATAL_ERROR:
        this.m_currentModeTextures = this.m_tavernBrawlTextures;
        offset.x = 0.5f;
        offset.y = 0.61f;
        break;
    }
    VisualsFormatType visualsFormatType = VisualsFormatTypeExtensions.GetCurrentVisualsFormatType();
    Texture textureForFormat1 = this.m_currentModeTextures.GetTextureForFormat(visualsFormatType);
    Texture textureForFormat2 = this.m_currentModeTextures.GetCustomTextureForFormat(visualsFormatType);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (SceneMgr.Mode.TOURNAMENT != mode)
        this.m_detailsTrayFrame.GetComponent<MeshFilter>().mesh = this.m_alternateDetailsTrayMesh;
      this.SetPhoneDetailsTrayTextures(textureForFormat1, textureForFormat1);
    }
    else
      this.SetTrayFrameAndBasicDeckPageTextures(textureForFormat1, textureForFormat1);
    this.SetCustomDeckPageTextures(textureForFormat2, textureForFormat2);
    this.SetKeyholeTextureOffsets(offset);
    this.UpdateDeckVisuals();
    base.InitForMode(mode);
  }

  private PegasusShared.GameType GetGameTypeForNewPlayModeGame() => !Options.GetInRankedPlayMode() ? PegasusShared.GameType.GT_CASUAL : PegasusShared.GameType.GT_RANKED;

  private PegasusShared.FormatType GetFormatTypeForNewPlayModeGame()
  {
    if (this.GetGameTypeForNewPlayModeGame() != PegasusShared.GameType.GT_CASUAL)
      return Options.GetFormatType();
    CollectionDeck selectedCollectionDeck = this.GetSelectedCollectionDeck();
    return selectedCollectionDeck == null ? PegasusShared.FormatType.FT_STANDARD : selectedCollectionDeck.FormatType;
  }

  private void UpdateFormat_Tournament(VisualsFormatType newVisualsFormatType)
  {
    int formatType = (int) Options.GetFormatType();
    int num = CollectionManager.Get().ShouldAccountSeeStandardWild() ? 1 : 0;
    this.SetPlayButtonText(GameStrings.Get("GLOBAL_PLAY"));
    if (num != 0)
    {
      if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && SetRotationManager.HasSeenStandardModeTutorial())
      {
        if (newVisualsFormatType == VisualsFormatType.VFT_WILD && !Options.Get().GetBool(Option.HAS_SEEN_WILD_MODE_VO) && UserAttentionManager.CanShowAttentionGrabber("DeckPickerTrayDisplay.UpdateFormat_Tournament:" + (object) Option.HAS_SEEN_WILD_MODE_VO))
        {
          this.HideSetRotationNotifications();
          NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_WILD_GAME"), "VO_INNKEEPER_Male_Dwarf_SetRotation_35.prefab:db2f6e3818fa49b4d8423121eba762f6");
          Options.Get().SetBool(Option.HAS_SEEN_WILD_MODE_VO, true);
        }
        if (newVisualsFormatType == VisualsFormatType.VFT_CLASSIC && !Options.Get().GetBool(Option.HAS_SEEN_CLASSIC_MODE_VO) && UserAttentionManager.CanShowAttentionGrabber("DeckPickerTrayDisplay.UpdateFormat_Tournament:" + (object) Option.HAS_SEEN_CLASSIC_MODE_VO))
        {
          this.HideSetRotationNotifications();
          NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_CLASSIC_TAKES_YOU_BACK_ORIGINAL_HEARTHSTONE"), "VO_Innkeeper_Male_Dwarf_ClassicMode_06.prefab:f91da6f7e66fd754fb4e568d15d49116");
          Options.Get().SetBool(Option.HAS_SEEN_CLASSIC_MODE_VO, true);
        }
      }
      if ((UnityEngine.Object) this.m_selectedCustomDeckBox != (UnityEngine.Object) null && !this.m_selectedCustomDeckBox.CanSelectDeck())
        this.Deselect();
      this.UpdateCustomTournamentBackgroundAndDecks();
    }
    this.ChangePlayButtonTextAlpha();
    this.UpdateRankedClassWinsPlate();
    this.UpdateRankedPlayDisplay(newVisualsFormatType);
  }

  private void ChangePlayButtonTextAlpha()
  {
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null))
      return;
    if (this.m_playButton.IsEnabled())
      this.m_playButton.m_newPlayButtonText.TextAlpha = 1f;
    else
      this.m_playButton.m_newPlayButtonText.TextAlpha = 0.0f;
  }

  private void UpdateRankedPlayDisplay(VisualsFormatType newVisualsFormatType)
  {
    if (!newVisualsFormatType.IsRanked())
    {
      this.m_casualPlayDisplayWidget.Show();
      this.m_rankedPlayDisplay.Hide();
    }
    else
    {
      this.m_casualPlayDisplayWidget.Hide();
      this.m_rankedPlayDisplay.Show();
      this.m_rankedPlayDisplay.UpdateMode(newVisualsFormatType);
      RankedRewardInfoButton componentInChildren = this.m_rankedPlayDisplay.GetComponentInChildren<RankedRewardInfoButton>();
      if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
        return;
      TournamentDisplay tournamentDisplay = TournamentDisplay.Get();
      if ((UnityEngine.Object) tournamentDisplay == (UnityEngine.Object) null)
        return;
      NetCache.NetCacheMedalInfo currentMedalInfo = tournamentDisplay.GetCurrentMedalInfo();
      if (currentMedalInfo == null)
        return;
      MedalInfoTranslator mit = new MedalInfoTranslator(currentMedalInfo);
      componentInChildren.Initialize(mit);
    }
  }

  private void UpdateFormat_CollectionManager()
  {
    PegasusShared.FormatType formatType = Options.GetFormatType();
    bool inRankedPlayMode = Options.GetInRankedPlayMode();
    if (formatType == PegasusShared.FormatType.FT_WILD && !this.m_HasSeenPlayStandardToWildVO)
    {
      this.m_HasSeenPlayStandardToWildVO = true;
      this.m_HasSeenPlayStandardToClassicVO = false;
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get("VO_INNKEEPER_PLAY_STANDARD_TO_WILD"), "VO_INNKEEPER_Male_Dwarf_SetRotation_43.prefab:4b4ce858139927946905ec0d40d5b3c1");
    }
    else if (formatType == PegasusShared.FormatType.FT_CLASSIC && !this.m_HasSeenPlayStandardToClassicVO)
    {
      this.m_HasSeenPlayStandardToClassicVO = true;
      this.m_HasSeenPlayStandardToWildVO = false;
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get("VO_INNKEEPER_CLASSIC_PLAY_CLASSIC_MODE_ONLY"), "VO_Innkeeper_Male_Dwarf_ClassicMode_01.prefab:5ac6a7a19130d8c4795330b7a8693513");
    }
    else if (formatType == PegasusShared.FormatType.FT_STANDARD)
    {
      this.m_HasSeenPlayStandardToClassicVO = false;
      this.m_HasSeenPlayStandardToWildVO = false;
    }
    this.StartCoroutine(this.InitModeWhenReady());
    this.TransitionToFormatType(formatType, inRankedPlayMode, 2f);
  }

  private void UpdateCustomTournamentBackgroundAndDecks()
  {
    this.TransitionToFormatType(Options.GetFormatType(), Options.GetInRankedPlayMode(), 2f);
    this.UpdateDeckVisuals();
  }

  private void InitButtonAchievements()
  {
    this.UpdateCollectionButtonGlow();
    foreach (TAG_CLASS validClass1 in this.m_validClasses)
    {
      TAG_CLASS validClass = validClass1;
      if (GameUtils.HasUnlockedClass(validClass))
      {
        HeroPickerButton heroPickerButton = this.m_heroButtons.Find((Predicate<HeroPickerButton>) (obj => obj.GetEntityDef().GetClass() == validClass));
        heroPickerButton.Unlock();
        if (this.IsChoosingHero())
        {
          CollectionManager.PreconDeck preconDeck = CollectionManager.Get().GetPreconDeck(validClass);
          long preconDeckID = 0;
          if (preconDeck != null)
            preconDeckID = preconDeck.ID;
          heroPickerButton.SetPreconDeckID(preconDeckID);
          if (preconDeckID == 0L)
            Debug.LogError((object) string.Format("DeckPickerTrayDisplay.InitButtonAchievements() - preconDeckID = 0 for class {0}", (object) validClass));
          SceneMgr.Mode mode = SceneMgr.Get().GetMode();
          if ((mode == SceneMgr.Mode.TAVERN_BRAWL || mode == SceneMgr.Mode.FRIENDLY && FriendChallengeMgr.Get().IsChallengeTavernBrawl() ? 1 : (mode != SceneMgr.Mode.FIRESIDE_GATHERING ? 0 : (FiresideGatheringManager.Get().InBrawlMode() ? 1 : 0))) != 0)
            heroPickerButton.Unlock();
        }
      }
    }
  }

  protected override void SetHeaderForTavernBrawl()
  {
    if ((UnityEngine.Object) this.m_labelDecoration != (UnityEngine.Object) null)
      this.m_labelDecoration.SetActive(false);
    base.SetHeaderForTavernBrawl();
  }

  protected override void InitHeroPickerButtons()
  {
    base.InitHeroPickerButtons();
    CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK);
    this.m_heroDefsLoading = this.m_validClasses.Count;
    for (int index = 0; index < this.m_validClasses.Count; ++index)
    {
      if (index >= this.m_heroButtons.Count || (UnityEngine.Object) this.m_heroButtons[index] == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "InitHeroPickerButtons: not enough buttons for total guest heroes.");
        break;
      }
      HeroPickerButton heroButton = this.m_heroButtons[index];
      heroButton.Lock();
      TAG_CLASS validClass = this.m_validClasses[index];
      NetCache.CardDefinition randomFavoriteHero = CollectionManager.Get().GetRandomFavoriteHero(validClass);
      if (randomFavoriteHero == null)
      {
        if (validClass != TAG_CLASS.WHIZBANG)
          Debug.LogWarning((object) ("Couldn't find Favorite Hero for hero class: " + (object) validClass + " defaulting to Vanilla Hero!"));
        string vanillaHero = CollectionManager.GetVanillaHero(validClass);
        TAG_PREMIUM heroPremium = CollectionManager.Get().GetHeroPremium(validClass);
        AbsDeckPickerTrayDisplay.HeroFullDefLoadedCallbackData userData = new AbsDeckPickerTrayDisplay.HeroFullDefLoadedCallbackData(heroButton, heroPremium);
        DefLoader.Get().LoadFullDef(vanillaHero, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(((AbsDeckPickerTrayDisplay) this).OnHeroFullDefLoaded), (object) userData);
      }
      else
      {
        AbsDeckPickerTrayDisplay.HeroFullDefLoadedCallbackData userData = new AbsDeckPickerTrayDisplay.HeroFullDefLoadedCallbackData(heroButton, randomFavoriteHero.Premium);
        DefLoader.Get().LoadFullDef(randomFavoriteHero.Name, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(((AbsDeckPickerTrayDisplay) this).OnHeroFullDefLoaded), (object) userData);
      }
      if (this.IsChoosingHero())
        heroButton.SetDivotTexture(this.m_currentModeTextures.classDivotTex);
      else
        heroButton.SetDivotTexture(this.m_currentModeTextures.guestHeroDivotTex);
    }
    if (!this.IsChoosingHeroForDungeonCrawlAdventure())
      return;
    this.SetUpHeroCrowns();
  }

  private void InitDeckPages()
  {
    Log.PlayModeInvestigation.PrintInfo("DeckPickerTrayDisplay.InitDeckPages() called." + string.Format("m_numPagesToShow={0}, m_customPages.Count={1}", (object) this.m_numPagesToShow, (object) this.m_customPages.Count));
    if (this.m_numPagesToShow <= 0)
    {
      Debug.LogWarning((object) "DeckPickerTrayDisplay.InitDeckPages() called with invalid amount of pages");
    }
    else
    {
      while (this.m_numPagesToShow > this.m_customPages.Count)
        this.CreateCustomDeckPage(false);
      while (this.m_numPagesToShow < this.m_customPages.Count)
      {
        CustomDeckPage customPage = this.m_customPages[this.m_customPages.Count - 1];
        this.m_customPages.Remove(customPage);
        UnityEngine.Object.Destroy((UnityEngine.Object) customPage.gameObject);
        Log.PlayModeInvestigation.PrintInfo("DeckPickerTrayDisplay.InitDeckPages() -- Deck page removed." + string.Format("New total: {0}", (object) this.m_customPages.Count));
      }
    }
  }

  private void CreateCustomDeckPage(bool isPageForLoanerDecks)
  {
    GameObject gameObject1 = AssetLoader.Get().InstantiatePrefab(DeckPickerTrayDisplay.CUSTOM_DECK_PAGE);
    gameObject1.transform.SetParent(this.m_customDeckPagesRoot.transform, false);
    gameObject1.transform.localPosition = this.m_customPages.Count == 0 ? this.m_customDeckPageUpperBone.transform.localPosition : this.m_customDeckPageLowerBone.transform.localPosition;
    CustomDeckPage component = gameObject1.GetComponent<CustomDeckPage>();
    component.m_isPageForLoanerDecks = isPageForLoanerDecks;
    component.SetDeckButtonCallback(new CustomDeckPage.DeckButtonCallback(this.OnCustomDeckPressed));
    if (isPageForLoanerDecks)
    {
      this.m_customPages.Insert(0, component);
      GameObject gameObject2 = AssetLoader.Get().InstantiatePrefab(DeckPickerTrayDisplay.LOANER_DECK_TIMER);
      gameObject2.transform.parent = gameObject1.transform;
      gameObject2.transform.position = Vector3.zero;
    }
    else
      this.m_customPages.Add(component);
    Log.PlayModeInvestigation.PrintInfo("DeckPickerTrayDisplay.InitDeckPages() -- Deck page added." + string.Format(" New total: {0}", (object) this.m_customPages.Count));
  }

  private void SetPageDecks(List<CollectionDeck> decks)
  {
    if (this.m_customPages == null)
      Debug.LogError((object) "{0}.UpdateCustomPages(): m_customPages is null. Make sure you call InitCustomPages() first!", (UnityEngine.Object) this);
    int loanerDecksDisplayed = 0;
    List<CollectionDeck> loanerDecks = new List<CollectionDeck>();
    FreeDeckMgr freeDeckMgr = FreeDeckMgr.Get();
    if (freeDeckMgr.Status == FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD && freeDeckMgr.GetLoanerDecksCount() > 0)
    {
      foreach (KeyValuePair<int, CollectionDeck> loanerDecksAs in freeDeckMgr.GetLoanerDecksAsMap())
        loanerDecks.Add(loanerDecksAs.Value);
    }
    foreach (CustomDeckPage customPage in this.m_customPages)
    {
      if (customPage.m_isPageForLoanerDecks)
      {
        this.SetLoanerDeckPageDecks(customPage, loanerDecks, ref loanerDecksDisplayed);
      }
      else
      {
        int count = Mathf.Min(decks.Count, customPage.m_maxCustomDecksToDisplay);
        List<CollectionDeck> range = decks.GetRange(0, count);
        customPage.InitDecks(range);
        foreach (CollectionDeck collectionDeck in range)
        {
          string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(collectionDeck.HeroCardID);
          if (string.IsNullOrEmpty(powerCardIdFromHero))
            Debug.LogErrorFormat("No hero power set up for hero {0}", (object) collectionDeck.HeroCardID);
          else
            this.LoadHeroPowerDef(powerCardIdFromHero, CollectionManager.Get().GetHeroPremium(collectionDeck.GetClass()));
        }
        decks.RemoveRange(0, count);
        if (decks.Count <= 0)
          break;
      }
    }
    if (decks.Count <= 0)
      return;
    Debug.LogWarningFormat("DeckPickerTrayDisplay - {0} more decks than we can display!", (object) decks.Count);
  }

  private void SetLoanerDeckPageDecks(
    CustomDeckPage page,
    List<CollectionDeck> loanerDecks,
    ref int loanerDecksDisplayed)
  {
    if (loanerDecksDisplayed >= loanerDecks.Count)
      return;
    int count = Mathf.Min(loanerDecks.Count - loanerDecksDisplayed, LoanerDeckDisplay.Get().MaximumLoanerDecksToDisplay);
    List<CollectionDeck> range = loanerDecks.GetRange(loanerDecksDisplayed, count);
    foreach (CollectionDeck collectionDeck in range)
    {
      string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(collectionDeck.HeroCardID);
      if (string.IsNullOrEmpty(powerCardIdFromHero))
        Debug.LogErrorFormat("No hero power set up for hero {0}", (object) collectionDeck.HeroCardID);
      else
        this.LoadHeroPowerDef(powerCardIdFromHero, CollectionManager.Get().GetHeroPremium(collectionDeck.GetClass()));
    }
    page.InitDecks(range, true);
    loanerDecksDisplayed = count;
  }

  private void InitMode()
  {
    if (this.IsChoosingHero())
      this.ShowHeroPickerPage(true);
    else
      this.SetSelectionAndPageFromOptions();
    this.InitExpoDemoMode();
    this.ShowSwitchToWildTutorialIfNecessary();
  }

  private void InitExpoDemoMode()
  {
    if (!DemoMgr.Get().IsExpoDemo())
      return;
    this.UpdatePageArrows();
    this.SetBackButtonEnabled(false);
    this.StartCoroutine("ShowDemoQuotes");
  }

  private IEnumerator ShowDemoQuotes()
  {
    string str = Vars.Key("Demo.ThankQuote").GetStr("");
    int num = Vars.Key("Demo.ThankQuoteMsTime").GetInt(0);
    string text = str.Replace("\\n", "\n");
    if ((string.IsNullOrEmpty(text) ? 0 : (num > 0 ? 1 : 0)) != 0)
    {
      this.m_expoThankQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(158.1f, NotificationManager.DEPTH, 80.2f), text, "", (float) num / 1000f);
      this.EnableClickBlocker(true);
      yield return (object) new WaitForSeconds((float) num / 1000f);
      this.EnableClickBlocker(false);
    }
    this.ShowIntroQuote();
  }

  private void ShowIntroQuote()
  {
    this.HideIntroQuote();
    string text = Vars.Key("Demo.IntroQuote").GetStr("").Replace("\\n", "\n");
    if (string.IsNullOrEmpty(text))
      return;
    this.m_expoIntroQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(147.6f, NotificationManager.DEPTH, 23.1f), text, "");
  }

  private void EnableClickBlocker(bool enable)
  {
    if ((UnityEngine.Object) this.m_clickBlocker == (UnityEngine.Object) null)
      return;
    if (enable)
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective with
      {
        Blur = new BlurParameters(brightness: 1f)
      });
    else
      this.m_screenEffectsHandle.StopEffect();
    this.m_clickBlocker.gameObject.SetActive(enable);
  }

  private void HideDemoQuotes()
  {
    DemoMgr demoMgr = DemoMgr.Get();
    if (demoMgr != null && !demoMgr.IsExpoDemo())
      return;
    this.StopCoroutine("ShowDemoQuotes");
    if ((UnityEngine.Object) this.m_expoThankQuote != (UnityEngine.Object) null)
    {
      NotificationManager notificationManager = NotificationManager.Get();
      if ((UnityEngine.Object) notificationManager != (UnityEngine.Object) null)
        notificationManager.DestroyNotification(this.m_expoThankQuote, 0.0f);
      this.m_expoThankQuote = (Notification) null;
      this.m_screenEffectsHandle.StopEffect();
    }
    this.HideIntroQuote();
  }

  private void HideIntroQuote()
  {
    if (!((UnityEngine.Object) this.m_expoIntroQuote != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotification(this.m_expoIntroQuote, 0.0f);
    this.m_expoIntroQuote = (Notification) null;
  }

  private void HideSetRotationNotifications()
  {
    if ((UnityEngine.Object) this.m_innkeeperQuote != (UnityEngine.Object) null)
    {
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_innkeeperQuote);
      this.m_innkeeperQuote = (Notification) null;
    }
    if (!((UnityEngine.Object) this.m_switchFormatPopup != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_switchFormatPopup);
    this.m_switchFormatPopup = (Notification) null;
  }

  private void OnTransitionFromGameplayFinished(bool cutoff, object userData)
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY && !FriendChallengeMgr.Get().HasChallenge())
      this.GoBackUntilOnNavigateBackCalled();
    LoadingScreen.Get().UnregisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
  }

  private void CollectionButtonPress(UIEvent e)
  {
    if (this.ShouldGlowCollectionButton())
    {
      if (!Options.Get().GetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_DECK) && this.HaveDecksThatNeedNames())
        Options.Get().SetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_DECK, true);
      else if (!Options.Get().GetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_CARD) && this.HaveUnseenCards())
        Options.Get().SetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_CARD, true);
      if (Options.Get().GetBool(Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION) && SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT)
        Options.Get().SetBool(Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION, false);
    }
    if ((UnityEngine.Object) PracticePickerTrayDisplay.Get() != (UnityEngine.Object) null && PracticePickerTrayDisplay.Get().IsShown())
      Navigation.GoBack();
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FIRESIDE_GATHERING)
      Navigation.Clear();
    TelemetryWatcher.StopWatchingFor(TelemetryWatcherWatchType.CollectionManagerFromDeckPicker);
    TelemetryManager.Client().SendDeckPickerToCollection(DeckPickerToCollection.Path.DECK_PICKER_BUTTON);
    CollectionManager.Get().NotifyOfBoxTransitionStart();
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.COLLECTIONMANAGER);
  }

  private void RequestDeckShareButtonPress(UIEvent e)
  {
    if (this.m_doingDeckShareTransition)
      return;
    if (this.m_usingSharedDecks)
    {
      FriendChallengeMgr.Get().EndDeckShare();
    }
    else
    {
      if (!FriendChallengeMgr.Get().HasOpponentSharedDecks())
        this.EnableRequestDeckShareButton(false);
      FriendChallengeMgr.Get().RequestDeckShare();
    }
    this.UpdateDeckShareTooltip();
  }

  private void RequestDeckShareButtonOver(UIEvent e)
  {
    this.m_isDeckShareRequestButtonHovered = true;
    this.UpdateDeckShareTooltip();
  }

  private void RequestDeckShareButtonOut(UIEvent e)
  {
    this.m_isDeckShareRequestButtonHovered = false;
    this.UpdateDeckShareTooltip();
  }

  private void EnableRequestDeckShareButton(bool enable)
  {
    if (this.m_DeckShareRequestButton.IsEnabled() != enable)
    {
      if (!enable)
        this.m_DeckShareRequestButton.TriggerOut();
      this.m_DeckShareRequestButton.SetEnabled(enable);
      this.m_DeckShareRequestButton.Flip(enable);
    }
    this.UpdateDeckShareRequestButton();
  }

  private void UpdateDeckShareRequestButton()
  {
    if ((UnityEngine.Object) this.m_DeckShareRequestButton == (UnityEngine.Object) null || !this.IsDeckSharingActive())
      return;
    if (!FriendChallengeMgr.Get().HasOpponentSharedDecks())
      this.m_DeckShareRequestButton.SetText(GameStrings.Get("GLUE_DECK_SHARE_BUTTON_BORROW_DECKS"));
    else if (this.m_usingSharedDecks)
      this.m_DeckShareRequestButton.SetText(GameStrings.Get("GLUE_DECK_SHARE_BUTTON_SHOW_MY_DECKS"));
    else
      this.m_DeckShareRequestButton.SetText(GameStrings.Format("GLUE_DECK_SHARE_BUTTON_SHOW_OPPONENT_DECKS"));
    this.UpdateDeckShareTooltip();
  }

  private void UpdateDeckShareTooltip()
  {
    if ((UnityEngine.Object) this.m_DeckShareRequestButton == (UnityEngine.Object) null)
      return;
    TooltipZone componentInChildren = this.m_DeckShareRequestButton.GetComponentInChildren<TooltipZone>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    if (!FriendChallengeMgr.Get().HasOpponentSharedDecks())
    {
      if (this.m_isDeckShareRequestButtonHovered && !componentInChildren.IsShowingTooltip())
      {
        string str = string.Empty;
        BnetPlayer myOpponent = FriendChallengeMgr.Get().GetMyOpponent();
        if (myOpponent != null)
          str = myOpponent.GetBestName();
        componentInChildren.ShowTooltip(GameStrings.Get("GLUE_DECK_SHARE_TOOLTIP_HEADER"), GameStrings.Format("GLUE_DECK_SHARE_TOOLTIP_BODY_REQUEST", (object) str), 5f);
      }
      else
      {
        if (this.m_isDeckShareRequestButtonHovered || !componentInChildren.IsShowingTooltip())
          return;
        componentInChildren.HideTooltip();
      }
    }
    else
    {
      if (!componentInChildren.IsShowingTooltip())
        return;
      componentInChildren.HideTooltip();
    }
  }

  private void OnDeckShareRequestCancelDeclineOrError()
  {
    this.StopCoroutine("WaitThanEnableRequestDeckShareButton");
    this.StartCoroutine("WaitThanEnableRequestDeckShareButton");
  }

  private IEnumerator WaitThanEnableRequestDeckShareButton()
  {
    yield return (object) new WaitForSeconds(1f);
    this.EnableRequestDeckShareButton(true);
  }

  public void UseSharedDecks(List<CollectionDeck> decks) => this.StartCoroutine(this.UseSharedDecksImpl(decks));

  private IEnumerator UseSharedDecksImpl(List<CollectionDeck> decks)
  {
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    if (!pickerTrayDisplay.m_usingSharedDecks && decks != null)
    {
      pickerTrayDisplay.m_doingDeckShareTransition = true;
      pickerTrayDisplay.m_clickBlocker.gameObject.SetActive(true);
      pickerTrayDisplay.m_usingSharedDecks = true;
      pickerTrayDisplay.UpdateDeckShareRequestButton();
      pickerTrayDisplay.Deselect();
      pickerTrayDisplay.m_deckPickerMode = DeckPickerMode.CUSTOM;
      if (!string.IsNullOrEmpty(pickerTrayDisplay.m_wildDeckTransitionSound))
        SoundManager.Get().LoadAndPlay((AssetReference) pickerTrayDisplay.m_wildDeckTransitionSound);
      if ((UnityEngine.Object) pickerTrayDisplay.m_DeckShareGlowOutQuad != (UnityEngine.Object) null)
      {
        pickerTrayDisplay.m_DeckShareGlowOutQuad.SetActive(true);
        yield return (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.FadeDeckShareGlowOutQuad(0.0f, pickerTrayDisplay.m_DeckShareGlowOutIntensity, pickerTrayDisplay.m_DeckShareTransitionTime * 0.5f));
      }
      if ((UnityEngine.Object) pickerTrayDisplay.m_DeckShareParticles != (UnityEngine.Object) null)
      {
        pickerTrayDisplay.m_DeckShareParticles.Stop();
        pickerTrayDisplay.m_DeckShareParticles.Play();
      }
      pickerTrayDisplay.SetupDeckPages(decks);
      pickerTrayDisplay.m_basicDeckPageContainer.gameObject.SetActive(false);
      foreach (CollectionDeck deck in decks)
        deck.Locked = false;
      VisualsFormatType visualsFormatType = VisualsFormatTypeExtensions.GetCurrentVisualsFormatType();
      Texture textureForFormat = pickerTrayDisplay.m_currentModeTextures.GetCustomTextureForFormat(visualsFormatType);
      pickerTrayDisplay.SetCustomDeckPageTextures(textureForFormat, textureForFormat);
      pickerTrayDisplay.ShowPage(0, true);
      if ((UnityEngine.Object) pickerTrayDisplay.m_DeckShareGlowOutQuad != (UnityEngine.Object) null)
      {
        yield return (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.FadeDeckShareGlowOutQuad(pickerTrayDisplay.m_DeckShareGlowOutIntensity, 0.0f, pickerTrayDisplay.m_DeckShareTransitionTime * 0.5f));
        pickerTrayDisplay.m_DeckShareGlowOutQuad.SetActive(false);
      }
      pickerTrayDisplay.EnableRequestDeckShareButton(true);
      pickerTrayDisplay.m_clickBlocker.gameObject.SetActive(false);
      pickerTrayDisplay.m_doingDeckShareTransition = false;
    }
  }

  public void StopUsingSharedDecks() => this.StartCoroutine(this.StopUsingSharedDecksImpl());

  private IEnumerator StopUsingSharedDecksImpl()
  {
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    if (pickerTrayDisplay.m_usingSharedDecks)
    {
      pickerTrayDisplay.m_clickBlocker.gameObject.SetActive(true);
      pickerTrayDisplay.m_doingDeckShareTransition = true;
      pickerTrayDisplay.m_usingSharedDecks = false;
      pickerTrayDisplay.UpdateDeckShareRequestButton();
      pickerTrayDisplay.Deselect();
      if (!string.IsNullOrEmpty(pickerTrayDisplay.m_wildDeckTransitionSound))
        SoundManager.Get().LoadAndPlay((AssetReference) pickerTrayDisplay.m_wildDeckTransitionSound);
      if ((UnityEngine.Object) pickerTrayDisplay.m_DeckShareGlowOutQuad != (UnityEngine.Object) null)
      {
        pickerTrayDisplay.m_DeckShareGlowOutQuad.SetActive(true);
        yield return (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.FadeDeckShareGlowOutQuad(0.0f, pickerTrayDisplay.m_DeckShareGlowOutIntensity, pickerTrayDisplay.m_DeckShareTransitionTime * 0.5f));
      }
      if ((UnityEngine.Object) pickerTrayDisplay.m_DeckShareParticles != (UnityEngine.Object) null)
      {
        pickerTrayDisplay.m_DeckShareParticles.Stop();
        pickerTrayDisplay.m_DeckShareParticles.Play();
      }
      List<CollectionDeck> all = CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK).FindAll((Predicate<CollectionDeck>) (deck => deck.IsValidForFormat(FriendChallengeMgr.Get().GetFormatType())));
      pickerTrayDisplay.SetupDeckPages(all);
      pickerTrayDisplay.ShowPage(0, true);
      if ((UnityEngine.Object) pickerTrayDisplay.m_DeckShareGlowOutQuad != (UnityEngine.Object) null)
      {
        yield return (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.FadeDeckShareGlowOutQuad(pickerTrayDisplay.m_DeckShareGlowOutIntensity, 0.0f, pickerTrayDisplay.m_DeckShareTransitionTime * 0.5f));
        pickerTrayDisplay.m_DeckShareGlowOutQuad.SetActive(false);
      }
      pickerTrayDisplay.EnableRequestDeckShareButton(true);
      pickerTrayDisplay.m_doingDeckShareTransition = false;
      pickerTrayDisplay.m_clickBlocker.gameObject.SetActive(false);
    }
  }

  private IEnumerator FadeDeckShareGlowOutQuad(
    float startingIntensity,
    float finalIntensity,
    float fadeTime)
  {
    if (!((UnityEngine.Object) this.m_DeckShareGlowOutQuad == (UnityEngine.Object) null))
    {
      int propertyID = Shader.PropertyToID("_Intensity");
      float currentIntensity = startingIntensity;
      Material mat = RendererExtension.GetMaterial((Renderer) this.m_DeckShareGlowOutQuad.GetComponentInChildren<MeshRenderer>(true));
      mat.SetFloat(propertyID, currentIntensity);
      float transitionSpeed = Mathf.Abs(finalIntensity - startingIntensity) / fadeTime;
      while ((double) currentIntensity != (double) finalIntensity)
      {
        currentIntensity = Mathf.MoveTowards(currentIntensity, finalIntensity, transitionSpeed * Time.deltaTime);
        mat.SetFloat(propertyID, currentIntensity);
        yield return (object) null;
      }
    }
  }

  private void SwitchFormatButtonPress(UIEvent e)
  {
    this.m_switchFormatButton.Disable();
    this.m_switchFormatButton.gameObject.SetActive(false);
    this.ShowFormatTypePickerPopup();
    this.TransitionToFormatType(PegasusShared.FormatType.FT_STANDARD, true, 2f);
  }

  public void ShowFormatTypePickerPopup()
  {
    this.m_formatTypePickerWidget.transform.position = new Vector3(0.0f, this.m_formatTypePickerYOffset, 0.0f);
    this.m_formatTypePickerWidget.Show();
    this.UpdateAvailableFormatOptions();
    this.m_formatTypePickerWidget.TriggerEvent("OPEN");
  }

  public void ShowPopupDuringSetRotation(VisualsFormatType visualsFormatType)
  {
    this.m_formatTypePickerWidget.transform.position = new Vector3(0.0f, this.m_formatTypePickerYOffset, 0.0f);
    this.m_formatTypePickerWidget.Show();
    this.m_formatTypePickerWidget.TriggerEvent("SET_ROTATION_OPEN");
  }

  private void SwitchFormatTypeAndRankedPlayMode(VisualsFormatType newVisualsFormatType)
  {
    if (VisualsFormatTypeExtensions.ToVisualsFormatType(Options.GetFormatType(), Options.GetInRankedPlayMode()) != newVisualsFormatType)
    {
      if (newVisualsFormatType.ToFormatType() == PegasusShared.FormatType.FT_UNKNOWN)
      {
        RankMgr.LogMessage("newVisualsFormatType.ToFormatType() = FT_UNKOWN", nameof (SwitchFormatTypeAndRankedPlayMode), "D:\\builders\\work\\source\\25.0.0\\Pegasus\\Client\\Assets\\Game\\DeckPickerTray\\DeckPickerTrayDisplay.cs", 2200);
        return;
      }
      Options.SetFormatType(newVisualsFormatType.ToFormatType());
      Options.SetInRankedPlayMode(newVisualsFormatType.IsRanked());
      this.TransitionToFormatType(newVisualsFormatType.ToFormatType(), newVisualsFormatType.IsRanked(), 2f);
    }
    RankMgr.Get().SetRankPresenceField();
    this.m_visualsFormatType = newVisualsFormatType;
    this.m_switchFormatButton.SetVisualsFormatType(this.m_visualsFormatType);
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    switch (mode)
    {
      case SceneMgr.Mode.COLLECTIONMANAGER:
        this.UpdateCreateDeckText();
        this.UpdateFormat_CollectionManager();
        break;
      case SceneMgr.Mode.TOURNAMENT:
        this.UpdateFormat_Tournament(newVisualsFormatType);
        TournamentDisplay.Get().UpdateHeaderText();
        this.m_rankedPlayDisplay.OnSwitchFormat(newVisualsFormatType);
        break;
    }
    this.m_missingClassicDeck.SetActive(false);
    if (newVisualsFormatType == VisualsFormatType.VFT_CLASSIC && mode == SceneMgr.Mode.TOURNAMENT && CollectionManager.Get().GetNumberOfClassicDecks() == 0)
      this.m_missingClassicDeck.SetActive(true);
    this.UpdatePageArrows();
    this.m_formatTypePickerWidget.TriggerEvent("HIDE");
    this.StartCoroutine(this.m_switchFormatButton.EnableWithDelay(0.8f));
    if (mode != SceneMgr.Mode.TOURNAMENT)
      return;
    if (this.ShouldShowRotatedBoosterPopup(newVisualsFormatType))
    {
      this.StartCoroutine(this.ShowRotatedBoostersPopup());
    }
    else
    {
      if (!this.ShouldShowStandardDeckVO(newVisualsFormatType))
        return;
      this.StartCoroutine(this.ShowStandardDeckVO());
    }
  }

  public static bool OnNavigateBack()
  {
    if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null)
      return DeckPickerTrayDisplay.Get().OnNavigateBackImplementation();
    Debug.LogError((object) "HeroPickerTrayDisplay: tried to navigate back but had null instance!");
    return false;
  }

  protected override bool OnNavigateBackImplementation()
  {
    if (!this.m_backButton.IsEnabled())
      return false;
    switch (SceneMgr.Get() != null ? SceneMgr.Get().GetMode() : SceneMgr.Mode.INVALID)
    {
      case SceneMgr.Mode.COLLECTIONMANAGER:
      case SceneMgr.Mode.TAVERN_BRAWL:
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
        if ((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null)
          CollectionDeckTray.Get().GetDecksContent().CreateNewDeckCancelled();
        if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null && !DeckPickerTrayDisplay.Get().m_heroChosen && (UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
          collectibleDisplay.CancelSelectNewDeckHeroMode();
        if ((UnityEngine.Object) HeroPickerDisplay.Get() != (UnityEngine.Object) null)
          HeroPickerDisplay.Get().HideTray();
        PresenceMgr.Get().SetPrevStatus();
        if (SceneMgr.Get().IsInTavernBrawlMode())
          TavernBrawlDisplay.Get().EnablePlayButton();
        if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
        {
          DeckTemplatePicker deckTemplatePicker = (bool) UniversalInputManager.UsePhoneUI ? collectibleDisplay.GetPhoneDeckTemplateTray() : collectibleDisplay.m_pageManager.GetDeckTemplatePicker();
          if ((UnityEngine.Object) deckTemplatePicker != (UnityEngine.Object) null)
          {
            Navigation.RemoveHandler(new Navigation.NavigateBackHandler(deckTemplatePicker.OnNavigateBack));
            break;
          }
          break;
        }
        break;
      case SceneMgr.Mode.TOURNAMENT:
        this.BackOutToHub();
        GameMgr.Get().CancelFindGame();
        break;
      case SceneMgr.Mode.ADVENTURE:
        AdventureConfig.Get().SubSceneGoBack();
        if (AdventureConfig.Get().CurrentSubScene == AdventureData.Adventuresubscene.PRACTICE)
          PracticePickerTrayDisplay.Get().gameObject.SetActive(false);
        GameMgr.Get().CancelFindGame();
        break;
    }
    return base.OnNavigateBackImplementation();
  }

  protected override void GoBackUntilOnNavigateBackCalled() => Navigation.GoBackUntilOnNavigateBackCalled(new Navigation.NavigateBackHandler(DeckPickerTrayDisplay.OnNavigateBack));

  public override void PreUnload()
  {
    if (this.IsShowingFirstPage() || !this.m_randomDeckPickerTray.activeSelf)
      return;
    this.HideHeroPickerPage();
  }

  private void ShowNextPage(bool skipTraySlidingAnimation = false) => this.ShowPage(this.m_currentPageIndex + 1, skipTraySlidingAnimation);

  private void ShowPreviousPage(bool skipTraySlidingAnimation = false) => this.ShowPage(this.m_currentPageIndex - 1, skipTraySlidingAnimation);

  private void ShowPage(int pageNum, bool skipTraySlidingAnimation = false)
  {
    if (iTween.Count(this.m_randomDeckPickerTray) > 0 || pageNum < 0 || pageNum >= this.m_customPages.Count)
      return;
    for (int index = 0; index < this.m_customPages.Count; ++index)
    {
      this.m_customPages[index].gameObject.SetActive(index == this.m_currentPageIndex && !skipTraySlidingAnimation || index == pageNum);
      if (skipTraySlidingAnimation)
      {
        Vector3 localPosition = this.m_customDeckPageUpperBone.transform.localPosition;
        if (index < pageNum)
          localPosition = this.m_customDeckPageHideBone.transform.localPosition;
        else if (index > pageNum)
          localPosition = this.m_customDeckPageLowerBone.transform.localPosition;
        this.m_customPages[index].gameObject.transform.localPosition = localPosition;
      }
    }
    if (this.m_currentPageIndex != pageNum && !skipTraySlidingAnimation)
    {
      GameObject currentPage = this.m_customPages[this.m_currentPageIndex].gameObject;
      GameObject gameObject = this.m_customPages[pageNum].gameObject;
      if (pageNum > this.m_currentPageIndex)
      {
        iTween.MoveTo(currentPage, iTween.Hash((object) "time", (object) 0.25f, (object) "position", (object) this.m_customDeckPageHideBone.transform.localPosition, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) (Action<object>) (e => currentPage.SetActive(false)), (object) "oncompletetarget", (object) this.gameObject));
        iTween.MoveTo(gameObject, iTween.Hash((object) "time", (object) 0.25f, (object) "delay", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "position", (object) this.m_customDeckPageUpperBone.transform.localPosition, (object) "isLocal", (object) true));
      }
      else
      {
        iTween.MoveTo(currentPage, iTween.Hash((object) "time", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) (Action<object>) (e => currentPage.SetActive(false)), (object) "position", (object) this.m_customDeckPageLowerBone.transform.localPosition, (object) "isLocal", (object) true));
        iTween.MoveTo(gameObject, iTween.Hash((object) "time", (object) 0.25f, (object) "delay", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "position", (object) this.m_customDeckPageUpperBone.transform.localPosition, (object) "isLocal", (object) true));
      }
    }
    this.m_currentPageIndex = pageNum;
    this.HideAllPreconHighlights();
    this.LowerHeroButtons();
    if (this.ShouldHandleBoxTransition() | skipTraySlidingAnimation)
    {
      this.HideHeroPickerPage();
      Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    }
    else
      iTween.MoveTo(this.m_randomDeckPickerTray, iTween.Hash((object) "time", (object) 0.25f, (object) "position", (object) this.m_randomDecksHiddenBone.transform.localPosition, (object) "oncomplete", (object) (Action<object>) (e => this.HideHeroPickerPage()), (object) "oncompletetarget", (object) this.gameObject, (object) "isLocal", (object) true));
    LoanerDeckDisplay loanerDeckDisplay = LoanerDeckDisplay.Get();
    if ((UnityEngine.Object) loanerDeckDisplay != (UnityEngine.Object) null)
      loanerDeckDisplay.SetCurrentPageStatusInDataModel(this.m_customPages[pageNum].m_isPageForLoanerDecks);
    this.UpdatePageArrows();
    Options.Get().SetBool(Option.HAS_SEEN_CUSTOM_DECK_PICKER, true);
  }

  private IEnumerator ArrowDelayedActivate(UIBButton arrow, float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    arrow.gameObject.SetActive(true);
  }

  private bool ShouldHandleBoxTransition() => SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY && (Box.Get().IsBusy() || Box.Get().GetState() == Box.State.LOADING || Box.Get().GetState() == Box.State.LOADING_HUB);

  private void OnBoxTransitionFinished(object userData)
  {
    if ((UnityEngine.Object) this.m_randomDeckPickerTray != (UnityEngine.Object) null && this.IsShowingFirstPage())
      this.PositionHeroPickerPageAtStartingPos();
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
  }

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    this.HideSetRotationNotifications();
    SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
  }

  private void LowerHeroButtons()
  {
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
    {
      if (heroButton.gameObject.activeSelf)
        heroButton.Lower();
    }
  }

  private void RaiseHeroButtons()
  {
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
    {
      if (heroButton.gameObject.activeSelf)
        heroButton.Raise();
    }
  }

  protected void SetKeyholeTextureOffsets(UnityEngine.Vector2 offset)
  {
    if (this.IsChoosingHero())
      return;
    int num = (bool) UniversalInputManager.UsePhoneUI ? 1 : 0;
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
    {
      Renderer component = heroButton.m_buttonFrame.GetComponent<Renderer>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        Debug.LogWarning((object) "Couldn't set keyhole texture offset on invalid renderer");
      else
        RendererExtension.GetMaterial(component, num).mainTextureOffset = offset;
    }
  }

  private void HideHeroPickerPage() => this.m_randomDeckPickerTray.transform.localPosition = new Vector3(-5000f, -5000f, -5000f);

  private void PositionHeroPickerPageAtStartingPos() => this.m_randomDeckPickerTray.transform.localPosition = this.m_randomDecksHiddenBone.transform.localPosition;

  private void OnShowPreviousPage(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "hero_panel_slide_on.prefab:236147a924d7cb442872b46dddd56132");
    this.ShowPreviousPage();
  }

  private void ShowHeroPickerPage(bool skipTraySlidingAnimation = false)
  {
    if (iTween.Count(this.m_randomDeckPickerTray) > 0)
      return;
    this.m_currentPageIndex = 0;
    if ((UnityEngine.Object) this.m_modeLabelBg != (UnityEngine.Object) null)
      this.m_modeLabelBg.transform.localEulerAngles = new Vector3(180f, 0.0f, 0.0f);
    if (skipTraySlidingAnimation)
    {
      this.m_randomDeckPickerTray.transform.localPosition = this.m_randomDecksShownBone.transform.localPosition;
      this.RaiseHeroButtons();
    }
    else
    {
      this.PositionHeroPickerPageAtStartingPos();
      iTween.MoveTo(this.m_randomDeckPickerTray, iTween.Hash((object) "time", (object) 0.25f, (object) "position", (object) this.m_randomDecksShownBone.transform.localPosition, (object) "isLocal", (object) true, (object) "oncomplete", (object) "RaiseHeroButtons", (object) "oncompletetarget", (object) this.gameObject));
    }
    this.UpdatePageArrows();
  }

  private void OnCustomDeckPressed(CollectionDeckBoxVisual deckbox)
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && Options.GetInRankedPlayMode())
    {
      CollectionDeck collectionDeck = deckbox.GetCollectionDeck();
      if (collectionDeck == null)
        return;
      if (collectionDeck.FormatType == PegasusShared.FormatType.FT_CLASSIC && Options.GetFormatType() != PegasusShared.FormatType.FT_CLASSIC)
      {
        this.ShowClickedClassicDeckInNonClassicPopup();
        return;
      }
      if (collectionDeck.FormatType == PegasusShared.FormatType.FT_STANDARD && Options.GetFormatType() == PegasusShared.FormatType.FT_CLASSIC)
      {
        this.ShowClickedStandardDeckInClassicPopup();
        return;
      }
      if (collectionDeck.FormatType == PegasusShared.FormatType.FT_WILD && Options.GetFormatType() == PegasusShared.FormatType.FT_CLASSIC)
      {
        this.ShowClickedWildDeckInClassicPopup();
        return;
      }
      if (collectionDeck.FormatType == PegasusShared.FormatType.FT_WILD && Options.GetFormatType() == PegasusShared.FormatType.FT_STANDARD)
      {
        this.ShowClickedWildDeckInStandardPopup();
        return;
      }
    }
    if (this.SelectCustomDeck(deckbox))
      return;
    this.HandleClickToFixDeck(deckbox);
  }

  private bool SelectCustomDeck(CollectionDeckBoxVisual deckbox)
  {
    if (!deckbox.CanSelectDeck())
      return false;
    this.HideDemoQuotes();
    this.SetPlayButtonEnabled(true);
    this.RemoveHeroLockedTooltip();
    if (deckbox.m_isLoanerDeck)
    {
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RETURNING_PLAYER_EXPERIENCE, GameSaveKeySubkeyId.LAST_LOANER_DECK_SELECTED_TEMPLATE_ID, new long[1]
      {
        (long) deckbox.GetDeckTemplateId()
      }));
      Options.Get().SetLong(Option.LAST_CUSTOM_DECK_CHOSEN, 0L);
    }
    else
      Options.Get().SetLong(Option.LAST_CUSTOM_DECK_CHOSEN, deckbox.GetDeckID());
    deckbox.SetIsSelected(true);
    if ((bool) AbsDeckPickerTrayDisplay.HIGHLIGHT_SELECTED_DECK)
      deckbox.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    else
      deckbox.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
    if ((UnityEngine.Object) this.m_selectedCustomDeckBox != (UnityEngine.Object) null && (UnityEngine.Object) this.m_selectedCustomDeckBox != (UnityEngine.Object) deckbox)
    {
      this.m_selectedCustomDeckBox.SetIsSelected(false);
      this.m_selectedCustomDeckBox.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
    }
    this.m_selectedCustomDeckBox = deckbox;
    this.UpdateHeroInfo(deckbox);
    this.ShowPreconHero(true);
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_slidingTray.ToggleTraySlider(true);
    LoanerDeckDisplay loanerDeckDisplay = LoanerDeckDisplay.Get();
    if ((UnityEngine.Object) loanerDeckDisplay != (UnityEngine.Object) null)
      loanerDeckDisplay.SetSelectedDeckInDataModel(deckbox.m_isLoanerDeck);
    this.m_heroActor.UpdateDeckRunesComponent(deckbox.GetCollectionDeck());
    return true;
  }

  private void HandleClickToFixDeck(CollectionDeckBoxVisual deckBox)
  {
    if ((UnityEngine.Object) deckBox == (UnityEngine.Object) null || !deckBox.IsDeckEnabled())
      return;
    CollectionDeck collectionDeck = deckBox.GetCollectionDeck();
    if (collectionDeck == null)
      return;
    DeckRuleset ruleset = collectionDeck.GetRuleset();
    if (ruleset != null && ruleset.EntityInDeckIgnoresRuleset(collectionDeck))
      return;
    CollectionDeck.CardCountByStatus deckCardCount = collectionDeck.CountCardsByStatus(deckBox.GetFormatTypeToValidateAgainst());
    if (deckCardCount.Extra > 0)
    {
      this.HandleExtraCards(collectionDeck, deckCardCount);
    }
    else
    {
      if (deckCardCount.MissingPlusInvalid <= 0)
        return;
      this.HandleMissingAndInvalidCards(collectionDeck, deckCardCount);
    }
  }

  private void HandleExtraCards(CollectionDeck deck, CollectionDeck.CardCountByStatus deckCardCount)
  {
    GameStrings.PluralNumber[] pluralNumbers = GameStrings.MakePlurals(deckCardCount.Extra);
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_showAlertIcon = false,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
      m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_headerText = GameStrings.FormatPlurals("GLUE_COLLECTION_DECK_EXTRA_CARDS_POPUP_HEADER", pluralNumbers),
      m_text = GameStrings.Format("GLUE_COLLECTION_DECK_EXTRA_CARDS_POPUP_TEXT", (object) deckCardCount.Extra)
    };
    info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
    {
      if (response != AlertPopup.Response.CONFIRM || deck.IsSavingChanges())
        return;
      deck.RemoveExtraCards(new PegasusShared.FormatType?(Options.GetFormatType()));
      this.UpdateDeckVisualsAndSelectDeck(deck);
      deck.SendChanges(CollectionDeck.ChangeSource.ClickToFixExtraCards);
    });
    DialogManager.Get().ShowPopup(info);
  }

  private void HandleMissingAndInvalidCards(
    CollectionDeck deck,
    CollectionDeck.CardCountByStatus deckCardCount)
  {
    if (CollectionManager.Get().HasPendingSmartDeckRequest(deck.ID))
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_showAlertIcon = false,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
      m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_INCOMPLETE_POPUP_HEADER")
    };
    int num = deck.GetSlots().Count<CollectionDeckSlot>((Func<CollectionDeckSlot, bool>) (slot => RankMgr.Get().IsCardLockedInCurrentLeague(slot.GetEntityDef())));
    if (num > 0)
      info.m_text = GameStrings.Format("GLUE_COLLECTION_DECK_INCOMPLETE_POPUP_TEXT_NPR", (object) num);
    else
      info.m_text = GameStrings.Format("GLUE_COLLECTION_DECK_INCOMPLETE_POPUP_TEXT", (object) deckCardCount.MissingPlusInvalid);
    info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
    {
      if (response != AlertPopup.Response.CONFIRM || deck.IsSavingChanges())
        return;
      deck.RemoveInvalidCards(new PegasusShared.FormatType?(Options.GetFormatType()));
      CollectionManager.Get().AutoFillDeck(deck, true, new CollectionManager.DeckAutoFillCallback(this.OnClickToFixAutoFillCallback));
    });
    DialogManager.Get().ShowPopup(info);
  }

  private void OnClickToFixAutoFillCallback(
    CollectionDeck deck,
    IEnumerable<DeckMaker.DeckFill> fillCards)
  {
    if (deck == null)
      return;
    deck.FillFromCardList(fillCards, CollectionDeck.ChangeSource.ClickToFixMissingAndInvalidCards);
    this.UpdateDeckVisualsAndSelectDeck(deck);
  }

  private void UpdateDeckVisualsAndSelectDeck(CollectionDeck deck)
  {
    CustomDeckPage currentCustomPage = this.GetCurrentCustomPage();
    if ((UnityEngine.Object) currentCustomPage == (UnityEngine.Object) null)
      return;
    currentCustomPage.UpdateDeckVisuals();
    CollectionDeckBoxVisual deckVisual = currentCustomPage.FindDeckVisual(deck);
    if ((UnityEngine.Object) deckVisual == (UnityEngine.Object) null || !this.SelectCustomDeck(deckVisual) || (bool) UniversalInputManager.UsePhoneUI)
      return;
    deckVisual.PlayGlowAnim();
  }

  protected override void OnHeroButtonReleased(UIEvent e)
  {
    base.OnHeroButtonReleased(e);
    this.HideDemoQuotes();
  }

  protected override void SelectHero(HeroPickerButton button, bool showTrayForPhone = true)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) this.m_selectedHeroButton && !(bool) UniversalInputManager.UsePhoneUI)
      return;
    base.SelectHero(button, showTrayForPhone);
    Options.Get().SetInt(Option.LAST_PRECON_HERO_CHOSEN, (int) button.m_heroClass);
    if (!button.IsLocked())
      return;
    string shortName = button.GetEntityDef().GetShortName();
    string className = GameStrings.GetClassName(button.m_heroClass);
    if (button.m_heroClass == TAG_CLASS.DEATHKNIGHT)
      this.AddHeroLockedTooltip(GameStrings.Get("GLUE_HERO_LOCKED_NAME"), GameStrings.Format("GLUE_HERO_LOCKED_PROLOGUE_DESC", (object) className), button.m_heroClass);
    else
      this.AddHeroLockedTooltip(GameStrings.Get("GLUE_HERO_LOCKED_NAME"), GameStrings.Format("GLUE_HERO_LOCKED_DESC", (object) shortName, (object) className), button.m_heroClass);
  }

  private void Deselect()
  {
    if ((UnityEngine.Object) this.m_selectedHeroButton == (UnityEngine.Object) null && (UnityEngine.Object) this.m_selectedCustomDeckBox == (UnityEngine.Object) null)
      return;
    this.SetPlayButtonEnabled(false);
    if ((UnityEngine.Object) this.m_heroLockedTooltip != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_heroLockedTooltip.gameObject);
    if ((UnityEngine.Object) this.m_selectedCustomDeckBox != (UnityEngine.Object) null)
    {
      this.m_selectedCustomDeckBox.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
      this.m_selectedCustomDeckBox.SetEnabled(true, false);
      this.m_selectedCustomDeckBox.SetIsSelected(false);
      this.m_selectedCustomDeckBox = (CollectionDeckBoxVisual) null;
    }
    this.m_heroActor.SetEntityDef((EntityDef) null);
    this.m_heroActor.SetCardDef((DefLoader.DisposableCardDef) null);
    this.m_heroActor.Hide();
    if ((UnityEngine.Object) this.m_selectedHeroButton != (UnityEngine.Object) null)
    {
      this.m_selectedHeroButton.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
      this.m_selectedHeroButton.SetSelected(false);
      this.m_selectedHeroButton = (HeroPickerButton) null;
    }
    if (this.ShouldShowHeroPower())
    {
      this.m_heroPowerActor.SetCardDef((DefLoader.DisposableCardDef) null);
      this.m_heroPowerActor.SetEntityDef((EntityDef) null);
      this.m_heroPowerActor.Hide();
      this.m_goldenHeroPowerActor.SetCardDef((DefLoader.DisposableCardDef) null);
      this.m_goldenHeroPowerActor.SetEntityDef((EntityDef) null);
      this.m_goldenHeroPowerActor.Hide();
      this.m_heroPower.GetComponent<Collider>().enabled = false;
      this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
      if ((UnityEngine.Object) this.m_heroPowerShadowQuad != (UnityEngine.Object) null)
        this.m_heroPowerShadowQuad.SetActive(false);
    }
    this.m_selectedHeroPowerFullDef = (DefLoader.DisposableFullDef) null;
    if ((UnityEngine.Object) this.m_heroPowerBigCard != (UnityEngine.Object) null)
    {
      iTween.Stop(this.m_heroPowerBigCard.gameObject);
      this.m_heroPowerBigCard.Hide();
    }
    if ((UnityEngine.Object) this.m_goldenHeroPowerBigCard != (UnityEngine.Object) null)
    {
      iTween.Stop(this.m_goldenHeroPowerBigCard.gameObject);
      this.m_goldenHeroPowerBigCard.Hide();
    }
    this.m_selectedHeroName = (string) null;
    this.m_heroName.Text = "";
    if (!((UnityEngine.Object) LoanerDeckDisplay.Get() != (UnityEngine.Object) null))
      return;
    LoanerDeckDisplay.Get().SetSelectedDeckInDataModel(false);
  }

  private void UpdateHeroInfo(CollectionDeckBoxVisual deckBox)
  {
    using (DefLoader.DisposableFullDef fullDef = deckBox.SharedDisposableFullDef())
      this.UpdateHeroInfo(fullDef, deckBox.GetDeckNameText().Text, deckBox.GetHeroCardPremium());
  }

  protected override void UpdateHeroInfo(HeroPickerButton button)
  {
    using (DefLoader.DisposableFullDef fullDef = button.ShareFullDef())
    {
      string name = fullDef.EntityDef.GetName();
      TAG_PREMIUM heroPremium = CollectionManager.Get().GetHeroPremium(fullDef.EntityDef.GetClass());
      this.UpdateHeroInfo(fullDef, name, heroPremium, button.IsLocked());
    }
  }

  private void UpdateHeroInfo(
    DefLoader.DisposableFullDef fullDef,
    string heroName,
    TAG_PREMIUM premium,
    bool locked = false)
  {
    this.m_heroName.Text = heroName;
    this.m_selectedHeroName = fullDef.EntityDef.GetName();
    this.m_heroActor.SetPremium(premium);
    this.m_heroActor.SetFullDef(fullDef);
    this.m_heroActor.UpdateAllComponents();
    this.m_heroActor.SetUnlit();
    this.m_xpBar.UpdateDisplay(!locked ? GameUtils.GetHeroLevel(fullDef.EntityDef.GetClass()) : (NetCache.HeroLevel) null, GameUtils.GetTotalHeroLevel() ?? 0);
    string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(fullDef.EntityDef.GetCardId());
    if (!locked && this.ShouldShowHeroPower() && !string.IsNullOrEmpty(powerCardIdFromHero))
    {
      this.m_heroPowerContainer.SetActive(true);
      DefLoader.DisposableFullDef def;
      this.m_heroPowerDefs.TryGetValue(powerCardIdFromHero, out def);
      if (def == null)
      {
        this.LoadHeroPowerDef(powerCardIdFromHero, premium);
        this.m_heroPowerDefs.TryGetValue(powerCardIdFromHero, out def);
      }
      if (def != null)
        this.UpdateHeroPowerInfo(def, premium);
    }
    else
    {
      this.SetHeroPowerActorColliderEnabled(false);
      this.HideHeroPowerActor();
      this.m_heroPowerContainer.SetActive(false);
    }
    this.UpdateRankedClassWinsPlate();
  }

  protected override void TransitionToFormatType(
    PegasusShared.FormatType formatType,
    bool inRankedPlayMode,
    float transitionSpeed = 2f)
  {
    VisualsFormatType visualsFormatType1 = VisualsFormatTypeExtensions.ToVisualsFormatType(this.m_PreviousFormatType, this.m_PreviousInRankedPlayMode);
    VisualsFormatType visualsFormatType2 = VisualsFormatTypeExtensions.ToVisualsFormatType(formatType, inRankedPlayMode);
    this.m_PreviousFormatType = formatType;
    this.m_PreviousInRankedPlayMode = inRankedPlayMode;
    base.TransitionToFormatType(formatType, inRankedPlayMode, transitionSpeed);
    this.UpdateTrayBackgroundTransitionValues(visualsFormatType1, visualsFormatType2, transitionSpeed);
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      if (!inRankedPlayMode)
      {
        this.m_casualPlayDisplayWidget.Show();
        this.m_rankedPlayDisplay.Hide(this.m_rankedPlayDisplayHideDelay);
      }
      else
      {
        this.m_casualPlayDisplayWidget.Hide();
        if ((UnityEngine.Object) this.m_rankedPlayDisplay != (UnityEngine.Object) null)
        {
          this.m_rankedPlayDisplay.Show(this.m_rankedPlayDisplayShowDelay);
          this.m_rankedPlayDisplay.OnSwitchFormat(visualsFormatType2);
        }
      }
    }
    this.UpdateValidHeroClasses();
    if (this.m_inHeroPicker && (visualsFormatType1 == VisualsFormatType.VFT_CLASSIC && visualsFormatType2 != VisualsFormatType.VFT_CLASSIC || visualsFormatType1 != VisualsFormatType.VFT_CLASSIC && visualsFormatType2 == VisualsFormatType.VFT_CLASSIC))
    {
      this.Deselect();
      this.StartCoroutine(this.LoadHeroButtons());
    }
    this.PlayTrayTransitionSound(visualsFormatType2);
    this.PlayTrayTransitionGlowBursts(visualsFormatType1, visualsFormatType2);
  }

  private void UpdateTrayBackgroundTransitionValues(
    VisualsFormatType oldVisualsFormatType,
    VisualsFormatType visualsFormatType,
    float transitionSpeed = 2f)
  {
    float targetValue = 1f;
    Texture textureForFormat1 = this.m_currentModeTextures.GetTextureForFormat(oldVisualsFormatType);
    Texture textureForFormat2 = this.m_currentModeTextures.GetCustomTextureForFormat(oldVisualsFormatType);
    Texture textureForFormat3 = this.m_currentModeTextures.GetTextureForFormat(visualsFormatType);
    Texture textureForFormat4 = this.m_currentModeTextures.GetCustomTextureForFormat(visualsFormatType);
    this.SetCustomDeckPageTextures(textureForFormat2, textureForFormat4);
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.SetPhoneDetailsTrayTextures(textureForFormat1, textureForFormat3);
    else
      this.SetTrayFrameAndBasicDeckPageTextures(textureForFormat1, textureForFormat3);
    this.StopCoroutine("TransitionTrayMaterial");
    this.StartCoroutine(this.TransitionTrayMaterial(targetValue, transitionSpeed));
  }

  private void PlayTrayTransitionGlowBursts(
    VisualsFormatType oldVisualsFormatType,
    VisualsFormatType visualsFormatType)
  {
    if (oldVisualsFormatType == visualsFormatType)
      return;
    if (this.m_customPages != null && (oldVisualsFormatType == VisualsFormatType.VFT_WILD || visualsFormatType == VisualsFormatType.VFT_WILD))
    {
      bool useFX = oldVisualsFormatType == VisualsFormatType.VFT_WILD;
      bool hasValidStandardDeck = this.GetNumValidStandardDecks() > 0U;
      if (this.m_customPages.Count > 1 && !this.IsShowingFirstPage())
        this.m_customPages[1].PlayVineGlowBurst(useFX, hasValidStandardDeck);
      else if (this.m_customPages.Count > 0)
      {
        if (hasValidStandardDeck)
        {
          foreach (GameObject gameObject in this.m_customPages[0].m_customVineGlowToggle)
            gameObject.SetActive(true);
        }
        this.m_customPages[0].PlayVineGlowBurst(useFX, hasValidStandardDeck);
      }
    }
    if (this.m_inHeroPicker)
      this.PlayTransitionGlowBurstsForNewDeckFSMs(oldVisualsFormatType, visualsFormatType);
    else
      this.PlayTransitionGlowBurstsForNonNewDeckFSMs(oldVisualsFormatType, visualsFormatType);
  }

  private void PlayTransitionGlowBurstsForNonNewDeckFSMs(
    VisualsFormatType oldVisualsFormatType,
    VisualsFormatType visualsFormatType)
  {
    string str1;
    switch (oldVisualsFormatType)
    {
      case VisualsFormatType.VFT_WILD:
        str1 = this.m_leavingWildGlowEvent;
        break;
      case VisualsFormatType.VFT_CLASSIC:
        str1 = this.m_leavingClassicGlowEvent;
        break;
      case VisualsFormatType.VFT_CASUAL:
        str1 = this.m_leavingCasualGlowEvent;
        break;
      default:
        str1 = (string) null;
        break;
    }
    if (!string.IsNullOrEmpty(str1))
    {
      foreach (PlayMakerFSM formatChangeGlowFsM in this.formatChangeGlowFSMs)
      {
        if ((UnityEngine.Object) formatChangeGlowFsM != (UnityEngine.Object) null)
          formatChangeGlowFsM.SendEvent(str1);
      }
      if ((UnityEngine.Object) this.m_rankedPlayDisplay != (UnityEngine.Object) null)
        this.m_rankedPlayDisplay.PlayTransitionGlowBurstsForNonNewDeckFSMs(str1);
    }
    string str2;
    switch (visualsFormatType)
    {
      case VisualsFormatType.VFT_WILD:
        str2 = this.m_enteringWildGlowEvent;
        break;
      case VisualsFormatType.VFT_CLASSIC:
        str2 = this.m_enteringClassicGlowEvent;
        break;
      case VisualsFormatType.VFT_CASUAL:
        str2 = this.m_enteringCasualGlowEvent;
        break;
      default:
        str2 = (string) null;
        break;
    }
    if (string.IsNullOrEmpty(str2))
      return;
    foreach (PlayMakerFSM formatChangeGlowFsM in this.formatChangeGlowFSMs)
    {
      if ((UnityEngine.Object) formatChangeGlowFsM != (UnityEngine.Object) null)
        formatChangeGlowFsM.SendEvent(str2);
    }
    if (!((UnityEngine.Object) this.m_rankedPlayDisplay != (UnityEngine.Object) null))
      return;
    this.m_rankedPlayDisplay.PlayTransitionGlowBurstsForNonNewDeckFSMs(str2);
  }

  private void PlayTransitionGlowBurstsForNewDeckFSMs(
    VisualsFormatType oldVisualsFormatType,
    VisualsFormatType visualsFormatType)
  {
    string str = (string) null;
    if (oldVisualsFormatType == VisualsFormatType.VFT_CLASSIC && visualsFormatType != VisualsFormatType.VFT_CLASSIC)
      str = this.m_newDeckLeavingClassicGlowEvent;
    else if (oldVisualsFormatType != VisualsFormatType.VFT_CLASSIC && visualsFormatType == VisualsFormatType.VFT_CLASSIC)
      str = this.m_newDeckEnteringClassicGlowEvent;
    else if (oldVisualsFormatType == VisualsFormatType.VFT_WILD && visualsFormatType != VisualsFormatType.VFT_WILD)
      str = this.m_newDeckLeavingWildGlowEvent;
    else if (oldVisualsFormatType != VisualsFormatType.VFT_WILD && visualsFormatType == VisualsFormatType.VFT_WILD)
      str = this.m_newDeckEnteringWildGlowEvent;
    if (string.IsNullOrEmpty(str))
      return;
    foreach (PlayMakerFSM formatChangeGlowFsM in this.newDeckFormatChangeGlowFSMs)
    {
      if ((UnityEngine.Object) formatChangeGlowFsM != (UnityEngine.Object) null)
        formatChangeGlowFsM.SendEvent(str);
    }
    if (!((UnityEngine.Object) this.m_rankedPlayDisplay != (UnityEngine.Object) null))
      return;
    this.m_rankedPlayDisplay.PlayTransitionGlowBurstsForNewDeckFSMs(str);
  }

  private void PlayTrayTransitionSound(VisualsFormatType visualsFormatType)
  {
    switch (Box.Get().GetState())
    {
      case Box.State.LOADING:
        return;
      case Box.State.SET_ROTATION_OPEN:
        if (this.m_setRotationTutorialState == DeckPickerTrayDisplay.SetRotationTutorialState.PREPARING)
          return;
        break;
    }
    string assetRef;
    switch (visualsFormatType)
    {
      case VisualsFormatType.VFT_WILD:
      case VisualsFormatType.VFT_CASUAL:
        assetRef = this.m_wildTransitionSound;
        break;
      case VisualsFormatType.VFT_STANDARD:
        assetRef = this.m_standardTransitionSound;
        break;
      case VisualsFormatType.VFT_CLASSIC:
        assetRef = this.m_classicTransitionSound;
        break;
      default:
        Debug.LogError((object) ("No transition sound for format " + visualsFormatType.ToString()));
        assetRef = "";
        break;
    }
    if (string.IsNullOrEmpty(assetRef))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) assetRef);
  }

  private IEnumerator TransitionTrayMaterial(float targetValue, float speed)
  {
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    Material trayMat = (Material) null;
    Material detailsTrayMat = (Material) null;
    Material randomTrayMat = (Material) null;
    float currentValue;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      trayMat = (Material) null;
      detailsTrayMat = RendererExtension.GetSharedMaterial(pickerTrayDisplay.m_detailsTrayFrame.GetComponent<Renderer>());
      randomTrayMat = RendererExtension.GetSharedMaterial(pickerTrayDisplay.m_basicDeckPage.GetComponent<Renderer>());
      currentValue = randomTrayMat.GetFloat("_Transistion");
    }
    else
    {
      trayMat = RendererExtension.GetSharedMaterial(pickerTrayDisplay.m_trayFrame.GetComponentInChildren<Renderer>());
      currentValue = trayMat.GetFloat("_Transistion");
      Renderer componentInChildren = pickerTrayDisplay.m_basicDeckPage.GetComponentInChildren<Renderer>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
        randomTrayMat = RendererExtension.GetSharedMaterial(componentInChildren);
    }
    do
    {
      currentValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);
      if ((UnityEngine.Object) trayMat != (UnityEngine.Object) null)
        trayMat.SetFloat("_Transistion", currentValue);
      if ((UnityEngine.Object) detailsTrayMat != (UnityEngine.Object) null)
        detailsTrayMat.SetFloat("_Transistion", currentValue);
      if ((UnityEngine.Object) randomTrayMat != (UnityEngine.Object) null)
        randomTrayMat.SetFloat("_Transistion", currentValue);
      if (pickerTrayDisplay.m_customPages != null)
      {
        foreach (CustomDeckPage customPage in pickerTrayDisplay.m_customPages)
          customPage.UpdateTrayTransitionValue(currentValue);
      }
      yield return (object) null;
    }
    while ((double) currentValue != (double) targetValue);
  }

  private void SetTrayFrameAndBasicDeckPageTextures(
    Texture mainTexture,
    Texture transitionToTexture)
  {
    Material sharedMaterial1 = RendererExtension.GetSharedMaterial(this.m_trayFrame.GetComponentInChildren<Renderer>());
    sharedMaterial1.mainTexture = mainTexture;
    sharedMaterial1.SetTexture("_MainTex2", transitionToTexture);
    sharedMaterial1.SetFloat("_Transistion", 0.0f);
    Renderer componentInChildren = this.m_basicDeckPage.GetComponentInChildren<Renderer>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    Material sharedMaterial2 = RendererExtension.GetSharedMaterial(componentInChildren);
    sharedMaterial2.mainTexture = mainTexture;
    sharedMaterial2.SetTexture("_MainTex2", transitionToTexture);
    sharedMaterial2.SetFloat("_Transistion", 0.0f);
  }

  private void SetCustomDeckPageTextures(Texture transitionFromTexture, Texture targetTexture)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      Material sharedMaterial = RendererExtension.GetSharedMaterial(this.m_basicDeckPage.GetComponent<Renderer>());
      sharedMaterial.mainTexture = transitionFromTexture;
      sharedMaterial.SetTexture("_MainTex2", targetTexture);
      sharedMaterial.SetFloat("_Transistion", 0.0f);
    }
    if (this.m_customPages == null)
      return;
    foreach (CustomDeckPage customPage in this.m_customPages)
      customPage.SetTrayTextures(transitionFromTexture, targetTexture);
  }

  private void SetPhoneDetailsTrayTextures(Texture transitionFromTexture, Texture targetTexture)
  {
    Material sharedMaterial = RendererExtension.GetSharedMaterial(this.m_detailsTrayFrame.GetComponent<Renderer>());
    if (!this.m_slidingTray.IsShown() || this.m_slidingTray.IsAnimatingToShow())
    {
      sharedMaterial.mainTexture = targetTexture;
      sharedMaterial.SetTexture("_MainTex2", targetTexture);
      sharedMaterial.SetFloat("_Transistion", 0.0f);
    }
    else
    {
      sharedMaterial.mainTexture = transitionFromTexture;
      sharedMaterial.SetTexture("_MainTex2", targetTexture);
      sharedMaterial.SetFloat("_Transistion", 0.0f);
    }
  }

  private void OnRankedPlayDisplayWidgetReady()
  {
    this.m_rankedPlayDisplayWidget.transform.SetParent(this.m_rankedPlayDisplayWidgetBone, false);
    this.m_rankedPlayDisplay = this.m_rankedPlayDisplayWidget.GetComponentInChildren<RankedPlayDisplay>();
    this.UpdateRankedPlayDisplay(VisualsFormatTypeExtensions.GetCurrentVisualsFormatType());
    this.StartCoroutine(this.SetRankedMedalWhenReady());
  }

  private void OnFormatTypePickerPopupReady()
  {
    this.m_formatTypePickerWidget.transform.SetParent(this.gameObject.transform);
    this.m_formatTypePickerWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnFormatTypePickerPopupEvent));
    this.UpdateAvailableFormatOptions();
  }

  private void UpdateAvailableFormatOptions()
  {
    int num = CollectionManager.Get().ShouldAccountSeeStandardWild() ? 1 : 0;
    bool inHeroPicker = this.m_inHeroPicker;
    if (num != 0)
      this.m_formatTypePickerWidget.TriggerEvent(inHeroPicker ? "3BUTTONS" : "4BUTTONS");
    else
      this.m_formatTypePickerWidget.TriggerEvent("2BUTTONS");
  }

  private void OnFormatTypePickerPopupEvent(string eventName)
  {
    if (eventName == "WILD_BUTTON_CLICKED")
      this.SwitchFormatTypeAndRankedPlayMode(VisualsFormatType.VFT_WILD);
    else if (eventName == "STANDARD_BUTTON_CLICKED")
      this.SwitchFormatTypeAndRankedPlayMode(VisualsFormatType.VFT_STANDARD);
    else if (eventName == "CLASSIC_BUTTON_CLICKED")
      this.SwitchFormatTypeAndRankedPlayMode(VisualsFormatType.VFT_CLASSIC);
    else if (eventName == "CASUAL_BUTTON_CLICKED")
    {
      this.SwitchFormatTypeAndRankedPlayMode(VisualsFormatType.VFT_CASUAL);
    }
    else
    {
      if (!(eventName == "HIDE"))
        return;
      this.FireFormatTypePickerClosedEvent();
    }
  }

  private IEnumerator SetRankedMedalWhenReady()
  {
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    while (TournamentDisplay.Get().GetCurrentMedalInfo() == null)
      yield return (object) null;
    pickerTrayDisplay.OnMedalChanged(TournamentDisplay.Get().GetCurrentMedalInfo());
    TournamentDisplay.Get().RegisterMedalChangedListener(new TournamentDisplay.DelMedalChanged(pickerTrayDisplay.OnMedalChanged));
  }

  private void OnMedalChanged(NetCache.NetCacheMedalInfo medalInfo) => this.m_rankedPlayDisplay.OnMedalChanged(medalInfo);

  protected override void OnPlayGameButtonReleased(UIEvent e)
  {
    if (SetRotationManager.Get().CheckForSetRotationRollover() || PlayerMigrationManager.Get() != null && PlayerMigrationManager.Get().CheckForPlayerMigrationRequired())
      return;
    this.HideDemoQuotes();
    this.HideSetRotationNotifications();
    this.m_heroChosen = true;
    base.OnPlayGameButtonReleased(e);
  }

  protected override void SetCollectionButtonEnabled(bool enable)
  {
    base.SetCollectionButtonEnabled(enable);
    this.UpdateCollectionButtonGlow();
  }

  private void UpdateCollectionButtonGlow()
  {
    if (this.ShouldGlowCollectionButton())
      this.m_collectionButtonGlow.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    else
      this.m_collectionButtonGlow.ChangeState(ActorStateType.HIGHLIGHT_OFF);
  }

  private void InitSwitchFormatButton()
  {
    if (!((UnityEngine.Object) this.m_switchFormatButtonContainer != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_switchFormatButtonContainer.PrefabGameObject() != (UnityEngine.Object) null))
      return;
    this.m_switchFormatButton = this.m_switchFormatButtonContainer.PrefabGameObject().GetComponent<SwitchFormatButton>();
    if (!RankMgr.Get().IsNewPlayer())
    {
      PegasusShared.FormatType formatType;
      bool inRankedPlayMode;
      if (this.m_inHeroPicker)
      {
        formatType = Options.GetFormatType();
        inRankedPlayMode = true;
      }
      else
      {
        formatType = Options.GetFormatType();
        inRankedPlayMode = Options.GetInRankedPlayMode();
      }
      this.m_visualsFormatType = VisualsFormatTypeExtensions.ToVisualsFormatType(formatType, inRankedPlayMode);
      this.m_switchFormatButton.SetVisualsFormatType(this.m_visualsFormatType);
      this.m_switchFormatButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.SwitchFormatButtonPress));
      switch (SceneMgr.Get().GetMode())
      {
        case SceneMgr.Mode.COLLECTIONMANAGER:
          if (CollectionManager.Get().AccountHasUnlockedWild())
          {
            this.m_switchFormatButton.Uncover();
            break;
          }
          this.m_switchFormatButton.Cover();
          this.m_switchFormatButton.Disable();
          break;
        case SceneMgr.Mode.TOURNAMENT:
          this.m_switchFormatButton.Uncover();
          break;
        case SceneMgr.Mode.FRIENDLY:
        case SceneMgr.Mode.ADVENTURE:
        case SceneMgr.Mode.TAVERN_BRAWL:
          this.m_switchFormatButton.Cover();
          this.m_switchFormatButton.Disable();
          break;
      }
    }
    else
    {
      this.m_switchFormatButton.Cover();
      this.m_switchFormatButton.Disable();
    }
  }

  protected override void ShowHero()
  {
    if ((UnityEngine.Object) this.m_selectedHeroButton != (UnityEngine.Object) null)
      this.UpdateHeroInfo(this.m_selectedHeroButton);
    else if ((UnityEngine.Object) this.m_selectedCustomDeckBox != (UnityEngine.Object) null)
    {
      this.UpdateHeroInfo(this.m_selectedCustomDeckBox);
    }
    else
    {
      Log.All.PrintError("DeckPickerTrayDisplay.ShowHero with no button or deck box selected!");
      return;
    }
    base.ShowHero();
    this.SetLockedPortraitMaterial(this.m_selectedHeroButton);
  }

  protected override void SetHeroRaised(bool raised)
  {
    this.m_xpBar.SetEnabled(raised);
    base.SetHeroRaised(raised);
  }

  private void HideAllPreconHighlights()
  {
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
      heroButton.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
  }

  protected override void PlayGame()
  {
    base.PlayGame();
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.COLLECTIONMANAGER:
        this.SelectHeroForCollectionManager();
        break;
      case SceneMgr.Mode.TOURNAMENT:
        if (this.BlockOnInvalidDeckHero())
          return;
        long selectedDeckId1 = this.GetSelectedDeckID();
        if (!this.m_selectedCustomDeckBox.m_isLoanerDeck && this.GetSelectedDeckID() == 0L && this.GetSelectedDeckTemplateID() == 0)
        {
          Debug.LogError((object) "Trying to play game with deck ID 0!");
          return;
        }
        this.SetBackButtonEnabled(false);
        PegasusShared.GameType forNewPlayModeGame1 = this.GetGameTypeForNewPlayModeGame();
        PegasusShared.FormatType forNewPlayModeGame2 = this.GetFormatTypeForNewPlayModeGame();
        Options.Get().SetEnum<PegasusShared.FormatType>(Option.FORMAT_TYPE_LAST_PLAYED, forNewPlayModeGame2);
        if (!this.HandleMysteriousDeck(forNewPlayModeGame1, selectedDeckId1))
        {
          int deckTemplateId = this.m_selectedCustomDeckBox.GetDeckTemplateId();
          if (deckTemplateId == 0)
            GameMgr.Get().FindGame(forNewPlayModeGame1, forNewPlayModeGame2, 2, deckId: selectedDeckId1);
          else
            GameMgr.Get().FindGame(forNewPlayModeGame1, forNewPlayModeGame2, 2, deckTemplateId: deckTemplateId);
          bool flag = true;
          if (forNewPlayModeGame1 == PegasusShared.GameType.GT_RANKED && RankMgr.Get().IsLegendRank(forNewPlayModeGame2))
            flag = false;
          if (flag)
          {
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_QUEUE);
            break;
          }
          break;
        }
        break;
      case SceneMgr.Mode.FRIENDLY:
        if (!FriendChallengeMgr.Get().IsChallengeTavernBrawl())
        {
          if (this.BlockOnInvalidDeckHero())
            return;
          long selectedDeckId2 = this.GetSelectedDeckID();
          if (selectedDeckId2 == 0L)
          {
            Debug.LogError((object) "Trying to play friendly game with deck ID 0!");
            return;
          }
          FriendChallengeMgr.Get().SelectDeck(selectedDeckId2);
          FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_DECK", new AlertPopup.ResponseCallback(((AbsDeckPickerTrayDisplay) this).OnFriendChallengeWaitingForOpponentDialogResponse));
          break;
        }
        goto case SceneMgr.Mode.TAVERN_BRAWL;
      case SceneMgr.Mode.ADVENTURE:
        long selectedDeckId3 = this.GetSelectedDeckID();
        AdventureConfig adventureConfig = AdventureConfig.Get();
        if (adventureConfig.GetSelectedAdventure() == AdventureDbId.NAXXRAMAS && !Options.Get().GetBool(Option.HAS_PLAYED_NAXX))
        {
          AdTrackingManager.Get().TrackAdventureProgress(Option.HAS_PLAYED_NAXX.ToString());
          Options.Get().SetBool(Option.HAS_PLAYED_NAXX, true);
        }
        switch (adventureConfig.CurrentSubScene)
        {
          case AdventureData.Adventuresubscene.PRACTICE:
            if (this.BlockOnInvalidDeckHero())
              return;
            PracticePickerTrayDisplay.Get().Show();
            this.SetHeroRaised(false);
            break;
          case AdventureData.Adventuresubscene.MISSION_DECK_PICKER:
            if (!this.OnPlayButtonPressed_SaveHeroAndAdvanceToDungeonRunIfNecessary())
            {
              int heroCardDbId = 0;
              if ((UnityEngine.Object) this.m_selectedHeroButton != (UnityEngine.Object) null && this.m_selectedHeroButton.m_heroClass != TAG_CLASS.INVALID)
                heroCardDbId = GameUtils.GetFavoriteHeroCardDBIdFromClass(this.m_selectedHeroButton.m_heroClass);
              ScenarioDbId missionToPlay = adventureConfig.GetMissionToPlay();
              if (GameDbf.Scenario.GetRecord((int) missionToPlay).RuleType == Scenario.RuleType.CHOOSE_HERO)
              {
                GameMgr.Get().FindGameWithHero(PegasusShared.GameType.GT_VS_AI, PegasusShared.FormatType.FT_WILD, (int) missionToPlay, 0, heroCardDbId);
                break;
              }
              GameMgr.Get().FindGame(PegasusShared.GameType.GT_VS_AI, PegasusShared.FormatType.FT_WILD, (int) missionToPlay, deckId: selectedDeckId3);
              break;
            }
            break;
        }
        break;
      case SceneMgr.Mode.TAVERN_BRAWL:
        if (!TavernBrawlManager.Get().SelectHeroBeforeMission())
        {
          this.SelectHeroForCollectionManager();
          break;
        }
        break;
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        if (FiresideGatheringManager.Get().InBrawlMode() && !TavernBrawlManager.Get().SelectHeroBeforeMission())
        {
          this.SelectHeroForCollectionManager();
          break;
        }
        if (FiresideGatheringManager.Get().InBrawlMode() && GameUtils.IsAIMission(TavernBrawlManager.Get().CurrentMission().missionId))
        {
          if (!TavernBrawlManager.Get().SelectHeroBeforeMission())
          {
            TavernBrawlManager.Get().StartGame();
            break;
          }
          break;
        }
        if (!FiresideGatheringManager.Get().InBrawlMode() || !TavernBrawlManager.Get().SelectHeroBeforeMission())
        {
          long selectedDeckId4 = this.GetSelectedDeckID();
          FriendChallengeMgr.Get().SelectDeckBeforeSendingChallenge(selectedDeckId4);
          break;
        }
        break;
    }
    if (!(bool) UniversalInputManager.UsePhoneUI || SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE && AdventureConfig.Get().CurrentSubScene == AdventureData.Adventuresubscene.PRACTICE)
      return;
    this.m_slidingTray.ToggleTraySlider(false);
  }

  private bool BlockOnInvalidDeckHero()
  {
    if (GameUtils.IsCardGameplayEventActive(this.m_selectedCustomDeckBox.GetHeroCardID()))
      return false;
    DialogManager.Get().ShowClassUpcomingPopup();
    return true;
  }

  private bool HandleMysteriousDeck(PegasusShared.GameType gameType, long deckId)
  {
    if (gameType != PegasusShared.GameType.GT_RANKED)
      return false;
    CollectionDeck deck = CollectionManager.Get().GetDeck(deckId);
    if (deck == null || DeckPickerTrayDisplay.s_mysteriousDeck.Count != deck.GetSlotCount())
      return false;
    foreach (CollectionDeckSlot slot in deck.GetSlots())
    {
      if (slot == null)
        return false;
      int dbId = GameUtils.TranslateCardIdToDbId(slot.CardID);
      int num;
      if (!DeckPickerTrayDisplay.s_mysteriousDeck.TryGetValue(dbId, out num) || slot.Count != num)
        return false;
    }
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(5026);
    if (record == null)
      return false;
    GameMgr.Get().FindGameWithHero(PegasusShared.GameType.GT_VS_AI, PegasusShared.FormatType.FT_WILD, 5026, 0, record.Player1HeroCardId, (long) record.Player1DeckId);
    return true;
  }

  private void SelectHeroForCollectionManager()
  {
    if ((UnityEngine.Object) this.m_selectedHeroButton == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "DeckPickerTrayDisplay.SelectHeroForCollectionManager called when m_selectedHeroButton was null");
    }
    else
    {
      this.m_backButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(((AbsDeckPickerTrayDisplay) this).OnBackButtonReleased));
      Navigation.RemoveHandler(new Navigation.NavigateBackHandler(DeckPickerTrayDisplay.OnNavigateBack));
      if (DeckPickerTrayDisplay.s_selectHeroCoroutine != null)
        Processor.CancelCoroutine(DeckPickerTrayDisplay.s_selectHeroCoroutine);
      DeckPickerTrayDisplay.s_selectHeroCoroutine = Processor.RunCoroutine(DeckPickerTrayDisplay.SelectHeroForCollectionManagerImpl(this.m_selectedHeroButton.GetEntityDef()));
    }
  }

  private static IEnumerator SelectHeroForCollectionManagerImpl(EntityDef heroDef)
  {
    PegasusShared.FormatType formatType = Options.GetFormatType();
    if (formatType == PegasusShared.FormatType.FT_UNKNOWN)
    {
      RankMgr.LogMessage("Options.GetFormatType() = FT_UNKOWN", nameof (SelectHeroForCollectionManagerImpl), "D:\\builders\\work\\source\\25.0.0\\Pegasus\\Client\\Assets\\Game\\DeckPickerTray\\DeckPickerTrayDisplay.cs", 4146);
    }
    else
    {
      CollectionManager.s_HeroPickerFormat = formatType;
      if ((UnityEngine.Object) HeroPickerDisplay.Get() != (UnityEngine.Object) null)
        HeroPickerDisplay.Get().HideTray((bool) UniversalInputManager.UsePhoneUI ? 0.25f : 0.0f);
      CollectionDeckTray deckTray = CollectionDeckTray.Get();
      DeckTrayDeckListContent decksContent = deckTray.GetDecksContent();
      if (SceneMgr.Get().IsInTavernBrawlMode())
      {
        decksContent.CreateNewDeckFromUserSelection(heroDef.GetClass(), heroDef.GetCardId());
        CollectionManager.Get().GetCollectibleDisplay().EnableInput(true);
      }
      else
      {
        CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
        DeckTemplatePicker deckTemplatePicker = (DeckTemplatePicker) null;
        if (CollectionManager.Get().GetNonStarterTemplateDecks(Options.GetFormatType(), heroDef.GetClass()).Count > 0)
          deckTemplatePicker = (bool) UniversalInputManager.UsePhoneUI ? collectibleDisplay.GetPhoneDeckTemplateTray() : collectibleDisplay.m_pageManager.GetDeckTemplatePicker();
        if ((UnityEngine.Object) deckTemplatePicker != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI)
          deckTemplatePicker.m_phoneBackButton.SetEnabled(false);
        deckTray.m_doneButton.SetEnabled(false);
        while (deckTray.IsUpdatingTrayMode() || decksContent.NumDecksToDelete() > 0 || deckTray.IsWaitingToDeleteDeck())
          yield return (object) null;
        decksContent.CreateNewDeckFromUserSelection(heroDef.GetClass(), heroDef.GetCardId());
        while ((UnityEngine.Object) deckTemplatePicker != (UnityEngine.Object) null && !deckTemplatePicker.IsShowingPacks())
          yield return (object) null;
        if ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() != (UnityEngine.Object) null)
          CollectionManager.Get().GetCollectibleDisplay().EnableInput(true);
        while ((UnityEngine.Object) deckTray != (UnityEngine.Object) null && deckTray.IsUpdatingTrayMode())
          yield return (object) null;
        if ((UnityEngine.Object) deckTemplatePicker != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI)
          deckTemplatePicker.m_phoneBackButton.SetEnabled(true);
        if ((UnityEngine.Object) deckTray != (UnityEngine.Object) null)
          deckTray.m_doneButton.SetEnabled(true);
      }
    }
  }

  protected override void OnSlidingTrayToggled(bool isShowing)
  {
    base.OnSlidingTrayToggled(isShowing);
    if (!isShowing)
      return;
    this.TransitionToFormatType(Options.GetFormatType(), Options.GetInRankedPlayMode(), 2f);
  }

  protected override IEnumerator InitModeWhenReady()
  {
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    while (pickerTrayDisplay.ShouldLoadCustomDecks() && !pickerTrayDisplay.CustomPagesReady() || SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && (UnityEngine.Object) pickerTrayDisplay.m_rankedPlayDisplay == (UnityEngine.Object) null)
    {
      if (!SceneMgr.Get().DoesCurrentSceneSupportOfflineActivity() && !Network.IsLoggedIn())
      {
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        yield break;
      }
      else
        yield return (object) null;
    }
    if (!pickerTrayDisplay.IsChoosingHero())
    {
      while (!NetCache.Get().IsNetObjectAvailable<NetCache.NetCacheDecks>())
        yield return (object) null;
    }
    // ISSUE: reference to a compiler-generated method
    yield return (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.\u003C\u003En__1());
    pickerTrayDisplay.InitMode();
    while (LoadingScreen.Get().IsTransitioning())
      yield return (object) null;
    if (pickerTrayDisplay.ShouldShowBonusStarsPopUp())
      pickerTrayDisplay.ShowBonusStarsPopup();
    else
      pickerTrayDisplay.PlayEnterModeDialogues();
  }

  private bool CustomPagesReady()
  {
    if (this.m_customPages == null)
      return false;
    foreach (CustomDeckPage customPage in this.m_customPages)
    {
      if ((UnityEngine.Object) customPage == (UnityEngine.Object) null || !customPage.PageReady())
        return false;
    }
    return true;
  }

  private CustomDeckPage GetCurrentCustomPage() => this.m_currentPageIndex < this.m_customPages.Count ? this.m_customPages[this.m_currentPageIndex] : (CustomDeckPage) null;

  protected override void InitRichPresence(Global.PresenceStatus? newStatus = null)
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT)
      newStatus = new Global.PresenceStatus?(Global.PresenceStatus.PLAY_DECKPICKER);
    base.InitRichPresence(newStatus);
  }

  private void SetSelectionAndPageFromOptions()
  {
    bool flag = (bool) UniversalInputManager.UsePhoneUI && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY;
    long deckId;
    if (this.HasNewRewardedDeck(out deckId))
      RewardUtils.MarkNewestRewardedDeckAsSeen();
    else
      deckId = this.GetLastChosenDeckId();
    int pageNum;
    CollectionDeckBoxVisual deckbox = this.GetDeckboxWithDeckID(deckId, out pageNum);
    FreeDeckMgr.FreeDeckStatus status = FreeDeckMgr.Get().Status;
    if ((UnityEngine.Object) deckbox == (UnityEngine.Object) null && status == FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD)
    {
      long deckTemplateId = 0;
      GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.RETURNING_PLAYER_EXPERIENCE, GameSaveKeySubkeyId.LAST_LOANER_DECK_SELECTED_TEMPLATE_ID, out deckTemplateId);
      if (deckTemplateId != 0L)
        deckbox = this.GetDeckBoxWithDeckTemplateId(deckTemplateId, out pageNum);
    }
    long num = 0;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.RETURNING_PLAYER_EXPERIENCE, GameSaveKeySubkeyId.HAS_SEEN_LOANER_DECKS_ON_FIRST_LOGIN_TRIAL_START, out num);
    if (status == FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD && num <= 0L)
    {
      deckbox = (CollectionDeckBoxVisual) null;
      pageNum = 0;
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RETURNING_PLAYER_EXPERIENCE, GameSaveKeySubkeyId.HAS_SEEN_LOANER_DECKS_ON_FIRST_LOGIN_TRIAL_START, new long[1]
      {
        1L
      }));
    }
    this.ShowPage(pageNum, true);
    if (flag || !((UnityEngine.Object) deckbox != (UnityEngine.Object) null))
      return;
    this.SelectCustomDeck(deckbox);
  }

  private bool HasNewRewardedDeck(out long deckId)
  {
    bool flag = RewardUtils.HasNewRewardedDeck(out deckId);
    if (!flag || this.HasValidDeckboxWithId(deckId))
      return flag;
    Log.DeckTray.PrintWarning("HasNewRewardedDeckId - Newest rewarded deck ID option was set to an invalid deck ID: {0}", (object) deckId);
    return false;
  }

  private bool HasValidDeckboxWithId(long deckId) => (UnityEngine.Object) this.GetDeckboxWithDeckID(deckId) != (UnityEngine.Object) null;

  private long GetLastChosenDeckId() => SceneMgr.Get().GetMode() != SceneMgr.Mode.FRIENDLY ? Options.Get().GetLong(Option.LAST_CUSTOM_DECK_CHOSEN) : 0L;

  private CollectionDeckBoxVisual GetDeckboxWithDeckID(long deckID) => this.GetDeckboxWithDeckID(deckID, out int _);

  private CollectionDeckBoxVisual GetDeckboxWithDeckID(
    long deckID,
    out int pageNum)
  {
    pageNum = 0;
    while (pageNum < this.m_customPages.Count)
    {
      CollectionDeckBoxVisual deckboxWithDeckId = this.m_customPages[pageNum].GetDeckboxWithDeckID(deckID);
      if ((UnityEngine.Object) deckboxWithDeckId != (UnityEngine.Object) null)
        return deckboxWithDeckId;
      ++pageNum;
    }
    pageNum = 0;
    return (CollectionDeckBoxVisual) null;
  }

  private CollectionDeckBoxVisual GetDeckBoxWithDeckTemplateId(
    long deckTemplateId,
    out int pageNum)
  {
    pageNum = 0;
    while (pageNum < this.m_customPages.Count)
    {
      CustomDeckPage customPage = this.m_customPages[pageNum];
      if (customPage.m_isPageForLoanerDecks)
      {
        CollectionDeckBoxVisual withDeckTemplateId = customPage.GetDeckboxWithDeckTemplateID(deckTemplateId);
        if ((UnityEngine.Object) withDeckTemplateId != (UnityEngine.Object) null)
          return withDeckTemplateId;
      }
      ++pageNum;
    }
    pageNum = 0;
    return (CollectionDeckBoxVisual) null;
  }

  protected override void OnFriendChallengeWaitingForOpponentDialogResponse(
    AlertPopup.Response response,
    object userData)
  {
    if (response != AlertPopup.Response.CANCEL || FriendChallengeMgr.Get().AmIInGameState())
      return;
    this.Deselect();
    base.OnFriendChallengeWaitingForOpponentDialogResponse(response, userData);
  }

  protected override void OnFriendChallengeChanged(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData,
    object userData)
  {
    base.OnFriendChallengeChanged(challengeEvent, player, challengeData, userData);
    switch (challengeEvent)
    {
      case FriendChallengeEvent.I_ACCEPTED_DECK_SHARE_REQUEST:
      case FriendChallengeEvent.I_DECLINED_DECK_SHARE_REQUEST:
        if (!FriendChallengeMgr.Get().DidISelectDeckOrHero())
          break;
        FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_DECK", new AlertPopup.ResponseCallback(((AbsDeckPickerTrayDisplay) this).OnFriendChallengeWaitingForOpponentDialogResponse));
        break;
      case FriendChallengeEvent.I_CANCELED_DECK_SHARE_REQUEST:
        this.OnDeckShareRequestCancelDeclineOrError();
        break;
      case FriendChallengeEvent.I_ENDED_DECK_SHARE:
        this.StopUsingSharedDecks();
        break;
      case FriendChallengeEvent.I_RECEIVED_SHARED_DECKS:
        this.UseSharedDecks(FriendChallengeMgr.Get().GetSharedDecks());
        break;
      case FriendChallengeEvent.DECK_SHARE_ERROR_OCCURED:
        this.OnDeckShareRequestCancelDeclineOrError();
        break;
      case FriendChallengeEvent.OPPONENT_DECLINED_DECK_SHARE_REQUEST:
        this.OnDeckShareRequestCancelDeclineOrError();
        break;
    }
  }

  protected override void OnHeroFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef fullDef,
    object userData)
  {
    base.OnHeroFullDefLoaded(cardId, fullDef, userData);
    if (!this.IsChoosingHero() || this.m_heroDefsLoading > 0)
      return;
    this.InitButtonAchievements();
  }

  protected override void OnHeroActorLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    base.OnHeroActorLoaded(assetRef, go, callbackData);
    if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      return;
    this.m_xpBar.transform.parent = this.m_heroActor.GetRootObject().transform;
    this.m_xpBar.transform.localScale = new Vector3(0.89f, 0.89f, 0.89f);
    this.m_xpBar.transform.localPosition = new Vector3(-0.1776525f, 0.2245596f, -0.7309282f);
    this.m_xpBar.m_isOnDeck = false;
    this.m_heroActor.AddCustomFrameCallback(new Actor.CustomFrameChangedEventHandler(this.OnCustomFrameLoadedCallback));
  }

  protected override bool ShouldShowHeroPower() => !(bool) UniversalInputManager.UsePhoneUI || this.IsChoosingHero();

  private bool IsDeckSharingActive() => !((UnityEngine.Object) this.m_DeckShareRequestButton == (UnityEngine.Object) null) && SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY && FriendChallengeMgr.Get().IsDeckShareEnabled() && !this.IsChoosingHero();

  private bool ShouldShowCollectionButton() => !this.IsDeckSharingActive() && !this.IsChoosingHero() && SceneMgr.Get().GetMode() != SceneMgr.Mode.FRIENDLY;

  private bool ShouldGlowCollectionButton() => this.ShouldShowCollectionButton() && this.m_collectionButton.IsEnabled() && (!Options.Get().GetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_DECK) && this.HaveDecksThatNeedNames() || !Options.Get().GetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_CARD) && this.HaveUnseenCards() || Options.Get().GetBool(Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION) && SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT);

  private bool HaveDecksThatNeedNames()
  {
    foreach (CollectionDeck deck in CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK))
    {
      if (deck.NeedsName)
        return true;
    }
    return false;
  }

  private uint GetNumValidStandardDecks()
  {
    uint validStandardDecks = 0;
    foreach (CollectionDeck deck in CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK))
    {
      if (deck.IsValidForFormat(PegasusShared.FormatType.FT_STANDARD) && deck.IsValidForRuleset)
        ++validStandardDecks;
    }
    return validStandardDecks;
  }

  private bool HaveUnseenCards()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    int? manaCost = new int?();
    TAG_RARITY? rarity = new TAG_RARITY?();
    TAG_RACE? race = new TAG_RACE?();
    int? nullable1 = new int?(1);
    bool? nullable2 = new bool?(true);
    bool? isHero = new bool?(false);
    int? minOwned = nullable1;
    bool? notSeen = nullable2;
    bool? isCraftable = new bool?();
    bool? filterCoreCounterpartCards = new bool?();
    return collectionManager.FindCards(manaCost: manaCost, rarity: rarity, race: race, isHero: isHero, minOwned: minOwned, notSeen: notSeen, isCraftable: isCraftable, returnAfterFirstResult: true, filterCoreCounterpartCards: filterCoreCounterpartCards).m_cards.Count > 0;
  }

  private void PlayEnterModeDialogues()
  {
    if (this.ShowInnkeeperQuoteIfNeeded())
      return;
    this.ShowWhizbangPopupIfNeeded();
  }

  private bool ShowInnkeeperQuoteIfNeeded()
  {
    bool flag = false;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER && Options.Get().GetBool(Option.SHOW_WILD_DISCLAIMER_POPUP_ON_CREATE_DECK) && Options.GetFormatType() == PegasusShared.FormatType.FT_WILD && UserAttentionManager.CanShowAttentionGrabber("DeckPickTrayDisplay.ShowInnkeeperQuoteIfNeeded:" + (object) Option.SHOW_WILD_DISCLAIMER_POPUP_ON_CREATE_DECK))
    {
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get("VO_INNKEEPER_PLAY_STANDARD_TO_WILD"), "VO_INNKEEPER_Male_Dwarf_SetRotation_43.prefab:4b4ce858139927946905ec0d40d5b3c1");
      Options.Get().SetBool(Option.SHOW_WILD_DISCLAIMER_POPUP_ON_CREATE_DECK, false);
      flag = true;
    }
    return flag;
  }

  private bool ShowWhizbangPopupIfNeeded()
  {
    if (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY)
      return false;
    LastGameData lastGameData = GameMgr.Get().LastGameData;
    if (lastGameData.GameResult != TAG_PLAYSTATE.WON || !lastGameData.HasWhizbangDeckID())
      return false;
    int val = Options.Get().GetInt(Option.WHIZBANG_POPUP_COUNTER);
    if (val >= 7)
      return false;
    CollectionManager.TemplateDeck templateDeck = CollectionManager.Get().GetTemplateDeck(lastGameData.WhizbangDeckID);
    if (templateDeck == null || templateDeck.m_event != SpecialEventType.UNKNOWN && !SpecialEventManager.Get().IsEventActive(templateDeck.m_event, false))
      return false;
    bool flag = false;
    if (val == 0 || val == 2 || val == 6)
    {
      if (UserAttentionManager.CanShowAttentionGrabber("DeckPickerTrayDisplay.ShowWhizbangPopupIfNeeded()"))
      {
        this.StartCoroutine(this.ShowWhizbangPopup(templateDeck));
        ++val;
        flag = true;
      }
    }
    else
      ++val;
    Options.Get().SetInt(Option.WHIZBANG_POPUP_COUNTER, val);
    return flag;
  }

  private IEnumerator ShowWhizbangPopup(CollectionManager.TemplateDeck whizbangDeck)
  {
    if (whizbangDeck != null)
    {
      yield return (object) new WaitForSeconds(1f);
      DialogManager.Get().ShowBasicPopup(UserAttentionBlocker.NONE, new BasicPopup.PopupInfo()
      {
        m_prefabAssetRefs = {
          "WhizbangDialog_notification.prefab:89912cf72b2d5cf47820d2328de40f3f"
        },
        m_headerText = GameStrings.Get("GLUE_COLLECTION_MANAGER_WHIZBANG_POPUP_HEADER"),
        m_bodyText = GameStrings.Format("GLUE_COLLECTION_MANAGER_WHIZBANG_POPUP_BODY", (object) GameStrings.GetClassName(whizbangDeck.m_class), (object) whizbangDeck.m_title),
        m_disableBnetBar = true
      });
    }
  }

  private void SetLockedPortraitMaterial(HeroPickerButton button)
  {
    if (!((UnityEngine.Object) button != (UnityEngine.Object) null) || !button.IsLocked())
      return;
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    if ((mode == SceneMgr.Mode.TAVERN_BRAWL || mode == SceneMgr.Mode.FRIENDLY && FriendChallengeMgr.Get().IsChallengeTavernBrawl() ? 1 : (mode != SceneMgr.Mode.FIRESIDE_GATHERING ? 0 : (FiresideGatheringManager.Get().InBrawlMode() ? 1 : 0))) != 0)
      return;
    using (DefLoader.DisposableFullDef disposableFullDef = button.ShareFullDef())
    {
      if ((UnityEngine.Object) disposableFullDef.CardDef.m_LockedClassPortrait == (UnityEngine.Object) null)
        return;
      this.m_heroActor.SetPortraitMaterial(disposableFullDef.CardDef.m_LockedClassPortrait);
    }
  }

  private bool ShouldLoadCustomDecks()
  {
    if (this.m_deckPickerMode == DeckPickerMode.INVALID)
      Debug.LogWarning((object) "DeckPickerTrayDisplay.ShouldLoadCustomDecks() - querying m_deckPickerMode when it hasn't been set yet!");
    return this.IsDeckSharingActive() || this.m_deckPickerMode == DeckPickerMode.CUSTOM;
  }

  private RankedPlayDataModel GetBonusStarsPopupDataModel()
  {
    TournamentDisplay tournamentDisplay = TournamentDisplay.Get();
    if ((UnityEngine.Object) tournamentDisplay == (UnityEngine.Object) null)
      return (RankedPlayDataModel) null;
    NetCache.NetCacheMedalInfo currentMedalInfo = tournamentDisplay.GetCurrentMedalInfo();
    return currentMedalInfo == null ? (RankedPlayDataModel) null : new MedalInfoTranslator(currentMedalInfo).CreateDataModel(Options.GetFormatType(), RankedMedal.DisplayMode.Default);
  }

  private void ShowInvalidClassPopup()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLUE_CLASS_INVALID_DECK_TITLE");
    info.m_text = GameStrings.Get("GLUE_CLASS_INVALID_DECK_DESC");
    info.m_showAlertIcon = false;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
    DialogManager.DialogProcessCallback callback = (DialogManager.DialogProcessCallback) ((dialog, userData) =>
    {
      this.SetPlayButtonEnabled(true);
      return true;
    });
    DialogManager.Get().ShowPopup(info, callback);
  }

  private void UpdatePageArrows()
  {
    bool flag1 = true;
    bool flag2 = true;
    if (this.m_numPagesToShow <= 1 || Options.GetFormatType() == PegasusShared.FormatType.FT_CLASSIC && CollectionManager.Get().GetNumberOfClassicDecks() == 0 || DemoMgr.Get().IsExpoDemo() || this.IsChoosingHero())
    {
      flag1 = false;
      flag2 = false;
    }
    else
    {
      if (this.IsShowingFirstPage())
        flag1 = false;
      if (this.IsShowingLastPage())
        flag2 = false;
    }
    if (flag1)
    {
      if (!this.m_leftArrow.gameObject.activeInHierarchy)
        this.m_showLeftArrowCoroutine = this.StartCoroutine(this.ArrowDelayedActivate(this.m_leftArrow, 0.25f));
    }
    else
    {
      if (this.m_showLeftArrowCoroutine != null)
        this.StopCoroutine(this.m_showLeftArrowCoroutine);
      this.m_leftArrow.gameObject.SetActive(false);
    }
    if (flag2)
    {
      if (this.m_rightArrow.gameObject.activeInHierarchy)
        return;
      this.m_showRightArrowCoroutine = this.StartCoroutine(this.ArrowDelayedActivate(this.m_rightArrow, 0.25f));
    }
    else
    {
      if (this.m_showRightArrowCoroutine != null)
        this.StopCoroutine(this.m_showRightArrowCoroutine);
      this.m_rightArrow.gameObject.SetActive(false);
    }
  }

  private bool IsShowingFirstPage() => this.m_currentPageIndex == 0;

  private bool IsShowingLastPage() => this.m_currentPageIndex == this.m_customPages.Count - 1;

  private void OnCustomFrameLoadedCallback(CustomFrameController customFrameController)
  {
    if (customFrameController != null)
      SetContainerOffsets(customFrameController.HeroPowerContainerOffset);
    else
      SetContainerOffsets(0.0f);

    void SetContainerOffsets(float offset)
    {
      if (!((UnityEngine.Object) this.m_heroPowerContainer != (UnityEngine.Object) null))
        return;
      if (!this.m_heroPowerContainerOffset.HasValue)
        this.m_heroPowerContainerOffset = new Vector3?(this.m_heroPowerContainer.transform.localPosition);
      this.m_heroPowerContainer.transform.localPosition = this.m_heroPowerContainerOffset.Value + new Vector3(0.0f, offset, 0.0f);
    }
  }

  public void InitSetRotationTutorial(bool veteranFlow)
  {
    if (this.m_setRotationTutorialState != DeckPickerTrayDisplay.SetRotationTutorialState.INACTIVE)
    {
      Debug.LogError((object) ("Tried to call DeckPickerTrayDisplay.InitTutorial() when m_setRotationTutorialState was " + this.m_setRotationTutorialState.ToString()));
    }
    else
    {
      this.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.PREPARING;
      this.m_switchFormatButton.Disable();
      this.m_switchFormatButton.gameObject.SetActive(false);
      this.TransitionToFormatType(PegasusShared.FormatType.FT_STANDARD, true, 2f);
      Options.SetFormatType(PegasusShared.FormatType.FT_STANDARD);
      Options.SetInRankedPlayMode(true);
      this.Deselect();
      this.ShowPage(0, true);
      this.m_rankedPlayDisplay.StartSetRotationTutorial();
      this.SetPlayButtonEnabled(false);
      this.SetBackButtonEnabled(false);
      this.SetCollectionButtonEnabled(false);
      this.m_rightArrow.gameObject.SetActive(false);
      this.m_leftArrow.gameObject.SetActive(false);
      this.m_rightArrow.SetEnabled(false);
      this.m_leftArrow.SetEnabled(false);
      this.SetHeaderText(GameStrings.Get("GLUE_TOURNAMENT"));
      if ((UnityEngine.Object) this.m_heroPower != (UnityEngine.Object) null)
        this.m_heroPower.GetComponent<Collider>().enabled = false;
      if ((UnityEngine.Object) this.m_goldenHeroPower != (UnityEngine.Object) null)
        this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
      foreach (CustomDeckPage customPage in this.m_customPages)
        customPage.EnableDeckButtons(false);
      this.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.READY;
    }
  }

  public void StartSetRotationTutorial(SetRotationClock.DisableTheClockCallback callback)
  {
    if (this.m_setRotationTutorialState == DeckPickerTrayDisplay.SetRotationTutorialState.READY)
    {
      this.StartCoroutine(this.ShowSetRotationTutorialPopups(callback));
    }
    else
    {
      Debug.LogError((object) "Tried to start Play Screen Set Rotation Tutorial without calling DeckPickerTrayDisplay.InitTutorial()");
      callback();
    }
  }

  private IEnumerator ShowSetRotationTutorialPopups(
    SetRotationClock.DisableTheClockCallback disableClockCallback)
  {
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    bool veteranFlow = SetRotationManager.HasSeenStandardModeTutorial();
    pickerTrayDisplay.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.SHOW_TUTORIAL_POPUPS;
    pickerTrayDisplay.m_dimQuad.GetComponent<Renderer>().enabled = true;
    pickerTrayDisplay.m_dimQuad.enabled = true;
    pickerTrayDisplay.m_dimQuad.StopPlayback();
    pickerTrayDisplay.m_dimQuad.Play("DimQuad_FadeIn");
    GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
    if (gameSaveDataManager != null)
    {
      long num1 = -1;
      long num2 = -1;
      gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.ROTATED_BOOSTER_POPUP_PROGRESS, out num1);
      gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.INNKEEPER_STANDARD_DECKS_VO_PROGRESS, out num2);
      bool flag = false;
      List<GameSaveDataManager.SubkeySaveRequest> requests = new List<GameSaveDataManager.SubkeySaveRequest>();
      if (num1 == 0L)
      {
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.ROTATED_BOOSTER_POPUP_PROGRESS, new long[1]
        {
          1L
        }));
        flag = true;
      }
      if (num2 == 0L)
      {
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.INNKEEPER_STANDARD_DECKS_VO_PROGRESS, new long[1]
        {
          1L
        }));
        flag = true;
      }
      if (flag)
        gameSaveDataManager.SaveSubkeys(requests);
    }
    pickerTrayDisplay.m_shouldContinue = false;
    DeckPickerTrayDisplay.Get().AddFormatTypePickerClosedListener(new AbsDeckPickerTrayDisplay.FormatTypePickerClosed(pickerTrayDisplay.ContinueTutorial));
    DeckPickerTrayDisplay.Get().ShowPopupDuringSetRotation(VisualsFormatType.VFT_STANDARD);
    disableClockCallback();
    while (!pickerTrayDisplay.m_shouldContinue)
      yield return (object) null;
    DeckPickerTrayDisplay.Get().RemoveFormatTypePickerClosedListener(new AbsDeckPickerTrayDisplay.FormatTypePickerClosed(pickerTrayDisplay.ContinueTutorial));
    if (veteranFlow)
      pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.ShowWelcomeQuests());
    else
      pickerTrayDisplay.StartSwitchModeWalkthrough();
  }

  private void ContinueTutorial(DialogBase dialog, object userData) => this.m_shouldContinue = true;

  private void ContinueTutorial() => this.m_shouldContinue = true;

  private bool ShouldShowRotatedBoosterPopup(VisualsFormatType newVisualsFormatType)
  {
    switch (newVisualsFormatType)
    {
      case VisualsFormatType.VFT_WILD:
        if (!newVisualsFormatType.IsRanked())
          break;
        goto case VisualsFormatType.VFT_STANDARD;
      case VisualsFormatType.VFT_STANDARD:
        GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
        if (gameSaveDataManager != null)
        {
          long num = -1;
          gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.ROTATED_BOOSTER_POPUP_PROGRESS, out num);
          if (num == 1L)
            return true;
          break;
        }
        break;
    }
    return false;
  }

  private IEnumerator ShowRotatedBoostersPopup(Action callbackOnHide = null)
  {
    yield return (object) new WaitForSeconds(1f);
    if (UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_INTRO, "ShowSetRotationTutorialDialog"))
    {
      DialogManager.Get().ShowSetRotationTutorialPopup(UserAttentionBlocker.SET_ROTATION_INTRO, new SetRotationRotatedBoostersPopup.SetRotationRotatedBoostersPopupInfo()
      {
        m_onHiddenCallback = callbackOnHide
      });
      GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
      if (gameSaveDataManager != null)
        gameSaveDataManager.SaveSubkeys(new List<GameSaveDataManager.SubkeySaveRequest>()
        {
          new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.ROTATED_BOOSTER_POPUP_PROGRESS, new long[1]
          {
            2L
          })
        });
    }
  }

  private void StartSwitchModeWalkthrough()
  {
    this.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.SWITCH_MODE_WALKTHROUGH;
    this.StartCoroutine(this.TutorialSwitchToStandard());
  }

  private IEnumerator TutorialSwitchToStandard()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    pickerTrayDisplay.m_switchFormatPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_INTRO, pickerTrayDisplay.m_Switch_Format_Notification_Bone.position, pickerTrayDisplay.m_Switch_Format_Notification_Bone.localScale, GameStrings.Get("GLUE_TOURNAMENT_SWITCH_MODE"));
    if ((UnityEngine.Object) pickerTrayDisplay.m_switchFormatPopup != (UnityEngine.Object) null)
    {
      Notification.PopUpArrowDirection direction = (bool) UniversalInputManager.UsePhoneUI ? Notification.PopUpArrowDirection.RightUp : Notification.PopUpArrowDirection.Up;
      pickerTrayDisplay.m_switchFormatPopup.ShowPopUpArrow(direction);
    }
    pickerTrayDisplay.m_switchFormatButton.EnableHighlight(true);
    pickerTrayDisplay.m_switchFormatButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(pickerTrayDisplay.OnSwitchFormatReleased));
    pickerTrayDisplay.m_switchFormatButton.Enable();
    return false;
  }

  private void OnSwitchFormatReleased(UIEvent e)
  {
    if (this.m_setRotationTutorialState == DeckPickerTrayDisplay.SetRotationTutorialState.SWITCH_MODE_WALKTHROUGH)
    {
      this.m_switchFormatButton.Disable();
      this.m_switchFormatButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSwitchFormatReleased));
      Processor.QueueJob("LoginManager.CompleteLoginFlow", LoginManager.Get().CompleteLoginFlow());
      this.StartCoroutine(this.ShowWelcomeQuests());
    }
    else
      Debug.Log((object) "OnSwitchFormatReleased called when not in SWITCH_MODE_WALKTHROUGH Set Rotation Tutorial state");
  }

  private void PlayTransitionSounds()
  {
    if (!this.m_customPages[this.m_currentPageIndex].HasWildDeck() || string.IsNullOrEmpty(this.m_wildDeckTransitionSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_wildDeckTransitionSound);
  }

  private void MarkSetRotationComplete()
  {
    this.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.INACTIVE;
    Options.Get().SetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, true);
    SetRotationManager.Get().SetRotationIntroProgress();
    GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_SEEN_WILD_CRAFT_ALERT, false);
    foreach (long id in this.m_noticeIdsToAck)
      Network.Get().AckNotice(id);
  }

  private IEnumerator ShowWelcomeQuests()
  {
    this.MarkSetRotationComplete();
    this.m_switchFormatButton.EnableHighlight(false);
    NotificationManager.Get().DestroyNotification(this.m_switchFormatPopup, 0.0f);
    this.m_switchFormatPopup = (Notification) null;
    this.m_dimQuad.StopPlayback();
    this.m_dimQuad.Play("DimQuad_FadeOut");
    yield return (object) new WaitForEndOfFrame();
    yield return (object) new WaitForSeconds(this.m_dimQuad.GetCurrentAnimatorStateInfo(0).length);
    this.m_dimQuad.GetComponent<Renderer>().enabled = false;
    this.m_dimQuad.enabled = false;
    yield return (object) new WaitForSeconds(this.m_showQuestPause);
    this.OnWelcomeQuestDismiss();
  }

  private void OnWelcomeQuestDismiss() => this.StartCoroutine(this.EndTutorial());

  private IEnumerator EndTutorial()
  {
    DeckPickerTrayDisplay pickerTrayDisplay = this;
    yield return (object) new WaitForSeconds(pickerTrayDisplay.m_playVOPause);
    if ((UnityEngine.Object) pickerTrayDisplay.m_heroPower != (UnityEngine.Object) null)
      pickerTrayDisplay.m_heroPower.GetComponent<Collider>().enabled = true;
    if ((UnityEngine.Object) pickerTrayDisplay.m_goldenHeroPower != (UnityEngine.Object) null)
      pickerTrayDisplay.m_goldenHeroPower.GetComponent<Collider>().enabled = true;
    pickerTrayDisplay.SetBackButtonEnabled(true);
    pickerTrayDisplay.SetCollectionButtonEnabled(true);
    pickerTrayDisplay.m_rightArrow.SetEnabled(true);
    pickerTrayDisplay.m_leftArrow.SetEnabled(true);
    pickerTrayDisplay.m_leftArrow.gameObject.SetActive(!pickerTrayDisplay.IsShowingFirstPage());
    pickerTrayDisplay.m_rightArrow.gameObject.SetActive(!pickerTrayDisplay.IsShowingLastPage());
    foreach (CustomDeckPage customPage in pickerTrayDisplay.m_customPages)
      customPage.EnableDeckButtons(true);
    Options.Get().SetBool(Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION, true);
    pickerTrayDisplay.UpdateCollectionButtonGlow();
    if ((UnityEngine.Object) pickerTrayDisplay.m_switchFormatButton != (UnityEngine.Object) null)
      pickerTrayDisplay.m_switchFormatButton.Enable();
    UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
  }

  private bool ShouldShowStandardDeckVO(VisualsFormatType newVisualsFormatType)
  {
    if (newVisualsFormatType == VisualsFormatType.VFT_STANDARD)
    {
      GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
      if (gameSaveDataManager != null)
      {
        long num = -1;
        gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.INNKEEPER_STANDARD_DECKS_VO_PROGRESS, out num);
        if (num == 1L)
          return true;
      }
    }
    return false;
  }

  private IEnumerator ShowStandardDeckVO()
  {
    yield return (object) new WaitForSeconds(1f);
    switch (this.GetNumValidStandardDecks())
    {
      case 0:
        GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
        if (gameSaveDataManager == null)
          break;
        gameSaveDataManager.SaveSubkeys(new List<GameSaveDataManager.SubkeySaveRequest>()
        {
          new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.SET_ROTATION, GameSaveKeySubkeyId.INNKEEPER_STANDARD_DECKS_VO_PROGRESS, new long[1]
          {
            2L
          })
        });
        break;
      case 1:
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_INTRO, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_HAVE_ONE_STANDARD_DECK"), "VO_INNKEEPER_Male_Dwarf_HAVE_STANDARD_DECK_07.prefab:282cd0db8b3737d4bb55d71f915074e4");
        goto case 0;
      default:
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_INTRO, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_HAVE_STANDARD_DECKS"), "VO_INNKEEPER_Male_Dwarf_HAVE_STANDARD_DECKS_08.prefab:0c1c2ab2c4ead094abc69ec278aa4878");
        goto case 0;
    }
  }

  [Serializable]
  public class ModeTextures
  {
    [SerializeField]
    public Texture customStandardTex;
    [SerializeField]
    public Texture customWildTex;
    [SerializeField]
    public Texture customClassicTex;
    [SerializeField]
    public Texture customCasualTex;
    [SerializeField]
    public Texture standardTex;
    [SerializeField]
    public Texture wildTex;
    [SerializeField]
    public Texture classicTex;
    [SerializeField]
    public Texture casualTex;
    [SerializeField]
    public Texture classDivotTex;
    [SerializeField]
    public Texture guestHeroDivotTex;

    public Texture GetTextureForFormat(VisualsFormatType visualsFormatType)
    {
      switch (visualsFormatType)
      {
        case VisualsFormatType.VFT_WILD:
          return this.wildTex;
        case VisualsFormatType.VFT_STANDARD:
          return this.standardTex;
        case VisualsFormatType.VFT_CLASSIC:
          return this.classicTex;
        case VisualsFormatType.VFT_CASUAL:
          return this.casualTex;
        default:
          Debug.LogError((object) ("ModeTextures.GetTextureForFormat does not support " + visualsFormatType.ToString()));
          return (Texture) null;
      }
    }

    public Texture GetCustomTextureForFormat(VisualsFormatType visualsFormatType)
    {
      switch (visualsFormatType)
      {
        case VisualsFormatType.VFT_WILD:
          return this.customWildTex;
        case VisualsFormatType.VFT_STANDARD:
          return this.customStandardTex;
        case VisualsFormatType.VFT_CLASSIC:
          return this.customClassicTex;
        case VisualsFormatType.VFT_CASUAL:
          return this.customCasualTex;
        default:
          Debug.LogError((object) ("ModeTextures.GetTextureForFormat does not support " + visualsFormatType.ToString()));
          return (Texture) null;
      }
    }
  }

  private enum SetRotationTutorialState
  {
    INACTIVE,
    PREPARING,
    READY,
    SHOW_TUTORIAL_POPUPS,
    SWITCH_MODE_WALKTHROUGH,
    SHOW_QUEST_LOG,
  }
}
