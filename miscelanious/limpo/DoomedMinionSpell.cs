using UnityEngine;

public class DoomedMinionSpell : SuperSpell
{
  public SpellType m_SpellType;

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    foreach (GameObject visualTarget in this.GetVisualTargets())
    {
      if (!((Object) visualTarget == (Object) null))
        visualTarget.GetComponent<Card>().ActivateActorSpell(this.m_SpellType);
    }
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }
}
