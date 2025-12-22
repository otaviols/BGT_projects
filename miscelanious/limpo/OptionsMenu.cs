using Blizzard.T5.Configuration;
using Blizzard.T5.Fonts;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Streaming;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class OptionsMenu : MonoBehaviour
{
  [CustomEditField(Sections = "Layout")]
  public MultiSliceElement m_leftPane;
  [CustomEditField(Sections = "Layout")]
  public MultiSliceElement m_rightPane;
  [CustomEditField(Sections = "Layout")]
  public MultiSliceElement m_middlePane;
  [CustomEditField(Sections = "Layout")]
  public MultiSliceElement m_middleBottomPane;
  [CustomEditField(Sections = "Layout")]
  public MultiSliceElement m_middleBottomLeftPane;
  [CustomEditField(Sections = "Layout")]
  public MultiSliceElement m_middleBottomRightPane;
  [CustomEditField(Sections = "Placeholder")]
  public GameObject m_middleLeftPaneLabel;
  [CustomEditField(Sections = "Placeholder")]
  public GameObject m_middleRightPaneLabel;
  [CustomEditField(Sections = "Graphics")]
  public GameObject m_graphicsGroup;
  [CustomEditField(Sections = "Graphics")]
  public DropdownControl m_graphicsRes;
  [CustomEditField(Sections = "Graphics")]
  public DropdownControl m_graphicsQuality;
  [CustomEditField(Sections = "Graphics")]
  public DropdownControl m_graphicsFps;
  [CustomEditField(Sections = "Graphics")]
  public CheckBox m_fullScreenCheckbox;
  [CustomEditField(Sections = "Sound")]
  public GameObject m_soundGroup;
  [CustomEditField(Sections = "Sound")]
  public ScrollbarControl m_masterVolume;
  [CustomEditField(Sections = "Sound")]
  public ScrollbarControl m_musicVolume;
  [CustomEditField(Sections = "Sound")]
  public CheckBox m_backgroundSound;
  [CustomEditField(Sections = "Language")]
  public GameObject m_languageGroup;
  [CustomEditField(Sections = "Language")]
  public DropdownControl m_languageDropdown;
  [CustomEditField(Sections = "Language")]
  public FontDefinition m_languageDropdownFont;
  [CustomEditField(Sections = "Language")]
  public CheckBox m_languagePackCheckbox;
  [CustomEditField(Sections = "Other")]
  public CheckBox m_spectatorOpenJoinCheckbox;
  [CustomEditField(Sections = "Other")]
  public CheckBox m_screenShakeCheckbox;
  [CustomEditField(Sections = "Other")]
  public UIBButton m_switchAccountButton;
  [CustomEditField(Sections = "Other")]
  public UIBButton m_miscellaneousButton;
  [CustomEditField(Sections = "Other")]
  public UIBButton m_privacyButton;
  [CustomEditField(Sections = "Internal Stuff")]
  public UberText m_versionLabel;
  private static OptionsMenu s_instance;
  private bool m_isShown;
  private OptionsMenu.hideHandler m_hideHandler;
  private MiscellaneousMenu m_miscellaneousMenu;
  private bool m_miscellaneousMenuLoading;
  private PrivacyMenu m_privacyMenu;
  private bool m_privacyMenuLoading;
  private PegUIElement m_inputBlocker;
  private RegionSwitchMenuController m_controller = new RegionSwitchMenuController();
  private IGraphicsManager m_graphicsManager;
  private List<GraphicsResolution> m_fullScreenResolutions = new List<GraphicsResolution>();
  private Vector3 NORMAL_SCALE;
  private Vector3 HIDDEN_SCALE;
  private readonly PlatformDependentValue<bool> LANGUAGE_SELECTION = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    iOS = true,
    Android = true,
    PC = false,
    Mac = false
  };

  private IGameDownloadManager DownloadManager => GameDownloadManagerProvider.Get();

  private void Awake()
  {
    OptionsMenu.s_instance = this;
    this.NORMAL_SCALE = this.transform.localScale;
    this.HIDDEN_SCALE = 0.01f * this.NORMAL_SCALE;
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    OverlayUI.Get().AddGameObject(this.gameObject);
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_graphicsRes.setUnselectedItemText(GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_RESOLUTION_CUSTOM"));
      this.m_graphicsRes.setItemTextCallback(new DropdownControl.itemTextCallback(this.OnGraphicsResolutionDropdownText));
      this.m_graphicsRes.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnNewGraphicsResolution));
      foreach (object obj in this.GetGoodGraphicsResolution())
        this.m_graphicsRes.addItem(obj);
      this.m_graphicsRes.setSelection((object) this.GetCurrentGraphicsResolution());
      this.m_fullScreenCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnToggleFullScreenCheckbox));
      this.m_fullScreenCheckbox.SetChecked(Screen.fullScreen);
      this.m_graphicsQuality.addItem((object) GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_LOW"));
      this.m_graphicsQuality.addItem((object) GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_MEDIUM"));
      this.m_graphicsQuality.addItem((object) GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_HIGH"));
      this.m_graphicsQuality.setSelection((object) this.GetCurrentGraphicsQuality());
      this.m_graphicsQuality.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnNewGraphicsQuality));
    }
    this.m_graphicsFps.addItem((object) GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_DEFAULT"));
    if (Screen.currentResolution.refreshRate > 60)
      this.m_graphicsFps.addItem((object) GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_MEDIUM"));
    this.m_graphicsFps.addItem((object) GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_HIGH"));
    this.m_graphicsFps.setSelection((object) this.GetCurrentGraphicsFps());
    this.m_graphicsFps.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnNewGraphicsFps));
    this.m_graphicsFps.gameObject.SetActive(true);
    this.m_masterVolume.SetValue(Options.Get().GetFloat(Option.SOUND_VOLUME));
    this.m_masterVolume.SetUpdateHandler(new ScrollbarControl.UpdateHandler(this.OnNewMasterVolume));
    this.m_masterVolume.SetFinishHandler(new ScrollbarControl.FinishHandler(this.OnMasterVolumeRelease));
    this.m_musicVolume.SetValue(Options.Get().GetFloat(Option.MUSIC_VOLUME));
    this.m_musicVolume.SetUpdateHandler(new ScrollbarControl.UpdateHandler(this.OnNewMusicVolume));
    if ((UnityEngine.Object) this.m_backgroundSound != (UnityEngine.Object) null)
    {
      this.m_backgroundSound.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleBackgroundSound));
      this.m_backgroundSound.SetChecked(Options.Get().GetBool(Option.BACKGROUND_SOUND));
    }
    this.m_languageGroup.gameObject.SetActive((bool) this.LANGUAGE_SELECTION);
    if ((bool) this.LANGUAGE_SELECTION && (this.DownloadManager == null || !this.DownloadManager.ShouldNotDownloadOptionalData))
    {
      this.m_languageDropdown.setFont(this.m_languageDropdownFont.m_Font);
      foreach (Locale locale in Enum.GetValues(typeof (Locale)))
      {
        if (locale != -1 && (PlatformSettings.LocaleVariant != LocaleVariant.China || locale == null || locale == 9))
          this.m_languageDropdown.addItem((object) GameStrings.Get(this.StringNameFromLocale(locale)));
      }
      this.m_languageDropdown.setSelection((object) this.GetCurrentLanguage());
      this.m_languageDropdown.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnNewLanguage));
    }
    this.UpdateOtherUI();
    if (TemporaryAccountManager.IsTemporaryAccount())
    {
      this.m_spectatorOpenJoinCheckbox.gameObject.SetActive(false);
      this.m_switchAccountButton.gameObject.SetActive(true);
      this.m_switchAccountButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSwitchAccountButtonReleased));
    }
    else
    {
      this.m_spectatorOpenJoinCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleSpectatorOpenJoin));
      this.m_spectatorOpenJoinCheckbox.SetChecked(Options.Get().GetBool(Option.SPECTATOR_OPEN_JOIN));
    }
    if ((UnityEngine.Object) this.m_screenShakeCheckbox != (UnityEngine.Object) null)
    {
      this.m_screenShakeCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleScreenShake));
      this.m_screenShakeCheckbox.SetChecked(Options.Get().GetBool(Option.SCREEN_SHAKE_ENABLED));
    }
    this.m_miscellaneousButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnMiscellaneousButtonReleased));
    this.m_privacyButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPrivacyButtonReleased));
    this.CreateInputBlocker();
    this.ShowOrHide(false);
    if (PlatformSettings.IsMobile())
    {
      if ((UnityEngine.Object) this.m_backgroundSound != (UnityEngine.Object) null)
        this.m_backgroundSound.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_graphicsRes != (UnityEngine.Object) null)
        this.m_graphicsRes.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_graphicsQuality != (UnityEngine.Object) null)
        this.m_graphicsQuality.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_fullScreenCheckbox != (UnityEngine.Object) null)
        this.m_fullScreenCheckbox.gameObject.SetActive(false);
      string str1 = string.Format("{0} {1}.{2}", (object) GameStrings.Get("GLOBAL_VERSION"), (object) "25.0", (object) 158725);
      string str2 = Vars.Key("Application.Referral").GetStr("none");
      if (str2 != "none")
        str1 = str1 + "-" + str2;
      this.m_versionLabel.Text = str1;
      this.m_versionLabel.gameObject.SetActive(true);
    }
    this.UpdateUI();
    this.m_graphicsGroup.GetComponent<MultiSliceElement>().UpdateSlices();
    this.m_graphicsManager.OnResolutionChangedEvent += new Action<int, int>(this.UpdateMenuItemValues);
  }

  public void OnDestroy()
  {
    if (FatalErrorMgr.Get() != null)
      FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    if (this.m_graphicsManager != null)
      this.m_graphicsManager.OnResolutionChangedEvent -= new Action<int, int>(this.UpdateMenuItemValues);
    OptionsMenu.s_instance = (OptionsMenu) null;
  }

  public static OptionsMenu Get() => OptionsMenu.s_instance;

  public OptionsMenu.hideHandler GetHideHandler() => this.m_hideHandler;

  public void SetHideHandler(OptionsMenu.hideHandler handler) => this.m_hideHandler = handler;

  public void RemoveHideHandler(OptionsMenu.hideHandler handler)
  {
    if (!(this.m_hideHandler == handler))
      return;
    this.m_hideHandler = (OptionsMenu.hideHandler) null;
  }

  public bool IsShown() => this.m_isShown;

  public void Show()
  {
    this.UpdateOtherUI();
    this.ShowOrHide(true);
    AnimationUtil.ShowWithPunch(this.gameObject, this.HIDDEN_SCALE, 1.1f * this.NORMAL_SCALE, this.NORMAL_SCALE, (string) null, true);
  }

  public void Hide(bool callHideHandler = true)
  {
    this.ShowOrHide(false);
    if (!(this.m_hideHandler != null & callHideHandler))
      return;
    this.m_hideHandler();
    this.m_hideHandler = (OptionsMenu.hideHandler) null;
  }

  private GraphicsResolution GetCurrentGraphicsResolution() => GraphicsResolution.create(Options.Get().GetInt(Option.GFX_WIDTH, Screen.currentResolution.width), Options.Get().GetInt(Option.GFX_HEIGHT, Screen.currentResolution.height));

  private string GetCurrentGraphicsQuality()
  {
    switch (Options.Get().GetInt(Option.GFX_QUALITY))
    {
      case 0:
        return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_LOW");
      case 1:
        return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_MEDIUM");
      case 2:
        return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_HIGH");
      default:
        return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_LOW");
    }
  }

  private string GetCurrentGraphicsFps()
  {
    int num = Options.Get().GetInt(Option.GFX_TARGET_FRAME_RATE);
    if (num == 30)
      return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_DEFAULT");
    return Screen.currentResolution.refreshRate == num ? GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_HIGH") : GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_MEDIUM");
  }

  private List<GraphicsResolution> GetGoodGraphicsResolution()
  {
    if (this.m_fullScreenResolutions.Count == 0)
    {
      foreach (GraphicsResolution graphicsResolution in GraphicsResolution.list)
      {
        if (graphicsResolution.x >= 1024 && graphicsResolution.y >= 728 && (double) graphicsResolution.aspectRatio - 0.01 <= 16.0 / 9.0 && (double) graphicsResolution.aspectRatio + 0.01 >= 4.0 / 3.0)
          this.m_fullScreenResolutions.Add(graphicsResolution);
      }
    }
    return this.m_fullScreenResolutions;
  }

  private string GetCurrentLanguage() => GameStrings.Get(this.StringNameFromLocale(Localization.GetLocale()));

  private void ShowOrHide(bool showOrHide)
  {
    this.m_isShown = showOrHide;
    this.gameObject.SetActive(showOrHide);
    this.UpdateUI();
  }

  private string StringNameFromLocale(Locale locale) => "GLOBAL_LANGUAGE_NATIVE_" + locale.ToString().ToUpper();

  private void UpdateOtherUI()
  {
    this.m_miscellaneousButton.gameObject.SetActive(this.CanShowOtherMenuOptions());
    this.m_middleBottomRightPane.gameObject.SetActive(true);
  }

  private void UpdateUI()
  {
    this.m_middleLeftPaneLabel.SetActive(true);
    this.m_middleRightPaneLabel.SetActive(true);
    this.m_middleBottomLeftPane.UpdateSlices();
    this.m_middleBottomRightPane.UpdateSlices();
    this.m_middleBottomPane.UpdateSlices();
    this.m_leftPane.UpdateSlices();
    this.m_rightPane.UpdateSlices();
    this.m_middlePane.UpdateSlices();
    this.m_middleLeftPaneLabel.SetActive(false);
    this.m_middleRightPaneLabel.SetActive(false);
  }

  private bool CanShowOtherMenuOptions() => UserAttentionManager.GetAvailabilityBlockerReason(false) == AvailabilityBlockerReasons.NONE && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.PACKOPENING) && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.ADVENTURE) && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.CREDITS) && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FRIENDLY);

  private void CreateInputBlocker()
  {
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "OptionMenuInputBlocker", (Component) this, (Component) this.transform, 10f);
    inputBlocker.layer = this.gameObject.layer;
    this.m_inputBlocker = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Hide()));
  }

  private void UpdateMenuItemValues(int newWidth, int newHeight)
  {
    if (this.m_fullScreenCheckbox.IsChecked() != Screen.fullScreen)
    {
      this.m_fullScreenCheckbox.SetChecked(Screen.fullScreen);
      if (!(this.m_graphicsRes.getSelection() is GraphicsResolution selection) || this.m_fullScreenCheckbox.IsChecked())
      {
        this.m_graphicsRes.setSelectionToFirstItem();
        selection = this.m_graphicsRes.getSelection() as GraphicsResolution;
      }
      else if (!this.m_fullScreenCheckbox.IsChecked())
        selection = GraphicsResolution.create(newWidth, newHeight);
      if (selection == null)
        return;
      this.m_graphicsRes.setSelection((object) GraphicsResolution.create(selection.x, selection.y));
    }
    else
    {
      if (Screen.fullScreen)
        return;
      this.m_graphicsRes.setSelection((object) GraphicsResolution.create(newWidth, newHeight));
    }
  }

  private void OnFatalError(FatalErrorMessage message, object userData)
  {
    if (SceneMgr.Get().GetNextMode() != SceneMgr.Mode.FATAL_ERROR)
      return;
    this.Hide();
  }

  private void OnToggleFullScreenCheckbox(UIEvent e)
  {
    if (this.m_fullScreenCheckbox.IsChecked() == Screen.fullScreen)
      return;
    if (!(this.m_graphicsRes.getSelection() is GraphicsResolution selection))
    {
      this.m_graphicsRes.setSelectionToFirstItem();
      selection = this.m_graphicsRes.getSelection() as GraphicsResolution;
    }
    if (selection == null)
      return;
    int width = selection.x;
    int height = selection.y;
    if (this.m_fullScreenCheckbox.IsChecked())
    {
      width = Screen.currentResolution.width;
      height = Screen.currentResolution.height;
    }
    this.m_graphicsRes.setSelection((object) GraphicsResolution.create(width, height));
    this.m_graphicsManager.SetScreenResolution(width, height, this.m_fullScreenCheckbox.IsChecked());
    Options.Get().SetBool(Option.GFX_FULLSCREEN, this.m_fullScreenCheckbox.IsChecked());
  }

  private void OnNewGraphicsQuality(object selection, object prevSelection)
  {
    GraphicsQuality graphicsQuality = GraphicsQuality.Low;
    string str = (string) selection;
    if (str == GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_LOW"))
      graphicsQuality = GraphicsQuality.Low;
    else if (str == GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_MEDIUM"))
      graphicsQuality = GraphicsQuality.Medium;
    else if (str == GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_HIGH"))
      graphicsQuality = GraphicsQuality.High;
    Log.Options.Print("Graphics Quality: " + graphicsQuality.ToString());
    this.m_graphicsManager.RenderQualityLevel = graphicsQuality;
  }

  private void OnNewGraphicsResolution(object selection, object prevSelection)
  {
    GraphicsResolution graphicsResolution = (GraphicsResolution) selection;
    this.m_graphicsManager.SetScreenResolution(graphicsResolution.x, graphicsResolution.y, this.m_fullScreenCheckbox.IsChecked());
    Options.Get().SetInt(Option.GFX_WIDTH, graphicsResolution.x);
    Options.Get().SetInt(Option.GFX_HEIGHT, graphicsResolution.y);
  }

  private void OnNewLanguage(object selection, object prevSelection)
  {
    if (selection == prevSelection)
      return;
    long num = FreeSpace.Measure();
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    if (num < 314572800L)
    {
      info.m_headerText = GameStrings.Get("GLOBAL_LANGUAGE_CHANGE_OUT_OF_SPACE_TITLE");
      info.m_text = string.Format(GameStrings.Get("GLOBAL_LANGUAGE_CHANGE_OUT_OF_SPACE_MESSAGE"), (object) DownloadStatusView.FormatBytesAsHumanReadable(314572800L));
      info.m_showAlertIcon = false;
      info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
    }
    else
    {
      info.m_headerText = GameStrings.Get("GLOBAL_LANGUAGE_CHANGE_CONFIRM_TITLE");
      info.m_text = GameStrings.Get("GLOBAL_LANGUAGE_CHANGE_CONFIRM_MESSAGE");
      info.m_showAlertIcon = false;
      info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
      info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnChangeLanguageConfirmationResponse);
      info.m_responseUserData = selection;
    }
    DialogManager.Get().ShowPopup(info);
  }

  private void OnNewGraphicsFps(object selection, object prevSelection)
  {
    string a = (string) selection;
    if (string.Equals(a, GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_DEFAULT")))
      this.m_graphicsManager.UpdateTargetFramerate(30, true);
    else if (string.Equals(a, GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_HIGH")))
    {
      this.m_graphicsManager.UpdateTargetFramerate(Screen.currentResolution.refreshRate, false);
    }
    else
    {
      if (!string.Equals(a, GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_FPS_MEDIUM")))
        return;
      this.m_graphicsManager.UpdateTargetFramerate(60, false);
    }
  }

  private void OnChangeLanguageConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
    {
      this.m_languageDropdown.setSelection((object) this.GetCurrentLanguage());
    }
    else
    {
      string str = (string) userData;
      Locale locale1 = (Locale) -1;
      foreach (Locale locale2 in Enum.GetValues(typeof (Locale)))
      {
        if (str == GameStrings.Get(this.StringNameFromLocale(locale2)))
        {
          locale1 = locale2;
          break;
        }
      }
      if (locale1 == -1)
      {
        Debug.LogError((object) string.Format("OptionsMenu.OnChangeLanguageConfirmationResponse() - locale not found"));
      }
      else
      {
        TelemetryManager.Client().SendLanguageChanged(Localization.GetLocaleName(), locale1.ToString());
        Localization.SetLocale(locale1);
        Options.Get().SetString(Option.LOCALE, locale1.ToString());
        Debug.LogFormat("Change Locale: {0}", (object) locale1);
        this.Hide(false);
        HearthstoneApplication.Get().IsLocaleChanged = true;
        if (this.DownloadManager.ShouldDownloadLocalizedAssets)
          HearthstoneApplication.Get().Resetting += new Action(this.StartUpdateProcessAfterReset);
        HearthstoneApplication.Get().Reset();
      }
    }
  }

  private void StartUpdateProcessAfterReset()
  {
    HearthstoneApplication.Get().Resetting -= new Action(this.StartUpdateProcessAfterReset);
    this.DownloadManager.StartUpdateProcess(true);
  }

  private string OnGraphicsResolutionDropdownText(object val)
  {
    GraphicsResolution graphicsResolution = (GraphicsResolution) val;
    return string.Format("{0} x {1}", (object) graphicsResolution.x, (object) graphicsResolution.y);
  }

  private void OnNewMasterVolume(float newVolume) => Options.Get().SetFloat(Option.SOUND_VOLUME, newVolume);

  private void OnMasterVolumeRelease()
  {
    SoundManager.LoadedCallback callback = (SoundManager.LoadedCallback) ((source, userData) => SoundManager.Get().Set3d(source, false));
    SoundManager.Get().LoadAndPlay((AssetReference) "UI_MouseClick_01.prefab:fa537702a0db1c3478c989967458788b", this.gameObject, 1f, callback);
  }

  private void OnNewMusicVolume(float newVolume) => Options.Get().SetFloat(Option.MUSIC_VOLUME, newVolume);

  private void ToggleBackgroundSound(UIEvent e) => Options.Get().SetBool(Option.BACKGROUND_SOUND, this.m_backgroundSound.IsChecked());

  private void OnSwitchAccountButtonReleased(UIEvent e)
  {
    this.Hide(false);
    this.m_controller.ShowRegionMenuWithDefaultSettings();
  }

  private void ToggleSpectatorOpenJoin(UIEvent e) => Options.Get().SetBool(Option.SPECTATOR_OPEN_JOIN, this.m_spectatorOpenJoinCheckbox.IsChecked());

  private void ToggleScreenShake(UIEvent e) => Options.Get().SetBool(Option.SCREEN_SHAKE_ENABLED, this.m_screenShakeCheckbox.IsChecked());

  private void OnMiscellaneousButtonReleased(UIEvent e)
  {
    this.LoadMiscellaneousMenu();
    this.Hide(false);
  }

  private void LoadMiscellaneousMenu()
  {
    if (this.m_miscellaneousMenuLoading)
      return;
    if ((UnityEngine.Object) this.m_miscellaneousMenu == (UnityEngine.Object) null)
    {
      this.m_miscellaneousMenuLoading = true;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "MiscellaneousMenu.prefab:ee334ff827a9f834ea8b96e3dd2f5c5d", new PrefabCallback<GameObject>(this.ShowMiscellaneousMenu));
    }
    else
      this.m_miscellaneousMenu.Show(false);
  }

  private void ShowMiscellaneousMenu(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_miscellaneousMenu = go.GetComponent<MiscellaneousMenu>();
    this.m_miscellaneousMenu.Show(false);
    this.m_miscellaneousMenuLoading = false;
  }

  private void OnPrivacyButtonReleased(UIEvent e)
  {
    this.LoadPrivacyMenu();
    this.Hide(false);
  }

  private void LoadPrivacyMenu()
  {
    if (this.m_privacyMenuLoading)
      return;
    if ((UnityEngine.Object) this.m_privacyMenu == (UnityEngine.Object) null)
    {
      this.m_privacyMenuLoading = true;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "PrivacyMenu.prefab:57d6ca815c24ab948be8f1d27490ee86", new PrefabCallback<GameObject>(this.ShowPrivacyMenu));
    }
    else
      this.m_privacyMenu.Show(false);
  }

  private void ShowPrivacyMenu(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_privacyMenu = go.GetComponent<PrivacyMenu>();
    this.m_privacyMenu.Show(false);
    this.m_privacyMenuLoading = false;
  }

  public delegate void hideHandler();
}
