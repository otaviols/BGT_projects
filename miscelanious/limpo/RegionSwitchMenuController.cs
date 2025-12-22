using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Services;
using Hearthstone.Login;
using System.Collections.Generic;
using UnityEngine;

public class RegionSwitchMenuController
{
  private RegionMenu m_regionMenu;
  private const int BUTTON_COUNT = 3;
  private const string WARNING_PREFAB = "RegionSelect.prefab:a29650226d94fae408628b0c5aad1348";
  private const string REGION_MENU_PREFAB = "RegionMenu.prefab:81394e6ea3adb1140a29ff4b44744891";
  private static readonly PlatformDependentValue<float> WARNING_PADDING = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 60f,
    Phone = 80f
  };

  public void ShowRegionMenuWithDefaultSettings()
  {
    if (this.ShouldSkipRegionSwitch())
      GameUtils.LogoutConfirmation();
    else if (PlatformSettings.LocaleVariant == LocaleVariant.China)
      this.SwitchRegion(BnetRegion.REGION_CN, true);
    else
      this.ShowRegionMenu(RegionSwitchMenuController.CreateDefaultSettings());
  }

  private bool ShouldSkipRegionSwitch() => ServiceManager.Get<ILoginService>() == null || !PlatformSettings.IsMobileRuntimeOS;

  private static RegionSwitchMenuController.RegionMenuSettings CreateDefaultSettings() => new RegionSwitchMenuController.RegionMenuSettings()
  {
    CurrentRegion = BattleNet.GetCurrentRegion(),
    Buttons = RegionSwitchMenuController.CreateDefaultRegionButtons()
  };

  private static List<RegionSwitchMenuController.RegionMenuSettings.RegionButtonSetting> CreateDefaultRegionButtons() => new List<RegionSwitchMenuController.RegionMenuSettings.RegionButtonSetting>(3)
  {
    new RegionSwitchMenuController.RegionMenuSettings.RegionButtonSetting()
    {
      Region = BnetRegion.REGION_US,
      ButtonLabel = "GLOBAL_REGION_AMERICAS"
    },
    new RegionSwitchMenuController.RegionMenuSettings.RegionButtonSetting()
    {
      Region = BnetRegion.REGION_EU,
      ButtonLabel = "GLOBAL_REGION_EUROPE"
    },
    new RegionSwitchMenuController.RegionMenuSettings.RegionButtonSetting()
    {
      Region = BnetRegion.REGION_KR,
      ButtonLabel = "GLOBAL_REGION_ASIA"
    }
  };

  public void ShowRegionMenu(
    RegionSwitchMenuController.RegionMenuSettings settings)
  {
    if ((Object) this.m_regionMenu != (Object) null && this.m_regionMenu.IsShown())
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "RegionMenu.prefab:81394e6ea3adb1140a29ff4b44744891", new PrefabCallback<GameObject>(this.OnMenuLoaded), (object) settings);
  }

  private void OnMenuLoaded(AssetReference assetRef, GameObject instance, object callbackData)
  {
    this.m_regionMenu = instance.GetComponent<RegionMenu>();
    if ((Object) this.m_regionMenu == (Object) null)
    {
      Log.Login.PrintError("Could not load Region Menu game object");
      Object.Destroy((Object) instance);
    }
    else if (callbackData == null || !(callbackData is RegionSwitchMenuController.RegionMenuSettings settings))
    {
      Log.Login.PrintError("No region menu settings found");
      Object.Destroy((Object) instance);
    }
    else
      this.SetMenuButtonsAndShow(settings);
  }

  private void SetMenuButtonsAndShow(
    RegionSwitchMenuController.RegionMenuSettings settings)
  {
    List<UIBButton> buttons = new List<UIBButton>(3);
    foreach (RegionSwitchMenuController.RegionMenuSettings.RegionButtonSetting button in settings.Buttons)
    {
      RegionSwitchMenuController.RegionMenuSettings.RegionButtonSetting buttonSettings = button;
      buttons.Add(this.m_regionMenu.CreateMenuButton((string) null, buttonSettings.ButtonLabel, (UIEvent.Handler) (_ => this.OnRegionButtonPressed(buttonSettings.Region, settings.CurrentRegion))));
    }
    this.m_regionMenu.SetButtons(buttons);
    this.m_regionMenu.Show(true);
  }

  private void OnRegionButtonPressed(BnetRegion selectedRegion, BnetRegion currentRegion)
  {
    this.m_regionMenu.Hide();
    if (selectedRegion != currentRegion)
      this.ShowRegionWarningDialog(selectedRegion);
    else
      this.SwitchRegion(selectedRegion, true);
  }

  private void ShowRegionWarningDialog(BnetRegion region)
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_MOBILE_REGION_SELECT_WARNING_HEADER"),
      m_text = GameStrings.Get("GLUE_MOBILE_REGION_SELECT_WARNING"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnRegionWarningResponse),
      m_padding = (float) RegionSwitchMenuController.WARNING_PADDING,
      m_responseUserData = (object) region
    };
    DialogManager.Get().ShowPopup(info, new DialogManager.DialogProcessCallback(this.OnDialogProcess));
  }

  private bool OnDialogProcess(DialogBase dialog, object userData)
  {
    ((GameObject) GameUtils.InstantiateGameObject("RegionSelect.prefab:a29650226d94fae408628b0c5aad1348", dialog.gameObject)).SetActive(true);
    return true;
  }

  private void OnRegionWarningResponse(AlertPopup.Response response, object userData)
  {
    if (response != AlertPopup.Response.CONFIRM)
      return;
    this.SwitchRegion((BnetRegion) userData, false);
  }

  private void SwitchRegion(BnetRegion region, bool requestConfirmation)
  {
    Options.Get().SetInt(Option.PREFERRED_REGION, (int) region);
    if (requestConfirmation)
      GameUtils.LogoutConfirmation();
    else
      GameUtils.Logout();
  }

  public struct RegionMenuSettings
  {
    public List<RegionSwitchMenuController.RegionMenuSettings.RegionButtonSetting> Buttons { get; set; }

    public BnetRegion CurrentRegion { get; set; }

    public struct RegionButtonSetting
    {
      public string ButtonLabel { get; set; }

      public BnetRegion Region { get; set; }
    }
  }
}
