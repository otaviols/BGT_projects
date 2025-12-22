using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTurnSpell : Spell
{
  public float m_WaitAnim = 4f;
  public string m_TurnText = "GAMEPLAY_NEXT_TURN";
  public string m_AnimName = "ENDTURN_NEXT_TURN";
  public bool m_DoTimeScale = true;

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.StartCoroutine(this.SpellEffect(prevStateType));
    base.OnAction(prevStateType);
  }

  private IEnumerator SpellEffect(SpellStateType prevStateType)
  {
    ExtraTurnSpell extraTurnSpell = this;
    Entity sourceEntity = extraTurnSpell.m_taskList.GetSourceEntity();
    if (sourceEntity != null)
    {
      Player controller = sourceEntity.GetController();
      if (controller != null && controller.GetSide() == Player.Side.FRIENDLY)
      {
        EndTurnButton endButton = EndTurnButton.Get();
        if (!((Object) endButton == (Object) null))
        {
          endButton.AddInputBlocker();
          yield return (object) new WaitForSeconds(extraTurnSpell.m_WaitAnim);
          Animation anim = endButton.m_EndTurnButtonMesh.gameObject.GetComponent<Animation>();
          float length1 = anim.GetClip(extraTurnSpell.m_AnimName).length;
          anim.Play(extraTurnSpell.m_AnimName);
          List<PowerTask> taskList = extraTurnSpell.m_taskList.GetTaskList();
          for (int index = 0; index < taskList.Count; ++index)
          {
            if (taskList[index].GetPower() is Network.HistTagChange power && power.Tag == 272)
            {
              extraTurnSpell.m_taskList.DoTasks(0, index + 1, (PowerTaskList.CompleteCallback) null);
              break;
            }
          }
          endButton.DisplayExtraTurnState();
          yield return (object) new WaitForSeconds(length1);
          if (endButton.IsInWaitingState())
          {
            float length2 = anim.GetClip("ENDTURN_WAITING").length;
            anim.Play("ENDTURN_WAITING");
            yield return (object) new WaitForSeconds(length2);
          }
          endButton.RemoveInputBlocker();
          endButton.DisplayExtraTurnState();
          endButton = (EndTurnButton) null;
          anim = (Animation) null;
        }
      }
    }
  }
}
