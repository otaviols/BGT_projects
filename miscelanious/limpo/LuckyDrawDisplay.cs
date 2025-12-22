using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class LuckyDrawDisplay : AbsSceneDisplay
{
  [Header("Lucky Draw Display")]
  public AsyncReference m_LuckyDrawWidgetReference;
  public Transform m_OffScreenBonePC;
  public Transform m_OnScreenBonePC;
  public Transform m_OffScreenBoneMobile;
  public Transform m_OnScreenBoneMobile;
  private LuckyDrawWidget m_luckyDrawWidget;
  private LuckyDrawManager m_luckyDrawManager;
  private bool m_luckyDrawWidgetFinishedLoading;

  private void Awake()
  {
    this.InitSlidingTray();
    this.InitializeLuckyDrawManager();
  }

  private void InitSlidingTray()
  {
    if ((UnityEngine.Object) this.m_slidingTray == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error", "Warning [LuckyDrawDisplay] InitSlidingTray() reference to the sliding tray is missing! This may lead to an improper layout.");
    else if (PlatformSettings.IsMobile())
    {
      this.m_slidingTray.m_trayHiddenBone = this.m_OffScreenBoneMobile;
      this.m_slidingTray.m_trayShownBone = this.m_OnScreenBoneMobile;
    }
    else
    {
      this.m_slidingTray.m_trayHiddenBone = this.m_OffScreenBonePC;
      this.m_slidingTray.m_trayShownBone = this.m_OnScreenBonePC;
    }
  }

  private void InitializeLuckyDrawManager()
  {
    this.m_luckyDrawManager = LuckyDrawManager.Get();
    if (this.m_luckyDrawManager == null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawDisplay] InitailizeLuckyDrawManager() lucky draw manager is null");
    }
    else
    {
      if (this.m_luckyDrawManager.IsIntialized())
        return;
      this.m_luckyDrawManager.InitializeOrUpdateData();
    }
  }

  public override void Start()
  {
    base.Start();
    this.m_luckyDrawManager.RegisterOnEventEndsListeners(new Action(this.OnLuckyDrawEventEnds));
    this.StartCoroutine(this.WaitForDataAndInitializeWidget());
    this.m_sceneDisplayWidget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "Button_Framed_Clicked"))
        return;
      this.OnBackButtonReleased();
    }));
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.FTUE, GameSaveKeySubkeyId.FTUE_HAS_SEEN_BATTLE_BASH_BUTTON_TOOLTIP, new long[1]
    {
      1L
    }));
    this.StartCoroutine(this.InitializeSceneObjects());
  }

  private IEnumerator InitializeSceneObjects()
  {
    while (SceneMgr.Get().IsTransitionNowOrPending() || !this.m_luckyDrawWidgetFinishedLoading)
      yield return (object) null;
    yield return (object) new WaitForSeconds(0.5f);
    this.m_luckyDrawWidget.DisplayFirstHammerPopup();
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_BattleBash);
  }

  private IEnumerator WaitForDataAndInitializeWidget()
  {
    LuckyDrawDisplay luckyDrawDisplay = this;
    while (luckyDrawDisplay.m_luckyDrawManager.IsDataDirty())
      yield return (object) new WaitForSeconds(0.1f);
    luckyDrawDisplay.m_LuckyDrawWidgetReference.RegisterReadyListener<WidgetInstance>(new Action<WidgetInstance>(luckyDrawDisplay.OnLuckyDrawWidgetReady));
  }

  private void OnLuckyDrawWidgetReady(WidgetInstance widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawDisplay] OnLuckyDrawWidgetReady() widget was null!");
    }
    else
    {
      this.m_luckyDrawWidget = widget.GetComponentInChildren<LuckyDrawWidget>();
      Widget component = this.m_luckyDrawWidget?.GetComponent<Widget>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Error.AddDevWarning("UI Error", "[LuckyDrawDisplay] OnLuckyDrawWidgetReady() could not find Widget on m_luckyDrawWidget!");
      }
      else
      {
        this.m_luckyDrawManager.SetShowHighlight(false);
        TelemetryManager.Client()?.SendLuckyDrawEventMessage("LuckyDrawPageEntered");
        this.m_luckyDrawManager.BindAllLuckyDrawDataModelToWidget(component);
        this.m_luckyDrawWidget.Show();
        this.m_luckyDrawWidgetFinishedLoading = true;
      }
    }
  }

  private void OnBackButtonReleased() => this.ReturnToBaconScene();

  private void ReturnToBaconScene() => this.SetNextModeAndHandleTransition(SceneMgr.Mode.BACON, SceneMgr.TransitionHandlerType.NEXT_SCENE, (object) null);

  private void OnLuckyDrawEventEnds()
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.LUCKY_DRAW)
      return;
    this.StartCoroutine(this.WaitThenShowBattleBashEndedPopupThenReturnToBaconScene());
  }

  private IEnumerator WaitThenShowBattleBashEndedPopupThenReturnToBaconScene()
  {
    LuckyDrawManager luckyDrawManager = LuckyDrawManager.Get();
    while (luckyDrawManager.IsDataDirty())
      yield return (object) new WaitForSeconds(0.1f);
    string key = luckyDrawManager.GetBattlegroundsLuckyDrawDataModel().Hammers > 0 ? "GLUE_BATTLEBASH_ALERT_EVENT_END_DESCRIPTION" : "GLUE_BATTLEBASH_ALERT_EVENT_END_DESCRIPTION_NO_HAMMERS";
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_BATTLEBASH_ALERT_EVENT_END_TITLE"),
      m_text = GameStrings.Get(key),
      m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
      m_showAlertIcon = true,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
    this.ReturnToBaconScene();
  }

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.m_luckyDrawWidgetFinishedLoading)
    {
      failureMessage = "LuckyDrawDisplay - LuckyDrawWidget never loaded";
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  protected override bool ShouldStartShown() => true;

  private void OnDestroy()
  {
    if (this.m_luckyDrawManager == null)
      return;
    this.m_luckyDrawManager.RemoveOnEventEndsListenders(new Action(this.OnLuckyDrawEventEnds));
  }
}
