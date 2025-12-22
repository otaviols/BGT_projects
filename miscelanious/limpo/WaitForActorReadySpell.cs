using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForActorReadySpell : Spell
{
  public float m_timeoutSeconds;
  public bool m_useTimeout;
  public float m_secondsDelayAfterActorTransition;
  private bool m_isContinueFired;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.ContinueAfterActorReady());
    if (!this.m_useTimeout)
      return;
    this.StartCoroutine(this.ContinueAfterTimeOut());
  }

  private IEnumerator ContinueAfterTimeOut()
  {
    yield return (object) new WaitForSeconds(this.m_timeoutSeconds);
    this.Continue();
  }

  private IEnumerator ContinueAfterActorReady()
  {
    WaitForActorReadySpell forActorReadySpell = this;
    List<Card> cards = new List<Card>();
    Card component1 = forActorReadySpell.m_source.GetComponent<Card>();
    if ((Object) component1 != (Object) null)
      cards.Add(component1);
    foreach (GameObject target in forActorReadySpell.m_targets)
    {
      Card component2 = target.GetComponent<Card>();
      if ((Object) component2 != (Object) null)
        cards.Add(component2);
    }
    bool hasActorTransition = false;
    if (!forActorReadySpell.AreActorsReady(cards))
    {
      hasActorTransition = true;
      yield return (object) null;
    }
    if (hasActorTransition && (double) forActorReadySpell.m_secondsDelayAfterActorTransition > 0.0)
      yield return (object) new WaitForSeconds(forActorReadySpell.m_secondsDelayAfterActorTransition);
    forActorReadySpell.Continue();
  }

  private bool AreActorsReady(List<Card> cards)
  {
    foreach (Card card in cards)
    {
      if (!card.IsActorReady() || !card.IsTransitioningZones())
        return false;
    }
    return true;
  }

  private void Continue()
  {
    if (this.m_isContinueFired)
      return;
    this.m_isContinueFired = true;
    this.OnStateFinished();
  }
}
