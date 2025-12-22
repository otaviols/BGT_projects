using Blizzard.T5.Core;
using Blizzard.T5.Services;
using System.Collections;
using UnityEngine;

public class ShaderPreCompiler : MonoBehaviour
{
  private readonly string[] GOLDEN_UBER_KEYWORDS1 = new string[2]
  {
    "FX3_ADDBLEND",
    "FX3_ALPHABLEND"
  };
  private readonly string[] GOLDEN_UBER_KEYWORDS2 = new string[3]
  {
    "LAYER3",
    "FX3_FLOWMAP",
    "LAYER4"
  };
  private readonly Vector3[] MESH_VERTS = new Vector3[3]
  {
    Vector3.zero,
    Vector3.zero,
    Vector3.zero
  };
  private readonly Vector2[] MESH_UVS = new Vector2[3]
  {
    new Vector2(0.0f, 0.0f),
    new Vector2(1f, 0.0f),
    new Vector2(0.0f, 1f)
  };
  private readonly Vector3[] MESH_NORMALS = new Vector3[3]
  {
    Vector3.up,
    Vector3.up,
    Vector3.up
  };
  private readonly Vector4[] MESH_TANGENTS = new Vector4[3]
  {
    new Vector4(1f, 0.0f, 0.0f, 0.0f),
    new Vector4(1f, 0.0f, 0.0f, 0.0f),
    new Vector4(1f, 0.0f, 0.0f, 0.0f)
  };
  private readonly int[] MESH_TRIANGLES = new int[3]
  {
    2,
    1,
    0
  };
  public Shader m_GoldenUberShader;
  public Shader[] m_StartupCompileShaders;
  public Shader[] m_SceneChangeCompileShaders;
  protected static Map<string, Shader> s_shaderCache = new Map<string, Shader>();
  private bool SceneChangeShadersCompiled;
  private bool PremiumShadersCompiled;
  private IGraphicsManager m_graphicsManager;

  private void Awake() => this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();

