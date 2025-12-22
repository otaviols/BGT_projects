using UnityEngine;

public class OverrideCustomDiscardSpell : SuperSpell
{
  public Spell m_CustomDiscardSpell;

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    foreach (GameObject visualTarget in this.GetVisualTargets())
    {
      if (!((Object) visualTarget == (Object) null))
        visualTarget.GetComponent<Card>().OverrideCustomDiscardSpell(SpellManager.Get().GetSpell(this.m_CustomDiscardSpell));
    }
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }
}
