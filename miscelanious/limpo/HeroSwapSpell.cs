using System.Collections;
using UnityEngine;

public class HeroSwapSpell : Spell
{
  public Spell m_OldHeroFX;
  public Spell m_NewHeroFX;
  public Spell m_OldHeroFX_long;
  public Spell m_NewHeroFX_long;
  public float m_FinishDelay;
  public bool removeOldStats;
  protected Card m_oldHeroCard;
  protected Card m_newHeroCard;
  protected Spell m_OldHeroFXToUse;
  protected Spell m_NewHeroFXToUse;

  public override bool AddPowerTargets()
  {
    if (GameState.Get().GetGameEntity() is KAR12_Portals gameEntity && gameEntity.ShouldPlayLongMidmissionCutscene())
    {
      this.m_OldHeroFXToUse = this.m_OldHeroFX_long;
      this.m_NewHeroFXToUse = this.m_NewHeroFX_long;
    }
    else
    {
      this.m_OldHeroFXToUse = this.m_OldHeroFX;
      this.m_NewHeroFXToUse = this.m_NewHeroFX;
    }
    this.m_oldHeroCard = (Card) null;
    this.m_newHeroCard = (Card) null;
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.FULL_ENTITY)
      {
        int id = ((Network.HistFullEntity) power).Entity.ID;
        Entity entity = GameState.Get().GetEntity(id);
        if (entity == null)
        {
          Debug.LogWarning((object) string.Format("{0}.AddPowerTargets() - WARNING encountered HistFullEntity where entity id={1} but there is no entity with that id", (object) this, (object) id));
          return false;
        }
        if (entity.IsHero())
          this.m_newHeroCard = entity.GetCard();
      }
      else if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        if (histTagChange.Tag == 49 && histTagChange.Value == 6)
        {
          int entity1 = histTagChange.Entity;
          Entity entity2 = GameState.Get().GetEntity(entity1);
          if (entity2 == null)
          {
            Debug.LogWarning((object) string.Format("{0}.AddPowerTargets() - WARNING encountered HistTagChange where entity id={1} but there is no entity with that id", (object) this, (object) entity1));
            return false;
          }
          if (entity2.IsHero())
            this.m_oldHeroCard = entity2.GetCard();
        }
      }
    }
    if ((Object) this.m_newHeroCard != (Object) null && (Object) this.m_oldHeroCard == (Object) null)
    {
      Player controller = this.m_newHeroCard.GetController();
      if (controller != null)
        this.m_oldHeroCard = controller.GetHeroCard();
    }
    if (!(bool) (Object) this.m_oldHeroCard)
    {
      this.m_newHeroCard = (Card) null;
      return false;
    }
    if ((bool) (Object) this.m_newHeroCard)
      return true;
    this.m_oldHeroCard = (Card) null;
    return false;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.SetupHero());
  }

  private IEnumerator SetupHero()
  {
    HeroSwapSpell heroSwapSpell = this;
    Entity newHeroEntity = heroSwapSpell.m_newHeroCard.GetEntity();
    heroSwapSpell.FindFullEntityTask().DoTask();
    while (newHeroEntity.IsLoadingAssets())
      yield return (object) null;
    heroSwapSpell.m_newHeroCard.HideCard();
    Zone zoneForEntity = ZoneMgr.Get().FindZoneForEntity(newHeroEntity);
    heroSwapSpell.m_newHeroCard.TransitionToZone(zoneForEntity);
    while (heroSwapSpell.m_newHeroCard.IsActorLoading())
      yield return (object) null;
    heroSwapSpell.m_newHeroCard.GetActor().TurnOffCollider();
    heroSwapSpell.m_newHeroCard.transform.position = heroSwapSpell.m_newHeroCard.GetZone().transform.position;
    if ((Object) heroSwapSpell.m_OldHeroFXToUse != (Object) null)
    {
      if (heroSwapSpell.removeOldStats)
      {
        Actor actor = heroSwapSpell.m_oldHeroCard.GetActor();
        Object.Destroy((Object) actor.m_healthObject);
        Object.Destroy((Object) actor.m_attackObject);
      }
      heroSwapSpell.StartCoroutine(heroSwapSpell.PlaySwapFx(heroSwapSpell.m_OldHeroFXToUse, heroSwapSpell.m_oldHeroCard));
    }
    if ((Object) heroSwapSpell.m_NewHeroFXToUse != (Object) null)
      heroSwapSpell.StartCoroutine(heroSwapSpell.PlaySwapFx(heroSwapSpell.m_NewHeroFXToUse, heroSwapSpell.m_newHeroCard));
    yield return (object) new WaitForSeconds(heroSwapSpell.m_FinishDelay);
    heroSwapSpell.Finish();
  }

  public virtual void CustomizeFXProcess(Actor heroActor)
  {
  }

  private IEnumerator PlaySwapFx(Spell heroFX, Card heroCard)
  {
    Actor actor = heroCard.GetActor();
    this.CustomizeFXProcess(actor);
    Spell swapSpell = SpellManager.Get().GetSpell(heroFX);
    SpellUtils.SetCustomSpellParent(swapSpell, (Component) actor);
    swapSpell.SetSource(heroCard.gameObject);
    swapSpell.Activate();
    while (!swapSpell.IsFinished())
      yield return (object) null;
    while (swapSpell.GetActiveState() != SpellStateType.NONE)
      yield return (object) null;
    SpellManager.Get().ReleaseSpell(swapSpell);
  }

  private PowerTask FindFullEntityTask()
  {
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      if (task.GetPower().Type == Network.PowerType.FULL_ENTITY)
        return task;
    }
    return (PowerTask) null;
  }

  private void Finish()
  {
    this.m_newHeroCard.GetActor().TurnOnCollider();
    this.m_newHeroCard.ActivateStateSpells();
    this.m_newHeroCard.ShowCard();
    this.OnSpellFinished();
  }
}
