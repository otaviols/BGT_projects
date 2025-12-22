using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using UnityEngine;

public class DiamondRenderToTexture : MonoBehaviour
{
  private static readonly Vector3 ALPHA_OBJECT_OFFSET = new Vector3(0.0f, 1000f, 0.0f);
  private static readonly Color GIZMOS_COLOR = new Color(1f, 1f, 0.0f, 0.8f);
  public GameObject m_ObjectToRender;
  public GameObject m_AlphaObjectToRender;
  public bool m_AllowRepetition;
  public bool m_HideRenderObject = true;
  public bool m_RealtimeRender;
  public bool m_RealtimeTranslation;
  public bool m_OpaqueObjectAlphaFill;
  public DiamondRenderToTexture.RenderToTextureMaterial m_RenderMaterial;
  public Material m_Material;
  public bool m_CreateRenderPlane;
  public Color m_ClearColor = Color.clear;
  public GameObject m_RenderToObject;
  [Range(1f, 2048f)]
  public int m_Resolution = 128;
  public Vector3 m_bounds = Vector3.one;
  public bool m_UniformWorldScale;
  public Vector3 m_PositionOffset = Vector3.zero;
  private const string TRANSPARENT_SHADER_NAME = "Hidden/R2TTransparent";
  private Shader m_TransparentShader;
  private Material m_TransparentMaterial;
  private bool m_isRegisteredToManager;
  private bool m_isDirty;
  private DiamondRenderToTextureService m_diamondRenderToTextureService;
  private Vector3 m_worldSize;
  private Vector3 m_worldScale;
  private DiamondRenderToTexture.TransformData m_transformSnapshot;
  private Bounds m_renderBounds = new Bounds(Vector3.zero, Vector3.zero);
  private Renderer m_outputRenderer;
  private DiamondRenderToTexture.TransformData m_atlasPositionSnapshot;
  private Transform m_selfOriginalParent;
  private Transform m_objectToRenderOriginalParent;

  protected Material TransparentMaterial
  {
    get
    {
      if ((Object) this.m_TransparentMaterial == (Object) null)
      {
        if ((Object) this.m_TransparentShader == (Object) null)
        {
          this.m_TransparentShader = ShaderUtils.FindShader("Hidden/R2TTransparent");
          if (!(bool) (Object) this.m_TransparentShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TTransparent");
        }
        this.m_TransparentMaterial = new Material(this.m_TransparentShader);
        GameObjectUtils.SetHideFlags((Object) this.m_TransparentMaterial, HideFlags.DontSave);
      }
      return this.m_TransparentMaterial;
    }
  }

  public GameObject OffscreenGameObject { get; private set; }

  public Vector2Int TextureSize { get; private set; }

  public Bounds RendererBounds => this.m_renderBounds;

  public Vector3 PivotPosition => this.m_PositionOffset - Vector3.Scale(new Vector3(-1f, 1f, 1f), this.m_bounds / 2f);

  public Vector3 WorldPivotOffset => this.transform.TransformPoint(this.PivotPosition) - this.transform.position;

  public DiamondRenderToTexture.TransformData TransformSnapshot => this.m_transformSnapshot;

  public bool HasAtlasPosition { get; set; }

  public Vector3 WorldBounds => this.m_worldSize;

  public Vector3 ObjectToRenderOffset { get; private set; }

  public RenderCommandLists RenderCommands { get; private set; }

  private void Awake() => this.FetchOutputRenderer();

  private void Start()
  {
    this.m_diamondRenderToTextureService = ServiceManager.Get<DiamondRenderToTextureService>();
    if (!(bool) (Object) this.m_ObjectToRender)
    {
      this.m_isDirty = true;
    }
    else
    {
      if (this.m_HideRenderObject)
        this.m_ObjectToRender.SetActive(false);
      this.FetchObjectRequiredData();
      this.RegisterToService();
    }
  }

  private void Update()
  {
    if (this.transform.hasChanged)
      this.m_isDirty = true;
    if (this.m_isDirty)
      this.FetchObjectRequiredData();
    if (this.m_isRegisteredToManager)
      return;
    this.RegisterToService();
  }

  private void OnValidate()
  {
    this.CalcWorldWidthHeightScale();
    this.m_isDirty = true;
  }

  private void OnDisable()
  {
    this.UnregisterFromService();
    this.ReleaseRenderCommands();
  }

  private void OnEnable()
  {
    this.FetchObjectRequiredData();
    this.RegisterToService();
  }

  private void OnDestroy()
  {
    this.UnregisterFromService();
    this.ReleaseRenderCommands();
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.enabled || !(bool) (Object) this.m_ObjectToRender)
      return;
    Gizmos.matrix = Matrix4x4.TRS(this.m_ObjectToRender.transform.position, this.transform.rotation, this.transform.lossyScale);
    Gizmos.color = DiamondRenderToTexture.GIZMOS_COLOR;
    Gizmos.DrawSphere(this.m_PositionOffset, 0.1f);
    Gizmos.DrawWireCube(this.m_PositionOffset, this.m_bounds);
    Gizmos.DrawSphere(this.PivotPosition, 0.1f);
    Vector3 pos = this.m_PositionOffset + new Vector3(0.0f, this.m_bounds.y / 2f, 0.0f);
    DiamondRenderToTexture.GizmosDrawArrow(pos, Vector3.forward, Color.blue);
    DiamondRenderToTexture.GizmosDrawArrow(pos, Vector3.up, Color.green);
    Gizmos.matrix = Matrix4x4.identity;
  }

