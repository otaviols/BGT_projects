using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardedCardReturnToHandSpell : Spell
{
  [SerializeField]
  private Spell m_TargetSpell;
  private Entity m_entityDiscarded;
  private List<Spell> m_activeTargetSpells = new List<Spell>();

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.m_entityDiscarded = this.m_taskList.GetSourceEntity(false);
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoActionWithTiming());
  }

  private IEnumerator DoActionWithTiming()
  {
    DiscardedCardReturnToHandSpell returnToHandSpell = this;
    returnToHandSpell.ProcessShowEntityForTargets();
    yield return (object) returnToHandSpell.StartCoroutine(returnToHandSpell.WaitAssetLoad());
    yield return (object) returnToHandSpell.StartCoroutine(returnToHandSpell.PlayTargetSpells());
  }

  private void ProcessShowEntityForTargets()
  {
    foreach (PowerTask task in this.GetPowerTaskList().GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.SHOW_ENTITY)
      {
        Network.Entity entity = (power as Network.HistShowEntity).Entity;
        Entity targetEntity = this.FindTargetEntity(entity.ID);
        if (targetEntity != null)
        {
          foreach (Network.Entity.Tag tag in entity.Tags)
            targetEntity.SetTag(tag.Name, tag.Value);
        }
      }
    }
  }

  private Entity FindTargetEntity(int entityID)
  {
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      if (!((Object) component == (Object) null))
      {
        Entity entity = component.GetEntity();
        if (entity != null && entity.GetEntityId() == entityID)
          return entity;
      }
    }
    return (Entity) null;
  }

  private IEnumerator WaitAssetLoad()
  {
    DiscardedCardReturnToHandSpell returnToHandSpell = this;
    foreach (GameObject target in returnToHandSpell.m_targets)
    {
      Card card = target.GetComponent<Card>();
      if (!((Object) card == (Object) null))
      {
        string cardId = returnToHandSpell.m_entityDiscarded.GetCardId();
        EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
        card.GetEntity().LoadCard(cardId);
        card.UpdateActor(true, ActorNames.GetHandActor(entityDef, returnToHandSpell.m_entityDiscarded.GetPremiumType()));
        while (card.IsActorLoading())
          yield return (object) null;
        TransformUtil.CopyWorld((Component) card, (Component) returnToHandSpell.m_entityDiscarded.GetCard().transform);
        card.HideCard();
        card = (Card) null;
      }
    }
  }

  private IEnumerator PlayTargetSpells()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DiscardedCardReturnToHandSpell returnToHandSpell = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if ((Object) returnToHandSpell.m_TargetSpell == (Object) null)
      return false;
    foreach (GameObject target in returnToHandSpell.m_targets)
    {
      Spell spell = SpellManager.Get().GetSpell(returnToHandSpell.m_TargetSpell);
      if (!((Object) spell == (Object) null))
      {
        returnToHandSpell.m_activeTargetSpells.Add(spell);
        TransformUtil.AttachAndPreserveLocalTransform(spell.transform, target.transform);
        spell.SetSource(target);
        spell.AddFinishedCallback(new Spell.FinishedCallback(returnToHandSpell.OnSelectedSpellFinished));
        spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(returnToHandSpell.OnSelectedSpellStateFinished));
        spell.Activate();
      }
    }
    return false;
  }

  private void OnSelectedSpellFinished(Spell spell, object userData)
  {
    if (this.m_activeTargetSpells.Count == 0)
      return;
    foreach (Spell activeTargetSpell in this.m_activeTargetSpells)
    {
      if (!activeTargetSpell.IsFinished())
        return;
    }
    this.OnSpellFinished();
  }

  private void OnSelectedSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (this.m_activeTargetSpells.Count == 0)
      return;
    foreach (Spell activeTargetSpell in this.m_activeTargetSpells)
    {
      if (spell.GetActiveState() != SpellStateType.NONE)
        return;
    }
    foreach (Object activeTargetSpell in this.m_activeTargetSpells)
      Object.Destroy(activeTargetSpell);
    this.m_activeTargetSpells.Clear();
    this.OnStateFinished();
  }
}
