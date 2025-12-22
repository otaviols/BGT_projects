using Blizzard.T5.Configuration;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMgr : IService
{
  private static DemoMgr s_instance;
  private DemoMode m_mode;
  private Notification m_demoText;
  private bool m_shouldGiveArenaInstruction;
  private bool m_nextTipUnclickable;
  private bool m_nextDemoTipIsNewArenaMatch;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DemoMgr demoMgr = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new Action(demoMgr.WillReset);
    string modeString = demoMgr.GetStoredGameMode() ?? Vars.Key("Demo.Mode").GetStr("NONE");
    demoMgr.SetModeFromString(modeString);
    demoMgr.WillReset();
    return false;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown() => DemoMgr.s_instance = (DemoMgr) null;

  private void WillReset()
  {
    if (this.m_mode != DemoMode.BLIZZ_MUSEUM)
      return;
    this.ApplyBlizzMuseumDemoDefaults();
  }

  public static DemoMgr Get()
  {
    if (DemoMgr.s_instance == null)
      DemoMgr.s_instance = ServiceManager.Get<DemoMgr>();
    return DemoMgr.s_instance;
  }

  private string GetStoredGameMode() => (string) null;

  public bool IsDemo() => this.m_mode != 0;

  public bool IsExpoDemo() => (uint) (this.m_mode - 1) <= 11U;

  public bool IsSocialEnabled()
  {
    switch (this.m_mode)
    {
      case DemoMode.BLIZZCON_2013:
      case DemoMode.BLIZZCON_2015:
      case DemoMode.BLIZZ_MUSEUM:
      case DemoMode.BLIZZCON_2017_ADVENTURE:
      case DemoMode.BLIZZCON_2017_BRAWL:
      case DemoMode.BLIZZCON_2018_BRAWL:
      case DemoMode.BLIZZCON_2019_BATTLEGROUNDS:
        return false;
      default:
        return true;
    }
  }

  public bool IsCurrencyEnabled()
  {
    switch (this.m_mode)
    {
      case DemoMode.BLIZZCON_2013:
      case DemoMode.BLIZZCON_2014:
      case DemoMode.BLIZZCON_2015:
      case DemoMode.ANNOUNCEMENT_5_0:
      case DemoMode.BLIZZCON_2016:
      case DemoMode.BLIZZCON_2017_ADVENTURE:
      case DemoMode.BLIZZCON_2017_BRAWL:
      case DemoMode.BLIZZCON_2018_BRAWL:
      case DemoMode.BLIZZCON_2019_BATTLEGROUNDS:
        return false;
      default:
        return true;
    }
  }

  public bool IsHubEscMenuEnabled(bool enabledInGameplay)
  {
    switch (this.m_mode)
    {
      case DemoMode.BLIZZCON_2013:
      case DemoMode.BLIZZCON_2014:
      case DemoMode.BLIZZCON_2015:
      case DemoMode.ANNOUNCEMENT_5_0:
      case DemoMode.BLIZZCON_2016:
      case DemoMode.BLIZZCON_2017_ADVENTURE:
      case DemoMode.BLIZZCON_2017_BRAWL:
      case DemoMode.BLIZZCON_2018_BRAWL:
      case DemoMode.BLIZZCON_2019_BATTLEGROUNDS:
        return enabledInGameplay;
      case DemoMode.BLIZZ_MUSEUM:
        return false;
      default:
        return true;
    }
  }

  public bool CantExitArena() => this.m_mode == DemoMode.BLIZZCON_2013;

  public bool ArenaIs1WinMode() => this.m_mode == DemoMode.BLIZZCON_2013;

  public bool CanRestartMissions() => this.m_mode != DemoMode.BLIZZCON_2017_ADVENTURE;

  public DemoMode GetMode() => this.m_mode;

  public void SetModeFromString(string modeString) => this.m_mode = this.GetModeFromString(modeString);

  public DemoMode GetModeFromString(string modeString)
  {
    try
    {
      return EnumUtils.GetEnum<DemoMode>(modeString, StringComparison.OrdinalIgnoreCase);
    }
    catch (Exception ex)
    {
      return DemoMode.NONE;
    }
  }

  public void CreateDemoText(string demoText) => this.CreateDemoText(demoText, false, false);

  public void CreateDemoText(string demoText, bool unclickable) => this.CreateDemoText(demoText, unclickable, false);

  public void CreateDemoText(string demoText, bool unclickable, bool shouldDoArenaInstruction)
  {
    if ((UnityEngine.Object) this.m_demoText != (UnityEngine.Object) null)
      return;
    this.m_shouldGiveArenaInstruction = shouldDoArenaInstruction;
    this.m_nextTipUnclickable = unclickable;
    GameObject go = AssetLoader.Get().InstantiatePrefab((AssetReference) "DemoText.prefab:5749aead2db66ce4d958e44bab4a5219");
    OverlayUI.Get().AddGameObject(go);
    this.m_demoText = go.GetComponent<Notification>();
    this.m_demoText.ChangeText(demoText);
    UniversalInputManager.Get().SetSystemDialogActive(true);
    go.transform.GetComponentInChildren<PegUIElement>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.RemoveDemoTextDialog));
    if (!this.m_nextTipUnclickable)
      return;
    this.m_nextTipUnclickable = false;
    this.MakeDemoTextClickable(false);
  }

  public void NextDemoTipIsNewArenaMatch() => this.m_nextDemoTipIsNewArenaMatch = true;

  private void RemoveDemoTextDialog(UIEvent e) => this.RemoveDemoTextDialog();

  public void RemoveDemoTextDialog()
  {
    UniversalInputManager.Get().SetSystemDialogActive(false);
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_demoText.gameObject);
    if (this.m_shouldGiveArenaInstruction)
    {
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_INST1_19"), "VO_INNKEEPER_FORGE_INST1_19.prefab:a0e06e90b545b274290dad8e442e83d0", 3f);
      this.m_shouldGiveArenaInstruction = false;
    }
    if (!this.m_nextDemoTipIsNewArenaMatch)
      return;
    this.m_nextDemoTipIsNewArenaMatch = false;
    this.CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA"), false, true);
  }

  public void MakeDemoTextClickable(bool clickable)
  {
    if (!clickable)
    {
      this.m_demoText.transform.GetComponentInChildren<BoxCollider>().enabled = false;
      this.m_demoText.transform.Find("continue").gameObject.SetActive(false);
    }
    else
    {
      this.m_demoText.transform.GetComponentInChildren<BoxCollider>().enabled = true;
      this.m_demoText.transform.Find("continue").gameObject.SetActive(true);
    }
  }

  public void ApplyBlizzMuseumDemoDefaults()
  {
    Options.Get().SetBool(Option.CONNECT_TO_AURORA, false);
    Options.Get().SetBool(Option.HAS_SEEN_NEW_CINEMATIC, true);
  }

  public IEnumerator CompleteBlizzMuseumDemo()
  {
    yield return (object) new WaitForSeconds(3f);
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_DEMO_COMPLETE_HEADER"),
      m_text = GameStrings.Get("GLOBAL_DEMO_COMPLETE_BODY"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => HearthstoneApplication.Get().Reset())
    });
  }
}
