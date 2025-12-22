using Hearthstone.UI;
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class BaconCollectionEmoteLayoutWidgetBehaviour : MonoBehaviour
{
  [SerializeField]
  private AsyncReference m_imageWidgetRef;
  private SortingGroup m_sortingGroup;
  private BoxCollider m_dragCollider;
  public bool m_flipBubble;
  [SerializeField]
  private bool m_disableClickable;

  private void Start() => this.m_imageWidgetRef.RegisterReadyListener<Transform>((Action<Transform>) (t =>
  {
    LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_sortingGroup = this.GetComponentInChildren<SortingGroup>();
    foreach (Collider componentsInChild in t.GetComponentsInChildren<BoxCollider>())
      componentsInChild.enabled = false;
    if (!this.m_disableClickable)
      return;
    this.gameObject.GetComponentInChildren<Clickable>().enabled = false;
  }));

  public void IncreaseSpriteSortOrder(int amount) => this.m_sortingGroup.sortingOrder += amount;

  public BoxCollider GetDragCollider()
  {
    if ((UnityEngine.Object) this.m_dragCollider == (UnityEngine.Object) null)
      this.m_dragCollider = this.GetComponentInChildren<BoxCollider>();
    return this.m_dragCollider;
  }
}
