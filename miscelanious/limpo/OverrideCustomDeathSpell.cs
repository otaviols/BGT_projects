using UnityEngine;

public class OverrideCustomDeathSpell : SuperSpell
{
  public Spell m_CustomDeathSpell;
  public bool m_SuppressKeywordDeaths = true;
  public float m_KeywordDeathDelay = 0.6f;

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    foreach (GameObject visualTarget in this.GetVisualTargets())
    {
      if (!((Object) visualTarget == (Object) null))
      {
        Card component = visualTarget.GetComponent<Card>();
        component.OverrideCustomDeathSpell(SpellManager.Get().GetSpell(this.m_CustomDeathSpell));
        component.SuppressKeywordDeaths(this.m_SuppressKeywordDeaths);
        component.SetKeywordDeathDelaySec(this.m_KeywordDeathDelay);
      }
    }
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }
}
