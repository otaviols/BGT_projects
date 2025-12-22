using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (VisualController), typeof (Widget))]
public class MercenariesConsolationReward : MonoBehaviour
{
  private List<Action> m_doneCallbacks;

  public Widget Widget { get; private set; }

  public VisualController VisualController { get; private set; }

  private void Awake()
  {
    this.VisualController = this.GetComponent<VisualController>();
    this.Widget = this.GetComponent<Widget>();
    this.m_doneCallbacks = new List<Action>();
    this.Widget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener));
  }

  public void RegisterDoneCallback(Action action)
  {
    if (action == null)
      return;
    this.m_doneCallbacks.Add(action);
  }

  private void WidgetEventListener(string eventName)
  {
    if (!(eventName == "LOSE_REWARDS_CLOSE"))
      return;
    foreach (Action doneCallback in this.m_doneCallbacks)
      doneCallback();
    this.m_doneCallbacks.Clear();
  }
}
