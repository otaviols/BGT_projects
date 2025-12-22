using System.Collections;
using UnityEngine;

public class RitualActivateSpell : SuperSpell
{
  public RitualSpellConfig m_ritualSpellConfig;
  public float m_minTimeRitualActivateSpellPlays = 2f;
  private bool m_playSuperSpellVisuals;
  private bool m_isRitualPortalOpenForMinTime = true;
  private bool m_willShowRitualActorVisuals = true;
  private bool m_hasRitualTriggerSpell;
  private Entity m_proxyRitualEntity;
  private Actor m_proxyRitualActor;
  private Spell m_ritualPortalSpellInstance;

  public void SetHasRitualTriggerSpell(bool hasSpell) => this.m_hasRitualTriggerSpell = hasSpell;

  public override bool AddPowerTargets()
  {
    this.m_playSuperSpellVisuals = base.AddPowerTargets();
    Player controller = this.m_taskList.GetSourceEntity().GetController();
    if (!this.m_ritualSpellConfig.m_showRitualVisualsInPlay && this.m_ritualSpellConfig.IsRitualEntityInPlay(controller))
      this.m_willShowRitualActorVisuals = false;
    int tag = controller.GetTag(this.m_ritualSpellConfig.m_proxyRitualEntityTag);
    this.m_proxyRitualEntity = GameState.Get().GetEntity(tag);
    if (this.m_proxyRitualEntity == null)
    {
      Log.Spells.PrintError("RitualActivateSpell.AddPowerTargets(): Failed to get proxy ritual entity. Unable to display visuals. Proxy ritual entity ID: {0}, Proxy ritual entity tag: {1}", (object) tag, (object) this.m_ritualSpellConfig.m_proxyRitualEntityTag);
      this.m_willShowRitualActorVisuals = false;
    }
    if (this.m_taskList.IsOrigin())
    {
      if (this.m_ritualSpellConfig.DoesTaskListContainRitualEntity(this.m_taskList, tag))
        this.m_willShowRitualActorVisuals = false;
      else if (this.m_ritualSpellConfig.DoesFutureTaskListContainsRitualEntity(GameState.Get().GetPowerProcessor().GetPowerQueue().GetList(), this.m_taskList, tag))
        this.m_willShowRitualActorVisuals = false;
    }
    return true;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    if ((Object) this.m_ritualPortalSpellInstance == (Object) null && this.m_willShowRitualActorVisuals && this.InitPortalEffect())
    {
      this.m_isRitualPortalOpenForMinTime = false;
      this.StartCoroutine(this.DoPortalEffect());
    }
    if (this.m_playSuperSpellVisuals)
      base.OnAction(prevStateType);
    else
      this.OnStateFinished();
  }

  public override bool CanPurge() => (this.m_taskList == null || this.m_taskList.IsEndOfBlock()) && (!((Object) this.m_ritualPortalSpellInstance != (Object) null) || !this.m_ritualPortalSpellInstance.IsActive());

  public override void OnSpellFinished()
  {
    this.TryPortalClose();
    if ((((Object) this.m_ritualPortalSpellInstance != (Object) null ? 1 : (this.m_hasRitualTriggerSpell ? 1 : 0)) & (this.m_taskList == null ? (false ? 1 : 0) : (this.m_taskList.IsEndOfBlock() ? 1 : 0))) != 0)
      return;
    base.OnSpellFinished();
  }

  private bool InitPortalEffect()
  {
    Spell ritualActivateSpell = this.m_ritualSpellConfig.GetRitualActivateSpell(this.m_proxyRitualEntity);
    if ((Object) ritualActivateSpell == (Object) null)
      return false;
    this.m_proxyRitualActor = this.m_ritualSpellConfig.LoadRitualActor(this.m_proxyRitualEntity);
    if ((Object) this.m_proxyRitualActor == (Object) null)
      return false;
    this.m_ritualSpellConfig.UpdateAndPositionActor(this.m_proxyRitualActor);
    this.m_ritualPortalSpellInstance = SpellManager.Get().GetSpell(ritualActivateSpell);
    SpellUtils.SetCustomSpellParent(this.m_ritualPortalSpellInstance, (Component) this);
    this.m_ritualPortalSpellInstance.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnPortalSpellEvent));
    this.m_ritualPortalSpellInstance.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnPortalSpellStateFinished));
    TransformUtil.AttachAndPreserveLocalTransform(this.m_ritualPortalSpellInstance.transform, this.m_proxyRitualActor.transform);
    this.m_ritualSpellConfig.UpdateRitualActorComponents(this.m_proxyRitualActor);
    return true;
  }

  private IEnumerator DoPortalEffect()
  {
    this.m_ritualPortalSpellInstance.Activate();
    yield return (object) new WaitForSeconds(this.m_minTimeRitualActivateSpellPlays);
    this.m_isRitualPortalOpenForMinTime = true;
    this.TryPortalClose();
  }

  private void OnPortalSpellEvent(string eventName, object eventData, object userData)
  {
    if (eventName != this.m_ritualSpellConfig.m_portalSpellEventName)
    {
      Log.Spells.PrintError("RitualActivateSpell received unexpected Spell Event {0}. Expected {1}", (object) eventName, (object) this.m_ritualSpellConfig.m_portalSpellEventName);
    }
    else
    {
      if (!this.m_ritualSpellConfig.m_hideRitualActor)
        return;
      this.m_proxyRitualActor.Show();
    }
  }

  public void OnPortalSpellFinished() => base.OnSpellFinished();

  private void OnPortalSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    Object.Destroy((Object) this.m_proxyRitualActor.gameObject);
    this.OnPortalSpellFinished();
  }

  private void TryPortalClose()
  {
    if (this.m_taskList != null && !this.m_taskList.IsEndOfBlock())
    {
      for (PowerTaskList powerTaskList = this.m_taskList; powerTaskList != null; powerTaskList = powerTaskList.GetNext())
      {
        if (powerTaskList.HasTasks())
          return;
      }
    }
    if ((Object) this.m_ritualPortalSpellInstance == (Object) null || this.m_ritualPortalSpellInstance.GetActiveState() == SpellStateType.DEATH || this.m_ritualPortalSpellInstance.GetActiveState() == SpellStateType.NONE || !this.m_isRitualPortalOpenForMinTime)
      return;
    this.m_ritualPortalSpellInstance.ActivateState(SpellStateType.DEATH);
  }
}
