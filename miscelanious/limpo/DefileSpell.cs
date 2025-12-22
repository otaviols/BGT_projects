using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefileSpell : SuperSpell
{
  public Spell m_SpellPrefab;
  public float m_TimeBetweenCasts = 1f;
  private List<GameObject> m_singleCastTargets;

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DefileEffect());
  }

  private void FindTargetsForSingleCast(int index)
  {
    this.m_singleCastTargets.Clear();
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index1 = index + 1; index1 < taskList.Count; ++index1)
    {
      Network.PowerHistory power = taskList[index1].GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.EFFECT_TIMING)
          break;
        if (histMetaData.MetaType == HistoryMeta.Type.TARGET && histMetaData.Info != null && histMetaData.Info.Count != 0)
        {
          for (int index2 = 0; index2 < histMetaData.Info.Count; ++index2)
          {
            Entity entity = GameState.Get().GetEntity(histMetaData.Info[index2]);
            if (entity != null)
              this.m_singleCastTargets.Add(entity.GetCard().gameObject);
          }
        }
      }
    }
  }

  private IEnumerator DefileEffect()
  {
    DefileSpell defileSpell = this;
    defileSpell.m_singleCastTargets = new List<GameObject>();
    Card sourceCard = defileSpell.m_taskList.GetSourceEntity().GetCard();
    List<PowerTask> tasks = defileSpell.m_taskList.GetTaskList();
    for (int i = 0; i < tasks.Count; ++i)
    {
      Network.PowerHistory power = tasks[i].GetPower();
      if (power.Type == Network.PowerType.META_DATA && ((Network.HistMetaData) power).MetaType == HistoryMeta.Type.EFFECT_TIMING)
      {
        bool complete = false;
        PowerTaskList.CompleteCallback callback = (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true);
        defileSpell.FindTargetsForSingleCast(i);
        defileSpell.m_taskList.DoTasks(0, i, callback);
        while (!complete)
          yield return (object) null;
        if ((Object) defileSpell.m_SpellPrefab != (Object) null)
        {
          ++defileSpell.m_effectsPendingFinish;
          Spell spell = defileSpell.CloneSpell(defileSpell.m_SpellPrefab);
          spell.SetSource(sourceCard.gameObject);
          spell.AddTargets(defileSpell.m_singleCastTargets);
          spell.ActivateState(SpellStateType.ACTION);
          while (!spell.IsFinished())
            yield return (object) null;
          spell = (Spell) null;
        }
        yield return (object) new WaitForSeconds(defileSpell.m_TimeBetweenCasts);
      }
    }
    --defileSpell.m_effectsPendingFinish;
    defileSpell.FinishIfPossible();
  }
}
