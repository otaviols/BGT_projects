using UnityEngine;

[CustomEditClass]
public class AdventureBossDef : MonoBehaviour
{
  [CustomEditField(Sections = "Intro Line")]
  public string m_IntroLine;
  [CustomEditField(Sections = "Intro Line")]
  public AdventureBossDef.IntroLinePlayTime m_IntroLinePlayTime;
  [CustomEditField(Sections = "General", T = EditType.GAME_OBJECT)]
  public string m_quotePrefabOverride;
  [CustomEditField(Sections = "General")]
  public MusicPlaylistType m_MissionMusic;
  public MaterialReference m_CoinPortraitMaterial;

  public virtual string GetIntroLine() => this.m_IntroLine;

  public enum IntroLinePlayTime
  {
    MissionSelect,
    MissionStart,
  }
}
