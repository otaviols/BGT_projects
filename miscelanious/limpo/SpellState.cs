using Blizzard.T5.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellState : MonoBehaviour
{
  public SpellStateType m_StateType;
  public float m_StartDelaySec;
  public List<SpellStateAnimObject> m_ExternalAnimatedObjects;
  public List<SpellStateAudioSource> m_AudioSources;
  private Spell m_spell;
  private bool m_playing;
  private bool m_initialized;
  private bool m_shown = true;

  private void Start()
  {
    this.m_spell = GameObjectUtils.FindComponentInParents<Spell>(this.gameObject);
    for (int index = 0; index < this.m_ExternalAnimatedObjects.Count; ++index)
      this.m_ExternalAnimatedObjects[index].Init();
    for (int index = 0; index < this.m_AudioSources.Count; ++index)
      this.m_AudioSources[index].Init();
    this.m_initialized = true;
    if (this.m_shown && this.m_playing)
      this.PlayImpl();
    else
      this.StopImpl((List<SpellState>) null);
  }

  public void Play()
  {
    if (this.m_playing || !this.m_shown)
      return;
    this.m_playing = true;
    if (!this.m_initialized)
      return;
    this.PlayImpl();
  }

  public void Stop(List<SpellState> nextStateList)
  {
    if (!this.m_playing)
      return;
    this.m_playing = false;
    if (!this.m_initialized)
      return;
    this.StopImpl(nextStateList);
  }

  public void ShowState()
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    if (!this.m_initialized || !this.m_playing)
      return;
    this.PlayImpl();
  }

  public void HideState()
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    if (!this.m_initialized || !this.m_playing)
      return;
    this.StopImpl((List<SpellState>) null);
  }

  public void OnLoad()
  {
    this.gameObject.SetActive(true);
    foreach (SpellStateAnimObject externalAnimatedObject in this.m_ExternalAnimatedObjects)
      externalAnimatedObject.OnLoad(this);
  }

  public void Reset()
  {
    this.m_playing = false;
    this.m_shown = true;
    this.gameObject.SetActive(false);
  }

  private void OnStateFinished() => this.m_spell.OnStateFinished();

  private void OnSpellFinished() => this.m_spell.OnSpellFinished();

  private void OnChangeState(SpellStateType stateType) => this.m_spell.ChangeState(stateType);

  private IEnumerator DelayedPlay()
  {
    yield return (object) new WaitForSeconds(this.m_StartDelaySec);
    this.PlayNow();
  }

  private void PlayImpl()
  {
    this.gameObject.SetActive(true);
    if (Mathf.Approximately(this.m_StartDelaySec, 0.0f))
      this.PlayNow();
    else
      this.StartCoroutine(this.DelayedPlay());
  }

  private void StopImpl(List<SpellState> nextStateList)
  {
    if (nextStateList == null)
    {
      foreach (SpellStateAnimObject externalAnimatedObject in this.m_ExternalAnimatedObjects)
        externalAnimatedObject.Stop();
    }
    else
    {
      foreach (SpellStateAnimObject externalAnimatedObject in this.m_ExternalAnimatedObjects)
        externalAnimatedObject.Stop(nextStateList);
    }
    foreach (SpellStateAudioSource audioSource in this.m_AudioSources)
      audioSource.Stop();
    this.gameObject.SetActive(false);
  }

  private void PlayNow()
  {
    foreach (SpellStateAnimObject externalAnimatedObject in this.m_ExternalAnimatedObjects)
      externalAnimatedObject.Play();
    foreach (SpellStateAudioSource audioSource in this.m_AudioSources)
      audioSource.Play(this);
  }
}
