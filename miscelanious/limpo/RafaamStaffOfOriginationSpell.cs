using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RafaamStaffOfOriginationSpell : Spell
{
  public Spell m_CustomSpawnSpell;
  private int m_spawnTaskIndex;

  public override bool AddPowerTargets()
  {
    if (!this.m_taskList.DoesBlockHaveMetaDataTasks())
      return false;
    this.m_spawnTaskIndex = -1;
    bool flag = false;
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      switch (taskList[index].GetPower())
      {
        case Network.HistTagChange histTagChange when histTagChange.Tag == 420:
          flag = true;
          break;
        case Network.HistFullEntity histFullEntity when flag:
          Card card = GameState.Get().GetEntity(histFullEntity.Entity.ID).GetCard();
          if (!((Object) card == (Object) null))
          {
            this.m_targets.Add(card.gameObject);
            this.m_spawnTaskIndex = index;
            goto label_9;
          }
          else
            break;
      }
    }
label_9:
    return this.m_spawnTaskIndex >= 0;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.ApplyCustomSpawnOverride();
    this.DoTasksUntilSpawn();
  }

  private void ApplyCustomSpawnOverride()
  {
    foreach (GameObject target in this.m_targets)
      target.GetComponent<Card>().OverrideCustomSpawnSpell(SpellManager.Get().GetSpell(this.m_CustomSpawnSpell));
  }

  private void DoTasksUntilSpawn() => this.m_taskList.DoTasks(0, this.m_spawnTaskIndex, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => this.StartCoroutine(this.WaitThenFinish())));

  private IEnumerator WaitThenFinish()
  {
    RafaamStaffOfOriginationSpell originationSpell = this;
    Network.HistFullEntity power = (Network.HistFullEntity) originationSpell.m_taskList.GetTaskList()[originationSpell.m_spawnTaskIndex].GetPower();
    Spell electricSpell = GameState.Get().GetEntity(power.Entity.ID).GetHeroPowerCard().GetActorSpell(SpellType.ELECTRIC_CHARGE_LEVEL_LARGE);
    while (!electricSpell.IsFinished())
      yield return (object) null;
    originationSpell.OnStateFinished();
  }
}
