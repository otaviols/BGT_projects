using System.Collections.Generic;
using UnityEngine;

public class Row : Rectangle
{
  private List<Rectangle> columns;

  private int widthAvailable
  {
    get
    {
      if (this.used)
        return 0;
      if (this.columns == null)
        return this.position.width;
      int width = this.position.width;
      foreach (Rectangle column in this.columns)
        width -= column.position.width;
      return width;
    }
  }

  public Row(int x, int y, int w, int h)
    : base(x, y, w, h)
  {
  }

  public override Rectangle Insert(int w, int h)
  {
    if (this.used)
      return (Rectangle) null;
    if (h > this.position.height)
      return (Rectangle) null;
    if (this.columns == null && w == this.position.width && h == this.position.height)
    {
      this.used = true;
      return (Rectangle) this;
    }
    if (this.columns == null)
      this.columns = new List<Rectangle>();
    foreach (Rectangle column in this.columns)
    {
      Rectangle rectangle = column.Insert(w, h);
      if (rectangle != null)
        return rectangle;
    }
    if (h > this.widthAvailable)
      return (Rectangle) null;
    Column column1 = new Column(this.position.x + this.position.width - this.widthAvailable, this.position.y, w, this.position.height);
    this.columns.Add((Rectangle) column1);
    return column1.Insert(w, h);
  }

  public override bool Remove(RectInt rect)
  {
    if (!this.Contains(rect))
      return false;
    if (this.used && this.position.Equals(rect))
    {
      this.used = false;
      return true;
    }
    if (this.columns != null)
    {
      foreach (Rectangle column in this.columns)
      {
        if (column.Remove(rect))
          return true;
      }
    }
    return false;
  }
}
