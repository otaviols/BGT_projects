using Hearthstone;
using PegasusGame;
using System.Collections.Generic;
using UnityEngine;

public class ScriptDebugDisplay : MonoBehaviour
{
  private static ScriptDebugDisplay s_instance;
  private List<ScriptDebugInformation> m_debugInformation = new List<ScriptDebugInformation>();
  public bool m_isDisplayed;
  private float m_currentDumpScrollBarValue = 1f;
  private float m_currentStatementScrollBarValue;

  public static ScriptDebugDisplay Get()
  {
    if ((Object) ScriptDebugDisplay.s_instance == (Object) null)
    {
      GameObject gameObject = new GameObject();
      ScriptDebugDisplay.s_instance = gameObject.AddComponent<ScriptDebugDisplay>();
      gameObject.name = "ScriptDebugDisplay (Dynamically created)";
    }
    return ScriptDebugDisplay.s_instance;
  }

  private void Start()
  {
    if (HearthstoneApplication.IsPublic() || GameState.Get() == null)
      return;
    GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent), (object) null);
  }

  private void GameState_CreateGameEvent(GameState.CreateGamePhase createGamePhase, object userData) => this.m_debugInformation.Clear();

  public bool ToggleDebugDisplay(bool shouldDisplay)
  {
    this.m_isDisplayed = shouldDisplay;
    return true;
  }

  private void Update()
  {
    if (HearthstoneApplication.IsPublic() || GameState.Get() == null || !this.m_isDisplayed)
      return;
    ScriptDebugInformation debugInfo = (ScriptDebugInformation) null;
    if (this.m_debugInformation.Count > 0)
      debugInfo = this.m_debugInformation[this.GetCurrentDumpIndex()];
    if (debugInfo == null)
      return;
    this.UpdateDisplay(debugInfo);
  }

  private int GetCurrentDumpIndex()
  {
    int currentDumpIndex = (int) ((double) this.m_currentDumpScrollBarValue * (double) this.m_debugInformation.Count);
    if (currentDumpIndex >= this.m_debugInformation.Count)
      currentDumpIndex = this.m_debugInformation.Count - 1;
    return currentDumpIndex;
  }

  private void UpdateDisplay(ScriptDebugInformation debugInfo)
  {
    string str1 = string.Format("Script Debug: {0} (ID{1})\n", (object) debugInfo.EntityName, (object) debugInfo.EntityID);
    Vector3 position1 = new Vector3((float) Screen.width, (float) Screen.height, 0.0f);
    Vector3 position2 = new Vector3(0.0f, (float) Screen.height, 0.0f);
    int num1 = (int) ((double) this.m_currentStatementScrollBarValue * (double) debugInfo.Calls.Count);
    if (num1 >= debugInfo.Calls.Count)
      num1 = debugInfo.Calls.Count - 1;
    int num2 = 0;
    foreach (ScriptDebugCall call in debugInfo.Calls)
    {
      string stringToAppend1 = call.OpcodeName;
      if (num2 == num1)
      {
        str1 = call.ErrorStrings.Count <= 0 ? this.AppendLine(str1, string.Format("<color=#00ff00ff>{0}</color>", (object) stringToAppend1)) : this.AppendLine(str1, string.Format("<color=#ffff00ff>{0}</color>", (object) stringToAppend1));
        string str2 = "Inputs";
        int num3 = 0;
        foreach (ScriptDebugVariable input in call.Inputs)
        {
          str2 = this.AppendVariable(str2, input, string.Format("Input Variable {0}", (object) num3));
          ++num3;
        }
        if (call.ErrorStrings.Count > 0)
        {
          str2 = this.AppendLine(str2, "\n<color=#ff0000ff>ERRORS</color>");
          foreach (object errorString in call.ErrorStrings)
          {
            string stringToAppend2 = string.Format("<color=#ff0000ff>{0}</color>", errorString);
            str2 = this.AppendLine(str2, stringToAppend2);
          }
        }
        string str3 = this.AppendLine(str2, "\nOutput");
        if (call.Output.IntValue.Count > 0 || call.Output.StringValue.Count > 0)
          str3 = this.AppendVariable(str3, call.Output, string.Format("Output Variable"));
        string str4 = this.AppendLine(str3, "\nOther variables");
        int num4 = 0;
        foreach (ScriptDebugVariable variable in call.Variables)
        {
          str4 = this.AppendVariable(str4, variable, string.Format("Other Variable {0}", (object) num4));
          ++num4;
        }
        DebugTextManager.Get().DrawDebugText(str4, position2, 0.0f, true);
      }
      else
      {
        if (call.ErrorStrings.Count > 0)
          stringToAppend1 = string.Format("<color=#ff0000ff>{0}</color>", (object) stringToAppend1);
        str1 = this.AppendLine(str1, stringToAppend1);
      }
      ++num2;
    }
    DebugTextManager.Get().DrawDebugText(str1, position1, 0.0f, true, "ScriptDebugDisplayCallLog");
  }

  private string AppendVariable(
    string inspectString,
    ScriptDebugVariable variable,
    string defaultVariableName)
  {
    string stringToAppend = "";
    string str = variable.VariableName;
    if (str == "")
      str = defaultVariableName;
    if (variable.IntValue.Count == 1)
      stringToAppend = string.Format("{0} ({1}): {2}", (object) str, (object) variable.VariableType, (object) variable.IntValue[0]);
    else if (variable.StringValue.Count == 1)
      stringToAppend = string.Format("{0} ({1}): {2}", (object) str, (object) variable.VariableType, (object) variable.StringValue[0]);
    else if (variable.IntValue.Count > 1)
    {
      stringToAppend = string.Format("{0} ({1}): {2}", (object) str, (object) variable.VariableType, (object) variable.IntValue[0]);
      for (int index = 1; index < variable.IntValue.Count; ++index)
        stringToAppend = string.Format("{0}, {1}", (object) stringToAppend, (object) variable.IntValue[index]);
    }
    else if (variable.StringValue.Count > 1)
    {
      stringToAppend = string.Format("{0} ({1}):", (object) str, (object) variable.VariableType);
      for (int index = 0; index < variable.StringValue.Count; ++index)
        stringToAppend = string.Format("{0}\n{1}", (object) stringToAppend, (object) variable.StringValue[index]);
    }
    if (stringToAppend != "")
      inspectString = this.AppendLine(inspectString, stringToAppend);
    return inspectString;
  }

  private string AppendLine(string inputString, string stringToAppend) => string.Format("{0}\n{1}", (object) inputString, (object) stringToAppend);

  public void OnScriptDebugInfo(ScriptDebugInformation debugInfo) => this.m_debugInformation.Add(debugInfo);
}
