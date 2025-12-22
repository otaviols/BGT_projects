using PegasusGame;
using UnityEngine;

public class LifestealAuxiliarySpell : Spell
{
  public override bool AttachPowerTaskList(PowerTaskList taskList)
  {
    if (!base.AttachPowerTaskList(taskList))
      return false;
    PowerTaskList powerTaskList = taskList;
    if (powerTaskList == null)
    {
      Log.Gameplay.PrintError("{0}.AttachPowerTaskList(): Tasklist is NULL. Can't check for healing and damage metadata.", (object) this);
      return false;
    }
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard == (Object) null)
    {
      Log.Gameplay.PrintError("{0}.AttachPowerTaskList(): No source card found.", (object) this);
      return false;
    }
    Entity entity1 = sourceCard.GetEntity();
    if (entity1 == null)
    {
      Log.Gameplay.PrintError("{0}.AttachPowerTaskList(): Current tasklist has no source entity.", (object) this);
      return false;
    }
    Player controller = entity1.GetController();
    if (controller == null)
    {
      Log.Gameplay.PrintError("{0}.AttachPowerTaskList(): Source entity has no controller.", (object) this);
      return false;
    }
    Entity entity2 = (Entity) null;
    if (controller.HasTag(GAME_TAG.LIFESTEAL_DAMAGES_OPPOSING_HERO))
    {
      Player firstOpponentPlayer = GameState.Get().GetFirstOpponentPlayer(controller);
      if (firstOpponentPlayer != null)
        entity2 = firstOpponentPlayer.GetHero();
      if (entity2 == null)
      {
        Log.Gameplay.PrintError("{0}.AttachPowerTaskList(): Opposing entity's controller has no hero.", (object) this);
        return false;
      }
    }
    else
    {
      entity2 = controller.GetHero();
      if (entity2 == null)
      {
        Log.Gameplay.PrintError("{0}.AttachPowerTaskList(): Source entity's controller has no hero.", (object) this);
        return false;
      }
    }
    foreach (PowerTask task in powerTaskList.GetTaskList())
    {
      if (task.GetPower() is Network.HistMetaData power && (power.MetaType == HistoryMeta.Type.HEALING || power.MetaType == HistoryMeta.Type.DAMAGE))
      {
        Entity entity3 = GameState.Get().GetEntity(power.Info[0]);
        if (entity3 != null && entity3 == entity2 && !((Object) entity3.GetCard() == (Object) null))
        {
          this.SetSource(entity3.GetCard().gameObject);
          return true;
        }
      }
    }
    return false;
  }
}
