using UnityEngine;

[ExecuteInEditMode]
public class SimpleTransformConstraint : MonoBehaviour
{
  public int currentParent;
  public Transform[] parents;
  public bool position = true;
  public bool rotation = true;
  public bool scale = true;

  private void Update()
  {
    if (this.position)
      this.transform.position = this.parents[this.currentParent].position;
    if (this.rotation)
      this.transform.rotation = this.parents[this.currentParent].rotation;
    if (!this.scale)
      return;
    this.transform.localScale = this.parents[this.currentParent].localScale;
  }
}
