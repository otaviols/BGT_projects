using Hearthstone.UI.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Actor))]
public class CustomHeroFrameBehaviour : MonoBehaviour, IPopupRendering
{
  [SerializeField]
  private GameObject m_defaultFrame;
  private GameObject m_frameMesh;
  private Actor m_actor;
  private IPopupRoot m_popupRoot;
  private HashSet<IPopupRendering> m_popupRenderingComponents = new HashSet<IPopupRendering>();

  private void Awake() => this.UpdateFrame();

  public void UpdateFrame()
  {
    this.m_actor = this.GetComponent<Actor>();
    if ((Object) this.m_actor != (Object) null && this.m_actor.HasCardDef)
    {
      this.LoadFrameMesh();
    }
    else
    {
      this.InstantiateFrameMesh(this.m_defaultFrame);
      this.StartCoroutine(this.WaitForCardDef());
    }
  }

  private IEnumerator WaitForCardDef()
  {
    while ((Object) this.m_actor != (Object) null && !this.m_actor.HasCardDef)
      yield return (object) null;
    this.LoadFrameMesh();
  }

  private void LoadFrameMesh()
  {
    if ((Object) this.m_actor == (Object) null)
      return;
    DefLoader.DisposableCardDef disposableCardDef = this.m_actor.ShareDisposableCardDef();
    if (disposableCardDef != null && (Object) disposableCardDef.CardDef.m_FrameMeshOverride != (Object) null)
      this.InstantiateFrameMesh(disposableCardDef.CardDef.m_FrameMeshOverride);
    else
      this.InstantiateFrameMesh(this.m_defaultFrame);
  }

  private void InstantiateFrameMesh(GameObject frameObject)
  {
    if ((Object) this.m_frameMesh != (Object) null)
      Object.Destroy((Object) this.m_frameMesh);
    this.m_frameMesh = Object.Instantiate<GameObject>(frameObject, this.m_actor.m_portraitMesh.transform);
    LayerUtils.SetLayer(this.m_frameMesh, this.m_actor.m_portraitMesh.layer);
    if (this.m_popupRoot == null)
      return;
    this.m_popupRoot.ApplyPopupRendering(this.m_frameMesh.transform, this.m_popupRenderingComponents);
  }

  public GameObject GetMeshObject() => this.m_frameMesh;

  public void EnablePopupRendering(IPopupRoot popupRoot)
  {
    if (this.m_popupRoot != popupRoot && (Object) this.m_frameMesh != (Object) null)
      popupRoot.ApplyPopupRendering(this.m_frameMesh.transform, this.m_popupRenderingComponents, true, this.gameObject.layer);
    this.m_popupRoot = popupRoot;
  }

  public void DisablePopupRendering()
  {
    if (this.m_popupRoot == null)
      return;
    this.m_popupRoot.CleanupPopupRendering(this.m_popupRenderingComponents);
    this.m_popupRoot = (IPopupRoot) null;
  }

  public bool HandlesChildPropagation() => false;
}
