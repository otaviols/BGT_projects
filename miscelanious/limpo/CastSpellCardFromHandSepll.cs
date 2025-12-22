using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSpellCardFromHandSepll : Spell
{
  [SerializeField]
  private float m_BigCardDisplayTime = 1f;

  public override bool AddPowerTargets()
  {
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.TARGET && histMetaData.Info.Count != 0)
        {
          int id = histMetaData.Info[0];
          Entity entity = GameState.Get().GetEntity(id);
          if (entity != null && entity.GetZone() == TAG_ZONE.HAND)
          {
            this.AddTarget(entity.GetCard().gameObject);
            return true;
          }
        }
      }
    }
    return false;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.StartCoroutine(this.DoEffectWithTiming());
    base.OnAction(prevStateType);
  }

  private IEnumerator DoEffectWithTiming()
  {
    CastSpellCardFromHandSepll cardFromHandSepll = this;
    Card card = cardFromHandSepll.GetTargetCard();
    Player controller = card.GetController();
    card.SetDoNotSort(true);
    bool complete = false;
    if (controller.IsFriendlySide())
    {
      yield return (object) cardFromHandSepll.StartCoroutine(cardFromHandSepll.MoveCardToBigCardSpot());
      yield return (object) cardFromHandSepll.StartCoroutine(cardFromHandSepll.PlayPowerUpSpell());
      complete = true;
    }
    else
    {
      yield return (object) cardFromHandSepll.StartCoroutine(cardFromHandSepll.ShowBigCard());
      complete = true;
    }
    while (!complete)
      yield return (object) null;
    card.SetDoNotSort(false);
    cardFromHandSepll.OnSpellFinished();
    cardFromHandSepll.OnStateFinished();
  }

  private IEnumerator ShowBigCard()
  {
    CastSpellCardFromHandSepll cardFromHandSepll = this;
    Card targetCard = cardFromHandSepll.GetTargetCard();
    targetCard.HideCard();
    Entity entity = targetCard.GetEntity();
    cardFromHandSepll.UpdateTags(entity);
    HistoryManager.Get().CreatePlayedBigCard(entity, (HistoryManager.BigCardStartedCallback) (() => { }), (HistoryManager.BigCardFinishedCallback) (() => { }), true, false, (int) ((double) cardFromHandSepll.m_BigCardDisplayTime * 1000.0));
    yield return (object) new WaitForSeconds(cardFromHandSepll.m_BigCardDisplayTime);
  }

  private void UpdateTags(Entity entity)
  {
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.SHOW_ENTITY)
      {
        Network.Entity entity1 = (power as Network.HistShowEntity).Entity;
        if (entity1.ID == entity.GetEntityId())
        {
          entity.LoadCard(entity1.CardID);
          using (List<Network.Entity.Tag>.Enumerator enumerator = entity1.Tags.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Network.Entity.Tag current = enumerator.Current;
              entity.SetTag(current.Name, current.Value);
            }
            return;
          }
        }
      }
    }
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        if (histTagChange.Entity == entity.GetEntityId() && (histTagChange.Tag == 219 || histTagChange.Tag == 199 || histTagChange.Tag == 48))
          entity.SetTag(histTagChange.Tag, histTagChange.Value);
      }
    }
  }

  private IEnumerator MoveCardToBigCardSpot()
  {
    CastSpellCardFromHandSepll cardFromHandSepll = this;
    while (HistoryManager.Get().IsShowingBigCard())
      yield return (object) null;
    Card targetCard = cardFromHandSepll.GetTargetCard();
    string bigCardBoneName = HistoryManager.Get().GetBigCardBoneName();
    Transform bone = Board.Get().FindBone(bigCardBoneName);
    iTween.MoveTo(targetCard.gameObject, bone.position, cardFromHandSepll.m_BigCardDisplayTime);
    iTween.RotateTo(targetCard.gameObject, bone.rotation.eulerAngles, cardFromHandSepll.m_BigCardDisplayTime);
    iTween.ScaleTo(targetCard.gameObject, new Vector3(1f, 1f, 1f), cardFromHandSepll.m_BigCardDisplayTime);
    SoundManager.Get().LoadAndPlay((AssetReference) "play_card_from_hand_1.prefab:ac4be75e319a97947a68308a08e54e88");
    yield return (object) new WaitForSeconds(cardFromHandSepll.m_BigCardDisplayTime);
  }

  private IEnumerator PlayPowerUpSpell()
  {
    Spell powerUpSpell = this.GetTargetCard().GetActor().GetSpell(SpellType.POWER_UP);
    if (!((Object) powerUpSpell == (Object) null))
    {
      bool complete = false;
      powerUpSpell.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
      {
        if (prevStateType != SpellStateType.BIRTH)
          return;
        complete = true;
      }));
      powerUpSpell.ActivateState(SpellStateType.BIRTH);
      while (!complete)
        yield return (object) null;
      powerUpSpell.Deactivate();
    }
  }
}
