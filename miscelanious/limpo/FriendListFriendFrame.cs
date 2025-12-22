using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class FriendListFriendFrame : MonoBehaviour
{
  private const float REFRESH_FRIENDS_SECONDS = 30f;
  private const bool TabletShouldCloseFriendListOnCloseChatUI = false;
  public PlayerIcon m_playerIcon;
  public FriendListChatIcon m_chatIcon;
  public Widget m_challengeButtonWidget;
  public Widget m_selectableMedalWidget;
  [SerializeField]
  private AsyncReference m_friendFlyoutMenuReference;
  private WidgetTemplate m_widget;
  private Clickable m_clickable;
  private FriendListChallengeButton m_challengeButton;
  private Widget m_friendFlyoutMenuWidget;
  private VisualController m_challengeButtonVisualController;
  private Clickable m_challengeButtonClickable;
  private bool m_isRecentPlayerFrame;
  private BnetPlayer m_player;
  private MedalInfoTranslator m_rankedMedalInfo;
  private SelectableMedal m_selectableMedal;
  private FriendDataModel m_friendDataModel;
  private Coroutine m_friendUpdateCoroutine;

  private void Awake()
  {
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    BnetWhisperMgr.Get().AddWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
    ChatMgr.Get().AddPlayerChatInfoChangedListener(new ChatMgr.PlayerChatInfoChangedCallback(this.OnPlayerChatInfoChanged));
    PartyManager.Get().AddChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    DialogManager.Get().OnDialogHidden += new System.Action(this.UpdateFriend);
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterReadyListener(new System.Action<object>(this.OnFriendFrameWidgetReady), (object) null, true);
    this.m_friendDataModel = new FriendDataModel();
    this.m_widget.BindDataModel((IDataModel) this.m_friendDataModel, false);
    this.m_challengeButtonWidget.RegisterReadyListener(new System.Action<object>(this.OnChallengeButtonWidgetReady), (object) null, true);
    this.m_selectableMedalWidget.RegisterReadyListener(new System.Action<object>(this.OnSelectableMedalWidgetReady), (object) null, true);
    this.m_friendFlyoutMenuReference.RegisterReadyListener<Widget>((System.Action<Widget>) (widget => this.m_friendFlyoutMenuWidget = widget));
    this.m_friendUpdateCoroutine = this.StartCoroutine(this.RefreshFriend());
    ChatMgr.Get().OnChatLogShown += new System.Action(this.CloseChallengeMenu);
  }

  private void OnEnable()
  {
    this.StopCoroutine(this.m_friendUpdateCoroutine);
    this.m_friendUpdateCoroutine = this.StartCoroutine(this.RefreshFriend());
  }

  private void OnDisable() => this.CloseChallengeMenu();

  private void OnDestroy()
  {
    if (BnetPresenceMgr.Get() != null)
      BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    if (BnetWhisperMgr.Get() != null)
      BnetWhisperMgr.Get().RemoveWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
    if ((UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null)
    {
      ChatMgr.Get().RemovePlayerChatInfoChangedListener(new ChatMgr.PlayerChatInfoChangedCallback(this.OnPlayerChatInfoChanged));
      ChatMgr.Get().OnChatLogShown -= new System.Action(this.CloseChallengeMenu);
    }
    if (PartyManager.Get() != null)
      PartyManager.Get().RemoveChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    if (!((UnityEngine.Object) DialogManager.Get() != (UnityEngine.Object) null))
      return;
    DialogManager.Get().OnDialogHidden -= new System.Action(this.UpdateFriend);
  }

  private IEnumerator RefreshFriend()
  {
    while (true)
    {
      this.UpdateFriend();
      yield return (object) new WaitForSeconds(30f);
    }
  }

  private void OnFriendFrameWidgetReady(object unused)
  {
    this.m_clickable = this.m_widget.FindWidgetComponent<Clickable>();
    this.m_clickable.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFriendFrameReleased));
    BoxCollider component = this.m_clickable.GetComponent<BoxCollider>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.size = TransformUtil.ComputeSetPointBounds(this.gameObject).size;
  }

  private void OnChallengeButtonWidgetReady(object unused)
  {
    this.m_challengeButton = this.m_challengeButtonWidget.FindWidgetComponent<FriendListChallengeButton>();
    this.m_challengeButton.SetPlayer(this.m_player);
    this.m_challengeButton.FriendFrame = this;
    this.m_challengeButtonVisualController = this.m_challengeButtonWidget.FindWidgetComponent<VisualController>();
    this.m_challengeButtonClickable = this.m_challengeButtonWidget.FindWidgetComponent<Clickable>();
    this.m_challengeButtonClickable.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChallengeButtonRelease));
    this.UpdateFriend();
  }

  private void OnSelectableMedalWidgetReady(object unused)
  {
    this.m_selectableMedal = this.m_selectableMedalWidget.GetComponentInChildren<SelectableMedal>();
    this.UpdatePlayerIcon();
  }

  public void Initialize(
    BnetPlayer player,
    bool isFSGPatron = false,
    bool isFSGInnkeeper = false,
    bool isRecentPlayerFrame = false)
  {
    this.m_player = player;
    this.m_playerIcon.SetPlayer(player);
    this.m_isRecentPlayerFrame = isRecentPlayerFrame;
    this.m_friendDataModel.IsFSGPatron = isFSGPatron;
    this.m_friendDataModel.IsFSGInnkeeper = isFSGInnkeeper;
    this.UpdateFriend();
    if (!this.m_widget.IsChangingStates)
      return;
    this.m_widget.Hide();
    this.m_widget.RegisterDoneChangingStatesListener(new System.Action<object>(this.OnWidgetDoneChangingStates), (object) null, true, false);
  }

  private void OnWidgetDoneChangingStates(object payload)
  {
    if (!this.m_widget.gameObject.activeInHierarchy || !this.m_widget.enabled || !this.m_widget.IsDesiredHidden)
      return;
    this.m_widget.Show();
  }

  public bool ShouldShowRankedMedal => this.m_rankedMedalInfo != null && this.m_rankedMedalInfo.IsDisplayable();

  public Widget GetWidget() => (Widget) this.m_widget;

  public BnetPlayer GetFriend() => this.m_player;

  public void InitializeMobileFriendListItem(MobileFriendListItem item) => item.OnScrollOutOfViewEvent += new System.Action(this.OnScrollOutOfView);

  public void OpenChallengeMenu()
  {
    if (ChatMgr.Get().IsChatLogUIShowing() && PlatformSettings.IsTablet)
    {
      ChatMgr.Get().CloseChatUI(false);
      ChatMgr.Get().UpdateLayout();
    }
    else
    {
      this.m_widget.TriggerEvent("OPEN_CHALLENGE_MENU", new Widget.TriggerEventParameters());
      if ((UnityEngine.Object) this.m_friendFlyoutMenuWidget != (UnityEngine.Object) null)
        this.m_friendFlyoutMenuWidget.gameObject.SetActive(true);
      ChatMgr.Get().FriendListFrame.CloseFlyoutMenu();
    }
  }

  public void CloseChallengeMenu()
  {
    if ((UnityEngine.Object) this.m_challengeButtonWidget != (UnityEngine.Object) null)
      this.m_challengeButtonWidget.TriggerEvent("CLOSE_CHALLENGE_MENU");
    if (!((UnityEngine.Object) this.m_friendFlyoutMenuWidget != (UnityEngine.Object) null))
      return;
    this.m_friendFlyoutMenuWidget.gameObject.SetActive(false);
  }

  public void DismissFlyoutAndPopups(bool showAlert)
  {
    this.CloseChallengeMenu();
    if (!((UnityEngine.Object) this.m_challengeButton != (UnityEngine.Object) null))
      return;
    this.m_challengeButton.DismissPopups(showAlert);
  }

  public void CloseFriendsListMenu() => ChatMgr.Get().CloseFriendsList();

  private void OnChallengeButtonRelease(UIEvent e)
  {
    string state = this.m_challengeButtonVisualController.State;
    if (!(state == "AVAILABLE"))
    {
      if (!(state == "DELETE"))
        return;
      this.OnDeleteFriendButtonPressed();
    }
    else
      this.OnAvailableButtonPressed();
  }

  private void OnFriendFrameReleased(UIEvent e)
  {
    if (ChatMgr.Get().FriendListFrame.IsInEditMode)
      return;
    FriendMgr.Get().SetSelectedFriend(this.m_player);
    if (!BnetFriendMgr.Get().IsFriend(this.m_player.GetAccountId()))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    ChatMgr.Get().OnFriendListFriendSelected(this.m_player);
  }

  private void OnAvailableButtonPressed()
  {
    if (this.m_challengeButton.IsChallengeMenuOpen)
      this.CloseChallengeMenu();
    else
      this.OpenChallengeMenu();
  }

  private void OnDeleteFriendButtonPressed() => ChatMgr.Get().FriendListFrame.ShowRemoveFriendPopup(this.m_player);

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (!changelist.HasChange(this.m_player))
      return;
    this.UpdateFriend();
  }

  private void OnWhisper(BnetWhisper whisper, object userData)
  {
    if (this.m_player == null || !WhisperUtil.IsSpeakerOrReceiver(this.m_player, whisper))
      return;
    this.UpdateFriend();
  }

  private void OnPlayerChatInfoChanged(PlayerChatInfo chatInfo, object userData)
  {
    if (this.m_player != chatInfo.GetPlayer())
      return;
    this.UpdateFriend();
  }

  private void OnScrollOutOfView()
  {
    if (!((UnityEngine.Object) this.m_challengeButton != (UnityEngine.Object) null))
      return;
    this.CloseChallengeMenu();
  }

  public void UpdateFriend()
  {
    if (this.m_player == null)
      return;
    BnetPlayer friend = BnetFriendMgr.Get().FindFriend(this.m_player.GetAccountId());
    this.m_friendDataModel.PlayerName = friend == null ? FriendUtils.GetFriendListName(this.m_player, false) : FriendUtils.GetFriendListName(friend, true);
    BnetGameAccount bestGameAccount = this.m_player.GetBestGameAccount();
    this.m_rankedMedalInfo = bestGameAccount == (BnetGameAccount) null ? (MedalInfoTranslator) null : RankMgr.Get().GetRankedMedalFromRankPresenceField(bestGameAccount);
    this.m_chatIcon.UpdateIcon();
    this.UpdatePresence();
    if (this.m_isRecentPlayerFrame)
    {
      this.m_friendDataModel.PlayerStatus = BnetRecentPlayerMgr.Get().GetRecentReason(this.m_player);
      if (BnetRecentPlayerMgr.Get().IsCurrentOpponent(this.m_player))
      {
        if (Options.Get().GetBool(Option.STREAMER_MODE))
          this.m_friendDataModel.PlayerName = GameStrings.Get("GAMEPLAY_MISSING_OPPONENT_NAME");
        else if (BnetRecentPlayerMgr.Get().IsRecentStranger(this.m_player) && !BnetNearbyPlayerMgr.Get().IsNearbyPlayer(this.m_player))
          this.m_friendDataModel.PlayerName = this.m_player.GetBestName();
      }
    }
    this.UpdatePlayerIcon();
    this.UpdateInteractionState();
  }

  private void UpdateInteractionState()
  {
    if ((UnityEngine.Object) this.m_challengeButtonWidget == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) this.m_challengeButton != (UnityEngine.Object) null)
      this.m_challengeButton.SetPlayer(this.m_player);
    if ((UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null && (UnityEngine.Object) ChatMgr.Get().FriendListFrame != (UnityEngine.Object) null && ChatMgr.Get().FriendListFrame.IsInEditMode && ChatMgr.Get().FriendListFrame.EditMode == FriendListFrame.FriendListEditMode.REMOVE_FRIENDS)
    {
      this.m_friendDataModel.IsInEditMode = true;
      this.m_challengeButtonWidget.TriggerEvent("DELETE");
    }
    else
    {
      this.m_challengeButtonWidget.TriggerEvent("AVAILABLE");
      if (!((UnityEngine.Object) this.m_challengeButton != (UnityEngine.Object) null))
        return;
      this.m_challengeButton.UpdateFlyoutMenu();
    }
  }

  private void UpdatePlayerIcon()
  {
    if (!this.m_player.IsOnline())
    {
      this.m_widget.TriggerEvent("LAYOUT_WITHOUT_ICON", new Widget.TriggerEventParameters());
    }
    else
    {
      BnetProgramId bestProgramId = this.m_player.GetBestProgramId();
      if ((Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId == (Blizzard.GameService.SDK.Client.Integration.FourCC) null || bestProgramId.IsPhoenix())
      {
        this.m_widget.TriggerEvent("LAYOUT_WITHOUT_ICON", new Widget.TriggerEventParameters());
      }
      else
      {
        this.m_widget.TriggerEvent("LAYOUT_WITH_ICON", new Widget.TriggerEventParameters());
        System.Action onDisplayNoMedal = (System.Action) (() =>
        {
          this.m_playerIcon.Show();
          this.m_playerIcon.UpdateIcon();
          this.m_selectableMedalWidget.gameObject.SetActive(false);
        });
        if ((Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE && !BnetRecentPlayerMgr.Get().IsRecentStranger(this.m_player))
        {
          this.m_playerIcon.Hide();
          this.m_selectableMedalWidget.gameObject.SetActive(true);
          this.m_selectableMedal?.UpdateWidget(this.m_player, onDisplayNoMedal: onDisplayNoMedal);
        }
        else
          onDisplayNoMedal();
      }
    }
  }

  protected void UpdatePresence()
  {
    if (this.m_isRecentPlayerFrame && BnetRecentPlayerMgr.Get().IsRecentStranger(this.m_player))
    {
      this.m_friendDataModel.IsOnline = true;
      this.m_friendDataModel.IsInHS = true;
    }
    else if (!this.m_player.IsOnline())
    {
      if (this.m_friendDataModel.IsOnline)
        this.DismissFlyoutAndPopups(true);
      this.m_friendDataModel.IsOnline = false;
      this.m_friendDataModel.PlayerStatus = FriendUtils.GetLastOnlineElapsedTimeString(this.m_player.GetBestLastOnlineMicrosec());
    }
    else
    {
      this.m_friendDataModel.IsOnline = true;
      BnetGameAccount hearthstoneGameAccount = this.m_player.GetHearthstoneGameAccount();
      if (hearthstoneGameAccount == (BnetGameAccount) null || !hearthstoneGameAccount.IsOnline())
      {
        BnetProgramId bestProgramId = this.m_player.GetBestProgramId();
        this.m_friendDataModel.PlayerStatus = !((Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId != (Blizzard.GameService.SDK.Client.Integration.FourCC) null) ? GameStrings.Get("GLOBAL_PROGRAMNAME_PHOENIX") : BnetUtils.GetNameForProgramId(bestProgramId);
        this.m_friendDataModel.IsInHS = false;
      }
      else
      {
        this.m_friendDataModel.IsInHS = true;
        if (this.m_player.IsAway())
        {
          this.m_friendDataModel.PlayerStatus = FriendUtils.GetAwayTimeString(this.m_player.GetBestAwayTimeMicrosec());
          this.m_friendDataModel.IsAway = true;
        }
        else
        {
          this.m_friendDataModel.IsAway = false;
          if (this.m_player.IsBusy())
          {
            this.m_friendDataModel.PlayerStatus = GameStrings.Get("GLOBAL_FRIENDLIST_BUSYSTATUS");
            this.m_friendDataModel.IsBusy = true;
          }
          else
          {
            this.m_friendDataModel.IsBusy = false;
            this.m_friendDataModel.PlayerStatus = PresenceMgr.Get().GetStatusText(this.m_player);
          }
        }
      }
    }
  }

  private void OnPartyChanged(
    PartyManager.PartyInviteEvent inviteEvent,
    BnetGameAccountId playerGameAccountId,
    PartyManager.PartyData data,
    object userData)
  {
    this.UpdateInteractionState();
  }
}
