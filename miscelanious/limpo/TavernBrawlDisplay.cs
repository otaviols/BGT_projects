using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Configuration;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DungeonCrawl;
using PegasusShared;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class TavernBrawlDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_createDeckButton;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_editDeckButton;
  [CustomEditField(Sections = "Buttons")]
  public PlayButton m_playButton;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_backButton;
  [CustomEditField(Sections = "Buttons")]
  public PegUIElement m_rewardChest;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_viewDeckButton;
  [CustomEditField(Sections = "Strings")]
  public UberText m_chalkboardHeader;
  [CustomEditField(Sections = "Strings")]
  public UberText m_chalkboardInfo;
  [CustomEditField(Sections = "Strings")]
  public UberText m_chalkboardEndInfo;
  [CustomEditField(Sections = "Strings")]
  public UberText m_numWins;
  [CustomEditField(Sections = "Strings")]
  public UberText m_TavernBrawlHeadline;
  [CustomEditField(Sections = "Animating Elements")]
  public SlidingTray m_tavernBrawlTray;
  [CustomEditField(Sections = "Animating Elements")]
  public SlidingTray m_cardListPanel;
  [CustomEditField(Sections = "Animating Elements")]
  public Animation m_cardCountPanelAnim;
  [CustomEditField(Sections = "Animating Elements")]
  public GameObject m_rewardsPreview;
  [CustomEditField(Sections = "Animating Elements")]
  public GameObject m_rewardContainer;
  [CustomEditField(Sections = "Animating Elements")]
  public UberText m_rewardsText;
  [CustomEditField(Sections = "Animating Elements")]
  public Animator m_LockedDeckTray;
  [CustomEditField(Sections = "Animating Elements")]
  public TavernBrawlPhoneDeckTray m_PhoneDeckTrayView;
  [CustomEditField(Sections = "Animating Elements")]
  public DraftManaCurve m_ManaCurvePhone;
  [CustomEditField(Sections = "Highlights")]
  public HighlightState m_createDeckHighlight;
  [CustomEditField(Sections = "Highlights")]
  public HighlightState m_rewardHighlight;
  [CustomEditField(Sections = "Highlights")]
  public HighlightState m_editDeckHighlight;
  [CustomEditField(Sections = "DungeonCrawl")]
  public float m_transitionStartingOffset = 100f;
  [CustomEditField(Sections = "DungeonCrawl")]
  public float m_transitionTime = 1f;
  [CustomEditField(Sections = "DungeonCrawl")]
  public float m_rootDropHeight = 10f;
  [CustomEditField(Sections = "DungeonCrawl", T = EditType.SOUND_PREFAB)]
  public string m_SlideInSound;
  public GameObject m_winsBanner;
  public GameObject m_panelWithCreateDeck;
  public GameObject m_fullPanel;
  public GameObject m_chalkboard;
  public Material m_chestOpenMaterial;
  public float m_wipeAnimStartDelay;
  public PegUIElement m_rewardOffClickCatcher;
  public GameObject m_editIcon;
  public GameObject m_deleteIcon;
  public UberText m_editText;
  public Color m_disabledTextColor = new Color(0.5f, 0.5f, 0.5f);
  public GameObject m_lossesRoot;
  public LossMarks m_lossMarks;
  public GameObject m_rewardBoxesBone;
  public GameObject m_normalWinLocationBone;
  public GameObject m_sessionWinLocationBone;
  public PegUIElement m_LockedDeckTooltipTrigger;
  public TooltipZone m_LockedDeckTooltipZone;
  public Transform m_SocketHeroBone;
  public BoxCollider m_clickBlocker;
  public GameObject m_chalkboardFX;
  public FiresideGatheringPlayButtonLantern m_FiresideGatheringPlayButtonLantern;
  public GameObject m_sessionRewardBoxesBone;
  public Vector3 m_firesideArrowHintPositionOffset;
  public Vector3 m_firesideArrowRotation;
  public float m_firesideArrowScale;
  public Texture m_chalkboardTexture;
  private static TavernBrawlDisplay s_instance;
  private bool m_doFirstSeenAnimations;
  private long m_deckBeingEdited;
  private GameObject m_rewardObject;
  private Vector3 m_rewardsScale;
  private readonly string CARD_COUNT_PANEL_OPEN_ANIM = "TavernBrawl_DecksNumberCoverUp_Open";
  private readonly string CARD_COUNT_PANEL_CLOSE_ANIM = "TavernBrawl_DecksNumberCoverUp_Close";
  private bool m_cardCountPanelAnimOpen;
  private Color? m_originalEditTextColor;
  private Color? m_originalEditIconColor;
  private TavernBrawlMission m_currentMission;
  private TavernBrawlStatus m_currentlyShowingMode;
  private bool m_firstTimeIntroductionPopupShowing;
  private BannerPopup m_firstTimeIntroBanner;
  private Actor m_chosenHero;
  private Notification m_expoThankQuote;
  private AssetLoadingHelper m_assetLoadingHelper;
  private AdventureDungeonCrawlDisplay m_dungeonCrawlDisplay;
  private DungeonCrawlServices m_dungeonCrawlServices;
  private AdventureDefCache m_adventureDefCache = new AdventureDefCache(false);
  private AdventureWingDefCache m_adventureWingDefCache = new AdventureWingDefCache(false);
  private bool m_rewardChestDeprecated;
  private bool m_tavernBrawlHasEndedDialogActive;
  private static readonly PlatformDependentValue<string> DEFAULT_CHALKBOARD_TEXTURE_NAME_NO_DECK = new PlatformDependentValue<string>(PlatformCategory.Screen)
  {
    PC = "TavernBrawl_Chalkboard_Default_NoBorders.psd:556aa8938a98460498f590d2458e88b2",
    Phone = "TavernBrawl_Chalkboard_Default_phone.psd:c8421199aaf31fc4da69869c716fcf98"
  };
  private static readonly PlatformDependentValue<string> DEFAULT_CHALKBOARD_TEXTURE_NAME_WITH_DECK = new PlatformDependentValue<string>(PlatformCategory.Screen)
  {
    PC = "TavernBrawl_Chalkboard_Default_Borders.psd:e61e732d5bdd27e408e21fd873c99aa0",
    Phone = "TavernBrawl_Chalkboard_Default_phone.psd:c8421199aaf31fc4da69869c716fcf98"
  };
  private static readonly PlatformDependentValue<UnityEngine.Vector2> DEFAULT_CHALKBOARD_TEXTURE_OFFSET_NO_DECK = new PlatformDependentValue<UnityEngine.Vector2>(PlatformCategory.Screen)
  {
    PC = UnityEngine.Vector2.zero,
    Phone = UnityEngine.Vector2.zero
  };
  private static readonly PlatformDependentValue<UnityEngine.Vector2> DEFAULT_CHALKBOARD_TEXTURE_OFFSET_WITH_DECK = new PlatformDependentValue<UnityEngine.Vector2>(PlatformCategory.Screen)
  {
    PC = UnityEngine.Vector2.zero,
    Phone = new UnityEngine.Vector2(0.0f, -0.389f)
  };
  private static readonly AssetReference HEROIC_BRAWL_DIFFICULTY_WARNING_POPUP = new AssetReference("NewPopUp_HeroicBrawl.prefab:cac9ec2e7b497e641a02a03f65609486");
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    TavernBrawlDisplay.s_instance = this;
    this.transform.localScale = Vector3.one;
    this.m_currentMission = TavernBrawlManager.Get().CurrentMission();
    this.m_assetLoadingHelper = new AssetLoadingHelper();
    this.m_assetLoadingHelper.AssetLoadingComplete += new EventHandler(this.OnAssetLoadingComplete);
    this.Awake_InitializeRewardDisplay();
    this.SetupUniversalButtons();
    this.RegisterListeners();
    this.SetUIForFriendlyChallenge(FriendChallengeMgr.Get().IsChallengeTavernBrawl() && !FiresideGatheringManager.Get().InBrawlMode());
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Start()
  {
    this.m_tavernBrawlTray.ToggleTraySlider(true, animate: false);
    this.m_rewardChestDeprecated = false;
    if (PresenceMgr.Get().CurrentStatus != Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING)
    {
      Global.PresenceStatus presenceStatus = this.m_currentMission.BrawlType == BrawlType.BRAWL_TYPE_TAVERN_BRAWL ? Global.PresenceStatus.TAVERN_BRAWL_SCREEN : Global.PresenceStatus.FIRESIDE_BRAWL_SCREEN;
      PresenceMgr.Get().SetStatus((Enum) presenceStatus);
    }
    this.StartCoroutine(this.RefreshUIWhenReady());
    if (this.m_currentMission == null)
      return;
    MusicPlaylistType type = this.m_currentMission.brawlMode == TavernBrawlMode.TB_MODE_HEROIC ? MusicPlaylistType.UI_HeroicBrawl : MusicPlaylistType.UI_TavernBrawl;
    MusicManager.Get().StartPlaylist(type);
    NarrativeManager.Get().OnTavernBrawlEntered();
    this.InitExpoDemoMode();
  }

  private IEnumerator RefreshUIWhenReady()
  {
    while (TavernBrawlManager.Get() == null)
      yield return (object) null;
    TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
    if (tavernBrawlMission != null && tavernBrawlMission.canCreateDeck && !(bool) UniversalInputManager.UsePhoneUI)
    {
      while ((UnityEngine.Object) CollectionDeckTray.Get() == (UnityEngine.Object) null)
        yield return (object) null;
    }
    this.RefreshStateBasedUI(false);
    this.RefreshDataBasedUI(this.m_wipeAnimStartDelay);
  }

  private void OnDestroy()
  {
    this.HideDemoQuotes();
    this.UnregisterListeners();
    TavernBrawlDisplay.s_instance = (TavernBrawlDisplay) null;
  }

  public static TavernBrawlDisplay Get() => TavernBrawlDisplay.s_instance;

  public void Unload()
  {
    if (FriendChallengeMgr.Get().IsChallengeTavernBrawl() && !SceneMgr.Get().IsInGame() && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FRIENDLY) && (!FriendChallengeMgr.Get().DidReceiveChallenge() || FriendChallengeMgr.Get().DidChallengeeAccept()))
      FriendChallengeMgr.Get().CancelChallenge();
    if (!this.IsInDeckEditMode())
      return;
    Navigation.Pop();
  }

  public void RefreshDataBasedUI(float animDelay = 0.0f)
  {
    if (this.m_currentlyShowingMode == TavernBrawlStatus.TB_STATUS_IN_REWARDS || this.m_tavernBrawlHasEndedDialogActive)
      return;
    this.RefreshTavernBrawlInfo(animDelay);
    if (this.m_currentMission == null)
      return;
    if (this.m_currentMission.BrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING && !FiresideGatheringManager.Get().IsCheckedIn)
    {
      if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        return;
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
    else
    {
      this.UpdateRecordUI();
      if (this.m_currentMission.brawlMode == TavernBrawlMode.TB_MODE_HEROIC && !this.m_firstTimeIntroductionPopupShowing && !Options.Get().GetBool(Option.HAS_SEEN_HEROIC_BRAWL, false) && UserAttentionManager.CanShowAttentionGrabber("TavernBrawlDisplay.RefreshDataBasedUI:" + (object) Option.HAS_SEEN_HEROIC_BRAWL))
      {
        this.m_firstTimeIntroductionPopupShowing = true;
        this.StartCoroutine(this.DoFirstTimeHeroicIntro());
      }
      else
      {
        if (this.m_firstTimeIntroductionPopupShowing)
          return;
        TavernBrawlStatus playerStatus = TavernBrawlManager.Get().PlayerStatus;
        if (playerStatus != TavernBrawlStatus.TB_STATUS_TICKET_REQUIRED)
          StoreManager.Get().HideStore(ShopType.TAVERN_BRAWL_STORE);
        switch (playerStatus - 1)
        {
          case TavernBrawlStatus.TB_STATUS_INVALID:
            this.StartCoroutine(this.ShowPurchaseScreen());
            break;
          case TavernBrawlStatus.TB_STATUS_ACTIVE:
            this.StartCoroutine(this.ShowRewardsScreen());
            break;
          default:
            if (playerStatus != TavernBrawlStatus.TB_STATUS_ACTIVE && this.m_currentMission != null && this.m_currentMission.IsSessionBased)
            {
              Debug.LogErrorFormat("TavernBrawlDisplay.UpdateDisplayState(): don't know how to handle currentStatus={0}. Kicking to HUB", (object) playerStatus);
              if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
                SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
              AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
              if (this.m_currentMission.brawlMode == TavernBrawlMode.TB_MODE_HEROIC)
              {
                info.m_headerText = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_ERROR_TITLE");
                info.m_text = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_ERROR");
              }
              else
              {
                info.m_headerText = GameStrings.Get("GLUE_BRAWLISEUM_SESSION_ERROR_TITLE");
                info.m_text = GameStrings.Get("GLUE_BRAWLISEUM_SESSION_ERROR");
              }
              info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => TavernBrawlManager.Get().RefreshServerData());
              info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
              info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
              DialogManager.Get().ShowPopup(info);
              break;
            }
            this.ShowActiveScreen(animDelay);
            break;
        }
      }
    }
  }

  public bool IsInDeckEditMode() => this.m_deckBeingEdited > 0L;

  public bool IsInRewards() => this.m_currentlyShowingMode == TavernBrawlStatus.TB_STATUS_IN_REWARDS;

  public bool BackFromDeckEdit(bool animate)
  {
    if (!this.IsInDeckEditMode())
      return false;
    if (animate)
      PresenceMgr.Get().SetPrevStatus();
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() != CollectionUtils.ViewMode.CARDS)
    {
      if (TavernBrawlManager.Get().CurrentDeck() == null)
        CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.CARDS);
      else
        (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as CollectionPageManager).JumpToCollectionClassPage(TavernBrawlManager.Get().CurrentDeck().GetClass());
    }
    this.m_tavernBrawlTray.ToggleTraySlider(true, animate: animate);
    this.RefreshStateBasedUI(animate);
    this.m_deckBeingEdited = 0L;
    BnetBar.Get().RefreshCurrency();
    FriendChallengeMgr.Get().UpdateMyAvailability();
    this.UpdateEditOrCreate();
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_editDeckButton.SetText(GameStrings.Get("GLUE_EDIT"));
      if ((UnityEngine.Object) this.m_editIcon != (UnityEngine.Object) null)
        this.m_editIcon.SetActive(true);
      if ((UnityEngine.Object) this.m_deleteIcon != (UnityEngine.Object) null)
        this.m_deleteIcon.SetActive(false);
    }
    CollectionDeckTray.Get().ExitEditDeckModeForTavernBrawl();
    return true;
  }

  public static bool IsTavernBrawlOpen() => SceneMgr.Get().IsInTavernBrawlMode() && !((UnityEngine.Object) TavernBrawlDisplay.s_instance == (UnityEngine.Object) null);

  public static bool IsTavernBrawlEditing() => TavernBrawlDisplay.IsTavernBrawlOpen() && TavernBrawlDisplay.s_instance.IsInDeckEditMode();

  public static bool IsTavernBrawlViewing() => TavernBrawlDisplay.IsTavernBrawlOpen() && !TavernBrawlDisplay.s_instance.IsInDeckEditMode();

  public void ValidateDeck()
  {
    if (this.m_currentMission == null)
    {
      this.DisablePlayButton();
    }
    else
    {
      if (!this.m_currentMission.canCreateDeck)
        return;
      if (TavernBrawlManager.Get().HasValidDeckForCurrent())
      {
        if (TavernBrawlManager.Get().PlayerStatus == TavernBrawlStatus.TB_STATUS_ACTIVE)
        {
          if ((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null)
            this.m_playButton.Enable();
          this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(true);
        }
        if (!((UnityEngine.Object) this.m_editDeckHighlight != (UnityEngine.Object) null))
          return;
        this.m_editDeckHighlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
      }
      else
      {
        this.DisablePlayButton();
        if (!((UnityEngine.Object) this.m_editDeckHighlight != (UnityEngine.Object) null))
          return;
        this.m_editDeckHighlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
      }
    }
  }

  public void EnablePlayButton()
  {
    if (this.m_currentMission == null || this.m_currentMission.canCreateDeck)
    {
      this.ValidateDeck();
    }
    else
    {
      if ((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null)
        this.m_playButton.Enable();
      if (!((UnityEngine.Object) this.m_FiresideGatheringPlayButtonLantern != (UnityEngine.Object) null))
        return;
      this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(true);
    }
  }

  private void DisablePlayButton()
  {
    if ((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null)
      this.m_playButton.Disable();
    if (!((UnityEngine.Object) this.m_FiresideGatheringPlayButtonLantern != (UnityEngine.Object) null))
      return;
    this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(false);
  }

  public void EnableBackButton(bool enable)
  {
    if (!((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null))
      return;
    this.m_backButton.SetEnabled(enable);
    this.m_backButton.Flip(enable);
  }

  public void OnHeroPickerClosed()
  {
    if ((UnityEngine.Object) this.m_dungeonCrawlDisplay != (UnityEngine.Object) null && this.m_dungeonCrawlServices != null)
    {
      this.m_dungeonCrawlDisplay.EnablePlayButton();
    }
    else
    {
      this.EnablePlayButton();
      this.EnableBackButton(true);
    }
  }

  public AdventureDef GetAdventureDef(AdventureDbId id) => this.m_adventureDefCache.GetDef(id);

  public AdventureWingDef GetAdventureWingDef(WingDbId id) => this.m_adventureWingDefCache.GetDef(id);

  private void RefreshTavernBrawlInfo(float animDelay)
  {
    this.UpdateEditOrCreate();
    this.m_currentMission = TavernBrawlManager.Get().CurrentMission();
    if (this.m_currentMission == null || this.m_currentMission.missionId < 0)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_TAVERN_BRAWL_HAS_ENDED_HEADER"),
        m_text = GameStrings.Get("GLUE_TAVERN_BRAWL_HAS_ENDED_TEXT"),
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_responseCallback = new AlertPopup.ResponseCallback(this.RefreshTavernBrawlInfo_ConfirmEnded),
        m_offset = new Vector3(0.0f, 104f, 0.0f),
        m_alertTextAlignment = UberText.AlignmentOptions.Center
      });
      this.m_tavernBrawlHasEndedDialogActive = true;
    }
    else
    {
      if (this.m_rewardChestDeprecated || this.m_currentMission.rewardType == PegasusShared.RewardType.REWARD_CHEST || this.m_currentMission.rewardType == PegasusShared.RewardType.REWARD_NONE || DemoMgr.Get().IsDemo())
        this.m_rewardChest.gameObject.SetActive(false);
      if (this.m_currentMission.IsSessionBased)
      {
        if ((UnityEngine.Object) this.m_sessionWinLocationBone != (UnityEngine.Object) null)
          this.m_winsBanner.transform.position = this.m_sessionWinLocationBone.transform.position;
        if ((UnityEngine.Object) this.m_lossMarks != (UnityEngine.Object) null)
        {
          this.m_lossesRoot.SetActive(true);
          this.m_lossMarks.Init(this.m_currentMission.maxLosses);
        }
        this.m_TavernBrawlHeadline.Text = this.m_currentMission.brawlMode != TavernBrawlMode.TB_MODE_HEROIC ? GameStrings.Get("GLOBAL_BRAWLISEUM") : GameStrings.Get("GLOBAL_HEROIC_BRAWL");
      }
      else
      {
        if ((UnityEngine.Object) this.m_normalWinLocationBone != (UnityEngine.Object) null)
          this.m_winsBanner.transform.position = this.m_normalWinLocationBone.transform.position;
        if ((UnityEngine.Object) this.m_lossMarks != (UnityEngine.Object) null)
          this.m_lossesRoot.SetActive(false);
        this.m_TavernBrawlHeadline.Text = GameStrings.Get("GLOBAL_TAVERN_BRAWL");
      }
      if (this.m_currentMission.BrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
        this.m_TavernBrawlHeadline.Text = GameStrings.Get("GLOBAL_FIRESIDE_BRAWL");
      if (DemoMgr.Get().IsExpoDemo())
      {
        string str = Vars.Key("Demo.Header").GetStr("");
        if (!string.IsNullOrEmpty(str))
          this.m_TavernBrawlHeadline.Text = str;
      }
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_currentMission.missionId);
      if (record != null)
      {
        this.m_chalkboardHeader.Text = (string) record.Name;
        this.m_chalkboardInfo.Text = (string) (!(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty((string) record.ShortDescription) ? record.Description : record.ShortDescription);
      }
      this.LoadChalkboardTexture();
      this.CancelInvoke("UpdateTimeText");
      this.InvokeRepeating("UpdateTimeText", 0.1f, 0.1f);
      this.UpdateTimeText();
    }
  }

  private void RefreshTavernBrawlInfo_ConfirmEnded(AlertPopup.Response response, object userData)
  {
    if ((UnityEngine.Object) TavernBrawlDisplay.s_instance == (UnityEngine.Object) null)
      return;
    Navigation.Clear();
    this.ExitScene();
  }

  private void SetUIForFriendlyChallenge(bool isTavernBrawlChallenge)
  {
    string key = "GLUE_BRAWL";
    bool flag1 = this.m_currentMission.BrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING;
    if (this.ShouldPlayButtonShowOpponentPickerTray())
      key = "GLUE_CHOOSE_OPPONENT";
    else if (flag1)
      key = "GLUE_BRAWL";
    else if (TavernBrawlManager.Get().SelectHeroBeforeMission())
      key = "GLUE_CHOOSE";
    else if (isTavernBrawlChallenge && !DemoMgr.Get().IsExpoDemo())
      key = "GLUE_BRAWL_FRIEND";
    this.m_playButton.SetText(GameStrings.Get(key));
    bool flag2 = true;
    bool flag3;
    if (this.m_rewardChestDeprecated)
    {
      flag3 = false;
    }
    else
    {
      if (isTavernBrawlChallenge)
      {
        flag2 = false;
        NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
        if (netObject != null && netObject.FriendWeekAllowsTavernBrawlRecordUpdate && SpecialEventManager.Get().IsEventActive(SpecialEventType.FRIEND_WEEK, false))
          flag2 = true;
      }
      flag3 = flag2 && this.m_currentMission.rewardType != PegasusShared.RewardType.REWARD_CHEST && this.m_currentMission.rewardType != PegasusShared.RewardType.REWARD_NONE;
      if (DemoMgr.Get().IsDemo())
        flag3 = false;
    }
    this.m_rewardChest.gameObject.SetActive(flag3);
    this.m_winsBanner.SetActive(!isTavernBrawlChallenge && !flag1);
    if ((UnityEngine.Object) this.m_lossMarks != (UnityEngine.Object) null)
      this.m_lossMarks.gameObject.SetActive(!isTavernBrawlChallenge);
    if (!((UnityEngine.Object) this.m_editDeckButton != (UnityEngine.Object) null))
      return;
    if (!this.m_originalEditTextColor.HasValue)
      this.m_originalEditTextColor = new Color?(this.m_editText.TextColor);
    if (isTavernBrawlChallenge)
    {
      this.m_editText.TextColor = this.m_disabledTextColor;
      this.m_editDeckButton.SetEnabled(false);
    }
    else
    {
      this.m_editText.TextColor = this.m_originalEditTextColor.Value;
      this.m_editDeckButton.SetEnabled(true);
    }
    if (!((UnityEngine.Object) this.m_editIcon != (UnityEngine.Object) null))
      return;
    Material material = RendererExtension.GetMaterial(this.m_editIcon.GetComponent<Renderer>());
    if (!this.m_originalEditIconColor.HasValue)
      this.m_originalEditIconColor = new Color?(material.color);
    if (isTavernBrawlChallenge)
      material.color = this.m_disabledTextColor;
    else
      material.color = this.m_originalEditIconColor.Value;
  }

  private void LoadChalkboardTexture()
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_currentMission.missionId);
    MeshRenderer component;
    if (!((UnityEngine.Object) this.m_chalkboard != (UnityEngine.Object) null) || !this.m_chalkboard.TryGetComponent<MeshRenderer>(out component) || !((UnityEngine.Object) RendererExtension.GetMaterial((Renderer) component) != (UnityEngine.Object) null))
      return;
    Material material = RendererExtension.GetMaterial((Renderer) component);
    string assetRef1 = (string) null;
    UnityEngine.Vector2 vector2 = UnityEngine.Vector2.zero;
    if (record != null)
    {
      if (!string.IsNullOrEmpty(record.ScriptObject))
      {
        AssetReference assetRef2 = new AssetReference(record.ScriptObject);
        using (AssetHandle<ScenarioData> assetHandle = AssetLoader.Get().LoadAsset<ScenarioData>(assetRef2))
        {
          if (assetHandle == null)
            Debug.LogErrorFormat("Pointing to {0} but unable to load.  Rebuilding RAD will fix.", (object) assetRef2.ToString());
          else if (PlatformSettings.Screen == ScreenCategory.Phone)
          {
            assetRef1 = assetHandle.Asset.m_Texture_Phone;
            vector2.y = assetHandle.Asset.m_Texture_Phone_offsetY;
          }
          else
            assetRef1 = assetHandle.Asset.m_Texture;
        }
      }
      if (string.IsNullOrEmpty(assetRef1))
      {
        assetRef1 = record.TbTexture;
        if (PlatformSettings.Screen == ScreenCategory.Phone)
        {
          assetRef1 = record.TbTexturePhone;
          vector2.y = (float) record.TbTexturePhoneOffsetY;
        }
      }
      this.m_chalkboardTexture = string.IsNullOrEmpty(assetRef1) ? (Texture) null : AssetLoader.Get().LoadTexture((AssetReference) assetRef1);
    }
    if ((UnityEngine.Object) this.m_chalkboardTexture == (UnityEngine.Object) null)
    {
      int num = this.m_currentMission.canCreateDeck ? 1 : 0;
      string assetRef3 = (string) (num != 0 ? TavernBrawlDisplay.DEFAULT_CHALKBOARD_TEXTURE_NAME_WITH_DECK : TavernBrawlDisplay.DEFAULT_CHALKBOARD_TEXTURE_NAME_NO_DECK);
      vector2 = (UnityEngine.Vector2) (num != 0 ? TavernBrawlDisplay.DEFAULT_CHALKBOARD_TEXTURE_OFFSET_WITH_DECK : TavernBrawlDisplay.DEFAULT_CHALKBOARD_TEXTURE_OFFSET_NO_DECK);
      this.m_chalkboardTexture = AssetLoader.Get().LoadTexture((AssetReference) assetRef3);
    }
    if (!((UnityEngine.Object) this.m_chalkboardTexture != (UnityEngine.Object) null))
      return;
    material.SetTexture("_TopTex", this.m_chalkboardTexture);
    material.SetTextureOffset("_MainTex", vector2);
  }

  private void UpdateChalkboardVisual(float animDelay)
  {
    if ((UnityEngine.Object) this.m_chalkboardTexture == (UnityEngine.Object) null)
      this.LoadChalkboardTexture();
    this.StartCoroutine(this.WaitThenPlayWipeAnim(this.m_doFirstSeenAnimations ? animDelay : 0.0f));
  }

  private void UpdateTimeText()
  {
    if (DemoMgr.Get().IsExpoDemo())
      return;
    if (this.m_currentMission != null && this.m_currentMission.BrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)
    {
      this.m_chalkboardEndInfo.Text = "";
    }
    else
    {
      string endingTimeText = TavernBrawlManager.Get().EndingTimeText;
      if (endingTimeText == null)
        this.CancelInvoke(nameof (UpdateTimeText));
      else
        this.m_chalkboardEndInfo.Text = endingTimeText;
    }
  }

  private void UpdateRecordUI()
  {
    this.m_numWins.Text = TavernBrawlManager.Get().GamesWon.ToString();
    if (this.m_currentMission.IsSessionBased)
    {
      this.m_lossMarks.SetNumMarked(TavernBrawlManager.Get().GamesLost);
    }
    else
    {
      if (TavernBrawlManager.Get().RewardProgress < this.m_currentMission.RewardTriggerQuota)
        return;
      RendererExtension.SetMaterial(this.m_rewardChest.GetComponent<Renderer>(), this.m_chestOpenMaterial);
      this.m_rewardHighlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
      this.m_rewardChest.SetEnabled(false);
    }
  }

  private IEnumerator DoFirstTimeHeroicIntro()
  {
    Box.Get().SetToIgnoreFullScreenEffects(true);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.DisablePlayButton();
    string text = GameStrings.Get("GLUE_HEROIC_BRAWL_INTRO");
    while (SceneMgr.Get().IsTransitioning())
      yield return (object) 0;
    if (!BannerManager.Get().ShowBanner((string) TavernBrawlDisplay.HEROIC_BRAWL_DIFFICULTY_WARNING_POPUP, (string) null, text, new BannerManager.DelOnCloseBanner(TavernBrawlDisplay.OnFirstTimeIntroClosed), new Action<BannerPopup>(TavernBrawlDisplay.OnFirstTimeIntroCreated)))
    {
      Log.TavernBrawl.PrintWarning("TavernBrawlManager.DoFirstTimeHeroicIntro: First time popup failed to show.");
      this.ExitScene();
    }
  }

  private static void OnFirstTimeIntroCreated(BannerPopup popup)
  {
    TavernBrawlDisplay tavernBrawlDisplay = TavernBrawlDisplay.Get();
    if ((UnityEngine.Object) tavernBrawlDisplay == (UnityEngine.Object) null)
      return;
    tavernBrawlDisplay.m_firstTimeIntroBanner = popup;
  }

  private static void OnFirstTimeIntroClosed()
  {
    Box.Get().SetToIgnoreFullScreenEffects(false);
    TavernBrawlDisplay tavernBrawlDisplay = TavernBrawlDisplay.Get();
    if ((UnityEngine.Object) tavernBrawlDisplay == (UnityEngine.Object) null)
      return;
    tavernBrawlDisplay.m_firstTimeIntroBanner = (BannerPopup) null;
    tavernBrawlDisplay.m_firstTimeIntroductionPopupShowing = false;
    Options.Get().SetBool(Option.HAS_SEEN_HEROIC_BRAWL, true);
    if (!SceneMgr.Get().IsInTavernBrawlMode())
      return;
    tavernBrawlDisplay.RefreshDataBasedUI();
  }

  private void RegisterListeners()
  {
    SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    CollectionManager.Get().RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
    CollectionManager.Get().RegisterDeckDeletedListener(new CollectionManager.DelOnDeckDeleted(this.OnDeckDeleted));
    CollectionManager.Get().RegisterDeckContentsListener(new CollectionManager.DelOnDeckContents(this.OnDeckContents));
    CollectionManager.Get().RegisterCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(this.OnCollectionChanged));
    FriendChallengeMgr.Get().AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    TavernBrawlManager.Get().OnTavernBrawlUpdated += new Action(this.OnTavernBrawlUpdated);
    FiresideGatheringManager.Get().OnLeaveFSG += new FiresideGatheringManager.CheckedOutOfFSGCallback(this.OnLeaveFSG);
    if (this.m_currentMission != null && this.m_currentMission.canEditDeck || SceneMgr.Get().GetMode() == SceneMgr.Mode.FIRESIDE_GATHERING)
      return;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
  }

  private void UnregisterListeners()
  {
    SceneMgr.UnregisterScenePreUnloadEventFromInstance(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager != null)
    {
      collectionManager.RemoveDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
      collectionManager.RemoveDeckDeletedListener(new CollectionManager.DelOnDeckDeleted(this.OnDeckDeleted));
      collectionManager.RemoveDeckContentsListener(new CollectionManager.DelOnDeckContents(this.OnDeckContents));
      collectionManager.RemoveCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(this.OnCollectionChanged));
    }
    FriendChallengeMgr.RemoveChangedListenerFromInstance(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    GameMgr.Get()?.UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    if (TavernBrawlManager.Get() != null)
      TavernBrawlManager.Get().OnTavernBrawlUpdated -= new Action(this.OnTavernBrawlUpdated);
    if (FiresideGatheringManager.Get() == null)
      return;
    FiresideGatheringManager.Get().OnLeaveFSG -= new FiresideGatheringManager.CheckedOutOfFSGCallback(this.OnLeaveFSG);
  }

  private void Start_ShowAttentionGrabbers()
  {
    if (this.m_currentMission == null)
      return;
    bool flag = UserAttentionManager.CanShowAttentionGrabber("TavernBrawlDisplay.Show");
    int tavernBrawlChalkboard = TavernBrawlManager.Get().LatestSeenTavernBrawlChalkboard;
    if (tavernBrawlChalkboard == 0)
    {
      this.m_doFirstSeenAnimations = true;
      if (flag && !NotificationManager.Get().HasSoundPlayedThisSession("VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27.prefab:094070b7fecad8548b0b8fdb02bde052") && this.m_currentMission.BrawlType == BrawlType.BRAWL_TYPE_TAVERN_BRAWL)
      {
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27"), "VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27.prefab:094070b7fecad8548b0b8fdb02bde052");
        NotificationManager.Get().ForceAddSoundToPlayedList("VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27.prefab:094070b7fecad8548b0b8fdb02bde052");
      }
    }
    else if (tavernBrawlChalkboard < this.m_currentMission.seasonId)
    {
      this.m_doFirstSeenAnimations = true;
      if (this.m_currentMission.BrawlType == BrawlType.BRAWL_TYPE_TAVERN_BRAWL)
      {
        int num = Options.Get().GetInt(Option.TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE);
        if (flag && !NotificationManager.Get().HasSoundPlayedThisSession("VO_INNKEEPER_TAVERNBRAWL_DESC2_30.prefab:498657df8d08bc1468bfd1ad9f74ccac") && num < 3)
        {
          NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_TAVERNBRAWL_DESC2_30"), "VO_INNKEEPER_TAVERNBRAWL_DESC2_30.prefab:498657df8d08bc1468bfd1ad9f74ccac");
          NotificationManager.Get().ForceAddSoundToPlayedList("VO_INNKEEPER_TAVERNBRAWL_DESC2_30.prefab:498657df8d08bc1468bfd1ad9f74ccac");
          int val = num + 1;
          Options.Get().SetInt(Option.TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE, val);
        }
      }
    }
    if (!flag || tavernBrawlChalkboard == this.m_currentMission.seasonId)
      return;
    TavernBrawlManager.Get().LatestSeenTavernBrawlChalkboard = this.m_currentMission.seasonId;
  }

  private void OnTavernBrawlUpdated()
  {
    this.m_currentMission = TavernBrawlManager.Get().CurrentMission();
    this.RefreshDataBasedUI();
  }

  private void OnLeaveFSG(FSGConfig fsg) => this.RefreshDataBasedUI();

  private IEnumerator ShowPurchaseScreen()
  {
    TavernBrawlDisplay tavernBrawlDisplay = this;
    if (TavernBrawlManager.Get().CurrentTavernBrawlSeasonNewSessionsClosedInSeconds <= 0L)
    {
      Log.TavernBrawl.Print("TavernBrawlManager.ShowPurchaseScreen: New sessions in this season closed! Kicking out of TB");
      StoreManager.Get().HideStore(ShopType.TAVERN_BRAWL_STORE);
      if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_HEROIC_BRAWL_SIGNUPS_CLOSED_TITLE"),
        m_text = GameStrings.Get("GLUE_HEROIC_BRAWL_SIGNUPS_CLOSED"),
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_alertTextAlignment = UberText.AlignmentOptions.Center
      });
    }
    else if (tavernBrawlDisplay.m_currentlyShowingMode != TavernBrawlStatus.TB_STATUS_TICKET_REQUIRED)
    {
      tavernBrawlDisplay.m_currentlyShowingMode = TavernBrawlStatus.TB_STATUS_TICKET_REQUIRED;
      if (!(bool) UniversalInputManager.UsePhoneUI)
        tavernBrawlDisplay.DisablePlayButton();
      while (SceneMgr.Get().IsTransitioning())
        yield return (object) 0;
      if (tavernBrawlDisplay.m_currentlyShowingMode == TavernBrawlStatus.TB_STATUS_TICKET_REQUIRED)
        StoreManager.Get().StartTavernBrawlTransaction(new Store.ExitCallback(tavernBrawlDisplay.OnStoreBackButtonPressed), false);
    }
  }

  private void ShowActiveScreen(float animDelay)
  {
    if (this.m_currentlyShowingMode == TavernBrawlStatus.TB_STATUS_ACTIVE)
      return;
    this.m_currentlyShowingMode = TavernBrawlStatus.TB_STATUS_ACTIVE;
    this.Start_ShowAttentionGrabbers();
    this.UpdateChalkboardVisual(animDelay);
    this.UpdateDeckUI(false);
    if (!this.m_currentMission.IsDungeonRun)
      return;
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_currentMission.missionId);
    AdventureDbId adventureId = (AdventureDbId) record.AdventureId;
    AdventureModeDbId adventureModeId = (AdventureModeDbId) record.ModeId;
    this.m_adventureDefCache.LoadDefForId(adventureId);
    this.m_adventureWingDefCache.LoadDefForId((WingDbId) record.WingId);
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY)
      DungeonCrawlUtil.ClearDungeonRunServerData(adventureId, adventureModeId);
    if (DungeonCrawlUtil.IsDungeonRunDataReady(adventureId, adventureModeId))
    {
      this.StartDungeonRunIfInProgress(adventureId, adventureModeId);
    }
    else
    {
      this.DisablePlayButton();
      DungeonCrawlUtil.LoadDungeonRunData(adventureId, adventureModeId, (DungeonCrawlUtil.DungeonRunDataLoadedCallback) (success =>
      {
        if ((UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
          return;
        this.EnablePlayButton();
        if (!success)
          return;
        AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventureId, (int) adventureModeId);
        if (adventureDataRecord != null)
          DungeonCrawlUtil.MigrateDungeonCrawlSubkeys((GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey, (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey);
        this.StartDungeonRunIfInProgress(adventureId, adventureModeId);
      }));
    }
  }

  private void StartDungeonRunIfInProgress(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    if (!DungeonCrawlUtil.IsDungeonRunInProgress(adventureId, modeId))
      return;
    this.StartDungeonRun();
  }

  private IEnumerator ShowRewardsScreen()
  {
    TavernBrawlDisplay tavernBrawlDisplay = this;
    if (tavernBrawlDisplay.m_currentlyShowingMode != TavernBrawlStatus.TB_STATUS_IN_REWARDS)
    {
      tavernBrawlDisplay.m_currentlyShowingMode = TavernBrawlStatus.TB_STATUS_IN_REWARDS;
      if (!(bool) UniversalInputManager.UsePhoneUI)
        tavernBrawlDisplay.DisablePlayButton();
      if (!TavernBrawlManager.Get().CurrentSession.HasChest)
      {
        Log.TavernBrawl.PrintError("TavernBrawlManager.ShowHeroicRewardsScreen: Server said we're in rewards but no rewards were specified!");
        if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
          SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
        if (tavernBrawlDisplay.m_currentMission != null && tavernBrawlDisplay.m_currentMission.brawlMode == TavernBrawlMode.TB_MODE_HEROIC)
        {
          info.m_headerText = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_ERROR_TITLE");
          info.m_text = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_ERROR");
        }
        else
        {
          info.m_headerText = GameStrings.Get("GLUE_BRAWLISEUM_SESSION_ERROR_TITLE");
          info.m_text = GameStrings.Get("GLUE_BRAWLISEUM_SESSION_ERROR");
        }
        info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
        info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
        DialogManager.Get().ShowPopup(info);
      }
      else
      {
        while (SceneMgr.Get().IsTransitioning())
          yield return (object) null;
        if ((UnityEngine.Object) tavernBrawlDisplay.m_PhoneDeckTrayView != (UnityEngine.Object) null)
          tavernBrawlDisplay.m_PhoneDeckTrayView.gameObject.GetComponent<SlidingTray>().HideTray();
        Transform rewardBone = TavernBrawlManager.Get().CurrentSeasonBrawlMode == TavernBrawlMode.TB_MODE_HEROIC ? tavernBrawlDisplay.m_rewardBoxesBone.transform : tavernBrawlDisplay.m_sessionRewardBoxesBone.transform;
        RewardUtils.ShowTavernBrawlRewards(TavernBrawlManager.Get().GamesWon, TavernBrawlManager.Get().CurrentSessionRewards, rewardBone, new Action(tavernBrawlDisplay.OnRewardsDone));
      }
    }
  }

  private void OnRewardsDone()
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
      return;
    Network.Get().AckTavernBrawlSessionRewards();
    this.OnOpenRewardsComplete();
  }

  public void OnOpenRewardsComplete() => this.ExitScene();

  private void ExitScene()
  {
    this.m_tavernBrawlTray.m_animateBounce = false;
    this.m_tavernBrawlTray.ShowTray();
    GameMgr.Get().CancelFindGame();
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.DisablePlayButton();
    this.EnableBackButton(false);
    StoreManager.Get().HideStore(ShopType.TAVERN_BRAWL_STORE);
    if (FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.NONE)
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAME_MODE, SceneMgr.TransitionHandlerType.NEXT_SCENE);
    else
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.FIRESIDE_GATHERING);
  }

  private void UpdateDeckUI(bool animate)
  {
    this.UpdateDeckPanels(animate);
    this.ValidateDeck();
  }

  private bool OnNavigateBack()
  {
    if ((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null && !this.m_backButton.IsEnabled())
      return false;
    this.ExitScene();
    return true;
  }

  private void OnBackButton()
  {
    if (!this.m_backButton.IsEnabled())
      return;
    Navigation.GoBack();
  }

  private void OnStoreBackButtonPressed(bool authorizationBackButtonPressed, object userData) => this.ExitScene();

  private void RefreshStateBasedUI(bool animate) => this.UpdateDeckUI(animate);

  private void UpdateEditOrCreate()
  {
    bool flag1 = this.m_currentMission != null && this.m_currentMission.canCreateDeck;
    int num1 = this.m_currentMission == null || !this.m_currentMission.canEditDeck ? 0 : (!TavernBrawlManager.Get().IsDeckLocked ? 1 : 0);
    bool flag2 = TavernBrawlManager.Get().HasCreatedDeck();
    bool isDeckLocked = TavernBrawlManager.Get().IsDeckLocked;
    int num2 = (bool) UniversalInputManager.UsePhoneUI & isDeckLocked ? 1 : 0;
    bool flag3 = flag1 && !flag2;
    bool flag4 = num2 != 0;
    int num3 = flag1 ? 1 : 0;
    bool flag5 = (num1 & num3 & (flag2 ? 1 : 0)) != 0 && !flag4;
    if ((UnityEngine.Object) this.m_viewDeckButton != (UnityEngine.Object) null)
      this.m_viewDeckButton.gameObject.SetActive(flag4);
    if ((UnityEngine.Object) this.m_editDeckButton != (UnityEngine.Object) null)
    {
      this.m_editDeckButton.gameObject.SetActive(flag5);
      if (TavernBrawlManager.Get().IsDeckLocked)
      {
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          this.m_PhoneDeckTrayView.Initialize();
          this.InitializeDeckTrayManaCurve();
          this.LoadAndPositionPhoneDeckTrayHeroCard();
        }
        else
        {
          CollectionDeckTray.Get().m_cardsContent.UpdateDeckCompleteHighlight();
          if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && GameMgr.Get().WasTavernBrawl() && TavernBrawlManager.Get().GamesWon + TavernBrawlManager.Get().GamesLost == 1)
            this.StartCoroutine(this.DoDeckTrayLockedAnimation());
          else
            this.ShowDeckTrayLocked();
        }
      }
      if ((UnityEngine.Object) this.m_editIcon != (UnityEngine.Object) null)
        this.m_editIcon.SetActive(true);
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if ((UnityEngine.Object) this.m_createDeckButton != (UnityEngine.Object) null)
        this.m_createDeckButton.gameObject.SetActive(flag3);
    }
    else
    {
      if ((UnityEngine.Object) this.m_panelWithCreateDeck != (UnityEngine.Object) null)
        this.m_panelWithCreateDeck.SetActive(flag3);
      if ((UnityEngine.Object) this.m_fullPanel != (UnityEngine.Object) null)
        this.m_fullPanel.SetActive(!flag3);
    }
    if (!((UnityEngine.Object) this.m_createDeckHighlight != (UnityEngine.Object) null))
      return;
    if (!this.m_createDeckHighlight.gameObject.activeInHierarchy & flag3)
      Debug.LogWarning((object) "Attempting to activate m_createDeckHighlight, but it is inactive! This will not behave correctly!");
    this.m_createDeckHighlight.ChangeState(flag3 ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.HIGHLIGHT_OFF);
  }

  private void LoadAndPositionPhoneDeckTrayHeroCard()
  {
    if ((UnityEngine.Object) this.m_chosenHero != (UnityEngine.Object) null)
      return;
    CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
    if (collectionDeck == null)
      Log.TavernBrawl.PrintError("TavernBrawlManager.LoadAndPositionPhoneDeckTrayHeroCard: No deck found but trying to load the deck tray!");
    else
      GameUtils.LoadAndPositionCardActor("Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d", collectionDeck.HeroCardID, CollectionManager.Get().GetHeroPremium(collectionDeck.GetClass()), new GameUtils.LoadActorCallback(this.OnHeroActorLoaded));
  }

  private void OnHeroActorLoaded(Actor actor)
  {
    this.m_chosenHero = actor;
    this.m_chosenHero.transform.parent = this.m_SocketHeroBone.transform;
    this.m_chosenHero.transform.localPosition = Vector3.zero;
    this.m_chosenHero.transform.localScale = Vector3.one;
  }

  private IEnumerator DoDeckTrayLockedAnimation()
  {
    while (SceneMgr.Get().IsTransitioning())
      yield return (object) 0;
    yield return (object) new WaitForSeconds(1.5f);
    this.ShowDeckTrayLocked();
  }

  private void ShowDeckTrayLocked()
  {
    this.m_LockedDeckTray.enabled = true;
    this.m_LockedDeckTooltipZone.GetComponent<BoxCollider>().enabled = true;
  }

  private void InitializeDeckTrayManaCurve()
  {
    CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
    if (collectionDeck == null)
      return;
    foreach (CollectionDeckSlot slot in collectionDeck.GetSlots())
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(slot.CardID);
      for (int index = 0; index < slot.Count; ++index)
        this.AddCardToManaCurve(entityDef);
    }
  }

  public void AddCardToManaCurve(EntityDef entityDef)
  {
    if ((UnityEngine.Object) this.m_ManaCurvePhone == (UnityEngine.Object) null)
      Debug.LogWarning((object) string.Format("TavernBrawlDisplay.AddCardToManaCurve({0}) - m_manaCurve is null", (object) entityDef));
    else
      this.m_ManaCurvePhone.AddCardToManaCurve(entityDef);
  }

  private void UpdateDeckPanels(bool animate = true) => this.UpdateDeckPanels(this.m_currentMission != null && this.m_currentMission.canCreateDeck && TavernBrawlManager.Get().HasCreatedDeck(), animate);

  private void UpdateDeckPanels(bool hasDeck, bool animate)
  {
    if ((UnityEngine.Object) this.m_cardListPanel != (UnityEngine.Object) null)
    {
      bool show = !hasDeck;
      if (animate && !show)
      {
        this.m_createDeckButton.gameObject.SetActive(false);
        this.m_createDeckHighlight.gameObject.SetActive(false);
      }
      else if (show)
      {
        this.m_createDeckButton.gameObject.SetActive(true);
        this.m_createDeckHighlight.gameObject.SetActive(true);
      }
      this.m_cardListPanel.ToggleTraySlider(show, animate: animate);
    }
    if (!((UnityEngine.Object) this.m_cardCountPanelAnim != (UnityEngine.Object) null) || this.m_cardCountPanelAnimOpen == hasDeck)
      return;
    this.m_cardCountPanelAnim.Play(hasDeck ? this.CARD_COUNT_PANEL_OPEN_ANIM : this.CARD_COUNT_PANEL_CLOSE_ANIM);
    this.m_cardCountPanelAnimOpen = hasDeck;
  }

  private void CreateDeck()
  {
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_DECKEDITOR);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.EnterSelectNewDeckHeroMode();
    this.HideChalkboardFX();
  }

  private void EditDeckButton_OnRelease(UIEvent e)
  {
    if (this.IsInDeckEditMode())
    {
      this.OnDeleteButtonPressed();
    }
    else
    {
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_DECKEDITOR);
      this.SwitchToEditDeckMode(TavernBrawlManager.Get().CurrentDeck(), false);
    }
  }

  private void ViewDeckButton_OnRelease(UIEvent e) => this.m_PhoneDeckTrayView.gameObject.GetComponent<SlidingTray>().ShowTray();

  private bool SwitchToEditDeckMode(CollectionDeck deck, bool isNewDeck)
  {
    if ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() == (UnityEngine.Object) null || deck == null)
      return false;
    this.m_tavernBrawlTray.HideTray();
    this.UpdateDeckPanels(true, true);
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_editDeckButton.gameObject.SetActive(this.m_currentMission.canEditDeck);
      this.m_editDeckButton.SetText(GameStrings.Get("GLUE_COLLECTION_DECK_DELETE"));
      if ((UnityEngine.Object) this.m_editIcon != (UnityEngine.Object) null)
        this.m_editIcon.SetActive(false);
      if ((UnityEngine.Object) this.m_deleteIcon != (UnityEngine.Object) null)
        this.m_deleteIcon.SetActive(true);
      this.m_editDeckHighlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    }
    this.m_deckBeingEdited = deck.ID;
    BnetBar.Get().RefreshCurrency();
    CollectionDeckTray.Get().EnterEditDeckModeForTavernBrawl(deck, isNewDeck);
    FriendChallengeMgr.Get().UpdateMyAvailability();
    return true;
  }

  private void ShowNonSessionRewardPreview(UIEvent e)
  {
    if (this.m_currentMission == null)
      return;
    switch (this.m_currentMission.rewardType)
    {
      case PegasusShared.RewardType.REWARD_BOOSTER_PACKS:
        if ((UnityEngine.Object) this.m_rewardObject == (UnityEngine.Object) null)
        {
          int rewardData2 = (int) this.m_currentMission.RewardData2;
          BoosterDbfRecord record = GameDbf.Booster.GetRecord(rewardData2);
          if (record == null)
          {
            Debug.LogErrorFormat("TavernBrawlDisplay.ShowReward() - no record found for booster {0}!", (object) rewardData2);
            return;
          }
          string packOpeningPrefab = record.PackOpeningPrefab;
          if (string.IsNullOrEmpty(packOpeningPrefab))
          {
            Debug.LogErrorFormat("TavernBrawlDisplay.ShowReward() - no prefab found for booster {0}!", (object) rewardData2);
            return;
          }
          GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) packOpeningPrefab, AssetLoadingOptions.IgnorePrefabPosition);
          if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
          {
            Debug.LogError((object) string.Format("TavernBrawlDisplay.ShowReward() - failed to load prefab {0} for booster {1}!", (object) packOpeningPrefab, (object) rewardData2));
            return;
          }
          this.m_rewardObject = gameObject;
          UnopenedPack component = gameObject.GetComponent<UnopenedPack>();
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          {
            Debug.LogError((object) string.Format("TavernBrawlDisplay.ShowReward() - No UnopenedPack script found on prefab {0} for booster {1}!", (object) packOpeningPrefab, (object) rewardData2));
            return;
          }
          GameUtils.SetParent(this.m_rewardObject, this.m_rewardContainer);
          component.SetBoosterId((int) this.m_currentMission.RewardData2);
          component.SetCount((int) this.m_currentMission.RewardData1);
          break;
        }
        break;
      case PegasusShared.RewardType.REWARD_CARD_BACK:
        if ((UnityEngine.Object) this.m_rewardObject == (UnityEngine.Object) null)
        {
          int rewardData1 = (int) this.m_currentMission.RewardData1;
          CardBackManager.LoadCardBackData loadCardBackData = CardBackManager.Get().LoadCardBackByIndex(rewardData1, shadowActive: true);
          if (loadCardBackData == null)
          {
            Debug.LogErrorFormat("TavernBrawlDisplay.ShowReward() - Could not load cardback ID {0}!", (object) rewardData1);
            return;
          }
          this.m_rewardObject = loadCardBackData.m_GameObject;
          GameUtils.SetParent(this.m_rewardObject, this.m_rewardContainer);
          this.m_rewardObject.transform.localScale = Vector3.one * 5.92f;
          break;
        }
        break;
      default:
        Debug.LogErrorFormat("Tavern Brawl reward type currently not supported! Add type {0} to TaverBrawlDisplay.ShowReward().", (object) this.m_currentMission.rewardType);
        return;
    }
    this.m_rewardsPreview.SetActive(true);
    iTween.Stop(this.m_rewardsPreview);
    iTween.ScaleTo(this.m_rewardsPreview, iTween.Hash((object) "scale", (object) this.m_rewardsScale, (object) "time", (object) 0.15f));
  }

  private void HideNonSessionRewardPreview(UIEvent e)
  {
    iTween.Stop(this.m_rewardsPreview);
    iTween.ScaleTo(this.m_rewardsPreview, iTween.Hash((object) "scale", (object) (Vector3.one * 0.01f), (object) "time", (object) 0.15f, (object) "oncomplete", (object) (Action<object>) (o => this.m_rewardsPreview.SetActive(false))));
  }

  private void StartDungeonRun()
  {
    this.DisablePlayButton();
    ScenarioDbfRecord scen = GameDbf.Scenario.GetRecord(this.m_currentMission.missionId);
    if (scen == null)
      return;
    DungeonCrawlUtil.LoadDungeonRunPrefab((DungeonCrawlUtil.DungeonRunLoadCallback) (go =>
    {
      DungeonCrawlServices dungeonCrawlServices = DungeonCrawlUtil.CreateTavernBrawlDungeonCrawlServices((AdventureDbId) scen.AdventureId, (AdventureModeDbId) scen.ModeId, this.m_assetLoadingHelper);
      AdventureDungeonCrawlDisplay component = go.GetComponent<AdventureDungeonCrawlDisplay>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      this.m_dungeonCrawlServices = dungeonCrawlServices;
      this.m_dungeonCrawlDisplay = component;
      GameUtils.SetParent(go, (Component) this.transform);
      go.transform.position = new Vector3(-500f, 0.0f, 0.0f);
      component.StartRun(dungeonCrawlServices);
    }));
  }

  private void PlayButton_OnRelease(UIEvent e)
  {
    if (!Network.IsLoggedIn())
      DialogManager.Get().ShowReconnectHelperDialog((Action) (() => this.PlayButton_OnRelease(e)));
    else if (this.m_currentMission == null)
    {
      this.RefreshDataBasedUI();
    }
    else
    {
      if (SetRotationManager.Get().CheckForSetRotationRollover() || PlayerMigrationManager.Get() != null && PlayerMigrationManager.Get().CheckForPlayerMigrationRequired())
        return;
      if (this.ShouldPlayButtonShowOpponentPickerTray())
      {
        FiresideGatheringDisplay.Get().ShowOpponentPickerTray(new Action(this.EnablePlayButton));
        this.DisablePlayButton();
      }
      else if (this.m_currentMission.IsSessionBased && this.m_currentMission.canEditDeck && !TavernBrawlManager.Get().IsDeckLocked)
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_HEROIC_BRAWL_PLAY_CONFIRMATION_TITLE"),
          m_text = GameStrings.Get("GLUE_HEROIC_BRAWL_PLAY_CONFIRMATION"),
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_confirmText = GameStrings.Get("GLUE_HEROIC_BRAWL_PLAY_CONFIRMATION_OK"),
          m_cancelText = GameStrings.Get("GLUE_HEROIC_BRAWL_PLAY_CONFIRMATION_CANCEL"),
          m_responseCallback = new AlertPopup.ResponseCallback(this.OnPlayButtonConfirmationResponse),
          m_alertTextAlignment = UberText.AlignmentOptions.Center
        });
      else
        this.OnPlayButtonExecute();
    }
  }

  private void OnPlayButtonConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    this.OnPlayButtonExecute();
  }

  private void OnPlayButtonExecute()
  {
    if (this.m_currentMission.IsDungeonRun)
      this.StartDungeonRun();
    else if (TavernBrawlManager.Get().SelectHeroBeforeMission())
    {
      bool flag = false;
      int num = (UnityEngine.Object) GuestHeroPickerDisplay.Get() != (UnityEngine.Object) null ? 1 : ((UnityEngine.Object) HeroPickerDisplay.Get() != (UnityEngine.Object) null ? 1 : 0);
      if (num == 0)
        flag = AssetLoader.Get().InstantiatePrefab((AssetReference) this.GetHeroPickerAssetStr(this.m_currentMission.missionId), (PrefabCallback<GameObject>) ((name, go, data) =>
        {
          if ((UnityEngine.Object) go == (UnityEngine.Object) null)
          {
            Debug.LogError((object) "Failed to load hero picker.");
          }
          else
          {
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_DECKEDITOR);
            this.HideChalkboardFX();
          }
        }));
      if (!flag)
        Log.All.PrintWarning("Failed to load hero picker.");
      if (num != 0)
      {
        Log.All.PrintWarning("Attempting to load HeroPickerDisplay a second time!");
        return;
      }
    }
    else if (this.m_currentMission.canCreateDeck)
    {
      if (TavernBrawlManager.Get().HasValidDeckForCurrent())
      {
        CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
        if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
        {
          FriendChallengeMgr.Get().SelectDeck(collectionDeck.ID);
          FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_TAVERN_BRAWL_OPPONENT_WAITING_READY", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
        }
        else
          TavernBrawlManager.Get().StartGame(collectionDeck.ID);
      }
      else
      {
        Debug.LogError((object) "Attempting to start a Tavern Brawl game without having a valid deck!");
        return;
      }
    }
    else if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
    {
      FriendChallengeMgr.Get().SkipDeckSelection();
      FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_TAVERN_BRAWL_OPPONENT_WAITING_READY", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
    }
    else
      TavernBrawlManager.Get().StartGame();
    this.DisablePlayButton();
    this.EnableBackButton(false);
  }

  private string GetHeroPickerAssetStr(int scenarioId) => GameUtils.GetScenarioGuestHeroes(scenarioId).Count > 0 ? "GuestHeroPicker.prefab:3ecbc18da1de3ef4fa30532f90b20e59" : "HeroPicker.prefab:59e2d2f899d09f4488a194df18967915";

  private bool ShouldPlayButtonShowOpponentPickerTray() => (this.m_currentMission.BrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING || SceneMgr.Get().GetMode() == SceneMgr.Mode.FIRESIDE_GATHERING) && !GameUtils.IsAIMission(this.m_currentMission.missionId) && !TavernBrawlManager.Get().SelectHeroBeforeMission();

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    int num;
    switch (prevMode)
    {
      case SceneMgr.Mode.TAVERN_BRAWL:
        num = 1;
        break;
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        num = FiresideGatheringManager.Get().InBrawlMode() ? 1 : 0;
        break;
      default:
        num = 0;
        break;
    }
    if (num == 0 || !((UnityEngine.Object) this.m_firstTimeIntroBanner != (UnityEngine.Object) null))
      return;
    this.m_firstTimeIntroBanner.Close();
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        this.HandleGameStartupFailure();
        break;
      case FindGameState.SERVER_GAME_STARTED:
        FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
        break;
    }
    return false;
  }

  private void HandleGameStartupFailure()
  {
    if (TavernBrawlManager.Get().SelectHeroBeforeMission())
      return;
    this.EnablePlayButton();
    this.EnableBackButton(true);
  }

  private void OnDeleteButtonPressed()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_HEADER"),
      m_text = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_DESC"),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnDeleteButtonConfirmationResponse)
    };
    info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
    DialogManager.Get().ShowPopup(info);
  }

  private void OnDeleteButtonConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    CollectionDeckTray.Get().DeleteEditingDeck();
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.OnDoneEditingDeck();
  }

  private void OnDeckCreated(long deckID, string name)
  {
    CollectionDeck deck = TavernBrawlManager.Get().CurrentDeck();
    if (deck == null || deckID != deck.ID)
      return;
    this.SwitchToEditDeckMode(deck, true);
  }

  private void OnDeckDeleted(CollectionDeck removedDeck)
  {
    if (removedDeck.ID != this.m_deckBeingEdited || !TavernBrawlDisplay.IsTavernBrawlOpen())
      return;
    this.StartCoroutine(this.WaitThenCreateDeck());
  }

  private IEnumerator WaitThenPlayWipeAnim(float waitTime)
  {
    yield return (object) new WaitForSeconds(waitTime);
    if ((UnityEngine.Object) this.m_chalkboard != (UnityEngine.Object) null && TavernBrawlManager.Get().IsCurrentBrawlTypeActive && TavernBrawlManager.Get().IsCurrentBrawlAllDataReady)
      this.m_chalkboard.GetComponent<PlayMakerFSM>().SendEvent(this.m_doFirstSeenAnimations ? "Wipe" : "QuickShow");
    while (NotificationManager.Get().IsQuotePlaying)
      yield return (object) null;
    if (this.m_doFirstSeenAnimations && this.m_currentMission.FirstTimeSeenCharacterDialogID > 0)
      NarrativeManager.Get().PushDialogSequence(this.m_currentMission.FirstTimeSeenCharacterDialogSequence);
    yield return (object) new WaitForSeconds(1f);
  }

  private IEnumerator WaitThenCreateDeck()
  {
    yield return (object) new WaitForEndOfFrame();
    this.CreateDeck();
    yield return (object) new WaitForSeconds(0.4f);
    this.BackFromDeckEdit(false);
  }

  private void OnCollectionChanged()
  {
    if (!TavernBrawlDisplay.IsTavernBrawlViewing())
      return;
    this.ValidateDeck();
  }

  private void OnDeckContents(long deckID)
  {
    CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
    if (collectionDeck == null || deckID != collectionDeck.ID || !TavernBrawlDisplay.IsTavernBrawlOpen())
      return;
    this.ValidateDeck();
  }

  private void Awake_InitializeRewardDisplay()
  {
    if (this.m_rewardChestDeprecated)
      return;
    PegasusShared.RewardType rewardType = this.m_currentMission == null ? PegasusShared.RewardType.REWARD_UNKNOWN : this.m_currentMission.rewardType;
    RewardTrigger rewardTrigger = this.m_currentMission == null ? RewardTrigger.REWARD_TRIGGER_UNKNOWN : this.m_currentMission.rewardTrigger;
    string key = (string) null;
    long num = 1;
    switch (rewardType)
    {
      case PegasusShared.RewardType.REWARD_BOOSTER_PACKS:
        num = this.m_currentMission.RewardData1;
        key = rewardTrigger == RewardTrigger.REWARD_TRIGGER_WIN_GAME || rewardTrigger != RewardTrigger.REWARD_TRIGGER_FINISH_GAME ? "GLUE_TAVERN_BRAWL_REWARD_DESC" : "GLUE_TAVERN_BRAWL_REWARD_DESC_FINISH";
        break;
      case PegasusShared.RewardType.REWARD_CARD_BACK:
        key = rewardTrigger == RewardTrigger.REWARD_TRIGGER_WIN_GAME || rewardTrigger != RewardTrigger.REWARD_TRIGGER_FINISH_GAME ? "GLUE_TAVERN_BRAWL_REWARD_DESC_CARDBACK" : "GLUE_TAVERN_BRAWL_REWARD_DESC_FINISH_CARDBACK";
        break;
    }
    if (key != null)
    {
      if (this.m_currentMission.RewardTriggerQuota != 1)
        key += "_QUOTA";
      if (num != 1L)
        key += "_MULTIPLE_PACKS";
      this.m_rewardsText.Text = GameStrings.Format(key, (object) num, (object) this.m_currentMission.RewardTriggerQuota);
    }
    if ((UnityEngine.Object) this.m_rewardOffClickCatcher != (UnityEngine.Object) null)
    {
      this.m_rewardChest.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ShowNonSessionRewardPreview));
      this.m_rewardOffClickCatcher.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.HideNonSessionRewardPreview));
    }
    else
    {
      this.m_rewardChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowNonSessionRewardPreview));
      this.m_rewardChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideNonSessionRewardPreview));
    }
    this.m_rewardsScale = this.m_rewardsPreview.transform.localScale;
    this.m_rewardsPreview.transform.localScale = Vector3.one * 0.01f;
    if (this.m_currentMission == null || TavernBrawlManager.Get().RewardProgress >= this.m_currentMission.RewardTriggerQuota)
      return;
    this.m_rewardHighlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }

  private void SetupUniversalButtons()
  {
    if ((UnityEngine.Object) this.m_editDeckButton != (UnityEngine.Object) null)
      this.m_editDeckButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.EditDeckButton_OnRelease));
    if ((UnityEngine.Object) this.m_createDeckButton != (UnityEngine.Object) null)
      this.m_createDeckButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.CreateDeck()));
    if ((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null)
      this.m_backButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnBackButton()));
    if ((UnityEngine.Object) this.m_LockedDeckTooltipTrigger != (UnityEngine.Object) null && (UnityEngine.Object) this.m_LockedDeckTooltipZone != (UnityEngine.Object) null)
    {
      this.m_LockedDeckTooltipTrigger.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnLockedTooltipRollover));
      this.m_LockedDeckTooltipTrigger.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnLockedTooltipRollout));
    }
    if ((UnityEngine.Object) this.m_viewDeckButton != (UnityEngine.Object) null)
      this.m_viewDeckButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ViewDeckButton_OnRelease));
    this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayButton_OnRelease));
    this.m_FiresideGatheringPlayButtonLantern.gameObject.SetActive(FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL);
    this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(true);
  }

  private void OnLockedTooltipRollover(UIEvent e)
  {
    if (!TavernBrawlManager.Get().IsDeckLocked)
      return;
    this.m_LockedDeckTooltipZone.ShowLayerTooltip(GameStrings.Get("GLUE_LOCKED_DECK_TOOLTIP_TITLE"), GameStrings.Get("GLUE_LOCKED_DECK_TOOLTIP"));
  }

  private void OnLockedTooltipRollout(UIEvent e) => this.m_LockedDeckTooltipZone.HideTooltip();

  private void DoDungeonRunTransition()
  {
    Vector3 localPosition1 = this.transform.localPosition;
    Vector3 localPosition2 = this.transform.localPosition;
    localPosition2.x -= this.m_transitionStartingOffset;
    this.m_dungeonCrawlDisplay.gameObject.transform.localPosition = localPosition2;
    Transform transform = this.transform.Find("Root");
    GameObject rootGo = transform.gameObject;
    iTween.MoveTo(this.m_dungeonCrawlDisplay.gameObject, iTween.Hash((object) "islocal", (object) true, (object) "position", (object) localPosition1, (object) "time", (object) this.m_transitionTime, (object) "easeType", (object) "easeOutBounce", (object) "oncomplete", (object) (Action<object>) (e =>
    {
      this.m_dungeonCrawlServices.SubsceneController.OnTransitionComplete();
      rootGo.SetActive(false);
    }), (object) "oncompletetarget", (object) this.gameObject));
    if (!string.IsNullOrEmpty(this.m_SlideInSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_SlideInSound);
    Vector3 localPosition3 = transform.localPosition;
    localPosition3.y -= this.m_rootDropHeight;
    Hashtable args = iTween.Hash((object) "islocal", (object) true, (object) "position", (object) localPosition3, (object) "time", (object) (float) ((double) this.m_transitionTime / 2.0), (object) "easeType", (object) "easeOutBounce", (object) "oncomplete", (object) (Action<object>) (e => { }), (object) "oncompletetarget", (object) rootGo);
    iTween.MoveTo(rootGo, args);
  }

  private void OnAssetLoadingComplete(object sender, EventArgs args)
  {
    if (this.m_dungeonCrawlServices == null || !((UnityEngine.Object) this.m_dungeonCrawlDisplay != (UnityEngine.Object) null))
      return;
    this.DoDungeonRunTransition();
  }

  private void OnFriendChallengeWaitingForOpponentDialogResponse(
    AlertPopup.Response response,
    object userData)
  {
    if (response != AlertPopup.Response.CANCEL || FriendChallengeMgr.Get().AmIInGameState())
      return;
    FriendChallengeMgr.Get().DeselectDeckOrHero();
    FriendlyChallengeHelper.Get().StopWaitingForFriendChallenge();
    if (TavernBrawlManager.Get().SelectHeroBeforeMission())
      return;
    this.EnablePlayButton();
  }

  private void OnFriendChallengeChanged(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData,
    object userData)
  {
    switch (challengeEvent)
    {
      case FriendChallengeEvent.I_RESCINDED_CHALLENGE:
      case FriendChallengeEvent.OPPONENT_DECLINED_CHALLENGE:
      case FriendChallengeEvent.OPPONENT_RESCINDED_CHALLENGE:
        this.SetUIForFriendlyChallenge(false);
        break;
      case FriendChallengeEvent.OPPONENT_ACCEPTED_CHALLENGE:
      case FriendChallengeEvent.I_ACCEPTED_CHALLENGE:
        this.SetUIForFriendlyChallenge(true);
        break;
      case FriendChallengeEvent.SELECTED_DECK_OR_HERO:
        if (player == BnetPresenceMgr.Get().GetMyPlayer() || !FriendChallengeMgr.Get().DidISelectDeckOrHero())
          break;
        FriendlyChallengeHelper.Get().HideFriendChallengeWaitingForOpponentDialog();
        break;
      case FriendChallengeEvent.DESELECTED_DECK_OR_HERO:
        if (player == BnetPresenceMgr.Get().GetMyPlayer())
          break;
        if (!TavernBrawlManager.Get().SelectHeroBeforeMission())
        {
          this.EnablePlayButton();
          break;
        }
        if (!FriendChallengeMgr.Get().DidISelectDeckOrHero())
          break;
        FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_DECK", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
        break;
      case FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE:
      case FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS:
      case FriendChallengeEvent.QUEUE_CANCELED:
        this.SetUIForFriendlyChallenge(false);
        FriendlyChallengeHelper.Get().StopWaitingForFriendChallenge();
        break;
    }
  }

  private void InitExpoDemoMode()
  {
    if (!DemoMgr.Get().IsExpoDemo())
      return;
    if ((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null)
    {
      this.m_backButton.Flip(false);
      this.m_backButton.SetEnabled(false);
    }
    this.m_chalkboardEndInfo.gameObject.SetActive(false);
    this.StartCoroutine("ShowDemoQuotes");
  }

  private IEnumerator ShowDemoQuotes()
  {
    yield return (object) new WaitForSeconds(1f);
    string str = Vars.Key("Demo.ThankQuote").GetStr("");
    int num = Vars.Key("Demo.ThankQuoteMsTime").GetInt(0);
    string text = str.Replace("\\n", "\n");
    if ((string.IsNullOrEmpty(text) ? 0 : (num > 0 ? 1 : 0)) != 0)
    {
      this.m_expoThankQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(138.3f, NotificationManager.DEPTH, 58.7f), text, "", (float) num / 1000f);
      this.EnableClickBlocker(true);
      yield return (object) new WaitForSeconds((float) num / 1000f);
      this.EnableClickBlocker(false);
    }
  }

  private void EnableClickBlocker(bool enable)
  {
    if ((UnityEngine.Object) this.m_clickBlocker == (UnityEngine.Object) null)
      return;
    if (enable)
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective with
      {
        Time = 0.4f,
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
    if (!((UnityEngine.Object) this.m_expoThankQuote != (UnityEngine.Object) null))
      return;
    NotificationManager notificationManager = NotificationManager.Get();
    if ((UnityEngine.Object) notificationManager != (UnityEngine.Object) null)
      notificationManager.DestroyNotification(this.m_expoThankQuote, 0.0f);
    this.m_expoThankQuote = (Notification) null;
    this.m_screenEffectsHandle.StopEffect();
  }

  private void HideChalkboardFX() => this.m_chalkboardFX.SetActive(false);
}
