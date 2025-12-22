using System;

public class Bolvar : SuperSpell
{
  public SpellValueRange[] m_atkPrefabs;

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    Card sourceCard = this.GetSourceCard();
    Spell rangePrefab = this.DetermineRangePrefab(sourceCard.GetEntity().GetATK());
    ++this.m_effectsPendingFinish;
    Spell spell = this.CloneSpell(rangePrefab);
    spell.SetSource(sourceCard.gameObject);
    spell.Activate();
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }

  private Spell DetermineRangePrefab(int atk) => SpellUtils.GetAppropriateElementAccordingToRanges<SpellValueRange>(this.m_atkPrefabs, (Func<SpellValueRange, ValueRange>) (x => x.m_range), atk)?.m_spellPrefab;
}
