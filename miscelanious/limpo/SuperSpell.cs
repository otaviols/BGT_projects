using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using HutongGames.PlayMaker;
using PegasusGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSpell : Spell
{
  public bool m_MakeClones = true;
  [Tooltip("If used as a subspell, setting this to true will skip auto-cleanup done by the SubspellController. Do not use unless you're sure you want this effect to stay around until the scene is cleaned up!")]
  public bool m_SkipAutoDestroyForSubspell;
  public SpellTargetInfo m_TargetInfo = new SpellTargetInfo();
  public SpellStartInfo m_StartInfo;
  public SpellActionInfo m_ActionInfo;
  public SpellMissileInfo m_MissileInfo;
  public SpellImpactInfo m_ImpactInfo;
  public SpellAreaEffectInfo m_FriendlyAreaEffectInfo;
  public SpellAreaEffectInfo m_OpponentAreaEffectInfo;
  [HideInInspector]
  public SpellChainInfo m_ChainInfo;
  protected Spell m_startSpell;
  protected List<GameObject> m_visualTargets = new List<GameObject>();
  protected int m_currentTargetIndex;
  protected int m_effectsPendingFinish;
  protected bool m_pendingNoneStateChange;
  protected bool m_pendingSpellFinish;
  protected List<Spell> m_activeClonedSpells = new List<Spell>();
  protected Map<int, int> m_visualToTargetIndexMap = new Map<int, int>();
  protected Map<int, int> m_targetToMetaDataMap = new Map<int, int>();
  protected bool m_settingUpAction;
  protected Spell m_activeAreaEffectSpell;
  private readonly List<int> m_createCardsInSetAside = new List<int>();

  public override List<GameObject> GetVisualTargets() => this.m_visualTargets;

  public override GameObject GetVisualTarget() => this.m_visualTargets.Count != 0 ? this.m_visualTargets[0] : (GameObject) null;

  public override void AddVisualTarget(GameObject go) => this.m_visualTargets.Add(go);

  public override void AddVisualTargets(List<GameObject> targets) => this.m_visualTargets.AddRange((IEnumerable<GameObject>) targets);

  public override bool RemoveVisualTarget(GameObject go) => this.m_visualTargets.Remove(go);

  public override void RemoveAllVisualTargets() => this.m_visualTargets.Clear();

  public override bool IsVisualTarget(GameObject go) => this.m_visualTargets.Contains(go);

  public override Card GetVisualTargetCard()
  {
    GameObject visualTarget = this.GetVisualTarget();
    return (UnityEngine.Object) visualTarget == (UnityEngine.Object) null ? (Card) null : visualTarget.GetComponent<Card>();
  }

  protected bool AddPowerTargetsInternal(bool fallbackToStartBlockTarget)
  {
    this.m_visualToTargetIndexMap.Clear();
    this.m_targetToMetaDataMap.Clear();
    if (!this.CanAddPowerTargets() || this.HasChain() && !this.AddPrimaryChainTarget() || !this.AddMultiplePowerTargets())
      return false;
    if (this.m_targets.Count > 0 || !fallbackToStartBlockTarget)
      return true;
    Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
    return blockStart == null || blockStart.Target == 0 || this.AddSinglePowerTarget_FromBlockStart(blockStart);
  }

  public override bool AddPowerTargets() => this.AddPowerTargetsInternal(true);

  protected override void AddTargetFromMetaData(int metaDataIndex, Card targetCard)
  {
    this.m_targetToMetaDataMap[this.m_targets.Count] = metaDataIndex;
    this.AddTarget(targetCard.gameObject);
  }

  protected override void OnBirth(SpellStateType prevStateType)
  {
    this.UpdatePosition();
    this.UpdateOrientation();
    this.m_currentTargetIndex = 0;
    if (this.HasStart())
    {
      this.SpawnStart();
      this.m_startSpell.SafeActivateState(SpellStateType.BIRTH);
      if (this.m_startSpell.GetActiveState() == SpellStateType.NONE)
        this.m_startSpell = (Spell) null;
    }
    base.OnBirth(prevStateType);
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.m_settingUpAction = true;
    this.UpdateTargets();
    if (this.m_Location == SpellLocation.CHOSEN_TARGET)
      this.m_positionDirty = true;
    this.UpdatePosition();
    if (this.m_Facing == SpellFacing.TOWARDS_CHOSEN_TARGET)
      this.m_orientationDirty = true;
    this.UpdateOrientation();
    this.m_currentTargetIndex = this.GetPrimaryTargetIndex();
    this.UpdatePendingStateChangeFlags(SpellStateType.ACTION);
    this.DoAction();
    base.OnAction(prevStateType);
    this.m_settingUpAction = false;
    this.FinishIfPossible();
  }

  protected override void OnCancel(SpellStateType prevStateType)
  {
    this.UpdatePendingStateChangeFlags(SpellStateType.CANCEL);
    if ((UnityEngine.Object) this.m_startSpell != (UnityEngine.Object) null)
    {
      this.m_startSpell.SafeActivateState(SpellStateType.CANCEL);
      this.m_startSpell = (Spell) null;
    }
    base.OnCancel(prevStateType);
    this.FinishIfPossible();
  }

  public override void OnStateFinished()
  {
    if (this.GuessNextStateType() == SpellStateType.NONE && this.AreEffectsActive())
      this.m_pendingNoneStateChange = true;
    else
      base.OnStateFinished();
  }

  public override void OnSpellFinished()
  {
    if (this.AreEffectsActive())
      this.m_pendingSpellFinish = true;
    else
      base.OnSpellFinished();
  }

  public override void OnFsmStateStarted(FsmState state, SpellStateType stateType)
  {
    if (this.m_activeStateChange == stateType)
      return;
    if (stateType == SpellStateType.NONE && this.AreEffectsActive())
    {
      this.m_pendingSpellFinish = true;
      this.m_pendingNoneStateChange = true;
    }
    else
      base.OnFsmStateStarted(state, stateType);
  }

  public override bool CanPurge() => this.m_activeClonedSpells.Count <= 0 && base.CanPurge();

  public void ActivateFinisher(bool opponentFinisher = false)
  {
    this.m_ImpactInfo.m_AdjustRotation = opponentFinisher;
    this.m_StartInfo.m_AdjustRotation = opponentFinisher;
    this.Activate();
  }

  private void DoAction()
  {
    if (this.CheckAndWaitForGameEventsThenDoAction() || this.CheckAndWaitForStartDelayThenDoAction() || this.CheckAndWaitForStartPrefabThenDoAction())
      return;
    this.DoActionNow();
  }

  private bool CheckAndWaitForGameEventsThenDoAction()
  {
    if (this.m_taskList == null)
      return false;
    if (this.m_ActionInfo.m_ShowSpellVisuals == SpellVisualShowTime.DURING_GAME_EVENTS)
      return this.DoActionDuringGameEvents();
    if (this.m_ActionInfo.m_ShowSpellVisuals != SpellVisualShowTime.AFTER_GAME_EVENTS)
      return false;
    this.DoActionAfterGameEvents();
    return true;
  }

  private bool DoActionDuringGameEvents()
  {
    this.m_taskList.DoAllTasks();
    if (this.m_taskList.IsComplete())
      return false;
    QueueList<PowerTask> tasksToWaitFor = this.DetermineTasksToWaitFor(0, this.m_taskList.GetTaskList().Count);
    if (tasksToWaitFor.Count == 0)
      return false;
    this.StartCoroutine(this.DoDelayedActionDuringGameEvents(tasksToWaitFor));
    return true;
  }

  private IEnumerator DoDelayedActionDuringGameEvents(QueueList<PowerTask> tasksToWaitFor)
  {
    SuperSpell superSpell = this;
    ++superSpell.m_effectsPendingFinish;
    yield return (object) superSpell.StartCoroutine(superSpell.WaitForTasks(tasksToWaitFor));
    --superSpell.m_effectsPendingFinish;
    if (!superSpell.CheckAndWaitForStartDelayThenDoAction() && !superSpell.CheckAndWaitForStartPrefabThenDoAction())
      superSpell.DoActionNow();
  }

  private Entity GetEntityFromZoneChangePowerTask(PowerTask task)
  {
    Entity entity;
    this.GetZoneChangeFromPowerTask(task, out entity, out int _);
    return entity;
  }

  private bool GetZoneChangeFromPowerTask(PowerTask task, out Entity entity, out int zoneTag)
  {
    entity = (Entity) null;
    zoneTag = 0;
    Network.PowerHistory power = task.GetPower();
    switch (power.Type)
    {
      case Network.PowerType.FULL_ENTITY:
        Network.HistFullEntity histFullEntity = (Network.HistFullEntity) power;
        Entity entity1 = GameState.Get().GetEntity(histFullEntity.Entity.ID);
        if ((UnityEngine.Object) entity1.GetCard() == (UnityEngine.Object) null)
          return false;
        using (List<Network.Entity.Tag>.Enumerator enumerator = histFullEntity.Entity.Tags.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Network.Entity.Tag current = enumerator.Current;
            if (current.Name == 49)
            {
              entity = entity1;
              zoneTag = current.Value;
              return true;
            }
          }
          break;
        }
      case Network.PowerType.SHOW_ENTITY:
        Network.HistShowEntity histShowEntity = (Network.HistShowEntity) power;
        Entity entity2 = GameState.Get().GetEntity(histShowEntity.Entity.ID);
        if ((UnityEngine.Object) entity2.GetCard() == (UnityEngine.Object) null)
          return false;
        using (List<Network.Entity.Tag>.Enumerator enumerator = histShowEntity.Entity.Tags.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Network.Entity.Tag current = enumerator.Current;
            if (current.Name == 49)
            {
              entity = entity2;
              zoneTag = current.Value;
              return true;
            }
          }
          break;
        }
      case Network.PowerType.TAG_CHANGE:
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        Entity entity3 = GameState.Get().GetEntity(histTagChange.Entity);
        if ((UnityEngine.Object) entity3.GetCard() == (UnityEngine.Object) null || histTagChange.Tag != 49)
          return false;
        entity = entity3;
        zoneTag = histTagChange.Value;
        return true;
    }
    return false;
  }

  private void DoActionAfterGameEvents()
  {
    ++this.m_effectsPendingFinish;
    this.m_taskList.DoAllTasks((PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) =>
    {
      --this.m_effectsPendingFinish;
      if (this.CheckAndWaitForStartDelayThenDoAction() || this.CheckAndWaitForStartPrefabThenDoAction())
        return;
      this.DoActionNow();
    }));
  }

  private bool CheckAndWaitForStartDelayThenDoAction()
  {
    if ((double) Mathf.Min(this.m_ActionInfo.m_StartDelayMax, this.m_ActionInfo.m_StartDelayMin) <= (double) Mathf.Epsilon)
      return false;
    ++this.m_effectsPendingFinish;
    this.StartCoroutine(this.WaitForStartDelayThenDoAction());
    return true;
  }

  private IEnumerator WaitForStartDelayThenDoAction()
  {
    yield return (object) new WaitForSeconds(UnityEngine.Random.Range(this.m_ActionInfo.m_StartDelayMin, this.m_ActionInfo.m_StartDelayMax));
    --this.m_effectsPendingFinish;
    if (!this.CheckAndWaitForStartPrefabThenDoAction())
      this.DoActionNow();
  }

  private bool CheckAndWaitForStartPrefabThenDoAction()
  {
    if (!this.HasStart() || (UnityEngine.Object) this.m_startSpell != (UnityEngine.Object) null && this.m_startSpell.GetActiveState() == SpellStateType.IDLE)
      return false;
    if ((UnityEngine.Object) this.m_startSpell == (UnityEngine.Object) null)
      this.SpawnStart();
    this.m_startSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnStartSpellBirthStateFinished));
    if (this.m_startSpell.GetActiveState() != SpellStateType.BIRTH)
    {
      this.m_startSpell.SafeActivateState(SpellStateType.BIRTH);
      if (this.m_startSpell.GetActiveState() == SpellStateType.NONE)
      {
        this.m_startSpell = (Spell) null;
        return false;
      }
    }
    return true;
  }

  private void OnStartSpellBirthStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (prevStateType != SpellStateType.BIRTH)
      return;
    spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnStartSpellBirthStateFinished), userData);
    this.DoActionNow();
  }

  protected virtual void DoActionNow()
  {
    SpellAreaEffectInfo areaEffectInfo = this.DetermineAreaEffectInfo();
    if (areaEffectInfo != null)
      this.SpawnAreaEffect(areaEffectInfo);
    bool flag1 = this.HasMissile();
    bool flag2 = this.HasImpact();
    bool flag3 = this.HasChain();
    if (this.GetVisualTargetCount() > 0 && flag1 | flag2 | flag3)
    {
      if (flag1)
      {
        if (flag3)
          this.SpawnChainMissile();
        else if (this.m_MissileInfo.m_SpawnInSequence)
          this.SpawnMissileInSequence();
        else
          this.SpawnAllMissiles();
      }
      else
      {
        if (flag2)
        {
          if (flag3)
            this.SpawnImpact(this.m_currentTargetIndex);
          else
            this.SpawnAllImpacts();
        }
        if (flag3)
          this.SpawnChain();
        this.DoStartSpellAction();
      }
    }
    else
      this.DoStartSpellAction();
    this.FinishIfPossible();
  }

  private bool HasStart() => this.m_StartInfo != null && this.m_StartInfo.m_Enabled && (UnityEngine.Object) this.m_StartInfo.m_Prefab != (UnityEngine.Object) null;

  private void SpawnStart()
  {
    ++this.m_effectsPendingFinish;
    this.m_startSpell = this.CloneSpell(this.m_StartInfo.m_Prefab);
    this.m_startSpell.SetSource(this.GetSource());
    this.m_startSpell.AddTargets(this.GetTargets());
    if (this.m_StartInfo.m_UseSuperSpellLocation)
      this.m_startSpell.SetPosition(this.transform.position);
    if (!this.m_StartInfo.m_AdjustRotation || !(this.m_StartInfo.m_StartRotationAdjustment != Vector3.zero))
      return;
    this.m_startSpell.transform.Rotate(this.m_StartInfo.m_StartRotationAdjustment);
    this.m_startSpell.UpdateOrientation();
  }

  private void DoStartSpellAction()
  {
    if ((UnityEngine.Object) this.m_startSpell == (UnityEngine.Object) null)
      return;
    if (!this.m_startSpell.HasUsableState(SpellStateType.ACTION))
    {
      this.m_startSpell.UpdateTransform();
      this.m_startSpell.SafeActivateState(SpellStateType.DEATH);
    }
    else
    {
      this.m_startSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnStartSpellActionFinished));
      this.m_startSpell.ActivateState(SpellStateType.ACTION);
    }
    this.m_startSpell = (Spell) null;
  }

  private void OnStartSpellActionFinished(Spell spell, object userData)
  {
    if (spell.GetActiveState() != SpellStateType.ACTION)
      return;
    spell.SafeActivateState(SpellStateType.DEATH);
  }

  private bool HasMissile()
  {
    if (this.m_MissileInfo == null || !this.m_MissileInfo.m_Enabled)
      return false;
    return (UnityEngine.Object) this.m_MissileInfo.m_Prefab != (UnityEngine.Object) null || (UnityEngine.Object) this.m_MissileInfo.m_ReversePrefab != (UnityEngine.Object) null;
  }

  private void SpawnChainMissile()
  {
    this.SpawnMissile(this.GetPrimaryTargetIndex());
    this.DoStartSpellAction();
  }

  private void SpawnMissileInSequence()
  {
    if (this.m_currentTargetIndex >= this.GetVisualTargetCount())
      return;
    this.SpawnMissile(this.m_currentTargetIndex);
    ++this.m_currentTargetIndex;
    if ((UnityEngine.Object) this.m_startSpell == (UnityEngine.Object) null)
      return;
    if (this.m_StartInfo.m_DeathAfterAllMissilesFire)
    {
      if (this.m_currentTargetIndex < this.GetVisualTargetCount())
      {
        if (!this.m_startSpell.HasUsableState(SpellStateType.ACTION))
          return;
        this.m_startSpell.ActivateState(SpellStateType.ACTION);
      }
      else
        this.DoStartSpellAction();
    }
    else
      this.DoStartSpellAction();
  }

  private void SpawnAllMissiles()
  {
    for (int targetIndex = 0; targetIndex < this.GetVisualTargetCount(); ++targetIndex)
      this.SpawnMissile(targetIndex);
    this.DoStartSpellAction();
  }

  private void SpawnMissile(int targetIndex)
  {
    ++this.m_effectsPendingFinish;
    this.StartCoroutine(this.WaitAndSpawnMissile(targetIndex));
  }

  private IEnumerator WaitAndSpawnMissile(int targetIndex)
  {
    SuperSpell superSpell = this;
    float seconds = UnityEngine.Random.Range(superSpell.m_MissileInfo.m_SpawnDelaySecMin, superSpell.m_MissileInfo.m_SpawnDelaySecMax);
    if (!superSpell.m_MissileInfo.m_SpawnInSequence || targetIndex == 0)
      yield return (object) new WaitForSeconds(seconds);
    if ((double) superSpell.m_MissileInfo.m_SpawnOffset > 0.0 && targetIndex > 0)
      yield return (object) new WaitForSeconds(superSpell.m_MissileInfo.m_SpawnOffset * (float) targetIndex);
    int dataIndexForTarget = superSpell.GetMetaDataIndexForTarget(targetIndex);
    if (superSpell.ShouldCompleteTasksUntilMetaData(dataIndexForTarget))
      yield return (object) superSpell.StartCoroutine(superSpell.CompleteTasksUntilMetaData(dataIndexForTarget));
    if (superSpell.m_visualTargets.Count <= targetIndex || (UnityEngine.Object) superSpell.m_visualTargets[targetIndex] == (UnityEngine.Object) null)
    {
      --superSpell.m_effectsPendingFinish;
    }
    else
    {
      GameObject source = superSpell.GetSource();
      GameObject visualTarget = superSpell.m_visualTargets[targetIndex];
      if ((UnityEngine.Object) superSpell.m_MissileInfo.m_Prefab != (UnityEngine.Object) null)
      {
        Spell spell;
        if (superSpell.m_MissileInfo.m_UseSuperSpellLocation)
        {
          spell = superSpell.CloneSpell(superSpell.m_MissileInfo.m_Prefab, new Vector3?(superSpell.transform.position));
          spell.ClearPositionDirtyFlag();
        }
        else
          spell = superSpell.CloneSpell(superSpell.m_MissileInfo.m_Prefab);
        spell.SetSource(source);
        spell.AddTarget(visualTarget);
        spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(superSpell.OnMissileSpellStateFinished), (object) targetIndex);
        spell.ActivateState(SpellStateType.BIRTH);
      }
      else
        --superSpell.m_effectsPendingFinish;
      if ((UnityEngine.Object) superSpell.m_MissileInfo.m_ReversePrefab != (UnityEngine.Object) null)
      {
        ++superSpell.m_effectsPendingFinish;
        superSpell.StartCoroutine(superSpell.SpawnReverseMissile(superSpell.m_MissileInfo.m_ReversePrefab, source, visualTarget, superSpell.m_MissileInfo.m_reverseDelay));
      }
    }
  }

  private IEnumerator SpawnReverseMissile(
    Spell cloneSpell,
    GameObject sourceObject,
    GameObject targetObject,
    float delay)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    SuperSpell superSpell = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Spell spell = superSpell.CloneSpell(cloneSpell);
      spell.SetSource(targetObject);
      spell.AddTarget(sourceObject);
      spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(superSpell.OnMissileSpellStateFinished), (object) -1);
      spell.ActivateState(SpellStateType.BIRTH);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(delay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void OnMissileSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (prevStateType != SpellStateType.BIRTH)
      return;
    spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnMissileSpellStateFinished), userData);
    int targetIndex = (int) userData;
    bool reverse = targetIndex < 0;
    this.FireMissileOnPath(spell, targetIndex, reverse);
  }

  private void FireMissileOnPath(Spell missile, int targetIndex, bool reverse)
  {
    Vector3[] missilePath = this.GenerateMissilePath(missile);
    float num = UnityEngine.Random.Range(this.m_MissileInfo.m_PathDurationMin, this.m_MissileInfo.m_PathDurationMax);
    Hashtable args = iTween.Hash((object) "path", (object) missilePath, (object) "time", (object) num, (object) "easetype", (object) this.m_MissileInfo.m_PathEaseType, (object) "oncompletetarget", (object) this.gameObject);
    if (reverse)
    {
      args.Add((object) "oncomplete", (object) "OnReverseMissileTargetReached");
      args.Add((object) "oncompleteparams", (object) missile);
    }
    else
    {
      Hashtable hashtable = iTween.Hash((object) nameof (missile), (object) missile, (object) nameof (targetIndex), (object) targetIndex);
      args.Add((object) "oncomplete", (object) "OnMissileTargetReached");
      args.Add((object) "oncompleteparams", (object) hashtable);
    }
    if (!object.Equals((object) missilePath[0], (object) missilePath[2]))
      args.Add((object) "orienttopath", (object) this.m_MissileInfo.m_OrientToPath);
    if (this.m_MissileInfo.m_TargetJoint.Length > 0)
    {
      GameObject childBySubstring = GameObjectUtils.FindChildBySubstring(missile.gameObject, this.m_MissileInfo.m_TargetJoint);
      if ((UnityEngine.Object) childBySubstring != (UnityEngine.Object) null)
      {
        missile.transform.LookAt(missile.GetTarget().transform, this.m_MissileInfo.m_JointUpVector);
        missilePath[2].y += this.m_MissileInfo.m_TargetHeightOffset;
        iTween.MoveTo(childBySubstring, args);
        return;
      }
    }
    iTween.MoveTo(missile.gameObject, args);
  }

  private Vector3[] GenerateMissilePath(Spell missile)
  {
    Vector3[] path = new Vector3[3];
    path[0] = missile.transform.position;
    Card targetCard = missile.GetTargetCard();
    if ((UnityEngine.Object) targetCard != (UnityEngine.Object) null && targetCard.GetZone() is ZoneHand && !this.m_MissileInfo.m_UseTargetCardPositionInsteadOfHandSlot)
    {
      ZoneHand zone = targetCard.GetZone() as ZoneHand;
      path[2] = zone.GetCardPosition(zone.GetCardSlot(targetCard), -1);
    }
    else
      path[2] = missile.GetTarget().transform.position;
    path[1] = this.GenerateMissilePathCenterPoint(path);
    return path;
  }

  private Vector3 GenerateMissilePathCenterPoint(Vector3[] path)
  {
    Vector3 vector3_1 = path[0];
    Vector3 vector3_2 = path[2];
    Vector3 vector3_3 = vector3_2 - vector3_1;
    double magnitude = (double) vector3_3.magnitude;
    Vector3 missilePathCenterPoint = vector3_1;
    bool flag1 = magnitude <= (double) Mathf.Epsilon;
    if (!flag1)
      missilePathCenterPoint = vector3_1 + vector3_3 * (this.m_MissileInfo.m_CenterOffsetPercent * 0.01f);
    float num1 = (float) magnitude / this.m_MissileInfo.m_DistanceScaleFactor;
    if (flag1)
    {
      if ((double) this.m_MissileInfo.m_CenterPointHeightMin <= (double) Mathf.Epsilon && (double) this.m_MissileInfo.m_CenterPointHeightMax <= (double) Mathf.Epsilon)
        missilePathCenterPoint.y += 2f;
      else
        missilePathCenterPoint.y += UnityEngine.Random.Range(this.m_MissileInfo.m_CenterPointHeightMin, this.m_MissileInfo.m_CenterPointHeightMax);
    }
    else
      missilePathCenterPoint.y += num1 * UnityEngine.Random.Range(this.m_MissileInfo.m_CenterPointHeightMin, this.m_MissileInfo.m_CenterPointHeightMax);
    float num2 = 1f;
    if ((double) vector3_1.z > (double) vector3_2.z)
      num2 = -1f;
    bool flag2 = GeneralUtils.RandomBool();
    if ((double) this.m_MissileInfo.m_RightMin == 0.0 && (double) this.m_MissileInfo.m_RightMax == 0.0)
      flag2 = false;
    if ((double) this.m_MissileInfo.m_LeftMin == 0.0 && (double) this.m_MissileInfo.m_LeftMax == 0.0)
      flag2 = true;
    if (flag2)
    {
      if ((double) this.m_MissileInfo.m_RightMin == (double) this.m_MissileInfo.m_RightMax || this.m_MissileInfo.m_DebugForceMax)
        missilePathCenterPoint.x += this.m_MissileInfo.m_RightMax * num1 * num2;
      else
        missilePathCenterPoint.x += UnityEngine.Random.Range(this.m_MissileInfo.m_RightMin * num1, this.m_MissileInfo.m_RightMax * num1) * num2;
    }
    else if ((double) this.m_MissileInfo.m_LeftMin == (double) this.m_MissileInfo.m_LeftMax || this.m_MissileInfo.m_DebugForceMax)
      missilePathCenterPoint.x -= this.m_MissileInfo.m_LeftMax * num1 * num2;
    else
      missilePathCenterPoint.x -= UnityEngine.Random.Range(this.m_MissileInfo.m_LeftMin * num1, this.m_MissileInfo.m_LeftMax * num1) * num2;
    return missilePathCenterPoint;
  }

  private void OnMissileTargetReached(Hashtable args)
  {
    Spell spell = (Spell) args[(object) "missile"];
    int targetIndex = (int) args[(object) "targetIndex"];
    if (this.HasImpact())
      this.SpawnImpact(targetIndex);
    if (this.HasChain())
      this.SpawnChain();
    else if (this.m_MissileInfo.m_SpawnInSequence)
      this.SpawnMissileInSequence();
    spell.ActivateState(SpellStateType.DEATH);
  }

  private void OnReverseMissileTargetReached(Spell missile) => missile.ActivateState(SpellStateType.DEATH);

  private bool HasImpact()
  {
    if (this.m_ImpactInfo == null || !this.m_ImpactInfo.m_Enabled)
      return false;
    return (UnityEngine.Object) this.m_ImpactInfo.m_Prefab != (UnityEngine.Object) null || this.m_ImpactInfo.m_damageAmountImpactSpells.Length != 0;
  }

  private void SpawnAllImpacts()
  {
    for (int index = 0; index < this.GetVisualTargetCount(); ++index)
    {
      if (this.IsValidSpellTarget(this.m_visualTargets[index]))
        this.SpawnImpact(index);
    }
  }

  private void SpawnImpact(int targetIndex)
  {
    ++this.m_effectsPendingFinish;
    this.StartCoroutine(this.WaitAndSpawnImpact(targetIndex));
  }

  private IEnumerator WaitAndSpawnImpact(int targetIndex)
  {
    SuperSpell superSpell = this;
    yield return (object) new WaitForSeconds(UnityEngine.Random.Range(superSpell.m_ImpactInfo.m_SpawnDelaySecMin, superSpell.m_ImpactInfo.m_SpawnDelaySecMax));
    if ((double) superSpell.m_ImpactInfo.m_SpawnOffset > 0.0 && targetIndex > 0)
      yield return (object) new WaitForSeconds(superSpell.m_ImpactInfo.m_SpawnOffset * (float) targetIndex);
    int metaDataIndex = superSpell.GetMetaDataIndexForTarget(targetIndex);
    if (metaDataIndex >= 0)
    {
      if (superSpell.ShouldCompleteTasksUntilMetaData(metaDataIndex))
        yield return (object) superSpell.StartCoroutine(superSpell.CompleteTasksUntilMetaData(metaDataIndex));
      float delaySec = UnityEngine.Random.Range(superSpell.m_ImpactInfo.m_GameDelaySecMin, superSpell.m_ImpactInfo.m_GameDelaySecMax);
      superSpell.StartCoroutine(superSpell.CompleteTasksFromMetaData(metaDataIndex, delaySec));
    }
    if (superSpell.m_visualTargets.Count <= targetIndex || (UnityEngine.Object) superSpell.m_visualTargets[targetIndex] == (UnityEngine.Object) null)
    {
      --superSpell.m_effectsPendingFinish;
    }
    else
    {
      GameObject source = superSpell.GetSource();
      GameObject visualTarget = superSpell.m_visualTargets[targetIndex];
      Spell impactPrefab = superSpell.DetermineImpactPrefab(visualTarget);
      Spell spell = superSpell.CloneSpell(impactPrefab);
      spell.SetSource(source);
      spell.AddTarget(visualTarget);
      if (superSpell.m_ImpactInfo.m_UseSuperSpellLocation)
      {
        spell.SetPosition(superSpell.transform.position);
      }
      else
      {
        if (superSpell.IsMakingClones())
        {
          spell.m_Location = superSpell.m_ImpactInfo.m_Location;
          spell.m_SetParentToLocation = superSpell.m_ImpactInfo.m_SetParentToLocation;
        }
        spell.UpdatePosition();
        if (superSpell.m_ImpactInfo.m_AdjustRotation && superSpell.m_ImpactInfo.m_ImpactRotationAdjustment != Vector3.zero)
          spell.transform.Rotate(superSpell.m_ImpactInfo.m_ImpactRotationAdjustment);
      }
      spell.UpdateOrientation();
      spell.Activate();
    }
  }

  private Spell DetermineImpactPrefab(GameObject targetObject)
  {
    if (this.m_ImpactInfo.m_damageAmountImpactSpells.Length == 0)
      return this.m_ImpactInfo.m_Prefab;
    Spell impactPrefab = this.m_ImpactInfo.m_Prefab;
    if (this.m_taskList == null)
      return impactPrefab;
    Card component = targetObject.GetComponent<Card>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return impactPrefab;
    PowerTaskList.DamageInfo damageInfo = this.m_taskList.GetDamageInfo(component.GetEntity());
    if (damageInfo == null)
      return impactPrefab;
    SpellValueRange accordingToRanges = SpellUtils.GetAppropriateElementAccordingToRanges<SpellValueRange>(this.m_ImpactInfo.m_damageAmountImpactSpells, (Func<SpellValueRange, ValueRange>) (x => x.m_range), damageInfo.m_damage);
    if (accordingToRanges != null && (UnityEngine.Object) accordingToRanges.m_spellPrefab != (UnityEngine.Object) null)
      impactPrefab = accordingToRanges.m_spellPrefab;
    return impactPrefab;
  }

  private bool HasChain() => this.m_ChainInfo != null && this.m_ChainInfo.m_Enabled && (UnityEngine.Object) this.m_ChainInfo.m_Prefab != (UnityEngine.Object) null;

  private void SpawnChain()
  {
    if (this.GetVisualTargetCount() <= 1)
      return;
    ++this.m_effectsPendingFinish;
    this.StartCoroutine(this.WaitAndSpawnChain());
  }

  private IEnumerator WaitAndSpawnChain()
  {
    yield return (object) new WaitForSeconds(UnityEngine.Random.Range(this.m_ChainInfo.m_SpawnDelayMin, this.m_ChainInfo.m_SpawnDelayMax));
    Spell spell = this.CloneSpell(this.m_ChainInfo.m_Prefab);
    GameObject primaryTarget = this.GetPrimaryTarget();
    spell.SetSource(primaryTarget);
    foreach (GameObject visualTarget in this.m_visualTargets)
    {
      if (!((UnityEngine.Object) visualTarget == (UnityEngine.Object) primaryTarget))
        spell.AddTarget(visualTarget);
    }
    spell.ActivateState(SpellStateType.ACTION);
  }

  private SpellAreaEffectInfo DetermineAreaEffectInfo()
  {
    Card sourceCard = this.GetSourceCard();
    if ((UnityEngine.Object) sourceCard != (UnityEngine.Object) null)
    {
      Player controller = sourceCard.GetController();
      if (controller != null)
      {
        if (controller.IsFriendlySide() && this.HasFriendlyAreaEffect())
          return this.m_FriendlyAreaEffectInfo;
        if (!controller.IsFriendlySide() && this.HasOpponentAreaEffect())
          return this.m_OpponentAreaEffectInfo;
      }
    }
    if (this.HasFriendlyAreaEffect())
      return this.m_FriendlyAreaEffectInfo;
    return this.HasOpponentAreaEffect() ? this.m_OpponentAreaEffectInfo : (SpellAreaEffectInfo) null;
  }

  private bool HasAreaEffect() => this.HasFriendlyAreaEffect() || this.HasOpponentAreaEffect();

  private bool HasFriendlyAreaEffect() => this.m_FriendlyAreaEffectInfo != null && this.m_FriendlyAreaEffectInfo.m_Enabled && (UnityEngine.Object) this.m_FriendlyAreaEffectInfo.m_Prefab != (UnityEngine.Object) null;

  private bool HasOpponentAreaEffect() => this.m_OpponentAreaEffectInfo != null && this.m_OpponentAreaEffectInfo.m_Enabled && (UnityEngine.Object) this.m_OpponentAreaEffectInfo.m_Prefab != (UnityEngine.Object) null;

  private void SpawnAreaEffect(SpellAreaEffectInfo info)
  {
    ++this.m_effectsPendingFinish;
    this.StartCoroutine(this.WaitAndSpawnAreaEffect(info));
  }

  private IEnumerator WaitAndSpawnAreaEffect(SpellAreaEffectInfo info)
  {
    SuperSpell superSpell = this;
    float seconds = UnityEngine.Random.Range(info.m_SpawnDelaySecMin, info.m_SpawnDelaySecMax);
    if ((double) seconds > 0.0)
      yield return (object) new WaitForSeconds(seconds);
    Spell spell = superSpell.CloneSpell(info.m_Prefab);
    spell.SetSource(superSpell.GetSource());
    if (superSpell.m_taskList != null)
      spell.AttachPowerTaskList(superSpell.m_taskList);
    if (info.m_UseSuperSpellLocation)
      spell.SetPosition(superSpell.transform.position);
    else if (superSpell.IsMakingClones() && info.m_Location != SpellLocation.NONE)
    {
      spell.m_Location = info.m_Location;
      spell.m_SetParentToLocation = info.m_SetParentToLocation;
      spell.UpdatePosition();
    }
    if (superSpell.IsMakingClones() && info.m_Facing != SpellFacing.NONE)
    {
      spell.m_Facing = info.m_Facing;
      spell.m_FacingOptions = info.m_FacingOptions;
      spell.UpdateOrientation();
    }
    if (superSpell.OnBeforeActivateAreaEffectSpell != null)
      superSpell.OnBeforeActivateAreaEffectSpell(spell);
    spell.Activate();
    superSpell.m_activeAreaEffectSpell = spell;
  }

  protected Action<Spell> OnBeforeActivateAreaEffectSpell { get; set; }

  private bool AddPrimaryChainTarget()
  {
    Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
    return blockStart != null && this.AddSinglePowerTarget_FromBlockStart(blockStart);
  }

  private int GetPrimaryTargetIndex() => 0;

  private GameObject GetPrimaryTarget() => this.m_visualTargets[this.GetPrimaryTargetIndex()];

  protected virtual void UpdateTargets()
  {
    this.UpdateVisualTargets();
    this.SuppressPlaySoundsOnVisualTargets();
  }

  private int GetVisualTargetCount() => this.IsMakingClones() ? this.m_visualTargets.Count : Mathf.Min(1, this.m_visualTargets.Count);

  protected virtual void UpdateVisualTargets()
  {
    switch (this.m_TargetInfo.m_Behavior)
    {
      case SpellTargetBehavior.FRIENDLY_PLAY_ZONE_CENTER:
        this.AddVisualTarget(SpellUtils.FindFriendlyPlayZone((Spell) this).gameObject);
        break;
      case SpellTargetBehavior.FRIENDLY_PLAY_ZONE_RANDOM:
        this.GenerateRandomPlayZoneVisualTargets(SpellUtils.FindFriendlyPlayZone((Spell) this));
        break;
      case SpellTargetBehavior.OPPONENT_PLAY_ZONE_CENTER:
        this.AddVisualTarget(SpellUtils.FindOpponentPlayZone((Spell) this).gameObject);
        break;
      case SpellTargetBehavior.OPPONENT_PLAY_ZONE_RANDOM:
        this.GenerateRandomPlayZoneVisualTargets(SpellUtils.FindOpponentPlayZone((Spell) this));
        break;
      case SpellTargetBehavior.BOARD_CENTER:
        this.AddVisualTarget(Board.Get().FindBone("CenterPointBone").gameObject);
        break;
      case SpellTargetBehavior.UNTARGETED:
        this.AddVisualTarget(this.GetSource());
        break;
      case SpellTargetBehavior.CHOSEN_TARGET_ONLY:
        this.AddChosenTargetAsVisualTarget();
        break;
      case SpellTargetBehavior.BOARD_RANDOM:
        this.GenerateRandomBoardVisualTargets();
        break;
      case SpellTargetBehavior.TARGET_ZONE_CENTER:
        this.AddVisualTarget(SpellUtils.FindTargetZone((Spell) this).gameObject);
        break;
      case SpellTargetBehavior.NEW_CREATED_CARDS:
        this.GenerateCreatedCardsTargets();
        break;
      case SpellTargetBehavior.NEW_CREATED_CARDS_IN_PLAY:
        this.GenerateCreatedCardsTargets(TAG_ZONE.PLAY);
        break;
      default:
        this.AddAllTargetsAsVisualTargets();
        if (this.GetVisualTargets().Count != 1 || this.m_MissileInfo.m_TimesToHitSameTarget <= 1)
          break;
        this.AddSameTargetForAdditionalMissiles();
        break;
    }
  }

  protected void GenerateRandomBoardVisualTargets()
  {
    ZonePlay friendlyPlayZone = SpellUtils.FindFriendlyPlayZone((Spell) this);
    ZonePlay opponentPlayZone = SpellUtils.FindOpponentPlayZone((Spell) this);
    Bounds bounds1 = friendlyPlayZone.GetComponent<Collider>().bounds;
    Bounds bounds2 = opponentPlayZone.GetComponent<Collider>().bounds;
    Vector3 vector3_1 = Vector3.Min(bounds1.min, bounds2.min);
    Vector3 vector3_2 = Vector3.Max(bounds1.max, bounds2.max);
    Vector3 center = 0.5f * (vector3_2 + vector3_1);
    Vector3 vector3_3 = vector3_2 - vector3_1;
    Vector3 size = new Vector3(Mathf.Abs(vector3_3.x), Mathf.Abs(vector3_3.y), Mathf.Abs(vector3_3.z));
    this.GenerateRandomVisualTargets(new Bounds(center, size));
  }

  protected void GenerateRandomPlayZoneVisualTargets(ZonePlay zonePlay) => this.GenerateRandomVisualTargets(zonePlay.GetComponent<Collider>().bounds);

  private void GenerateRandomVisualTargets(Bounds bounds)
  {
    int length = UnityEngine.Random.Range(this.m_TargetInfo.m_RandomTargetCountMin, this.m_TargetInfo.m_RandomTargetCountMax + 1);
    if (length == 0)
      return;
    float x = bounds.min.x;
    float z1 = bounds.max.z;
    float z2 = bounds.min.z;
    float num1 = bounds.size.x / (float) length;
    int[] boxUsageCounts = new int[length];
    int[] numArray = new int[length];
    for (int index = 0; index < length; ++index)
    {
      boxUsageCounts[index] = 0;
      numArray[index] = -1;
    }
    for (int index1 = 0; index1 < length; ++index1)
    {
      float num2 = UnityEngine.Random.Range(0.0f, 1f);
      int max1 = 0;
      for (int index2 = 0; index2 < length; ++index2)
      {
        if ((double) this.ComputeBoxPickChance(boxUsageCounts, index2) >= (double) num2)
          numArray[max1++] = index2;
      }
      int boxIndex = numArray[UnityEngine.Random.Range(0, max1)];
      ++boxUsageCounts[boxIndex];
      float min = x + (float) boxIndex * num1;
      float max2 = min + num1;
      this.GenerateVisualTarget(new Vector3()
      {
        x = UnityEngine.Random.Range(min, max2),
        y = bounds.center.y,
        z = UnityEngine.Random.Range(z2, z1)
      }, index1, boxIndex);
    }
  }

  private void GenerateVisualTarget(Vector3 position, int index, int boxIndex)
  {
    GameObject go = new GameObject();
    go.name = string.Format("{0} Target {1} (box {2})", (object) this, (object) index, (object) boxIndex);
    go.transform.position = position;
    go.AddComponent<SpellGeneratedTarget>();
    this.AddVisualTarget(go);
  }

  private float ComputeBoxPickChance(int[] boxUsageCounts, int index) => 1f - (float) boxUsageCounts[index] / ((float) boxUsageCounts.Length * 0.25f);

  private void GenerateCreatedCardsTargets(TAG_ZONE onlyAffectZone = TAG_ZONE.INVALID)
  {
    if (this.m_taskList == null)
      return;
    if (this.m_taskList.IsStartOfBlock())
      this.m_createCardsInSetAside.Clear();
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      switch (power.Type)
      {
        case Network.PowerType.FULL_ENTITY:
          int id1 = ((Network.HistFullEntity) power).Entity.ID;
          Entity entity1 = GameState.Get().GetEntity(id1);
          if (entity1 == null)
          {
            Debug.LogWarning((object) string.Format("{0}.GenerateCreatedCardsTargets() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) id1));
            continue;
          }
          TAG_ZONE tag1 = entity1.GetTag<TAG_ZONE>(GAME_TAG.ZONE);
          if (onlyAffectZone == TAG_ZONE.INVALID || tag1 == onlyAffectZone)
          {
            switch (tag1)
            {
              case TAG_ZONE.SETASIDE:
                this.m_createCardsInSetAside.Add(id1);
                continue;
              case TAG_ZONE.LETTUCE_ABILITY:
                continue;
              default:
                Card card1 = entity1.GetCard();
                if ((UnityEngine.Object) card1 == (UnityEngine.Object) null)
                {
                  Debug.LogWarning((object) string.Format("{0}.GenerateCreatedCardsTargets() - WARNING trying to target entity.GetCard() with id {1} but there is no card with that id", (object) this, (object) id1));
                  continue;
                }
                this.m_visualTargets.Add(card1.gameObject);
                continue;
            }
          }
          else
            continue;
        case Network.PowerType.SHOW_ENTITY:
          Network.HistShowEntity histShowEntity = (Network.HistShowEntity) power;
          int id2 = histShowEntity.Entity.ID;
          if (this.m_createCardsInSetAside.Contains(id2))
          {
            TAG_ZONE tagZone = TAG_ZONE.INVALID;
            foreach (Network.Entity.Tag tag2 in histShowEntity.Entity.Tags)
            {
              if (tag2.Name == 49)
                tagZone = (TAG_ZONE) tag2.Value;
            }
            if (tagZone != TAG_ZONE.INVALID && tagZone != TAG_ZONE.LETTUCE_ABILITY && tagZone != TAG_ZONE.SETASIDE && (onlyAffectZone == TAG_ZONE.INVALID || tagZone == onlyAffectZone))
            {
              Card card2 = GameState.Get().GetEntity(id2)?.GetCard();
              if ((UnityEngine.Object) card2 == (UnityEngine.Object) null)
              {
                Debug.LogWarning((object) string.Format("{0}.GenerateCreatedCardsTargets() - WARNING trying to target entity.GetCard() with id {1} but there is no card with that id", (object) this, (object) id2));
                continue;
              }
              this.m_createCardsInSetAside.Remove(id2);
              this.m_visualTargets.Add(card2.gameObject);
              continue;
            }
            continue;
          }
          continue;
        case Network.PowerType.TAG_CHANGE:
          Network.HistTagChange histTagChange = (Network.HistTagChange) power;
          if (histTagChange.Tag == 49)
          {
            TAG_ZONE tagZone = (TAG_ZONE) histTagChange.Value;
            switch (tagZone)
            {
              case TAG_ZONE.SETASIDE:
              case TAG_ZONE.LETTUCE_ABILITY:
                continue;
              default:
                if (onlyAffectZone == TAG_ZONE.INVALID || tagZone == onlyAffectZone)
                {
                  int entity2 = histTagChange.Entity;
                  if (this.m_createCardsInSetAside.Contains(entity2))
                  {
                    Card card3 = GameState.Get().GetEntity(entity2)?.GetCard();
                    if ((UnityEngine.Object) card3 == (UnityEngine.Object) null)
                    {
                      Debug.LogWarning((object) string.Format("{0}.GenerateCreatedCardsTargets() - WARNING trying to target entity.GetCard() with id {1} but there is no card with that id", (object) this, (object) entity2));
                      continue;
                    }
                    this.m_createCardsInSetAside.Remove(entity2);
                    this.m_visualTargets.Add(card3.gameObject);
                    continue;
                  }
                  continue;
                }
                continue;
            }
          }
          else
            continue;
        default:
          continue;
      }
    }
  }

  private void AddChosenTargetAsVisualTarget()
  {
    Card powerTargetCard = this.GetPowerTargetCard();
    if ((UnityEngine.Object) powerTargetCard == (UnityEngine.Object) null)
      Debug.LogWarning((object) string.Format("{0}.AddChosenTargetAsVisualTarget() - there is no chosen target", (object) this));
    else
      this.AddVisualTarget(powerTargetCard.gameObject);
  }

  private void AddAllTargetsAsVisualTargets()
  {
    for (int index = 0; index < this.m_targets.Count; ++index)
    {
      this.m_visualToTargetIndexMap[this.m_visualTargets.Count] = index;
      this.AddVisualTarget(this.m_targets[index]);
    }
  }

  private void AddSameTargetForAdditionalMissiles()
  {
    for (int index = 1; index < this.m_MissileInfo.m_TimesToHitSameTarget; ++index)
    {
      this.m_visualToTargetIndexMap[this.GetVisualTargets().Count] = index;
      this.AddVisualTarget(this.GetVisualTargets()[0]);
    }
  }

  private void SuppressPlaySoundsOnVisualTargets()
  {
    if (!this.m_TargetInfo.m_SuppressPlaySounds)
      return;
    for (int index = 0; index < this.m_visualTargets.Count; ++index)
    {
      Card component = this.m_visualTargets[index].GetComponent<Card>();
      if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
      {
        if (component.GetEntity().GetTag(GAME_TAG.DONT_SUPPRESS_SUMMON_VO) == 1)
          break;
        component.SuppressPlaySounds(true);
      }
    }
  }

  protected virtual void CleanUp()
  {
    foreach (GameObject visualTarget in this.m_visualTargets)
    {
      if ((UnityEngine.Object) visualTarget == (UnityEngine.Object) null)
        Debug.LogWarning((object) string.Format("{0}.CleanUp() - found a null GameObject in m_visualTargets", (object) this));
      else if (!((UnityEngine.Object) visualTarget.GetComponent<SpellGeneratedTarget>() == (UnityEngine.Object) null))
        UnityEngine.Object.Destroy((UnityEngine.Object) visualTarget);
    }
    this.m_visualTargets.Clear();
  }

  protected bool HasMetaDataTargets() => this.m_targetToMetaDataMap.Count > 0;

  protected int GetMetaDataIndexForTarget(int visualTargetIndex)
  {
    int key;
    int num;
    return !this.m_visualToTargetIndexMap.TryGetValue(visualTargetIndex, out key) || !this.m_targetToMetaDataMap.TryGetValue(key, out num) ? -1 : num;
  }

  protected bool ShouldCompleteTasksUntilMetaData(int metaDataIndex) => this.m_taskList != null && !this.IsBatchedTargetInfo(metaDataIndex) && this.m_taskList.HasEarlierIncompleteTask(metaDataIndex);

  private bool IsBatchedTargetInfo(int metaDataIndex) => this.m_taskList.GetTaskList().Count < metaDataIndex && this.m_taskList.GetTaskList()[metaDataIndex].GetPower() is Network.HistMetaData power && power.MetaType == HistoryMeta.Type.TARGET && power.Data != 0;

  protected IEnumerator CompleteTasksUntilMetaData(int metaDataIndex)
  {
    SuperSpell superSpell = this;
    ++superSpell.m_effectsPendingFinish;
    superSpell.m_taskList.DoTasks(0, metaDataIndex);
    QueueList<PowerTask> tasksToWaitFor = superSpell.DetermineTasksToWaitFor(0, metaDataIndex);
    if (tasksToWaitFor != null && tasksToWaitFor.Count > 0)
      yield return (object) superSpell.StartCoroutine(superSpell.WaitForTasks(tasksToWaitFor));
    --superSpell.m_effectsPendingFinish;
  }

  protected QueueList<PowerTask> DetermineTasksToWaitFor(
    int startIndex,
    int count)
  {
    if (count == 0)
      return (QueueList<PowerTask>) null;
    int num = startIndex + count;
    QueueList<PowerTask> tasksToWaitFor = new QueueList<PowerTask>();
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index1 = startIndex; index1 < num; ++index1)
    {
      PowerTask task = taskList[index1];
      Entity entity = this.GetEntityFromZoneChangePowerTask(task);
      if (entity != null && !((UnityEngine.Object) this.m_visualTargets.Find((Predicate<GameObject>) (currTargetObject =>
      {
        Card component = currTargetObject.GetComponent<Card>();
        return (UnityEngine.Object) entity.GetCard() == (UnityEngine.Object) component;
      })) == (UnityEngine.Object) null))
      {
        for (int index2 = 0; index2 < tasksToWaitFor.Count; ++index2)
        {
          Entity zoneChangePowerTask = this.GetEntityFromZoneChangePowerTask(tasksToWaitFor[index2]);
          if (entity == zoneChangePowerTask)
          {
            tasksToWaitFor.RemoveAt(index2);
            break;
          }
        }
        tasksToWaitFor.Enqueue(task);
      }
    }
    return tasksToWaitFor;
  }

  protected IEnumerator WaitForTasks(QueueList<PowerTask> tasksToWaitFor)
  {
    while (tasksToWaitFor.Count > 0)
    {
      PowerTask task = tasksToWaitFor.Peek();
      if (!task.IsCompleted())
      {
        yield return (object) null;
      }
      else
      {
        Entity entity;
        int zoneTag;
        this.GetZoneChangeFromPowerTask(task, out entity, out zoneTag);
        Card card = entity.GetCard();
        Zone zone = ZoneMgr.Get().FindZoneForEntityAndZoneTag(entity, (TAG_ZONE) zoneTag);
        while ((UnityEngine.Object) card.GetZone() != (UnityEngine.Object) zone)
          yield return (object) null;
        while (card.IsActorLoading())
          yield return (object) null;
        tasksToWaitFor.Dequeue();
        card = (Card) null;
        zone = (Zone) null;
      }
    }
  }

  protected IEnumerator CompleteTasksFromMetaData(int metaDataIndex, float delaySec)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    SuperSpell superSpell = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      superSpell.CompleteMetaDataTasks(metaDataIndex, new PowerTaskList.CompleteCallback(superSpell.OnMetaDataTasksComplete));
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    ++superSpell.m_effectsPendingFinish;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(delaySec);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected void OnMetaDataTasksComplete(
    PowerTaskList taskList,
    int startIndex,
    int count,
    object userData)
  {
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }

  protected bool IsMakingClones() => true;

  protected bool AreEffectsActive() => this.m_effectsPendingFinish > 0;

  protected Spell CloneSpell(
    Spell prefab,
    Vector3? position = null,
    Spell.FinishedCallback finishedCallback = null)
  {
    Spell spell;
    if (this.IsMakingClones())
    {
      if (position.HasValue)
      {
        spell = SpellManager.Get().GetSpell(prefab);
        spell.transform.position = position.Value;
      }
      else
        spell = SpellManager.Get().GetSpell(prefab);
      spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnCloneSpellStateStarted));
      spell.transform.parent = this.transform;
      this.m_activeClonedSpells.Add(spell);
    }
    else
    {
      spell = prefab;
      spell.RemoveAllTargets();
    }
    Spell.FinishedCallback callback = finishedCallback == null ? new Spell.FinishedCallback(this.OnCloneSpellFinished) : finishedCallback;
    spell.AddFinishedCallback(callback);
    return spell;
  }

  private void OnCloneSpellFinished(Spell spell, object userData)
  {
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }

  private void OnCloneSpellStateStarted(Spell spell, SpellStateType prevStateType, object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    this.m_activeClonedSpells.Remove(spell);
    SpellManager.Get().ReleaseSpell(spell);
  }

  private void UpdatePendingStateChangeFlags(SpellStateType stateType)
  {
    if (!this.HasStateContent(stateType))
    {
      this.m_pendingNoneStateChange = true;
      this.m_pendingSpellFinish = true;
    }
    else
    {
      this.m_pendingNoneStateChange = false;
      this.m_pendingSpellFinish = false;
    }
  }

  protected void FinishIfPossible()
  {
    if (this.m_settingUpAction || this.AreEffectsActive())
      return;
    if (this.m_pendingSpellFinish)
    {
      this.OnSpellFinished();
      this.m_pendingSpellFinish = false;
    }
    if (this.m_pendingNoneStateChange)
    {
      this.OnStateFinished();
      this.m_pendingNoneStateChange = false;
    }
    this.CleanUp();
  }
}
