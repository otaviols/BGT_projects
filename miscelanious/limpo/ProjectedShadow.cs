using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class ProjectedShadow : MonoBehaviour
{
  private const int RENDER_SIZE = 64;
  private const string CONTACT_SHADER_NAME = "Custom/ContactShadow";
  private const string UNLIT_WHITE_SHADER_NAME = "Custom/Unlit/Color/White";
  private const string UNLIT_DARKGREY_SHADER_NAME = "Custom/Unlit/Color/DarkGrey";
  private const string MULTISAMPLE_SHADER_NAME = "Custom/Selection/HighlightMultiSample";
  private const float NEARCLIP_PLANE = 0.0f;
  private const float SHADOW_OFFSET_SCALE = 0.3f;
  private const float RENDERMASK_OFFSET = 0.11f;
  private const float RENDERMASK_BLUR = 0.6f;
  private const float RENDERMASK_BLUR2 = 0.8f;
  private const float CONTACT_SHADOW_SCALE = 0.98f;
  private const float CONTACT_SHADOW_FADE_IN_HEIGHT = 0.08f;
  private const float CONTACT_SHADOW_INTENSITY = 3.5f;
  private static CommandBuffer s_commandBuffer;
  private static RenderTexture s_tempBuffer;
  private static readonly int BLUR_OFFSETS_ID = Shader.PropertyToID("_BlurOffsets");
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
  public float m_ShadowProjectorSize = 1.5f;
  public bool m_ShadowEnabled;
  public bool m_AutoBoardHeightDisable;
  public float m_AutoDisableHeight;
  public float m_ProjectionFarClip = 10f;
  public Vector3 m_ProjectionOffset;
  public bool m_ContactShadow;
  public Vector3 m_ContactOffset = Vector3.zero;
  public bool m_isDirtyContactShadow = true;
  public bool m_enabledAlongsideRealtimeShadows;
  private static float s_offset = -12000f;
  private static Color s_ShadowColor = new Color(0.098f, 0.098f, 0.235f, 0.45f);
  private GameObject m_RootObject;
  private GameObject m_ProjectorGameObject;
  private Transform m_ProjectorTransform;
  private DecalProjector m_Projector;
  private RenderTexture m_ShadowTexture;
  private RenderTexture m_ContactShadowTexture;
  private float m_AdjustedShadowProjectorSize = 1.5f;
  private float m_BoardHeight = 0.2f;
  private bool m_HasBoardHeight;
  private Mesh m_PlaneMesh;
  private GameObject m_PlaneGameObject;
  private CancellationTokenSource m_LoadTokenSource;
  private IGraphicsManager m_graphicsManager;
  private List<MeshRenderer> m_objectsToRender;
  private bool m_projectorShadowDelay;
  private List<Material> m_materialsTempList = new List<Material>();
  private Shader m_UnlitWhiteShader;
  private Shader m_UnlitDarkGreyShader;
  private Material m_ShadowMaterial;
  private Material m_WhiteMaterial;
  private Material m_UnlitDarkGreyMaterial;
  private Shader m_ContactShadowShader;
  private Material m_ContactShadowMaterial;
  private Shader m_MultiSampleShader;
  private Material m_MultiSampleMaterial;

  private Material WhiteMaterial => this.m_WhiteMaterial ?? (this.m_WhiteMaterial = new Material(this.m_UnlitWhiteShader));

  private Material UnlitDarkGreyMaterial => this.m_UnlitDarkGreyMaterial ?? (this.m_UnlitDarkGreyMaterial = new Material(this.m_UnlitDarkGreyShader));

  protected Material ContactShadowMaterial
  {
    get
    {
      if ((Object) this.m_ContactShadowMaterial == (Object) null)
      {
        this.m_ContactShadowMaterial = new Material(this.m_ContactShadowShader);
        this.m_ContactShadowMaterial.SetFloat("_Intensity", 3.5f);
        this.m_ContactShadowMaterial.SetColor("_Color", ProjectedShadow.s_ShadowColor);
        GameObjectUtils.SetHideFlags((Object) this.m_ContactShadowMaterial, HideFlags.DontSave);
      }
      return this.m_ContactShadowMaterial;
    }
  }

  protected Material MultiSampleMaterial
  {
    get
    {
      if ((Object) this.m_MultiSampleMaterial == (Object) null)
      {
        this.m_MultiSampleMaterial = new Material(this.m_MultiSampleShader);
        GameObjectUtils.SetHideFlags((Object) this.m_MultiSampleMaterial, HideFlags.DontSave);
      }
      return this.m_MultiSampleMaterial;
    }
  }

  private void Awake()
  {
    if (ProjectedShadow.s_commandBuffer == null)
      ProjectedShadow.s_commandBuffer = CommandBufferPool.Get("Render Shadow");
    if ((Object) ProjectedShadow.s_tempBuffer == (Object) null || !ProjectedShadow.s_tempBuffer.IsCreated())
      ProjectedShadow.s_tempBuffer = RenderTextureTracker.Get().CreateNewTexture(64, 64, RenderTextureTracker.TEXTURE_DEPTH, RenderTextureFormat.R8);
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.m_objectsToRender = new List<MeshRenderer>();
  }

  protected void Start()
  {
    if (this.m_graphicsManager != null && this.m_graphicsManager.RealtimeShadows && !this.m_enabledAlongsideRealtimeShadows)
      this.enabled = false;
    if ((Object) this.m_ContactShadowShader == (Object) null)
      this.m_ContactShadowShader = ShaderUtils.FindShader("Custom/ContactShadow");
    if (!(bool) (Object) this.m_ContactShadowShader)
    {
      Debug.LogError((object) "Failed to load Projected Shadow Shader: Custom/ContactShadow");
      this.enabled = false;
    }
    if ((Object) this.m_MultiSampleShader == (Object) null)
      this.m_MultiSampleShader = ShaderUtils.FindShader("Custom/Selection/HighlightMultiSample");
    if (!(bool) (Object) this.m_MultiSampleShader)
    {
      Debug.LogError((object) "Failed to load Projected Shadow Shader: Custom/Selection/HighlightMultiSample");
      this.enabled = false;
    }
    this.m_UnlitWhiteShader = ShaderUtils.FindShader("Custom/Unlit/Color/White");
    if (!(bool) (Object) this.m_UnlitWhiteShader)
      Debug.LogError((object) "Failed to load Projected Shadow Shader: Custom/Unlit/Color/White");
    this.m_UnlitDarkGreyShader = ShaderUtils.FindShader("Custom/Unlit/Color/DarkGrey");
    if (!(bool) (Object) this.m_UnlitDarkGreyShader)
      Debug.LogError((object) "Failed to load Projected Shadow Shader: Custom/Unlit/Color/DarkGrey");
    if ((Object) Board.Get() != (Object) null)
    {
      if (this.m_LoadTokenSource == null)
        this.m_LoadTokenSource = new CancellationTokenSource();
      this.AssignBoardHeight_WaitForBoardStandardGameLoaded(this.m_LoadTokenSource.Token).Forget();
    }
    Actor component = this.GetComponent<Actor>();
    if ((Object) component != (Object) null)
    {
      this.m_RootObject = component.GetRootObject();
    }
    else
    {
      GameObject childBySubstring = GameObjectUtils.FindChildBySubstring(this.gameObject, "RootObject");
      if ((Object) childBySubstring != (Object) null)
        this.m_RootObject = childBySubstring;
      else
        this.m_RootObject = this.gameObject;
    }
  }

  private async UniTaskVoid AssignBoardHeight_WaitForBoardStandardGameLoaded(
    CancellationToken token = default (CancellationToken))
  {
    SceneMgr sceneMgr;
    if (!ServiceManager.TryGet<SceneMgr>(out sceneMgr))
      return;
    while (sceneMgr.GetMode() == SceneMgr.Mode.GAMEPLAY && (Object) Gameplay.Get().GetBoardLayout() == (Object) null)
      await UniTask.Yield(PlayerLoopTiming.Initialization, token);
    if (sceneMgr.GetMode() != SceneMgr.Mode.GAMEPLAY)
      return;
    Transform bone = Board.Get().FindBone("CenterPointBone");
    if (!((Object) bone != (Object) null))
      return;
    this.m_BoardHeight = bone.position.y;
    this.m_HasBoardHeight = true;
  }

  protected void LateUpdate()
  {
    if (this.m_graphicsManager != null && this.m_graphicsManager.RealtimeShadows && !this.m_enabledAlongsideRealtimeShadows)
    {
      this.enabled = false;
    }
    else
    {
      this.Render();
      if (!this.m_ContactShadow)
        return;
      this.RenderContactShadow();
    }
  }

  private void OnDisable()
  {
    if ((bool) (Object) this.m_PlaneGameObject)
      this.m_PlaneGameObject.SetActive(false);
    if (!((Object) this.m_Projector != (Object) null))
      return;
    this.m_Projector.enabled = false;
  }

  protected void OnDestroy()
  {
    if ((bool) (Object) this.m_ContactShadowMaterial)
      Object.Destroy((Object) this.m_ContactShadowMaterial);
    if ((bool) (Object) this.m_ShadowMaterial)
      Object.Destroy((Object) this.m_ShadowMaterial);
    if ((bool) (Object) this.m_MultiSampleMaterial)
      Object.Destroy((Object) this.m_MultiSampleMaterial);
    if ((bool) (Object) this.m_ProjectorGameObject)
      Object.Destroy((Object) this.m_ProjectorGameObject);
    if ((bool) (Object) this.m_ShadowTexture)
    {
      RenderTextureTracker.Get().DestroyRenderTexture(this.m_ShadowTexture);
      this.m_ShadowTexture = (RenderTexture) null;
    }
    if ((bool) (Object) this.m_ContactShadowTexture)
    {
      RenderTextureTracker.Get().DestroyRenderTexture(this.m_ContactShadowTexture);
      this.m_ContactShadowTexture = (RenderTexture) null;
    }
    if ((bool) (Object) this.m_PlaneMesh)
    {
      Object.DestroyImmediate((Object) this.m_PlaneMesh);
      MeshFilter component = this.m_PlaneGameObject.GetComponent<MeshFilter>();
      Object.DestroyImmediate((Object) component.mesh);
      component.mesh = (Mesh) null;
      this.m_PlaneMesh = (Mesh) null;
    }
    if ((bool) (Object) this.m_PlaneGameObject)
    {
      Object.DestroyImmediate((Object) this.m_PlaneGameObject);
      this.m_PlaneGameObject = (GameObject) null;
    }
    this.m_LoadTokenSource?.Cancel();
    this.m_LoadTokenSource?.Dispose();
  }

  private void OnDrawGizmos()
  {
    float num = (float) ((double) this.m_ShadowProjectorSize * (double) TransformUtil.ComputeWorldScale((Component) this.transform).x * 2.0);
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.color = new Color(0.6f, 0.15f, 0.6f);
    if (this.m_ContactShadow)
      Gizmos.DrawWireCube(this.m_ContactOffset, new Vector3(num, 0.0f, num));
    else
      Gizmos.DrawWireCube(Vector3.zero, new Vector3(num, 0.0f, num));
    Gizmos.matrix = Matrix4x4.identity;
  }

  public void Render()
  {
    if (!this.m_ShadowEnabled || (bool) (Object) this.m_RootObject && !this.m_RootObject.activeSelf)
    {
      if ((bool) (Object) this.m_Projector && this.m_Projector.enabled)
        this.m_Projector.enabled = false;
      if (!(bool) (Object) this.m_PlaneGameObject)
        return;
      this.m_PlaneGameObject.SetActive(false);
    }
    else
    {
      this.m_AdjustedShadowProjectorSize = this.m_ShadowProjectorSize * TransformUtil.ComputeWorldScale((Component) this.transform).x;
      if ((double) this.m_AdjustedShadowProjectorSize == 0.0)
        return;
      if ((Object) this.m_Projector == (Object) null)
        this.CreateProjector();
      float y = this.transform.position.y;
      float num1 = this.m_HasBoardHeight ? this.m_BoardHeight : (float) ((double) y - (double) Mathf.Max(0.0f, this.m_AutoDisableHeight) - 1.40129846432482E-45);
      float num2 = (float) (((double) y - (double) num1) * 0.300000011920929);
      this.m_AdjustedShadowProjectorSize += Mathf.Lerp(0.0f, 0.5f, num2 * 0.5f);
      if (this.m_ContactShadow)
      {
        float num3 = num1 + 0.08f;
        if ((double) num2 < (double) num3)
        {
          if ((Object) this.m_PlaneGameObject == (Object) null)
            this.m_isDirtyContactShadow = true;
          else if (!this.m_PlaneGameObject.activeSelf)
            this.m_isDirtyContactShadow = true;
          float num4 = Mathf.Clamp((float) (((double) num3 - (double) num2) / 0.0799999982118607), 0.0f, 1f);
          if ((bool) (Object) this.m_ContactShadowTexture && (bool) (Object) this.m_PlaneGameObject)
          {
            Renderer component = this.m_PlaneGameObject.GetComponent<Renderer>();
            Material material = (bool) (Object) component ? component.GetSharedMaterial() : (Material) null;
            if ((bool) (Object) material)
            {
              material.mainTexture = (Texture) this.m_ContactShadowTexture;
              material.color = ProjectedShadow.s_ShadowColor;
              material.SetFloat("_Alpha", num4);
            }
          }
        }
        else if ((Object) this.m_PlaneGameObject != (Object) null)
          this.m_PlaneGameObject.SetActive(false);
      }
      if ((double) num2 < (double) this.m_AutoDisableHeight && this.m_AutoBoardHeightDisable)
      {
        this.m_Projector.enabled = false;
        Object.DestroyImmediate((Object) this.m_ShadowTexture);
        this.m_ShadowTexture = (RenderTexture) null;
      }
      else
      {
        this.m_Projector.enabled = true;
        float num5 = 0.0f;
        if (this.m_projectorShadowDelay)
        {
          this.m_projectorShadowDelay = false;
          num5 = 1000f;
        }
        else if ((Object) this.transform.parent != (Object) null)
          num5 = Mathf.Lerp(-0.7f, 1.8f, (float) ((double) this.transform.parent.position.x / 17.0 * -1.0)) * num2;
        this.m_ProjectorTransform.position = new Vector3((float) ((double) this.transform.position.x - (double) num5 - (double) num2 * 0.25), this.transform.position.y, this.transform.position.z - num2 * 0.8f);
        this.m_ProjectorTransform.Translate(this.m_ProjectionOffset);
        Quaternion rotation = this.transform.rotation;
        float num6 = (float) ((1.0 - (double) rotation.z) * 0.5 + 0.5);
        float num7 = rotation.x * 0.5f;
        this.m_Projector.AspectRatio = num6 - num7;
        this.m_Projector.OrthographicSize = this.m_AdjustedShadowProjectorSize + num7;
        this.m_ProjectorTransform.rotation = Quaternion.identity;
        this.m_ProjectorTransform.Rotate(90f, rotation.eulerAngles.y, 0.0f);
        if (!((Object) this.m_ShadowTexture == (Object) null) && this.m_ShadowTexture.IsCreated())
          return;
        this.m_ShadowTexture = RenderTextureTracker.Get().CreateNewTexture(64, 64, RenderTextureTracker.TEXTURE_DEPTH, RenderTextureFormat.R8);
        this.RenderShadowMask();
      }
    }
  }

  public static void SetShadowColor(Color color) => ProjectedShadow.s_ShadowColor = color;

  public void EnableShadow() => this.m_ShadowEnabled = true;

  public void EnableShadow(float FadeInTime)
  {
    this.m_ShadowEnabled = true;
    Hashtable args = iTween.Hash((object) "from", (object) 0, (object) "to", (object) 1, (object) "time", (object) FadeInTime, (object) "easetype", (object) iTween.EaseType.easeInCubic, (object) "onupdate", (object) "UpdateShadowColor", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "ProjectedShadowFade");
    iTween.StopByName(this.gameObject, "ProjectedShadowFade");
    iTween.ValueTo(this.gameObject, args);
  }

  public void DisableShadow() => this.DisableShadowProjector();

  public void DisableShadow(float FadeOutTime)
  {
    if ((Object) this.m_Projector == (Object) null || !this.m_ShadowEnabled)
      return;
    Hashtable args = iTween.Hash((object) "from", (object) 1, (object) "to", (object) 0, (object) "time", (object) FadeOutTime, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "onupdate", (object) "UpdateShadowColor", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "ProjectedShadowFade", (object) "oncomplete", (object) "DisableShadowProjector");
    iTween.StopByName(this.gameObject, "ProjectedShadowFade");
    iTween.ValueTo(this.gameObject, args);
  }

  public void UpdateContactShadow(Spell spell, SpellStateType prevStateType, object userData) => this.UpdateContactShadow();

  public void UpdateContactShadow(Spell spell, object userData) => this.UpdateContactShadow();

  public void UpdateContactShadow(Spell spell) => this.UpdateContactShadow();

  public void UpdateContactShadow()
  {
    if (!this.m_ContactShadow)
      return;
    this.m_isDirtyContactShadow = true;
  }

  private void DisableShadowProjector()
  {
    if ((Object) this.m_Projector != (Object) null)
      this.m_Projector.enabled = false;
    this.m_ShadowEnabled = false;
  }

  private void UpdateShadowColor(float val)
  {
    if ((Object) this.m_Projector == (Object) null || (Object) this.m_Projector.Material == (Object) null)
      return;
    this.m_Projector.Material.SetColor("_Color", Color.Lerp(new Color(0.5f, 0.5f, 0.5f, 0.5f), ProjectedShadow.s_ShadowColor, val));
  }

  private void RenderShadowMask()
  {
    Vector3 position = this.transform.position;
    ProjectedShadow.s_offset -= 10f;
    if ((double) ProjectedShadow.s_offset < -19000.0)
      ProjectedShadow.s_offset = -12000f;
    Vector3 renderPosition = Vector3.left * ProjectedShadow.s_offset;
    this.transform.position = renderPosition;
    float halfSize = (float) ((double) this.m_ShadowProjectorSize * (double) TransformUtil.ComputeWorldScale((Component) this.transform).x - 0.109999999403954 - 0.0500000007450581);
    this.RenderToShadowTexture(this.m_ShadowTexture, renderPosition, halfSize, false);
    this.m_ShadowMaterial.SetTexture("_MainTex", (Texture) this.m_ShadowTexture);
    this.m_ShadowMaterial.SetColor("_Color", ProjectedShadow.s_ShadowColor);
    this.transform.position = position;
  }

  private async UniTaskVoid DelayRenderContactShadow()
  {
    await UniTask.NextFrame();
    this.m_isDirtyContactShadow = true;
  }

  private void RenderContactShadow()
  {
    if (this.m_graphicsManager != null && this.m_graphicsManager.RealtimeShadows && !this.m_enabledAlongsideRealtimeShadows)
      this.enabled = false;
    if ((Object) this.m_ContactShadowTexture != (Object) null && !this.m_isDirtyContactShadow && this.m_ContactShadowTexture.IsCreated())
      return;
    if ((Object) this.m_PlaneGameObject == (Object) null)
      this.CreateRenderPlane();
    this.m_PlaneGameObject.SetActive(true);
    if ((Object) this.m_ContactShadowTexture == (Object) null)
      this.m_ContactShadowTexture = RenderTextureTracker.Get().CreateNewTexture(64, 64, RenderTextureTracker.TEXTURE_DEPTH, RenderTextureFormat.R8);
    Quaternion localRotation = this.transform.localRotation;
    Vector3 localPosition = this.transform.localPosition;
    Vector3 localScale = this.transform.localScale;
    ProjectedShadow.s_offset -= 10f;
    if ((double) ProjectedShadow.s_offset < -19000.0)
      ProjectedShadow.s_offset = -12000f;
    Vector3 renderPosition = Vector3.left * ProjectedShadow.s_offset;
    this.transform.position = renderPosition;
    this.transform.rotation = Quaternion.identity;
    this.SetWorldScale(this.transform, Vector3.one);
    float halfSize = (float) ((double) this.m_ShadowProjectorSize - 0.109999999403954 - 0.150000005960464);
    this.RenderToShadowTexture(this.m_ContactShadowTexture, renderPosition, halfSize, true);
    this.transform.localRotation = localRotation;
    this.transform.localPosition = localPosition;
    this.transform.localScale = localScale;
    this.m_PlaneGameObject.GetComponent<Renderer>().GetSharedMaterial().mainTexture = (Texture) this.m_ContactShadowTexture;
    this.m_isDirtyContactShadow = false;
  }

  private void RenderToShadowTexture(
    RenderTexture destTexture,
    Vector3 renderPosition,
    float halfSize,
    bool isContactShadow)
  {
    Matrix4x4 inverse = Matrix4x4.TRS(renderPosition, Quaternion.Euler(90f, 0.0f, 0.0f), new Vector3(1f, 1f, -1f)).inverse;
    Matrix4x4 proj = Matrix4x4.Ortho(-halfSize, halfSize, -halfSize, halfSize, -3f, 3f);
    ProjectedShadow.s_commandBuffer.SetRenderTarget((RenderTargetIdentifier) (Texture) destTexture);
    ProjectedShadow.s_commandBuffer.ClearRenderTarget(true, true, Color.clear);
    ProjectedShadow.s_commandBuffer.SetViewProjectionMatrices(inverse, proj);
    this.GetComponentsInChildren<MeshRenderer>(false, this.m_objectsToRender);
    foreach (MeshRenderer meshRenderer in this.m_objectsToRender)
    {
      if (meshRenderer.enabled)
      {
        meshRenderer.GetSharedMaterials(this.m_materialsTempList);
        int count = this.m_materialsTempList.Count;
        for (int index = 0; index < count; ++index)
        {
          Material materialsTemp = this.m_materialsTempList[index];
          if ((Object) materialsTemp != (Object) null && materialsTemp.GetTag("Highlight", false) != "")
            ProjectedShadow.s_commandBuffer.DrawRenderer((Renderer) meshRenderer, isContactShadow ? this.UnlitDarkGreyMaterial : this.WhiteMaterial, index);
        }
      }
    }
    ProjectedShadow.s_commandBuffer.SetGlobalVector(ProjectedShadow.BLUR_OFFSETS_ID, Vector4.one * -0.6f);
    ProjectedShadow.s_commandBuffer.Blit((Texture) destTexture, (RenderTargetIdentifier) (Texture) ProjectedShadow.s_tempBuffer, this.MultiSampleMaterial);
    ProjectedShadow.s_commandBuffer.SetGlobalVector(ProjectedShadow.BLUR_OFFSETS_ID, Vector4.one * -0.8f);
    ProjectedShadow.s_commandBuffer.Blit((Texture) ProjectedShadow.s_tempBuffer, (RenderTargetIdentifier) (Texture) destTexture, this.MultiSampleMaterial);
    Graphics.ExecuteCommandBuffer(ProjectedShadow.s_commandBuffer);
    ProjectedShadow.s_commandBuffer.Clear();
  }

  private void CreateProjector()
  {
    if ((Object) this.m_ProjectorGameObject != (Object) null)
    {
      Object.Destroy((Object) this.m_ProjectorGameObject);
      this.m_ProjectorGameObject = (GameObject) null;
      this.m_ProjectorTransform = (Transform) null;
    }
    this.m_ProjectorGameObject = (GameObject) Object.Instantiate(Resources.Load("Prefabs/ShadowProjector"));
    this.m_Projector = this.m_ProjectorGameObject.GetComponent<DecalProjector>();
    this.m_ProjectorTransform = this.m_ProjectorGameObject.transform;
    this.m_ProjectorTransform.Rotate(90f, 0.0f, 0.0f);
    if ((Object) this.m_RootObject != (Object) null)
      this.m_ProjectorTransform.parent = this.m_RootObject.transform;
    this.m_Projector.NearClipPlane = 0.0f;
    this.m_Projector.FarClipPlane = this.m_ProjectionFarClip;
    this.m_Projector.OrthographicSize = this.m_AdjustedShadowProjectorSize;
    GameObjectUtils.SetHideFlags((Object) this.m_Projector, HideFlags.HideAndDontSave);
    this.m_ShadowMaterial = this.m_Projector.Material;
    this.m_projectorShadowDelay = true;
  }

  private void CreateRenderPlane()
  {
    if ((Object) this.m_PlaneGameObject != (Object) null)
      Object.DestroyImmediate((Object) this.m_PlaneGameObject);
    this.m_PlaneGameObject = new GameObject();
    this.m_PlaneGameObject.name = this.name + "_ContactShadowRenderPlane";
    if ((Object) this.m_RootObject != (Object) null)
      this.m_PlaneGameObject.transform.parent = this.m_RootObject.transform;
    this.m_PlaneGameObject.transform.localPosition = this.m_ContactOffset;
    this.m_PlaneGameObject.transform.localRotation = Quaternion.identity;
    this.m_PlaneGameObject.transform.localScale = new Vector3(0.98f, 1f, 0.98f);
    this.m_PlaneGameObject.AddComponent<MeshFilter>();
    this.m_PlaneGameObject.AddComponent<MeshRenderer>();
    GameObjectUtils.SetHideFlags((Object) this.m_PlaneGameObject, HideFlags.HideAndDontSave);
    Mesh mesh = new Mesh();
    mesh.name = "ContactShadowMeshPlane";
    float shadowProjectorSize1 = this.m_ShadowProjectorSize;
    float shadowProjectorSize2 = this.m_ShadowProjectorSize;
    mesh.vertices = new Vector3[4]
    {
      new Vector3(-shadowProjectorSize1, 0.0f, -shadowProjectorSize2),
      new Vector3(shadowProjectorSize1, 0.0f, -shadowProjectorSize2),
      new Vector3(-shadowProjectorSize1, 0.0f, shadowProjectorSize2),
      new Vector3(shadowProjectorSize1, 0.0f, shadowProjectorSize2)
    };
    mesh.uv = this.PLANE_UVS;
    mesh.normals = this.PLANE_NORMALS;
    mesh.triangles = this.PLANE_TRIANGLES;
    this.m_PlaneMesh = this.m_PlaneGameObject.GetComponent<MeshFilter>().mesh = mesh;
    this.m_PlaneMesh.RecalculateBounds();
    this.m_ContactShadowMaterial = this.ContactShadowMaterial;
    this.m_ContactShadowMaterial.color = ProjectedShadow.s_ShadowColor;
    if (!(bool) (Object) this.m_ContactShadowMaterial)
      return;
    this.m_PlaneGameObject.GetComponent<Renderer>().SetSharedMaterial(this.m_ContactShadowMaterial);
  }

  public void SetWorldScale(Transform xform, Vector3 scale)
  {
    GameObject gameObject = new GameObject();
    Transform transform = gameObject.transform;
    transform.parent = (Transform) null;
    transform.localRotation = Quaternion.identity;
    transform.localScale = Vector3.one;
    Transform parent = xform.parent;
    xform.parent = transform;
    xform.localScale = scale;
    xform.parent = parent;
    Object.Destroy((Object) gameObject);
  }
}
