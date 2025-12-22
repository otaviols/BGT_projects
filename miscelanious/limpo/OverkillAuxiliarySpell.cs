using UnityEngine;

public class OverkillAuxiliarySpell : Spell
{
  public override bool AttachPowerTaskList(PowerTaskList taskList)
  {
    if (!base.AttachPowerTaskList(taskList))
      return false;
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard == (Object) null)
    {
      Log.Gameplay.PrintError("{0}.AttachPowerTaskList(): No source card found.", (object) this);
      return false;
    }
    Entity entity = sourceCard.GetEntity();
    if (entity == null)
    {
      Log.Gameplay.PrintError("{0}.AttachPowerTaskList(): Current tasklist has no source entity.", (object) this);
      return false;
    }
    return !entity.IsSpell();
  }
}
