using System.Collections;
using UnityEngine;

public class JaraxxusMinionSpell : Spell
{
  public float m_MoveToLocationDelay;
  public float m_MoveToLocationDuration = 1.5f;
  public iTween.EaseType m_MoveToLocationEaseType = iTween.EaseType.linear;
  public float m_MoveToHeroSpotDelay = 3.5f;
  public float m_MoveToHeroSpotDuration = 0.3f;
  public iTween.EaseType m_MoveToHeroSpotEaseType = iTween.EaseType.linear;

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
        if (!entity.IsHero())
        {
          Debug.LogWarning((object) string.Format("{0}.AddPowerTargets() - WARNING HistFullEntity where entity id={1} is not a hero", (object) this, (object) id));
          return false;
        }
        this.AddTarget(entity.GetCard().gameObject);
        return true;
      }
    }
    return false;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.SetupHero());
  }

  private IEnumerator SetupHero()
  {
    JaraxxusMinionSpell jaraxxusMinionSpell = this;
    Card minionCard = jaraxxusMinionSpell.GetSourceCard();
    Card heroCard = jaraxxusMinionSpell.GetTargetCard();
    Entity heroEntity = heroCard.GetEntity();
    minionCard.SuppressDeathEffects(true);
    minionCard.GetActor().TurnOffCollider();
    jaraxxusMinionSpell.FindFullEntityTask().DoTask();
    while (heroEntity.IsLoadingAssets())
      yield return (object) null;
    heroCard.HideCard();
    heroCard.TransitionToZone(ZoneMgr.Get().FindZoneForEntity(heroEntity));
    while (heroCard.IsActorLoading())
      yield return (object) null;
    heroCard.GetActor().TurnOffCollider();
    jaraxxusMinionSpell.StartCoroutine(jaraxxusMinionSpell.PlaySummoningSpells(minionCard, heroCard));
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
    this.GetSourceCard().GetActor().TurnOnCollider();
    Card targetCard = this.GetTargetCard();
    targetCard.GetActor().TurnOnCollider();
    targetCard.ActivateStateSpells();
    targetCard.ShowCard();
    this.OnSpellFinished();
  }

  private IEnumerator PlaySummoningSpells(Card minionCard, Card heroCard)
  {
    heroCard.transform.position = minionCard.transform.position;
    minionCard.ActivateActorSpell(SpellType.SUMMON_JARAXXUS);
    heroCard.ActivateActorSpell(SpellType.SUMMON_JARAXXUS);
    yield return (object) new WaitForSeconds(this.m_MoveToLocationDelay);
    this.MoveToSpellLocation(minionCard, heroCard);
    yield return (object) new WaitForSeconds(this.m_MoveToHeroSpotDelay);
    this.MoveToHeroSpot(minionCard, heroCard);
  }

  private void MoveToSpellLocation(Card minionCard, Card heroCard)
  {
    Hashtable args1 = iTween.Hash((object) "position", (object) this.transform.position, (object) "time", (object) this.m_MoveToLocationDuration, (object) "easetype", (object) this.m_MoveToLocationEaseType);
    iTween.MoveTo(minionCard.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "position", (object) this.transform.position, (object) "time", (object) this.m_MoveToLocationDuration, (object) "easetype", (object) this.m_MoveToLocationEaseType);
    iTween.MoveTo(heroCard.gameObject, args2);
  }

  private void MoveToHeroSpot(Card minionCard, Card heroCard)
  {
    ZoneHero heroZone = heroCard.GetController().GetHeroZone();
    Hashtable args1 = iTween.Hash((object) "position", (object) heroZone.transform.position, (object) "time", (object) this.m_MoveToHeroSpotDuration, (object) "easetype", (object) this.m_MoveToHeroSpotEaseType);
    iTween.MoveTo(minionCard.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "position", (object) heroZone.transform.position, (object) "time", (object) this.m_MoveToHeroSpotDuration, (object) "easetype", (object) this.m_MoveToHeroSpotEaseType, (object) "oncomplete", (object) "OnMoveToHeroSpotComplete", (object) "oncompletetarget", (object) this.gameObject);
    iTween.MoveTo(heroCard.gameObject, args2);
  }

  private void OnMoveToHeroSpotComplete() => this.Finish();
}
