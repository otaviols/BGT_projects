using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CheatMgr : IService
{
  private Map<string, List<Delegate>> m_funcMap = new Map<string, List<Delegate>>();
  private Map<string, string> m_cheatAlias = new Map<string, string>();
  private Map<string, string> m_cheatDesc = new Map<string, string>();
  private Map<string, string> m_cheatArgs = new Map<string, string>();
  private Map<string, string> m_cheatExamples = new Map<string, string>();
  private Map<string, int> m_cheatCategoryIndex = new Map<string, int>();
  private List<string> m_categoryList = new List<string>();
  private int m_lastRegisteredCategoryIndex = -1;
  private List<string> m_cheatHistory;
  private int m_cheatHistoryIndex = -1;
  private string m_cheatTextBeforeScrollingThruHistory;
  private string m_cheatTextBeforeAutofill;
  private int m_autofillMatchIndex = -1;
  private string m_lastAutofillParamFunc;
  private string m_lastAutofillParamPrefix;
  private string m_lastAutofillParamMatch;
  private const string DEFAULT_CATEGORY = "other";
  private const int MAX_HISTORY_LINES = 25;
  private const float CHEAT_CONSOLE_PADDING = 1f;
  private const float CHEAT_CONSOLE_HEIGHT = 30f;
  private bool m_closingConsole;
  private GameObject m_sceneObject;
  private static CheatMgr s_instance;

  public Map<string, string> cheatDesc => this.m_cheatDesc;

  public Map<string, string> cheatArgs => this.m_cheatArgs;

  public Map<string, string> cheatExamples => this.m_cheatExamples;

  private GameObject SceneObject
  {
    get
    {
      if ((UnityEngine.Object) this.m_sceneObject == (UnityEngine.Object) null)
        this.m_sceneObject = new GameObject("CheatMgrSceneObject", new System.Type[1]
        {
          typeof (HSDontDestroyOnLoad)
        });
      return this.m_sceneObject;
    }
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    this.m_cheatHistory = new List<string>();
    yield break;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown() => CheatMgr.s_instance = (CheatMgr) null;

  public static CheatMgr Get()
  {
    if (CheatMgr.s_instance == null)
      CheatMgr.s_instance = ServiceManager.Get<CheatMgr>();
    return CheatMgr.s_instance;
  }

  public IEnumerable<string> GetCheatCommands() => (IEnumerable<string>) this.m_funcMap.Keys;

  public bool HandleKeyboardInput()
  {
    if (HearthstoneApplication.IsPublic() || !InputCollection.GetKeyUp(KeyCode.BackQuote))
      return false;
    if (this.m_closingConsole)
    {
      this.m_closingConsole = false;
      return true;
    }
    this.ShowConsole();
    return true;
  }

  public void ShowConsole()
  {
    Rect rect = new Rect(0.0f, 0.0f, 1f, 0.05f);
    this.m_cheatHistoryIndex = -1;
    this.m_cheatTextBeforeAutofill = (string) null;
    this.m_autofillMatchIndex = -1;
    this.ReadCheatHistoryOption();
    this.m_cheatTextBeforeScrollingThruHistory = (string) null;
    UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams()
    {
      m_owner = this.SceneObject,
      m_preprocessCallback = new UniversalInputManager.TextInputPreprocessCallback(this.OnInputPreprocess),
      m_rect = rect,
      m_color = new Color?(Color.white),
      m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
      m_showBackground = true
    };
    UniversalInputManager.Get().UseTextInput(parms);
  }

  public void HideConsole() => UniversalInputManager.Get().CancelTextInput(this.SceneObject);

  private void ReadCheatHistoryOption() => this.m_cheatHistory = new List<string>((IEnumerable<string>) Options.Get().GetString(Option.CHEAT_HISTORY).Split(';'));

  private void WriteCheatHistoryOption() => Options.Get().SetString(Option.CHEAT_HISTORY, string.Join(";", this.m_cheatHistory.ToArray()));

  private bool OnInputPreprocess()
  {
    if (Input.GetKeyDown(KeyCode.BackQuote) && string.IsNullOrEmpty(UniversalInputManager.Get().GetInputText()))
    {
      this.m_closingConsole = true;
      UniversalInputManager.Get().CancelTextInput(this.SceneObject);
      return true;
    }
    if (this.m_cheatHistory.Count < 1)
      return false;
    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      if (this.m_cheatHistoryIndex >= this.m_cheatHistory.Count - 1)
        return true;
      string inputText = UniversalInputManager.Get().GetInputText();
      if (this.m_cheatTextBeforeScrollingThruHistory == null)
        this.m_cheatTextBeforeScrollingThruHistory = inputText;
      string text = this.m_cheatHistory[++this.m_cheatHistoryIndex];
      UniversalInputManager.Get().SetInputText(text, true);
      return true;
    }
    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      string scrollingThruHistory;
      if (this.m_cheatHistoryIndex <= 0)
      {
        this.m_cheatHistoryIndex = -1;
        if (this.m_cheatTextBeforeScrollingThruHistory == null)
          return false;
        scrollingThruHistory = this.m_cheatTextBeforeScrollingThruHistory;
        this.m_cheatTextBeforeScrollingThruHistory = (string) null;
      }
      else
        scrollingThruHistory = this.m_cheatHistory[--this.m_cheatHistoryIndex];
      UniversalInputManager.Get().SetInputText(scrollingThruHistory);
      return true;
    }
    if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Backspace))
    {
      string inputText = UniversalInputManager.Get().GetInputText();
      if (inputText[inputText.Length - 1] == ' ')
        inputText.Trim();
      string text = !inputText.Contains(" ") ? "" : inputText.Substring(0, inputText.LastIndexOf(' '));
      UniversalInputManager.Get().SetInputText(text);
    }
    if (Input.GetKeyDown(KeyCode.Tab) && HearthstoneApplication.IsInternal())
    {
      string text = UniversalInputManager.Get().GetInputText();
      int num = !text.Contains(' ') ? 1 : 0;
      bool isShiftTab = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
      if (num != 0)
      {
        bool flag = true;
        if (this.m_cheatTextBeforeAutofill != null)
        {
          text = this.m_cheatTextBeforeAutofill;
          flag = false;
        }
        else
          this.m_cheatTextBeforeAutofill = text;
        List<string> list = this.m_funcMap.Keys.Where<string>((Func<string, bool>) (f => f.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))).ToList<string>();
        if (list.Count > 0)
        {
          list.Sort();
          int index = 0;
          this.m_autofillMatchIndex += isShiftTab ? -1 : 1;
          if (this.m_autofillMatchIndex >= list.Count)
            this.m_autofillMatchIndex = 0;
          else if (this.m_autofillMatchIndex < 0)
            this.m_autofillMatchIndex = list.Count - 1;
          if (this.m_autofillMatchIndex >= 0 && this.m_autofillMatchIndex < list.Count)
            index = this.m_autofillMatchIndex;
          text = list[index];
          UniversalInputManager.Get().SetInputText(text, true);
          if (flag && list.Count > 1)
          {
            float delay = (5f + Mathf.Max(0.0f, (float) (list.Count - 3))) * Time.timeScale;
            UIStatus.Get().AddInfo("Available cheats:\n" + string.Join("   ", list.ToArray()), delay);
          }
        }
      }
      else
      {
        string[] args;
        string rawArgs;
        string funcAndArgs = this.ParseFuncAndArgs(text, out args, out rawArgs);
        if (funcAndArgs == null)
          return false;
        UIStatus.Get().AddInfo("", 0.0f);
        if (this.CallCheatCallback(funcAndArgs, args, rawArgs, true, isShiftTab))
        {
          string str;
          if (string.IsNullOrEmpty(this.m_lastAutofillParamPrefix) && rawArgs.EndsWith(" "))
          {
            str = rawArgs + this.m_lastAutofillParamMatch;
          }
          else
          {
            args[args.Length - 1] = this.m_lastAutofillParamMatch;
            str = string.Join(" ", args);
          }
          UniversalInputManager.Get().SetInputText(funcAndArgs + " " + str, true);
        }
      }
    }
    else
    {
      bool flag = false;
      if (Input.GetKeyDown(KeyCode.None) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.End) || Input.GetKeyDown(KeyCode.Insert) || Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageDown) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.CapsLock) || Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightWindows) || Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand) || Input.GetKeyDown(KeyCode.Menu))
        flag = true;
      if (flag)
      {
        if (this.m_autofillMatchIndex != -1 || this.m_lastAutofillParamPrefix != null)
          UIStatus.Get().AddInfo("", 0.0f);
        this.m_cheatTextBeforeAutofill = (string) null;
        this.m_autofillMatchIndex = -1;
        this.m_lastAutofillParamFunc = (string) null;
        this.m_lastAutofillParamPrefix = (string) null;
        this.m_lastAutofillParamMatch = (string) null;
      }
    }
    return false;
  }

  public void RegisterCategory(string cat)
  {
    cat = cat.ToLowerInvariant();
    int length;
    for (string str = cat; !string.IsNullOrEmpty(str); str = length > 0 ? str.Substring(0, length) : (string) null)
    {
      if (this.m_categoryList.IndexOf(str) < 0)
      {
        this.m_categoryList.Count<string>();
        this.m_categoryList.Add(str);
      }
      length = str.LastIndexOf(':');
    }
    this.m_lastRegisteredCategoryIndex = this.m_categoryList.IndexOf(cat);
  }

  public void DefaultCategory() => this.RegisterCategory("other");

  public void RegisterCheatHandler(
    string func,
    CheatMgr.ProcessCheatCallback callback,
    string desc = null,
    string argDesc = null,
    string exampleArgs = null)
  {
    this.RegisterCheatHandler_(func, (Delegate) callback);
    if (desc != null)
      this.m_cheatDesc[func] = desc;
    if (argDesc != null)
      this.m_cheatArgs[func] = argDesc;
    if (exampleArgs == null)
      return;
    this.m_cheatExamples[func] = exampleArgs;
  }

  public void RegisterCheatHandler(
    string func,
    CheatMgr.ProcessCheatAutofillCallback callback,
    string desc = null,
    string argDesc = null,
    string exampleArgs = null)
  {
    this.RegisterCheatHandler_(func, (Delegate) callback);
    if (desc != null)
      this.m_cheatDesc[func] = desc;
    if (argDesc != null)
      this.m_cheatArgs[func] = argDesc;
    if (exampleArgs == null)
      return;
    this.m_cheatExamples[func] = exampleArgs;
  }

  public void RegisterCheatAlias(string func, params string[] aliases)
  {
    if (!this.m_funcMap.ContainsKey(func))
    {
      Debug.LogError((object) string.Format("CheatMgr.RegisterCheatAlias() - cannot register aliases for func {0} because it does not exist", (object) func));
    }
    else
    {
      foreach (string alias in aliases)
        this.m_cheatAlias[alias] = func;
    }
  }

  public void UnregisterCheatHandler(string func, CheatMgr.ProcessCheatCallback callback) => this.UnregisterCheatHandler_(func, (Delegate) callback);

  public string GetCheatCategory(string cheat)
  {
    int index;
    return this.m_cheatCategoryIndex.TryGetValue(cheat, out index) && index >= 0 ? this.m_categoryList[index] : "other";
  }

  private void RegisterCheatHandler_(string func, Delegate callback)
  {
    if (string.IsNullOrEmpty(func.Trim()))
    {
      Debug.LogError((object) "CheatMgr.RegisterCheatHandler() - FAILED to register a null, empty, or all-whitespace function name");
    }
    else
    {
      List<Delegate> delegateList;
      if (this.m_funcMap.TryGetValue(func, out delegateList))
      {
        if (!delegateList.Contains(callback))
          delegateList.Add(callback);
      }
      else
      {
        delegateList = new List<Delegate>();
        this.m_funcMap.Add(func, delegateList);
        delegateList.Add(callback);
      }
      this.m_cheatCategoryIndex[func] = this.m_lastRegisteredCategoryIndex;
    }
  }

  private void UnregisterCheatHandler_(string func, Delegate callback)
  {
    List<Delegate> delegateList;
    if (!this.m_funcMap.TryGetValue(func, out delegateList))
      return;
    delegateList.Remove(callback);
  }

  private void OnInputComplete(string inputCommand)
  {
    inputCommand = inputCommand.TrimStart();
    if (string.IsNullOrEmpty(inputCommand))
      return;
    this.m_cheatTextBeforeAutofill = (string) null;
    this.m_autofillMatchIndex = -1;
    string message = this.ProcessCheat(inputCommand);
    if (string.IsNullOrEmpty(message))
      return;
    UIStatus.Get().AddError(message, 4f);
  }

  private string ParseFuncAndArgs(string inputCommand, out string[] args, out string rawArgs)
  {
    rawArgs = (string) null;
    args = (string[]) null;
    string func = this.ExtractFunc(inputCommand);
    if (func == null)
      return (string) null;
    int length = func.Length;
    if (length == inputCommand.Length)
    {
      rawArgs = "";
      args = new string[1];
      args[0] = "";
    }
    else
    {
      rawArgs = inputCommand.Remove(0, length + 1);
      MatchCollection matchCollection = Regex.Matches(rawArgs, "\\S+");
      if (matchCollection.Count == 0)
      {
        args = new string[1];
        args[0] = "";
      }
      else
      {
        args = new string[matchCollection.Count];
        for (int i = 0; i < matchCollection.Count; ++i)
          args[i] = matchCollection[i].Value;
      }
    }
    return func;
  }

  public string RunCheatInternally(string inputCommand)
  {
    string[] args;
    string rawArgs;
    string funcAndArgs = this.ParseFuncAndArgs(inputCommand, out args, out rawArgs);
    return funcAndArgs == null ? "\"" + inputCommand.Split(' ')[0] + "\" cheat command not found!" : (!this.CallCheatCallback(funcAndArgs, args, rawArgs, false, false) ? "\"" + funcAndArgs + "\" cheat command executed, but failed!" : (string) null);
  }

  public string ProcessCheat(string inputCommand, bool doNotSaveToHistory = false)
  {
    if (!doNotSaveToHistory)
    {
      if (this.m_cheatHistory.Count < 1 || !this.m_cheatHistory[0].Equals(inputCommand))
      {
        this.m_cheatHistory.Remove(inputCommand);
        this.m_cheatHistory.Insert(0, inputCommand);
      }
      if (this.m_cheatHistory.Count > 25)
        this.m_cheatHistory.RemoveRange(24, this.m_cheatHistory.Count - 25);
      this.m_cheatHistoryIndex = -1;
      this.m_cheatTextBeforeScrollingThruHistory = (string) null;
      this.WriteCheatHistoryOption();
    }
    string[] args;
    string rawArgs;
    string funcAndArgs = this.ParseFuncAndArgs(inputCommand, out args, out rawArgs);
    if (funcAndArgs == null)
      return "\"" + inputCommand.Split(' ')[0] + "\" cheat command not found!";
    UIStatus.Get().AddInfo("", 0.0f);
    return !this.CallCheatCallback(funcAndArgs, args, rawArgs, false, false) ? "\"" + funcAndArgs + "\" cheat command executed, but failed!" : (string) null;
  }

  private bool CallCheatCallback(
    string func,
    string[] args,
    string rawArgs,
    bool isAutofill,
    bool isShiftTab)
  {
    List<Delegate> func1 = this.m_funcMap[this.GetOriginalFunc(func)];
    bool flag = false;
    for (int index = 0; index < func1.Count; ++index)
    {
      Delegate @delegate = func1[index];
      switch (@delegate)
      {
        case CheatMgr.ProcessCheatCallback _ when !isAutofill:
          flag = ((CheatMgr.ProcessCheatCallback) @delegate)(func, args, rawArgs) | flag;
          break;
        case CheatMgr.ProcessCheatAutofillCallback _:
          if (isAutofill && func != this.m_lastAutofillParamFunc)
            this.m_lastAutofillParamMatch = (string) null;
          CheatMgr.ProcessCheatAutofillCallback autofillCallback = (CheatMgr.ProcessCheatAutofillCallback) @delegate;
          AutofillData autofillData1 = (AutofillData) null;
          if (isAutofill)
          {
            autofillData1 = new AutofillData();
            autofillData1.m_isShiftTab = isShiftTab;
            autofillData1.m_lastAutofillParamPrefix = this.m_lastAutofillParamPrefix;
            autofillData1.m_lastAutofillParamMatch = this.m_lastAutofillParamMatch;
          }
          string func2 = func;
          string[] args1 = args;
          string rawArgs1 = rawArgs;
          AutofillData autofillData2 = autofillData1;
          flag = autofillCallback(func2, args1, rawArgs1, autofillData2) | flag;
          if (isAutofill & flag)
          {
            this.m_lastAutofillParamFunc = func;
            this.m_lastAutofillParamPrefix = autofillData1.m_lastAutofillParamPrefix;
            this.m_lastAutofillParamMatch = autofillData1.m_lastAutofillParamMatch;
            break;
          }
          break;
      }
    }
    return flag;
  }

  private string ExtractFunc(string inputCommand)
  {
    inputCommand = inputCommand.TrimStart('/');
    inputCommand = inputCommand.Trim();
    int index1 = 0;
    List<string> funcs = new List<string>();
    foreach (string key in this.m_funcMap.Keys)
    {
      funcs.Add(key);
      if (key.Length > funcs[index1].Length)
        index1 = funcs.Count - 1;
    }
    foreach (string key in this.m_cheatAlias.Keys)
    {
      funcs.Add(key);
      if (key.Length > funcs[index1].Length)
        index1 = funcs.Count - 1;
    }
    int index2;
    for (index2 = 0; index2 < inputCommand.Length; ++index2)
    {
      char c = inputCommand[index2];
      int index3 = 0;
      while (index3 < funcs.Count)
      {
        string func = funcs[index3];
        if (index2 == func.Length)
        {
          if (char.IsWhiteSpace(c))
            return func;
          funcs.RemoveAt(index3);
          if (index3 <= index1)
            index1 = this.ComputeLongestFuncIndex(funcs);
        }
        else if ((int) func[index2] != (int) c)
        {
          funcs.RemoveAt(index3);
          if (index3 <= index1)
            index1 = this.ComputeLongestFuncIndex(funcs);
        }
        else
          ++index3;
      }
      if (funcs.Count == 0)
        return (string) null;
    }
    if (funcs.Count > 1)
    {
      foreach (string func in funcs)
      {
        if (inputCommand == func)
          return func;
      }
      return (string) null;
    }
    string str = funcs[0];
    return index2 < str.Length ? (string) null : str;
  }

  private int ComputeLongestFuncIndex(List<string> funcs)
  {
    int index1 = 0;
    for (int index2 = 1; index2 < funcs.Count; ++index2)
    {
      if (funcs[index2].Length > funcs[index1].Length)
        index1 = index2;
    }
    return index1;
  }

  private string GetOriginalFunc(string func)
  {
    string originalFunc;
    if (!this.m_cheatAlias.TryGetValue(func, out originalFunc))
      originalFunc = func;
    return originalFunc;
  }

  public delegate bool ProcessCheatCallback(string func, string[] args, string rawArgs);

  public delegate bool ProcessCheatAutofillCallback(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData);
}
