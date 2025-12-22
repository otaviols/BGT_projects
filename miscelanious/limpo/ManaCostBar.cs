using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCostBar : MonoBehaviour
{
  public GameObject m_manaCostBarObject;
  public GameObject m_ParticleObject;
  public GameObject m_ParticleStart;
  public GameObject m_ParticleEnd;
  public GameObject m_ParticleImpact;
  public float m_maxValue = 10f;
  public float m_BarIntensity = 1.6f;
  public float m_maxIntensity = 2f;
  public float m_increaseAnimTime = 2f;
  public float m_coolDownAnimTime = 1f;
  private float m_previousVal;
  private float m_currentVal;
  private float m_factor;
  private Vector3 m_particleStartPoint = Vector3.zero;
  private Vector3 m_particleEndPoint = Vector3.zero;
  private Material m_barMaterial;
  private ParticleSystem m_particleImpactSystem;
  private List<ParticleSystem> m_allParticleSystems;

  private void Start()
  {
    if ((Object) this.m_manaCostBarObject == (Object) null)
      this.enabled = false;
    if ((Object) this.m_ParticleStart != (Object) null)
      this.m_particleStartPoint = this.m_ParticleStart.transform.localPosition;
    if ((Object) this.m_ParticleEnd != (Object) null)
      this.m_particleEndPoint = this.m_ParticleEnd.transform.localPosition;
    this.m_barMaterial = this.m_manaCostBarObject.GetComponent<Renderer>().GetMaterial();
    this.m_barMaterial.SetFloat("_Seed", Random.Range(0.0f, 1f));
    if (!((Object) this.m_ParticleImpact != (Object) null))
      return;
    this.m_particleImpactSystem = this.m_ParticleImpact.GetComponent<ParticleSystem>();
  }

  public void SetBar(float newValue)
  {
    this.m_currentVal = newValue / this.m_maxValue;
    this.SetBarValue(this.m_currentVal);
    this.m_previousVal = this.m_currentVal;
  }

  public void AnimateBar(float newValue)
  {
    if ((double) newValue == 0.0)
    {
      this.SetBarValue(0.0f);
    }
    else
    {
      this.m_currentVal = newValue / this.m_maxValue;
      if ((Object) this.m_manaCostBarObject == (Object) null || (double) this.m_currentVal == (double) this.m_previousVal)
        return;
      this.m_factor = (double) this.m_currentVal <= (double) this.m_previousVal ? this.m_previousVal - this.m_currentVal : this.m_currentVal - this.m_previousVal;
      this.m_factor = Mathf.Abs(this.m_factor);
      if ((double) this.m_currentVal > (double) this.m_previousVal)
        this.IncreaseBar(this.m_currentVal, this.m_previousVal);
      else
        this.DecreaseBar(this.m_currentVal, this.m_previousVal);
      this.m_previousVal = this.m_currentVal;
    }
  }

  private void SetBarValue(float val)
  {
    this.m_currentVal = val / this.m_maxValue;
    if ((Object) this.m_manaCostBarObject == (Object) null || (double) this.m_currentVal == (double) this.m_previousVal)
      return;
    this.BarPercent_OnUpdate(val);
    this.ParticlePosition_OnUpdate(val);
    if ((double) val == 0.0)
      this.PlayParticles(false);
    this.m_previousVal = this.m_currentVal;
  }

  private void IncreaseBar(float newVal, float prevVal)
  {
    float num = this.m_increaseAnimTime * this.m_factor;
    this.PlayParticles(true);
    iTween.EaseType easeType = iTween.EaseType.easeInQuad;
    Hashtable args1 = iTween.Hash((object) "from", (object) prevVal, (object) "to", (object) newVal, (object) "time", (object) num, (object) "easetype", (object) easeType, (object) "onupdate", (object) "BarPercent_OnUpdate", (object) "oncomplete", (object) "Increase_OnComplete", (object) "oncompletetarget", (object) this.gameObject, (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "IncreaseBarPercent");
    iTween.StopByName(this.m_manaCostBarObject.gameObject, "IncreaseBarPercent");
    iTween.ValueTo(this.m_manaCostBarObject.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "from", (object) prevVal, (object) "to", (object) newVal, (object) "time", (object) num, (object) "easetype", (object) easeType, (object) "onupdate", (object) "ParticlePosition_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "ParticlePos");
    iTween.StopByName(this.m_manaCostBarObject.gameObject, "ParticlePos");
    iTween.ValueTo(this.m_manaCostBarObject.gameObject, args2);
    Hashtable args3 = iTween.Hash((object) "from", (object) this.m_BarIntensity, (object) "to", (object) this.m_maxIntensity, (object) "time", (object) num, (object) "easetype", (object) easeType, (object) "onupdate", (object) "Intensity_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "Intensity");
    iTween.StopByName(this.m_manaCostBarObject.gameObject, "Intensity");
    iTween.ValueTo(this.m_manaCostBarObject.gameObject, args3);
  }

  private void DecreaseBar(float newVal, float prevVal)
  {
    float num = this.m_increaseAnimTime * this.m_factor;
    this.PlayParticles(true);
    iTween.EaseType easeType = iTween.EaseType.easeOutQuad;
    Hashtable args1 = iTween.Hash((object) "from", (object) prevVal, (object) "to", (object) newVal, (object) "time", (object) num, (object) "easetype", (object) easeType, (object) "onupdate", (object) "BarPercent_OnUpdate", (object) "oncomplete", (object) "Decrease_OnComplete", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "IncreaseBarPercent");
    iTween.StopByName(this.m_manaCostBarObject.gameObject, "IncreaseBarPercent");
    iTween.ValueTo(this.m_manaCostBarObject.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "from", (object) prevVal, (object) "to", (object) newVal, (object) "time", (object) num, (object) "easetype", (object) easeType, (object) "onupdate", (object) "ParticlePosition_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "ParticlePos");
    iTween.StopByName(this.m_manaCostBarObject.gameObject, "ParticlePos");
    iTween.ValueTo(this.m_manaCostBarObject.gameObject, args2);
  }

  private void BarPercent_OnUpdate(float val) => this.m_barMaterial.SetFloat("_Percent", val);

  private void ParticlePosition_OnUpdate(float val) => this.m_ParticleObject.transform.localPosition = Vector3.Lerp(this.m_particleStartPoint, this.m_particleEndPoint, val);

  private void Intensity_OnUpdate(float val) => this.m_barMaterial.SetFloat("_Intensity", val);

  private void Increase_OnComplete()
  {
    if ((Object) this.m_ParticleImpact != (Object) null)
      this.m_ParticleImpact.GetComponent<ParticleSystem>().Play();
    this.CoolDown();
  }

  private void CoolDown()
  {
    Hashtable args = iTween.Hash((object) "from", (object) this.m_maxIntensity, (object) "to", (object) this.m_BarIntensity, (object) "time", (object) this.m_coolDownAnimTime, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "onupdate", (object) "Intensity_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "CoolDownIntensity", (object) "oncomplete", (object) "CoolDown_OnComplete", (object) "oncompletetarget", (object) this.gameObject);
    iTween.StopByName(this.m_manaCostBarObject.gameObject, "CoolDownIntensity");
    iTween.ValueTo(this.m_manaCostBarObject.gameObject, args);
  }

  private void CoolDown_OnComplete() => iTween.StopByName(this.m_manaCostBarObject.gameObject, "CoolDownIntensity");

  private void PlayParticles(bool state)
  {
    if (this.m_allParticleSystems == null)
    {
      this.m_allParticleSystems = new List<ParticleSystem>();
      this.m_ParticleObject.GetComponentsInChildren<ParticleSystem>(this.m_allParticleSystems);
    }
    foreach (ParticleSystem allParticleSystem in this.m_allParticleSystems)
    {
      if (state && (Object) allParticleSystem != (Object) this.m_particleImpactSystem)
        allParticleSystem.Play();
      else
        allParticleSystem.Stop();
      allParticleSystem.emission.enabled = state;
    }
  }

  private void OnDestroy() => this.m_allParticleSystems = (List<ParticleSystem>) null;
}
