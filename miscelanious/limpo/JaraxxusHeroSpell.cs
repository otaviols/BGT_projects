using System.Collections;
using UnityEngine;

public class JaraxxusHeroSpell : Spell
{
  private PowerTask m_heroPowerTask;
  private PowerTask m_weaponTask;

  public override bool AddPowerTargets()
  {
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.FULL_ENTITY)
      {
        int id = (power as Network.HistFullEntity).Entity.ID;
        Entity entity = GameState.Get().GetEntity(id);
        if (entity == null)
        {
          Debug.LogWarning((object) string.Format("{0}.AddPowerTargets() - WARNING encountered HistFullEntity where entity id={1} but there is no entity with that id", (object) this, (object) id));
          return false;
        }
        if (entity.IsHeroPower())
        {
          this.m_heroPowerTask = task;
          this.AddTarget(entity.GetCard().gameObject);
          if (this.m_weaponTask != null)
            return true;
        }
        else if (entity.IsWeapon())
        {
          this.m_weaponTask = task;
          this.AddTarget(entity.GetCard().gameObject);
          if (this.m_heroPowerTask != null)
            return true;
        }
      }
    }
    this.Reset();
    return false;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.SetupCards());
  }

  private IEnumerator SetupCards()
  {
    Entity heroPower = this.LoadCardFromTask(this.m_heroPowerTask);
    Entity weapon = this.LoadCardFromTask(this.m_weaponTask);
    while (heroPower.IsLoadingAssets() || weapon.IsLoadingAssets())
      yield return (object) null;
    Card heroPowerCard = heroPower.GetCard();
    heroPowerCard.HideCard();
    heroPowerCard.TransitionToZone(ZoneMgr.Get().FindZoneForEntity(heroPower));
    Card weaponCard = weapon.GetCard();
    weaponCard.HideCard();
    weaponCard.TransitionToZone(ZoneMgr.Get().FindZoneForEntity(weapon));
    while (heroPowerCard.IsActorLoading() || weaponCard.IsActorLoading())
      yield return (object) null;
    this.PlayCardSpells(heroPowerCard, weaponCard);
  }

  private Entity LoadCardFromTask(PowerTask task)
  {
    Network.Entity entity1 = (task.GetPower() as Network.HistFullEntity).Entity;
    int id = entity1.ID;
    Entity entity2 = GameState.Get().GetEntity(id);
    entity2.LoadCard(entity1.CardID);
    return entity2;
  }

  private Card GetCardFromTask(PowerTask task)
  {
    int id = (task.GetPower() as Network.HistFullEntity).Entity.ID;
    return GameState.Get().GetEntity(id).GetCard();
  }

  private void Reset()
  {
    this.m_heroPowerTask = (PowerTask) null;
    this.m_weaponTask = (PowerTask) null;
  }

  private void Finish()
  {
    this.Reset();
    this.OnSpellFinished();
  }

  private void PlayCardSpells(Card heroPowerCard, Card weaponCard)
  {
    heroPowerCard.ShowCard();
    heroPowerCard.ActivateStateSpells();
    heroPowerCard.ActivateActorSpell(SpellType.SUMMON_JARAXXUS, new Spell.FinishedCallback(this.OnSpellFinished_HeroPower));
    weaponCard.ActivateActorSpell(SpellType.SUMMON_JARAXXUS, new Spell.FinishedCallback(this.OnSpellFinished_Weapon));
  }

  private void OnSpellFinished_HeroPower(Spell spell, object userData)
  {
    this.m_heroPowerTask.SetCompleted(true);
    if (!this.m_weaponTask.IsCompleted())
      return;
    this.Finish();
  }

  private void OnSpellFinished_Weapon(Spell spell, object userData)
  {
    Card cardFromTask = this.GetCardFromTask(this.m_weaponTask);
    cardFromTask.ShowCard();
    cardFromTask.ActivateStateSpells();
    this.m_weaponTask.SetCompleted(true);
    if (!this.m_heroPowerTask.IsCompleted())
      return;
    this.Finish();
  }
}
