using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActorStateAnimObject
{
  public bool m_Enabled = true;
  public GameObject m_GameObject;
  public AnimationClip m_AnimClip;
  public int m_AnimLayer;
  public float m_CrossFadeSec;
  public bool m_EmitParticles;
  public string m_Comment;
  private bool m_prevParticleEmitValue;

  public void Init()
  {
    if ((UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null || (UnityEngine.Object) this.m_AnimClip == (UnityEngine.Object) null)
      return;
    string name = this.m_AnimClip.name;
    Animation component;
    if (!this.m_GameObject.TryGetComponent<Animation>(out component))
      component = this.m_GameObject.AddComponent<Animation>();
    component.playAutomatically = false;
    if ((TrackedReference) component[name] == (TrackedReference) null)
      component.AddClip(this.m_AnimClip, name);
    component[name].layer = this.m_AnimLayer;
  }

  public void Play()
  {
    if (!this.m_Enabled || (UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null || !((UnityEngine.Object) this.m_AnimClip != (UnityEngine.Object) null))
      return;
    string name = this.m_AnimClip.name;
    Animation component = this.m_GameObject.GetComponent<Animation>();
    component[name].enabled = true;
    if (Mathf.Approximately(this.m_CrossFadeSec, 0.0f))
    {
      if (component.Play(name))
        return;
      Debug.LogWarning((object) string.Format("ActorStateAnimObject.PlayNow() - FAILED to play clip {0} on {1}", (object) name, (object) this.m_GameObject));
    }
    else
      component.CrossFade(name, this.m_CrossFadeSec);
  }

  public void Stop()
  {
    if (!this.m_Enabled || (UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null || !((UnityEngine.Object) this.m_AnimClip != (UnityEngine.Object) null))
      return;
    Animation component = this.m_GameObject.GetComponent<Animation>();
    component[this.m_AnimClip.name].time = 0.0f;
    component.Sample();
    component[this.m_AnimClip.name].enabled = false;
  }

  public void Stop(List<ActorState> nextStateList)
  {
    if (!this.m_Enabled || (UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) null || !((UnityEngine.Object) this.m_AnimClip != (UnityEngine.Object) null))
      return;
    bool flag = false;
    for (int index1 = 0; !flag && index1 < nextStateList.Count; ++index1)
    {
      ActorState nextState = nextStateList[index1];
      for (int index2 = 0; index2 < nextState.m_ExternalAnimatedObjects.Count; ++index2)
      {
        ActorStateAnimObject externalAnimatedObject = nextState.m_ExternalAnimatedObjects[index2];
        if ((UnityEngine.Object) this.m_GameObject == (UnityEngine.Object) externalAnimatedObject.m_GameObject && this.m_AnimLayer == externalAnimatedObject.m_AnimLayer)
        {
          flag = true;
          break;
        }
      }
    }
    if (flag)
      return;
    this.m_GameObject.GetComponent<Animation>().Stop(this.m_AnimClip.name);
  }
}
