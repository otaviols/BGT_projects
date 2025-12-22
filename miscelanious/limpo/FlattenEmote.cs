using Hearthstone.UI;
using System;
using UnityEngine;

public class FlattenEmote : MonoBehaviour
{
  public WidgetInstance target;
  public int TargetSortingOrder;

  private void Start() => this.target.RegisterReadyListener(new Action<object>(this.FlattenListener), (object) null, true);

  private void FlattenListener(object o) => this.Flatten();

  public void Flatten()
  {
    foreach (Renderer componentsInChild in this.GetComponentsInChildren<SpriteRenderer>(true))
      componentsInChild.sortingOrder = this.TargetSortingOrder;
  }
}
