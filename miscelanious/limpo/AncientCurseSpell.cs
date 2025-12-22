using UnityEngine;

public class AncientCurseSpell : SuperSpell
{
  public void DoHeroDamage()
  {
    PowerTaskList currentTaskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
    if (currentTaskList == null)
      Debug.LogWarning((object) "AncientCurseSpell.DoHeroDamage() called when there was no current PowerTaskList!");
    else
      GameUtils.DoDamageTasks(currentTaskList, this.GetSourceCard(), this.GetVisualTargetCard());
  }
}
