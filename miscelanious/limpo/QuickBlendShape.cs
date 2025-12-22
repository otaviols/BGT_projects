using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class QuickBlendShape : MonoBehaviour
{
  public bool m_DisableOnMobile;
  public QuickBlendShape.BLEND_SHAPE_ANIMATION_TYPE m_AnimationType;
  public float m_BlendValue;
  public AnimationCurve m_BlendCurve;
  public bool m_Loop = true;
  public bool m_PlayOnAwake;
  public Mesh[] m_Meshes;
  private List<Mesh> m_BlendMeshes;
  private bool m_Play;
  private MeshFilter m_MeshFilter;
  private float m_animTime;
  private List<Material> m_BlendMaterials;
  private Mesh m_OrgMesh;

  private void Awake()
  {
    this.m_MeshFilter = this.GetComponent<MeshFilter>();
    this.m_OrgMesh = this.m_MeshFilter.sharedMesh;
    if (this.m_DisableOnMobile && PlatformSettings.IsMobile())
    {
      if (!((Object) this.m_MeshFilter.sharedMesh == (Object) null) || this.m_Meshes.Length == 0 || !((Object) this.m_Meshes[0] != (Object) null))
        return;
      this.m_MeshFilter.sharedMesh = this.m_Meshes[0];
    }
    else
      this.CreateBlendMeshes();
  }

  private void Update()
  {
    if (this.m_DisableOnMobile && PlatformSettings.IsMobile() || !this.m_Play && this.m_AnimationType != QuickBlendShape.BLEND_SHAPE_ANIMATION_TYPE.Float)
      return;
    this.BlendShapeAnimate();
  }

  private void OnEnable()
  {
    if (this.m_DisableOnMobile && PlatformSettings.IsMobile() || !this.m_PlayOnAwake)
      return;
    this.PlayAnimation();
  }

  private void OnDisable()
  {
    if (this.m_DisableOnMobile && PlatformSettings.IsMobile())
    {
      this.m_MeshFilter.sharedMesh = this.m_OrgMesh;
    }
    else
    {
      this.m_animTime = 0.0f;
      if (this.m_BlendMaterials != null)
      {
        foreach (Material blendMaterial in this.m_BlendMaterials)
          blendMaterial.SetFloat("_Blend", 0.0f);
      }
      if (!((Object) this.m_MeshFilter != (Object) null) || !((Object) this.m_OrgMesh != (Object) null))
        return;
      this.m_MeshFilter.sharedMesh = this.m_OrgMesh;
    }
  }

  public void PlayAnimation()
  {
    if (this.m_DisableOnMobile && PlatformSettings.IsMobile() || (Object) this.m_MeshFilter == (Object) null || this.m_Meshes == null || this.m_BlendMeshes == null)
      return;
    this.m_animTime = 0.0f;
    this.m_Play = true;
  }

  public void StopAnimation()
  {
    if (this.m_DisableOnMobile && PlatformSettings.IsMobile())
      return;
    this.m_Play = false;
  }

  private void BlendShapeAnimate()
  {
    if (this.m_BlendMaterials == null)
      this.m_BlendMaterials = this.GetComponent<Renderer>().GetMaterials();
    MeshFilter component = this.GetComponent<MeshFilter>();
    if ((Object) this.m_MeshFilter == (Object) null)
      this.m_MeshFilter = component;
    float time = this.m_BlendCurve.keys[this.m_BlendCurve.length - 1].time;
    this.m_animTime += Time.deltaTime;
    float blendValue = this.m_BlendValue;
    if ((double) blendValue < 0.0)
      return;
    if (this.m_AnimationType == QuickBlendShape.BLEND_SHAPE_ANIMATION_TYPE.Curve)
      blendValue = this.m_BlendCurve.Evaluate(this.m_animTime);
    int index = Mathf.FloorToInt(blendValue);
    if (index > this.m_BlendMeshes.Count - 1)
      index -= this.m_BlendMeshes.Count - 1;
    this.m_MeshFilter.mesh = this.m_BlendMeshes[index];
    foreach (Material blendMaterial in this.m_BlendMaterials)
      blendMaterial.SetFloat("_Blend", blendValue - (float) Mathf.FloorToInt(blendValue));
    if ((double) this.m_animTime <= (double) time)
      return;
    if (this.m_Loop)
      this.m_animTime = 0.0f;
    else
      this.m_Play = false;
  }

  private void CreateBlendMeshes()
  {
    this.m_BlendMeshes = new List<Mesh>();
    for (int index1 = 0; index1 < this.m_Meshes.Length; ++index1)
    {
      if ((Object) this.GetComponent<MeshFilter>() == (Object) null)
        this.gameObject.AddComponent<MeshFilter>();
      Mesh mesh1 = Object.Instantiate<Mesh>(this.m_Meshes[index1]);
      int length = this.m_Meshes[index1].vertices.Length;
      int index2 = index1 + 1;
      if (index2 > this.m_Meshes.Length - 1)
        index2 = 0;
      Mesh mesh2 = this.m_Meshes[index2];
      Vector4[] vector4Array = new Vector4[length];
      Vector3[] vertices = mesh2.vertices;
      for (int index3 = 0; index3 < length; ++index3)
      {
        if (index3 <= vertices.Length - 1)
        {
          Vector3 vector3 = vertices[index3];
          vector4Array[index3] = new Vector4(vector3.x, vector3.y, vector3.z, 1f);
        }
      }
      mesh1.vertices = this.m_Meshes[index1].vertices;
      mesh1.tangents = vector4Array;
      this.m_BlendMeshes.Add(mesh1);
    }
  }

  public enum BLEND_SHAPE_ANIMATION_TYPE
  {
    Curve,
    Float,
  }
}
