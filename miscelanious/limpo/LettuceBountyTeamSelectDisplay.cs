using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class LettuceBountyTeamSelectDisplay : AbsSceneDisplay, IMercDetailsDisplayProvider
{
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_PlayButtonPhoneReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_BackButtonPhoneReference;
  public AsyncReference m_CollectionButtonReference;
  public AsyncReference m_TeamPreviewReference;
  public AsyncReference m_TeamPreviewPhoneReference;
  public AsyncReference m_TeamListDisplay;
  public AsyncReference m_MercDetailsDisplayReference;
  private PlayButton m_playButton;
  private UIBButton m_backButton;
  private UIBButton m_collectionButton;
  private VisualController m_teamListVisualController;
  private Widget m_teamPreviewWidget;
  private bool m_playButtonFinishedLoading;
  private bool m_backButtonFinishedLoading;
  private bool m_collectionButtonFinishedLoading;
  private bool m_teamListFinishedLoading;
  private bool m_teamPreviewFinishedLoading;
  private List<LettuceTeam> m_teamList;
  private LettuceTeam m_selectedTeam;
  private static bool m_hasSeenTeamLockConfirmationThisSession;

  public MercenaryDetailDisplay MercenaryDetailDisplay { get; private set; }

  public override void Start()
  {
    base.Start();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_PlayButtonPhoneReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
      this.m_BackButtonPhoneReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
      this.m_TeamPreviewPhoneReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTeamPreviewReady));
    }
    else
    {
      this.m_PlayButtonReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
      this.m_BackButtonReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
      this.m_TeamPreviewReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTeamPreviewReady));
    }
    this.m_CollectionButtonReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnCollectionButtonReady));
    this.m_TeamListDisplay.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnTeamListDisplayReady));
    this.m_MercDetailsDisplayReference.RegisterReadyListener<MercenaryDetailDisplay>(new Action<MercenaryDetailDisplay>(this.OnMercDetailsDisplayReady));
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesSubMenus);
    CollectionManager.Get().MercenaryArtVariationChangedEvent += new Action<int, int, TAG_PREMIUM>(this.OnMercenaryArtVariationChangedEvent);
    this.StartCoroutine(this.InitializeWhenReady());
  }

  private void OnDestroy()
  {
    if ((UnityEngine.Object) this.MercenaryDetailDisplay != (UnityEngine.Object) null)
      this.MercenaryDetailDisplay.UnregisterOnHideEvent(new MercenaryDetailDisplay.OnHideDelegate(this.OnDetailDisplayClosed));
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return;
    collectionManager.MercenaryArtVariationChangedEvent -= new Action<int, int, TAG_PREMIUM>(this.OnMercenaryArtVariationChangedEvent);
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

  public void OnCollectionButtonReady(UIBButton collectionButton)
  {
    if ((UnityEngine.Object) collectionButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "CollectionButton could not be found! You will not be able to click 'Back'!");
    }
    else
    {
      this.m_collectionButtonFinishedLoading = true;
      this.m_collectionButton = collectionButton;
      this.m_collectionButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCollectionButtonRelease));
    }
  }

  public void OnTeamListDisplayReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController != (UnityEngine.Object) null)
      visualController.GetComponent<Widget>().RegisterEventListener(new Widget.EventListenerDelegate(this.TeamListEventListener));
    this.m_teamListFinishedLoading = true;
    this.m_teamListVisualController = visualController;
  }

  private void OnMercDetailsDisplayReady(MercenaryDetailDisplay display)
  {
    if ((UnityEngine.Object) this.MercenaryDetailDisplay != (UnityEngine.Object) null)
      this.MercenaryDetailDisplay.UnregisterOnHideEvent(new MercenaryDetailDisplay.OnHideDelegate(this.OnDetailDisplayClosed));
    this.MercenaryDetailDisplay = display;
    if (!((UnityEngine.Object) this.MercenaryDetailDisplay != (UnityEngine.Object) null))
      return;
    this.MercenaryDetailDisplay.RegisterOnHideEvent(new MercenaryDetailDisplay.OnHideDelegate(this.OnDetailDisplayClosed));
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

  private void OnPlayButtonRelease(UIEvent e)
  {
    if (!Network.IsLoggedIn())
      DialogManager.Get().ShowReconnectHelperDialog();
    else if (this.m_selectedTeam == null)
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_VALIDATE_NO_TEAM_SELECTED"),
        m_showAlertIcon = true,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
    else if (!this.m_selectedTeam.IsValid())
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_INVALID"),
        m_showAlertIcon = true,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
    else if (!LettuceBountyTeamSelectDisplay.m_hasSeenTeamLockConfirmationThisSession)
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_BOUNTY_BOARD_TEAM_LOCK_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_BOUNTY_BOARD_TEAM_LOCK_BODY"),
        m_showAlertIcon = false,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_confirmText = GameStrings.Get("GLUE_LETTUCE_BOUNTY_BOARD_TEAM_LOCK_CONFIRM"),
        m_cancelText = GameStrings.Get("GLOBAL_CANCEL"),
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response != AlertPopup.Response.CONFIRM)
            return;
          LettuceBountyTeamSelectDisplay.m_hasSeenTeamLockConfirmationThisSession = true;
          this.NavigateToMapScene();
        })
      });
    else
      this.NavigateToMapScene();
  }

  private void OnBackButtonRelease(UIEvent e) => SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_BOUNTY_BOARD, SceneMgr.TransitionHandlerType.NEXT_SCENE, sceneTransitionPayload: this.m_sceneTransitionPayload);

  private void OnCollectionButtonRelease(UIEvent e)
  {
    if (!Network.IsLoggedIn())
    {
      DialogManager.Get().ShowReconnectHelperDialog();
    }
    else
    {
      ((LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload).m_TeamId = 0L;
      this.SetNextModeAndHandleTransition(SceneMgr.Mode.LETTUCE_COLLECTION, SceneMgr.TransitionHandlerType.CURRENT_SCENE, this.m_sceneTransitionPayload);
    }
  }

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.m_playButtonFinishedLoading)
    {
      failureMessage = "LettuceBountyTeamSelectDisplay - Play button never loaded.";
      return false;
    }
    if (!this.m_backButtonFinishedLoading)
    {
      failureMessage = "LettuceBountyTeamSelectDisplay - Back button never loaded.";
      return false;
    }
    if (!this.m_collectionButtonFinishedLoading)
    {
      failureMessage = "LettuceBountyTeamSelectDisplay - Collection button never loaded.";
      return false;
    }
    if (!this.m_teamListFinishedLoading)
    {
      failureMessage = "LettuceBountyTeamSelectDisplay - Team list never loaded.";
      return false;
    }
    if (!this.m_teamPreviewFinishedLoading)
    {
      failureMessage = "LettuceBountyTeamSelectDisplay - Team preview never loaded.";
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  protected override bool ShouldStartShown() => SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_COLLECTION;

  private IEnumerator InitializeWhenReady()
  {
    LettuceBountyTeamSelectDisplay teamSelectDisplay = this;
    teamSelectDisplay.m_teamList = CollectionManager.Get().GetTeams();
    while (!teamSelectDisplay.IsFinishedLoading(out string _))
      yield return (object) null;
    teamSelectDisplay.InitializeTeamListDataModel();
    teamSelectDisplay.InitializeBountyTeamSelectDataModel();
  }

  private void InitializeTeamListDataModel() => CollectionUtils.PopulateMercenariesTeamListDataModel(this.GetTeamListDataModel(), !(bool) UniversalInputManager.UsePhoneUI, this.m_teamList);

  private void InitializeBountyTeamSelectDataModel()
  {
    LettuceBountyTeamSelectDataModel teamSelectDataModel = this.GetBountyTeamSelectDataModel();
    LettuceBountySetDbfRecord selectedBountySet = ((LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload).m_SelectedBountySet;
    if (selectedBountySet == null)
      return;
    teamSelectDataModel.HeaderText = GameStrings.Format("GLUE_LETTUCE_BOUNTY_BOARD_TEAM_SELECT_HEADER", (object) selectedBountySet.Name.GetString());
  }

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

  private LettuceBountyTeamSelectDataModel GetBountyTeamSelectDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (LettuceBountyTeamSelectDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(518, out model))
    {
      model = (IDataModel) new LettuceBountyTeamSelectDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceBountyTeamSelectDataModel;
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
      ((LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload).m_TeamId = this.m_selectedTeam.ID;
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_LAST_SELECTED_PVP_TEAM, new long[1]
      {
        payload.TeamId
      }));
      this.PopulateTeamPreviewData(this.m_selectedTeam);
      if (this.m_selectedTeam.IsValid() && !this.m_selectedTeam.DoesContainDisabledMerc())
      {
        this.m_playButton.Enable();
      }
      else
      {
        this.m_playButton.Disable();
        if (!this.m_selectedTeam.DoesContainDisabledMerc())
          return;
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_LETTUCE_DISABLED_TEAM_HEADER"),
          m_text = GameStrings.Get("GLUE_LETTUCE_DISABLED_TEAM"),
          m_alertTextAlignment = UberText.AlignmentOptions.Center,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
          m_confirmText = GameStrings.Get("GLUE_EDIT_TEAM"),
          m_blurWhenShown = true,
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
          {
            if (response != AlertPopup.Response.CONFIRM)
              return;
            this.OnCollectionButtonRelease((UIEvent) null);
          })
        });
      }
    }
  }

  private void PopulateTeamPreviewData(LettuceTeam team)
  {
    if (team == null)
      return;
    LettuceTeamDataModel previewDataModel = this.GetTeamPreviewDataModel();
    CollectionUtils.PopulateTeamPreviewData(previewDataModel, team, (List<int>) null, false);
    previewDataModel.TeamName = team.Name;
  }

  private void NavigateToMapScene()
  {
    this.SetNextModeAndHandleTransition(SceneMgr.Mode.LETTUCE_MAP, this.m_sceneTransitionPayload);
    this.m_playButton.Disable();
  }

  private void OnMercenaryArtVariationChangedEvent(
    int mercenaryDbId,
    int artVariationId,
    TAG_PREMIUM premium)
  {
    foreach (LettuceMercenary merc in this.m_selectedTeam.GetMercs())
    {
      if (merc.ID == mercenaryDbId)
      {
        CollectionUtils.PopulateTeamPreviewData(this.GetTeamPreviewDataModel(), this.m_selectedTeam, (List<int>) null, false);
        break;
      }
    }
  }

  public void ShowMercDetailsDisplay(LettuceMercenary mercenary)
  {
    if ((UnityEngine.Object) this.MercenaryDetailDisplay == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintError("ShowMercDetailsDisplay - MercenaryDetailDisplay is null");
    }
    else
    {
      if (this.m_selectedTeam != null)
      {
        LettuceTeamDataModel previewDataModel = this.GetTeamPreviewDataModel();
        CollectionUtils.PopulateMercenariesTeamDataModel(previewDataModel, this.m_selectedTeam);
        this.MercenaryDetailDisplay.GetComponent<Widget>().BindDataModel((IDataModel) previewDataModel);
      }
      this.MercenaryDetailDisplay.Show(mercenary, (bool) UniversalInputManager.UsePhoneUI ? "SHOW_PARTIAL" : "SHOW_FULL", this.m_selectedTeam);
    }
  }

  private void OnDetailDisplayClosed() => CollectionManager.Get()?.GetEditingTeam()?.SendChanges();
}
