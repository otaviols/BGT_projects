using System.Collections;
using UnityEngine;

public class NefarianSwapSpell : HeroSwapSpell
{
  public float m_obsoleteRemovalDelay;
  private Card m_obsoleteHeroCard;

  public override bool AddPowerTargets()
  {
    if (!base.AddPowerTargets())
      return false;
    int tag = this.m_oldHeroCard.GetEntity().GetTag(GAME_TAG.LINKED_ENTITY);
    if (tag != 0)
      this.m_obsoleteHeroCard = GameState.Get().GetEntity(tag).GetCard();
    return !((Object) this.m_obsoleteHeroCard == (Object) null);
  }

  public override void CustomizeFXProcess(Actor heroActor)
  {
    if (!((Object) heroActor == (Object) this.m_newHeroCard.GetActor()))
      return;
    this.StartCoroutine(this.DestroyObsolete());
  }

  private IEnumerator DestroyObsolete()
  {
    yield return (object) new WaitForSeconds(this.m_obsoleteRemovalDelay);
    Actor actor = this.m_obsoleteHeroCard.GetActor();
    if ((Object) actor != (Object) null)
      Object.Destroy((Object) actor.gameObject);
  }
}
