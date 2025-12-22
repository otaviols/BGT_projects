using UnityEngine;

public class ActivateToggle : MonoBehaviour
{
  public GameObject obj;
  private bool onoff;

  private void Start()
  {
    if (!((Object) this.obj != (Object) null))
      return;
    this.onoff = this.obj.activeSelf;
  }

  public void ToggleActive()
  {
    this.onoff = !this.onoff;
    if (!((Object) this.obj != (Object) null))
      return;
    this.obj.SetActive(this.onoff);
  }

  public void ToggleOn()
  {
    this.onoff = true;
    if (!((Object) this.obj != (Object) null))
      return;
    this.obj.SetActive(this.onoff);
  }

  public void ToggleOff()
  {
    this.onoff = false;
    if (!((Object) this.obj != (Object) null))
      return;
    this.obj.SetActive(this.onoff);
  }
}
