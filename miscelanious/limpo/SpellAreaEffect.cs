using UnityEngine;

public class SpellAreaEffect : Spell
{
  public Spell m_ImpactSpellPrefab;

  public override bool AddPowerTargets() => this.CanAddPowerTargets() && this.AddMultiplePowerTargets() && this.GetTargets().Count > 0;

  protected override void OnDeath(SpellStateType prevStateType)
  {
    base.OnDeath(prevStateType);
    if ((Object) this.m_ImpactSpellPrefab == (Object) null)
      return;
    for (int index = 0; index < this.m_targets.Count; ++index)
      this.SpawnImpactSpell(this.m_targets[index]);
  }

  private void SpawnImpactSpell(GameObject targetObject)
  {
    Spell spell = SpellManager.Get().GetSpell(this.m_ImpactSpellPrefab);
    spell.transform.position = targetObject.transform.position;
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnImpactSpellStateFinished));
    spell.Activate();
  }

  private void OnImpactSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    Object.Destroy((Object) spell.gameObject);
  }
}
