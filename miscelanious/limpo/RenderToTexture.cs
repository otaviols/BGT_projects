using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone.UI.Core;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderToTexture : MonoBehaviour, IPopupRendering
{
  private const string BLUR_SHADER_NAME = "Hidden/R2TBlur";
  private const string BLUR_ALPHA_SHADER_NAME = "Hidden/R2TAlphaBlur";
  private const string ALPHA_BLEND_SHADER_NAME = "Hidden/R2TColorAlphaCombine";
  private const string ALPHA_BLEND_ADD_SHADER_NAME = "Hidden/R2TColorAlphaCombineAdd";
  private const string ALPHA_FILL_SHADER_NAME = "Custom/AlphaFillOpaque";
  private const string BLOOM_SHADER_NAME = "Hidden/R2TBloom";
  private const string BLOOM_ALPHA_SHADER_NAME = "Hidden/R2TBloomAlpha";
  private const string ADDITIVE_SHADER_NAME = "Hidden/R2TAdditive";
  private const string TRANSPARENT_SHADER_NAME = "Hidden/R2TTransparent";
  private const string ALPHA_CLIP_SHADER_NAME = "Hidden/R2TAlphaClip";
  private const string ALPHA_CLIP_BLOOM_SHADER_NAME = "Hidden/R2TAlphaClipBloom";
  private const string ALPHA_CLIP_GRADIENT_SHADER_NAME = "Hidden/R2TAlphaClipGradient";
  private const RenderTextureFormat ALPHA_TEXTURE_FORMAT = RenderTextureFormat.R8;
  private const float OFFSET_DISTANCE = 300f;
  private const float MIN_OFFSET_DISTANCE = -4000f;
  private const float MAX_OFFSET_DISTANCE = -90000f;
  private readonly Vector3 ALPHA_OBJECT_OFFSET = new Vector3(0.0f, 1000f, 0.0f);
  private const float RENDER_SIZE_QUALITY_LOW = 0.75f;
  private const float RENDER_SIZE_QUALITY_MEDIUM = 1f;
  private const float RENDER_SIZE_QUALITY_HIGH = 1.5f;
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
  public GameObject m_ObjectToRender;
  public GameObject m_AlphaObjectToRender;
  public bool m_HideRenderObject = true;
  public bool m_RealtimeRender;
  public bool m_RealtimeTranslation;
  public bool m_RenderMeshAsAlpha;
  public bool m_OpaqueObjectAlphaFill;
  public RenderToTexture.RenderToTextureMaterial m_RenderMaterial;
  public Material m_Material;
  public bool m_CreateRenderPlane = true;
  public GameObject m_RenderToObject;
  public string m_ShaderTextureName = string.Empty;
  public int m_Resolution = 128;
  public float m_Width = 1f;
  public float m_Height = 1f;
  public float m_NearClip = -0.1f;
  public float m_FarClip = 0.5f;
  public float m_BloomIntensity;
  public float m_BloomThreshold = 0.7f;
  public float m_BloomBlur = 0.3f;
  public float m_BloomResolutionRatio = 0.5f;
  public RenderToTexture.BloomRenderType m_BloomRenderType;
  public float m_BloomAlphaIntensity = 1f;
  public RenderToTexture.BloomBlendType m_BloomBlend;
  public Color m_BloomColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
  public RenderToTexture.AlphaClipShader m_AlphaClipRenderStyle;
  public float m_AlphaClip = 15f;
  public float m_AlphaClipIntensity = 1.5f;
  public float m_AlphaClipAlphaIntensity = 1f;
  public Texture2D m_AlphaClipGradientMap;
  public float m_BlurAmount;
  public bool m_BlurAlphaOnly;
  public Color m_TintColor = Color.white;
  public int m_RenderQueueOffset = 3000;
  public int m_RenderQueue;
  public Color m_ClearColor = Color.clear;
  public Shader m_ReplacmentShader;
  public string m_ReplacmentTag;
  public string m_AlphaReplacementTag;
  public RenderTextureFormat m_RenderTextureFormat = RenderTextureFormat.Default;
  public Vector3 m_PositionOffset = Vector3.zero;
  public Vector3 m_CameraOffset = Vector3.zero;
  public LayerMask m_LayerMask = (LayerMask) -1;
  public bool m_UniformWorldScale;
  public float m_OverrideCameraSize;
  public bool m_LateUpdate;
  public bool m_RenderOnStart = true;
  public bool m_RenderOnEnable = true;
  private bool m_renderEnabled = true;
  private bool m_init;
  private float m_WorldWidth;
  private float m_WorldHeight;
  private Vector3 m_WorldScale;
  private GameObject m_OffscreenGameObject;
  private GameObject m_CameraGO;
  private RenderToTextureUtils.LightWeightCamera m_CameraData;
  private GameObject m_AlphaCameraGO;
  private RenderToTextureUtils.LightWeightCamera m_AlphaCameraData;
  private GameObject m_BloomCaptureCameraGO;
  private RenderToTextureUtils.LightWeightCamera m_BloomCameraData;
  private Camera m_Camera;
  private string m_RttCommandBufferName;
  private string m_BloomCommandbufferName;
  private RenderTexture m_RenderTexture;
  private RenderTexture m_BloomRenderTexture;
  private RenderTexture m_BloomRenderBuffer1;
  private RenderTexture m_BloomRenderBuffer2;
  private GameObject m_PlaneGameObject;
  private GameObject m_BloomPlaneGameObject;
  private GameObject m_BloomCapturePlaneGameObject;
  private bool m_ObjectToRenderOrgPositionStored;
  private Transform m_ObjectToRenderOrgParent;
  private Vector3 m_ObjectToRenderOrgPosition = Vector3.zero;
  private Vector3 m_OriginalRenderPosition = Vector3.zero;
  private bool m_isDirty;
  private Shader m_AlphaFillShader;
  private Vector3 m_OffscreenPos;
  private Vector3 m_ObjectToRenderOffset = Vector3.zero;
  private Vector3 m_AlphaObjectToRenderOffset = Vector3.zero;
  private RenderToTexture.RenderToTextureMaterial m_PreviousRenderMaterial;
  private int m_previousRenderQueue;
  private List<Renderer> m_OpaqueObjectAlphaFillTransparent;
  private List<UberText> m_OpaqueObjectAlphaFillUberText;
  private bool m_hasMaterialInstance;
  private static IMaterialService s_materialService;
  private RenderCommandLists.MatOverrideDictionary m_materialOverrides;
  private Vector3 m_Offset = Vector3.zero;
  private static Vector3 s_offset = new Vector3(-4000f, -4000f, -4000f);
  private IGraphicsManager m_graphicsManager;
  private static ProfilerMarker s_RenderTexInit = new ProfilerMarker("RTT_Init");
  private static ProfilerMarker s_RenderTex = new ProfilerMarker("RTT_RenderTex");
  private static ProfilerMarker s_RenderTexBloom = new ProfilerMarker("RTT_RenderBloom");
  private Shader m_AlphaBlendShader;
  private Material m_AlphaBlendMaterial;
  private Shader m_AlphaBlendAddShader;
  private Material m_AlphaBlendAddMaterial;
  private Shader m_AdditiveShader;
  private Material m_AdditiveMaterial;
  private Shader m_BloomShader;
  private Material m_BloomMaterial;
  private Shader m_BloomShaderAlpha;
  private Material m_BloomMaterialAlpha;
  private Shader m_BlurShader;
  private Material m_BlurMaterial;
  private Shader m_AlphaBlurShader;
  private Material m_AlphaBlurMaterial;
  private Shader m_TransparentShader;
  private Material m_TransparentMaterial;
  private Shader m_AlphaClipShader;
  private Material m_AlphaClipMaterial;
  private Shader m_AlphaClipBloomShader;
  private Material m_AlphaClipBloomMaterial;
  private Shader m_AlphaClipGradientShader;
  private Material m_AlphaClipGradientMaterial;
  private IPopupRoot m_popupRoot;
  private HashSet<IPopupRendering> m_popupRenderers = new HashSet<IPopupRendering>();

  public void SetMaterialOverrides(RenderCommandLists.MatOverrideDictionary arg) => this.m_materialOverrides = arg;

  public void ClearMaterialOverrides() => this.m_materialOverrides = (RenderCommandLists.MatOverrideDictionary) null;

  protected Vector3 Offset
  {
    get
    {
      if (this.m_Offset == Vector3.zero)
      {
        RenderToTexture.s_offset.x -= 300f;
        if ((double) RenderToTexture.s_offset.x < -90000.0)
        {
          RenderToTexture.s_offset.x = -4000f;
          RenderToTexture.s_offset.y -= 300f;
          if ((double) RenderToTexture.s_offset.y < -90000.0)
          {
            RenderToTexture.s_offset.y = -4000f;
            RenderToTexture.s_offset.z -= 300f;
            if ((double) RenderToTexture.s_offset.z < -90000.0)
              RenderToTexture.s_offset.z = -4000f;
          }
        }
        this.m_Offset = RenderToTexture.s_offset;
      }
      return this.m_Offset;
    }
  }

  protected Material AlphaBlendMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_AlphaBlendMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_AlphaBlendShader == (UnityEngine.Object) null)
        {
          this.m_AlphaBlendShader = ShaderUtils.FindShader("Hidden/R2TColorAlphaCombine");
          if (!(bool) (UnityEngine.Object) this.m_AlphaBlendShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TColorAlphaCombine");
        }
        this.m_AlphaBlendMaterial = new Material(this.m_AlphaBlendShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_AlphaBlendMaterial, HideFlags.DontSave);
      }
      return this.m_AlphaBlendMaterial;
    }
  }

  protected Material AlphaBlendAddMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_AlphaBlendAddMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_AlphaBlendAddShader == (UnityEngine.Object) null)
        {
          this.m_AlphaBlendAddShader = ShaderUtils.FindShader("Hidden/R2TColorAlphaCombineAdd");
          if (!(bool) (UnityEngine.Object) this.m_AlphaBlendAddShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TColorAlphaCombineAdd");
        }
        this.m_AlphaBlendAddMaterial = new Material(this.m_AlphaBlendAddShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_AlphaBlendAddMaterial, HideFlags.DontSave);
      }
      return this.m_AlphaBlendAddMaterial;
    }
  }

  protected Material AdditiveMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_AdditiveMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_AdditiveShader == (UnityEngine.Object) null)
        {
          this.m_AdditiveShader = ShaderUtils.FindShader("Hidden/R2TAdditive");
          if (!(bool) (UnityEngine.Object) this.m_AdditiveShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TAdditive");
        }
        this.m_AdditiveMaterial = new Material(this.m_AdditiveShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_AdditiveMaterial, HideFlags.DontSave);
      }
      return this.m_AdditiveMaterial;
    }
  }

  protected Material BloomMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_BloomMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_BloomShader == (UnityEngine.Object) null)
        {
          this.m_BloomShader = ShaderUtils.FindShader("Hidden/R2TBloom");
          if (!(bool) (UnityEngine.Object) this.m_BloomShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TBloom");
        }
        this.m_BloomMaterial = new Material(this.m_BloomShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_BloomMaterial, HideFlags.DontSave);
      }
      return this.m_BloomMaterial;
    }
  }

  protected Material BloomMaterialAlpha
  {
    get
    {
      if ((UnityEngine.Object) this.m_BloomMaterialAlpha == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_BloomShaderAlpha == (UnityEngine.Object) null)
        {
          this.m_BloomShaderAlpha = ShaderUtils.FindShader("Hidden/R2TBloomAlpha");
          if (!(bool) (UnityEngine.Object) this.m_BloomShaderAlpha)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TBloomAlpha");
        }
        this.m_BloomMaterialAlpha = new Material(this.m_BloomShaderAlpha);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_BloomMaterialAlpha, HideFlags.DontSave);
      }
      return this.m_BloomMaterialAlpha;
    }
  }

  protected Material BlurMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_BlurMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_BlurShader == (UnityEngine.Object) null)
        {
          this.m_BlurShader = ShaderUtils.FindShader("Hidden/R2TBlur");
          if (!(bool) (UnityEngine.Object) this.m_BlurShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TBlur");
        }
        this.m_BlurMaterial = new Material(this.m_BlurShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_BlurMaterial, HideFlags.DontSave);
      }
      return this.m_BlurMaterial;
    }
  }

  protected Material AlphaBlurMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_AlphaBlurMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_AlphaBlurShader == (UnityEngine.Object) null)
        {
          this.m_AlphaBlurShader = ShaderUtils.FindShader("Hidden/R2TAlphaBlur");
          if (!(bool) (UnityEngine.Object) this.m_AlphaBlurShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TAlphaBlur");
        }
        this.m_AlphaBlurMaterial = new Material(this.m_AlphaBlurShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_AlphaBlurMaterial, HideFlags.DontSave);
      }
      return this.m_AlphaBlurMaterial;
    }
  }

  protected Material TransparentMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_TransparentMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_TransparentShader == (UnityEngine.Object) null)
        {
          this.m_TransparentShader = ShaderUtils.FindShader("Hidden/R2TTransparent");
          if (!(bool) (UnityEngine.Object) this.m_TransparentShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TTransparent");
        }
        this.m_TransparentMaterial = new Material(this.m_TransparentShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_TransparentMaterial, HideFlags.DontSave);
      }
      return this.m_TransparentMaterial;
    }
  }

  protected Material AlphaClipMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_AlphaClipMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_AlphaClipShader == (UnityEngine.Object) null)
        {
          this.m_AlphaClipShader = ShaderUtils.FindShader("Hidden/R2TAlphaClip");
          if (!(bool) (UnityEngine.Object) this.m_AlphaClipShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TAlphaClip");
        }
        this.m_AlphaClipMaterial = new Material(this.m_AlphaClipShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_AlphaClipMaterial, HideFlags.DontSave);
      }
      return this.m_AlphaClipMaterial;
    }
  }

  protected Material AlphaClipBloomMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_AlphaClipBloomMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_AlphaClipBloomShader == (UnityEngine.Object) null)
        {
          this.m_AlphaClipBloomShader = ShaderUtils.FindShader("Hidden/R2TAlphaClipBloom");
          if (!(bool) (UnityEngine.Object) this.m_AlphaClipBloomShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TAlphaClipBloom");
        }
        this.m_AlphaClipBloomMaterial = new Material(this.m_AlphaClipBloomShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_AlphaClipBloomMaterial, HideFlags.DontSave);
      }
      return this.m_AlphaClipBloomMaterial;
    }
  }

  protected Material AlphaClipGradientMaterial
  {
    get
    {
      if ((UnityEngine.Object) this.m_AlphaClipGradientMaterial == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_AlphaClipGradientShader == (UnityEngine.Object) null)
        {
          this.m_AlphaClipGradientShader = ShaderUtils.FindShader("Hidden/R2TAlphaClipGradient");
          if (!(bool) (UnityEngine.Object) this.m_AlphaClipGradientShader)
            Debug.LogError((object) "Failed to load RenderToTexture Shader: Hidden/R2TAlphaClipGradient");
        }
        this.m_AlphaClipGradientMaterial = new Material(this.m_AlphaClipGradientShader);
        GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_AlphaClipGradientMaterial, HideFlags.DontSave);
      }
      return this.m_AlphaClipGradientMaterial;
    }
  }

  public bool DontRefreshOnFocus { set; get; }

  private void Awake()
  {
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.m_AlphaFillShader = ShaderUtils.FindShader("Custom/AlphaFillOpaque");
    if (!(bool) (UnityEngine.Object) this.m_AlphaFillShader)
      Debug.LogError((object) "Failed to load RenderToTexture Shader: Custom/AlphaFillOpaque");
    this.m_OffscreenPos = this.Offset;
    if (!((UnityEngine.Object) this.m_Material != (UnityEngine.Object) null))
      return;
    this.m_Material = UnityEngine.Object.Instantiate<Material>(this.m_Material);
    this.m_hasMaterialInstance = true;
    RenderToTexture.GetMaterialService().IgnoreMaterial(this.m_Material);
  }

  private void Start()
  {
    if (this.m_RenderOnStart)
      this.m_isDirty = true;
    this.Init();
  }

  private void Update()
  {
    if (!this.m_renderEnabled)
      return;
    if ((bool) (UnityEngine.Object) this.m_RenderTexture && !this.m_RenderTexture.IsCreated())
    {
      Log.Graphics.Print("RenderToTexture Texture lost. Render Called");
      this.m_isDirty = true;
      this.RenderTex();
    }
    else
    {
      if (this.m_LateUpdate)
        return;
      if (this.m_HideRenderObject && (bool) (UnityEngine.Object) this.m_ObjectToRender)
        this.PositionHiddenObjectsAndCameras();
      if (!this.m_RealtimeRender && !this.m_isDirty)
        return;
      this.RenderTex();
    }
  }

  private void LateUpdate()
  {
    if (!this.m_renderEnabled)
      return;
    if (this.m_LateUpdate)
    {
      if (this.m_HideRenderObject && (bool) (UnityEngine.Object) this.m_ObjectToRender)
        this.PositionHiddenObjectsAndCameras();
      if (!this.m_RealtimeRender && !this.m_isDirty)
        return;
      this.RenderTex();
    }
    else if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom || this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.Bloom)
    {
      this.RenderBloom();
    }
    else
    {
      if (!(bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_BloomPlaneGameObject);
    }
  }

  private void OnApplicationFocus(bool state)
  {
    if (this.DontRefreshOnFocus || !((bool) (UnityEngine.Object) this.m_RenderTexture & state))
      return;
    this.m_isDirty = true;
    this.RenderTex();
  }

  private void OnDrawGizmos()
  {
    if (!this.enabled)
      return;
    if ((double) this.m_FarClip < 0.0)
      this.m_FarClip = 0.0f;
    if ((double) this.m_NearClip > 0.0)
      this.m_NearClip = 0.0f;
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Vector3 vector3 = new Vector3(0.0f, (float) (-(double) this.m_NearClip * 0.5), 0.0f);
    Gizmos.color = new Color(0.1f, 0.5f, 0.7f, 0.8f);
    Vector3 positionOffset = this.m_PositionOffset;
    Gizmos.DrawWireCube(vector3 + positionOffset, new Vector3(this.m_Width, -this.m_NearClip, this.m_Height));
    Gizmos.color = new Color(0.2f, 0.2f, 0.9f, 0.8f);
    Gizmos.DrawWireCube(new Vector3(0.0f, (float) (-(double) this.m_FarClip * 0.5), 0.0f) + this.m_PositionOffset, new Vector3(this.m_Width, -this.m_FarClip, this.m_Height));
    Gizmos.color = new Color(0.8f, 0.8f, 1f, 1f);
    Gizmos.DrawWireCube(this.m_PositionOffset, new Vector3(this.m_Width, 0.0f, this.m_Height));
    Gizmos.matrix = Matrix4x4.identity;
  }

  private void OnDisable()
  {
    this.RestoreAfterRender();
    if ((bool) (UnityEngine.Object) this.m_ObjectToRender)
    {
      if ((UnityEngine.Object) this.m_ObjectToRenderOrgParent != (UnityEngine.Object) null)
        this.m_ObjectToRender.transform.parent = this.m_ObjectToRenderOrgParent;
      this.m_ObjectToRender.transform.localPosition = this.m_ObjectToRenderOrgPosition;
    }
    if ((bool) (UnityEngine.Object) this.m_PlaneGameObject)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_PlaneGameObject);
    if ((bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_BloomPlaneGameObject);
    if ((bool) (UnityEngine.Object) this.m_BloomCapturePlaneGameObject)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_BloomCapturePlaneGameObject);
    if ((bool) (UnityEngine.Object) this.m_BloomCaptureCameraGO)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_BloomCaptureCameraGO);
    this.ReleaseTexture();
    this.m_init = false;
    this.m_isDirty = true;
  }

  private void OnDestroy() => this.CleanUp();

  private void OnEnable()
  {
    if (!this.m_RenderOnEnable)
      return;
    this.RenderTex();
  }

  public RenderTexture Render()
  {
    this.m_isDirty = true;
    return this.m_RenderTexture;
  }

  public RenderTexture RenderNow()
  {
    this.RenderTex();
    return this.m_RenderTexture;
  }

  public void ForceTextureRebuild()
  {
    if (!this.enabled)
      return;
    this.ReleaseTexture();
    this.m_isDirty = true;
    this.RenderTex();
  }

  public void Show() => this.Show(false);

  public void Show(bool render)
  {
    this.m_renderEnabled = true;
    if ((bool) (UnityEngine.Object) this.m_RenderToObject)
      this.m_RenderToObject.GetComponent<Renderer>().enabled = true;
    else if ((bool) (UnityEngine.Object) this.m_PlaneGameObject)
    {
      this.m_PlaneGameObject.GetComponent<Renderer>().enabled = true;
      if ((bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
        this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = true;
    }
    if (!render)
      return;
    this.Render();
  }

  public void Hide()
  {
    this.m_renderEnabled = false;
    if ((bool) (UnityEngine.Object) this.m_RenderToObject)
    {
      this.m_RenderToObject.GetComponent<Renderer>().enabled = false;
    }
    else
    {
      if (!(bool) (UnityEngine.Object) this.m_PlaneGameObject)
        return;
      this.m_PlaneGameObject.GetComponent<Renderer>().enabled = false;
      if (!(bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
        return;
      this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = false;
    }
  }

  public void SetDirty()
  {
    this.m_init = false;
    this.m_isDirty = true;
  }

  public Material GetRenderMaterial()
  {
    if ((bool) (UnityEngine.Object) this.m_RenderToObject)
      return this.m_RenderToObject.GetComponent<Renderer>().GetMaterial();
    return (bool) (UnityEngine.Object) this.m_PlaneGameObject ? this.m_PlaneGameObject.GetComponent<Renderer>().GetMaterial() : this.m_Material;
  }

  public GameObject GetRenderToObject() => (bool) (UnityEngine.Object) this.m_RenderToObject ? this.m_RenderToObject : this.m_PlaneGameObject;

  public RenderTexture GetRenderTexture() => this.m_RenderTexture;

  public Vector3 GetOffscreenPosition() => this.m_OffscreenPos;

  public Vector3 GetOffscreenPositionOffset() => this.m_OffscreenPos - this.transform.position;

  private void Init()
  {
    if (this.m_init)
      return;
    if (this.m_RealtimeTranslation)
    {
      this.m_OffscreenGameObject = new GameObject();
      this.m_OffscreenGameObject.name = string.Format("R2TOffsetRenderRoot_{0}", (object) this.name);
      this.m_OffscreenGameObject.transform.position = this.transform.position;
    }
    if ((bool) (UnityEngine.Object) this.m_ObjectToRender)
    {
      if (!this.m_ObjectToRenderOrgPositionStored)
      {
        this.m_ObjectToRenderOrgParent = this.m_ObjectToRender.transform.parent;
        this.m_ObjectToRenderOrgPosition = this.m_ObjectToRender.transform.localPosition;
        this.m_ObjectToRenderOrgPositionStored = true;
      }
      if (this.m_HideRenderObject)
      {
        if (this.m_RealtimeTranslation)
        {
          this.m_ObjectToRender.transform.parent = this.m_OffscreenGameObject.transform;
          if ((bool) (UnityEngine.Object) this.m_AlphaObjectToRender)
            this.m_AlphaObjectToRender.transform.parent = this.m_OffscreenGameObject.transform;
        }
        this.m_OriginalRenderPosition = !(bool) (UnityEngine.Object) this.m_RenderToObject ? this.transform.position : this.m_RenderToObject.transform.position;
        if ((bool) (UnityEngine.Object) this.m_ObjectToRender && this.m_ObjectToRenderOffset == Vector3.zero)
          this.m_ObjectToRenderOffset = this.transform.position - this.m_ObjectToRender.transform.position;
        if ((bool) (UnityEngine.Object) this.m_AlphaObjectToRender && this.m_AlphaObjectToRenderOffset == Vector3.zero)
          this.m_AlphaObjectToRenderOffset = this.transform.position - this.m_AlphaObjectToRender.transform.position;
      }
    }
    else if (!this.m_ObjectToRenderOrgPositionStored)
    {
      this.m_ObjectToRenderOrgPosition = this.transform.localPosition;
      if ((UnityEngine.Object) this.m_OffscreenGameObject != (UnityEngine.Object) null)
        this.m_OffscreenGameObject.transform.position = this.transform.position;
      this.m_ObjectToRenderOrgPositionStored = true;
    }
    if (this.m_HideRenderObject)
    {
      if (this.m_RealtimeTranslation)
      {
        if ((UnityEngine.Object) this.m_OffscreenGameObject != (UnityEngine.Object) null)
          this.m_OffscreenGameObject.transform.position = this.m_OffscreenPos;
      }
      else if ((bool) (UnityEngine.Object) this.m_ObjectToRender)
        this.m_ObjectToRender.transform.position = this.m_OffscreenPos;
      else
        this.transform.position = this.m_OffscreenPos;
    }
    if ((UnityEngine.Object) this.m_ObjectToRender == (UnityEngine.Object) null)
      this.m_ObjectToRender = this.gameObject;
    this.CalcWorldWidthHeightScale();
    this.CreateTexture();
    this.CreateCamera();
    if (this.m_OpaqueObjectAlphaFill || this.m_RenderMeshAsAlpha || (UnityEngine.Object) this.m_AlphaObjectToRender != (UnityEngine.Object) null)
      this.CreateAlphaCamera();
    if (!(bool) (UnityEngine.Object) this.m_RenderToObject && this.m_CreateRenderPlane)
      this.CreateRenderPlane();
    if ((bool) (UnityEngine.Object) this.m_RenderToObject)
      this.m_RenderToObject.GetComponent<Renderer>().GetMaterial().renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue;
    this.SetupMaterial();
    this.m_RttCommandBufferName = "RenderToTexture " + this.name;
    this.m_BloomCommandbufferName = "RenderToTexture Bloom " + this.name;
    this.m_init = true;
  }

  private void RenderTex()
  {
    if (!this.m_renderEnabled)
      return;
    this.Init();
    if (!this.m_init)
      return;
    this.SetupForRender();
    if (this.m_RenderMaterial != this.m_PreviousRenderMaterial || this.m_RenderQueue != this.m_previousRenderQueue)
      this.SetupMaterial();
    if (this.m_HideRenderObject && (bool) (UnityEngine.Object) this.m_ObjectToRender)
      this.PositionHiddenObjectsAndCameras();
    if ((bool) (UnityEngine.Object) this.m_PlaneGameObject && !this.m_HideRenderObject)
    {
      this.m_PlaneGameObject.GetComponent<Renderer>().enabled = false;
      if ((bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
        this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = false;
    }
    int num = this.m_OpaqueObjectAlphaFill ? 1 : (this.m_RenderMeshAsAlpha ? 1 : ((UnityEngine.Object) this.m_AlphaObjectToRender != (UnityEngine.Object) null ? 1 : 0));
    bool flag = (double) this.m_BlurAmount > 0.0;
    RenderCommandLists renderCommandLists1 = RenderToTextureUtils.RenderCommandListPool.Get(this.m_ObjectToRender, overrides: this.m_materialOverrides);
    RenderCommandLists renderCommandLists2 = (UnityEngine.Object) this.m_AlphaObjectToRender == (UnityEngine.Object) null ? (RenderCommandLists) null : RenderToTextureUtils.RenderCommandListPool.Get(this.m_AlphaObjectToRender, overrides: this.m_materialOverrides);
    CommandBuffer commandBuffer = CommandBufferPool.Get(this.m_RttCommandBufferName);
    RenderTexture temporary = RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, this.m_RenderTexture.depth, this.m_RenderTexture.format);
    RenderTexture renderTexture1 = num != 0 ? RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, 16, RenderTextureFormat.R8) : (RenderTexture) null;
    RenderTexture renderTexture2 = flag ? RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, this.m_RenderTexture.depth, this.m_RenderTexture.format) : (RenderTexture) null;
    this.m_RenderTexture.DiscardContents();
    this.m_CameraData.SetOrthoProjectionMatrix(this.OrthoSize(), this.m_NearClip * this.m_WorldScale.z, this.m_FarClip * this.m_WorldScale.z);
    this.m_CameraData.SetWorldToCameraMatrix(this.m_CameraGO.transform);
    this.m_Camera.orthographicSize = this.OrthoSize();
    this.m_Camera.farClipPlane = this.m_FarClip * this.m_WorldScale.z;
    this.m_Camera.nearClipPlane = this.m_NearClip * this.m_WorldScale.z;
    Camera.SetupCurrent(this.m_Camera);
    if (num != 0)
    {
      RenderToTextureUtils.RenderCamera(commandBuffer, temporary, this.m_CameraData, renderCommandLists1, this.m_ReplacmentShader, this.m_ReplacmentTag);
      this.m_AlphaCameraData.SetOrthoProjectionMatrix(this.OrthoSize(), this.m_NearClip * this.m_WorldScale.z, this.m_FarClip * this.m_WorldScale.z);
      this.m_AlphaCameraData.SetWorldToCameraMatrix(this.m_AlphaCameraGO.transform);
      this.AlphaCameraRender(commandBuffer, renderTexture1, this.m_AlphaCameraData, renderCommandLists1, renderCommandLists2);
      if (this.m_OpaqueObjectAlphaFill)
        this.AlphaBlendAddMaterial.SetTexture("_AlphaTex", (Texture) renderTexture1);
      else
        this.AlphaBlendMaterial.SetTexture("_AlphaTex", (Texture) renderTexture1);
      if (flag)
      {
        if (this.m_OpaqueObjectAlphaFill)
          commandBuffer.Blit((Texture) temporary, (RenderTargetIdentifier) (Texture) renderTexture2, this.AlphaBlendAddMaterial);
        else
          commandBuffer.Blit((Texture) temporary, (RenderTargetIdentifier) (Texture) renderTexture2, this.AlphaBlendMaterial);
        Material mat = this.m_BlurAlphaOnly ? this.BlurMaterial : this.AlphaBlurMaterial;
        mat.SetVector("_BlurOffsets", new Vector4(this.m_BlurAmount, this.m_BlurAmount, this.m_BlurAmount, this.m_BlurAmount));
        mat.SetVector("_MainTex_TexelSize", new Vector4(1f / (float) renderTexture2.width, 1f / (float) renderTexture2.height, 0.0f, 0.0f));
        commandBuffer.Blit((Texture) renderTexture2, (RenderTargetIdentifier) (Texture) this.m_RenderTexture, mat);
      }
      else if (this.m_OpaqueObjectAlphaFill)
        commandBuffer.Blit((Texture) temporary, (RenderTargetIdentifier) (Texture) this.m_RenderTexture, this.AlphaBlendAddMaterial);
      else
        commandBuffer.Blit((Texture) temporary, (RenderTargetIdentifier) (Texture) this.m_RenderTexture, this.AlphaBlendMaterial);
    }
    else if (flag)
    {
      RenderToTextureUtils.RenderCamera(commandBuffer, renderTexture2, this.m_CameraData, renderCommandLists1, this.m_ReplacmentShader, this.m_ReplacmentTag);
      Material mat = this.BlurMaterial;
      if (this.m_BlurAlphaOnly)
        mat = this.m_AlphaBlurMaterial;
      mat.SetVector("_BlurOffsets", new Vector4(this.m_BlurAmount, this.m_BlurAmount, this.m_BlurAmount, this.m_BlurAmount));
      mat.SetVector("_MainTex_TexelSize", new Vector4(1f / (float) renderTexture2.width, 1f / (float) renderTexture2.height, 0.0f, 0.0f));
      commandBuffer.Blit((Texture) renderTexture2, (RenderTargetIdentifier) (Texture) this.m_RenderTexture, mat);
    }
    else
      RenderToTextureUtils.RenderCamera(commandBuffer, this.m_RenderTexture, this.m_CameraData, renderCommandLists1, this.m_ReplacmentShader, this.m_ReplacmentTag);
    Graphics.ExecuteCommandBuffer(commandBuffer);
    CommandBufferPool.Release(commandBuffer);
    RenderToTextureUtils.RenderCommandListPool.Release(renderCommandLists1);
    RenderToTextureUtils.RenderCommandListPool.Release(renderCommandLists2);
    RenderTexture.ReleaseTemporary(temporary);
    if ((bool) (UnityEngine.Object) renderTexture1)
      RenderTexture.ReleaseTemporary(renderTexture1);
    if ((bool) (UnityEngine.Object) renderTexture2)
      RenderTexture.ReleaseTemporary(renderTexture2);
    if ((bool) (UnityEngine.Object) this.m_RenderToObject)
    {
      Renderer renderer = this.m_RenderToObject.GetComponent<Renderer>();
      if ((UnityEngine.Object) renderer == (UnityEngine.Object) null)
        renderer = this.m_RenderToObject.GetComponentInChildren<Renderer>();
      if (this.m_ShaderTextureName != string.Empty)
        renderer.GetMaterial().SetTexture(this.m_ShaderTextureName, (Texture) this.m_RenderTexture);
      else
        renderer.GetMaterial().mainTexture = (Texture) this.m_RenderTexture;
    }
    else if ((bool) (UnityEngine.Object) this.m_PlaneGameObject)
    {
      if (this.m_ShaderTextureName != string.Empty)
        this.m_PlaneGameObject.GetComponent<Renderer>().GetMaterial().SetTexture(this.m_ShaderTextureName, (Texture) this.m_RenderTexture);
      else
        this.m_PlaneGameObject.GetComponent<Renderer>().GetMaterial().mainTexture = (Texture) this.m_RenderTexture;
    }
    if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClip || this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom)
    {
      GameObject gameObject = this.m_PlaneGameObject;
      if ((bool) (UnityEngine.Object) this.m_RenderToObject)
        gameObject = this.m_RenderToObject;
      Material material = gameObject.GetComponent<Renderer>().GetMaterial();
      material.SetFloat("_Cutoff", this.m_AlphaClip);
      material.SetFloat("_Intensity", this.m_AlphaClipIntensity);
      material.SetFloat("_AlphaIntensity", this.m_AlphaClipAlphaIntensity);
      if (this.m_AlphaClipRenderStyle == RenderToTexture.AlphaClipShader.ColorGradient)
        material.SetTexture("_GradientTex", (Texture) this.m_AlphaClipGradientMap);
    }
    if ((bool) (UnityEngine.Object) this.m_PlaneGameObject && !this.m_HideRenderObject)
    {
      this.m_PlaneGameObject.GetComponent<Renderer>().enabled = true;
      if ((bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
        this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = true;
    }
    if (!this.m_RealtimeRender)
      this.RestoreAfterRender();
    if (this.m_popupRoot != null && ((UnityEngine.Object) this.m_PlaneGameObject != (UnityEngine.Object) null || (UnityEngine.Object) this.m_BloomPlaneGameObject != (UnityEngine.Object) null || (UnityEngine.Object) this.m_BloomCapturePlaneGameObject != (UnityEngine.Object) null))
      this.m_popupRoot.ApplyPopupRendering(this.transform, this.m_popupRenderers, true, this.gameObject.layer);
    this.m_isDirty = false;
    Camera.SetupCurrent(CameraUtils.GetMainCamera());
  }

  private void RenderBloom()
  {
    if ((double) this.m_BloomIntensity == 0.0)
    {
      if (!(bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_BloomPlaneGameObject);
    }
    else if ((double) this.m_BloomIntensity == 0.0)
    {
      if (!(bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_BloomPlaneGameObject);
    }
    else
    {
      Camera.SetupCurrent(Camera.main);
      int width = (int) ((double) this.m_RenderTexture.width * (double) Mathf.Clamp01(this.m_BloomResolutionRatio));
      int height = (int) ((double) this.m_RenderTexture.height * (double) Mathf.Clamp01(this.m_BloomResolutionRatio));
      RenderTexture renderTexture = this.m_RenderTexture;
      if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom)
      {
        if (!(bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
          this.CreateBloomPlane();
        if (!(bool) (UnityEngine.Object) this.m_BloomRenderTexture)
          this.m_BloomRenderTexture = RenderTextureTracker.Get().CreateNewTexture(width, height, RenderTextureTracker.TEXTURE_DEPTH, RenderTextureFormat.ARGB32);
      }
      if (!(bool) (UnityEngine.Object) this.m_BloomRenderBuffer1)
        this.m_BloomRenderBuffer1 = RenderTextureTracker.Get().CreateNewTexture(width, height, RenderTextureTracker.TEXTURE_DEPTH, RenderTextureFormat.ARGB32);
      if (!(bool) (UnityEngine.Object) this.m_BloomRenderBuffer2)
        this.m_BloomRenderBuffer2 = RenderTextureTracker.Get().CreateNewTexture(width, height, RenderTextureTracker.TEXTURE_DEPTH, RenderTextureFormat.ARGB32);
      Material mat = this.BloomMaterial;
      if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom)
      {
        mat = this.AlphaClipBloomMaterial;
        renderTexture = this.m_BloomRenderTexture;
        if (!(bool) (UnityEngine.Object) this.m_BloomCaptureCameraGO)
          this.CreateBloomCaptureCamera();
        this.m_BloomCameraData.SetWorldToCameraMatrix(this.m_BloomCaptureCameraGO.transform);
        mat.SetFloat("_Cutoff", this.m_AlphaClip);
        mat.SetFloat("_Intensity", this.m_AlphaClipIntensity);
        mat.SetFloat("_AlphaIntensity", this.m_AlphaClipAlphaIntensity);
        RenderCommandLists renderCommandLists = RenderToTextureUtils.RenderCommandListPool.Get(this.m_ObjectToRender, overrides: this.m_materialOverrides);
        CommandBuffer commandBuffer = CommandBufferPool.Get(this.m_BloomCommandbufferName);
        RenderToTextureUtils.RenderCamera(commandBuffer, renderTexture, this.m_BloomCameraData, renderCommandLists);
        Graphics.ExecuteCommandBuffer(commandBuffer);
        CommandBufferPool.Release(commandBuffer);
        RenderToTextureUtils.RenderCommandListPool.Release(renderCommandLists);
      }
      if (this.m_BloomRenderType == RenderToTexture.BloomRenderType.Alpha)
      {
        mat = this.BloomMaterialAlpha;
        mat.SetFloat("_AlphaIntensity", this.m_BloomAlphaIntensity);
      }
      float num1 = 1f / (float) width;
      float num2 = 1f / (float) height;
      mat.SetFloat("_Threshold", this.m_BloomThreshold);
      mat.SetFloat("_Intensity", this.m_BloomIntensity / (1f - this.m_BloomThreshold));
      mat.SetVector("_OffsetA", new Vector4(1.5f * num1, 1.5f * num2, -1.5f * num1, 1.5f * num2));
      mat.SetVector("_OffsetB", new Vector4(-1.5f * num1, -1.5f * num2, 1.5f * num1, -1.5f * num2));
      this.m_BloomRenderBuffer2.DiscardContents();
      Graphics.Blit((Texture) renderTexture, this.m_BloomRenderBuffer2, mat, 1);
      float num3 = num1 * (4f * this.m_BloomBlur);
      float num4 = num2 * (4f * this.m_BloomBlur);
      mat.SetVector("_OffsetA", new Vector4(1.5f * num3, 0.0f, -1.5f * num3, 0.0f));
      mat.SetVector("_OffsetB", new Vector4(0.5f * num3, 0.0f, -0.5f * num3, 0.0f));
      this.m_BloomRenderBuffer1.DiscardContents();
      Graphics.Blit((Texture) this.m_BloomRenderBuffer2, this.m_BloomRenderBuffer1, mat, 2);
      mat.SetVector("_OffsetA", new Vector4(0.0f, 1.5f * num4, 0.0f, -1.5f * num4));
      mat.SetVector("_OffsetB", new Vector4(0.0f, 0.5f * num4, 0.0f, -0.5f * num4));
      renderTexture.DiscardContents();
      Graphics.Blit((Texture) this.m_BloomRenderBuffer1, renderTexture, mat, 2);
      Material material1 = this.m_PlaneGameObject.GetComponent<Renderer>().GetMaterial();
      if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom)
      {
        Material material2 = this.m_BloomPlaneGameObject.GetComponent<Renderer>().GetMaterial();
        material2.color = this.m_BloomColor;
        material2.mainTexture = (Texture) renderTexture;
        if (!(bool) (UnityEngine.Object) this.m_PlaneGameObject)
          return;
        material2.renderQueue = material1.renderQueue + 1;
      }
      else if ((bool) (UnityEngine.Object) this.m_RenderToObject)
      {
        Material material3 = this.m_RenderToObject.GetComponent<Renderer>().GetMaterial();
        material3.color = this.m_BloomColor;
        material3.mainTexture = (Texture) renderTexture;
      }
      else
      {
        material1.color = this.m_BloomColor;
        material1.mainTexture = (Texture) renderTexture;
      }
    }
  }

  private void SetupForRender()
  {
    this.CalcWorldWidthHeightScale();
    if (!(bool) (UnityEngine.Object) this.m_RenderTexture)
      this.CreateTexture();
    if (!this.m_HideRenderObject)
      return;
    if ((bool) (UnityEngine.Object) this.m_PlaneGameObject)
    {
      this.m_PlaneGameObject.transform.localPosition = this.m_PositionOffset;
      this.m_PlaneGameObject.layer = this.gameObject.layer;
    }
    this.m_CameraData.backgroundColor = this.m_ClearColor;
  }

  private void SetupMaterial()
  {
    GameObject gameObject = this.m_PlaneGameObject;
    if ((bool) (UnityEngine.Object) this.m_RenderToObject)
    {
      gameObject = this.m_RenderToObject;
      if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.Custom)
        return;
    }
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return;
    Renderer component1 = gameObject.GetComponent<Renderer>();
    switch (this.m_RenderMaterial)
    {
      case RenderToTexture.RenderToTextureMaterial.Transparent:
        component1.SetMaterial(this.TransparentMaterial);
        break;
      case RenderToTexture.RenderToTextureMaterial.Additive:
        component1.SetMaterial(this.AdditiveMaterial);
        break;
      case RenderToTexture.RenderToTextureMaterial.Bloom:
        if (this.m_BloomBlend == RenderToTexture.BloomBlendType.Additive)
        {
          component1.SetMaterial(this.AdditiveMaterial);
          break;
        }
        if (this.m_BloomBlend == RenderToTexture.BloomBlendType.Transparent)
        {
          component1.SetMaterial(this.TransparentMaterial);
          break;
        }
        break;
      case RenderToTexture.RenderToTextureMaterial.AlphaClip:
        Material material1 = this.m_AlphaClipRenderStyle != RenderToTexture.AlphaClipShader.Standard ? this.AlphaClipGradientMaterial : this.AlphaClipMaterial;
        component1.SetMaterial(material1);
        material1.SetFloat("_Cutoff", this.m_AlphaClip);
        material1.SetFloat("_Intensity", this.m_AlphaClipIntensity);
        material1.SetFloat("_AlphaIntensity", this.m_AlphaClipAlphaIntensity);
        if (this.m_AlphaClipRenderStyle == RenderToTexture.AlphaClipShader.ColorGradient)
        {
          material1.SetTexture("_GradientTex", (Texture) this.m_AlphaClipGradientMap);
          break;
        }
        break;
      case RenderToTexture.RenderToTextureMaterial.AlphaClipBloom:
        Material material2 = this.m_AlphaClipRenderStyle != RenderToTexture.AlphaClipShader.Standard ? this.AlphaClipGradientMaterial : this.AlphaClipMaterial;
        component1.SetMaterial(material2);
        material2.SetFloat("_Cutoff", this.m_AlphaClip);
        material2.SetFloat("_Intensity", this.m_AlphaClipIntensity);
        material2.SetFloat("_AlphaIntensity", this.m_AlphaClipAlphaIntensity);
        if (this.m_AlphaClipRenderStyle == RenderToTexture.AlphaClipShader.ColorGradient)
        {
          material2.SetTexture("_GradientTex", (Texture) this.m_AlphaClipGradientMap);
          break;
        }
        break;
      default:
        if ((UnityEngine.Object) this.m_Material != (UnityEngine.Object) null)
        {
          component1.SetMaterial(this.m_Material);
          break;
        }
        break;
    }
    Material material3 = component1.GetMaterial();
    if ((UnityEngine.Object) material3 != (UnityEngine.Object) null)
      material3.color *= this.m_TintColor;
    if ((double) this.m_BloomIntensity > 0.0 && (bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
      this.m_BloomPlaneGameObject.GetComponent<Renderer>().GetMaterial().color = this.m_BloomColor;
    component1.sortingOrder = this.m_RenderQueue;
    material3.renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue;
    if ((bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
    {
      Renderer component2 = this.m_BloomPlaneGameObject.GetComponent<Renderer>();
      component2.sortingOrder = this.m_RenderQueue + 1;
      component2.GetMaterial().renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue + 1;
    }
    this.m_PreviousRenderMaterial = this.m_RenderMaterial;
    this.m_previousRenderQueue = this.m_RenderQueue;
  }

  private void PositionHiddenObjectsAndCameras()
  {
    Vector3 zero = Vector3.zero;
    Vector3 vector3 = !(bool) (UnityEngine.Object) this.m_RenderToObject ? this.transform.position - this.m_OriginalRenderPosition : this.m_RenderToObject.transform.position - this.m_OriginalRenderPosition;
    if (this.m_RealtimeTranslation)
    {
      this.m_OffscreenGameObject.transform.position = this.m_OffscreenPos + vector3;
      this.m_OffscreenGameObject.transform.rotation = this.transform.rotation;
      if (!(bool) (UnityEngine.Object) this.m_AlphaObjectToRender)
        return;
      this.m_AlphaObjectToRender.transform.position = this.m_OffscreenPos - this.ALPHA_OBJECT_OFFSET - this.m_AlphaObjectToRenderOffset + vector3;
      this.m_AlphaObjectToRender.transform.rotation = this.transform.rotation;
    }
    else
    {
      if ((bool) (UnityEngine.Object) this.m_ObjectToRender)
      {
        this.m_ObjectToRender.transform.rotation = Quaternion.identity;
        this.m_ObjectToRender.transform.position = this.m_OffscreenPos - this.m_ObjectToRenderOffset + this.m_PositionOffset;
        this.m_ObjectToRender.transform.rotation = this.transform.rotation;
      }
      if ((bool) (UnityEngine.Object) this.m_AlphaObjectToRender)
      {
        this.m_AlphaObjectToRender.transform.position = this.m_OffscreenPos - this.ALPHA_OBJECT_OFFSET;
        this.m_AlphaObjectToRender.transform.rotation = this.transform.rotation;
      }
      if ((UnityEngine.Object) this.m_CameraGO == (UnityEngine.Object) null)
        return;
      this.m_CameraGO.transform.rotation = Quaternion.identity;
      this.m_CameraGO.transform.position = !(bool) (UnityEngine.Object) this.m_ObjectToRender ? this.m_OffscreenPos + this.m_PositionOffset + this.m_CameraOffset : this.m_ObjectToRender.transform.position + this.m_CameraOffset;
      this.m_CameraGO.transform.rotation = this.m_ObjectToRender.transform.rotation;
      this.m_CameraGO.transform.Rotate(90f, 0.0f, 0.0f);
    }
  }

  private void RestoreAfterRender()
  {
    if (this.m_HideRenderObject)
      return;
    if ((bool) (UnityEngine.Object) this.m_ObjectToRender)
    {
      if ((UnityEngine.Object) this.m_ObjectToRenderOrgParent != (UnityEngine.Object) null)
        this.m_ObjectToRender.transform.parent = this.m_ObjectToRenderOrgParent;
      this.m_ObjectToRender.transform.localPosition = this.m_ObjectToRenderOrgPosition;
    }
    else
      this.transform.localPosition = this.m_ObjectToRenderOrgPosition;
  }

  private void CreateTexture()
  {
    if ((UnityEngine.Object) this.m_RenderTexture != (UnityEngine.Object) null)
      return;
    Vector2 vector2 = this.CalcTextureSize();
    if (this.m_graphicsManager != null)
    {
      if (this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.Low)
        vector2 *= 0.75f;
      else if (this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.Medium)
        vector2 *= 1f;
      else if (this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.High)
        vector2 *= 1.5f;
    }
    this.m_RenderTexture = RenderTextureTracker.Get().CreateNewTexture((int) vector2.x, (int) vector2.y, RenderTextureTracker.TEXTURE_DEPTH, this.m_RenderTextureFormat);
    this.m_RenderTexture.Create();
  }

  private void ReleaseTexture()
  {
    if ((UnityEngine.Object) RenderTexture.active == (UnityEngine.Object) this.m_RenderTexture)
      RenderTexture.active = (RenderTexture) null;
    if ((UnityEngine.Object) RenderTexture.active == (UnityEngine.Object) this.m_BloomRenderTexture)
      RenderTexture.active = (RenderTexture) null;
    if ((UnityEngine.Object) this.m_RenderTexture != (UnityEngine.Object) null)
    {
      RenderTextureTracker.Get().DestroyRenderTexture(this.m_RenderTexture);
      this.m_RenderTexture = (RenderTexture) null;
    }
    if ((UnityEngine.Object) this.m_BloomRenderTexture != (UnityEngine.Object) null)
    {
      RenderTextureTracker.Get().DestroyRenderTexture(this.m_BloomRenderTexture);
      this.m_BloomRenderTexture = (RenderTexture) null;
    }
    if ((UnityEngine.Object) this.m_BloomRenderBuffer1 != (UnityEngine.Object) null)
    {
      RenderTextureTracker.Get().DestroyRenderTexture(this.m_BloomRenderBuffer1);
      this.m_BloomRenderBuffer1 = (RenderTexture) null;
    }
    if (!((UnityEngine.Object) this.m_BloomRenderBuffer2 != (UnityEngine.Object) null))
      return;
    RenderTextureTracker.Get().DestroyRenderTexture(this.m_BloomRenderBuffer2);
    this.m_BloomRenderBuffer2 = (RenderTexture) null;
  }

  private void CreateCamera()
  {
    if ((UnityEngine.Object) this.m_CameraGO != (UnityEngine.Object) null)
      return;
    this.m_CameraGO = new GameObject(this.name + "_R2TRenderCamera", new System.Type[1]
    {
      typeof (Camera)
    });
    this.m_CameraGO.TryGetComponent<Camera>(out this.m_Camera);
    if (this.m_HideRenderObject)
    {
      if (this.m_RealtimeTranslation)
      {
        this.m_OffscreenGameObject.transform.position = this.transform.position;
        this.m_CameraGO.transform.parent = this.m_OffscreenGameObject.transform;
        this.m_CameraGO.transform.localPosition = Vector3.zero + this.m_PositionOffset + this.m_CameraOffset;
        this.m_CameraGO.transform.rotation = this.transform.rotation;
        this.m_OffscreenGameObject.transform.position = this.m_OffscreenPos;
      }
      else
      {
        this.m_CameraGO.transform.parent = (Transform) null;
        this.m_CameraGO.transform.position = this.m_OffscreenPos + this.m_PositionOffset + this.m_CameraOffset;
        this.m_CameraGO.transform.rotation = this.transform.rotation;
      }
    }
    else
    {
      this.m_CameraGO.transform.parent = this.transform;
      this.m_CameraGO.transform.position = this.transform.position + this.m_PositionOffset + this.m_CameraOffset;
      this.m_CameraGO.transform.rotation = this.transform.rotation;
    }
    this.m_CameraGO.transform.Rotate(90f, 0.0f, 0.0f);
    if ((double) this.m_FarClip < 0.0)
      this.m_FarClip = 0.0f;
    if ((double) this.m_NearClip > 0.0)
      this.m_NearClip = 0.0f;
    this.m_Camera.orthographic = true;
    this.m_Camera.nearClipPlane = this.m_NearClip * this.m_WorldScale.y;
    this.m_Camera.farClipPlane = this.m_FarClip * this.m_WorldScale.y;
    this.m_Camera.clearFlags = CameraClearFlags.Color;
    this.m_Camera.backgroundColor = this.m_ClearColor;
    this.m_Camera.depthTextureMode = DepthTextureMode.None;
    this.m_Camera.renderingPath = RenderingPath.Forward;
    this.m_Camera.cullingMask = (int) this.m_LayerMask;
    this.m_Camera.allowHDR = false;
    this.m_Camera.enabled = false;
    this.m_Camera.targetTexture = this.m_RenderTexture;
    this.m_CameraData = new RenderToTextureUtils.LightWeightCamera();
    this.m_CameraData.cullingMask = this.m_LayerMask;
    this.m_CameraData.backgroundColor = this.m_ClearColor;
    this.m_CameraData.aspectRatio = (float) this.m_RenderTexture.width / (float) this.m_RenderTexture.height;
  }

  private float OrthoSize()
  {
    if ((double) this.m_OverrideCameraSize > 0.0)
      return this.m_OverrideCameraSize;
    return (double) this.m_WorldWidth <= (double) this.m_WorldHeight ? this.m_WorldHeight * 0.5f : Mathf.Min(this.m_WorldWidth * 0.5f, this.m_WorldHeight * 0.5f);
  }

  private void CreateAlphaCamera()
  {
    if ((UnityEngine.Object) this.m_AlphaCameraGO != (UnityEngine.Object) null)
      return;
    this.m_AlphaCameraGO = new GameObject(this.name + "_R2TAlphaRenderCamera");
    this.m_AlphaCameraGO.transform.parent = this.m_CameraGO.transform;
    this.m_AlphaCameraGO.transform.position = !(bool) (UnityEngine.Object) this.m_AlphaObjectToRender ? this.m_CameraGO.transform.position : this.m_CameraGO.transform.position - this.ALPHA_OBJECT_OFFSET;
    this.m_AlphaCameraGO.transform.localRotation = Quaternion.identity;
    this.m_AlphaCameraData = new RenderToTextureUtils.LightWeightCamera(this.m_CameraData);
    this.m_AlphaCameraData.backgroundColor = Color.clear;
  }

  private void AlphaCameraRender(
    CommandBuffer cmd,
    RenderTexture targetTexture,
    RenderToTextureUtils.LightWeightCamera alphaCamera,
    RenderCommandLists objectToRender,
    RenderCommandLists alphaObjectToRender)
  {
    if (this.m_OpaqueObjectAlphaFill)
      RenderToTextureUtils.RenderCamera(cmd, targetTexture, alphaCamera, objectToRender, this.m_AlphaFillShader, "RenderType");
    else if ((UnityEngine.Object) this.m_AlphaObjectToRender == (UnityEngine.Object) null)
    {
      string replacementTag = this.m_AlphaReplacementTag;
      if (replacementTag == string.Empty)
        replacementTag = this.m_ReplacmentTag;
      RenderToTextureUtils.RenderCamera(cmd, targetTexture, alphaCamera, objectToRender, this.m_AlphaFillShader, replacementTag);
    }
    else
      RenderToTextureUtils.RenderCamera(cmd, targetTexture, alphaCamera, alphaObjectToRender);
  }

  private void CreateBloomCaptureCamera()
  {
    if ((UnityEngine.Object) this.m_BloomCaptureCameraGO != (UnityEngine.Object) null)
      return;
    this.m_BloomCaptureCameraGO = new GameObject(this.name + "_R2TBloomRenderCamera");
    this.m_BloomCaptureCameraGO.transform.parent = this.m_CameraGO.transform;
    this.m_BloomCaptureCameraGO.transform.localPosition = Vector3.zero;
    this.m_BloomCaptureCameraGO.transform.localRotation = Quaternion.identity;
    this.m_BloomCameraData = new RenderToTextureUtils.LightWeightCamera(this.m_CameraData);
  }

  private Vector2 CalcTextureSize()
  {
    Vector2 vector2 = new Vector2((float) this.m_Resolution, (float) this.m_Resolution);
    if ((double) this.m_WorldWidth > (double) this.m_WorldHeight)
    {
      vector2.x = (float) this.m_Resolution;
      vector2.y = (float) this.m_Resolution * (this.m_WorldHeight / this.m_WorldWidth);
    }
    else
    {
      vector2.x = (float) this.m_Resolution * (this.m_WorldWidth / this.m_WorldHeight);
      vector2.y = (float) this.m_Resolution;
    }
    return vector2;
  }

  private void CreateRenderPlane()
  {
    if ((UnityEngine.Object) this.m_PlaneGameObject != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_PlaneGameObject);
    this.m_PlaneGameObject = this.CreateMeshPlane(string.Format("{0}_RenderPlane", (object) this.name), this.m_Material);
    GameObjectUtils.SetHideFlags((UnityEngine.Object) this.m_PlaneGameObject, HideFlags.DontSave);
  }

  private void CreateBloomPlane()
  {
    if ((UnityEngine.Object) this.m_BloomPlaneGameObject != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_BloomPlaneGameObject);
    Material material = this.AdditiveMaterial;
    if (this.m_BloomBlend == RenderToTexture.BloomBlendType.Transparent)
      material = this.TransparentMaterial;
    this.m_BloomPlaneGameObject = this.CreateMeshPlane(string.Format("{0}_BloomRenderPlane", (object) this.name), material);
    this.m_BloomPlaneGameObject.transform.parent = this.m_PlaneGameObject.transform;
    this.m_BloomPlaneGameObject.transform.localPosition = new Vector3(0.0f, 0.15f, 0.0f);
    this.m_BloomPlaneGameObject.transform.localRotation = Quaternion.identity;
    this.m_BloomPlaneGameObject.transform.localScale = Vector3.one;
    this.m_BloomPlaneGameObject.GetComponent<Renderer>().GetMaterial().color = this.m_BloomColor;
  }

  private void CreateBloomCapturePlane()
  {
    if ((UnityEngine.Object) this.m_BloomCapturePlaneGameObject != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_BloomCapturePlaneGameObject);
    Material material = this.AdditiveMaterial;
    if (this.m_BloomBlend == RenderToTexture.BloomBlendType.Transparent)
      material = this.TransparentMaterial;
    this.m_BloomCapturePlaneGameObject = this.CreateMeshPlane(string.Format("{0}_BloomCaptureRenderPlane", (object) this.name), material);
    this.m_BloomCapturePlaneGameObject.transform.parent = this.m_BloomCaptureCameraGO.transform;
    this.m_BloomCapturePlaneGameObject.transform.localPosition = Vector3.zero;
    this.m_BloomCapturePlaneGameObject.transform.localRotation = Quaternion.identity;
    this.m_BloomCapturePlaneGameObject.transform.Rotate(-90f, 0.0f, 0.0f);
    this.m_BloomCapturePlaneGameObject.transform.localScale = this.m_WorldScale;
    if ((bool) (UnityEngine.Object) this.m_Material)
      this.m_BloomCapturePlaneGameObject.GetComponent<Renderer>().SetMaterial(this.m_PlaneGameObject.GetComponent<Renderer>().GetMaterial());
    if (!(bool) (UnityEngine.Object) this.m_RenderTexture)
      return;
    this.m_BloomCapturePlaneGameObject.GetComponent<Renderer>().GetMaterial().mainTexture = (Texture) this.m_RenderTexture;
  }

  private GameObject CreateMeshPlane(string name, Material material)
  {
    GameObject meshPlane = new GameObject(name, new System.Type[2]
    {
      typeof (MeshFilter),
      typeof (MeshRenderer)
    });
    meshPlane.transform.parent = this.transform;
    meshPlane.transform.localPosition = this.m_PositionOffset;
    meshPlane.transform.localRotation = Quaternion.identity;
    meshPlane.transform.localScale = Vector3.one;
    Mesh mesh = new Mesh();
    float x = this.m_Width * 0.5f;
    float z = this.m_Height * 0.5f;
    mesh.vertices = new Vector3[4]
    {
      new Vector3(-x, 0.0f, -z),
      new Vector3(x, 0.0f, -z),
      new Vector3(-x, 0.0f, z),
      new Vector3(x, 0.0f, z)
    };
    mesh.uv = this.PLANE_UVS;
    mesh.normals = this.PLANE_NORMALS;
    mesh.triangles = this.PLANE_TRIANGLES;
    (meshPlane.GetComponent<MeshFilter>().mesh = mesh).RecalculateBounds();
    Renderer component = meshPlane.GetComponent<Renderer>();
    if ((bool) (UnityEngine.Object) material)
      component.SetMaterial(material);
    component.sortingOrder = this.m_RenderQueue;
    component.GetMaterial().renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue;
    this.m_previousRenderQueue = this.m_RenderQueue;
    return meshPlane;
  }

  public void EnablePopupRendering(IPopupRoot popupRoot) => this.m_popupRoot = popupRoot;

  public void DisablePopupRendering()
  {
    if (this.m_popupRoot != null)
      this.m_popupRoot.CleanupPopupRendering(this.m_popupRenderers);
    this.m_popupRoot = (IPopupRoot) null;
  }

  public bool HandlesChildPropagation() => false;

  private void CalcWorldWidthHeightScale()
  {
    Quaternion rotation = this.transform.rotation;
    Vector3 localScale = this.transform.localScale;
    Transform parent = this.transform.parent;
    this.transform.rotation = Quaternion.identity;
    bool flag = false;
    if ((double) this.transform.lossyScale.magnitude == 0.0)
    {
      this.transform.parent = (Transform) null;
      this.transform.localScale = Vector3.one;
      flag = true;
    }
    if (this.m_UniformWorldScale)
    {
      float num = Mathf.Max(this.transform.lossyScale.x, this.transform.lossyScale.y, this.transform.lossyScale.z);
      this.m_WorldScale = new Vector3(num, num, num);
    }
    else
      this.m_WorldScale = this.transform.lossyScale;
    this.m_WorldWidth = this.m_Width * this.m_WorldScale.x;
    this.m_WorldHeight = this.m_Height * this.m_WorldScale.y;
    if (flag)
    {
      this.transform.parent = parent;
      this.transform.localScale = localScale;
    }
    this.transform.rotation = rotation;
    if ((double) this.m_WorldWidth != 0.0 && (double) this.m_WorldHeight != 0.0)
      return;
    Debug.LogError((object) string.Format(" \"{0}\": RenderToTexture has a world scale of zero. \nm_WorldWidth: {1},   m_WorldHeight: {2}", (object) this.m_WorldWidth, (object) this.m_WorldHeight));
  }

  private void CleanUp()
  {
    this.ReleaseTexture();
    if (this.m_hasMaterialInstance)
    {
      if (!RenderToTexture.GetMaterialService().CanIgnoreMaterial(this.m_Material))
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_Material);
      this.m_hasMaterialInstance = false;
    }
    if ((bool) (UnityEngine.Object) this.m_CameraGO)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_CameraGO);
    if ((bool) (UnityEngine.Object) this.m_AlphaCameraGO)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_AlphaCameraGO);
    if ((bool) (UnityEngine.Object) this.m_PlaneGameObject)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_PlaneGameObject);
    if ((bool) (UnityEngine.Object) this.m_BloomPlaneGameObject)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_BloomPlaneGameObject);
    if ((bool) (UnityEngine.Object) this.m_BloomCaptureCameraGO)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_BloomCaptureCameraGO);
    if ((bool) (UnityEngine.Object) this.m_BloomCapturePlaneGameObject)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_BloomCapturePlaneGameObject);
    if ((bool) (UnityEngine.Object) this.m_OffscreenGameObject)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_OffscreenGameObject);
    if ((UnityEngine.Object) this.m_ObjectToRender != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) this.m_ObjectToRenderOrgParent != (UnityEngine.Object) null)
        this.m_ObjectToRender.transform.parent = this.m_ObjectToRenderOrgParent;
      this.m_ObjectToRender.transform.localPosition = this.m_ObjectToRenderOrgPosition;
    }
    this.m_init = false;
    this.m_isDirty = true;
  }

  private static IMaterialService GetMaterialService()
  {
    if (RenderToTexture.s_materialService == null)
      RenderToTexture.s_materialService = ServiceManager.Get<IMaterialService>();
    return RenderToTexture.s_materialService;
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

  public enum AlphaClipShader
  {
    Standard,
    ColorGradient,
  }

  public enum BloomRenderType
  {
    Color,
    Alpha,
  }

  public enum BloomBlendType
  {
    Additive,
    Transparent,
  }
}
