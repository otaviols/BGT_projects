using UnityEngine;

public class DeathrattleSummonSpell : Spell
{
  protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
  {
    Network.PowerHistory power = task.GetPower();
    if (power.Type != Network.PowerType.FULL_ENTITY)
      return (Card) null;
    Network.Entity entity1 = ((Network.HistFullEntity) power).Entity;
    Entity entity2 = GameState.Get().GetEntity(entity1.ID);
    if (entity2 != null)
      return entity2.GetCard();
    Debug.LogWarning((object) string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) entity1.ID));
    return (Card) null;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    Card sourceCard = this.GetSourceCard();
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      component.transform.position = sourceCard.transform.position;
      float num = 0.2f;
      component.transform.localScale = new Vector3(num, num, num);
      component.SetTransitionStyle(ZoneTransitionStyle.VERY_SLOW);
      component.SetDoNotWarpToNewZone(true);
    }
    this.OnBirth(prevStateType);
    this.OnSpellFinished();
  }
}
