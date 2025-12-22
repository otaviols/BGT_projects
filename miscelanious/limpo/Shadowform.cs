using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using UnityEngine;

public class Shadowform : SuperSpell
{
  public Material m_ShadowformMaterial;
  public int m_MaterialIndex = 1;
  public float m_FadeInTime = 1f;
  public float m_Desaturate = 0.8f;
  public Color m_Tint = new Color(177f / 256f, 21f / 64f, 103f / 128f, 1f);
  public float m_Contrast = -0.29f;
  public float m_Intensity = 0.85f;
  public float m_FxIntensity = 4f;
  private Material m_MaterialInstance;
  private Material m_OriginalMaterial;
  private Coroutine m_StartFXCoroutine;

  protected override void OnBirth(SpellStateType prevStateType)
  {
    base.OnBirth(prevStateType);
    this.OnSpellFinished();
    if ((UnityEngine.Object) this.m_ShadowformMaterial == (UnityEngine.Object) null)
      return;
    if (this.m_StartFXCoroutine != null)
      this.StopCoroutine(this.m_StartFXCoroutine);
    this.m_StartFXCoroutine = this.StartCoroutine(this.StartShadowformFX());
  }

  private IEnumerator StartShadowformFX()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    Shadowform start = this;
    Actor actor;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      actor.SetShadowform(true);
      start.m_MaterialInstance = new Material(start.m_ShadowformMaterial);
      Texture staticPortraitTexture = actor.GetStaticPortraitTexture();
      start.m_MaterialInstance.mainTexture = staticPortraitTexture;
      actor.SetPortraitMaterial(start.m_MaterialInstance);
      Material mat = actor.GetPortraitMesh().GetComponent<Renderer>().GetMaterial(actor.m_portraitMatIdx);
      Action<object> action1 = (Action<object>) (desat => mat.SetFloat("_Desaturate", (float) desat));
      object[] objArray1 = new object[10]
      {
        (object) "time",
        (object) start.m_FadeInTime,
        (object) "from",
        (object) 0.0f,
        (object) "to",
        (object) start.m_Desaturate,
        (object) "onupdate",
        (object) action1,
        (object) "onupdatetarget",
        (object) actor.gameObject
      };
      iTween.ValueTo(actor.gameObject, iTween.Hash(objArray1));
      Action<object> action2 = (Action<object>) (col => mat.SetColor("_Color", (Color) col));
      object[] objArray2 = new object[10]
      {
        (object) "time",
        (object) start.m_FadeInTime,
        (object) "from",
        (object) Color.white,
        (object) "to",
        (object) start.m_Tint,
        (object) "onupdate",
        (object) action2,
        (object) "onupdatetarget",
        (object) actor.gameObject
      };
      iTween.ValueTo(actor.gameObject, iTween.Hash(objArray2));
      Action<object> action3 = (Action<object>) (desat => mat.SetFloat("_Contrast", (float) desat));
      object[] objArray3 = new object[10]
      {
        (object) "time",
        (object) start.m_FadeInTime,
        (object) "from",
        (object) 0.0f,
        (object) "to",
        (object) start.m_Contrast,
        (object) "onupdate",
        (object) action3,
        (object) "onupdatetarget",
        (object) actor.gameObject
      };
      iTween.ValueTo(actor.gameObject, iTween.Hash(objArray3));
      Action<object> action4 = (Action<object>) (desat => mat.SetFloat("_Intensity", (float) desat));
      object[] objArray4 = new object[10]
      {
        (object) "time",
        (object) start.m_FadeInTime,
        (object) "from",
        (object) 1f,
        (object) "to",
        (object) start.m_Intensity,
        (object) "onupdate",
        (object) action4,
        (object) "onupdatetarget",
        (object) actor.gameObject
      };
      iTween.ValueTo(actor.gameObject, iTween.Hash(objArray4));
      Action<object> action5 = (Action<object>) (desat => mat.SetFloat("_FxIntensity", (float) desat));
      object[] objArray5 = new object[10]
      {
        (object) "time",
        (object) start.m_FadeInTime,
        (object) "from",
        (object) 0.0f,
        (object) "to",
        (object) start.m_FxIntensity,
        (object) "onupdate",
        (object) action5,
        (object) "onupdatetarget",
        (object) actor.gameObject
      };
      iTween.ValueTo(actor.gameObject, iTween.Hash(objArray5));
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    actor = GameObjectUtils.FindComponentInThisOrParents<Actor>((Component) start);
    start.m_OriginalMaterial = actor.GetPortraitMaterial();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override void OnDeath(SpellStateType prevStateType)
  {
    base.OnDeath(prevStateType);
    if (this.m_StartFXCoroutine != null)
      this.StopCoroutine(this.m_StartFXCoroutine);
    Actor componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<Actor>((Component) this);
    componentInThisOrParents.SetShadowform(false);
    componentInThisOrParents.UpdateAllComponents();
    componentInThisOrParents.SetPortraitMaterial(this.m_OriginalMaterial);
  }
}
