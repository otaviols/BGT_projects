using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTransformedCardSpell : SuperSpell
{
  public float m_OldCardHoldTime;
  public float m_NewCardHoldTime;
  public bool m_FriendlyOnly;
  private int m_transformTaskIndex;

  public override bool AddPowerTargets()
  {
    base.AddPowerTargets();
    return this.FindTransformTask();
  }

  private bool FindTransformTask()
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      Network.PowerHistory power = taskList[index].GetPower();
      if (power.Type == Network.PowerType.CHANGE_ENTITY)
      {
        Network.HistChangeEntity histChangeEntity = (Network.HistChangeEntity) power;
        Entity entity = GameState.Get().GetEntity(histChangeEntity.Entity.ID);
        if (entity != null)
        {
          Card card = entity.GetCard();
          if (!((Object) card == (Object) null) && (!this.m_FriendlyOnly || card.GetEntity().IsControlledByFriendlySidePlayer()))
          {
            this.m_transformTaskIndex = index;
            this.AddTarget(card.gameObject);
            return true;
          }
        }
      }
    }
    return false;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoTasksBeforeTransform());
    this.StartCoroutine(this.DoEffectWithTiming());
  }

  private IEnumerator DoTasksBeforeTransform()
  {
    DrawTransformedCardSpell transformedCardSpell = this;
    bool complete = false;
    transformedCardSpell.m_taskList.DoTasks(0, transformedCardSpell.m_transformTaskIndex, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true));
    while (!complete)
      yield return (object) null;
  }

  private IEnumerator DoEffectWithTiming()
  {
    DrawTransformedCardSpell transformedCardSpell = this;
    yield return (object) new WaitForSeconds(transformedCardSpell.m_OldCardHoldTime);
    bool complete = false;
    transformedCardSpell.m_taskList.DoTasks(transformedCardSpell.m_transformTaskIndex, 1, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true));
    while (!complete)
      yield return (object) null;
    PowerTask transformTask = transformedCardSpell.m_taskList.GetTaskList()[transformedCardSpell.m_transformTaskIndex];
    transformTask.SetCompleted(false);
    yield return (object) new WaitForSeconds(transformedCardSpell.m_NewCardHoldTime);
    transformTask.SetCompleted(true);
    transformedCardSpell.OnSpellFinished();
  }
}
