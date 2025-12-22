using UnityEngine;

public class ActivateToggle2 : MonoBehaviour
{
  public GameObject obj;
  private bool onoff;

  private void Start()
  {
    if (!((Object) this.obj != (Object) null))
      return;
    this.onoff = this.obj.activeSelf;
  }

  public void ToggleActive2()
  {
    this.onoff = !this.onoff;
    if (!((Object) this.obj != (Object) null))
      return;
    this.obj.SetActive(this.onoff);
  }

  public void ToggleOn2()
  {
    this.onoff = true;
    if (!((Object) this.obj != (Object) null))
      return;
    this.obj.SetActive(this.onoff);
  }

  public void ToggleOff2()
  {
    this.onoff = false;
    if (!((Object) this.obj != (Object) null))
      return;
    this.obj.SetActive(this.onoff);
  }
}
