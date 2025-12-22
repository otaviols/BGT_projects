using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class TargetAnimUtils : MonoBehaviour
{
  public GameObject m_Target;

  private void Awake()
  {
    if (!((Object) this.m_Target == (Object) null))
      return;
    this.enabled = false;
  }

  public void PrintLog(string message) => Debug.Log((object) message);

  public void PrintLogWarning(string message) => Debug.LogWarning((object) message);

  public void PrintLogError(string message) => Debug.LogError((object) message);

  public void PlayNewParticles() => this.m_Target.GetComponent<ParticleSystem>().Play();

  public void StopNewParticles()
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    this.m_Target.GetComponent<ParticleSystem>().Stop();
  }

  public void PlayAnimation()
  {
    Animation component;
    if ((Object) this.m_Target == (Object) null || !this.m_Target.TryGetComponent<Animation>(out component))
      return;
    component.Play();
  }

  public void StopAnimation()
  {
    Animation component;
    if ((Object) this.m_Target == (Object) null || !this.m_Target.TryGetComponent<Animation>(out component))
      return;
    component.Stop();
  }

  public void PlayAnimationsInChildren()
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    foreach (Animation componentsInChild in this.m_Target.GetComponentsInChildren<Animation>())
      componentsInChild.Play();
  }

  public void StopAnimationsInChildren()
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    foreach (Animation componentsInChild in this.m_Target.GetComponentsInChildren<Animation>())
      componentsInChild.Stop();
  }

  public void ActivateHierarchy() => this.m_Target.SetActive(true);

  public void DeactivateHierarchy()
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    this.m_Target.SetActive(false);
  }

  public void DestroyHierarchy()
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    Object.Destroy((Object) this.m_Target);
  }

  public void FadeIn(float FadeSec)
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    iTween.FadeTo(this.m_Target, 1f, FadeSec);
  }

  public void FadeOut(float FadeSec)
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    iTween.FadeTo(this.m_Target, 0.0f, FadeSec);
  }

  public void SetAlphaHierarchy(float alpha)
  {
    if ((Object) this.m_Target == (Object) null)
      return;
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

  public void PlayDefaultSound()
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    AudioSource component;
    if (!this.m_Target.TryGetComponent<AudioSource>(out component))
      Debug.LogError((object) string.Format("TargetAnimUtils.PlayDefaultSound() - Tried to play the AudioSource on {0} but it has no AudioSource. You need an AudioSource to use this function.", (object) this.m_Target));
    else if (SoundManager.Get() == null)
      component.Play();
    else
      SoundManager.Get().Play(component);
  }

  public void PlaySound(SoundDef clip)
  {
    if ((Object) this.m_Target == (Object) null)
      return;
    if ((Object) clip == (Object) null)
    {
      Debug.LogError((object) string.Format("TargetAnimUtils.PlayDefaultSound() - No clip was given when trying to play the AudioSource on {0}. You need a clip to use this function.", (object) this.m_Target));
    }
    else
    {
      AudioSource component;
      if (!this.m_Target.TryGetComponent<AudioSource>(out component))
        Debug.LogError((object) string.Format("TargetAnimUtils.PlayDefaultSound() - Tried to play clip {0} on {1} but it has no AudioSource. You need an AudioSource to use this function.", (object) clip, (object) this.m_Target));
      else if (SoundManager.Get() == null)
        Debug.LogErrorFormat("TargetAnimutils2: SoundManager is null attempting to play {0}", (object) clip.m_AudioClip);
      else
        SoundManager.Get().PlayOneShot(component, clip);
    }
  }
}
