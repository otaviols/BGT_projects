using UnityEngine;

public class GreedyPacker
{
  private Rectangle root;

  public GreedyPacker(int w, int h) => this.root = (Rectangle) new Column(0, 0, w, h);

  public RectInt Insert(int w, int h)
  {
    Rectangle rectangle = this.root.Insert(w, h);
    return rectangle == null ? new RectInt(-1, -1, -1, -1) : rectangle.position;
  }

  public void Remove(RectInt pos) => this.root.Remove(pos);
}
