using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomFrameController : IDisposable
{
  private readonly MeshFilter m_meshFilter;
  private readonly MeshRenderer m_meshRenderer;
  private readonly int m_originalMatIdx;
  private readonly int m_originalFrameMatIdx;
  private GameObject m_customFrameObject;
  private AssetHandle<GameObject> m_frameHandle;
  private CustomFrameDef m_frameDef;
  private HighlightState m_highlightState;
  private HighlightRender m_highlightRender;
  private Texture2D m_originalHighlightSilouetteTexture;
  private Vector3 m_originalHighlightPosition;
  private Vector3 m_originalHighlightScale;
  private Material m_originalPortraitMaterial;

  public AssetReference FrameAssetReference { get; private set; }

  public LegendarySkinDynamicResController DynamicResolutionController { get; private set; }

  public int PortraitMatIdx => !((UnityEngine.Object) this.m_frameDef != (UnityEngine.Object) null) ? -1 : this.m_frameDef.PortraitMatIdx;

  public int FrameMatIdx => !((UnityEngine.Object) this.m_frameDef != (UnityEngine.Object) null) ? -1 : this.m_frameDef.FrameMatIdx;

  public float DecorationRootOffset => !((UnityEngine.Object) this.m_frameDef != (UnityEngine.Object) null) ? 0.0f : this.m_frameDef.DecorationRootOffset;

  public float HeroZonePositionOffset => !((UnityEngine.Object) this.m_frameDef != (UnityEngine.Object) null) ? 0.0f : this.m_frameDef.HeroZonePositionOffset;

  public float RaiseAndLowerLimit => !((UnityEngine.Object) this.m_frameDef != (UnityEngine.Object) null) ? 0.0f : this.m_frameDef.HeroPickerRaiseAndLowerLimit;

  public float HeroClassIconOffset => !((UnityEngine.Object) this.m_frameDef != (UnityEngine.Object) null) ? 0.0f : this.m_frameDef.HeroClassIconOffset;

  public float HeroPowerContainerOffset => !((UnityEngine.Object) this.m_frameDef != (UnityEngine.Object) null) ? 0.0f : this.m_frameDef.HeroPowerContainerOffset;

  public CustomFrameController(GameObject frameObject)
    : this(frameObject, -1, -1)
  {
  }

  public CustomFrameController(GameObject frameObject, int matIdx, int frameMatIdx)
  {
    this.m_meshFilter = frameObject.GetComponent<MeshFilter>();
    this.m_meshRenderer = frameObject.GetComponent<MeshRenderer>();
    this.m_originalMatIdx = matIdx;
    this.m_originalFrameMatIdx = frameMatIdx;
  }

  public void SetAssetHandle(AssetReference reference, AssetHandle<GameObject> handle)
  {
    this.FrameAssetReference = reference;
    AssetHandle.Set<GameObject>(ref this.m_frameHandle, handle);
    if ((bool) this.m_frameHandle && (bool) (UnityEngine.Object) this.m_frameHandle.Asset)
      this.m_frameDef = this.m_frameHandle.Asset.GetComponent<CustomFrameDef>();
    else
      this.m_frameDef = (CustomFrameDef) null;
  }

  public void CacheHighlightState(HighlightState highlightState)
  {
    if (!((UnityEngine.Object) this.m_highlightState == (UnityEngine.Object) null))
      return;
    this.m_highlightState = highlightState;
    this.m_highlightRender = (HighlightRender) null;
    if (!((UnityEngine.Object) highlightState != (UnityEngine.Object) null))
      return;
    this.m_originalHighlightSilouetteTexture = highlightState.m_StaticSilouetteTexture;
    this.m_highlightRender = highlightState.GetComponentInChildren<HighlightRender>();
    if (!((UnityEngine.Object) this.m_highlightRender != (UnityEngine.Object) null))
      return;
    this.m_originalHighlightPosition = this.m_highlightRender.transform.localPosition;
    this.m_originalHighlightScale = this.m_highlightRender.transform.localScale;
  }

  public void CacheInitialPortraitMaterial(Material material)
  {
    if (!((UnityEngine.Object) this.m_originalPortraitMaterial == (UnityEngine.Object) null))
      return;
    this.m_originalPortraitMaterial = material;
  }

  public void RestoreInitialPortraitMaterial(ref Material material)
  {
    if (!((UnityEngine.Object) material != (UnityEngine.Object) null))
      return;
    material = this.m_originalPortraitMaterial;
  }

  public void ApplyCustomMeshAndMaterials(out GameObject frameObject)
  {
    if ((UnityEngine.Object) this.m_customFrameObject == (UnityEngine.Object) null)
      this.m_customFrameObject = new GameObject("Custom Frame", new System.Type[3]
      {
        typeof (MeshFilter),
        typeof (MeshRenderer),
        typeof (LegendarySkinDynamicResController)
      });
    List<Material> materialList = new List<Material>();
    this.m_frameDef.Mesh.GetSharedMaterials(materialList);
    Material material1 = materialList[this.m_frameDef.PortraitMatIdx];
    Material material2 = UnityEngine.Object.Instantiate<Material>(material1);
    materialList[this.m_frameDef.PortraitMatIdx] = material2;
    LegendarySkinDynamicResController component1 = this.m_customFrameObject.GetComponent<LegendarySkinDynamicResController>();
    component1.CacheMaterialProperties(material1);
    component1.Renderer = (Renderer) this.m_customFrameObject.GetComponent<MeshRenderer>();
    component1.MaterialIdx = this.m_frameDef.PortraitMatIdx;
    this.DynamicResolutionController = component1;
    this.m_customFrameObject.GetComponent<MeshFilter>().sharedMesh = this.m_frameDef.Mesh.GetComponent<MeshFilter>().sharedMesh;
    RendererExtension.SetSharedMaterials((Renderer) this.m_customFrameObject.GetComponent<MeshRenderer>(), materialList);
    this.m_customFrameObject.transform.SetParent(this.m_meshRenderer.gameObject.transform);
    this.m_customFrameObject.transform.localPosition = new Vector3(0.0f, this.m_frameDef.AvoidShadowPlaneOffset, 0.0f);
    this.m_customFrameObject.transform.localRotation = Quaternion.identity;
    this.m_customFrameObject.transform.localScale = Vector3.one;
    this.m_customFrameObject.layer = this.m_meshRenderer.gameObject.layer;
    this.m_meshRenderer.enabled = false;
    if ((UnityEngine.Object) this.m_highlightState != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) this.m_originalHighlightSilouetteTexture != (UnityEngine.Object) null)
        this.m_highlightState.m_StaticSilouetteTexture = this.m_frameDef.Silhouette;
      if ((UnityEngine.Object) this.m_highlightRender != (UnityEngine.Object) null)
      {
        HighlightRenderOverrides renderOverrides = (HighlightRenderOverrides) null;
        if ((UnityEngine.Object) this.m_highlightState.GetComponentInParent<CollectionCardVisual>() != (UnityEngine.Object) null)
        {
          renderOverrides = this.m_frameDef.CollectionOverrides;
        }
        else
        {
          switch (this.m_highlightState.m_highlightType)
          {
            case HighlightStateType.CARD:
              renderOverrides = this.m_frameDef.CardOverrides;
              break;
            case HighlightStateType.HIGHLIGHT:
              renderOverrides = this.m_frameDef.HighlightOverrides;
              break;
          }
        }
        this.m_highlightRender.SetRenderOverrides(renderOverrides);
        if ((UnityEngine.Object) renderOverrides != (UnityEngine.Object) null)
        {
          if (renderOverrides.OverrideTransform)
          {
            this.m_highlightRender.transform.localPosition = renderOverrides.Position;
            this.m_highlightRender.transform.localScale = Vector3.one * renderOverrides.Scale;
          }
          else
          {
            this.m_highlightRender.transform.localPosition = this.m_originalHighlightPosition;
            this.m_highlightRender.transform.localScale = this.m_originalHighlightScale;
          }
        }
      }
      this.m_highlightState.ForceUpdate();
    }
    PopupRenderer component2 = this.m_meshRenderer.GetComponent<PopupRenderer>();
    if ((bool) (UnityEngine.Object) component2 && (bool) (UnityEngine.Object) component2.PopupRoot)
      component2.PopupRoot.GetOrCreatePopupRenderer(this.m_customFrameObject, false, 1, true);
    frameObject = this.m_customFrameObject;
  }

  public void RestoreMeshAndMaterials(ref GameObject frameObject)
  {
    this.m_meshRenderer.enabled = true;
    if ((UnityEngine.Object) this.m_customFrameObject != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_customFrameObject);
      this.m_customFrameObject = (GameObject) null;
    }
    if ((UnityEngine.Object) this.m_highlightState != (UnityEngine.Object) null)
    {
      this.m_highlightState.m_StaticSilouetteTexture = this.m_originalHighlightSilouetteTexture;
      if ((UnityEngine.Object) this.m_highlightRender != (UnityEngine.Object) null)
      {
        this.m_highlightRender.SetRenderOverrides((HighlightRenderOverrides) null);
        this.m_highlightRender.transform.localPosition = this.m_originalHighlightPosition;
        this.m_highlightRender.transform.localScale = this.m_originalHighlightScale;
      }
      this.m_highlightState.ForceUpdate();
    }
    frameObject = this.m_meshRenderer.gameObject;
  }

  public void RestoreMeshAndMaterials(
    ref GameObject frameObject,
    ref int matIdx,
    ref int frameMatIdx)
  {
    this.RestoreMeshAndMaterials(ref frameObject);
    matIdx = this.m_originalMatIdx;
    frameMatIdx = this.m_originalFrameMatIdx;
  }

  void IDisposable.Dispose()
  {
    this.m_frameDef = (CustomFrameDef) null;
    AssetHandle.SafeDispose<GameObject>(ref this.m_frameHandle);
  }
}
