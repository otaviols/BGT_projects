using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class SelfAnimUtils : MonoBehaviour
{
  public void PrintLog(string message) => Debug.Log((object) message);

  public void PrintLogWarning(string message) => Debug.LogWarning((object) message);

  public void PrintLogError(string message) => Debug.LogError((object) message);

  public void PlayAnimation()
  {
    Animation component;
    if (!this.TryGetComponent<Animation>(out component))
      return;
    component.Play();
  }

  public void StopAnimation()
  {
    Animation component;
    if (!this.TryGetComponent<Animation>(out component))
      return;
    component.Stop();
  }

  public void ActivateHierarchy() => this.gameObject.SetActive(true);

  public void DeactivateHierarchy() => this.gameObject.SetActive(false);

  public void DestroyHierarchy() => Object.Destroy((Object) this.gameObject);

  public void FadeIn(float FadeSec) => iTween.FadeTo(this.gameObject, 1f, FadeSec);

  public void FadeOut(float FadeSec) => iTween.FadeTo(this.gameObject, 0.0f, FadeSec);

  public void SetAlphaHierarchy(float alpha)
  {
    foreach (Renderer componentsInChild in this.GetComponentsInChildren<Renderer>())
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
    AudioSource component;
    if (!this.TryGetComponent<AudioSource>(out component))
      Debug.LogError((object) string.Format("SelfAnimUtils.PlayDefaultSound() - Tried to play the AudioSource on {0} but it has no AudioSource. You need an AudioSource to use this function.", (object) this.gameObject));
    else if (SoundManager.Get() == null)
      component.Play();
    else
      SoundManager.Get().Play(component);
  }

  public void PlaySound(SoundDef clip)
  {
    if ((Object) clip == (Object) null)
    {
      Debug.LogError((object) string.Format("SelfAnimUtils.PlayDefaultSound() - No clip was given when trying to play the AudioSource on {0}. You need a clip to use this function.", (object) this.gameObject));
    }
    else
    {
      AudioSource component;
      if (!this.TryGetComponent<AudioSource>(out component))
        Debug.LogError((object) string.Format("SelfAnimUtils.PlayDefaultSound() - Tried to play clip {0} on {1} but it has no AudioSource. You need an AudioSource to use this function.", (object) clip, (object) this.gameObject));
      else if (SoundManager.Get() == null)
        Debug.LogErrorFormat("TargetAnimutils2: SoundManager is null attempting to play {0}", (object) clip.m_AudioClip);
      else
        SoundManager.Get().PlayOneShot(component, clip);
    }
  }

  public void RandomRotationX() => TransformUtil.SetEulerAngleX(this.gameObject, Random.Range(0.0f, 360f));

  public void RandomRotationY() => TransformUtil.SetEulerAngleY(this.gameObject, Random.Range(0.0f, 360f));

  public void RandomRotationZ() => TransformUtil.SetEulerAngleZ(this.gameObject, Random.Range(0.0f, 360f));
}
