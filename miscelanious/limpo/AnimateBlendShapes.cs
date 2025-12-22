using UnityEngine;

[ExecuteAlways]
public class AnimateBlendShapes : MonoBehaviour
{
  private float prevBlendAmount;
  public float blendAmount;
  public int index;
  private SkinnedMeshRenderer skinMR;

  private void Start() => this.skinMR = this.GetComponent<SkinnedMeshRenderer>();

  private void Update()
  {
    if ((double) this.prevBlendAmount == (double) this.blendAmount)
      return;
    this.prevBlendAmount = this.blendAmount;
    this.skinMR.SetBlendShapeWeight(this.index, this.blendAmount);
  }
}
