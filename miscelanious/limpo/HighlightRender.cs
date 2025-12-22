using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class HighlightRender : MonoBehaviour
{
  private readonly string MULTISAMPLE_SHADER_NAME = "Custom/Selection/HighlightMultiSample";
  private readonly string MULTISAMPLE_BLEND_SHADER_NAME = "Custom/Selection/HighlightMultiSampleBlend";
  private readonly string BLEND_SHADER_NAME = "Custom/Selection/HighlightMaskBlend";
  private readonly string HIGHLIGHT_SHADER_NAME = "Custom/Selection/Highlight";
  private readonly string UNLIT_COLOR_SHADER_NAME = "Custom/UnlitColor";
  private readonly string UNLIT_GREY_SHADER_NAME = "Custom/Unlit/Color/Grey";
  private readonly string UNLIT_LIGHTGREY_SHADER_NAME = "Custom/Unlit/Color/LightGrey";
  private readonly string UNLIT_DARKGREY_SHADER_NAME = "Custom/Unlit/Color/DarkGrey";
  private readonly string UNLIT_BLACK_SHADER_NAME = "Custom/Unlit/Color/BlackOverlay";
  private readonly string UNLIT_WHITE_SHADER_NAME = "Custom/Unlit/Color/White";
  private static Material s_whiteMaterial;
  private static Material s_lightGreyMaterial;
  private static Material s_greyMaterial;
  private static Material s_darkGreyMaterial;
  private static Material s_blurMaterial;
  private static Material s_blurBlendMaterial;
  private static Material s_maskBlendMaterial;
  private const float RENDER_SIZE1 = 0.3f;
  private const float RENDER_SIZE2 = 0.3f;
  private const float RENDER_SIZE3 = 0.5f;
  private const float RENDER_SIZE4 = 0.92f;
  private const float ORTHO_SIZE1 = 0.2f;
  private const float ORTHO_SIZE2 = 0.25f;
  private const float ORTHO_SIZE3 = 0.01f;
  private const float ORTHO_SIZE4 = -0.05f;
  private const float BLUR_BLEND1 = 1.25f;
  private const float BLUR_BLEND2 = 1.25f;
  private const float BLUR_BLEND3 = 1f;
  private const float BLUR_BLEND4 = 1.5f;
  private const int SILHOUETTE_RENDER_SIZE = 256;
  private const int MAX_HIGHLIGHT_EXCLUDE_PARENT_SEARCH = 25;
  private static readonly int s_sid0 = Shader.PropertyToID("temp_id_0");
  private static readonly int s_sid1 = Shader.PropertyToID("temp_id_1");
  private static readonly int s_sid2 = Shader.PropertyToID("temp_id_2");
  private static readonly int s_blurOffsetsId = Shader.PropertyToID("_BlurOffsets");
  private static readonly int s_blendTexId = Shader.PropertyToID("_BlendTex");
  private static List<Renderer> s_cachedRenderers = new List<Renderer>();
  private static List<RenderCommand> s_cachedCommands = new List<RenderCommand>();
  private static List<Material> s_cachedMaterials = new List<Material>();
  public Transform m_RootTransform;
  public float m_SilouetteRenderSize = 1f;
  public float m_SilouetteClipSize = 1f;
  private GameObject m_RenderPlane;
  private float m_RenderScale = 1f;
  private Quaternion m_OrgRotation;
  private Vector3 m_OrgScale;
  private Shader m_MultiSampleShader;
  private Shader m_MultiSampleBlendShader;
  private Shader m_BlendShader;
  private Shader m_HighlightShader;
  private Shader m_UnlitColorShader;
  private Shader m_UnlitGreyShader;
  private Shader m_UnlitLightGreyShader;
  private Shader m_UnlitDarkGreyShader;
  private Shader m_UnlitBlackShader;
  private Shader m_UnlitWhiteShader;
  private RenderTexture m_CameraTexture;
  private float m_CameraOrthoSize;
  private Map<Renderer, bool> m_VisibilityStates;
  private Map<Transform, Vector3> m_ObjectsOrginalPosition;
  private int m_RenderSizeX = 256;
  private int m_RenderSizeY = 256;
  private HighlightRenderOverrides m_renderOverrides;
  private bool m_Initialized;

  protected void OnDisable()
  {
    if (this.m_VisibilityStates != null)
      this.m_VisibilityStates.Clear();
    if ((Object) this.m_CameraTexture != (Object) null)
    {
      if ((Object) RenderTexture.active == (Object) this.m_CameraTexture)
        RenderTexture.active = (RenderTexture) null;
      RenderTextureTracker.Get().DestroyRenderTexture(this.m_CameraTexture);
      this.m_CameraTexture = (RenderTexture) null;
    }
    this.m_Initialized = false;
  }

  protected void Initialize()
  {
    if (this.m_Initialized)
      return;
    this.m_Initialized = true;
    if ((Object) this.m_HighlightShader == (Object) null)
      this.m_HighlightShader = ShaderUtils.FindShader(this.HIGHLIGHT_SHADER_NAME);
    if (!(bool) (Object) this.m_HighlightShader)
    {
      Debug.LogError((object) ("Failed to load Highlight Shader: " + this.HIGHLIGHT_SHADER_NAME));
      this.enabled = false;
    }
    this.GetComponent<Renderer>().GetMaterial().shader = this.m_HighlightShader;
    if ((Object) this.m_MultiSampleShader == (Object) null)
      this.m_MultiSampleShader = ShaderUtils.FindShader(this.MULTISAMPLE_SHADER_NAME);
    if (!(bool) (Object) this.m_MultiSampleShader)
    {
      Debug.LogError((object) ("Failed to load Highlight Shader: " + this.MULTISAMPLE_SHADER_NAME));
      this.enabled = false;
    }
    if ((Object) this.m_MultiSampleBlendShader == (Object) null)
      this.m_MultiSampleBlendShader = ShaderUtils.FindShader(this.MULTISAMPLE_BLEND_SHADER_NAME);
    if (!(bool) (Object) this.m_MultiSampleBlendShader)
    {
      Debug.LogError((object) ("Failed to load Highlight Shader: " + this.MULTISAMPLE_BLEND_SHADER_NAME));
      this.enabled = false;
    }
    if ((Object) this.m_BlendShader == (Object) null)
      this.m_BlendShader = ShaderUtils.FindShader(this.BLEND_SHADER_NAME);
    if (!(bool) (Object) this.m_BlendShader)
    {
      Debug.LogError((object) ("Failed to load Highlight Shader: " + this.BLEND_SHADER_NAME));
      this.enabled = false;
    }
    if ((Object) this.m_RootTransform == (Object) null)
    {
      Transform parent = this.transform.parent.parent;
      this.m_RootTransform = !(bool) (Object) parent.GetComponent<ActorStateMgr>() ? parent : parent.parent;
      if ((Object) this.m_RootTransform == (Object) null)
      {
        Debug.LogError((object) "m_RootTransform is null. Highlighting disabled!");
        this.enabled = false;
      }
    }
    this.m_VisibilityStates = new Map<Renderer, bool>();
    HighlightSilhouetteInclude[] componentsInChildren = this.m_RootTransform.GetComponentsInChildren<HighlightSilhouetteInclude>();
    if (componentsInChildren != null)
    {
      foreach (Component component1 in componentsInChildren)
      {
        Renderer component2 = component1.gameObject.GetComponent<Renderer>();
        if (!((Object) component2 == (Object) null))
          this.m_VisibilityStates.Add(component2, false);
      }
    }
    this.m_UnlitColorShader = ShaderUtils.FindShader(this.UNLIT_COLOR_SHADER_NAME);
    if (!(bool) (Object) this.m_UnlitColorShader)
      Debug.LogError((object) ("Failed to load Highlight Rendering Shader: " + this.UNLIT_COLOR_SHADER_NAME));
    this.m_UnlitGreyShader = ShaderUtils.FindShader(this.UNLIT_GREY_SHADER_NAME);
    if (!(bool) (Object) this.m_UnlitGreyShader)
      Debug.LogError((object) ("Failed to load Highlight Rendering Shader: " + this.UNLIT_GREY_SHADER_NAME));
    this.m_UnlitLightGreyShader = ShaderUtils.FindShader(this.UNLIT_LIGHTGREY_SHADER_NAME);
    if (!(bool) (Object) this.m_UnlitLightGreyShader)
      Debug.LogError((object) ("Failed to load Highlight Rendering Shader: " + this.UNLIT_LIGHTGREY_SHADER_NAME));
    this.m_UnlitDarkGreyShader = ShaderUtils.FindShader(this.UNLIT_DARKGREY_SHADER_NAME);
    if (!(bool) (Object) this.m_UnlitDarkGreyShader)
      Debug.LogError((object) ("Failed to load Highlight Rendering Shader: " + this.UNLIT_DARKGREY_SHADER_NAME));
    this.m_UnlitBlackShader = ShaderUtils.FindShader(this.UNLIT_BLACK_SHADER_NAME);
    if (!(bool) (Object) this.m_UnlitBlackShader)
      Debug.LogError((object) ("Failed to load Highlight Rendering Shader: " + this.UNLIT_BLACK_SHADER_NAME));
    this.m_UnlitWhiteShader = ShaderUtils.FindShader(this.UNLIT_WHITE_SHADER_NAME);
    if (!(bool) (Object) this.m_UnlitWhiteShader)
      Debug.LogError((object) ("Failed to load Highlight Rendering Shader: " + this.UNLIT_WHITE_SHADER_NAME));
    if (!((Object) HighlightRender.s_whiteMaterial == (Object) null))
      return;
    HighlightRender.s_whiteMaterial = new Material(this.m_UnlitWhiteShader);
    HighlightRender.s_lightGreyMaterial = new Material(this.m_UnlitLightGreyShader);
    HighlightRender.s_greyMaterial = new Material(this.m_UnlitGreyShader);
    HighlightRender.s_darkGreyMaterial = new Material(this.m_UnlitDarkGreyShader);
    HighlightRender.s_blurMaterial = new Material(this.m_MultiSampleShader);
    HighlightRender.s_blurBlendMaterial = new Material(this.m_MultiSampleBlendShader);
    HighlightRender.s_maskBlendMaterial = new Material(this.m_BlendShader);
  }

  protected void Update()
  {
    if (!(bool) (Object) this.m_CameraTexture || !this.m_Initialized || this.m_CameraTexture.IsCreated())
      return;
    this.CreateSilhouetteTexture();
  }

  [ContextMenu("Export Silhouette Texture")]
  public void ExportSilhouetteTexture()
  {
    RenderTexture.active = this.m_CameraTexture;
    Texture2D tex = new Texture2D(this.m_RenderSizeX, this.m_RenderSizeY, TextureFormat.RGB24, false);
    tex.ReadPixels(new Rect(0.0f, 0.0f, (float) this.m_RenderSizeX, (float) this.m_RenderSizeY), 0, 0, false);
    tex.Apply();
    string path = Application.dataPath + "/SilhouetteTexture.png";
    File.WriteAllBytes(path, tex.EncodeToPNG());
    RenderTexture.active = (RenderTexture) null;
    Debug.Log((object) string.Format("Silhouette Texture Created: {0}", (object) path));
  }

  private static void DrawRenderers(CommandBuffer cmd, Material material)
  {
    foreach (RenderCommand cachedCommand in HighlightRender.s_cachedCommands)
      cmd.DrawRenderer(cachedCommand.Renderer, material, cachedCommand.MeshIndex);
  }

  private static void BlendBlit(
    CommandBuffer cmd,
    RenderTargetIdentifier src,
    RenderTargetIdentifier blend,
    RenderTargetIdentifier dst,
    float blur,
    Material material)
  {
    cmd.SetGlobalTexture(HighlightRender.s_blendTexId, blend);
    cmd.SetGlobalVector(HighlightRender.s_blurOffsetsId, Vector4.one * -blur);
    cmd.Blit(src, dst, material);
  }

  private void SetProjectionMatrix(CommandBuffer cmd, float orthoSize)
  {
    Matrix4x4 proj = Matrix4x4.Ortho(-orthoSize, orthoSize, -orthoSize, orthoSize, (float) (-(double) this.m_RenderScale + 1.0), this.m_RenderScale + 1f);
    cmd.SetProjectionMatrix(proj);
  }

  public void CreateSilhouetteTexture(bool force = false)
  {
    this.Initialize();
    if (!this.VisibilityStatesChanged() && !force)
      return;
    this.SetupRenderObjects();
    if ((Object) this.m_RenderPlane == (Object) null || this.m_RenderSizeX < 1 || this.m_RenderSizeY < 1)
      return;
    Renderer component1 = this.GetComponent<Renderer>();
    bool enabled = component1.enabled;
    component1.enabled = false;
    this.m_RootTransform.GetComponentsInChildren<Renderer>(HighlightRender.s_cachedRenderers);
    foreach (Renderer cachedRenderer in HighlightRender.s_cachedRenderers)
    {
      if (cachedRenderer.enabled)
      {
        MeshFilter component2 = cachedRenderer.GetComponent<MeshFilter>();
        if (!((Object) component2 == (Object) null) && !((Object) component2.sharedMesh == (Object) null))
        {
          cachedRenderer.GetSharedMaterials(HighlightRender.s_cachedMaterials);
          int num = Mathf.Min(component2.sharedMesh.subMeshCount, HighlightRender.s_cachedMaterials.Count);
          for (int index = 0; index < num; ++index)
          {
            Material cachedMaterial = HighlightRender.s_cachedMaterials[index];
            if ((Object) cachedMaterial != (Object) null && cachedMaterial.GetTag("Highlight", false) != "")
              HighlightRender.s_cachedCommands.Add(new RenderCommand()
              {
                Renderer = cachedRenderer,
                MeshIndex = index
              });
          }
        }
      }
    }
    HighlightRender.s_cachedRenderers.Clear();
    HighlightRender.s_cachedMaterials.Clear();
    RenderTargetIdentifier targetIdentifier1 = new RenderTargetIdentifier(HighlightRender.s_sid0);
    RenderTargetIdentifier targetIdentifier2 = new RenderTargetIdentifier(HighlightRender.s_sid1);
    RenderTargetIdentifier targetIdentifier3 = new RenderTargetIdentifier(HighlightRender.s_sid2);
    CommandBuffer commandBuffer = CommandBufferPool.Get("Create Silhouette Texture");
    int renderSizeX = this.m_RenderSizeX;
    int renderSizeY = this.m_RenderSizeY;
    FilterMode filter = FilterMode.Bilinear;
    RenderTextureFormat format = RenderTextureFormat.R8;
    int textureDepth = RenderTextureTracker.TEXTURE_DEPTH;
    this.m_CameraTexture.DiscardContents();
    Transform transform = this.m_RenderPlane.transform;
    Matrix4x4 inverse = Matrix4x4.TRS(transform.position, transform.rotation * Quaternion.Euler(90f, 180f, 0.0f), new Vector3(1f, 1f, -1f)).inverse;
    commandBuffer.SetViewMatrix(inverse);
    float num1 = (bool) (Object) this.m_renderOverrides ? this.m_renderOverrides.SilouetteRenderSize : this.m_SilouetteRenderSize;
    float num2 = (bool) (Object) this.m_renderOverrides ? this.m_renderOverrides.SilouetteClipSize : this.m_SilouetteClipSize;
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid0, (int) ((double) renderSizeX * 0.300000011920929), (int) ((double) renderSizeY * 0.300000011920929), textureDepth, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier1);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    this.SetProjectionMatrix(commandBuffer, this.m_CameraOrthoSize - 0.2f * num1);
    HighlightRender.DrawRenderers(commandBuffer, HighlightRender.s_darkGreyMaterial);
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid1, (int) ((double) renderSizeX * 0.300000011920929), (int) ((double) renderSizeY * 0.300000011920929), textureDepth, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier2);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    this.SetProjectionMatrix(commandBuffer, this.m_CameraOrthoSize - 0.25f * num1);
    HighlightRender.DrawRenderers(commandBuffer, HighlightRender.s_greyMaterial);
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid2, (int) ((double) renderSizeX * 0.300000011920929), (int) ((double) renderSizeY * 0.300000011920929), 0, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier3);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    HighlightRender.BlendBlit(commandBuffer, targetIdentifier1, targetIdentifier2, targetIdentifier3, 1.25f, HighlightRender.s_blurBlendMaterial);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid0);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid1);
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid0, (int) ((double) renderSizeX * 0.5), (int) ((double) renderSizeY * 0.5), textureDepth, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier1);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    this.SetProjectionMatrix(commandBuffer, this.m_CameraOrthoSize - 0.01f * num1);
    HighlightRender.DrawRenderers(commandBuffer, HighlightRender.s_lightGreyMaterial);
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid1, (int) ((double) renderSizeX * 0.5), (int) ((double) renderSizeY * 0.5), 0, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier2);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    HighlightRender.BlendBlit(commandBuffer, targetIdentifier3, targetIdentifier1, targetIdentifier2, 1.25f, HighlightRender.s_blurBlendMaterial);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid2);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid0);
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid0, renderSizeX, renderSizeY, textureDepth, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier1);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    this.SetProjectionMatrix(commandBuffer, this.m_CameraOrthoSize - -0.05f * num1);
    HighlightRender.DrawRenderers(commandBuffer, HighlightRender.s_lightGreyMaterial);
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid2, renderSizeX, renderSizeY, 0, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier3);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    HighlightRender.BlendBlit(commandBuffer, targetIdentifier2, targetIdentifier1, targetIdentifier3, 1f, HighlightRender.s_blurBlendMaterial);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid1);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid0);
    commandBuffer.SetGlobalVector(HighlightRender.s_blurOffsetsId, Vector4.one * -1.5f);
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid0, renderSizeX, renderSizeY, 0, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier1);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    commandBuffer.Blit(targetIdentifier3, targetIdentifier1, HighlightRender.s_blurMaterial);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid2);
    commandBuffer.GetTemporaryRT(HighlightRender.s_sid1, (int) ((double) renderSizeX * 0.920000016689301), (int) ((double) renderSizeY * 0.920000016689301), textureDepth, filter, format);
    commandBuffer.SetRenderTarget(targetIdentifier2);
    commandBuffer.ClearRenderTarget(true, true, Color.clear);
    this.SetProjectionMatrix(commandBuffer, this.m_CameraOrthoSize + 0.1f * num2);
    HighlightRender.DrawRenderers(commandBuffer, HighlightRender.s_whiteMaterial);
    HighlightRender.BlendBlit(commandBuffer, targetIdentifier1, targetIdentifier2, (RenderTargetIdentifier) (Texture) this.m_CameraTexture, 0.8f, HighlightRender.s_maskBlendMaterial);
    Graphics.ExecuteCommandBuffer(commandBuffer);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid0);
    commandBuffer.ReleaseTemporaryRT(HighlightRender.s_sid1);
    CommandBufferPool.Release(commandBuffer);
    component1.enabled = enabled;
    this.RestoreRenderObjects();
    HighlightRender.s_cachedCommands.Clear();
  }

  public RenderTexture SilhouetteTexture => this.m_CameraTexture;

  public bool isTextureCreated() => (bool) (Object) this.m_CameraTexture && this.m_CameraTexture.IsCreated();

  private void SetupRenderObjects()
  {
    if ((Object) this.m_RootTransform == (Object) null)
    {
      this.m_RenderPlane = (GameObject) null;
    }
    else
    {
      this.m_OrgRotation = this.m_RootTransform.rotation;
      this.m_OrgScale = this.m_RootTransform.localScale;
      this.SetWorldScale(this.m_RootTransform, Vector3.one);
      this.m_RootTransform.rotation = Quaternion.identity;
      Bounds bounds = this.GetComponent<Renderer>().bounds;
      float x = bounds.size.x;
      float num = bounds.size.z;
      if ((double) num < (double) bounds.size.y)
        num = bounds.size.y;
      if ((double) x > (double) num)
      {
        this.m_RenderSizeX = 256;
        this.m_RenderSizeY = (int) (256.0 * ((double) num / (double) x));
      }
      else
      {
        this.m_RenderSizeX = (int) (256.0 * ((double) x / (double) num));
        this.m_RenderSizeY = 256;
      }
      this.m_CameraOrthoSize = num * 0.5f;
      if ((Object) this.m_CameraTexture == (Object) null)
      {
        if (this.m_RenderSizeX < 1 || this.m_RenderSizeY < 1)
        {
          this.m_RenderSizeX = 256;
          this.m_RenderSizeY = 256;
        }
        this.m_CameraTexture = RenderTextureTracker.Get().CreateNewTexture(this.m_RenderSizeX, this.m_RenderSizeY, RenderTextureTracker.TEXTURE_DEPTH, RenderTextureFormat.R8);
      }
      HighlightState componentInChildren1 = this.m_RootTransform.GetComponentInChildren<HighlightState>();
      if ((Object) componentInChildren1 == (Object) null)
      {
        Debug.LogError((object) "Can not find Highlight(HighlightState component) object for selection highlighting.");
        this.m_RenderPlane = (GameObject) null;
      }
      else
      {
        componentInChildren1.transform.localPosition = Vector3.zero;
        HighlightRender componentInChildren2 = this.m_RootTransform.GetComponentInChildren<HighlightRender>();
        if ((Object) componentInChildren2 == (Object) null)
        {
          Debug.LogError((object) "Can not find render plane object(HighlightRender component) for selection highlighting.");
          this.m_RenderPlane = (GameObject) null;
        }
        else
        {
          this.m_RenderPlane = componentInChildren2.gameObject;
          this.m_RenderScale = HighlightRender.GetWorldScale(this.m_RenderPlane.transform).x;
        }
      }
    }
  }

  private void RestoreRenderObjects()
  {
    this.m_RootTransform.rotation = this.m_OrgRotation;
    this.m_RootTransform.localScale = this.m_OrgScale;
    this.m_RenderPlane = (GameObject) null;
  }

  private bool VisibilityStatesChanged()
  {
    bool flag1 = false;
    HighlightSilhouetteInclude[] componentsInChildren = this.m_RootTransform.GetComponentsInChildren<HighlightSilhouetteInclude>();
    List<Renderer> rendererList = new List<Renderer>();
    foreach (Component component1 in componentsInChildren)
    {
      Renderer component2 = component1.gameObject.GetComponent<Renderer>();
      if ((Object) component2 != (Object) null)
        rendererList.Add(component2);
    }
    foreach (Renderer key in rendererList)
    {
      bool flag2 = key.enabled && key.gameObject.activeInHierarchy;
      if (!this.m_VisibilityStates.ContainsKey(key))
      {
        this.m_VisibilityStates.Add(key, flag2);
        if (flag2)
          flag1 = true;
      }
      else if (this.m_VisibilityStates[key] != flag2)
      {
        this.m_VisibilityStates[key] = flag2;
        flag1 = true;
      }
    }
    return flag1;
  }

  public static Vector3 GetWorldScale(Transform transform)
  {
    Vector3 a = transform.localScale;
    for (Transform parent = transform.parent; (Object) parent != (Object) null; parent = parent.parent)
      a = Vector3.Scale(a, parent.localScale);
    return a;
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

  public void SetRenderOverrides(HighlightRenderOverrides renderOverrides) => this.m_renderOverrides = renderOverrides;
}
