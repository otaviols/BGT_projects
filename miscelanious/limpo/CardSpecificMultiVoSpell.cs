using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpecificMultiVoSpell : CardSoundSpell
{
  public CardSpecificMultiVoData m_CardSpecificVoData = new CardSpecificMultiVoData();
  private int m_ActiveAudioIndex;
  private bool m_SpecificCardFound;

  protected override void Play()
  {
    if (!this.m_forceDefaultAudioSource)
      this.m_SpecificCardFound = this.SearchForCard();
    if (this.m_SpecificCardFound)
    {
      this.Stop();
      this.m_ActiveAudioIndex = 0;
      this.m_activeAudioSource = this.m_forceDefaultAudioSource ? this.m_CardSoundData.m_AudioSource : this.DetermineBestAudioSource();
      if ((Object) this.m_activeAudioSource == (Object) null)
        this.OnStateFinished();
      else
        this.StartCoroutine("DelayedPlayMulti");
    }
    else
      base.Play();
  }

  protected virtual void PlayNowMulti()
  {
    SoundManager.Get().Play(this.m_activeAudioSource);
    this.StartCoroutine("WaitForSourceThenContinue");
  }

  protected override void Stop()
  {
    this.StopCoroutine("WaitForSourceThenContinue");
    base.Stop();
  }

  public override AudioSource DetermineBestAudioSource()
  {
    if (!this.m_SpecificCardFound)
      return base.DetermineBestAudioSource();
    return this.m_ActiveAudioIndex < this.m_CardSpecificVoData.m_Lines.Length ? this.m_CardSpecificVoData.m_Lines[this.m_ActiveAudioIndex].m_AudioSource : (AudioSource) null;
  }

  private bool SearchForCard()
  {
    if (string.IsNullOrEmpty(this.m_CardSpecificVoData.m_CardId))
      return false;
    foreach (SpellZoneTag zoneTag in this.m_CardSpecificVoData.m_ZonesToSearch)
    {
      if (this.IsCardInZones(SpellUtils.FindZonesFromTag((Spell) this, zoneTag, this.m_CardSpecificVoData.m_SideToSearch)))
        return true;
    }
    return false;
  }

  private bool IsCardInZones(List<Zone> zones)
  {
    if (zones == null)
      return false;
    foreach (Zone zone in zones)
    {
      foreach (Card card in zone.GetCards())
      {
        if (card.GetEntity().GetCardId() == this.m_CardSpecificVoData.m_CardId)
          return true;
      }
    }
    return false;
  }

  protected IEnumerator DelayedPlayMulti()
  {
    float delaySec = this.m_CardSpecificVoData.m_Lines[this.m_ActiveAudioIndex].m_DelaySec;
    if ((double) delaySec > 0.0)
      yield return (object) new WaitForSeconds(delaySec);
    this.PlayNowMulti();
  }

  protected IEnumerator WaitForSourceThenContinue()
  {
    CardSpecificMultiVoSpell specificMultiVoSpell = this;
    while (SoundManager.Get().IsActive(specificMultiVoSpell.m_activeAudioSource))
      yield return (object) 0;
    ++specificMultiVoSpell.m_ActiveAudioIndex;
    specificMultiVoSpell.m_activeAudioSource = specificMultiVoSpell.DetermineBestAudioSource();
    if ((Object) specificMultiVoSpell.m_activeAudioSource != (Object) null)
      specificMultiVoSpell.StartCoroutine("DelayedPlayMulti");
    else
      specificMultiVoSpell.OnStateFinished();
  }
}
