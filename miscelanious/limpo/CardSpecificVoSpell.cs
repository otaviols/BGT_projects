using System.Collections.Generic;
using UnityEngine;

public class CardSpecificVoSpell : CardSoundSpell
{
  public List<CardSpecificVoData> m_CardSpecificVoDataList = new List<CardSpecificVoData>();

  public override AudioSource DetermineBestAudioSource()
  {
    CardSpecificVoData bestVoiceData = this.GetBestVoiceData();
    return bestVoiceData == null ? base.DetermineBestAudioSource() : bestVoiceData.m_AudioSource;
  }

  public CardSpecificVoData GetBestVoiceData()
  {
    foreach (CardSpecificVoData cardSpecificVoData in this.m_CardSpecificVoDataList)
    {
      if (this.SearchForCard(cardSpecificVoData))
        return cardSpecificVoData;
    }
    return (CardSpecificVoData) null;
  }

  private bool SearchForCard(CardSpecificVoData cardVOData)
  {
    foreach (SpellZoneTag zoneTag in cardVOData.m_ZonesToSearch)
    {
      List<Zone> zonesFromTag = SpellUtils.FindZonesFromTag((Spell) this, zoneTag, cardVOData.m_SideToSearch);
      if (this.IsCardInZones(cardVOData.m_CardId, cardVOData.m_RequireTag, cardVOData.m_TagValue, zonesFromTag))
        return true;
    }
    return false;
  }

  private bool IsCardInZones(string cardId, GAME_TAG requireTag, int tagValue, List<Zone> zones)
  {
    if (zones == null)
      return false;
    foreach (Zone zone in zones)
    {
      foreach (Card card in zone.GetCards())
      {
        Entity entity = card.GetEntity();
        bool flag1 = true;
        bool flag2 = true;
        if (requireTag != GAME_TAG.TAG_NOT_SET)
          flag1 = entity.GetTag(requireTag) == tagValue;
        if (!string.IsNullOrEmpty(cardId))
          flag2 = entity.GetCardId() == cardId;
        if (flag1 & flag2)
          return true;
      }
    }
    return false;
  }
}
