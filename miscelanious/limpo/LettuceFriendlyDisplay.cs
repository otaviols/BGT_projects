using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLettuce;
using System.Collections;
using System.Collections.Generic;

public class LettuceFriendlyDisplay : AbsSceneDisplay, IMercDetailsDisplayProvider
{
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_PlayButtonPhoneReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_BackButtonPhoneReference;
  public AsyncReference m_SharingButtonReference;
  public AsyncReference m_TeamPreviewReference;
  public AsyncReference m_TeamPreviewPhoneReference;
  public AsyncReference m_TeamListDisplay;
  public AsyncReference m_MercDetailsDisplayReference;
  private PlayButton m_playButton;
  private UIBButton m_backButton;
  private UIBButton m_sharingButton;
  private Widget m_sharingButtonWidget;
  private VisualController m_teamListVisualController;
  private Widget m_teamPreviewWidget;
  private bool m_playButtonFinishedLoading;
  private bool m_backButtonFinishedLoading;
  private bool m_sharingButtonFinishedLoading;
  private bool m_teamListFinishedLoading;
  private bool m_teamPreviewFinishedLoading;
  private bool m_detailsDisplayFinishedLoading;
  private LettuceTeam m_selectedTeam;
  private bool m_isTeamLockedIn;
  private long m_opponentsSelectedTeam;
  private List<LettuceTeam> m_teamListInUse;
  private List<LettuceTeam> m_remoteTeamList;
  private PartyManager.MercTeamShareState m_teamSharingState;
  private bool m_isSharingButtonFlipped;
  private bool m_isSharingButtonDisabled;

  public MercenaryDetailDisplay MercenaryDetailDisplay { get; private set; }

  private bool IsSharingButtonFlipped
  {
    get => this.m_isSharingButtonFlipped;
    set
    {
      if (value == this.m_isSharingButtonFlipped)
        return;
      this.m_isSharingButtonFlipped = value;
      this.m_sharingButtonWidget.TriggerEvent(this.m_isSharingButtonFlipped || this.m_isSharingButtonDisabled ? "INACTIVE" : "SETACTIVE");
    }
  }

  private bool IsSharingButtonDisabled
  {
    get => this.m_isSharingButtonDisabled;
    set
    {
      if (value == this.m_isSharingButtonDisabled)
        return;
      this.m_isSharingButtonDisabled = value;
      this.m_sharingButtonWidget.TriggerEvent(this.m_isSharingButtonFlipped || this.m_isSharingButtonDisabled ? "INACTIVE" : "SETACTIVE");
    }
  }

  private PartyManager.MercTeamShareState TeamSharingState
  {
    get => this.m_teamSharingState;
    set
    {
      this.m_teamSharingState = value;
      PartyManager.Get().SetTeamSharingState(value);
      if (value != PartyManager.MercTeamShareState.NOT_SHARING)
        return;
      this.m_remoteTeamList = (List<LettuceTeam>) null;
    }
  }

  private bool UsingLocalTeams => this.TeamSharingState != PartyManager.MercTeamShareState.USING_REMOTE_TEAMS;

  private bool TeamSharingEnabled => this.TeamSharingState != 0;

  private bool AreAnyLocalTeamsValid
  {
    get
    {
      foreach (LettuceTeam team in CollectionManager.Get().GetTeams())
      {
        if (team.IsValid())
          return true;
      }
      return false;
    }
  }

