using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class StorybookInkDissolverLayered : MonoBehaviour
{
  public Material TargetMaterial;
  public bool RandomOffsetCycle = true;
  public AnimationCurve DissolveAnimation;
  public float DissolveAnimationSpeed = 0.05f;
  public float DissolveAnimationTimeOffset;
  public AnimationCurve Dissolve2Animation;
  public float Dissolve2AnimationSpeed = 0.05f;
  public float Dissolve2AnimationTimeOffset = 0.333f;
  public AnimationCurve Dissolve3Animation;
  public float Dissolve3AnimationSpeed = 0.05f;
  public float Dissolve3AnimationTimeOffset = 0.666f;
  private float matDissolve;
  private float matDissolve2;
  private float matDissolve3;
  private Vector4 matDissolveST;
  private Vector4 matDissolve2ST;
  private Vector4 matDissolve3ST;
  private float dissolveOffsetTime;
  private float dissolve2OffsetTime;
  private float dissolve3OffsetTime;
  private float matDissolveStartTime;
  private float matDissolve2StartTime;
  private float matDissolve3StartTime;
  public GameObject TargetObjectWithIllustrationMaterial;
  private Material TargetMaterialIllustration;
  public bool AnimateIllustrationIntensity = true;
  public AnimationCurve IntensityAnimation;
  public float IntensityAnimationSpeed = 0.25f;
  public float IntensityAnimationValueScale;
  public float IntensityAnimationValueOffset = 1f;
  private float illusIntensity;

  private void Start() => this.DoChecks();

  private void Reset() => this.DoChecks();

  private void Update()
  {
    this.matDissolve = this.DissolveAnimation.Evaluate((Time.time - this.matDissolveStartTime) * this.DissolveAnimationSpeed);
    this.matDissolve2 = this.Dissolve2Animation.Evaluate((Time.time - this.matDissolve2StartTime) * this.Dissolve2AnimationSpeed);
    this.matDissolve3 = this.Dissolve3Animation.Evaluate((Time.time - this.matDissolve3StartTime) * this.Dissolve3AnimationSpeed);
    this.TargetMaterial.SetFloat("_Dissolve", this.matDissolve);
    this.TargetMaterial.SetFloat("_Dissolve2", this.matDissolve2);
    this.TargetMaterial.SetFloat("_Dissolve3", this.matDissolve3);
    if (this.RandomOffsetCycle)
    {
      float num1 = this.dissolveOffsetTime / this.DissolveAnimationSpeed;
      float num2 = this.dissolve2OffsetTime / this.Dissolve2AnimationSpeed;
      float num3 = this.dissolve3OffsetTime / this.Dissolve3AnimationSpeed;
      if ((double) Time.time >= (double) this.matDissolveStartTime + (double) num1)
      {
        this.matDissolveST[2] = Random.Range(0.0f, 1f);
        this.matDissolveST[3] = Random.Range(0.0f, 1f);
        this.TargetMaterial.SetVector("_DissolveTex_ST", this.matDissolveST);
        ++this.dissolveOffsetTime;
      }
      if ((double) Time.time >= (double) this.matDissolve2StartTime + (double) num2)
      {
        this.matDissolve2ST[2] = Random.Range(0.0f, 1f);
        this.matDissolve2ST[3] = Random.Range(0.0f, 1f);
        this.TargetMaterial.SetVector("_Dissolve2Mod", this.matDissolve2ST);
        ++this.dissolve2OffsetTime;
      }
      if ((double) Time.time >= (double) this.matDissolve3StartTime + (double) num3)
      {
        this.matDissolve3ST[2] = Random.Range(0.0f, 1f);
        this.matDissolve3ST[3] = Random.Range(0.0f, 1f);
        this.TargetMaterial.SetVector("_Dissolve3Mod", this.matDissolve3ST);
        ++this.dissolve3OffsetTime;
      }
    }
    if (!this.AnimateIllustrationIntensity || !(bool) (Object) this.TargetMaterialIllustration)
      return;
    this.illusIntensity = this.IntensityAnimation.Evaluate(Time.time * this.IntensityAnimationSpeed) * this.IntensityAnimationValueScale + this.IntensityAnimationValueOffset;
    this.TargetMaterialIllustration.SetFloat("_MainTexIntensity", this.illusIntensity);
  }

  private AnimationCurve MakeNewDefault()
  {
    AnimationCurve animationCurve = AnimationCurve.Linear(0.0f, 1f, 1f, -1f);
    animationCurve.preWrapMode = WrapMode.Loop;
    animationCurve.postWrapMode = WrapMode.Loop;
    return animationCurve;
  }

  private void DoChecks()
  {
    if ((Object) this.TargetMaterial == (Object) null)
      this.TargetMaterial = this.gameObject.GetComponent<Renderer>().GetMaterial();
    if ((Object) this.TargetMaterial == (Object) null)
    {
      Debug.Log((object) "StorybookInkDissolver: no target material");
    }
    else
    {
      if ((Object) this.TargetMaterialIllustration == (Object) null && (Object) this.TargetObjectWithIllustrationMaterial != (Object) null)
        this.TargetMaterialIllustration = this.TargetObjectWithIllustrationMaterial.GetComponent<Renderer>().GetMaterial();
      if ((Object) this.TargetMaterialIllustration == (Object) null)
      {
        Debug.Log((object) "StorybookInkDissolver: no target material for intensity animation");
      }
      else
      {
        if (this.DissolveAnimation == null || this.DissolveAnimation.length < 1)
        {
          this.DissolveAnimation = this.MakeNewDefault();
          this.Dissolve2Animation = this.MakeNewDefault();
          this.Dissolve3Animation = this.MakeNewDefault();
        }
        if (this.IntensityAnimation == null || this.IntensityAnimation.length < 1)
          this.IntensityAnimation = this.MakeNewDefault();
        this.matDissolveST = this.TargetMaterial.GetVector("_DissolveTex_ST");
        this.matDissolve2ST = this.TargetMaterial.GetVector("_Dissolve2Mod");
        this.matDissolve3ST = this.TargetMaterial.GetVector("_Dissolve3Mod");
        this.dissolveOffsetTime = 1f;
        this.dissolve2OffsetTime = 1f;
        this.dissolve3OffsetTime = 1f;
        this.matDissolveStartTime = Time.time - (1f - this.DissolveAnimationTimeOffset) / this.DissolveAnimationSpeed;
        this.matDissolve2StartTime = Time.time - (1f - this.Dissolve2AnimationTimeOffset) / this.Dissolve2AnimationSpeed;
        this.matDissolve3StartTime = Time.time - (1f - this.Dissolve3AnimationTimeOffset) / this.Dissolve3AnimationSpeed;
      }
    }
  }
}
