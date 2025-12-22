using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SherazinSeedMorphSpell : SuperSpell
{
  private Card m_sherazinCard;
  private int m_newSherazinChangeTaskIndex;
  public Spell m_CustomSpawnSpell;

  public override bool AddPowerTargets()
  {
    if (!this.CanAddPowerTargets() || this.m_taskList.GetBlockType() != HistoryBlock.Type.TRIGGER || !this.FindSherazinChange())
      return false;
    this.m_sherazinCard = this.GetSourceCard();
    return true;
  }

  private bool FindSherazinChange()
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    this.m_newSherazinChangeTaskIndex = -1;
    for (int index = 0; index < taskList.Count; ++index)
    {
      if (taskList[index].GetPower() is Network.HistChangeEntity)
      {
        this.m_newSherazinChangeTaskIndex = index;
        return true;
      }
    }
    return false;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnSpellEvent));
    this.StartCoroutine(this.FlipSeedIntoMinion());
  }

  public void OnSpellEvent(string eventName, object eventData, object userData) => this.StartCoroutine(this.FinishNewSherazinSpawn());

  private IEnumerator FlipSeedIntoMinion()
  {
    SherazinSeedMorphSpell sherazinSeedMorphSpell = this;
    bool complete = false;
    PowerTaskList.CompleteCallback callback = (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true);
    sherazinSeedMorphSpell.m_taskList.DoTasks(0, sherazinSeedMorphSpell.m_newSherazinChangeTaskIndex, callback);
    while (!complete)
      yield return (object) null;
    Spell sherazinLeafSpell = sherazinSeedMorphSpell.m_sherazinCard.GetCustomKeywordSpell();
    while ((Object) sherazinLeafSpell != (Object) null && sherazinLeafSpell.GetActiveState() != SpellStateType.NONE)
      yield return (object) null;
    sherazinSeedMorphSpell.GetComponent<PlayMakerFSM>().SendEvent("DoFlip");
  }

  private IEnumerator FinishNewSherazinSpawn()
  {
    SherazinSeedMorphSpell sherazinSeedMorphSpell = this;
    bool complete = false;
    PowerTaskList.CompleteCallback callback = (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true);
    sherazinSeedMorphSpell.m_taskList.DoTasks(sherazinSeedMorphSpell.m_newSherazinChangeTaskIndex, sherazinSeedMorphSpell.m_taskList.GetTaskList().Count - sherazinSeedMorphSpell.m_newSherazinChangeTaskIndex, callback);
    while (!complete)
      yield return (object) null;
    sherazinSeedMorphSpell.m_sherazinCard.GetActor().transform.localPosition = Vector3.zero;
    Spell spell = SpellManager.Get().GetSpell(sherazinSeedMorphSpell.m_CustomSpawnSpell);
    spell.SetSource(sherazinSeedMorphSpell.m_sherazinCard.gameObject);
    spell.RemoveAllTargets();
    spell.AddTarget(sherazinSeedMorphSpell.m_sherazinCard.gameObject);
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(sherazinSeedMorphSpell.OnCustomSummonSpellFinished));
    SpellUtils.SetCustomSpellParent(spell, (Component) sherazinSeedMorphSpell.m_sherazinCard.GetActor());
    spell.ActivateState(SpellStateType.ACTION);
    sherazinSeedMorphSpell.OnSpellFinished();
    sherazinSeedMorphSpell.OnStateFinished();
  }

  private void OnCustomSummonSpellFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    SpellManager.Get().ReleaseSpell(spell);
  }
}
