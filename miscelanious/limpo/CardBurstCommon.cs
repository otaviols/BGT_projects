using UnityEngine;

public class CardBurstCommon : Spell
{
  public ParticleSystem m_BurstMotes;
  public GameObject m_EdgeGlow;

  protected override void OnBirth(SpellStateType prevStateType)
  {
    if ((bool) (Object) this.m_BurstMotes)
      this.m_BurstMotes.Play();
    if ((bool) (Object) this.m_EdgeGlow)
      this.m_EdgeGlow.GetComponent<Renderer>().enabled = true;
    this.OnSpellFinished();
  }
}
