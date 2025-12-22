using UnityEngine;
using UnityEngine.Playables;

public class SoundTimelineAsset : PlayableAsset
{
  public ExposedReference<AudioSource> m_AudioSource;

  public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
  {
    ScriptPlayable<SoundTimelineBehavior> playable = ScriptPlayable<SoundTimelineBehavior>.Create(graph);
    SoundTimelineBehavior behaviour = playable.GetBehaviour();
    if ((Object) behaviour.m_AudioSource == (Object) null)
      behaviour.m_AudioSource = this.m_AudioSource.Resolve(graph.GetResolver());
    return (Playable) playable;
  }
}
