using Blizzard.GameService.SDK.Client.Integration;
using UnityEngine;

public class FriendlyChallengeHelper
{
  private static FriendlyChallengeHelper s_instance;
  private AlertPopup m_friendChallengeWaitingPopup;
  private AlertPopup m_deckShareRequestWaitingPopup;
  private AlertPopup m_deckShareRequestDeclinedPopup;
  private AlertPopup m_deckShareRequestCanceledPopup;
  private AlertPopup m_deckShareRequestPopup;
  private AlertPopup m_deckShareErrorPopup;

  public BnetAccountId ActiveChallengeMenu
  {
    set => this.\u003CActiveChallengeMenu\u003Ek__BackingField = value;
  }

  public static FriendlyChallengeHelper Get()
  {
    if (FriendlyChallengeHelper.s_instance == null)
      FriendlyChallengeHelper.s_instance = new FriendlyChallengeHelper();
    return FriendlyChallengeHelper.s_instance;
  }

  public void StartChallengeOrWaitForOpponent(
    string waitingDialogText,
    AlertPopup.ResponseCallback waitingCallback)
  {
    if (FriendChallengeMgr.Get().DidOpponentSelectDeckOrHero())
      return;
    this.ShowFriendChallengeWaitingForOpponentDialog(waitingDialogText, waitingCallback);
  }

  public void HideFriendChallengeWaitingForOpponentDialog()
  {
    if ((Object) this.m_friendChallengeWaitingPopup == (Object) null)
      return;
    this.m_friendChallengeWaitingPopup.Hide();
    this.m_friendChallengeWaitingPopup = (AlertPopup) null;
  }

  public void WaitForFriendChallengeToStart()
  {
    int brawlLibraryItemId = 0;
    if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
    {
      TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
      if (tavernBrawlMission != null)
        brawlLibraryItemId = tavernBrawlMission.SelectedBrawlLibraryItemId;
    }
    GameMgr.Get().WaitForFriendChallengeToStart(FriendChallengeMgr.Get().GetFormatType(), FriendChallengeMgr.Get().GetChallengeBrawlType(), FriendChallengeMgr.Get().GetScenarioId(), brawlLibraryItemId, FriendChallengeMgr.Get().IsChallengeBacon() ? PartyType.BATTLEGROUNDS_PARTY : PartyType.FRIENDLY_CHALLENGE);
  }

  public void StopWaitingForFriendChallenge() => this.HideFriendChallengeWaitingForOpponentDialog();

  public void HideAllDeckShareDialogs()
  {
    this.HideDeckShareRequestDialog();
    this.HideDeckShareRequestCanceledDialog();
    this.HideDeckShareRequestDeclinedDialog();
    this.HideDeckShareRequestWaitingDialog();
    this.HideDeckShareErrorDialog();
  }

  public void ShowDeckShareRequestCanceledDialog()
  {
    BnetPlayer myOpponent = FriendChallengeMgr.Get().GetMyOpponent();
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLOBAL_DECK_SHARE_HEADER");
    info.m_text = GameStrings.Format("GLOBAL_DECK_SHARE_REQUEST_CANCELED", (object) myOpponent.GetBestName());
    info.m_showAlertIcon = false;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM;
    DialogManager.DialogProcessCallback callback = (DialogManager.DialogProcessCallback) ((dialog, userData) =>
    {
      if (!FriendChallengeMgr.Get().HasChallenge())
        return false;
      this.m_deckShareRequestCanceledPopup = (AlertPopup) dialog;
      return true;
    });
    DialogManager.Get().ShowPopup(info, callback);
  }

  public void HideDeckShareRequestCanceledDialog()
  {
    if ((Object) this.m_deckShareRequestCanceledPopup == (Object) null)
      return;
    this.m_deckShareRequestCanceledPopup.Hide();
    this.m_deckShareRequestCanceledPopup = (AlertPopup) null;
  }

  public void ShowDeckShareRequestDeclinedDialog()
  {
    BnetPlayer myOpponent = FriendChallengeMgr.Get().GetMyOpponent();
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLOBAL_DECK_SHARE_HEADER");
    info.m_text = GameStrings.Format("GLOBAL_DECK_SHARE_REQUEST_DECLINED", (object) myOpponent.GetBestName());
    info.m_showAlertIcon = false;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM;
    DialogManager.DialogProcessCallback callback = (DialogManager.DialogProcessCallback) ((dialog, userData) =>
    {
      if (!FriendChallengeMgr.Get().HasChallenge())
        return false;
      this.m_deckShareRequestDeclinedPopup = (AlertPopup) dialog;
      return true;
    });
    DialogManager.Get().ShowPopup(info, callback);
  }

