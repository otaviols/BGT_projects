using UnityEngine;

public class PuzzleSuccessResetSpell : Spell
{
  public override bool AttachPowerTaskList(PowerTaskList taskList) => base.AttachPowerTaskList(taskList);

  protected override void OnDeath(SpellStateType prevStateType)
  {
    base.OnDeath(prevStateType);
    EndTurnButton endTurnButton = EndTurnButton.Get();
    if (!((Object) endTurnButton != (Object) null))
      return;
    endTurnButton.RemoveInputBlocker();
  }
}
