using System;
using UnityEngine;

[Serializable]
public class CanvasAnchors
{
  public Transform m_Center;
  public Transform m_Left;
  public Transform m_Right;
  public Transform m_Bottom;
  public Transform m_Top;
  public Transform m_BottomLeft;
  public Transform m_BottomRight;
  public Transform m_TopLeft;
  public Transform m_TopRight;

  public Transform GetAnchor(CanvasAnchor type)
  {
    switch (type)
    {
      case CanvasAnchor.CENTER:
        return this.m_Center;
      case CanvasAnchor.LEFT:
        return this.m_Left;
      case CanvasAnchor.RIGHT:
        return this.m_Right;
      case CanvasAnchor.BOTTOM:
        return this.m_Bottom;
      case CanvasAnchor.TOP:
        return this.m_Top;
      case CanvasAnchor.BOTTOM_LEFT:
        return this.m_BottomLeft;
      case CanvasAnchor.BOTTOM_RIGHT:
        return this.m_BottomRight;
      case CanvasAnchor.TOP_LEFT:
        return this.m_TopLeft;
      case CanvasAnchor.TOP_RIGHT:
        return this.m_TopRight;
      default:
        return this.m_Center;
    }
  }

  public void WillReset()
  {
    foreach (Component component in this.m_Center)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    foreach (Component component in this.m_Left)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    foreach (Component component in this.m_Right)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    foreach (Component component in this.m_Bottom)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    foreach (Component component in this.m_Top)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    foreach (Component component in this.m_BottomLeft)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    foreach (Component component in this.m_BottomRight)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    foreach (Component component in this.m_TopLeft)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    foreach (Component component in this.m_TopRight)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
  }
}
