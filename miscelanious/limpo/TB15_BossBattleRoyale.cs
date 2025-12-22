using System.Collections;

public class TB15_BossBattleRoyale : MissionEntity
{
  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB15_BossBattleRoyale bossBattleRoyale = this;
    while (bossBattleRoyale.m_enemySpeaking)
      yield return (object) null;
  }

  public TB15_BossBattleRoyale()
    : base()
  {
  }
}
