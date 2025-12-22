using UnityEngine;

public class SetVisible : MonoBehaviour
{
  public GameObject obj;
  private MeshRenderer[] renderers;
  private SkinnedMeshRenderer[] skinRenderers;
  private bool onoff;

  private void Start()
  {
    if ((Object) this.obj == (Object) null)
      return;
    this.renderers = this.obj.GetComponentsInChildren<MeshRenderer>();
    this.skinRenderers = this.obj.GetComponentsInChildren<SkinnedMeshRenderer>();
  }

  public void SetOn()
  {
    this.onoff = true;
    this.SetRenderers(this.onoff);
  }

  public void SetOff()
  {
    this.onoff = false;
    this.SetRenderers(this.onoff);
  }

  private void SetRenderers(bool value)
  {
    if ((Object) this.obj == (Object) null || this.renderers == null || this.renderers.Length == 0)
      return;
    foreach (Renderer renderer in this.renderers)
      renderer.enabled = value;
    foreach (Renderer skinRenderer in this.skinRenderers)
      skinRenderer.enabled = value;
  }
}
