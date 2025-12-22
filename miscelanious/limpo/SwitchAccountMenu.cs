using Blizzard.MobileAuth;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAccountMenu : ButtonListMenu
{
  private const int TEMPORARY_ACCOUNT_SHOWN_LIMIT = 4;
  public Transform m_menuBone;
  private List<UIBButton> m_temporaryAccountButtons;
  private SwitchAccountMenu.OnSwitchAccountLogInPressed m_onSwitchAccountLoginInPressedHandler;

  protected override void Awake()
  {
    this.m_menuParent = this.m_menuBone;
    this.m_showAnimation = false;
    base.Awake();
    this.m_menu.m_headerText.Text = GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_SWITCH_ACCOUNT_HEADER");
    this.m_temporaryAccountButtons = new List<UIBButton>();
  }

  protected override void OnDestroy()
  {
  }

  public void Show(
    SwitchAccountMenu.OnSwitchAccountLogInPressed onSwitchAccountLogInPressedHandler)
  {
    this.m_onSwitchAccountLoginInPressedHandler = onSwitchAccountLogInPressedHandler;
    this.Show();
  }

  public void AddTemporaryAccountButtons(
    IEnumerable<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount> sortedTemporaryAccounts,
    string selectedTemporaryAccountId)
  {
    this.m_temporaryAccountButtons.Clear();
    this.m_temporaryAccountButtons.Add(this.CreateMenuButton("Log In", "GLOBAL_LOGIN", new UIEvent.Handler(this.OnLogInButtonPressed)));
    this.m_temporaryAccountButtons.Add((UIBButton) null);
    int num = 0;
    foreach (TemporaryAccountManager.TemporaryAccountData.TemporaryAccount temporaryAccount in sortedTemporaryAccounts)
    {
      if (num >= 4)
        break;
      if ((selectedTemporaryAccountId == null || !string.Equals(selectedTemporaryAccountId, temporaryAccount.m_temporaryAccountId)) && !temporaryAccount.m_isHealedUp)
      {
        UIBButton menuButton = this.CreateMenuButton("TemporaryAccountButton" + num.ToString(), temporaryAccount.m_battleTag, new UIEvent.Handler(this.OnTemporaryAccountButtonPressed));
        menuButton.SetData((object) temporaryAccount);
        this.m_temporaryAccountButtons.Add(menuButton);
        ++num;
      }
    }
  }

  public void AddAccountButtons(IEnumerable<Account> sortedAccounts)
  {
    this.m_temporaryAccountButtons.Clear();
    this.m_temporaryAccountButtons.Add(this.CreateMenuButton("Log In", "GLOBAL_LOGIN", new UIEvent.Handler(this.OnLogInButtonPressed)));
    this.m_temporaryAccountButtons.Add((UIBButton) null);
    int num = 0;
    foreach (Account sortedAccount in sortedAccounts)
    {
      if (num >= 4)
        break;
      UIBButton menuButton = this.CreateMenuButton("TemporaryAccountButton" + num.ToString(), sortedAccount.displayName, new UIEvent.Handler(this.OnTemporaryAccountButtonPressed));
      menuButton.SetData((object) sortedAccount);
      this.m_temporaryAccountButtons.Add(menuButton);
      ++num;
    }
  }

  protected override List<UIBButton> GetButtons() => this.m_temporaryAccountButtons;

  private void OnLogInButtonPressed(UIEvent e)
  {
    if (this.m_onSwitchAccountLoginInPressedHandler != null)
    {
      this.m_onSwitchAccountLoginInPressedHandler((object) null);
      this.m_onSwitchAccountLoginInPressedHandler = (SwitchAccountMenu.OnSwitchAccountLogInPressed) null;
    }
    this.Hide();
  }

  private void OnTemporaryAccountButtonPressed(UIEvent e)
  {
    object data = e.GetElement().GetData();
    this.Hide();
    if (this.m_onSwitchAccountLoginInPressedHandler == null)
      return;
    this.m_onSwitchAccountLoginInPressedHandler(data);
    this.m_onSwitchAccountLoginInPressedHandler = (SwitchAccountMenu.OnSwitchAccountLogInPressed) null;
  }

  public delegate void OnSwitchAccountLogInPressed(object account);
}
