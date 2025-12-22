using UnityEngine;

public class ConstantScale : MonoBehaviour
{
  public Vector3 scale = Vector3.one;
  public bool everyFrame;
  private bool isItFirstIteration = true;

  private void LateUpdate()
  {
    if (!this.everyFrame)
    {
      if (!this.isItFirstIteration)
        return;
      this.isItFirstIteration = false;
    }
    Vector3 vector3 = Vector3.one;
    if ((Object) this.transform.parent != (Object) null)
      vector3 = this.transform.parent.transform.lossyScale;
    if ((double) vector3.x + (double) vector3.y + (double) vector3.z == 0.0)
      vector3 = new Vector3(1E-05f, 1E-05f, 1E-05f);
    this.transform.localScale = Vector3.Scale(new Vector3(1f / vector3.x, 1f / vector3.y, 1f / vector3.z), this.scale);
  }
}
