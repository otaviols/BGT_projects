using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (MeshRenderer), typeof (MeshFilter))]
public class Tile9Mesh : MonoBehaviour
{
  public float width = 1f;
  public float height = 1f;
  [Range(0.0f, 0.5f)]
  public float uvLeft = 0.2f;
  [Range(0.0f, 0.5f)]
  public float uvRight = 0.2f;
  [Range(0.0f, 0.5f)]
  public float uvTop = 0.2f;
  [Range(0.0f, 0.5f)]
  public float uvBottom = 0.2f;
  public float uvToWorldScaleX = 1f;
  public float uvToWorldScaleY = 1f;
  public Vector2 pivot = new Vector2(0.5f, 0.5f);
  private Mesh mesh;
  private Vector3[] vertices;
  private Vector2[] uv;

  private void Start()
  {
    this.vertices = new Vector3[16];
    this.uv = new Vector2[16];
    Mesh mesh = new Mesh();
    mesh.name = nameof (Tile9Mesh);
    this.mesh = mesh;
    this.FillGeometry();
    this.FillMesh();
    this.mesh.triangles = new int[54]
    {
      0,
      1,
      12,
      0,
      12,
      11,
      1,
      2,
      13,
      1,
      13,
      12,
      2,
      3,
      4,
      2,
      4,
      13,
      13,
      4,
      5,
      13,
      5,
      14,
      14,
      5,
      6,
      14,
      6,
      7,
      15,
      14,
      7,
      15,
      7,
      8,
      10,
      15,
      8,
      10,
      8,
      9,
      11,
      12,
      15,
      11,
      15,
      10,
      12,
      13,
      14,
      12,
      14,
      15
    };
    this.RecalculateMesh();
    this.gameObject.GetComponent<MeshFilter>().mesh = this.mesh;
  }

  public void UpdateMesh()
  {
    if (!((Object) this.mesh != (Object) null))
      return;
    this.FillGeometry();
    this.FillMesh();
    this.mesh.RecalculateBounds();
    this.mesh.RecalculateNormals();
  }

  private void FillGeometry()
  {
    float num1 = this.pivot.x * this.width;
    float num2 = this.pivot.y * this.height;
    float width = this.width;
    float height = this.height;
    float num3 = this.uvLeft * this.uvToWorldScaleX;
    float num4 = this.width - this.uvRight * this.uvToWorldScaleX;
    float num5 = this.height - this.uvTop * this.uvToWorldScaleY;
    float num6 = this.uvBottom * this.uvToWorldScaleY;
    this.vertices[0] = new Vector3(0.0f - num1, 0.0f - num2, 0.0f);
    this.vertices[1] = new Vector3(0.0f - num1, num6 - num2, 0.0f);
    this.vertices[2] = new Vector3(0.0f - num1, num5 - num2, 0.0f);
    this.vertices[3] = new Vector3(0.0f - num1, height - num2, 0.0f);
    this.vertices[4] = new Vector3(num3 - num1, height - num2, 0.0f);
    this.vertices[5] = new Vector3(num4 - num1, height - num2, 0.0f);
    this.vertices[6] = new Vector3(width - num1, height - num2, 0.0f);
    this.vertices[7] = new Vector3(width - num1, num5 - num2, 0.0f);
    this.vertices[8] = new Vector3(width - num1, num6 - num2, 0.0f);
    this.vertices[9] = new Vector3(width - num1, 0.0f - num2, 0.0f);
    this.vertices[10] = new Vector3(num4 - num1, 0.0f - num2, 0.0f);
    this.vertices[11] = new Vector3(num3 - num1, 0.0f - num2, 0.0f);
    this.vertices[12] = new Vector3(num3 - num1, num6 - num2, 0.0f);
    this.vertices[13] = new Vector3(num3 - num1, num5 - num2, 0.0f);
    this.vertices[14] = new Vector3(num4 - num1, num5 - num2, 0.0f);
    this.vertices[15] = new Vector3(num4 - num1, num6 - num2, 0.0f);
    float uvLeft = this.uvLeft;
    float x = 1f - this.uvRight;
    float y = 1f - this.uvTop;
    float uvBottom = this.uvBottom;
    this.uv[0] = new Vector2(0.0f, 0.0f);
    this.uv[1] = new Vector2(0.0f, uvBottom);
    this.uv[2] = new Vector2(0.0f, y);
    this.uv[3] = new Vector2(0.0f, 1f);
    this.uv[4] = new Vector2(uvLeft, 1f);
    this.uv[5] = new Vector2(x, 1f);
    this.uv[6] = new Vector2(1f, 1f);
    this.uv[7] = new Vector2(1f, y);
    this.uv[8] = new Vector2(1f, uvBottom);
    this.uv[9] = new Vector2(1f, 0.0f);
    this.uv[10] = new Vector2(x, 0.0f);
    this.uv[11] = new Vector2(uvLeft, 0.0f);
    this.uv[12] = new Vector2(uvLeft, uvBottom);
    this.uv[13] = new Vector2(uvLeft, y);
    this.uv[14] = new Vector2(x, y);
    this.uv[15] = new Vector2(x, uvBottom);
  }

  private void FillMesh()
  {
    this.mesh.vertices = this.vertices;
    this.mesh.uv = this.uv;
  }

  private void RecalculateMesh()
  {
    this.mesh.RecalculateBounds();
    this.mesh.RecalculateNormals();
  }
}
