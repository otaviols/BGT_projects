using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class SpellImpl : Spell
{
  protected Actor m_actor;
  protected GameObject m_rootObject;
  protected MeshRenderer m_rootObjectRenderer;
  private static List<Renderer> s_cachedRenderers;

  protected void InitActorVariables()
  {
    this.m_actor = SpellUtils.GetParentActor((Spell) this);
    this.m_rootObject = SpellUtils.GetParentRootObject((Spell) this);
    this.m_rootObjectRenderer = SpellUtils.GetParentRootObjectMesh((Spell) this);
  }

  protected void SetActorVisibility(bool visible, bool ignoreSpells)
  {
    if (!((Object) this.m_actor != (Object) null))
      return;
    if (visible)
      this.m_actor.Show(ignoreSpells);
    else
      this.m_actor.Hide(ignoreSpells);
  }

  protected void SetVisibility(GameObject go, bool visible) => go.GetComponent<Renderer>().enabled = visible;

  protected void SetVisibilityRecursive(GameObject go, bool visible)
  {
    if ((Object) go == (Object) null)
      return;
    if (SpellImpl.s_cachedRenderers == null)
      SpellImpl.s_cachedRenderers = new List<Renderer>();
    go.GetComponentsInChildren<Renderer>(SpellImpl.s_cachedRenderers);
    foreach (Renderer cachedRenderer in SpellImpl.s_cachedRenderers)
      cachedRenderer.enabled = visible;
  }

  protected void SetAnimationSpeed(GameObject go, string animName, float speed)
  {
    if ((Object) go == (Object) null)
      return;
    go.GetComponent<Animation>()[animName].speed = speed;
  }

  protected void SetAnimationTime(GameObject go, string animName, float time)
  {
    if ((Object) go == (Object) null)
      return;
    go.GetComponent<Animation>()[animName].time = time;
  }

  protected void PlayAnimation(GameObject go, string animName, PlayMode playMode, float crossFade = 0.0f)
  {
    if ((Object) go == (Object) null)
      return;
    Animation component = go.GetComponent<Animation>();
    if ((double) crossFade <= (double) Mathf.Epsilon)
      component.Play(animName, playMode);
    else
      component.CrossFade(animName, crossFade, playMode);
  }

  protected void PlayParticles(GameObject go, bool includeChildren)
  {
    if ((Object) go == (Object) null)
      return;
    go.GetComponent<ParticleSystem>().Play(includeChildren);
  }

  protected GameObject GetActorObject(string name) => (Object) this.m_actor == (Object) null ? (GameObject) null : GameObjectUtils.FindChildBySubstring(this.m_actor.gameObject, name);

  protected void SetMaterialColor(
    GameObject go,
    Material material,
    string colorName,
    Color color,
    int materialIndex = 0)
  {
    if (colorName == "")
      colorName = "_Color";
    if ((Object) material != (Object) null)
    {
      material.SetColor(colorName, color);
    }
    else
    {
      if ((Object) go == (Object) null)
        return;
      Renderer component = go.GetComponent<Renderer>();
      if ((Object) component == (Object) null)
        return;
      Material material1 = component.GetMaterial();
      if ((Object) material1 == (Object) null)
        return;
      if (materialIndex == 0)
      {
        material1.SetColor(colorName, color);
      }
      else
      {
        if (component.GetMaterials().Count <= materialIndex)
          return;
        component.GetMaterial(materialIndex).SetColor(colorName, color);
      }
    }
  }

  protected Material GetMaterial(
    GameObject go,
    Material material,
    bool getSharedMaterial = false,
    int materialIndex = 0)
  {
    if ((Object) go == (Object) null)
      return (Material) null;
    Renderer component = go.GetComponent<Renderer>();
    if ((Object) component == (Object) null)
      return (Material) null;
    if (materialIndex == 0 && !getSharedMaterial)
      return component.GetMaterial();
    if (materialIndex == 0 & getSharedMaterial)
      return component.GetSharedMaterial();
    if (component.GetMaterials().Count <= materialIndex)
      return (Material) null;
    return !getSharedMaterial ? component.GetMaterial(materialIndex) : component.GetSharedMaterial(materialIndex);
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (SpellImpl.s_cachedRenderers == null)
      return;
    SpellImpl.s_cachedRenderers.Clear();
  }
}
