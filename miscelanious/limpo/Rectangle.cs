using UnityEngine;

public abstract class Rectangle
{
  public RectInt position;
  public bool used;

  public Rectangle(int x, int y, int w, int h) => this.position = new RectInt(x, y, w, h);

  public bool Contains(RectInt rect) => this.position.x <= rect.x && this.position.y <= rect.y && this.position.xMax >= rect.xMax && this.position.yMax >= rect.yMax;

  public abstract Rectangle Insert(int w, int h);

  public abstract bool Remove(RectInt rect);
}
