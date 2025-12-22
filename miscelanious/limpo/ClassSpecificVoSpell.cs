using System.Collections.Generic;
using UnityEngine;

public class ClassSpecificVoSpell : CardSoundSpell
{
  public ClassSpecificVoData m_ClassSpecificVoData = new ClassSpecificVoData();

  public override AudioSource DetermineBestAudioSource()
  {
    AudioSource audioSource = this.SearchForClassSpecificVo();
    return (bool) (Object) audioSource ? audioSource : base.DetermineBestAudioSource();
  }

  private AudioSource SearchForClassSpecificVo()
  {
    foreach (SpellZoneTag zoneTag in this.m_ClassSpecificVoData.m_ZonesToSearch)
    {
      AudioSource audioSource = this.SearchForClassSpecificVo(SpellUtils.FindZonesFromTag((Spell) this, zoneTag, this.m_ClassSpecificVoData.m_SideToSearch));
      if ((bool) (Object) audioSource)
        return audioSource;
    }
    return (AudioSource) null;
  }

  private AudioSource SearchForClassSpecificVo(List<Zone> zones)
  {
    if (zones == null)
      return (AudioSource) null;
    foreach (Zone zone in zones)
    {
      foreach (Card card in zone.GetCards())
      {
        SpellClassTag spellEnum = SpellUtils.ConvertClassTagToSpellEnum(card.GetEntity().GetClass());
        if (spellEnum != SpellClassTag.NONE)
        {
          foreach (ClassSpecificVoLine line in this.m_ClassSpecificVoData.m_Lines)
          {
            if (line.m_Class == spellEnum)
              return line.m_AudioSource;
          }
        }
      }
    }
    return (AudioSource) null;
  }
}
