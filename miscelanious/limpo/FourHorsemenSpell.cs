using UnityEngine;

public class FourHorsemenSpell : SuperSpell
{
  public SuperSpell m_MissileSpell;
  public Spell m_DeathSpell;
  private int m_missilesWAitingToFinish;

  protected override void OnAction(SpellStateType prevStateType)
  {
    if (this.m_targets.Count <= 0)
    {
      this.OnSpellFinished();
      this.OnStateFinished();
    }
    else
    {
      ++this.m_effectsPendingFinish;
      base.OnAction(prevStateType);
      this.FireMissileVolley();
    }
  }

  private void FireMissileVolley()
  {
    if (!((Object) this.m_MissileSpell != (Object) null))
      return;
    for (int targetIndex = 0; targetIndex < this.m_visualTargets.Count; ++targetIndex)
    {
      ++this.m_missilesWAitingToFinish;
      this.FireSingleMissile(targetIndex);
    }
  }

  private void FireSingleMissile(int targetIndex)
  {
    ++this.m_effectsPendingFinish;
    SuperSpell superSpell = (SuperSpell) this.CloneSpell((Spell) this.m_MissileSpell);
    GameObject visualTarget = this.m_visualTargets[targetIndex];
    GameObject spellLocationObject = SpellUtils.GetSpellLocationObject((Spell) this, SpellLocation.OPPONENT_HERO);
    superSpell.SetSource(visualTarget);
    superSpell.AddTarget(spellLocationObject);
    if (targetIndex > 0)
      superSpell.m_ImpactInfo = (SpellImpactInfo) null;
    superSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnMissileFinished));
    superSpell.ActivateState(SpellStateType.ACTION);
  }

  private void OnMissileFinished(Spell spell, object userData)
  {
    --this.m_missilesWAitingToFinish;
    this.DoFinalImpactIfPossible();
  }

  protected void DoFinalImpactIfPossible()
  {
    if (this.m_missilesWAitingToFinish > 0)
      return;
    Spell spell = this.CloneSpell(this.m_DeathSpell);
    spell.SetSource(SpellUtils.GetSpellLocationObject((Spell) this, SpellLocation.OPPONENT_HERO));
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnDeathFinished));
    spell.Activate();
  }

  private void OnDeathFinished(Spell spell, object userData)
  {
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }
}
