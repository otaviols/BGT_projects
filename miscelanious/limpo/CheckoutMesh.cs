using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CheckoutMesh : MonoBehaviour, IScreenSpace
{
  private static readonly Color kBlizzardBlue = new Color(0.0f, 0.1490196f, 0.3137255f, 1f);

  public MeshRenderer MeshRenderer { get; private set; }

  public Texture2D Texture { get; private set; }

  public GameObject CloseButton { get; private set; }

  public static CheckoutMesh GenerateCheckoutMesh(
    int browserWidth,
    int browserHeight,
    float meshWidth,
    float meshHeight)
  {
    CheckoutMesh checkoutMesh = new GameObject(nameof (CheckoutMesh)).AddComponent<CheckoutMesh>();
    checkoutMesh.Initialize(browserWidth, browserHeight, meshWidth, meshHeight);
    return checkoutMesh;
  }

  private void Initialize(int browserWidth, int browserHeight, float meshWidth, float meshHeight)
  {
    MeshFilter filter = this.gameObject.AddComponent<MeshFilter>();
    MeshCollider collider = this.gameObject.AddComponent<MeshCollider>();
    this.MeshRenderer = this.gameObject.AddComponent<MeshRenderer>();
    this.CreateBrowserMesh(filter, collider, meshWidth, meshHeight);
    this.CreateTexture(browserWidth, browserHeight);
  }

  public void UpdateTexture(byte[] bytes)
  {
    if ((Object) this.Texture == (Object) null)
      return;
    this.Texture.LoadRawTextureData(bytes);
    this.Texture.Apply();
  }

  public void ResizeTexture(int width, int height) => this.CreateTexture(width, height);

  public Rect GetScreenRect()
  {
    float height = (float) Screen.height * this.transform.localScale.x;
    return this.GetScreenRect((int) (height * 1.5f), (int) height);
  }

  public Rect GetScreenRect(int width, int height) => new Rect((float) ((Screen.width - width) / 2), (float) ((Screen.height - height) / 2), (float) width, (float) height);

  public float GetScreenSpaceScale() => (float) Screen.height * this.transform.localScale.x / (float) this.Texture.height;

  private void CreateBrowserMesh(
    MeshFilter filter,
    MeshCollider collider,
    float width,
    float height)
  {
    int x = 0;
    int y = 0;
    int z = 0;
    Vector3[] vector3Array = new Vector3[4]
    {
      new Vector3((float) x, (float) y, (float) z),
      new Vector3((float) x + width, (float) y, (float) z),
      new Vector3((float) x, (float) y + height, (float) z),
      new Vector3((float) x + width, (float) y + height, (float) z)
    };
    Vector4[] vector4Array = new Vector4[4]
    {
      new Vector4(1f, 0.0f, 0.0f, -1f),
      new Vector4(1f, 0.0f, 0.0f, -1f),
      new Vector4(1f, 0.0f, 0.0f, -1f),
      new Vector4(1f, 0.0f, 0.0f, -1f)
    };
    int[] numArray = new int[6];
    numArray[0] = 0;
    numArray[3] = numArray[2] = 1;
    numArray[4] = numArray[1] = 2;
    numArray[5] = 3;
    Vector2[] vector2Array = new Vector2[4];
    vector2Array[2] = new Vector2(0.0f, 0.0f);
    vector2Array[3] = new Vector2(1f, 0.0f);
    vector2Array[0] = new Vector2(0.0f, 1f);
    vector2Array[1] = new Vector2(1f, 1f);
    MeshFilter meshFilter = filter;
    Mesh mesh = new Mesh();
    mesh.name = "Blizzard Checkout";
    mesh.vertices = vector3Array;
    mesh.triangles = numArray;
    mesh.uv = vector2Array;
    mesh.tangents = vector4Array;
    meshFilter.mesh = mesh;
    filter.mesh.RecalculateNormals();
    collider.sharedMesh = filter.mesh;
  }

  private void CreateTexture(int width, int height)
  {
    Object.Destroy((Object) this.Texture);
    this.Texture = (Texture2D) null;
    Texture2D texture2D = new Texture2D(width, height, TextureFormat.BGRA32, false, false);
    texture2D.filterMode = FilterMode.Point;
    texture2D.wrapMode = TextureWrapMode.Clamp;
    this.Texture = texture2D;
    Color kBlizzardBlue = CheckoutMesh.kBlizzardBlue;
    for (int x = 0; x < width; ++x)
    {
      for (int y = 0; y < height; ++y)
        this.Texture.SetPixel(x, y, kBlizzardBlue);
    }
    this.Texture.Apply(false, false);
    Material material = new Material(Shader.Find("Hero/Unlit/Unlit_Texture"));
    material.SetTexture("_MainTex", (UnityEngine.Texture) this.Texture);
    this.MeshRenderer.SetMaterial(material);
  }

  private void OnDestroy()
  {
    Object.Destroy((Object) this.Texture);
    this.Texture = (Texture2D) null;
  }
}
