using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class GhostCard : MonoBehaviour
{
  public Actor m_Actor;
  public Vector3 m_CardOffset = Vector3.zero;
  public RenderToTexture m_R2T_EffectGhost;
  public GameObject m_EffectRoot;
  public GameObject m_GlowPlane;
  public GameObject m_GlowPlaneElite;
  private static GhostStyleDef s_ghostStyles;
  private static IMaterialService s_materialService;
  private static ProfilerMarker s_RenderGhost = new ProfilerMarker("RenderGhost");
  private static ProfilerMarker s_RenderGhostInit = new ProfilerMarker("RenderGhost_Init");
  private static ProfilerMarker s_RenderGhostStoreOrgMaterials = new ProfilerMarker("RenderGhost_StoreOrgMaterials");
  private static ProfilerMarker s_RenderGhostRestoreOrgMaterials = new ProfilerMarker("RenderGhost_RestoreOrgMaterials");
  private static ProfilerMarker s_RenderGhostApplyGhostMaterials = new ProfilerMarker("RenderGhost_ApplyGhostMaterials");
  private static ProfilerMarker s_RenderGhostSetupRTTOverrides = new ProfilerMarker("RenderGhost_SetupRTTOverrides");
  private bool m_isBigCard;
  private bool m_Init;
  private RenderToTexture m_R2T_BaseCard;
  private bool m_R2T_BaseCard_OrigHideRenderObject;
  private GhostCard.Type m_ghostType;
  private TAG_PREMIUM m_ghostPremium;
  public int m_renderQueue;
  private GameObject m_CardMesh;
  private int m_CardFrontIdx;
  private int m_PremiumRibbonIdx = -1;
  private GameObject m_PortraitMesh;
  private int m_PortraitFrameIdx;
  private GameObject m_NameMesh;
  private GameObject m_DescriptionMesh;
  private GameObject m_DescriptionTrimMesh;
  private GameObject m_RarityFrameMesh;
  private GameObject m_ManaCostMesh;
  private GameObject m_AttackMesh;
  private GameObject m_HealthMesh;
  private GameObject m_RacePlateMesh;
  private GameObject m_MultiRacePlateMesh;
  private GameObject m_EliteMesh;
  private GameObject m_mercenaryLevelMesh;
  private bool m_hasOriginalMaterialsStored;
  private Material m_OrgMat_CardFront;
  private Material m_OrgMat_PremiumRibbon;
  private Material m_OrgMat_PortraitFrame;
  private Material m_OrgMat_Name;
  private Material m_OrgMat_Description;
  private Material m_OrgMat_Description2;
  private Material m_OrgMat_DescriptionTrim;
  private Material m_OrgMat_RarityFrame;
  private Material m_OrgMat_ManaCost;
  private Material m_OrgMat_Attack;
  private Material m_OrgMat_Health;
  private Material m_OrgMat_RacePlate;
  private Material m_OrgMat_MultiRacePlate;
  private Material m_OrgMat_Elite;
  private Material m_OrgMat_mercenaryLevel;
  private RenderCommandLists.MatOverrideDictionary m_rttMatOverrides;

  public static GhostCard.Type GetGhostTypeFromSlot(
    CollectionDeck deck,
    CollectionDeckSlot slot)
  {
    if (deck == null || slot == null)
      return GhostCard.Type.NONE;
    switch (deck.GetSlotStatus(slot))
    {
      case CollectionDeck.SlotStatus.NOT_VALID:
        return GhostCard.Type.NOT_VALID;
      case CollectionDeck.SlotStatus.MISSING:
        return GhostCard.Type.MISSING;
      default:
        return GhostCard.Type.NONE;
    }
  }

  private void Awake()
  {
    this.m_R2T_BaseCard = this.GetComponent<RenderToTexture>();
    this.m_R2T_BaseCard_OrigHideRenderObject = this.m_R2T_BaseCard.m_HideRenderObject;
    if (!((Object) GhostCard.s_ghostStyles == (Object) null) || AssetLoader.Get() == null)
      return;
    GhostCard.s_ghostStyles = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit("GhostStyleDef.prefab:932fbc50238e04673aeb0f59c9cfaed1"), (AssetLoadingOptions) 0).GetComponent<GhostStyleDef>();
  }

  private void OnDisable() => this.Disable();

  private void OnDestroy()
  {
    this.DropMaterialReferences();
    this.DestroyRTTOverrides();
    if (!(bool) (Object) this.m_EffectRoot)
      return;
    ParticleSystem componentInChildren = this.m_EffectRoot.GetComponentInChildren<ParticleSystem>();
    if (!(bool) (Object) componentInChildren)
      return;
    componentInChildren.Stop();
  }

  public void SetBigCard(bool isBigCard) => this.m_isBigCard = isBigCard;

  public void SetGhostType(GhostCard.Type ghostType)
  {
    if (this.m_ghostType != ghostType && (ghostType == GhostCard.Type.DORMANT || this.m_ghostType == GhostCard.Type.DORMANT))
      this.Reset();
    this.m_ghostType = ghostType;
  }

  public void SetPremium(TAG_PREMIUM premium) => this.m_ghostPremium = premium;

  public void SetRenderQueue(int renderQueue) => this.m_renderQueue = renderQueue;

  public void SetRTTDirty()
  {
    if (!((Object) this.m_R2T_BaseCard != (Object) null))
      return;
    this.m_R2T_BaseCard.SetDirty();
  }

  public void RenderGhostCard() => this.RenderGhostCard(false);

  public void RenderGhostCard(bool forceRender) => this.RenderGhost(forceRender);

  public void Reset() => this.m_Init = false;

  private void RenderGhost() => this.RenderGhost(false);

  private void RenderGhost(bool forceRender)
  {
    bool usingRenderToTexture = this.m_ghostType != GhostCard.Type.DORMANT;
    this.Init(forceRender, usingRenderToTexture);
    if (usingRenderToTexture)
    {
      this.m_R2T_BaseCard.enabled = true;
      this.m_R2T_BaseCard.m_HideRenderObject = this.m_R2T_BaseCard_OrigHideRenderObject;
    }
    else
    {
      this.m_R2T_BaseCard.enabled = false;
      this.m_R2T_BaseCard.m_HideRenderObject = false;
    }
    this.m_R2T_BaseCard.m_RenderQueue = this.m_renderQueue;
    if ((bool) (Object) this.m_R2T_EffectGhost)
    {
      this.m_R2T_EffectGhost.enabled = true;
      this.m_R2T_EffectGhost.m_RenderQueue = this.m_renderQueue;
    }
    this.m_Actor.m_ghostCardActive = true;
    this.m_R2T_BaseCard.m_ObjectToRender = this.m_Actor.GetRootObject();
    this.m_Actor.GetRootObject().transform.localPosition = this.m_CardOffset;
    this.m_Actor.ShowAllText();
    if (usingRenderToTexture)
    {
      this.SetupRTTOverrides();
      this.m_R2T_BaseCard.SetMaterialOverrides(this.m_rttMatOverrides);
      this.m_R2T_BaseCard.RenderNow();
    }
    else
      this.ApplyGhostMaterials();
    Renderer renderer1 = (Renderer) null;
    if ((bool) (Object) this.m_GlowPlane)
    {
      renderer1 = this.m_GlowPlane.GetComponent<Renderer>();
      renderer1.enabled = false;
    }
    Renderer renderer2 = (Renderer) null;
    if ((bool) (Object) this.m_GlowPlaneElite)
    {
      renderer2 = this.m_GlowPlaneElite.GetComponent<Renderer>();
      renderer2.enabled = false;
    }
    if ((bool) (Object) renderer1 && !this.m_Actor.IsElite())
    {
      renderer1.enabled = true;
      renderer1.GetMaterial().renderQueue = 3000 + this.GetGlowPlaneRenderOrderAdjustment();
      renderer1.sortingOrder = this.GetGlowPlaneRenderOrderAdjustment();
    }
    if ((bool) (Object) renderer2 && this.m_Actor.IsElite())
    {
      renderer2.enabled = true;
      renderer2.GetMaterial().renderQueue = 3000 + this.GetGlowPlaneRenderOrderAdjustment();
      renderer2.sortingOrder = this.GetGlowPlaneRenderOrderAdjustment();
    }
    if (!(bool) (Object) this.m_EffectRoot)
      return;
    this.m_EffectRoot.transform.parent = (Transform) null;
    this.m_EffectRoot.transform.position = new Vector3(-500f, -500f, -500f);
    this.m_EffectRoot.transform.localScale = Vector3.one;
    if ((bool) (Object) this.m_R2T_EffectGhost)
    {
      this.m_R2T_EffectGhost.enabled = true;
      RenderTexture renderTexture = this.m_R2T_EffectGhost.RenderNow();
      if ((Object) renderTexture != (Object) null)
        this.m_R2T_BaseCard.GetRenderMaterial().SetTexture("_FxTex", (Texture) renderTexture);
    }
    ParticleSystem componentInChildren = this.m_EffectRoot.GetComponentInChildren<ParticleSystem>();
    if (!(bool) (Object) componentInChildren)
      return;
    Renderer component = componentInChildren.GetComponent<Renderer>();
    if ((bool) (Object) component)
      component.enabled = true;
    componentInChildren.Play();
  }

  private int GetGlowPlaneRenderOrderAdjustment() => this.m_ghostType == GhostCard.Type.DORMANT ? 51 : this.m_renderQueue + 1;

  public void ShowRenderers()
  {
    foreach (MeshRenderer componentsInChild in this.gameObject.GetComponentsInChildren<MeshRenderer>())
    {
      bool flag = true;
      if ((Object) this.m_GlowPlane != (Object) this.m_GlowPlaneElite)
      {
        if (this.m_Actor.IsElite() && (Object) componentsInChild.gameObject == (Object) this.m_GlowPlane)
          flag = false;
        else if (!this.m_Actor.IsElite() && (Object) componentsInChild.gameObject == (Object) this.m_GlowPlaneElite)
          flag = false;
      }
      componentsInChild.enabled = flag;
    }
  }

  public void DisableGhost()
  {
    this.Disable();
    this.enabled = false;
  }

  private void Init(bool forceRender, bool usingRenderToTexture)
  {
    if (this.m_Init && !forceRender)
      return;
    if ((Object) this.m_Actor == (Object) null)
    {
      this.m_Actor = GameObjectUtils.FindComponentInThisOrParents<Actor>(this.gameObject);
      if ((Object) this.m_Actor == (Object) null)
      {
        Debug.LogError((object) string.Format("{0} Ghost card effect failed to find Actor!", (object) this.transform.root.name));
        this.enabled = false;
        return;
      }
    }
    this.m_CardMesh = this.m_Actor.m_cardMesh;
    this.m_CardFrontIdx = this.m_Actor.m_cardFrontMatIdx;
    this.m_PremiumRibbonIdx = this.m_Actor.m_premiumRibbon;
    this.m_PortraitMesh = this.m_Actor.m_portraitMesh;
    this.m_PortraitFrameIdx = this.m_Actor.m_portraitFrameMatIdx;
    this.m_NameMesh = this.m_Actor.m_nameBannerMesh;
    this.m_DescriptionMesh = this.m_Actor.m_descriptionMesh;
    this.m_DescriptionTrimMesh = this.m_Actor.m_descriptionTrimMesh;
    this.m_RarityFrameMesh = this.m_Actor.m_rarityFrameMesh;
    if ((bool) (Object) this.m_Actor.m_attackObject)
    {
      Renderer component = this.m_Actor.m_attackObject.GetComponent<Renderer>();
      if ((Object) component != (Object) null)
        this.m_AttackMesh = component.gameObject;
      if ((Object) this.m_AttackMesh == (Object) null)
      {
        foreach (Renderer componentsInChild in this.m_Actor.m_attackObject.GetComponentsInChildren<Renderer>())
        {
          if (!(bool) (Object) componentsInChild.GetComponent<UberText>())
            this.m_AttackMesh = componentsInChild.gameObject;
        }
      }
    }
    if ((bool) (Object) this.m_Actor.m_healthObject)
    {
      Renderer component = this.m_Actor.m_healthObject.GetComponent<Renderer>();
      if ((Object) component != (Object) null)
        this.m_HealthMesh = component.gameObject;
      if ((Object) this.m_HealthMesh == (Object) null)
      {
        foreach (Renderer componentsInChild in this.m_Actor.m_healthObject.GetComponentsInChildren<Renderer>())
        {
          if (!(bool) (Object) componentsInChild.GetComponent<UberText>())
            this.m_HealthMesh = componentsInChild.gameObject;
        }
      }
    }
    this.m_ManaCostMesh = this.m_Actor.m_manaObject;
    this.m_MultiRacePlateMesh = this.m_Actor.m_multiRacePlateObject;
    this.m_RacePlateMesh = this.m_Actor.m_racePlateObject;
    this.m_EliteMesh = this.m_Actor.m_eliteObject;
    this.m_mercenaryLevelMesh = this.m_Actor.m_mercenaryLevelObject?.m_xpBarBacking;
    if (!usingRenderToTexture)
      this.StoreOrgMaterials();
    this.m_R2T_BaseCard.m_ObjectToRender = this.m_Actor.GetRootObject();
    if ((bool) (Object) this.m_R2T_BaseCard.m_Material && this.m_R2T_BaseCard.m_Material.HasProperty("_Seed"))
      this.m_R2T_BaseCard.m_Material.SetFloat("_Seed", Random.Range(0.0f, 1f));
    if (this.m_Actor.UsesMultiClassBanner())
      this.m_Actor.GetMultiClassBanner().TurnOffShadowsAndFX();
    this.m_Init = true;
  }

  private void StoreOrgMaterials()
  {
    if (this.m_hasOriginalMaterialsStored)
      return;
    this.m_hasOriginalMaterialsStored = true;
    IMaterialService materialService = GhostCard.GetMaterialService();
    if ((bool) (Object) this.m_CardMesh)
    {
      if (this.m_CardFrontIdx > -1)
      {
        this.m_OrgMat_CardFront = this.m_CardMesh.GetComponent<Renderer>().GetMaterial(this.m_CardFrontIdx);
        materialService?.KeepMaterial(this.m_OrgMat_CardFront);
      }
      if (this.m_PremiumRibbonIdx > -1)
      {
        this.m_OrgMat_PremiumRibbon = this.m_CardMesh.GetComponent<Renderer>().GetMaterial(this.m_PremiumRibbonIdx);
        materialService?.KeepMaterial(this.m_OrgMat_PremiumRibbon);
      }
    }
    if ((bool) (Object) this.m_PortraitMesh && this.m_PortraitFrameIdx > -1)
    {
      this.m_OrgMat_PortraitFrame = this.m_PortraitMesh.GetComponent<Renderer>().GetMaterial(this.m_PortraitFrameIdx);
      materialService?.KeepMaterial(this.m_OrgMat_PortraitFrame);
    }
    if ((bool) (Object) this.m_NameMesh)
    {
      this.m_OrgMat_Name = this.m_NameMesh.GetComponent<Renderer>().GetMaterial();
      materialService?.KeepMaterial(this.m_OrgMat_Name);
    }
    if ((bool) (Object) this.m_ManaCostMesh)
    {
      this.m_OrgMat_ManaCost = this.m_ManaCostMesh.GetComponent<Renderer>().GetMaterial();
      materialService?.KeepMaterial(this.m_OrgMat_ManaCost);
    }
    if ((bool) (Object) this.m_AttackMesh)
    {
      this.m_OrgMat_Attack = this.m_AttackMesh.GetComponent<Renderer>().GetMaterial();
      materialService?.KeepMaterial(this.m_OrgMat_Attack);
    }
    if ((bool) (Object) this.m_HealthMesh)
    {
      this.m_OrgMat_Health = this.m_HealthMesh.GetComponent<Renderer>().GetMaterial();
      materialService?.KeepMaterial(this.m_OrgMat_Health);
    }
    if ((bool) (Object) this.m_RacePlateMesh)
    {
      this.m_OrgMat_RacePlate = this.m_RacePlateMesh.GetComponent<Renderer>().GetMaterial();
      materialService?.KeepMaterial(this.m_OrgMat_RacePlate);
    }
    if ((bool) (Object) this.m_MultiRacePlateMesh)
    {
      this.m_OrgMat_MultiRacePlate = this.m_MultiRacePlateMesh.GetComponent<Renderer>().GetMaterial();
      materialService?.KeepMaterial(this.m_OrgMat_MultiRacePlate);
    }
    if ((bool) (Object) this.m_mercenaryLevelMesh)
    {
      this.m_OrgMat_mercenaryLevel = this.m_RacePlateMesh.GetComponent<Renderer>().GetMaterial();
      materialService?.KeepMaterial(this.m_OrgMat_mercenaryLevel);
    }
    if ((bool) (Object) this.m_RarityFrameMesh)
    {
      Renderer component = this.m_RarityFrameMesh.GetComponent<Renderer>();
      if ((Object) component != (Object) null)
      {
        this.m_OrgMat_RarityFrame = component.GetMaterial();
        materialService?.KeepMaterial(this.m_OrgMat_RarityFrame);
      }
    }
    if ((bool) (Object) this.m_DescriptionMesh && (Object) this.m_DescriptionMesh.GetComponent<Renderer>() != (Object) null)
    {
      List<Material> materials = this.m_DescriptionMesh.GetComponent<Renderer>().GetMaterials();
      if (materials.Count > 0)
      {
        this.m_OrgMat_Description = materials[0];
        materialService?.KeepMaterial(this.m_OrgMat_Description);
        if (materials.Count > 1)
        {
          this.m_OrgMat_Description2 = materials[1];
          materialService?.KeepMaterial(this.m_OrgMat_Description2);
        }
      }
    }
    if ((bool) (Object) this.m_DescriptionTrimMesh)
    {
      this.m_OrgMat_DescriptionTrim = this.m_DescriptionTrimMesh.GetComponent<Renderer>().GetMaterial();
      materialService?.KeepMaterial(this.m_OrgMat_DescriptionTrim);
    }
    if (!(bool) (Object) this.m_EliteMesh)
      return;
    this.m_OrgMat_Elite = this.m_EliteMesh.GetComponent<Renderer>().GetMaterial();
    materialService?.KeepMaterial(this.m_OrgMat_Elite);
  }

  private void RestoreOrgMaterials()
  {
    if (!this.m_hasOriginalMaterialsStored)
      return;
    this.ApplyMaterialByIdx(this.m_CardMesh, this.m_OrgMat_CardFront, this.m_CardFrontIdx);
    this.ApplyMaterialByIdx(this.m_CardMesh, this.m_OrgMat_PremiumRibbon, this.m_PremiumRibbonIdx);
    this.ApplyMaterialByIdx(this.m_PortraitMesh, this.m_OrgMat_PortraitFrame, this.m_PortraitFrameIdx);
    this.ApplyMaterialByIdx(this.m_DescriptionMesh, this.m_OrgMat_Description, 0);
    this.ApplyMaterialByIdx(this.m_DescriptionMesh, this.m_OrgMat_Description2, 1);
    this.ApplyMaterial(this.m_NameMesh, this.m_OrgMat_Name);
    this.ApplyMaterial(this.m_ManaCostMesh, this.m_OrgMat_ManaCost);
    this.ApplyMaterial(this.m_AttackMesh, this.m_OrgMat_Attack);
    this.ApplyMaterial(this.m_HealthMesh, this.m_OrgMat_Health);
    this.ApplyMaterial(this.m_RacePlateMesh, this.m_OrgMat_RacePlate);
    this.ApplyMaterial(this.m_MultiRacePlateMesh, this.m_OrgMat_MultiRacePlate);
    this.ApplyMaterial(this.m_RarityFrameMesh, this.m_OrgMat_RarityFrame);
    this.ApplyMaterial(this.m_DescriptionTrimMesh, this.m_OrgMat_DescriptionTrim);
    this.ApplyMaterial(this.m_EliteMesh, this.m_OrgMat_Elite);
    this.ApplyMaterial(this.m_mercenaryLevelMesh, this.m_OrgMat_mercenaryLevel);
  }

  private void DropMaterialReferences()
  {
    if (!this.m_hasOriginalMaterialsStored)
      return;
    IMaterialService materialService = GhostCard.GetMaterialService();
    materialService?.DropMaterial(this.m_OrgMat_CardFront);
    materialService?.DropMaterial(this.m_OrgMat_PremiumRibbon);
    materialService?.DropMaterial(this.m_OrgMat_PortraitFrame);
    materialService?.DropMaterial(this.m_OrgMat_Description);
    materialService?.DropMaterial(this.m_OrgMat_Description2);
    materialService?.DropMaterial(this.m_OrgMat_Name);
    materialService?.DropMaterial(this.m_OrgMat_ManaCost);
    materialService?.DropMaterial(this.m_OrgMat_Attack);
    materialService?.DropMaterial(this.m_OrgMat_Health);
    materialService?.DropMaterial(this.m_OrgMat_RacePlate);
    materialService?.DropMaterial(this.m_OrgMat_MultiRacePlate);
    materialService?.DropMaterial(this.m_OrgMat_RarityFrame);
    materialService?.DropMaterial(this.m_OrgMat_DescriptionTrim);
    materialService?.DropMaterial(this.m_OrgMat_Elite);
    materialService?.DropMaterial(this.m_OrgMat_mercenaryLevel);
  }

  private GhostStyle GetGhostStyle()
  {
    switch (this.m_ghostType)
    {
      case GhostCard.Type.NOT_VALID:
        if (this.m_ghostPremium == TAG_PREMIUM.DIAMOND)
          return GhostCard.s_ghostStyles.m_invalidDiamond;
        return this.m_ghostPremium == TAG_PREMIUM.SIGNATURE ? GhostCard.s_ghostStyles.m_invalidSignature : GhostCard.s_ghostStyles.m_invalid;
      case GhostCard.Type.DORMANT:
        if (this.m_ghostPremium == TAG_PREMIUM.DIAMOND)
          return GhostCard.s_ghostStyles.m_dormantDiamond;
        return this.m_ghostPremium == TAG_PREMIUM.SIGNATURE ? GhostCard.s_ghostStyles.m_dormantSignature : GhostCard.s_ghostStyles.m_dormant;
      case GhostCard.Type.PURCHASABLE_HERO_SKIN:
        return GhostCard.s_ghostStyles.m_purchasableHeroSkin;
      default:
        if (this.m_ghostPremium == TAG_PREMIUM.DIAMOND)
          return GhostCard.s_ghostStyles.m_missingDiamond;
        return this.m_ghostPremium == TAG_PREMIUM.SIGNATURE ? GhostCard.s_ghostStyles.m_missingSignature : GhostCard.s_ghostStyles.m_missing;
    }
  }

  private void SetupGhostPlane(GhostStyle ghostStyle)
  {
    if ((bool) (Object) this.m_GlowPlane)
    {
      if ((Object) this.m_AttackMesh != (Object) null)
        this.m_GlowPlane.GetComponent<Renderer>().SetMaterial(ghostStyle.m_GhostMaterialGlowPlane);
      else
        this.m_GlowPlane.GetComponent<Renderer>().SetMaterial(ghostStyle.m_GhostMaterialAbilityGlowPlane);
    }
    if (!(bool) (Object) this.m_GlowPlaneElite)
      return;
    if ((Object) this.m_AttackMesh != (Object) null)
      this.m_GlowPlaneElite.GetComponent<Renderer>().SetMaterial(ghostStyle.m_GhostMaterialGlowPlane);
    else
      this.m_GlowPlaneElite.GetComponent<Renderer>().SetMaterial(ghostStyle.m_GhostMaterialAbilityGlowPlane);
  }

  private void ApplyGhostMaterials()
  {
    GhostStyle ghostStyle = this.GetGhostStyle();
    this.SetupGhostPlane(ghostStyle);
    this.ApplyMaterialByIdx(this.m_CardMesh, ghostStyle.m_GhostMaterial, this.m_CardFrontIdx);
    this.ApplyMaterialByIdx(this.m_CardMesh, ghostStyle.m_GhostMaterial, this.m_PremiumRibbonIdx);
    this.ApplyMaterialByIdx(this.m_PortraitMesh, ghostStyle.m_GhostMaterial, this.m_PortraitFrameIdx);
    this.ApplyMaterialByIdx(this.m_DescriptionMesh, ghostStyle.m_GhostMaterialMod2x, 0);
    this.ApplyMaterialByIdx(this.m_DescriptionMesh, ghostStyle.m_GhostMaterial, 1);
    this.ApplyMaterial(this.m_NameMesh, ghostStyle.m_GhostMaterial);
    this.ApplyMaterial(this.m_ManaCostMesh, ghostStyle.m_GhostMaterial);
    this.ApplyMaterial(this.m_AttackMesh, ghostStyle.m_GhostMaterial);
    this.ApplyMaterial(this.m_HealthMesh, ghostStyle.m_GhostMaterial);
    this.ApplyMaterial(this.m_RacePlateMesh, ghostStyle.m_GhostMaterial);
    this.ApplyMaterial(this.m_MultiRacePlateMesh, ghostStyle.m_GhostMaterial);
    this.ApplyMaterial(this.m_RarityFrameMesh, ghostStyle.m_GhostMaterial);
    this.ApplyMaterial(this.m_DescriptionTrimMesh, ghostStyle.m_GhostMaterialTransparent);
    this.ApplyMaterial(this.m_EliteMesh, ghostStyle.m_GhostMaterial);
    this.ApplyMaterial(this.m_mercenaryLevelMesh, ghostStyle.m_GhostMaterial);
    RenderUtils.SetRenderQueue(this.gameObject, this.m_R2T_BaseCard.m_RenderQueueOffset + this.m_renderQueue, true);
  }

  private void ApplyMaterial(GameObject go, Material mat)
  {
    if ((Object) go == (Object) null || (Object) mat == (Object) null)
      return;
    Renderer component = go.GetComponent<Renderer>();
    Texture mainTexture = component.GetMaterial().mainTexture;
    Vector2 mainTextureOffset = component.GetMaterial().mainTextureOffset;
    Vector2 mainTextureScale = component.GetMaterial().mainTextureScale;
    component.SetMaterial(mat);
    component.GetMaterial().mainTexture = mainTexture;
    component.GetMaterial().mainTextureOffset = mainTextureOffset;
    component.GetMaterial().mainTextureScale = mainTextureScale;
  }

  private void ApplyMaterialByIdx(GameObject go, Material mat, int idx)
  {
    if ((Object) go == (Object) null || (Object) mat == (Object) null || idx < 0)
      return;
    Renderer component = go.GetComponent<Renderer>();
    if (!(bool) (Object) component)
      return;
    List<Material> materials = component.GetMaterials();
    if (idx >= materials.Count)
      return;
    Texture mainTexture = materials[idx].mainTexture;
    Vector2 mainTextureOffset = materials[idx].mainTextureOffset;
    Vector2 mainTextureScale = materials[idx].mainTextureScale;
    Texture texture = (Texture) null;
    Material material1 = materials[idx];
    if ((Object) material1 == (Object) null)
      return;
    if (material1.HasProperty("_SecondTex"))
      texture = material1.GetTexture("_SecondTex");
    Color color = Color.clear;
    int num = material1.HasProperty("_SecondTint") ? 1 : 0;
    if (num != 0)
      color = material1.GetColor("_SecondTint");
    materials[idx] = mat;
    component.SetMaterials(materials);
    Material material2 = component.GetMaterial(idx);
    material2.mainTexture = mainTexture;
    material2.mainTextureOffset = mainTextureOffset;
    material2.mainTextureScale = mainTextureScale;
    if ((Object) texture != (Object) null)
      material2.SetTexture("_SecondTex", texture);
    if (num == 0)
      return;
    material2.SetColor("_SecondTint", color);
  }

  private void AddRTTMaterialOverride(
    RenderCommandLists.MatOverrideDictionary dict,
    GameObject go,
    Material mat)
  {
    if ((Object) go == (Object) null || (Object) mat == (Object) null)
      return;
    Renderer component = go.GetComponent<Renderer>();
    if (!(bool) (Object) component)
      return;
    Material material = !GhostCard.GetMaterialService().HasCustomMaterial(component) ? component.GetSharedMaterial() : component.GetMaterial();
    Texture mainTexture = material.mainTexture;
    Vector2 mainTextureOffset = material.mainTextureOffset;
    Vector2 mainTextureScale = material.mainTextureScale;
    dict.Add(component, new RenderCommandLists.MaterialOveride(new Material(mat)
    {
      mainTexture = mainTexture,
      mainTextureOffset = mainTextureOffset,
      mainTextureScale = mainTextureScale
    }));
  }

  private void AddRTTMaterialOverideByIdx(
    RenderCommandLists.MatOverrideDictionary dict,
    GameObject go,
    Material mat,
    int idx)
  {
    if ((Object) go == (Object) null || (Object) mat == (Object) null || idx < 0)
      return;
    Renderer component = go.GetComponent<Renderer>();
    if (!(bool) (Object) component)
      return;
    List<Material> materialList = !GhostCard.GetMaterialService().HasCustomMaterial(component) ? component.GetSharedMaterials() : component.GetMaterials();
    if (idx >= materialList.Count)
      return;
    Texture mainTexture = materialList[idx].mainTexture;
    Vector2 mainTextureOffset = materialList[idx].mainTextureOffset;
    Vector2 mainTextureScale = materialList[idx].mainTextureScale;
    Texture texture = (Texture) null;
    Material material = materialList[idx];
    if ((Object) material == (Object) null)
      return;
    if (material.HasProperty("_SecondTex"))
      texture = material.GetTexture("_SecondTex");
    Color color = Color.clear;
    int num = material.HasProperty("_SecondTint") ? 1 : 0;
    if (num != 0)
      color = material.GetColor("_SecondTint");
    Material toUse = new Material(mat);
    toUse.mainTexture = mainTexture;
    toUse.mainTextureOffset = mainTextureOffset;
    toUse.mainTextureScale = mainTextureScale;
    if ((Object) texture != (Object) null)
      toUse.SetTexture("_SecondTex", texture);
    if (num != 0)
      toUse.SetColor("_SecondTint", color);
    dict.Add(component, new RenderCommandLists.MaterialOveride(toUse, idx));
  }

  private void SetupRTTOverrides()
  {
    Material material = (Material) null;
    GhostStyle ghostStyle = this.GetGhostStyle();
    this.SetupGhostPlane(ghostStyle);
    if ((Object) ghostStyle.m_GhostCardMaterial != (Object) null && !this.m_isBigCard)
      material = Object.Instantiate<Material>(ghostStyle.m_GhostCardMaterial);
    else if ((Object) ghostStyle.m_GhostBigCardMaterial != (Object) null && this.m_isBigCard)
      material = Object.Instantiate<Material>(ghostStyle.m_GhostBigCardMaterial);
    this.m_R2T_BaseCard.m_Material = material;
    if ((bool) (Object) this.m_R2T_EffectGhost)
      this.m_R2T_EffectGhost.m_Material = material;
    if (this.m_rttMatOverrides != null)
      this.DestroyRTTOverrides();
    this.m_rttMatOverrides = new RenderCommandLists.MatOverrideDictionary();
    this.AddRTTMaterialOverideByIdx(this.m_rttMatOverrides, this.m_CardMesh, ghostStyle.m_GhostMaterial, this.m_CardFrontIdx);
    this.AddRTTMaterialOverideByIdx(this.m_rttMatOverrides, this.m_CardMesh, ghostStyle.m_GhostMaterial, this.m_PremiumRibbonIdx);
    this.AddRTTMaterialOverideByIdx(this.m_rttMatOverrides, this.m_PortraitMesh, ghostStyle.m_GhostMaterial, this.m_PortraitFrameIdx);
    this.AddRTTMaterialOverideByIdx(this.m_rttMatOverrides, this.m_DescriptionMesh, ghostStyle.m_GhostMaterialMod2x, 0);
    this.AddRTTMaterialOverideByIdx(this.m_rttMatOverrides, this.m_DescriptionMesh, ghostStyle.m_GhostMaterial, 1);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_NameMesh, ghostStyle.m_GhostMaterial);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_ManaCostMesh, ghostStyle.m_GhostMaterial);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_AttackMesh, ghostStyle.m_GhostMaterial);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_HealthMesh, ghostStyle.m_GhostMaterial);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_RacePlateMesh, ghostStyle.m_GhostMaterial);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_MultiRacePlateMesh, ghostStyle.m_GhostMaterial);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_RarityFrameMesh, ghostStyle.m_GhostMaterial);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_DescriptionTrimMesh, ghostStyle.m_GhostMaterialTransparent);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_EliteMesh, ghostStyle.m_GhostMaterial);
    this.AddRTTMaterialOverride(this.m_rttMatOverrides, this.m_mercenaryLevelMesh, ghostStyle.m_GhostMaterial);
    RenderUtils.SetRenderQueue(this.gameObject, this.m_R2T_BaseCard.m_RenderQueueOffset + this.m_renderQueue, true);
  }

  private void DestroyRTTOverrides()
  {
    if (this.m_rttMatOverrides != null)
    {
      foreach (KeyValuePair<Renderer, List<RenderCommandLists.MaterialOveride>> rttMatOverride in (Dictionary<Renderer, List<RenderCommandLists.MaterialOveride>>) this.m_rttMatOverrides)
      {
        foreach (RenderCommandLists.MaterialOveride materialOveride in rttMatOverride.Value)
          Object.Destroy((Object) materialOveride.materialToUse);
      }
    }
    this.m_rttMatOverrides = (RenderCommandLists.MatOverrideDictionary) null;
  }

  private void Disable()
  {
    this.RestoreOrgMaterials();
    if ((bool) (Object) this.m_R2T_BaseCard)
      this.m_R2T_BaseCard.enabled = false;
    if ((bool) (Object) this.m_R2T_EffectGhost)
      this.m_R2T_EffectGhost.enabled = false;
    if ((bool) (Object) this.m_GlowPlane)
      this.m_GlowPlane.GetComponent<Renderer>().enabled = false;
    if ((bool) (Object) this.m_GlowPlaneElite)
      this.m_GlowPlaneElite.GetComponent<Renderer>().enabled = false;
    if ((bool) (Object) this.m_EffectRoot)
    {
      ParticleSystem componentInChildren = this.m_EffectRoot.GetComponentInChildren<ParticleSystem>();
      if ((bool) (Object) componentInChildren)
      {
        componentInChildren.Stop();
        componentInChildren.GetComponent<Renderer>().enabled = false;
      }
    }
    if (!((Object) this.m_Actor != (Object) null))
      return;
    this.m_Actor.m_ghostCardActive = false;
  }

  private static IMaterialService GetMaterialService()
  {
    if (GhostCard.s_materialService == null)
      GhostCard.s_materialService = ServiceManager.Get<IMaterialService>();
    return GhostCard.s_materialService;
  }

  public enum Type
  {
    NONE,
    MISSING_UNCRAFTABLE,
    MISSING,
    NOT_VALID,
    DORMANT,
    PURCHASABLE_HERO_SKIN,
  }
}
