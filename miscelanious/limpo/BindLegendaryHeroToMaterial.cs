using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class BindLegendaryHeroToMaterial : MonoBehaviour
{
  [CustomEditField(Sections = "Setup", T = EditType.DEFAULT)]
  public bool ShouldBindOnStart = true;
  [CustomEditField(Sections = "Hero Prefab", T = EditType.GAME_OBJECT)]
  public string LegendaryHeroPrefab;
  [CustomEditField(Sections = "Hero Prefab")]
  public Player.Side PlayerSide;
  [CustomEditField(Sections = "Target Material")]
  public Renderer PortraitRenderer;
  [CustomEditField(Sections = "Target Material")]
  public int MaterialIndex;
  private Coroutine m_bindingCoroutine;
  private ILegendaryHeroPortrait m_legendaryHeroPortrait;
  private bool m_hasBoundMaterial;
  private string m_boundAssetPath;

  private void Start()
  {
    if (!this.ShouldBindOnStart)
      return;
    this.BindMaterial();
  }

  private void OnDestroy() => this.Cleanup();

  public void BindMaterial()
  {
    if (!string.IsNullOrEmpty(this.m_boundAssetPath) && this.LegendaryHeroPrefab != this.m_boundAssetPath)
      this.Cleanup();
    if (this.m_hasBoundMaterial || this.m_bindingCoroutine != null)
      return;
    this.m_bindingCoroutine = this.StartCoroutine(this.BindMaterialAsync());
  }

  private IEnumerator BindMaterialAsync()
  {
    BindLegendaryHeroToMaterial legendaryHeroToMaterial = this;
    if ((Object) legendaryHeroToMaterial.PortraitRenderer != (Object) null && !string.IsNullOrEmpty(legendaryHeroToMaterial.LegendaryHeroPrefab))
    {
      legendaryHeroToMaterial.m_boundAssetPath = legendaryHeroToMaterial.LegendaryHeroPrefab;
      LegendaryHeroRenderToTextureService service;
      while (!ServiceManager.TryGet<LegendaryHeroRenderToTextureService>(out service))
        yield return (object) null;
      if (service != null)
      {
        legendaryHeroToMaterial.m_legendaryHeroPortrait = service.CreatePortrait(legendaryHeroToMaterial.m_boundAssetPath, legendaryHeroToMaterial.PlayerSide);
        Texture portraitTexture = legendaryHeroToMaterial.m_legendaryHeroPortrait?.PortraitTexture;
        if ((Object) portraitTexture != (Object) null)
        {
          List<Material> materialList = new List<Material>();
          legendaryHeroToMaterial.PortraitRenderer.GetSharedMaterials(materialList);
          if (legendaryHeroToMaterial.MaterialIndex < materialList.Count)
          {
            Material original = materialList[legendaryHeroToMaterial.MaterialIndex];
            if ((bool) (Object) original)
            {
              Material material = Object.Instantiate<Material>(original);
              material.mainTexture = portraitTexture;
              materialList[legendaryHeroToMaterial.MaterialIndex] = material;
              legendaryHeroToMaterial.PortraitRenderer.SetSharedMaterials(materialList.ToArray());
              LegendarySkinDynamicResController componentInChildren = legendaryHeroToMaterial.GetComponentInChildren<LegendarySkinDynamicResController>();
              if ((bool) (Object) componentInChildren)
              {
                componentInChildren.CacheMaterialProperties(material);
                componentInChildren.Renderer = legendaryHeroToMaterial.PortraitRenderer;
                componentInChildren.MaterialIdx = legendaryHeroToMaterial.MaterialIndex;
                legendaryHeroToMaterial.m_legendaryHeroPortrait.ConnectDynamicResolutionController(componentInChildren);
              }
              legendaryHeroToMaterial.m_hasBoundMaterial = true;
              legendaryHeroToMaterial.m_bindingCoroutine = (Coroutine) null;
              yield break;
            }
          }
        }
      }
    }
    legendaryHeroToMaterial.Cleanup();
  }

  public void Cleanup()
  {
    this.m_hasBoundMaterial = false;
    this.m_boundAssetPath = string.Empty;
    if (this.m_bindingCoroutine != null)
    {
      this.StopCoroutine(this.m_bindingCoroutine);
      this.m_bindingCoroutine = (Coroutine) null;
    }
    this.m_legendaryHeroPortrait?.Dispose();
    this.m_legendaryHeroPortrait = (ILegendaryHeroPortrait) null;
  }
}
