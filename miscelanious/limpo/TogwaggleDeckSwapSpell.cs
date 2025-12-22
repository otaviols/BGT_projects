using System;
using System.Collections;
using System.Collections.Generic;

public class TogwaggleDeckSwapSpell : SpawnToHandSpell
{
  protected override void OnAction(SpellStateType prevStateType) => this.StartCoroutine(this.DoActionWithTiming(prevStateType));

  private IEnumerator DoActionWithTiming(SpellStateType prevStateType)
  {
    TogwaggleDeckSwapSpell togwaggleDeckSwapSpell = this;
    int friendlyDeckSize = 0;
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    if (friendlySidePlayer != null)
    {
      ZoneDeck deckZone = friendlySidePlayer.GetDeckZone();
      if ((UnityEngine.Object) deckZone != (UnityEngine.Object) null)
        friendlyDeckSize = deckZone.GetCardCount();
    }
    int opponentDeckSize = 0;
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    if (opposingSidePlayer != null)
    {
      ZoneDeck deckZone = opposingSidePlayer.GetDeckZone();
      if ((UnityEngine.Object) deckZone != (UnityEngine.Object) null)
        opponentDeckSize = deckZone.GetCardCount();
    }
    foreach (Zone zone in SpellUtils.FindZonesFromTag(SpellZoneTag.DECK))
      zone.AddLayoutBlocker();
    int num = -1;
    List<PowerTask> taskList1 = togwaggleDeckSwapSpell.m_taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      if (taskList1[index].GetPower() is Network.HistTagChange power)
      {
        bool flag = false;
        if (power.Tag == 49 && power.Value == 2)
          flag = true;
        if (power.Tag == 50)
          flag = true;
        if (flag)
          num = index;
      }
    }
    if (num >= 0)
    {
      bool complete = false;
      togwaggleDeckSwapSpell.m_taskList.DoTasks(0, num + 1, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true));
      while (!complete)
        yield return (object) null;
    }
    togwaggleDeckSwapSpell.OnBeforeActivateAreaEffectSpell = (Action<Spell>) (spell =>
    {
      spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnAEFinished));
      PlayMakerFSM component = spell.GetComponent<PlayMakerFSM>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.FsmVariables.GetFsmInt("FriendlyDeckSize").Value = friendlyDeckSize;
      component.FsmVariables.GetFsmInt("OpponentDeckSize").Value = opponentDeckSize;
    });
    // ISSUE: reference to a compiler-generated method
    togwaggleDeckSwapSpell.\u003C\u003En__0(prevStateType);
  }

  private void OnAEFinished(Spell spell, object userData)
  {
    if ((UnityEngine.Object) spell != (UnityEngine.Object) this.m_activeAreaEffectSpell)
      return;
    foreach (Zone zone in SpellUtils.FindZonesFromTag(SpellZoneTag.DECK))
    {
      ZoneDeck zoneDeck = zone as ZoneDeck;
      if ((UnityEngine.Object) zoneDeck != (UnityEngine.Object) null)
      {
        zoneDeck.RemoveLayoutBlocker();
        zoneDeck.SetSuppressEmotes(true);
        zoneDeck.SetVisibility(true);
        zoneDeck.UpdateLayout();
        zoneDeck.SetSuppressEmotes(false);
      }
    }
  }
}
