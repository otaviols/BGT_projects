using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Cysharp.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using PegasusGame;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spell : MonoBehaviour
{
  [Tooltip("If checked, this spell will block power history processing when the spell leaves the None state.")]
  public bool m_BlockServerEvents;
  [Tooltip("Additional configuration on when this spell should block power history processing")]
  public PowerProcessorBlockingBehavior m_BlockPowerProcessing;
  public GameObject m_ObjectContainer;
  public SpellLocation m_Location = SpellLocation.SOURCE_AUTO;
  public string m_LocationTransformName;
  public bool m_SetParentToLocation;
  public SpellFacing m_Facing;
  public SpellFacingOptions m_FacingOptions;
  public TARGET_RETICLE_TYPE m_TargetReticle;
  public List<SpellZoneTag> m_ZonesToDisable;
  [Tooltip("Delay (in seconds) to wait before sorting a zone after processing entity death. This is often used in CustomDeath spells, in order to wait for the custom death animation to play through before sorting the Play zone.")]
  public float m_ZoneLayoutDelayForDeaths;
  public bool m_UseFastActorTriggers;
  public bool m_ExclusivelyUseMetadataForTargeting;
  protected SpellType m_spellType;
  private Map<SpellStateType, List<SpellState>> m_spellStateMap;
  protected SpellStateType m_activeStateType;
  protected SpellStateType m_activeStateChange;
  private List<Spell.FinishedListener> m_finishedListeners = new List<Spell.FinishedListener>();
  private List<Spell.StateFinishedListener> m_stateFinishedListeners = new List<Spell.StateFinishedListener>();
  private List<Spell.StateStartedListener> m_stateStartedListeners = new List<Spell.StateStartedListener>();
  private List<Spell.SpellEventListener> m_spellEventListeners = new List<Spell.SpellEventListener>();
  private List<Spell.SpellReleasedListener> m_spellReleasedListeners = new List<Spell.SpellReleasedListener>();
  protected GameObject m_source;
  protected List<GameObject> m_targets = new List<GameObject>();
  protected PowerTaskList m_taskList;
  protected bool m_shown = true;
  protected PlayMakerFSM m_fsm;
  private Map<SpellStateType, FsmState> m_fsmStateMap;
  private bool m_fsmSkippedFirstFrame;
  private bool m_fsmReady;
  protected CancellationTokenSource m_fsmTokenSource;
  protected bool m_positionDirty = true;
  protected bool m_orientationDirty = true;
  protected bool m_finished;
  private int m_prefabHash = -1;
  private TransformProps m_defaultTransformProps;

  public bool IsPooled => this.PrefabHash != -1;

  public int PrefabHash => this.m_prefabHash;

  protected virtual void Awake()
  {
    this.BuildSpellStateMap();
    this.m_fsm = this.GetComponent<PlayMakerFSM>();
    if (string.IsNullOrEmpty(this.m_LocationTransformName))
      return;
    this.m_LocationTransformName = this.m_LocationTransformName.Trim();
  }

  protected virtual void OnDestroy()
  {
    if ((UnityEngine.Object) this.m_ObjectContainer != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_ObjectContainer);
      this.m_ObjectContainer = (GameObject) null;
    }
    if (!((UnityEngine.Object) this.gameObject != (UnityEngine.Object) null))
      return;
    this.StopAllAsyncs();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  protected virtual void Start()
  {
    if (this.m_activeStateType == SpellStateType.NONE)
      this.ActivateObjectContainer(false);
    else if (this.m_shown)
      this.ShowImpl();
    else
      this.HideImpl();
  }

  private void Update()
  {
    if (this.m_fsmReady)
      return;
    if ((UnityEngine.Object) this.m_fsm == (UnityEngine.Object) null)
      this.m_fsmReady = true;
    else if (!this.m_fsmSkippedFirstFrame)
    {
      this.m_fsmSkippedFirstFrame = true;
    }
    else
    {
      if (!this.m_fsm.enabled)
        return;
      this.BuildFsmStateMap();
      this.m_fsmReady = true;
    }
  }

  public SpellType GetSpellType() => this.m_spellType;

  public void SetSpellType(SpellType spellType) => this.m_spellType = spellType;

  public bool DoesBlockServerEvents() => GameState.Get() != null && this.m_BlockServerEvents;

  public SuperSpell GetSuperSpellParent() => (UnityEngine.Object) this.transform.parent == (UnityEngine.Object) null ? (SuperSpell) null : this.transform.parent.GetComponent<SuperSpell>();

  public PowerTaskList GetPowerTaskList() => this.m_taskList;

  public Entity GetPowerSource() => this.m_taskList == null ? (Entity) null : this.m_taskList.GetSourceEntity();

  public Card GetPowerSourceCard() => this.GetPowerSource()?.GetCard();

  public Entity GetPowerTarget() => this.m_taskList == null ? (Entity) null : this.m_taskList.GetTargetEntity();

  public Card GetPowerTargetCard() => this.GetPowerTarget()?.GetCard();

  public virtual bool CanPurge() => !this.IsActive();

  public virtual bool ShouldReconnectIfStuck() => true;

  public void UpdateParentActorComponents()
  {
    Transform parent1 = this.transform.parent;
    if ((UnityEngine.Object) parent1 == (UnityEngine.Object) null)
      return;
    Actor component = parent1.GetComponent<Actor>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Transform parent2 = parent1.parent;
      if ((UnityEngine.Object) parent2 != (UnityEngine.Object) null)
        component = parent2.GetComponent<Actor>();
    }
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.UpdateAllComponents();
  }

  public SpellLocation GetLocation() => this.m_Location;

  public string GetLocationTransformName() => this.m_LocationTransformName;

  public SpellFacing GetFacing() => this.m_Facing;

  public SpellFacingOptions GetFacingOptions() => this.m_FacingOptions;

  public void ClearPositionDirtyFlag() => this.m_positionDirty = false;

  public void SetPosition(Vector3 position)
  {
    this.transform.position = position;
    this.m_positionDirty = false;
  }

  public void SetLocalPosition(Vector3 position)
  {
    this.transform.localPosition = position;
    this.m_positionDirty = false;
  }

  public void SetOrientation(Quaternion orientation)
  {
    this.transform.rotation = orientation;
    this.m_orientationDirty = false;
  }

  public void SetLocalOrientation(Quaternion orientation)
  {
    this.transform.localRotation = orientation;
    this.m_orientationDirty = false;
  }

  public void ForceUpdateTransform()
  {
    this.m_positionDirty = true;
    this.UpdateTransform();
  }

  public void UpdateTransform()
  {
    this.UpdatePosition();
    this.UpdateOrientation();
  }

  public void UpdatePosition()
  {
    if (!this.m_positionDirty)
      return;
    SpellUtils.SetPositionFromLocation(this, this.m_SetParentToLocation);
    this.m_positionDirty = false;
  }

  public void UpdateOrientation()
  {
    if (!this.m_orientationDirty)
      return;
    SpellUtils.SetOrientationFromFacing(this);
    this.m_orientationDirty = false;
  }

  public GameObject GetSource() => this.m_source;

  public virtual void SetSource(GameObject go) => this.m_source = go;

  public virtual void RemoveSource() => this.m_source = (GameObject) null;

  public bool IsSource(GameObject go) => (UnityEngine.Object) this.m_source == (UnityEngine.Object) go;

  public Card GetSourceCard() => (UnityEngine.Object) this.m_source == (UnityEngine.Object) null ? (Card) null : this.m_source.GetComponent<Card>();

  public void InitializePooledSpell(int spellHashCode)
  {
    if (this.IsPooled)
    {
      Error.AddDevWarning("Spell Pooling", "Spell {0} has already been initialized for pooling", (object) this.name);
    }
    else
    {
      this.m_prefabHash = spellHashCode;
      this.m_defaultTransformProps = TransformUtil.GetLocalTransformProps((Component) this);
    }
  }

  private void ResetLocalPosition()
  {
    this.transform.localPosition = this.m_defaultTransformProps.position;
    this.transform.localScale = this.m_defaultTransformProps.scale;
    this.transform.localRotation = this.m_defaultTransformProps.rotation;
  }

  public void ResetSpellHash() => this.m_prefabHash = -1;

  public void ReleaseSpell()
  {
    this.FinishIfNecessary();
    this.FireSpellReleasedCallbacks();
    this.ResetSpellStates();
    this.RemoveSource();
    this.ClearAllListeners();
    this.RemoveAllTargets();
    this.ForceDeactivate();
    this.ResetLocalPosition();
  }

  private void ResetSpellStates()
  {
    if (this.m_spellStateMap == null)
      return;
    foreach (KeyValuePair<SpellStateType, List<SpellState>> spellState1 in this.m_spellStateMap)
    {
      foreach (SpellState spellState2 in spellState1.Value)
        spellState2.Reset();
    }
  }

  public List<GameObject> GetTargets() => this.m_targets;

  public GameObject GetTarget() => this.m_targets.Count != 0 ? this.m_targets[0] : (GameObject) null;

  public virtual void AddTarget(GameObject go) => this.m_targets.Add(go);

  public virtual void AddTargets(List<GameObject> targets) => this.m_targets.AddRange((IEnumerable<GameObject>) targets);

  public virtual bool RemoveTarget(GameObject go) => this.m_targets.Remove(go);

  public virtual void RemoveAllTargets() => this.m_targets.Clear();

  public bool IsTarget(GameObject go) => this.m_targets.Contains(go);

  public Card GetTargetCard()
  {
    GameObject target = this.GetTarget();
    return (UnityEngine.Object) target == (UnityEngine.Object) null ? (Card) null : target.GetComponent<Card>();
  }

  public virtual List<GameObject> GetVisualTargets() => this.GetTargets();

  public virtual GameObject GetVisualTarget() => this.GetTarget();

  public virtual void AddVisualTarget(GameObject go) => this.AddTarget(go);

  public virtual void AddVisualTargets(List<GameObject> targets) => this.AddTargets(targets);

  public virtual bool RemoveVisualTarget(GameObject go) => this.RemoveTarget(go);

  public virtual void RemoveAllVisualTargets() => this.RemoveAllTargets();

  public virtual bool IsVisualTarget(GameObject go) => this.IsTarget(go);

  public virtual Card GetVisualTargetCard() => this.GetTargetCard();

  public bool IsValidSpellTarget(Entity ent) => !ent.IsEnchantment();

  public bool IsValidSpellTarget(GameObject go)
  {
    Card component;
    return go.TryGetComponent<Card>(out component) && component.GetEntity() != null && this.IsValidSpellTarget(go.GetComponent<Card>().GetEntity());
  }

  public bool IsShown() => this.m_shown;

  public void Show()
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    if (this.m_activeStateType != SpellStateType.NONE)
      this.OnExitedNoneState();
    this.ShowImpl();
  }

  public void Hide()
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    this.HideImpl();
    if (this.m_activeStateType == SpellStateType.NONE)
      return;
    this.OnEnteredNoneState();
  }

  public void ActivateObjectContainer(bool enable)
  {
    if ((UnityEngine.Object) this.m_ObjectContainer == (UnityEngine.Object) null)
      return;
    RenderUtils.EnableRenderers(this.m_ObjectContainer, enable);
  }

  public bool IsActive() => this.m_activeStateType != 0;

  public void Activate()
  {
    SpellStateType stateType = this.GuessNextStateType();
    if (stateType == SpellStateType.NONE)
      this.Deactivate();
    else
      this.ChangeState(stateType);
  }

  public void Reactivate()
  {
    SpellStateType stateType = this.GuessNextStateType(SpellStateType.NONE);
    if (stateType == SpellStateType.NONE)
      this.Deactivate();
    else
      this.ChangeState(stateType);
  }

  public void Deactivate()
  {
    if (this.m_activeStateType == SpellStateType.NONE)
      return;
    this.ForceDeactivate();
  }

  public void ForceDeactivate() => this.ChangeState(SpellStateType.NONE);

  public void ActivateState(SpellStateType stateType)
  {
    if (!this.HasUsableState(stateType))
      this.Deactivate();
    else
      this.ChangeState(stateType);
  }

  public void SafeActivateState(SpellStateType stateType)
  {
    if (!this.HasUsableState(stateType))
      this.ForceDeactivate();
    else
      this.ChangeState(stateType);
  }

  public virtual bool HasUsableState(SpellStateType stateType) => stateType != SpellStateType.NONE && (this.HasStateContent(stateType) || this.HasOverriddenStateMethod(stateType) || this.m_activeStateType == SpellStateType.NONE && this.m_ZonesToDisable != null && this.m_ZonesToDisable.Count > 0);

  public SpellStateType GetActiveState() => this.m_activeStateType;

  public List<SpellState> GetSpellStates(SpellStateType stateType)
  {
    if (this.m_spellStateMap == null)
      return (List<SpellState>) null;
    List<SpellState> spellStateList = (List<SpellState>) null;
    return !this.m_spellStateMap.TryGetValue(stateType, out spellStateList) ? (List<SpellState>) null : spellStateList;
  }

  public List<SpellState> GetActiveStateList()
  {
    if (this.m_spellStateMap == null)
      return (List<SpellState>) null;
    List<SpellState> spellStateList = (List<SpellState>) null;
    return !this.m_spellStateMap.TryGetValue(this.m_activeStateType, out spellStateList) ? (List<SpellState>) null : spellStateList;
  }

  public bool IsFinished() => this.m_finished;

  public void AddFinishedCallback(Spell.FinishedCallback callback) => this.AddFinishedCallback(callback, (object) null);

  public void AddFinishedCallback(Spell.FinishedCallback callback, object userData)
  {
    Spell.FinishedListener finishedListener = new Spell.FinishedListener();
    finishedListener.SetCallback(callback);
    finishedListener.SetUserData(userData);
    if (this.m_finishedListeners.Contains(finishedListener))
      return;
    this.m_finishedListeners.Add(finishedListener);
  }

  public bool RemoveFinishedCallback(Spell.FinishedCallback callback) => this.RemoveFinishedCallback(callback, (object) null);

  public bool RemoveFinishedCallback(Spell.FinishedCallback callback, object userData)
  {
    Spell.FinishedListener finishedListener = new Spell.FinishedListener();
    finishedListener.SetCallback(callback);
    finishedListener.SetUserData(userData);
    return this.m_finishedListeners.Remove(finishedListener);
  }

  public void AddStateFinishedCallback(Spell.StateFinishedCallback callback) => this.AddStateFinishedCallback(callback, (object) null);

  public void AddStateFinishedCallback(Spell.StateFinishedCallback callback, object userData)
  {
    Spell.StateFinishedListener finishedListener = new Spell.StateFinishedListener();
    finishedListener.SetCallback(callback);
    finishedListener.SetUserData(userData);
    if (this.m_stateFinishedListeners.Contains(finishedListener))
      return;
    this.m_stateFinishedListeners.Add(finishedListener);
  }

  public bool RemoveStateFinishedCallback(Spell.StateFinishedCallback callback) => this.RemoveStateFinishedCallback(callback, (object) null);

  public bool RemoveStateFinishedCallback(Spell.StateFinishedCallback callback, object userData)
  {
    Spell.StateFinishedListener finishedListener = new Spell.StateFinishedListener();
    finishedListener.SetCallback(callback);
    finishedListener.SetUserData(userData);
    return this.m_stateFinishedListeners.Remove(finishedListener);
  }

  public void AddStateStartedCallback(Spell.StateStartedCallback callback) => this.AddStateStartedCallback(callback, (object) null);

  public void AddStateStartedCallback(Spell.StateStartedCallback callback, object userData)
  {
    Spell.StateStartedListener stateStartedListener = new Spell.StateStartedListener();
    stateStartedListener.SetCallback(callback);
    stateStartedListener.SetUserData(userData);
    if (this.m_stateStartedListeners.Contains(stateStartedListener))
      return;
    this.m_stateStartedListeners.Add(stateStartedListener);
  }

  public bool RemoveStateStartedCallback(Spell.StateStartedCallback callback) => this.RemoveStateStartedCallback(callback, (object) null);

  public bool RemoveStateStartedCallback(Spell.StateStartedCallback callback, object userData)
  {
    Spell.StateStartedListener stateStartedListener = new Spell.StateStartedListener();
    stateStartedListener.SetCallback(callback);
    stateStartedListener.SetUserData(userData);
    return this.m_stateStartedListeners.Remove(stateStartedListener);
  }

  public void AddSpellEventCallback(Spell.SpellEventCallback callback) => this.AddSpellEventCallback(callback, (object) null);

  public void AddSpellEventCallback(Spell.SpellEventCallback callback, object userData)
  {
    Spell.SpellEventListener spellEventListener = new Spell.SpellEventListener();
    spellEventListener.SetCallback(callback);
    spellEventListener.SetUserData(userData);
    if (this.m_spellEventListeners.Contains(spellEventListener))
      return;
    this.m_spellEventListeners.Add(spellEventListener);
  }

  public bool RemoveSpellEventCallback(Spell.SpellEventCallback callback) => this.RemoveSpellEventCallback(callback, (object) null);

  public bool RemoveSpellEventCallback(Spell.SpellEventCallback callback, object userData)
  {
    Spell.SpellEventListener spellEventListener = new Spell.SpellEventListener();
    spellEventListener.SetCallback(callback);
    spellEventListener.SetUserData(userData);
    return this.m_spellEventListeners.Remove(spellEventListener);
  }

  public void AddSpellReleasedCallback(Spell.SpellReleasedCallback callback)
  {
    Spell.SpellReleasedListener releasedListener = new Spell.SpellReleasedListener();
    releasedListener.SetCallback(callback);
    if (this.m_spellReleasedListeners.Contains(releasedListener))
      return;
    this.m_spellReleasedListeners.Add(releasedListener);
  }

  public bool RemoveSpellReleasedCallback(Spell.SpellReleasedCallback callback)
  {
    Spell.SpellReleasedListener releasedListener = new Spell.SpellReleasedListener();
    releasedListener.SetCallback(callback);
    return this.m_spellReleasedListeners.Remove(releasedListener);
  }

  private void ClearAllListeners()
  {
    this.m_finishedListeners.Clear();
    this.m_stateFinishedListeners.Clear();
    this.m_stateStartedListeners.Clear();
    this.m_spellEventListeners.Clear();
    this.m_spellReleasedListeners.Clear();
  }

  public virtual void ChangeState(SpellStateType stateType)
  {
    this.ChangeStateImpl(stateType);
    if (this.m_activeStateType != stateType)
      return;
    this.ChangeFsmState(stateType);
  }

  public SpellStateType GuessNextStateType() => this.GuessNextStateType(this.m_activeStateType);

  public SpellStateType GuessNextStateType(SpellStateType stateType)
  {
    switch (stateType)
    {
      case SpellStateType.NONE:
        if (this.HasUsableState(SpellStateType.BIRTH))
          return SpellStateType.BIRTH;
        if (this.HasUsableState(SpellStateType.IDLE))
          return SpellStateType.IDLE;
        if (this.HasUsableState(SpellStateType.ACTION))
          return SpellStateType.ACTION;
        if (this.HasUsableState(SpellStateType.DEATH))
          return SpellStateType.DEATH;
        if (this.HasUsableState(SpellStateType.CANCEL))
          return SpellStateType.CANCEL;
        break;
      case SpellStateType.BIRTH:
        if (this.HasUsableState(SpellStateType.IDLE))
          return SpellStateType.IDLE;
        break;
      case SpellStateType.IDLE:
        if (this.HasUsableState(SpellStateType.ACTION))
          return SpellStateType.ACTION;
        break;
      case SpellStateType.ACTION:
        if (this.HasUsableState(SpellStateType.DEATH))
          return SpellStateType.DEATH;
        break;
    }
    return SpellStateType.NONE;
  }

  public virtual bool AttachPowerTaskList(PowerTaskList taskList)
  {
    PowerTaskList taskList1 = this.m_taskList;
    this.m_taskList = taskList;
    this.RemoveAllTargets();
    if (!this.AddPowerTargets())
    {
      this.m_taskList = taskList1;
      return false;
    }
    this.OnAttachPowerTaskList();
    return true;
  }

  public virtual bool AddPowerTargets() => this.CanAddPowerTargets() && this.AddMultiplePowerTargets();

  public PowerTaskList GetLastHandledTaskList(PowerTaskList taskList)
  {
    if (taskList == null)
      return (PowerTaskList) null;
    Spell spell = UnityEngine.Object.Instantiate<Spell>(this);
    spell.SetSource(this.GetSource());
    PowerTaskList lastHandledTaskList = (PowerTaskList) null;
    for (PowerTaskList powerTaskList = taskList.GetLast(); powerTaskList != null; powerTaskList = powerTaskList.GetPrevious())
    {
      spell.m_taskList = powerTaskList;
      spell.RemoveAllTargets();
      if (spell.AddPowerTargets())
      {
        lastHandledTaskList = powerTaskList;
        break;
      }
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) spell);
    return lastHandledTaskList;
  }

  public bool IsHandlingLastTaskList() => this.GetLastHandledTaskList(this.m_taskList) == this.m_taskList;

  public virtual void OnStateFinished() => this.ChangeState(this.GuessNextStateType());

  public virtual void OnSpellFinished()
  {
    this.m_finished = true;
    if (GameState.Get() != null)
      GameState.Get().RemoveServerBlockingSpell(this);
    this.BlockZones(false);
    if (this.m_UseFastActorTriggers && GameState.Get() != null && this.IsHandlingLastTaskList())
      GameState.Get().SetUsingFastActorTriggers(false);
    this.FireFinishedCallbacks();
  }

  public virtual void OnSpellEvent(string eventName, object eventData) => this.FireSpellEventCallbacks(eventName, eventData);

  public virtual void OnFsmStateStarted(FsmState state, SpellStateType stateType)
  {
    if (this.m_activeStateChange == stateType)
      return;
    this.ChangeStateImpl(stateType);
  }

  protected virtual void OnAttachPowerTaskList()
  {
    if (!this.m_UseFastActorTriggers || !this.m_taskList.IsStartOfBlock())
      return;
    GameState.Get().SetUsingFastActorTriggers(true);
  }

  protected virtual void OnBirth(SpellStateType prevStateType)
  {
    this.UpdateTransform();
    this.FireStateStartedCallbacks(prevStateType);
  }

  protected virtual void OnIdle(SpellStateType prevStateType) => this.FireStateStartedCallbacks(prevStateType);

  protected virtual void OnAction(SpellStateType prevStateType)
  {
    this.UpdateTransform();
    this.FireStateStartedCallbacks(prevStateType);
  }

  protected virtual void OnCancel(SpellStateType prevStateType) => this.FireStateStartedCallbacks(prevStateType);

  protected virtual void OnDeath(SpellStateType prevStateType) => this.FireStateStartedCallbacks(prevStateType);

  protected virtual void OnNone(SpellStateType prevStateType) => this.FireStateStartedCallbacks(prevStateType);

  private void BuildSpellStateMap()
  {
    foreach (Component component1 in this.transform)
    {
      SpellState component2 = component1.gameObject.GetComponent<SpellState>();
      if (!((UnityEngine.Object) component2 == (UnityEngine.Object) null))
      {
        SpellStateType stateType = component2.m_StateType;
        if (stateType != SpellStateType.NONE)
        {
          if (this.m_spellStateMap == null)
            this.m_spellStateMap = new Map<SpellStateType, List<SpellState>>();
          List<SpellState> spellStateList;
          if (!this.m_spellStateMap.TryGetValue(stateType, out spellStateList))
          {
            spellStateList = new List<SpellState>();
            this.m_spellStateMap.Add(stateType, spellStateList);
          }
          spellStateList.Add(component2);
        }
      }
    }
  }

  private void BuildFsmStateMap()
  {
    if ((UnityEngine.Object) this.m_fsm == (UnityEngine.Object) null)
      return;
    List<FsmState> spellFsmStateList = this.GenerateSpellFsmStateList();
    if (spellFsmStateList.Count > 0)
      this.m_fsmStateMap = new Map<SpellStateType, FsmState>();
    Map<SpellStateType, int> map1 = new Map<SpellStateType, int>();
    foreach (SpellStateType key in Enum.GetValues(typeof (SpellStateType)))
      map1[key] = 0;
    Map<SpellStateType, int> map2 = new Map<SpellStateType, int>();
    foreach (SpellStateType key in Enum.GetValues(typeof (SpellStateType)))
      map2[key] = 0;
    foreach (FsmTransition globalTransition in this.m_fsm.FsmGlobalTransitions)
    {
      SpellStateType key;
      try
      {
        key = EnumUtils.GetEnum<SpellStateType>(globalTransition.EventName);
      }
      catch (ArgumentException ex)
      {
        continue;
      }
      ++map2[key];
      foreach (FsmState fsmState in spellFsmStateList)
      {
        if (globalTransition.ToState.Equals(fsmState.Name))
        {
          ++map1[key];
          if (!this.m_fsmStateMap.ContainsKey(key))
            this.m_fsmStateMap.Add(key, fsmState);
        }
      }
    }
    foreach (KeyValuePair<SpellStateType, int> keyValuePair in map1)
    {
      if (keyValuePair.Value > 1)
        Debug.LogWarning((object) string.Format("{0}.BuildFsmStateMap() - Found {1} states for SpellStateType {2}. There should be 1.", (object) this, (object) keyValuePair.Value, (object) keyValuePair.Key));
    }
    foreach (KeyValuePair<SpellStateType, int> keyValuePair in map2)
    {
      if (keyValuePair.Value > 1)
        Debug.LogWarning((object) string.Format("{0}.BuildFsmStateMap() - Found {1} transitions for SpellStateType {2}. There should be 1.", (object) this, (object) keyValuePair.Value, (object) keyValuePair.Key));
      if (keyValuePair.Value > 0 && map1[keyValuePair.Key] == 0)
        Debug.LogWarning((object) string.Format("{0}.BuildFsmStateMap() - SpellStateType {1} is missing a SpellStateAction.", (object) this, (object) keyValuePair.Key));
    }
    if (this.m_fsmStateMap == null || this.m_fsmStateMap.Values.Count != 0)
      return;
    this.m_fsmStateMap = (Map<SpellStateType, FsmState>) null;
  }

  private List<FsmState> GenerateSpellFsmStateList()
  {
    List<FsmState> spellFsmStateList = new List<FsmState>();
    foreach (FsmState fsmState in this.m_fsm.FsmStates)
    {
      SpellStateAction spellStateAction = (SpellStateAction) null;
      int num = 0;
      for (int index = 0; index < fsmState.Actions.Length; ++index)
      {
        if (fsmState.Actions[index] is SpellStateAction action)
        {
          ++num;
          if (spellStateAction == null)
            spellStateAction = action;
        }
      }
      if (spellStateAction != null)
        spellFsmStateList.Add(fsmState);
      if (num > 1)
        Debug.LogWarning((object) string.Format("{0}.GenerateSpellFsmStateList() - State \"{1}\" has {2} SpellStateActions. There should be 1.", (object) this, (object) fsmState.Name, (object) num));
    }
    return spellFsmStateList;
  }

  protected void ChangeStateImpl(SpellStateType stateType)
  {
    this.m_activeStateChange = stateType;
    SpellStateType activeStateType = this.m_activeStateType;
    this.m_activeStateType = stateType;
    if (stateType == SpellStateType.NONE)
      this.FinishIfNecessary();
    List<SpellState> nextStateList = (List<SpellState>) null;
    if (this.m_spellStateMap != null)
      this.m_spellStateMap.TryGetValue(stateType, out nextStateList);
    if (activeStateType != SpellStateType.NONE)
    {
      List<SpellState> spellStateList;
      if (this.m_spellStateMap != null && this.m_spellStateMap.TryGetValue(activeStateType, out spellStateList))
      {
        foreach (SpellState spellState in spellStateList)
          spellState.Stop(nextStateList);
      }
      this.FireStateFinishedCallbacks(activeStateType);
    }
    else if (stateType != SpellStateType.NONE)
    {
      this.m_finished = false;
      this.OnExitedNoneState();
    }
    if (nextStateList != null)
    {
      foreach (SpellState spellState in nextStateList)
        spellState.Play();
    }
    this.CallStateFunction(activeStateType, stateType);
    if (activeStateType == SpellStateType.NONE || stateType != SpellStateType.NONE)
      return;
    this.OnEnteredNoneState();
  }

  protected void ChangeFsmState(SpellStateType stateType)
  {
    if ((UnityEngine.Object) this.m_fsm == (UnityEngine.Object) null)
      return;
    if (!this.gameObject.activeInHierarchy)
    {
      Log.Spells.PrintWarning("Spell.ChangeFsmState() - WARNING gameObject {0} wants to go into state {1} but is inactive!", (object) this.gameObject, (object) stateType);
    }
    else
    {
      if (this.m_fsmTokenSource == null)
        this.m_fsmTokenSource = new CancellationTokenSource();
      this.WaitThenChangeFsmState(stateType, this.m_fsmTokenSource.Token).Forget();
    }
  }

  private async UniTaskVoid WaitThenChangeFsmState(
    SpellStateType stateType,
    CancellationToken token = default (CancellationToken))
  {
    while (!this.m_fsmReady)
      await UniTask.Yield(PlayerLoopTiming.Update, token);
    if (this.m_activeStateType != stateType)
      return;
    this.ChangeFsmStateNow(stateType);
  }

  protected virtual void StopAllAsyncs()
  {
    if (this.m_fsmTokenSource == null)
      return;
    this.m_fsmTokenSource.Cancel();
    this.m_fsmTokenSource.Dispose();
    this.m_fsmTokenSource = (CancellationTokenSource) null;
  }

  private void ChangeFsmStateNow(SpellStateType stateType)
  {
    if (this.m_fsmStateMap == null)
    {
      Debug.LogError((object) string.Format("Spell.ChangeFsmStateNow() - stateType {0}  was requested but the m_fsmStateMap for {1} is null", (object) stateType, (object) this.m_fsm.name));
    }
    else
    {
      FsmState fsmState = (FsmState) null;
      if (!this.m_fsmStateMap.TryGetValue(stateType, out fsmState))
        return;
      this.m_fsm.SendEvent(EnumUtils.GetString<SpellStateType>(stateType));
    }
  }

  protected void FinishIfNecessary()
  {
    if (this.m_finished)
      return;
    this.OnSpellFinished();
  }

  protected void CallStateFunction(SpellStateType prevStateType, SpellStateType stateType)
  {
    switch (stateType)
    {
      case SpellStateType.BIRTH:
        this.OnBirth(prevStateType);
        break;
      case SpellStateType.IDLE:
        this.OnIdle(prevStateType);
        break;
      case SpellStateType.ACTION:
        this.OnAction(prevStateType);
        break;
      case SpellStateType.CANCEL:
        this.OnCancel(prevStateType);
        break;
      case SpellStateType.DEATH:
        if (this.m_BlockPowerProcessing.m_OnEnterDeathState)
          GameState.Get().AddServerBlockingSpell(this);
        this.OnDeath(prevStateType);
        break;
      default:
        this.OnNone(prevStateType);
        break;
    }
  }

  protected void FireFinishedCallbacks()
  {
    for (int index = this.m_finishedListeners.Count - 1; index >= 0; --index)
      this.m_finishedListeners[index].Fire(this);
    this.m_finishedListeners.Clear();
  }

  protected void FireStateFinishedCallbacks(SpellStateType prevStateType)
  {
    for (int index = this.m_stateFinishedListeners.Count - 1; index >= 0; --index)
    {
      if (index < this.m_stateFinishedListeners.Count)
        this.m_stateFinishedListeners[index].Fire(this, prevStateType);
    }
    if (this.m_activeStateType != SpellStateType.NONE)
      return;
    this.m_stateFinishedListeners.Clear();
  }

  protected void FireStateStartedCallbacks(SpellStateType prevStateType)
  {
    for (int index = this.m_stateStartedListeners.Count - 1; index >= 0; --index)
      this.m_stateStartedListeners[index].Fire(this, prevStateType);
    if (this.m_activeStateType != SpellStateType.NONE)
      return;
    this.m_stateStartedListeners.Clear();
  }

  protected void FireSpellEventCallbacks(string eventName, object eventData)
  {
    for (int index = this.m_spellEventListeners.Count - 1; index >= 0; --index)
      this.m_spellEventListeners[index].Fire(eventName, eventData);
  }

  protected void FireSpellReleasedCallbacks()
  {
    for (int index = this.m_spellReleasedListeners.Count - 1; index >= 0; --index)
      this.m_spellReleasedListeners[index].Fire(this);
  }

  protected bool HasStateContent(SpellStateType stateType)
  {
    if (this.m_spellStateMap != null && this.m_spellStateMap.ContainsKey(stateType))
      return true;
    if (!this.m_fsmReady)
    {
      if ((UnityEngine.Object) this.m_fsm != (UnityEngine.Object) null && this.m_fsm.Fsm.HasEvent(EnumUtils.GetString<SpellStateType>(stateType)))
        return true;
    }
    else if (this.m_fsmStateMap != null && this.m_fsmStateMap.ContainsKey(stateType))
      return true;
    return false;
  }

  protected bool HasOverriddenStateMethod(SpellStateType stateType)
  {
    string stateMethodName = this.GetStateMethodName(stateType);
    return stateMethodName != null && GeneralUtils.IsOverriddenMethod(((object) this).GetType(), typeof (Spell), stateMethodName);
  }

  protected string GetStateMethodName(SpellStateType stateType)
  {
    switch (stateType)
    {
      case SpellStateType.BIRTH:
        return "OnBirth";
      case SpellStateType.IDLE:
        return "OnIdle";
      case SpellStateType.ACTION:
        return "OnAction";
      case SpellStateType.CANCEL:
        return "OnCancel";
      case SpellStateType.DEATH:
        return "OnDeath";
      default:
        return (string) null;
    }
  }

  protected bool CanAddPowerTargets() => SpellUtils.CanAddPowerTargets(this.m_taskList);

  protected bool AddSinglePowerTarget()
  {
    Card sourceCard = this.GetSourceCard();
    if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
    {
      Log.Power.PrintWarning("{0}.AddSinglePowerTarget() - a source card was never added", (object) this);
      return false;
    }
    Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
    if (blockStart == null)
    {
      Log.Power.PrintError("{0}.AddSinglePowerTarget() - got a task list with no block start", (object) this);
      return false;
    }
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    return this.AddSinglePowerTarget_FromBlockStart(blockStart) || this.AddSinglePowerTarget_FromMetaData(taskList) || this.AddSinglePowerTarget_FromAnyPower(sourceCard, taskList);
  }

  protected bool AddSinglePowerTarget_FromBlockStart(Network.HistBlockStart blockStart)
  {
    Entity entity = GameState.Get().GetEntity(blockStart.Target);
    if (entity == null)
      return false;
    Card card = entity.GetCard();
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
    {
      Log.Power.Print("{0}.AddSinglePowerTarget_FromSourceAction() - FAILED Target {1} in blockStart has no Card", (object) this, (object) blockStart.Target);
      return false;
    }
    this.AddTarget(card.gameObject);
    return true;
  }

  protected bool AddSinglePowerTarget_FromMetaData(List<PowerTask> tasks)
  {
    GameState gameState = GameState.Get();
    for (int index1 = 0; index1 < tasks.Count; ++index1)
    {
      Network.PowerHistory power = tasks[index1].GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.TARGET)
        {
          if (histMetaData.Info == null || histMetaData.Info.Count == 0)
          {
            Debug.LogError((object) string.Format("{0}.AddSinglePowerTarget_FromMetaData() - META_DATA at index {1} has no Info", (object) this, (object) index1));
          }
          else
          {
            for (int index2 = 0; index2 < histMetaData.Info.Count; ++index2)
            {
              Entity entity = gameState.GetEntity(histMetaData.Info[index2]);
              if (entity == null)
              {
                Debug.LogError((object) string.Format("{0}.AddSinglePowerTarget_FromMetaData() - Entity is null for META_DATA at index {1} Info index {2}", (object) this, (object) index1, (object) index2));
              }
              else
              {
                Card card = entity.GetCard();
                this.AddTargetFromMetaData(index1, card);
                return true;
              }
            }
          }
        }
      }
    }
    return false;
  }

  protected bool AddSinglePowerTarget_FromAnyPower(Card sourceCard, List<PowerTask> tasks)
  {
    for (int index = 0; index < tasks.Count; ++index)
    {
      PowerTask task = tasks[index];
      Card cardFromPowerTask = this.GetTargetCardFromPowerTask(index, task);
      if (!((UnityEngine.Object) cardFromPowerTask == (UnityEngine.Object) null) && !((UnityEngine.Object) sourceCard == (UnityEngine.Object) cardFromPowerTask) && this.IsValidSpellTarget(cardFromPowerTask.GetEntity()))
      {
        this.AddTarget(cardFromPowerTask.gameObject);
        return true;
      }
    }
    return false;
  }

  protected bool AddMultiplePowerTargets()
  {
    Card sourceCard = this.GetSourceCard();
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    if (this.AddMultiplePowerTargets_FromMetaData(taskList) || this.m_ExclusivelyUseMetadataForTargeting)
      return true;
    this.AddMultiplePowerTargets_FromAnyPower(sourceCard, taskList);
    return true;
  }

  protected bool AddMultiplePowerTargets_FromMetaData(List<PowerTask> tasks)
  {
    int count = this.m_targets.Count;
    GameState gameState = GameState.Get();
    for (int index1 = 0; index1 < tasks.Count; ++index1)
    {
      Network.PowerHistory power = tasks[index1].GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.TARGET)
        {
          if (histMetaData.Info == null || histMetaData.Info.Count == 0)
          {
            Debug.LogError((object) string.Format("{0}.AddMultiplePowerTargets_FromMetaData() - META_DATA at index {1} has no Info", (object) this, (object) index1));
          }
          else
          {
            int data = histMetaData.Data;
            if (data == 0 || !((UnityEngine.Object) this.GetSourceCard() == (UnityEngine.Object) null) && this.GetSourceCard().GetEntity() != null && data == this.GetSourceCard().GetEntity().GetEntityId())
            {
              for (int index2 = 0; index2 < histMetaData.Info.Count; ++index2)
              {
                Entity entity = gameState.GetEntity(histMetaData.Info[index2]);
                if (entity == null)
                {
                  Debug.LogError((object) string.Format("{0}.AddMultiplePowerTargets_FromMetaData() - Entity is null for META_DATA at index {1} Info index {2}", (object) this, (object) index1, (object) index2));
                }
                else
                {
                  Card card = entity.GetCard();
                  this.AddTargetFromMetaData(index1, card);
                }
              }
            }
          }
        }
      }
    }
    return this.m_targets.Count != count;
  }

  protected void AddMultiplePowerTargets_FromAnyPower(Card sourceCard, List<PowerTask> tasks)
  {
    for (int index = 0; index < tasks.Count; ++index)
    {
      PowerTask task = tasks[index];
      Card cardFromPowerTask = this.GetTargetCardFromPowerTask(index, task);
      if (!((UnityEngine.Object) cardFromPowerTask == (UnityEngine.Object) null) && !((UnityEngine.Object) sourceCard == (UnityEngine.Object) cardFromPowerTask) && !this.IsTarget(cardFromPowerTask.gameObject) && this.IsValidSpellTarget(cardFromPowerTask.GetEntity()))
        this.AddTarget(cardFromPowerTask.gameObject);
    }
  }

  protected virtual Card GetTargetCardFromPowerTask(int index, PowerTask task)
  {
    Network.PowerHistory power = task.GetPower();
    if (power.Type != Network.PowerType.TAG_CHANGE)
      return (Card) null;
    Network.HistTagChange histTagChange = power as Network.HistTagChange;
    Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
    if (entity != null)
      return entity.GetCard();
    Debug.LogWarning((object) string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity));
    return (Card) null;
  }

  protected virtual void AddTargetFromMetaData(int metaDataIndex, Card targetCard) => this.AddTarget(targetCard.gameObject);

  protected bool CompleteMetaDataTasks(int metaDataIndex) => this.CompleteMetaDataTasks(metaDataIndex, (PowerTaskList.CompleteCallback) null, (object) null);

  protected bool CompleteMetaDataTasks(
    int metaDataIndex,
    PowerTaskList.CompleteCallback completeCallback)
  {
    return this.CompleteMetaDataTasks(metaDataIndex, completeCallback, (object) null);
  }

  protected bool CompleteMetaDataTasks(
    int metaDataIndex,
    PowerTaskList.CompleteCallback completeCallback,
    object callbackData)
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    int count = 1;
    for (int index = metaDataIndex + 1; index < taskList.Count; ++index)
    {
      Network.PowerHistory power = taskList[index].GetPower();
      if (power.Type != Network.PowerType.META_DATA || ((Network.HistMetaData) power).MetaType != HistoryMeta.Type.TARGET)
        ++count;
      else
        break;
    }
    if (count == 0)
    {
      Debug.LogError((object) string.Format("{0}.CompleteMetaDataTasks() - there are no tasks to complete for meta data {1}", (object) this, (object) metaDataIndex));
      return false;
    }
    this.m_taskList.DoTasks(metaDataIndex, count, completeCallback, callbackData);
    return true;
  }

  protected virtual void ShowImpl()
  {
    List<SpellState> activeStateList = this.GetActiveStateList();
    if (activeStateList == null)
      return;
    foreach (SpellState spellState in activeStateList)
      spellState.ShowState();
  }

  protected virtual void HideImpl()
  {
    List<SpellState> activeStateList = this.GetActiveStateList();
    if (activeStateList == null)
      return;
    foreach (SpellState spellState in activeStateList)
      spellState.HideState();
  }

  protected void OnExitedNoneState()
  {
    if (this.DoesBlockServerEvents())
      GameState.Get().AddServerBlockingSpell(this);
    this.ActivateObjectContainer(true);
    this.BlockZones(true);
    if (!((UnityEngine.Object) ZoneMgr.Get() != (UnityEngine.Object) null))
      return;
    ZoneMgr.Get().RequestNextDeathBlockLayoutDelaySec(this.m_ZoneLayoutDelayForDeaths);
  }

  protected void OnEnteredNoneState()
  {
    if (GameState.Get() != null)
      GameState.Get().RemoveServerBlockingSpell(this);
    this.ActivateObjectContainer(false);
  }

  protected void BlockZones(bool block)
  {
    if (this.m_ZonesToDisable == null)
      return;
    foreach (SpellZoneTag zoneTag in this.m_ZonesToDisable)
    {
      List<Zone> zonesFromTag = SpellUtils.FindZonesFromTag(zoneTag);
      if (zonesFromTag != null)
      {
        foreach (Zone zone in zonesFromTag)
          zone.BlockInput(block);
      }
    }
  }

  public void OnLoad()
  {
    foreach (Component component1 in this.transform)
    {
      SpellState component2 = component1.gameObject.GetComponent<SpellState>();
      if (!((UnityEngine.Object) component2 == (UnityEngine.Object) null))
        component2.OnLoad();
    }
  }

  public delegate void FinishedCallback(Spell spell, object userData);

  public delegate void StateFinishedCallback(
    Spell spell,
    SpellStateType prevStateType,
    object userData);

  public delegate void StateStartedCallback(
    Spell spell,
    SpellStateType prevStateType,
    object userData);

  public delegate void SpellEventCallback(string eventName, object eventData, object userData);

  public delegate void SpellReleasedCallback(Spell spell);

  private class FinishedListener : EventListener<Spell.FinishedCallback>
  {
    public void Fire(Spell spell) => this.m_callback(spell, this.m_userData);
  }

  private class StateFinishedListener : EventListener<Spell.StateFinishedCallback>
  {
    public void Fire(Spell spell, SpellStateType prevStateType) => this.m_callback(spell, prevStateType, this.m_userData);
  }

  private class StateStartedListener : EventListener<Spell.StateStartedCallback>
  {
    public void Fire(Spell spell, SpellStateType prevStateType) => this.m_callback(spell, prevStateType, this.m_userData);
  }

  private class SpellEventListener : EventListener<Spell.SpellEventCallback>
  {
    public void Fire(string eventName, object eventData) => this.m_callback(eventName, eventData, this.m_userData);
  }

  private class SpellReleasedListener : EventListener<Spell.SpellReleasedCallback>
  {
    public void Fire(Spell spell) => this.m_callback(spell);
  }
}
