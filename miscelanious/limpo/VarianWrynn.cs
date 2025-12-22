using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarianWrynn : SuperSpell
{
  public string m_perMinionSound;
  public Spell m_varianSpellPrefab;
  public Spell m_deckSpellPrefab;
  public float m_spellLeadTime = 1f;

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoVariansCoolThing());
  }

  private IEnumerator DoVariansCoolThing()
  {
    VarianWrynn varianWrynn = this;
    Card card1 = varianWrynn.m_taskList.GetSourceEntity().GetCard();
    List<Spell> fxObjects = new List<Spell>();
    if ((Object) varianWrynn.m_varianSpellPrefab != (Object) null && varianWrynn.m_taskList.IsOrigin())
    {
      Spell spell = SpellManager.Get().GetSpell(varianWrynn.m_varianSpellPrefab);
      fxObjects.Add(spell);
      spell.SetSource(card1.gameObject);
      spell.Activate();
    }
    List<PowerTask> tasks = varianWrynn.m_taskList.GetTaskList();
    bool foundTarget = false;
    bool lastWasMinion = false;
    for (int i = 0; i < tasks.Count; ++i)
    {
      Network.PowerHistory power = tasks[i].GetPower();
      if (power.Type == Network.PowerType.SHOW_ENTITY)
      {
        Network.HistShowEntity showEntity = (Network.HistShowEntity) power;
        if (!foundTarget)
        {
          Card card2 = GameState.Get().GetEntity(showEntity.Entity.ID).GetCard();
          foundTarget = true;
          if ((Object) varianWrynn.m_deckSpellPrefab != (Object) null && varianWrynn.m_taskList.IsOrigin())
          {
            Spell spell = SpellManager.Get().GetSpell(varianWrynn.m_deckSpellPrefab);
            fxObjects.Add(spell);
            spell.SetSource(card2.gameObject);
            spell.Activate();
            while (!spell.IsFinished())
              yield return (object) null;
            spell = (Spell) null;
          }
        }
        bool complete = false;
        PowerTaskList.CompleteCallback callback = (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true);
        varianWrynn.m_taskList.DoTasks(0, i, callback);
        if (lastWasMinion)
          yield return (object) new WaitForSeconds(varianWrynn.m_spellLeadTime);
        lastWasMinion = varianWrynn.IsMinion(showEntity);
        while (!complete)
          yield return (object) null;
        showEntity = (Network.HistShowEntity) null;
      }
    }
    foreach (Spell spell in fxObjects)
      SpellManager.Get().ReleaseSpell(spell);
    --varianWrynn.m_effectsPendingFinish;
    varianWrynn.FinishIfPossible();
  }

  private bool IsMinion(Network.HistShowEntity showEntity)
  {
    for (int index = 0; index < showEntity.Entity.Tags.Count; ++index)
    {
      Network.Entity.Tag tag = showEntity.Entity.Tags[index];
      if (tag.Name == 202)
        return tag.Value == 4;
    }
    return false;
  }
}
