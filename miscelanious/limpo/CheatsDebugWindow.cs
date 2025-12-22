using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheatsDebugWindow : DebuggerGuiWindow
{
  private Vector2 m_GUISize;
  private CheatsDebugWindow.CheatCommand m_currentlyDisplayedCheat;
  private string m_cheatSearchTerm = "";
  private Vector2 m_cheatScrollPosition;
  private Dictionary<string, CheatsDebugWindow.CheatCategory> m_categories;
  private GUIStyle m_labelStyle;

  public CheatsDebugWindow(Vector2 guiSize)
    : base("Cheats", (DebuggerGui.LayoutGui) null)
  {
    this.m_GUISize = guiSize;
    this.m_OnGui = new DebuggerGui.LayoutGui(this.LayoutCheats);
    this.m_labelStyle = new GUIStyle((GUIStyle) "box")
    {
      alignment = TextAnchor.MiddleLeft,
      wordWrap = false,
      clipping = TextClipping.Clip,
      stretchWidth = false
    };
    Rect scaledScreen = this.GetScaledScreen();
    this.ResizeToFit(scaledScreen.width / 2f, scaledScreen.height / 2f);
  }

  private void InitializeCheatsAsNecessary()
  {
    if (this.m_categories != null)
      return;
    CheatMgr cheatMgr = CheatMgr.Get();
    Options options = Options.Get();
    if (cheatMgr == null || options == null || cheatMgr.GetCheatCommands().Count<string>() == 0)
      return;
    this.m_categories = new Dictionary<string, CheatsDebugWindow.CheatCategory>();
    CheatsDebugWindow.CheatCategory cheatCategory1 = new CheatsDebugWindow.CheatCategory("options");
    this.m_categories.Add(cheatCategory1.Path, cheatCategory1);
    foreach (KeyValuePair<Option, string> clientOption in options.GetClientOptions())
    {
      string title = clientOption.Value;
      Option key = clientOption.Key;
      string str1 = options.GetOptionType(key).ToString();
      if (str1.StartsWith("System."))
        str1 = str1.Remove(0, 7);
      string str2 = cheatCategory1.Path + ":" + str1;
      CheatsDebugWindow.CheatCategory cheatCategory2 = (CheatsDebugWindow.CheatCategory) null;
      if (!this.m_categories.TryGetValue(str2, out cheatCategory2))
      {
        cheatCategory2 = new CheatsDebugWindow.CheatCategory(str2);
        this.m_categories.Add(str2, cheatCategory2);
      }
      int num = (int) key;
      cheatCategory2.children.Add((CheatsDebugWindow.CheatEntry) new CheatsDebugWindow.CheatOption(title, (Option) num)
      {
        parent = cheatCategory2
      });
    }
    foreach (string cheatCommand in cheatMgr.GetCheatCommands())
    {
      string cheatCategory3 = cheatMgr.GetCheatCategory(cheatCommand);
      CheatsDebugWindow.CheatCategory cheatCategory4 = (CheatsDebugWindow.CheatCategory) null;
      if (!this.m_categories.TryGetValue(cheatCategory3, out cheatCategory4) || cheatCategory4 == null)
      {
        cheatCategory4 = new CheatsDebugWindow.CheatCategory(cheatCategory3);
        this.m_categories.Add(cheatCategory3, cheatCategory4);
      }
      cheatCategory4.children.Add((CheatsDebugWindow.CheatEntry) new CheatsDebugWindow.CheatCommand(cheatCommand)
      {
        parent = cheatCategory4
      });
    }
  }

  private Rect LayoutCheats(Rect space)
  {
    this.InitializeCheatsAsNecessary();
    if (this.m_categories == null)
      return space;
    if (this.m_currentlyDisplayedCheat == null)
    {
      space = this.LayoutFilteredCheats(space);
    }
    else
    {
      if (GUI.Button(new Rect(space.x, space.y, this.m_GUISize.x, this.m_GUISize.y), "Back"))
      {
        this.m_currentlyDisplayedCheat = (CheatsDebugWindow.CheatCommand) null;
        return space;
      }
      if (GUI.Button(new Rect(space.xMax - this.m_GUISize.x, space.y, this.m_GUISize.x, this.m_GUISize.y), "Hide Console"))
      {
        CheatMgr.Get().HideConsole();
        return space;
      }
      space.yMin += this.m_GUISize.y;
      string title = this.m_currentlyDisplayedCheat.Title;
      if (!string.IsNullOrEmpty(this.m_currentlyDisplayedCheat.args))
        title += string.Format(" {0}", (object) this.m_currentlyDisplayedCheat.args);
      GUI.Box(new Rect(space.xMin, space.yMin, space.width, this.m_GUISize.y), title, this.m_labelStyle);
      space.yMin += 1.1f * this.m_GUISize.y;
      if (!string.IsNullOrEmpty(this.m_currentlyDisplayedCheat.description))
      {
        GUI.Box(new Rect(space.xMin, space.yMin, space.width, this.m_GUISize.y), this.m_currentlyDisplayedCheat.description, this.m_labelStyle);
        space.yMin += 1.1f * this.m_GUISize.y;
      }
      if (!string.IsNullOrEmpty(this.m_currentlyDisplayedCheat.example))
      {
        GUI.Box(new Rect(space.xMin, space.yMin, space.width, this.m_GUISize.y), string.Format("Example: {0}", (object) this.m_currentlyDisplayedCheat.example), this.m_labelStyle);
        space.yMin += 1.1f * this.m_GUISize.y;
      }
      GUI.Label(new Rect(space.min, this.m_GUISize), "History:");
      space.yMin += this.m_GUISize.y;
      string str1 = Options.Get().GetOption(Option.CHEAT_HISTORY).ToString();
      string str2 = ";" + this.m_currentlyDisplayedCheat.Title;
      for (int startIndex1 = str1.IndexOf(this.m_currentlyDisplayedCheat.Title); startIndex1 >= 0; startIndex1 = str1.IndexOf(str2, startIndex1 + str2.Length))
      {
        int startIndex2 = str1.IndexOf(this.m_currentlyDisplayedCheat.Title, startIndex1);
        int num = str1.IndexOf(';', startIndex2);
        string text = num <= 0 ? str1.Substring(startIndex2) : str1.Substring(startIndex2, num - startIndex2);
        if (GUI.Button(new Rect(space.xMin, space.yMin, space.width, this.m_GUISize.y), text, this.m_labelStyle))
        {
          CheatMgr.Get().ShowConsole();
          UniversalInputManager.Get().SetInputText(text, true);
        }
        space.yMin += this.m_GUISize.y;
      }
    }
    return space;
  }

  private Rect LayoutFilteredCheats(Rect space)
  {
    float y1 = this.m_GUISize.y;
    GUI.Label(new Rect(space.xMin + 10f, space.yMin + 5f, y1 * 2f, y1), "Filter:");
    this.m_cheatSearchTerm = GUI.TextField(new Rect(space.xMin + y1 * 2f, space.yMin, 0.0f, this.m_GUISize.y)
    {
      xMax = space.xMax
    }, this.m_cheatSearchTerm);
    space.yMin += y1;
    List<CheatsDebugWindow.CheatEntry> cheatEntryList = this.CollectCheats(this.m_cheatSearchTerm);
    Rect position1 = space;
    float num1 = (float) ((PlatformSettings.IsMobile() ? 20.0 : 10.0) - 5.0);
    position1.xMin += num1;
    position1.xMax -= num1;
    position1.yMax -= num1;
    Rect viewRect = new Rect(0.0f, 0.0f, position1.width - 18f, (float) cheatEntryList.Count * y1);
    this.m_cheatScrollPosition = GUI.BeginScrollView(position1, this.m_cheatScrollPosition, viewRect, false, true);
    float y2 = 0.0f;
    foreach (CheatsDebugWindow.CheatEntry cheatEntry in cheatEntryList)
    {
      Rect position2 = new Rect(0.0f, y2, viewRect.width, y1);
      if (cheatEntry is CheatsDebugWindow.CheatCategory)
      {
        CheatsDebugWindow.CheatCategory cheatCategory = cheatEntry as CheatsDebugWindow.CheatCategory;
        position2.xMin += (float) cheatCategory.Depth * 15f;
        GUI.Label(position2, cheatCategory.Title);
      }
      else
      {
        int num2 = 0;
        string text1 = "";
        string text2 = "";
        Action action = (Action) null;
        if (cheatEntry is CheatsDebugWindow.CheatOption)
        {
          CheatsDebugWindow.CheatOption cheatOption = cheatEntry as CheatsDebugWindow.CheatOption;
          string optionName = cheatOption.Title;
          object obj = (object) null;
          Option option = cheatOption.option;
          OptionDataTables.s_defaultsMap.TryGetValue(option, out obj);
          num2 = cheatOption.parent.Depth + 1;
          if (Options.Get().GetOptionType(option) == typeof (bool))
          {
            text1 = string.Format("{0}={1}", (object) optionName, Options.Get().GetBool(option) ? (object) "1" : (object) "0");
            action = (Action) (() => Options.Get().SetBool(option, !Options.Get().GetBool(option)));
          }
          else
          {
            text1 = optionName;
            action = (Action) (() =>
            {
              CheatMgr.Get().ShowConsole();
              UniversalInputManager.Get().SetInputText(string.Format("set {0} ", (object) optionName), true);
            });
          }
          object option1 = Options.Get().GetOption(option);
          text2 = string.Format("={0}", option1);
          if (obj != null)
            text2 += string.Format(" (default={1})", option1, obj);
        }
        else if (cheatEntry is CheatsDebugWindow.CheatCommand)
        {
          CheatsDebugWindow.CheatCommand command = cheatEntry as CheatsDebugWindow.CheatCommand;
          num2 = command.parent.Depth + 1;
          text1 = command.Title;
          action = (Action) (() =>
          {
            CheatMgr.Get().ShowConsole();
            UniversalInputManager.Get().SetInputText(command.Title, true);
            this.m_currentlyDisplayedCheat = command;
          });
          text2 = command.description;
        }
        position2.xMin += (float) num2 * 15f;
        Rect position3 = new Rect(position2.xMin, position2.yMin, 200f, y1);
        if (GUI.Button(position3, text1) && action != null)
          action();
        if (!string.IsNullOrEmpty(text2))
          GUI.Box(new Rect(position3.xMax, position2.yMin, position2.xMax - position3.xMax, y1), text2, this.m_labelStyle);
      }
      y2 += y1;
    }
    GUI.EndScrollView();
    space.yMin = position1.yMax;
    return space;
  }

  private List<CheatsDebugWindow.CheatEntry> CollectCheats(string filter)
  {
    List<string> list = this.m_categories.Keys.ToList<string>();
    list.Sort();
    List<CheatsDebugWindow.CheatEntry> cheatEntryList1 = new List<CheatsDebugWindow.CheatEntry>();
    string[] terms = filter.ToLowerInvariant().Split(' ');
    foreach (string key1 in list)
    {
      CheatsDebugWindow.CheatCategory category = this.m_categories[key1];
      bool flag = false;
      List<CheatsDebugWindow.CheatEntry> cheatEntryList2 = new List<CheatsDebugWindow.CheatEntry>();
      if (this.CheatMatchesFilter((CheatsDebugWindow.CheatEntry) category, terms))
      {
        flag = true;
        cheatEntryList2.AddRange((IEnumerable<CheatsDebugWindow.CheatEntry>) category.children);
      }
      else
      {
        foreach (CheatsDebugWindow.CheatEntry child in category.children)
        {
          if (this.CheatMatchesFilter(child, terms))
          {
            flag = true;
            cheatEntryList2.Add(child);
          }
        }
      }
      if (flag)
      {
        foreach (string key2 in CheatsDebugWindow.CheatCategory.GetLineage(category.Path))
        {
          CheatsDebugWindow.CheatCategory cheatCategory = (CheatsDebugWindow.CheatCategory) null;
          if (this.m_categories.TryGetValue(key2, out cheatCategory) && !cheatEntryList1.Contains((CheatsDebugWindow.CheatEntry) cheatCategory))
            cheatEntryList1.Add((CheatsDebugWindow.CheatEntry) cheatCategory);
        }
      }
      foreach (CheatsDebugWindow.CheatEntry cheatEntry in cheatEntryList2)
        cheatEntryList1.Add(cheatEntry);
    }
    return cheatEntryList1;
  }

  private bool CheatMatchesFilter(CheatsDebugWindow.CheatEntry cheat, string[] terms)
  {
    if (((IEnumerable<string>) terms).Count<string>() == 0)
      return true;
    string searchString = cheat?.SearchString;
    if (string.IsNullOrEmpty(searchString))
      return false;
    foreach (string term in terms)
    {
      if (!searchString.Contains(term))
        return false;
    }
    return true;
  }

  private class CheatEntry
  {
    public CheatEntry(string title) => this.Title = title;

    public virtual string SearchString => this.Title.ToLowerInvariant();

    public string Title { get; protected set; }
  }

  private class CheatCategory : CheatsDebugWindow.CheatEntry
  {
    public List<CheatsDebugWindow.CheatEntry> children = new List<CheatsDebugWindow.CheatEntry>();

    public CheatCategory(string path)
      : base("")
    {
      this.Path = path;
      int num = path.LastIndexOf(':');
      this.Title = num > 0 ? path.Substring(num + 1) : path;
    }

    public override string SearchString => this.Path.ToLowerInvariant();

    public string Path { get; protected set; }

    public int Depth
    {
      get
      {
        int depth = 0;
        for (int index = this.Path.IndexOf(':'); index > 0 && index < this.Path.Length; index = this.Path.IndexOf(':', index + 1))
          ++depth;
        return depth;
      }
    }

    public static List<string> GetLineage(string fullPath)
    {
      List<string> lineage = new List<string>();
      for (int length = fullPath.IndexOf(':'); length > 0; length = fullPath.IndexOf(':', length + 1))
        lineage.Add(fullPath.Substring(0, length));
      lineage.Add(fullPath);
      return lineage;
    }
  }

  private class CheatCommand : CheatsDebugWindow.CheatEntry
  {
    public string example = "";
    public string description = "";
    public string args = "";
    public CheatsDebugWindow.CheatCategory parent;

    public CheatCommand(string name)
      : base(name)
    {
      CheatMgr cheatMgr = CheatMgr.Get();
      if (cheatMgr == null)
        return;
      cheatMgr.cheatArgs.TryGetValue(name, out this.args);
      cheatMgr.cheatDesc.TryGetValue(name, out this.description);
      cheatMgr.cheatExamples.TryGetValue(name, out this.example);
    }

    public override string SearchString => (this.Title + " " + this.description).ToLowerInvariant();
  }

  private class CheatOption : CheatsDebugWindow.CheatEntry
  {
    public Option option;
    public CheatsDebugWindow.CheatCategory parent;

    public CheatOption(string title, Option option)
      : base(title)
    {
      this.option = option;
    }
  }
}
