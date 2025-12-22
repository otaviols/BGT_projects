using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellStateAnimObject
{
  public GameObject m_GameObject;
  public SpellStateAnimObject.Target m_Target;
  public AnimationClip m_AnimClip;
  public int m_AnimLayer;
  public float m_AnimSpeed = 1f;
  public float m_CrossFadeSec;
  public bool m_ControlParticles;
  public bool m_EmitParticles;
  public string m_Comment;
  public bool m_Enabled = true;
  private bool m_prevParticleEmitValue;

  public void Init()
  {
    if ((UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null || (UnityEngine.Object) this.m_AnimClip == (UnityEngine.Object) null)
      return;
    this.SetupAnimation();
  }

  private void SetupAnimation()
  {
    string name = this.m_AnimClip.name;
    Animation component;
    if (!this.m_GameObject.TryGetComponent<Animation>(out component))
      component = this.m_GameObject.AddComponent<Animation>();
    component.playAutomatically = false;
    if ((TrackedReference) component[name] == (TrackedReference) null)
      component.AddClip(this.m_AnimClip, name);
    component[name].layer = this.m_AnimLayer;
  }

  public void OnLoad(SpellState state)
  {
    if (this.m_Target == SpellStateAnimObject.Target.AS_SPECIFIED)
    {
      if (!((UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null))
        return;
      Debug.LogError((object) "Error: spell state anim target has a null game object after load");
    }
    else if (this.m_Target == SpellStateAnimObject.Target.ACTOR)
    {
      Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) state.transform);
      if ((UnityEngine.Object) componentInParents == (UnityEngine.Object) null || (UnityEngine.Object) componentInParents.gameObject == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Error: spell state anim target has a null game object after load");
      }
      else
      {
        this.m_GameObject = componentInParents.gameObject;
        this.SetupAnimation();
      }
    }
    else if (this.m_Target == SpellStateAnimObject.Target.ROOT_OBJECT)
    {
      Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) state.transform);
      if ((UnityEngine.Object) componentInParents == (UnityEngine.Object) null || (UnityEngine.Object) componentInParents.gameObject == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Error: spell state anim target has a null game object after load");
      }
      else
      {
        this.m_GameObject = componentInParents.GetRootObject();
        this.SetupAnimation();
      }
    }
    else
      Debug.LogWarning((object) "Error: unimplemented spell anim target");
  }

  public void Play()
  {
    if (!this.m_Enabled || (UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) this.m_AnimClip != (UnityEngine.Object) null)
    {
      Animation component = this.m_GameObject.GetComponent<Animation>();
      string name = this.m_AnimClip.name;
      AnimationState animationState = component[name];
      animationState.enabled = true;
      animationState.speed = this.m_AnimSpeed;
      if (Mathf.Approximately(this.m_CrossFadeSec, 0.0f))
      {
        if (!component.Play(name))
          Debug.LogWarning((object) string.Format("SpellStateAnimObject.PlayNow() - FAILED to play clip {0} on {1}", (object) name, (object) this.m_GameObject));
      }
      else
        component.CrossFade(name, this.m_CrossFadeSec);
    }
    if (!this.m_ControlParticles)
      return;
    ParticleSystem component1 = this.m_GameObject.GetComponent<ParticleSystem>();
    if (!((UnityEngine.Object) component1 != (UnityEngine.Object) null))
      return;
    component1.Play();
  }

  public void Stop()
  {
    if (!this.m_Enabled || (UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) this.m_AnimClip != (UnityEngine.Object) null)
      this.m_GameObject.GetComponent<Animation>().Stop(this.m_AnimClip.name);
    if (!this.m_ControlParticles)
      return;
    ParticleSystem component = this.m_GameObject.GetComponent<ParticleSystem>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.Stop();
  }

  public void Stop(List<SpellState> nextStateList)
  {
    if ((UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) this.m_AnimClip != (UnityEngine.Object) null)
    {
      bool flag = false;
      for (int index1 = 0; !flag && index1 < nextStateList.Count; ++index1)
      {
        SpellState nextState = nextStateList[index1];
        for (int index2 = 0; index2 < nextState.m_ExternalAnimatedObjects.Count; ++index2)
        {
          SpellStateAnimObject externalAnimatedObject = nextState.m_ExternalAnimatedObjects[index2];
          if (externalAnimatedObject.m_Enabled && (UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) externalAnimatedObject.m_GameObject && this.m_AnimLayer == externalAnimatedObject.m_AnimLayer)
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        this.m_GameObject.GetComponent<Animation>().Stop(this.m_AnimClip.name);
    }
    if (!this.m_ControlParticles)
      return;
    ParticleSystem component = this.m_GameObject.GetComponent<ParticleSystem>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.Stop();
  }

  public enum Target
  {
    AS_SPECIFIED,
    ACTOR,
    ROOT_OBJECT,
  }
}
