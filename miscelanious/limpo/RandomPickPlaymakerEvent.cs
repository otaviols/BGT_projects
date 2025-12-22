using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPickPlaymakerEvent : MonoBehaviour
{
  public int m_AwakeStateIndex = -1;
  public bool m_AllowNoneState = true;
  public List<RandomPickPlaymakerEvent.PickEvent> m_State;
  public List<RandomPickPlaymakerEvent.PickEvent> m_AlternateState;
  private bool m_StateActive;
  private RandomPickPlaymakerEvent.PickEvent m_CurrentState;
  private Collider m_Collider;
  private bool m_AlternateEventState;
  private int m_LastEventIndex;
  private int m_LastAlternateIndex;
  private bool m_StartAnimationFinished = true;
  private bool m_EndAnimationFinished = true;

  private void Awake()
  {
    this.m_Collider = this.GetComponent<Collider>();
    if (this.m_AwakeStateIndex <= -1)
      return;
    this.m_CurrentState = this.m_State[this.m_AwakeStateIndex];
    this.m_LastEventIndex = this.m_AwakeStateIndex;
    this.m_StateActive = true;
  }

  public void RandomPickEvent()
  {
    if (!this.m_StartAnimationFinished || !this.m_EndAnimationFinished)
      return;
    if (this.m_StateActive && this.m_CurrentState.m_EndEvent != string.Empty && (UnityEngine.Object) this.m_CurrentState.m_FSM != (UnityEngine.Object) null)
    {
      this.m_CurrentState.m_FSM.SendEvent(this.m_CurrentState.m_EndEvent);
      this.m_EndAnimationFinished = false;
      this.m_StateActive = false;
      this.StartCoroutine(this.WaitForEndAnimation());
    }
    else if (this.m_AlternateState.Count > 0)
    {
      if (this.m_AlternateEventState)
        this.SendRandomEvent();
      else
        this.SendAlternateRandomEvent();
    }
    else
      this.SendRandomEvent();
  }

  public void StartAnimationFinished() => this.m_StartAnimationFinished = true;

  public void EndAnimationFinished() => this.m_EndAnimationFinished = true;

  private void SendRandomEvent()
  {
    this.m_StateActive = true;
    this.m_AlternateEventState = false;
    List<int> intList = new List<int>();
    if (this.m_State.Count == 1)
    {
      intList.Add(0);
    }
    else
    {
      for (int index = 0; index < this.m_State.Count; ++index)
      {
        if (index != this.m_LastEventIndex)
          intList.Add(index);
      }
    }
    int index1 = UnityEngine.Random.Range(0, intList.Count);
    RandomPickPlaymakerEvent.PickEvent pickEvent = this.m_State[intList[index1]];
    this.m_CurrentState = pickEvent;
    this.m_LastEventIndex = intList[index1];
    this.m_StartAnimationFinished = false;
    this.StartCoroutine(this.WaitForStartAnimation());
    pickEvent.m_FSM.SendEvent(pickEvent.m_StartEvent);
  }

  private void SendAlternateRandomEvent()
  {
    this.m_StateActive = true;
    this.m_AlternateEventState = true;
    List<int> intList = new List<int>();
    if (this.m_AlternateState.Count == 1)
    {
      intList.Add(0);
    }
    else
    {
      for (int index = 0; index < this.m_AlternateState.Count; ++index)
      {
        if (index != this.m_LastAlternateIndex)
          intList.Add(index);
      }
    }
    int index1 = UnityEngine.Random.Range(0, intList.Count);
    RandomPickPlaymakerEvent.PickEvent pickEvent = this.m_AlternateState[intList[index1]];
    this.m_CurrentState = pickEvent;
    this.m_LastAlternateIndex = intList[index1];
    this.m_StartAnimationFinished = false;
    this.StartCoroutine(this.WaitForStartAnimation());
    pickEvent.m_FSM.SendEvent(pickEvent.m_StartEvent);
  }

  private IEnumerator WaitForStartAnimation()
  {
    while (!this.m_StartAnimationFinished)
      yield return (object) null;
  }

  private IEnumerator WaitForEndAnimation()
  {
    while (!this.m_EndAnimationFinished)
      yield return (object) null;
    this.m_CurrentState = (RandomPickPlaymakerEvent.PickEvent) null;
    if (!this.m_AllowNoneState)
    {
      while (!this.m_StartAnimationFinished)
        yield return (object) null;
      this.RandomPickEvent();
    }
  }

  private void EnableCollider()
  {
    if (!((UnityEngine.Object) this.m_Collider != (UnityEngine.Object) null))
      return;
    this.m_Collider.enabled = true;
  }

  private void DisableCollider()
  {
    if (!((UnityEngine.Object) this.m_Collider != (UnityEngine.Object) null))
      return;
    this.m_Collider.enabled = false;
  }

  [Serializable]
  public class PickEvent
  {
    public PlayMakerFSM m_FSM;
    public string m_StartEvent;
    public string m_EndEvent;
    [HideInInspector]
    public int m_CurrentItemIndex;
  }
}
