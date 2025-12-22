using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructCard : MonoBehaviour
{
  private readonly Vector3 IMPACT_CAMERA_SHAKE_AMOUNT = new Vector3(0.35f, 0.35f, 0.35f);
  private readonly float IMPACT_CAMERA_SHAKE_TIME = 0.25f;
  public Material m_GhostMaterial;
  public Material m_GhostMaterialTransparent;
  public float m_ImpactRotationTime = 0.5f;
  public float m_RandomDelayVariance = 0.2f;
  public float m_AnimationRarityScaleCommon = 1f;
  public float m_AnimationRarityScaleRare = 0.9f;
  public float m_AnimationRarityScaleEpic = 0.8f;
  public float m_AnimationRarityScaleLegendary = 0.7f;
  public GameObject m_GhostGlow;
  public Texture m_GhostTextureUnique;
  public GameObject m_FuseGlow;
  public ParticleSystem m_RarityBurstCommon;
  public ParticleSystem m_RarityBurstRare;
  public ParticleSystem m_RarityBurstEpic;
  public ParticleSystem m_RarityBurstLegendary;
  public Transform m_ManaGemStartPosition;
  public Transform m_ManaGemTargetPosition;
  public float m_ManaGemStartDelay;
  public float m_ManaGemAnimTime = 1f;
  public GameObject m_ManaGemGlow;
  public ParticleSystem m_ManaGemHitBlastParticle;
  public Vector3 m_ManaGemImpactRotation = new Vector3(20f, 0.0f, 20f);
  public Transform m_DescriptionStartPosition;
  public Transform m_DescriptionTargetPosition;
  public float m_DescriptionStartDelay;
  public float m_DescriptionAnimTime = 1f;
  public GameObject m_DescriptionGlow;
  public ParticleSystem m_DescriptionHitBlastParticle;
  public Vector3 m_DescriptionImpactRotation = new Vector3(-15f, 0.0f, 0.0f);
  public Transform m_AttackStartPosition;
  public Transform m_AttackTargetPosition;
  public float m_AttackStartDelay;
  public float m_AttackAnimTime = 1f;
  public GameObject m_AttackGlow;
  public ParticleSystem m_AttackHitBlastParticle;
  public Vector3 m_AttackImpactRotation = new Vector3(-15f, 0.0f, 0.0f);
  public Transform m_HealthStartPosition;
  public Transform m_HealthTargetPosition;
  public float m_HealthStartDelay;
  public float m_HealthAnimTime = 1f;
  public GameObject m_HealthGlow;
  public ParticleSystem m_HealthHitBlastParticle;
  public Vector3 m_HealthImpactRotation = new Vector3(-15f, 0.0f, 0.0f);
  public Transform m_ArmorStartPosition;
  public Transform m_ArmorTargetPosition;
  public float m_ArmorStartDelay;
  public float m_ArmorAnimTime = 1f;
  public GameObject m_ArmorGlow;
  public ParticleSystem m_ArmorHitBlastParticle;
  public Vector3 m_ArmorImpactRotation = new Vector3(-15f, 0.0f, 0.0f);
  public Transform m_PortraitStartPosition;
  public Transform m_PortraitTargetPosition;
  public float m_PortraitStartDelay;
  public float m_PortraitAnimTime = 1f;
  public GameObject m_PortraitGlow;
  public GameObject m_PortraitGlowStandard;
  public GameObject m_PortraitGlowUnique;
  public ParticleSystem m_PortraitHitBlastParticle;
  public Vector3 m_PortraitImpactRotation = new Vector3(-15f, 0.0f, 0.0f);
  public Transform m_NameStartPosition;
  public Transform m_NameTargetPosition;
  public float m_NameStartDelay;
  public float m_NameAnimTime = 1f;
  public GameObject m_NameGlow;
  public ParticleSystem m_NameHitBlastParticle;
  public Vector3 m_NameImpactRotation = new Vector3(-15f, 0.0f, 0.0f);
  public Transform m_RarityStartPosition;
  public Transform m_RarityTargetPosition;
  public float m_RarityStartDelay;
  public float m_RarityAnimTime = 1f;
  public GameObject m_RarityGlowCommon;
  public GameObject m_RarityGlowRare;
  public GameObject m_RarityGlowEpic;
  public GameObject m_RarityGlowLegendary;
  public ParticleSystem m_RarityHitBlastParticle;
  public Vector3 m_RarityImpactRotation = new Vector3(-15f, 0.0f, 0.0f);
  public Transform m_DkRunesStartPosition;
  public Transform m_DkRunesTargetPosition;
  public float m_DkRuneStartDelay;
  public float m_DkRuneAnimTime = 1f;
  public GameObject m_DkRunes;
  public ParticleSystem m_DkRunesHitBlastParticle;
  public Vector3 m_DkRuneImpactRotation = new Vector3(-15f, 0.0f, 0.0f);
  private Actor m_Actor;
  private Spell m_GhostSpell;
  private float m_AnimationScale = 1f;
  private bool isInit;
  private GameObject m_ManaGemInstance;
  private GameObject m_DescriptionInstance;
  private GameObject m_AttackInstance;
  private GameObject m_HealthInstance;
  private GameObject m_ArmorInstance;
  private GameObject m_PortraitInstance;
  private GameObject m_NameInstance;
  private GameObject m_RarityInstance;
  private GameObject m_DkRunesInstance;
  private GameObject m_CardMesh;
  private int m_CardFrontIdx;
  private GameObject m_PortraitMesh;
  private int m_PortraitFrameIdx;
  private GameObject m_NameMesh;
  private GameObject m_DescriptionMesh;
  private GameObject m_DescriptionTrimMesh;
  private GameObject m_RarityGemMesh;
  private GameObject m_RarityFrameMesh;
  private GameObject m_ManaCostMesh;
  private GameObject m_AttackMesh;
  private GameObject m_HealthMesh;
  private GameObject m_ArmorMesh;
  private GameObject m_RacePlateMesh;
  private GameObject m_EliteMesh;
  private GameObject m_DkRunesMesh;
  private GameObject m_ManaGemClone;
  private Material m_OrgMat_CardFront;
  private Material m_OrgMat_PortraitFrame;
  private Material m_OrgMat_Name;
  private Material m_OrgMat_Description;
  private Material m_OrgMat_Description2;
  private Material m_OrgMat_DescriptionTrim;
  private Material m_OrgMat_RarityFrame;
  private Material m_OrgMat_ManaCost;
  private Material m_OrgMat_Attack;
  private Material m_OrgMat_Health;
  private Material m_OrgMat_Armor;
  private Material m_OrgMat_RacePlate;
  private Material m_OrgMat_Elite;
  private List<ParticleSystem> m_tempParticleSystems = new List<ParticleSystem>();
  private List<Renderer> m_tempRenderers = new List<Renderer>();

  private void OnDisable() => this.Cancel();

  private void OnDestroy()
  {
    this.m_tempParticleSystems = (List<ParticleSystem>) null;
    this.m_tempRenderers = (List<Renderer>) null;
  }

  public void Construct() => this.StartCoroutine(this.DoConstruct());

  private IEnumerator DoConstruct()
  {
    ConstructCard constructCard = this;
    constructCard.m_Actor = GameObjectUtils.FindComponentInThisOrParents<Actor>(constructCard.gameObject);
    if ((Object) constructCard.m_Actor == (Object) null)
    {
      Debug.LogError((object) string.Format("{0} Ghost card effect failed to find Actor!", (object) constructCard.transform.root.name));
      constructCard.enabled = false;
    }
    else
    {
      constructCard.m_Actor.HideAllText();
      constructCard.m_GhostSpell = constructCard.m_Actor.GetSpell(SpellType.GHOSTMODE);
      constructCard.m_GhostSpell.ActivateState(SpellStateType.CANCEL);
      constructCard.m_Actor.ActivateSpellDeathState(SpellType.GHOSTMODE);
      while (constructCard.m_GhostSpell.IsActive() || constructCard.m_Actor.m_ghostCardActive)
        yield return (object) new WaitForEndOfFrame();
      constructCard.m_Actor.HideAllText();
      constructCard.Init();
      constructCard.CreateInstances();
      if ((bool) (Object) constructCard.m_GhostGlow)
      {
        Renderer component = constructCard.m_GhostGlow.GetComponent<Renderer>();
        if (constructCard.m_Actor.IsElite() && (bool) (Object) constructCard.m_GhostTextureUnique)
          component.GetMaterial().mainTexture = constructCard.m_GhostTextureUnique;
        component.enabled = true;
        constructCard.m_GhostGlow.GetComponent<Animation>().Play("GhostModeHot", PlayMode.StopAll);
      }
      if ((bool) (Object) constructCard.m_RarityGemMesh)
        constructCard.m_RarityGemMesh.GetComponent<Renderer>().enabled = false;
      if ((bool) (Object) constructCard.m_RarityFrameMesh)
        constructCard.m_RarityFrameMesh.GetComponent<Renderer>().enabled = false;
      if ((bool) (Object) constructCard.m_DkRunesMesh)
      {
        foreach (Renderer componentsInChild in constructCard.m_DkRunesMesh.GetComponentsInChildren<Renderer>())
          componentsInChild.enabled = false;
      }
      if ((bool) (Object) constructCard.m_ManaGemStartPosition && (bool) (Object) constructCard.m_ManaGemInstance)
        constructCard.AnimateManaGem();
      if ((bool) (Object) constructCard.m_DescriptionStartPosition && (bool) (Object) constructCard.m_DescriptionInstance)
        constructCard.AnimateDescription();
      if ((bool) (Object) constructCard.m_AttackStartPosition && (bool) (Object) constructCard.m_AttackInstance)
        constructCard.AnimateAttack();
      if ((bool) (Object) constructCard.m_HealthStartPosition && (bool) (Object) constructCard.m_HealthInstance)
        constructCard.AnimateHealth();
      if ((bool) (Object) constructCard.m_ArmorStartPosition && (bool) (Object) constructCard.m_ArmorInstance)
        constructCard.AnimateArmor();
      if ((bool) (Object) constructCard.m_PortraitStartPosition && (bool) (Object) constructCard.m_PortraitInstance)
        constructCard.AnimatePortrait();
      if ((bool) (Object) constructCard.m_NameStartPosition && (bool) (Object) constructCard.m_NameInstance)
        constructCard.AnimateName();
      if ((bool) (Object) constructCard.m_RarityStartPosition)
        constructCard.AnimateRarity();
      if ((bool) (Object) constructCard.m_DkRunesStartPosition)
        constructCard.AnimateDkRunes();
    }
  }

  private void Init()
  {
    if (this.isInit)
      return;
    this.m_Actor = GameObjectUtils.FindComponentInThisOrParents<Actor>(this.gameObject);
    if ((Object) this.m_Actor == (Object) null)
    {
      Debug.LogError((object) string.Format("{0} Ghost card effect failed to find Actor!", (object) this.transform.root.name));
      this.enabled = false;
    }
    else
    {
      this.m_CardMesh = this.m_Actor.m_cardMesh;
      this.m_CardFrontIdx = this.m_Actor.m_cardFrontMatIdx;
      this.m_PortraitMesh = this.m_Actor.m_portraitMesh;
      this.m_PortraitFrameIdx = this.m_Actor.m_portraitFrameMatIdx;
      this.m_NameMesh = this.m_Actor.m_nameBannerMesh;
      this.m_DescriptionMesh = this.m_Actor.m_descriptionMesh;
      this.m_DescriptionTrimMesh = this.m_Actor.m_descriptionTrimMesh;
      this.m_RarityGemMesh = this.m_Actor.m_rarityGemMesh;
      this.m_RarityFrameMesh = this.m_Actor.m_rarityFrameMesh;
      if ((bool) (Object) this.m_Actor.m_attackObject)
      {
        Renderer component = this.m_Actor.m_attackObject.GetComponent<Renderer>();
        if ((Object) component != (Object) null)
          this.m_AttackMesh = component.gameObject;
        if ((Object) this.m_AttackMesh == (Object) null)
        {
          this.m_Actor.m_attackObject.GetComponentsInChildren<Renderer>(this.m_tempRenderers);
          foreach (Renderer tempRenderer in this.m_tempRenderers)
          {
            if (!(bool) (Object) tempRenderer.GetComponent<UberText>())
              this.m_AttackMesh = tempRenderer.gameObject;
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
          this.m_Actor.m_healthObject.GetComponentsInChildren<Renderer>(this.m_tempRenderers);
          foreach (Renderer tempRenderer in this.m_tempRenderers)
          {
            if (!(bool) (Object) tempRenderer.GetComponent<UberText>())
              this.m_HealthMesh = tempRenderer.gameObject;
          }
        }
      }
      if ((bool) (Object) this.m_Actor.m_armorObject)
      {
        Renderer component = this.m_Actor.m_armorObject.GetComponent<Renderer>();
        if ((Object) component != (Object) null)
          this.m_ArmorMesh = component.gameObject;
        if ((Object) this.m_ArmorMesh == (Object) null)
        {
          this.m_Actor.m_armorObject.GetComponentsInChildren<Renderer>(this.m_tempRenderers);
          foreach (Renderer tempRenderer in this.m_tempRenderers)
          {
            if (!(bool) (Object) tempRenderer.GetComponent<UberText>())
              this.m_ArmorMesh = tempRenderer.gameObject;
          }
        }
      }
      this.m_ManaCostMesh = this.m_Actor.m_manaObject;
      this.m_RacePlateMesh = this.m_Actor.m_racePlateObject;
      this.m_EliteMesh = this.m_Actor.m_eliteObject;
      CardRuneBanner cardRuneBanner = this.m_Actor.m_cardRuneBanner;
      if ((Object) cardRuneBanner != (Object) null)
      {
        this.m_DkRunesMesh = cardRuneBanner.gameObject;
        cardRuneBanner.Hide();
      }
      this.StoreOrgMaterials();
      switch (this.m_Actor.GetRarity())
      {
        case TAG_RARITY.RARE:
          this.m_AnimationScale = this.m_AnimationRarityScaleRare;
          break;
        case TAG_RARITY.EPIC:
          this.m_AnimationScale = this.m_AnimationRarityScaleEpic;
          break;
        case TAG_RARITY.LEGENDARY:
          this.m_AnimationScale = this.m_AnimationRarityScaleLegendary;
          break;
        default:
          this.m_AnimationScale = this.m_AnimationRarityScaleCommon;
          break;
      }
      this.isInit = true;
    }
  }

  private void Cancel()
  {
    this.StopAllCoroutines();
    this.RestoreOrgMaterials();
    this.DisableManaGem();
    this.DisableDescription();
    this.DisableAttack();
    this.DisableHealth();
    this.DisableArmor();
    this.DisablePortrait();
    this.DisableName();
    this.DisableRarity();
    this.DestroyInstances();
    this.StopAllParticles();
    this.HideAllMeshObjects();
    if ((bool) (Object) this.m_Actor)
      this.m_Actor.ShowAllText();
    if (!((Object) this.m_Actor != (Object) null))
      return;
    iTween.StopByName(this.m_Actor.gameObject, "CardConstructImpactRotation");
  }

  private void StopAllParticles()
  {
    this.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
    {
      if (tempParticleSystem.isPlaying)
        tempParticleSystem.Stop();
    }
  }

  private void HideAllMeshObjects()
  {
    foreach (Component componentsInChild in this.GetComponentsInChildren<MeshRenderer>())
      componentsInChild.GetComponent<Renderer>().enabled = false;
  }

  private void CreateInstances()
  {
    Vector3 vector3 = new Vector3(0.0f, -5000f, 0.0f);
    if ((bool) (Object) this.m_RarityGemMesh)
      this.m_RarityGemMesh.GetComponent<Renderer>().enabled = false;
    if ((bool) (Object) this.m_RarityFrameMesh)
      this.m_RarityFrameMesh.GetComponent<Renderer>().enabled = false;
    if ((bool) (Object) this.m_ManaGemStartPosition && (bool) (Object) this.m_ManaCostMesh)
    {
      this.m_ManaGemInstance = Object.Instantiate<GameObject>(this.m_ManaCostMesh);
      this.m_ManaGemInstance.transform.parent = this.transform.parent;
      this.m_ManaGemInstance.transform.position = vector3;
    }
    if ((bool) (Object) this.m_DescriptionStartPosition && (bool) (Object) this.m_DescriptionMesh)
    {
      this.m_DescriptionInstance = Object.Instantiate<GameObject>(this.m_DescriptionMesh);
      this.m_DescriptionInstance.transform.parent = this.transform.parent;
      this.m_DescriptionInstance.transform.position = vector3;
    }
    if ((bool) (Object) this.m_AttackStartPosition && (bool) (Object) this.m_AttackMesh)
    {
      this.m_AttackInstance = Object.Instantiate<GameObject>(this.m_AttackMesh);
      this.m_AttackInstance.transform.parent = this.transform.parent;
      this.m_AttackInstance.transform.position = vector3;
    }
    if ((bool) (Object) this.m_HealthStartPosition && (bool) (Object) this.m_HealthMesh)
    {
      this.m_HealthInstance = Object.Instantiate<GameObject>(this.m_HealthMesh);
      this.m_HealthInstance.transform.parent = this.transform.parent;
      this.m_HealthInstance.transform.position = vector3;
    }
    if ((bool) (Object) this.m_ArmorStartPosition && (bool) (Object) this.m_ArmorMesh)
    {
      this.m_ArmorInstance = Object.Instantiate<GameObject>(this.m_ArmorMesh);
      this.m_ArmorInstance.transform.parent = this.transform.parent;
      this.m_ArmorInstance.transform.position = vector3;
    }
    if ((bool) (Object) this.m_PortraitStartPosition && (bool) (Object) this.m_PortraitMesh)
    {
      this.m_PortraitInstance = Object.Instantiate<GameObject>(this.m_PortraitMesh);
      this.m_PortraitInstance.transform.parent = this.transform.parent;
      this.m_PortraitInstance.transform.position = vector3;
    }
    if ((bool) (Object) this.m_NameStartPosition && (bool) (Object) this.m_NameMesh)
    {
      this.m_NameInstance = Object.Instantiate<GameObject>(this.m_NameMesh);
      this.m_NameInstance.transform.parent = this.transform.parent;
      this.m_NameInstance.transform.position = vector3;
    }
    if ((bool) (Object) this.m_RarityStartPosition && (bool) (Object) this.m_RarityGemMesh)
    {
      this.m_RarityInstance = Object.Instantiate<GameObject>(this.m_RarityGemMesh);
      this.m_RarityInstance.transform.parent = this.transform.parent;
      this.m_RarityInstance.transform.position = vector3;
    }
    if (!(bool) (Object) this.m_DkRunesStartPosition || !(bool) (Object) this.m_DkRunes)
      return;
    this.m_DkRunesInstance = Object.Instantiate<GameObject>(this.m_DkRunesMesh);
    this.m_DkRunesInstance.transform.parent = this.transform.parent;
    this.m_DkRunesInstance.transform.position = vector3;
  }

  private void DestroyInstances()
  {
    if ((bool) (Object) this.m_ManaGemInstance)
      Object.Destroy((Object) this.m_ManaGemInstance);
    if ((bool) (Object) this.m_DescriptionInstance)
      Object.Destroy((Object) this.m_DescriptionInstance);
    if ((bool) (Object) this.m_AttackInstance)
      Object.Destroy((Object) this.m_AttackInstance);
    if ((bool) (Object) this.m_HealthInstance)
      Object.Destroy((Object) this.m_HealthInstance);
    if ((bool) (Object) this.m_ArmorInstance)
      Object.Destroy((Object) this.m_ArmorInstance);
    if ((bool) (Object) this.m_PortraitInstance)
      Object.Destroy((Object) this.m_PortraitInstance);
    if ((bool) (Object) this.m_NameInstance)
      Object.Destroy((Object) this.m_NameInstance);
    if ((bool) (Object) this.m_RarityInstance)
      Object.Destroy((Object) this.m_RarityInstance);
    if (!(bool) (Object) this.m_DkRunesInstance)
      return;
    Object.Destroy((Object) this.m_DkRunesInstance);
  }

  private void AnimateManaGem()
  {
    GameObject manaGemInstance = this.m_ManaGemInstance;
    manaGemInstance.transform.parent = (Transform) null;
    manaGemInstance.transform.localScale = this.m_ManaCostMesh.transform.lossyScale;
    manaGemInstance.transform.position = this.m_ManaGemStartPosition.transform.position;
    manaGemInstance.transform.parent = this.transform.parent;
    manaGemInstance.GetComponent<Renderer>().SetMaterial(this.m_OrgMat_ManaCost);
    float num = Random.Range(this.m_ManaGemStartDelay - this.m_ManaGemStartDelay * this.m_RandomDelayVariance, this.m_ManaGemStartDelay + this.m_ManaGemStartDelay * this.m_RandomDelayVariance);
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "ManaGem",
      AnimateTransform = manaGemInstance.transform,
      StartTransform = this.m_ManaGemStartPosition.transform,
      TargetTransform = this.m_ManaGemTargetPosition.transform,
      HitBlastParticle = this.m_ManaGemHitBlastParticle,
      AnimationTime = this.m_ManaGemAnimTime,
      StartDelay = num,
      GlowObject = this.m_ManaGemGlow,
      ImpactRotation = this.m_ManaGemImpactRotation,
      OnComplete = "ManaGemOnComplete"
    });
  }

  private IEnumerator ManaGemOnComplete()
  {
    this.DisableManaGem();
    yield break;
  }

  private void DisableManaGem()
  {
    if (!(bool) (Object) this.m_ManaGemGlow)
      return;
    this.m_ManaGemGlow.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
      tempParticleSystem.Stop();
  }

  private void AnimateDescription()
  {
    GameObject descriptionInstance = this.m_DescriptionInstance;
    descriptionInstance.transform.parent = (Transform) null;
    descriptionInstance.transform.localScale = this.m_DescriptionMesh.transform.lossyScale;
    descriptionInstance.transform.position = this.m_DescriptionStartPosition.transform.position;
    descriptionInstance.transform.parent = this.transform.parent;
    descriptionInstance.GetComponent<Renderer>().SetMaterial(this.m_OrgMat_Description);
    float num = Random.Range(this.m_DescriptionStartDelay - this.m_DescriptionStartDelay * this.m_RandomDelayVariance, this.m_DescriptionStartDelay + this.m_DescriptionStartDelay * this.m_RandomDelayVariance);
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "Description",
      AnimateTransform = descriptionInstance.transform,
      StartTransform = this.m_DescriptionStartPosition.transform,
      TargetTransform = this.m_DescriptionTargetPosition.transform,
      HitBlastParticle = this.m_DescriptionHitBlastParticle,
      AnimationTime = this.m_DescriptionAnimTime,
      StartDelay = num,
      GlowObject = this.m_DescriptionGlow,
      ImpactRotation = this.m_DescriptionImpactRotation,
      OnComplete = "DescriptionOnComplete"
    });
  }

  private IEnumerator DescriptionOnComplete()
  {
    this.DisableDescription();
    yield break;
  }

  private void DisableDescription()
  {
    if (!(bool) (Object) this.m_DescriptionGlow)
      return;
    this.m_DescriptionGlow.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
      tempParticleSystem.Stop();
  }

  private void AnimateAttack()
  {
    GameObject attackInstance = this.m_AttackInstance;
    attackInstance.transform.parent = (Transform) null;
    attackInstance.transform.localScale = this.m_AttackMesh.transform.lossyScale;
    attackInstance.transform.position = this.m_AttackStartPosition.transform.position;
    attackInstance.transform.parent = this.transform.parent;
    attackInstance.GetComponent<Renderer>().SetMaterial(this.m_OrgMat_Attack);
    float num = Random.Range(this.m_AttackStartDelay - this.m_AttackStartDelay * this.m_RandomDelayVariance, this.m_AttackStartDelay + this.m_AttackStartDelay * this.m_RandomDelayVariance);
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "Attack",
      AnimateTransform = attackInstance.transform,
      StartTransform = this.m_AttackStartPosition.transform,
      TargetTransform = this.m_AttackTargetPosition.transform,
      HitBlastParticle = this.m_AttackHitBlastParticle,
      AnimationTime = this.m_AttackAnimTime,
      StartDelay = num,
      GlowObject = this.m_AttackGlow,
      ImpactRotation = this.m_AttackImpactRotation,
      OnComplete = "AttackOnComplete"
    });
  }

  private IEnumerator AttackOnComplete()
  {
    this.DisableAttack();
    yield break;
  }

  private void DisableAttack()
  {
    if (!(bool) (Object) this.m_AttackGlow)
      return;
    this.m_AttackGlow.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
      tempParticleSystem.Stop();
  }

  private void AnimateHealth()
  {
    GameObject healthInstance = this.m_HealthInstance;
    healthInstance.transform.parent = (Transform) null;
    healthInstance.transform.localScale = this.m_HealthMesh.transform.lossyScale;
    healthInstance.transform.position = this.m_HealthStartPosition.transform.position;
    healthInstance.transform.parent = this.transform.parent;
    healthInstance.GetComponent<Renderer>().SetMaterial(this.m_OrgMat_Health);
    float num = Random.Range(this.m_HealthStartDelay - this.m_HealthStartDelay * this.m_RandomDelayVariance, this.m_HealthStartDelay + this.m_HealthStartDelay * this.m_RandomDelayVariance);
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "Health",
      AnimateTransform = healthInstance.transform,
      StartTransform = this.m_HealthStartPosition.transform,
      TargetTransform = this.m_HealthTargetPosition.transform,
      HitBlastParticle = this.m_HealthHitBlastParticle,
      AnimationTime = this.m_HealthAnimTime,
      StartDelay = num,
      GlowObject = this.m_HealthGlow,
      ImpactRotation = this.m_HealthImpactRotation,
      OnComplete = "HealthOnComplete"
    });
  }

  private IEnumerator HealthOnComplete()
  {
    this.DisableHealth();
    yield break;
  }

  private void DisableHealth()
  {
    if (!(bool) (Object) this.m_HealthGlow)
      return;
    this.m_HealthGlow.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
      tempParticleSystem.Stop();
  }

  private void AnimateArmor()
  {
    GameObject armorInstance = this.m_ArmorInstance;
    armorInstance.transform.parent = (Transform) null;
    armorInstance.transform.localScale = this.m_ArmorMesh.transform.lossyScale;
    armorInstance.transform.position = this.m_ArmorStartPosition.transform.position;
    armorInstance.transform.parent = this.transform.parent;
    armorInstance.GetComponent<Renderer>().SetMaterial(this.m_OrgMat_Armor);
    float num = Random.Range(this.m_ArmorStartDelay - this.m_ArmorStartDelay * this.m_RandomDelayVariance, this.m_ArmorStartDelay + this.m_ArmorStartDelay * this.m_RandomDelayVariance);
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "Armor",
      AnimateTransform = armorInstance.transform,
      StartTransform = this.m_ArmorStartPosition.transform,
      TargetTransform = this.m_ArmorTargetPosition.transform,
      HitBlastParticle = this.m_ArmorHitBlastParticle,
      AnimationTime = this.m_ArmorAnimTime,
      StartDelay = num,
      GlowObject = this.m_ArmorGlow,
      ImpactRotation = this.m_ArmorImpactRotation,
      OnComplete = "ArmorOnComplete"
    });
  }

  private IEnumerator ArmorOnComplete()
  {
    this.DisableArmor();
    yield break;
  }

  private void DisableArmor()
  {
    if (!(bool) (Object) this.m_ArmorGlow)
      return;
    this.m_ArmorGlow.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
      tempParticleSystem.Stop();
  }

  private void AnimatePortrait()
  {
    GameObject portraitInstance = this.m_PortraitInstance;
    portraitInstance.transform.parent = (Transform) null;
    portraitInstance.transform.localScale = this.m_PortraitMesh.transform.lossyScale;
    portraitInstance.transform.position = this.m_PortraitStartPosition.transform.position;
    portraitInstance.transform.parent = this.transform.parent;
    float num = Random.Range(this.m_PortraitStartDelay - this.m_PortraitStartDelay * this.m_RandomDelayVariance, this.m_PortraitStartDelay + this.m_PortraitStartDelay * this.m_RandomDelayVariance);
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "Portrait",
      AnimateTransform = portraitInstance.transform,
      StartTransform = this.m_PortraitStartPosition.transform,
      TargetTransform = this.m_PortraitTargetPosition.transform,
      HitBlastParticle = this.m_PortraitHitBlastParticle,
      AnimationTime = this.m_PortraitAnimTime,
      StartDelay = num,
      GlowObject = this.m_PortraitGlow,
      GlowObjectStandard = this.m_PortraitGlowStandard,
      GlowObjectUnique = this.m_PortraitGlowUnique,
      ImpactRotation = this.m_PortraitImpactRotation,
      OnComplete = "PortraitOnComplete"
    });
  }

  private IEnumerator PortraitOnComplete()
  {
    this.DisablePortrait();
    yield break;
  }

  private void DisablePortrait()
  {
    if (!(bool) (Object) this.m_PortraitGlow)
      return;
    this.m_PortraitGlow.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
      tempParticleSystem.Stop();
  }

  private void AnimateName()
  {
    GameObject nameInstance = this.m_NameInstance;
    nameInstance.transform.parent = (Transform) null;
    nameInstance.transform.localScale = this.m_NameMesh.transform.lossyScale;
    nameInstance.transform.position = this.m_NameStartPosition.transform.position;
    nameInstance.transform.parent = this.transform.parent;
    nameInstance.GetComponent<Renderer>().SetMaterial(this.m_OrgMat_Name);
    float num = Random.Range(this.m_NameStartDelay - this.m_NameStartDelay * this.m_RandomDelayVariance, this.m_NameStartDelay + this.m_NameStartDelay * this.m_RandomDelayVariance);
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "Name",
      AnimateTransform = nameInstance.transform,
      StartTransform = this.m_NameStartPosition.transform,
      TargetTransform = this.m_NameTargetPosition.transform,
      HitBlastParticle = this.m_NameHitBlastParticle,
      AnimationTime = this.m_NameAnimTime,
      StartDelay = num,
      GlowObject = this.m_NameGlow,
      ImpactRotation = this.m_NameImpactRotation,
      OnComplete = "NameOnComplete"
    });
  }

  private IEnumerator NameOnComplete()
  {
    this.DisableName();
    yield break;
  }

  private void DisableName()
  {
    if (!(bool) (Object) this.m_NameGlow)
      return;
    this.m_NameGlow.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
      tempParticleSystem.Stop();
  }

  private void AnimateRarity()
  {
    if (this.m_Actor.GetRarity() == TAG_RARITY.FREE)
      return;
    GameObject rarityInstance = this.m_RarityInstance;
    rarityInstance.transform.parent = (Transform) null;
    rarityInstance.transform.localScale = this.m_RarityGemMesh.transform.lossyScale;
    rarityInstance.transform.position = this.m_RarityStartPosition.transform.position;
    rarityInstance.transform.parent = this.transform.parent;
    this.m_RarityInstance.GetComponent<Renderer>().enabled = true;
    GameObject gameObject = this.m_RarityGlowCommon;
    switch (this.m_Actor.GetRarity())
    {
      case TAG_RARITY.RARE:
        gameObject = this.m_RarityGlowRare;
        break;
      case TAG_RARITY.EPIC:
        gameObject = this.m_RarityGlowEpic;
        break;
      case TAG_RARITY.LEGENDARY:
        gameObject = this.m_RarityGlowLegendary;
        break;
    }
    float num = Random.Range(this.m_RarityStartDelay - this.m_RarityStartDelay * this.m_RandomDelayVariance, this.m_RarityStartDelay + this.m_RarityStartDelay * this.m_RandomDelayVariance);
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "Rarity",
      AnimateTransform = rarityInstance.transform,
      StartTransform = this.m_RarityStartPosition.transform,
      TargetTransform = this.m_RarityTargetPosition.transform,
      HitBlastParticle = this.m_RarityHitBlastParticle,
      AnimationTime = this.m_RarityAnimTime,
      StartDelay = num,
      GlowObject = gameObject,
      ImpactRotation = this.m_RarityImpactRotation,
      OnComplete = "RarityOnComplete"
    });
  }

  private IEnumerator RarityOnComplete()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    ConstructCard constructCard = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    constructCard.DisableRarity();
    if (constructCard.m_Actor.GetRarity() != TAG_RARITY.FREE)
    {
      if ((bool) (Object) constructCard.m_RarityGemMesh)
        constructCard.m_RarityGemMesh.GetComponent<Renderer>().enabled = true;
      if ((bool) (Object) constructCard.m_RarityFrameMesh)
        constructCard.m_RarityFrameMesh.GetComponent<Renderer>().enabled = true;
    }
    constructCard.StartCoroutine(constructCard.EndAnimation());
    return false;
  }

  private void DisableRarity()
  {
    if (!(bool) (Object) this.m_RarityGlowCommon)
      return;
    this.m_RarityGlowCommon.GetComponentsInChildren<ParticleSystem>(this.m_tempParticleSystems);
    foreach (ParticleSystem tempParticleSystem in this.m_tempParticleSystems)
      tempParticleSystem.Stop();
  }

  private void AnimateDkRunes()
  {
    GameObject dkRunesInstance = this.m_DkRunesInstance;
    dkRunesInstance.transform.parent = (Transform) null;
    dkRunesInstance.transform.localScale = this.m_DkRunesMesh.transform.lossyScale;
    dkRunesInstance.transform.position = this.m_DkRunesStartPosition.transform.position;
    dkRunesInstance.transform.parent = this.transform.parent;
    float num = Random.Range(this.m_DkRuneStartDelay - this.m_DkRuneStartDelay * this.m_RandomDelayVariance, this.m_DkRuneStartDelay + this.m_DkRuneStartDelay * this.m_RandomDelayVariance);
    CardRuneBanner component1 = dkRunesInstance.GetComponent<CardRuneBanner>();
    CardRuneBanner component2 = this.m_DkRunesMesh.GetComponent<CardRuneBanner>();
    if ((Object) component1 != (Object) null && (Object) component2 != (Object) null)
    {
      RunePattern currentRunePattern = component2.GetCurrentRunePattern();
      component1.Show(currentRunePattern);
    }
    this.StartCoroutine("AnimateObject", (object) new ConstructCard.AnimationData()
    {
      Name = "Runes",
      AnimateTransform = dkRunesInstance.transform,
      StartTransform = this.m_DkRunesStartPosition.transform,
      TargetTransform = this.m_DkRunesTargetPosition.transform,
      HitBlastParticle = this.m_DkRunesHitBlastParticle,
      AnimationTime = this.m_DkRuneAnimTime,
      StartDelay = num,
      GlowObject = (GameObject) null,
      ImpactRotation = this.m_DkRuneImpactRotation,
      OnComplete = "RunesOnComplete"
    });
  }

  private IEnumerator RunesOnComplete()
  {
    if ((Object) this.m_DkRunesMesh != (Object) null)
    {
      CardRuneBanner component = this.m_DkRunesMesh.GetComponent<CardRuneBanner>();
      if ((Object) component != (Object) null)
      {
        component.ShowLastShownRuneBanner();
        yield break;
      }
    }
  }

  private IEnumerator EndAnimation()
  {
    ParticleSystem particleSystem = this.m_RarityBurstCommon;
    TAG_RARITY rarity = this.m_Actor.GetRarity();
    switch (rarity)
    {
      case TAG_RARITY.RARE:
        particleSystem = this.m_RarityBurstRare;
        break;
      case TAG_RARITY.EPIC:
        particleSystem = this.m_RarityBurstEpic;
        break;
      case TAG_RARITY.LEGENDARY:
        particleSystem = this.m_RarityBurstLegendary;
        break;
    }
    if ((bool) (Object) particleSystem)
    {
      particleSystem.GetComponentsInChildren<Renderer>(this.m_tempRenderers);
      foreach (Renderer tempRenderer in this.m_tempRenderers)
        tempRenderer.enabled = true;
      particleSystem.Play(true);
    }
    string animation = "CardFuse_Common";
    switch (rarity)
    {
      case TAG_RARITY.RARE:
        animation = "CardFuse_Rare";
        break;
      case TAG_RARITY.EPIC:
        animation = "CardFuse_Epic";
        break;
      case TAG_RARITY.LEGENDARY:
        animation = "CardFuse_Legendary";
        break;
    }
    if ((bool) (Object) this.m_FuseGlow)
    {
      this.m_FuseGlow.GetComponent<Renderer>().enabled = true;
      this.m_FuseGlow.GetComponent<Animation>().Play(animation, PlayMode.StopAll);
    }
    yield return (object) new WaitForSeconds(0.25f);
    this.DestroyInstances();
    this.m_Actor.ShowAllText();
    this.RestoreOrgMaterials();
  }

  private IEnumerator AnimateObject(ConstructCard.AnimationData animData)
  {
    ConstructCard constructCard = this;
    yield return (object) new WaitForSeconds(animData.StartDelay);
    float animPos = 0.0f;
    float rate = (float) (1.0 / ((double) animData.AnimationTime * (double) constructCard.m_AnimationScale));
    Quaternion rotation1 = constructCard.m_Actor.transform.rotation;
    constructCard.m_Actor.transform.rotation = Quaternion.identity;
    Vector3 startPosition = animData.StartTransform.position;
    Quaternion startRotation = animData.StartTransform.rotation;
    constructCard.m_Actor.transform.rotation = rotation1;
    if ((bool) (Object) animData.GlowObject)
    {
      GameObject glowObject = animData.GlowObject;
      glowObject.transform.parent = animData.AnimateTransform;
      glowObject.transform.localPosition = Vector3.zero;
      glowObject.GetComponentsInChildren<ParticleSystem>(constructCard.m_tempParticleSystems);
      foreach (ParticleSystem tempParticleSystem in constructCard.m_tempParticleSystems)
        tempParticleSystem.Play();
      if ((bool) (Object) animData.GlowObjectStandard && (bool) (Object) animData.GlowObjectUnique)
      {
        if (constructCard.m_Actor.IsElite())
          animData.GlowObjectUnique.GetComponent<Renderer>().enabled = true;
        else
          animData.GlowObjectStandard.GetComponent<Renderer>().enabled = true;
      }
      else
      {
        glowObject.GetComponentsInChildren<Renderer>(constructCard.m_tempRenderers);
        foreach (Renderer tempRenderer in constructCard.m_tempRenderers)
          tempRenderer.enabled = true;
      }
    }
    while ((double) animPos < 1.0)
    {
      Vector3 position = animData.TargetTransform.position;
      Quaternion rotation2 = animData.TargetTransform.rotation;
      animPos += rate * Time.deltaTime;
      Vector3 vector3 = Vector3.Lerp(startPosition, position, animPos);
      Quaternion quaternion = Quaternion.Lerp(startRotation, rotation2, animPos);
      animData.AnimateTransform.position = vector3;
      animData.AnimateTransform.rotation = quaternion;
      yield return (object) null;
    }
    if ((bool) (Object) animData.HitBlastParticle)
    {
      animData.HitBlastParticle.transform.position = animData.TargetTransform.position;
      animData.HitBlastParticle.GetComponent<Renderer>().enabled = true;
      animData.HitBlastParticle.Play();
    }
    animData.AnimateTransform.parent = animData.TargetTransform;
    animData.AnimateTransform.position = animData.TargetTransform.position;
    animData.AnimateTransform.rotation = animData.TargetTransform.rotation;
    if ((bool) (Object) animData.GlowObject)
    {
      foreach (ParticleSystem tempParticleSystem in constructCard.m_tempParticleSystems)
        tempParticleSystem.Stop();
    }
    if (!((Object) constructCard.m_Actor.gameObject == (Object) null))
    {
      constructCard.m_Actor.gameObject.transform.localRotation = Quaternion.Euler(animData.ImpactRotation);
      Hashtable args = iTween.Hash((object) "rotation", (object) Vector3.zero, (object) "time", (object) constructCard.m_ImpactRotationTime, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "space", (object) Space.Self, (object) "name", (object) ("CardConstructImpactRotation" + animData.Name));
      iTween.StopByName(constructCard.m_Actor.gameObject, "CardConstructImpactRotation" + animData.Name);
      iTween.RotateTo(constructCard.m_Actor.gameObject, args);
      CameraShakeMgr.Shake(Camera.main, constructCard.IMPACT_CAMERA_SHAKE_AMOUNT, constructCard.IMPACT_CAMERA_SHAKE_TIME);
      if (animData.OnComplete != string.Empty)
        constructCard.StartCoroutine(animData.OnComplete);
    }
  }

  private void StoreOrgMaterials()
  {
    if ((bool) (Object) this.m_CardMesh)
      this.m_OrgMat_CardFront = this.m_CardMesh.GetComponent<Renderer>().GetMaterial(this.m_CardFrontIdx);
    if ((bool) (Object) this.m_PortraitMesh)
      this.m_OrgMat_PortraitFrame = this.m_PortraitMesh.GetComponent<Renderer>().GetSharedMaterial(this.m_PortraitFrameIdx);
    if ((bool) (Object) this.m_NameMesh)
      this.m_OrgMat_Name = this.m_NameMesh.GetComponent<Renderer>().GetMaterial();
    if ((bool) (Object) this.m_ManaCostMesh)
      this.m_OrgMat_ManaCost = this.m_ManaCostMesh.GetComponent<Renderer>().GetMaterial();
    if ((bool) (Object) this.m_AttackMesh)
      this.m_OrgMat_Attack = this.m_AttackMesh.GetComponent<Renderer>().GetMaterial();
    if ((bool) (Object) this.m_HealthMesh)
      this.m_OrgMat_Health = this.m_HealthMesh.GetComponent<Renderer>().GetMaterial();
    if ((bool) (Object) this.m_ArmorMesh)
      this.m_OrgMat_Armor = this.m_ArmorMesh.GetComponent<Renderer>().GetMaterial();
    if ((bool) (Object) this.m_RacePlateMesh)
      this.m_OrgMat_RacePlate = this.m_RacePlateMesh.GetComponent<Renderer>().GetMaterial();
    if ((bool) (Object) this.m_RarityFrameMesh)
      this.m_OrgMat_RarityFrame = this.m_RarityFrameMesh.GetComponent<Renderer>().GetMaterial();
    if ((bool) (Object) this.m_DescriptionMesh)
    {
      List<Material> materials = this.m_DescriptionMesh.GetComponent<Renderer>().GetMaterials();
      if ((Object) this.m_DescriptionMesh.GetComponent<Renderer>() != (Object) null)
      {
        if (materials.Count > 1)
        {
          this.m_OrgMat_Description = materials[0];
          this.m_OrgMat_Description2 = materials[1];
        }
        else
          this.m_OrgMat_Description = this.m_DescriptionMesh.GetComponent<Renderer>().GetMaterial();
      }
    }
    if ((bool) (Object) this.m_DescriptionTrimMesh)
      this.m_OrgMat_DescriptionTrim = this.m_DescriptionTrimMesh.GetComponent<Renderer>().GetMaterial();
    if (!(bool) (Object) this.m_EliteMesh)
      return;
    this.m_OrgMat_Elite = this.m_EliteMesh.GetComponent<Renderer>().GetMaterial();
  }

  private void RestoreOrgMaterials()
  {
    this.ApplyMaterialByIdx(this.m_CardMesh, this.m_OrgMat_CardFront, this.m_CardFrontIdx);
    this.ApplySharedMaterialByIdx(this.m_PortraitMesh, this.m_OrgMat_PortraitFrame, this.m_PortraitFrameIdx);
    this.ApplyMaterialByIdx(this.m_DescriptionMesh, this.m_OrgMat_Description, 0);
    this.ApplyMaterialByIdx(this.m_DescriptionMesh, this.m_OrgMat_Description2, 1);
    this.ApplyMaterial(this.m_NameMesh, this.m_OrgMat_Name);
    this.ApplyMaterial(this.m_ManaCostMesh, this.m_OrgMat_ManaCost);
    this.ApplyMaterial(this.m_AttackMesh, this.m_OrgMat_Attack);
    this.ApplyMaterial(this.m_HealthMesh, this.m_OrgMat_Health);
    this.ApplyMaterial(this.m_ArmorMesh, this.m_OrgMat_Armor);
    this.ApplyMaterial(this.m_RacePlateMesh, this.m_OrgMat_RacePlate);
    this.ApplyMaterial(this.m_RarityFrameMesh, this.m_OrgMat_RarityFrame);
    this.ApplyMaterial(this.m_DescriptionTrimMesh, this.m_OrgMat_DescriptionTrim);
    this.ApplyMaterial(this.m_EliteMesh, this.m_OrgMat_Elite);
  }

  private void ApplyMaterial(GameObject go, Material mat)
  {
    if ((Object) go == (Object) null)
      return;
    Renderer component = go.GetComponent<Renderer>();
    Texture mainTexture = component.GetMaterial().mainTexture;
    component.SetMaterial(mat);
    component.GetMaterial().mainTexture = mainTexture;
  }

  private void ApplyMaterialByIdx(GameObject go, Material mat, int idx)
  {
    if ((Object) go == (Object) null || (Object) mat == (Object) null || idx < 0)
      return;
    Renderer component = go.GetComponent<Renderer>();
    List<Material> materials = component.GetMaterials();
    if (idx >= materials.Count)
      return;
    Texture mainTexture = component.GetMaterial(idx).mainTexture;
    component.SetMaterial(idx, mat);
    component.GetMaterial(idx).mainTexture = mainTexture;
  }

  private void ApplySharedMaterialByIdx(GameObject go, Material mat, int idx)
  {
    if ((Object) go == (Object) null || (Object) mat == (Object) null || idx < 0)
      return;
    Renderer component = go.GetComponent<Renderer>();
    List<Material> sharedMaterials = component.GetSharedMaterials();
    if (idx >= sharedMaterials.Count)
      return;
    Texture mainTexture = component.GetSharedMaterial(idx).mainTexture;
    component.SetSharedMaterial(idx, mat);
    component.GetSharedMaterial(idx).mainTexture = mainTexture;
  }

  private class AnimationData
  {
    public string Name;
    public Transform AnimateTransform;
    public Transform StartTransform;
    public Transform TargetTransform;
    public float AnimationTime = 1f;
    public float StartDelay;
    public GameObject GlowObject;
    public GameObject GlowObjectStandard;
    public GameObject GlowObjectUnique;
    public ParticleSystem HitBlastParticle;
    public Vector3 ImpactRotation;
    public string OnComplete = string.Empty;
  }
}