  private void Start()
  {
    if (this.m_graphicsManager.isVeryLowQualityDevice())
    {
      Debug.Log((object) "ShaderPreCompiler: Disabled, very low quality mode");
    }
    else
    {
      if (this.m_graphicsManager.RenderQualityLevel != GraphicsQuality.Low)
        this.StartCoroutine(this.WarmupShaders(this.m_StartupCompileShaders));
      SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.WarmupSceneChangeShader));
      this.AddShader(this.m_GoldenUberShader.name, this.m_GoldenUberShader);
      foreach (Shader startupCompileShader in this.m_StartupCompileShaders)
      {
        if (!((Object) startupCompileShader == (Object) null))
          this.AddShader(startupCompileShader.name, startupCompileShader);
      }
      foreach (Shader changeCompileShader in this.m_SceneChangeCompileShaders)
      {
        if (!((Object) changeCompileShader == (Object) null))
          this.AddShader(changeCompileShader.name, changeCompileShader);
      }
    }
  }

  public static Shader GetShader(string shaderName)
  {
    Shader shader1;
    if (ShaderPreCompiler.s_shaderCache.TryGetValue(shaderName, out shader1))
      return shader1;
    Shader shader2 = Shader.Find(shaderName);
    if ((Object) shader2 != (Object) null)
      ShaderPreCompiler.s_shaderCache.Add(shaderName, shader2);
    return shader2;
  }

  private void AddShader(string shaderName, Shader shader)
  {
    if (ShaderPreCompiler.s_shaderCache.ContainsKey(shaderName))
      return;
    ShaderPreCompiler.s_shaderCache.Add(shaderName, shader);
  }

  private void WarmupSceneChangeShader(
    SceneMgr.Mode prevMode,
    PegasusScene prevScene,
    object userData)
  {
    if ((SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY || SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER || SceneMgr.Get().GetMode() == SceneMgr.Mode.BACON_COLLECTION || SceneMgr.Get().IsInTavernBrawlMode()) && Network.ShouldBeConnectedToAurora())
    {
      this.StartCoroutine(this.WarmupGoldenUberShader());
      this.PremiumShadersCompiled = true;
    }
    if (prevMode != SceneMgr.Mode.HUB || this.SceneChangeShadersCompiled)
      return;
    this.SceneChangeShadersCompiled = true;
    if (this.m_graphicsManager.RenderQualityLevel != GraphicsQuality.Low)
      this.StartCoroutine(this.WarmupShaders(this.m_SceneChangeCompileShaders));
    if (!this.SceneChangeShadersCompiled || !this.PremiumShadersCompiled)
      return;
    SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.WarmupSceneChangeShader));
  }

  private IEnumerator WarmupGoldenUberShader()
  {
    float totalTime = 0.0f;
    string[] strArray1 = this.GOLDEN_UBER_KEYWORDS1;
    for (int index1 = 0; index1 < strArray1.Length; ++index1)
    {
      string kw1 = strArray1[index1];
      string[] strArray2 = this.GOLDEN_UBER_KEYWORDS2;
      for (int index2 = 0; index2 < strArray2.Length; ++index2)
      {
        string str = strArray2[index2];
        ShaderVariantCollection variantCollection = new ShaderVariantCollection();
        variantCollection.Add(new ShaderVariantCollection.ShaderVariant()
        {
          shader = this.m_GoldenUberShader,
          keywords = new string[2]{ kw1, str }
        });
        float realtimeSinceStartup1 = Time.realtimeSinceStartup;
        variantCollection.WarmUp();
        float realtimeSinceStartup2 = Time.realtimeSinceStartup;
        totalTime += realtimeSinceStartup2 - realtimeSinceStartup1;
        Log.Graphics.Print(string.Format("Golden Uber Shader Compile: {0} Keywords: {1}, {2} ({3}s)", (object) this.m_GoldenUberShader.name, (object) kw1, (object) str, (object) (float) ((double) realtimeSinceStartup2 - (double) realtimeSinceStartup1)));
        yield return (object) null;
      }
      strArray2 = (string[]) null;
      kw1 = (string) null;
    }
    strArray1 = (string[]) null;
    Log.Graphics.Print("Profiling Shader Warmup: " + (object) totalTime);
  }

  private IEnumerator WarmupShaders(Shader[] shaders)
  {
    float totalTime = 0.0f;
    Shader[] shaderArray = shaders;
    for (int index = 0; index < shaderArray.Length; ++index)
    {
      Shader shader = shaderArray[index];
      if (!((Object) shader == (Object) null))
      {
        ShaderVariantCollection variantCollection = new ShaderVariantCollection();
        variantCollection.Add(new ShaderVariantCollection.ShaderVariant()
        {
          shader = shader
        });
        float realtimeSinceStartup1 = Time.realtimeSinceStartup;
        variantCollection.WarmUp();
        float realtimeSinceStartup2 = Time.realtimeSinceStartup;
        totalTime += realtimeSinceStartup2 - realtimeSinceStartup1;
        Log.Graphics.Print(string.Format("Shader Compile: {0} ({1}s)", (object) shader.name, (object) (float) ((double) realtimeSinceStartup2 - (double) realtimeSinceStartup1)));
        yield return (object) null;
      }
    }
    shaderArray = (Shader[]) null;
  }

  private GameObject CreateMesh(string name)
  {
    GameObject mesh = new GameObject();
    mesh.name = name;
    mesh.transform.parent = this.gameObject.transform;
    mesh.transform.localPosition = Vector3.zero;
    mesh.transform.localRotation = Quaternion.identity;
    mesh.transform.localScale = Vector3.one;
    mesh.AddComponent<MeshFilter>();
    mesh.AddComponent<MeshRenderer>();
    mesh.GetComponent<MeshFilter>().mesh = new Mesh()
    {
      vertices = this.MESH_VERTS,
      uv = this.MESH_UVS,
      normals = this.MESH_NORMALS,
      tangents = this.MESH_TANGENTS,
      triangles = this.MESH_TRIANGLES
    };
    return mesh;
  }

  private Material CreateMaterial(string name, Shader shader)
  {
    Material material = new Material(shader);
    material.name = name;
    return material;
  }
}
