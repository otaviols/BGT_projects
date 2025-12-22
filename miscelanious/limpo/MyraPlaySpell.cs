using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyraPlaySpell : Spell
{
  [SerializeField]
  private Spell m_Spell;
  [SerializeField]
  private float m_DrawSpeedScale = 1f;
  private Spell m_spell;
  private List<Entity> m_entitiesToDrawBeforeFX = new List<Entity>();

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.SetSuppressDeckEmotes(true);
    this.StartCoroutine(this.DoEffectWithTiming());
    base.OnAction(prevStateType);
  }

  private void SetSuppressDeckEmotes(bool suppress)
  {
    ZoneDeck deckZone = this.GetSourceCard().GetController().GetDeckZone();
    deckZone.SetSuppressEmotes(suppress);
    deckZone.UpdateLayout();
  }

  private IEnumerator DoEffectWithTiming()
  {
    MyraPlaySpell myraPlaySpell = this;
    myraPlaySpell.ActivateBirth();
    myraPlaySpell.FindEntitiesToDrawBeforeFX();
    yield return (object) myraPlaySpell.StartCoroutine(myraPlaySpell.CompleteTasks());
    yield return (object) myraPlaySpell.StartCoroutine(myraPlaySpell.WaitForDrawing());
    yield return (object) myraPlaySpell.StartCoroutine(myraPlaySpell.ActivateAction());
  }

  private void ActivateBirth()
  {
    if ((UnityEngine.Object) this.m_spell == (UnityEngine.Object) null)
    {
      this.m_spell = SpellManager.Get().GetSpell(this.m_Spell);
      this.m_spell.SetSource(this.GetSource());
    }
    SpellUtils.ActivateBirthIfNecessary(this.m_spell);
  }

  private void FindEntitiesToDrawBeforeFX()
  {
    Card sourceCard = this.GetSourceCard();
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      if (sourceCard.GetControllerSide() == Player.Side.FRIENDLY)
      {
        this.FindRevealedEntitiesToDrawBeforeFX(task.GetPower());
      }
      else
      {
        this.FindRevealedEntitiesToDrawBeforeFX(task.GetPower());
        this.FindHiddenEntitiesToDrawBeforeFX(task.GetPower());
      }
    }
  }

  private void FindRevealedEntitiesToDrawBeforeFX(Network.PowerHistory power)
  {
    if (power.Type != Network.PowerType.SHOW_ENTITY)
      return;
    Network.HistShowEntity histShowEntity = (Network.HistShowEntity) power;
    Entity entity = GameState.Get().GetEntity(histShowEntity.Entity.ID);
    if (entity == null || entity.GetZone() != TAG_ZONE.DECK)
      return;
    if (histShowEntity.Entity.Tags.Exists((Predicate<Network.Entity.Tag>) (tag => tag.Name == 49 && tag.Value == 3)))
    {
      this.m_entitiesToDrawBeforeFX.Add(entity);
    }
    else
    {
      if (!histShowEntity.Entity.Tags.Exists((Predicate<Network.Entity.Tag>) (tag => tag.Name == 49 && tag.Value == 4)))
        return;
      entity.GetCard().SetSkipMilling(true);
      this.m_entitiesToDrawBeforeFX.Add(entity);
    }
  }

  private void FindHiddenEntitiesToDrawBeforeFX(Network.PowerHistory power)
  {
    if (power.Type != Network.PowerType.TAG_CHANGE)
      return;
    Network.HistTagChange histTagChange = (Network.HistTagChange) power;
    if (histTagChange.Tag != 49 || histTagChange.Value != 3 && histTagChange.Value != 4)
      return;
    Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
    if (entity == null)
    {
      Debug.LogWarningFormat("{0}.FindOpponentEntitiesToDrawBeforeFX() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity);
    }
    else
    {
      if (entity.GetZone() != TAG_ZONE.DECK)
        return;
      if (histTagChange.Value == 4)
        entity.GetCard().SetSkipMilling(true);
      this.m_entitiesToDrawBeforeFX.Add(entity);
    }
  }

  private void SetDrawTimeScale(float scale)
  {
    foreach (Entity entity in this.m_entitiesToDrawBeforeFX)
      entity.GetCard().SetDrawTimeScale(scale);
  }

  private IEnumerator CompleteTasks()
  {
    MyraPlaySpell myraPlaySpell = this;
    myraPlaySpell.SetDrawTimeScale(1f / myraPlaySpell.m_DrawSpeedScale);
    bool complete = false;
    myraPlaySpell.m_taskList.DoAllTasks((PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true));
    while (!complete)
      yield return (object) null;
  }

  private IEnumerator WaitForDrawing()
  {
    MyraPlaySpell myraPlaySpell = this;
    if (myraPlaySpell.m_taskList.GetBlockEnd() != null)
    {
      while (myraPlaySpell.IsDrawing())
        yield return (object) null;
      myraPlaySpell.SetDrawTimeScale(1f);
    }
  }

  private bool IsDrawing()
  {
    foreach (Entity entity in this.m_entitiesToDrawBeforeFX)
    {
      Card card = entity.GetCard();
      if (entity.GetZone() == TAG_ZONE.HAND && !(card.GetZone() is ZonePlay) && !((UnityEngine.Object) card.GetZone() == (UnityEngine.Object) null))
      {
        if (!(card.GetZone() is ZoneHand) || card.IsDoNotSort())
          return true;
        if (entity.IsControlledByFriendlySidePlayer())
        {
          if (!card.CardStandInIsInteractive())
            return true;
        }
        else if (card.IsBeingDrawnByOpponent())
          return true;
      }
    }
    return false;
  }

  private IEnumerator ActivateAction()
  {
    MyraPlaySpell myraPlaySpell = this;
    if (myraPlaySpell.m_taskList.GetBlockEnd() == null)
    {
      myraPlaySpell.OnSpellFinished();
    }
    else
    {
      myraPlaySpell.m_spell.ActivateState(SpellStateType.ACTION);
      while (!myraPlaySpell.m_spell.IsFinished())
        yield return (object) null;
      myraPlaySpell.OnSpellFinished();
      myraPlaySpell.SetSuppressDeckEmotes(false);
      while (myraPlaySpell.m_spell.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
      SpellManager.Get().ReleaseSpell(myraPlaySpell.m_spell);
      myraPlaySpell.m_spell = (Spell) null;
      myraPlaySpell.Deactivate();
    }
  }
}
