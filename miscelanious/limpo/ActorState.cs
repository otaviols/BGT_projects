using Blizzard.T5.Core.Utils;
using System.Collections.Generic;
using UnityEngine;

public class ActorState : MonoBehaviour
{
  public ActorStateType m_StateType;
  public List<ActorStateAnimObject> m_ExternalAnimatedObjects;
  private ActorStateMgr m_stateMgr;
  private bool m_playing;
  private bool m_initialized;

  private void Start()
  {
    this.m_stateMgr = GameObjectUtils.FindComponentInParents<ActorStateMgr>(this.gameObject);
    foreach (ActorStateAnimObject externalAnimatedObject in this.m_ExternalAnimatedObjects)
      externalAnimatedObject.Init();
    this.m_initialized = true;
    if (!this.m_playing)
      return;
    this.gameObject.SetActive(true);
    this.PlayNow();
  }

  public void Play()
  {
    if (this.m_playing)
      return;
    this.m_playing = true;
    if (!this.m_initialized)
      return;
    this.gameObject.SetActive(true);
    this.PlayNow();
  }

  public void Stop(List<ActorState> nextStateList)
  {
    if (!this.m_playing)
      return;
    this.m_playing = false;
    if (!this.m_initialized)
      return;
    Animation component;
    if (this.TryGetComponent<Animation>(out component))
      component.Stop();
    if (nextStateList == null)
    {
      foreach (ActorStateAnimObject externalAnimatedObject in this.m_ExternalAnimatedObjects)
        externalAnimatedObject.Stop();
    }
    else
    {
      foreach (ActorStateAnimObject externalAnimatedObject in this.m_ExternalAnimatedObjects)
        externalAnimatedObject.Stop(nextStateList);
    }
    this.gameObject.SetActive(false);
  }

  public float GetAnimationDuration()
  {
    float b = 0.0f;
    for (int index = 0; index < this.m_ExternalAnimatedObjects.Count; ++index)
    {
      if ((Object) this.m_ExternalAnimatedObjects[index].m_GameObject != (Object) null)
        b = Mathf.Max(this.m_ExternalAnimatedObjects[index].m_AnimClip.length, b);
    }
    return b;
  }

  public void ShowState()
  {
    this.gameObject.SetActive(true);
    this.Play();
  }

  public void HideState()
  {
    this.Stop((List<ActorState>) null);
    this.gameObject.SetActive(false);
  }

  private void OnChangeState(ActorStateType stateType) => this.m_stateMgr.ChangeState(stateType);

  private void PlayNow()
  {
    Animation component;
    if (this.TryGetComponent<Animation>(out component))
      component.Play();
    foreach (ActorStateAnimObject externalAnimatedObject in this.m_ExternalAnimatedObjects)
      externalAnimatedObject.Play();
  }
}
