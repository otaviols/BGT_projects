using System.Collections.Generic;
using UnityEngine;

public class KarazhanPortalSpell : IdleSuperSpell
{
  public Spell m_customSpawnSpell;
  private bool m_waitForSpawnSpell;
  private Spell m_spawnSpellInstance;
  private Card m_spawnedMinion;
  private bool m_willSummonAMinion;

  public KarazhanPortalSpell() => this.m_playIdleSpellWithoutTargets = true;

  protected override void DoActionPreTasks()
  {
    this.m_willSummonAMinion = false;
    if (!((Object) this.m_spawnedMinion == (Object) null))
      return;
    this.m_spawnedMinion = this.GetSpawnedMinion();
    if (!((Object) this.m_spawnedMinion != (Object) null))
      return;
    this.m_waitForSpawnSpell = true;
    this.m_spawnSpellInstance = SpellManager.Get().GetSpell(this.m_customSpawnSpell);
    this.m_spawnSpellInstance.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnSpawnSpellEvent));
    this.m_spawnedMinion.OverrideCustomSpawnSpell(this.m_spawnSpellInstance);
    this.m_willSummonAMinion = true;
  }

  protected override void DoActionPostTasks()
  {
    if (!this.m_willSummonAMinion)
      return;
    this.SuppressDeathSoundsOnKilledTargets();
  }

  protected override bool HasPendingTasks() => this.m_waitForSpawnSpell;

  private void SuppressDeathSoundsOnKilledTargets()
  {
    List<Entity> targetEntities = new List<Entity>();
    foreach (GameObject visualTarget in this.GetVisualTargets())
    {
      if (!((Object) visualTarget == (Object) null))
      {
        Card component = visualTarget.GetComponent<Card>();
        targetEntities.Add(component.GetEntity());
      }
    }
    foreach (Entity sourceAmongstTarget in GameUtils.GetEntitiesKilledBySourceAmongstTargets(this.GetSourceCard().GetEntity().GetEntityId(), targetEntities))
      sourceAmongstTarget.GetCard().SuppressDeathSounds(true);
  }

  public void OnSpawnSpellEvent(string eventName, object eventData, object userData)
  {
    if (!(eventName == "ClosePortal"))
      return;
    this.m_waitForSpawnSpell = false;
    this.OnSpellFinished();
  }

  private Card GetSpawnedMinion()
  {
    for (int index = 0; index < this.m_taskList.GetTaskList().Count; ++index)
    {
      Network.PowerHistory power = this.m_taskList.GetTaskList()[index].GetPower();
      if (power.Type == Network.PowerType.FULL_ENTITY)
      {
        int id = (power as Network.HistFullEntity).Entity.ID;
        Entity entity = GameState.Get().GetEntity(id);
        if (entity.GetTag(GAME_TAG.ZONE) != 6 && entity != null)
        {
          Card card = entity.GetCard();
          if (!((Object) card == (Object) null))
            return card;
        }
      }
    }
    return (Card) null;
  }
}
