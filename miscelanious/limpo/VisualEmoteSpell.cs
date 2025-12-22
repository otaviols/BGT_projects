using UnityEngine;

public class VisualEmoteSpell : Spell
{
  public Spell m_FriendlySpellPrefab;
  public Spell m_OpponentSpellPrefab;
  public bool m_PositionOnSpeechBubble;
  protected int m_effectsPendingFinish;

  protected override void OnBirth(SpellStateType prevStateType)
  {
    base.OnBirth(prevStateType);
    Spell spell1 = (Spell) null;
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard != (Object) null)
    {
      Player controller = sourceCard.GetController();
      if (controller != null)
      {
        if (controller.IsFriendlySide())
          spell1 = this.m_FriendlySpellPrefab;
        else if (!controller.IsFriendlySide())
          spell1 = this.m_OpponentSpellPrefab;
      }
    }
    if ((Object) spell1 != (Object) null)
    {
      Spell spell2 = SpellManager.Get().GetSpell(spell1);
      spell2.SetSource(this.GetSource());
      spell2.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished));
      spell2.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellEffectFinished));
      ++this.m_effectsPendingFinish;
      spell2.Activate();
    }
    if (this.HasStateContent(SpellStateType.BIRTH))
      return;
    this.OnStateFinished();
  }

  private void FinishIfPossible()
  {
    if (this.m_effectsPendingFinish != 0)
      return;
    base.OnSpellFinished();
  }

  public override void OnSpellFinished() => this.FinishIfPossible();

  private void OnSpellEffectFinished(Spell spell, object userData)
  {
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }

  private void OnSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    SpellManager.Get().ReleaseSpell(spell);
  }
}
