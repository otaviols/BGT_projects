using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebuggerGuiWindow : DebuggerGui
{
  public float? collapsedWidth;
  protected Vector2 m_pos;
  protected Vector2 m_size;
  protected Rect m_window;
  protected int m_windowId;
  protected bool m_spaceIsDirty;
  protected bool m_canClose;
  protected bool m_canResize;
  protected Vector2 m_resizingSide;
  protected Vector3 m_resizeClickStart;
  protected Rect m_resizeInitialWindow;
  public const float PADDING = 5f;
  protected const float RESIZE_HANDLE_SIZE = 10f;
  protected const float MOBILE_RESIZE_HANDLE_SIZE = 20f;
  protected static readonly Vector2 SIZE_PADDING = new Vector2(10f, 34f);
  private const float MIN_WINDOW_WIDTH = 100f;
  private const float MIN_WINDOW_HEIGHT = 48f;
  private const string CLOSE_SYMBOL = "✕";
  internal new static string SERIAL_ID = "[W]";

  public DebuggerGuiWindow(
    string title,
    DebuggerGui.LayoutGui onGui,
    bool canClose = true,
    bool canResize = true)
    : base(title, onGui)
  {
    this.m_windowId = title.GetHashCode();
    this.m_canClose = canClose;
    this.m_canResize = canResize;
    this.OnChanged += new Action(this.HandleChange);
  }

  public Vector2 Position
  {
    get => this.m_pos;
    set
    {
      if (!(this.m_pos != value))
        return;
      this.m_pos = value;
      this.UpdateWindowSize();
      this.InvokeOnChanged();
    }
  }

  private Vector2 Size
  {
    get => this.m_size;
    set
    {
      Vector2 size = this.GetScaledScreen().size;
      value.x = Mathf.Clamp(value.x, 100f, size.x);
      value.y = Mathf.Clamp(value.y, 48f, size.y);
      if (!(this.m_size != value))
        return;
      this.m_size = value;
      this.UpdateWindowSize();
      this.InvokeOnChanged();
    }
  }

  public bool IsMouseOver() => this.m_window.Contains(this.GetScaledMouse());

  public void ResizeToFit(Vector2 dims) => this.Size = dims + DebuggerGuiWindow.SIZE_PADDING;

  public void ResizeToFit(float width, float height) => this.ResizeToFit(new Vector2(width, height));

  public Rect GetHeaderRect() => new Rect(5f, 5f, this.m_size.x - 10f, 24f);

  public void Layout() => this.Layout(new Rect(this.Position, this.Size));

  public override Rect Layout(Rect space)
  {
    if (!this.m_isShown)
      return space;
    this.Position = space.min;
    this.Size = space.size;
    this.UpdateWindowSize();
    this.m_window = GUI.Window(this.m_windowId, this.m_window, new GUI.WindowFunction(this.WindowFunction), "");
    this.ConstrainPosition(this.m_window);
    if (this.m_canResize && this.m_isExpanded)
    {
      Vector3 scaledMouse = this.GetScaledMouse();
      float num = PlatformSettings.IsMobile() ? 20f : 10f;
      Rect window = this.m_window;
      window.xMin += num;
      window.yMin += 5f;
      window.xMax -= num;
      window.yMax -= num;
      if (Input.GetMouseButtonDown(0) && this.m_window.Contains(scaledMouse) && !window.Contains(scaledMouse))
      {
        this.m_resizingSide.x = (float) (((double) scaledMouse.x >= (double) window.xMax ? 1 : 0) + ((double) scaledMouse.x <= (double) window.xMin ? -1 : 0));
        this.m_resizingSide.y = (float) (((double) scaledMouse.y >= (double) window.yMax ? 1 : 0) + ((double) scaledMouse.y <= (double) window.yMin ? -1 : 0));
        this.m_resizeClickStart = scaledMouse;
        this.m_resizeInitialWindow = this.m_window;
      }
      else if (this.IsResizing())
      {
        if (Input.GetMouseButton(0))
        {
          Vector2 vector2 = (Vector2) (scaledMouse - this.m_resizeClickStart);
          if ((double) this.m_resizingSide.x < 0.0)
            this.m_window.xMin = Mathf.Min(this.m_resizeInitialWindow.xMin + vector2.x, this.m_resizeInitialWindow.xMax - 100f);
          else if ((double) this.m_resizingSide.x > 0.0)
            this.m_window.xMax = Mathf.Max(this.m_resizeInitialWindow.xMax + vector2.x, this.m_resizeInitialWindow.xMin + 100f);
          if ((double) this.m_resizingSide.y < 0.0)
            this.m_window.yMin = Mathf.Min(this.m_resizeInitialWindow.yMin + vector2.y, this.m_resizeInitialWindow.yMax - 48f);
          else if ((double) this.m_resizingSide.y > 0.0)
            this.m_window.yMax = Mathf.Max(this.m_resizeInitialWindow.yMax + vector2.y, this.m_resizeInitialWindow.yMin + 48f);
          this.m_pos = this.m_window.min;
          this.m_size = this.m_window.size;
          this.InvokeOnChanged();
        }
        if (Input.GetMouseButtonUp(0))
          this.m_resizingSide = new Vector2(0.0f, 0.0f);
      }
    }
    return new Rect(this.m_window.xMin, this.m_window.yMax, space.width, space.height - this.m_window.height);
  }

  private void HandleChange()
  {
    Rect window = this.m_window;
    this.UpdateWindowSize();
    if (window.Equals(this.m_window))
      return;
    this.m_spaceIsDirty = true;
  }

  private void UpdateWindowSize() => this.m_window = new Rect(this.m_pos.x, this.m_pos.y, this.m_isExpanded || !this.collapsedWidth.HasValue ? this.m_size.x : this.collapsedWidth.Value, this.m_isExpanded ? this.m_size.y : 34f);

  private void WindowFunction(int windowId)
  {
    this.m_spaceIsDirty = false;
    Rect space = new Rect(5f, 5f, this.m_window.width - 10f, this.m_window.height - 10f);
    if (this.m_canClose && GUI.Button(new Rect(space.xMax - 24f, space.yMin, 24f, 24f), "✕"))
      this.IsShown = false;
    space = this.LayoutHeader(space);
    if (this.m_spaceIsDirty)
      return;
    space = this.LayoutInternal(space);
    if (this.IsResizing())
      return;
    GUI.DragWindow();
  }

  protected bool IsResizing() => (double) this.m_resizingSide.sqrMagnitude > 0.0;

  protected Rect GetScaledScreen() => new Rect(0.0f, 0.0f, Math.Max(0.0f, (float) Screen.width / GUI.matrix.lossyScale.x), Math.Max(0.0f, (float) Screen.height / GUI.matrix.lossyScale.y));

  protected Vector3 GetScaledMouse()
  {
    Vector3 mousePosition = Input.mousePosition;
    mousePosition.y = (float) Screen.height - mousePosition.y;
    mousePosition.x /= GUI.matrix.lossyScale.x;
    mousePosition.y /= GUI.matrix.lossyScale.y;
    return mousePosition;
  }

  private void ConstrainPosition(Rect window)
  {
    Vector2 vector2 = new Vector2(48f, 24f);
    Rect scaledScreen = this.GetScaledScreen();
    this.Position = new Vector2(Mathf.Clamp(window.x, scaledScreen.xMin - window.width + vector2.x, scaledScreen.xMax - vector2.x), Mathf.Clamp(window.y, scaledScreen.yMin - window.height + vector2.y, scaledScreen.yMax - vector2.y));
  }

  internal override string SerializeToString()
  {
    string serialId = DebuggerGuiWindow.SERIAL_ID;
    Vector2 position = this.Position;
    Vector2 size = this.Size;
    string str = string.Join("x", new List<string>()
    {
      Mathf.RoundToInt(position.x).ToString(),
      Mathf.RoundToInt(position.y).ToString(),
      Mathf.RoundToInt(size.x).ToString(),
      Mathf.RoundToInt(size.y).ToString()
    }.ToArray());
    return serialId + str + base.SerializeToString();
  }

  internal override void DeserializeFromString(string str)
  {
    int num = str.IndexOf(DebuggerGuiWindow.SERIAL_ID);
    int startIndex1 = str.IndexOf(DebuggerGui.SERIAL_ID);
    if (num >= 0)
    {
      int startIndex2 = num + DebuggerGuiWindow.SERIAL_ID.Length;
      List<string> source = new List<string>();
      string str1 = str.Substring(startIndex2, startIndex1 > startIndex2 ? startIndex1 - startIndex2 : str.Length - startIndex2);
      source.AddRange((IEnumerable<string>) str1.Split('x'));
      Vector2 position = this.Position;
      Vector2 size = this.Size;
      if (float.TryParse(source.ElementAtOrDefault<string>(0), out position.x) && float.TryParse(source.ElementAtOrDefault<string>(1), out position.y))
        this.Position = position;
      if (this.m_canResize && float.TryParse(source.ElementAtOrDefault<string>(2), out size.x) && float.TryParse(source.ElementAtOrDefault<string>(3), out size.y))
        this.Size = size;
      this.ConstrainPosition(this.m_window);
    }
    if (startIndex1 < 0)
      return;
    base.DeserializeFromString(str.Substring(startIndex1));
  }
}
