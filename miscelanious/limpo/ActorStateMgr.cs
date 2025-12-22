using Blizzard.T5.Core;
using System.Collections.Generic;
using UnityEngine;

public class ActorStateMgr : MonoBehaviour
{
  public GameObject m_ObjectContainer;
  private Map<ActorStateType, List<ActorState>> m_actorStateMap = new Map<ActorStateType, List<ActorState>>();
  private ActorStateType m_activeStateType;
  private bool m_shown = true;
  private HighlightState m_HighlightState;
  private int m_initialHighlightRenderQueue;

  private void Start()
  {
    this.m_HighlightState = this.FindHightlightObject();
    if ((Object) this.m_HighlightState != (Object) null)
      this.m_initialHighlightRenderQueue = this.m_HighlightState.m_RenderQueue;
    this.BuildStateMap();
    if (this.m_activeStateType == ActorStateType.NONE)
      this.HideImpl();
    else if (this.m_shown)
      this.ShowImpl();
    else
      this.HideImpl();
  }

  public Map<ActorStateType, List<ActorState>> GetStateMap() => this.m_actorStateMap;

  public ActorStateType GetActiveStateType() => this.m_activeStateType;

  public List<ActorState> GetActiveStateList()
  {
    List<ActorState> actorStateList = (List<ActorState>) null;
    return !this.m_actorStateMap.TryGetValue(this.m_activeStateType, out actorStateList) ? (List<ActorState>) null : actorStateList;
  }

  public float GetMaximumAnimationTimeOfActiveStates()
  {
    if (this.GetActiveStateList() == null)
      return 0.0f;
    float b = 0.0f;
    foreach (ActorState activeState in this.GetActiveStateList())
      b = Mathf.Max(activeState.GetAnimationDuration(), b);
    return b;
  }

  public bool ChangeState(ActorStateType stateType) => this.ChangeState_NewState(stateType) || this.ChangeState_LegacyState(stateType);

  public bool ChangeState_NewState(ActorStateType stateType)
  {
    if (!(bool) (Object) this.m_HighlightState)
      return false;
    int activeStateType = (int) this.m_activeStateType;
    this.m_activeStateType = stateType;
    int num = (int) stateType;
    return activeStateType == num || this.m_HighlightState.ChangeState(stateType);
  }

  public bool ChangeState_LegacyState(ActorStateType stateType)
  {
    List<ActorState> nextStateList = (List<ActorState>) null;
    this.m_actorStateMap.TryGetValue(stateType, out nextStateList);
    ActorStateType activeStateType = this.m_activeStateType;
    this.m_activeStateType = stateType;
    if (activeStateType != ActorStateType.NONE)
    {
      List<ActorState> actorStateList;
      if (this.m_actorStateMap.TryGetValue(activeStateType, out actorStateList))
      {
        foreach (ActorState actorState in actorStateList)
          actorState.Stop(nextStateList);
      }
    }
    else if (stateType != ActorStateType.NONE && (Object) this.m_ObjectContainer != (Object) null)
      this.m_ObjectContainer.SetActive(true);
    if (stateType == ActorStateType.NONE)
    {
      if (activeStateType != ActorStateType.NONE && (Object) this.m_ObjectContainer != (Object) null)
        this.m_ObjectContainer.SetActive(false);
      return true;
    }
    if (nextStateList == null)
      return false;
    foreach (ActorState actorState in nextStateList)
      actorState.Play();
    return true;
  }

  public void ShowStateMgr()
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    this.ShowImpl();
  }

  public void HideStateMgr()
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    this.HideImpl();
  }

  public void RefreshStateMgr()
  {
    if (!(bool) (Object) this.m_HighlightState)
      return;
    this.m_HighlightState.SetDirty();
  }

  public bool SetStateRenderQueue(bool reset, int renderQueue)
  {
    if ((Object) this.m_HighlightState == (Object) null)
      return false;
    this.m_HighlightState.m_RenderQueue = reset ? this.m_initialHighlightRenderQueue : renderQueue;
    return true;
  }

  private HighlightState FindHightlightObject()
  {
    foreach (Component component1 in this.transform)
    {
      HighlightState component2 = component1.gameObject.GetComponent<HighlightState>();
      if ((bool) (Object) component2)
        return component2;
    }
    return (HighlightState) null;
  }

  private void BuildStateMap()
  {
    foreach (Component component1 in this.transform)
    {
      ActorState component2 = component1.gameObject.GetComponent<ActorState>();
      if (!((Object) component2 == (Object) null))
      {
        ActorStateType stateType = component2.m_StateType;
        if (stateType != ActorStateType.NONE)
        {
          List<ActorState> actorStateList;
          if (!this.m_actorStateMap.TryGetValue(stateType, out actorStateList))
          {
            actorStateList = new List<ActorState>();
            this.m_actorStateMap.Add(stateType, actorStateList);
          }
          actorStateList.Add(component2);
        }
      }
    }
  }

  private void ShowImpl()
  {
    if ((bool) (Object) this.m_HighlightState)
      this.m_HighlightState.ChangeState(this.m_activeStateType);
    if (this.m_activeStateType != ActorStateType.NONE && (Object) this.m_ObjectContainer != (Object) null)
      this.m_ObjectContainer.SetActive(true);
    List<ActorState> activeStateList = this.GetActiveStateList();
    if (activeStateList == null)
      return;
    foreach (ActorState actorState in activeStateList)
      actorState.ShowState();
  }

  private void HideImpl()
  {
    if ((bool) (Object) this.m_HighlightState)
      this.m_HighlightState.ChangeState(ActorStateType.NONE);
    List<ActorState> activeStateList = this.GetActiveStateList();
    if (activeStateList != null)
    {
      foreach (ActorState actorState in activeStateList)
        actorState.HideState();
    }
    if (this.m_activeStateType == ActorStateType.NONE || !((Object) this.m_ObjectContainer != (Object) null))
      return;
    this.m_ObjectContainer.SetActive(false);
  }
}