  public bool IsEqual(DiamondRenderToTexture other) => other.m_ObjectToRender.GetInstanceID() == this.m_ObjectToRender.GetInstanceID();

  public void OnAddedToAtlas(RenderTexture atlasTexture, Rect atlasUV)
  {
    this.UpdatePlaneUVS(atlasUV);
    this.UpdateMaterial(atlasTexture);
  }

  public void PushTransform()
  {
    Transform transform = this.m_ObjectToRender.transform;
    this.m_transformSnapshot.position = transform.position;
    this.m_transformSnapshot.localScale = transform.localScale;
    this.m_transformSnapshot.rotation = transform.rotation;
    this.m_transformSnapshot.layer = this.m_ObjectToRender.layer;
    this.m_transformSnapshot.up = this.transform.up;
    this.m_transformSnapshot.forward = this.transform.forward;
    this.m_transformSnapshot.objectParent = transform.parent;
    this.m_transformSnapshot.atlasedComponentParent = this.transform.parent;
  }

  public void ResetTransform(Vector3 position)
  {
    Transform transform1 = this.m_ObjectToRender.transform;
    transform1.parent = (Transform) null;
    transform1.localScale = Vector3.one;
    transform1.position = position;
    Transform transform2 = this.transform;
    transform2.parent = (Transform) null;
    transform2.localScale = Vector3.one;
    transform2.position = position;
    this.CalcWorldWidthHeightScale();
  }

  public void RestoreParents()
  {
    this.m_ObjectToRender.transform.parent = this.m_transformSnapshot.objectParent;
    this.transform.parent = this.m_transformSnapshot.atlasedComponentParent;
  }

  public void PopTransform()
  {
    Transform transform = this.m_ObjectToRender.transform;
    transform.position = this.TransformSnapshot.position;
    transform.localScale = this.TransformSnapshot.localScale;
    transform.rotation = this.TransformSnapshot.rotation;
    this.m_ObjectToRender.layer = this.m_transformSnapshot.layer;
    this.transform.up = this.m_transformSnapshot.up;
    this.transform.forward = this.m_transformSnapshot.forward;
  }

  public void Refresh() => this.m_isDirty = true;

  public void CaptureAtlasPosition()
  {
    this.HasAtlasPosition = true;
    Transform transform1 = this.transform;
    Transform transform2 = this.m_ObjectToRender.transform;
    this.m_atlasPositionSnapshot.position = transform2.position;
    this.m_atlasPositionSnapshot.localScale = transform2.localScale;
    this.m_atlasPositionSnapshot.rotation = transform2.rotation;
    this.m_atlasPositionSnapshot.up = transform1.up;
    this.m_atlasPositionSnapshot.forward = transform1.forward;
  }

  public bool MaintainsAtlasPosition()
  {
    Transform transform = this.m_ObjectToRender.transform;
    if (!transform.hasChanged)
      return true;
    int num1 = this.m_atlasPositionSnapshot.position == transform.position ? 1 : 0;
    bool flag1 = this.m_atlasPositionSnapshot.localScale == transform.localScale;
    bool flag2 = this.m_atlasPositionSnapshot.rotation == transform.rotation;
    int num2 = flag1 ? 1 : 0;
    return (num1 & num2 & (flag2 ? 1 : 0)) != 0;
  }

  public void RestoreAtlasPosition()
  {
    Transform transform = this.m_ObjectToRender.transform;
    transform.position = this.m_atlasPositionSnapshot.position;
    transform.localScale = this.m_atlasPositionSnapshot.localScale;
    transform.rotation = this.m_atlasPositionSnapshot.rotation;
    this.transform.position = this.m_atlasPositionSnapshot.position;
    this.transform.localScale = this.m_atlasPositionSnapshot.localScale;
    this.transform.rotation = Quaternion.LookRotation(this.m_atlasPositionSnapshot.forward, this.m_atlasPositionSnapshot.up);
  }

