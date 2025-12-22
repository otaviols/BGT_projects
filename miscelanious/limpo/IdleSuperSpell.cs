using System.Collections;
using UnityEngine;

public class IdleSuperSpell : SuperSpell
{
  public Spell m_idleSpell;
  public float m_waitTimeBeforeSuperSpellVisuals = 1.5f;
  public float m_minTimeIdleIsPlaying = 1.5f;
  public bool m_playIdleSpellWithoutTargets;
  private Spell m_idleSpellInstance;
  private bool m_playSuperSpellVisuals;
  private bool m_hasIdlePlayedForMinTime = true;

  public override bool AddPowerTargets()
  {
    this.m_playSuperSpellVisuals = base.AddPowerTargets();
    return true;
  }

  protected virtual void DoActionPreTasks()
  {
  }

  protected virtual void DoActionPostTasks()
  {
  }

  protected virtual bool HasPendingTasks() => false;

  protected override void OnAction(SpellStateType prevStateType)
  {
    if ((Object) this.m_idleSpellInstance == (Object) null && (this.m_targets.Count > 0 || this.m_playIdleSpellWithoutTargets))
    {
      this.m_hasIdlePlayedForMinTime = false;
      this.StartCoroutine(this.DoIdleSpell(prevStateType));
    }
    else
    {
      this.DoActionPreTasks();
      if (this.m_playSuperSpellVisuals)
        base.OnAction(prevStateType);
      else
        this.OnStateFinished();
      this.DoActionPostTasks();
    }
  }

  public override bool CanPurge() => (this.m_taskList == null || this.m_taskList.IsEndOfBlock()) && (!((Object) this.m_idleSpellInstance != (Object) null) || !this.m_idleSpellInstance.IsActive());

  public override bool ShouldReconnectIfStuck() => false;

  private IEnumerator DoIdleSpell(SpellStateType prevStateType)
  {
    IdleSuperSpell c = this;
    Actor actor = c.GetSourceCard().GetActor();
    c.m_idleSpellInstance = SpellManager.Get().GetSpell(c.m_idleSpell);
    SpellUtils.SetCustomSpellParent(c.m_idleSpellInstance, (Component) c);
    if ((Object) actor != (Object) null)
      TransformUtil.AttachAndPreserveLocalTransform(c.m_idleSpellInstance.transform, actor.transform);
    c.m_idleSpellInstance.SetSource(c.GetSource());
    c.m_idleSpellInstance.AddFinishedCallback(new Spell.FinishedCallback(c.OnIdleSpellFinished));
    c.m_idleSpellInstance.Activate();
    yield return (object) new WaitForSeconds(c.m_waitTimeBeforeSuperSpellVisuals);
    c.DoActionPreTasks();
    if (c.m_playSuperSpellVisuals)
    {
      // ISSUE: reference to a compiler-generated method
      c.\u003C\u003En__0(prevStateType);
    }
    else
      c.OnStateFinished();
    c.DoActionPostTasks();
    yield return (object) new WaitForSeconds(c.m_minTimeIdleIsPlaying);
    c.m_hasIdlePlayedForMinTime = true;
    while (!c.TryIdleFinish())
      yield return (object) null;
  }

  private void OnIdleSpellFinished(Spell spell, object userData)
  {
  }

  public override void OnSpellFinished()
  {
    this.TryIdleFinish();
    base.OnSpellFinished();
  }

  private bool TryIdleFinish()
  {
    if (this.m_taskList != null && !this.m_taskList.IsEndOfBlock())
    {
      for (PowerTaskList powerTaskList = this.m_taskList; powerTaskList != null; powerTaskList = powerTaskList.GetNext())
      {
        if (powerTaskList.HasTasks() && !powerTaskList.AreTasksComplete())
          return false;
      }
    }
    if (this.HasPendingTasks())
      return false;
    if ((Object) this.m_idleSpellInstance == (Object) null || this.m_idleSpellInstance.GetActiveState() == SpellStateType.DEATH || this.m_idleSpellInstance.GetActiveState() == SpellStateType.NONE)
      return true;
    if (!this.m_hasIdlePlayedForMinTime)
      return false;
    this.m_idleSpellInstance.ActivateState(SpellStateType.DEATH);
    return true;
  }
}
