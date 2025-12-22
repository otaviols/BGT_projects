using System.Collections.Generic;
using UnityEngine;

public class DeathSpellController : SpellController
{
  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    this.AddDeadCardsToTargetList(taskList);
    return this.m_targets.Count != 0;
  }

  protected override void OnProcessTaskList()
  {
    int num = this.PickDeathSoundCardIndex();
    for (int index = 0; index < this.m_targets.Count; ++index)
    {
      Card target = this.m_targets[index];
      if (index != num)
        target.SuppressDeathSounds(true);
      target.ActivateCharacterDeathEffects();
    }
    base.OnProcessTaskList();
  }

  private void AddDeadCardsToTargetList(PowerTaskList taskList)
  {
    List<PowerTask> taskList1 = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      Network.PowerHistory power = taskList1[index].GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange tagChange = power as Network.HistTagChange;
        if (GameUtils.IsCharacterDeathTagChange(tagChange))
        {
          Entity entity = GameState.Get().GetEntity(tagChange.Entity);
          Card card = entity.GetCard();
          if (this.CanAddTarget(entity, card))
            this.AddTarget(card);
        }
      }
    }
  }

  private bool CanAddTarget(Entity entity, Card card) => !card.WillSuppressDeathEffects();

  private int PickDeathSoundCardIndex()
  {
    if (this.m_targets.Count == 1)
      return this.CanPlayDeathSound(this.m_targets[0].GetEntity()) ? 0 : -1;
    if (this.m_targets.Count == 2)
    {
      Card target1 = this.m_targets[0];
      Card target2 = this.m_targets[1];
      Entity entity1 = target1.GetEntity();
      Entity entity2 = target2.GetEntity();
      if (this.WasAttackedBy(entity1, entity2))
        return this.CanPlayDeathSound(entity1) ? 0 : 1;
      if (this.WasAttackedBy(entity2, entity1))
        return this.CanPlayDeathSound(entity2) ? 1 : 0;
    }
    return this.PickRandomDeathSoundCardIndex();
  }

  private bool WasAttackedBy(Entity defender, Entity attacker) => attacker.HasTag(GAME_TAG.ATTACKING) && defender.HasTag(GAME_TAG.DEFENDING) && defender.GetTag(GAME_TAG.LAST_AFFECTED_BY) == attacker.GetEntityId();

  private int PickRandomDeathSoundCardIndex()
  {
    List<int> intList = new List<int>();
    for (int index = 0; index < this.m_targets.Count; ++index)
    {
      if (this.CanPlayDeathSound(this.m_targets[index].GetEntity()))
        intList.Add(index);
    }
    return intList.Count == 0 ? -1 : intList[Random.Range(0, intList.Count)];
  }

  private bool CanPlayDeathSound(Entity entity) => !entity.HasTag(GAME_TAG.DEATHRATTLE_RETURN_ZONE) && !entity.HasTag(GAME_TAG.SUPPRESS_DEATH_SOUND);
}
