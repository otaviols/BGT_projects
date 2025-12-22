using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebuggerGui
{
  protected bool m_canCollapse = true;
  protected bool m_isShown = true;
  protected bool m_isExpanded = true;
  protected DebuggerGui.LayoutGui m_OnGui;
  public const float HEADER_SIZE = 24f;
  private const char DOWN_ARROW = '▼';
  private const char RIGHT_ARROW = '▶';
  internal static string SERIAL_ID = "[DG]";

  public event Action OnChanged;

  public DebuggerGui(string title, DebuggerGui.LayoutGui onGui, bool canCollapse = true, bool drawWindow = false)
  {
    this.Title = title;
    this.m_canCollapse = canCollapse;
    this.m_OnGui = onGui;
  }

  public string Title { get; set; }

  public bool IsExpanded
  {
    get => this.m_isExpanded;
    set
    {
      if (this.m_isExpanded == value)
        return;
      this.m_isExpanded = value;
      this.InvokeOnChanged();
    }
  }

  public bool IsShown
  {
    get => this.m_isShown;
    set
    {
      if (this.m_isShown == value)
        return;
      this.m_isShown = value;
      this.InvokeOnChanged();
    }
  }

  public virtual Rect Layout(Rect space)
  {
    if (!this.m_isShown)
      return space;
    space = this.LayoutHeader(space);
    return this.LayoutInternal(space);
  }

  protected Rect LayoutHeader(Rect space)
  {
    Rect rect = new Rect(space.x, space.y, space.width, 24f);
    if (this.m_canCollapse && GUI.Button(new Rect(rect.xMin, rect.yMin, rect.height, rect.height), this.m_isExpanded ? '▼'.ToString() : '▶'.ToString()))
      this.IsExpanded = !this.m_isExpanded;
    GUI.Label(new Rect((float) ((double) rect.xMin + (double) rect.height + 5.0), rect.yMin, (float) ((double) rect.width - (double) rect.height * 2.0 - 5.0), rect.height), this.Title);
    space.yMin += rect.height;
    return space;
  }

  protected Rect LayoutInternal(Rect space) => this.m_isExpanded && this.m_OnGui != null ? this.m_OnGui(space) : space;

  protected void InvokeOnChanged()
  {
    if (this.OnChanged == null)
      return;
    this.OnChanged();
  }

  internal virtual string SerializeToString() => DebuggerGui.SERIAL_ID + (this.IsShown ? "S" : "H") + (this.IsExpanded ? "E" : "C");

  internal virtual void DeserializeFromString(string str)
  {
    int num = str.IndexOf(DebuggerGui.SERIAL_ID);
    if (num < 0)
      return;
    int index = num + DebuggerGui.SERIAL_ID.Length;
    this.IsShown = str.ElementAtOrDefault<char>(index) == 'S';
    this.IsExpanded = str.ElementAtOrDefault<char>(index + 1) == 'E';
  }

  public static void SaveConfig(List<DebuggerGui> guis)
  {
    List<string> stringList = new List<string>();
    foreach (DebuggerGui gui in guis)
    {
      string str = gui.SerializeToString();
      stringList.Add(str);
    }
    string val = string.Join(";", stringList.ToArray());
    Options.Get().SetString(Option.HUD_CONFIG, val);
  }

  public static void LoadConfig(List<DebuggerGui> guis)
  {
    string str = Options.Get().GetString(Option.HUD_CONFIG);
    if (string.IsNullOrEmpty(str))
      return;
    List<string> stringList = new List<string>();
    stringList.AddRange((IEnumerable<string>) str.Split(';'));
    int index1 = 0;
    for (int index2 = Math.Min(guis.Count, stringList.Count); index1 < index2; ++index1)
      guis[index1].DeserializeFromString(stringList[index1]);
  }

  public delegate Rect LayoutGui(Rect space);
}
