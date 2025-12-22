using Assets;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SpellStateAudioSource
{
  public AudioSource m_AudioSource;
  public float m_StartDelaySec;
  public bool m_PlayGlobally;
  public bool m_StopOnStateChange;
  public string m_Comment;
  public bool m_Enabled = true;

  public void Init()
  {
    if ((UnityEngine.Object) this.m_AudioSource == (UnityEngine.Object) null)
      return;
    this.m_AudioSource.playOnAwake = false;
  }

  public void Play(SpellState parent)
  {
    if (!this.m_Enabled)
      return;
    if (Mathf.Approximately(this.m_StartDelaySec, 0.0f))
      this.PlayNow();
    else
      parent.StartCoroutine(this.DelayedPlay());
  }

  public void Stop()
  {
    if (!this.m_Enabled || (UnityEngine.Object) this.m_AudioSource == (UnityEngine.Object) null || this.m_PlayGlobally || !this.m_StopOnStateChange)
      return;
    this.m_AudioSource.Stop();
  }

  private IEnumerator DelayedPlay()
  {
    yield return (object) new WaitForSeconds(this.m_StartDelaySec);
    this.PlayNow();
  }

  private void PlayNow()
  {
    if ((UnityEngine.Object) this.m_AudioSource == (UnityEngine.Object) null)
      return;
    if (this.m_PlayGlobally)
      SoundManager.Get().PlayClip(new SoundPlayClipArgs()
      {
        m_def = SoundManager.Get().GetSoundDef(this.m_AudioSource),
        m_volume = new float?(this.m_AudioSource.volume),
        m_pitch = new float?(this.m_AudioSource.pitch),
        m_category = new Global.SoundCategory?(SoundManager.Get().GetCategory(this.m_AudioSource)),
        m_parentObject = this.m_AudioSource.gameObject
      });
    else
      SoundManager.Get().Play(this.m_AudioSource);
  }
}
