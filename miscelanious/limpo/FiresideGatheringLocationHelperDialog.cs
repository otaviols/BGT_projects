using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FiresideGatheringLocationHelperDialog : DialogBase
{
  public UIBButton m_continueButton;
  public UIBButton m_choiceOneButton;
  public UIBButton m_choiceTwoButton;
  public GameObject m_continueButtonContainer;
  public GameObject m_choiceButtonContainer;
  public UIBButton m_innkeeperSuccessAddAccessPointsButton;
  public UIBButton m_innkeeperSuccessFinishButton;
  public UberText m_innkeeperSuccessText;
  public UIBButton m_searchingForFSGFailureLearnMoreButton;
  public UIBButton m_searchingForFSGFailureOkButton;
  public Spell m_successRingSpell;
  public GameObject m_successRingContainer;
  public GameObject m_gpsOffIntroPanel;
  public GameObject m_gpsSearchingPanel;
  public GameObject m_gpsSuccessPanel;
  public GameObject m_gpsFailurePanel;
  public GameObject m_wifiOffIntroPanel;
  public GameObject m_waitingForWifiPanel;
  public GameObject m_networkConfirmPanel;
  public GameObject m_accessPointPanel;
  public GameObject m_unpackingTavernPanel;
  public GameObject m_unpackFailedPanel;
  public GameObject m_searchingForFSGsPanel;
  public GameObject m_innkeeperSuccessPanel;
  public GameObject m_searchingForFSGFailurePanel;
  public UberText m_gpsIntroText;
  public UberText m_wifiOffIntroText;
  public UberText m_waitingForWifiText;
  public UberText m_networkNameText;
  public UberText m_accessPointsText;
  public UberText m_numAccessPointsText;
  public UberText m_wifiConfirmText;
  public UberText m_searchFailureBodyText;
  private Action m_completedCallback;
  private bool m_isInnkeeperSetup;
  private bool m_isCheckInFailure;
  private bool m_provideWifiForTavern = true;
  private HashSet<string> m_innkeeperCollectedBSSIDS = new HashSet<string>();
  private FiresideGatheringLocationHelperDialog.DialogState m_state;
  private Map<FiresideGatheringLocationHelperDialog.DialogState, FiresideGatheringLocationHelperDialog.Layout> m_stateLayouts = new Map<FiresideGatheringLocationHelperDialog.DialogState, FiresideGatheringLocationHelperDialog.Layout>();
  private List<GameObject> m_panels = new List<GameObject>();
  private double m_searchStartTimestamp = double.MaxValue;
  private float m_wifiCheckTimer;
  private float m_wifiCheckCadence = 5f;
  private float m_fsgSearchTimer;
  private float m_fsgSearchTimeMaximum = 20f;

  private void Start()
  {
    this.PopulatePanels();
    this.CreateStateMap();
    FiresideGatheringManager gatheringManager = FiresideGatheringManager.Get();
    gatheringManager.OnNearbyFSGsChanged += new FiresideGatheringManager.NearbyFSGsChangedCallback(this.OnFSGSearchComplete);
    gatheringManager.OnInnkeeperSetupFinished += new FiresideGatheringManager.OnInnkeeperSetupFinishedCallback(this.OnInnkeeperSetupFinished);
    gatheringManager.PlayerAccountShouldAutoCheckin.Set(true);
    gatheringManager.RequestFSGNotificationAndCheckinsHalt();
    this.m_continueButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.m_stateLayouts[this.m_state].m_continueButtonAction()));
    this.m_choiceOneButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.m_stateLayouts[this.m_state].m_choiceButtonOneAction()));
    this.m_choiceTwoButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.m_stateLayouts[this.m_state].m_choiceButtonTwoAction()));
    this.m_innkeeperSuccessAddAccessPointsButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.COLLECTING_ACCESS_POINTS)));
    this.m_innkeeperSuccessFinishButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnInnkeeperSuccessOk()));
    this.m_searchingForFSGFailureLearnMoreButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnSearchFailedLearnMore()));
    this.m_searchingForFSGFailureOkButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnSearchFailedOk()));
    if (this.m_isCheckInFailure)
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS_FAILURE);
    else if (ClientLocationManager.Get().GPSAvailable && FiresideGatheringManager.IsGpsFeatureEnabled)
    {
      if (!ClientLocationManager.Get().GPSServicesEnabled)
        this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.GPS_INTRO);
      else
        this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_GPS);
    }
    else if (!string.IsNullOrEmpty(ClientLocationManager.Get().GetWifiSSID))
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.NETWORK_CONFIRM);
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.WIFI_INTRO);
  }

  private void Update()
  {
    if (this.m_state != FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS)
    {
      FiresideGatheringManager.Get().RequestFSGNotificationAndCheckinsHalt();
    }
    else
    {
      this.m_fsgSearchTimer += Time.deltaTime;
      if ((double) this.m_fsgSearchTimer > (double) this.m_fsgSearchTimeMaximum)
      {
        this.OnFSGSearchComplete();
        this.m_fsgSearchTimer = 0.0f;
      }
    }
    if (this.m_state != FiresideGatheringLocationHelperDialog.DialogState.NETWORK_CONFIRM)
      return;
    if ((double) this.m_wifiCheckTimer > (double) this.m_wifiCheckCadence)
    {
      this.m_wifiCheckTimer = 0.0f;
      this.DoWifiConnectedCheck();
    }
    else
      this.m_wifiCheckTimer += Time.deltaTime;
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
    FiresideGatheringManager.Get().OnNearbyFSGsChanged -= new FiresideGatheringManager.NearbyFSGsChangedCallback(this.OnFSGSearchComplete);
    FiresideGatheringManager.Get().OnInnkeeperSetupFinished -= new FiresideGatheringManager.OnInnkeeperSetupFinishedCallback(this.OnInnkeeperSetupFinished);
    DialogBase.EndBlur();
  }

  public void SetInfo(FiresideGatheringLocationHelperDialog.Info info)
  {
    this.m_completedCallback = info.m_callback;
    this.m_gpsIntroText.Text = info.m_gpsOffIntroText;
    this.m_wifiOffIntroText.Text = info.m_wifiOffIntroText;
    this.m_waitingForWifiText.Text = info.m_waitingForWifiText;
    this.m_wifiConfirmText.Text = info.m_wifiConfirmText;
    this.m_isInnkeeperSetup = info.m_isInnkeeperSetup;
    this.m_isCheckInFailure = info.m_isCheckInFailure;
    string key = "GLUE_FIRESIDE_GATHERING_SEARCH_FAILURE_BODY";
    if ((bool) UniversalInputManager.UsePhoneUI)
      key += "_PHONE";
    this.m_searchFailureBodyText.Text = GameStrings.Get(key);
  }

  private void PopulatePanels()
  {
    this.m_panels.Add(this.m_gpsOffIntroPanel);
    this.m_panels.Add(this.m_gpsSearchingPanel);
    this.m_panels.Add(this.m_gpsSuccessPanel);
    this.m_panels.Add(this.m_gpsFailurePanel);
    this.m_panels.Add(this.m_wifiOffIntroPanel);
    this.m_panels.Add(this.m_waitingForWifiPanel);
    this.m_panels.Add(this.m_networkConfirmPanel);
    this.m_panels.Add(this.m_accessPointPanel);
    this.m_panels.Add(this.m_unpackingTavernPanel);
    this.m_panels.Add(this.m_unpackFailedPanel);
    this.m_panels.Add(this.m_searchingForFSGsPanel);
    this.m_panels.Add(this.m_innkeeperSuccessPanel);
    this.m_panels.Add(this.m_searchingForFSGFailurePanel);
  }

  private void CreateStateMap()
  {
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.GPS_INTRO] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_gpsOffIntroPanel,
      m_continueButtonText = GameStrings.Get("GLOBAL_BUTTON_NEXT"),
      m_continueButtonAction = new Action(this.OnGPSIntroContinue)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_GPS] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_gpsSearchingPanel,
      m_successRingState = SpellStateType.BIRTH,
      m_continueButtonText = GameStrings.Get("GLOBAL_CANCEL"),
      m_continueButtonAction = new Action(this.OnSearchingForGPSCancel),
      m_onInit = new Action(this.DoGPSRequest)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.GPS_SUCCESS] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_gpsSuccessPanel,
      m_successRingState = SpellStateType.ACTION,
      m_continueButtonText = GameStrings.Get("GLOBAL_BUTTON_NEXT"),
      m_continueButtonAction = new Action(this.OnGPSSuccessNext)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.GPS_FAILURE] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_gpsFailurePanel,
      m_twoButtons = true,
      m_successRingState = SpellStateType.DEATH,
      m_choiceOneButtonText = GameStrings.Get("GLOBAL_RETRY"),
      m_choiceTwoButtonText = GameStrings.Get("GLOBAL_SKIP"),
      m_choiceButtonOneAction = new Action(this.OnGPSFailureRetry),
      m_choiceButtonTwoAction = new Action(this.OnGPSFailureSkip)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.WIFI_INTRO] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_wifiOffIntroPanel,
      m_continueButtonText = GameStrings.Get("GLOBAL_BUTTON_NEXT"),
      m_continueButtonAction = new Action(this.OnWifiIntroNext)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.WAITING_FOR_WIFI] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_waitingForWifiPanel,
      m_twoButtons = true,
      m_choiceOneButtonText = GameStrings.Get("GLOBAL_SKIP"),
      m_choiceTwoButtonText = GameStrings.Get("GLOBAL_REFRESH"),
      m_choiceButtonOneAction = new Action(this.OnWaitingForWifiSkip),
      m_choiceButtonTwoAction = new Action(this.OnWaitingForWifiRefresh),
      m_onInit = new Action(this.DoWifiRequest)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.NETWORK_CONFIRM] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_networkConfirmPanel,
      m_twoButtons = this.m_isInnkeeperSetup,
      m_continueButtonText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_CONNECT_TO_WIFI_USE_WIFI_BUTTON"),
      m_choiceOneButtonText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_CONNECT_TO_WIFI_NO_WIFI_BUTTON"),
      m_choiceTwoButtonText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_CONNECT_TO_WIFI_USE_WIFI_BUTTON"),
      m_continueButtonAction = new Action(this.OnNetworkConfirmAccept),
      m_choiceButtonOneAction = new Action(this.OnNetworkConfirmCancel),
      m_choiceButtonTwoAction = new Action(this.OnNetworkConfirmAccept),
      m_onInit = new Action(this.DoWifiConnectedCheck)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.SETTING_UP_TAVERN] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_unpackingTavernPanel,
      m_successRingState = SpellStateType.BIRTH,
      m_continueButtonText = GameStrings.Get("GLOBAL_CANCEL"),
      m_continueButtonAction = new Action(this.OnTavernSetupCancel),
      m_onInit = new Action(this.DoTavernSetup)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.INNKEEPER_SUCCESS] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_innkeeperSuccessPanel,
      m_onInit = new Action(this.InnkeeperSuccessSetup)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.COLLECTING_ACCESS_POINTS] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_accessPointPanel,
      m_continueButtonText = GameStrings.Get("GLOBAL_FINISH"),
      m_continueButtonAction = new Action(this.OnAccessPointsDone),
      m_onInit = new Action(this.DoAccessPointSearch)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.UNPACK_FAILED] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_unpackFailedPanel,
      m_successRingState = SpellStateType.DEATH,
      m_continueButtonText = GameStrings.Get("GLOBAL_OK"),
      m_continueButtonAction = new Action(this.OnUnpackFailureOk)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_searchingForFSGsPanel,
      m_successRingState = SpellStateType.BIRTH,
      m_continueButtonText = GameStrings.Get("GLOBAL_CANCEL"),
      m_continueButtonAction = new Action(this.OnSearchingForFSGsCancel),
      m_onInit = new Action(this.BeginSearchForFSGs)
    };
    this.m_stateLayouts[FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS_FAILURE] = new FiresideGatheringLocationHelperDialog.Layout()
    {
      m_activePanel = this.m_searchingForFSGFailurePanel,
      m_onInit = new Action(this.OnFSGSearchFailure)
    };
  }

  private void ChangeState(
    FiresideGatheringLocationHelperDialog.DialogState state)
  {
    if (state == this.m_state || (UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
      return;
    this.m_state = state;
    this.LoadState();
  }

  private void LoadState()
  {
    FiresideGatheringLocationHelperDialog.Layout stateLayout = this.m_stateLayouts[this.m_state];
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

  private void DoWifiRequest()
  {
    Log.FiresideGatherings.Print("FiresideGatheringLocationHelperDialog.DoWifiRequest");
    if (!ClientLocationManager.Get().WifiEnabled)
    {
      Log.FiresideGatherings.Print("FiresideGatheringLocationHelperDialog.DoWifiRequest Requesting WIFI permission");
      MobilePermissionsManager.Get().RequestPermission(MobilePermission.WIFI, new MobilePermissionsManager.PermissionResultCallback(this.DoWifiRequest_OnPermissionRequestResponse));
    }
    else
    {
      Log.FiresideGatherings.Print("FiresideGatheringLocationHelperDialog.DoWifiRequest Sent wifi data request");
      this.SendRequestWifiData();
    }
  }

  private void DoWifiRequest_OnPermissionRequestResponse(MobilePermission permission, bool granted)
  {
    if (!granted)
      return;
    this.SendRequestWifiData();
  }

  private void SendRequestWifiData() => ClientLocationManager.Get().RequestWifiData(new Action<ClientLocationData>(this.OnLocationDataWIFIUpdated), (Action) (() =>
  {
    if (this.m_state == FiresideGatheringLocationHelperDialog.DialogState.WAITING_FOR_WIFI && !string.IsNullOrEmpty(ClientLocationManager.Get().GetWifiSSID))
    {
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.NETWORK_CONFIRM);
    }
    else
    {
      if (this.m_state != FiresideGatheringLocationHelperDialog.DialogState.COLLECTING_ACCESS_POINTS)
        return;
      this.DoWifiRequest();
    }
  }));

  private void DoGPSRequest()
  {
    if (!ClientLocationManager.Get().GPSServicesEnabled)
      MobilePermissionsManager.Get().RequestPermission(MobilePermission.FINE_LOCATION, new MobilePermissionsManager.PermissionResultCallback(this.DoGPSRequest_OnPermissionRequestResponse));
    else
      this.SendRequestGPSData();
  }

  private void DoGPSRequest_OnPermissionRequestResponse(MobilePermission permission, bool granted)
  {
    if (granted)
      this.SendRequestGPSData();
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.GPS_FAILURE);
  }

  private void SendRequestGPSData()
  {
    this.m_searchStartTimestamp = TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;
    ClientLocationManager.Get().RequestGPSData(new Action<ClientLocationData>(this.OnLocationDataGPSUpdated), (Action) (() =>
    {
      ClientLocationData bestLocationData = ClientLocationManager.Get().GetBestLocationData();
      if (this.m_state != FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_GPS)
        return;
      if (bestLocationData.location == null || bestLocationData.location.Timestamp < this.m_searchStartTimestamp || !FiresideGatheringManager.Get().IsGpsLocationValid)
        this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.GPS_FAILURE);
      else
        this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.GPS_SUCCESS);
    }));
  }

  private void OnLocationDataGPSUpdated(ClientLocationData data) => FiresideGatheringManager.Get().OnLocationDataGPSUpdate(data);

  private void OnLocationDataWIFIUpdated(ClientLocationData data)
  {
    FiresideGatheringManager.Get().OnLocationDataWIFIUpdate(data);
    if (this.m_state != FiresideGatheringLocationHelperDialog.DialogState.COLLECTING_ACCESS_POINTS)
      return;
    this.DoAccessPointUpdate();
  }

  private void DoWifiConnectedCheck()
  {
    if (ClientLocationManager.Get().WifiEnabled)
    {
      this.DoWifiRequest();
      this.m_networkNameText.Text = ClientLocationManager.Get().GetWifiSSID;
    }
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.WAITING_FOR_WIFI);
  }

  private void DoAccessPointUpdate(ClientLocationData data = null)
  {
    ClientLocationData bestLocationData = ClientLocationManager.Get().GetBestLocationData();
    FiresideGatheringManager.Get().AddWIFIAccessPoints(bestLocationData);
    foreach (AccessPointInfo accessPointSample in bestLocationData.accessPointSamples)
    {
      if (FiresideGatheringManager.IsValidBSSID(accessPointSample.bssid))
        this.m_innkeeperCollectedBSSIDS.Add(accessPointSample.bssid);
    }
    int count = this.m_innkeeperCollectedBSSIDS.Count;
    this.m_numAccessPointsText.Text = count.ToString();
    string key = "GLUE_FIRESIDE_GATHERING_WIFI_ACCESS_POINTS_SEARCH_TITLE";
    if (count == 1)
      key = "GLUE_FIRESIDE_GATHERING_WIFI_ACCESS_POINT_SEARCH_TITLE";
    this.m_accessPointsText.Text = GameStrings.Get(key);
  }

  private void DoAccessPointSearch()
  {
    if (this.m_state != FiresideGatheringLocationHelperDialog.DialogState.COLLECTING_ACCESS_POINTS)
      return;
    this.DoAccessPointUpdate();
    ClientLocationManager.Get().RequestWifiData(new Action<ClientLocationData>(this.DoAccessPointUpdate), new Action(this.DoAccessPointSearch));
  }

  private void DoTavernSetup() => FiresideGatheringManager.Get().InnkeeperSetupFSG(this.m_provideWifiForTavern);

  private void InnkeeperSuccessSetup()
  {
    if (!this.m_provideWifiForTavern)
      this.m_innkeeperSuccessAddAccessPointsButton.Flip(this.m_provideWifiForTavern);
    this.m_innkeeperSuccessAddAccessPointsButton.SetEnabled(this.m_provideWifiForTavern);
    SoundManager.Get().LoadAndPlay((AssetReference) "tavern_crowd_play_reaction_very_positive_3.prefab:30519a2212fbd18499c08fb02ba05c81");
    string str = "GLUE_FIRESIDE_GATHERING_WIFI_SUCCESS";
    if ((bool) UniversalInputManager.UsePhoneUI)
      str += "_PHONE";
    this.m_innkeeperSuccessText.Text = GameStrings.Get(this.m_provideWifiForTavern ? str : "GLUE_FIRESIDE_GATHERING_NO_WIFI_SUCCESS");
  }

  private void BeginSearchForFSGs()
  {
    FiresideGatheringManager.Get().ClearErrorOccuredOnCheckIn();
    FiresideGatheringManager.Get().RequestNearbyFSGs();
  }

  private void OnSearchFailedLearnMore()
  {
    this.Done();
    FiresideGatheringManager.Get().GotoFSGLink();
  }

  private void OnSearchFailedOk() => this.Done();

  private void OnFSGSearchComplete()
  {
    if (this.m_state != FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS)
      return;
    if (FiresideGatheringManager.Get().GetFSGs().Count < 1)
    {
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS_FAILURE);
    }
    else
    {
      this.Done();
      FiresideGatheringManager.Get().SetWaitingForCheckIn();
    }
  }

  private void OnInnkeeperSetupFinished(bool success)
  {
    if (!success)
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.UNPACK_FAILED);
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.INNKEEPER_SUCCESS);
  }

  private void OnFSGSearchFailure()
  {
    this.m_continueButtonContainer.SetActive(false);
    this.m_choiceButtonContainer.SetActive(false);
  }

  private void Done()
  {
    this.Hide();
    if (this.m_completedCallback == null)
      return;
    this.m_completedCallback();
  }

  private void OnGPSIntroContinue() => this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_GPS);

  private void OnSearchingForGPSCancel() => this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.GPS_FAILURE);

  private void OnGPSSuccessNext()
  {
    if (!FiresideGatheringManager.IsWifiFeatureEnabled)
    {
      if (this.m_isInnkeeperSetup)
        this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SETTING_UP_TAVERN);
      else
        this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS);
    }
    else if (!string.IsNullOrEmpty(ClientLocationManager.Get().GetWifiSSID))
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.NETWORK_CONFIRM);
    else if (ClientLocationManager.Get().WifiEnabled)
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.WAITING_FOR_WIFI);
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.WIFI_INTRO);
  }

  private void OnGPSFailureRetry() => this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_GPS);

  private void OnGPSFailureSkip()
  {
    if (!FiresideGatheringManager.IsWifiFeatureEnabled || MobilePermissionsManager.Get().WifiRequiresLocationPermission())
    {
      if (this.m_isInnkeeperSetup)
      {
        this.m_provideWifiForTavern = false;
        this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SETTING_UP_TAVERN);
      }
      else
        this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS);
    }
    else if (!string.IsNullOrEmpty(ClientLocationManager.Get().GetWifiSSID))
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.NETWORK_CONFIRM);
    else if (ClientLocationManager.Get().WifiEnabled)
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.WAITING_FOR_WIFI);
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.WIFI_INTRO);
  }

  private void OnWifiIntroNext()
  {
    if (!string.IsNullOrEmpty(ClientLocationManager.Get().GetWifiSSID))
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.NETWORK_CONFIRM);
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.WAITING_FOR_WIFI);
  }

  private void OnWaitingForWifiSkip()
  {
    if (this.m_isInnkeeperSetup)
    {
      this.m_provideWifiForTavern = false;
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SETTING_UP_TAVERN);
    }
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS);
  }

  private void OnWaitingForWifiRefresh() => this.DoWifiRequest();

  private void OnNetworkConfirmCancel()
  {
    if (this.m_isInnkeeperSetup)
    {
      this.m_provideWifiForTavern = false;
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SETTING_UP_TAVERN);
    }
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS);
  }

  private void OnNetworkConfirmAccept()
  {
    if (this.m_isInnkeeperSetup)
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SETTING_UP_TAVERN);
    else
      this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SEARCHING_FOR_FSGS);
  }

  private void OnAccessPointsDone() => this.ChangeState(FiresideGatheringLocationHelperDialog.DialogState.SETTING_UP_TAVERN);

  private void OnTavernSetupCancel() => this.Done();

  private void OnSearchingForFSGsCancel() => this.Done();

  private void OnUnpackFailureOk() => this.Done();

  private void OnInnkeeperSuccessOk()
  {
    this.Done();
    FiresideGatheringManager.Get().SetWaitingForCheckIn();
  }

  public class Info
  {
    public Action m_callback;
    public string m_gpsOffIntroText;
    public string m_wifiOffIntroText;
    public string m_waitingForWifiText;
    public string m_wifiConfirmText;
    public bool m_isInnkeeperSetup;
    public bool m_isCheckInFailure;
  }

  private enum DialogState
  {
    INVALID,
    GPS_INTRO,
    SEARCHING_FOR_GPS,
    GPS_SUCCESS,
    GPS_FAILURE,
    WIFI_INTRO,
    WAITING_FOR_WIFI,
    NETWORK_CONFIRM,
    INNKEEPER_SUCCESS,
    COLLECTING_ACCESS_POINTS,
    SETTING_UP_TAVERN,
    UNPACK_FAILED,
    SEARCHING_FOR_FSGS,
    SEARCHING_FOR_FSGS_FAILURE,
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
