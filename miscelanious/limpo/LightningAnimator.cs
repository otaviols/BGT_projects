using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class LightningAnimator : MonoBehaviour
{
  public bool m_StartOnEnable;
  public bool m_SetAlphaToZeroOnStart = true;
  public float m_StartDelayMin;
  public float m_StartDelayMax;
  public string m_MatFrameProperty = "_Frame";
  public float m_FrameTime = 0.01f;
  public List<int> m_FrameList;
  public Transform m_SourceJount;
  public Vector3 m_SourceMinRotation = new Vector3(0.0f, -10f, 0.0f);
  public Vector3 m_SourceMaxRotation = new Vector3(0.0f, 10f, 0.0f);
  public Transform m_TargetJoint;
  public Vector3 m_TargetMinRotation = new Vector3(0.0f, -20f, 0.0f);
  public Vector3 m_TargetMaxRotation = new Vector3(0.0f, 20f, 0.0f);
  private Material m_material;
  private float m_matGlowIntensity;

  private void Start()
  {
    this.m_material = this.GetComponent<Renderer>().GetMaterial();
    if ((Object) this.m_material == (Object) null)
      this.enabled = false;
    if (this.m_SetAlphaToZeroOnStart)
      this.m_material.color = this.m_material.color with
      {
        a = 0.0f
      };
    if (!this.m_material.HasProperty("_GlowIntensity"))
      return;
    this.m_matGlowIntensity = this.m_material.GetFloat("_GlowIntensity");
  }

  private void OnEnable()
  {
    if (!this.m_StartOnEnable)
      return;
    this.StartAnimation();
  }

  public void StartAnimation() => this.StartCoroutine(this.AnimateMaterial());

  private IEnumerator AnimateMaterial()
  {
    this.RandomJointRotation();
    Color matColor = this.m_material.color with
    {
      a = 0.0f
    };
    this.m_material.color = matColor;
    yield return (object) new WaitForSeconds(Random.Range(this.m_StartDelayMin, this.m_StartDelayMax));
    matColor = this.m_material.color with { a = 1f };
    this.m_material.color = matColor;
    if (this.m_material.HasProperty("_GlowIntensity"))
      this.m_material.SetFloat("_GlowIntensity", this.m_matGlowIntensity);
    foreach (float frame in this.m_FrameList)
    {
      this.m_material.SetFloat(this.m_MatFrameProperty, frame);
      yield return (object) new WaitForSeconds(this.m_FrameTime);
    }
    matColor.a = 0.0f;
    this.m_material.color = matColor;
    if (this.m_material.HasProperty("_GlowIntensity"))
      this.m_material.SetFloat("_GlowIntensity", 0.0f);
  }

  private void RandomJointRotation()
  {
    if ((Object) this.m_SourceJount != (Object) null)
      this.m_SourceJount.Rotate(Vector3.Lerp(this.m_SourceMinRotation, this.m_SourceMaxRotation, Random.value));
    if (!((Object) this.m_TargetJoint != (Object) null))
      return;
    this.m_TargetJoint.Rotate(Vector3.Lerp(this.m_TargetMinRotation, this.m_TargetMaxRotation, Random.value));
  }
}
