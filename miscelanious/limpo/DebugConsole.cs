using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using BobNetProto;
using Hearthstone.Core;
using System.Collections.Generic;
using System.ComponentModel;

public class DebugConsole : IService
{
  private static Map<string, DebugConsole.ConsoleCallbackInfo> s_serverConsoleCallbackMap;
  private static Map<string, DebugConsole.ConsoleCallbackInfo> s_clientConsoleCallbackMap;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    Network net = serviceLocator.Get<Network>();
    if (net.ShouldBeConnectedToAurora_NONSTATIC())
    {
      Processor.QueueJob("InitializeDebugConsole", this.Job_InitializeAfterBGSInits(net), (IJobDependency[]) null);
    }
    else
    {
      this.InitializeConsole(net);
      yield break;
    }
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (Network)
  };

  public void Shutdown()
  {
  }

  private IEnumerator<IAsyncJobResult> Job_InitializeAfterBGSInits(
    Network net)
  {
    while (!BattleNet.IsInitialized())
      yield return (IAsyncJobResult) null;
    this.InitializeConsole(net);
  }

  private void InitializeConsole(Network net)
  {
    this.InitConsoleCallbackMaps();
    net.RegisterNetHandler((object) DebugConsoleCommand.PacketID.ID, new Network.NetHandler(this.OnCommandReceived));
    net.RegisterNetHandler((object) BobNetProto.DebugConsoleResponse.PacketID.ID, new Network.NetHandler(this.OnCommandResponseReceived));
  }

  private static List<DebugConsole.CommandParamDecl> CreateParamDeclList(
    params DebugConsole.CommandParamDecl[] paramDecls)
  {
    List<DebugConsole.CommandParamDecl> paramDeclList = new List<DebugConsole.CommandParamDecl>();
    foreach (DebugConsole.CommandParamDecl paramDecl in paramDecls)
      paramDeclList.Add(paramDecl);
    return paramDeclList;
  }

  private void InitConsoleCallbackMaps()
  {
    this.InitClientConsoleCallbackMap();
    this.InitServerConsoleCallbackMap();
  }

  private void InitServerConsoleCallbackMap()
  {
    if (DebugConsole.s_serverConsoleCallbackMap != null)
      return;
    DebugConsole.s_serverConsoleCallbackMap = new Map<string, DebugConsole.ConsoleCallbackInfo>();
    DebugConsole.s_serverConsoleCallbackMap.Add("spawncard", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.STR, "cardGUID"), new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"), new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.STR, "zoneName"), new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "premium"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("loadcard", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.STR, "cardGUID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("drawcard", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("shuffle", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("cyclehand", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("nuke", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("damage", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID"), new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "damage"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("addmana", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("readymana", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("maxmana", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("nocosts", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList()));
    DebugConsole.s_serverConsoleCallbackMap.Add("healhero", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("healentity", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("ready", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("exhaust", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("freeze", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("move", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID"), new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "zoneID"))));
    DebugConsole.s_serverConsoleCallbackMap.Add("tiegame", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList()));
    DebugConsole.s_serverConsoleCallbackMap.Add("aiplaylastspawnedcard", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList()));
    DebugConsole.s_serverConsoleCallbackMap.Add("forcestallingprevention", new DebugConsole.ConsoleCallbackInfo(true, (DebugConsole.ConsoleCallback) null, DebugConsole.CreateParamDeclList()));
  }

  private void InitClientConsoleCallbackMap()
  {
    if (DebugConsole.s_clientConsoleCallbackMap != null)
      return;
    DebugConsole.s_clientConsoleCallbackMap = new Map<string, DebugConsole.ConsoleCallbackInfo>();
  }

  private void SendDebugConsoleResponse(DebugConsole.DebugConsoleResponseType type, string message) => Network.Get().SendDebugConsoleResponse((int) type, message);

  private void SendConsoleCmdToServer(string commandName, List<string> commandParams)
  {
    if (!DebugConsole.s_serverConsoleCallbackMap.ContainsKey(commandName))
      return;
    string command = commandName;
    foreach (string commandParam in commandParams)
      command = command + " " + commandParam;
    if (Network.Get().SendDebugConsoleCommand(command))
      return;
    this.SendDebugConsoleResponse(DebugConsole.DebugConsoleResponseType.CONSOLE_OUTPUT, string.Format("Cannot send command '{0}'; not currently connected to a game server.", (object) commandName));
  }

  private void OnCommandReceived()
  {
    string[] strArray = Network.Get().GetDebugConsoleCommand().Split(' ');
    if (strArray.Length == 0)
    {
      Log.All.Print("Received empty command from debug console!");
    }
    else
    {
      string str = strArray[0];
      List<string> commandParams = new List<string>();
      for (int index = 1; index < strArray.Length; ++index)
        commandParams.Add(strArray[index]);
      if (DebugConsole.s_serverConsoleCallbackMap.ContainsKey(str))
        this.SendConsoleCmdToServer(str, commandParams);
      else if (!DebugConsole.s_clientConsoleCallbackMap.ContainsKey(str))
      {
        this.SendDebugConsoleResponse(DebugConsole.DebugConsoleResponseType.CONSOLE_OUTPUT, string.Format("Unknown command '{0}'.", (object) str));
      }
      else
      {
        DebugConsole.ConsoleCallbackInfo clientConsoleCallback = DebugConsole.s_clientConsoleCallbackMap[str];
        if (clientConsoleCallback.GetNumParams() != commandParams.Count)
        {
          this.SendDebugConsoleResponse(DebugConsole.DebugConsoleResponseType.CONSOLE_OUTPUT, string.Format("Invalid params for command '{0}'.", (object) str));
        }
        else
        {
          Log.All.Print(string.Format("Processing command '{0}' from debug console.", (object) str));
          clientConsoleCallback.Callback(commandParams);
        }
      }
    }
  }

  private void OnCommandResponseReceived()
  {
    Network.DebugConsoleResponse debugConsoleResponse = Network.Get().GetDebugConsoleResponse();
    if (debugConsoleResponse != null)
      this.SendDebugConsoleResponse((DebugConsole.DebugConsoleResponseType) debugConsoleResponse.Type, debugConsoleResponse.Response);
    Log.All.Print("DebugConsoleResponse: {0}", string.IsNullOrEmpty(debugConsoleResponse.Response) ? (object) "<empty>" : (object) debugConsoleResponse.Response);
    if (string.IsNullOrEmpty(debugConsoleResponse.Response))
      return;
    UIStatus.Get().AddInfo(debugConsoleResponse.Response);
  }

  private class CommandParamDecl
  {
    public string Name;
    public DebugConsole.CommandParamDecl.ParamType Type;

    public CommandParamDecl(DebugConsole.CommandParamDecl.ParamType type, string name)
    {
      this.Type = type;
      this.Name = name;
    }

    public enum ParamType
    {
      [Description("string")] STR,
      [Description("int32")] I32,
      [Description("float32")] F32,
      [Description("bool")] BOOL,
    }
  }

  private delegate void ConsoleCallback(List<string> commandParams);

  private class ConsoleCallbackInfo
  {
    public bool DisplayInCommandList;
    public List<DebugConsole.CommandParamDecl> ParamList;
    public DebugConsole.ConsoleCallback Callback;

    public ConsoleCallbackInfo(
      bool displayInCmdList,
      DebugConsole.ConsoleCallback callback,
      DebugConsole.CommandParamDecl[] commandParams)
    {
      this.DisplayInCommandList = displayInCmdList;
      this.ParamList = new List<DebugConsole.CommandParamDecl>((IEnumerable<DebugConsole.CommandParamDecl>) commandParams);
      this.Callback = callback;
    }

    public ConsoleCallbackInfo(
      bool displayInCmdList,
      DebugConsole.ConsoleCallback callback,
      List<DebugConsole.CommandParamDecl> commandParams)
      : this(displayInCmdList, callback, commandParams.ToArray())
    {
    }

    public int GetNumParams() => this.ParamList.Count;
  }

  private enum DebugConsoleResponseType
  {
    CONSOLE_OUTPUT,
    LOG_MESSAGE,
  }
}
