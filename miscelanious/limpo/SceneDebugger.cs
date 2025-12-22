using Assets;
using Blizzard.T5.Core.Time;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Progression;
using Hearthstone.Streaming;
using PegasusGame;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class SceneDebugger : IService, IHasUpdate
{
  private readonly Vector2 m_GUISize;
  private float m_UpdateInterval;
  private double m_LastInterval;
  private int m_frames;
  private string m_fpsText;
  private static readonly Blizzard.T5.Logging.LogLevel[] LOG_LEVELS_TO_DISPLAY = new Blizzard.T5.Logging.LogLevel[3]
  {
    Blizzard.T5.Logging.LogLevel.Info,
    Blizzard.T5.Logging.LogLevel.Warning,
    Blizzard.T5.Logging.LogLevel.Error
  };
  private bool m_enableSceneDebugger;
  private bool m_testMessaging;
  private DebuggerGuiWindow m_guiWindow;
  private DebuggerGuiWindow m_ratingWindow;
  private DebuggerGuiWindow m_assetsWindow;
  private DebuggerGuiWindow m_gameplayWindow;
  private DebuggerGuiWindow m_presenceWindow;
  private DebuggerGuiWindow m_questWindow;
  private DebuggerGuiWindow m_achievementWindow;
  private DebuggerGuiWindow m_rewardTrackWindow;
  private LoggerDebugWindow m_messageWindow;
  private LoggerDebugWindow m_serverLogWindow;
  private CheatsDebugWindow m_cheatsWindow;
  private LoggerDebugWindow m_slushTrackerWindow;
  private DebuggerGuiWindow m_notepadWindow;
  private string m_notepadContents;
  private bool m_notepadFirstRun;
  private Vector2 scrollViewVector;
  private DebuggerGui m_timeSection;
  private DebuggerGui m_qualitySection;
  private DebuggerGui m_statsSection;
  private bool m_showGuiCustomization;
  private int m_guiSaveTimer;
  private List<DebuggerGui> m_debuggerGui;
  private long? m_playerId;
  private MedalInfoData m_debugMedalInfo;
  private float m_lastMedalInfoRequestTime;
  private IGraphicsManager m_graphicsManager;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    SceneDebugger sceneDebugger = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    sceneDebugger.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    TimeScaleMgr.Get().SetTimeScaleMultiplier(SceneDebugger.GetDevTimescaleMultiplier());
    Vector2 scaledScreen = sceneDebugger.GetScaledScreen();
    sceneDebugger.m_guiWindow = new DebuggerGuiWindow("Scene Debugger", new DebuggerGui.LayoutGui(sceneDebugger.LayoutGuiControls), false, false);
    sceneDebugger.m_guiWindow.Position = new Vector2(scaledScreen.x * 0.05f, scaledScreen.y * 0.125f);
    sceneDebugger.m_timeSection = new DebuggerGui("Time Scale", new DebuggerGui.LayoutGui(sceneDebugger.LayoutTimeControls));
    sceneDebugger.m_qualitySection = new DebuggerGui("Quality", new DebuggerGui.LayoutGui(sceneDebugger.LayoutQualityControls));
    sceneDebugger.m_statsSection = new DebuggerGui("Stats", new DebuggerGui.LayoutGui(sceneDebugger.LayoutStats));
    sceneDebugger.m_cheatsWindow = new CheatsDebugWindow(sceneDebugger.m_GUISize);
    sceneDebugger.m_cheatsWindow.Position = new Vector2(scaledScreen.x * 0.5f, 0.0f);
    sceneDebugger.m_cheatsWindow.ResizeToFit(scaledScreen.x * 0.5f, scaledScreen.y * 0.5f);
    sceneDebugger.m_cheatsWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_messageWindow = new LoggerDebugWindow("Messages", sceneDebugger.m_GUISize, Enum.GetValues(typeof (Blizzard.T5.Logging.LogLevel)).Cast<object>());
    sceneDebugger.m_messageWindow.CustomLayout = new DebuggerGui.LayoutGui(sceneDebugger.LayoutMessages);
    sceneDebugger.m_messageWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_messageWindow.Position = new Vector2(0.0f, (float) (0.649999976158142 * (double) scaledScreen.y - 35.0));
    sceneDebugger.m_messageWindow.ResizeToFit(scaledScreen.x, scaledScreen.y * 0.35f);
    sceneDebugger.m_serverLogWindow = new LoggerDebugWindow("Server Script Log", sceneDebugger.m_GUISize, Enum.GetValues(typeof (ServerLogs.ServerLogLevel)).Cast<object>());
    sceneDebugger.m_serverLogWindow.CustomLayout = new DebuggerGui.LayoutGui(sceneDebugger.LayoutScriptWarnings);
    sceneDebugger.m_serverLogWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_serverLogWindow.Position = new Vector2(0.0f, (float) (0.649999976158142 * (double) scaledScreen.y - 35.0));
    sceneDebugger.m_serverLogWindow.ResizeToFit(scaledScreen.x, scaledScreen.y * 0.35f);
    sceneDebugger.m_ratingWindow = new DebuggerGuiWindow("Rating", new DebuggerGui.LayoutGui(sceneDebugger.LayoutRatingDebug), canResize: false);
    sceneDebugger.m_ratingWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_ratingWindow.Position = new Vector2(scaledScreen.x - sceneDebugger.m_GUISize.x, 0.5f * scaledScreen.y);
    sceneDebugger.m_ratingWindow.ResizeToFit(sceneDebugger.m_GUISize.x, sceneDebugger.m_GUISize.y);
    serviceLocator.Get<Network>().RegisterNetHandler((object) DebugRatingInfoResponse.PacketID.ID, new Network.NetHandler(sceneDebugger.OnDebugRatingInfoResponse));
    sceneDebugger.m_assetsWindow = new DebuggerGuiWindow("Assets", new DebuggerGui.LayoutGui(sceneDebugger.LayoutAssetsDebug));
    sceneDebugger.m_assetsWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_assetsWindow.Position = new Vector2(scaledScreen.x - sceneDebugger.m_GUISize.x, 0.5f * scaledScreen.y);
    sceneDebugger.m_assetsWindow.ResizeToFit(sceneDebugger.m_GUISize.x, sceneDebugger.m_GUISize.y);
    sceneDebugger.m_gameplayWindow = new DebuggerGuiWindow("Gameplay", new DebuggerGui.LayoutGui(sceneDebugger.LayoutGameplayDebug));
    sceneDebugger.m_gameplayWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_gameplayWindow.Position = new Vector2(scaledScreen.x - sceneDebugger.m_GUISize.x, 0.5f * scaledScreen.y);
    sceneDebugger.m_gameplayWindow.ResizeToFit(sceneDebugger.m_GUISize.x, sceneDebugger.m_GUISize.y);
    sceneDebugger.m_presenceWindow = new DebuggerGuiWindow("Presence", new DebuggerGui.LayoutGui(sceneDebugger.LayoutPresenceDebug));
    sceneDebugger.m_presenceWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_presenceWindow.Position = new Vector2(scaledScreen.x - sceneDebugger.m_GUISize.x, 0.5f * scaledScreen.y);
    sceneDebugger.m_presenceWindow.ResizeToFit(sceneDebugger.m_GUISize.x, sceneDebugger.m_GUISize.y);
    float width1 = sceneDebugger.m_GUISize.x * 2f;
    sceneDebugger.m_questWindow = new DebuggerGuiWindow("Quest", new DebuggerGui.LayoutGui(sceneDebugger.LayoutQuestDebug), canResize: false);
    sceneDebugger.m_questWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_questWindow.Position = new Vector2(scaledScreen.x - width1, 0.5f * scaledScreen.y);
    sceneDebugger.m_questWindow.ResizeToFit(width1, sceneDebugger.m_GUISize.y);
    float width2 = sceneDebugger.m_GUISize.x * 2.5f;
    sceneDebugger.m_achievementWindow = new DebuggerGuiWindow("Achievement", new DebuggerGui.LayoutGui(sceneDebugger.LayoutAchievementDebug), canResize: false);
    sceneDebugger.m_achievementWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_achievementWindow.Position = new Vector2(scaledScreen.x - width2, 0.5f * scaledScreen.y);
    sceneDebugger.m_achievementWindow.ResizeToFit(width2, sceneDebugger.m_GUISize.y);
    float width3 = sceneDebugger.m_GUISize.x * 2f;
    sceneDebugger.m_rewardTrackWindow = new DebuggerGuiWindow("Reward Track", new DebuggerGui.LayoutGui(sceneDebugger.LayoutRewardTrackDebug), canResize: false);
    sceneDebugger.m_rewardTrackWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_rewardTrackWindow.Position = new Vector2(scaledScreen.x - width3, 0.5f * scaledScreen.y);
    sceneDebugger.m_rewardTrackWindow.ResizeToFit(width3, sceneDebugger.m_GUISize.y);
    sceneDebugger.m_slushTrackerWindow = new LoggerDebugWindow("Slush Time Log", sceneDebugger.m_GUISize, Enum.GetValues(typeof (Blizzard.T5.Logging.LogLevel)).Cast<object>());
    sceneDebugger.m_slushTrackerWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_slushTrackerWindow.Position = new Vector2(0.0f, (float) (0.649999976158142 * (double) scaledScreen.y - 35.0));
    sceneDebugger.m_slushTrackerWindow.ResizeToFit(scaledScreen.x, scaledScreen.y * 0.35f);
    float width4 = sceneDebugger.m_GUISize.x * 2f;
    sceneDebugger.m_notepadWindow = new DebuggerGuiWindow("Notepad", new DebuggerGui.LayoutGui(sceneDebugger.LayoutNotepadDebug));
    sceneDebugger.m_notepadWindow.collapsedWidth = new float?(sceneDebugger.m_GUISize.x);
    sceneDebugger.m_notepadWindow.Position = new Vector2(scaledScreen.x - width4, 0.5f * scaledScreen.y);
    sceneDebugger.m_notepadWindow.ResizeToFit(width4, sceneDebugger.m_GUISize.y);
    sceneDebugger.m_debuggerGui = new List<DebuggerGui>();
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_guiWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_cheatsWindow);
    if (sceneDebugger.m_messageWindow != null)
      sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_messageWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_serverLogWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_ratingWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_assetsWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_questWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_achievementWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_rewardTrackWindow);
    sceneDebugger.m_debuggerGui.Add(sceneDebugger.m_timeSection);
    sceneDebugger.m_debuggerGui.Add(sceneDebugger.m_qualitySection);
    sceneDebugger.m_debuggerGui.Add(sceneDebugger.m_statsSection);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_slushTrackerWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_notepadWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_gameplayWindow);
    sceneDebugger.m_debuggerGui.Add((DebuggerGui) sceneDebugger.m_presenceWindow);
    foreach (DebuggerGui debuggerGui in sceneDebugger.m_debuggerGui)
      debuggerGui.OnChanged += new Action(sceneDebugger.HandleGuiChanged);
    sceneDebugger.m_guiWindow.IsShown = true;
    sceneDebugger.m_cheatsWindow.IsShown = false;
    if (sceneDebugger.m_messageWindow != null)
      sceneDebugger.m_messageWindow.IsShown = false;
    sceneDebugger.m_serverLogWindow.IsShown = false;
    sceneDebugger.m_ratingWindow.IsShown = false;
    sceneDebugger.m_assetsWindow.IsShown = false;
    sceneDebugger.m_gameplayWindow.IsShown = false;
    sceneDebugger.m_presenceWindow.IsShown = false;
    sceneDebugger.m_questWindow.IsShown = false;
    sceneDebugger.m_achievementWindow.IsShown = false;
    sceneDebugger.m_rewardTrackWindow.IsShown = false;
    sceneDebugger.m_slushTrackerWindow.IsShown = false;
    sceneDebugger.m_notepadWindow.IsShown = false;
    DebuggerGui.LoadConfig(sceneDebugger.m_debuggerGui);
    OnGUIDelegateComponent.CreateGUIDelegate(new Action(sceneDebugger.OnGUI));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (IGraphicsManager),
    typeof (Network)
  };

  public void Shutdown()
  {
  }

  public bool IsMouseOverGui()
  {
    if (!Options.Get().GetBool(Option.HUD))
      return false;
    foreach (DebuggerGui debuggerGui in this.m_debuggerGui)
    {
      if (debuggerGui is DebuggerGuiWindow debuggerGuiWindow && debuggerGui.IsShown && debuggerGuiWindow.IsMouseOver())
        return true;
    }
    return false;
  }

  public void Update()
  {
    ++this.m_frames;
    float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
    if ((double) realtimeSinceStartup > this.m_LastInterval + (double) this.m_UpdateInterval)
    {
      this.m_fpsText = string.Format("{0}  FPS: {1:f2}\nScreen: {2} x {3}\n", (object) SystemInfo.graphicsDeviceType, (object) ((float) this.m_frames / (realtimeSinceStartup - (float) this.m_LastInterval)), (object) Screen.width, (object) Screen.height);
      this.m_frames = 0;
      this.m_LastInterval = (double) UnityEngine.Time.realtimeSinceStartup;
    }
    if (!this.m_testMessaging)
      return;
    string str = "abcdefghijklmnopqrstuvwxyz0123456789";
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < 5000; ++index)
    {
      char ch = str[UnityEngine.Random.Range(0, str.Length)];
      stringBuilder.Append(ch);
    }
    this.AddErrorMessage(stringBuilder.ToString());
  }

  private void OnGUI()
  {
    if (ScriptDebugDisplay.Get().m_isDisplayed || !this.m_enableSceneDebugger || !Options.Get().GetBool(Option.HUD))
      return;
    double guiScaling = (double) this.GetGuiScaling();
    GUI.matrix = Matrix4x4.Scale(new Vector3((float) guiScaling, (float) guiScaling, (float) guiScaling));
    if (GameState.Get() != null && (double) GameState.Get().GetSlushTimeTracker().GetAccruedLostTimeInSeconds() > (double) GameplayDebug.LOST_SLUSH_TIME_ERROR_THRESHOLD_SECONDS)
      this.m_gameplayWindow.IsShown = true;
    this.m_guiWindow.Layout();
    this.m_cheatsWindow.Layout();
    if (this.m_messageWindow != null)
      this.m_messageWindow.Layout();
    this.m_serverLogWindow.Layout();
    this.m_ratingWindow.Layout();
    this.m_assetsWindow.Layout();
    this.m_gameplayWindow.Layout();
    this.m_presenceWindow.Layout();
    this.m_questWindow.Layout();
    this.m_achievementWindow.Layout();
    this.m_rewardTrackWindow.Layout();
    this.m_slushTrackerWindow.Layout();
    this.m_notepadWindow.Layout();
    this.LayoutCursorDebug();
    if (this.m_guiSaveTimer <= 0)
      return;
    --this.m_guiSaveTimer;
    if (this.m_guiSaveTimer != 0)
      return;
    DebuggerGui.SaveConfig(this.m_debuggerGui);
  }

  private float GetGuiScaling()
  {
    float val = 1f;
    GeneralUtils.TryParseFloat(Options.Get().GetOption(Option.HUD_SCALE).ToString(), out val);
    float num = (float) Screen.height;
    switch (PlatformSettings.Screen)
    {
      case ScreenCategory.Phone:
        num = 480f;
        break;
      case ScreenCategory.MiniTablet:
        num = 576f;
        break;
      case ScreenCategory.Tablet:
        num = 640f;
        break;
    }
    return Mathf.Max(0.1f, val) * Mathf.Max(1f, (float) Screen.height / num);
  }

  private Vector2 GetScaledScreen() => new Vector2((float) Screen.width, (float) Screen.height) / this.GetGuiScaling();

  public static SceneDebugger Get() => ServiceManager.Get<SceneDebugger>();

  public static float GetDevTimescaleMultiplier() => HearthstoneApplication.IsPublic() ? 1f : Options.Get().GetFloat(Option.DEV_TIMESCALE, 1f);

  public static void SetDevTimescaleMultiplier(float f)
  {
    if (HearthstoneApplication.IsPublic() || (double) f == (double) TimeScaleMgr.Get().GetTimeScaleMultiplier())
      return;
    if ((double) f == 0.0)
      f = 0.0001f;
    Options.Get().SetFloat(Option.DEV_TIMESCALE, f);
    TimeScaleMgr.Get().SetTimeScaleMultiplier(f);
  }

  public void SetPlayerId(long? playerId) => this.m_playerId = playerId;

  public long? GetPlayerId_DebugOnly() => this.m_playerId;

  public void AddMessage(string message) => this.AddMessage(Blizzard.T5.Logging.LogLevel.Info, message);

  public void AddMessage(Blizzard.T5.Logging.LogLevel level, string message, bool autoShow = false)
  {
    if (this.m_messageWindow == null || !Array.Exists<Blizzard.T5.Logging.LogLevel>(SceneDebugger.LOG_LEVELS_TO_DISPLAY, (Predicate<Blizzard.T5.Logging.LogLevel>) (l => l == level)))
      return;
    this.m_messageWindow.AddEntry((LoggerDebugWindow.LogEntry) new SceneDebugger.ConsoleLogEntry(level, message), autoShow);
  }

  public void AddErrorMessage(string message) => this.AddMessage(Blizzard.T5.Logging.LogLevel.Error, message);

  public void AddSlushTimeEntry(
    int taskId,
    float expectedStart,
    float expectedEnd,
    float actualStart = 0.0f,
    float actualEnd = 0.0f,
    int entityId = 0)
  {
    this.m_slushTrackerWindow.AddEntry((LoggerDebugWindow.LogEntry) new SceneDebugger.SlushTimeRecord(taskId, expectedStart, expectedEnd, actualStart, actualEnd, entityId));
  }

  public void AddServerScriptLogMessage(ScriptLogMessage message)
  {
    int minSeverity = 3;
    if (message.Severity >= minSeverity && this.m_serverLogWindow.GetEntries().Count<LoggerDebugWindow.LogEntry>((Func<LoggerDebugWindow.LogEntry, bool>) (m => (m as SceneDebugger.ScriptWarning).Severity >= minSeverity)) == 0)
    {
      this.m_serverLogWindow.IsShown = true;
      this.m_serverLogWindow.IsExpanded = true;
    }
    string str1 = "";
    string powerDef = "";
    string pc = "";
    string str2 = "";
    StringBuilder stringBuilder = new StringBuilder();
    string message1 = message.Message;
    char[] chArray = new char[1]{ '|' };
    foreach (string input in message1.Split(chArray))
    {
      if (input.Length > 0)
      {
        if (input.StartsWith("source="))
        {
          Match match = Regex.Match(input, ".*source=(?<source>[^\\(]+) \\(ID=(?<entityId>[0-9]+)( CardID=(?<cardId>[^\\)]*))?\\).*");
          str1 = !match.Success ? input.Substring(7) : (match.Groups["cardId"].Length <= 0 ? string.Format("{0}", (object) match.Groups["source"]) : string.Format("{0} ({1})", (object) match.Groups["source"], (object) match.Groups["cardId"]));
        }
        else
        {
          if (input.StartsWith("powerDef="))
            powerDef = input.Substring(9);
          else if (input.StartsWith("pc="))
            pc = input.Substring(3);
          else if (input.StartsWith("entity="))
          {
            Match match = Regex.Match(input, ".*entity=(?<source>[^\\(]+) \\(ID=(?<entityId>[0-9]+)( CardID=(?<cardId>[^\\)]*))?\\).*");
            if (match.Success)
              str2 = match.Groups["cardId"].Length <= 0 ? string.Format("{0}", (object) match.Groups["source"]) : string.Format("{0} ({1})", (object) match.Groups["source"], (object) match.Groups["cardId"]);
          }
          stringBuilder.AppendFormat("{0}|", (object) input);
        }
      }
    }
    SceneDebugger.ScriptWarning entry = new SceneDebugger.ScriptWarning(str1.Length > 0 ? str1 : str2, message.Event, stringBuilder.ToString());
    if (message.HasSeverity)
      entry.Severity = message.Severity;
    entry.SetPowerDefInfo(powerDef, pc);
    entry.ComputeIssueGUID();
    this.m_serverLogWindow.AddEntry((LoggerDebugWindow.LogEntry) entry);
    string str3 = entry.ToString();
    Log.Gameplay.PrintWarning(str3);
    Debug.LogWarning((object) str3);
  }

  private Rect LayoutGuiControls(Rect space)
  {
    space.width = this.m_GUISize.x;
    space.yMax = this.GetScaledScreen().y;
    float yMin = space.yMin;
    Rect headerRect = this.m_guiWindow.GetHeaderRect();
    if (GUI.Button(new Rect(headerRect.xMax - headerRect.height, headerRect.y, headerRect.height, headerRect.height), "☰"))
      this.m_showGuiCustomization = !this.m_showGuiCustomization;
    if (this.m_showGuiCustomization)
    {
      space = this.LayoutCustomizeMenu(space);
    }
    else
    {
      space = this.m_timeSection.Layout(space);
      space = this.m_qualitySection.Layout(space);
      space = this.m_statsSection.Layout(space);
    }
    this.m_guiWindow.ResizeToFit(space.width, space.yMin - yMin);
    return new Rect(space.xMin, space.yMax, space.width, 0.0f);
  }

  private void LayoutCursorDebug()
  {
    if (Options.Get() == null || !((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null) || !Options.Get().GetBool(Option.DEBUG_CURSOR) || !HearthstoneApplication.IsInternal())
      return;
    RaycastHit hit;
    PegUIElement hitElement = PegUI.Get().FindHitElement(out hit);
    string text = "none";
    UnityEngine.Object collider = (UnityEngine.Object) hit.collider;
    if (collider != (UnityEngine.Object) null)
    {
      string str = string.Empty;
      if (PegUI.Get().IsUsingRenderPassPriorityHitTest)
        str = ", HitTestCamera=" + ((UnityEngine.Object) PegUI.Get().LastCameraPriorityHitCamera != (UnityEngine.Object) null ? PegUI.Get().LastCameraPriorityHitCamera.name : "none");
      text = string.Format("<color=#FFFFFF>{0}: {1}\nObjLayer={2}, HasPegUI={3}, RenderPassPriority={4}{5}</color>", (object) ((object) collider).GetType().ToString(), (object) DebugUtils.GetHierarchyPath(collider, '/'), (object) (GameLayer) hit.collider.gameObject.layer, (object) ((UnityEngine.Object) hitElement != (UnityEngine.Object) null), (object) PegUI.Get().IsUsingRenderPassPriorityHitTest, (object) str);
    }
    Vector2 scaledScreen = this.GetScaledScreen();
    GUIStyle style = new GUIStyle((GUIStyle) "box")
    {
      fontSize = GUI.skin.button.fontSize,
      fontStyle = GUI.skin.button.fontStyle,
      alignment = TextAnchor.UpperLeft,
      wordWrap = true,
      clipping = TextClipping.Overflow,
      stretchWidth = true,
      richText = true
    };
    GUI.Box(new Rect(scaledScreen.x / 2f, 0.0f, scaledScreen.x / 2f, this.m_GUISize.y * 3f), text, style);
  }

  private Rect LayoutTimeControls(Rect space)
  {
    SceneDebugger.SetDevTimescaleMultiplier(GUI.HorizontalSlider(new Rect(space.min, this.m_GUISize), TimeScaleMgr.Get().GetTimeScaleMultiplier(), 0.01f, 4f));
    space.yMin += 0.5f * this.m_GUISize.y;
    GUI.Box(new Rect(space.min, this.m_GUISize), string.Format("Time Scale: {0}", (object) TimeScaleMgr.Get().GetTimeScaleMultiplier()));
    space.yMin += 0.75f * this.m_GUISize.y;
    if (GUI.Button(new Rect(space.min, this.m_GUISize), "Reset Time Scale"))
      SceneDebugger.SetDevTimescaleMultiplier(1f);
    space.yMin += 1.1f * this.m_GUISize.y;
    return space;
  }

  private Rect LayoutQualityControls(Rect space)
  {
    if (this.m_graphicsManager == null)
      return space;
    string text1 = "Low";
    if (this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.Low)
      text1 = "<color=cyan>Low</color>";
    string text2 = "Medium";
    if (this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.Medium)
      text2 = "<color=cyan>Medium</color>";
    string text3 = "High";
    if (this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.High)
      text3 = "<color=cyan>High</color>";
    float width = space.width / 3f;
    if (GUI.Button(new Rect(space.xMin, space.yMin, width, this.m_GUISize.y), text1))
      this.m_graphicsManager.RenderQualityLevel = GraphicsQuality.Low;
    if (GUI.Button(new Rect(space.xMin + width, space.yMin, width, this.m_GUISize.y), text2))
      this.m_graphicsManager.RenderQualityLevel = GraphicsQuality.Medium;
    if (GUI.Button(new Rect(space.xMin + width * 2f, space.yMin, width, this.m_GUISize.y), text3))
      this.m_graphicsManager.RenderQualityLevel = GraphicsQuality.High;
    space.yMin += this.m_GUISize.y;
    return space;
  }

  private Rect LayoutStats(Rect space)
  {
    float lineHeight = GUI.skin.box.lineHeight;
    float vertical = (float) GUI.skin.box.border.vertical;
    float height1 = 2f * lineHeight + vertical;
    GUI.Box(new Rect(space.xMin, space.yMin, this.m_GUISize.x, height1), this.m_fpsText);
    space.yMin += height1;
    string text = string.Format("Build: {0}.{1}\nServer: {2}", (object) "25.0", (object) 158725, (object) Network.GetVersion());
    float height2 = 2f * lineHeight + vertical;
    IGameDownloadManager downloadMgr = GameDownloadManagerProvider.Get();
    if ((PlatformSettings.IsMobileRuntimeOS || Application.isEditor && PlatformSettings.IsEmulating) && downloadMgr != null)
    {
      string downloadOverrideString = this.GetDownloadOverrideString(downloadMgr);
      text += downloadOverrideString;
      height2 += lineHeight;
    }
    if (HearthstoneApplication.IsInternal() && this.m_playerId.HasValue)
    {
      text += string.Format("\nPlayer Id: {0}", (object) this.m_playerId);
      height2 += lineHeight;
    }
    if (!string.IsNullOrEmpty(Network.GetUsername()))
    {
      text += string.Format("\nAccount: {0}", (object) Network.GetUsername().Split('@')[0]);
      height2 += lineHeight;
    }
    GUI.Box(new Rect(space.xMin, space.yMin, this.m_GUISize.x, height2), text);
    space.yMin += height2;
    if (Application.isEditor && AssetLoaderPrefs.AssetLoadingMethod == AssetLoaderPrefs.ASSET_LOADING_METHOD.ASSET_BUNDLES)
    {
      GUI.Box(new Rect(space.min, this.m_GUISize), "<color=red>Using Asset Bundles</color>");
      space.yMin += this.m_GUISize.y;
    }
    return space;
  }

  private string GetDownloadOverrideString(IGameDownloadManager downloadMgr)
  {
    string patchOverrideUrl = downloadMgr.PatchOverrideUrl;
    string versionOverrideUrl = downloadMgr.VersionOverrideUrl;
    bool flag1 = patchOverrideUrl.Equals("Live");
    bool flag2 = versionOverrideUrl.Equals("Live");
    if (flag1 & flag2)
      return "\nPatch & VerSrv: Live";
    string downloadOverrideString = "";
    if (!flag1)
      downloadOverrideString = string.Format("\nPatch: {0}", (object) patchOverrideUrl);
    if (!flag2)
      downloadOverrideString += string.Format("\nVersionSrv: {0}", (object) versionOverrideUrl);
    return downloadOverrideString;
  }

  private Rect LayoutMessages(Rect space)
  {
    Rect rect = new Rect(space.min, this.m_GUISize);
    if (GUI.Button(rect, string.Format("Clear ({0})", (object) this.m_messageWindow.GetEntries().Count<LoggerDebugWindow.LogEntry>((Func<LoggerDebugWindow.LogEntry, bool>) (m => this.m_messageWindow.AreLogsDisplayed((object) m.category))))))
      this.m_messageWindow.Clear();
    rect.xMin = rect.xMax + 10f;
    rect.width = 40f;
    GUI.Label(new Rect(rect), "Filter:");
    rect.xMin = rect.xMax;
    rect.xMax = space.xMax - 100f * (float) ((IEnumerable<Blizzard.T5.Logging.LogLevel>) SceneDebugger.LOG_LEVELS_TO_DISPLAY).Count<Blizzard.T5.Logging.LogLevel>();
    this.m_messageWindow.FilterString = GUI.TextField(rect, this.m_messageWindow.FilterString);
    foreach (Blizzard.T5.Logging.LogLevel category in SceneDebugger.LOG_LEVELS_TO_DISPLAY)
    {
      rect.xMin = rect.xMax;
      rect.width = 100f;
      bool flag = this.m_messageWindow.AreLogsDisplayed((object) category);
      int count = this.m_messageWindow.GetCount(category);
      string text = string.Format("<color={0}>{1} ({2})</color>", flag ? (object) "white" : (object) "grey", (object) category.ToString(), (object) count);
      if (GUI.Button(rect, text))
        this.m_messageWindow.ToggleLogsDisplay((object) category, !flag);
    }
    space.yMin = rect.yMax;
    return this.m_messageWindow.LayoutLog(space);
  }

  private Rect LayoutRatingDebug(Rect space)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.m_debugMedalInfo != null)
    {
      stringBuilder.AppendLine(string.Format("{0}", (object) (RatingDebugOption) this.m_debugMedalInfo.RatingId));
      stringBuilder.AppendLine(string.Format("Rating ID: {0}", (object) this.m_debugMedalInfo.RatingId));
      stringBuilder.AppendLine(string.Format("Rating: {0}", (object) this.m_debugMedalInfo.Rating));
      stringBuilder.AppendLine(string.Format("Variance: {0}", (object) this.m_debugMedalInfo.Variance));
      stringBuilder.Append(string.Format("Public Rating: {0}", (object) this.m_debugMedalInfo.PublicRating));
      if (this.m_debugMedalInfo.LeagueId != 0)
      {
        stringBuilder.AppendLine("\n");
        stringBuilder.AppendLine(string.Format("League ID: {0}", (object) this.m_debugMedalInfo.LeagueId));
        stringBuilder.AppendLine(string.Format("Season ID: {0}", (object) this.m_debugMedalInfo.SeasonId));
        stringBuilder.AppendLine(string.Format("Games: {0}", (object) this.m_debugMedalInfo.SeasonGames));
        stringBuilder.AppendLine(string.Format("Wins: {0}", (object) this.m_debugMedalInfo.SeasonWins));
        stringBuilder.AppendLine(string.Format("Streak: {0}", (object) this.m_debugMedalInfo.Streak));
        stringBuilder.AppendLine(string.Empty);
        stringBuilder.AppendLine(string.Format("Stars Per Win: {0}", (object) this.m_debugMedalInfo.StarsPerWin));
        stringBuilder.AppendLine(string.Format("Star Level: {0}", (object) this.m_debugMedalInfo.StarLevel));
        stringBuilder.AppendLine(string.Format("Stars: {0}", (object) this.m_debugMedalInfo.Stars));
        stringBuilder.AppendLine(string.Format("LegendRank: {0}", (object) this.m_debugMedalInfo.LegendRank));
        stringBuilder.AppendLine(string.Empty);
        stringBuilder.AppendLine(string.Format("Best Star Level: {0}", (object) this.m_debugMedalInfo.BestStarLevel));
        stringBuilder.AppendLine(string.Format("Best Stars: {0}", (object) this.m_debugMedalInfo.BestStars));
        stringBuilder.AppendLine(string.Format("Best Rating: {0}", (object) this.m_debugMedalInfo.BestRating));
        stringBuilder.AppendLine(string.Empty);
        stringBuilder.AppendLine(string.Format("Best Ever League ID: {0}", (object) this.m_debugMedalInfo.BestEverLeagueId));
        stringBuilder.Append(string.Format("Best Ever Star Level: {0}", (object) this.m_debugMedalInfo.BestEverStarLevel));
      }
    }
    string text = stringBuilder.ToString();
    GUIStyle style = new GUIStyle(GUI.skin.box);
    style.alignment = TextAnchor.MiddleLeft;
    GUIContent content = new GUIContent(text);
    space.height = style.CalcHeight(content, space.width);
    GUI.Box(space, text, style);
    float y = this.m_GUISize.y;
    if (GUI.Button(new Rect(space.xMin, space.yMax, space.width, y), "Refresh") || (double) UnityEngine.Time.realtimeSinceStartup - (double) this.m_lastMedalInfoRequestTime >= 5.0)
      this.RequestDebugRatingInfo();
    space.yMax += y;
    this.m_ratingWindow.ResizeToFit(new Vector2(space.width, space.height));
    space.yMin = space.yMax;
    return space;
  }

  public void RequestDebugRatingInfo()
  {
    int ratingId = (int) Options.Get().GetEnum<RatingDebugOption>(Option.RATING_DEBUG);
    Network.Get().SetDebugRatingInfo(ratingId);
    this.m_lastMedalInfoRequestTime = UnityEngine.Time.realtimeSinceStartup;
  }

  private void OnDebugRatingInfoResponse()
  {
    DebugRatingInfoResponse ratingInfoResponse = Network.Get().GetDebugRatingInfoResponse();
    if (ratingInfoResponse == null)
      return;
    this.m_debugMedalInfo = ratingInfoResponse.MedalData;
  }

  private Rect LayoutAssetsDebug(Rect space)
  {
    space = AssetLoaderDebug.LayoutUI(space);
    this.m_assetsWindow.ResizeToFit(new Vector2(space.width, space.height));
    return space;
  }

  private Rect LayoutGameplayDebug(Rect space)
  {
    space = GameplayDebug.LayoutUI(space);
    this.m_gameplayWindow.ResizeToFit(new Vector2(space.width, space.height));
    return space;
  }

  private Rect LayoutPresenceDebug(Rect space)
  {
    space = PresenceDebug.LayoutUI(space);
    this.m_gameplayWindow.ResizeToFit(new Vector2(space.width, space.height));
    return space;
  }

  private Rect LayoutQuestDebug(Rect space)
  {
    string str = QuestManager.Get()?.GetQuestDebugHudString() ?? string.Empty;
    GUIStyle style = new GUIStyle(GUI.skin.box);
    style.alignment = TextAnchor.MiddleLeft;
    GUIContent content = new GUIContent(str);
    space.height = style.CalcHeight(content, space.width);
    GUI.Box(space, str, style);
    float y = this.m_GUISize.y;
    if (GUI.Button(new Rect(space.xMin, space.yMax, space.width, y), "Copy to Clipboard"))
      ClipboardUtils.CopyToClipboard(str);
    space.yMax += y;
    this.m_questWindow.ResizeToFit(new Vector2(space.width, space.height));
    space.yMin = space.yMax;
    return space;
  }

  private Rect LayoutAchievementDebug(Rect space)
  {
    string str = AchievementManager.Get()?.Debug_GetAchievementHudString() ?? string.Empty;
    GUIStyle style = new GUIStyle(GUI.skin.box);
    style.alignment = TextAnchor.MiddleLeft;
    GUIContent content = new GUIContent(str);
    space.height = style.CalcHeight(content, space.width);
    GUI.Box(space, str, style);
    float y = this.m_GUISize.y;
    if (GUI.Button(new Rect(space.xMin, space.yMax, space.width, y), "Copy to Clipboard"))
      ClipboardUtils.CopyToClipboard(str);
    space.yMax += y;
    this.m_achievementWindow.ResizeToFit(new Vector2(space.width, space.height));
    space.yMin = space.yMax;
    return space;
  }

  private Rect LayoutRewardTrackDebug(Rect space)
  {
    string str = RewardTrackManager.Get()?.GetRewardTrack(Global.RewardTrackType.GLOBAL)?.GetRewardTrackDebugHudString() ?? string.Empty;
    GUIStyle style = new GUIStyle(GUI.skin.box);
    style.alignment = TextAnchor.MiddleLeft;
    GUIContent content = new GUIContent(str);
    space.height = style.CalcHeight(content, space.width);
    GUI.Box(space, str, style);
    float y = this.m_GUISize.y;
    if (GUI.Button(new Rect(space.xMin, space.yMax, space.width, y), "Copy to Clipboard"))
      ClipboardUtils.CopyToClipboard(str);
    space.yMax += y;
    this.m_rewardTrackWindow.ResizeToFit(new Vector2(space.width, space.height));
    space.yMin = space.yMax;
    return space;
  }

  private Rect LayoutNotepadDebug(Rect space)
  {
    GUIStyle guiStyle = new GUIStyle(GUI.skin.box);
    guiStyle.alignment = TextAnchor.MiddleLeft;
    GUIContent content = new GUIContent("");
    space.height = guiStyle.CalcHeight(content, space.width);
    string path = Directory.GetCurrentDirectory() + "\\notepad.txt";
    if (this.m_notepadFirstRun)
    {
      if (!File.Exists(path))
        File.Create(path).Close();
      else
        this.m_notepadContents = File.ReadAllText(path);
      this.m_notepadFirstRun = false;
    }
    GUILayout.BeginArea(new Rect(space.xMin, space.yMax, space.width, 300f));
    GUILayout.BeginVertical();
    this.scrollViewVector = GUILayout.BeginScrollView(this.scrollViewVector);
    this.m_notepadContents = GUILayout.TextArea(this.m_notepadContents, GUILayout.ExpandHeight(true));
    GUILayout.EndScrollView();
    GUILayout.BeginHorizontal();
    float y = this.m_GUISize.y;
    if (GUILayout.Button("Copy to Clipboard"))
      ClipboardUtils.CopyToClipboard(this.m_notepadContents);
    if (GUILayout.Button("Save Contents"))
      File.WriteAllText(path, this.m_notepadContents);
    space.yMax += y;
    space.yMin = space.yMax;
    GUILayout.EndHorizontal();
    GUILayout.EndVertical();
    GUILayout.EndArea();
    return space;
  }

  private Rect LayoutScriptWarnings(Rect space)
  {
    Vector2 min = space.min;
    Vector2 guiSize = this.m_GUISize;
    if (GUI.Button(new Rect(min.x, min.y, guiSize.x, guiSize.y), "Clear Script Warnings"))
      this.m_serverLogWindow.Clear();
    min.x += guiSize.x;
    if (GUI.Button(new Rect(min.x, min.y, guiSize.x, guiSize.y), "Search JIRA for GUID") && this.m_serverLogWindow.GetEntries().LastOrDefault<LoggerDebugWindow.LogEntry>() is SceneDebugger.ScriptWarning scriptWarning)
      Application.OpenURL(string.Format("https://jira.blizzard.com/issues/?jql=text~%22{0}%22", (object) UnityWebRequest.EscapeURL(scriptWarning.IssueGUID)));
    min.x += guiSize.x;
    min.y += guiSize.y;
    space.yMin = min.y;
    return this.m_serverLogWindow.LayoutLog(space);
  }

  private void LayoutButton(
    ref Vector2 offset,
    float top,
    Vector2 size,
    string label,
    Action action)
  {
    if ((double) offset.y + (double) size.y > (double) this.GetScaledScreen().y)
    {
      offset.y = top + size.y;
      offset.x += 1.1f * size.x;
    }
    if (GUI.Button(new Rect(offset.x, offset.y, size.x, size.y), label))
      action();
    offset.y += 1.1f * size.y;
  }

  private Rect LayoutCustomizeMenu(Rect space)
  {
    List<DebuggerGui> debuggerGuiList = new List<DebuggerGui>();
    debuggerGuiList.Add((DebuggerGui) this.m_cheatsWindow);
    if (this.m_messageWindow != null)
      debuggerGuiList.Add((DebuggerGui) this.m_messageWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_serverLogWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_ratingWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_assetsWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_questWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_achievementWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_rewardTrackWindow);
    debuggerGuiList.Add(this.m_timeSection);
    debuggerGuiList.Add(this.m_qualitySection);
    debuggerGuiList.Add(this.m_statsSection);
    debuggerGuiList.Add((DebuggerGui) this.m_slushTrackerWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_notepadWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_gameplayWindow);
    debuggerGuiList.Add((DebuggerGui) this.m_presenceWindow);
    Vector2 min1 = space.min;
    Vector2 min2 = space.min;
    foreach (DebuggerGui debuggerGui in debuggerGuiList)
    {
      DebuggerGui section = debuggerGui;
      string label = (section.IsShown ? "☑" : "☐") + " " + section.Title;
      this.LayoutButton(ref min2, 0.0f, this.m_GUISize, label, (Action) (() => section.IsShown = !section.IsShown));
      if ((double) min2.x > (double) min1.x)
      {
        min1.x = min2.x;
        space.width += min1.x;
      }
      if ((double) min2.y > (double) min1.y)
        min1.y = min2.y;
    }
    space.yMin = min1.y;
    return space;
  }

  private void HandleGuiChanged() => this.m_guiSaveTimer = 3;

  public class ConsoleLogEntry : LoggerDebugWindow.LogEntry
  {
    public ConsoleLogEntry(Blizzard.T5.Logging.LogLevel level, string message)
    {
      this.category = level;
      message = message.Trim();
      switch (level)
      {
        case Blizzard.T5.Logging.LogLevel.Debug:
          message = string.Format("<color=grey>{0}</color>", (object) message);
          break;
        case Blizzard.T5.Logging.LogLevel.Warning:
          message = string.Format("<color=yellow>{0}</color>", (object) message);
          break;
        case Blizzard.T5.Logging.LogLevel.Error:
          message = string.Format("<color=red>{0}</color>", (object) message);
          break;
      }
      DateTime now = DateTime.Now;
      this.text = string.Format("{0} {1}", (object) string.Format("<color=grey>[{0}:{1}:{2}]</color>", (object) now.Hour.ToString().PadLeft(2, '0'), (object) now.Minute.ToString().PadLeft(2, '0'), (object) now.Second.ToString().PadLeft(2, '0')), (object) message);
    }
  }

  private class SlushTimeRecord : LoggerDebugWindow.LogEntry
  {
    public float ExpectedStart { get; set; }

    public float ExpectedEnd { get; set; }

    public int TaskId { get; set; }

    public float ActualStart { get; set; }

    public float ActualEnd { get; set; }

    public int EntityId { get; set; }

    public SlushTimeRecord(
      int taskId,
      float expectedStart,
      float expectedEnd,
      float actualStart = 0.0f,
      float actualEnd = 0.0f,
      int entityId = 0)
    {
      this.TaskId = taskId;
      this.ExpectedStart = expectedStart;
      this.ExpectedEnd = expectedEnd;
      this.ActualStart = actualStart;
      this.ActualEnd = actualEnd;
      this.EntityId = entityId;
      this.text = this.ToString();
    }

    private float GetDuration(float start, float end) => end - start;

    public override string ToString()
    {
      float duration1 = this.GetDuration(this.ExpectedStart, this.ExpectedEnd);
      double duration2 = (double) this.GetDuration(this.ActualStart, this.ActualEnd);
      float num1 = this.ActualStart - this.ExpectedStart;
      double num2 = (double) duration1;
      float num3 = (float) (duration2 - num2) + num1;
      string str1 = (double) num3 > 0.0 ? "+" : "";
      string str2 = "";
      if (this.EntityId != 0)
      {
        Entity entity = GameState.Get().GetEntity(this.EntityId);
        if (entity != null)
          str2 = entity.GetName();
      }
      return string.Format("TaskId: {0}, ({1}) {2}{3}", (object) this.TaskId, (object) str2, (object) str1, (object) num3);
    }
  }

  private class ScriptWarning : LoggerDebugWindow.LogEntry
  {
    public string Source { get; private set; }

    public string Event { get; private set; }

    public string Message { get; private set; }

    public int Severity { get; set; }

    public string PowerDef { get; private set; }

    public int PC { get; private set; }

    public string IssueGUID { get; private set; }

    public ScriptWarning(string logSource, string logEvent, string logMessage)
    {
      this.Source = logSource;
      this.Event = logEvent;
      this.Message = logMessage;
      this.Severity = -1;
      this.PowerDef = "";
      this.PC = -1;
      this.IssueGUID = "";
      this.RebuildString();
    }

    public void SetPowerDefInfo(string powerDef, string pc)
    {
      int result;
      if (powerDef.Length < 0 || pc.Length < 0 || !int.TryParse(pc, out result))
        return;
      this.PowerDef = powerDef;
      this.PC = result;
      this.RebuildString();
    }

    public string ComputeIssueGUID()
    {
      string s = "";
      if (this.PowerDef.Length > 0 && this.PC >= 0)
        s = string.Format("{0}|{1}|{2}", (object) this.Event, (object) this.PowerDef, (object) this.PC);
      else if (this.Source.Length > 0)
        s = string.Format("{0}|{1}", (object) this.Event, (object) this.Source);
      if (s.Length <= 0)
        return "";
      this.IssueGUID = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(s)));
      this.RebuildString();
      return this.IssueGUID;
    }

    public void RebuildString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format("<color=red>-> [{0}]</color>", (object) this.Event));
      if (this.Source.Length > 0)
        stringBuilder.AppendLine(string.Format("    -source={0}", (object) this.Source));
      string message = this.Message;
      char[] chArray = new char[1]{ '|' };
      foreach (string str in message.Split(chArray))
      {
        if (str.Length > 0)
          stringBuilder.AppendLine(string.Format("    -{0}", (object) str));
      }
      if (this.IssueGUID.Length > 0)
        stringBuilder.AppendLine(string.Format("    -(guid: {0})", (object) this.IssueGUID));
      this.text = stringBuilder.ToString();
    }

    public override string ToString() => string.Format("Received script warning from '{0}'!  event:[{1}]  message:\"{2}\"  guid:({3})", (object) this.Source, (object) this.Event, (object) this.Message, (object) this.IssueGUID);
  }
}
