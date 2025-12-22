using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class LocalRenderEffect : MonoBehaviour
{
  private readonly string ADDITIVE_SHADER_NAME = "Hero/Additive/Additive";
  private readonly string BLOOM_SHADER_NAME = "Hidden/LocalRenderBloom";
  private readonly string BLUR_SHADER_NAME = "Hidden/LocalRenderBlur";
  private readonly float RENDER_PLANE_OFFSET = 0.05f;
  public localRenderEffects m_Effect;
  public int m_Resolution = 128;
  public float m_Width = 1f;
  public float m_Height = 1f;
  public float m_Depth = 5f;
  public LayerMask m_LayerMask = (LayerMask) -1;
  public Color m_Color = Color.gray;
  public float m_BlurAmount = 0.6f;
  private GameObject m_CameraGO;
  private Camera m_Camera;
  private RenderTexture m_RenderTexture;
  private Mesh m_PlaneMesh;
  private GameObject m_PlaneGameObject;
  private float m_PreviousWidth;
  private float m_PreviousHeight;
  private Shader m_BloomShader;
  private Material m_BloomMaterial;
  private Shader m_BlurShader;
  private Material m_BlurMaterial;
  private Shader m_AdditiveShader;
  private Material m_AdditiveMaterial;
  private readonly Vector2[] PLANE_UVS = new Vector2[4]
  {
    new Vector2(0.0f, 0.0f),
    new Vector2(1f, 0.0f),
    new Vector2(0.0f, 1f),
    new Vector2(1f, 1f)
  };
  private readonly Vector3[] PLANE_NORMALS = new Vector3[4]
  {
    Vector3.up,
    Vector3.up,
    Vector3.up,
    Vector3.up
  };
  private readonly int[] PLANE_TRIANGLES = new int[6]
  {
    3,
    1,
    2,
    2,
    1,
    0
  };

  protected Material BloomMaterial
  {
    get
    {
      if ((Object) this.m_BloomMaterial == (Object) null)
      {
        this.m_BloomMaterial = new Material(this.m_BloomShader);
        GameObjectUtils.SetHideFlags((Object) this.m_BloomMaterial, HideFlags.DontSave);
      }
      return this.m_BloomMaterial;
    }
  }

  protected Material BlurMaterial
  {
    get
    {
      if ((Object) this.m_BlurMaterial == (Object) null)
      {
        this.m_BlurMaterial = new Material(this.m_BlurShader);
        GameObjectUtils.SetHideFlags((Object) this.m_BlurMaterial, HideFlags.DontSave);
      }
      return this.m_BlurMaterial;
    }
  }

  protected Material AdditiveMaterial
  {
    get
    {
      if ((Object) this.m_AdditiveMaterial == (Object) null)
      {
        this.m_AdditiveMaterial = new Material(this.m_AdditiveShader);
        GameObjectUtils.SetHideFlags((Object) this.m_AdditiveMaterial, HideFlags.DontSave);
      }
      return this.m_AdditiveMaterial;
    }
  }

  protected void Start()
  {
    this.m_BloomShader = ShaderUtils.FindShader(this.BLOOM_SHADER_NAME);
    if (!(bool) (Object) this.m_BloomShader)
      Debug.LogError((object) ("Failed to load Local Rendering Effect Shader: " + this.BLOOM_SHADER_NAME));
    this.m_BlurShader = ShaderUtils.FindShader(this.BLUR_SHADER_NAME);
    if (!(bool) (Object) this.m_BlurShader)
      Debug.LogError((object) ("Failed to load Local Rendering Effect Shader: " + this.BLUR_SHADER_NAME));
    this.m_AdditiveShader = ShaderUtils.FindShader(this.ADDITIVE_SHADER_NAME);
    if (!(bool) (Object) this.m_AdditiveShader)
      Debug.LogError((object) ("Failed to load Local Rendering Effect Shader: " + this.ADDITIVE_SHADER_NAME));
    this.CreateTexture();
    this.CreateCamera();
    this.CreateRenderPlane();
  }

  protected void Update() => this.Render();

  protected void OnDestroy()
  {
    if ((bool) (Object) this.m_Camera)
    {
      this.m_Camera.targetTexture = (RenderTexture) null;
      this.m_Camera.enabled = false;
      Object.Destroy((Object) this.m_Camera);
      Object.Destroy((Object) this.m_CameraGO);
    }
    RenderTexture.active = (RenderTexture) null;
    if ((Object) this.m_BloomMaterial == (Object) null)
      Object.Destroy((Object) this.m_BloomMaterial);
    if ((Object) this.m_BlurMaterial == (Object) null)
      Object.Destroy((Object) this.m_BlurMaterial);
    if ((Object) this.m_AdditiveMaterial == (Object) null)
      Object.Destroy((Object) this.m_AdditiveMaterial);
    if (!(bool) (Object) this.m_RenderTexture)
      return;
    RenderTextureTracker.Get().DestroyRenderTexture(this.m_RenderTexture);
  }

  private void OnDrawGizmos()
  {
    if ((double) this.m_Depth < 0.0)
      this.m_Depth = 0.0f;
    Gizmos.DrawWireCube(new Vector3(this.transform.position.x, this.transform.position.y - this.m_Depth * 0.5f, this.transform.position.z), new Vector3(this.m_Width, this.m_Depth, this.m_Height));
  }

  public void Render()
  {
    if (this.m_Effect == localRenderEffects.Bloom || this.m_Effect == localRenderEffects.Blur)
    {
      RenderTexture temporary = RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, this.m_RenderTexture.depth, this.m_RenderTexture.format);
      this.m_Camera.targetTexture = temporary;
      this.m_Camera.Render();
      this.Sample(temporary, this.m_RenderTexture, this.BlurMaterial, this.m_BlurAmount);
      RenderTexture.ReleaseTemporary(temporary);
      Renderer component = this.m_PlaneGameObject.GetComponent<Renderer>();
      component.SetSharedMaterial(this.BloomMaterial);
      Material sharedMaterial = component.GetSharedMaterial();
      sharedMaterial.SetColor("_TintColor", this.m_Color);
      sharedMaterial.mainTexture = (Texture) this.m_RenderTexture;
    }
    else
    {
      this.m_Camera.targetTexture = this.m_RenderTexture;
      this.m_Camera.Render();
    }
  }

  private void CreateTexture()
  {
    if ((Object) this.m_RenderTexture != (Object) null)
      return;
    Vector2 vector2 = this.CalcTextureSize();
    this.m_RenderTexture = RenderTextureTracker.Get().CreateNewTexture((int) vector2.x, (int) vector2.y, 32, RenderTextureFormat.ARGB32);
  }

  private void CreateCamera()
  {
    if ((Object) this.m_Camera != (Object) null)
      return;
    this.m_CameraGO = new GameObject();
    this.m_Camera = this.m_CameraGO.AddComponent<Camera>();
    this.m_CameraGO.name = this.name + "_TextRenderCamera";
    GameObjectUtils.SetHideFlags((Object) this.m_CameraGO, HideFlags.HideAndDontSave);
    this.m_Camera.orthographic = true;
    this.m_CameraGO.transform.parent = this.transform;
    this.m_CameraGO.transform.rotation = Quaternion.identity;
    this.m_CameraGO.transform.position = this.transform.position;
    this.m_CameraGO.transform.Rotate(90f, 0.0f, 0.0f);
    this.m_Camera.nearClipPlane = 0.0f;
    this.m_Camera.farClipPlane = this.m_Depth;
    this.m_Camera.depth = Camera.main.depth - 1f;
    this.m_Camera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    this.m_Camera.clearFlags = CameraClearFlags.Color;
    this.m_Camera.depthTextureMode = DepthTextureMode.None;
    this.m_Camera.renderingPath = RenderingPath.Forward;
    this.m_Camera.cullingMask = (int) this.m_LayerMask;
    this.m_Camera.allowHDR = false;
    this.m_Camera.targetTexture = this.m_RenderTexture;
    this.m_Camera.enabled = false;
    this.m_Camera.orthographicSize = Mathf.Min(this.m_Width * 0.5f, this.m_Height * 0.5f);
  }

  private Vector2 CalcTextureSize() => new Vector2((float) this.m_Resolution, (float) this.m_Resolution * (this.m_Height / this.m_Width));

  private void CreateRenderPlane()
  {
    if ((double) this.m_Width == (double) this.m_PreviousWidth && (double) this.m_Height == (double) this.m_PreviousHeight)
      return;
    if ((Object) this.m_PlaneGameObject != (Object) null)
      Object.Destroy((Object) this.m_PlaneGameObject);
    this.m_PlaneGameObject = this.gameObject;
    this.m_PlaneGameObject.AddComponent<MeshFilter>();
    this.m_PlaneGameObject.AddComponent<MeshRenderer>();
    Mesh mesh = new Mesh();
    mesh.name = "TextMeshPlane";
    float x = this.m_Width * 0.5f;
    float z = this.m_Height * 0.5f;
    mesh.vertices = new Vector3[4]
    {
      new Vector3(-x, this.RENDER_PLANE_OFFSET, -z),
      new Vector3(x, this.RENDER_PLANE_OFFSET, -z),
      new Vector3(-x, this.RENDER_PLANE_OFFSET, z),
      new Vector3(x, this.RENDER_PLANE_OFFSET, z)
    };
    mesh.uv = this.PLANE_UVS;
    mesh.normals = this.PLANE_NORMALS;
    mesh.triangles = this.PLANE_TRIANGLES;
    this.m_PlaneMesh = this.m_PlaneGameObject.GetComponent<MeshFilter>().mesh = mesh;
    this.m_PlaneMesh.RecalculateBounds();
    this.BloomMaterial.mainTexture = (Texture) this.m_RenderTexture;
    this.m_PlaneGameObject.GetComponent<Renderer>().SetSharedMaterial(this.AdditiveMaterial);
    this.m_PreviousWidth = this.m_Width;
    this.m_PreviousHeight = this.m_Height;
  }

  private void Sample(RenderTexture source, RenderTexture dest, Material sampleMat, float offset)
  {
    dest.DiscardContents();
    Graphics.BlitMultiTap((Texture) source, dest, sampleMat, new Vector2(-offset, -offset), new Vector2(-offset, offset), new Vector2(offset, offset), new Vector2(offset, -offset));
  }
}
