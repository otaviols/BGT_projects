using System;
using UnityEngine;

[CustomEditClass]
public class KARChemistry : MonoBehaviour
{
  public PlayMakerFSM m_Lever;
  public PlayMakerFSM m_Knob;
  public Material m_BeakerMat;
  public Material m_CurlTubeMat;
  public Material m_SmallGlobeMat;
  public Material m_HeatGlowMat;
  public ParticleSystem m_BubbleFX;
  private bool m_isLeverOn;
  private bool m_isKnobOn;

  private void Start()
  {
    this.m_HeatGlowMat.SetFloat("_Intensity", 0.0f);
    this.SetEmissionRate(this.m_BubbleFX, 0.0f);
    this.m_CurlTubeMat.SetFloat("_UVOffsetSecondX", -0.8f);
    this.m_CurlTubeMat.SetFloat("_Intensity", 0.0f);
    this.m_SmallGlobeMat.SetFloat("_Transistion", 0.0f);
    this.m_BeakerMat.SetFloat("_Transistion", 0.0f);
  }

  private void Update() => this.HandleHits();

  private void HandleHits()
  {
    if (InputCollection.GetMouseButtonUp(0) && this.IsOver(this.m_Lever.gameObject))
    {
      if (!this.m_isLeverOn)
        this.LeverOnAnimations();
      else
        this.LeverOffAnimations();
    }
    if (!InputCollection.GetMouseButtonUp(0) || !this.IsOver(this.m_Knob.gameObject))
      return;
    if (!this.m_isKnobOn)
      this.KnobOnAnimations();
    else
      this.KnobOffAnimations();
  }

  private void KnobOnAnimations()
  {
    if (this.m_Knob.FsmVariables.GetFsmBool("knobAnimating").Value)
      return;
    this.m_Knob.SendEvent("KnobTurnedOn");
    this.m_isKnobOn = true;
    iTween.Stop(this.gameObject);
    float num = this.m_HeatGlowMat.GetFloat("_Intensity");
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num, (object) "to", (object) 2f, (object) "time", (object) (float) (2.0 * ((2.0 - (double) num) / 2.0)), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_HeatGlowMat, "_Intensity", (float) newVal))));
    float emissionRate = this.GetEmissionRate(this.m_BubbleFX);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) emissionRate, (object) "to", (object) 50f, (object) "time", (object) (float) (3.0 * ((50.0 - (double) emissionRate) / 50.0)), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.BubbleRate((float) newVal))));
  }

  private void KnobOffAnimations()
  {
    if (this.m_Knob.FsmVariables.GetFsmBool("knobAnimating").Value)
      return;
    this.m_Knob.SendEvent("KnobTurnedOff");
    this.m_isKnobOn = false;
    iTween.Stop(this.gameObject);
    float num = this.m_HeatGlowMat.GetFloat("_Intensity");
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num, (object) "to", (object) 0.0f, (object) "time", (object) (float) (2.0 * ((double) num / 2.0)), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_HeatGlowMat, "_Intensity", (float) newVal))));
    float emissionRate = this.GetEmissionRate(this.m_BubbleFX);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) emissionRate, (object) "to", (object) 0.0f, (object) "time", (object) (float) (1.0 * ((double) emissionRate / 50.0)), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.BubbleRate((float) newVal))));
  }

  private void LeverOnAnimations()
  {
    if (this.m_Lever.FsmVariables.GetFsmBool("leverAnimating").Value)
      return;
    this.m_Lever.SendEvent("LeverTurnedOn");
    this.m_isLeverOn = true;
    iTween.Stop(this.gameObject);
    float num1 = this.m_CurlTubeMat.GetFloat("_Intensity");
    float num2 = this.m_CurlTubeMat.GetFloat("_UVOffsetSecondX");
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num1, (object) "to", (object) 8f, (object) "time", (object) (float) (1.0 * ((8.0 - (double) num1) / 8.0)), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_CurlTubeMat, "_Intensity", (float) newVal))));
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num2, (object) "to", (object) 0.8f, (object) "time", (object) 1f, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_CurlTubeMat, "_UVOffsetSecondX", (float) newVal))));
    float num3 = this.m_SmallGlobeMat.GetFloat("_Transistion");
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num3, (object) "to", (object) 1f, (object) "time", (object) (float) (3.0 * ((1.0 - (double) num3) / 1.0)), (object) "delay", (object) 1f, (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_SmallGlobeMat, "_Transistion", (float) newVal))));
    float num4 = this.m_BeakerMat.GetFloat("_Transistion");
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num4, (object) "to", (object) 1f, (object) "time", (object) (float) (3.0 * ((1.0 - (double) num4) / 1.0)), (object) "delay", (object) 1f, (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_BeakerMat, "_Transistion", (float) newVal))));
  }

  public float GetEmissionRate(ParticleSystem particleSystem) => particleSystem.emission.rateOverTime.constantMax;

  public void SetEmissionRate(ParticleSystem particleSystem, float emissionRate)
  {
    ParticleSystem.EmissionModule emission = particleSystem.emission;
    ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime with
    {
      constantMax = emissionRate
    };
    emission.rateOverTime = rateOverTime;
  }

  private void LeverOffAnimations(bool hasScience = true)
  {
    if (this.m_Lever.FsmVariables.GetFsmBool("leverAnimating").Value)
      return;
    if (((!this.m_isKnobOn || !this.m_isLeverOn ? 0 : ((double) this.m_BeakerMat.GetFloat("_Transistion") == 1.0 ? 1 : 0)) & (hasScience ? 1 : 0)) != 0)
    {
      this.BlindMeWithScience();
    }
    else
    {
      this.m_Lever.SendEvent("LeverTurnedOff");
      iTween.Stop(this.gameObject);
      float num1 = this.m_CurlTubeMat.GetFloat("_Intensity");
      float num2 = this.m_CurlTubeMat.GetFloat("_UVOffsetSecondX");
      iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num1, (object) "to", (object) 0.0f, (object) "time", (object) (float) (1.0 * ((double) num1 / 8.0)), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_CurlTubeMat, "_Intensity", (float) newVal))));
      iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num2, (object) "to", (object) -0.8f, (object) "time", (object) 1f, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_CurlTubeMat, "_UVOffsetSecondX", (float) newVal))));
      float num3 = this.m_SmallGlobeMat.GetFloat("_Transistion");
      iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num3, (object) "to", (object) 0.0f, (object) "time", (object) (float) (4.0 * ((double) num3 / 1.0)), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_SmallGlobeMat, "_Transistion", (float) newVal))));
      float num4 = this.m_BeakerMat.GetFloat("_Transistion");
      iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) num4, (object) "to", (object) 0.0f, (object) "time", (object) (float) (4.0 * ((double) num4 / 1.0)), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.MaterialValueTo(this.m_BeakerMat, "_Transistion", (float) newVal))));
    }
    this.m_isLeverOn = false;
  }

  private void BlindMeWithScience()
  {
    this.m_Lever.FsmVariables.GetFsmBool("doPoof").Value = true;
    this.LeverOffAnimations(false);
  }

  private void MaterialValueTo(Material mat, string property, float newVal) => mat.SetFloat(property, newVal);

  private void BubbleRate(float newVal) => this.SetEmissionRate(this.m_BubbleFX, newVal);

  private bool IsOver(GameObject go) => (bool) (UnityEngine.Object) go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
}
