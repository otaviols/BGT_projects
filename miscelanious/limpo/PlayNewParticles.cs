using UnityEngine;

public class PlayNewParticles : MonoBehaviour
{
  public GameObject m_Target;
  public GameObject m_Target2;
  public GameObject m_Target3;
  public GameObject m_Target4;

  public void PlayNewParticles3()
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    this.m_Target.GetComponent<ParticleSystem>().Play();
  }

  public void StopNewParticles3()
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    this.m_Target.GetComponent<ParticleSystem>().Stop();
  }

  public void PlayNewParticles3andChilds()
  {
    if ((Object) this.m_Target2 == (Object) null)
      return;
    this.m_Target2.GetComponent<ParticleSystem>().Play(true);
  }

  public void StopNewParticles3andChilds()
  {
    if ((Object) this.m_Target2 == (Object) null)
      return;
    this.m_Target2.GetComponent<ParticleSystem>().Stop(true);
  }

  public void PlayNewParticles3andChilds2()
  {
    if ((Object) this.m_Target3 == (Object) null)
      return;
    this.m_Target3.GetComponent<ParticleSystem>().Play(true);
  }

  public void StopNewParticles3andChilds2()
  {
    if ((Object) this.m_Target3 == (Object) null)
      return;
    this.m_Target3.GetComponent<ParticleSystem>().Stop(true);
  }

  public void PlayNewParticles3andChilds3()
  {
    if ((Object) this.m_Target4 == (Object) null)
      return;
    this.m_Target4.GetComponent<ParticleSystem>().Play(true);
  }

  public void StopNewParticles3andChilds3()
  {
    if ((Object) this.m_Target4 == (Object) null)
      return;
    this.m_Target4.GetComponent<ParticleSystem>().Stop(true);
  }
}
