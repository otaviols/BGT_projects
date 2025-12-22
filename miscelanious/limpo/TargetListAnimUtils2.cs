using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class TargetListAnimUtils2 : MonoBehaviour
{
  public List<GameObject> m_TargetList;

  public void PlayAnimationList2()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        target.GetComponent<Animation>().Play();
    }
  }

  public void StopAnimationList2()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        target.GetComponent<Animation>().Stop();
    }
  }

  public void PlayAnimationListInChildren2()
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

  public void StopAnimationListInChildren2()
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

  public void ActivateHierarchyList2()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        target.SetActive(true);
    }
  }

  public void DeactivateHierarchyList2()
  {
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        target.SetActive(false);
    }
  }

  public void DestroyHierarchyList2()
  {
    foreach (Object target in this.m_TargetList)
      Object.Destroy(target);
  }

  public void FadeInList2(float FadeSec)
  {
    foreach (GameObject target in this.m_TargetList)
      iTween.FadeTo(target, 1f, FadeSec);
  }

  public void FadeOutList2(float FadeSec)
  {
    foreach (GameObject target in this.m_TargetList)
      iTween.FadeTo(target, 0.0f, FadeSec);
  }

  public void SetAlphaHierarchyList2(float alpha)
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
