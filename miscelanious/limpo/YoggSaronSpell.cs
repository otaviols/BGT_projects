using Blizzard.T5.Core;
using System.Collections;
using UnityEngine;

public class YoggSaronSpell : Spell
{
  public Spell m_MistSpellPrefab;
  private static Map<int, Spell> s_mistSpellInstances = new Map<int, Spell>();

  public override bool CanPurge() => YoggSaronSpell.s_mistSpellInstances.Count == 0;

  public override bool AddPowerTargets()
  {
    int id = this.m_taskList.GetOrigin().GetId();
    return !YoggSaronSpell.s_mistSpellInstances.ContainsKey(id) || this.m_taskList.IsEndOfBlock();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoEffectsWithTiming());
  }

  private IEnumerator DoEffectsWithTiming()
  {
    YoggSaronSpell yoggSaronSpell = this;
    int taskListID = yoggSaronSpell.m_taskList.GetOrigin().GetId();
    Spell mistSpellInstance = (Spell) null;
    if (!YoggSaronSpell.s_mistSpellInstances.ContainsKey(taskListID))
    {
      mistSpellInstance = SpellManager.Get().GetSpell(yoggSaronSpell.m_MistSpellPrefab);
      YoggSaronSpell.s_mistSpellInstances[taskListID] = mistSpellInstance;
      if ((bool) (Object) mistSpellInstance)
      {
        mistSpellInstance.ActivateState(SpellStateType.BIRTH);
        while (mistSpellInstance.GetActiveState() != SpellStateType.IDLE)
          yield return (object) null;
      }
    }
    else
      mistSpellInstance = YoggSaronSpell.s_mistSpellInstances[taskListID];
    if ((bool) (Object) mistSpellInstance && yoggSaronSpell.m_taskList.IsEndOfBlock())
    {
      mistSpellInstance.ActivateState(SpellStateType.DEATH);
      while (!mistSpellInstance.IsFinished())
        yield return (object) null;
      yoggSaronSpell.OnSpellFinished();
      while (mistSpellInstance.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
      YoggSaronSpell.s_mistSpellInstances.Remove(taskListID);
      SpellManager.Get().ReleaseSpell(mistSpellInstance);
    }
    if (yoggSaronSpell.GetActiveState() != SpellStateType.NONE)
      yoggSaronSpell.OnStateFinished();
  }
}
