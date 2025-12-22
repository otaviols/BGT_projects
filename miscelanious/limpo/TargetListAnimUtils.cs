using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class TargetListAnimUtils : MonoBehaviour
{
  public List<GameObject> m_TargetList;

  public void PlayNewParticlesListInChildren()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
      {
        foreach (ParticleSystem componentsInChild in target.GetComponentsInChildren<ParticleSystem>())
          componentsInChild.Play();
      }
    }
  }

  public void StopNewParticlesListInChildren()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
      {
        foreach (ParticleSystem componentsInChild in target.GetComponentsInChildren<ParticleSystem>())
          componentsInChild.Stop();
      }
    }
  }

  public void PlayAnimationList()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        target.GetComponent<Animation>().Play();
    }
  }

  public void StopAnimationList()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        target.GetComponent<Animation>().Stop();
    }
  }

  public void PlayAnimationListInChildren()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
      {
        foreach (Animation componentsInChild in target.GetComponentsInChildren<Animation>())
          componentsInChild.Play();
      }
    }
  }

  public void StopAnimationListInChildren()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
      {
        foreach (Animation componentsInChild in target.GetComponentsInChildren<Animation>())
          componentsInChild.Stop();
      }
    }
  }

  public void ActivateHierarchyList()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        target.SetActive(true);
    }
  }

  public void DeactivateHierarchyList()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        target.SetActive(false);
    }
  }

  public void DestroyHierarchyList()
  {
    foreach (Object target in this.m_TargetList)
      Object.Destroy(target);
  }

  public void FadeInList(float FadeSec)
  {
    foreach (GameObject target in this.m_TargetList)
      iTween.FadeTo(target, 1f, FadeSec);
  }

  public void FadeOutList(float FadeSec)
  {
    foreach (GameObject target in this.m_TargetList)
      iTween.FadeTo(target, 0.0f, FadeSec);
  }

  public void SetAlphaHierarchyList(float alpha)
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
      {
        foreach (Renderer componentsInChild in target.GetComponentsInChildren<Renderer>())
        {
          Material material = componentsInChild.GetMaterial();
          if (material.HasProperty("_Color"))
          {
            Color color = material.color with
            {
              a = alpha
            };
            material.color = color;
          }
        }
      }
    }
  }
}
