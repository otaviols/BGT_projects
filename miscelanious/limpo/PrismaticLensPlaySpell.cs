using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismaticLensPlaySpell : Spell
{
  public Spell m_CostSwapSpell;
  private Spell m_swapSpell;
  private const string SWAPPED_COST_ENCHANTMENT = "BOT_436e";

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.SetInputEnabled(false);
    this.StartCoroutine(this.DoEffectWithTiming());
    base.OnAction(prevStateType);
  }

  public override void OnSpellFinished()
  {
    this.SetInputEnabled(true);
    base.OnSpellFinished();
  }

  private void SetInputEnabled(bool enabled)
  {
    foreach (GameObject target in this.m_targets)
      target.GetComponent<Card>().SetInputEnabled(enabled);
  }

  private IEnumerator DoEffectWithTiming()
  {
    PrismaticLensPlaySpell prismaticLensPlaySpell = this;
    int countToRunUntilDraw = prismaticLensPlaySpell.FindTaskCountToRunUntilDraw();
    if (countToRunUntilDraw > 0)
    {
      yield return (object) prismaticLensPlaySpell.StartCoroutine(prismaticLensPlaySpell.CompleteTasksUntilDraw(countToRunUntilDraw));
      yield return (object) prismaticLensPlaySpell.StartCoroutine(prismaticLensPlaySpell.WaitForDrawing());
      yield return (object) prismaticLensPlaySpell.StartCoroutine(prismaticLensPlaySpell.PlayCostSwapSpell());
    }
    else
    {
      prismaticLensPlaySpell.OnSpellFinished();
      prismaticLensPlaySpell.Deactivate();
    }
  }

  private int FindTaskCountToRunUntilDraw()
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      Network.PowerHistory power = taskList[index].GetPower();
      if (power.Type == Network.PowerType.SHOW_ENTITY && !(((Network.HistShowEntity) power).Entity.CardID != "BOT_436e"))
        return index;
    }
    return -1;
  }

  private IEnumerator CompleteTasksUntilDraw(int taskCount)
  {
    PrismaticLensPlaySpell prismaticLensPlaySpell = this;
    bool complete = false;
    prismaticLensPlaySpell.m_taskList.DoTasks(0, taskCount, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true));
    while (!complete)
      yield return (object) null;
  }

  private IEnumerator WaitForDrawing()
  {
    while (this.IsDrawing())
      yield return (object) null;
  }

  private bool IsDrawing()
  {
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      if (!(component.GetZone() is ZoneHand) || component.IsDoNotSort() || !component.CardStandInIsInteractive())
        return true;
    }
    return false;
  }

  private IEnumerator PlayCostSwapSpell()
  {
    PrismaticLensPlaySpell prismaticLensPlaySpell = this;
    prismaticLensPlaySpell.m_swapSpell = SpellManager.Get().GetSpell(prismaticLensPlaySpell.m_CostSwapSpell);
    prismaticLensPlaySpell.m_swapSpell.AttachPowerTaskList(prismaticLensPlaySpell.GetPowerTaskList());
    prismaticLensPlaySpell.m_swapSpell.SetSource(prismaticLensPlaySpell.GetSource());
    prismaticLensPlaySpell.m_swapSpell.ActivateState(SpellStateType.ACTION);
    while (!prismaticLensPlaySpell.m_swapSpell.IsFinished())
      yield return (object) null;
    prismaticLensPlaySpell.OnSpellFinished();
    while (prismaticLensPlaySpell.m_swapSpell.GetActiveState() != SpellStateType.NONE)
      yield return (object) null;
    SpellManager.Get().ReleaseSpell(prismaticLensPlaySpell.m_swapSpell);
    prismaticLensPlaySpell.m_swapSpell = (Spell) null;
    prismaticLensPlaySpell.Deactivate();
  }
}
