using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMoveToTarget : Spell
{
  public float m_MovementDurationSec = 1f;
  public iTween.EaseType m_EaseType = iTween.EaseType.easeInOutSine;
  public bool m_DisableContainerAfterAction;
  public bool m_OnlyMoveContainer;
  public bool m_OrientToPath;
  public List<SpellPath> m_Paths;
  private bool m_waitingToAct = true;
  private SpellPath m_spellPath;
  private Vector3[] m_pathNodes;
  private bool m_sourceComputed;
  private bool m_targetComputed;

  public override void SetSource(GameObject go)
  {
    if ((UnityEngine.Object) this.GetSource() != (UnityEngine.Object) go)
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
    if ((UnityEngine.Object) this.GetTarget() != (UnityEngine.Object) go)
      this.m_targetComputed = false;
    base.AddTarget(go);
  }

  public override bool RemoveTarget(GameObject go)
  {
    GameObject target = this.GetTarget();
    if (!base.RemoveTarget(go))
      return false;
    if ((UnityEngine.Object) target == (UnityEngine.Object) go)
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
    if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
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
    Card sourceCard = this.GetSourceCard();
    if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("SpellMoveToTarget.OnAction() - no source card"));
      this.DoActionFallback(prevStateType);
    }
    else
    {
      Card targetCard = this.GetTargetCard();
      if ((UnityEngine.Object) targetCard == (UnityEngine.Object) null)
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

  public override void OnSpellFinished()
  {
    base.OnSpellFinished();
    this.ResetPath();
  }

  protected IEnumerator WaitThenDoAction(SpellStateType prevStateType)
  {
    SpellMoveToTarget spellMoveToTarget = this;
    while (spellMoveToTarget.m_waitingToAct)
      yield return (object) null;
    Hashtable args = iTween.Hash((object) "path", (object) spellMoveToTarget.m_pathNodes, (object) "time", (object) spellMoveToTarget.m_MovementDurationSec, (object) "easetype", (object) spellMoveToTarget.m_EaseType, (object) "oncomplete", (object) "OnMoveToTargetComplete", (object) "oncompletetarget", (object) spellMoveToTarget.gameObject, (object) "orienttopath", (object) spellMoveToTarget.m_OrientToPath);
    iTween.MoveTo(spellMoveToTarget.m_OnlyMoveContainer ? spellMoveToTarget.m_ObjectContainer : spellMoveToTarget.gameObject, args);
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
    this.m_spellPath = (SpellPath) null;
    this.m_pathNodes = (Vector3[]) null;
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
    if (this.m_pathNodes == null)
    {
      if (this.m_Paths == null || this.m_Paths.Count == 0)
      {
        Debug.LogError((object) string.Format("SpellMoveToTarget.DeterminePath() - no SpellPaths available"));
        return false;
      }
      iTweenPath[] components = this.GetComponents<iTweenPath>();
      if (components == null || components.Length == 0)
      {
        Debug.LogError((object) string.Format("SpellMoveToTarget.DeterminePath() - no iTweenPaths available"));
        return false;
      }
      iTweenPath tweenPath;
      SpellPath spellPath;
      if (!this.FindBestPath(sourcePlayer, sourceCard, components, out tweenPath, out spellPath) && !this.FindFallbackPath(components, out tweenPath, out spellPath))
        return false;
      this.m_spellPath = spellPath;
      this.m_pathNodes = tweenPath.nodes.ToArray();
    }
    this.FixupPathNodes(sourcePlayer, sourceCard, targetCard);
    this.SetStartPosition();
    return true;
  }

  private bool FindBestPath(
    Player sourcePlayer,
    Card sourceCard,
    iTweenPath[] pathComponents,
    out iTweenPath tweenPath,
    out SpellPath spellPath)
  {
    tweenPath = (iTweenPath) null;
    spellPath = (SpellPath) null;
    if (sourcePlayer == null)
      return false;
    if (sourcePlayer.GetSide() == Player.Side.FRIENDLY)
    {
      Predicate<SpellPath> match = (Predicate<SpellPath>) (currSpellPath => currSpellPath.m_Type == SpellPathType.FRIENDLY);
      return this.FindPath(pathComponents, out tweenPath, out spellPath, match);
    }
    if (sourcePlayer.GetSide() != Player.Side.OPPOSING)
      return false;
    Predicate<SpellPath> match1 = (Predicate<SpellPath>) (currSpellPath => currSpellPath.m_Type == SpellPathType.OPPOSING);
    return this.FindPath(pathComponents, out tweenPath, out spellPath, match1);
  }

  private bool FindFallbackPath(
    iTweenPath[] pathComponents,
    out iTweenPath tweenPath,
    out SpellPath spellPath)
  {
    Predicate<SpellPath> match = (Predicate<SpellPath>) (currSpellPath => currSpellPath != null);
    return this.FindPath(pathComponents, out tweenPath, out spellPath, match);
  }

  private bool FindPath(
    iTweenPath[] pathComponents,
    out iTweenPath tweenPath,
    out SpellPath spellPath,
    Predicate<SpellPath> match)
  {
    tweenPath = (iTweenPath) null;
    spellPath = (SpellPath) null;
    SpellPath spellPath1 = this.m_Paths.Find(match);
    if (spellPath1 == null)
      return false;
    string desiredSpellPathName = spellPath1.m_PathName.ToLower().Trim();
    iTweenPath iTweenPath = Array.Find<iTweenPath>(pathComponents, (Predicate<iTweenPath>) (currTweenPath => currTweenPath.pathName.ToLower().Trim() == desiredSpellPathName));
    if ((UnityEngine.Object) iTweenPath == (UnityEngine.Object) null || iTweenPath.nodes == null || iTweenPath.nodes.Count == 0)
      return false;
    tweenPath = iTweenPath;
    spellPath = spellPath1;
    return true;
  }

  private void FixupPathNodes(Player sourcePlayer, Card sourceCard, Card targetCard)
  {
    if (!this.m_sourceComputed)
    {
      this.m_pathNodes[0] = this.transform.position + this.m_spellPath.m_FirstNodeOffset;
      this.m_sourceComputed = true;
    }
    if (this.m_targetComputed || !((UnityEngine.Object) targetCard != (UnityEngine.Object) null))
      return;
    this.m_pathNodes[this.m_pathNodes.Length - 1] = targetCard.transform.position + this.m_spellPath.m_LastNodeOffset;
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
}
