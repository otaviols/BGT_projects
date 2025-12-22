using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.Login;
using Hearthstone.Streaming;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class SplashScreen : MonoBehaviour
{
  private const float RATINGS_SCREEN_DISPLAY_TIME = 5f;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_queueSign;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_quitButtonParent;
  public UberText m_queueTitle;
  public UberText m_queueText;
  public UberText m_queueTime;
  public StandardPegButtonNew m_quitButton;
  public Glow m_glow1;
  public Glow m_glow2;
  public GameObject m_blizzardLogo;
  public GameObject m_demoDisclaimer;
  public StandardPegButtonNew m_devClearLoginButton;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_cnRatingsPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_krRatingsPrefab;
  private const float GLOW_FADE_TIME = 1f;
  private static SplashScreen s_instance;
  private bool m_queueShown;
  private bool m_fadingStarted;
  private bool m_inputCameraSet;
  private const long MAX_MINUTES_TO_SHOW_FOR_QUEUE_ETA = 15;

  private void Awake()
  {
    SplashScreen.s_instance = this;
    OverlayUI.Get().AddGameObject(this.gameObject);
    this.Show();
    LogoAnimation.Get().ShowLogo();
    if (Vars.Key("Aurora.ClientCheck").GetBool(true) && BattleNetClient.needsToRun)
    {
      BattleNetClient.quitHearthstoneAndRun();
    }
    else
    {
      if (DemoMgr.Get().GetMode() == DemoMode.BLIZZ_MUSEUM)
        this.m_demoDisclaimer.SetActive(true);
      if (HearthstoneApplication.IsInternal() && (bool) HearthstoneApplication.AllowResetFromFatalError && !ServiceManager.Get<GameDownloadManager>().IsAnyDownloadRequestedAndIncomplete)
      {
        this.m_devClearLoginButton.gameObject.SetActive(true);
        this.m_devClearLoginButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ClearLogin));
      }
      HearthstoneApplication.Get().WillReset += new System.Action(this.OnWillReset);
    }
  }

  private void OnDestroy()
  {
    if (this.m_inputCameraSet)
    {
      if ((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null && (UnityEngine.Object) OverlayUI.Get() != (UnityEngine.Object) null)
        PegUI.Get().RemoveInputCamera(OverlayUI.Get().m_UICamera);
      this.m_inputCameraSet = false;
    }
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().WillReset -= new System.Action(this.OnWillReset);
    SplashScreen.s_instance = (SplashScreen) null;
  }

  private void Update()
  {
    if (!this.m_inputCameraSet && (UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null && (UnityEngine.Object) OverlayUI.Get() != (UnityEngine.Object) null)
    {
      this.m_inputCameraSet = true;
      PegUI.Get().AddInputCamera(OverlayUI.Get().m_UICamera);
    }
    this.HandleKeyboardInput();
  }

  public static SplashScreen Get() => SplashScreen.s_instance;

  public void Show()
  {
    this.gameObject.SetActive(true);
    iTween.FadeTo(this.gameObject, iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutCubic));
    if (this.m_fadingStarted)
      return;
    this.FadeGlowsIn();
  }

  public IEnumerator<IAsyncJobResult> Hide(
    JobDefinition sceneTransitionJob)
  {
    yield return (IAsyncJobResult) new JobDefinition("Splashscreen.AnimateStartupSequence", this.Job_AnimateStartupSequence(sceneTransitionJob), Array.Empty<IJobDependency>());
  }

  private void UpdateQueueInfo(Network.QueueInfo queueInfo)
  {
    if (queueInfo.secondsTilEnd / 60L > 15L)
    {
      this.m_queueTime.Text = GameStrings.Format("GLOBAL_DATETIME_GREATER_THAN_X_MINUTES", (object) 15L);
    }
    else
    {
      TimeUtils.ElapsedStringSet datetimeStringset = TimeUtils.SPLASHSCREEN_DATETIME_STRINGSET;
      this.m_queueTime.Text = TimeUtils.GetElapsedTimeString((int) queueInfo.secondsTilEnd, datetimeStringset, false);
    }
    this.m_queueTime.TextAlpha = 1f;
    if (this.m_queueShown || queueInfo.secondsTilEnd <= 1L)
      return;
    this.m_queueShown = true;
    if (PlatformSettings.IsMobile())
    {
      this.m_quitButtonParent.SetActive(false);
    }
    else
    {
      this.m_quitButton.SetOriginalLocalPosition();
      this.m_quitButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.QuitGame));
    }
    RenderUtils.SetAlpha(this.m_queueSign, 0.0f);
    this.m_queueSign.SetActive(true);
    iTween.FadeTo(this.m_queueSign, iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) 0.5f, (object) "easeType", (object) iTween.EaseType.easeInCubic));
    Hashtable args = iTween.Hash((object) "amount", (object) 0.0f, (object) "time", (object) 0.5f, (object) "includechildren", (object) true, (object) "easeType", (object) iTween.EaseType.easeOutCubic);
    iTween.FadeTo(LogoAnimation.Get().m_logoContainer, args);
  }

  private void QuitGame(UIEvent e) => HearthstoneApplication.Get().Exit();

  private void ClearLogin(UIEvent e)
  {
    Debug.Log((object) "Clear Login Button pressed from the Splash Screen!");
    ServiceManager.Get<ILoginService>()?.WipeAllAuthenticationData();
  }

  private IEnumerator FadeGlowInOut(Glow glow, float timeDelay, bool shouldStartOver)
  {
    SplashScreen splashScreen = this;
    yield return (object) new WaitForSeconds(timeDelay);
    object[] objArray = new object[12]
    {
      (object) "time",
      (object) 1f,
      (object) "easeType",
      (object) iTween.EaseType.linear,
      (object) "from",
      (object) 0.0f,
      (object) "to",
      (object) 0.4f,
      (object) "onupdate",
      (object) "UpdateAlpha",
      (object) "onupdatetarget",
      (object) glow.gameObject
    };
    iTween.ValueTo(glow.gameObject, iTween.Hash(objArray));
    Hashtable args = iTween.Hash((object) "delay", (object) 1f, (object) "time", (object) 1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "from", (object) 0.4f, (object) "to", (object) 0.0f, (object) "onupdate", (object) "UpdateAlpha", (object) "onupdatetarget", (object) glow.gameObject);
    if (shouldStartOver)
    {
      args.Add((object) "oncomplete", (object) "FadeGlowsIn");
      args.Add((object) "oncompletetarget", (object) splashScreen.gameObject);
    }
    iTween.ValueTo(glow.gameObject, args);
  }

  private void FadeGlowsIn()
  {
    this.m_fadingStarted = true;
    this.StartCoroutine(this.FadeGlowInOut(this.m_glow1, 0.0f, false));
    this.StartCoroutine(this.FadeGlowInOut(this.m_glow2, 1f, true));
  }

  private SplashScreen.RatingsScreenRegion GetRatingsScreenRegion()
  {
    SplashScreen.RatingsScreenRegion ratingsScreenRegion = SplashScreen.RatingsScreenRegion.NONE;
    string str = Vars.Key("Debug.ForceRatingScreen").GetStr(string.Empty);
    if (!string.IsNullOrEmpty(str))
    {
      SplashScreen.RatingsScreenRegion result;
      if (Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<SplashScreen.RatingsScreenRegion>(str, StringComparison.OrdinalIgnoreCase, out result))
        return result;
      Debug.LogWarning((object) ("Unknown rating screen override " + str));
    }
    string accountCountry = BattleNet.GetAccountCountry();
    if (!(accountCountry == "CHN"))
    {
      if (accountCountry == "KOR")
        ratingsScreenRegion = SplashScreen.RatingsScreenRegion.KOREA;
    }
    else
      ratingsScreenRegion = SplashScreen.RatingsScreenRegion.CHINA;
    if (PlatformSettings.IsMobile() && ratingsScreenRegion == SplashScreen.RatingsScreenRegion.NONE && MobileDeviceLocale.GetCountryCode() == "KR")
      ratingsScreenRegion = SplashScreen.RatingsScreenRegion.KOREA;
    if (PlatformSettings.LocaleVariant == LocaleVariant.China)
      ratingsScreenRegion = SplashScreen.RatingsScreenRegion.CHINA;
    return ratingsScreenRegion;
  }

  public bool HandleKeyboardInput() => false;

  public IEnumerator<IAsyncJobResult> Job_AnimateStartupSequence(
    JobDefinition sceneTransitionJob)
  {
    yield return (IAsyncJobResult) new JobDefinition("Splashscreen,ShowScaryWarnings", this.Job_ShowScaryWarnings(), Array.Empty<IJobDependency>());
    yield return (IAsyncJobResult) new JobDefinition("Splashscreen.AnimateRatings", this.Job_AnimateRatings(), Array.Empty<IJobDependency>());
    yield return (IAsyncJobResult) new JobDefinition("SplashScreen.FadeLogoIn", LogoAnimation.Get().Job_FadeLogoIn(), Array.Empty<IJobDependency>());
    Processor.QueueJob(sceneTransitionJob);
    yield return (IAsyncJobResult) new WaitForDuration(2f);
    yield return (IAsyncJobResult) new JobDefinition("Splashscreen.FadeOutSplashscreen", this.Job_FadeOutSplashscreen(), Array.Empty<IJobDependency>());
    this.OnSplashScreenFadeOutComplete();
  }

  public IEnumerator<IAsyncJobResult> Job_ShowLoginQueue()
  {
    if (Network.ShouldBeConnectedToAurora())
    {
      WaitForCallback<Network.QueueInfo> OnQueueModified = new WaitForCallback<Network.QueueInfo>();
      LoginManager.Get().RegisterQueueModifiedListener(OnQueueModified.Callback);
      if (LoginManager.Get().CurrentQueueInfo != null)
        OnQueueModified.Callback(LoginManager.Get().CurrentQueueInfo);
      while (true)
      {
        yield return (IAsyncJobResult) OnQueueModified;
        Network.QueueInfo queueInfo = OnQueueModified.Data.Arg1;
        if (queueInfo.position != 0)
        {
          this.UpdateQueueInfo(queueInfo);
          OnQueueModified.Reset();
        }
        else
          break;
      }
      ServiceManager.Get<LoginManager>().RemoveQueueModifiedListener(OnQueueModified.Callback);
      this.m_queueShown = false;
      this.m_queueSign.SetActive(false);
    }
  }

  private IEnumerator<IAsyncJobResult> Job_ShowScaryWarnings()
  {
    while ((UnityEngine.Object) DialogManager.Get() == (UnityEngine.Object) null)
      yield return (IAsyncJobResult) null;
    while (DialogManager.Get().ShowingDialog())
      yield return (IAsyncJobResult) null;
    this.ShowDevicePerformanceWarning();
    this.ShowGraphicsDeviceWarning();
    this.ShowTextureCompressionWarning();
  }

  public IEnumerator<IAsyncJobResult> Job_AnimateRatings()
  {
    SplashScreen.RatingsScreenRegion ratingsScreenRegion = this.GetRatingsScreenRegion();
    if (ratingsScreenRegion != SplashScreen.RatingsScreenRegion.NONE)
    {
      WidgetInstance widget = WidgetInstance.Create(ratingsScreenRegion == SplashScreen.RatingsScreenRegion.CHINA ? this.m_cnRatingsPrefab : this.m_krRatingsPrefab);
      while (!widget.IsReady)
        yield return (IAsyncJobResult) null;
      IDataModel ratingsDataModel = SplashScreen.GetRatingsDataModel(ratingsScreenRegion);
      if (ratingsDataModel != null)
        widget.BindDataModel(ratingsDataModel, false);
      OverlayUI.Get().AddGameObject(widget.gameObject);
      Hashtable args = iTween.Hash((object) "amount", (object) 0.0f, (object) "time", (object) 0.5f, (object) "includechildren", (object) true, (object) "easeType", (object) iTween.EaseType.easeOutCubic);
      LogoAnimation logoAnimation = LogoAnimation.Get();
      iTween.FadeTo(logoAnimation.m_logoContainer, args);
      yield return (IAsyncJobResult) new WaitForDuration(0.5f);
      logoAnimation.HideLogo();
      widget.Show();
      object[] objArray1 = new object[8]
      {
        (object) "amount",
        (object) 1f,
        (object) "time",
        (object) 0.5f,
        (object) "includechildren",
        (object) true,
        (object) "easeType",
        (object) iTween.EaseType.easeInCubic
      };
      iTween.FadeTo(widget.gameObject, iTween.Hash(objArray1));
      RatingsPopupControl popupControl = widget.GetComponentInChildren<RatingsPopupControl>();
      if ((UnityEngine.Object) popupControl != (UnityEngine.Object) null && popupControl.WaitForUserToStart)
      {
        WaitForCallback waitForCB = new WaitForCallback();
        popupControl.OnUserStartPressed += waitForCB.Callback;
        yield return (IAsyncJobResult) waitForCB;
        popupControl.OnUserStartPressed -= waitForCB.Callback;
        waitForCB = (WaitForCallback) null;
      }
      else
        yield return (IAsyncJobResult) new WaitForDuration(5.5f);
      object[] objArray2 = new object[8]
      {
        (object) "amount",
        (object) 0.0f,
        (object) "time",
        (object) 0.5f,
        (object) "includechildren",
        (object) true,
        (object) "easeType",
        (object) iTween.EaseType.easeInCubic
      };
      iTween.FadeTo(widget.gameObject, iTween.Hash(objArray2));
      yield return (IAsyncJobResult) new WaitForDuration(0.5f);
      widget.Hide();
      UnityEngine.Object.Destroy((UnityEngine.Object) widget.gameObject);
    }
  }

  private static IDataModel GetRatingsDataModel(
    SplashScreen.RatingsScreenRegion ratingsScreenRegion)
  {
    ExternalUrlService service;
    if (ratingsScreenRegion != SplashScreen.RatingsScreenRegion.CHINA || !ServiceManager.TryGet<ExternalUrlService>(out service))
      return (IDataModel) null;
    return (IDataModel) new RatingsScreenDataModel()
    {
      Url = service.GetChinaRatingsWebsiteLink()
    };
  }

  public IEnumerator<IAsyncJobResult> Job_FadeOutSplashscreen()
  {
    SplashScreen splashScreen = this;
    float seconds = 0.5f;
    Hashtable args1 = iTween.Hash((object) "amount", (object) 0.0f, (object) "delay", (object) 0.0f, (object) "time", (object) seconds, (object) "easeType", (object) iTween.EaseType.linear, (object) "oncompletetarget", (object) splashScreen.gameObject);
    iTween.FadeTo(splashScreen.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "amount", (object) 0.0f, (object) "delay", (object) 0.0f, (object) "time", (object) seconds, (object) "easeType", (object) iTween.EaseType.linear, (object) "oncompletetarget", (object) splashScreen.gameObject);
    if ((UnityEngine.Object) splashScreen.m_glow1 != (UnityEngine.Object) null)
      iTween.FadeTo(splashScreen.m_glow1.gameObject, args2);
    Hashtable args3 = iTween.Hash((object) "amount", (object) 0.0f, (object) "delay", (object) 0.0f, (object) "time", (object) seconds, (object) "easeType", (object) iTween.EaseType.linear, (object) "oncompletetarget", (object) splashScreen.gameObject);
    if ((UnityEngine.Object) splashScreen.m_glow2 != (UnityEngine.Object) null)
      iTween.FadeTo(splashScreen.m_glow2.gameObject, args3);
    Processor.QueueJob("SplashScreen.FadeLogoOut", LogoAnimation.Get().Job_FadeLogoOut());
    yield return (IAsyncJobResult) new WaitForDuration(seconds);
    if ((UnityEngine.Object) splashScreen.m_glow1 != (UnityEngine.Object) null)
      splashScreen.m_glow1.gameObject.SetActive(false);
    if ((UnityEngine.Object) splashScreen.m_glow2 != (UnityEngine.Object) null)
      splashScreen.m_glow2.gameObject.SetActive(false);
  }

  private void OnSplashScreenFadeOutComplete() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  private void ShowDevicePerformanceWarning()
  {
    if (Vars.Key("Mobile.CheckNewMinSpec").GetBool(true) || Options.Get().GetBool(Option.HAS_SHOWN_DEVICE_PERFORMANCE_WARNING, false) || PlatformSettings.s_isDeviceInMinSpec)
      return;
    Options.Get().SetBool(Option.HAS_SHOWN_DEVICE_PERFORMANCE_WARNING, true);
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_DEVICE_PERFORMANCE_WARNING_TITLE"),
      m_text = GameStrings.Get("GLUE_DEVICE_PERFORMANCE_WARNING"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_iconSet = AlertPopup.PopupInfo.IconSet.None,
      m_confirmText = GameStrings.Get("GLOBAL_OKAY"),
      m_cancelText = GameStrings.Get("GLOBAL_SUPPORT"),
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, data) =>
      {
        if (response != AlertPopup.Response.CANCEL)
          return;
        Application.OpenURL(ExternalUrlService.Get().GetSystemRequirementsLink());
      })
    });
  }

  private void ShowGraphicsDeviceWarning()
  {
    if (PlatformSettings.RuntimeOS != OSCategory.Android || Options.Get().GetBool(Option.SHOWN_GFX_DEVICE_WARNING, false))
      return;
    Options.Get().SetBool(Option.SHOWN_GFX_DEVICE_WARNING, true);
    string lower = SystemInfo.graphicsDeviceName.ToLower();
    if (!lower.Contains("powervr") || !lower.Contains("540") && !lower.Contains("544"))
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_UNRELIABLE_GPU_WARNING_TITLE"),
      m_text = GameStrings.Get("GLUE_UNRELIABLE_GPU_WARNING"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_iconSet = AlertPopup.PopupInfo.IconSet.None,
      m_cancelText = GameStrings.Get("GLOBAL_SUPPORT"),
      m_confirmText = GameStrings.Get("GLOBAL_OKAY"),
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, data) =>
      {
        if (response != AlertPopup.Response.CANCEL)
          return;
        Application.OpenURL(ExternalUrlService.Get().GetSystemRequirementsLink());
      })
    });
  }

  private void ShowTextureCompressionWarning()
  {
    if (PlatformSettings.RuntimeOS != OSCategory.Android || !HearthstoneApplication.IsInternal() || PlatformSettings.LocaleVariant != LocaleVariant.China || AndroidDeviceSettings.Get().IsCurrentTextureFormatSupported())
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_TEXTURE_COMPRESSION_WARNING_TITLE"),
      m_text = GameStrings.Get("GLUE_TEXTURE_COMPRESSION_WARNING"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_iconSet = AlertPopup.PopupInfo.IconSet.None,
      m_cancelText = GameStrings.Get("GLOBAL_SUPPORT"),
      m_confirmText = GameStrings.Get("GLOBAL_OKAY"),
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, data) =>
      {
        if (response != AlertPopup.Response.CANCEL)
          return;
        Application.OpenURL("http://www.hearthstone.com.cn/download");
      })
    });
  }

  private void OnWillReset() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  private enum RatingsScreenRegion
  {
    NONE,
    KOREA,
    CHINA,
  }
}
