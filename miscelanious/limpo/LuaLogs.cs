using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System;
using System.Collections.Generic;

public class LuaLogs : IService
{
  private LuaLogs.ListenAction m_currentAction;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LuaLogs luaLogs = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Network network = serviceLocator.Get<Network>();
    luaLogs.m_currentAction = LuaLogs.ListenAction.CreateEmpty();
    // ISSUE: variable of a boxed type
    __Boxed<PegasusGame.GameSetup.PacketID> enumId = (Enum) PegasusGame.GameSetup.PacketID.ID;
    Network.NetHandler handler = new Network.NetHandler(luaLogs.OnGameSetup);
    network.RegisterNetHandler((object) enumId, handler);
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (Network),
    typeof (CheatMgr)
  };

  public void Shutdown()
  {
    Network service;
    if (!ServiceManager.TryGet<Network>(out service))
      return;
    service.RemoveNetHandler((object) PegasusGame.GameSetup.PacketID.ID, new Network.NetHandler(this.OnGameSetup));
  }

  private void OnGameSetup()
  {
    if (this.m_currentAction.Action == LuaLogs.ListenActionType.LISTEN)
    {
      this.EnableListeningOnGameServer();
    }
    else
    {
      if (this.m_currentAction.Action != LuaLogs.ListenActionType.CLEAR)
        return;
      this.ClearListenOnGameServer();
    }
  }

  private void ClearListenOnGameServer()
  {
    Network network = ServiceManager.Get<Network>();
    if (network == null || !network.IsConnectedToGameServer())
      return;
    CheatMgr.Get()?.RunCheatInternally(string.Format("cheat luaclearlisten {0}", (object) this.m_currentAction.PlayerId));
  }

  public void ClearListenOnGameServer(int playerId)
  {
    this.m_currentAction.SetAsClearListen(playerId);
    this.ClearListenOnGameServer();
  }

  public void ListenOnGameServer(
    int playerId,
    int scriptId,
    LuaLogs.ListenableScriptType scriptType)
  {
    this.m_currentAction.SetAsListen(playerId, scriptId, scriptType);
    this.EnableListeningOnGameServer();
  }

  private void EnableListeningOnGameServer()
  {
    Network network = ServiceManager.Get<Network>();
    if (network == null || !network.IsConnectedToGameServer())
      return;
    CheatMgr cheatMgr = CheatMgr.Get();
    if (cheatMgr == null)
      return;
    string inputCommand = string.Format("cheat lua{0}listen {1} {2}", (object) EnumUtils.GetString<LuaLogs.ListenableScriptType>(this.m_currentAction.ScriptType).ToLower(), (object) this.m_currentAction.ScriptId, (object) this.m_currentAction.PlayerId);
    cheatMgr.RunCheatInternally(inputCommand);
  }

  public enum ListenableScriptType
  {
    INVALID,
    QUEST,
    ACHIEVE,
    TASK,
  }

  private enum ListenActionType
  {
    IDLE,
    LISTEN,
    CLEAR,
  }

  private struct ListenAction
  {
    public LuaLogs.ListenActionType Action;
    public int PlayerId;
    public int ScriptId;
    public LuaLogs.ListenableScriptType ScriptType;

    public static LuaLogs.ListenAction CreateEmpty() => new LuaLogs.ListenAction(LuaLogs.ListenActionType.IDLE, 0, 0, LuaLogs.ListenableScriptType.INVALID);

    public ListenAction(
      LuaLogs.ListenActionType action,
      int playerId,
      int scriptId,
      LuaLogs.ListenableScriptType scriptType)
    {
      this.Action = action;
      this.PlayerId = playerId;
      this.ScriptId = scriptId;
      this.ScriptType = scriptType;
    }

    public void SetAsListen(int playerId, int scriptId, LuaLogs.ListenableScriptType type)
    {
      this.Action = LuaLogs.ListenActionType.LISTEN;
      this.PlayerId = playerId;
      this.ScriptId = scriptId;
      this.ScriptType = type;
    }

    public void SetAsClearListen(int playerId)
    {
      this.Action = LuaLogs.ListenActionType.LISTEN;
      this.PlayerId = playerId;
      this.ScriptId = 0;
      this.ScriptType = LuaLogs.ListenableScriptType.INVALID;
    }
  }
}
