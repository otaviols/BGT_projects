using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoodooDollLinkSpell : Spell
{
  public Spell m_VooDooFX;
  private List<Spell> m_voodooSpells;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.SetupTargetsAndPlay());
  }

  protected override void OnNone(SpellStateType prevStateType)
  {
    if (this.m_voodooSpells != null)
    {
      foreach (Spell voodooSpell in this.m_voodooSpells)
      {
        voodooSpell.Deactivate();
        SpellManager.Get().ReleaseSpell(voodooSpell);
      }
      this.m_voodooSpells.Clear();
      this.m_voodooSpells = (List<Spell>) null;
    }
    base.OnNone(prevStateType);
  }

  private IEnumerator SetupTargetsAndPlay()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    VoodooDollLinkSpell voodooDollLinkSpell = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    voodooDollLinkSpell.SetupTargets();
    if (voodooDollLinkSpell.m_targets.Count == 0)
    {
      voodooDollLinkSpell.OnSpellFinished();
      return false;
    }
    voodooDollLinkSpell.PlaySpells();
    return false;
  }

  private void SetupTargets()
  {
    this.m_targets.Clear();
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard == (Object) null)
      return;
    Entity entity1 = sourceCard.GetEntity();
    if (entity1.HasTag(GAME_TAG.VOODOO_LINK))
    {
      Entity entity2 = GameState.Get().GetEntity(entity1.GetTag(GAME_TAG.VOODOO_LINK));
      if (entity2 == null || !((Object) entity2.GetCard() != (Object) null))
        return;
      this.m_targets.Add(sourceCard.gameObject);
      this.m_targets.Add(entity2.GetCard().gameObject);
    }
    else
    {
      foreach (Entity attachment in entity1.GetAttachments())
      {
        if (attachment.HasTag(GAME_TAG.VOODOO_LINK))
        {
          Entity entity3 = GameState.Get().GetEntity(attachment.GetTag(GAME_TAG.VOODOO_LINK));
          if (entity3 != null && (Object) entity3.GetCard() != (Object) null)
            this.m_targets.Add(entity3.GetCard().gameObject);
        }
      }
      if (this.m_targets.Count <= 0)
        return;
      this.m_targets.Add(sourceCard.gameObject);
    }
  }

  private void PlaySpells()
  {
    this.m_voodooSpells = new List<Spell>();
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      Spell spell = SpellManager.Get().GetSpell(this.m_VooDooFX);
      SpellUtils.SetCustomSpellParent(spell, (Component) component.GetActor());
      spell.SetSource(component.gameObject);
      spell.Activate();
      this.m_voodooSpells.Add(spell);
    }
  }
}
