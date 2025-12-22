using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualTriggerSpell : SuperSpell
{
  public RitualSpellConfig m_ritualSpellConfig;
  public float m_minTimeRitualTriggerSpellPlays = 2f;
  private Entity m_proxyRitualEntity;
  private Actor m_proxyRitualActor;
  private Spell m_ritualPortalSpellInstance;
  private RitualActivateSpell m_linkedSpellInstance;

  public override bool AddPowerTargets()
  {
    Player controller = this.m_taskList.GetSourceEntity().GetController();
    if (!this.m_ritualSpellConfig.m_showRitualVisualsInPlay && this.m_ritualSpellConfig.IsRitualEntityInPlay(controller))
      return false;
    int tag = controller.GetTag(this.m_ritualSpellConfig.m_proxyRitualEntityTag);
    this.m_proxyRitualEntity = GameState.Get().GetEntity(tag);
    if (this.m_proxyRitualEntity == null)
    {
      Log.Spells.PrintError("RitualTriggerSpell.AddPowerTargets(): Failed to get proxy ritual entity. Unable to display visuals. Proxy ritual entity ID: {0}, Proxy ritual entity tag: {1}", (object) tag, (object) this.m_ritualSpellConfig.m_proxyRitualEntityTag);
      return false;
    }
    return this.m_ritualSpellConfig.DoesTaskListContainRitualEntity(this.m_taskList, tag) && base.AddPowerTargets();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    if (!this.InitPortalEffect())
      return;
    this.m_linkedSpellInstance = this.GetRitualActivateSpell();
    if ((Object) this.m_linkedSpellInstance != (Object) null)
      this.m_linkedSpellInstance.SetHasRitualTriggerSpell(true);
    this.StartCoroutine(this.DoPortalAndTransformEffect());
  }

  private RitualActivateSpell GetRitualActivateSpell()
  {
    for (PowerTaskList taskList = this.m_taskList; taskList != null; taskList = taskList.GetParent())
    {
      if (taskList.GetBlockType() == HistoryBlock.Type.POWER)
      {
        CardEffect effect = PowerSpellController.GetOrCreateEffect(taskList.GetSourceEntity().GetCard(), taskList);
        if (effect != null)
        {
          RitualActivateSpell spell = effect.GetSpell() as RitualActivateSpell;
          if ((Object) spell != (Object) null)
            return spell;
        }
      }
    }
    return (RitualActivateSpell) null;
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

  private IEnumerator DoPortalAndTransformEffect()
  {
    RitualTriggerSpell ritualTriggerSpell = this;
    ritualTriggerSpell.m_ritualPortalSpellInstance.Activate();
    bool complete = false;
    PowerTaskList.CompleteCallback callback = (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true);
    ritualTriggerSpell.m_taskList.DoTasks(0, ritualTriggerSpell.m_taskList.GetTaskList().Count, callback);
    yield return (object) new WaitForSeconds(ritualTriggerSpell.m_minTimeRitualTriggerSpellPlays);
    while (!complete)
      yield return (object) null;
    Spell spell = ritualTriggerSpell.ActivateTransformSpell();
    while ((Object) spell != (Object) null && !spell.IsFinished())
      yield return (object) null;
    ritualTriggerSpell.m_proxyRitualActor.SetEntity(ritualTriggerSpell.m_proxyRitualEntity);
    ritualTriggerSpell.m_proxyRitualActor.SetCardDefFromEntity(ritualTriggerSpell.m_proxyRitualEntity);
    ritualTriggerSpell.m_proxyRitualActor.UpdateAllComponents();
    ritualTriggerSpell.OnSpellFinished();
    ritualTriggerSpell.OnStateFinished();
    PowerTaskList targetTaskList = ritualTriggerSpell.m_taskList;
    if ((Object) ritualTriggerSpell.m_linkedSpellInstance != (Object) null)
      targetTaskList = ritualTriggerSpell.m_linkedSpellInstance.GetPowerTaskList();
    while (!ritualTriggerSpell.CanClosePortal(targetTaskList))
      yield return (object) null;
    ritualTriggerSpell.m_ritualPortalSpellInstance.ActivateState(SpellStateType.DEATH);
  }

  public bool CanClosePortal(PowerTaskList targetTaskList)
  {
    List<PowerTaskList> list = GameState.Get().GetPowerProcessor().GetPowerQueue().GetList();
    if (list.Count == 0)
      return true;
    PowerTaskList powerTaskList = list[0];
    return powerTaskList == null || !powerTaskList.IsDescendantOfBlock(targetTaskList);
  }

  private void OnPortalSpellEvent(string eventName, object eventData, object userData)
  {
    if (eventName != this.m_ritualSpellConfig.m_portalSpellEventName)
    {
      Log.Spells.PrintError("RitualTriggerSpell received unexpected Spell Event {0}. Expected {1}", (object) eventName, (object) this.m_ritualSpellConfig.m_portalSpellEventName);
    }
    else
    {
      if (!this.m_ritualSpellConfig.m_hideRitualActor)
        return;
      this.m_proxyRitualActor.Show();
    }
  }

  private void OnPortalSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    Object.Destroy((Object) this.m_proxyRitualActor.gameObject);
    if (!((Object) this.m_linkedSpellInstance != (Object) null))
      return;
    this.m_linkedSpellInstance.SetHasRitualTriggerSpell(false);
    this.m_linkedSpellInstance.OnPortalSpellFinished();
  }

  private Spell ActivateTransformSpell()
  {
    Spell ritualTriggerSpell = this.m_ritualSpellConfig.GetRitualTriggerSpell(this.m_proxyRitualEntity);
    if ((Object) ritualTriggerSpell == (Object) null)
      return (Spell) null;
    Spell spell = SpellManager.Get().GetSpell(ritualTriggerSpell);
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnTransformSpellStateFinished));
    this.UpdateAndPositionTransformSpell(spell);
    SpellUtils.SetCustomSpellParent(spell, (Component) this.m_proxyRitualActor);
    TransformUtil.AttachAndPreserveLocalTransform(spell.transform, this.m_proxyRitualActor.transform);
    spell.ActivateState(SpellStateType.ACTION);
    return spell;
  }

  private void OnTransformSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    SpellManager.Get().ReleaseSpell(spell);
  }

  public override bool CanPurge() => (this.m_taskList == null || this.m_taskList.IsEndOfBlock()) && (!((Object) this.m_ritualPortalSpellInstance != (Object) null) || !this.m_ritualPortalSpellInstance.IsActive());

  private void UpdateAndPositionActor(Actor actor)
  {
    if ((Object) actor == (Object) null)
      return;
    if (this.m_ritualSpellConfig.m_hideRitualActor)
      actor.Hide();
    Transform bone = Board.Get().FindBone(this.GetRitualBoneName());
    actor.transform.parent = bone;
    actor.transform.localPosition = Vector3.zero;
  }

  private void UpdateAndPositionTransformSpell(Spell spell)
  {
    if ((Object) spell == (Object) null)
      return;
    Transform bone = Board.Get().FindBone(this.GetRitualBoneName());
    spell.transform.parent = bone;
    spell.transform.localPosition = Vector3.zero;
  }

  private string GetRitualBoneName()
  {
    if (this.m_proxyRitualEntity == null)
      return string.Empty;
    return this.m_proxyRitualEntity.GetControllerSide() != Player.Side.FRIENDLY ? this.m_ritualSpellConfig.m_opponentBoneName : this.m_ritualSpellConfig.m_friendlyBoneName;
  }
}
