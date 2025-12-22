using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoingOozeSpell : Spell
{
  public Spell m_CustomSpawnSpell;
  public float m_PostSpawnDelayMin;
  public float m_PostSpawnDelayMax;

  protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
  {
    if (!(task.GetPower() is Network.HistFullEntity power))
      return (Card) null;
    Network.Entity entity1 = power.Entity;
    Entity entity2 = GameState.Get().GetEntity(entity1.ID);
    if (entity2 != null)
      return entity2.GetCard();
    Debug.LogWarning((object) string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) entity1.ID));
    return (Card) null;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    Card targetCard = this.GetTargetCard();
    if ((Object) targetCard == (Object) null)
      this.OnStateFinished();
    else
      this.DoEffect(targetCard);
  }

  private void DoEffect(Card targetCard)
  {
    Spell spell = SpellManager.Get().GetSpell(this.m_CustomSpawnSpell);
    targetCard.OverrideCustomSpawnSpell(spell);
    this.DoTasksUntilSpawn(targetCard);
    this.StartCoroutine(this.WaitThenFinish());
  }

  private void DoTasksUntilSpawn(Card targetCard)
  {
    int entityId = targetCard.GetEntity().GetEntityId();
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    int num = 0;
    for (int index = 0; index < taskList.Count; ++index)
    {
      if (taskList[index].GetPower() is Network.HistFullEntity power && power.Entity.ID == entityId)
      {
        num = index;
        break;
      }
    }
    this.m_taskList.DoTasks(0, num + 1);
  }

  private IEnumerator WaitThenFinish()
  {
    EchoingOozeSpell echoingOozeSpell = this;
    float num = Random.Range(echoingOozeSpell.m_PostSpawnDelayMin, echoingOozeSpell.m_PostSpawnDelayMax);
    if (!Mathf.Approximately(num, 0.0f))
      yield return (object) new WaitForSeconds(num);
    echoingOozeSpell.OnStateFinished();
  }
}
