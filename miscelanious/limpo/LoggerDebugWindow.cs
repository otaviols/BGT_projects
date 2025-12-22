using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LoggerDebugWindow : DebuggerGuiWindow
{
  public DebuggerGui.LayoutGui CustomLayout;
  internal new static string SERIAL_ID = "[LOG]";
  private Vector2 m_GUISize;
  private QueueList<LoggerDebugWindow.LogEntry> m_entries;
  private Dictionary<Blizzard.T5.Logging.LogLevel, int> m_levels;
  private string m_title;
  private GUIStyle m_textStyle;
  private List<object> m_categories;
  private Dictionary<object, bool> m_categoryToggles;
  private Vector2 m_scrollPosition;
  private bool m_autoScroll = true;
  private int m_alertCount;

  public LoggerDebugWindow(string title, Vector2 guiSize, IEnumerable<object> categories)
    : base(title, (DebuggerGui.LayoutGui) null)
  {
    this.m_title = title;
    this.m_categories = categories.ToList<object>();
    this.m_GUISize = guiSize;
    this.m_OnGui = new DebuggerGui.LayoutGui(this.LayoutMessages);
    this.m_categoryToggles = new Dictionary<object, bool>();
    this.m_entries = new QueueList<LoggerDebugWindow.LogEntry>();
    this.m_levels = new Dictionary<Blizzard.T5.Logging.LogLevel, int>();
    this.m_textStyle = new GUIStyle((GUIStyle) "box")
    {
      fontSize = 17,
      alignment = TextAnchor.UpperLeft,
      wordWrap = true,
      clipping = TextClipping.Clip,
      stretchWidth = true
    };
  }

  public void AddEntry(LoggerDebugWindow.LogEntry entry, bool addAlert = false)
  {
    if (entry.text.Length > 2100)
      entry.text = entry.text.Substring(0, 2100);
    this.m_entries.Enqueue(entry);
    if (this.m_levels.ContainsKey(entry.category))
      this.m_levels[entry.category]++;
    else
      this.m_levels.Add(entry.category, 1);
    if (!addAlert || !this.AreLogsDisplayed((object) entry.category))
      return;
    ++this.m_alertCount;
    if (this.m_alertCount != 1)
      return;
    this.IsShown = true;
    this.IsExpanded = true;
  }

  public void Clear()
  {
    this.m_entries.Clear();
    this.m_levels.Clear();
    this.m_alertCount = 0;
  }

  public int GetCount(Blizzard.T5.Logging.LogLevel category)
  {
    int count;
    this.m_levels.TryGetValue(category, out count);
    return count;
  }

  public IEnumerable<LoggerDebugWindow.LogEntry> GetEntries() => (IEnumerable<LoggerDebugWindow.LogEntry>) this.m_entries;

  public string FilterString { get; set; }

  public void ToggleLogsDisplay(object category, bool display)
  {
    this.m_categoryToggles[category] = display;
    this.InvokeOnChanged();
  }

  public bool AreLogsDisplayed(object category)
  {
    bool flag;
    if (category == null || !this.m_categoryToggles.TryGetValue(category, out flag))
      flag = true;
    return flag;
  }

  public Rect LayoutLog(Rect space)
  {
    GUI.skin.settings.selectionColor = Color.blue;
    Rect position1 = space;
    Rect viewRect = new Rect(0.0f, 0.0f, space.width - 20f, 0.0f);
    string[] strArray;
    if (!string.IsNullOrEmpty(this.FilterString))
      strArray = this.FilterString.ToLowerInvariant().Split(' ');
    else
      strArray = (string[]) null;
    string[] source = strArray;
    List<int> intList = new List<int>();
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < this.m_entries.Count; ++index)
    {
      if (this.AreLogsDisplayed((object) this.m_entries[index].category))
      {
        if (source != null && ((IEnumerable<string>) source).Count<string>() > 0)
        {
          bool flag = true;
          string lowerInvariant = this.m_entries[index].text.ToLowerInvariant();
          foreach (string str in source)
          {
            if (!lowerInvariant.ToLowerInvariant().Contains(str))
            {
              flag = false;
              break;
            }
          }
          if (!flag)
            continue;
        }
        intList.Add(index);
        viewRect.height += this.GetLogEntryHeight(this.m_entries[index], viewRect.width);
      }
    }
    float num = viewRect.height - position1.height;
    if (this.m_autoScroll)
      this.m_scrollPosition.y = num;
    this.m_scrollPosition = GUI.BeginScrollView(position1, this.m_scrollPosition, viewRect, false, true);
    this.m_autoScroll = (double) this.m_scrollPosition.y >= (double) num;
    Rect position2 = new Rect(0.0f, 0.0f, viewRect.width - 60f, 0.0f);
    for (int index = 0; index < intList.Count; ++index)
    {
      LoggerDebugWindow.LogEntry entry = this.m_entries.ElementAtOrDefault<LoggerDebugWindow.LogEntry>(intList[index]);
      position2.height = this.GetLogEntryHeight(entry, viewRect.width);
      GUI.TextArea(position2, entry.text, this.m_textStyle);
      if (GUI.Button(new Rect(viewRect.width - 55f, position2.y, 55f, position2.height), "COPY"))
        GUIUtility.systemCopyBuffer = entry.text;
      position2.yMin += position2.height;
    }
    GUI.EndScrollView();
    space.yMin = space.yMax;
    return space;
  }

  private Rect LayoutMessages(Rect space) => this.CustomLayout != null ? this.CustomLayout(space) : this.LayoutLog(space);

  private float GetLogEntryHeight(LoggerDebugWindow.LogEntry entry, float width) => this.m_textStyle.CalcHeight(new GUIContent(entry.text), width) + 5f;

  internal override string SerializeToString()
  {
    string serialId = LoggerDebugWindow.SERIAL_ID;
    List<string> stringList = new List<string>();
    foreach (object category in this.m_categories)
      stringList.Add(this.AreLogsDisplayed(category) ? "1" : "0");
    return serialId + string.Join(",", stringList.ToArray()) + base.SerializeToString();
  }

  internal override void DeserializeFromString(string str)
  {
    int num = str.IndexOf(LoggerDebugWindow.SERIAL_ID);
    int startIndex1 = str.IndexOf(DebuggerGuiWindow.SERIAL_ID);
    if (num >= 0)
    {
      int startIndex2 = num + LoggerDebugWindow.SERIAL_ID.Length;
      List<string> list = ((IEnumerable<string>) str.Substring(startIndex2, startIndex1 > startIndex2 ? startIndex1 - startIndex2 : str.Length - startIndex2).Split(',')).ToList<string>();
      for (int index = 0; index < this.m_categories.Count; ++index)
      {
        string str1 = list.ElementAtOrDefault<string>(index);
        this.m_categoryToggles[this.m_categories[index]] = str1 != "0";
      }
    }
    if (startIndex1 < 0)
      return;
    base.DeserializeFromString(str.Substring(startIndex1));
  }

  public class LogEntry
  {
    public Blizzard.T5.Logging.LogLevel category;
    public string text;

    public override string ToString() => string.Format("[{0}] {1}", (object) this.category, (object) this.text);
  }
}
