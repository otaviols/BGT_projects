using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone;
using Hearthstone.Progression;
using Hearthstone.Streaming;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GameMenu : ButtonListMenu, GameMenuInterface
{
  [CustomEditField(Sections = "Template Items")]
  public Vector3 m_ratingsObjectMinPadding = new Vector3(0.0f, 0.0f, -0.06f);
  public Transform m_menuBone;
  public Material m_redButtonMaterial;
  public string m_anchorForKoreanRatings;
  private static GameMenu s_instance;
  private GameMenuBase m_gameMenuBase;
  private UIBButton m_concedeButton;
  private UIBButton m_endGameButton;
  private UIBButton m_leaveButton;
  private UIBButton m_restartButton;
  private UIBButton m_quitButton;
  private UIBButton m_loginButton;
  private UIBButton m_optionsButton;
  private UIBButton m_downloadButton;
  private UIBButton m_signUpButton;
  private Notification m_loginButtonPopup;
  private bool m_hasSeenLoginTooltip;
  private BnetRegion m_AccountRegion;
  private GameObject m_ratingsObject;
  private Transform m_ratingsAnchor;
  private RegionSwitchMenuController m_regionSwitchMenuController = new RegionSwitchMenuController();
  private readonly Vector3 BUTTON_SCALE = 15f * Vector3.one;
  private readonly Vector3 BUTTON_SCALE_PHONE = 25f * Vector3.one;
  private int m_minTurnsForProgressAfterConcede;
  private int m_minHPForProgressAfterConcede;

  private IGameDownloadManager DownloadManager => GameDownloadManagerProvider.Get();

  protected override void Awake()
  {
    this.m_menuParent = this.m_menuBone;
    this.m_targetLayer = GameLayer.HighPriorityUI;
    base.Awake();
    GameMenu.s_instance = this;
    this.m_gameMenuBase = new GameMenuBase();
    this.m_gameMenuBase.m_showCallback = (GameMenuBase.ShowCallback) (() => this.Show(true));
    this.m_gameMenuBase.m_hideCallback = new GameMenuBase.HideCallback(((ButtonListMenu) this).Hide);
    this.LoadRatings();
    this.m_concedeButton = this.CreateMenuButton("ConcedeButton", "GLOBAL_CONCEDE", new UIEvent.Handler(this.ConcedeButtonPressed));
    ButtonListMenu.MakeButtonRed(this.m_concedeButton, this.m_redButtonMaterial);
    this.m_endGameButton = this.CreateMenuButton("EndGameButton", "GLOBAL_END_GAME", new UIEvent.Handler(this.ConcedeButtonPressed));
    ButtonListMenu.MakeButtonRed(this.m_endGameButton, this.m_redButtonMaterial);
    this.m_leaveButton = this.CreateMenuButton("LeaveButton", "GLOBAL_LEAVE_SPECTATOR_MODE", new UIEvent.Handler(this.LeaveButtonPressed));
    this.m_restartButton = this.CreateMenuButton("RestartButton", "GLOBAL_RESTART", new UIEvent.Handler(this.RestartButtonPressed));
    if ((bool) HearthstoneApplication.CanQuitGame)
      this.m_quitButton = this.CreateMenuButton("QuitButton", "GLOBAL_QUIT", new UIEvent.Handler(this.QuitButtonPressed));
    if (PlatformSettings.IsMobile())
      this.m_loginButton = this.CreateMenuButton("LogoutButton", Network.ShouldBeConnectedToAurora() ? "GLOBAL_SWITCH_ACCOUNT" : "GLOBAL_LOGIN", new UIEvent.Handler(this.LogoutButtonPressed));
    this.m_optionsButton = this.CreateMenuButton("OptionsButton", "GLOBAL_OPTIONS", new UIEvent.Handler(this.OptionsButtonPressed));
    if ((Object) this.m_menu.m_templateDownloadButton != (Object) null)
      this.m_downloadButton = this.CreateMenuButton("AssetDownloadButton", "GLOBAL_ASSET_DOWNLOAD", new UIEvent.Handler(this.AssetDownloadButtonPressed), this.m_menu.m_templateDownloadButton);
    if ((Object) this.m_menu.m_templateSignUpButton != (Object) null)
      this.m_signUpButton = this.CreateMenuButton("SignUpButton", "GLUE_TEMPORARY_ACCOUNT_SIGN_UP", new UIEvent.Handler(this.OnSignUpPressed), this.m_menu.m_templateSignUpButton);
    this.m_menu.m_headerText.Text = GameStrings.Get("GLOBAL_GAME_MENU");
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    this.m_minTurnsForProgressAfterConcede = netObject.MinTurnsForProgressAfterConcede;
    this.m_minHPForProgressAfterConcede = netObject.MinHPForProgressAfterConcede;
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    this.m_gameMenuBase.DestroyOptionsMenu();
    GameMenu.s_instance = (GameMenu) null;
  }

  private void Start() => this.gameObject.SetActive(false);

  private void OnEnable() => this.m_leaveButton.SetText(GameStrings.Get("GLOBAL_LEAVE_SPECTATOR_MODE"));

  public bool GameMenuIsShown() => this.IsShown();

  public void GameMenuShow() => this.Show(true);

  public void GameMenuHide() => this.Hide();

  public void GameMenuShowOptionsMenu() => this.ShowOptionsMenu();

  public GameObject GameMenuGetGameObject() => this.gameObject;

  public static GameMenu Get() => GameMenu.s_instance;

  public override void Show(bool playSound = true)
  {
    if ((Object) MiscellaneousMenu.Get() != (Object) null && MiscellaneousMenu.Get().IsShown())
      MiscellaneousMenu.Get().Hide();
    if ((Object) OptionsMenu.Get() != (Object) null && OptionsMenu.Get().IsShown())
    {
      UniversalInputManager.Get().CancelTextInput(this.gameObject, true);
      OptionsMenu.Get().Hide();
    }
    else
    {
      this.UpdateConcedeButtonAlternativeText();
      base.Show(playSound);
      if ((bool) UniversalInputManager.UsePhoneUI && (Object) this.m_ratingsObject != (Object) null)
        this.m_ratingsObject.SetActive(this.m_gameMenuBase.UseKoreanRating());
      this.ShowCursorIfNeeded();
      this.ShowLoginTooltipIfNeeded();
      BnetBar.Get().m_menuButton.SetSelected(true);
    }
  }

  public override void Hide()
  {
    base.Hide();
    this.HideLoginTooltip();
    BnetBar.Get().m_menuButton.SetSelected(false);
  }

  public void ShowCursorIfNeeded()
  {
    if (!((Object) PegCursor.Get() != (Object) null))
      return;
    PegCursor.Get().Show();
  }

  public void ShowLoginTooltipIfNeeded()
  {
    if (Network.ShouldBeConnectedToAurora() || this.m_hasSeenLoginTooltip)
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      Vector3 position = new Vector3(-82.9f, 42.1f, 17.2f);
      this.m_loginButtonPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, this.BUTTON_SCALE_PHONE, GameStrings.Get("GLOBAL_MOBILE_LOG_IN_TOOLTIP"), false);
    }
    else
    {
      Vector3 position = new Vector3(-46.9f, 34.2f, 9.4f);
      this.m_loginButtonPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, this.BUTTON_SCALE, GameStrings.Get("GLOBAL_MOBILE_LOG_IN_TOOLTIP"), false);
    }
    if (!((Object) this.m_loginButtonPopup != (Object) null))
      return;
    this.m_loginButtonPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
    this.m_hasSeenLoginTooltip = true;
  }

  public void HideLoginTooltip()
  {
    if ((Object) this.m_loginButtonPopup != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_loginButtonPopup);
    this.m_loginButtonPopup = (Notification) null;
  }

  public static bool IsInGameMenu() => SceneMgr.Get().IsInGame() && SceneMgr.Get().IsSceneLoaded() && !LoadingScreen.Get().IsTransitioning() && GameState.Get() != null && !GameState.Get().IsGameOver() && (!((Object) TutorialProgressScreen.Get() != (Object) null) || !TutorialProgressScreen.Get().gameObject.activeInHierarchy);

  public static bool CanLogInOrCreateAccount()
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.STARTUP:
      case SceneMgr.Mode.LOGIN:
        return false;
      default:
        return true;
    }
  }

  public void ShowOptionsMenu()
  {
    if (this.m_gameMenuBase == null)
      return;
    this.m_gameMenuBase.ShowOptionsMenu();
  }

  protected override List<UIBButton> GetButtons()
  {
    List<UIBButton> buttons = new List<UIBButton>();
    bool flag1 = GameMenu.IsInGameMenu();
    if (flag1)
    {
      bool flag2 = false;
      if (GameUtils.CanConcedeCurrentMission())
      {
        if (GameUtils.IsWaitingForOpponentReconnect())
          buttons.Add(this.m_endGameButton);
        else
          buttons.Add(this.m_concedeButton);
        flag2 = true;
      }
      if (SpectatorManager.Get().IsSpectatingOrWatching)
      {
        buttons.Add(this.m_leaveButton);
        flag2 = true;
      }
      if (GameUtils.CanRestartCurrentMission() && !this.ShouldHideRestartButton())
      {
        buttons.Add(this.m_restartButton);
        flag2 = true;
      }
      if (flag2)
        buttons.Add((UIBButton) null);
    }
    bool flag3 = false;
    if (!GameMenu.IsInGameMenu() && GameMenu.CanLogInOrCreateAccount() && TemporaryAccountManager.IsTemporaryAccount() && (Object) this.m_signUpButton != (Object) null && !flag1)
    {
      flag3 = true;
      buttons.Add(this.m_signUpButton);
    }
    if (!DemoMgr.Get().IsExpoDemo())
    {
      buttons.Add(this.m_optionsButton);
      if (!flag3)
      {
        if ((bool) HearthstoneApplication.CanQuitGame)
        {
          if (GameMenu.CanLogInOrCreateAccount() && PlatformSettings.OS == OSCategory.Android)
            buttons.Add(this.m_loginButton);
          buttons.Add(this.m_quitButton);
        }
        else if (GameMenu.CanLogInOrCreateAccount() && !flag1)
          buttons.Add(this.m_loginButton);
      }
    }
    if ((Object) this.m_downloadButton != (Object) null && this.DownloadManager != null && this.DownloadManager.IsAnyDownloadRequestedAndIncomplete && this.DownloadManager.InterruptionReason != InterruptionReason.Fetching && !this.DownloadManager.ShouldNotDownloadOptionalData)
      buttons.Add(this.m_downloadButton);
    return buttons;
  }

  protected override void LayoutMenu()
  {
    this.LayoutMenuButtons();
    this.m_menu.m_buttonContainer.UpdateSlices();
    this.LayoutMenuBackground();
    if (!((Object) this.m_ratingsObject != (Object) null) || !((Object) this.m_ratingsAnchor != (Object) null))
      return;
    this.m_ratingsObject.transform.position = this.m_ratingsAnchor.position;
  }

  private void QuitButtonPressed(UIEvent e)
  {
    Network.Get().AutoConcede();
    HearthstoneApplication.Get().Exit();
  }

  private void LogoutButtonPressed(UIEvent e)
  {
    this.HideLoginTooltip();
    this.Hide();
    this.m_regionSwitchMenuController.ShowRegionMenuWithDefaultSettings();
  }

  private void ConcedeButtonPressed(UIEvent e)
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      this.Hide();
    if (GameMgr.Get().IsTraditionalTutorial())
    {
      GameUtils.CompleteTraditionalTutorial();
      gameState.Concede();
      this.Hide();
    }
    else if (this.IsValidConcede(gameState))
    {
      gameState.Concede();
      this.Hide();
    }
    else
      this.ShowConfirmConcedePopup();
  }

  private bool IsValidConcede(GameState gameState)
  {
    if (ProgressUtils.EarlyConcedeConfirmationDisabled)
      return true;
    Player friendlySidePlayer = gameState.GetFriendlySidePlayer();
    if ((friendlySidePlayer == null ? 0 : (friendlySidePlayer.IsEarlyConcedePopupAvailable() ? 1 : 0)) == 0)
      return true;
    int turn = gameState.GetTurn();
    Entity hero = friendlySidePlayer.GetHero();
    return (hero != null ? hero.GetCurrentHealth() : 0) <= this.m_minHPForProgressAfterConcede || turn >= this.m_minTurnsForProgressAfterConcede;
  }

  private void ShowConfirmConcedePopup()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_PROGRESSION_NO_QUEST_PROGRESS_FOR_CONCEDE_HEADER"),
      m_text = GameStrings.Get("GLUE_PROGRESSION_NO_QUEST_PROGRESS_FOR_CONCEDE_BODY"),
      m_confirmText = GameStrings.Get("GLUE_PROGRESSION_NO_QUEST_PROGRESS_FOR_CONCEDE_CONFIRM"),
      m_cancelText = GameStrings.Get("GLUE_PROGRESSION_NO_QUEST_PROGRESS_FOR_CONCEDE_CANCEL"),
      m_showAlertIcon = true,
      m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnConcededWarningAlertAnswered)
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void OnConcededWarningAlertAnswered(AlertPopup.Response response, object userData)
  {
    switch (response)
    {
      case AlertPopup.Response.OK:
      case AlertPopup.Response.CONFIRM:
        if (GameState.Get() != null)
        {
          GameState.Get().Concede();
          break;
        }
        break;
    }
    this.Hide();
  }

  private void LeaveButtonPressed(UIEvent e)
  {
    if (SpectatorManager.Get().IsInSpectatorMode())
      SpectatorManager.Get().LeaveSpectatorMode();
    this.Hide();
  }

  private void RestartButtonPressed(UIEvent e)
  {
    if (GameState.Get() != null)
      GameState.Get().Restart();
    this.Hide();
  }

  private void OptionsButtonPressed(UIEvent e) => this.ShowOptionsMenu();

  private void AssetDownloadButtonPressed(UIEvent e)
  {
    this.Hide();
    DialogManager.Get().ShowAssetDownloadPopup(new AssetDownloadDialog.Info());
  }

  private void OnSignUpPressed(UIEvent e)
  {
    this.Hide();
    TemporaryAccountManager.Get().ShowHealUpPage(TemporaryAccountManager.HealUpReason.GAME_MENU);
  }

  private void LoadRatings()
  {
    this.m_ratingsAnchor = this.m_menu.transform.Find(this.m_anchorForKoreanRatings);
    if (!this.m_gameMenuBase.UseKoreanRating() || !((Object) this.m_ratingsAnchor != (Object) null))
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Korean_Ratings_OptionsScreen.prefab:aea866fab02b24ca697ede020cd85772", (PrefabCallback<GameObject>) ((name, go, data) =>
    {
      if ((Object) go == (Object) null)
        return;
      Quaternion localRotation = go.transform.localRotation;
      go.transform.parent = this.m_menu.transform;
      go.transform.localScale = Vector3.one;
      go.transform.localRotation = localRotation;
      go.transform.position = this.m_ratingsAnchor.position;
      this.m_ratingsObject = go;
      this.LayoutMenu();
    }));
  }

  private bool ShouldHideRestartButton()
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      return false;
    GameEntity gameEntity = gameState.GetGameEntity();
    return gameEntity != null && gameEntity.HasTag(GAME_TAG.HIDE_RESTART_BUTTON);
  }

  private void UpdateConcedeButtonAlternativeText()
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      return;
    GameEntity gameEntity = gameState.GetGameEntity();
    if (gameEntity == null)
      return;
    switch (gameEntity.GetTag(GAME_TAG.CONCEDE_BUTTON_ALTERNATIVE_TEXT))
    {
      case 0:
        this.m_concedeButton.SetText(GameStrings.Get("GLOBAL_CONCEDE"));
        break;
      case 1:
        this.m_concedeButton.SetText(GameStrings.Get("GLOBAL_LEAVE"));
        break;
      case 2:
        this.m_concedeButton.SetText(GameStrings.Get("GLOBAL_LEAVE_TUTORIAL"));
        break;
      case 3:
        this.m_concedeButton.SetText(GameStrings.Get("GLOBAL_SKIP_TUTORIAL"));
        break;
      default:
        this.m_concedeButton.SetText(GameStrings.Get("GLOBAL_CONCEDE"));
        Log.Gameplay.PrintError(string.Format("GameMenu.UpdateConcedeButtonAlternativeText() - invalid concede button alternative text"));
        break;
    }
  }
}
