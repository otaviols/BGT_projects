using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiClassBannerTransition : MonoBehaviour
{
  public List<TAG_CLASS> m_ClassList;
  public List<MultiClassBannerTransition.ClassMaterials> m_ClassMaterials;
  public GameObject m_BannerMesh;
  public Material m_GoldenRibbonMaterial;
  public float m_TransitionTime = 0.5f;
  public float m_HoldTime = 2.5f;
  public float m_MoteDelayTime = 0.6f;
  public GameObject m_ParticleFXRoot;
  public ParticleSystem m_TransitionFXParticles;
  public ParticleSystem m_subFXSparks;
  public ParticleSystem m_subFXAura;
  public ParticleSystem m_subFXMotes;
  public GameObject m_Shadow;
  private GameObject m_GoldenRibbonMesh;
  private List<Color> m_bannerColors;
  private List<Color> m_glowColors;
  private List<Material> m_effectMaterials;
  private int m_currentClassNum;
  private float m_nextTransitionTime;
  private float m_transitionEndTime;
  private int m_multiClassGroupID;
  private GameObject m_multiClassGroupIcon;
  private Material m_bannerMaterial;
  private Material m_goldenRibbonMaterial;
  private float m_materialDissolve;
  private Renderer m_TransitionFXMat;
  private Renderer m_subFXSparksMat;

  public static int CompareClasses(TAG_CLASS classA, TAG_CLASS classB)
  {
    int num1 = Array.IndexOf<TAG_CLASS>(CollectionPageManager.CLASS_TAB_ORDER, classA);
    if (num1 < 0)
      return 1;
    int num2 = Array.IndexOf<TAG_CLASS>(CollectionPageManager.CLASS_TAB_ORDER, classB);
    return num2 < 0 ? -1 : num1 - num2;
  }

  private MultiClassBannerTransition.ClassMaterials GetClassMaterials(
    TAG_CLASS cycleClass)
  {
    foreach (MultiClassBannerTransition.ClassMaterials classMaterial in this.m_ClassMaterials)
    {
      if (classMaterial.m_Class == cycleClass)
        return classMaterial;
    }
    return (MultiClassBannerTransition.ClassMaterials) null;
  }

  public void SetClasses(IEnumerable<TAG_CLASS> classes)
  {
    this.m_ClassList.Clear();
    this.m_ClassList.AddRange(classes);
    this.UpdateClassList();
    this.TransitionClass();
  }

  public void SetMultiClassGroup(int groupID)
  {
    if (this.m_multiClassGroupID == groupID)
      return;
    this.m_multiClassGroupID = groupID;
    if ((UnityEngine.Object) this.m_multiClassGroupIcon != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_multiClassGroupIcon);
    MultiClassGroupDbfRecord record = GameDbf.MultiClassGroup.GetRecord(groupID);
    if (record == null || string.IsNullOrEmpty(record.IconAssetPath))
      return;
    this.m_multiClassGroupIcon = AssetLoader.Get().InstantiatePrefab((AssetReference) record.IconAssetPath);
    if ((UnityEngine.Object) this.m_multiClassGroupIcon == (UnityEngine.Object) null)
      return;
    this.m_multiClassGroupIcon.transform.parent = this.m_BannerMesh.transform.parent;
    TransformUtil.Identity(this.m_multiClassGroupIcon);
  }

  public void UpdateClassList()
  {
    if (this.m_bannerColors == null)
      this.m_bannerColors = new List<Color>();
    else
      this.m_bannerColors.Clear();
    if (this.m_glowColors == null)
      this.m_glowColors = new List<Color>();
    else
      this.m_glowColors.Clear();
    if (this.m_effectMaterials == null)
      this.m_effectMaterials = new List<Material>();
    else
      this.m_effectMaterials.Clear();
    foreach (TAG_CLASS cycleClass in this.m_ClassList)
    {
      MultiClassBannerTransition.ClassMaterials classMaterials = this.GetClassMaterials(cycleClass);
      this.m_bannerColors.Add(classMaterials.m_BannerColor);
      this.m_glowColors.Add(classMaterials.m_GlowColor);
      this.m_effectMaterials.Add(classMaterials.m_EffectMaterial);
    }
  }

  public void Awake()
  {
    this.UpdateClassList();
    this.m_bannerMaterial = RendererExtension.GetMaterial(this.m_BannerMesh.GetComponent<Renderer>());
    this.m_TransitionFXMat = this.m_TransitionFXParticles.GetComponent<ParticleSystem>().GetComponent<Renderer>();
    this.m_subFXSparksMat = this.m_subFXSparks.GetComponent<Renderer>();
    this.m_subFXMotes.main.startDelay = (ParticleSystem.MinMaxCurve) this.m_MoteDelayTime;
    this.m_bannerMaterial.SetFloat("_DissolveTime", 0.0f);
    this.UpdateMaterials();
  }

  private void TransitionClass()
  {
    this.m_nextTransitionTime = Time.time + this.m_HoldTime;
    this.m_transitionEndTime = Time.time + this.m_TransitionTime;
    this.m_currentClassNum = (this.m_currentClassNum + 1) % this.m_ClassList.Count;
    this.UpdateMaterials();
    this.PlayParticles();
  }

  public void TransitionClassImmediately()
  {
    this.m_currentClassNum = (this.m_currentClassNum + 1) % this.m_ClassList.Count;
    this.m_bannerMaterial.SetFloat("_DissolveTime", 0.0f);
    if ((UnityEngine.Object) this.m_GoldenRibbonMesh != (UnityEngine.Object) null && (UnityEngine.Object) this.m_goldenRibbonMaterial != (UnityEngine.Object) null)
      this.m_goldenRibbonMaterial.SetFloat("_DissolveTime", 0.0f);
    this.UpdateMaterials();
  }

  public void Update()
  {
    if (this.m_ClassList.Count > 1 && (double) Time.time > (double) this.m_nextTransitionTime)
      this.TransitionClass();
    if ((double) Time.time >= (double) this.m_transitionEndTime + (double) this.m_TransitionTime * 0.25)
      return;
    this.m_materialDissolve = Mathf.Clamp((this.m_transitionEndTime - Time.time) / this.m_TransitionTime, 0.0f, 1f);
    this.m_bannerMaterial.SetFloat("_DissolveTime", this.m_materialDissolve);
    if (!((UnityEngine.Object) this.m_GoldenRibbonMesh != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_goldenRibbonMaterial != (UnityEngine.Object) null))
      return;
    this.m_goldenRibbonMaterial.SetFloat("_DissolveTime", this.m_materialDissolve);
  }

  public void SetGoldenCardMesh(GameObject mesh, int matIdx)
  {
    if (matIdx < 0)
      return;
    this.m_GoldenRibbonMesh = mesh;
    if (!((UnityEngine.Object) this.m_GoldenRibbonMesh != (UnityEngine.Object) null))
      return;
    Renderer component = this.m_GoldenRibbonMesh.GetComponent<Renderer>();
    List<Material> materials = RendererExtension.GetMaterials(component);
    if (matIdx >= materials.Count)
      return;
    RendererExtension.SetMaterial(component, matIdx, this.m_GoldenRibbonMaterial);
    this.m_goldenRibbonMaterial = RendererExtension.GetMaterial(component, matIdx);
    this.m_goldenRibbonMaterial.SetFloat("_DissolveTime", 0.0f);
    this.UpdateMaterials();
  }

  public void TurnOffShadowsAndFX()
  {
    this.m_ParticleFXRoot.SetActive(false);
    this.m_Shadow.gameObject.SetActive(false);
    this.m_multiClassGroupIcon.GetComponent<MultiClassIcon>().m_ShadowMesh.SetActive(false);
  }

  private void PlayParticles()
  {
    this.UpdateParticleScale();
    this.m_TransitionFXParticles.Play();
    this.m_subFXSparks.Play();
    this.m_subFXAura.Play();
    this.m_subFXMotes.Play();
  }

  private void UpdateParticleScale()
  {
    int nextClassNum = this.GetNextClassNum();
    RendererExtension.SetMaterial(this.m_TransitionFXMat, this.m_effectMaterials[nextClassNum]);
    RendererExtension.SetMaterial(this.m_subFXSparksMat, this.m_effectMaterials[nextClassNum]);
    ParticleSystem.MainModule main1 = this.m_subFXAura.main;
    ParticleSystem.MainModule main2 = this.m_subFXMotes.main;
    main1.startColor = (ParticleSystem.MinMaxGradient) this.m_glowColors[nextClassNum];
    main2.startColor = (ParticleSystem.MinMaxGradient) this.m_bannerColors[nextClassNum];
    Vector3 vector3 = this.transform.lossyScale;
    float num = Mathf.Max(vector3.x, Mathf.Max(vector3.y, vector3.z));
    vector3 = new Vector3(num, num, num);
    this.m_TransitionFXParticles.transform.localScale = vector3;
    this.m_subFXAura.transform.localScale = vector3;
    this.m_subFXMotes.transform.localScale = vector3;
    this.m_subFXSparks.transform.localScale = vector3;
  }

  private void UpdateMaterials()
  {
    int nextClassNum = this.GetNextClassNum();
    this.m_bannerMaterial.SetColor("_Color1", this.m_bannerColors[this.m_currentClassNum]);
    this.m_bannerMaterial.SetColor("_Color2", this.m_bannerColors[nextClassNum]);
    this.m_bannerMaterial.SetColor("_ColorGlow", this.m_glowColors[nextClassNum]);
    if (!((UnityEngine.Object) this.m_GoldenRibbonMesh != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_goldenRibbonMaterial != (UnityEngine.Object) null))
      return;
    this.m_goldenRibbonMaterial.SetColor("_Color1", this.m_bannerColors[this.m_currentClassNum]);
    this.m_goldenRibbonMaterial.SetColor("_Color2", this.m_bannerColors[nextClassNum]);
    this.m_goldenRibbonMaterial.SetColor("_ColorGlow", this.m_glowColors[nextClassNum]);
  }

  private int GetNextClassNum() => (this.m_currentClassNum + 1) % this.m_ClassList.Count;

  [Serializable]
  public class ClassMaterials
  {
    public TAG_CLASS m_Class;
    public Color m_BannerColor;
    public Color m_GlowColor;
    public Material m_EffectMaterial;
  }
}
