using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSpellController : SpellController
{
  private Stack<SubSpellController.SubSpellInstance> m_subSpellInstanceStack = new Stack<SubSpellController.SubSpellInstance>();

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    SubSpellController.SubSpellInstance instanceForTasklist = this.GetSubSpellInstanceForTasklist(taskList);
    if (instanceForTasklist == null)
      return false;
    if (!SpellUtils.CanAddPowerTargets(taskList))
    {
      this.CheckForSubSpellEnd(taskList);
      return false;
    }
    Network.HistSubSpellStart subSpellStartTask = instanceForTasklist.SubSpellStartTask;
    Entity entity1 = taskList.GetSourceEntity();
    if (subSpellStartTask.SourceEntityID != 0)
      entity1 = GameState.Get().GetEntity(subSpellStartTask.SourceEntityID);
    Card card1 = entity1?.GetCard();
    this.SetSource(card1);
    if (subSpellStartTask.TargetEntityIDS.Count > 0)
    {
      foreach (int id in subSpellStartTask.TargetEntityIDS)
      {
        Entity entity2 = GameState.Get().GetEntity(id);
        if (entity2 != null)
        {
          Card card2 = entity2.GetCard();
          if (!((Object) card2 == (Object) null) && !((Object) card1 == (Object) card2) && !this.IsTarget(card2))
            this.AddTarget(card2);
        }
      }
    }
    else
    {
      List<PowerTask> taskList1 = this.m_taskList.GetTaskList();
      for (int index = 0; index < taskList1.Count; ++index)
      {
        Card cardFromPowerTask = this.GetTargetCardFromPowerTask(taskList1[index]);
        if (!((Object) cardFromPowerTask == (Object) null) && !((Object) card1 == (Object) cardFromPowerTask) && !this.IsTarget(cardFromPowerTask))
          this.AddTarget(cardFromPowerTask);
      }
    }
    int num = (Object) card1 != (Object) null || this.m_targets.Count > 0 ? 1 : (entity1 == null ? 0 : (entity1.IsGame() ? 1 : 0));
    if (num != 0)
      return num != 0;
    this.CheckForSubSpellEnd(taskList);
    return num != 0;
  }

  private SubSpellController.SubSpellInstance GetSubSpellInstanceForTasklist(
    PowerTaskList taskList)
  {
    SubSpellController.SubSpellInstance callbackData = (SubSpellController.SubSpellInstance) null;
    Network.HistSubSpellStart subSpellStart = taskList.GetSubSpellStart();
    if (subSpellStart != null)
    {
      callbackData = new SubSpellController.SubSpellInstance();
      callbackData.SubSpellStartTask = subSpellStart;
      this.m_subSpellInstanceStack.Push(callbackData);
      string spellPrefabGuid = subSpellStart.SpellPrefabGUID;
      AssetLoader.Get().InstantiatePrefab(new AssetReference(spellPrefabGuid), new PrefabCallback<GameObject>(this.OnSubSpellLoaded), (object) callbackData);
    }
    else if (this.m_subSpellInstanceStack.Count > 0)
      callbackData = this.m_subSpellInstanceStack.Peek();
    return callbackData;
  }

  public void OnSubSpellLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    SubSpellController.SubSpellInstance subSpellInstance = (SubSpellController.SubSpellInstance) callbackData;
    subSpellInstance.SpellLoaded = true;
    if (this.m_subSpellInstanceStack.Count <= 0)
      Log.Power.PrintError("{0}.OnSubSpellLoaded(): Loaded GameObject without an active sub-spell! GameObject: {1}", (object) this, (object) go);
    else if (!this.m_subSpellInstanceStack.Contains(subSpellInstance))
      Log.Power.PrintError("{0}.OnSubSpellLoaded(): SubSpellInstance is not on the active sub-spell stack! GameObject: {1}", (object) this, (object) go);
    else if ((Object) go == (Object) null)
    {
      Log.Power.PrintError("{0}.OnSubSpellLoaded(): Failed to load spell prefab! Prefab GUID: {1}", (object) this, (object) subSpellInstance.SubSpellStartTask.SpellPrefabGUID);
    }
    else
    {
      Spell component = go.GetComponent<Spell>();
      if ((Object) component == (Object) null)
      {
        Object.Destroy((Object) go);
        Log.Power.PrintError("{0}.OnSubSpellLoaded(): Loaded spell prefab doesn't have a Spell component! Spell Prefab: {1}", (object) this, (object) go);
      }
      else if ((Object) subSpellInstance.SpellInstance != (Object) null)
      {
        Object.Destroy((Object) go);
        Log.Power.PrintError("{0}.OnSubSpellLoaded(): Active SubSpellInstance already has an existing spell. Existing Spell: {1}, New Spell: {2}", (object) this, (object) subSpellInstance.SpellInstance, (object) component);
      }
      else
      {
        component.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSubSpellStateFinished));
        subSpellInstance.SpellInstance = component;
      }
    }
  }

  protected override void OnProcessTaskList()
  {
    if (this.m_subSpellInstanceStack.Count <= 0)
    {
      Log.Spells.PrintError("{0}.OnProcessTaskList(): No active sub-spell!", (object) this);
      this.OnFinishedTaskList();
    }
    else
      this.StartCoroutine(this.WaitForSubSpellThenDoTaskList());
  }

  private IEnumerator WaitForSubSpellThenDoTaskList()
  {
    SubSpellController subSpellController = this;
    SubSpellController.SubSpellInstance subSpellInstance = subSpellController.m_subSpellInstanceStack.Peek();
    while (!subSpellInstance.SpellLoaded)
      yield return (object) null;
    if (!subSpellController.AttachTasklistToSubSpell(subSpellController.m_taskList, subSpellInstance))
    {
      subSpellController.CheckForSubSpellEnd(subSpellController.m_taskList);
      subSpellController.OnFinishedTaskList();
    }
    else
    {
      if (GameState.Get().IsTurnStartManagerActive())
      {
        TurnStartManager.Get().NotifyOfTriggerVisual();
        while (TurnStartManager.Get().IsTurnStartIndicatorShowing())
          yield return (object) null;
      }
      subSpellInstance.SpellInstance.AddFinishedCallback(new Spell.FinishedCallback(subSpellController.OnSubSpellFinished));
      subSpellInstance.SpellInstance.ActivateState(SpellStateType.ACTION);
    }
  }

  private bool AttachTasklistToSubSpell(
    PowerTaskList taskList,
    SubSpellController.SubSpellInstance subSpellInstance)
  {
    if ((Object) subSpellInstance.SpellInstance == (Object) null)
      return false;
    Spell spellInstance = subSpellInstance.SpellInstance;
    Card card = taskList.GetSourceEntity()?.GetCard();
    if ((Object) card != (Object) null)
      spellInstance.SetSource(card.gameObject);
    Network.HistSubSpellStart subSpellStartTask = subSpellInstance.SubSpellStartTask;
    if (!spellInstance.AttachPowerTaskList(taskList))
      return false;
    if (subSpellStartTask.SourceEntityID != 0)
    {
      Entity entity = GameState.Get().GetEntity(subSpellStartTask.SourceEntityID);
      if ((Object) entity.GetCard() != (Object) null)
      {
        spellInstance.SetSource(entity.GetCard().gameObject);
        spellInstance.m_Location = SpellLocation.SOURCE;
      }
    }
    else
    {
      Card source = this.GetSource();
      if ((Object) source != (Object) null)
        spellInstance.SetSource(source.gameObject);
    }
    if (subSpellStartTask.TargetEntityIDS.Count > 0)
    {
      spellInstance.RemoveAllTargets();
      spellInstance.RemoveAllVisualTargets();
      if (spellInstance is SuperSpell)
        (spellInstance as SuperSpell).m_TargetInfo.m_Behavior = SpellTargetBehavior.DEFAULT;
      foreach (int id in subSpellStartTask.TargetEntityIDS)
      {
        Entity entity = GameState.Get().GetEntity(id);
        if (entity != null && (Object) entity.GetCard() != (Object) null)
          spellInstance.AddTarget(entity.GetCard().gameObject);
      }
    }
    return true;
  }

  private void OnSubSpellFinished(Spell spell, object userData)
  {
    this.CheckForSubSpellEnd(spell.GetPowerTaskList());
    this.OnFinishedTaskList();
  }

  private void CheckForSubSpellEnd(PowerTaskList taskList)
  {
    if (taskList.GetSubSpellEnd() == null)
      return;
    if (this.m_subSpellInstanceStack.Count <= 0)
      Log.Spells.PrintError("{0}.CheckForSubSpellEnd(): SubSpellEnd task hit without an active sub-spell!", (object) this);
    else
      this.m_subSpellInstanceStack.Pop();
  }

  private void OnSubSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    foreach (SubSpellController.SubSpellInstance subSpellInstance in this.m_subSpellInstanceStack.ToArray())
    {
      if ((Object) subSpellInstance.SpellInstance == (Object) spell)
        return;
    }
    if (spell is SuperSpell && (spell as SuperSpell).m_SkipAutoDestroyForSubspell)
      return;
    this.StartCoroutine(this.DestroySpellAfterDelay(spell));
  }

  private IEnumerator DestroySpellAfterDelay(Spell spell)
  {
    yield return (object) new WaitForSeconds(10f);
    if ((Object) spell != (Object) null && (Object) spell.gameObject != (Object) null)
      SpellManager.Get().ReleaseSpell(spell);
  }

  private class SubSpellInstance
  {
    public Network.HistSubSpellStart SubSpellStartTask;
    public Spell SpellInstance;
    public bool SpellLoaded;
  }
}
