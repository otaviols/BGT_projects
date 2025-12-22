using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class AssignActorPortraitToChildren : MonoBehaviour
{
  private Actor m_Actor;

  private void Start() => this.m_Actor = GameObjectUtils.FindComponentInThisOrParents<Actor>(this.gameObject);

  public void AssignPortraitToAllChildren()
  {
    if (!(bool) (Object) this.m_Actor || (Object) this.m_Actor.m_portraitMesh == (Object) null)
      return;
    Texture actorPortraitTexture = this.m_Actor.GetCard().GetPreferredActorPortraitTexture();
    if ((Object) actorPortraitTexture == (Object) null)
    {
      Debug.LogWarning((object) string.Format("AssignPortraitToAllChildren could not find a preferred portrait for {0}", (object) this.m_Actor));
    }
    else
    {
      foreach (Renderer componentsInChild in this.GetComponentsInChildren<Renderer>())
      {
        foreach (Material material in componentsInChild.GetMaterials())
        {
          if (material.name.Contains("portrait"))
            material.mainTexture = actorPortraitTexture;
        }
      }
    }
  }
}
