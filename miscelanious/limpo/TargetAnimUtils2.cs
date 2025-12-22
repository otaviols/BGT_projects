using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class TargetAnimUtils2 : MonoBehaviour
{
  public GameObject m_Target;

  public void PrintLog2(string message) => Debug.Log((object) message);

  public void PrintLogWarning2(string message) => Debug.LogWarning((object) message);

  public void PrintLogError2(string message) => Debug.LogError((object) message);

  public void PlayNewParticles2() => this.m_Target.GetComponent<ParticleSystem>().Play();

  public void StopNewParticles2() => this.m_Target.GetComponent<ParticleSystem>().Stop();

  public void PlayAnimation2()
  {
    Animation component;
    if (!this.m_Target.TryGetComponent<Animation>(out component))
      return;
    component.Play();
  }

  public void StopAnimation2()
  {
    Animation component;
    if (!this.m_Target.TryGetComponent<Animation>(out component))
      return;
    component.Stop();
  }

  public void PlayAnimationsInChildren2()
  {
    foreach (Animation componentsInChild in this.m_Target.GetComponentsInChildren<Animation>())
      componentsInChild.Play();
  }

  public void StopAnimationsInChildren2()
  {
    foreach (Animation componentsInChild in this.m_Target.GetComponentsInChildren<Animation>())
      componentsInChild.Stop();
  }

  public void ActivateHierarchy2() => this.m_Target.SetActive(true);

  public void DeactivateHierarchy2() => this.m_Target.SetActive(false);

  public void DestroyHierarchy2() => Object.Destroy((Object) this.m_Target);

  public void FadeIn2(float FadeSec) => iTween.FadeTo(this.m_Target, 1f, FadeSec);

  public void FadeOut2(float FadeSec) => iTween.FadeTo(this.m_Target, 0.0f, FadeSec);

  public void SetAlphaHierarchy2(float alpha)
  {
    foreach (Renderer componentsInChild in this.m_Target.GetComponentsInChildren<Renderer>())
    {
      Material material = componentsInChild.GetMaterial();
      if (material.HasProperty("_Color"))
      {
        Color color = material.color with { a = alpha };
        material.color = color;
      }
    }
  }

  public void PlayDefaultSound2()
  {
    AudioSource component;
    if (!this.m_Target.TryGetComponent<AudioSource>(out component))
      Debug.LogError((object) string.Format("TargetAnimUtils2.PlayDefaultSound() - Tried to play the AudioSource on {0} but it has no AudioSource. You need an AudioSource to use this function.", (object) this.m_Target));
    else if (SoundManager.Get() == null)
      component.Play();
    else
      SoundManager.Get().Play(component);
  }

  public void PlaySound2(SoundDef clip)
  {
    if ((Object) clip == (Object) null)
    {
      Debug.LogError((object) string.Format("TargetAnimUtils2.PlayDefaultSound() - No clip was given when trying to play the AudioSource on {0}. You need a clip to use this function.", (object) this.m_Target));
    }
    else
    {
      AudioSource component;
      if (!this.m_Target.TryGetComponent<AudioSource>(out component))
        Debug.LogError((object) string.Format("TargetAnimUtils2.PlayDefaultSound() - Tried to play clip {0} on {1} but it has no AudioSource. You need an AudioSource to use this function.", (object) clip, (object) this.m_Target));
      else if (SoundManager.Get() == null)
        Debug.LogErrorFormat("TargetAnimutils2: SoundManager is null attempting to play {0}", (object) clip.m_AudioClip);
      else
        SoundManager.Get().PlayOneShot(component, clip);
    }
  }
}
