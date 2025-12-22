using Blizzard.T5.Jobs;
using Hearthstone.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyMenu : ButtonListMenu
{
  [CustomEditField(Sections = "Template Items")]
  public Transform m_menuBone;
  private static PrivacyMenu s_instance;
  private PrivacySettingsMenu m_privacySettingsMenu;
  private UIBButton m_privacyRightsButton;
  private UIBButton m_privacyPolicyButton;
  private UIBButton m_privacySettingsButton;
  private UIBButton m_accountDeletionButton;
  private static readonly AssetReference OPTIONS_MENU_PRIVACY_SETTINGS = new AssetReference("OptionsMenuPrivacySettings.prefab:bb3df91bd8b46004db4fb741957b1eb4");
  private static readonly AssetReference OPTIONS_MENU_PRIVACY_SETTINGS_PHONE = new AssetReference("OptionsMenuPrivacySettings_phone.prefab:c524014467fe1604eac8d35a4952b7e1");

  private bool IsOpeningDataManagementLink { get; set; }

  protected override void Awake()
  {
    this.m_menuParent = this.m_menuBone;
    this.m_targetLayer = GameLayer.HighPriorityUI;
    base.Awake();
    PrivacyMenu.s_instance = this;
    this.m_privacyRightsButton = this.CreateMenuButton("PrivacyRightsButton", "GLOBAL_AADC_PRIVACYSETTINGSMENU_PRIVACYRIGHTS", new UIEvent.Handler(this.OnPrivacyRightsButtonReleased));
    this.m_privacyPolicyButton = this.CreateMenuButton("PrivacyPolicyButton", "GLUE_PRIVACY_POLICY_TITLE", new UIEvent.Handler(this.OnPrivacyPolicyButtonReleased));
    this.m_privacySettingsButton = this.CreateMenuButton("PrivacySettingsButton", "GLOBAL_AADC_BUTTON_PRIVACYSETTINGS", new UIEvent.Handler(this.OnPrivacySettingsButtonReleased));
    this.m_accountDeletionButton = this.IsAccountDeletionEnabled ? this.CreateMenuButton("AccountDeletionButton", "GLUE_DELETE_ACCOUNT", new UIEvent.Handler(this.OnAccountDeleteButtonReleased)) : (UIBButton) null;
    this.m_menu.m_headerText.Text = GameStrings.Get("GLOBAL_AADC_BUTTON_PRIVACY");
  }

  public static PrivacyMenu Get() => PrivacyMenu.s_instance;

  protected override List<UIBButton> GetButtons()
  {
    List<UIBButton> buttons = new List<UIBButton>();
    buttons.Add(this.m_privacyRightsButton);
    buttons.Add(this.m_privacyPolicyButton);
    buttons.Add(this.m_privacySettingsButton);
    if ((UnityEngine.Object) this.m_accountDeletionButton != (UnityEngine.Object) null)
      buttons.Add(this.m_accountDeletionButton);
    return buttons;
  }

  private IEnumerator<IAsyncJobResult> Job_OpenDataManagementLink()
  {
    GenerateSSOToken tokenGenerator = new GenerateSSOToken();
    yield return (IAsyncJobResult) tokenGenerator;
    if (!tokenGenerator.HasToken)
      yield return (IAsyncJobResult) new JobFailedResult("Could not generate SSO token to open data management link", Array.Empty<object>());
    Application.OpenURL(ExternalUrlService.Get().GetDataManagementLink(tokenGenerator.Token));
  }

  private void OnPrivacyRightsButtonReleased(UIEvent e)
  {
    if (this.IsOpeningDataManagementLink)
      return;
    this.IsOpeningDataManagementLink = true;
    Processor.QueueJob("OpenDataManagementLink", this.Job_OpenDataManagementLink()).AddJobFinishedEventListener((JobDefinition.JobFinishedEventListener) ((job, success) => this.IsOpeningDataManagementLink = false));
  }

  private void OnPrivacyPolicyButtonReleased(UIEvent e) => Application.OpenURL(ExternalUrlService.Get().GetPrivacyPolicyLink());

  private void OnPrivacySettingsButtonReleased(UIEvent e)
  {
    this.Hide();
    if ((UnityEngine.Object) this.m_privacySettingsMenu == (UnityEngine.Object) null)
      this.m_privacySettingsMenu = !PlatformSettings.IsMobile() ? AssetLoader.Get().InstantiatePrefab(PrivacyMenu.OPTIONS_MENU_PRIVACY_SETTINGS).GetComponent<PrivacySettingsMenu>() : AssetLoader.Get().InstantiatePrefab(PrivacyMenu.OPTIONS_MENU_PRIVACY_SETTINGS_PHONE).GetComponent<PrivacySettingsMenu>();
    this.m_privacySettingsMenu.Show();
  }

  private void OnAccountDeleteButtonReleased(UIEvent e)
  {
    if (TemporaryAccountManager.IsTemporaryAccount())
      Processor.QueueJobIfNotExist("Job_OpenSoftAccountDeletionLink", this.Job_OpenSoftAccountDeletionLink());
    else
      Application.OpenURL(ExternalUrlService.Get().GetAccountDeletionLink());
  }

  private IEnumerator<IAsyncJobResult> Job_OpenSoftAccountDeletionLink()
  {
    GenerateSSOToken tokenGenerator = new GenerateSSOToken();
    yield return (IAsyncJobResult) tokenGenerator;
    if (!tokenGenerator.HasToken)
      yield return (IAsyncJobResult) new JobFailedResult("Could not generate SSO token to open account deletion link", Array.Empty<object>());
    Application.OpenURL(ExternalUrlService.Get().GetSoftAccountDeletionLink(tokenGenerator.Token));
  }

  private bool IsAccountDeletionEnabled => PlatformSettings.OS == OSCategory.iOS;
}
