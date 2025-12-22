using UnityEngine;

public class DistinctSpellOnEachSide : Spell
{
  public Spell m_FriendlySideSpell;
  public Spell m_OpponentSideSpell;
  private Spell m_oneSideSpell;

  private bool InitSpell()
  {
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard == (Object) null)
      return false;
    Spell spell = sourceCard.GetControllerSide() == Player.Side.FRIENDLY ? this.m_FriendlySideSpell : this.m_OpponentSideSpell;
    this.m_oneSideSpell = SpellManager.Get().GetSpell(spell);
    this.m_oneSideSpell.SetSource(sourceCard.gameObject);
    return true;
  }

  public override bool AttachPowerTaskList(PowerTaskList taskList) => this.InitSpell() && base.AttachPowerTaskList(taskList) && this.m_oneSideSpell.AttachPowerTaskList(taskList);

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.m_oneSideSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnOneSideSpellFinished));
    this.m_oneSideSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOneSideSpellStateFinished));
    this.m_oneSideSpell.ActivateState(SpellStateType.ACTION);
  }

  private void OnOneSideSpellFinished(Spell spell, object userData) => this.OnSpellFinished();

  private void OnOneSideSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    Object.Destroy((Object) this.m_oneSideSpell.gameObject);
    this.m_oneSideSpell = (Spell) null;
    this.Deactivate();
  }
}
