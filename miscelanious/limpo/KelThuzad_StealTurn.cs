using System.Collections;
using UnityEngine;

public class KelThuzad_StealTurn : Spell
{
  public GameObject m_Lightning;

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.StartCoroutine(this.SpellEffect(prevStateType));
    base.OnAction(prevStateType);
  }

  private IEnumerator SpellEffect(SpellStateType prevStateType)
  {
    yield return (object) new WaitForSeconds(0.25f);
    if ((Object) TurnTimer.Get() != (Object) null)
      TurnTimer.Get().OnEndTurnRequested();
    Animation endTurnButtonMeshAnimation = EndTurnButton.Get().m_EndTurnButtonMesh.GetComponent<Animation>();
    endTurnButtonMeshAnimation["ENDTURN_YOUR_TIMER_DONE"].speed = 0.7f;
    EndTurnButton.Get().OnTurnTimerEnded(true);
    yield return (object) new WaitForSeconds(1f);
    endTurnButtonMeshAnimation["ENDTURN_YOUR_TIMER_DONE"].speed = 1f;
  }
}
