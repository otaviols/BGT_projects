using System.Collections;
using UnityEngine;

public class HeroWeld : MonoBehaviour
{
  private Light[] m_lights;
  public AudioSource m_weldInSound;

  public void DoAnim()
  {
    AudioSource component1 = this.m_weldInSound.GetComponent<AudioSource>();
    if (SoundManager.Get() == null)
      component1.Play();
    else
      SoundManager.Get().Play(component1);
    this.gameObject.SetActive(true);
    this.m_lights = this.gameObject.GetComponentsInChildren<Light>();
    foreach (Behaviour light in this.m_lights)
      light.enabled = true;
    string str = "HeroWeldIn";
    Animation component2 = this.gameObject.GetComponent<Animation>();
    component2.Stop(str);
    component2.Play(str);
    this.StartCoroutine(this.DestroyWhenFinished());
  }

  private IEnumerator DestroyWhenFinished()
  {
    HeroWeld heroWeld = this;
    yield return (object) new WaitForSeconds(5f);
    foreach (Behaviour light in heroWeld.m_lights)
      light.enabled = false;
    Object.Destroy((Object) heroWeld.gameObject);
  }
}
