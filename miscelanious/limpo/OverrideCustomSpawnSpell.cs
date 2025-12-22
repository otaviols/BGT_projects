using UnityEngine;

public class OverrideCustomSpawnSpell : SuperSpell
{
  public Spell m_CustomSpawnSpell;

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    if ((Object) this.m_CustomSpawnSpell == (Object) null)
    {
      Debug.LogError((object) "OverrideCustomSpawnSpell.OverrideCustomSpawnSpell in null!");
      --this.m_effectsPendingFinish;
      this.FinishIfPossible();
    }
    else
    {
      foreach (GameObject visualTarget in this.GetVisualTargets())
      {
        if (!((Object) visualTarget == (Object) null))
          visualTarget.GetComponent<Card>().OverrideCustomSpawnSpell(SpellManager.Get().GetSpell(this.m_CustomSpawnSpell));
      }
      --this.m_effectsPendingFinish;
      this.FinishIfPossible();
    }
  }
}
