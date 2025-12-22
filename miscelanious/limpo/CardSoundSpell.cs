using System.Collections;
using UnityEngine;

public class CardSoundSpell : Spell
{
  public CardSoundData m_CardSoundData = new CardSoundData();
  protected AudioSource m_activeAudioSource;
  protected bool m_forceDefaultAudioSource;

  protected override void OnBirth(SpellStateType prevStateType)
  {
    base.OnBirth(prevStateType);
    this.Play();
  }

  protected override void OnNone(SpellStateType prevStateType)
  {
    base.OnNone(prevStateType);
    this.Stop();
  }

  public AudioSource GetActiveAudioSource() => this.m_activeAudioSource;

  public void ForceDefaultAudioSource() => this.m_forceDefaultAudioSource = true;

  public bool HasActiveAudioSource() => (Object) this.m_activeAudioSource != (Object) null;

  public virtual AudioSource DetermineBestAudioSource() => this.m_CardSoundData.m_AudioSource;

  public virtual string DetermineGameStringKey() => "";

  protected virtual void Play()
  {
    this.Stop();
    this.m_activeAudioSource = this.m_forceDefaultAudioSource ? this.m_CardSoundData.m_AudioSource : this.DetermineBestAudioSource();
    if ((Object) this.m_activeAudioSource == (Object) null)
      this.OnStateFinished();
    else
      this.StartCoroutine("DelayedPlay");
  }

  protected virtual void PlayNow()
  {
    SoundManager.Get().Play(this.m_activeAudioSource);
    this.StartCoroutine("WaitForSourceThenFinishState");
  }

  protected virtual void Stop()
  {
    this.StopCoroutine("DelayedPlay");
    this.StopCoroutine("WaitForSourceThenFinishState");
    SoundManager.Get().Stop(this.m_activeAudioSource);
  }

  protected IEnumerator DelayedPlay()
  {
    if ((double) this.m_CardSoundData.m_DelaySec > 0.0)
      yield return (object) new WaitForSeconds(this.m_CardSoundData.m_DelaySec);
    this.PlayNow();
  }

  protected IEnumerator WaitForSourceThenFinishState()
  {
    CardSoundSpell cardSoundSpell = this;
    while (SoundManager.Get().IsActive(cardSoundSpell.m_activeAudioSource))
      yield return (object) 0;
    cardSoundSpell.OnStateFinished();
  }
}