  public void RestoreOriginalParents()
  {
    if ((bool) (Object) this.m_objectToRenderOriginalParent && (bool) (Object) this.m_ObjectToRender)
      this.m_ObjectToRender.transform.parent = this.m_objectToRenderOriginalParent;
    if (!(bool) (Object) this.m_selfOriginalParent || !(bool) (Object) this.transform)
      return;
    this.transform.parent = this.m_selfOriginalParent;
  }

  private void FetchObjectRequiredData()
  {
    if (!(bool) (Object) this.m_ObjectToRender)
      return;
    this.CaptureOriginalParents();
    this.FetchOutputRenderer();
    this.CalculateObjectToRenderOffset();
    this.CalcTextureSize();
    Renderer[] componentsInChildren = this.m_ObjectToRender.GetComponentsInChildren<Renderer>(true);
    this.m_renderBounds = RenderToTextureUtils.CalcRendererBounds(componentsInChildren);
    if (this.RenderCommands == null)
    {
      this.RenderCommands = RenderToTextureUtils.RenderCommandListPool.Get(componentsInChildren);
    }
    else
    {
      this.RenderCommands.Clear();
      this.RenderCommands.AppendRenderCommands(componentsInChildren);
    }
    this.HasAtlasPosition = false;
    this.m_isDirty = false;
  }

  private void ReleaseRenderCommands()
  {
    if (this.RenderCommands == null)
      return;
    RenderToTextureUtils.RenderCommandListPool.Release(this.RenderCommands);
    this.RenderCommands = (RenderCommandLists) null;
  }

  private void SetupAuxRenderObjects()
  {
    if (!(bool) (Object) this.m_ObjectToRender)
      return;
    if (this.m_RealtimeTranslation)
    {
      this.OffscreenGameObject = new GameObject("R2TOffsetRenderRoot_" + this.name);
      this.OffscreenGameObject.transform.position = this.transform.position;
      this.m_ObjectToRender.transform.SetParent(this.OffscreenGameObject.transform);
    }
    if (!this.m_HideRenderObject)
      return;
    if (this.m_RealtimeTranslation && (bool) (Object) this.m_AlphaObjectToRender)
      this.m_AlphaObjectToRender.transform.SetParent(this.OffscreenGameObject.transform);
    if (!(bool) (Object) this.m_AlphaObjectToRender)
      return;
    this.m_AlphaObjectToRender.transform.position = this.transform.position - DiamondRenderToTexture.ALPHA_OBJECT_OFFSET;
  }

  private void CalcWorldWidthHeightScale()
  {
    Transform transform = this.transform;
    Quaternion rotation = transform.rotation;
    Vector3 localScale = transform.localScale;
    Transform parent = transform.parent;
    transform.rotation = Quaternion.identity;
    Vector3 lossyScale = transform.lossyScale;
    bool flag = false;
    if ((double) lossyScale.magnitude == 0.0)
    {
      this.transform.parent = (Transform) null;
      this.transform.localScale = Vector3.one;
      flag = true;
    }
    if (this.m_UniformWorldScale)
    {
      float num = Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
      this.m_worldScale = new Vector3(num, num, num);
    }
    else
      this.m_worldScale = lossyScale;
    this.m_worldSize = new Vector3(this.m_bounds.x * this.m_worldScale.x, this.m_bounds.y * this.m_worldScale.y, this.m_bounds.z * this.m_worldScale.z);
    if (flag)
    {
      this.transform.parent = parent;
      this.transform.localScale = localScale;
    }
    this.transform.rotation = rotation;
    if ((double) this.m_worldSize.x != 0.0 && (double) this.m_worldSize.y != 0.0)
      return;
    Debug.LogError((object) string.Format(" \"{0}\": RenderToTexture has a world scale of zero. \nm_WorldWidth: {1},   m_WorldHeight: {2}", (object) this.m_worldSize.x, (object) this.m_worldSize.y));
  }

  private void CalcTextureSize() => this.TextureSize = new Vector2Int(this.m_Resolution, Mathf.RoundToInt((float) this.m_Resolution * (this.m_bounds.y / this.m_bounds.x)));

  private void CalculateObjectToRenderOffset() => this.ObjectToRenderOffset = (this.transform.position - this.m_ObjectToRender.transform.position) with
  {
    z = 0.0f
  };

  private void FetchOutputRenderer()
  {
    if (!(bool) (Object) this.m_RenderToObject || (bool) (Object) this.m_outputRenderer)
      return;
    this.m_outputRenderer = this.m_RenderToObject.GetComponent<Renderer>();
    if (!(bool) (Object) this.m_outputRenderer)
      Debug.LogError((object) "RenderToObject should have a renderer!");
    else
      this.m_outputRenderer.enabled = false;
  }

