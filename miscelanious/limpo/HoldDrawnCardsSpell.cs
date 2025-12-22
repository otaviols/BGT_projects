using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldDrawnCardsSpell : SuperSpell
{
  public float m_PreEffectHoldTime;
  public float m_PostEffectHoldTime;
  public Spell m_DrawnCardSpell;
  private SortedList<int, Card> m_drawCardData = new SortedList<int, Card>();
  private List<Spell> m_drawnCardSpellInstances = new List<Spell>();

  public override bool AttachPowerTaskList(PowerTaskList taskList)
  {
    if (!base.AttachPowerTaskList(taskList))
      return false;
    this.FindHoldDrawnCardMetaDataTasks();
    return true;
  }

  private void FindHoldDrawnCardMetaDataTasks()
  {
    this.m_drawCardData.Clear();
    if (this.m_taskList == null)
      return;
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      if (taskList[index].GetPower() is Network.HistMetaData power && power.MetaType == HistoryMeta.Type.HOLD_DRAWN_CARD && power.Info.Count == 1)
      {
        Entity entity = GameState.Get().GetEntity(power.Info[0]);
        if (entity != null)
        {
          Card card = entity.GetCard();
          if (!((Object) card == (Object) null))
            this.m_drawCardData.Add(index, card);
        }
      }
    }
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DrawCardsWithEffects());
  }

  private IEnumerator DrawCardsWithEffects()
  {
    HoldDrawnCardsSpell c = this;
    for (int drawnCardIndex = 0; drawnCardIndex < c.m_drawCardData.Count; ++drawnCardIndex)
    {
      int holdDrawMetaDataTaskIndex = c.m_drawCardData.Keys[drawnCardIndex];
      Card drawnCard = c.m_drawCardData.Values[drawnCardIndex];
      if (TurnStartManager.Get().IsCardDrawHandled(drawnCard))
        TurnStartManager.Get().DrawCardImmediately(drawnCard);
      bool complete = false;
      c.m_taskList.DoTasks(0, holdDrawMetaDataTaskIndex + 1, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true));
      while (!complete)
        yield return (object) null;
      c.m_taskList.GetTaskList()[holdDrawMetaDataTaskIndex].SetCompleted(false);
      while (!drawnCard.IsActorReady())
        yield return (object) null;
      yield return (object) new WaitForSeconds(c.m_PreEffectHoldTime);
      if ((Object) c.m_DrawnCardSpell != (Object) null)
      {
        Spell spell = SpellManager.Get().GetSpell(c.m_DrawnCardSpell);
        SpellUtils.SetCustomSpellParent(spell, (Component) c);
        spell.SetSource(c.GetSource());
        spell.AddTarget(drawnCard.gameObject);
        spell.Activate();
        c.m_drawnCardSpellInstances.Add(spell);
      }
      int count1 = c.m_taskList.GetTaskList().Count;
      if (drawnCardIndex + 1 < c.m_drawCardData.Count)
        count1 = c.m_drawCardData.Keys[drawnCardIndex + 1] - holdDrawMetaDataTaskIndex - 1;
      c.m_taskList.DoTasks(holdDrawMetaDataTaskIndex + 1, count1);
      yield return (object) new WaitForSeconds(c.m_PostEffectHoldTime);
      c.m_taskList.GetTaskList()[holdDrawMetaDataTaskIndex].SetCompleted(true);
      drawnCard = (Card) null;
    }
    foreach (Spell cardSpellInstance in c.m_drawnCardSpellInstances)
    {
      Spell spell = cardSpellInstance;
      if (!((Object) spell == (Object) null))
      {
        while (!spell.CanPurge())
          yield return (object) null;
        SpellUtils.PurgeSpell(spell);
        spell = (Spell) null;
      }
    }
    c.m_drawnCardSpellInstances.Clear();
    --c.m_effectsPendingFinish;
    c.FinishIfPossible();
  }
}
