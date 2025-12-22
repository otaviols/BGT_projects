using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone;
using UnityEngine;

public class GameMenuBase
{
  public GameMenuBase.ShowCallback m_showCallback;
  public GameMenuBase.HideCallback m_hideCallback;
  private const string OPTIONS_MENU_NAME = "OptionsMenu.prefab:a6e5621068fd7c8429475b3e1a1aa991";
  private OptionsMenu m_optionsMenu;

  public void ShowOptionsMenu()
  {
    if (this.m_hideCallback != null)
      this.m_hideCallback();
    if ((Object) this.m_optionsMenu == (Object) null)
    {
      this.m_optionsMenu = AssetLoader.Get().InstantiatePrefab((AssetReference) "OptionsMenu.prefab:a6e5621068fd7c8429475b3e1a1aa991").GetComponent<OptionsMenu>();
      if (!((Object) this.m_optionsMenu != (Object) null))
        return;
      this.SwitchToOptionsMenu();
    }
    else
      this.SwitchToOptionsMenu();
  }

  public void DestroyOptionsMenu()
  {
    if (!((Object) this.m_optionsMenu != (Object) null))
      return;
    this.m_optionsMenu.RemoveHideHandler(new OptionsMenu.hideHandler(this.OnOptionsMenuHidden));
  }

  public bool UseKoreanRating()
  {
    if (SceneMgr.Get().IsInGame())
      return false;
    bool flag = BattleNet.GetAccountCountry() == "KOR";
    if (PlatformSettings.IsMobile() && !flag)
      flag = MobileDeviceLocale.GetCountryCode() == "KR";
    return flag;
  }

  private void SwitchToOptionsMenu()
  {
    this.m_optionsMenu.SetHideHandler(new OptionsMenu.hideHandler(this.OnOptionsMenuHidden));
    this.m_optionsMenu.Show();
  }

  private void OnOptionsMenuHidden()
  {
    Object.Destroy((Object) this.m_optionsMenu.gameObject);
    this.m_optionsMenu = (OptionsMenu) null;
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR) || HearthstoneApplication.Get().IsResetting() || !BnetBar.Get().AreButtonsEnabled() || this.m_showCallback == null)
      return;
    this.m_showCallback();
  }

  public delegate void ShowCallback();

  public delegate void HideCallback();
}
