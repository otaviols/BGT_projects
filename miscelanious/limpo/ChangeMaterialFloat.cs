using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class ChangeMaterialFloat : MonoBehaviour
{
  public Renderer m_Rend1;
  public float m_Intensity1;
  private Material m_mat1;
  public Renderer m_Rend2;
  public float m_Intensity2;
  private Material m_mat2;
  public Renderer m_Rend3;
  public float m_Intensity3;
  private Material m_mat3;
  public Renderer m_Rend4;
  public float m_Intensity4;
  private Material m_mat4;
  public Renderer m_Rend5;
  public float m_Intensity5;
  private Material m_mat5;
  public Renderer m_Rend6;
  public float m_Intensity6;
  private Material m_mat6;
  private int m_intensityProperty;

  private void Start()
  {
    this.m_intensityProperty = Shader.PropertyToID("_Intensity");
    if ((Object) this.m_Rend1 != (Object) null)
      this.m_mat1 = this.m_Rend1.GetMaterial();
    if ((Object) this.m_Rend2 != (Object) null)
      this.m_mat2 = this.m_Rend2.GetMaterial();
    if ((Object) this.m_Rend3 != (Object) null)
      this.m_mat3 = this.m_Rend3.GetMaterial();
    if ((Object) this.m_Rend4 != (Object) null)
      this.m_mat4 = this.m_Rend4.GetMaterial();
    if ((Object) this.m_Rend5 != (Object) null)
      this.m_mat5 = this.m_Rend5.GetMaterial();
    if (!((Object) this.m_Rend6 != (Object) null))
      return;
    this.m_mat6 = this.m_Rend6.GetMaterial();
  }

  private void Update()
  {
    if ((Object) this.m_Rend1 != (Object) null)
    {
      this.m_Rend1.enabled = (double) this.m_Intensity1 > 0.0;
      this.m_mat1.SetFloat(this.m_intensityProperty, this.m_Intensity1);
    }
    if ((Object) this.m_Rend2 != (Object) null)
    {
      this.m_Rend2.enabled = (double) this.m_Intensity2 > 0.0;
      this.m_mat2.SetFloat(this.m_intensityProperty, this.m_Intensity2);
    }
    if ((Object) this.m_Rend3 != (Object) null)
    {
      this.m_Rend3.enabled = (double) this.m_Intensity3 > 0.0;
      this.m_mat3.SetFloat(this.m_intensityProperty, this.m_Intensity3);
    }
    if ((Object) this.m_Rend4 != (Object) null)
    {
      this.m_Rend4.enabled = (double) this.m_Intensity4 > 0.0;
      this.m_mat4.SetFloat(this.m_intensityProperty, this.m_Intensity4);
    }
    if ((Object) this.m_Rend5 != (Object) null)
    {
      this.m_Rend5.enabled = (double) this.m_Intensity5 > 0.0;
      this.m_mat5.SetFloat(this.m_intensityProperty, this.m_Intensity5);
    }
    if (!((Object) this.m_Rend6 != (Object) null))
      return;
    this.m_Rend6.enabled = (double) this.m_Intensity6 > 0.0;
    this.m_mat6.SetFloat(this.m_intensityProperty, this.m_Intensity6);
  }

  private void OnDestroy()
  {
    if ((Object) this.m_mat1 != (Object) null)
    {
      Object.Destroy((Object) this.m_mat1);
      this.m_mat1 = (Material) null;
    }
    if ((Object) this.m_mat2 != (Object) null)
    {
      Object.Destroy((Object) this.m_mat2);
      this.m_mat2 = (Material) null;
    }
    if ((Object) this.m_mat3 != (Object) null)
    {
      Object.Destroy((Object) this.m_mat3);
      this.m_mat3 = (Material) null;
    }
    if ((Object) this.m_mat4 != (Object) null)
    {
      Object.Destroy((Object) this.m_mat4);
      this.m_mat4 = (Material) null;
    }
    if ((Object) this.m_mat5 != (Object) null)
    {
      Object.Destroy((Object) this.m_mat5);
      this.m_mat5 = (Material) null;
    }
    if (!((Object) this.m_mat6 != (Object) null))
      return;
    Object.Destroy((Object) this.m_mat6);
    this.m_mat1 = (Material) null;
  }
}
