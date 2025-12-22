using System.Collections;

public class ActorAttackSpell : Spell
{
  private bool m_waitingToAct = true;

  protected override void Start() => base.Start();

  protected override void OnBirth(SpellStateType prevStateType)
  {
    this.m_waitingToAct = true;
    base.OnBirth(prevStateType);
  }

  protected override void OnAction(SpellStateType prevStateType) => this.StartCoroutine(this.WaitThenDoAction(prevStateType));

  private void StopWaitingToAct() => this.m_waitingToAct = false;

  protected IEnumerator WaitThenDoAction(SpellStateType prevStateType)
  {
    while (this.m_waitingToAct)
      yield return (object) null;
    base.OnAction(prevStateType);
  }
}
