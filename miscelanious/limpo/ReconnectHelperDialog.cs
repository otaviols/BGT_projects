using Blizzard.T5.Core;
using Hearthstone;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconnectHelperDialog : DialogBase
{
  public UIBButton m_continueButton;
  public UIBButton m_choiceOneButton;
  public UIBButton m_choiceTwoButton;
  public GameObject m_continueButtonContainer;
  public GameObject m_choiceButtonContainer;
  public Spell m_successRingSpell;
  public GameObject m_successRingContainer;
  public GameObject m_reconnectPromptPanel;
  public GameObject m_reconnectInProgressPanel;
  public GameObject m_reconnectFailurePanel;
  public GameObject m_wifiDisabledPanel;
  public GameObject m_badVersionCanResetPanel;
  public GameObject m_badVersionUseLauncherPanel;
  public GameObject m_inactiveTimeoutPanel;
  public GameObject m_restartRequiredPanel;
  public UberText m_inProgressTextNormal;
  public UberText m_inProgressTextTimeout;
  private const float IN_PROGRESS_SPINNER_TIMEOUT_SECONDS = 20f;
  private List<GameObject> m_panels = new List<GameObject>();
  private Map<ReconnectHelperDialog.DialogState, ReconnectHelperDialog.Layout> m_stateLayouts = new Map<ReconnectHelperDialog.DialogState, ReconnectHelperDialog.Layout>();
  private ReconnectHelperDialog.DialogState m_state;
  private Action m_reconnectSuccessCallback;
  private Action m_goBackCallback;

  private void Start()
  {
    this.PopulatePanels();
    this.CreateStateMap();
    this.m_continueButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      Action continueButtonAction = this.m_stateLayouts[this.m_state].m_continueButtonAction;
      if (continueButtonAction == null)
        return;
      continueButtonAction();
    }));
    this.m_choiceOneButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      Action choiceButtonOneAction = this.m_stateLayouts[this.m_state].m_choiceButtonOneAction;
      if (choiceButtonOneAction == null)
        return;
      choiceButtonOneAction();
    }));
    this.m_choiceTwoButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      Action choiceButtonTwoAction = this.m_stateLayouts[this.m_state].m_choiceButtonTwoAction;
      if (choiceButtonTwoAction == null)
        return;
      choiceButtonTwoAction();
    }));
    this.ChangeStateToPromptBasedOnReconnectMgr();
    ReconnectMgr.Get().OnReconnectComplete += new Action(this.OnReconnectComplete);
  }

  private void Update()
  {
    if (this.m_state != ReconnectHelperDialog.DialogState.IN_PROGRESS)
      return;
    this.UpdateWhileInProgress();
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    ReconnectMgr reconnectMgr = ReconnectMgr.Get();
    if (reconnectMgr == null)
      return;
    reconnectMgr.OnReconnectComplete -= new Action(this.OnReconnectComplete);
  }

  public override void Show()
  {
    base.Show();
    BnetBar.Get().DisableButtonsByDialog((DialogBase) this);
    SoundManager.Get().LoadAndPlay((AssetReference) "Expand_Up.prefab:775d97ea42498c044897f396362b9db3");
    this.DoShowAnimation();
    DialogBase.DoBlur();
  }

  public override void Hide()
  {
    base.Hide();
    SoundManager.Get().LoadAndPlay((AssetReference) "Shrink_Down.prefab:a6d5184049ac041418cd5896e7d9a87a");
    DialogBase.EndBlur();
  }

  public void SetInfo(ReconnectHelperDialog.Info info)
  {
    this.m_reconnectSuccessCallback = info.m_reconnectSuccessCallback;
    this.m_goBackCallback = info.m_goBackCallback;
  }

  private void PopulatePanels()
  {
    this.m_panels.Add(this.m_reconnectPromptPanel);
    this.m_panels.Add(this.m_reconnectInProgressPanel);
    this.m_panels.Add(this.m_reconnectFailurePanel);
    this.m_panels.Add(this.m_wifiDisabledPanel);
    this.m_panels.Add(this.m_badVersionCanResetPanel);
    this.m_panels.Add(this.m_badVersionUseLauncherPanel);
    this.m_panels.Add(this.m_inactiveTimeoutPanel);
    this.m_panels.Add(this.m_restartRequiredPanel);
  }

  private void CreateStateMap()
  {
    this.m_stateLayouts[ReconnectHelperDialog.DialogState.PROMPT] = new ReconnectHelperDialog.Layout()
    {
      m_activePanel = this.m_reconnectPromptPanel,
      m_twoButtons = true,
      m_choiceOneButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CONFIRM"),
      m_choiceTwoButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CANCEL"),
      m_choiceButtonOneAction = new Action(this.OnReconnectButtonPressed),
      m_choiceButtonTwoAction = new Action(this.OnGoBackButtonPressed)
    };
    this.m_stateLayouts[ReconnectHelperDialog.DialogState.IN_PROGRESS] = new ReconnectHelperDialog.Layout()
    {
      m_activePanel = this.m_reconnectInProgressPanel,
      m_successRingState = SpellStateType.BIRTH,
      m_continueButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CANCEL"),
      m_continueButtonAction = new Action(this.OnCancelButtonPressed)
    };
    this.m_stateLayouts[ReconnectHelperDialog.DialogState.FAILURE] = new ReconnectHelperDialog.Layout()
    {
      m_activePanel = this.m_reconnectFailurePanel,
      m_twoButtons = true,
      m_successRingState = SpellStateType.DEATH,
      m_choiceOneButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CONFIRM"),
      m_choiceTwoButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CANCEL"),
      m_choiceButtonOneAction = new Action(this.OnReconnectButtonPressed),
      m_choiceButtonTwoAction = new Action(this.OnGoBackButtonPressed)
    };
    this.m_stateLayouts[ReconnectHelperDialog.DialogState.WIFI_DISABLED] = new ReconnectHelperDialog.Layout()
    {
      m_activePanel = this.m_wifiDisabledPanel,
      m_twoButtons = true,
      m_choiceOneButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CONFIRM"),
      m_choiceTwoButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CANCEL"),
      m_choiceButtonOneAction = new Action(this.OnReconnectButtonPressed),
      m_choiceButtonTwoAction = new Action(this.OnGoBackButtonPressed)
    };
    this.m_stateLayouts[ReconnectHelperDialog.DialogState.BAD_VERSION_CAN_RESET] = new ReconnectHelperDialog.Layout()
    {
      m_activePanel = this.m_badVersionCanResetPanel,
      m_twoButtons = true,
      m_choiceOneButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_UPDATE"),
      m_choiceTwoButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CANCEL"),
      m_choiceButtonOneAction = new Action(this.OnUpdateButtonPressed),
      m_choiceButtonTwoAction = new Action(this.OnGoBackButtonPressed)
    };
    this.m_stateLayouts[ReconnectHelperDialog.DialogState.BAD_VERSION_USE_LAUNCHER] = new ReconnectHelperDialog.Layout()
    {
      m_activePanel = this.m_badVersionUseLauncherPanel,
      m_twoButtons = false,
      m_choiceOneButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_EXIT_GAME"),
      m_choiceButtonOneAction = new Action(this.OnExitGameButtonPressed),
      m_choiceButtonTwoAction = new Action(this.OnGoBackButtonPressed)
    };
    this.m_stateLayouts[ReconnectHelperDialog.DialogState.INACTIVE_TIMEOUT] = new ReconnectHelperDialog.Layout()
    {
      m_activePanel = this.m_inactiveTimeoutPanel,
      m_twoButtons = true,
      m_choiceOneButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CONFIRM"),
      m_choiceTwoButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CANCEL"),
      m_choiceButtonOneAction = new Action(this.OnReconnectButtonPressed),
      m_choiceButtonTwoAction = new Action(this.OnGoBackButtonPressed)
    };
    this.m_stateLayouts[ReconnectHelperDialog.DialogState.RESTART_REQUIRED] = new ReconnectHelperDialog.Layout()
    {
      m_activePanel = this.m_restartRequiredPanel,
      m_twoButtons = true,
      m_choiceOneButtonText = GameStrings.Get((bool) HearthstoneApplication.AllowResetFromFatalError ? "GLUE_RECONNECT_HELPER_RESTART_GAME" : "GLUE_RECONNECT_HELPER_EXIT_GAME"),
      m_choiceTwoButtonText = GameStrings.Get("GLUE_RECONNECT_HELPER_CANCEL"),
      m_choiceButtonOneAction = new Action(this.OnExitGameButtonPressed),
      m_choiceButtonTwoAction = new Action(this.OnGoBackButtonPressed)
    };
  }

  private void ChangeState(ReconnectHelperDialog.DialogState state)
  {
    if (state == this.m_state || (UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
      return;
    this.m_state = state;
    this.LoadState();
  }

  private void LoadState()
  {
    ReconnectHelperDialog.Layout stateLayout = this.m_stateLayouts[this.m_state];
    this.m_continueButton.SetText(stateLayout.m_continueButtonText);
    this.m_choiceOneButton.SetText(stateLayout.m_choiceOneButtonText);
    this.m_choiceTwoButton.SetText(stateLayout.m_choiceTwoButtonText);
    this.m_continueButtonContainer.SetActive(!stateLayout.m_twoButtons);
    this.m_choiceButtonContainer.SetActive(stateLayout.m_twoButtons);
    this.m_successRingContainer.SetActive(stateLayout.m_successRingState != 0);
    if (stateLayout.m_successRingState != SpellStateType.NONE)
      this.m_successRingSpell.ActivateState(stateLayout.m_successRingState);
    for (int index = 0; index < this.m_panels.Count; ++index)
    {
      GameObject panel = this.m_panels[index];
      panel.SetActive((UnityEngine.Object) panel == (UnityEngine.Object) stateLayout.m_activePanel);
    }
    if (stateLayout.m_onInit == null)
      return;
    stateLayout.m_onInit();
  }

  private void ChangeStateToPromptBasedOnReconnectMgr()
  {
    if (ReconnectMgr.Get().FullResetRequired)
      this.ChangeState_FullResetRequired();
    else if (InactivePlayerKicker.Get().WasKickedForInactivity)
      this.ChangeState(ReconnectHelperDialog.DialogState.INACTIVE_TIMEOUT);
    else
      this.ChangeState(ReconnectHelperDialog.DialogState.PROMPT);
  }

  private void ChangeState_FullResetRequired()
  {
    if (ReconnectMgr.Get().UpdateRequired)
      this.ChangeState((bool) HearthstoneApplication.AllowResetFromFatalError ? ReconnectHelperDialog.DialogState.BAD_VERSION_CAN_RESET : ReconnectHelperDialog.DialogState.BAD_VERSION_USE_LAUNCHER);
    else
      this.ChangeState(ReconnectHelperDialog.DialogState.RESTART_REQUIRED);
  }

  private void OnReconnectButtonPressed()
  {
    if (Network.IsLoggedIn())
      this.OnReconnectSuccess();
    else if (!NetworkReachabilityManager.InternetAvailable)
    {
      this.ChangeState(ReconnectHelperDialog.DialogState.WIFI_DISABLED);
    }
    else
    {
      this.ChangeToInProgressState();
      ReconnectMgr.Get().StartUtilReconnect();
    }
  }

  private void OnGoBackButtonPressed() => this.OnGiveUpReconnecting();

  private void OnCancelButtonPressed() => this.OnGiveUpReconnecting();

  private void OnUpdateButtonPressed()
  {
    if ((bool) HearthstoneApplication.AllowResetFromFatalError)
      HearthstoneApplication.Get().Reset();
    else
      HearthstoneApplication.Get().Exit();
  }

  private void OnExitGameButtonPressed()
  {
    if ((bool) HearthstoneApplication.AllowResetFromFatalError)
      HearthstoneApplication.Get().Reset();
    else
      HearthstoneApplication.Get().Exit();
  }

  private void OnReconnectSuccess()
  {
    ReconnectMgr.Get().SetNextReLoginCallback(this.m_reconnectSuccessCallback);
    this.Hide();
  }

  private void OnGiveUpReconnecting()
  {
    ReconnectMgr.Get().SetNextReLoginCallback((Action) null);
    if (this.m_goBackCallback != null)
      this.m_goBackCallback();
    this.Hide();
  }

  private void OnReconnectComplete()
  {
    if (this.m_state != ReconnectHelperDialog.DialogState.IN_PROGRESS)
      return;
    this.OnReconnectSuccess();
  }

  private void ChangeToInProgressState()
  {
    this.ChangeState(ReconnectHelperDialog.DialogState.IN_PROGRESS);
    this.SetInProgressText(false);
    this.StopAllCoroutines();
    this.StartCoroutine(this.WaitThenSwitchInProgressText());
  }

  private IEnumerator WaitThenSwitchInProgressText()
  {
    yield return (object) new WaitForSeconds(20f);
    this.SetInProgressText(true);
  }

  private void SetInProgressText(bool hasTimedOut)
  {
    this.m_inProgressTextNormal.gameObject.SetActive(!hasTimedOut);
    this.m_inProgressTextTimeout.gameObject.SetActive(hasTimedOut);
  }

  private void UpdateWhileInProgress()
  {
    if (!ReconnectMgr.Get().FullResetRequired)
      return;
    this.ChangeState_FullResetRequired();
  }

  public class Info
  {
    public Action m_reconnectSuccessCallback;
    public Action m_goBackCallback;
  }

  private enum DialogState
  {
    INVALID,
    PROMPT,
    IN_PROGRESS,
    FAILURE,
    WIFI_DISABLED,
    BAD_VERSION_CAN_RESET,
    BAD_VERSION_USE_LAUNCHER,
    INACTIVE_TIMEOUT,
    RESTART_REQUIRED,
  }

  private class Layout
  {
    public SpellStateType m_successRingState;
    public bool m_twoButtons;
    public GameObject m_activePanel;
    public string m_continueButtonText = "";
    public string m_choiceOneButtonText = "";
    public string m_choiceTwoButtonText = "";
    public Action m_continueButtonAction;
    public Action m_choiceButtonOneAction;
    public Action m_choiceButtonTwoAction;
    public Action m_onInit;
  }
}
