using System.Collections.Generic;
using UnityEngine;

public abstract class MouseOverLinkSpell : Spell
{
  public Spell m_targetSpellFX;
  private List<Spell> m_spells;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine("SetupTargetsAndPlay");
  }

  protected override void OnNone(SpellStateType prevStateType)
  {
    if (this.m_spells != null)
    {
      foreach (Spell spell in this.m_spells)
      {
        spell.Deactivate();
        SpellManager.Get().ReleaseSpell(spell);
      }
      this.m_spells.Clear();
      this.m_spells = (List<Spell>) null;
    }
    base.OnNone(prevStateType);
  }

  protected abstract void GetAllTargets(Entity source, List<GameObject> targets);

  private void SetupTargetsAndPlay()
  {
    this.SetupTargets();
    if (this.m_targets.Count == 0)
      this.OnSpellFinished();
    else
      this.PlaySpells();
  }

  private void SetupTargets()
  {
    this.m_targets.Clear();
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard == (Object) null)
      return;
    Entity entity = sourceCard.GetEntity();
    if (entity == null)
      return;
    this.GetAllTargets(entity, this.m_targets);
  }

  private void PlaySpells()
  {
    this.m_spells = new List<Spell>();
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      Spell spell = SpellManager.Get().GetSpell(this.m_targetSpellFX);
      SpellUtils.SetCustomSpellParent(spell, (Component) component.GetActor());
      spell.SetSource(component.gameObject);
      spell.Activate();
      this.m_spells.Add(spell);
    }
  }
}