  public override void Start()
  {
    base.Start();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_PlayButtonPhoneReference.RegisterReadyListener<PlayButton>(new System.Action<PlayButton>(this.OnPlayButtonReady));
      this.m_BackButtonPhoneReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnBackButtonReady));
      this.m_TeamPreviewPhoneReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnTeamPreviewReady));
    }
    else
    {
      this.m_PlayButtonReference.RegisterReadyListener<PlayButton>(new System.Action<PlayButton>(this.OnPlayButtonReady));
      this.m_BackButtonReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnBackButtonReady));
      this.m_TeamPreviewReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnTeamPreviewReady));
    }
    this.m_SharingButtonReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnSharingButtonReady));
    this.m_TeamListDisplay.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnTeamListDisplayReady));
    this.m_MercDetailsDisplayReference.RegisterReadyListener<MercenaryDetailDisplay>(new System.Action<MercenaryDetailDisplay>(this.OnDetailsDisplayReady));
    this.CancelLockedInTeam();
    this.InitPartySelections();
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesSubMenus);
    CollectionManager.Get().StartInitialMercenaryLoadIfRequired();
    this.StartCoroutine(this.InitializeWhenReady());
  }

  private void InitPartySelections()
  {
    PartyManager partyManager = PartyManager.Get();
    partyManager.AddChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    partyManager.AddMemberAttributeChangedListener(new PartyManager.MemberAttributeChangedCallback(this.OnPartyMemberAttributeChanged));
    if (partyManager.GetCurrentPartySize() == 2)
    {
      this.m_opponentsSelectedTeam = partyManager.GetOpponentSelectedTeam();
      this.IsSharingButtonDisabled = partyManager.GetMyTeamSharingButtonStatus() == PartyManager.MercTeamSharingButtonStatus.DISABLED;
      if (!this.IsSharingButtonDisabled)
      {
        this.m_teamSharingState = partyManager.GetTeamSharingState();
        if (!this.TeamSharingEnabled)
          return;
        this.GetRemoteTeams();
        return;
      }
    }
    this.TeamSharingState = PartyManager.MercTeamShareState.NOT_SHARING;
  }

  private void GetRemoteTeams()
  {
    LettuceTeamList sharedTeams = PartyManager.Get().GetSharedTeams();
    if (sharedTeams == null)
      this.TeamSharingState = PartyManager.MercTeamShareState.NOT_SHARING;
    else
      this.m_remoteTeamList = this.MakeTeamListFromSharedProtos(sharedTeams);
  }

  public void OnDestroy()
  {
    if ((UnityEngine.Object) this.MercenaryDetailDisplay != (UnityEngine.Object) null)
      this.MercenaryDetailDisplay.UnregisterOnHideEvent(new MercenaryDetailDisplay.OnHideDelegate(this.OnDetailDisplayClosed));
    PartyManager partyManager = PartyManager.Get();
    if (partyManager != null)
    {
      partyManager.RemoveChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
      partyManager.RemoveMemberAttributeChangedListener(new PartyManager.MemberAttributeChangedCallback(this.OnPartyMemberAttributeChanged));
    }
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager != null)
    {
      LettuceTeam editingTeam = collectionManager.GetEditingTeam();
      collectionManager.ClearEditingTeam();
      editingTeam?.SendChanges();
    }
    GameMgr.Get()?.UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
  }

  private void TeamListEventListener(string eventName)
  {
    if (!(eventName == "TEAM_SELECTED"))
      return;
    this.OnTeamSelected();
  }

  public void OnPlayButtonReady(PlayButton playButton)
  {
    if ((UnityEngine.Object) playButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButtonFinishedLoading = true;
      this.m_playButton = playButton;
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonRelease));
      this.m_playButton.Disable();
    }
  }

  public void OnBackButtonReady(UIBButton backButton)
  {
    if ((UnityEngine.Object) backButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "BackButton could not be found! You will not be able to click 'Back'!");
    }
    else
    {
      this.m_backButtonFinishedLoading = true;
      this.m_backButton = backButton;
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonRelease));
    }
  }

  public void OnSharingButtonReady(UIBButton sharingButton)
  {
    if ((UnityEngine.Object) sharingButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "SharingButton could not be found! You will not be able to click 'Back'!");
    }
    else
    {
      this.m_sharingButtonFinishedLoading = true;
      this.m_sharingButton = sharingButton;
      this.m_sharingButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTeamSharingButtonReleased));
      this.m_sharingButtonWidget = this.m_sharingButton.GetComponent<Widget>();
      this.UpdateSharingButtonText();
    }
  }

  public void OnTeamListDisplayReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController != (UnityEngine.Object) null)
      visualController.GetComponent<Widget>().RegisterEventListener(new Widget.EventListenerDelegate(this.TeamListEventListener));
    this.m_teamListFinishedLoading = true;
    this.m_teamListVisualController = visualController;
  }

  public void OnTeamPreviewReady(Widget preview)
  {
    if ((UnityEngine.Object) preview == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "TeamPreview could not be found!");
    }
    else
    {
      this.m_teamPreviewFinishedLoading = true;
      this.m_teamPreviewWidget = preview;
      this.PopulateTeamPreviewData(new LettuceTeam());
    }
  }

  public void OnDetailsDisplayReady(MercenaryDetailDisplay details)
  {
    if ((UnityEngine.Object) details == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "MercenaryDetailsDisplay could not be found!");
    }
    else
    {
      this.MercenaryDetailDisplay = details;
      this.MercenaryDetailDisplay.RegisterOnHideEvent(new MercenaryDetailDisplay.OnHideDelegate(this.OnDetailDisplayClosed));
      this.m_detailsDisplayFinishedLoading = true;
    }
  }

  private void UpdateSharingButtonText()
  {
    string eventName;
    switch (this.TeamSharingState)
    {
      case PartyManager.MercTeamShareState.USING_LOCAL_TEAMS:
        eventName = "FRIENDLY_CHALLENGE_OPPONENT_TEAMS";
        break;
      case PartyManager.MercTeamShareState.USING_REMOTE_TEAMS:
        eventName = "FRIENDLY_CHALLENGE_MY_TEAMS";
        break;
      default:
        eventName = "FRIENDLY_CHALLENGE_BORROW";
        break;
    }
    this.m_sharingButtonWidget.TriggerEvent(eventName);
  }

  private void UpdatePlayButton()
  {
    if (this.m_selectedTeam != null && this.m_selectedTeam.IsValid() && !this.m_selectedTeam.DoesContainDisabledMerc())
      this.m_playButton.Enable();
    else
      this.m_playButton.Disable();
  }

  private void OnPlayButtonRelease(UIEvent e)
  {
    if (this.m_selectedTeam == null)
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_VALIDATE_NO_TEAM_SELECTED"),
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
    else if (!this.m_selectedTeam.IsValid())
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_INVALID"),
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
    }
    else
    {
      PartyManager.Get().SetSelectedMercenariesTeamId(this.m_selectedTeam.ID);
      this.m_isTeamLockedIn = true;
      if (this.m_opponentsSelectedTeam > 0L)
        return;
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_text = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_TEAM"),
        m_showAlertIcon = false,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => this.CancelLockedInTeam())
      });
    }
  }

  private void OnBackButtonRelease(UIEvent e) => this.NavigateBack();

  private void OnTeamSharingButtonReleased(UIEvent e)
  {
    if (!this.TeamSharingEnabled)
    {
      this.RequestTeamSharing();
    }
    else
    {
      this.TeamSharingState = this.TeamSharingState != PartyManager.MercTeamShareState.USING_LOCAL_TEAMS ? PartyManager.MercTeamShareState.USING_LOCAL_TEAMS : PartyManager.MercTeamShareState.USING_REMOTE_TEAMS;
      this.UpdateSharingButtonText();
      this.CreateTeamListDataModel();
      this.UpdatePlayButton();
    }
  }

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.m_playButtonFinishedLoading)
    {
      failureMessage = "LettuceFriendlyDisplay - Play button never loaded.";
      return false;
    }
    if (!this.m_backButtonFinishedLoading)
    {
      failureMessage = "LettuceFriendlyDisplay - Back button never loaded.";
      return false;
    }
    if (!this.m_sharingButtonFinishedLoading)
    {
      failureMessage = "LettuceFriendlyDisplay - Sharing button never loaded.";
      return false;
    }
    if (!this.m_teamListFinishedLoading)
    {
      failureMessage = "LettuceFriendlyDisplay - Team list never loaded.";
      return false;
    }
    if (!this.m_teamPreviewFinishedLoading)
    {
      failureMessage = "LettuceFriendlyDisplay - Team preview never loaded.";
      return false;
    }
    if (!this.m_detailsDisplayFinishedLoading)
    {
      failureMessage = "LettuceFriendlyDisplay - Details Display never loaded.";
      return false;
    }
    if (!CollectionManager.Get().IsLettuceLoaded())
    {
      failureMessage = "LettuceFriendlyDisplay - Mercenaries collection was never loaded.";
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  protected override bool ShouldStartShown() => !SceneMgr.Get().IsDoingSceneDrivenTransition() || SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_COLLECTION;

  private IEnumerator InitializeWhenReady()
  {
    LettuceFriendlyDisplay lettuceFriendlyDisplay = this;
    while (!lettuceFriendlyDisplay.IsFinishedLoading(out string _))
      yield return (object) null;
    if (!PartyManager.Get().IsInMercenariesFriendlyChallenge())
    {
      lettuceFriendlyDisplay.ShowChallengeCanceledDialog(GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_QUEUE_CANCELED"));
      lettuceFriendlyDisplay.NavigateBack();
    }
    PartyManager.Get()?.SetOpponentTeamSharingButtonStatus(lettuceFriendlyDisplay.AreAnyLocalTeamsValid ? PartyManager.MercTeamSharingButtonStatus.ENABLED : PartyManager.MercTeamSharingButtonStatus.DISABLED);
    lettuceFriendlyDisplay.CreateTeamListDataModel();
  }

  private LettuceTeam GetTeamFromTeamList(int teamId)
  {
    foreach (LettuceTeam teamFromTeamList in this.m_teamListInUse)
    {
      if (teamFromTeamList.ID == (long) teamId)
        return teamFromTeamList;
    }
    return (LettuceTeam) null;
  }

  private bool TeamIdIsValid(int teamId)
  {
    foreach (LettuceTeam lettuceTeam in this.m_teamListInUse)
    {
      if (lettuceTeam.ID == (long) teamId)
        return lettuceTeam.IsValid();
    }
    return false;
  }

  private int GetDefaultTeamIDToSelect()
  {
    long teamId = 0;
    if (this.UsingLocalTeams)
    {
      GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_LAST_SELECTED_PVP_TEAM, out teamId);
      if (!this.TeamIdIsValid((int) teamId))
        teamId = 0L;
    }
    if (teamId == 0L)
    {
      if (this.m_selectedTeam != null && this.TeamIdIsValid((int) this.m_selectedTeam.ID))
        teamId = this.m_selectedTeam.ID;
      if (teamId == 0L)
      {
        foreach (LettuceTeam lettuceTeam in this.m_teamListInUse)
        {
          if (lettuceTeam.IsValid())
          {
            teamId = lettuceTeam.ID;
            break;
          }
        }
      }
    }
    return (int) teamId;
  }

  private void CreateTeamListDataModel()
  {
    LettuceTeamListDataModel dataModel = new LettuceTeamListDataModel();
    this.m_teamListInUse = !this.UsingLocalTeams ? this.m_remoteTeamList : CollectionManager.Get().GetTeams();
    if (this.m_selectedTeam != null)
      this.m_selectedTeam = this.GetTeamFromTeamList((int) this.m_selectedTeam.ID);
    CollectionUtils.PopulateMercenariesTeamListDataModel(dataModel, false, this.m_teamListInUse, this.TeamSharingState == PartyManager.MercTeamShareState.USING_REMOTE_TEAMS, true, true);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (this.m_selectedTeam != null)
        this.PopulateTeamPreviewData(this.m_selectedTeam);
    }
    else
    {
      if (this.m_teamListInUse.Count == 0)
        this.PopulateTeamPreviewData((LettuceTeam) null);
      dataModel.AutoSelectedTeamId = this.GetDefaultTeamIDToSelect();
    }
    this.BindTeamListDataModel(dataModel);
  }

  private void BindTeamListDataModel(LettuceTeamListDataModel dataModel)
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    Widget owner = (Widget) component.Owner;
    if (!((UnityEngine.Object) owner != (UnityEngine.Object) null))
      return;
    owner.BindDataModel((IDataModel) dataModel);
  }

  private LettuceTeamDataModel GetTeamPreviewDataModel()
  {
    if ((UnityEngine.Object) this.m_teamPreviewWidget == (UnityEngine.Object) null)
      return (LettuceTeamDataModel) null;
    IDataModel model;
    if (!this.m_teamPreviewWidget.GetDataModel(217, out model))
    {
      model = (IDataModel) new LettuceTeamDataModel();
      this.m_teamPreviewWidget.BindDataModel(model);
    }
    return model as LettuceTeamDataModel;
  }

  private void NavigateBack()
  {
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    PartyManager.Get().LeaveParty();
  }

  private void OnTeamSelected()
  {
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_teamListVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("No event data model attached to the TeamListVisualController");
    }
    else
    {
      LettuceTeamDataModel payload = (LettuceTeamDataModel) eventDataModel.Payload;
      this.m_selectedTeam = this.GetTeamFromTeamList((int) payload.TeamId);
      if (this.UsingLocalTeams)
        GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_LAST_SELECTED_PVP_TEAM, new long[1]
        {
          payload.TeamId
        }));
      this.PopulateTeamPreviewData(this.m_selectedTeam);
      this.UpdatePlayButton();
      if (this.m_selectedTeam != null && this.m_selectedTeam.IsValid() && !this.m_selectedTeam.DoesContainDisabledMerc())
        return;
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_DISABLED_TEAM_HEADER"),
        m_text = this.m_selectedTeam == null || !this.m_selectedTeam.DoesContainDisabledMerc() ? GameStrings.Get("GLUE_LETTUCE_DISABLED_INVALID_TEAM") : GameStrings.Get("GLUE_LETTUCE_DISABLED_TEAM_NO_EDIT"),
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM,
        m_showAlertIcon = true,
        m_confirmText = GameStrings.Get("GLOBAL_OKAY"),
        m_blurWhenShown = true
      });
    }
  }

  private void CancelLockedInTeam()
  {
    this.m_isTeamLockedIn = false;
    PartyManager.Get().SetSelectedMercenariesTeamId(0L);
  }

  private void PopulateTeamPreviewData(LettuceTeam team) => CollectionUtils.PopulateTeamPreviewData(this.GetTeamPreviewDataModel(), team, (List<int>) null, false, !this.UsingLocalTeams);

  private void ShowChallengeCanceledDialog(string message) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
    m_text = message,
    m_alertTextAlignment = UberText.AlignmentOptions.Center,
    m_responseDisplay = AlertPopup.ResponseDisplay.OK,
    m_showAlertIcon = false,
    m_okText = GameStrings.Get("GLOBAL_OKAY")
  });

  private void OnPartyChanged(
    PartyManager.PartyInviteEvent inviteEvent,
    BnetGameAccountId playerGameAccountId,
    PartyManager.PartyData challengeData,
    object userData)
  {
    Log.Party.PrintDebug("LettuceFriendlyDisplay.OnPartyChanged(): Event={0}, gameAccountId={1}", (object) inviteEvent, (object) playerGameAccountId);
    switch (inviteEvent)
    {
      case PartyManager.PartyInviteEvent.I_RESCINDED_INVITE:
      case PartyManager.PartyInviteEvent.INVITE_EXPIRED:
      case PartyManager.PartyInviteEvent.FRIEND_LEFT:
      case PartyManager.PartyInviteEvent.LEADER_DISSOLVED_PARTY:
        DialogManager.Get().ClearAllImmediately();
        this.ShowChallengeCanceledDialog(GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_QUEUE_CANCELED"));
        this.NavigateBack();
        break;
    }
  }

  private void OnPartyMemberAttributeChanged(
    BnetGameAccountId playerGameAccountId,
    Blizzard.GameService.Protocol.V2.Client.Attribute attribute,
    object userData)
  {
    string name = attribute.Name;
    if (!(name == "team_id"))
    {
      if (!(name == "ts_status"))
      {
        if (!(name == "ts_MSG"))
        {
          if (!(name == "ts_teams") || !((BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId() != (BnetEntityId) playerGameAccountId))
            return;
          this.HandleSharedTeamsReceived(attribute.Value.BlobValue.ToByteArray());
        }
        else
        {
          if (!((BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId() != (BnetEntityId) playerGameAccountId) || !attribute.Value.HasIntValue || attribute.Value.IntValue == 0L)
            return;
          this.HandleTeamSharingMessage((PartyManager.MercTeamShareMSG) attribute.Value.IntValue);
        }
      }
      else
      {
        if (!((BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId() != (BnetEntityId) playerGameAccountId) || !attribute.Value.HasIntValue)
          return;
        this.IsSharingButtonDisabled = attribute.Value.IntValue == 1L;
      }
    }
    else
    {
      if (!attribute.Value.HasIntValue)
        return;
      if ((BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId() != (BnetEntityId) playerGameAccountId)
        this.m_opponentsSelectedTeam = attribute.Value.IntValue;
      if (!PartyManager.Get().IsPartyLeader() || !this.m_isTeamLockedIn || this.m_opponentsSelectedTeam <= 0L)
        return;
      PartyManager.Get().FindGame();
    }
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    Log.Party.PrintDebug("LettuceFriendlyDisplay.OnFindGameEvent(): State={0}", (object) eventData.m_state);
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_STARTED:
      case FindGameState.BNET_QUEUE_ENTERED:
        DialogManager.Get().ClearAllImmediately();
        break;
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        this.CancelLockedInTeam();
        break;
      case FindGameState.SERVER_GAME_STARTED:
        this.CancelLockedInTeam();
        break;
    }
    return false;
  }

  private void OnDetailDisplayClosed()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return;
    LettuceTeam editingTeam = collectionManager.GetEditingTeam();
    collectionManager.ClearEditingTeam();
    if (editingTeam == null || !editingTeam.SendChanges())
      return;
    this.CreateTeamListDataModel();
    PartyManager partyManager = PartyManager.Get();
    if (partyManager.GetTeamSharingState(true) == PartyManager.MercTeamShareState.NOT_SHARING)
      return;
    partyManager.SetSharedTeams(this.MakeSharableTeamList());
  }

  public void ShowMercDetailsDisplay(LettuceMercenary mercenary)
  {
    if (!this.UsingLocalTeams)
      return;
    if (this.m_selectedTeam != null)
    {
      LettuceTeamDataModel previewDataModel = this.GetTeamPreviewDataModel();
      CollectionUtils.PopulateMercenariesTeamDataModel(previewDataModel, this.m_selectedTeam);
      CollectionManager.Get().SetEditingTeam(this.m_selectedTeam);
      this.MercenaryDetailDisplay.GetComponent<Widget>().BindDataModel((IDataModel) previewDataModel);
    }
    this.MercenaryDetailDisplay.Show(mercenary, (bool) UniversalInputManager.UsePhoneUI ? "SHOW_PARTIAL" : "SHOW_FULL", this.m_selectedTeam);
  }

  private void RequestTeamSharing()
  {
    PartyManager.Get().SetTeamSharingMsg(PartyManager.MercTeamShareMSG.REQUEST_SHARING);
    this.IsSharingButtonFlipped = true;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_TEAM_SHARE_HEADER"),
      m_text = GameStrings.Format("GLUE_TEAM_SHARE_REQUEST_WAITING_RESPONSE", (object) PartyManager.Get().GetOpponentBestName()),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, context) =>
      {
        PartyManager.Get().SetTeamSharingMsg(PartyManager.MercTeamShareMSG.SHARING_REQUEST_CANCELLED);
        this.UpdateSharingButtonText();
        this.IsSharingButtonFlipped = false;
      })
    });
  }

  private void OnTeamSharingRequestDialogResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      PartyManager.Get().SetTeamSharingMsg(PartyManager.MercTeamShareMSG.SHARING_REQUEST_DENIED);
    else
      PartyManager.Get().SetSharedTeams(this.MakeSharableTeamList());
  }

  private void HandleTeamSharingRequest()
  {
    DialogManager dialogManager = DialogManager.Get();
    dialogManager.ClearAllImmediately();
    dialogManager.ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_TEAM_SHARE_HEADER"),
      m_text = GameStrings.Format("GLUE_TEAM_SHARE_REQUESTED", (object) PartyManager.Get().GetOpponentBestName()),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnTeamSharingRequestDialogResponse),
      m_confirmText = GameStrings.Get("GLUE_TEAM_SHARE_ACCEPT_REQUEST"),
      m_cancelText = GameStrings.Get("GLUE_TEAM_SHARE_DECLINE_REQUEST")
    });
  }

  private void HandleTeamSharingRequestCancelled()
  {
    DialogManager dialogManager = DialogManager.Get();
    dialogManager.ClearAllImmediately();
    dialogManager.ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_TEAM_SHARE_HEADER"),
      m_text = GameStrings.Format("GLUE_TEAM_SHARE_REQUEST_CANCELED", (object) PartyManager.Get().GetOpponentBestName()),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM
    });
  }

  private void HandleTeamSharingRequestDenied()
  {
    DialogManager dialogManager = DialogManager.Get();
    dialogManager.ClearAllImmediately();
    dialogManager.ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_TEAM_SHARE_HEADER"),
      m_text = GameStrings.Format("GLUE_TEAM_SHARE_REQUEST_DECLINED", (object) PartyManager.Get().GetOpponentBestName()),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM
    });
    PartyManager.Get().SetTeamSharingMsg(PartyManager.MercTeamShareMSG.NO_MSG);
    this.TeamSharingState = PartyManager.MercTeamShareState.NOT_SHARING;
    this.m_remoteTeamList = (List<LettuceTeam>) null;
    this.UpdateSharingButtonText();
    this.IsSharingButtonFlipped = false;
  }

  private void ShowTeamSharingError()
  {
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_TEAM_SHARE_HEADER"),
      m_text = GameStrings.Get("GLUE_TEAM_SHARE_ERROR"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM
    });
    this.UpdateSharingButtonText();
    this.IsSharingButtonFlipped = false;
  }

  private void HandleSharedTeamsReceived(byte[] blob)
  {
    DialogManager.Get().ClearAllImmediately();
    if (blob == null || blob.Length == 0)
    {
      this.ShowTeamSharingError();
    }
    else
    {
      List<LettuceTeam> lettuceTeamList = this.MakeTeamListFromSharedProtos(ProtobufUtil.ParseFrom<LettuceTeamList>(blob));
      if (lettuceTeamList.Count == 0)
      {
        this.ShowTeamSharingError();
      }
      else
      {
        this.m_remoteTeamList = lettuceTeamList;
        if (this.TeamSharingState == PartyManager.MercTeamShareState.NOT_SHARING)
        {
          this.TeamSharingState = PartyManager.MercTeamShareState.USING_REMOTE_TEAMS;
          PartyManager.Get().SetTeamSharingMsg(PartyManager.MercTeamShareMSG.NO_MSG);
          this.UpdateSharingButtonText();
          this.IsSharingButtonFlipped = false;
        }
        if (this.UsingLocalTeams)
          return;
        this.CreateTeamListDataModel();
      }
    }
  }

  private void HandleTeamSharingMessage(PartyManager.MercTeamShareMSG msg)
  {
    switch (msg)
    {
      case PartyManager.MercTeamShareMSG.REQUEST_SHARING:
        this.HandleTeamSharingRequest();
        break;
      case PartyManager.MercTeamShareMSG.SHARING_REQUEST_CANCELLED:
        this.HandleTeamSharingRequestCancelled();
        break;
      case PartyManager.MercTeamShareMSG.SHARING_REQUEST_DENIED:
        this.HandleTeamSharingRequestDenied();
        break;
    }
  }

  private LettuceMercenary MakeMercenaryFromSharedProto(
    LettuceTeamMercenary protoMerc)
  {
    LettuceMercenaryDbfRecord record1 = GameDbf.LettuceMercenary.GetRecord(protoMerc.MercenaryId);
    if (record1 == null)
    {
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.RegisterMercenary(): Invalid mercenary ID [{0}]!", (object) protoMerc.MercenaryId);
      return (LettuceMercenary) null;
    }
    LettuceMercenary lettuceMercenary = new LettuceMercenary()
    {
      ID = protoMerc.MercenaryId,
      m_rarity = (TAG_RARITY) record1.Rarity,
      m_acquireType = (TAG_ACQUIRE_TYPE) record1.AcquireType,
      m_customAcquireText = (string) record1.HowToAcquireText,
      m_isFullyUpgraded = protoMerc.SharedTeamMercenaryIsFullyUpgraded
    };
    lettuceMercenary.SetExperience(protoMerc.SharedTeamMercenaryXp);
    MercenaryArtVariationDbfRecord record2 = GameDbf.MercenaryArtVariation.GetRecord(protoMerc.SelectedArtVariationId);
    EntityDef entityDef = DefLoader.Get().GetEntityDef(record2.CardId);
    string shortName = entityDef.GetShortName();
    lettuceMercenary.m_mercName = entityDef.GetName();
    lettuceMercenary.m_mercShortName = string.IsNullOrEmpty(shortName) ? lettuceMercenary.m_mercName : shortName;
    lettuceMercenary.m_role = entityDef.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
    if (record2 != null)
    {
      TAG_PREMIUM variationPremium = (TAG_PREMIUM) protoMerc.SelectedArtVariationPremium;
      lettuceMercenary.m_artVariations.Add(new LettuceMercenary.ArtVariation(record2, variationPremium, record2.DefaultVariation));
      lettuceMercenary.GetBaseLoadout().SetArtVariation(record2, variationPremium);
    }
    return lettuceMercenary;
  }

  private LettuceTeam MakeTeamFromSharedProto(PegasusLettuce.LettuceTeam protoTeam)
  {
    if (protoTeam.MercenaryList.Mercenaries.Count == 0)
      return (LettuceTeam) null;
    LettuceTeam lettuceTeam = new LettuceTeam()
    {
      Name = protoTeam.Name,
      SortOrder = protoTeam.SortOrder,
      ID = protoTeam.TeamId,
      TeamType = protoTeam.Type_
    };
    foreach (LettuceTeamMercenary mercenary in protoTeam.MercenaryList.Mercenaries)
    {
      LettuceMercenary merc = this.MakeMercenaryFromSharedProto(mercenary);
      if (merc != null)
        lettuceTeam.AddMerc(merc);
    }
    lettuceTeam.ClearDirty();
    return lettuceTeam;
  }

  private List<LettuceTeam> MakeTeamListFromSharedProtos(LettuceTeamList protoList)
  {
    List<LettuceTeam> lettuceTeamList = new List<LettuceTeam>();
    foreach (PegasusLettuce.LettuceTeam team in protoList.Teams)
    {
      LettuceTeam lettuceTeam = this.MakeTeamFromSharedProto(team);
      if (lettuceTeam != null && lettuceTeam.GetMercCount() != 0)
        lettuceTeamList.Add(lettuceTeam);
    }
    return lettuceTeamList;
  }

  private LettuceTeamList MakeSharableTeamList()
  {
    LettuceTeamList lettuceTeamList = new LettuceTeamList();
    foreach (LettuceTeam team in CollectionManager.Get().GetTeams())
      lettuceTeamList.Teams.Add(LettuceTeam.Convert(team, true));
    return lettuceTeamList;
  }
}
