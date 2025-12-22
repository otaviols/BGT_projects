using System.Collections.Generic;
using UnityEngine;

public class HemetJungleHunterSpell : Spell
{
  private int m_cardsDestroyed;

  public override bool AddPowerTargets()
  {
    if (!this.CanAddPowerTargets())
      return false;
    int num = 0;
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      if (taskList[index].GetPower() is Network.HistShowEntity power)
      {
        foreach (Network.Entity.Tag tag in power.Entity.Tags)
        {
          if (tag.Name == 49 && tag.Value == 6)
          {
            ++num;
            break;
          }
        }
      }
    }
    this.m_cardsDestroyed = num;
    return true;
  }

  protected override void OnAttachPowerTaskList()
  {
    base.OnAttachPowerTaskList();
    PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
    if (!((Object) component != (Object) null))
      return;
    component.FsmVariables.GetFsmInt("CardsDestroyed").Value = this.m_cardsDestroyed;
  }
}
