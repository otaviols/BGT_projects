using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System.Collections.Generic;
using UnityEngine;

public class InactivePlayerKicker : IService, IHasUpdate
{
  private bool m_checkingForInactivity;
  private bool m_shouldCheckForInactivity = true;
  private float m_kickSec = 1800f;
  private bool m_activityDetected;
  private float m_inactivityStartTimestamp;
  private GameMgr m_gameMgr;

  public bool WasKickedForInactivity { get; private set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    InactivePlayerKicker inactivePlayerKicker = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new System.Action(inactivePlayerKicker.WillReset);
    serviceLocator.Get<SceneMgr>().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(inactivePlayerKicker.OnScenePreUnload));
    serviceLocator.Get<ReconnectMgr>().OnReconnectComplete += new System.Action(inactivePlayerKicker.OnReconnect);
    inactivePlayerKicker.m_gameMgr = serviceLocator.Get<GameMgr>();
    if (HearthstoneApplication.IsInternal())
    {
      Options.Get().RegisterChangedListener(Option.IDLE_KICK_TIME, new Options.ChangedCallback(inactivePlayerKicker.OnOptionChanged));
      Options.Get().RegisterChangedListener(Option.IDLE_KICKER, new Options.ChangedCallback(inactivePlayerKicker.OnOptionChanged));
    }
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[4]
  {
    typeof (Network),
    typeof (SceneMgr),
    typeof (ReconnectMgr),
    typeof (GameMgr)
  };

  public void Shutdown()
  {
    HearthstoneApplication.Get().WillReset -= new System.Action(this.WillReset);
    SceneMgr service1;
    if (ServiceManager.TryGet<SceneMgr>(out service1))
      service1.UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    ReconnectMgr service2;
    if (ServiceManager.TryGet<ReconnectMgr>(out service2))
      service2.OnReconnectComplete -= new System.Action(this.OnReconnect);
    if (!HearthstoneApplication.IsInternal())
      return;
    Options.Get().UnregisterChangedListener(Option.IDLE_KICK_TIME, new Options.ChangedCallback(this.OnOptionChanged));
    Options.Get().UnregisterChangedListener(Option.IDLE_KICKER, new Options.ChangedCallback(this.OnOptionChanged));
  }

  public void Update()
  {
    this.CheckInactivity();
    this.CheckActivity();
  }

  private void WillReset() => this.SetShouldCheckForInactivity(true);

  public static InactivePlayerKicker Get() => ServiceManager.Get<InactivePlayerKicker>();

  public void OnLoggedIn()
  {
    this.UpdateIdleKickTimeOption();
    this.UpdateCheckForInactivity();
  }

  private void OnReconnect()
  {
    this.SetShouldCheckForInactivity(true);
    this.WasKickedForInactivity = false;
  }

  public bool IsCheckingForInactivity() => this.m_checkingForInactivity;

  public void SetShouldCheckForInactivity(bool check)
  {
    if (this.m_shouldCheckForInactivity == check)
      return;
    this.m_shouldCheckForInactivity = check;
    this.UpdateCheckForInactivity();
  }

  public void SetKickSec(float sec) => this.m_kickSec = sec;

  public bool SetKickTimeStr(string timeStr)
  {
    float sec;
    if (!TimeUtils.TryParseDevSecFromElapsedTimeString(timeStr, out sec))
      return false;
    this.SetKickSec(sec);
    return true;
  }

  private bool CanCheckForInactivity() => !DemoMgr.Get().IsExpoDemo() && this.m_shouldCheckForInactivity && (!HearthstoneApplication.IsInternal() || Options.Get().GetBool(Option.IDLE_KICKER));

  private void UpdateCheckForInactivity()
  {
    bool checkingForInactivity = this.m_checkingForInactivity;
    this.m_checkingForInactivity = this.CanCheckForInactivity();
    if (!this.m_checkingForInactivity || checkingForInactivity)
      return;
    this.StartCheckForInactivity();
  }

  private void StartCheckForInactivity()
  {
    this.m_activityDetected = false;
    this.m_inactivityStartTimestamp = Time.realtimeSinceStartup;
  }

  private void CheckActivity()
  {
    if (!this.IsCheckingForInactivity())
      return;
    if (Input.anyKey || Input.touchCount > 0)
    {
      this.m_activityDetected = true;
    }
    else
    {
      if (!this.m_gameMgr.IsSpectator())
        return;
      this.m_activityDetected = true;
    }
  }

  private void CheckInactivity()
  {
    if (!this.IsCheckingForInactivity())
      return;
    if (this.m_activityDetected)
    {
      this.m_inactivityStartTimestamp = Time.realtimeSinceStartup;
      this.m_activityDetected = false;
      ReconnectMgr.Get().ReconnectBlockedByInactivity = false;
    }
    else
    {
      if (this.WasKickedForInactivity || (double) Time.realtimeSinceStartup - (double) this.m_inactivityStartTimestamp < (double) this.m_kickSec)
        return;
      Error.AddFatal(FatalErrorReason.INACTIVITY_TIMEOUT, "GLOBAL_ERROR_INACTIVITY_KICK");
      ReconnectMgr.Get().ReconnectBlockedByInactivity = true;
      this.WasKickedForInactivity = true;
      BattleNet.RequestCloseAurora();
      DialogManager.Get().ShowReconnectHelperDialog();
    }
  }

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.FATAL_ERROR)
      return;
    this.SetShouldCheckForInactivity(false);
  }

  private void UpdateIdleKickTimeOption()
  {
    if (!HearthstoneApplication.IsInternal())
      return;
    this.SetKickTimeStr(Options.Get().GetString(Option.IDLE_KICK_TIME));
  }

  private void OnOptionChanged(Option option, object prevValue, bool existed, object userData)
  {
    if (option != Option.IDLE_KICKER)
    {
      if (option == Option.IDLE_KICK_TIME)
        this.UpdateIdleKickTimeOption();
      else
        Error.AddDevFatal("InactivePlayerKicker.OnOptionChanged() - unhandled option {0}", (object) option);
    }
    else
      this.UpdateCheckForInactivity();
  }
}
