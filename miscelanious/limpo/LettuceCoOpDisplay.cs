using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System.Collections;

public class LettuceCoOpDisplay : AbsSceneDisplay
{
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_EditTeamButtonReference;
  public AsyncReference m_TeamListDisplay;
  private bool m_playButtonFinishedLoading;
  private bool m_backButtonFinishedLoading;
  private bool m_editTeamButtonFinishedLoading;
  private PlayButton m_playButton;
  private UIBButton m_editTeamButton;
  private VisualController m_teamListVisualController;
  private LettuceTeam m_selectedTeam;
  private long m_coopPartnerTeamId;

  public override void Start()
  {
    base.Start();
    this.m_PlayButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnPlayButtonReady));
    this.m_BackButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnBackButtonReady));
    this.m_EditTeamButtonReference.RegisterReadyListener<UIBButton>(new System.Action<UIBButton>(this.OnEditTeamButtonReady));
    this.m_TeamListDisplay.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnTeamListDisplayReady));
    PartyManager.Get().AddChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    PartyManager.Get().AddMemberAttributeChangedListener(new PartyManager.MemberAttributeChangedCallback(this.OnPartyMemberAttributeChanged));
    PartyManager.Get().SetReadyStatus(false);
    this.StartCoroutine(this.InitializeWhenReady());
  }

  public void OnDestroy()
  {
    if (PartyManager.Get() == null)
      return;
    PartyManager.Get().RemoveChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    PartyManager.Get().RemoveMemberAttributeChangedListener(new PartyManager.MemberAttributeChangedCallback(this.OnPartyMemberAttributeChanged));
  }

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.m_playButtonFinishedLoading)
    {
      failureMessage = "LettuceCoOpDisplay - Play button never loaded.";
      return false;
    }
    if (!this.m_backButtonFinishedLoading)
    {
      failureMessage = "LettuceCoOpDisplay - Back button never loaded.";
      return false;
    }
    if (!this.m_editTeamButtonFinishedLoading)
    {
      failureMessage = "LettuceCoOpDisplay - Edit Team button never loaded.";
      return false;
    }
    if ((UnityEngine.Object) this.m_teamListVisualController == (UnityEngine.Object) null)
    {
      failureMessage = "LettuceCoOpDisplay - Team List never loaded.";
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  public void OnPlayButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButton = buttonVisualController.gameObject.GetComponent<PlayButton>();
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayButtonRelease));
      this.m_playButton.Disable();
      this.m_playButtonFinishedLoading = true;
    }
  }

  public void OnBackButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "BackButton could not be found! You will not be able to click 'Back'!");
    }
    else
    {
      buttonVisualController.gameObject.GetComponent<UIBButton>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonRelease));
      this.m_backButtonFinishedLoading = true;
    }
  }

  public void OnEditTeamButtonReady(UIBButton editTeamButton)
  {
    this.m_editTeamButtonFinishedLoading = true;
    if ((UnityEngine.Object) editTeamButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "EditTeamButton could not be found! You will not be able to click 'Edit Team'!");
    }
    else
    {
      this.m_editTeamButton = editTeamButton;
      this.m_editTeamButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnEditTeamButtonRelease));
    }
  }

  public void OnTeamListDisplayReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController != (UnityEngine.Object) null)
      visualController.GetComponent<Widget>().RegisterEventListener(new Widget.EventListenerDelegate(this.TeamListEventListener));
    this.m_teamListVisualController = visualController;
  }

  private void TeamListEventListener(string eventName)
  {
    if (!(eventName == "TEAM_SELECTED"))
      return;
    this.OnTeamSelected();
  }

  public void PlayButtonRelease(UIEvent e)
  {
    PartyManager.Get().SetReadyStatus(true);
    if (!PartyManager.Get().IsPartyLeader())
      return;
    if (!PartyManager.Get().AreAllPartyMembersReady())
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("Not Ready"),
        m_text = GameStrings.Get("Wait for all party members to be ready."),
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_showAlertIcon = true,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
    else if (this.m_coopPartnerTeamId == 0L)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("Not Ready"),
        m_text = GameStrings.Get("Wait for your partner to select a team."),
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_showAlertIcon = true,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
    }
    else
    {
      LettuceVillageDisplay.LettuceSceneTransitionPayload sceneTransitionPayload = new LettuceVillageDisplay.LettuceSceneTransitionPayload()
      {
        m_SelectedBountySet = GameDbf.LettuceBountySet.GetRecord(1),
        m_DifficultyMode = LettuceBounty.MercenariesBountyDifficulty.NORMAL,
        m_SelectedBounty = GameDbf.LettuceBounty.GetRecord(48),
        m_TeamId = this.m_selectedTeam.ID,
        m_CoOpPartnerTeamId = this.m_coopPartnerTeamId
      };
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_MAP, sceneTransitionPayload: ((object) sceneTransitionPayload));
    }
  }

  public void BackButtonRelease(UIEvent e) => this.NavigateBack();

  private void OnEditTeamButtonRelease(UIEvent e)
  {
    CollectionManager.Get().NotifyOfBoxTransitionStart();
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_COLLECTION);
  }

  protected override bool ShouldStartShown() => true;

  private void NavigateBack()
  {
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.NEXT_SCENE);
    PartyManager.Get().LeaveParty();
  }

  private void OnPartyChanged(
    PartyManager.PartyInviteEvent inviteEvent,
    BnetGameAccountId playerGameAccountId,
    PartyManager.PartyData challengeData,
    object userData)
  {
    Log.Party.PrintDebug("LettuceCoOpDisplay.OnPartyChanged(): Event={0}, gameAccountId={1}", (object) inviteEvent, (object) playerGameAccountId);
    switch (inviteEvent)
    {
      case PartyManager.PartyInviteEvent.I_RESCINDED_INVITE:
      case PartyManager.PartyInviteEvent.FRIEND_DECLINED_INVITE:
      case PartyManager.PartyInviteEvent.INVITE_EXPIRED:
      case PartyManager.PartyInviteEvent.FRIEND_LEFT:
      case PartyManager.PartyInviteEvent.LEADER_DISSOLVED_PARTY:
        this.NavigateBack();
        break;
    }
  }

  private void OnPartyMemberAttributeChanged(
    BnetGameAccountId playerGameAccountId,
    Blizzard.GameService.Protocol.V2.Client.Attribute attribute,
    object userData)
  {
    if ((BnetEntityId) PartyManager.Get().GetPartyLeaderGameAccountId() == (BnetEntityId) playerGameAccountId && attribute.Name == "scene" && attribute.Value.HasStringValue)
    {
      SceneMgr.Mode mode = Blizzard.T5.Core.Utils.EnumUtils.Parse<SceneMgr.Mode>(attribute.Value.StringValue);
      if (mode != SceneMgr.Mode.INVALID)
        SceneMgr.Get().SetNextMode(mode);
    }
    if (!((BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId() != (BnetEntityId) playerGameAccountId) || !(attribute.Name == "team_id") || !attribute.Value.HasIntValue)
      return;
    this.m_coopPartnerTeamId = attribute.Value.IntValue;
  }

  private IEnumerator InitializeWhenReady()
  {
    LettuceCoOpDisplay lettuceCoOpDisplay = this;
    while (!lettuceCoOpDisplay.IsFinishedLoading(out string _))
      yield return (object) null;
    lettuceCoOpDisplay.InitializeTeamListDataModel();
  }

  private void InitializeTeamListDataModel() => CollectionUtils.PopulateMercenariesTeamListDataModel(this.GetTeamListDataModel(), false);

  private LettuceTeamListDataModel GetTeamListDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (LettuceTeamListDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(218, out model))
    {
      model = (IDataModel) new LettuceTeamListDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceTeamListDataModel;
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
      this.m_selectedTeam = CollectionManager.Get().GetTeam(payload.TeamId);
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_LAST_SELECTED_PVP_TEAM, new long[1]
      {
        payload.TeamId
      }));
      this.m_playButton.Enable();
      PartyManager.Get().SetSelectedMercenariesTeamId(payload.TeamId);
    }
  }
}
