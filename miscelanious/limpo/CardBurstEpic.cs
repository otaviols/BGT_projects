using System.Collections;
using UnityEngine;

public class CardBurstEpic : Spell
{
  public GameObject m_RenderPlane;
  public GameObject m_RaysMask;
  public GameObject m_EdgeGlow;
  public string m_EdgeGlowBirthAnimation = "StandardEdgeGlowFade";
  public ParticleSystem m_BurstFlare;
  public ParticleSystem m_Bang;
  public ParticleSystem m_BangLinger;
  public string m_EdgeGlowDeathAnimation = "StandardEdgeGlowFadeOut";

  protected override void OnBirth(SpellStateType prevStateType)
  {
    if ((bool) (Object) this.m_RenderPlane)
      this.m_RenderPlane.SetActive(true);
    if ((bool) (Object) this.m_RaysMask)
      this.m_RaysMask.SetActive(true);
    if ((bool) (Object) this.m_EdgeGlow)
    {
      this.m_EdgeGlow.GetComponent<Renderer>().enabled = true;
      this.m_EdgeGlow.GetComponent<Animation>().Play(this.m_EdgeGlowBirthAnimation, PlayMode.StopAll);
    }
    if ((bool) (Object) this.m_BurstFlare)
      this.m_BurstFlare.Play();
    if ((bool) (Object) this.m_Bang)
      this.m_Bang.Play();
    if ((bool) (Object) this.m_BangLinger)
      this.m_BangLinger.Play();
    this.OnSpellFinished();
  }

  protected override void OnDeath(SpellStateType prevStateType)
  {
    if ((bool) (Object) this.m_EdgeGlow)
      this.m_EdgeGlow.GetComponent<Animation>().Play(this.m_EdgeGlowDeathAnimation, PlayMode.StopAll);
    this.StartCoroutine(this.DeathState());
  }

  private IEnumerator DeathState()
  {
    CardBurstEpic cardBurstEpic = this;
    yield return (object) new WaitForSeconds(0.2f);
    if ((bool) (Object) cardBurstEpic.m_EdgeGlow)
      cardBurstEpic.m_EdgeGlow.GetComponent<Renderer>().enabled = false;
    cardBurstEpic.OnSpellFinished();
  }
}
