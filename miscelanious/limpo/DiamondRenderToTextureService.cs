using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

public class DiamondRenderToTextureService : IService
{
  private const float MIN_OFFSET_DISTANCE = -4000f;
  private const float CAMERA_SIZE = 3.45f;
  private const float CAMERA_NEAR_CLIP = -0.3f;
  private const float CAMERA_FAR_CLIP = 15f;
  private const int LAYER_MASK = 23;
  private const int RENDER_TEXTURE_SIZE = 1024;
  private const float CAMERA_PIXEL_SIZE = 0.006738281f;
  private static readonly Vector3 CAMERA_OFFSET = new Vector3(0.0f, 20f, 0.0f);
  private static readonly Vector3 OFFSCREEN_POS = new Vector3(-4000f, -4000f, -4000f);
  private static readonly Vector3 DEFAULT_ATLAS_POSITION = DiamondRenderToTextureService.OFFSCREEN_POS - new Vector3(3.45f, 0.0f, 3.45f);
  private static readonly ProfilerMarker s_lateUpdateProfiler = new ProfilerMarker("DiamondRenderToTextureAtlas.LateUpdate");
  private static readonly ProfilerMarker s_removeUnusedProfiler = new ProfilerMarker("DiamondRenderToTextureAtlas.RemoveUnusedTextures");
  private static readonly ProfilerMarker s_renderAllAtlasesProfiler = new ProfilerMarker("DiamondRenderToTextureAtlas.RenderAllAtlases");
  private Dictionary<int, DiamondRenderToTextureService.TextureReference> m_textures = new Dictionary<int, DiamondRenderToTextureService.TextureReference>();
  private List<DiamondRenderToTextureAtlas> m_atlases = new List<DiamondRenderToTextureAtlas>();
  private GameObject m_containerObject;
  private GameObject m_itemsContainerObject;
  private Vector3 m_atlasOriginPosition;
  private int m_lastAddedAtlas;
  private Quaternion m_directionToCamera;
  private CommandBuffer m_atlasFilterCommandBuffer;
  private bool m_dirty;
  private List<DiamondRenderToTexture> m_texturesToRemove = new List<DiamondRenderToTexture>();

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DiamondRenderToTextureService toTextureService = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    toTextureService.m_lastAddedAtlas = 0;
    toTextureService.SetupObjects();
    Processor.RegisterLateUpdateDelegate(new Action(toTextureService.LateUpdate));
    return false;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
  }

  private void LateUpdate()
  {
    if (!this.NeedsUpdate())
      return;
    this.RemoveUnusedTextures();
    this.RenderAllAtlases();
  }

  public bool Register(DiamondRenderToTexture r2t)
  {
    if (!(bool) (UnityEngine.Object) r2t.m_ObjectToRender)
      return false;
    int instanceId1 = r2t.m_ObjectToRender.GetInstanceID();
    int instanceId2 = r2t.GetInstanceID();
    DiamondRenderToTextureService.TextureReference reference;
    if (this.m_textures.TryGetValue(instanceId2, out reference))
    {
      if (reference.RenderingObjectId != r2t.m_ObjectToRender.GetInstanceID())
      {
        this.RemoveTexture(reference);
      }
      else
      {
        if (reference.Remove)
        {
          reference.Remove = false;
          this.m_textures[instanceId2] = reference;
        }
        return true;
      }
    }
    if (!r2t.m_AllowRepetition)
    {
      foreach (KeyValuePair<int, DiamondRenderToTextureService.TextureReference> texture in this.m_textures)
      {
        if (r2t.IsEqual(texture.Value.Texture))
          return false;
      }
    }
    DiamondRenderToTextureAtlas atlas = this.AppendToAtlas(r2t);
    DiamondRenderToTextureService.TextureReference textureReference = new DiamondRenderToTextureService.TextureReference()
    {
      Texture = r2t,
      Atlas = atlas,
      RenderingObjectId = instanceId1
    };
    if (r2t.m_HideRenderObject)
    {
      GameObject gameObject = new GameObject("R2T_" + r2t.name);
      gameObject.transform.parent = this.m_itemsContainerObject.transform;
      r2t.transform.parent = gameObject.transform;
      r2t.m_ObjectToRender.transform.parent = gameObject.transform;
      textureReference.Container = gameObject;
    }
    this.m_textures.Add(instanceId2, textureReference);
    this.m_dirty = true;
    return true;
  }

  public void Unregister(DiamondRenderToTexture r2t)
  {
    int instanceId = r2t.GetInstanceID();
    DiamondRenderToTextureService.TextureReference textureReference;
    if (!this.m_textures.TryGetValue(instanceId, out textureReference))
      return;
    textureReference.Remove = true;
    this.m_textures[instanceId] = textureReference;
    this.m_texturesToRemove.Add(r2t);
    this.m_dirty = true;
  }

  private void SetupObjects()
  {
    this.m_containerObject = new GameObject("AtlasedRenderToTexture");
    this.m_containerObject.transform.position = DiamondRenderToTextureService.OFFSCREEN_POS;
    this.m_itemsContainerObject = new GameObject("Items");
    this.m_itemsContainerObject.transform.parent = this.m_containerObject.transform;
    UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.m_containerObject);
    this.m_directionToCamera = Quaternion.LookRotation(Vector3.up, Vector3.forward);
  }

  private bool NeedsUpdate() => this.m_dirty;

  private void RemoveUnusedTextures()
  {
    if (this.m_texturesToRemove.Count <= 0)
      return;
    foreach (UnityEngine.Object @object in this.m_texturesToRemove)
    {
      DiamondRenderToTextureService.TextureReference reference;
      if (this.m_textures.TryGetValue(@object.GetInstanceID(), out reference) && reference.Remove)
        this.RemoveTexture(reference);
    }
    this.CleanAtlases();
    this.m_texturesToRemove.Clear();
  }

  private void RemoveTexture(
    DiamondRenderToTextureService.TextureReference reference)
  {
    if ((UnityEngine.Object) reference.Texture == (UnityEngine.Object) null)
      return;
    reference.Atlas.Remove(reference.Texture);
    this.m_textures.Remove(reference.Texture.GetInstanceID());
    if (!(bool) (UnityEngine.Object) reference.Container)
      return;
    reference.Texture.RestoreOriginalParents();
    UnityEngine.Object.Destroy((UnityEngine.Object) reference.Container);
    reference.Container = (GameObject) null;
  }

  private DiamondRenderToTextureAtlas AppendToAtlas(
    DiamondRenderToTexture r2t)
  {
    foreach (DiamondRenderToTextureAtlas atlase in this.m_atlases)
    {
      if (atlase.Insert(r2t))
        return atlase;
    }
    this.m_atlases.Add(new DiamondRenderToTextureAtlas(this.m_lastAddedAtlas, 1024, 1024));
    ++this.m_lastAddedAtlas;
    DiamondRenderToTextureAtlas atlase1 = this.m_atlases[this.m_lastAddedAtlas - 1];
    atlase1.Insert(r2t);
    return atlase1;
  }

  private void RenderAllAtlases()
  {
    bool flag = false;
    this.m_atlasOriginPosition = DiamondRenderToTextureService.DEFAULT_ATLAS_POSITION;
    foreach (DiamondRenderToTextureAtlas atlase in this.m_atlases)
    {
      if (atlase.Dirty || atlase.IsRealTime)
        this.RenderAtlas(atlase, this.m_atlasOriginPosition);
      flag |= atlase.IsRealTime;
    }
    this.m_dirty = flag;
  }

  private void RenderAtlas(DiamondRenderToTextureAtlas atlas, Vector3 atlasOrigin)
  {
    foreach (DiamondRenderToTextureAtlas.RegisteredTexture registeredTexture in atlas.RegisteredTextures)
    {
      DiamondRenderToTexture diamondRenderToTexture = registeredTexture.DiamondRenderToTexture;
      if ((bool) (UnityEngine.Object) diamondRenderToTexture)
      {
        diamondRenderToTexture.PushTransform();
        if (diamondRenderToTexture.m_HideRenderObject)
          diamondRenderToTexture.m_ObjectToRender.SetActive(true);
        this.PositionObjectForAtlas(registeredTexture, atlasOrigin);
      }
    }
    atlas.Render();
    foreach (DiamondRenderToTextureAtlas.RegisteredTexture registeredTexture in atlas.RegisteredTextures)
    {
      DiamondRenderToTexture diamondRenderToTexture = registeredTexture.DiamondRenderToTexture;
      if ((bool) (UnityEngine.Object) diamondRenderToTexture && !diamondRenderToTexture.m_HideRenderObject)
      {
        diamondRenderToTexture.HasAtlasPosition = false;
        diamondRenderToTexture.PopTransform();
      }
    }
    atlas.Dirty = false;
  }

  private void PositionObjectForAtlas(
    DiamondRenderToTextureAtlas.RegisteredTexture texture,
    Vector3 atlasPosition)
  {
    DiamondRenderToTexture diamondRenderToTexture = texture.DiamondRenderToTexture;
    if (diamondRenderToTexture.m_HideRenderObject && diamondRenderToTexture.HasAtlasPosition && diamondRenderToTexture.MaintainsAtlasPosition())
      diamondRenderToTexture.transform.hasChanged = false;
    else if (!diamondRenderToTexture.m_ObjectToRender.activeInHierarchy)
    {
      diamondRenderToTexture.transform.hasChanged = false;
    }
    else
    {
      diamondRenderToTexture.ResetTransform(atlasPosition);
      if (diamondRenderToTexture.HasAtlasPosition)
      {
        diamondRenderToTexture.RestoreAtlasPosition();
      }
      else
      {
        float atlasPosition1 = this.ScaleObjectToAtlasPosition(texture);
        Quaternion rotationApplied = this.RotateTowardsCamera(texture);
        this.MoveToAtlasPosition(texture, atlasPosition, atlasPosition1, rotationApplied);
      }
      diamondRenderToTexture.CaptureAtlasPosition();
      diamondRenderToTexture.RestoreParents();
      diamondRenderToTexture.transform.hasChanged = false;
    }
  }

  private float ScaleObjectToAtlasPosition(
    DiamondRenderToTextureAtlas.RegisteredTexture texture)
  {
    DiamondRenderToTexture diamondRenderToTexture = texture.DiamondRenderToTexture;
    float b = 0.006738281f * (float) texture.AtlasPosition.height / diamondRenderToTexture.WorldBounds.x;
    float atlasPosition = Mathf.Max(0.006738281f * (float) texture.AtlasPosition.width / diamondRenderToTexture.WorldBounds.y, b);
    Vector3 vector3_1 = Vector3.one * atlasPosition;
    Transform transform = diamondRenderToTexture.m_ObjectToRender.transform;
    Vector3 localScale = transform.localScale;
    if (localScale == vector3_1)
      return 1f;
    Vector3 vector3_2 = localScale * atlasPosition;
    transform.localScale = vector3_2;
    diamondRenderToTexture.transform.localScale *= atlasPosition;
    return atlasPosition;
  }

  private void MoveToAtlasPosition(
    DiamondRenderToTextureAtlas.RegisteredTexture texture,
    Vector3 atlasOrigin,
    float scaleApplied,
    Quaternion rotationApplied)
  {
    DiamondRenderToTexture diamondRenderToTexture = texture.DiamondRenderToTexture;
    Transform transform = diamondRenderToTexture.transform;
    Vector3 vector3_1 = 0.006738281f * new Vector3((float) texture.AtlasPosition.x, 0.0f, (float) texture.AtlasPosition.y);
    diamondRenderToTexture.m_ObjectToRender.transform.position = atlasOrigin + vector3_1 - diamondRenderToTexture.WorldPivotOffset;
    Vector3 vector3_2 = atlasOrigin + vector3_1 - diamondRenderToTexture.WorldPivotOffset;
    transform.position = vector3_2;
  }

  private Quaternion RotateTowardsCamera(
    DiamondRenderToTextureAtlas.RegisteredTexture texture)
  {
    DiamondRenderToTexture diamondRenderToTexture = texture.DiamondRenderToTexture;
    Transform transform1 = diamondRenderToTexture.m_ObjectToRender.transform;
    Transform transform2 = diamondRenderToTexture.transform;
    transform2.rotation = Quaternion.RotateTowards(transform2.rotation, this.m_directionToCamera, 360f);
    transform1.eulerAngles = Vector3.zero;
    return Quaternion.identity;
  }

  private void CleanAtlases()
  {
    for (int index = this.m_atlases.Count - 1; index >= 0; --index)
    {
      DiamondRenderToTextureAtlas atlase = this.m_atlases[index];
      if (atlase.IsEmpty())
      {
        atlase.Destroy();
        this.m_atlases.RemoveAt(index);
        --this.m_lastAddedAtlas;
      }
    }
  }

  private struct TextureReference
  {
    public DiamondRenderToTexture Texture;
    public DiamondRenderToTextureAtlas Atlas;
    public GameObject Container;
    public int RenderingObjectId;
    public bool Remove;
  }
}
