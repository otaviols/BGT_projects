using UnityEngine;

public class TutorialLesson1 : MonoBehaviour
{
  public UberText m_health;
  public UberText m_attack;
  public UberText m_minion;

  private void Start()
  {
    this.m_health.SetText(GameStrings.Get("GLOBAL_TUTORIAL_HEALTH"));
    this.m_attack.SetText(GameStrings.Get("GLOBAL_TUTORIAL_ATTACK"));
    this.m_minion.SetText(GameStrings.Get("GLOBAL_TUTORIAL_MINION"));
  }
}
