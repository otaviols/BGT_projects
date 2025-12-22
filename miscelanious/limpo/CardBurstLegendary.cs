using System.Collections;
using UnityEngine;

public class CardBurstLegendary : Spell
{
  public GameObject m_RenderPlane;
  public GameObject m_RaysMask;
  public GameObject m_EdgeGlow;
  public string m_EdgeGlowBirthAnimation = "StandardEdgeGlowFade_Forge";
  public ParticleSystem m_Shockwave;
  public ParticleSystem m_Bang;
  public string m_EdgeGlowDeathAnimation = "StandardEdgeGlowFadeOut_Forge";

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
    if ((bool) (Object) this.m_Shockwave)
      this.m_Shockwave.Play();
    if ((bool) (Object) this.m_Bang)
      this.m_Bang.Play();
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
    CardBurstLegendary cardBurstLegendary = this;
    yield return (object) new WaitForSeconds(0.2f);
    if ((bool) (Object) cardBurstLegendary.m_EdgeGlow)
      cardBurstLegendary.m_EdgeGlow.GetComponent<Renderer>().enabled = false;
    cardBurstLegendary.OnSpellFinished();
  }
}
