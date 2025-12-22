using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;

public class DisconnectMgr : IService
{
  private AlertPopup m_dialog;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (GameMgr),
    typeof (Network)
  };

  public void Shutdown()
  {
    SceneMgr service;
    if (!ServiceManager.TryGet<SceneMgr>(out service))
      return;
    service.UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
  }

  public static DisconnectMgr Get() => ServiceManager.Get<DisconnectMgr>();

  public void DisconnectFromGameplay()
  {
    PerformanceAnalytics.Get()?.DisconnectEvent(SceneMgr.Get().GetMode().ToString());
    SceneMgr.Mode disconnectSceneMode = GameMgr.Get().GetPostDisconnectSceneMode();
    GameMgr.Get().PreparePostGameSceneMode(disconnectSceneMode);
    if (disconnectSceneMode == SceneMgr.Mode.INVALID)
      Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_LOST_GAME_CONNECTION");
    else if (Network.Get().WasDisconnectRequested())
      SceneMgr.Get().SetNextMode(disconnectSceneMode);
    else
      this.ShowGameplayDialog(disconnectSceneMode);
  }

  private void ShowGameplayDialog(SceneMgr.Mode nextMode) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLOBAL_ERROR_NETWORK_TITLE"),
    m_text = GameStrings.Get("GLOBAL_ERROR_NETWORK_LOST_GAME_CONNECTION"),
    m_responseDisplay = AlertPopup.ResponseDisplay.NONE,
    m_layerToUse = new GameLayer?(GameLayer.UI)
  }, new DialogManager.DialogProcessCallback(this.OnGameplayDialogProcessed), (object) nextMode);

  private bool OnGameplayDialogProcessed(DialogBase dialog, object userData)
  {
    this.m_dialog = (AlertPopup) dialog;
    SceneMgr.Mode mode = (SceneMgr.Mode) userData;
    SceneMgr.Get().SetNextMode(mode);
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    return true;
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded), userData);
    this.UpdateGameplayDialog();
  }

  private void UpdateGameplayDialog()
  {
    if (!((UnityEngine.Object) this.m_dialog != (UnityEngine.Object) null))
      return;
    AlertPopup.PopupInfo info = this.m_dialog.GetInfo();
    info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
    info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnGameplayDialogResponse);
    this.m_dialog.UpdateInfo(info);
  }

  private void OnGameplayDialogResponse(AlertPopup.Response response, object userData)
  {
    this.m_dialog = (AlertPopup) null;
    if (!Network.IsLoggedIn())
      Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_LOST_GAME_CONNECTION");
    else
      SpectatorManager.Get().LeaveSpectatorMode();
  }
}
