using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPowerSwapSpell : Spell
{
  public Spell m_swapFX;

  public override bool AddPowerTargets()
  {
    if (!this.CanAddPowerTargets())
      return false;
    int entityId1 = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetEntityId();
    int entityId2 = GameState.Get().GetOpposingSidePlayer().GetHeroPower().GetEntityId();
    int num1 = -1;
    int num2 = -1;
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      if (taskList[index].GetPower() is Network.HistTagChange power && power.Tag == 50)
      {
        if (power.Entity == entityId1)
          num1 = index;
        else if (power.Entity == entityId2)
          num2 = index;
      }
    }
    return num1 >= 0 && num2 >= 0;
  }

  protected override void OnAction(SpellStateType prevStateType) => this.StartCoroutine(this.DoActionWithTiming(prevStateType));

  private IEnumerator DoActionWithTiming(SpellStateType prevStateType)
  {
    HeroPowerSwapSpell c = this;
    Card yourHeroPowerCard = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard();
    Card theirHeroPowerCard = GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard();
    Animation yourAnim = yourHeroPowerCard.GetActor().GetComponent<Animation>();
    Animation theirAnim = theirHeroPowerCard.GetActor().GetComponent<Animation>();
    while (yourAnim.isPlaying || theirAnim.isPlaying)
      yield return (object) null;
    if ((Object) c.m_swapFX == (Object) null)
    {
      c.OnSpellFinished();
      c.OnStateFinished();
    }
    else
    {
      Spell spell = SpellManager.Get().GetSpell(c.m_swapFX);
      SpellUtils.SetCustomSpellParent(spell, (Component) c);
      spell.SetSource(yourHeroPowerCard.gameObject);
      spell.AddTarget(theirHeroPowerCard.gameObject);
      // ISSUE: reference to a compiler-generated method
      spell.AddFinishedCallback(new Spell.FinishedCallback(c.\u003CDoActionWithTiming\u003Eb__3_0));
      // ISSUE: reference to a compiler-generated method
      spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(c.\u003CDoActionWithTiming\u003Eb__3_1));
      spell.Activate();
    }
  }
}
