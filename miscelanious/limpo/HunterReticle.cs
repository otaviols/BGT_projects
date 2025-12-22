using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class HunterReticle : BlitToTexture
{
  private static readonly Vector3[] s_planeVertices = new Vector3[4]
  {
    new Vector3(-1f, 0.0f, -1f),
    new Vector3(1f, 0.0f, -1f),
    new Vector3(-1f, 0.0f, 1f),
    new Vector3(1f, 0.0f, 1f)
  };
  private static readonly Vector2[] s_planeUvs = new Vector2[4]
  {
    new Vector2(0.0f, 0.0f),
    new Vector2(1f, 0.0f),
    new Vector2(0.0f, 1f),
    new Vector2(1f, 1f)
  };
  private static readonly Vector3[] s_planeNormals = new Vector3[4]
  {
    Vector3.up,
    Vector3.up,
    Vector3.up,
    Vector3.up
  };
  private static readonly int[] s_planeTriangles = new int[6]
  {
    3,
    1,
    2,
    2,
    1,
    0
  };
  public float ReticleSize = 1f;
  public Material Material;
  private Camera m_mainCamera;

  protected override void Awake()
  {
    base.Awake();
    GameObject gameObject = new GameObject();
    gameObject.name = this.name;
    gameObject.transform.parent = this.transform;
    gameObject.transform.localPosition = Vector3.zero;
    gameObject.transform.localRotation = Quaternion.identity;
    gameObject.transform.localScale = Vector3.one * this.ReticleSize;
    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
    MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
    renderer.SetMaterial(this.Material);
    renderer.enabled = false;
    this.DrawAfterBlit = (Renderer) renderer;
    Mesh mesh = new Mesh();
    mesh.vertices = HunterReticle.s_planeVertices;
    mesh.uv = HunterReticle.s_planeUvs;
    mesh.normals = HunterReticle.s_planeNormals;
    mesh.triangles = HunterReticle.s_planeTriangles;
    mesh.RecalculateBounds();
    meshFilter.mesh = mesh;
    if (!((Object) this.Material != (Object) null))
      return;
    this.Material.mainTexture = (Texture) this.TargetTexture;
  }

  protected override void Update()
  {
    if ((Object) this.m_mainCamera == (Object) null)
      this.m_mainCamera = CameraUtils.GetMainCamera();
    this.Offset = (Vector2) this.m_mainCamera.WorldToScreenPoint(this.transform.position);
    base.Update();
  }
}
