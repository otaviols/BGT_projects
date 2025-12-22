using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class MiscellaneousMenu : ButtonListMenu
{
  [CustomEditField(Sections = "Template Items")]
  public Transform m_menuBone;
  public Material m_redButtonMaterial;
  private static MiscellaneousMenu s_instance;
  private UIBButton m_creditsButton;
  private UIBButton m_restorePurchasesButton;
  private UIBButton m_skipNprButton;

  protected override void Awake()
  {
    this.m_menuParent = this.m_menuBone;
    this.m_targetLayer = GameLayer.HighPriorityUI;
    base.Awake();
    MiscellaneousMenu.s_instance = this;
    this.m_creditsButton = this.CreateMenuButton("CreditsButton", "GLOBAL_OPTIONS_CREDITS", new UIEvent.Handler(this.OnCreditsButtonReleased));
    this.m_restorePurchasesButton = this.CreateMenuButton("RestorePurchasesButton", "GLOBAL_OPTIONS_RESTORE_PURCHASES", new UIEvent.Handler(this.OnRestorePurchasesButtonReleased));
    this.m_skipNprButton = this.CreateMenuButton("SkipNprButton", RankMgr.Get().UseLegacyRankedPlay() ? "GLOBAL_OPTIONS_SKIP_TO_RANK_25" : "GLOBAL_OPTIONS_SKIP_NPR", new UIEvent.Handler(this.OnSkipNprButtonReleased));
    ButtonListMenu.MakeButtonRed(this.m_skipNprButton, this.m_redButtonMaterial);
    this.m_menu.m_headerText.Text = GameStrings.Get("GLOBAL_OPTIONS_MISCELLANEOUS_LABEL");
  }

  public static MiscellaneousMenu Get() => MiscellaneousMenu.s_instance;

  protected override List<UIBButton> GetButtons()
  {
    List<UIBButton> buttons = new List<UIBButton>();
    buttons.Add(this.m_creditsButton);
    if (PlatformSettings.OS == OSCategory.iOS)
      buttons.Add(this.m_restorePurchasesButton);
    if (RankMgr.Get().CanPromoteSelfManually() && !CollectionManager.Get().IsInEditMode() && Network.IsLoggedIn())
      buttons.Add(this.m_skipNprButton);
    return buttons;
  }

  private void OnCreditsButtonReleased(UIEvent e)
  {
    this.Hide();
    if ((UnityEngine.Object) NarrativeManager.Get() != (UnityEngine.Object) null && NarrativeManager.Get().IsShowingBlockingDialog() || SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN)
      return;
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.CREDITS);
  }

  private void OnPrivacyPolicyButtonReleased(UIEvent e) => Application.OpenURL(ExternalUrlService.Get().GetPrivacyPolicyLink());

  private void OnRestorePurchasesButtonReleased(UIEvent e)
  {
    this.Hide();
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_OPTIONS_RESTORE_PURCHASES"),
      m_text = GameStrings.Get("GLOBAL_OPTIONS_RESTORE_PURCHASES_POPUP_TEXT"),
      m_confirmText = GameStrings.Get("GLOBAL_SWITCH_ACCOUNT"),
      m_cancelText = GameStrings.Get("GLOBAL_BACK"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response != AlertPopup.Response.CONFIRM)
          return;
        GameUtils.LogoutConfirmation();
      })
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void OnSkipNprButtonReleased(UIEvent e)
  {
    this.Hide();
    DialogManager.Get().ShowLeaguePromoteSelfManuallyDialog((LeaguePromoteSelfManuallyDialog.ResponseCallback) (() =>
    {
      if (!Network.IsLoggedIn())
        DialogManager.Get().ShowReconnectHelperDialog(new Action(this.RequestLeaguePromoteSelf));
      else
        this.RequestLeaguePromoteSelf();
    }));
  }

  private void RequestLeaguePromoteSelf()
  {
    RankMgr.Get().DidPromoteSelfThisSession = true;
    Network.Get().RegisterNetHandler((object) LeaguePromoteSelfResponse.PacketID.ID, new Network.NetHandler(this.OnLeaguePromoteSelfResponse));
    Network.Get().RequestLeaguePromoteSelf();
  }

  private void OnTransitionToHubComplete_ShowRankedIntro(object userData)
  {
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnTransitionToHubComplete_ShowRankedIntro));
    PopupDisplayManager.Get().ShowRankedIntro();
  }

  public void OnLeaguePromoteSelfResponse()
  {
    Network.Get().RemoveNetHandler((object) LeaguePromoteSelfResponse.PacketID.ID, new Network.NetHandler(this.OnLeaguePromoteSelfResponse));
    LeaguePromoteSelfResponse promoteSelfResponse = Network.Get().GetLeaguePromoteSelfResponse();
    if (promoteSelfResponse.ErrorCode == PegasusShared.ErrorCode.ERROR_OK)
    {
      RankMgr.Get().DidPromoteSelfThisSession = true;
      Network.Get().RegisterNetHandler((object) MedalInfo.PacketID.ID, new Network.NetHandler(this.OnMedalInfoResponse));
      NetCache.Get().RefreshNetObject<NetCache.NetCacheMedalInfo>();
    }
    else
    {
      RankMgr.Get().DidPromoteSelfThisSession = false;
      Log.All.PrintError("Player not able to Skip NPR. Player={0}, Error={1}", (object) BnetPresenceMgr.Get().GetMyPlayer().GetAccountId(), (object) promoteSelfResponse.ErrorCode);
    }
  }

  public void OnMedalInfoResponse()
  {
    Network.Get().RemoveNetHandler((object) MedalInfo.PacketID.ID, new Network.NetHandler(this.OnMedalInfoResponse));
    if (SetRotationManager.Get().ShouldShowSetRotationIntro())
    {
      if (SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB)
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      else
        Box.Get().TryToStartSetRotationFromHub();
      Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnTransitionToHubComplete_ShowRankedIntro));
    }
    else if (SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB)
    {
      Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnTransitionToHubComplete_ShowRankedIntro));
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
    else
      this.OnTransitionToHubComplete_ShowRankedIntro((object) null);
  }
}
