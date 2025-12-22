using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMigrationManager : IService
{
  public bool RestartRequired { get; private set; }

  public bool IsShowingPlayerMigrationRelogPopup { get; private set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    PlayerMigrationManager migrationManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    serviceLocator.Get<Network>().RegisterNetHandler((object) PegasusUtil.GenericResponse.PacketID.ID, new Network.NetHandler(migrationManager.OnGenericResponse));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (Network)
  };

  public void Shutdown()
  {
  }

  public void ShowRestartAlert()
  {
    if (this.IsShowingPlayerMigrationRelogPopup)
      return;
    this.IsShowingPlayerMigrationRelogPopup = true;
    GameMgr service;
    if (ServiceManager.TryGet<GameMgr>(out service) && service.IsFindingGame())
      GameMgr.Get().CancelFindGame();
    Log.All.Print("Player Migration is required! Forcing the client to restart.");
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_PLAYER_MIGRATION_RESTART_HEADER"),
      m_text = GameStrings.Get("GLOBAL_PLAYER_MIGRATION_RESTART_BODY"),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = true,
      m_disableBnetBar = true,
      m_blurWhenShown = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if ((bool) HearthstoneApplication.AllowResetFromFatalError)
          HearthstoneApplication.Get().Reset();
        else
          HearthstoneApplication.Get().Exit();
      })
    });
    TelemetryManager.Client().SendRestartDueToPlayerMigration();
  }

  public static PlayerMigrationManager Get() => ServiceManager.Get<PlayerMigrationManager>();

  public bool CheckForPlayerMigrationRequired() => this.IsShowingPlayerMigrationRelogPopup || this.RestartRequired && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame();

  private void OnGenericResponse()
  {
    Network.GenericResponse genericResponse = Network.Get().GetGenericResponse();
    if (genericResponse == null)
    {
      Debug.LogError((object) string.Format("PlayerMigrationManager - GenericResponse parse error"));
    }
    else
    {
      if (Network.GenericResponse.Result.RESULT_DATA_MIGRATION_REQUIRED != genericResponse.ResultCode)
        return;
      this.RestartRequired = true;
      if (!this.CheckForPlayerMigrationRequired())
        return;
      this.ShowRestartAlert();
    }
  }
}
