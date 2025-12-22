using Hearthstone.UI;
using System;
using UnityEngine;

public class BaconEmoteTrayWidget : MonoBehaviour
{
  public bool m_pointBubbleRight;
  [SerializeField]
  private AsyncReference m_imageWidget;

  private void Start() => this.m_imageWidget.RegisterReadyListener<Transform>((Action<Transform>) (t =>
  {
    foreach (Collider componentsInChild in t.GetComponentsInChildren<BoxCollider>())
      componentsInChild.enabled = false;
  }));
}
