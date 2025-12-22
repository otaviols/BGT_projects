using UnityEngine;

[ExecuteAlways]
public class FrameAnimation : MonoBehaviour
{
  public Vector2 tiles = Vector2.one;
  public float currentFrame;
  public Material material;
  private Vector4 scaleOffsetUV;
  private string scaleOffsetUVParametrName = "_MainTex_ST";

  private void Start() => this.scaleOffsetUV = new Vector4(1f / this.tiles.x, 1f / this.tiles.y);

  private void Update()
  {
    if ((Object) this.material == (Object) null)
      return;
    int num = Mathf.FloorToInt(this.currentFrame);
    this.scaleOffsetUV.z = (float) num % this.tiles.x;
    this.scaleOffsetUV.w = (float) ((double) this.tiles.y - 1.0 - ((double) num - (double) this.scaleOffsetUV.z) / (double) this.tiles.x % (double) this.tiles.y);
    this.scaleOffsetUV.z *= this.scaleOffsetUV.x;
    this.scaleOffsetUV.w *= this.scaleOffsetUV.y;
    this.material.SetVector(this.scaleOffsetUVParametrName, this.scaleOffsetUV);
  }
}
