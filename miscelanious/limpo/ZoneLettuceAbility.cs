using UnityEngine;

public class ZoneLettuceAbility : Zone
{
  public override string ToString() => string.Format("{0} (Lettuce Ability)", (object) base.ToString());

  public override bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return this.m_ServerTag == zoneTag && cardType == TAG_CARDTYPE.LETTUCE_ABILITY && entity != null;
  }

  public override void OnSpellPowerEntityMousedOver(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    if (TargetReticleManager.Get().IsActive())
      return;
    foreach (Card card in this.m_cards)
    {
      if (card.CanPlaySpellPowerHint(spellSchool))
      {
        Spell actorSpell = card.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST);
        if ((Object) actorSpell != (Object) null)
          actorSpell.Reactivate();
      }
    }
  }
}
