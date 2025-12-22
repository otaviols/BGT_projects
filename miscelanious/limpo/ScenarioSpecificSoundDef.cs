using System;
using System.Collections.Generic;

[CustomEditClass]
public class ScenarioSpecificSoundDef : SoundDef, IMultipleRandomClipSoundDef
{
  public List<ScenarioSpecificSoundDef.ScenarioClipPair> m_ScenarioSpecificRandomClips = new List<ScenarioSpecificSoundDef.ScenarioClipPair>();

  public List<RandomAudioClip> GetRandomAudioClips()
  {
    ScenarioDbId currentScenario = GameMgr.Get() != null ? (ScenarioDbId) GameMgr.Get().GetMissionId() : ScenarioDbId.MULTIPLAYER_1v1;
    ScenarioSpecificSoundDef.ScenarioClipPair scenarioClipPair = this.m_ScenarioSpecificRandomClips.Find((Predicate<ScenarioSpecificSoundDef.ScenarioClipPair>) (pair => pair.m_ScenarioID == currentScenario));
    return scenarioClipPair == null ? this.m_RandomClips : scenarioClipPair.m_RandomClips;
  }

  [CustomEditClass]
  [Serializable]
  public class ScenarioClipPair
  {
    public ScenarioDbId m_ScenarioID;
    public List<RandomAudioClip> m_RandomClips;
  }
}
