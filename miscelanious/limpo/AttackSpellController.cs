using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpellController : SpellController
{
  public HeroAttackDef m_HeroInfo;
  public AllyAttackDef m_AllyInfo;
  public float m_ImpactStagingPoint = 1f;
  public float m_SourceImpactOffset = -0.25f;
  public SpellHandleValueRange[] m_ImpactDefHandles;
  public SpellHandleValueRange[] m_CriticalImpactDefHandles;
  public string m_DefaultImpactSpellPrefabHandle;
  private const float PROPOSED_ATTACK_IMPACT_POINT_SCALAR = 0.5f;
  private const float WINDFURY_REMINDER_WAIT_SEC = 1.2f;
  private const int FINISHER_DAMAGE_THRESHOLD = 15;
  private AttackType m_attackType;
  private Spell m_sourceAttackSpell;
  private Coroutine m_finisherTrackingCoroutine;
  private readonly WaitForSeconds MAX_FINISHER_DURATION = new WaitForSeconds(15f);
  private Vector3 m_sourcePos;
  private Vector3 m_sourceToTarget;
  private Vector3 m_sourceFacing;
  private bool m_repeatProposed;

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    this.m_attackType = taskList.GetAttackType();
    this.m_repeatProposed = taskList.IsRepeatProposedAttack();
    if (this.m_attackType == AttackType.INVALID)
      return false;
    Entity attacker = taskList.GetAttacker();
    if (attacker != null)
      this.SetSource(attacker.GetCard());
    Entity defender = taskList.GetDefender();
    if (defender != null)
      this.AddTarget(defender.GetCard());
    return true;
  }

  protected override void OnProcessTaskList()
  {
    if (this.m_attackType == AttackType.ONLY_PROPOSED_ATTACKER || this.m_attackType == AttackType.ONLY_PROPOSED_DEFENDER || this.m_attackType == AttackType.ONLY_ATTACKER || this.m_attackType == AttackType.ONLY_DEFENDER || this.m_attackType == AttackType.WAITING_ON_PROPOSED_ATTACKER || this.m_attackType == AttackType.WAITING_ON_PROPOSED_DEFENDER || this.m_attackType == AttackType.WAITING_ON_ATTACKER || this.m_attackType == AttackType.WAITING_ON_DEFENDER)
      this.FinishEverything();
    else if (this.m_repeatProposed)
    {
      this.FinishEverything();
    }
    else
    {
      Card source = this.GetSource();
      if ((UnityEngine.Object) source == (UnityEngine.Object) null || (UnityEngine.Object) source.GetActor() == (UnityEngine.Object) null)
      {
        this.FinishEverything();
      }
      else
      {
        Entity entity1 = source.GetEntity();
        if (entity1 == null)
        {
          this.FinishEverything();
        }
        else
        {
          Zone zone = source.GetZone();
          if ((UnityEngine.Object) zone == (UnityEngine.Object) null)
            zone = (Zone) ZoneMgr.Get().FindZoneOfType<ZonePlay>(source.GetControllerSide());
          if ((GameMgr.Get().IsBattlegrounds() || GameMgr.Get().IsBattlegroundsTutorial() || GameMgr.Get().IsFriendlyBattlegrounds()) && entity1.IsHero())
          {
            FinisherGameplaySettings gameplaySettings = FinisherGameplaySettings.GetFinisherGameplaySettings(entity1);
            Card target = this.GetTarget();
            Entity entity2 = target.GetEntity();
            bool opponentFinisher = entity1.IsControlledByOpposingSidePlayer();
            string spellPrefabName = this.GetSpellPath(entity1, entity2, opponentFinisher, gameplaySettings);
            if (string.IsNullOrEmpty(spellPrefabName))
            {
              AssetReference fromAssetString = AssetReference.CreateFromAssetString(GameDbf.BattlegroundsFinisher.GetRecord(1).GameplaySettings);
              gameplaySettings = AssetLoader.Get().LoadAsset<FinisherGameplaySettings>(fromAssetString).Asset;
              int tag = entity1.GetTag(GAME_TAG.BATTLEGROUNDS_FAVORITE_FINISHER);
              if (opponentFinisher)
              {
                Log.Spells.PrintError(string.Format("Finisher ID {0} is missing a small opponent finisher prefab entry in its gameplay settings. Using default finisher.", (object) tag));
                spellPrefabName = gameplaySettings.SmallOpponentPrefab;
              }
              else
              {
                Log.Spells.PrintError(string.Format("Finisher ID {0} is missing a small finisher prefab entry in its gameplay settings. Using default finisher.", (object) tag));
                spellPrefabName = gameplaySettings.SmallPrefab;
              }
              if (string.IsNullOrEmpty(spellPrefabName))
              {
                string str = opponentFinisher ? "Small Opponent" : "Small";
                Error.AddDevWarning("Missing Default Finisher Path", "Unable to get spellpath for the " + str + " Default Finisher. Make sure " + fromAssetString.FileName + " contains a prefab for " + str + " Prefab.");
              }
            }
            this.m_sourceAttackSpell = this.InstantiateFinisherSpell(source.gameObject, spellPrefabName);
            if ((UnityEngine.Object) this.m_sourceAttackSpell != (UnityEngine.Object) null)
            {
              this.m_sourceAttackSpell.SetSource(source.gameObject);
              this.m_sourceAttackSpell.AddTarget(target.gameObject);
              this.m_sourceAttackSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnBattlegroundsFinisherFinished), (object) gameplaySettings);
              this.m_finisherTrackingCoroutine = this.StartCoroutine(this.EnsureFinisherCompletes(this.m_sourceAttackSpell));
              SuperSpell sourceAttackSpell = this.m_sourceAttackSpell as SuperSpell;
              if ((UnityEngine.Object) sourceAttackSpell != (UnityEngine.Object) null)
              {
                sourceAttackSpell.ActivateFinisher(opponentFinisher);
                return;
              }
              this.m_sourceAttackSpell.Activate();
              return;
            }
            Log.Spells.PrintError(string.Format("Finisher ID {0} failed to instantiate finisher spell at {1}. No spell will be played.", (object) entity1.GetTag(GAME_TAG.BATTLEGROUNDS_FAVORITE_FINISHER), (object) spellPrefabName));
          }
          bool isSourceFriendly = zone.m_Side == Player.Side.FRIENDLY;
          this.m_sourceAttackSpell = this.GetSourceAttackSpell(source, isSourceFriendly);
          if (this.m_attackType == AttackType.CANCELED)
          {
            this.CancelAttackSpell(entity1, this.m_sourceAttackSpell);
            source.SetDoNotSort(false);
            zone.UpdateLayout();
            source.EnableAttacking(false);
            this.FinishEverything();
          }
          else if ((UnityEngine.Object) this.m_sourceAttackSpell == (UnityEngine.Object) null)
          {
            this.FinishEverything();
          }
          else
          {
            source.EnableAttacking(true);
            if (entity1.GetTag(GAME_TAG.IMMUNE_WHILE_ATTACKING) != 0)
              source.ActivateActorSpell(SpellType.IMMUNE);
            else if (!source.ShouldShowImmuneVisuals())
              SpellUtils.ActivateDeathIfNecessary(source.GetActor().GetSpellIfLoaded(SpellType.IMMUNE));
            this.m_sourceAttackSpell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSourceAttackStateStarted));
            if (GameState.Get().GetBooleanGameOption(GameEntityOption.USE_FASTER_ATTACK_SPELL_BIRTH_STATE))
            {
              List<SpellState> spellStates = this.m_sourceAttackSpell.GetSpellStates(SpellStateType.BIRTH);
              if (spellStates != null)
              {
                foreach (SpellState spellState in spellStates)
                {
                  if (spellState.m_ExternalAnimatedObjects != null)
                  {
                    foreach (SpellStateAnimObject externalAnimatedObject in spellState.m_ExternalAnimatedObjects)
                      externalAnimatedObject.m_AnimSpeed = 2f;
                  }
                }
              }
            }
            if (isSourceFriendly)
            {
              if (this.m_sourceAttackSpell.GetActiveState() != SpellStateType.IDLE && this.m_sourceAttackSpell.GetActiveState() != SpellStateType.ACTION)
                this.m_sourceAttackSpell.ActivateState(SpellStateType.BIRTH);
              else
                this.m_sourceAttackSpell.ActivateState(SpellStateType.ACTION);
            }
            else if (this.m_sourceAttackSpell.GetActiveState() != SpellStateType.IDLE && this.m_sourceAttackSpell.GetActiveState() != SpellStateType.ACTION)
              this.m_sourceAttackSpell.ActivateState(SpellStateType.BIRTH);
            else
              this.m_sourceAttackSpell.ActivateState(SpellStateType.ACTION);
          }
        }
      }
    }
  }

  private void OnSourceAttackStateStarted(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    switch (spell.GetActiveState())
    {
      case SpellStateType.IDLE:
        spell.ActivateState(SpellStateType.ACTION);
        break;
      case SpellStateType.ACTION:
        spell.RemoveStateStartedCallback(new Spell.StateStartedCallback(this.OnSourceAttackStateStarted));
        this.LaunchAttack();
        break;
    }
  }

  private void LaunchAttack()
  {
    Card source = this.GetSource();
    Entity entity = source.GetEntity();
    Card target = this.GetTarget();
    bool flag = this.m_attackType == AttackType.PROPOSED;
    if (flag && entity.IsHero())
    {
      this.m_sourceAttackSpell.ActivateState(SpellStateType.IDLE);
      this.FinishEverything();
    }
    else
    {
      this.m_sourcePos = source.transform.position;
      this.m_sourceToTarget = target.transform.position - this.m_sourcePos;
      Vector3 impactPos = this.ComputeImpactPos();
      source.SetDoNotSort(!flag);
      this.MoveSourceToTarget(source, entity, impactPos);
      if (entity.IsHero())
        this.OrientSourceHeroToTarget(source);
      if (flag)
        return;
      target.SetDoNotSort(true);
      this.MoveTargetToSource(target, entity, impactPos);
    }
  }

  private bool HasFinishAttackSpellOnDamage()
  {
    Card source = this.GetSource();
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return false;
    Entity entity1 = source.GetEntity();
    if (entity1 == null)
      return false;
    if (!entity1.IsHero())
      return entity1.HasTag(GAME_TAG.FINISH_ATTACK_SPELL_ON_DAMAGE);
    Player controller = entity1.GetController();
    if (controller == null)
      return false;
    Card weaponCard = controller.GetWeaponCard();
    if ((UnityEngine.Object) weaponCard == (UnityEngine.Object) null)
      return false;
    Entity entity2 = weaponCard.GetEntity();
    return entity2 != null && entity2.HasTag(GAME_TAG.FINISH_ATTACK_SPELL_ON_DAMAGE);
  }

  private void UpdateTargetOnMoveToTargetFinished(Card targetCard)
  {
    targetCard.SetDoNotSort(false);
    Zone zone = targetCard.GetZone();
    if ((UnityEngine.Object) zone == (UnityEngine.Object) null)
    {
      zone = targetCard.GetPrevZone();
      if (!targetCard.GetEntity().IsHero())
        Log.Spells.PrintWarning("AttackSpellController.UpdateTargetOnMoveToTargetFinished() - Non-hero target ({0}) was moved from {1} to SETASIDE before the attack was resolved.", (object) targetCard.name, (object) zone.name);
    }
    zone.UpdateLayout();
  }

  private void OnMoveToTargetFinished()
  {
    Card source = this.GetSource();
    Entity entity = source.GetEntity();
    Card target = this.GetTarget();
    bool flag = this.m_attackType == AttackType.PROPOSED;
    this.DoTasks(source, target);
    if (!flag)
      this.ActivateImpactEffects(source, target);
    if (entity.IsHero())
    {
      this.MoveSourceHeroBack(source);
      this.OrientSourceHeroBack(source);
      this.UpdateTargetOnMoveToTargetFinished(target);
      if (!this.HasFinishAttackSpellOnDamage())
        return;
      this.FinishHeroAttack();
    }
    else if (flag)
    {
      this.FinishEverything();
    }
    else
    {
      source.SetDoNotSort(false);
      source.GetZone().UpdateLayout();
      this.UpdateTargetOnMoveToTargetFinished(target);
      if (this.HasFinishAttackSpellOnDamage())
        this.FinishAttackSpellController();
      else
        this.m_sourceAttackSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnMinionSourceAttackStateFinished));
      this.m_sourceAttackSpell.ActivateState(SpellStateType.DEATH);
    }
  }

  private void DoTasks(Card sourceCard, Card targetCard) => GameUtils.DoDamageTasks(this.m_taskList, sourceCard, targetCard);

  private void MoveSourceHeroBack(Card sourceCard)
  {
    Hashtable args = iTween.Hash((object) "position", (object) this.m_sourcePos, (object) "time", (object) this.m_HeroInfo.m_MoveBackDuration, (object) "easetype", (object) this.m_HeroInfo.m_MoveBackEaseType, (object) "oncomplete", (object) "OnHeroMoveBackFinished", (object) "oncompletetarget", (object) this.gameObject);
    iTween.MoveTo(sourceCard.gameObject, args);
  }

  private void OrientSourceHeroBack(Card sourceCard)
  {
    Hashtable args = iTween.Hash((object) "rotation", (object) Quaternion.LookRotation(this.m_sourceFacing).eulerAngles, (object) "time", (object) this.m_HeroInfo.m_OrientBackDuration, (object) "easetype", (object) this.m_HeroInfo.m_OrientBackEaseType);
    iTween.RotateTo(sourceCard.gameObject, args);
  }

  private void OnHeroMoveBackFinished()
  {
    Card source = this.GetSource();
    Entity entity = source.GetEntity();
    source.SetDoNotSort(false);
    source.EnableAttacking(false);
    if (this.HasFinishAttackSpellOnDamage())
      return;
    if (entity.GetController().IsLocalUser() || this.m_sourceAttackSpell.GetActiveState() == SpellStateType.NONE)
      this.FinishHeroAttack();
    else
      this.m_sourceAttackSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnHeroSourceAttackStateFinished));
  }

  private void OnHeroSourceAttackStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnHeroSourceAttackStateFinished));
    this.FinishHeroAttack();
  }

  private void FinishHeroAttack()
  {
    Card source = this.GetSource();
    this.PlayWindfuryReminderIfPossible(source.GetEntity(), source);
    this.FinishEverything();
  }

  private IEnumerator EnsureFinisherCompletes(Spell spell)
  {
    yield return (object) this.MAX_FINISHER_DURATION;
    this.m_finisherTrackingCoroutine = (Coroutine) null;
    Log.Spells.PrintError("Finisher spell " + spell.gameObject.name + " did not terminate and was killed to prevent game hang. Run the finisher in the authoring scene to diagnose potential problems.");
    spell.ReleaseSpell();
  }

  private void OnBattlegroundsFinisherFinished(Spell spell, object favoriteFinisherRecordObject)
  {
    if (this.m_finisherTrackingCoroutine != null)
    {
      this.StopCoroutine(this.m_finisherTrackingCoroutine);
      this.m_finisherTrackingCoroutine = (Coroutine) null;
    }
    Card source = this.GetSource();
    Card target = this.GetTarget();
    int num = ((FinisherGameplaySettings) favoriteFinisherRecordObject).ShowImpactEffects ? 1 : 0;
    this.ActivateSpellDeathState();
    this.OnFinishedTaskList();
    if (num != 0)
      this.ActivateImpactEffects(source, target);
    spell.RemoveAllTargets();
    BaconBoard baconBoard = BaconBoard.Get();
    if ((UnityEngine.Object) baconBoard != (UnityEngine.Object) null)
      baconBoard.FriendlyPlayerFinisherCalled();
    this.StartCoroutine(this.WaitThenDestroySpellAndFinish());
  }

  private void ActivateSpellDeathState(Card source = null)
  {
    Card card = source;
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      card = this.GetSource();
    if (!((UnityEngine.Object) card != (UnityEngine.Object) null) || card.ShouldShowImmuneVisuals() || card.GetEntity() != null && card.GetEntity().HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING) && this.m_attackType == AttackType.PROPOSED)
      return;
    card.GetActor().ActivateSpellDeathState(SpellType.IMMUNE);
  }

  private IEnumerator WaitThenDestroySpellAndFinish()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AttackSpellController attackSpellController = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      SpellUtils.PurgeSpell(attackSpellController.m_sourceAttackSpell);
      attackSpellController.OnFinished();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(10f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void OnMinionSourceAttackStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnMinionSourceAttackStateFinished));
    this.FinishAttackSpellController();
  }

  private void FinishAttackSpellController()
  {
    Card source = this.GetSource();
    Entity entity = source.GetEntity();
    source.EnableAttacking(false);
    if (!this.CanPlayWindfuryReminder(entity, source))
    {
      this.FinishEverything();
    }
    else
    {
      this.OnFinishedTaskList();
      this.ActivateSpellDeathState(source);
      this.StartCoroutine(this.WaitThenPlayWindfuryReminder(entity, source));
    }
  }

  private void FinishEverything()
  {
    this.ActivateSpellDeathState();
    this.OnFinishedTaskList();
    this.OnFinished();
  }

  private IEnumerator WaitThenPlayWindfuryReminder(Entity entity, Card card)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AttackSpellController attackSpellController = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      attackSpellController.PlayWindfuryReminderIfPossible(entity, card);
      attackSpellController.OnFinished();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(1.2f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private bool CanPlayWindfuryReminder(Entity entity, Card card) => entity.HasWindfury() && !entity.IsExhausted() && entity.GetZone() == TAG_ZONE.PLAY && entity.GetController().IsCurrentPlayer() && !((UnityEngine.Object) card.GetActorSpell(SpellType.WINDFURY_BURST) == (UnityEngine.Object) null);

  private void PlayWindfuryReminderIfPossible(Entity entity, Card card)
  {
    if (!this.CanPlayWindfuryReminder(entity, card))
      return;
    card.ActivateActorSpell(SpellType.WINDFURY_BURST);
  }

  private void MoveSourceToTarget(Card sourceCard, Entity sourceEntity, Vector3 impactPos)
  {
    Vector3 impactOffset = this.ComputeImpactOffset(sourceCard, impactPos);
    Vector3 vector3 = impactPos + impactOffset;
    float toTargetDuration;
    iTween.EaseType toTargetEaseType;
    if (sourceEntity.IsHero())
    {
      toTargetDuration = this.m_HeroInfo.m_MoveToTargetDuration;
      toTargetEaseType = this.m_HeroInfo.m_MoveToTargetEaseType;
    }
    else
    {
      toTargetDuration = this.m_AllyInfo.m_MoveToTargetDuration;
      toTargetEaseType = this.m_AllyInfo.m_MoveToTargetEaseType;
    }
    Hashtable args = iTween.Hash((object) "position", (object) vector3, (object) "time", (object) toTargetDuration, (object) "easetype", (object) toTargetEaseType, (object) "oncomplete", (object) "OnMoveToTargetFinished", (object) "oncompletetarget", (object) this.gameObject);
    iTween.MoveTo(sourceCard.gameObject, args);
  }

  private void OrientSourceHeroToTarget(Card sourceCard)
  {
    this.m_sourceFacing = sourceCard.transform.forward;
    Vector3 vector3 = sourceCard.transform.InverseTransformDirection(this.m_sourceToTarget);
    if ((double) Vector3.Dot(this.m_sourceFacing, vector3) < 0.0)
      vector3 = -vector3;
    Hashtable args = iTween.Hash((object) "rotation", (object) Quaternion.LookRotation(vector3).eulerAngles, (object) "time", (object) this.m_HeroInfo.m_OrientToTargetDuration, (object) "easetype", (object) this.m_HeroInfo.m_OrientToTargetEaseType);
    iTween.RotateTo(sourceCard.gameObject, args);
  }

  private void MoveTargetToSource(Card targetCard, Entity sourceEntity, Vector3 impactPos)
  {
    float toTargetDuration;
    iTween.EaseType toTargetEaseType;
    if (sourceEntity.IsHero())
    {
      toTargetDuration = this.m_HeroInfo.m_MoveToTargetDuration;
      toTargetEaseType = this.m_HeroInfo.m_MoveToTargetEaseType;
    }
    else
    {
      toTargetDuration = this.m_AllyInfo.m_MoveToTargetDuration;
      toTargetEaseType = this.m_AllyInfo.m_MoveToTargetEaseType;
    }
    Hashtable args = iTween.Hash((object) "position", (object) impactPos, (object) "time", (object) toTargetDuration, (object) "easetype", (object) toTargetEaseType);
    iTween.MoveTo(targetCard.gameObject, args);
  }

  private Vector3 ComputeImpactPos()
  {
    float num = 1f;
    if (this.m_attackType == AttackType.PROPOSED)
      num = 0.5f;
    return this.m_sourcePos + num * this.m_ImpactStagingPoint * this.m_sourceToTarget;
  }

  private Vector3 ComputeImpactOffset(Card sourceCard, Vector3 impactPos)
  {
    if (Mathf.Approximately(this.m_SourceImpactOffset, 0.5f) || (UnityEngine.Object) sourceCard.GetActor().GetMeshRenderer() == (UnityEngine.Object) null)
      return Vector3.zero;
    Bounds bounds = sourceCard.GetActor().GetMeshRenderer().bounds with
    {
      center = this.m_sourcePos
    };
    Ray ray = new Ray(impactPos, bounds.center - impactPos);
    float distance;
    if (!bounds.IntersectRay(ray, out distance))
      return Vector3.zero;
    Vector3 vector3_1 = ray.origin + distance * ray.direction;
    Vector3 vector3_2 = 2f * bounds.center - vector3_1 - vector3_1;
    return 0.5f * vector3_2 - this.m_SourceImpactOffset * vector3_2;
  }

  private void ActivateImpactEffects(Card sourceCard, Card targetCard)
  {
    string impactSpellPrefab = this.DetermineImpactSpellPrefab(sourceCard, targetCard);
    if (string.IsNullOrEmpty(impactSpellPrefab))
      return;
    Spell spell = SpellManager.Get().GetSpell(impactSpellPrefab);
    spell.transform.parent = (Transform) null;
    spell.SetSource(sourceCard.gameObject);
    spell.AddTarget(targetCard.gameObject);
    spell.SetPosition(targetCard.transform.position);
    spell.SetOrientation(Quaternion.LookRotation(this.m_sourceToTarget));
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnImpactSpellStateFinished));
    spell.Activate();
    BaconBoard baconBoard = BaconBoard.Get();
    if (!((UnityEngine.Object) baconBoard != (UnityEngine.Object) null))
      return;
    baconBoard.CheckForHeroHeavyHitBoardEffects(sourceCard, targetCard);
  }

  private string DetermineImpactSpellPrefab(Card sourceCard, Card targetCard)
  {
    int atk = sourceCard.GetEntity().GetATK();
    SpellHandleValueRange accordingToRanges = SpellUtils.GetAppropriateElementAccordingToRanges<SpellHandleValueRange>(!this.WasAttackCriticalHit(sourceCard, targetCard) || this.m_CriticalImpactDefHandles == null || this.m_CriticalImpactDefHandles.Length == 0 ? this.m_ImpactDefHandles : this.m_CriticalImpactDefHandles, (Func<SpellHandleValueRange, ValueRange>) (x => x.m_range), atk);
    return accordingToRanges != null && !string.IsNullOrEmpty(accordingToRanges.m_spellPrefabName) ? accordingToRanges.m_spellPrefabName : this.m_DefaultImpactSpellPrefabHandle;
  }

  private bool WasAttackCriticalHit(Card sourceCard, Card targetCard)
  {
    bool flag = false;
    if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null || (UnityEngine.Object) targetCard == (UnityEngine.Object) null)
      return flag;
    Player.Side controllerSide1 = sourceCard.GetControllerSide();
    Player.Side controllerSide2 = targetCard.GetControllerSide();
    if (controllerSide1 != Player.Side.NEUTRAL && controllerSide2 != Player.Side.NEUTRAL)
      flag = controllerSide1 != controllerSide2 && sourceCard.GetEntity().IsLettuceMercenary() && sourceCard.GetEntity().IsMyLettuceRoleStrongAgainst(targetCard.GetEntity());
    return flag;
  }

  private void OnImpactSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    SpellManager.Get().ReleaseSpell(spell);
  }

  protected override float GetLostFrameTimeCatchUpSeconds()
  {
    Card source = this.GetSource();
    if ((UnityEngine.Object) source != (UnityEngine.Object) null && source.GetEntity() != null && source.GetEntity().IsHero())
      return 0.0f;
    Card target = this.GetTarget();
    return (UnityEngine.Object) target != (UnityEngine.Object) null && target.GetEntity() != null && target.GetEntity().IsHero() ? 0.0f : 0.8f;
  }

  protected override void OnFinishedTaskList()
  {
    if (this.m_attackType != AttackType.PROPOSED)
    {
      Card source = this.GetSource();
      source.SetDoNotSort(false);
      if (!source.GetEntity().IsHero())
      {
        Zone zone = source.GetZone();
        if ((UnityEngine.Object) zone != (UnityEngine.Object) null)
        {
          zone.UpdateLayout();
          if ((UnityEngine.Object) this.m_sourceAttackSpell == (UnityEngine.Object) null)
          {
            bool isSourceFriendly = zone.m_Side == Player.Side.FRIENDLY;
            this.m_sourceAttackSpell = this.GetSourceAttackSpell(source, isSourceFriendly);
          }
        }
        if ((UnityEngine.Object) this.m_sourceAttackSpell != (UnityEngine.Object) null && (this.m_sourceAttackSpell.GetActiveState() == SpellStateType.BIRTH || this.m_sourceAttackSpell.GetActiveState() == SpellStateType.IDLE || this.m_sourceAttackSpell.GetActiveState() == SpellStateType.ACTION))
          this.CancelAttackSpell(source.GetEntity(), this.m_sourceAttackSpell);
      }
    }
    base.OnFinishedTaskList();
  }

  private void CancelAttackSpell(Entity sourceEntity, Spell attackSpell)
  {
    if ((UnityEngine.Object) attackSpell == (UnityEngine.Object) null)
      return;
    if (sourceEntity == null)
      attackSpell.ActivateState(SpellStateType.DEATH);
    else if (sourceEntity.IsHero())
      attackSpell.ActivateState(SpellStateType.CANCEL);
    else
      attackSpell.ActivateState(SpellStateType.DEATH);
  }

  private Spell GetSourceAttackSpell(Card sourceCard, bool isSourceFriendly)
  {
    if (GameState.Get().GetGameEntity().HasTag(GAME_TAG.HIGHLIGHT_ATTACKING_MINION_DURING_COMBAT))
    {
      Spell actorSpell = sourceCard.GetActorSpell(SpellType.AUTO_ATTACK_WITH_HIGHLIGHT);
      return (UnityEngine.Object) actorSpell != (UnityEngine.Object) null ? actorSpell : (Spell) null;
    }
    return isSourceFriendly ? sourceCard.GetActorSpell(SpellType.FRIENDLY_ATTACK) : sourceCard.GetActorSpell(SpellType.OPPONENT_ATTACK);
  }

  private Spell InstantiateFinisherSpell(GameObject sourceObject, string spellPrefabName)
  {
    if (string.IsNullOrEmpty(spellPrefabName))
      return (Spell) null;
    Spell spell = SpellManager.Get().GetSpell(spellPrefabName);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return (Spell) null;
    TransformUtil.AttachAndPreserveLocalTransform(spell.transform, sourceObject.transform);
    return spell;
  }

  private string GetSpellPath(
    Entity sourceEntity,
    Entity targetEntity,
    bool opponentFinisher,
    FinisherGameplaySettings finisherSettings)
  {
    if (sourceEntity.GetATK() >= targetEntity.GetCurrentDefense())
    {
      if (GameState.Get().CountPlayersAlive() == 1)
      {
        if (opponentFinisher && !string.IsNullOrEmpty(finisherSettings.FirstPlaceVictoryOpponentPrefab))
          return finisherSettings.FirstPlaceVictoryOpponentPrefab;
        if (!string.IsNullOrEmpty(finisherSettings.FirstPlaceVictoryPrefab))
          return finisherSettings.FirstPlaceVictoryPrefab;
      }
      if (opponentFinisher && !string.IsNullOrEmpty(finisherSettings.LethalOpponentPrefab))
        return finisherSettings.LethalOpponentPrefab;
      if (!string.IsNullOrEmpty(finisherSettings.LethalPrefab))
        return finisherSettings.LethalPrefab;
    }
    return sourceEntity.GetATK() >= 15 ? (opponentFinisher ? (string.IsNullOrEmpty(finisherSettings.LargeOpponentPrefab) ? finisherSettings.SmallOpponentPrefab : finisherSettings.LargeOpponentPrefab) : (string.IsNullOrEmpty(finisherSettings.LargePrefab) ? finisherSettings.SmallPrefab : finisherSettings.LargePrefab)) : (opponentFinisher ? finisherSettings.SmallOpponentPrefab : finisherSettings.SmallPrefab);
  }
}
