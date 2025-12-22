using System;
using System.Collections.Generic;
using UnityEngine;

public class Column : Rectangle
{
  private List<Rectangle> rows;

  private int heightAvailable
  {
    get
    {
      if (this.used)
        return 0;
      if (this.rows == null)
        return this.position.height;
      int height = this.position.height;
      foreach (Rectangle row in this.rows)
        height -= row.position.height;
      return height;
    }
  }

  public Column(int x, int y, int w, int h)
    : base(x, y, w, h)
  {
  }

  public override Rectangle Insert(int w, int h)
  {
    if (this.used)
      return (Rectangle) null;
    if (w > this.position.width)
      return (Rectangle) null;
    if (this.rows == null && this.position.width == w && this.position.height == h)
    {
      this.used = true;
      return (Rectangle) this;
    }
    if (this.rows == null)
      this.rows = new List<Rectangle>();
    foreach (Rectangle row in this.rows)
    {
      Rectangle rectangle = row.Insert(w, h);
      if (rectangle != null)
        return rectangle;
    }
    if (h > this.heightAvailable)
      return (Rectangle) null;
    Row row1 = new Row(this.position.x, this.position.y + this.position.height - this.heightAvailable, this.position.width, h);
    this.rows.Add((Rectangle) row1);
    this.rows.Sort((Comparison<Rectangle>) ((lhs, rhs) => lhs.position.height.CompareTo(rhs.position.height)));
    return row1.Insert(w, h);
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
    if (this.rows != null)
    {
      foreach (Rectangle row in this.rows)
      {
        if (row.Remove(rect))
          return true;
      }
    }
    return false;
  }
}
