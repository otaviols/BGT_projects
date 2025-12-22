using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class BaconCollectionUnownedItemRender : MonoBehaviour
{
  [SerializeField]
  private RenderToTexture TextureRenderer;
  [SerializeField]
  private UberText NameText;

  private void Start() => this.gameObject.GetComponent<Widget>().RegisterDoneChangingStatesListener(new Action<object>(this.DoneChangingStates), (object) null, true, false);

  private void DoneChangingStates(object unused)
  {
    if (!((UnityEngine.Object) this.TextureRenderer != (UnityEngine.Object) null) || !this.TextureRenderer.gameObject.activeInHierarchy)
      return;
    if ((UnityEngine.Object) this.NameText != (UnityEngine.Object) null && this.NameText.gameObject.activeInHierarchy)
      this.StartCoroutine(this.RenderOnceTextReady());
    else
      this.TextureRenderer.RenderNow();
  }

  private IEnumerator RenderOnceTextReady()
  {
    while (!this.NameText.IsDone())
      yield return (object) null;
    this.TextureRenderer.RenderNow();
  }
}