  public void HideDeckShareRequestDeclinedDialog()
  {
    if ((Object) this.m_deckShareRequestDeclinedPopup == (Object) null)
      return;
    this.m_deckShareRequestDeclinedPopup.Hide();
    this.m_deckShareRequestDeclinedPopup = (AlertPopup) null;
  }

  public void ShowDeckShareRequestWaitingDialog(AlertPopup.ResponseCallback waitingCallback)
  {
    BnetPlayer myOpponent = FriendChallengeMgr.Get().GetMyOpponent();
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLOBAL_DECK_SHARE_HEADER");
    info.m_text = GameStrings.Format("GLOBAL_DECK_SHARE_REQUEST_WAITING_RESPONSE", (object) myOpponent.GetBestName());
    info.m_showAlertIcon = false;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
    info.m_responseCallback = waitingCallback;
    DialogManager.DialogProcessCallback callback = (DialogManager.DialogProcessCallback) ((dialog, userData) =>
    {
      if (!FriendChallengeMgr.Get().HasChallenge())
        return false;
      this.m_deckShareRequestWaitingPopup = (AlertPopup) dialog;
      return true;
    });
    DialogManager.Get().ShowPopup(info, callback);
  }

  public void HideDeckShareRequestWaitingDialog()
  {
    if ((Object) this.m_deckShareRequestWaitingPopup == (Object) null)
      return;
    this.m_deckShareRequestWaitingPopup.Hide();
    this.m_deckShareRequestWaitingPopup = (AlertPopup) null;
  }

  public void ShowDeckShareRequestDialog(AlertPopup.ResponseCallback waitingCallback)
  {
    BnetPlayer myOpponent = FriendChallengeMgr.Get().GetMyOpponent();
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLOBAL_DECK_SHARE_HEADER");
    info.m_text = GameStrings.Format("GLOBAL_DECK_SHARE_REQUESTED", (object) myOpponent.GetBestName());
    info.m_showAlertIcon = false;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
    info.m_responseCallback = waitingCallback;
    info.m_confirmText = GameStrings.Get("GLOBAL_DECK_SHARE_ACCEPT_REQUEST");
    info.m_cancelText = GameStrings.Get("GLOBAL_DECK_SHARE_DECLINE_REQUEST");
    DialogManager.DialogProcessCallback callback = (DialogManager.DialogProcessCallback) ((dialog, userData) =>
    {
      if (!FriendChallengeMgr.Get().HasChallenge())
        return false;
      this.m_deckShareRequestPopup = (AlertPopup) dialog;
      return true;
    });
    DialogManager.Get().ShowPopup(info, callback);
  }

  public bool IsShowingDeckShareRequestDialog() => (Object) this.m_deckShareRequestPopup != (Object) null;

  public void HideDeckShareRequestDialog()
  {
    if ((Object) this.m_deckShareRequestPopup == (Object) null)
      return;
    this.m_deckShareRequestPopup.Hide();
    this.m_deckShareRequestPopup = (AlertPopup) null;
  }

  public void ShowDeckShareErrorDialog()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLOBAL_DECK_SHARE_HEADER");
    info.m_text = GameStrings.Get("GLOBAL_DECK_SHARE_ERROR");
    info.m_showAlertIcon = false;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM;
    DialogManager.DialogProcessCallback callback = (DialogManager.DialogProcessCallback) ((dialog, userData) =>
    {
      if (!FriendChallengeMgr.Get().HasChallenge())
        return false;
      this.m_deckShareErrorPopup = (AlertPopup) dialog;
      return true;
    });
    DialogManager.Get().ShowPopup(info, callback);
  }

  public void HideDeckShareErrorDialog()
  {
    if ((Object) this.m_deckShareErrorPopup == (Object) null)
      return;
    this.m_deckShareErrorPopup.Hide();
    this.m_deckShareErrorPopup = (AlertPopup) null;
  }

  private void ShowFriendChallengeWaitingForOpponentDialog(
    string dialogText,
    AlertPopup.ResponseCallback callback)
  {
    BnetPlayer myOpponent = FriendChallengeMgr.Get().GetMyOpponent();
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_text = GameStrings.Format(dialogText, (object) FriendUtils.GetUniqueName(myOpponent)),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL,
      m_responseCallback = callback
    }, new DialogManager.DialogProcessCallback(this.OnFriendChallengeWaitingForOpponentDialogProcessed));
  }

  private bool OnFriendChallengeWaitingForOpponentDialogProcessed(
    DialogBase dialog,
    object userData)
  {
    if (!FriendChallengeMgr.Get().HasChallenge() || FriendChallengeMgr.Get().DidOpponentSelectDeckOrHero())
      return false;
    this.m_friendChallengeWaitingPopup = (AlertPopup) dialog;
    return true;
  }
}
