using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Login;
using Hearthstone.Progression;
using HutongGames.PlayMaker;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Login : PegasusScene
{
  private int m_nextMissionId;
  private ExistingAccountPopup m_existingAccountPopup;
  private static global::Login s_instance;
  private bool m_blockingBnetBar;

  protected override void Awake()
  {
    global::Login.s_instance = this;
    base.Awake();
    if (LoginManager.Get() == null || !((UnityEngine.Object) SplashScreen.Get() != (UnityEngine.Object) null))
      return;
    Processor.QueueJob("Login.GoToNextMode", this.GoToNextMode(), (IJobDependency) LoginManager.Get().ReadyToGoToNextModeDependency);
    JobDefinition jobDefinition = Processor.QueueJob("Splashscreen.ShowLoginQueue", SplashScreen.Get().Job_ShowLoginQueue());
    Processor.QueueJob("Login.OnLoginStateResolved", this.OnLoginStateResolved(), (IJobDependency) LoginManager.Get().ReadyToReconnectOrChangeModeDependency, (IJobDependency) jobDefinition.CreateDependency());
  }

  private void Start() => SceneMgr.Get().NotifySceneLoaded();

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) global::Login.s_instance == (UnityEngine.Object) this))
      return;
    global::Login.s_instance = (global::Login) null;
  }

  private void Update()
  {
    if (Network.Get() == null)
      return;
    Network.Get().ProcessNetwork();
  }

  public static global::Login Get() => global::Login.s_instance;

  public override void Unload()
  {
    GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.SetBlockingBnetBar(false);
  }

  private IEnumerator<IAsyncJobResult> OnLoginStateResolved()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    global::Login login = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (!Network.ShouldBeConnectedToAurora())
    {
      bool useCNStyle = PlatformSettings.LocaleVariant == LocaleVariant.China;
      DialogManager.Get().ShowExistingAccountPopup(new ExistingAccountPopup.ResponseCallback(login.OnExistingAccountPopupResponse), new DialogManager.DialogProcessCallback(login.OnExistingAccountLoadedCallback), useCNStyle);
    }
    else
    {
      JobDefinition sceneTransitionJob = new JobDefinition("Login.ReconnectOrChangeMode", login.ReconnectOrChangeMode(), Array.Empty<IJobDependency>());
      Processor.QueueJob("SplashScreen.Hide", SplashScreen.Get().Hide(sceneTransitionJob));
    }
    return false;
  }

  private IEnumerator<IAsyncJobResult> ReconnectOrChangeMode()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    global::Login login = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.SendStartupTimeTelemetry("Login.ReconnectOrChangeMode");
    if ((UnityEngine.Object) BaseUI.Get() != (UnityEngine.Object) null)
      BaseUI.Get().OnLoggedIn();
    if (!Cheats.Get().IsLaunchingQuickGame() && LoginManager.Get().AttemptToReconnectToGame(new ReconnectMgr.TimeoutCallback(login.OnReconnectTimeout)))
      return false;
    login.ChangeMode();
    return false;
  }

  private void ChangeMode()
  {
    if (RewardTrackManager.Get().HasReceivedRewardTracksFromServer)
      Box.Get().PlayBoxMusic();
    else
      Box.Get().OnBoxDressingReadyOnce += new Action(Box.Get().PlayBoxMusic);
    this.m_nextMissionId = GameUtils.GetNextTutorial();
    if (this.m_nextMissionId > 3)
      this.ChangeMode_Tutorial();
    else if (SetRotationManager.Get().ShouldShowSetRotationIntro())
    {
      if (CreateSkipHelper.ShouldShowCreateSkip() && CreateSkipHelper.ShowCreateSkipDialog(new Action(this.ChangeToAppropriateHubMode)))
        return;
      this.ChangeToAppropriateHubMode();
      this.ChangeMode_SetRotation();
    }
    else
    {
      if (NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>() == null)
        Debug.LogError((object) " Could not get NetCacheFeatures Object");
      this.ChangeMode_Hub();
    }
  }

  private void ChangeToAppropriateHubMode()
  {
    Log.Login.PrintInfo("Changing mode");
    if (SetRotationManager.Get().ShouldShowSetRotationIntro())
      this.ChangeMode_SetRotation();
    else
      this.ChangeMode_Hub();
  }

  private bool OnReconnectTimeout(object userData)
  {
    this.ChangeMode();
    return true;
  }

  private void ChangeMode_Hub()
  {
    this.SetBlockingBnetBar(true);
    ServiceManager.Get<LoginManager>().OnFullLoginFlowComplete += (Action) (() => this.SetBlockingBnetBar(false));
    if (Options.Get().GetBool(Option.HAS_SEEN_HUB, false))
      this.PlayInnkeeperIntroVO();
    Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.STARTUP_HUB);
    eventSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnStartupHubSpellFinished));
    eventSpell.Activate();
  }

  private void SetBlockingBnetBar(bool blocked)
  {
    if (blocked == this.m_blockingBnetBar)
      return;
    this.m_blockingBnetBar = blocked;
    if (blocked)
      BaseUI.Get().m_BnetBar.RequestDisableButtons();
    else
      BaseUI.Get().m_BnetBar.CancelRequestToDisableButtons();
  }

  private void PlayInnkeeperIntroVO()
  {
    if (ReturningPlayerMgr.Get().PlayReturningPlayerInnkeeperGreetingIfNecessary())
      return;
    if (RewardTrackManager.Get().HasReceivedRewardTracksFromServer)
      Box.Get().PlayInnkeeperGreetings();
    else
      Box.Get().OnBoxDressingReadyOnce += new Action(Box.Get().PlayInnkeeperGreetings);
  }

  private IEnumerator<IAsyncJobResult> GoToNextMode()
  {
    if (this.m_nextMissionId == 0 || this.m_nextMissionId <= 3)
    {
      SceneMgr.Mode nextScene = SceneMgr.Mode.INVALID;
      if (!this.DeterminePostLoginScene(ref nextScene))
      {
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        yield break;
      }
    }
  }

  private bool DeterminePostLoginScene(ref SceneMgr.Mode nextScene)
  {
    foreach (KeyValuePair<StartupSceneSource, DetermineStartupSceneCallback> keyValuePair in new SortedList<StartupSceneSource, DetermineStartupSceneCallback>((IDictionary<StartupSceneSource, DetermineStartupSceneCallback>) LoginManager.GetPostLoginCallbacks()))
    {
      if (keyValuePair.Key != StartupSceneSource.DEFAULT_NORMAL_STARTUP)
      {
        DetermineStartupSceneCallback startupSceneCallback = keyValuePair.Value;
        nextScene = SceneMgr.Mode.INVALID;
        ref SceneMgr.Mode local = ref nextScene;
        if (startupSceneCallback(ref local))
          return true;
      }
      else
        break;
    }
    return false;
  }

  private void ChangeMode_Tutorial()
  {
    if (this.m_nextMissionId == 3)
      this.StartTutorial();
    else
      this.ShowTutorialProgressScreen();
  }

  private void ChangeMode_TutorialWithStart()
  {
    Box.Get().ChangeLightState(BoxLightStateType.TUTORIAL);
    Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.STARTUP_TUTORIAL);
    eventSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnStartupTutorialSpellFinished));
    eventSpell.Activate();
  }

  private void OnStartupTutorialSpellFinished(Spell spell, object userData)
  {
    Box.Get().AddButtonPressListener(new Box.ButtonPressCallback(this.OnStartButtonPressed));
    Box.Get().ChangeState(Box.State.PRESS_START);
  }

  private void OnStartButtonPressed(
    Box.ButtonType buttonType,
    bool isShowingTutorialPreview,
    object userData)
  {
    if (buttonType != Box.ButtonType.START)
      return;
    TelemetryManager.Client().SendButtonPressed("PressToStart");
    if (this.m_nextMissionId == 3)
      AdTrackingManager.Get().TrackTutorialProgress(TutorialProgress.NOTHING_COMPLETE);
    Box.Get().RemoveButtonPressListener(new Box.ButtonPressCallback(this.OnStartButtonPressed));
    this.ChangeMode_Tutorial();
  }

  private void ShowTutorialProgressScreen()
  {
    Box.Get().m_StartButton.ChangeState(BoxStartButton.State.HIDDEN);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "TutorialProgressScreen.prefab:a78bac9caa971494ea8fac23dc1a9bd8", new PrefabCallback<GameObject>(this.OnTutorialProgressScreenCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnTutorialProgressScreenCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    TutorialProgressScreen component = go.GetComponent<TutorialProgressScreen>();
    component.SetCoinPressCallback(new HeroCoin.CoinPressCallback(this.StartTutorial));
    component.StartTutorialProgress();
  }

  private void OnExistingAccountPopupResponse(bool hasAccount)
  {
    this.m_existingAccountPopup.gameObject.SetActive(false);
    HearthstoneApplication.Get().ResetAndForceLogin(!hasAccount);
  }

  private void StartTutorial()
  {
    MusicManager.Get().StopPlaylist();
    Box.Get().ChangeState(Box.State.CLOSED);
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    GameMgr.Get().FindGame(GameType.GT_TUTORIAL, FormatType.FT_WILD, this.m_nextMissionId);
  }

  private bool OnExistingAccountLoadedCallback(DialogBase dialog, object userData)
  {
    this.m_existingAccountPopup = (ExistingAccountPopup) dialog;
    this.m_existingAccountPopup.gameObject.SetActive(true);
    return true;
  }

  private void ChangeMode_SetRotation()
  {
    UserAttentionManager.StartBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
    Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.STARTUP_SET_ROTATION);
    Box.Get().m_StoreButton.gameObject.SetActive(false);
    Box.Get().m_QuestLogButton.gameObject.SetActive(false);
    if (PlatformSettings.IsMobile())
    {
      PlayMakerFSM component = eventSpell.gameObject.GetComponent<PlayMakerFSM>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Missing FSM on Startup_Hub");
      }
      else
      {
        FsmFloat fsmFloat1 = component.FsmVariables.GetFsmFloat("PanDuration");
        FsmFloat fsmFloat2 = component.FsmVariables.GetFsmFloat("PanStartTime");
        fsmFloat1.Value = 3f;
        fsmFloat2.Value = 2f;
      }
    }
    eventSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSetRotationSpellFinished));
    eventSpell.Activate();
  }

  private void OnSetRotationSpellFinished(Spell spell, object userData) => Processor.QueueJob("Login.GoToNextMode", this.GoToNextMode());

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    if (eventData.m_state != FindGameState.SERVER_GAME_STARTED || GameMgr.Get().IsNextReconnect())
      return false;
    Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.TUTORIAL_PLAY);
    eventSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnTutorialPlaySpellStateBirthFinished));
    eventSpell.ActivateState(SpellStateType.BIRTH);
    return true;
  }

  private void OnTutorialPlaySpellStateBirthFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    SpellStateType activeState = spell.GetActiveState();
    if (prevStateType == SpellStateType.BIRTH)
    {
      LoadingScreen.Get().SetFadeColor(Color.white);
      LoadingScreen.Get().EnableFadeOut(false);
      LoadingScreen.Get().AddTransitionObject(Box.Get().gameObject);
      LoadingScreen.Get().AddTransitionBlocker();
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnMissionSceneLoaded));
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAMEPLAY);
    }
    else
    {
      if (activeState != SpellStateType.NONE)
        return;
      LoadingScreen.Get().NotifyTransitionBlockerComplete();
    }
  }

  private void OnMissionSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnMissionSceneLoaded));
    Box.Get().GetEventSpell(BoxEventType.TUTORIAL_PLAY).ActivateState(SpellStateType.ACTION);
  }

  private void OnStartupHubSpellFinished(Spell spell, object userData)
  {
    HearthstoneApplication.SendStartupTimeTelemetry("Login.OnStartupHubSpellFinished");
    if (!Network.ShouldBeConnectedToAurora() || this.m_nextMissionId > 3)
      return;
    IJobDependency[] jobDependencyArray = new IJobDependency[1]
    {
      (IJobDependency) Processor.QueueJob("LoginManager.ShowIntroPopups", LoginManager.Get().ShowIntroPopups()).CreateDependency()
    };
    Processor.QueueJob("LoginManager.CompleteLoginFlow", LoginManager.Get().CompleteLoginFlow(), jobDependencyArray);
  }
}
