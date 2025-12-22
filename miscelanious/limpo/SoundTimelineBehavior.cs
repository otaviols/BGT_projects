using UnityEngine;
using UnityEngine.Playables;

public class SoundTimelineBehavior : PlayableBehaviour
{
  public AudioSource m_AudioSource;
  public float m_RandomPitchMin = 1f;
  public float m_RandomPitchMax = 1f;
  public float m_RandomVolumeMin = 1f;
  public float m_RandomVolumeMax = 1f;

  public override void ProcessFrame(Playable playable, FrameData info, object playerData)
  {
    if (!((Object) this.m_AudioSource == (Object) null))
      return;
    this.m_AudioSource = playerData as AudioSource;
  }

  public override void OnBehaviourPlay(Playable playable, FrameData info) => SoundManager.Get().Play(this.m_AudioSource);

  public override void OnBehaviourPause(Playable playable, FrameData info) => SoundManager.Get().Pause(this.m_AudioSource);
}