  private void CaptureOriginalParents()
  {
    if ((bool) (Object) this.m_ObjectToRender && !(bool) (Object) this.m_objectToRenderOriginalParent)
      this.m_objectToRenderOriginalParent = this.m_ObjectToRender.transform.parent;
    if ((bool) (Object) this.m_selfOriginalParent)
      return;
    this.m_selfOriginalParent = this.transform.parent;
  }

  private void RegisterToService()
  {
    if (this.m_isRegisteredToManager || this.m_diamondRenderToTextureService == null || !(bool) (Object) this.m_ObjectToRender || !(bool) (Object) this.m_outputRenderer)
      return;
    bool flag = this.m_diamondRenderToTextureService.Register(this);
    if (flag)
      this.SetupAuxRenderObjects();
    this.m_isRegisteredToManager = flag;
  }

  private void UnregisterFromService()
  {
    if (!this.m_isRegisteredToManager)
      return;
    this.m_diamondRenderToTextureService.Unregister(this);
    this.m_isRegisteredToManager = false;
  }

  private void UpdatePlaneUVS(Rect atlasUV)
  {
    if (!(bool) (Object) this.m_RenderToObject)
      return;
    Mesh mesh = this.m_RenderToObject.GetComponent<MeshFilter>().mesh;
    Vector2[] uv = mesh.uv;
    Rect currentUvBounds = this.GetCurrentUVBounds(uv);
    Vector2 vector2_1 = new Vector2(atlasUV.width / currentUvBounds.width, atlasUV.height / currentUvBounds.height);
    Vector2 vector2_2 = new Vector2(atlasUV.xMin - currentUvBounds.xMin, atlasUV.yMin - currentUvBounds.yMin);
    for (int index = 0; index < uv.Length; ++index)
    {
      Vector2 vector2_3 = uv[index];
      vector2_3.x = vector2_3.x * vector2_1.x + vector2_2.x;
      vector2_3.y = vector2_3.y * vector2_1.y + vector2_2.y;
      uv[index] = vector2_3;
    }
    mesh.uv = uv;
  }

  private Rect GetCurrentUVBounds(Vector2[] currentUv)
  {
    Vector2 one = Vector2.one;
    Vector2 zero = Vector2.zero;
    foreach (Vector2 vector2 in currentUv)
    {
      if ((double) vector2.x < (double) one.x)
        one.x = vector2.x;
      if ((double) vector2.y < (double) one.y)
        one.y = vector2.y;
      if ((double) vector2.x > (double) zero.x)
        zero.x = vector2.x;
      if ((double) vector2.y > (double) zero.y)
        zero.y = vector2.y;
    }
    return new Rect(one.x, one.y, zero.x - one.x, zero.y - one.y);
  }

  private void UpdateMaterial(RenderTexture atlasTexture)
  {
    if (!(bool) (Object) this.m_outputRenderer)
      return;
    if (this.m_RenderMaterial == DiamondRenderToTexture.RenderToTextureMaterial.Transparent)
    {
      this.TransparentMaterial.mainTexture = (Texture) atlasTexture;
      this.m_outputRenderer.SetMaterial(this.TransparentMaterial);
      this.m_outputRenderer.enabled = true;
    }
    else
      this.m_outputRenderer.GetMaterial().mainTexture = (Texture) atlasTexture;
  }

  public void UpdateMaterialBlend(bool inPlay) => this.UpdateMaterialBlend(inPlay ? 1f : 0.0f);

  public void UpdateMaterialBlend(float blendValue) => this.TransparentMaterial.SetFloat("_LightingBlend", blendValue);

  private static void GizmosDrawArrow(
    Vector3 pos,
    Vector3 direction,
    Color color,
    float arrowHeadLength = 0.25f,
    float arrowHeadAngle = 20f)
  {
    Gizmos.color = color;
    Gizmos.DrawRay(pos, direction);
    Vector3 vector3_1 = Quaternion.LookRotation(direction) * Quaternion.Euler(0.0f, 180f + arrowHeadAngle, 0.0f) * new Vector3(0.0f, 0.0f, 1f);
    Vector3 vector3_2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0.0f, 180f - arrowHeadAngle, 0.0f) * new Vector3(0.0f, 0.0f, 1f);
    Gizmos.DrawRay(pos + direction, vector3_1 * arrowHeadLength);
    Gizmos.DrawRay(pos + direction, vector3_2 * arrowHeadLength);
  }

  public enum RenderToTextureMaterial
  {
    Custom,
    Transparent,
    Additive,
    Bloom,
    AlphaClip,
    AlphaClipBloom,
  }

  public struct TransformData
  {
    public Vector3 position;
    public Vector3 localScale;
    public Quaternion rotation;
    public Vector3 up;
    public Vector3 forward;
    public int layer;
    public Transform objectParent;
    public Transform atlasedComponentParent;
  }
}
