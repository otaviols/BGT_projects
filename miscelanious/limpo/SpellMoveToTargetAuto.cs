using System.Collections;
using UnityEngine;

public class SpellMoveToTargetAuto : Spell
{
  public float m_MovementDurationSec = 0.5f;
  public iTween.EaseType m_EaseType = iTween.EaseType.linear;
  public bool m_DisableContainerAfterAction;
  public bool m_OnlyMoveContainer;
  public bool m_OrientToPath;
  public float CenterOffsetPercent = 50f;
  public float CenterPointHeightMin;
  public float CenterPointHeightMax;
  public float RightMin;
  public float RightMax;
  public float LeftMin;
  public float LeftMax;
  public bool DebugForceMax;
  public float DistanceScaleFactor = 8f;
  private bool m_waitingToAct = true;
  private Vector3[] m_pathNodes;
  private bool m_sourceComputed;
  private bool m_targetComputed;

  public override void SetSource(GameObject go)
  {
    if ((Object) this.GetSource() != (Object) go)
      this.m_sourceComputed = false;
    base.SetSource(go);
  }

  public override void RemoveSource()
  {
    base.RemoveSource();
    this.m_sourceComputed = false;
  }

  public override void AddTarget(GameObject go)
  {
    if ((Object) this.GetTarget() != (Object) go)
      this.m_targetComputed = false;
    base.AddTarget(go);
  }

  public override bool RemoveTarget(GameObject go)
  {
    GameObject target = this.GetTarget();
    if (!base.RemoveTarget(go))
      return false;
    if ((Object) target == (Object) go)
      this.m_targetComputed = false;
    return true;
  }

  public override void RemoveAllTargets()
  {
    int num = this.m_targets.Count > 0 ? 1 : 0;
    base.RemoveAllTargets();
    if (num == 0)
      return;
    this.m_targetComputed = false;
  }

  public override bool AddPowerTargets() => this.CanAddPowerTargets() && this.AddSinglePowerTarget();

