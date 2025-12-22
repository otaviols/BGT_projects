using UnityEngine;

public class MeshUVScaler : MonoBehaviour
{
  public float UVScaleX;
  public float UVScaleY;
  private Vector2[] uvcache;
  private Vector2[] uvs;
  private MeshFilter meshFilter;
  private SkinnedMeshRenderer skinnedMeshRenderer;
  private Mesh mesh;

  private void OnEnable()
  {
    this.meshFilter = this.GetComponent<MeshFilter>();
    this.skinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>();
    if ((bool) (Object) this.meshFilter)
      this.mesh = this.meshFilter.mesh;
    else if ((bool) (Object) this.skinnedMeshRenderer)
      this.mesh = this.skinnedMeshRenderer.sharedMesh;
    if (!(bool) (Object) this.mesh)
      this.enabled = false;
    this.uvcache = this.mesh.uv;
    this.uvs = this.mesh.uv;
    this.UVScaleX = 1f;
    this.UVScaleY = 1f;
  }

  private void Update()
  {
    if (!(bool) (Object) this.mesh)
      return;
    for (int index = 0; index < this.uvcache.Length; ++index)
      this.uvs[index] = new Vector2(this.uvcache[index].x * this.UVScaleX, this.uvcache[index].y * this.UVScaleY);
    this.mesh.uv = this.uvs;
  }

  private void OnDisable() => this.mesh.uv = this.uvcache;
}
