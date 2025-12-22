using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class StateEventTable : MonoBehaviour
{
  [CustomEditField(ListTable = true, Sections = "Event Table")]
  public List<StateEventTable.StateEvent> m_Events = new List<StateEventTable.StateEvent>();
  private Map<string, List<StateEventTable.StateEventTrigger>> m_StateEventStartListeners = new Map<string, List<StateEventTable.StateEventTrigger>>();
  private Map<string, List<StateEventTable.StateEventTrigger>> m_StateEventEndListeners = new Map<string, List<StateEventTable.StateEventTrigger>>();
  private Map<string, List<StateEventTable.StateEventTrigger>> m_StateEventStartOnceListeners = new Map<string, List<StateEventTable.StateEventTrigger>>();
  private Map<string, List<StateEventTable.StateEventTrigger>> m_StateEventEndOnceListeners = new Map<string, List<StateEventTable.StateEventTrigger>>();
  private QueueList<StateEventTable.QueueStateEvent> m_QueuedEvents = new QueueList<StateEventTable.QueueStateEvent>();
  private string m_LastState;

  public void TriggerState(string eventName, bool saveLastState = true, string nameOverride = null)
  {
    StateEventTable.StateEvent stateEvent = this.GetStateEvent(eventName);
    if (stateEvent == null)
    {
      Debug.LogError((object) string.Format("{0} not defined in event table.", (object) eventName), (UnityEngine.Object) this.gameObject);
    }
    else
    {
      this.m_QueuedEvents.Enqueue(new StateEventTable.QueueStateEvent()
      {
        m_StateEvent = stateEvent,
        m_NameOverride = nameOverride,
        m_SaveAsLastState = saveLastState
      });
      Log.EventTable.Print("Enqueuing event {0}", (object) eventName);
      if (this.m_QueuedEvents.Count == 1)
        this.StartNextQueuedState((StateEventTable.QueueStateEvent) null);
      else
        Log.EventTable.Print("Event {0} will not start yet, currently waiting on event {1}.", (object) eventName, (object) this.m_QueuedEvents.Peek().m_StateEvent.m_Name);
    }
  }

  public bool HasState(string eventName) => this.m_Events.Find((Predicate<StateEventTable.StateEvent>) (e => e.m_Name == eventName)) != null;

  public void CancelQueuedStates() => this.m_QueuedEvents.Clear();

  public Spell GetSpellEvent(string eventName) => this.GetStateEvent(eventName)?.m_Event;

  public string GetLastState() => this.m_LastState;

  public void AddStateEventStartListener(
    string eventName,
    StateEventTable.StateEventTrigger dlg,
    bool once = false)
  {
    this.AddStateEventListener(once ? this.m_StateEventStartOnceListeners : this.m_StateEventStartListeners, eventName, dlg);
  }

  public void RemoveStateEventStartListener(string eventName, StateEventTable.StateEventTrigger dlg) => this.RemoveStateEventListener(this.m_StateEventStartListeners, eventName, dlg);

  public void AddStateEventEndListener(
    string eventName,
    StateEventTable.StateEventTrigger dlg,
    bool once = false)
  {
    this.AddStateEventListener(once ? this.m_StateEventEndOnceListeners : this.m_StateEventEndListeners, eventName, dlg);
  }

  public void RemoveStateEventEndListener(string eventName, StateEventTable.StateEventTrigger dlg) => this.RemoveStateEventListener(this.m_StateEventEndListeners, eventName, dlg);

  public PlayMakerFSM GetFSMFromEvent(string evtName)
  {
    Spell spellEvent = this.GetSpellEvent(evtName);
    return (UnityEngine.Object) spellEvent != (UnityEngine.Object) null ? spellEvent.GetComponent<PlayMakerFSM>() : (PlayMakerFSM) null;
  }

  public void SetFloatVar(string eventName, string varName, float value)
  {
    PlayMakerFSM fsmFromEvent = this.GetFSMFromEvent(eventName);
    if ((UnityEngine.Object) fsmFromEvent == (UnityEngine.Object) null)
      return;
    fsmFromEvent.FsmVariables.GetFsmFloat(varName).Value = value;
  }

  public void SetIntVar(string eventName, string varName, int value)
  {
    PlayMakerFSM fsmFromEvent = this.GetFSMFromEvent(eventName);
    if ((UnityEngine.Object) fsmFromEvent == (UnityEngine.Object) null)
      return;
    fsmFromEvent.FsmVariables.GetFsmInt(varName).Value = value;
  }

  public void SetBoolVar(string eventName, string varName, bool value)
  {
    PlayMakerFSM fsmFromEvent = this.GetFSMFromEvent(eventName);
    if ((UnityEngine.Object) fsmFromEvent == (UnityEngine.Object) null)
      return;
    fsmFromEvent.FsmVariables.GetFsmBool(varName).Value = value;
  }

  public void SetGameObjectVar(string eventName, string varName, GameObject value)
  {
    PlayMakerFSM fsmFromEvent = this.GetFSMFromEvent(eventName);
    if ((UnityEngine.Object) fsmFromEvent == (UnityEngine.Object) null)
      return;
    fsmFromEvent.FsmVariables.GetFsmGameObject(varName).Value = value;
  }

  public void SetGameObjectVar(string eventName, string varName, Component value)
  {
    PlayMakerFSM fsmFromEvent = this.GetFSMFromEvent(eventName);
    if ((UnityEngine.Object) fsmFromEvent == (UnityEngine.Object) null)
      return;
    fsmFromEvent.FsmVariables.GetFsmGameObject(varName).Value = value.gameObject;
  }

  public void SetVector3Var(string eventName, string varName, Vector3 value)
  {
    PlayMakerFSM fsmFromEvent = this.GetFSMFromEvent(eventName);
    if ((UnityEngine.Object) fsmFromEvent == (UnityEngine.Object) null)
      return;
    fsmFromEvent.FsmVariables.GetFsmVector3(varName).Value = value;
  }

  public void SetVar(string eventName, string varName, object value)
  {
    if (value is GameObject)
      this.SetGameObjectVar(eventName, varName, (GameObject) value);
    else if (value is Component)
    {
      this.SetGameObjectVar(eventName, varName, (Component) value);
    }
    else
    {
      Action action;
      if (new Map<System.Type, Action>()
      {
        {
          typeof (float),
          (Action) (() => this.SetFloatVar(eventName, varName, (float) value))
        },
        {
          typeof (int),
          (Action) (() => this.SetIntVar(eventName, varName, (int) value))
        },
        {
          typeof (bool),
          (Action) (() => this.SetBoolVar(eventName, varName, (bool) value))
        }
      }.TryGetValue(value.GetType(), out action))
        action();
      else
        Debug.LogError((object) string.Format("Set var type ({0}) not supported.", (object) value.GetType()));
    }
  }

  protected StateEventTable.StateEvent GetStateEvent(string eventName) => this.m_Events.Find((Predicate<StateEventTable.StateEvent>) (e => e.m_Name == eventName));

  private void StartNextQueuedState(StateEventTable.QueueStateEvent lastEvt)
  {
    if (this.m_QueuedEvents.Count == 0)
    {
      if (lastEvt == null)
        return;
      this.FireStateEventFinishedEvent(this.m_StateEventEndListeners, lastEvt);
      this.FireStateEventFinishedEvent(this.m_StateEventEndOnceListeners, lastEvt, true);
    }
    else
    {
      StateEventTable.QueueStateEvent queueStateEvent = this.m_QueuedEvents.Peek();
      StateEventTable.StateEvent stateEvent = queueStateEvent.m_StateEvent;
      if (queueStateEvent.m_SaveAsLastState)
        this.m_LastState = queueStateEvent.GetEventName();
      stateEvent.m_Event.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.QueueNextState), (object) queueStateEvent);
      this.FireStateEventFinishedEvent(this.m_StateEventStartListeners, queueStateEvent);
      this.FireStateEventFinishedEvent(this.m_StateEventStartOnceListeners, queueStateEvent, true);
      stateEvent.m_Event.Activate();
    }
  }

  private void QueueNextState(Spell spell, SpellStateType prevStateType, object thisStateEvent)
  {
    if (this.m_QueuedEvents.Count == 0)
      return;
    this.m_QueuedEvents.Dequeue();
    this.StartNextQueuedState((StateEventTable.QueueStateEvent) thisStateEvent);
  }

  private void AddStateEventListener(
    Map<string, List<StateEventTable.StateEventTrigger>> listenerDict,
    string eventName,
    StateEventTable.StateEventTrigger dlg)
  {
    List<StateEventTable.StateEventTrigger> stateEventTriggerList;
    if (!listenerDict.TryGetValue(eventName, out stateEventTriggerList))
    {
      stateEventTriggerList = new List<StateEventTable.StateEventTrigger>();
      listenerDict[eventName] = stateEventTriggerList;
    }
    stateEventTriggerList.Add(dlg);
  }

  private void RemoveStateEventListener(
    Map<string, List<StateEventTable.StateEventTrigger>> listenerDict,
    string eventName,
    StateEventTable.StateEventTrigger dlg)
  {
    List<StateEventTable.StateEventTrigger> stateEventTriggerList;
    if (!listenerDict.TryGetValue(eventName, out stateEventTriggerList))
      return;
    stateEventTriggerList.Remove(dlg);
  }

  private void FireStateEventFinishedEvent(
    Map<string, List<StateEventTable.StateEventTrigger>> listenerDict,
    StateEventTable.QueueStateEvent stateEvt,
    bool clear = false)
  {
    List<StateEventTable.StateEventTrigger> stateEventTriggerList;
    if (!listenerDict.TryGetValue(stateEvt.GetEventName(), out stateEventTriggerList))
      return;
    foreach (StateEventTable.StateEventTrigger stateEventTrigger in stateEventTriggerList.ToArray())
      stateEventTrigger(stateEvt.m_StateEvent.m_Event);
    if (!clear)
      return;
    stateEventTriggerList.Clear();
  }

  [Serializable]
  public class StateEvent
  {
    public string m_Name;
    public Spell m_Event;
  }

  protected class QueueStateEvent
  {
    public StateEventTable.StateEvent m_StateEvent;
    public string m_NameOverride;
    public bool m_SaveAsLastState = true;

    public string GetEventName() => !string.IsNullOrEmpty(this.m_NameOverride) ? this.m_NameOverride : this.m_StateEvent.m_Name;
  }

  public delegate void StateEventTrigger(Spell evt);
}
