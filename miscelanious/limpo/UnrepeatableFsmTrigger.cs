using Hearthstone.UI.Core;
using HutongGames.PlayMaker;
using System;
using UnityEngine;

public class UnrepeatableFsmTrigger : MonoBehaviour
{
  [SerializeField]
  private PlayMakerFSM m_targetFsm;
  [SerializeField]
  private string m_targetState;
  private string m_activeAction;

  [Overridable]
  public string StartingAction
  {
    set => this.m_activeAction = value;
  }

  [Overridable]
  public string TriggerAction
  {
    get => this.m_activeAction;
    set
    {
      if (string.IsNullOrEmpty(value))
      {
        this.m_activeAction = string.Empty;
      }
      else
      {
        if (!string.IsNullOrEmpty(this.m_activeAction) && this.m_activeAction.Equals(value, StringComparison.OrdinalIgnoreCase))
          return;
        this.m_activeAction = value;
        this.TryTriggerFsmState();
      }
    }
  }

  private void TryTriggerFsmState()
  {
    if ((UnityEngine.Object) this.m_targetFsm == (UnityEngine.Object) null || string.IsNullOrEmpty(this.m_targetState))
    {
      Debug.LogError((object) ("Failed to Trigger Fsm as " + this.gameObject.name + "\\UnrepeatableFsmTrigger is miss configured!"));
    }
    else
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      foreach (FsmState fsmState in this.m_targetFsm.FsmStates)
      {
        if (fsmState.Name == this.m_targetState)
        {
          this.m_targetFsm.SetState(this.m_targetState);
          break;
        }
      }
    }
  }
}
