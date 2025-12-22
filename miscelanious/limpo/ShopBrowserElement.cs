using Hearthstone.UI;
using Hearthstone.UI.Core;
using UnityEngine;

public class ShopBrowserElement : MonoBehaviour
{
  public Rect Bounds;
  protected bool m_isElementEnabled = true;
  public bool m_previewBounds;
  public bool m_previewOutline;
  public Color m_boundsColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
  public float Thickness = 0.1f;

  [Overridable]
  public float BoundsX
  {
    get => this.Bounds.x;
    set
    {
      this.Bounds.x = value;
      this.OnElementBoundsChanged();
    }
  }

  [Overridable]
  public float BoundsY
  {
    get => this.Bounds.y;
    set
    {
      this.Bounds.y = value;
      this.OnElementBoundsChanged();
    }
  }

  [Overridable]
  public float Width
  {
    get => this.Bounds.width;
    set
    {
      this.Bounds.width = value;
      this.OnElementBoundsChanged();
    }
  }

  [Overridable]
  public float Height
  {
    get => this.Bounds.height;
    set
    {
      this.Bounds.height = value;
      this.OnElementBoundsChanged();
    }
  }

  public float Top
  {
    get => this.GetDisplayTransform().localPosition.z + this.Bounds.yMax;
    set
    {
      Transform displayTransform = this.GetDisplayTransform();
      Vector3 localPosition = displayTransform.localPosition;
      displayTransform.localPosition = new Vector3(localPosition.x, localPosition.y, value - this.Bounds.yMax);
    }
  }

  public float Left
  {
    get => this.GetDisplayTransform().localPosition.x + this.Bounds.xMin;
    set
    {
      Transform displayTransform = this.GetDisplayTransform();
      Vector3 localPosition = displayTransform.localPosition;
      displayTransform.localPosition = new Vector3(value - this.Bounds.xMin, localPosition.y, localPosition.z);
    }
  }

  public float Right
  {
    get => this.GetDisplayTransform().localPosition.x + this.Bounds.xMax;
    set
    {
      Transform displayTransform = this.GetDisplayTransform();
      Vector3 localPosition = displayTransform.localPosition;
      displayTransform.localPosition = new Vector3(value - this.Bounds.xMax, localPosition.y, localPosition.z);
    }
  }

  public float Bottom
  {
    get => this.GetDisplayTransform().localPosition.z + this.Bounds.yMin;
    set
    {
      Transform displayTransform = this.GetDisplayTransform();
      Vector3 localPosition = displayTransform.localPosition;
      displayTransform.localPosition = new Vector3(localPosition.x, value - this.Bounds.yMin, localPosition.z);
    }
  }

  [Overridable]
  public bool IsElementEnabled
  {
    get => this.m_isElementEnabled;
    set
    {
      this.m_isElementEnabled = value;
      this.OnElementEnabled();
    }
  }

  protected virtual void OnElementBoundsChanged()
  {
  }

  protected virtual void OnElementEnabled()
  {
  }

  protected Transform GetDisplayTransform() => !((Object) this.GetComponent<WidgetTemplate>() != (Object) null) ? this.transform : this.transform.parent;

  public static int ComparePosition(ShopBrowserElement A, ShopBrowserElement B)
  {
    if (Mathf.Approximately(A.Top, B.Top))
    {
      if (Mathf.Approximately(A.Left, B.Left))
        return 0;
      return (double) A.Left >= (double) B.Left ? 1 : -1;
    }
    return (double) A.Top <= (double) B.Top ? 1 : -1;
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.isActiveAndEnabled)
      return;
    this.DrawRegion(this.m_boundsColor);
  }

  private void OnDrawGizmos()
  {
    if (!this.isActiveAndEnabled && !this.m_previewBounds)
      return;
    this.DrawRegion(this.m_boundsColor);
  }

  protected void DrawRegion(Color color, float padding = 0.0f)
  {
    Vector3 size = new Vector3(this.Width, this.Thickness, this.Height);
    size.x += padding;
    size.z += padding;
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.color = color;
    if (this.m_previewOutline)
      Gizmos.DrawWireCube(new Vector3(this.Bounds.center.x, 0.0f, this.Bounds.center.y), size);
    else
      Gizmos.DrawCube(new Vector3(this.Bounds.center.x, 0.0f, this.Bounds.center.y), size);
  }

  public enum Side
  {
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
  }
}
