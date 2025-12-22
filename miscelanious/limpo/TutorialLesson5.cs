using UnityEngine;

public class TutorialLesson5 : MonoBehaviour
{
  public UberText m_heroPower;
  public UberText m_used;
  public UberText m_yourTurn;

  private void Start()
  {
    this.m_heroPower.SetText(GameStrings.Get("GLOBAL_TUTORIAL_HERO_POWER"));
    this.m_used.SetText(GameStrings.Get("GLOBAL_TUTORIAL_USED"));
    this.m_yourTurn.SetText(GameStrings.Get("GLOBAL_TUTORIAL_YOUR_TURN"));
  }
}
