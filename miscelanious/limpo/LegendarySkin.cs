using Hearthstone;
using LegendarySkins;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LegendarySkin : MonoBehaviour
{
  [Header("Camera")]
  public Camera RTTCamera;
  public int TextureSize;
  public GameObject LookAtJoint;
  [Header("Lighting")]
  public LegendarySkinLight DirectionalLight;
  public bool ShadowPassEnabled;
  public int ShadowTextureSize;
  public PortraitLighting LightSettings;
  [Header("Render Texture Settings")]
  public LegendarySkin.AntiAliasingSetting AntiAliasingLevel;
  [Range(0.0f, 1f)]
  public float ScissorRegion = 1f;
  [Header("Baking pose")]
  public AnimationClip PoseAnimation;
  public float ClipTime;
  private RenderTexture m_renderTexture;
  private HashSet<LegendarySkinDynamicResController> m_dynamicResolutionControllers = new HashSet<LegendarySkinDynamicResController>();
  private static Stack<int> s_freeSlots = new Stack<int>();
  private static int s_nextFreeSlot = 0;
  private int m_slot;
  private bool m_renderersDirty;
  private int m_dynamicResolution;
  private Renderer[] m_allRenderers;
  private List<LegendarySkin.RenderCommandWithPass> m_shadowRenderCommands;
  private List<RenderCommand> m_forwardRenderCommands;
  private CommandBuffer m_forwardCommandBuffer;
  private Matrix4x4 m_projectionMatrix;
  private Matrix4x4 m_viewMatrix;
  private Vector3 m_cameraPosition;
  private static readonly int s_MainTexID = Shader.PropertyToID("_MainTex");
  private static readonly int s_PortraitShadowMatrixID = Shader.PropertyToID("_PortraitShadowMatrix");
  private static readonly int s_PortraitLightDirectionID = Shader.PropertyToID("_PortraitLightDirection");
  private static readonly int s_PortraitLightColourID = Shader.PropertyToID("_PortraitLightColour");
  private static readonly int s_PortraitShadowMapID = Shader.PropertyToID("_PortraitShadowMap");
  private static readonly int s_PortraitRimLightColorID = Shader.PropertyToID("_PortraitRimLightColor");
  private static readonly int s_PortraitHairRimLightColorID = Shader.PropertyToID("_PortraitHairRimLightColor");
  private static readonly int s_PortraitShadowColorID = Shader.PropertyToID("_PortraitShadowColor");
  private static readonly int s_PortraitCameraPositionID = Shader.PropertyToID("_PortraitCameraPosition");
  private static readonly int s_SoftnessID = Shader.PropertyToID("_Softness");
  private static readonly int s_SoftnessFalloffID = Shader.PropertyToID("_SoftnessFalloff");
  private static readonly int s_SSSLightDirID = Shader.PropertyToID("_SSSLightDir");
  private static readonly int s_ViewDirID = Shader.PropertyToID("_ViewDir");
  private static readonly int s_CubemapRotationID = Shader.PropertyToID("_CubemapRotationMatrix");
  private static readonly int s_CubemapID = Shader.PropertyToID("_Cubemap");
  private static readonly int s_RimLightConeID = Shader.PropertyToID("_RimLightCone");
  private static readonly int s_RimLightConeDirectionID = Shader.PropertyToID("_RimLightConeDirection");
  private static readonly int s_RimLightFalloffID = Shader.PropertyToID("_RimLightFalloff");
  private static readonly int s_ShadowRenderTextureID = Shader.PropertyToID("_ShadowRenderTexture");
  private static readonly int s_InvResolutionID = Shader.PropertyToID("_InvResolution");
  public static bool DynamicResolutionEnabled = false;
  public static float DynamicResolutionScale = 1f;

  public Texture PortraitTexture => (Texture) this.m_renderTexture;

  private void Awake()
  {
    if ((UnityEngine.Object) this.RTTCamera != (UnityEngine.Object) null && (UnityEngine.Object) this.LookAtJoint != (UnityEngine.Object) null)
    {
      this.RTTCamera.transform.LookAt(this.LookAtJoint.transform, this.RTTCamera.transform.up);
      this.RTTCamera.transform.SetParent(this.LookAtJoint.transform, true);
    }
    int num1 = Mathf.NextPowerOfTwo(this.TextureSize);
    int num2 = 1;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (num1 > 512)
        num1 = Mathf.Max(512, num1 / 2);
    }
    else
    {
      switch (this.AntiAliasingLevel)
      {
        case LegendarySkin.AntiAliasingSetting.Off:
          num2 = 1;
          break;
        case LegendarySkin.AntiAliasingSetting.Two:
          num2 = 2;
          break;
        case LegendarySkin.AntiAliasingSetting.Four:
          num2 = 4;
          break;
        case LegendarySkin.AntiAliasingSetting.Eight:
          num2 = 8;
          break;
      }
    }
    this.m_renderTexture = new RenderTexture(num1, num1, 24, RenderTextureFormat.ARGB32);
    this.m_renderTexture.antiAliasing = num2;
    this.m_renderTexture.filterMode = FilterMode.Bilinear;
    this.m_dynamicResolution = num1;
    this.m_allRenderers = this.GetComponentsInChildren<Renderer>(false);
    foreach (Renderer allRenderer in this.m_allRenderers)
    {
      if (allRenderer is SkinnedMeshRenderer)
        (allRenderer as SkinnedMeshRenderer).updateWhenOffscreen = true;
      else
        allRenderer.enabled = false;
    }
    this.RTTCamera.aspect = 1f;
    this.RTTCamera.enabled = false;
  }

  private void OnEnable()
  {
    if (LegendarySkin.s_freeSlots.Count == 0)
      LegendarySkin.s_freeSlots.Push(LegendarySkin.s_nextFreeSlot++);
    this.m_slot = LegendarySkin.s_freeSlots.Pop();
    this.transform.SetPositionAndRotation(new Vector3(100f * (float) this.m_slot, -200f, -1000f), Quaternion.identity);
    this.CreateRenderCommands();
    this.BuildCommandBuffers();
    RenderPipelineManager.beginFrameRendering += new Action<ScriptableRenderContext, Camera[]>(this.OnBeginFrameRendering);
  }

  private void OnDisable()
  {
    RenderPipelineManager.beginFrameRendering -= new Action<ScriptableRenderContext, Camera[]>(this.OnBeginFrameRendering);
    LegendarySkin.s_freeSlots.Push(this.m_slot);
    this.m_slot = -1;
  }

  private void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.RTTCamera)
      this.RTTCamera.targetTexture = (RenderTexture) null;
    if (!(bool) (UnityEngine.Object) this.m_renderTexture)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_renderTexture);
    this.m_renderTexture = (RenderTexture) null;
  }

  private void LateUpdate()
  {
    if ((UnityEngine.Object) this.LookAtJoint != (UnityEngine.Object) null)
      this.m_renderersDirty = true;
    if (!this.m_renderersDirty)
      return;
    this.CreateRenderCommands();
    this.BuildCommandBuffers();
    this.m_renderersDirty = false;
  }

  private void OnBeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
  {
    int width = this.m_renderTexture.width;
    if (this.m_dynamicResolutionControllers.Count <= 0 && LegendarySkin.DynamicResolutionEnabled)
      return;
    int dynamicResolution1 = this.m_dynamicResolution;
    this.m_dynamicResolution = width;
    if (LegendarySkin.DynamicResolutionEnabled)
    {
      bool flag = true;
      float a = 0.0f;
      foreach (LegendarySkinDynamicResController resolutionController in this.m_dynamicResolutionControllers)
      {
        float size;
        switch (resolutionController.GetSize((IEnumerable<Camera>) cameras, out size))
        {
          case LegendarySkinDynamicResController.SizeResult.Bounded:
            a = Mathf.Max(a, size);
            continue;
          case LegendarySkinDynamicResController.SizeResult.MaxSize:
            flag = false;
            continue;
          default:
            continue;
        }
      }
      if (flag)
        this.m_dynamicResolution = Mathf.RoundToInt(Mathf.Min(a * LegendarySkin.DynamicResolutionScale, (float) width));
    }
    if (this.m_dynamicResolution <= 0)
      return;
    float dynamicResolution2 = (float) this.m_dynamicResolution / (float) width;
    foreach (LegendarySkinDynamicResController resolutionController in this.m_dynamicResolutionControllers)
      resolutionController.UpdateMaterial(dynamicResolution2);
    if (dynamicResolution1 != this.m_dynamicResolution)
      this.BuildCommandBuffers();
    Camera.SetupCurrent(this.RTTCamera);
    Graphics.ExecuteCommandBuffer(this.m_forwardCommandBuffer);
  }

  public void SetDirty() => this.m_renderersDirty = true;

  private void BuildCommandBuffers(RenderTexture renderTextureOverride = null, int shadowTextureOverride = 0)
  {
    this.m_forwardCommandBuffer = new CommandBuffer()
    {
      name = "PortraitRender"
    };
    Bounds bounds = new Bounds();
    foreach (LegendarySkin.RenderCommandWithPass shadowRenderCommand in this.m_shadowRenderCommands)
    {
      if ((double) bounds.extents.sqrMagnitude > 0.0)
        bounds.Encapsulate(shadowRenderCommand.Command.Renderer.bounds);
      else
        bounds = shadowRenderCommand.Command.Renderer.bounds;
    }
    Vector3 forward = Vector3.forward;
    Vector3 vector3_1 = Vector3.one;
    Matrix4x4 matrix4x4_1 = Matrix4x4.identity;
    if ((bool) (UnityEngine.Object) this.DirectionalLight)
    {
      Color color = this.DirectionalLight.color;
      vector3_1 = new Vector3(color.r, color.g, color.b) * this.DirectionalLight.intensity;
      forward = this.DirectionalLight.transform.forward;
      matrix4x4_1 = Matrix4x4.TRS(bounds.center, Quaternion.FromToRotation(Vector3.forward, -forward), Vector3.one * bounds.extents.magnitude).inverse;
    }
    Matrix4x4 proj = Matrix4x4.Ortho(-1f, 1f, -1f, 1f, -1f, 1f);
    bool flag1 = this.ShadowPassEnabled && this.m_shadowRenderCommands.Count > 0;
    if (flag1)
    {
      int num = Mathf.NextPowerOfTwo(this.ShadowTextureSize);
      if ((bool) UniversalInputManager.UsePhoneUI && num > 512)
        num = Mathf.Max(512, num / 2);
      if (shadowTextureOverride > 0)
        num = shadowTextureOverride;
      this.m_forwardCommandBuffer.GetTemporaryRT(LegendarySkin.s_ShadowRenderTextureID, num, num, 24, FilterMode.Bilinear, RenderTextureFormat.Depth);
      this.m_forwardCommandBuffer.SetRenderTarget((RenderTargetIdentifier) LegendarySkin.s_ShadowRenderTextureID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
      this.m_forwardCommandBuffer.SetViewport(new Rect(0.0f, 0.0f, (float) num, (float) num));
      this.m_forwardCommandBuffer.ClearRenderTarget(true, false, Color.clear);
      this.m_forwardCommandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, proj);
      this.m_forwardCommandBuffer.SetGlobalMatrix(LegendarySkin.s_PortraitShadowMatrixID, matrix4x4_1);
      foreach (LegendarySkin.RenderCommandWithPass shadowRenderCommand in this.m_shadowRenderCommands)
        this.m_forwardCommandBuffer.DrawRenderer(shadowRenderCommand.Command.Renderer, shadowRenderCommand.Command.Material, shadowRenderCommand.Command.MeshIndex, 1);
    }
    RenderTexture rt = renderTextureOverride ?? this.m_renderTexture;
    this.m_forwardCommandBuffer.SetRenderTarget((RenderTargetIdentifier) (Texture) rt, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
    int num1 = (UnityEngine.Object) renderTextureOverride == (UnityEngine.Object) null ? this.m_dynamicResolution : rt.width;
    this.m_forwardCommandBuffer.SetViewport(new Rect(0.0f, 0.0f, (float) num1, (float) num1));
    this.m_forwardCommandBuffer.SetViewProjectionMatrices(this.m_viewMatrix, this.m_projectionMatrix);
    this.m_forwardCommandBuffer.SetGlobalMatrix(LegendarySkin.s_PortraitShadowMatrixID, matrix4x4_1);
    this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitLightDirectionID, (Vector4) forward);
    this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitLightColourID, (Vector4) vector3_1);
    this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitCameraPositionID, (Vector4) this.m_cameraPosition);
    if (flag1)
      this.m_forwardCommandBuffer.SetGlobalTexture(LegendarySkin.s_PortraitShadowMapID, (RenderTargetIdentifier) LegendarySkin.s_ShadowRenderTextureID);
    else
      this.m_forwardCommandBuffer.SetGlobalTexture(LegendarySkin.s_PortraitShadowMapID, (RenderTargetIdentifier) (Texture) Texture2D.blackTexture);
    this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_SSSLightDirID, (Vector4) forward);
    this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_ViewDirID, (Vector4) this.RTTCamera.transform.forward);
    this.m_forwardCommandBuffer.SetGlobalFloat(LegendarySkin.s_InvResolutionID, (float) rt.width);
    if ((UnityEngine.Object) this.LightSettings != (UnityEngine.Object) null)
    {
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitRimLightColorID, (Vector4) this.LightSettings.RimLightColor);
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitHairRimLightColorID, (Vector4) this.LightSettings.HairRimLightColor);
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitShadowColorID, (Vector4) this.LightSettings.ShadowColor);
      this.m_forwardCommandBuffer.SetGlobalFloat(LegendarySkin.s_SoftnessID, this.LightSettings.DepthBias / 64f);
      this.m_forwardCommandBuffer.SetGlobalFloat(LegendarySkin.s_SoftnessFalloffID, Mathf.Exp(this.LightSettings.SoftnessFalloff));
      Matrix4x4 matrix4x4_2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, this.LightSettings.CubemapRotation * 360f, 0.0f), Vector3.one);
      this.m_forwardCommandBuffer.SetGlobalMatrix(LegendarySkin.s_CubemapRotationID, matrix4x4_2);
      this.m_forwardCommandBuffer.SetGlobalTexture(LegendarySkin.s_CubemapID, (RenderTargetIdentifier) this.LightSettings.Cubemap);
      Vector4 vector4 = new Vector4();
      float f1 = (float) Math.PI / 180f * this.LightSettings.RimLightDirection;
      vector4.x = Mathf.Cos(f1);
      vector4.y = Mathf.Sin(f1);
      double f2 = Math.PI / 180.0 * (double) Mathf.Clamp(this.LightSettings.RimLightAngle, 0.0f, 360f) * 0.5;
      float a = Mathf.Cos((float) f2);
      float num2 = Mathf.Cos((float) (f2 * (1.0 - (double) this.LightSettings.RimLightAngleSoftness)));
      vector4.z = Mathf.Min(a, num2 - Mathf.Epsilon);
      vector4.w = num2;
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_RimLightConeID, vector4);
      Vector3 vector3_2 = (Vector3) (this.RTTCamera.cameraToWorldMatrix * (Vector4) new Vector3(vector4.x, vector4.y, 0.0f));
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_RimLightConeDirectionID, (Vector4) vector3_2);
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_RimLightFalloffID, (Vector4) new Vector2()
      {
        x = (this.LightSettings.RimLightMinNormal - Mathf.Epsilon),
        y = (this.LightSettings.RimLightMaxNormal + Mathf.Epsilon)
      });
    }
    else
    {
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitRimLightColorID, (Vector4) Color.white);
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitHairRimLightColorID, (Vector4) Color.white);
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_PortraitShadowColorID, (Vector4) Color.black);
      this.m_forwardCommandBuffer.SetGlobalFloat(LegendarySkin.s_SoftnessID, 1f / 64f);
      this.m_forwardCommandBuffer.SetGlobalFloat(LegendarySkin.s_SoftnessFalloffID, Mathf.Exp(4f));
      this.m_forwardCommandBuffer.SetGlobalMatrix(LegendarySkin.s_CubemapRotationID, Matrix4x4.identity);
      this.m_forwardCommandBuffer.SetGlobalTexture(LegendarySkin.s_CubemapID, (RenderTargetIdentifier) (Texture) Texture2D.blackTexture);
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_RimLightConeID, new Vector4(1f, 0.0f, -0.1f, 0.1f));
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_RimLightConeDirectionID, new Vector4(1f, 0.0f, 0.0f, 0.0f));
      this.m_forwardCommandBuffer.SetGlobalVector(LegendarySkin.s_RimLightFalloffID, (Vector4) new Vector2(0.85f, 0.95f));
    }
    bool flag2 = (double) this.ScissorRegion < 1.0 - (double) Mathf.Epsilon;
    this.m_forwardCommandBuffer.ClearRenderTarget(true, true, Color.clear);
    if (flag2)
      this.m_forwardCommandBuffer.EnableScissorRect(new Rect(0.0f, 0.0f, (float) num1, (float) num1 * this.ScissorRegion));
    foreach (RenderCommand forwardRenderCommand in this.m_forwardRenderCommands)
      this.m_forwardCommandBuffer.DrawRenderer(forwardRenderCommand.Renderer, forwardRenderCommand.Material, forwardRenderCommand.MeshIndex, 0);
    if (!flag2)
      return;
    this.m_forwardCommandBuffer.DisableScissorRect();
  }

  private void CreateRenderCommands(ISkinMaterialProcessor materialProcessor = null)
  {
    this.m_projectionMatrix = this.RTTCamera.projectionMatrix;
    this.m_viewMatrix = this.RTTCamera.worldToCameraMatrix;
    this.m_cameraPosition = this.RTTCamera.transform.position;
    this.m_shadowRenderCommands = new List<LegendarySkin.RenderCommandWithPass>();
    this.m_forwardRenderCommands = new List<RenderCommand>();
    foreach (Renderer allRenderer in this.m_allRenderers)
    {
      List<Material> materialList = new List<Material>();
      allRenderer.GetSharedMaterials(materialList);
      MeshRenderer meshRenderer = allRenderer as MeshRenderer;
      SkinnedMeshRenderer skinnedMeshRenderer = allRenderer as SkinnedMeshRenderer;
      int num = 1;
      if ((bool) (UnityEngine.Object) meshRenderer)
      {
        MeshFilter component = allRenderer.GetComponent<MeshFilter>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null) && !((UnityEngine.Object) component.sharedMesh == (UnityEngine.Object) null))
          num = component.sharedMesh.subMeshCount;
        else
          continue;
      }
      if ((bool) (UnityEngine.Object) skinnedMeshRenderer)
        num = skinnedMeshRenderer.sharedMesh.subMeshCount;
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = index1;
        if (index2 >= materialList.Count)
          index2 = 0;
        Material material = materialList[index2];
        if (materialProcessor != null)
          material = materialProcessor.ProcessMaterial(material);
        List<RenderCommand> forwardRenderCommands = this.m_forwardRenderCommands;
        RenderCommand renderCommand1 = new RenderCommand();
        renderCommand1.Renderer = allRenderer;
        renderCommand1.Material = material;
        renderCommand1.MeshIndex = index1;
        RenderCommand renderCommand2 = renderCommand1;
        forwardRenderCommands.Add(renderCommand2);
        if (this.ShadowPassEnabled && material.GetTag("RenderType", false) == "LegendaryPortrait" && allRenderer.shadowCastingMode != ShadowCastingMode.Off)
        {
          int passCount = material.passCount;
          for (int pass = 0; pass < passCount; ++pass)
          {
            if (material.GetPassName(pass) == "Shadow Pass")
            {
              List<LegendarySkin.RenderCommandWithPass> shadowRenderCommands = this.m_shadowRenderCommands;
              LegendarySkin.RenderCommandWithPass renderCommandWithPass1 = new LegendarySkin.RenderCommandWithPass();
              ref LegendarySkin.RenderCommandWithPass local = ref renderCommandWithPass1;
              renderCommand1 = new RenderCommand();
              renderCommand1.Renderer = allRenderer;
              renderCommand1.Material = material;
              renderCommand1.MeshIndex = index1;
              RenderCommand renderCommand3 = renderCommand1;
              local.Command = renderCommand3;
              renderCommandWithPass1.ShaderPass = pass;
              LegendarySkin.RenderCommandWithPass renderCommandWithPass2 = renderCommandWithPass1;
              shadowRenderCommands.Add(renderCommandWithPass2);
              break;
            }
          }
        }
      }
    }
    this.m_forwardRenderCommands.Sort(new Comparison<RenderCommand>(LegendarySkin.SortRenderCommands));
    this.m_shadowRenderCommands.Sort(new Comparison<LegendarySkin.RenderCommandWithPass>(LegendarySkin.SortRenderCommands));
  }

  public void AddDynamicResController(LegendarySkinDynamicResController controller) => this.m_dynamicResolutionControllers.Add(controller);

  public void RemoveDynamicResController(LegendarySkinDynamicResController controller) => this.m_dynamicResolutionControllers.Remove(controller);

  private static int SortRenderCommands(RenderCommand a, RenderCommand b) => a.Material.renderQueue == b.Material.renderQueue ? a.Renderer.rendererPriority - b.Renderer.rendererPriority : a.Material.renderQueue - b.Material.renderQueue;

  private static int SortRenderCommands(
    LegendarySkin.RenderCommandWithPass a,
    LegendarySkin.RenderCommandWithPass b)
  {
    return LegendarySkin.SortRenderCommands(a.Command, b.Command);
  }

  public enum AntiAliasingSetting
  {
    Off,
    Two,
    Four,
    Eight,
  }

  private struct RenderCommandWithPass
  {
    public RenderCommand Command;
    public int ShaderPass;
  }
}