  protected override void OnBirth(SpellStateType prevStateType)
  {
    base.OnBirth(prevStateType);
    this.ResetPath();
    this.m_waitingToAct = true;
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard == (Object) null)
    {
      Debug.LogError((object) string.Format("{0}.OnBirth() - sourceCard is null", (object) this));
      base.OnBirth(prevStateType);
    }
    else
    {
      if (this.DeterminePath(sourceCard.GetEntity().GetController(), sourceCard, (Card) null))
        return;
      Debug.LogError((object) string.Format("{0}.OnBirth() - no paths available", (object) this));
      base.OnBirth(prevStateType);
    }
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    if (this.m_pathNodes == null)
      this.ResetPath();
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard == (Object) null)
    {
      Debug.LogError((object) string.Format("SpellMoveToTarget.OnAction() - no source card"));
      this.DoActionFallback(prevStateType);
    }
    else
    {
      Card targetCard = this.GetTargetCard();
      if ((Object) targetCard == (Object) null)
      {
        Debug.LogError((object) string.Format("SpellMoveToTarget.OnAction() - no target card"));
        this.DoActionFallback(prevStateType);
      }
      else if (!this.DeterminePath(sourceCard.GetEntity().GetController(), sourceCard, targetCard))
      {
        Debug.LogError((object) string.Format("SpellMoveToTarget.DoAction() - no paths available, going to DEATH state"));
        this.DoActionFallback(prevStateType);
      }
      else
        this.StartCoroutine(this.WaitThenDoAction(prevStateType));
    }
  }

  protected IEnumerator WaitThenDoAction(SpellStateType prevStateType)
  {
    SpellMoveToTargetAuto moveToTargetAuto = this;
    while (moveToTargetAuto.m_waitingToAct)
      yield return (object) null;
    Hashtable args = iTween.Hash((object) "path", (object) moveToTargetAuto.m_pathNodes, (object) "time", (object) moveToTargetAuto.m_MovementDurationSec, (object) "easetype", (object) moveToTargetAuto.m_EaseType, (object) "oncomplete", (object) "OnMoveToTargetComplete", (object) "oncompletetarget", (object) moveToTargetAuto.gameObject, (object) "orienttopath", (object) moveToTargetAuto.m_OrientToPath);
    iTween.MoveTo(moveToTargetAuto.m_OnlyMoveContainer ? moveToTargetAuto.m_ObjectContainer : moveToTargetAuto.gameObject, args);
  }

  private void OnMoveToTargetComplete()
  {
    if (this.m_DisableContainerAfterAction)
      this.ActivateObjectContainer(false);
    this.ChangeState(SpellStateType.DEATH);
  }

  private void StopWaitingToAct() => this.m_waitingToAct = false;

  private void ResetPath()
  {
    this.m_pathNodes = new Vector3[3]
    {
      Vector3.zero,
      Vector3.zero,
      Vector3.zero
    };
    this.m_sourceComputed = false;
    this.m_targetComputed = false;
  }

  private void DoActionFallback(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.ChangeState(SpellStateType.DEATH);
  }

  private void SetStartPosition()
  {
    this.transform.position = this.m_pathNodes[0];
    if (!this.m_OnlyMoveContainer)
      return;
    this.m_ObjectContainer.transform.position = this.transform.position;
  }

  private bool DeterminePath(Player sourcePlayer, Card sourceCard, Card targetCard)
  {
    this.FixupPathNodes(sourcePlayer, sourceCard, targetCard);
    this.SetStartPosition();
    return true;
  }

  private void FixupPathNodes(Player sourcePlayer, Card sourceCard, Card targetCard)
  {
    if (!this.m_sourceComputed)
    {
      this.m_pathNodes[0] = this.transform.position;
      this.m_sourceComputed = true;
    }
    if (!this.m_targetComputed && (Object) targetCard != (Object) null)
    {
      this.m_pathNodes[this.m_pathNodes.Length - 1] = targetCard.transform.position;
      double f = (double) targetCard.transform.position.x - (double) this.transform.position.x;
      float a = (float) f / Mathf.Abs((float) f);
      for (int index = 1; index < this.m_pathNodes.Length - 1; ++index)
      {
        float num = this.m_pathNodes[index].x - this.transform.position.x;
        float b = num / Mathf.Sqrt(num * num);
        if (Mathf.Approximately(a, b))
          this.m_pathNodes[index].x = this.transform.position.x - num;
      }
      this.m_targetComputed = true;
    }
    this.MoveCenterPoint();
  }

  private void MoveCenterPoint()
  {
    if (this.m_pathNodes.Length < 3)
      return;
    Vector3 pathNode1 = this.m_pathNodes[0];
    Vector3 pathNode2 = this.m_pathNodes[this.m_pathNodes.Length - 1];
    float num1 = Vector3.Distance(pathNode1, pathNode2);
    Vector3 vector3_1 = (pathNode2 - pathNode1) / num1;
    Vector3 vector3_2 = pathNode1 + vector3_1 * (num1 * (this.CenterOffsetPercent * 0.01f));
    float num2 = num1 / this.DistanceScaleFactor;
    if ((double) this.CenterPointHeightMin == (double) this.CenterPointHeightMax)
      vector3_2[1] += this.CenterPointHeightMax * num2;
    else
      vector3_2[1] += Random.Range(this.CenterPointHeightMin * num2, this.CenterPointHeightMax * num2);
    float num3 = 1f;
    if ((double) pathNode1[2] > (double) pathNode2[2])
      num3 = -1f;
    bool flag = false;
    if ((double) Random.value > 0.5)
      flag = true;
    if ((double) this.RightMin == 0.0 && (double) this.RightMax == 0.0)
      flag = false;
    if ((double) this.LeftMin == 0.0 && (double) this.LeftMax == 0.0)
      flag = true;
    if (flag)
    {
      if ((double) this.RightMin == (double) this.RightMax || this.DebugForceMax)
        vector3_2[0] += this.RightMax * num2 * num3;
      else
        vector3_2[0] += Random.Range(this.RightMin * num2, this.RightMax * num2) * num3;
    }
    else if ((double) this.LeftMin == (double) this.LeftMax || this.DebugForceMax)
      vector3_2[0] -= this.LeftMax * num2 * num3;
    else
      vector3_2[0] -= Random.Range(this.LeftMin * num2, this.LeftMax * num2) * num3;
    this.m_pathNodes[1] = vector3_2;
  }
}
